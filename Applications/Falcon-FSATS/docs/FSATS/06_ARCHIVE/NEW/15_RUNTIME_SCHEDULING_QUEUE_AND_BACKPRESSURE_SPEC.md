# FSATS Specialized Implementation Architecture — Runtime, Scheduling, Queue and Backpressure Specification

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`

## 1. Purpose

Define the runtime execution model for each Application so coding workers do not invent arbitrary timers, unbounded queues, thread pools, retry storms or hidden priority behavior.

## 2. Runtime Model

Candidate baseline:

- one independently governable .NET host per Application;
- LSA modules execute in-process behind explicit ports/queues;
- cross-Application traffic uses governed Foundation transport only;
- CPU-bound and I/O-bound work are separated into bounded worker pools;
- no `Task.Run` fire-and-forget for material business work;
- no unbounded `Channel`, queue, collection or retry loop in production runtime.

## 3. Work Item Identity

Every scheduled/queued material work item carries:

```text
WorkItemId
ApplicationId
OwningLSA
WorkClass
SubjectKey
BusinessPriorityClass
EffectiveDeadline?
Correlation/Causation refs
IdempotencyKey?
InputSnapshotRefs[]
ConfigurationVersion
EnqueuedAtRef
AttemptNumber
```

Queue priority is business scheduling evidence only. It does not create authority or Foundation technical criticality.

## 4. Canonical Work Classes

```text
PROTECTION_EFFECT
ORDER_POSITION_RECONCILIATION
CAPITAL_RECONCILIATION
CRITICAL_DATA_PATH
EXECUTION_SUBMISSION
ACTIVE_DECISION
PROVIDER_DELIVERY
RESOURCE_COORDINATION
MARKET_SCAN_DISCOVERY
FEATURE_COMPUTE
ANALYTICS
LEARNING
EXPERIMENT
SIMULATION
RESEARCH
MAINTENANCE
```

Default scheduling order within one Application is consequence-aware, not pure FIFO. Hard safety/protection/reconciliation work may bypass deferrable work but cannot bypass authority/precondition checks.

## 5. Queue Classes

Each Application host has separate bounded queue lanes rather than one global queue:

```text
Q0_PROTECTION_AND_RECONCILIATION
Q1_CRITICAL_OPERATIONAL
Q2_NORMAL_OPERATIONAL
Q3_DISCOVERY_ANALYTICS
Q4_EXPERIMENT_SIMULATION_RESEARCH
```

Queue lane is selected from work class by a versioned local scheduling policy.

No Application may mark arbitrary work as Q0 through caller input alone.

## 6. Capacity Rule

Queue capacities are configuration values, but every lane SHALL have:

- finite maximum items;
- finite maximum estimated work units/bytes where applicable;
- explicit full-queue behavior;
- metrics for depth, age and rejection;
- cancellation/drain behavior;
- FSARM/T-LSA resource degradation mapping.

A missing queue-capacity configuration is startup validation failure.

## 7. Full-Queue Behavior

### Q0 Protection/Reconciliation

No silent drop. If saturated:

1. reject/defer lower-lane intake;
2. attempt bounded reserved worker capacity;
3. shed declared deferrable work;
4. raise Guardian/resource pressure evidence;
5. if still unable, mark affected protection/reconciliation path unavailable and fail safe.

### Q1 Critical Operational

No silent drop. Apply backpressure to producer if possible; otherwise explicit `OVERLOADED/UNAVAILABLE` response and preserve safety impact evidence.

### Q2 Normal Operational

Bounded backpressure. May reject new non-obligatory work with `OVERLOADED_RETRY_LATER` where caller contract supports retry.

### Q3 Discovery/Analytics

May coalesce/supersede stale work, reduce breadth/frequency or drop not-yet-started work with explicit metrics/evidence.

### Q4 Experiment/Simulation/Research

May pause/cancel/checkpoint according to declared reclaimability.

## 8. No Silent Queue Drop

Every dropped/coalesced/canceled material work item receives an explicit terminal disposition:

```text
COMPLETED
SUPERSEDED
COALESCED
REJECTED_OVERLOAD
CANCELED_RESOURCE_PRESSURE
CANCELED_SHUTDOWN
EXPIRED_BEFORE_EXECUTION
FAILED
```

## 9. Deadline Semantics

A business deadline affects local scheduling and expiry only.

It SHALL NOT be represented as transport QoS guarantee until FCR-0009 capability exists.

If a work item starts after its decision/data validity deadline, the worker revalidates preconditions; it cannot execute stale action merely because it finally reached the front of the queue.

## 10. Worker Pools

Candidate baseline per Application:

- `CriticalCoordinatorPool`: small reserved concurrency for Q0/Q1, never consumed by Q4;
- `OperationalIoPool`: provider/broker/transport I/O bounded concurrency;
- `ComputePool`: feature/strategy/analysis CPU-bound work bounded by Application resource profile;
- `BackgroundPool`: analytics/learning/maintenance;
- FSTSimA additionally `SimulationPool` with FSARM-reclaimable concurrency.

Exact maximum concurrency is configuration/profile-driven and cannot exceed current effective resource coordination state.

## 11. Scheduling Fairness

High-priority work cannot permanently starve all lower work when capacity is healthy.

Candidate weighted service rule after Q0 urgent obligations:

```text
Q0: drain required safety/reconciliation obligations subject to loop budget
Q1: weight 8
Q2: weight 4
Q3: weight 2
Q4: weight 1
```

Weights operate only when queues are non-empty and resources permit. Under critical pressure, FSARM/local degradation may set Q3/Q4 service weight to zero.

A Q0 loop budget prevents malformed recurring high-priority work from monopolizing a worker forever; repeated self-generating Q0 work triggers integrity/overload evidence.

## 12. Coalescing Rules

Coalescing is allowed only for work where only latest state matters.

Allowed examples:

- market/universe refresh request: keep newest same-key refresh not started;
- analytics dashboard projection refresh;
- provider reliability recompute;
- resource demand forecast recompute.

Forbidden coalescing:

- broker order attempts/fills;
- capital reservation events;
- Guardian directives/outcomes;
- resource coordination actions/effect confirmations;
- audit/evidence events;
- state-machine transitions requiring each event for reconstruction.

## 13. Polling / Timer Rule

Every recurring task uses a named schedule profile:

```text
ScheduleProfileId
OwnerLSA
TriggerMode = EVENT_DRIVEN | FIXED_INTERVAL | CRON_LIKE | MARKET_PHASE_AWARE | ADAPTIVE_BOUNDED
MinimumInterval
MaximumInterval
JitterRule
BackoffRule
ResourcePressureRule
FreshnessObjective
```

No hardcoded arbitrary timer in business component code.

Adaptive schedules stay within the versioned min/max envelope.

## 14. Initial Scheduling Profiles

These are semantic profiles; exact milliseconds/minutes remain environment/config values subject to validation unless explicitly bound later by market/provider rules.

### Market Universe Scan

- market-phase-aware;
- event-driven immediate rescan on material instrument/tradability/corporate-action changes;
- regular refresh cadence configurable;
- under pressure reduce breadth/frequency before safety-critical state.

### Feature Computation

- event-driven on new required Data Product/window completion where practical;
- coalesce superseded feature requests by `(FeatureId, Subject, DecisionBoundary)`;
- expired decision-bound feature work is canceled.

### Provider Health/Quota

- event-driven on failures/quota responses;
- bounded periodic reconciliation;
- increased frequency during degradation only within quota/resource budget.

### Broker Reconciliation

- immediate event-driven after ambiguous/active order events;
- bounded periodic reconciliation for open orders/positions;
- may not be shed below minimum-safe while active financial obligations exist.

### Guardian

- event-driven protection signals;
- periodic incident/directive expiry/recovery checks;
- reserved Q0 capacity.

### FSARM

- event-driven on Foundation resource epoch/pressure/outcome or Application demand report change;
- bounded periodic consistency refresh as safety net;
- no plan recomputation storm from semantically identical report duplicates.

### FSTSimA

- explicit run queue;
- checkpoint/pause at deterministic safe boundaries when resource reclaimed.

## 15. Backoff Rules

External/provider/broker/transport retry uses bounded exponential backoff only where retry is semantically safe.

A generic candidate sequence may be configured as:

```text
base delay
x 2^(attempt-1)
with bounded jitter
capped at max delay
max attempts / terminal deadline
```

But retry eligibility comes from the owning contract/state machine.

Forbidden generic retry:

- ambiguous broker order submission;
- non-idempotent protection/resource command without idempotency identity;
- authority/security rejection;
- schema incompatibility;
- invalid business precondition;
- stale/superseded command.

## 16. Circuit Breaker Semantics

Provider/broker/Foundation adapter may use a local circuit state:

```text
CLOSED
OPEN
HALF_OPEN
```

Circuit breaking is technical availability behavior only. It must publish explicit dependency degradation state and cannot convert a blocked operation into business success.

Circuit thresholds/profile are versioned/configured. Security/authority failures use immediate fail-closed handling, not normal transient breaker retries.

## 17. Bulkheads

Separate concurrency semaphores/bulkheads are mandatory where one dependency could exhaust all Application workers.

At minimum:

- per provider route/account;
- per broker route/account;
- Foundation transport publishing;
- heavy feature/model evaluation;
- simulation runs;
- external research when future capability exists.

Bulkhead limits are coordinated with current resource profile.

## 18. Trading Hot Path

The latency-sensitive Trading decision path prioritizes:

```text
DataProduct receive
-> required incremental feature update
-> active strategy evaluations
-> T06 orchestration
-> T07 Risk
-> T08 reservation
-> T09 intent/submission
```

Rules:

- analytics/learning are not synchronous dependencies;
- evidence/outbox persistence required for high-consequence state remains synchronous where invariants require it;
- expensive strategy evaluation runs only for qualified candidates;
- feature calculation reuses immutable cache where exact input/version match;
- no network call to another Application inside an internal Trading database transaction.

## 19. Cancellation

Cancellation token propagation is required for cancellable work, but cancellation cannot leave authoritative state half-transitioned.

A material operation checks cancellation before starting a DB transaction/network dispatch. Once a commit-critical section begins, it completes or rolls back; later cancellation becomes a follow-up state-machine command if needed.

## 20. Shutdown

Graceful shutdown sequence per Application:

1. stop accepting new nonessential work;
2. stop new risk-increasing operations;
3. preserve/finish Q0 reconciliation/protection work within governed shutdown window;
4. drain or persist outbox/inbox state as designed;
5. persist checkpoints for eligible long-running work;
6. mark incomplete external operations AMBIGUOUS/RECONCILIATION_REQUIRED where needed;
7. stop workers;
8. emit final health/evidence state.

Forced shutdown must still be recoverable through durable state on restart.

## 21. Startup Recovery

Before Application becomes business-ready:

- validate configuration/schema versions;
- establish Foundation dependency availability;
- recover aggregate/outbox/inbox state;
- reconcile ambiguous external operations;
- revalidate Guardian restrictions;
- revalidate resource effective state;
- warm only required caches/features;
- verify market/account/provider/broker current truth;
- then publish readiness.

Startup process existence != readiness.

## 22. Resource Pressure Integration

Each queue/worker pool exposes:

- current depth;
- oldest age;
- active concurrency;
- saturation ratio;
- rejected/coalesced count;
- estimated work units;
- protected minimum concurrency;
- reclaimable concurrency;
- degradation options.

Application local resource reporter maps this into FSARM demand evidence.

A Foundation pressure/load-shedding projection is consumed through the accepted boundary and cannot be ignored by local scheduling.

## 23. Overload Fail-Safe Invariants

Under overload:

- do not weaken Risk/Guardian checks;
- do not skip persistence needed for order/capital correctness;
- do not accept stale data to maintain throughput;
- do not blind-retry broker/provider operations;
- do not drop evidence required for reconstruction;
- reduce new work before existing financial obligation safety;
- expose degradation truth.

## 24. Metrics Required

Per queue/worker:

```text
queue_depth
queue_oldest_age
work_enqueued_total
work_completed_total
work_rejected_total
work_coalesced_total
work_expired_total
work_duration
active_workers
saturation_ratio
retry_total
circuit_state
bulkhead_rejection_total
```

Every metric is tagged by Application/LSA/work class with bounded cardinality. InstrumentId/OrderId are not default metric labels.

## 25. Verification Families

Tests/verifier SHALL cover:

1. every queue bounded;
2. full-queue behavior by lane;
3. no silent drop;
4. Q0 reserved capacity;
5. no caller self-promotes queue class;
6. Q4 starvation/shedding under pressure;
7. fairness when healthy;
8. coalescing only allowed classes;
9. no coalescing financial/protection/resource history;
10. stale-deadline work revalidation;
11. generic retry forbidden for ambiguous broker submission;
12. backoff bounded;
13. circuit breaker truth/degradation;
14. bulkhead isolation;
15. shutdown/restart ambiguous-operation recovery;
16. outbox persists through shutdown;
17. resource-pressure changes concurrency without violating minimum-safe;
18. Trading hot path has no analytics synchronous dependency;
19. overload never disables Risk/Guardian/persistence invariants;
20. deterministic scheduling order where input ordering/priority are equal.
