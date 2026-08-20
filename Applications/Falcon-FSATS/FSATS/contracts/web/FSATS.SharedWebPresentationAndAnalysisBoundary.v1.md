# FSATS Shared Web Presentation and Analysis Boundary v1

**Status:** OWNER-DIRECTED APPLICATION SEMANTIC BOUNDARY  
**Owning side for Trading semantics:** FSATS Application workstream  
**Shared Web implementation ownership:** Shared Falcon Web workstream  
**Runtime authority:** NOT GRANTED

## Purpose

This contract records the Project Owner-directed separation between Shared Web presentation market data and FSATS operational Trading data.

Shared Falcon Web is a reusable Falcon Shared Application serving multiple Falcon Applications. It is not part of FSATS Trading and does not need to route ordinary continuous presentation-only market data through FSATS or FSAPMA.

## Controlling separation

```text
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_DIRECT_PROVIDER_DISPLAY != FSAPMA_OPERATIONAL_PROVIDER_ROUTE
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
CUSTOMER_ANALYSIS_REQUEST -> FSATS
FSATS_ANALYSIS_DATA_ACQUISITION -> FSAPMA
FSATS_ANALYSIS_RESULT -> WEB
```

## Web presentation-only data

Shared Web may obtain ordinary presentation/display market information from its own separately governed external provider/URL routes, including as applicable:

- live/raw price display;
- chart stream/display series;
- volume display;
- other provider information used only for Web presentation.

The exact Web provider selection, URL registry, credentials, quotas, entitlements, egress authority, security controls, runtime activation and provider integration are Web/Foundation-owned concerns and are not implemented or prescribed by FSATS.

FSATS does not treat Web-fetched presentation data as operational Trading truth.

## Customer analysis requests

When a customer asks Shared Web for Trading-domain intelligence, Web sends a governed request to FSATS rather than supplying its own presentation feed as Trading input.

Trading-domain intelligence includes at least:

- asset analysis;
- detailed asset study;
- School applicability;
- Strategy applicability and ranking;
- best Strategy selection/explanation;
- Trading Risk;
- account-aware risk where the governed broker-account scope is available;
- Trading recommendation;
- other FSATS-owned business/domain analysis outputs.

For such a request:

```text
CUSTOMER
-> SHARED WEB
-> FSATS ANALYSIS REQUEST
-> FSATS / TRADING
-> FSAPMA OPERATIONAL DATA ACQUISITION AS NEEDED
-> TRADING ANALYSIS / SCHOOL / STRATEGY / RISK SEMANTICS
-> FSATS RESULT PROJECTION
-> SHARED WEB
-> CUSTOMER
```

## No backflow rule

Web-fetched presentation data SHALL NOT be reintroduced into FSATS as a shortcut around FSAPMA.

It SHALL NOT become:

- FSATS operational market truth;
- analysis input;
- School or Strategy evidence;
- Risk input;
- portfolio truth;
- broker truth;
- execution truth;
- order/execution authority;
- Trading-universe admission evidence by implication.

## Existing Web analysis semantics

The following semantic identities remain Application-owned analysis/request-result surfaces where applicable:

```text
FSATS.WebOnDemandAnalysisRequest.v1
FSATS.WebOnDemandAnalysisResult.v1
FSATS.WebDetailedAssetAnalysisProjection.v1
```

The previous chart semantic identities may still be used when FSATS explicitly provides an Application-owned chart projection:

```text
FSATS.WebChartDataRequest.v1
FSATS.WebChartHistoricalProjection.v1
FSATS.WebChartUpdateProjection.v1
```

They are not the mandatory default path for ordinary raw Web presentation market data.

## Authority boundary

```text
WEB_DISPLAY != FSATS_MARKET_DATA_TRUTH_OWNER
WEB_PRESENTATION_PROVIDER_ACCESS != TRADING_AUTHORITY
CUSTOMER_ANALYSIS_REQUEST != STRATEGY_ACTIVATION
CUSTOMER_ANALYSIS_REQUEST != CAPITAL_RESERVATION
CUSTOMER_ANALYSIS_REQUEST != EXECUTION_AUTHORITY
FSATS_ANALYSIS_RESULT != TRADE_AUTHORIZATION
```

This document creates no provider connectivity, broker connectivity, Paper, Shadow, Tiny-Live, Live, deployment or runtime-route authority.

## FCR synchronization

FCR-0125 is the current Shared-Web chart/display coordination record and has received an Application semantic update reflecting this boundary.

FCR-0127 and FCR-0130 remain the principal current cross-workstream records for on-demand and detailed Trading analysis semantics.

Foundation/Web external provider connectivity remains outside FSATS ownership. The FSATS Application workstream does not originate or implement Web-owned provider-egress work.
