using System;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
public class TransformsPageState
{
public double Rotation { get; set; }
public double ScaleX { get; set; } = 1;
public double ScaleY { get; set; } = 1;
public double TranslationX { get; set; }
public double TranslationY { get; set; }
public double Opacity { get; set; } = 1;
public double AnchorX { get; set; } = 0.5;
public double AnchorY { get; set; } = 0.5;
}

	public class TransformsPage : Component<TransformsPageState>
	{
		Border? animatedTarget;

public override View Render() =>
GalleryPageHelpers.Scaffold("Transforms",
GalleryPageHelpers.Section("Static transforms", "Sliders update rotation, scale, translation, and opacity directly on the Comet view.",
BuildStaticTarget(),
SliderRow("Rotation", State.Rotation, -180, 180, value => SetState(s => s.Rotation = value)),
SliderRow("Scale X", State.ScaleX, 0.5, 1.8, value => SetState(s => s.ScaleX = value)),
SliderRow("Scale Y", State.ScaleY, 0.5, 1.8, value => SetState(s => s.ScaleY = value)),
SliderRow("Translation X", State.TranslationX, -100, 100, value => SetState(s => s.TranslationX = value)),
SliderRow("Translation Y", State.TranslationY, -60, 60, value => SetState(s => s.TranslationY = value)),
SliderRow("Opacity", State.Opacity, 0.2, 1.0, value => SetState(s => s.Opacity = value))
),
GalleryPageHelpers.Section("Animated transforms", "Comet exposes animation helpers for fade, rotation, scaling, translation, and composite movement.",
VStack(10,
BuildAnimationButton("Rotate 360°", () => animatedTarget?.RotateTo(360, 0.85, Easing.CubicInOut)),
BuildAnimationButton("Scale pulse", () => animatedTarget?.Animate(v =>
{
v.ScaleX(1.25);
v.ScaleY(1.25);
}, duration: 0.35, autoReverses: true)),
BuildAnimationButton("Translate", () => animatedTarget?.TranslateTo(96, -24, 0.55, Easing.CubicInOut)),
BuildAnimationButton("Fade", () => animatedTarget?.FadeTo(0.25, 0.45, Easing.CubicInOut)),
BuildAnimationButton("Composite", () => animatedTarget?.Animate(v =>
{
v.Rotation(180);
v.ScaleX(1.2);
v.ScaleY(1.2);
v.TranslationX(90);
v.Opacity(0.55);
}, duration: 0.8, autoReverses: true)),
BuildAnimationButton("Reset all", () => ResetState())
),
GalleryPageHelpers.Caption("Animation helpers in this sample target the live Comet view directly, while the sliders continue to demonstrate the underlying transform properties.")
),
GalleryPageHelpers.Section("Anchor demo", "AnchorX and AnchorY shift the rotation origin so you can see how the box pivots around different edges.",
BuildAnchorTarget(),
SliderRow("Anchor X", State.AnchorX, 0.0, 1.0, value => SetState(s => s.AnchorX = value)),
SliderRow("Anchor Y", State.AnchorY, 0.0, 1.0, value => SetState(s => s.AnchorY = value)),
GalleryPageHelpers.BodyText($"Current anchor: ({State.AnchorX:F2}, {State.AnchorY:F2})")
)
);

View BuildStaticTarget()
{
animatedTarget = Border(
Text("Transform me")
.FontWeight(FontWeight.Bold)
.Color(Colors.White)
.HorizontalTextAlignment(TextAlignment.Center)
)
.Background(Color.FromArgb("#6750A4"))
.CornerRadius(22)
.Padding(new Thickness(18))
.Frame(width: 200, height: 110)
.Rotation(State.Rotation)
.ScaleX(State.ScaleX)
.ScaleY(State.ScaleY)
.TranslationX(State.TranslationX)
.TranslationY(State.TranslationY)
.Opacity(State.Opacity);

return Border(animatedTarget)
.Background(ColorTokens.SurfaceContainerLow.Resolve(ThemeManager.Current()))
.CornerRadius(24)
.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
.StrokeThickness(1)
.Padding(new Thickness(20))
.Frame(height: 220);
}

View BuildAnchorTarget()
{
var anchorTarget = Border(
Text("Anchor origin")
.FontWeight(FontWeight.Bold)
.Color(Colors.White)
.HorizontalTextAlignment(TextAlignment.Center)
)
.Background(Color.FromArgb("#0F766E"))
.CornerRadius(20)
.Padding(new Thickness(18))
.Frame(width: 170, height: 88)
.AnchorX(State.AnchorX)
.AnchorY(State.AnchorY)
.Rotation(28);

return Border(anchorTarget)
.Background(ColorTokens.SurfaceContainerLow.Resolve(ThemeManager.Current()))
.CornerRadius(24)
.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
.StrokeThickness(1)
.Padding(new Thickness(20))
.Frame(height: 190);
}

View SliderRow(string label, double value, double minimum, double maximum, Action<double> onChanged) =>
VStack(4,
GalleryPageHelpers.Caption($"{label}: {value:F2}"),
Slider(() => value, () => minimum, () => maximum)
.OnValueChanged(onChanged)
);

View BuildAnimationButton(string label, Action action) =>
Button(label, action)
.ButtonStyle(ButtonStyles.Outlined);

void ResetState() => SetState(s =>
{
s.Rotation = 0;
s.ScaleX = 1;
s.ScaleY = 1;
s.TranslationX = 0;
s.TranslationY = 0;
s.Opacity = 1;
s.AnchorX = 0.5;
s.AnchorY = 0.5;
});
}
}
