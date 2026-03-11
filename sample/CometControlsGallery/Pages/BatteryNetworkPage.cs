using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class BatteryNetworkPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Battery & Network",
			GalleryPageHelpers.Section("Battery & Network",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
