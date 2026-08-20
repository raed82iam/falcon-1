# FSATS SIA — Statistical Primitive and Net-Edge Estimator Specification v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `RT-STRAT-001`
**Owner:** APP-TRD / T-LSA-03 through T-LSA-06

## 1. Purpose

Close implementation variability in statistical primitives used by the exact strategy algorithms, particularly HNT-004, HNT-006, cross-sectional ranking and expected-net-edge estimation.

All calculations use only observations available at or before the exact decision boundary.

## 2. Common Sample Rules

Unless a StrategyVersion states otherwise:

- sample elements are ordered by canonical effective time then ObservationId;
- invalid/CONFLICTED/STALE observations are excluded and may invalidate the window when required coverage is not met;
- duplicates are removed by canonical ObservationId/digest rules before statistics;
- no forward fill across a missing market bar unless an exact profile explicitly defines it;
- numeric inputs are exact decimal canonical values; intermediate statistical calculations may use IEEE-754 binary64 under the pinned .NET runtime, with final strategy thresholds converted/compared to exact declared decimals after canonical 1e-12 statistical quantization;
- a statistic with insufficient sample is `UNKNOWN`, not zero.

## 3. Mean

For `n>0`:

```text
mean = sum(x_i) / n
```

Use deterministic input order. Final dimensionless result quantized to 12 decimal places half-to-even before downstream threshold comparison where the strategy does not already operate in exact decimal arithmetic.

## 4. Sample Standard Deviation

For `n>=2`:

```text
m = mean(x)
s2 = sum((x_i-m)^2) / (n-1)
s = sqrt(s2)
```

`n<2` -> UNKNOWN.

This is the meaning of `sample_stddev` in files 17/17A.

## 5. Median

Sort ascending numeric value, stable tie identity irrelevant to result.

- odd n -> middle element;
- even n -> arithmetic mean of the two central values.

No interpolation beyond that rule.

## 6. Percentile / Quantile

Unless a weighted quantile is explicitly required, use **nearest-rank**:

For percentile `p` in `(0,1]` and sorted `n` observations:

```text
rank = ceil(p*n)
value = sorted[rank-1]
```

For `p=0`, return minimum.

This governs:

- trailing 95th-percentile trade size in HNT-004;
- volatility/width percentile classifications where historical percentile thresholding is used;
- other non-weighted strategy percentile references.

## 7. Cross-Sectional Percentile Rank

For an instrument value `x` in a compatible universe of `n>=2` values:

```text
less = count(v < x)
equal = count(v == x)
PercentileRank = (less + 0.5*equal) / n
```

Return score 0..10000 by half-to-even rounding.

Exact ties therefore receive the same mid-rank.

CLS-006 `RSPercentile` uses this rule.

## 8. Pearson Correlation

For aligned pairs `(x_i,y_i)`, `n>=2`:

```text
corr = sum((x_i-mx)*(y_i-my)) /
       sqrt(sum((x_i-mx)^2)*sum((y_i-my)^2))
```

If either variance is zero -> UNKNOWN.

The initial HNT-006 relationship profile requires at least 120 valid aligned pairs; portfolio correlation controls requiring 60 samples use the same formula.

## 9. Time Alignment For HNT-006

Relationship observations are aligned by exact canonical `BarStart/BarEnd/BarIntervalId`.

A pair exists only when both instruments have a VALID compatible completed DP-005 bar for the exact same boundary.

Rules:

- no forward fill;
- no interpolation;
- no nearest-neighbor timestamp matching;
- missing one side => omit that pair;
- after omission, at least 120 aligned pairs remain or relationship = NOT_ELIGIBLE;
- corporate-action adjustment profiles must match their validated relationship profile.

## 10. OLS Relationship Fit For HNT-006

Using aligned pairs:

```text
x_i = ln(P_B_i)
y_i = ln(P_A_i)
```

with strictly positive prices.

Ordinary least squares with intercept:

```text
beta = sum((x_i-mx)*(y_i-my)) / sum((x_i-mx)^2)
alpha = my - beta*mx
Residual_i = y_i - (alpha + beta*x_i)
```

Zero x variance -> relationship invalid.

The fit window uses the most recent 120 aligned observations ending before/current at the decision boundary as the RelationshipProfile specifies; no later bars.

## 11. Residual Z-Score For HNT-006

For the same fit-window residuals:

```text
ResidualMean = mean(Residual_i)
ResidualStd = sample_stddev(Residual_i)
CurrentResidual = ln(P_A_current) - (alpha + beta*ln(P_B_current))
Z = (CurrentResidual - ResidualMean) / ResidualStd
```

ResidualStd <=0/UNKNOWN -> ineligible.

## 12. Residual Half-Life

Fit AR(1) with intercept on consecutive residual observations:

```text
r_t = c + phi*r_(t-1) + epsilon_t
```

using OLS over the fit window residual sequence.

Valid mean-reversion half-life requires:

```text
0 < phi < 1
```

