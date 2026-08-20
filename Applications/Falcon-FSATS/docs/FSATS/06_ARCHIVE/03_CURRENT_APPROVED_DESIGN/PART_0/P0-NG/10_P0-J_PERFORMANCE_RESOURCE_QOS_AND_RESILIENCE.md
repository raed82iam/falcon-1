# P0-J — Performance, Resource, QoS, Overload and Resilience

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-J only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-J defines how FSATS preserves low latency, throughput, protection continuity, truth reconstruction, and controlled degradation without bypassing authority or confusing business importance with Foundation technical criticality.

Performance is a quality of correct governed behavior. It is not permission to skip a required control.

---

## 2. Responsibility

P0-J owns the FSATS-side design for:

- multi-dimensional Work Identity;
- business workload lanes;
- end-to-end deadlines and freshness;
- bounded queues and concurrency;
- backpressure;
- load shedding inside Application authority;
- Trading workload-to-TARC translation;
- TARC internal resource tiers;
- the boundary to accepted Foundation resource-priority / technical-criticality governance;
- priority inversion controls;
- coherency and stale-work invalidation;
- overload/degraded behavior;
- staged restoration;
- latency/resource evidence.

P0-J does not own Foundation total-resource truth, Foundation grant decisions, or Foundation technical-criticality decisions.

---

## 3. Prime Performance Rule

```text
PERFORMANCE_OPTIMIZATION != AUTHORITY_BYPASS
LOW_LATENCY != LESS_VALIDATION
FAST_TRACK != EMERGENCY_AUTHORITY
```

A faster invalid decision remains invalid.

Hot paths may precompute, cache, pipeline, parallelize, partition, and remove avoidable work only when required authority, truth, Risk, protection, and dispatch semantics remain preserved.

---

## 4. Multi-Dimensional Work Identity

A material work item SHALL carry enough identity to determine purpose, scope, deadline, dependencies, invalidation behavior, and resource treatment.

Where applicable, Work Identity includes:

- Application identity;
- user/account/environment;
- market/instrument;
- business lane;
- action/purpose class;
- correlation/causation;
- source/decision/control epochs;
- original end-to-end deadline;
- relevant data/decision freshness;
- Risk/Guardian/control versions;
- resource-policy classification evidence;
- replay/test/operational classification.

Work Identity is descriptive/governance evidence. It does not create authority.

---

## 5. Canonical Trading Business Lanes

Initial business lanes are:

1. `PROTECTION`;
2. `RECONCILIATION`;
3. `OPEN_POSITION_MANAGEMENT`;
4. `NEAR_TRADE`;
5. `ACTIVE_WATCH`;
6. `CANDIDATE_EVALUATION`;
7. `DISCOVERY`;
8. `BACKGROUND`.

They describe Trading workload purpose and relative business importance.

```text
BUSINESS_LANE != TARC_RESOURCE_TIER
BUSINESS_LANE != FOUNDATION_APPLICATION_PRIORITY
BUSINESS_LANE != FOUNDATION_TECHNICAL_CRITICALITY
```

A lane does not grant resources beyond the admitted allocation and does not mint Foundation priority.

---

## 6. Lane Meaning

### 6.1 PROTECTION
Work required to preserve capital/protection obligations for existing exposure or active protection state.

### 6.2 RECONCILIATION
Work required to reconstruct authoritative broker/order/position/business truth after partial, delayed, ambiguous, or conflicting outcomes.

### 6.3 OPEN_POSITION_MANAGEMENT
Routine management of already-open exposure within valid Risk/Guardian/user/Owner authority.

### 6.4 NEAR_TRADE
Time-sensitive work for an already-promoted candidate approaching final admission/dispatch. It remains subject to all normal safety gates.

### 6.5 ACTIVE_WATCH
Monitoring of contexts already admitted to active attention.

### 6.6 CANDIDATE_EVALUATION
Analysis/strategy/Risk evaluation for bounded opportunities not yet admitted for execution.

### 6.7 DISCOVERY
Broader scanning/opportunity search.

### 6.8 BACKGROUND
Learning, analytics, maintenance, indexing, non-urgent research-support work, and other deferrable computation.

A workload may be reclassified only by admitted policy/evidence. A producer may not self-promote because it wants resources.

---

## 7. Original End-to-End Deadline

A latency-sensitive work item SHALL preserve the original validity/deadline budget across internal and cross-Application hops where technically applicable.

