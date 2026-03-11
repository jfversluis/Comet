using System;
using System.Collections.Generic;
using Comet;
using Comet.Styles;
using CometControlsGallery.Pages;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using static Comet.CometControls;
#if DEBUG
using MauiDevFlow.Agent;
#endif
using MauiApplication = Microsoft.Maui.Controls.Application;
using MauiContentPage = Microsoft.Maui.Controls.ContentPage;
using MauiWindow = Microsoft.Maui.Controls.Window;

namespace CometControlsGallery
{
	public class NavItem
	{
		public string Title { get; set; } = "";
		public string Icon { get; set; } = "";
		public Func<View> CreatePage { get; set; } = () => new Text("Empty");
		public string Category { get; set; } = "";
	}

	public class App : MauiApplication
	{
		protected override MauiWindow CreateWindow(IActivationState activationState)
		{
			var page = new MauiContentPage
			{
				Padding = 0,
				Content = new CometHost(new SidebarLayout())
			};
			return new MauiWindow(page);
		}

		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();

			builder.UseMauiApp<App>();
			builder.UseCometHandlers();

#if DEBUG
			builder.AddMauiDevFlowAgent();
#endif

#if MACCATALYST
			builder.ConfigureLifecycleEvents(events =>
			{
				events.AddiOS(ios =>
				{
					ios.SceneWillConnect((scene, session, options) =>
					{
						if (scene is UIKit.UIWindowScene windowScene)
						{
							windowScene.SizeRestrictions.MinimumSize = new CoreGraphics.CGSize(900, 600);
							windowScene.SizeRestrictions.MaximumSize = new CoreGraphics.CGSize(2000, 1400);
						}
					});
				});
			});
#endif

			builder.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

			Theme.Current = Defaults.Light;

			return builder.Build();
		}
	}

	public class SidebarLayout : View
	{
		readonly State<int> selectedIndex = 0;
		readonly State<string> selectedTitle = "Controls";

		static readonly List<NavItem> navItems = new()
		{
			new NavItem { Title = "Controls", Icon = "🎛", Category = "General", CreatePage = () => new ControlsPage() },
			// RadioButton disabled — CrossPlatformMeasure returns Size.Zero, causing infinite re-render
			// new NavItem { Title = "RadioButton", Icon = "🔘", Category = "General", CreatePage = () => new RadioButtonPage() },
			new NavItem { Title = "Pickers", Icon = "📅", Category = "General", CreatePage = () => new PickersPage() },
			new NavItem { Title = "Fonts", Icon = "🔤", Category = "General", CreatePage = () => new FontsPage() },
			new NavItem { Title = "Layouts", Icon = "📐", Category = "General", CreatePage = () => new LayoutsPage() },
			new NavItem { Title = "Alerts & Dialogs", Icon = "💬", Category = "General", CreatePage = () => new AlertsPage() },
			new NavItem { Title = "Shapes", Icon = "🔷", Category = "Shapes & Visuals", CreatePage = () => new ShapesPage() },
			new NavItem { Title = "Transforms", Icon = "🔄", Category = "Shapes & Visuals", CreatePage = () => new TransformsPage() },
			new NavItem { Title = "Gestures", Icon = "👆", Category = "Shapes & Visuals", CreatePage = () => new GesturesPage() },
			new NavItem { Title = "Collection View", Icon = "📋", Category = "Lists & Collections", CreatePage = () => new CollectionViewPage() },
			new NavItem { Title = "Settings", Icon = "⚙️", Category = "Lists & Collections", CreatePage = () => new SettingsPage() },
			new NavItem { Title = "Theme", Icon = "🎨", Category = "Settings", CreatePage = () => new ThemePage() },
			new NavItem { Title = "Navigation Demo", Icon = "🧭", Category = "Settings", CreatePage = () => new NavigationDemoPage() },
		};

		[Body]
		View body()
		{
			var sidebar = BuildSidebar();
			var detail = navItems[selectedIndex.Value].CreatePage();

			return Grid(
				new object[] { 280, "*" },
				null,
				sidebar.Cell(row: 0, column: 0),
				NavigationView(detail)
					.Title(selectedTitle.Value)
					.Cell(row: 0, column: 1)
			);
		}

		View BuildSidebar()
		{
			var items = new List<View>();

			items.Add(
				VStack(
					Text("Comet Gallery")
						.FontSize(18)
						.FontWeight(FontWeight.Bold)
						.Color(Colors.White),
					Text("Controls Reference")
						.FontSize(11)
						.Color(new Color(255, 255, 255, 180))
				)
				.Padding(new Thickness(16, 20, 16, 12))
				.Background(new Color(88, 86, 214))
			);

			string lastCategory = null;

			for (int i = 0; i < navItems.Count; i++)
			{
				var item = navItems[i];
				var index = i;

				if (item.Category != lastCategory)
				{
					lastCategory = item.Category;
					items.Add(
						Text(item.Category.ToUpperInvariant())
							.FontSize(10)
							.FontWeight(FontWeight.Bold)
							.Color(Colors.Grey)
							.Padding(new Thickness(16, 14, 16, 4))
					);
				}

				var isSelected = selectedIndex.Value == index;

				items.Add(
					Button($"{item.Icon}  {item.Title}", () =>
					{
						selectedIndex.Value = index;
						selectedTitle.Value = navItems[index].Title;
					})
					.FontSize(14)
					.Color(isSelected ? new Color(88, 86, 214) : new Color(60, 60, 67))
					.Background(isSelected ? new Color(88, 86, 214, 25) : new Color(242, 242, 247))
					.Padding(new Thickness(16, 8))
					.Frame(height: 36)
				);
			}

			return VStack((float?)0, items.ToArray())
				.Background(new Color(242, 242, 247));
		}
	}
}
