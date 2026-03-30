 += (s, e) => callback(iPicker.SelectedIndex);
#endif
```

13. **Image Aspect** (Lines 520-536):
```csharp
#if __IOS__ || MACCATALYST
    platformView.ContentMode = aspect switch { ... }
#elif ANDROID
    platformView.SetScaleType(aspect switch { ... })
#endif
```

14. **Editor Placeholder & Keyboard** (Lines 550-575):
```csharp
#if __IOS__ || MACCATALYST
    editor.ApplyKeyboard(keyboard);
#elif ANDROID
    editor.SetHintTextColor(ColorStateList(...))
#endif
```

15. **ProgressBar Colors** (Lines 586-602):
```csharp
#if __IOS__ || MACCATALYST
    platformView.ProgressTintColor
    platformView.TrackTintColor
#elif ANDROID
    platformView.ProgressTintList
    platformView.ProgressBackgroundTintList
#endif
```

16. **DatePicker Text Color** (Lines 631-636):
```csharp
#if __IOS__ || MACCATALYST
    platformView.TintColor = color.ToPlatform();
#elif ANDROID
    platformView.SetTextColor(color.ToPlatform());
#endif
```

17. **Button Styling** (Lines 647-681):
```csharp
#if __IOS__ || MACCATALYST
    platformView.Layer.CornerRadius
    platformView.Layer.BorderWidth
    platformView.Layer.BorderColor
    platformView.ClipsToBounds = true
#elif ANDROID
    var drawable = new GradientDrawable()
    drawable.SetCornerRadius()
    drawable.SetStroke()
#endif
```

18. **Entry Text Changed** (Lines 695-715):
```csharp
#if __IOS__ || MACCATALYST
    entry.EditingChanged += newHandler;
#elif ANDROID
    entry.AfterTextChanged += (s, e) => callback(entry.Text);
#endif
```

19. **Editor Text Changed** (Lines 729-749):
```csharp
#if __IOS__ || MACCATALYST
    editor.Changed += newHandler;
#elif ANDROID
    editor.AfterTextChanged += (s, e) => callback(editor.Text);
#endif
```

20. **SearchBar Text Changed** (Lines 763-**):
```csharp
#if __IOS__ || MACCATALYST
    // Uses TryGetValue with ConditionalWeakTable
#elif ANDROID
    // Different handler pattern
```

---

## 7. iOS-Specific Code Patterns

### CometApp.cs (Lines 19-24, 88-104)

```csharp
#if __IOS__
    public CometApp()
    {
        ModalView.PerformPresent = (o) => ThreadHelper.RunOnMainThread(()=> {
            var vc = PresentingViewController;
            vc?.PresentViewController(
                new Comet.iOS.CometViewController
                {
                    MauiContext = o.GetMauiContext(),
                    CurrentView = o
                }, true, null);
        });
        ModalView.PerformDismiss = () => ThreadHelper.RunOnMainThread(
            ()=> PresentingViewController?.DismissViewController(true, null));
    }
    
    internal static UIKit.UIViewController PresentingViewController
    {
        get
        {
            var window = UIKit.UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIKit.UIWindowScene>()
                .SelectMany(s => s.Windows)
                .FirstOrDefault(w => w.IsKeyWindow);
            var vc = window?.RootViewController;
            while (vc?.PresentedViewController != null)
                vc = vc.PresentedViewController;
            return vc;
        }
    }
#endif
```

**Key iOS features:**
- Modal presentation uses `UIViewController.PresentViewController`
- Modal dismissal via `DismissViewController`
- `PresentingViewController` walks up the presented view controller stack
- Finds key window from `UIApplication.SharedApplication.ConnectedScenes`
- Threading via `ThreadHelper.RunOnMainThread()`

### CometViewController.cs (Full implementation)

**Class definition** (Lines 10-36):
```csharp
public class CometViewController : UIViewController
{
    private CometView _containerView;
    private View _startingCurrentView;
    public IMauiContext MauiContext { get; set; }
    
    public View CurrentView
    {
        get => _containerView?.CurrentView as View ?? _startingCurrentView;
        set
        {
            if (_containerView != null)
                _containerView.CurrentView = value;
            else
                _startingCurrentView = value;
            Title = value?.GetTitle() ?? "";
        }
    }
}
```

**View Loading** (Lines 38-44):
```csharp
public override void LoadView()
{
    base.View = _containerView = new CometView(MauiContext);
    _containerView.CurrentView = _startingCurrentView;
    Title = _startingCurrentView?.GetTitle() ?? "";
    _startingCurrentView = null;
}
```

**Lifecycle Methods** (Lines 54-75):
```csharp
public override void ViewDidAppear(bool animated)
{
    base.ViewDidAppear(animated);
    CurrentView?.ViewDidAppear();
}

public override void ViewWillAppear(bool animated)
{
    base.ViewWillAppear(animated);
    ApplyStyle();
}

public override void ViewDidDisappear(bool animated)
{
    base.ViewDidDisappear(animated);
    CurrentView?.ViewDidDisappear();
    if (wasPopped)
    {
        CurrentView?.Dispose();
        CurrentView = null;
    }
}
```

**Style Application** (Lines 77-109):
```csharp
public void ApplyStyle()
{
    if (NavigationController == null)
        return;
    
    var barColor = CurrentView?.GetNavigationBackgroundColor()?.ToPlatform();
    var textColor = CurrentView?.GetNavigationTextColor()?.ToPlatform();
    
    // Fallback to view background
    if (barColor == null)
    {
        var bg = CurrentView?.GetBackground();
        if (bg is Microsoft.Maui.Graphics.SolidPaint solid && solid.Color != null)
            barColor = solid.Color.ToPlatform();
    }
    
    var appearance = new UINavigationBarAppearance();
    appearance.ConfigureWithOpaqueBackground();
    
    if (barColor != null)
        appearance.BackgroundColor = barColor;
    
    if (textColor != null)
    {
        appearance.TitleTextAttributes = new UIStringAttributes { ForegroundColor = textColor };
        NavigationController.NavigationBar.TintColor = textColor;
    }
    
    appearance.ShadowColor = UIColor.Clear;
    NavigationController.NavigationBar.StandardAppearance = appearance;
    NavigationController.NavigationBar.ScrollEdgeAppearance = appearance;
    NavigationController.NavigationBar.CompactAppearance = appearance;
}
```

---

## 8. Android-Specific Code Patterns

### CometApp.cs (Lines 25-28)

```csharp
#elif ANDROID
    ModalView.PerformPresent = Comet.Android.Controls.ModalManager.ShowModal;
    ModalView.PerformDismiss = Comet.Android.Controls.ModalManager.DismisModal;
#endif
```

### CometWindow.cs (Lines 20-23)

```csharp
#elif __ANDROID__
    DisplayScale = mauiContext?.Context?.Resources.DisplayMetrics.Density ?? 1;
#endif
```

**DisplayScale calculation:**
- Android: Gets density from `DisplayMetrics`
- iOS: Empty (no display scale adjustment needed)

### CometFragment.cs (Full implementation)

**Fragment initialization** (Lines 18-31):
```csharp
public class CometFragment : Fragment
{
    CometView containerView;
    IView startingCurrentView;
    
    public IMauiContext MauiContext { get; set; }
    
    public CometFragment() { }
    
    public CometFragment(IMauiContext mauiContext)
    {
        MauiContext = mauiContext;
    }
    
    public CometFragment(View view, IMauiContext mauiContext) : this(mauiContext)
    {
        this.CurrentView = view;
    }
}
```

**View Creation** (Lines 49-64):
```csharp
public override AView OnCreateView(LayoutInflater inflater,
    ViewGroup container,
    Bundle savedInstanceState)
{
    if (CurrentView == null && savedInstanceState != null)
    {
        var oldViewId = savedInstanceState.GetString(currentViewID);
        var oldView = Comet.Internal.Extensions.FindViewById(null, oldViewId);
        startingCurrentView = oldView;
        MauiContext = oldView.ViewHandler?.MauiContext;
    }
    
    containerView ??= new CometView(MauiContext);
    containerView.CurrentView = startingCurrentView;
    return containerView;
}
```

**State Persistence** (Lines 67-76):
```csharp
const string currentViewID = nameof(currentViewID);
public override void OnSaveInstanceState(Bundle outState)
{
    if (CurrentView != null)
    {
        string viewId = (CurrentView as View)?.Id;
        outState.PutString(currentViewID, viewId);
    }
    base.OnSaveInstanceState(outState);
}
```

**Cleanup** (Lines 78-89):
```csharp
public override void OnDestroy()
{
    if (containerView != null && containerView.CurrentView != null)
    {
        containerView.CurrentView = null;
    }
    base.OnDestroy();
    this.Dispose();
}
```

### ModalManager.cs (Full implementation)

**Static Management** (Lines 13-32):
```csharp
public class ModalManager
{
    static ViewModal currrentDialog;
    static List<View> currentDialogs = new List<View>();
    
