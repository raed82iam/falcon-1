$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$projects = @(
    'applications/FSATS/tests/Architecture/Falcon.FSATS.Architecture.Verifier/Falcon.FSATS.Architecture.Verifier.csproj',
    'applications/FSATS/tests/Security/Falcon.FSATS.Security.Verifier/Falcon.FSATS.Security.Verifier.csproj',
    'applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/Falcon.FSATS.Behavior.Verifier.csproj',
    'applications/FSATS/tests/Behavior/Falcon.FSATS.OperationalDataOutcome.Verifier/Falcon.FSATS.OperationalDataOutcome.Verifier.csproj',
    'applications/FSATS/tests/Behavior/Falcon.FSATS.OwnerUpdateGovernance.Verifier/Falcon.FSATS.OwnerUpdateGovernance.Verifier.csproj',
    'applications/FSATS/tests/Behavior/Falcon.FSATS.FoundationBinding.Verifier/Falcon.FSATS.FoundationBinding.Verifier.csproj',
    'applications/FSATS/tests/Behavior/Falcon.FSATS.OwnerFeatureEntitlement.Verifier/Falcon.FSATS.OwnerFeatureEntitlement.Verifier.csproj',
    'applications/FSATS/tests/FoundationCompatibility/Falcon.FSATS.FoundationOnboarding.Verifier/Falcon.FSATS.FoundationOnboarding.Verifier.csproj',
    'applications/FSATS/tests/Integration/Falcon.FSATS.Integration.Verifier/Falcon.FSATS.Integration.Verifier.csproj',
    'applications/FSATS/tests/Failure/Falcon.FSATS.Failure.Verifier/Falcon.FSATS.Failure.Verifier.csproj'
)

Push-Location $repoRoot
try {
    $failures = @()
    foreach ($project in $projects) {
        Write-Host ""
        Write-Host "=== RUN VERIFIER: $project ==="
        dotnet run --project $project -c Release --no-build
        if ($LASTEXITCODE -ne 0) {
            $failures += "$project => exit $LASTEXITCODE"
        }
    }

    if ($failures.Count -gt 0) {
        Write-Host ""
        Write-Error ("APPLICATION VERIFIERS FAILED:`n - " + ($failures -join "`n - "))
        exit 1
    }

    Write-Host ""
    Write-Host "APPLICATION VERIFIERS: PASS ($($projects.Count)/$($projects.Count))"
    exit 0
}
finally {
    Pop-Location
}
