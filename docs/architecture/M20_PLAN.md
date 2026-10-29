**M20 complete — Engine 0.0.62 / SDK12.** All 311 frozen assigned contracts have attributed acceptance: 281 behavioral profiles and 30 obsolete compiler-error contracts. Component 62/62, GameObject 96/96, Transform 81/81 and Object 72/72. All three milestone criteria passed. This is bounded M20 compatibility, not complete Unity API parity. Stable main remains 0.0.57; 0.0.62 is an experimental review release.

# M20 — Compatible objects, components and hierarchies

Completed against the frozen Unity 6000.3 LTS inventory. [Report](../reports/M20_REPORT.md). The design and required scope below are retained.

## Required scope

- Complete Object, GameObject, Component, Behaviour and Transform: searches, tags/layers, activation, parenting, cloning, destruction and assigned overloads.
- Resolve destroyed-object equality/null, identity, component order, references and lifetime; UnityEngine code compatibility preserves Axiom.Gameplay.
- Correct transforms, spaces, conventions and numeric types according to M19.

## Planned batches

- 20.1 Identity/null/create/destroy.
- 20.2 Component queries and graph cloning.
- 20.3 Transforms, namespaces and migration corpus.

## Closure criteria

- [x] Corpus scripts use assigned Unity signatures without hidden adaptations.
- [x] Hierarchy/references/deferred destruction/activation match reference behavior without leaks.
- [x] Stop, Undo and existing projects retain contracts; assigned members have tests or explicit closure-blocking dependencies.

## Required editable demo

Object Lifecycle Lab: hierarchies, destroyed references, clones and activation changes.

## Shared execution rules

Before implementing, attach exact M19 signatures/overloads, ownership, dependencies, contracts and tests. Family lists do not replace the member inventory. Work in coherent vertical batches (roughly20–40 signatures when useful), including engine/C# bridge behavior, tests, editable demo and documentation; a phase may require several releases.

Verify actual behavior/errors/references/resources/Play-Stop/persistence/compatibility and regression across supported C# development/AOT/Windows and native/Wasm profiles. Rendering/audio/devices need observable output evidence. Empty wrappers, fake values or compile-only signatures do not count. Keep partial/blocked capabilities visible.

Preserve isolated proposals, reviewed Apply, user projects, external credentials and explicit publication. Do not auto-merge PRs. Close only with accepted criteria, editable demos and retained evidence; update README/handoff/status/changelog/matrix/GitHub. No invented dates or effort; M19 measures scope/weighting and replanning requires explicit scope decisions. [Reporting rules](MILESTONE_REPORTING.md).
