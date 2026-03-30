# Comet Platform-Specific Code - Complete Index

## Documentation Files Generated

1. **PLATFORM_SPECIFIC_CODE_DOCUMENTATION.md** (2375 lines, 85KB)
   - Complete comprehensive documentation with all code implementations
   - Every method signature with line numbers
   - All conditional compilation patterns
   - Full implementation details for iOS, Android, Windows

2. **PLATFORM_ANALYSIS_SUMMARY.txt** (191 lines)
   - Quick reference summary of key findings
   - Architecture overview
   - Feature comparison table

3. **PLATFORM_CODE_INDEX.md** (this file)
   - Navigation guide to all platform-specific code

---

## File Organization

### Root Platform Directory
**Location:** `src/Comet/Platform/`

#### iOS Implementation (26 files)
```
iOS/
├── CometView.cs (105 lines) - Root UIView container
├── CometViewController.cs (122 lines) - UIViewController wrapper
├── CUINavigationController.cs (37 lines) - UINavigationController customization
├── CUITabView.cs - UITabBarController for tab navigation
├── CUITableView.cs - UITableView for list display
├── CUIScrollView.cs - UIScrollView customization
├── CUIRadioButton.cs - Custom radio button implementation
├── CUIShapeView.cs - Shape rendering
├── CUIContainerView.cs - Generic UIView container
├── CUITableViewSource.cs - UITableViewSource for data binding
├── CUITableViewCell.cs - UITableViewCell customization
├── CUITapGestures.cs - Tap gesture handling
├── iOSExtensions.cs - iOS-specific extension methods
└── HandlerExtensions.cs - Handler integration for iOS
```

#### Android Implementation (12 files)
```
Android/
├── CometView.cs (119 lines) - Root ViewGroup container
├── CometFragment.cs (92 lines) - Fragment wrapper for views
├── ModalManager.cs (91 lines) - DialogFragment modal management
├── CometTabView.cs (88 lines) - BottomNavigationView for tabs
├── CometRecyclerView.cs - RecyclerView for lists
├── CometRecyclerViewAdapter.cs - RecyclerView data adapter
├── CometRecyclerViewHolder.cs - ViewHolder for cells
├── CometScrollView.cs - ScrollView customization
├── CometNavigationView.cs - Navigation management
├── CustomFrameLayout.cs - Custom FrameLayout base
├── CometTouchGesterListener.cs - Touch/gesture listener
└── HandlerExtensions.cs - Handler integration for Android
```

#### Windows Implementation (3 files)
```
Windows/
├── CometView.cs (106 lines) - Root Grid (WinUI) container
├── ListCell.cs (87 lines) - Grid-based list cell
└── HandlerExtensions.cs - Handler integration for Windows
```

#### Standard/Shared (1 file)
```
Standard/
└── HandlerExtensions.cs - Cross-platform handler extensions
```

---

## Handler Directory

**Location:** `src/Comet/Handlers/`

### CometHost Handlers
- **CometHostHandler.cs** (shared) - PropertyMapper definition
- **CometHostHandler.iOS.cs** (143 lines) - UIView container
- **CometHostHandler.Android.cs** (114 lines) - FrameLayout container
- **CometHostHandler.Windows.cs** (112 lines) - Canvas container

### CometView Handlers
- **CometViewHandler.iOS.cs** (49 lines) - UIView with UIViewController
- **CometViewHandler.Android.cs** (11 lines) - Empty (uses shared)
- **CometViewHandler.Windows.cs** (57 lines) - WinUI Grid

### Other Handlers (3 each: iOS, Android, Windows)
- NavigationViewHandler
- MauiViewHostHandler
- NativeHostHandler
- ListViewHandler
- TabViewHandler
- CollectionViewHandler
- ScrollViewHandler
- RadioButtonHandler
- ShapeViewHandler

**Total:** 33 platform-specific handler files

---

## Key Implementation Files

### Application Integration
- **CometApp.cs** (107 lines)
  - Lines 19-24: iOS modal presentation setup
  - Lines 88-104: iOS PresentingViewController helper
  - Lines 25-28: Android modal manager setup
  
- **CometWindow.cs** (100+ lines)
  - Lines 19-23: Platform-specific DisplayScale setup
  - Android: Sets DisplayScale from DisplayMetrics.Density

### Handler Mappers
- **AppHostBuilderExtensions.cs** (1151 lines)
  - 20+ conditional compilation sections
  - Platform-specific property mappers
  - Event handler registration patterns
  - Styling and color application per platform

