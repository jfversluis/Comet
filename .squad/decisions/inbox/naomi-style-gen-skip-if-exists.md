# Decision: Style Generator Uses Skip-If-Exists for Parallel Safety

**Author:** Naomi (Source Generator Dev)
**Date:** 2026-03-10
**Status:** Active

## Context

The StyleInfrastructureGenerator generates configuration structs, style extensions, and ResolveCurrentStyle() methods from `[CometControlState]` attributes. However, Amos has hand-written initial versions of these types (Configurations.cs, ControlStyleExtensions.cs).

## Decision

The generator checks the compilation before emitting:
- **Config structs:** Skips if `Comet.Styles.{Control}Configuration` already exists as a type.
- **Style extensions:** Skips if `{Control}Style` method already exists on `ControlStyleExtensions`.
- **ResolveCurrentStyle:** Always emits (different signature from hand-written extension method).
- **Theme fallback:** Conditional — only emits `GetControlStyle<T,TConfig>()` call if the 2-param overload exists on Theme.

## Rationale

This avoids duplicate-type compilation errors during parallel development. When hand-written files are removed in a future merge, the generator automatically takes over. The generator is the source of truth per D6.

## Impact

- Amos's hand-written config structs/style extensions are authoritative until deleted.
- The generator acts as a safety net — any future styleable control only needs a `[CometControlState]` attribute.
- Theme fallback auto-enables when Holden adds `Theme.GetControlStyle<T, TConfig>()`.
