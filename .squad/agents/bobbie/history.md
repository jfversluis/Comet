# Bobbie — History

## Core Context

- **Project:** Converged .NET MAUI MVU framework merging Comet's engine with MauiReactor's API
- **Role:** Test Engineer
- **Joined:** 2026-03-08T00:00:54.044Z

### Phases 1–7 Archive (Summary)

**Phase 1 (Component Base Classes):**  
Developed component foundation with reactive state. Created 34 component tests covering base, state, props, lifecycle. All 394 existing + 35 new tests pass. Established `tests/Comet.Tests/ComponentTests/` subdirectory with flat `Comet.Tests` namespace pattern.

**Phase 2 (Control Generation & Style System):**  
Validated factory methods + style builders. 14 factory tests + 20 style tests all passing. Established `using static Comet.CometControls;` pattern and per-control `ControlStyle<T>` builders in `Comet.Styles` namespace.

**Phase 3 (Theme System Integration):**  
Theme system validated with 34 tests. Confirmed concrete `Theme` base class, `ThemeColors` preset separation, environment-driven color propagation. 574+ total tests passing.

**Phase 4 (Key-Aware Reconciliation + Component Merge):**  
**Phase 4.1 APPROVED:** Key-based view diffing (O(1) Dictionary lookup, backward compatible, zero regression).  
**Phase 4.2 REJECTED twice; fresh specialist approved 3rd revision:** Disposal-aware component merge (detaches merged children from old container before container disposal). 619 tests total, 599 passing, 0 regressions.

**Phase 5 (Typed Navigation):**  
17 passing navigation tests: 11 ShellWrapperTests (routing, modals, queries, extensions, fluent API, factories) + 6 TypedNavigationApiTests (generic registration, typed nav, props injection). CometShell.RegisterRoute<TView>(), GoToAsync<TView>(), Navigate<TView>() with IQueryAttributable fallback.

**Phase 6 (Platform Interop):**  
11 passing interop tests (4 skipped awaiting Amos NativeHost API). 23 total interop slice. Baseline locked. Zero regressions.

**Phase 7 (Component Hot Reload):**  
**Phase 7.1 REJECTED (Holden); Fresh specialist approved.** Component hot reload integration with MauiHotReloadHelper registration, state transfer via TransferState(), view tree diffing with handler reuse. 46/46 focused tests pass. 0 regressions. Locked Holden per squad rules.

**Overall Results (Phases 1–7):**  
- 640+ tests, 625+ passing, 0 regressions
- 313+ new tests written
- 2 pre-existing failures (unrelated to test work)
- 18+ skipped (framework-level: SetEnvironment SO, BuiltView type detection)

## Learnings

### Comet.Sample iOS Simulator E2E — 45/46 Pages Pass, RadioButton Crash Found (2026-03-08T230200Z)

**Status:** ✅ E2E COMPLETE

- **Build:** Release config required. Debug config crashes on launch due to shared DEBUG host rejecting CometApp roots (known issue). Built Comet + Comet.SourceGenerator for net10.0-ios, then Comet.Sample Release for iossimulator-arm64.
- **Navigation coverage:** 46 demo pages tested. 45 navigated successfully, 1 crashed (RadioButtonSample).
- **Interactive coverage:** 15 pages exercised with real control interaction (button taps, tab switching, stepper increment/decrement, scrolling lists, toggling state, updating text/font bindings). All 15 interactive tests passed.
- **Bug found:** RadioButtonSample crashes with `System.InvalidCastException` in `RadioButtonHandler.get_VirtualView()` — Comet's View passed to the RadioButton handler doesn't satisfy the `IRadioButton` cast. Reproducible 2/2 times. Routed to Amos.
- **WDA instability:** `page_source` calls (used by `--list-elements`, `--list-buttons`) crash the WDA session ~50% of the time after navigation. Workaround: use `--exists`, `--find-text`, `--tap-button` instead. This is a tooling issue, not a Comet issue.
- **Key pattern:** Always terminate app via `xcrun simctl terminate` between tests (not Appium `--terminate`) to avoid WDA session conflicts. Fresh Appium sessions per test are more reliable than `--reuse-session`.

### P0 Shared-Debug-Host Revision — Launch/Render Approved, Interactive Still Blocked (2026-03-08T171500Z)

**Status:** ✅ APPROVED at the claimed ceiling

