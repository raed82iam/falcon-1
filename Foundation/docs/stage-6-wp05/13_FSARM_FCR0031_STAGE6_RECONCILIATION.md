# Stage 6 — FSARM / FCR-0031 Reconciliation for WP-05 through WP-08

**Status:** FOUNDATION RECONCILIATION CANDIDATE / NO IMPLEMENTATION AUTHORITY  
**Branch:** `foundation-development`  
**Controlling new input:** FCR-0031  
**Related FCRs:** FCR-0010, FCR-0007  
**Preserved closed predecessors:** Stage 6 WP-01 through WP-04  
**Current WP-05 implementation state:** PAUSED / UNVALIDATED

## 1. Purpose

This record reconciles the Owner-directed FSATS-wide resource-management model `FSARM — Falcon Self-Aware Resource Management` with the accepted Stage 6 Foundation Resource Governance sequence.

The reconciliation is prospective. It SHALL NOT reopen or reinterpret accepted-and-closed WP-01 through WP-04 unless independent explicit closure evidence proves a defect inside their exact accepted scope. No such defect is identified here.

## 2. Controlling architectural result

The existing Stage 6 WP split remains valid and does not require amendment to IMP-001 v1.3 at this time:

- WP-05 remains Foundation-owned Resource Pressure, Preemption Eligibility and Enforcement-State Truth.
- WP-06 remains Additional Resource Request and Decision Boundary.
- WP-07 remains Reclamation, Redistribution, Rebalance and Restoration.
- WP-08 remains Per-Application Resource State and Load-Shedding Signal Boundary.

FCR-0031 changes the FSATS-side integration identity and coordination semantics, not the Foundation ownership of total-resource truth or the Stage 6 family decomposition.

`STAGE6_SEQUENCE_CHANGE_REQUIRED = NO`

`FSATS_RESOURCE_COORDINATOR = FSARM`

`FOUNDATION_RESOURCE_AUTHORITY = PRESERVED`

## 3. Authority and boundary invariants

The following remain controlling:

1. Foundation owns total-resource truth and final grant/cap/deny/reduce/revoke/reclaim/rebalance/restore authority.
2. Foundation survival/protection/control floors and non-reclaimable reserves remain protected above Application workloads.
3. `REQUESTED_RESOURCE != GRANTED_RESOURCE` remains mandatory.
4. FSARM does not gain business, Trading, Risk, Guardian-command, provider, simulation, lifecycle, FSA or Owner authority.
5. Each admitted Application remains independently identifiable, attributable, accountable and isolated.
6. Aggregate FSATS coordination SHALL NOT erase per-Application allocation/evidence/accounting truth.
7. FSARM is an Application/System-side coordinator and consumer/requester role. It does not become the Foundation resource truth owner.
8. Foundation validity with zero Applications remains unchanged.

## 4. Superseded future-facing assumptions

The following prospective assumptions are superseded where they conflict with FCR-0031:

- TARC as the sole future FSATS-wide resource requester/outcome endpoint.
- TARC as the system-wide operational resource controller across FSATS Applications.
- the assumption that FSAPMA, Guardian and FSTSimA can never participate in a governed shared FSATS resource coordination model.

Historical TARC records remain audit evidence. T-LSA-13 may remain Trading resource awareness/evaluation, but it SHALL NOT be treated as the future system-wide FSATS resource coordinator.

`TARC_HISTORICAL_RECORD = PRESERVED`

`TARC_FUTURE_FSATS_WIDE_AUTHORITY = SUPERSEDED`

## 5. WP-05 impact

WP-05's generic Foundation truth role remains valid.

Preserved WP-05 responsibilities:

- deterministic resource-pressure truth;
- exact resource scope and resource-class binding;
- Foundation/global versus Application-bound pressure separation;
- preemption eligibility truth without execution;
- observed enforcement-state truth without mutation;
- evidence, time, freshness, expiry, epoch, sequence and supersession;
- accepted WP-01 through WP-04 predecessor consumption;
- scope isolation/non-disclosure;
- fail-closed unknown/unavailable behavior;
- separation of pressure state from Guardian/protective authority;
- zero-Application validity.

Required WP-05 amendment:

- remove TARC-specific compatibility as a mandatory WP-05 truth invariant;
- make Application/System consumer identity generic and attributable;
- permit later WP-08 projection to an authorized aggregate coordinator such as FSARM without allowing WP-05 to define or execute redistribution;
- preserve per-Application pressure truth even when an aggregate FSATS coordinator consumes it;
- do not create an aggregate resource envelope or redistribution execution inside WP-05.

`WP05_GENERIC_TRUTH_MODEL = PRESERVED`

`WP05_TARC_HARD_BINDING = REMOVE`

`WP05_FSARM_REDISTRIBUTION_EXECUTION = PROHIBITED`

## 6. WP-06 impact

WP-06 is materially affected because the requester identity changes prospectively.

WP-06 planning SHALL define a generic governed request/decision boundary capable of admitting an authorized aggregate Application-system resource coordinator while preserving exact attribution.

At minimum the future design must bind:

- exact FSARM identity and admitted system scope;
- exact set of constituent admitted Application identities covered by the request;
- attributable unmet need per Application/resource class;
- evidence that governed internal redistribution/shedding was evaluated first where required by admitted FSATS policy;
- current Foundation grants/ceilings relevant to the request;
- protected minimums/non-reclaimable constraints;
- request correlation/causation, effective period and expiry;
- Foundation decision identity and exact outcome;
- projection of decision impact back to each affected Application;
- fencing/single-active-requester semantics and stale/split-brain rejection;
- fail-closed rejection of unauthorized coordinator substitution.

