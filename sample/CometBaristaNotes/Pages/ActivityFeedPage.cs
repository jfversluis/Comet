using System.Collections.ObjectModel;
using Comet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;
using UXDivers.Popups.Services;

using MauiLabel = Microsoft.Maui.Controls.Label;
using MauiBoxView = Microsoft.Maui.Controls.BoxView;
using MauiButton = Microsoft.Maui.Controls.Button;
using MauiCollectionView = Microsoft.Maui.Controls.CollectionView;
using MauiFontAttributes = Microsoft.Maui.Controls.FontAttributes;

namespace CometBaristaNotes.Pages;

public class ActivityFeedPage : Comet.View
{
	const int _pageSize = 50;
	int _currentPage;
	int _totalShotCount;
	int _filteredShotCount;
	bool _hasMorePages = true;
	readonly ObservableCollection<ShotRecord> _displayedShots = new();
	readonly ShotFilterCriteria _filters = new();

	[State] readonly State<bool> _isLoading = new(false);
	[State] readonly State<string> _errorMessage = new("");
	[State] readonly State<int> _filterVersion = new(0);

	public ActivityFeedPage()
	{
		var notifier = IPlatformApplication.Current?.Services.GetService<IDataChangeNotifier>();
		if (notifier is not null)
		{
			notifier.DataChanged += OnDataChanged;
		}

		// Load seed data immediately
		LoadNextPage(reset: true);
		Console.WriteLine($"[ActivityFeedPage] Constructor: store={InMemoryDataStore.Instance != null}, shots={_displayedShots.Count}, total={_totalShotCount}");
	}

	void OnDataChanged(string entityType, int entityId, DataChangeType changeType)
	{
		if (entityType == "Shot")
		{
			Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
			{
				LoadNextPage(reset: true);
				_filterVersion.Value++;
			});
		}
	}

	[Body]
	Comet.View body()
	{
		// Touch _filterVersion so body rebuilds when filters change
		var _ = _filterVersion.Value;

		var shotCount = _displayedShots.Count;

		// Simple view that should always render
		if (shotCount == 0 && !_filters.HasFilters)
		{
			return new VStack
			{
				new Text("No Shots Yet")
					.FontSize(20)
					.Color(Theme.TextPrimary),
				new Text("Start logging your espresso shots to see them here.")
					.FontSize(14)
					.Color(Theme.TextSecondary),
			}.Background(Theme.Background).FillVertical().FillHorizontal();
		}

		// Main content
		var wrapper = new VerticalStackLayout { Spacing = 0, BackgroundColor = Theme.Background };

		// Header row: shot count
		wrapper.Add(new MauiBoxView { HeightRequest = 20, BackgroundColor = Colors.Transparent });

		var countText = _filters.HasFilters
			? $"{_filteredShotCount} of {_totalShotCount} shots"
			: $"{_totalShotCount} shots logged";
		wrapper.Add(new MauiLabel
		{
			Text = countText,
			FontFamily = Theme.FontSemibold,
			FontSize = 14,
			FontAttributes = MauiFontAttributes.Bold,
			TextColor = Theme.TextSecondary,
			Padding = new Thickness(Theme.SpacingM, Theme.SpacingS),
		});

		// Shot cards
		foreach (var shot in _displayedShots)
		{
			wrapper.Add(ShotRecordCardFactory.Create(shot, () =>
			{
				Navigation?.Navigate(new ShotLoggingPage(shot.Id));
			}));
		}

		return new MauiViewHost(wrapper);
	}

	async void OnFilterTapped(object? sender, EventArgs e)
	{
		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		// Build bean options: only beans that have at least one shot
		var allShots = store.GetAllShots();
		var beanNames = allShots
			.Where(s => s.BeanName != null)
			.Select(s => s.BeanName!)
			.Distinct()
			.ToList();
		var allBeans = store.GetAllBeans();
		var beanOptions = allBeans
			.Where(b => beanNames.Contains(b.Name))
			.Select(b => (b.Id, b.Name))
			.ToList();

		// Build people options: profiles that appear as MadeFor
		var madeForIds = allShots
			.Where(s => s.MadeForId.HasValue)
			.Select(s => s.MadeForId!.Value)
			.Distinct()
			.ToHashSet();
		var allProfiles = store.GetAllProfiles();
		var peopleOptions = allProfiles
			.Where(p => madeForIds.Contains(p.Id))
			.Select(p => (p.Id, p.Name))
			.ToList();

		var popup = new ShotFilterPopup(
			_filters,
			beanOptions,
			peopleOptions,
			onApply: applied =>
			{
				_filters.BeanIds = applied.BeanIds;
				_filters.MadeForIds = applied.MadeForIds;
				_filters.Ratings = applied.Ratings;
				LoadNextPage(reset: true);
				_filterVersion.Value++;
			},
			onClear: () =>
			{
				_filters.Clear();
				LoadNextPage(reset: true);
				_filterVersion.Value++;
			});

		await IPopupService.Current.PushAsync(popup);
	}

	void OnThresholdReached(object? sender, EventArgs e)
	{
		if (_hasMorePages)
		{
			LoadNextPage(reset: false);
		}
	}

	void LoadNextPage(bool reset)
	{
		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		if (reset)
		{
			_currentPage = 0;
			_hasMorePages = true;
			_displayedShots.Clear();
		}

		var allShots = store.GetAllShots();
		_totalShotCount = allShots.Count;

		// Apply filters
		var filtered = ApplyFilters(allShots);
		_filteredShotCount = filtered.Count;

		var page = filtered.Skip(_currentPage * _pageSize).Take(_pageSize).ToList();

		foreach (var shot in page)
		{
			_displayedShots.Add(shot);
		}

		_currentPage++;
		_hasMorePages = page.Count == _pageSize;
	}

	List<ShotRecord> ApplyFilters(List<ShotRecord> shots)
	{
		if (!_filters.HasFilters)
			return shots;

		var store = InMemoryDataStore.Instance;

		return shots.Where(s =>
		{
			// Bean filter: match by looking up the bag's bean ID
			if (_filters.BeanIds.Count > 0)
			{
				var bag = store?.GetBag(s.BagId);
				if (bag == null || !_filters.BeanIds.Contains(bag.BeanId))
					return false;
			}

			// Made-for filter
			if (_filters.MadeForIds.Count > 0)
			{
				if (!s.MadeForId.HasValue || !_filters.MadeForIds.Contains(s.MadeForId.Value))
					return false;
			}

			// Rating filter
			if (_filters.Ratings.Count > 0)
			{
				if (!s.Rating.HasValue || !_filters.Ratings.Contains(s.Rating.Value))
					return false;
			}

			return true;
		}).ToList();
	}
}
