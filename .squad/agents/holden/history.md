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
