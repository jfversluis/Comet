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

