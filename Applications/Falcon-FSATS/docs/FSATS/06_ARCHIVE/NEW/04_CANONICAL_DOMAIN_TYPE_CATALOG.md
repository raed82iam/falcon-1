# FSATS Specialized Implementation Architecture — Canonical Domain Type Catalog

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`

## 1. Purpose

Define Application-owned semantic types before contract schemas and state machines. A coding worker SHALL NOT replace these types with untyped strings/numbers where the semantic distinction affects authority, units, validation, equality, determinism or evidence.

## 2. Ownership Rule

```text
FOUNDATION-OWNED IDENTITY / TIME / FIL / SECURITY / EVIDENCE SEMANTIC
=> consume the accepted Foundation contract/type when build consumption is available

APPLICATION BUSINESS SEMANTIC
=> define here or in the owning Application specification

APPLICATION WRAPPER AROUND FOUNDATION IDENTITY
=> zero-loss semantic wrapper only; no alternate authority/identity system
```

No Application type may clone a Foundation-owned semantic under a new name merely to avoid the Foundation boundary.

## 3. Canonical Scalar Rules

### 3.1 Decimal arithmetic

Financial and quantity arithmetic SHALL use deterministic base-10 decimal semantics. Binary floating-point SHALL NOT be used for authoritative price, quantity, money, exposure, fee, PnL, limit or capital calculations.

### 3.2 Rounding

Rounding SHALL be explicit and profile-driven. Default financial rounding is `MidpointRounding.ToEven` only where the market/broker/product profile does not require a different exact rule.

Rounding direction SHALL NOT be selected for profitability. Risk/capital collars use the conservative direction defined by the owning rule.

### 3.3 Nullability

A value is nullable only when absence is a meaningful domain state. `null` SHALL NOT represent `UNKNOWN`, `NOT_APPLICABLE`, `UNAVAILABLE`, `REJECTED` or `ZERO` interchangeably.

### 3.4 Unknown

Where uncertainty is material, use an explicit typed status/result rather than a magic value.

## 4. Identifier Types

All identifiers below are opaque semantic wrappers around a governed identifier value. Generation SHALL use the current Foundation identifier boundary when available. Equality is exact identifier equality; identifiers are never case-folded business names.

| Type | Owner | Meaning |
|---|---|---|
| `UserId` | Trading/business identity context | exact Falcon user/business principal reference |
| `TradingAccountId` | T-LSA-01 | canonical Trading account |
| `BrokerAccountId` | T-LSA-01/T-LSA-09 | broker-side account mapping identity |
| `MarketId` | T-LSA-02 | canonical market profile identity |
| `InstrumentId` | T-LSA-02 | canonical instrument identity |
| `InstrumentVersionId` | T-LSA-02 | immutable instrument-definition version |
| `UniverseId` | T-LSA-02 | qualified instrument universe snapshot family |
| `UniverseSnapshotId` | T-LSA-02 | immutable ranked/qualified universe snapshot |
| `ProviderId` | P-LSA-01 | provider organization/service identity |
| `ProviderAccountId` | P-LSA-03 | exact provider account/plan context |
| `ProviderRouteId` | P-LSA-04 | FSAPMA business route identity |
| `DataProductId` | P-LSA-02 | normalized business data-product family |
| `DataProductVersionId` | P-LSA-02 | immutable data-product schema/semantic version |
| `DataObservationId` | P-LSA-02/P-LSA-05 | one normalized observation identity |
| `BrokerId` | T-LSA-09 | broker/execution provider identity |
| `BrokerRouteId` | T-LSA-09 | broker adapter/business route identity |
| `StrategyId` | T-LSA-04/05/06 | central strategy family identity |
| `StrategyVersionId` | T-LSA-04/05/12 | immutable executable/config strategy version |
| `FeatureId` | T-LSA-03 | canonical analysis feature identity |
| `FeatureVersionId` | T-LSA-03 | immutable formula/model version |
| `ModelId` | owning intelligent component | model family identity |
| `ModelVersionId` | owning intelligent component | immutable trained/model artifact version |
| `TradeProposalId` | T-LSA-06 | immutable proposal identity |
| `RiskDecisionId` | T-LSA-07 | immutable risk evaluation identity |
| `CapitalReservationId` | T-LSA-08 | authoritative capital reservation aggregate identity |
| `ExecutionIntentId` | T-LSA-09 | approved execution intent identity |
| `OrderChainId` | T-LSA-09 | logical parent order lifecycle identity |
| `OrderAttemptId` | T-LSA-09 | one broker submission/replace/cancel attempt identity |
| `PositionId` | T-LSA-09 | authoritative Trading position aggregate identity |
| `PortfolioSnapshotId` | T-LSA-08 | immutable portfolio snapshot identity |
| `GuardianIncidentId` | G-LSA-01 | protection incident identity |
| `ProtectionDirectiveId` | G-LSA-02 | immutable Guardian directive identity |
| `CrisisEpisodeId` | G-LSA-03 | crisis lifecycle identity |
| `RecoveryAssessmentId` | G-LSA-04 | protection recovery assessment identity |
| `SimulationRunId` | S-LSA-01 | one simulation/replay/shadow run |
| `ScenarioId` | S-LSA-01 | scenario definition identity |
| `ScenarioVersionId` | S-LSA-01 | immutable scenario version |
| `SimulationEvidenceId` | S-LSA-08 | frozen validation evidence set identity |
| `FSARMPlanId` | FSARM | resource coordination plan identity |
| `ResourceDemandReportId` | Application local reporter | attributable Application demand/effect report |
| `AwarenessProposalId` | MSA/LSA/CSA origin | self-development proposal identity |
| `CandidateArtifactId` | awareness origin owner | isolated candidate identity |
| `ExperimentId` | T-LSA-12/FSTSimA | governed experiment identity |

## 5. Currency and Money

### `CurrencyCode`

- canonical ISO-4217 three-letter uppercase where the asset is a fiat currency;
- crypto settlement/base assets use a separate `AssetCode`, not fake ISO codes;
- validation rejects whitespace, mixed case, empty and unknown unsupported codes;
- equality is ordinal exact after canonical construction.

### `Money`

Fields:

```text
Amount: decimal
Currency: CurrencyCode
```

Rules:

- cross-currency addition/subtraction is forbidden without an explicit valuation/conversion operation;
- sign is allowed where the semantic permits PnL/fees/adjustments;
- capital reservation requested/held amounts must be non-negative;
- serialization SHALL preserve exact decimal text without exponent notation;
- currency scale is validated by the applicable accounting/market profile, not globally hardcoded to 2.

## 6. Asset, Price and Quantity Types

### `AssetCode`

Canonical uppercase asset symbol/code within an exact market/profile namespace. Display symbol is not identity; `InstrumentId` remains authoritative.

### `Price`

```text
Value: decimal > 0
QuoteAsset: AssetCode or CurrencyCode
```

Rules:

- scale/tick compliance validated against `InstrumentVersionId`;
- zero/negative authoritative trade prices are invalid;
- stale/unknown price is not represented as `0`.

### `Quantity`

```text
Value: decimal >= 0
Unit: QuantityUnit
```

`QuantityUnit` values initially:

```text
SHARES
BASE_ASSET_UNITS
CONTRACT_UNITS (future only when a market profile explicitly supports it)
```

Quantity step/minimum validation belongs to the instrument/broker profile.

### `SignedQuantity`

Used only for position delta/accounting. Value may be negative. Order quantity itself remains non-negative with explicit side.

## 7. Order / Position Enums

### `OrderSide`

```text
BUY
SELL
```

No `UNKNOWN` order side is executable. Unknown source evidence fails before intent creation.

### `PositionSide`

```text
FLAT
LONG
SHORT
```

Initial 1:1 funded deployment profile may prohibit `SHORT` by market/account policy even though the canonical type can represent it for future compatibility/replay.

### `OrderType`

Initial canonical set:

```text
MARKET
LIMIT
STOP
STOP_LIMIT
```

A market/broker profile determines which are enabled. Unsupported type => deterministic rejection.

### `TimeInForce`

Canonical set:

```text
DAY
GTC
IOC
FOK
```

Market/broker profile may restrict the set. Extended-hours eligibility is a separate explicit flag/profile rule, not inferred from TIF.

## 8. Percent / Ratio Types

### `BasisPoints`

Integer signed basis points for exact percentage deltas where 1 bp = 0.01%.

### `ConfidenceScore`

Integer range `0..10000` representing calibrated 0.00%..100.00% confidence.

Rules:

- confidence is evidence, never authority;
- 10000 does not mean certainty;
- unknown confidence uses `ConfidenceStatus=UNKNOWN`, not zero;
- comparison uses integer order.

### `QualityScore`

Integer range `0..10000` with an owning `QualityModelVersionId` and dimension context. Scores from different model/version/dimension are not directly comparable unless a normalization rule exists.

### `FitnessScore`

Integer `0..10000`, always bound to exact evaluation context/version and not automatically comparable across strategy families.

## 9. Time / Freshness Business Wrappers

Foundation time semantics SHALL be consumed rather than reimplemented.

Application business wrappers MAY include:

### `FreshnessRequirement`

```text
MaxAge
MaxFutureSkew
RequireMonotonicSourceSequence: bool
```

Durations are positive bounded values. The actual authoritative current time comes from the Foundation time provider/boundary.

### `DecisionDeadline`

Business deadline metadata only. It does not create transport QoS authority and SHALL map to future FCR-0009 transport deadline semantics when available.

## 10. Market Session Types

### `MarketSessionPhase`

US equities profile:

```text
CLOSED
PRE_MARKET
REGULAR
AFTER_HOURS
HALTED
HOLIDAY
UNKNOWN
```

Crypto spot profile:

```text
CONTINUOUS
VENUE_MAINTENANCE
VENUE_HALTED
UNKNOWN
```

`UNKNOWN` is non-executable unless the specific action explicitly allows session-independent safe handling such as cancel/reconcile.

## 11. Data Product Types

### `DataProductClass`

```text
REFERENCE
QUOTE
TRADE
BAR
ORDER_BOOK
CORPORATE_ACTION
MARKET_STATUS
NEWS_EVENT_NORMALIZED
DERIVED_FEATURE_INPUT
```

Raw unverified external text is not an executable Trading Data Product.

### `DataQualityState`

```text
VALID
DEGRADED
CONFLICTED
STALE
INCOMPLETE
UNAVAILABLE
UNKNOWN
```

Only `VALID` or explicitly allowed `DEGRADED` states may satisfy a consumer's declared requirement. `UNKNOWN` never silently maps to VALID.

### `ObservationAuthorityClass`

```text
OPERATIONAL_AUTHORITATIVE_INPUT
REPLAY
SIMULATION
SHADOW
RESEARCH
TEST
```

Only the explicitly permitted class can feed each production decision path.

## 12. Strategy / Decision Types

### `StrategyLifecycleState`

```text
EXPERIMENTAL
VALIDATION
WATCH
ACTIVE
RESTRICTED
DORMANT
RETIRED
```

### `StrategySignalDirection`

```text
LONG_BIAS
SHORT_BIAS
FLAT_EXIT
NO_TRADE
```

`SHORT_BIAS` may be rejected by initial market/account policy.

### `TradeProposalDisposition`

```text
PROPOSED
REJECTED_BY_APPLICABILITY
REJECTED_BY_UNCERTAINTY
REJECTED_BY_RISK
REJECTED_BY_CAPITAL
EXPIRED
SUPERSEDED
APPROVED_FOR_EXECUTION_INTENT
```

Approval here does not equal broker execution.

## 13. Risk Types

### `RiskSeverity`

```text
INFO
LOW
MEDIUM
HIGH
CRITICAL
UNKNOWN
```

`UNKNOWN` is conservatively handled according to the affected rule; it does not become LOW.

### `RiskDecision`

Core semantic fields:

```text
DecisionId
ProposalId
Decision: ALLOW | ALLOW_WITH_REDUCTION | DENY | REQUIRE_REVIEW | UNKNOWN
MaxApprovedQuantity
MaxApprovedNotional
TriggeredRuleIds[]
ReasonCodes[]
PolicyVersionId
EvidenceRefs[]
```

An ALLOW with a smaller max than requested is not equivalent to the original proposal.

## 14. Capital Types

### `CapitalReservationState`

```text
REQUESTED
HELD
PARTIALLY_CONSUMED
CONSUMED
RELEASING
RELEASED
EXPIRED
RECONCILIATION_REQUIRED
INVALID
```

### `CapitalPurpose`

```text
NEW_POSITION
INCREASE_POSITION
EXIT_COST_BUFFER
FEE_BUFFER
SETTLEMENT_BUFFER
PROTECTIVE_ACTION
```

A purpose is immutable for one reservation identity.

## 15. Execution Types

### `ExecutionIntentState`

```text
CREATED
VALIDATED
SUBMISSION_ELIGIBLE
SUBMITTING
SUBMITTED
PARTIALLY_FILLED
FILLED
CANCEL_REQUESTED
CANCELED
REPLACE_REQUESTED
REPLACED
REJECTED
EXPIRED
AMBIGUOUS
RECONCILIATION_REQUIRED
TERMINAL_FAILURE
```

### `BrokerEvidenceState`

```text
ACKNOWLEDGED
REJECTED
PARTIAL_FILL
FILL
CANCEL_ACK
REPLACE_ACK
UNKNOWN_ORDER
TRANSPORT_UNKNOWN
CONFLICTED
```

Broker evidence is input to reconciliation, not automatically authoritative Application truth.

## 16. Guardian Types

### `ProtectionSeverity`

```text
ADVISORY
ELEVATED
SEVERE
CRITICAL
EMERGENCY
```

### `ProtectionAction`

Current Application-owned action classes:

```text
RESTRICT_NEW_RISK
REDUCE_ALLOWED_EXPOSURE
SUSPEND_STRATEGY_SCOPE
SUSPEND_INSTRUMENT_SCOPE
SUSPEND_MARKET_SCOPE
CANCEL_OPEN_ORDERS
EXIT_POSITION_SCOPE
ISOLATE_PROVIDER_ROUTE
REQUEST_RESOURCE_PRIORITY
HOLD_PROMOTION
```

Each action requires an explicit authority mapping. Merely representing the enum does not grant the action.

## 17. FSARM Types

### `ResourceNeedClass`

```text
SURVIVAL_MINIMUM
PROTECTION_CRITICAL
OPEN_OBLIGATION_SAFETY
OPERATIONAL_REQUIRED
NORMAL
DEFERRABLE
EXPERIMENTAL
```

This is FSARM/Application coordination evidence and SHALL NOT be represented as Foundation technical criticality.

### `ReclaimabilityClass`

```text
NON_RECLAIMABLE_WHILE_OBLIGATION_ACTIVE
RECLAIMABLE_WITH_DEGRADATION
RECLAIMABLE_AFTER_CHECKPOINT
PAUSABLE
TERMINABLE_AND_RESTARTABLE
```

### `ResourcePlanDisposition`

```text
NO_CHANGE
INTERNAL_REBALANCE
SHED_DEFERRABLE
THROTTLE
PAUSE
REQUEST_ADDITIONAL_FOUNDATION_CAPACITY
PARTIAL_SATISFACTION
DENIED_BY_FOUNDATION_OUTCOME
RESTORE
FAIL_CLOSED
```

## 18. Awareness Types

### `AwarenessOriginTier`

```text
CSA
LSA
MSA
```

FSA is Foundation-owned and not created by this Application enum as an Application principal.

### `CandidateLifecycleState`

```text
IDEA
SCOPED
AUTHORIZED_FOR_ISOLATED_RESEARCH
CANDIDATE_BUILT
TESTING
EVIDENCE_COMPLETE
PARENT_REVIEW
MSA_REVIEW
PENDING_FSA_COMPATIBILITY_REVIEW
PENDING_OWNER_GOVERNANCE
REJECTED
SUPERSEDED
ACCEPTED_FOR_SEPARATE_IMPLEMENTATION_AUTHORIZATION
```

No state means deployed/production unless a separate governed production lifecycle says so.

## 19. Typed Result Pattern

Material operations SHALL return a typed result:

```text
Result<T>
  Status: SUCCESS | REJECTED | UNAVAILABLE | AMBIGUOUS | CONFLICTED | UNKNOWN
  Value: T? only when semantically valid
  ReasonCodes[]
  EvidenceRefs[]
  Causation/Correlation refs using Foundation semantics
