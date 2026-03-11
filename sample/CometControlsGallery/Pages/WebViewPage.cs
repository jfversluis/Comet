using Comet;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class WebViewPage : View
	{
		[Body]
		View body() => GalleryPageHelpers.Scaffold("WebView",
			GalleryPageHelpers.Section("WebView",
				new Text("This page is under construction.")
					.FontSize(14)
					.Color(Colors.Grey)
			)
		);
	}
}
