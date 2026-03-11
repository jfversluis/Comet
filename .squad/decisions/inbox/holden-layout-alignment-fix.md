# Decision: Default Layout Alignment Changed from Center to Fill

**Author:** Holden (Lead Architect)
**Date:** 2026-03-11
**Status:** Implemented

## Context

All content in Comet apps was horizontally centered by default — every page, every control, every layout container. This was a framework-level default alignment bug, not a per-page issue.

## Root Cause

Five default values across 4 files all defaulted to `LayoutAlignment.Center` instead of `LayoutAlignment.Fill`:

1. `VStack` constructor — cross-axis (horizontal) alignment for children
2. `HStack` constructor — cross-axis (vertical) alignment for children
3. `VStackLayoutManager` constructor — same as VStack
4. `HStackLayoutManager` constructor — same as HStack
5. `SetFrameFromPlatformView()` in `LayoutExtensions.cs` — fallback defaults for both axes

In .NET MAUI, `HorizontalOptions` and `VerticalOptions` default to `LayoutOptions.Fill`, which stretches views to fill available space. Comet's `Center` default caused views to size-to-content and center within their containers.

## Decision

Changed all five defaults from `LayoutAlignment.Center` to `LayoutAlignment.Fill`.

## Files Changed

- `src/Comet/Controls/VStack.cs`
- `src/Comet/Controls/HStack.cs`
- `src/Comet/Layout/VStackLayoutManager.cs`
- `src/Comet/Layout/HStackLayoutManager.cs`
- `src/Comet/Helpers/LayoutExtensions.cs`

## Impact

- All 846 tests pass (0 regressions)
- All 19 skipped tests remain skipped (pre-existing)
- Gallery sample builds cleanly
- Views now stretch to fill containers by default, matching MAUI behavior
- Explicit `.FillHorizontal()` / `.FitHorizontal()` still override defaults
- Style system overrides still apply (e.g., TextField/Slider/SecureField in VStack)

## Breaking Change

Any existing Comet app relying on implicit center alignment will see layout changes. This is intentional — the previous Center default was wrong relative to MAUI conventions.
