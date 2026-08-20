# FSATS SIA — Risk Accounting, Time Boundary and Tail-Risk Integration Remediation

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** fresh Red-Team attack against freeze `ce489698b8cb4d614daa82627eb5a58d9795c6ad`
**Finding:** raw NAV/time-boundary/tail-risk ambiguity in `07A`

## 1. Purpose

Close three high-consequence ambiguities before a new semantic freeze:

1. define one exact cross-market portfolio risk-day/week boundary for US Equities + 24/7 Crypto;
2. prevent deposits/withdrawals/transfers from falsifying daily loss/drawdown performance state;
3. integrate stop loss, tail/gap loss and execution cost into one exact position-sizing/open-risk formula.

This file controls where it conflicts with the earlier `07A` wording.

## 2. Risk Clock

Portfolio-level Risk accounting uses one canonical clock independent of individual market sessions:

```text
RISK_TIMEZONE = UTC
RISK_DAY_START = 00:00:00 UTC
RISK_DAY_END = next 00:00:00 UTC exclusive
RISK_WEEK_START = Monday 00:00:00 UTC
RISK_WEEK_END = next Monday 00:00:00 UTC exclusive
```

Reasons:

- Crypto trades continuously;
- US market calendar boundaries remain important for strategy/session behavior but cannot define the only portfolio risk day;
- UTC avoids DST ambiguity in the combined portfolio loss ledger.

US regular-session metrics may still be tracked separately, but Paper/Tiny-Live daily/weekly hard loss stops in `07A` use the UTC portfolio risk clock above.

## 3. Risk Epoch

A `RiskEpochId` begins when the Owner/governance starts a new Paper/Tiny-Live validation/promotion epoch.

The epoch cannot be reset automatically by:

- restart;
- calendar month/year;
- deposit/withdrawal;
- strategy version change;
- recovering from drawdown;
- crossing a daily/weekly boundary.

A new epoch requires an explicit governed reason/decision and preserves predecessor evidence.

`PeakRiskEquity` for drawdown is tracked within the current RiskEpochId.

## 4. External Cash Flow

`ExternalCashFlow` means capital entering/leaving the TradingAccount from outside the Trading strategy/execution PnL process, including:

```text
OWNER_DEPOSIT
OWNER_WITHDRAWAL
EXTERNAL_TRANSFER_IN
EXTERNAL_TRANSFER_OUT
ACCOUNT_FUNDING_ADJUSTMENT
```

Trading buys/sells, fees, commissions, realized/unrealized PnL and normal position settlement are **not** external cash flows.

Every external cash flow is a reconciled ledger event with exact amount/currency/asset, effective time, valuation reference and evidence.

## 5. Cash-Flow-Adjusted Risk Equity

For the account base valuation currency:

```text
RawNAV_t = reconciled current net liquidation value
NetExternalFlowSinceEpoch_t = sum(value of external inflows) - sum(value of external outflows)
RiskEquity_t = RawNAV_t - NetExternalFlowSinceEpoch_t
```

At epoch start:

```text
RiskEquity_0 = RawNAV_0
NetExternalFlowSinceEpoch_0 = 0
```

Interpretation:

- a deposit increases RawNAV and NetExternalFlow by the same amount, so it does not create fake profit or erase drawdown;
- a withdrawal decreases both, so it does not create fake loss;
- Trading PnL changes RiskEquity.

If an external flow asset/currency requires valuation, use a reconciled canonical valuation at the external-flow effective boundary. Unknown material valuation -> Risk accounting state `RECONCILIATION_REQUIRED`, blocking new risk.

## 6. Day / Week Start Equity

At each UTC boundary, persist immutable snapshots:

```text
RiskDayStartSnapshot:
  RiskDayId = YYYY-MM-DD UTC
  RiskEquityAtStart
  RawNAVAtStart
  NetExternalFlowSinceEpochAtStart
  OpenPositionRiskSnapshot
  EvidenceRefs

RiskWeekStartSnapshot:
  ISO week identity with Monday UTC start
  same fields
```

