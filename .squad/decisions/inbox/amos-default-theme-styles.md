# Decision: Default theme styles registered automatically via Theme.Apply()

**Author:** Amos (Controls & API Dev)
**Date:** 2025-07-24
**Status:** Implemented

## Context

Phase 3.2 needed a way for controls to pick up themed defaults without modifying each control class individually.

## Decision

`DefaultThemeStyles.Register(theme)` is called inside `Theme.Apply()` before applying control styles. It registers `ControlStyle<T>` entries for Button, Text, TextField, Toggle, and Slider using the theme's color scheme. It skips any control type that already has a custom style registered.

This means:
- Setting `Theme.Current = Theme.Light` automatically gives all buttons a primary-colored background.
- Users can override any default by calling `theme.SetControlStyle(customStyle)` BEFORE setting `Theme.Current`, or by using explicit fluent methods like `.Background(Colors.Red)`.

## Rationale

- Keeps controls decoupled from theme logic (no `IThemeable` needed on generated controls).
- Uses the existing `ControlStyle<T>` → `SetGlobalEnvironment(Type, key, value)` pipeline.
- Idempotent: safe to call multiple times.

## Impact

- All team members adding new control types should add a corresponding section to `DefaultThemeStyles` if the control has sensible color defaults.
- Custom themes that want different control defaults should register their `ControlStyle<T>` entries before calling `Apply()` or setting `Theme.Current`.
