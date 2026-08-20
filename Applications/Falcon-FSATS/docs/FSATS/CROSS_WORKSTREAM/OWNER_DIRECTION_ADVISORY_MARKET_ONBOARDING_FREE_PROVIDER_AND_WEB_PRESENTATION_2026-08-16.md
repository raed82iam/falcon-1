# FSATS Owner Direction — Advisory Market Onboarding, Free-Only Provider Discovery, and Shared Web Presentation

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Status:** `OWNER_DIRECTION_RECORDED / PLANNING_AND_CROSS_WORKSTREAM_SEMANTICS_ONLY`  
**Part 8 Authority:** `NOT_AUTHORIZED`  
**Runtime Authority:** `NOT_GRANTED`  
**Provider Connectivity Authority:** `NOT_GRANTED`  
**Shared Web Write Authority:** `NOT_GRANTED`

## 1. Purpose

This record preserves the Project Owner's current direction for future FSATS market onboarding where a market is intentionally operated as an advisory-only market rather than an execution market, and defines the Application-owned semantics that Shared Falcon Web must be able to consume and render.

The immediate motivating example is the Saudi market, but the model is generic and shall not hard-code Saudi-specific behavior into Falcon's architecture.

This record does not implement a market, activate a provider, create a runtime route, create a credential, authorize Part 8, authorize deployment, or modify Shared Web-owned files.

## 2. Owner Intent — Advisory-Only Market

When the Owner directs Falcon to add a market with no intraday trading, and the intended role is advisory opportunity discovery rather than execution or trade follow-up, the intended market operating profile is:

```text
MarketOperatingMode = ADVISORY_ONLY
IntradayOpportunityGeneration = DISABLED
Execution = NONE
PositionTrackingForAdvisoryOpportunities = NONE
OpportunityFollowUp = NONE
SupportedOpportunityHorizons = DAILY / WEEKLY / MONTHLY
```

For this mode, FSATS may analyze the market and provide opportunities/recommendations on the supported horizons. Once an advisory opportunity is presented, FSATS is not responsible for determining whether the user acted on it, whether an order was submitted, whether a target was reached, or whether a position was closed.

Mandatory distinctions:

```text
OPPORTUNITY_PROPOSAL != TRADE
ADVISORY_OPPORTUNITY != EXECUTION_REQUEST
USER_ACTION_AFTER_ADVISORY = OUTSIDE_FSATS_ADVISORY_OPPORTUNITY_RESPONSIBILITY
ADVISORY_ONLY != MANUAL_EXECUTION
NO_INTRADAY != NO_ANALYSIS
```

## 3. Delayed Data Suitability

For an `ADVISORY_ONLY` market restricted to `DAILY`, `WEEKLY`, and `MONTHLY` opportunity horizons, a data source delayed by up to 15 minutes may be considered suitable for that intended horizon if its completeness, quality, history, market coverage, timestamps, terms, and required fields are otherwise sufficient.

A 15-minute delayed source shall not be represented as real-time and shall not be used to silently enable intraday opportunity behavior.

```text
DELAY_UP_TO_15_MINUTES + DAILY/WEEKLY/MONTHLY = ELIGIBLE_FOR_SUITABILITY_REVIEW
DELAY_UP_TO_15_MINUTES != AUTOMATIC_DATA_FITNESS
DELAYED_DATA != REAL_TIME_DATA
DELAYED_DATA_ALLOWED_FOR_ADVISORY_HORIZONS != INTRADAY_ALLOWED
DATA_DELAY_MUST_BE_DISCLOSED = TRUE
```

## 4. Provider Discovery Policy — Free Only

For the current Owner-directed provider-discovery policy, Falcon shall search for and consider only providers whose required service can be used without a paid subscription for the required use case.

```text
PROVIDER_DISCOVERY_POLICY = FREE_ONLY
PAID_PROVIDER_REQUIRED = REJECT
PAID_PLAN_REQUIRED = REJECT
FREE_TIER = CANDIDATE_IF_SUFFICIENT
FREE_API_REQUIRING_API_KEY = ALLOWED_CANDIDATE
TRIAL_THAT_REQUIRES_FUTURE_PAYMENT_FOR_CONTINUED_REQUIRED_USE = REJECT
```

Provider evaluation shall still consider, where applicable:

- market/instrument coverage;
- data fields and history;
- daily/weekly/monthly suitability;
- delay and timestamp semantics;
- quality and completeness;
- rate limits and quotas;
- terms/attribution obligations;
- API or file/web access method;
- provider account identity;
- credential-reference requirement;
- availability and fallback suitability.

If no suitable free provider exists, Falcon shall report `NO_SUITABLE_FREE_PROVIDER_FOUND`. It shall not silently select a paid provider.

## 5. Free Provider Requiring an API Key

