using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Comet;

using View = Comet.View;
using Button = Comet.Button;
using Grid = Comet.Grid;
using Border = Comet.Border;
using Slider = Comet.Slider;
using Picker = Comet.Picker;

namespace CometBaristaNotes.Components;

/// <summary>
/// Factory methods returning Comet views for form fields and UI components.
/// </summary>
public static class FormHelpers
{
	public static View MakeIcon(string glyph, double size, Color color)
	{
		return new Text(glyph)
			.FontFamily(Icons.FontFamily)
			.FontSize(size)
			.Color(color)
			.HorizontalTextAlignment(TextAlignment.Center)
			.VerticalTextAlignment(TextAlignment.Center);
	}

	public static View MakeCard(View content)
	{
		return new Border { content }
			.CornerRadius(Theme.RadiusCard)
			.Background(Theme.CardBackground)
			.StrokeColor(Theme.CardStroke)
			.StrokeThickness(1)
			.Padding(new Thickness(Theme.SpacingM));
	}

	public static View MakeSectionHeader(string title)
	{
		return new Text(title.ToUpperInvariant())
			.FontFamily(Theme.FontSemibold)
			.FontSize(13)
			.FontWeight(FontWeight.Bold)
			.Color(Theme.TextSecondary)
			.Margin(new Thickness(0, Theme.SpacingM, 0, Theme.SpacingXS));
	}

	public static View MakeFormEntry(string label, string value, string placeholder, Action<string> onChanged)
	{
		return new VStack(spacing: 0)
		{
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(Theme.TextSecondary)
				.Margin(new Thickness(16, 0, 0, 4)),

			new Border
			{
				new TextField(value, placeholder)
					.FontSize(16)
					.Color(Theme.TextPrimary)
					.Background(Colors.Transparent)
					.Frame(height: (float)Theme.FormFieldHeight)
					.Margin(new Thickness(16, 0))
					.OnTextChanged(onChanged)
			}
			.CornerRadius(Theme.RadiusPill)
			.Background(Theme.SurfaceVariant)
			.StrokeThickness(0),
		};
	}

