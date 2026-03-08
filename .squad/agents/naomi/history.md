# Naomi — History

## Core Context

- **Project:** Converged .NET MAUI MVU framework merging Comet's engine with MauiReactor's API
- **Role:** Source Generator Dev
- **Joined:** 2026-03-08T00:00:54.044Z

## Learnings

<!-- Append learnings below -->

### Phase 2.1 & 2.2 — Factory Methods + Extension Enhancement (2026-03-08)

**What was done:**
- Enhanced `CometViewSourceGenerator` to produce static factory methods for all generated controls
- Factory methods live in `Comet.CometControls` partial static class — importable via `using static Comet.CometControls;`
- Each control gets: Binding<T> variant, Func<T> variant (when non-Action params exist), parameterless overload
- Added "On" prefixed aliases for Action-type extension properties (e.g., `.OnPressed()`, `.OnReleased()`)
- 14 new tests in `FactoryMethodTests.cs`, all green

**Key files:**
- `src/Comet.SourceGenerator/CometViewSourceGenerator.cs` — templates: `factoryMustacheTemplate`, `onPrefixedExtensionActionProperty`; model: `ParameterNamesFunction`; Execute: factory source generation
- `tests/Comet.Tests/FactoryMethodTests.cs` — test coverage for factory methods and OnX aliases

**Architecture decisions:**
- Factory class is `CometControls` not `Component` — keeps static factories decoupled from the component base class, allowing `using static` from any context
- Reused `FuncConstructorFunction` lambda for factory Func variants — same conditional logic (only generate when HasParameters is true)
- "On" prefix only on Action-type extension properties — non-Action properties already have clean names
- `Binding<T>` has implicit conversion from `T`, so raw-value extension overloads aren't needed

**Generator patterns learned:**
- Stubble Mustache lambdas: section content between `{{#Func}}...{{/Func}}` is passed as `str` parameter; the lambda renders it with model data
- `ParametersFunction` joins rendered per-parameter templates with commas
- Source generator output files appear at `obj/{Config}/{TFM}/generated/{GeneratorAssembly}/{GeneratorClass}/`
- Need `EmitCompilerGeneratedFiles=true` build property to see generated files on disk

### Phase 2 Complete — Phase 2.1/2.2 (2026-03-08T004600Z)

**Status:** Phase 2.1 and 2.2 implementation complete. All 14 factory method tests passing. Generator now produces:
- Static factory methods in `Comet.CometControls` for all 20+ generated controls
- Binding<T>, Func<T> (parameterized only), and parameterless overloads
- "On" prefixed extension aliases for Action-type properties
- Factory pattern fully integrated with fluent extension API

**Test suite status:** 458 baseline tests green + 14 new factory tests = 472 total (Phase 1 + Phase 2.1-2.2).

**Next:** Phase 2.4 (Bobbie) will add regression and integration tests for the new factory API.

