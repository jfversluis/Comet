using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class FoodItem
	{
		public string Emoji { get; set; } = "";
		public string Name { get; set; } = "";
		public string Category { get; set; } = "";
	}

	public class DetailItem
	{
		public string Title { get; set; } = "";
		public string Detail { get; set; } = "";
	}

	public class ListViewPageState
	{
		public string SelectedFood { get; set; } = "Nothing selected";
		public string SelectedDetail { get; set; } = "Nothing selected";
	}

	public class ListViewPage : Component<ListViewPageState>
	{
		static readonly IReadOnlyList<FoodItem> FoodItems = new List<FoodItem>
		{
			new() { Emoji = "🍜", Name = "Ramen", Category = "Comfort Food" },
			new() { Emoji = "🥗", Name = "Garden Bowl", Category = "Fresh Picks" },
			new() { Emoji = "🍰", Name = "Cheesecake", Category = "Dessert" },
			new() { Emoji = "☕️", Name = "Flat White", Category = "Coffee" }
		};

		static readonly IReadOnlyList<DetailItem> DetailItems = new List<DetailItem>
		{
			new() { Title = "Account", Detail = "Manage your profile" },
			new() { Title = "Notifications", Detail = "Review push and email settings" },
			new() { Title = "Appearance", Detail = "Pick a theme mode" },
			new() { Title = "Support", Detail = "Get help and documentation" }
		};

		public override View Render()
		{
			var foodListView = new ListView<FoodItem>(() => FoodItems)
			{
				ViewFor = item => HStack(12,
					Text(item.Emoji).FontSize(20),
					VStack(4,
						Text(item.Name).FontSize(14).FontWeight(FontWeight.Bold),
						Text(item.Category).FontSize(11).Color(Colors.Gray)
					)
				)
				.Padding(new Thickness(12, 8))
			};

			foodListView.ItemSelected = selection =>
				SetState(s => s.SelectedFood = ((FoodItem)selection.item).Name);

			var detailListView = new ListView<DetailItem>(() => DetailItems)
			{
				ViewFor = item => VStack(2,
					Text(item.Title)
						.FontWeight(FontWeight.Bold)
						.Color(ColorTokens.OnSurface),
					Text(item.Detail)
						.FontSize(12)
						.Color(ColorTokens.OnSurfaceVariant)
				)
				.Padding(new Thickness(12, 10))
			};

			detailListView.ItemSelected = selection =>
				SetState(s => s.SelectedDetail = ((DetailItem)selection.item).Title);

			return GalleryPageHelpers.Scaffold("Lists",
				GalleryPageHelpers.Section("ViewCell-style ListView", "Emoji, title, and category rows rendered with a custom item template.",
					foodListView.Frame(height: 220),
					GalleryPageHelpers.BodyText($"Selected food: {State.SelectedFood}")
				),
				GalleryPageHelpers.Section("TextCell-style ListView", "Title/detail rows that feel like settings or inbox lists.",
					detailListView.Frame(height: 220),
					GalleryPageHelpers.BodyText($"Selected row: {State.SelectedDetail}")
				),
				GalleryPageHelpers.Section("Drill Down", "Open the TableView gallery from the Lists tab.",
					GalleryPageHelpers.NavButton("TableView →", () => Comet.NavigationView.Navigate(this, new TableViewPage()))
				)
			);
		}
	}
}
