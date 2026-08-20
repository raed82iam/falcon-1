# FSATS SIA — Initial Canonical Data Product and Quality Profile v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `RT-DATA-001`
**Owner:** APP-PMA / P-LSA-02 and P-LSA-05

## 1. Purpose

Materialize the exact initial operational Data Products consumed by Trading/Guardian/FSTSimA so provider adapters cannot assign incompatible semantics to the same generic label.

Every product is immutable/versioned and delivered through the cross-Application operational-data contract. Raw provider payloads are never canonical Data Products.

## 2. Initial Data Product IDs

```text
DP-001 = falcon.data.instrument.reference.v1
DP-002 = falcon.data.market.session-state.v1
DP-003 = falcon.data.market.quote-top.v1
DP-004 = falcon.data.market.trade-print.v1
DP-005 = falcon.data.market.bar.v1
DP-006 = falcon.data.market.orderbook-l2.v1
DP-007 = falcon.data.corporate-action.v1
DP-008 = falcon.data.news.normalized-event.v1
DP-009 = falcon.data.macro.series-point.v1
DP-010 = falcon.data.company.fundamental-snapshot.v1
```

DP-001 through DP-008 form the initial market/trading operational catalog. DP-009/010 are initial research/fundamental/reference catalog items and are not automatically required by current strategy versions.

## 3. Universal Observation Fields

Every canonical observation is bound to:

```text
DataProductId
DataProductVersion
ObservationId
MarketId?
InstrumentId?
EffectiveTimeRef
ReceivedTimeRef
SourceProviderIds[]
SourceObservationRefs[]
OperationalClassification
QualityState
QualityScore?
QualityProfileVersion
CorrectionOfObservationId?
SupersedesObservationId?
PayloadDigest
BusinessProvenanceRefs[]
```

Exact Foundation message/correlation/schema/security metadata remains Foundation-owned and is not duplicated as business truth.

## 4. Common Quality State

Hard states:

```text
VALID
DEGRADED
CONFLICTED
STALE
INCOMPLETE
UNAVAILABLE
UNKNOWN
```

A score is computed only after hard identity/schema/provenance validation passes.

Initial quality thresholds:

```text
VALID_MIN_SCORE = 8000 / 10000
DEGRADED_MIN_SCORE = 6500 / 10000
<6500 = INCOMPLETE/UNFIT according to failing dimension; never DEGRADED usable
```

Hard invalidity overrides numeric score.

For **new-risk Trading v1**, DP-001 through DP-008 are usable only when the exact required product is `VALID`, unless a StrategyVersion explicitly declares a narrower product-specific degraded acceptance. Current 14 StrategyVersion v1.0 definitions declare no degraded operational-data acceptance for new risk.

Protective valuation/reconciliation may use a separately governed degraded fallback when safer than no information; that fallback cannot be presented as VALID.

## 5. Quality Score Formula

For hard-valid observations/windows:

```text
30% Freshness
20% Completeness
20% Schema/InternalConsistency
15% CrossSourceConsistency when profile requires/has independent corroboration
10% Continuity/GapFitness
 5% ProvenanceConfidence
```

When a dimension is legitimately not applicable, its weight is redistributed proportionally across the applicable dimensions according to the exact product quality profile; the implementation SHALL NOT silently assign 10000.

Every delivered QualityScore records the dimension subscores used.

## 6. DP-001 — Instrument Reference v1

### Scope

One exact instrument version/effective record.

### Payload

```text
InstrumentId                   required
MarketId                       required
PrimaryVenueId?                optional where market model supports multiple
CanonicalSymbol                required display/reference symbol
ProviderSymbolMappings[]       provenance mapping refs, not authority
AssetClass                     required
BaseAssetId?                   required for crypto pair
QuoteAssetId/CurrencyId        required where price is quoted
PriceTick                      required >0
PriceDecimalScale              required 0..18
QuantityStep                   required >0
QuantityDecimalScale           required 0..18
MinimumOrderQuantity?          optional only if external capability not yet broker-specific
MinimumNotional?               optional provider/reference fact
FractionalReferenceCapability  TRUE | FALSE | UNKNOWN
InstrumentStatus               ACTIVE | HALTED | DELISTED | SUSPENDED | UNKNOWN
CorporateActionStateRef?       optional
EffectiveFrom                  required
EffectiveTo?                   optional
```

Broker-specific tradability remains BrokerProfile/effective capability, not DP-001 authority.

### Hard invalid

- missing/duplicate InstrumentId;
- MarketId mismatch;
- nonpositive tick/step;
- base/quote identity conflict;
- overlapping incompatible effective versions;
- unknown asset class for a Trading-required instrument.

### Freshness

