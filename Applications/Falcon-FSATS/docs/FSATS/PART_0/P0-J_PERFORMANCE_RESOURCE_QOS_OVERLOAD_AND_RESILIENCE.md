# P0-J - Performance, Resource, QoS, Overload and Resilience

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-J defines how FSATS preserves low latency, throughput, protection continuity, truth reconstruction and controlled degradation without bypassing authority. It integrates the later Owner-accepted APP-RSC fifth-Application architecture and supersedes the historical Trading/TARC FSATS-wide resource-control model.

Performance is a quality of correct governed behavior, never permission to skip a required control.

## 2. Ownership hierarchy

```text
FOUNDATION RESOURCE GOVERNANCE
= Falcon-wide authoritative total-resource truth, grants, ceilings, floors, reserves, reclaim/revoke and Foundation technical criticality

APP-RSC
= FSATS-only resource coordination Application

CONSTITUENT APPLICATIONS
= own internal workload meaning, internal safe shedding order and attributable demand/evidence
```

```text
APP_RSC != FOUNDATION_RESOURCE_GOVERNANCE
APP_RSC != FSATS_CONTAINER
APP_RSC_COORDINATION != FOUNDATION_RESOURCE_GRANT
```

## 3. APP-RSC topology

APP-RSC is the fifth independent Falcon Application:

```text
APPLICATION_ID = APP-RSC
MSA = 1
LSA = 3
CSA = 0 initially
SCOPE = FSATS_ONLY
```

Canonical LSA names from the accepted Part 1 code-ready decomposition:

1. `R-LSA-01 Resource Picture, Demand Integrity & Coordination Envelope`
2. `R-LSA-02 Redistribution, Degradation & Rebalance`
3. `R-LSA-03 Foundation Binding, Restoration & Resource Evidence`

## 4. R-LSA-01 Resource Picture, Demand Integrity & Coordination Envelope

Owns the current FSATS coordination picture derived from separately attributable constituent evidence and current Foundation-authoritative resource outcomes. It never replaces Foundation truth.

Material claims preserve Application identity, resource class, current allocation/consumption, minimum-safe requirement, desired capacity, pressure/urgency evidence, reclaimability, degradation consequence, freshness/confidence and current Foundation envelope reference.

```text
APPLICATION_REPORTED_NEED != PROVEN_RESIDUAL_NEED
APP_RSC_EFFECTIVE_PICTURE != FOUNDATION_AUTHORITATIVE_TRUTH
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

Anti-gaming checks include contradictory minimum-safe claims, urgency inflation, repeated non-reclaimable classification without evidence, pressure claims inconsistent with observed use, hidden retained capacity, churn designed to bias priority and attempts to classify business desire as survival floor.

Unknown claim integrity reduces aggressive redistribution eligibility rather than increasing it.

## 5. R-LSA-02 Redistribution, Degradation & Rebalance

Owns bounded coordination within the currently valid Foundation envelope. Operational components may include `ResourceStrategyController`, `RedistributionPlanner`, `ReclaimPlanner`, `DegradationCoordinator`, `RebalanceExecutor`, `RestorationPlanner`, `OscillationGuard` and `StarvationGuard`.

`ResourceStrategyController` is operational control, not the APP-RSC MSA or an Awareness tier.

Prime sequence:

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
```

APP-RSC evaluates active obligation, starvation consequence, minimum-safe need, reclaimability, current pressure, Guardian protection state, checkpoint cost and restoration cost rather than using a permanent Application rank.

It cannot rewrite Foundation criticality, seize protected minima or treat profitability alone as survival priority.

## 6. R-LSA-03 Foundation Binding, Restoration & Resource Evidence

Owns the exact Application-side binding between current Foundation-authoritative envelope/outcomes and APP-RSC coordination. Additional-resource requests are assembled only after safe internal optimization and proven residual deficit.

Foundation outcomes remain distinguishable as supplied by the authoritative boundary, such as grant, partial, cap, deny, reduce, revoke, reclaim, rebalance or restore.

APP-RSC may consume these outcomes but cannot manufacture them.

Final canonical runtime binding remains pending the Foundation Stage 14 canonical consumption mechanism tracked by FCR-0016/FCR-0031.

## 7. Constituent interfaces

Trading, FSAPMA, Guardian and FSTSimA publish separately attributable Application-owned resource evidence. APP-RSC consumes that evidence and returns governed effective coordination outcomes. It does not read peer internals or business state directly.

