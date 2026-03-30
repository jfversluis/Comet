using Android.App;
using Android.Runtime;

namespace CometOrderingApp
{
	[Application]
	public class MainApplication : MauiApplication
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership)
			: base(handle, ownership)
		{
		}

		protected override MauiApp CreateMauiApp() => OrderingApp.CreateMauiApp();
	}
}
