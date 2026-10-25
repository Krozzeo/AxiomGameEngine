# Object Lifecycle Lab — identity, queries and graph cloning

This is the M20 identity/query/graph review batch, not closure of the complete milestone. The editable physical scripts use the existing `Axiom.Gameplay` surface. Canonical `UnityEngine` namespace/signature migration remains required in M20.3; these behavior results do not certify the 311 assigned target signatures. Public experimental0.0.58/SDK9 contains the earlier17-check lifecycle lab; this0.0.59/SDK10 source contains the expanded32-check lab.

1. Install and explicitly activate experimental Engine0.0.59 after publication, or use this source checkout. Stable M19 is0.0.57.
2. From an extracted source/installation folder run `npm run demo:m20`, then open **Demo · M20 Object Lifecycle Lab**. The normal starter camera/light/cube remain editable.
3. Select **Object Lifecycle Controller**. Its Inspector contains the physical `Scripts/ObjectLifecycleLab.cs` script and a reference to **Clone template**.
4. Press Play. A visible clone appears temporarily. The script also creates a parent/child hierarchy, retains managed references, schedules destruction, removes/replaces a collider, and exercises immediate and delayed destruction.
5. At frame12 or later, Inspector shows **Passed32 / Failed0**. `Report` lists identity/lifetime, Type/interface/list queries, active/inactive searches and graph cases. Open both physical scripts in the IDE to inspect/change and rerun them. The graph checks observe the final parent and references during Awake, preserve literal ID strings, and keep the clone alive after its source is removed.
6. Stop. Runtime objects and script field changes disappear; the complete authored scene is restored. Save/reopen retains the original physical script and template assignment.

Native behavior checks: `npm run test:native:m20`. Real editor/Wasm and Stop restoration: `npm run test:browser:m20`. M19 regression now requires all 27 observations to pass; its original historical evidence remains unchanged in the M19 baseline.

The `Destroy` change is intentional: scheduled objects/components remain alive during the current Update/LateUpdate and are invalidated in the end phase before the next rendered frame. Scripts needing immediate removal must use `DestroyImmediate`. Delays use scaled game time. Null/destroyed targets are harmless. Destruction callbacks are idempotent even when they request destruction recursively. Old component references remain destroyed after another component is attached in the same slot.

Not certified here: all Unity overloads, UnityEngine numeric/namespace compatibility, async instantiation, tag registries, Transform matrices or whole-milestone completion. Component order/indices are supported for the existing component profile; unsupported managed serialization and navigation-agent clone profiles reject explicitly. No physical GPU or external Unity execution comparison is claimed.
