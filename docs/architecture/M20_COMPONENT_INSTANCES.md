# M20 component instance integration

Status: implementation and focused acceptance passed; complete current-source regression and final CI/artifact gates remain open. This document does not waive the frozen311-target denominator or whole-milestone criteria.

## Runtime identity and lifecycle

Repeated compiled scripts have separate runtime identities, fields, activation, ordered queries, messages and destruction. Clone maps bind original component identities to their corresponding new instances before remapping references, including cyclic peers and async snapshots whose source was destroyed. The native Unity27-observation corpus executes unchanged in owned .NET, Development and effective AOT. The real Inspector edits fields, toggles activation and selects the exact sibling reference. Existing authored-document constraints and8-scripts/entity,32-total limits are unchanged.

Colliders retain their actual entity owner. The primary collider remains backward compatible; up to seven additional records carry independent logical slots, runtime native identities, typed geometry, activation and asset bindings. Queries, clone remapping, serialization, reference pickers, drag/drop and Inspector cards preserve those identities. Removing the primary collider does not remove its siblings. Replacing a component never revives its retired wrapper. Runtime slots cannot enter authored documents; Stop restores the exact saved scene.

## AddComponent profiles

Both AddComponent(Type) and AddComponent<T> match140 direct Unity6000.3.27f1 Play outcomes in owned native .NET, Development and effective AOT. Coverage includes invalid/base/null types, null and retired receivers, Transform duplicates, default and repeated Box/Sphere/Capsule/Mesh colliders, empty and primitive-backed MeshFilter/MeshRenderer, repeated scripts and bounded DisallowMultipleComponent/RequireComponent fixtures. Invalid requests preserve the native exception or log/null outcome. An empty MeshCollider has no physical geometry; MeshFilter alone does not manufacture a Renderer. Shared mesh retirement clears every affected collider binding.

The native oracle is reproducible using Run-Oracle.ps1 -Profile Components. Captured public observations, source hashes and the acceptance script live in docs/compatibility/evidence and scripts/compatibility. No Unity binaries or implementation bodies are distributed.

## Physics and editor integration

The owned Rust CPU solver integrates each body once and tests its attached shapes. Sibling shapes never collide with one another. Broad-phase bounds include every shape; contacts and raycasts retain exact logical collider slots. Component-specific exclusions preserve collisions involving other siblings. Extra colliders can be disabled, destroyed and rebuilt without duplicating gravity or losing the owner velocity. Trigger and physical manifolds have separate identities and events. Canonical UnityEngine.MonoBehaviour callbacks now receive the matching collider and contact point identities; legacy callbacks retain their public API.

Six compound Rust core tests, the scalar ABI test, real Wasm host tests and14 added native component/callback/reference checks pass. The normal Compound Colliders Lab exercises13 physical assertions plus actual Inspector geometry, activation and reference edits in Development/effective AOT, followed by exact Stop restoration. Both this lab and Component Creation Lab load through Platform's standard version-owned demo manifest.

## Explicit scope

This establishes functional creation and lifecycle integration with the existing CPU baseline, not PhysX-wide numerical parity. Automatic compound mass properties, full mesh cooking, broader physics APIs and GPU physics remain in their planned milestones. Null-renderer editor tests do not establish GPU pixel acceptance. Uncompiled script types and unrestricted attribute dependency graphs are outside the recorded component creation profile.

The Windows AOT compiler uses a short, privately owned temporary staging path and copies validated output back to the project build directory. This avoids native compiler failures caused by long extracted project paths. Cleanup verifies the staging directory is a direct child of the temporary root with the expected generated name.

## Final regression corrections

The old Entity.Spawn API creates a bare runtime object; it must not copy live Script IDs or numeric Script activation aliases from its template. Canonical Instantiate attaches its own independently identified clones afterward. Native regressions and the full real M4 gameplay/compile/reopen/Undo/watchdog test verify this distinction.

Background authoring refreshes preserve a live-only Inspector target while the same project and preview still own the running renderer. They reject replies requested before a subsequent Play/Stop or state change. Runtime Inspector edits wait for the preceding editor operation, retain their original entity target, merge activation against current live state and reject a different Play generation. The real Script Instances browser test deliberately delays a genuine compile-completion event in Development, then releases its normal refresh after a runtime object is selected and edited. Both browser modes verify independent fields, activation, references and exact Stop restoration. Tests await completion/observable state rather than treating frames that elapsed before an action as proof that the action finished.
