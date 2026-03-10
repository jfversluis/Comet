using System;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
public class GesturesPageState
{
public int TapCount { get; set; }
public int LongPressCount { get; set; }
public string SwipeDirection { get; set; } = "Waiting for a swipe";
public string PointerState { get; set; } = "Hover over the card on desktop or Mac Catalyst.";
public double PointerX { get; set; }
public double PointerY { get; set; }
public double PanX { get; set; }
public double PanY { get; set; }
public string PanStatus { get; set; } = "Drag the card to see pan updates.";
public double PinchScale { get; set; } = 1;
public string PinchStatus { get; set; } = "Pinch on touch hardware or trackpad to zoom.";
public int AccentIndex { get; set; }
}

public class GesturesPage : Component<GesturesPageState>
{
public override View Render() =>
GalleryPageHelpers.Scaffold("Gestures",
GalleryPageHelpers.Section("Tap, long press, swipe, and pointer", "The main interaction surface tracks quick taps, long presses, directional swipes, and desktop hover movement.",
BuildInteractionSurface(),
HStack(12,
MetricChip($"Taps: {State.TapCount}"),
MetricChip($"Long presses: {State.LongPressCount}")
),
GalleryPageHelpers.BodyText($"Last swipe: {State.SwipeDirection}"),
GalleryPageHelpers.Caption(State.PointerState),
GalleryPageHelpers.Caption($"Pointer position: {State.PointerX:F0}, {State.PointerY:F0}")
),
GalleryPageHelpers.Section("Pan", "Drag the demo tile to update its translation in real time.",
BuildPanSurface(),
GalleryPageHelpers.BodyText($"Offset: {State.PanX:F0}, {State.PanY:F0}"),
GalleryPageHelpers.Caption(State.PanStatus)
),
GalleryPageHelpers.Section("Pinch", "Pinch gestures adjust scale and report status so you can build zoomable surfaces.",
BuildPinchSurface(),
GalleryPageHelpers.BodyText($"Current scale: {State.PinchScale:F2}x"),
GalleryPageHelpers.Caption(State.PinchStatus)
)
);

View BuildInteractionSurface()
{
var surface = Border(
VStack(8,
Text("Interactive gesture surface")
.Typography(TypographyTokens.TitleLarge)
.Color(Colors.White),
GalleryPageHelpers.BodyText("Tap to cycle colors, long press for a secondary action, swipe in any direction, or hover for pointer data.")
.Color(Colors.White)
)
)
.Background(GetAccentColor())
.CornerRadius(24)
.Padding(new Thickness(20))
.Frame(height: 170)
.OnTap(_ => SetState(s =>
{
s.TapCount++;
s.AccentIndex = (s.AccentIndex + 1) % AccentPalette.Length;
}))
.OnLongPress(_ => SetState(s => s.LongPressCount++));

			surface
				.AddGesture(new SwipeGesture(_ => SetSwipe("Left")) { Direction = Comet.SwipeDirection.Left })
				.AddGesture(new SwipeGesture(_ => SetSwipe("Right")) { Direction = Comet.SwipeDirection.Right })
				.AddGesture(new SwipeGesture(_ => SetSwipe("Up")) { Direction = Comet.SwipeDirection.Up })
				.AddGesture(new SwipeGesture(_ => SetSwipe("Down")) { Direction = Comet.SwipeDirection.Down })
				.AddGesture(new PointerGesture
				{
					PointerEntered = (_, point) => SetPointer("Pointer entered the surface.", point),
PointerMoved = (_, point) => SetPointer("Pointer moving across the surface.", point),
PointerExited = (_, point) => SetPointer("Pointer exited the surface.", point)
});

return surface;
}

View BuildPanSurface() =>
Border(
Border(
Text("Drag me")
.FontWeight(FontWeight.Bold)
.Color(Colors.White)
.HorizontalTextAlignment(TextAlignment.Center)
)
.Background(Color.FromArgb("#2563EB"))
.CornerRadius(18)
.Padding(new Thickness(16))
.Frame(width: 140, height: 88)
.TranslationX(State.PanX)
.TranslationY(State.PanY)
)
.Background(ColorTokens.SurfaceContainerLow.Resolve(ThemeManager.Current()))
.CornerRadius(24)
.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
.StrokeThickness(1)
.Padding(new Thickness(20))
.Frame(height: 210)
.OnPan(gesture => SetState(s =>
{
s.PanX = Clamp(gesture.TotalX, -90, 90);
s.PanY = Clamp(gesture.TotalY, -50, 50);
s.PanStatus = $"{gesture.Status}: {s.PanX:F0}, {s.PanY:F0}";
}));

View BuildPinchSurface() =>
Border(
Border(
Text("Pinch to zoom")
.FontWeight(FontWeight.Bold)
.Color(Colors.White)
.HorizontalTextAlignment(TextAlignment.Center)
)
.Background(Color.FromArgb("#7C3AED"))
.CornerRadius(20)
.Padding(new Thickness(18))
.Frame(width: 180, height: 100)
.Scale(State.PinchScale)
)
.Background(ColorTokens.SurfaceContainerLow.Resolve(ThemeManager.Current()))
.CornerRadius(24)
.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
.StrokeThickness(1)
.Padding(new Thickness(20))
.Frame(height: 210)
.OnPinch(gesture => SetState(s =>
{
s.PinchScale = Clamp(gesture.Scale, 0.75, 2.4);
s.PinchStatus = $"{gesture.Status}: scale {s.PinchScale:F2}x at ({gesture.OriginX:F2}, {gesture.OriginY:F2})";
}));

View MetricChip(string text) =>
Border(
Text(text)
.FontWeight(FontWeight.Bold)
.Color(ColorTokens.OnSecondaryContainer)
)
.Background(ColorTokens.SecondaryContainer.Resolve(ThemeManager.Current()))
.CornerRadius(16)
.Padding(new Thickness(12, 8));

void SetSwipe(string direction) => SetState(s => s.SwipeDirection = direction);

void SetPointer(string message, Point point) => SetState(s =>
{
s.PointerState = message;
s.PointerX = point.X;
s.PointerY = point.Y;
});

Color GetAccentColor() => AccentPalette[Math.Abs(State.AccentIndex % AccentPalette.Length)];

static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));

static readonly Color[] AccentPalette =
{
Color.FromArgb("#2563EB"),
Color.FromArgb("#0F766E"),
Color.FromArgb("#7C3AED"),
Color.FromArgb("#D97706")
};
}
}