	public static View MakeReadOnlyField(string label, string value)
	{
		return new VStack(spacing: 0)
		{
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(Theme.TextSecondary)
				.Margin(new Thickness(16, 0, 0, 4)),

			new Border
			{
				new Text(value)
					.FontFamily(Theme.FontSemibold)
					.FontSize(16)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary)
					.VerticalTextAlignment(TextAlignment.Center)
					.Frame(height: (float)Theme.FormFieldHeight)
					.Padding(new Thickness(Theme.SpacingM, 0))
			}
			.CornerRadius(Theme.RadiusPill)
			.Background(Theme.SurfaceVariant)
			.StrokeThickness(0),
		};
	}

	public static View MakePrimaryButton(string title, Action action)
	{
		return new Button(title, action)
			.FontFamily(Theme.FontSemibold)
			.Background(Theme.Primary)
			.Color(Colors.White)
			.FontSize(16)
			.FontWeight(FontWeight.Bold)
			.Frame(height: (float)Theme.ButtonHeight)
			.CornerRadius((int)Theme.RadiusPill);
	}

	public static View MakeSecondaryButton(string title, Action action)
	{
		return new Button(title, action)
			.FontFamily(Theme.FontSemibold)
			.Background(Theme.SurfaceVariant)
			.Color(Theme.Primary)
			.FontSize(16)
			.FontWeight(FontWeight.Bold)
			.Frame(height: (float)Theme.ButtonHeight)
			.CornerRadius((int)Theme.RadiusPill);
	}

	public static View MakeDangerButton(string title, Action action)
	{
		return new Button(title, action)
			.FontFamily(Theme.FontSemibold)
			.Background(Theme.Error)
			.Color(Colors.White)
			.FontSize(16)
			.FontWeight(FontWeight.Bold)
			.Frame(height: (float)Theme.ButtonHeight)
			.CornerRadius((int)Theme.RadiusPill);
	}

	public static View MakeEmptyState(string icon, string title, string description)
	{
		return new VStack(spacing: 12)
		{
			new Text(icon)
				.FontFamily(Icons.FontFamily)
				.FontSize(48)
				.HorizontalTextAlignment(TextAlignment.Center),

			new Text(title)
				.FontFamily(Theme.FontSemibold)
				.FontSize(18)
				.FontWeight(FontWeight.Bold)
				.Color(Theme.TextPrimary)
				.HorizontalTextAlignment(TextAlignment.Center),

			new Text(description)
				.FontFamily(Theme.FontRegular)
				.FontSize(14)
				.Color(Theme.TextSecondary)
				.HorizontalTextAlignment(TextAlignment.Center),
		}
		.Padding(new Thickness(Theme.SpacingXL));
	}

	public static View MakeListCard(string title, string? subtitle, string? detail, Action? onTap)
	{
		var infoViews = new List<View>
		{
			new Text(title)
				.FontFamily(Theme.FontSemibold)
				.FontSize(16)
				.FontWeight(FontWeight.Bold)
				.Color(Theme.TextPrimary),
		};
		if (subtitle != null)
			infoViews.Add(
				new Text(subtitle)
					.FontFamily(Theme.FontRegular)
					.FontSize(14)
					.Color(Theme.TextSecondary));
		if (detail != null)
			infoViews.Add(
				new Text(detail)
					.FontFamily(Theme.FontRegular)
					.FontSize(12)
					.Color(Theme.TextMuted));

		var infoStack = new VStack(spacing: 2);
		foreach (var v in infoViews)
			infoStack.Add(v);

		var chevron = new Text(Icons.ChevronRight)
			.FontFamily(Icons.FontFamily)
			.FontSize(20)
			.Color(Theme.TextMuted)
			.VerticalTextAlignment(TextAlignment.Center)
			.Padding(new Thickness(Theme.SpacingS, 0));

		var row = new HStack(spacing: Theme.SpacingS)
		{
			infoStack.FillHorizontal(),
			chevron,
		};

		View card = new Border { row }
			.CornerRadius(Theme.RadiusCard)
			.Background(Theme.CardBackground)
			.StrokeColor(Theme.CardStroke)
			.StrokeThickness(1)
			.Padding(new Thickness(Theme.SpacingM));

		if (onTap != null)
			card = card.OnTap(_ => onTap());

		return card;
	}

	public static View MakeFormPicker(string label, int selectedIndex, string[] items, Action<int> onChanged)
	{
		return new VStack(spacing: 0)
		{
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(Theme.TextSecondary)
				.Margin(new Thickness(16, 0, 0, 4)),

			new Border
			{
				new Picker(selectedIndex, items)
					.Color(Theme.TextPrimary)
					.Background(Colors.Transparent)
					.Frame(height: (float)Theme.FormFieldHeight)
					.Margin(new Thickness(16, 0))
					.OnSelectedIndexChanged(onChanged)
			}
			.CornerRadius(Theme.RadiusPill)
			.Background(Theme.SurfaceVariant)
			.StrokeThickness(0),
		};
	}

	public static View MakeFormSlider(string label, double value, double min, double max, Action<double> onChanged)
	{
		return new VStack(spacing: 0)
		{
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(Theme.TextSecondary)
				.Margin(new Thickness(16, 0, 0, 4)),

			new Border
			{
				new Slider(value, min, max)
					.MinimumTrackColor(Theme.Primary)
					.MaximumTrackColor(Theme.SurfaceVariant)
					.Margin(new Thickness(16, 0))
					.OnValueChanged(onChanged)
			}
			.CornerRadius(Theme.RadiusPill)
			.Background(Theme.SurfaceVariant)
			.StrokeThickness(0)
			.Frame(height: (float)Theme.FormFieldHeight),
		};
	}

	public static View MakeFormEditor(string label, string value, Action<string> onChanged)
	{
		return new VStack(spacing: 0)
		{
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(Theme.TextSecondary)
				.Margin(new Thickness(16, 0, 0, 4)),

			new Border
			{
				new TextEditor(value)
					.FontSize(16)
					.Color(Theme.TextPrimary)
					.Background(Colors.Transparent)
					.Frame(height: 80)
					.Margin(new Thickness(16, 8))
					.OnTextChanged(onChanged)
			}
			.CornerRadius(Theme.RadiusEditor)
			.Background(Theme.SurfaceVariant)
			.StrokeThickness(0),
		};
	}

	public static View MakeFormEntryWithLimit(string label, string value, string placeholder, int maxLength, Action<string> onChanged)
	{
		var currentLength = value?.Length ?? 0;

		return new VStack(spacing: 0)
		{
			new Text(label)
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(Theme.TextSecondary)
				.Margin(new Thickness(16, 0, 0, 4)),

			new Border
			{
				new TextField(value, placeholder)
					.FontSize(16)
					.Color(Theme.TextPrimary)
					.Background(Theme.SurfaceVariant)
					.Frame(height: (float)Theme.FormFieldHeight)
					.OnTextChanged(text =>
					{
						text ??= string.Empty;
						if (text.Length > maxLength)
							text = text[..maxLength];
						onChanged(text);
					})
			}
			.CornerRadius(Theme.RadiusPill)
			.Background(Theme.SurfaceVariant)
			.StrokeThickness(0),

			new Text($"{currentLength}/{maxLength}")
				.FontFamily(Theme.FontRegular)
				.FontSize(12)
				.Color(currentLength >= maxLength ? Theme.Warning : Theme.TextMuted)
				.HorizontalTextAlignment(TextAlignment.End),
		};
	}

	public static View MakeToggleRow(string label, bool isOn, Action<bool> onChanged)
	{
		var grid = new Grid(columns: new object[] { "*", "Auto" }, rows: new object[] { "Auto" })
		{
			new Text(label)
				.FontFamily(Theme.FontSemibold)
				.FontSize(14)
				.FontWeight(FontWeight.Bold)
				.Color(Theme.TextPrimary)
				.VerticalTextAlignment(TextAlignment.Center)
				.Cell(row: 0, column: 0),

			new Toggle(isOn)
				.OnColor(Theme.Primary)
				.OnToggled(onChanged)
				.Cell(row: 0, column: 1),
		};

		return MakeCard(grid);
	}
}
