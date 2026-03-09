### Style System Core Primitives — Implementation Decisions

**Owner:** Holden (Lead Architect)  
**Date:** 2026-03-10  
**Status:** Implemented

#### D1: UseTheme() instead of Theme()
The scoped theme override extension is named `.UseTheme()` not `.Theme()` per the spec. C# cannot distinguish an extension method named `Theme<T>()` from the `Theme` type within `Comet.Styles` namespace. All consuming code should use `.UseTheme(myTheme)` for subtree theme scoping.

#### D2: Additive Theme properties, not new Theme record
The spec defined `record Theme` but the existing codebase has `class Theme` with 640+ tests depending on `Theme.Current`, `Theme.Light`, `Theme.Dark`, etc. Creating a parallel `record Theme` in the same namespace is a C# error. Instead, token set properties (`Colors`, `Typography`, `Spacing`, `Shapes`) were added directly to the existing Theme class. The new `ThemeManager` provides the reactive resolution path forward; the legacy `Theme.Current` path continues to work.

#### D3: MauiColors alias for Microsoft.Maui.Graphics.Colors
The `Colors` property on Theme shadows `Microsoft.Maui.Graphics.Colors` (a static class with `White`, `Black`, etc.). Theme.cs and ThemeDefaults.cs use `using MauiColors = Microsoft.Maui.Graphics.Colors;` to disambiguate. Other files in `Comet.Styles` that need `Colors.xxx` should use this alias pattern.

#### D4: Token.Resolve(View) overload
Added `Token<T>.Resolve(View)` in addition to `Resolve(Theme)` because Amos's BuiltInStyles already calls `token.Resolve(config.TargetView)`. This overload resolves the nearest scoped theme from the view, then delegates to `Resolve(Theme)`.

#### D5: Pre-existing BuiltInStyles build error
`BuiltInStyles.cs` references `Comet.Graphics.RoundedRectangle` but the class lives in `Comet` namespace. This blocks the full project build. Amos should fix the namespace reference to `new Comet.RoundedRectangle(20)` or add a `using Comet;` directive.

**Impact:** Bobbie's test suite cannot build until D5 is resolved (Amos's fix needed). The style primitives themselves compile cleanly and are ready for Amos's control style types and Naomi's source generator integration.
