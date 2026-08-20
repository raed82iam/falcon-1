# Stage 7 WP-08 — Authority, Lifecycle and Protective-Consumer Boundary

Status: IMPLEMENTATION DESIGN / AUTHORIZED UNDER STAGE7 v0.3  
Date: 2026-08-14

## Scope

WP-08 provides a bounded Stage-7-owned consumption-evidence surface over canonical CON-006 health/fitness assessments for governed consumers.

It does not implement Authority Engine policy, Lifecycle transition policy, Guardian commands, Platform Safe State, recovery execution, or independent release.

## Governing semantics

- AUT-001 may consume fitness as a condition/input but fitness never grants permission.
- SYS-002 Lifecycle consumes only under Lifecycle-owned rules and AUT-001 authorization.
- missing, expired, insufficient, invalid, contradictory, unknown or otherwise non-current required awareness/fitness cannot support positive authority inference.
- material fitness reduction can be exposed as restriction/denial input evidence to governed consumers.
- `RECOVERY_REQUIRED` creates a recovery gate for the affected capability but does not declare recovery complete.
- source/evidence reappearance does not restore authority.
- after a material loss/recovery condition, a fresh independent reassessment is required before fitness may again support positive authority conditions; actual authority restoration still requires a new attributable AUT-001 decision where applicable.

## Dependency shape

Implementation remains inside existing `Foundation.HealthFitness`.

No new production project is introduced.
No ProjectReference is added from `Foundation.HealthFitness` to Authority, Lifecycle, Guardian, State, EventSystem or Recovery.

Consumers are represented only by canonical consumer-role identifiers/enums and immutable evidence objects.

## Output semantics

`GovernedFitnessConsumptionEvidence` exposes:

- exact assessment identity and scope;
- consumer role;
- freshness/currentness result;
- whether the assessment may support a positive authority condition;
- whether restriction input is required;
- whether positive authority inference must be blocked;
- whether recovery gating is required;
- whether independent reassessment is required;
- whether a new authority decision is required before restoration;
- immutable deterministic evidence identity;
- reason.

These are inputs/evidence only.

## Explicit non-authority invariant

`FITNESS_CONSUMPTION_EVIDENCE != AUTHORITY_DECISION`

`FITNESS_CONSUMPTION_EVIDENCE != LIFECYCLE_TRANSITION`

`FITNESS_CONSUMPTION_EVIDENCE != GUARDIAN_COMMAND`

`SOURCE_RECOVERY != AUTHORITY_RESTORATION`
