# Decision: Always use named `spacing:` parameter in VStack/HStack factory calls

**Author:** Amos (Controls & API Dev)  
**Date:** 2026-03-10  
**Status:** Proposed

## Context

When calling `VStack(0f, children...)`, the C# compiler reports CS0121 ambiguity between `VStack(float?, params View[])` and `VStack(LayoutAlignment, params View[])` because literal `0` in any numeric form implicitly converts to enum types.

## Decision

All sample and application code should use the named parameter form: `VStack(spacing: 0, ...)` or `HStack(spacing: 8, ...)`. This avoids the ambiguity and improves readability.

## Impact

Affects all code using VStack/HStack factory methods with a numeric spacing value. The pattern `VStack(spacing: N, child1, child2)` is the canonical form going forward.
