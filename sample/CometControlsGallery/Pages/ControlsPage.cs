using System;
using Comet;
using Comet.Graphics;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ControlsPageState
	{
		public int ClickCount { get; set; }
		public string EntryText { get; set; } = "";
		public string EditorText { get; set; } = "Comet makes it easy to build controls with MVU patterns.";
		public double SliderValue { get; set; } = 35;
		public bool ToggleValue { get; set; } = true;
		public int RadioIndex { get; set; } = 1;
	}

	public class ControlsPage : Component<ControlsPageState>
	{
		readonly Reactive<bool> isChecked = true;
		readonly Reactive<double> stepperValue = 2;

		public override View Render() =>
			GalleryPageHelpers.Scaffold("Controls",
				GalleryPageHelpers.Section("Buttons", "Interactive buttons, icon treatments, and progress feedback.",
					Button("Tap me", () => SetState(s => s.ClickCount++))
						.Color(Colors.White)
						.Background(Color.FromArgb("#6750A4"))
						.ClipShape(new RoundedRectangle(20))
						.Padding(new Thickness(24, 12)),
					ProgressBar(() => Math.Min(1, State.ClickCount / 10.0)),
					GalleryPageHelpers.Caption($"Clicks: {State.ClickCount} / 10"),
					Button("Gradient Button", () => SetState(s => s.ClickCount += 2))
						.Color(Colors.White)
						.Background(Color.FromArgb("#6750A4"))
						.CornerRadius(20),
					BuildIconButton("★", "Button with Image", () => SetState(s => s.ClickCount++)),
					Border(
						new ImageButton(() => "dotnet_bot.svg", () => SetState(s => s.ClickCount++))
							.Frame(width: 48, height: 48)
					)
					.Background(ColorTokens.SecondaryContainer.Resolve(ThemeManager.Current()))
					.CornerRadius(16)
					.Padding(new Thickness(8))
				),
				GalleryPageHelpers.Section("Text Input", "Single-line and multi-line input with live value feedback.",
					TextField(() => State.EntryText, () => "Type something")
						.OnTextChanged(value => SetState(s => s.EntryText = value ?? "")),
					GalleryPageHelpers.BodyText($"Echo: {State.EntryText}"),
					TextEditor(() => State.EditorText)
						.Frame(height: 120)
						.OnTextChanged(value => SetState(s => s.EditorText = value ?? ""))
				),
				GalleryPageHelpers.Section("Values", "Continuous and discrete inputs with bound labels.",
					Slider(() => State.SliderValue, () => 0.0, () => 100.0)
						.OnValueChanged(value => SetState(s => s.SliderValue = value)),
					GalleryPageHelpers.BodyText($"Slider value: {State.SliderValue:F0}"),
					HStack(12,
						Toggle(() => State.ToggleValue)
							.OnToggled(value => SetState(s => s.ToggleValue = value)),
						GalleryPageHelpers.BodyText(State.ToggleValue ? "Switch is On" : "Switch is Off")
					),
					HStack(12,
						CheckBox(() => isChecked.Value),
						GalleryPageHelpers.BodyText(isChecked.Value ? "Checked" : "Unchecked")
					),
					HStack(12,
						Stepper(() => stepperValue.Value, () => 0.0, () => 10.0, () => 1.0),
						GalleryPageHelpers.BodyText($"Stepper: {stepperValue.Value:F0}")
					)
				),
				GalleryPageHelpers.Section("Radio Buttons", "Single-choice selection with summary feedback.",
					// TODO: RadioButton measurement causes Size.Zero → infinite re-render.
					// Fix PresentedContent/CrossPlatformMeasure to return proper size.
					GalleryPageHelpers.BodyText("RadioButton demo temporarily disabled — measurement fix needed."),
					GalleryPageHelpers.BodyText($"Selected option: {GetRadioLabel(State.RadioIndex)}")
				),
				GalleryPageHelpers.Section("Drill Down", "Open additional control demos inside the Controls navigation stack.",
					GalleryPageHelpers.NavButton("Pickers →", () => Comet.NavigationView.Navigate(this, new PickersPage())),
					// RadioButton page disabled — measurement fix needed
					GalleryPageHelpers.NavButton("Alerts →", () => Comet.NavigationView.Navigate(this, new AlertsPage())),
					GalleryPageHelpers.NavButton("Navigation →", () => Comet.NavigationView.Navigate(this, new NavigationDemoPage()))
				)
			);

		View BuildIconButton(string glyph, string label, Action action) =>
			Border(
				HStack(10,
					Image(() => (IImageSource)new FontImageSource("OpenSansSemibold", glyph, 18, Colors.White))
						.Frame(width: 20, height: 20),
					Text(label)
						.Color(Colors.White)
						.FontWeight(FontWeight.Bold)
				)
			)
			.Background(ColorTokens.Primary.Resolve(ThemeManager.Current()))
			.CornerRadius(20)
			.Padding(new Thickness(16, 12))
			.OnTap(_ => action());

		View BuildRadioGroup()
		{
			var group = new RadioGroup(Orientation.Vertical);
			group.Add(new RadioButton(() => "Option One", () => State.RadioIndex == 0, () => SetState(s => s.RadioIndex = 0)));
			group.Add(new RadioButton(() => "Option Two", () => State.RadioIndex == 1, () => SetState(s => s.RadioIndex = 1)));
			group.Add(new RadioButton(() => "Option Three", () => State.RadioIndex == 2, () => SetState(s => s.RadioIndex = 2)));
			return group;
		}

		static string GetRadioLabel(int index) => index switch
		{
			0 => "Option One",
			1 => "Option Two",
			_ => "Option Three"
		};
	}
}
