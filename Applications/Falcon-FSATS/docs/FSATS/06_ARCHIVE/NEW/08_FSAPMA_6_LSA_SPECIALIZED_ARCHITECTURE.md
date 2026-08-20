# Falcon Self-Aware Provider Management Application — 6-LSA Specialized Implementation Architecture

**Package:** `FSATS-SIA-v0.1`
**Application:** `APP-PMA`
**MSA:** `MSA-PMA`
**Status:** `DESIGN_CANDIDATE`

## 1. Mission

FSAPMA is the sole FSATS Application owner for operational external market/reference/provider-data acquisition, provider business capability knowledge, normalization, quality, reconciliation, quota/cost/reliability and governed Data Product delivery.

```text
EXTERNAL PROVIDER DATA
-> FSAPMA
-> CANONICAL DATA PRODUCT
-> GOVERNED FOUNDATION TRANSPORT
-> AUTHORIZED CONSUMER APPLICATION
```

Trading and Guardian SHALL NOT directly implement operational provider clients.

## 2. Provider Adapter Rule

Every provider integration implements an internal FSAPMA adapter contract. Presence of an adapter does not activate or authorize the provider.

Adapter identity binds:

```text
ProviderId
AdapterId
AdapterVersion
Supported Provider Service/API Version
Supported DataProductClass set
Required CredentialReferenceClass
Endpoint/Destination Class
NormalizationProfileVersion
RateLimit/QuotaProfileVersion
HealthCheckProfileVersion
```

Adapters SHALL NOT expose raw third-party SDK types outside the adapter boundary.

## 3. P-LSA-01 — Provider Registry & Onboarding

### Components

- `P01.ProviderRegistry`
- `P01.ProviderDefinitionValidator`
- `P01.AdapterRegistry`
- `P01.ProviderLifecycleEvaluator`
- `P01.ProviderProfileVersionStore`

### Owned state

- canonical `ProviderId`;
- provider display/legal metadata needed operationally;
- immutable provider profile versions;
- adapter compatibility declarations;
- onboarding state;
- operational enable/disable business state, distinct from Foundation external-egress authority.

### Provider lifecycle

```text
DISCOVERED
-> PROFILED
-> ADAPTER_AVAILABLE
-> VALIDATED
-> ELIGIBLE
-> ENABLED

side states:
DISABLED
DEGRADED
QUARANTINED
RETIRED
INCOMPATIBLE
```

A provider cannot become ENABLED unless required P-LSA-02/03/04/05/06 declarations exist and the Foundation egress/credential capability is available for runtime use.

### Compatibility

Provider profile/adapter/API changes are versioned. Unknown incompatible external API behavior => provider route unavailable; no best-effort guessing.

### Concurrency

Registry writes serialized per ProviderId; readers consume immutable profile snapshots.

### CSA

No CSA by default. Registry/onboarding is governed deterministic infrastructure.

## 4. P-LSA-02 — Data Products, Semantics & Normalization

### Components

- `P02.DataProductRegistry`
- `P02.SchemaMapperRegistry`
- `P02.NormalizationPipeline`
- `P02.UnitPrecisionNormalizer`
- `P02.SymbolIdentityResolver`
- `P02.CanonicalObservationBuilder`
- `P02.NormalizationVerifier`

### Principle

Raw provider payloads are never canonical merely because they parse successfully.

Normalization pipeline:

```text
RAW PAYLOAD
-> adapter parsing
-> provider schema validation
-> provider identity/instrument mapping
-> units/time/precision normalization
-> canonical DataProduct schema validation
-> provenance binding
-> immutable normalized observation
```

### Data Product definition

Each product declares:

```text
DataProductId + Version
Class
Market applicability
Canonical schema fields/types/units
Required/optional fields
Source timestamp semantics
Arrival/observation timestamp refs using Foundation time
Freshness rules
Ordering key/sequence semantics
Correction semantics
Duplicate identity semantics
Provider mapping profiles
Quality policy
Consumer compatibility profile
```

### Instrument mapping

