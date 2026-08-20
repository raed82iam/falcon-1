# P0-G - FSAPMA Operational Data Fabric

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-G defines FSAPMA as the sole FSATS operational external-data gateway and the provider-independent operational data fabric. It integrates provider/service-role/API-instance identity, provider-account separation, entitlements/use rights, Data Products, capability modeling, quota/capacity, Route Leases, continuity, correction, degraded modes and current Foundation egress boundaries.

## 2. Responsibility

FSAPMA owns operational external-data acquisition, normalization, provider/service-role/account/API-instance capability modeling, Data Product formation, acquisition and use-right enforcement, provider selection, data quality/provenance, continuity, correction/supersession, quota/capacity accounting and provider-side resilience policy.

FSAPMA does not own Trading Risk, strategy selection, portfolio/capital decisions, broker execution, Guardian crisis authority, APP-RSC resource coordination, Foundation egress/security authority, research Internet or customer identity.

## 3. Sole operational data gateway

```text
OPERATIONAL_EXTERNAL_MARKET_DATA
-> FSAPMA
-> NORMALIZED / GOVERNED DATA PRODUCT
-> DECLARED AUTHORIZED CONSUMER
```

Trading, Guardian, FSTSimA, Shared Web, Shared Communication, MSA/LSA/CSA or another FSATS Application cannot bypass FSAPMA for operational market/reference data unless a later explicit accepted architecture creates a different owner.

Research-only Internet is not an operational-data fallback.

## 4. Provider, account, service-role and API-instance separation

```text
PROVIDER != PROVIDER_ACCOUNT != SERVICE_ROLE != API_INSTANCE
```

A Provider is the external vendor/entity. A Provider Account or API Account identifies an attributable subscription/account/entitlement context. A Service Role is the governed capability/purpose consumed, such as market data or reference data. An API Instance is the exact credential/configuration/endpoint instance used for an authorized role.

```text
SAME_VENDOR != SAME_AUTHORITY
SHARED_CREDENTIAL_SOURCE != SHARED_PURPOSE_PERMISSION
PROVIDER_ACCOUNT != CUSTOMER
PROVIDER_ACCOUNT != BROKER_ACCOUNT
CREDENTIAL_REFERENCE != SECRET_BYTES
```

## 5. Provider Capability Graph

FSAPMA maintains an evidence-backed, versioned provider/account/service-role/API-instance capability model containing as applicable:

- supported markets/asset classes;
- instrument identifiers;
- real-time/delayed/reference/history support;
- streaming/snapshot support;
- entitlement constraints;
- redistribution/use-right constraints;
- provider-account/API-plan boundaries;
- rate/quota/cost constraints;
- connection/session limits;
- quality/precision/time semantics;
- corrections/adjustments;
- availability/health;
- role/environment restrictions;
- shared provider-side capacity dependencies.

Unknown capability is not supported.

## 6. Data Products

Trading consumers depend on provider-independent governed Data Products, not raw vendor semantics.

Each Data Product defines as applicable:

- stable Data Product identity/version;
- market/asset/instrument scope;
- fields/units/currency/precision;
- event/source time semantics;
- snapshot/delta/stream/reference/history type;
- freshness/validity expectations;
- continuity/completeness semantics;
- adjustment/corporate-action semantics;
- provenance/lineage;
- quality/confidence/degradation state;
- correction/supersession behavior;
- entitlement/use-right scope;
- authorized consumer class;
- replay/test/operational classification.

```text
DATA_PRODUCT = OPERATIONAL_ELIGIBLE_INPUT_WITH_DECLARED_SEMANTICS
DATA_PRODUCT != ABSOLUTE_MARKET_TRUTH
NO_SOURCE_VALUE != ZERO
STALE != CURRENT
PARTIAL != COMPLETE
```

## 7. Entitlement and use-right separation

```text
ACQUISITION_ENTITLEMENT != REDISTRIBUTION_ENTITLEMENT
PROVIDER_ACCESS != CONSUMER_USE_RIGHT
```

For every material delivery FSAPMA must establish that source, provider account, service role, API instance and intended consumer/use remain permitted. Customer-owned or broker-linked data entitlement does not automatically become Falcon-wide entitlement.

## 8. Provider/API-instance pools

FSAPMA may use multiple provider accounts/API instances for resilience/capacity only where contracts and entitlements permit.

Pools preserve exact provider-account/instance identity, owner/service role where relevant, credential isolation, quota/capacity accounting, entitlement, rate-limit state, health, shared provider limits and failover eligibility.

```text
MULTIPLE_API_INSTANCES != UNLIMITED_CAPACITY
POOLING != ENTITLEMENT_EXPANSION
POOLING != QUOTA_LAUNDERING
```

## 9. Shared capacity modeling

When multiple roles/accounts/instances share a vendor-side rate, cost, session or credential constraint, that shared constraint is modeled once and consumed by all affected routes. One role may not consume protected capacity of another merely because requests are technically possible.

