param(
    [Parameter(Mandatory = $true)]
    [string]$ExpectedApplicationCommit,

    [Parameter(Mandatory = $true)]
    [string]$ExpectedFoundationCommit,

    [string]$TestRoot = 'C:\F11'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Require([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}

function Run([string]$label, [scriptblock]$action) {
    Write-Host ''
    Write-Host "=== $label ==="
    & $action
    if ($LASTEXITCODE -ne 0) { throw "$label failed with exit code $LASTEXITCODE" }
}

$repoUrl = 'https://github.com/raed82iam/Falcon.git'
$appRoot = Join-Path $TestRoot 'A'
$foundationRoot = Join-Path $TestRoot 'F'

Write-Host 'FALCON FSATS PART 11 - RUNTIME ONBOARDING / ADMISSION & BINDING VALIDATION'
Write-Host "Application expected: $ExpectedApplicationCommit"
Write-Host "Foundation expected : $ExpectedFoundationCommit"
Write-Host "Test root           : $TestRoot"

if (Test-Path $TestRoot) { Remove-Item $TestRoot -Recurse -Force }
New-Item -ItemType Directory -Path $TestRoot -Force | Out-Null

$dotnet = (dotnet --version).Trim()
Require ($dotnet -eq '10.0.302') "Expected .NET SDK 10.0.302 but found $dotnet"
Write-Host "DOTNET_SDK = $dotnet"

Run 'Clone exact Application branch' {
    git -c core.longpaths=true clone --branch application-development --single-branch $repoUrl $appRoot
}
Push-Location $appRoot
try {
    Run 'Fetch Application' { git -c core.longpaths=true fetch origin application-development }
    Run 'Checkout exact Application commit' { git -c core.longpaths=true checkout --detach $ExpectedApplicationCommit }
    $actualApp = (git rev-parse HEAD).Trim()
    Require ($actualApp -eq $ExpectedApplicationCommit) "Application HEAD mismatch: $actualApp"
    Require (@(git -c core.longpaths=true status --porcelain).Count -eq 0) 'Application working tree is not clean before validation.'
}
finally { Pop-Location }

Run 'Clone exact Foundation branch' {
    git -c core.longpaths=true clone --branch foundation-development --single-branch $repoUrl $foundationRoot
}
Push-Location $foundationRoot
try {
    Run 'Fetch Foundation' { git -c core.longpaths=true fetch origin foundation-development }
    Run 'Checkout exact Foundation commit' { git -c core.longpaths=true checkout --detach $ExpectedFoundationCommit }
    $actualFoundation = (git rev-parse HEAD).Trim()
    Require ($actualFoundation -eq $ExpectedFoundationCommit) "Foundation HEAD mismatch: $actualFoundation"
    Require (@(git -c core.longpaths=true status --porcelain).Count -eq 0) 'Foundation working tree is not clean before validation.'

    Run 'Restore Foundation Admission' {
        dotnet restore 'src/Foundation.Admission/Foundation.Admission.csproj'
    }
    Run 'Build Foundation Admission Release' {
        dotnet build 'src/Foundation.Admission/Foundation.Admission.csproj' -c Release --no-restore
    }
    Run 'Restore Foundation Runtime Hosting' {
        dotnet restore 'src/Foundation.ApplicationRuntimeHosting/Foundation.ApplicationRuntimeHosting.csproj'
    }
    Run 'Build Foundation Runtime Hosting Release' {
        dotnet build 'src/Foundation.ApplicationRuntimeHosting/Foundation.ApplicationRuntimeHosting.csproj' -c Release --no-restore
    }
}
finally { Pop-Location }

Push-Location $appRoot
try {
    Run 'Restore Application solution' {
        dotnet restore 'applications/Falcon.Applications.slnx'
    }
    Run 'Build Application solution Release' {
        dotnet build 'applications/Falcon.Applications.slnx' -c Release --no-restore
    }
    Run 'Application dotnet test' {
        dotnet test 'applications/Falcon.Applications.slnx' -c Release --no-build
    }

    Run 'Governed Application verifiers run 1' {
        & 'applications/ci/Run-Application-Verifiers.ps1'
    }
    Run 'Governed Application verifiers run 2' {
        & 'applications/ci/Run-Application-Verifiers.ps1'
    }

    $admissionDll = Join-Path $foundationRoot 'src\Foundation.Admission\bin\Release\net10.0\Foundation.Admission.dll'
    $runtimeDll = Join-Path $foundationRoot 'src\Foundation.ApplicationRuntimeHosting\bin\Release\net10.0\Foundation.ApplicationRuntimeHosting.dll'
    Require (Test-Path $admissionDll -PathType Leaf) "Foundation Admission DLL missing: $admissionDll"
    Require (Test-Path $runtimeDll -PathType Leaf) "Foundation Runtime Hosting DLL missing: $runtimeDll"

    Run 'Cross-branch sealed Foundation onboarding compatibility' {
        dotnet run --project 'applications/FSATS/tests/FoundationCompatibility/Falcon.FSATS.CrossBranchFoundationOnboarding.Verifier/Falcon.FSATS.CrossBranchFoundationOnboarding.Verifier.csproj' -c Release --no-build -- $admissionDll $runtimeDll
    }

    $appFinal = (git rev-parse HEAD).Trim()
    $appDirty = @(git -c core.longpaths=true status --porcelain)
    Require ($appFinal -eq $ExpectedApplicationCommit) "Application final HEAD changed: $appFinal"
    Require ($appDirty.Count -eq 0) 'Application working tree changed during validation.'
}
finally { Pop-Location }

Push-Location $foundationRoot
try {
    $foundationFinal = (git rev-parse HEAD).Trim()
    $foundationDirty = @(git -c core.longpaths=true status --porcelain)
    Require ($foundationFinal -eq $ExpectedFoundationCommit) "Foundation final HEAD changed: $foundationFinal"
    Require ($foundationDirty.Count -eq 0) 'Foundation working tree changed during validation.'
}
finally { Pop-Location }

Write-Host ''
Write-Host '============================================================'
Write-Host 'PART 11 EXACT EXECUTABLE VALIDATION = PASS'
Write-Host "APPLICATION_HEAD             = $ExpectedApplicationCommit"
Write-Host "FOUNDATION_HEAD              = $ExpectedFoundationCommit"
Write-Host 'DOTNET_SDK                    = 10.0.302'
Write-Host 'APPLICATION_RESTORE          = PASS'
Write-Host 'APPLICATION_BUILD            = PASS'
Write-Host 'APPLICATION_DOTNET_TEST      = PASS'
Write-Host 'APPLICATION_VERIFIERS_RUN_1  = PASS'
Write-Host 'APPLICATION_VERIFIERS_RUN_2  = PASS'
Write-Host 'CROSS_BRANCH_ONBOARDING      = PASS'
Write-Host 'APPLICATION_WORKING_TREE     = CLEAN'
Write-Host 'FOUNDATION_WORKING_TREE      = CLEAN'
Write-Host 'RUNTIME_ACTIVATION           = NOT_AUTHORIZED / NOT_EXECUTED'
Write-Host 'PROVIDER_BROKER_CONNECTIVITY = NOT_AUTHORIZED / NOT_EXECUTED'
Write-Host 'PAPER_LIVE                    = NOT_AUTHORIZED / NOT_EXECUTED'
Write-Host 'FAILED_CHECKS                 = 0'
Write-Host '============================================================'
