# FSATS Web Analysis and Strategy Contracts v1

**Status:** `APPLICATION_PUBLIC_CONTRACT_MATERIALIZED / WEB_BINDING_PENDING`  
**Runtime Authority:** `NOT_GRANTED`  
**Provider/Broker Connectivity Authority:** `NOT_GRANTED`

## 1. Purpose

This contract materializes the Application-owned public payload boundary used when Shared Web customers request Trading analysis, detailed asset analysis, School/Strategy applicability, Strategy catalog discovery or Trading Risk results.

Shared Web may independently acquire presentation-only market data through its own separately governed provider routes. That presentation data is not an input to these FSATS analysis contracts.

This document shall be read together with current Part 2 cross-workstream clarification and live FCR-0127, FCR-0128 and FCR-0130 current state.

## 2. Canonical current v1 identities

```text
FSATS.WebOnDemandAnalysisRequest.v1
FSATS.WebOnDemandAnalysisResult.v1
FSATS.WebDetailedAssetAnalysisProjection.v1

FSATS.WebStrategyCatalogRequest.v1
FSATS.WebStrategyCatalogProjection.v1
FSATS.WebStrategyCatalogUpdate.v1
```

The earlier R3 source markers:

```text
FSATS.WebOnDemandAnalysisProjection.v1
FSATS.WebOnDemandAnalysisCommand.v1
```

are retained in source only as historical/source-compatibility markers. They are not canonical current Web binding identities and create no additional route or authority.

## 3. Ownership

```text
WEB_OWNS_CUSTOMER_INTERACTION_AND_PRESENTATION = YES
TRADING_OWNS_INSTRUMENT_RESOLUTION_ANALYSIS_SCHOOL_STRATEGY_AND_RISK_SEMANTICS = YES
FSAPMA_OWNS_FSATS_OPERATIONAL_EXTERNAL_MARKET_DATA_ACQUISITION = YES
WEB_PRESENTATION_DATA -> FSATS_ANALYSIS_INPUT = PROHIBITED
```

A Web request identifies the requested business result. It does not provide or select the external data source used by FSATS.

## 4. Canonical on-demand request

`WebOnDemandAnalysisRequest` materializes the FCR-0127 request semantics:

```text
RequestId
CorrelationId
RequestingApplicationId = SHARED_WEB
RequestedInstrumentReference
MarketOrVenueHint?
AssetClassHint?
AnalysisIntent
RequestedAt
EntitlementReference?
BrokerAccountScope?        // only when account-aware analysis needs exact broker-account context
```

The executable constructor requires `RequestingApplicationId == SHARED_WEB`, a non-empty requested instrument reference, and a non-empty bounded `AnalysisIntent`.

It intentionally exposes no provider, provider-account, API-instance, endpoint, URL, credential secret, API key, raw quote, raw candle, raw order book, or raw Web market-data payload control.

Instrument hints are disambiguation context only. Web does not silently choose a market, venue, asset class or instrument when Trading reports ambiguity.

## 5. Instrument resolution and serviceability

Trading remains owner of Trading market/instrument resolution, admitted-market context and serviceability.

Canonical result states are:

```text
COMPLETED
PARTIAL
UNAVAILABLE
UNSUPPORTED
NEEDS_CLARIFICATION
REJECTED
```

For `NEEDS_CLARIFICATION`:
- no resolved instrument may be claimed;
- no analysis projection may be claimed;
- bounded candidate instrument identities are required so Web can ask the customer to clarify.

For `COMPLETED`:
- an exact resolved instrument is required;
- an analysis projection is required;
- the input completeness summary must be `COMPLETE`.

The initial admitted market scope remains governed separately. An on-demand request never silently expands market scope.

## 6. Canonical on-demand result

`WebOnDemandAnalysisResult` materializes:

```text
RequestId
CorrelationId
AnalysisResultId
ResolvedInstrumentIdentity?
AnalysisIntent
ResultState
AnalysisProjection?
AsOfTime
InputTruthFreshnessSummary
ConfidenceOrStrength?
Limitations[]
ReasonCode?
ClarificationCandidates[]
```

Optional confidence remains nullable. No code path is allowed to manufacture confidence merely because the public payload has a field for it.

Controlling distinctions:

```text
ON_DEMAND_ANALYSIS != SILENT_UNIVERSE_MUTATION
ON_DEMAND_ANALYSIS != CANDIDATE_UNIVERSE_ADMISSION
ON_DEMAND_ANALYSIS != MANAGED_INSTRUMENT_ADMISSION
ON_DEMAND_ANALYSIS != STRATEGY_ACTIVATION
ON_DEMAND_ANALYSIS != CAPITAL_RESERVATION
ON_DEMAND_ANALYSIS != ORDER_INTENT
ON_DEMAND_ANALYSIS != EXECUTION_AUTHORITY
ANALYSIS_RESULT != TRADE_AUTHORIZATION
```

## 7. Detailed asset-analysis projection

`WebDetailedAssetAnalysisProjection` materializes `FSATS.WebDetailedAssetAnalysisProjection.v1` as a semantic view inside the existing on-demand result family, not as a new route or authority class.

Top-level shape:

```text
ResolvedInstrumentIdentity
AnalysisResultId
AsOfTime
OverallTruthState
InputTruthFreshnessSummary
HorizonViews[]
StrategyViews[]
SchoolViews[]
Synthesis
```

### 7.1 Horizon views

Each `WebDetailedHorizonView` carries:

```text
HorizonId
ResultState
Conclusion
MaterialLevelsOrTargets[]
ConfidenceOrStrength?
Limitations[]
EvidenceOrSourceOutputReferences[]
```

