using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ListViewPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("ListView",
			GalleryPageHelpers.Section("ListView",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
