# FSATS SIA — Universe Ranking Exact Formula Specification v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-ALG-001`
**Owner:** APP-TRD / T-LSA-02

## 1. Purpose

Define how T-LSA-02 converts market/reference/data evidence into the exact ranked instrument set. Price zones remain eligibility envelopes; price is not the ranking score.

## 2. Ranking Boundary

Universe refresh runs from a `UniverseRankingSnapshot` containing only observations available at or before its exact boundary.

Initial normal refresh:

- US Equities: once after prior regular-session DP-005 daily-bar finalization and before the next regular session, plus event-driven hard requalification for halt/delisting/corporate-action/tradability changes;
- Crypto Spot: once per UTC day after DP-005 `PT24H_UTC` finalization, plus event-driven hard requalification.

Intraday opportunity strategies scan the selected set; they do not continuously rewrite the core Top-N universe on every tick.

## 3. Preliminary Candidate Set

An instrument enters scoring only when all hard gates pass.

### Common hard gates

- DP-001 Instrument Reference VALID/current;
- exact target MarketId;
- instrument status ACTIVE;
- broker/account capability supports the instrument under current environment;
- required core historical data coverage available;
- no Guardian instrument/market block;
- no unresolved corporate-action identity conflict;
- RiskBaseCurrency/quote policy compatible with 07C.

### US Equities additional v1 gates

```text
Reference price >= 1.00 USD
20-day median regular-session DollarVolume >= 5,000,000 USD
at least 15 valid completed regular-session daily bars in the last 20 trading sessions
median regular-session quoted spread (sampled at qualified observation points) <= 50 bps
```

The Owner-directed price-zone ceiling/floor is then applied:

- Zone C: price <30 USD;
- Zone B tier: price <250 USD and not already selected through Zone C tier logic;
- Zone A tier: price >=250 USD.

### Crypto Spot additional v1 gates

```text
quote asset = USD
20-day median UTC-day DollarVolume >= 2,000,000 USD
at least 15 valid completed PT24H_UTC bars in last 20 UTC days
current broker/provider capability active
```

Crypto does not inherit the US stock price-zone bands. Initial Crypto selected target count is 10 qualified USD-quoted spot instruments where available.

## 4. Daily Dollar Volume

For each completed daily/24h bar:

```text
DollarVolume = BarVolume * BarVWAP
```

If canonical BarVWAP absent, use:

```text
TypicalPrice = (High + Low + Close)/3
DollarVolume = BarVolume * TypicalPrice
```

The fallback identity is recorded. Missing Volume or valid price -> day excluded; hard coverage gate still applies.

`MedianDollarVolume20` = median of valid last-20-period DollarVolume using 17B median rule.

## 5. Liquidity Score — Weight 25%

Within the exact hard-eligible candidate tier/market being ranked:

```text
LiquidityScore = cross-sectional percentile rank of MedianDollarVolume20
```

using 17B mid-rank rule, scaled 0..10000.

This makes liquidity relative among qualified candidates after an absolute minimum floor has already passed.

## 6. Volume Activity Score — Weight 20%

Define:

```text
RecentMedianDV5 = median DollarVolume of most recent 5 valid completed daily/24h periods
BaselineMedianDV20 = MedianDollarVolume20
ActivityRatio = RecentMedianDV5 / BaselineMedianDV20
```

If baseline <=0 -> hard invalid.

Score:

```text
ratio <=0.50 -> 0
ratio >=2.00 -> 10000
between -> 10000*(ratio-0.50)/1.50
```

half-to-even integer rounding.

This measures recent participation expansion/maintenance relative to the instrument's own baseline rather than duplicating raw liquidity rank.

## 7. Spread / Execution Score — Weight 15%

Use two components:

### Median Spread Fitness — 60% of subscore

From valid DP-003 observations sampled at the market profile's ranking observation schedule over the last 5 completed active periods/session-equivalents, compute median SpreadBps.

US v1 ranking observation schedule: 30-minute intervals during regular session.
Crypto v1: 30-minute UTC intervals continuously.

Score against market hard ceiling used for ranking:

```text
US SpreadCeiling = 50 bps
Crypto SpreadCeiling = 40 bps
SpreadFitness = round(10000 * clamp(1 - MedianSpreadBps/SpreadCeiling,0,1))
```

### Execution Fitness — 40%

Use median valid INT-002 ExecutionQuality for the standardized probe notional from 07D over the latest 24h/active-session evidence set.

Fewer than 3 valid execution estimates -> ExecutionFitness=0.

```text
SpreadExecutionScore = round(0.60*SpreadFitness + 0.40*ExecutionFitness)
```

## 8. Opportunity Density Score — Weight 15%

Universe ranking uses a **cheap generic opportunity-activity measure**, not full strategy evaluations, to avoid circular dependence on selected-universe membership.

Use last 20 primary ranking windows:

- US Equities: completed 30-minute regular-session windows;
- Crypto Spot: completed 1-hour UTC windows.

For each window, event=1 if any is true using only valid historical features at that boundary:

```text
abs(WindowReturn) >= 0.75 * ATR14_primary / ReferenceClose
OR VolumeZ60 >= 1.50
OR abs(Z20) >= 1.75
OR TrueRange >= 1.25*ATR14
```

Each window counts at most 1 regardless of how many predicates pass.

