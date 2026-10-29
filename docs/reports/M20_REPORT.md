# M20 — Objects, components and hierarchies

**M20 complete — Engine 0.0.62 / SDK12.** All 311 frozen assigned contracts have attributed acceptance: 281 behavioral profiles and 30 obsolete compiler-error contracts. Component 62/62, GameObject 96/96, Transform 81/81 and Object 72/72. All three milestone criteria passed. This is bounded M20 compatibility, not complete Unity API parity. Stable main remains 0.0.57; 0.0.62 is an experimental review release.

## Outcome

Canonical UnityEngine Object, Component, GameObject and Transform source contracts execute alongside preserved Axiom.Gameplay consumers. The implemented profiles cover typed searches, tags/layers and activation; spaces and poses; identity and destroyed handles; deferred destruction; synchronous/async graph cloning and references; bulk/lifetime APIs; message overloads; primitive and component creation. Repeated script/collider instances have independent identities, references, activation and Inspector edits. Compound CPU/Wasm colliders share one body with slot-specific queries and callbacks.

## Acceptance criteria

| Required criterion | Result | Retained evidence |
| --- | --- | --- |
| Unadapted assigned Unity corpus | Passed | [Migration](../compatibility/evidence/m20-sdk12-migration.json) |
| Hierarchy, references, lifecycle and dependencies | Passed | [Strict ledger](../compatibility/m20-batches.json), [compound/Inspector](../compatibility/evidence/m20-compound-colliders-acceptance.json) |
| Stop, Undo, existing projects and complete regression | Passed | [Final regression](../compatibility/evidence/m20-final-regression.json), [CI](https://github.com/Krozzeo/AxiomGameEngine/actions/runs/40965093597), [actual public package](../compatibility/evidence/m20-public-0.0.62.json) |

## Automated evidence

The frozen denominator stays **311**: Component 62, GameObject 96, Transform 81, Object 72. All are attributed to exact native Unity6000.3.27f1 observations and owned .NET/Wasm execution, or obsolete compiler-error contracts. The 30 obsolete errors are compiler compatibility, not executable runtime features. Source/profile hashes and failures resolved during development remain in the evidence tree; matching method names alone do not count.

Local Node **506/506**, native SDK core **925**, thirteen additional native corpora, Rust core **43** plus compound ABI, and **17 labs per Development/effective-AOT profile** pass. The labs contain 2739 overlapping observations per mode; that number is not an API coverage denominator. Windows AOT staging avoids the compiler's deep-path failure. Legacy bare Spawn no longer duplicates Script identities. Inspector selection survives background refresh, stale generations reject edits and rapid runtime edits serialize. Assistant cancellation acceptance waits for bounded cleanup rather than assuming a terminal UI label implies cleanup has already finished.

Accepted executable code: `11745ba76281dc41e206537b0cba4e9c5e746c67`. [CI run](https://github.com/Krozzeo/AxiomGameEngine/actions/runs/40965093597). The real public ZIP was downloaded and checksum-verified by Platform, safely extracted, handshaken, explicitly activated and loaded 17 demos idempotently. Its packaged SDK compiled and executed the 13-check Compound Colliders Lab through the real hosted product shell, then saved and closed the project without page errors. Null rendering was used for this package/UI check; GPU pixels are covered separately by existing real-WebGPU regressions and are not inferred from that check.

## Editable demos and manual checks

In Platform, refresh **Installations → Add Installation**, choose the latest **0.0.62 Experimental**, install/activate it, then **Load Demos**. Open the new version-owned projects and follow the [demo guide](../../demos/M20_GUIDE.md). Automated tests already exercise the controls, native/Wasm builds and Stop restoration. Required additional manual gates: **None**. User exploratory testing is welcome; no paid model/account call was made for this delivery.

## Limits and risks

Acceptance is for the explicitly recorded bounded profiles, not every possible Unity serialization/package/platform combination. Full managed serialization and navigation cloning profiles outside this milestone's supported contract reject explicitly. The frozen global Unity inventory remains 40126 records; M20's 311 does not imply global parity. Broader math is M21. Full PhysX numerical parity, automatic compound mass properties, full mesh cooking, programmable shaders and GPU physics retain their later roadmap gates. Dense physics fluency is not silently reclassified as accepted.

Stable main **0.0.57** and recovery refs remain unchanged. [0.0.62](https://github.com/Krozzeo/AxiomGameEngine/releases/tag/experimental-pr-63-11745ba76281dc41e206537b0cba4e9c5e746c67) is experimental pending human review of PR#63 and its base; completion does not authorize merging.

## Completion

M20 **100% (3/3 required criteria)**. Expanded whole-project percentage remains **unquantified**, since approved effort weights for the expanded roadmap do not exist. Next: **M21 — Mathematics, geometry and utilities**.
