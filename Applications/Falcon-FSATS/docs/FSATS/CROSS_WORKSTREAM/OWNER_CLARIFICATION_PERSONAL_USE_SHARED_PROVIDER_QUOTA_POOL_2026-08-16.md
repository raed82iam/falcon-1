# FSATS Owner Clarification — Personal-Use Release and Shared Provider Quota-Pool Allocation

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Status:** `OWNER_DIRECTION_RECORDED / PLANNING_AND_CROSS_WORKSTREAM_SEMANTICS_ONLY`  
**Current Release Context:** `PERSONAL_USE / CAPABILITY_PROVING`  
**Part 8 Authority:** `NOT_AUTHORIZED`  
**Runtime Authority:** `NOT_GRANTED`  
**Provider Connectivity Authority:** `NOT_GRANTED`

## 1. Purpose

This record clarifies how FSATS and Shared Falcon Web shall reason about free-provider capacity when both consume services from the same external provider during the current personal-use Falcon release phase.

The rule is based on the provider's actual enforced quota pool, not merely on provider name, URL, Application identity, credential count, or network path.

This record is supplemented by the later controlling clarification:

`OWNER_CLARIFICATION_MULTI_CREDENTIAL_PROVIDER_CAPACITY_AND_WEB_FALLBACK_SHARING_2026-08-16.md`

The later clarification defines multi-provider/account/credential aggregation, per-pool atomic reservation, Web-independent-source-first behavior, and 50/50 as a fallback rule only for the exact real shared constrained pool.

## 2. Current Release Context

The current Falcon release is a personal/private application intended to prove capability and fitness before any future commercial-product decision.

```text
CURRENT_RELEASE_USE_CLASS = PERSONAL_USE
CURRENT_WEB_USE_CLASS = PERSONAL_PRIVATE_NOT_PUBLIC
CURRENT_RELEASE_COMMERCIAL_PRODUCT = NO
FUTURE_COMMERCIALIZATION = SEPARATELY_GOVERNED
```

Provider suitability for this release must therefore be evaluated for the actual current personal/private use case. Any future commercialization shall trigger fresh provider terms, licensing, redistribution, display-rights, account-entitlement, and cost revalidation before commercial use is claimed or activated.

```text
PERSONAL_USE_ELIGIBLE_NOW != COMMERCIAL_USE_ELIGIBLE_LATER
CURRENT_PROVIDER_ACCEPTANCE != FUTURE_COMMERCIAL_LICENSE
PERSONAL_USE != TERMS_BYPASS
```

## 3. Quota-Pool Identity Comes First

Before allocating capacity, Falcon must establish how the provider actually enforces each relevant limit from current authoritative provider evidence.

A provider may enforce one or more quota dimensions, including conceptually:

```text
API_KEY
ACCOUNT
ORGANIZATION
PUBLIC_IP
SERVICE_PLAN
ENDPOINT_CREDIT_POOL
BURST_WINDOW
DAILY_WINDOW
WEBSOCKET_CONNECTION_POOL
WEBSOCKET_SUBSCRIPTION_POOL
OTHER_PROVIDER_DEFINED_SCOPE
```

The exact provider-enforced identity is evidence-derived and must not be guessed.

Mandatory distinctions:

```text
SAME_PROVIDER != SAME_QUOTA_POOL
SAME_URL != SAME_QUOTA_POOL
DIFFERENT_CREDENTIAL != INDEPENDENT_QUOTA
DIFFERENT_APPLICATION != INDEPENDENT_QUOTA
SAME_PUBLIC_IP != SHARED_QUOTA_UNLESS_PROVIDER_ENFORCES_IT_THAT_WAY
```

## 4. When the 50/50 Split Applies

The Owner-directed 50/50 reservation applies only when Shared Web and FSATS/FSAPMA actually consume the same constrained upstream quota pool.

The later controlling clarification narrows this further: Shared Web first uses a suitable independent governed presentation source when one exists. Only when no such suitable independent source exists and Web must share the same real constrained quota pool does the 50/50 ceiling apply.

```text
IF WEB_HAS_SUITABLE_INDEPENDENT_DATA_SOURCE = FALSE
AND WEB_QUOTA_POOL_ID == FSATS_QUOTA_POOL_ID
AND QUOTA_POOL_IS_CONSTRAINED == TRUE
THEN
    WEB_MAX_SHARE = 50_PERCENT
    FSATS_MAX_SHARE = 50_PERCENT
```

The 50/50 values are maximum ceilings/reservations, not utilization targets.

If Web and FSATS have independently enforced provider quota pools, no artificial 50/50 split is created between those independent pools.

```text
INDEPENDENT_PROVIDER_QUOTA_POOLS -> NO_CROSS_POOL_50_50_SPLIT
50_50 = FALLBACK_SHARED_POOL_RULE
50_50 != DEFAULT_PROVIDER_RULE
```

## 5. Multiple Limit Dimensions