    static FragmentManager FragmentManager(View view) 
        => (view.GetMauiContext().Context as AppCompatActivity)?.SupportFragmentManager;
    
    public static void ShowModal(View view)
    {
        var transaction = FragmentManager(view).BeginTransaction();
        if (currrentDialog != null)
            transaction.Remove(currrentDialog);
        transaction.AddToBackStack(null);
        
        var dialog = new ViewModal(view);
        currentDialogs.Add(dialog.HView);
        currrentDialog = dialog;
        dialog.Show(transaction, "dialog");
    }
}
```

**Modal Stack Management** (Lines 33-57):
```csharp
public static void DismisModal() => PerformDismiss(true);

static void PerformDismiss(bool removeCurrent = true)
{
    if (currrentDialog == null)
        return;
    var transaction = FragmentManager(currrentDialog.HView).BeginTransaction();
    
    if (removeCurrent)
    {
        transaction.Remove(currrentDialog);
        transaction.AddToBackStack(null);
    }
    
    currentDialogs.Remove(currrentDialog.HView);
    currrentDialog = null;
    var currentView = currentDialogs.LastOrDefault();
    if (currentView == null)
    {
        transaction.CommitAllowingStateLoss();
        return;
    }
    
    currrentDialog = new ViewModal(currentView);
    currrentDialog.Show(transaction, "dialog");
}
```

**ViewModal DialogFragment** (Lines 60-89):
```csharp
class ViewModal : DialogFragment
{
    public ViewModal(View view)
    {
        HView = view;
    }
    
    public View HView { get; }
    
    AView currentBuiltView;
    public override AView OnCreateView(LayoutInflater inflater,
        ViewGroup container,
        Bundle savedInstanceState) 
        => currentBuiltView = HView.ToContainerView(HView.GetMauiContext());
    
    public override void OnDestroy()
    {
        PerformDismiss(false);
        if (HView != null)
        {
            HView.ViewHandler = null;
        }
        if (currentBuiltView != null)
        {
            currentBuiltView?.Dispose();
            currentBuiltView = null;
        }
        base.OnDestroy();
        this.Dispose();
    }
}
```

**Key Android features:**
- Uses `DialogFragment` for modals
- Maintains modal stack via `List<View>`
- Fragment transactions with `AddToBackStack`
- State persistence via Bundle
- `DisplayScale` from DisplayMetrics density

### CometTabView.cs (Lines 1-87)

**Initialization** (Lines 14-37):
```csharp
public class CometTabView : CustomFrameLayout
{
    private readonly BottomNavigationView _bottomNavigationView;
    private List<CometFragment> _fragments;
    public IMauiContext MauiContext { get; set; }
    
    public CometTabView(IMauiContext context) : base(context.Context)
    {
        MauiContext = context;
        _bottomNavigationView = new BottomNavigationView(context.Context)
        {
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.Bottom
            }
        };
        
        var val = new TypedValue();
        context.Context.Theme.ResolveAttribute(
            global::Android.Resource.Attribute.ColorBackground, val, true);
        _bottomNavigationView.SetBackgroundColor(
            new global::Android.Graphics.Color(val.Data));
        
        _bottomNavigationView.ItemSelected += HandleNavigationItemSelected;
        AddView(_bottomNavigationView);
    }
}
```

**Tab Creation** (Lines 39-61):
```csharp
public void CreateTabs(IList<View> views)
{
    _fragments = views.Select(v => new CometFragment(v, MauiContext)).ToList();
    _bottomNavigationView.Menu.Clear();
    
    if (views == null)
        return;
    
    for (int i = 0; i < views.Count(); i++)
    {
        var view = views[i];
        var title = view.GetEnvironment<string>(EnvironmentKeys.TabView.Title);
        var imagePath = view.GetEnvironment<string>(EnvironmentKeys.TabView.Image);
        
        _bottomNavigationView.Menu.Add(0, i, i, title);
    }
}
```

**Fragment Navigation** (Lines 62-86):
```csharp
protected override void OnAttachedToWindow()
{
    base.OnAttachedToWindow();
    var index = 0;
    MauiContext.GetFragmentManager()
        .BeginTransaction()
        .Add(Id, _fragments[index], index.ToString())
        .Show(_fragments[index])
        .Commit();
}

private void HandleNavigationItemSelected(object sender, 
    Google.Android.Material.Navigation.NavigationBarView.ItemSelectedEventArgs e)
{
    var index = e.Item.ItemId;
    var manager = MauiContext.GetFragmentManager();
    var transaction = manager.BeginTransaction();
    
    if (manager.FindFragmentByTag(index.ToString()) == null)
    {
        transaction.Add(Id, _fragments[index], index.ToString());
    }
    
    transaction.Hide(_fragments[_bottomNavigationView.SelectedItemId]);
    transaction.Show(_fragments[index]);
    transaction.Commit();
}
```

---

## 9. Windows-Specific Code Patterns

### CometView.cs (Lines 1-106)

**WinUI Imports:**
```csharp
using Microsoft.Maui;
using Microsoft.Maui.HotReload;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml.Controls;
```

**Grid-based container** (Lines 9-21):
```csharp
public class CometView : Grid, IReloadHandler
{
    IView _view;
    IViewHandler currentHandler;
    UIElement currentPlatformView;
    IMauiContext MauiContext;
    
    public CometView(IMauiContext mauiContext)
    {
        MauiContext = mauiContext;
        Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
            Microsoft.UI.Colors.White);
    }
}
```

**Measurement/Arrangement** (Lines 77-102):
```csharp
protected override Microsoft.UI.Xaml.Size MeasureOverride(
    Microsoft.UI.Xaml.Size availableSize)
{
    if (_view == null)
        return availableSize;
    
    var width = availableSize.Width > 0 ? availableSize.Width : 1000;
    var height = availableSize.Height > 0 ? availableSize.Height : 1000;
    
    var size = _view.Measure(width, height);
    return new Microsoft.UI.Xaml.Size(size.Width, size.Height);
}

protected override Microsoft.UI.Xaml.Size ArrangeOverride(
    Microsoft.UI.Xaml.Size finalSize)
{
    if (_view == null)
        return finalSize;
    
    _view.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
    
    if (currentPlatformView != null)
    {
        currentPlatformView.Arrange(
            new Microsoft.UI.Xaml.Rect(0, 0, finalSize.Width, finalSize.Height));
    }
    
    return finalSize;
}
```

### ListCell.cs (Lines 1-86)

**WinUI Grid-based cell** (Lines 14-24):
```csharp
class ListCell : WGrid
{
    private View _view;
    private UIElement _nativeView;
    private IElementHandler _handler;
    
    public ListCell(View view, IMauiContext context)
    {
        Context = context;
        View = view;
    }
}
```

**Measurement/Arrangement** (Lines 72-84):
```csharp
protected override UwpSize MeasureOverride(UwpSize availableSize)
{
    var measuredSize = _view?.Measure(availableSize.Width, availableSize.Height).ToPlatform();
    return measuredSize ?? availableSize;
}

