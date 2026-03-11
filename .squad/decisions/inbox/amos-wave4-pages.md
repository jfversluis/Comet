# Wave 4 Gallery Pages — API Disambiguation Patterns

**Date:** 2026-03-11  
**Author:** Amos (Controls & API Dev)  
**Context:** Gallery Wave 4 (Gestures, Graphics, Transforms, Theme)

## Decision

When writing Comet gallery pages that import both `Comet` and `Microsoft.Maui` namespaces:

1. **Always fully qualify `Comet.GestureStatus`** — it collides with `Microsoft.Maui.GestureStatus`.
2. **Always use named parameter `scale:` for `ScaleTo`** — `ScaleTo(1.5, 0.5, easing)` is ambiguous between the uniform-scale and separate-scaleXY overloads.
3. **Use `SetState(s => { })` instead of `Invalidate()`** on `Component<T>` — Component has no Invalidate method; an empty SetState triggers re-render.
4. **GraphicsView inline Draw lambdas** — Comet's GraphicsView accepts `Action<ICanvas, RectF>` directly, so no separate IDrawable class is needed for gallery demos.

## Rationale

These patterns avoid the most common compilation errors discovered during Wave 4 and should be followed in all future gallery pages.
