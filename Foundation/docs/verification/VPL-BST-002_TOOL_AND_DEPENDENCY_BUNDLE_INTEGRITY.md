# VPL-BST-002 — Tool and Dependency Bundle Integrity Verification Plan

**Identifier:** VPL-BST-002  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-031  
**Owner:** Falcon Verification Authority  
**Governing Sources:** ADR-I008; BLD-001; SEC-001; SEC-002; CON-008; CON-010; CON-020; CON-021  
**Master Plan:** VPL-BST-000  
**Implementation Authority:** Not Granted

## 1. Objective

Prove that every mandatory tool and dependency is exact, attributable, integrity-verifiable, license- and vulnerability-assessed, reproducibly bundled, and incapable of becoming an approved Falcon build input merely by being acquired.

## 2. Required Setup

- active `PREPARATION` environment;
- declared Build Intent;
- candidate BLD-001 tool list;
- allowlisted acquisition sources;
- publisher, signature, digest, license, and vulnerability policies;
- independent digest computation;
- offline-bundle target; and
- external evidence capture.

## 3. Procedure

1. Acquire only exact allowlisted subjects.
2. preserve source, publisher, version, signature, and original bytes.
3. compute independent content digests.
4. assess license and vulnerability status.
5. construct a content-identified offline bundle and Manifest.
6. rebuild the bundle independently and compare identities.
7. test mutation, substitution, rollback, omission, duplication, and unauthorized addition.
8. verify that the bundle remains candidate-only.
9. export and reconstruct the evidence.

## 4. Pass Rule

`PASS` requires exact reproducibility, agreement of independent integrity checks, complete provenance and assessments, detection of every controlled fault, and explicit non-activation.

## 5. Requirements

- **VPL-BST-002-REQ-001:** Every item SHALL have exact identity, version, source, publisher evidence, and digest.
- **VPL-BST-002-REQ-002:** Independent digest computation SHALL confirm content identity.
- **VPL-BST-002-REQ-003:** License and vulnerability disposition SHALL be explicit.
- **VPL-BST-002-REQ-004:** Offline bundle reconstruction SHALL produce the same canonical Manifest.
- **VPL-BST-002-REQ-005:** Mutation, substitution, rollback, omission, duplication, and addition SHALL be detected.
- **VPL-BST-002-REQ-006:** Developer-machine state SHALL NOT become official input.
- **VPL-BST-002-REQ-007:** Acquisition SHALL NOT imply Build Baseline Activation.
- **VPL-BST-002-REQ-008:** Evidence SHALL preserve original provenance and external classification.

## 6. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-031 | 2026-07-25 |
