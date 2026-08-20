# FSATS Specialized Implementation Architecture — Cross-Application Contract Schema and Route Catalog

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`
**Runtime Route Activation:** `NOT_GRANTED`

## 1. Purpose

Define the canonical Application-owned business contracts that cross FSATS Application boundaries. Foundation owns FIL/admission/routing/delivery/event/security infrastructure; the producer/semantic-owner Application owns the business payload meaning.

No contract in this file creates runtime authority merely because its schema exists.

## 2. Contract Envelope Binding

Every cross-Application message SHALL be carried through the accepted Foundation message/route model when runtime is authorized.

Application payloads SHALL NOT reimplement Foundation-owned:

- message identity;
- correlation/causation identity;
- producer/recipient technical identity;
- schema registry identity where Foundation owns it;
- delivery attempt/outcome;
- cryptographic transport context;
- event journal identity;
- Foundation authority/security context.

Every Application-owned schema below nevertheless declares the exact business fields that must be bound to the Foundation envelope/manifest route.

## 3. Common Business Header

Every business payload includes or is bound to this conceptual metadata:

```text
ContractFamilyId
BusinessSchemaVersion
ProducerApplicationId
IntendedConsumerApplicationId
OperationalClassification
BusinessEffectiveAt
BusinessExpiresAt?              // null only when contract explicitly allows non-expiring state snapshot
SubjectScope
BusinessPolicyVersion?          // where decision/policy bound
ReasonCodes[]
BusinessEvidenceRefs[]
PayloadDigest
```

`OperationalClassification` is one of:

```text
OPERATIONAL
SHADOW
REPLAY
SIMULATION
TEST
RESEARCH
```

Only `OPERATIONAL` is eligible for production business action unless a consumer contract explicitly expects another class for non-authoritative evaluation.

## 4. Schema Compatibility Rule

For each contract family:

- same major version must preserve required field meaning;
- adding optional field is compatible only when old consumers safely ignore it under the registered compatibility policy;
- removing/renaming/changing units/authority meaning of a required field is breaking;
- changing enum semantics without new version is breaking;
- defaulting a missing authority/scope/quality field to permissive behavior is forbidden;
- unknown major version fails closed;
- compatibility is established by schema registry/verification, not assumption.

## 5. Route/Authority Classes

Business authority class is one of:

```text
OBSERVATION
QUERY
REQUEST
RECOMMENDATION
DECISION_RESULT
RESOURCE_COORDINATION
PROTECTION_DIRECTIVE
VALIDATION_EVIDENCE
STATE_PROJECTION
ACKNOWLEDGEMENT
```

These classes do not replace Foundation authority instruments. They describe business meaning and required consumer handling.

## 6. Canonical Contract Family Inventory

### Provider Management -> Trading

| ID | Contract | Class |
|---|---|---|
| PMA-TRD-001 | Normalized Data Product Delivery | STATE_PROJECTION |
| PMA-TRD-002 | Data Quality / Availability State | STATE_PROJECTION |
| PMA-TRD-003 | Provider Capability Projection | STATE_PROJECTION |
| PMA-TRD-004 | Provider Route Degradation Notice | OBSERVATION |

### Trading -> Provider Management

| ID | Contract | Class |
|---|---|---|
| TRD-PMA-001 | Data Product Demand Declaration | REQUEST |
| TRD-PMA-002 | Data Consumption / Fitness Feedback | OBSERVATION |

### Provider Management -> Guardian

| ID | Contract | Class |
|---|---|---|
| PMA-GRD-001 | Provider/Data Protection Observation | OBSERVATION |
| PMA-GRD-002 | Provider Route State Projection | STATE_PROJECTION |

### Guardian -> Provider Management

| ID | Contract | Class |
|---|---|---|
| GRD-PMA-001 | Provider Route Protection Directive | PROTECTION_DIRECTIVE |
| GRD-PMA-002 | Provider Protection Release Directive | PROTECTION_DIRECTIVE |

### Trading -> Guardian

| ID | Contract | Class |
|---|---|---|
| TRD-GRD-001 | Trading Protection Observation | OBSERVATION |
| TRD-GRD-002 | Trading Exposure/Order/Position Safety Projection | STATE_PROJECTION |
| TRD-GRD-003 | Protection Directive Effect Outcome | ACKNOWLEDGEMENT |

### Guardian -> Trading

| ID | Contract | Class |
|---|---|---|
| GRD-TRD-001 | Trading Protection Directive | PROTECTION_DIRECTIVE |
| GRD-TRD-002 | Protection Release Directive | PROTECTION_DIRECTIVE |
| GRD-TRD-003 | Guardian Crisis State Projection | STATE_PROJECTION |

### Production Applications -> FSTSimA

| ID | Contract | Class |
|---|---|---|
| TRD-SIM-001 | Candidate/Experiment Definition | REQUEST |
| PMA-SIM-001 | Sanitized Provider/Data Fixture Definition | REQUEST |
| GRD-SIM-001 | Protection Scenario Validation Request | REQUEST |

### FSTSimA -> Production Applications

| ID | Contract | Class |
|---|---|---|
| SIM-TRD-001 | Strategy/Execution Validation Evidence | VALIDATION_EVIDENCE |
| SIM-PMA-001 | Provider/Data Pipeline Validation Evidence | VALIDATION_EVIDENCE |
| SIM-GRD-001 | Guardian/Crisis Validation Evidence | VALIDATION_EVIDENCE |

### Constituent Applications -> FSARM / APP-RSC candidate

| ID | Contract | Class |
|---|---|---|
| TRD-RSC-001 | Trading Resource Demand Report | RESOURCE_COORDINATION |
| PMA-RSC-001 | Provider Management Resource Demand Report | RESOURCE_COORDINATION |
| GRD-RSC-001 | Guardian Resource Demand / Crisis Need Report | RESOURCE_COORDINATION |
| SIM-RSC-001 | Simulation Resource Demand / Reclaimability Report | RESOURCE_COORDINATION |

### FSARM / APP-RSC candidate -> Constituent Applications

| ID | Contract | Class |
|---|---|---|
| RSC-TRD-001 | Trading Resource Coordination Directive | RESOURCE_COORDINATION |
| RSC-PMA-001 | Provider Management Resource Coordination Directive | RESOURCE_COORDINATION |
| RSC-GRD-001 | Guardian Resource Coordination Directive | RESOURCE_COORDINATION |
| RSC-SIM-001 | Simulation Resource Coordination Directive | RESOURCE_COORDINATION |
| RSC-ALL-001 | Effective Resource Coordination State Projection | STATE_PROJECTION |

### Constituent Applications -> FSARM effect confirmation

| ID | Contract | Class |
|---|---|---|
| TRD-RSC-002 | Trading Resource Effect Outcome | ACKNOWLEDGEMENT |
| PMA-RSC-002 | Provider Management Resource Effect Outcome | ACKNOWLEDGEMENT |
| GRD-RSC-002 | Guardian Resource Effect Outcome | ACKNOWLEDGEMENT |
| SIM-RSC-002 | Simulation Resource Effect Outcome | ACKNOWLEDGEMENT |

Total current candidate families in this file: **37**.

This count is a new SIA candidate count and SHALL NOT rewrite or masquerade as the historical accepted P0 43-contract baseline. Any historical family not represented here must be reconciled in file 20 traceability before the package can reach semantic freeze.

## 7. PMA-TRD-001 — Normalized Data Product Delivery

**Owner:** APP-PMA / P-LSA-02 and P-LSA-05
**Consumer:** APP-TRD
**Authority class:** STATE_PROJECTION
**Operational classes accepted by Trading production path:** OPERATIONAL only

Payload v1 fields:

```text
DataProductId                 required
DataProductVersionId          required
ObservationId                 required
MarketId                      required
InstrumentId?                 required when product is instrument-scoped
DataProductClass              required
EffectiveTimeRef              required
SourceProviderIds[]           required, min 1
SourceObservationRefs[]       required, min 1
QualityState                  required
QualityScore                  required when state is VALID/DEGRADED
QualityModelVersionId         required when QualityScore exists
FreshnessStatus               required
CorrectionOfObservationId?    optional
SupersedesObservationId?      optional
Payload                       required, typed by DataProductId/Version
PayloadDigest                 required
BusinessProvenanceRefs[]      required
```

Consumer rules:

- `VALID`: usable if product/version/profile matches Trading requirement;
- `DEGRADED`: usable only where the exact consumer declares degraded acceptance;
- `STALE/CONFLICTED/INCOMPLETE/UNAVAILABLE/UNKNOWN`: cannot feed new-risk strategy evaluation;
- REPLAY/SIMULATION/SHADOW payload cannot enter the production-operational decision path.

Duplicate rule: same ObservationId + same digest = idempotent duplicate; same ObservationId + different digest = integrity conflict.

Correction rule: append correction/supersession; never silently rewrite prior decision evidence.

## 8. TRD-PMA-001 — Data Product Demand Declaration

**Owner:** APP-TRD / T-LSA-02/T-LSA-03
**Consumer:** APP-PMA
**Class:** REQUEST

Payload:

```text
DemandId
TradingAccountId
MarketId
InstrumentSet/UniverseSnapshotId
RequiredDataProducts[]:
  DataProductId
  MinimumCompatibleVersion
  RequiredFreshness
  RequiredQualityStateFloor
  SessionCoverage
  History/LookbackRequirement
  Depth/GranularityRequirement
