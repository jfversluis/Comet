using Comet;
using CometBaristaNotes.Pages;
using CometBaristaNotes.Components;
using Microsoft.Maui.Graphics;

namespace CometBaristaNotes;

public class BaristaApp : CometApp
{
	[Body]
	Comet.View body() =>
		new TabView
		{
			MakeTab(new ControlValidationPage(), "Validate", "checkmark.circle.fill"),
			MakeTab(new ShotLoggingPage(), "New Shot", "cup.and.saucer.fill"),
			MakeTab(new ActivityFeedPage(), "Activity", "chart.line.uptrend.xyaxis"),
			MakeTab(new SettingsPage(), "Settings", "gearshape.fill"),
		};

	static NavigationView MakeTab(Comet.View page, string title, string sfSymbol)
	{
		var nav = new NavigationView { page.Title(title) };
		nav.SetEnvironment("NavigationBackgroundColor", (Binding<Color>)Theme.Primary);
		nav.SetEnvironment("NavigationTextColor", (Binding<Color>)Theme.Surface);
		nav.TabText(title);
		nav.TabIcon(sfSymbol);
		return nav;
	}
}
