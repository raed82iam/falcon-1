# FSATS Complete Blueprint — Owner-Requested CSA Assignment and Eligibility Register

**Candidate:** `FSATS-CB-v0.1 / CSA SEMANTIC REVISION`
**Status:** `OWNER_REQUESTED_SEMANTIC_CHANGE_APPLIED / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Date:** `2026-08-11`
**Implementation Authority:** `NOT GRANTED`
**Runtime Authority:** `NOT GRANTED`
**Controlling Scope:** CSA assignment for currently defined FSATS strategy and intelligent components only

## 1. Owner Direction

The Project Owner directed the FSATS design candidate to assign Component Self-Awareness (CSA) to the components that actually need it.

This record applies that direction without turning CSA into a default property of every component.

## 2. Governing Eligibility Rule

Current Foundation authority `AWR-008 — Component Self-Awareness v1.1` establishes:

- CSA is optional;
- health reporting alone does not establish self-awareness;
- CSA is eligible only where meaningful self-development value is established through specialized intelligence, self-evaluation, learning or research capability, owned improvement opportunities, and safe candidate testing;
- deterministic validators, passive structures, simple storage adapters, basic configuration loaders and ordinary infrastructure should not receive CSA;
- one CSA belongs to one eligible intelligent component, one parent LSA and one Application;
- a CSA-originated production-bound proposal follows `CSA -> Parent LSA -> Application MSA -> FSA`, followed by separate Owner/governance adoption authority;
- CSA cannot expand responsibility, authority, permissions, market scope, protected architecture or production state by itself.

`APP-001`, `CON-023` and `ADR-I015` further require optional CSA identities/eligibility to remain explicit and origin-correct.

## 3. Assignment Decision

The current FSATS Complete Blueprint assigns exactly:

```text
STRATEGY CSA IDENTITIES       = 14
INTELLIGENCE CSA IDENTITIES   = 11
META-LEARNER CSA IDENTITIES   = 1
---------------------------------
TOTAL ASSIGNED CSA            = 26
```

These are awareness identities, not 26 separate deployable Applications or 26 mandatory processes.

A shared CSA implementation framework may be reused technically, but every CSA instance must retain separate component identity, Self-Knowledge, evidence, candidate lineage, parent-LSA binding and authority scope.

## 4. Trading Strategy CSA Assignments — 14

Every currently defined initial strategy is treated as an eligible intelligent strategy component because the design requires strategy-specific self-evaluation of regime fitness, calibration, recurring failure patterns, feature usefulness, execution sensitivity and bounded candidate improvements.

### T-LSA-04 — Classical Trading School

| CSA Identity | Component | Parent LSA | Assignment |
|---|---|---|---|
| `CSA-T-CLS-001` | `CLS-001 Multi-Horizon Trend Continuation` | `T-LSA-04` | ASSIGNED |
| `CSA-T-CLS-002` | `CLS-002 Momentum Breakout` | `T-LSA-04` | ASSIGNED |
| `CSA-T-CLS-003` | `CLS-003 Pullback Continuation` | `T-LSA-04` | ASSIGNED |
| `CSA-T-CLS-004` | `CLS-004 Mean Reversion` | `T-LSA-04` | ASSIGNED |
| `CSA-T-CLS-005` | `CLS-005 Volatility Compression / Expansion` | `T-LSA-04` | ASSIGNED |
| `CSA-T-CLS-006` | `CLS-006 Relative Strength / Weakness Rotation` | `T-LSA-04` | ASSIGNED |

### T-LSA-05 — Opportunity Hunting School

| CSA Identity | Component | Parent LSA | Assignment |
|---|---|---|---|
| `CSA-T-HNT-001` | `HNT-001 Unusual Volume / Participation Surge` | `T-LSA-05` | ASSIGNED |
| `CSA-T-HNT-002` | `HNT-002 Momentum Ignition` | `T-LSA-05` | ASSIGNED |
| `CSA-T-HNT-003` | `HNT-003 Gap / Session Transition Hunter` | `T-LSA-05` | ASSIGNED |
| `CSA-T-HNT-004` | `HNT-004 Large-Flow / Whale Signature Hunter` | `T-LSA-05` | ASSIGNED |
| `CSA-T-HNT-005` | `HNT-005 Liquidity Vacuum / Refill Hunter` | `T-LSA-05` | ASSIGNED |
| `CSA-T-HNT-006` | `HNT-006 Cross-Instrument / Cross-Market Dislocation Hunter` | `T-LSA-05` | ASSIGNED |
| `CSA-T-HNT-007` | `HNT-007 Crypto Continuous-Regime Transition Hunter` | `T-LSA-05` | ASSIGNED |
| `CSA-T-HNT-008` | `HNT-008 Catalyst / Event Reaction Hunter` | `T-LSA-05` | ASSIGNED |

### Strategy CSA Responsibility

Each strategy CSA may understand and evaluate only its own strategy component, including:

- current validated applicability scope;
- performance by market, regime, session and horizon;
- calibration and uncertainty;
- recurring false-positive / false-negative patterns;
- entry/exit failure patterns;
- feature usefulness and degradation;
- execution-cost and slippage sensitivity;
- capital-efficiency evidence;
- drawdown and adverse-sequence behavior;
- known blind spots and unvalidated conditions;
- current accepted strategy baseline;
- strategy-owned isolated candidate versions;
- candidate improvement evidence and rejection history.

A strategy CSA may propose isolated candidates for the same strategy responsibility. It may not:

- create a new market authority;
- change Unified Risk limits;
- increase capital allocation authority;
- bypass StrategyController;
- bypass capital reservation;
- submit orders;
- self-promote a candidate;
- convert research into operational market truth;
- redefine another strategy or another LSA's assets.

Candidate testing may use governed FSTSimA facilities while strategy ownership remains with the originating strategy component and its parent Trading LSA.

## 5. Trading Intelligence CSA Assignments — 6

| CSA Identity | Component | Parent LSA | Assignment Rationale |
|---|---|---|---|
| `CSA-T-INT-001` | `INT-001 Regime Classifier` | `T-LSA-03` | Requires calibration, regime-specific error learning, drift awareness and bounded model improvement. |
| `CSA-T-INT-002` | `INT-002 Liquidity and Execution Quality Estimator` | `T-LSA-03` | Requires self-evaluation against realized liquidity/execution outcomes and bounded estimator improvement. |
| `CSA-T-INT-003` | `INT-003 Opportunity Ranker` | `T-LSA-05` | Requires ranking-quality evaluation, missed-opportunity analysis, false-priority analysis and bounded ranking improvement. |
| `CSA-T-INT-004` | `INT-004 Strategy Applicability Model` | `T-LSA-06` | Requires scope-fit calibration, regime mismatch learning and bounded applicability-model improvement. |
| `CSA-T-INT-005` | `INT-005 Decision Calibration / Uncertainty Model` | `T-LSA-06` | Its mission is explicitly self-evaluative calibration of confidence versus outcome and uncertainty quality. |
| `CSA-T-INT-006` | `INT-006 Execution Cost / Slippage Model` | `T-LSA-09` | Requires comparison of forecast cost bands to reconciled fills and bounded execution-cost model improvement. |

None of these CSAs owns the Trading hard-risk gate, capital authority or broker execution authority.

## 6. FSAPMA Intelligence CSA Assignments — 2

| CSA Identity | Component | Parent LSA | Assignment Rationale |
|---|---|---|---|
| `CSA-P-INT-007` | `INT-007 Provider Reliability Forecast Model` | `P-LSA-06` | Requires forecast calibration against provider degradation/quota/reliability outcomes and bounded model improvement. |
| `CSA-P-INT-008` | `INT-008 Data Quality Anomaly Model` | `P-LSA-05` | Requires anomaly-quality evaluation, false-positive/false-negative learning and bounded data-quality model improvement. |

`Provider Controller` remains an operational controller and does not receive CSA merely because it consumes intelligent evidence.

The previously suggested `adaptive route-quality estimator` and `quota/capacity forecast model` are not separately assigned CSA in this revision because the current catalog does not yet define them as canonical component identities. If they become concrete intelligent components later, they require explicit AWR-008 eligibility review before CSA assignment.

## 7. Guardian Intelligence CSA Assignment — 1

| CSA Identity | Component | Parent LSA | Assignment Rationale |
|---|---|---|---|
| `CSA-G-INT-009` | `INT-009 Guardian Incident Correlation Model` | `G-LSA-01` | Requires correlation-quality evaluation, missed-incident/false-incident learning and bounded model improvement while remaining separate from command authority. |

Guardian hard protection-command logic, scope enforcement, expiry validation, command idempotency and release authority remain deterministic/governed and do not receive CSA.

The previously suggested crisis-consequence estimator and recovery-quality anomaly detector remain future eligibility candidates until they are defined as concrete component identities.

## 8. FSTSimA Intelligence CSA Assignments — 2

| CSA Identity | Component | Parent LSA | Assignment Rationale |
|---|---|---|---|
| `CSA-S-INT-010` | `INT-010 FSTSimA Synthetic Scenario Generator` | `S-LSA-02` | Requires scenario-quality/diversity evaluation, coverage-gap learning and bounded generator improvement. |
| `CSA-S-INT-011` | `INT-011 FSTSimA Fidelity Calibration Model` | `S-LSA-07` | Requires comparison against observed reality, calibration-gap learning and bounded simulator-fit improvement. |

The independent `S-LSA-08` validation oracle does not receive CSA in this revision. The same evolving intelligence must not become both candidate builder/calibrator and sole independent judge of its own trustworthiness.

The configurable execution/fill model and future adversarial-scenario components remain eligible for later CSA review if they become concrete self-evaluating intelligent component identities. Their current deterministic/configurable forms do not receive CSA automatically.

## 9. Adaptive Meta-Learner CSA Assignment — 1

| CSA Identity | Component | Parent LSA | Assignment Rationale |
|---|---|---|---|
| `CSA-T-META-001` | `Adaptive Meta-Learner` | `T-LSA-12` | It explicitly learns from strategy evidence, generates bounded strategy/feature/weight candidates, compares candidates to baseline and therefore has direct specialized self-evaluation and self-improvement value. |

The Meta-Learner CSA may improve the Meta-Learner's own same-responsibility methods and may propose strategy-evolution candidates through T-LSA-12.

It is not a master strategy, does not own capital, does not approve strategy promotion and does not bypass the origin-correct review chain.

## 10. Explicit No-CSA Set

The following current components/roles SHALL NOT receive CSA merely because they are important or consume AI outputs:

```text
StrategyController
StrategyCatalog
Unified Risk hard gate
Global Capital Reservation Ledger
OrderStateMachine
BrokerAdapter
ReconciliationController
Provider Controller
Provider registries
normalizers / serializers / DTOs
hard data validators
Guardian command / restriction authority logic
Guardian expiry / idempotency logic
FSARM
Monitor AI
S-LSA-08 independent validation oracle
Foundation-owned FSA internals
ordinary storage/configuration/infrastructure adapters
```

Importance does not create CSA eligibility.

## 11. Monitor AI Separation

Monitor AI is not CSA.

The eight FSATS Application MSA Monitor AI perspectives remain bounded oversight tools with no autonomous self-development authority under the current design.

No CSA is inserted beneath Monitor AI and no recursive monitor-awareness hierarchy is created.

## 12. Internet and Research Boundary

CSA direct unrestricted Internet access is not created by this assignment.

Any CSA research must use the same governed Application-awareness research path defined by the Blueprint and FCR-0008 when that Foundation capability exists and is separately authorized.

Trading MSA and FSA direct Internet prohibitions remain unchanged.

Research content remains non-operational and non-authoritative until governed evidence conversion and validation complete.

## 13. Self-Development and Candidate Ownership

Every assigned CSA follows:

```text
OBSERVE COMPONENT
-> MEASURE COMPONENT
-> IDENTIFY SAME-RESPONSIBILITY GAP
-> LEARN / RESEARCH IF AUTHORIZED
-> FORM HYPOTHESIS
-> BUILD ISOLATED COMPONENT-OWNED CANDIDATE
-> TEST / CHALLENGE
-> COMPARE TO CURRENT BASELINE
-> CSA RECOMMENDATION
-> PARENT LSA REVIEW
-> APPLICATION MSA REVIEW
-> FSA OS-GOVERNANCE / COMPATIBILITY REVIEW
-> SEPARATE OWNER / GOVERNANCE ADOPTION DECISION
-> SEPARATELY AUTHORIZED DEPLOYMENT
```

No arrow grants the next state automatically.

## 14. Manifest Requirement

When implementation planning later materializes Application manifests, each Application SHALL declare the CSA identities assigned here with:

- exact owning component identity;
- exact parent LSA;
- Application identity;
- eligibility basis;
- responsibility boundary;
- authority ceiling;
- permissions;
- research capability status;
- self-development origin path;
- candidate ownership boundary;
- evidence requirements;
- lifecycle and disable/revocation behavior.

Undeclared CSA identity or undeclared CSA authority fails closed.

## 15. Count by Application

```text
TRADING APPLICATION
  Strategy CSA          = 14
  Intelligence CSA      = 6
  Adaptive Meta CSA     = 1
  TOTAL                 = 21

