param([Parameter(Mandatory=$true)][string]$UnityEditor,[Parameter(Mandatory=$true)][string]$Consumer)
$ErrorActionPreference='Stop'
$oracleEditor=(Resolve-Path -LiteralPath $UnityEditor).Path
$oracleCore=Join-Path (Split-Path $oracleEditor) 'Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
if((Get-FileHash -LiteralPath $oracleCore -Algorithm SHA256).Hash.ToLowerInvariant() -ne '42e4b6c8def1193816c79961e31d88df7d53b373d739d9b7b931716a7ca7f737'){throw 'Wrong frozen reference CoreModule.'}
$oracleRepo=(Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '../../..')).Path
$oracleProject=Join-Path $oracleRepo ('.axiom/unity-obsolete-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $oracleProject 'Assets'),(Join-Path $oracleProject 'Packages'),(Join-Path $oracleProject 'ProjectSettings') | Out-Null
Copy-Item -LiteralPath (Resolve-Path -LiteralPath $Consumer).Path -Destination (Join-Path $oracleProject 'Assets/Probe.cs')
[IO.File]::WriteAllText((Join-Path $oracleProject 'Packages/manifest.json'),'{}')
[IO.File]::WriteAllText((Join-Path $oracleProject 'ProjectSettings/ProjectVersion.txt'),"m_EditorVersion: 6000.3.27f1`n")
$oracleLog=Join-Path $oracleProject 'compiler.log'
$oracleProcess=Start-Process -FilePath $oracleEditor -ArgumentList @('-batchmode','-nographics','-noUpm','-quit','-projectPath',('"'+$oracleProject+'"'),'-logFile',('"'+$oracleLog+'"')) -WindowStyle Hidden -PassThru
$oracleProcess.WaitForExit()
Write-Output "Native compiler log: $oracleLog"
Write-Output "Compiler process exit: $($oracleProcess.ExitCode). Expected compile errors must be checked against the attributed case catalog."
