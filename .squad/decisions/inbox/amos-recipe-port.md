# Decision: CometRecipeApp Sample Patterns

**Author:** Amos (Controls & API Dev)
**Date:** 2025-07-18
**Verdict:** GO — merged to dev

## Context

Ported the MauiReactor RecipeApp to Comet as `sample/CometRecipeApp/`. This established patterns for building real-world Comet samples.

## Decisions

### 1. Grid over ZStack for positioned layouts
ZStack gives all children the full bounds (no alignment support). For card layouts with text on the left and image on the right, use Grid with column definitions:
```csharp
new Grid(columns: new object[] { "*", 140 }) {
    textContent.Cell(row: 0, column: 0),
    image.Cell(row: 0, column: 1)
}
```

### 2. Navigation from parent View, not from layout child
`OnTapNavigate(() => dest)` doesn't work on Grid/layout views (LayoutHandler doesn't fire gesture mappers). Use `OnTap` with explicit navigation from the parent View:
```csharp
// Inside MainPage (a View connected to NavigationView)
card.OnTap(_ => this.Navigate(new DetailPage(recipe)))
```

### 3. Sample follows CometApp direct pattern
Uses `builder.UseCometApp<RecipeApp>()` where RecipeApp extends CometApp with a `[Body]` method. This is the simpler pattern (vs. Shell + Comet views) appropriate for single-flow apps.

### 4. Named parameters required for VStack/HStack spacing
`new VStack(16)` is interpreted as `LayoutAlignment` enum. Always use: `new VStack(spacing: 16)`.

## Impact
These patterns should be documented for future sample authors. The ZStack limitation and navigation workaround are the most likely to trip up contributors.