If the same provider enforces multiple independent constrained dimensions, allocation is evaluated separately for every shared dimension.

Example conceptual dimensions:

```text
PER_MINUTE_API_CREDITS
PER_DAY_API_CREDITS
BURST_REQUEST_LIMIT
WEBSOCKET_CONNECTION_LIMIT
WEBSOCKET_SUBSCRIPTION_LIMIT
```

If both Web and FSATS share all five, the 50/50 ceiling applies separately to all five. If only some are shared, only those shared dimensions are split.

```text
SHARED_DIMENSION -> APPLY_50_50_TO_THAT_DIMENSION
INDEPENDENT_DIMENSION -> DO_NOT_INVENT_SHARED_SPLIT
```

## 6. Allocation Calculation

For a discrete shared quota `Q` where units cannot be fractional:

```text
WEB_MAX_UNITS = floor(Q / 2)
FSATS_MAX_UNITS = floor(Q / 2)
UNALLOCATED_SAFETY_REMAINDER = Q - WEB_MAX_UNITS - FSATS_MAX_UNITS
```

For an even `Q`, the remainder is zero. For an odd `Q`, the single remaining unit stays unallocated rather than silently granting either workstream more than the 50% ceiling.

For continuous/rate limits, each side receives a 50% ceiling of the actual shared upstream limit.

Reset windows, endpoint weights, burst semantics, connection semantics, and provider-specific consumption weights remain part of the quota definition and must be modeled from current provider evidence.

## 7. Unknown or Changing Provider Limits

Provider limits are not static Falcon truth.

Before use and when provider evidence changes, Falcon must revalidate the current limit identity, value, reset window, weights, entitlement, and terms.

```text
UNKNOWN_QUOTA_SCOPE != UNLIMITED
UNKNOWN_QUOTA_VALUE != UNLIMITED
STALE_PROVIDER_LIMIT != CURRENT_LIMIT
```

If the shared quota scope or value cannot be established safely, affected consumption must fail/degrade closed rather than assume independent or unlimited capacity.

## 8. FSATS On-Demand Consumption Still Controls

This clarification does not change the previously recorded advisory-market provider model:

```text
SAUDI_ADVISORY_PROVIDER_MODE = ON_DEMAND
BACKGROUND_MARKET_POLLING = DISABLED
CONTINUOUS_FSAPMA_API_CONSUMPTION = DISABLED
AUTONOMOUS_OPPORTUNITY_SCANNING_OR_PUSH = DISABLED
NO_ANALYSIS_REQUEST -> NO_FSAPMA_ANALYSIS_DATA_FETCH
```

The FSATS reserved share therefore protects analysis capacity for valid user-requested analysis; it does not create a duty to consume that share continuously.

## 9. Web Presentation Separation

Shared Web presentation traffic remains separate from FSATS operational analysis traffic even where both consume the same external provider quota pool.

```text
SHARED_QUOTA_POOL != SHARED_AUTHORITY
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
```

A shared provider-enforced quota is a shared external resource constraint only. It does not merge routes, credentials, data ownership, or business authority.

## 10. Provider Selection Rule

Falcon shall not encode assumptions such as `Provider X is IP-based` or `Provider Y is key-based` as permanent product truth without current authoritative provider evidence.

At onboarding/revalidation time, provider management shall derive and record the actual quota scope for each relevant limit, then determine whether Web and FSATS share that quota pool.

The later controlling multi-credential clarification defines the complete provider-capacity model and shall be read together with this record.

## 11. Final Invariants

```text
PERSONAL_USE_NOW != COMMERCIAL_USE_LATER
PERSONAL_USE != TERMS_BYPASS
SAME_PROVIDER != SAME_QUOTA_POOL
SAME_API_PRODUCT != SAME_QUOTA_POOL_UNLESS_PROVIDER_ENFORCES_SHARED_SCOPE
DIFFERENT_API_KEY != GUARANTEED_INDEPENDENT_CAPACITY
SAME_IP != GUARANTEED_SHARED_CAPACITY
WEB_INDEPENDENT_SOURCE_FIRST = TRUE
SHARED_CONSTRAINED_POOL_AS_FALLBACK -> 50_50_MAX_CEILING
INDEPENDENT_POOLS -> NO_ARTIFICIAL_SPLIT
UNKNOWN_POOL_SCOPE -> FAIL_OR_DEGRADE_CLOSED
QUOTA_ALLOCATION != ROUTE_AUTHORITY
QUOTA_ALLOCATION != CREDENTIAL_AUTHORITY
QUOTA_ALLOCATION != PROVIDER_CONNECTIVITY_AUTHORITY
```

## 12. Authority Boundary

This planning clarification does not authorize Part 8, runtime, provider connectivity, credentials, Web provider connectivity, broker connectivity, Paper/Shadow/Tiny-Live/Live, deployment, or Application writes inside Shared Web-owned files.
