---
name: "runtime-evidence-no-overclaim"
description: "How Bobbie captures sample runtime evidence without claiming more verification than the retained artifacts support."
domain: "testing"
confidence: "high"
source: "earned"
---

## Context

Use this when validating Comet samples through real runtime launches and you need report-ready evidence that later waves can trust.

## Patterns

### Separate build truth from runtime truth
- A successful build is useful evidence, but it is not runtime verification.
- Record build logs, then independently prove whether the sample rendered, blocked, or crashed at runtime.

### Use three explicit runtime states
- `baseline_captured` — a visible baseline UI (or failure-state capture) exists.
- `runtime_blocked` — launch/interaction is blocked, but the blocker is precisely captured.
- `runtime_verified` — only after screenshots, logs, comparison artifacts, completed flow evidence, and rerun evidence for fixed bugs are all retained.

### Keep evidence outside the repo
- Store screenshots, logs, notes, and comparisons in the session workspace under a stable `sample-validation/` root.
- Use per-sample folders for `baselines`, `evolved`, `comparisons`, `issues`, and `checklists` so later waves can resume without losing fidelity.

### Capture the first useful failure, not just the first failure string
- If a sample crashes, keep the launch screenshot, simulator/app logs, and the crash signature together.
- Example: `CometBaristaNotes` looked like a launch miss until the simulator log showed `ObjCRuntime.ObjCException` / `CALayerInvalidGeometry` with a NaN layout position.

### Treat missing runtime automation as a blocker
- If MauiDevFlow/Appium wiring is missing, stale, or unreachable, record that explicitly instead of pretending the untouched flows are verified.
- A broker entry that cannot be reached is not trustworthy evidence.

### Fixed bugs require rerun evidence
- Discovery evidence alone is not enough.
- If an issue is marked fixed, retain post-fix logs/screenshots and attach them to the same issue entry before promoting the sample to `runtime_verified`.

## Examples

- **CometMauiApp Wave 1:** baseline render captured on iOS simulator; interaction coverage left open because no trustworthy live MauiDevFlow session was available.
- **CometBaristaNotes Wave 1:** build green, runtime blocked; issue recorded with SpringBoard fallback screenshot plus crash log showing `CALayerInvalidGeometry`.

## Anti-Patterns

- Calling a sample “verified” because it builds.
- Dropping blocker details into chat only, with no retained evidence path.
- Marking a bug fixed before rerun evidence exists.
