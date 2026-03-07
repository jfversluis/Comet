using Comet;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;
using Border = Comet.Border;
using Grid = Comet.Grid;

namespace CometBaristaNotes.Pages;

public class BeanDetailPage : Comet.View
{
	readonly int _beanId;
	const int ShotsPageSize = 10;

	[State] readonly State<string> _name = new("");
	[State] readonly State<string> _roaster = new("");
	[State] readonly State<string> _origin = new("");
	[State] readonly State<string> _notes = new("");
	[State] readonly State<bool> _isLoaded = new(false);
	[State] readonly State<string> _error = new("");
	[State] readonly State<List<Bag>> _bags = new(new());
	[State] readonly State<RatingAggregate> _rating = new(new());
	[State] readonly State<List<ShotRecord>> _allShots = new(new());
	[State] readonly State<int> _visibleShotCount = new(ShotsPageSize);

	public BeanDetailPage(int beanId = 0) { _beanId = beanId; }

	void LoadBean()
	{
		if (_beanId <= 0) { _isLoaded.Value = true; return; }

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		var bean = store.GetBean(_beanId);
		if (bean == null) { _error.Value = "Bean not found"; _isLoaded.Value = true; return; }

		_name.Value = bean.Name;
		_roaster.Value = bean.Roaster ?? "";
		_origin.Value = bean.Origin ?? "";
		_notes.Value = bean.Notes ?? "";
		_bags.Value = store.GetBagsForBean(_beanId);
		_rating.Value = store.GetBeanRating(_beanId);
		_allShots.Value = store.GetShotsByBean(_beanId);

		_isLoaded.Value = true;
	}

	void Save()
	{
		if (string.IsNullOrWhiteSpace(_name.Value))
		{
			_error.Value = "Bean name is required";
			return;
		}
		_error.Value = "";

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		if (_beanId > 0)
		{
			store.UpdateBean(new Bean
			{
				Id = _beanId,
				Name = _name.Value,
				Roaster = string.IsNullOrWhiteSpace(_roaster.Value) ? null : _roaster.Value,
				Origin = string.IsNullOrWhiteSpace(_origin.Value) ? null : _origin.Value,
				Notes = string.IsNullOrWhiteSpace(_notes.Value) ? null : _notes.Value,
				IsActive = true
			});
		}
		else
		{
			store.CreateBean(new Bean
			{
				Name = _name.Value,
				Roaster = string.IsNullOrWhiteSpace(_roaster.Value) ? null : _roaster.Value,
				Origin = string.IsNullOrWhiteSpace(_origin.Value) ? null : _origin.Value,
				Notes = string.IsNullOrWhiteSpace(_notes.Value) ? null : _notes.Value,
			});
		}

		Navigation?.Pop();
	}

	async void DeleteBean()
	{
		var page = Services.PageHelper.GetCurrentPage();
		if (page == null) return;

		var confirmed = await page.DisplayAlertAsync(
			"Delete Bean",
			$"Are you sure you want to delete \"{_name.Value}\"? This will also archive all associated bags.",
			"Delete", "Cancel");

		if (!confirmed) return;

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		store.ArchiveBean(_beanId);
		Navigation?.Pop();
	}

	[Body]
	Comet.View body()
	{
		if (!_isLoaded.Value)
			LoadBean();

		var isEdit = _beanId > 0;

		var items = new List<Comet.View>();

		items.Add(FormHelpers.MakeSectionHeader(isEdit ? "EDIT BEAN" : "NEW BEAN"));
		items.Add(FormHelpers.MakeFormEntry("Name *", _name.Value, "Bean name", v => _name.Value = v));
		items.Add(FormHelpers.MakeFormEntry("Roaster", _roaster.Value, "Roaster name", v => _roaster.Value = v));
		items.Add(FormHelpers.MakeFormEntry("Origin", _origin.Value, "Country or region", v => _origin.Value = v));
		items.Add(FormHelpers.MakeFormEntry("Notes", _notes.Value, "Tasting notes, processing, etc.", v => _notes.Value = v));

		if (!string.IsNullOrEmpty(_error.Value))
			items.Add(new Text(_error.Value).Color(Theme.Error).FontFamily(Theme.FontRegular).FontSize(14));

		items.Add(FormHelpers.MakePrimaryButton(isEdit ? "Save Changes" : "Create Bean", Save));

		if (isEdit)
		{
			items.Add(FormHelpers.MakeDangerButton("Delete Bean", DeleteBean));
			items.Add(FormHelpers.MakeSectionHeader("RATINGS"));
			items.Add(RatingDisplayFactory.Create(_rating.Value));
			items.Add(BuildRatingDistribution());

			items.Add(FormHelpers.MakeSectionHeader("BAGS"));
			if (_bags.Value.Count == 0)
				items.Add(new Text("No bags added yet").FontFamily(Theme.FontRegular).FontSize(14).Color(Theme.TextSecondary));

			items.Add(FormHelpers.MakeSecondaryButton("+ Add Bag", () =>
			{
				Navigation?.Navigate(new BagFormPage(_beanId));
			}));

			foreach (var bag in _bags.Value)
			{
				items.Add(BuildBagCard(bag));
			}

			// Shot History
			items.Add(FormHelpers.MakeSectionHeader("SHOT HISTORY"));
			var shots = _allShots.Value;
			if (shots.Count == 0)
			{
				items.Add(new Text("No shots recorded yet").FontFamily(Theme.FontRegular).FontSize(14).Color(Theme.TextSecondary));
			}
			else
			{
				var visible = shots.Take(_visibleShotCount.Value).ToList();
				foreach (var shot in visible)
				{
					var shotId = shot.Id;
					items.Add(ShotRecordCardFactory.Create(shot, () =>
					{
						Navigation?.Navigate(new ShotLoggingPage(shotId));
					}));
				}

				if (_visibleShotCount.Value < shots.Count)
				{
					items.Add(FormHelpers.MakeSecondaryButton(
						$"Load More ({shots.Count - _visibleShotCount.Value} remaining)",
						() => { _visibleShotCount.Value += ShotsPageSize; }));
				}
			}
		}

		var stack = new VStack(spacing: Theme.SpacingS);
		foreach (var item in items) stack.Add(item);

		return new ScrollView {
			stack.Padding(new Thickness(Theme.SpacingM))
		}
		.Background(Theme.Background);
	}

