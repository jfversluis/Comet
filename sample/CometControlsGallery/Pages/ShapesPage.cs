using System;
using Comet;
using Comet.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometControlsGallery.Pages
{
public class ShapesPageState
{
}

public class ShapesPage : Component<ShapesPageState>
{
public override View Render() =>
GalleryPageHelpers.Scaffold("Shapes",
GalleryPageHelpers.Section("Filled primitives", "Comet shape primitives render through ShapeView with real fill and stroke treatments.",
HStack(12,
ShapeCard("Rectangle", "Fill + stroke", new Rectangle()
.Fill(Color.FromArgb("#8FD3FE"))
.Stroke(Color.FromArgb("#145DA0"), 3), 160, 84),
ShapeCard("Ellipse", "Soft oval", new Ellipse()
.Fill(Color.FromArgb("#F7C9FF"))
.Stroke(Color.FromArgb("#7C3AED"), 3), 160, 84)
),
HStack(12,
ShapeCard("Circle", "Centered fill", new Circle()
.Fill(Color.FromArgb("#C9F7D9"))
.Stroke(Color.FromArgb("#1B8A5A"), 3), 120, 120),
RoundedRectangleShowcase()
)
),
GalleryPageHelpers.Section("Lines and dash patterns", "Straight segments, dashed strokes, and point-based shapes map closely to the MAUI sample.",
HStack(12,
ShapeCard("Line", "Round caps", WithLineCap(new Line(8, 44, 150, 44)
.Stroke(Color.FromArgb("#005A9C"), 6), LineCap.Round), 160, 88),
ShapeCard("Dashed line", "StrokeDashPattern", WithLineCap(WithDash(new Line(8, 44, 150, 44)
.Stroke(Color.FromArgb("#D97706"), 5), 14f, 8f), LineCap.Round), 160, 88)
),
HStack(12,
ShapeCard("Polyline", "Open path", WithLineJoin(new Polyline(
new PointF(8, 64),
new PointF(42, 18),
new PointF(84, 54),
new PointF(126, 14),
new PointF(152, 46))
.Stroke(Color.FromArgb("#2563EB"), 4), LineJoin.Round), 160, 88),
ShapeCard("Polygon", "Closed fill", WithLineJoin(new Polygon(
new PointF(80, 10),
new PointF(150, 40),
new PointF(122, 78),
new PointF(38, 78),
new PointF(10, 40))
.Fill(Color.FromArgb("#FFE2B8"))
.Stroke(Color.FromArgb("#C2410C"), 3), LineJoin.Round), 160, 88)
)
),
GalleryPageHelpers.Section("Custom paths", "Bezier-style paths and SVG-like point data make it easy to build richer illustrations.",
HStack(12,
ShapeCard("Wave path", "Bezier curve", new Path(BuildWavePath())
.Stroke(Color.FromArgb("#0F766E"), 4), 160, 96),
ShapeCard("Dash pattern", "Alternating stroke", WithDash(new Path(BuildDiamondPath())
.Fill(Color.FromArgb("#E9D5FF"))
.Stroke(Color.FromArgb("#7E22CE"), 3), 10f, 5f), 160, 96)
),
GalleryPageHelpers.Caption("RoundedRectangle currently uses a uniform corner radius in Comet, so the gallery shows low and high radius variants instead of per-corner radii.")
)
);

static View RoundedRectangleShowcase() =>
Border(
VStack(8,
GalleryPageHelpers.Caption("RoundedRectangle"),
new ShapeView(new RoundedRectangle(12)
.Fill(Color.FromArgb("#DDE7FF"))
.Stroke(Color.FromArgb("#335CFF"), 3))
.Frame(width: 160, height: 40),
new ShapeView(new RoundedRectangle(28)
.Fill(Color.FromArgb("#FFE0E6"))
.Stroke(Color.FromArgb("#BE123C"), 3))
.Frame(width: 160, height: 40)
)
)
.Background(ColorTokens.SurfaceContainerLow.Resolve(ThemeManager.Current()))
.CornerRadius(20)
.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
.StrokeThickness(1)
.Padding(new Thickness(16))
.Frame(width: 196);

		static View ShapeCard(string title, string caption, Shape shape, double width, double height) =>
			Border(
				VStack(10,
					GalleryPageHelpers.Caption(title),
					new ShapeView(shape)
						.Frame(width: (float)width, height: (float)height),
					GalleryPageHelpers.BodyText(caption)
				)
			)
			.Background(ColorTokens.SurfaceContainerLow.Resolve(ThemeManager.Current()))
			.CornerRadius(20)
			.StrokeColor(ColorTokens.Outline.Resolve(ThemeManager.Current()))
			.StrokeThickness(1)
			.Padding(new Thickness(16))
			.Frame(width: (float)(width + 36));

static PathF BuildWavePath()
{
var path = new PathF();
path.MoveTo(8, 74);
path.QuadTo(40, 12, 78, 44);
path.QuadTo(116, 78, 152, 18);
return path;
}

static PathF BuildDiamondPath()
{
var path = new PathF();
path.MoveTo(80, 10);
path.LineTo(148, 48);
path.LineTo(80, 86);
path.LineTo(12, 48);
path.Close();
return path;
}

		static TShape WithDash<TShape>(TShape shape, params float[] pattern) where TShape : Shape
		{
			shape.SetEnvironment("StrokeDashPattern", pattern, false);
			return shape;
		}

		static TShape WithLineCap<TShape>(TShape shape, LineCap lineCap) where TShape : Shape
		{
			shape.SetEnvironment("StrokeLineCap", lineCap, false);
			return shape;
		}

		static TShape WithLineJoin<TShape>(TShape shape, LineJoin lineJoin) where TShape : Shape
		{
			shape.SetEnvironment("StrokeLineJoin", lineJoin, false);
			return shape;
		}
	}
}
