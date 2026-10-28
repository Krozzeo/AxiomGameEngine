# 0.0.62 canonical core lab

Load the demo from this exact installation through Platform **Installations → Load Demos**, or run the source checkout’s **npm run demo:m20**. Open **Demo · M20 Object Lifecycle Lab**. Existing edited demos remain pinned to their original installation; load a fresh copy from the new installation.

Press Play and inspect these controllers after frame200:

| Controller | Passed | Failed |
| --- | ---: | ---: |
| Object Lifecycle Controller |32|0|
| Transform Contracts Controller |19|0|
| Unity Migration Controller |20|0|
| Async Instantiation Controller |17|0|
| Primitive Contracts Controller |11|0|
| Migration Corpus Observer |5|0|
| Async Overload Controller |35|0|
| Native Transform Controller |128|0|

The Native Transform Controller compares128 three-level hierarchies with exact Unity6000.3.27f1 captures, and exercises default/explicit-pose clone defaults across those hierarchies. The Unity Migration Controller also checks that world-pose setters preserve local scale. Native Play-mode clone observations are a separate bounded reference sample; full M20 certification remains open.

The async and primitive work starts in separate frame batches to respect the unchanged phase budget. All scripts are ordinary editable project files. The observer executes the original HierarchyMigration.cs without rewriting it. Primitive checks include the owned CPU/Wasm capsule resting on its finite plane. Stop restores the authored scene. Removing a runtime MeshFilter removes that mesh from rendering while retaining its collider. No runtime objects become saved authoring objects.

For automated validation use **npm run test:native:m20** and **npm run test:browser:m20**. Set **AXIOM_CSHARP_MODE=aot** for the browser AOT profile; the harness precompiles and asserts the bundle mode actually executed. These are source/runtime/CPU checks with a Null renderer, not GPU pixel acceptance. Full M20 per-signature certification is still open; see [current report](../docs/reports/M20_CANONICAL_BATCH.md).

---

# 0.0.61 transform controller

Install this experimental build through Platform0.1.11+ and click **Load Demos** on that installation. Open **Demo · M20 Object Lifecycle Lab**. A new **Transform Contracts Controller** runs the physical **Scripts/TransformContractsLab.cs**. At frame8 or later its Inspector must show **Passed19 / Failed0**. The original **Object Lifecycle Controller** still shows **Passed32 / Failed0** at frame12.

The transform controller creates a three-generation hierarchy, checks parent translation/rotation/scaling after the next real frame, exercises inverse transforms, overlapping spans, invalid pose rejection, Z-X-Y rotation, look-at and detachment, then destroys its runtime objects. Stop restores the complete authored scene. Modify the normal script file in the IDE to inspect/retest the behavior. No special sample implementation is used.

The lab uses Axiom.Gameplay double types. It does not certify canonical UnityEngine signatures or full matrix/shear/zero-scale behavior; M20 remains open. Loading demos preserves edited existing copies: to inspect the updated demo, use **Load Demos on the new0.0.61 installation**, rather than an old0.0.60 demo pinned to its original build.

---

# Object Lifecycle Lab — identity, queries and graph cloning

## Load through Axiom Platform

With Platform0.1.11+, install the new experimental0.0.60 build that declares the M20 demo catalog. In Installations, click **Load Demos** on that exact build. The Hub creates/registers only M20 Object Lifecycle Lab in configured Project Storage, shows completion notification and pins the project to the source installation. Repeating the action preserves edits and avoids duplicates. Older immutable0.0.60 packages lack this optional capability; the commit/PR label distinguishes builds. Math Lab demo generation is deferred.

## Manual fallback and behavior acceptance

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

## Asset Lifetime Lab —0.0.62

Load Demos also creates **Demo · M20 Asset Lifetime Lab** from this installation. Press Play and inspect Asset Lifetime Controller: Passed17, Failed0. Async Lifecycle Controller finishes with Passed137, Failed0. It exercises all17 direct async overloads with activation gates, cancellation/completion events, initialization, identity/pose and destruction. The same editable source runs in the independent native Unity harness. Stop restores the exact authored scene.

Runtime assets are temporary; cloned Mesh/TextAsset identities and names are independent, while imported geometry remains immutable. Async standalone asset rejection matches Unity; the async graph cases share asset references. These observations do not close the full311-record milestone.

