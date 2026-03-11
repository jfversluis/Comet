# Decision: Avoid Separate State for Derived Values

**Author:** Holden (Lead Architect)
**Date:** 2025-07-17
**Context:** NavigationView title bug in gallery sidebar

## Problem

Using two `State<T>` variables where one is derivable from the other creates a subtle binding system pitfall. When `selectedTitle.Value` is read during body evaluation inside `.Title(selectedTitle.Value)`, the implicit `State<T>→Binding<T>` conversion consumes the property-read token from `StateManager.EndProperty()`. This means `selectedTitle` is tracked as a **binding** (not a global property), so changes to it do NOT trigger a full body rebuild — only the binding value updates silently. The platform view never gets notified.

## Decision

When a value can be derived from another state variable, derive it inline in `body()` rather than tracking it in a separate `State<T>`. This ensures the single state change triggers the rebuild with all correct values.

**Do:** `NavigationView(detail).Title(navItems[idx].Title)`
**Don't:** Use a separate `selectedTitle` state that you update after `selectedIndex`

## Impact

All team members writing view bodies should be aware: if you read `State<T>.Value` as an argument to an environment method like `.Title()`, `.Color()`, etc., the state is tracked as a binding, not a global property. Changing it won't rebuild the view.
