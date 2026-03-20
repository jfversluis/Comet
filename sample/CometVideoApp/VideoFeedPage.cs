namespace CometVideoApp;

/// <summary>
/// Full-screen vertical feed of video cards. Swipe up/down to navigate.
/// Uses ScrollView wrapping a VStack of full-screen VideoCard views.
/// Reactive state tracks the current index and per-video like state.
/// </summary>
public class VideoFeedPage : View
{
	readonly Reactive<int> currentIndex = 0;
	readonly Reactive<bool[]> likedStates = new(new bool[VideoModel.All.Length]);
	readonly Reactive<int[]> likeCounts = new(
		VideoModel.All.Select(v => v.Likes).ToArray());

	void ToggleLike(int index)
	{
		var liked = likedStates.Value;
		var counts = likeCounts.Value;
		liked[index] = !liked[index];
		counts[index] += liked[index] ? 1 : -1;
		// Trigger reactive update by reassigning arrays
		likedStates.Value = liked.ToArray();
		likeCounts.Value = counts.ToArray();
	}

	[Body]
	View body()
	{
		var videos = VideoModel.All;

		return new ZStack
		{
			// Dark background behind everything
			new BoxView(Color.FromArgb("#0a0a0a"))
				.FillHorizontal()
				.FillVertical(),

			ScrollView(Orientation.Vertical,
				VStack((float?)0,
					videos.Select((video, index) =>
						new VideoCard(
							video,
							() => likedStates.Value[index],
							() => likeCounts.Value[index],
							() => ToggleLike(index)
						)
					).ToArray()
				)
			),

			// Top gradient overlay with app title
			VStack(
				Text("Comet Video")
					.FontSize(20)
					.FontWeight(FontWeight.Bold)
					.Color(Colors.White)
					.HorizontalTextAlignment(TextAlignment.Center)
					.Margin(new Thickness(0, 54, 0, 0)),
				HStack(8,
					Text("Following")
						.FontSize(15)
						.Color(new Color(255, 255, 255, 180))
						.HorizontalTextAlignment(TextAlignment.Center),
					Text("|")
						.FontSize(15)
						.Color(new Color(255, 255, 255, 100)),
					Text("For You")
						.FontSize(15)
						.FontWeight(FontWeight.Bold)
						.Color(Colors.White)
						.HorizontalTextAlignment(TextAlignment.Center)
				).Alignment(Alignment.Center)
			)
			.FillHorizontal()
			.FitVertical()
			.Alignment(Alignment.Top)
		}.IgnoreSafeArea();
	}
}
