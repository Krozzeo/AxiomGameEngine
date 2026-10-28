# Independent native reference observations

This directory contains our own observation harness, not vendor implementation code. It runs only in a newly created disposable project with an explicitly supplied Unity Editor. No vendor binaries, resources, installer, account information or raw Editor logs belong in this repository or product artifacts.

Run `Run-Oracle.ps1 -UnityEditor <absolute path to Unity.exe>` using a separately licensed Unity 6000.3.27f1 installation. The runner first verifies the CoreModule SHA256 against the frozen reference, creates a unique project below the repository's ignored `.axiom` directory and starts the Editor hidden in batch mode. It retains that disposable project and local log for inspection. It never opens or edits an existing game project.

The committed `docs/compatibility/evidence/m20-unity-6000.3-native-observations.json` records the exact version and assembly hash, observed public outcomes and 128 explicit signed three-level transform inputs/outputs. The profile is **native Editor edit mode**. It does not establish play-mode lifecycle acceptance, all input domains, GPU rendering or all 311 assigned signatures.

Use `-Profile Play` to run the separate synchronous clone/world-setter observations in real Play mode. The matching `m20-unity-6000.3-play-observations.json` also retains the exact CoreModule hash. All13 direct clone signatures (17 positive variants) distinguish local defaults, explicit world parameters, destination scenes, null/real parents and explicit poses. World setters preserve local scale. This bounded Play sample does not certify all lifecycle timing or async ordering. Both runner profiles were verified in fresh disposable projects.

The observations correct assumptions previously taken from documentation or Axiom-only tests: the parameterless constructor creates `New Game Object` (explicit empty string stays empty); built-in qualified string component searches differ from script searches; flags affect existing/new components and search visibility; world rotations mirror through signed parent scale; `Transform.lossyScale` projects the world matrix onto its world rotation while matrix scale uses basis lengths. Synchronous supported asset clones now have owned identity/lifetime behavior; full mutable asset APIs remain outside this bounded profile. Native async standalone Mesh/TextAsset rejection is also captured.

After reviewing and replacing the committed observation JSON, run `node scripts/compatibility/generate-m20-transform-fixtures.mjs`. It produces a compact, portable little-endian float fixture consumed by both native .NET and real Development/AOT C#/Wasm tests. Compact encoding keeps the existing 64 KiB demo text budget; it does not reduce the 128-case sample or change tolerances. The host test independently composes the same captured inputs and tests valid and contradictory transport packets.

The generated C# fixture additionally exercises the observed default/local and explicit-pose clone contracts across all128 signed hierarchies. Those clone expectations apply the separately observed Play contracts; they are not128 additional captured native clone outputs.

Do not promote this sample into complete per-signature certification. The closure inventory and all 311 assigned records remain in force.

## Attributed async and compiler contracts

