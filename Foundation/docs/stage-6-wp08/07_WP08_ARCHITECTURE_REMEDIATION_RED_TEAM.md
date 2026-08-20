# Stage 6 WP-08 — Architecture Remediation Red-Team

**Stage/WP:** Stage 6 WP-08 — Per-Application Resource State and Load-Shedding Signal Boundary  
**Review Type:** Post-failed-executable architecture remediation Red-Team  
**Date:** 2026-08-10

## Trigger

Exact executable validation of HEAD `9eef49d828b3bc7fa95b76e127df73ba23b9224d` produced:

- Restore: PASS
- Release Build: PASS — 0 warnings / 0 errors
- Foundation Architecture: FAIL
- Security and Stage 6 WP verifiers: NOT REACHED

The architecture harness writes detailed failures to stderr. The PowerShell wrapper stopped on the first stderr line, but direct inspection of the accepted architecture rule identified the concrete WP-08 defect.

## Finding

### RT-R01 — HIGH — Work-package identity leaked into permanent production public type names

WP-08 introduced the following public types under permanent `Foundation.State`:

- `Wp07AcceptedCapacityBasis`
- `Wp06DecisionProjectionReference`

The accepted architecture harness tokenizes permanent production public type names and rejects `Falcon`, `Stage`, and `WP` identity tokens. Therefore the WP-08 surface violated the permanent-production identity rule.

This is a WP-08 implementation defect. It does not reopen Stage 6 WP-01 through WP-07 and is not an architecture-test defect.

## Remediation

The permanent production surface was generalized without changing semantics or authority:

- `Wp07AcceptedCapacityBasis` -> `AcceptedResourceCapacityTransitionBasis`
- `Wp06DecisionProjectionReference` -> `AdditionalResourceDecisionProjectionReference`
- `AcceptedWp07BasisIdentitySha256` -> `AcceptedCapacityBasisIdentitySha256`
- related canonical identity labels and diagnostic wording were generalized
- active WP-08 verifier references were updated to the generic production names

No predecessor production file was modified. `Foundation.Contracts` was not modified. WP-09 remains unauthorized.

## Static verification

Comparison from failed executable target `9eef49d828b3bc7fa95b76e127df73ba23b9224d` to the remediated implementation before this report shows changes limited to:

1. `src/Foundation.State/ApplicationResourceStateProjectionGovernance.cs`
2. `verification/Falcon.Stage6.WP08.Verifier/ProgramV3.cs`

Search confirms the obsolete permanent production type names are absent from the current branch.

## Red-Team result

- Critical: **0 open**
- High: **0 open**
- Medium: **0 open**
- Result: **PASS / EXECUTABLE REVALIDATION REQUIRED**

## Preserved boundaries

- Stage 6 WP-01 through WP-07 remain `ACCEPTED_AND_CLOSED`.
- `RESOURCE_STATE_PROJECTION != RESOURCE_AUTHORITY`.
- `LOAD_SHEDDING_SIGNAL != LOAD_SHEDDING_EXECUTOR`.
- `PROJECTION_SCOPE != RUNTIME_AUTHENTICATION_OR_ADMISSION`.
- Pressure/enforcement observation does not mint authority.
- Binding compliance remains tied to exact accepted resource-capacity transition evidence.
- Additional-resource decision context remains distinct from applied capacity.
- Aggregate state remains exact constituent projections, never an opaque pool.
- WP-09 implementation authority remains NOT GRANTED.

## Disposition

`WP08_ARCHITECTURE_REMEDIATION_STATIC_RED_TEAM = PASS_0C_0H_0M`

`WP08_EXECUTABLE_REVALIDATION = REQUIRED`

No technical acceptance, Application compatibility, Owner closure, or successor authority is claimed.