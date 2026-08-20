# FSATS SIA — Strategy Capital Competition and Reservation Priority Specification v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-CAPITAL-001`
**Owner:** APP-TRD / T-LSA-06, T-LSA-07, T-LSA-08

## 1. Purpose

Materialize the Owner-directed concept that simultaneously valid strategies compete for scarce new capital by proven efficiency, without allowing that competition to bypass Risk, Guardian, market allocation, existing obligations or atomic capital reservation.

This mechanism allocates **new-risk opportunity priority only**. It is not a market that owns capital, Risk or existing positions.

## 2. Entry Point

Capital competition occurs only when all are true:

1. two or more `TradeProposal` candidates are valid at the same `CapitalCompetitionBoundary`;
2. each has already passed T-LSA-06 strategy orchestration/conflict checks;
3. each has a valid T-LSA-07 `RiskDecision` with a maximum admissible quantity/risk ceiling;
4. Guardian and market/environment authority allow new risk;
5. total requested new reservations exceed currently available unreserved capital or another portfolio-capacity ceiling.

If sufficient capital exists for all approved proposals, no competition ranking is needed; T-LSA-08 may reserve each in canonical boundary order.

## 3. Competition Boundary

Initial v1 batching boundary:

```text
CapitalCompetitionWindow = 250 milliseconds
```

measured using Foundation authoritative time.

The first eligible proposal opens the boundary `[t0, t0+250ms)`. All eligible proposals whose authoritative eligibility time falls inside that half-open interval compete together.

A proposal arriving at or after the end starts/joins the next boundary.

The 250ms window is a Trading batching/consistency rule, not a Foundation QoS guarantee.

If a proposal TTL would expire before the window closes, the proposal is evaluated at the boundary close only if still valid; otherwise it is removed as expired.

## 4. Competition Candidate Record

Each candidate binds:

```text
TradeProposalId
StrategyVersionId
SchoolId
MarketId
InstrumentId
RiskDecisionId
MaximumRiskApprovedQuantity
RequestedQuantity
RequestedCapitalCash
RequestedInitialRiskCash
NetEdgeR
CalibratedConfidence
RegimeFitness
ExecutionQuality
DiversificationScore
StrategyHealthScore
CapitalEfficiencyEvidenceRef
DecisionEffectiveTime
ProposalExpiresAt
```

All values use exact versions from the proposal/risk decision. No score component may be recomputed using future observations inside the competition window.

## 5. Hard Candidate Exclusion

Before ranking, remove candidate when:

- proposal/risk decision expired or superseded;
- current Guardian restriction newly blocks it;
- market/account authority changed;
- Data Product state required for execution became non-VALID;
- current capital/risk state invalidates the RiskDecision version and revalidation fails;
- requested quantity normalizes to zero;
- same position/instrument/strategy intent is a duplicate under current idempotency policy.

Hard exclusion is not a low score.

## 6. Strategy Capital Efficiency Score

All subscores `0..10000`.

### Net Edge Score — 30%

```text
NetEdgeScore = round(10000 * clamp(NetEdgeR / 1.50, 0, 1))
```

NetEdgeR <=0 is already hard-ineligible.

### Calibration Score — 20%

Use exact `CalibratedConfidence` from INT-005, but multiply by calibration fitness:

```text
CalibrationFitness = round(10000 * clamp(1 - CalibrationError/0.15, 0, 1))
CalibrationScore = round(CalibratedConfidence * CalibrationFitness / 10000)
```

If calibration state is insufficient for ACTIVE eligibility, candidate is hard-excluded before this point.

### Execution Quality — 15%

Use current proposal-bound INT-002 `ExecutionQuality`.

### Diversification — 15%

Use T-LSA-07 portfolio diversification/correlation score for this exact proposal after existing + higher-priority pending exposure context is considered at competition start.

Initial value before winner iteration is the proposal's snapshot `DiversificationScore` from 07E/portfolio risk profile.