```text
END_TO_END_DEADLINE = ORIGINAL_BUDGET
PER_HOP_TIMEOUT_RESET = PROHIBITED_IF_IT_EXTENDS_ORIGINAL_VALIDITY
```

Deadline classes may include:

- opportunity deadline;
- protection deadline;
- reconciliation deadline;
- Data Product freshness deadline;
- user/Owner command expiry;
- provider/broker interaction deadline.

Deadline expiry does not automatically authorize retry.

---

## 8. Freshness and Latency Separation

A message can arrive quickly with stale source truth. Data can be fresh while the business action deadline is already invalid.

```text
LOW_TRANSPORT_LATENCY != FRESH_SOURCE_TRUTH
FRESH_DATA != VALID_ACTION_DEADLINE
CACHE_READ_TIME != SOURCE_FRESHNESS
```

Both deadline and freshness SHALL be checked when material.

---

## 9. Bounded Queues

Each material queue SHALL define:

- accountable owner;
- purpose;
- bounded capacity/depth policy;
- ordering semantics;
- deadline/expiry behavior;
- control-epoch invalidation;
- overload/degraded behavior;
- shedding rules;
- recovery rules;
- metrics/evidence.

Unbounded queues are prohibited for material latency-sensitive or safety-relevant work.

Queue presence does not preserve expired authority or opportunity validity.

---

## 10. Bounded Concurrency

Concurrency SHALL be constrained by actual bottlenecks, including:

- admitted Application resources;
- provider/broker quota/session limits;
- account/market limits;
- CPU/memory/network/storage;
- serialization/order requirements;
- idempotency;
- protection of higher-value work;
- dependency availability.

More threads do not create throughput when the limiting resource is an external quota, serialized broker state, or single authoritative owner.

---

## 11. Backpressure

When consumers or dependencies cannot keep up, FSATS SHALL propagate truthful pressure instead of silently accumulating unbounded work.

Permitted responses may include:

- slowing producers;
- reducing discovery/candidate intake;
- coalescing replaceable observations;
- rejecting expired work;
- shedding eligible lower-value work;
- entering a bounded degraded mode;
- rebalancing inside current Trading allocation through TARC;
- submitting an evidenced resource request through TARC only when the Foundation request runtime becomes available and authorized.

Backpressure does not permit silent loss of mandatory protection/reconciliation truth.

---

## 12. Load Shedding

Application-internal shedding SHALL remove/defer the lowest-value eligible work first while protecting the minimum capabilities needed for capital protection and truth reconstruction.

```text
SHED_REQUESTED != SHED_EFFECTIVE
```

TARC/resource policy SHALL verify that shedding actually released capacity.

A typical policy may prefer:

```text
BACKGROUND
 -> DISCOVERY
 -> CANDIDATE_EVALUATION
 -> ACTIVE_WATCH REDUCTION / COALESCING
 -> NEAR_TRADE ONLY WHEN NO LONGER VALID / ELIGIBLE
```

Protection, reconciliation, and required open-position management receive stronger preservation subject to actual resources and higher Foundation floors.

This Application-internal policy does not claim Foundation technical criticality.

---

## 13. TARC Internal Resource Tiers

TARC MAY map attributable Trading workload evidence into Trading-internal resource tiers according to admitted, versioned Application policy.

Caller-supplied priority is evidence only.

```text
CALLER_REQUESTED_PRIORITY != EFFECTIVE_TARC_TIER
```

TARC may consider:

- business lane;
- protection/Risk/execution context;
- deadline;
- internal pressure;
- dependency state;
- current admitted allocation/ceiling;
- governed Trading resource policy;
- attributable evidence.

TARC tiers remain inside the actual Trading allocation unless Foundation later grants a changed allocation through an authorized request/decision boundary.

---

## 14. Accepted Foundation Application Priority vs Technical Criticality

Stage 6 WP-04 is now `ACCEPTED_AND_CLOSED` within its exact Foundation-owned scope.

P0-J may rely on the accepted generic governance distinction:

```text
APPLICATION_RESOURCE_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY
```

Current Owner/Foundation semantics establish that Trading-related Applications are in the highest cross-Application **Application resource-priority domain**, while the following remain protected above Application workloads:

- Foundation survival/protection/control capacity;
- non-reclaimable reserves;
- Foundation Authority capability;
- Foundation Health/Recovery capability;
- security/evidence-integrity capacity;
- minimum capacity required to govern, revoke, isolate, and restore Applications.

