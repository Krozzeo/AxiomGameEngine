# M20 — Objects, components and hierarchies (in progress)

First review batch: Engine 0.0.58 / scripting SDK 9 / native formats 1. This report does **not** close M20. The frozen denominator remains 311 assigned signatures; alias behavior tests do not become certified Unity signatures.

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

M20.2: exact component query overloads, ordering, interface/type searches, graph clone/reference/lifecycle behavior. M20.3: canonical UnityEngine source migration with preserved Axiom.Gameplay, transform/numeric/space contracts and the unadapted migration corpus. Async/new/obsolete target members require explicit dependency handling and tests. All three milestone closure criteria remain open. Do not label the complete milestone accepted merely because this first behavior batch passes.

[Plan](../architecture/M20_PLAN.md) · [Editable demo guide](../../demos/M20_GUIDE.md) · [Exact member ledger](../compatibility/m20-batches.json).
