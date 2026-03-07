using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;

namespace CometBaristaNotes.Pages;

public class UserProfileManagementPage : Comet.View
{
	[State] readonly State<List<UserProfile>> _profiles = new(new());
	[State] readonly State<bool> _isLoaded = new(false);

	void LoadProfiles()
	{
		var store = InMemoryDataStore.Instance;
		if (store == null) return;
		_profiles.Value = store.GetAllProfiles();
		_isLoaded.Value = true;
	}

	[Body]
	Comet.View body()
	{
		if (!_isLoaded.Value)
			LoadProfiles();

		var profiles = _profiles.Value;

		if (profiles.Count == 0)
		{
			return new VStack(spacing: Theme.SpacingM) {
				FormHelpers.MakeEmptyState(Icons.Person, "No Profiles Yet", "Create profiles for different users or coffee preferences"),
				FormHelpers.MakePrimaryButton("+ Add Profile", () => Navigation?.Navigate(new ProfileFormPage(0))),
			}
			.Padding(new Thickness(Theme.SpacingL))
			.Background(Theme.Background);
		}

		var stack = new VStack(spacing: Theme.SpacingS) {
			FormHelpers.MakePrimaryButton("+ Add Profile", () => Navigation?.Navigate(new ProfileFormPage(0))),
		};

		foreach (var profile in profiles)
		{
			stack.Add(FormHelpers.MakeListCard(
				profile.Name,
				$"Member since {profile.CreatedAt:MMM yyyy}",
				null,
				() => Navigation?.Navigate(new ProfileFormPage(profile.Id))
			));
		}

		return new ScrollView { stack.Padding(new Thickness(Theme.SpacingM)) }
			.Background(Theme.Background);
	}
}
