# FSATS Complete Blueprint — FSAPMA 6-LSA Data Fabric Architecture

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Application:** `Falcon Self-Aware Provider Management Application (FSAPMA)`
**MSA:** `MSA-P`
**LSA Count:** `6`
**Implementation Authority:** `NOT GRANTED`

## 1. Mission

FSAPMA is the sole FSATS operational external provider-data gateway. It converts heterogeneous provider APIs into governed, attributable, normalized Data Products without leaking provider-specific mechanics into Trading, Guardian or other consumers.

```text
EXTERNAL PROVIDER
-> FSAPMA
-> NORMALIZED DATA PRODUCT
-> GOVERNED FOUNDATION TRANSPORT
-> CONSUMER APPLICATION
```

FSAPMA is not a broker execution gateway and is not a research Internet gateway.

## 2. Provider Identity Model

A provider is not equivalent to one API endpoint or one account.

Canonical hierarchy:

```text
PROVIDER
  -> SERVICE ROLE
      -> ACCOUNT / SUBSCRIPTION
          -> API INSTANCE / ENDPOINT PROFILE
```

Examples of Service Roles:

- equity quotes/trades;
- equity bars;
- crypto trades/quotes/order book;
- reference/master data;
- corporate actions;
- news/events where governed;
- historical data.

One commercial provider may expose several independent Service Roles with different quotas, entitlements, latency and reliability.

## 3. P-LSA-01 — Provider Registry and Onboarding

### Owns

- provider identity;
- service-role identity;
- account/subscription identity references;
- endpoint/API-instance metadata;
- legal/contractual usage profile supplied by Owner/configuration;
- onboarding state;
- provider retirement/suspension;
- capability discovery evidence.

### Components

- `ProviderRegistry`.
- `ServiceRoleRegistry`.
- `ProviderOnboardingController`.
- `ApiInstanceRegistry`.
- `ProviderStatusStateMachine`.

### Provider status

```text
DISCOVERED
-> REGISTERED
-> VALIDATED
-> AVAILABLE
-> DEGRADED / RESTRICTED / SUSPENDED
-> RETIRED
```

Registration never implies operational egress authority.

## 4. P-LSA-02 — Data Products, Semantics and Normalization

### Owns

Provider-independent business data products and semantic normalization.

Candidate Data Product families:

- instrument/reference identity;
- market/session status;
- trade tick;
- quote/BBO;
- bar/candle;
- order-book depth where available;
- corporate action;
- adjustment/reference factor;
- crypto market state;
- data-quality/continuity status;
- event/news product when separately governed.

### Required semantic fields

As applicable:

- product identity/version;
- market/instrument identity;
- provider/service-role/API-instance lineage;
- event/observation time;
- receive time;
- normalization time;
- source sequence where available;
- currency/unit/precision;
- raw/adjusted classification;
- session classification;
- correction/supersession identity;
- quality/confidence state;
- replay/test/operational truth class.

### Normalization rule

Normalization may align representation, units, symbols and semantics. It must not fabricate precision, source data, sequence, venue coverage or completeness not actually provided.

## 5. Symbol and Instrument Identity

FSAPMA maintains provider-specific symbol mapping to a canonical Application-level instrument identity.

Mappings include temporal validity because tickers/symbols can change.

```text
PROVIDER_SYMBOL
+ PROVIDER_ID
+ MARKET
+ VALID_TIME_RANGE
-> CANONICAL_INSTRUMENT_ID
```

Ambiguous mapping fails closed for operational trading consumption.

## 6. P-LSA-03 — Provider Capability, Account and Entitlement

### Owns

- current capability truth;
- subscription/entitlement truth;
- quota/rate-limit profile;
- historical lookback limits;
- streaming symbol limits;
- market coverage;
- session coverage;
- precision/depth capability;
- account-specific access differences;
- current capability confidence/freshness.

### Capability is dynamic

A provider capability can change because of:

- subscription plan;
- provider maintenance;
- account state;
- market session;
- endpoint health;
- entitlement changes;
- provider product changes;
- regulatory/provider restrictions.

Therefore capability is versioned current state, not a hardcoded constant.

## 7. P-LSA-04 — Provider Selection, Routing and Delivery

