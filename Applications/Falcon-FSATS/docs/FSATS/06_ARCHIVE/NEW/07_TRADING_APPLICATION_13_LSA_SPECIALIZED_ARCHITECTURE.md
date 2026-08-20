# Falcon Self-Aware Trading Application — 13-LSA Specialized Implementation Architecture

**Package:** `FSATS-SIA-v0.1`
**Application:** `APP-TRD`
**MSA:** `MSA-TRD`
**Status:** `DESIGN_CANDIDATE`

## 1. Application Mission

APP-TRD transforms governed operational Data Products and account/market evidence into bounded Trading decisions, risk/capital decisions, execution intent and reconciled order/position state.

It is the business owner of Trading semantics, not operational provider acquisition, independent Guardian protection, Foundation resource governance, FSA, or FSATS-wide resource redistribution.

## 2. Primary End-to-End Pipeline

```text
ACCOUNT + MARKET PROFILE + DATA PRODUCTS
-> UNIVERSE / FEATURES
-> STRATEGY EVALUATION
-> STRATEGY ORCHESTRATION
-> TRADE PROPOSAL
-> UNIFIED RISK
-> CAPITAL RESERVATION
-> EXECUTION INTENT
-> BROKER SUBMISSION / RECONCILIATION
-> ORDER / POSITION STATE
-> OUTCOME / LEARNING / ATTRIBUTION
```

No ordinary Trading order path bypasses T-LSA-06 -> T-LSA-07 -> T-LSA-08 -> T-LSA-09.

## 3. T-LSA-01 — Operations, Account & Environment

### Components

- `T01.AccountProfileRegistry`
- `T01.EnvironmentProfileRegistry`
- `T01.AccountStateReconciler`
- `T01.TradingReadinessEvaluator`
- `T01.SessionCapabilityResolver`

### Owned state

- immutable Trading account profile/version;
- broker-account mapping reference;
- business environment classification `SIMULATION | PAPER | TINY_LIVE | LIVE` as a descriptive state only;
- account capability projection;
- current reconciled buying-power/account snapshot reference;
- Trading readiness result.

The environment label does not create runtime authority. Authority is separately supplied by governed external/Owner state.

### Input ports

- reconciled broker/account evidence from T-LSA-09;
- market profile/session state from T-LSA-02;
- Guardian restriction state from APP-GRD contract;
- Foundation lifecycle/authority availability projection through FoundationAdapters.

### Output

`TradingOperatingContextSnapshot` with exact version, freshness, account, environment, permissions, market/session capability and readiness status.

### Algorithm

`TradingReadinessEvaluator` returns READY only when all mandatory gates are true:

1. Application lifecycle allows operation;
2. exact environment has separate required business authority;
3. account mapping is valid;
4. account evidence is fresh enough for the requested action;
5. market/instrument/session capability is known;
6. no applicable Guardian restriction blocks the action;
7. required Foundation dependency state is available;
8. broker execution capability is available for execution actions.

Any material UNKNOWN => `NOT_READY_UNKNOWN`.

### Consistency

Single-writer aggregate per `TradingAccountId`; immutable snapshots are read concurrently.

### Fail closed

No new exposure when account/buying-power/environment authority is stale, conflicted or unknown. Safe cancel/reconcile/exit actions may be allowed by explicit degraded rules.

### CSA

Default: no CSA for registries/reconciler. An intelligent account-anomaly model may be separately eligible, but it cannot own account truth.

## 4. T-LSA-02 — Market & Instrument Universe

### Components

- `T02.MarketProfileRegistry`
- `T02.InstrumentMaster`
- `T02.TradabilityResolver`
- `T02.UniverseEligibilityEngine`
- `T02.UniverseRanker`
- `T02.UniverseSnapshotStore`

### Owned state

- market profiles and versions;
- instrument semantic definitions;
- tradability/capability projections;
- universe policy versions;
- immutable qualified/ranked universe snapshots.

Operational quote/trade data remains sourced from FSAPMA; T-LSA-02 does not become a provider.

### US Equities initial universe policy

Capital tier is an eligibility envelope, **not the ranking score**:

