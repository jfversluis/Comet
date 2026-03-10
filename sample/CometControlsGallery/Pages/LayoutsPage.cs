using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class LayoutsPageState { }

	public class LayoutsPage : Component<LayoutsPageState>
	{
		static readonly SectionCard Card = new();

		public override View Render()
		{
			return ScrollView(
				VStack(24,
					Text("Layouts & Spacing")
						.Typography(TypographyTokens.HeadlineMedium)
						.Color(ColorTokens.OnBackground),

					BuildSpacingSection(),
					BuildHStackSection(),
					BuildGridSection(),
					BuildShapeTokensSection(),
					BuildShadowsSection(),
					BuildHorizontalScrollSection()
				)
				.Padding(new Thickness(20))
			)
			.Background(ColorTokens.Background);
		}

		// --- Helpers ---

		View BuildSection(string title, string description, params View[] content)
		{
			var inner = new View[content.Length + 2];
			inner[0] = Text(title)
				.Typography(TypographyTokens.TitleLarge)
				.Color(ColorTokens.OnSurface);
			inner[1] = Text(description)
				.Typography(TypographyTokens.BodyMedium)
				.Color(ColorTokens.OnSurfaceVariant)
				.LineBreakMode(LineBreakMode.WordWrap);
			for (int i = 0; i < content.Length; i++)
				inner[i + 2] = content[i];

			return Border(VStack(12, inner)).Modifier(Card);
		}

		View Box(Token<Color> color, float width, float height) =>
			Border(Text(""))
				.Background(color)
				.Frame(width: width, height: height)
				.CornerRadius(4);

		// --- 1. VStack Spacing Demo ---

		View BuildSpacingSection()
		{
			return BuildSection(
				"VStack Spacing",
				"Each column uses a different SpacingToken value.",
				HStack(16,
					SpacingDemo("None", 0f),
					SpacingDemo("XS", 4f),
					SpacingDemo("Small", 8f)
				),
				HStack(16,
					SpacingDemo("Medium", 16f),
					SpacingDemo("Large", 24f),
					SpacingDemo("XL", 32f)
				)
			);
		}

		View SpacingDemo(string name, float spacing)
		{
			return VStack(4,
				Text($"{name} ({spacing}px)")
					.Typography(TypographyTokens.LabelMedium)
					.Color(ColorTokens.OnSurface),
				VStack(spacing,
					Box(ColorTokens.Primary, 60, 24),
					Box(ColorTokens.Secondary, 60, 24),
					Box(ColorTokens.Tertiary, 60, 24)
				)
			);
		}

		// --- 2. HStack Demo ---

		View BuildHStackSection()
		{
			return BuildSection(
				"HStack",
				"Horizontal row of colored chips with 12px spacing.",
				HStack(12,
					ColorChip("Primary", ColorTokens.Primary, ColorTokens.OnPrimary),
					ColorChip("Secondary", ColorTokens.Secondary, ColorTokens.OnSecondary),
					ColorChip("Tertiary", ColorTokens.Tertiary, ColorTokens.OnTertiary),
					ColorChip("Error", ColorTokens.Error, ColorTokens.OnError),
					ColorChip("Surface", ColorTokens.SurfaceVariant, ColorTokens.OnSurfaceVariant)
				)
			);
		}

		View ColorChip(string label, Token<Color> bg, Token<Color> fg) =>
			Border(
				Text(label)
					.Typography(TypographyTokens.LabelSmall)
					.Color(fg)
					.HorizontalTextAlignment(TextAlignment.Center)
			)
			.Background(bg)
			.CornerRadius(8)
			.Padding(new Thickness(10, 6));

		// --- 3. Grid-style Demo (simulated with nested stacks) ---

		View BuildGridSection()
		{
			return BuildSection(
				"Grid Layout",
				"A 2×3 grid simulated with nested HStack/VStack.",
				VStack(4,
					HStack(4,
						GridCell("1,1", ColorTokens.PrimaryContainer, ColorTokens.OnPrimaryContainer),
						GridCell("1,2", ColorTokens.SecondaryContainer, ColorTokens.OnSecondaryContainer)
					),
					HStack(4,
						GridCell("2,1", ColorTokens.TertiaryContainer, ColorTokens.OnTertiaryContainer),
						GridCell("2,2", ColorTokens.ErrorContainer, ColorTokens.OnErrorContainer)
					),
					HStack(4,
						GridCell("3,1", ColorTokens.SurfaceVariant, ColorTokens.OnSurfaceVariant),
						GridCell("3,2", ColorTokens.SurfaceContainerHigh, ColorTokens.OnSurface)
					)
				)
			);
		}

		View GridCell(string label, Token<Color> bg, Token<Color> fg) =>
			Border(
				Text(label)
					.Typography(TypographyTokens.LabelLarge)
					.Color(fg)
					.HorizontalTextAlignment(TextAlignment.Center)
			)
			.Background(bg)
			.Frame(height: 60)
			.CornerRadius(8)
			.Padding(new Thickness(16, 8));

		// --- 4. Border & Shape Tokens ---

		View BuildShapeTokensSection()
		{
			return BuildSection(
				"Shape Tokens",
				"Border corner radius using ShapeToken values.",
				HStack(8,
					ShapeDemo("None", 0),
					ShapeDemo("XS", 4),
					ShapeDemo("Small", 8),
					ShapeDemo("Med", 12)
				),
				HStack(8,
					ShapeDemo("Large", 16),
					ShapeDemo("XL", 28),
					ShapeDemo("Full", 9999)
				)
			);
		}

		View ShapeDemo(string name, int radius) =>
			Border(
				Text(name)
					.Typography(TypographyTokens.LabelSmall)
					.Color(ColorTokens.OnSurfaceVariant)
					.HorizontalTextAlignment(TextAlignment.Center)
			)
			.Background(ColorTokens.SurfaceVariant)
			.CornerRadius(radius)
			.Frame(width: 80, height: 50);

		// --- 5. Shadows Demo ---

		View BuildShadowsSection()
		{
			return BuildSection(
				"Shadows",
				"Shadow effects on text, buttons, and cards.",
				Text("Shadow on Text")
					.Typography(TypographyTokens.TitleMedium)
					.Color(ColorTokens.OnSurface)
					.Shadow(Colors.Black.WithAlpha(0.3f), radius: 4, x: 2, y: 2),
				Button("Elevated Button", () => { })
					.ButtonStyle(ButtonStyles.Elevated),
				Border(
					Text("Shadow Card")
						.Typography(TypographyTokens.BodyMedium)
						.Color(ColorTokens.OnSurface)
						.HorizontalTextAlignment(TextAlignment.Center)
				)
				.Background(ColorTokens.Surface)
				.CornerRadius(16)
				.Padding(new Thickness(24, 16))
				.Shadow(Colors.Black.WithAlpha(0.15f), radius: 8, x: 0, y: 4)
			);
		}

		// --- 6. Horizontal ScrollView ---

		View BuildHorizontalScrollSection()
		{
			return BuildSection(
				"Horizontal Scroll",
				"Swipe to scroll through cards horizontally.",
				ScrollView(Orientation.Horizontal,
					HStack(12,
						HorizontalCard("Card 1", ColorTokens.PrimaryContainer, ColorTokens.OnPrimaryContainer),
						HorizontalCard("Card 2", ColorTokens.SecondaryContainer, ColorTokens.OnSecondaryContainer),
						HorizontalCard("Card 3", ColorTokens.TertiaryContainer, ColorTokens.OnTertiaryContainer),
						HorizontalCard("Card 4", ColorTokens.ErrorContainer, ColorTokens.OnErrorContainer),
						HorizontalCard("Card 5", ColorTokens.SurfaceContainerHigh, ColorTokens.OnSurface),
						HorizontalCard("Card 6", ColorTokens.PrimaryContainer, ColorTokens.OnPrimaryContainer),
						HorizontalCard("Card 7", ColorTokens.SecondaryContainer, ColorTokens.OnSecondaryContainer),
						HorizontalCard("Card 8", ColorTokens.TertiaryContainer, ColorTokens.OnTertiaryContainer)
					)
				)
			);
		}

		View HorizontalCard(string title, Token<Color> bg, Token<Color> fg) =>
			Border(
				VStack(8,
					Text(title)
						.Typography(TypographyTokens.TitleSmall)
						.Color(fg),
					Text("Sample content")
						.Typography(TypographyTokens.BodySmall)
						.Color(fg)
				)
				.Padding(new Thickness(16, 12))
			)
			.Background(bg)
			.CornerRadius(12)
			.Frame(width: 140, height: 80);
	}
}
