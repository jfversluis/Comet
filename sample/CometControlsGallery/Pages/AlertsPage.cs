using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
public class AlertsPageState
{
public string Status { get; set; } = "No alert shown yet.";
}

public class AlertsPage : Component<AlertsPageState>
{
static readonly SectionCard Card = new();

		public override Comet.View Render()
{
return NavigationView(
ScrollView(
VStack(24,
BuildSection(
"Alerts",
"Drive platform alerts through the active MAUI page.",
Button("Display Alert", ShowSimpleAlert)
.ButtonStyle(ButtonStyles.Filled),
Button("Display Confirm", ShowConfirmAlert)
.ButtonStyle(ButtonStyles.Outlined),
Button("Display Action Sheet", ShowActionSheet)
.ButtonStyle(ButtonStyles.Elevated),
Button("Display Prompt", ShowPrompt)
.ButtonStyle(ButtonStyles.Text),
Border(
Text(() => State.Status)
.Typography(TypographyTokens.BodyMedium)
.Color(ColorTokens.OnSurface)
.LineBreakMode(LineBreakMode.WordWrap)
)
.Background(ColorTokens.SurfaceContainerLow)
.CornerRadius(14)
.Padding(new Thickness(16))
)
)
.Padding(new Thickness(24))
)
.Background(ColorTokens.Background)
)
.Title("Alerts");
}

		Comet.View BuildSection(string title, string description, params Comet.View[] content)
{
			var items = new List<Comet.View>
{
Text(title)
.Typography(TypographyTokens.TitleLarge)
.Color(ColorTokens.OnSurface),
Text(description)
.Typography(TypographyTokens.BodyMedium)
.Color(ColorTokens.OnSurfaceVariant)
.LineBreakMode(LineBreakMode.WordWrap)
};
items.AddRange(content);
return Border(VStack(16, items.ToArray())).Modifier(Card);
}

async void ShowSimpleAlert()
{
var page = GetCurrentPage();
if (page == null)
{
SetState(s => s.Status = "Unable to locate the current MAUI page.");
return;
}

await page.DisplayAlertAsync("Hello", "This is a simple alert from Comet.", "OK");
SetState(s => s.Status = "Displayed a simple alert.");
}

async void ShowConfirmAlert()
{
var page = GetCurrentPage();
if (page == null)
{
SetState(s => s.Status = "Unable to locate the current MAUI page.");
return;
}

var accepted = await page.DisplayAlertAsync("Confirm", "Do you want to continue?", "Yes", "No");
SetState(s => s.Status = accepted ? "User confirmed the action." : "User canceled the action.");
}

async void ShowActionSheet()
{
var page = GetCurrentPage();
if (page == null)
{
SetState(s => s.Status = "Unable to locate the current MAUI page.");
return;
}

var result = await page.DisplayActionSheetAsync("Choose a color", "Cancel", null, "Red", "Green", "Blue");
SetState(s => s.Status = $"Action sheet result: {result}");
}

async void ShowPrompt()
{
var page = GetCurrentPage();
if (page == null)
{
SetState(s => s.Status = "Unable to locate the current MAUI page.");
return;
}

var value = await page.DisplayPromptAsync("Prompt", "Enter a short note", "Save", "Cancel", "Type here");
SetState(s => s.Status = string.IsNullOrWhiteSpace(value) ? "Prompt canceled or left empty." : $"Prompt result: {value}");
}

		static Microsoft.Maui.Controls.Page? GetCurrentPage()
		{
			if (Microsoft.Maui.Controls.Application.Current?.Windows?.Count > 0)
			{
				return Microsoft.Maui.Controls.Application.Current.Windows[0].Page;
			}

return null;
}
}
}
