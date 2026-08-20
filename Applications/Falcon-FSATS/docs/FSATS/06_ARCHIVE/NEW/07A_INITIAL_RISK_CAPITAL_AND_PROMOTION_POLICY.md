# FSATS SIA — Initial Risk, Capital and Promotion Policy v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `MATERIAL DESIGN CANDIDATE / OWNER DECISION REQUIRED`
**Trading Authority:** `NONE`
**Purpose:** close numeric ambiguity in T-LSA-07/T-LSA-08 and define a conservative initial validation progression without claiming these values came from V1.3.

## 1. Provenance and Honesty Rule

Historical V1.3 evidence states that risk limits, Paper duration/sample, drawdown, Tiny Live rules and Guardian triggers/playbooks were fixed in that baseline, but the currently available historical excerpts do not expose the exact numeric values.

Therefore:

```text
VALUES IN THIS FILE = NEW SIA v0.1 CANDIDATE VALUES
VALUES IN THIS FILE != CLAIMED V1.3 NUMBERS
OWNER_ACCEPTANCE = REQUIRED BEFORE CURRENT-DESIGN STATUS
```

The values are deliberately conservative initial test/validation limits. A different Owner-approved policy may replace them before implementation or activation.

## 2. Capital Definitions

For risk calculations:

```text
NAV = reconciled net liquidation/equity value of the exact TradingAccountId
DeployableCapital = capital currently authorized for Trading in the environment
AvailableUnreservedCapital = DeployableCapital - active held reservations - protected settlement/fee buffers
PeakNAV = highest reconciled end-of-evaluation NAV since current validation/promotion epoch start
CurrentDrawdown = max(0, (PeakNAV - CurrentNAV) / PeakNAV)
```

Unknown/stale/conflicted NAV/capital state blocks new risk.

## 3. Global Initial Exposure Invariants

For all initial environments:

```text
BorrowedLeverage = 0
MarginBorrowing = disabled
ShortBorrow = disabled
Derivatives = disabled
MaximumGrossNotionalExposure <= 100% of authorized DeployableCapital
MaximumNetLongExposure <= 100% of authorized DeployableCapital
```

Fees/slippage/settlement buffers reduce usable capital; they do not permit gross exposure above 100%.

## 4. Initial Market Allocation Envelope

Owner design direction preserves dynamic multi-market allocation with an initial 1:1 starting split.

Initial policy:

```text
US_EQUITIES target = 50%
CRYPTO_SPOT target = 50%
```

This is a **target**, not a requirement to force capital into weak opportunities.

Dynamic allocator may vary each market between:

```text
Minimum normal target floor = 25% of DeployableCapital
Maximum normal target ceiling = 75% of DeployableCapital
```

Unused allocation remains cash/uncommitted. It is not automatically transferred to create a trade.

The allocator may go below the 25% target floor, including to 0%, when:

- market is closed/unavailable;
- Guardian restriction applies;
- Data Products are unfit;
- no positive validated opportunity exists;
- drawdown/risk policy requires de-risking.

The allocator may not exceed 75% in one market under the initial normal policy without a new reviewed policy version.

## 5. Market Allocation Score

For each market, compute `MarketCapitalFitness` 0..10000:

```text
25% validated opportunity density
20% aggregate calibrated expected net edge
20% data/provider quality
15% execution/liquidity quality
10% recent risk-adjusted strategy performance
10% diversification benefit vs other market
```

Hard Guardian/Risk/Data gates override the score.

For two eligible markets with scores `S_us`, `S_crypto`:

```text
RawUS = S_us / (S_us + S_crypto)
RawCrypto = 1 - RawUS
```

Then clamp each normal target to 25%..75%, renormalizing the pair. If both scores are zero/invalid, deployable new-risk allocation = 0 and capital remains cash.

Tie/identical score -> 50/50.

## 6. Paper Environment Risk Policy

Paper exists to test realism, not to make limits artificially loose.

### Per-trade risk

Maximum initial stop-loss cash risk per **new position decision**:

```text
0.25% of current reconciled NAV
```

If multiple entries belong to the same position thesis/OrderChain/strategy decision scope, their combined initial stop risk counts toward the same position risk budget.

### Position concentration

Maximum gross notional in one instrument:

```text
10% of DeployableCapital
```

### Correlated cluster concentration