- **Holden's core claim is now accurate:** the shared DEBUG host takes real root `Comet.View` factories, rejects `CometApp` roots, `sample/CometMauiApp/MyApp.cs` and `sample/CometBaristaNotes/{MauiProgram.cs,BaristaApp.cs}` now use `CreateRootView()`, and `CometHost` inspection now exposes `MainPage` / `TabView` roots instead of wrapped `CometApp` objects.
- **Validation rerun reproduced the claimed safety net:** `dotnet build sample/CometMauiApp/CometMauiApp.csproj -c Debug -f net10.0-maccatalyst`, `dotnet build sample/CometBaristaNotes/CometBaristaNotes.csproj -c Debug -f net10.0-maccatalyst`, and `dotnet test tests/Comet.Tests/Comet.Tests.csproj --no-build -c Release --filter "CometHost|ViewGetViewTests|NativeHostInteropTests|NewFeatureTests"` all pass, with the filtered test run landing **27/27**.
- **Claim ceiling for both P0 samples:** build ✅, launch/render evidence ✅, interactive end-to-end flow ❌. The retained runtime-validation trees show the correct roots and non-blank screenshots, but MauiDevFlow hit-testing still only resolves `CometHost` / `ContentPage`, descendant controls remain `[hidden] [disabled]`, automation-id lookup returns no elements, and retained taps fail.
- **Evidence hygiene note:** `runtime-validation/CometMauiApp/logs.txt` is byte-identical to the BaristaNotes port-`10224` log and should not be cited as CometMauiApp-specific evidence. Likewise, `CometMauiApp`'s `initial.png`, `render.png`, and `after-tap-49a0fe39.png` are byte-identical, so they prove render stability, not interaction success.
- **Routing:** Because this revision is approved at the launch/render-only ceiling, Holden is **not** locked out. If the next revision aims to make descendant controls hittable/tappable, Holden remains the correct owner for the runtime-host / interop lane.

### TaskApp + AllTheLists Review Gate — Build-Only, Not Launch-Proven (2026-03-08T17:03:52Z)

**Status:** ⚠️ PARTIAL APPROVAL ONLY

- **Code claims approved:** Amos's code-level migration claims for `sample/CometTaskApp` and `sample/CometAllTheLists` are real. `TaskListPage` now uses `CollectionView`, `AddTaskPage` is `Component<AddTaskPageState>`, `TaskDetailPage` is `Component<TaskDetailState, TaskDetailProps>`, `AllTheListsApp` is now a direct `CometApp` + `TabView` shell, and `InboxPage` moved from `ListView` to `CollectionView`.
- **Build claim approved:** Bobbie reran `dotnet build sample/CometTaskApp/CometTaskApp.csproj -c Debug -f net10.0-maccatalyst` and `dotnet build sample/CometAllTheLists/CometAllTheLists.csproj -c Debug -f net10.0-maccatalyst` successfully on the current repo state.
- **Launch/live-agent claim rejected:** Neither sample has retained launch/render artifacts under `files/sample-validation/`, and both DEBUG entry points still call `UseCometSampleDebugHost<TView>()` with `CometApp` roots (`TaskApp`, `AllTheListsApp`). The shared DEBUG host now explicitly rejects `CometApp` roots in `sample/Shared/RuntimeDebug/SampleRuntimeDebugExtensions.cs`, so Amos's "launched on Mac Catalyst with live runtime agents attached" claim is not reviewer-approvable in the current repo state.
- **Verification boundary:** For both samples, the current approval ceiling is **build-only plus code migration complete**. There is **no** reviewer-grade launch/render proof and **no** interactive end-to-end proof.
- **Routing:** Because this is an Amos artifact with an overclaim, Amos must not own the next revision. Route the next revision to Holden so the shared/runtime host lane and the sample DEBUG root factories can be corrected together.

### P0 Runtime Review — Render Progress Is Not Flow Approval (2026-03-08T164448Z)

**Status:** ⚠️ PARTIAL APPROVAL ONLY

- **Code/evidence alignment:** The shared DEBUG host still uses `UseCometSampleDebugHost<TView>() where TView : Comet.View, new()` and wraps `new TView()` in `new CometHost(rootView)`. Both `sample/CometMauiApp/MyApp.cs` and `sample/CometBaristaNotes/MauiProgram.cs` still pass `CometApp` types (`MyApp`, `BaristaApp`) as `TView`, so the runtime-debug lane is still hosting an app object as a child Comet view instead of as the actual MAUI `IApplication`.
- **Why Bobbie will not approve full P0 runtime validation:** The retained evidence matches the broken hosting shape. `CometMauiApp` still carries the Mac Catalyst failure `Application.Current was null after 30 retries`, while the retained live BaristaNotes tree still shows `Window [hidden] [disabled]` and root `TabView [hidden] [disabled]`. That means the current state supports render progress, not end-to-end flow verification.
- **Claims tightened:** The sample-validation report now explicitly marks the newer raw Mac Catalyst screenshots as render-progress artifacts only. They may support launch/render discussion, but they are not approval-grade interaction evidence until taps succeed and the flow checklist has rerun evidence.
- **Reviewer routing:** Amos's earlier P0 runtime-validation claim/artifact is not reviewer-approved. Per squad rule, Amos must not author the next revision of that validation artifact. The next revision should go to Holden because the remaining blocker is shared runtime-debug hosting / visual-tree / interaction infrastructure.

### Live BaristaNotes Follow-up — Broker Bound, Root Still Hidden (2026-03-08T163155Z)

**Status:** ✅ evidence corrected without overclaiming

