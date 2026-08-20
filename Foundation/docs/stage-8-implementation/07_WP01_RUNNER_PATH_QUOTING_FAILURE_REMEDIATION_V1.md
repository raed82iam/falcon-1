# Stage 8 WP-01 Runner Path-Quoting Failure Remediation v1

Status: DOCUMENTED_RUNNER_ONLY_FAILURE / NO_PRODUCTION_CODE_CHANGE
Date: 2026-08-14
Branch: foundation-development

## Exact failed candidate

`3c1e3fa6231ed0ff81ace5f30b4c4373c7d217d9`

## Observed result

The exact candidate cloned and checked out cleanly. Controlled restore, Release build, Architecture validation, Security validation, and Stage 7 Cross-Stage predecessor regression all passed.

The first failure occurred before the Stage 8 WP-01 verifier DLL actually executed. The PowerShell helper used `Start-Process -ArgumentList @($Wp01Dll)` while the DLL path was under `C:\falcon\Foundation test\...`. The embedded space caused the argument to be split so `dotnet` attempted to resolve a truncated command beginning with `C:\falcon\Foundation`.

Observed runtime diagnostic included:

`dotnet-C:\falcon\Foundation does not exist`

and

`Could not execute because the specified command or file was not found.`

## Disposition

This is a local test-runner invocation defect, not a Guardian runtime defect and not a verifier logic defect.

No change is required to:

- `src/Foundation.Guardian/**`
- `verification/Falcon.Stage8.WP01.Verifier/**`
- architecture or security behavior
- Stage 8 WP-01 semantics

The replacement runner shall avoid `Start-Process -ArgumentList` for the DLL invocation and shall execute the already-built verifier through PowerShell's call operator:

`& dotnet $Wp01Dll`

with temporary relaxation of `$ErrorActionPreference` only around native-process capture so stderr remains observable while preserving the exact path as one argument.

## Governance

- FCR-0076 remains `Waiting On: FOUNDATION` under Stage 8.
- FCR-0082 remains `Waiting On: FOUNDATION` under Stage 8.
- no Owner blocker exists for WP-01 continuity.
- no WP-01 technical PASS is claimed yet.
- no Stage 8 closure is created.
- no Stage 9 or Stage 13 authority is created.

`WP01_FAILURE_CLASS = RUNNER_PATH_QUOTING_ONLY`
`WP01_PRODUCTION_CODE_CHANGE_REQUIRED = NO`
`WP01_VERIFIER_CODE_CHANGE_REQUIRED = NO`
`NEXT_STEP = EXACT_RETEST_WITH_CORRECTED_RUNNER`
