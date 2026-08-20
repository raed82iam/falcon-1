# P1-J — APP-RSC Code-Ready Resource Management Decomposition

**Status:** `DESIGN_MATERIALIZATION / OWNER_DIRECTED_COMPLETION_CYCLE`  
**Scope:** `P1-J DESIGN ONLY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## 1. Identity and Scope

APP-RSC is the Owner-accepted fifth independent FSATS Falcon Application:

```text
APPLICATION_ID = APP-RSC
CANONICAL_ROLE = FALCON SELF-AWARE RESOURCE MANAGEMENT APPLICATION
SCOPE = FSATS_ONLY
APPLICATION = YES
FOUNDATION_RESOURCE_GOVERNANCE = NO
FSATS_CONTAINER = NO
MSA = 1
LSA = 3
CSA = 0 initially
```

APP-RSC coordinates bounded effective resource use among current FSATS Applications. Foundation remains authoritative for total Falcon resource truth, Application grants, ceilings, protected floors and Foundation-governed priority/resource decisions.

## 2. Physical Placement

P1-C topology applies:

```text
ResourceManagement.Contracts
ResourceManagement.Domain
ResourceManagement.Application
ResourceManagement.Infrastructure
ResourceManagement.Awareness
ResourceManagement.Host
```

No constituent Application is a child process/hidden module of APP-RSC. Each remains separately admitted, accountable, isolatable and removable.

## 3. R-LSA-01 Resource Picture, Demand Integrity & Coordination Envelope

Components: `ConstituentResourceRegistry`, `ResourceEvidenceIngestor`, `DemandIntegrityEvaluator`, `EffectiveEnvelopeTracker`, `ResourcePictureBuilder`, `ClaimFreshnessEvaluator`.

Owns the current FSATS coordination picture derived from separately attributable constituent evidence and current Foundation-authoritative resource outcomes. It never replaces Foundation truth.

Distinctions:

```text
APPLICATION_REPORTED_NEED != PROVEN_RESIDUAL_NEED
APP_RSC_EFFECTIVE_PICTURE != FOUNDATION_AUTHORITATIVE_TRUTH
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

Every claim preserves Application identity, resource class, current allocation/consumption, minimum-safe need, desired capacity, pressure/urgency, reclaimability, degradation consequence, freshness, confidence/evidence and current Foundation envelope reference.

## 4. R-LSA-02 Redistribution, Degradation & Rebalance

Components: `ResourceStrategyController`, `RedistributionPlanner`, `ReclaimPlanner`, `DegradationCoordinator`, `RebalanceExecutor`, `RestorationPlanner`, `OscillationGuard`, `StarvationGuard`.

`ResourceStrategyController` is operational control, not the APP-RSC MSA and not an Awareness tier.

