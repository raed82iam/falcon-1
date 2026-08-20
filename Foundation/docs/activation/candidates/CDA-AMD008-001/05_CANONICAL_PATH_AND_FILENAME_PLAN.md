# Canonical Path and Filename Plan

**Status:** Proposed  
**File Operations:** Not Authorized

## Policy Finding

GOV-001 requires one canonical location and states that an approved document shall not remain solely in a candidate location. Therefore `_PROPOSED` filenames are unsuitable as final canonical names.

## Planned Renames

| Candidate file | Canonical filename |
|---|---|
| `adrs/ADR-I015_FALCON_OS_APPLICATION_AND_AWARENESS_ALIGNMENT.md` | unchanged basename; moved to `docs/adrs/` |
| `AWR-006_MAIN_SELF_AWARENESS_v2.0_PROPOSED.md` | `AWR-006_MAIN_SELF_AWARENESS.md` |
| `AWR-007_LOCAL_SELF_AWARENESS_v2.0_PROPOSED.md` | `AWR-007_LOCAL_SELF_AWARENESS.md` |
| `AWR-008_COMPONENT_SELF_AWARENESS_v1.1_PROPOSED.md` | `AWR-008_COMPONENT_SELF_AWARENESS.md` |
| `APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE_v1.1_PROPOSED.md` | `APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE.md` |
| `CON-023_APPLICATION_CONTRACT_AND_MANIFEST_v1.1_PROPOSED.md` | `CON-023_APPLICATION_CONTRACT_AND_MANIFEST.md` |
| `SYS-003_FOUNDATION_SERVICE_OWNERSHIP_AND_CATALOG_v1.1_PROPOSED.md` | `SYS-003_FOUNDATION_SERVICE_OWNERSHIP_AND_CATALOG.md` |
| `SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE_v1.0_PROPOSED.md` | `SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE.md` |
| `SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE_v1.1_PROPOSED.md` | `SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE.md` |
| `successors/AWR-001_SELF_AWARENESS_SYSTEM_v2.1_PROPOSED.md` | `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md` after complete successor approval |
| future GLO-001 candidate | `docs/glossary/GLO-001_FALCON_FOUNDATION_AND_APPLICATION_TERMINOLOGY.md` |

Administrative successors retain their existing canonical basenames and replace canonical pointers only through the atomic transition. Dependency-surface successors retain their current canonical basenames with versions in metadata; their exact source candidate paths remain a gap-closure deliverable.

Versions remain in metadata, not canonical filenames.

## Collision and Integrity Gates

Before any move:

1. For a replacement admission, the canonical target SHALL already exist as the active predecessor path, and the staged successor bytes SHALL be validated against the archive source before any canonical pointer change.
2. For a new canonical admission, the canonical target path SHALL NOT exist prior to activation.
3. ID/version pair must be unique.
4. source digest must match the activation manifest.
5. generated canonical metadata must match GOV-001.
6. all links must resolve against the staged canonical tree.
7. original candidate package remains preserved.
8. archive targets must be explicit and non-overlapping.

Any failed gate aborts the entire activation.
