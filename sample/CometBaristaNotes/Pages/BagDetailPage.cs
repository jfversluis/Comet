using Comet;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;

namespace CometBaristaNotes.Pages;

public class BagDetailPage : Comet.View
{
	readonly int _bagId;

	[State] readonly State<string> _beanName = new("");
	[State] readonly State<int> _beanId = new(0);
	[State] readonly State<string> _roastDate = new("");
	[State] readonly State<string> _notes = new("");
	[State] readonly State<bool> _isComplete = new(false);
	[State] readonly State<int> _shotCount = new(0);
	[State] readonly State<bool> _isLoaded = new(false);
	[State] readonly State<string> _error = new("");
	[State] readonly State<RatingAggregate> _rating = new(new());

	public BagDetailPage(int bagId = 0) { _bagId = bagId; }

	void LoadBag()
	{
		if (_bagId <= 0) { _isLoaded.Value = true; return; }

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		var bag = store.GetBag(_bagId);
		if (bag == null) { _error.Value = "Bag not found"; _isLoaded.Value = true; return; }

		_beanName.Value = bag.BeanName ?? "";
		_beanId.Value = bag.BeanId;
		_roastDate.Value = bag.RoastDate.ToString("yyyy-MM-dd");
		_notes.Value = bag.Notes ?? "";
		_isComplete.Value = bag.IsComplete;
		_shotCount.Value = bag.ShotCount;
		_rating.Value = store.GetBagRating(_bagId);

		_isLoaded.Value = true;
	}

	void Save()
	{
		_error.Value = "";
		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		if (_bagId > 0)
		{
			store.UpdateBag(new Bag
			{
				Id = _bagId,
				BeanId = _beanId.Value,
				RoastDate = DateTime.TryParse(_roastDate.Value, out var d) ? d : DateTime.Now,
				Notes = string.IsNullOrWhiteSpace(_notes.Value) ? null : _notes.Value,
				IsComplete = _isComplete.Value,
				IsActive = true
			});
		}
		Navigation?.Pop();
	}

	async void DeleteBag()
	{
		var page = CometBaristaNotes.Services.PageHelper.GetCurrentPage();
		if (page == null) return;

		var message = _shotCount.Value > 0
			? $"This bag has {_shotCount.Value} shot(s) logged. Deleting it will hide it from all lists. Continue?"
			: "Are you sure you want to delete this bag?";

		var confirmed = await page.DisplayAlertAsync("Delete Bag", message, "Delete", "Cancel");
		if (!confirmed) return;

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		store.ArchiveBag(_bagId);
		Navigation?.Pop();
	}

	void ReactivateBag()
	{
		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		store.ReactivateBag(_bagId);
		_isComplete.Value = false;
	}

	[Body]
	Comet.View body()
	{
		if (!_isLoaded.Value)
			LoadBag();

		if (_bagId <= 0)
		{
			return new VStack {
				new Text("Bag not found")
					.FontFamily(Theme.FontRegular)
					.Color(Theme.TextSecondary),
			}
			.Padding(new Thickness(Theme.SpacingM))
			.Background(Theme.Background);
		}

		var items = new List<Comet.View>();

		items.Add(FormHelpers.MakeSectionHeader("BAG DETAILS"));
		items.Add(FormHelpers.MakeReadOnlyField("Bean", _beanName.Value));
		items.Add(FormHelpers.MakeReadOnlyField("Roast Date", _roastDate.Value));
		items.Add(FormHelpers.MakeFormEntryWithLimit("Notes", _notes.Value, "Bag notes", 500, v => _notes.Value = v));

		// Shot count card
		items.Add(FormHelpers.MakeCard(
			new VStack(spacing: 2) {
				new Text("Shots Logged")
					.FontFamily(Theme.FontRegular)
					.FontSize(14)
					.Color(Theme.TextSecondary),
				new Text($"{_shotCount.Value}")
					.FontFamily(Theme.FontSemibold)
					.FontSize(24)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),
			}
		));

		// Status toggle card
		items.Add(FormHelpers.MakeToggleRow(
			_isComplete.Value ? "Status: Complete" : "Status: Active",
			_isComplete.Value,
			v => _isComplete.Value = v
		));

		if (!string.IsNullOrEmpty(_error.Value))
			items.Add(new Text(_error.Value).Color(Theme.Error).FontSize(14));

		items.Add(FormHelpers.MakePrimaryButton("Save Changes", Save));

		if (_isComplete.Value)
			items.Add(FormHelpers.MakeSecondaryButton("Reactivate Bag", ReactivateBag));

		items.Add(FormHelpers.MakeDangerButton("Delete Bag", DeleteBag));

		items.Add(FormHelpers.MakeSectionHeader("RATINGS"));
		items.Add(RatingDisplayFactory.Create(_rating.Value));

		var stack = new VStack(spacing: Theme.SpacingS);
		foreach (var item in items) stack.Add(item);

		return new ScrollView {
			stack.Padding(new Thickness(Theme.SpacingM))
		}
		.Background(Theme.Background);
	}
}
