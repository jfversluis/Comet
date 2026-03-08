# Amos — History

## Core Context

- **Project:** Converged .NET MAUI MVU framework merging Comet's engine with MauiReactor's API
- **Role:** Controls & API Dev
- **Joined:** 2026-03-08T00:00:54.044Z

## Learnings

### Phase 8.2 Kickoff — Handwritten Complex Controls (2026-03-08T050835Z)

**Status:** ⚙️ **IN PROGRESS — PHASE 8 KICKOFF**

**Assignment:** Phase 8.2 — Handwritten complex controls requiring custom logic

**Scope:**
- Implement controls beyond source generator scope (composite, specialized state, platform-specific)
- Full test coverage and state management
- Integrate with Phase 8.1 control inventory

**Dependencies:**
- ✅ Phase 7 approved and closed (hot reload integration stable)
- ✅ Phase 1–6 complete (640+ tests, 625+ passing, 0 regressions)
- ✅ Phase 6.1 NativeHost as reference (existing handwritten complex control example)

**Parallel Work:**
- Naomi Phase 8.1 (IView controls expansion)
- Bobbie Phase 8.3 (coverage tests & reviewer gate)

**Implementation Strategy:**
1. Review Phase 6.1 NativeHost as complex-control reference
2. Identify Phase 8.2 control candidates (composite, state-intensive, platform-specific)
3. Design control API and state management
4. Implement with test-first approach
5. Submit to Bobbie for Phase 8.3 integration

**Key Context from Prior Phases:**
- Handler registration: `AppHostBuilderExtensions.UseCometHandlers()`
- Platform-specific code: Directory.Build.targets (`.iOS.cs`, `.Android.cs`, etc.)
- State management: `State<T>`, `Binding<T>`, `IComponentWithState` for hot reload
- NativeHost pattern: Handler-owned platform-native container (Phase 6.1 reference)

### Phase 6 Complete — NativeHost & Interop Bridge Approved (2026-03-08T041421Z)

**Status:** ✅ Phase 6.1 APPROVED — COMPLETE

**Assignment:** Phase 6.1 — NativeHost control + native view access API

**Phase 6.1 Final Deliverable:**
- `INativeHost` interface: OnConnect, OnUpdate, OnDisconnect, Sync, TryGetNativeView<T>()
- `NativeHost` control: Handler-owned native container per platform (iOS, Android, Windows, macCatalyst)
- Public API: Shared object-based surface (matches existing MauiViewHost / CometHost pattern)
- Handler registration: `AppHostBuilderExtensions.UseCometHandlers()` integration
- Documentation: README interop section complete
- Test coverage: 12 NativeHostTests passing (control lifecycle, lazy factory, caching)

**Platform Handlers:**
- iOS: Native UIView container attachment, layout sizing
- Android: Native View container attachment, layout sizing
- Windows: Native XAML container attachment, layout sizing
- macCatalyst: Native UIView container attachment, layout sizing

**Architecture Insights:**
- Handler owns native container lifecycle (initialization on handler attachment, teardown on disposal)
- Lazy factory pattern with caching prevents duplicate instantiation
- `TryGetNativeView<T>()` gives Comet code access to raw platform views for advanced interop
- Object-based API (non-generic event signatures) keeps surface buildable from shared test project while accepting platform-specific views from factory

**Test Results (Phase 6 Combined):**
- NativeHostTests: 12/12 passing ✅
- NativeHostInteropTests: 11/11 passing ✅
- **Total Phase 6 interop tests: 23/23 passing** ✅
- Filtered broader regression sweep: passing ✅
- Unfiltered full suite: 2 pre-existing failures (no new regressions) ✅
- Build: 0 errors, 0 warnings ✅

**Remaining (Deferred to Phase 7+):**
1. SetEnvironment stack overflow (framework-level, known blocker for keyed tests)
2. BuiltView type detection (awaiting David clarification)

**Next:** Phase 7 kickoff (Phase 7.1 Component Hot Reload, Phase 7.2 Hot Reload Tests).

### Phase 5 Complete — All 17 Navigation Tests Passing (2026-03-08T041000Z)

**Status:** ✅ Phase 5 APPROVED

