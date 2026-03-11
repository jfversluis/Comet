using System;
using System.Collections.Generic;
using System.Linq;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Primitives;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	record SimpleItem(string Name, string Description, Color AccentColor);

	public class CollectionViewPageState
	{
		public string SelectedItem { get; set; } = "Tap an item";
	}

	public class CollectionViewPage : Component<CollectionViewPageState>
	{
		static readonly Color[] AccentColors =
		{
			Colors.CornflowerBlue, Colors.Coral, Colors.MediumSeaGreen, Colors.MediumOrchid,
			Colors.SandyBrown, Colors.Teal, Colors.IndianRed, Colors.DodgerBlue,
			Colors.SlateBlue, Colors.OliveDrab, Colors.Crimson, Colors.DarkCyan,
		};

		static List<SimpleItem> GenerateItems(int count) =>
			Enumerable.Range(1, count)
				.Select(i => new SimpleItem(
					$"Item {i}",
					$"Description for item {i}",
					AccentColors[(i - 1) % AccentColors.Length]))
				.ToList();

		static readonly List<SimpleItem> VerticalItems = GenerateItems(30);
		static readonly List<SimpleItem> HorizontalItems = GenerateItems(20);
		static readonly List<SimpleItem> GridItems = GenerateItems(24);

		public override View Render()
		{
			return GalleryPageHelpers.Scaffold("CollectionView",
				// Vertical List
				GalleryPageHelpers.Section("Vertical List",
					Text(State.SelectedItem)
						.FontSize(12)
						.Color(Colors.Gray),
					new CollectionView<SimpleItem>(() => VerticalItems)
					{
						ViewFor = item =>
							Border(
								HStack(spacing: 0,
									new Spacer()
										.Background(item.AccentColor)
										.Frame(width: 4),
									VStack(2,
										Text(item.Name)
											.FontSize(15)
											.FontWeight(FontWeight.Bold),
										Text(item.Description)
											.FontSize(12)
											.Color(Colors.Gray)
									)
									.Padding(new Thickness(12, 8))
								)
							)
							.StrokeColor(Colors.Gray.WithAlpha(0.3f))
							.StrokeThickness(1)
							.CornerRadius(8),
						ItemsLayout = ItemsLayout.Vertical(spacing: 8),
						SelectionMode = SelectionMode.Single,
					}
					.OnSelected(item =>
						SetState(s => s.SelectedItem = $"Selected: {item.Name}"))
					.Frame(height: 350)
				),

				// Horizontal List
				GalleryPageHelpers.Section("Horizontal List",
					Text("Scroll horizontally to see more items")
						.FontSize(12)
						.Color(Colors.Gray),
					new CollectionView<SimpleItem>(() => HorizontalItems)
					{
						ViewFor = item =>
							VStack(6,
								new ShapeView(new Circle())
									.Frame(width: 60, height: 60)
									.Background(new SolidPaint(item.AccentColor)),
								Text(item.Name)
									.FontSize(13)
									.FontWeight(FontWeight.Bold)
									.HorizontalTextAlignment(TextAlignment.Center)
							)
							.Frame(width: 100)
							.Padding(new Thickness(8)),
						ItemsLayout = ItemsLayout.Horizontal(spacing: 8),
					}
					.Frame(height: 120)
				),

				// 3-Column Vertical Grid
				GalleryPageHelpers.Section("3-Column Vertical Grid",
					new CollectionView<SimpleItem>(() => GridItems)
					{
						ViewFor = item =>
							Border(
								VStack(6,
									new Spacer()
										.Background(item.AccentColor)
										.Frame(height: 50),
									Text(item.Name)
										.FontSize(12)
										.FontWeight(FontWeight.Bold)
										.HorizontalTextAlignment(TextAlignment.Center)
								)
								.Padding(new Thickness(8))
							)
							.StrokeColor(Colors.Gray.WithAlpha(0.3f))
							.StrokeThickness(1)
							.CornerRadius(8),
						ItemsLayout = GridItemsLayout.Vertical(span: 3, spacing: 8),
					}
					.Frame(height: 400)
				),

				// Grouped CollectionView (using VStack sections since Comet CollectionView grouping is manual)
				GalleryPageHelpers.Section("Grouped CollectionView",
					BuildGroupedSection("Mammals", new[] {
						("Dog", "Loyal companion"),
						("Cat", "Independent feline"),
						("Horse", "Majestic equine"),
						("Dolphin", "Intelligent marine mammal"),
						("Elephant", "Gentle giant"),
					}),
					BuildGroupedSection("Birds", new[] {
						("Eagle", "Bird of prey"),
						("Parrot", "Colorful talker"),
						("Penguin", "Flightless swimmer"),
						("Owl", "Nocturnal hunter"),
					}),
					BuildGroupedSection("Reptiles", new[] {
						("Turtle", "Slow and steady"),
						("Gecko", "Wall climber"),
						("Iguana", "Tropical lizard"),
					}),
					BuildGroupedSection("Fish", new[] {
						("Clownfish", "Reef dweller"),
						("Salmon", "Upstream swimmer"),
						("Shark", "Ocean predator"),
						("Swordfish", "Fast swimmer"),
						("Pufferfish", "Inflatable defense"),
					})
				)
			);
		}

		View BuildGroupedSection(string groupName, (string Name, string Detail)[] items)
		{
			var views = new List<View>
			{
				Text(groupName)
					.FontSize(16)
					.FontWeight(FontWeight.Bold)
					.Color(Colors.CornflowerBlue)
					.Padding(new Thickness(0, 12, 0, 4))
			};

			foreach (var item in items)
			{
				views.Add(
					HStack(12,
						VStack(2,
							Text(item.Name)
								.FontSize(14)
								.FontWeight(FontWeight.Bold),
							Text(item.Detail)
								.FontSize(12)
								.Color(Colors.Gray)
						)
					)
					.Padding(new Thickness(16, 6, 0, 6))
				);
			}

			views.Add(
				new Spacer()
					.Background(Colors.Grey)
					.Frame(height: 1)
					.Opacity(0.3f)
			);

			return VStack((float?)0, views.ToArray());
		}
	}
}
