# FSATS Specialized Implementation Architecture — Strategy and Intelligence Exact Specifications

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE / INITIAL ALGORITHM BASELINE`
**Trading Authority:** `NONE BY LISTING`

## 1. Purpose

Replace vague strategy descriptions with deterministic, versioned initial algorithms that can be implemented, backtested and challenged. These are **candidate initial algorithms**, not claims of profitability and not Paper/Live authority.

A later improved algorithm is a new StrategyVersion/ModelVersion and must preserve history and pass the governed validation lifecycle.

## 2. Shared Definitions

For bar interval `i`, all rolling calculations exclude future data and use completed bars unless the strategy explicitly declares intrabar behavior.

### 2.1 Returns

```text
r_t(n) = Close_t / Close_(t-n) - 1
```

### 2.2 True Range / ATR

```text
TR_t = max(
  High_t - Low_t,
  abs(High_t - Close_(t-1)),
  abs(Low_t - Close_(t-1))
)
ATR_n = WilderEMA(TR, n)
```

Default `ATR14`.

### 2.3 EMA

Standard exponential moving average with:

```text
alpha = 2 / (n + 1)
EMA_t = alpha*x_t + (1-alpha)*EMA_(t-1)
```

Warmup minimum: `3*n` observations before eligible use.

### 2.4 RSI14

Wilder 14-period RSI with minimum 42-bar warmup.

### 2.5 ADX14

Standard Wilder ADX(14), minimum 42-bar warmup.

### 2.6 Volume Z-score

```text
VolumeZ_t(n) = (Volume_t - mean(Volume_(t-n..t-1))) / sample_stddev(...)
```

Current bar is excluded from baseline window. If standard deviation is zero/insufficient -> UNKNOWN.

### 2.7 Bollinger Z

```text
Mean20 = SMA(Close,20)
Std20  = sample_stddev(Close,20)
Z20 = (Close - Mean20) / Std20
```

### 2.8 Spread BPS

```text
Mid = (BestBid + BestAsk)/2
SpreadBps = 10000 * (BestAsk-BestBid)/Mid
```

If quote is crossed/locked/incomplete according to product policy, spread state is invalid/unknown.

### 2.9 R multiple

`1R` = absolute entry-to-initial-invalidation price distance multiplied by executable quantity, before fees. Targets expressed as R do not override execution collars or market structure constraints.

## 3. Shared Hard Strategy Gates

Every strategy evaluation first requires:

1. exact ACTIVE/WATCH/RESTRICTED permitted StrategyVersion;
2. market profile supports strategy;
3. instrument in current qualified UniverseSnapshot;
4. required Data Products = VALID or explicitly permitted DEGRADED;
5. required features warm/current;
6. account/environment authority/readiness;
7. no Guardian block;
8. execution-cost estimate available;
9. spread/liquidity within strategy-specific ceiling;
10. proposal can expire before source evidence becomes stale.

Failure -> `NOT_APPLICABLE` or `INSUFFICIENT_EVIDENCE`, never a weak signal.

Initial profile permits LONG-biased risk only because initial exposure model disables shorting. A negative signal may emit NO_TRADE/EXIT evidence, not a new short position.

## 4. CLS-001 — Multi-Horizon Trend Continuation v1.0

### Market profiles

- US Equities: primary 5-minute bars, confirmation 15-minute bars.
- Crypto Spot: primary 15-minute bars, confirmation 1-hour bars.

### Required features

Primary: EMA20, EMA50, ADX14, ATR14, VolumeZ60, SpreadBps.
Confirmation: EMA20, EMA50, ADX14.

### Applicability

All true:

```text
Primary EMA20 > EMA50
Confirmation EMA20 > EMA50
Primary ADX14 >= 20
Confirmation ADX14 >= 18
Close > Primary EMA20
ATR14/Close between market-profile MinTradableVol and MaxTradableVol
VolumeZ60 >= -0.50
SpreadBps <= profile CLS001_MaxSpreadBps
```

### Entry trigger

After applicability, current completed bar closes above the maximum High of previous 3 completed primary bars OR pulls above EMA20 after touching within `0.35*ATR14` of EMA20 on one of previous 3 bars.

### Initial entry reference

`EntryRef = max(current Close, current BestAsk)` at decision snapshot, subject to execution collar.

### Invalidation

```text
StopRef = min(
  lowest Low of last 5 completed primary bars,
  EntryRef - 1.50*ATR14
)
```

If risk distance > `3.0*ATR14` or <= market minimum tick distance -> no proposal.

### Targets

- TP1 hypothesis = Entry + 1.5R
- TP2 hypothesis = Entry + 2.5R
- after +1.5R, strategy hypothesis permits trailing invalidation = max(previous invalidation, highest close since entry - 1.5*ATR14).

Actual position management remains T-LSA-06/07/09 governed.

### Confidence score

Subscores 0..10000:

- trend separation `clamp((EMA20/EMA50-1)/0.02,0,1)` ×10000;
- ADX `clamp((ADX14-20)/20,0,1)` ×10000;
- volume `clamp((VolumeZ60+0.5)/3,0,1)` ×10000;
- confirmation alignment binary 10000 when confirmation conditions pass;
- execution quality from INT-002.

Weighted confidence raw:

```text
30% trend separation
20% ADX
15% volume
20% confirmation
15% execution quality
```

Then calibrated by INT-005.

## 5. CLS-002 — Momentum Breakout v1.0

### Primary interval

US Equities 5m; Crypto 15m.

### Features

ATR14, ADX14, VolumeZ60, DonchianHigh20 excluding current bar, DonchianLow20, SpreadBps.

### Entry applicability/trigger

```text
ADX14 >= 18
VolumeZ60 >= 1.50
Close >= DonchianHigh20 + 0.10*ATR14
Current range (High-Low) <= 3.0*ATR14
Spread within profile ceiling
```

Reject if Close is more than `1.25*ATR14` above DonchianHigh20 (`BREAKOUT_CHASE_EXCESS`).

### Stop

`StopRef = max(DonchianHigh20 - 0.25*ATR14, EntryRef - 1.20*ATR14)`.

### Targets

TP1 = 1.2R, TP2 = 2.2R. Expiry for unsubmitted proposal = 2 primary bars.

### False breakout invalidation

Strategy thesis invalid if a completed primary bar closes below DonchianHigh20 - `0.20*ATR14` before TP1.

### Confidence raw

```text
30% breakout distance quality (best at 0.10..0.60 ATR; decays to zero at 1.25 ATR)
25% VolumeZ normalized 1.5..4.0
20% ADX normalized 18..35
15% execution quality
10% regime TRENDING/TRANSITION compatibility
```

## 6. CLS-003 — Pullback Continuation v1.0

### Applicability

```text
EMA20 > EMA50
ADX14 >= 18
at least one of last 4 lows <= EMA20 + 0.35*ATR14
no last-4 close < EMA50 - 0.25*ATR14
RSI14 between 40 and 65 before trigger
```

### Trigger

Current completed bar:

```text
Close > EMA20
Close > previous bar High
VolumeZ60 >= -0.50
```

### Stop

`min(lowest Low last 4, EMA50 - 0.25*ATR14)`; reject risk distance >2.5 ATR.

### Target

TP1=1.5R, TP2=2.5R; thesis expires after 4 primary bars if no entry.

## 7. CLS-004 — Mean Reversion v1.0

### Applicability

Only range-compatible regime:

```text
ADX14 < 18
abs(EMA20/EMA50 - 1) <= profile MeanReversionMaxTrendSeparation
Z20 <= -2.0
RSI14 <= 32
Spread valid
No active market halt/liquidity stress
```

### Trigger

Do not enter on the first extreme bar. Require next completed bar to close:

```text
Close > prior Close
AND Z20 > prior Z20
AND Close >= lower Bollinger band (Mean20 - 2*Std20)
```

### Stop

`min(trigger bar Low - 0.50*ATR14, EntryRef - 1.25*ATR14)`.

### Target

Primary target = Mean20. If Mean20 provides <1.0R expected reward after costs -> no trade. Hard thesis invalidation if ADX rises >=25 before entry.

## 8. CLS-005 — Volatility Compression / Expansion v1.0

### Features

BollingerWidth20 = `(Upper20-Lower20)/Mean20`.
Percentile rank of width over previous 120 completed bars.
ATR14, VolumeZ60, DonchianHigh20.

### Compression qualification

At least 5 of last 8 bars have BollingerWidth percentile <=20th percentile and ADX14 <=22.

### Expansion trigger

```text
Close > DonchianHigh20(ex-current)
VolumeZ60 >= 1.00
Current TrueRange >= 1.20*ATR14(previous)
BollingerWidth current > median width of prior 8 compression bars * 1.15
```

Reject chase >1.25 ATR beyond breakout reference.

### Stop/targets

Stop = breakout reference - 1.2 ATR.
TP1=1.5R; TP2=2.5R.

## 9. CLS-006 — Relative Strength / Weakness Rotation v1.0

### Horizon

Multi-session; recompute after regular-session close for US equities and every 24h rolling UTC boundary for crypto.

### Eligible universe

Only current qualified universe members with minimum 40 daily/24h-equivalent completed observations.

### Strength score

```text
R5  = 5-period return
R20 = 20-period return
R40 = 40-period return
Vol20 = stddev daily returns 20
Raw = 0.25*R5 + 0.45*R20 + 0.30*R40
RiskAdjusted = Raw / max(Vol20, profile VolFloor)
```

Cross-sectional percentile rank `RSPercentile` among compatible universe.

### Entry candidate

```text
RSPercentile >= 80
R20 > 0
Close > EMA20_daily
Liquidity hard gates pass
Portfolio correlation/concentration gate remains T-LSA-07 responsibility
```

Exit/rotation thesis when RSPercentile <55 or Close < EMA20_daily for 2 consecutive completed periods.

No short leg in initial profile.

## 10. HNT-001 — Unusual Volume / Participation Surge v1.0

Primary interval: US 5m, Crypto 15m.

Trigger candidate:

```text
VolumeZ60 >= 3.0
TrueRange >= 1.0*ATR14
Close > Open
Close >= previous Close + 0.35*ATR14
Spread within profile ceiling
```

Confirmation requires next completed bar not close below midpoint of trigger bar and cumulative next-bar volume >=0.50 of trigger volume.

Entry after confirmation. Stop below trigger low -0.25 ATR. TP1=1.3R, TP2=2.0R. Proposal TTL 2 bars.

Volume alone never qualifies.

## 11. HNT-002 — Momentum Ignition v1.0

Primary interval: 1m US only when certified data quality/latency supports; 5m crypto.

Features: ATR14, VolumeZ60, 3-bar return, 3-bar range, spread.

Trigger:

```text
3BarReturn > 1.20 * (ATR14/Close)
VolumeZ60 >= 2.0
sum(3 bar ranges) >= 1.75*ATR14
last Close in top 20% of last bar range
Spread <= strict ignition ceiling
```

Reject when current Close > first-trigger reference +1.0 ATR.

Stop = trigger sequence low or Entry-1ATR, whichever produces smaller risk distance while remaining below entry. Target=1.8R. TTL=2 primary bars.

## 12. HNT-003 — Gap / Session Transition Hunter v1.0

US Equities only.

### Gap definition

```text
GapPct = RegularOpen / PriorRegularClose - 1
GapATR = (RegularOpen - PriorRegularClose) / PriorDailyATR14
```

Long candidate:

```text
GapATR >= 1.0
GapPct > 0
first completed 5m bar Close >= VWAP_5m
first 5m Volume / median first-5m Volume of prior 20 sessions >= 1.5
first 5m low > PriorRegularClose
Spread within opening ceiling
```

Entry after first completed 5m bar or a later retest of VWAP within first 30 regular-session minutes.

Stop below min(first5m low, VWAP-0.5ATR_5m). Target=2R; invalidate if a 5m close falls below PriorRegularClose before entry.

Pre-market gap is evidence but regular-session execution uses the exact session profile unless extended-hours strategy version is separately admitted.

## 13. HNT-004 — Large-Flow Signature Hunter v1.0

Requires trade stream plus quote; order-book depth adds evidence if certified but is not mandatory for v1.

### Features over rolling 20 primary intervals

- `LargeTradeRatio`: volume in trades >= 95th percentile of trade size over trailing 20 sessions/rolling equivalent divided by total volume;
- `DirectionalBlockScore`: signed large-trade volume / total large-trade volume, clipped 0..1 for buy direction using quote/trade classification;
- `AbsorptionScore`: positive large buy flow with price downside contained, defined `1 - min(1, abs(min adverse move)/ATR)`;
- `ParticipationZ`: VolumeZ60 normalized;
- optional `BookPressureScore` if compatible order book available.

### Flow score

Without book:

```text
30% LargeTradeRatio normalized to profile bounds
30% DirectionalBlockScore
20% AbsorptionScore
20% ParticipationZ
```

With book, use `25/25/20/15/15` including BookPressure.

Candidate threshold = 7000/10000.

Direction long only when DirectionalBlockScore >=0.65 and price closes above rolling VWAP.

No hidden participant identity claim is emitted.

Stop=1.25 ATR; target=2R; TTL=3 bars.

## 14. HNT-005 — Liquidity Vacuum / Refill Hunter v1.0

Requires quote; depth if available.

Define trailing baseline over 60 completed primary bars:

```text
MedianSpread
MedianTopDepth (if available)
```

Vacuum detected when:

```text
Spread >= 3*MedianSpread
AND (TopDepth <= 0.35*MedianTopDepth when depth exists OR QuoteUpdateGap >= profile stress threshold)
```

No entry during vacuum.

Refill confirmation requires 2 consecutive completed observation windows:

```text
Spread <= 1.5*MedianSpread
Depth >=0.70*MedianTopDepth if available
DataQuality VALID
Close > vacuum low +0.75*ATR14
```

Long proposal only after refill, stop below vacuum low -0.25ATR, target 1.8R. If quality remains DEGRADED/UNKNOWN -> no proposal.

## 15. HNT-006 — Cross-Instrument / Cross-Market Dislocation Hunter v1.0

Initial implementation is **relative-value observation with one-leg long candidate only**, not market-neutral arbitrage.

Pair relationship must have an active validated `RelationshipProfile`:

```text
InstrumentA/B
Market/Profile versions
Lookback >= 120 aligned observations
RollingReturnCorrelation >= 0.80
Regression beta/intercept version
Residual half-life estimate within allowed range
```

Residual:

```text
Residual_t = log(P_A) - (alpha + beta*log(P_B))
Z = (Residual - mean120)/std120
```

Long A candidate when `Z <= -2.0`, relationship remains valid, A hard gates pass, B data quality is valid, and no structural/corporate-action break exists.

Exit thesis at Z >= -0.5. Invalidate at Z <= -3.5 or correlation <0.65 on the current validation window.

No short B is created in initial 1:1 long-only profile.

## 16. HNT-007 — Crypto Continuous-Regime Transition Hunter v1.0

Crypto Spot only, 15m primary.

Features:

- realized volatility over 16 bars (4h) and 96 bars (24h);
- percentile of 4h vol vs trailing 30d 4h samples;
- VolumeZ96;
- EMA20/EMA50;
- ADX14.

Long transition trigger:

```text
VolPercentile_4h rises from <=40 to >=70 within last 4 bars
VolumeZ96 >=1.5
EMA20 > EMA50
ADX14 >=20
Close breaks previous 16-bar high
```

Stop=1.5 ATR14; TP1=1.5R, TP2=2.5R. TTL=3 bars.

No dependence on exchange opening time.

## 17. HNT-008 — Catalyst / Event Reaction Hunter v1.0

Consumes only normalized FSAPMA `NEWS_EVENT_NORMALIZED`/official event Data Product.

Required event fields include event identity/category, affected instruments, source/provenance, published/effective time, normalized significance, confidence and quality.

### Candidate event gate

```text
QualityState = VALID
EventAge <= 15 minutes for intraday reaction profile
NormalizedSignificance >= 7000
AffectedInstrument exact
No unresolved conflicting correction
```

The strategy does **not** infer trade direction solely from text sentiment.

Long reaction confirmation after event:

```text
price return over first completed reaction window >= 0.75*ATR14/Price
VolumeZ60 >=1.5
Close > pre-event 20-bar high OR above event-window VWAP
spread/liquidity valid
```

Entry only after market reaction confirms. Stop below event reaction low -0.25ATR; target 2R; TTL 3 primary bars.

Raw external text never reaches this strategy directly.

## 18. Strategy Normalized Net Edge

For every applicable evaluation T-LSA-06 computes:

```text
ExpectedGrossRewardR = probability-weighted target R
ExpectedLossR        = probability-weighted invalidation loss, minimum 1.0 baseline risk unit
ExpectedCostR        = INT-006 expected fees+spread+slippage / initial R cash risk
NetEdgeR = ExpectedGrossRewardR - ExpectedLossR - ExpectedCostR
```

Strategy cannot propose new risk when calibrated `NetEdgeR <= 0` or expected cost exceeds profile maximum share of expected gross edge.

Probability estimates come from versioned calibration evidence; absent calibration uses conservative defaults defined by strategy validation state and may block activation rather than fabricate edge.

## 19. Conflict / Ensemble Resolution v1.0

For all evaluations on one decision key:

### 19.1 Normalize score

```text
EvalScore = clamp(NetEdgeNormalized,0,10000)
          * CalibratedConfidence/10000
          * RegimeFitness/10000
          * ExecutionQuality/10000
