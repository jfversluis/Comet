using CometBaristaNotes.Models;
using CometBaristaNotes.Services;
using CometBaristaNotes.Components;

using ScrollView = Comet.ScrollView;

namespace CometBaristaNotes.Pages;

public class EquipmentDetailPage : Comet.View
{
	static readonly string[] TypeNames = { "Machine", "Grinder", "Tamper", "PuckScreen", "Other" };
	static readonly EquipmentType[] TypeValues =
		{ EquipmentType.Machine, EquipmentType.Grinder, EquipmentType.Tamper, EquipmentType.PuckScreen, EquipmentType.Other };

	readonly int _equipmentId;

	[State] readonly State<string> _name = new("");
	[State] readonly State<int> _selectedTypeIndex = new(0);
	[State] readonly State<string> _notes = new("");
	[State] readonly State<bool> _isLoaded = new(false);
	[State] readonly State<string> _error = new("");

	public EquipmentDetailPage(int equipmentId = 0) { _equipmentId = equipmentId; }

	void LoadEquipment()
	{
		if (_equipmentId <= 0) { _isLoaded.Value = true; return; }

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		var eq = store.GetEquipment(_equipmentId);
		if (eq == null) { _error.Value = "Equipment not found"; _isLoaded.Value = true; return; }

		_name.Value = eq.Name;
		_selectedTypeIndex.Value = Array.IndexOf(TypeValues, eq.Type);
		if (_selectedTypeIndex.Value < 0) _selectedTypeIndex.Value = 0;
		_notes.Value = eq.Notes ?? "";

		_isLoaded.Value = true;
	}

	void Save()
	{
		if (string.IsNullOrWhiteSpace(_name.Value))
		{
			_error.Value = "Equipment name is required";
			return;
		}
		_error.Value = "";

		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		var typeIdx = _selectedTypeIndex.Value;
		var eqType = (typeIdx >= 0 && typeIdx < TypeValues.Length) ? TypeValues[typeIdx] : EquipmentType.Machine;

		if (_equipmentId > 0)
		{
			store.UpdateEquipment(new Equipment
			{
				Id = _equipmentId,
				Name = _name.Value,
				Type = eqType,
				Notes = string.IsNullOrWhiteSpace(_notes.Value) ? null : _notes.Value,
				IsActive = true
			});
		}
		else
		{
			store.CreateEquipment(new Equipment
			{
				Name = _name.Value,
				Type = eqType,
				Notes = string.IsNullOrWhiteSpace(_notes.Value) ? null : _notes.Value,
			});
		}

		Navigation?.Pop();
	}

	async void Archive()
	{
		if (_equipmentId <= 0) return;
		var store = InMemoryDataStore.Instance;
		if (store == null) return;

		var page = Services.PageHelper.GetCurrentPage();
		if (page == null) return;

		var confirm = await page.DisplayAlertAsync(
			"Archive Equipment?",
			$"Are you sure you want to archive '{_name.Value}'? This action cannot be undone.",
			"Archive", "Cancel");
		if (!confirm) return;

		store.ArchiveEquipment(_equipmentId);
		Navigation?.Pop();
	}

	[Body]
	Comet.View body()
	{
		if (!_isLoaded.Value)
			LoadEquipment();

		var isEdit = _equipmentId > 0;

		var items = new List<Comet.View>
		{
			FormHelpers.MakeSectionHeader(isEdit ? "EDIT EQUIPMENT" : "NEW EQUIPMENT"),
			FormHelpers.MakeFormEntry("Name *", _name.Value, "Equipment name", v => _name.Value = v),
			FormHelpers.MakeFormPicker("Type", _selectedTypeIndex.Value, TypeNames, v => _selectedTypeIndex.Value = v),
			FormHelpers.MakeFormEntry("Notes", _notes.Value, "Additional details", v => _notes.Value = v),
		};

		if (!string.IsNullOrEmpty(_error.Value))
			items.Add(new Text(_error.Value).Color(Theme.Error).FontFamily(Theme.FontRegular).FontSize(14));

		items.Add(FormHelpers.MakePrimaryButton(isEdit ? "Save Changes" : "Add Equipment", Save));

		if (isEdit)
			items.Add(FormHelpers.MakeDangerButton("Archive Equipment", Archive));

		var stack = new VStack(spacing: Theme.SpacingS);
		foreach (var item in items)
			stack.Add(item);

		return new ScrollView {
			stack.Padding(new Thickness(Theme.SpacingM))
		}
		.Background(Theme.Background);
	}
}
