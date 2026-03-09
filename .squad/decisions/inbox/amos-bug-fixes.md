# Decision: Bug Fixes from E2E Testing

**Author:** Amos (Controls & API Dev)
**Date:** 2025-07-17

## Bug 1 — GetHashCode Negative Index (P1, Fixed)

**File:** `sample/CometAllTheLists/Pages/AddressBookPage.cs`
**Problem:** `GetHashCode() % array.Length` produces negative indices when the hash is negative.
**Fix:** Changed to `((hash % len) + len) % len` pattern. This is safer than `Math.Abs()` which throws `OverflowException` on `Int32.MinValue`.
**Convention:** All future code using hash-based indexing must use the double-modulo pattern.

## Bug 2 — RadioButton InvalidCastException (P2, Documented as Known Issue)

**File:** `sample/Comet.Sample/Views/RadioButtonSample.cs`
**Problem:** MAUI's `RadioButtonHandler` casts the view to `IRadioButton`, but Comet's `RadioButton` doesn't implement that interface. The `CometGenerate` attribute for `IRadioButton` is intentionally commented out in `ControlsGenerator.cs`.
**Root cause:** Architectural mismatch — Comet uses container-based grouping (RadioGroup required) while MAUI expects property-based grouping (GroupName). Implementing `IRadioButton` requires reconciling these models.
**Action taken:** Replaced sample body with a known-issue placeholder (text labels explaining the limitation). No RadioButton controls are instantiated, preventing the crash.
**Future work:** To fully fix, Comet's RadioButton needs to implement `IRadioButton` with explicit interface mappings (`Selected` → `IsChecked`, `Label` → `Content`), handle `GroupName` gracefully, and potentially create a custom handler instead of reusing MAUI's.
