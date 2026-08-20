# FCR-0076 — Recovery FIL Runtime Binding Checkpoint

Date: 2026-08-17  
Branch: `web-development`  
Writable scope: `applications/shared/web/**` only  
Foundation exact governed executable candidate: `00fb65a4e5ddf89ff053ed30804f6858efcf6dba`

## Purpose

Record the exact Shared Web consuming-side binding for the Foundation Recovery public runtime projection delivered for FCR-0076.

This checkpoint does not activate a live Service Bus route, deployment, release execution, lifecycle transition, recovery action, or Foundation/business authority.

## Canonical Foundation profile consumed

```text
route_identity = route:foundation:recovery:web:v1
message_type = Foundation.Operational.RecoveryProjection
schema_identity = foundation.operational.recovery
contract_version = 1.0.0
producer = foundation.runtime
recipient_scope = shared-web
message_kind = Event
classification = Operational
transport_authority = authority:transport:projection-only
artifact_id = foundation/runtime-projection/recovery
compatibility_identity = compat:foundation-public-runtime-projection:v1
```

Foundation source/evidence consumed:

- `src/Foundation.Contracts/PublicRuntimeProjectionProfiles.cs`
- `src/Foundation.Contracts/PublicRuntimeProjectionTransport.cs`
- `src/Foundation.ArtifactPublication/RecoveryOperationalProjection.cs`
- `verification/Falcon.PublicRuntimeProjection.Verifier/Program.cs`
- Foundation FCR-0076 handoff and exact governed executable candidate `00fb65a4e5ddf89ff053ed30804f6858efcf6dba`.

## Web implementation

### Canonical FIL consumer

`src/adapters/foundation-fil-public-runtime-v1.js`

The previous identity-only implementation was generalized into a stage-neutral Web consumer for accepted Foundation public runtime projection profiles while preserving the exact Stage 16 identity behavior.

The recovery profile is now validated for:

- exact transport decision acceptance;
- no activation, execution or business authority;
- exact route/message/schema/version/producer/recipient/classification/transport-authority identity;
- canonical message/correlation/causation/idempotency/delivery/retry lineage fields;
- current message window;
- exact payload SHA-256;
- exact artifact id/version/digest/evidence/provenance/compatibility;
- deterministic reconstruction of the Foundation `PublicRuntimeProjectionBinding.BindingIdentity`;
- exact envelope provenance `projection-binding:<binding identity>`;
- fail-closed JSON projection parsing.

### Exact RecoveryOperationalProjection adapter

`src/adapters/foundation-stage9-recovery-release-v1.js`

The existing historical presentation adapter remains available for already-verified Web fixtures. The new exact consuming path additionally validates the Foundation `RecoveryOperationalProjection` contract including:

- projection identity;
- recovery case identity;
- exact governed recovery state;
- restoration outcome;
- `ReadyForReleaseDecision` consistency;
- release authorization consistency;
- release execution consistency;
- reintroduction consistency;
- lifecycle and evidence identity;
- observed/valid-until timestamps;
- complete/partial truth;
- current/stale truth without stale-to-current upgrade;
- `PresentationOnly = true`;
- `CarriesReleaseExecutionAuthority = false`;
- `CarriesLifecycleAuthority = false`;
- `CarriesBusinessAuthority = false`.

The Web projection always preserves:

```text
mayAuthorizeRelease = false
mayExecuteRelease = false
mayChangeLifecycle = false
businessAuthorityGranted = false
```

### Stable Web runtime port

`src/core/ports/falcon-system-runtime-port.js`

Added Web-owned runtime method:

`recoveryOperational(reference)`

`src/adapters/foundation-runtime-port-v1.js`

Binds the exact recovery consumer behind the stable aggregate Web runtime port. Presentation code does not import Foundation internals or transport details.

This keeps the integration plug-and-play:

```text
Foundation authoritative recovery truth
-> Foundation public runtime projection
-> Foundation canonical FIL transport decision
-> Web canonical FIL consumer
-> Web exact RecoveryOperationalProjection adapter
-> Web-owned runtime port: recoveryOperational()
-> presentation / later Owner emergency UX
```

## Tests added/updated

`tests/foundation-stage9-recovery-release-v1.test.mjs`

Covers:

- no projection -> UNAVAILABLE;
- stale/partial preservation;
- exact canonical FIL acceptance;
- exact readiness/auth/execution/reintroduction separation;
- Foundation projection authority leakage rejection;
- route mutation rejection;
- recipient mutation rejection;
- payload digest mutation rejection;
- artifact digest mismatch rejection;
- transport-authority escalation rejection;
- Command substitution rejection;
- expired FIL message rejection;
- runtime adapter happy path and unavailable path.

`tests/foundation-runtime-port-v1.test.mjs`

Covers stable runtime-port composition and proves a missing recovery adapter is rejected instead of fabricating a transport.

`package.json`

Includes `src/adapters/foundation-runtime-port-v1.js` in the syntax gate.

## Targeted executable evidence

The current tool execution environment cannot resolve `github.com` for a complete checkout, and GitHub Actions currently reports zero workflow runs for `web-development`. Therefore a full checkout-backed current-HEAD `npm test` / `npm run check` PASS is not claimed.

The exact new canonicalization, recovery consumer and projection rules were executed in a local isolated Node harness after implementation:

```text
RECOVERY_FIL_BINDING_CANONICALIZATION = PASS
RECOVERY_STATE_AUTHORITY_SEPARATION = PASS
RECOVERY_PROJECTION_FAIL_CLOSED_PRECONDITIONS = PASS
RECOVERY_FIL_CONSUMER = PASS
RECOVERY_EXACT_PROJECTION = PASS
RECOVERY_MUTATION_FAIL_CLOSED = PASS
RECOVERY_AUTHORITY_SEPARATION = PASS
```

This is targeted executable evidence only. Full current-HEAD governed Web verification remains required before FCR closure.

## Red Team

Challenged:

1. route substitution;
2. recipient substitution;
3. message type/schema/version/producer/classification substitution;
4. Command/Query control-message injection;
5. payload mutation;
6. artifact digest/evidence/provenance/compatibility mutation;
7. forged binding identity;
8. forged envelope binding provenance;
9. expired transport message;
10. transport decision that claims activation/execution/business authority;
11. projection that claims release/lifecycle/business authority;
12. `ReadyForReleaseDecision` silently upgraded to authorization;
13. authorization silently upgraded to execution;
14. execution with inconsistent reintroduction state;
15. stale truth silently upgraded to current;
16. missing transport silently replaced by a local endpoint;
17. availability of the contract interpreted as live route/deployment authority.

Result:

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_LOW = 0
FCR0076_TARGETED_RED_TEAM = PASS
```

## Mandatory distinctions preserved

```text
REPAIR_SUCCESS != RELEASE
RESTART != RECOVERY
READY_FOR_RELEASE_DECISION != RELEASE
READY_FOR_RELEASE_DECISION != RELEASE_AUTHORIZATION
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
RELEASE_EXECUTION != LIFECYCLE_AUTHORITY
LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION
PUBLICATION != ACTIVATION
FIL_ENVELOPE_AVAILABLE != LIVE_SERVICE_BUS_ROUTE_ACTIVATED
ROUTE_AVAILABLE != ROUTE_AUTHORIZED != CONNECTION_EXECUTED
WEB_PRESENTATION != FOUNDATION_AUTHORITY
UI_CLICK != AUTHORIZATION
PLUG_AND_PLAY != IMPLICIT_TRUST
```

## Current disposition

```text
FCR0076_FOUNDATION_PORTION = COMPLETE_AND_GOVERNED_VERIFIED
FCR0076_WEB_EXACT_FIL_BINDING = IMPLEMENTED
FCR0076_WEB_RUNTIME_PORT_BINDING = IMPLEMENTED
FCR0076_WEB_TARGETED_EXECUTABLE_VERIFICATION = PASS
FCR0076_WEB_RED_TEAM = PASS
FCR0076_FULL_CURRENT_HEAD_WEB_SUITE = PENDING_ENVIRONMENT_CAPABILITY
FCR0076_WAITING_ON = WEB
FCR0076_CLOSURE = NOT_ELIGIBLE_YET
```

The remaining action is full current-HEAD governed Web verification on one exact checkout-backed candidate. No Foundation incompatibility is currently identified, so no new Foundation FCR is required by this checkpoint.
