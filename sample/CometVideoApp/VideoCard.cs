namespace CometVideoApp;

/// <summary>
/// A single video card in the feed — placeholder image with play button overlay,
/// creator info at the bottom, and like/comment/share action buttons on the right.
/// </summary>
public class VideoCard : View
{
	readonly VideoModel video;
	readonly Func<bool> isLiked;
	readonly Func<int> likeCount;
	readonly Action onToggleLike;

	public VideoCard(VideoModel video, Func<bool> isLiked, Func<int> likeCount, Action onToggleLike)
	{
		this.video = video;
		this.isLiked = isLiked;
		this.likeCount = likeCount;
		this.onToggleLike = onToggleLike;
	}

	[Body]
	View body() => new ZStack
	{
		// Colored placeholder background simulating a video thumbnail
		new BoxView(Color.FromArgb(video.ThumbnailColor))
			.FillHorizontal()
			.FillVertical(),

		// Centered play button overlay
		VStack(
			Text("▶")
				.FontSize(64)
				.Color(new Color(255, 255, 255, 180))
				.HorizontalTextAlignment(TextAlignment.Center),
			Text(video.Title)
				.FontSize(16)
				.Color(new Color(255, 255, 255, 140))
				.HorizontalTextAlignment(TextAlignment.Center)
		).Alignment(Alignment.Center),

		// Bottom-left: creator info and description
		VStack(4,
			Text(video.Creator)
				.FontSize(16)
				.FontWeight(FontWeight.Bold)
				.Color(Colors.White),
			Text(video.Description)
				.FontSize(14)
				.Color(new Color(255, 255, 255, 220))
		)
		.Margin(new Thickness(16, 0, 80, 40))
		.Alignment(Alignment.BottomLeading),

		// Right side: action buttons (like, comment, share)
		VStack(20,
			ActionButton(
				() => isLiked() ? "❤️" : "🤍",
				() => VideoModel.FormatCount(likeCount()),
				onToggleLike),
			ActionButton(
				() => "💬",
				() => VideoModel.FormatCount(video.Comments),
				() => { }),
			ActionButton(
				() => "↗",
				() => VideoModel.FormatCount(video.Shares),
				() => { })
		)
		.Margin(new Thickness(0, 0, 12, 100))
		.Alignment(Alignment.BottomTrailing)
	}.Frame(height: 680);

	static View ActionButton(Func<string> icon, Func<string> label, Action onTap) =>
		VStack(2,
			Button(icon, onTap)
				.FontSize(28)
				.Color(Colors.White)
				.Background(Colors.Transparent)
				.Frame(width: 48, height: 48),
			Text(label)
				.FontSize(12)
				.Color(new Color(255, 255, 255, 200))
				.HorizontalTextAlignment(TextAlignment.Center)
		);
}