PriorityPurpose = OPEN_OBLIGATION_SAFETY | ACTIVE_DECISION | DISCOVERY | ANALYTICS | RESEARCH
EffectiveFrom
ExpiresAt
EvidenceRefs[]
```

A demand does not guarantee provider availability/delivery. FSAPMA returns explicit availability/degradation through PMA-TRD-002/003/004 and Data Product delivery.

## 9. PMA-TRD-002 — Data Quality / Availability State

Payload:

```text
DataProductId
MarketId
InstrumentScope
CurrentQualityState
CurrentQualityScore?
FreshnessState
AvailableProviderCount
CurrentPrimaryProviderId?
ObservedGapState
EffectiveAt
ExpiresAt
EvidenceRefs[]
```

This projection may cause Trading to fail closed but cannot create Trading authority.

## 10. TRD-PMA-002 — Data Fitness Feedback

Payload:

```text
FeedbackId
DataProductId/Version
Observation/WindowRefs[]
ConsumerUseCase
FitnessOutcome = FIT | DEGRADED_USABLE | UNFIT | UNKNOWN
ObservedImpactCategory
ObservedLatency/Gap/Conflict metrics
ReasonCodes[]
EvidenceRefs[]
```

This is feedback for FSAPMA quality/routing improvement, not permission to rewrite historical Data Product truth.

## 11. GRD-TRD-001 — Trading Protection Directive

**Owner:** APP-GRD / G-LSA-02
**Consumer:** APP-TRD
**Class:** PROTECTION_DIRECTIVE

Payload:

```text
ProtectionDirectiveId
GuardianIncidentId
AuthorityReference
PolicyVersion
TargetTradingAccountId?
TargetMarketId?
TargetInstrumentId?
TargetStrategyId?
TargetOrderChainId?
TargetPositionId?
Action
Severity
EffectiveFrom
ExpiresAt?
SupersedesDirectiveId?
RequiredEffectConfirmation
ReasonCodes[]
EvidenceRefs[]
```

Consumer validation order:

1. producer identity = admitted APP-GRD;
2. operational classification = OPERATIONAL;
3. authority reference valid/current;
4. target scope includes this APP-TRD instance/account;
5. directive not expired/superseded;
6. action supported and exact schema version compatible;
7. Foundation transport/security evidence valid.

Invalid/unknown material authority => directive cannot be applied as authoritative, but Trading must emit a protection-integrity failure/alert rather than ignore silently.

Action semantics:

- `RESTRICT_NEW_RISK`: block creation of new risk-increasing execution intents in scope;
- `REDUCE_ALLOWED_EXPOSURE`: apply exact ceiling supplied by versioned directive extension/profile;
- `SUSPEND_*`: block relevant new risk and evaluate existing obligation policy;
- `CANCEL_OPEN_ORDERS`: request T-LSA-09 cancel of eligible orders; not a canceled-state assertion;
- `EXIT_POSITION_SCOPE`: initiate governed risk-reducing exit through T-LSA-07/08/09 protective path;
- release requires GRD-TRD-002 or a valid superseding directive.

## 12. TRD-GRD-003 — Protection Directive Effect Outcome

Payload:

```text
ProtectionDirectiveId
TargetApplicationId = APP-TRD
TargetScope
OutcomeState = RECEIVED | REJECTED_INVALID | APPLYING | EFFECT_CONFIRMED | PARTIAL | FAILED | RECONCILIATION_REQUIRED
AffectedOrderChainIds[]
AffectedPositionIds[]
EffectiveRestrictionSnapshotRef?
ReasonCodes[]
EvidenceRefs[]
```

`RECEIVED` is not `EFFECT_CONFIRMED`.

## 13. GRD-PMA-001 — Provider Route Protection Directive

Payload:

```text
ProtectionDirectiveId
GuardianIncidentId
AuthorityReference
ProviderId?
ProviderRouteId?
DataProductScope?
Action = ISOLATE_PROVIDER_ROUTE | RESTRICT_PROVIDER_ROUTE | REQUIRE_MULTI_SOURCE_VALIDATION
Severity
EffectiveFrom
ExpiresAt?
EvidenceRefs[]
```

PMA applies only actions within its declared Guardian interface. Guardian cannot choose a replacement provider business route directly; FSAPMA re-runs route eligibility/selection after restriction.

## 14. Trading Protection Observation — TRD-GRD-001

Payload:

```text
ObservationId
TradingAccountId
ObservationClass
RiskDecisionId?
OrderChainId?
PositionId?
MarketId?
InstrumentId?
SeverityEvidence
ObservedAt
CurrentStateDigest
ReasonCodes[]
EvidenceRefs[]
```

Observation is not an incident and not a directive.

## 15. Trading Safety Projection — TRD-GRD-002

Payload snapshot:

```text
ProjectionId
TradingAccountId
PortfolioSnapshotId
OpenOrderSummary
OpenPositionSummary
CapitalStateRef
DrawdownState
ExecutionAmbiguityCount
RiskRestrictionState
TradingReadinessState
DataDependencyHealthSummary
EffectiveAt
ExpiresAt
EvidenceRefs[]
```

Guardian may consume this for protection only; it cannot mutate Trading state through the projection.

## 16. PMA-GRD-001 — Provider/Data Protection Observation

Payload:

```text
ObservationId
ProviderId/RouteId/DataProductId scope
ObservationClass = STALE | CONFLICTED | CORRUPT | QUOTA_EXHAUSTION | OUTAGE | AUTH_FAILURE | REPEATED_DISCONNECT | QUALITY_COLLAPSE
QualityState
SeverityEvidence
ObservedAt
EvidenceRefs[]
```

## 17. Simulation Validation Contracts

### TRD-SIM-001 Candidate/Experiment Definition

Payload:

```text
ExperimentId
CandidateArtifactIds[]
BaselineArtifactIds[]
Strategy/Model/Config version refs
RequiredMarketProfiles[]
ScenarioRequirements[]
Metrics/AcceptanceCriteriaVersion
RequestedRunClasses[]
EvidenceRefs[]
```

OperationalClassification must be REQUEST but runtime route itself is for non-authoritative validation. No production adoption authority included.

### SIM-TRD-001 Validation Evidence

Payload:

```text
ExperimentId
SimulationEvidenceId
CandidateArtifactIds[]
BaselineArtifactIds[]
RunIds[]
ScenarioCoverage
FidelityResult
ReproducibilityResult
MetricResults[]
FailureFindings[]
StatisticalValidityResult
OracleVersions[]
EvidenceArtifactDigests[]
Recommendation = BETTER | WORSE | INCONCLUSIVE | UNSAFE | INVALID_EVIDENCE
```

Recommendation is not production approval.

PMA-SIM-001/SIM-PMA-001 and GRD-SIM-001/SIM-GRD-001 follow the same exact evidence separation using provider/Guardian-specific payload fields.

## 18. Resource Demand Report Schema

All `*-RSC-001` families use one canonical schema owned by APP-RSC candidate, populated by the constituent Application:

```text
ResourceDemandReportId
ApplicationId
ReportVersion
FoundationResourceEpochRef
CurrentEffectiveAllocationRefs[]
For each ResourceClass:
  MeasuredUsage
  MinimumSafe
  DesiredNormal
  MaximumUseful
  ReclaimableNow
  ReclaimabilityClass
  CheckpointCost
  DegradationOptions[]:
     OptionId
     QuantityReleased
     BusinessEffectClass
     ConsequenceSeverity
     ApplyDeadline
     RecoveryCost
     RequiresCheckpoint
  ActiveObligationRefs[]
  ConsequenceOfStarvation
