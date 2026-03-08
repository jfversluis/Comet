# Squad Decisions

## Active Decisions

### 2026-03-08T00:25:46Z: Project Name — Comet
**Owner:** David Ortinau  
**Status:** Affirmed  
**Decision:** The project name stays as **Comet** — no rename to "Orbit" or other alternatives. All execution plans and team context use "Comet" throughout.
**Context:** User confirmed during Phase 1 kickoff review.

### 2026-03-08T00:36:05Z: Component Base Class Architecture
**Owner:** Holden (Lead Architect)  
**Status:** Implemented  
**Decision:** Component extends View and wires `Body = () => Render()` in its constructor. Render() is public abstract. State<T> unsealed to allow Reactive<T> subclassing (binary-additive, non-breaking). Component<TState>.State uses `public new` to hide View.State with typed variant. SetState() batches mutations via StateManager and schedules Reload() on main thread. IComponentWithState interface for hot reload state transfer without generic coupling.
**Context:** Phase 1.1 + 1.2 complete. No changes to View.cs required. All 394 existing + 35 new tests pass. Two pre-existing hot reload failures not related to Component work.

### 2026-03-08T00:36:05Z: Component Tests — Subdirectory, Flat Namespace
**Owner:** Bobbie (Test Engineer)  
**Status:** Adopted  
**Decision:** Component tests live in `tests/Comet.Tests/ComponentTests/` subdirectory but keep the `Comet.Tests` namespace (matching existing project convention where namespace doesn't mirror folder structure). This groups related tests logically on disk without fragmenting the namespace.
**Context:** Phase 1.3 complete. 34 new Component tests passing (ComponentBaseTests, ComponentStateTests, ComponentPropsTests, ComponentLifecycleTests). All 394 existing tests remain green.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
