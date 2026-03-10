using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using MauiAppTheme = Microsoft.Maui.ApplicationModel.AppTheme;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public enum ThemeSelectionMode
	{
		Light,
		Dark,
		System
	}

	public class ThemePageState
	{
		public ThemeSelectionMode Mode { get; set; }
		public bool IsDarkTheme { get; set; }
	}

	public class ThemePage : Component<ThemePageState>
	{
		public override View Render() =>
			GalleryPageHelpers.Scaffold("Theme",
				GalleryPageHelpers.Section("Theme Mode", "Toggle light, dark, or system behavior for the gallery sample.",
					HStack(8,
						ThemeButton("Light", ThemeSelectionMode.Light),
						ThemeButton("Dark", ThemeSelectionMode.Dark),
						ThemeButton("System", ThemeSelectionMode.System)
					),
					GalleryPageHelpers.BodyText(GetThemeLabel()),
					Text(() => State.IsDarkTheme ? "Dark theme colors are active." : "Light theme colors are active.")
						.Color(() => State.IsDarkTheme
							? ColorTokens.Tertiary.Resolve(ThemeManager.Current())
							: ColorTokens.Primary.Resolve(ThemeManager.Current())),
					Border(Text(""))
						.Background(() => State.IsDarkTheme
							? ColorTokens.SecondaryContainer.Resolve(ThemeManager.Current())
							: ColorTokens.PrimaryContainer.Resolve(ThemeManager.Current()))
						.CornerRadius(18)
						.Frame(height: 72)
				),
				BuildColorSwatchSection(),
				BuildTypographySection(),
				GalleryPageHelpers.Section("Fonts", "Open the fonts demo from the Theme navigation stack.",
					GalleryPageHelpers.NavButton("Fonts →", () => Comet.NavigationView.Navigate(this, new FontsPage()))
				)
			);

		View ThemeButton(string label, ThemeSelectionMode mode) =>
			Button(label, () => ApplyTheme(mode))
				.ButtonStyle(State.Mode == mode ? ButtonStyles.Filled : ButtonStyles.Outlined);

		View BuildColorSwatchSection() =>
			GalleryPageHelpers.Section("Color Tokens", "Material 3 color token swatches from the current theme.",
				SwatchRow(
					TokenSwatch("Primary", ColorTokens.Primary, ColorTokens.OnPrimary),
					TokenSwatch("OnPrimary", ColorTokens.OnPrimary, ColorTokens.Primary),
					TokenSwatch("PrimaryCtr", ColorTokens.PrimaryContainer, ColorTokens.OnPrimaryContainer)
				),
				SwatchRow(
					TokenSwatch("Secondary", ColorTokens.Secondary, ColorTokens.OnSecondary),
					TokenSwatch("OnSecondary", ColorTokens.OnSecondary, ColorTokens.Secondary),
					TokenSwatch("SecondaryCtr", ColorTokens.SecondaryContainer, ColorTokens.OnSecondaryContainer)
				),
				SwatchRow(
					TokenSwatch("Tertiary", ColorTokens.Tertiary, ColorTokens.OnTertiary),
					TokenSwatch("OnTertiary", ColorTokens.OnTertiary, ColorTokens.Tertiary),
					TokenSwatch("TertiaryCtr", ColorTokens.TertiaryContainer, ColorTokens.OnTertiaryContainer)
				),
				SwatchRow(
					TokenSwatch("Error", ColorTokens.Error, ColorTokens.OnError),
					TokenSwatch("OnError", ColorTokens.OnError, ColorTokens.Error),
					TokenSwatch("ErrorCtr", ColorTokens.ErrorContainer, ColorTokens.OnErrorContainer)
				),
				SwatchRow(
					TokenSwatch("Surface", ColorTokens.Surface, ColorTokens.OnSurface),
					TokenSwatch("SurfaceVar", ColorTokens.SurfaceVariant, ColorTokens.OnSurfaceVariant),
					TokenSwatch("SurfaceCtr", ColorTokens.SurfaceContainer, ColorTokens.OnSurface)
				)
			);

		View BuildTypographySection() =>
			GalleryPageHelpers.Section("Typography", "The current typography scale from title to label sizes.",
				TypographySample("DisplaySmall", TypographyTokens.DisplaySmall),
				TypographySample("HeadlineSmall", TypographyTokens.HeadlineSmall),
				TypographySample("TitleLarge", TypographyTokens.TitleLarge),
				TypographySample("BodyLarge", TypographyTokens.BodyLarge),
				TypographySample("LabelLarge", TypographyTokens.LabelLarge)
			);

		static View TokenSwatch(string name, Token<Color> token, Token<Color> textToken) =>
			Border(
				Text(name)
					.FontSize(10)
					.Color(textToken)
					.HorizontalTextAlignment(TextAlignment.Center)
			)
			.Background(token)
			.CornerRadius(8)
			.Frame(width: 90, height: 50);

		static View SwatchRow(params View[] swatches) => HStack(8, swatches);

		static View TypographySample(string name, Token<FontSpec> token) =>
			VStack(4,
				Text(name)
					.Typography(TypographyTokens.LabelSmall)
					.Color(ColorTokens.OnSurfaceVariant),
				Text("The quick brown fox")
					.Typography(token)
					.Color(ColorTokens.OnSurface)
			);

		string GetThemeLabel()
		{
			var descriptor = State.Mode switch
			{
				ThemeSelectionMode.Light => "Light",
				ThemeSelectionMode.Dark => "Dark",
				_ => $"System ({(State.IsDarkTheme ? "Dark" : "Light")})"
			};

			return $"Current theme: {descriptor}";
		}

		void ApplyTheme(ThemeSelectionMode mode)
		{
			var requestedTheme = Microsoft.Maui.Controls.Application.Current?.RequestedTheme ?? MauiAppTheme.Light;
			var isDark = mode == ThemeSelectionMode.Dark || (mode == ThemeSelectionMode.System && requestedTheme == MauiAppTheme.Dark);

			if (Microsoft.Maui.Controls.Application.Current != null)
			{
				Microsoft.Maui.Controls.Application.Current.UserAppTheme = mode switch
				{
					ThemeSelectionMode.Light => MauiAppTheme.Light,
					ThemeSelectionMode.Dark => MauiAppTheme.Dark,
					_ => MauiAppTheme.Unspecified
				};
			}

			var theme = isDark ? Defaults.Dark : Defaults.Light;
			theme.CurrentTheme = mode switch
			{
				ThemeSelectionMode.Light => Comet.Styles.AppTheme.Light,
				ThemeSelectionMode.Dark => Comet.Styles.AppTheme.Dark,
				_ => Comet.Styles.AppTheme.System
			};

			Theme.Current = theme;

			SetState(s =>
			{
				s.Mode = mode;
				s.IsDarkTheme = isDark;
			});
		}
	}
}
