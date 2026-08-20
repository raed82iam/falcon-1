# Stage 6 WP-05 — Resource Pressure, Preemption Eligibility and Enforcement-State Truth

**Document:** Planning Draft v0.3 — Application ACK Reconciled  
**Status:** OWNER-REVIEW CANDIDATE / NOT OWNER ACCEPTED / NO IMPLEMENTATION AUTHORITY  
**Branch:** `foundation-development`  
**Governing Master Plan:** IMP-001 v1.3  
**Relevant FCR:** FCR-0010  
**Supersedes for planning review:** `03_WP05_PLANNING_DRAFT_v0.2_REMEDIATED.md`

## 1. Purpose

WP-05 defines the singular Foundation-owned technical truth describing resource pressure, preemption eligibility, and resource enforcement state by consuming accepted Stage 6 WP-01 through WP-04 resource identities, Foundation total-resource truth, Application allocation/ceiling truth, and governed Application-priority / Foundation technical-criticality truth.

WP-05 is a truth derivation and observation boundary. It SHALL NOT mutate resource grants, allocations, ceilings, reclaimable quantities, resource ownership, Application business state, lifecycle state, Guardian state, or Falcon-wide protective state.

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

Closed predecessors remain closed. A missing later capability SHALL NOT be reclassified as a defect in WP-01 through WP-04 unless explicit closure trace proves it was inside the exact accepted scope.

## 3. FCR-0010 Cross-Workstream Reconciliation

The Application workstream has completed the refreshed acknowledgement of the activated Stage 6 mapping and returned the FCR to Foundation.

Current canonical FCR-0010 state:

- `Waiting On: FOUNDATION`;
- `Application ACK Status: COMPLETE`;
- WP-05 = Resource Pressure + Preemption + Enforcement-State Truth;
- WP-06 = Additional Resource Request + Decision Boundary;
- WP-07 = Reclamation + Redistribution + Rebalance + Restoration;
- WP-08 = Per-Application Resource State + Load-Shedding Signal Boundary;
- TARC-only Trading resource-governance boundary acknowledged and preserved.

Application acknowledgement does not grant implementation authority. It confirms the consuming-side mapping and jurisdiction boundary that later implementation SHALL preserve.

For Falcon Self-Aware Trading Application:

- authoritative Foundation resource truth terminates at the admitted Trading Application boundary;
- TARC is the sole Trading Application operational resource controller and Foundation resource-request communicator;
- Trading internal roles may contribute evidence but SHALL NOT become separate Foundation resource principals;
- no direct Guardian/break-glass Trading resource-request principal is authorized;
- Foundation retains total-resource truth and final grant/cap/deny/reduce/revoke/reclaim/rebalance/restore authority;
- `REQUESTED_RESOURCE != GRANTED_RESOURCE` remains mandatory.

`FCR_0010_REFRESHED_APPLICATION_ACK = COMPLETE`

`FCR_0010_CURRENT_ACTOR = FOUNDATION`

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

## 5. Pressure Truth Scope Separation

WP-05 SHALL distinguish Foundation-global/resource-class pressure truth from exact Application-bound pressure/enforcement truth.

Global pressure SHALL NOT expose another Application's private allocation details. Application-bound truth SHALL bind one exact admitted Application and its own effective allocation/ceiling context only.

`GLOBAL_PRESSURE != APPLICATION_PRESSURE`

`APPLICATION_A_PRESSURE != APPLICATION_B_PRESSURE`

Any scope substitution SHALL fail closed.

## 6. Pressure-State Model

Exact enum names remain implementation-design candidates, but semantics SHALL distinguish at least:

- `NORMAL`;
- `CONSTRAINED`;
- `DEGRADED`;
- `CRITICAL`;
- `UNKNOWN/UNAVAILABLE`.

These states are evidence/truth only.

`PRESSURE_STATE != AUTHORITY`

`PRESSURE_STATE != PROTECTIVE_AUTHORITY`

`PRESSURE_STATE != GUARDIAN_COMMAND`

`PRESSURE_STATE != SAFE_STATE_ENTRY`

WP-05 may produce evidence consumed later by separately authorized protection/lifecycle/resource authorities, but it SHALL NOT command Guardian restriction, Falcon Safe State, lifecycle halt, protective release, or Application business action.

## 7. Deterministic Transition Stability

WP-05 SHALL prevent uncontrolled state flapping while preserving prompt recognition of genuine deterioration.

Implementation design SHALL use a governed, versioned, deterministic transition-stability mechanism such as hysteresis, debounce, minimum-observation qualification, asymmetric promotion/recovery conditions, or equivalent behavior.

