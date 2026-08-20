# Broad Red Team Review — FCR-0220 FSAPMA Quota-Pool Implementation

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Reviewed implementation checkpoint:** `8811738a83d4740b8b17cb0797602d21e8bf0fc9`  
**Architecture/impact review:** `FCR0220_FSAPMA_QUOTA_POOL_IMPLEMENTATION_IMPACT_AND_ARCHITECTURE_REVIEW_2026-08-16.md`  
**Status:** `STATIC_RED_TEAM_PASS / EXECUTABLE_VALIDATION_BLOCKED_BY_GITHUB_ACTIONS_BILLING`

## 1. Red Team objective

Attempt to defeat the FCR-0220 capacity semantics after the quota-pool remediation, with emphasis on accidental capacity multiplication, shared-pool bypass, unknown-scope optimism, cross-route races, artificial Web sharing, and authority escalation.

## 2. Attack cases

### RT-QP-01 — Different API keys manufacture capacity

Attack: create two complete routes for the same provider using different credential references and API-instance identities, then attempt to obtain two independent budgets without authoritative quota-pool evidence.

Result: `BLOCKED`.

Unbound routes resolve to the same conservative `UNKNOWN_PROVIDER_SCOPE:<PROVIDER>` pool. Different route identity alone does not create capacity independence.

### RT-QP-02 — Different provider accounts manufacture capacity

Attack: use different account IDs for the same provider while quota scope is unknown.

Result: `BLOCKED`.

Unknown scope remains one conservative provider pool until governed evidence explicitly establishes independent pools.

### RT-QP-03 — Same upstream pool consumed through multiple routes

Attack: explicitly bind two different routes to the same provider quota pool and consume through each.

Result: `BLOCKED`.

Both routes resolve to one shared remaining counter.

### RT-QP-04 — Race two consumers against the same last unit

Attack: exploit route separation to consume beyond a shared pool.

Result: `BLOCKED BY DESIGN`.

Pool resolution and decrement occur inside the ledger lock. The atomic reservation unit is the resolved pool, not route identity.

Executable concurrency stress remains pending because CI could not start.

### RT-QP-05 — Rebind a route after capacity setup

Attack: bind a route to one constrained pool, then move it to another pool to escape the first pool's remaining budget.

Result: `BLOCKED`.

A different second binding throws `PROVIDER_ROUTE_QUOTA_POOL_REBIND_FORBIDDEN`.

### RT-QP-06 — Collapse genuinely independent quota pools

Attack: verify that conservative failure behavior does not permanently prevent use of provider-authoritatively independent quotas.

Result: `PASS`.

Explicit governed pool bindings allow distinct independent pools. The implementation is conservative when scope is unknown, not permanently provider-global.

### RT-QP-07 — Exceed the Web/FSAPMA 50% fallback ceiling

Attack: apply a shared constrained pool of five discrete upstream units and attempt to grant FSAPMA three.

Result: `BLOCKED`.

`SetSharedWebFsapmaWindow` grants `floor(5 / 2) = 2`. The odd unit is left unallocated on the Application side.

### RT-QP-08 — Apply 50/50 to every provider merely because Web uses the same vendor

Attack: make the low-level ledger infer shared-Web status from provider name, URL, API vendor, credential, account or route identity.

Result: `BLOCKED BY ABSENCE OF SUCH INFERENCE`.

The ledger applies the half-ceiling only when the governed caller explicitly identifies the actual shared constrained pool. It does not infer Web sharing from provider name or URL.

### RT-QP-09 — Turn shared quota into shared authority/data ownership

Attack: use `ProviderQuotaPoolId` to merge Web and FSAPMA routes, credentials, operational data or business authority.

Result: `BLOCKED BY MODEL SEPARATION`.

The change only affects FSAPMA quota accounting. `ProviderRouteIdentity`, credentials, provider egress, Web presentation data and FSATS operational data remain separate.

### RT-QP-10 — Unknown pool means unlimited

Attack: omit authoritative quota-pool evidence and rely on unknown state to gain capacity.

Result: `BLOCKED`.

Unknown scope does not create an independent pool per route. No remaining budget exists unless a window is explicitly established.

### RT-QP-11 — Multiple constrained dimensions accidentally summed as one universal capacity claim

Attack: treat minute credits, daily credits, burst limits, session limits and subscription limits as interchangeable.

Result: `NO CURRENT DEFECT FOUND`.

The implementation does not hard-code dimension semantics. Separate real constrained dimensions can be represented as separate governed pool IDs. Future provider onboarding must still bind each applicable dimension from authoritative provider evidence and enforce all relevant constraints. This implementation does not claim onboarding/runtime binding is complete.

### RT-QP-12 — Runtime authority smuggling

Attack: interpret quota configuration/hardening as authority to connect to a provider, load credentials, deploy, or execute trading.

Result: `BLOCKED BY GOVERNANCE / NOT IMPLEMENTED`.

No provider egress binding, credential runtime/storage, broker binding, Paper/Shadow/Tiny-Live/Live activation or deployment authority was added.

## 3. Findings

```text
CRITICAL = 0
HIGH     = 0
MEDIUM   = 0
LOW      = 0
```

One validation limitation remains and is not classified as a code finding:

```text
EXECUTABLE_VALIDATION = BLOCKED_BY_GITHUB_ACTIONS_BILLING
```

GitHub Actions did not start the job due to the repository account payment/spending-limit condition. Therefore compilation, executable behavior and concurrency verification are not claimed as passed.

## 4. Governance impact

```text
CLOSED_PART_REOPEN_REQUIRED = NO
PART8_REMEDIATION_REQUIRED = NO
RUNTIME_AUTHORITY_CREATED = NO
WEB_WRITE_PERFORMED = NO
FOUNDATION_WRITE_PERFORMED = NO
OWNER_ACCEPTANCE = NOT_INFERRED
```

FCR-0220 remains `Waiting On: WEB` for its existing Web-owned governed verification/binding obligation. The Application hardening does not change that lifecycle handoff.

## 5. Red Team conclusion

Static Red Team found no unresolved code/design defect in the quota-pool remediation against the examined FCR-0220 semantics.

The code finding is **statically remediated**, but it must not be declared executable-verified or finally closed until a governed build/test/verifier execution is available and passes.
