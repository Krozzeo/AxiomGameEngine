# M20.3a — transform behavior and closure audit

Engine **0.0.61 / scripting SDK11 / native formats1**. This is a partial implementation batch. **M20 is not closed.** Stable main remains0.0.57/M19; previous experimental packages remain immutable.

## Outcome

Parent translation, rotation and scaling now publish descendant world poses in the real scripting packet. Previously the C# lookup table followed children but the host received only the parent's operation, so a subsequent frame could restore stale descendant poses. The editable demo now checks the next real frame as well as same-call getters.

The runtime computes and validates the full candidate subtree and its operation budget before modifying any pose. Combined position/rotation changes reject invalid quaternions without partially moving the object. Reparenting computes the destination first and leaves both hierarchy and poses intact on rejection. Translation works for a collapsed hierarchy; unsupported recovery of local coordinates after zero-scale expansion rejects explicitly.

The existing double-precision Axiom.Gameplay API gains Euler Z-X-Y rotation/getters, local/world/axis rotation, orbiting, look-at and direction setters, pose getters, xyz overloads, immediate-child enumeration and all six point/vector/direction span operations. Span operations support overlapping buffers and validate inputs/results before writing the destination. They reject short destinations, non-finite input/output and stale handles. Existing Quaternion.Euler retains its historical behavior; Transform.Rotate and the new Euler properties use the documented Z-X-Y transform order. SDK11 forces recompilation of older compiled bindings.

These are real legacy runtime behaviors, not canonical UnityEngine float/type compatibility. The existing flattened world-TRS representation still cannot certify general sheared hierarchies or all singular/local-matrix behavior. Operation limits remain explicit; the host never silently drops descendant updates.

## Required closure criteria

| Required criterion | Status | Evidence or unresolved work |
| --- | --- | --- |
| Unadapted scripts use assigned Unity signatures | **Blocked** | `tests/csharp/M20.Migration/HierarchyMigration.cs` fails to compile with CS0246 because the own SDK has no canonical UnityEngine namespace. No alias or source rewrite was added to hide this failure. |
| Full assigned hierarchy/reference/lifecycle behavior | **Partial** | Native and development-Wasm observations pass for the implemented profile. Complete Transform matrix/shear/zero-scale local state, exact overload profiles, async instantiation/cancellation/activation and assigned package identities remain. |
| Regression, existing projects, Stop/Undo and all dependency gates | **Partial** | The editable lab restores the authored scene exactly on Stop. Full assigned managed serialization/navigation clone and obsolete/message/tag/package acceptance is still required. Current local regression and exact-head CI are independent checks. |

The frozen denominator remains **311**: Component62, GameObject96, Object72, Transform81. Strict exact-source/behavior signature certification remains **0/311**. Existing name/family alias observations cannot count as accepted overloads. No required item was removed, moved to another milestone or waived.

## Reproduction and retained evidence

- `npm run test:native:m20`:138 native observations, including51 new transform/atomicity cases. [Native results](../compatibility/evidence/m20-transforms-native-windows.json).
- `npm run test:browser:m20`:32 original lifecycle plus19 transform observations, no page errors and exact authored-scene restoration after Stop. [Development-Wasm results](../compatibility/evidence/m20-transforms-development-wasm.json).
- `dotnet build tests/csharp/M20.Migration/M20.Migration.csproj --nologo`:required source migration gate **fails**. [Recorded failure](../compatibility/evidence/m20-unadapted-migration.json). This command is intentionally separate from supported-profile regressions and must pass before closure.
- `node scripts/compatibility/m20-closure.mjs --require-closed`:exits2 because required targets/criteria are not accepted. [Complete target audit](../compatibility/evidence/m20-closure-audit.json). The audit rejects changed/missing/duplicate frozen records and fabricated completion or certification counts.

The first local browser invocation could not locate Chromium because its project-local browser path was unset. Setting PLAYWRIGHT_BROWSERS_PATH to the existing project browser installation resolved this environment problem; no test expectation or timeout was reduced. Final exact-source checks and publication evidence must be reported separately from the local observations above.

## Editable demo

Use Platform0.1.11+ **Load Demos** on this installation. The same version-owned M20 project includes a second **Transform Contracts Controller**, with a physical editable script. At frame8 its Inspector reports **Passed19 / Failed0**; the original lifecycle controller reports **Passed32 / Failed0** at frame12. [Guide](../../demos/M20_GUIDE.md). Existing edited demos remain preserved and pinned to their original installation.

## Remaining implementation order

1. Introduce canonical UnityEngine object/component/Transform and float types with a shared runtime identity and interoperability with Axiom.Gameplay. Pass the unchanged migration corpus; a second disconnected runtime or textual namespace alias is insufficient.
2. Preserve local hierarchy state and implement the complete matrix/space/shear/singular contracts rather than deriving every local transform from a flattened lossy world TRS.
3. Implement and attribute all remaining assigned Object async/obsolete members, message/tag APIs, EntityId/TransformHandle/NativeArray profiles and full managed/navigation clone dependencies.
4. Attach positive/negative/lifecycle evidence to each exact assigned overload across native, Wasm/AOT and supported platform profiles; complete project/Undo/Play-Stop regressions and editable demo coverage. Only then run the closure gate, update milestone/GitHub status and request stable integration.

## Contract sources

The own implementation consults published interface contracts, not vendor method bodies: [Transform](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Transform.html), [Transform.Rotate](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Transform.Rotate.html), [Quaternion.Euler](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Quaternion.Euler.html). The frozen ABI and per-overload ledger remain authoritative for assignment; live documentation alone does not certify patch-level equivalence.
