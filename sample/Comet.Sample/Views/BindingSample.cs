using System;
using static Comet.CometControls;

namespace Comet.Samples
{
	public class BindingSample : Component
	{
		class MyBindingObject : BindingObject
		{
			public bool CanEdit
			{
				get => GetProperty<bool>();
				set => SetProperty(value);
			}

			public string Text
			{
				get => GetProperty<string>();
				set => SetProperty(value);
			}
		}

		[State]
		readonly MyBindingObject state;

		readonly State<int> clickCount = 1;

		readonly State<bool> bar = false;

		public BindingSample()
		{
			state = new MyBindingObject
			{
				Text = "Bar",
				CanEdit = true,
			};
			
		}

		public override View Render() =>
			NavigationView(ScrollView(
				VStack(
					(state.CanEdit
						? (View) TextField(state.Text)
						: Text(() => $"{state.Text}: multiText")), // Formatted Text will warn you. This should be done by TextBinding
					Text(state.Text),
					HStack(
						Button("Toggle Entry/Label",
							() => state.CanEdit = !state.CanEdit),
						Button("Update Text",
							() => state.Text = $"Click Count: {clickCount.Value++}"),
						Button("Update FontSize",
							() => {
								var font = View.GetGlobalEnvironment<float?>(EnvironmentKeys.Fonts.Size) ?? 14;
								var size = font + 5;
								View.SetGlobalEnvironment (EnvironmentKeys.Fonts.Size, size);
							})
					),
					Toggle(state.CanEdit)
				)
			));
	}
}
