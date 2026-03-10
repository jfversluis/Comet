using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ThemeState
	{
		public bool IsDarkTheme { get; set; }
		public bool IsActionEnabled { get; set; } = true;
		public int StyleTapCount { get; set; }
	}

	public class ThemePage : Component<ThemeState>
	{
		static readonly SectionCard Card = new();

		public override View Render()
		{
			return NavigationView(
				ScrollView(
					VStack(24,
						BuildThemeToggleSection(),
						BuildColorSwatchSection(),
						BuildTypographySection(),
						BuildButtonStylesSection(),
						BuildViewModifierSection(),
						BuildControlStateSection()
					)
					.Padding(new Thickness(24))
				)
				.Background(ColorTokens.Background)
			)
			.Title("Theme");
		}

		View BuildSection(string title, string description, params View[] content)
		{
			var items = new List<View>
			{
				Text(title)
					.Typography(TypographyTokens.TitleLarge)
					.Color(ColorTokens.OnSurface),
				Text(description)
					.Typography(TypographyTokens.BodyMedium)
					.Color(ColorTokens.OnSurfaceVariant)
					.LineBreakMode(LineBreakMode.WordWrap)
			};
			items.AddRange(content);
			return Border(VStack(12, items.ToArray())).Modifier(Card);
		}

		// --- 1. Theme Toggle ---

		View BuildThemeToggleSection() =>
			BuildSection("Theme Toggle", "Switch between light and dark themes.",
				HStack(12,
					Toggle(State.IsDarkTheme)
						.OnColor(ColorTokens.Primary.Resolve(ThemeManager.Current()))
						.OnToggled(isDark => SetState(s =>
						{
							s.IsDarkTheme = isDark;
							Theme.Current = isDark ? Defaults.Dark : Defaults.Light;
						})),
					Text(State.IsDarkTheme ? "Dark" : "Light")
						.Typography(TypographyTokens.LabelLarge)
						.Color(ColorTokens.OnSurface)
				)
			);

		// --- 2. Color Token Swatches ---

		View BuildColorSwatchSection() =>
			BuildSection("Color Tokens", "Material 3 color token swatches from the current theme.",
				// Primary group
				SwatchRow(
					TokenSwatch("Primary", ColorTokens.Primary, ColorTokens.OnPrimary),
					TokenSwatch("OnPrimary", ColorTokens.OnPrimary, ColorTokens.Primary),
					TokenSwatch("PrimaryCtr", ColorTokens.PrimaryContainer, ColorTokens.OnPrimaryContainer)
				),
				// Secondary group
				SwatchRow(
					TokenSwatch("Secondary", ColorTokens.Secondary, ColorTokens.OnSecondary),
					TokenSwatch("OnSecondary", ColorTokens.OnSecondary, ColorTokens.Secondary),
					TokenSwatch("SecondaryCtr", ColorTokens.SecondaryContainer, ColorTokens.OnSecondaryContainer)
				),
				// Tertiary group
				SwatchRow(
					TokenSwatch("Tertiary", ColorTokens.Tertiary, ColorTokens.OnTertiary),
					TokenSwatch("OnTertiary", ColorTokens.OnTertiary, ColorTokens.Tertiary),
					TokenSwatch("TertiaryCtr", ColorTokens.TertiaryContainer, ColorTokens.OnTertiaryContainer)
				),
				// Error group
				SwatchRow(
					TokenSwatch("Error", ColorTokens.Error, ColorTokens.OnError),
					TokenSwatch("OnError", ColorTokens.OnError, ColorTokens.Error),
					TokenSwatch("ErrorCtr", ColorTokens.ErrorContainer, ColorTokens.OnErrorContainer)
				),
				// Surface group
				SwatchRow(
					TokenSwatch("Surface", ColorTokens.Surface, ColorTokens.OnSurface),
					TokenSwatch("SurfaceVar", ColorTokens.SurfaceVariant, ColorTokens.OnSurfaceVariant),
					TokenSwatch("SurfaceCtr", ColorTokens.SurfaceContainer, ColorTokens.OnSurface)
				),
				// Background & Outline group
				SwatchRow(
					TokenSwatch("Background", ColorTokens.Background, ColorTokens.OnBackground),
					TokenSwatch("Outline", ColorTokens.Outline, ColorTokens.Surface),
					TokenSwatch("InverseSrf", ColorTokens.InverseSurface, ColorTokens.InverseOnSurface)
				)
			);

		View TokenSwatch(string name, Token<Color> token, Token<Color> textToken) =>
			Border(
				Text(name)
					.FontSize(10)
					.Color(textToken)
					.HorizontalTextAlignment(TextAlignment.Center)
			)
			.Background(token)
			.CornerRadius(8)
			.Frame(width: 90, height: 50);

		View SwatchRow(params View[] swatches) =>
			HStack(8, swatches);

		// --- 3. Typography Scale ---

		View BuildTypographySection() =>
			BuildSection("Typography Scale", "Material 3 type scale from DisplayLarge to LabelSmall.",
				TypographySample("DisplayLarge", TypographyTokens.DisplayLarge),
				TypographySample("DisplayMedium", TypographyTokens.DisplayMedium),
				TypographySample("DisplaySmall", TypographyTokens.DisplaySmall),
				TypographySample("HeadlineLarge", TypographyTokens.HeadlineLarge),
				TypographySample("HeadlineMedium", TypographyTokens.HeadlineMedium),
				TypographySample("HeadlineSmall", TypographyTokens.HeadlineSmall),
				TypographySample("TitleLarge", TypographyTokens.TitleLarge),
				TypographySample("TitleMedium", TypographyTokens.TitleMedium),
				TypographySample("TitleSmall", TypographyTokens.TitleSmall),
				TypographySample("BodyLarge", TypographyTokens.BodyLarge),
				TypographySample("BodyMedium", TypographyTokens.BodyMedium),
				TypographySample("BodySmall", TypographyTokens.BodySmall),
				TypographySample("LabelLarge", TypographyTokens.LabelLarge),
				TypographySample("LabelMedium", TypographyTokens.LabelMedium),
				TypographySample("LabelSmall", TypographyTokens.LabelSmall)
			);

		View TypographySample(string name, Token<FontSpec> token) =>
			VStack(4,
				Text(name)
					.Typography(TypographyTokens.LabelSmall)
					.Color(ColorTokens.OnSurfaceVariant),
				Text("The quick brown fox")
					.Typography(token)
					.Color(ColorTokens.OnSurface)
			);

		// --- 4. Button Styles ---

		View BuildButtonStylesSection() =>
			BuildSection("Button Styles", "Filled, Outlined, Text, and Elevated button variants.",
				HStack(8,
					Button("Filled", () => SetState(s => s.StyleTapCount++))
						.ButtonStyle(ButtonStyles.Filled),
					Button("Outlined", () => SetState(s => s.StyleTapCount++))
						.ButtonStyle(ButtonStyles.Outlined)
				),
				HStack(8,
					Button("Text", () => SetState(s => s.StyleTapCount++))
						.ButtonStyle(ButtonStyles.Text),
					Button("Elevated", () => SetState(s => s.StyleTapCount++))
						.ButtonStyle(ButtonStyles.Elevated)
				),
				Button("Disabled", () => { })
					.ButtonStyle(ButtonStyles.Filled)
					.IsEnabled(false),
				Text($"Taps: {State.StyleTapCount}")
					.Typography(TypographyTokens.LabelLarge)
					.Color(ColorTokens.OnSurface)
			);

		// --- 5. ViewModifier Demo ---

		View BuildViewModifierSection()
		{
			var highlight = new HighlightModifier();
			var composed = Card.Then(highlight);

			return BuildSection("ViewModifier Composition", "Compose modifiers with .Then() for reusable styling.",
				Border(
					Text("Card modifier only")
						.Typography(TypographyTokens.BodyMedium)
						.Color(ColorTokens.OnSurface)
				).Modifier(Card),
				Border(
					Text("Card + Highlight composed")
						.Typography(TypographyTokens.BodyMedium)
						.Color(ColorTokens.OnSurface)
				).Modifier(composed),
				// Scoped token override demo
				Border(
					Text("Scoped Override: Primary → Red")
						.Typography(TypographyTokens.BodyMedium)
						.Color(ColorTokens.OnPrimary)
				)
				.Background(ColorTokens.Primary)
				.CornerRadius(12)
				.Padding(new Thickness(16, 12))
				.OverrideToken(ColorTokens.Primary, Colors.Red)
			);
		}

		// --- 6. Control State Demo ---

		View BuildControlStateSection() =>
			BuildSection("Control State", "Toggle the button's enabled/disabled state.",
				HStack(12,
					Toggle(State.IsActionEnabled)
						.OnColor(ColorTokens.Primary.Resolve(ThemeManager.Current()))
						.OnToggled(isOn => SetState(s => s.IsActionEnabled = isOn)),
					Text(State.IsActionEnabled ? "Enabled" : "Disabled")
						.Typography(TypographyTokens.LabelLarge)
						.Color(ColorTokens.OnSurface)
				),
				Button("Stateful Button", () => SetState(s => s.StyleTapCount++))
					.ButtonStyle(ButtonStyles.Filled)
					.IsEnabled(State.IsActionEnabled),
				Text($"Taps: {State.StyleTapCount}")
					.Typography(TypographyTokens.BodySmall)
					.Color(ColorTokens.OnSurfaceVariant)
			);
	}

	class HighlightModifier : ViewModifier
	{
		public override View Apply(View view)
		{
			view
				.Background(new SolidPaint(ColorTokens.PrimaryContainer.Resolve(ThemeManager.Current())))
				.ClipShape(new RoundedRectangle(12))
				.Padding(new Thickness(16, 12));
			return view;
		}
	}
}
