# FSATS Part 9 — First Executable Validation Finding and Remediation

**Date:** 2026-08-16  
**Branch:** `application-development`  
**First tested candidate:** `068aa41321e3a73e19eb09d9429415ae92245dde`  
**Status:** `REMEDIATED / FRESH_EXECUTABLE_RETEST_REQUIRED`

## 1. First executable validation result

The isolated Owner-run Part 9 validation reached the exact expected candidate and passed:

- .NET SDK identity `10.0.302`;
- exact sparse Application checkout;
- restore;
- Release build;
- Architecture verifier;
- Security verifier.

Behavior verification then stopped at:

```text
H-01_PROVIDER_ACCOUNT_QUOTA_CROSS_CONSUMPTION
```

The failure originated in `BrokerAccountIsolationAdversarialChecks.ProviderAccountsHaveIndependentQuotaAndRouteTruth()` and not in the new Part 9 Digital City implementation.

## 2. Source-before-semantics reconciliation

Current FSAPMA `QuotaLedger` semantics intentionally implement the accepted FCR-0220 rule:

```text
MULTIPLE_CREDENTIALS != AUTOMATIC_MULTIPLIED_CAPACITY
MULTIPLE_ACCOUNTS != AUTOMATIC_MULTIPLIED_CAPACITY
UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY
```

When no governed upstream quota-pool binding exists, routes for the same provider conservatively resolve to one `UNKNOWN_PROVIDER_SCOPE:<PROVIDER>` pool.

Therefore the historical H-01 test assumption that two provider-account identities automatically own independent quota was stale.

## 3. Remediation

The regression test was reconciled instead of weakening production quota accounting.

It now verifies both required behaviors:

1. two routes for the same provider with unknown upstream quota scope resolve to the same conservative UNKNOWN pool, and consumption is shared;
2. two routes explicitly bound to distinct governed `ProviderQuotaPoolId` values remain consumption-isolated.

Wrong provider-account route outcome rejection remains unchanged.

## 4. Authority and scope

This remediation:

- modifies only `applications/**`;
- does not grant provider connectivity;
- does not grant broker connectivity;
- does not grant runtime activation;
- does not grant Paper, Shadow, Tiny-Live or Live authority;
- does not modify Foundation or Shared Web source;
- preserves FCR-0220 capacity semantics.

## 5. Required next step

Fresh exact executable validation is required on the new post-remediation candidate. No executable PASS is claimed by this record.