protected override UwpSize ArrangeOverride(UwpSize finalSize)
{
    if (finalSize.Width > 0 && finalSize.Height > 0 && _view != null)
        _view.Frame = new RectF(0, 0, (float)finalSize.Width, (float)finalSize.Height);
    
    return finalSize;
}
```

---

## 10. Platform-Specific Service Registration

### AppHostBuilderExtensions.cs Handler Mapper Pattern

**iOS/MacCatalyst Weak Table Event Handler Caching** (Lines 27-33):
```csharp
#if __IOS__ || MACCATALYST
static readonly ConditionalWeakTable<UIKit.UITextField, EventHandler> _pickerEditingDidEndHandlers = new();
static readonly ConditionalWeakTable<UIKit.UITextField, EventHandler> _entryEditingChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UITextView, EventHandler> _editorChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISearchBar, EventHandler<UIKit.UISearchBarTextChangedEventArgs>> _searchBarT extChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISlider, EventHandler> _sliderValueChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISwitch, EventHandler> _switchValueChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UIButton, EventHandler> _checkBoxCheckedHandlers = new();
```

**Purpose:** Prevents duplicate event handler subscriptions during mapper re-fires.

**Android Placeholder Weak Tables** (Lines 35-42):
```csharp
#elif ANDROID
static readonly ConditionalWeakTable<object, object> _pickerTextChangedHandlers = new();
// ... other handlers use object type as placeholder
```

---

## 11. Safe Area & Status Bar Handling

### Safe Area Extensions

**File:** `src/Comet/Helpers/LayoutExtensions.cs`

```csharp
public static T IgnoreSafeArea<T>(this T view) where T : View
{
    view.SetEnvironment(EnvironmentKeys.Layout.IgnoreSafeArea, true, false);
    return view;
}

public static bool GetIgnoreSafeArea(this View view, bool defaultValue) 
    => (bool?)view.GetEnvironment(view, EnvironmentKeys.Layout.IgnoreSafeArea, false) ?? defaultValue;
```

**Environment Key:**
```csharp
public const string IgnoreSafeArea = "Layout.IgnoreSafeArea";
```

**View Interface:**
```csharp
public class View : ContextualObject, IDisposable, IView, IHotReloadableView, 
    ISafeAreaView, IContentTypeHash, IAnimator, ITitledElement, IGestureView, IVisualTreeElement, IPadding
{
    bool ISafeAreaView.IgnoreSafeArea => this.GetIgnoreSafeArea(false);
}
```

**TabView iOS Commented Safe Area** (TabViewHandler.iOS.cs):
```csharp
//public override bool IgnoreSafeArea => VirtualView?.GetIgnoreSafeArea(true) ?? true;
```

---

## 12. Platform Extension Helpers

### PlatformExtensions.cs (Full implementation)

**OnPlatform Helper** (Lines 11-29):
```csharp
public static class OnPlatform
{
    public static T Value<T>(
        T defaultValue = default,
        T iOS = default,
        T android = default,
        T windows = default,
        T macCatalyst = default)
    {
        return new OnPlatform<T>
        {
            Default = defaultValue,
            iOS = iOS,
            Android = android,
            WinUI = windows,
            MacCatalyst = macCatalyst,
        };
    }
}
```

**OnPlatform<T> Implementation** (Lines 31-63):
```csharp
public class OnPlatform<T>
{
    public T Default { get; set; }
    public T iOS { get; set; }
    public T Android { get; set; }
    public T WinUI { get; set; }
    public T MacCatalyst { get; set; }
    
    public static implicit operator T(OnPlatform<T> value)
    {
        if (value == null)
            return default;
        return value.GetValue();
    }
    
    public T GetValue()
    {
#if IOS
        if (!EqualityComparer<T>.Default.Equals(iOS, default))
            return iOS;
#elif ANDROID
        if (!EqualityComparer<T>.Default.Equals(Android, default))
            return Android;
#elif WINDOWS
        if (!EqualityComparer<T>.Default.Equals(WinUI, default))
            return WinUI;
#elif MACCATALYST
        if (!EqualityComparer<T>.Default.Equals(MacCatalyst, default))
            return MacCatalyst;
#endif
        return Default;
    }
}
```

**OnIdiom Helper** (Lines 69-116):
```csharp
public static class OnIdiom
{
    public static T Value<T>(
        T defaultValue = default,
        T phone = default,
        T tablet = default,
        T desktop = default)
    {
        return new OnIdiom<T>
        {
            Default = defaultValue,
            Phone = phone,
            Tablet = tablet,
            Desktop = desktop,
        };
    }
}

public class OnIdiom<T>
{
    public T Default { get; set; }
    public T Phone { get; set; }
    public T Tablet { get; set; }
    public T Desktop { get; set; }
    
    public static implicit operator T(OnIdiom<T> value)
    {
        if (value == null)
            return default;
        return value.GetValue();
    }
    
    public T GetValue()
    {
        var idiom = DeviceInfo.Idiom;
        
        if (idiom == DeviceIdiom.Phone && !EqualityComparer<T>.Default.Equals(Phone, default))
            return Phone;
        
        if (idiom == DeviceIdiom.Tablet && !EqualityComparer<T>.Default.Equals(Tablet, default))
            return Tablet;
        
        if (idiom == DeviceIdiom.Desktop && !EqualityComparer<T>.Default.Equals(Desktop, default))
            return Desktop;
        
        return Default;
    }
}
```

---

## 13. Navigation & iOS Specifics

### CUINavigationController.cs

**Default Style Caching** (Lines 9-19):
```csharp
public class CUINavigationController : UINavigationController
{
    public static UIColor DefaultBarTintColor { get; private set; }
    public static UIColor DefaultTintColor { get; private set; }
    public static UIStringAttributes DefaultTitleTextAttributes { get; private set; }
    
    public CUINavigationController()
    {
        if (DefaultBarTintColor == null)
        {
            DefaultBarTintColor = NavigationBar.BarTintColor;
            DefaultTintColor = NavigationBar.TintColor;
            DefaultTitleTextAttributes = NavigationBar.TitleTextAttributes;
        }
        
        // Ensure safe area background
        View.BackgroundColor = UIColor.SystemBackground;
    }
}
```

**Back Button Handling** (Lines 24-35):
```csharp
public override UIViewController[] PopToRootViewController(bool animated)
{
    return base.PopToRootViewController(animated);
}

public override UIViewController PopViewController(bool animated)
{
    var vc = base.PopViewController(animated);
    var cometVC = vc as CometViewController;
    cometVC?.WasPopped();
    
    return vc;
}
```

---

## Summary of Key Patterns

| Aspect | iOS | Android | Windows |
|--------|-----|---------|---------|
| Root View Base | UIView | ViewGroup | Grid |
| Layout Method | LayoutSubviews() | OnMeasure/OnLayout | MeasureOverride/ArrangeOverride |
| Modal Host | UIViewController.Present | DialogFragment | N/A |
| Tab View | UITabBarController | BottomNavigationView | N/A |
| Density/Scale | None (1.0) | DisplayMetrics.Density | None (1.0) |
| Gesture System | UIGestureRecognizer | GestureDetector | Pointer/Manipulation |
| Navigation | UINavigationController | Fragment Stack | Frame Navigation |
| Color Format | UIColor | int (Android color) | WinUI Brush |
| Safe Area | ISafeAreaView | WindowInsets | N/A |

EOF
cat /tmp/comet_platform_summary.md

                EC=$?
                echo "___BEGIN___COMMAND_DONE_MARKER___$EC"
            }
___BEGIN___COMMAND_OUTPUT_MARKER___
# Comet Platform-Specific Code Documentation

## Overview
Comet is a cross-platform .NET MAUI framework with platform-specific implementations for iOS, Android, Windows (WinUI), and Mac Catalyst. The codebase uses file naming conventions and conditional compilation to organize platform code.

---

## 1. Directory Build Targets & File Inclusion Pattern

**File:** `Directory.Build.targets`

Platform-specific file exclusion/inclusion strategy:
- iOS files: `*.iOS.cs` and `iOS/**/*.cs`
- Android files: `*.Android.cs` and `Android/**/*.cs`
- Windows files: `*.Windows.cs`, `Windows/**/*.cs`, `*.uwp.cs`
- Mac files: `*.Mac.cs`, `MacCatalyst/**/*.cs`, `*.MaciOS.cs`
- Mac Catalyst: `*.MacCatalyst.cs`, `MacCatalyst/**/*.cs`

Files are conditionally compiled based on target framework:
- iOS: `Xamarin.iOS` or contains `-ios` in target framework
- Android: `MonoAndroid` or contains `-android`
- Windows: contains `-windows` in target framework
- Mac Catalyst: contains `-maccatalyst` in target framework

---

## 2. Platform-Specific Root View Implementation

### iOS: `src/Comet/Platform/iOS/CometView.cs`

```csharp
namespace Comet.iOS
{
    public class CometView : UIView, IReloadHandler
    {
        // Properties
        bool _inLayout;
        IView _view;
        UIView currentPlatformView;
        IViewHandler currentHandler;
        
