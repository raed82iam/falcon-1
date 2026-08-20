# FSATS Specialized Implementation Architecture — Market, Provider, Broker and Execution Profiles

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`
**Initial Markets:** `US_EQUITIES`, `CRYPTO_SPOT`
**Initial Exposure Model:** `FUNDED_1_TO_1 / NO_BORROWED_LEVERAGE`

## 1. Purpose

Define exact profile schemas and the initial fixed business selections while preventing volatile external API/exchange facts from being hardcoded as eternal architecture.

External provider/broker capabilities are admitted only after point-in-time certification against official current behavior. Historical V1.3 provider roles are retained as candidate onboarding targets, not asserted as permanently current API facts.

## 2. MarketProfile Schema

Every market profile is immutable/versioned and declares:

```text
MarketId
MarketProfileVersion
AssetClass
TradingCalendarPolicy
SessionPhases
Timezone/Calendar identity
InstrumentIdentityPolicy
PriceTickPolicy
QuantityStepPolicy
FractionalPolicy
ShortingPolicy
LeveragePolicy
OrderTypePolicy
TimeInForcePolicy
ExtendedHoursPolicy
MarketHaltPolicy
CorporateActionPolicy
SettlementPolicy
RequiredDataProducts
DataFreshnessProfiles
LiquidityEligibilityPolicy
ExecutionCollarPolicy
RiskOverlayPolicy
UniversePolicyVersion
StrategyApplicabilityOverrides
ProviderCapabilityRequirements
BrokerCapabilityRequirements
```

A new exchange/venue/product is a new/updated profile, not an if-statement scattered through strategies.

## 3. US Equities Initial Profile

### Identity

```text
MarketId = US-EQUITIES
AssetClass = EQUITY
ExposureModel = FUNDED_1_TO_1
BorrowedLeverage = DISABLED
Shorting = DISABLED_IN_INITIAL_PROFILE
Fractional = ALLOWED_ONLY_WHEN_BROKER_AND_INSTRUMENT_CAPABILITY_CONFIRM
```

### Trading calendar

Use an authoritative versioned US-equity exchange-calendar dataset/profile. Do not hardcode a yearly holiday list in strategy code.

Business phases:

```text
CLOSED
PRE_MARKET
REGULAR
AFTER_HOURS
HALTED
HOLIDAY
UNKNOWN
```

The exact exchange/venue schedule and exceptional closes are data/profile inputs. `UNKNOWN` blocks new risk.

### Required canonical data products

For baseline Trading eligibility:

- instrument/reference definition;
- market/session status;
- quote or executable price context sufficient for the strategy/order type;
- trades/bars required by active strategy features;
- corporate-action state where material;
- liquidity/spread evidence;
- provider quality/freshness state.

A strategy may require additional products.

### Universe zones

Initial current Owner-directed business structure:

```text
ZONE_C: deployable capital < 500 USD
  candidate price envelope < 30 USD
  select top 10 qualified instruments

ZONE_B: 500 <= deployable capital < 2000 USD
  tier-B candidate price envelope < 250 USD
  select top 10 qualified tier-B instruments
  + retain current qualified Zone-C set

ZONE_A: deployable capital >= 2000 USD
  tier-A candidate price envelope >= 250 USD
  select top 10 qualified tier-A instruments
  + retain current qualified Zone-B and Zone-C sets
