# FSATS Remediation — Exact Local Revalidation After Guardian Adversarial Test Repair

Date: 2026-08-17
Target branch: `application-development`
Exact source/test candidate: `281f63773849477139235269d3ac4fc6575b04ce`
Comparison base: `76c8f5aadb83193bc54405dce6d3c574c6412d59`
Required SDK: `10.0.302`

The prior Owner-machine run successfully passed long-path checkout, exact candidate identity, Application ownership boundary, SDK pin, Foundation restore/build, and Application restore. Application Release build then exposed two compile errors in `BroadRedTeamAdversarialChecks.cs` because the verifier still referenced the removed raw `IProtectionCommandPort` compatibility surface.

The repair preserves the production hardening: the old raw port is not restored. The adversarial checks now exercise wrong-target reconciliation and route-exception reconciliation through `IGovernedProtectionCommandRoutePort` + `GovernedProtectionCommandDispatcher`.

Use the already cloned short-path repository at `C:\FAV\R`. The procedure below fetches and checks out the exact repaired candidate, cleans generated outputs, verifies ownership/SDK, and reruns the full Foundation/Application build-test-verifier chain.

```powershell
& {
    $ErrorActionPreference = "Stop"
    Set-StrictMode -Version Latest

    $TestRoot = "C:\FAV"
    $RepoRoot = Join-Path $TestRoot "R"
    $Transcript = Join-Path $TestRoot "Validation-Transcript-R2.txt"
    $Summary = Join-Path $TestRoot "Validation-Summary-R2.txt"
    $Zip = Join-Path $TestRoot "Validation-Evidence-R2.zip"

    $ExpectedCommit = "281f63773849477139235269d3ac4fc6575b04ce"
    $ComparisonBase = "76c8f5aadb83193bc54405dce6d3c574c6412d59"
    $ExpectedSdk = "10.0.302"

    $DotNetHome = Join-Path $TestRoot "D"
    $NuGetRoot = Join-Path $TestRoot "N"
    $TempRoot = Join-Path $TestRoot "T"

    if (-not (Test-Path $RepoRoot -PathType Container)) { throw "Expected existing validation repository not found: $RepoRoot" }

    $env:DOTNET_CLI_HOME = $DotNetHome
    $env:NUGET_PACKAGES = Join-Path $NuGetRoot "P"
    $env:NUGET_HTTP_CACHE_PATH = Join-Path $NuGetRoot "H"
    $env:NUGET_PLUGINS_CACHE_PATH = Join-Path $NuGetRoot "C"
    $env:TEMP = $TempRoot
    $env:TMP = $TempRoot
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

    New-Item -ItemType Directory -Force -Path $TestRoot, $DotNetHome, $NuGetRoot, $TempRoot, $env:NUGET_PACKAGES, $env:NUGET_HTTP_CACHE_PATH, $env:NUGET_PLUGINS_CACHE_PATH | Out-Null

    if (Test-Path $Transcript) { Remove-Item $Transcript -Force }
    if (Test-Path $Summary) { Remove-Item $Summary -Force }
    if (Test-Path $Zip) { Remove-Item $Zip -Force }

    Start-Transcript -Path $Transcript -Force | Out-Null
    $success = $false

    try {
        Push-Location $RepoRoot
        try {
            git config core.longpaths true
            git config --global --add safe.directory ($RepoRoot -replace '\\','/')

            Write-Host "Step 1/11: Fetching exact repaired candidate..."
            git fetch origin $ExpectedCommit
            if ($LASTEXITCODE -ne 0) { throw "git fetch failed with exit $LASTEXITCODE" }

            Write-Host "Step 2/11: Cleaning generated outputs and checking out exact candidate..."
            git reset --hard
            if ($LASTEXITCODE -ne 0) { throw "git reset failed" }
            git clean -fdx
            if ($LASTEXITCODE -ne 0) { throw "git clean failed" }
            git -c core.longpaths=true checkout --detach $ExpectedCommit
            if ($LASTEXITCODE -ne 0) { throw "git checkout failed with exit $LASTEXITCODE" }

            $head = (git rev-parse HEAD).Trim()
            if ($head -ne $ExpectedCommit) { throw "HEAD mismatch. Expected $ExpectedCommit but got $head" }
            Write-Host "HEAD: $head"

            Write-Host "Step 3/11: Verifying Application ownership boundary and clean tree..."
            $changed = @(git diff --name-only $ComparisonBase $ExpectedCommit)
            if ($LASTEXITCODE -ne 0) { throw "git diff failed" }
            $forbidden = @($changed | Where-Object { -not $_.StartsWith("applications/") })
            if ($forbidden.Count -gt 0) { throw "Ownership boundary crossed: $($forbidden -join ', ')" }
            $status = @(git status --porcelain)
            if ($status.Count -ne 0) { throw "Repository not clean before validation: $($status -join '; ')" }
            Write-Host "APPLICATION OWNERSHIP BOUNDARY: PASS"

            Write-Host "Step 4/11: Verifying exact SDK..."
            $sdk = (dotnet --version).Trim()
            Write-Host "dotnet SDK: $sdk"
            if ($sdk -ne $ExpectedSdk) { throw "SDK mismatch. Required $ExpectedSdk but found $sdk" }

            Write-Host "Step 5/11: Restoring inherited Foundation snapshot..."
            dotnet restore "Falcon.Foundation.ControlledProjectFoundation.slnx"
            if ($LASTEXITCODE -ne 0) { throw "Foundation restore failed with exit $LASTEXITCODE" }

            Write-Host "Step 6/11: Building inherited Foundation snapshot Release..."
            dotnet build "Falcon.Foundation.ControlledProjectFoundation.slnx" -c Release --no-restore
            if ($LASTEXITCODE -ne 0) { throw "Foundation build failed with exit $LASTEXITCODE" }

            $appSolution = "applications/Falcon.Applications.slnx"
            Write-Host "Step 7/11: Restoring Application solution..."
            dotnet restore $appSolution
            if ($LASTEXITCODE -ne 0) { throw "Application restore failed with exit $LASTEXITCODE" }

            Write-Host "Step 8/11: Building Application solution Release..."
            dotnet build $appSolution -c Release --no-restore
            if ($LASTEXITCODE -ne 0) { throw "Application build failed with exit $LASTEXITCODE" }

            Write-Host "Step 9/11: Running Application tests..."
            dotnet test $appSolution -c Release --no-build
            if ($LASTEXITCODE -ne 0) { throw "Application tests failed with exit $LASTEXITCODE" }

            $runner = "applications/ci/Run-Application-Verifiers.ps1"
            Write-Host "Step 10/11: Running governed Application verifiers pass 1..."
            & $runner
            if ($LASTEXITCODE -ne 0) { throw "Application verifier pass 1 failed with exit $LASTEXITCODE" }

            Write-Host "Step 11/11: Running governed Application verifiers pass 2..."
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
            Write-Host "FSATS REMEDIATION EXECUTABLE REVALIDATION: PASS"
            Write-Host "============================================================"
        }
        finally { Pop-Location }
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
        Write-Host "Evidence ZIP: $Zip"
        Write-Host "Summary     : $Summary"
        if (-not $success) { exit 1 }
    }
}
```

Upload `C:\FAV\Validation-Evidence-R2.zip` after the run, whether PASS or FAIL.

A successful technical revalidation still grants no runtime, provider, broker, Paper/Shadow/Tiny-Live/Live, deployment, or AI release/revival authority.
