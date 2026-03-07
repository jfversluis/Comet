using System;
using System.Collections.Generic;
using Comet.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui;
using Microsoft.Maui.Animations;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;

#if WINDOWS
using Microsoft.Maui.Graphics.Win2D;
#endif

namespace Comet
{
	public static class AppHostBuilderExtensions
	{
		static void AddHandlers(this IMauiHandlersCollection collection, Dictionary<Type, Type> handlers) => handlers.ForEach(x => collection.AddHandler(x.Key, x.Value));
		public static MauiAppBuilder UseCometApp<TApp>(this MauiAppBuilder builder)
			where TApp : class, IApplication
		{
			builder.Services.TryAddSingleton<IApplication, TApp>();
			builder.UseCometHandlers();
			return builder;
		}
		public static MauiAppBuilder UseCometHandlers(this MauiAppBuilder builder)
		{

			//AnimationManger.SetTicker(new iOSTicker());

			//Set Default Style
			var style = new Styles.Style();
			style.Apply();

			ViewHandler.ViewMapper.AppendToMapping(nameof(IGestureView.Gestures), CometViewHandler.AddGestures);
			ViewHandler.ViewCommandMapper.AppendToMapping(Gesture.AddGestureProperty, CometViewHandler.AddGesture);
			ViewHandler.ViewCommandMapper.AppendToMapping(Gesture.RemoveGestureProperty, CometViewHandler.RemoveGesture);

			// Apply shadow to any view that has it set via environment
			ViewHandler.ViewMapper.AppendToMapping("CometShadow", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var shadow = cometView.GetEnvironment<Comet.Graphics.Shadow>(EnvironmentKeys.View.Shadow);
				if (shadow == null)
					return;
#if __IOS__ || MACCATALYST
				var platformView = handler.PlatformView as UIKit.UIView;
				if (platformView == null)
					return;
				var layer = platformView.Layer;
				layer.ShadowOpacity = shadow.Opacity;
				layer.ShadowRadius = shadow.Radius;
				layer.ShadowOffset = new CoreGraphics.CGSize(shadow.Offset.X, shadow.Offset.Y);
				if (shadow.Paint is SolidPaint sp && sp.Color != null)
					layer.ShadowColor = sp.Color.ToPlatform().CGColor;
				else
					layer.ShadowColor = UIKit.UIColor.Black.CGColor;
				layer.MasksToBounds = false;
#elif ANDROID
				var platformView = handler.PlatformView as global::Android.Views.View;
				if (platformView == null)
					return;
				var density = platformView.Context?.Resources?.DisplayMetrics?.Density ?? 1;
				platformView.Elevation = shadow.Radius * density;
#endif
			});

			// Apply border visual styling to Border's platform view via handler mapper
			LayoutHandler.Mapper.AppendToMapping("CometBorderStyling", (handler, view) =>
			{
				if (view is not Border border)
					return;
				var borderStroke = (IBorderStroke)border;
				var platformView = handler.PlatformView;
				if (platformView == null)
					return;
#if __IOS__ || MACCATALYST
				var layer = platformView.Layer;
				if (borderStroke.Shape is RoundedRectangle rr)
				{
					layer.CornerRadius = rr.CornerRadius;
				}
				else if (borderStroke.Shape != null)
				{
					layer.CornerRadius = 0;
				}
				layer.MasksToBounds = true;
				if (borderStroke.Stroke is SolidPaint sp && sp.Color != null)
				{
					layer.BorderColor = sp.Color.ToPlatform().CGColor;
					layer.BorderWidth = (float)borderStroke.StrokeThickness;
				}
				else
				{
					layer.BorderWidth = 0;
				}
				var bg = border.GetBackground();
				if (bg is SolidPaint bgPaint && bgPaint.Color != null)
				{
					layer.BackgroundColor = bgPaint.Color.ToPlatform().CGColor;
				}
#elif ANDROID
				var context = platformView.Context;
				if (context != null)
				{
					var drawable = new global::Android.Graphics.Drawables.GradientDrawable();
					if (borderStroke.Shape is RoundedRectangle rr)
						drawable.SetCornerRadius((float)(rr.CornerRadius * context.Resources.DisplayMetrics.Density));
					if (borderStroke.Stroke is SolidPaint sp && sp.Color != null)
						drawable.SetStroke((int)(borderStroke.StrokeThickness * context.Resources.DisplayMetrics.Density), sp.Color.ToPlatform());
					var bg = border.GetBackground();
					if (bg is SolidPaint bgPaint && bgPaint.Color != null)
						drawable.SetColor(bgPaint.Color.ToPlatform());
					platformView.Background = drawable;
				}
#endif
			});