Then:

```text
HalfLife = -ln(2) / ln(phi)
```

If phi <=0 or >=1, half-life = INVALID and HNT-006 relationship is not eligible under v1.

Initial allowed interval from 17A remains 5..60 primary observations inclusive.

## 13. HNT-004 Trade Direction Classification v1.0

Each DP-004 NEW trade is classified using only the most recent VALID DP-003 quote whose effective time is <= trade effective time.

Quote maximum age for classification:

```text
US Equities = 2 seconds
Crypto Spot = 5 seconds
```

### Step 1 — midpoint test

```text
mid = (BidPrice + AskPrice)/2
if TradePrice > mid -> BUY_INITIATED
if TradePrice < mid -> SELL_INITIATED
```

### Step 2 — tick test for exact midpoint

If TradePrice == mid:

- compare with previous valid NEW trade price for the same instrument;
- greater -> BUY_INITIATED;
- lower -> SELL_INITIATED;
- equal -> carry the most recent nonzero tick-test direction within the current classification session/window.

If no usable prior nonzero direction exists -> UNKNOWN_DIRECTION.

### Step 3 — missing quote

If no valid quote <= trade time within the age limit, use the tick-test rule from Step 2 directly.

If no tick direction exists -> UNKNOWN_DIRECTION.

UNKNOWN_DIRECTION trade volume remains in total participation/volume but is excluded from directional buy/sell numerator/denominator.

Provider-supplied aggressor side is evidence only and does not override this canonical classifier in v1.

## 14. HNT-004 Large-Trade Threshold

Compute the 95th-percentile `TradeQuantity` using nearest-rank over a trailing reference sample excluding the current primary interval.

### US Equities

Reference = valid regular-session NEW trade prints from the previous 20 completed regular trading sessions for the instrument.

Minimum valid trade prints = 1000.

### Crypto Spot

Reference = valid NEW trade prints from the previous 20 completed UTC days `[00:00,24:00)`.

Minimum valid trade prints = 1000.

If fewer -> HNT-004 ineligible.

CANCELED trades are removed from corrected reference reconstruction. CORRECT trades replace the corrected original through immutable lineage.

A current trade is `LargeTrade` when:

```text
TradeQuantity >= P95TradeQuantity
```

## 15. HNT-004 Rolling VWAP

### US Equities

Use current regular-session VWAP from regular-session open through the latest classified/valid trade at or before the decision boundary:

```text
VWAP = sum(TradePrice*TradeQuantity) / sum(TradeQuantity)
```

Pre-market/after-hours trades are excluded from v1 regular-session HNT-004 VWAP.

### Crypto Spot

Use rolling/current UTC-day VWAP from 00:00 UTC through the decision boundary.

If total valid trade quantity =0 or current source window incomplete -> UNKNOWN and no HNT-004 new risk.

## 16. HNT-004 Directional Block Score

Over the current rolling 20 primary intervals:

```text
BuyLargeVolume = sum(quantity of LargeTrade classified BUY_INITIATED)
SellLargeVolume = sum(quantity of LargeTrade classified SELL_INITIATED)
DirectionalKnownLargeVolume = BuyLargeVolume + SellLargeVolume

DirectionalBlockScore = BuyLargeVolume / DirectionalKnownLargeVolume
```

If known directional large volume is zero or known-direction volume is <60% of total LargeTrade volume, score = UNKNOWN and HNT-004 cannot satisfy its long-direction gate.

## 17. HNT-004 LargeTradeRatio

```text
LargeTradeRatio = total LargeTrade quantity / total valid NEW trade quantity
```

over the same current 20-primary-interval evaluation window.

Canceled/corrected source trades are reflected through corrected reconstruction.

## 18. HNT-004 Absorption Adverse Move

For long-flow absorption:

- identify the beginning of the current 20-primary-interval flow evaluation window;
- `FlowStartPrice` = first valid primary-interval close;
- `MinPrice` = minimum valid low in the window;
- `AdverseMove = max(0, FlowStartPrice - MinPrice)`;
- `AbsorptionScore = 1 - min(1, AdverseMove / ATR14_current)`.

ATR unavailable/zero -> score UNKNOWN.

## 19. Weighted Quantile For Strategy Target Resolution

File 17 T-LSA-06 target resolution uses weighted 35th percentile of positive target hypotheses.

For target hypotheses `(R_i, w_i)` where `R_i>0`, `w_i=EvalScore_i>0`:

1. sort by `R_i` ascending; ties by StrategyVersionId ordinal;
2. total weight `W=sum(w_i)`;
3. threshold `T=0.35*W`;
4. choose first sorted `R_i` whose cumulative weight >= T.

No interpolation.

No positive weighted target -> NO_TRADE.

## 20. Exact Historical Outcome Sample For NetEdge

A calibration/outcome sample is one closed `PositionEpisodeId` attributable to exact:

```text
StrategyVersionId
MarketProfileVersion
RegimeClassAtEntry
DecisionPolicyVersion
```