Caller-proposed or Application-internal urgency/priority cannot self-create Foundation technical criticality.

P0-J SHALL NOT allow:

```text
PROTECTION_LANE -> AUTOMATIC_FOUNDATION_TECHNICAL_CRITICALITY
TARC_HIGH_TIER -> AUTOMATIC_FOUNDATION_TECHNICAL_CRITICALITY
GUARDIAN_URGENCY -> AUTOMATIC_FOUNDATION_TECHNICAL_CRITICALITY
TRADING_HIGH_APPLICATION_PRIORITY -> FOUNDATION_FLOOR_OVERRIDE
```

The accepted WP-04 relation is a Foundation governance capability, not an inheritance rule from FSATS metadata.

---

## 15. What WP-04 Closure Does Not Provide

WP-04 closure does not authorize or prove complete runtime capability for:

- resource-pressure handling beyond exact accepted predecessor/transport scope;
- preemption;
- enforcement-state runtime;
- full Application-facing load shedding;
- additional-resource request/decision runtime;
- reclamation;
- redistribution;
- rebalance;
- restoration;
- TARC-specific Foundation production behavior;
- Foundation ownership of Trading-internal distribution/business semantics.

```text
STAGE6_WP04_ACCEPTED != FULL_RESOURCE_RUNTIME
```

These remain later Stage 6 scope and/or open FCR dependencies.

---

## 16. Resource Truth Separation

Trading SHALL distinguish:

- accepted Foundation total-resource truth;
- admitted Trading allocation;
- quota;
- ceiling;
- internal current use;
- internal reservation;
- internal need;
- pressure evidence;
- shed request;
- shed effect;
- additional-resource request;
- Foundation decision/outcome;
- restoration eligibility.

```text
INTERNAL_NEED != FOUNDATION_PRESSURE_TRUTH
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

Stage 6 WP-01/02/03 provide accepted prerequisites within exact scopes. The remaining Application-facing pressure/request/shedding/restoration runtime remains separately governed.

---

## 17. Sole Trading Resource Request Path

When internal rebalancing/shedding is insufficient and the Foundation runtime request boundary later becomes implemented, verified, and authorized:

```text
DOMAIN / GUARDIAN / MSA / LSA / CSA NEED OR URGENCY EVIDENCE
 -> TARC
 -> TARC POLICY + CURRENT ALLOCATION ASSESSMENT
 -> TARC GOVERNED FOUNDATION REQUEST
 -> FOUNDATION GRANT / PARTIAL / CAP / DENY / REDUCE / REVOKE / RESTORE
 -> TARC INTERNAL RESPONSE
