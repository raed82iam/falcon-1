# P0-G — FSAPMA Operational Data Fabric

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-G only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-G defines FSAPMA as the sole FSATS operational external-data gateway and establishes a provider-independent data fabric that can safely use multiple providers, service roles, credentials/API instances, quotas, entitlements, fallback paths, and degraded modes without contaminating Trading with provider-specific coupling.

---

## 2. Responsibility

FSAPMA owns operational external-data acquisition, normalization, provider/service-role/API-instance capability modeling, Data Product formation, data entitlement/use-right enforcement, provider selection, operational data quality/provenance, continuity, correction/supersession, quota/capacity use, and provider-side resilience policy.

FSAPMA does not own Trading Risk, strategy selection, portfolio decisions, broker order execution, Guardian crisis authority, Foundation egress/security authority, or external broker execution authority.

---

## 3. Sole Operational Data Gateway

```text
OPERATIONAL_EXTERNAL_MARKET_DATA
 -> FSAPMA
 -> NORMALIZED / GOVERNED DATA PRODUCT
 -> DECLARED AUTHORIZED CONSUMER
```

Trading, Guardian, FSTSimA, Web, Communication, MSA/LSA/CSA, or another FSATS Application SHALL NOT bypass FSAPMA to acquire operational market data directly unless a later explicit architecture decision creates a different accepted owner.

Research-only Internet access is not an operational-data fallback.

---

## 4. Provider / Service Role / API Instance Separation

```text
PROVIDER != SERVICE_ROLE != API_INSTANCE
```

A Provider is the external vendor/entity.

A Service Role is a governed capability/purpose offered or consumed, for example market-data role, reference-data role, or broker-execution role.

An API Instance is an exact credential/configuration/endpoint instance used for an authorized role.

The same vendor or credential source may technically support multiple roles, but those roles retain separate authority and policy.

```text
SAME_VENDOR != SAME_AUTHORITY
SHARED_CREDENTIAL_SOURCE != SHARED_PURPOSE_PERMISSION
```

---

## 5. Provider Capability Graph

FSAPMA SHALL maintain a provider/service-role/instance capability model including as applicable:

- supported markets/asset classes;
- supported instrument identifiers;
- real-time/delayed/reference/history capabilities;
- streaming/snapshot capability;
- entitlement constraints;
- redistribution/use-right constraints;
- rate/quota/cost constraints;
- connection/session limits;
- data quality/precision/time semantics;
- supported corrections/adjustments;
- operational availability/health;
- role/environment restrictions;
- shared provider-side capacity dependencies.

A capability claim must be evidence-backed and versioned.

Unknown capability is not treated as supported.

---

## 6. Data Products

Trading consumers SHALL depend on provider-independent **Data Products**, not raw vendor semantics.

A Data Product contract SHALL define as applicable:

- Data Product identity/version;
- market/asset/instrument scope;
- fields/units/currency/precision;
- event/source time semantics;
- update type: snapshot/delta/stream/reference/history;
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
```

---

## 7. Entitlement and Use Rights

Acquisition permission and redistribution/consumer permission are separate.

```text
ACQUISITION_ENTITLEMENT != REDISTRIBUTION_ENTITLEMENT
PROVIDER_ACCESS != CONSUMER_USE_RIGHT
```

For each Data Product delivery, FSAPMA must be able to establish that the source/role/API instance and intended consumer/use remain permitted.

A user-owned broker/data entitlement SHALL NOT automatically become Falcon-wide entitlement.

---

## 8. Multi-User Provider Instance Pools

FSAPMA may manage multiple provider/API instances to improve resilience and capacity only when contracts/entitlements allow it.

Pools SHALL preserve:

- exact instance identity;
- exact owner/user/service role where relevant;
- credential isolation;
- quota/capacity accounting;
- entitlement scope;
- rate-limit state;
- health/degradation;
- shared vendor-side limits;
- failover eligibility.

Pooling SHALL NOT launder quota, licenses, account entitlements, or user-specific authority.

```text
MULTIPLE_API_INSTANCES != UNLIMITED_CAPACITY
POOLING != ENTITLEMENT_EXPANSION
```

---

## 9. Shared Capacity Modeling

If multiple roles/instances share one provider-side rate/cost/session/credential limit, that shared constraint SHALL be modeled once and consumed by all affected roles.

One service role SHALL NOT consume all capacity required for a higher-value or separately protected role without governed policy.

Business priority inside FSAPMA does not create Foundation technical criticality.

---

## 10. Provider Selection

Selection SHALL be policy/evidence based and reconstructable.

Inputs may include:

- required Data Product;
- entitlement;
- provider capability;
- freshness/quality;
- latency;
- continuity;
- health;
- quota/cost/capacity;
- environment;
- market/session;
- current degradation/circuit state.

The selected provider/API instance does not become permanent authority merely because it recently succeeded.

---

## 11. Route Lease

A Route Lease is a bounded, versioned, invalidatable selection cache indicating that a specific provider/service-role/API-instance choice remains eligible for a Data Product under exact conditions.

A Route Lease SHALL bind at least:

- Data Product requirement;
- provider/service-role/API-instance identity;
- entitlement context;
- capability/policy version;
- validity/expiry;
- relevant health/circuit epoch;
- reason/evidence.

It must be invalidated on material changes such as revocation, capability change, circuit state change, entitlement change, or incompatible policy update.

```text
ROUTE_LEASE != EXTERNAL_EGRESS_AUTHORITY
ROUTE_LEASE != DATA_QUALITY_GUARANTEE
```

---

## 12. Time, Continuity and Completeness

FSAPMA SHALL distinguish source/event time from receive/cache/read time.

```text
CACHE_READ_TIME != SOURCE_FRESHNESS
RECENT_RECEIPT != COMPLETE_STREAM
```

Stream continuity SHALL account for:

- sequence/gap detection where supported;
- snapshot/delta compatibility;
- reconnect/recovery;
- duplicate handling;
- correction/supersession;
- explicit unknown/missing intervals;
- bounded stale-cache use only when accepted for the exact purpose.

A reconnect is not proof that no gap occurred.

---

## 13. Precision, Units, Currency and Adjustments

Normalization SHALL preserve or explicitly transform:

- numeric precision;
- unit basis;
- currency;
- price/quantity scale;
- corporate-action adjustment status;
- instrument identity mapping;
- timezone/session/calendar semantics.

Lossy transformation that affects business meaning must be explicit and evidence-backed.

Adjusted and unadjusted data SHALL NOT be mixed silently.

---

## 14. Conflicting Providers

Conflicting values SHALL not be resolved by arbitrary “majority vote” or newest-receipt time alone.

Resolution policy may consider:

- provider role/quality evidence;
- source timestamp;
- entitlement/classification;
- expected latency;
- continuity state;
- known correction behavior;
- instrument identity consistency;
- cross-source confidence/uncertainty.

If operationally material conflict cannot be resolved safely, Data Product state becomes degraded/uncertain and downstream exposure authority is restricted according to consumer policy.

FSAPMA owns data truth/degradation classification, not Trading Risk action.

---

## 15. Circuit Breakers

Circuit state SHALL be scoped to the smallest correct provider/service-role/API-instance/endpoint failure domain.

A circuit breaker SHOULD distinguish:

- closed/healthy;
- open/unavailable;
- half-open/probe;
- degraded/limited where policy supports it.

False recovery must be challenged. A single successful probe is not automatically sufficient for full restoration.

Provider-wide outage SHALL not be incorrectly represented as one API-instance-only failure when evidence shows common failure.

---

## 16. Retry and Anti-Amplification

Retries SHALL be bounded by:

- purpose;
- end-to-end deadline;
- idempotency;
- provider rate/cost/quota;
- circuit state;
- exponential/backoff policy where applicable;
- duplicate suppression;
- shared capacity;
- downstream demand.

Cascading fallbacks SHALL not create retry storms across multiple providers.

```text
RETRYABLE != RETRY_UNBOUNDED
```

---

## 17. Bounded Hedged Acquisition

Hedged/parallel acquisition may be used only when the benefit justifies additional quota/cost/capacity and the policy prevents amplification.

It SHALL NOT:

- violate entitlement;
- consume protected capacity without authority;
- create uncontrolled duplicate streams;
- confuse the losing hedge response with an independent correction;
- become default behavior for all requests.

---

## 18. Operational vs Research Separation

Research data may inform learning/candidate development but cannot directly enter operational Data Products without being reacquired/validated through the authorized operational provider path and meeting all operational semantics.

```text
RESEARCH_RESULT -> LEARNING / CANDIDATE_EVIDENCE
RESEARCH_RESULT -/-> LIVE_OPERATIONAL_DATA_PRODUCT
```

FCR-0008 research egress and FCR-0013 operational provider egress are distinct authority contexts.

---

## 19. Provider Egress / Credential Boundary

FSAPMA external operational-provider connectivity requires a governed Foundation external-service egress/security capability with exact Application/service-role/destination/environment/purpose/credential-reference binding.

Current dependency: FCR-0013 is open and `Waiting On: FOUNDATION`.

Therefore:

```text
FSAPMA_OPERATIONAL_PROVIDER_DESIGN = VALID_CANDIDATE
FSAPMA_EXTERNAL_PROVIDER_RUNTIME_CONNECTIVITY = NOT_YET_AVAILABLE
```

Application-side design SHALL NOT create its own Foundation-equivalent secret/egress authority.

---

## 20. Market Data Delivery to Consumers

Delivery from FSAPMA to Trading/Guardian/other authorized consumers uses P0-F declared contracts and current Foundation transport capabilities.

FCR-0005 remains the end-to-end operational market-data delivery dependency where Foundation/Application verification remains open.

Delivery SHALL preserve:

- exact producer/consumer;
- Data Product identity/version;
- source/freshness state;
- provenance/lineage;
- quality/degradation;
- replay/operational classification;
- correlation/causation where applicable.

---

## 21. Failure / Degraded Behavior

Examples:

- one API instance revoked: isolate exact instance, select alternative only if eligible;
- provider-wide outage: broaden failure scope based on evidence, not per-instance fiction;
- stale cache: mark source freshness correctly;
- stream gap: mark incomplete until reconciled;
- quota exhaustion: degrade/throttle according to policy, do not launder another user's entitlement;
- conflicting sources: mark uncertainty and prevent fabricated certainty;
- no authorized provider path: operational Data Product unavailable, downstream new exposure may fail closed;
- existing exposure: consumers/Guardian/Risk manage protective behavior using available trustworthy truth and explicit uncertainty.

---

## 22. Explicit Non-Authority

FSAPMA SHALL NOT:

- execute broker orders;
- own Trading strategies;
- choose position size as Trading decision;
- own Unified Risk;
- declare Trading Guardian crisis state;
- use awareness research egress as operational data path;
- use broker-execution credential authority merely because same vendor provides data;
- create Foundation egress/security authority;
- turn pooled credentials into expanded entitlement.

---

## 23. Invariants

```text
FSAPMA = SOLE_OPERATIONAL_EXTERNAL_DATA_GATEWAY
PROVIDER != SERVICE_ROLE != API_INSTANCE
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
```

---

## 24. Forbidden Interpretations

Invalid interpretations include:

- “Trading can query the provider directly if FSAPMA is slow”;
- “same API key supports trading and data, therefore role authority is shared”;
- “two accounts double the provider's legal/global quota”;
- “recent cache read means data is fresh”;
- “stream reconnect means continuity was preserved”;
- “three providers agree, therefore truth is certain”;
- “research source may be used Live because it looks accurate”;
- “Route Lease grants network/credential authority”.

---

## 25. Mandatory Scenarios

At minimum test:

- revoked API instance among many;
- provider-wide outage;
- one slow endpoint;
- stale recent cache;
- gap then reconnect;
- snapshot/delta mismatch;
- conflicting providers;
- adjusted/unadjusted conflict;
- precision-loss transformation;
- quota exhaustion;
- shared credential dual role/shared limit;
- user broker credential attempted as Falcon-wide quota;
- provider-only instance attempted for broker execution;
- research data entering operational product;
- missing redistribution entitlement;
- circuit false recovery;
- retry-storm amplification.

---

## 26. Exit Gates

```text
SOLE_OPERATIONAL_DATA_GATEWAY = FSAPMA
DATA_PRODUCT_SEMANTICS = COMPLETE
PROVIDER_SERVICE_ROLE_INSTANCE_SEPARATION = PASS
ENTITLEMENT_SCOPE = PASS
CONTINUITY_CORRECTION_MODEL = PASS
PRECISION_UNIT_ADJUSTMENT_MODEL = PASS
NO_QUOTA_LAUNDERING = PASS
FAILURE_DOMAIN_CONTAINMENT = PASS
RESEARCH_OPERATIONAL_CONTAMINATION_PATHS = 0
FCR0005_STATE = EXPLICIT
FCR0013_STATE = EXPLICIT_AND_RUNTIME_FAIL_CLOSED
```

---

## 27. Next Authorized Gate

P0-G acceptance would establish FSAPMA/data design only. It would not authorize external provider connectivity, credentials, route activation, broker execution, Paper, Tiny Live, or Live operation.
