using Comet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;
using Border = Comet.Border;

namespace CometBaristaNotes.Pages;

public class SettingsPage : Comet.View
{
	readonly IThemeService _themeService;
	[State] readonly State<AppThemeMode> _themeMode;

	public SettingsPage()
	{
		_themeService = IPlatformApplication.Current!.Services.GetRequiredService<IThemeService>();
		_themeService.LoadSavedTheme();
		_themeMode = new(_themeService.CurrentMode);
	}

	[Body]
	Comet.View body() =>
		new ScrollView {
			new VStack(spacing: Theme.SpacingM) {
				FormHelpers.MakeSectionHeader("APPEARANCE"),
				BuildAppearanceButtons(),
				FormHelpers.MakeSectionHeader("MANAGE"),
				BuildManageItem("Equipment", "Manage machines, grinders", () =>
					Navigation?.Navigate(new EquipmentManagementPage())),
				BuildManageItem("Beans", "Manage coffee beans", () =>
					Navigation?.Navigate(new BeanManagementPage())),
				BuildManageItem("User Profiles", "Manage household members", () =>
					Navigation?.Navigate(new UserProfileManagementPage())),
				FormHelpers.MakeSectionHeader("ABOUT"),
				BuildAboutCard(),
			}
			.Padding(new Thickness(Theme.SpacingM))
		}
		.Background(Theme.Background);

	Comet.View BuildAppearanceButtons() =>
		new HStack(spacing: Theme.SpacingS) {
			BuildThemeButton(Icons.LightMode, "Light", AppThemeMode.Light),
			BuildThemeButton(Icons.DarkMode, "Dark", AppThemeMode.Dark),
			BuildThemeButton(Icons.BrightnessAuto, "Auto", AppThemeMode.System),
		};

	Comet.View BuildThemeButton(string icon, string label, AppThemeMode mode)
	{
		var isSelected = _themeMode.Value == mode;
		return new Border {
			new VStack(spacing: 4) {
				new Text(icon)
					.FontFamily(Icons.FontFamily)
					.FontSize(24)
					.HorizontalTextAlignment(TextAlignment.Center),
				new Text(label)
					.FontFamily(Theme.FontRegular)
					.FontSize(12)
					.Color(isSelected ? Theme.Primary : Theme.TextSecondary)
					.HorizontalTextAlignment(TextAlignment.Center),
			}
		}
		.CornerRadius(Theme.RadiusCard)
		.Background(isSelected ? Theme.Primary.WithAlpha(0.15f) : Theme.CardBackground)
		.StrokeColor(isSelected ? Theme.Primary : Theme.CardStroke)
		.StrokeThickness(1)
		.Frame(width: 100, height: 64)
		.Padding(new Thickness(8))
		.OnTap(_ => {
			_themeMode.Value = mode;
			_themeService.SetTheme(mode);
		});
	}

	Comet.View BuildManageItem(string title, string description, Action onTap) =>
		FormHelpers.MakeListCard(title, description, null, onTap);

	Comet.View BuildAboutCard() =>
		FormHelpers.MakeCard(
			new VStack(spacing: Theme.SpacingXS) {
				new Text("BaristaNotes")
					.FontFamily(Theme.FontSemibold)
					.FontSize(18)
					.FontWeight(Microsoft.Maui.FontWeight.Bold)
					.Color(Theme.TextPrimary),
				new Text("Version 1.0")
					.FontFamily(Theme.FontRegular)
					.FontSize(14)
					.Color(Theme.TextSecondary),
				new Text("Track your espresso journey")
					.FontFamily(Theme.FontRegular)
					.FontSize(14)
					.Color(Theme.TextSecondary)
					.Margin(new Thickness(0, Theme.SpacingXS, 0, 0)),
			}
		);
}
