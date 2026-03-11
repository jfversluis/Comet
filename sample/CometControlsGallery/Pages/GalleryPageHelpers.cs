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
		// Reference-style accent color for section headers
		static readonly Color SectionHeaderColor = Color.FromArgb("#2980B9");
		static readonly Color PageBackground = Color.FromArgb("#F0F0F5");
		static readonly Color SeparatorColor = new Color(128, 128, 128, 0.25f);

		public static View Scaffold(string title, params View[] sections) =>
			ScrollView(
				VStack(10, sections)
					.Padding(new Thickness(24))
			)
			.Background(PageBackground)
			.Title(title);

		public static View Section(string title, string description, params View[] content)
		{
			var views = new List<View>
			{
				SectionHeader(title),
				Text(description)
					.Typography(TypographyTokens.BodyMedium)
					.Color(ColorTokens.OnSurfaceVariant)
					.LineBreakMode(LineBreakMode.WordWrap)
			};

			views.AddRange(content);
			views.Add(Separator());

			return VStack(10, views.ToArray());
		}

		public static View SectionHeader(string title) =>
			Text(title)
				.Typography(TypographyTokens.TitleLarge)
				.Color(SectionHeaderColor);

		public static View Separator() =>
			Border((View)null)
				.Background(SeparatorColor)
				.Frame(height: 1);

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
