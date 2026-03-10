using System;
using System.Collections.Generic;
using System.Linq;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ListItem
	{
		public string Title { get; set; } = "";
		public string Subtitle { get; set; } = "";
		public string Category { get; set; } = "";
	}

	public class ListsState
	{
		public int SelectedIndex { get; set; } = -1;
		public string SelectedItem { get; set; } = "None";
	}

	public class ListsPage : Component<ListsState>
	{
		static readonly SectionCard Card = new();

		static readonly IReadOnlyList<ListItem> SampleItems = Enumerable.Range(1, 20)
			.Select(i => new ListItem
			{
				Title = $"Item {i}",
				Subtitle = $"Description for item {i}",
				Category = i % 3 == 0 ? "Important" : i % 2 == 0 ? "Normal" : "Low"
			})
			.ToList();

		public override View Render()
		{
			var listView = new ListView<ListItem>(() => SampleItems)
			{
				ViewFor = item => HStack(12,
					Border(Text(""))
						.Background(GetCategoryColor(item))
						.Frame(width: 4, height: 40)
						.CornerRadius(2),
					VStack(4,
						Text(item.Title)
							.Typography(TypographyTokens.TitleMedium)
							.Color(ColorTokens.OnSurface),
						Text(item.Subtitle)
							.Typography(TypographyTokens.BodySmall)
							.Color(ColorTokens.OnSurfaceVariant)
					)
				)
				.Padding(new Thickness(12, 8))
			};

			listView.ItemSelected = selection =>
				SetState(s =>
				{
					s.SelectedItem = ((ListItem)selection.item).Title;
					s.SelectedIndex = selection.row;
				});

			return VStack(16,
				Border(
					VStack(8,
						Text("Selection")
							.Typography(TypographyTokens.TitleLarge)
							.Color(ColorTokens.OnSurface),
						Text($"Selected: {State.SelectedItem}")
							.Typography(TypographyTokens.BodyMedium)
							.Color(ColorTokens.Primary)
					)
				)
				.Modifier(Card),
				listView
			)
			.Background(ColorTokens.Background);
		}

		static Token<Color> GetCategoryColor(ListItem item) => item.Category switch
		{
			"Important" => ColorTokens.Error,
			"Normal" => ColorTokens.Primary,
			_ => ColorTokens.Tertiary
		};
	}
}