        public IMauiContext MauiContext { get; internal set; }
        
        public IView CurrentView
        {
            get => _view;
            set => SetView(value);
        }
        
        // Constructor
        public CometView(IMauiContext mauiContext) {
            MauiContext = mauiContext;
            BackgroundColor = UIColor.SystemBackground;
        }
        
        public CometView(CGRect rect, IMauiContext mauiContext) : base(rect) {
            MauiContext = mauiContext;
            BackgroundColor = UIColor.SystemBackground;
        }
        
        // Core Methods
        void SetView(IView view, bool forceRefresh = false)
        {
            // Reuses handlers for compatible view types
            // Prevents circular handler references
            // Invalidates measurement on view changes for rotation support
        }
        
        public override void LayoutSubviews()
        {
            // Lines 73-99
            // Prevents re-entrant layout calls via _inLayout flag
            // Invalidates measurement (critical for device rotation)
            // Measures and arranges view tree
            // Sets platform view frame and triggers layout
        }
        
        public void Reload() => SetView(CurrentView, true);
    }
}
```

**Key iOS-specific features:**
- Uses `UIView` as base class
- `BackgroundColor = UIColor.SystemBackground` for system background
- `LayoutSubviews()` override for iOS layout system
- Handles device rotation via measurement invalidation
- `CGRect` constructor for frame-based initialization

---

### Android: `src/Comet/Platform/Android/CometView.cs`

```csharp
namespace Comet.Android
{
    public class CometView : AViewGroup, IReloadHandler
    {
        // Properties
        IView _view;
        IViewHandler currentHandler;
        AView currentPlatformView;
        private bool inLayout;
        IMauiContext MauiContext;
        
        // Constructor
        public CometView(IMauiContext mc) : base(mc.Context) {
            MauiContext = mc;
        }
        
        public IView CurrentView
        {
            get => _view;
            set => SetView(value);
        }
        
        // Core Methods
        void SetView(IView view, bool forceRefresh = false)
        {
            // Handles IReplaceableView for view replacement scenarios
            // Manages view parent hierarchy
        }
        
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // Lines 74-82
            // Converts Android measure specs to device-independent pixels
            // Uses context DisplayMetrics.Density for conversion
            // Measures Comet view and sets measured dimensions
        }
        
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            // Lines 84-98
            // Prevents re-entrant layout via inLayout flag
            // Calculates size using DisplayScale
            // Arranges view hierarchy
        }
        
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            // Lines 99-109
            // Alternative size notification mechanism
            // Arranges view when size changes
        }
    }
}
```

**Key Android-specific features:**
- Uses `ViewGroup` (AViewGroup) as base class
- `OnMeasure()` for Android measurement system
- `OnLayout()` for Android layout positioning
- Density conversion via `DisplayMetrics.Density`
- Handles `IReplaceableView` for view replacement patterns
- `OnSizeChanged()` for size notification

---

### Windows: `src/Comet/Platform/Windows/CometView.cs`

```csharp
namespace Comet.Windows
{
    public class CometView : Grid, IReloadHandler
    {
        // Properties
        IView _view;
        IViewHandler currentHandler;
        UIElement currentPlatformView;
        IMauiContext MauiContext;
        
        // Constructor
        public CometView(IMauiContext mauiContext) {
            MauiContext = mauiContext;
            Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
        }
        
        public IView CurrentView
        {
            get => _view;
            set => SetView(value);
        }
        
        // Core Methods
        void SetView(IView view, bool forceRefresh = false)
        {
            // Similar handler reuse logic as iOS
            // Manages Children collection instead of Subviews
        }
        
        protected override Microsoft.UI.Xaml.Size MeasureOverride(Microsoft.UI.Xaml.Size availableSize)
        {
            // Lines 77-87
            // Uses XAML measurement override pattern
            // Provides default 1000x1000 size if unlimited
            // Returns measured size
        }
        
        protected override Microsoft.UI.Xaml.Size ArrangeOverride(Microsoft.UI.Xaml.Size finalSize)
        {
            // Lines 89-102
            // Arranges current platform view
            // Updates child element layout
        }
    }
}
```

**Key Windows-specific features:**
- Uses `Grid` (WinUI) as base class
- `MeasureOverride()` / `ArrangeOverride()` for WinUI layout system
- White background initialization
- `Children` collection management for WinUI
- Uses `Microsoft.UI.Xaml.Size` and `Microsoft.UI.Xaml.Rect`

---

## 3. Handler Implementations

All handlers follow the pattern: `<HandlerName>.cs` (shared), `<HandlerName>.iOS.cs`, `<HandlerName>.Android.cs`, `<HandlerName>.Windows.cs`

### CometHostHandler

**Shared:** `src/Comet/Handlers/CometHost/CometHostHandler.cs`
```csharp
public partial class CometHostHandler
{
    public static IPropertyMapper<CometHost, CometHostHandler> CometHostMapper =
        new PropertyMapper<CometHost, CometHostHandler>(ViewHandler.ViewMapper);
}
```

#### iOS Implementation: `CometHostHandler.iOS.cs` (Lines 1-143)

```csharp
public partial class CometHostHandler : ViewHandler<CometHost, CometHostHandler.CometHostContainerView>
{
    public CometHostHandler() : base(CometHostMapper) { }
    
    protected override CometHostContainerView CreatePlatformView()
        => new CometHostContainerView();
    
    public override Microsoft.Maui.Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        // Delegates to CometHost.CrossPlatformMeasure
        // Critical for CollectionView cells which must auto-size
        // Returns default 400x800 if unconstrained
    }
    
    void UpdateCometView()
    {
        // Gets render view (body content)
        // Avoids CometViewHandler handler circularity
        // Calls ToPlatform to convert to native view
        // Sets content on platform container
    }
    
    public class CometHostContainerView : UIView
    {
        UIView _contentView;
        IView _virtualView;
        Comet.View _rootCometView;
        IMauiContext _mauiContext;
        
        public void SetContent(UIView platformView, IView virtualView, Comet.View rootCometView = null, IMauiContext mauiContext = null)
        {
            // Lines 83-98
            // Sets AutoresizingMask to FlexibleWidth | FlexibleHeight
            // Adds subview
        }
        
        public override void LayoutSubviews()
        {
            // Lines 109-130
            // Re-resolves virtual view from root in case body was rebuilt
            // Prevents issues with disposed Grid references
            // Measures and arranges view tree
        }
        
        public override CGSize SizeThatFits(CGSize size)
        {
            // Returns measured size for intrinsic sizing
        }
    }
}
```

#### Android Implementation: `CometHostHandler.Android.cs` (Lines 1-114)

```csharp
public partial class CometHostHandler : ViewHandler<CometHost, CometHostHandler.CometHostContainerView>
{
    public CometHostHandler() : base(CometHostMapper) { }
    
    protected override CometHostContainerView CreatePlatformView()
        => new CometHostContainerView(Context);
    
    public class CometHostContainerView : FrameLayout
    {
        global::Android.Views.View _contentView;
        IView _virtualView;
        
        public void SetContent(global::Android.Views.View platformView, IView virtualView)
        {
            // Removes old view
            // Adds new view with MatchParent layout params
        }
        
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            // Lines 97-112
            // Calculates density-adjusted dimensions
            // Measures and arranges virtual view
        }
    }
}
```

#### Windows Implementation: `CometHostHandler.Windows.cs` (Lines 1-112)

```csharp
public partial class CometHostHandler : ViewHandler<CometHost, CometHostHandler.CometHostContainerPanel>
{
    protected override CometHostContainerPanel CreatePlatformView()
        => new CometHostContainerPanel();
    
    public class CometHostContainerPanel : Canvas
    {
        FrameworkElement _contentElement;
        IView _virtualView;
        