Prime sequence:

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
```

Within the currently valid Foundation envelope, APP-RSC may coordinate only actions explicitly permitted by current policy and constituent declarations. It evaluates active obligation, consequence of starvation, minimum-safe requirement, reclaimability, current pressure, protection state, checkpoint cost and restoration cost rather than relying on a permanent Application rank.

Risk/safety-critical obligations may justify preferential effective capacity when evidence proves current consequence, but APP-RSC cannot rewrite Foundation criticality or seize protected minima.

## 5. R-LSA-03 Foundation Binding, Restoration & Resource Evidence

Components: `FoundationEnvelopeConsumer`, `ResidualNeedCalculator`, `FoundationRequestAssembler`, `FoundationOutcomeConsumer`, `ResourceEpochRegistry`, `RestorationCoordinator`, `ResourceEvidenceLedger`.

Owns exact binding between current Foundation-authoritative envelope/outcomes and APP-RSC effective coordination. Additional-resource requests are assembled only after safe internal optimization and proven residual deficit.

Foundation outcomes distinguish grant/partial/cap/deny/reduce/revoke/reclaim/rebalance/restore as supplied by the authoritative Foundation boundary. APP-RSC may consume but never manufacture them.

FCR-0031 design compatibility is complete; final implementation/binding verification remains pending until code/executable fixtures exist.

## 6. Coordination Epoch and Split-Brain Fencing

Exactly one valid coordination epoch may govern an effective FSATS resource coordination decision at a time. Every action binds:

- APP-RSC Application identity;
- coordinator instance/epoch identity;
- input evidence versions/freshness;
- Foundation envelope identity/version;
- policy/strategy version;
- decision identity;
- target constituent Application;
- effective amount/action;
- causation/correlation;
- expiry/lease where applicable.

Stale, duplicate, conflicting or revoked coordinator epochs are fenced. A restarted APP-RSC cannot silently resume old coordination authority.

## 7. Constituent Interfaces

Trading, FSAPMA, Guardian and FSTSimA publish Application-owned resource evidence. APP-RSC consumes that evidence and returns only governed effective coordination outcomes. It does not read peer internals or business state directly.

Required constituent fields include as applicable:

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

Constituent Applications own the business meaning and safe internal shedding order of their work.

## 8. Safety / Crisis Behavior

During a protection crisis, Guardian may publish increased minimum-safe/urgency evidence. APP-RSC may reclaim eligible resources from deferrable FSTSimA/analytics/research/discovery workloads only within current policy and protected floors.

APP-RSC must not create a feedback loop where an Application exaggerates urgency to steal capacity. Repeated claims are checked for attribution, freshness, consistency, observed consequence and policy eligibility.

## 9. APP-RSC Failure / Degraded State

If APP-RSC becomes unavailable/untrusted:

- no new cross-Application redistribution is assumed;
- no sibling Application inherits coordination authority;
- constituent Applications continue only within last valid Foundation/App resource truth and their own safe degraded rules where permitted;
- stale/unknown Foundation envelope prevents new effective coordination;
- in-flight coordination actions are fenced by epoch/causation;
- restoration waits for trustworthy state reconstruction and Controlled Revival where AI trust was affected.

APP-RSC AI Kill does not necessarily kill the APP-RSC Application if deterministic non-AI coordination safety/evidence functions remain trustworthy, but risk-increasing/new redistribution decisions requiring killed intelligence are denied.

## 10. Anti-Gaming Rules

Resource claims are evidence, not entitlement. APP-RSC checks:

- impossible/contradictory minimum-safe claims;
- urgency inflation;
- repeated non-reclaimable classification without evidence;
- pressure claims inconsistent with observed consumption;
- hidden retained capacity;
- churn designed to bias dynamic priority;
- attempts to classify business desire as survival floor.

Unknown claim integrity reduces eligibility for aggressive redistribution rather than increasing it.

## 11. Persistence / Reconstruction

Resource picture, latest valid Foundation envelope reference, coordinator epoch, committed redistribution actions, outstanding restoration obligations and evidence must be reconstructable outside any killed AI's volatile memory. Restart does not restore coordination trust automatically.

## 12. Required Later Implementation Tests

Two coordinator instances; stale epoch action; duplicate rebalance; Foundation envelope revoked mid-action; constituent report stale; Guardian crisis + reclaimable FSTSimA capacity; false urgency claim; minimum-safe conflict; APP-RSC crash during redistribution; restart with incomplete action; Foundation deny after residual request; partial grant; resource reclaim then staged restore; peer tries direct seizure; constituent bypasses APP-RSC; APP-RSC attempts non-FSATS control; APP-RSC tries to mint grant/ceiling; AI Kill with queued redistribution; oscillation/thrashing challenge; starvation challenge.

## 13. P1-J Closure Invariants

- APP-RSC remains an independent FSATS-only Application;
- Foundation authoritative resource truth/authority is never cloned;
- constituent Applications remain separately attributable and admitted;
- internal redistribution precedes additional-resource request when safe/allowed;
- stale/duplicate coordinator epochs are fenced;
- no peer seizure or authority inheritance occurs on APP-RSC failure;
- dynamic priority is evidence-based and bounded by protected minima/policy;
- Resource Strategy Controller is operational control, not Awareness;
- design remains compatible with FCR-0031 while executable verification stays deferred to implementation.
