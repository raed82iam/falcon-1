# Owner Decision — Shared Free-Provider Quota Coordination for Web Presentation and FSATS

**Date:** 2026-08-16
**Scope:** Shared Falcon Web presentation-provider usage coordination with FSATS/FSAPMA
**Status:** OWNER_DECISION_RECORDED_AND_RECONCILED_WITH_FCR_0220

## Primary rule — independent Web source first

Shared Web should use a suitable independently governed presentation source whenever one is available.

```text
WEB_HAS_SUITABLE_INDEPENDENT_DATA_SOURCE = TRUE
-> WEB_USES_ITS_OWN_GOVERNED_SOURCE
-> NO_FSAPMA_QUOTA_SHARING_REQUIRED
-> 50_50_SPLIT = NOT_APPLICABLE
```

The 50/50 rule is therefore a fallback shared-pool rule, not a default provider rule.

## Shared constrained pool fallback

The 50/50 allocation applies only when all of the following are true:

1. Shared Web has no suitable independent presentation source;
2. Shared Web and FSATS/FSAPMA draw from the same actual provider-enforced quota pool;
3. that pool is constrained by a documented quota/rate/session/connection/request/burst or equivalent capacity limit.

```text
IF WEB_HAS_SUITABLE_INDEPENDENT_DATA_SOURCE = FALSE
AND WEB_QUOTA_POOL_ID == FSAPMA_QUOTA_POOL_ID
AND QUOTA_POOL_IS_CONSTRAINED = TRUE:
  WEB_MAX_SHARE_OF_CURRENT_LIMIT = 50%
  FSATS_MAX_SHARE_OF_CURRENT_LIMIT = 50%

OTHERWISE:
  50_50_QUOTA_SPLIT = NOT_APPLICABLE
```

Typical shared-pool identity keys may include provider account, API/application credential, source IP, session/connection pool, burst budget, request budget, or another provider-defined shared capacity key.

```text
SAME_PROVIDER_NAME != SAME_QUOTA_POOL
SAME_API_VENDOR != SAME_QUOTA_POOL
DIFFERENT_API_KEY != GUARANTEED_INDEPENDENT_QUOTA
MULTIPLE_ACCOUNTS != AUTOMATIC_MULTIPLIED_CAPACITY
UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY
```

A plain `WEB_URL` used only as a presentation/source link is not assigned an artificial 50/50 quota merely because it is a URL. If the provider documents a rate, traffic, session, access, contractual, or other constraint for that URL-backed service, then that real constraint must be governed and respected.

The 50% allocation is a maximum ceiling, not a target consumption level.

Shared Web must throttle before exhausting its allocation and must never intentionally consume beyond 50% of the provider's currently effective constrained free capacity when the fallback shared-pool rule applies. The remaining 50% is reserved for FSATS/FSAPMA. Web does not enforce FSATS internals, so the corresponding FSATS/FSAPMA quota discipline remains an Application-owned obligation.

## Dynamic provider-limit rule

Provider limits, quotas, entitlement rules, reset windows, burst rules, connection/session limits, and free-tier terms may change. Falcon must not hard-code a historical provider limit as permanent truth.

For each governed constrained provider capability, current quota identity and current quota/terms metadata should be revalidated from the provider's authoritative terms/documentation or governed provider metadata before activation and periodically thereafter.

When the provider changes a relevant limit or term, budgets must be recalculated from the new effective constrained capacity.

```text
OLD_PROVIDER_LIMIT != CURRENT_PROVIDER_LIMIT
UNKNOWN_QUOTA_IDENTITY != SAFE_TO_ASSUME_INDEPENDENT_CAPACITY
UNKNOWN_CONSTRAINED_LIMIT != SAFE_TO_CONSUME_FULL_CAPACITY
PLAIN_WEB_URL_WITHOUT_DOCUMENTED_SHARED_LIMIT != API_QUOTA
```

