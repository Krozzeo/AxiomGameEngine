# Current canonical M20 batch —0.0.62 / SDK12

Current review: **0.0.62 / SDK12**. All311 frozen compiled source contracts match. Current native suite722; real browser267 core plus190 asset/lifecycle checks per Development/effective-AOT mode. Attributed acceptance51/311:17 async behavior,4 hierarchy behavior and30 compile-error obsolete contracts. M20 remains open; source matches and test counts are not full Unity parity. See [current report](../reports/M20_CANONICAL_BATCH.md). Stable main remains0.0.57.

---

## Historical partial transform batch

# M20.3a transform behavior — Engine 0.0.61 / SDK11

M20 remains **in progress**. This batch fixes host publication of descendant transforms, atomic pose/reparent rejection and adds tested legacy Euler/axis/orbit/look/direction/span APIs. 138 native observations and 32 lifecycle + 19 transform development-Wasm observations pass, including next-frame consistency and exact Stop restoration. The required unadapted UnityEngine corpus fails CS0246; all 311 assigned records and 0 certified exact signatures remain visible.

Full Transform matrices/shear/zero-scale local state, canonical namespace/float interoperation, async and package profiles, message/tag/obsolete overloads and managed/navigation clone dependencies remain required. See [partial report](../reports/M20_TRANSFORM_BATCH.md). Stable main remains 0.0.57. No automatic merge or milestone closure. Older sections below retain historical versions.

---

# Confirmed implementation status

Current branch candidate: **0.0.59 / SDK10 / M20 in progress**. M19 is merged and published as stable0.0.57. Earlier experimental0.0.58/SDK9 passed48 CI checks and actual public Platform installation/hosted-demo acceptance. The current query/order/graph expansion passes457 Engine tests,87 native SDK cases,32 editable development-Wasm checks, M19's27 observations and the eight M18.3.8 browser criteria locally. Exact-head CI and publication remain independent gates. [M20 report](../reports/M20_REPORT.md), [M19 report](../reports/M19_REPORT.md), [strict coverage baseline](../compatibility/README.md). Older evidence below retains its historical scope.

| Area | Confirmed behavior | Remaining limits |
| --- | --- | --- |
| Editor/Hub |2D/3D scenes/resources/multiple Inspectors/docking; P4 unified project/session Hub/Settings/lifecycle | Older browser CI regressions remain independently tracked |
| Renderer | WebGPU/Null and first-scene initialization fix from M18.3.12 | Null does not prove physical GPU quality/performance |
| Scripting | C# runtime, SDK10, identity/lifetime, Type/interface/list queries, component order, graph/reference cloning | Canonical UnityEngine namespace/numeric/transform migration remains open; full native-source and behavior certification remain0% |
| Physics | CPU/Wasm2D/3D and stress demos | Dense whole-editor performance unresolved; GPU physics not implemented |
| AI | Semantic tools, isolated proposals, human review/Apply/PMD; P3 generic infrastructure shared | Real-model quality/cost and contract-specific limitations |
| GitHub | Session account, project destinations/preferences/autosave/review/publication/history | Native real-account acceptance deferred; TestRepo testing-only |
| P4 distribution | Relocatable prepared Windows launcher, dependency checks/local runtimes/browser/error console, local installation activation/rollback | Fresh-machine download/manual picker acceptance separate; no signed online product registry |
| Build/export | Direct independent editor startup | No standalone game build/export pipeline; Engine build/export gate open (transferred from Platform) |
| Shaders/compute/GPU physics expansion | Approved dependencies/batches |0% new acceptance; future M25/M26/M28/M30/M39/M40 |

## Evidence

P3 local check445/445 and Platform 21 with controlled HTTP/Null. Earlier0.0.52 had438 Node/44 Rust/18 browser groups locally; CI 320 (42/45) with M18/M18.2/M7 failures. Later consolidation fixed stale bridge adoption with deterministic regression tests and 441 Node; historical CI 329 (46/46) did not prevent later M17/#51 failure. P3 final CI 336 (43/46) failed M17/M18.3.11/aggregate; #47/#51/#55 remain separate.

P4 local final evidence is recorded in [its report](../reports/PLATFORM_P4_REPORT.md). Never substitute old successful runs for current checks or fixtures for manual/hardware evidence. Dense1024-body historical Null editor measured about5.1FPS; isolated CPU benchmarks are not editor FPS or GPU results.

## Planning

45 main milestones;23 historical closures plus stable M19,21 implementation milestones M20–M40 remain. M18.4 closed in the explicitly agreed scope with native account validation deferred (ADR0034). M20 object/component/hierarchy compatibility is in progress with all three whole-milestone closure gates open. The historical94%(122/130) refers only to original MVP weighting; expanded global/complete Unity coverage remains unquantified. [Roadmap](ROADMAP_M19_M40.md), [GPU/shaders](GPU_PHYSICS_AND_SHADERS_PLAN.md), [handoff](../AI_HANDOFF.md).

P5: negotiated Platform framing, shared public folder lease/native picker, safe file-based import, one hosted editor with disconnection/reconnection. Math Lab is independent and gives no Engine API credit. [Details](PLATFORM_P5.md).