Its gross realized R before fees/slippage is:

```text
GrossRealizedR = GrossTradingPnL / InitialRiskCash
```

where `InitialRiskCash` is the exact risk amount recorded when the position episode began.

If InitialRiskCash <=0/unknown -> sample invalid for NetEdge calibration.

Costs are separately recorded in R:

```text
RealizedCostR = (fees + commissions + adverse execution/slippage attribution) / InitialRiskCash
NetRealizedR = GrossRealizedR - RealizedCostR
```

Partial fills belong to the same PositionEpisode sample.

## 21. Initial Expected Gross Reward / Loss Estimator

For the exact `(StrategyVersion, MarketProfile, RegimeClass)` bucket with `n` valid closed samples:

```text
PositiveGrossSum = sum(max(GrossRealizedR_i,0))
NegativeGrossSum = sum(max(-GrossRealizedR_i,0))
```

Use a conservative two-pseudo-observation prior, each equivalent to a -1R gross loss:

```text
ExpectedGrossRewardR = PositiveGrossSum / (n + 2)
ExpectedLossR = (NegativeGrossSum + 2.0) / (n + 2)
```

This is deliberately conservative under small samples.

### Matching-regime minimum

- if n >=30: use bucket estimator directly;
- if n <30: blend with exact StrategyVersion+Market parent bucket using weight `n/30` as specified by INT-005 calibration philosophy;
- if the parent bucket has <100 valid samples total, ACTIVE new-risk eligibility is unavailable under 17A/07A policy.

Parent expected reward/loss uses the same formulas with its own sample count/sums.

Blend:

```text
w = n/30
ExpectedReward = w*RegimeReward + (1-w)*ParentReward
ExpectedLoss   = w*RegimeLoss   + (1-w)*ParentLoss
```

When n=0, parent only.

## 22. Expected Cost R

Before the current trade:

```text
ExpectedCostR = CurrentExpectedExecutionCostCash / ProposedInitialRiskCash
```

using INT-006 and current proposed quantity/entry/stop/tail risk context.

A proposed quantity change requires recomputation.

## 23. NetEdgeR

```text
NetEdgeR = ExpectedGrossRewardR - ExpectedLossR - ExpectedCostR
```

New risk requires:

- NetEdgeR >0;
- 17A expected-cost share ceiling;
- sufficient calibration/sample state;
- all hard gates.

The current trade itself never enters the historical estimator before its decision.

## 24. Calibrated Confidence Separation

`CalibratedConfidence` from INT-005 remains a separate reliability estimate and multiplies EvalScore.

It SHALL NOT be substituted for ExpectedGrossReward/Loss. A strategy can have high confidence but nonpositive NetEdge after costs and must then produce no new-risk proposal.

## 25. Correlation Cluster Calculation

Strategy recent signal/outcome correlation for T-LSA-06 uses aligned `NetRealizedR` samples or validated signal-return series defined by the current `CorrelationProfile`.

For the initial v1 cluster rule:

- exact StrategyVersion pair;
- same market profile;
- most recent 60 aligned closed position/risk-event observations where available;
- Pearson correlation from Section 8;
- <30 aligned observations => conservatively treat as correlated when strategies share the same school + primary feature/trigger family, otherwise UNKNOWN and do not grant diversification credit.

Cluster threshold remains `>=0.75` positive correlation.

## 26. Numerical Tie/Threshold Rule

All threshold-driving final statistics are canonicalized before comparison:

```text
correlation / Z / ratios / expected R -> 12 decimal places, half-to-even
scores -> integer 0..10000
```

Exact threshold comparison includes equality where strategy text uses `>=`/`<=`.

No library-default percentile/rank/regression method may replace this file.

## 27. Golden Fixtures

Verifier SHALL include independent vectors for:

1. nearest-rank quantiles including ties;
2. cross-sectional mid-rank percentile;
3. sample stddev;
4. Pearson correlation;
5. exact HNT-006 OLS alpha/beta/residual/Z;
6. AR(1) phi/half-life valid/invalid cases;
7. missing-bar alignment behavior;
8. midpoint/tick trade classification;
9. quote-missing fallback/UNKNOWN direction;
10. trade corrections affecting P95/large-flow metrics;
11. US/crypto VWAP boundaries;
12. weighted 35th target quantile;
13. ExpectedGrossReward/Loss prior math at n=0,1,29,30;
14. parent-regime blend;
15. NetEdge cost recomputation on quantity change;
16. correlation insufficient-sample conservative handling;
17. no current/future sample leakage.

## 28. Finding Disposition

```text
RT-STRAT-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
HNT004_CLASSIFICATION = EXACT
HNT004_PERCENTILE/VWAP = EXACT
HNT006_REGRESSION/HALF_LIFE = EXACT
WEIGHTED_TARGET_QUANTILE = EXACT
NET_EDGE_ESTIMATOR = EXACT
STATISTICAL_TIE/RANK RULES = EXACT
```
