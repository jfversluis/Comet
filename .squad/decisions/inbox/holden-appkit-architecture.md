# AppKit Platform Architecture

**Date:** 2026-03-11  
**Owner:** Holden (Lead Architect)  
**Status:** In Progress  

## Decision: Proceed with AppKit (macOS native) support

David has directed the team to bring AppKit support to Comet. This overrides the previous assessment against the port. The implementation strategy is documented here.

## Architecture Decisions

### 1. File Convention: `*.MacOS.cs` and `Platform/MacOS/`

New AppKit-specific code uses the `*.MacOS.cs` suffix for handler partials and `Platform/MacOS/` for native view types. This parallels the existing `*.iOS.cs`/`Platform/iOS/` convention. The `__MACOS__` preprocessor symbol is automatically defined by the macOS workload for `net10.0-macos`.

**Rationale:** Consistent with existing platform conventions. `Directory.Build.targets` already had file filtering for iOS, Android, Windows, Mac — we added the MacOS pattern.

### 2. `UseMaui=false` for `net10.0-macos`

MAUI doesn't natively support `net10.0-macos` (AppKit). The MAUI workload targets only iOS, Android, macCatalyst, and Windows. For the macOS TFM:
- `UseMaui` is conditionally set to `true` only for non-macOS TFMs
- `MauiVersion` is defined explicitly for the macOS TFM (currently `10.0.1`)
- `Microsoft.Maui.Controls` is referenced as a NuGet package for all TFMs (works because MAUI ships platform-agnostic assemblies)

**Rationale:** Attempting `UseMaui=true` with a macOS TFM would fail at the workload level. The explicit approach gives us control.

### 3. Dependency Strategy: Platform.Maui.MacOS

**Development:** Project reference to `mauiplatforms/src/Platform.Maui.MacOS/Platform.Maui.MacOS.csproj` (commented out in csproj, to be enabled when we're ready to wire up real handlers).

**Shipping:** NuGet reference to `Platform.Maui.MacOS` package once published.

**Interim:** Comet ships a local `MacOSViewExtensions.cs` that provides `ToMacOSPlatform()` as a thin wrapper around MAUI's standard handler resolution. This allows handlers to compile standalone.

**Rationale:** Project reference for dev iteration speed; NuGet for shipping. The local extension method ensures the macOS TFM compiles independently.

### 4. Source Generator: No Changes Required

The `CometViewSourceGenerator` generates View wrappers from MAUI interfaces — it's platform-agnostic. The generated `Binding<T>` properties and constructors don't reference platform types. **Confirmed: generator compiles and runs correctly for the macOS TFM with zero changes.**

### 5. Handler Registration

`AppHostBuilderExtensions.cs` now includes `#elif __MACOS__` branches alongside existing `#if __IOS__` / `#if __MOBILE__` blocks. macOS uses Comet's custom handlers for:
- `ScrollView` → `Handlers.ScrollViewHandler`
- `ShapeView` → `Handlers.ShapeViewHandler`
- `NavigationView` → `Handlers.NavigationViewHandler`
- `View` → `CometViewHandler`

### 6. Handler Implementation Approach

Each of the 10 handler types gets a `.MacOS.cs` partial class. The pattern mirrors iOS but substitutes AppKit types:

| UIKit | AppKit Equivalent |
|-------|-------------------|
| `UIView` | `NSView` |
| `UIScrollView` | `NSScrollView` + `NSClipView` |
| `UINavigationController` | Custom container (no direct equivalent) |
| `UITabBarController` | `NSSegmentedControl` + content area |
| `UITableView` | `NSTableView` |
| `UITextField`/`UILabel` | `NSTextField` (serves both roles) |

### 7. Implementation Order

**Phase A — Build Infrastructure (DONE):**
1. ✅ Add `net10.0-macos` TFM to `Comet.csproj`
2. ✅ Add MacOS file filtering to `Directory.Build.targets`
3. ✅ Create stub `.MacOS.cs` handler files for all 10 handler types
4. ✅ Create `Platform/MacOS/` native view stubs (CometNSView, CUITabNSView)
5. ✅ Update handler registration in `AppHostBuilderExtensions.cs`
6. ✅ Verify all existing TFMs and tests still pass

**Phase B — Minimal Viable Window:**
1. Wire up Platform.Maui.MacOS project reference
2. Implement `CometViewHandler.MacOS.cs` with real view hosting
3. Implement basic layout (VStack/HStack) rendering
4. Get CometMauiApp sample running on macOS with a Text + Button

**Phase C — Navigation & Collections:**
1. Implement `NavigationViewHandler` with push/pop
2. Implement `TabViewHandler` with NSSegmentedControl
3. Implement `ScrollViewHandler` with NSScrollView
4. Implement `ListViewHandler` / `CollectionViewHandler` with NSTableView

**Phase D — Polish:**
1. Gesture support (NSClickGestureRecognizer, NSPanGestureRecognizer)
2. Native host interop
3. Menu bar integration (NSMenu)
4. Keyboard shortcuts

### 8. Sample App Target

`CometMauiApp` should be the first sample to target `net10.0-macos` — it's the minimal starter template. After that, `Comet.Sample` (the reference app with 50+ demos) provides broader coverage.

## File Inventory Created

### Handler `.MacOS.cs` files (10 files):
- `Handlers/View/CometViewHandler.MacOS.cs`
- `Handlers/ScrollView/ScrollViewHandler.MacOS.cs`
- `Handlers/TabView/TabViewHandler.MacOS.cs`
- `Handlers/Navigation/NavigationViewHandler.MacOS.cs`
- `Handlers/CollectionView/CollectionViewHandler.MacOS.cs`
- `Handlers/ListView/ListViewHandler.MacOS.cs`
- `Handlers/ShapeView/ShapeViewHandler.MacOS.cs`
- `Handlers/CometHost/CometHostHandler.MacOS.cs`
- `Handlers/MauiViewHost/MauiViewHostHandler.MacOS.cs`
- `Handlers/NativeHost/NativeHostHandler.MacOS.cs`

### Platform native views (4 files):
- `Platform/MacOS/CometNSView.cs` — Main container view (NSView + IReloadHandler)
- `Platform/MacOS/CUITabNSView.cs` — Tab container (NSSegmentedControl + content area)
- `Platform/MacOS/HandlerExtensions.cs` — Gesture add/remove stubs
- `Platform/MacOS/MacOSViewExtensions.cs` — `ToMacOSPlatform()` bridge method
