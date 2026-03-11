using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class FormattedTextPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("Formatted Text",
			GalleryPageHelpers.Section("Formatted Text",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
