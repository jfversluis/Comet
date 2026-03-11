using System;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ControlsPage : View
	{
		readonly State<int> clickCount = 0;
		readonly State<string> entryText = "";
		readonly State<double> sliderValue = 50;
		readonly State<bool> toggleValue = false;
		readonly State<bool> checkValue = false;
		readonly State<double> stepperValue = 0;
		readonly State<int> radioIndex = 0;

		[Body]
		View body() =>
			GalleryPageHelpers.Scaffold("Controls",
				GalleryPageHelpers.Section("Button & ProgressBar",
					Button("Click me!", () => clickCount.Value++),
					Button("Gradient Button", () => clickCount.Value++)
						.Color(Colors.White)
						.Background(Colors.Purple),
					Text($"Clicks: {clickCount.Value}")
						.FontSize(14),
					Text("Progress (click 20x to fill):")
						.FontSize(12)
						.Color(Colors.Grey),
					ProgressBar(() => Math.Min(1.0, clickCount.Value / 20.0))
				),
				GalleryPageHelpers.Section("Button with Image",
					Button("Image Left (default)", () => { }),
					Button("Image Right", () => { }),
					Button("Image Top", () => { }),
					Button("Image Bottom", () => { })
				),
				GalleryPageHelpers.Section("ImageButton",
					new ImageButton(
						() => (IImageSource)new FontImageSource(null, "\U0001F514", 24, Colors.CornflowerBlue),
						() => { })
						.Frame(width: 44, height: 44)
				),
				GalleryPageHelpers.Section("Entry",
					TextField(() => entryText.Value, () => "Type here...")
						.OnTextChanged(value => entryText.Value = value ?? ""),
					Text($"Echo: {entryText.Value}")
						.FontSize(14)
						.Color(Colors.Grey)
				),
				GalleryPageHelpers.Section("Editor",
					TextEditor(() => "")
						.Placeholder("Multi-line text editor...")
						.Frame(height: 80)
				),
				GalleryPageHelpers.Section("Slider",
					Slider(() => sliderValue.Value, () => 0.0, () => 100.0)
						.OnValueChanged(value => sliderValue.Value = value),
					Text($"Slider: {sliderValue.Value:F0}")
						.FontSize(14)
				),
				GalleryPageHelpers.Section("Switch",
					HStack(12,
						Toggle(() => toggleValue.Value)
							.OnToggled(value => toggleValue.Value = value),
						Text(toggleValue.Value ? "On" : "Off")
							.FontSize(14)
					)
				),
				GalleryPageHelpers.Section("CheckBox",
					HStack(12,
						CheckBox(() => checkValue.Value),
						Text(checkValue.Value ? "Checked \u2713" : "Unchecked")
							.FontSize(14)
					)
				),
				GalleryPageHelpers.Section("Stepper (increment by 5)",
					Stepper(() => stepperValue.Value, () => 0.0, () => 50.0, () => 5.0),
					Text($"Stepper: {stepperValue.Value:F0}")
						.FontSize(14)
				),
				GalleryPageHelpers.Section("RadioButton",
					Text("Option A")
						.FontSize(14)
						.FontWeight(radioIndex.Value == 0 ? FontWeight.Bold : FontWeight.Regular)
						.Color(radioIndex.Value == 0 ? Colors.DodgerBlue : Colors.Black)
						.OnTap(_ => radioIndex.Value = 0),
					Text("Option B")
						.FontSize(14)
						.FontWeight(radioIndex.Value == 1 ? FontWeight.Bold : FontWeight.Regular)
						.Color(radioIndex.Value == 1 ? Colors.DodgerBlue : Colors.Black)
						.OnTap(_ => radioIndex.Value = 1),
					Text("Option C")
						.FontSize(14)
						.FontWeight(radioIndex.Value == 2 ? FontWeight.Bold : FontWeight.Regular)
						.Color(radioIndex.Value == 2 ? Colors.DodgerBlue : Colors.Black)
						.OnTap(_ => radioIndex.Value = 2),
					Text($"Selected: Option {(char)('A' + radioIndex.Value)}")
						.FontSize(14)
						.Color(Colors.DodgerBlue)
				)
			);
	}
}