```

No other Trading role may submit the Trading Application resource request.

Guardian has no emergency/break-glass bypass.

TARC failure fails closed and does not mint a second requester.

FCR-0007 and FCR-0010 remain open for the full request/pressure/load-shedding/restoration runtime boundary.

---

## 18. Coherency Vector

Material multi-source work SHALL identify relevant versions/epochs rather than assuming similar wall-clock read times mean coherent truth.

A coherency vector may bind:

- Data Product identity/version/freshness;
- Market Profile/version;
- strategy/model version;
- Risk decision/version;
- capital state/reservation;
- Guardian directive epoch;
- user/Owner/subscription epoch;
- broker/account/capability state;
- resource state where material.

A material dependency change invalidates or requires revalidation of affected work.

---

## 19. Precomputation and Invalidation

FSATS may precompute stable expensive work to reduce hot-path latency.

Every precomputed result SHALL define:

- dependency set;
- version/freshness;
- invalidation triggers;
- permitted use scope;
- fallback behavior;
- whether it is advisory or authority-bearing evidence.

```text
FAST_CACHE != PERMISSION_TO_IGNORE_CHANGED_DEPENDENCY
```

The architecture favors:

```text
PRECOMPUTE_STABLE_WORK
+ REVALIDATE_MATERIAL_MUTABLE_GATES_LATE
= LOW_LATENCY_WITHOUT_STALE_AUTHORITY
```

---

## 20. Dispatch-Time Revalidation

P0-H owns the Trade Admission Chain. P0-J prohibits performance optimization from deleting late revalidation of material mutable gates.

Where required by consequence, dispatch-time checks include current:

- Guardian/control epoch;
- user/Owner restriction state;
- Risk validity;
- capital reservation;
- broker/account/capability state;
- market/session eligibility;
- critical Data Product freshness;
- other material mutable authority/safety facts.

A hot path may reuse evidence only when its validity remains established.

---

## 21. Priority Inversion

Priority inversion occurs when lower-value work blocks resources required by higher-value work.

Controls may include where appropriate:

- bounded critical sections;
- lock avoidance/partitioning;
- admission limits;
- queue separation;
- cancellation of eligible lower-value work;
- reserved internal capacity;
- technically safe priority inheritance that does not create authority;
- preemption only when later explicitly supported/authorized.

Priority handling may not corrupt business state or external API semantics.

---

## 22. Near-Trade Fast Track

Fast Track reduces avoidable latency for already-eligible time-sensitive work.

It SHALL NOT:

- skip Unified Risk;
- skip capital reservation;
- skip Guardian/user/Owner/subscription controls;
- skip broker/account/capability checks;
- skip pre-dispatch safety;
- skip required dispatch-time revalidation;
- self-mint Foundation technical criticality;
- become emergency authority.

```text
FAST_TRACK = LESS_AVOIDABLE_LATENCY
FAST_TRACK != FEWER_REQUIRED_GATES
```

---

## 23. Telemetry vs Exact Evidence

Bounded operational metrics and exact reconstructable evidence are distinct.

Metrics MAY aggregate/sample/bucket to control cost/cardinality.

Material audit/decision evidence SHALL remain reconstructable where required.

```text
METRIC_AGGREGATION != AUDIT_EVIDENCE_DELETION
```

---

## 24. Overload Behavior

Under overload:

- expired work is rejected/removed;
- lower-value eligible work is shed first;
- new exposure is restricted when required gates cannot complete safely/in time;
- protection/reconciliation/open-position obligations are preserved as far as actual resources permit;
- TARC may rebalance inside the admitted allocation;
- extra resource need goes only through TARC after the future runtime request boundary exists;
- Guardian may change Trading protection scope based on evidence but cannot request Foundation resources;
- unknown resource state is not treated as abundant capacity.

---

## 25. Staged Recovery / Restoration

Restoration SHALL avoid oscillation and thundering-herd behavior.

Conceptual sequence:

```text
PRESSURE / SHEDDING
 -> RESOURCE / DEPENDENCY RECOVERY EVIDENCE
 -> TARC BOUNDED INTERNAL RESTORATION
 -> LOWER-LANE RE-ADMISSION IN STAGES
 -> OBSERVE TAIL LATENCY / QUEUES / ERRORS / HEADROOM
 -> NORMALIZE ONLY WITH EVIDENCE
