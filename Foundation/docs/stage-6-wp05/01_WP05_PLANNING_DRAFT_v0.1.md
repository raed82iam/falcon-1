# Stage 6 WP-05 — Resource Pressure, Preemption and Enforcement-State Truth

**Document:** Planning Draft v0.1  
**Status:** DRAFT / NOT OWNER ACCEPTED / NO IMPLEMENTATION AUTHORITY  
**Branch:** `foundation-development`  
**Governing Master Plan:** IMP-001 v1.3  
**Relevant FCR:** FCR-0010  

## 1. Purpose

WP-05 defines the singular Foundation-owned technical truth describing current resource pressure, preemption eligibility, and resource enforcement state after consuming the already accepted Stage 6 WP-01 through WP-04 resource identities, Foundation total-resource truth, Application allocation/ceiling truth, and governed Application priority / Foundation technical-criticality truth.

WP-05 SHALL NOT execute Application business degradation, transport QoS scheduling, resource-request decisions, redistribution/rebalance, or Application-specific load-shedding logic.

## 2. Accepted Preconditions Preserved

WP-05 SHALL consume and SHALL NOT redefine:

- WP-01 canonical resource-governance identities and evidence primitives;
- WP-02 Foundation total-resource truth, Foundation protection floors, recovery reserves, and allocatable-capacity truth;
- WP-03 attributable Application allocation/quota/ceiling/isolation state;
- WP-04 governed cross-Application Application-priority truth and separate Foundation technical-criticality truth;
- accepted Stage 5 delivery-side pressure-consumption behavior where it already consumes Foundation-governed pressure truth;
- Foundation validity with zero Applications.

No closed predecessor is reopened by this planning draft.

## 3. WP-05 Owned Truth

WP-05 owns only Foundation technical resource-pressure and enforcement-state truth.

At minimum, the model SHALL support attributable truth for:

1. resource class and governed resource scope;
2. current Foundation resource epoch/version;
3. total capacity reference and allocatable-capacity reference;
4. current admitted Application allocation/ceiling references where an Application-specific pressure state is derived;
5. current pressure state;
6. current enforcement state;
7. preemption/reclamation eligibility state, without executing reclamation;
8. effective time, observation time, freshness, expiry and supersession;
9. authority/evidence identities;
10. causation/correlation identities where pressure is derived from an earlier governed state change;
11. deterministic reason/category for the pressure/enforcement state;
12. deterministic snapshot/state identity.

## 4. Pressure-State Model

The exact enum names remain design-detail candidates until Red-Team and Owner acceptance, but the semantics SHALL distinguish at least:

- NORMAL: current governed capacity/allocation state is within the applicable safe technical envelope;
- CONSTRAINED: technical resource headroom is materially reduced and lower-priority admission may need restriction;
- DEGRADED: current resource conditions require enforced reduction or restriction of reclaimable workload to preserve higher-priority and Foundation-protected capacity;
- CRITICAL: Foundation survival/protection/control capacity is at risk or resource governance can no longer safely preserve the required protected technical set without stronger protective action;
- UNKNOWN / UNAVAILABLE: authoritative pressure truth cannot be established; SHALL NOT be represented as NORMAL.

Pressure-state names do not create business meaning and do not authorize any Application action by themselves.

## 5. Enforcement-State Truth

WP-05 SHALL distinguish observed pressure from Foundation enforcement truth.

Enforcement truth may describe bounded technical states such as:

- no additional technical restriction currently enforced;
- admission restricted;
- lower-priority reclaimable allocation marked eligible for reduction/preemption;
- ceiling reduction pending/effective where that reduction is already authorized by the applicable resource-governance authority;
- protected Foundation floor/reserve preservation active;
- recovery/restoration not yet eligible;
- truth unavailable / fail-closed.

WP-05 SHALL NOT itself perform WP-07 reclamation, redistribution, rebalance or restoration execution.

## 6. Preemption Eligibility Versus Preemption Execution

WP-05 may compute or expose attributable eligibility indicating that an exact reclaimable allocation/scope may be considered for preemption under the governing priority/criticality/floor/reserve rules.

Eligibility SHALL NOT equal execution.

`PREEMPTION_ELIGIBLE != PREEMPTED`

Any actual reclaim/reduction/redistribution/rebalance/restoration execution belongs to separately governed WP-07 or an already accepted predecessor capability where exact evidence proves such ownership.

## 7. Foundation and Application Boundary

Foundation owns:

- total-resource truth;
- protected Foundation floors and reserves;
- allocation/ceiling truth;
- pressure/enforcement truth;
- governed cross-Application priority and Foundation technical criticality;
- final Foundation technical resource authority.

Application owns:

- its business degradation hierarchy;
- which strategies/features/workloads it sheds inside the granted envelope;
- internal Application scheduling and bounded resource distribution;
- Application-specific business interpretation of pressure.