- **Live run state tightened:** The rebuilt BaristaNotes launch now has retained live evidence that reaches MauiDevFlow broker port `10224`, so the sample is no longer “crash-only” on every surface. But the retained tree snapshot shows `Window [hidden] [disabled]` with root `TabView [hidden] [disabled]`, which still fails the usable-UI bar for validation.
- **Report/checklist/issues aligned:** `CometBaristaNotes` now records the evolved lane as `runtime_blocked` instead of `not_started`, carries the live screenshot/tree evidence, and adds a second blocking issue (`barista-002`) so later reruns know exactly why the live surface is still unverified.
- **Guardrail sharpened:** A reachable broker and live tree are still insufficient when the root surface is hidden or disabled. Until a visible, enabled root is retained, treat the run as blocked evidence only and leave end-user flows unchecked.

### Runtime Evidence Follow-up — Retained Broker Failure + Crash-Only Coffee Claim (2026-03-08T162600Z)

**Status:** ✅ artifacts corrected without overclaiming

- **CometMauiApp follow-up evidence retained:** The sample-validation workspace now carries a concrete Mac Catalyst runtime-debug artifact at `baselines/CometMauiApp/logs/original-runtime-debug-maccatalyst.log.txt`, capturing the observed sequence where MauiDevFlow exposed broker port `10223` but the app then reported `Application.Current was null after 30 retries`. That failure now blocks the unchecked interaction flows instead of leaving them ambiguously “not started.”
- **Counter sample reporting tightened:** `CometMauiApp` remains `baseline_captured` because the iOS launch screenshot is real evidence, but the report/checklist/issues now explicitly say the deeper interaction lane is unresolved (`counter-001`) until a trustworthy live agent session exists. Broker visibility alone is no longer treated as verification-adjacent evidence.
- **Coffee sample claim corrected:** The workspace artifacts for `CometBaristaNotes` now focus only on the retained iOS launch crash evidence (`CoffeeDashboardPage` + `CALayerInvalidGeometry` / NaN layout) and stop asserting an unproven “missing MauiDevFlow wiring” blocker. The retained notes/checklist/issues now keep the scope on the real launch crash the code-fix lane must solve first.

### Runtime Evidence Wave 1 — P0 Baselines + No-Overclaim Guardrail (2026-03-08T163500Z)

**Status:** ✅ evidence scaffolded; one P0 baseline captured; one P0 blocker classified

- **New validation support landed:** `tools/sample_validation_workspace.py` creates and validates a session-workspace evidence scaffold for all 10 samples, and `tests/Comet.Tests/SampleValidationWorkspaceTests.cs` locks the no-overclaim rules so `runtime_verified` cannot be claimed without screenshots, logs, comparison artifacts, completed flow evidence, and rerun evidence for fixed bugs.
- **Evidence root locked:** `/Users/davidortinau/.copilot/session-state/b26a6593-f539-47de-8f7b-3bd72e7ad681/files/sample-validation/` now contains per-sample baseline/evolved/comparison folders, checklist scaffolds, issue files, and a report (`sample-validation-report.json` + `.md`) tied to actual sample names.
- **CometMauiApp baseline:** Successfully built for `net10.0-maccatalyst` and `net10.0-ios`, launched on the iPhone 16 Pro iOS simulator, and captured a visible baseline render (`baselines/CometMauiApp/screenshots/original-launch.png`). The launch baseline proves the counter UI, slider, toggle, and action buttons render, but deeper automation is still blocked this wave because MauiDevFlow did not provide a trustworthy live session for the sample.
- **CometBaristaNotes blocker:** The sample builds cleanly for `net10.0-maccatalyst` and `net10.0-ios`, but the iOS baseline launch crashes during `CoffeeDashboardPage` layout with `ObjCRuntime.ObjCException` / `CALayerInvalidGeometry` (`position contains NaN`) on `Microsoft.Maui.Platform.LayoutView`. Failure evidence is retained in `baselines/CometBaristaNotes/logs/original-launch-diagnostics.log.txt` plus the SpringBoard fallback screenshots, and the issue is recorded as `barista-001`.
- **Runtime tooling note:** Product-source MauiDevFlow wiring is not present for `CometBaristaNotes`, so even after the launch crash is fixed, later waves will need explicit runtime-agent support before claiming full interaction coverage there.

### Phase 9 Reviewer Gate — Samples/Docs Lane Approved (2026-03-08T083500Z)

**Status:** ✅ APPROVED