```text
OpportunityDensityScore = round(10000 * EventCount / ValidWindowCount)
```

Requires at least 15 valid of the 20 expected windows; otherwise score=UNKNOWN and candidate excluded from Top-N until coverage restored.

## 9. Data Quality Score — Weight 10%

For the ranking boundary, collect:

- DP-001 current QualityScore;
- DP-002 current/latest session profile QualityScore;
- median DP-003 QualityScore over the spread observation set;
- median DP-005 QualityScore over valid 20-day bars.

```text
DataQualityScore = minimum(of the four values)
```

All must be VALID-state products for current v1 ranking eligibility.

## 10. Volatility Tradability Score — Weight 10%

Use current daily/24h:

```text
v = ATR14 / Close
minV,maxV = exact market tradable volatility envelope from 17A
center = (minV + maxV)/2
```

If `v <= minV` or `v >= maxV`: score 0.

If `minV < v <= center`:

```text
score = 10000*(v-minV)/(center-minV)
```

If `center < v < maxV`:

```text
score = 10000*(maxV-v)/(maxV-center)
```

Thus extreme low/high volatility is less tradable, while the middle of the admitted envelope scores highest.

## 11. Diversification Contribution Score — Weight 5%

If there are no current open Trading positions in the same market:

```text
DiversificationScore = 10000
```

Otherwise, for each candidate instrument compute Pearson correlation of the most recent 60 aligned completed daily/24h returns against each currently held instrument with >=40 aligned observations.

Let:

```text
MaxPositiveCorr = maximum(max(0,corr_i))
```

If no held-instrument pair has >=40 aligned observations:

```text
DiversificationScore = 0
```

Otherwise:

```text
DiversificationScore = round(10000*(1-MaxPositiveCorr))
```

This score can prefer diversification but cannot override hard portfolio concentration/Risk.

## 12. Final Universe Score

```text
UniverseScore = round(
  0.25*LiquidityScore
+ 0.20*VolumeActivityScore
+ 0.15*SpreadExecutionScore
+ 0.15*OpportunityDensityScore
+ 0.10*DataQualityScore
+ 0.10*VolatilityTradabilityScore
+ 0.05*DiversificationScore
)
```

All subscores must be known; unknown required score excludes candidate from the current ranking rather than substituting a neutral value.

## 13. Deterministic Sort

Descending:

1. UniverseScore;
2. DataQualityScore;
3. LiquidityScore;
4. SpreadExecutionScore;
5. canonical InstrumentId ordinal ascending.

No random tie break.

## 14. US Zone Selection

The current Owner-directed structure is preserved.

### Zone C capital state `<500 USD`

Rank hard-eligible `<30 USD` tier and select up to 10.

### Zone B capital state `500 <= capital <2000 USD`

- retain/recompute current qualified Zone-C tier up to 10 `<30 USD`;
- independently rank the additional `30 <= price <250 USD` tier and select up to 10;
- combined active universe is union, deduplicated by InstrumentId.

### Zone A capital state `>=2000 USD`

- retain/recompute Zone C up to10;
- retain/recompute Zone B additional tier up to10;
- independently rank `price >=250 USD` tier and select up to10;
- union/deduplicate.

This materializes the intended nested C/B/A knowledge without selecting the same stock twice.

If fewer than target count pass, keep fewer. Never lower hard gates.

## 15. Crypto Selection

Rank all hard-eligible USD-quoted Crypto Spot instruments by the same UniverseScore and select up to 10.

There is no price-band tiering in Crypto v1.

A later capital-size-specific Crypto universe policy requires a new profile.

## 16. Replacement / Churn Hysteresis

To prevent ranking churn:

An incumbent selected instrument remains selected when it still passes hard gates unless a non-incumbent's UniverseScore exceeds it by at least:

```text
500 points (5 percentage points)
```

Replacement algorithm per tier:

1. rank all candidates;
2. hard-remove any incumbent failing eligibility;
3. fill empty slots by ranking;
4. for a full slot set, replace the lowest-score incumbent only when best outsider score >= incumbent score +500;
5. repeat until no eligible replacement passes threshold.

Hard failure bypasses hysteresis.

## 17. Price Boundary Observation

Zone price classification uses the latest VALID DP-003 midpoint at the universe refresh boundary when market active; otherwise latest VALID completed DP-005 close from the most recent regular session.

A price crossing a zone band during the day does not force intraday universe tier migration unless an explicit event-driven universe refresh occurs. Risk/execution still uses current price.

## 18. Verification Families

Verifier SHALL cover:

1. absolute liquidity gates;
2. stock price >=1 rule;
3. exact daily dollar-volume fallback;
4. cross-sectional liquidity mid-rank;
5. ActivityRatio score endpoints;
6. spread sampling schedule;
7. cheap opportunity event formula/no double count;
8. minimum valid-window rule;
9. data quality minimum;
10. triangular volatility score;
11. diversification current-position logic;
12. exact weighted score;
13. deterministic tie-break;
14. C/B/A tier boundaries and union/deduplication;
15. Crypto no price bands;
16. fewer-than-target does not relax gates;
17. 500-point incumbent replacement hysteresis;
18. hard failure immediate removal.

## 19. Finding Disposition

```text
AC-ALG-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
UNIVERSE_SUBSCORE_FORMULAS = EXACT v1
TOP_N_SELECTION/HYSTERESIS = EXACT v1
```
