# P0-H - Self-Aware Trading Core 13-LSA Model

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-H defines the complete current architecture of the Falcon Self-Aware Trading Application. It directly integrates the accepted 13-LSA Trading topology, the current initial market/exposure scope, central strategy architecture, Unified Risk, portfolio/capital reservation, execution/reconciliation, learning/analytics/evolution and the later APP-RSC resource correction so no programmer must reconstruct Trading truth from a 12-LSA map, TARC-era documents, NEW material and later Part 1 records.

## 2. Canonical Application boundary

Trading is one independent Falcon Application with exactly one MSA, 13 LSAs and 3 current eligible CSAs.

Trading owns Trading business logic only. It does not own Foundation lifecycle/admission, Foundation total-resource truth, FSAPMA provider routing, Guardian protection authority, FSTSimA validation authority, APP-RSC FSATS-wide resource coordination, Shared Web customer identity, external credential infrastructure or Owner governance.

## 3. Current initial Trading scope

```text
INITIAL_MARKETS = US_EQUITIES + CRYPTO_SPOT
INITIAL_EXPOSURE_MODEL = 1_TO_1_FUNDED_EXPOSURE
```

The initial scope does not imply authority for margin leverage above funded capital, options, futures, derivatives, leveraged tokens, borrowing-based short exposure, cross-customer pooled capital or any market/asset class not separately admitted.

```text
INITIAL_1_TO_1_FUNDED_SCOPE != PERMISSION_FOR_LEVERAGE
US_EQUITIES_AND_CRYPTO_SPOT != ALL_MARKETS
```

A new market, asset class, leverage model, derivative capability or materially different capital model requires separate Market Profile, Risk, execution, validation, Foundation-dependency and Owner/governance review. This scope is design semantics only and grants no Paper, Shadow, Tiny-Live, Live, broker connectivity, capital deployment or execution authority.

## 4. Trading runtime identity

Trading operates on exact broker-account scope, not an FSATS customer/user principal.

```text
FSATS_USER_ID = NONE
FSATS_USERNAME = NONE
FSATS_CUSTOMER_ID = NONE
TRADING_OPERATING_SUBJECT = BROKER_ACCOUNT
BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = additional identity dimension where material
```

Shared Web may map a customer/user/contact identity to one or more exact broker-account scopes. Trading receives only the governed scope required for the business request.

## 5. Exact 13-LSA topology

```text
Trading MSA
|
+-- T-LSA-01 Operations, Account & Environment
+-- T-LSA-02 Market & Instrument Universe
+-- T-LSA-03 Analysis Frameworks
+-- T-LSA-04 Classical Trading School
+-- T-LSA-05 Opportunity Hunting School
+-- T-LSA-06 Strategy Orchestration & Decision
+-- T-LSA-07 Unified Risk Management
+-- T-LSA-08 Portfolio & Capital Management
+-- T-LSA-09 Execution & Position Lifecycle
+-- T-LSA-10 Trading Learning & Knowledge
+-- T-LSA-11 Trading Analytics & Attribution
+-- T-LSA-12 Strategy Evolution & Experimentation
+-- T-LSA-13 Trading Resource Awareness & Evaluation
```

The historical 12-LSA map and the historical TARC-as-FSATS-resource-owner interpretation are superseded for current design.

## 6. T-LSA-01 Operations, Account & Environment

Owns Trading-side awareness/evaluation of broker-account and environment context, business operating mode, account readiness, session/business availability, broker/account bindings, current business authorization and resource-readiness projections consumed from the current resource path.

It does not own Foundation lifecycle, APP-RSC, broker execution, Unified Risk or Guardian crisis authority.

```text
T_LSA01_RESOURCE_READINESS_CONSUMER != RESOURCE_OWNER
```

## 7. T-LSA-02 Market & Instrument Universe

Owns Market Profiles, market/instrument eligibility, dynamic Candidate Universe, managed-instrument awareness and market-specific constraints.

```text
CANDIDATE_UNIVERSE != MANAGED_POSITION_SET
```

An instrument leaving the Candidate Universe does not erase an existing position or obligation.

Each admitted Market Profile defines as applicable market/asset identity, sessions/calendar, instrument eligibility, liquidity/volatility characteristics, tick/quantity/order constraints, required Data Products, analysis requirements, strategy-applicability inputs, execution constraints, settlement/account constraints, market-specific Risk inputs, broker capability dependencies, prohibited/unsupported conditions and validation/Intended Use requirements.

Initial Market Profiles are US Equities and Crypto Spot.

