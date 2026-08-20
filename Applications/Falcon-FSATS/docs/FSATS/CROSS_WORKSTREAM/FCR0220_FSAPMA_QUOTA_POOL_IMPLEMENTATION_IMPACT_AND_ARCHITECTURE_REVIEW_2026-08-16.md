# FCR-0220 FSAPMA Quota-Pool Implementation Impact and Architecture/Consistency Review

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Implementation checkpoint:** `8811738a83d4740b8b17cb0797602d21e8bf0fc9`  
**Review type:** post-change static Architecture / Consistency / closed-baseline impact review  
**Status:** `STATIC_PASS / EXECUTABLE_VALIDATION_BLOCKED_BY_GITHUB_ACTIONS_BILLING`

## 1. Scope

This review covers only the Application-owned FSAPMA quota-accounting hardening introduced after the FCR-0220 source/code audit finding.

Changed Application source:

- `applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Domain/ProviderDomain.cs`
- `applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/QuotaPoolAdversarialChecks.cs`

No Foundation-owned source and no Shared Web-owned source was changed.

## 2. Finding being remediated

The previous quota ledger keyed available quota directly by complete `ProviderRouteIdentity`. That route identity intentionally contains provider/account/API-instance/endpoint/credential-reference dimensions.

That was insufficient for FCR-0220 because provider-enforced quota truth can sit above or across multiple route identities. Different API keys, accounts, endpoints or route identities therefore must not automatically manufacture independent effective capacity.

Controlling semantics remain:

```text
FSAPMA_EFFECTIVE_CAPACITY
=
SUM(LEGITIMATELY_INDEPENDENT_AVAILABLE_QUOTA_POOLS)

MULTIPLE_CREDENTIALS != AUTOMATIC_MULTIPLIED_CAPACITY
MULTIPLE_ACCOUNTS != AUTOMATIC_MULTIPLIED_CAPACITY
DIFFERENT_API_KEY != GUARANTEED_INDEPENDENT_QUOTA
SHARED_UPSTREAM_POOL -> COUNT_ONCE
UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY
```

## 3. Implemented architecture

The quota ledger now separates provider route identity from upstream quota-pool identity.

```text
ProviderRouteIdentity
        |
        +--> governed explicit ProviderQuotaPoolId, when authoritative quota scope is known
        |
        +--> conservative UNKNOWN_PROVIDER_SCOPE:<PROVIDER>, when quota scope is not known

ProviderQuotaPoolId
        |
        +--> one atomic remaining-unit counter per governed pool
```

Multiple routes may bind to one pool. Proven independent routes may bind to separate pools. Unknown scope does not receive an independence assumption.

`QuotaLedger` retains its historical `SetWindow(route, remaining)` source shape for compatibility, but an unbound route now resolves to the conservative unknown-provider pool instead of using full route identity as a capacity key.

## 4. Shared Web fallback coordination

The ledger also exposes an Application-owned FSAPMA half-ceiling operation for a quota pool that has already been governed as an actual constrained pool shared with Shared Web:

```text
FSAPMA_MAX_UNITS = floor(UPSTREAM_AVAILABLE_UNITS / 2)
```

This preserves the FCR-0220 rule that the 50/50 split is only a fallback for the exact real shared constrained pool. It is not a default provider rule.

The ledger does not decide whether Shared Web has a suitable independent source, whether a pool is constrained, or whether Web and FSAPMA actually share that pool. Those are governed onboarding/binding facts and remain outside this low-level counter.

## 5. Atomicity and concurrency

Consumption and window updates use the existing ledger lock. The atomic unit is now the resolved quota pool rather than the route identity.

Therefore two distinct FSAPMA routes bound to the same provider-enforced pool cannot consume the same remaining capacity independently.

```text
ATOMIC_RESERVATION_PER_POOL = SATISFIED_BY_LEDGER_LOCK
```

## 6. Multiple provider-enforced dimensions

FCR-0220 requires independent evaluation of each real constrained dimension, for example minute credits, daily credits, burst budgets, WebSocket connections or subscriptions.

The implementation does not hard-code provider-specific dimension names. Each governed constrained dimension can be represented by its own `ProviderQuotaPoolId`. A route may be governed against the appropriate pool instance for the resource being reserved.

No claim is made here that future provider onboarding/binding is complete. Authoritative pool identity, scope, limit, reset window and current provider terms remain future binding evidence.

## 7. Closed-baseline impact

This is an additive safety hardening to Application-owned source after a cross-workstream quota finding. It does not change the accepted Application architecture, ownership boundary, awareness topology, market semantics, provider authority model or runtime authority ceiling.

```text
PART0_PART7_REOPEN_REQUIRED = NO
PART8_REOPEN_REQUIRED = NO
PART8_RUNTIME_AUTHORITY_CHANGED = NO
PROVIDER_CONNECTIVITY_AUTHORIZED = NO
CREDENTIAL_RUNTIME_AUTHORIZED = NO
BROKER_CONNECTIVITY_AUTHORIZED = NO
DEPLOYMENT_AUTHORIZED = NO
```

Historical source compatibility is retained through `SetWindow(route, remaining)`.

## 8. Architecture/consistency checks

Static review result:

- Application-only ownership boundary: `PASS`
- route identity remains distinct from quota identity: `PASS`
- multiple credentials/accounts do not automatically multiply capacity: `PASS`
- shared upstream pool counts once: `PASS`
- unknown quota scope fails conservatively: `PASS`
- explicitly proven independent pools remain possible: `PASS`
- route-to-pool rebinding fails closed: `PASS`
- atomic per-pool consumption preserved: `PASS`
- shared constrained pool FSAPMA ceiling uses floor-half: `PASS`
- odd discrete remainder remains unallocated by FSAPMA: `PASS`
- Web/FSAPMA authority separation preserved: `PASS`
- provider/runtime authority remains false: `PASS`

Open static architecture findings: `0`

## 9. Executable validation status

GitHub Actions runs for the implementation did not start the runner. GitHub reported:

`The job was not started because recent account payments have failed or your spending limit needs to be increased.`

The downstream build/test/verifier job was therefore skipped.

This is an infrastructure validation blocker, not a reported compilation or test failure. No executable PASS is claimed by this review.

```text
STATIC_ARCHITECTURE_CONSISTENCY = PASS
EXECUTABLE_BUILD_TEST_VERIFICATION = BLOCKED_BY_GITHUB_ACTIONS_BILLING
OWNER_ACCEPTANCE = NOT_INFERRED
```

## 10. Conclusion

The identified route-keyed quota-accounting architectural gap is statically remediated at the Application source level without creating new runtime authority or crossing workstream ownership boundaries.

Final executable closure of the code finding remains contingent on a governed build/test/verifier run when executable validation infrastructure is available.
