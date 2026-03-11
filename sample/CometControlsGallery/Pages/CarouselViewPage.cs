using System;
using System.Collections.Generic;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	record SlideItem(string Title, string Description, Color Color, string Icon);

	public class CarouselViewPageState
	{
		public int Position { get; set; }
	}

	public class CarouselViewPage : Component<CarouselViewPageState>
	{
		static readonly List<SlideItem> Slides = new()
		{
			new("Welcome", "Swipe left and right to navigate between slides", Colors.DodgerBlue, "W"),
			new("Features", "CarouselView supports paging, templates, and position tracking", Colors.MediumSeaGreen, "F"),
			new("Templates", "Each slide uses a DataTemplate for custom content", Colors.MediumOrchid, "T"),
			new("Navigation", "Use the Previous/Next buttons or swipe to move", Colors.Coral, "N"),
			new("Complete", "You've reached the last slide!", Colors.SlateBlue, "C"),
		};

		public override View Render()
		{
			return GalleryPageHelpers.Scaffold("CarouselView",
				// Carousel
				new CarouselView<SlideItem>(() => Slides)
				{
					ViewFor = slide =>
						Border(
							VStack(12,
								Text(slide.Icon)
									.FontSize(48)
									.HorizontalTextAlignment(TextAlignment.Center)
									.Color(Colors.White),
								Text(slide.Title)
									.FontSize(24)
									.FontWeight(FontWeight.Bold)
									.Color(Colors.White)
									.HorizontalTextAlignment(TextAlignment.Center),
								Text(slide.Description)
									.FontSize(14)
									.Color(Colors.White)
									.HorizontalTextAlignment(TextAlignment.Center)
									.Padding(new Thickness(20, 0))
							)
							.Padding(new Thickness(24))
						)
						.Background(slide.Color)
						.CornerRadius(12)
						.StrokeThickness(0),
					Position = (Func<int>)(() => State.Position),
					PositionChanged = pos => SetState(s => s.Position = pos),
				}
				.Frame(height: 300),

				// Dots indicator
				BuildDots(),

				// Position label
				Text($"Slide {State.Position + 1} of {Slides.Count}")
					.FontSize(14)
					.Color(Colors.Gray)
					.HorizontalTextAlignment(TextAlignment.Center),

				// Navigation buttons
				HStack(12,
					Button("Previous", () =>
					{
						if (State.Position > 0)
							SetState(s => s.Position--);
					})
					.FontSize(13),
					Button("Next", () =>
					{
						if (State.Position < Slides.Count - 1)
							SetState(s => s.Position++);
					})
					.FontSize(13)
				)
			);
		}

		View BuildDots()
		{
			var dots = new List<View>();
			for (int i = 0; i < Slides.Count; i++)
			{
				var isActive = i == State.Position;
				dots.Add(
					new ShapeView(new Circle())
						.Frame(width: 10, height: 10)
						.Background(new SolidPaint(isActive ? Colors.DodgerBlue : Colors.Gray))
				);
			}
			return HStack(8, dots.ToArray());
		}
	}
}
