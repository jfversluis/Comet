using Comet;
using CometBaristaNotes.Components;
using Microsoft.Maui.Graphics;
using Border = Comet.Border;
using Button = Comet.Button;
using Picker = Comet.Picker;
using ActivityIndicator = Comet.ActivityIndicator;
using ScrollView = Comet.ScrollView;
using Toggle = Comet.Toggle;
using Slider = Comet.Slider;
using Image = Comet.Image;
using ProgressBar = Comet.ProgressBar;
using DatePicker = Comet.DatePicker;
using TextEditor = Comet.TextEditor;
using Grid = Comet.Grid;

namespace CometBaristaNotes.Pages;

/// <summary>
/// Validation page to test Comet framework features across all rounds.
/// </summary>
public class ControlValidationPage : Comet.View
{
	readonly State<int> _pickerIndex = 0;
	readonly State<bool> _isLoading = false;
	readonly State<double> _sliderValue = 50.0;
	readonly State<double> _progressValue = 0.5;
	readonly State<DateTime?> _selectedDate = (DateTime?)DateTime.Today;
	readonly State<string> _editorText = "";

	[Body]
	Comet.View body() =>
		new ScrollView
		{
			new VStack(spacing: 16)
			{
				// Section 1: Border with CornerRadius/Stroke/Background
				new Text("Border Tests")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new Border
				{
					new VStack(spacing: 8)
					{
						new Text("Card with CornerRadius(12)")
							.Color(Theme.TextPrimary),
						new Text("Surface background, brown stroke")
							.Color(Theme.TextSecondary)
							.FontSize(14),
					}
				}
				.CornerRadius(12)
				.StrokeColor(Theme.Primary)
				.StrokeThickness(1)
				.Background(Theme.Surface)
				.Padding(new Thickness(16)),

				new Border
				{
					new Text("Pill shape (CornerRadius 25)")
						.Color(Colors.White)
				}
				.CornerRadius(25)
				.Background(Theme.Primary)
				.Padding(new Thickness(16, 10)),

				// Section 2: Picker
				new Text("Picker Test")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new Picker(_pickerIndex, "Espresso", "Latte", "Cappuccino", "Flat White", "Americano")
					.Background(Theme.Surface),

				new Text(() => $"Selected: {_pickerIndex.Value}")
					.Color(Theme.TextSecondary),

				// Section 3: ActivityIndicator
				new Text("ActivityIndicator Test")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new ActivityIndicator(_isLoading.Value)
					.Color(Theme.Primary),

				new Button("Toggle Loading", () => _isLoading.Value = !_isLoading.Value)
					.Color(Colors.White)
					.Background(Theme.Primary),

				// Section 4: TextField in Border (form field pattern)
				new Text("Form Field Pattern")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new Border
				{
					new TextField("", "Enter bean name...")
						.FontSize(16)
						.FontFamily(Theme.FontRegular)
				}
				.CornerRadius(Theme.RadiusPill)
				.Background(Theme.SurfaceVariant)
				.Padding(new Thickness(16, 8)),

				// Section 5: Nested Borders (card within card)
				new Border
				{
					new VStack(spacing: 12)
					{
						new Text("Shot Record")
							.FontSize(18)
							.FontWeight(FontWeight.Bold)
							.Color(Theme.TextPrimary),
						new Border
						{
							new HStack(spacing: 8)
							{
								new Text("18.0g → 36.0g")
									.Color(Theme.TextPrimary),
								new Spacer(),
								new Text("25s")
									.Color(Theme.TextSecondary),
							}
						}
						.CornerRadius(8)
						.Background(Theme.SurfaceVariant)
						.Padding(new Thickness(12, 8)),
					}
				}
				.CornerRadius(Theme.RadiusCard)
				.StrokeColor(Theme.Outline)
				.StrokeThickness(1)
				.Background(Theme.Surface)
				.Padding(new Thickness(16)),

				// ---- Round 2: TextField PlaceholderColor ----
				new Text("Round 2: Styling")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new TextField("", "Placeholder with custom color...")
					.PlaceholderColor(Theme.Primary)
					.FontSize(16),

				// Round 2: Toggle with OnColor/ThumbColor
				new Text("Toggle (OnColor/ThumbColor)")
					.Color(Theme.TextPrimary),

				new Toggle(_isLoading)
					.OnColor(Theme.Primary)
					.ThumbColor(Colors.White),

				// Round 2: Slider with track/thumb colors
				new Text(() => $"Slider: {_sliderValue.Value:F1}")
					.Color(Theme.TextPrimary),

				new Slider(_sliderValue, 0, 100)
					.MinimumTrackColor(Theme.Primary)
					.MaximumTrackColor(Theme.Outline)
					.ThumbColor(Theme.Primary),

				// Round 2: Image with Aspect
				new Text("Image (AspectFit)")
					.Color(Theme.TextPrimary),

				new Image("dotnet_bot.png")
					.Aspect(Microsoft.Maui.Aspect.AspectFit)
					.Frame(height: 100),

				// ---- Round 4: Keyboard, Shadow ----
				new Text("Round 4: Keyboard & Shadow")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new TextField("", "Numeric keyboard...")
					.Keyboard(Microsoft.Maui.Keyboard.Numeric)
					.FontSize(16),

				new TextField("", "Email keyboard...")
					.Keyboard(Microsoft.Maui.Keyboard.Email)
					.ReturnType(Microsoft.Maui.ReturnType.Send)
					.FontSize(16),

				new Border
				{
					new Text("Card with Shadow")
						.Color(Theme.TextPrimary)
				}
				.CornerRadius(12)
				.Background(Theme.Surface)
				.Padding(new Thickness(16))
				.Shadow(new Comet.Graphics.Shadow()
					.WithColor(Colors.Black)
					.WithOpacity(0.3f)
					.WithRadius(8)
					.WithOffset(new Point(0, 4))),

				// ---- Round 5: Grid, ProgressBar, DatePicker, TextEditor ----
				new Text("Round 5: Grid Layout")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new Grid(
					rows: new object[] { "Auto", "Auto" },
					columns: new object[] { "*", "*" })
				{
					new Text("Row 0, Col 0")
						.Color(Theme.TextPrimary)
						.Cell(row: 0, column: 0),
					new Text("Row 0, Col 1")
						.Color(Theme.Primary)
						.Cell(row: 0, column: 1),
					new Text("Row 1, Span 2")
						.Color(Theme.TextSecondary)
						.GridRow(1).GridColumnSpan(2),
				}
				.Frame(height: 80),

				new Text("ProgressBar")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new ProgressBar(_progressValue)
					.ProgressColor(Theme.Primary)
					.TrackColor(Theme.SurfaceVariant),

				new Button("Advance Progress", () =>
					_progressValue.Value = _progressValue.Value >= 1.0 ? 0.0 : _progressValue.Value + 0.1)
					.Color(Colors.White)
					.Background(Theme.Primary),

				new Text("DatePicker")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new DatePicker(_selectedDate)
					.TextColor(Theme.Primary),

				new Text(() => $"Selected date: {_selectedDate.Value:yyyy-MM-dd}")
					.Color(Theme.TextSecondary),

				new Text("TextEditor (multi-line)")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new Border
				{
					new TextEditor(_editorText)
						.PlaceholderColor(Theme.TextSecondary)
						.Keyboard(Microsoft.Maui.Keyboard.Default)
						.Frame(height: 100)
				}
				.CornerRadius(8)
				.Background(Theme.SurfaceVariant)
				.Padding(new Thickness(8)),

				new Text(() => $"Editor text: {_editorText.Value}")
					.Color(Theme.TextSecondary),

				// ---- Round 6: Grid spacing, Button styling, OnTextChanged ----
				new Text("Round 6: Grid with Spacing")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new Grid(
					rows: new object[] { "Auto", "Auto" },
					columns: new object[] { "*", "*" })
				{
					new Border
					{
						new Text("Col 0").Color(Colors.White)
					}
					.CornerRadius(8)
					.Background(Theme.Primary)
					.Padding(new Thickness(12, 8))
					.Cell(row: 0, column: 0),

					new Border
					{
						new Text("Col 1").Color(Theme.TextPrimary)
					}
					.CornerRadius(8)
					.Background(Theme.SurfaceVariant)
					.Padding(new Thickness(12, 8))
					.Cell(row: 0, column: 1),

					new Text("Row 1, full width")
						.Color(Theme.TextSecondary)
						.GridRow(1).GridColumnSpan(2),
				}
				.ColumnSpacing(12)
				.RowSpacing(8),

				new Text("Button Styling")
					.FontSize(22)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.TextPrimary),

				new Button("Rounded Button", () => { })
					.Color(Colors.White)
					.Background(Theme.Primary)
					.CornerRadius(20),

				new Button("Outlined Button", () => { })
					.Color(Theme.Primary)
					.Background(Colors.Transparent)
					.CornerRadius(8)
					.BorderWidth(1.5)
					.BorderColor(Theme.Primary),

				new Spacer().Frame(height: 40),
			}
			.Padding(new Thickness(16))
		};
}