			// Apply PlaceholderColor to TextField/SecureField via handler mapper
			EntryHandler.Mapper.AppendToMapping("CometPlaceholderColor", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var color = cometView.GetEnvironment<Color>(EnvironmentKeys.Entry.PlaceholderColor);
				if (color == null)
					return;
				var entry = handler.PlatformView;
				if (entry == null)
					return;
#if __IOS__ || MACCATALYST
				entry.AttributedPlaceholder = new Foundation.NSAttributedString(
					entry.Placeholder ?? "",
					new UIKit.UIStringAttributes { ForegroundColor = color.ToPlatform() });
#elif ANDROID
				entry.SetHintTextColor(new global::Android.Content.Res.ColorStateList(
					new[] { Array.Empty<int>() },
					new[] { (int)color.ToPlatform() }));
#endif
			});

			// Apply Keyboard type to TextField via handler mapper
			EntryHandler.Mapper.AppendToMapping("CometKeyboard", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var keyboard = cometView.GetEnvironment<Microsoft.Maui.Keyboard>(EnvironmentKeys.Entry.Keyboard);
				if (keyboard == null)
					return;
				var entry = handler.PlatformView;
				if (entry == null)
					return;
#if __IOS__ || MACCATALYST
				entry.ApplyKeyboard(keyboard);
#elif ANDROID
				// Android keyboard handled via MAUI's IEntry.Keyboard interface
				if (view is IEntry entryView)
					EntryHandler.MapKeyboard(handler, entryView);
#endif
			});

			// Apply ReturnType to TextField via handler mapper
			EntryHandler.Mapper.AppendToMapping("CometReturnType", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var returnType = cometView.GetEnvironment<ReturnType?>(EnvironmentKeys.Entry.ReturnType);
				if (returnType == null)
					return;
				var entry = handler.PlatformView;
				if (entry == null)
					return;
#if __IOS__ || MACCATALYST
				entry.ReturnKeyType = returnType.Value switch
				{
					ReturnType.Go => UIKit.UIReturnKeyType.Go,
					ReturnType.Next => UIKit.UIReturnKeyType.Next,
					ReturnType.Search => UIKit.UIReturnKeyType.Search,
					ReturnType.Send => UIKit.UIReturnKeyType.Send,
					ReturnType.Done => UIKit.UIReturnKeyType.Done,
					_ => UIKit.UIReturnKeyType.Default,
				};
#endif
			});

			// Apply OnColor/ThumbColor to Toggle/Switch via handler mapper
			SwitchHandler.Mapper.AppendToMapping("CometSwitchColors", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var platformView = handler.PlatformView;
				if (platformView == null)
					return;
#if __IOS__ || MACCATALYST
				var onColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Switch.OnColor);
				if (onColor != null)
					platformView.OnTintColor = onColor.ToPlatform();
				var thumbColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Switch.ThumbColor);
				if (thumbColor != null)
					platformView.ThumbTintColor = thumbColor.ToPlatform();
#elif ANDROID
				var onColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Switch.OnColor);
				if (onColor != null)
					platformView.TrackTintList = new global::Android.Content.Res.ColorStateList(
						new[] { new[] { global::Android.Resource.Attribute.StateChecked } },
						new[] { (int)onColor.ToPlatform() });
				var thumbColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Switch.ThumbColor);
				if (thumbColor != null)
					platformView.ThumbTintList = new global::Android.Content.Res.ColorStateList(
						new[] { Array.Empty<int>() },
						new[] { (int)thumbColor.ToPlatform() });
#endif
			});

			// Apply Slider track/thumb colors via handler mapper
			SliderHandler.Mapper.AppendToMapping("CometSliderColors", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var platformView = handler.PlatformView;
				if (platformView == null)
					return;
#if __IOS__ || MACCATALYST
				var minTrackColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Slider.ProgressColor);
				if (minTrackColor != null)
					platformView.MinimumTrackTintColor = minTrackColor.ToPlatform();
				var maxTrackColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Slider.TrackColor);
				if (maxTrackColor != null)
					platformView.MaximumTrackTintColor = maxTrackColor.ToPlatform();
				var thumbColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Slider.ThumbColor);
				if (thumbColor != null)
					platformView.ThumbTintColor = thumbColor.ToPlatform();