Required evidence fields include as applicable:

```text
CurrentAllocationReference
CurrentConsumption
MinimumSafeRequirement
DesiredCapacity
Pressure/UrgencyEvidence
ReclaimableCapacity
DegradationOptions
ConsequenceOfStarvation
Checkpoint/RecoveryCost
RestorationNeed
EvidenceFreshness
```

Constituent Applications own business meaning and safe internal shedding order.

## 8. Trading T-LSA-13 correction

Trading T-LSA-13 is Trading-local resource awareness/evaluation only. It can measure Trading workload demand, pressure, internal safe degradation and effectiveness and publish evidence to APP-RSC.

```text
T_LSA_13 != APP_RSC
T_LSA_13 != FOUNDATION_RESOURCE_GOVERNANCE
TRADING_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
```

Historical TARC requester singularity is superseded by APP-RSC for current FSATS resource coordination.

## 9. Multi-dimensional Work Identity

Every material work item carries enough identity to determine purpose, scope, deadline, dependencies, invalidation and resource treatment. Where applicable:

- Application identity;
- BrokerId/BrokerAccountId/environment where business material;
- market/instrument;
- business lane;
- action/purpose class;
- correlation/causation;
- source/decision/control epochs;
- original end-to-end deadline;
- data/decision freshness;
- Risk/Guardian/control versions;
- resource-policy evidence;
- replay/test/operational classification.

Work Identity describes and governs work. It creates no authority.

## 10. Canonical Trading business lanes

Initial lanes:

1. `PROTECTION`
2. `RECONCILIATION`
3. `OPEN_POSITION_MANAGEMENT`
4. `NEAR_TRADE`
5. `ACTIVE_WATCH`
6. `CANDIDATE_EVALUATION`
7. `DISCOVERY`
8. `BACKGROUND`

```text
BUSINESS_LANE != APP_RSC_COORDINATION_PRIORITY_EVIDENCE
BUSINESS_LANE != FOUNDATION_APPLICATION_PRIORITY
BUSINESS_LANE != FOUNDATION_TECHNICAL_CRITICALITY
```

A producer cannot self-promote its lane merely to obtain resources.

## 11. Lane meaning

`PROTECTION` preserves capital/protection obligations. `RECONCILIATION` reconstructs authoritative broker/order/position truth. `OPEN_POSITION_MANAGEMENT` manages existing exposure. `NEAR_TRADE` covers time-sensitive already-promoted candidates but keeps all safety gates. `ACTIVE_WATCH` is admitted monitoring. `CANDIDATE_EVALUATION` is pre-execution analysis. `DISCOVERY` is broader scanning. `BACKGROUND` is deferrable learning/analytics/maintenance/research-support work.

## 12. Original end-to-end deadline

Latency-sensitive work preserves its original validity budget across hops where applicable.

```text
END_TO_END_DEADLINE = ORIGINAL_BUDGET
PER_HOP_TIMEOUT_RESET = PROHIBITED_IF_IT_EXTENDS_ORIGINAL_VALIDITY
```

Deadline classes may include opportunity, protection, reconciliation, Data Product freshness, Owner-command expiry and provider/broker interaction deadline.

Deadline expiry does not authorize retry.

## 13. Freshness versus latency

```text
LOW_TRANSPORT_LATENCY != FRESH_SOURCE_TRUTH
FRESH_DATA != VALID_ACTION_DEADLINE
CACHE_READ_TIME != SOURCE_FRESHNESS
```

Both freshness and action deadline are checked when material.

## 14. Bounded queues

Every material queue defines accountable owner, purpose, bounded capacity, ordering, deadline/expiry behavior, control-epoch invalidation, overload behavior, shedding, recovery and evidence/metrics.

Unbounded safety-relevant or latency-sensitive queues are prohibited. Queue presence does not preserve expired authority.

## 15. Bounded concurrency

Concurrency respects actual bottlenecks including admitted resources, provider/broker quotas/sessions, broker-account/market limits, CPU/memory/network/storage, serialization/order requirements, idempotency, protected work and dependency availability.

More workers cannot create throughput beyond a serialized or external quota bottleneck.

## 16. Backpressure

When consumers/dependencies cannot keep up, pressure is propagated truthfully rather than hidden in unbounded queues. Responses may include producer slowing, reducing discovery/candidate intake, coalescing replaceable observations, rejecting expired work, shedding eligible lower-value work, bounded degraded mode and APP-RSC coordination.

