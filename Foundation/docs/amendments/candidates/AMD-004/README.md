# AMD-004 — Self-Awareness Architecture Boundary Correction Package

**Identifier:** AMD-004  
**Version:** 0.2  
**Status:** Approved Architecture — Documentary Activation Deferred  
**Approval Record:** GOV-061  
**Date:** 2026-07-27  
**Owner:** Falcon Project Owner  
**Prepared By:** Falcon Work Environment  
**Authority Basis:** Project Owner instruction, “Pre-Stage 1 Self-Awareness Architecture Correction”  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted  
**Supersedes:** Nothing until separate controlled documentary activation

## Purpose

This package proposes the controlled correction of Falcon self-awareness architecture into:

```text
FSA — Falcon Self-Awareness System
  ↓
MSA — Main Self-Awareness
  ↓
LSA — Local Self-Awareness
  ↓
CSA — Component Self-Awareness
```

The hierarchy governs awareness scope, escalation, and conformance. It does not transfer ownership upward.

## Package Contents

| Deliverable | File |
|---|---|
| Architectural Impact Assessment | `01_ARCHITECTURAL_IMPACT_ASSESSMENT.md` |
| Repair and Evolution Impact Assessment | `02_FSA_REPAIR_AND_EVOLUTION_IMPACT_ASSESSMENT.md` |
| Proposed ADR | `adrs/ADR-I009_SELF_AWARENESS_HIERARCHY_AND_BOUNDARY.md` |
| Proposed FSA Specification | `specifications/AWR-001_FALCON_SELF_AWARENESS_SYSTEM_v2.0_PROPOSED.md` |
| Proposed MSA Specification | `specifications/AWR-006_MAIN_SELF_AWARENESS_PROPOSED.md` |
| Proposed LSA Specification | `specifications/AWR-007_LOCAL_SELF_AWARENESS_PROPOSED.md` |
| Proposed CSA Specification | `specifications/AWR-008_COMPONENT_SELF_AWARENESS_PROPOSED.md` |
| Hierarchy and Boundary Diagrams | `support/AWARENESS_HIERARCHY_AND_BOUNDARIES.md` |
| Authority, Ownership, and Conformance Matrices | `support/AUTHORITY_OWNERSHIP_CONFORMANCE_MATRICES.md` |
| Repair and Evolution Authority Matrix | `support/FSA_REPAIR_AND_EVOLUTION_AUTHORITY_MATRIX.md` |
| Candidate and Owner Decision Lifecycle | `support/FSA_CANDIDATE_AND_OWNER_DECISION_LIFECYCLE.md` |
| Owner Communication and Approval Center | `support/OWNER_COMMUNICATION_AND_APPROVAL_CENTER_SPECIFICATION.md` |
| Guardian Readiness Supervision | `support/FSA_GUARDIAN_READINESS_SUPERVISION_REQUIREMENTS.md` |
| Registry, Index, Tree, and Glossary Change Set | `support/REGISTRY_INDEX_TREE_GLOSSARY_CHANGESET.md` |
| Migration and Compatibility Note | `support/MIGRATION_COMPATIBILITY_AND_CROSS_REFERENCE.md` |
| Acceptance-Evidence Plan | `verification/VPL-AWR-001_AWARENESS_BOUNDARY_ACCEPTANCE_PLAN.md` |
| Cross-Document Consistency Review | `06_CROSS_DOCUMENT_CONSISTENCY_REVIEW.md` |
| Constitutional Compliance Report | `07_CONSTITUTIONAL_COMPLIANCE_REPORT.md` |
| Owner Approval Package | `08_OWNER_APPROVAL_PACKAGE.md` |
| Pre-Stage 1 Readiness Result | `09_PRE_STAGE_1_READINESS_RESULT.md` |
| Changelog | `CHANGELOG.md` |
| v0.1 Content Manifest | `history/AMD-004_v0.1_CONTENT_MANIFEST.md` |

## Historical Protection

- No Approved document is modified by this package.
- AWR-001 v1.0 remains Approved and effective until a later Owner approval activates a successor.
- ADR-I009 and all proposed Specifications have no authority before approval.
- Proposed identifiers AWR-006, AWR-007, and AWR-008 are reservations within this package only.
- No Stage 1 planning, implementation, activation, deployment, or financial authority is created.

## Governing Statement

> CSA understands the component. LSA understands the Application. MSA understands the Falcon Applications ecosystem. FSA protects Falcon itself and determines whether a proposed change conforms to Falcon’s governing rules for admission.

> FSA understands how Falcon operates. FSA does not understand what an Application does as a business product.

> FSA may autonomously restore an Approved trusted Foundation state. FSA may create and validate a new candidate state in isolation. FSA may not approve or activate the candidate it created.
