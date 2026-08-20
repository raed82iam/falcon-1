# Stage 6 WP-05 — Implementation Entry Existing-Capability Reconciliation

Status: IMPLEMENTATION AUTHORIZED / RECONCILIATION COMPLETE / PRODUCTION CHANGE NOT YET COMMITTED
Date: 2026-08-09
Branch: `foundation-development`
Owner implementation authorization: `02cbd642c35774fe5148464efee3b8804505f762`
Relevant FCR: FCR-0010

## 1. Purpose

This record performs the mandatory existing-capability reconciliation before Stage 6 WP-05 production implementation.

The objective is to extend the accepted singular Foundation resource-governance state owner without duplicating WP-01 through WP-04 ownership or inventing missing authority.

## 2. Accepted Existing Production Surface

Fresh inspection confirms the accepted resource-governance production surface is already consolidated under:

`Foundation.State.ResourceGovernance`

Relevant accepted files include:

- `src/Foundation.State/ResourceTruth.cs` — WP-02 Foundation total-resource truth, protection floors, recovery reserves and allocatable capacity;
- `src/Foundation.State/ResourceAllocation.cs` — WP-03 Application allocation/quota/ceiling/isolation truth;
- `src/Foundation.State/ResourcePriorityGovernance.cs` — WP-04 Application-priority and Foundation technical-criticality policy/truth.

WP-05 SHALL extend this same state owner.

`SECOND_RESOURCE_STATE_OWNER = PROHIBITED`

`NEW_GLOBAL_RESOURCE_CONTROLLER_PROJECT = PROHIBITED`

The Owner-selected concrete name for the resource-governance capability is:

`Foundation Resource Governance`

This name refers to the same singular governed capability and does not create a second authority.

## 3. Reuse Classification

### ALREADY_SATISFIED_BY_ACCEPTED_BASELINE

- canonical resource identity/evidence primitives — WP-01;
- total capacity / Foundation floor / recovery reserve / allocatable capacity — WP-02;
- exact Application allocation/quota/ceiling and isolation truth — WP-03;
- Application priority and separate Foundation technical-criticality truth — WP-04;
- Stage 5 delivery-side consumption of governed pressure evidence where already accepted.

These SHALL be consumed read-only and SHALL NOT be redesigned.

### GENUINELY_MISSING / WP-05 AUTHORIZED

- singular Foundation pressure-state truth;
- exact global/resource-class versus Application-bound pressure scope;
- deterministic transition-stability and anti-flapping state;
- monotonic pressure ordering / anti-rollback supersession;
- observed enforcement-state truth without resource mutation;
- attributable preemption/reclamation eligibility truth without execution;
- WP-05-specific verification surface.

## 4. Material Gap Found Before Coding

WP-03 `ApplicationResourceAllocation` currently binds:

- grant identity;
- Application identity;
- resource class;
- allocation;
- quota;
- ceiling;
- lifetime;
- evidence.

It does **not** carry an exact allocation/grant-level reclaimability classification.

WP-05 planning requires preemption/reclamation **eligibility truth**, but eligibility cannot safely be inferred from Application priority alone and cannot be caller-invented.

Therefore:

`PRIORITY != RECLAIMABILITY`

`LOWER_PRIORITY != AUTOMATICALLY_RECLAIMABLE`

`PREEMPTION_ELIGIBILITY_REQUIRES_ATTRIBUTABLE_RECLAIMABILITY_EVIDENCE = TRUE`

## 5. Governed Resolution Within WP-05 Boundary

WP-05 implementation SHALL add a WP-05-owned, non-mutating eligibility binding that:

- references an exact existing WP-03 `ResourceGrantId`;
- references the exact `ApplicationPrincipalId` and `ResourceClassId` already bound by that grant;
- carries the canonical WP-01 `ResourceReclaimability` value;
- carries exact `ResourceEvidenceReference` and current resource epoch;
- is valid only while the referenced WP-03 allocation/grant is current;
- cannot create, change or widen the allocation, quota or ceiling;
- cannot authorize or execute preemption;
- becomes invalid on grant removal, expiry, epoch replacement or evidence mismatch.

This creates WP-05 eligibility truth without modifying accepted WP-03 allocation bytes or semantics.

`WP03_PRODUCTION_MUTATION_REQUIRED = NO`

`WP05_ELIGIBILITY_BINDING_CREATES_PREEMPTION_AUTHORITY = NO`

## 6. Pressure Observation Boundary

WP-05 needs attributable current technical-use observations to derive pressure. Those observations SHALL be treated as evidence input, not authority.

The implementation shall distinguish:

- Foundation-global/resource-class observations derived against WP-02 total/protected/allocatable truth;
- exact Application-bound observations derived only against that Application's WP-03 allocation/ceiling context.

An Application-bound observation SHALL NOT include or expose another Application's allocation detail.

`GLOBAL_PRESSURE != APPLICATION_PRESSURE`

## 7. Transition Stability

WP-05 SHALL implement a versioned transition policy rather than hard-code one universal pressure threshold across resource classes.

Each resource-class transition policy must be attributable and deterministic and may define class-appropriate thresholds/hysteresis under the authorized implementation design.

Deterioration must remain promptly observable; recovery must not flap on noisy measurements.

## 8. Enforcement Truth Boundary

WP-05 may observe/report enforcement states produced by an authorized resource owner, but SHALL NOT perform resource mutation.

`ENFORCEMENT_OBSERVED_BY_WP05 != ENFORCEMENT_EXECUTED_BY_WP05`

`ENFORCEMENT_EXECUTED_BY_WP05 = PROHIBITED`

## 9. FCR-0010 / TARC Boundary

Current Application ACK remains COMPLETE.

For Falcon Self-Aware Trading Application:

- Foundation truth terminates at the admitted Application boundary;
- TARC remains the sole Trading Application operational resource controller / Foundation resource-request communicator;
- internal Trading roles and Guardian signals may provide evidence but cannot become independent Foundation resource principals;
- no direct Guardian/break-glass resource requester is created by WP-05.

## 10. Production Change Strategy

Authorized production implementation should proceed by:

1. adding WP-05 pressure/eligibility/enforcement truth types inside `src/Foundation.State` under namespace `Foundation.State.ResourceGovernance`;
2. preserving WP-02/WP-03/WP-04 production files unless a proven compile/integration defect requires a minimal compatible amendment;
3. adding a dedicated Stage 6 WP-05 verifier project;
4. adding the verifier to the controlled solution surface;
5. running predecessor regressions plus WP-05 deterministic rerun before technical completion is claimed.

## 11. Entry Result

`WP05_EXISTING_CAPABILITY_RECONCILIATION = COMPLETE`

`ACCEPTED_PREDECESSOR_REUSE = REQUIRED`

`DUPLICATE_RESOURCE_OWNER = NO`

`WP05_PRODUCTION_SCOPE = READY_TO_IMPLEMENT`

`WP05_IMPLEMENTATION_AUTHORITY = GRANTED`

`WP05_OWNER_CLOSURE = NOT_GRANTED`

`WP06_AND_LATER_IMPLEMENTATION = NOT_AUTHORIZED`
