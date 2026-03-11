using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class GraphicsPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Graphics",
			GalleryPageHelpers.Section("Graphics",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
