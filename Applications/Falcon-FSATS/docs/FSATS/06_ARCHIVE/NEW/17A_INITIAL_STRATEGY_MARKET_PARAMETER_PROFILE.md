# FSATS SIA — Initial Strategy / Market Parameter Profile v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE / ALGORITHM-COMPLETENESS REMEDIATION`
**Purpose:** supply exact initial numeric values for profile parameters referenced by file 17 so implementation does not invent them.

## 1. Important Status

These values are **initial code/test baselines**, not evidence of profitability and not Paper/Live authority.

They are deliberately versioned. Any change that affects strategy behavior creates a new Strategy/Market parameter profile version and must be revalidated.

```text
PARAMETER_PROFILE_ID = FSATS-STRAT-MKT-PARAM-v1.0
ACTIVE_FOR_DESIGN_FIXTURES = YES
PRODUCTION_AUTHORITY = NO
```

## 2. Common Market Volatility Envelope

Measured as `ATR14 / Close` on the strategy primary interval.

| Market | MinTradableVol | MaxTradableVol |
|---|---:|---:|
| US Equities primary intraday | 0.0005 | 0.0500 |
| Crypto Spot primary intraday | 0.0008 | 0.0800 |

Values outside the envelope make strategies that reference this common envelope NOT_APPLICABLE. A strategy with its own stricter rule uses the stricter intersection.

## 3. Strategy Spread Ceilings

Maximum `SpreadBps` at the decision snapshot for new risk:

| Strategy | US Equities Regular | US Pre/After if later separately enabled | Crypto Spot |
|---|---:|---:|---:|
| CLS-001 | 40 | 75 | 35 |
| CLS-002 | 35 | 65 | 30 |
| CLS-003 | 40 | 75 | 35 |
| CLS-004 | 30 | 60 | 25 |
| CLS-005 | 40 | 75 | 35 |
| CLS-006 | 50 | not enabled v1 | 40 |
| HNT-001 | 40 | 70 | 35 |
| HNT-002 | 25 | not enabled v1 | 30 |
| HNT-003 | 60 during first 30m regular | not applicable | not applicable |
| HNT-004 | 35 | 65 | 35 |
| HNT-005 refill state | 40 | 70 | 40 |
| HNT-006 | 35 | not enabled v1 | 35 |
| HNT-007 | not applicable | not applicable | 35 |
| HNT-008 | 50 | 80 if later enabled | 40 |

If spread is unavailable/invalid, no new-risk proposal.

## 4. CLS-004 Mean-Reversion Trend Separation

```text
US Equities MeanReversionMaxTrendSeparation = 0.0050   // 0.50%
Crypto Spot MeanReversionMaxTrendSeparation = 0.0080   // 0.80%
```

This is `abs(EMA20/EMA50 - 1)` on the primary interval.

## 5. CLS-006 Relative-Strength Volatility Floor

Used in `RiskAdjusted = Raw / max(Vol20, VolFloor)`:

```text
US Equities daily VolFloor = 0.0050
Crypto 24h-equivalent VolFloor = 0.0080
```

`Vol20` is standard deviation of one-period returns, decimal fraction.

### Relationship profile for HNT-006

Initial accepted algorithmic eligibility values:

```text
AlignedLookback = 120 observations minimum
EntryCorrelationMin = 0.80
HoldRelationshipCorrelationMin = 0.65
ResidualHalfLifeMin = 5 primary observations
ResidualHalfLifeMax = 60 primary observations
EntryZ = -2.0
ExitZ = -0.5
HardInvalidationZ = -3.5
```

Relationship profiles are trained/fitted only on historical data available before the decision boundary.

## 6. HNT-002 Ignition Latency / Spread Profile

```text
US Equities Regular MaxSpreadBps = 25
Crypto Spot MaxSpreadBps = 30
US MaxDecisionDataAge = 2 seconds
Crypto MaxDecisionDataAge = 5 seconds
US MaxProviderToTradingObservedP95 = 750 ms for this strategy
Crypto MaxProviderToTradingObservedP95 = 1500 ms for this strategy
```

If the certified data path cannot measure/satisfy the required age/latency evidence, HNT-002 is not eligible. These are local business eligibility thresholds, not Foundation QoS guarantees.

## 7. HNT-003 Opening Profile