For instruments/strategies in the same correlation cluster (`>=0.75` recent validated correlation):

```text
Maximum combined gross notional = 25% of DeployableCapital
Maximum combined initial stop risk = 1.00% NAV
```

### Market gross notional

```text
US Equities <= current market allocation ceiling
Crypto Spot <= current market allocation ceiling
```

### Portfolio concurrent initial stop risk

Sum of current initial/remaining protected loss-at-invalidation estimates across open risk positions:

```text
<= 2.00% NAV
```

### Daily loss states

Using current evaluation day boundary defined by market/account policy:

```text
SOFT_DEGRADE at -1.50% NAV from day-start NAV
HARD_NEW_RISK_STOP at -2.00%
GUARDIAN_ESCALATION_CANDIDATE at -2.50%
```

At HARD_NEW_RISK_STOP:
- no new/increased exposure;
- existing positions managed under Risk/Guardian policy;
- reconciliation/exit remains enabled.

### Weekly loss state

```text
HARD_NEW_RISK_STOP = -4.00% from week-start NAV
```

### Drawdown

```text
WATCH = 4.0%
RESTRICTED = 6.0%
HARD_NEW_RISK_STOP = 8.0%
```

`RESTRICTED` reduces per-trade risk budget by 50% and portfolio concurrent risk ceiling by 50%.

Recovery from drawdown restriction requires both:
- current drawdown below the lower threshold for the configured recovery qualification window;
- no unresolved Guardian/risk integrity incident.

Time alone does not reset drawdown state.

## 7. Tiny Live Candidate Risk Policy

Tiny Live remains **unauthorized** until separate Owner/runtime decision and all promotion gates pass.

When eventually authorized, initial Tiny Live policy is intentionally stricter than Paper.

### Capital cap

```text
TinyLiveAuthorizedCapital <= min(
  5% of total owner-authorized Trading capital,
  separately declared TinyLive absolute capital cap
)
```

The absolute cap has **no permissive default**. It must be explicitly supplied in the Owner Tiny Live authorization. Without it, Tiny Live remains disabled.

### Per-trade risk

```text
<= 0.10% of TinyLiveAuthorizedCapital-equivalent NAV
```

### Instrument concentration

```text
<= 5% of TinyLiveAuthorizedCapital
```

### Portfolio concurrent initial stop risk

```text
<= 0.75% of TinyLive NAV
```

### Daily loss

```text
SOFT_DEGRADE = -0.50%
HARD_NEW_RISK_STOP = -1.00%
GUARDIAN_ESCALATION_CANDIDATE = -1.25%
```

### Weekly loss

```text
HARD_NEW_RISK_STOP = -2.00%
```

### Drawdown

```text
WATCH = 1.5%
RESTRICTED = 2.0%
HARD_NEW_RISK_STOP = 3.0%
```

At RESTRICTED, per-trade and concurrent risk ceilings halve again.

No automatic scale-up from Tiny Live exists.

## 8. Live / Scale Policy

No full Live numeric risk policy is defined by this SIA v0.1.

```text
FULL_LIVE = NOT_AUTHORIZED / POLICY_NOT_YET_ADMITTED
```

A proven Paper/Tiny Live record does not authorize extrapolating these percentages to larger capital. Full Live/Scale requires a separately reviewed Owner-approved risk/capital policy based on observed Paper-to-Live divergence, liquidity capacity and operational evidence.

## 9. Protective Exit Exception

Risk limits constrain **new/increased risk**.

A valid risk-reducing/protective exit may temporarily:

- exceed normal order-rate/participation preferences;
- consume reserved exit/fee buffer;
- use a stricter Guardian emergency execution profile;

only when the explicit protective policy shows it reduces expected capital harm versus inaction.

It still cannot fabricate broker capability, authority, price truth or fill truth.

## 10. Position-Sizing Formula

For a new-risk proposal:

```text
RiskCashBudget = NAV * EnvironmentPerTradeRiskPct
StopDistancePerUnit = abs(EntryReference - InitialInvalidation)
EstimatedPerUnitExitCost = conservative P90 slippage + fees estimate attributable per unit
EffectiveLossPerUnit = StopDistancePerUnit + EstimatedPerUnitExitCost
RiskBudgetQuantity = floor_to_quantity_step(RiskCashBudget / EffectiveLossPerUnit)
```