Business priority inside FSAPMA does not create Foundation technical criticality.

## 10. Free-first capability-aware routing

FSAPMA may prefer free or lower-cost provider capacity when it satisfies current capability, quality, entitlement and continuity needs. Cost preference cannot override legal/contractual use rights, required freshness, correctness or operational safety.

Provider selection considers as applicable required Data Product, entitlement, exact provider account/API instance, capability, quality, freshness, latency, continuity, health, quota/cost/capacity, environment, market/session and current degradation/circuit state.

Recent success does not create permanent authority.

## 11. Route Lease

A Route Lease is a bounded, versioned, invalidatable selection cache stating that one provider/account/service-role/API-instance route remains eligible for a Data Product under exact conditions.

A Route Lease binds at least:

- Data Product requirement;
- provider/account/service-role/API-instance identity;
- entitlement context;
- capability/policy version;
- validity/expiry;
- health/circuit epoch;
- reason/evidence.

Invalidate on revocation, capability change, entitlement change, circuit change, incompatible policy update or other material route change.

```text
ROUTE_LEASE != EXTERNAL_EGRESS_AUTHORITY
ROUTE_LEASE != DATA_QUALITY_GUARANTEE
```

## 12. Time, continuity and completeness

FSAPMA distinguishes event/source time from receive/cache/read time.

```text
CACHE_READ_TIME != SOURCE_FRESHNESS
RECENT_RECEIPT != COMPLETE_STREAM
RECONNECT != PROOF_OF_NO_GAP
```

Stream continuity covers sequence/gap detection where supported, snapshot/delta compatibility, reconnect/recovery, duplicates, correction/supersession, unknown/missing intervals and bounded stale-cache use only where accepted for exact purpose.

Consumers must be able to distinguish contiguous, gapped, replayed, corrected, stale, partial, unknown and unavailable states.

## 13. Precision, units, currency and adjustments

Normalization preserves or explicitly transforms numeric precision, unit basis, currency, price/quantity scale, corporate-action adjustment status, instrument mapping and timezone/session/calendar semantics.

Lossy business-significant transformation must be explicit and evidence-backed. Adjusted/unadjusted data is never silently mixed.

## 14. Conflicting providers

Conflicts are not resolved by majority vote or newest receive time alone. Policy may consider source role/quality evidence, source timestamp, entitlement/classification, expected latency, continuity, correction behavior, instrument identity consistency and cross-source uncertainty.

Material unresolved conflict produces degraded/uncertain Data Product state. Downstream exposure authority is then restricted by its owning consumer/Risk/Guardian policy. FSAPMA owns data degradation/truth classification, not Trading Risk action.

## 15. Data-quality dimensions

Where applicable quality evaluation preserves completeness, validity, timeliness/freshness, continuity, consistency, precision, provenance, adjustment status, entitlement validity and cross-source agreement/uncertainty.

A quality dimension that is not applicable must be explicitly N/A rather than silently passed.

## 16. Circuit breakers and failure domains

Circuit state is scoped to the smallest correct provider/account/service-role/API-instance/endpoint failure domain while allowing evidence to broaden to provider-wide/common failure.

States may include healthy/closed, open/unavailable, half-open/probe and degraded/limited.

A single successful probe is not automatic full recovery. Provider-wide outage cannot be misrepresented as one isolated instance failure when evidence proves common cause.

## 17. Retry and anti-amplification

Retries are bounded by purpose, end-to-end deadline, idempotency, provider quota/cost/rate, circuit state, backoff policy, duplicate suppression, shared capacity and downstream demand.

```text
RETRYABLE != RETRY_UNBOUNDED
```

Fallbacks cannot create retry storms across providers.

## 18. Bounded hedged acquisition

Parallel/hedged acquisition is permitted only when benefit justifies quota/cost/capacity and policy prevents amplification. It cannot violate entitlement, consume protected capacity without authority, create uncontrolled duplicate streams, confuse losing responses with corrections or become the default for all requests.

## 19. Operational vs research separation

```text
RESEARCH_RESULT -> LEARNING / CANDIDATE_EVIDENCE
RESEARCH_RESULT -/-> LIVE_OPERATIONAL_DATA_PRODUCT
RESEARCH_EGRESS != OPERATIONAL_PROVIDER_EGRESS
```

Research data may inform learning/candidates but must be reacquired/validated through the authorized operational FSAPMA path before operational use.

FCR-0008 research egress and FCR-0013 provider egress remain distinct Foundation authority contexts.

## 20. External provider egress and credentials

Operational provider connectivity requires the Foundation Stage 12/FCR-0013 egress and credential-reference boundary with exact Application/service-role/destination/environment/purpose binding.

```text
FSAPMA_EXTERNAL_PROVIDER_RUNTIME_CONNECTIVITY = NOT_YET_AVAILABLE
```

