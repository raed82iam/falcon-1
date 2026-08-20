# FMOF / FSAPMA Provider Architecture Reconciliation — Free-First Capability-Aware Routing

**Package:** `FSATS-FMOF-PROPOSAL-001`  
**Applies To:** `00` + `01` + `02` of this proposal package and the historical FSATS V1.3 provider-management design  
**Supporting Point-in-Time Evidence:** `04_CURRENT_FREE_PROVIDER_CAPABILITY_MATRIX_2026-08-13.md`  
**Decision Type:** `PROJECT OWNER DIRECTED DESIGN CHANGE PROPOSAL / V1.3 RECONCILIATION`  
**Classification:** `DCC-3 — MATERIAL_DOMAIN_CHANGE`  
**Status:** `DESIGN_CHANGE_PROPOSAL / OUTSIDE_CURRENT_R7_FREEZE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Workspace:** `applications/docs/FSATS/NEW-2/`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Provider / Broker Connectivity Authority:** `NOT_GRANTED`  
**Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`  

---

# 1. Purpose

This proposal records the Project Owner's clarified provider-capacity and routing intent and reconciles it with provider-management mechanisms that were already substantially solved in historical FSATS V1.3.

The redesign SHALL NOT discard mature V1.3 concepts merely because FMOF introduces a stronger market/opportunity truth model.

The intended architecture is:

```text
FMOF
  Market / Instrument Truth
  Observation Coverage Truth
  Adaptive Discovery
  Global Opportunity
          |
          | canonical Data Product demand
          v
FSAPMA PROVIDER FABRIC
  Provider Registry
  Endpoint Capability Profiles
  Semantic Data Product Contracts
  Capability-Aware Provider Controller
  Quota / Capacity Management
  Protected Capacity
  Quality / Lineage / Reconciliation
  Entitlement / Usage Rights
  Controlled Fallback
          |
          v
Certified ObservationCoverage
          |
          v
FMOF analysis and opportunity pipeline
```

The central design correction is simple:

> FSAPMA SHALL first understand what the request actually needs, then select a provider route capable of satisfying that need. It SHALL NOT select a provider first and then weaken the request to match whatever that provider happens to offer.

---

# 2. Owner Intent Recorded

The current Owner-only evaluation phase intends to exploit lawful free-access capacity aggressively and intelligently so Falcon can approximate a much more expensive market-data service without provider-data subscription cost during the proof phase.

The target capacity has two additive layers.

## 2.1 Layer A — Dedicated Core Provider Capacity

The historical V1.3 thirteen-provider portfolio is preserved as the current **core candidate provider portfolio**:

1. Alpaca
2. SEC EDGAR
3. FRED / ALFRED
4. Finnhub
5. Financial Modeling Prep
6. Alpha Vantage
7. Twelve Data
8. Tiingo
9. Tradier Sandbox / applicable Tradier provider capability
10. Massive
11. Nasdaq Data Link
12. Marketstack
13. EODHD

The Owner intends at least three dedicated API credentials/connections/routes per provider **where the provider's legitimate credential and quota model actually supports them**.

Conceptually:

```text
13 CORE PROVIDER FAMILIES
        ×
TARGET 3+ DEDICATED PROVIDER LANES WHERE PERMITTED
        =
39+ INTENDED DEDICATED LANES
```

Those lanes are dedicated to provider/data acquisition work such as:

- market scanning;
- canonical universe observation;
- discovery;
- candidate qualification;
- deep analysis;
- historical acquisition;
- fundamental/non-price acquisition;
- freshness maintenance;
- cross-source verification;
- coverage recovery.

They are not execution lanes.

The numeric `39+` target SHALL NOT be interpreted as `39 independent quota pools`. Actual capacity is determined by certified `QuotaDomainIdentity`, provider terms, plan scope, endpoint weighting, account identity and other provider-specific limits.

## 2.2 Layer B — User/Broker-Derived Spare Capacity

Broker API connections belonging to the Owner now, and to future users only after commercial authorization, have a different purpose hierarchy.

Every such connection is first an execution/account-truth resource and only secondarily a possible market-data contributor.