        public void SetContent(FrameworkElement element, IView virtualView)
        {
            // Removes old element from Children
            // Adds new element
        }
        
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            // Lines 92-100
            // Arranges content element and virtual view
        }
        
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            // Lines 102-110
            // Measures content element and virtual view
        }
    }
}
```

---

## 4. Platform-Specific Handler List

**iOS Handlers (11 files):**
- CometHostHandler.iOS.cs
- CometViewHandler.iOS.cs
- NavigationViewHandler.iOS.cs
- MauiViewHostHandler.iOS.cs
- NativeHostHandler.iOS.cs
- ListViewHandler.iOS.cs
- TabViewHandler.iOS.cs
- CollectionViewHandler.iOS.cs
- ScrollViewHandler.iOS.cs
- RadioButtonHandler.iOS.cs
- ShapeViewHandler.iOS.cs

**Android Handlers (11 files):**
- CometHostHandler.Android.cs
- CometViewHandler.Android.cs
- NavigationViewHandler.Android.cs
- MauiViewHostHandler.Android.cs
- NativeHostHandler.Android.cs
- ListViewHandler.Android.cs
- TabViewHandler.Android.cs
- CollectionViewHandler.Android.cs
- ScrollViewHandler.Android.cs
- RadioButtonHandler.Android.cs
- ShapeViewHandler.Android.cs

**Windows Handlers (11 files):**
- CometHostHandler.Windows.cs
- CometViewHandler.Windows.cs
- NavigationViewHandler.Windows.cs
- MauiViewHostHandler.Windows.cs
- NativeHostHandler.Windows.cs
- ListViewHandler.Windows.cs
- TabViewHandler.Windows.cs
- CollectionViewHandler.Windows.cs
- ScrollViewHandler.Windows.cs
- RadioButtonHandler.Windows.cs
- ShapeViewHandler.Windows.cs

---

## 5. CometView Handlers

### iOS: `CometViewHandler.iOS.cs` (Lines 1-49)

```csharp
public partial class CometViewHandler : ViewHandler<View, CometView>, IPlatformViewHandler
{
    public static PropertyMapper<View, CometViewHandler> CometViewMapper = new()
    {
        [nameof(ITitledElement.Title)] = MapTitle,
        [nameof(IView.Background)] = MapBackgroundColor,
    };
    
    CometViewController viewController;
    UIViewController IPlatformViewHandler.ViewController 
        => viewController ??= new CometViewController 
        { 
            ContainerView = this.PlatformView, 
            MauiContext = MauiContext 
        };
    
    protected override CometView CreatePlatformView() 
        => new CometView(MauiContext);
    
    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        PlatformView.CurrentView = view;
    }
    
    public static void MapTitle(CometViewHandler handler, View view)
    {
        var vc = handler?.viewController;
        if (vc == null)
            return;
        vc.Title = view.GetTitle() ?? "";
    }
    
    public static void MapBackgroundColor(CometViewHandler handler, View view)
    {
        var vc = handler?.viewController;
        if (vc == null)
            return;
        vc.View.UpdateBackground(view);
    }
}
```

**Key iOS-specific:**
- Implements `IPlatformViewHandler` for UIViewController access
- Creates CometViewController for view controller integration
- Maps Title to UIViewController.Title
- Maps Background via UIView extensions

### Android: `CometViewHandler.Android.cs` (Lines 1-11)

```csharp
public partial class CometViewHandler
{
    // Empty - Android uses shared implementation
}
```

### Windows: `CometViewHandler.Windows.cs` (Lines 1-57)

```csharp
public partial class CometViewHandler : ViewHandler<View, Comet.Windows.CometView>
{
    public static PropertyMapper<View, CometViewHandler> CometViewMapper = new()
    {
        [nameof(ITitledElement.Title)] = MapTitle,
        [nameof(IView.Background)] = MapBackgroundColor,
    };
    
    protected override Comet.Windows.CometView CreatePlatformView() 
        => new(MauiContext);
    
    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        PlatformView.CurrentView = view;
    }
    
    public static void MapTitle(CometViewHandler handler, View view)
    {
        // No-op - Windows Grid doesn't have direct title
    }
    
    public static void MapBackgroundColor(CometViewHandler handler, View view)
    {
        // Lines 35-55
        if (handler?.PlatformView == null)
            return;
        
        var background = view.Background;
        if (background is SolidPaint solid && solid.Color != null)
        {
            // Converts to WinUI SolidColorBrush with ARGB
        }
        else if (background == null)
        {
            // Sets white background
        }
    }
}
```

**Key Windows-specific:**
- MapTitle is no-op (Grid doesn't have title)
- Background uses WinUI `SolidColorBrush` with `ColorHelper.FromArgb`
- Color conversion: Alpha/Red/Green/Blue * 255

---

## 6. Conditional Compilation Directives

### Location: `src/Comet/AppHostBuilderExtensions.cs`

**Global Conditional Blocks:**

```csharp
#if WINDOWS                                          // Line 17
using Microsoft.Maui.Graphics.Win2D;
#endif

#if __IOS__ || MACCATALYST                          // Lines 27-33
static readonly ConditionalWeakTable<UIKit.UITextField, EventHandler> _pickerEditingDidEndHandlers = new();
static readonly ConditionalWeakTable<UIKit.UITextField, EventHandler> _entryEditingChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UITextView, EventHandler> _editorChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISearchBar, EventHandler<UIKit.UISearchBarTextChangedEventArgs>> _searchBarTextChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISlider, EventHandler> _sliderValueChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISwitch, EventHandler> _switchValueChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UIButton, EventHandler> _checkBoxCheckedHandlers = new();

#elif ANDROID                                        // Lines 35-42
static readonly ConditionalWeakTable<object, object> _pickerTextChangedHandlers = new();
static readonly ConditionalWeakTable<object, object> _entryTextChangedHandlers = new();
static readonly ConditionalWeakTable<object, object> _editorTextChangedHandlers = new();
static readonly ConditionalWeakTable<object, object> _searchBarTextChangedHandlers = new();
static readonly ConditionalWeakTable<object, object> _sliderValueChangedHandlers = new();
static readonly ConditionalWeakTable<object, object> _switchValueChangedHandlers = new();
static readonly ConditionalWeakTable<object, object> _checkBoxCheckedHandlers = new();
#endif
```

**Property Mapper Conditionals Examples:**

1. **Shadow Styling** (Lines 109-128):
```csharp
#if __IOS__ || MACCATALYST
    var platformView = handler.PlatformView as UIKit.UIView;
    // iOS: CALayer shadow properties
    layer.ShadowOpacity, layer.ShadowRadius, layer.ShadowOffset, layer.ShadowColor
#elif ANDROID
    var platformView = handler.PlatformView as global::Android.Views.View;
    // Android: Elevation property
    platformView.Elevation
#endif
```

2. **Border Styling** (Lines 140-179):
```csharp
#if __IOS__ || MACCATALYST
    // iOS: CALayer border + corner radius
    layer.CornerRadius, layer.BorderColor, layer.BorderWidth
#elif ANDROID
    // Android: GradientDrawable
    drawable.SetCornerRadius(), drawable.SetStroke(), drawable.SetColor()
#endif
```

3. **Entry Placeholder Color** (Lines 193-201):
```csharp
#if __IOS__ || MACCATALYST
    entry.AttributedPlaceholder = new Foundation.NSAttributedString(...)
#elif ANDROID
    entry.SetHintTextColor(new global::Android.Content.Res.ColorStateList(...))
#endif
```

4. **Keyboard Configuration** (Lines 215-221):
```csharp
#if __IOS__ || MACCATALYST
    entry.ApplyKeyboard(keyboard);
#elif ANDROID
    EntryHandler.MapKeyboard(handler, entryView);
#endif
```

5. **Switch Colors** (Lines 256-274):
```csharp
#if __IOS__ || MACCATALYST
    platformView.OnTintColor = onColor.ToPlatform();
    platformView.ThumbTintColor = thumbColor.ToPlatform();
#elif ANDROID
    platformView.TrackTintList = new ColorStateList(...)
    platformView.ThumbTintList = new ColorStateList(...)
#endif
```

6. **Slider Configuration** (Lines 285-304):
```csharp
#if __IOS__ || MACCATALYST
    platformView.MinimumTrackTintColor
    platformView.MaximumTrackTintColor
    platformView.ThumbTintColor
#elif ANDROID
    platformView.ProgressTintList
    platformView.ThumbTintList
#endif
```

7. **Slider Value Changed** (Lines 318-347):
```csharp
#if __IOS__ || MACCATALYST
    slider.ValueChanged += (s, e) => callback(slider.Value);