```text
ZONE_C: effective deployable capital < 500 USD
  eligible price band: price < 30 USD
  target selected count: 10

ZONE_B: 500 <= effective deployable capital < 2000 USD
  tier-B eligible price band: price < 250 USD
  selected: top 10 tier-B + currently qualified Zone-C set

ZONE_A: effective deployable capital >= 2000 USD
  tier-A eligible price band: price >= 250 USD
  selected: top 10 tier-A + currently qualified Zone-B + Zone-C sets
```

Fractional-share support may widen execution feasibility but does not eliminate liquidity/data/risk hard gates.

### Hard eligibility gates

An instrument is excluded if any required condition is false/unknown:

- broker/account tradable capability;
- required market session support;
- price/quantity precision known;
- required Data Products available and quality acceptable;
- minimum liquidity/spread rule satisfied;
- no market/instrument Guardian prohibition;
- no unresolved corporate-action identity conflict;
- supported strategy set is non-empty;
- execution-cost estimate can be produced within allowed uncertainty.

### Ranking score

For eligible US equities, initial deterministic rank score is integer `0..10000`:

```text
25% LiquidityScore
20% ParticipationVolumeScore
15% SpreadExecutionScore
15% OpportunityDensityScore
10% DataQualityScore
10% VolatilityTradabilityScore
 5% DiversificationScore
```

Each subscore is normalized `0..10000` by a versioned formula in the market profile. Final score uses integer weighted sum with exact weights above. Tie-break order:

1. higher DataQualityScore;
2. higher LiquidityScore;
3. lower expected execution cost;
4. lexical canonical `InstrumentId` for deterministic stability.

Ranking does not create a trade recommendation.

### Update cadence

- market/instrument profile: event/version driven;
- candidate scan: configurable by market phase, bounded by FSARM/resource state;
- rank snapshot: new immutable version only when source evidence set/version changes or scheduled refresh occurs.

### Consistency

Universe snapshots are immutable. One rank build writes a new snapshot; readers pin one snapshot ID for a decision cycle.

### CSA

`T02.UniverseRanker` may be CSA-eligible only if an intelligent ranking model replaces/augments deterministic subscores. Hard eligibility gates remain deterministic and outside CSA authority.

## 5. T-LSA-03 — Analysis Frameworks

### Components

- `T03.FeatureRegistry`
- `T03.FeatureDependencyPlanner`
- `T03.FeatureEngine`
- `T03.FeatureCache`
- `T03.LookAheadLeakageGuard`
- `T03.RegimeClassifier`
- `T03.LiquidityExecutionEstimator`
- `T03.DataSufficiencyEvaluator`

### Feature contract

Every feature defines:

```text
FeatureId + Version
Formula/model identity
Inputs/DataProduct versions
Units
Lookback
Warmup
Freshness
Missing-data rule
Market applicability
Output range
Determinism/random-seed rule
Leakage prohibition
Cost class
```

A feature version is immutable.

### Leakage guard

For a decision effective at time `t`, a feature may consume only observations whose authoritative availability/effective semantics permit use at or before `t`. Later correction/replay may be used for post-analysis but not retroactively represented as decision-time evidence.

### Regime output

`RegimeClassifier` emits calibrated probabilities for:

- TRENDING;
- RANGING;
- HIGH_VOLATILITY;
- LOW_VOLATILITY;
- LIQUIDITY_STRESS;
- TRANSITION_UNCERTAIN.

Probabilities must sum to 10000 basis points ± deterministic rounding correction applied to the largest component.

### Consistency

Feature cache key includes feature version + exact input snapshot/window identity + market/instrument + effective boundary. Cache entries are immutable.

### Fail closed

Missing required feature, stale input, leakage suspicion, incompatible feature version or insufficient warmup => strategy applicability fails for strategies requiring that feature.

### CSA

Eligible intelligent components:

- `T03.RegimeClassifier`;
- `T03.LiquidityExecutionEstimator`.

Not eligible by default: deterministic registry/cache/leakage guard.

## 6. T-LSA-04 — Classical Trading School

### Components

- `T04.ClassicalStrategyRegistry`
- `T04.CLS001.TrendContinuation`
- `T04.CLS002.MomentumBreakout`
- `T04.CLS003.PullbackContinuation`
- `T04.CLS004.MeanReversion`
- `T04.CLS005.VolatilityCompressionExpansion`
- `T04.CLS006.RelativeStrengthRotation`

