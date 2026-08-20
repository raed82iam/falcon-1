# Stage 6 WP-05 — Resource Pressure, Preemption Eligibility and Enforcement-State Truth

**Document:** Planning Draft v0.2 — Remediated Candidate  
**Status:** OWNER-REVIEW CANDIDATE / NOT OWNER ACCEPTED / NO IMPLEMENTATION AUTHORITY  
**Branch:** `foundation-development`  
**Governing Master Plan:** IMP-001 v1.3  
**Relevant FCR:** FCR-0010  
**Supersedes for planning review:** `01_WP05_PLANNING_DRAFT_v0.1.md`

## 1. Purpose

WP-05 defines the singular Foundation-owned technical truth describing resource pressure, preemption eligibility, and resource enforcement state by consuming the already accepted Stage 6 WP-01 through WP-04 resource identities, Foundation total-resource truth, Application allocation/ceiling truth, and governed Application-priority / Foundation technical-criticality truth.

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

## 3. WP-05 Owned Truth

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

## 4. Pressure Truth Scope Separation

WP-05 SHALL distinguish, at minimum:

### 4.1 Foundation-global/resource-class pressure truth

Describes the Foundation-wide or exact resource-class technical pressure condition derived from Foundation-owned resource truth.

It SHALL NOT expose another Application's private allocation details merely because global pressure exists.

### 4.2 Exact Application-bound pressure/enforcement truth

Describes only the pressure/enforcement implications attributable to one exact admitted Application identity and its own effective allocation/ceiling/resource scope.

An Application-bound snapshot SHALL NOT contain another Application's allocation, ceiling, request, internal workload, or business-degradation state unless a separately authorized contract explicitly permits a safe aggregate/non-identifying field.

### 4.3 Non-substitutability

Global and Application-bound pressure identities are not interchangeable.

`GLOBAL_PRESSURE != APPLICATION_PRESSURE`

`APPLICATION_A_PRESSURE != APPLICATION_B_PRESSURE`

Any scope substitution SHALL fail closed.

## 5. Pressure-State Model

Exact enum names remain implementation-design candidates, but semantics SHALL distinguish at least:

- `NORMAL`: authoritative current state is within the applicable governed technical envelope;
- `CONSTRAINED`: headroom is materially reduced and technical admission/reclaimability restrictions may become relevant;
- `DEGRADED`: effective governed resource conditions indicate material technical restriction is present or required by an already authorized resource-governance action;
- `CRITICAL`: Foundation resource survival/protection/control capacity is at material technical risk, or the governed resource state can no longer preserve the required protected technical set without escalation to an appropriate separately authorized authority;
- `UNKNOWN/UNAVAILABLE`: authoritative pressure truth cannot be established and SHALL NOT be represented as NORMAL.

These states are evidence/truth only.

`PRESSURE_STATE != AUTHORITY`

`PRESSURE_STATE != PROTECTIVE_AUTHORITY`

`PRESSURE_STATE != GUARDIAN_COMMAND`

`PRESSURE_STATE != SAFE_STATE_ENTRY`

WP-05 may produce evidence later consumed by an authorized Guardian/protection/lifecycle/resource authority, but WP-05 SHALL NOT command Guardian restriction, Falcon Safe State, lifecycle halt, protective release, or business action.

## 6. Deterministic Transition Stability

WP-05 SHALL prevent uncontrolled state flapping while preserving rapid recognition of genuine worsening conditions.

The implementation design SHALL therefore use a governed, versioned, deterministic transition-stability mechanism such as hysteresis, debounce, minimum-observation qualification, asymmetric promotion/recovery conditions, or an equivalent method.

The planning requirement is semantic, not numeric:

- worsening to a materially safer-to-fail state SHALL NOT be hidden by smoothing;
- recovery to a less restrictive state SHALL require sufficient governed evidence;
- transition policy and version SHALL be evidence-material;
- identical ordered observations under the same transition policy SHALL produce the same transition result;
- threshold values/timers SHALL be defined only during authorized implementation design and verified against resource-class characteristics rather than invented in planning.

## 7. Enforcement-State Truth Is Observation, Not Mutation

WP-05 SHALL distinguish derived/observed enforcement truth from the authority that created or executed the underlying resource mutation.

WP-05 may report states such as:

- no resource-local restriction observed;
- technical admission restriction observed/effective;
- exact reclaimable scope eligible for consideration;
- exact ceiling/allocation reduction observed as pending/effective because another authorized owner produced that state;
- protected Foundation floor/reserve preservation active;
- restoration eligibility not established;
- enforcement truth unavailable/fail-closed.

WP-05 SHALL NOT:

- reduce/increase an allocation;
- reduce/increase a ceiling;
- grant/cap/deny a resource request;
- reclaim or redistribute capacity;
- rebalance allocations;
- restore a prior allocation;
- mutate WP-03 allocation truth;
- execute WP-07 behavior.

