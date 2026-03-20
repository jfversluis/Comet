using Comet;
using Microsoft.Maui.Hosting;

namespace CometSurfingApp
{
	public class MyApp : CometApp
	{
		public MyApp()
		{
			Body = CreateRootView;
		}

		public static View CreateRootView() => new Pages.MainPage();

		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder.UseCometApp<MyApp>();

			builder.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

			return builder.Build();
		}
	}
}
