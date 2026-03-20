using CometMarvelousApp.Models;
using Comet.Reactive;

namespace CometMarvelousApp.Pages.Components;

public class MainCarouselViewState
{
	public WonderType CurrentType { get; set; }
	public int CurrentIndex { get; set; }
}

/// <summary>
/// A carousel-style wonder selector. Users swipe horizontally to browse wonders
/// and tap/swipe up to select one. Ported from MauiReactor's custom pan-gesture
/// carousel to a Comet CarouselView-backed approach with illustration overlays.
/// </summary>
public class MainCarouselView : View
{
	readonly WonderType _currentType;
	readonly bool _show;
	readonly Action<WonderType>? _onSelected;

	readonly Reactive<int> _selectedIndex = 0;

	static readonly WonderType[] _allWonders = Enum.GetValues<WonderType>();

	public MainCarouselView(WonderType currentType, bool show, Action<WonderType>? onSelected)
	{
		_currentType = currentType;
		_show = show;
		_onSelected = onSelected;
	}

	[Body]
	View body()
	{
		var idx = _selectedIndex.Value;
		var wonderType = _allWonders[Math.Clamp(idx, 0, _allWonders.Length - 1)];
		var config = Illustration.Config[wonderType];

		return new Comet.Grid
		{
			// Background gradient
			new BoxView(config.SecondaryColor)
				.FillHorizontal()
				.FillVertical(),

			// Main illustration image
			Image(config.MainObject)
				.Aspect(Aspect.AspectFill)
				.FillHorizontal()
				.FillVertical()
				.Opacity(0.7),

			// Foreground gradient overlay
			new BoxView(config.PrimaryColor)
				.Opacity(0.4)
				.FillHorizontal()
				.FillVertical(),

			// Wonder title
			VStack(
				Text(() =>
				{
					var i = _selectedIndex.Value;
					var wt = _allWonders[Math.Clamp(i, 0, _allWonders.Length - 1)];
					return Illustration.Config[wt].Title;
				})
					.Color(Colors.White)
					.FontFamily("YesevaOne")
					.FontSize(58)
					.HorizontalTextAlignment(TextAlignment.Center),

				// Indicator dots
				HStack(
					_allWonders.Select((wt, i) =>
						new BoxView(Colors.White)
							.Frame(width: i == idx ? 20 : 10, height: 6)
							.ClipShape(new RoundedRectangle(3))
							.Margin(2)
					).ToArray()
				).Alignment(Alignment.Center)
					.Margin(new Thickness(0, 20, 0, 0)),

				// Swipe up hint
				Image("common_arrow_indicator.png")
					.Frame(width: 30, height: 30)
					.Margin(new Thickness(0, 20, 0, 0))
					.Alignment(Alignment.Center)
			)
			.Alignment(Alignment.Bottom)
			.Margin(new Thickness(0, 0, 0, 80)),

			// Invisible buttons for navigation (left/right tap zones)
			HStack(
				Button("", () =>
				{
					if (_selectedIndex.Value > 0)
						_selectedIndex.Value--;
				})
					.Background(Colors.Transparent)
					.FillVertical()
					.Frame(width: 80),

				Spacer(),

				Button("", () =>
				{
					if (_selectedIndex.Value < _allWonders.Length - 1)
						_selectedIndex.Value++;
				})
					.Background(Colors.Transparent)
					.FillVertical()
					.Frame(width: 80)
			)
			.FillHorizontal()
			.FillVertical(),

			// Center tap zone to select
			Button("", () =>
			{
				var i = _selectedIndex.Value;
				var wt = _allWonders[Math.Clamp(i, 0, _allWonders.Length - 1)];
				_onSelected?.Invoke(wt);
			})
				.Background(Colors.Transparent)
				.Frame(width: 200, height: 200)
				.Alignment(Alignment.Center),
		}
		.Opacity(_show ? 1.0 : 0.0);
	}
}