Other ceilings are computed:

```text
CapitalAvailableQuantity
InstrumentConcentrationQuantity
CorrelatedClusterQuantity
PortfolioConcurrentRiskQuantity
MarketAllocationQuantity
LiquidityCapacityQuantity
Broker/InstrumentMaximumQuantity
GuardianRestrictionQuantity
StrategyValidityQuantity
```

Final quantity:

```text
ApprovedQuantity = min(all valid ceilings)
```

If any mandatory ceiling is UNKNOWN -> no new-risk quantity.
If ApprovedQuantity is below minimum executable quantity/notional -> NO_TRADE.

## 11. Existing-Position Risk Recalculation

Current portfolio risk uses a conservative loss-at-protection estimate:

```text
RemainingPositionRisk = max(
  0,
  (CurrentReferencePrice - CurrentProtectiveInvalidationPrice) * Quantity
  + conservative exit costs
)
```

For positions whose protective level is above entry/current cost basis and locks profit, remaining loss risk may be zero but concentration/exposure limits still apply.

No strategy may claim zero risk merely because it plans to exit.

## 12. Gap / Tail Add-On

For US equities held across session close and crypto during high-volatility regime, T-LSA-07 adds a tail-risk buffer:

```text
US overnight candidate: max(1.0 * daily ATR per unit, model P90 adverse gap estimate)
Crypto HIGH_VOL/LIQUIDITY_STRESS: max(0.75 * 24h ATR per unit, model P90 adverse move estimate)
```

This add-on is used for risk-capacity evaluation, not as a guaranteed stop fill loss.

If required tail estimate/data is unavailable, Risk may size using the deterministic ATR fallback above; if ATR itself is unavailable, no new risk.

## 13. Paper Validation Minimum Before Tiny Live Eligibility

Tiny Live may not be presented for Owner authorization until all are true:

### Time coverage

```text
US Equities: >= 60 US regular trading days of Paper evidence
Crypto Spot: >= 60 calendar days of Paper evidence
Combined system: both market minima satisfied when both are intended for Tiny Live
```

### Decision/trade sample

System-level:

```text
>= 300 closed/reconciled risk-bearing Paper trades total
```

Strategy-level for any strategy proposed active in Tiny Live:

```text
>= 50 closed/reconciled Paper trades for that exact StrategyVersion
AND >= 100 combined FSTSimA + Paper independent risk events across multiple regimes
```

If a low-frequency strategy cannot meet 50 Paper trades in the window, it remains non-Tiny-Live until sufficient evidence exists; the threshold is not waived to meet schedule.

### Regime coverage

At least:

- trending;
- ranging;
- high volatility;
- low volatility;
- at least one materially degraded provider/data scenario;
- broker ambiguity/cancel/replace failure fixtures;
- Guardian/resource-pressure adversarial scenarios in FSTSimA.

### Drawdown / policy compliance

- no unresolved HARD_NEW_RISK_STOP cause;
- no open Critical/High architecture/security/integrity finding;
- zero unapproved Risk/Guardian/authority bypass events;
- zero unreconciled duplicate/excess broker exposure events at final evidence cut.

## 14. Tiny Live -> Larger Live Eligibility

This SIA defines only **evidence requirements**, not authority or final Live thresholds.

Minimum candidate evidence before even proposing a larger Live policy:

```text
>= 30 calendar days Tiny Live
>= 100 closed/reconciled Tiny Live trades across the exact proposed active StrategyVersions collectively
Paper-vs-Tiny-Live slippage/latency/fill divergence quantified
No unresolved capital/order/position reconciliation defect
No Critical/High security/authority/protection finding
Guardian and FSARM behavior exercised in real operational degradation or equivalent governed drills where possible
```

Meeting these does not authorize scale.

## 15. Strategy-Level Loss/Drawdown Controls

For each ACTIVE StrategyVersion:

```text
STRATEGY_WATCH when rolling peak-to-current strategy equity contribution drawdown >= 6R
STRATEGY_RESTRICTED when >= 8R
STRATEGY_DORMANCY_REVIEW when >= 10R
```

`R` is strategy realized risk unit normalized from actual initial risk at each trade, so it is comparable across varying capital.

At RESTRICTED:
- new strategy risk budget multiplier = 0.50;
- candidate requires calibration/failure review.