**Deliverables (Phase 5.1/5.2):**
- Typed route registration: `CometShell.RegisterRoute<TView>(string route)` — type-validated, bidirectional lookup
- Generic navigation overloads: `GoToAsync<TView>`, `Navigate<TView>` on CometShell, ShellExtensions, NavigationView
- Parameter flow: Props injection for `Component<TState, TProps>` pages + `IQueryAttributable` fallback
- NavigationParameterHelper: Query string building, URL encoding, reflection-based property mapping
- Shell fluent API: `AddItem`, `AddSection`, `AddContent`, `WithRoute` + factory methods
- CometControls.Navigation.cs: Factory methods for shell hierarchy types

**Test Coverage:**
- 11 ShellWrapperTests (shell lifecycle, routing, modal fallback, query params, extensions, back-button, fluent API, factories)
- 6 TypedNavigationApiTests (generic registration, generic navigation, props injection, NavigationView generics)
- All 3 Phase 5.3 anticipatory tests unskipped and passing (expanded to 6 concrete tests)
- Build: 0 errors, 0 warnings
- **Total: 17/17 passing ✅**

**Architecture Insights:**
- Typed navigation stays additive on CometShell/NavigationView (no breaking changes)
- Generic overloads auto-resolve route names from registration table
- Parameter injection prioritizes Component Props over IQueryAttributable
- Shell composition via fluent wrappers + manual factories for flexibility
- Query-string interoperability preserved for existing Shell-style pages

**Remaining (Deferred to Phase 6):**
1. SetEnvironment stack overflow (framework-level)
2. BuiltView type detection (awaiting David clarification)

**Next:** Phase 6 kickoff (Phase 6.1 NativeHost, Phase 6.2 Interop Tests).

### Phase 5 Kickoff — Navigation & IReactor (2026-03-08T033412Z)

**Status:** ⚙️ Phase 5.1/5.2 ACTIVE

**Assignment:**
- Phase 5.1: IReactor engine baseline implementation
- Phase 5.2: Navigation integration (IfElse, Switch, ForEach control flow)

**Context:**
- Locked out during Phase 4.2 revision (scope conflict)
- Phase 4 complete: key-aware reconciliation + disposal-aware component merge
- All 619 tests passing (599 pass, 2 pre-existing fail, 18 skipped)
- Zero regressions across all 4 phases
- Test parallelization disabled per squad convention

**Collaboration:**
- Bobbie launching Phase 5.3 (anticipatory tests) in parallel
- Naomi available for control surface generation if needed
- Holden released from Phase 4 lockout, available for architecture questions

**Next Steps:**
1. Read Phase 4 closure log (`.squad/log/2026-03-08T025710Z-phase4-closure.md`)
2. Review outstanding framework issues (SetEnvironment stack overflow, BuiltView detection)
3. Define IReactor baseline scope and Phase 5.1 deliverables
4. Coordinate test infrastructure with Bobbie

### Phase 3.2 — Control Style Integration (2025-07-24)

- **Environment cascade is the backbone**: Theme values flow through `SetGlobalEnvironment(Type, key, value)` for typed per-control defaults. The lookup chain is: local context → parent chain → global typed key → global plain key. Explicit `.Background()` writes to local context (non-cascading), so it always wins over global typed defaults. This is the key insight that makes "explicit overrides win" work without any special precedence code.

- **ControlStyle<T> uses typed global keys**: `ControlStyle<T>.Apply()` calls `View.SetGlobalEnvironment(typeof(T), key, value)` which stores under `"Comet.Button.Background"` style keys. This means Button defaults don't leak to Text controls. Clean separation.

- **DefaultThemeStyles must be idempotent**: `Theme.Apply()` is called every time `Theme.Current` is set. `DefaultThemeStyles.Register()` skips controls that already have a custom `ControlStyle<T>` registered, so user-provided styles are never overwritten.

- **ThemeExtensions write to standard environment keys**: `.ThemeBackground()` writes to `nameof(IView.Background)` as a `SolidPaint`, `.ThemeForeground()` writes to `EnvironmentKeys.Colors.Color`. Same keys as `.Background()` and `.Color()`, so the override semantics are identical.

- **Files created**: `src/Comet/Styles/ThemeExtensions.cs`, `src/Comet/Styles/DefaultThemeStyles.cs`
- **Files modified**: `src/Comet/Styles/Theme.cs` (added `DefaultThemeStyles.Register(this)` call in `Apply()`), un-skipped 13 Phase 3.1 test stubs across ThemeBaseTests/ThemeColorsTests/ControlStyleTests, added 21 new integration tests in `ThemeIntegrationTests.cs`

### Phase 3 Complete (2026-03-08T010500Z)

