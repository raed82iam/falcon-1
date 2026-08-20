[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$part1Root = Split-Path -Parent $scriptRoot
$fsatsRoot = Split-Path -Parent $part1Root
$repoRoot = Resolve-Path (Join-Path $fsatsRoot '..\..')
$solution = Join-Path $fsatsRoot 'Falcon.FSATS.Part1.slnx'

function Fail([string]$Message) {
    Write-Error $Message
    exit 1
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Fail 'DOTNET_SDK_NOT_AVAILABLE'
}

if (-not (Test-Path $solution -PathType Leaf)) {
    Fail "PART1_SOLUTION_NOT_FOUND: $solution"
}

Push-Location $repoRoot
try {
    $branch = (& git branch --show-current).Trim()
    if ($LASTEXITCODE -ne 0 -or $branch -ne 'application-development') {
        Fail "WRONG_BRANCH: expected application-development, actual '$branch'"
    }

    $applicationRoot = Join-Path $repoRoot 'applications\FSATS'

    $forbiddenSourcePatterns = @(
        'System\.Net\.',
        '\bHttpClient\b',
        '\bSocket\b',
        '\bWebRequest\b',
        '\bDllImport\b',
        '\bProcess\.Start\b'
    )

    $sourceFiles = Get-ChildItem -Path (Join-Path $applicationRoot 'src') -Recurse -File -Filter *.cs |
        Where-Object {
            $_.FullName -notmatch '[\\/](bin|obj)[\\/]'
        }

    foreach ($pattern in $forbiddenSourcePatterns) {
        $matches = $sourceFiles | Select-String -Pattern $pattern
        if ($matches) {
            Fail "FORBIDDEN_PART1_RUNTIME_API: pattern '$pattern' found in $($matches.Path -join ', ')"
        }
    }

    $projectFiles = Get-ChildItem -Path $applicationRoot -Recurse -File -Filter *.csproj |
        Where-Object {
            $_.FullName -notmatch '[\\/](bin|obj)[\\/]'
        }

    foreach ($project in $projectFiles) {
        $text = Get-Content $project.FullName -Raw
        if ($text -match '<ProjectReference\s+Include="[^"]*src\\Foundation\.') {
            Fail "DIRECT_FOUNDATION_SOURCE_REFERENCE_FORBIDDEN: $($project.FullName)"
        }
    }

    Write-Host 'P1F STATIC SECURITY/BOUNDARY SCAN PASS'

    & dotnet restore $solution
    if ($LASTEXITCODE -ne 0) { Fail 'PART1_RESTORE_FAILED' }

    & dotnet build $solution -c Release --no-restore
    if ($LASTEXITCODE -ne 0) { Fail 'PART1_RELEASE_BUILD_FAILED' }

    $verifiers = @(
        'verification/Falcon.FSATS.Part1.Primitives.Verifier/Falcon.FSATS.Part1.Primitives.Verifier.csproj',
        'verification/Falcon.FSATS.Part1.Shells.Verifier/Falcon.FSATS.Part1.Shells.Verifier.csproj',
        'verification/Falcon.FSATS.Part1.ContractSpine.Verifier/Falcon.FSATS.Part1.ContractSpine.Verifier.csproj',
        'verification/Falcon.FSATS.Part1.FoundationBindings.Verifier/Falcon.FSATS.Part1.FoundationBindings.Verifier.csproj',
        'verification/Falcon.FSATS.Part1.Verifier/Falcon.FSATS.Part1.Verifier.csproj'
    )

    for ($pass = 1; $pass -le 2; $pass++) {
        Write-Host "PART1 VERIFIER PASS $pass/2"
        foreach ($relativeProject in $verifiers) {
            $project = Join-Path $fsatsRoot $relativeProject
            & dotnet run --project $project -c Release --no-build
            if ($LASTEXITCODE -ne 0) {
                Fail "PART1_VERIFIER_FAILED: pass=$pass project=$relativeProject"
            }
        }
    }

    Write-Host 'FSATS_PART1_EXECUTION_VALIDATION_PASS'
    Write-Host 'PART2_THROUGH_PART10_NOT_AUTHORIZED'
    Write-Host 'RUNTIME_TRADING_AUTHORITY_NOT_GRANTED'
    exit 0
}
finally {
    Pop-Location
}
