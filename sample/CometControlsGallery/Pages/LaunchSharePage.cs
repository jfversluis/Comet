using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class LaunchSharePage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Launch & Share",
			GalleryPageHelpers.Section("Launch & Share",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
