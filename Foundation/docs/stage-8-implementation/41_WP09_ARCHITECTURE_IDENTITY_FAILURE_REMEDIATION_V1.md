# Stage 8 WP-09 Architecture Identity Failure Remediation V1

**Stage:** 8 — Foundation Guardian, Protective Restriction and Platform Safe State  
**WP:** WP-09 — No-Self-Release, Release Preconditions & Stage-9 Recovery Handoff  
**Status:** REMEDIATED_AWAITING_EXECUTABLE_RETEST  
**Date:** 2026-08-15  
**Branch:** `foundation-development`

## Failed exact candidate

The first WP-09 executable validation candidate was:

`585800494ef684520ca8e557876d0321c329ad9e`

The controlled Release build succeeded, but the Foundation Architecture gate failed before Security and WP-09 executable verification could continue.

The Architecture gate rejected permanent production public type identities containing stage-number identity tokens. The rejected public type names were:

- `Stage8ReleaseActorRole`
- `Stage8ReleaseGuardReason`
- `Stage8ReleaseGuardResult`
- `Stage8ReleaseGuard`
- `Stage9RecoveryEvidencePackage`
- `Stage9RecoveryHandoffReason`
- `Stage9RecoveryHandoffRecord`
- `Stage9RecoveryHandoffRuntime`

This was an architecture identity failure, not evidence that the no-self-release logic executed incorrectly.

## Governing architecture rule

Permanent Foundation production public identities shall not be named by transient implementation stage/work-package identity. The Architecture gate therefore remains unchanged. No exception or weakening was introduced.

## Remediation

The WP-09 production API was renamed to permanent domain identities:

- `ProtectiveReleaseActorRole`
- `ProtectiveReleaseGuardReason`
- `ProtectiveReleaseGuardResult`
- `ProtectiveReleaseGuard`
- `RecoveryEvidencePackage`
- `RecoveryHandoffReason`
- `RecoveryHandoffRecord`
- `RecoveryHandoffRuntime`

The implementation source is now:

`src/Foundation.Authority/ProtectiveRecoveryHandoff.cs`

The stage-identified production source `src/Foundation.Authority/Stage9RecoveryHandoff.cs` was removed.

The WP-09 verifier was rebound to the permanent identities while retaining 35 explicit checks.

## Semantic non-change

This remediation does not introduce or execute recovery or release.

The following WP-09 semantics remain mandatory:

- subject self-release is denied;
- Guardian self-release is denied;
- repair actor self-certification/release is denied;
- declared release authority cannot release from the Stage 8 protection context;
- independently supplied recovery evidence and role separation are required for a recovery handoff;
- a recovery-ready handoff is not release;
- restriction expiry/review time is not release;
- the restriction remains enforced;
- Lifecycle reintroduction and the new post-recovery authority decision remain downstream recovery responsibilities;
- no FSA-specific Stage 13 governance/revival capability is introduced.

## Governance state

FCR-0076 and FCR-0082 remain `Waiting On: FOUNDATION`.

WP-09 remains technically open until the remediated exact candidate passes controlled Release build, Architecture, Security, predecessor regressions, WP-09 verifier, determinism and source/worktree integrity checks.

No WP-10 implementation or Stage 8 closure is claimed by this remediation record.