#elif ANDROID
    slider.ProgressChanged += (s, e) => callback(value);
#endif
```

8. **Slider Tracking Prevention** (Lines 353-357):
```csharp
#if __IOS__ || MACCATALYST
    if (handler.PlatformView is UIKit.UISlider uiSlider && uiSlider.Tracking)
        return;  // Skip value mapping during drag
#endif
```

9. **Switch Toggled** (Lines 371-396):
```csharp
#if __IOS__ || MACCATALYST
    platformView.ValueChanged += (s, e) => callback(platformView.On);
#elif ANDROID
    platformView.CheckedChange += (s, e) => callback(e.IsChecked);
#endif
```

10. **CheckBox Checked** (Lines 410-435):
```csharp
#if __IOS__ || MACCATALYST
    platformView.CheckedChanged += (s, e) => callback(platformView.IsChecked);
#elif ANDROID
    platformView.CheckedChange += (s, e) => callback(e.IsChecked);
#endif
```

11. **Stepper Value Changed** (Lines 449-458):
```csharp
#if __IOS__ || MACCATALYST
    platformView.ValueChanged += (s, e) => callback(platformView.Value);
#elif ANDROID
    // TODO: Wire up button click events
#endif
```

12. **Picker Selected Index** (Lines 472-506):
```csharp
#if __IOS__ || MACCATALYST
    picker.EditingDidEnd += newHandler;
#elif ANDROID
    picker.AfterTextChanged += (s, e) => callback(iPicker.SelectedIndex);
#endif
```

13. **Image Aspect** (Lines 520-536):
```csharp
#if __IOS__ || MACCATALYST
    platformView.ContentMode = aspect switch { ... }
#elif ANDROID
    platformView.SetScaleType(aspect switch { ... })
#endif
```

14. **Editor Placeholder & Keyboard** (Lines 550-575):
```csharp
#if __IOS__ || MACCATALYST
    editor.ApplyKeyboard(keyboard);
#elif ANDROID
    editor.SetHintTextColor(ColorStateList(...))
#endif
```

15. **ProgressBar Colors** (Lines 586-602):
```csharp
#if __IOS__ || MACCATALYST
    platformView.ProgressTintColor
    platformView.TrackTintColor
#elif ANDROID
    platformView.ProgressTintList
    platformView.ProgressBackgroundTintList
#endif
```

16. **DatePicker Text Color** (Lines 631-636):
```csharp
#if __IOS__ || MACCATALYST
    platformView.TintColor = color.ToPlatform();
#elif ANDROID
    platformView.SetTextColor(color.ToPlatform());
#endif
```

17. **Button Styling** (Lines 647-681):
```csharp
#if __IOS__ || MACCATALYST
    platformView.Layer.CornerRadius
    platformView.Layer.BorderWidth
    platformView.Layer.BorderColor
    platformView.ClipsToBounds = true
#elif ANDROID
    var drawable = new GradientDrawable()
    drawable.SetCornerRadius()
    drawable.SetStroke()
#endif
```

18. **Entry Text Changed** (Lines 695-715):
```csharp
#if __IOS__ || MACCATALYST
    entry.EditingChanged += newHandler;
#elif ANDROID
    entry.AfterTextChanged += (s, e) => callback(entry.Text);
#endif
```

19. **Editor Text Changed** (Lines 729-749):
```csharp
#if __IOS__ || MACCATALYST
    editor.Changed += newHandler;
#elif ANDROID
    editor.AfterTextChanged += (s, e) => callback(editor.Text);
#endif
```

20. **SearchBar Text Changed** (Lines 763-**):
```csharp
#if __IOS__ || MACCATALYST
    // Uses TryGetValue with ConditionalWeakTable
#elif ANDROID
    // Different handler pattern
```

---

## 7. iOS-Specific Code Patterns

### CometApp.cs (Lines 19-24, 88-104)

```csharp
#if __IOS__
    public CometApp()
    {
        ModalView.PerformPresent = (o) => ThreadHelper.RunOnMainThread(()=> {
            var vc = PresentingViewController;
            vc?.PresentViewController(
                new Comet.iOS.CometViewController
                {
                    MauiContext = o.GetMauiContext(),
                    CurrentView = o
                }, true, null);
        });
        ModalView.PerformDismiss = () => ThreadHelper.RunOnMainThread(
            ()=> PresentingViewController?.DismissViewController(true, null));
    }
    
    internal static UIKit.UIViewController PresentingViewController
    {
        get
        {
            var window = UIKit.UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIKit.UIWindowScene>()
                .SelectMany(s => s.Windows)
                .FirstOrDefault(w => w.IsKeyWindow);
            var vc = window?.RootViewController;
            while (vc?.PresentedViewController != null)
                vc = vc.PresentedViewController;
            return vc;
        }
    }
#endif
```

**Key iOS features:**
- Modal presentation uses `UIViewController.PresentViewController`
- Modal dismissal via `DismissViewController`
- `PresentingViewController` walks up the presented view controller stack
- Finds key window from `UIApplication.SharedApplication.ConnectedScenes`
- Threading via `ThreadHelper.RunOnMainThread()`

### CometViewController.cs (Full implementation)

**Class definition** (Lines 10-36):
```csharp
public class CometViewController : UIViewController
{
    private CometView _containerView;
    private View _startingCurrentView;
    public IMauiContext MauiContext { get; set; }
    
    public View CurrentView
    {
        get => _containerView?.CurrentView as View ?? _startingCurrentView;
        set
        {
            if (_containerView != null)
                _containerView.CurrentView = value;
            else
                _startingCurrentView = value;
            Title = value?.GetTitle() ?? "";
        }
    }
}
```

**View Loading** (Lines 38-44):
```csharp
public override void LoadView()
{
    base.View = _containerView = new CometView(MauiContext);
    _containerView.CurrentView = _startingCurrentView;
    Title = _startingCurrentView?.GetTitle() ?? "";
    _startingCurrentView = null;
}
```

**Lifecycle Methods** (Lines 54-75):
```csharp
public override void ViewDidAppear(bool animated)
{
    base.ViewDidAppear(animated);
    CurrentView?.ViewDidAppear();
}

public override void ViewWillAppear(bool animated)
{
    base.ViewWillAppear(animated);
    ApplyStyle();
}

public override void ViewDidDisappear(bool animated)
{
    base.ViewDidDisappear(animated);
    CurrentView?.ViewDidDisappear();
    if (wasPopped)
    {
        CurrentView?.Dispose();
        CurrentView = null;
    }
}
```

**Style Application** (Lines 77-109):
```csharp
public void ApplyStyle()
{
    if (NavigationController == null)
        return;
    
    var barColor = CurrentView?.GetNavigationBackgroundColor()?.ToPlatform();
    var textColor = CurrentView?.GetNavigationTextColor()?.ToPlatform();
    
    // Fallback to view background
    if (barColor == null)
    {
        var bg = CurrentView?.GetBackground();
        if (bg is Microsoft.Maui.Graphics.SolidPaint solid && solid.Color != null)
            barColor = solid.Color.ToPlatform();
    }
    
    var appearance = new UINavigationBarAppearance();
    appearance.ConfigureWithOpaqueBackground();
    
    if (barColor != null)
        appearance.BackgroundColor = barColor;
    
    if (textColor != null)
    {
        appearance.TitleTextAttributes = new UIStringAttributes { ForegroundColor = textColor };
        NavigationController.NavigationBar.TintColor = textColor;
    }
    
    appearance.ShadowColor = UIColor.Clear;
    NavigationController.NavigationBar.StandardAppearance = appearance;
    NavigationController.NavigationBar.ScrollEdgeAppearance = appearance;
    NavigationController.NavigationBar.CompactAppearance = appearance;
}
```

---

## 8. Android-Specific Code Patterns

### CometApp.cs (Lines 25-28)

```csharp
#elif ANDROID
    ModalView.PerformPresent = Comet.Android.Controls.ModalManager.ShowModal;
    ModalView.PerformDismiss = Comet.Android.Controls.ModalManager.DismisModal;
#endif
```

### CometWindow.cs (Lines 20-23)

```csharp
#elif __ANDROID__
    DisplayScale = mauiContext?.Context?.Resources.DisplayMetrics.Density ?? 1;
