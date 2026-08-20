# FSATS SIA v0.1 R3 — Fresh Architecture and Consistency Review

**Review Type:** `FRESH STATIC ARCHITECTURE / CONSISTENCY REVIEW`
**Reviewed Freeze:** `FSATS-SIA-v0.1-R3`
**Freeze Manifest:** `00_CURRENT_SIA_MASTER_AND_SEMANTIC_FREEZE_R3.md`
**Status:** `FAIL / SEMANTIC COMPLETENESS REMEDIATION REQUIRED`
**Owner Review Eligible:** `NO`
**Implementation Authority:** `NOT_GRANTED`

## 1. Review Restart Rule

R3 was reviewed from scratch after R2 Red-Team semantic remediation. No R2 PASS was inherited.

The R2 findings for Data Products, FSTSimA randomness, strategy statistics, Guardian directive parameters and base-currency policy are structurally reconciled in R3.

However, the fresh A/C review found remaining algorithmic non-ambiguity issues that existed across earlier files but were not exposed by the prior narrower reviews.

## 2. AC-ALG-001 — Universe Ranker Weights Are Exact But Subscore Functions Are Not

**Severity:** `HIGH`
**Status:** `OPEN`

### Finding

T-LSA-02 specifies an exact weighted score:

```text
25 liquidity
20 volume
15 spread/execution
15 opportunity density
10 data quality
10 volatility tradability
5 diversification
```

but the SIA still permits materially different implementations of the component scores.

Examples:

- liquidity could mean dollar volume, depth, turnover or a mixture;
- volume could be current z-score or long-run baseline;
- opportunity density could count signals, strategies or historical events;
- volatility tradability could reward high/low/middle volatility differently;
- diversification could use sector, correlation or portfolio holdings.

Two implementations can therefore select different Top-10 zone instruments from the same data.

### Required remediation

Define exact initial lookbacks, formulas, normalization/ranks, minimum sample, missing-data behavior and deterministic tie rules for every universe subscore.

## 3. AC-ALG-002 — Market Capital Fitness / Dynamic Allocation Is Not Fully Deterministic

**Severity:** `HIGH`
**Status:** `OPEN`

### Finding

07A correctly makes initial target 50/50 and bounds normal dynamic allocation 25/75, but `MarketCapitalFitness` component meanings and the allocation reevaluation schedule/hysteresis are not exact enough.

Without exact semantics, implementations can:

- shift capital at different times;
- compute opportunity density differently;
- oscillate between markets;
- use different performance windows;
- grant diversification credit differently.

### Required remediation

Define exact allocation epoch, input snapshot, subscore formulas, no-op/hysteresis threshold, clamping and unused-capital behavior.

## 4. AC-PMA-001 — Provider Route Weighted Score Has Materially Undefined Subscore Normalization

**Severity:** `HIGH`
**Status:** `OPEN`

### Finding

P-LSA-04 defines weights:

```text
30 quality
20 freshness/latency
20 quota headroom
15 reliability
10 cost efficiency
5 continuity
```

but initial formulas that transform route observations/certification into these 0..10000 subscores are incomplete.

This can change provider selection/failover under identical provider state.

### Required remediation

Define exact quality input, latency normalization, quota-headroom ratio, reliability mapping, cost normalization, continuity penalty and tie-break/failover stability.

## 5. AC-SIM-001 — Digest-to-xoshiro State Mixing Wording Allows Two Seed Initializations

**Severity:** `MEDIUM`
**Status:** `OPEN`

### Finding

10A says the four SHA-256 digest words are each "mixed through SplitMix64" to initialize xoshiro state, but does not explicitly state whether SplitMix64 state is independent per digest word or chained across words.

Golden vectors would eventually select one, but allowing the reference implementation to make the choice would still be semantic invention.

### Required remediation

Define exact initial state algorithm in prose/pseudocode before golden vector generation.

## 6. R3 Remediation Subjects That Passed A/C

The fresh R3 review found no architecture conflict in:

- 10 canonical Data Products and product quality/state models;
- Data Quality applicability and source-comparability separation;
- USD-only initial Risk base/quote policy;
- exact Guardian action parameter schemas;
- exact HNT-004/HNT-006 statistical primitives and NetEdge estimator;
- FSTSimA selected PRNG/distribution family except the seed-initialization wording above;
- accepted 43 contract preservation;
- APP-RSC candidate boundary;
- Risk cash-flow/time/tail integration;
- Awareness/research/FSA boundaries;
- persistence/concurrency/runtime/overload architecture.

## 7. Severity Summary

| ID | Severity | Status |
|---|---|---|
| AC-ALG-001 | HIGH | OPEN |
| AC-ALG-002 | HIGH | OPEN |
| AC-PMA-001 | HIGH | OPEN |
| AC-SIM-001 | MEDIUM | OPEN |

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 3
OPEN_MEDIUM = 1
A_C_R3 = FAIL
RED_TEAM_R3_ELIGIBLE = NO
OWNER_REVIEW_ELIGIBLE = NO
```

## 8. Required Lifecycle

These are semantic changes. Therefore:

```text
REMEDIATE ALL R3 A/C FINDINGS
-> FREEZE R4
-> FRESH A/C R4
-> FRESH RED-TEAM R4
-> OWNER REVIEW only if unchanged R4 passes both
```

## 9. Final Disposition

```text
FSATS_SIA_v0.1_R3_ARCHITECTURE_CONSISTENCY = FAIL
SEMANTIC_REMEDIATION_REQUIRED = YES
OWNER_ACCEPTANCE = NOT_ELIGIBLE
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
```