Secret bytes do not live in ordinary FSAPMA state or ordinary cross-Application payloads.

## 21. Delivery to consumers

Delivery uses P0-F declared contracts and current Foundation transport within exact implemented scope. Delivery preserves exact producer/consumer, Data Product identity/version, source/freshness, provenance, quality/degradation, operational/replay classification and correlation/causation where applicable.

FCR-0005 or its current successor state must be refreshed when end-to-end operational market-data delivery capability is material.

## 22. Interaction with Guardian

Guardian may consume FSAPMA readiness/degradation evidence and may issue accepted bounded provider-use protection directives. Guardian does not choose credentials, provider instances, Data Product correction logic or normal failover.

## 23. Interaction with FSTSimA

FSTSimA may consume explicitly non-Live replay/test/calibration Data Products through governed contracts. Non-Live data remains visibly non-operational and cannot create Live authority.

## 24. Awareness topology

FSAPMA has exactly one MSA, six LSAs and one current eligible CSA. Awareness evaluates FSAPMA and proposes improvements through P0-C; it does not replace operational provider controllers or self-authorize external egress.

## 25. Failure/degraded behavior

Required behaviors include:

- revoked instance -> isolate exact instance; use alternative only if eligible;
- provider-wide outage -> broaden failure scope based on evidence;
- stale cache -> preserve stale source state;
- stream gap -> incomplete until reconciled;
- quota exhaustion -> throttle/degrade, never launder another entitlement;
- conflicting sources -> preserve uncertainty;
- no authorized route -> Data Product unavailable;
- existing exposure -> consumer Risk/Guardian protection uses available trustworthy truth plus explicit uncertainty.

## 26. Explicit non-authority

FSAPMA SHALL NOT execute broker orders, own Trading strategies, choose position size, own Unified Risk, declare Guardian crisis state, use research egress as operational data, use broker execution credential authority merely because one vendor provides both services, create Foundation egress/security authority or pool credentials to expand entitlement.

## 27. Invariants

```text
FSAPMA = SOLE_FSATS_OPERATIONAL_EXTERNAL_DATA_GATEWAY
PROVIDER != PROVIDER_ACCOUNT != SERVICE_ROLE != API_INSTANCE
ACQUISITION_ENTITLEMENT != REDISTRIBUTION_ENTITLEMENT
PROVIDER_ACCESS != CONSUMER_USE_RIGHT
RESEARCH_EGRESS != OPERATIONAL_PROVIDER_EGRESS
MARKET_DATA_ROLE != BROKER_EXECUTION_ROLE
SAME_VENDOR != SHARED_AUTHORITY
CACHE_READ_TIME != SOURCE_FRESHNESS
RECONNECT != PROOF_OF_NO_GAP
MULTIPLE_API_INSTANCES != UNLIMITED_CAPACITY
POOLING != QUOTA_OR_ENTITLEMENT_LAUNDERING
OPERATIONAL_DATA_PRODUCT != ABSOLUTE_MARKET_TRUTH
ROUTE_LEASE != EGRESS_AUTHORITY
```

## 28. Forbidden interpretations

Invalid: Trading directly queries provider because FSAPMA is slow; same key means shared authority; multiple accounts multiply legal/global quota; recent cache read means fresh; reconnect proves continuity; source majority proves certainty; research source may be used Live because accurate; Route Lease grants network authority.

## 29. Mandatory scenarios

Test revoked API instance; provider-wide outage; slow endpoint; stale recent cache; gap/reconnect; snapshot/delta mismatch; provider conflict; adjusted/unadjusted conflict; precision loss; quota exhaustion; shared credential dual role/shared limit; customer/broker-linked entitlement attempted as Falcon-wide quota; provider-only instance attempted for broker execution; research data entering operational product; missing redistribution use right; false circuit recovery; and retry-storm amplification.

## 30. Exit gates

```text
SOLE_OPERATIONAL_DATA_GATEWAY = FSAPMA
DATA_PRODUCT_SEMANTICS = COMPLETE
PROVIDER_ACCOUNT_SERVICE_ROLE_INSTANCE_SEPARATION = PASS
ENTITLEMENT_SCOPE = PASS
CONTINUITY_CORRECTION_MODEL = PASS
PRECISION_UNIT_ADJUSTMENT_MODEL = PASS
NO_QUOTA_LAUNDERING = PASS
FAILURE_DOMAIN_CONTAINMENT = PASS
RESEARCH_OPERATIONAL_CONTAMINATION_PATHS = 0
FCR0013_STATE = EXPLICIT_AND_RUNTIME_FAIL_CLOSED
```

## 31. Non-grant

Acceptance of P0-G would establish FSAPMA/data design only. It would not authorize external provider connectivity, credentials, route activation, broker execution, Paper, Shadow, Tiny-Live, Live or deployment.