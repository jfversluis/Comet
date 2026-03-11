using System;
using System.Collections.Generic;
using Comet;
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

			return builder.Build();
		}
	}

	public class SidebarLayout : View
	{
		readonly State<int> selectedIndex = 0;
		NavigationView? _mainNav;

		static readonly Color SidebarBackground = Color.FromArgb("#E8E8F0");
		static readonly Color CategoryHeaderColor = Colors.Grey;
		static readonly Color ItemTextColor = new Color(60, 60, 67);
		static readonly Color SelectedAccent = new Color(88, 86, 214);

		static readonly List<NavItem> navItems = new()
		{
			// General
			new NavItem { Title = "Home", Category = "General", CreatePage = () => new HomePage() },
			new NavItem { Title = "Controls", Category = "General", CreatePage = () => new ControlsPage() },
			new NavItem { Title = "RadioButton", Category = "General", CreatePage = () => new RadioButtonPage() },
			new NavItem { Title = "Pickers & Search", Category = "General", CreatePage = () => new PickersPage() },
			new NavItem { Title = "Fonts", Category = "General", CreatePage = () => new FontsPage() },
			new NavItem { Title = "Formatted Text", Category = "General", CreatePage = () => new FormattedTextPage() },
			new NavItem { Title = "Layouts", Category = "General", CreatePage = () => new LayoutsPage() },
			new NavItem { Title = "Alerts & Dialogs", Category = "General", CreatePage = () => new AlertsPage() },
			// Lists & Collections
			new NavItem { Title = "Collection View", Category = "Lists & Collections", CreatePage = () => new CollectionViewPage() },
			new NavItem { Title = "CarouselView", Category = "Lists & Collections", CreatePage = () => new CarouselViewPage() },
			new NavItem { Title = "ListView", Category = "Lists & Collections", CreatePage = () => new ListViewPage() },
			new NavItem { Title = "Grouped Lists", Category = "Lists & Collections", CreatePage = () => new GroupedListsPage() },
			new NavItem { Title = "TableView", Category = "Lists & Collections", CreatePage = () => new TableViewPage() },
			// Drawing & Visual
			new NavItem { Title = "Graphics", Category = "Drawing & Visual", CreatePage = () => new GraphicsPage() },
			new NavItem { Title = "Gestures", Category = "Drawing & Visual", CreatePage = () => new GesturesPage() },
			new NavItem { Title = "Shapes", Category = "Drawing & Visual", CreatePage = () => new ShapesPage() },
			new NavItem { Title = "Transforms", Category = "Drawing & Visual", CreatePage = () => new TransformsPage() },
			// Platform
			new NavItem { Title = "Menu Bar", Category = "Platform", CreatePage = () => new MenuBarPage() },
			new NavItem { Title = "Toolbar", Category = "Platform", CreatePage = () => new ToolbarPage() },
			new NavItem { Title = "Multi-Window", Category = "Platform", CreatePage = () => new MultiWindowPage() },
			new NavItem { Title = "Theme", Category = "Platform", CreatePage = () => new ThemePage() },
			new NavItem { Title = "WebView", Category = "Platform", CreatePage = () => new WebViewPage() },
			new NavItem { Title = "Device & App Info", Category = "Platform", CreatePage = () => new DeviceInfoPage() },
			new NavItem { Title = "Battery & Network", Category = "Platform", CreatePage = () => new BatteryNetworkPage() },
			new NavItem { Title = "Clipboard & Storage", Category = "Platform", CreatePage = () => new ClipboardStoragePage() },
			new NavItem { Title = "Launch & Share", Category = "Platform", CreatePage = () => new LaunchSharePage() },
			// Navigation
			new NavItem { Title = "Navigation Demo", Category = "Navigation", CreatePage = () => new NavigationDemoPage() },
			new NavItem { Title = "TabbedPage", Category = "Navigation", CreatePage = () => new TabbedPageDemoPage() },
			new NavItem { Title = "FlyoutPage", Category = "Navigation", CreatePage = () => new FlyoutPageDemoPage() },
			new NavItem { Title = "Map", Category = "Navigation", CreatePage = () => new MapPage() },
		};

		[Body]
		View body()
		{
			var sidebar = BuildSidebar();
			var idx = selectedIndex.Value;
			var detail = navItems[idx].CreatePage();

			_mainNav = NavigationView(detail)
				.Title(navItems[idx].Title);

			return Grid(
				new object[] { 280, "*" },
				null,
				sidebar.Cell(row: 0, column: 0),
				_mainNav.Cell(row: 0, column: 1)
			);
		}

		View BuildSidebar()
		{
			var items = new List<View>();
			string lastCategory = null;

			for (int i = 0; i < navItems.Count; i++)
			{
				var item = navItems[i];
				var index = i;

				if (item.Category != lastCategory)
				{
					lastCategory = item.Category;
					items.Add(
						Text(item.Category)
							.FontSize(11)
							.FontWeight(FontWeight.Bold)
							.HorizontalTextAlignment(TextAlignment.Start)
							.Color(CategoryHeaderColor)
							.Padding(new Thickness(16, 14, 16, 4))
					);
				}

				var isSelected = selectedIndex.Value == index;

				items.Add(
					Text(item.Title)
						.FontSize(13)
						.HorizontalTextAlignment(TextAlignment.Start)
						.Color(isSelected ? SelectedAccent : ItemTextColor)
						.Background(isSelected ? new Color(88, 86, 214, 25) : SidebarBackground)
						.Padding(new Thickness(16, 8))
						.Frame(height: 34)
						.OnTap((v) =>
						{
							_mainNav?.PopToRoot();
							selectedIndex.Value = index;
						})
				);
			}

			return ScrollView(
				VStack((float?)0, items.ToArray())
			)
			.Background(SidebarBackground);
		}
	}
}
