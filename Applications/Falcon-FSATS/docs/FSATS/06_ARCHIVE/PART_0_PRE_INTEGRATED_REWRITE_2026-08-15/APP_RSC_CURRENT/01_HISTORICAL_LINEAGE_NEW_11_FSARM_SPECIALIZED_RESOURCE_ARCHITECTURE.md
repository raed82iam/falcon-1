# FSARM — Specialized Resource Architecture and Runtime Identity Decision Candidate

**Package:** `FSATS-SIA-v0.1`
**Status:** `MATERIAL DESIGN CANDIDATE / REQUIRES FRESH A-C + RED-TEAM + OWNER DECISION`
**Scope:** `FSATS ONLY`
**Implementation Authority:** `NOT_GRANTED`

## 1. Purpose

Resolve the runtime ownership/identity problem for FSARM while preserving:

- APP-001 independent Application governance;
- ADR-I012 no hidden cross-Application coupling or privileged Application;
- Foundation ownership of authoritative total-resource truth, grants and ceilings;
- FCR-0031 Owner-preserved requirement for real bounded cross-Application effective redistribution without a Foundation round-trip for every move inside the valid coordination envelope;
- constituent Application identity, attribution, protected minimums, accounting, isolation, fencing and reconstructability.

## 2. Problem Statement

FSARM must coordinate effective resource distribution across APP-TRD, APP-PMA, APP-GRD and APP-SIM.

A real coordinator therefore needs:

- stable identity;
- declared contracts/permissions;
- independent lifecycle;
- resource/evidence state ownership;
- failure containment;
- authority/revocation boundaries;
- replacement/recovery semantics;
- an exact Foundation-facing Application identity if it consumes Foundation resource contracts.

A documentation grouping named `FSATS` cannot safely own those runtime responsibilities because FSATS is currently a non-owning system boundary.

## 3. Alternatives Reviewed

### A. Place FSARM inside APP-TRD

**Rejected as target architecture.**

Problems:

- makes Trading the privileged coordinator over independent peer Applications;
- gives Trading operational resource influence over Guardian/FSAPMA/FSTSimA outside its business ownership;
- Trading failure can remove the coordinator precisely when Guardian protection needs resource movement;
- creates an architectural exception inconsistent with APP-001/ADR-I012 independence.

### B. Place FSARM inside APP-GRD

**Rejected.**

Guardian must remain protection/crisis authority, not generic FSATS resource management. Combining the roles creates excessive control concentration and makes every resource decision look like protection authority.

### C. Place FSARM inside APP-PMA or APP-SIM

**Rejected.**

Neither Application has legitimate ownership over peer Application resource coordination, and both create inappropriate privilege/failure coupling.

### D. Place FSARM as a hidden runtime service under the non-owning `FSATS` grouping

**Rejected.**

This would create an undeclared lifecycle/permission/resource/state principal outside APP-001 Application governance.

```text
NON_OWNING_GROUPING + STATEFUL_CROSS_APP_CONTROLLER
= HIDDEN_APPLICATION_IN_PRACTICE
```

### E. Distributed leader/election protocol across the four Applications

**Rejected for current design.**

It introduces split-brain/election/lease complexity, duplicates coordinator logic in peers, weakens single ownership and still needs a governed cross-Application coordination authority. No current requirement justifies this complexity.

### F. Dedicated FSATS-scoped Resource Management Application

**PROPOSED TARGET.**

The clean current-Falcon realization is a fifth independent Application:

```text
APP-RSC — Falcon Self-Aware Resource Management Application
Role name: FSARM
Scope: FSATS only
```

This is a **material topology delta** from the current accepted historical four-Application design. It is not accepted by being written here.

## 4. Proposed Topology Delta

If Owner-accepted after fresh review:

```text
FSATS non-owning domain grouping
├── APP-TRD  Falcon Self-Aware Trading Application
├── APP-PMA  Falcon Self-Aware Provider Management Application
├── APP-GRD  Falcon Trading Guardian Application
├── APP-SIM  Falcon Self-Aware Trading Simulation Application
└── APP-RSC  Falcon Self-Aware Resource Management Application (FSARM)
```

Proposed awareness topology for APP-RSC:

```text
MSA-RSC
├── R-LSA-01 Resource Picture, Demand Integrity & Coordination Envelope
├── R-LSA-02 Internal Redistribution, Degradation & Rebalance
└── R-LSA-03 Foundation Binding, Restoration & Resource Evidence
```

