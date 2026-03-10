using System;
using Comet;
using Microsoft.Maui;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class RadioButtonPageState
	{
		public int SizeIndex { get; set; } = 1;
		public int ColorIndex { get; set; }
		public int PlanIndex { get; set; } = 2;
	}

	public class RadioButtonPage : Component<RadioButtonPageState>
	{
		static readonly string[] Sizes = { "Small", "Medium", "Large" };
		static readonly string[] Colors = { "Purple", "Mint", "Coral" };
		static readonly string[] Plans = { "Starter", "Team", "Enterprise" };

		public override View Render() =>
			GalleryPageHelpers.Scaffold("Radio Buttons",
				GalleryPageHelpers.Section("Size", "Choose a single size option from the vertical group.",
					BuildGroup(Sizes, State.SizeIndex, index => SetState(s => s.SizeIndex = index))
				),
				GalleryPageHelpers.Section("Color", "Choose the accent color that best fits the preview.",
					BuildGroup(Colors, State.ColorIndex, index => SetState(s => s.ColorIndex = index))
				),
				GalleryPageHelpers.Section("Plan", "Choose the plan level for this demo experience.",
					BuildGroup(Plans, State.PlanIndex, index => SetState(s => s.PlanIndex = index))
				),
				GalleryPageHelpers.Section("Selection Feedback", "Summarize all three radio groups in a single label.",
					GalleryPageHelpers.BodyText($"Size: {Sizes[State.SizeIndex]}"),
					GalleryPageHelpers.BodyText($"Color: {Colors[State.ColorIndex]}"),
					GalleryPageHelpers.BodyText($"Plan: {Plans[State.PlanIndex]}")
				)
			);

		static View BuildGroup(string[] options, int selectedIndex, Action<int> onSelected)
		{
			var group = new RadioGroup(Orientation.Vertical);

			for (var index = 0; index < options.Length; index++)
			{
				var capturedIndex = index;
				group.Add(new RadioButton(
					() => options[capturedIndex],
					() => selectedIndex == capturedIndex,
					() => onSelected(capturedIndex)));
			}

			return group;
		}
	}
}
