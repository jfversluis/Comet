---
name: "anticipatory-phase-gates"
description: "How Bobbie writes forward-looking tests that lock today's baseline while leaving clean gates for a parallel implementation."
domain: "testing"
confidence: "high"
source: "earned"
---

## Context

Use this when implementation and test work are happening in parallel. The goal is to protect today’s behavior, document tomorrow’s acceptance criteria, and avoid turning known historical noise into misleading regressions.

## Patterns

### Split baseline tests from implementation gates
- Keep runnable tests active only when they validate behavior that already works today.
- Encode future API/behavior as `[Fact(Skip = "...")]` gates with precise reasons tied to the implementation phase.

### Preserve historical noise explicitly
- Do not silence long-standing failures by widening skips or weakening assertions.
- Run a focused slice that still reproduces the known failure signature so reviewers can tell “same old noise” from “new regression.”

### Promote newly discovered blockers carefully
- If a new gate exposes a tightly-coupled framework defect (for example, a recursion bug in the intended transfer path), capture it as a targeted skip reason instead of letting the whole suite fail before the implementation owner can respond.
- Document the blocker in squad history/decision artifacts so the implementer knows exactly what to fix.

### Validate in repo order
- For Comet, build in this order:
  1. `dotnet build src/Comet.SourceGenerator/Comet.SourceGenerator.csproj -c Release`
  2. `dotnet build src/Comet/Comet.csproj -c Release`
  3. `dotnet build tests/Comet.Tests/Comet.Tests.csproj -c Release`
  4. `dotnet test tests/Comet.Tests/Comet.Tests.csproj --no-build -c Release ...`
- If ref-assembly file locks appear during validation, retry serialized with `/m:1` before treating it as a product issue.

## Examples

- **Phase 7.2 hot reload:** Keep the historical plain-view hot reload failures active, add one runnable Component state-transfer baseline, and leave skipped gates for Component replacement/state+props/nested hot reload until Holden lands Phase 7.1.

## Anti-Patterns

- Converting historical failures into broad skips just to get a green run.
- Writing only skipped tests, leaving no runnable baseline coverage.
- Leaving skip reasons vague (“not working”) instead of naming the missing phase or blocked transfer path.
