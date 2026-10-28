param([Parameter(Mandatory=$true)][string]$UnityEditor,[ValidateSet('Edit','Play','Vectors','Poses','Queries','Find','GameObject','Messages','SyncClone','Bulk','ObjectLifetime','Tags')][string]$Profile='Edit')
$ErrorActionPreference = 'Stop'
$oracleEditor = (Resolve-Path -LiteralPath $UnityEditor).Path
$oracleCore = Join-Path (Split-Path $oracleEditor) 'Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
$oracleExpected = '42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737'
if ((Get-FileHash -LiteralPath $oracleCore -Algorithm SHA256).Hash.ToLowerInvariant() -ne $oracleExpected) { throw 'This oracle requires the frozen Unity 6000.3.27f1 CoreModule.' }
$oracleRepo = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '../../..')).Path
& node (Join-Path $oracleRepo 'scripts/compatibility/generate-m20-async-oracle.mjs') --check
if ($LASTEXITCODE -ne 0) { throw 'Native async corpus differs from its portable source.' }
# Each invocation owns a fresh disposable project; no user Unity project is modified.
$oracleProject = Join-Path $oracleRepo ('.axiom/unity-reference-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $oracleProject 'Assets/Editor'),(Join-Path $oracleProject 'Packages'),(Join-Path $oracleProject 'ProjectSettings') | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20TagSearchOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20TagSearchOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/TagSearchCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/TagSearchCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20ObjectLifetimeOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20ObjectLifetimeOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/ObjectLifetimeCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/ObjectLifetimeCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20BulkOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20BulkOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/BulkObjectCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/BulkObjectCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20SyncCloneOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20SyncCloneOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/SyncCloneCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/SyncCloneCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20MessageOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20MessageOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/MessageDispatchCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/MessageDispatchCorpus.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/MessageReceiver.cs') -Destination (Join-Path $oracleProject 'Assets/MessageReceiver.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20GameObjectStateOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20GameObjectStateOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/GameObjectStateCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/GameObjectStateCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20FindOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20FindOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/ObjectFindCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/ObjectFindCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20QueriesOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20QueriesOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/ComponentQueryCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/ComponentQueryCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20PosesOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20PosesOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/PoseTransformCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/PoseTransformCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20VectorsOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20VectorsOracle.cs')
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/VectorTransformCorpus.cs') -Destination (Join-Path $oracleProject 'Assets/VectorTransformCorpus.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20Oracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20Oracle.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'OracleMarker.cs') -Destination (Join-Path $oracleProject 'Assets/OracleMarker.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20PlayOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20PlayOracle.cs')
foreach($oracleSourceName in @('AsyncLifecycleLab.cs','AsyncLifecycleProbe.cs','HierarchyPathLab.cs','HierarchyAuxCorpus.cs','ObjectMetaCorpus.cs')){Copy-Item -LiteralPath (Join-Path $oracleRepo ('demos/scripts/'+$oracleSourceName)) -Destination (Join-Path $oracleProject ('Assets/'+$oracleSourceName))}
Copy-Item -LiteralPath (Join-Path $oracleRepo 'demos/scripts/AsyncOverloadCatalog.cs') -Destination (Join-Path $oracleProject 'Assets/AsyncOverloadCatalog.original.txt')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'AsyncOverloadCatalog.cs') -Destination (Join-Path $oracleProject 'Assets/AsyncOverloadCatalog.cs')
[IO.File]::WriteAllText((Join-Path $oracleProject 'Packages/manifest.json'),'{}')
[IO.File]::WriteAllText((Join-Path $oracleProject 'ProjectSettings/ProjectVersion.txt'),"m_EditorVersion: 6000.3.27f1`n")
$oracleLog = Join-Path $oracleProject 'oracle.log'
$oracleArguments = @('-batchmode','-nographics','-noUpm','-projectPath',('"' + $oracleProject + '"'),'-logFile',('"' + $oracleLog + '"'))
if ($Profile -eq 'Tags') { $oracleArguments += @('-executeMethod','M20TagSearchOracle.Run') } elseif ($Profile -eq 'ObjectLifetime') { $oracleArguments += @('-executeMethod','M20ObjectLifetimeOracle.Run') } elseif ($Profile -eq 'Bulk') { $oracleArguments += @('-executeMethod','M20BulkOracle.Run') } elseif ($Profile -eq 'SyncClone') { $oracleArguments += @('-executeMethod','M20SyncCloneOracle.Run') } elseif ($Profile -eq 'Play') { $oracleArguments += @('-executeMethod','M20PlayOracle.Run') } elseif ($Profile -eq 'Messages') { $oracleArguments += @('-executeMethod','M20MessageOracle.Run') } elseif ($Profile -eq 'GameObject') { $oracleArguments += @('-executeMethod','M20GameObjectStateOracle.Run') } elseif ($Profile -eq 'Find') { $oracleArguments += @('-quit','-executeMethod','M20FindOracle.Run') } elseif ($Profile -eq 'Queries') { $oracleArguments += @('-quit','-executeMethod','M20QueriesOracle.Run') } elseif ($Profile -eq 'Poses') { $oracleArguments += @('-quit','-executeMethod','M20PosesOracle.Run') } elseif ($Profile -eq 'Vectors') { $oracleArguments += @('-quit','-executeMethod','M20VectorsOracle.Run') } else { $oracleArguments += @('-quit','-executeMethod','M20Oracle.Run') }
$oracleProcess = Start-Process -FilePath $oracleEditor -ArgumentList $oracleArguments -WindowStyle Hidden -PassThru
$oracleProcess.WaitForExit()
$oracleOutput = Join-Path $oracleProject $(if ($Profile -eq 'Tags') { 'oracle-tag-search-facts.json' } elseif ($Profile -eq 'ObjectLifetime') { 'oracle-object-lifetime-facts.json' } elseif ($Profile -eq 'Bulk') { 'oracle-bulk-facts.json' } elseif ($Profile -eq 'SyncClone') { 'oracle-sync-clone-facts.json' } elseif ($Profile -eq 'Play') { 'oracle-play-facts.json' } elseif ($Profile -eq 'Messages') { 'oracle-message-facts.json' } elseif ($Profile -eq 'GameObject') { 'oracle-gameobject-state-facts.json' } elseif ($Profile -eq 'Find') { 'oracle-find-facts.json' } elseif ($Profile -eq 'Queries') { 'oracle-query-facts.json' } elseif ($Profile -eq 'Vectors') { 'oracle-vector-facts.json' } elseif ($Profile -eq 'Poses') { 'oracle-pose-facts.json' } else { 'oracle-facts.json' })
if (!(Test-Path -LiteralPath $oracleOutput)) { throw "Unity did not produce observations. Inspect the local log: $oracleLog" }
$oracleFacts = Get-Content -LiteralPath $oracleOutput -Raw | ConvertFrom-Json
if ($oracleProcess.ExitCode -ne 0 -or $oracleFacts.error -or $oracleFacts.unityVersion -ne '6000.3.27f1' -or $oracleFacts.coreAssemblySha256 -ne $oracleExpected) { throw "Native oracle failed. Inspect the local observations: $oracleOutput" }
Write-Output "Native reference observations: $oracleOutput"
Write-Output 'These bounded observations do not certify all input domains, full lifecycle acceptance or all M20 signatures.'