### Ownership

Owns classical strategy business hypotheses and evaluations. Does not own portfolio-level conflict resolution, Risk, capital reservation or broker execution.

### Strategy output

Every evaluation returns immutable `StrategyEvaluation`:

```text
StrategyId/Version
InstrumentId
MarketProfileVersion
InputSnapshotIds
Applicability = APPLICABLE | NOT_APPLICABLE | INSUFFICIENT_EVIDENCE
Direction
EntryEnvelope
InvalidationLevel
TargetHypothesis[]
ExpectedHorizon
RawEdgeScore
ConfidenceScore + calibration version
ExpectedRiskDescriptor
ExpectedExecutionCostRef
Expiry
ReasonCodes
EvidenceRefs
```

Exact algorithms are defined in file 17.

### CSA

Each strategy implementation may be individually CSA-eligible only when it uses meaningful specialized intelligence/self-evaluation. A deterministic strategy rule-set is not automatically CSA.

## 7. T-LSA-05 — Opportunity Hunting School

### Components

- `T05.HunterRegistry`
- `T05.HNT001.UnusualParticipation`
- `T05.HNT002.MomentumIgnition`
- `T05.HNT003.GapSessionTransition`
- `T05.HNT004.LargeFlowSignature`
- `T05.HNT005.LiquidityVacuumRefill`
- `T05.HNT006.CrossInstrumentDislocation`
- `T05.HNT007.CryptoContinuousRegimeTransition`
- `T05.HNT008.CatalystReaction`
- `T05.OpportunityRanker`

### Two-stage resource model

```text
CHEAP DISCOVERY
-> rank top candidate set
-> EXPENSIVE CONFIRMATION only for selected candidates
```

The selection budget is provided by T-LSA-13/FSARM resource state. Lower resource budget reduces candidate breadth; it never weakens hard data/risk/authority gates.

### Large-flow invariant

HNT-004 may infer patterns consistent with large participation only from observable lawful data. It SHALL NOT claim hidden participant identity.

### Catalyst invariant

HNT-008 consumes a governed normalized event/news Data Product from FSAPMA. Raw unverified external text SHALL NOT directly produce a trade proposal.

### CSA

Opportunity Ranker and intelligent hunter models may be eligible. Discovery scheduling/controller logic is not automatically CSA.

## 8. T-LSA-06 — Strategy Orchestration & Decision

### Components

- `T06.StrategyController`
- `T06.ApplicabilityResolver`
- `T06.EvaluationNormalizer`
- `T06.CorrelationConflictResolver`
- `T06.DecisionCalibrationGate`
- `T06.TradeProposalBuilder`
- `T06.NoTradeReasonBuilder`

### Deterministic orchestration pipeline

```text
PIN OperatingContext + MarketProfile + UniverseSnapshot + FeatureSnapshot
-> load ACTIVE/RESTRICTED eligible strategy versions
-> apply hard applicability gates
-> request evaluations
-> reject expired/incomplete evaluations
-> normalize edge/confidence/execution cost
-> cluster correlated proposals
-> resolve direction/target conflicts
-> apply calibration/uncertainty gate
-> emit at most one proposal per governed decision key
   OR explicit NO_TRADE
```

### Decision key

Initial decision key:

`(TradingAccountId, MarketId, InstrumentId, DecisionHorizonClass, DecisionCycleId)`.

One active proposal version per key. A new proposal supersedes the previous proposal and retains causation.

### Conflict rule

Hard opposite-direction conflict where neither side exceeds the other by the configured `MIN_DIRECTION_DOMINANCE_BPS` after calibrated net-edge normalization => `NO_TRADE_CONFLICT`.

Differing take-profit hypotheses in the same direction are not resolved by selecting the largest target. The ensemble builds an evidence-weighted target distribution and T-LSA-07/T-LSA-09 receive the conservative executable envelope.

Exact scoring is in file 17.

### Consistency

Single-writer per decision key. Input snapshots are immutable/pinned. Same inputs/config/strategy versions => same normalized proposal identity.

### CSA

`StrategyApplicabilityModel`/`DecisionCalibrationModel` may be CSA-eligible. `StrategyController` authority logic itself remains deterministic and is not self-modifiable by CSA.