`MaterialLevelsOrTargets[]` may be empty and `ConfidenceOrStrength` may be null. Missing target/confidence is preserved as missing and shall not be invented.

### 7.2 Strategy views

Each `WebDetailedStrategyView` carries:

```text
StrategyId
ApplicabilityState
ResultState
Conclusion
MaterialLevelsOrTargets[]
ConfidenceOrStrength?
AsOfTime
TruthState
FreshnessState
Limitations[]
EvidenceOrSourceOutputReferences[]
```

### 7.3 School views

Each `WebDetailedSchoolView` carries:

```text
SchoolId
ApplicabilityState
ResultState
PerspectiveOrConclusion
MaterialLevelsOrTargets[]
ConfidenceOrStrength?
AsOfTime
TruthState
FreshnessState
Limitations[]
EvidenceOrSourceOutputReferences[]
```

### 7.4 Synthesis

`WebDetailedAnalysisSynthesis` carries:

```text
SynthesisState
Agreements[]
Disagreements[]
UnresolvedConflicts[]
BoundedCombinedExplanation
ContributingOutputReferences[]
Limitations[]
```

Executable fail-closed rules prevent:

```text
STALE_OR_NONCURRENT_INPUT -> CURRENT_OVERALL_TRUTH
PARTIAL_INPUTS -> COMPLETE_SYNTHESIS
MATERIAL_DISAGREEMENT_OR_UNRESOLVED_CONFLICT -> UNQUALIFIED_COMPLETE_SYNTHESIS
```

Therefore:

```text
SYNTHESIS != CONSENSUS
DISAGREEMENT != ERROR
MISSING_TARGET != INVENT_TARGET
MISSING_CONFIDENCE != INVENT_CONFIDENCE
STALE_SOURCE != CURRENT_SYNTHESIS
PARTIAL_INPUTS != COMPLETE_RESULT
```

## 8. Risk and Strategy applicability invariants

```text
BEST_STRATEGY_CANDIDATE != STRATEGY_ACTIVATION
ANALYSIS_CONFIDENCE != EXECUTION_AUTHORITY
GENERAL_RISK != ACCOUNT_AWARE_RISK
ACCOUNT_AWARE_RISK_REQUIRES_EXACT_BROKER_ACCOUNT_SCOPE
```

`WebTradingRiskProjection` rejects:
- `IsAccountAware=true` with no exact account scope;
- `IsAccountAware=false` while an account scope is supplied.

For the Strategy catalog:

```text
CATALOG_PRESENT + NOT_APPLICABLE_TO_CURRENT_ASSET
-> VISIBLE_DISABLED_WITH_REASON
```

`WebStrategyCatalogItem` rejects a `NotApplicable` item that is hidden or enabled.

`WebStrategyCatalogUpdate` enforces exact update lineage:

```text
ORDINARY -> no correction/supersession id
CORRECTION -> correctsUpdateId required; supersedesUpdateId absent
SUPERSESSION -> supersedesUpdateId required; correctsUpdateId absent
```

## 9. Web truth and presentation boundary

```text
AI_CHAT_EXPLANATION != ANALYSIS_TRUTH_OWNER
WEB_PRESENTATION != STRATEGY_OR_SCHOOL_LOGIC
DETAILED_ANALYSIS != STRATEGY_ACTIVATION
```

Trading/Application owns authoritative Strategy/School outputs and synthesis semantics. Shared Web may explain and format the returned projection but shall not reconstruct Strategy/School logic, manufacture targets/confidence, suppress material disagreement, or upgrade stale/partial/unknown truth.

Shared Web presentation-only provider access remains a separate Web authority path:

```text
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
SAME_PROVIDER != SAME_AUTHORITY
SAME_URL != SAME_AUTHORITY
SAME_URL != SHARED_CREDENTIAL
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
```

## 10. Wire-format policy

Current v1 public payload serialization is materialized by `WebContractSerialization.CreateV1Options()`.

Mandatory wire rules:
- JSON property names are lower camel case;
- enums are uppercase snake-case semantic strings, never ordinals;
- integer enum fallback is rejected;
- timestamps remain JSON ISO-8601/RFC3339-compatible `DateTimeOffset` strings;
- decimal values remain JSON numbers;
- unavailable numeric truth remains nullable rather than fabricated zero.

Examples:

```text
LAST_KNOWN
NEEDS_CLARIFICATION
PARTIALLY_FILLED
CANCEL_REQUESTED
UNKNOWN_BROKER_OUTCOME
NOT_APPLICABLE
```

## 11. Executable enforcement and verifier source

Current Application-side enforcement is materialized in:

- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/WebAnalysisAndStrategyContracts.cs`
- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/TradingContracts.cs`
- `applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/WebContractSerialization.cs`
- `applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Domain/ProviderDomain.cs`
- `applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Application/OperationalConfiguration.cs`
- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/CrossPartSynchronizationAdversarialChecks.cs`
- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/DetailedAnalysisContractAdversarialChecks.cs`
- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/WebContractSerializationAdversarialChecks.cs`

The adversarial source challenges invalid account-aware Risk, invalid NotApplicable selectors, ambiguous-result truth laundering, stale/partial synthesis upgrades, disagreement suppression, invented target/confidence defaults, portfolio pagination/lineage misuse, provider-route identity mismatch, request raw-data/provider-control smuggling, and noncanonical wire serialization.

Presence of verifier source does not itself prove executable PASS. Exact executable validation must be evidenced separately.

## 12. Non-grant

This materialization creates no executable transport route, Foundation egress authority, provider/broker connectivity, credential authority, Paper/Shadow/Tiny-Live/Live authority, Strategy activation, trade authorization, universe admission or deployment authority.
