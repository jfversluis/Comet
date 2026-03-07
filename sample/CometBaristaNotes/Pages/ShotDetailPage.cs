using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;
using Border = Comet.Border;
using Grid = Comet.Grid;

namespace CometBaristaNotes.Pages;

/// <summary>
/// Displays the details of a previously logged shot.
/// Navigated to from the Activity feed when tapping a shot card.
/// </summary>
public class ShotDetailPage : Comet.View
{
	readonly int _shotId;
	ShotRecord? _shot;

	public ShotDetailPage(int shotId)
	{
		_shotId = shotId;
		_shot = InMemoryDataStore.Instance?.GetShot(shotId);
	}

	[Body]
	Comet.View body()
	{
		if (_shot == null)
		{
			return new VStack
			{
				FormHelpers.MakeEmptyState(Icons.Coffee, "Shot Not Found", "This shot could not be loaded."),
			}
			.Background(Theme.Background)
			.FillVertical();
		}

		var items = new List<Comet.View>
		{
			// Header
			new Text(_shot.Timestamp.ToString("dddd, MMMM d 'at' h:mm tt"))
				.FontFamily(Theme.FontRegular)
				.FontSize(14)
				.Color(Theme.TextSecondary),

			new Text(_shot.DrinkType)
				.FontFamily(Theme.FontSemibold)
				.FontSize(28)
				.Color(Theme.TextPrimary),

			new Spacer().Frame(height: 8),

			// Dose card
			BuildCard("Dose",
				BuildStatRow("In", $"{_shot.DoseIn:F1}g"),
				BuildStatRow("Out", _shot.ActualOutput.HasValue ? $"{_shot.ActualOutput.Value:F1}g" : "—"),
				BuildStatRow("Ratio", _shot.DoseIn > 0 && _shot.ActualOutput.HasValue
					? $"1:{(_shot.ActualOutput.Value / _shot.DoseIn):F1}"
					: "—")
			),

			// Time card
			BuildCard("Extraction",
				BuildStatRow("Expected", $"{_shot.ExpectedTime:F0}s"),
				BuildStatRow("Actual", _shot.ActualTime.HasValue ? $"{_shot.ActualTime.Value:F1}s" : "—"),
				BuildStatRow("Grind", string.IsNullOrEmpty(_shot.GrindSetting) ? "—" : _shot.GrindSetting)
			),
		};

		// Equipment card
		if (!string.IsNullOrEmpty(_shot.MachineName) || !string.IsNullOrEmpty(_shot.GrinderName))
		{
			var equipmentRows = new List<Comet.View>();
			if (!string.IsNullOrEmpty(_shot.MachineName))
				equipmentRows.Add(BuildStatRow("Machine", _shot.MachineName));
			if (!string.IsNullOrEmpty(_shot.GrinderName))
				equipmentRows.Add(BuildStatRow("Grinder", _shot.GrinderName));
			items.Add(BuildCard("Equipment", equipmentRows.ToArray()));
		}

		// Bean card
		if (!string.IsNullOrEmpty(_shot.BeanName))
		{
			items.Add(BuildCard("Coffee",
				BuildStatRow("Bean", _shot.BeanName),
				BuildStatRow("Bag", _shot.BagDisplayName ?? "—")
			));
		}

		// Rating
		if (_shot.Rating.HasValue && _shot.Rating.Value >= 0 && _shot.Rating.Value <= 4)
		{
			var sentiments = new[]
			{
				Icons.SentimentVeryDissatisfied,
				Icons.SentimentDissatisfied,
				Icons.SentimentNeutral,
				Icons.SentimentSatisfied,
				Icons.SentimentVerySatisfied,
			};
			var ratingRow = new HStack(spacing: Theme.SpacingS);
			for (int i = 0; i < sentiments.Length; i++)
			{
				var color = i == _shot.Rating.Value ? Theme.Primary : Theme.StarEmpty;
				ratingRow.Add(new Text(sentiments[i])
					.FontFamily(Icons.FontFamily)
					.FontSize(28)
					.Color(color));
			}
			items.Add(BuildCard("Rating", ratingRow));
		}

		// Tasting notes
		if (!string.IsNullOrEmpty(_shot.TastingNotes))
		{
			items.Add(BuildCard("Tasting Notes",
				new Text(_shot.TastingNotes)
					.FontFamily(Theme.FontRegular)
					.FontSize(16)
					.Color(Theme.TextPrimary)
			));
		}

		// Bottom padding
		items.Add(new Spacer().Frame(height: 40));

		var stack = new VStack(spacing: Theme.SpacingS);
		foreach (var item in items)
			stack.Add(item);
		stack.Padding(new Thickness(Theme.SpacingM));

		return new ScrollView { stack }.Background(Theme.Background);
	}

	static Comet.View BuildCard(string title, params Comet.View[] children)
	{
		var stack = new VStack(spacing: Theme.SpacingS);
		stack.Add(new Text(title.ToUpperInvariant())
			.FontFamily(Theme.FontSemibold)
			.FontSize(11)
			.Color(Theme.TextSecondary));
		foreach (var child in children)
			stack.Add(child);

		return new Border { stack }
			.CornerRadius(16)
			.Background(Theme.Surface)
			.StrokeThickness(0)
			.Padding(new Thickness(Theme.SpacingM))
			.Margin(new Thickness(0, 4));
	}

	static Comet.View BuildStatRow(string label, string value) =>
		new Grid(columns: new object[] { 100, "*" }, rows: new object[] { "Auto" })
		{
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(15)
				.Color(Theme.TextSecondary)
				.VerticalTextAlignment(TextAlignment.Center)
				.Cell(row: 0, column: 0),
			new Text(value)
				.FontFamily(Theme.FontSemibold)
				.FontSize(15)
				.Color(Theme.TextPrimary)
				.VerticalTextAlignment(TextAlignment.Center)
				.Cell(row: 0, column: 1),
		};
}
