using System;
using AppKit;
using CoreGraphics;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Comet.MacOS;

namespace Comet.Handlers
{
	public partial class CollectionViewHandler : ViewHandler<IListView, CollectionViewHandler.CollectionViewNSContainer>
	{
		protected override CollectionViewNSContainer CreatePlatformView() => new CollectionViewNSContainer();

		public static void MapListViewProperty(IElementHandler handler, IListView virtualView)
		{
			var cvHandler = (CollectionViewHandler)handler;
			cvHandler._mauiCollectionView = CreateAndConfigureMauiCollectionView(virtualView);
			cvHandler.EmbedMauiCollectionView();
		}

#nullable enable
		public static void MapReloadData(CollectionViewHandler handler, IListView virtualView, object? value)
#nullable restore
		{
			if (handler._mauiCollectionView != null)
				RefreshItemsSource(handler._mauiCollectionView, virtualView);
		}

		void EmbedMauiCollectionView()
		{
			if (_mauiCollectionView == null || MauiContext == null)
				return;

			try
			{
				var platformView = _mauiCollectionView.ToMacOSPlatform(MauiContext);
				PlatformView.SetContent(platformView, _mauiCollectionView);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[CollectionViewHandler.MacOS] EmbedMauiCollectionView failed: {ex.Message}");
			}
		}

		protected override void DisconnectHandler(CollectionViewNSContainer platformView)
		{
			platformView.ClearContent();
			_mauiCollectionView = null;
			base.DisconnectHandler(platformView);
		}

		public override Microsoft.Maui.Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return new Microsoft.Maui.Graphics.Size(
				double.IsInfinity(widthConstraint) ? 400 : widthConstraint,
				double.IsInfinity(heightConstraint) ? 600 : heightConstraint);
		}

		public class CollectionViewNSContainer : NSView
		{
			NSView _contentView;
			IView _virtualView;

			public CollectionViewNSContainer() { WantsLayer = true; }

			public void SetContent(NSView platformView, IView virtualView)
			{
				_contentView?.RemoveFromSuperview();
				_contentView = platformView;
				_virtualView = virtualView;
				if (_contentView != null)
				{
					_contentView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
					AddSubview(_contentView);
					NeedsLayout = true;
				}
			}

			public void ClearContent()
			{
				_contentView?.RemoveFromSuperview();
				_contentView = null;
				_virtualView = null;
			}

			public override void Layout()
			{
				base.Layout();
				if (_contentView == null || Bounds.Width <= 0 || Bounds.Height <= 0)
					return;

				_virtualView?.Measure(Bounds.Width, Bounds.Height);
				_virtualView?.Arrange(new Microsoft.Maui.Graphics.Rect(0, 0, Bounds.Width, Bounds.Height));
				_contentView.Frame = Bounds;
			}
		}
	}
}
