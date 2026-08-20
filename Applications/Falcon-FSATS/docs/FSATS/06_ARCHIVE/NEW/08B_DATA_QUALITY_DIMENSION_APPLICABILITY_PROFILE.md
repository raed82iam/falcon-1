# FSATS SIA — Data Quality Dimension Applicability Profile v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN CANDIDATE / RT-DATA-001 HARDENING`
**Owner:** APP-PMA / P-LSA-05

## 1. Purpose

Make the 08A quality-score formula deterministic per DataProduct rather than leaving dimension applicability/weight redistribution to implementation preference.

Base weights from 08A:

```text
Freshness             30
Completeness          20
SchemaConsistency     20
CrossSourceConsistency15
ContinuityGapFitness  10
ProvenanceConfidence   5
TOTAL                 100
```

## 2. Applicability Matrix

Legend:

- `R` = required dimension;
- `C` = conditional when >=2 sufficiently independent current sources are actually available;
- `N` = not applicable for the product's normal quality score;
- `P` = profile-required multi-source mode: if a Guardian/provider profile explicitly requires multi-source confirmation, insufficient independent sources is a hard state and not weight redistribution.

| Product | Freshness | Complete | Schema | CrossSource | Continuity | Provenance |
|---|---|---|---|---|---|---|
| DP-001 Instrument Reference | R | R | R | C | N | R |
| DP-002 Session State | R | R | R | C | R | R |
| DP-003 Top Quote | R | R | R | C/P | R | R |
| DP-004 Trade Print | R | R | R | C/P | R | R |
| DP-005 Bar | R | R | R | C/P | R | R |
| DP-006 Order Book L2 | R | R | R | C/P | R | R |
| DP-007 Corporate Action | R | R | R | C/P | N | R |
| DP-008 Normalized News/Event | R | R | R | C/P | N | R |
| DP-009 Macro Series Point | R | R | R | C | N | R |
| DP-010 Fundamental Snapshot | R | R | R | C | N | R |

## 3. Conditional Cross-Source Rule

When CrossSource=`C` and fewer than two sufficiently independent current sources are available:

- CrossSourceConsistency is `NOT_APPLICABLE`;
- its weight 15 is redistributed proportionally among the other applicable dimensions;
- the observation may still be VALID if all product hard rules and resulting score pass;
- the provenance dimension records the actual source situation; it does not pretend corroboration exists.

When >=2 independent sources are available, CrossSourceConsistency becomes required for that observation/window and weight 15 is used normally.

## 4. Profile-Required Multi-Source Rule

When Guardian/provider policy activates `REQUIRE_MULTI_SOURCE_CONFIRMATION` for a product/scope:

```text
RequiredIndependentSourceCount >= 2
```

If the exact required count cannot be met:

```text
QualityState = UNAVAILABLE or DEGRADED/CONFLICTED according to the exact reason,
NEW_RISK_ELIGIBLE = NO
```

The CrossSource weight is **not** redistributed away to make the score pass.

Source independence uses P-LSA-05/P-LSA-06 certified upstream lineage, not provider brand count.

## 5. Continuity Applicability

Continuity/GapFitness is not scored for discrete reference/event products DP-001, DP-007, DP-008, DP-009, DP-010 under normal observation quality.

For streaming/series products DP-002 through DP-006, continuity covers exact sequence/time-window expectations:

- DP-002 session-state expected status freshness/transitions;
- DP-003 quote update gaps;
- DP-004 trade sequence/time gaps where source profile supports expectations;
- DP-005 expected completed-bar continuity;
- DP-006 book sequence integrity/snapshot lineage.

Hard sequence gaps that invalidate reconstruction override the score and produce INCOMPLETE/CONFLICTED.

## 6. Weight Redistribution Formula

Let `A` be the set of applicable dimensions after conditional applicability is resolved, and original weights `w_i`.

For each applicable dimension:

```text
AdjustedWeight_i = w_i / sum(w_j for j in A)
```

Final score:

```text
QualityScore = round_half_to_even(
  sum(AdjustedWeight_i * DimensionScore_i),
  integer 0..10000
)
```