## 9. T-LSA-07 — Unified Risk Management

### Components

- `T07.RiskPolicyRegistry`
- `T07.ProposalRiskEvaluator`
- `T07.PortfolioRiskEvaluator`
- `T07.ConcentrationCorrelationEvaluator`
- `T07.DrawdownController`
- `T07.PositionSizingEngine`
- `T07.RiskDecisionLedger`
- optional `T07.TailRiskEstimator` intelligent model

### Risk evaluation order

A proposal passes in this fixed order:

1. authority/environment gate;
2. Guardian restriction gate;
3. data/market/account readiness gate;
4. instrument/market risk eligibility;
5. per-trade loss limit;
6. position/instrument exposure limit;
7. market exposure limit;
8. portfolio gross/net exposure;
9. concentration/correlation limit;
10. drawdown/loss-streak degradation rule;
11. liquidity/execution-capacity sizing;
12. gap/tail-risk constraint;
13. final conservative size intersection.

Any DENY at an earlier gate stops risk-increasing evaluation. Protective risk-reducing action has separately defined handling.

### Position sizing

Approved quantity is the minimum of all positive size ceilings:

```text
RiskBudgetSize
CapitalAvailableSize
LiquidityCapacitySize
Broker/InstrumentMaxSize
PortfolioExposureRemainingSize
GuardianRestrictionSize
StrategyValiditySize
```

Then quantity is rounded **down** to the allowed step for risk-increasing actions.

No model/confidence may override a deterministic hard limit.

### State

Risk policies are immutable versioned records. Risk decisions are append-only immutable evidence. Current drawdown state is authoritative aggregate per account/portfolio scope.

### Concurrency

Risk evaluation pins one portfolio snapshot/version. If portfolio state changes before capital reservation, reservation must revalidate risk-sensitive constraints or reject with `RISK_SNAPSHOT_STALE`.

### CSA

Tail-risk/forecast models may be CSA-eligible. Risk policy engine and hard-limit enforcement are not CSA-modifiable.

## 10. T-LSA-08 — Portfolio & Capital Management

### Components

- `T08.PortfolioLedger`
- `T08.CapitalStateAggregator`
- `T08.CapitalReservationManager`
- `T08.AllocationController`
- `T08.ExposureAggregator`
- `T08.SettlementBufferManager`
- `T08.CapitalReconciler`

### Authoritative business aggregates

- `PortfolioState` per account;
- `CapitalState` per account/currency/asset;
- `CapitalReservation` per reservation identity;
- allocation envelope by market/strategy bucket where configured.

### Reservation invariant

No risk-increasing execution intent may be created without an active compatible reservation.

Reservation transaction:

```text
validate RiskDecision still eligible
+ validate proposal not expired/superseded
+ validate available unreserved capital
+ create HELD reservation atomically
```

Idempotent duplicate reservation request with same idempotency key returns the existing compatible reservation; conflicting duplicate fails closed.

### Release

Unused held capital is released on terminal rejection/cancel/expiry or after fill settlement/reconciliation computes consumed amount. Release is idempotent.

### Capital allocator

Capital allocation ranking may use strategy capital-efficiency evidence, but hard Risk/Guardian/account constraints dominate. Allocation is not delegated to individual strategies.

### Consistency

`CapitalReservationManager` is serialized per `(TradingAccountId, settlement asset/currency)` using the exact concurrency model in file 14.

### CSA

An allocation forecast/recommendation model may be eligible, but reservation ledger authority is deterministic and not CSA-controlled.

## 11. T-LSA-09 — Execution & Position Lifecycle

### Components

- `T09.ExecutionIntentFactory`
- `T09.BrokerCapabilityResolver`
- `T09.OrderPolicyEngine`
- `T09.BrokerRouteSelector`
- `T09.BrokerAdapterRegistry`
- `T09.OrderChainAggregate`
- `T09.ExecutionReconciler`
- `T09.PositionLedger`
- `T09.SlippageCostModel`
- `T09.RecoveryCoordinator`

### Preconditions for new risk-increasing intent

Required:

- valid non-expired TradeProposal;
- ALLOW/ALLOW_WITH_REDUCTION RiskDecision bound to proposal;
- HELD CapitalReservation bound to proposal/risk decision;
- account/environment readiness;
- no conflicting Guardian restriction;
- broker capability available;
- market/session/order type valid;
- execution price/quantity collar valid.

### Broker submission

The adapter is invoked only from a persisted `SUBMISSION_ELIGIBLE` intent with a durable idempotency/client-order key.

Transport timeout/unknown response does **not** create a blind retry. The intent transitions to `AMBIGUOUS` and reconciliation queries broker state before any resubmission unless the broker contract proves idempotent duplicate submission semantics.

### Position truth

Position changes only from reconciled fills/corrections, not strategy expectation or order ACK.

### Order chain

Replace/cancel preserves parent `OrderChainId` and creates distinct `OrderAttemptId`/causation links. An older broker update cannot roll back a later canonical order state unless it is an explicit correction with accepted ordering semantics.

### Slippage model

Provides expected cost band/evidence. It does not submit/cancel orders and cannot override hard price collars.

### Concurrency

Single writer per `OrderChainId` and `PositionId`; account-level capital effects coordinate transactionally/outbox with T-LSA-08 boundaries defined in file 14.

### CSA

Eligible: `SlippageCostModel` if intelligent; optional execution-quality estimator. Not eligible: authoritative order/position state machines.

## 12. T-LSA-10 — Trading Learning & Knowledge

### Components

- `T10.OutcomeKnowledgeStore`
- `T10.RegimePerformanceModel`
- `T10.FailurePatternMiner`
- `T10.AssumptionCalibrationStore`
- `T10.LessonCandidateBuilder`

### Inputs

Only finalized/reconciled outcomes with provenance. Unsettled/ambiguous execution is excluded from final performance learning or labeled provisional.

### Knowledge classes

```text
OBSERVATION
MEASURED_OUTCOME
CALIBRATED_STATISTIC
HYPOTHESIS
EXPERIMENTAL_LESSON
GOVERNED_ACCEPTED_KNOWLEDGE
```

A hypothesis does not silently become accepted knowledge through repetition.

### Leakage rule

Post-outcome knowledge cannot alter the historical evidence used by the original decision record.

### CSA

`FailurePatternMiner` and intelligent regime-performance model may be eligible.

## 13. T-LSA-11 — Trading Analytics & Attribution

### Components

- `T11.PerformanceAttributionEngine`
- `T11.DecisionQualityEvaluator`
- `T11.StrategyAttributionEngine`
- `T11.ExecutionAttributionEngine`
- `T11.RiskCostAttributionEngine`
- `T11.ResourceCostAttributionEngine`
- `T11.KpiProjectionService`

### Required separations

```text
OUTCOME_QUALITY != DECISION_QUALITY
GROSS_PNL != NET_PNL
STRATEGY_EDGE != EXECUTION_RESULT
PROFIT != POLICY_COMPLIANCE
```

Attribution binds exact strategy/model/config/market/profile/execution versions and fees/slippage/funding where applicable.

Analytics is read/projection oriented and cannot rewrite authoritative order/capital/risk state.

### CSA

Optional anomaly/attribution explanation model may be eligible; core accounting arithmetic remains deterministic.

## 14. T-LSA-12 — Strategy Evolution & Experimentation

### Components

- `T12.CandidateRegistry`
- `T12.ExperimentPlanner`
- `T12.MetaLearner`
- `T12.FeatureCandidateGenerator`
- `T12.StrategyParameterCandidateGenerator`
- `T12.SchoolWeightCandidateGenerator`
- `T12.RetirementCandidateEvaluator`
- `T12.ValidationEvidenceAssembler`

### Authority boundary

T-LSA-12 can create only isolated candidates and experiments under valid authority.

It cannot:

- change active strategy bytes/config;
- deploy a candidate;
- widen market scope;
- change Risk limits;
- create broker/provider authority;
- bypass MSA/FSA/Owner review;
- mark its own experiment as production accepted.

### Required experiment comparison

Candidate is evaluated against exact baseline(s) using matched scenario/market periods and reports at least:

- net performance;
- drawdown/tail risk;
- calibration;
- turnover;
- slippage/execution sensitivity;
- regime distribution;
- sample size/effective independent events;
- resource cost;
- failure-mode deltas;
- statistical uncertainty;
- adverse scenario results.