If Falcon discovers a suitable free provider that requires an API key, discovery may continue to the point of a bounded Owner action request, but the missing key does not become permission or connectivity.

The Owner-facing business request shall contain metadata only and must never contain secret bytes.

Example human-facing message:

> `Please create/add the free API key for ExampleProvider.`

Conceptual status:

```text
ProviderCandidateStatus = WAITING_FOR_OWNER_CREDENTIAL
CredentialType = API_KEY
CredentialEntryMode = SECURE_ONLY
PlaintextCredentialInChat = PROHIBITED
```

The Web-facing Owner action projection shall be able to carry at least:

```text
requestId
originatingApplicationId
marketId
providerId
providerDisplayName
actionType = ADD_PROVIDER_CREDENTIAL
message
reason
providerHelpOrSignupUrl = OPTIONAL
credentialType = API_KEY
providerCostClass = FREE
status = ACTION_REQUIRED | WAITING_FOR_OWNER_CREDENTIAL
secureEntryRequired = true
chatEntryProhibited = true
```

If a credential is later provisioned through an authorized secure mechanism, ordinary Application/Web state may consume only a governed credential reference and non-secret status, for example:

```text
credentialReferenceId
credentialStatus = AVAILABLE | VALIDATED | REJECTED | EXPIRED | REVOKED
```

Mandatory distinctions:

```text
OWNER_ACTION_MESSAGE != CREDENTIAL
API_KEY_VALUE != CHAT_PAYLOAD
API_KEY_VALUE != ORDINARY_WEB_PAYLOAD
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
FREE_PROVIDER_DISCOVERED != PROVIDER_ROUTE_AUTHORIZED
OWNER_PROVIDED_CREDENTIAL != PROVIDER_CONNECTIVITY_AUTHORIZED
```

The exact secure credential-entry and Foundation secret-provider runtime mechanism remains governed separately and is not created by this planning record.

## 6. Application-to-Web Market Projection

Shared Web shall not be required to hard-code new markets. Future Application-owned market onboarding semantics should allow FSATS to expose a dynamic market projection containing, at minimum, the following conceptual fields:

```text
contract = FSATS.WebMarketProfileProjection.v1
marketId
marketCode
displayName
operatingMode = ADVISORY_ONLY | other_governed_modes
supportedOpportunityHorizons[]
intradayOpportunityEnabled
executionCapability
positionTrackingForAdvisoryOpportunities
opportunityFollowUpEnabled
availability
reason
marketCalendarReference
marketRulesReference
strategyCatalogReference
schoolCatalogReference
revision
```

For the Saudi advisory example the expected values are:

```text
operatingMode = ADVISORY_ONLY
supportedOpportunityHorizons = [DAILY, WEEKLY, MONTHLY]
intradayOpportunityEnabled = false
executionCapability = NONE
positionTrackingForAdvisoryOpportunities = false
opportunityFollowUpEnabled = false
```

This is a proposed planning contract identity for future governed implementation. It is not a currently materialized executable runtime contract.

## 7. Schools and Strategies Sent to Web

Trading remains the authoritative Application owner of School/Strategy catalog truth and applicability semantics. Shared Web shall render the dynamic catalog rather than hard-code Falcon product truth.

Existing public semantic surfaces remain the preferred compatibility basis:

```text
FSATS.WebStrategyCatalogRequest.v1
FSATS.WebStrategyCatalogProjection.v1
FSATS.WebStrategyCatalogUpdate.v1
```

A newly onboarded market shall therefore be represented to Web by market identity/profile plus the applicable current School/Strategy catalog state, including explicit applicability/unavailability reasons.

Mandatory distinctions remain:

```text
TRADING_CATALOG = AUTHORITATIVE_DISCOVERY_SOURCE
WEB_SELECTOR_OPTIONS != HARD_CODED_FALCON_PRODUCT_TRUTH
CATALOG_PRESENT != APPLICABLE_TO_CURRENT_MARKET_OR_ASSET
CATALOG_AVAILABLE != STRATEGY_ACTIVATED
STRATEGY_VISIBLE != STRATEGY_ACTIVATED
SCHOOL_VISIBLE != SCHOOL_ACTIVATED
WEB_PRESENTATION != STRATEGY_OR_SCHOOL_LOGIC
```

## 8. Application-to-Web Chart Source Projection

FSATS may provide Shared Web with information describing a discovered presentation source/URL and its intended purpose. The URL is metadata describing a source candidate; it is not a runtime route grant.

Conceptual projection:

```text
contract = FSATS.WebMarketChartSourceProjection.v1
marketId
sourceId
providerId
providerDisplayName
sourceUrl
purpose = CHART_PRESENTATION_ONLY
accessType = WEB_URL | API | FILE
costClass = FREE
dataMode = DELAYED | END_OF_DAY
delayMinutes
realTime = false
supportedDisplayHorizons[]
attributionText = OPTIONAL
disclosureText
termsReference = OPTIONAL
availability
reason
revision
```