```text
USER / OWNER BROKER API
        |
        +-- PRIMARY PROTECTED LANE
        |     Execution
        |     Order submit/cancel/replace
        |     Fills
        |     Open orders
        |     Positions
        |     Balance
        |     Buying power
        |     Account state
        |     Reconciliation
        |     Guardian/protection-critical reads
        |
        +-- SECONDARY OPPORTUNISTIC LANE
              only certified spare capacity
              market-data products only
              routed through FSAPMA
```

Mandatory invariant:

```text
PRIMARY_CAPACITY_IS_PROTECTED
PRIMARY_ALWAYS_PREEMPTS_SECONDARY
SECONDARY_MUST_NEVER_DEGRADE_EXECUTION_OR_ACCOUNT_TRUTH
```

User/broker market-data capacity is **supplemental**. Falcon's baseline provider fabric SHALL NOT require it in order to remain semantically correct.

---

# 3. Zero-Cost Personal Evaluation Principle

During the current Owner-only, personal, non-commercial evaluation profile established by `02_OWNER_CLARIFICATION_PERSONAL_EVALUATION_AND_FUTURE_COMMERCIALIZATION_GATE.md`:

```text
FREE_FIRST = TRUE
COST_CEILING_FOR_NORMAL_PROVIDER_SELECTION = ZERO
AUTOMATIC_PAID_PURCHASE = FORBIDDEN
AUTOMATIC_PAID_FALLBACK = FORBIDDEN
TERMS_BYPASS = FORBIDDEN
QUOTA_CIRCUMVENTION = FORBIDDEN
```

Falcon may combine complementary free capabilities to approximate a higher-value aggregate service.

It may not pretend that the aggregate is identical to a paid consolidated feed.

A free historical specialist, a quota-limited realtime specialist, a macro source and a regulatory source may all be valuable for different requests. Their differences are a feature of the portfolio, not an error to be hidden.

When no zero-cost route satisfies the exact requested semantics, FSAPMA SHALL return an explicit unavailable/degraded result according to the request's declared degradation policy rather than silently buying service or fabricating equivalence.

---

# 4. Request-First Routing Model

The canonical flow SHALL be:

```text
DataProductRequest
      |
      v
UNDERSTAND REQUIRED SEMANTICS
      |
      v
HARD COMPATIBILITY GATES
      |
      v
ELIGIBLE ProviderEndpointCapabilityProfiles
      |
      v
RIGHTS + COST + HEALTH + CAPACITY GATES
      |
      v
RANK ELIGIBLE ROUTES
      |
      v
SELECT ONE CANONICAL ROUTE
      |
      v
CanonicalRouteLease / DataProductResponse
      |
      v
ObservationCoverageSnapshot
```

A route that fails a hard semantic requirement cannot win because it is cheaper, faster, healthier or has more spare quota.

```text
SEMANTIC_INCOMPATIBILITY + HIGH_SCORE
= INELIGIBLE
```

---

# 5. Canonical DataProductRequest

Every operational data request routed through FSAPMA should carry enough semantics to determine whether a provider can actually satisfy it.

The design requires at least the following conceptual fields.

```text
DataProductRequest
  RequestId
  ProductType
  MarketScope
  InstrumentScope
  DecisionContext
  RequiredFields

  TemporalRequirement
    REALTIME_REQUIRED
    DELAYED_ACCEPTABLE
    EOD_ACCEPTABLE
    HISTORICAL_ONLY
    POINT_IN_TIME_REQUIRED

  MaxObservationDelay
  MaxDataAge
  HistoricalStart
  HistoricalEnd
  RequiredHistoryDepth
  Granularity
  SessionCoverage
  AdjustmentSemantics
  VenueScope
  ConsolidationRequirement

  TransportRequirementOrPreference
    REST
    WEBSOCKET
    BATCH
    FILE
    ANY_COMPATIBLE

  Deadline
  MaximumLatency
  MinimumQuality
  MinimumConfidence
  IndependentSourceRequirement

  AccountScopeIfApplicable
  BrokerScopeIfApplicable
  TenantOrUserScopeIfApplicable

  PurposeProfile
  DisplayMode
  RedistributionMode
  DerivedUseRequirement
  RetentionRequirement

  CostCeiling
  PriorityClass
  FallbackPolicy
  DegradationPolicy
```