`ENFORCEMENT_OBSERVED_BY_WP05 != ENFORCEMENT_EXECUTED_BY_WP05`

`ENFORCEMENT_EXECUTED_BY_WP05 = PROHIBITED`

If an observed enforcement state derives from another owner, its authority/result identity SHALL be attributable in the WP-05 evidence chain.

## 8. Preemption Eligibility Versus Execution

WP-05 may derive attributable eligibility indicating that an exact reclaimable scope may be considered for preemption under effective priority, technical-criticality, floor, reserve, allocation and resource-state rules.

Eligibility does not mutate anything.

`PREEMPTION_ELIGIBLE != PREEMPTION_AUTHORIZED`

`PREEMPTION_AUTHORIZED != PREEMPTED`

Actual reclaim/reduction/redistribution/rebalance/restoration belongs to the separately authorized execution owner, planned principally for WP-07, or to an accepted predecessor only where exact accepted evidence proves ownership.

## 9. Foundation/Application Boundary

Foundation owns platform technical truth and final Foundation technical resource authority.

Application owns its business interpretation and internal reaction inside its admitted envelope.

For Falcon Self-Aware Trading Application:

- authoritative Foundation resource truth terminates at the admitted Trading Application boundary;
- TARC remains the sole Trading Application operational resource controller and Foundation resource-request communicator;
- T-LSA-13 owns Trading resource awareness/evaluation inside the Application design;
- Trading LSAs/CSAs/components/Risk/Execution/strategies/Guardian-related signals may not become independent Foundation resource principals merely by consuming or contributing evidence;
- WP-05 SHALL NOT prescribe Trading's degradation hierarchy or business load-shedding decisions.

For any Application:

- internal projections are Application-owned and non-authoritative to Foundation;
- an internal projection cannot widen a Foundation grant/ceiling;
- stale/revoked/superseded Foundation truth invalidates any dependent projection according to the Application contract.

## 10. Priority and Criticality Consumption

WP-05 SHALL consume accepted WP-04 truth.

- No caller may self-promote Application priority.
- No business urgency may mint Foundation technical criticality.
- Trading-related Applications retain the Owner-approved highest Application-level technical-resource priority domain only within Foundation-governed resource decisions.
- Foundation survival/protection/control floors and non-reclaimable reserves remain above Application workloads.

## 11. Monotonic Ordering and Supersession Protection

Freshness alone is insufficient. WP-05 SHALL preserve deterministic ordering/supersession.

Each pressure/enforcement truth surface SHALL bind to:

- exact Foundation resource epoch/version;
- exact scope identity;
- monotonic sequence/effective-order identity or an equivalent deterministic anti-rollback mechanism;
- observation/effective time;
- predecessor/superseded state identity where applicable.

A numerically/time-valid but older or superseded snapshot SHALL NOT overwrite newer authoritative truth.

Fail closed on:

- rollback to an older epoch;
- lower sequence/effective order presented as current;
- same-order conflicting payload;
- epoch/scope/order substitution;
- ambiguous concurrent truth that cannot be deterministically reconciled.

## 12. Fail-Closed Rules

WP-05 SHALL fail closed for at least:

- missing resource-truth evidence;
- unavailable total/allocatable capacity where required;
- mismatched resource epoch/version;
- stale/future/expired observation;
- rollback/supersession violation;
- same-order contradictory truth;
- global/Application scope substitution;
- cross-Application substitution;
- allocation/ceiling mismatch;
- unknown/contradictory priority or technical-criticality truth;
- malformed or mismatched authority/evidence identity;
- contradictory pressure/enforcement states;
- unavailable truth represented as NORMAL;
- caller business priority used as Foundation technical criticality;
- enforcement observation presented as WP-05 mutation authority;
- pressure state presented as Guardian/protective authority.

Unknown pressure is not absence of pressure.

## 13. Determinism and Reconstructability

Given the same accepted predecessor truth, same ordered observations, same transition policy/version and same effective boundary, WP-05 SHALL produce the same truth identity and outcome.

The evidence chain SHALL reconstruct:

- resource truth known at derivation;
- exact scope and epoch;
- allocations/ceilings effective where relevant;
- priority/technical-criticality records effective;
- transition policy/version;
- prior pressure state and ordered observations;
- derived pressure/enforcement state and reason;
- preemption eligibility versus any later authorization/execution;
- exact superseding state.

## 14. Consumer Compatibility

WP-05 truth SHALL be suitable for later consumption by:

- accepted Stage 5 delivery/flow-control capability where it already consumes Foundation-governed pressure truth;
- WP-06 as request/decision evidence input;
- WP-07 as eligibility/context input without delegating execution ownership to WP-05;
- WP-08 as the source for safe Application-facing pressure/enforcement projections;
- Stage 11 QoS/observability as a future consumer of technical evidence.

Consumers SHALL NOT become alternate pressure-truth owners.