Provider symbol/name is not the canonical instrument identity.

Mapping requires exact provider symbol namespace + market/venue + effective version. Ambiguous mapping => reject/quarantine observation with `PMA_SYMBOL_MAPPING_AMBIGUOUS`.

### Price/quantity normalization

No binary floating conversion. Provider numeric data is parsed into exact decimal semantics and validated against the canonical instrument profile where known.

### Corporate actions / corrections

Historical corrections append a corrected/superseding observation relationship. They do not rewrite previously delivered evidence as if the corrected value had been known earlier.

### CSA

`DataQualityAnomalyModel` belongs primarily to P-LSA-05, not normalization. Normalization itself remains deterministic.

## 5. P-LSA-03 — Provider Capability, Account & Entitlement

### Components

- `P03.ProviderAccountRegistry`
- `P03.CapabilityCatalog`
- `P03.EntitlementResolver`
- `P03.MarketCoverageResolver`
- `P03.DataLatencyClassResolver`
- `P03.SessionCoverageResolver`
- `P03.CapabilitySnapshotStore`

### Capability dimensions

At minimum:

```text
DataProductClass
Market/Venue
Instrument class
Real-time/delayed/historical class
Session coverage
Granularity
Depth
History depth
Streaming/polling
Correction/backfill support
Quota unit model
Credential/plan requirement
Commercial/free entitlement class
```

### Entitlement truth

FSAPMA owns provider/business entitlement knowledge; it does not own Foundation credential authority.

An entitlement snapshot says what the provider account/plan is believed permitted to request. Runtime access still requires the Foundation egress/credential boundary.

### Effective capability

```text
EffectiveCapability = intersection(
  ProviderProfile,
  AdapterCapability,
  AccountEntitlement,
  Market/Session applicability,
  Foundation egress availability,
  Guardian/provider restriction,
  current provider health
)
```

If any required dimension is UNKNOWN, the route is ineligible for operations needing that dimension.

### Consistency

Capability snapshots are immutable/versioned and pinned to route plans.

## 6. P-LSA-04 — Provider Selection, Routing & Delivery

### Components

- `P04.ProviderController`
- `P04.RouteEligibilityEngine`
- `P04.RouteScorer`
- `P04.QuotaReservationClient`
- `P04.ProviderSessionManager`
- `P04.RequestScheduler`
- `P04.StreamSubscriptionManager`
- `P04.BackfillCoordinator`
- `P04.DeliveryPublisher`

`ProviderController` is an operational controller, not an MSA/LSA/CSA and not Foundation authority.

### Route eligibility hard gates

A provider route is eligible only if all required conditions pass:

1. provider/profile enabled and not quarantined;
2. compatible adapter available;
3. requested DataProduct capability/market/session supported;
4. entitlement supports request;
5. required Foundation external-egress/credential capability available and authorized when runtime is attempted;
6. provider not blocked by Guardian directive;
7. provider/route health above the product's minimum state;
8. quota reservation can be obtained;
9. required freshness/latency objective can plausibly be met;
10. required normalization/reconciliation profile exists.

### Default route score

For all eligible routes, initial score `0..10000`:

```text
30% current verified DataQualityScore
20% freshness/latency fitness
20% quota headroom fitness
15% rolling reliability
10% cost efficiency
 5% continuity/diversification benefit
```

Weights are versioned by `ProviderRoutePolicyVersion` and may differ by DataProduct, but there is always one exact active policy version. Tie-break:

1. higher verified DataQualityScore;
2. higher quota headroom;
3. lower cost;
4. lower observed latency;
5. canonical ProviderId lexical order.

### Multi-source policy

Each DataProduct declares one acquisition mode:

```text
SINGLE_PRIMARY_WITH_FAILOVER
PRIMARY_PLUS_VALIDATOR
MULTI_SOURCE_RECONCILED
BROAD_DISCOVERY_THEN_CONFIRMATION
```

A provider route cannot change acquisition mode dynamically without a versioned route-policy change.

### Failover

