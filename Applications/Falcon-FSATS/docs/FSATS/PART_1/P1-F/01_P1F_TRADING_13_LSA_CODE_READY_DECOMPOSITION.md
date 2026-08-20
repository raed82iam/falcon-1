# P1-F — Trading Application 13-LSA Code-Ready Decomposition

**Status:** `DESIGN_MATERIALIZATION / OWNER_DIRECTED_COMPLETION_CYCLE`  
**Scope:** `P1-F DESIGN ONLY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## 1. Controlling Boundary

The Falcon Self-Aware Trading Application remains one independent Falcon Application with exactly one MSA and thirteen major branches. FSATS is not its runtime owner. Trading does not own Foundation lifecycle/resource/security truth, FSAPMA provider truth, Guardian protection authority, FSTSimA validation truth, APP-RSC coordination authority, or Owner governance.

Initial market scope remains US Equities + Crypto Spot with 1:1 funded exposure unless separately changed by governed authority.

## 2. Physical Placement

P1-C topology applies:

```text
Trading.Contracts
Trading.Domain
Trading.Application
Trading.Infrastructure
Trading.Awareness
Trading.Host
```

No cross-Application direct project reference is permitted. Cross-Application data/commands use governed contracts/routes materialized in P1-K.

## 3. Branch Decomposition

### T-LSA-01 Operations, Account & Environment
Owns Trading account/environment business state, environment eligibility, account trading-mode state, broker-account reference mapping, market-session operating context, and execution-environment readiness. It does not own secret storage or broker execution lifecycle.

Core components: `TradingEnvironmentRegistry`, `TradingAccountContext`, `EnvironmentReadinessEvaluator`, `BrokerAccountReferenceMapper`.

### T-LSA-02 Market & Instrument Universe
Owns Market Models, instrument eligibility, candidate universe, protected/managed-position set distinction, market-specific constraints and instrument mapping from producer-owned external-data identities into Trading-owned domain identities.

Invariant: `CANDIDATE_UNIVERSE != MANAGED_POSITION_SET`.

### T-LSA-03 Analysis Frameworks
Owns reusable Trading analysis frameworks, indicator/feature orchestration, evidence quality and analysis outputs. It consumes governed operational data but never becomes a provider gateway.

### T-LSA-04 Classical Trading School
Owns classical-school method implementations and school-local evidence, not global strategy selection or capital authority.

### T-LSA-05 Opportunity Hunting School
Owns opportunity-hunting methods including whale/liquidity/opportunity discovery logic within approved market models. Discovery never creates execution authority.

### T-LSA-06 Strategy Orchestration & Decision
Owns the central Strategy Registry/Controller, strategy eligibility/configuration, school weighting, decision synthesis and conflict handling. Strategies remain centrally registered, not duplicated per market.

Decision outputs are attributable proposals/decisions and remain separate from execution authorization.

### T-LSA-07 Unified Risk Management
Owns Trading business Risk across trade/instrument/strategy/market/correlation/portfolio/concentration/drawdown/session-loss/liquidity/volatility/slippage/existing obligations/worst-credible-loss/capital floors/current context and Guardian restrictions.

Invariant: `DYNAMIC_RISK != UNBOUNDED_RISK`; risk may always reduce toward zero/no-trade.

### T-LSA-08 Portfolio & Capital Management
Owns Trading portfolio state, capital reservation/use, allocation among Trading opportunities and exposure accounting. It cannot mint Foundation resources or override APP-RSC/Foundation resource envelopes.

### T-LSA-09 Execution & Position Lifecycle
Owns broker-order lifecycle, execution interpretation, positions and reconciliation. Exact outcome truth remains:

```text
ORDER_REQUEST != SUBMISSION_ATTEMPT
SUBMISSION_ATTEMPT != BROKER_ACK
BROKER_ACK != FILL
PARTIAL_FILL != FULL_FILL
CANCEL_REQUEST != CANCELLED
CLOSE_REQUEST != ZERO_EXPOSURE
```

Ambiguous execution goes to reconciliation, never blind retry.

Pre-broker binding includes user/account/environment/instrument/broker/order intent/capability/Risk version/capital reservation/Guardian+Owner controls/validation scope.

### T-LSA-10 Trading Learning & Knowledge
Owns Trading knowledge, outcome learning, durable domain lessons and evidence-backed learning records. Learning cannot change authority, goals or protected architecture.

### T-LSA-11 Trading Analytics & Attribution
Owns Trading performance analytics, attribution, decision/outcome separation, strategy/school/market/account attribution and explainability evidence.

### T-LSA-12 Strategy Evolution & Experimentation
Owns governed generation/modification/reweighting experiments for Trading strategies in isolated non-production paths. Candidate creation does not create adoption or deployment authority. FSTSimA supplies simulation/validation laboratory behavior through governed contracts.

### T-LSA-13 Trading Resource Management
Owns Trading-side resource awareness/evaluation only: current admitted allocation, demand, pressure, minimum-safe requirement, desired capacity, reclaimability, degradation/shedding consequence and restoration need.

```text
T_LSA13 != APP_RSC
T_LSA13 != FOUNDATION_RESOURCE_GOVERNANCE
```

It produces attributable resource evidence to APP-RSC and consumes APP-RSC effective coordination outcomes. It cannot independently bypass APP-RSC for FSATS redistribution/additional-resource control.

## 4. Internal Dependency Direction

Allowed conceptual flow:

```text
Environment/Universe/Data Context
 -> Analysis/Schools
 -> Strategy Orchestration
 -> Unified Risk
 -> Portfolio/Capital Reservation
 -> Execution/Position Lifecycle
 -> Reconciliation/Analytics/Learning