Reference stays current until superseded/effective expiry, but the FSAPMA instrument-master route SHALL revalidate provider/reference status at least every 24 hours for an instrument in the active universe and immediately on known corporate-action/status events.

Missing revalidation -> DP-001 `STALE` for new-universe admission.

## 7. DP-002 — Market Session State v1

### Payload

```text
MarketId
SessionState = CLOSED | PRE_MARKET | REGULAR | AFTER_HOURS | CONTINUOUS | VENUE_MAINTENANCE | HALTED | HOLIDAY | UNKNOWN
SessionBoundaryId
SessionStart?
SessionEnd?
NewOrderEligibilityHint?       observation only, not authority
HaltReasonCategory?
CalendarProfileVersion
SourceMarketStatusRefs[]
```

Trading combines this observation with MarketProfile and broker/account capability. The hint cannot grant order authority.

### Freshness

```text
US Equities active session: max age 5 seconds
Crypto Spot active/continuous: max age 10 seconds
Closed/Holiday future-boundary record: valid to explicit next boundary if sourced from current calendar profile, but unexpected live halt/status events supersede immediately
```

UNKNOWN/STALE blocks new risk.

## 8. DP-003 — Top-of-Book Quote v1

### Payload

```text
InstrumentId
MarketId
BidPrice
BidQuantity
AskPrice
AskQuantity
QuoteVenueId/CompositeSourceId
QuoteSequence?                 source-supported monotonic sequence
QuoteCondition = NORMAL | LOCKED | CROSSED | INDICATIVE | UNKNOWN
ProviderEffectiveTimeRef
```

### Exact semantics

- prices/quantities are exact decimal canonical units;
- BidQuantity/AskQuantity represent the quantity associated with the delivered top quote at the declared source/venue, not total-market liquidity unless product source explicitly is a certified composite;
- `LOCKED` means BidPrice == AskPrice and may remain hard-valid if source condition is explicitly LOCKED;
- `CROSSED` means BidPrice > AskPrice and is `CONFLICTED` for v1 Trading new-risk use;
- `INDICATIVE` is not executable quote truth and is `DEGRADED`/unusable for v1 new risk;
- negative price/quantity is invalid; zero quantity makes the corresponding side incomplete.

### Freshness

Baseline:

```text
US Equities = 5 seconds
Crypto Spot = 10 seconds
```

Strategy-specific stricter profile in 17A overrides:

- HNT-002 US current context max age 2s;
- HNT-002 Crypto max age 5s;
- order-book strategies may require stricter DP-006 age.

## 9. DP-004 — Trade Print v1

### Payload

```text
InstrumentId
MarketId
CanonicalTradeObservationId
SourceTradeId?
TradePrice
TradeQuantity
VenueId?
TradeConditionCodes[]
TradeAction = NEW | CANCEL | CORRECT
OriginalTradeObservationId?   required for CANCEL/CORRECT
ProviderEffectiveTimeRef
SourceSequence?
```

### Correction semantics

`NEW` creates a trade observation.

`CANCEL` references an existing prior trade and produces a successor/correction event removing its contribution from corrected downstream aggregates; it does not delete the old event.

`CORRECT` references one prior trade and supplies the corrected replacement values in the new observation. The old trade remains historical evidence and is marked superseded for corrected aggregate rebuilds.

A correction whose original reference cannot be resolved => `RECONCILIATION_REQUIRED` for affected aggregate/window.

### Duplicate

Same canonical ObservationId + same digest = idempotent duplicate.
Same ID + different digest = integrity conflict.

### Freshness for real-time strategy flow

```text
US = 5 seconds
Crypto = 10 seconds
```

Historical trades used for completed bars/features are freshness-bound by the relevant window completeness, not current wall age.

## 10. DP-005 — Market Bar v1

### Supported interval IDs

```text
PT1M
PT5M
PT15M
PT1H
P1D_US_REGULAR_SESSION
PT24H_UTC
```

Intervals use canonical half-open time boundaries:

```text
[BarStart, BarEnd)
```

### Payload

```text
InstrumentId
MarketId
BarIntervalId
BarStart
BarEnd
Open
High
Low
Close
Volume
TradeCount?                    optional when source cannot supply
VWAP?                          optional but exact if supplied
BarState = FINAL | CORRECTED
SourceAggregationProfileId
UnderlyingObservationSetRef/Digest
```

### Numeric invariants

```text
High >= max(Open,Close,Low)
Low <= min(Open,Close,High)
Volume >= 0
TradeCount >= 0 when present
```

### Completion

Strategies consume only `FINAL` or latest `CORRECTED` completed bars.

A bar becomes `FINAL` only after the interval end and the source/reconciliation pipeline asserts completion under the product profile. An in-progress bar is not emitted as DP-005 FINAL and cannot enter strategies that specify completed bars.

