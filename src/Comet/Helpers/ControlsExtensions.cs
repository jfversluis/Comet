using System;
using Comet.Internal;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
namespace Comet
{
	public static class ControlsExtensions
	{
		// TextField / SecureField extensions
		public static T PlaceholderColor<T>(this T view, Color color) where T : View
		{
			view.SetEnvironment(EnvironmentKeys.Entry.PlaceholderColor, color, false);
			return view;
		}

		// Slider extensions
		public static Slider MinimumTrackColor(this Slider view, Color color)
		{
			view.SetEnvironment(EnvironmentKeys.Slider.ProgressColor, color, false);
			return view;
		}

		public static Slider MaximumTrackColor(this Slider view, Color color)
		{
			view.SetEnvironment(EnvironmentKeys.Slider.TrackColor, color, false);
			return view;
		}

		public static Slider ThumbColor(this Slider view, Color color)
		{
			view.SetEnvironment(EnvironmentKeys.Slider.ThumbColor, color, false);
			return view;
		}

		// Toggle/Switch extensions
		public static Toggle OnColor(this Toggle view, Color color)
		{
			view.SetEnvironment(EnvironmentKeys.Switch.OnColor, color, false);
			return view;
		}

		public static Toggle ThumbColor(this Toggle view, Color color)
		{
			view.SetEnvironment(EnvironmentKeys.Switch.ThumbColor, color, false);
			return view;
		}

		// Image extensions
		public static Image Aspect(this Image view, Microsoft.Maui.Aspect aspect)
		{
			view.SetEnvironment(EnvironmentKeys.Image.Aspect, aspect, false);
			return view;
		}
	}
}
