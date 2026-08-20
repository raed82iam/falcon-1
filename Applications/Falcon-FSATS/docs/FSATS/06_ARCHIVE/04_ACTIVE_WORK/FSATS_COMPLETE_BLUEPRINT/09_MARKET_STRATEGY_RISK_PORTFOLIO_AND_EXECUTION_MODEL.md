# FSATS Complete Blueprint — Market, Strategy, Risk, Portfolio and Execution Model

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This document defines the end-to-end trading semantics that connect market state to an attributable, risk-governed, capital-reserved, broker-reconciled outcome.

The design explicitly avoids a single opaque `TradingEngine` that combines analysis, strategy, risk, capital and execution authority.

## 2. Market Model

Each market owns a Market Profile as Application business configuration/knowledge, not duplicated strategy code.

A Market Profile declares:

- market/asset-class identity;
- sessions/calendar;
- instrument/quantity/price semantics;
- minimum increments;
- supported order types/time-in-force;
- fractional/whole quantity behavior;
- broker support profile;
- required Data Products;
- liquidity/spread features;
- volatility/regime features;
- market-specific constraints;
- strategy applicability features;
- Risk constraints;
- validation obligations.

The same central strategy may therefore be eligible for several markets with different applicability/configuration envelopes.

## 3. Initial Market Profiles

### US Equities

Initial design supports cash/funded exposure with no borrowed leverage. Session capabilities are discovered/configured rather than assumed globally.

Profile may distinguish:

- regular session;
- extended-hours session;
- overnight/24x5-capable instruments where broker/provider capability exists;
- fractionable/non-fractionable instruments;
- halted/restricted state;
- consolidated versus limited-venue data coverage.

### Crypto Spot

Profile supports continuous 24/7 session modeling, spot quantity/precision rules and crypto-specific market-data/execution behavior.

Leverage/derivatives remain outside initial scope.

## 4. Strategy Catalog

There is one central Strategy Catalog inside the Trading Application.

Each strategy identity includes:

- immutable strategy ID/version;
- school;
- purpose;
- algorithm/model identity;
- supported markets/assets;
- required features/data products;
- permitted sessions;
- liquidity/volatility/regime applicability;
- capital constraints;
- Risk assumptions;
- execution assumptions;
- horizon;
- validation scope;
- current fitness evidence;
- known failure modes;
- lifecycle state.

A strategy does not get cloned per market merely to store different settings.

## 5. Strategy Lifecycle

```text
DISCOVERED / PROPOSED
-> EXPERIMENTAL
-> VALIDATED_FOR_SCOPE
-> PAPER_ELIGIBLE
-> SHADOW_ELIGIBLE
-> TINY_LIVE_ELIGIBLE (ONLY IF AUTHORIZED)
-> LIVE_ELIGIBLE (ONLY IF AUTHORIZED)
-> RESTRICTED
-> DEMOTED
-> RETIRED
```

Eligibility is not runtime authority. Environment activation requires separate current authority and capability.

## 6. Strategy Applicability Engine

Before a strategy may emit a trade proposal, applicability checks include:

- market profile match;
- instrument characteristics;
- current regime;
- session;
- data quality/freshness;
- feature availability;
- broker capability;
- liquidity/spread;
- capital capacity;
- current strategy validation scope;
- current fitness/drift state;
- Guardian restrictions;
- current Risk posture.

Failure means `NOT_APPLICABLE`, not a low-confidence trade.

## 7. Decision Evidence

A Trade Proposal includes:

- proposal ID;
- strategy ID/version;
- market/instrument;
- side/direction intent;
- horizon;
- expected entry/exit behavior;
- target/stop concept where applicable;
- expected distribution/edge estimate;
- evidence references;
- regime/session;
- data quality state;
- confidence/uncertainty;
- expiry;
- model/config identities;
- reason codes;
- correlation/causation lineage.

Proposal is non-authoritative until downstream hard gates pass.

## 8. Strategy Conflict Resolution

Conflict is resolved at portfolio/decision level, not by naive majority vote.

The controller considers:

- correlated strategy evidence;
- incompatible holding horizons;
- opposite direction signals;
- different target/stop structures;
- aggregate expected outcome;
- uncertainty;
- portfolio exposure;
- capital efficiency;
- Risk interaction;
- execution feasibility.

Possible outputs:

- select one proposal;
- combine compatible proposals under one bounded intent;
- reduce size;
- defer for more evidence;
- reject all as `NO_TRADE`.

## 9. Unified Risk Hard Gate

Every execution-bound intent passes a deterministic `PreTradeRiskDecision`.

Possible outcomes:

```text
ALLOW
ALLOW_WITH_REDUCTION
HOLD_FOR_RECONCILIATION
REJECT
```

The decision includes exact rule-set/version identity, inputs, reason codes and expiry.

### Minimum hard checks

1. identity and environment validity;
2. current Guardian restriction;
3. strategy/market/instrument eligibility;
4. data freshness/quality;
5. price sanity/collar;
6. maximum order size/notional;
7. funded-exposure limit;
8. current/projected position exposure;
9. market/instrument/concentration limits;
10. current loss/drawdown state;
11. capital availability/reservation;
12. duplicate/idempotency state;
13. active-order conflicts;
14. liquidity/spread constraints;
15. session/order-type/time-in-force compatibility;
16. broker/account capability;
17. stop/exit feasibility where required;
18. current authority/lifecycle state.

## 10. Pre-Trade Independent Control Principle

External industry evidence supports the engineering principle that orders should be blocked before market entry when capital/credit thresholds or obvious error conditions are violated.

Falcon adopts the stronger internal principle:

```text
NO ORDER MAY REACH BROKER SUBMISSION
UNTIL CURRENT HARD RISK + CAPITAL + AUTHORITY GATES PASS
```

This is an engineering design rule, not a legal determination about Falcon's future regulatory status.

## 11. Risk Envelope Hierarchy

Limits may be layered:

```text
OWNER / GOVERNED HARD CEILING
-> ACCOUNT / ENVIRONMENT ENVELOPE
-> MARKET ENVELOPE
-> PORTFOLIO ENVELOPE
-> STRATEGY ALLOCATION ENVELOPE
-> ORDER-SPECIFIC DECISION
```

A lower layer can be stricter but cannot exceed a higher ceiling.

AI may recommend dynamic tightening or sizing within an allowed range, but cannot raise a protected hard ceiling by confidence.

## 12. Capital Model

Capital state is portfolio-owned and exact.

Candidate categories:

- `CashAvailable`;
- `ReservedForIntent`;
- `CommittedToOpenOrder`;
- `PositionCost/Exposure`;
- `ReconciliationHold`;
- `Unavailable/Restricted`.

The model must distinguish cash, exposure, reservation and broker-reported buying-power-like values rather than using one ambiguous `balance` number.

## 13. Capital Reservation

Trade flow:

```text
RISK-ELIGIBLE INTENT
-> CALCULATE REQUIRED CAPITAL
-> CREATE RESERVATION
-> REVALIDATE EXPIRY / STATE
-> SUBMIT ORDER
-> UPDATE RESERVATION ON ACK/FILL/CANCEL/REJECT
-> RELEASE ONLY AFTER RECONCILED TERMINAL STATE
```

Timeout alone does not release capital if broker order state is ambiguous.

## 14. Position Sizing

Position size may consider:

- current portfolio capital;
- strategy edge/uncertainty;
- stop distance/risk unit;
- volatility;
- liquidity;
- spread/slippage estimate;
- concentration;
- correlation;
- existing exposure;
- broker minimums/fractional rules;
- current Risk posture;
- Guardian restrictions.

Final size is capped by deterministic risk and capital limits.

## 15. Order Intent Model

An `ExecutionIntent` binds:

- intent ID/idempotency key;
- source proposal/decision;
- risk decision ID/version;
- capital reservation ID;
- Guardian/control epoch;
- account/environment;
- broker role;
- market/instrument;
- side;
- quantity/notional;
- order type;
- limit/stop prices as applicable;
- time-in-force;
- session eligibility;
- expiry;
- causation/correlation.

No broker adapter accepts an unbound free-form order.

## 16. Canonical Broker-Independent Order State

Falcon maintains its own canonical order state independent of any single broker API.

Candidate state machine:

```text
INTENT_CREATED
-> RISK_APPROVED
-> CAPITAL_RESERVED
-> SUBMISSION_PENDING
-> SUBMITTED
-> ACKNOWLEDGED
-> PARTIALLY_FILLED
-> FILLED

SUBMITTED / ACKNOWLEDGED / PARTIALLY_FILLED
-> CANCEL_PENDING
-> CANCELED

ANY NONTERMINAL
-> REJECTED / EXPIRED / AMBIGUOUS / RECONCILING

TERMINAL + RECONCILED
-> CLOSED
```

Broker adapters map broker-specific states/events to this canonical model without hiding unmappable ambiguity.

## 17. State vs Event Separation

The design preserves the distinction between:

- **event purpose** — what just happened;
- **current order state** — what the order currently is.

This follows the useful industry pattern represented by FIX ExecutionReport concepts such as `ExecType` versus `OrdStatus`.