`ProductType` remains canonical and provider-neutral. Examples include:

- instrument master / listing / tradability evidence;
- latest trade;
- quote;
- bar series;
- corporate actions;
- fundamentals;
- regulatory filings / XBRL;
- news / sentiment;
- macro/economic series;
- point-in-time macro vintage;
- broker/account capability evidence.

Applications request the product, not a vendor endpoint.

---

# 6. ProviderEndpointCapabilityProfile

Provider suitability SHALL be certified at service/plan/endpoint level, not merely by provider brand.

```text
ProviderEndpointCapabilityProfile
  ProviderFamilyId
  ServiceId
  PlanId
  EndpointId
  Transport
  CredentialIdentity
  QuotaDomainIdentity

  SupportedDataProducts
  SupportedMarkets
  SupportedInstrumentClasses
  SupportedFields
  VenueScope
  UpstreamLineageIdentity

  RealtimeMode
  DelaySemantics
  HistoryDepth
  Granularities
  SessionCoverage
  AdjustmentSemantics
  TimestampSemantics

  RateLimitModel
  EndpointWeight
  ConcurrencyLimit
  BatchLimit
  WebSocketLimit
  ResetModel

  CostClass
  EntitlementState
  UsageRightsProfile
  DisplayRights
  NonDisplayRights
  DerivedUseRights
  RedistributionRights
  RetentionRights

  HealthState
  FreshnessState
  LatencyState
  QualityState
  CorrectionBehavior

  CertificationInstant
  CertificationEvidence
  CertificationExpiryOrReviewTrigger
```

Canonical rule:

```text
PROVIDER_IS_NOT_GLOBALLY_GOOD_OR_BAD
CAPABILITY_IS_CONTEXTUAL
```

A provider can be excellent for 30-year EOD history and unusable for a realtime quote request. Another can be excellent for IEX streaming and invalid for consolidated NBBO. A third can be authoritative for filings and irrelevant for prices.

---

# 7. Mandatory Hard Gates

Before ranking, a candidate route SHALL pass all applicable hard gates.

## Gate 1 — Canonical product compatibility

Does the endpoint provide the requested Data Product and required fields?

## Gate 2 — Market / instrument coverage

Does it cover the actual market, listing and instrument class?

## Gate 3 — Temporal semantics

Does it satisfy realtime, delayed, EOD, historical or point-in-time requirements exactly?

## Gate 4 — Freshness and deadline

Can the route meet `MaxObservationDelay`, `MaxDataAge` and deadline?

## Gate 5 — Granularity / session / adjustment semantics

Does the route match bars, session scope, raw/adjusted behavior, timezone and corporate-action requirements?

## Gate 6 — Venue / consolidation semantics

IEX-only, derived reference, venue-limited, indicative and consolidated data SHALL remain distinguishable.

## Gate 7 — Usage rights / entitlement / purpose

The exact intended personal/internal/non-display/derived-use purpose must be permitted.

## Gate 8 — Cost ceiling

During the current normal Owner-only zero-cost phase, a route above `CostCeiling=0` is ineligible unless a later separate Owner decision explicitly authorizes it.

## Gate 9 — Health and current availability

A degraded/suspended/restricted route is excluded or handled only according to declared degraded semantics.

## Gate 10 — Quota / capacity

The route must have admissible capacity after protected reserves and current commitments.

## Gate 11 — Account / broker / user isolation

A user-derived route must be legally and architecturally shareable for the requested market-data purpose and must never leak private account state.

## Gate 12 — Independence requirement

If independent confirmation is required, two routes sharing the same upstream lineage do not count as two independent confirmations.

## Gate 13 — Guardian / protection restriction

A current Guardian restriction or governed safety restriction can make an otherwise capable route ineligible.

Only routes that pass applicable hard gates enter ranking.

---

# 8. Ranking After Eligibility

After hard eligibility is established, the Provider Controller may rank candidates using factors such as:

