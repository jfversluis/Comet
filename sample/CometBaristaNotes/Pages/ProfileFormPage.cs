using Microsoft.Maui.Storage;
using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;
using Border = Comet.Border;
using Image = Comet.Image;
using Button = Comet.Button;

namespace CometBaristaNotes.Pages;

public class ProfileFormPage : Comet.View
{
	const double AvatarSize = 100;

	readonly int _profileId;

	[State] readonly State<string> _name = new("");
	[State] readonly State<string> _avatarPath = new("");
	[State] readonly State<string> _error = new("");
	[State] readonly State<bool> _isLoaded = new(false);

	public ProfileFormPage(int profileId = 0) { _profileId = profileId; }

	void LoadProfile()
	{
		if (_profileId <= 0) { _isLoaded.Value = true; return; }

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		var profile = store.GetProfile(_profileId);
		if (profile != null)
		{
			_name.Value = profile.Name;
			_avatarPath.Value = profile.AvatarPath ?? "";
		}

		_isLoaded.Value = true;
	}

	void Save()
	{
		if (string.IsNullOrWhiteSpace(_name.Value))
		{
			_error.Value = "Please enter a profile name";
			return;
		}
		_error.Value = "";

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		if (_profileId > 0)
		{
			store.UpdateProfile(new UserProfile
			{
				Id = _profileId,
				Name = _name.Value,
				AvatarPath = string.IsNullOrEmpty(_avatarPath.Value) ? null : _avatarPath.Value,
			});
		}
		else
		{
			store.CreateProfile(new UserProfile
			{
				Name = _name.Value,
				AvatarPath = string.IsNullOrEmpty(_avatarPath.Value) ? null : _avatarPath.Value,
			});
		}

		Navigation?.Pop();
	}

	async void Delete()
	{
		if (_profileId <= 0) return;
		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		var page = CometBaristaNotes.Services.PageHelper.GetCurrentPage();
		if (page != null)
		{
			var confirmed = await page.DisplayAlertAsync(
				"Delete Profile?",
				$"Are you sure you want to delete '{_name.Value}'? This action cannot be undone.",
				"Delete",
				"Cancel");
			if (!confirmed) return;
		}

		store.DeleteProfile(_profileId);
		Navigation?.Pop();
	}

	async void PickPhoto()
	{
		try
		{
			var results = await Microsoft.Maui.Media.MediaPicker.Default.PickPhotosAsync();
			var result = results?.FirstOrDefault();
			if (result == null) return;

			var profilesDir = System.IO.Path.Combine(FileSystem.AppDataDirectory, "profiles");
			System.IO.Directory.CreateDirectory(profilesDir);

			var destPath = System.IO.Path.Combine(profilesDir, $"{(_profileId > 0 ? _profileId : 0)}.jpg");
			using var sourceStream = await result.OpenReadAsync();
			using var destStream = System.IO.File.Create(destPath);
			await sourceStream.CopyToAsync(destStream);

			_avatarPath.Value = destPath;
		}
		catch
		{
			// Photo pick cancelled or failed — ignore silently
		}
	}

	Comet.View BuildAvatar()
	{
		var hasPhoto = !string.IsNullOrEmpty(_avatarPath.Value) && System.IO.File.Exists(_avatarPath.Value);

		var items = new List<Comet.View>();

		if (hasPhoto)
		{
			items.Add(
				new Border {
					new Image(_avatarPath.Value)
						.Aspect(Aspect.AspectFill)
						.Frame(width: (float)AvatarSize, height: (float)AvatarSize)
				}
				.CornerRadius((float)(AvatarSize / 2))
				.StrokeColor(Theme.Primary)
				.StrokeThickness(2)
				.Frame(width: (float)AvatarSize, height: (float)AvatarSize)
			);
		}
		else
		{
			items.Add(
				new Border {
					new Text(Icons.Person)
						.FontFamily(Icons.FontFamily)
						.FontSize(48)
						.Color(Theme.TextMuted)
						.HorizontalTextAlignment(TextAlignment.Center)
						.VerticalTextAlignment(TextAlignment.Center)
						.Frame(width: (float)AvatarSize, height: (float)AvatarSize)
				}
				.CornerRadius((float)(AvatarSize / 2))
				.StrokeColor(Theme.Outline)
				.StrokeThickness(2)
				.Background(Theme.SurfaceVariant)
				.Frame(width: (float)AvatarSize, height: (float)AvatarSize)
			);
		}

		if (_profileId > 0)
		{
			items.Add(
				new Button(hasPhoto ? "Change Photo" : "Add Photo", PickPhoto)
					.FontFamily(Theme.FontSemibold)
					.FontSize(14)
					.Color(Theme.Primary)
					.Background(Colors.Transparent)
					.Frame(height: 36)
			);
		}

		var stack = new VStack(spacing: Theme.SpacingS);
		foreach (var item in items)
			stack.Add(item);

		return stack.Padding(new Thickness(0, Theme.SpacingS));
	}

	[Body]
	Comet.View body()
	{
		if (!_isLoaded.Value)
			LoadProfile();

		var isEdit = _profileId > 0;

		var items = new List<Comet.View>
		{
			FormHelpers.MakeSectionHeader(isEdit ? "EDIT PROFILE" : "NEW PROFILE"),
			BuildAvatar(),
			FormHelpers.MakeFormEntry("Name *", _name.Value, "Profile name", v => _name.Value = v),
		};

		if (!string.IsNullOrEmpty(_error.Value))
			items.Add(new Text(_error.Value).Color(Theme.Error).FontFamily(Theme.FontRegular).FontSize(14));

		items.Add(FormHelpers.MakePrimaryButton(isEdit ? "Save Changes" : "Create Profile", Save));

		if (isEdit)
			items.Add(FormHelpers.MakeDangerButton("Delete Profile", Delete));

		var stack = new VStack(spacing: Theme.SpacingS);
		foreach (var item in items)
			stack.Add(item);

		return new ScrollView {
			stack.Padding(new Thickness(Theme.SpacingM))
		}
		.Background(Theme.Background);
	}
}
