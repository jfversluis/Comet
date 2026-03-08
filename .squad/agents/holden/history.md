# Holden — History

## Core Context

- **Project:** Converged .NET MAUI MVU framework merging Comet's engine with MauiReactor's API
- **Role:** Lead Architect
- **Joined:** 2026-03-08T00:00:54.043Z

## Learnings

<!-- Append learnings below -->

### Phase 1.1 + 1.2 — Component Base Class & Reactive<T> (2026-03-08)

**Architecture decisions:**
- `Component` extends `View` and wires `Body = () => Render()` in its constructor, so the entire existing View pipeline (GetRenderView, diff, hot reload) works unchanged.
- `Render()` is `public abstract` — not `protected` — because component subclasses need to be testable and the method is the primary public contract.
- `Component<TState>.State` uses `public new TState State` to deliberately hide `View.State` (the `BindingState`). The typed state is the public API for Components; the internal `BindingState` is still accessible via `GetState()`.
- `SetState()` uses `StateManager.BeginBatch()`/`EndBatch()` for batching, then `ThreadHelper.RunOnMainThread(() => Reload())` to trigger re-render — safe from any thread.
- `State<T>` was unsealed (removed `sealed` keyword) to allow `Reactive<T>` to subclass it directly. This is binary-additive (non-breaking).
- `Reactive<T> : State<T>` is a thin subclass with only implicit operators added. Full backward compat with `State<T>`.
- Lifecycle hooks: `OnMounted()` fires from `OnLoaded()` (first handler set), `OnWillUnmount()` fires from `Dispose(bool)`.

**Key files:**
- `src/Comet/Component.cs` — Component, Component<TState>, Component<TState, TProps>
- `src/Comet/IComponentWithState.cs` — Interface for hot reload state transfer
- `src/Comet/Reactive.cs` — Reactive<T> : State<T>
- `src/Comet/State.cs` — Unsealed State<T>

**Pre-existing test failures (not introduced by this work):**
- `HotReloadTests.HotReloadRegisterReplacedViewReplacesView` — hot reload mock issue
- `ReloadTransfersStateTest.StateTransfersOnlyChangedValues` — null replaced view

### Phase 1 Completion (2026-03-08T003605Z)

**Verification:** All 394 existing tests pass. 32 new Component tests pass (Bobbie Phase 1.3). 3 Reactive<T> tests pass. Total 35 new tests, zero regressions. Phase 1 orchestration log: `.squad/orchestration-log/2026-03-08T003605Z-holden.md`. Phase 1 session log: `.squad/log/2026-03-08T003605Z-phase1-complete.md`. Merged decisions into `.squad/decisions.md` (Component Base Class Architecture, two Component test decisions from Bobbie). Ready for Phase 2: MauiReactor API surface.

### Phase 3.1 — Theme Base Class

**Architecture decisions:**
- `Theme` stays a concrete class (not abstract) because existing code instantiates it directly (`new Theme()`, `_current ??= new Theme()`). Making it abstract would break 12+ existing tests and the default-construction pattern.
- `ThemeColors` is a separate class holding Material Design 3 semantic color tokens (29 roles: Primary/OnPrimary/PrimaryContainer/etc.). Keeps Theme backward-compatible while adding the rich color system.
- `Theme.ColorScheme` property (type `ThemeColors`) is the opt-in bridge — null by default for backward compat, set automatically on `Theme.Light` and `Theme.Dark` presets.
- `ControlStyle<T>` implements internal `IControlStyleApplicable` to allow Theme.Apply() to iterate untyped styles without reflection. The dictionary stores `Type → object` and casts through the interface.
- `Theme.Current` setter triggers `Apply()` + `ThemeChanged` event. This pushes all theme values into the global environment via `View.SetGlobalEnvironment()`, which broadcasts to all active views — the same mechanism the existing `Style.Apply()` uses.
- `IThemeable` is an opt-in interface for controls. During `Theme.Apply()`, active views implementing `IThemeable` receive the theme directly. This avoids requiring all controls to poll the environment for theme changes.
- All semantic color tokens get their own `EnvironmentKeys.ThemeColor.*` constants with `"Theme."` prefix, cleanly separated from existing keys.
- `ThemeColors.ApplyToEnvironment()` is `internal` — only Theme orchestrates environment writes. External code uses `Theme.Apply()` or `Theme.Current = ...`.

**Key files:**
- `src/Comet/Styles/ThemeColors.cs` — MD3 semantic color tokens with light/dark presets
- `src/Comet/Styles/IThemeable.cs` — Opt-in interface for themed controls
- `src/Comet/Styles/ControlStyle.cs` — Generic typed control style, environment-integrated
- `src/Comet/Styles/Theme.cs` — Enhanced with ColorScheme, Apply(), ThemeChanged event, control style registry
- `src/Comet/EnvironmentData.cs` — Added `EnvironmentKeys.ThemeColor` with 29 semantic keys
- `src/Comet/Helpers/ViewExtensions.cs` — Added `ApplyTheme()` and `ApplyControlStyle()` extensions

**Verification:** 574 total tests, 544 passed, 2 pre-existing failures, 28 skipped. Zero regressions.

**Orchestration log:** `.squad/orchestration-log/2026-03-08T005500Z-holden.md`  
**Session log:** `.squad/log/2026-03-08T005500Z-phase3-1-complete.md`