- semantic fidelity above the minimum;
- measured data quality;
- freshness;
- latency relative to request deadline;
- route health;
- remaining quota and reset horizon;
- ability to batch or reuse an existing stream;
- cache availability under valid freshness/rights rules;
- protection of scarce specialized capacity;
- upstream diversity when useful;
- zero-cost preference;
- load distribution across genuine independent quota domains.

Cost/quota may optimize **how Falcon observes** an opportunity. They SHALL NOT change the economic score of the opportunity itself.

```text
PROVIDER_COST_OR_QUOTA
!= MARKET_NATIVE_OPPORTUNITY_SCORE
```

---

# 9. Canonical Route and Fallback

For one canonical Data Product and decision context, FSAPMA SHALL identify one canonical authoritative route at a time unless the contract explicitly requests multiple independent observations for reconciliation.

Fallback routes must be prequalified.

Mandatory rules:

```text
DELAYED != REALTIME
EOD != INTRADAY
IEX_ONLY != CONSOLIDATED_US_MARKET
DERIVED_REFERENCE_PRICE != RAW_TOP_OF_BOOK
INDICATIVE_OPTIONS_PRICE != OPRA
SAME_UPSTREAM != INDEPENDENT_CONFIRMATION
```

A fallback SHALL NOT silently weaken semantics.

If the request says:

```text
TemporalRequirement = REALTIME_REQUIRED
DegradationPolicy = NONE
```

and no free eligible real-time route exists, the result is explicit unavailability.

If the request instead says:

```text
TemporalRequirement = DELAYED_ACCEPTABLE
```

then a certified delayed route may be eligible.

The consumer decides acceptable degradation in the request contract. FSAPMA does not invent it after failure.

---

# 10. Worked Routing Examples

## 10.1 US equity realtime quote where IEX semantics are acceptable

Request:

```text
ProductType = QUOTE
Market = US_EQUITIES
TemporalRequirement = REALTIME_REQUIRED
VenueScope = IEX_ACCEPTABLE
CostCeiling = 0
```

Potential eligible routes may include currently certified Alpaca Basic, Twelve Data or Tiingo IEX/derived-reference paths depending on the exact fields required.

SEC, FRED, FMP Free, Alpha Vantage Free, Marketstack Free and EODHD general Free are excluded because their free product semantics do not satisfy the request.

## 10.2 Regulatory 10-K XBRL facts

```text
ProductType = REGULATORY_FILINGS_XBRL
AuthorityPreference = OFFICIAL_SOURCE
```

SEC EDGAR is the natural specialized route if the exact product and access policy are satisfied. A realtime price provider is irrelevant even if it has enormous spare quota.

## 10.3 Macro series with revision-aware point-in-time truth

```text
ProductType = MACRO_SERIES
TemporalRequirement = POINT_IN_TIME_REQUIRED
VintageSemantics = REQUIRED
```

FRED/ALFRED is the specialist candidate. A stock quote provider cannot substitute merely because it returns an economic calendar or current macro value.

## 10.4 Long-depth EOD price history

```text
ProductType = BAR_SERIES
TemporalRequirement = HISTORICAL_ONLY
RequiredHistoryDepth = 20_YEARS_OR_MORE
Granularity = DAILY
CostCeiling = 0
```

The current dated capability matrix identifies Tiingo as a strong zero-cost candidate advertising 30+ years of history. Providers whose free depth is one or two years are hard-gated out for this request.

## 10.5 Consolidated US realtime NBBO

```text
ProductType = QUOTE
TemporalRequirement = REALTIME_REQUIRED
ConsolidationRequirement = CONSOLIDATED_NBBO
CostCeiling = 0
```

If no currently certified zero-cost dedicated route satisfies the exact consolidated entitlement, Falcon SHALL return:

```text
UNAVAILABLE_AT_CURRENT_COST_CEILING
```

It SHALL NOT relabel IEX-only or derived-reference data as NBBO.

---

# 11. Dedicated Provider Capacity Management

Dedicated provider credentials/routes are pooled by **real quota domain**, not by raw key count.

```text
Provider Family
   |
   +-- Service / Plan
          |
          +-- Credential A --+
          +-- Credential B --+--> QuotaDomainIdentity X, if provider defines them as shared
          +-- Credential C --+
```

