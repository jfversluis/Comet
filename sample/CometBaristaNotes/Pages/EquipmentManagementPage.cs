using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;

namespace CometBaristaNotes.Pages;

public class EquipmentManagementPage : Comet.View
{
	[State] readonly State<List<Equipment>> _equipment = new(new());
	[State] readonly State<bool> _isLoaded = new(false);

	void LoadEquipment()
	{
		var store = InMemoryDataStore.Instance;
		if (store == null) return;
		_equipment.Value = store.GetAllEquipment();
		_isLoaded.Value = true;
	}

	[Body]
	Comet.View body()
	{
		if (!_isLoaded.Value)
			LoadEquipment();

		var items = _equipment.Value;

		if (items.Count == 0)
		{
			return new VStack(spacing: Theme.SpacingM) {
				FormHelpers.MakeEmptyState(Icons.Build, "No Equipment Yet", "Add your coffee machines, grinders, and accessories"),
				FormHelpers.MakePrimaryButton("+ Add Equipment", () => Navigation?.Navigate(new EquipmentDetailPage(0))),
			}
			.Padding(new Thickness(Theme.SpacingL))
			.Background(Theme.Background);
		}

		var stack = new VStack(spacing: Theme.SpacingS) {
			FormHelpers.MakePrimaryButton("+ Add Equipment", () => Navigation?.Navigate(new EquipmentDetailPage(0))),
		};

		foreach (var eq in items)
		{
			stack.Add(FormHelpers.MakeListCard(
				eq.Name,
				eq.Type.ToString(),
				eq.Notes,
				() => Navigation?.Navigate(new EquipmentDetailPage(eq.Id))
			));
		}

		return new ScrollView { stack.Padding(new Thickness(Theme.SpacingM)) }
			.Background(Theme.Background);
	}
}
