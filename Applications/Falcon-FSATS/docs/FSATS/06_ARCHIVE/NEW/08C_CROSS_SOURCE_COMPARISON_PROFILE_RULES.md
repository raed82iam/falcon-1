# FSATS SIA — Cross-Source Comparison Profile Rules v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN CANDIDATE / RT-DATA-001 HARDENING`
**Owner:** APP-PMA / P-LSA-05

## 1. Purpose

Prevent the Data Quality layer from treating two independent but semantically different market feeds as directly comparable with an invented tolerance.

## 2. Independence != Comparability

Two sources may be sufficiently independent for corroboration and still expose different scopes:

- different venues;
- exchange-only vs consolidated/composite feed;
- different trade-condition inclusion;
- different quote entitlement;
- different bar/session construction;
- different timestamp/latency characteristics.

Therefore:

```text
SOURCE_INDEPENDENT != SEMANTICALLY_COMPARABLE
```

Cross-source scoring requires both.

## 3. CrossSourceComparisonProfile

Every numeric cross-source comparison uses a versioned profile:

```text
CrossSourceComparisonProfileId
DataProductId/Version
MarketProfileVersion
SourceSemanticClassA
SourceSemanticClassB
ComparableFields[]
ComparisonMethod
AbsoluteTolerance?
RelativeToleranceBps?
TimeAlignmentTolerance?
RequiredSourceQualityFloor
Correction/LatenessWindow
HardConflictConditions[]
ScoreCurve
Evidence/CertificationRefs[]
```

No hardcoded generic tolerance inside P-LSA-05.

## 4. Applicability

CrossSourceConsistency is applicable only when:

```text
>=2 independent sources
AND exact compatible CrossSourceComparisonProfile exists
AND each source observation meets the profile quality/freshness floor
```

Otherwise, in ordinary non-required corroboration mode, the dimension is NOT_APPLICABLE and 08B weight redistribution applies.

If Guardian/provider policy explicitly requires multi-source confirmation and no valid comparison profile exists:

```text
MULTI_SOURCE_REQUIREMENT_UNSATISFIABLE
-> affected product new-risk eligibility = NO
```

The implementation does not invent a tolerance.

## 5. Exact Comparison Methods

Initial allowed methods:

```text
EXACT_IDENTITY_EQUAL
ABSOLUTE_DIFFERENCE
RELATIVE_DIFFERENCE_BPS
SET_EQUALITY
ORDERED_SEQUENCE_EQUAL
CATEGORY_COMPATIBILITY_MATRIX
```

A product comparison profile selects one per field.

No fuzzy string matching or AI judgment in canonical operational comparison v1.

## 6. Relative Price Difference

When a profile selects `RELATIVE_DIFFERENCE_BPS`:

```text
mid = (abs(A)+abs(B))/2
if mid <= 0 -> invalid numeric comparison
DiffBps = 10000 * abs(A-B) / mid
```

Then the exact profile's tolerance/score curve applies.

A coding worker SHALL NOT choose a default 10/20/50 bps tolerance.

## 7. Time Alignment

Observations are comparable only when their effective-time difference is within exact `TimeAlignmentTolerance` and neither is stale under its source/product profile.

For bars, exact BarStart/BarEnd/Interval must match unless the profile explicitly defines a time-normalization relationship.

For quotes/trades, later observation cannot be used to validate an earlier decision boundary retroactively.

## 8. Hard Identity Conflicts

The following are categorical and do not use numeric tolerance:

- InstrumentId/base/quote asset mismatch;
- incompatible market/venue identity represented as the same canonical product without certified mapping;
- corporate action split ratio/effective date conflict;
- event retracted by an authoritative source while another claims current;
- schema/version/unit mismatch;
- source provenance/digest conflict.

These produce CONFLICTED/RECONCILIATION_REQUIRED as defined by product profile.

## 9. Quote Scope Example

A venue-specific quote and a consolidated/composite quote are not assumed equal.

They may have a comparison profile that treats the venue quote as a bounded corroboration source, but:

```text
VENUE_QUOTE != NBBO/CONSOLIDATED_TRUTH
```

and a discrepancy beyond the profile tolerance does not automatically mean either source is corrupt; the profile's reason classification determines the outcome.

## 10. Bar Scope Example

Two 5-minute bars are comparable only when their `SourceAggregationProfile` confirms compatible:

- session inclusion;
- trade-condition filtering;
- adjustment state;
- interval boundaries;
- correction/lateness policy.

Otherwise cross-source bar equality is not scored as if they were the same product construction.

## 11. Certification Responsibility

CrossSourceComparisonProfile values/tolerances are part of provider/data-product certification evidence.

They may be changed only through a new profile version and revalidation. Current external provider facts are not immortal architecture constants.

## 12. Verification

Verifier SHALL reject:

1. cross-source score with no ComparisonProfileId;
2. independent-but-incomparable sources treated equal;
3. hardcoded implementation tolerance not present in profile;
4. future source used to validate prior decision;
5. bar interval/session mismatch;
6. venue-specific quote labeled consolidated truth;
7. required multi-source policy silently dropping comparison when profile absent;
8. profile version change without new evidence/digest.

## 13. Hardening Result

```text
CROSS_SOURCE_TOLERANCE_IMPLEMENTATION_DISCRETION = 0
SOURCE_INDEPENDENCE_AND_COMPARABILITY = SEPARATE
```
