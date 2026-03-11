using System;
using Comet;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
	public class GesturesPageState
	{
		public int TapCount { get; set; }
		public bool TapEven { get; set; }
		public double PanTotalX { get; set; }
		public double PanTotalY { get; set; }
		public double PanTranslationX { get; set; }
		public double PanTranslationY { get; set; }
		public string SwipeResult { get; set; } = "Swipe result: (none)";
		public double PinchScale { get; set; } = 1;
		public double CurrentPinchScale { get; set; } = 1;
		public string PinchLabel { get; set; } = "Scale: 1.00";
		public string PointerLabel { get; set; } = "Hover over the box";
		public bool PointerHovered { get; set; }
	}

	public class GesturesPage : Component<GesturesPageState>
	{
		public override View Render()
		{
			var tapLabel = GalleryPageHelpers.BodyText($"Tap the box! Taps: {State.TapCount}");

			var tapBox = Border(new Spacer())
				.Background(State.TapEven ? Colors.DodgerBlue : Colors.Coral)
				.CornerRadius(8)
				.StrokeThickness(0)
				.Frame(width: 120, height: 80)
				.OnTap(_ => SetState(s =>
				{
					s.TapCount++;
					s.TapEven = s.TapCount % 2 == 0;
				}));

			var panBox = Border(new Spacer())
				.Background(Colors.MediumSeaGreen)
				.CornerRadius(8)
				.StrokeThickness(0)
				.Frame(width: 80, height: 80)
				.TranslationX(State.PanTranslationX)
				.TranslationY(State.PanTranslationY);

			var panContainer = Border(panBox)
				.Background(Color.FromArgb("#F0F0F0"))
				.StrokeThickness(0)
				.Frame(width: 400, height: 120)
				.OnPan(gesture => SetState(s =>
				{
					if (gesture.Status == Comet.GestureStatus.Running)
					{
						s.PanTranslationX = s.PanTotalX + gesture.TotalX;
						s.PanTranslationY = s.PanTotalY + gesture.TotalY;
					}
					else if (gesture.Status == Comet.GestureStatus.Completed)
					{
						s.PanTotalX = s.PanTranslationX;
						s.PanTotalY = s.PanTranslationY;
					}
				}));

			var swipeInstruction = GalleryPageHelpers.BodyText("Click and drag quickly, then release");

			var swipeResultLabel = Text(State.SwipeResult)
				.FontSize(16)
				.FontWeight(FontWeight.Bold)
				.Color(Colors.DodgerBlue);

			var swipeBox = Border(new Spacer())
				.Background(Color.FromArgb("#E8F0FE"))
				.CornerRadius(8)
				.StrokeThickness(0)
				.Frame(width: 300, height: 80)
				.AddGesture(new SwipeGesture(_ => SetState(s => s.SwipeResult = "Swiped: Left")) { Direction = Comet.SwipeDirection.Left })
				.AddGesture(new SwipeGesture(_ => SetState(s => s.SwipeResult = "Swiped: Right")) { Direction = Comet.SwipeDirection.Right })
				.AddGesture(new SwipeGesture(_ => SetState(s => s.SwipeResult = "Swiped: Up")) { Direction = Comet.SwipeDirection.Up })
				.AddGesture(new SwipeGesture(_ => SetState(s => s.SwipeResult = "Swiped: Down")) { Direction = Comet.SwipeDirection.Down });

			var pinchLabel = GalleryPageHelpers.BodyText(State.PinchLabel);

			var pinchBox = Border(new Spacer())
				.Background(Colors.MediumPurple)
				.CornerRadius(8)
				.StrokeThickness(0)
				.Frame(width: 100, height: 100)
				.Scale(State.PinchScale)
				.OnPinch(gesture => SetState(s =>
				{
					if (gesture.Status == Comet.GestureStatus.Running)
					{
						s.PinchScale = s.CurrentPinchScale * gesture.Scale;
						s.PinchLabel = $"Scale: {s.PinchScale:F2}";
					}
					else if (gesture.Status == Comet.GestureStatus.Completed)
					{
						s.CurrentPinchScale = s.PinchScale;
					}
				}));

			var pointerLabel = GalleryPageHelpers.BodyText(State.PointerLabel);

			var pointerBox = Border(new Spacer())
				.Background(State.PointerHovered ? Colors.Orange : Colors.SteelBlue)
				.CornerRadius(8)
				.StrokeThickness(0)
				.Frame(width: 150, height: 80)
				.AddGesture(new PointerGesture
				{
					PointerEntered = (_, _) => SetState(s =>
					{
						s.PointerLabel = "Pointer: Entered";
						s.PointerHovered = true;
					}),
					PointerExited = (_, _) => SetState(s =>
					{
						s.PointerLabel = "Pointer: Exited";
						s.PointerHovered = false;
					}),
					PointerMoved = (_, point) => SetState(s =>
					{
						s.PointerLabel = $"Pointer: Moved ({point.X:F0}, {point.Y:F0})";
					})
				});

			return GalleryPageHelpers.Scaffold("Gestures",
				GalleryPageHelpers.Section("TapGestureRecognizer",
					tapLabel,
					tapBox
				),
				GalleryPageHelpers.Section("PanGestureRecognizer",
					GalleryPageHelpers.BodyText("Click and drag in the gray area:"),
					panContainer
				),
				GalleryPageHelpers.Section("SwipeGestureRecognizer",
					swipeInstruction,
					swipeBox,
					swipeResultLabel
				),
				GalleryPageHelpers.Section("PinchGestureRecognizer",
					pinchLabel,
					pinchBox
				),
				GalleryPageHelpers.Section("PointerGestureRecognizer",
					pointerLabel,
					pointerBox
				)
			);
		}
	}
}