Proposed totals if accepted:

```text
APPLICATIONS = 5
MSA = 5
LSA = 34
FSATS BOUNDARY MSA/LSA = 0/0
```

The three-LSA decomposition is deliberately small. FSARM is a narrow coordinator, not another general management platform.

## 5. APP-RSC Purpose

Maintain and execute the bounded FSATS-wide **effective resource coordination** model across admitted FSATS Applications while preserving Foundation authoritative resource truth and exact constituent Application attribution.

It SHALL:

- consume Foundation resource truth/projections/contracts available to Applications;
- consume attributable resource demand/minimum/reclaimability/degradation evidence from constituent Applications;
- maintain the valid FSATS coordination envelope;
- perform bounded internal effective redistribution inside that envelope;
- request additional Foundation capacity only for proven remaining deficit;
- consume Foundation outcomes without treating requests as grants;
- coordinate staged degradation, restoration and reconciliation;
- preserve evidence for every resource decision and effect.

## 6. Explicit Non-Responsibilities

APP-RSC / FSARM SHALL NOT own:

- Foundation total-resource truth;
- Foundation grants/ceilings/protection floors/reserves;
- Foundation technical criticality;
- Application lifecycle authority;
- Guardian protection authority;
- Trading Risk, strategies, capital, portfolio or execution;
- provider/data business truth;
- simulation/validation truth;
- FSA/Owner governance;
- security/credential authority;
- Falcon-wide resource management outside FSATS.

```text
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
FSARM_SCOPE = FSATS
FSARM != GUARDIAN
FSARM != TRADING_RISK
```

## 7. Coordination Envelope

The `FSATSCoordinationEnvelope` is an immutable/versioned effective coordination boundary derived from current Foundation authoritative resource state plus constituent Application declarations/policy.

Minimum fields:

```text
EnvelopeId
EnvelopeVersion
FoundationResourceEpoch
FoundationGrant/AllocationRefs by Application and ResourceClass
FoundationProtectedFloor/ReserveRefs where exposed
ConstituentApplicationIds
PolicyVersion
EffectiveFrom
ExpiresAt/FreshnessRule
AllowedEffectiveRedistributionRules
PerApplicationHardCeilings
PerApplicationMinimumProtectedConstraints
AggregateFSATSCapacityByClass
EvidenceRefs
Digest
```

### Envelope invariants

1. Effective FSATS capacity cannot exceed the aggregate current Foundation-authorized capacity represented in the envelope.
2. FSARM cannot increase a constituent Foundation authoritative grant/ceiling by local bookkeeping.
3. Internal effective redistribution cannot violate a Foundation hard ceiling/floor/revocation/isolation state.
4. Constituent Application identity is never erased into an opaque pool.
5. Every effective transfer/reduction/restoration is reconstructable.
6. Stale/unknown/conflicted Foundation epoch or envelope invalidates new redistribution decisions.
7. An envelope may be narrower than what Foundation technically grants; it cannot be broader.

## 8. Resource Classes

Initial canonical technical classes are declared by mapping to the current Foundation resource contract rather than invented as replacements. Application business workload categories are separate evidence.

FSARM policy operates over exact Foundation resource-class identities plus business workload descriptors such as:

```text
COMPUTE
MEMORY
STORAGE_IO
NETWORK_IO
FOUNDATION-EXPOSED EXECUTION/SCHEDULING CAPACITY IF GOVERNED
```

The exact Foundation class identity is authoritative. The labels above are design categories only until bound to accepted Foundation contracts.

## 9. R-LSA-01 — Resource Picture, Demand Integrity & Coordination Envelope

### Components

- `R01.FoundationResourceProjectionConsumer`
- `R01.ApplicationResourceReportRegistry`
- `R01.ReportFreshnessValidator`
- `R01.MinimumSafeConstraintValidator`
- `R01.CoordinationEnvelopeBuilder`
- `R01.ResourcePictureAggregate`
- `R01.DemandConflictDetector`
- `R01.ResourceDemandForecastModel` (optional intelligent component)

### Inputs

From each constituent Application:

```text
ResourceDemandReportId
ApplicationId
ResourceClass
CurrentEffectiveAllocationRef
MeasuredUsage
MinimumSafe
DesiredNormal
MaximumUseful
ReclaimableNow
ReclaimabilityClass
CheckpointCost/Requirement
DegradationOptions[]
ConsequenceOfStarvation
ActiveObligationRefs[]
Freshness
EvidenceRefs
```

A report is evidence/need, not resource authority.

### Resource picture

One immutable `ResourcePictureSnapshot` pins:

- exact valid Foundation resource epoch/projection;
- exact coordination envelope;
- latest valid report per Application/resource class;
- active Guardian protection/crisis evidence;
- outstanding FSARM plans/effects;
- pending Foundation request/outcome state.

### Minimum-safe validation

Constituent Applications cannot self-assert arbitrary `SURVIVAL_MINIMUM` to capture capacity.

A minimum-safe claim must bind:

- declared workload/obligation class;
- current active obligation evidence;
- versioned minimum policy/profile;
- measured or justified requirement;
- consequence evidence.

Invalid/unattributable minimum claims are rejected from the protected-minimum calculation and surfaced as integrity/configuration findings.

### Conflict detection

If aggregate valid minimum-safe requirements exceed available effective capacity, R-LSA-01 emits `MINIMUMS_EXCEED_CAPACITY` and hands the snapshot to R-LSA-02 for consequence-aware degradation plus R-LSA-03 for proven remaining-deficit request. It does not fabricate full satisfaction.

### CSA

Resource-demand forecast model may be CSA-eligible. Envelope construction, minimum validation and Foundation truth consumption remain deterministic.

## 10. R-LSA-02 — Internal Redistribution, Degradation & Rebalance

### Components

- `R02.ResourcePlanBuilder`
- `R02.ObligationPriorityResolver`
- `R02.ReclaimabilityResolver`
- `R02.DegradationOptionSelector`
- `R02.RedistributionEngine`
- `R02.ResourcePlanAggregate`
- `R02.EffectTracker`
- `R02.SplitBrainFenceValidator`

### Prime algorithm

For each resource class and exact resource picture snapshot:

```text
1. Preserve Foundation hard constraints from envelope.
2. Determine each Application's currently justified minimum-safe requirement.
3. Preserve non-reclaimable active obligations.
4. Compute free/uncommitted effective capacity.
5. If every minimum + justified desired need fits, allocate according to normal policy.
6. If not, reclaim from eligible workloads in ascending consequence-of-starvation order.
7. Prefer lower-cost degradation before pause/termination when both satisfy the same deficit safely.
8. Reassign reclaimed effective capacity to the highest-consequence unsatisfied valid obligation.
9. Repeat until no eligible internal move can reduce the deficit.
10. Produce exact remaining deficit by resource class.
11. Never exceed envelope or mutate Foundation authoritative grants.
```

### Consequence ordering

The coordinator does not use one permanent Application ranking. It evaluates an exact `ObligationPriorityKey`:

```text
ProtectionClass
ActiveObligationClass
ConsequenceSeverity
Irreversibility
TimeCriticality
MinimumSafeDeficitRatio
ReclaimabilityOfDonor
RecoveryCost
PolicyTieBreaker
```

Hard ordering principles:

1. Foundation constraints always dominate Application policy.
2. Capital/protection/open-obligation safety outranks growth/experimentation.
3. Active Guardian emergency protection may outrank normal new-risk Trading workload.
4. Reconciliation of open orders/positions/capital and minimum required operational data is protected above discovery/analytics/research/experimentation when needed for safety.
5. Simulation/experimentation is generally highly reclaimable unless a current explicit validation/governance obligation raises its consequence class.
6. `APPLICATION_NAME` alone is not a priority algorithm.

### Deterministic donor selection

Among equivalent eligible donor options, select in this order:

1. lower consequence severity;
2. higher immediately reclaimable quantity;
3. lower checkpoint/recovery cost;
4. lower current utilization efficiency;
5. canonical ApplicationId;
6. canonical WorkloadId.

### Plan identity

A `ResourcePlan` binds exact ResourcePictureSnapshotId, envelope/epoch, policy version and ordered actions. It is immutable once execution begins.

### Action classes

```text
NO_CHANGE
THROTTLE
REDUCE_CONCURRENCY
REDUCE_REFRESH_FREQUENCY
CHECKPOINT_AND_PAUSE
TERMINATE_RESTARTABLE
EFFECTIVE_REASSIGN
RESTORE
```

