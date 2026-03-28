# Comet.Sample Visual Audit Report

**Date:** June 2025
**Branch:** `squad/visual-parity-marvelous-app`
**Device:** iPhone 17 Pro Simulator (iOS 26)
**Agent:** Amos, Controls & API Dev

---

## Executive Summary

Audited all **79 active pages** in Comet.Sample (GridSample1 is commented out).
Captured screenshots of every page via Appium automation on iOS Simulator.

| Category | Count |
|----------|-------|
| ✅ Clean / Working | **57** |
| ⚠️ Nav bar overlap (systemic) | **8** |
| ⚠️ Blank page (pre-existing layout bugs) | **5** |
| ⚠️ Minimal/partial content (by design) | **5** |
| 🔴 App crash | **1** |
| 🔴 Missing image asset | **1** |
| ☑️ **Fixed by this audit** | **12 files across 3 commits** |

---

## Fixes Applied (This Audit)

### Commit `ed325943` — App entry point crash
- **MyApp.cs**: `UseCometSampleDebugHost<MyApp>()` → `UseCometSampleDebugHost(CreateRootView)`
- The debug host rejects `CometApp` subtypes; must pass a `Func<View>` factory instead

### Commit `f76d6a80` — Ride the Comet button non-reactive
- **RideTheCometSample.cs**: Old `Component` + `[State]` on plain class → `Component<CometRideState>` with `SetState()`
- Verified via Appium: button now increments ride count and re-renders comet emojis

### Commit `3f2456f4` — 10 files with broken `[State]` attribute pattern
The `[State]` attribute is vestigial — nothing reads it at runtime. All samples using it had silently non-reactive UI (mutations did nothing visible).

| File | Fix Applied |
|------|------------|
| `TextFieldSample3.cs` | `Signal<string>` two-way binding |
| `TextFieldSample4.cs` | `Signal<string>` two-way binding |
| `BasicTestView.cs` | `Component<BasicTestState>` with `SetState()` |
| `BindingSample.cs` | `Component<BindingSampleState>` with `SetState()` |
| `DemoCreditCardView.cs` | `Component<CreditCard>` with `SetState()` + `OnTextChanged` |
| `DemoCreditCardView2.cs` | `Component<CreditCard>` with `SetState()` + `OnTextChanged` |
| `DemoCreditCardView3.cs` | `Component<CreditCard>` with `SetState()` + `OnTextChanged` |
| `Issue133.cs` | `Component<CreditCard>` with `SetState()` + `OnTextChanged` |
| `Issue133b.cs` | `Component<CreditCard>` with `SetState()` + `OnTextChanged` |
| `Issue133c.cs` | `Component<CreditCard>` with `SetState()` + `OnTextChanged` |

---

## Page-by-Page Results

### ✅ Clean / Working (57 pages)

