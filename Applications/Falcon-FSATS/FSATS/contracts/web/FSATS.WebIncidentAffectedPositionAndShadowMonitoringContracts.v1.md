# FSATS Web Incident Affected-Position, Affected-Order, and Emergency Shadow-Monitoring Contracts v1

**Status:** `APPLICATION_PUBLIC_CONTRACT_MATERIALIZED / WEB_BINDING_PENDING`  
**Source Request:** `FCR-0201`  
**Runtime Authority:** `NOT_GRANTED`  
**Paper/Shadow/Tiny-Live/Live Authority:** `NOT_GRANTED`

## 1. Purpose

This contract defines the Application-owned business semantics Shared Web may present during a broker/API incident affecting an exact broker account and its positions/orders.

It separates three authoritative surfaces:

1. Trading/Application affected-position follow-up and protection classification;
2. Trading/Application affected-order follow-up and broker-order truth classification;
3. FSTSimA emergency non-Live shadow-monitoring evidence.

Shared Web owns presentation and customer interaction. It does not classify protection/order truth, decide follow-up, invent monitoring times, recompute FSTSimA evidence, or convert simulator estimates into broker truth.

## 2. Identity boundary

FSATS still owns no customer/user principal.

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
EXACT_APPLICATION_SCOPE = BrokerId + BrokerAccountId + Environment
WEB_OWNS_CUSTOMER_TO_BROKER_ACCOUNT_MAPPING = YES
```

The public projections cross the FSATS/Web boundary with exact broker-account scope. Shared Web maps that scope to the authenticated customer/contact context it owns.

## 3. Canonical contract identities

Trading/Application:

```text
FSATS.WebAffectedPositionFollowupProjection.v1
FSATS.WebAffectedPositionFollowupUpdate.v1
FSATS.WebAffectedOrderFollowupProjection.v1
FSATS.WebAffectedOrderFollowupUpdate.v1
```

FSTSimA:

```text
FSATS.WebEmergencyShadowMonitoringRequest.v1
FSATS.WebEmergencyShadowMonitoringProjection.v1
FSATS.WebEmergencyShadowMonitoringUpdate.v1
```

Contract materialization does not create a transport route or runtime authority.

## 4. Affected-position follow-up

For each affected position, Application supplies:

```text
ProjectionId
IncidentId
Account = BrokerId + BrokerAccountId + Environment
Position
Instrument
LastBrokerConfirmedAt
ProtectionState
FollowupRequirement
FollowupReasonCode
OrderedActions[]
EmergencyShadowMonitoringActive
ShadowCaseId?
IncidentState
TruthState
FreshnessState
EvidenceReference
AsOfTime
```

### 4.1 Protection-state classification

```text
BROKER_CONFIRMED_PROTECTED
PROTECTION_UNKNOWN_OR_AMBIGUOUS
INTENTIONALLY_RETAINED_WITHOUT_CURRENT_BROKER_PROTECTION
UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION
RECONCILIATION_REQUIRED
NOT_APPLICABLE
```

Executable truth rules:

```text
INTENTIONALLY_RETAINED_WITHOUT_CURRENT_BROKER_PROTECTION
!=
UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION

BROKER_CONFIRMED_PROTECTED
-> TruthState = CURRENT
-> FreshnessState = CURRENT

UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION
-> FollowupRequirement = REQUIRED
```

`BROKER_CONFIRMED_PROTECTED` cannot be emitted from stale/last-known evidence merely because protection existed before the outage.

`INTENTIONALLY_RETAINED_WITHOUT_CURRENT_BROKER_PROTECTION` is legal only when Application-owned business semantics explicitly classify that condition and preserve its reason/evidence. Absence of a visible protective order alone cannot create this state.

### 4.2 Follow-up classification

```text
NONE
RECOMMENDED
REQUIRED
```

A `REQUIRED` projection must contain at least one required ordered action. `NONE` cannot contain a required action.

Allowed actions include:

```text
VERIFY_BROKER_ACCOUNT_STATE
VERIFY_OPEN_POSITION
VERIFY_WORKING_ORDERS
VERIFY_PROTECTION_ORDERS
RESOLVE_AMBIGUOUS_SUBMISSION
TAKE_PROTECTIVE_ACTION_AT_BROKER_IF_NEEDED
REPAIR_GOVERNED_CREDENTIAL_PATH
AWAIT_FALCON_RECONCILIATION
CONTACT_SUPPORT
```

## 5. Affected-order follow-up

An incident may contain affected or ambiguous orders even when no live position identity is currently provable. Application therefore exposes a separate order projection rather than inventing a position.

Each affected order carries:

```text
ProjectionId
IncidentId
Account = BrokerId + BrokerAccountId + Environment
Order
Instrument
LastBrokerConfirmedAt?
OrderTruthState
FollowupRequirement
FollowupReasonCode
OrderedActions[]
TruthState
FreshnessState
EvidenceReference
AsOfTime
```

Exact order-truth states:

```text
BROKER_CONFIRMED_WORKING
BROKER_CONFIRMED_REJECTED
BROKER_CONFIRMED_PARTIALLY_FILLED
BROKER_CONFIRMED_FILLED
BROKER_CONFIRMED_CANCELLED
OUTCOME_UNKNOWN_OR_AMBIGUOUS
RECONCILIATION_REQUIRED
```

Executable rules:

```text
BROKER_CONFIRMED_* -> CURRENT truth + CURRENT freshness + broker-confirmed timestamp
OUTCOME_UNKNOWN_OR_AMBIGUOUS -> FOLLOWUP != NONE
RECONCILIATION_REQUIRED -> FOLLOWUP != NONE
UNKNOWN != FAILED
UNKNOWN != SAFE_TO_RETRY
```

The order projection therefore preserves the FCR-0201 need to show affected positions **and orders** without collapsing them into one invented trade object.

## 6. Emergency FSTSimA shadow monitoring

When broker/API truth is lost and governed Application policy activates an emergency non-Live shadow case, FSTSimA may provide Web-displayable diagnostic evidence.

A shadow subject may be:
- an exact last-confirmed/known `PositionId`, or
- an ambiguous `SourceOrderId` for which no position identity is yet authoritative.

At least one of those identities is required. FSTSimA shall not invent a `PositionId` merely to represent an ambiguous order.

The shadow projection includes:

```text
ProjectionId
IncidentId
ShadowCaseId
BrokerId + BrokerAccountId + Environment
PositionId?
SourceOrderId?
InstrumentId
LastBrokerConfirmedAt
MonitoringStartedAt
MonitoringEndedAt?
ShadowState
ContainsExecutionAmbiguity
Scenarios[]
ProtectionClassificationProjectionReference?
AsOfTime
ProjectionTruth
FreshnessState
ProvenanceReference
EvidenceReference
```

A position-backed shadow requires a Trading/Application protection-classification projection reference. An order-only ambiguous shadow does not manufacture a position protection classification.

Web can display elapsed monitoring time from supplied start/end timestamps. It must not invent a business monitoring duration or infer that elapsed time changes authority.

### 6.1 Shadow lifecycle

```text
ACTIVE
RECONCILING
ENDED_RECONCILED
ENDED_UNRESOLVED
```

`ACTIVE` cannot have an end time. An ended state requires an explicit end timestamp.

### 6.2 Top-level shadow truth and freshness

The projection truth class is strongly typed and limited to non-broker diagnostic classes:

```text
ProjectionTruth = SIMULATOR | REPLAY | SYNTHETIC | TEST
FreshnessState = CURRENT | STALE | UNKNOWN | UNAVAILABLE
```

No `BROKER_CONFIRMED`, `LIVE`, or equivalent broker/account truth class exists in the FSTSimA projection truth enum. `CURRENT` freshness means only that the diagnostic projection is current relative to its own simulation/replay/test evidence. It does not upgrade the projection into broker truth.

```text
SHADOW_PROJECTION_TRUTH != BROKER_TRUTH
CURRENT_SHADOW_FRESHNESS != CURRENT_BROKER_ACCOUNT_TRUTH
```

### 6.3 Scenario semantics

```text
LAST_BROKER_CONFIRMED_POSITION
NOT_EXECUTED
PARTIALLY_EXECUTED
FULLY_EXECUTED
USER_REPORTED_STATE
```

When `ContainsExecutionAmbiguity=true`:
- `SourceOrderId` is mandatory;
- v1 requires explicit `NOT_EXECUTED`, `PARTIALLY_EXECUTED`, and `FULLY_EXECUTED` scenarios;
- those are alternative diagnostic cases, not probabilities or broker facts.

Scenario estimates remain nullable and may include quantity, market value, risk amount and a threshold state only when actually supported by simulation evidence.

Evidence truth remains explicitly:

```text
SIMULATOR_ESTIMATE
USER_REPORTED_INPUT
LAST_BROKER_CONFIRMED_SEED
```

Mandatory distinctions:

```text
SIMULATOR_ESTIMATE != BROKER_TRUTH
SHADOW_PROJECTION_TRUTH != BROKER_TRUTH
SHADOW_POSITION != CONFIRMED_LIVE_POSITION
USER_REPORTED_INPUT != BROKER_CONFIRMED_TRUTH
SHADOW_MONITORING != EXECUTION_CONFIRMATION
MARKET_PROVIDER_TRUTH != BROKER_ACCOUNT_TRUTH
```

## 7. Ownership split

FSTSimA does not become protection-state, order-truth or follow-up owner.

```text
FSTSIMA_SIMULATES_EXPOSURE_AND_RISK = YES
FSTSIMA_CLASSIFIES_CUSTOMER_FOLLOWUP = NO
FSTSIMA_OWNS_BROKER_PROTECTION_TRUTH = NO
FSTSIMA_OWNS_BROKER_ORDER_TRUTH = NO
TRADING_APPLICATION_OWNS_POSITION_ORDER_AND_FOLLOWUP_BUSINESS_CLASSIFICATION = YES
```

## 8. Automatic and on-demand interaction

Both modes are governed:

1. **Automatic update:** material changes to incident, affected-position, affected-order or shadow state may emit their corresponding update projections through an already-governed Web binding.
2. **On-demand read:** Shared Web may request current emergency shadow evidence by exact incident/account plus optional position/order filter through `FSATS.WebEmergencyShadowMonitoringRequest.v1`.

The executable request requires `RequestingApplicationId=SHARED_WEB`. Neither mode grants Web authority to create a shadow case, choose protection/order truth, alter scenarios or release restrictions.

## 9. Reconnect, reconciliation and end-state

Connectivity return does not itself restore risk-increasing authority.

```text
RECONNECT
-> AUTHENTICATE
-> FETCH ACCOUNT
-> FETCH ORDERS
-> FETCH FILLS
-> FETCH POSITIONS
-> FETCH PROTECTION STATE
-> RECONCILE LAST_CONFIRMED + AMBIGUOUS_REQUESTS + SHADOW_CASE
-> RESOLVE MATERIAL DIFFERENCES
-> SAFETY/RISK CHECK
-> RELEASE ACCOUNT-SCOPED RESTRICTION WHEN GOVERNED
```

Current status reaches Web through new Application/FSTSimA projections, never through Web inference.

```text
RECONNECT != RECOVERED
RECONNECT != INCIDENT_RESOLVED
SHADOW_ENDED != BROKER_TRUTH_RECONCILED
```

## 10. Web boundary

```text
WEB_DOES_NOT_CLASSIFY_POSITION_PROTECTION_STATE
WEB_DOES_NOT_CLASSIFY_ORDER_TRUTH
WEB_DOES_NOT_DECIDE_FOLLOWUP_REQUIRED
WEB_DOES_NOT_INVENT_SHADOW_MONITORING_TIMES
WEB_DOES_NOT_RECOMPUTE_FSTSIMA_ANALYSIS
WEB_PRESENTS_APPLICATION_SUPPLIED_SEMANTICS_ONLY
WEB_ACCOUNT_TO_CUSTOMER_MAPPING != FSATS_USER_IDENTITY
CUSTOMER_FOLLOWUP_REQUEST != EXECUTION_AUTHORITY
USER_CHECKED_ITEM != BROKER_VERIFIED_STATE
```

## 11. Executable materialization

Trading/Application:
- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/WebIncidentFollowupContracts.cs`
- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/WebIncidentOrderFollowupContracts.cs`

FSTSimA:
- `applications/FSATS/src/FSTSimA/Falcon.FSATS.FSTSimA.Contracts/WebEmergencyShadowMonitoringContracts.cs`

Adversarial verification source:
- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/IncidentShadowMonitoringAdversarialChecks.cs`
- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/IncidentProtectionTruthAdversarialChecks.cs`
- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/IncidentOrderFollowupAdversarialChecks.cs`

The shadow adversarial checks explicitly verify that top-level shadow truth exposes no broker/live truth class and that typed `ProjectionTruth`/`FreshnessState` cannot be replaced by arbitrary strings.

Verifier source presence does not itself prove executable validation.

## 12. Authority non-grant

This semantic materialization grants none of:
- broker/provider connectivity;
- Paper/Shadow/Tiny-Live/Live trading;
- FSTSimA operational-truth promotion;
- execution;
- incident release;
- Web business authority;
- deployment.
