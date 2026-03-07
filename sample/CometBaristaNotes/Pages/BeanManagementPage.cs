using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;

namespace CometBaristaNotes.Pages;

public class BeanManagementPage : Comet.View
{
	[State] readonly State<List<Bean>> _beans = new(new());
	[State] readonly State<bool> _isLoaded = new(false);

	void LoadBeans()
	{
		var store = InMemoryDataStore.Instance;
		if (store == null) return;
		_beans.Value = store.GetAllBeans();
		_isLoaded.Value = true;
	}

	[Body]
	Comet.View body()
	{
		if (!_isLoaded.Value)
			LoadBeans();

		var beans = _beans.Value;

		if (beans.Count == 0)
		{
			return new VStack(spacing: Theme.SpacingM) {
				FormHelpers.MakeEmptyState(Icons.Coffee, "No Beans Yet", "Add your favorite coffee beans to track freshness and tasting notes"),
				FormHelpers.MakePrimaryButton("+ Add Bean", () => Navigation?.Navigate(new BeanDetailPage(0))),
			}
			.Padding(new Thickness(Theme.SpacingL))
			.Background(Theme.Background);
		}

		var stack = new VStack(spacing: Theme.SpacingS) {
			FormHelpers.MakePrimaryButton("+ Add Bean", () => Navigation?.Navigate(new BeanDetailPage(0))),
		};

		foreach (var bean in beans)
		{
			stack.Add(FormHelpers.MakeListCard(
				bean.Name,
				bean.Roaster,
				bean.Origin,
				() => Navigation?.Navigate(new BeanDetailPage(bean.Id))
			));
		}

		return new ScrollView { stack.Padding(new Thickness(Theme.SpacingM)) }
			.Background(Theme.Background);
	}
}