### Owns

Operational provider selection and delivery orchestration.

`Provider Controller` is an operational controller inside P-LSA-04 and is not a CSA by definition.

### Inputs

- requested Data Product;
- required market/instrument/session;
- minimum freshness;
- minimum coverage;
- required precision/depth;
- current provider health;
- entitlement;
- quota/capacity;
- cost profile;
- latency profile;
- quality history;
- reliability history;
- consumer criticality/expiry;
- permitted egress routes.

### Route decision

Provider selection should be evidence-weighted rather than permanently hardcoded.

Possible outcome:

```text
PRIMARY INSTANCE
+ OPTIONAL VALIDATION SOURCE
+ OPTIONAL FALLBACK INSTANCE
```

Hedging/multi-provider duplication is used only when value exceeds quota/cost/resource impact.

### Delivery rule

FSAPMA owns provider acquisition and normalization. Foundation communication owns generic cross-Application transport mechanics. FSAPMA does not recreate Service Bus/FIL/Foundation event semantics locally.

FCR-0005 remains open until actual Application delivery bindings and executable verification exist.

## 8. P-LSA-05 — Data Quality, Verification and Reconciliation

### Owns

Quality classification and provider-data reconciliation.

### Quality dimensions

- freshness/age;
- completeness;
- sequence continuity;
- duplicate rate;
- timestamp plausibility;
- price/quantity validity;
- precision/unit correctness;
- spread sanity;
- outlier score;
- provider divergence;
- session consistency;
- stale-stream detection;
- correction handling;
- corporate-action consistency;
- cross-source agreement where available.

### Quality states

```text
TRUSTED_FOR_INTENDED_USE
USABLE_WITH_LIMITS
DEGRADED
STALE
CONFLICTED
UNAVAILABLE
UNKNOWN
```

Quality is purpose-specific. Data adequate for a daily analytical chart may be inadequate for a sub-second execution/risk decision.

### Cross-provider verification

When multiple independent sources exist, reconciliation may improve confidence but never uses majority vote blindly. Provider correlation, coverage differences, latency and venue scope must be considered.

## 9. P-LSA-06 — Quota, Capacity, Cost and Reliability

### Owns

- rate-limit accounting;
- streaming subscription capacity;
- historical request budget;
- provider/account cost profile;
- retry budget;
- concurrency limits;
- circuit state;
- reliability statistics;
- quota reservation;
- degradation/shedding policy inside FSAPMA;
- resource evidence sent to FSARM.

### Free-tier aware orchestration

The provider controller treats free/basic plans as explicit capability envelopes rather than defects.

Example optimization:

```text
LOW-COST / BATCH DATA FOR BROAD UNIVERSE DISCOVERY
+
RICH / STREAMING DATA FOR SMALL ACTIVE SET
+
DYNAMIC SUBSCRIPTION ROTATION
```

Provider limits are never evaded by violating terms, rotating identities deceptively or hiding consumption.

## 10. Data Subscription Controller

FSAPMA maintains subscriptions according to instrument tier and use case.

Priority order is dynamic, but typical protected subscriptions include:

1. instruments with open positions;
2. instruments with active orders;
3. instruments required for protection/reconciliation;
4. active decision candidates;
5. active watchlist;
6. discovery candidates;
7. background research/analytics.

Under quota/resource pressure, lower classes degrade first.

## 11. Provider Health and Circuit Control

Each API instance maintains independent health:

- connectivity;
- latency percentiles;
- error rates;
- throttling state;
- data-quality state;
- sequence-gap state;
- authentication/entitlement state;
- provider-reported status;
- recent recovery stability.

Circuit behavior must distinguish:

- transport outage;
- quota exhaustion;
- entitlement denial;
- malformed data;
- stale stream;
- semantic conflict;
- provider-wide outage;
- single endpoint degradation.

Blind retry storms are forbidden.

## 12. Freshness and Deadline Model

Every request/data product may carry an intended-use freshness/deadline requirement.

A stale product is not automatically usable because it exists in cache.

```text
CACHE_HIT != CURRENT_ENOUGH
PROVIDER_ACK != DATA_QUALITY_PASS
RECEIVED != AUTHORITATIVE_FOR_INTENDED_USE
```

