# Stage 6 WP-05 — Resource Pressure, Preemption Eligibility and Enforcement-State Truth

**Document:** Planning v0.4 — FSARM Reconciled  
**Status:** OWNER-REVIEW CANDIDATE / NOT OWNER ACCEPTED / NO IMPLEMENTATION AUTHORITY  
**Branch:** `foundation-development`  
**Governing Master Plan:** IMP-001 v1.3  
**Relevant FCRs:** FCR-0010, FCR-0031, FCR-0007  
**Supersedes for prospective planning review:** v0.3 Application-ACK-reconciled planning package where future TARC-only semantics conflict with FCR-0031

## 1. Purpose

WP-05 defines the singular Foundation-owned technical truth describing resource pressure, preemption eligibility, and resource enforcement state by consuming accepted Stage 6 WP-01 through WP-04 resource identities, Foundation total-resource truth, Application allocation/ceiling truth, and governed Application-priority / Foundation technical-criticality truth.

WP-05 is a truth derivation and observation boundary. It SHALL NOT mutate resource grants, allocations, ceilings, reclaimable quantities, Foundation resource ownership, Application business state, lifecycle state, Guardian state, or Falcon-wide protective state.

`WP05_ROLE = PRESSURE_AND_ENFORCEMENT_TRUTH_DERIVATION`

`WP05_RESOURCE_MUTATION_AUTHORITY = NONE`

## 2. Accepted Preconditions Preserved

WP-05 SHALL consume and SHALL NOT redefine:

- WP-01 canonical resource-governance identities/evidence primitives;
- WP-02 total-resource truth, Foundation protection floors, recovery reserves and allocatable-capacity truth;
- WP-03 attributable Application allocation/quota/ceiling/isolation state;
- WP-04 governed Application-priority truth and separate Foundation technical-criticality truth;
- accepted Stage 5 delivery-side pressure-consumption behavior where it already consumes Foundation-governed pressure truth;
- Foundation validity with zero Applications.

Closed predecessors remain closed. FCR-0031 does not reopen WP-01 through WP-04.

## 3. FCR Reconciliation

FCR-0031 supersedes the prior future-facing assumption that Trading-only TARC is the controlling FSATS-wide resource coordination/request identity.

Current reconciled planning position:

- FSARM is the requested FSATS-wide delegated aggregate resource coordinator;
- FSARM does not replace constituent Application identities;
- FSARM does not own Foundation resource truth;
- Foundation retains final resource authority;
- constituent Application allocation/ceiling/pressure/accounting truth remains exact and separately attributable;
- aggregate coordination may be consumed later only through separately authorized WP-06/WP-07/WP-08 behavior;
- WP-05 remains generic and shall not hard-bind to TARC or implement FSARM coordination mechanics.

`FCR0031_CONTROLLING_FUTURE_FSATS_COORDINATOR = FSARM`

`WP05_CONSUMER_MODEL = GENERIC_ATTRIBUTABLE_CONSUMER_OR_COORDINATOR`

## 4. WP-05 Owned Truth

WP-05 owns only Foundation technical resource pressure/preemption-eligibility/enforcement-state truth.

The model SHALL support attributable truth for at least:

1. exact pressure-truth scope identity;
2. resource class and governed resource scope;
3. Foundation resource epoch/version;
4. monotonic pressure-state order/sequence or equivalent deterministic supersession identity;
5. total-capacity and allocatable-capacity references where applicable;
6. exact Application allocation/ceiling reference only for Application-bound pressure truth;
7. current pressure state;
8. current observed enforcement state;
9. preemption/reclamation eligibility state without execution;
10. observation time, effective time, freshness, expiry and supersession;
11. authority/evidence identities;
12. causation/correlation identities;
13. deterministic reason/category;
14. transition-policy identity/version;
15. deterministic snapshot/state identity.

## 5. Scope Separation

WP-05 SHALL distinguish Foundation-global/resource-class pressure truth from exact Application-bound pressure/enforcement truth.

Global pressure SHALL NOT expose another Application's private allocation details. Application-bound truth SHALL bind one exact admitted Application and its own effective allocation/ceiling context only.

An authorized aggregate coordinator consumer may later receive a governed aggregate view only if a later WP explicitly authorizes that projection. Such a view must remain derivable from constituent Application truth and must not erase constituent identities.

