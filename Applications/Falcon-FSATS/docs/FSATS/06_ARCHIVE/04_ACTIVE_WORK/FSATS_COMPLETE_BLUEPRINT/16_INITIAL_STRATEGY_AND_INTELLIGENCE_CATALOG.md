# FSATS Complete Blueprint — Initial Strategy and Intelligence Catalog

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This document defines the initial strategy/intelligence families so future coding does not invent trading scope ad hoc.

The catalog is deliberately broad enough to test Falcon's architecture but small enough to validate seriously.

No strategy below is authorized for Paper or Live merely because it is listed.

## 2. Strategy Architecture

One strategy identity is centrally registered and receives market applicability through Market Profiles and validation evidence.

Each strategy emits a typed `TradeProposal`; it never submits orders directly.

Each strategy declares:

- identity/version;
- school;
- required data/features;
- market applicability;
- session/horizon;
- liquidity/volatility assumptions;
- entry/exit hypothesis;
- risk assumptions;
- execution assumptions;
- invalidation/stop conditions;
- validation state;
- known failure modes.

## 3. Classical Trading School — Initial Families

### CLS-001 — Multi-Horizon Trend Continuation

Purpose: participate in sustained directional movement only when trend structure is consistent across required horizons.

Inputs may include:

- normalized returns;
- moving/trend structure;
- volatility state;
- volume/liquidity;
- regime classification.

Primary failure risks:

- late entry;
- whipsaw/range regime;
- gap reversal;
- thin liquidity.

### CLS-002 — Momentum Breakout

Purpose: trade a confirmed break from a bounded price structure with liquidity/volume confirmation.

Controls:

- false-breakout evidence;
- spread/liquidity gate;
- volatility-adjusted distance;
- session-aware behavior;
- no chase beyond risk/price collar.

### CLS-003 — Pullback Continuation

Purpose: enter a prevailing trend after a controlled pullback rather than at maximum extension.

Requires:

- trend/regime qualification;
- pullback depth/quality;
- liquidity recovery;
- invalidation level.

### CLS-004 — Mean Reversion

Purpose: exploit statistically validated temporary deviation from a local equilibrium in regimes where reversion behavior is credible.

Must fail closed in uncontrolled trend/breakout regimes.

### CLS-005 — Volatility Compression / Expansion

Purpose: identify sustained compression followed by validated expansion.

Separates:

- pre-break compression detection;
- direction evidence;
- liquidity/volume confirmation;
- execution feasibility.

### CLS-006 — Relative Strength / Weakness Rotation

Purpose: compare qualified instruments within a compatible universe and prefer stronger/weakening opportunities where portfolio/risk constraints permit.

Must control correlation and common-factor exposure.

## 4. Opportunity Hunting School — Initial Families

### HNT-001 — Unusual Volume / Participation Surge

Detects statistically abnormal participation relative to instrument/session baseline.

Volume alone does not imply direction or trade eligibility.

### HNT-002 — Momentum Ignition

Detects rapid acceleration in price/volume/liquidity state and requests expensive confirmation only for top candidates.

Designed for short-lived opportunity discovery; strict expiry is mandatory.

### HNT-003 — Gap / Session Transition Hunter

Targets qualified opening, extended-session or session-transition dislocations.

Requires explicit session capability and stronger spread/liquidity controls.

### HNT-004 — Large-Flow / Whale Signature Hunter

Attempts to identify evidence consistent with unusually large market participation using only lawfully available observable data.

No claim of knowing hidden participant identity is permitted.

Potential features:

- abnormal trade-size distribution;
- repeated directional blocks;
- volume concentration;
- order-book pressure where data permits;
- price response/absorption behavior.

### HNT-005 — Liquidity Vacuum / Refill Hunter

Detects rapid deterioration or restoration in tradable liquidity and searches for bounded opportunities while treating execution risk as first-class.

### HNT-006 — Cross-Instrument / Cross-Market Dislocation Hunter

Finds statistically material divergence between related instruments/markets where the relationship has current validation.

It is not automatic arbitrage and must account for asynchronous market/session/liquidity behavior.

### HNT-007 — Crypto Continuous-Regime Transition Hunter

Focuses on 24/7 crypto-specific transitions such as volatility/volume regime shifts that do not align to an exchange opening bell.

### HNT-008 — Catalyst/Event Reaction Hunter

Uses governed event/news Data Products when available to identify measurable market reaction, not to trade raw unverified text directly.

Research/news content must not bypass FSAPMA operational Data Product rules.

## 5. Initial Analysis Intelligence Components

Candidate intelligent components include:

### INT-001 — Regime Classifier

Outputs probability/calibration across market regimes such as:

- trending;
- ranging;
- high-volatility;
- low-volatility;
- liquidity stress;
- transition/uncertain.

May be CSA-eligible.

### INT-002 — Liquidity and Execution Quality Estimator

Estimates current execution difficulty, spread/slippage risk and capacity.

Feeds strategy applicability and sizing evidence, not final broker authority.

May be CSA-eligible.

### INT-003 — Opportunity Ranker

Ranks discovery candidates using multi-dimensional expected opportunity quality and resource cost.

It decides which candidates deserve richer analysis, not which orders to submit.

### INT-004 — Strategy Applicability Model

Estimates how well a validated strategy's applicability envelope matches current conditions.

Hard invalid conditions remain deterministic exclusions.

### INT-005 — Decision Calibration / Uncertainty Model

Evaluates whether strategy/ensemble confidence historically matches observed outcomes by regime.

### INT-006 — Execution Cost / Slippage Model

