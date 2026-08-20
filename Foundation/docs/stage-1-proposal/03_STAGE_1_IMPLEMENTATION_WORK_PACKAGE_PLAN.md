# 03 - Stage 1 Implementation Work Package Plan

## Stage 1 requirement set

| Requirement ID | Exact requirement |
|---|---|
| S1-REQ-001 | one canonical Falcon Foundation repository boundary |
| S1-REQ-002 | one canonical .NET solution |
| S1-REQ-003 | exact planned project structure and dependency direction |
| S1-REQ-004 | dependency locks and provenance records |
| S1-REQ-005 | isolated build and test environment usage |
| S1-REQ-006 | formatting, static-analysis, architecture-analysis, security-scan, secret-scan, dependency-scan, and verification commands |
| S1-REQ-007 | generated-artifact and secret-exclusion rules |
| S1-REQ-008 | version and artifact identity mechanism |
| S1-REQ-009 | traceability-output locations |
| S1-REQ-010 | evidence-output locations |
| S1-REQ-011 | deterministic and reproducible empty-build design |
| S1-REQ-012 | proof design for absence of financial dependencies, credentials, endpoints, data, and paths |
| S1-REQ-013 | constitutional and Foundation/Application scope checks |

## Work package plan

| Work Package ID | Exact objective | Canonical authority | Planned affected paths | Inputs | Exact outputs | Dependencies | Verification scenario IDs | Stop conditions | Rollback target | Completion criteria |
|---|---|---|---|---|---|---|---|---|---|---|
| WP-01 | Establish the repository boundary and canonical solution identity | `docs/adrs/ADR-I002_REPOSITORY_AND_DEPENDENCY_POLICY.md` | `./`, `./Falcon.Foundation.ControlledProjectFoundation.slnx` | Stage 0 closure, current repo tree | boundary definition and solution identity | S1-REQ-001, S1-REQ-002 | VS-01, VS-03 | any path outside the repository boundary is required | prior boundary state | boundary and solution are exact |
| WP-02 | Establish project ownership and dependency direction | `docs/adrs/ADR-I002_REPOSITORY_AND_DEPENDENCY_POLICY.md` | `./src/Falcon.Foundation.Core/`, `./src/Falcon.Foundation.Contracts/`, `./src/Falcon.Foundation.Infrastructure/` | boundary map and contract authority | project ownership and dependency graph | S1-REQ-003 | VS-02 | project reference flows outward or into an application layer | prior graph state | dependency direction is inward |
| WP-03 | Pin the toolchain and SDK identity | `docs/catalogs/BLD-001_FOUNDATION_TOOLCHAIN_AND_BUILD_BASELINE_CATALOG.md`, `docs/environments/ENV-001_FOUNDATION_BUILD_AND_VERIFICATION_ENVIRONMENT_PROFILE.md` | `./build/`, `./tools/` | exact toolchain values, environment identity, manifest digests | pinned toolchain and tool manifests | S1-REQ-005, S1-REQ-006, S1-REQ-008 | VS-04, VS-05, VS-07, VS-13 | tool identity or provenance is ambiguous | prior pin set | all tools are exact and identified |
| WP-04 | Define dependency lock, provenance, license, vulnerability, and SBOM controls | `docs/adrs/ADR-I002_REPOSITORY_AND_DEPENDENCY_POLICY.md` | `./artifacts/locks/`, `./artifacts/evidence/stage-1/` | approved dependency policy | lock and provenance controls | S1-REQ-004, S1-REQ-007 | VS-05, VS-06, VS-08 | any financial or external dependency appears | prior lock set | dependency admission is governed and reproducible |
| WP-05 | Define formatting and static-analysis commands | `docs/adrs/ADR-I007_FOUNDATION_BUILD_VERIFICATION_AND_PROMOTION_PIPELINE.md` | `./build/`, `./tools/` | toolchain pins and repository map | exact command list and execution order | S1-REQ-006 | VS-07, VS-10 | commands require network or production access | prior command set | commands are deterministic |
| WP-06 | Define architecture-boundary enforcement | `docs/adrs/ADR-F001_FOUNDATION_EXECUTION_AND_ISOLATION_MODEL.md` | `./src/Falcon.Foundation.Core/`, `./src/Falcon.Foundation.Infrastructure/`, `./tests/Falcon.Foundation.Architecture.Tests/` | repository map and baseline architecture | architecture enforcement rules | S1-REQ-003, S1-REQ-013 | VS-02, VS-10, VS-17 | runtime behavior enters the foundation core | prior architecture plan | prohibited references are blocked |
| WP-07 | Define security and secret scanning | `docs/adrs/ADR-F001_FOUNDATION_EXECUTION_AND_ISOLATION_MODEL.md` | `./src/`, `./tests/`, `./artifacts/` | security policy and toolchain | security scan design | S1-REQ-006, S1-REQ-007, S1-REQ-012 | VS-07, VS-08, VS-09, VS-17 | secrets or external endpoints are admitted | prior security plan | scans are exact and repeatable |
| WP-08 | Define artifact version and identity | `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` | `./artifacts/generated/stage-1/`, `./artifacts/evidence/stage-1/` | traceability rules | identity and artifact naming rules | S1-REQ-008, S1-REQ-009, S1-REQ-010 | VS-13, VS-14, VS-15 | artifact identity cannot be reconstructed | prior identity rules | every artifact is identifiable |
| WP-09 | Define traceability and evidence outputs | `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` | `./artifacts/traceability/stage-1/`, `./artifacts/evidence/stage-1/` | traceability baseline | output map and retention model | S1-REQ-009, S1-REQ-010 | VS-14, VS-15 | outputs are not reproducible or are self-referential | prior evidence map | outputs are locatable and immutable |
| WP-10 | Define deterministic empty-build design | `docs/releases/FRS-001_FOUNDATION_RELEASE.md`, `docs/adrs/ADR-I007_FOUNDATION_BUILD_VERIFICATION_AND_PROMOTION_PIPELINE.md` | `./src/`, `./build/`, `./tests/` | toolchain pins and repository map | empty-build design | S1-REQ-011 | VS-11, VS-12, VS-13 | build is not reproducible without runtime variance | prior build design | empty build is deterministic |
| WP-11 | Prove absence of financial dependencies and financial paths | `docs/adrs/ADR-I002_REPOSITORY_AND_DEPENDENCY_POLICY.md` | `./`, `./artifacts/` | dependency and path inventory | financial-path exclusion proof | S1-REQ-012 | VS-08, VS-09, VS-17 | any financial path or endpoint exists | prior exclusion evidence | absence of financial path is provable |
| WP-12 | Define constitutional and Foundation/Application scope checks | `docs/02_FALCON_CONSTITUTION.md`, `docs/03_DOCUMENT_AUTHORITY.md`, `docs/01_FALCON_VISION.md` | all Stage 1 planned paths | constitutional constraints and current-state sequence | scope-check criteria and evidence rules | S1-REQ-013 | VS-16, VS-17 | scope crosses into Application logic | prior scope review | constitutional and scope compliance is testable |
| WP-13 | Assemble the Stage 1 execution-authority owner decision package | all above canonical authorities | `./docs/stage-1-proposal/` | completed work packages | execution-authority decision package and validation report | S1-REQ-001 through S1-REQ-013 | VS-01 through VS-18 | any prerequisite remains unresolved | prior package state | package is complete for Owner decision |
| WP-14 | Draft the bounded Foundation Implementation Authority Instrument | `docs/contracts/CON-012_AUTHORITY_INSTRUMENT.md`, `docs/plans/IMP-001_FOUNDATION_IMPLEMENTATION_WORK_PLAN.md`, Constitution, Document Authority | `./docs/stage-1-proposal/14_STAGE_1_FOUNDATION_IMPLEMENTATION_AUTHORITY_INSTRUMENT_DRAFT.md` | execution authority design, jurisdictional limits, scope boundaries | instrument draft and review record | P-03, P-04 | VS-18 | draft cannot remain bounded or conflicts with jurisdiction | pre-draft authority state | draft exists and remains DRAFT_NOT_ISSUED |

