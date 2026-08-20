# FSATS Owner Clarification — On-Demand Advisory Analysis and Provider Fetch

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Status:** `OWNER_CLARIFICATION_RECORDED / PLANNING_AND_CROSS_WORKSTREAM_SEMANTICS_ONLY`  
**Applies To:** `OWNER_DIRECTION_ADVISORY_MARKET_ONBOARDING_FREE_PROVIDER_AND_WEB_PRESENTATION_2026-08-16.md`  
**Runtime Authority:** `NOT_GRANTED`  
**Provider Connectivity Authority:** `NOT_GRANTED`

## 1. Purpose

This record preserves the Project Owner's clarification for advisory-only markets in the current personal Falcon release phase.

The current motivating example is the Saudi market. The behavior is request-driven advisory analysis. Falcon shall not autonomously scan the market and push opportunities merely because the market or provider exists.

This record does not authorize Part 8, runtime activation, provider connectivity, broker connectivity, deployment, or Shared Web implementation changes.

## 2. Advisory Request Flow

The intended business flow is:

```text
USER REQUEST
-> SHARED WEB
-> FSATS ON-DEMAND ANALYSIS REQUEST
-> FSATS DETERMINES REQUIRED ANALYSIS DATA
-> FSAPMA SELECTS SUITABLE GOVERNED FREE PROVIDER(S)
-> FSAPMA FETCHES ONLY THE DATA REQUIRED FOR THAT REQUEST
-> FSATS PERFORMS MARKET / ASSET ANALYSIS
-> FSATS RETURNS ANALYSIS RESULT
-> SHARED WEB PRESENTS RESULT
-> END
```

Supported examples include:

```text
USER: "Find opportunities in the Saudi market"
USER: "Analyze this Saudi stock"
```

The user remains free to act on or ignore the returned advisory result.

```text
USER_ACTION_AFTER_RESULT = OUTSIDE_FSATS_ADVISORY_RESPONSIBILITY
FSATS_EXECUTION = NONE
FSATS_POSITION_TRACKING_FOR_ADVISORY = NONE
FSATS_OPPORTUNITY_FOLLOW_UP = NONE
```

## 3. Provider API Consumption Mode

For this advisory-only request-driven mode, provider API consumption is on-demand.

```text
SAUDI_ADVISORY_PROVIDER_MODE = ON_DEMAND
BACKGROUND_MARKET_POLLING = DISABLED
CONTINUOUS_FSAPMA_API_CONSUMPTION = DISABLED
AUTONOMOUS_OPPORTUNITY_SCANNING_OR_PUSH = DISABLED
```

The normal FSAPMA provider fetch trigger is a valid FSATS analysis request that requires external market data.

```text
NO_ANALYSIS_REQUEST
-> NO_ANALYSIS_DATA_FETCH
-> NO_FSAPMA_PROVIDER_API_CALL
```

When a valid analysis request exists:

```text
VALID_ANALYSIS_REQUEST
-> DETERMINE_REQUIRED_DATA
-> SELECT_SUITABLE_FREE_PROVIDER_ROUTE(S)
-> FETCH_MINIMUM_REQUIRED_DATA
-> ANALYZE
-> RETURN_RESULT
```

`MINIMUM_REQUIRED_DATA` means the data needed to perform the requested analysis honestly and sufficiently. It does not require one provider call if multiple governed provider sources are needed for completeness or fallback, but provider usage must remain bounded by the request and provider quota/entitlement policy.

## 4. Provider Selection and Free-Tier Preservation

The existing Owner direction remains controlling:

```text
PROVIDER_DISCOVERY_POLICY = FREE_ONLY
PAID_PROVIDER_REQUIRED = REJECT
PAID_PLAN_REQUIRED = REJECT
FREE_TIER = CANDIDATE_IF_SUFFICIENT
FREE_API_REQUIRING_API_KEY = ALLOWED_CANDIDATE
```

On-demand consumption is intended to preserve free-tier quotas and avoid unnecessary provider traffic, but quota efficiency does not override truth, data sufficiency, provider terms, security, route authority, or required evidence.

## 5. Analysis Freshness Metadata

Each returned on-demand analysis shall preserve enough temporal truth for Web presentation and later audit, including conceptually:

```text
analysisGeneratedAt
marketDataAsOf
requestedHorizon = DAILY | WEEKLY | MONTHLY
providerDataDelayMinutes = KNOWN_VALUE | UNKNOWN
```

