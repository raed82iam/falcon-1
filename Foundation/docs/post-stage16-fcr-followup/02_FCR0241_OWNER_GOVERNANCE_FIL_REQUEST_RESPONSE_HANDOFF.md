# FCR-0241 Owner Governance FIL Request/Response Transport — AUDIT REMEDIATION RECORD

Date: 2026-08-18
Branch: `foundation-development`
Scope: Post-Stage16 governed Foundation follow-up. No Stage 17 is created and no closed Stage is reopened.

> **HANDOFF STATUS: SUSPENDED PENDING REMEDIATION VERIFICATION**
>
> The previous exact tested candidate `b7afc10b69f81c37938457cb3424e49756ab1532` remains valid historical evidence for the checks that were actually executed, but a fresh Foundation audit found material scenarios that the old 47/47 test set did not cover. Therefore that candidate is no longer the final authoritative FCR-0241 transport candidate and the previous Web handoff is suspended until the remediated candidate passes exact executable validation and fresh Red Team.

## Fresh audit findings

The 2026-08-18 Foundation audit confirmed the following gaps:

1. transport freshness constrained TTL duration but did not bind request/response acceptance to a current observation time;
2. response construction did not require the originally accepted request to remain current;
3. standing Owner policy mutation and Owner rollback-order requests could remain structurally valid after their own expiry when other authority evidence was still current;
4. malformed public transport profiles could reach profile-identity dereference instead of a deterministic fail-closed decision;
5. deterministic FCR-0241 verification compared PASS transcript output instead of deterministic envelope/decision identities;
6. delivery retry idempotency metadata was not explicitly bound to the canonical envelope idempotency identity;
7. Foundation CI built many verifier projects but executed only a subset;
8. baseline security scan coverage did not include GitHub workflows, scripts and Markdown documentation.

The branch-protection setting is a separate repository-hosting governance control and remains outside the code remediation record. It must not be confused with runtime correctness.

## Remediation implemented in source

### Observation-time-bound transport freshness

`src/Foundation.Contracts/PublicRuntimeRequestResponseTransport.cs` now binds request and response freshness to an explicit UTC observation time. For compatibility, callers may omit the optional observation parameter, in which case the transport uses `DateTimeOffset.UtcNow`; governed verifiers pass an explicit observation time to preserve deterministic execution.

A valid transport window now requires:

```text
createdAt <= observationTime < expiresAt
expiresAt > createdAt
expiresAt <= createdAt + profile TTL ceiling
all relevant times are UTC
```

Response construction additionally requires the accepted request envelope to remain current at the response observation time.

### Malformed-profile fail-closed behavior

The public transport validates all required schema, producer, recipient and authority references before profile hashing. Denied malformed profiles use a safe profile identity (`INVALID_PROFILE`) instead of relying on unsafe dereference during denial construction.

### Owner request freshness

The following Foundation authority surfaces now explicitly reject requests outside their own UTC validity windows:

- `StandingOwnerPolicyManagementService`
- `WebOwnerStandingPreapprovalEvaluator`
- `OwnerRollbackOrderEvaluator`

This keeps request freshness distinct from, and additional to, fresh underlying authority evidence.

### Delivery idempotency integrity

`src/Foundation.MessageDelivery/MessageDelivery.cs` now requires retry idempotency binding to match all of:

```text
binding.RouteDecisionId == route.DecisionId
binding.AdmissionDecisionId == admission.DecisionId
binding.IdempotencyIdentity == canonicalEnvelope.IdempotencyId.Value
```

### Verification hardening

`verification/Falcon.Fcr0241.OwnerGovernanceTransport.Verifier/` now covers:

- stale-by-observation requests;
- response creation after the accepted request expires;
- malformed public profile references failing closed without exception;
- caller-created structurally valid non-canonical profiles;
- revoked/superseded/wrong-kind/unstable-idempotency profiles;
- stale Owner policy mutation requests;
- stale Owner rollback orders;
- stale Owner preapproval proposals;
- deterministic canonical profile/request/response envelope identities, with a real SHA-256 digest emitted for deterministic material.

Additional governed verification surfaces:

- `verification/Falcon.MessageDeliveryIntegrity.Verifier/`
- `verification/Falcon.RepositorySecuritySurface.Verifier/`