At 10R:
- automatic state is not RETIRED;
- it triggers DORMANCY_REVIEW and no new risk until governed review outcome.

A profitable single trade does not reset the rolling peak drawdown history.

## 16. Loss-Streak Control

Per exact StrategyVersion/market:

```text
3 consecutive full-risk losses -> WATCH / risk multiplier 0.75
5 consecutive losses -> RESTRICTED / risk multiplier 0.50
7 consecutive losses -> no new risk pending review
```

A "loss" for streak purposes is a closed reconciled trade with net realized R <= -0.50R. Partial small losses between 0 and -0.50R affect drawdown but not streak count.

A win >= +0.50R resets the consecutive-loss counter; scratch outcomes between -0.50R and +0.50R neither increment nor reset.

## 17. Correlation / Concentration

Correlation is measured from strategy/instrument daily or decision-horizon returns using the current validated profile, minimum 60 aligned samples.

If evidence insufficient, potential same-sector/same-underlying/obvious dependency is treated conservatively as correlated until proven otherwise.

Initial cluster threshold:

```text
abs(correlation) >= 0.75
```

For long-only initial profile, positive correlation is the primary concentration risk; negative correlation can contribute diversification but cannot increase gross exposure beyond 100%.

## 18. Guardian Numeric Triggers From Risk State

These are **incident/protection signal candidates**, not direct Guardian command authority:

### Paper

- daily loss <= -2.50% -> `RISK_DAILY_LOSS_CRITICAL_SIGNAL`;
- current drawdown >= 8.0% -> `RISK_DRAWDOWN_HARD_STOP_SIGNAL`;
- any deterministic Risk hard limit exceeded due to state drift/external fill -> immediate high-severity protection observation;
- capital reservation/position/order truth conflict involving possible excess exposure -> high/critical depending quantified maximum possible exposure.

### Tiny Live candidate

- daily loss <= -1.25% -> critical protection signal;
- drawdown >=3.0% -> hard-stop signal;
- possible duplicate/excess external order/fill exposure -> critical until reconciled when maximum exposure cannot be bounded safely.

Guardian still applies its G-LSA-01 deterministic incident policy and authority mapping; these thresholds do not let T-LSA-07 issue Guardian commands itself.

## 19. Risk-Policy Version Binding

Every RiskDecision records:

```text
RiskPolicyId = FSATS-RISK-CAPITAL-v1.0
EnvironmentPolicy
NAV/Capital snapshot
Market allocation profile
Correlation profile
Strategy state/multiplier
All computed ceilings
Selected minimum ceiling
Reason codes
```

Any numeric change to this file's behavior creates `FSATS-RISK-CAPITAL-v1.x/2.x` as governed successor and invalidates stale risk validation evidence for affected rules.

## 20. Owner Decisions Embedded in This Candidate

Explicit review points:

1. 0.25% Paper per-trade risk;
2. 2.00% concurrent portfolio stop risk;
3. 10% instrument concentration;
4. 25% correlated cluster gross concentration;
5. Paper daily/weekly/drawdown thresholds;
6. 50/50 initial market target with 25/75 dynamic normal envelope;
7. Tiny Live 5%-of-authorized-capital cap plus mandatory absolute Owner cap;
8. Tiny Live risk/drawdown limits;
9. Paper minimum 60-day/300-trade system sample and 50 per exact strategy;
10. Tiny Live minimum evidence before larger Live proposal;
11. strategy R-drawdown/loss-streak restrictions.

These are not silently inherited from V1.3 and must be explicitly accepted or changed by the Owner as part of the SIA package.

## 21. Negative Fixtures

Verifier SHALL reject:

- position sizing above any ceiling;
- rounding risk-increasing quantity upward;
- gross exposure >100%;
- borrowed leverage/short/derivatives in initial profile;
- forced market allocation trade merely to reach 50/50;
- market normal allocation >75% without successor policy;
- new risk after daily/weekly/drawdown hard stop;
- Risk unknown treated as allow;
- Tiny Live with no explicit absolute capital cap;
- Tiny Live using Paper risk percentages;
- strategy active after 7-loss review trigger without governed disposition;
- correlated cluster exposure above limit;
- Paper sample threshold waived because of calendar deadline;
- meeting promotion evidence interpreted as Owner authorization.