The same Asset Lifetime Lab also contains **Hierarchy Path Controller**: Passed54, Failed0. It exercises absolute/relative object lookup, inactive descendant paths, missing/null/out-of-range and destroyed references, duplicate Transform rejection and query null inputs. These are ordinary editable script calls compared with the exact native Unity reference.

The hierarchy controller also checks explicit/default world/local reparenting, null/destroyed parent detachment, invalid cycles and attempted Transform destruction without graph changes. Errors are logged for forbidden operations; the controller continues.

## Vector Transforms Lab —0.0.62

Platform Load Demos also creates **Demo · M20 Vector Transforms Lab**. Press Play and select Vector Transform Controller. It finishes Count240, Passed240, Failed0; Diagnostics stays empty. The same direct-call corpus covers24 scalar/vector/in-place/separate-span overloads under regular, reflected, deep and collapsed-scale hierarchies, destroyed owners and empty/mismatched span lengths. Reference data comes from the exact native Unity version; all scripts are editable. Stop restores the authored project. These results verify CPU/.NET/Wasm behavior, not GPU pixels or all311 M20 contracts.

## Pose Transforms Lab —0.0.62

Platform Load Demos also creates **Demo · M20 Pose Transforms Lab**. Press Play and select Pose Transform Controller. Expected: Count348, Passed348, Failed0, empty Diagnostics. It exercises36 direct position/local position/rotation/scale/Euler/axis/matrix/translation/look-at/pose overloads against actual native outcomes. Null LookAt targets are no-ops; zero direction setters use identity; reflected/collapsed coordinate spaces retain native behavior. Scripts and compact reference fixtures are ordinary editable project files. Stop restores the authored scene. This is CPU/.NET/Wasm acceptance; M20's full311-record closure remains open.

Hierarchy Path Controller also exposes AuxiliaryPassed63, AuxiliaryFailed0 and AuxiliaryReport. Its second-frame corpus covers live child enumeration, protected constructor/type shape, capacity/root/count, sibling ordering, detachment and hasChanged. The primary Passed54/Failed0 remains separate. Combined Asset Lifetime Lab:17+137+54+63 checks.

## Object metadata and component indices

Load **Demo · M20 Object Contracts Lab** from the same installation. Play and inspect **Object Contracts Controller**: Count77, Passed77, Failed0 and Complete enabled. The same direct corpus runs in native Unity6000.3.27f1, .NET and both browser modes. It exercises unbound/live/destroyed identities and fake null, names, registered tags, component flag propagation and independent flags, component index bounds/type exceptions and destroyed-owner checks. Stop restores the complete authored scene. These bounded profiles accept29 additional contracts; they do not certify multiple components of the same type or unregistered tags.

## Component queries

Load **Demo · M20 Component Queries Lab** from the exact installation. Play and select **Component Queries Controller**. Count476, Passed476, Failed0, Diagnostics empty and Complete enabled are expected. Report uses compact q001:p / q001:f entries to stay within existing field limits. The same56 direct public overloads run unchanged in Unity, .NET and both real browser modes. The corpus covers distinct Box/Sphere colliders and missing MeshCollider; custom script interfaces and duplicate components remain separate milestone dependencies. Stop restores the authored scene exactly.

The native distinction matters: children are traversed relative to the query root even when its ancestors are inactive; inactive child subtrees are skipped unless included. Parent queries filter each candidate, including the receiver, by activeInHierarchy unless inactive objects are explicitly included.

## Global object searches

Load **Demo · M20 Object Find Lab**. Play and select **Object Find Controller**: Count152, Passed152, Failed0, empty Diagnostics and Complete enabled are expected. The same23 direct overloads execute against native Unity and both browser modes. The lab tests active/inactive objects, DontSave flags, retired objects, null/invalid Type arguments and owned TextAssets. First validates the lowest eligible actual ID; Any validates eligible membership without specifying which object must be returned. Stop restores the complete authored scene.

## GameObject state and scenes

Load **Demo · M20 GameObject State Lab**. Play and select **GameObject State Controller**: Count53, Passed53, Failed0, empty Diagnostics and Complete enabled are expected. The lab compares constructors, layer/static/activation flags, transform handles, scene movement/culling and ID lookup with exact native Play outcomes. Invalid layer assignments intentionally log an error and retain the prior layer without throwing. The runtime-only scene and objects disappear on Stop, restoring the entire authored scene. Duplicate component creation and full scene-manager behavior are separate dependencies.