```

Implemented with scaled integer/decimal arithmetic to avoid floating drift.

### 19.2 Correlation cluster

Strategies whose recent signal/outcome return correlation >=0.75 under current validation profile are placed in the same evidence cluster. Cluster contribution is capped at the maximum individual score + 25% of remaining member scores to avoid counting near-duplicate evidence as independent votes.

### 19.3 Direction

Initial long-only profile:

- positive cluster contributes LONG support;
- NO_TRADE/invalid contributes none;
- explicit exit/negative evidence subtracts from long support.

If normalized opposing/exit evidence is within `1000` points (10 percentage points) of long support after cluster adjustment -> `NO_TRADE_CONFLICT`.

### 19.4 Target distribution

Same-direction target hypotheses are not averaged naively. T06 forms weighted empirical target R quantiles. Executable proposal uses:

```text
ConservativeTargetR = weighted 35th percentile of positive target hypotheses
```

If ConservativeTargetR < 1.0 after cost -> no proposal.

## 20. INT-001 — Regime Classifier v1.0 Baseline

Initial implementation is deterministic, replaceable by a later validated model.

Inputs: ADX14, EMA20/50 separation, ATR percentile 120, BollingerWidth percentile120, Spread percentile, data quality.

Rules evaluated in order:

1. LIQUIDITY_STRESS if SpreadPercentile>=95 or data-quality liquidity stress flag.
2. HIGH_VOLATILITY if ATRPercentile>=80.
3. LOW_VOLATILITY if ATRPercentile<=20 and BollingerWidthPercentile<=25.
4. TRENDING if ADX>=22 and abs(EMA20/EMA50-1)>=0.003.
5. RANGING if ADX<=18 and abs(EMA20/EMA50-1)<=0.002.
6. otherwise TRANSITION_UNCERTAIN.

Output probability baseline:

- winning rule 7000;
- adjacent plausible rules share 2000 based on thresholds distance;
- TRANSITION/other receives remaining 1000;
- exact normalization to 10000.

A later ML model is a new ModelVersion and cannot bypass hard liquidity/data gates.

## 21. INT-002 — Liquidity / Execution Quality Estimator v1.0

Inputs normalized to 0..1 badness:

```text
SpreadBad = clamp(SpreadBps / MaxStrategySpreadBps,0,1)
VolBad = clamp(OrderNotional / max(EstimatedIntervalDollarVolume*MaxParticipation, epsilon),0,1)
DepthBad = 1-clamp(AvailableDepthWithinCollar / RequiredQuantity,0,1) when depth exists
LatencyBad = clamp(ObservedRouteP95Latency / MaxAllowedLatency,0,1)
```

Without depth:

```text
Badness = 0.40*SpreadBad + 0.35*VolBad + 0.25*LatencyBad
```

With depth:

```text
Badness = 0.30*SpreadBad + 0.25*VolBad + 0.25*DepthBad + 0.20*LatencyBad
```

`ExecutionQuality = round(10000*(1-Badness))`.

Any hard spread/data/capacity failure returns INELIGIBLE, not a low score.

## 22. INT-003 — Opportunity Ranker v1.0

Uses the T-LSA-02 universe score for broad candidates, then hunter-specific cheap discovery score:

```text
30% OpportunitySignalStrength
20% Liquidity
15% DataQuality
15% ExecutionQuality
10% StrategyDiversityValue
10% ResourceCostEfficiency
```

Top-N expensive confirmations are selected by score subject to resource budget; ties use DataQuality, Liquidity, InstrumentId.

## 23. INT-004 — Strategy Applicability Model v1.0

Hard invalid gates are deterministic. For remaining strategies, soft fitness:

```text
35% Regime compatibility
20% Data quality margin above minimum
20% Execution quality
15% historical calibrated performance in matching regime
10% current drawdown/strategy health
```

Output `0..10000`. Strategy requires profile-specific minimum, default candidate `6000`.

This score cannot resurrect a hard-inapplicable strategy.

## 24. INT-005 — Decision Calibration / Uncertainty v1.0

For each `(StrategyVersion, MarketProfile, RegimeClass)` maintain rolling calibration bins based on historical predicted confidence.

Ten confidence bins: `[0,.1), ... [.9,1]`.

For each bin use Beta-Binomial smoothing:

```text
posterior_success = (successes + 1) / (trials + 2)
```

Minimum trials for trusted bin: 30. Below 30, blend with parent StrategyVersion-market aggregate weighted by `trials/30`; if parent also insufficient, calibration status = INSUFFICIENT and active strategy policy decides fail-closed/size-reduced behavior.

Calibration error = weighted mean absolute difference between predicted bin midpoint and posterior success.

If error >0.15 over >=100 effective samples -> strategy calibration state RESTRICTED review candidate.

## 25. INT-006 — Execution Cost / Slippage Model v1.0

Expected per-unit adverse cost:

```text
HalfSpread = (Ask-Bid)/2
Participation = OrderNotional / max(ExpectedIntervalDollarVolume, epsilon)
ImpactPct = k_market * sqrt(max(Participation,0)) * RealizedVolPct
ExpectedSlippage = max(HalfSpread, Price*ImpactPct)
ExpectedCost = ExpectedSlippage*Quantity + ExpectedFees
```

`k_market` is a versioned calibrated parameter, initial candidate:

- US Equities: 0.50
- Crypto Spot: 0.75

These values MUST be calibrated in FSTSimA/Paper before any production authority. Hard execution collars override the estimate.

P10/P50/P90 bands may be derived from historical residual distribution; absent adequate samples use conservative multiplier P90 = `2*ExpectedSlippage`.

## 26. INT-007 — Provider Reliability Forecast v1.0

Maintain EWMA with alpha=0.10 per provider route for:

- success indicator;
- timeout/error indicator;
- stale/conflicted observation indicator;
- normalized latency;
- quota headroom.

ReliabilityScore:

```text
35% EWMA success
20% (1-error EWMA)
15% (1-quality-failure EWMA)
15% latency fitness
15% quota headroom fitness
```

Forecast does not override hard route eligibility.

## 27. INT-008 — Data Quality Anomaly Model v1.0

Baseline robust anomaly detector per DataProduct/Instrument/Session bucket.

For feature vector values such as spread, return, volume, update gap and source divergence:

```text
RobustZ = 0.6745*(x - median(window)) / MAD(window)
```

Minimum window 60, target 240. Feature anomaly if `abs(RobustZ)>=5`. Multi-feature anomaly state when >=2 independent dimensions trigger or one critical dimension >=8.

Anomaly adds evidence/DEGRADED or investigation candidate per product policy; cannot validate invalid data.

## 28. INT-009 — Guardian Incident Correlation Model v1.0

Baseline model is a weighted evidence suggester, not directive authority.

Signal score:

```text
30% severity
25% source independence
20% temporal clustering
15% cross-domain consistency
10% recurrence/escalation
```

Score >=7000 suggests incident-correlation candidate to deterministic G01 policy. Hard Guardian incident predicates bypass the model suggestion requirement; model cannot create authority.

## 29. INT-010 — FSTSimA Synthetic Scenario Generator v1.0

Baseline generator uses a finite Markov regime chain:

States:

```text
RANGE_LOW_VOL
TREND_NORMAL
HIGH_VOL
LIQUIDITY_STRESS
CRISIS_GAP/DISLOCATION
```

Transition matrix is a versioned scenario profile estimated/calibrated from historical regime transitions, then deliberately stressed by adversarial profiles.

Within state, returns use a Student-t distribution with versioned degrees-of-freedom, mean/scale; liquidity/spread/volume distributions are state-conditioned. Random streams use S-LSA-01 named deterministic seeds.

Generator output is synthetic and never labeled forecast/historical truth.

## 30. INT-011 — FSTSimA Fidelity Calibration v1.0

Calibration target is a weighted normalized error:

```text
20% return distribution distance
15% volatility distribution distance
15% spread/liquidity distance
15% fill/partial-fill distance
15% slippage distribution distance
10% latency/error behavior distance
10% strategy outcome sensitivity distance
```

Initial optimizer: deterministic bounded coordinate descent over declared calibratable parameters:

1. fixed canonical parameter order;
2. evaluate +/- configured step from current value within bounds;
3. accept the candidate with greatest reduction in weighted error, tie-break lower parameter value then canonical parameter ID;
4. reduce step by half when no improvement across full pass;
5. stop at minimum step or max iterations.

Output is a candidate parameter version. S-LSA-08 independently assesses it.

## 31. Strategy CSA Self-Knowledge Minimum

Any strategy granted CSA eligibility observes at minimum:

- exact StrategyVersion/active scope;
- sample count by market/regime;
- calibrated success/edge distribution;
- drawdown/failure patterns;
- execution-cost sensitivity;
- feature contribution/stability evidence;
- known invalid regimes;
- current model/config drift state;
- candidate improvement backlog.

It cannot alter hard gates, market scope, Risk, authority or active bytes.

## 32. Parameter Governance

Every numeric parameter above belongs to a versioned Strategy/Model/Market policy.

Forbidden:

- magic numbers embedded outside the owning versioned policy;
- live tuning without new config/version evidence;
- selecting parameters based only on the same test sample used to report performance;
- silently changing a strategy under the same StrategyVersionId.

## 33. Validation Requirements Per Strategy

Before ACTIVE eligibility, each StrategyVersion must have evidence for:

1. code/formula parity test against this spec;
2. no look-ahead leakage;
3. deterministic replay;
4. market/profile compatibility;
5. sufficient sample across multiple regimes;
6. transaction cost/slippage sensitivity;
7. drawdown/tail scenarios;
8. parameter sensitivity/overfitting challenge;
9. provider/data degradation cases;
10. broker/execution ambiguity cases;
11. correlation with other active strategies;
12. FSTSimA baseline/adversarial tests;
13. Paper/Shadow stage when separately authorized;
14. explicit promotion authority.

No minimum numeric performance threshold is fabricated in this design where it has not yet been Owner-accepted; those promotion thresholds belong to a separately reviewed validation/promotion policy and shall fail closed until defined.

## 34. Verification Families

Strategy/intelligence verifier SHALL test at least:

- exact feature formulas/warmups;
- current bar exclusion where specified;
- no future input access;
- all hard gates;
- entry/stop/target math;
- deterministic confidence/scoring;
- long-only initial profile enforcement;
- conflict/correlation cluster caps;
- no duplicate correlated vote inflation;
- conservative target quantile;
- calibration minimum samples;
- hard limit/model separation;
- intelligence model cannot create authority;
- external Data Product quality failures;
- strategy parameter version immutability;
- synthetic output classification;
- fidelity optimizer deterministic tie-break;
- same inputs/policies -> same outputs.
