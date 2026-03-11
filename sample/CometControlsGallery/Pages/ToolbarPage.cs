using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ToolbarPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Toolbar",
			GalleryPageHelpers.Section("Toolbar",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
