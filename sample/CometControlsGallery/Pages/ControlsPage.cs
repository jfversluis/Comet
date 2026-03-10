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
		public string EditorText { get; set; } = "Multi-line text editor";
		public string SearchText { get; set; } = "";
		public bool ToggleValue { get; set; }
		public bool CheckValue { get; set; }
		public int RadioIndex { get; set; }
		public int PickerIndex { get; set; }
		public double SliderValue { get; set; } = 50;
		public double StepperValue { get; set; } = 5;
		public DateTime? DateValue { get; set; } = DateTime.Today;
		public TimeSpan? TimeValue { get; set; } = new TimeSpan(14, 30, 0);
	}

	public class ControlsPage : Component<ControlsState>
	{
		static readonly SectionCard Card = new();

		View BuildSection(string title, string subtitle, params View[] content)
		{
			var items = new List<View>
			{
				Text(title)
					.Typography(TypographyTokens.TitleLarge)
					.Color(ColorTokens.OnSurface),
				Text(subtitle)
					.Typography(TypographyTokens.BodyMedium)
					.Color(ColorTokens.OnSurfaceVariant)
					.LineBreakMode(LineBreakMode.WordWrap)
			};
			items.AddRange(content);
			return Border(VStack(12, items.ToArray())).Modifier(Card);
		}

		public override View Render()
		{
			return ScrollView(
				VStack(24,
					BuildButtonsSection(),
					BuildTextInputSection(),
					BuildSelectionSection(),
					BuildNumericSection(),
					BuildDateTimeSection(),
					BuildDisplaySection()
				)
				.Padding(new Thickness(24))
			)
			.Background(ColorTokens.Background);
		}

		View BuildButtonsSection() =>
			BuildSection("Buttons", "Four built-in ButtonStyles with a shared tap counter.",
				Button("Filled", () => SetState(s => s.TapCount++))
					.ButtonStyle(ButtonStyles.Filled),
				Button("Outlined", () => SetState(s => s.TapCount++))
					.ButtonStyle(ButtonStyles.Outlined),
				Button("Text", () => SetState(s => s.TapCount++))
					.ButtonStyle(ButtonStyles.Text),
				Button("Elevated", () => SetState(s => s.TapCount++))
					.ButtonStyle(ButtonStyles.Elevated),
				Text($"Taps: {State.TapCount}")
					.Typography(TypographyTokens.LabelLarge)
					.Color(ColorTokens.Primary)
			);

		View BuildTextInputSection() =>
			BuildSection("Text Input", "TextField, SecureField, TextEditor, and SearchBar.",
				TextField(() => State.TextValue, () => "Enter text…"),
				SecureField(() => State.SecureText, () => "Password"),
				TextEditor(() => State.EditorText)
					.Frame(height: 80),
				SearchBar(() => State.SearchText, () => { }),
				Text($"Text: {State.TextValue}")
					.Typography(TypographyTokens.BodySmall)
					.Color(ColorTokens.OnSurfaceVariant)
			);

		View BuildSelectionSection() =>
			BuildSection("Selection", "Toggle, CheckBox, RadioGroup, and Picker.",
				HStack(12,
					Toggle(() => State.ToggleValue)
						.OnColor(ColorTokens.Primary.Resolve(ThemeManager.Current()))
						.OnToggled(isOn => SetState(s => s.ToggleValue = isOn)),
					Text(State.ToggleValue ? "On" : "Off")
						.Typography(TypographyTokens.LabelLarge)
						.Color(ColorTokens.OnSurface)
				),
				HStack(12,
					CheckBox(() => State.CheckValue),
					Text("Accept terms")
						.Typography(TypographyTokens.BodyMedium)
						.Color(ColorTokens.OnSurface)
				),
				Picker(State.PickerIndex, "Red", "Green", "Blue", "Yellow")
			);

		View BuildNumericSection()
		{
			return BuildSection("Numeric", "Slider (0–100) and Stepper (0–10).",
				Text($"Slider: {(int)State.SliderValue}")
					.Typography(TypographyTokens.LabelLarge)
					.Color(ColorTokens.Primary),
				Slider(() => State.SliderValue, () => 0.0, () => 100.0),
				HStack(12,
					Text($"Stepper: {State.StepperValue}")
						.Typography(TypographyTokens.LabelLarge)
						.Color(ColorTokens.Primary),
					Stepper(() => State.StepperValue, () => 0.0, () => 10.0, () => 1.0)
				)
			);
		}

		View BuildDateTimeSection() =>
			BuildSection("Date & Time", "DatePicker and TimePicker.",
				HStack(16,
					DatePicker(() => State.DateValue),
					TimePicker(() => State.TimeValue)
				)
			);

		View BuildDisplaySection() =>
			BuildSection("Display", "Image, ActivityIndicator, and ProgressBar.",
				Image("https://aka.ms/dotnet-bot-image")
					.Frame(width: 120, height: 120),
				HStack(12,
					ActivityIndicator(() => true),
					Text("Loading…")
						.Typography(TypographyTokens.BodyMedium)
						.Color(ColorTokens.OnSurfaceVariant)
				),
				ProgressBar(() => 0.6),
				ProgressBar(() => 0.3)
			);
	}
}