Failover is permitted only to an already eligible route. It preserves request/correlation identity and records source switch causation. A failover response is re-normalized and re-quality-checked; route eligibility does not imply data validity.

### Backfill

Gap detection requests backfill only where provider capability supports it. Backfill observations preserve original effective time and later arrival/retrieval evidence. They cannot be represented as live-at-the-time evidence.

### Delivery

Only normalized and P-LSA-05 dispositioned Data Products are published cross-Application. Foundation transport handles delivery; FSAPMA owns payload quality/business meaning.

### External egress gate

Actual Internet/provider access remains fail closed until FCR-0013 is implemented/available. No local HttpClient shortcut is a permitted substitute in a governed implementation.

## 7. P-LSA-05 — Data Quality, Verification & Reconciliation

### Components

- `P05.DataQualityEngine`
- `P05.FreshnessValidator`
- `P05.CompletenessValidator`
- `P05.CrossProviderConsistencyChecker`
- `P05.DuplicateCorrectionResolver`
- `P05.GapDetector`
- `P05.ReconciliationEngine`
- `P05.DataQualityAnomalyModel`
- `P05.QualityEvidenceBuilder`

### Quality dimensions

Initial normalized quality score:

```text
30% Freshness
20% Completeness
20% Internal consistency/schema validity
15% Cross-source consistency where available
10% Continuity/gap fitness
 5% Provenance confidence
```

Hard failures override score. An observation with invalid identity/schema/provenance cannot become valid merely due to a high numeric score.

### Quality state derivation

`VALID` requires all product-specific hard validators and score >= configured `VALID_MIN_SCORE`.

`DEGRADED` may be emitted only when hard identity/schema/provenance checks pass, quality is below VALID but above a product-specific usable floor, and the consumer contract explicitly allows degraded use.

`CONFLICTED` when material sources disagree beyond tolerance and no deterministic product-specific reconciliation rule can resolve the conflict.

`STALE` when freshness rule fails.

`INCOMPLETE` when required fields/window coverage fail.

`UNAVAILABLE` when no eligible evidence exists.

### Cross-source reconciliation patterns

The reconciliation rule is DataProduct-specific:

- quotes: compare bid/ask/mid/spread within exact time alignment tolerance; median/consensus may be used only under the versioned quote reconciliation profile;
- bars: reconcile OHLCV only when interval boundaries/source definitions are compatible; conflicting bars remain explicit rather than averaging incompatible semantics;
- trades: deduplicate by provider/source identity and normalized venue/time/price/quantity semantics; do not fabricate one synthetic authoritative trade from unrelated prints;
- order-book: source/venue depth is not merged across incompatible books unless a specific composite-book product declares that behavior;
- market status: conflict involving halt/session state fails closed for risk-increasing Trading use.

### Anomaly model

May raise anomaly evidence; it cannot override deterministic invalid identity/schema/provenance or convert CONFLICTED/STALE into VALID.

### CSA

`P05.DataQualityAnomalyModel` may be CSA-eligible.

## 8. P-LSA-06 — Quota, Capacity, Cost & Reliability

### Components

- `P06.QuotaProfileRegistry`
- `P06.QuotaLedger`
- `P06.QuotaReservationManager`
- `P06.CostLedger`
- `P06.ReliabilityTracker`
- `P06.DegradationForecaster`
- `P06.ProviderCapacityPlanner`
- `P06.FSARMResourceReporter`

### Quota model

Provider quota semantics are represented as versioned buckets:

```text
ProviderAccountId
QuotaBucketId
Scope (endpoint/product/global/etc.)
CapacityUnits
WindowType = FIXED | ROLLING | TOKEN_BUCKET | CONCURRENT_SESSION
WindowDuration where applicable
RefillRule
CostPerOperationUnits
ReservedUnits
ConsumedUnits
Reset/RefillEvidence
```

### Reservation

A provider request requiring quota must obtain an atomic `QuotaReservation` before dispatch when the provider quota model is known/reservable.

Duplicate idempotency key returns same compatible reservation. Reservation expires/release deterministically.

