# Shared Falcon Web — FCR-0225 Owner AI Emergency Red Team

Date: 2026-08-16  
Branch: `web-development`  
Scope: `applications/shared/web/**`  
Foundation dependency: Stage 13 / WP-01 `ACCEPTED_AND_CLOSED`

## Review target

This review challenges the new Shared Web Owner emergency-control consumer slice for FCR-0225 against the accepted Foundation Stage 13 / WP-01 behavior and the Web authority boundary.

Reviewed Web artifacts include:

- `src/core/ports/owner-ai-emergency-port.js`
- `src/features/owner-ai-emergency/owner-ai-emergency.js`
- `src/platform/navigation/routes.js`
- `src/composition/shell.js`
- `src/app.js`
- `tests/owner-ai-emergency-port.test.mjs`
- `tests/owner-ai-emergency.test.mjs`
- updated navigation/auth/runtime-family tests

## Foundation behavior preserved

The Web slice preserves the accepted Stage 13 WP-01 distinctions:

```text
WEB_UI != KILL_AUTHORITY
UI_CLICK != AUTHORIZATION
KILL_REQUEST != KILL_AUTHORIZATION
KILL_AUTHORIZATION != KILL_EXECUTION
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
AMBIGUOUS_OR_UNKNOWN_TARGET = FAIL_CLOSED_NO_WIDEN
GLOBAL_AI_KILL = ALL_REGISTERED_EXECUTABLE_AI
GLOBAL_AI_KILL != FALCON_SHUTDOWN
GLOBAL_AI_KILL -> FALCON_SAFE_CORE
TARGET_AI_COOPERATION_NOT_REQUIRED
RECOVERY != RELEASE
WEB_CANNOT_RELEASE_KILLED_AI
```

No Release, Revival, Restore-Trust, or recovery-execution control is exposed by this Web emergency surface.

## Findings challenged and disposition

### RT-WEB-0225-01 — Outcome payload could bypass canonical Web validation

Initial implementation allowed the feature to receive a `model.decision` presentation object directly.

Risk: a future runtime binding could accidentally present contradictory Foundation outcome data without first applying the Stage 13 WP-01 Web validator.

Remediation:

- `owner-ai-emergency.js` now calls `bindOwnerAiEmergencyDecision()` before rendering any decision;
- malformed or contradictory outcome payloads are suppressed fail-closed;
- the Owner sees an explicit malformed-outcome notice instead of an upgraded/fabricated state.

Disposition: `REMEDIATED_IN_SOURCE`.

### RT-WEB-0225-02 — Global Kill could be confused with Falcon shutdown

Challenge: Global AI Kill presentation must never imply full Falcon shutdown.

Controls:

- Global presentation explicitly says AI-only;
- validator requires `safeCorePreserved === true`;
- validator requires `falconShutdownAuthorized === false`;
- contradictory Global decisions are rejected;
- Safe Core operational continuity is shown separately.

Disposition: `PASS_AT_SOURCE_BOUNDARY`.

### RT-WEB-0225-03 — Unknown/ambiguous target could widen blast radius

Controls:

- exact target identity required;
- exact governed target scope required;
- current authoritative target state required when supplied;
- current authoritative blast radius required;
- duplicate/empty impacted identities rejected;
- denied decisions cannot carry impacted targets;
- ordinary actions cannot target `ALL_AI`;
- `GLOBAL_AI_KILL` requires explicit `ALL_AI` plus a non-empty current AI census.

Disposition: `PASS_AT_SOURCE_BOUNDARY`.

### RT-WEB-0225-04 — UI state could be mistaken for authorization or completion

Controls:

- request intent carries `webAuthorizationClaim:false` and `executionClaim:false`;
- accepted decisions default to `ACTION_ACCEPTED`, not `ACTION_COMPLETED`;
- `ACTION_COMPLETED` requires separate authoritative completion evidence;
- Owner route remains protected by authoritative Owner session policy;
- current application composition keeps transport unavailable and submission disabled.

Disposition: `PASS_AT_SOURCE_BOUNDARY`.

### RT-WEB-0225-05 — Kill surface could leak into Trading/customer UI

Controls:

- route is `owner-ai-emergency` and registered as `OWNER` surface;
- Owner navigation contains the emergency surface; customer navigation does not;
- `owner-*` auth policy requires authoritative `PROJECT_OWNER` session;
- no FSATS runtime or Trading business contract is imported.

Disposition: `PASS_AT_SOURCE_BOUNDARY`.

### RT-WEB-0225-06 — Runtime transport could be invented by Web

Current repository evidence does not publish an exact Web-consumable runtime endpoint/transport binding for the Foundation Kill Control Plane. The Web implementation therefore deliberately renders the emergency surface fail-closed with transport unavailable.

No `fetch`, `WebSocket`, Foundation-internal import, guessed URL, or invented route was added.

Disposition: `OPEN_GOVERNED_BINDING_BLOCKER / NOT_A_SOURCE_AUTHORITY_BYPASS`.

## Executable verification performed in available environment

A focused source-equivalent Node.js validation was run for the new emergency port/feature logic:

```text
owner-ai-emergency-port.js syntax = PASS
owner-ai-emergency.js syntax = PASS
focused targeted-request validation = PASS
accepted-vs-completed separation = PASS
Global Safe Core contradiction rejection = PASS
fail-closed unavailable UI = PASS
no release/revival control = PASS
```

This is not a full governed repository test run.

Exact full commands still not claimed:

```text
npm test
npm run check
```

The local execution environment still cannot resolve `github.com`, so no complete checkout-backed full-suite PASS is claimed.

## Final Red Team disposition

```text
CRITICAL = 0
HIGH = 0
MEDIUM_SOURCE_FINDINGS_OPEN = 0
LOW_SOURCE_FINDINGS_OPEN = 0
FCR_0225_WEB_SOURCE_BINDING = IMPLEMENTED_FAIL_CLOSED
FCR_0225_RUNTIME_TRANSPORT_BINDING = UNAVAILABLE_NOT_INVENTED
FCR_0225_FULL_GOVERNED_VERIFICATION = PENDING
FCR_0225_CLOSURE_ELIGIBILITY = NO
PRODUCTION_ACTIVATION = NOT_AUTHORIZED
```

The next required step is an exact governed Web-consumable runtime transport/binding contract plus complete checkout-backed Web verification. Shared Web must not invent either.