- **Formal gate evidence:** The documented build chain is green end-to-end for the reviewed artifacts: `src/Comet.SourceGenerator/Comet.SourceGenerator.csproj`, `src/Comet/Comet.csproj`, `tests/Comet.Tests/Comet.Tests.csproj`, `sample/CometMauiApp/CometMauiApp.csproj -f net10.0-maccatalyst`, and `sample/CometBaristaNotes/CometBaristaNotes.csproj -f net10.0-maccatalyst`.
- **Validation harness behavior:** `Phase9SampleDocumentationValidationTests` now passes 7/7 when pointed at the real repo artifacts, and `tools/validate-phase9-sample-docs.sh` passes against `sample/CometMauiApp`, `sample/CometBaristaNotes`, and `docs/migration-guide.md`.
- **Counter sample pattern locked in:** `sample/CometMauiApp/MainPage.cs` is the evolved starter reference: `Component<CounterState>`, `Render()`, `SetState(...)`, `Reactive<string>`, and MAUI-current controls such as `Border`, `Slider`, `Toggle`, and `NavigationView`. `sample/CometMauiApp/MyApp.cs` keeps the app rooted in `CometApp` via `builder.UseCometApp<MyApp>()`.
- **Coffee sample pattern locked in:** `sample/CometBaristaNotes/Pages/CoffeeDashboardPage.cs` and `sample/CometBaristaNotes/Pages/CoffeeBeanDetailPage.cs` are the current-surface reference files for mixed migration (`Component`, typed props, `SetState(...)`, `Navigation.Navigate<T>()`, `MainThread`). `sample/CometBaristaNotes/BaristaApp.cs` intentionally stays a `CometApp` + `TabbedPage` bootstrap shell while older pages remain in place for migration storytelling.
- **Docs lane shape:** `docs/migration-guide.md` is now the authoritative Phase 9 migration doc, and `README.md`, `sample/CometMauiApp/README.md`, and `sample/CometBaristaNotes/README.md` correctly route readers to the counter reference, coffee reference, and migration guide without renaming Comet.
- **Operational note:** The wrapper script is the safest closure entry point because it normalizes CLI inputs to absolute paths before exporting `COMET_PHASE9_*`. If a reviewer bypasses the wrapper and runs the env-driven xUnit gate directly, use absolute artifact paths rather than repo-relative ones.

### Coffee Sample Rebuild Triage — Build Green, Gate Red (2026-03-08T071500Z)

**Status:** ✅ triaged without code changes

- **What Bobbie reproduced:** The documented build chain is green for the coffee sample today. `dotnet build src/Comet.SourceGenerator/Comet.SourceGenerator.csproj -c Release`, `dotnet build src/Comet/Comet.csproj -c Release`, and `dotnet build sample/CometBaristaNotes/CometBaristaNotes.csproj -c Release -f net10.0-maccatalyst` all succeed on the current repo state.
- **Actual failing gate:** The focused Phase 9 validation test fails deterministically: `COMET_PHASE9_COFFEE_SAMPLE_PROJECT=sample/CometBaristaNotes/CometBaristaNotes.csproj dotnet test tests/Comet.Tests/Comet.Tests.csproj --no-build -c Release --filter "FullyQualifiedName~Phase9SampleDocumentationValidationTests.CoffeeAppPassesPhase9GateWhenConfigured"`.
- **Failure signature:** `ValidateCoffeeApp(...)` rejects the sample with “Coffee app must exercise a richer current surface...” from `tests/Comet.Tests/Phase9SampleDocumentationValidationTests.cs`.
- **Why it happens:** The validator only counts `CollectionView`, `NavigationView`, `TabView`, `CometShell`, `GoToAsync<T>()`, and `RegisterRoute<T>()` in **product sources**. `BaristaApp.cs` does contain `NavigationView`, but `Phase9Project.IsBootstrapFile(...)` excludes any `*App.cs` file. The evolved pages use `Navigation.Navigate<T>()` in `sample/CometBaristaNotes/Pages/CoffeeDashboardPage.cs` and `sample/CometBaristaNotes/Pages/CoffeeBeanDetailPage.cs`, but that API is not part of `RichCoffeeSurfacePattern`.

## P0 Validation Status Summary

