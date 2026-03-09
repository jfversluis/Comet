# Grid Factory Method Gap

**Author:** Bobbie (Test Engineer)  
**Date:** 2026-07-25  
**Category:** API Gap

## Decision

During Comet.Sample migration, discovered that `CometControls.Grid(params View[])` only accepts child views. There's no factory overload for `Grid(rows, columns, children)`. Files using Grid with row/column definitions must use the constructor form `new Grid(rows:, columns:) { children }` instead of the factory.

## Impact

3 files in Comet.Sample (DemoCreditCardView, ContinuosSample, ViewLayoutTestCase) cannot fully use the factory API for Grid. This is a minor inconsistency — all other container types (VStack, HStack, ZStack, ScrollView, NavigationView, Border) have complete factory coverage.

## Recommendation

Amos should consider adding `Grid(object[] rows, object[] columns, params View[] children)` overloads to `CometControls.Containers.cs` for API consistency.
