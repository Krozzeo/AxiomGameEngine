param([Parameter(Mandatory=$true)][string]$UnityEditor,[ValidateSet('Edit','Play')][string]$Profile='Edit')
$ErrorActionPreference = 'Stop'
$oracleEditor = (Resolve-Path -LiteralPath $UnityEditor).Path
$oracleCore = Join-Path (Split-Path $oracleEditor) 'Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
$oracleExpected = '42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737'
if ((Get-FileHash -LiteralPath $oracleCore -Algorithm SHA256).Hash.ToLowerInvariant() -ne $oracleExpected) { throw 'This oracle requires the frozen Unity 6000.3.27f1 CoreModule.' }
$oracleRepo = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '../../..')).Path
$oracleProject = Join-Path $oracleRepo ('.axiom/unity-reference-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $oracleProject 'Assets/Editor'),(Join-Path $oracleProject 'Packages'),(Join-Path $oracleProject 'ProjectSettings') | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20Oracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20Oracle.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'OracleMarker.cs') -Destination (Join-Path $oracleProject 'Assets/OracleMarker.cs')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'M20PlayOracle.cs') -Destination (Join-Path $oracleProject 'Assets/Editor/M20PlayOracle.cs')
[IO.File]::WriteAllText((Join-Path $oracleProject 'Packages/manifest.json'),'{}')
[IO.File]::WriteAllText((Join-Path $oracleProject 'ProjectSettings/ProjectVersion.txt'),"m_EditorVersion: 6000.3.27f1`n")
$oracleLog = Join-Path $oracleProject 'oracle.log'
$oracleArguments = @('-batchmode','-nographics','-noUpm','-projectPath',('"' + $oracleProject + '"'),'-logFile',('"' + $oracleLog + '"'))
if ($Profile -eq 'Play') { $oracleArguments += @('-executeMethod','M20PlayOracle.Run') } else { $oracleArguments += @('-quit','-executeMethod','M20Oracle.Run') }
$oracleProcess = Start-Process -FilePath $oracleEditor -ArgumentList $oracleArguments -WindowStyle Hidden -PassThru
$oracleProcess.WaitForExit()
$oracleOutput = Join-Path $oracleProject $(if ($Profile -eq 'Play') { 'oracle-play-facts.json' } else { 'oracle-facts.json' })
if (!(Test-Path -LiteralPath $oracleOutput)) { throw "Unity did not produce observations. Inspect the local log: $oracleLog" }
$oracleFacts = Get-Content -LiteralPath $oracleOutput -Raw | ConvertFrom-Json
if ($oracleProcess.ExitCode -ne 0 -or $oracleFacts.error -or $oracleFacts.unityVersion -ne '6000.3.27f1' -or $oracleFacts.coreAssemblySha256 -ne $oracleExpected) { throw "Native oracle failed. Inspect the local observations: $oracleOutput" }
Write-Output "Native reference observations: $oracleOutput"
Write-Output 'These bounded observations do not certify all input domains, full lifecycle acceptance or all M20 signatures.'
