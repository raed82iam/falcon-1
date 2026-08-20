# FSATS SIA — Risk Red-Team Finding and Freeze Supersession Record

**Package:** `FSATS-SIA-v0.1`
**Record Type:** `SEMANTIC CHANGE / FREEZE SUPERSESSION`
**Previous Freeze:** `ce489698b8cb4d614daa82627eb5a58d9795c6ad`
**Previous A/C Review:** `21_ARCHITECTURE_AND_CONSISTENCY_REVIEW.md`
**Previous A/C Result:** `PASS`, valid only for the previous freeze

## 1. Finding

During the fresh Red-Team attack that began after the previous A/C PASS, a high-consequence ambiguity was found in the initial Risk/Capital policy:

```text
RT-RISK-001
```

Affected semantics:

- combined US Equities + 24/7 Crypto portfolio daily/weekly risk clock was not exact;
- deposits/withdrawals/external transfers could distort raw-NAV-based loss/drawdown if not cash-flow adjusted;
- tail/gap loss was described but not bound into one exact `RiskBudgetQuantity` formula;
- open concurrent risk could understate current tail exposure if only nominal stop distance were considered;
- promotion sample counting did not explicitly prevent partial fills/overlapping risk events from inflating sample count.

Severity before remediation: `HIGH`.

## 2. Remediation

Added:

`07B_RISK_ACCOUNTING_TIME_BOUNDARY_AND_TAIL_INTEGRATION.md`

The remediation defines:

- UTC risk day/week boundaries;
- explicit RiskEpoch;
- external cash-flow events;
- cash-flow-adjusted `RiskEquity`;
- day/week start snapshots;
- cash-flow-adjusted daily/weekly return and drawdown;
- exact base-currency/conversion requirement;
- `ConservativePriceLossDistance = max(StopLossDistance, TailRiskDistance)`;
- P90 exit cost addition;
- exact RiskBudgetQuantity formula;
- tail-aware remaining/open concurrent risk;
- no subtraction of unconfirmed cancel/exit risk;
- position-episode trade sample identity;
- independent validation-event clustering.

## 3. Governance Consequence

The semantic change occurred **after** the exact freeze `ce489698...` and after `21_ARCHITECTURE_AND_CONSISTENCY_REVIEW.md` reviewed that freeze.

Therefore:

```text
ce489698... = SUPERSEDED_AS_CURRENT_REVIEW_FREEZE
21_ARCHITECTURE_AND_CONSISTENCY_REVIEW.md = HISTORICALLY_VALID_FOR_ce489_ONLY
21 PASS != PASS FOR NEW SEMANTIC VERSION
```

The old review record SHALL remain preserved. It SHALL NOT be edited to pretend it reviewed `07B`.

Required lifecycle:

```text
07B semantic remediation
-> new exact semantic freeze
-> fresh Architecture/Consistency review on new freeze
-> fresh Red-Team restart/continuation on the same new freeze
-> Owner review only after both pass without later semantic change
```

## 4. Master-Index Reconciliation

`07B_RISK_ACCOUNTING_TIME_BOUNDARY_AND_TAIL_INTEGRATION.md` and this record are controlling additions to the semantic/review history even if an earlier Master Index revision predates their creation.

At the next Master Index revision, they SHALL be included explicitly. No implementation worker may rely on a stale file count/index to omit `07B`.

## 5. Finding Disposition

```text
RT-RISK-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
SEMANTIC_CHANGE = YES
OLD_FREEZE_REUSABLE_FOR_FINAL_REVIEW = NO
OLD_A_C_PASS_REUSABLE_FOR_NEW_FREEZE = NO
OWNER_ACCEPTANCE = NOT_YET
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
```
