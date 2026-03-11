# Decision: MauiDevFlow + Comet Gesture/Tap Compatibility

**Author:** Holden (Lead Architect)  
**Date:** 2025-07-08  
**Status:** Proposed  
**Impact:** Critical — blocks automated visual parity workflow

## Context

MauiDevFlow's tap, scroll, and property inspection commands work on standard MAUI views but fail on Comet views embedded via CometHost. This blocks our automated visual parity comparison skill.

## Root Causes (5 issues identified)

### Issue 1: Tap fails on Comet views with gestures (e.g., Text with `.OnTap()`)
MauiDevFlow checks `Microsoft.Maui.Controls.View.GestureRecognizers` for MAUI `TapGestureRecognizer`. Comet views are NOT `Microsoft.Maui.Controls.View` — they use `IGestureView.Gestures` (Comet's gesture system). The switch statement skips the gesture check entirely.

### Issue 2: Scroll fails — "No scrollable view found"
MauiDevFlow uses `FindDescendant<ScrollView>()` targeting `Microsoft.Maui.Controls.ScrollView`. Comet's `ScrollView` implements `IScrollView` but is NOT a MAUI ScrollView. The scroll handler also checks `el is VisualElement` which fails for Comet views.

### Issue 3: Hash-based IDs are unstable across state changes
Comet views are not `Element` or `VisualElement`, so MauiDevFlow generates IDs via `RuntimeHelpers.GetHashCode()`. Stable for same instance, but Comet rebuilds view instances on state changes.

### Issue 4: `EnsurePlatformStableId` misses Comet views
Receives the IVisualTreeElement (Comet.View), checks `is UIKit.UIView` — fails. Doesn't try getting the platform view via `IView.Handler.PlatformView`.

### Issue 5: Controls with MAUI interfaces (IButton, ISwitch) — WORKS
MauiDevFlow already handles `IButton.Clicked()`, `ISwitch.IsOn`, etc. No fix needed.

## Decisions

### 1. Comet-side: Always stamp platform views with stable identifiers (IMPLEMENTED)
Modified `ApplyInspectionMetadata` to set `AccessibilityIdentifier` (iOS) / `ContentDescription` (Android) using `View.Id` as fallback when no explicit `AutomationId` is set. Also marks gesture-bearing views as `IsAccessibilityElement = true` and `UserInteractionEnabled = true`.

### 2. MauiDevFlow: Needs IGestureView tap support (PROPOSED — external repo)
`HandleTap` should check `IGestureView.Gestures` for `TapGesture` and call `.Invoke()`. This is a ~15-line addition via reflection (to avoid Comet dependency).

### 3. MauiDevFlow: Needs IScrollView/IView scroll support (PROPOSED — external repo)
`HandleScroll` should check `IScrollView` interface, and the `VisualElement` type check should be relaxed to `IView` for Comet views. Native scroll via `UIScrollView` should be attempted via handler platform view.

### 4. MauiDevFlow: Needs IView.AutomationId in GenerateId (PROPOSED — external repo)
`GenerateId` should check `IView.AutomationId` (not just `VisualElement.AutomationId`) to pick up Comet's auto-stamped platform identifiers.

## Impact

With decision #1 (implemented), Comet views are better automation citizens. With decisions #2-4 (MauiDevFlow changes), full tap/scroll/property support would work on Comet views.

## Workarounds (current)

- Button/Toggle/Switch tap: Works via MAUI interfaces
- Text with gestures: Does NOT work — must use direct HTTP POST to agent and handle result
- Scroll: Does NOT work — no workaround
- Tree: Works with `--depth 0` (unlimited)
