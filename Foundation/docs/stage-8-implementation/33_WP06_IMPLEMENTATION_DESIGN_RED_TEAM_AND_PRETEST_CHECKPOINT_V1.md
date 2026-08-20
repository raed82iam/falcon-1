# Stage 8 WP-06 Implementation Design, Red-Team and Pre-Test Checkpoint V1

Date: 2026-08-15
Workstream: Falcon Foundation
Branch: `foundation-development`
WP: Stage 8 WP-06 — Durable Restriction Persistence, Restart Reconstruction and Containment Fencing

## Governing FCR Mapping

FCR-0076 and FCR-0082 remain `Waiting On: FOUNDATION` and explicitly assign restart-resistant protective restriction persistence/fencing to Stage 8 WP-06.

This WP does not close either FCR. It implements only the WP-06 Foundation portion within the already authorized Stage 8 sequence.

## Runtime Design

WP-06 adds `GuardianRestrictionPersistence` in `Foundation.Guardian`.

The implementation provides:

- deterministic, versioned persisted restriction snapshots;
- exact binding to Guardian protective decision identity and protective restriction identity;
- atomic write-through persistence through a temporary file and replacement move;
- restart reconstruction that re-validates the persisted decision/restriction pair;
- canonical CON-011 re-publication after restart;
- containment fencing after successful reconstruction;
- fail-closed behavior when persisted state is missing, unreadable, malformed, structurally inconsistent or identity-mismatched;
- review-deadline behavior that preserves enforcement and changes the status to review-required rather than releasing the subject.

Mandatory invariants:

`RESTART != RELEASE`

`RESTART != TRUST_RESTORATION`

`REVIEW_DEADLINE != RELEASE`

`MISSING_OR_UNTRUSTED_PERSISTED_RESTRICTION != PERMISSION`

`RECONSTRUCTED_RESTRICTION -> CONTAINMENT_FENCE_REQUIRED`

## Integrity Model Boundary

The snapshot identity is a deterministic SHA-256 structural integrity binding over the snapshot version, capture time, Guardian decision identity, Guardian restriction identity, restart-persistence requirement and self-release prohibition.

This WP does **not** claim that an untrusted actor with arbitrary write access and the ability to recompute all snapshot content is defeated by a bare digest. Hostile-storage authentication, key custody or cryptographic signing are not silently invented here. Missing, malformed and ordinary mutation/tampering are detected and fail closed; stronger storage-authentication controls remain subject to their governing security architecture where required.

## Integration

On successful restart reconstruction, the persisted restriction is republished through the existing canonical `GuardianRestrictionContractPublisher` as CON-011 `IMPOSED` state.

The executable verifier then proves that the reconstructed contract:

- continues to constrain Authority through `ProtectiveRestrictionAuthorityEnforcer`;
- continues to constrain Lifecycle through `ProtectiveLifecycleEnforcer`;
- does not permit new execution;
- does not produce recovery, release or trust-restoration authority.

## Executable Verification Coverage

The WP-06 verifier covers 28 checks, including:

- format/identity binding;
- deterministic serialization;
- real durable file creation;
- atomic temp-file cleanup;
- successful restart reconstruction;
- preserved decision/restriction identities;
- CON-011 canonical validation and `IMPOSED` result;
- Authority denial after restart;
- Lifecycle restriction after restart;
- review-deadline persistence;
- missing persistence fail closed;
- mutated persisted bytes fail closed;
- same governed identity after atomic rewrite;
- refusal of source-decision/restriction mismatch;
- explicit restart-persistence and no-self-release flags.

## Pre-Executable Architecture / Consistency / Red-Team

Challenges considered:

1. Restart silently clears restriction.
2. Restart fabricates trusted operation.
3. Review deadline is interpreted as automatic release.
4. Missing persistence is interpreted as no restriction.
5. Corrupted persisted state is accepted.
6. Source decision and restriction are mixed across identities.
7. Reconstructed restriction is not propagated to Authority.
8. Reconstructed restriction is not propagated to Lifecycle.
9. Temporary persistence file is left as an ambiguous competing state.
10. Path/storage errors escape the fail-closed result path.
11. WP-06 accidentally implements Stage 9 recovery/release.
12. Structural digest is overclaimed as hostile-storage authentication.

Result:

- Critical: 0
- High: 0
- Medium: 0
- Product-Low: 0

No open pre-executable blocker remains.

## Authority Boundary

WP-06 creates no Stage 9 recovery authority, release authority, trust-restoration authority, Application business authority, FSA-specific governance authority or deployment authority.

`WP06_IMPLEMENTATION = READY_FOR_EXACT_EXECUTABLE_VALIDATION`

`STAGE9_RECOVERY_RELEASE = NOT_IMPLEMENTED`

`OWNER_FINAL_STAGE8_CLOSURE = NOT_REQUESTED_AT_THIS_CHECKPOINT`
