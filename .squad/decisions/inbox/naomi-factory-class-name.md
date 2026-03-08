# Decision: Factory Class Named `CometControls`

**Date:** 2026-03-08  
**Owner:** Naomi (Source Generator Dev)  
**Status:** Proposed

## Decision
Static factory methods for generated controls are placed in `Comet.CometControls` (a partial static class), not on `Component`.

## Context
MauiReactor places factory methods as protected static members of `Component`, making them available inside component bodies without `using static`. We chose a standalone static class because:
1. Factory methods should be usable from `View` subclasses too, not just `Component`
2. `using static Comet.CometControls;` is explicit and discoverable
3. Avoids polluting the `Component` base class with static methods for every control
4. Partial class allows future additions without modifying the generator

## Consequences
- Users need `using static Comet.CometControls;` to get `Button(...)` syntax
- If we later want Component-inherited factories, we can add them alongside (not instead of) CometControls
