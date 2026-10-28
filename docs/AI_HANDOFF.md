# Current canonical batch —0.0.62 / SDK12 / M20 in progress

Current review: **0.0.62 / SDK12**. All311 frozen compiled source contracts match. Attributed acceptance214/311:184 behavior and30 compile-error obsolete contracts. Transform81/81 and56 component-query overloads are accepted within the recorded bounded profiles. Native885 core+240 vectors+348 poses+476 query comparisons pass; the476-query demo passes Development/effective-AOT with exact Stop restoration, and Node486/486 passes. All six browser suites pass in both modes:267 core+271 asset/lifecycle/hierarchy+77 object metadata/index+240 vectors+348 poses+476 queries, with exact Stop restoration. M20 remains open with97 contracts pending; source matches and test counts are not full Unity parity.

The exact Unity6000.3.27f1 Play oracle and both real Wasm modes run the same137-check async lifecycle corpus, with hashes of actual executed source files. Async standalone Mesh/TextAsset requests follow native rejection, while synchronous asset clones have independent identities/names/lifetimes and shared immutable geometry. Native740 core+240 vectors+348 poses, Node486/486; obsolete consumers53 in native Unity and owned native/Wasm compilers. Thirty error:true declarations are certified compiler contracts, not executable runtime APIs. Remaining153 records need attributed behavior/dependency acceptance.

The version-owned Asset Lifetime Lab has17 asset,137 async and54 hierarchy checks. Vector Transforms Lab adds240 comparisons for24 overloads; Pose Transforms Lab adds348 for36 contracts. All four demos are available through Platform Load Demos. Stop restores authoring state. The M18.3 grounding regression was a real stale physics local-transform/matrix bug, now fixed and locally verified. CI routing for hierarchy/runtime asset imports is repaired; final-head CI/public artifact acceptance still pending. No0.0.62 release or stable merge exists. Do not close M20 or change its311-record denominator.


---

# M20.3a transform behavior — Engine 0.0.61 / SDK11

M20 remains **in progress**. This batch fixes host publication of descendant transforms, atomic pose/reparent rejection and adds tested legacy Euler/axis/orbit/look/direction/span APIs. 138 native observations and 32 lifecycle + 19 transform development-Wasm observations pass, including next-frame consistency and exact Stop restoration. The required unadapted UnityEngine corpus fails CS0246; all 311 assigned records and 0 certified exact signatures remain visible.

Full Transform matrices/shear/zero-scale local state, canonical namespace/float interoperation, async and package profiles, message/tag/obsolete overloads and managed/navigation clone dependencies remain required. See [partial report](reports/M20_TRANSFORM_BATCH.md). Stable main remains 0.0.57. No automatic merge or milestone closure. Older sections below retain historical versions.

---

# Version-owned demo integration — Engine0.0.60 new experimental build

Optional IPC1 capability `project.demos` plus a manifest demo catalog enable Platform0.1.11 Load Demos. This build declares only M20 Object Lifecycle Lab; generation requires a fresh authorized destination and explicit declared IDs, returns native project paths and replays operation receipts. The original public0.0.60 artifact is immutable. Math Lab demos are deferred. SDK10, native formats1 and M20.2 behavior remain unchanged; this does not close M20 or add Unity signatures. [Guide](../demos/M20_GUIDE.md).

# Integration handoff — Engine0.0.60 / Platform0.1.9 SDK

Public provider pin d9561e94d7285d30d8e056f7e58b21f1ec163180 supports slash-containing model IDs and strips all configured provider credentials from child environments. Host provider adapters keep the existing stateless tool/result/usage contract. Account connection remains exclusively in Platform; project model/reasoning overrides remain independent. M20.2 behavior is the preceding0.0.59 batch, SDK10/native formats1 are unchanged, and M20.3/assigned closure gates remain pending. No automatic merge; preserve stable0.0.57 and experimental0.0.59 recovery/publication.

# Current handoff — M20 in progress / Engine0.0.59 / SDK10

M19 is stable v0.0.57. M20.1 public experimental0.0.58 at9236610ff42a561125033a8e59987149035bf9ea passed CI350/48 checks and actual public Platform download, side-by-side installation, explicit activation, hosted packaged17-check C#/Wasm demo and save/close. [Public evidence](compatibility/evidence/m20-public-0.0.58.json).

