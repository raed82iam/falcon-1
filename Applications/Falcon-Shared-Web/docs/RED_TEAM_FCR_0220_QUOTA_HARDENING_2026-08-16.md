# Shared Falcon Web — FCR-0220 Quota Hardening Red Team

Date: 2026-08-16  
Branch: `web-development`  
Scope: `applications/shared/web/**`

## Review target

Fresh adversarial review of Shared Web provider-capacity coordination under FCR-0220 after the Owner rule:

```text
WEB_INDEPENDENT_SOURCE = FIRST_CHOICE
50_50 = EXACT_SHARED_CONSTRAINED_POOL_FALLBACK_ONLY
UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY
MULTIPLE_ACCOUNTS != AUTOMATIC_MULTIPLIED_CAPACITY
DIFFERENT_API_KEY != GUARANTEED_INDEPENDENT_CAPACITY
```

Reviewed source/tests:

- `src/core/market-data-plan.js`
- `tests/market-data-plan-fcr0220.test.mjs`

## Finding RT-WEB-0220-01 — HIGH — unknown quota-pool identity could be interpreted as independent capacity

### Original behavior

When Web had no suitable independent presentation source, `decideQuotaCoordination()` compared `webQuotaPoolId` and `fsapmaQuotaPoolId` through a boolean `samePool` expression. If either pool identity was missing, `samePool` became false and the function returned:

```text
NO_SHARED_POOL / QUOTA_POOL_NOT_SHARED
```

That was not safe. Failure to prove equality is not evidence of independent upstream capacity.

It violated:

```text
UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY
```

### Remediation

If either exact pool identity is missing/empty, Web now returns:

```text
FAIL_CLOSED
WEB_MAX_SHARE = 0
FSAPMA_RESERVED_SHARE = 1
REASON = QUOTA_POOL_IDENTITY_UNKNOWN
```

Known-distinct exact pool identities still return `NO_SHARED_POOL` correctly.

Disposition: `REMEDIATED_IN_SOURCE`.

## Finding RT-WEB-0220-02 — MEDIUM — empty quota-dimension evidence could look like successful evaluation

### Original behavior

With no independent Web source and `dimensions=[]`, dimensional evaluation returned a normal empty `DIMENSIONAL_EVALUATION` result.

That could be misread as evidence that no provider-enforced capacity constraint existed, even though no quota evidence was supplied.

### Remediation

The current source returns:

```text
FAIL_CLOSED / QUOTA_DIMENSIONS_REQUIRED
```

unless a suitable independent Web source has already made shared-pool coordination irrelevant.

Disposition: `REMEDIATED_IN_SOURCE`.

## Finding RT-WEB-0220-03 — MEDIUM — missing/duplicate quota-dimension identity could permit ambiguous accounting

### Risk

Provider-enforced constraints are evaluated independently. A missing dimension identity cannot be audited, and duplicate dimension IDs could represent accidental double-counting or ambiguous capacity accounting.

### Remediation

```text
MISSING_DIMENSION_ID -> FAIL_CLOSED / QUOTA_DIMENSION_ID_REQUIRED
DUPLICATE_DIMENSION_ID -> FAIL_CLOSED / DUPLICATE_QUOTA_DIMENSION_ID
```

Disposition: `REMEDIATED_IN_SOURCE`.

## Preserved behaviors

The hardening does not change the accepted good paths:

- suitable independent Web presentation source means no FSAPMA quota sharing;
- known-distinct exact quota pools do not receive an artificial 50/50 split;
- exact same documented constrained pool with known limit receives a 50/50 hard ceiling;
- odd discrete limits allocate `floor(Q/2)` to each side and leave the remainder unused;
- unknown shared constrained limit remains fail-closed;
- Web does not enforce FSAPMA internals;
- Web display data never becomes FSATS operational input;
- no route, credential, connectivity, deployment, or Trading authority is created.

## Test artifacts added/strengthened

Adversarial test coverage now includes:

- unknown Web pool identity;
- unknown FSAPMA pool identity;
- both pool identities unknown;
- empty pool identity;
- exact known-distinct pools;
- exact known shared pool;
- unknown shared constrained limit;
- empty dimensions;
- unknown identity inside one dimension;
- missing dimension ID;
- duplicate dimension ID;
- multiple independent dimensions;
- odd discrete quota remainder.

## Verification boundary

These are source/test-artifact changes. Full checkout-backed governed verification is still not claimed:

```text
npm test = NOT_CLAIMED
npm run check = NOT_CLAIMED
live provider quota/runtime verification = NOT_CLAIMED
```

## Final Red Team disposition

```text
RT_WEB_0220_01 = REMEDIATED
RT_WEB_0220_02 = REMEDIATED
RT_WEB_0220_03 = REMEDIATED
CRITICAL_OPEN = 0
HIGH_OPEN = 0
MEDIUM_OPEN = 0
LOW_OPEN = 0
WEB_QUOTA_SOURCE_HARDENING = PASS_WITH_GOVERNED_EXECUTABLE_VERIFICATION_PENDING
FCR_0220_CLOSURE_ELIGIBILITY = NO
PROVIDER_CONNECTIVITY = NOT_AUTHORIZED
```