A fill event, status snapshot, rejection, cancel response and correction are not collapsed into one state field.

## 18. Execution Events

Candidate event families:

- submission attempted;
- broker accepted/acknowledged;
- broker rejected;
- partial fill;
- fill;
- cancel requested;
- cancel accepted/rejected;
- replace requested/accepted/rejected;
- expired;
- status snapshot;
- correction/bust where supported;
- disconnect/reconnect;
- ambiguous outcome;
- reconciliation result.

Every event is deduplicated/idempotent and attributable.

## 19. Ambiguous Order Handling

Network failure after submission is not treated as rejection or success.

```text
SUBMISSION OUTCOME UNKNOWN
-> MARK AMBIGUOUS
-> PRESERVE CAPITAL RESERVATION
-> BLOCK DUPLICATE CONFLICTING SUBMISSION
-> RECONCILE WITH BROKER TRUTH
-> RESOLVE CANONICAL STATE
```

This is a critical capital-protection invariant.

## 20. Reconciliation

Reconciliation compares internal expected state with broker/account observed truth.

Subjects include:

- open orders;
- fills;
- positions;
- cash/equity;
- fees;
- corporate-action effects;
- cancel/replace state;
- terminal status.

Divergence is explicit and may trigger `NO_NEW_RISK` or Guardian escalation according to severity.

## 21. Reconciliation Precedence

Internal memory does not override authoritative external outcome evidence.

However broker data is also validated for identity/environment/freshness and reconciled through the governed execution route.

No unverified webhook/message becomes position truth merely because it came from the broker domain.

## 22. Session and Order-Type Safety

Capabilities are profile-driven.

An order type allowed during regular equities hours may be disallowed or behave differently in extended/overnight sessions. Crypto may have different continuous-session constraints.

Trading checks the active broker/market capability profile at decision time.

## 23. Execution Quality Model

Track:

- decision-to-submit latency;
- submit-to-ack latency;
- submit-to-fill latency;
- slippage versus decision price;
- slippage versus arrival price;
- spread cost;
- fill ratio;
- partial-fill behavior;
- cancel effectiveness;
- rejected-order rate;
- ambiguous-order rate;
- broker uptime;
- session-specific performance.

This evidence feeds strategy applicability and FSTSimA calibration.

## 24. No Direct AI-to-Broker Path

```text
AI / STRATEGY OUTPUT
-> TYPED PROPOSAL
-> ORCHESTRATION
-> HARD RISK
-> CAPITAL RESERVATION
-> EXECUTION INTENT
-> BROKER ADAPTER
```

No model tool call may invoke broker order submission directly.

## 25. Initial Broker Profile

The architecture supports pluggable broker adapters. Alpaca Paper is a suitable initial broker profile for US equities and crypto experimentation because it exposes Paper trading APIs, but vendor capabilities remain runtime profile data.

Paper and Live identities/credentials/domains remain isolated.

## 26. Paper Reality Gap

Broker Paper results are adjusted conceptually by FSTSimA evidence for omitted effects such as:

- market impact;
- information leakage;
- latency slippage;
- queue position.

A strategy must survive realistic cost/execution bands before Tiny Live eligibility can be proposed.

## 27. Stop / Exit Semantics

Exit logic distinguishes:

- strategy-requested exit;
- Risk-enforced reduction;
- Guardian-managed exit restriction;
- reconciliation correction;
- Owner/user stop/close instruction;
- broker/exchange forced outcome.

The reason and authority chain remain reconstructable.

## 28. Decision Proof Envelope

Every material trade outcome should be reconstructable through references to:

- data products/provenance;
- analysis/model versions;
- strategy version;
- orchestration decision;
- Risk decision;
- capital reservation;
- Guardian state/control epoch;
- execution intent;
- broker request/response events;
- reconciliation evidence;
- final portfolio effect.

This proof graph explains the decision without turning evidence into authority.

## 29. Acceptance Gates

```text
ORDER_WITHOUT_RISK_DECISION = 0
ORDER_WITHOUT_CAPITAL_RESERVATION = 0
AI_DIRECT_BROKER_PATH = 0
AMBIGUOUS_ORDER_AUTO_RETRY_WITHOUT_RECONCILIATION = 0
CAPITAL_RELEASE_BEFORE_TERMINAL_RECONCILIATION = 0
BROKER_SPECIFIC_STATE_LEAK_INTO_DOMAIN = 0
EVENT_STATE_CONFLATION = 0
UNPROFILED_SESSION_ORDER_CAPABILITY = 0
RISK_LIMIT_SELF_EXPANSION = 0
```