### Expected final-arrival tolerance

For active operational paths:

```text
PT1M/PT5M: 15 seconds after BarEnd
PT15M: 30 seconds
PT1H: 60 seconds
P1D_US_REGULAR_SESSION: 5 minutes after regular-session close
PT24H_UTC: 5 minutes after UTC day boundary
```

Missing expected final bar after tolerance -> gap/INCOMPLETE/UNAVAILABLE until reconciled.

### US daily bar

`P1D_US_REGULAR_SESSION` covers exactly the regular session according to the versioned US market calendar, not pre/after hours.

### Crypto 24h bar

`PT24H_UTC` covers `[00:00 UTC, next 00:00 UTC)`.

## 11. DP-006 — Order Book L2 v1

### Payload

```text
InstrumentId
MarketId
BookSourceId
BookSequence
BookEffectiveTimeRef
Aggregation = PRICE_LEVEL
DepthLevelCount
Bids[]: {LevelIndex, Price, Quantity}
Asks[]: {LevelIndex, Price, Quantity}
BookState = SNAPSHOT | INCREMENTALLY_RECONSTRUCTED
UnderlyingSnapshotRef?        required for incremental reconstruction lineage
```

### Exact level rules

- LevelIndex starts at 1;
- bids sorted strictly descending price;
- asks sorted strictly ascending price;
- one price level appears once per side after canonical aggregation;
- quantity is summed only for orders/entries the provider product semantically defines as belonging to the same price-level book;
- Bid1 < Ask1 for NORMAL book; locked book allowed only when source profile says legitimate; crossed book = CONFLICTED;
- negative/zero quantities are invalid levels;
- sequence gap in an incrementally reconstructed book invalidates current book until resnapshot/reconciliation.

### Cross-venue rule

Books from different venues/sources are **not merged** into DP-006 v1.

A future composite book requires a different DataProductId/version and explicit merge semantics.

### Freshness

```text
US = 2 seconds
Crypto = 5 seconds
```

Strategies requiring DP-006 fail if depth is stale/incomplete. Strategies where depth is optional use the exact no-depth algorithm branch in 17/17A.

## 12. DP-007 — Corporate Action v1

### Payload

```text
CorporateActionId
InstrumentId
ActionType = SPLIT | REVERSE_SPLIT | CASH_DIVIDEND | STOCK_DIVIDEND | SYMBOL_CHANGE | MERGER | SPINOFF | DELISTING | OTHER
AnnouncementTime?
ExDate?
EffectiveTime
RatioNumerator/Denominator?   exact decimal/integer as applicable
CashAmount/Currency?          as applicable
NewInstrumentId?              as applicable
ActionStatus = ANNOUNCED | CONFIRMED | EFFECTIVE | CANCELED | CORRECTED
SourceOfficialityClass
CorrectionOfActionId?
```

### Hard rule

Unresolved effective split/symbol/delisting identity conflict blocks affected instrument new risk/universe eligibility.

Historical prices/features declare adjustment profile; no strategy silently mixes adjusted and unadjusted series.

## 13. DP-008 — Normalized News / Event v1

### Purpose

Provide a bounded normalized catalyst/event product for HNT-008 and later governed analysis without exposing raw unverified Internet text directly to Trading strategy code.

### Payload

```text
NormalizedEventId
EventCategory = EARNINGS | GUIDANCE | SEC_FILING | MACRO_RELEASE | CORPORATE_ACTION_NEWS | REGULATORY | PRODUCT_COMPANY_EVENT | MARKET_STRUCTURE_EVENT | OTHER
AffectedInstrumentIds[]
AffectedMarketIds[]
SourceEventRefs[]
SourceClass = OFFICIAL_FILING | OFFICIAL_AGENCY | ISSUER | LICENSED_NEWS | OTHER_VALIDATED
PublishedTimeRef
FirstObservedTimeRef
NormalizedSignificanceScore 0..10000
NormalizationConfidenceScore 0..10000
EventFacts[]                 structured key/value facts with schema/version, not arbitrary executable text
SummaryText?                 presentation/research field; NOT consumed by v1 strategy direction logic
CorrectionOfEventId?
EventStatus = NEW | UPDATED | CORRECTED | RETRACTED
```

### HNT-008 eligibility

Requires:

```text
QualityState = VALID
NormalizedSignificance >= 7000
EventAge <= 15 minutes
exact affected InstrumentId
not RETRACTED
no unresolved conflicting correction
```

HNT-008 direction still requires price/volume reaction confirmation. Neither SummaryText nor source sentiment directly creates a trade direction.

## 14. DP-009 — Macro Series Point v1

### Payload