`GLOBAL_PRESSURE != APPLICATION_PRESSURE`

`APPLICATION_A_PRESSURE != APPLICATION_B_PRESSURE`

`AGGREGATE_VIEW != OPAQUE_POOL`

## 6. Pressure-State Model

Semantics SHALL distinguish at least:

- NORMAL;
- CONSTRAINED;
- DEGRADED;
- CRITICAL;
- UNKNOWN/UNAVAILABLE.

These states are evidence/truth only.

`PRESSURE_STATE != AUTHORITY`

`PRESSURE_STATE != PROTECTIVE_AUTHORITY`

`PRESSURE_STATE != GUARDIAN_COMMAND`

`PRESSURE_STATE != SAFE_STATE_ENTRY`

## 7. Deterministic Transition Stability

WP-05 SHALL prevent uncontrolled state flapping while preserving prompt recognition of genuine deterioration.

Implementation design SHALL use governed, versioned, deterministic transition stability such as hysteresis, debounce, qualification windows, asymmetric promotion/recovery or equivalent behavior.

Numeric thresholds/timers SHALL be evidence-bound and resource-class appropriate.

## 8. Enforcement-State Truth Is Observation, Not Mutation

WP-05 may report observed/effective resource enforcement states produced by an authorized owner, but SHALL NOT perform the mutation itself.

WP-05 SHALL NOT:

- change allocation or ceiling;
- grant/cap/deny a request;
- reclaim/redistribute/rebalance capacity;
- restore a prior allocation;
- mutate WP-03 allocation truth;
- execute WP-07 behavior;
- modify FSARM internal effective distribution.

`ENFORCEMENT_OBSERVED_BY_WP05 != ENFORCEMENT_EXECUTED_BY_WP05`

## 9. Preemption Eligibility Versus Execution

WP-05 may derive attributable eligibility for an exact reclaimable scope.

`PREEMPTION_ELIGIBLE != PREEMPTION_AUTHORIZED`

`PREEMPTION_AUTHORIZED != PREEMPTED`

Actual reclaim/reduction/redistribution/rebalance/restoration belongs to separately authorized execution owners, principally WP-07 for Stage 6 resource execution semantics.

## 10. Foundation / Application / FSARM Boundary

Foundation owns platform technical resource truth and final Foundation technical resource authority.

Applications own their business semantics and internal business reactions.

FSARM, when separately admitted and authorized in later Stage 6 work, is a bounded delegated aggregate resource coordinator over an exact set of admitted FSATS Applications. It may consume attributable resource truth but SHALL NOT become a Foundation truth owner or mint Foundation priority/criticality.

WP-05 SHALL NOT prescribe FSARM redistribution policy, Trading degradation hierarchy, Risk, strategy, execution, provider behavior, simulation semantics or Guardian command behavior.

## 11. Priority and Criticality Consumption

WP-05 consumes accepted WP-04 truth.

- No caller may self-promote Application priority.
- No business or internal urgency may mint Foundation technical criticality.
- FSARM internal urgency/degradation policy is evidence only relative to Foundation priority/criticality truth.
- Foundation survival/protection/control floors and non-reclaimable reserves remain above Application workloads.

`FSARM_INTERNAL_URGENCY != FOUNDATION_APPLICATION_PRIORITY`

`FSARM_INTERNAL_URGENCY != FOUNDATION_TECHNICAL_CRITICALITY`

## 12. Monotonic Ordering and Supersession Protection

Each pressure/enforcement truth surface SHALL bind exact resource epoch/version, scope, monotonic sequence/effective order or equivalent anti-rollback identity, observation/effective time, and supersession relationship.

Older/superseded/same-order-conflicting truth SHALL fail closed.

## 13. Fail-Closed Rules

WP-05 SHALL fail closed for at least:

- missing/unavailable resource truth;
- epoch/version mismatch;
- stale/future/expired observations;
- rollback/supersession violations;
- contradictory same-order truth;
- global/Application or cross-Application substitution;
- allocation/ceiling mismatch;
- unknown/contradictory priority or technical-criticality truth;
- malformed/mismatched evidence or authority identities;
- unavailable truth represented as NORMAL;
- caller business/internal urgency treated as Foundation technical criticality;
- enforcement observation treated as WP-05 mutation authority;
- pressure state treated as Guardian/protective authority;
- aggregate consumer view treated as permission to erase constituent Application identity.