| # | Page | Notes |
|---|------|-------|
| 01 | RideTheComet | Button works, shows ride count + comet emojis (fixed) |
| 02 | TextStyles | H1–H6, subtitles, body, caption, overline |
| 03 | LayoutTest | Recommended cards and ZStack alignment demo |
| 04 | TextWeight | Font weights Black 900 through Thin 100 |
| 08 | GraphicsFingerPaint | Blank canvas (expected for paint app) |
| 09 | MaterialDesign | Color swatches |
| 10 | AuditReportPage | Purple card with "Generate Report" |
| 11 | VStackSample | Text, text field, toggle, slider |
| 15 | SectionedListView | Grouped list |
| 16 | VirtualSectionedListView | Virtualized grouped list |
| 17 | VirtualListView | Virtualized list |
| 20 | NestedView | "Hi!" text centered |
| 21 | AnimationSample | Animation demo with blue bar |
| 22 | TabView | Tab 1 / Tab 2 |
| 23 | BasicTestView | Text field "Bar", toggle/update buttons (fixed) |
| 24 | ListViewSample1 | 2 songs |
| 25 | ListViewSample2 | 2 songs, different layout |
| 26 | InsaneDiff | "State: False" button + numbered list |
| 27 | ButtonSample1 | "Click Me" centered |
| 28 | ClipSample1 | Circular clipped landscape |
| 30 | ClipSampleAspectFit | Landscape image |
| 31 | ClipSampleAspectFill | Landscape image |
| 32 | ClipSampleFill | Landscape image |
| 33 | ClipSampleNone | Landscape image, no clip |
| 36 | SecureFieldSample1 | Secure text entry |
| 37 | SecureFieldSample2 | Secure text entry |
| 38 | SecureFieldSample3 | Secure text entry |
| 39 | ShapeSample1 | Various shapes |
| 40 | ShapeSample2 | Gradient-filled shapes |
| 41 | SliderSample1 | Slider with Celsius/Fahrenheit |
| 42 | StepperSample1 | Stepper with value display |
| 43 | DatePickerSample | Date picker |
| 44 | TextFieldSample1 | Text field |
| 45 | TextFieldSample2 | Text field |
| 46 | TextFieldSample3 | Signal two-way binding (fixed) |
| 47 | TextFieldSample4 | Signal two-way binding (fixed) |
| 48 | RadioButtonSample | Radio buttons |
| 49 | GraphicsSample1 | Custom graphics drawing |
| 50 | GraphicsSample2 | Colored rectangles |
| 51 | GraphicsSample3 | Gradients |
| 52 | GraphicsSample3Scroll | Scrollable graphics |
| 53 | GraphicsSample4 | Graphics drawing |
| 54 | GraphicsSample5 | Graphics with text |
| 55 | GraphicsSample6 | Complex graphics |
| 56 | SwiftUISection1 | "Turtle Rock" with green title |
| 58 | SwiftUISection3 | Map + "Turtle Rock" + subtitle |
| 59 | SwiftUISection4 | Map + Turtle Rock + star toggle |
| 60 | SwiftUISection4b | Similar to Section4 |
| 61 | SwiftUISection4c | Similar to Section4 |
| 62 | SwiftUISection4d | Similar to Section4 |
| 63 | CompareFlutter | "Oeschinen Lake" card with star + icons |
| 64 | DavidSample1 | HStack layout demo |
| 65 | DavidSample1a | HStack layout variant |
| 66 | DavidSample1b | HStack layout variant |
| 67 | DavidSample1c | HStack layout variant |
| 76 | Question1 | Image thumbnail + Title + Description |
| 77 | Question1a | Same card layout |
| 78 | Question1b | Centered title/description variant |
| 79 | Question1c | Same card layout |
| 80 | Question1d | Same card layout |

---

### 🔴 Critical: App Crash (1 page)

#### #35 — ProgressBarSample1
**Severity:** Critical — crashes the entire app, returns to iOS home screen
**Screenshot:** Shows iOS home screen (app terminated)

**Root cause:** `System.Threading.Timer` fires every 100ms on a ThreadPool thread, writing to `Signal<double>.Value`. The reactive update triggers a UI rebuild from a non-main thread, which iOS rejects with a hard crash.

**Source:** `sample/Comet.Sample/Views/ProgressBarSample1.cs`
```csharp
_timer = new Timer(state => {
    var p = (Signal<double>)state;
    p.Value = value; // ← background thread → UI crash on iOS
}, percentage, 100, 100);
```

**Recommended fix:** Use `Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread()` to marshal the Signal update onto the UI thread, or replace `System.Threading.Timer` with `Microsoft.Maui.Dispatching.DispatcherTimer`.

---

### ⚠️ Nav Bar Overlap — Systemic (8 pages)

These pages have content partially hidden behind the iOS navigation bar. This is a **systemic framework issue** — pages that don't use `ScrollView` or explicit top padding get clipped under the nav bar chrome.

| # | Page | Severity | Detail |
|---|------|----------|--------|
| 05 | VGridSample | Medium | Items 0–3 hidden under nav bar |
| 06 | VGridNumberPad | Medium | Top row (7,8,9) partially hidden |
| 07 | ShapeViewSample | Medium | Shape tucked under nav bar |
| 12 | DemoCreditCard | Medium | Credit card top clipped |
| 13 | DemoCreditCard2 | Medium | Same as #12 |
| 14 | DemoCreditCard3 | Medium | Same as #12 |
| 18 | BindingSample | High | Nearly blank — most content under nav bar |
| 29 | ClipSample2 | Low | Slightly tucked at top edge |

**Root cause:** The NavigationView in MainPage pushes detail pages without the layout system automatically accounting for the navigation bar's safe area inset. Pages that begin with `ScrollView` work fine (it respects safe area), but pages that use bare `VStack` or `Grid` as their root get clipped.

**Recommended fix:** Framework-level — ensure `NavigationView` detail content is inset below the navigation bar, or add a default safe-area-aware container.

---

### ⚠️ Blank Pages — Pre-existing Layout Bugs (5 pages)

