using System;
using System.Linq;
using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class PickersPageState
	{
		public DateTime? SelectedDate { get; set; }
		public TimeSpan SelectedTime { get; set; } = DateTime.Now.TimeOfDay;
		public int PickerIndex { get; set; } = -1;
		public string SearchText { get; set; } = "";
	}

	public class PickersPage : Component<PickersPageState>
	{
		static readonly string[] ColorOptions = { "Red", "Green", "Blue", "Orange", "Purple" };
		static readonly string[] Fruits =
		{
			"Apple", "Banana", "Cherry", "Date", "Elderberry",
			"Fig", "Grape", "Honeydew", "Kiwi", "Lemon", "Mango"
		};

		public override View Render()
		{
			var dateText = State.SelectedDate.HasValue
				? $"Selected date: {State.SelectedDate.Value:D}"
				: "Selected date: (none)";

			var timeText = $"Selected time: {DateTime.Today.Add(State.SelectedTime):hh\\:mm tt}";

			var pickerText = State.PickerIndex >= 0 && State.PickerIndex < ColorOptions.Length
				? $"Selected: {ColorOptions[State.PickerIndex]}"
				: "Selected: (none)";

			string searchResultsText;
			if (string.IsNullOrWhiteSpace(State.SearchText))
			{
				searchResultsText = "Search results will appear here...";
			}
			else
			{
				var matches = Fruits.Where(f => f.Contains(State.SearchText, StringComparison.OrdinalIgnoreCase)).ToArray();
				searchResultsText = matches.Length > 0
					? $"Found: {string.Join(", ", matches)}"
					: "No matches found.";
			}

			return GalleryPageHelpers.Scaffold("Pickers",
				GalleryPageHelpers.Section("DatePicker",
					DatePicker(
						(Binding<DateTime?>)(() => State.SelectedDate ?? DateTime.Today)
					),
					GalleryPageHelpers.BodyText(dateText)
				),
				GalleryPageHelpers.Section("TimePicker",
					TimePicker(
						(Binding<TimeSpan?>)(() => State.SelectedTime)
					),
					GalleryPageHelpers.BodyText(timeText)
				),
				GalleryPageHelpers.Section("Picker (Dropdown)",
					new Picker((Binding<int>)(() => State.PickerIndex), ColorOptions)
					{
						Title = "Pick a color"
					}.OnSelectedIndexChanged(index => SetState(s => s.PickerIndex = index)),
					GalleryPageHelpers.BodyText(pickerText)
				),
				GalleryPageHelpers.Section("SearchBar",
					SearchBar(() => State.SearchText, () => { })
						.Placeholder("Search fruits...")
						.OnTextChanged(value => SetState(s => s.SearchText = value ?? "")),
					Text(searchResultsText)
						.FontSize(14)
						.Color(string.IsNullOrWhiteSpace(State.SearchText) ? Colors.Grey : null)
				)
			);
		}
	}
}
