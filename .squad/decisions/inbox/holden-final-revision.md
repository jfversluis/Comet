# Decisions — Final Spec Revision (Holden, 2026-03-10)

## D-FR1: Configuration structs carry `View TargetView`

**Owner:** Holden  
**Status:** Decided  
**Decision:** All control style configuration structs (`ButtonConfiguration`, `ToggleConfiguration`, `TextFieldConfiguration`, `SliderConfiguration`) include a `View TargetView { get; init; }` property. The control populates this with `this` when constructing the config. Style implementations use `config.TargetView` to resolve tokens from the nearest scoped theme.  
**Rationale:** This is the minimal change that makes the entire control-style resolution path view-aware. It doesn't change the `IControlStyle<T, TConfig>` interface, and it's opt-in — styles that don't need scoped resolution can ignore `TargetView`.

## D-FR2: Environment methods remain string-keyed; use `Token<T>.Key`

**Owner:** Holden  
**Status:** Decided  
**Decision:** No `Token<T>`-accepting overloads will be added to `GetEnvironment`, `SetEnvironment`, `GetGlobalEnvironment`, or `SetGlobalEnvironment`. All token usage in the environment goes through `token.Key`. `Token<T>.Key` is `internal` by design — user code uses `view.GetToken(token)` which wraps the key lookup.  
**Rationale:** The Comet environment is string-keyed at every level (`ContextualObject`, `EnvironmentData`, `View`). Adding typed overloads would duplicate every signature for marginal ergonomic gain. Keeping `.Key` explicit in spec examples ensures they compile against the real API.

## D-FR3: `ImmutableDictionary` for Theme control styles

**Owner:** Holden  
**Status:** Decided  
**Decision:** `Theme._controlStyles` is an `ImmutableDictionary<Type, object>` (not a mutable `Dictionary`). `SetControlStyle()` replaces the field via `_controlStyles.SetItem(key, value)`. This eliminates aliasing when themes are composed via `record with`.  
**Rationale:** `with` shallow-copies references. A mutable dictionary means derived themes mutate the base. `ImmutableDictionary` is the simplest fix — no copy-on-write flag needed, no custom clone logic. `System.Collections.Immutable` is already available in .NET 10.

## D-FR4: Control styles use eager token resolution, not `Binding<T>`

**Owner:** Holden  
**Status:** Decided  
**Decision:** Inside `IControlStyle<T, TConfig>.Resolve()`, tokens are resolved eagerly via `token.Resolve(theme)` where `theme = ThemeManager.Current(config.TargetView)`. Control styles do NOT use `Binding<T>` or implicit `Token<T>` conversions. This is the authoritative strategy throughout the spec (§9.4, §15.2, all examples).  
**Rationale:** Control styles re-evaluate on every state change via `OnControlStateChanged()`. Wrapping resolved values in `Binding<T>` adds unnecessary indirection and reintroduces the global-theme-resolution problem that the view-aware path was designed to solve. Eager resolution is simpler and scoped-correct.
