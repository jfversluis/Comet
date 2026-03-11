using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class DemoItem
	{
		public string Icon { get; set; } = "";
		public string Title { get; set; } = "";
		public string Description { get; set; } = "";
		public Color AccentColor { get; set; } = Colors.Purple;
	}

	public class CollectionViewPageState
	{
		public string SelectedItem { get; set; } = "Nothing selected";
	}

	public class CollectionViewPage : Component<CollectionViewPageState>
	{
		static readonly IReadOnlyList<DemoItem> DemoItems = new List<DemoItem>
		{
			new() { Icon = "🎨", Title = "Styling & Theming", Description = "Customize colors, typography, and visual appearance", AccentColor = Color.FromArgb("#6750A4") },
			new() { Icon = "📱", Title = "Responsive Layout", Description = "Adaptive UI that works across all screen sizes", AccentColor = Color.FromArgb("#0061A4") },
			new() { Icon = "🔔", Title = "Notifications", Description = "Push alerts, badges, and in-app messaging", AccentColor = Color.FromArgb("#D93025") },
			new() { Icon = "🌐", Title = "Networking", Description = "HTTP requests, REST APIs, and data sync", AccentColor = Color.FromArgb("#188038") },
			new() { Icon = "🔒", Title = "Authentication", Description = "Login flows, OAuth 2.0, and secure storage", AccentColor = Color.FromArgb("#E37400") },
			new() { Icon = "📊", Title = "Data Visualization", Description = "Charts, graphs, and interactive dashboards", AccentColor = Color.FromArgb("#7627BB") },
			new() { Icon = "🎬", Title = "Animations", Description = "Smooth transitions and visual feedback", AccentColor = Color.FromArgb("#C61851") },
			new() { Icon = "🗺", Title = "Maps & Location", Description = "GPS, geocoding, and map-based experiences", AccentColor = Color.FromArgb("#006A6A") }
		};

		public override View Render()
		{
			var itemViews = new List<View>();

			foreach (var item in DemoItems)
			{
				var itemView = BuildCollectionViewItem(item);
				itemViews.Add(itemView);
			}

			return GalleryPageHelpers.Scaffold("Collection View",
				GalleryPageHelpers.Section("List with Custom Items", "Each row has a colored left accent bar (4px), icon, title (15px bold), and description (12px gray).",
					ScrollView(
						VStack(spacing: 0, itemViews.ToArray())
					)
					.Frame(height: 400),
					GalleryPageHelpers.BodyText($"Selected: {State.SelectedItem}")
				),
				GalleryPageHelpers.Section("Collection View Features", "CollectionView replaces the deprecated ListView in .NET MAUI 10.",
					GalleryPageHelpers.BodyText("✓ Vertical and horizontal scrolling"),
					GalleryPageHelpers.BodyText("✓ Single and multiple selection modes"),
					GalleryPageHelpers.BodyText("✓ Grouping and headers"),
					GalleryPageHelpers.BodyText("✓ Pull-to-refresh support"),
					GalleryPageHelpers.BodyText("✓ Incremental data loading"),
					GalleryPageHelpers.BodyText("✓ Custom item templates")
				)
			);
		}

		View BuildCollectionViewItem(DemoItem item)
		{
			return Border(
				HStack(spacing: 0,
					// Left accent bar (4px colored border)
					new Spacer()
						.Background(item.AccentColor)
						.Frame(width: 4),
					// Content area
					HStack(12,
						Text(item.Icon)
							.FontSize(24),
						VStack(4,
							Text(item.Title)
								.FontSize(15)
								.FontWeight(FontWeight.Bold)
								.Color(ColorTokens.OnSurface),
							Text(item.Description)
								.FontSize(12)
								.Color(Colors.Gray)
								.LineBreakMode(LineBreakMode.WordWrap)
						)
					)
					.Padding(new Thickness(12, 12))
				)
			)
			.Background(ColorTokens.Surface)
			.CornerRadius(0)
			.StrokeColor(new Color(128, 128, 128, 0.3f))
			.StrokeThickness(0.5f)
			.OnTap(_ => SetState(s => s.SelectedItem = item.Title));
		}
	}
}