EffectiveAt
ExpiresAt
EvidenceRefs[]
```

The schema contains needs/effects only. It cannot encode `FoundationTechnicalCriticality=true` as an Application assertion.

## 19. FSARM Resource Coordination Directive Schema

All `RSC-*-001` families use:

```text
ResourcePlanId
ActionSequence
CoordinatorApplicationId = APP-RSC
CoordinatorEpoch
CoordinationEnvelopeId/Version
FoundationResourceEpochRef
TargetApplicationId
ResourceClass
Action
CurrentConfirmedEffectiveQuantity
TargetEffectiveQuantity
RequiredReleaseOrGainQuantity
DegradationOptionId?          // when invoking a target-declared degradation option
EffectiveBy
ExpiresAt
IdempotencyKey
SupersedesActionRef?
ReasonCodes[]
EvidenceRefs[]
```

Target rules:

- reject stale CoordinatorEpoch;
- reject mismatched Foundation resource epoch/envelope;
- reject amount outside declared action semantics/hard Foundation bounds;
- idempotently re-ack duplicate identical action;
- conflicting same action identity => integrity failure;
- target applies its own business degradation mechanism and returns effect outcome.

## 20. Resource Effect Outcome Schema

All `*-RSC-002` families:

```text
ResourcePlanId
ActionSequence
TargetApplicationId
Outcome = ACKNOWLEDGED | APPLYING | EFFECT_CONFIRMED | PARTIAL | REJECTED | FAILED | RECONCILIATION_REQUIRED
BeforeUsage/EffectiveQuantity
AfterUsage/EffectiveQuantity
ConfirmedReleasedQuantity
CheckpointRef?
AffectedWorkloadIds[]
ReasonCodes[]
EvidenceRefs[]
```

APP-RSC cannot count `ACKNOWLEDGED` as released capacity where the action requires effect confirmation.

## 21. Effective Resource State Projection — RSC-ALL-001

Payload:

```text
ProjectionId
CoordinatorEpoch
CoordinationEnvelopeId
FoundationResourceEpochRef
PerApplicationEffectiveState[]:
  ApplicationId
  ResourceClass
  ConfirmedEffectiveQuantity
  MinimumSafe
  CurrentPressureClass
  ActivePlanRefs[]
