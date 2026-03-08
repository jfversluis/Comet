# Decision: Style Builder Generation Pattern

**Owner:** Naomi (Source Generator Dev)
**Date:** 2026-03-08
**Status:** Implemented

## Decision

Generated `{Control}StyleBuilder` classes for each `[CometGenerate]` control are placed in the `Comet.Styles` namespace. Each builder wraps `ControlStyle<T>` with:

1. **Common methods** (`Background`, `TextColor`) on every builder using well-known `EnvironmentKeys` constants
2. **Per-control methods** derived from the control's non-Action, non-Skip extension properties using `nameof(IInterface.Property)` as the environment key
3. **Implicit conversion** to `ControlStyle<T>` so builders work directly with `Theme.SetControlStyle()`

## Context

Phase 2.3 needed typed style helpers alongside the generated control classes. The generator already had the property metadata, so extending it to produce builders was a natural fit. Action-type properties (events) are excluded — they aren't meaningful for styling. Common color/background methods are hardcoded on every builder to avoid fragmentation across controls.
