# M20 canonical core — Engine 0.0.62 / SDK12

M20 remains **in progress**. The owned compiled SDK matches all **311 frozen public source contracts** for Object, Component, GameObject and Transform. This is a declaration/type/constraint/parameter-mode audit, separate from complete behavior certification. The original migration corpus now compiles and executes unchanged. Do not carry forward the previous CS0246 failure as the current result.

## Implementation

`UnityEngine` facades preserve the public Object/Component/Behaviour/MonoBehaviour/GameObject/Transform inheritance and float-facing vectors/quaternions. The retained `Axiom.Gameplay` API shares the same identities, entity table, serialized fields and lifecycle through explicit adapters. Private Unity lifecycle messages execute on that shared runtime. Inspector metadata supports comma-separated field declarations, qualified core types and canonical script references.

Runtime local TRS and full affine world matrices retain rotated nonuniform scale, shear and collapsed local coordinates across actual host frames. Matrices drive the ordinary render packets. Human runtime edits invalidate derived metadata. Authored reparenting still rejects shear that cannot be stored as authored TRS. Stop restores the original scene.

Cloning copies inline serialized classes/lists, remaps nested scene references and preserves SerializeReference cycles; navigation clones have independent paths. The async API captures an independent snapshot, defers integration/Awake, honors activation gates and cancellation, supports await/completion callbacks, and rolls back failed integration before publishing partial objects. All 17 assigned InstantiateAsync overloads have direct canonical count/pose/identity/null-input observations with frozen IDs retained in the case catalog. Their final reference-contract review is still separate from executing those observations.

EntityId and int bulk operations resolve actual native identities. NativeArray inputs have independent pinned contiguous storage, shared struct aliases and explicit disposal; this is the M20 input/lifetime dependency, not completion of Unity Collections or its job/allocator ecosystem. Bulk activation and clone imports are validated as complete packets.

CreatePrimitive creates owned Sphere, Capsule, Cylinder, Cube, Plane and Quad meshes through the normal mesh/render pipeline. It attaches distinct MeshFilter, MeshRenderer and appropriate collider identities. Capsule/Cylinder use capsule colliders; Plane/Quad use finite one-sided mesh surfaces. The owned Rust CPU/Wasm solver implements capsule segments/endcaps, finite surface contacts and ray tests. Tests check that bounding-box corners do not produce capsule hits. Collapsed primitive colliders leave physics safely and return after expansion. These additions do not implement arbitrary mesh collision, all collider APIs, shaders or GPU physics; those remain in their assigned later areas.

The canonical default GameObject constructor creates an unnamed root in the active scene with only Transform, as specified by the[6000.3 constructor documentation](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/GameObject-ctor.html). The name and component-array constructors retain their distinct contracts. Runtime creation, name editing and clone imports allow empty names without relaxing the saved-project schema.

World-preserving parenting carries local/matrix state in its parent operation. Combined position/rotation/scale changes publish one validated pose per affected object. Navigation uses the combined update, preserving the existing128-operation phase budget and the original66-object regression workload. Serialized legacy component/script references, canonical base Object references and null snapshots remain compatible after facade separation. Canonical DontDestroyOnLoad ignores child requests rather than promoting their root; native tests verify actual source-scene unload, descendant survival and explicit destruction. Persistent-scene ownership remains under review.

The compile controller retains the saved AOT build mode on project/branch entry and rebuilds mode-only changes. The browser harness asserts the **effective executed bundle mode**, so a Development recompile cannot be reported as an AOT result.

## Evidence

- [Compiled source audit](../compatibility/evidence/m20-sdk12-source.json):311/311 declaration, base/visibility/layout flag, constraint, parameter-mode and obsolete-classification matches. [Owned PE facts](../compatibility/evidence/m20-sdk12-abi.json) contain declarations/metadata, never vendor implementations.
- [Native .NET](../compatibility/evidence/m20-sdk12-native.json):330 semantic observations across the retained and canonical surfaces. The test count is not a count of certified target signatures.
- [Development editor/Wasm](../compatibility/evidence/m20-sdk12-development-wasm.json) and [effective AOT editor/Wasm](../compatibility/evidence/m20-sdk12-aot-wasm.json):132 observations each, no page errors, and exact Stop restoration.
- [Original migration corpus](../compatibility/evidence/m20-sdk12-migration.json):unchanged source/hash, real private Start, serialized float pose, hierarchy, rotation and delayed destruction in both execution modes. Earlier failed migration evidence is preserved as history.
- [Obsolete compiler contracts](../compatibility/evidence/m20-sdk12-obsolete-compile.json):53 direct consumer sites,30 required CS0619 errors and23 CS0618 warnings. Public attribute blobs match the frozen reference; no vendor method bodies are included.
- Full local Node checks:479/479 passed. Rust workspace tests and strict clippy passed; final formatting and unchanged-corpus compilation passed. Shared gameplay, runtime, physics and66-object batch AOT regressions also passed during this batch; final-head CI remains separate. CI now recompiles the source audit and obsolete consumers, and executes the M20 lab in effective AOT.
- [Direct async overload catalog](../compatibility/evidence/m20-async-overload-cases.json):17 frozen IDs and their explicit direct-call observations.

These browser runs use the Null renderer. They exercise source execution, scene/render packet preparation and CPU/Wasm physics, not GPU pixels, physical-GPU measurements or a live Unity execution oracle. Exact-head CI, public ZIP verification and complete per-signature acceptance are distinct gates.

## Work still required to close M20

Keep all 311 records. Attribute canonical positive, negative and lifecycle observations to every remaining overload; broad family tests must not be promoted into per-signature proof. Finish the reference/edge/dependency review, including remaining constructor edge cases, asset-backed clone requirements, persistent-scene ownership and degenerate transforms. Obsolete compiler-error/warning contracts are verified; deprecated runtime behavior still needs per-signature attribution. Implement any missing required behavior rather than waiving a record or changing its milestone.

The unchanged-corpus criterion now passes. Hierarchy/reference/lifecycle acceptance and final regression/dependency/publication criteria remain open. The strict closure command still rejects a full close with0/311 completely accepted behavior signatures. Source matching must not be presented as 100% functional Unity API parity. Stable main remains0.0.57; no automatic merge is authorized by this report.
