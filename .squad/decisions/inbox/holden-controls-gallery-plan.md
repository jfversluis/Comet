# Decision: CometControlsGallery Plan

**Author:** Holden  
**Date:** 2025-07-18  
**Status:** Proposed  

## Context

David requested a comprehensive sample app exercising all Comet controls and the style/theme system, inspired by Redth's mauiplatforms sample. A draft plan existed but needed architectural review and refinement.

## Decision

The refined plan lives at `.squad/controls-gallery-plan.md`. Key architectural decisions:

1. **CometApp + TabView** (not Shell) — pure Comet, no MAUI bridging
2. **4 tabs** (Controls, Layouts, Lists, Theme) — dropped DeviceInfo, merged theme sub-pages
3. **Component<TState>** for stateful pages, plain View for static
4. **One SectionCard modifier** — not three separate modifier classes
5. **GraphicsView deferred** — canvas drawing is a separate concern from controls/styles
6. **Full control coverage** including SecureField, CheckBox, RadioGroup, DatePicker, TimePicker, Stepper, SearchBar

## Work Routing

- **Amos**: All page implementation (scaffold → controls → theme → layouts → lists)
- **Bobbie**: Build verification and runtime validation
- **Naomi**: Not needed unless generator bugs surface
- **Holden**: Architecture review, decision authority

## Impact

All team members should read `.squad/controls-gallery-plan.md` before starting work on the CometControlsGallery sample.