No action may change another Application's business truth. The target Application executes its own declared degradation mechanism after receiving the governed FSARM coordination command/outcome.

### Effect confirmation

FSARM distinguishes:

```text
PLAN_CREATED
COMMAND_DELIVERED
TARGET_ACKNOWLEDGED
TARGET_EFFECT_CONFIRMED
RESOURCE_RECLAIM_CONFIRMED
RESOURCE_EFFECTIVELY_REASSIGNED
```

It cannot reassign capacity based solely on delivery/ACK if actual reclaim confirmation is required by the resource class.

### Fencing

Every command/plan contains:

- FSARM coordinator identity;
- monotonic `CoordinatorEpoch`;
- ResourcePlanId/action sequence;
- Foundation resource epoch/envelope ID;
- target Application;
- idempotency key;
- expiry/supersession.

A target rejects older coordinator epoch/plan sequence after accepting a newer valid one.

## 11. R-LSA-03 — Foundation Binding, Restoration & Resource Evidence

### Components

- `R03.RemainingDeficitCalculator`
- `R03.FoundationRequestBuilder`
- `R03.FoundationOutcomeReconciler`
- `R03.RestorationPlanner`
- `R03.ResourceEvidenceLedger`
- `R03.ResourceDecisionExplainer`
- `R03.FoundationDependencyHealthEvaluator`

### Remaining deficit

Foundation additional request may be created only from:

```text
RemainingDeficit = max(
  0,
  ValidRequiredNeedAfterInternalActions
  - ConfirmedEffectiveCapacityAfterInternalActions
)
```

The calculation is per Foundation resource class and binds the exact ResourcePicture/Plan evidence.

### Request rule

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

A request contains the remaining deficit plus consequence/evidence. It never updates effective capacity until a valid Foundation outcome is received and reconciled.

### Foundation outcomes

At minimum FSARM handles:

```text
GRANTED
PARTIALLY_GRANTED
CAPPED
DENIED
REDUCED
REVOKED
RECLAIMED
REBALANCED
RESTORED
UNKNOWN/UNAVAILABLE
```

Exact names map to Foundation contracts. Unknown outcome freezes expansion and triggers reconciliation; it is not treated as grant.

### Restoration

After pressure/crisis decreases or Foundation capacity returns, restore in stages:

1. minimum-safe obligations;
2. suspended but safety-relevant operational paths;
3. normal production workload;
4. discovery/analytics;
5. experiments/simulation/research.

Restoration is bounded by current demand and cannot blindly restore a stale pre-crisis allocation.

### Evidence ledger

Every plan/request/outcome/effect/restoration record preserves:

- exact Foundation epoch/envelope;
- source Application reports;
- active obligation/protection evidence;
- policy version;
- selected/rejected alternatives;
- quantities before/after;
- delivery/effect confirmations;
- remaining deficit;
- Foundation request/outcome references;
- reason codes;
- causation/correlation.

## 12. APP-RSC MSA

`MSA-RSC` understands the complete FSARM Application condition, demand/report quality, coordination effectiveness, forecasting accuracy, resource policy performance and improvement opportunities.

It does not own Foundation resource policy/authority and cannot self-modify protected coordination constraints or deploy its own policy changes.

If APP-RSC is eventually accepted as an Application, APP-001 requires exactly one MSA. Two bounded Monitor AI perspectives would apply consistently with the current FSATS MSA direction unless the Owner explicitly decides otherwise during topology acceptance.

## 13. APP-RSC Manifest Candidate

If accepted, CON-023 declaration shall include:

- immutable APP-RSC identity;
- FSATS-only purpose/scope;
- exact three-LSA topology;
- constituent resource-report/coordination contracts;
- Foundation resource dependencies;
- authority ceiling limited to valid coordination envelope;
- no business-state ownership;
- resource/evidence persistence;
- lifecycle/fencing/recovery behavior;
- one MSA and optional CSA policy;
- Guardian protection interface;
- fail-closed behavior if Foundation resource truth unavailable.

## 14. High Availability / Single Coordinator

Current design uses one authoritative active APP-RSC instance/leader per governed FSATS coordination scope, controlled by Foundation Application lifecycle/deployment infrastructure when available.

It does not invent its own distributed consensus platform.