## 15. Explicit Non-Scope

WP-05 SHALL NOT implement:

- Application request submission or grant/cap/deny decisions (WP-06);
- reclamation/redistribution/rebalance/restoration execution (WP-07);
- final per-Application load-shedding signal contract (WP-08);
- transport QoS scheduling or tail-latency service (Stage 11);
- external egress/credential security (Stage 12);
- FSA/Owner bounded evolution control plane (Stage 13);
- artifact publication/consumption (Stage 14);
- Application runtime hosting (Stage 15);
- environment qualification (Stage 16);
- operational-readiness authority (Stage 17);
- Guardian/Safe-State authority except later evidence consumption by their separately authorized owner;
- Application business semantics.

## 16. Requirement-to-Verification Families

The following design-level families are mandatory before implementation closure can ever be claimed:

### WP05-VF-01 — Deterministic pressure derivation
Same authoritative ordered input -> same pressure/enforcement truth and identity.

### WP05-VF-02 — Foundation floors/reserves preservation
Pressure truth cannot imply consumption/mutation of protected non-reclaimable Foundation floors/reserves.

### WP05-VF-03 — Application allocation/ceiling binding
Application-bound pressure truth must bind exact admitted Application and exact effective allocation/ceiling context.

### WP05-VF-04 — Priority/technical-criticality binding
Caller declarations cannot replace accepted WP-04 truth.

### WP05-VF-05 — Freshness/time rejection
Missing/stale/future/expired observations fail closed.

### WP05-VF-06 — Scope isolation/non-disclosure
Global pressure and Application pressure are non-substitutable; one Application cannot receive another Application's allocation details through WP-05 truth.

### WP05-VF-07 — Unknown/unavailable fail-closed
Unknown cannot become NORMAL and contradictory truth cannot be silently resolved.

### WP05-VF-08 — Eligibility-versus-execution separation
Preemption eligibility cannot mutate resource state and cannot be represented as execution.

### WP05-VF-09 — Non-mutation enforcement boundary
Attempts to use WP-05 to change allocation/ceiling/grant/reclaim state are rejected structurally or by ownership architecture.

### WP05-VF-10 — Pressure-versus-protective-authority separation
CRITICAL/DEGRADED pressure cannot mint Guardian, Safe-State, lifecycle or business authority.

### WP05-VF-11 — Transition stability
Deterministic stability mechanism prevents uncontrolled flapping, while materially worsening conditions remain observable promptly according to governed policy.

### WP05-VF-12 — Monotonic ordering/supersession
Older/superseded/same-order-conflicting snapshots fail closed.

### WP05-VF-13 — Stage 5 pressure-consumer compatibility
Accepted Stage 5 consumers use WP-05 truth without duplicate pressure ownership or accepted-capability redesign.

### WP05-VF-14 — Zero-Application operation
Foundation pressure truth remains valid/healthy where applicable with zero admitted Applications and does not invent an Application dependency.

### WP05-VF-15 — Predecessor regression
WP-01 through WP-04 accepted behavior remains unchanged.

### WP05-VF-16 — Application-business exclusion
No Trading/load-shedding/business hierarchy becomes Foundation behavior.

Exact test counts and numeric transition thresholds are intentionally deferred to authorized implementation design.

## 17. Entry Conditions

For final Owner planning acceptance:

- IMP-001 v1.3 remains active;
- WP-01 through WP-04 remain accepted/closed;
- FCR-0010 canonical Stage 6 mapping remains valid;
- current Application ACK/objection on the activated mapping is reconciled;
- Red-Team of this remediated candidate passes with no HIGH/CRITICAL open finding;
- no duplicate pressure owner is found;
- no closed predecessor is reopened.

Draft preparation itself does not require Application ACK, but final Owner design acceptance SHALL NOT be represented as complete until that cross-workstream review is reconciled.

## 18. Planning Exit Conditions

WP-05 planning is ready for Owner acceptance only when:

1. all v0.1 Red-Team findings are closed;
2. fresh Red-Team passes this v0.2 or a later remediated successor;
3. requirement-to-verification coverage is complete at planning level;
4. FCR-0010 Application response is reconciled or explicitly determines no new objection to WP-05 scope;
5. no unresolved architecture/authority ambiguity remains;
6. Owner explicitly accepts the final planning/design package.

Planning acceptance does not grant implementation authority.

## 19. Authority Markers

`WP05_PLANNING_CANDIDATE = YES`

`WP05_OWNER_ACCEPTED = NO`

`WP05_IMPLEMENTATION_AUTHORITY = NO`

`WP05_RESOURCE_MUTATION_AUTHORITY = NONE`

`PRESSURE_STATE_CREATES_PROTECTIVE_AUTHORITY = NO`

`FCR_0010_RECONCILIATION_REQUIRED_BEFORE_FINAL_ACCEPTANCE = YES`