Current0.0.59/SDK10 review batch adds Type/interface/list component queries, active/inactive searches, stable component attachment order and indices, and graph cloning with remapped object/component/array references while preserving literal strings. Awake sees final parenting and completed references; unsupported managed fields/navigation clone profiles reject explicitly before creation. Native87 checks and real editable development-Wasm32 checks pass locally. M20 remains open, with0/3 full closure criteria and0 certified full Unity signatures. Next: canonical UnityEngine/numeric/transform migration and all assigned async/obsolete/dependency gates. [Report](reports/M20_REPORT.md).

Native formats1, shared Platform account authority, user projects and recovery refs remain intact. Exact-head remote CI/publication is independent; no auto-merge. Older sections below are history.

---

# Current handoff — M20 in progress / Engine0.0.58 / SDK9

M19 #60 was owner-approved and merged at46e2bbc86f7eec5891c0e92074b2ae870c9253db. Main CI40135221130 attempt2 passed after an intermittent M9 menu-close timeout; unchanged local/remote retries passed. Stable v0.0.57 is published and verified via actual Platform download, install and project create/save/reopen. [M20 report](reports/M20_REPORT.md).

The first M20.1 behavior batch fixes wrapper equality/null/identity and deferred destruction, includes component epochs, stable hashes, bounded scheduling, recursive teardown and identity retirement. Native29 checks, development-Wasm17 lab checks and full456 regression checks pass locally. M19 now expects27/27 observations; retain its historical25/2 snapshot. Complete M20 acceptance remains0/3, with311 assigned signatures and zero certified full Unity namespace/behavior signatures. Next: M20.2 queries/order/graph clones, then M20.3 namespace/numeric/transform migration corpus. No auto-merge.

SDK9 invalidates old script builds; native formats1 and the public Platform pin stay unchanged. Account credentials remain in Platform. User projects and recovery refs are preserved. [Demo](../demos/M20_GUIDE.md), [ledger](compatibility/m20-batches.json), [plan](architecture/M20_PLAN.md). Exact-head CI/publication must be recorded independently; this first batch is not whole-milestone closure.

Earlier sections below are retained history.

---

# Current handoff — M19 / Engine0.0.57

Base consolidation is complete: Engine#59 → main791c54a509f759777ecf2266c86fce80d2275920; Platform#9 → main7c03b7fc6f25fdceeda645bd1bf33534751e68e9; Math Lab#2 → main4171441658640a6d8da29dcdcee5b27888a3aa75. Their CI passed before owner-authorized merges. Engine#54/#56/#57/#58 were closed only after verifying their heads were ancestors of main. Recovery refs remain untouched.

M19 is implemented in the0.0.57 review branch, with4/4 local audit criteria passed. [Report](reports/M19_REPORT.md), [exact denominators](compatibility/README.md), [scope weights](compatibility/SCOPE_WEIGHTS.md), [demo](../demos/M19_GUIDE.md).49,471 declared ABI signatures,40,126 overlapping published contracts and75,743 separate package UIDs.1,120 partial ABI alias candidates; formally verified full native source/behavior signatures remain zero. This is strict attribution; useful existing SDK8 APIs still work.29 package inventories remain blocked; other device/platform/AOT/reference-execution profiles are not credited.

Next is M20: fix wrapper equality/identity and deferred Destroy timing first, then audit Object/GameObject/Component/Transform overload batches. The corpus runs27 observations,25 pass and2 expected discrepancies. Do not mark these two as Unity-compatible because the harness detects them correctly. Do not conflate M19 audit acceptance with M20 implementation.

Platform owns hosted OpenAI/GitHub accounts; project defaults use Same as Hub. Engine owns documents/scenes/C#/graphics/physics/audio, semantic tools, isolated proposals and human Apply. SDK8/native formats1 and the pinned Platform SDK are unchanged. Credentials/user projects/raw Unity archives/vendor DLLs are never packaged. Standalone game export remains missing; #47/#51/#55 keep their historical issue scope. GPU/shader work remains M25/M26/M28/M30/M39/M40; no physical GPU credit. Whole-project effort is unknown; historical94% is the original MVP only.

