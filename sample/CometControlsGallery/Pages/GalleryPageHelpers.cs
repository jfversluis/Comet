using System;
using System.Collections.Generic;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	internal static class GalleryPageHelpers
	{
		static readonly SectionCard Card = new();

		public static View Scaffold(string title, params View[] sections) =>
			ScrollView(
				VStack(24, sections)
					.Padding(new Thickness(20))
			)
			.Background(ColorTokens.Background)
			.Title(title);

		public static View Section(string title, string description, params View[] content)
		{
			var views = new List<View>
			{
				SectionHeader(title),
				Text(description)
					.Typography(TypographyTokens.BodyMedium)
					.Color(ColorTokens.OnSurfaceVariant)
			};

			views.AddRange(content);

			return Border(
				VStack(12, views.ToArray())
			)
			.Modifier(Card);
		}

		public static View SectionHeader(string title) =>
			Text(title)
				.Typography(TypographyTokens.TitleLarge)
				.Color(ColorTokens.OnSurface);

		public static View BodyText(string value) =>
			Text(value)
				.Typography(TypographyTokens.BodyMedium)
				.Color(ColorTokens.OnSurface);

		public static View Caption(string value) =>
			Text(value)
				.Typography(TypographyTokens.BodySmall)
				.Color(ColorTokens.OnSurfaceVariant);

		public static View NavButton(string label, Action action) =>
			Button(label, action)
				.ButtonStyle(ButtonStyles.Outlined);

		public static View ColorBlock(string label, Color background, Color textColor, float height = 56) =>
			Border(
				Text(label)
					.FontWeight(FontWeight.Bold)
					.Color(textColor)
			)
			.Background(background)
			.CornerRadius(16)
			.Frame(height: height)
			.Padding(new Thickness(16, 12));
	}
}
