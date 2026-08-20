# FSATS SIA — Market Capital Fitness and Dynamic Allocation Specification v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-ALG-002`
**Owner:** APP-TRD / T-LSA-08 with T-LSA-02/06/07 evidence inputs

## 1. Purpose

Make the initial US Equities / Crypto Spot capital-target allocation deterministic while preserving the Owner-directed initial 1:1 target and the principle that capital is never forced into a weak/unfit market.

This is a capital/notional envelope. It does not override Risk/Guardian/strategy eligibility.

## 2. Initial State

At the start of each governed RiskEpoch:

```text
US_EQUITIES_TARGET = 0.50
CRYPTO_SPOT_TARGET = 0.50
UNALLOCATED_CASH_TARGET = 0.00
```

No trade is created merely to fill a target.

## 3. Allocation Epoch

Normal dynamic allocation is recalculated once per UTC Risk day at:

```text
00:05:00 UTC
```

using only input observations whose effective time is <= the allocation boundary.

Additional **hard-state reevaluation** occurs immediately when:

- Guardian blocks/restricts a market;
- required operational Data Products become unavailable/CONFLICTED beyond the market readiness rule;
- broker/account capability removes market tradability;
- Risk hard stop prevents the market from taking new risk.

Hard-state reevaluation may reduce a market target immediately, including to zero. It SHALL NOT use fresh opportunity/performance scoring to increase another market above its previous target between normal daily allocation epochs. Freed capital remains cash until the next normal allocation epoch unless an explicit protective operation uses it.

## 4. Input Snapshot

`MarketCapitalFitnessSnapshot` for each market binds:

```text
MarketId
AllocationEpochId
QualifiedUniverseSnapshotId
TopOpportunitySnapshotIds[]
Strategy/DecisionPolicy versions
DataQualityAggregate
ExecutionQualityAggregate
RecentPerformanceSampleRef
MarketReturnSeriesRef
Guardian/Risk readiness state
Config/Profile versions
EvidenceRefs[]
```

Any component required below that is UNKNOWN makes the market fitness UNKNOWN unless a specified fallback exists.

## 5. Opportunity Density Score — 25%

Use the last 24 hours ending at allocation boundary, with market-session eligibility respected.

An `EligibleOpportunityEvent` is one unique `(InstrumentId, StrategyVersionId, DecisionWindowId)` for which:

- strategy hard applicability passed;
- calibrated NetEdgeR >0;
- EvalScore >=6000;
- no Guardian/Risk hard block existed at that event boundary;
- event was not a duplicate/superseded decision.

For the exact current qualified universe of size `U` and maximum possible normalized decision windows `W` available in the 24h period under each active strategy's primary interval, define:

```text
OpportunityEventRate = UniqueEligibleOpportunityEvents /
                       max(1, U * SumExpectedDecisionWindowsAcrossApplicableStrategyVersions)
```

To avoid tiny-interval strategies dominating, each StrategyVersion contributes at most one opportunity event per instrument per **30-minute canonical opportunity bucket**. Buckets are half-open UTC intervals.

Score:

```text
OpportunityDensityScore = round(10000 * clamp(OpportunityEventRate / 0.05, 0, 1))
```

Thus a 5% normalized opportunity-event density saturates the score.

If qualified universe is empty -> score 0.

## 6. Aggregate Expected Net Edge Score — 20%

At allocation boundary, collect the latest still-valid positive NetEdgeR opportunity for each qualified instrument, at most one per StrategyVersion/Instrument.

Rank by EvalScore and keep top 10 unique InstrumentId opportunities, one best strategy per instrument.

If none -> score 0.

Compute median NetEdgeR using 17B median rule.

```text
EdgeScore = round(10000 * clamp(MedianNetEdgeR / 1.50, 0, 1))
```

`1.50R` median expected net edge saturates the v1 score. Values <=0 score 0.

## 7. Data Quality Score — 20%

For every instrument in the **selected current top-10 qualified universe**, gather the current required core Data Product quality scores at the allocation boundary:

- DP-001 instrument reference;
- DP-002 session state;
- DP-003 current quote when market active;
- latest expected DP-005 completed bar for the market profile;
- any additional product required by the currently top-ranked applicable strategy for that instrument.

Per instrument:

```text
InstrumentDataScore = minimum(required QualityScore)
```

Market score = median InstrumentDataScore.

If selected universe empty ->0.
If any hard-required product state is non-VALID for >50% of selected instruments -> market hard readiness = degraded/unavailable and normal new-risk allocation cannot increase.

## 8. Execution / Liquidity Quality Score — 15%

For the same top-10 set, compute current INT-002 `ExecutionQuality` for a standardized probe size:

```text
ProbeNotional = min(
  1% of current market target capital,
  market profile StandardExecutionProbeNotional
)
```

Initial StandardExecutionProbeNotional:

```text
US Equities = 1,000 USD
Crypto Spot = 500 USD
```

This probe is an estimator input only, not an order.

Market ExecutionScore = median of valid instrument ExecutionQuality scores.

