using System;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class FontsPage : View
	{
		[Body]
		View body()
		{
			return ScrollView(
				VStack(15,
					// System & Embedded Fonts
					Text("System Font (Default)").FontSize(16),
					Text("OpenSans Regular (Embedded Font via alias)")
						.FontFamily("OpenSansRegular").FontSize(16),
					Text("OpenSans Regular (Embedded Font via filename)")
						.FontFamily("OpenSans-Regular").FontSize(16),
					Text("System Font Bold")
						.FontSize(16).FontWeight(FontWeight.Bold),
					Text("System Font Italic")
						.FontSize(16).FontSlant(FontSlant.Italic),

					// Various Sizes with OpenSans
					Text("OpenSans Size 10")
						.FontFamily("OpenSansRegular").FontSize(10),
					Text("OpenSans Size 18")
						.FontFamily("OpenSansRegular").FontSize(18),
					Text("OpenSans Size 24")
						.FontFamily("OpenSansRegular").FontSize(24),

					// macOS System Fonts
					Text("Menlo (macOS system font)")
						.FontFamily("Menlo").FontSize(14),
					Text("Georgia (macOS system font)")
						.FontFamily("Georgia").FontSize(14),

					// Controls with Embedded Fonts
					Button("Button with OpenSans", () => { })
						.FontFamily("OpenSansRegular").FontSize(14),
					TextField(() => "", () => "Entry with OpenSans")
						.FontFamily("OpenSansRegular").FontSize(14),

					// FontImageSource section
					Text("FontImageSource (Font Icons)")
						.FontSize(18)
						.FontWeight(FontWeight.Bold)
						.Color(Colors.CornflowerBlue)
						.Margin(new Thickness(0, 16, 0, 0)),

					Text("Unicode glyphs (system font):").FontSize(13),
					HStack(12,
						FontIcon("\u2605", Colors.Gold),
						FontIcon("\u2665", Colors.Red),
						FontIcon("\u26A1", Colors.Orange),
						FontIcon("\u2713", Colors.Green),
						FontIcon("\u2699", Colors.Grey),
						FontIcon("\u2708", Colors.DodgerBlue),
						FontIcon("\u2318", Colors.Purple),
						FontIcon("\u267B", Colors.Teal)
					),

					Text("Buttons with font icons:")
						.FontSize(13)
						.Margin(new Thickness(0, 8, 0, 0)),
					HStack(8,
						Button("\u2B07 Download", () => { }),
						Button("\u2605 Favorite", () => { }),
						Button("\u2699 Settings", () => { })
					),

					Text("Various sizes:")
						.FontSize(13)
						.Margin(new Thickness(0, 8, 0, 0)),
					HStack(16,
						FontIcon("\u2605", Colors.Gold, 16),
						FontIcon("\u2605", Colors.Gold, 24),
						FontIcon("\u2605", Colors.Gold, 32),
						FontIcon("\u2605", Colors.Gold, 48),
						FontIcon("\u2605", Colors.Gold, 64)
					)
				).Padding(new Thickness(20))
			).Title("Fonts");
		}

		static View FontIcon(string glyph, Color color, double size = 32)
		{
			return Image(() => new FontImageSource(null, glyph, size, color))
				.Frame(width: (float)size, height: (float)size);
		}
	}
}