```text
US Regular FirstWindow = 5 minutes
EntryWindowMax = first 30 regular-session minutes
OpeningMaxSpreadBps = 60
GapATRMin = 1.0
OpeningVolumeRatioMin = 1.5
VWAPRetestTolerance = 0.15 * ATR_5m
```

No extended-hours execution in v1 HNT-003.

## 8. HNT-004 Flow-Score Normalization

### LargeTradeRatio subscore

```text
ratio <= 0.10 -> 0
ratio >= 0.40 -> 10000
between -> linear
```

### DirectionalBlockScore subscore

Input already `0..1`; score = input*10000.
Long-direction hard gate = `>= 0.65`.

### AbsorptionScore

As defined in file 17, `0..1`; score=input*10000.

### ParticipationZ subscore

```text
z <= 0 -> 0
z >= 4 -> 10000
between -> linear
```

### BookPressureScore

When available, canonical definition:

```text
BidDepth = summed certified executable/displayed bid quantity within 10 bps of mid
AskDepth = summed certified executable/displayed ask quantity within 10 bps of mid
BookPressure = BidDepth / max(BidDepth + AskDepth, epsilon)
score = clamp((BookPressure - 0.50) / 0.30, 0, 1) * 10000
```

A provider without semantically compatible depth cannot populate this feature.

Candidate FlowScore threshold = `7000`.

## 9. HNT-005 Quote Update Stress Threshold

Vacuum alternative when depth is unavailable:

```text
US Regular QuoteUpdateGapStress = max(5 seconds, 5 * rolling median quote-update gap over prior 60 primary windows)
Crypto QuoteUpdateGapStress = max(10 seconds, 5 * rolling median quote-update gap over prior 60 primary windows)
```

Provider/data profile must support meaningful quote-update timestamps. If not, HNT-005 without depth is ineligible.

## 10. Execution Quality / INT-002 Parameters

### Maximum participation for quality scoring

`ExpectedIntervalDollarVolume` uses the strategy primary interval.

```text
US Equities MaxParticipation = 0.02    // 2%
Crypto Spot MaxParticipation = 0.015   // 1.5%
```

These are execution-quality model normalization values, not automatic Risk limits. T-LSA-07 may impose stricter capacity.

### Latency normalization

Default for non-HNT002 strategies:

```text
US Equities MaxAllowedObservedP95Latency = 1500 ms
Crypto Spot MaxAllowedObservedP95Latency = 2500 ms
```

If the strategy-specific profile is stricter, use the stricter value.

## 11. Expected Cost / Edge Rule

Global initial new-risk rule:

```text
ExpectedCostR <= 0.35 * ExpectedGrossRewardR
AND NetEdgeR > 0
```

For HNT-002 Momentum Ignition:

```text
ExpectedCostR <= 0.25 * ExpectedGrossRewardR
```

For CLS-004 Mean Reversion:

```text
ExpectedCostR <= 0.30 * ExpectedGrossRewardR
```

If no calibrated probability distribution exists to compute credible ExpectedGrossRewardR/ExpectedLossR, strategy cannot be ACTIVE. Validation/EXPERIMENTAL state may still produce non-authoritative test evaluations.

## 12. T-LSA-06 Applicability Threshold

INT-004 default strategy soft-fitness minimum:

```text
StrategyApplicabilityMin = 6000 / 10000
```

Strategies may define stricter versioned minima. No strategy may set lower than 5000 in v1 without a new reviewed parameter profile.

## 13. Calibration Status Rules

From INT-005:

```text
TrustedBinMinTrials = 30
TrustedAggregateMinEffectiveSamples = 100
CalibrationErrorRestrictionThreshold = 0.15
```

Active-strategy behavior when calibration evidence becomes insufficient:

```text
if effective samples >=100 but current matching-regime bin <30:
  use specified parent-blended calibration

if total effective strategy/market samples <100:
  ACTIVE new-risk state is not eligible; state must be WATCH/VALIDATION/RESTRICTED according to governance
```

No default assumed success probability is permitted for ACTIVE execution.

## 14. INT-006 Slippage Parameters

```text
US Equities k_market = 0.50
Crypto Spot k_market = 0.75
P90 fallback multiplier = 2.0 * ExpectedSlippage when empirical residual sample insufficient
EmpiricalResidualMinSamples = 100
```

`epsilon` for denominator protection is the smallest positive market-profile notional unit, never an arbitrary floating constant.

## 15. HNT-008 Catalyst Profile