If the Application was unavailable exactly at the boundary, reconstruct the start snapshot from the first authoritative state/evidence after restart plus ledger replay to the boundary. Until reconstruction succeeds, daily/weekly risk state is UNKNOWN and no new risk is permitted.

## 7. Daily / Weekly Loss Formula

```text
DayReturn = (CurrentRiskEquity - RiskDayStart.RiskEquityAtStart)
            / RiskDayStart.RiskEquityAtStart

WeekReturn = (CurrentRiskEquity - RiskWeekStart.RiskEquityAtStart)
             / RiskWeekStart.RiskEquityAtStart
```

If start equity <=0 or unknown -> new risk blocked.

These returns include realized and unrealized Trading PnL, fees and current reconciled valuation effects, but exclude external cash-flow distortions by construction.

## 8. Drawdown Formula

Track:

```text
PeakRiskEquity = max(previous PeakRiskEquity, CurrentRiskEquity)
CurrentDrawdown = max(0, (PeakRiskEquity - CurrentRiskEquity) / PeakRiskEquity)
```

Peak updates only from authoritative reconciled `RiskEquity` observations.

A deposit does not raise RiskEquity and cannot reset the peak/drawdown. A withdrawal does not lower RiskEquity by itself.

If material position valuation is UNKNOWN, drawdown state is UNKNOWN rather than assuming last price/zero. New risk blocked; protective actions remain governed separately.

## 9. Base Valuation Currency

Every TradingAccount profile declares exact `RiskBaseCurrency`.

Initial US-equities/Alpaca-Paper candidate account normally uses USD, but this is a certified account-profile field, not a global hardcoded assumption.

For crypto assets/positions not quoted directly in RiskBaseCurrency, valuation requires a validated canonical conversion path from FSAPMA Data Products.

Conversion path rules:

- exact instrument/asset identities;
- DataQuality VALID or explicitly permitted degraded valuation policy;
- freshness requirement;
- no cyclic conversion path;
- deterministic path selection policy;
- conversion evidence bound to Risk snapshot.

No conversion path -> affected valuation UNKNOWN -> new risk blocked.

## 10. Stop-Risk Distance

For a LONG new-risk proposal:

```text
StopLossDistancePerUnit = max(0, EntryReference - InitialInvalidationPrice)
```

For a future SHORT profile, the symmetric formula is defined only when shorting is separately enabled.

A stop order/level is a risk hypothesis, not guaranteed fill price.

## 11. Tail / Gap Risk Distance

Tail risk from `07A` is converted to a per-unit adverse distance:

### US Equities with overnight/session-gap exposure possible

```text
TailRiskDistancePerUnit = max(
  1.0 * DailyATR14,
  ModelP90AdverseGapDistance if valid else 0
)
```

### Crypto HIGH_VOL or LIQUIDITY_STRESS regime

```text
TailRiskDistancePerUnit = max(
  0.75 * ATR24hEquivalent,
  ModelP90AdverseMoveDistance if valid else 0
)
```

### Ordinary intraday/non-tail condition

```text
TailRiskDistancePerUnit = 0
```

Market/strategy policy determines whether overnight holding is permitted. If a US equity strategy is guaranteed by explicit policy to flatten before overnight gap exposure and the position is not expected/permitted to cross that boundary, the overnight tail add-on need not apply before entry. If the position remains open into the overnight-risk boundary, Risk recomputes the tail exposure and must still fit portfolio risk capacity or initiate governed reduction/protection.

## 12. Conservative Loss Distance

New-risk sizing uses:

```text
ConservativePriceLossDistancePerUnit = max(
  StopLossDistancePerUnit,
  TailRiskDistancePerUnit
)
```

They are **not added together**, because both represent alternative adverse price-distance envelopes for the same unit. The more conservative envelope governs.

