# Comet Platform-Specific Code Documentation

## 📋 Overview

This comprehensive documentation package provides **complete platform-specific code analysis** for Comet, a .NET MAUI cross-platform framework. All platform-specific implementations for **iOS**, **Android**, **Windows (WinUI)**, and **Mac Catalyst** are thoroughly documented.

## 📦 Documentation Files

### 1. **PLATFORM_SPECIFIC_CODE_DOCUMENTATION.md** ⭐ (Main Document)
   - **Size:** 65 KB, 2375 lines
   - **Content:** Complete technical documentation with:
     - Full method signatures and line numbers
     - All iOS, Android, and Windows implementations
     - 20+ conditional compilation patterns
     - Event handling patterns per platform
     - Modal dialog implementations
     - Navigation patterns
     - Layout measurement algorithms

### 2. **PLATFORM_CODE_INDEX.md** (Navigation Guide)
   - **Size:** 12 KB, 404 lines
   - **Content:** Quick reference navigation:
     - File organization by platform
     - Method signature reference
     - Feature mapping by platform
     - Key patterns explained
     - Critical code locations
     - Statistics and quick links

### 3. **PLATFORM_ANALYSIS_SUMMARY.txt** (Executive Summary)
   - **Size:** 10 KB, 267 lines
   - **Content:** High-level overview:
     - Key findings summary
     - Architecture comparison
     - Feature implementation details
     - Directory structure
     - Platform-specific features list

## 🎯 Key Findings

### Root View Architecture

| Platform | Base Class | Layout Method | File | Lines |
|----------|-----------|---------------|----|-------|
| **iOS** | UIView | `LayoutSubviews()` | `Platform/iOS/CometView.cs` | 105 |
| **Android** | ViewGroup | `OnMeasure()`/`OnLayout()` | `Platform/Android/CometView.cs` | 119 |
| **Windows** | Grid (WinUI) | `MeasureOverride()`/`ArrangeOverride()` | `Platform/Windows/CometView.cs` | 106 |

### Platform-Specific Features

**iOS (26 files)**
- UIViewController modal presentation
- UINavigationController back button handling
- CALayer shadow & border styling
- UIKit gesture recognition
- Safe area support via ISafeAreaView

**Android (12 files)**
- DialogFragment modal management
- Fragment transaction stack
- BottomNavigationView tab navigation
- GestureDetector touch handling
- DisplayMetrics density conversion
- State persistence via Bundle

**Windows (3 files)**
- WinUI Grid-based layout
- Canvas container positioning
- Color conversion via ColorHelper.FromArgb
- No modal/tab system (uses MAUI defaults)

### Handler Implementation

**Total:** 33 platform-specific handlers (11 each: iOS, Android, Windows)

**Key Handlers:**
- CometHostHandler (3 × 112-143 lines)
- CometViewHandler (iOS: 49, Android: 11, Windows: 57)
- NavigationViewHandler, TabViewHandler, ListViewHandler, etc.

## 📍 File Locations

### Platform-Specific Implementations
```
src/Comet/Platform/
├── iOS/          (26 files - 3000+ lines)
├── Android/      (12 files - 2500+ lines)
├── Windows/      (3 files - 1500+ lines)
└── Standard/     (1 file)

src/Comet/Handlers/
├── CometHost/    (CometHostHandler.cs + 3 platform implementations)
├── View/         (CometViewHandler.cs + 3 platform implementations)
├── Navigation/   (NavigationViewHandler.cs + 3 platform implementations)
└── ... 8 more handlers × 3 platforms
```

### Configuration
```
Directory.Build.targets      - File inclusion/exclusion by platform
AppHostBuilderExtensions.cs  - 1151 lines with 20+ conditional sections
CometApp.cs                  - Platform-specific initialization
CometWindow.cs               - DisplayScale setup per platform
PlatformExtensions.cs        - OnPlatform<T> and OnIdiom<T> helpers
```

## 🔧 Conditional Compilation

**Supported Symbols:**
- `#if __IOS__` - iOS specific code
- `#if __ANDROID__` / `#if ANDROID` - Android specific code
- `#if WINDOWS` - Windows/WinUI specific code
- `#if MACCATALYST` - Mac Catalyst specific code
- `#if __IOS__ || MACCATALYST` - iOS and Mac Catalyst shared code

**Usage Statistics:**
- Total conditional regions: 40+
- Largest file: AppHostBuilderExtensions.cs (20+ regions, ~800 conditional lines)
- Platforms covered: iOS, Android, Windows, Mac Catalyst

## 📊 Code Statistics

| Metric | Count |
|--------|-------|
| Platform-Specific Files | 42 |
| Platform-Specific Handlers | 33 |
| Conditional Compilation Regions | 40+ |
| iOS Implementation Lines | 3000+ |
| Android Implementation Lines | 2500+ |
| Windows Implementation Lines | 1500+ |
| Total Platform-Specific Code | 7000+ |

## 🎓 How to Use This Documentation

### For Architecture Overview
→ Start with **PLATFORM_ANALYSIS_SUMMARY.txt**

### For Implementation Details
→ Use **PLATFORM_SPECIFIC_CODE_DOCUMENTATION.md**
- Look up specific handler implementations
- Find method signatures with exact line numbers
- Understand conditional compilation patterns

### For Navigation
→ Reference **PLATFORM_CODE_INDEX.md**
- Find file locations quickly
- Locate method signatures
- Understand feature mapping by platform
- Follow code examples

### For Quick Lookups
Use these quick references:

**Find iOS Implementation:**
```
iOS/CometView.cs (Root view)
iOS/CometViewController.cs (View controller)
Handlers/*/iOS.cs (Handler implementations)
```

