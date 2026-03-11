using System;
using System.Runtime.InteropServices;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class HomePage : View
	{
		[Body]
		View body() =>
			ScrollView(
				VStack(16,
					Text("\U0001F34E .NET MAUI on macOS")
						.FontSize(32)
						.FontWeight(FontWeight.Bold)
						.HorizontalTextAlignment(TextAlignment.Center),
					Text("Rendered natively with AppKit")
						.FontSize(16)
						.HorizontalTextAlignment(TextAlignment.Center)
						.Color(Colors.Grey),
					Text("This sample app demonstrates the Microsoft.Maui.Platform.MacOS backend \u2014 " +
						"a standalone .NET MAUI backend for macOS that maps MAUI controls " +
						"to native AppKit widgets. No MAUI fork required!")
						.FontSize(14),
					Border(
						VStack(8,
							Text("Platform Details")
								.FontSize(18)
								.FontWeight(FontWeight.Bold),
							Text("\u2022 MAUI control handlers mapped to AppKit")
								.FontSize(14),
							Text("\u2022 Native NSView-based rendering")
								.FontSize(14),
							Text("\u2022 WebKit for BlazorWebView")
								.FontSize(14),
							Text("\u2022 CoreGraphics-backed ICanvas for GraphicsView")
								.FontSize(14),
							Text("\u2022 .NET 10 / MAUI 10")
								.FontSize(14),
							Text($"\u2022 Runtime: {RuntimeInformation.FrameworkDescription}")
								.FontSize(14)
								.Color(Colors.Grey),
							Text($"\u2022 OS: {RuntimeInformation.OSDescription}")
								.FontSize(14)
								.Color(Colors.Grey)
						)
						.Padding(new Thickness(16))
					)
					.RoundedBorder(radius: 8, color: Colors.DodgerBlue, strokeSize: 1),
					Text("Use the menu on the left to explore different control demos.")
						.FontSize(14)
						.Color(Colors.Grey)
						.HorizontalTextAlignment(TextAlignment.Center)
				)
				.Padding(new Thickness(24))
			)
			.Title("Home");
	}
}