	Comet.View BuildRatingDistribution()
	{
		var shots = _allShots.Value;
		var sentiments = new[] { Icons.SentimentVeryDissatisfied, Icons.SentimentDissatisfied, Icons.SentimentNeutral, Icons.SentimentSatisfied, Icons.SentimentVerySatisfied };
		var sentimentColors = new[] { Theme.Error, Theme.Warning, Theme.TextMuted, Theme.Success, Theme.StarFilled };

		var counts = new int[5];
		foreach (var shot in shots)
		{
			if (shot.Rating.HasValue)
			{
				var idx = Math.Clamp(shot.Rating.Value - 1, 0, 4);
				counts[idx]++;
			}
		}
		var maxCount = counts.Max();

		var container = new VStack(spacing: Theme.SpacingXS);

		// Iterate from highest rating (5/VerySatisfied) down to lowest (1/VeryDissatisfied)
		for (var i = 4; i >= 0; i--)
		{
			var barFraction = maxCount > 0 ? (double)counts[i] / maxCount : 0;

			container.Add(
				new Grid(columns: new object[] { 28, "*", 30 }, rows: new object[] { "Auto" })
				{
					// Sentiment icon
					new Text(sentiments[i])
						.FontFamily(Icons.FontFamily)
						.FontSize(18)
						.Color(sentimentColors[i])
						.HorizontalTextAlignment(TextAlignment.Center)
						.VerticalTextAlignment(TextAlignment.Center)
						.Cell(row: 0, column: 0),

					// Progress bar
					BuildBarOverlay(barFraction, sentimentColors[i])
						.Cell(row: 0, column: 1),

					// Count label
					new Text(counts[i].ToString())
						.FontFamily(Theme.FontRegular)
						.FontSize(12)
						.Color(Theme.TextSecondary)
						.HorizontalTextAlignment(TextAlignment.End)
						.VerticalTextAlignment(TextAlignment.Center)
						.Cell(row: 0, column: 2),
				}
				.ColumnSpacing(Theme.SpacingS)
				.Frame(height: 24)
			);
		}

		return new Border {
			container
		}
		.CornerRadius(Theme.RadiusCard)
		.Background(Theme.CardBackground)
		.StrokeColor(Theme.CardStroke)
		.StrokeThickness(1)
		.Padding(new Thickness(Theme.SpacingM))
		.Margin(new Thickness(0, Theme.SpacingXS, 0, 0));
	}

	Comet.View BuildBarOverlay(double fraction, Color fillColor)
	{
		return new Comet.ProgressBar(fraction)
			.ProgressColor(fillColor)
			.TrackColor(Theme.SurfaceVariant)
			.Frame(height: 12);
	}

	Comet.View BuildBagCard(Bag bag)
	{
		var statsItems = new List<Comet.View>();
		statsItems.Add(new Text($"{bag.ShotCount} shots").FontFamily(Theme.FontRegular).FontSize(12).Color(Theme.TextMuted));

		if (bag.AverageRating.HasValue)
			statsItems.Add(new Text($"{Icons.SentimentVerySatisfied} {bag.AverageRating.Value:F1}").FontFamily(Icons.FontFamily).FontSize(12).Color(Theme.StarFilled));
		else
			statsItems.Add(new Text("No ratings").FontFamily(Theme.FontRegular).FontSize(12).Color(Theme.TextMuted));

		statsItems.Add(new Text(bag.IsComplete ? "Complete" : "Active").FontFamily(Theme.FontRegular).FontSize(12).Color(bag.IsComplete ? Theme.Success : Theme.Primary));

		var statsStack = new HStack(spacing: 12);
		foreach (var s in statsItems) statsStack.Add(s);

		var infoItems = new List<Comet.View>();
		infoItems.Add(new Text($"Roasted {bag.RoastDate:MMM d, yyyy}").FontFamily(Theme.FontSemibold).FontSize(14).FontWeight(FontWeight.Bold).Color(Theme.TextPrimary));
		if (bag.Notes != null)
			infoItems.Add(new Text(bag.Notes).FontFamily(Theme.FontRegular).FontSize(12).Color(Theme.TextSecondary));
		infoItems.Add(statsStack);

		var infoStack = new VStack(spacing: 4);
		foreach (var item in infoItems) infoStack.Add(item);

		return new Border {
			new Grid(columns: new object[] { "*", "Auto" }, rows: new object[] { "Auto" })
			{
				infoStack.Cell(row: 0, column: 0),
				new Text(Icons.ChevronRight)
					.FontFamily(Icons.FontFamily)
					.FontSize(20)
					.Color(Theme.TextMuted)
					.VerticalTextAlignment(TextAlignment.Center)
					.Cell(row: 0, column: 1),
			}
		}
		.CornerRadius(Theme.RadiusCard)
		.Background(Theme.CardBackground)
		.StrokeColor(Theme.CardStroke)
		.StrokeThickness(1)
		.Padding(new Thickness(Theme.SpacingM))
		.OnTap(_ => Navigation?.Navigate(new BagDetailPage(bag.Id)));
	}
}
