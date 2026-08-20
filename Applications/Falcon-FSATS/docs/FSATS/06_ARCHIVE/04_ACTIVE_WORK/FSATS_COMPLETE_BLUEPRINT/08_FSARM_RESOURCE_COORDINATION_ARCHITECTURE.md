# FSATS Complete Blueprint — FSARM Resource Coordination Architecture

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Role:** `FSATS-wide governed resource coordination`
**Awareness Tier:** `NONE`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

FSARM coordinates resource use across the four FSATS Applications so already-available capacity is used safely before additional Foundation capacity is requested.

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
REQUESTED_RESOURCE != PROVEN_RESIDUAL_NEED != GRANTED_RESOURCE
```

FSARM is not Foundation Resource Governance and cannot create resources or mutate Foundation-authoritative total-resource truth.

## 2. Identity Boundary

FSARM is not:

- an MSA;
- an LSA;
- a CSA;
- a fifth hidden FSATS Application;
- a Foundation service;
- a business strategy/risk engine.

Its final executable/admission identity must use the Foundation-supported coordinator/delegation boundary accepted through Stage 6 and reflected by FCR-0031. Application code shall not invent an unauthorized runtime principal to host it.

The code plan therefore separates:

- pure resource-coordination business logic;
- contracts/profiles;
- Foundation binding/hosting adapter.

The last element remains gated by exact accepted Foundation consumption/admission evidence.

## 3. Two-Layer Resource Model

### Foundation layer

Foundation owns:

- total-resource truth;
- protected Foundation floors/reserves;
- per-Application authoritative grants/ceilings;
- Foundation technical criticality;
- final additional-resource grant/partial/cap/deny/defer outcomes;
- governed reduce/revoke/reclaim/rebalance/restore authority where defined.

### FSARM layer

Within an accepted coordination envelope, FSARM owns bounded effective distribution of eligible FSATS capacity while preserving constituent attribution.

```text
FOUNDATION AUTHORITATIVE GRANT TRUTH
!= FSARM EFFECTIVE INTERNAL DISTRIBUTION
```

FSARM cannot forge a larger Foundation grant.

## 4. Constituent Applications

FSARM coordinates resource evidence for:

- Falcon Self-Aware Trading Application;
- FSAPMA;
- Falcon Trading Guardian Application;
- FSTSimA.

Every constituent remains individually identifiable/accountable. FSARM does not collapse them into an opaque shared resource pool.

## 5. Resource Profile

Each Application publishes an attributable resource profile containing, as applicable:

- current effective allocation;
- current observed consumption;
- minimum-safe/survival requirement;
- desired allocation;
- protected workload requirement;
- reclaimable capacity;
- degradable workload;
- suspendable workload;
- recovery/restoration requirement;
- consequence of starvation;
- pressure/urgency evidence;
- current business/protection state;
- freshness/expiry;
- source identity/evidence.

A requested value is evidence, not entitlement.

## 6. Workload Classes

Applications classify workloads by consequence and reclaimability, not by permanent Application rank.

Candidate classes:

```text
SURVIVAL_CRITICAL
PROTECTION_CRITICAL
OPEN_POSITION_SAFETY
RECONCILIATION_CRITICAL
LIVE_DATA_CRITICAL
EXECUTION_CRITICAL
NORMAL_OPERATIONAL
ANALYTICS
DISCOVERY
SIMULATION
EXPERIMENTATION
RESEARCH
BACKGROUND
```

Exact priority is dynamic because an Application can contain both critical and deferrable work simultaneously.

## 7. Dynamic Priority Evidence

FSARM evaluates:

- active obligation;
- consequence of starvation;
- minimum-safe requirement;
- currently protected capital/execution responsibility;
- reclaimability;
- current pressure;
- Guardian crisis state;
- open-position/order state;
- operational data need;
- restoration dependency;
- current admitted resource policy;
- Foundation constraints.

It does not use one permanent `Guardian > Trading > FSAPMA > FSTSimA` ranking for all situations.

## 8. Internal Redistribution Decision

Conceptual flow:

```text
COLLECT CURRENT RESOURCE PROFILES
-> VERIFY IDENTITY / FRESHNESS / POLICY
-> ESTABLISH CURRENT EFFECTIVE ENVELOPE
-> PROTECT MINIMUMS / NON-RECLAIMABLE FLOORS
-> IDENTIFY CURRENT DEFICIT
-> IDENTIFY ELIGIBLE RECLAIMABLE CAPACITY
-> EVALUATE STARVATION CONSEQUENCES
-> BUILD REDISTRIBUTION PLAN
-> VERIFY PLAN AGAINST FOUNDATION CEILINGS / COORDINATION ENVELOPE
-> FENCE PLAN VERSION / EPOCH
-> APPLY BOUNDED EFFECTIVE REDISTRIBUTION
-> OBSERVE RESULT
-> PUBLISH ATTRIBUTABLE OUTCOME
```

## 9. Example Crisis Redistribution

If Guardian has a verified protection-critical deficit while FSTSimA runs pauseable simulations:

```text
GUARDIAN VERIFIED MINIMUM DEFICIT
+ FSTSIMA RECLAIMABLE CAPACITY
+ VALID COORDINATION ENVELOPE
-> PAUSE / THROTTLE ELIGIBLE FSTSIMA WORK
-> MAKE CAPACITY EFFECTIVELY AVAILABLE TO GUARDIAN
-> PRESERVE PER-APPLICATION ATTRIBUTION
-> OBSERVE EFFECT
```

This must not require a Foundation round trip for every internal move that already fits the accepted envelope.

If the move requires changing Foundation-authoritative grants/ceilings, Foundation authority is required.

## 10. Residual Need Calculation

After safe internal redistribution:

```text
PROVEN_REQUIRED_NEED
- VERIFIED_EFFECTIVE_CAPACITY
- SAFE_ELIGIBLE_INTERNAL_REALLOCATION
= PROVEN_RESIDUAL_NEED
```

Only the residual need is eligible for additional Foundation request.

FSARM must not inflate demand to improve chances of receiving capacity.

## 11. Foundation Request Binding

A Foundation additional-resource request binds, as applicable:

- coordinator/requester identity;
- delegation/scope;
- exact constituent attribution;
- current authoritative grants/ceilings;
- protected minimums;
- proven residual need;
- evidence;
- correlation/causation;
- expiry;
- request version/epoch.

Foundation decision outcome remains distinct from applied capacity.

## 12. Resource Decision States

FSARM tracks:

```text
OBSERVED
PLANNED
REQUESTED
FOUNDATION_DECIDED
INTERNAL_REDISTRIBUTION_PENDING
EFFECTIVE
PARTIAL
DEGRADED
RESTORING
SUPERSEDED
FAILED
```

No state is inferred merely from an API/transport acknowledgement.

## 13. Fencing and Split-Brain Protection

Only the current valid coordinator epoch/delegation may issue effective FSARM coordination decisions.

Required controls:

- coordinator identity;
- epoch/lease identity where Foundation design uses one;
- monotonic plan version;
- idempotency key;
- expiry;
- stale coordinator rejection;
- duplicate suppression;
- supersession trace;
- constituent acknowledgement/outcome evidence.

Two active coordinators must never independently redistribute the same capacity without deterministic conflict rejection.

## 14. Application-Internal Shedding

FSARM determines resource outcomes/envelopes, but each Application owns its business-specific internal shedding sequence.

Examples:

- Trading knows which analytics/discovery tasks can pause without harming open positions.
- FSAPMA knows which provider refresh/background requests can degrade before protected streams.
- FSTSimA knows which simulations can pause while preserving accepted evidence.
- Guardian knows its own noncritical analytics versus survival functions.

```text
FSARM RESOURCE OUTCOME != CROSS-APPLICATION BUSINESS CONTROL
```

## 15. Restoration

Resource restoration is controlled and staged.

```text
PRESSURE REDUCED
-> VERIFY FOUNDATION / COORDINATION STATE
-> VERIFY PROTECTED OBLIGATIONS STABLE
-> IDENTIFY RESTORATION CANDIDATES
-> RESTORE IN BOUNDED STAGES
-> OBSERVE
-> CONTINUE / HOLD / REVERSE
```

Background/research/simulation work must not surge simultaneously after recovery and recreate overload.

## 16. Resource Evidence History

FSARM preserves attributable records of:

- pressure observations;
- profiles;
- plans;
- decision reasons;
- reclaimed capacity;
- affected workloads;
- Foundation requests/outcomes;
- applied internal effects;
- failures;
- restoration;
- unresolved divergence.

This permits post-incident reconstruction and tuning of resource policy.

## 17. Self-Awareness Relationship

FSARM is not self-awareness.

Each Application's MSA/LSAs may learn about resource behavior within their jurisdiction and propose improvements. T-LSA-13 is Trading-side resource awareness.

FSARM may contain deterministic/adaptive algorithms, but any intelligent component eligible for CSA must still belong to a valid Application owner; FSARM does not invent its own awareness hierarchy.

## 18. Fail-Closed Conditions

Do not redistribute when material uncertainty exists in:

- coordinator authority/delegation;
- constituent identity;
- current Foundation grant/ceiling;
- protected minimum;
- capacity ownership;
- current plan epoch;
- effect confirmation where required.

When FSARM is unavailable, Applications remain inside the last valid Foundation-authoritative and locally enforceable limits. They may degrade according to pre-approved local policies but cannot infer permission to exceed ceilings.

## 19. Resource Abuse Protections

Prevent:

- exaggerated demand;
- self-minted priority;
- fake urgency;
- reclaiming protected minimums;
- hiding consumption in opaque shared pools;
- unbounded resource hoarding;
- permanent emergency priority after condition expiry;
- starvation of reconciliation/protection tasks;
- restoration stampede.

## 20. FCR Binding

The implementation must later verify exact compatibility with:

- FCR-0007 accepted Stage 6 additional-resource request boundary;
- FCR-0010 resource pressure/load-shedding boundary;
- FCR-0031 aggregate resource-management boundary.

Current design work does not close FCR-0010 or FCR-0031 because their Application-side closure requires actual code/binding/fixture evidence.

## 21. Acceptance Gates

```text
FOUNDATION_TOTAL_RESOURCE_TRUTH_DUPLICATION = 0
OPAQUE_FSATS_SHARED_POOL = 0
PERMANENT_STATIC_APP_PRIORITY = 0
APPLICATION_DIRECT_FOUNDATION_RESOURCE_BYPASS = 0
FSARM_BUSINESS_DECISION_OWNERSHIP = 0
REQUEST_AS_GRANT_CONFLATION = 0
UNFENCED_COORDINATOR_SPLIT_BRAIN = 0
PROTECTED_MINIMUM_RECLAIM = 0
RESTORATION_STAMPEDE_PATH = 0
```