#endif
```

**DisplayScale calculation:**
- Android: Gets density from `DisplayMetrics`
- iOS: Empty (no display scale adjustment needed)

### CometFragment.cs (Full implementation)

**Fragment initialization** (Lines 18-31):
```csharp
public class CometFragment : Fragment
{
    CometView containerView;
    IView startingCurrentView;
    
    public IMauiContext MauiContext { get; set; }
    
    public CometFragment() { }
    
    public CometFragment(IMauiContext mauiContext)
    {
        MauiContext = mauiContext;
    }
    
    public CometFragment(View view, IMauiContext mauiContext) : this(mauiContext)
    {
        this.CurrentView = view;
    }
}
```

**View Creation** (Lines 49-64):
```csharp
public override AView OnCreateView(LayoutInflater inflater,
    ViewGroup container,
    Bundle savedInstanceState)
{
    if (CurrentView == null && savedInstanceState != null)
    {
        var oldViewId = savedInstanceState.GetString(currentViewID);
        var oldView = Comet.Internal.Extensions.FindViewById(null, oldViewId);
        startingCurrentView = oldView;
        MauiContext = oldView.ViewHandler?.MauiContext;
    }
    
    containerView ??= new CometView(MauiContext);
    containerView.CurrentView = startingCurrentView;
    return containerView;
}
```

**State Persistence** (Lines 67-76):
```csharp
const string currentViewID = nameof(currentViewID);
public override void OnSaveInstanceState(Bundle outState)
{
    if (CurrentView != null)
    {
        string viewId = (CurrentView as View)?.Id;
        outState.PutString(currentViewID, viewId);
    }
    base.OnSaveInstanceState(outState);
}
```

**Cleanup** (Lines 78-89):
```csharp
public override void OnDestroy()
{
    if (containerView != null && containerView.CurrentView != null)
    {
        containerView.CurrentView = null;
    }
    base.OnDestroy();
    this.Dispose();
}
```

### ModalManager.cs (Full implementation)

**Static Management** (Lines 13-32):
```csharp
public class ModalManager
{
    static ViewModal currrentDialog;
    static List<View> currentDialogs = new List<View>();
    
    static FragmentManager FragmentManager(View view) 
        => (view.GetMauiContext().Context as AppCompatActivity)?.SupportFragmentManager;
    
    public static void ShowModal(View view)
    {
        var transaction = FragmentManager(view).BeginTransaction();
        if (currrentDialog != null)
            transaction.Remove(currrentDialog);
        transaction.AddToBackStack(null);
        
        var dialog = new ViewModal(view);
        currentDialogs.Add(dialog.HView);
        currrentDialog = dialog;
        dialog.Show(transaction, "dialog");
    }
}
```

**Modal Stack Management** (Lines 33-57):
```csharp
public static void DismisModal() => PerformDismiss(true);

static void PerformDismiss(bool removeCurrent = true)
{
    if (currrentDialog == null)
        return;
    var transaction = FragmentManager(currrentDialog.HView).BeginTransaction();
    
    if (removeCurrent)
    {
        transaction.Remove(currrentDialog);
        transaction.AddToBackStack(null);
    }
    
    currentDialogs.Remove(currrentDialog.HView);
    currrentDialog = null;
    var currentView = currentDialogs.LastOrDefault();
    if (currentView == null)
    {
        transaction.CommitAllowingStateLoss();
        return;
    }
    
    currrentDialog = new ViewModal(currentView);
    currrentDialog.Show(transaction, "dialog");
}
```

**ViewModal DialogFragment** (Lines 60-89):
```csharp
class ViewModal : DialogFragment
{
    public ViewModal(View view)
    {
        HView = view;
    }
    
    public View HView { get; }
    
    AView currentBuiltView;
    public override AView OnCreateView(LayoutInflater inflater,
        ViewGroup container,
        Bundle savedInstanceState) 
        => currentBuiltView = HView.ToContainerView(HView.GetMauiContext());
    
    public override void OnDestroy()
    {
        PerformDismiss(false);
        if (HView != null)
        {
            HView.ViewHandler = null;
        }
        if (currentBuiltView != null)
        {
            currentBuiltView?.Dispose();
            currentBuiltView = null;
        }
        base.OnDestroy();
        this.Dispose();
    }
}
```

**Key Android features:**
- Uses `DialogFragment` for modals
- Maintains modal stack via `List<View>`
- Fragment transactions with `AddToBackStack`
- State persistence via Bundle
- `DisplayScale` from DisplayMetrics density

### CometTabView.cs (Lines 1-87)

**Initialization** (Lines 14-37):
```csharp
public class CometTabView : CustomFrameLayout
{
    private readonly BottomNavigationView _bottomNavigationView;
    private List<CometFragment> _fragments;
    public IMauiContext MauiContext { get; set; }
    
    public CometTabView(IMauiContext context) : base(context.Context)
    {
        MauiContext = context;
        _bottomNavigationView = new BottomNavigationView(context.Context)
        {
            LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.Bottom
            }
        };
        
        var val = new TypedValue();
        context.Context.Theme.ResolveAttribute(
            global::Android.Resource.Attribute.ColorBackground, val, true);
        _bottomNavigationView.SetBackgroundColor(
            new global::Android.Graphics.Color(val.Data));
        
        _bottomNavigationView.ItemSelected += HandleNavigationItemSelected;
        AddView(_bottomNavigationView);
    }
}
```

**Tab Creation** (Lines 39-61):
```csharp
public void CreateTabs(IList<View> views)
{
    _fragments = views.Select(v => new CometFragment(v, MauiContext)).ToList();
    _bottomNavigationView.Menu.Clear();
    
    if (views == null)
        return;
    
    for (int i = 0; i < views.Count(); i++)
    {
        var view = views[i];
        var title = view.GetEnvironment<string>(EnvironmentKeys.TabView.Title);
        var imagePath = view.GetEnvironment<string>(EnvironmentKeys.TabView.Image);
        
        _bottomNavigationView.Menu.Add(0, i, i, title);
    }
}
```

**Fragment Navigation** (Lines 62-86):
```csharp
protected override void OnAttachedToWindow()
{
    base.OnAttachedToWindow();
    var index = 0;
    MauiContext.GetFragmentManager()
        .BeginTransaction()
        .Add(Id, _fragments[index], index.ToString())
        .Show(_fragments[index])
        .Commit();
}

private void HandleNavigationItemSelected(object sender, 
    Google.Android.Material.Navigation.NavigationBarView.ItemSelectedEventArgs e)
{
    var index = e.Item.ItemId;
    var manager = MauiContext.GetFragmentManager();
    var transaction = manager.BeginTransaction();
    
    if (manager.FindFragmentByTag(index.ToString()) == null)
    {
        transaction.Add(Id, _fragments[index], index.ToString());
    }
    
    transaction.Hide(_fragments[_bottomNavigationView.SelectedItemId]);
    transaction.Show(_fragments[index]);
    transaction.Commit();
}
```

---

## 9. Windows-Specific Code Patterns

### CometView.cs (Lines 1-106)

**WinUI Imports:**
```csharp
using Microsoft.Maui;
using Microsoft.Maui.HotReload;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml.Controls;
```

**Grid-based container** (Lines 9-21):
```csharp
public class CometView : Grid, IReloadHandler
{
    IView _view;
    IViewHandler currentHandler;
    UIElement currentPlatformView;
    IMauiContext MauiContext;
    
    public CometView(IMauiContext mauiContext)
    {
        MauiContext = mauiContext;
        Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
            Microsoft.UI.Colors.White);
    }
}
```

**Measurement/Arrangement** (Lines 77-102):
```csharp
protected override Microsoft.UI.Xaml.Size MeasureOverride(
    Microsoft.UI.Xaml.Size availableSize)
{
    if (_view == null)
        return availableSize;
    
    var width = availableSize.Width > 0 ? availableSize.Width : 1000;
    var height = availableSize.Height > 0 ? availableSize.Height : 1000;
    
    var size = _view.Measure(width, height);
    return new Microsoft.UI.Xaml.Size(size.Width, size.Height);
}

