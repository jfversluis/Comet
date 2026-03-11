using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class TabbedPageDemoPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("TabbedPage",
			GalleryPageHelpers.Section("TabbedPage",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