For a 15-minute delayed Saudi-market source, Web shall be able to visibly render a disclosure such as:

> `البيانات متأخرة 15 دقيقة`

or an equivalent localized wording that preserves the exact delay truth.

Mandatory distinctions:

```text
CHART_SOURCE_URL != WEB_PROVIDER_ROUTE_AUTHORITY
CHART_SOURCE_URL != FSAPMA_OPERATIONAL_ROUTE
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
SAME_PROVIDER != SAME_AUTHORITY
SAME_URL != SAME_AUTHORITY
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
```

Shared Web remains free to implement its own presentation adapter under Web ownership and its separately governed provider-destination authority. FSATS does not write or operate Web internals.

## 9. Web Rendering Expectations

The Application-side contract semantics require Shared Web to be able to represent, without inventing business truth:

1. a newly available market dynamically;
2. an `ADVISORY_ONLY` market badge/state;
3. enabled advisory horizons: Daily, Weekly, Monthly;
4. intraday opportunity controls as unavailable for this market mode;
5. no execution or advisory-position-follow-up UI implied by an advisory opportunity;
6. dynamic School/Strategy catalog and applicability state supplied by Trading;
7. chart source/provider attribution where applicable;
8. explicit delayed-data disclosure, including known delay minutes;
9. source unavailable/stale/unsupported states without fabricating current data;
10. Owner-only action-required status when a suitable free provider needs an API key;
11. a secure configure/add-credential action surface without accepting secret bytes in ordinary chat;
12. explicit `NO_SUITABLE_FREE_PROVIDER_FOUND` state when applicable.

## 10. Owner Communication Flow

The intended cross-workstream flow is:

```text
OWNER MARKET INTENT
-> FSATS MARKET / PROVIDER DISCOVERY
-> FREE-ONLY FILTER
-> PROVIDER SUITABILITY EVALUATION
-> IF FREE PROVIDER NEEDS API KEY:
     FSATS OWNER-ACTION PROJECTION
     -> SHARED WEB OWNER-ONLY PRESENTATION
     -> SECURE CREDENTIAL CONFIGURATION PATH
     -> GOVERNED CREDENTIAL REFERENCE / STATUS ONLY
-> MARKET PROFILE + SCHOOL/STRATEGY CATALOG + CHART SOURCE METADATA
-> SHARED WEB DYNAMIC PRESENTATION
-> SEPARATE GOVERNED OWNER APPROVAL / RUNTIME AUTHORITY WHERE REQUIRED
```

This flow does not authorize automatic activation.

## 11. Current Authority Boundary

Current state remains:

```text
PART 0 THROUGH PART 7 = OWNER_ACCEPTED_AND_CLOSED
PART 8 = NOT_AUTHORIZED
RUNTIME = NOT_AUTHORIZED
PROVIDER CONNECTIVITY = NOT_AUTHORIZED
BROKER CONNECTIVITY = NOT_AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT_AUTHORIZED
```

FCR-0082 remains Application-held for separately authorized canonical runtime binding. FCR-0013 remains the Foundation-owned future FSAPMA provider-egress dependency. Existing Web chart/provider and School/Strategy contracts remain governed by their current FCRs.

## 12. Final Invariants

```text
MARKET_PROFILE_DECLARED != MARKET_ACTIVATED
PROVIDER_DISCOVERED != PROVIDER_AUTHORIZED
FREE_PROVIDER != TRUSTED_PROVIDER
FREE_PROVIDER != SUITABLE_PROVIDER
SUITABILITY_PASS != CONNECTIVITY_AUTHORITY
OWNER_APPROVAL != FOUNDATION_RUNTIME_ROUTE_AUTHORITY
DELAY_DISCLOSURE != REAL_TIME
ADVISORY_RECOMMENDATION != EXECUTION
WEB_DISPLAY != FSATS_OPERATIONAL_TRUTH
WEB_DISPLAY != EXECUTION_AUTHORITY
```

## 13. Current Disposition

```text
OWNER_DIRECTION = RECORDED
APPLICATION_TO_WEB_SEMANTICS = DEFINED_FOR_PLANNING_AND_COMPATIBILITY_REVIEW
IMPLEMENTATION = NOT_AUTHORIZED_BY_THIS RECORD
RUNTIME = NOT_AUTHORIZED
WEB IMPLEMENTATION CHANGE = NOT PERFORMED BY APPLICATION
NEXT CROSS_WORKSTREAM ACTION = WEB COMPATIBILITY / PRESENTATION REVIEW THROUGH FCR
```
