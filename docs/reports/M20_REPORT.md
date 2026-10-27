Current canonical review: **0.0.62 / SDK12**. All311 compiled public source contracts match; unchanged UnityEngine corpus and132 Development/effective-AOT observations pass. M20 stays open for complete per-signature behavior/dependency acceptance. [Current batch](M20_CANONICAL_BATCH.md). Older batch sections below retain their historical outcomes.

Current batch: **0.0.61 / SDK11 / M20.3a partial transform behavior**. [Current batch and failed closure gate](M20_TRANSFORM_BATCH.md). Native138 and editable development-Wasm51 observations pass. The unadapted UnityEngine corpus fails CS0246. This milestone is **not closed**; older batch counts below remain historical.

# M20 â€” Objects, components and hierarchies (in progress)

Current review batch: Engine0.0.59 / scripting SDK10 / native formats1. Earlier public batch:0.0.58 / SDK9. This report does **not** close M20. The frozen denominator remains311 assigned signatures; alias behavior tests do not become certified Unity signatures.

## Query/order/graph expansion

Adds generic/Type/interface/list component queries, explicit active/inactive object searches, stable attachment order and index APIs, and generic/component/parent/pose cloning. Typed object/component/array references remap after the full graph exists; literal strings remain unchanged. Parenting finishes before Awake/OnEnable. Clone metadata uses new identities and runtime clones detach prefab authoring connections. Unsupported managed serialization/navigation profiles reject before creating objects; incompatible physics component profiles reject without metadata/operation changes.

Current local evidence:87 native SDK checks;32 editable development-Wasm lab checks, no page errors and complete Stop restoration; C# runtime API and53-check runtime batches pass. Expanded full Engine regression457/457 and M19 browser27/27 pass. Exact-head remote CI/publication are separate gates. [Native expansion](../compatibility/evidence/m20-queries-native-windows.json), [Wasm expansion](../compatibility/evidence/m20-queries-development-wasm.json).

Contracts consulted: [hierarchy queries always search self](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/GameObject.GetComponentsInChildren.html), [parent queries](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/GameObject.GetComponentsInParent.html), [list/index stability](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/GameObject.GetComponentAtIndex.html), [clone parent/active/lifecycle rules](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Object.Instantiate.html). Reference interface/contract facts inform Axiom's own implementation; no vendor method body is shipped.

Public0.0.58 at9236610ff42a561125033a8e59987149035bf9ea passed CI350/48 checks. Its actual public ZIP passed Platform SHA256/extraction/handshake, installed alongside stable0.0.57, activated explicitly, ran the packaged17-check C#/Wasm demo through the hosted product shell, and saved/closed the project without page errors. [Public acceptance](../compatibility/evidence/m20-public-0.0.58.json). A first network download timed out and succeeded on retry; external verifier path/stale-input mistakes were corrected, not reported as product successes. The32-check expansion belongs to0.0.59, not the immutable0.0.58 package.

## Base

M19 PR #60 was explicitly approved and merged into main at `46e2bbc86f7eec5891c0e92074b2ae870c9253db`. Main CI [40135221130](https://github.com/Krozzeo/AxiomGameEngine/actions/runs/40135221130) passed in attempt 2. Attempt 1 had a real M9 menu-close wait timeout; the unchanged M9 profile subsequently passed both locally and remotely, without reducing assertions or increasing timeouts. This intermittent legacy fixture remains a maintenance concern, not an unreported successful first run.

Stable [0.0.57](https://github.com/Krozzeo/AxiomGameEngine/releases/tag/v0.0.57) was published by [40136461014](https://github.com/Krozzeo/AxiomGameEngine/actions/runs/40136461014). The actual public Platform catalog detected it as stable, downloaded/verified/installed the ZIP, and created/saved/closed/reopened a real project. Its ZIP SHA256 is `69def006a7fb05b7c6db9a6fcefdbe85eee3da24d5d4aac0982dbfb52251a3a4`. Existing stable/experimental publications and recovery refs remain unchanged.

## Implemented M20.1 behavior

- Canonical per-session native identities across recreated GameObject/Transform/component wrappers; type/slot/generation separation, hash/dictionary consistency and retained IDs after destruction.
- Null-like comparison and bool for destroyed handles, retaining actual managed references. Removed component slots receive new identities and cannot revive old handles.
- Deferred Object.Destroy committed in the end phase; scaled delays and duplicate requests use the earlier deadline. Explicit DestroyImmediate remains immediate.
- Single lifecycle teardown for recursively requested destruction; component removal preserves its owner. Identity registries retire destroyed entities, scheduling is bounded and stop/reset cancels pending work.
- SDK 9 forces recompilation instead of reusing SDK 8 semantics. Native project/scene formats remain 1. Account authority and the pinned public Platform SDK are unchanged.

## Evidence and limits

Native SDK executable: 29 checks pass, including 1,000 create/destroy cycles without registry growth and rejected stale component access. Full Engine check: 456/456 pass. Real editable Object Lifecycle Lab: 17/17 C#/Wasm observations, no page errors, exact authoring restoration on Stop. Additional regression and exact-head CI/publication results must be recorded separately when complete.

The two historical M19 identity/lifetime discrepancies have functional fixes; the updated M19 regression requires 27/27. Its original 25-pass/2-discrepancy snapshot remains historical evidence. Current strict full Unity source/signature/behavior certification stays zero: canonical UnityEngine namespace, numeric types and complete overload attribution are still pending. These checks measure real Axiom alias behavior, not a Unity execution-oracle comparison.

Initial exact-head CI40145053827 detected a genuine old-demo assumption: Runtime API Lab used deferred `Destroy` to remove a just-cloned script before it could start, allowing that script to clone recursively. The demo now explicitly uses `DestroyImmediate` for this operation. The unchanged M18.3.8 assertions then pass locally across its full eight criteria (unique clone selection, repeated Play/Stop, actual image captures, controlled Responses image delivery, durable Apply and native2D/3D motion). [Retained regression evidence](../compatibility/evidence/m20-m18-3-8-regression.json). No selector, timeout or assertion was weakened. New exact-head CI remains mandatory; the initial failing run is not relabeled as successful.

## Remaining required work

M20.3: canonical UnityEngine source migration with preserved Axiom.Gameplay, transform/numeric/space contracts and the unadapted migration corpus. Complete exact overload/negative/reference-profile attribution, async/new/obsolete target members and managed serialization/navigation dependencies remain required. All three milestone closure criteria remain open. Do not label the complete milestone accepted merely because the alias behavior batches pass.

[Plan](../architecture/M20_PLAN.md) Â· [Editable demo guide](../../demos/M20_GUIDE.md) Â· [Exact member ledger](../compatibility/m20-batches.json).
