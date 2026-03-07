using System;
using System.Collections.Generic;
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

		// CollectionView fluent extensions
		public static CollectionView<T> Header<T>(this CollectionView<T> view, View header)
		{
			view.Header = header;
			return view;
		}

		public static CollectionView<T> Footer<T>(this CollectionView<T> view, View footer)
		{
			view.Footer = footer;
			return view;
		}

		public static CollectionView<T> EmptyView<T>(this CollectionView<T> view, View emptyView)
		{
			view.EmptyView = emptyView;
			return view;
		}

		public static CollectionView<T> ItemTemplate<T>(this CollectionView<T> view, Func<T, View> template)
		{
			view.ViewFor = template;
			return view;
		}

		public static CollectionView<T> OnRemainingItemsThresholdReached<T>(this CollectionView<T> view, int threshold, Action action)
		{
			view.RemainingItemsThreshold = threshold;
			view.RemainingItemsThresholdReached = action;
			return view;
		}
	}
}