FSAPMA
  Intelligence CSA      = 2
  TOTAL                 = 2

TRADING GUARDIAN
  Intelligence CSA      = 1
  TOTAL                 = 1

FSTSIMA
  Intelligence CSA      = 2
  TOTAL                 = 2

FSATS TOTAL ASSIGNED CSA = 26
```

## 16. Supersession / Interpretation Rule

For the current Blueprint candidate, this record supersedes earlier non-final phrases such as `May be CSA-eligible`, `Possible CSA eligibility`, `Suggested CSA Candidates`, or equivalent optional wording **only for the 26 component identities explicitly assigned above**.

All other components remain `NO_CSA_BY_DEFAULT` unless a later governed semantic change establishes AWR-008 eligibility and explicit assignment.

The broader architectural rule remains true:

```text
CSA = OPTIONAL / ELIGIBILITY-BASED
```

Optional means not every component receives CSA. It does not mean an explicitly assigned eligible CSA is optional at implementation time once the final design baseline is Owner-accepted and implementation of that component is separately authorized.

## 17. Non-Authority

This semantic revision does not grant:

- Owner acceptance of the Blueprint;
- implementation authority;
- runtime authority;
- research egress implementation;
- FSA implementation;
- Paper, Shadow, Tiny Live or Live authority;
- strategy promotion;
- model promotion;
- self-deployment;
- new market authority;
- new capital authority.

The changed candidate requires a fresh semantic freeze, Architecture/Consistency review, fresh Red-Team review and Owner final decision under `WORKSTREAM_RULES.md`.
