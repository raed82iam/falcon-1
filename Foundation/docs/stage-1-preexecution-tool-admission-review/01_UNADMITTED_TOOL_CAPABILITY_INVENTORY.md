# 01 - Unadmitted Tool Capability Inventory

## Canonical Stage 1 outcomes

| Outcome ID | Exact outcome | Canonical source | Mandatory in Stage 1 | Requires Falcon runtime or behavioral execution | Requires controlled build or verification command execution | Requires external package | Required evidence | Interpretation confidence |
|---|---|---|---|---|---|---|---|---|
| OUT-01 | repository and solution structure | `docs/stage-1-proposal/01_STAGE_1_PURPOSE_SCOPE_AND_NON_SCOPE.md`; `docs/stage-1-proposal/02_STAGE_1_FOUNDATION_COMPONENT_AND_PATH_BOUNDARY.md`; `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md` | Yes | No | Yes | No | boundary map, repository-relative paths | EXPLICIT |
| OUT-02 | deterministic empty build | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | Yes | No | Yes | No | reproducible empty-build design evidence | EXPLICIT |
| OUT-03 | dependency lock and restore | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/stage-1-proposal/07_STAGE_1_ENVIRONMENT_TOOLCHAIN_AND_RESOURCE_PLAN.md` | Yes | No | Yes | No | locked versions, provenance, approved sources | EXPLICIT |
| OUT-04 | project-reference and architecture-boundary verification | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/stage-1-proposal/08_STAGE_1_VERIFICATION_PLAN.md` | Yes | No | Yes | No | dependency graph and boundary evidence | EXPLICIT |
| OUT-05 | formatting, compiler, and static-analysis enforcement | `docs/stage-1-proposal/01_STAGE_1_PURPOSE_SCOPE_AND_NON_SCOPE.md`; `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/stage-1-proposal/08_STAGE_1_VERIFICATION_PLAN.md` | Yes | No | Yes | No | governed command design and analysis rules | STRONGLY_IMPLIED |
| OUT-06 | secret, credential, financial-path, and prohibited-endpoint exclusion | `docs/stage-1-proposal/01_STAGE_1_PURPOSE_SCOPE_AND_NON_SCOPE.md`; `docs/stage-1-proposal/08_STAGE_1_VERIFICATION_PLAN.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | Yes | No | Yes | No | inspection and exclusion evidence | EXPLICIT |
| OUT-07 | environment and manifest revalidation | `docs/stage-1-proposal/04_STAGE_1_PREREQUISITE_AND_DEPENDENCY_MATRIX.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | Yes | No | Yes | No | environment admission and manifest revalidation evidence | EXPLICIT |
| OUT-08 | artifact identity | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | Yes | No | Yes | No | artifact identity rules | EXPLICIT |
| OUT-09 | evidence and traceability generation | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/stage-1-proposal/08_STAGE_1_VERIFICATION_PLAN.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | Yes | No | Yes | No | evidence-location and trace output design | EXPLICIT |
| OUT-10 | constitutional and Foundation/Application scope review | `docs/stage-1-proposal/01_STAGE_1_PURPOSE_SCOPE_AND_NON_SCOPE.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | Yes | No | Yes | No | constitutional and scope review documents | EXPLICIT |
| OUT-11 | test execution and result collection | `docs/stage-1-proposal/08_STAGE_1_VERIFICATION_PLAN.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | No | Yes | Yes | Yes | test-results evidence model | DEFERRED_TO_LATER_STAGE |
| OUT-12 | generated SBOM | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/stage-1-proposal/07_STAGE_1_ENVIRONMENT_TOOLCHAIN_AND_RESOURCE_PLAN.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` | No | Yes | Yes | Yes | SBOM evidence | DEFERRED_TO_LATER_STAGE |

## Normalized capability inventory

| Capability ID | Exact capability | Mandatory outcome IDs | Existing admitted mechanism | New tool required | Classification | Evidence |
|---|---|---|---|---|---|---|
| S1-TCAP-001 | controlled empty-build execution | OUT-02 | active SDK-bound build baseline and empty-build design | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/catalogs/BLD-001_FOUNDATION_TOOLCHAIN_AND_BUILD_BASELINE_CATALOG.md`; `docs/environments/ENV-001_FOUNDATION_BUILD_AND_VERIFICATION_ENVIRONMENT_PROFILE.md` |
| S1-TCAP-002 | compiler and static-analysis enforcement | OUT-05 | governed command design and SDK-bound analyzers | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`; `docs/adrs/ADR-I007_FOUNDATION_BUILD_VERIFICATION_AND_PROMOTION_PIPELINE.md` |
| S1-TCAP-003 | project-graph and architecture-boundary verification | OUT-04 | repository inspection and architecture policy | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/stage-1-proposal/02_STAGE_1_FOUNDATION_COMPONENT_AND_PATH_BOUNDARY.md`; `docs/adrs/ADR-F001_FOUNDATION_EXECUTION_AND_ISOLATION_MODEL.md` |
| S1-TCAP-004 | security, secret, and prohibited-path inspection | OUT-06 | inspection rules and documentary evidence review | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/stage-1-proposal/08_STAGE_1_VERIFICATION_PLAN.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` |
| S1-TCAP-005 | behavioral test execution and result collection | OUT-11 | none yet proven as admitted for Stage 1 | Yes | `DEFERRED_TO_LATER_STAGE` | `docs/stage-1-proposal/07_STAGE_1_ENVIRONMENT_TOOLCHAIN_AND_RESOURCE_PLAN.md`; `docs/stage-1-proposal/08_STAGE_1_VERIFICATION_PLAN.md` |
| S1-TCAP-006 | evidence and traceability output | OUT-09 | traceability and evidence documentation design | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` |
| S1-TCAP-007 | environment and manifest revalidation | OUT-07 | active activation manifests and current-state reconciliation | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/reviews/STAGE_SEQUENCE_STATE_RECONCILIATION_001.md`; `docs/stage-1-proposal/04_STAGE_1_PREREQUISITE_AND_DEPENDENCY_MATRIX.md` |
| S1-TCAP-008 | dependency acquisition and locked restore | OUT-03 | .NET SDK-bound restore behavior plus dependency policy | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/catalogs/BLD-001_FOUNDATION_TOOLCHAIN_AND_BUILD_BASELINE_CATALOG.md`; `docs/adrs/ADR-I002_REPOSITORY_AND_DEPENDENCY_POLICY.md` |
| S1-TCAP-009 | generated SBOM production | OUT-12 | SPDX schema identity and inventory governance only | Yes, if later required | `DEFERRED_TO_LATER_STAGE` | `docs/adrs/ADR-I007_FOUNDATION_BUILD_VERIFICATION_AND_PROMOTION_PIPELINE.md`; `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` |
| S1-TCAP-010 | constitutional and scope review | OUT-10 | constitutional review documents | No | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` | `docs/stage-1-proposal/01_STAGE_1_PURPOSE_SCOPE_AND_NON_SCOPE.md`; `docs/stage-1-proposal/11_STAGE_1_COMPLETION_AND_ACCEPTANCE_CRITERIA.md` |

## Current interpretation

The current canonical reading leaves no Stage 1 tool-admission need for
behavioral testing, and no separate Stage 1 generated SBOM tool requirement.

The package therefore records deferred later-stage capabilities rather than a
current admission blocker.
