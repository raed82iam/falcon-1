# Historical and Effective File Disposition

**Status:** Proposed
**Package Status:** Proposed Frozen Final-Review Candidate
**Documentary Activation:** Not Authorized
**Owner Activation Decision:** Not Issued
**Migration Execution:** Not Authorized
**Stage 1 Preparation:** Not Authorized
**Stage 1:** Blocked

## Exact Historical Lineage Destinations

| Current source | Historical disposition |
|---|---|
| `docs/amendments/candidates/AMD-004/` | remains in place, immutable; manifest entry `CDA-HIST-AMD004` |
| `docs/governance/GOV-061_SELF_AWARENESS_ARCHITECTURE_APPROVAL.md` | remains in place, immutable; manifest entry `CDA-HIST-GOV061` |
| `docs/amendments/candidates/AMD-007/` | remains in place, immutable; manifest entry `CDA-HIST-AMD007` |
| `docs/amendments/candidates/AMD-008/` | remains approval provenance; manifest entry `CDA-HIST-AMD008` |
| `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md` | copy to `docs/archive/pre-cda-amd008-001/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM_v1.0.md` only when v2.1 activates |
| `docs/specifications/SPEC-000_REGISTRY.md` | copy to `docs/archive/pre-cda-amd008-001/specifications/SPEC-000_REGISTRY_v1.4.md` |
| `docs/contracts/CON-000_CONTRACT_REGISTRY.md` | copy to `docs/archive/pre-cda-amd008-001/contracts/CON-000_CONTRACT_REGISTRY_v1.6.md` |
| `docs/adrs/ADR-000_INDEX.md` | copy to `docs/archive/pre-cda-amd008-001/adrs/ADR-000_INDEX_v2.6.md` |
| `docs/04_SPECIFICATION_TREE.md` | copy to `docs/archive/pre-cda-amd008-001/TREE-001_SPECIFICATION_TREE_v1.2.md` |
| `docs/05_LEGACY_MIGRATION_MAP.md` | copy to `docs/archive/pre-cda-amd008-001/GOV-002_LEGACY_MIGRATION_MAP_v1.0.md` |
| `docs/specifications/core/README.md` | copy to `docs/archive/pre-cda-amd008-001/specifications/core/README_v1.0.md` |
| `docs/06_FALCON_SELF_AWARE_SYSTEM_CONCEPT_AR.md` | copy to `docs/archive/pre-cda-amd008-001/06_FALCON_SELF_AWARE_SYSTEM_CONCEPT_AR_v1.0.md` |
| `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` | copy to `docs/archive/pre-cda-amd008-001/traceability/TRC-001_v1.2.md` |
| `docs/roadmap/ROADMAP-001_FOUNDATION_GOVERNANCE_AND_SECURITY_BACKLOG.md` | copy to `docs/archive/pre-cda-amd008-001/roadmap/ROADMAP-001_v2.8.md` |
| `docs/releases/FRS-001_READINESS_REPORT.md` | copy to `docs/archive/pre-cda-amd008-001/releases/FRS-001_READINESS_v4.1.md` |

The future sealed manifest identifier is `CDA-AMD008-001-ACTIVATION-MANIFEST`. It shall record every source/target digest and lineage relation.

Historical means attributable to its decision-time context. It does not mean invalid, erased, corrected in place, or silently reinterpreted.

## Files Intended to Become Canonical and Effective

| ID | Target canonical path |
|---|---|
| ADR-I015 | `docs/adrs/ADR-I015_FALCON_OS_APPLICATION_AND_AWARENESS_ALIGNMENT.md` |
| AWR-006 | `docs/specifications/self-awareness/AWR-006_MAIN_SELF_AWARENESS.md` |
| AWR-007 | `docs/specifications/self-awareness/AWR-007_LOCAL_SELF_AWARENESS.md` |
| AWR-008 | `docs/specifications/self-awareness/AWR-008_COMPONENT_SELF_AWARENESS.md` |
| APP-001 | `docs/specifications/applications/APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE.md` |
| CON-023 | `docs/contracts/CON-023_APPLICATION_CONTRACT_AND_MANIFEST.md` |
| SYS-003 | `docs/specifications/foundation/SYS-003_FOUNDATION_SERVICE_OWNERSHIP_AND_CATALOG.md` |
| SYS-004 | `docs/specifications/foundation/SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE.md` |
| SYS-006 | `docs/specifications/foundation/SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE.md` |
| AWR-001 v2.1 | `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md` |
| GLO-001 | `docs/glossary/GLO-001_FALCON_FOUNDATION_AND_APPLICATION_TERMINOLOGY.md` |
| SPEC-000 v1.5 | `docs/specifications/SPEC-000_REGISTRY.md` |
| CON-000 v1.7 | `docs/contracts/CON-000_CONTRACT_REGISTRY.md` |
| ADR-000 v2.7 | `docs/adrs/ADR-000_INDEX.md` |
| TREE-001 v1.3 | `docs/04_SPECIFICATION_TREE.md` |
| GOV-002 v1.1 | `docs/05_LEGACY_MIGRATION_MAP.md` |
| Core README v1.1 | `docs/specifications/core/README.md` |
| Concept AR v1.1 | `docs/06_FALCON_SELF_AWARE_SYSTEM_CONCEPT_AR.md` |
| TRC-001 v1.3 | `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` |
| ROADMAP-001 v2.9 | `docs/roadmap/ROADMAP-001_FOUNDATION_GOVERNANCE_AND_SECURITY_BACKLOG.md` |
| FRS-001-READINESS v4.2 | `docs/releases/FRS-001_READINESS_REPORT.md` |
| CON-002 successor | current canonical path; version 1.1 proposed successor |
| CON-006 successor | current canonical path; version 1.1 proposed successor |
| EVO-001 successor | current canonical path; version 1.1 proposed successor |
| VPL-005 successor | current canonical path; version 1.1 proposed successor |
| FDN-001 successor | current canonical path; version 1.1 proposed successor |
| FDN-002 successor | current canonical path; version 1.2 proposed successor |
| FDN-004 formal no-change decision | `docs/activation/candidates/CDA-AMD008-001/decisions/FDN-004_FORMAL_NO_CHANGE_DECISION.md` |

AWR-001 successor, the dependency-surface successors, the administrative successors, and the FDN-004 no-change decision remain prerequisites and remain Not Authorized.

## Canonical Source Rule

After activation, candidate-package files remain approval provenance but SHALL NOT be the sole canonical sources. Registries point only to canonical effective files.