A Market Profile supplies facts/constraints. It does not duplicate strategies and does not own Unified Risk.

The Candidate Universe may change with eligibility, liquidity, broker availability, market conditions, data quality, funded capital and governed ranking criteria. Universe change cannot erase exposure, override Guardian/Risk, create broker capability, create capital authority or admit a new market.

## 8. T-LSA-03 Analysis Frameworks

Owns reusable attributable Trading analysis frameworks, including accepted technical, statistical, quantitative, structural, regime, flow, anomaly, pattern or behavioral methods.

Analysis exposes method/version, required inputs, applicable context, evidence/confidence, assumptions, uncertainty, failure modes, freshness and provenance.

```text
ANALYSIS_SIGNAL != TRADING_DECISION
ANALYSIS_CONFIDENCE != RISK_AUTHORITY
```

Eligible intelligent analysis components may own CSA only through P0-C governance.

## 9. T-LSA-04 Classical Trading School

Owns awareness/evaluation of Classical Trading School methods, applicability, evidence, limitations, interaction effects and improvement opportunities. It produces evidence/recommendations but does not own final decision, capital, Unified Risk, execution, Guardian or market admission.

## 10. T-LSA-05 Opportunity Hunting School

Owns specialized opportunity-discovery methods, including bounded whale/activity hunting, anomaly/opportunity discovery and separately accepted search methods.

```text
OPPORTUNITY_FOUND != TRADE_AUTHORIZED
```

It may rank/score only within admitted market/instrument/data contexts and does not own final decision, Risk, capital or execution.

## 11. T-LSA-06 Strategy Orchestration & Decision

### 11.1 Central Strategy Catalog

Strategies are centrally registered, not duplicated per market. Each strategy declares identity/version, school/method family, supported Market Profile classes, instrument applicability, predicates, required Data Products/features, horizon/session, limitations, Risk assumptions, execution requirements, validation/Intended Use evidence, experiment lineage and lifecycle/status.

Market-specific facts belong to Market Profiles or the correct owner, not copied into strategy definitions.

### 11.2 Strategy Controller

The Strategy Controller evaluates current Market Profiles, strategy applicability, analysis evidence and Trading state to select eligible strategies. It may compare/combine recommendations under governed rules but cannot override Unified Risk, capital reservation, Guardian restrictions, Owner controls, broker capability, execution safety, market admission or validation scope.

### 11.3 Decision completeness

Trading decisions explicitly support action and non-action states such as:

```text
TRADE_CANDIDATE
NO_TRADE
DEFER
INSUFFICIENT_EVIDENCE
REJECTED_BY_RISK
RESTRICTED_BY_CONTROL
```

Falcon never forces a trade merely because a strategy emits a signal.

```text
CATALOG_PRESENT != APPLICABLE
APPLICABLE != ACTIVATED
ACTIVATED != TRADE_AUTHORIZED
SYNTHESIS != CONSENSUS
DISAGREEMENT != ERROR
```

## 12. T-LSA-07 Unified Risk Management

Unified Risk owns Trading Risk business semantics and decisions across all admitted markets. It considers per-trade, instrument, strategy, market, correlated/common-factor and aggregate portfolio exposure, concentration, drawdown, daily/session loss, liquidity, volatility, execution/slippage risk, current obligations/open exposure, intended versus worst credible loss, capital-preservation floors, broker-account/environment context and current Guardian restrictions.

Risk may adapt only inside separately authorized Owner/governance envelopes.

```text
DYNAMIC_RISK != UNBOUNDED_RISK
RISK_APPROVAL != CAPITAL_RESERVATION
RISK_APPROVAL != BROKER_ACCEPTANCE
```

Risk may always reduce toward zero/no-trade when protection requires it. The funded 1:1 ceiling cannot be enlarged by Risk confidence.

If Risk changes size or another material property, the result is a new exact Risk decision/version and downstream gates must bind the new state. Earlier confidence or pre-dispatch proof cannot authorize the resized intent.

Risk does not own Guardian crisis classification, Foundation resources, broker outcome truth, FSAPMA data truth, strategy catalog ownership or Owner authority.

## 13. T-LSA-08 Portfolio & Capital Management

Owns portfolio/capital business semantics, portfolio composition, allocation/reservation, concentration coordination, capital reservations, release/reconciliation, multi-market capital coordination and enforcement of the initial funded 1:1 model.

### 13.1 Global Capital Reservation Ledger

Trading maintains a governed reservation mechanism preventing double allocation across simultaneous decisions and markets.

Each material reservation binds as applicable:

- reservation identity;
- `BrokerId + BrokerAccountId` and Environment where material;
- portfolio scope;
- market/instrument;
- Trading Intent;
- amount/notional/currency;
- Risk decision/version;
- effective/expiry state;
- release/commit/reconciliation state;
- causation/correlation.

```text
TRADING_CAPITAL_RESERVATION != BROKER_BUYING_POWER_TRUTH
TRADING_CAPITAL_RESERVATION != FOUNDATION_RESOURCE_GRANT
```

Broker funds/buying-power truth is separately reconciled through authorized broker/execution paths.

### 13.2 Funded-exposure invariant

```text
AUTHORIZED_TRADING_EXPOSURE <= GOVERNED_AVAILABLE_FUNDED_CAPITAL
```

subject to stricter Risk, Guardian, Owner, broker, market and validation limits. The funded-capital rule is a ceiling, not a target.

## 14. T-LSA-09 Execution & Position Lifecycle

Owns Trading execution business semantics, broker order lifecycle, execution outcome interpretation, position lifecycle and reconciliation.

### 14.1 Exact pre-submission binding

Before broker submission, bind exact broker-account identity, environment, market/instrument, broker/service role, order intent, capability requirements, Risk decision/version, capital reservation, Guardian/Owner control epochs and validation/Intended Use scope.

### 14.2 Broker Capability Profile

Capability state distinguishes:

```text
SUPPORTED
UNSUPPORTED
CONDITIONALLY_SUPPORTED
UNKNOWN_UNVERIFIED
```

`UNKNOWN != SUPPORTED`. Unsupported/unknown capabilities are not silently emulated if emulation changes Risk/protection/business semantics.

### 14.3 Outcome truth

```text
ORDER_REQUESTED != SUBMISSION_ATTEMPT
SUBMISSION_ATTEMPT != ORDER_ACCEPTED
ORDER_ACCEPTED != PARTIALLY_FILLED
PARTIALLY_FILLED != FILLED
CANCEL_REQUESTED != CANCELED
REPLACEMENT_REQUESTED != REPLACED
CLOSE_REQUESTED != ZERO_EXPOSURE
UNKNOWN_BROKER_OUTCOME != REJECTED
```

Submission timeout, uncertain state, late ACK, partial fill, cancel/replace race or conflicting broker evidence enters reconciliation before unsafe retry.

### 14.4 Execution Runtime Cell

A future Execution Runtime Cell may be an implementation fault-domain pattern. It is not an Application, LSA, authority source, broker principal or cross-Application identity.

## 15. T-LSA-10 Trading Learning & Knowledge

Owns durable attributable Trading knowledge: process/outcome learning, market/strategy behavior, failure lessons, provenance, drift, competence/uncertainty, invalidated beliefs and links to experiment/decision evidence.

Learning does not promote models or create authority. It feeds the governed evolution path through the actual origin owner and P0-C.

## 16. T-LSA-11 Trading Analytics & Attribution

Owns analytics, attribution and effectiveness measurement. It distinguishes decision quality, outcome quality, strategy/Risk/execution contribution, market/regime context, capital usage, valid counterfactual/opportunity-cost evidence and statistical uncertainty.

Profit alone does not prove decision quality.

### 16.1 Counterfactual Decision Ledger

Counterfactual evidence may record what might have occurred under an alternate/declined decision, but:

```text
COUNTERFACTUAL_OUTCOME != REALIZED_OUTCOME
```

It never becomes executed-order truth, realized P&L or authority.

## 17. T-LSA-12 Strategy Evolution & Experimentation

Coordinates strategy/model candidate experiments, sandbox/replay/Paper evidence when separately authorized, preregistration, baseline comparison, lineage, FSTSimA interaction and evidence handoff to Trading MSA.

It cannot self-promote, bypass MSA/FSA/Owner governance, turn Paper success into Live authority, expand market/Risk/leverage scope without renewed validation or treat experimentation authority as broker execution authority.

## 18. T-LSA-13 Trading Resource Awareness & Evaluation

T-LSA-13 owns Trading-local resource awareness/evaluation only. It understands Trading workload demand, pressure, efficiency, degradation consequence, internal shedding options, restoration evidence and resource need.

It produces attributable Trading resource evidence to the current APP-RSC contract path.

```text
T_LSA_13 != APP_RSC
T_LSA_13 != RESOURCE_STRATEGY_CONTROLLER
T_LSA_13 != FOUNDATION_RESOURCE_GOVERNANCE
```

