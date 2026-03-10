using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class LayoutsPageState
	{
	}

	public class LayoutsPage : Component<LayoutsPageState>
	{
		public override View Render() =>
			GalleryPageHelpers.Scaffold("Layouts",
				GalleryPageHelpers.Section("Vertical Stack", "Three vertically stacked blocks with even spacing.",
					VStack(10,
						GalleryPageHelpers.ColorBlock("Top", Color.FromArgb("#6750A4"), Colors.White),
						GalleryPageHelpers.ColorBlock("Middle", Color.FromArgb("#625B71"), Colors.White),
						GalleryPageHelpers.ColorBlock("Bottom", Color.FromArgb("#7D5260"), Colors.White)
					)
				),
				GalleryPageHelpers.Section("Horizontal Stack", "Four blocks arranged in a horizontal row.",
					HStack(10,
						Block("1", Color.FromArgb("#D0BCFF"), Color.FromArgb("#381E72")).Frame(height: 72),
						Block("2", Color.FromArgb("#CCC2DC"), Color.FromArgb("#332D41")).Frame(height: 72),
						Block("3", Color.FromArgb("#EFB8C8"), Color.FromArgb("#492532")).Frame(height: 72),
						Block("4", Color.FromArgb("#F9DEDC"), Color.FromArgb("#410E0B")).Frame(height: 72)
					)
				),
				GalleryPageHelpers.Section("Nested Grid", "A 2×2 grid using explicit row and column placement.",
					Grid(new object[] { "*", "*" }, new object[] { "*", "*" },
						Block("A1", Color.FromArgb("#EADDFF"), Color.FromArgb("#21005D")).Cell(0, 0),
						Block("A2", Color.FromArgb("#E8DEF8"), Color.FromArgb("#1D192B")).Cell(0, 1),
						Block("B1", Color.FromArgb("#FFD8E4"), Color.FromArgb("#31111D")).Cell(1, 0),
						Block("B2", Color.FromArgb("#F3EDF7"), Color.FromArgb("#1C1B1F")).Cell(1, 1)
					)
					.ColumnSpacing(10)
					.RowSpacing(10)
				),
				GalleryPageHelpers.Section("Bordered Container", "A card-style container with stroke, radius, and padding.",
					Border(
						VStack(8,
							GalleryPageHelpers.SectionHeader("Bordered Content"),
							GalleryPageHelpers.BodyText("Use Border for rounded containers instead of Frame in MAUI 10.")
						)
					)
					.Background(ColorTokens.Surface.Resolve(ThemeManager.Current()))
					.CornerRadius(20)
					.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
					.StrokeThickness(1.5)
					.Padding(new Thickness(16))
				),
				GalleryPageHelpers.Section("Styled Border", "Icon and content arranged inside a highlighted border.",
					Border(
						HStack(12,
							Image(() => (IImageSource)new FontImageSource("OpenSansSemibold", "✦", 18, Color.FromArgb("#21005D")))
								.Frame(width: 24, height: 24),
							VStack(4,
								GalleryPageHelpers.BodyText("Styled Border"),
								GalleryPageHelpers.Caption("Accent icon, content, and stroke work together in a single card.")
							)
						)
					)
					.Background(Color.FromArgb("#EADDFF"))
					.CornerRadius(18)
					.StrokeColor(Color.FromArgb("#6750A4"))
					.StrokeThickness(2)
					.Padding(new Thickness(16))
				),
				GalleryPageHelpers.Section("Rounded Borders", "Uniform, soft, and pill-style rounded border treatments.",
					HStack(10,
						RoundedSample("Uniform", 12),
						RoundedSample("Soft", 24),
						RoundedSample("Pill", 999)
					)
				),
				GalleryPageHelpers.Section("Deeply Nested Borders", "Four border layers showing progressively tighter padding and color.",
					Border(
						Border(
							Border(
								Border(
									GalleryPageHelpers.BodyText("Level 4")
								)
								.Background(Color.FromArgb("#FFD8E4"))
								.CornerRadius(14)
								.Padding(new Thickness(12))
							)
							.Background(Color.FromArgb("#E8DEF8"))
							.CornerRadius(18)
							.Padding(new Thickness(12))
						)
						.Background(Color.FromArgb("#EADDFF"))
						.CornerRadius(22)
						.Padding(new Thickness(12))
					)
					.Background(Color.FromArgb("#F3EDF7"))
					.CornerRadius(26)
					.Padding(new Thickness(12))
				),
				GalleryPageHelpers.Section("Drill Down", "Open the transforms gallery for translation, scale, and rotation demos.",
					GalleryPageHelpers.NavButton("Transforms →", () => Comet.NavigationView.Navigate(this, new TransformsPage()))
				)
			);

		static View Block(string label, Color background, Color foreground) =>
			Border(
				Text(label)
					.FontWeight(FontWeight.Bold)
					.Color(foreground)
					.HorizontalTextAlignment(TextAlignment.Center)
			)
			.Background(background)
			.CornerRadius(16)
			.Padding(new Thickness(12))
			.Frame(height: 84);

		static View RoundedSample(string label, float radius) =>
			Border(
				Text(label)
					.Color(ColorTokens.OnSurface)
					.HorizontalTextAlignment(TextAlignment.Center)
			)
			.Background(ColorTokens.SurfaceContainer.Resolve(ThemeManager.Current()))
			.CornerRadius(radius)
			.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
			.StrokeThickness(1)
			.Padding(new Thickness(16, 10))
			.Frame(height: 64);
	}
}