If provider quota semantics are unknown, route eligibility fails for operations where oversubscription can cause material service degradation unless a separately validated safe conservative policy exists.

### Cost

Financial provider costs and quota costs are distinct. Cost is evidence for route planning, never authority to choose low quality over a required data-quality floor.

### Reliability

Reliability tracks at least:

- request success rate;
- timeout/error rate;
- stale/conflicted data rate;
- stream disconnect frequency;
- recovery time;
- quota forecast accuracy;
- observed latency distribution.

Metrics are window/version bound. Historical reputation never overrides current hard unavailability.

### Provider reliability forecast model

May forecast degradation/quota exhaustion and is eligible for CSA. Forecasts cannot mint quota or bypass a route hard gate.

### FSARM report

FSAPMA reports technical workload resource needs to FSARM separately from provider API quota.

```text
PROVIDER_API_QUOTA != FOUNDATION_RESOURCE_ALLOCATION
```

Report includes internal compute/memory/network workload requirement, minimum-safe critical Data Product paths, deferrable backfills/discovery, reclaimability and consequences.

## 9. Data Product Delivery Contract

Cross-Application normalized product envelope business fields include at minimum:

```text
DataProductId/Version
ObservationId
MarketId
InstrumentId where applicable
EffectiveTimeRef
Observed/ReceivedTimeRef
SourceProviderIds[]
SourceObservationRefs[]
OperationalClassification
QualityState
QualityScore + ModelVersion
FreshnessStatus
Correction/Supersession refs
Payload
PayloadDigest
BusinessProvenanceRefs[]
```

Foundation FIL/event/transport correlation/causation/security fields are consumed, not redefined in payload semantics.

## 10. FSAPMA Degradation Policy

Default workload shedding sequence under Application resource pressure:

1. historical nonurgent backfill expansion;
2. exploratory provider discovery/profiling;
3. noncritical analytics/reliability recomputation frequency;
4. duplicate validator routes beyond the required minimum;
5. lower-priority market/universe breadth not currently needed;
6. reduce polling frequency only within consumer freshness contracts;
7. preserve current critical Data Product paths required for open-position safety/Guardian/essential Trading decisions as long as the valid resource envelope permits.

If minimum required operational Data Product quality/freshness cannot be met, publish explicit degradation/unavailability; never fabricate healthy data.

## 11. Security

FSAPMA never stores plaintext provider secrets in domain/persistence state. It stores governed credential-reference identity only where Foundation supports it.

Provider response bodies are treated as untrusted external input until parsing/schema/normalization validation.

Adapter/plugin allowlist and exact version/integrity verification are required before registration.

## 12. FSAPMA MSA

MSA-PMA consumes bounded Self-Knowledge from all six LSAs and understands provider/data business condition end to end.

It may recommend provider/profile/model/process improvements and coordinate self-development review. It cannot create external egress authority, provider credentials, Foundation resources or Trading business decisions.

Two independent Monitor AI perspectives apply under file 18.

## 13. Verification Families

The FSAPMA verifier SHALL cover at least:

1. exactly six LSAs + one MSA;
2. no provider adapter outside FSAPMA;
3. adapter presence != provider enablement;
4. raw payload cannot bypass normalization;
5. provider symbol != InstrumentId;
6. exact decimal/unit normalization;
7. capability = profile ∩ adapter ∩ entitlement ∩ runtime gates;
8. route hard-gate enforcement;
9. deterministic route scoring/tie-break;
10. atomic quota reservation/no oversubscription;
11. failover only to eligible routes;
12. correction/backfill truth preservation;
13. quality hard failure overrides score;
14. cross-provider conflict not silently averaged where semantics incompatible;
15. operational/simulation/replay classification preserved;
16. no local external-egress substitute for FCR-0013;
17. provider API quota separated from Foundation resource allocation;
18. resource degradation preserves minimum-safe Data Product paths;
19. CSA/model cannot override deterministic invalidity/authority;
20. deterministic rerun for identical provider evidence/policies.