For Falcon Self-Aware Trading Application, Foundation truth terminates at the admitted Application boundary and TARC remains the sole Trading Application operational resource controller/request communicator. WP-05 SHALL NOT communicate directly with Trading LSAs/CSAs/strategies/Risk/Execution/Guardian as independent Foundation resource principals.

## 8. Application-Neutral Priority Consumption

WP-05 SHALL consume accepted WP-04 priority truth rather than allow callers to self-declare priority.

Trading-related Applications may carry the Owner-approved highest Application-level technical resource priority, but:

- Foundation survival/protection/control floors remain above all Application workloads;
- Trading priority applies only to Foundation-governed technical resource decisions;
- no Trading business urgency may mint Foundation technical criticality;
- no Application may self-promote its priority class.

## 9. Fail-Closed Rules

WP-05 SHALL fail closed for at least:

- missing resource-truth evidence;
- unknown total capacity where required for the decision;
- mismatched resource epoch/version;
- stale/future/expired observation;
- cross-Application substitution;
- allocation/ceiling mismatch;
- unknown or contradictory priority/criticality truth;
- malformed authority/evidence identity;
- contradictory pressure/enforcement states;
- impossible state such as representing unavailable truth as NORMAL;
- caller-supplied business priority used as Foundation technical criticality.

Unknown pressure SHALL NOT be treated as absence of pressure.

## 10. Determinism and Reconstructability

Given the same accepted predecessor truth and the same effective observation boundary, WP-05 SHALL produce the same pressure/enforcement truth identity and outcome.

The evidence chain SHALL permit later reconstruction of:

- what resource truth was known;
- which allocations/ceilings were effective;
- which priority/technical-criticality records were effective;
- what pressure/enforcement state was derived;
- why the state was derived;
- whether preemption was merely eligible or actually executed later under another authority;
- what later state superseded the snapshot.

## 11. Consumer Compatibility

WP-05 truth SHALL be suitable for consumption by:

- accepted Stage 5 delivery/flow-control capability where it already consumes Foundation-governed pressure truth;
- later WP-06 request/decision boundary as evidence input;
- later WP-07 reclamation/rebalance execution;
- later WP-08 Application-facing resource-state/load-shedding signal boundary;
- Stage 11 QoS/observability as a future consumer of technical pressure evidence.

Consumer compatibility SHALL NOT create a second pressure-truth owner.

## 12. Explicit Non-Scope

WP-05 SHALL NOT implement:

- Application resource-request submission or grant/cap/deny decisions (WP-06);
- actual reclamation, redistribution, rebalance or restoration execution (WP-07);
- final per-Application load-shedding signal/consumer contract (WP-08);
- transport QoS scheduler, queue policy or tail-latency service (Stage 11);
- research/provider/broker egress (Stage 12);
- FSA/Owner evolution control plane (Stage 13);
- artifact publication/consumption mechanics (Stage 14);
- Application runtime hosting (Stage 15);
- deployment-environment qualification (Stage 16);
- production/operational-readiness authority (Stage 17);
- Trading strategy/Risk/execution/business semantics.

## 13. Proposed Verification Families

Planning-only verifier families:

1. deterministic pressure derivation;
2. Foundation-floor/reserve preservation;
3. Application-allocation/ceiling binding;
4. priority/technical-criticality binding;
5. stale/future/expired evidence rejection;
6. cross-Application substitution rejection;
7. unknown/unavailable truth fail-closed behavior;
8. eligibility-versus-execution separation;
9. Application business-semantics exclusion;
10. Stage 5 pressure-consumer compatibility;
11. zero-Application Foundation pressure truth;
12. predecessor regression WP-01 through WP-04.

Exact test counts are intentionally not invented at planning v0.1.

## 14. Entry and Exit Conditions

### Entry

- IMP-001 v1.3 active;
- Stage 6 WP-01 through WP-04 accepted and closed;
- FCR-0010 mapped to WP-05/WP-06/WP-07/WP-08;
- Application ACK of the newly activated canonical mapping may still be pending during draft preparation, but SHALL be reconciled before final Owner design acceptance and before any implementation authorization.

### Planning Exit

WP-05 planning can be presented for Owner acceptance only after:

- Red-Team of this draft is complete;
- any FCR-0010 Application ACK/objection is reconciled;
- no conflict remains with accepted WP-01 through WP-04 boundaries;
- no duplicate pressure owner is created;
- requirement-to-verification trace exists at design level;
- Owner explicitly accepts the final WP-05 planning/design package.

## 15. Authority Markers

`WP05_PLANNING_DRAFT = YES`

`WP05_OWNER_ACCEPTED = NO`

`WP05_IMPLEMENTATION_AUTHORITY = NO`

`FCR_0010_ACK_REQUIRED_BEFORE_FINAL_DESIGN_ACCEPTANCE = YES`