### Recent Strategy Efficiency — 10%

Use most recent up to 100 closed PositionEpisodes over previous 30 UTC days for exact StrategyVersion, minimum 20.

If fewer than 20:

```text
RecentEfficiencyScore = 4000
```

Else:

```text
MeanNetR = mean(NetRealizedR)
StdNetR = sample_stddev(NetRealizedR)
RiskAdj = MeanNetR / max(StdNetR,0.50)
RecentEfficiencyScore = round(clamp(5000 + 2500*RiskAdj,0,10000))
```

### Capital Consumption Efficiency — 10%

```text
ExpectedNetEdgeCash = NetEdgeR * RequestedInitialRiskCash
CapitalConsumption = RequestedCapitalCash
```

If CapitalConsumption <=0 -> hard invalid.

```text
EdgePerCapital = ExpectedNetEdgeCash / CapitalConsumption
CapitalConsumptionScore = round(10000 * clamp(EdgePerCapital / 0.02, 0, 1))
```

2% expected net-edge cash per deployed capital saturates this subscore.

### Final score

```text
CapitalPriorityScore = round(
  0.30*NetEdgeScore
+ 0.20*CalibrationScore
+ 0.15*ExecutionQuality
+ 0.15*DiversificationScore
+ 0.10*RecentEfficiencyScore
+ 0.10*CapitalConsumptionScore
)
```

## 7. Deterministic Ranking

Descending:

1. `CapitalPriorityScore`;
2. `NetEdgeScore`;
3. `CalibrationScore`;
4. `DiversificationScore`;
5. lower `RequestedCapitalCash`;
6. earlier `DecisionEffectiveTime`;
7. canonical `TradeProposalId` ordinal ascending.

No random tie break.

## 8. Reservation Algorithm

At boundary close:

1. create one exact `CapitalCompetitionSnapshot` including current capital/reservations/positions/Risk policy;
2. hard-filter candidates;
3. score/sort;
4. iterate in rank order;
5. before each candidate reservation, re-evaluate only **portfolio interaction ceilings affected by earlier winners**: remaining unreserved capital, market allocation remaining, concurrent risk, instrument/correlation concentration;
6. compute `CurrentMaxQuantity = min(original RiskDecision maximum, updated interaction ceilings)`;
7. if current max quantity >= minimum executable quantity, reserve the maximum quantity up to the candidate's requested quantity **after rounding down**;
8. atomically persist reservation/evidence;
9. continue to next candidate on the new state.

The algorithm never increases the original RiskDecision maximum.

## 9. Partial Award Rule

Partial reservation is permitted when:

- StrategyVersion/TradeProposal explicitly allows partial quantity;
- normalized partial quantity meets broker/instrument minimum quantity/notional;
- expected NetEdge and execution-cost checks are recomputed at the smaller quantity and still pass;
- proposal remains within its stated minimum viable quantity if one is declared.

If any condition fails, the candidate receives zero reservation for that boundary and the algorithm continues to the next candidate.

There is no arbitrary equal pro-rata split.

## 10. Existing Obligations Are Not Preempted By Score

Capital competition SHALL NOT reclaim capital from:

- current open positions;
- submitted/ambiguous broker order attempts;
- confirmed active capital reservations whose associated intent remains valid;
- Guardian-protected exit/reconciliation buffers;
- settlement/fee buffers;
- another market's already committed valid obligation.

A higher CapitalPriorityScore is not preemption authority.

Existing obligations change only through their normal Risk/position/order/Guardian lifecycle.

## 11. Correlation Update During Winner Iteration

After a winner reservation, the next candidate's correlation/concentration ceiling is recomputed against:

```text
confirmed current positions
+ active prior reservations
+ winners already reserved in this competition boundary
```

A lower-ranked highly correlated proposal may therefore become zero-quantity even when its original isolated RiskDecision was valid.

Its score is not dynamically re-ranked after each winner in v1; only its current admissible quantity is recomputed. This keeps ranking deterministic and prevents recursive score feedback.

