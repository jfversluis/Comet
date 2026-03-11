using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class MenuBarPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Menu Bar",
			GalleryPageHelpers.Section("Menu Bar",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
