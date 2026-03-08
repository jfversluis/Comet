# Amos — History

## Core Context

- **Project:** Converged .NET MAUI MVU framework merging Comet's engine with MauiReactor's API
- **Role:** Controls & API Dev
- **Joined:** 2026-03-08T00:00:54.044Z

## Learnings

<!-- Append learnings below -->

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