Execution/exit costs are then added:

```text
ConservativeExitCostPerUnit = P90 expected adverse exit slippage per unit
                              + per-unit allocated expected fees/commissions

EffectiveLossPerUnit = ConservativePriceLossDistancePerUnit
                       + ConservativeExitCostPerUnit
```

This formula supersedes the simpler `StopDistance + cost` formula in 07A Section 10.

## 13. Risk Budget Quantity

```text
RiskCashBudget = CurrentRiskEquity * EnvironmentPerTradeRiskPct

RiskBudgetQuantityRaw = RiskCashBudget / EffectiveLossPerUnit
RiskBudgetQuantity = floor_to_instrument_quantity_step(RiskBudgetQuantityRaw)
```

Preconditions:

- CurrentRiskEquity > 0 and current;
- EffectiveLossPerUnit > 0 and current;
- all required price/ATR/model/cost data valid;
- no hard Risk/Guardian restriction.

Unknown required tail/exit-cost evidence uses the deterministic fallback defined by policy; if no fallback exists, quantity = no new risk.

## 14. Open Position Remaining Risk

For each LONG open position with current reference price `P`, protective invalidation `S`, quantity `Q`:

```text
StopRemainingDistance = max(0, P - S)
CurrentTailDistance = tail/gap distance under current holding/regime profile
ConservativeRemainingDistance = max(StopRemainingDistance, CurrentTailDistance)
RemainingPositionRiskCash = Q * (ConservativeRemainingDistance + P90ExitCostPerUnit)
```

If protective invalidation is above current price due to stale/invalid state, do not treat negative distance as profit protection automatically; reconcile the protective state first.

A stop above entry may reduce ordinary stop-risk to zero, but current tail/liquidity risk can keep RemainingPositionRiskCash positive.

## 15. Portfolio Concurrent Risk

```text
PortfolioConcurrentRiskCash = sum(RemainingPositionRiskCash across all open risk positions)
                              + pending approved-but-not-yet-filled new-risk reservation risk where exposure could still occur
```

For a new proposal:

```text
ProjectedConcurrentRisk = CurrentConcurrentRisk
                          + ProposedRiskCash
                          - risk being replaced/reduced only when the reduction is already confirmed
```

Do not subtract expected cancels/exits before effect confirmation.

The environment ceiling in 07A is applied to `ProjectedConcurrentRisk / CurrentRiskEquity`.

## 16. Correlated Cluster Risk

The 1.00% NAV correlated-cluster initial stop-risk ceiling in Paper and any future environment equivalent uses the same **Conservative Remaining Risk** definition, not only nominal stop distance.

This prevents overnight/tail risk from disappearing from correlation concentration math.

## 17. Market Allocation And Risk Are Independent Ceilings

Market capital allocation governs notional/capital envelope.

Risk budget governs expected adverse loss envelope.

```text
AllowedQuantity = min(
  MarketAllocationQuantity,
  RiskBudgetQuantity,
  all other ceilings
)
```

Unused 50/50 target allocation does not increase per-trade/portfolio risk budget.

## 18. Daily Hard Stop Evaluation Frequency

Daily/weekly/drawdown state is recomputed:

- on every reconciled fill/correction;
- on material valuation update for open positions;
- on external cash-flow event;
- before every new RiskDecision;
- at the UTC day/week boundary;
- after recovery/restart before readiness.

A scheduled timer is not the sole enforcement mechanism.

## 19. Threshold Crossing Between Events

If a price/Data Product update indicates the portfolio crossed a daily/drawdown hard threshold while no order event occurred:

- T-LSA-07 state transitions immediately on the authoritative valuation update;
- new-risk queues/work already waiting must revalidate and be denied;
- Guardian receives the configured protection observation when its signal threshold is crossed.

An already submitted broker order is reconciled/handled by execution/Guardian policy; Risk cannot pretend it was never sent.

## 20. Paper / Tiny Live Threshold Mapping

