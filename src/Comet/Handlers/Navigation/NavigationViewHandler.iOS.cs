using Comet.iOS;
using CoreGraphics;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Comet.Handlers
{
	public partial class NavigationViewHandler : ViewHandler<NavigationView, UIView>, IPlatformViewHandler
	{
		UIViewController viewController;
		UIViewController IPlatformViewHandler.ViewController => viewController;
		protected override UIView CreatePlatformView()
		{
			var vc = new Comet.iOS.CometViewController { MauiContext = MauiContext, CurrentView = VirtualView.Content };
			var nav = VirtualView;
			if (nav.Navigation != null)
			{
				viewController = vc;
				return viewController.View;
			}
			var navigationController = new CUINavigationController();
			viewController = navigationController;

			nav.SetPerformNavigate((toView) => {
				if (toView is NavigationView newNav)
				{
					newNav.SetPerformNavigate(nav);
					newNav.SetPerformPop(nav);
				}

				toView.Navigation = nav;
				var newVc = new Comet.iOS.CometViewController { MauiContext = MauiContext, CurrentView = toView };
				navigationController.PushViewController(newVc, true);
			});
			nav.SetPerformPop(() => navigationController.PopViewController(true));
			navigationController.PushViewController(vc, true);

			// Add leading bar button (hamburger icon) if configured
			if (nav.LeadingBarAction != null)
			{
				var action = nav.LeadingBarAction;
				vc.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(
					nav.LeadingBarIcon ?? "☰",
					UIBarButtonItemStyle.Plain,
					(s, e) => action());
			}

			// Add toolbar items as right bar button items
			if (nav.ToolbarItems.Count > 0)
			{
				var rightItems = new List<UIBarButtonItem>();
				foreach (var item in nav.ToolbarItems)
				{
					if (item.Order == ToolbarItemOrder.Secondary) continue;
					var toolbarAction = item.OnClicked;
					UIBarButtonItem barItem;

					// Render font icon as UIImage if font family is specified
					if (!string.IsNullOrEmpty(item.IconGlyph) && !string.IsNullOrEmpty(item.IconFontFamily))
					{
						var image = CreateFontIconImage(item.IconGlyph, item.IconFontFamily, 24);
						if (image != null)
						{
							barItem = new UIBarButtonItem(
								image,
								UIBarButtonItemStyle.Plain,
								(s, e) => toolbarAction?.Invoke());
						}
						else
						{
							barItem = new UIBarButtonItem(
								item.IconGlyph ?? item.Text ?? "",
								UIBarButtonItemStyle.Plain,
								(s, e) => toolbarAction?.Invoke());
						}
					}
					// Use SF Symbol name if glyph looks like an SF Symbol identifier
					else if (!string.IsNullOrEmpty(item.IconGlyph) && item.IconGlyph.Contains('.'))
					{
						var sfImage = UIImage.GetSystemImage(item.IconGlyph);
						if (sfImage != null)
						{
							barItem = new UIBarButtonItem(
								sfImage,
								UIBarButtonItemStyle.Plain,
								(s, e) => toolbarAction?.Invoke());
						}
						else
						{
							barItem = new UIBarButtonItem(
								item.Text ?? item.IconGlyph,
								UIBarButtonItemStyle.Plain,
								(s, e) => toolbarAction?.Invoke());
						}
					}
					else
					{
						barItem = new UIBarButtonItem(
							item.IconGlyph ?? item.Text ?? "",
							UIBarButtonItemStyle.Plain,
							(s, e) => toolbarAction?.Invoke());
					}
					barItem.Enabled = item.IsEnabled;
					rightItems.Add(barItem);
				}
				if (rightItems.Count > 0)
					vc.NavigationItem.RightBarButtonItems = rightItems.ToArray();
			}

			return navigationController.View;
		}

		protected override void ConnectHandler(UIView platformView)
		{
			base.ConnectHandler(platformView);
			ApplyNavigationBarBackground();
		}

		void ApplyNavigationBarBackground()
		{
			if (viewController is CUINavigationController navController)
			{
				var bgPaint = VirtualView?.GetBackground();
				if (bgPaint is Microsoft.Maui.Graphics.SolidPaint solid && solid.Color != null)
				{
					var uiColor = solid.Color.ToPlatform();
					var appearance = new UINavigationBarAppearance();
					appearance.ConfigureWithOpaqueBackground();
					appearance.BackgroundColor = uiColor;
					appearance.ShadowColor = UIColor.Clear;
					navController.NavigationBar.StandardAppearance = appearance;
					navController.NavigationBar.ScrollEdgeAppearance = appearance;
					navController.NavigationBar.CompactAppearance = appearance;
				}
			}
		}

		static UIImage CreateFontIconImage(string glyph, string fontFamily, nfloat size)
		{
			var font = UIFont.FromName(fontFamily, size);
			if (font == null)
				return null;

			var text = new Foundation.NSString(glyph);
			var attributes = new UIStringAttributes { Font = font };
			var textSize = text.GetSizeUsingAttributes(attributes);
			if (textSize.Width <= 0 || textSize.Height <= 0)
				return null;

			UIGraphics.BeginImageContextWithOptions(textSize, false, 0);
			text.DrawString(CoreGraphics.CGPoint.Empty, attributes);
			var image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return image?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
		}
	}
}
