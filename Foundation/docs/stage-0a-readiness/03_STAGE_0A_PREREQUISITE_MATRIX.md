# Stage 0A Prerequisite Matrix

**Stage 0A Status:** `BLOCKED`

| Prerequisite ID | Category | Canonical source | Requirement | Evidence class | Evidence required | Dependency | Severity | Owner decision required | Status |
|---|---|---|---|---|---|---|---|---|---|
| S0A-PR-001 | Authority | `docs/releases/FRS-001_READINESS_REPORT.md` | Stage 0A SHALL remain not authorized unless separately approved | `POST_STAGE_0A_ACCEPTANCE_EVIDENCE` | Explicit Owner authorization | Governance | HIGH | Yes | UNSATISFIED |
| S0A-PR-002 | Bootstrap sequencing | `docs/adrs/ADR-I008_FOUNDATION_BOOTSTRAP_AND_ACTIVATION_SEQUENCING.md` | Stage 0A SHALL be sequenced under bounded bootstrap rules without operational trust | `PREPARATION_DESIGN_EVIDENCE` | Governed bootstrap sequence and candidate rules | Authority | HIGH | Yes | UNSATISFIED |
| S0A-PR-003 | Pipeline boundary | `docs/specifications/foundation/PIPE-001_FOUNDATION_PIPELINE_SPECIFICATION.md` | Stage 0A SHALL not cross into governed or active Pipeline execution | `PREPARATION_DESIGN_EVIDENCE` | Pipeline mode and gate boundary evidence | Pipeline governance | HIGH | Yes | UNSATISFIED |
| S0A-PR-004 | Foundation/Application separation | `docs/adrs/ADR-I015_FALCON_OS_APPLICATION_AND_AWARENESS_ALIGNMENT.md` | Stage 0A SHALL preserve Foundation/Application boundaries | `PREPARATION_DESIGN_EVIDENCE` | Boundary map and reviewer evidence | Architecture | MEDIUM | Yes | SATISFIED |
| S0A-PR-005 | Security | `docs/02_FALCON_CONSTITUTION.md` ; `docs/03_DOCUMENT_AUTHORITY.md` | Stage 0A SHALL remain non-operational, non-financial, and non-external | `PREPARATION_DESIGN_EVIDENCE` | Security and non-authority statements | Security | HIGH | Yes | SATISFIED |
| S0A-PR-006 | Environment candidates | `docs/adrs/ADR-I008_FOUNDATION_BOOTSTRAP_AND_ACTIVATION_SEQUENCING.md` | Stage 0A environment candidates SHALL be bounded and non-operational | `PREPARATION_DESIGN_EVIDENCE` | Environment candidate definition | Environment | MEDIUM | Yes | UNSATISFIED |
| S0A-PR-007 | Dependency candidates | `docs/specifications/foundation/SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE.md` | Stage 0A dependency candidates SHALL not become active dependencies | `PREPARATION_DESIGN_EVIDENCE` | Dependency candidate list and containment rule | Dependencies | MEDIUM | Yes | UNSATISFIED |
| S0A-PR-008 | Resource governance | `docs/specifications/foundation/SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE.md` | Stage 0A SHALL not exceed governed resource boundaries | `PREPARATION_DESIGN_EVIDENCE` | Resource floor and quota evidence | Resources | LOW | Yes | SATISFIED |
| S0A-PR-009 | Evidence model | `docs/specifications/foundation/PIPE-001_FOUNDATION_PIPELINE_SPECIFICATION.md` | Stage 0A SHALL distinguish design, execution, and acceptance evidence | `PREPARATION_DESIGN_EVIDENCE` | Evidence-class mapping | Evidence | LOW | No | SATISFIED |
| S0A-PR-010 | Stage 0B promotion conditions | `docs/specifications/foundation/PIPE-001_FOUNDATION_PIPELINE_SPECIFICATION.md` | Stage 0A outputs SHALL not promote into Stage 0B without separate conditions | `PREPARATION_DESIGN_EVIDENCE` | Promotion conditions | Promotion | MEDIUM | Yes | UNSATISFIED |
| S0A-PR-011 | Stage 1 separation | `docs/releases/FRS-001_READINESS_REPORT.md` | Stage 0A SHALL not create Stage 1 authority or Stage 1 readiness | `PREPARATION_DESIGN_EVIDENCE` | Stage 1 non-authority evidence | Stage boundary | HIGH | Yes | SATISFIED |
| S0A-PR-012 | Filesystem and process boundary | `docs/adrs/ADR-I008_FOUNDATION_BOOTSTRAP_AND_ACTIVATION_SEQUENCING.md` | Stage 0A SHALL use isolated boundaries and bounded candidates | `PREPARATION_DESIGN_EVIDENCE` | Process and filesystem boundary definition | Isolation | MEDIUM | Yes | UNSATISFIED |

## Summary

The prerequisite matrix shows Stage 0A is defined, but not ready for authorization.