The historical TARC role that acted as the sole Trading-to-Foundation resource requester is superseded by the accepted APP-RSC fifth-Application model. Trading no longer owns an independent direct Foundation resource-request principal. Trading publishes its exact resource evidence to APP-RSC; APP-RSC performs bounded FSATS coordination and only APP-RSC may assemble a proven residual request to Foundation under P0-J/current P1K resource contracts.

## 19. Resource priority separation

```text
BUSINESS_LANE
!= TRADING_INTERNAL_WORKLOAD_IMPORTANCE
!= APP_RSC_COORDINATION_PRIORITY_EVIDENCE
!= FOUNDATION_APPLICATION_PRIORITY
!= FOUNDATION_TECHNICAL_CRITICALITY
```

Trading urgency cannot mint Foundation technical criticality or seize protected Foundation floors.

## 20. Canonical Trade Admission Chain

```text
CONTEXT / AUTHORITY
-> BROKER_ACCOUNT / ENVIRONMENT READINESS
-> MARKET / INSTRUMENT ELIGIBILITY
-> FSAPMA DATA PRODUCT ELIGIBILITY / FRESHNESS
-> ANALYSIS
-> STRATEGY APPLICABILITY
-> DECISION COMPLETENESS
-> UNIFIED RISK DECISION
-> CAPITAL RESERVATION
-> OWNER / GOVERNED BUSINESS CONTROLS
-> GUARDIAN RESTRICTIONS
-> EXACT BROKER / ACCOUNT / ENVIRONMENT / CAPABILITY ELIGIBILITY
-> TRADING INTENT FINALIZATION
-> EXECUTION PLAN / ORDER PREPARATION
-> PRE-DISPATCH SAFETY VALIDATION
-> DISPATCH-TIME REVALIDATION OF MATERIAL MUTABLE GATES
-> BROKER SUBMISSION ATTEMPT
-> BROKER OUTCOME TRUTH
-> T-LSA-09 EXECUTION / POSITION RECONCILIATION
```

No stage assumes a later gate will pass.

## 21. Pre-dispatch safety

After exact Trading Intent and Execution Plan exist, validate order parameters, current Risk decision, capital reservation, Guardian/Owner epochs, broker/account/environment capability, size/price/notional sanity, funded 1:1 ceiling, duplicate/idempotency identity, market/session eligibility, critical Data Product freshness and maximum authorized consequence.

This is not a strategy-selection stage.

## 22. Dispatch-time revalidation and control epochs

Material mutable facts must be revalidated immediately before dispatch as consequence requires. Changes to Guardian restriction, Owner stop/restriction, Risk validity, capital reservation/funded-capital validity, broker capability, market/session eligibility, critical Data Product validity or environment authorization suppress stale opening work.

```text
STALE_CONTROL_EPOCH -> NO_NEW_DISPATCH
```

Existing positions continue through valid protection/reconciliation paths.

## 23. No implicit broker fallback

A failed broker route/account does not authorize another broker/account. Different brokers may have different ownership, buying power, positions, order capabilities, fees/slippage, market access and protection semantics.

```text
BROKER_A_FAILURE != AUTHORITY_TO_USE_BROKER_B
```

Any future broker fallback requires separate policy, Risk review, authorization and validation.

## 24. Portfolio/performance truth exposed to Shared Web

Trading owns bounded customer-facing portfolio/position/order-trade/performance projections derived from exact broker-account scope. Shared Web renders them and may map customer identity to account scope, but Web does not calculate or upgrade Trading truth.

Current semantic identities include:

```text
FSATS.WebPortfolioViewRequest.v1
FSATS.WebPortfolioSummaryProjection.v1
FSATS.WebPositionCollectionProjection.v1
FSATS.WebOrderTradeActivityProjection.v1
FSATS.WebPortfolioPerformanceProjection.v1
FSATS.WebPortfolioProjectionUpdate.v1
```

Implementation-ready public payload/binding metadata remains tracked by live FCR-0133.

## 25. Cross-boundary dependencies

- FSAPMA provides operational Data Products through P0-F contracts.
- Guardian provides protection/restriction outcomes through governed contracts.
- APP-RSC receives Trading resource evidence and coordinates resources under P0-J.
- Foundation provides generic lifecycle/resource/communication/security boundaries within exact implemented/authorized scopes.
- broker execution depends on FCR-0014 Stage 12 external execution egress/credential-reference capability.
- APP-RSC final canonical Foundation resource binding remains subject to FCR-0016/FCR-0031.
- P0-K governs validation/credibility/promotion.
- P0-C governs self-development.

