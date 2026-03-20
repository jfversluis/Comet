using System.Collections.Generic;
using Comet;
using CometSurfingApp.Models;
using CometSurfingApp.Services;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometSurfingApp.Pages
{
	/// <summary>
	/// Comet port of the MauiReactor SurfingApp.
	/// Original: https://github.com/nicolgit/mauireactor-samples/tree/main/SurfingApp
	/// Design:  https://github.com/jsuarezruiz/netmaui-surfing-app-challenge
	///
	/// Renders a social-style surf feed with a horizontal user avatar strip
	/// and a vertical list of post cards, each with an image overlay, user
	/// info, like count, and title.
	/// </summary>
	public class MainPage : View
	{
		[Body]
		View body()
		{
			var users = UserService.Instance.GetUsers();
			var posts = PostService.Instance.GetPosts();

			return new Grid(
				rows: new object[] { 48, "Auto", "*" },
				columns: new object[] { "*" })
			{
				// ── Header row ──────────────────────────────────────
				BuildHeader(),

				// ── User avatars (horizontal scroll) ────────────────
				BuildUserStrip(users)
					.Cell(row: 1, column: 0),

				// ── Posts feed (vertical scroll) ────────────────────
				BuildPostFeed(posts)
					.Cell(row: 2, column: 0),
			};
		}

		// ── Header ──────────────────────────────────────────────────
		View BuildHeader()
		{
			return new Grid(
				rows: new object[] { "*" },
				columns: new object[] { "Auto", "*", "Auto" })
			{
				// Hamburger icon placeholder
				Text("☰")
					.FontSize(22)
					.Color(Colors.Black)
					.Margin(new Thickness(24, 6))
					.VerticalTextAlignment(TextAlignment.Center)
					.Cell(row: 0, column: 0),

				// Title
				Text("Surfers")
					.FontSize(28)
					.FontWeight(FontWeight.Bold)
					.Color(Colors.Black)
					.HorizontalTextAlignment(TextAlignment.Center)
					.VerticalTextAlignment(TextAlignment.Center)
					.Cell(row: 0, column: 1),

				// Search icon placeholder
				Text("🔍")
					.FontSize(20)
					.Margin(new Thickness(24, 6))
					.VerticalTextAlignment(TextAlignment.Center)
					.Cell(row: 0, column: 2),
			}.Cell(row: 0, column: 0);
		}

		// ── Horizontal user avatar strip ────────────────────────────
		View BuildUserStrip(List<User> users)
		{
			var hstack = new HStack(spacing: 6);
			foreach (var user in users)
			{
				hstack.Add(BuildUserAvatar(user));
			}
			return ScrollView(Orientation.Horizontal, hstack)
				.Padding(new Thickness(24, 0))
				.Margin(new Thickness(0, 18));
		}

		View BuildUserAvatar(User user)
		{
			return Image(user.Image)
				.Frame(width: 56, height: 56)
				.ClipShape(new Ellipse())
				.Aspect(Aspect.AspectFill)
				.RoundedBorder(radius: 28, color: user.Color, strokeSize: 4);
		}

		// ── Vertical post feed ──────────────────────────────────────
		View BuildPostFeed(List<Post> posts)
		{
			var vstack = new VStack(spacing: 36);
			foreach (var post in posts)
			{
				vstack.Add(BuildPostCard(post));
			}
			return ScrollView(Orientation.Vertical, vstack)
				.Padding(new Thickness(24, 12, 14, 0));
		}

		// ── Individual post card ────────────────────────────────────
		View BuildPostCard(Post post)
		{
			return new ZStack
			{
				// Background image fills the card
				Image(post.Image)
					.Aspect(Aspect.AspectFill)
					.FillHorizontal()
					.FillVertical(),

				// Dark overlay for contrast
				new Spacer()
					.Background(Colors.Black)
					.Opacity(0.1f),

				// Content overlay
				new Grid(
					rows: new object[] { "Auto", "Auto", "*" },
					columns: new object[] { 72, "*" })
				{
					// User avatar (top-left)
					Image(post.User.Image)
						.Frame(width: 54, height: 54)
						.Aspect(Aspect.AspectFill)
						.ClipShape(new Ellipse())
						.RoundedBorder(radius: 27, color: post.User.Color, strokeSize: 4)
						.Margin(new Thickness(12))
						.Cell(row: 0, column: 0),

					// User name + time
					VStack(
						Text(post.User.Name.ToUpper())
							.FontWeight(FontWeight.Bold)
							.Color(Colors.Black),
						Text("4 HOURS AGO")
							.FontSize(10)
							.Color(Colors.Black)
							.Opacity(0.75f)
					)
					.Margin(new Thickness(0, 18))
					.Cell(row: 0, column: 1),

					// Likes + bookmark row
					HStack(spacing: 8,
						Text("♥")
							.FontSize(14)
							.Color(Colors.Black),
						Text(post.Likes)
							.FontSize(10)
							.Margin(new Thickness(2, 0)),
						new Spacer(),
						Text("🔖")
							.FontSize(14)
							.Color(Colors.Black)
					)
					.Margin(new Thickness(12, 0))
					.Cell(row: 1, column: 0)
					.GridColumnSpan(2),

					// Play button + title area (bottom)
					HStack(
						// Play button circle
						Text("▶")
							.FontSize(16)
							.Color(Colors.Black)
							.HorizontalTextAlignment(TextAlignment.Center)
							.VerticalTextAlignment(TextAlignment.Center)
							.Frame(width: 44, height: 44)
							.ClipShape(new Ellipse())
							.Background(Colors.White)
							.Shadow(Colors.Black, 4, 0, 2)
							.Margin(new Thickness(12)),

						// Title + location
						VStack(
							Text(post.Title)
								.FontSize(18)
								.FontWeight(FontWeight.Bold)
								.Color(Colors.White)
								.LineBreakMode(LineBreakMode.WordWrap)
								.Shadow(Colors.Black.WithAlpha(0.5f), 2, 0, 1),
							Text(post.User.From.ToUpper())
								.FontSize(12)
								.Color(Colors.LightGray)
								.Opacity(0.95f)
						)
					)
					.Margin(new Thickness(0, 12))
					.Cell(row: 2, column: 0)
					.GridColumnSpan(2)
					.Alignment(Alignment.Bottom),
				},
			}
			.Frame(height: 240)
			.ClipShape(new RoundedRectangle(12))
			.Shadow(Colors.Black.WithAlpha(0.15f), 4, 1, 1);
		}
	}
}