#elif ANDROID
				var minTrackColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Slider.ProgressColor);
				if (minTrackColor != null && platformView.ProgressTintList != null)
					platformView.ProgressTintList = global::Android.Content.Res.ColorStateList.ValueOf(
						new global::Android.Graphics.Color((int)minTrackColor.ToPlatform()));
				var thumbColor = cometView.GetEnvironment<Color>(EnvironmentKeys.Slider.ThumbColor);
				if (thumbColor != null)
					platformView.ThumbTintList = global::Android.Content.Res.ColorStateList.ValueOf(
						new global::Android.Graphics.Color((int)thumbColor.ToPlatform()));
#endif
			});

			// Apply Aspect to Image via handler mapper
			ImageHandler.Mapper.AppendToMapping("CometImageAspect", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var aspect = cometView.GetEnvironment<Microsoft.Maui.Aspect?>(EnvironmentKeys.Image.Aspect);
				if (aspect == null)
					return;
				var platformView = handler.PlatformView;
				if (platformView == null)
					return;
#if __IOS__ || MACCATALYST
				platformView.ContentMode = aspect.Value switch
				{
					Microsoft.Maui.Aspect.AspectFit => UIKit.UIViewContentMode.ScaleAspectFit,
					Microsoft.Maui.Aspect.AspectFill => UIKit.UIViewContentMode.ScaleAspectFill,
					Microsoft.Maui.Aspect.Fill => UIKit.UIViewContentMode.ScaleToFill,
					_ => UIKit.UIViewContentMode.ScaleAspectFit
				};
#elif ANDROID
				platformView.SetScaleType(aspect.Value switch
				{
					Microsoft.Maui.Aspect.AspectFit => global::Android.Widget.ImageView.ScaleType.FitCenter,
					Microsoft.Maui.Aspect.AspectFill => global::Android.Widget.ImageView.ScaleType.CenterCrop,
					Microsoft.Maui.Aspect.Fill => global::Android.Widget.ImageView.ScaleType.FitXy,
					_ => global::Android.Widget.ImageView.ScaleType.FitCenter
				});
#endif
			});

			// Apply PlaceholderColor and Keyboard to TextEditor (Editor) via handler mapper
			EditorHandler.Mapper.AppendToMapping("CometEditorPlaceholderColor", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var color = cometView.GetEnvironment<Color>(EnvironmentKeys.Entry.PlaceholderColor);
				if (color == null)
					return;
				var editor = handler.PlatformView;
				if (editor == null)
					return;
#if __IOS__ || MACCATALYST
				// iOS UITextView doesn't have a built-in placeholder; MAUI handles it
				// through the EditorHandler. We can set the placeholder color via attributed string.
#elif ANDROID
				editor.SetHintTextColor(new global::Android.Content.Res.ColorStateList(
					new[] { Array.Empty<int>() },
					new[] { (int)color.ToPlatform() }));
#endif
			});

			EditorHandler.Mapper.AppendToMapping("CometEditorKeyboard", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var keyboard = cometView.GetEnvironment<Microsoft.Maui.Keyboard>(EnvironmentKeys.Entry.Keyboard);
				if (keyboard == null)
					return;
				var editor = handler.PlatformView;
				if (editor == null)
					return;
#if __IOS__ || MACCATALYST
				editor.ApplyKeyboard(keyboard);
#elif ANDROID
				if (view is IEditor editorView)
					EditorHandler.MapKeyboard(handler, editorView);
#endif
			});

			// Apply ProgressBar track/progress colors via handler mapper
			ProgressBarHandler.Mapper.AppendToMapping("CometProgressBarColors", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var platformView = handler.PlatformView;
				if (platformView == null)
					return;
#if __IOS__ || MACCATALYST
				var progressColor = cometView.GetEnvironment<Color>(EnvironmentKeys.ProgressBar.ProgressColor);
				if (progressColor != null)
					platformView.ProgressTintColor = progressColor.ToPlatform();
				var trackColor = cometView.GetEnvironment<Color>(EnvironmentKeys.ProgressBar.TrackColor);
				if (trackColor != null)
					platformView.TrackTintColor = trackColor.ToPlatform();
#elif ANDROID
				var progressColor = cometView.GetEnvironment<Color>(EnvironmentKeys.ProgressBar.ProgressColor);
				if (progressColor != null)
					platformView.ProgressTintList = global::Android.Content.Res.ColorStateList.ValueOf(
						new global::Android.Graphics.Color((int)progressColor.ToPlatform()));
				var trackColor = cometView.GetEnvironment<Color>(EnvironmentKeys.ProgressBar.TrackColor);
				if (trackColor != null)
					platformView.ProgressBackgroundTintList = global::Android.Content.Res.ColorStateList.ValueOf(
						new global::Android.Graphics.Color((int)trackColor.ToPlatform()));
#endif
			});

			// Apply DatePicker format via handler mapper
			DatePickerHandler.Mapper.AppendToMapping("CometDatePickerFormat", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var format = cometView.GetEnvironment<string>(EnvironmentKeys.DatePicker.Format);
				if (string.IsNullOrEmpty(format))
					return;
				if (view is IDatePicker datePicker)
				{
					// Format is handled by MAUI's IDatePicker.Format property
					// The environment value is read by the generated DatePicker class
				}
			});

			// Apply DatePicker text color via handler mapper
			DatePickerHandler.Mapper.AppendToMapping("CometDatePickerTextColor", (handler, view) =>
			{
				if (view is not View cometView)
					return;
				var color = cometView.GetEnvironment<Color>(EnvironmentKeys.DatePicker.TextColor);
				if (color == null)
					return;
				var platformView = handler.PlatformView;
				if (platformView == null)
					return;
#if __IOS__ || MACCATALYST
				// UIDatePicker uses tintColor for text color on iOS 15+
				platformView.TintColor = color.ToPlatform();
#elif ANDROID
				platformView.SetTextColor(color.ToPlatform());
#endif
			});

			Lerp.Lerps[typeof(FrameConstraints)] = new Lerp
			{
				Calculate = (s, e, progress) => {
					var start = (FrameConstraints)s;
					var end = (FrameConstraints)(e);
					return start.Lerp(end, progress);
				}
			};
			builder.ConfigureMauiHandlers((handlersCollection) => handlersCollection.AddHandlers(new Dictionary<Type, Type>
			{
				{ typeof(AbstractLayout), typeof(LayoutHandler) },
				{ typeof(AbsoluteLayout), typeof(LayoutHandler) },
				{ typeof(FlexLayout), typeof(LayoutHandler) },
				{ typeof(ActivityIndicator), typeof(ActivityIndicatorHandler) },
				{ typeof(Border), typeof(LayoutHandler) },
			{ typeof(MauiViewHost), typeof(Handlers.MauiViewHostHandler) },
				{ typeof(Button), typeof(ButtonHandler) },
				{ typeof(CheckBox), typeof(CheckBoxHandler) },
				{ typeof(CometWindow), typeof(WindowHandler) },
				{ typeof(DatePicker), typeof(DatePickerHandler) },
				{ typeof(FlyoutView), typeof(FlyoutViewHandler) },
				{ typeof(Frame), typeof(ContentViewHandler) },
				{ typeof(GraphicsView), typeof(GraphicsViewHandler) },
				{ typeof(Image) , typeof(ImageHandler) },
				{ typeof(ImageButton) , typeof(ImageButtonHandler) },
				{ typeof(IndicatorView), typeof(IndicatorViewHandler) },
				{ typeof(Picker), typeof(PickerHandler) },
				{ typeof(ProgressBar), typeof(ProgressBarHandler) },
				{ typeof(RadioButton), typeof(Microsoft.Maui.Handlers.RadioButtonHandler) },
				{ typeof(RadioGroup), typeof(LayoutHandler) },
				{ typeof(RefreshView), typeof(RefreshViewHandler) },
				{ typeof(SearchBar), typeof(SearchBarHandler) },
				{ typeof(SecureField), typeof(EntryHandler) },
				{ typeof(Slider), typeof(SliderHandler) },
				{ typeof(Stepper), typeof(StepperHandler) },
				{ typeof(Spacer), typeof(SpacerHandler) },
				{ typeof(SwipeView), typeof(SwipeViewHandler) },
				{ typeof(TabView), typeof(TabViewHandler) },
				{ typeof(TextEditor), typeof(EditorHandler) },
				{ typeof(TextField), typeof(EntryHandler) },
				{ typeof(Text), typeof(LabelHandler) },
				{ typeof(TimePicker), typeof(TimePickerHandler) },
				{ typeof(Toggle), typeof(SwitchHandler) },
				{ typeof(Toolbar), typeof(ToolbarHandler) },
				{ typeof(CometApp), typeof(ApplicationHandler) },
				{ typeof(ListView),typeof(ListViewHandler) },
				{ typeof(CollectionView),typeof(Handlers.CollectionViewHandler) },
				{ typeof(CarouselView),typeof(Handlers.CollectionViewHandler) },
				{ typeof(BoxView), typeof(Handlers.ShapeViewHandler) },
#if __MOBILE__
				{typeof(ScrollView), typeof(Handlers.ScrollViewHandler) },
				{typeof(ShapeView), typeof(Handlers.ShapeViewHandler)},
#else
				
				{typeof(ScrollView), typeof(Microsoft.Maui.Handlers.ScrollViewHandler) },
#endif


#if __IOS__
				{typeof(NavigationView), typeof (Handlers.NavigationViewHandler)},
				{typeof(View), typeof(CometViewHandler)},
#else
				
				{typeof(NavigationView), typeof (Microsoft.Maui.Handlers.NavigationViewHandler)},
#endif
				{typeof(WebView), typeof(Microsoft.Maui.Handlers.WebViewHandler)},
				{typeof(MenuBar), typeof(Microsoft.Maui.Handlers.MenuBarHandler)},
				{typeof(MenuBarItem), typeof(Microsoft.Maui.Handlers.MenuBarItemHandler)},
				{typeof(MenuFlyoutItem), typeof(Microsoft.Maui.Handlers.MenuFlyoutItemHandler)},
				{typeof(MenuFlyoutSubItem), typeof(Microsoft.Maui.Handlers.MenuFlyoutSubItemHandler)},
				{typeof(MenuFlyoutSeparator), typeof(Microsoft.Maui.Handlers.MenuFlyoutSeparatorHandler)},
			}));

			// Register standard MAUI Controls handlers for MauiViewHost embedding.
			// These enable Microsoft.Maui.Controls types (Label, Entry, Border, etc.)
			// to be rendered when hosted inside a Comet view tree via MauiViewHost.
			// Interface-based registrations are critical for third-party controls (e.g. Syncfusion)
			// that implement IContentView but don't extend ContentView directly.
			builder.ConfigureMauiHandlers((handlersCollection) =>
			{
				// Interface-based handler registrations (matches MAUI's own registrations)
				handlersCollection.TryAddHandler<IContentView, Microsoft.Maui.Handlers.ContentViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Label, Microsoft.Maui.Handlers.LabelHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Entry, Microsoft.Maui.Handlers.EntryHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Editor, Microsoft.Maui.Handlers.EditorHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Button, Microsoft.Maui.Handlers.ButtonHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.CheckBox, Microsoft.Maui.Handlers.CheckBoxHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Switch, Microsoft.Maui.Handlers.SwitchHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Slider, Microsoft.Maui.Handlers.SliderHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Stepper, Microsoft.Maui.Handlers.StepperHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Picker, Microsoft.Maui.Handlers.PickerHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.DatePicker, Microsoft.Maui.Handlers.DatePickerHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.TimePicker, Microsoft.Maui.Handlers.TimePickerHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Image, Microsoft.Maui.Handlers.ImageHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.ImageButton, Microsoft.Maui.Handlers.ImageButtonHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.SearchBar, Microsoft.Maui.Handlers.SearchBarHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.ProgressBar, Microsoft.Maui.Handlers.ProgressBarHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.ActivityIndicator, Microsoft.Maui.Handlers.ActivityIndicatorHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.RadioButton, Microsoft.Maui.Handlers.RadioButtonHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Border, Microsoft.Maui.Handlers.BorderHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.BoxView, Microsoft.Maui.Handlers.ShapeViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.ContentView, Microsoft.Maui.Handlers.ContentViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Layout, Microsoft.Maui.Handlers.LayoutHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Frame, Microsoft.Maui.Handlers.BorderHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.ScrollView, Microsoft.Maui.Handlers.ScrollViewHandler>();
				handlersCollection.AddHandler<CometHost, Handlers.CometHostHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Grid, Microsoft.Maui.Handlers.LayoutHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.StackLayout, Microsoft.Maui.Handlers.LayoutHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.HorizontalStackLayout, Microsoft.Maui.Handlers.LayoutHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.VerticalStackLayout, Microsoft.Maui.Handlers.LayoutHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.FlexLayout, Microsoft.Maui.Handlers.LayoutHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.AbsoluteLayout, Microsoft.Maui.Handlers.LayoutHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.RefreshView, Microsoft.Maui.Handlers.RefreshViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.SwipeView, Microsoft.Maui.Handlers.SwipeViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.IndicatorView, Microsoft.Maui.Handlers.IndicatorViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.WebView, Microsoft.Maui.Handlers.WebViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.Page, Microsoft.Maui.Handlers.PageHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.ContentPage, Microsoft.Maui.Handlers.PageHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.NavigationPage, Microsoft.Maui.Handlers.NavigationViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.TabbedPage, Microsoft.Maui.Handlers.TabbedViewHandler>();
				handlersCollection.TryAddHandler<Microsoft.Maui.Controls.FlyoutPage, Microsoft.Maui.Handlers.FlyoutViewHandler>();
			});


			ThreadHelper.SetFireOnMainThread(MainThread.BeginInvokeOnMainThread);

			return builder;
		}


	}
}