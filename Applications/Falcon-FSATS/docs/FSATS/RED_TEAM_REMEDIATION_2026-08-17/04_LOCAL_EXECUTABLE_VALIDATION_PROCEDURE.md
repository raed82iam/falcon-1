# FSATS Red-Team Remediation — Exact Local Executable Validation Procedure

Date: 2026-08-17
Target branch: `application-development`
Exact source/test candidate: `f3d09d7b226e1d239f2b5dc963130c88c195d965`
Pre-remediation comparison base: `76c8f5aadb83193bc54405dce6d3c574c6412d59`
Required SDK: `10.0.302`

This procedure mirrors the governed `Falcon Application CI` build/test/verifier sequence while also validating the remediation ownership boundary and rerunning all six Application verifiers for deterministic confirmation.

It does not grant runtime, provider, broker, Paper, Shadow, Tiny-Live, Live, deployment, or AI release/revival authority.

## Windows long-path correction

The first Owner-machine validation attempt reached the exact candidate but Git checkout could not materialize several historically long repository paths under the original deep test root. The resulting tracked deletions correctly caused the clean-tree gate to fail before any restore/build/test step ran.

This corrected procedure:

- uses the deliberately short validation root `C:\FAV`;
- clones with `--no-checkout` so the default branch is not materialized before long-path support is configured;
- sets repository-local `core.longpaths=true` before the exact candidate checkout;
- preserves the exact source/test candidate, ownership-boundary check, clean-tree check, SDK pin, build/test/verifier sequence, and fail-closed evidence behavior.

The prior path-length failure is an environment/setup failure, not executable evidence for or against the candidate.

## PowerShell procedure

Run the entire block in PowerShell on the Owner validation machine:

```powershell
& {
    $ErrorActionPreference = "Stop"
    Set-StrictMode -Version Latest

    # Keep the checkout path intentionally short because Falcon contains
    # governed historical records whose full Windows paths can exceed legacy limits.
    $TestRoot = "C:\FAV"
    $RepoRoot = Join-Path $TestRoot "R"
    $Transcript = Join-Path $TestRoot "Validation-Transcript.txt"
    $Summary = Join-Path $TestRoot "Validation-Summary.txt"
    $Zip = Join-Path $TestRoot "Validation-Evidence.zip"

    $ExpectedCommit = "f3d09d7b226e1d239f2b5dc963130c88c195d965"
    $ComparisonBase = "76c8f5aadb83193bc54405dce6d3c574c6412d59"
    $ExpectedSdk = "10.0.302"

    $DotNetHome = Join-Path $TestRoot "D"
    $NuGetRoot = Join-Path $TestRoot "N"
    $TempRoot = Join-Path $TestRoot "T"

    New-Item -ItemType Directory -Force -Path $TestRoot, $DotNetHome, $NuGetRoot, $TempRoot | Out-Null

    $env:DOTNET_CLI_HOME = $DotNetHome
    $env:NUGET_PACKAGES = Join-Path $NuGetRoot "P"
    $env:NUGET_HTTP_CACHE_PATH = Join-Path $NuGetRoot "H"
    $env:NUGET_PLUGINS_CACHE_PATH = Join-Path $NuGetRoot "C"
    $env:TEMP = $TempRoot
    $env:TMP = $TempRoot
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

    New-Item -ItemType Directory -Force -Path $env:NUGET_PACKAGES, $env:NUGET_HTTP_CACHE_PATH, $env:NUGET_PLUGINS_CACHE_PATH | Out-Null

    if (Test-Path $Transcript) { Remove-Item $Transcript -Force }
    if (Test-Path $Summary) { Remove-Item $Summary -Force }
    if (Test-Path $Zip) { Remove-Item $Zip -Force }
    if (Test-Path $RepoRoot) { Remove-Item $RepoRoot -Recurse -Force }

    Start-Transcript -Path $Transcript -Force | Out-Null
    $success = $false

    try {
        Write-Host "============================================================"
        Write-Host "FALCON FSATS RED-TEAM REMEDIATION EXACT EXECUTABLE VALIDATION"
        Write-Host "Expected commit : $ExpectedCommit"
        Write-Host "Expected SDK    : $ExpectedSdk"
        Write-Host "Validation root : $TestRoot"
        Write-Host "============================================================"

        Write-Host "Step 1/12: Checking required tools..."
        $git = Get-Command git -ErrorAction Stop
        $dotnet = Get-Command dotnet -ErrorAction Stop
        Write-Host "git    : $($git.Source)"
        Write-Host "dotnet : $($dotnet.Source)"

        Write-Host "Step 2/12: Cloning repository without checkout..."
        git -c core.longpaths=true clone --no-checkout "https://github.com/raed82iam/Falcon.git" $RepoRoot
        if ($LASTEXITCODE -ne 0) { throw "git clone --no-checkout failed with exit $LASTEXITCODE" }

        Push-Location $RepoRoot
        try {
            git config core.longpaths true
            if ($LASTEXITCODE -ne 0) { throw "Unable to enable repository-local core.longpaths" }

            git config --global --add safe.directory ($RepoRoot -replace '\\','/')

            $longPaths = (git config --get core.longpaths).Trim()
            if ($LASTEXITCODE -ne 0 -or $longPaths -ne "true") {
                throw "core.longpaths was not enabled before checkout"
            }
            Write-Host "core.longpaths: $longPaths"

            Write-Host "Step 3/12: Checking out exact candidate with long-path support..."
            git -c core.longpaths=true checkout --detach $ExpectedCommit
            if ($LASTEXITCODE -ne 0) { throw "git checkout failed with exit $LASTEXITCODE" }

            $head = (git rev-parse HEAD).Trim()
            if ($LASTEXITCODE -ne 0 -or $head -ne $ExpectedCommit) {
                throw "HEAD mismatch. Expected $ExpectedCommit but got $head"
            }
            Write-Host "HEAD: $head"

            Write-Host "Step 4/12: Verifying ownership boundary from pre-remediation base..."
            $changed = @(git diff --name-only $ComparisonBase $ExpectedCommit)
            if ($LASTEXITCODE -ne 0) { throw "git diff failed" }
            $forbidden = @($changed | Where-Object { -not $_.StartsWith("applications/") })
            Write-Host "Changed paths: $($changed.Count)"
            $changed | ForEach-Object { Write-Host " - $_" }
            if ($forbidden.Count -gt 0) {
                throw "Ownership boundary crossed: $($forbidden -join ', ')"
            }
            Write-Host "APPLICATION OWNERSHIP BOUNDARY: PASS"

            Write-Host "Step 5/12: Verifying clean tracked tree and exact SDK..."
            $status = @(git status --porcelain)
            if ($status.Count -ne 0) { throw "Repository is not clean before validation: $($status -join '; ')" }

            $sdk = (dotnet --version).Trim()
            Write-Host "dotnet SDK: $sdk"
            if ($sdk -ne $ExpectedSdk) {
                throw "SDK mismatch. Required $ExpectedSdk but found $sdk"
            }

            Write-Host "Step 6/12: Restoring inherited Foundation snapshot..."
            dotnet restore "Falcon.Foundation.ControlledProjectFoundation.slnx"
            if ($LASTEXITCODE -ne 0) { throw "Foundation restore failed with exit $LASTEXITCODE" }

            Write-Host "Step 7/12: Building inherited Foundation snapshot Release..."
            dotnet build "Falcon.Foundation.ControlledProjectFoundation.slnx" -c Release --no-restore
            if ($LASTEXITCODE -ne 0) { throw "Foundation build failed with exit $LASTEXITCODE" }

            Write-Host "Step 8/12: Restoring Application solution..."
            $appSolution = "applications/Falcon.Applications.slnx"
            if (-not (Test-Path $appSolution -PathType Leaf)) { throw "Application solution missing: $appSolution" }
            dotnet restore $appSolution
            if ($LASTEXITCODE -ne 0) { throw "Application restore failed with exit $LASTEXITCODE" }

            Write-Host "Step 9/12: Building Application solution Release..."
            dotnet build $appSolution -c Release --no-restore
            if ($LASTEXITCODE -ne 0) { throw "Application build failed with exit $LASTEXITCODE" }

            Write-Host "Step 10/12: Running Application tests..."
            dotnet test $appSolution -c Release --no-build
            if ($LASTEXITCODE -ne 0) { throw "Application tests failed with exit $LASTEXITCODE" }

            $runner = "applications/ci/Run-Application-Verifiers.ps1"
            if (-not (Test-Path $runner -PathType Leaf)) { throw "Verifier runner missing: $runner" }

            Write-Host "Step 11/12: Running governed Application verifiers pass 1..."
            & $runner
            if ($LASTEXITCODE -ne 0) { throw "Application verifier pass 1 failed with exit $LASTEXITCODE" }

            Write-Host "Step 12/12: Running governed Application verifiers pass 2 for deterministic confirmation..."
            & $runner
            if ($LASTEXITCODE -ne 0) { throw "Application verifier pass 2 failed with exit $LASTEXITCODE" }

            $finalHead = (git rev-parse HEAD).Trim()
            $finalStatus = @(git status --porcelain)
            if ($finalHead -ne $ExpectedCommit) { throw "Final HEAD changed: $finalHead" }
            if ($finalStatus.Count -ne 0) { throw "Tracked tree changed during validation: $($finalStatus -join '; ')" }

            @(
                "RESULT=PASS",
                "EXACT_COMMIT=$ExpectedCommit",
                "SDK=$ExpectedSdk",
                "CORE_LONGPATHS=TRUE",
                "FOUNDATION_RESTORE=PASS",
                "FOUNDATION_BUILD_RELEASE=PASS",
                "APPLICATION_RESTORE=PASS",
                "APPLICATION_BUILD_RELEASE=PASS",
                "APPLICATION_TEST=PASS",
                "APPLICATION_VERIFIERS_PASS1=6/6 PASS",
                "APPLICATION_VERIFIERS_PASS2=6/6 PASS",
                "OWNERSHIP_BOUNDARY=PASS",
                "FINAL_HEAD=$finalHead",
                "FINAL_TRACKED_TREE=CLEAN",
                "RUNTIME_AUTHORITY=NOT_GRANTED",
                "AI_RELEASE_OR_REVIVAL=NOT_GRANTED"
            ) | Set-Content -Path $Summary -Encoding UTF8

            $success = $true
            Write-Host "============================================================"
            Write-Host "FSATS REMEDIATION EXECUTABLE VALIDATION: PASS"
            Write-Host "============================================================"
        }
        finally {
            Pop-Location
        }
    }
    catch {
        @(
            "RESULT=FAIL",
            "EXACT_COMMIT=$ExpectedCommit",
            "ERROR=$($_.Exception.Message)",
            "RUNTIME_AUTHORITY=NOT_GRANTED",
            "AI_RELEASE_OR_REVIVAL=NOT_GRANTED"
        ) | Set-Content -Path $Summary -Encoding UTF8
        Write-Error $_
    }
    finally {
        Stop-Transcript | Out-Null
        Compress-Archive -Path $Transcript, $Summary -DestinationPath $Zip -Force
        Write-Host ""
        Write-Host "Evidence ZIP: $Zip"
        Write-Host "Summary     : $Summary"
        if (-not $success) { exit 1 }
    }
}
```

## Expected evidence

If the procedure passes, upload:

`C:\FAV\Validation-Evidence.zip`

The Application workstream must then verify the transcript/summary, record the exact executable evidence in governed documentation, rerun the final FCR-0226 disposition check, and only then consider FCR closure. A PASS still does not grant runtime or AI release authority.
