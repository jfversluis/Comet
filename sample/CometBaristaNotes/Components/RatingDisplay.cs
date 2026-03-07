using CometBaristaNotes.Models;
using Border = Comet.Border;

namespace CometBaristaNotes.Components;

/// <summary>
/// Factory for creating rating display using Comet fluent UI.
/// </summary>
public static class RatingDisplayFactory
{
	public static Comet.View Create(RatingAggregate rating)
	{
		return new Border
		{
			new HStack(spacing: 12)
			{
				MakeStatBlock("Avg", rating.RatedShots > 0 ? $"{rating.AverageRating:F1}" : "—"),
				MakeStatBlock("Shots", $"{rating.TotalShots}"),
				MakeStatBlock("Best", rating.BestRating?.ToString() ?? "—"),
				MakeStatBlock("Worst", rating.WorstRating?.ToString() ?? "—"),
			}
		}
		.CornerRadius(Theme.RadiusCard)
		.Background(Theme.CardBackground)
		.StrokeColor(Theme.CardStroke)
		.StrokeThickness(1)
		.Padding(new Thickness(Theme.SpacingM));
	}

	static Comet.View MakeStatBlock(string label, string value)
	{
		return new VStack(spacing: 2)
		{
			new Text(value)
				.FontFamily(Theme.FontSemibold)
				.FontWeight(FontWeight.Bold)
				.FontSize(20)
				.Color(Theme.TextPrimary),
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(Theme.TextMuted),
		};
	}
}