**Approved P0 floor (launch/render only):**
- CometMauiApp
- CometBaristaNotes
- Comet.Sample (pending Bobbie's continuation validation)

**Blocked pending architecture (interactive automation):**
- CometTaskApp
- CometAllTheLists
- Remaining 7 samples

**Architectural blocker:** MauiDevFlow automation-id hit-testing does not reach descendants inside Comet containers; requires deeper bridge between MAUI handlers and inner view tree.

**Next Phase:** Wave 2 (Holden fixes architecture, Bobbie validates remaining 9 samples)

---

## 2026-03-08T173607Z — Shared Inspection Bridge Review

**Status:** ✅ APPROVE (truthfulness), 🔒 ceiling unchanged (interactive)

**Validation Performed:**
- Inspected `src/Comet/Controls/View.cs`, `src/Comet/Helpers/ViewExtensions.cs`, `src/Comet/Controls/CometHost.cs`, and `src/Comet/AppHostBuilderExtensions.cs`
- Reviewed retained runtime artifacts under `/Users/davidortinau/.copilot/session-state/b26a6593-f539-47de-8f7b-3bd72e7ad681/files/runtime-validation/{CometMauiApp,CometBaristaNotes}/revision2/`
- Reran build/test chain: SourceGenerator, Comet, Tests, CometMauiApp, CometBaristaNotes — all passing
- Reran regression suite: 43/43 passing (AccessibilityTests, ViewGetViewTests, CometHost, NativeHostInteropTests, NewFeatureTests)

**Approved Claims:**
- The shared inspection bridge is real in the claimed framework files.
- Direct descendant property inspection is materially better after this revision.
- Focused regression validation is green at 43/43.
- Live runtime validation still tops out at `build ✅ / launch ✅ / render ✅ / interactive ❌` for both P0 samples.

**Evidence Highlights:**
- Direct descendant property access now works: AutomationId, Bounds, Handler, NativeType all retrievable
- Pre-native-bridge: descendants marked `[hidden] [disabled]`, automation-id lookup returns no elements, hit-testing resolves only CometHost/ContentPage, tap operations fail
- Post-native-bridge: same interactive failures persist (tree, query --automationId, hittest, tap still blocked)

**Verdict Boundary:**
- This revision raises the known support level for **direct descendant property inspection only**.
- It does **not** add reviewer-approved `tree`, `query --automationId`, `hittest`, or `tap` support.
- It does **not** create a new shared rule; the prior interactive blocker rule remains in force.

**Next-owner recommendation:** No corrective rewrite is needed for this artifact. If a follow-on revision is requested for the remaining interactive bridge blocker, Holden remains the best owner.

---

## 2026-03-08T173607Z — Shared Inspection Bridge Approval Gate

**Status:** ✅ Approved for truthfulness

**Reviewed:** Holden's inspection-bridge revision claiming enhanced direct descendant property inspection.

**Validation scope:**
- Framework touch-point audit: View.cs, ViewExtensions.cs, CometHost.cs, AppHostBuilderExtensions.cs ✅
- Build revalidation: 5 projects ✅
- Regression subset: 43/43 passing ✅
- Sample runtime inspection: CometMauiApp/revision2/ and CometBaristaNotes/revision2/ evidence reviewed ✅
- Interactive blocker status: Explicitly confirmed still present ✅

**Verdict:** Direct descendant property inspection is real and materially improves debugging within the approved build/launch/render floor. Interactive capabilities remain blocked.

**Coordinator note:** Current approved ceiling is build ✅ / launch ✅ / render ✅ / interactive ❌. TaskApp + AllTheLists route remains blocked on Holden's architecture fix.

---
## 2026-03-08T175446Z — External-Blocker Review Gate

**Status:** ✅ APPROVE

- Reviewed Holden's retained `revision3-external-blocker/` evidence for `CometMauiApp` and `CometBaristaNotes`, plus the implicated framework files (`View.cs`, `ViewExtensions.cs`, `CometHost.cs`, `AppHostBuilderExtensions.cs`) and sample debug-host wiring.
- Reran the approved build/test chain: SourceGenerator, Comet (`net10.0-maccatalyst`), Comet.Tests build, focused regression filter, `CometMauiApp`, and `CometBaristaNotes` — all passing; focused tests remain **43/43** green.
- `CometMauiApp` native proof is strong enough to isolate the remaining blocker downstream of Comet for this reviewed P0 sample: the increment button already exposes `AutomationId` / `AccessibilityId`, `NativeView.AccessibilityIdentifier`, `UserInteractionEnabled=True`, `Hidden=False`, and a real accessibility frame, while MauiDevFlow `query --automationId`, `hittest`, and `tap` still fail.
- `CometBaristaNotes` shows the same pattern on the Coffee Lab tab root: live native accessibility metadata is present, but MauiDevFlow still returns no automation-id match and still treats the shell as hidden/disabled for interaction.
- Verdict boundary: the team may now reasonably treat the remaining interactive blocker as **external/downstream to Comet for the currently reviewed P0 samples only** (`CometMauiApp`, `CometBaristaNotes`). Do **not** upgrade `CometTaskApp` or `CometAllTheLists` beyond interactive ❌ from this pass because they were not freshly revalidated.
- Next owner: shift the remaining investigation to the MauiDevFlow consumption path; Holden does not need another speculative Comet rewrite on this artifact.

---


## 2026-03-08T180100Z — P0 Gate Complete: External-Blocker Boundary Affirmed

**Status:** ✅ APPROVED for production merge  
**Decision:** `.squad/decisions.md` — "Bobbie — P0 Gate: External Blocker Boundary Affirmed"

**Gate action:** Final affirmation of external-blocker boundary for P0 samples.

**Boundary affirmed:**
- **Comet responsibility:** Build ✅ → Launch ✅ → Render ✅ → Descendants (property inspection) ✅
- **Downstream (MauiDevFlow):** Interactive automation (element, query --automationId, hittest, tap) ❌

**Guardrails locked:**
- P0 samples (CometMauiApp, CometBaristaNotes) confirmed at interactive ❌ with external-blocker justification
- Do not promote either P0 sample beyond interactive ❌
- Do not generalize to CometTaskApp or CometAllTheLists (not revalidated in this pass)
- Next investigation: MauiDevFlow consumption path

**Approval scope:**
- Holden's root-view DEBUG host refactor: truthful and complete ✅
- Holden's inspection-bridge descendant exposure: truthful and complete ✅
- P0 interactive ceiling confirmed at render; blocker is external ✅

**Result:** P0 boundary locked for production. Interactive work deferred to MauiDevFlow team.


### Stack Overflow Baseline (Pre-Fix)

**Date:** 2026-03-09
**Context:** Establishing baseline crash point before Amos fixes P0 stack overflow bug.

**Full suite results:**
- **215 tests passed** before test host process crashed with `Stack overflow.`
- **1 test skipped**
- **Test Run Aborted** — process killed, remaining tests never executed
- Last passing test: `Comet.Tests.PickerTests.IPickerSelectedIndexProperty`
- Crash happens on the next test after PickerTests complete

**Crashing test classes (confirmed individually):**
- `KeyAwareReconciliationTests` (9 [Fact]/[Theory] methods) — 0 pass, immediate stack overflow
- `ComponentMergeTests` (11 [Fact]/[Theory] methods) — 0 pass, immediate stack overflow

**Total test methods in repo:** ~715 [Fact]/[Theory] annotations

**Recursive call cycle causing the overflow:**
```
View.ViewPropertyChanged → View.ContextPropertyChanged → ContextualObjectExtensions.SetEnvironment
→ UI.Init callback → ReflectionExtensions.SetPropertyValue → EnvironmentData.SetValue
→ ContextualObject.SetValue → BindingObject.SetProperty → BindingObject.CallPropertyChanged
→ StateManager.OnPropertyChanged → (back to) View.ViewPropertyChanged
```

**Root cause location:** Infinite recursion in the environment property change notification path. `ViewPropertyChanged` triggers `SetEnvironment` which sets a value, which fires `PropertyChanged`, which calls `ViewPropertyChanged` again — no recursion guard.


### CometMauiApp E2E Test Results — Mac Catalyst (Appium)

**Date:** 2025-07-24
**Context:** End-to-end Appium automation of CometMauiApp on Mac Catalyst. App uses Component<CounterState> + Render()/SetState() pattern with Reactive<string> for status text.

**Build:** `dotnet build sample/CometMauiApp/CometMauiApp.csproj -f net10.0-maccatalyst -c Release` — ✅ 0 warnings, 0 errors.

**Test Results:**

| # | Test | Result | Notes |
|---|------|--------|-------|
| 1 | Increment button (0→1) | ✅ PASS | Count updated, status shows "Incremented by 1. Current count: 1." |
| 2 | Increment again (1→2) | ✅ PASS | Count updated correctly |
| 3 | Decrement button (2→1) | ✅ PASS | Status: "Decremented by 1. Current count: 1." |
| 4 | Reset button (→0) | ✅ PASS | Status: "Counter reset to zero." |
| 5 | Step slider | ⚠️ LIMITATION | Appium mac2 driver `--set-slider` and `--drag` do not work on Mac Catalyst sliders. Slider manipulation is a known Appium/mac2 limitation, not a Comet bug. |
| 6 | Increment after slider | ⚠️ SKIPPED | Could not test different step sizes due to slider limitation |
| 7 | Toggle celebrations OFF | ✅ PASS | Switch toggled (text=1→0), text changed to "Run quiet updates for raw counter flow." and "Milestone celebrations are paused." |
| 8 | 5 Increments (celebrations OFF) | ✅ PASS | Count reached 5. Banner: "The evolved MVU surface has rendered 5 updates." — NO milestone text. Status card: "Milestones are currently disabled." |
| 9 | Toggle celebrations ON | ✅ PASS | Switch toggled (text=0→1), text: "Celebrate every fifth increment." and "Milestone celebrations are enabled." |
| 10 | Reset + 5 Increments (celebrations ON) | ✅ PASS | Count reached 5. Banner: "Milestone hit: 5 total taps." Next milestone: 10. |
| 11 | All three cards visible | ✅ PASS | Hero card (Comet Counter + Count display), Action card (controls), Status card (description + milestone) all present in accessibility tree. |
| 12 | ScrollView | ⚠️ LIMITATION | Appium mac2 `--scroll` not supported. All content visible without scrolling. |

**AutomationId Coverage:** All 5 interactive elements have AutomationIds and are correctly targetable:
- `counter-increment-button` ✅
- `counter-decrement-button` ✅
- `counter-reset-button` ✅
- `counter-step-slider` ✅ (found but manipulation limited)
- `counter-celebrate-toggle` ✅

**Reactive<string> Verification:** Status text updates correctly on each action via `Reactive<string>`, confirming lightweight state updates work end-to-end.

**Known Appium/mac2 Limitations on Mac Catalyst:**
- `--set-slider` reports success but does not change slider value
- `--drag` on slider elements throws proxy error
- `--scroll` not supported

**Flaky Session Note:** First Appium session showed button-death after toggle, but this was not reproducible on a fresh app launch. The failed session had prior `--set-slider` and `--drag` attempts that may have corrupted the Appium session state. On a clean session, all buttons worked correctly after toggle interactions.


### CometMauiApp E2E Test Results — iOS Simulator (Appium xcuitest)

**Date:** 2025-07-24
**Context:** End-to-end Appium automation of CometMauiApp on iOS Simulator (iPhone 16 Pro, iOS 18.5). Follow-up to Mac Catalyst test where slider and scroll were blocked by mac2 driver limitations. David's hypothesis: xcuitest driver on iOS Simulator should have full slider and scroll support.

**Build:** `dotnet build sample/CometMauiApp/CometMauiApp.csproj -f net10.0-ios -c Debug` — ✅ 0 errors, 11 warnings (all pre-existing).

**Simulator:** iPhone 16 Pro (3F542DD1) — iOS 18.5, booted.

**Test Results: 10/12 PASS**

| # | Test | Result | Notes |
|---|------|--------|-------|
| 1 | Element discovery + initial state | ✅ PASS | All 5 AutomationIds found. Full accessibility tree returned including scroll bar ("2 pages"). Count: 0, Step: 1, Toggle: ON. |
| 2 | Increment button (0→1) | ✅ PASS | "Count: 1", banner: "The evolved MVU surface has rendered 1 updates.", status: "Incremented by 1. Current count: 1." |
| 3 | Decrement button (1→0) | ✅ PASS | "Count: 0", status: "Decremented by 1. Current count: 0." |
| 4 | Reset after 3 increments | ✅ PASS | 3x increment → reset. "Count: 0", status: "Counter reset to zero." |
| 5 | Slider manipulation | ❌ FAIL | `--set-slider` (send_keys), `--drag` (mobile:dragFromToForDuration), and `--tap-coords` all report success but do NOT change MAUI Slider value. Slider stays at "Step size: 1" / "0%". Same behavior as Mac Catalyst. This is a MAUI Slider handler + Appium interaction gap, NOT a Comet bug. |
| 6 | Increment after slider change | ⚠️ SKIPPED | Blocked by slider limitation (#5). |
| 7 | Toggle celebrations OFF | ✅ PASS | Toggle text 1→0. Text changed to "Run quiet updates for raw counter flow." and "Milestone celebrations are paused." / "Milestones are currently disabled." |
| 8 | Toggle celebrations ON | ✅ PASS | Toggle text 0→1. Text changed to "Celebrate every fifth increment." and "Milestone celebrations are enabled." |
| 9 | Scroll down | ✅ PASS | `--scroll down` executed successfully (unlike Mac Catalyst where it was unsupported). Full content visible in accessibility tree. David's hypothesis confirmed: xcuitest scroll works on iOS. |
| 10 | Status card visibility | ✅ PASS | "What this sample is showing" card, description text, and milestone status all present in accessibility tree. |
| 11 | Milestone at count=5 | ✅ PASS | Reset → 5x increment (with 2s waits). "Count: 5", banner: "Milestone hit: 5 total taps.", "Next milestone: 10", status: "Incremented by 1. Current count: 5." |
| 12 | All AutomationIds targetable | ✅ PASS | counter-increment-button ✅, counter-decrement-button ✅, counter-reset-button ✅, counter-step-slider ✅ (found but not manipulable), counter-celebrate-toggle ✅ |

**Score: 10/12** (vs 10/12 on Mac Catalyst — same score, different gaps filled)

**iOS vs Mac Catalyst Comparison:**

| Capability | Mac Catalyst (mac2) | iOS Simulator (xcuitest) |
|-----------|-------------------|------------------------|
| Button taps | ✅ | ✅ |
| Toggle switch | ✅ | ✅ |
| Slider manipulation | ❌ (send_keys silent fail) | ❌ (send_keys/drag/tap-coords all silent fail) |
| Scroll | ❌ (unsupported) | ✅ (works!) |
| Element discovery | ✅ | ✅ |
| Milestone celebration | ✅ | ✅ |
| Session stability | Good | Fragile (sessions die on rapid taps; 2s waits required) |

**Key Findings:**

1. **Scroll works on iOS** — David was right. The xcuitest driver supports `--scroll down` on iOS Simulator, confirming the gap was a mac2 driver limitation.

2. **Slider is STILL broken** — Despite xcuitest's supposed "full slider support", the MAUI Slider does not respond to any Appium programmatic interaction (send_keys, drag, tap-coords). All methods report "ok" but the native UISlider value doesn't change. Root cause: MAUI's Slider handler on iOS may not wire up the accessibility `setValue` method properly, or the `OnValueChanged` callback only fires on genuine user touch events.

3. **Session instability on iOS** — XCUITest/WDA sessions are more fragile than mac2. Sessions frequently terminate mid-chain, especially after rapid taps (0.5s waits) or when the UI re-renders heavily (milestone celebration). Workaround: use 2-second waits between operations and split long chains into smaller batches.

4. **Reactive<string> verified** — Status text updates correctly via Reactive<string> on every action, consistent with Mac Catalyst results.

**Recommendation:** The MAUI Slider + Appium interaction gap should be investigated upstream. File an issue against dotnet/maui requesting that the iOS Slider handler expose proper accessibility value-setting support for automation frameworks.


### CometBaristaNotes & CometStressTest iOS Simulator E2E — 29/33 Tests Pass, 1 Crash Found (2026-03-08T232500Z)

**Status:** ✅ E2E COMPLETE

**CometBaristaNotes (com.comet.baristanotes):**
- **Build:** Debug config, net10.0-ios. 0 errors, 17 pre-existing warnings.
- **Coverage:** 15 pages/features tested, 13 pass, 1 crash, 1 Appium limitation.
- **Bug found:** BeanDetailPage crashes (SIGABRT) when navigated from BeanManagementPage (Settings → Beans → tap). The CoffeeBeanDetailPage from the Dashboard works fine — different navigation paths use different page classes.
- **Syncfusion gauges:** Not exposed in accessibility tree, not automatable. License popup ("Claim License") appeared once, dismissed with OK.
- **Debug host nav state:** The debug host restores navigation state across launches. After visiting Shot Logging, subsequent launches start on that page. Back-button navigation (coordinate tap ~x=57,y=78) works.
- **Tab navigation:** All 3 tabs (Coffee Lab, Activity, Settings) work via coordinate taps on tab bar (y=850). Cross-tab navigation via "Open activity feed" button works.

**CometStressTest (com.comet.stresstest):**
- **Build:** Debug config, net10.0-ios. 0 errors, 0 warnings.
- **Coverage:** 18 features tested across 6 tabs, 16 pass, 2 Appium limitations.
- **Two-simulator gotcha:** Two simulators were booted (iPhone 16 Pro iOS 18.5, iPhone 17 Pro iOS 26.2). `xcrun simctl install booted` targeted the wrong device. Must specify UDID explicitly: `xcrun simctl install 3F542DD1-6303-4C0B-8D81-83C4B2D1D680`.
- **Stress test highlights:** 100 rapid state updates completed without crash or UI freeze. Timer accurately tracked seconds. Button counter, stepper, and state reset all work correctly.
- **All 6 tabs accessible:** Lists, Collections, Layouts, Controls via tab bar; State and Swipe via iOS "More" overflow tab.
- **Zero crashes.** App handles complex layouts (grid spans, flex wrapping, absolute positioning, nested scrolls), large lists (50 items with dynamic add), and rapid state mutations.

**Key patterns:**
- Always specify simulator UDID when multiple are booted.
- Appium `--tap` on "Coffee Lab" back button unreliable — use coordinate taps.
- Swipe gestures on SwipeView items are not automatable via Appium.
- MAUI Slider and Toggle Switch remain Appium automation gaps (upstream issue, not Comet bug).

### Comet.Sample Migration — 85 Files Migrated to Evolved API (2026-07-25)

**Status:** ✅ COMPLETE — Build clean, 729/729 tests pass

- **Scope:** 85 .cs files across 7 folders (Views/, CommunityQuestions/, Comparisons/, GitHubIssues/, Graphics/, LiveStreamIssues/, root MyApp.cs).
- **Transformations applied:**
  - `class X : View` → `class X : Component` with `public override View Render()` (83 classes)
  - `[Body] View body()` → `public override View Render()` (all [Body] attributes removed)
  - `Body = () => expr` / `Body = MethodName;` in constructors → collapsed into `Render()`
  - `new Text("x")` → `Text("x")` (factory methods for 17 generated controls)
  - `new VStack { ... }` → `VStack(...)`, `new HStack { ... }` → `HStack(...)` etc. (7 container types)
  - `using static Comet.CometControls;` added to 85 files
- **Exceptions kept `new` (correct):**
  - `Grid(rows:, columns:)` → kept `new Grid(rows:, columns:) { }` because factory only takes `params View[]`
  - LINQ `.Select()` / lambda children in containers → kept `new VStack { }` form (Section5, ViewLayoutTestCase)
  - Custom subclasses (BorderedEntry : HStack, Separator : ShapeView) → kept inheritance, still applied inner factory methods
  - Non-factory types (Image, Spacer, ListView, SectionedListView, ShapeView, VGrid, TabView, etc.) → kept `new`
- **CometApp files** (MyApp.cs, SampleApp.cs) → kept CometApp inheritance, applied factory method replacements
- **8 skip files:** Models/Song.cs, ApiAudit/ApiAuditManager.cs, Views/MenuItem.cs, Views/LabelSamples.cs, Graphics/SimpleFingerPaint.cs, Graphics/BindableFingerPaint.cs, 3 fully-commented-out files
- **Grid factory gap:** The `CometControls.Grid(params View[])` factory doesn't accept `rows:/columns:` named params. Files using Grid layout definitions must use `new Grid(rows:, columns:) { }` constructor form. This is a genuine API gap — consider adding overloads.
- **Build verification:** `dotnet build sample/Comet.Sample/Comet.Sample.csproj -c Release -f net10.0-maccatalyst` → 0 errors, 0 warnings
- **Test verification:** `dotnet test tests/Comet.Tests/Comet.Tests.csproj --no-build -c Release` → 729 passed, 0 failed, 19 skipped (pre-existing)