The numeric percentages in 07A remain unchanged; only their accounting base is clarified:

```text
"NAV" for per-trade/portfolio percentage limits -> CurrentRiskEquity
"day-start NAV" -> RiskDayStart.RiskEquityAtStart
"week-start NAV" -> RiskWeekStart.RiskEquityAtStart
"PeakNAV" -> PeakRiskEquity
```

Where capital concentration is explicitly a percentage of `DeployableCapital` or `TinyLiveAuthorizedCapital`, that capital definition remains as written in 07A.

## 21. Promotion Sample Independence

For Paper/Tiny-Live minimum sample accounting:

### Closed trade identity

One `PositionEpisodeId` counts as one closed risk-bearing trade sample, regardless of how many partial fills/child order attempts occurred.

A position episode begins when exposure moves FLAT -> non-FLAT and ends when the exact instrument/account exposure returns to FLAT.

### Independent risk-event evidence

For FSTSimA + Paper strategy validation, multiple trades are not counted as independent regime events when they share all of:

```text
same StrategyVersion
same InstrumentId
same underlying market shock/catalyst/scenario identity
overlapping position episode time
```

Such observations form one `ValidationRiskEventCluster` for the minimum `100 combined independent risk events` rule.

Statistical analyses may use formal effective sample-size methods in addition, but cannot inflate the hard minimum by counting partial fills/retries/duplicate scenarios as separate independent evidence.

## 22. Deposits / Withdrawals During Promotion Evaluation

External cash flows do not reset risk performance state.

Promotion reports SHALL show:

- raw capital path;
- external cash flows;
- cash-flow-adjusted RiskEquity path;
- drawdown from RiskEquity;
- performance returns using an accepted cash-flow-adjusted method.

A large deposit immediately before review cannot make prior drawdown disappear.

## 23. Reconciliation Failure

If cash-flow ledger, valuation conversion, day/week boundary reconstruction, tail-risk input or CurrentRiskEquity is materially conflicted:

```text
RISK_ACCOUNTING_STATE = RECONCILIATION_REQUIRED
NEW_RISK = DENIED
PROTECTIVE / RECONCILIATION ACTIONS = governed separately
```

## 24. Red-Team Finding Disposition

```text
RT-PREFREEZE-RISK-001 = REMEDIATED
PORTFOLIO_RISK_CLOCK = UTC EXACT
CASH_FLOW_ADJUSTED_RISK_EQUITY = DEFINED
DAILY_WEEKLY_DRAWDOWN_BASE = DEFINED
TAIL_RISK_IN_POSITION_SIZING = EXACT MAX ENVELOPE + EXIT COST
OPEN_CONCURRENT_RISK = TAIL-AWARE
PROMOTION_SAMPLE_IDENTITY = DEFINED
```

Because this file changes/clarifies material Risk semantics after freeze `ce489...`, that freeze and its A/C PASS cannot be used for final Owner review. A new semantic freeze and fresh A/C review are mandatory.

## 25. Verification Families

Verifier SHALL cover at minimum:

1. UTC day/week boundary across US/crypto activity;
2. DST has no effect on portfolio risk-day boundary;
3. deposit does not create profit/reduce drawdown;
4. withdrawal does not create loss/increase drawdown;
5. unknown FX/asset valuation blocks new risk;
6. RiskEquity reconstruction after restart;
7. per-trade sizing uses max(stop, tail) not stop alone;
8. tail and stop are not double-added;
9. P90 exit cost is added to conservative price loss;
10. open risk retains tail risk even when stop locks profit;
11. pending unconfirmed exit/cancel risk is not subtracted prematurely;
12. hard stop applies on valuation update before new order event;
13. queued new-risk work revalidates after threshold crossing;
14. one position episode counts one trade despite partial fills;
15. duplicate/overlapping scenario samples do not inflate independent-risk-event minimum;
16. external cash flows cannot reset promotion epoch/peak.
