using System;
using Comet.Styles;

// ReSharper disable once CheckNamespace
namespace Comet
{
	/// <summary>
	/// Extension methods for applying per-control-type styles via the environment.
	/// </summary>
	public static class ControlStyleExtensions
	{
		/// <summary>
		/// Sets the ButtonStyle for this view and its subtree.
		/// </summary>
		public static T ButtonStyle<T>(
			this T view,
			IControlStyle<Button, ButtonConfiguration> style) where T : View
		{
			view.SetEnvironment(StyleToken<Button>.Key, style, cascades: true);
			return view;
		}

		/// <summary>
		/// Sets the ToggleStyle for this view and its subtree.
		/// </summary>
		public static T ToggleStyle<T>(
			this T view,
			IControlStyle<Toggle, ToggleConfiguration> style) where T : View
		{
			view.SetEnvironment(StyleToken<Toggle>.Key, style, cascades: true);
			return view;
		}

		/// <summary>
		/// Sets the TextFieldStyle for this view and its subtree.
		/// </summary>
		public static T TextFieldStyle<T>(
			this T view,
			IControlStyle<TextField, TextFieldConfiguration> style) where T : View
		{
			view.SetEnvironment(StyleToken<TextField>.Key, style, cascades: true);
			return view;
		}

		/// <summary>
		/// Sets the SliderStyle for this view and its subtree.
		/// </summary>
		public static T SliderStyle<T>(
			this T view,
			IControlStyle<Slider, SliderConfiguration> style) where T : View
		{
			view.SetEnvironment(StyleToken<Slider>.Key, style, cascades: true);
			return view;
		}

		/// <summary>
		/// Resolves the current button style from environment or theme defaults.
		/// Call from Button's handler mapping or property changed logic.
		/// </summary>
		public static ViewModifier ResolveCurrentStyle(this Button button, ButtonConfiguration config)
		{
			var style = button.GetEnvironment<IControlStyle<Button, ButtonConfiguration>>(
				StyleToken<Button>.Key);

			if (style == null)
				return ViewModifier.Empty;

			return style.Resolve(config);
		}
	}
}
