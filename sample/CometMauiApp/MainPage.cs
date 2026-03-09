using System;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometMauiApp
{
	public class CounterState
	{
		public int Count { get; set; }

		public int Step { get; set; } = 1;

		public bool CelebrateMilestones { get; set; } = true;
	}

	public class MainPage : Component<CounterState>
	{
		readonly Reactive<string> status = "Ready to count with Comet.";

		public override View Render()
		{
			var accent = State.CelebrateMilestones ? Color.FromArgb("#6750A4") : Color.FromArgb("#1D3557");
			var background = Color.FromArgb("#F5F7FB");
			var banner = State.Count == 0
				? "Tap Increment to drive the first update."
				: State.CelebrateMilestones && State.Count % 5 == 0
					? $"Milestone hit: {State.Count} total taps."
					: $"The evolved MVU surface has rendered {State.Count} updates.";

			return NavigationView(
				ScrollView(
					VStack(20,
						BuildHeroCard(accent, banner),
						BuildActionCard(accent),
						BuildStatusCard(accent)
					)
					.Padding(new Thickness(24))
				)
				.Background(background)
			)
			.Title("Comet Counter");
		}

		View BuildHeroCard(Color accent, string banner)
		{
			return Border(
				VStack(12,
					Text("Comet Counter")
						.FontSize(30)
						.FontWeight(FontWeight.Bold)
						.Color(Colors.White)
						.SemanticHeadingLevel(SemanticHeadingLevel.Level1),

					Text("Component<TState> + Render() + SetState()")
						.FontSize(16)
						.Color(Colors.White),

					Text($"Count: {State.Count}")
						.FontSize(42)
						.FontWeight(FontWeight.Bold)
						.Color(Colors.White),

					Text(banner)
						.FontSize(14)
						.Color(Colors.White)
				)
			)
			.Background(accent)
			.CornerRadius(24)
			.Padding(new Thickness(24));
		}

		View BuildActionCard(Color accent)
		{
			return Border(
				VStack(18,
					Text("Adjust the update loop")
						.FontSize(20)
						.FontWeight(FontWeight.Bold)
						.Color(Color.FromArgb("#1F1F1F")),

					Text($"Step size: {State.Step}")
						.FontSize(14)
						.Color(Color.FromArgb("#4E5B6A")),

					Slider(State.Step, 1, 5)
						.MinimumTrackColor(accent)
						.MaximumTrackColor(Color.FromArgb("#D0D7E2"))
						.OnValueChanged(value => SetState(s => s.Step = Math.Max(1, (int)Math.Round(value))))
						.AutomationId("counter-step-slider"),

					HStack(12,
						Toggle(State.CelebrateMilestones)
							.OnColor(accent)
							.OnToggled(isOn => {
								SetState(s => s.CelebrateMilestones = isOn);
								status.Value = isOn
									? "Milestone celebrations are enabled."
									: "Milestone celebrations are paused.";
							})
							.AutomationId("counter-celebrate-toggle"),

						Text(State.CelebrateMilestones
								? "Celebrate every fifth increment."
								: "Run quiet updates for raw counter flow.")
							.FontSize(13)
							.Color(Color.FromArgb("#4E5B6A"))
							.VerticalTextAlignment(TextAlignment.Center)
					),

					HStack(12,
						Button("Increment", Increment)
							.Background(accent)
							.Padding(new Thickness(12, 0))
							.Color(Colors.White)
							.Frame(height: 48)
							.CornerRadius(18)
							.SemanticHint("Adds the selected step size to the counter")
							.AutomationId("counter-increment-button"),

						Button("Decrement", Decrement)
							.Background(Color.FromArgb("#DCE3F2"))
							.Padding(new Thickness(12, 0))
							.Color(Color.FromArgb("#1D3557"))
							.Frame(height: 48)
							.CornerRadius(18)
							.AutomationId("counter-decrement-button"),

						Button("Reset", Reset)
							.Background(Color.FromArgb("#F2E8E8"))
							.Padding(new Thickness(12, 0))
							.Color(Color.FromArgb("#8C3A3A"))
							.Frame(height: 48)
							.CornerRadius(18)
							.AutomationId("counter-reset-button")
					)
				)
			)
			.Background(Colors.White)
			.CornerRadius(24)
			.Padding(new Thickness(20));
		}

		View BuildStatusCard(Color accent)
		{
			var nextMilestone = ((State.Count / 5) + 1) * 5;
			var milestoneText = State.CelebrateMilestones
				? $"Next milestone: {nextMilestone}"
				: "Milestones are currently disabled.";

			return Border(
				VStack(10,
					Text("What this sample is showing")
						.FontSize(20)
						.FontWeight(FontWeight.Bold)
						.Color(Color.FromArgb("#1F1F1F")),

					Text("The page itself is a Component<CounterState>. Buttons mutate state with SetState(...), while the status line below uses Reactive<T> for lightweight updates.")
						.FontSize(14)
						.Color(Color.FromArgb("#4E5B6A")),

					Text(() => status.Value)
						.FontSize(14)
						.Color(accent),

					Text(milestoneText)
						.FontSize(14)
						.Color(Color.FromArgb("#4E5B6A"))
				)
			)
			.Background(Colors.White)
			.CornerRadius(24)
			.Padding(new Thickness(20));
		}

		void Increment()
		{
			SetState(s => s.Count += s.Step);
			status.Value = $"Incremented by {State.Step}. Current count: {State.Count}.";
		}

		void Decrement()
		{
			SetState(s => s.Count = Math.Max(0, s.Count - s.Step));
			status.Value = $"Decremented by {State.Step}. Current count: {State.Count}.";
		}

		void Reset()
		{
			SetState(s => s.Count = 0);
			status.Value = "Counter reset to zero.";
		}
	}
}
