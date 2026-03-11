using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class MapPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Map",
			GalleryPageHelpers.Section("Map",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