protected override Microsoft.UI.Xaml.Size ArrangeOverride(
    Microsoft.UI.Xaml.Size finalSize)
{
    if (_view == null)
        return finalSize;
    
    _view.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
    
    if (currentPlatformView != null)
    {
        currentPlatformView.Arrange(
            new Microsoft.UI.Xaml.Rect(0, 0, finalSize.Width, finalSize.Height));
    }
    
    return finalSize;
}
```

### ListCell.cs (Lines 1-86)

**WinUI Grid-based cell** (Lines 14-24):
```csharp
class ListCell : WGrid
{
    private View _view;
    private UIElement _nativeView;
    private IElementHandler _handler;
    
    public ListCell(View view, IMauiContext context)
    {
        Context = context;
        View = view;
    }
}
```

**Measurement/Arrangement** (Lines 72-84):
```csharp
protected override UwpSize MeasureOverride(UwpSize availableSize)
{
    var measuredSize = _view?.Measure(availableSize.Width, availableSize.Height).ToPlatform();
    return measuredSize ?? availableSize;
}

protected override UwpSize ArrangeOverride(UwpSize finalSize)
{
    if (finalSize.Width > 0 && finalSize.Height > 0 && _view != null)
        _view.Frame = new RectF(0, 0, (float)finalSize.Width, (float)finalSize.Height);
    
    return finalSize;
}
```

---

## 10. Platform-Specific Service Registration

### AppHostBuilderExtensions.cs Handler Mapper Pattern

**iOS/MacCatalyst Weak Table Event Handler Caching** (Lines 27-33):
```csharp
#if __IOS__ || MACCATALYST
static readonly ConditionalWeakTable<UIKit.UITextField, EventHandler> _pickerEditingDidEndHandlers = new();
static readonly ConditionalWeakTable<UIKit.UITextField, EventHandler> _entryEditingChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UITextView, EventHandler> _editorChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISearchBar, EventHandler<UIKit.UISearchBarTextChangedEventArgs>> _searchBarTextChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISlider, EventHandler> _sliderValueChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UISwitch, EventHandler> _switchValueChangedHandlers = new();
static readonly ConditionalWeakTable<UIKit.UIButton, EventHandler> _checkBoxCheckedHandlers = new();
```

**Purpose:** Prevents duplicate event handler subscriptions during mapper re-fires.

**Android Placeholder Weak Tables** (Lines 35-42):
```csharp
#elif ANDROID
static readonly ConditionalWeakTable<object, object> _pickerTextChangedHandlers = new();
// ... other handlers use object type as placeholder
```

---

## 11. Safe Area & Status Bar Handling

### Safe Area Extensions

**File:** `src/Comet/Helpers/LayoutExtensions.cs`

```csharp
public static T IgnoreSafeArea<T>(this T view) where T : View
{
    view.SetEnvironment(EnvironmentKeys.Layout.IgnoreSafeArea, true, false);
    return view;
}

public static bool GetIgnoreSafeArea(this View view, bool defaultValue) 
    => (bool?)view.GetEnvironment(view, EnvironmentKeys.Layout.IgnoreSafeArea, false) ?? defaultValue;
```

**Environment Key:**
```csharp
public const string IgnoreSafeArea = "Layout.IgnoreSafeArea";
```

**View Interface:**
```csharp
public class View : ContextualObject, IDisposable, IView, IHotReloadableView, 
    ISafeAreaView, IContentTypeHash, IAnimator, ITitledElement, IGestureView, IVisualTreeElement, IPadding
{
    bool ISafeAreaView.IgnoreSafeArea => this.GetIgnoreSafeArea(false);
}
```

**TabView iOS Commented Safe Area** (TabViewHandler.iOS.cs):
```csharp
//public override bool IgnoreSafeArea => VirtualView?.GetIgnoreSafeArea(true) ?? true;
```

---

## 12. Platform Extension Helpers

### PlatformExtensions.cs (Full implementation)

**OnPlatform Helper** (Lines 11-29):
```csharp
public static class OnPlatform
{
    public static T Value<T>(
        T defaultValue = default,
        T iOS = default,
        T android = default,
        T windows = default,
        T macCatalyst = default)
    {
        return new OnPlatform<T>
        {
            Default = defaultValue,
            iOS = iOS,
            Android = android,
            WinUI = windows,
            MacCatalyst = macCatalyst,
        };
    }
}
```

**OnPlatform<T> Implementation** (Lines 31-63):
```csharp
public class OnPlatform<T>
{
    public T Default { get; set; }
    public T iOS { get; set; }
    public T Android { get; set; }
    public T WinUI { get; set; }
    public T MacCatalyst { get; set; }
    
    public static implicit operator T(OnPlatform<T> value)
    {
        if (value == null)
            return default;
        return value.GetValue();
    }
    
    public T GetValue()
    {
#if IOS
        if (!EqualityComparer<T>.Default.Equals(iOS, default))
            return iOS;
#elif ANDROID
        if (!EqualityComparer<T>.Default.Equals(Android, default))
            return Android;
#elif WINDOWS
        if (!EqualityComparer<T>.Default.Equals(WinUI, default))
            return WinUI;
#elif MACCATALYST
        if (!EqualityComparer<T>.Default.Equals(MacCatalyst, default))
            return MacCatalyst;
#endif
        return Default;
    }
}
```

**OnIdiom Helper** (Lines 69-116):
```csharp
public static class OnIdiom
{
    public static T Value<T>(
        T defaultValue = default,
        T phone = default,
        T tablet = default,
        T desktop = default)
    {
        return new OnIdiom<T>
        {
            Default = defaultValue,
            Phone = phone,
            Tablet = tablet,
            Desktop = desktop,
        };
    }
}

public class OnIdiom<T>
{
    public T Default { get; set; }
    public T Phone { get; set; }
    public T Tablet { get; set; }
    public T Desktop { get; set; }
    
    public static implicit operator T(OnIdiom<T> value)
    {
        if (value == null)
            return default;
        return value.GetValue();
    }
    
    public T GetValue()
    {
        var idiom = DeviceInfo.Idiom;
        
        if (idiom == DeviceIdiom.Phone && !EqualityComparer<T>.Default.Equals(Phone, default))
            return Phone;
        
        if (idiom == DeviceIdiom.Tablet && !EqualityComparer<T>.Default.Equals(Tablet, default))
            return Tablet;
        
        if (idiom == DeviceIdiom.Desktop && !EqualityComparer<T>.Default.Equals(Desktop, default))
            return Desktop;
        
        return Default;
    }
}
```

---

## 13. Navigation & iOS Specifics

### CUINavigationController.cs

**Default Style Caching** (Lines 9-19):
```csharp
public class CUINavigationController : UINavigationController
{
    public static UIColor DefaultBarTintColor { get; private set; }
    public static UIColor DefaultTintColor { get; private set; }
    public static UIStringAttributes DefaultTitleTextAttributes { get; private set; }
    
    public CUINavigationController()
    {
        if (DefaultBarTintColor == null)
        {
            DefaultBarTintColor = NavigationBar.BarTintColor;
            DefaultTintColor = NavigationBar.TintColor;
            DefaultTitleTextAttributes = NavigationBar.TitleTextAttributes;
        }
        
        // Ensure safe area background
        View.BackgroundColor = UIColor.SystemBackground;
    }
}
```

**Back Button Handling** (Lines 24-35):
```csharp
public override UIViewController[] PopToRootViewController(bool animated)
{
    return base.PopToRootViewController(animated);
}

public override UIViewController PopViewController(bool animated)
{
    var vc = base.PopViewController(animated);
    var cometVC = vc as CometViewController;
    cometVC?.WasPopped();
    
    return vc;
}
```

---

## Summary of Key Patterns

| Aspect | iOS | Android | Windows |
|--------|-----|---------|---------|
| Root View Base | UIView | ViewGroup | Grid |
| Layout Method | LayoutSubviews() | OnMeasure/OnLayout | MeasureOverride/ArrangeOverride |
| Modal Host | UIViewController.Present | DialogFragment | N/A |
| Tab View | UITabBarController | BottomNavigationView | N/A |
| Density/Scale | None (1.0) | DisplayMetrics.Density | None (1.0) |
| Gesture System | UIGestureRecognizer | GestureDetector | Pointer/Manipulation |
| Navigation | UINavigationController | Fragment Stack | Frame Navigation |
| Color Format | UIColor | int (Android color) | WinUI Brush |
| Safe Area | ISafeAreaView | WindowInsets | N/A |

___BEGIN___COMMAND_DONE_MARKER___0
