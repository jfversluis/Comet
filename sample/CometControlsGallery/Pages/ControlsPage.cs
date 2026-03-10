using System;
using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ControlsState
	{
		public int TapCount { get; set; }
		public string TextValue { get; set; } = "";
		public string SecureText { get; set; } = "";
		public string EditorText { get; set; } = "";
		public string SearchText { get; set; } = "";
		public bool ToggleValue { get; set; }
		public bool CheckValue { get; set; }
		public int RadioIndex { get; set; }
		public int PickerIndex { get; set; }
		public double SliderValue { get; set; } = 50;
		public double StepperValue { get; set; } = 5;
		public DateTime? DateValue { get; set; } = DateTime.Today;
		public TimeSpan? TimeValue { get; set; } = new TimeSpan(14, 30, 0);
		public double ProgressValue { get; set; } = 0.6;
	}

	public class ControlsPage : Component<ControlsState>
	{
		static readonly SectionCard Card = new();

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

		View BuildRadioGroup()
		{
			var group = new RadioGroup(Orientation.Vertical);
			group.Add(new RadioButton(() => "Small", () => State.RadioIndex == 0, () => SetState(s => s.RadioIndex = 0)));
			group.Add(new RadioButton(() => "Medium", () => State.RadioIndex == 1, () => SetState(s => s.RadioIndex = 1)));
			group.Add(new RadioButton(() => "Large", () => State.RadioIndex == 2, () => SetState(s => s.RadioIndex = 2)));
			return group;
		}

		public override View Render() => ScrollView(
			VStack(24,
				// Buttons
				BuildSection("Buttons", "Tap actions with Material 3 button styles.",
					HStack(12,
						Button("Filled", () => SetState(s => s.TapCount++))
							.ButtonStyle(ButtonStyles.Filled),
						Button("Outlined", () => SetState(s => s.TapCount++))
							.ButtonStyle(ButtonStyles.Outlined)
					),
					HStack(12,
						Button("Text", () => SetState(s => s.TapCount++))
							.ButtonStyle(ButtonStyles.Text),
						Button("Elevated", () => SetState(s => s.TapCount++))
							.ButtonStyle(ButtonStyles.Elevated)
					),
					Text(() => $"Taps: {State.TapCount}")
						.Typography(TypographyTokens.LabelLarge)
						.Color(ColorTokens.Primary)
				),

				// Text Input
				BuildSection("Text Input", "Fields for text entry, passwords, and search.",
					TextField(() => State.TextValue, () => "Enter text\u2026")
						.OnTextChanged(v => SetState(s => s.TextValue = v ?? "")),
					SecureField(() => State.SecureText, () => "Password")
						.OnTextChanged(v => SetState(s => s.SecureText = v ?? "")),
					TextEditor(() => State.EditorText)
						.Frame(height: 80)
						.OnTextChanged(v => SetState(s => s.EditorText = v ?? "")),
					SearchBar(() => State.SearchText, () => { })
						.OnTextChanged(v => SetState(s => s.SearchText = v ?? "")),
					Text(() => string.IsNullOrEmpty(State.TextValue) ? "" : $"Entered: {State.TextValue}")
						.Typography(TypographyTokens.BodySmall)
						.Color(ColorTokens.OnSurfaceVariant)
				),

				// Selection
				BuildSection("Selection", "Toggles, checkboxes, and pickers.",
					HStack(12,
						Toggle(() => State.ToggleValue)
							.OnToggled(isOn => SetState(s => s.ToggleValue = isOn)),
						Text(() => State.ToggleValue ? "On" : "Off")
							.Typography(TypographyTokens.BodyLarge)
							.Color(ColorTokens.OnSurface)
					),
					HStack(12,
						CheckBox(() => State.CheckValue),
						Text("Accept terms")
							.Typography(TypographyTokens.BodyLarge)
							.Color(ColorTokens.OnSurface)
					),
					Picker((Binding<int>)(() => State.PickerIndex), "Red", "Green", "Blue")
						.OnSelectedIndexChanged(i => SetState(s => s.PickerIndex = i))
				),

				// Numeric
				BuildSection("Numeric", "Sliders and steppers for numeric input.",
					Text(() => $"Slider: {(int)State.SliderValue}")
						.Typography(TypographyTokens.BodyLarge)
						.Color(ColorTokens.OnSurface),
					Slider(() => State.SliderValue, () => 0.0, () => 100.0)
						.OnValueChanged(v => SetState(s => s.SliderValue = v)),
					Text(() => $"Stepper: {State.StepperValue}")
						.Typography(TypographyTokens.BodyLarge)
						.Color(ColorTokens.OnSurface),
					Stepper(() => State.StepperValue, () => 0.0, () => 10.0, () => 1.0)
				),

				// Date & Time
				BuildSection("Date & Time", "Date and time selection controls.",
					HStack(12,
						DatePicker(() => State.DateValue),
						TimePicker(() => State.TimeValue)
					)
				),

				// Display
				BuildSection("Display", "Read-only display controls and indicators.",
					Image("https://aka.ms/dotnet-bot-image")
						.Frame(width: 120, height: 120),
					ActivityIndicator(() => true),
					ProgressBar(() => State.ProgressValue),
					Text("Loading\u2026")
						.Typography(TypographyTokens.LabelLarge)
						.Color(ColorTokens.Primary)
				)
			).Padding(new Thickness(20))
		).Background(ColorTokens.Background);
	}
}