Unknown pressure is not absence of pressure.

## 14. Determinism and Reconstructability

Given the same accepted predecessor truth, same ordered observations, same transition policy/version and same effective boundary, WP-05 SHALL produce the same truth identity and outcome.

Evidence SHALL reconstruct exact inputs, scope, epoch, effective allocations/ceilings where applicable, priority/criticality truth, transition policy, previous state, derived state/reason, eligibility state and superseding state.

## 15. Consumer Compatibility

WP-05 truth SHALL remain suitable for later consumption by:

- accepted Stage 5 delivery/flow-control behavior;
- WP-06 request/decision evidence;
- WP-07 eligibility/context input;
- WP-08 safe per-Application and authorized aggregate-coordinator projection;
- Stage 11 QoS/observability technical evidence.

Consumers SHALL NOT become alternate pressure-truth owners.

Compatibility SHALL be generic and identity-bound. WP-05 SHALL NOT hard-code TARC or FSARM as its exclusive consumer.

## 16. Explicit Non-Scope

WP-05 SHALL NOT implement:

- FSARM coordinator identity/delegation mechanics;
- aggregate envelope creation;
- request/grant/cap/deny decisions (WP-06);
- reclamation/redistribution/rebalance/restoration execution (WP-07);
- FSARM internal redistribution execution;
- final per-Application or aggregate-coordinator load-shedding contract (WP-08);
- QoS/tail-latency service (Stage 11);
- external egress/credential security (Stage 12);
- FSA/Owner bounded evolution control plane (Stage 13);
- artifact publication/consumption (Stage 14);
- Application runtime hosting (Stage 15);
- environment qualification (Stage 16);
- operational-readiness authority (Stage 17);
- Guardian/Safe-State authority;
- Application business semantics.

## 17. Mandatory Verification Families

The final WP-05 implementation/closure design SHALL cover at least:

1. deterministic pressure derivation;
2. Foundation floors/reserves preservation;
3. exact Application allocation/ceiling binding;
4. accepted WP-04 priority/technical-criticality binding;
5. freshness/time rejection;
6. scope isolation/non-disclosure;
7. unknown/unavailable fail-closed behavior;
8. eligibility-versus-execution separation;
9. non-mutation ownership enforcement;
10. pressure-versus-protective-authority separation;
11. transition stability;
12. monotonic ordering/supersession;
13. accepted Stage 5 pressure-consumer compatibility;
14. zero-Application operation;
15. WP-01 through WP-04 predecessor regression;
16. Application-business exclusion;
17. FCR-0031 compatibility without TARC hard-binding;
18. aggregate-view constituent-identity preservation;
19. proof that WP-05 does not implement WP-06/WP-07/WP-08 mechanics.

## 18. Entry Conditions for Amended Owner Planning Acceptance

- IMP-001 v1.3 remains active;
- WP-01 through WP-04 remain accepted and closed;
- FCR-0031 reconciliation is completed at planning level;
- FCR-0010 and FCR-0007 are reconciled with the new FSARM direction;
- fresh Red-Team passes this v0.4 or a later successor;
- no duplicate pressure owner;
- no accepted closure reopened;
- no unresolved architecture/authority ambiguity within WP-05 scope.

## 19. Planning Exit Conditions

This amended WP-05 planning successor is ready for Owner acceptance only after fresh Red-Team passes.

Prior WP-05 planning acceptance remains historically valid for its prior artifact but does not automatically accept this amended successor.

Planning acceptance SHALL NOT grant implementation authority.

Because the cross-boundary consumer model materially changed, implementation shall remain paused until renewed explicit Owner implementation authorization is issued after amended planning acceptance.

## 20. Authority Markers

`WP05_PLANNING_v0.4 = OWNER_REVIEW_CANDIDATE`

`WP05_OWNER_ACCEPTED_v0.4 = NO`

`WP05_IMPLEMENTATION_AUTHORITY_FOR_v0.4 = NO`

`WP05_IMPLEMENTATION_RESUME = NO`

`WP05_RESOURCE_MUTATION_AUTHORITY = NONE`

`FCR0031_RECONCILED = YES_AT_PLANNING_CANDIDATE_LEVEL`

`TARC_HARD_BINDING = REMOVED`

`FSARM_EXCLUSIVE_WP05_CONSUMER = NO`