Implementation may perform the weighted numerator with integer basis points to avoid binary rounding drift.

Example: DP-001 with no independent second source and Continuity=N:

Applicable original weights = 30+20+20+5 =75.

Adjusted weights:

```text
Freshness 40.0000%
Completeness 26.6667%
Schema 26.6667%
Provenance 6.6667%
```

The executable implementation should use exact rational/integer weighting rather than truncated percentages shown for explanation.

## 7. Dimension Score Rules

Each dimension returns 0..10000 or HARD_FAIL.

`HARD_FAIL` is not numeric zero; it sets the product's hard failure state according to product rules and prevents a high score in other dimensions from masking the failure.

Examples:

- crossed top quote -> HARD_FAIL/CONFLICTED;
- missing required bar -> HARD_FAIL/INCOMPLETE;
- stale beyond hard max age -> HARD_FAIL/STALE;
- invalid provenance/identity -> HARD_FAIL;
- sequence gap in reconstructed L2 -> HARD_FAIL/INCOMPLETE.

## 8. Freshness Score

When age is within the product hard maximum `MaxAge`:

```text
FreshnessScore = round(10000 * max(0, 1 - Age/MaxAge))
```

At `Age >= MaxAge`, hard state = STALE and score cannot rescue it.

For discrete effective-until-superseded products such as DP-001/007, the product-specific revalidation/effective interval is the MaxAge/effective rule.

## 9. Completeness Score

Required fields are hard binary requirements. Missing required field = HARD_FAIL.

Optional expected fields declared by exact provider/product mapping may reduce CompletenessScore according to that mapping, but cannot change canonical required/optional status.

If no optional completeness metric exists for the product mapping, CompletenessScore=10000 after required-field validation passes.

## 10. Schema/Internal Consistency Score

Canonical schema/range/invariant failure = HARD_FAIL.

When all hard invariants pass and no soft consistency warnings are defined, score=10000.

Any soft checks must be registered in the exact DataProduct quality profile version; coding worker cannot add ad-hoc penalties.

## 11. Cross-Source Consistency Score

For exact comparable observations from independent sources, profile defines comparison dimensions/tolerances.

Initial v1 generic price comparison for quote/trade/bar source validation:

```text
RelativePriceDiffBps = 10000 * abs(Pa-Pb) / max((Pa+Pb)/2, smallest_positive_price_unit)
```

Product comparison profile sets a tolerance based on market/data class. Missing a declared tolerance when multi-source is required => profile invalid/fail closed; implementation does not choose one.

For exact identity/reference/event disagreement (symbol/base-quote/corporate action/event retraction), incompatibility is categorical/hard conflict rather than averaging.

## 12. Provenance Score

Initial provenance class score:

```text
certified official/venue/agency/issuer source with intact lineage = 10000
certified licensed provider with known upstream lineage = 9000
certified derived/composite profile with complete lineage = 8500
unknown/unverifiable lineage = HARD_FAIL for operational product
```

A lower provenance score does not itself invalidate if the provider/profile is certified and product threshold passes; unknown/unverified lineage is not accepted operationally.

## 13. Trading Acceptance

As 08A states:

```text
CURRENT v1 NEW-RISK STRATEGY REQUIRED DATA PRODUCTS MUST BE QualityState=VALID.
```

Therefore a DEGRADED score cannot be accepted merely because it is >=6500 unless a future StrategyVersion explicitly defines a degraded-use rule.

## 14. Verification Families

Verifier SHALL test:

1. exact applicability matrix;
2. conditional second-source redistribution;
3. profile-required multi-source cannot redistribute weight;
4. upstream independence enforcement;
5. continuity N vs R behavior;
6. exact rational weight normalization;
7. hard fail not masked by other scores;
8. freshness boundary exactly at MaxAge;
9. required fields hard fail;
10. provenance unknown hard fail;
11. new-risk VALID-only rule.

## 15. Hardening Result

```text
RT-DATA-001_QUALITY_APPLICABILITY_RESIDUAL = CLOSED_AT_DESIGN_CANDIDATE_LEVEL
CODING_WORKER_DIMENSION_APPLICABILITY_DISCRETION = 0
```