| # | Page | Root Cause |
|---|------|-----------|
| 69 | Issue123 | Wraps content in `NavigationView()` inside MainPage's NavigationView → nested nav causes layout collapse |
| 70 | Issue125 | `ContentView()` wrapper on `ListView<T>.ViewFor` breaks horizontal fill; list items render at zero width |
| 71 | Issue125b | Same `ListView<T>` layout issue — items don't expand despite `.FillHorizontal()` |
| 72 | Issue125c | Same as Issue125b, `.Frame(height:44)` added but insufficient |
| 73 | Issue133 | ✅ Actually shows content (CC Number, MM/YYYY, CVV) — icons render as `[?]` due to missing Font Awesome |

**Note:** These are **pre-existing framework-level bugs** in `ListView<T>` item layout and nested `NavigationView`. They were not caused by our migration work.

---

### ⚠️ Minimal / Partial Content — By Design (5 pages)

| # | Page | What Shows | Notes |
|---|------|-----------|-------|
| 57 | SwiftUISection2 | "Turtle Rock" text only | Missing landscape image asset (SwiftUI tutorial expects photo above title) |
| 68 | DavidSample2 | Small circle in corner | By design — demo of `ShapeView(Circle)` with `Alignment.BottomTrailing` |
| 73 | Issue133 | CC form with `[?]` icons | Font Awesome 5 not bundled; fields functional |
| 74 | Issue133b | CC Number field only | By design — simplified variant showing one `BorderedEntry` |
| 75 | Issue133c | CC Number field only | By design — `BorderedEntry` as nested `Component` variant |

---

## Issue Classification Summary

### Issues Caused by Previous Migration (All Fixed ✅)
1. App entry point crash (`UseCometSampleDebugHost<MyApp>()`)
2. Non-reactive "Ride the Comet" button
3. 10 files with dead `[State]` attribute on plain classes

### Pre-existing Framework Issues (Not Caused by Migration)
1. **ProgressBarSample1 crash** — Timer thread → Signal → UI crash
2. **Nav bar overlap** — Systemic safe-area issue affecting 8+ pages
3. **ListView<T> item layout** — Items render at zero width (Issue125 series)
4. **Nested NavigationView** — Layout collapse when page wraps in its own NavigationView (Issue123)
5. **Missing Font Awesome** — Icon characters render as `[?]` boxes (Issue133 series)
6. **Missing image asset** — SwiftUISection2 landscape photo

---

## Recommendations

### High Priority
1. **Fix ProgressBarSample1** — Marshal Signal update to main thread via `DispatcherTimer`
2. **Safe area handling** — Framework should automatically inset NavigationView detail content below nav bar

### Medium Priority
3. **ListView<T> item layout** — Investigate why `.FillHorizontal()` doesn't propagate to list view items
4. **Nested NavigationView** — Either detect and flatten, or document as unsupported pattern

### Low Priority
5. **Bundle Font Awesome** — Or replace unicode icons with `Image()` alternatives in Issue133 samples
6. **SwiftUISection2 image** — Add the missing landscape photo or use a bundled resource

---

## Files Changed

```
sample/Comet.Sample/MyApp.cs                             # Entry point fix
sample/Comet.Sample/Views/RideTheCometSample.cs           # Component<T> + SetState
sample/Comet.Sample/Views/TextFieldSample3.cs             # Signal<string>
sample/Comet.Sample/Views/TextFieldSample4.cs             # Signal<string>
sample/Comet.Sample/Views/BasicTestView.cs                # Component<T> + SetState
sample/Comet.Sample/Views/BindingSample.cs                # Component<T> + SetState
sample/Comet.Sample/Views/DemoCreditCardView.cs           # Component<T> + OnTextChanged
sample/Comet.Sample/Views/DemoCreditCardView2.cs          # Component<T> + OnTextChanged
sample/Comet.Sample/Views/DemoCreditCardView3.cs          # Component<T> + OnTextChanged
sample/Comet.Sample/GitHubIssues/Issue133.cs              # Component<T> + OnTextChanged
sample/Comet.Sample/GitHubIssues/Issue133b.cs             # Component<T> + OnTextChanged
sample/Comet.Sample/GitHubIssues/Issue133c.cs             # Component<T> + OnTextChanged
```

## Screenshots

All 79 page screenshots are in `visual-audit/ios-screenshots/` named `{NN}-{PageName}.png`.
