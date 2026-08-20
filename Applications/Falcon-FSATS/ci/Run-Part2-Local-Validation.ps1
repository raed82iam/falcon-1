param(
    [string]$ResultsDirectory = 'C:\falcon\Application test-results'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$runDir = Join-Path $ResultsDirectory $timestamp
$transcript = $null
$failure = $null

function Invoke-Checked {
    param([string]$Name, [scriptblock]$Action)

    Write-Host ""
    Write-Host "========== $Name =========="

    $global:LASTEXITCODE = 0
    & $Action

    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE"
    }

    Write-Host "$Name : PASS"
}

try {
    Push-Location $repoRoot

    # Remove only the known untracked legacy sandbox left by the superseded local-test harness.
    # Refuse to remove it if Git ever tracks content under that path.
    $legacySandbox = Join-Path $repoRoot '.part2-test-env'
    if (Test-Path $legacySandbox) {
        $trackedLegacy = @(git ls-files -- '.part2-test-env' '.part2-test-env/**')
        if ($trackedLegacy.Count -gt 0) {
            throw "Refusing to remove legacy sandbox because Git tracks content under .part2-test-env."
        }
        Remove-Item -LiteralPath $legacySandbox -Recurse -Force
    }

    # The checkout must be proven clean BEFORE any validation output is created.
    $head = (git rev-parse HEAD).Trim()
    $branch = (git branch --show-current).Trim()
    $status = @(git status --porcelain)

    if ($branch -ne 'application-development') {
        throw "Expected branch application-development, got '$branch'."
    }
    if ($status.Count -gt 0) {
        throw "Working tree is not clean. Local validation requires an exact clean checkout. Changed paths: $($status -join ', ')"
    }

    # Test evidence and temporary SDK/NuGet state deliberately live outside the Git checkout.
    New-Item -ItemType Directory -Force -Path $runDir | Out-Null
    $transcript = Join-Path $runDir 'Part2-Validation.log'
    Start-Transcript -Path $transcript -Force | Out-Null

    Write-Host "FALCON FSATS PART 2 LOCAL VALIDATION"
    Write-Host "Repository: $repoRoot"
    Write-Host "Results:    $runDir"
    Write-Host "Branch:     $branch"
    Write-Host "HEAD:       $head"

    $sdk = (dotnet --version).Trim()
    Write-Host "dotnet SDK: $sdk"
    if ($sdk -ne '10.0.302') {
        throw "Falcon requires exact .NET SDK 10.0.302. Installed/selected SDK is '$sdk'."
    }

    $sandbox = Join-Path $ResultsDirectory '.part2-test-env'
    $env:DOTNET_CLI_HOME = Join-Path $sandbox 'DotNetCliHome'
    $env:NUGET_PACKAGES = Join-Path $sandbox 'NuGet\Packages'
    $env:NUGET_HTTP_CACHE_PATH = Join-Path $sandbox 'NuGet\HttpCache'
    $env:NUGET_PLUGINS_CACHE_PATH = Join-Path $sandbox 'NuGet\PluginsCache'
    $env:TEMP = Join-Path $sandbox 'Temp'
    $env:TMP = $env:TEMP
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    New-Item -ItemType Directory -Force -Path $env:DOTNET_CLI_HOME,$env:NUGET_PACKAGES,$env:NUGET_HTTP_CACHE_PATH,$env:NUGET_PLUGINS_CACHE_PATH,$env:TEMP | Out-Null

    Invoke-Checked 'FOUNDATION RESTORE' { dotnet restore 'Falcon.Foundation.ControlledProjectFoundation.slnx' }
    Invoke-Checked 'FOUNDATION RELEASE BUILD' { dotnet build 'Falcon.Foundation.ControlledProjectFoundation.slnx' -c Release --no-restore }

    Invoke-Checked 'APPLICATION RESTORE' { dotnet restore 'applications/Falcon.Applications.slnx' }
    Invoke-Checked 'APPLICATION RELEASE BUILD' { dotnet build 'applications/Falcon.Applications.slnx' -c Release --no-restore }

    Invoke-Checked 'APPLICATION VERIFIERS RUN 1' { & 'applications/ci/Run-Application-Verifiers.ps1' }
    Invoke-Checked 'APPLICATION VERIFIERS RUN 2' { & 'applications/ci/Run-Application-Verifiers.ps1' }

    $afterStatus = @(git status --porcelain)
    if ($afterStatus.Count -gt 0) {
        throw "Validation mutated the governed checkout. Changed paths: $($afterStatus -join ', ')"
    }

    @(
        'Status=PASS',
        "Branch=$branch",
        "Head=$head",
        "DotNetSdk=$sdk",
        'FoundationBuild=PASS',
        'ApplicationBuild=PASS',
        'VerifierRun1=PASS',
        'VerifierRun2=PASS',
        'WorkingTreeAfterValidation=CLEAN'
    ) | Set-Content -Path (Join-Path $runDir 'SUMMARY.txt') -Encoding utf8

    Write-Host ""
    Write-Host 'PART 2 LOCAL EXECUTABLE VALIDATION: PASS'
}
catch {
    $failure = $_

    if (-not (Test-Path $runDir)) {
        New-Item -ItemType Directory -Force -Path $runDir | Out-Null
    }

    @(
        'Status=FAIL',
        "Error=$($_.Exception.Message)"
    ) | Set-Content -Path (Join-Path $runDir 'SUMMARY.txt') -Encoding utf8

    Write-Host ""
    Write-Host "PART 2 LOCAL EXECUTABLE VALIDATION: FAIL"
    Write-Host "ERROR: $($_.Exception.Message)"
}
finally {
    Pop-Location -ErrorAction SilentlyContinue

    if ($transcript -and (Test-Path $transcript)) {
        try { Stop-Transcript | Out-Null } catch { }
    }

    if (-not (Test-Path $runDir)) {
        New-Item -ItemType Directory -Force -Path $runDir | Out-Null
    }

    $zip = "$runDir.zip"
    Compress-Archive -Path (Join-Path $runDir '*') -DestinationPath $zip -Force
    Write-Host "RESULT ZIP: $zip"
}

if ($failure) {
    throw $failure
}