Numeric thresholds/timers SHALL be selected only during separately authorized implementation design and shall be evidence-bound and resource-class appropriate.

## 8. Enforcement-State Truth Is Observation, Not Mutation

WP-05 may report observed/effective resource enforcement states produced by an authorized owner, but SHALL NOT perform the mutation itself.

WP-05 SHALL NOT:

- change allocation or ceiling;
- grant/cap/deny a request;
- reclaim/redistribute/rebalance capacity;
- restore a prior allocation;
- mutate WP-03 allocation truth;
- execute WP-07 behavior.

`ENFORCEMENT_OBSERVED_BY_WP05 != ENFORCEMENT_EXECUTED_BY_WP05`

`ENFORCEMENT_EXECUTED_BY_WP05 = PROHIBITED`

## 9. Preemption Eligibility Versus Execution

WP-05 may derive attributable eligibility for an exact reclaimable scope.

`PREEMPTION_ELIGIBLE != PREEMPTION_AUTHORIZED`

`PREEMPTION_AUTHORIZED != PREEMPTED`

Actual reclaim/reduction/redistribution/rebalance/restoration belongs to the separately authorized execution owner, principally WP-07 unless accepted evidence proves an earlier owner for an exact behavior.

## 10. Foundation/Application Boundary

Foundation owns platform technical resource truth and final Foundation technical resource authority.

Application owns business interpretation and internal reaction inside its admitted envelope.

For Trading, TARC remains the sole operational resource controller/request communicator. WP-05 SHALL NOT prescribe Trading degradation hierarchy, internal scheduling, Risk, strategy, execution, or business load-shedding behavior.

## 11. Priority and Criticality Consumption

WP-05 consumes accepted WP-04 truth.

- No caller may self-promote Application priority.
- No business urgency may mint Foundation technical criticality.
- Trading-related Applications retain the Owner-approved highest Application-level technical-resource priority domain only inside Foundation-governed resource decisions.
- Foundation survival/protection/control floors and non-reclaimable reserves remain above all Application workloads.

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
- caller business priority treated as Foundation technical criticality;
- enforcement observation treated as WP-05 mutation authority;
- pressure state treated as Guardian/protective authority.

Unknown pressure is not absence of pressure.

## 14. Determinism and Reconstructability

Given the same accepted predecessor truth, same ordered observations, same transition policy/version and same effective boundary, WP-05 SHALL produce the same truth identity and outcome.

Evidence SHALL reconstruct the exact inputs, scope, epoch, effective allocations/ceilings where applicable, priority/criticality truth, transition policy, previous state, derived state/reason, eligibility state and superseding state.

## 15. Consumer Compatibility

WP-05 truth SHALL remain suitable for later consumption by:

- accepted Stage 5 delivery/flow-control behavior;
- WP-06 request/decision evidence;
- WP-07 eligibility/context input;
- WP-08 safe Application-facing resource-state projection;
- Stage 11 QoS/observability technical evidence.

Consumers SHALL NOT become alternate pressure-truth owners.

## 16. Explicit Non-Scope

WP-05 SHALL NOT implement:

- request/grant/cap/deny decisions (WP-06);
- reclamation/redistribution/rebalance/restoration execution (WP-07);
- final per-Application load-shedding contract (WP-08);
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
17. FCR-0010/TARC boundary compatibility.

Exact test counts and numeric thresholds are intentionally deferred to authorized implementation design.

## 18. Entry Conditions for Owner Planning Acceptance

- IMP-001 v1.3 active;
- WP-01 through WP-04 accepted and closed;
- FCR-0010 mapping valid;
- refreshed Application ACK complete and reconciled;
- fresh Red-Team passes this v0.3 or a later successor;
- no duplicate pressure owner;
- no accepted closure reopened;
- no unresolved architecture/authority ambiguity.

## 19. Planning Exit Conditions

WP-05 planning is ready for Owner acceptance only after fresh Red-Team of this ACK-reconciled candidate passes and the Owner explicitly accepts the package.

Planning acceptance SHALL NOT grant implementation authority.

## 20. Authority Markers

`WP05_PLANNING_CANDIDATE = YES`

`WP05_OWNER_ACCEPTED = NO`

`WP05_IMPLEMENTATION_AUTHORITY = NO`

`WP05_RESOURCE_MUTATION_AUTHORITY = NONE`

`PRESSURE_STATE_CREATES_PROTECTIVE_AUTHORITY = NO`

`FCR_0010_REFRESHED_APPLICATION_ACK = COMPLETE`

`FCR_0010_CURRENT_ACTOR = FOUNDATION`