FCR-0009 may later provide richer Foundation transport deadline/QoS support. Until available, the Application design preserves business expiry semantics locally without claiming unavailable Foundation runtime behavior.

## 13. Crypto and Equity Differences

FSAPMA normalizes common concepts but preserves genuine market distinctions.

### US Equities

Potential distinctions include exchange/venue coverage, consolidated vs single-venue feeds, regular/extended/overnight sessions, corporate actions and fractional capability.

### Crypto Spot

Potential distinctions include 24/7 sessions, venue-specific market structure, different symbol conventions, continuous operation and order-book/data-product differences.

Normalization must not erase these distinctions.

## 14. Initial Alpaca Profile

Alpaca may initially appear in two separate roles:

- market-data provider Service Roles consumed through FSAPMA;
- broker execution role consumed by Trading Execution.

Those roles remain separately identified even if one commercial vendor supplies both APIs.

Paper and Live credentials/domains/environments are strictly separated.

Current public Alpaca Basic constraints such as limited equity real-time venue coverage and bounded websocket symbol subscriptions are treated as runtime capability data, not permanent architecture constants.

## 15. Provider Diversity

FSAPMA supports multiple providers to reduce single-source failure and quota dependence.

Provider onboarding does not require Trading code changes when the new provider can produce existing canonical Data Products.

A new provider-specific semantic that cannot map safely requires explicit Data Product/schema evolution rather than lossy coercion.

## 16. Provenance

Every operational Data Product is reconstructable to source acquisition evidence.

Minimum lineage where available:

```text
DATA_PRODUCT
-> NORMALIZATION VERSION
-> RAW OBSERVATION ID
-> API INSTANCE
-> SERVICE ROLE
-> PROVIDER
-> ACCOUNT/SUBSCRIPTION PROFILE
-> ACQUISITION TIME
```

## 17. Research Separation

External research by awareness does not pass through operational provider routing merely because both access the Internet.

```text
FSAPMA EGRESS = OPERATIONAL PROVIDER DATA
AWARENESS RESEARCH EGRESS = RESEARCH ONLY
BROKER EGRESS = EXECUTION ONLY
```

Credential sets, routes, audit and authority remain separated.

## 18. Security and Credential Rule

FSAPMA stores no raw long-lived secret in source or normal business state. It references governed credential identities once the Foundation credential-reference/egress boundary is implemented and authorized.

Until FCR-0013 runtime capability exists, real external operational provider connectivity remains blocked.

## 19. Resource Interaction

FSAPMA reports to FSARM:

- current effective consumption;
- minimum-safe data-service resource requirement;
- protected live-data paths;
- reclaimable caches/workers/background tasks;
- quota/resource pressure;
- degradation options;
- consequence of starvation;
- restoration need.

FSARM may coordinate compute/resource availability but cannot select providers, alter data quality truth or redefine Data Product semantics.

## 20. MSA-P

MSA-P evaluates complete provider-management fitness, including:

- provider diversity;
- systemic data quality;
- quota/cost efficiency;
- entitlement risk;
- provider concentration;
- routing quality;
- degradation readiness;
- cross-LSA consistency;
- candidate improvements.

## 21. Suggested CSA Candidates

Potential CSA eligibility:

- provider reliability predictor;
- anomaly/data-quality intelligence model;
- adaptive route-quality estimator;
- quota/capacity forecast model.

Provider Controller itself remains an operational controller; its intelligent subcomponent may be CSA-eligible if separately justified.

## 22. Acceptance Gates

```text
DIRECT_PROVIDER_BYPASS_PATHS = 0
PROVIDER_SERVICE_ROLE_AMBIGUITY = 0
CAPABILITY_ENTITLEMENT_CONFLATION = 0
FABRICATED_PRECISION_OR_COVERAGE = 0
UNKNOWN_SYMBOL_MAPPING_TO_OPERATIONAL_TRUTH = 0
UNBOUNDED_RETRY_STORMS = 0
RESEARCH_EGRESS_CONFLATION = 0
BROKER_EGRESS_CONFLATION = 0
UNATTRIBUTED_DATA_PRODUCTS = 0
QUOTA_EVASION_DESIGN = 0
```
