using System;
using System.Linq;
using Comet;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class PickersPageState
	{
		public int PickerIndex { get; set; }
		public string SearchText { get; set; } = "";
	}

	public class PickersPage : Component<PickersPageState>
	{
		static readonly string[] PickerOptions = { "Citrus", "Berry", "Stone Fruit", "Melon" };
		static readonly string[] SearchItems =
		{
			"Apricot",
			"Blackberry",
			"Blueberry",
			"Clementine",
			"Grapefruit",
			"Strawberry",
			"Watermelon"
		};

		readonly Reactive<DateTime?> selectedDate = DateTime.Today;
		readonly Reactive<TimeSpan?> selectedTime = new(new TimeSpan(9, 30, 0));

		public override View Render()
		{
			var results = SearchItems
				.Where(item => string.IsNullOrWhiteSpace(State.SearchText) || item.Contains(State.SearchText, StringComparison.OrdinalIgnoreCase))
				.ToArray();

			return GalleryPageHelpers.Scaffold("Pickers",
				GalleryPageHelpers.Section("DatePicker", "Choose a date and show the current selection below.",
					DatePicker(selectedDate),
					GalleryPageHelpers.BodyText($"Selected date: {selectedDate.Value:dddd, MMM d, yyyy}")
				),
				GalleryPageHelpers.Section("TimePicker", "Choose a time and display it as formatted text.",
					TimePicker(selectedTime),
					GalleryPageHelpers.BodyText($"Selected time: {selectedTime.Value:hh\\:mm}")
				),
				GalleryPageHelpers.Section("Picker", "Standard dropdown selection with a value summary.",
					Picker((Binding<int>)(() => State.PickerIndex), PickerOptions)
						.OnSelectedIndexChanged(index => SetState(s => s.PickerIndex = index)),
					GalleryPageHelpers.BodyText($"Selected: {PickerOptions[Math.Clamp(State.PickerIndex, 0, PickerOptions.Length - 1)]}")
				),
				GalleryPageHelpers.Section("SearchBar", "Filter a small result set using the current search text.",
					SearchBar(() => State.SearchText, () => { })
						.OnTextChanged(value => SetState(s => s.SearchText = value ?? "")),
					VStack(6,
						results.Length == 0
							? GalleryPageHelpers.Caption("No matches found.")
							: VStack(6, results.Select(item => GalleryPageHelpers.BodyText($"• {item}")).ToArray())
					)
				)
			);
		}
	}
}