If multiple process instances exist for availability, only one valid `CoordinatorEpoch` may issue current plans. Election/lease authority must come from an approved Foundation/platform capability or a future explicitly governed design. Without that capability, multi-writer mode is forbidden.

```text
NO_VALID_COORDINATOR_EPOCH
=> NO_NEW_REBALANCE
=> PRESERVE_LAST_CONFIRMED_SAFE_EFFECTIVE_STATE
=> APPLICATIONS FALL BACK TO OWN MINIMUM-SAFE DEGRADATION
```

## 15. APP-RSC Failure Semantics

If FSARM is unavailable:

- Applications do not invent peer-to-peer redistribution;
- each Application stays within its last confirmed effective capacity and Foundation hard bounds;
- each Application uses local minimum-safe degradation rules;
- Guardian may continue valid protection directives but cannot seize resources directly;
- additional Foundation resource requests through FSARM are unavailable unless a separately governed failover coordinator exists;
- evidence records inability to coordinate.

This is safer than allowing multiple peers to independently reallocate the same capacity.

## 16. Security / Authority

FSARM commands are authoritative only when:

- APP-RSC identity is admitted/active;
- coordinator epoch is current;
- exact resource action is inside the current envelope;
- target Application and resource class are declared;
- command is within granted APP-RSC authority/permissions;
- Foundation transport/security validation succeeds;
- command is operational, not replay/test/simulation.

No FSARM payload can directly mutate Foundation authoritative resource state.

## 17. Relationship to Future Falcon-Wide FSARM Backlog

GitHub issue #33 remains `FUTURE_BACKLOG_ONLY` and has no current authority.

This candidate is **FSATS-scoped only**.

A future Falcon-wide resource intermediary would require a fresh architecture study and SHALL NOT be inferred from APP-RSC acceptance.

## 18. Material Delta Register

This file intentionally proposes these semantic changes relative to the accepted historical topology:

```text
DELTA-01: APPLICATION COUNT 4 -> 5
DELTA-02: MSA COUNT 4 -> 5
DELTA-03: LSA COUNT 31 -> 34
DELTA-04: FSARM GAINS EXPLICIT APP-001 PRINCIPAL/LIFECYCLE INSTEAD OF HIDDEN ROLE
```

It does **not** change:

- FSATS remains non-owning;
- Foundation resource authority remains Foundation-owned;
- current four Application business responsibilities;
- current FCR-0031 coordination-envelope semantics;
- future Falcon-wide backlog status.

These deltas require fresh Architecture/Consistency, fresh Red-Team and explicit Owner decision before becoming current design.

## 19. Verification Families

The FSARM verifier SHALL challenge at least:

1. APP-RSC cannot exceed envelope;
2. request cannot become grant;
3. stale Foundation epoch fails closed;
4. Application self-asserted minimum inflation is rejected without evidence/profile;
5. constituent identity/accounting preserved;
6. no permanent Application-name-only priority;
7. donor selection deterministic;
8. effect ACK != reclaim confirmation;
9. no double allocation before reclaim confirmation;
10. coordinator epoch anti-split-brain fencing;
11. old command replay rejected;
12. Guardian crisis evidence may raise consequence but not Foundation technical criticality;
13. FSTSimA capacity can be reclaimed only according to declared reclaimability/checkpoint semantics;
14. open-order/position reconciliation workload cannot be shed below safe minimum;
15. remaining deficit mathematically correct;
16. partial/deny/revoke Foundation outcomes reconciled exactly;
17. staged restoration uses current demand, not stale baseline;
18. FSARM failure does not create peer redistribution authority;
19. APP-RSC cannot read/mutate peer internals;
20. resource command cannot change Application business truth directly;
21. replay/simulation resource commands cannot affect operational state;
22. APP-RSC does not become Falcon-wide by implication;
23. APP-RSC MSA cannot self-promote resource policy;
24. topology delta is not treated as accepted before Owner decision.

## 20. Candidate Decision

```text
PROPOSED_FSARM_RUNTIME_REALIZATION = DEDICATED_FSATS_SCOPED_APP_RSC
CURRENT_OWNER_ACCEPTANCE = NOT_YET
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
```

The recommendation is made because it gives the coordinator the explicit identity, lifecycle, permission, failure-containment and ownership model required by APP-001 while avoiding privileged placement inside any existing peer Application or a hidden stateful principal under the FSATS grouping.