If a shared API/WebSocket/other constrained route clearly has a limit but the effective limit cannot be determined reliably, Web must fail/degrade closed rather than assume capacity. Current Web planning code represents this as zero consumable Web share until the limit becomes known.

## Safety margin

To avoid accidental provider blocking from races, retries, hidden provider overhead, burst accounting, delayed counters, or shared upstream accounting, Web should use an operational warning/throttle margin below its 50% hard ceiling. The exact margin is implementation/provider-specific and must never increase Web's hard ceiling above 50%.

```text
WEB_SOFT_THROTTLE < 50%
WEB_HARD_CEILING = 50%
```

The same principle should be respected by FSATS/FSAPMA within its reserved half where the shared constrained-capacity fallback rule applies.

## URL, API and credential boundary

Shared Web may consume provider source metadata for chart presentation only when separately governed for Web use. Source metadata can describe `WEB_URL`, `API`, `FILE`, or other governed access types.

A presentation URL is not automatically an API route and is not automatically quota-limited. Conversely, a URL does not imply unlimited or unrestricted access: provider terms, access controls, traffic rules, session constraints, attribution requirements and destination authority still apply where documented.

```text
WEB_URL != API_QUOTA_BY_DEFAULT
WEB_URL != UNRESTRICTED_ACCESS_AUTHORITY
SAME_PROVIDER != SAME_AUTHORITY
SAME_URL != SAME_AUTHORITY
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
SHARED_QUOTA_POOL != SHARED_AUTHORITY
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
```

## Advisory analysis consumption

For the current advisory-market model, FSATS analysis-provider consumption is request-driven only:

```text
USER REQUEST
-> SHARED WEB
-> FSATS ON-DEMAND ANALYSIS REQUEST
-> FSATS DETERMINES REQUIRED DATA
-> FSAPMA FETCHES ONLY DATA REQUIRED FOR THAT REQUEST
-> FSATS ANALYZES
-> WEB PRESENTS RESULT
-> END
```

```text
BACKGROUND_MARKET_POLLING = DISABLED
CONTINUOUS_FSAPMA_ANALYSIS_API_CONSUMPTION = DISABLED
AUTONOMOUS_OPPORTUNITY_SCANNING_OR_PUSH = DISABLED
NO_ANALYSIS_REQUEST -> NO_FSAPMA_ANALYSIS_DATA_FETCH
WEB_CHART_REFRESH != FSAPMA_ANALYSIS_FETCH_TRIGGER
```

## Credential handling

A free API key may be requested from the Owner only through a governed secure credential mechanism when one exists. Plaintext API key values remain prohibited in ordinary chat, incident conversation, logs, or ordinary Web/Application payloads.

The Owner-facing Web surface may display `ACTION_REQUIRED` and provider metadata, but the secure-entry control remains disabled until the governed credential runtime/storage mechanism and destination validation exist.

```text
OWNER_ACTION_MESSAGE != CREDENTIAL
API_KEY_VALUE != CHAT_PAYLOAD
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
OWNER_PROVIDED_CREDENTIAL != PROVIDER_CONNECTIVITY_AUTHORIZED
```

## Current Web implementation evidence

The Web-owned planning implementation now contains:

- `src/core/market-data-plan.js` — independent-source-first + shared constrained-pool fallback decision logic;
- `src/features/advisory-markets/advisory-markets.js` — user-facing advisory-only presentation;
- `src/features/owner-provider-actions/owner-provider-actions.js` — separate Owner-only provider action presentation with no plaintext secret input;
- tests covering independent source, independent pools, 50/50 fallback, and fail-closed unknown shared limit.

This implementation is source-level and does not itself authorize provider connectivity, Foundation egress, runtime routes, secret storage, or deployment.

## FCR coordination

This Owner decision refines FCR-0125 and FCR-0220 provider quota semantics and records the current Web-side reconciliation. Governed executable verification remains separately required before FCR closure eligibility.
