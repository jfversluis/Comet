# Decision: Component Base Class Architecture

**Author:** Holden (Lead Architect)
**Date:** 2026-03-08
**Status:** Implemented

## Context

Phase 1.1 + 1.2 of the convergence roadmap: add `Component`, `Component<S>`, `Component<S,P>` base classes and `Reactive<T>` alongside the existing `View` + `[Body]` pattern.

## Decisions

1. **Component extends View directly.** Component wires `Body = () => Render()` in its constructor. This means the entire existing View pipeline — `GetRenderView()`, `Diff()`, hot reload, handler management — works without modification. No changes to View.cs were needed.

2. **Render() is public abstract.** This makes components testable and follows the MauiReactor pattern where `Render()` is the primary public contract.

3. **State<T> unsealed.** Removed the `sealed` modifier so `Reactive<T>` can subclass it. This is additive and non-breaking for existing consumers.

4. **Component<TState>.State hides View.State.** Uses `public new TState State` to provide a typed state property. The internal `BindingState` remains accessible via `GetState()` for framework internals.

5. **SetState batches via StateManager.** `SetState(Action<TState>)` wraps mutations in `StateManager.BeginBatch()`/`EndBatch()`, then schedules `Reload()` on the main thread. This means multiple property changes within a single `SetState` call result in one re-render.

6. **IComponentWithState for hot reload.** A separate interface enables the hot reload system to transfer typed state between old and new component instances without coupling to the generic type parameters.

## Impact

- Additive only — no existing APIs changed behavior
- All 394 existing tests continue to pass
- 32 new Component tests + 3 Reactive tests pass