PendingFoundationRequestRefs[]
CurrentRemainingDeficitByClass[]
EffectiveAt
ExpiresAt
EvidenceRefs[]
```

This is APP-RSC effective coordination truth, not Foundation authoritative grant truth.

## 22. Event vs Command Rules

A business state change may emit an Application event projection for observation. A consumer SHALL NOT reconstruct command authority from an event.

Examples:

```text
GuardianIncidentQualifiedEvent != ProtectionDirective
DataQualityDegradedEvent != ProviderRouteProtectionDirective
ResourcePressureProjection != ResourceCoordinationDirective
SimulationPassEvent != ProductionPromotionDecision
```

## 23. Replay Rules

Replay of a previously valid operational command/event through a REPLAY/SIMULATION route is non-authoritative.

Operational idempotency windows are not a reason to accept replay traffic as current authority.

Every consumer checks classification before business action.

## 24. Expiry / Freshness Defaults

No single global TTL is valid for all business contracts.

Each family defines a profile-based maximum age/expiry. Hard rules:

- protection directives carry explicit effective/expiry or revocation semantics;
- resource demand/state projections must expire when the Foundation/resource epoch or report freshness is no longer valid;
- data observations use DataProduct-specific freshness;
- state projections cannot be treated as current after expiry;
- validation evidence is immutable/non-expiring as historical evidence but never current operational state.

## 25. Route Declaration Matrix

Every family above requires one declared producer->consumer route in the Application manifests, except `RSC-ALL-001` which expands to one route per constituent consumer.

No wildcard `FSATS.* -> FSATS.*` route is permitted.

The route verifier SHALL prove:

- exact producer/consumer IDs;
- one intended business direction;
- schema family/version compatibility;
- required authority/security class;
- operational vs non-authoritative classification;
- no undeclared reverse route implied;
- no direct internal Application transport.

## 26. Failure Reason Code Namespace

Cross-Application contract failures use stable namespaced reason codes:

```text
CONTRACT_SCHEMA_UNKNOWN
CONTRACT_SCHEMA_INCOMPATIBLE
CONTRACT_REQUIRED_FIELD_MISSING
CONTRACT_IDENTITY_MISMATCH
CONTRACT_PRODUCER_NOT_ALLOWED
CONTRACT_CONSUMER_NOT_ALLOWED
CONTRACT_AUTHORITY_MISSING
CONTRACT_AUTHORITY_INVALID
CONTRACT_SCOPE_MISMATCH
CONTRACT_EXPIRED
CONTRACT_STALE
CONTRACT_OPERATIONAL_CLASS_MISMATCH
CONTRACT_DUPLICATE_CONFLICT
CONTRACT_SUPERSEDED
CONTRACT_REPLAY_NOT_AUTHORIZED
CONTRACT_FOUNDATION_ROUTE_UNAVAILABLE
CONTRACT_SECURITY_VALIDATION_FAILED
CONTRACT_EVIDENCE_INSUFFICIENT
```

Business-specific reason codes are added by the owning Application namespace.

## 27. Contract Verification Families

Verifier SHALL cover at least:

1. unique family IDs;
2. exact producer/consumer ownership;
3. no wildcard route;
4. required schema fields/types/units;
5. compatibility behavior;
6. operational-class fail closed;
7. expired/stale rejection;
8. authority/scope validation;
9. same ID/different digest conflict;
10. correction/supersession preservation;
11. observation/event cannot become command authority;
12. transport delivery cannot become business effect;
13. simulation/replay cannot affect operational state;
14. Guardian command effects tracked separately;
15. FSARM report != grant;
16. FSARM ACK != capacity reclaim confirmation;
17. Data Product hard quality states preserved;
18. no raw provider payload cross-App route;
19. no business payload direct database/shared-memory path;
20. historical 43-contract baseline reconciliation remains explicitly pending file 20 rather than silently declared equivalent.
