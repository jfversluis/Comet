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

### 2026-03-08T004600Z: Factory Class Named `CometControls`
**Owner:** Naomi (Source Generator Dev)  
**Status:** Implemented  
**Decision:** Static factory methods for generated controls are placed in `Comet.CometControls` (a partial static class), not on `Component`. Each control generates Binding<T>, Func<T> (parameterized only), and parameterless overloads. Users import via `using static Comet.CometControls;`.
**Context:** Phase 2.1-2.2 complete. Factory class decoupled from Component base allows `using static` from any View context. 14 new factory method tests passing. Non-Action properties receive no "On" prefix (already clean). "On" prefix applied only to Action-type extension properties (e.g., `.OnPressed()`, `.OnReleased()`). Partial class allows future additions without modifying the generator.

### 2026-03-08T005500Z: Theme System Architecture
**Owner:** Holden (Lead Architect)  
**Status:** Implemented  
**Decision:** Theme is a concrete (not abstract) base class. ThemeColors holds MD3 semantic tokens as a separate class. ControlStyle<T> is a generic typed style builder that pushes properties through the environment system via an internal IControlStyleApplicable interface. Theme.Current setter auto-applies and fires ThemeChanged event. IThemeable is opt-in for controls that need direct notification.
**Context:** Existing code instantiates `new Theme()` directly in 12+ tests and the singleton getter. Making Theme abstract would break backward compatibility. The two-class approach (Theme + ThemeColors) keeps the old simple color properties working while adding the full MD3 semantic color system. The environment integration uses the same `View.SetGlobalEnvironment()` broadcast mechanism that the existing `Style.Apply()` uses, so theme changes propagate to all active views automatically.
**Impact:** All existing tests pass unchanged (574 total, 2 pre-existing failures only). `Theme.Light` and `Theme.Dark` now include `ThemeColors` presets. `Theme.Current = newTheme` now triggers environment updates (additive behavior). New `EnvironmentKeys.ThemeColor.*` keys do not conflict with existing keys.

### 2025-07-24: Default Theme Styles Registration
**Owner:** Amos (Controls & API Dev)  
**Status:** Implemented  
**Decision:** `DefaultThemeStyles.Register(theme)` is called inside `Theme.Apply()` before applying control styles. It registers `ControlStyle<T>` entries for Button, Text, TextField, Toggle, and Slider using the theme's color scheme. It skips any control type that already has a custom style registered.
**Rationale:** Keeps controls decoupled from theme logic (no `IThemeable` needed on generated controls). Uses the existing `ControlStyle<T>` → `SetGlobalEnvironment(Type, key, value)` pipeline. Idempotent: safe to call multiple times.
**Impact:** Setting `Theme.Current = Theme.Light` automatically gives all buttons a primary-colored background. Users can override any default by calling `theme.SetControlStyle(customStyle)` BEFORE setting `Theme.Current`, or by using explicit fluent methods like `.Background(Colors.Red)`.

### 2026-03-08: Style Builder Generation Pattern
**Owner:** Naomi (Source Generator Dev)  
**Status:** Implemented  
**Decision:** Generated `{Control}StyleBuilder` classes for each `[CometGenerate]` control are placed in the `Comet.Styles` namespace. Each builder wraps `ControlStyle<T>` with: (1) Common methods (`Background`, `TextColor`) on every builder using well-known `EnvironmentKeys` constants, (2) Per-control methods derived from the control's non-Action, non-Skip extension properties using `nameof(IInterface.Property)` as the environment key, (3) Implicit conversion to `ControlStyle<T>` so builders work directly with `Theme.SetControlStyle()`.
**Context:** Phase 2.3 needed typed style helpers alongside the generated control classes. The generator already had the property metadata, so extending it to produce builders was a natural fit. Action-type properties (events) are excluded — they aren't meaningful for styling. Common color/background methods are hardcoded on every builder to avoid fragmentation across controls.

### 2026-03-08: Key-Aware Reconciliation Architecture
**Owner:** Holden (Lead Architect)  
**Status:** Implemented  
**Decision:** Implement opt-in key-based view reconciliation using the existing environment system. Keys are assigned via `.Key(string)` fluent API, stored as `EnvironmentKeys.View.Key`, and used during diff to match children by identity rather than position. Key storage uses `cascades: false` to prevent propagation. Diff() algorithm has two code paths: (1) Key-aware (when any child has a key) using O(1) Dictionary lookup, (2) Index-based (original algorithm, unchanged). Activation is automatic — no keys present = original behavior.
**Context:** Phase 4.1 implemented. Key-aware diffing preserves view instances across reorders, improving state and animation fidelity. Dictionary pre-builds amortize cost across all children (O(n) vs O(n²) worst-case). String keys chosen over object for simpler equality and debugging. 100% backward compatible — unkeyed lists diff identically to before.
**Impact:** Dynamic lists (e.g., todo items, chat messages) reuse view instances when keyed, preserving handler state and animations. Non-component views and unkeyed lists see zero regression. Hot reload state transfer works transparently via environment mechanism.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