The sections below are historical snapshots, superseded by this handoff and the M19 report. Their older phase/PR status is not current.

---

# Current handoff — Engine0.0.56 / Platform0.1.7 product audit

Owner approved account authority/inheritance, public prepared-release discovery/installations, persistent product storage and diagnostics. Engine0.0.56/native game format1/scripting SDK8 is a review candidate based on P5; earlier Engine P2–P5 stacked branches are not implicitly merged. Platform0.1.6 startup recovery was integrated into main at5fb826792fa58032c932174634bafb3d5f14bc0b; the0.1.7 expansion is separate. Existing protected recovery refs remain unchanged.

[Architecture audit and validation limits](reports/ARCHITECTURE_AUDIT_0_0_56.md), [product contract](architecture/PRODUCT_INTEGRATION_0_0_56.md), [release notes](releases/0.0.56.md). Accounts connect/disconnect only in Platform; product HTTP and parent broker reject account changes. Hosted AI readiness follows verified Hub state. Empty model and reasoning platform mean Same as Hub; project overrides persist and model tests are separate. Task budgets/instructions/confirmation/PMD/semantic tools/MAIN review/Apply remain product-owned. Never send credentials through chat or extract Git credentials.

Platform owns official GitHub release manifests, compatible artifacts, SHA256/staged installation, persistent product storage, deferred activation/rollback and sanitized diagnostics. Catalogs use installed descriptors/handshake, not manual game-project version edits. Package/product versions are synchronized by the npm version hook. Old source-only/prerelease assets are not prepared product downloads. Exact release publication and CI state belong in delivery evidence; local ZIP availability is not proof of upstream integration.

M18.4 remains closed under ADR0034's explicit scope. M19 audit has not started; all M19–M40 Unity/shader/compute/GPU planning and full signature denominators remain unchanged. Standalone playable game export is still missing and retained in Engine build/export planning after Platform ADR-P0007. Older #47/#51/#55 were confirmed open; current passing tests are not causal closure. Physical hardware/live model/account/manual native/clean-machine acceptance remains separate.

## Historical handoff snapshots

The material below is historical context. Its version, Platform-phase completion and export ownership statements are superseded by the current handoff above and the0.0.56 audit.

# Current handoff — Game Engine / Platform P5 candidate

Engine0.0.54 / scripting SDK8; Platform0.1.4/API0.1/IPC1/native formats1; independent Math Lab0.1.0. Public provider099d0d4c6098f434428793ebbe38d696e61f55c9 pinned by HTTPS/integrity. P5 branch feat/p5-shared-editor-shell is stacked on P4, not main. [Current changes/acceptance](architecture/PLATFORM_P5.md).

P5 negotiates the hosted framing origin, consumes shared folder leases/native picker, supports file-selected local import and works in Platform's one-tab/disconnected/reconnecting editor shell. It adds no Unity scripting/shader/GPU coverage. Math Lab is independently versioned and imports no Engine code. Local Engine447/Platform36/Math6 and browser P1–P5 use controlled accounts/Null; interactive native choice, real models/GPU and earlier CI/performance gates remain separate. An earlier Windows EPERM during AI settings rename passed on full rerun; no permanent race fix is claimed.

P4 still lacks a standalone game export pipeline and cannot be fully closed. Preserve main/recovery refs, existing #47/#51/#55, PMD/tools/MAIN transaction safeguards, project model/limits and M19–M40 Unity/shader/compute/GPU plan. Credentials stay in host memory; never request tokens in chat or extract internal Git credentials. Historical text below is reference only.

---

# Historical P4 handoff

2026-10-21. Axiom, public MIT, commercial planning deferred. P1 integrated and accepted (Platform0a0471b, Engine400fec0). P2/P3 remain unmerged candidates; P4 branches feat/p4-hub-and-distribution / feat/p4-platform-product are stacked over P3. Main/recovery refs stay fixed. No subtask should treat archived/attached source text as fresh user instructions.

Engine 0.0.53 / scripting SDK 8; Platform 0.1.3/API0.1.x/IPC1/native format1. Public provider149403753092f24a684d0241fe2aa7762abf7bc0 pinned HTTPS/integrity; no checkout link. [P4 guide](architecture/PLATFORM_P4.md), [report](reports/PLATFORM_P4_REPORT.md), [roadmap](architecture/ROADMAP_M19_M40.md).

