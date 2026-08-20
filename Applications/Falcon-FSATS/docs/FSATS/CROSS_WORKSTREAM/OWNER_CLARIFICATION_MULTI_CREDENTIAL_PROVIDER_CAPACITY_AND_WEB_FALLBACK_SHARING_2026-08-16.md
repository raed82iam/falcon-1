# FSATS Owner Clarification — Multi-Credential Provider Capacity and Web Fallback Sharing

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Status:** `OWNER_DIRECTION_RECORDED / PLANNING_AND_CROSS_WORKSTREAM_SEMANTICS_ONLY`  
**Current Release Context:** `PERSONAL_USE / PRIVATE / NOT_PUBLIC`  
**Part 8 Authority:** `NOT_AUTHORIZED`  
**Runtime Authority:** `NOT_GRANTED`  
**Provider Connectivity Authority:** `NOT_GRANTED`

## 1. Purpose

This record clarifies how FSAPMA shall reason about multiple provider accounts/credentials and how Shared Falcon Web may share provider capacity only as a fallback when no suitable independent presentation-data source is available.

The model is capacity-pool based. Provider name alone does not define capacity. Each real provider-enforced quota pool is identified from current authoritative provider evidence and governed independently.

## 2. Current Release Context

The current Falcon release is personal/private and not a public or commercial service.

```text
CURRENT_RELEASE_USE_CLASS = PERSONAL_USE
CURRENT_WEB_USE_CLASS = PERSONAL_PRIVATE_NOT_PUBLIC
CURRENT_RELEASE_COMMERCIAL_PRODUCT = NO
FUTURE_COMMERCIALIZATION = SEPARATELY_GOVERNED
```

Current provider suitability must be evaluated against the provider's current terms for the actual personal/private use being performed. Future commercialization remains a separate revalidation gate.

## 3. FSAPMA Supports Multiple Providers, Accounts, and Credentials

FSAPMA shall not assume one provider equals one credential or one quota.

Conceptually:

```text
PROVIDER
  -> PROVIDER_ACCOUNT_1
       -> CREDENTIAL_1
       -> QUOTA_POOL_A
  -> PROVIDER_ACCOUNT_2
       -> CREDENTIAL_2
       -> QUOTA_POOL_B
  -> PROVIDER_ACCOUNT_3
       -> CREDENTIAL_3
       -> QUOTA_POOL_C
```

FSAPMA may use multiple governed providers, provider accounts, and credentials where the provider terms and Falcon authority permit them.

Secret bytes remain outside ordinary Application state. Shared Web may provide the future secure Owner-facing credential-entry surface, but ordinary Web payloads and chat do not carry plaintext credentials.

## 4. Effective FSAPMA Capacity

When provider-enforced quota pools are legitimately independent, FSAPMA effective capacity is the sum of the available capacity across those independent pools.

```text
FSAPMA_EFFECTIVE_CAPACITY
=
SUM(LEGITIMATELY_INDEPENDENT_AVAILABLE_QUOTA_POOLS)
```

This is not permission to manufacture capacity by creating additional credentials when the provider applies one shared/global limit or when provider terms prohibit such use.

Mandatory distinctions:

```text
MULTIPLE_CREDENTIALS != AUTOMATIC_MULTIPLIED_CAPACITY
MULTIPLE_ACCOUNTS != AUTOMATIC_MULTIPLIED_CAPACITY
DIFFERENT_API_KEY != GUARANTEED_INDEPENDENT_QUOTA
INDEPENDENT_PROVIDER_ENFORCEMENT + TERMS_ALLOWED -> INDEPENDENT_CAPACITY_ELIGIBLE
```

If multiple credentials/accounts share one upstream quota, entitlement, session, organization, IP, or other provider-defined pool, that pool is counted once.

## 5. Per-Pool Runtime Accounting Semantics

Each real quota pool shall be modeled independently with at least equivalent semantics to:

```text
quotaPoolId
providerId
providerAccountId
credentialReference
quotaDimensionId
quotaScopeType
limitValue
limitUnit
resetWindow
consumptionWeightModel
available
reserved
consumed
evidenceAsOf
termsAsOf
status
```

For concurrent FSATS requests, reservation is per actual quota pool:

```text
CHECK AVAILABLE
-> RESERVE REQUIRED CAPACITY ATOMICALLY
-> FETCH
-> COMMIT ACTUAL USAGE
-> RELEASE UNUSED RESERVATION
```

Mandatory invariant:

```text
AVAILABLE_QUOTA != RESERVED_QUOTA
CONCURRENT_REQUESTS_MUST_NOT_OVERCOMMIT_ANY_QUOTA_POOL
```

## 6. FSAPMA Distribution Behavior

FSAPMA may distribute required data-acquisition work across multiple eligible provider pools based on capability, data coverage, health, freshness, remaining quota, rate windows, provider-specific cost/credit weights, and fallback readiness.

Example conceptual behavior:

```text
REQUESTED_ANALYSIS
-> DETERMINE_REQUIRED_DATA
-> DISCOVER_ELIGIBLE_PROVIDER_POOLS
-> RANK BY FITNESS + CAPACITY + HEALTH
-> RESERVE CAPACITY
-> DISTRIBUTE FETCH WORK
-> VALIDATE / RECONCILE DATA
-> RETURN OPERATIONAL DATA TO FSATS
```

