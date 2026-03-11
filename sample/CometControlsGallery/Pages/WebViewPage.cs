using System;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class WebViewPageState
	{
		public string CurrentUrl { get; set; } = "https://dotnet.microsoft.com";
	}

	public class WebViewPage : Component<WebViewPageState>
	{
		public override View Render()
		{
			var webView = new WebView();
			webView.Source = State.CurrentUrl;
			webView.OnNavigated = url => SetState(s => s.CurrentUrl = url);

			return GalleryPageHelpers.Scaffold("WebView",
				HStack(8,
					Button("Back", () => ((IWebView)webView).GoBack()),
					Button("Forward", () => ((IWebView)webView).GoForward()),
					Button("Reload", () => ((IWebView)webView).Reload())
				),

				Grid(
					new object[] { "*", "Auto" },
					null,
					TextField(() => State.CurrentUrl, () => "Enter URL...")
						.Cell(row: 0, column: 0),
					Button("Go", () =>
					{
						var url = State.CurrentUrl?.Trim();
						if (!string.IsNullOrEmpty(url))
						{
							if (!url.StartsWith("http://") && !url.StartsWith("https://"))
								url = "https://" + url;
							SetState(s => s.CurrentUrl = url);
						}
					})
					.Background(Colors.DodgerBlue)
					.Color(Colors.White)
					.Cell(row: 0, column: 1)
				)
				.ColumnSpacing(8),

				webView.Frame(height: 500)
			);
		}
	}
}