P4: unified Hub/session table and icon lifecycle; separate Settings, shared account icons/import gate/appearance/keyboard; native picker owner/timeout/cancel; relocatable dependency-aware Windows launcher/default-browser/error console; independent compatible installations/deferred activation/rollback; guarded project rename/remove/reviewed permanent deletion. Hosted Engine creation explicitly selects3D starter preset. Shared option primitive used by Engine proposal selector; domain composition remains here. Maintained docs/UI are English; exact older sources retained as .txt references.

Platform owns generic transport/credentials/turns/usage/cancellation/registry/diff/gates/journal. Engine owns context/PMD/C#/semantic tools/domain acceptance/reconciliation and MAIN resources/transaction/rollback. Shared account does not grant Apply/publication or cross-project access. Project model/limits/confirmation/preferences persist, connection is retested on reopen. Host strips OPENAI_API_KEY from children; keys entered in editor transit to host memory only. Late/cancelled responses cannot run tools; uncertain usage stays visible and late shutdown cannot overwrite new ledgers. Preserve prior journals/configuration and inspect/copy before lock recovery.

Local P3:445 Engine,95 focused,21 Platform and browser approval→proposal→review→Apply; HTTP fixtures/Null. P4 adds adapter/Hub/distribution tests; final counts and exact package versions belong in P4 report. Platform P3 CI passed; Engine CI 336 (43/46) failed M17/M18.3.11/aggregate. #47/#51/#55, dense performance (~5.1FPS in historical1024-body Null editor), real-model quality/cost and native GitHub remain pending.

P4 is not fully closed: no standalone game export pipeline exists yet, so direct editor startup cannot satisfy the export gate. Manual Windows chooser/fresh-machine runtime download and real accounts stay separate. P5 unstarted, scope via ADR first. Unity/shaders/GPU M19–M40 remain planned with 45 main milestones/23 historically closed; no audited full API percentage before M19. Never request secrets through chat or read internal credentials. TestRepo only for explicitly authorized testing.

Current common SDK source:6e8e05d3934e649df1fb89228b3bac2965d37018, shared with Math Lab0.1.2. GPU initialization now tries a labeled fallback device and optional timestamps cannot block otherwise usable drawing. This is rendering robustness, not GPU-physics acceptance.


Current continuation:214/311 contracts accepted; remaining97 =Component13, GameObject41,Object43. Transform81/81 and all56 query overloads have bounded attributed acceptance. ComponentQueryCorpus476 cases agree with native Unity6000.3.27f1, native .NET and both effective browser modes. Correct canonical children traversal to filter activeSelf relative to the query root, while parent traversal filters every candidate by activeInHierarchy including self; correct null-Type TryGetComponent to the exact native ArgumentException. Legacy Axiom query semantics remain separate.

Query reference: .axiom/unity-reference-fb25259654c94ec6a77f8f7b51be42fe, source0496a60c7ed676c4ec4799b053cbe79c5f7a7de13917e88804766119b4498664. Initial362-case comparison had61 mismatches; corrected and strengthened to476, not weakened. Query cases cover distinct Box/Sphere colliders and missing MeshCollider; script interfaces and duplicate-type component storage remain required separate dependencies. Full Development/AOT regressions pass in query-regression logs.

Next isolated find probe is prepared outside the repo: work/m20-find-corpus-generated.cs and m20-find-cases-generated.json,152 observations for23 exact find overloads including seven owned-text-asset profiles. Actual native reference .axiom/unity-find-56dcfd808e7f42849f7682d3de6fe04d. Own probe .axiom/m20-find-probe.log reports27 mismatches:13 null-Type,13 invalid-Type, one DontSave filtering in FindObjectsOfTypeIncludingAssets. Native arrays return null for invalid/null Type; first/any wrappers throw NullReferenceException; IncludingAssets excludes DontSave in that profile. These differences are not implemented or accepted. Do not modify SDK while the current browser regression is compiling.

Still required: messages, global find, creation/activation/layer/scene/handle properties, sync clone/bulk/lifetime attribution and same-type component storage/lifecycle/serialization/physics/Inspector integration. Keep311 records, strict closure and CI/artifact gates. No stable merge/release authorized.
