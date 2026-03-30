using System;
using Android.Views;
using Android.Widget;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.HotReload;
using Microsoft.Maui.Platform;

namespace Comet.Handlers;

/// <summary>
/// Android handler for CometHost. Creates a container FrameLayout that hosts
/// the Comet View's rendered body content (typically a MauiViewHost).
/// </summary>
public partial class CometHostHandler : ViewHandler<CometHost, CometHostHandler.CometHostContainerView>
{
	public CometHostHandler() : base(CometHostMapper) { }

	protected override CometHostContainerView CreatePlatformView()
		=> new CometHostContainerView(Context);

	protected override void ConnectHandler(CometHostContainerView platformView)
	{
		base.ConnectHandler(platformView);
		UpdateCometView();
	}

	protected override void DisconnectHandler(CometHostContainerView platformView)
	{
		platformView.ClearContent();
		base.DisconnectHandler(platformView);
	}

	public override Microsoft.Maui.Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		if (VirtualView is IContentView contentView)
		{
			var size = contentView.CrossPlatformMeasure(widthConstraint, heightConstraint);
			if (size.Width > 0 && size.Height > 0)
				return size;
		}
		var w = double.IsInfinity(widthConstraint) ? 400 : widthConstraint;
		var h = double.IsInfinity(heightConstraint) ? 800 : heightConstraint;
		return new Microsoft.Maui.Graphics.Size(w, h);
	}

	void UpdateCometView()
	{
		if (VirtualView?.CometView == null || MauiContext == null)
			return;

		try
		{
			var cometView = VirtualView.CometView;

			// Give the container the context it needs to re-render on Reload()
			PlatformView.MauiContext = MauiContext;
			PlatformView.SetCometView(cometView);

			// Set the reload handler so Component.SetState → Reload() can
			// notify the host to re-render the platform view tree.
			if (cometView is Microsoft.Maui.HotReload.IHotReloadableView ihr)
			{
				ihr.ReloadHandler = PlatformView;
			}
			
			// Get the render view (body content) to avoid CometViewHandler handler circularity
			var renderView = cometView.GetView();
			IView viewToRender = (renderView != null && renderView != cometView) ? renderView : cometView;
			
			var platformView = viewToRender.ToPlatform(MauiContext);
			if (platformView != null)
				PlatformView.SetContent(platformView, viewToRender);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[CometHostHandler] UpdateCometView failed: {ex.Message}");
		}
	}

	public class CometHostContainerView : FrameLayout, IReloadHandler
	{
		global::Android.Views.View _contentView;
		IView _virtualView;
		View _cometView;

		public CometHostContainerView(global::Android.Content.Context context) : base(context) { }

		internal IMauiContext MauiContext { get; set; }

		internal void SetCometView(View cometView)
		{
			_cometView = cometView;
		}

		public void SetContent(global::Android.Views.View platformView, IView virtualView)
		{
			if (_contentView != null)
				RemoveView(_contentView);
			_contentView = platformView;
			_virtualView = virtualView;
			if (_contentView != null)
			{
				AddView(_contentView, new FrameLayout.LayoutParams(
					ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
			}
		}

		public void ClearContent()
		{
			if (_contentView != null)
				RemoveView(_contentView);
			_contentView = null;
			_virtualView = null;
		}

		/// <summary>
		/// Called when the Comet view's state changes (Component.SetState → Reload).
		/// After ResetView diffs the virtual tree and transfers handlers, we just
		/// need to tell the platform to re-measure and re-layout with the updated
		/// virtual view tree. We also update the virtual view reference so
		/// measurement uses the current render output.
		/// </summary>
		public void Reload()
		{
			if (_cometView == null || MauiContext == null) return;

			var renderView = _cometView.GetView();
			IView viewToRender = (renderView != null && renderView != _cometView) ? renderView : _cometView;

			// After ResetView's DiffUpdate, handlers have been transferred from
			// old to new views. The virtual tree is correct but the platform
			// ViewGroup children may be stale (pointing at old child instances).
			// Do a top-down sync of the entire platform view tree to match
			// the new virtual tree.
			var existingHandler = viewToRender.Handler;
			if (existingHandler?.PlatformView is global::Android.Views.View existingPlatformView)
			{
				if (existingPlatformView != _contentView)
				{
					SetContent(existingPlatformView, viewToRender);
				}
				else
				{
					_virtualView = viewToRender;
				}

				// Recursively sync platform children to match virtual tree
				SyncViewTreePlatformChildren(viewToRender);

				// Invalidate all measurements so the next layout pass recalculates
				InvalidateViewTreeMeasurements(viewToRender);

				// Request a full layout pass from the platform
				_contentView?.RequestLayout();
				RequestLayout();
			}
			else
			{
				// No handler yet — need full platform view creation
				var platformView = viewToRender.ToPlatform(MauiContext);
				if (platformView != null)
					SetContent(platformView, viewToRender);
			}
		}

		/// <summary>
		/// Recursively sync platform ViewGroup children to match the Comet
		/// virtual view tree. Called once after the entire diff completes.
		/// </summary>
		static void SyncViewTreePlatformChildren(IView view)
		{
			if (view is not Comet.ContainerView container) return;
			if (view.Handler?.PlatformView is not global::Android.Views.ViewGroup platformGroup) return;

			var virtualChildren = ((IContainerView)container).GetChildren();

			// Collect expected platform children
			var expectedPlatformChildren = new List<global::Android.Views.View>();
			foreach (var child in virtualChildren)
			{
				if (child?.ViewHandler?.PlatformView is global::Android.Views.View childPV)
					expectedPlatformChildren.Add(childPV);
			}

			// Check if sync is needed
			bool needsSync = platformGroup.ChildCount != expectedPlatformChildren.Count;
			if (!needsSync)
			{
				for (int i = 0; i < expectedPlatformChildren.Count; i++)
				{
					if (platformGroup.GetChildAt(i) != expectedPlatformChildren[i])
					{
						needsSync = true;
						break;
					}
				}
			}

			if (needsSync)
			{
				platformGroup.RemoveAllViews();
				foreach (var childPV in expectedPlatformChildren)
				{
					if (childPV.Parent is global::Android.Views.ViewGroup oldParent && oldParent != platformGroup)
						oldParent.RemoveView(childPV);
					if (childPV.Parent == null)
						platformGroup.AddView(childPV);
				}
				platformGroup.RequestLayout();
				platformGroup.Invalidate();
			}

			// Recurse into children
			foreach (var child in virtualChildren)
			{
				if (child != null)
					SyncViewTreePlatformChildren(child);
			}
		}

		static void InvalidateViewTreeMeasurements(IView view)
		{
			if (view is Comet.View cometView)
				cometView.InvalidateMeasurement();
			if (view is Comet.ContainerView container)
			{
				foreach (var child in ((IContainerView)container).GetChildren())
				{
					if (child != null)
						InvalidateViewTreeMeasurements(child);
				}
			}
		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);
			if (_contentView == null) return;

			var width = right - left;
			var height = bottom - top;
			if (width > 0 && height > 0 && _virtualView != null)
			{
				var density = Context?.Resources?.DisplayMetrics?.Density ?? 1;
				var widthDp = width / density;
				var heightDp = height / density;
				_virtualView.Measure(widthDp, heightDp);
				_virtualView.Arrange(new Microsoft.Maui.Graphics.Rect(0, 0, widthDp, heightDp));
			}
		}
	}
}
