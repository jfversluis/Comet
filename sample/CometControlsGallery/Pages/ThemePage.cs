using System;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using MauiAppTheme = Microsoft.Maui.ApplicationModel.AppTheme;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ThemePageState
	{
		public string ThemeInfo { get; set; } = "";
		public bool IsDark { get; set; }
	}

	public class ThemePage : Component<ThemePageState>
	{
		public ThemePage()
		{
			UpdateThemeInfo();
		}

		public override View Render()
		{
			var titleLabel = Text("App Theme Support")
				.FontSize(24)
				.FontWeight(FontWeight.Bold);

			var themeInfoLabel = GalleryPageHelpers.BodyText(State.ThemeInfo);

			var themedLabel = Text("I respond to theme changes")
				.FontSize(18)
				.FontWeight(FontWeight.Bold)
				.Color(State.IsDark ? Colors.White : Colors.Black)
				.Background(State.IsDark ? Color.FromArgb("#333333") : Color.FromArgb("#E8E8E8"))
				.Padding(new Thickness(20, 12));

			var themedBox = new Spacer()
				.Frame(width: 200, height: 60)
				.Background(State.IsDark ? Colors.DarkOrange : Colors.CornflowerBlue);

			var lightBtn = Button("Force Light", () =>
			{
				if (Microsoft.Maui.Controls.Application.Current != null)
					Microsoft.Maui.Controls.Application.Current.UserAppTheme = MauiAppTheme.Light;
				UpdateThemeInfo();
			});

			var darkBtn = Button("Force Dark", () =>
			{
				if (Microsoft.Maui.Controls.Application.Current != null)
					Microsoft.Maui.Controls.Application.Current.UserAppTheme = MauiAppTheme.Dark;
				UpdateThemeInfo();
			});

			var systemBtn = Button("Follow System", () =>
			{
				if (Microsoft.Maui.Controls.Application.Current != null)
					Microsoft.Maui.Controls.Application.Current.UserAppTheme = MauiAppTheme.Unspecified;
				UpdateThemeInfo();
			});

			return GalleryPageHelpers.Scaffold("Theme",
				VStack(16,
					titleLabel,
					themeInfoLabel,
					themedLabel,
					themedBox,
					HStack(8,
						lightBtn,
						darkBtn,
						systemBtn
					)
				)
			);
		}

		void UpdateThemeInfo()
		{
			var app = Microsoft.Maui.Controls.Application.Current;
			if (app == null)
			{
				SetState(s =>
				{
					s.ThemeInfo = "Application not available";
					s.IsDark = false;
				});
				return;
			}

			var platformTheme = app.PlatformAppTheme;
			var userTheme = app.UserAppTheme;
			var effectiveTheme = app.RequestedTheme;

			SetState(s =>
			{
				s.ThemeInfo = $"Platform: {platformTheme} | User: {userTheme} | Effective: {effectiveTheme}";
				s.IsDark = effectiveTheme == MauiAppTheme.Dark;
			});
		}
	}
}
