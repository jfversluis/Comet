# Comet Style & Theme System — Technical Specification

> **Status:** Proposal (greenfield)
> **Author:** Holden (Lead Architect)
> **Date:** 2026-03-09
> **Requested by:** David Ortinau

---

## Table of Contents

1. [Design Principles](#1-design-principles)
2. [Architecture Overview](#2-architecture-overview)
3. [Pillar 1: Reusable Styles (ViewModifier)](#3-pillar-1-reusable-styles-viewmodifier)
4. [Pillar 2: Per-Control Type Styling](#4-pillar-2-per-control-type-styling)
5. [Pillar 3: Theme Definition](#5-pillar-3-theme-definition)
6. [Pillar 4: Theme Switching](#6-pillar-4-theme-switching)
7. [Pillar 5: Theme Propagation](#7-pillar-5-theme-propagation)
8. [Type-Safe Environment Keys](#8-type-safe-environment-keys)
9. [Control State Model](#9-control-state-model)
10. [Composition Model](#10-composition-model)
11. [Performance Model](#11-performance-model)
12. [Integration Points](#12-integration-points)
13. [End-to-End Code Examples](#13-end-to-end-code-examples)
14. [SwiftUI Comparison](#14-swiftui-comparison)
15. [Open Questions](#15-open-questions)

---

## 1. Design Principles

Three values govern every API decision. When two values conflict, the one listed first wins.

### 1.1 Performance

- **Zero allocation in the read path.** Reading a style token from the environment must not allocate. Strongly-typed keys use generics and static fields — no boxing, no string concatenation.
- **Lazy evaluation.** A theme is a bag of token definitions. Values are only materialized when a view requests them. No upfront "push 27 colors into a dictionary."
- **Surgical invalidation.** When a theme changes, only views that actually read the changed tokens re-render. No full tree walks.

### 1.2 Simplicity

- **One way to do each thing.** There is one `ViewModifier` type for reusable styles. One `ControlStyleProtocol<T>` for per-control theming. One `Theme` for the design system. No legacy `Style` / `ControlStyle<T>` / `Style<T>` overlap.
- **Discoverable API.** A developer who types `.` on a view should find styling through IntelliSense, not through documentation.
- **No string keys in public API.** Every token is a strongly-typed static field.

### 1.3 Composition

- **Styles compose like views.** A `ViewModifier` is a function `View → View`. Composing two modifiers is function composition. No special merge rules.
- **Themes compose from token sets.** A theme is a `ColorTokens` + `TypographyTokens` + `SpacingTokens` + per-control defaults. Each is independently replaceable.
- **Scoped overrides just work.** Applying a modifier or a sub-theme to a container scopes it to that container's subtree. No opt-in protocol needed.

---

## 2. Architecture Overview

```
┌──────────────────────────────────────────────────────┐
│                    Developer API                      │
│                                                      │
│  ViewModifier    ControlStyle<T>    Theme             │
│  (reusable)      (per-type)         (design system)   │
└──────────┬──────────┬────────────────┬───────────────┘
           │          │                │
           ▼          ▼                ▼
┌──────────────────────────────────────────────────────┐
│              Token<T> (type-safe keys)                │
│                                                      │
│  ColorTokens   TypographyTokens   SpacingTokens      │
│  ShapeTokens   ControlTokens<T>                      │
└──────────────────────┬───────────────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────────────┐
│           EnvironmentData (reactive store)            │
│                                                      │
│  Token<T>.Key → object   (cascading dictionary)       │
│  View.Parent chain       (inherited lookup)           │
│  StateManager            (change notifications)       │
└──────────────────────────────────────────────────────┘
```

**Key insight:** The environment system stays. It already handles cascading, parent-chain lookup, and reactive invalidation. What changes is the **addressing layer** above it: strongly-typed `Token<T>` replaces string keys, and the three overlapping style abstractions collapse into one unified pattern per concern.

---

## 3. Pillar 1: Reusable Styles (ViewModifier)

### 3.1 The Problem

Today there are three ways to define a reusable style:

1. `Style<T>` — `Action<T>` that mutates a view
2. `ControlStyle<T>` — string-keyed dictionary pushed into environment
3. Legacy `Style` — monolithic class with per-control properties

This spec replaces all three with **`ViewModifier`**: a composable function from view to view.

### 3.2 API Design

```csharp
/// A reusable, composable set of view modifications.
/// Analogous to SwiftUI's ViewModifier protocol.
public abstract class ViewModifier
{
	/// Apply this modifier's properties to the given view.
	/// Implementations call fluent methods on the view.
	public abstract View Apply(View view);
}

/// Typed variant for control-specific modifiers.
public abstract class ViewModifier<T> : ViewModifier where T : View
{
	public sealed override View Apply(View view)
	{
		if (view is T typed)
			return Apply(typed);
		return view;
	}

	/// Apply this modifier's properties to a view of type T.
	public abstract T Apply(T view);
}
```

### 3.3 Defining a Modifier

```csharp
public class CardStyle : ViewModifier
{
	public override View Apply(View view) => view
		.Background(Theme.Token(ColorTokens.SurfaceContainer))
		.ClipShape(new RoundedRectangle(12))
		.Padding(16)
		.Shadow(new Shadow(2, 2, 4, Colors.Black.WithAlpha(0.15f)));
}

public class HeaderTextStyle : ViewModifier<Text>
{
	public override Text Apply(Text view) => view
		.FontSize(Theme.Token(TypographyTokens.TitleLarge.Size))
		.FontWeight(FontWeight.Bold)
		.Color(Theme.Token(ColorTokens.OnSurface));
}
```

### 3.4 Applying a Modifier

```csharp
// Single modifier
Text("Welcome")
	.Modifier(new HeaderTextStyle())

// Inline — same as calling fluent methods directly
VStack {
	Text("Card Title"),
	Text("Card body text"),
}.Modifier(new CardStyle())
```

### 3.5 Extension Method

```csharp
public static class ViewModifierExtensions
{
	/// Applies a ViewModifier to any view.
	public static T Modifier<T>(this T view, ViewModifier modifier) where T : View
	{
		modifier.Apply(view);
		return view;
	}

	/// Applies multiple modifiers in order (left to right).
	public static T Modifier<T>(this T view, params ViewModifier[] modifiers) where T : View
	{
		foreach (var m in modifiers)
			m.Apply(view);
		return view;
	}
}
```

### 3.6 Static Modifier Instances (Performance)

Modifiers that don't carry per-instance state should be singletons:

```csharp
public static class AppStyles
{
	public static readonly ViewModifier Card = new CardStyle();
	public static readonly ViewModifier<Text> Header = new HeaderTextStyle();
	public static readonly ViewModifier<Text> Caption = new CaptionTextStyle();
}

// Usage — no allocation at call site
Text("Title").Modifier(AppStyles.Header)
```

### 3.7 Composition

Modifiers compose via `Then()`:

```csharp
public static class ViewModifierComposition
{
	/// Creates a new modifier that applies `first`, then `second`.
	public static ComposedModifier Then(this ViewModifier first, ViewModifier second)
		=> new ComposedModifier(first, second);
}

public sealed class ComposedModifier : ViewModifier
{
	readonly ViewModifier _first;
	readonly ViewModifier _second;

	public ComposedModifier(ViewModifier first, ViewModifier second)
	{
		_first = first;
		_second = second;
	}

	public override View Apply(View view)
	{
		_first.Apply(view);
		_second.Apply(view);
		return view;
	}
}

// Usage
public static readonly ViewModifier DangerCard =
	AppStyles.Card.Then(new DangerBackground());
```

---

## 4. Pillar 2: Per-Control Type Styling

### 4.1 The Problem

Today, making "all buttons look like X" requires `ControlStyle<T>` with string-keyed environment writes. There's no way to provide a custom rendering configuration that knows about control state (pressed, hovered, disabled) in a type-safe way. SwiftUI solves this with style protocols (`ButtonStyle`, `ToggleStyle`), and this spec adapts that pattern for Comet's MVU architecture.

### 4.2 API Design

```csharp
/// Base interface for all control style protocols.
/// Each control type defines a concrete TConfiguration struct
/// that carries the control's interactive state.
public interface IControlStyle<TControl, TConfiguration>
	where TControl : View
	where TConfiguration : struct
{
	/// Given the control's current state, return the appearance properties.
	/// This is called during the view's render cycle.
	ViewModifier Resolve(TConfiguration configuration);
}
```

### 4.3 Built-In Configuration Structs

Each styleable control defines a configuration struct carrying its interactive state:

```csharp
/// Configuration provided to ButtonStyle implementations.
public readonly struct ButtonConfiguration
{
	public bool IsPressed { get; init; }
	public bool IsHovered { get; init; }
	public bool IsEnabled { get; init; }
	public bool IsFocused { get; init; }
	public string Label { get; init; }
}

/// Configuration provided to ToggleStyle implementations.
public readonly struct ToggleConfiguration
{
	public bool IsOn { get; init; }
	public bool IsEnabled { get; init; }
	public bool IsFocused { get; init; }
}

/// Configuration provided to TextFieldStyle implementations.
public readonly struct TextFieldConfiguration
{
	public bool IsEditing { get; init; }
	public bool IsEnabled { get; init; }
	public bool IsFocused { get; init; }
	public string Placeholder { get; init; }
}

/// Configuration provided to SliderStyle implementations.
public readonly struct SliderConfiguration
{
	public double Value { get; init; }
	public double Minimum { get; init; }
	public double Maximum { get; init; }
	public bool IsEnabled { get; init; }
	public bool IsDragging { get; init; }
}
```

### 4.4 Defining a Control Style

```csharp
public class FilledButtonStyle : IControlStyle<Button, ButtonConfiguration>
{
	public ViewModifier Resolve(ButtonConfiguration config)
	{
		var bg = config.IsPressed
			? ColorTokens.PrimaryContainer
			: ColorTokens.Primary;
		var fg = config.IsPressed
			? ColorTokens.OnPrimaryContainer
			: ColorTokens.OnPrimary;
		var opacity = config.IsEnabled ? 1.0 : 0.38;

		return new ButtonAppearance(bg, fg, opacity);
	}
}

// The modifier returned by Resolve:
sealed class ButtonAppearance : ViewModifier<Button>
{
	readonly Token<Color> _bg;
	readonly Token<Color> _fg;
	readonly double _opacity;

	public ButtonAppearance(Token<Color> bg, Token<Color> fg, double opacity)
	{
		_bg = bg;
		_fg = fg;
		_opacity = opacity;
	}

	public override Button Apply(Button view) => view
		.Background(Theme.Token(_bg))
		.Color(Theme.Token(_fg))
		.Opacity(_opacity)
		.ClipShape(new RoundedRectangle(20))
		.Padding(new Thickness(24, 12));
}
```

### 4.5 Applying a Control Style

Control styles propagate through the environment, so they can be set on any container:

```csharp
// Set for the entire app (in theme definition)
var theme = new AppTheme()
	.SetControlStyle(new FilledButtonStyle())
	.SetControlStyle(new RoundedTextFieldStyle());

// Scoped to a subtree
VStack {
	Button("Primary", OnClick),
	Button("Also Primary", OnClick),
}.ButtonStyle(new OutlinedButtonStyle())

// Scoped to a single view (explicit override)
Button("Danger", OnDelete)
	.ButtonStyle(new DangerButtonStyle())
```

### 4.6 Scoped Control Style Extensions

Generated for each styleable control:

```csharp
public static class ButtonStyleExtensions
{
	/// Sets the ButtonStyle for this view and its subtree.
	public static T ButtonStyle<T>(
		this T view,
		IControlStyle<Button, ButtonConfiguration> style) where T : View
	{
		view.SetEnvironment(StyleToken<Button>.Key, style, cascades: true);
		return view;
	}
}

public static class ToggleStyleExtensions
{
	public static T ToggleStyle<T>(
		this T view,
		IControlStyle<Toggle, ToggleConfiguration> style) where T : View
	{
		view.SetEnvironment(StyleToken<Toggle>.Key, style, cascades: true);
		return view;
	}
}
```

### 4.7 Built-In Styles

Each styleable control ships with a `Default` style and common variants:

```csharp
public static class ButtonStyles
{
	public static readonly IControlStyle<Button, ButtonConfiguration> Filled
		= new FilledButtonStyle();
	public static readonly IControlStyle<Button, ButtonConfiguration> Outlined
		= new OutlinedButtonStyle();
	public static readonly IControlStyle<Button, ButtonConfiguration> Text
		= new TextButtonStyle();
	public static readonly IControlStyle<Button, ButtonConfiguration> Elevated
		= new ElevatedButtonStyle();
}
```

### 4.8 How Controls Read Their Style

Inside the generated Button class (or hand-written equivalent), the handler queries the environment for the registered style and resolves it:

```csharp
// Inside Button's handler mapping or ViewPropertyChanged:
internal ViewModifier ResolveCurrentStyle()
{
	var style = this.GetEnvironment<IControlStyle<Button, ButtonConfiguration>>(
		StyleToken<Button>.Key);

	if (style == null)
		return ViewModifier.Empty;

	var config = new ButtonConfiguration
	{
		IsPressed = _isPressed,
		IsHovered = _isHovered,
		IsEnabled = this.GetEnvironment<bool?>(nameof(IView.IsEnabled)) ?? true,
		IsFocused = _isFocused,
		Label = _label?.CurrentValue,
	};

	return style.Resolve(config);
}
```

---

## 5. Pillar 3: Theme Definition

### 5.1 The Problem

Today `Theme` is a grab-bag: legacy color properties, a `ThemeColors` object with 27 MD3 tokens, and a `Dictionary<Type, object>` for `ControlStyle<T>`. There's no coherent structure, and token access goes through string keys.

### 5.2 Design: Theme = Token Sets + Control Defaults

A theme is a named, self-contained collection of **design tokens** organized into typed token sets, plus **per-control style defaults**.

```csharp
public class Theme
{
	/// Identifies this theme (e.g., "Light", "Dark", "BrandOcean").
	public string Name { get; init; }

	/// Color tokens — primary, secondary, surface, error, etc.
	public ColorTokenSet Colors { get; init; }

	/// Typography tokens — display, headline, title, body, label sizes/weights.
	public TypographyTokenSet Typography { get; init; }

	/// Spacing tokens — compact, standard, comfortable.
	public SpacingTokenSet Spacing { get; init; }

	/// Shape tokens — corner radii for small, medium, large containers.
	public ShapeTokenSet Shapes { get; init; }

	/// Per-control type style defaults.
	readonly Dictionary<Type, object> _controlStyles = new();

	/// Sets the default style for a control type within this theme.
	public Theme SetControlStyle<TControl, TConfig>(
		IControlStyle<TControl, TConfig> style)
		where TControl : View
		where TConfig : struct
	{
		_controlStyles[typeof(TControl)] = style;
		return this;
	}

	/// Gets the control style, or null if not set.
	public IControlStyle<TControl, TConfig> GetControlStyle<TControl, TConfig>()
		where TControl : View
		where TConfig : struct
	{
		return _controlStyles.TryGetValue(typeof(TControl), out var s)
			? (IControlStyle<TControl, TConfig>)s
			: null;
	}
}
```

### 5.3 Token Sets

Each token set is a record (or sealed class) with strongly-typed properties:

```csharp
public record ColorTokenSet
{
	// Primary
	public Color Primary { get; init; }
	public Color OnPrimary { get; init; }
	public Color PrimaryContainer { get; init; }
	public Color OnPrimaryContainer { get; init; }

	// Secondary
	public Color Secondary { get; init; }
	public Color OnSecondary { get; init; }
	public Color SecondaryContainer { get; init; }
	public Color OnSecondaryContainer { get; init; }

	// Tertiary
	public Color Tertiary { get; init; }
	public Color OnTertiary { get; init; }
	public Color TertiaryContainer { get; init; }
	public Color OnTertiaryContainer { get; init; }

	// Error
	public Color Error { get; init; }
	public Color OnError { get; init; }
	public Color ErrorContainer { get; init; }
	public Color OnErrorContainer { get; init; }

	// Surface
	public Color Surface { get; init; }
	public Color OnSurface { get; init; }
	public Color SurfaceVariant { get; init; }
	public Color OnSurfaceVariant { get; init; }
	public Color SurfaceContainer { get; init; }
	public Color SurfaceContainerLow { get; init; }
	public Color SurfaceContainerHigh { get; init; }

	// Background
	public Color Background { get; init; }
	public Color OnBackground { get; init; }

	// Outline
	public Color Outline { get; init; }
	public Color OutlineVariant { get; init; }

	// Inverse
	public Color InverseSurface { get; init; }
	public Color InverseOnSurface { get; init; }
	public Color InversePrimary { get; init; }
}

public record TypographyTokenSet
{
	// Display
	public FontSpec DisplayLarge { get; init; }
	public FontSpec DisplayMedium { get; init; }
	public FontSpec DisplaySmall { get; init; }

	// Headline
	public FontSpec HeadlineLarge { get; init; }
	public FontSpec HeadlineMedium { get; init; }
	public FontSpec HeadlineSmall { get; init; }

	// Title
	public FontSpec TitleLarge { get; init; }
	public FontSpec TitleMedium { get; init; }
	public FontSpec TitleSmall { get; init; }

	// Body
	public FontSpec BodyLarge { get; init; }
	public FontSpec BodyMedium { get; init; }
	public FontSpec BodySmall { get; init; }

	// Label
	public FontSpec LabelLarge { get; init; }
	public FontSpec LabelMedium { get; init; }
	public FontSpec LabelSmall { get; init; }
}

/// Immutable font specification.
public readonly record struct FontSpec(
	double Size,
	FontWeight Weight,
	string Family = null,
	double LineHeight = 0,
	double LetterSpacing = 0
);

public record SpacingTokenSet
{
	public double None { get; init; }        // 0
	public double ExtraSmall { get; init; }  // 4
	public double Small { get; init; }       // 8
	public double Medium { get; init; }      // 16
	public double Large { get; init; }       // 24
	public double ExtraLarge { get; init; }  // 32
}

public record ShapeTokenSet
{
	public double None { get; init; }         // 0
	public double ExtraSmall { get; init; }   // 4
	public double Small { get; init; }        // 8
	public double Medium { get; init; }       // 12
	public double Large { get; init; }        // 16
	public double ExtraLarge { get; init; }   // 28
	public double Full { get; init; }         // 9999 (pill)
}
```

### 5.4 Defining a Theme

```csharp
public static class AppThemes
{
	public static readonly Theme Light = new Theme
	{
		Name = "Light",
		Colors = new ColorTokenSet
		{
			Primary = Color.FromArgb("#512BD4"),
			OnPrimary = Colors.White,
			PrimaryContainer = Color.FromArgb("#EADDFF"),
			OnPrimaryContainer = Color.FromArgb("#21005D"),
			Secondary = Color.FromArgb("#625B71"),
			OnSecondary = Colors.White,
			Surface = Color.FromArgb("#FFFBFE"),
			OnSurface = Color.FromArgb("#1C1B1F"),
			SurfaceVariant = Color.FromArgb("#E7E0EC"),
			OnSurfaceVariant = Color.FromArgb("#49454F"),
			SurfaceContainer = Color.FromArgb("#F3EDF7"),
			Background = Color.FromArgb("#FFFBFE"),
			OnBackground = Color.FromArgb("#1C1B1F"),
			Error = Color.FromArgb("#B3261E"),
			OnError = Colors.White,
			Outline = Color.FromArgb("#79747E"),
			OutlineVariant = Color.FromArgb("#CAC4D0"),
			// ... remaining tokens
		},
		Typography = TypographyDefaults.Material3,
		Spacing = SpacingDefaults.Standard,
		Shapes = ShapeDefaults.Rounded,
	};

	public static readonly Theme Dark = Light with
	{
		Name = "Dark",
		Colors = new ColorTokenSet
		{
			Primary = Color.FromArgb("#D0BCFF"),
			OnPrimary = Color.FromArgb("#381E72"),
			Surface = Color.FromArgb("#1C1B1F"),
			OnSurface = Color.FromArgb("#E6E1E5"),
			Background = Color.FromArgb("#1C1B1F"),
			OnBackground = Color.FromArgb("#E6E1E5"),
			// ... remaining tokens
		},
	};
}
```

### 5.5 Brand Theme with Custom Colors

```csharp
public static class OceanTheme
{
	public static readonly Theme Light = AppThemes.Light with
	{
		Name = "OceanLight",
		Colors = AppThemes.Light.Colors with
		{
			Primary = Color.FromArgb("#006C7A"),
			OnPrimary = Colors.White,
			PrimaryContainer = Color.FromArgb("#A2E4F0"),
			OnPrimaryContainer = Color.FromArgb("#001F25"),
			Secondary = Color.FromArgb("#4A6267"),
			OnSecondary = Colors.White,
		},
	};
}
```

---

## 6. Pillar 4: Theme Switching

### 6.1 The Problem

Today, `Theme.Current = newTheme` triggers `Apply()` which walks all active views and pushes every value into a global string-keyed dictionary. This is O(tokens × views) on every switch.

### 6.2 Design: Reactive Theme via Environment

The active theme is stored as a single object in the environment. Views that read tokens go through an indirection layer that resolves from the active theme. When the theme reference changes, only views that actually read tokens are invalidated.

```csharp
public static class ThemeManager
{
	/// The environment token that holds the active theme reference.
	static readonly Token<Theme> ActiveThemeToken = new("Comet.Theme");

	/// Gets the current theme. Reads from the nearest environment scope.
	public static Theme Current(View view)
		=> view.GetEnvironment<Theme>(ActiveThemeToken) ?? Defaults.Light;

	/// Gets the current theme from the global environment.
	public static Theme Current()
		=> View.GetGlobalEnvironment<Theme>(ActiveThemeToken) ?? Defaults.Light;

	/// Sets the active theme globally. Reactive — views that read tokens update.
	public static void SetTheme(Theme theme)
	{
		View.SetGlobalEnvironment(ActiveThemeToken, theme);
	}

	/// Sets a scoped theme override on a subtree.
	public static T Theme<T>(this T view, Theme theme) where T : View
	{
		view.SetEnvironment(ActiveThemeToken, theme, cascades: true);
		return view;
	}
}
```

### 6.3 Reactive Token Resolution

When a view reads `Theme.Token(ColorTokens.Primary)`, it doesn't get a static color. It gets a `Binding<Color>` that resolves lazily from the active theme:

```csharp
public static class Theme
{
	/// Returns a binding that lazily resolves a color from the active theme.
	/// When the theme changes, the binding re-evaluates and the view updates.
	public static Binding<Color> Token(Token<Color> token)
		=> new Binding<Color>(() =>
		{
			var theme = ThemeManager.Current();
			return token.Resolve(theme);
		});

	/// Resolve a token value from a specific theme directly (non-reactive).
	public static Color Resolve(Token<Color> token, Theme theme)
		=> token.Resolve(theme);

	/// Resolve from the nearest scoped theme for a specific view.
	public static Binding<Color> Token(View view, Token<Color> token)
		=> new Binding<Color>(() =>
		{
			var theme = ThemeManager.Current(view);
			return token.Resolve(theme);
		});
}
```

### 6.4 Theme Switch in Practice

```csharp
public class SettingsPage : View
{
	readonly State<bool> _isDark = false;

	[Body]
	View Body() => VStack {
		Text("Settings")
			.Modifier(AppStyles.Header),

		Toggle("Dark Mode", _isDark)
			.OnChanged(isDark =>
			{
				ThemeManager.SetTheme(
					isDark ? AppThemes.Dark : AppThemes.Light);
			}),
	};
}
```

### 6.5 System Theme Follow

```csharp
// In app startup
public class MyApp : CometApp
{
	[Body]
	View Body()
	{
		// Subscribe to platform theme changes
		Application.Current.RequestedThemeChanged += (s, e) =>
		{
			ThemeManager.SetTheme(e.RequestedTheme == AppTheme.Dark
				? AppThemes.Dark
				: AppThemes.Light);
		};

		return new MainPage();
	}
}
```

---

## 7. Pillar 5: Theme Propagation

### 7.1 The Problem

Today, theme values are pushed globally via `SetGlobalEnvironment()` which writes every token to a flat dictionary and notifies every active view. There's no scoped override (e.g., "this card uses the dark theme even though the app is light").

### 7.2 Design: Cascading Theme Reference

Instead of pushing N individual token values into the environment, push **one** `Theme` reference. The environment's parent-chain lookup naturally handles scoping.

**Lookup chain for a token read:**

```
View reads ColorTokens.Primary
  → Binding evaluates: ThemeManager.Current(this)
    → this.GetEnvironment<Theme>(ActiveThemeToken)
      → LocalContext? → Context? → Parent.Context? → ... → Global Environment
    → Finds nearest Theme object
    → Calls token.Resolve(theme) → returns theme.Colors.Primary
```

### 7.3 Scoped Theme Override

```csharp
VStack {
	// This card renders in dark mode even when the app is light
	VStack {
		Text("Dark Card Title")
			.Color(Theme.Token(ColorTokens.OnSurface)),
		Text("Content on a dark surface")
			.Color(Theme.Token(ColorTokens.OnSurfaceVariant)),
	}
	.Background(Theme.Token(ColorTokens.Surface))
	.Theme(AppThemes.Dark),  // <-- scoped override

	// Everything below here uses the app's active theme
	Text("This follows the global theme")
		.Color(Theme.Token(ColorTokens.OnBackground)),
}
```

### 7.4 How It Works Internally

When `.Theme(AppThemes.Dark)` is called on a view:

1. The `Theme` object is stored in that view's **cascading** environment context under `ActiveThemeToken`.
2. Any child that reads a theme token does `ThemeManager.Current(this)`, which walks up the parent chain.
3. The scoped `Theme` is found before the global one. Token resolution uses that theme's values.
4. When the global theme switches, the scoped subtree is unaffected because its local override takes priority.

### 7.5 Token Override (Without Full Theme)

Sometimes you want to override just one token, not swap the entire theme:

```csharp
/// Override a single color token for a subtree.
public static T OverrideToken<T>(
	this T view,
	Token<Color> token,
	Color value) where T : View
{
	view.SetEnvironment(token.Key, value, cascades: true);
	return view;
}

// Usage: make all "Primary" accesses in this subtree use red
VStack {
	Button("Danger", OnDelete),
	Button("Also Danger", OnCancel),
}.OverrideToken(ColorTokens.Primary, Colors.Red)
```

---

## 8. Type-Safe Environment Keys

### 8.1 The Problem

Today, all environment keys are `string` constants. `EnvironmentKeys.Colors.Color` is `"Color"`, `EnvironmentKeys.Fonts.Size` is `"Font.Size"`. Typos compile but fail silently. Wrong-type reads return `default`.

### 8.2 Design: `Token<T>` Static Keys

```csharp
/// A strongly-typed environment key. The generic parameter T
/// is the type of value stored at this key.
public sealed class Token<T>
{
	/// The underlying string key used in EnvironmentData storage.
	/// Internal — not exposed to user code.
	internal string Key { get; }

	/// Human-readable name for debugging/IntelliSense.
	public string Name { get; }

	/// Optional default value when no theme/environment provides one.
	public T DefaultValue { get; }

	public Token(string key, string name = null, T defaultValue = default)
	{
		Key = key;
		Name = name ?? key;
		DefaultValue = defaultValue;
	}
}
```

### 8.3 Token Definitions

Tokens are defined as static fields on token set companion classes:

```csharp
/// Strongly-typed color token identifiers.
public static class ColorTokens
{
	public static readonly Token<Color> Primary
		= new("theme.color.primary", "Primary");
	public static readonly Token<Color> OnPrimary
		= new("theme.color.onPrimary", "On Primary");
	public static readonly Token<Color> PrimaryContainer
		= new("theme.color.primaryContainer", "Primary Container");
	public static readonly Token<Color> OnPrimaryContainer
		= new("theme.color.onPrimaryContainer", "On Primary Container");

	public static readonly Token<Color> Secondary
		= new("theme.color.secondary", "Secondary");
	public static readonly Token<Color> OnSecondary
		= new("theme.color.onSecondary", "On Secondary");
	public static readonly Token<Color> SecondaryContainer
		= new("theme.color.secondaryContainer", "Secondary Container");
	public static readonly Token<Color> OnSecondaryContainer
		= new("theme.color.onSecondaryContainer", "On Secondary Container");

	public static readonly Token<Color> Surface
		= new("theme.color.surface", "Surface");
	public static readonly Token<Color> OnSurface
		= new("theme.color.onSurface", "On Surface");
	public static readonly Token<Color> SurfaceVariant
		= new("theme.color.surfaceVariant", "Surface Variant");
	public static readonly Token<Color> OnSurfaceVariant
		= new("theme.color.onSurfaceVariant", "On Surface Variant");
	public static readonly Token<Color> SurfaceContainer
		= new("theme.color.surfaceContainer", "Surface Container");

	public static readonly Token<Color> Background
		= new("theme.color.background", "Background");
	public static readonly Token<Color> OnBackground
		= new("theme.color.onBackground", "On Background");

	public static readonly Token<Color> Error
		= new("theme.color.error", "Error");
	public static readonly Token<Color> OnError
		= new("theme.color.onError", "On Error");
	public static readonly Token<Color> ErrorContainer
		= new("theme.color.errorContainer", "Error Container");
	public static readonly Token<Color> OnErrorContainer
		= new("theme.color.onErrorContainer", "On Error Container");

	public static readonly Token<Color> Outline
		= new("theme.color.outline", "Outline");
	public static readonly Token<Color> OutlineVariant
		= new("theme.color.outlineVariant", "Outline Variant");

	public static readonly Token<Color> InverseSurface
		= new("theme.color.inverseSurface", "Inverse Surface");
	public static readonly Token<Color> InverseOnSurface
		= new("theme.color.inverseOnSurface", "Inverse On Surface");
	public static readonly Token<Color> InversePrimary
		= new("theme.color.inversePrimary", "Inverse Primary");
}

/// Strongly-typed typography token identifiers.
public static class TypographyTokens
{
	public static readonly Token<FontSpec> DisplayLarge
		= new("theme.type.displayLarge", "Display Large");
	public static readonly Token<FontSpec> DisplayMedium
		= new("theme.type.displayMedium", "Display Medium");
	public static readonly Token<FontSpec> DisplaySmall
		= new("theme.type.displaySmall", "Display Small");

	public static readonly Token<FontSpec> HeadlineLarge
		= new("theme.type.headlineLarge", "Headline Large");
	public static readonly Token<FontSpec> HeadlineMedium
		= new("theme.type.headlineMedium", "Headline Medium");
	public static readonly Token<FontSpec> HeadlineSmall
		= new("theme.type.headlineSmall", "Headline Small");

	public static readonly Token<FontSpec> TitleLarge
		= new("theme.type.titleLarge", "Title Large");
	public static readonly Token<FontSpec> TitleMedium
		= new("theme.type.titleMedium", "Title Medium");
	public static readonly Token<FontSpec> TitleSmall
		= new("theme.type.titleSmall", "Title Small");

	public static readonly Token<FontSpec> BodyLarge
		= new("theme.type.bodyLarge", "Body Large");
	public static readonly Token<FontSpec> BodyMedium
		= new("theme.type.bodyMedium", "Body Medium");
	public static readonly Token<FontSpec> BodySmall
		= new("theme.type.bodySmall", "Body Small");

	public static readonly Token<FontSpec> LabelLarge
		= new("theme.type.labelLarge", "Label Large");
	public static readonly Token<FontSpec> LabelMedium
		= new("theme.type.labelMedium", "Label Medium");
	public static readonly Token<FontSpec> LabelSmall
		= new("theme.type.labelSmall", "Label Small");
}

public static class SpacingTokens
{
	public static readonly Token<double> None
		= new("theme.spacing.none", "None", 0);
	public static readonly Token<double> ExtraSmall
		= new("theme.spacing.xs", "Extra Small", 4);
	public static readonly Token<double> Small
		= new("theme.spacing.sm", "Small", 8);
	public static readonly Token<double> Medium
		= new("theme.spacing.md", "Medium", 16);
	public static readonly Token<double> Large
		= new("theme.spacing.lg", "Large", 24);
	public static readonly Token<double> ExtraLarge
		= new("theme.spacing.xl", "Extra Large", 32);
}

public static class ShapeTokens
{
	public static readonly Token<double> None
		= new("theme.shape.none", "None", 0);
	public static readonly Token<double> ExtraSmall
		= new("theme.shape.xs", "Extra Small", 4);
	public static readonly Token<double> Small
		= new("theme.shape.sm", "Small", 8);
	public static readonly Token<double> Medium
		= new("theme.shape.md", "Medium", 12);
	public static readonly Token<double> Large
		= new("theme.shape.lg", "Large", 16);
	public static readonly Token<double> ExtraLarge
		= new("theme.shape.xl", "Extra Large", 28);
	public static readonly Token<double> Full
		= new("theme.shape.full", "Full (Pill)", 9999);
}
```

### 8.4 Token Resolution

Each token knows how to extract its value from a `Theme`:

```csharp
public sealed class Token<T>
{
	// ... existing members ...

	/// Resolver function set during token registration.
	internal Func<Theme, T> Resolver { get; init; }

	/// Extract this token's value from the given theme.
	public T Resolve(Theme theme)
	{
		if (Resolver != null)
			return Resolver(theme);
		return DefaultValue;
	}
}

// Token definitions wire up resolvers:
public static class ColorTokens
{
	public static readonly Token<Color> Primary = new("theme.color.primary", "Primary")
	{
		Resolver = theme => theme.Colors.Primary
	};

	public static readonly Token<Color> OnPrimary = new("theme.color.onPrimary", "On Primary")
	{
		Resolver = theme => theme.Colors.OnPrimary
	};

	// ... etc for all tokens
}
```

### 8.5 Reading a Token from a View

```csharp
/// Extension method on View for reading typed tokens.
public static T GetToken<T>(this View view, Token<T> token)
{
	// First check: is there a direct token override in the environment?
	var directOverride = view.GetEnvironment<T>(token.Key);
	if (directOverride != null)
		return directOverride;

	// Second check: resolve from the active theme
	var theme = ThemeManager.Current(view);
	return token.Resolve(theme);
}
```

### 8.6 Compile-Time Safety

```csharp
// ✅ Compiles — Token<Color> goes into a Color parameter
Text("Hello").Color(Theme.Token(ColorTokens.Primary))

// ❌ Does not compile — Token<FontSpec> is not Color
Text("Hello").Color(Theme.Token(TypographyTokens.BodyLarge))

// ✅ Compiles — Token<double> goes into a double parameter
VStack { ... }.Padding(Theme.Token(SpacingTokens.Medium))

// ❌ Does not compile — Token<Color> is not double
VStack { ... }.Padding(Theme.Token(ColorTokens.Primary))
```

---

## 9. Control State Model

### 9.1 Current State Enum

The existing `ControlState` enum is adequate:

```csharp
public enum ControlState
{
	Default,
	Disabled,
	Pressed,
	Hovered,
	Focused,
}
```

Add `Focused` (not present today) — needed for text fields, buttons with keyboard navigation.

### 9.2 State Tracking

Control state is tracked inside the control's handler (platform side) and pushed to the Comet view:

```csharp
// Inside ButtonHandler (platform-specific):
void OnPointerEntered() => VirtualView.UpdateControlState(ControlState.Hovered, true);
void OnPointerExited() => VirtualView.UpdateControlState(ControlState.Hovered, false);
void OnPressed() => VirtualView.UpdateControlState(ControlState.Pressed, true);
void OnReleased() => VirtualView.UpdateControlState(ControlState.Pressed, false);

// On the View base class:
public sealed class ControlStateSet
{
	ControlState _active = ControlState.Default;

	/// The current set of active states (can be multiple: Hovered + Focused).
	public ControlState Active => _active;

	public bool IsPressed => (_active & ControlState.Pressed) != 0;
	public bool IsHovered => (_active & ControlState.Hovered) != 0;
	public bool IsFocused => (_active & ControlState.Focused) != 0;
	public bool IsDisabled => (_active & ControlState.Disabled) != 0;
}
```

### 9.3 State Change → Style Re-evaluation

When control state changes, the control re-evaluates its style:

```csharp
// On View:
internal void UpdateControlState(ControlState state, bool active)
{
	var old = _controlStates.Active;
	_controlStates.Set(state, active);
	if (old != _controlStates.Active)
		OnControlStateChanged();
}

void OnControlStateChanged()
{
	// The control's style protocol is re-resolved with the new configuration.
	// This triggers the handler to update the native view.
	ViewPropertyChanged(StyleToken<View>.Key, null);
}
```

### 9.4 Using Control State in Styles

```csharp
public class FilledButtonStyle : IControlStyle<Button, ButtonConfiguration>
{
	public ViewModifier Resolve(ButtonConfiguration config)
	{
		// State-aware color selection
		Color bg, fg;
		if (!config.IsEnabled)
		{
			bg = Colors.Gray.WithAlpha(0.12f);
			fg = Colors.Gray.WithAlpha(0.38f);
		}
		else if (config.IsPressed)
		{
			bg = Theme.Resolve(ColorTokens.Primary).WithAlpha(0.88f);
			fg = Theme.Resolve(ColorTokens.OnPrimary);
		}
		else if (config.IsHovered)
		{
			bg = Theme.Resolve(ColorTokens.Primary).WithAlpha(0.92f);
			fg = Theme.Resolve(ColorTokens.OnPrimary);
		}
		else
		{
			bg = Theme.Resolve(ColorTokens.Primary);
			fg = Theme.Resolve(ColorTokens.OnPrimary);
		}

		return new InlineModifier(v => v
			.Background(new SolidPaint(bg))
			.Color(fg)
			.Opacity(config.IsEnabled ? 1.0 : 0.38)
			.ClipShape(new RoundedRectangle(20))
			.Padding(new Thickness(24, 12)));
	}
}
```

---

## 10. Composition Model

### 10.1 Modifier Composition

Modifiers compose left-to-right. Last writer wins for the same property:

```csharp
var baseCard = new CardStyle();              // Background=Surface, CornerRadius=12
var danger = new DangerModifier();           // Background=ErrorContainer
var combined = baseCard.Then(danger);        // Background=ErrorContainer, CornerRadius=12
```

### 10.2 Theme Composition (Record `with`)

Themes compose via C# record `with` syntax:

```csharp
// Start from a base, override just what you need
var brand = AppThemes.Light with
{
	Colors = AppThemes.Light.Colors with
	{
		Primary = Color.FromArgb("#FF6200"),
	},
};
```

### 10.3 Style Precedence (Highest to Lowest)

1. **Explicit fluent method** on the view: `.Background(Colors.Red)` — non-cascading, local context
2. **ViewModifier** applied to the view: `.Modifier(new CardStyle())` — writes to local context
3. **Per-control style protocol** (scoped): `.ButtonStyle(new OutlinedButtonStyle())` — cascading context
4. **Per-control style protocol** (theme-level): theme default button style — global environment
5. **Theme token** default: `theme.Colors.Primary` — resolved via `Theme.Token()`
6. **Token.DefaultValue**: fallback when no theme provides the token

### 10.4 No Implicit Styles

The current `Style<T>.RegisterImplicit()` mechanism (global side-effect registration) is removed. Per-control defaults go through the theme's control style dictionary. This is explicit, discoverable, and debuggable.

---

## 11. Performance Model

### 11.1 Theme Switch Cost

**Current system:** O(tokens × views) — pushes every token into global environment, notifies every view.

**New system:** O(1) for the switch + O(V) for invalidation where V = views that read theme tokens.

How:

1. `ThemeManager.SetTheme(newTheme)` writes ONE value to the global environment: the `Theme` reference under `ActiveThemeToken`.
2. `StateManager.OnPropertyChanged()` fires for `ActiveThemeToken.Key`.
3. Only views whose `Body()` function (or whose `Binding<T>` lambdas) read from `ThemeManager.Current()` — i.e., views that called `Theme.Token(...)` during their last render — are invalidated.
4. On re-render, `Theme.Token(ColorTokens.Primary)` now resolves to the new theme's primary color.

**No tree walk.** The reactive binding system already tracks which views depend on which environment keys. Changing the theme reference triggers rebuilds only for views that actually consumed it.

### 11.2 Token Read Cost

**Hot path:** A view reading a theme token during render.

```
Theme.Token(ColorTokens.Primary)
  → Binding<Color> lambda evaluates:
    → ThemeManager.Current()
      → View.GetGlobalEnvironment<Theme>(ActiveThemeToken)  // dictionary lookup
    → token.Resolve(theme)
      → theme.Colors.Primary  // property access, no allocation
  → returns Color value
```

**Cost:** One dictionary lookup + one property access. No string concatenation, no boxing (Color is a class in MAUI).

### 11.3 Style Resolution Cost

Per-control styles are resolved once per state change, not per frame:

```
Button state changes (Pressed → Default)
  → OnControlStateChanged()
    → ResolveCurrentStyle()
      → GetEnvironment<IControlStyle<Button, ButtonConfiguration>>(key)  // cached lookup
      → style.Resolve(new ButtonConfiguration { ... })  // struct, stack-allocated
      → returns ViewModifier
    → modifier.Apply(this)  // sets environment values
  → Handler updates native view
```

**Cost per state change:** One environment lookup + one `Resolve()` call + N `SetEnvironment()` calls where N is the number of properties the style sets (typically 3-5).

### 11.4 Modifier Application Cost

`ViewModifier.Apply()` calls fluent methods which call `SetEnvironment()`. Each `SetEnvironment()` is one dictionary write + one `StateManager` notification. This is the same cost as calling the fluent methods inline.

**Key optimization:** Modifier instances can be cached as `static readonly` fields. The modifier itself allocates nothing — it just calls existing methods on the view.

### 11.5 Memory Model

| Item | Allocation | Lifetime |
|------|-----------|----------|
| `Theme` | 1 object + 4 token set records | App lifetime (static) |
| `Token<T>` | 1 object per token definition | Static (never collected) |
| `ViewModifier` (cached) | 1 object | App lifetime |
| `ViewModifier` (inline) | 1 object per call | Short-lived |
| `ButtonConfiguration` | 0 (struct, stack) | Method scope |
| `Binding<Color>` from `Theme.Token()` | 1 closure | View lifetime |

### 11.6 Dirty Flagging

The reactive system already dirty-flags views that depend on changed environment keys. No additional dirty-flagging mechanism is needed. The `StateManager` tracks which `Binding<T>` lambdas read which environment keys during evaluation, and re-triggers only those bindings when a key changes.

---

## 12. Integration Points

### 12.1 Existing Environment System

**Unchanged:** `ContextualObject`, `EnvironmentData`, `View.SetEnvironment()`, `View.GetEnvironment()`, parent-chain lookup, `StateManager` tracking.

**Changed:** The **addressing layer** (what keys are used) moves from `EnvironmentKeys` string constants to `Token<T>` objects. The underlying storage remains `string → object` dictionaries in `EnvironmentData`. `Token<T>.Key` provides the string.

### 12.2 Generated Controls (Source Generator)

Generated controls currently read environment values like:

```csharp
Microsoft.Maui.Graphics.Color Microsoft.Maui.ITextStyle.TextColor
	=> this.GetEnvironment<Microsoft.Maui.Graphics.Color>("Color") ?? default;
```

**Migration path:** The generated code continues to use string keys internally (these are implementation details, not public API). The `Token<T>` system adds a type-safe public layer on top. Eventually, the source generator can be updated to emit `Token<T>` references directly.

### 12.3 Handler System

Handlers read view properties through MAUI interfaces (`IButton.TextColor`, `ITextStyle.Font`, etc.). The control style system writes values through the same environment keys that these interfaces read. No handler changes required.

**New integration:** Handlers push control state changes (pressed, hovered, focused) back to the Comet view via `UpdateControlState()`. This requires adding state-change callbacks to platform handlers.

### 12.4 Hot Reload

`ViewModifier.Apply()` calls fluent methods which write to the environment. On hot reload, the view's `Body()` re-evaluates, re-applies modifiers, and the diff system compares old vs new environment values. No special hot reload handling needed.

**Theme hot reload:** If a theme definition changes during development, `ThemeManager.SetTheme()` triggers the normal reactive update path. Views that read tokens rebuild automatically.

### 12.5 CometApp Startup

```csharp
public static MauiAppBuilder UseCometApp<TApp>(
	this MauiAppBuilder builder) where TApp : CometApp, new()
{
	// ... existing setup ...

	// Register default theme
	ThemeManager.SetTheme(AppThemes.Light);

	return builder;
}

// Custom theme at startup:
builder.UseCometApp<MyApp>();
ThemeManager.SetTheme(OceanTheme.Light);
```

---

## 13. End-to-End Code Examples

### 13.1 Defining a Custom Theme with Brand Colors

```csharp
// Themes/BrandThemes.cs
using Comet;
using Comet.Styles;
using Microsoft.Maui.Graphics;

public static class BrandThemes
{
	public static readonly Theme Light = new Theme
	{
		Name = "BrandLight",
		Colors = new ColorTokenSet
		{
			Primary = Color.FromArgb("#1B5E20"),       // Forest green
			OnPrimary = Colors.White,
			PrimaryContainer = Color.FromArgb("#A5D6A7"),
			OnPrimaryContainer = Color.FromArgb("#0A2F0D"),
			Secondary = Color.FromArgb("#558B2F"),
			OnSecondary = Colors.White,
			SecondaryContainer = Color.FromArgb("#C5E1A5"),
			OnSecondaryContainer = Color.FromArgb("#1B3409"),
			Surface = Color.FromArgb("#FAFDF6"),
			OnSurface = Color.FromArgb("#1A1C19"),
			SurfaceVariant = Color.FromArgb("#DEE5D9"),
			OnSurfaceVariant = Color.FromArgb("#424940"),
			SurfaceContainer = Color.FromArgb("#EEF2E9"),
			Background = Color.FromArgb("#FAFDF6"),
			OnBackground = Color.FromArgb("#1A1C19"),
			Error = Color.FromArgb("#BA1A1A"),
			OnError = Colors.White,
			ErrorContainer = Color.FromArgb("#FFDAD6"),
			OnErrorContainer = Color.FromArgb("#410002"),
			Outline = Color.FromArgb("#72796F"),
			OutlineVariant = Color.FromArgb("#C2C9BD"),
			InverseSurface = Color.FromArgb("#2F312D"),
			InverseOnSurface = Color.FromArgb("#F0F1EC"),
			InversePrimary = Color.FromArgb("#6DD573"),
			Tertiary = Color.FromArgb("#006874"),
			OnTertiary = Colors.White,
			TertiaryContainer = Color.FromArgb("#97F0FF"),
			OnTertiaryContainer = Color.FromArgb("#001F24"),
		},
		Typography = TypographyDefaults.Material3,
		Spacing = SpacingDefaults.Standard,
		Shapes = ShapeDefaults.Rounded,
	}
	.SetControlStyle(new BrandFilledButtonStyle())
	.SetControlStyle(new BrandTextFieldStyle());

	public static readonly Theme Dark = Light with
	{
		Name = "BrandDark",
		Colors = new ColorTokenSet
		{
			Primary = Color.FromArgb("#6DD573"),
			OnPrimary = Color.FromArgb("#0A3818"),
			PrimaryContainer = Color.FromArgb("#10491F"),
			OnPrimaryContainer = Color.FromArgb("#A5D6A7"),
			Secondary = Color.FromArgb("#AED581"),
			OnSecondary = Color.FromArgb("#1E3A0F"),
			Surface = Color.FromArgb("#1A1C19"),
			OnSurface = Color.FromArgb("#E2E3DE"),
			SurfaceVariant = Color.FromArgb("#424940"),
			OnSurfaceVariant = Color.FromArgb("#C2C9BD"),
			SurfaceContainer = Color.FromArgb("#252723"),
			Background = Color.FromArgb("#1A1C19"),
			OnBackground = Color.FromArgb("#E2E3DE"),
			Error = Color.FromArgb("#FFB4AB"),
			OnError = Color.FromArgb("#690005"),
			Outline = Color.FromArgb("#8C9388"),
			OutlineVariant = Color.FromArgb("#424940"),
			// ... remaining tokens
		},
	};
}
```

### 13.2 Creating Reusable Styles

```csharp
// Styles/AppStyles.cs
using Comet;
using Comet.Styles;

public static class AppStyles
{
	// Card container
	public static readonly ViewModifier Card = new CardModifier();
	public static readonly ViewModifier ElevatedCard = Card.Then(new ElevationModifier(4));

	// Text styles
	public static readonly ViewModifier<Text> PageTitle = new PageTitleModifier();
	public static readonly ViewModifier<Text> SectionHeader = new SectionHeaderModifier();
	public static readonly ViewModifier<Text> BodyText = new BodyTextModifier();
	public static readonly ViewModifier<Text> Caption = new CaptionModifier();

	// Danger variant
	public static readonly ViewModifier DangerCard =
		Card.Then(new TokenOverride(ColorTokens.Surface, Colors.Red.WithAlpha(0.08f)));
}

sealed class CardModifier : ViewModifier
{
	public override View Apply(View view) => view
		.Background(Theme.Token(ColorTokens.SurfaceContainer))
		.ClipShape(new RoundedRectangle(Theme.Resolve(ShapeTokens.Medium)))
		.Padding(Theme.Token(SpacingTokens.Medium))
		.Shadow(new Shadow(0, 1, 3, Colors.Black.WithAlpha(0.12f)));
}

sealed class PageTitleModifier : ViewModifier<Text>
{
	public override Text Apply(Text view) => view
		.FontSize(Theme.Token(TypographyTokens.HeadlineMedium).Map(f => f.Size))
		.FontWeight(FontWeight.Bold)
		.Color(Theme.Token(ColorTokens.OnBackground));
}

sealed class SectionHeaderModifier : ViewModifier<Text>
{
	public override Text Apply(Text view) => view
		.FontSize(Theme.Token(TypographyTokens.TitleMedium).Map(f => f.Size))
		.FontWeight(FontWeight.SemiBold)
		.Color(Theme.Token(ColorTokens.OnSurface));
}

sealed class BodyTextModifier : ViewModifier<Text>
{
	public override Text Apply(Text view) => view
		.FontSize(Theme.Token(TypographyTokens.BodyLarge).Map(f => f.Size))
		.Color(Theme.Token(ColorTokens.OnSurface));
}

sealed class CaptionModifier : ViewModifier<Text>
{
	public override Text Apply(Text view) => view
		.FontSize(Theme.Token(TypographyTokens.LabelSmall).Map(f => f.Size))
		.Color(Theme.Token(ColorTokens.OnSurfaceVariant));
}

sealed class ElevationModifier : ViewModifier
{
	readonly float _elevation;
	public ElevationModifier(float elevation) => _elevation = elevation;

	public override View Apply(View view) => view
		.Shadow(new Shadow(0, _elevation / 2, _elevation, Colors.Black.WithAlpha(0.2f)));
}
```

### 13.3 Per-Control Custom Button Style

```csharp
// Styles/BrandFilledButtonStyle.cs
using Comet;
using Comet.Styles;

public class BrandFilledButtonStyle : IControlStyle<Button, ButtonConfiguration>
{
	public ViewModifier Resolve(ButtonConfiguration config)
	{
		if (!config.IsEnabled)
			return DisabledAppearance.Instance;

		if (config.IsPressed)
			return PressedAppearance.Instance;

		if (config.IsHovered)
			return HoveredAppearance.Instance;

		return DefaultAppearance.Instance;
	}

	// Cache modifier instances — no allocation per state change
	sealed class DefaultAppearance : ViewModifier<Button>
	{
		public static readonly DefaultAppearance Instance = new();
		public override Button Apply(Button view) => view
			.Background(Theme.Token(ColorTokens.Primary))
			.Color(Theme.Token(ColorTokens.OnPrimary))
			.ClipShape(new RoundedRectangle(20))
			.Padding(new Thickness(24, 12));
	}

	sealed class PressedAppearance : ViewModifier<Button>
	{
		public static readonly PressedAppearance Instance = new();
		public override Button Apply(Button view) => view
			.Background(Theme.Token(ColorTokens.PrimaryContainer))
			.Color(Theme.Token(ColorTokens.OnPrimaryContainer))
			.ClipShape(new RoundedRectangle(20))
			.Padding(new Thickness(24, 12));
	}

	sealed class HoveredAppearance : ViewModifier<Button>
	{
		public static readonly HoveredAppearance Instance = new();
		public override Button Apply(Button view) => view
			.Background(Theme.Token(ColorTokens.Primary).Map(c => c.WithAlpha(0.92f)))
			.Color(Theme.Token(ColorTokens.OnPrimary))
			.ClipShape(new RoundedRectangle(20))
			.Padding(new Thickness(24, 12));
	}

	sealed class DisabledAppearance : ViewModifier<Button>
	{
		public static readonly DisabledAppearance Instance = new();
		public override Button Apply(Button view) => view
			.Background(new SolidPaint(Colors.Gray.WithAlpha(0.12f)))
			.Color(Colors.Gray.WithAlpha(0.38f))
			.ClipShape(new RoundedRectangle(20))
			.Padding(new Thickness(24, 12))
			.Opacity(0.38);
	}
}
```

### 13.4 Theme Switching (Light/Dark Toggle)

```csharp
// Pages/SettingsPage.cs
using Comet;
using Comet.Styles;

public class SettingsPage : View
{
	readonly State<bool> isDark = false;
	readonly State<string> userName = "David";

	[Body]
	View Body() => ScrollView(
		VStack(spacing: Theme.Token(SpacingTokens.Medium)) {
			Text("Settings")
				.Modifier(AppStyles.PageTitle),

			// Theme section
			VStack {
				Text("Appearance")
					.Modifier(AppStyles.SectionHeader),

				HStack {
					Text("Dark Mode")
						.Modifier(AppStyles.BodyText),
					Spacer(),
					Toggle(isDark)
						.OnChanged(dark =>
						{
							ThemeManager.SetTheme(
								dark ? BrandThemes.Dark : BrandThemes.Light);
						}),
				},
			}.Modifier(AppStyles.Card),

			// Profile section
			VStack {
				Text("Profile")
					.Modifier(AppStyles.SectionHeader),

				TextField("Name", userName),
			}.Modifier(AppStyles.Card),
		}
		.Padding(Theme.Token(SpacingTokens.Medium))
	).Background(Theme.Token(ColorTokens.Background));
}
```

### 13.5 Scoped Theme Override ("Dark Card" in Light Theme)

```csharp
// Pages/DashboardPage.cs
using Comet;
using Comet.Styles;

public class DashboardPage : View
{
	readonly State<int> alertCount = 3;

	[Body]
	View Body() => ScrollView(
		VStack(spacing: 16) {
			Text("Dashboard")
				.Modifier(AppStyles.PageTitle),

			// Normal cards — use the app's active theme
			VStack {
				Text("Revenue")
					.Modifier(AppStyles.SectionHeader),
				Text("$42,500")
					.FontSize(32)
					.FontWeight(FontWeight.Bold)
					.Color(Theme.Token(ColorTokens.Primary)),
			}.Modifier(AppStyles.Card),

			// This card is ALWAYS dark, regardless of the app theme
			VStack {
				Text("System Alerts")
					.Modifier(AppStyles.SectionHeader),
				Text(() => $"{alertCount.Value} active alerts")
					.Modifier(AppStyles.BodyText),
				Button("View All", () => { /* navigate */ })
			}
			.Modifier(AppStyles.Card)
			.Theme(BrandThemes.Dark),    // ← scoped dark theme

			// Back to the app's active theme
			VStack {
				Text("Recent Activity")
					.Modifier(AppStyles.SectionHeader),
				Text("Last login: 2 hours ago")
					.Modifier(AppStyles.Caption),
			}.Modifier(AppStyles.Card),
		}
		.Padding(16)
	).Background(Theme.Token(ColorTokens.Background));
}
```

---

## 14. SwiftUI Comparison

### 14.1 Reusable Styles

| Concern | SwiftUI | Comet (Proposed) |
|---------|---------|------------------|
| Abstraction | `ViewModifier` protocol | `ViewModifier` class |
| Define | `struct MyModifier: ViewModifier { func body(content:) }` | `class MyModifier : ViewModifier { View Apply(View) }` |
| Apply | `.modifier(MyModifier())` | `.Modifier(new MyModifier())` |
| Compose | `.modifier(A()).modifier(B())` | `.Modifier(a, b)` or `a.Then(b)` |
| Static | Extension method returning `some View` | `static readonly ViewModifier` field |

```swift
// SwiftUI
struct CardModifier: ViewModifier {
    func body(content: Content) -> some View {
        content
            .padding(16)
            .background(.surfaceContainer)
            .clipShape(RoundedRectangle(cornerRadius: 12))
    }
}
Text("Hello").modifier(CardModifier())
```

```csharp
// Comet
class CardModifier : ViewModifier
{
	public override View Apply(View view) => view
		.Padding(16)
		.Background(Theme.Token(ColorTokens.SurfaceContainer))
		.ClipShape(new RoundedRectangle(12));
}
Text("Hello").Modifier(new CardModifier())
```

### 14.2 Per-Control Styling

| Concern | SwiftUI | Comet (Proposed) |
|---------|---------|------------------|
| Protocol | `ButtonStyle` | `IControlStyle<Button, ButtonConfiguration>` |
| Config | `ButtonStyleConfiguration` | `ButtonConfiguration` (struct) |
| Apply scoped | `.buttonStyle(.borderedProminent)` | `.ButtonStyle(ButtonStyles.Filled)` |
| Apply global | In the app root or `@Environment` | `theme.SetControlStyle(...)` |

```swift
// SwiftUI
struct MyButtonStyle: ButtonStyle {
    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .padding(.horizontal, 24)
            .background(configuration.isPressed ? .blue.opacity(0.8) : .blue)
            .foregroundStyle(.white)
            .clipShape(Capsule())
    }
}
Button("Tap") { }.buttonStyle(MyButtonStyle())
```

```csharp
// Comet
class MyButtonStyle : IControlStyle<Button, ButtonConfiguration>
{
	public ViewModifier Resolve(ButtonConfiguration config)
	{
		var bg = config.IsPressed
			? Theme.Token(ColorTokens.Primary).Map(c => c.WithAlpha(0.8f))
			: Theme.Token(ColorTokens.Primary);
		return new InlineModifier(v => v
			.Padding(new Thickness(24, 0))
			.Background(bg)
			.Color(Theme.Token(ColorTokens.OnPrimary))
			.ClipShape(new RoundedRectangle(9999)));
	}
}
Button("Tap", OnClick).ButtonStyle(new MyButtonStyle())
```

### 14.3 Theme Definition

| Concern | SwiftUI | Comet (Proposed) |
|---------|---------|------------------|
| Type | `@Environment(\.colorScheme)` | `ThemeManager.Current()` returns `Theme` |
| Tokens | `Color.accentColor`, `ShapeStyle` | `ColorTokens.Primary`, `ShapeTokens.Medium` |
| Define | System provided, custom via assets | `new Theme { Colors = ..., Typography = ... }` |
| Compose | N/A (system themes only) | `theme with { Colors = ... }` (record `with`) |

### 14.4 Theme Switching

| Concern | SwiftUI | Comet (Proposed) |
|---------|---------|------------------|
| API | `.preferredColorScheme(.dark)` | `ThemeManager.SetTheme(AppThemes.Dark)` |
| Scoped | `.environment(\.colorScheme, .dark)` | `.Theme(AppThemes.Dark)` |
| Reactive | Automatic via `@Environment` | Automatic via `Binding<T>` + `StateManager` |

```swift
// SwiftUI
VStack {
    Text("Dark Card")
}
.environment(\.colorScheme, .dark)
```

```csharp
// Comet
VStack {
	Text("Dark Card"),
}
.Theme(AppThemes.Dark)
```

### 14.5 Token Access

| Concern | SwiftUI | Comet (Proposed) |
|---------|---------|------------------|
| Color | `Color.primary`, `.tint(.blue)` | `Theme.Token(ColorTokens.Primary)` |
| Font | `.font(.headline)` | `.FontSize(Theme.Token(TypographyTokens.HeadlineMedium).Map(f => f.Size))` |
| Spacing | No built-in tokens | `Theme.Token(SpacingTokens.Medium)` |
| Shape | `.clipShape(.rect(cornerRadius: 12))` | `.ClipShape(new RoundedRectangle(12))` |

---

## 15. Open Questions

### 15.1 Architecture Decisions for David

**Q1: Inline modifier sugar?**

Should there be a lightweight inline modifier for one-off cases?

```csharp
// Option A: Always require a class
Text("Hello").Modifier(new HeaderTextStyle())

// Option B: Also support inline lambdas
Text("Hello").Modifier(v => v.FontSize(24).FontWeight(FontWeight.Bold))
```

**Recommendation:** Support both. The `InlineModifier` class wraps a lambda:

```csharp
public sealed class InlineModifier : ViewModifier
{
	readonly Func<View, View> _apply;
	public InlineModifier(Func<View, View> apply) => _apply = apply;
	public override View Apply(View view) => _apply(view);
}

public static T Modifier<T>(this T view, Func<View, View> apply) where T : View
	=> view.Modifier(new InlineModifier(apply));
```

**Q2: Should `Token<T>` have a direct implicit conversion to `Binding<T>`?**

This would make token usage more concise but less explicit:

```csharp
// Explicit (proposed):
Text("Hello").Color(Theme.Token(ColorTokens.Primary))

// Implicit (alternative):
Text("Hello").Color(ColorTokens.Primary)  // implicit Token<Color> → Binding<Color>
```

**Recommendation:** Start explicit. Add implicit conversion later if ergonomics demand it.

**Q3: Typography tokens — FontSpec or separate Size/Weight/Family tokens?**

```csharp
// Option A: Composite FontSpec token (proposed)
.FontSize(Theme.Token(TypographyTokens.BodyLarge).Map(f => f.Size))

// Option B: Separate tokens for each property
.FontSize(Theme.Token(TypographyTokens.BodyLargeSize))
.FontWeight(Theme.Token(TypographyTokens.BodyLargeWeight))

// Option C: A convenience extension that applies all font properties at once
.Typography(TypographyTokens.BodyLarge)
```

**Recommendation:** Option C as sugar on top of Option A. The `.Typography()` extension applies size, weight, family, and line height in one call.

**Q4: Should `ViewModifier.Apply()` return `View` or `void`?**

Returning `View` enables chaining composition patterns but allows the modifier to return a different view (wrapping). Returning `void` means modifiers can only mutate the given view.

**Recommendation:** Return `View` (as proposed). The wrapping capability is useful for modifiers that add decoration (borders, shadows via container wrapping). The fluent chain already handles the common case.

**Q5: Animation integration?**

Should control styles support declarative transitions when state changes?

```csharp
// Hypothetical:
public ViewModifier Resolve(ButtonConfiguration config)
{
	return new ButtonAppearance(bg, fg)
		.WithTransition(duration: 150, easing: Easing.CubicOut);
}
```

**Recommendation:** Defer to a separate animation spec. The control state model (Section 9) provides the foundation. Animation can layer on top by observing state changes and interpolating property values.

**Q6: Source generator integration timeline?**

Should the source generator be updated to emit:
- `StyleToken<T>` for each generated control?
- `{Control}Configuration` structs?
- `{Control}StyleExtensions` with scoped `.{Control}Style()` methods?

**Recommendation:** Phase 1 is hand-written for 5 core controls (Button, Toggle, Slider, TextField, Text). Phase 2 updates the source generator to emit these for all `[CometGenerate]` controls.

### 15.2 Migration Strategy

Since this is greenfield, the old `Style`, `ControlStyle<T>`, `Style<T>`, and `Theme` classes remain in the codebase during the transition but are marked `[Obsolete]`. New code uses the new system exclusively. The old classes can be removed in a subsequent major version.

### 15.3 Naming

| Proposed Name | Alternative | Notes |
|---------------|-------------|-------|
| `ViewModifier` | `StyleModifier`, `Modifier` | Matches SwiftUI naming |
| `Token<T>` | `DesignToken<T>`, `ThemeKey<T>` | "Token" is standard design system terminology |
| `ColorTokens` | `ThemeColors`, `ColorKeys` | "Tokens" aligns with Material Design language |
| `ThemeManager` | `Theme` (static), `ThemeContext` | Avoids collision with `Theme` record |
| `IControlStyle<T,C>` | `IStyleProtocol<T,C>` | "ControlStyle" matches SwiftUI's pattern naming |

---

## Appendix A: File Layout

```
src/Comet/Styles/
├── Tokens/
│   ├── Token.cs                    // Token<T> definition
│   ├── ColorTokens.cs              // Static color token fields
│   ├── TypographyTokens.cs         // Static typography token fields
│   ├── SpacingTokens.cs            // Static spacing token fields
│   └── ShapeTokens.cs              // Static shape token fields
├── TokenSets/
│   ├── ColorTokenSet.cs            // Record with Color properties
│   ├── TypographyTokenSet.cs       // Record with FontSpec properties
│   ├── SpacingTokenSet.cs          // Record with double properties
│   ├── ShapeTokenSet.cs            // Record with double properties
│   └── FontSpec.cs                 // Readonly record struct
├── Modifiers/
│   ├── ViewModifier.cs             // Base class + generic variant
│   ├── ComposedModifier.cs         // a.Then(b) composition
│   ├── InlineModifier.cs           // Lambda wrapper
│   └── ViewModifierExtensions.cs   // .Modifier() extension methods
├── ControlStyles/
│   ├── IControlStyle.cs            // Interface definition
│   ├── ButtonConfiguration.cs      // Configuration struct
│   ├── ToggleConfiguration.cs
│   ├── TextFieldConfiguration.cs
│   ├── SliderConfiguration.cs
│   ├── ButtonStyles.cs             // Built-in button style variants
│   ├── ToggleStyles.cs
│   └── StyleToken.cs               // StyleToken<T> environment key
├── Theme.cs                        // Theme record
├── ThemeManager.cs                 // Static theme management
├── ThemeDefaults.cs                // Built-in Light/Dark themes
├── TypographyDefaults.cs           // Material3 typography presets
├── SpacingDefaults.cs              // Standard spacing presets
└── ShapeDefaults.cs                // Rounded shape presets
```

## Appendix B: Summary of What's Removed

| Current Type | Replacement | Reason |
|-------------|-------------|--------|
| `Style` (monolithic) | `ViewModifier` + `Theme` | One class did three jobs |
| `Style<T>` (`Action<T>`) | `ViewModifier<T>` | Same pattern, composable |
| `ControlStyle<T>` (string dict) | `IControlStyle<T, TConfig>` | Type-safe, state-aware |
| `StyleAwareValue<TEnum, T>` | `IControlStyle.Resolve(config)` | State handled in style protocol |
| `ButtonStyle` / `SliderStyle` / etc. | Per-control `IControlStyle` impls | No more one-class-per-control in Style.cs |
| `ThemeColors` (27 properties + Apply) | `ColorTokenSet` (record) | No push-to-env; resolved lazily |
| `EnvironmentKeys.ThemeColor.*` strings | `ColorTokens.*` static fields | Type-safe, IntelliSense-discoverable |
| `IThemeable` interface | Automatic via `Binding<T>` reactivity | No opt-in needed |
| `DefaultThemeStyles.Register()` | `Theme.SetControlStyle()` in theme definition | Explicit, not magical |
| `Theme.Apply()` (push all values) | `ThemeManager.SetTheme()` (set one reference) | O(1) switch vs O(N×M) |

---

*End of specification.*