### Platform Extensions
- **PlatformExtensions.cs** (Helpers)
  - OnPlatform<T> with iOS/Android/Windows/MacCatalyst properties
  - OnIdiom<T> with Phone/Tablet/Desktop variants
  - Compile-time platform detection

---

## Core Architecture Patterns

### View Hierarchy
```
iOS:       Android:      Windows:
CometView  CometView     CometView
(UIView)   (ViewGroup)   (Grid)
   ↓          ↓            ↓
Content    Content       Content
View       View          View
(Native)   (Native)      (Native)
```

### Handler Pattern
```
Handler.cs (PropertyMapper definition)
├── Handler.iOS.cs (UIView-based)
├── Handler.Android.cs (ViewGroup-based)
└── Handler.Windows.cs (WinUI-based)
```

### Conditional Compilation
```csharp
#if __IOS__ || MACCATALYST
    // iOS + Mac Catalyst code
#elif ANDROID
    // Android code
#elif WINDOWS
    // WinUI code
#elif MACCATALYST
    // Mac Catalyst specific
#endif
```

---

## Method Signature Reference

### CometView Root Classes

**iOS CometView:**
```csharp
public class CometView : UIView, IReloadHandler
public CometView(IMauiContext mauiContext)
public CometView(CGRect rect, IMauiContext mauiContext)
public override void LayoutSubviews()
public void Reload()
void SetView(IView view, bool forceRefresh = false)
```

**Android CometView:**
```csharp
public class CometView : ViewGroup, IReloadHandler
public CometView(IMauiContext mc)
protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
void SetView(IView view, bool forceRefresh = false)
```

**Windows CometView:**
```csharp
public class CometView : Grid, IReloadHandler
public CometView(IMauiContext mauiContext)
protected override Microsoft.UI.Xaml.Size MeasureOverride(Microsoft.UI.Xaml.Size availableSize)
protected override Microsoft.UI.Xaml.Size ArrangeOverride(Microsoft.UI.Xaml.Size finalSize)
void SetView(IView view, bool forceRefresh = false)
```

---

## Feature Mapping by Platform

### Modal Dialogs
| Feature | iOS | Android | Windows |
|---------|-----|---------|---------|
| Presentation | UIViewController.PresentViewController | DialogFragment.Show | N/A |
| Dismissal | DismissViewController | DialogFragment transaction | N/A |
| Stack Management | ViewController stack | Fragment transaction stack | N/A |
| Manager Class | CometApp | ModalManager | N/A |

### Tab Navigation
| Feature | iOS | Android | Windows |
|---------|-----|---------|---------|
| Container | UITabBarController | BottomNavigationView | N/A |
| Implementation | CUITabView (TabViewHandler) | CometTabView | N/A |
| Fragments | UIViewController | CometFragment | N/A |

### List Views
| Feature | iOS | Android | Windows |
|---------|-----|---------|---------|
| Base Class | UITableView | RecyclerView | DataGrid/ItemsControl |
| Source | UITableViewSource | RecyclerViewAdapter | ItemsSource |
| Cells | UITableViewCell | ViewHolder | DataTemplate |

### Navigation
| Feature | iOS | Android | Windows |
|---------|-----|---------|---------|
| Controller | UINavigationController | FragmentManager | Frame |
| ViewController | CometViewController | CometFragment | ContentFrame |
| Back Button | PopViewController | Transaction pop | Frame.GoBack |

---

## Conditional Compilation Summary

**Total Conditional Regions:** 40+

**Main Locations:**
1. AppHostBuilderExtensions.cs (20+ regions, ~800 lines)
2. CometApp.cs (2 regions)
3. CometWindow.cs (1 region)
4. PlatformExtensions.cs (4 regions in GetValue methods)

**Symbols Used:**
- `#if __IOS__` - iOS specific
- `#if __ANDROID__` - Android specific
- `#if WINDOWS` - Windows/WinUI specific
- `#if MACCATALYST` - Mac Catalyst specific
- `#if __IOS__ || MACCATALYST` - iOS and Mac Catalyst shared
- `#if IOS`, `#if ANDROID`, `#if WINDOWS` - In OnPlatform<T>

---

## Key Patterns Explained

### 1. Handler Reuse Pattern (CometView)
```csharp
// If view type is compatible, reuse handler to avoid recreation
if (view is View v && _view is View pv &&
    v.GetContentTypeHashCode() == pv.GetContentTypeHashCode()
    && currentHandler != null)
{
    _view = view;
    v.ViewHandler = currentHandler;
    return;
}
```

