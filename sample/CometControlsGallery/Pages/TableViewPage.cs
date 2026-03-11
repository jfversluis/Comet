using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class TableViewPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("TableView",
			GalleryPageHelpers.Section("TableView",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
