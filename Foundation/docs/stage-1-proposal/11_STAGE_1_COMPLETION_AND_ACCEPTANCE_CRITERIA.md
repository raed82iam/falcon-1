# 11 - Stage 1 Completion and Acceptance Criteria

| Criterion ID | Requirement ID | Work Package ID | Scenario ID | Execution evidence ID | Artifact path | Responsible evaluator | Pass rule | Blocking failure rule |
|---|---|---|---|---|---|---|---|---|
| AC-01 | S1-REQ-001 | WP-01 | VS-01 | EV-01 | `docs/stage-1-proposal/02_STAGE_1_FOUNDATION_COMPONENT_AND_PATH_BOUNDARY.md` | architecture reviewer | boundary is exact and repo-relative | any path outside root |
| AC-02 | S1-REQ-002 | WP-02 | VS-03 | EV-03 | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md` | build reviewer | solution identity is exact | solution identity mismatch |
| AC-03 | S1-REQ-003 | WP-02 | VS-02 | EV-02 | `docs/stage-1-proposal/05_STAGE_1_ARCHITECTURE_IMPACT_REVIEW.md` | architecture reviewer | dependency direction is inward | any prohibited reference |
| AC-04 | S1-REQ-004 | WP-04 | VS-04, VS-05, VS-06 | EV-04, EV-05, EV-06 | `docs/stage-1-proposal/04_STAGE_1_PREREQUISITE_AND_DEPENDENCY_MATRIX.md` | dependency reviewer | lock, provenance, license, and vulnerability controls are governed | any mismatch or unresolved admission |
| AC-05 | S1-REQ-005 | WP-01, WP-07, WP-14 | VS-04, VS-05, VS-18 | EV-04, EV-05, EV-18 | `artifacts/evidence/stage-1/environment-admission-report.txt` | environment reviewer | environment admission and revalidation are exact | environment drift or invalid manifest |
| AC-06 | S1-REQ-006 | WP-05, WP-06, WP-07 | VS-07, VS-08, VS-09, VS-10 | EV-07, EV-08, EV-09, EV-10 | `artifacts/evidence/stage-1/formatter-and-analyzer-log.txt` | security reviewer | formatting, analysis, and security controls are exact | any security or scan failure |
| AC-07 | S1-REQ-007 | WP-04, WP-05, WP-07 | VS-08, VS-13 | EV-08, EV-13 | `artifacts/evidence/stage-1/generated-artifact-inventory.json` | artifact reviewer | generated-artifact and secret exclusions are exact | unexpected generated artifact or secret |
| AC-08 | S1-REQ-008 | WP-08 | VS-13 | EV-13 | `artifacts/evidence/stage-1/artifact-identity-report.txt` | identity reviewer | artifact identity is exact | identity ambiguity |
| AC-09 | S1-REQ-009 | WP-09 | VS-14 | EV-14 | `artifacts/traceability/stage-1/traceability-export.json` | traceability reviewer | trace output location is exact | trace output missing |
| AC-10 | S1-REQ-010 | WP-09 | VS-15 | EV-15 | `artifacts/evidence/stage-1/evidence-inventory.json` | evidence reviewer | evidence output location is exact | evidence output missing |
| AC-11 | S1-REQ-011 | WP-10 | VS-11, VS-12 | EV-11, EV-12 | `artifacts/evidence/stage-1/reproducibility-report.txt` | build reviewer | empty build is deterministic and reproducible | build is non-deterministic or unreproducible |
| AC-12 | S1-REQ-012 | WP-11 | VS-08, VS-09, VS-17 | EV-08, EV-09, EV-17 | `artifacts/evidence/stage-1/financial-path-exclusion-report.txt` | security reviewer | no financial dependency, endpoint, data, or path exists | any financial path or endpoint exists |
| AC-13 | S1-REQ-013 | WP-12 | VS-16, VS-17 | EV-16, EV-17 | `artifacts/evidence/stage-1/scope-compliance-report.txt` | governance reviewer | constitutional and Foundation/Application scope checks pass | any scope conflict or runtime behavior |

## Acceptance rule

Each criterion is mandatory, objectively testable, and mapped to a single
evidence family. No criterion is optional.
