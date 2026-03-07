using Comet;
using CometBaristaNotes.Pages;
using CometBaristaNotes.Components;

namespace CometBaristaNotes;

public class BaristaApp : CometApp
{
	[Body]
	Comet.View body() =>
		new TabView
		{
			new NavigationView { new ShotLoggingPage().Title("New Shot") }
				.TabText("New Shot"),
			new NavigationView { new ActivityFeedPage().Title("Activity") }
				.TabText("Activity"),
			new NavigationView { new SettingsPage().Title("Settings") }
				.TabText("Settings"),
		};
}
