param(
    [string]$ResultsDirectory = 'C:\falcon\Application test-results',
    [string]$FoundationCommit = '0783337f84707c024b7a18f09be60c3c7fc5cdd4',
    [string]$FoundationScratchRoot = 'C:\falcon\FB'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$runDir = Join-Path $ResultsDirectory "foundation-binding-$timestamp"
$scratchRunDir = Join-Path $FoundationScratchRoot $timestamp
$foundationRoot = Join-Path $scratchRunDir 'f'
$transcript = $null
$failure = $null

function Invoke-Checked {
    param([string]$Name, [scriptblock]$Action)
    Write-Host ""
    Write-Host "========== $Name =========="
    $global:LASTEXITCODE = 0
    & $Action
    if ($LASTEXITCODE -ne 0) { throw "$Name failed with exit code $LASTEXITCODE" }
    Write-Host "$Name : PASS"
}

try {
    Push-Location $repoRoot

    $appHead = (git rev-parse HEAD).Trim()
    $appBranch = (git branch --show-current).Trim()
    $appStatus = @(git status --porcelain)
    if ($appBranch -ne 'application-development') { throw "Expected application-development, got '$appBranch'." }
    if ($appStatus.Count -gt 0) { throw "Application checkout must be clean before binding validation: $($appStatus -join ', ')" }

    $sdk = (dotnet --version).Trim()
    if ($sdk -ne '10.0.302') { throw "Falcon requires exact .NET SDK 10.0.302; selected '$sdk'." }

    New-Item -ItemType Directory -Force -Path $runDir | Out-Null
    New-Item -ItemType Directory -Force -Path $scratchRunDir | Out-Null
    $transcript = Join-Path $runDir 'Foundation-Binding-Validation.log'
    Start-Transcript -Path $transcript -Force | Out-Null

    Write-Host 'FALCON FSATS PART 2 FOUNDATION BINDING VALIDATION'
    Write-Host "Application HEAD:   $appHead"
    Write-Host "Foundation target:  $FoundationCommit"
    Write-Host "Results:            $runDir"
    Write-Host "Foundation scratch: $foundationRoot"

    $sandbox = Join-Path $scratchRunDir '.dotnet-env'
    $env:DOTNET_CLI_HOME = Join-Path $sandbox 'd'
    $env:NUGET_PACKAGES = Join-Path $sandbox 'n\p'
    $env:NUGET_HTTP_CACHE_PATH = Join-Path $sandbox 'n\h'
    $env:NUGET_PLUGINS_CACHE_PATH = Join-Path $sandbox 'n\c'
    $env:TEMP = Join-Path $sandbox 't'
    $env:TMP = $env:TEMP
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    New-Item -ItemType Directory -Force -Path $env:DOTNET_CLI_HOME,$env:NUGET_PACKAGES,$env:NUGET_HTTP_CACHE_PATH,$env:NUGET_PLUGINS_CACHE_PATH,$env:TEMP | Out-Null

    Invoke-Checked 'CLONE FOUNDATION REFERENCE' {
        git -c core.longpaths=true clone --no-checkout 'https://github.com/raed82iam/Falcon.git' $foundationRoot
    }

    Push-Location $foundationRoot
    try {
        Invoke-Checked 'ENABLE FOUNDATION LONG PATHS' {
            git config core.longpaths true
        }

        Invoke-Checked 'CHECKOUT EXACT FOUNDATION COMMIT' {
            git -c core.longpaths=true checkout --detach $FoundationCommit
        }

        $actualFoundation = (git rev-parse HEAD).Trim()
        if ($actualFoundation -ne $FoundationCommit) { throw "Foundation commit mismatch: $actualFoundation" }

        $foundationStatus = @(git -c core.longpaths=true status --porcelain)
        if ($foundationStatus.Count -gt 0) {
            throw "Foundation reference checkout is not clean: $($foundationStatus -join ', ')"
        }

        Invoke-Checked 'FOUNDATION REFERENCE RESTORE' { dotnet restore 'Falcon.Foundation.ControlledProjectFoundation.slnx' }
        Invoke-Checked 'FOUNDATION REFERENCE RELEASE BUILD' { dotnet build 'Falcon.Foundation.ControlledProjectFoundation.slnx' -c Release --no-restore }
    }
    finally {
        Pop-Location
    }

    Invoke-Checked 'APPLICATION RESTORE' { dotnet restore 'applications/Falcon.Applications.slnx' }
    Invoke-Checked 'APPLICATION RELEASE BUILD' { dotnet build 'applications/Falcon.Applications.slnx' -c Release --no-restore }
    Invoke-Checked 'APPLICATION VERIFIERS RUN 1' { & 'applications/ci/Run-Application-Verifiers.ps1' }
    Invoke-Checked 'APPLICATION VERIFIERS RUN 2' { & 'applications/ci/Run-Application-Verifiers.ps1' }

    $foundationContracts = Join-Path $foundationRoot 'src\Foundation.Contracts\bin\Release\net10.0\Foundation.Contracts.dll'
    $foundationState = Join-Path $foundationRoot 'src\Foundation.State\bin\Release\net10.0\Foundation.State.dll'
    $foundationRouting = Join-Path $foundationRoot 'src\Foundation.MessageRouting\bin\Release\net10.0\Foundation.MessageRouting.dll'
    $foundationDelivery = Join-Path $foundationRoot 'src\Foundation.MessageDelivery\bin\Release\net10.0\Foundation.MessageDelivery.dll'
    $foundationEvents = Join-Path $foundationRoot 'src\Foundation.EventSystem\bin\Release\net10.0\Foundation.EventSystem.dll'
    $compatProject = 'applications/FSATS/tests/FoundationCompatibility/Falcon.FSATS.FoundationCompatibility.Verifier/Falcon.FSATS.FoundationCompatibility.Verifier.csproj'

    Invoke-Checked 'EXACT FOUNDATION STRUCTURAL COMPATIBILITY' {
        dotnet run --project $compatProject -c Release -- `
            $foundationContracts $foundationState $foundationRouting $foundationDelivery $foundationEvents
    }

    if (@(git status --porcelain).Count -gt 0) { throw 'Binding validation mutated the Application checkout.' }

    @(
        'Status=PASS',
        "ApplicationHead=$appHead",
        "FoundationCommit=$FoundationCommit",
        "DotNetSdk=$sdk",
        'WindowsLongPathMitigation=ENABLED_BY_SHORT_SCRATCH_AND_GIT_CORE_LONGPATHS',
        'FoundationReferenceBuild=PASS',
        'ApplicationBuild=PASS',
        'ApplicationVerifierRun1=PASS',
        'ApplicationVerifierRun2=PASS',
        'FoundationStructuralCompatibility=PASS',
        'FCR0004GuardianProtectionRouteApplicationFixtures=INCLUDED',
        'FCR0005FSAPMAOperationalDataDeliveryApplicationFixtures=INCLUDED',
        'FCR0006EventEvidenceReplayApplicationFixtures=INCLUDED',
        'FoundationMessagingRouteDeliveryEventAssemblies=STRUCTURALLY_VERIFIED',
        'ProductionRuntimeBinding=NOT_GRANTED',
        'CanonicalCrossBranchArtifactConsumption=NOT_ESTABLISHED',
        'ApplicationWorkingTree=CLEAN'
    ) | Set-Content -Path (Join-Path $runDir 'SUMMARY.txt') -Encoding utf8

    Write-Host ""
    Write-Host 'PART 2 FOUNDATION BINDING VALIDATION: PASS'
}
catch {
    $failure = $_
    if (-not (Test-Path $runDir)) { New-Item -ItemType Directory -Force -Path $runDir | Out-Null }
    @(
        'Status=FAIL',
        "ApplicationHead=$(try { (git rev-parse HEAD).Trim() } catch { 'UNKNOWN' })",
        "FoundationCommit=$FoundationCommit",
        "Error=$($_.Exception.Message)"
    ) | Set-Content -Path (Join-Path $runDir 'SUMMARY.txt') -Encoding utf8
    Write-Host ""
    Write-Host 'PART 2 FOUNDATION BINDING VALIDATION: FAIL'
    Write-Host "ERROR: $($_.Exception.Message)"
}
finally {
    Pop-Location -ErrorAction SilentlyContinue
    if ($transcript -and (Test-Path $transcript)) { try { Stop-Transcript | Out-Null } catch { } }
    if (Test-Path $scratchRunDir) {
        try { Remove-Item -LiteralPath $scratchRunDir -Recurse -Force -ErrorAction Stop } catch {
            Write-Host "WARNING: Could not remove scratch directory: $scratchRunDir"
        }
    }
    if (-not (Test-Path $runDir)) { New-Item -ItemType Directory -Force -Path $runDir | Out-Null }
    $zip = "$runDir.zip"
    Compress-Archive -Path (Join-Path $runDir '*') -DestinationPath $zip -Force
    Write-Host "RESULT ZIP: $zip"
}

if ($failure) { throw $failure }
