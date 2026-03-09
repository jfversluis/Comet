# Decision: Style System Test Conventions

**Owner:** Bobbie (Test Engineer)  
**Date:** 2026-03-09  
**Status:** Proposed

## Decision

Style system tests live in `tests/Comet.Tests/Styles/` subdirectory with flat `Comet.Tests` namespace (matching existing project convention from ComponentTests). Tests are TDD against `docs/STYLE_THEME_SPEC.md` and import `using Comet.Styles;` for new types.

## Key findings for implementation agents

1. **`Theme` must become a `record`** (spec §5.2) for `with` expression tests to compile. Currently a `class`.
2. **`ControlState` values** need reordering per spec §9.1: Disabled=1, Pressed=2, Hovered=4, Focused=8.
3. **`Theme.SetControlStyle<TControl, TConfig>`** parameter should be typed `IControlStyle<TControl, TConfig>` not `object`.
4. **Missing types needed for tests:** `ViewModifier` (abstract), `ViewModifier<T>`, `ComposedModifier`, `ViewModifier.Empty`, `ThemeManager`, `ColorTokens`, `TypographyTokens`, `SpacingTokens`, `ShapeTokens`, `.OverrideToken()`, `.Theme()` extension, `.Modifier()` extension.

## Impact

113 test methods ready. When implementation lands, run: `dotnet test tests/Comet.Tests/Comet.Tests.csproj -c Release --filter "Comet.Tests.TokenTests|Comet.Tests.ViewModifierTests|Comet.Tests.NewThemeTests|Comet.Tests.ThemeManagerTests|Comet.Tests.ControlStateTests|Comet.Tests.NewControlStyleTests"`
