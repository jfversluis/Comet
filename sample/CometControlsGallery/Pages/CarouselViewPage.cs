using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class CarouselViewPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("CarouselView",
			GalleryPageHelpers.Section("CarouselView",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
