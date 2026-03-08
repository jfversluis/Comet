# Naomi — History

## Core Context

- **Project:** Converged .NET MAUI MVU framework merging Comet's engine with MauiReactor's API
- **Role:** Source Generator Dev
- **Joined:** 2026-03-08T00:00:54.044Z

## Learnings

### Phase 8.1 Kickoff — IView Controls Expansion (2026-03-08T050835Z)

**Status:** ⚙️ **IN PROGRESS — PHASE 8 KICKOFF**

**Assignment:** Phase 8.1 — Additional IView-generated controls

**Scope:**
- Implement missing/partial IView-generated controls
- Use existing `CometViewSourceGenerator` + `[CometGenerate]` attribute framework
- Focus on control inventory expansion (volume + API surface coverage)

**Dependencies:**
- ✅ Phase 7 approved and closed (hot reload integration stable)
- ✅ Phase 1–6 complete (640+ tests, 625+ passing, 0 regressions)
- ✅ Source generator production-ready (phases 1–3 built controls using this pipeline)

**Parallel Work:**
- Amos Phase 8.2 (handwritten complex controls)
- Bobbie Phase 8.3 (coverage tests & reviewer gate)

**Next Steps:**
1. Build control inventory (identify missing/partial controls)
2. Design additional IView surface expansion
3. Implement with full test coverage
4. Submit to Bobbie for Phase 8.3 integration

**Key Context from Prior Phases:**
- Source generator: `CometViewSourceGenerator.cs`, templates, `[CometGenerate]` attribute
- Factory pattern: `CometControls` partial static class, `using static` import
- Style builders: `{Control}StyleBuilder` classes in `Comet.Styles` namespace
- Test convention: Control tests in subdirectory, flat namespace (existing pattern)

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

### Phase 2.3 — Style Builder Generation (2026-03-08)

**What was done:**
- Enhanced `CometViewSourceGenerator` to produce `{Control}StyleBuilder` classes for each generated control
- 19 style builders generated (Button, Text, TextField, Slider, Toggle, CheckBox, etc.)
- Each builder wraps `ControlStyle<T>` with fluent setters matching the control's environment keys
- Common methods on every builder: `Background(Paint)`, `TextColor(Color)` — map to `EnvironmentKeys.Colors.Background` and `EnvironmentKeys.Colors.Color`
- Per-control methods generated from non-Action, non-Skip extension properties using `nameof(IInterface.Property)` as the environment key
- Implicit operator to `ControlStyle<T>` so builders can be passed directly to `Theme.SetControlStyle()`
- Style builders live in `Comet.Styles` namespace

**Key files:**
- `src/Comet.SourceGenerator/CometViewSourceGenerator.cs` — templates: `styleBuilderMustacheTemplate`, `styleBuilderPropertyMustache`; model: `StyleProperties`, `StylePropertyFunc`; Execute: style builder source generation
- Generated output: `{Control}StyleBuilder.g.cs` for each control

**Architecture decisions:**
- Common Background/TextColor methods on every builder since those EnvironmentKeys apply to all views
- Action-type properties (events/callbacks) excluded from builders — not meaningful for styling
- Properties already covered by common methods (Background, Color, TextColor) filtered from per-control generation to avoid duplicates
- Uses same `nameof(FullName)` pattern as existing extension methods for consistent environment key resolution
- Purely additive — no modifications to existing generated output

**Test suite status:** 578 passed + 2 pre-existing hot reload failures + 15 skipped = 595 total. No regressions.

### Phase 3 Complete (2026-03-08T010500Z)

**Status:** Phase 3 (Theme System with Style Builders) implementation complete. Style builders fully integrated with Amos's theme control styling system. All 578 tests passing (2 pre-existing hot reload failures, 15 skipped, 595 total). Build clean.

**Deliverables:**
- 19 `{Control}StyleBuilder.g.cs` files generated with fluent API
- Full implicit conversion to `ControlStyle<T>`
- Seamless Theme integration (Amos uses builders for themed defaults)
- Zero regressions

**Next:** Phase 4 (Reconciliation Upgrade) — pending Coordinator decision.