For Daily/Weekly/Monthly advisory analysis, delayed data up to 15 minutes remains eligible for suitability review under the existing Owner direction. Delay must not be represented as real-time.

This clarification does not define an autonomous advisory-opportunity lifecycle because FSATS is not maintaining a standing pushed opportunity feed in this mode.

## 6. Bar / Period Finality

Data delay and candle/period finality are separate facts.

```text
DATA_DELAY != PERIOD_FINALITY
```

If an analysis depends on a completed Daily/Weekly/Monthly bar, FSATS must know whether the relevant period is final or still in progress. An in-progress period shall not be silently represented as a completed period.

Conceptually:

```text
periodState = FINAL | IN_PROGRESS | UNKNOWN
```

If the requested analysis can validly use an in-progress period, that fact must remain explicit. If final-period semantics are required, the analysis shall fail closed, defer, or use the latest completed period according to the governed analysis rule rather than inventing finality.

## 7. Strategy and School Applicability

Trading remains authoritative for School/Strategy applicability. Market operating mode and supported horizons must constrain applicability at the Trading/business layer, not merely in Web presentation.

```text
MARKET_PROFILE -> CONSTRAINS_STRATEGY_APPLICABILITY
WEB_HIDE != BUSINESS_ENFORCEMENT
```

For the current Saudi advisory example:

```text
ADVISORY_ONLY
SUPPORTED_HORIZONS = DAILY / WEEKLY / MONTHLY
INTRADAY = DISABLED
EXECUTION = NONE
```

A Strategy or School path that requires intraday operation, real-time execution triggers, automated execution, or advisory-position management shall not be treated as applicable merely because it exists in the catalog.

## 8. Shared Web Separation

The on-demand FSAPMA rule applies to FSATS operational analysis data.

It does not prohibit Shared Web from using its own separately governed presentation-only chart source/route while a chart is displayed.

Mandatory separation remains:

```text
USER_ANALYSIS_REQUEST -> FSATS
FSATS_ANALYSIS_DATA_ACQUISITION -> FSAPMA
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
```

A Web chart refresh is therefore not, by itself, an FSAPMA analysis-data fetch trigger.

## 9. Current Personal Release Context

The current Owner direction is that this Falcon release is a personal application phase used to prove capability and prepare for possible future commercialization.

Current provider suitability therefore evaluates the required current personal/technical use case. Future commercialization shall require a separate governed revalidation of provider terms, licensing, redistribution/display rights, entitlements, and any commercial-use restrictions before commercial release.

```text
CURRENT_PERSONAL_USE_SUITABILITY != FUTURE_COMMERCIAL_USE_SUITABILITY
CURRENT_FREE_PROVIDER_ACCEPTANCE != FUTURE_COMMERCIAL_LICENSE_ACCEPTANCE
```

This future revalidation requirement does not authorize paid providers in the current `FREE_ONLY` policy.

## 10. Authority Boundary

```text
OWNER_CLARIFICATION = RECORDED
PART_8 = NOT_AUTHORIZED
RUNTIME = NOT_AUTHORIZED
PROVIDER_CONNECTIVITY = NOT_AUTHORIZED
BROKER_CONNECTIVITY = NOT_AUTHORIZED
DEPLOYMENT = NOT_AUTHORIZED
```

FCR-0013 remains the Foundation-owned future FSAPMA operational-provider egress dependency. FCR-0082 and FCR-0009 remain Application-held runtime/binding obligations whose execution requires separately authorized runtime-binding scope.

## 11. Final Invariants

```text
ANALYSIS_REQUEST != EXECUTION_REQUEST
USER_REQUEST != PROVIDER_ROUTE_AUTHORITY
PROVIDER_DISCOVERY != PROVIDER_CONNECTIVITY_AUTHORITY
NO_ANALYSIS_REQUEST -> NO_FSAPMA_ANALYSIS_DATA_FETCH
ON_DEMAND_FETCH != BACKGROUND_POLLING
WEB_CHART_REFRESH != FSAPMA_ANALYSIS_FETCH_TRIGGER
DATA_DELAY != PERIOD_FINALITY
CATALOG_PRESENT != APPLICABLE_TO_CURRENT_MARKET_MODE
CURRENT_PERSONAL_USE_SUITABILITY != FUTURE_COMMERCIAL_USE_SUITABILITY
```