Predicts expected execution cost bands and supplies evidence to sizing/Risk/strategy evaluation.

### INT-007 — Provider Reliability Forecast Model

Owned by FSAPMA; forecasts degradation/quota/reliability risk for route planning.

### INT-008 — Data Quality Anomaly Model

Owned by FSAPMA; detects patterns not captured by simple deterministic validation.

### INT-009 — Guardian Incident Correlation Model

Owned by Guardian; correlates weak signals into incident candidates without independently executing protection commands.

### INT-010 — FSTSimA Synthetic Scenario Generator

Generates adversarial/regime scenarios for validation and may be CSA-eligible.

### INT-011 — FSTSimA Fidelity Calibration Model

Improves simulator fit while remaining separate from independent S-LSA-08 assessment.

## 6. Strategy Controller

`StrategyController` performs:

```text
LOAD CURRENT MARKET PROFILE
-> LOAD ACTIVE VALIDATED STRATEGY VERSIONS
-> APPLY HARD APPLICABILITY FILTERS
-> SCORE CURRENT FITNESS
-> CONSIDER PORTFOLIO/RISK/EXECUTION CONTEXT
-> REQUEST STRATEGY EVALUATIONS
-> NORMALIZE OUTPUTS
-> RESOLVE CONFLICT/CORRELATION
-> EMIT TRADE PROPOSAL OR NO_TRADE
```

It does not own Unified Risk or capital reservation.

## 7. School Weighting

School weights are contextual, not permanent.

Inputs may include:

- current regime;
- recent calibrated performance;
- opportunity density;
- correlation between active strategies;
- data quality;
- execution quality;
- drawdown state;
- uncertainty.

The weighting engine cannot override hard strategy applicability or Risk.

## 8. Strategy Self-Awareness

Strategy-level specialized self-evaluation may be implemented as CSA only where the strategy/component is genuinely intelligent and eligible.

A strategy CSA may understand:

- current validated scope;
- performance by regime;
- calibration;
- recurring failure patterns;
- feature usefulness;
- execution sensitivity;
- candidate improvements.

It cannot expand its market scope, Risk limits or production state itself.

## 9. Adaptive Meta-Learning

T-LSA-12 may maintain an experimental Meta-Learner that proposes:

- strategy parameter candidates;
- feature candidates;
- school-weight candidates;
- strategy combinations;
- retirement candidates;
- applicability-boundary changes.

The Meta-Learner competes against the current baseline in FSTSimA and Paper/Shadow evidence. It is not a privileged master strategy.

## 10. Strategy Capital Efficiency Score

A candidate score for portfolio allocation may combine normalized evidence such as:

```text
EXPECTED NET EDGE
x CALIBRATION QUALITY
x REGIME FITNESS
x EXECUTION FEASIBILITY
x DIVERSIFICATION BENEFIT
-----------------------------------
DOWNSIDE RISK x CAPITAL CONSUMPTION x UNCERTAINTY PENALTY
```

This is a ranking signal, not capital authority. Exact formula/weights require validation and may vary by market/profile.

## 11. Time Horizons

Strategies declare their own horizon instead of forcing one global timeframe.

Possible analysis windows include:

- intraday micro/short;
- intraday medium;
- multi-session/swing;
- longer-term context used only as a filter.

The initial system may implement a subset first. A timeframe exists only when required data quality and execution assumptions are validated.

## 12. Feature Governance

Every feature has:

- identity/version;
- formula/model;
- units;
- required data product;
- lookback;
- warmup;
- missing-data behavior;
- freshness requirement;
- supported markets;
- leakage/look-ahead tests;
- performance cost.

A model cannot silently recompute a feature differently in simulation and production.

## 13. Strategy Failure Modes

Every strategy must explicitly test for:

- regime mismatch;
- low liquidity;
- spread expansion;
- stale/conflicted data;
- false breakout/signal noise;
- gap risk;
- correlation shock;
- execution cost change;
- provider/broker degradation;
- prolonged drawdown;
- parameter drift;
- overfitting;
- rare event/tail behavior.

## 14. Strategy Retirement / Dormancy

A strategy can remain cataloged but inactive.

States may include:

```text
ACTIVE
WATCH
RESTRICTED
DORMANT
EXPERIMENTAL
RETIRED
```

Dormancy preserves knowledge without wasting capital or runtime resources.

## 15. Initial Implementation Order

When implementation is later authorized, the first strategy set should favor diversity of failure modes rather than maximum quantity.

Recommended first implementation candidates for Paper validation:

1. CLS-001 Trend Continuation.
2. CLS-002 Momentum Breakout.
3. CLS-004 Mean Reversion.
4. CLS-005 Volatility Compression/Expansion.
5. HNT-001 Unusual Volume.
6. HNT-003 Gap/Session Transition.
7. HNT-004 Large-Flow Signature.
8. HNT-007 Crypto Regime Transition.

Remaining strategies can follow as separately testable additions.

This is a code-sequencing recommendation, not implementation authority.

## 16. Acceptance Gates

```text
STRATEGY_DUPLICATED_PER_MARKET_WITHOUT_SEMANTIC_NEED = 0
STRATEGY_DIRECT_ORDER_SUBMISSION = 0
CONFIDENCE_AS_RISK_AUTHORITY = 0
UNVERSIONED_FEATURE = 0
UNVALIDATED_MARKET_SCOPE_EXPANSION = 0
META_LEARNER_PRODUCTION_AUTHORITY = 0
WHALE_IDENTITY_CLAIM_FROM_UNOBSERVABLE_DATA = 0
RAW_NEWS_TEXT_DIRECT_TO_ORDER = 0
```