```

`UNKNOWN` or `AMBIGUOUS` SHALL NOT be collapsed into SUCCESS.

## 20. Canonical Serialization Rules

Application contract payloads SHALL use a deterministic schema/version and canonical representation defined by `12_CROSS_APPLICATION_CONTRACT_SCHEMA_AND_ROUTE_CATALOG.md`.

Minimum rules:

- no locale-dependent decimal/date formatting;
- no implicit enum ordinal serialization; use canonical names or governed numeric codes;
- no unordered-map digest dependence without canonical key ordering;
- absent and explicit null are distinct where schema says so;
- unknown fields handled by declared compatibility policy;
- every material payload has explicit schema identity/version;
- decimals serialize as canonical plain base-10 text or an equivalently exact governed encoding;
- identity wrappers serialize their underlying governed identifier without lossy remapping.

## 21. Negative Fixtures

The type verifier SHALL reject at minimum:

- `Money(10 USD) + Money(10 SAR)` without conversion;
- negative order quantity;
- zero/negative executable price;
- unsupported precision/tick/step;
- confidence > 10000 or < 0;
- unknown enum silently defaulted to first value;
- strategy lifecycle unknown mapped to ACTIVE;
- simulation observation mapped to operational-authoritative input;
- request status mapped to grant;
- broker ACK mapped directly to FILLED;
- null used as a substitute for UNKNOWN;
- direct use of binary floating point for authoritative finance values;
- an Application-local clone of a Foundation identity/time/evidence semantic.
