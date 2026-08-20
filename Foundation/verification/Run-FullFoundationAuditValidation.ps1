param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F]{40}$')]
    [string]$ExpectedCommit,

    [string]$TestRoot = 'C:\Falcon\Foundation-Audit-Validation'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

if (Test-Path variable:PSNativeCommandUseErrorActionPreference) {
    $PSNativeCommandUseErrorActionPreference = $false
}

$RepoRoot = Join-Path $TestRoot 'Falcon'
$LogsRoot = Join-Path $TestRoot 'Logs'
$Solution = 'Falcon.Foundation.ControlledProjectFoundation.slnx'
$ArchitectureProject = 'tests\Falcon.Foundation.Architecture.Tests\Falcon.Foundation.Architecture.Tests.csproj'
$SecurityProject = 'tests\Falcon.Foundation.Security.Tests\Falcon.Foundation.Security.Tests.csproj'
$RepositorySecurityProject = 'verification\Falcon.RepositorySecuritySurface.Verifier\Falcon.RepositorySecuritySurface.Verifier.csproj'
$Fcr0241Project = 'verification\Falcon.Fcr0241.OwnerGovernanceTransport.Verifier\Falcon.Fcr0241.OwnerGovernanceTransport.Verifier.csproj'
$Stage0CRemediationProject = 'verification\Falcon.Stage0C.RemediationVerifier\Falcon.Stage0C.RemediationVerifier.csproj'
$Stage0CRemediationEvidence = Join-Path $LogsRoot 'stage0c-remediation-evidence.json'
$Stage0CRemediationTrace = Join-Path $LogsRoot 'stage0c-remediation-trace.json'

function Invoke-NativeLogged {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Executable,
        [Parameter(Mandatory = $true)][string[]]$Arguments,
        [Parameter(Mandatory = $true)][string]$LogFile
    )

    Write-Host ''
    Write-Host '============================================================'
    Write-Host $Name
    Write-Host '============================================================'

    $previous = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        & $Executable @Arguments 2>&1 | Tee-Object -FilePath $LogFile
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previous
    }

    if ($exitCode -ne 0) {
        throw "$Name FAILED with exit code $exitCode. See $LogFile"
    }

    Write-Host "$Name = PASS"
}

function Get-RelativePathCompat {
    param(
        [Parameter(Mandatory = $true)][string]$BasePath,
        [Parameter(Mandatory = $true)][string]$TargetPath
    )

    $baseFull = [System.IO.Path]::GetFullPath($BasePath).TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    $targetFull = [System.IO.Path]::GetFullPath($TargetPath)
    $prefix = $baseFull + [System.IO.Path]::DirectorySeparatorChar

    if (-not $targetFull.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "TARGET PATH IS OUTSIDE REPOSITORY ROOT. base=$baseFull target=$targetFull"
    }

    return $targetFull.Substring($prefix.Length)
}

Write-Host ''
Write-Host '============================================================'
Write-Host 'FALCON FOUNDATION - FULL AUDIT REMEDIATION VALIDATION'
Write-Host '============================================================'
Write-Host "Expected commit: $ExpectedCommit"
Write-Host "Test root      : $TestRoot"