Backpressure cannot silently discard mandatory protection/reconciliation truth.

## 17. Load shedding

Each Application sheds/degrades its own eligible work in the safest order under its business rules. A typical Trading order is:

```text
BACKGROUND
-> DISCOVERY
-> CANDIDATE_EVALUATION
-> ACTIVE_WATCH REDUCTION / COALESCING
-> NEAR_TRADE ONLY WHEN NO LONGER VALID / ELIGIBLE
```

Protection, reconciliation and required open-position management are preserved as far as actual resources permit.

```text
SHED_REQUESTED != SHED_EFFECTIVE
SHED_OPTIONAL_WORK != DROP_AUTHORITATIVE_TRUTH
```

APP-RSC coordinates resource outcomes but does not execute hidden business actions inside another Application.

## 18. Foundation Application priority vs technical criticality

```text
APPLICATION_RESOURCE_PRIORITY != FOUNDATION_TECHNICAL_CRITICALITY
BUSINESS_URGENT != FOUNDATION_CRITICAL
```

Foundation survival/protection/control capacity, non-reclaimable reserves, Foundation authority, health/recovery and security/evidence-integrity capacity remain protected according to Foundation governance.

No Trading lane, Guardian urgency or APP-RSC priority evidence may self-create Foundation technical criticality or override a protected Foundation floor.

## 19. Resource truth separation

FSATS distinguishes:

- Foundation authoritative total-resource truth;
- Foundation Application envelope/grant/ceiling/floor;
- APP-RSC effective coordination picture;
- constituent current use;
- constituent reservations;
- reported need;
- proven residual need;
- pressure evidence;
- shed request;
- shed effect;
- APP-RSC residual request;
- Foundation decision/outcome;
- restoration eligibility.

```text
INTERNAL_NEED != FOUNDATION_PRESSURE_TRUTH
REQUESTED_RESOURCE != PROVEN_RESIDUAL_NEED != GRANTED_RESOURCE
```

## 20. Coordination epoch and split-brain fencing

Exactly one valid APP-RSC coordination epoch may govern a coordination decision at a time. Every action binds APP-RSC identity, coordinator instance/epoch, input evidence versions/freshness, Foundation envelope identity/version, policy version, decision identity, target Application, effective action, causation/correlation and expiry/lease where applicable.

Stale, duplicate, conflicting or revoked epochs are fenced. Restart cannot silently resume old authority.

## 21. Coherency Vector

Material multi-source work identifies relevant versions/epochs, potentially including Data Product, Market Profile, strategy/model, Risk decision, capital reservation, Guardian directive, Owner control, broker/account capability and resource state.

A material dependency change invalidates or triggers revalidation of affected work.

## 22. Precomputation and invalidation

Stable expensive work may be precomputed only with declared dependency set, version/freshness, invalidation triggers, permitted scope and fallback behavior.

```text
FAST_CACHE != PERMISSION_TO_IGNORE_CHANGED_DEPENDENCY
PRECOMPUTE_STABLE_WORK + REVALIDATE_MATERIAL_MUTABLE_GATES_LATE = LOW_LATENCY_WITHOUT_STALE_AUTHORITY
```

## 23. Dispatch-time revalidation

Performance optimization cannot remove late revalidation of current Guardian/Owner controls, Risk validity, capital reservation, broker/account capability, market/session eligibility, critical Data Product freshness or other material mutable gates.

## 24. Priority inversion

Controls may include bounded critical sections, lock avoidance/partitioning, admission limits, queue separation, cancellation of eligible lower-value work, reserved internal capacity and technically safe scheduling mechanisms that do not create business authority.

Preemption is used only if separately supported/authorized and must not corrupt state or external API semantics.

## 25. Near-Trade Fast Track

```text
FAST_TRACK = LESS_AVOIDABLE_LATENCY
FAST_TRACK != FEWER_REQUIRED_GATES
FAST_TRACK != EMERGENCY_AUTHORITY
```

Near-Trade cannot skip Risk, capital reservation, Guardian/Owner controls, broker capability checks, pre-dispatch safety or dispatch-time revalidation.

## 26. Telemetry versus exact evidence

Metrics may aggregate/sample/bucket to control operational cost, while material audit/decision evidence remains reconstructable where required.

```text
METRIC_AGGREGATION != AUDIT_EVIDENCE_DELETION
```

## 27. Overload behavior

Under overload:

- reject expired work;
- shed eligible lower-value work first;
- restrict new exposure when gates cannot complete safely/in time;
- preserve protection/reconciliation/open-position obligations as actual resources permit;
- coordinate resources through APP-RSC;
- if valid internal coordination is insufficient, APP-RSC may submit proven residual need only after the Foundation runtime boundary exists and is authorized;
- Guardian may alter protection scope based on evidence but cannot request Foundation resources;
- unknown resource state is not abundant capacity.

## 28. Staged recovery/restoration

```text
PRESSURE / SHEDDING
-> RESOURCE / DEPENDENCY RECOVERY EVIDENCE
-> APP_RSC / CONSTITUENT BOUNDED RESTORATION
-> LOWER-LANE RE_ADMISSION IN STAGES
-> OBSERVE TAIL LATENCY / QUEUES / ERRORS / HEADROOM
-> NORMALIZE ONLY_WITH_EVIDENCE
```

Consider queue depth, p95/p99/tail latency, errors, external limits, resource headroom, Guardian/Risk state, expired backlog, APP-RSC epoch and current Foundation decisions.

Stale backlog is not blindly replayed when capacity returns.

## 29. APP-RSC failure/degraded state

If APP-RSC is unavailable/untrusted:

- no new cross-Application redistribution is assumed;
- no sibling inherits coordination authority;
- constituents continue only within last valid Foundation/Application truth and their own safe degraded rules where permitted;
- stale/unknown Foundation envelope prevents new coordination;
- in-flight work remains fenced by epoch/causation;
- restoration waits for trustworthy state reconstruction and Controlled Revival where AI trust is involved.

APP-RSC AI failure does not necessarily kill deterministic trusted coordination/evidence functions, but new risk-increasing decisions requiring killed intelligence are denied.

## 30. Performance evidence

Measure where applicable throughput, queue depth, wait time, p50/p95/p99/tail latency, deadline misses, shed/drop/coalesce rate, pressure duration, protection/reconciliation starvation, resource headroom, retry amplification, restoration time and stale-work invalidation effectiveness.

Average latency alone is insufficient for safety-relevant paths.

## 31. Current Foundation/FCR boundary

Live FCR state controls. Relevant current dependencies include FCR-0009 QoS/deadline transport, FCR-0010 resource capability/canonical consumption gap, FCR-0016 Stage 14 artifact consumption and FCR-0031 APP-RSC canonical runtime binding. No P0-J text activates those runtimes.

## 32. Mandatory failures and adversarial scenarios

Test queue saturation; external quota bottleneck; retry amplification; expired backlog; stale cache; control epoch change; priority inflation; starvation; oscillation/thrashing; APP-RSC stale/duplicate/split-brain epoch; constituent stale report; false urgency; protected-minimum conflict; APP-RSC crash mid-redistribution; Foundation envelope revoked mid-action; deny/partial grant; staged restore; peer direct seizure; constituent bypass; APP-RSC non-FSATS control attempt; Foundation criticality minting attempt; Guardian crisis with reclaimable deferrable capacity; and canonical Foundation binding unavailable.

## 33. Invariants

```text
PERFORMANCE_OPTIMIZATION != AUTHORITY_BYPASS
LOW_LATENCY != LESS_VALIDATION
APP_RSC_IS_FIFTH_APPLICATION = YES
APP_RSC_SCOPE = FSATS_ONLY
APP_RSC_LSA_COUNT = 3
APP_RSC_CSA_COUNT = 0_INITIAL
APP_RSC_IS_FOUNDATION = NO
INTERNAL_REDISTRIBUTION_FIRST = YES
FOUNDATION_ADDITIONAL_REQUEST_SECOND = YES
BUSINESS_LANE != FOUNDATION_TECHNICAL_CRITICALITY
SHED_REQUESTED != SHED_EFFECTIVE
FAST_TRACK != FEWER_REQUIRED_GATES
TRADING_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
FINAL_CANONICAL_RUNTIME_BINDING = PENDING_FCR0016_FCR0031
```

## 34. Exit gate

P0-J is complete only when Work Identity, lanes, deadlines, freshness, queues, concurrency, backpressure, shedding, APP-RSC exact 3-LSA ownership, resource truth separation, fencing, coherency, fast path, overload and staged recovery are explicit and adversarially testable.

## 35. Non-grant

Acceptance of P0-J would establish performance/resource/QoS design only. It would not activate Foundation resource runtime binding, provider/broker connectivity, Paper, Shadow, Tiny-Live, Live or deployment.