or, where legitimately independent:

```text
Credential A -> QuotaDomain A
Credential B -> QuotaDomain B
Credential C -> QuotaDomain C
```

FSAPMA must discover and certify which model is true.

Forbidden behavior:

- opening duplicate accounts solely to evade provider limits where terms do not allow it;
- rotating keys to conceal quota exhaustion;
- treating IP/account/application-level limits as per-key limits;
- manufacturing synthetic independence from multiple credentials to the same upstream source.

The objective is maximum lawful efficiency, not quota evasion.

---

# 12. User/Broker Capacity Federation

User/broker connections introduce another capacity source, but only after primary obligations are reserved.

Conceptually:

```text
Dedicated Provider Capacity
        +
Certified Spare Broker-Market-Data Capacity
        =
Available Observation Fabric
```

For each broker connection:

```text
TotalBrokerApiCapacity
-
ProtectedExecutionAndAccountReserve
-
CurrentPrimaryCommitments
=
PotentialSecondaryMarketDataHeadroom
```

Secondary headroom may be admitted only if:

- the broker exposes the required market-data product;
- the use is permitted by entitlement and terms;
- exact `QuotaDomainIdentity` is known;
- use does not reduce execution/account/reconciliation safety margin;
- provider semantics and upstream lineage are known;
- FSAPMA certification is current.

When primary demand rises:

```text
SECONDARY_SCAN
-> THROTTLE
-> DEFER
-> CANCEL IF NEEDED

PRIMARY EXECUTION / ACCOUNT TRUTH
-> RETAIN PROTECTED CAPACITY
```

Private values such as balance, positions, orders and buying power SHALL never enter the shared market-data pool.

---

# 13. Scaling With More Users / Accounts

Future scale may increase aggregate observation capacity, but only when it introduces genuine independent quota domains.

The architecture SHALL measure:

```text
CertifiedIndependentQuotaDomains
```

not:

```text
RawUserCount
```

Examples:

```text
100 users + one application-wide shared broker quota
!= 100x capacity
```

while:

```text
multiple legitimately independent per-user quota domains
=> potentially greater aggregate spare capacity
```

The Provider Controller may distribute eligible market-data work across those domains, but user execution/account protection remains local and dominant.

More connections to the same upstream feed can increase capacity without increasing evidence independence.

---

# 14. Protected Capacity and Work Priority

The mature V1.3 protected-capacity idea is retained and strengthened.

Conceptual priority order:

1. Guardian / Trading protection critical work;
2. broker order/fill/position/account/reconciliation truth;
3. open-position monitoring;
4. near-trade decision monitoring;
5. active watchlist / active opportunity monitoring;
6. candidate qualification;
7. wide discovery / market scanning;
8. historical research / development / low-urgency backfill.

This ordering applies across dedicated and broker-derived capacity where relevant, but execution/account work exists only on the appropriate broker/account route.

Low-priority work yields first under pressure.

---

# 15. Efficiency Without Semantic Corruption

FSAPMA should maximize legal free capacity through mechanisms including:

- request deduplication;
- batching where provider semantics permit;
- shared validated stream fan-out where rights and tenant scope permit;
- cache reuse only when freshness, lineage, purpose and retention rules remain valid;
- local history reuse where provider terms permit storage;
- deadline-aware queues;
- cancellation of obsolete low-priority work;
- backpressure;
- quota-aware scheduling;
- staggered refresh across providers;
- coverage-debt recovery;
- no retry storms.

Efficiency SHALL NOT produce fake freshness or fake source independence.

---

# 16. Relationship to FMOF Truth Planes

FMOF's stronger truth separation remains controlling inside this proposal package.

A provider is an observation mechanism, not the owner of market existence.

```text
Provider unavailable
!= Instrument does not exist

Provider quota exhausted
!= Opportunity does not exist

Broker cannot trade instrument
!= Global opportunity does not exist
```

FSAPMA produces certified observation/data-coverage truth. FMOF uses that evidence while preserving:

1. Market / Instrument Truth;
2. Observation / Data-Coverage Truth;
3. Discovery / Opportunity Truth;
4. Broker Capability Truth;
5. Account Structural Eligibility Truth;
6. Risk / Capital / Execution authority truth.