WP-06 SHALL NOT assume that one constituent Application owns the entire aggregate system resource truth.

`WP06_REQUESTER_MODEL = GENERIC_AGGREGATE_COORDINATOR_CAPABLE`

`WP06_FSARM_IDENTITY_BINDING_REQUIRED = YES`

## 7. WP-07 impact

WP-07 is the principal execution-impact area for `INTERNAL_REDISTRIBUTION_FIRST`.

The future WP-07 design SHALL distinguish two different authorities:

### 7.1 Foundation-side resource mutation
Foundation retains final platform allocation/reclaim/rebalance/restore authority where Foundation-governed grants or ceilings are changed.

### 7.2 FSATS-internal redistribution inside an admitted aggregate envelope
If Foundation authorizes an aggregate FSATS envelope or equivalent coordinated allocation model, FSARM may perform bounded internal redistribution only within the exact admitted authority and only while preserving:

- per-Application identity and accounting;
- protected minimums/floors;
- non-reclaimable grants;
- isolation/security constraints;
- evidence and reconstructability;
- no increase above the Foundation-granted aggregate capacity;
- no creation of Foundation grant authority;
- deterministic restoration/reversal rules;
- fail-closed behavior when aggregate authority is absent, stale, ambiguous or split-brain.

Foundation reconciliation does not yet choose between a first-class aggregate-envelope contract and coordinated per-Application allocations. That exact contract shape must be settled in WP-06/WP-07 planning before implementation.

`INTERNAL_REDISTRIBUTION_FIRST = REQUIRED_FSATS_BEHAVIOR`

`FOUNDATION_ADDITIONAL_REQUEST_SECOND = REQUIRED_FSATS_BEHAVIOR`

`AGGREGATE_ENVELOPE_CONTRACT_SHAPE = OPEN_FOR_GOVERNED_WP06_WP07_DESIGN`

## 8. WP-08 impact

WP-08 SHALL project safe, attributable resource-state/load-shedding information suitable for both:

- each individual admitted Application; and
- an authorized aggregate coordinator such as FSARM.

The aggregate view SHALL NOT erase constituent identities or expose unrelated Applications outside the admitted FSATS coordination scope.

WP-08 must support evidence sufficient for FSARM to know:

- current effective per-Application allocation/ceiling/resource state;
- pressure state and enforcement observations;
- protected minimums and reclaimability where authorized for consumption;
- current internal redistribution eligibility/context;
- stale/unavailable/superseded state;
- restoration conditions when applicable.

WP-08 SHALL NOT itself execute business load shedding or Foundation resource mutation.

## 9. Foundation model for aggregate coordination

The safest current Foundation planning model is:

`AGGREGATE_COORDINATION_WITH_PRESERVED_CONSTITUENT_TRUTH`

This means:

- FSARM may be recognized as one admitted resource coordination/request identity for an FSATS system scope;
- constituent Applications remain separately admitted and separately attributable;
- Foundation retains exact per-Application truth even if it also exposes an aggregate coordination view;
- no hidden pooling may make one Application's consumption/accounting indistinguishable from another's;
- any aggregate capacity identity must be derivable from and reconciled against constituent Foundation-governed grants;
- every redistribution outcome must be reconstructable to before/after per-Application resource state.

This is a planning disposition, not an implementation contract.

## 10. WP-05 current code disposition

The current unvalidated WP-05 implementation through `a8e1dc1befa85b451f9a2a6cfa75e26d544860a8` SHALL remain paused.

It may be reused only after file-level review proves the code is generic and contains no TARC-hard-bound semantics incompatible with this reconciliation. Any incompatible implementation shall be prospectively amended under renewed/confirmed authority.

No executable validation result exists for that baseline.

`WP05_CURRENT_CODE = UNVALIDATED_PAUSED`

`WP05_CURRENT_CODE_ACCEPTED_BASELINE = NO`

## 11. Required next planning action

Before WP-05 implementation resumes:

1. issue an amended WP-05 planning successor replacing TARC-specific future semantics with FSARM-compatible generic consumer semantics;
2. preserve all still-valid v0.3 WP-05 truth requirements;
3. run a fresh Red-Team against FCR-0031, FCR-0010 and FCR-0007;
4. obtain Owner acceptance of the amended WP-05 planning successor;
5. determine whether prior WP-05 implementation authorization remains applicable or whether renewed explicit authorization is required. Safest governance default is renewed explicit authorization after the material cross-boundary amendment.

WP-06/WP-07/WP-08 remain separately gated and are not implementation-authorized by this reconciliation.

## 12. Acceptance markers

`FCR0031_FOUNDATION_RECONCILIATION = CANDIDATE`

`WP01_WP04_CLOSURES = PRESERVED`

`WP05_PLAN_AMENDMENT_REQUIRED = YES`

`WP05_IMPLEMENTATION_RESUME = NO`

`WP06_IMPLEMENTATION_AUTHORITY = NO`

`WP07_IMPLEMENTATION_AUTHORITY = NO`

`WP08_IMPLEMENTATION_AUTHORITY = NO`

`FSARM_FOUNDATION_AUTHORITY_ELEVATION = NO`
