using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class FlyoutPageDemoPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("FlyoutPage",
			GalleryPageHelpers.Section("FlyoutPage",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