### 2. Density Conversion (Android)
```csharp
// Convert Android measure spec to device-independent pixels
var deviceIndependentWidth = widthMeasureSpec.ToDouble(Context);
var nativeWidth = Context.ToPixels(size.Width);
```

### 3. Safe Layout Prevention (iOS)
```csharp
// Prevent re-entrant layout calls
bool _inLayout;
public override void LayoutSubviews()
{
    if (_inLayout) return;
    _inLayout = true;
    try { /* layout code */ }
    finally { _inLayout = false; }
}
```

### 4. Event Handler Caching (iOS)
```csharp
// Use ConditionalWeakTable to prevent duplicate subscriptions
if (_sliderValueChangedHandlers.TryGetValue(slider, out var oldHandler))
{
    slider.ValueChanged -= oldHandler;
    _sliderValueChangedHandlers.Remove(slider);
}
```

### 5. Platform Specific Styling (AppHostBuilderExtensions)
```csharp
#if __IOS__ || MACCATALYST
    // iOS: Use CALayer properties
    layer.ShadowOpacity = shadow.Opacity;
#elif ANDROID
    // Android: Use Elevation
    platformView.Elevation = shadow.Radius * density;
#endif
```

---

## Critical Code Locations

For each topic, find implementation at:

| Topic | iOS | Android | Windows |
|-------|-----|---------|---------|
| Root View | Platform/iOS/CometView.cs | Platform/Android/CometView.cs | Platform/Windows/CometView.cs |
| View Controller | Platform/iOS/CometViewController.cs | Platform/Android/CometFragment.cs | N/A |
| Handler | Handlers/*/.[iOS\|Android\|Windows].cs | Handlers/*/.[iOS\|Android\|Windows].cs | Handlers/*/Windows.cs |
| Tab View | Platform/iOS/CUITabView.cs | Platform/Android/CometTabView.cs | N/A |
| Modals | CometApp.cs (PresentingViewController) | Platform/Android/ModalManager.cs | N/A |
| Navigation | Platform/iOS/CUINavigationController.cs | CometFragment + FragmentManager | Standard Frame |

---

## Compilation Symbols Reference

In Visual Studio/Rider code analysis, these symbols are defined per target framework:

```xml
<!-- iOS target -->
<DefineConstants>__IOS__;IOS;</DefineConstants>

<!-- Android target -->
<DefineConstants>__ANDROID__;ANDROID;</DefineConstants>

<!-- Windows target -->
<DefineConstants>WINDOWS;</DefineConstants>

<!-- Mac Catalyst target -->
<DefineConstants>__IOS__;MACCATALYST;</DefineConstants>
```

---

## File Navigation Quick Links

**For iOS Implementation:**
1. Start: Platform/iOS/CometView.cs
2. Controller: Platform/iOS/CometViewController.cs
3. Navigation: Platform/iOS/CUINavigationController.cs
4. Handlers: Handlers/*/iOS.cs files

**For Android Implementation:**
1. Start: Platform/Android/CometView.cs
2. Fragment: Platform/Android/CometFragment.cs
3. Modals: Platform/Android/ModalManager.cs
4. Tabs: Platform/Android/CometTabView.cs
5. Handlers: Handlers/*/Android.cs files

**For Windows Implementation:**
1. Start: Platform/Windows/CometView.cs
2. List Cell: Platform/Windows/ListCell.cs
3. Handlers: Handlers/*Windows.cs files

**Cross-Platform:**
1. Setup: AppHostBuilderExtensions.cs
2. App: Maui/CometApp.cs
3. Window: Maui/CometWindow.cs
4. Helpers: Helpers/PlatformExtensions.cs

---

## Statistics

**Total Platform-Specific Files:** 42
- iOS: 26 files
- Android: 12 files
- Windows: 3 files
- Standard: 1 file

**Total Platform-Specific Handlers:** 33
- iOS: 11 handlers
- Android: 11 handlers
- Windows: 11 handlers

**Conditional Compilation Regions:** 40+
- Most in AppHostBuilderExtensions.cs (20+ regions)
- CometApp.cs (2 regions)
- CometWindow.cs (1 region)
- Various handler files

**Code Coverage:**
- iOS: ~3,000 lines of platform-specific code
- Android: ~2,500 lines of platform-specific code
- Windows: ~1,500 lines of platform-specific code
- Total: ~7,000+ lines of platform-specific implementations