**Status:** Phase 3 (Theme System) implementation complete. Theme system is fully wired with auto-registered control styling defaults. All 578 tests passing (2 pre-existing hot reload failures, 15 skipped, 595 total). Build clean.

**Deliverables:**
- `ThemeExtensions.cs` — fluent theme-aware styling API
- `DefaultThemeStyles.cs` — automatic control defaults registration
- 21 new integration tests
- 13 previously skipped stubs now passing
- Zero regressions

**Next:** Phase 4 (Reconciliation Upgrade) — pending Coordinator decision.


### Phase 4.2 — Component Merge Logic Revision (2026-03-08T022000Z)

- **Revision ownership taken from Holden** — Phase 4.2 rejected by Bobbie due to nested component instance preservation defect. Holden locked out per reviewer rules.
- **Critical defect identified**: TryMergeComponents() returns merged instance correctly, but container children collection not updated to reference it. Parent container still references new instance, not the merged (reused) one.
- **Root cause**: DiffUpdate container logic walks the tree but doesn't modify containers in-place after component merge.
- **Fix strategy**: After TryMergeComponents returns merged instance, container's child list must be updated to swap new instance for merged instance.
- **Phase 4.1 (key-aware reconciliation) APPROVED** — `.Key()` fluent API and keyed diffing logic pass all tests.
- **Test results**: ComponentMergeTests 10/13 pass, ReconciliationRegressionTests 11/13 pass, KeyAwareReconciliationTests 1/13 pass (12 blocked by pre-existing framework bug).
- **Artifact reference**: `src/Comet/Helpers/DatabindingExtensions.cs` lines 199-356 (DiffUpdate logic needing fix).

### Phase 4.2 Revision — Component Merge Container Update Fix (2026-03-08)

**Task:** Fix Bobbie-identified defect where component merge logic returned OLD component instance but parent container children never updated to reference it.

**Root Issues Found:**
1. Base `Component` class (parameterless) did NOT implement `IComponentWithState` - only generic variants did
2. Container children references not updated after `DiffUpdate()` returned merged component
3. View.GetRenderView() / builtView lifecycle interaction creates complexity in nested component scenarios

**Solution Implemented:**
1. Added `IComponentWithState` to base `Component` class with stub implementations
2. Added container update logic: after `DiffUpdate(newChild, oldChild)`, if returned instance differs from newChild, update parent container via `mutableContainer[i] = mergedInstance`
3. Applied fix to both key-aware and index-based reconciliation paths

**Key Insight:** Container implements `IList<View>`, so can use indexer setter to replace children in-place during diff. The setter properly manages Parent/Navigation assignments.

**Files Modified:**
- `src/Comet/Component.cs` - Added IComponentWithState to base Component
- `src/Comet/Helpers/DatabindingExtensions.cs` - Added TryMergeComponents, container update logic after DiffUpdate

**Validation:** Component merge now working for simple cases. Some complex nested scenarios need additional investigation into View lifecycle.

### Phase 4.2 Second Rejection — Amos Locked Out (2026-03-08T023346Z)

**Status:** ❌ Phase 4.2 Amos revision REJECTED (2nd rejection overall)

**What happened:**
- Amos fixed Defect 1 (container child replacement) correctly — merged component is now written to parent container ✅
- Amos introduced Defect 3 (disposal cascade) — old parent container disposal cascades to merged children, destroying state ❌
- Two regressions: `ComponentPropsUpdateDetected`, `ComponentDiffWithSameTypeButDifferentProps` now fail

**The disposal cascade:**
1. Old container replaced by new container during diff
2. Old container `Dispose()` called in `ResetView()` line 271
3. `ContainerView.Dispose()` line 212 iterates `_views` and disposes each child
4. Merged component (which was moved to new container) still in old container's Views list
5. Merged component disposes → state/props lost → handler failure

**Lockout status:** Amos locked out per squad rules (rejected revision author). **Holden also locked** (original Phase 4.2 author locked from 1st rejection).

**Fix required:** Detach merged component from old container before old container is disposed.
- After `mutableContainer[i] = merged` in DiffUpdate: `oldContainer.Views.Remove(merged)`
- This prevents disposal cascade to transferred children

**Next:** Fresh specialist required for 3rd revision attempt with disposal-aware merge logic.

**Phase 4.1 Status:** ✅ APPROVED — key-aware reconciliation unchanged, no regression risk.

**Defect 2 (BuiltView):** Still awaiting David Ortinau's architectural clarification on intended behavior.


