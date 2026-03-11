using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class ClipboardStoragePage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Clipboard & Storage",
			GalleryPageHelpers.Section("Clipboard & Storage",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
