# Falcon Repository Relocation to C Root

**Identifier:** GOV-066  
**Version:** 1.0  
**Status:** Approved  
**Decision Date:** 2026-07-30  
**Decision Authority:** Project Owner  
**Subject:** relocation reconciliation for the Falcon repository root  
**Document Classification:** APPROVED PENDING COORDINATED ACTIVATION  
**Coordinated Documentary Activation:** Not Granted  
**Implementation Authority:** Not Granted  
**Verification Execution Authority:** Not Granted  
**Stage 1 Authority:** Not Granted  
**Stage 1 Preparation Authority:** Not Granted

## 1. Decision

The Project Owner confirms that the Falcon repository has been relocated
from the previous OneDrive-controlled workspace to the new workspace root:

- previous root: `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1`
- new root: `C:\falcon\Falcon1`

This decision records the relocation for documentary reconciliation only.
It does not authorize Stage 1, implementation, deployment, or external
operation.

## 2. Relocation basis

- the new root exists and is readable by the current governed execution
  identity;
- the new root is outside OneDrive and outside profile-redirection paths;
- repository-relative structure is preserved;
- Git metadata exists at the new root;
- required governance, review, evidence, and manifest directories are present;
- the exact 13 activation manifests remain present in the relocated tree;
- the old OneDrive root is absent after the move and cannot be used as the
  current source of truth.

## 3. Comparison result

The old root is absent after the move, so a live byte-for-byte comparison of the
old and new repository trees is not possible in this workspace state.

The accepted current root is therefore the new root itself, with the relocated
repository treated as the active working copy.

## 4. Documentary consequence

- the prior v3 pre-Stage-1 baseline remains historical evidence for the old
  workspace cut;
- a new baseline is required for the new repository root after reconciliation;
- FIAI lifecycle remains suspended;
- Stage 1 execution authority remains not effective;
- the relocation does not create Stage 1 authority or implementation authority;
- NuGet host-path issues are not automatically resolved by relocation.

## 5. Explicit non-authorities

This decision does not authorize:

- Stage 1 implementation;
- Stage 1 execution;
- repository content modification by this reconciliation;
- archive execution;
- rollback execution;
- external connections;
- cloud activity;
- financial activity;
- permissions or ACL changes;
- creation of a replacement baseline within this task.

## 6. Record

| Role | Decision | Name | Date |
|---|---|---|---|
| Project Owner | Confirmed repository relocation to `C:\falcon\Falcon1` and recorded the documentary consequence | Project Owner | 2026-07-30 |

