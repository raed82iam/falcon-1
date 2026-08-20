# 07 - Stage 1 Environment, Toolchain, and Resource Plan

## Exact toolchain identity

| Tool ID | Exact version | Lifecycle | Stage 1 status | Pre-execution evidence | Source | Digest | Network requirement | Restriction |
|---|---|---|---|---|---|---|---|---|
| DOTNET-SDK | 10.0.302 | APPROVED_CATALOG_ONLY | ACTIVE_BASELINE_PREEXECUTION_REVALIDATION_REQUIRED | exact admitted environment and build evidence | `BLD-001` / `ENV-001` | `PREEXECUTION_REVALIDATION_REQUIRED` | No for empty-build proof path | SDK roll-forward governed; no preview features |
| DOTNET-RUNTIME | 10.0.10 | APPROVED_CATALOG_ONLY | ACTIVE_BASELINE_PREEXECUTION_REVALIDATION_REQUIRED | exact admitted environment and build evidence | `BLD-001` / `ENV-001` | `PREEXECUTION_REVALIDATION_REQUIRED` | No for empty-build proof path | runtime must match baseline |
| CSHARP-LANG | 14.0 | SDK-BOUND | SDK_BOUND_PREEXECUTION_REVALIDATION_REQUIRED | SDK 10.0.302 admission evidence | `BLD-001` / `ENV-001` | `PREEXECUTION_REVALIDATION_REQUIRED` | No | no preview features |
| TARGET-FRAMEWORK | net10.0 | SDK-BOUND | SDK_BOUND_PREEXECUTION_REVALIDATION_REQUIRED | solution and project identity | `BLD-001` / `ENV-001` | `PREEXECUTION_REVALIDATION_REQUIRED` | No | target is fixed for Stage 1 |
| MSBUILD | 10.0.302 bound payload | SDK-BOUND | SDK_BOUND_PREEXECUTION_REVALIDATION_REQUIRED | SDK payload identity | `BLD-001` / `ENV-001` | `PREEXECUTION_REVALIDATION_REQUIRED` | No | exact compiler/MSBuild binding |
| NUGET | SDK-bound payload of .NET SDK 10.0.302 | SDK-BOUND | SDK_BOUND_PREEXECUTION_REVALIDATION_REQUIRED | exact SDK payload identity and governed locked-restore behavior must be confirmed before execution | `BLD-001` / `ENV-001` | `PREEXECUTION_REVALIDATION_REQUIRED` | No for the offline locked-restore path | lock-file governed; no uncontrolled package source or network restore |
| Microsoft.Testing.Platform | 2.3.2 | APPROVED_CATALOG_ONLY | DEFERRED_TO_LATER_STAGE | no separate active admission evidence | `BLD-001` | `DEFERRED_TO_LATER_STAGE` | No | behavioral testing deferred |
| MSTest | 4.3.2 | APPROVED_CATALOG_ONLY | DEFERRED_TO_LATER_STAGE | no separate active admission evidence | `BLD-001` | `DEFERRED_TO_LATER_STAGE` | No | behavioral testing deferred |
| Microsoft.NET.Test.Sdk | 18.8.1 | APPROVED_WITH_RESTRICTION_NOT_ADMITTED_FOR_STAGE_1 | DEFERRED_TO_LATER_STAGE | no Stage 1 admission evidence | `BLD-001` | `DEFERRED_TO_LATER_STAGE` | No | behavioral testing deferred |
| Microsoft SBOM Tool | 4.1.5 | APPROVED_CATALOG_ONLY | DEFERRED_TO_LATER_STAGE | no exact admission evidence | `BLD-001` | `DEFERRED_TO_LATER_STAGE` | No | SBOM generation deferred |
| SPDX | 3.0.1 | APPROVED_CATALOG_ONLY | DEFERRED_TO_LATER_STAGE | no exact admission evidence | `BLD-001` | `DEFERRED_TO_LATER_STAGE` | No | schema identity for artifact metadata |
| PostgreSQL | 18.4 | NOT_APPLICABLE_TO_STAGE_1_EMPTY_BUILD | No | n/a | `BLD-001` / `FRS-001` | n/a | No | not needed for the Stage 1 empty build |

## Environment identity

The exact active Windows Foundation environment profile is the approved
`ENV-001` Windows Foundation build-verification profile established by the
active canonical baseline.

The operating-system identity and update boundary SHALL match that approved
profile and the exact activation evidence.

## Exact environment and baseline facts

| Field | Exact value |
|---|---|
| Active Foundation platform | Windows |
| Exact active Foundation environment profile | `ENV-001 v1.1` |
| Exact active build baseline | `BLD-001 v1.1` |
| Exact active pipeline definition | `PIPE-001 v1.1` |
| Exact active gate profile | `PIPE-001` governed bootstrap and governed modes per active baseline |
| Exact package sources | approved sources recorded in the active build baseline and activation evidence |
| Exact offline bundle identities | content-identified bundles recorded in the active build baseline and activation evidence |
| Exact offline bundle digests | recorded in the active build baseline and activation evidence |
| Network state | offline for the empty-build proof path |
| Telemetry restrictions | minimal and evidence-only; no uncontrolled telemetry |
| SDK roll-forward policy | governed by the active baseline; no silent drift |
| PostgreSQL 18.4 | `NOT_APPLICABLE_TO_STAGE_1_EMPTY_BUILD` unless a later exact requirement proves otherwise |

## Resource model

| Resource | Planned rule |
|---|---|
| CPU | one controlled local build or verification task at a time |
| Memory | bounded by the active Foundation environment profile |
| Disk | workspace-local only |
| Network state | offline for the empty-build proof path |
| Telemetry | minimized and restricted to approved evidence collection |
| Resource ceilings | exact values recorded in the approved environment profile |

## Toolchain and environment constraints

- The exact SDK, runtime, compiler, MSBuild, and NuGet payload identities are
  required, and their pre-execution revalidation is mandatory before any
  execution decision.
- The effective Build Baseline is scoped to a deterministic BCL-only local
  Foundation Release build and prohibits unapproved external packages.
- Controlled build and verification command execution is required only after
  authority effectiveness is established by the separately governed Stage 1
  execution path.
- Behavioral test execution is deferred to the first behavior implementation
  stage and does not create a new Stage 1 admission blocker.
- No tool is assumed executable merely because it appears in BLD-001.
- Any tool without an active admitted implementation is either deferred to a
  later stage or resolved through pre-execution revalidation rather than a new
  admission blocker.
- The Stage 1 proposal shall use exact package sources and exact offline bundle
  identities and digests only where such evidence is actually present in the
  active baseline.
- Lock files are required and shall be deterministic.
- SDK roll-forward is governed by the active baseline and shall not drift
  silently.
- Network access is not required for the empty-build proof path.
- The exact active Build Baseline, exact Pipeline Definition, and exact Gate
  Profile are those admitted by the Stage 0C closure evidence set.
