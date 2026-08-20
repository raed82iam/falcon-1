# AMD-008 — Falcon Foundation Architecture Final Alignment

**Identifier:** AMD-008  
**Version:** 0.2  
**Status:** APPROVED PENDING COORDINATED ACTIVATION  
**Approval Record:** GOV-063  
**Date:** 2026-07-28  
**Authority:** Project Owner instruction  
**Documentary Activation:** Not Authorized  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted

## Purpose

Align Falcon Foundation with its identity as the domain-independent operating-system foundation for multiple independent Falcon Applications.

This amendment preserves valid Foundation work. It corrects the Application awareness hierarchy, completes the Application hosting boundary, and prevents Application business logic from entering Foundation.

## Approved Direction Pending Coordinated Activation

```text
Falcon Foundation / Falcon OS
  └─ FSA — Foundation Self-Awareness

Falcon Application
  └─ MSA — Main Self-Awareness for that Application
      └─ LSA — exactly one Local Self-Awareness for every major Application branch
          └─ CSA — optional awareness for one eligible intelligent component
```

There is no Falcon-wide Applications-ecosystem MSA in this model.

Every major Application branch SHALL own exactly one LSA responsible for that branch. CSA remains optional and is permitted only for an eligible intelligent component.

## Package

| Document | Purpose |
|---|---|
| `01_ARCHITECTURAL_IMPACT_AND_GAP_REPORT.md` | Existing-state review and required corrections |
| `adrs/ADR-I015_FALCON_OS_APPLICATION_AND_AWARENESS_ALIGNMENT.md` | Governing architecture decision |
| `specifications/AWR-006_MAIN_SELF_AWARENESS_v2.0_PROPOSED.md` | One-Application MSA |
| `specifications/AWR-007_LOCAL_SELF_AWARENESS_v2.0_PROPOSED.md` | One-major-branch LSA |
| `specifications/AWR-008_COMPONENT_SELF_AWARENESS_v1.1_PROPOSED.md` | Optional eligible-component CSA |
| `specifications/APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE_v1.1_PROPOSED.md` | Plug-in Application model and lifecycle |
| `contracts/CON-023_APPLICATION_CONTRACT_AND_MANIFEST_v1.1_PROPOSED.md` | Uniform Application Contract |
| `specifications/SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE_v1.1_PROPOSED.md` | Foundation/Application resource boundary |
| `specifications/SYS-003_FOUNDATION_SERVICE_OWNERSHIP_AND_CATALOG_v1.1_PROPOSED.md` | Foundation service ownership |
| `specifications/SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE_v1.0_PROPOSED.md` | Application dependency governance |
| `02_REQUIREMENTS_TRACEABILITY_AND_VALIDATION.md` | Coverage and readiness result |
| `03_OWNER_APPROVAL_PACKAGE.md` | Owner decision package |
| `04_FOCUSED_CHANGE_SUMMARY.md` | Final-alignment correction summary |

## Historical Protection

- GOV-061 and all approved records remain immutable.
- ADR-I009 and AMD-004 remain historical evidence of the previously approved design.
- AMD-007 remains Proposed.
- No active document is modified, superseded, or reinterpreted by AMD-008 preparation.
- GOV-063 granted architectural approval. Documentary effect now requires a separate Coordinated Documentary Activation Package, its final consistency review, and explicit separate Project Owner approval for activation.
- No implementation, verification execution, runtime activation, deployment, cloud work, or Stage 1 work is authorized.
