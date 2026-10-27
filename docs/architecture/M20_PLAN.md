Current review: **0.0.62 / SDK12**. All311 frozen compiled source contracts match. Current native suite722; real browser267 core plus190 asset/lifecycle checks per Development/effective-AOT mode. Attributed acceptance51/311:17 async behavior,4 hierarchy behavior and30 compile-error obsolete contracts. M20 remains open; source matches and test counts are not full Unity parity.

Historical partial batch: **0.0.61 / SDK11 / M20.3a transform behavior**. Canonical migration corpus fails CS0246; all three full closure criteria remain open. [Batch report](../reports/M20_TRANSFORM_BATCH.md).

# M20 — Compatible objects, components and hierarchies

Approved plan since 2026-10-19. **In progress: M20.1 identity/lifetime and M20.2 query/order/graph behavior batches, Engine 0.0.59 / SDK10.** The preceding 0.0.58 / SDK9 experimental package is publicly verified. Whole-milestone acceptance remains open. [Report](../reports/M20_REPORT.md), [exact member ledger](../compatibility/m20-batches.json), [editable demo](../../demos/M20_GUIDE.md). Existing aliases and new behavior checks are not certified Unity source signatures.

Minimum dependencies: M19.

[Roadmap](ROADMAP_M19_M40.md) · [Manifest](roadmap-v2.json) · [GPU/shaders](GPU_PHYSICS_AND_SHADERS_PLAN.md)

## Required scope

- Complete Object, GameObject, Component, Behaviour and Transform: searches, tags/layers, activation, parenting, cloning, destruction and assigned overloads.
- Resolve destroyed-object equality/null, identity, component order, references and lifetime; UnityEngine code compatibility preserves Axiom.Gameplay.
- Correct transforms, spaces, conventions and numeric types according to M19.

## Planned batches

- 20.1 Identity/null/create/destroy.
- 20.2 Component queries and graph cloning.
- 20.3 Transforms, namespaces and migration corpus.

## Closure criteria

- [ ] Corpus scripts use assigned Unity signatures without hidden adaptations.
- [ ] Hierarchy/references/deferred destruction/activation match reference behavior without leaks.
- [ ] Stop, Undo and existing projects retain contracts; assigned members have tests or explicit closure-blocking dependencies.

## Required editable demo

Object Lifecycle Lab: hierarchies, destroyed references, clones and activation changes.

## Shared execution rules

Before implementing, attach exact M19 signatures/overloads, ownership, dependencies, contracts and tests. Family lists do not replace the member inventory. Work in coherent vertical batches (roughly20–40 signatures when useful), including engine/C# bridge behavior, tests, editable demo and documentation; a phase may require several releases.

Verify actual behavior/errors/references/resources/Play-Stop/persistence/compatibility and regression across supported C# development/AOT/Windows and native/Wasm profiles. Rendering/audio/devices need observable output evidence. Empty wrappers, fake values or compile-only signatures do not count. Keep partial/blocked capabilities visible.

Preserve isolated proposals, reviewed Apply, user projects, external credentials and explicit publication. Do not auto-merge PRs. Close only with accepted criteria, editable demos and retained evidence; update README/handoff/status/changelog/matrix/GitHub. No invented dates or effort; M19 measures scope/weighting and replanning requires explicit scope decisions. [Reporting rules](MILESTONE_REPORTING.md).
