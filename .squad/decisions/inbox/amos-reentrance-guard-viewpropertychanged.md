# Decision: Re-entrancy guard in ViewPropertyChanged

**Author:** Amos (Controls & API Dev)
**Date:** 2025-07
**Status:** Implemented

## Context

`ViewPropertyChanged` → `SetPropertyValue` → property setter → `SetPropertyInContext` → `SetEnvironment` → `ContextPropertyChanged` → `ViewPropertyChanged` creates infinite recursion (stack overflow). This killed 8 tests and the full test suite.

## Decision

Added a per-property re-entrancy guard (`HashSet<string> _propertiesBeingUpdated`) in `View.ViewPropertyChanged`. If a property is already being updated on the same View instance, the recursive call is silently skipped. The guard is per-property (not a single boolean) so independent property updates can still proceed.

Also fixed key-aware reconciliation to skip `DiffUpdate` for non-Component keyed matches and directly reuse the old view instance, preserving identity and handlers.

## Alternatives Considered

- **Single boolean flag:** Simpler but would block legitimate concurrent property updates on different properties.
- **Breaking the cycle at SetPropertyInContext:** Too invasive; would affect all environment propagation.
- **Value-equality check in SetEnvironment:** Wouldn't help when the same value is being re-set.

## Impact

- `src/Comet/Controls/View.cs` — 3 lines added (field + guard + cleanup)
- `src/Comet/Helpers/DatabindingExtensions.cs` — keyed reconciliation branch split for Component vs non-Component views
- All 720 tests pass, 0 regressions