```text
SeriesId
SeriesDefinitionVersion
ObservationPeriod
ReleaseTimeRef
VintageTimeRef?
Value
UnitId
SeasonalAdjustmentClass
RevisionOfObservationId?
SourceAgencyId
```

ALFRED-style vintages remain distinct; revised value does not rewrite what was known at an earlier decision boundary.

Current 14 strategy v1 catalog has no direct new-risk dependency on DP-009 unless a later StrategyVersion declares one.

## 15. DP-010 — Fundamental Snapshot v1

### Payload

```text
InstrumentId/IssuerId
SnapshotId
ReportingPeriod
Filed/PublishedTimeRef
Currency/Units
Fields[] with canonical FundamentalFieldId + value + unit
SourceDocumentRefs[]
Correction/Supersession refs
```

Different accounting periods/definitions are not averaged into one field without an explicit normalization profile.

Current v1 strategy catalog uses this as optional/future context, not a hard execution input unless declared.

## 16. Bar Construction / Provider Mapping

A provider may deliver native bars or raw trades.

Canonical DP-005 may be built only by a versioned `SourceAggregationProfile` that declares:

- native vs locally aggregated;
- timezone/session interval mapping;
- inclusion/exclusion of trade conditions;
- correction handling;
- volume definition;
- adjusted/unadjusted price policy;
- finalization/lateness behavior.

Two provider native bar products with materially different trade/session inclusion cannot be treated compatible merely because both are called "5 minute bar".

## 17. Product Substitution

Allowed only when:

- same exact canonical DataProductId/compatible version;
- same market/instrument identity;
- provider mapping profile certified to produce the canonical semantics;
- quality/freshness requirements pass;
- no source-lineage independence assumption is violated.

Forbidden:

- quote -> trade substitution;
- indicative -> executable quote substitution;
- regular-session daily bar -> 24h/extended-hours bar substitution;
- order-book source A + source B undeclared merge;
- raw news text -> DP-008 structured event substitution.

## 18. Strategy Product Requirement Matrix

Minimum initial product classes:

| Strategy | Required |
|---|---|
| CLS-001 | DP-001,002,003,005 |
| CLS-002 | DP-001,002,003,005 |
| CLS-003 | DP-001,002,003,005 |
| CLS-004 | DP-001,002,003,005 |
| CLS-005 | DP-001,002,003,005 |
| CLS-006 | DP-001,002,005 |
| HNT-001 | DP-001,002,003,005 |
| HNT-002 | DP-001,002,003,004,005 |
| HNT-003 | DP-001,002,003,005 |
| HNT-004 | DP-001,002,003,004,005; DP-006 optional branch |
| HNT-005 | DP-001,002,003; DP-006 preferred/required for depth branch |
| HNT-006 | DP-001,002,005 for both relationship instruments |
| HNT-007 | DP-001,002,003,005 |
| HNT-008 | DP-001,002,003,005,008 |

Execution additionally requires current DP-003 and exact instrument/session state even where a slow strategy decision used older completed bars.

## 19. Data Quality Usage By Consumer

Trading new risk:

```text
required DP state = VALID
```

Guardian may consume DEGRADED/CONFLICTED/STALE as protection evidence because the degraded state itself is the observation; it SHALL NOT reinterpret it as valid market truth.

FSTSimA may deliberately consume preserved invalid/degraded fixtures when run classification/scenario says so; simulation classification remains explicit.

## 20. Canonical Data Product Verification

Verifier/golden fixtures SHALL cover:

1. unique product IDs/versions;
2. exact payload required fields/types/units;
3. quote normal/locked/crossed/indicative cases;
4. trade NEW/CANCEL/CORRECT lineage;
5. duplicate ID/digest conflict;
6. bar half-open boundaries and final-only strategy use;
7. regular-session daily vs UTC-24h separation;
8. bar finalization/gap tolerance;
9. L2 sorting/sequence-gap/resnapshot behavior;
10. no cross-venue book merge;
11. corporate-action identity/split conflict;
12. normalized-event correction/retraction;
13. raw text cannot create HNT-008 direction;
14. quality hard invalidity overrides score;
15. score threshold 8000/6500 behavior;
16. current 14 strategy product requirement matrix;
17. source aggregation profile compatibility;
18. operational/replay/simulation class separation;
19. revised macro/fundamental evidence preserves historical vintage;
20. provider substitution only through certified canonical mapping.

## 21. Finding Disposition

```text
RT-DATA-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
INITIAL_CANONICAL_DATA_PRODUCTS = 10
CORE_TRADING_PRODUCTS = DP-001 THROUGH DP-008
QUALITY_THRESHOLDS = EXACT
STRATEGY_PRODUCT_MAPPING = EXACT
```
