# Stage 8 WP-05 Implementation Design, Red Team and Pretest Checkpoint V1

## Scope

WP-05 implements the Foundation Lifecycle response to an already-governed protective restriction. It does not create Guardian authority, recovery authority, release authority, or Application business semantics.

## Governing source alignment

SYS-002 requires canonical restriction/suspension/stop semantics, Guardian-requested protective transitions, immediate protective termination where required, and prohibits return to RUNNING before recovery validation.

FCR-0076 and FCR-0082 remain Waiting On: FOUNDATION and map WP-05 to Lifecycle enforcement within the Owner-authorized Stage 8 sequence.

## Implementation

Production file:
- `src/Foundation.ApplicationLifecycle/ProtectiveLifecycleEnforcement.cs`

Executable verifier:
- `verification/Falcon.Stage8.WP05.Verifier/`

The Lifecycle project does not acquire a direct dependency on `Foundation.Guardian`. The verifier proves compatibility by executing the chain:

`Guardian decision -> Guardian restriction -> CON-011 RestrictionRecord -> canonical protective lifecycle projection -> Lifecycle enforcement`.

Mapping:
- `RESTRICTED` -> Lifecycle `Restricted`
- `ISOLATED` -> Lifecycle `Suspended` plus `IsolationRequired=true`
- `SUSPENDED` -> Lifecycle `Suspended`
- `SAFE` / `STOPPED` -> Lifecycle `Stopped` plus `IsolationRequired=true`

All successful protective outcomes deny new execution and remain restricted. Missing, invalid, ambiguous or stale restriction/authority evidence fails closed to a stopped/isolation-required result.

## Boundary invariants

- Lifecycle does not decide that a subject is recovered.
- Lifecycle does not release an active restriction.
- Lifecycle does not return a protected subject to RUNNING.
- Isolation is preserved explicitly without inventing a canonical SYS-002 lifecycle state named ISOLATED.
- Stage 9 remains authoritative for generic recovery validation, release and reintroduction.
- Stage 13 remains authoritative for FSA-specific investigation/recovery semantics.

## Pre-executable Red Team

Challenges reviewed:
1. Direct Guardian -> Lifecycle coupling or dependency cycle: prevented.
2. Unknown protective mode interpreted optimistically: fails closed.
3. Missing restriction evidence: fails closed.
4. Ambiguous protective authority: fails closed.
5. Restriction used before effective time: rejected/fails closed.
6. Isolation collapsed into ordinary suspension without explicit isolation requirement: prevented.
7. Emergency SAFE mode accidentally interpreted as recovery-safe RUNNING: prevented; maps to STOPPED.
8. Lifecycle self-release or self-recovery: no such path exists in WP-05.
9. Outcome identity nondeterminism or evidence mutation blindness: executable verifier covers determinism and mutation sensitivity.
10. Application/trading business semantics entering Foundation Lifecycle: absent.

Pre-executable result:
- Critical: 0
- High: 0
- Medium: 0
- Product-Low: 0

## Executable acceptance target

The exact candidate shall pass:
- controlled restore/build;
- Architecture gate;
- Security gate;
- Stage 7 cross-stage regression;
- Stage 8 WP-01/WP-02/WP-03/WP-04 regressions;
- WP-05 verifier twice with identical output;
- 21/21 WP-05 checks;
- binary hash stability;
- exact final HEAD and clean worktree.

This checkpoint creates no Owner closure. On PASS the workstream continues automatically to WP-06 under the standing Stage 8 execution authority.