**Find Android Implementation:**
```
Android/CometView.cs (Root view)
Android/CometFragment.cs (Fragment wrapper)
Handlers/*/Android.cs (Handler implementations)
```

**Find Windows Implementation:**
```
Windows/CometView.cs (Root view)
Handlers/*Windows.cs (Handler implementations)
```

## 🔍 Specific Topic Lookup

### Modal Dialogs
- iOS: CometApp.cs (PresentingViewController), CometViewController.cs
- Android: Platform/Android/ModalManager.cs (91 lines)
- Windows: N/A (uses MAUI defaults)

### Tab Navigation
- iOS: Platform/iOS/CUITabView.cs
- Android: Platform/Android/CometTabView.cs (88 lines)
- Windows: N/A

### List Views
- iOS: Platform/iOS/CUITableView.cs + CUITableViewSource.cs
- Android: Platform/Android/CometRecyclerView.cs
- Windows: Platform/Windows/ListCell.cs (87 lines)

### Navigation
- iOS: Platform/iOS/CUINavigationController.cs (37 lines)
- Android: CometFragment.cs + FragmentManager
- Windows: Standard Frame navigation

### Styling & Colors
- iOS: AppHostBuilderExtensions.cs (#if __IOS__ sections)
- Android: AppHostBuilderExtensions.cs (#elif ANDROID sections)
- Windows: AppHostBuilderExtensions.cs (#if WINDOWS sections)

## 💡 Key Patterns

### 1. Handler Reuse Pattern (Optimization)
Avoids unnecessary handler recreation when view types match:
```csharp
if (v.GetContentTypeHashCode() == pv.GetContentTypeHashCode() 
    && currentHandler != null)
{
    v.ViewHandler = currentHandler;
    return;
}
```

### 2. Density Conversion (Android)
Converts Android pixels to device-independent units:
```csharp
var deviceIndependentWidth = widthMeasureSpec.ToDouble(Context);
var nativeWidth = Context.ToPixels(size.Width);
```

### 3. Re-entrant Layout Prevention (iOS)
Prevents recursive layout calls:
```csharp
if (_inLayout) return;
_inLayout = true;
try { /* layout */ }
finally { _inLayout = false; }
```

### 4. Event Handler Caching (iOS)
Prevents duplicate event subscriptions during mapper re-fires:
```csharp
ConditionalWeakTable<UIView, EventHandler> _handlers = new();
```

### 5. Platform-Specific Styling
Uses conditional compilation to apply native APIs:
```csharp
#if __IOS__ || MACCATALYST
    layer.ShadowOpacity = shadow.Opacity;
#elif ANDROID
    platformView.Elevation = shadow.Radius * density;
#endif
```

## 🚀 Getting Started

1. **Read the Overview** (PLATFORM_ANALYSIS_SUMMARY.txt)
2. **Find Your Platform** (PLATFORM_CODE_INDEX.md)
3. **Review Implementation** (PLATFORM_SPECIFIC_CODE_DOCUMENTATION.md)
4. **Look Up Specific Code** (Method signatures by platform)

## 📝 Document Contents at a Glance

### PLATFORM_SPECIFIC_CODE_DOCUMENTATION.md
- ✅ Complete root view implementations (iOS/Android/Windows)
- ✅ Handler implementations with full signatures
- ✅ Platform-specific handler details (33 handlers)
- ✅ Conditional compilation patterns (40+ sections)
- ✅ iOS-specific code (CometApp, CometViewController, CUINavigationController)
- ✅ Android-specific code (CometFragment, ModalManager, CometTabView)
- ✅ Windows-specific code (CometView, ListCell)
- ✅ Service registration patterns
- ✅ Safe area and status bar handling
- ✅ Platform extension helpers

### PLATFORM_CODE_INDEX.md
- ✅ File organization by platform
- ✅ Complete directory structure
- ✅ Handler classification by type
- ✅ Method signature reference for all root views
- ✅ Feature mapping table by platform
- ✅ Critical code locations index
- ✅ Quick navigation links
- ✅ File statistics and counts

### PLATFORM_ANALYSIS_SUMMARY.txt
- ✅ Key findings (10 sections)
- ✅ Root view architecture
- ✅ Handler implementation pattern
- ✅ Conditional compilation patterns
- ✅ Platform-specific features list
- ✅ Directory structure with file counts
- ✅ Service registration patterns
- ✅ Layout & measurement patterns
- ✅ Critical features implemented
- ✅ Density/scale handling summary

## ✨ Highlights

**Most Complete Documentation:**
- Every method signature documented with line numbers
- All conditional compilation patterns explained
- Platform comparison tables
- Code examples for each major pattern
- File organization clearly mapped

**Easy Navigation:**
- Quick reference index in PLATFORM_CODE_INDEX.md
- Section jumps to specific platforms
- File lookup table
- Feature-to-implementation mapping

**Practical Reference:**
- Exact line numbers for all code
- Copy-paste ready code examples
- Platform-specific implementation patterns
- Event handling patterns per platform

## 📚 Additional Notes

- **Total Documentation:** 3,046 lines across 3 files
- **Code Coverage:** 7,000+ lines of platform-specific implementation
- **Platforms:** iOS, Android, Windows, Mac Catalyst
- **Handlers:** 33 platform-specific handler implementations
- **Conditional Regions:** 40+ platform-specific code sections

---

**Generated:** March 2024
**Comet Version:** Current (Latest)
**Documentation Level:** Comprehensive Technical Reference

For questions or clarifications, refer to specific files or line numbers provided in the documentation.
