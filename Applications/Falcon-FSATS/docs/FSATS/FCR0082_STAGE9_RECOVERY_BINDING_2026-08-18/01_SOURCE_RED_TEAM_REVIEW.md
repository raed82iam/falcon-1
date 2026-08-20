# FCR-0082 Application Stage 9 Recovery Binding — Source Red Team

Date: 2026-08-18
Scope: Application-owned FCR-0082 consuming binding only
Base before implementation: `6c19aa0d31de3b4cce97ead5be0fc87ed2d863f1`
Source candidate reviewed: `4c2b465ccf46ce557386478b73bb2440ab39fe0d`
Foundation exact tested executable dependency: `30a01643723967985c0db6204ad627e531571aec`

## Red Team result

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 0
LOW = 0
SOURCE_RED_TEAM = PASS
EXECUTABLE_RED_TEAM = NOT_YET_CLAIMED
```

## Attack classes reviewed

### 1. Foundation identity substitution

Attempted substitutions covered Foundation candidate, route, recipient, artifact digest, evidence, provenance and source contract. The consumer fails closed on mismatch.

### 2. Stale/replayed/future projection

The consumer rejects explicit stale state, expiry and future observation timestamps. `ValidUntil` must be strictly later than `ObservedAt` and the evaluation time must remain within the current projection window.

### 3. Recovery-state contradiction

The consumer independently derives expected readiness, release authorization, release execution and reintroduction values from the accepted Stage 9 recovery-state vocabulary. Contradictory combinations are rejected.

### 4. Projection-identity mutation

The Application recomputes the Stage 9 recovery projection identity from the exact public field order, UTC round-trip timestamps and SHA-256 derivation. A mismatched projection identity fails closed.

### 5. Authority laundering

The consumer rejects any input claiming:

- release execution authority;
- lifecycle authority;
- business authority;
- runtime activation;
- live-route activation;
- deployment authority.

Observed `ReleaseAuthorization=Authorized` or `ReleaseExecution=Executed` remains observation of Foundation state only. The returned Application decision never mints runtime, route, deployment or business authority.

### 6. Stage 13 authority collapse

No FSA-specific Controlled Revival or Stage 13 authority is represented or inferred by the adapter. The Stage 9 consumer remains generic recovery/release projection consumption only.

### 7. Live transport activation confusion

The canonical FIL event profile is treated as an available contract identity, not proof that a live route is activated. `FIL_EVENT_PROFILE_AVAILABLE != LIVE_ROUTE_ACTIVATED` remains enforced.

## Cross-boundary review

Changed source remains entirely under `applications/**`.

No write was made to:

- `foundation-development`;
- `web-development`;
- `reference/fsats-v1.3-scratch`;
- `main`;
- `applications/shared/web/**`;
- `applications/FSATS/WORKSTREAM_RULES.md`.

## Remaining gate

A source Red Team cannot substitute for executable evidence. The exact Application candidate must still pass the governed restore/build/test/verifier sequence before `APPLICATION_VERIFIED` or FCR closure is claimed.
