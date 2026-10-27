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