No rule requires equal load distribution.

## 7. Web Independent Source First

Shared Web presentation traffic shall use a suitable independent governed presentation-data source when one is available.

```text
WEB_HAS_SUITABLE_INDEPENDENT_DATA_SOURCE = TRUE
-> WEB_USES_ITS_OWN_GOVERNED_SOURCE
-> NO_FSAPMA_QUOTA_SHARING_REQUIRED
```

This preserves FSAPMA operational capacity for FSATS analysis and maintains the existing separation between Web presentation data and FSATS operational data.

## 8. 50/50 Is a Fallback Sharing Rule

The Owner-directed 50/50 split applies only when all of the following are true:

1. Web has no suitable independent presentation-data source for the required data;
2. Web must consume a real constrained quota pool also used by FSAPMA;
3. the provider actually enforces that pool as shared upstream capacity.

```text
IF WEB_HAS_SUITABLE_INDEPENDENT_DATA_SOURCE = FALSE
AND WEB_QUOTA_POOL_ID == FSAPMA_QUOTA_POOL_ID
AND QUOTA_POOL_IS_CONSTRAINED = TRUE
THEN
    WEB_MAX_SHARE = 50_PERCENT
    FSAPMA_MAX_SHARE = 50_PERCENT
```

The split applies only to the shared pool. Other independent FSAPMA pools remain available to FSAPMA according to their own limits.

Mandatory invariant:

```text
50_50 = FALLBACK_SHARED_POOL_RULE
50_50 != DEFAULT_PROVIDER_RULE
```

## 9. Example

If FSAPMA has three legitimately independent provider quota pools:

```text
POOL_A = 800
POOL_B = 800
POOL_C = 800
```

and Web has no suitable independent source and must share only `POOL_A`:

```text
POOL_A:
  WEB_MAX = 400
  FSAPMA_MAX = 400

POOL_B:
  FSAPMA = 800

POOL_C:
  FSAPMA = 800
```

Then FSAPMA effective capacity under this condition is:

```text
400 + 800 + 800 = 2000
```

The Web share does not automatically halve unrelated independent FSAPMA pools.

## 10. On-Demand Advisory Behavior Remains Unchanged

For the current Saudi advisory mode:

```text
TRIGGER = USER_REQUEST_ONLY
BACKGROUND_MARKET_POLLING = DISABLED
CONTINUOUS_FSAPMA_ANALYSIS_API_CONSUMPTION = DISABLED
AUTONOMOUS_OPPORTUNITY_SCANNING_OR_PUSH = DISABLED
```

FSAPMA fetches only data required for a valid on-demand analysis request. Multi-provider and multi-credential capacity exists to improve availability and distribute work, not to create continuous background consumption.

## 11. Web / FSATS Separation Remains Intact

```text
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
SHARED_QUOTA_POOL != SHARED_AUTHORITY
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
USER_ANALYSIS_REQUEST -> FSATS
FSATS_ANALYSIS_DATA_ACQUISITION -> FSAPMA
```

Shared quota coordination is resource coordination only. It does not merge routes, credentials, data ownership, or business authority.

## 12. Provider Terms and Evidence

Provider terms and quota enforcement are not static Falcon truth.

At onboarding and revalidation, Falcon must establish from current authoritative provider evidence:

- whether multiple accounts/credentials are allowed for the intended use;
- whether quota is independent or shared;
- quota dimensions and reset windows;
- endpoint/request weights;
- session/connection limits;
- any IP/account/organization/global constraints;
- current personal/private use eligibility.

Unknown or stale quota identity must not be treated as extra capacity.

```text
UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY
UNKNOWN_PROVIDER_TERMS != PERMISSION
STALE_LIMIT != CURRENT_LIMIT
```

## 13. Final Invariants

```text
PERSONAL_PRIVATE_USE_NOW != PUBLIC_COMMERCIAL_USE
MULTI_PROVIDER = SUPPORTED
MULTI_ACCOUNT = SUPPORTED_WHEN_GOVERNED_AND_TERMS_ALLOWED
MULTI_CREDENTIAL = SUPPORTED_WHEN_GOVERNED_AND_TERMS_ALLOWED
INDEPENDENT_QUOTA_POOLS -> CAPACITY_MAY_SUM
SHARED_UPSTREAM_POOL -> COUNT_ONCE
ATOMIC_RESERVATION_PER_POOL = REQUIRED
WEB_INDEPENDENT_SOURCE_FIRST = TRUE
WEB_NO_SUITABLE_SOURCE + REAL_SHARED_POOL -> 50_50_FALLBACK
50_50_APPLIES_ONLY_TO_SHARED_POOL
SHARED_QUOTA_POOL != SHARED_AUTHORITY
QUOTA_CAPACITY != PROVIDER_CONNECTIVITY_AUTHORITY
```

## 14. Authority Boundary

This clarification is planning/cross-workstream semantics only. It does not authorize Part 8, provider connectivity, credential storage/runtime retrieval, Web provider connectivity, broker connectivity, Paper/Shadow/Tiny-Live/Live, deployment, or Application writes inside `applications/shared/web/**`.