The repository security verifier expands governed scanning to `src`, `tests`, `verification`, `.github`, `docs` and governed root configuration files, including `.yml`, `.yaml`, `.ps1`, `.sh` and `.md` surfaces.

### CI coverage hardening

`.github/workflows/foundation-ci.yml` now:

1. enforces the Foundation ownership boundary;
2. restores and Release-builds the controlled solution under exact SDK `10.0.302`;
3. executes architecture verification;
4. executes baseline security verification;
5. executes repository-wide security-surface verification;
6. discovers every verifier project under `verification/**` and executes every discovered governed verifier, failing if any verifier is skipped or returns non-zero.

## Canonical request/response families remain unchanged

### 1. Standing Owner policy management

- family: `foundation:owner-governance:standing-policy-management:v1`
- contract: `foundation/contracts/standing-owner-policy-management-request-response`
- request kind: `Command`
- request producer: `shared-web`
- request recipient: `foundation.owner-governance`
- response producer: `foundation.runtime`
- response recipient: `shared-web`

### 2. Standing Owner preapproval evaluation

- family: `foundation:owner-governance:standing-preapproval-evaluation:v1`
- contract: `foundation/contracts/standing-owner-preapproval-evaluation-request-response`
- request kind: `Query`
- request producer: `shared-web`
- request recipient: `foundation.owner-governance`
- response producer: `foundation.runtime`
- response recipient: `shared-web`

### 3. Owner rollback order

- family: `foundation:owner-governance:rollback-order:v1`
- contract: `foundation/contracts/owner-rollback-order-request-response`
- request kind: `Command`
- request producer: `shared-web`
- request recipient: `foundation.owner-governance`
- response producer: `foundation.runtime`
- response recipient: `shared-web`

## Authority separation remains unchanged

```text
WEB_OWNER_COMMAND_CENTER = ONLY_OWNER_DERIVED_DECISION_SURFACE
APPLICATION_AI_PROPOSAL != OWNER_DECISION
APPLICATION_AI_SELF_APPROVAL = FORBIDDEN
OWNER_SILENCE != OWNER_APPROVAL
AUTO_ACCEPT != EXECUTION_AUTHORITY
AUTO_ACCEPT != DEPLOYMENT_AUTHORITY
AUTO_ACCEPT != BUSINESS_AUTHORITY
ROLLBACK_REQUEST != ROLLBACK_AUTHORIZATION != ROLLBACK_EXECUTION
ROLLBACK_COMPLETED != AUTHORITY_RESTORED
FIL_ROUTE_AVAILABLE != ROUTE_ACTIVATED
ROUTE_AVAILABLE != ROUTE_AUTHORIZED != CONNECTION_EXECUTED
PLUG_AND_PLAY != IMPLICIT_TRUST
PUBLIC_RUNTIME_PROJECTION_TRANSPORT != OWNER_CONTROL_REQUEST_TRANSPORT
```

## Verification state

The remediation source has been committed, but this document deliberately does **not** claim executable PASS yet.

Required before renewed Web handoff:

```text
EXACT REMEDIATED CANDIDATE = PENDING
CONTROLLED RESTORE = PENDING
CONTROLLED RELEASE BUILD = PENDING
ARCHITECTURE VERIFICATION = PENDING
BASELINE SECURITY VERIFICATION = PENDING
REPOSITORY SECURITY SURFACE = PENDING
ALL GOVERNED VERIFIERS = PENDING
FCR-0241 HARDENED VERIFIER = PENDING
MESSAGE DELIVERY INTEGRITY VERIFIER = PENDING
DETERMINISTIC IDENTITY RERUN = PENDING
FRESH RED TEAM = PENDING
```

## Current disposition

```text
FOUNDATION_DISPOSITION = REMEDIATION_IMPLEMENTED_SOURCE / EXECUTABLE_VERIFICATION_REQUIRED
FCR0241_WAITING_ON = FOUNDATION
WEB_HANDOFF_EFFECTIVE = FALSE
LIVE_ROUTE_ACTIVATION = NOT_AUTHORIZED
DEPLOYMENT = NOT_AUTHORIZED
BUSINESS_EXECUTION_AUTHORITY = NOT_GRANTED
```

Only after the exact remediated candidate passes governed executable validation and fresh Red Team may this record be converted back to an effective Web handoff with an exact tested commit identity.