```

Risk and Guardian restrictions may veto or reduce downstream action. Execution truth feeds portfolio, analytics, learning and safety continuity. No school may call broker infrastructure directly.

## 5. Safety Continuity Binding

Before any exposure-opening action, Trading must have a valid AI-independent Position Safety Envelope containing position/instrument/quantity/maximum authorized loss/protection state/emergency exit rule/broker protection state/last trusted Risk decision/safety owner/recovery policy as applicable.

If affected AI becomes untrusted:

- new intelligent risk is denied;
- queued/cached/scheduled/in-flight risk-increasing derived work is fenced by trust/causation epoch;
- valid independent protective work is not blindly cancelled;
- open/pending/partial exposure remains owned and reconciled;
- risk-monotonic degraded authority permits reduce/protect/reconcile/exit, not expansion;
- Safety Continuity remains active through repair and Controlled Revival.

## 6. Broker Capability Boundary

Broker capability state distinguishes `SUPPORTED`, `UNSUPPORTED`, `CONDITIONAL`, `UNKNOWN`. `UNKNOWN != SUPPORTED`. Unsupported/unknown semantics may not be silently emulated if material meaning changes.

Provider/data credentials are not Trading-owned. User broker-execution credential references apply only when an execution capability requiring them is enabled; advisory/non-execution use does not require them.

## 7. Concurrency and State

Every order/position/reservation/protection mutation requires immutable identity, causation/correlation, expected-version or equivalent optimistic-concurrency protection, idempotency where retries are valid, and reconciliation for ambiguous external outcomes.

No mutable shared singleton may bypass branch ownership. Durable safety/reconciliation state must be reconstructable without the killed AI's volatile memory.

## 8. Failure / Degraded Rules

- provider data stale/unknown -> affected intelligent decisions denied or reduced; existing obligations continue via authoritative execution/account/protection truth where available;
- Risk unavailable/untrusted -> no new risk;
- Execution truth uncertain -> reconciliation state;
- Guardian restriction unknown where required -> fail closed for risk expansion;
- APP-RSC unavailable -> no new cross-App resource redistribution assumption; remain within last valid admitted/effective resource truth where safe;
- FSTSimA unavailable -> no simulation-derived promotion claim;
- MSA/LSA/CSA fault -> Safety Continuity + AI Repair/Controlled Recovery rules apply by scope.

## 9. Required Implementation Tests for Later Code

Each branch requires unit/contract/state tests plus cross-branch fixtures for stale data, duplicate decisions, partial fill, cancel race, stop/protection race, position leaves candidate universe, Risk downgrade, Guardian restriction, broker unknown capability, credential unavailable/revoked, APP-RSC pressure, AI Kill with queued order, restart with unresolved reconciliation, and Controlled Revival.

## 10. P1-F Closure Invariants

- all 13 LSAs have one non-overlapping accountable responsibility;
- no Trading responsibility is orphaned behind a generic engine;
- strategies remain central and market-configured;
- Unified Risk remains single Trading business Risk owner;
- T-LSA-09 owns execution/position truth;
- T-LSA-13 is resource awareness/evaluation, not APP-RSC;
- Safety Continuity and AI Repair/Recovery are structurally bound;
- no direct cross-Application internals or Foundation-source coupling is introduced.
