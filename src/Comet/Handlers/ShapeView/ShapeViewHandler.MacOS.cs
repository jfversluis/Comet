using System;
using AppKit;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace Comet.Handlers
{
	public partial class ShapeViewHandler : ViewHandler<ShapeView, NSView>
	{
		protected override NSView CreatePlatformView()
		{
			var view = new NSView();
			view.WantsLayer = true;
			return view;
		}

		public static void MapShapeProperty(IElementHandler viewHandler, ShapeView virtualView)
		{
			// TODO: Implement AppKit shape drawing via CAShapeLayer or Microsoft.Maui.Graphics
		}
	}
}
