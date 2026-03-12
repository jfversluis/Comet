# Decision: CometWindowHandler for macOS

**Owner:** Holden (Lead Architect)  
**Status:** Implemented  
**Date:** 2025-07-24

## Decision

Created a dedicated `CometWindowHandler` (`ElementHandler<IWindow, NSWindow>`) for macOS instead of reusing `Microsoft.Maui.Handlers.WindowHandler` or `Platform.Maui.MacOS.Handlers.WindowHandler`.

## Rationale

- The generic MAUI `WindowHandler` has no AppKit NSWindow creation — it's a stub that produces no visible window.
- Platform.Maui.MacOS's `WindowHandler` casts `VirtualView` to `BindableObject` and `Microsoft.Maui.Controls.Window`, but `CometWindow` extends Comet's `View → ContextualObject`, not `BindableObject`. Direct reuse would throw `InvalidCastException`.
- The new handler follows the same `FlippedNSView` content container pattern as Platform.Maui.MacOS (top-left coordinate system) and delegates content resolution to Comet's existing `ToMacOSPlatform()` bridge.

## Impact

- Handler registration uses `#if __MACOS__` conditional in `AppHostBuilderExtensions.cs` — non-macOS platforms keep using `WindowHandler` unchanged.
- File lives at `src/Comet/Handlers/Window/CometWindowHandler.MacOS.cs` (only compiled for `net10.0-macos` per `Directory.Build.targets` convention).
- 846 tests pass, 0 regressions.
