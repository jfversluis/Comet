---
name: "evolved-sample-migrations"
description: "How Amos updates Comet samples/docs to teach the evolved component-first MVU surface without rewriting the whole repo."
domain: "samples-docs"
confidence: "high"
source: "earned"
---

## Context

Use this when a Comet feature needs sample and documentation coverage, especially when the new API is additive and older surfaces still exist in the same repository.

## Patterns

### Reuse the lightest credible sample first
- If the goal is a starter/reference sample, prefer evolving `sample/CometMauiApp` instead of creating a new project.
- Keep `CometApp` as the app root; show the evolved API at the page/component layer where `UseCometApp<TApp>()` already expects it.
- When a reusable starter pattern stabilizes, mirror it into `templates/single-project/` so future sample/page migrations can copy from the template instead of re-deriving `Component<TState>` + `Render()` + `SetState(...)` from older `[Body]` files.

### Pair a green-path sample with an incremental-migration sample
- Use one minimal sample to show the clean happy path (`Component<TState>`, `Render()`, `SetState(...)`).
- Use one richer sample to prove coexistence with older pages, services, and interop (`sample/CometBaristaNotes` is the preferred host today).

### Prefer runtime-proven tab shells in samples
- Use `TabView` for sample tab layouts unless `TabbedPage` handler wiring is known to be active on the target runtime.
- Wrap each tab root in `NavigationView` when you still need typed in-tab navigation flows.

### Teach typed navigation with real props
- Prefer `Navigation.Navigate<TView>(props)` or typed shell routes in the richer sample.
- When showing detail pages, use `Component<TState, TProps>` so navigation data is explicit and reviewable.

### Keep older references, but redirect readers
- Do not delete legacy `[Body]` samples just because a newer surface exists.
- Add concise notes in older sample files/READMEs pointing to the current reference sample and migration guide.

### Preserve MAUI-current guidance in touched samples
- Avoid `Compatibility` APIs in refreshed sample code.
- Prefer MAUI 10-safe controls (`Border`, async dialog APIs, `MainThread`, etc.).
- If reused sample code emits warnings directly related to current-platform guidance, clean them while you are already in that sample.

### Validate in repository order
- Build in the documented sequence:
  1. `dotnet build src/Comet.SourceGenerator/Comet.SourceGenerator.csproj -c Release`
  2. `dotnet build src/Comet/Comet.csproj -c Release`
  3. `dotnet build tests/Comet.Tests/Comet.Tests.csproj -c Release`
  4. build the touched sample projects
  5. run the safest focused tests that exercise the evolved surface without confusing historical framework noise for regressions

## Examples

- **Phase 9 counter sample:** evolve `sample/CometMauiApp` into a counter page backed by `Component<CounterState>` and `Reactive<string>`.
- **Phase 9 coffee sample:** keep `ShotLoggingPage` intact, add a `CoffeeDashboardPage` and a typed `CoffeeBeanDetailPage`, then document the mixed-surface migration story.

## Anti-Patterns

- Creating a brand-new sample when an existing sample can communicate the feature with less maintenance.
- Rewriting legacy sample pages just to make everything uniform when coexistence is the lesson.
- Documenting the evolved API as if the project/package must be renamed; Comet stays Comet.
