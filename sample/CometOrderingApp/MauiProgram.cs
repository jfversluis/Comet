namespace CometOrderingApp;

public class OrderingApp : CometApp
{
	public OrderingApp()
	{
		Body = () => new Pages.MainPage();
	}

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseCometApp<OrderingApp>();

		builder.ConfigureFonts(fonts =>
		{
			fonts.AddFont("Mulish-Regular.ttf", "MulishRegular");
			fonts.AddFont("Mulish-SemiBold.ttf", "MulishSemiBold");
			fonts.AddFont("Mulish-Bold.ttf", "MulishBold");
		});

		return builder.Build();
	}
}