```

Restoration considers:

- queue depth;
- p95/p99/tail latency;
- error rate;
- provider/broker limits;
- resource headroom;
- Guardian/Risk state;
- stale/expired backlog;
- TARC requester epoch/split-brain state;
- current Foundation decisions.

Stale backlog is not blindly replayed when capacity returns.

---

## 26. Performance Evidence

Validation SHALL measure where applicable:

- throughput;
- queue depth;
- wait time;
- p50/p95/p99/tail latency;
- deadline miss rate;
- shed/drop/coalesce rate;
- backpressure duration;
- protection/reconciliation starvation;
- resource headroom;
- retry amplification;
- restoration time;
- stale-work invalidation effectiveness.

Average latency alone is insufficient for safety-relevant paths.

---

## 27. Foundation / FCR Dependencies

Current relevant Foundation/FCR state:

- Stage 6 WP-01 `ACCEPTED_AND_CLOSED`;
- Stage 6 WP-02 `ACCEPTED_AND_CLOSED`;
- Stage 6 WP-03 `ACCEPTED_AND_CLOSED`;
- Stage 6 WP-04 `ACCEPTED_AND_CLOSED` within exact priority/technical-criticality scope;
- Stage 6 WP-05 through WP-10 `NOT_AUTHORIZED`;
- FCR-0007 resource request/decision runtime `OPEN / Waiting On FOUNDATION`;
- FCR-0009 complete latency/deadline/QoS transport `OPEN / MISSING / Waiting On FOUNDATION`;
- FCR-0010 complete pressure/enforcement/load-shedding/restoration/request runtime `OPEN / PARTIAL / Waiting On FOUNDATION`.

Accepted Stage 5 transport flow-control/priority primitives may be used only within their exact scopes. P0-J does not claim complete cross-Application Fast Track/QoS runtime from them.

---

## 28. Explicit Non-Authority

P0-J SHALL NOT:

- turn business lane into Foundation priority/criticality;
- turn TARC tier into Foundation technical criticality;
- let Fast Track bypass Risk/Guardian/authority;
- let overload create permission;
- let queue residence extend validity;
- let callers self-declare technical criticality;
- let Guardian bypass TARC for resources;
- treat shedding request as proven capacity release;
- treat WP-04 closure as later Stage 6 runtime authorization;
- treat average latency as tail-latency proof.

---

## 29. Invariants

```text
PERFORMANCE != AUTHORITY
FAST_TRACK != FEWER_SAFETY_GATES
BUSINESS_LANE != TARC_RESOURCE_TIER
TARC_RESOURCE_TIER != FOUNDATION_TECHNICAL_CRITICALITY
APPLICATION_RESOURCE_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY
TRADING_HIGH_APPLICATION_PRIORITY != FOUNDATION_FLOOR_OVERRIDE
CALLER_PRIORITY = NON_AUTHORITATIVE_EVIDENCE
END_TO_END_DEADLINE != RESETTABLE_PER_HOP_BUDGET
CACHE_READ_TIME != SOURCE_FRESHNESS
SHED_REQUESTED != SHED_EFFECTIVE
REQUESTED_RESOURCE != GRANTED_RESOURCE
TARC = SOLE_TRADING_FOUNDATION_RESOURCE_REQUEST_ROLE
STAGE6_WP04_ACCEPTED != FULL_RESOURCE_RUNTIME
UNBOUNDED_QUEUE = PROHIBITED_FOR_MATERIAL_LATENCY_SENSITIVE_WORK
```

---

## 30. Forbidden Interpretations

Invalid interpretations include:

- “Trading has highest Application resource priority, so it outranks Foundation survival controls”;
- “WP-04 closed, so TARC resource request/load-shedding runtime exists”;
- “Protection lane is automatically Foundation-critical”;
- “TARC high tier means Foundation must grant resources”;
- “Guardian urgency mints Foundation technical criticality”;
- “Fast Track skips normal controls”;
- “deadline resets after each hop”;
- “queued work remains valid”;
- “shedding was requested, therefore capacity was freed”;
- “capacity returned, therefore replay the backlog”;
- “average latency passed, therefore tail latency is safe”.

---

## 31. Mandatory Scenarios

At minimum challenge:

- deadline expiry while queued;
- per-hop deadline reset attempt;
- stale Data Product with low transport latency;
- lower lane holding a resource needed by protection/reconciliation;
- discovery flood during open-position management;
- protection/reconciliation starvation attempt;
- caller/TARC tier inflation;
- business lane used to mint Foundation criticality;
- Trading high Application priority used to override Foundation floor;
- Guardian direct Foundation resource request attempt;
- WP-04 closure used to claim pressure/load-shedding/request runtime;
- TARC split-brain/stale requester identity;
- shed request with no effective capacity release;
- provider quota bottleneck despite free CPU;
- broker serialization bottleneck despite more workers;
- average latency pass with p99 failure;
- precomputed Risk/control evidence invalidated before dispatch;
- restoration thundering herd;
- stale backlog after recovery.

---

## 32. Exit Gates

```text
UNBOUNDED_MATERIAL_QUEUES = 0
DEADLINE_RESET_PATHS = 0
FAST_TRACK_CONTROL_BYPASSES = 0
BUSINESS_TO_FOUNDATION_PRIORITY_LAUNDERING = 0
APPLICATION_PRIORITY_TECHNICAL_CRITICALITY_CONFLATION = 0
FOUNDATION_FLOOR_OVERRIDE_BY_TRADING_PRIORITY = 0
WP04_LATER_RUNTIME_OVERCLAIM = 0
CALLER_SELF_PRIORITY_AUTHORITY = 0
PROTECTION_RECONCILIATION_STARVATION_PATHS = 0
SHED_EFFECT_ASSUMPTION_PATHS = 0
STALE_BACKLOG_BLIND_REPLAY_PATHS = 0
TARC_ALTERNATE_REQUESTER_PATHS = 0
COHERENCY_INVALIDATION_MODEL = COMPLETE
TAIL_LATENCY_EVIDENCE_MODEL = COMPLETE
FCR0009_STATE = EXPLICIT
FCR0010_STATE = EXPLICIT
```

---

## 33. Next Authorized Gate

P0-J acceptance would establish performance/resource/QoS/resilience design only. It would not authorize Stage 6 WP-05+, pressure/preemption/load-shedding/resource-request/rebalance/restoration runtime, Paper, Tiny Live, Live, or deployment.
