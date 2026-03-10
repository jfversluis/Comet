using Comet;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class TableViewPageState
	{
		public string LastTapped { get; set; } = "Nothing tapped yet.";
	}

	public class TableViewPage : Component<TableViewPageState>
	{
		readonly Reactive<bool> syncOnCellular = true;
		readonly Reactive<bool> offlineMode = false;
		readonly Reactive<string> statusText = "Available";

		public override View Render()
		{
			var tableView = new TableView(
				new TableSection("Profile",
					new TextCell
					{
						CellText = "Account",
						Detail = "Personal details",
						OnTapped = () => SetState(s => s.LastTapped = "Account")
					},
					new EntryCell
					{
						Label = "Status",
						CellText = (Binding<string>)(() => statusText.Value),
						Placeholder = "Set your status"
					}
				),
				new TableSection("Preferences",
					new SwitchCell
					{
						CellText = "Sync over cellular",
						On = (Binding<bool>)(() => syncOnCellular.Value)
					},
					new SwitchCell
					{
						CellText = "Offline mode",
						On = (Binding<bool>)(() => offlineMode.Value)
					}
				),
				new TableSection("More",
					new TextCell
					{
						CellText = "Notifications",
						Detail = "Alerts and badges",
						OnTapped = () => SetState(s => s.LastTapped = "Notifications")
					},
					new TextCell
					{
						CellText = "Help",
						Detail = "Documentation and support",
						OnTapped = () => SetState(s => s.LastTapped = "Help")
					}
				)
			);

			return GalleryPageHelpers.Scaffold("TableView",
				GalleryPageHelpers.Section("Grouped Settings Table", "TextCell, SwitchCell, and EntryCell arranged in settings-style sections.",
					tableView.Frame(height: 360)
				),
				GalleryPageHelpers.Section("Selection Feedback", "Mirror the latest tapped cell and the current toggle/entry state.",
					GalleryPageHelpers.BodyText($"Last tapped: {State.LastTapped}"),
					GalleryPageHelpers.BodyText($"Sync over cellular: {(syncOnCellular.Value ? "On" : "Off")}"),
					GalleryPageHelpers.BodyText($"Offline mode: {(offlineMode.Value ? "On" : "Off")}"),
					GalleryPageHelpers.BodyText($"Status: {statusText.Value}")
				)
			);
		}
	}
}
