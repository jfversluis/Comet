using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class FontsPageState
	{
		public string EntryText { get; set; } = "Comet custom font";
	}

	public class FontsPage : Component<FontsPageState>
	{
		static readonly SectionCard Card = new();

		public override View Render()
		{
			return NavigationView(
				ScrollView(
					VStack(24,
						BuildFamiliesSection(),
						BuildSizesSection(),
						BuildGlyphSection(),
						BuildControlsSection()
					)
					.Padding(new Thickness(24))
				)
				.Background(ColorTokens.Background)
			)
			.Title("Fonts");
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
			return Border(VStack(16, items.ToArray())).Modifier(Card);
		}

		View BuildFamiliesSection() =>
			BuildSection(
				"Font Families",
				"System and bundled font samples, including OpenSans and common desktop families.",
				FontSample("System Default", Text("The quick brown fox").FontSize(20)),
				FontSample("OpenSans Regular", Text("OpenSans Regular").FontFamily("OpenSansRegular").FontSize(22)),
				FontSample("OpenSans Semibold", Text("OpenSans Semibold").FontFamily("OpenSansSemibold").FontSize(22)),
				FontSample("Italic", Text("Italic sample").FontSlant(FontSlant.Italic).FontSize(22)),
				FontSample("Menlo", Text("Menlo 0123456789").FontFamily("Menlo").FontSize(20)),
				FontSample("Georgia", Text("Georgia heading").FontFamily("Georgia").FontSize(24))
			);

		View BuildSizesSection() =>
			BuildSection(
				"Font Sizes",
				"Compare several type sizes with the same content.",
				Text("12pt sample").FontSize(12).Color(ColorTokens.OnSurface),
				Text("18pt sample").FontSize(18).Color(ColorTokens.OnSurface),
				Text("24pt sample").FontSize(24).Color(ColorTokens.OnSurface),
				Text("32pt sample").FontSize(32).Color(ColorTokens.OnSurface)
			);

		View BuildGlyphSection() =>
			BuildSection(
				"Font Icons",
				"Comet's FontImageSource can render Unicode glyphs as images.",
				HStack(16,
					BuildGlyph("\u2605", Colors.Goldenrod),
					BuildGlyph("\u2665", Colors.DeepPink),
					BuildGlyph("\u2713", Colors.ForestGreen),
					BuildGlyph("\u2601", Colors.SteelBlue)
				)
			);

		View BuildControlsSection() =>
			BuildSection(
				"Controls With Fonts",
				"Buttons and entries can share the same font customization APIs.",
				Button("Semibold Button", () => { })
					.FontFamily("OpenSansSemibold")
					.FontSize(18)
					.ButtonStyle(ButtonStyles.Filled),
				TextField(() => State.EntryText, () => "Font sample")
					.OnTextChanged(text => SetState(s => s.EntryText = text ?? ""))
					.FontFamily("Menlo")
					.FontSize(18),
				Text(() => $"Entry preview: {State.EntryText}")
					.FontFamily("OpenSansRegular")
					.FontSize(18)
					.Color(ColorTokens.OnSurfaceVariant)
			);

		View FontSample(string title, View sample)
		{
			return VStack(6,
				Text(title)
					.Typography(TypographyTokens.LabelSmall)
					.Color(ColorTokens.OnSurfaceVariant),
				sample
			);
		}

		View BuildGlyph(string glyph, Color color)
		{
			return Border(
				VStack(8,
					Image(() => new FontImageSource("OpenSansSemibold", glyph, 28, color))
						.Frame(width: 40, height: 40),
					Text(glyph)
						.Typography(TypographyTokens.LabelSmall)
						.Color(ColorTokens.OnSurfaceVariant)
						.HorizontalTextAlignment(TextAlignment.Center)
				)
			)
			.Background(ColorTokens.SurfaceContainerLow)
			.CornerRadius(14)
			.Padding(new Thickness(16));
		}
	}
}
