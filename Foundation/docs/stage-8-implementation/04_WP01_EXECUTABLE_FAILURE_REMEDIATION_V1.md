# Stage 8 WP-01 Executable Failure Remediation v1

## Failed exact candidate

`baf78473bff1dd4c6579f3423c31448f9c49eb9a`

## User-side executable evidence

The exact candidate checked out cleanly and completed controlled restore, Release build, Architecture validation, Security validation, and the Stage 7 cross-stage predecessor regression successfully. The first real failure occurred when executing the Stage 8 WP-01 verifier.

Observed first failure:

`STAGE8_WP01_VERIFIER = FAIL`

The PowerShell runner used `$ErrorActionPreference = Stop` while redirecting native stderr into the capture pipeline, so the verifier's explanatory second stderr line was not preserved in the transcript.

## Root cause

Static source review identified a verifier defect in `VerifyApplicationNeutrality`.

The verifier required `Foundation.Contracts` to appear in `Foundation.Guardian.dll` through `Assembly.GetReferencedAssemblies()`.

The project file correctly declares `Foundation.Contracts` as the sole project reference, and the Architecture gate validates that exact project-reference boundary. WP-01 production source does not yet consume a concrete type from `Foundation.Contracts`, so the C# compiler may legitimately omit an unused AssemblyRef from emitted assembly metadata.

Therefore the runtime-reflection assertion was stronger than the governed project-reference requirement and could reject a valid build.

## Remediation

Only the WP-01 verifier was changed.

- Removed the incorrect positive runtime AssemblyRef requirement for `Foundation.Contracts`.
- Preserved runtime neutrality checks that reject Application, Trading, Web, or Recovery assembly dependencies.
- Preserved Architecture validation as the authoritative exact check that `Foundation.Guardian.csproj` references only `Foundation.Contracts`.
- No Guardian production behavior was changed.
- No Authority, Lifecycle, Recovery, Application, Web, or reference-owned source was changed.

## Boundary

This remediation does not weaken the production dependency boundary. It aligns two independent checks with their proper layers:

- project dependency ownership and allowed edge: Architecture gate;
- emitted runtime dependency neutrality: WP-01 verifier.

No Stage 8 technical checkpoint is claimed until replacement exact-candidate executable validation passes.