Provider-data limitations can reduce confidence, freshness or analysis coverage. They must not silently erase upstream market/instrument truth or contaminate market-native opportunity scoring with provider economics.

---

# 17. Provider Portfolio State

The thirteen-provider set from V1.3 is preserved because it contains complementary specialist roles and a previously mature provider-management concept.

However:

```text
V1_3_PROVIDER_MEMBERSHIP = HISTORICAL_DESIGN_INPUT_PRESERVED
V1_3_OLD_ACTIVATION_STATE = NOT_AUTOMATICALLY_REACTIVATED
CURRENT_PROVIDER_CAPABILITY = POINT_IN_TIME_RECERTIFICATION_REQUIRED
CURRENT_RUNTIME_ACTIVATION = NOT_GRANTED
```

The dated `04_CURRENT_FREE_PROVIDER_CAPABILITY_MATRIX_2026-08-13.md` is the current research snapshot supporting this proposal. It is intentionally separate so volatile provider facts can later be superseded by a newer dated matrix without rewriting this stable architecture record.

---

# 18. Capability Re-Certification Lifecycle

Before any future route activation:

```text
DISCOVER CURRENT PROVIDER FACTS
-> VERIFY OFFICIAL DOCUMENTATION / TERMS
-> VERIFY PLAN / ACCOUNT / ENTITLEMENT
-> VERIFY ENDPOINT BEHAVIOR IN AUTHORIZED TEST ENVIRONMENT
-> BIND QUOTA DOMAIN
-> BIND UPSTREAM LINEAGE
-> VERIFY RIGHTS FOR EXACT PURPOSE
-> RECORD QUALITY / LATENCY / FRESHNESS
-> CERTIFY CAPABILITY PROFILE
-> OWNER / GOVERNED ACTIVATION DECISION AS REQUIRED
```

Material provider change, plan change, rights change, endpoint/schema change, rate-limit change, upstream change or unexplained runtime behavior triggers re-certification or restriction.

Unknown capability does not become permission.

---

# 19. Current Design Consequences

This proposal resolves the gap identified when comparing current FMOF with historical V1.3:

- FMOF retains its stronger broker-neutral market/opportunity truth model;
- V1.3 Provider Controller, canonical Data Products, endpoint capability profiles, quota management, protected capacity, quality, lineage and controlled fallback are retained/adapted rather than discarded;
- the Owner's dedicated thirteen-provider / multi-credential free-capacity strategy is explicitly represented;
- user/broker spare capacity becomes an additive secondary observation resource rather than a replacement for dedicated providers;
- provider selection becomes request-semantic driven;
- free-tier differences become schedulable strengths rather than hidden inconsistencies;
- paid capability is not silently required during the current personal proof phase;
- future commercialization can replace or upgrade plan/certification profiles without redesigning the routing architecture.

---

# 20. Required Future Review Before Acceptance

Because this is a material semantic design proposal outside the current accepted freeze, it must not be presented as accepted merely because it is now documented.

Required lifecycle remains:

```text
EXACT CANDIDATE MATERIALIZATION
-> FRESH ARCHITECTURE / CONSISTENCY REVIEW
-> FRESH RED-TEAM REVIEW
-> REMEDIATE AND REPEAT IF SEMANTICS CHANGE
-> PROJECT OWNER REVIEW
-> EXPLICIT OWNER ACCEPTANCE
```

Any later material change to the candidate invalidates review coverage for the changed semantics until reviewed again.

---

# 21. Non-Grant

This proposal does not:

- alter the accepted R7 freeze;
- activate any provider or broker route;
- authorize creation/provisioning of provider credentials;
- authorize use of user/broker credentials;
- authorize provider or broker connectivity;
- authorize implementation;
- authorize Paper, Tiny Live, Live or deployment;
- authorize paid subscriptions;
- grant commercial or redistribution rights;
- claim that three credentials equal three independent quotas;
- claim that the thirteen providers are thirteen independent confirmations;
- claim overall Owner acceptance of FMOF.

It records the exact candidate architecture to be reviewed through the governed FSATS process.