## 12. Cross-Market Capital Boundary

Competition uses each market's current `MarketCapitalCeiling` from 07D.

A high-scoring Crypto proposal cannot consume unused US target capital outside the current allocation profile merely because US has no opportunity.

Unused market allocation remains cash until a future normal allocation epoch changes the target.

## 13. Same Instrument / Same Thesis

When multiple StrategyVersions produce simultaneously compatible LONG proposals for the same instrument:

- T-LSA-06 should normally resolve them into one TradeProposal under its correlation/conflict rules;
- if separate proposals still exist because they represent independent position-management scopes, combined instrument/Risk concentration is enforced;
- v1 does not create multiple separate broker positions if the account/broker position model is netted and the Trading PositionEpisode policy says they share one instrument exposure.

No competition algorithm bypasses T-LSA-06 consolidation semantics.

## 14. Rejected Candidate State

Competition outcome:

```text
FULL_RESERVATION
PARTIAL_RESERVATION
NO_RESERVATION_CAPITAL
NO_RESERVATION_PORTFOLIO_RISK
NO_RESERVATION_MARKET_CEILING
NO_RESERVATION_CORRELATION
NO_RESERVATION_EXPIRED
NO_RESERVATION_REVALIDATION_FAILED
```

A no-reservation candidate does not loop/retry within the same competition boundary.

It may be re-evaluated only on a later normal strategy decision boundary with fresh market/data/risk evidence and a new/superseding TradeProposal identity as required by strategy policy.

## 15. Reservation Expiry / Release

A winning reservation follows T-LSA-08/T-LSA-09 lifecycle.

If order intent is not validly created before reservation expiry:

- release reservation;
- preserve competition/outcome evidence;
- do not automatically give the freed capital to an old loser from the already-closed boundary.

Fresh opportunity evaluation is required.

## 16. Evidence

Persist `CapitalCompetitionRecord`:

```text
CompetitionId
BoundaryStart/End
CapitalSnapshotId
MarketAllocationProfileVersion
RiskPolicyVersion
CandidateIds in canonical arrival set
Subscores and CapitalPriorityScore
Ranking order
Winner iteration state deltas
ReservationIds/quantities
No-reservation reason codes
Correlation/concentration recomputations
EvidenceRefs[]
```

This record participates in the immutable provenance graph defined separately.

## 17. Interaction With Internal Resource Allocation Market Concept

The word "market" here means bounded deterministic competition for **new Trading capital** by proven strategy efficiency.

It does not imply:

- auction currency/pricing;
- autonomous strategy ownership of cash;
- transfer of capital between Applications;
- strategy preemption of existing positions;
- Risk/Guardian bypass;
- self-expanding strategy budget.

T-LSA-08 remains authoritative owner of reservations; T-LSA-07 remains Risk authority.

## 18. Verification Families

Verifier SHALL cover:

1. competition only when scarce capacity exists;
2. exact 250ms half-open boundary;
3. no future score input;
4. hard exclusions before scoring;
5. exact six subscore formulas/weights;
6. deterministic tie order;
7. winner iteration with atomic reservation;
8. original Risk ceiling never increased;
9. partial award only when explicitly viable;
10. no pro-rata invention;
11. existing obligations never preempted by score;
12. correlation ceiling updated after prior winners;
13. no recursive re-ranking feedback;
14. market allocation boundaries preserved;
15. loser not retried within same boundary;
16. expired winning reservation does not auto-award old loser;
17. concurrent competition boundaries cannot double reserve capital;
18. same snapshot/replay yields same winners/reservations.

## 19. Finding Disposition

```text
AC-CAPITAL-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
INTERNAL_STRATEGY_CAPITAL_COMPETITION = EXACT v1
RISK_FIRST = TRUE
ATOMIC_RESERVATION_OWNER = T-LSA-08
EXISTING_OBLIGATION_PREEMPTION_BY_SCORE = FORBIDDEN
```