if (Test-Path $TestRoot) {
    Remove-Item -LiteralPath $TestRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $TestRoot | Out-Null
New-Item -ItemType Directory -Path $LogsRoot | Out-Null

$env:DOTNET_CLI_HOME = Join-Path $TestRoot 'DotNetCliHome'
$env:NUGET_PACKAGES = Join-Path $TestRoot 'NuGet\Packages'
$env:NUGET_HTTP_CACHE_PATH = Join-Path $TestRoot 'NuGet\HttpCache'
$env:NUGET_PLUGINS_CACHE_PATH = Join-Path $TestRoot 'NuGet\PluginsCache'
$env:TEMP = Join-Path $TestRoot 'Temp'
$env:TMP = $env:TEMP
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'

@(
    $env:DOTNET_CLI_HOME,
    $env:NUGET_PACKAGES,
    $env:NUGET_HTTP_CACHE_PATH,
    $env:NUGET_PLUGINS_CACHE_PATH,
    $env:TEMP
) | ForEach-Object {
    New-Item -ItemType Directory -Path $_ -Force | Out-Null
}

Invoke-NativeLogged `
    -Name 'CLONE FOUNDATION-DEVELOPMENT' `
    -Executable 'git' `
    -Arguments @('clone', '--branch', 'foundation-development', '--single-branch', 'https://github.com/raed82iam/Falcon.git', $RepoRoot) `
    -LogFile (Join-Path $LogsRoot '00-clone.log')

Set-Location $RepoRoot

Invoke-NativeLogged `
    -Name 'CHECKOUT EXACT CANDIDATE' `
    -Executable 'git' `
    -Arguments @('checkout', '--detach', $ExpectedCommit) `
    -LogFile (Join-Path $LogsRoot '01-checkout.log')

$actualCommit = (git rev-parse HEAD).Trim()
if ($actualCommit -ne $ExpectedCommit.ToLowerInvariant()) {
    throw "COMMIT MISMATCH. expected=$ExpectedCommit actual=$actualCommit"
}

if (git status --porcelain) {
    throw 'FRESH CHECKOUT IS NOT CLEAN.'
}

$dotnetVersion = (& dotnet --version).Trim()
if ($LASTEXITCODE -ne 0) {
    throw 'dotnet --version failed.'
}
if ($dotnetVersion -ne '10.0.302') {
    throw "GOVERNED SDK MISMATCH. Expected 10.0.302, actual $dotnetVersion"
}
Write-Host "GOVERNED SDK = $dotnetVersion"

Invoke-NativeLogged `
    -Name 'CONTROLLED RESTORE' `
    -Executable 'dotnet' `
    -Arguments @('restore', $Solution) `
    -LogFile (Join-Path $LogsRoot '02-restore.log')

Invoke-NativeLogged `
    -Name 'CONTROLLED RELEASE BUILD' `
    -Executable 'dotnet' `
    -Arguments @('build', $Solution, '-c', 'Release', '--no-restore') `
    -LogFile (Join-Path $LogsRoot '03-build.log')

Invoke-NativeLogged `
    -Name 'STAGE0C REMEDIATION VERIFIER RESTORE' `
    -Executable 'dotnet' `
    -Arguments @('restore', $Stage0CRemediationProject) `
    -LogFile (Join-Path $LogsRoot '03a-stage0c-remediation-restore.log')

Invoke-NativeLogged `
    -Name 'STAGE0C REMEDIATION VERIFIER RELEASE BUILD' `
    -Executable 'dotnet' `
    -Arguments @('build', $Stage0CRemediationProject, '-c', 'Release', '--no-restore') `
    -LogFile (Join-Path $LogsRoot '03b-stage0c-remediation-build.log')

Invoke-NativeLogged `
    -Name 'ARCHITECTURE VERIFICATION' `
    -Executable 'dotnet' `
    -Arguments @('run', '--project', $ArchitectureProject, '-c', 'Release', '--no-build') `
    -LogFile (Join-Path $LogsRoot '04-architecture.log')

Invoke-NativeLogged `
    -Name 'BASELINE SECURITY VERIFICATION' `
    -Executable 'dotnet' `
    -Arguments @('run', '--project', $SecurityProject, '-c', 'Release', '--no-build') `
    -LogFile (Join-Path $LogsRoot '05-security.log')

Invoke-NativeLogged `
    -Name 'REPOSITORY SECURITY SURFACE' `
    -Executable 'dotnet' `
    -Arguments @('run', '--project', $RepositorySecurityProject, '-c', 'Release', '--no-build') `
    -LogFile (Join-Path $LogsRoot '06-repository-security.log')

$verifiers = @(
    Get-ChildItem -Path (Join-Path $RepoRoot 'verification') -Filter '*.csproj' -Recurse -File |
        Where-Object { $_.FullName -notlike '*Falcon.RepositorySecuritySurface.Verifier*' } |
        Sort-Object FullName
)

if ($verifiers.Count -lt 1) {
    throw 'NO GOVERNED VERIFIER PROJECTS DISCOVERED.'
}

$failed = @()
$executed = 0
$index = 0
foreach ($project in $verifiers) {
    $index++
    $relative = Get-RelativePathCompat -BasePath $RepoRoot -TargetPath $project.FullName
    $safeName = ($relative -replace '[^A-Za-z0-9._-]', '_')
    $log = Join-Path $LogsRoot ("07-verifier-{0:D3}-{1}.log" -f $index, $safeName)

    Write-Host ''
    Write-Host "VERIFY [$index/$($verifiers.Count)]: $relative"

    $previous = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        if ($relative -ieq $Stage0CRemediationProject) {
            & dotnet run --project $project.FullName -c Release --no-build -- `
                --evidence $Stage0CRemediationEvidence `
                --trace $Stage0CRemediationTrace `
                --root $RepoRoot 2>&1 | Tee-Object -FilePath $log
        }
        else {
            & dotnet run --project $project.FullName -c Release --no-build 2>&1 | Tee-Object -FilePath $log
        }
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previous
    }

    $executed++
    if ($exitCode -ne 0) {
        $failed += "$relative (exit $exitCode)"
    }
}

if ($executed -ne $verifiers.Count) {
    throw "VERIFIER COVERAGE MISMATCH. executed=$executed discovered=$($verifiers.Count)"
}

if ($failed.Count -gt 0) {
    $failedText = $failed -join "`n"
    throw "GOVERNED VERIFIER FAILURES:`n$failedText"
}

if (-not (Test-Path $Stage0CRemediationEvidence -PathType Leaf)) {
    throw 'STAGE0C REMEDIATION EVIDENCE FILE WAS NOT PRODUCED.'
}
if (-not (Test-Path $Stage0CRemediationTrace -PathType Leaf)) {
    throw 'STAGE0C REMEDIATION TRACE FILE WAS NOT PRODUCED.'
}
Write-Host 'STAGE0C_REMEDIATION_EVIDENCE = PASS'
Write-Host 'STAGE0C_REMEDIATION_TRACE = PASS'

function Invoke-Fcr0241Capture {
    param([string]$LogName)

    $previous = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        $output = @(
            & dotnet run --project $Fcr0241Project -c Release --no-build 2>&1
        )
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previous
    }

    $output | Tee-Object -FilePath (Join-Path $LogsRoot $LogName) | Out-Host
    if ($exitCode -ne 0) {
        throw "FCR-0241 verifier rerun failed: $LogName"
    }

    $text = $output -join "`n"
    if ($text -notmatch 'FCR0241_OWNER_GOVERNANCE_TRANSPORT_VERIFIER = PASS') {
        throw "FCR-0241 PASS marker missing: $LogName"
    }

    $digestLine = @($output | Where-Object { $_ -match '^DETERMINISTIC_IDENTITY_SHA256 = [0-9A-F]{64}$' })
    if ($digestLine.Count -ne 1) {
        throw "FCR-0241 deterministic identity digest missing or ambiguous: $LogName"
    }

    return $digestLine[0]
}

$digest1 = Invoke-Fcr0241Capture -LogName '08-fcr0241-rerun-1.log'
$digest2 = Invoke-Fcr0241Capture -LogName '09-fcr0241-rerun-2.log'

if ($digest1 -ne $digest2) {
    throw "FCR-0241 DETERMINISTIC IDENTITY MISMATCH.`n$digest1`n$digest2"
}

$finalCommit = (git rev-parse HEAD).Trim()
if ($finalCommit -ne $ExpectedCommit.ToLowerInvariant()) {
    throw 'HEAD CHANGED DURING VALIDATION.'
}
if (git status --porcelain) {
    git status --short
    throw 'VALIDATION MODIFIED THE REPOSITORY.'
}

Write-Host ''
Write-Host '============================================================'
Write-Host 'FALCON FOUNDATION FULL AUDIT REMEDIATION VALIDATION = PASS'
Write-Host '============================================================'
Write-Host "EXACT_COMMIT = $finalCommit"
Write-Host 'RESTORE = PASS'
Write-Host 'RELEASE_BUILD = PASS'
Write-Host 'STAGE0C_REMEDIATION_RESTORE = PASS'
Write-Host 'STAGE0C_REMEDIATION_BUILD = PASS'
Write-Host 'ARCHITECTURE = PASS'
Write-Host 'BASELINE_SECURITY = PASS'
Write-Host 'REPOSITORY_SECURITY_SURFACE = PASS'
Write-Host "GOVERNED_VERIFIERS_EXECUTED = $executed"
Write-Host 'ALL_GOVERNED_VERIFIERS = PASS'
Write-Host 'STAGE0C_REMEDIATION_EVIDENCE = PASS'
Write-Host 'STAGE0C_REMEDIATION_TRACE = PASS'
Write-Host 'FCR0241_RERUN_1 = PASS'
Write-Host 'FCR0241_RERUN_2 = PASS'
Write-Host 'FCR0241_DETERMINISTIC_IDENTITY = PASS'
Write-Host 'WORKING_TREE_CLEAN = PASS'
Write-Host "LOGS = $LogsRoot"