```

Price only forms the zone eligibility envelope. Ranking uses the multidimensional T-LSA-02 score and can remove/replace instruments whose score or hard eligibility degrades.

`TOP_10` is a target selected count after eligibility, not a guarantee that ten safe instruments exist. If fewer than ten pass, return fewer; never lower hard gates to fill quota.

### Fractional shares

A high-price stock may remain economically eligible when fractional trading is supported, but Zone categorization remains the current Owner-directed structure. Fractional capability affects executable quantity/affordability, not data/liquidity/risk eligibility.

### Initial order policy

Canonical engine supports MARKET/LIMIT/STOP/STOP_LIMIT, but a broker/account/instrument/session intersection controls actual eligibility.

Risk-increasing MARKET orders require a stricter price/slippage collar and sufficient live price/liquidity quality. When executable price uncertainty exceeds policy, prefer LIMIT or NO_TRADE rather than unconstrained market submission.

Extended-hours execution is separately capability-gated and does not inherit REGULAR-session assumptions.

## 4. Crypto Spot Initial Profile

```text
MarketId = CRYPTO-SPOT
AssetClass = CRYPTO_SPOT
ExposureModel = FUNDED_1_TO_1
BorrowedLeverage = DISABLED
Derivatives = DISABLED
ShortBorrow = DISABLED
Session = CONTINUOUS subject to venue maintenance/halt
```

### Market phases

```text
CONTINUOUS
VENUE_MAINTENANCE
VENUE_HALTED
UNKNOWN
```

No fake daily opening-bell model. Analysis windows may use UTC or strategy-defined rolling boundaries, but the market remains continuous.

### Venue/account capability

The effective crypto instrument universe is the intersection of:

- selected broker/exchange account capability;
- funded spot product availability;
- exact base/quote asset mapping;
- min notional/quantity step/price tick;
- operational Data Product quality;
- liquidity/execution thresholds;
- Guardian restrictions;
- current market profile.

No architecture assumption names a venue/product as permanently available merely because a provider historically supported crypto.

### Required data

At minimum active strategies declare needed trades/quotes/bars and, where available/required, order-book depth. 24/7-specific regime transition strategy uses rolling windows rather than equity session gap logic.

### Settlement/capital

Capital and position accounting uses exact base/quote asset units. No implicit USD conversion for a non-USD quote pair. Valuation requires an explicit validated conversion path.

## 5. MarketProfile Validation

Reject profile when:

- price/quantity precision absent;
- order/TIF support ambiguous;
- required Data Products undeclared;
- market session/calendar behavior ambiguous;
- leverage/shorting state unknown;
- broker/provider capability requirement absent;
- unsupported settlement asset semantics;
- risk overlay missing;
- profile version not bound to strategy/instrument decision evidence.

## 6. Historical Initial Provider Pool Preserved

Historical agreed V1.3/V1 design knowledge identifies the initial pool of 13 providers:

1. Alpaca
2. Massive (formerly Polygon.io)
3. Twelve Data
4. Finnhub
5. Alpha Vantage
6. Tiingo
7. Financial Modeling Prep
8. Tradier
9. SEC EDGAR
10. FRED / ALFRED
11. Nasdaq Data Link
12. Marketstack
13. EODHD

This remains the **candidate onboarding pool**, not a claim that all are active or have unchanged current APIs/plans/capabilities.

Historical role intentions are retained:

| Provider | Candidate business role |
|---|---|
| Alpaca | primary US-equity/crypto data candidate + selected initial Paper execution provider |
| Massive | detailed/high-resolution market-data validation candidate |
| Twelve Data | multi-market general/fallback candidate |
| Finnhub | non-price/fundamental/news/intelligence candidate |
| Alpha Vantage | free fallback/verification candidate |
| Tiingo | historical/research/validation candidate |
| Financial Modeling Prep | fundamentals/company-profile candidate |
| Tradier | equities/options data + reserve Paper sandbox candidate |
| SEC EDGAR | official filing/regulatory source |
| FRED / ALFRED | macro series/vintage source |
| Nasdaq Data Link | research/alternative datasets candidate |
| Marketstack | lower-priority historical/intraday backup candidate |
| EODHD | broad fallback dataset candidate |

## 7. Provider Certification Profile

Before a provider becomes `ELIGIBLE`, FSAPMA must capture point-in-time certification evidence:

```text
ProviderId
ProviderProfileVersion
CertifiedAt
OfficialDocumentationEvidenceRefs
AdapterVersion
Account/PlanIdClass
CredentialReferenceClass
AllowedEndpoint/DestinationClasses
Markets/Venues
DataProductCapabilities
Streaming/Polling/Batch capabilities
Historical depth/granularity
Session coverage
Rate/Quota model
Concurrency/session limits
Latency/freshness observations
Correction/backfill behavior
Entitlement/redistribution constraints relevant to system use
Cost model
Known upstream feed/source lineage
Known limitations
Test fixture results
CertificationExpiry/RevalidationTrigger
```

A change in provider plan/API/entitlement/upstream feed that affects capability invalidates the affected certification until revalidated.

## 8. Provider Independence Rule

Two providers are not automatically independent evidence sources if they derive from the same upstream feed.

Provider certification records known `UpstreamSourceLineage`. Cross-source validation requiring independence counts unique sufficiently independent upstream sources, not provider brand names.

## 9. Canonical Data Product Source Rule

For one canonical Data Product observation/window at a given decision boundary, one exact reconciliation profile decides the canonical output.

Forbidden examples:

- build one candle with Open from Provider A and High from Provider B without a declared composite/reconciliation algorithm;
- use one source's trade stream and another source's volume and present the mixture as one native source truth;
- call two wrappers around the same IEX feed independent confirmation.

Other sources may validate, reconcile or fail over according to P-LSA-05 rules.

## 10. Initial Active Provider Target Count

Historical V1.3 fixed baseline recorded `13 providers / 7 initial active targets`.

The new SIA preserves the **capacity objective** that initial production-ready provider certification SHOULD target a diverse subset sufficient to cover the required US-equity, crypto, reference/fundamental/macro and validation needs, but it does **not** blindly freeze the historical exact seven identities without current capability certification.

Before code-ready semantic freeze, the provider onboarding plan must publish:

```text
13 CANDIDATE POOL
-> CURRENT CERTIFICATION
-> EXACT INITIAL ACTIVE TARGET SET
-> ROLE / PRODUCT COVERAGE PROOF
```

If fewer providers are needed to meet all requirements safely and independently, do not create calls solely to reach the number seven. If more are needed, the Owner may expand the set through governed design update.

## 11. BrokerProfile Schema

Every broker profile declares:

```text
BrokerId
BrokerProfileVersion
EnvironmentClass
AccountType
Markets
SupportedInstrumentClasses
OrderTypes
TIFs
FractionalPolicy
Shorting/LeveragePolicy
ExtendedHoursPolicy
ClientOrderId/IdempotencySemantics
OrderStatusMapping
Fill/Correction/BustSemantics
Cancel/ReplaceSemantics
AmbiguousSubmissionReconciliationMethod
Rate/SessionLimits
Account/BuyingPowerEvidence
Fees/Commissions profile
Settlement behavior
CredentialReferenceClass
EndpointDestinationClasses
OfficialCertificationEvidenceRefs
RevalidationTrigger
```

No broker adapter may invent behavior when profile is UNKNOWN.

## 12. Initial Paper Broker Decision

Preserved Owner/agreed historical selection:

```text
SELECTED INITIAL PAPER EXECUTION PROVIDER = ALPACA PAPER
RESERVE PAPER CANDIDATE = TRADIER SANDBOX
NORMAL PAPER CYCLE ACTIVE BROKER COUNT PER ACCOUNT = 1
```

A second broker may be used in FSTSimA/comparison with isolated accounts/evidence. Do not submit the same production-like Paper account intent to multiple brokers and reconcile it as one order chain.

Paper is an external-provider/broker connection and still requires the relevant Foundation external egress/credential capability before runtime activation.

## 13. Alpaca Historical Free Profile

Historical agreed profile recorded:

```text
Alpaca Plan: Basic Free
US Equity Feed: IEX
Paper Execution: Alpaca Paper
Live Execution: Disabled
SIP: Paid Optional Future
```

This is historical/candidate configuration only until current official certification confirms the available plan/feed semantics at implementation/activation time.

Important invariant regardless of branding/current plan:

- a limited exchange/feed source SHALL NOT be represented as full US market consolidated truth;
- source-specific volume SHALL NOT be labeled total-market volume;
- source-specific quote SHALL NOT be labeled NBBO unless the certified feed actually establishes that property.

## 14. Broker Route Selection

Initial policy: one active broker route per TradingAccountId/environment.

Selection eligibility requires:

- account mapping exact;
- environment authority exact;
- instrument/market/order capability exact;
- broker health available;
- Guardian restriction allows route;
- credential/egress boundary available;
- current broker profile certified;
- no unresolved account/order reconciliation conflict.

If the active Paper broker becomes unavailable, automatic failover to a reserve broker is **not** permitted for an existing account/order chain unless a separately designed account/capital/position migration/reconciliation proves semantic continuity. New isolated Paper experiment may use reserve broker under a new account/profile context.

## 15. Execution Price Collar

Every risk-increasing order has an `ExecutionPriceEnvelope` derived from:

```text
ReferencePriceSource/DataProduct
ReferencePrice
MaximumAdverseSlippage
MaximumSpread
MaximumPriceAge
OrderType-specific rules
Market session/profile
Risk/Guardian additional collar
```

For BUY, max executable price cannot exceed the computed upper collar.
For SELL reducing a LONG, protective rules may choose a different emergency collar; exact Guardian/risk-reducing policy is separately versioned.

If market moves outside collar before submission/replacement, re-evaluate/reject; do not widen automatically because order is urgent.

## 16. Quantity Normalization

Risk/Capital computes maximum economic quantity. T-LSA-09 normalizes to broker/instrument step:

- risk-increasing quantity rounds down;
- minimum notional/quantity must still pass;
- fractional precision uses certified broker/instrument profile;
- if normalized quantity is zero/below minimum -> NO_EXECUTABLE_QUANTITY;
- never round up beyond Risk/capital ceiling.

## 17. Fees / Slippage

Position sizing and capital reservation include expected fee/slippage buffer from the current execution-cost profile.

Final accounting uses actual reconciled broker fees/fills. Difference between expected and actual is attribution evidence, not silently ignored.

## 18. Market Halt / Unknown

On confirmed/possible market halt or unknown session:

- new risk is blocked;
- open orders are reconciled/canceled only where venue/broker allows and protection policy requires;
- position state remains authoritative even when no current price is available;
- valuation becomes degraded/unknown rather than zero;
- Guardian may restrict affected scope.

## 19. Provider/Broker External-Change Rule

Provider/broker facts are **certified external configuration**, not immortal architecture.

Implementation SHALL provide a certification verifier that fails if:

- official endpoint/API version no longer matches;
- capability fixture fails;
- quota/entitlement behavior materially differs;
- status mapping produces unknown values not covered by profile;
- credential/egress destination differs;
- data-feed lineage changes materially;
- provider/broker changes terms relevant to permitted system behavior.

Such a failure disables the affected capability and requires profile update/review, not code guessing.

## 20. Verification Families

Verifier/tests SHALL cover:

1. exactly two initial market profiles;
2. funded 1:1 initial exposure/no borrowed leverage;
3. US calendar/session UNKNOWN fail closed;
4. crypto continuous-session semantics;
5. precision/tick/quantity validation;
6. universe price zones are eligibility only, ranking independent/multidimensional;
7. fewer-than-10 safe candidates does not lower gates;
8. fractional capability intersection;
9. 13 historical provider candidates preserved;
10. provider presence != active eligibility;
11. point-in-time provider certification required;
12. upstream-source independence logic;
13. canonical Data Product no undeclared mixing;
14. exact current active-target set cannot be inferred solely from historical seven count;
15. Alpaca Paper initial selected / Tradier reserve preserved as candidate business decision;
16. one active Paper broker per account normal cycle;
17. no automatic broker failover across unreconciled account state;
18. limited feed cannot masquerade as consolidated market truth;
19. execution price collar enforced;
20. risk-increasing quantity never rounded above ceiling;
21. halt/unknown does not convert valuation to zero;
22. external capability change invalidates certification rather than being guessed around.
