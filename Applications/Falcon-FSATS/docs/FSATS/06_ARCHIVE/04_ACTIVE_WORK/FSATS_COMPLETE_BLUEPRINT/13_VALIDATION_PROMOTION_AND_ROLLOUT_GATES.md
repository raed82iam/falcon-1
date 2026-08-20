# FSATS Complete Blueprint — Validation, Promotion and Rollout Gates

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`
**Paper / Shadow / Tiny Live / Live Authority:** `NOT GRANTED`

## 1. Prime Rule

No candidate moves toward greater real-world consequence because it merely completed the previous technical step.

```text
TESTED != VALIDATED
VALIDATED != PROMOTED
PROMOTED != RUNTIME AUTHORIZED
RUNTIME AUTHORIZED != LIVE AUTHORIZED
```

Promotion is evidence + authority + exact intended-use scope.

## 2. Evidence Layers

Candidate evidence may include:

1. static/architecture/security verification;
2. unit/property/state-machine tests;
3. historical replay;
4. synthetic/adversarial simulation;
5. provider/broker failure simulation;
6. Paper trading;
7. real-time Shadow observation;
8. Tiny Live bounded observation, only after separate authorization;
9. post-adoption monitoring.

The required set depends on consequence and intended use, but omission must be explicit and justified.

## 3. Candidate State Model

```text
DRAFT
-> ISOLATED_CANDIDATE
-> TESTED
-> VALIDATION_HOLD / VALIDATED_FOR_EXACT_SCOPE
-> APPLICATION_RECOMMENDED
-> FSA_GOVERNANCE_REVIEWED
-> OWNER/GOVERNANCE_DECIDED
-> DEPLOYMENT_ELIGIBLE
-> ENVIRONMENT_ELIGIBLE
-> ACTIVE_WITHIN_EXACT_SCOPE
-> RESTRICTED / DEMOTED / RETIRED
```

FSA review is not the Owner/governance adoption decision.

## 4. Intended-Use Gate

A candidate cannot be `VALIDATED` in the abstract. Validation always answers:

> Validated for what exact use, market, instrument characteristics, environment, capital/risk envelope and conditions?

Required dimensions include:

- market/asset class;
- account/environment;
- strategy/model purpose;
- data products/quality;
- liquidity/volatility/regime;
- session;
- broker/execution assumptions;
- resource requirements;
- risk envelope;
- known unknowns;
- prohibited conditions.

## 5. Offline Validation Gate

Before Paper eligibility, material trading candidates should demonstrate as applicable:

- deterministic build identity;
- no architecture/security critical findings;
- valid state-machine behavior;
- no hard-risk bypass;
- replay/simulation evidence;
- adverse-regime evidence;
- realistic transaction/execution cost sensitivity;
- minimum sample adequacy;
- no obvious overfit instability;
- failure/recovery behavior;
- exact candidate digest/provenance.

## 6. Paper Gate

Paper is a controlled operational-integration test with simulated financial effect.

Paper verifies:

- real provider/broker API integration when available/authorized;
- account/environment isolation;
- event ordering;
- order/reconciliation logic;
- end-to-end latency;
- strategy behavior under live-time conditions;
- operational stability;
- capital/risk correctness;
- observability;
- restart/recovery;
- provider/broker degradation.

Paper does not prove realistic fills or market impact.

## 7. Paper Minimum Evidence

No one fixed universal day count or trade count is embedded in architecture.

A promotion package must justify evidence sufficiency using:

- number of independent opportunities/trades;
- regime diversity;
- session diversity;
- market/instrument diversity within intended scope;
- exposure to adverse conditions;
- confidence interval/uncertainty;
- operational uptime;
- failure/recovery exercises;
- Paper Reality Gap assumptions.

A strategy with few trades may require longer time. A high-frequency strategy may require far more samples despite short elapsed time.

## 8. Shadow Gate

Shadow runs current decisions against real-time operational data without broker financial side effects.

Shadow verifies:

- decision timeliness;
- real data behavior;
- applicability/regime classification;
- hypothetical order timing;
- provider/market-session assumptions;
- comparison to Paper/fill simulation;
- stability under full production-like cadence.

Shadow cannot share a route that can accidentally become Live order submission through a boolean flag alone.

## 9. Paper / Shadow Cross-Comparison

Maintain matched cases where possible:

```text
SAME MARKET OBSERVATION
-> PAPER EXECUTION RESULT
-> SHADOW HYPOTHETICAL RESULT
-> FSTSIMA EXECUTION MODEL RESULT
-> DIVERGENCE ANALYSIS
```

This identifies whether edge comes from strategy logic or optimistic execution assumptions.

## 10. Tiny Live Gate

Tiny Live is a high-consequence future stage and requires explicit Owner/governance authorization.

Before Tiny Live eligibility:

- Paper and Shadow evidence are adequate for intended use;
- FSTSimA adverse scenarios pass;
- Paper Reality Gap is quantified enough to define a conservative live envelope;
- external egress/credential boundaries are implemented/verified;
- Guardian protection route is implemented/verified;
- reconciliation is proven;
- rollback/stop/manual control is proven;
- resource/observability/recovery are proven;
- security review is current;
- exact capital/risk ceilings are explicitly authorized.

No silence/timer may grant Tiny Live.

## 11. Tiny Live Purpose

The first Tiny Live objective is **measurement of reality**, not maximizing profit.

Measure:

- actual slippage;
- fill probability;
- latency;
- order rejection;
- market impact proxy;
- broker behavior;
- operational failure rate;
- divergence from Paper/Shadow/FSTSimA;
- strategy calibration;
- drawdown behavior.

A profitable Tiny Live period with unexplained divergence is not automatically a PASS.

## 12. Matched-Capital Validation

Where feasible and separately authorized, maintain a matched Paper/Live validation cohort so the same capital/risk scale can be compared across environments.

The exact capital amount is not fixed by this design. It is an Owner-controlled future decision.

## 13. Live Expansion

Expansion can occur only by bounded steps such as:

- more capital;
- more instruments;
- broader sessions;
- additional strategy;
- additional market;
- additional broker;
- additional user.

Each expansion changes the intended-use envelope and requires evidence/review proportionate to the changed consequence.

## 14. Automatic Demotion

Eligibility can decrease automatically under pre-approved protective rules even when promotion requires stronger authority.

Demotion triggers may include:

- drawdown/loss condition;
- strategy drift;
- model calibration failure;
- Paper/Live divergence;
- execution degradation;
- data-quality degradation;
- provider/broker instability;
- Guardian incident;
- risk-limit pressure;
- simulator fidelity concern;
- security/integrity anomaly;
- unresolved reconciliation.

Safety may remove authority faster than it grants authority.

## 15. Promotion Independence

The producer/builder of a candidate cannot be its sole validator and promoter.

For AI/self-developed candidates:

```text
ORIGINATOR
-> REQUIRED LSA/MSA APPLICATION REVIEW
-> INDEPENDENT VALIDATION / RED TEAM AS REQUIRED
-> FSA OS-GOVERNANCE REVIEW
-> SEPARATE OWNER / GOVERNANCE DECISION
```

## 16. Performance Promotion Metrics

Profit alone is insufficient.

Candidate metrics include:

- net return after realistic cost assumptions;
- drawdown;
- downside/tail risk;
- hit rate only where meaningful;
- payoff ratio;
- risk-adjusted return;
- stability by regime;
- execution quality;
- opportunity frequency;
- capital efficiency;
- calibration/confidence quality;
- operational failure contribution;
- data dependency sensitivity;
- correlation/diversification effect.

## 17. Stop Conditions

Every promoted candidate declares stop/demotion conditions before activation.

Examples:

- maximum allowed drawdown/loss envelope;
- consecutive anomaly limit;
- confidence/calibration breakdown;
- provider quality below intended-use minimum;
- broker reconciliation ambiguity;
- execution slippage beyond validated band;
- resource starvation;
- Guardian restriction;
- model integrity anomaly.

Unknown severe state defaults to safer behavior.

## 18. Rollback Readiness

A promotion cannot occur if rollback/recovery is required but unproven.

Rollback evidence includes:

- prior trusted artifact/config/model;
- compatible state schema;
- migration rollback/forward-repair plan;
- authority reconciliation;
- monitoring after rollback;
- preservation of failed-candidate evidence.

## 19. Post-Promotion Observation

A newly activated change enters heightened observation.

Monitor:

- behavior versus expected baseline;
- error/failure rate;
- resource use;
- latency;
- decision/output distribution;
- financial outcome within scope;
- Guardian/Risk interactions;
- Paper/Live divergence where applicable;
- unexplained state drift.

Success is confirmed only after current evidence, not assumed from deployment.

## 20. Strategy Retirement

A strategy may be retired when:

- its edge disappears;
- costs make it unprofitable;
- market structure changes;
- calibration degrades;
- better strategies supersede it;
- provider/broker capability required is no longer available;
- risk efficiency becomes poor;
- maintenance cost exceeds value.

Historical evidence remains available for learning.

## 21. Market Expansion Gate

Adding a new market requires:

- Market Profile;
- provider Data Products;
- broker capability;
- execution semantics;
- risk model coverage;
- strategy applicability review;
- FSTSimA market qualification;
- contract/FCR impact;
- security/egress impact;
- Owner acceptance/authorization.

No strategy success in US Equities or Crypto automatically authorizes another market.

## 22. Broker Expansion Gate

Adding a broker requires:

- adapter capability profile;
- account/environment isolation;
- canonical order-state mapping;
- reconciliation proof;
- error/ambiguity behavior;
- Paper evidence if available;
- credential/egress authority;
- performance/failure tests;
- Owner authorization.

## 23. Provider Expansion Gate

Adding a provider requires:

- Provider/ServiceRole/APIInstance registration;
- Data Product mapping;
- entitlement/quota/cost profile;
- quality/freshness verification;
- failover behavior;
- egress credential authority;
- no Trading code change beyond governed product configuration when existing semantics suffice.

## 24. Acceptance Gates

```text
PAPER_SUCCESS_AS_LIVE_AUTHORITY = 0
FIXED_DAY_COUNT_AS_UNIVERSAL_VALIDATION = 0
PROMOTION_WITHOUT_INTENDED_USE = 0
PROMOTION_WITHOUT_STOP_CONDITIONS = 0
PRODUCER_AS_SOLE_VALIDATOR = 0
TINY_LIVE_FROM_OWNER_SILENCE = 0
PROFIT_AS_ONLY_PASS_METRIC = 0
LIVE_EXPANSION_WITHOUT_NEW_SCOPE_EVIDENCE = 0
ROLLBACK_REQUIRED_BUT_UNPROVEN = 0
```
