# AMD-006 — Guardian Architecture Separation Correction

**Identifier:** AMD-006  
**Version:** 0.1  
**Status:** Approved Architecture — Documentary Activation Deferred  
**Approval Record:** GOV-062  
**Date:** 2026-07-27  
**Owner:** Falcon Project Owner  
**Preparation Authority:** Project Owner instruction, “Pre-Stage 1 Guardian Architecture Correction”  
**Stage 1 Authority:** Not Granted  
**Implementation Authority:** Not Granted  
**Activation Authority:** Not Granted

## Purpose

AMD-006 completes the separation between:

- **Falcon Foundation Guardian (FFG):** protects Falcon OS and shared technical continuity;
- **Trading Guardian (TG):** protects the Trading Application Suite and may understand trading-domain meaning; and
- **future Application Guardians:** protect only their own Application domains.

Application Guardians may request cross-Application technical protection. Only FFG may impose cross-Application technical isolation or Platform protective modes.

## Deliverables

| Required deliverable | File |
|---|---|
| Guardian Architecture Impact Assessment | `01_GUARDIAN_ARCHITECTURE_IMPACT_ASSESSMENT.md` |
| Constitutional and Authority Review | `02_CONSTITUTIONAL_AND_AUTHORITY_REVIEW.md` |
| Guardian Separation ADR | `adrs/ADR-I011_FOUNDATION_AND_APPLICATION_GUARDIAN_SEPARATION.md` |
| FFG Specification | `specifications/AUT-002_FALCON_FOUNDATION_GUARDIAN_v2.1_PROPOSED.md` |
| Trading Guardian Specification | `specifications/RSK-006_TRADING_GUARDIAN_PROPOSED.md` |
| Protection Request Contract | `contracts/CON-022_APPLICATION_GUARDIAN_PROTECTION_REQUEST_PROPOSED.md` |
| Authority and Knowledge Matrix | `support/AUTHORITY_KNOWLEDGE_AND_ISOLATION_MATRIX.md` |
| Safe Mode Separation Model | `support/SAFE_MODE_SEPARATION_MODEL.md` |
| AUT-002 Migration Plan | `support/AUT-002_MIGRATION_PLAN.md` |
| Manifest, Registry, Diagram, and Cross-Reference Change Set | `support/DOCUMENTATION_CHANGESET_AND_STAGE_1_PREREQUISITES.md` |
| Acceptance-Evidence Plan | `verification/VPL-GDN-002_GUARDIAN_SEPARATION_ACCEPTANCE_PLAN.md` |
| Cross-Document Consistency Review | `07_CROSS_DOCUMENT_CONSISTENCY_REVIEW.md` |
| Owner Approval Package | `08_OWNER_APPROVAL_PACKAGE.md` |
| Readiness Result | `09_READINESS_RESULT.md` |
| Changelog | `CHANGELOG.md` |

## Historical Protection

- GOV-060 and AMD-005 remain immutable historical decisions.
- GOV-061 and AMD-004 remain immutable historical decisions.
- AUT-002 v1.0 remains the active Approved Guardian Specification.
- AUT-002 v2.0 remains an Approved successor design whose activation was deferred.
- AUT-002 v2.1 is a Proposed successor refinement and has no authority.
- Proposed identifiers RSK-006 and CON-022 are reservations only.
- No document in AMD-006 is Approved automatically.

## Governing Statement

> Foundation Guardian protects the platform. Application Guardians protect their own business domains. Application Guardians may request cross-Application technical protection. Only FFG may impose cross-Application technical isolation and Platform protective modes.
