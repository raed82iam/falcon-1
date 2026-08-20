# FSATS Broad Red Team Review — Advisory On-Demand Provider Fetch

**Date:** `2026-08-16`  
**Reviewed Semantic Set Through Commit:** `7388347836b6e494c0d6d4d6cff8ecd4e7f0448b`  
**Result:** `PASS`  
**Open Critical / High / Medium / Low:** `0 / 0 / 0 / 0`

## 1. Scope

Fresh adversarial review challenged the newly clarified semantics for:

- user-triggered advisory analysis;
- no autonomous opportunity scanning/push;
- FSAPMA on-demand provider API use;
- minimum-required-data fetching;
- free-only provider policy;
- delayed data and period finality;
- Trading-level Strategy/School applicability;
- Web chart traffic separation;
- current personal release versus future commercialization;
- runtime/provider authority boundaries.

## 2. Adversarial Cases

### RT-OD-01 — Poll provider continuously despite no user request

**Attack:** Keep a background job pulling Saudi market data because the provider is free and may improve responsiveness.

**Result:** REJECTED.

```text
NO_ANALYSIS_REQUEST -> NO_FSAPMA_ANALYSIS_DATA_FETCH
BACKGROUND_MARKET_POLLING = DISABLED
CONTINUOUS_FSAPMA_API_CONSUMPTION = DISABLED
```

### RT-OD-02 — Treat chart refresh as analysis trigger

**Attack:** Use a Web chart refresh as a reason for FSAPMA to fetch operational analysis data.

**Result:** REJECTED.

```text
WEB_CHART_REFRESH != FSAPMA_ANALYSIS_FETCH_TRIGGER
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
```

### RT-OD-03 — Autonomous opportunity push disguised as cache warming

**Attack:** Scan Saudi instruments in the background and keep ranked opportunities ready so Web can display them instantly when asked.

**Result:** REJECTED for the current advisory mode.

Preparing a standing ranked opportunity feed is substantively autonomous opportunity scanning even if labeled cache warming.

### RT-OD-04 — Over-fetch the entire market for a single-stock request

**Attack:** A user requests one stock analysis, but FSAPMA downloads the entire market by default.

**Result:** REJECTED unless the analysis method can justify that broader market context as required input.

`MINIMUM_REQUIRED_DATA` is semantic, not merely the smallest byte count. Required benchmark/sector/market context may be fetched when genuinely necessary, but unrelated bulk collection is not justified by the request.

### RT-OD-05 — One provider must satisfy every field

**Attack:** Reject a valid request because no single free provider supplies all required fields.

**Result:** REJECTED.

Multiple separately governed free provider sources may be used where needed for completeness or fallback, subject to quota, entitlement, route and provenance rules.

### RT-OD-06 — API key exists, therefore provider may be called

**Attack:** Treat credential availability as authority to make the on-demand request.

**Result:** REJECTED.

```text
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
OWNER_PROVIDED_CREDENTIAL != PROVIDER_CONNECTIVITY_AUTHORIZED
```

FCR-0013 remains controlling for future FSAPMA provider egress.

### RT-OD-07 — 15-minute delay means the current Daily candle is final

**Attack:** Use a delayed quote to classify an in-progress Daily/Weekly/Monthly bar as completed.

**Result:** REJECTED.

```text
DATA_DELAY != PERIOD_FINALITY
```

Period state must remain explicit or fail closed according to the governed analysis rule.

### RT-OD-08 — Web hides intraday Strategy, but Trading still uses it

**Attack:** Keep an intraday or real-time execution-dependent Strategy in the Trading synthesis while only hiding it in Web.

**Result:** REJECTED.

Market mode and supported horizons constrain Strategy/School applicability in Trading business semantics. UI filtering alone is not enforcement.

### RT-OD-09 — Reuse Web-fetched chart data in FSATS analysis

**Attack:** Because the chart already has recent Saudi data, pass it back into FSATS to avoid another API call.

**Result:** REJECTED.

```text
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
FSATS_ANALYSIS_DATA_ACQUISITION -> FSAPMA
```

### RT-OD-10 — Personal release makes provider terms irrelevant

**Attack:** Ignore provider use restrictions because Falcon is currently personal.

**Result:** REJECTED.

Current provider suitability still requires terms compatible with the current personal/technical use case. Future commercial licensing/redistribution is a separate revalidation gate.

### RT-OD-11 — Commercial future means reject every provider without commercial rights today

**Attack:** Block current personal use unless the provider already supports future commercial redistribution.

**Result:** REJECTED.

```text
CURRENT_PERSONAL_USE_SUITABILITY != FUTURE_COMMERCIAL_USE_SUITABILITY
```

Future commercialization must revalidate provider rights; it is not a current paid-provider or commercial-license requirement.

### RT-OD-12 — User request becomes execution or standing recommendation authority

**Attack:** Interpret an analysis request as permission to execute, track a position, or maintain a standing opportunity lifecycle.

**Result:** REJECTED.

```text
ANALYSIS_REQUEST != EXECUTION_REQUEST
USER_ACTION_AFTER_RESULT = OUTSIDE_FSATS_ADVISORY_RESPONSIBILITY
```

## 3. Residual Future Implementation Requirements

No unresolved planning-semantic blocker remains. Future authorized implementation must executably verify at least:

- no background FSAPMA analysis polling in the advisory-only mode;
- exact valid-analysis-request trigger binding;
- bounded/minimum-required-data acquisition behavior;
- provider quota/entitlement handling;
- no Web presentation-data backflow;
- delayed-data versus period-finality truth;
- Trading-level Strategy/School applicability enforcement;
- no secret-byte exposure;
- provider/runtime authority gating.

These are future implementation requirements and do not grant current implementation authority.

## 4. Final Result

```text
BROAD RED TEAM = PASS
OPEN CRITICAL = 0
OPEN HIGH = 0
OPEN MEDIUM = 0
OPEN LOW = 0
AUTONOMOUS SCANNING LEAKAGE = NONE
RUNTIME AUTHORITY LEAKAGE = NONE
PROVIDER AUTHORITY LEAKAGE = NONE
WEB/FSAPMA DATA-BACKFLOW PATH = NONE IN DEFINED SEMANTICS
EXECUTION AUTHORITY LEAKAGE = NONE
```

This PASS applies to the planning semantic set only. It is not Part 8, runtime, provider-connectivity, Web implementation, deployment, or Owner final-acceptance authority.
