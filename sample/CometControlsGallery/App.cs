using Comet;
using Comet.Styles;
using CometControlsGallery.Pages;
using Microsoft.Maui.Hosting;
using static Comet.CometControls;

namespace CometControlsGallery
{
	public class App : CometApp
	{
		public App()
		{
			Body = CreateRootView;
		}

		public static View CreateRootView() =>
			TabView(
				("Controls", new ControlsPage()),
				("Layouts", new LayoutsPage()),
				("Lists", new ListsPage()),
				("Theme", new ThemePage())
			);

		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();

#if DEBUG
			builder.UseCometSampleDebugHost(CreateRootView);
#else
			builder.UseCometApp<App>();
#endif

			builder.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

			Theme.Current = Defaults.Light;

#if DEBUG
			builder.EnableSampleRuntimeDebugging();
#endif

			return builder.Build();
		}
	}
}
