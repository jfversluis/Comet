using Android.App;
using Android.Runtime;

namespace CometStressTest
{
	[Application]
	public class MainApplication : MauiApplication
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership)
			: base(handle, ownership)
		{
		}

		protected override MauiApp CreateMauiApp() => StressTestShell.CreateMauiApp();
	}
}
