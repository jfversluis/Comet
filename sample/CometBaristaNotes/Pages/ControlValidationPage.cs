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

namespace CometBaristaNotes.Pages;

/// <summary>
/// Temporary validation page to test Round 1 Comet framework features:
/// Border with corner radius/stroke, Picker, ActivityIndicator with bindable IsRunning.
/// </summary>
public class ControlValidationPage : Comet.View
{
	readonly State<int> _pickerIndex = 0;
	readonly State<bool> _isLoading = false;
	readonly State<double> _sliderValue = 50.0;

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
			}
			.Padding(new Thickness(16))
		};
}