No single metric is sufficient for promotion.

### CSA

`MetaLearner` may itself be CSA-eligible only under separately bounded component rules. Its CSA cannot approve its own candidate.

## 15. T-LSA-13 — Trading Resource Awareness & Evaluation

### Components

- `T13.ResourceUsageObserver`
- `T13.WorkloadRegistry`
- `T13.MinimumSafeEvaluator`
- `T13.ReclaimabilityEvaluator`
- `T13.DegradationPlanner`
- `T13.ResourceDemandForecaster`
- `T13.FSARMReportPublisher`

### Ownership

T-LSA-13 owns Trading-side knowledge of:

- current workload demand;
- current effective resource allocation projection;
- minimum-safe requirement by obligation;
- reclaimable/deferrable workload;
- consequence of shedding;
- pressure impact on Trading business quality;
- additional need evidence.

It does **not** own FSATS-wide redistribution or Foundation resource grants.

```text
T_LSA13 != FSARM
```

### Degradation order

Subject to active obligation evidence, default Trading shedding preference is:

1. research/evolution experiments;
2. historical analytics refresh;
3. broad opportunity discovery breadth;
4. nonessential feature refresh frequency;
5. lower-priority dormant/watch strategy evaluation;
6. new-risk opportunity throughput;
7. never silently shed active order/position reconciliation, capital truth, Guardian compliance or minimum-safe open-position monitoring.

The exact active obligation can reorder only within policy-permitted classes; it cannot self-create Foundation technical criticality.

### Report

Publishes immutable `TradingResourceDemandReport` to logical FSARM contract with current allocation reference, measured usage, minimum-safe, desired, reclaimable quantities, degradation plan, consequences and evidence.

### CSA

Resource-demand forecast model may be eligible. Hard minimum/protected obligation rules remain deterministic.

## 16. Trading MSA

`MSA-TRD` maintains an evidence-based Application-wide view by consuming bounded Self-Knowledge projections from all 13 LSAs.

It may:

- assess Trading business quality/readiness;
- reconcile cross-LSA conflicts;
- challenge proposals/evidence;
- coordinate Application-origin self-development review;
- recommend restrictions/improvements.

It may not bypass LSA ownership, directly mutate authoritative LSA state, grant its own new authority, deploy candidates, replace Guardian or act as FSA.

Two independent Monitor AI perspectives observe MSA-TRD under file 18 rules.

## 17. Synchronous Dependency Spine

Allowed high-consequence synchronous chain:

```text
T01/T02/T03 -> T04/T05 -> T06 -> T07 -> T08 -> T09
```

T10/T11/T12/T13 consume events/projections primarily asynchronously and shall not create a synchronous command cycle back into the order path.

T07 may query T08 current snapshot through a read port; T08 reservation creation validates a pinned T07 decision but does not call T07 recursively inside the same lock.

## 18. Trading Fail-Closed Global Rules

New risk SHALL be denied when materially unknown:

- environment authority;
- account state;
- market/instrument identity;
- operational data quality/freshness;
- strategy version/applicability;
- Risk policy or result;
- capital availability/reservation;
- Guardian restriction state;
- broker capability;
- Foundation route/security authority.

Risk-reducing cancel/exit/reconcile may use dedicated degraded paths when safer than inaction and explicitly authorized.

## 19. Trading Verification Families

The Trading verifier SHALL cover at least:

1. 13-LSA topology and one MSA;
2. no operational provider egress;
3. no strategy direct execution;
4. no Risk/Guardian collapse;
5. exact universe hard gates/ranking determinism;
6. feature leakage prevention;
7. strategy version/input pinning;
8. conflict/no-trade behavior;
9. risk limit ordering and conservative size intersection;
10. atomic/idempotent capital reservation;
11. ambiguous broker submission reconciliation before retry;
12. position changes only from reconciled execution evidence;
13. outcome vs decision-quality separation;
14. experiment non-promotion authority;
15. T-LSA-13 != FSARM;
16. replay/simulation inputs cannot become operational authority;
17. deterministic rerun from identical snapshots/config/versions;
18. protected degradation behavior under resource pressure;
19. cross-LSA project/dependency constraints;
20. MSA no direct mutation/self-deploy authority.