## 26. Failure/degraded behavior

At minimum preserve these outcomes:

- high strategy confidence + Risk reject -> no trade;
- exposure above funded 1:1 -> no new exposure;
- capital reservation conflicts with broker funds -> hold/reconcile;
- control epoch changes before dispatch -> suppress stale work;
- broker capability unknown -> unsupported for affected action;
- submission timeout -> reconcile before unsafe retry;
- partial fill/cancel race -> preserve exact executed/open quantity;
- Candidate Universe removes held instrument -> position remains managed;
- APP-RSC unavailable -> Trading does not become Foundation requester;
- Data Product degraded -> new exposure is reduced/denied according to policy while existing exposure receives protection;
- market outside US Equities/Crypto Spot -> reject until separately admitted.

## 27. Explicit non-authority

Trading SHALL NOT acquire operational external data outside FSAPMA, own Guardian crisis scope, own Foundation total resources, bypass APP-RSC for FSATS resource coordination, infer broker egress from provider egress, let analysis confidence override Risk, treat broker acknowledgement as fill, treat counterfactual evidence as realized truth or introduce leverage/derivatives/new markets by implication.

## 28. Invariants

```text
TRADING_MSA_COUNT = 1
TRADING_LSA_COUNT = 13
TRADING_CSA_COUNT = 3
INITIAL_MARKETS = US_EQUITIES + CRYPTO_SPOT
INITIAL_EXPOSURE_MODEL = 1_TO_1_FUNDED
INITIAL_1_TO_1_FUNDED_SCOPE != PERMISSION_FOR_LEVERAGE
OUT_OF_SCOPE_MARKET != AUTOMATIC_MARKET_ADMISSION
T_LSA_13 != APP_RSC
ANALYSIS_SIGNAL != TRADING_AUTHORITY
OPPORTUNITY_FOUND != TRADE_AUTHORIZED
STRATEGY_CONFIDENCE != RISK_OVERRIDE
TRADING_CAPITAL_RESERVATION != BROKER_BUYING_POWER_TRUTH
UNKNOWN_BROKER_CAPABILITY != SUPPORTED
ORDER_REQUESTED != ORDER_ACCEPTED != FILLED
AMBIGUOUS_EXECUTION -> RECONCILIATION_BEFORE_UNSAFE_RETRY
BROKER_A_FAILURE != BROKER_B_AUTHORITY
COUNTERFACTUAL_OUTCOME != REALIZED_OUTCOME
TRADING_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
```

## 29. Mandatory scenarios

Challenge at minimum US Equities and Crypto Spot nominal paths; leveraged crypto masquerading as spot; options/futures under equities scope; funded-capital exceedance; concurrent equities/crypto reservations; Risk resize; stale broker buying power; stop epoch before dispatch; duplicate intent; timeout then late ACK; partial-fill/cancel; cancel/replace; unknown capability; unsupported protection; universe removal with open exposure; isolated broker-account failure; unauthorized broker fallback; high confidence/Risk reject; NO_TRADE later profitable; correlation shock; resource-priority inflation; APP-RSC unavailable/stale resource state; and third-market admission attempt.

## 30. Exit gates

```text
TRADING_13_LSA_OWNERSHIP = PASS
INITIAL_MARKET_SCOPE = EXPLICIT
INITIAL_1_TO_1_FUNDED_SCOPE = EXPLICIT
IMPLICIT_LEVERAGE_OR_DERIVATIVE_AUTHORITY = 0
IMPLICIT_NEW_MARKET_AUTHORITY = 0
TRADE_ADMISSION_CHAIN = COMPLETE
RISK_AUTHORITY_COLLISIONS = 0
CAPITAL_DOUBLE_ALLOCATION_PATHS = 0
STALE_CONTROL_EXECUTION_PATHS = 0
BROKER_CAPABILITY_UNKNOWN_AS_SUPPORTED = 0
EXECUTION_AMBIGUITY_BLIND_RETRY = 0
COUNTERFACTUAL_REALIZED_TRUTH_COLLAPSE = 0
TRADING_DIRECT_FOUNDATION_RESOURCE_REQUEST_PATHS = 0
FCR0014_RUNTIME_STATE = EXPLICIT
APP_RSC_RESOURCE_BOUNDARY = EXPLICIT
```

## 31. Non-grant

Acceptance of P0-H would establish Trading architecture semantics only. It would not authorize broker connectivity, credentials, order submission, Paper, Shadow, Tiny-Live, Live, deployment, leverage, derivatives, additional markets or direct Foundation resource requests.