## Coverage matrix

| Requirement ID | Work Package IDs | Verification Scenario IDs | Evidence IDs | Acceptance Criterion IDs | Coverage result |
|---|---|---|---|---|---|
| S1-REQ-001 | WP-01 | VS-01 | EV-01 | AC-01 | Covered |
| S1-REQ-002 | WP-01 | VS-03 | EV-03 | AC-02 | Covered |
| S1-REQ-003 | WP-02, WP-06 | VS-02, VS-10 | EV-02, EV-10 | AC-03, AC-06 | Covered |
| S1-REQ-004 | WP-04 | VS-05, VS-06 | EV-05, EV-06 | AC-04 | Covered |
| S1-REQ-005 | WP-03, WP-04, WP-10 | VS-04, VS-05, VS-18 | EV-04, EV-05, EV-18 | AC-05 | Covered |
| S1-REQ-006 | WP-05, WP-06, WP-07 | VS-07, VS-08, VS-09, VS-10 | EV-07, EV-08, EV-09, EV-10 | AC-06, AC-07 | Covered |
| S1-REQ-007 | WP-04, WP-07 | VS-08, VS-13 | EV-08, EV-13 | AC-07 | Covered |
| S1-REQ-008 | WP-03, WP-08 | VS-13 | EV-13 | AC-08 | Covered |
| S1-REQ-009 | WP-08, WP-09 | VS-14 | EV-14 | AC-09 | Covered |
| S1-REQ-010 | WP-08, WP-09 | VS-15 | EV-15 | AC-10 | Covered |
| S1-REQ-011 | WP-10 | VS-11, VS-12 | EV-11, EV-12 | AC-11 | Covered |
| S1-REQ-012 | WP-07, WP-11 | VS-08, VS-09, VS-17 | EV-08, EV-09, EV-17 | AC-12 | Covered |
| S1-REQ-013 | WP-06, WP-12 | VS-16, VS-17 | EV-16, EV-17 | AC-13 | Covered |

## Coverage note

All requirements are mapped. All work packages are mapped. All scenarios have
evidence mappings. All acceptance criteria map to requirements.

