using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class MultiWindowPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Multi-Window",
			GalleryPageHelpers.Section("Multi-Window",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
