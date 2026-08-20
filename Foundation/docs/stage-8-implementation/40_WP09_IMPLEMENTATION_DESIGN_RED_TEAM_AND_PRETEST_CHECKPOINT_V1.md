# Stage 8 WP-09 Implementation Design, Red Team and Pretest Checkpoint V1

**Stage:** 8 — Foundation Guardian, Protective Restriction and Platform Safe State  
**WP:** WP-09 — No-Self-Release, Release Preconditions & Stage-9 Recovery Handoff  
**Status:** IMPLEMENTED_AWAITING_EXECUTABLE_VALIDATION  
**Date:** 2026-08-15  
**Branch:** `foundation-development`

## Governing boundary

Stage 8 owns protection, restriction, isolation, Safe-State and containment.

WP-09 SHALL:

- prevent subject self-release;
- prevent Guardian self-release;
- prevent repair actor self-certification/release;
- preserve exact release conditions and declared release authority from the active restriction;
- record independently supplied recovery evidence and role separation;
- produce a deterministic Stage-9 recovery handoff;
- keep the restriction enforced while the handoff exists.

WP-09 SHALL NOT:

- execute repair;
- declare trust restored;
- release a restriction;
- perform Lifecycle reintroduction;
- issue the new post-recovery authority decision;
- implement Controlled Revival;
- implement FSA-specific investigation, Factory Reset or recovery governance.

## Governing verification sources

- Stage 8 implementation plan v0.1;
- VPL-006 Guardian Restriction;
- VPL-007 Controlled Recovery;
- FCR-0076;
- FCR-0082.

VPL-006 requires any self-release to fail and states that cleanup does not lift restriction. VPL-007 requires independent recovery validation, declared release authority, controlled Lifecycle reintroduction and a new authority decision before unrestricted authority returns.

## Production implementation

Added:

`src/Foundation.Authority/Stage9RecoveryHandoff.cs`

The file contains two intentionally separate surfaces.

### 1. Stage8ReleaseGuard

`Stage8ReleaseGuard` never returns an allowed release inside Stage 8.

It distinguishes:

- subject self-release -> `SUBJECT_SELF_RELEASE_DENIED`;
- Guardian self-release -> `GUARDIAN_SELF_RELEASE_DENIED`;
- every other actor, including declared release authority -> `STAGE9_RELEASE_REQUIRED`;
- malformed request -> fail closed.

Changing a role label does not bypass subject/Guardian identity checks.

### 2. Stage9RecoveryHandoffRuntime

`Stage9RecoveryHandoffRuntime` evaluates evidence for handoff readiness only.

It preserves:

- exact restriction identity;
- restriction integrity evidence;
- exact release conditions;
- exact declared release authority;
- independent verifier identity;
- recovery evidence identity;
- authoritative-state reconciliation requirement;
- security-context reestablishment requirement;
- dependency reconciliation requirement;
- independent recovery validation requirement;
- Guardian release-condition evidence requirement;
- residual-risk evidence;
- Lifecycle reintroduction requirement;
- new authority decision requirement.

A ready handoff means only:

`READY_FOR_STAGE9_EVALUATION`

It explicitly does not mean release.

Every handoff record has:

- `ReleaseEligibleInStage8 = false`;
- `RestrictionRemainsEnforced = true`;
- `IndependentRecoveryValidationRequired = true`;
- `AuthorizedReleaseDecisionRequired = true`;
- `LifecycleReintroductionRequired = true`;
- `NewAuthorityDecisionRequired = true`.

The handoff record constructor is internal to `Foundation.Authority`, preventing ordinary external callers from constructing a fabricated Stage-9-ready record.

## Role-separation enforcement

The independent verifier may not be:

- the restricted subject;
- the Guardian;
- the repair actor.

The declared release authority may not be:

- the restricted subject;
- the Guardian;
- the repair actor.

WP-09 does not invent a stronger separation rule between independent verifier and declared release authority because VPL-007 does not require them to be different identities in all deployments.

## Restriction expiry semantics

WP-09 does not interpret `RestrictionRecord.Expiry` as automatic release.

A review/expiry time may require re-evaluation, but the restriction remains enforced until Stage 9 independently validates recovery and performs governed authorized release.

Therefore:

`RESTRICTION_EXPIRY != RELEASE`

`TIME_PASSAGE != TRUST_RESTORATION`

## Red Team findings closed before executable test

### RT-01 — Stage-8 release API leakage

Risk: a legitimate-looking release authority could gain a direct `Release()` path while WP-09 was intended only to prepare Stage 9.

Disposition: no production `Release()`, recovery execution, trust-restoration or reintroduction API exists in the WP-09 runtime. `Stage8ReleaseGuard` denies every Stage-8 release attempt.

### RT-02 — role-label spoofing

Risk: subject or Guardian changes its role label to `Other` and attempts release.

Disposition: release guard checks both declared role and exact actor identity against subject/Guardian identities.

### RT-03 — repair actor self-certification

Risk: repair actor also acts as independent verifier or release authority.

Disposition: explicit role-separation checks fail closed.

### RT-04 — expiry becomes implicit release

Risk: a finite restriction expiry is treated as freedom to resume.

Disposition: handoff always preserves restriction and never becomes Stage-8 release-eligible, even after restriction review/expiry time.

### RT-05 — fabricated Stage-9-ready handoff

Risk: external caller constructs a ready handoff directly.

Disposition: handoff record constructor is internal to Foundation.Authority; runtime validation remains defensive.

### RT-06 — Stage-9 semantics leak backward

Risk: WP-09 implements recovery/reintroduction rather than merely recording prerequisites.

Disposition: runtime records requirements only. It does not reference Lifecycle runtime or Guardian runtime and does not execute recovery.

## Executable verifier

Added:

- `verification/Falcon.Stage8.WP09.Verifier/Falcon.Stage8.WP09.Verifier.csproj`
- `verification/Falcon.Stage8.WP09.Verifier/Program.cs`

Static verifier count:

`35 checks`

Coverage includes:

- valid Stage-9-ready handoff;
- Stage-8 release always denied;
- subject self-release;
- Guardian self-release;
- role-label bypass;
- repair actor self-certification;
- independent verifier separation;
- release-authority separation;
- failed independent recovery validation;
- incomplete state/security/dependency/Guardian evidence;
- subject/restriction mismatch;
- expired recovery evidence;
- restriction expiry not release;
- deterministic and mutation-sensitive handoff identity;
- no public handoff constructor;
- no Stage-9 execution method leakage;
- no Guardian runtime dependency;
- zero Application/trading business-semantic leakage.

## FCR continuity

FCR-0076 and FCR-0082 remain:

`Waiting On: FOUNDATION`

WP-09 cannot make either FCR closure-eligible by itself. WP-10 integrated verification remains required, and generic recovery/release/reintroduction remains Stage 9-owned.

## Pretest state

No executable PASS is claimed by this record.

The next action is exact isolated executable validation of the frozen candidate containing this checkpoint.

`STAGE8_WP09_IMPLEMENTATION = PRESENT`

`STAGE8_WP09_STATIC_RED_TEAM = PASS`

`STAGE8_WP09_EXECUTABLE_VALIDATION = PENDING`

`RELEASE_ELIGIBLE_IN_STAGE8 = FALSE`

`STAGE9_RECOVERY_RELEASE_EXECUTION = NOT_IMPLEMENTED`

`NEXT_ON_PASS = WP10_AUTOMATIC_CONTINUITY`