Fewer than 3 valid instrument scores -> ExecutionScore=0 and allocation cannot increase.

## 9. Recent Risk-Adjusted Performance Score — 10%

Use closed PositionEpisode samples attributed to this market during the previous 30 UTC days, maximum most recent 100 episodes.

If fewer than 20 valid samples:

```text
PerformanceScore = 4000
```

(conservative below-neutral, not zero because lack of trades is not proof of failure).

Otherwise using `NetRealizedR`:

```text
MeanR = mean(NetRealizedR)
StdR = sample_stddev(NetRealizedR)
RiskAdjusted = MeanR / max(StdR, 0.50)
PerformanceScore = round( clamp(5000 + 2500*RiskAdjusted, 0, 10000) )
```

Hard strategy/market drawdown restrictions in 07A override the score and may prevent allocation increase.

## 10. Diversification Benefit Score — 10%

Use the previous 30 completed UTC daily market-return observations for the canonical market benchmark/market aggregate profile.

If fewer than 20 aligned observations with the other active market:

```text
DiversificationScore = 4000
```

Otherwise Pearson correlation from 17B:

```text
PositiveCorr = max(0, Corr)
DiversificationScore = round(10000 * (1 - PositiveCorr))
```

Negative correlation saturates at 10000; it never permits gross exposure >100%.

## 11. MarketCapitalFitness

```text
Fitness = round(
  0.25*OpportunityDensityScore
+ 0.20*EdgeScore
+ 0.20*DataQualityScore
+ 0.15*ExecutionScore
+ 0.10*PerformanceScore
+ 0.10*DiversificationScore
)
```

Integer score 0..10000.

Hard market readiness state is separate. A hard-blocked market has `EffectiveFitness=0` and target ceiling 0 regardless of weighted score.

## 12. Raw Two-Market Target

If both markets hard eligible:

```text
Total = Fitness_US + Fitness_Crypto
if Total == 0:
  proposed US = 0
  proposed Crypto = 0
  proposed Cash = 1
else:
  rawUS = Fitness_US / Total
  rawCrypto = Fitness_Crypto / Total
```

Apply normal target envelope:

```text
proposedUS = clamp(rawUS, 0.25, 0.75)
proposedCrypto = 1 - proposedUS
```

Because there are exactly two markets, this yields each within 25..75.

If one market is hard blocked:

```text
blocked target = 0
other target = min(previous/normal proposed target, 0.75)
cash target = 1 - other target
```

No forced 100% transfer.

If both blocked -> 100% cash target.

## 13. Hysteresis

At normal allocation epoch, compare proposed vs current target.

If absolute proposed change for US (and therefore Crypto) is <5 percentage points:

```text
keep current targets
```

Otherwise cap change magnitude per normal daily epoch to:

```text
10 percentage points per market
```

Example current 50/50, proposed 75/25 -> next normal target max 60/40.

Hard-state reductions to 0 are not subject to the 10-point cap. Subsequent recovery increases again follow normal daily cap/hysteresis.

## 14. Cash Is A First-Class Outcome

If target envelopes sum <100% because of hard blocks/ceiling restrictions:

```text
remainder = UNALLOCATED_CASH_TARGET
```

Cash is not a failure to allocate and does not lower Risk gates to search for trades.

## 15. Capital Availability

Per-market allocation target determines a notional capital ceiling:

```text
MarketCapitalCeiling = DeployableCapital * MarketTarget
```

Actual reserved/deployed capital can be less.

T-LSA-08 does not reserve the whole target in advance; reservations occur only for validated proposals.

## 16. Interaction With Risk

Market allocation is one ceiling in T-LSA-07/T-LSA-08 quantity calculation.

It cannot:

- increase per-trade risk percentage;
- increase concurrent risk ceiling;
- override instrument concentration;
- override Guardian restriction;
- create positive NetEdge;
- deploy cash because target exists.

## 17. Deterministic Tie / Missing Rules

- identical Fitness -> proposed 50/50;
- any missing component with no defined fallback -> market Fitness UNKNOWN and allocation cannot increase;
- integer rounding uses half-to-even;
- all input snapshots are exact-version/effective-boundary bound;
- recomputation with same snapshot produces same target.

## 18. Verification Families

Verifier SHALL cover:

1. RiskEpoch starts 50/50;
2. exact 00:05 UTC normal allocation epoch;
3. no future input;
4. opportunity bucket de-duplication;
5. top-10 edge selection one best strategy/instrument;
6. data quality minimum + median;
7. probe notional exact;
8. <20 performance samples fallback 4000;
9. diversification fallback 4000;
10. exact weighted Fitness;
11. 25/75 clamp;
12. <5-point hysteresis no-op;
13. 10-point max daily shift;
14. hard market block can drop target to 0 immediately;
15. freed capital stays cash rather than forced transfer;
16. target does not create reservation/trade;
17. same input snapshot deterministic target.

## 19. Finding Disposition

```text
AC-ALG-002 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
MARKET_CAPITAL_FITNESS_SUBSCORES = EXACT v1
ALLOCATION_EPOCH/HYSTERESIS = EXACT v1
```
