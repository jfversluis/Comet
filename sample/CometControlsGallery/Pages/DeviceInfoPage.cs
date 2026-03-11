using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class DeviceInfoPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Device & App Info",
			GalleryPageHelpers.Section("Device & App Info",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
