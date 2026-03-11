# AppKit Phase B — Platform.Maui.MacOS Integration + CometMacApp

**Owner:** Holden (Lead Architect)
**Date:** 2026-03-11
**Status:** Complete

## Decision

Wire Platform.Maui.MacOS into Comet as a live ProjectReference for `net10.0-macos` and ship a minimal `CometMacApp` sample that builds, bundles, and launches on macOS AppKit.

## Key Technical Findings

1. **MauiVersion alignment:** Platform.Maui.MacOS pins MAUI 10.0.31. Comet's macOS TFM must match (was 10.0.1).
2. **UseCometAppMacOS<T>():** New convenience extension combining `UseMauiAppMacOS<T>()` + `UseCometHandlers()`. Guarded by `#if __MACOS__`.
3. **MAUI SingleProject excludes macOS platform files:** Files under `Platforms/macOS/` are silently dropped from compilation by the MAUI SDK. Workaround: place `Main.cs` and `MauiMacOSApp.cs` at the project root.
4. **OutputType=Exe required for .app bundle:** Without it, the macOS SDK builds a library with no native bundle. The mauiplatforms sample comments out OutputType and doesn't produce a bundle.
5. **Platform.Maui.MacOS.targets import:** Required by consuming projects for icon generation, XProtect workarounds, and `dotnet build -t:Run` support.

## Outcome

- Build: ✅ (all TFMs: macos, maccatalyst, ios, android)
- App bundle: ✅ (`Comet Mac.app` created)
- Launch: ✅ (process runs without crash)
- Window visible: Requires manual verification by David (agent cannot screencapture)
- Tests: ✅ (846 passed, 0 failed)

## Impact

Comet now has a working AppKit build path. The `CometMacApp` sample serves as the template for Comet-on-AppKit development.