```text
IntradayEventMaxAge = 15 minutes
NormalizedSignificanceMin = 7000
ReactionReturnMin = 0.75 * ATR14 / Price
VolumeZMin = 1.50
ProposalTTL = 3 primary bars
```

A normalized sentiment field is evidence only. Long direction still requires price/volume reaction confirmation.

## 16. Common Primary Interval Table

| Strategy | US Equities | Crypto Spot |
|---|---|---|
| CLS-001 | 5m + 15m confirm | 15m + 1h confirm |
| CLS-002 | 5m | 15m |
| CLS-003 | 5m | 15m |
| CLS-004 | 5m | 15m |
| CLS-005 | 5m | 15m |
| CLS-006 | 1 regular-session day | 24h rolling period |
| HNT-001 | 5m | 15m |
| HNT-002 | 1m | 5m |
| HNT-003 | 5m | N/A |
| HNT-004 | 5m trade/quote aggregation | 15m trade/quote aggregation |
| HNT-005 | 1m quote/depth windows | 5m quote/depth windows |
| HNT-006 | 15m aligned | 15m aligned |
| HNT-007 | N/A | 15m |
| HNT-008 | 5m | 15m |

A provider path must supply compatible granularity/quality before strategy eligibility.

## 17. Feature Freshness

At decision time:

```text
US 1m strategy data max age = 2 seconds
US 5m/15m strategy quote/current context max age = 5 seconds
Crypto 5m strategy current context max age = 5 seconds
Crypto 15m/1h strategy current context max age = 10 seconds
Daily/multi-session strategy reference data max age = current market profile/session boundary
```

Completed historical bars may be older by definition; these limits apply to the current decision/execution context needed to act.

## 18. Simulation Generator Baseline Profile

Initial INT-010 Markov transition matrix, row -> next-state probability, exact sum 1.00:

| From \ To | RANGE_LOW_VOL | TREND_NORMAL | HIGH_VOL | LIQUIDITY_STRESS | CRISIS_DISLOCATION |
|---|---:|---:|---:|---:|---:|
| RANGE_LOW_VOL | 0.82 | 0.12 | 0.04 | 0.015 | 0.005 |
| TREND_NORMAL | 0.10 | 0.78 | 0.08 | 0.03 | 0.01 |
| HIGH_VOL | 0.05 | 0.20 | 0.60 | 0.10 | 0.05 |
| LIQUIDITY_STRESS | 0.03 | 0.07 | 0.20 | 0.55 | 0.15 |
| CRISIS_DISLOCATION | 0.02 | 0.08 | 0.20 | 0.25 | 0.45 |

Initial Student-t degrees of freedom:

```text
RANGE_LOW_VOL = 12
TREND_NORMAL = 10
HIGH_VOL = 6
LIQUIDITY_STRESS = 5
CRISIS_DISLOCATION = 3
```

State-specific mean/scale/liquidity parameters are **dataset-calibrated inputs** to the scenario profile, not coding-worker choices. A run with missing calibrated parameters is invalid. The matrix above is a deterministic initial test/default scenario profile only and may not be presented as estimated market truth.

## 19. Fidelity Calibration Optimizer Defaults

```text
MaxCoordinateDescentPasses = 50
MinimumRelativeImprovement = 0.0001
InitialStepFractionOfParameterRange = 0.10
MinimumStepFractionOfParameterRange = 0.001
```

Every calibratable parameter declares explicit min/max range. Missing range => parameter not calibratable.

## 20. Numeric Precision

All thresholds/weights are represented as exact decimal or integer basis/score units. Implementations SHALL NOT compare using approximate binary floating values where threshold crossing changes behavior.

Examples:

```text
0.0050 stored as decimal
7000/10000 score stored integer
40 spread bps stored integer BasisPoints
```

## 21. Missing/Unknown Parameter Rule

If any parameter required by a StrategyVersion/ModelVersion is missing, outside allowed range or belongs to a different profile version:

```text
STRATEGY/MODEL = NOT_APPLICABLE / CONFIG_INCOMPATIBLE
```

No fallback magic number is allowed outside this profile or a later exact successor.

## 22. Verification

Golden fixtures SHALL assert every parameter above is loaded from exact `FSATS-STRAT-MKT-PARAM-v1.0` identity/digest and that one-value mutation changes the expected strategy/config digest and invalidates stale validation evidence.