Play also runs the unchanged AsyncLifecycleLab/Probe source and all17 direct call expressions from AsyncOverloadCatalog (only record/file-scoped namespace syntax is adapted for Unity C#9). It records137 lifecycle checks and hashes of the actual copied inputs. Development/AOT reports hash their actual project sources. `accept-m20-async-contracts.mjs` requires matching hashes, exact CoreModule hash, all per-overload cases and zero failures.

`Run-ObsoleteOracle.ps1` compiles the generated owned consumer in a fresh native Unity project. `verify-m20-unity-obsolete.mjs` verifies exact compiler diagnostics,53 source sites, consumer and Core hashes. The same consumer is compiled against the owned native and Wasm SDK. Only30 error:true records count as accepted compiler rejection contracts;23 warnings still need runtime review.

## Direct vector/span method profile

`-Profile Vectors` invokes the unchanged owned VectorTransformCorpus in native Editor mode. It records240 outcomes for24 frozen exact overloads, including deep/local/ancestor reflections, collapsed axes, retired owners and empty/mismatched spans. Results include world/local pose to detect unintended mutations. Only public output floats and exception types are captured.

Regenerate direct calls with `python scripts/compatibility/generate-m20-vector-corpus.py`; observe Unity; replace the reviewed vector observation JSON; generate compact fixtures with `node scripts/compatibility/generate-m20-vector-fixtures.mjs`. Both generators have `--check`. Native .NET and the real editable browser demo execute the same corpus and compare against those fixtures. `accept-m20-vector-contracts.mjs` validates all input hashes, native/owned floats, both effective Wasm modes and per-overload cases before recording acceptance. Generated source/field budgets stay unchanged. Core pose/rotation/matrix and unrelated API records are not certified by this profile.

## Pose/rotation/matrix profile

`-Profile Poses` executes the unchanged PoseTransformCorpus:348 native observations for36 exact contracts. The generator is `generate-m20-pose-corpus.py`; compact fixture generation is `generate-m20-pose-fixtures.mjs`, both with `--check`. Owned native SDK input hashes and actual project source hashes must match. Nullable/dead targets preserve observed no-op/error semantics; destroyed-receiver cases avoid reading state after the evaluated call. Euler comparisons wrap modulo360 and quaternion signs represent equivalent orientations at2e-5 tolerance. `accept-m20-pose-contracts.mjs` attributes all profiles and both real Wasm modes before acceptance.

Play also executes the identical63-check HierarchyAuxCorpus and records its actual source hash. The auxiliary acceptance script attributes the15 remaining Transform contracts, including the protected constructor/type and deprecated aliases. New/unchanged/change-flag, capacity, live-enumerator and retired-handle outcomes remain distinct from unrelated Object/Component/GameObject acceptance. Five owned host-input regressions cover transport-specific flag updates.

## Object properties and component query profiles

Play also runs ObjectMetaCorpus unchanged:77 cases for unbound/live/retired identity, names, registered tags/flags and component slots. The actual source hash is recorded. Its acceptance script attributes29 exact source contracts using native Unity, owned native SDK hashes and both effective Wasm modes.

`-Profile Queries` runs ComponentQueryCorpus unchanged in native Editor mode. Its56 direct public query overloads produce476 source-attributed observations: regular/missing/retired receivers, inactive self/parent, explicit includeInactive, null/invalid Type, null/empty strings, null/reused result lists, disabled colliders, child-only/parent-only matches and matches only in inactive descendants. Results preserve native order through named-object/type identities, and exact exception types. No scene IDs or account data are captured. The corpus covers distinct Box/Sphere collider attachments; repeated same-type attachments and custom script interfaces remain separate dependencies, not inferred acceptance. `generate-m20-query-fixtures.mjs --check` verifies exact frozen IDs/declarations, source/Core hashes and full case keys before comparing portable fixtures. Both browser modes must execute the same source and restore the authored scene exactly.

Reference fixture generators freeze gzip's nonsemantic OS header byte to10, preserving existing fixture bytes on Windows, Linux and macOS. Native outcome/source hashes remain unchanged.

`-Profile Find` executes ObjectFindCorpus unchanged in native Editor mode:152 public outcomes for23 exact global find overloads. `generate-m20-find-fixtures.mjs --check` validates frozen IDs/declarations, source/Core hashes and all case keys. Controlled SphereCollider membership, actual ID sorting/first/any rules, owned TextAssets, DontSave variants and invalid Type outcomes are compared without assuming native allocation IDs or unspecified order. Both real browser modes must restore the authored scene. Imported asset persistence and custom script lifetime remain outside this bounded profile.

`-Profile GameObject` executes GameObjectStateCorpus unchanged in native Play mode. Ordinary SceneManager.CreateScene requires Play; the earlier Editor-only incomplete probe is not accepted. Fifty-three outcomes cover17 bounded state/constructor/scene/handle contracts. `generate-m20-gameobject-fixtures.mjs --check` pins the exact source/Core hashes and observed strings, and both real browser modes must restore the authored scene including its temporary runtime scene. Broader activation callbacks, component storage and custom preview culling remain separate dependencies.

`-Profile Messages` runs MessageDispatchCorpus and MessageReceiver unchanged in native Play and hashes both actual inputs. The360 observations map24 exact overloads. `generate-m20-message-fixtures.mjs --check` validates both hashes and all case keys. Owned native cases reset per-frame log budgets; the earlier cumulative-budget attempt is an invalid harness run, not a runtime regression. The real browser proves inherited factory registration and private-base preservation in effective AOT. Exact error wording and repeated same-type storage remain separate dependencies.
