# FSATS SIA — R3 Architecture/Consistency Remediation Reconciliation

**Package:** `FSATS-SIA-v0.1`
**Record Type:** `PRE-R4 SEMANTIC RECONCILIATION`
**R3 A/C:** `21B_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R3.md` = FAIL

## 1. Purpose

Bind each R3 A/C finding to the exact semantic remediation before R4 freeze. This record does not claim review PASS.

## 2. AC-ALG-001 — HIGH — Universe Ranking Subscores

Remediation:

`07E_UNIVERSE_RANKING_EXACT_FORMULA_SPEC.md`

Now exact:

- preliminary hard eligibility;
- US absolute liquidity/price/spread coverage gates;
- Crypto USD-quote/liquidity/data gates;
- daily dollar-volume formula/fallback;
- LiquidityScore cross-sectional mid-rank;
- VolumeActivityScore 5-vs-20-day ratio;
- spread/execution score;
- generic opportunity-density events;
- DataQuality minimum;
- triangular VolatilityTradability;
- position-correlation DiversificationScore;
- final weighted UniverseScore;
- deterministic sort;
- C/B/A tier materialization;
- Crypto top-10 rule;
- 500-point incumbent replacement hysteresis.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 3. AC-ALG-002 — HIGH — MarketCapitalFitness / Allocation

Remediation:

`07D_MARKET_CAPITAL_FITNESS_AND_DYNAMIC_ALLOCATION_SPEC.md`

Now exact:

- RiskEpoch starts 50/50;
- normal allocation epoch 00:05 UTC daily;
- hard-state event-driven reductions;
- OpportunityDensityScore;
- top-opportunity median NetEdge score;
- DataQuality aggregate;
- standardized execution probe/score;
- 30-day/100-sample performance score with <20 fallback;
- diversification score/fallback;
- exact weighted MarketCapitalFitness;
- two-market raw share/clamp;
- 25..75 normal bounds;
- cash remainder;
- <5-point hysteresis no-op;
- 10-point maximum normal daily shift;
- no forced trade/reservation.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 4. AC-PMA-001 — HIGH — Provider Route Score

Remediation:

`08D_PROVIDER_ROUTE_FITNESS_AND_FAILOVER_FORMULA_SPEC.md`

Now exact:

- hard eligibility before score;
- 100-observation/60-minute quality EWMA + certification fallback;
- current freshness score;
- nearest-rank P95 latency fitness + certification fallback;
- quota headroom after outstanding reservations;
- direct reuse of INT-007 ReliabilityScore;
- marginal cost authority/normalization;
- exact ContinuityScore states;
- weighted RouteScore;
- deterministic tie-break;
- 750-point switch hysteresis;
- hard failure bypass;
- quota reservation concurrency;
- streaming route stability.

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 5. AC-SIM-001 — MEDIUM — Digest To PRNG State

Remediation:

`10B_FSTSIMA_DIGEST_TO_PRNG_STATE_INITIALIZATION_CLARIFICATION.md`

Now exact:

```text
SHA256 digest -> four big-endian UInt64 d0..d3
for each i independently:
  temp = d_i
  s_i = SplitMix64_Next(ref temp)
no chained SplitMix state
all-zero protection exact
```

Status: `REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL`.

## 6. R3 Review Outcome Preservation

```text
R3 A/C = FAIL
R3 Red-Team = NOT_RUN_AS_FINAL because A/C blocked progression
R3 OWNER REVIEW = NOT_ELIGIBLE
```

The R3 A/C record remains immutable historical evidence.

## 7. R4 Required Lifecycle

```text
FREEZE R4
-> FRESH A/C R4
-> FRESH RED-TEAM R4
-> OWNER REVIEW only if unchanged R4 passes both
```

No R3/R2 PASS is inherited.

## 8. Pre-R4 Known Finding State

```text
AC-ALG-001 = REMEDIATED
AC-ALG-002 = REMEDIATED
AC-PMA-001 = REMEDIATED
AC-SIM-001 = REMEDIATED

KNOWN OPEN CRITICAL = 0
KNOWN OPEN HIGH = 0
KNOWN OPEN MEDIUM = 0
```

This is a reconciliation record, not A/C or Red-Team PASS.
