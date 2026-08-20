# 01 - Stage 1 Purpose, Scope, and Non-Scope

## Purpose

Project Stage 1, `CONTROLLED_PROJECT_FOUNDATION`, establishes the controlled
project boundary that will host Falcon Foundation work without implementing
Falcon runtime behavior.

Its purpose is to make the repository, solution, build, verification,
dependency, evidence, identity, and non-financial project boundary exact,
repeatable, and reviewable.

## Stage 1 requirements

Stage 1 SHALL satisfy the following requirement set:

- `S1-REQ-001` one canonical Falcon Foundation repository boundary;
- `S1-REQ-002` one canonical .NET solution;
- `S1-REQ-003` exact planned project structure and dependency direction;
- `S1-REQ-004` dependency locks and provenance records;
- `S1-REQ-005` isolated build and test environment usage;
- `S1-REQ-006` formatting, static-analysis, architecture-analysis, security-scan,
  secret-scan, dependency-scan, and verification commands;
- `S1-REQ-007` generated-artifact and secret-exclusion rules;
- `S1-REQ-008` version and artifact identity mechanism;
- `S1-REQ-009` traceability-output locations;
- `S1-REQ-010` evidence-output locations;
- `S1-REQ-011` deterministic and reproducible empty-build design;
- `S1-REQ-012` proof design for absence of financial dependencies, credentials,
  endpoints, data, and paths; and
- `S1-REQ-013` constitutional and Foundation/Application scope checks.

## Non-scope

Stage 1 SHALL NOT include:

- Kernel behavior;
- Authority Engine behavior;
- Lifecycle behavior;
- FIL runtime behavior;
- Service Bus runtime behavior;
- Guardian behavior;
- Health and Self-Awareness behavior;
- persistence behavior;
- Application logic;
- Stage 2 through Stage 9 implementation;
- production activity;
- cloud activity;
- external-connection activity; or
- financial activity.

## Scope sources

The scope above is constrained by the active canonical documents and by the
Stage 0 closure record:

- `docs/releases/FRS-001_FOUNDATION_RELEASE.md`
- `docs/plans/IMP-001_FOUNDATION_IMPLEMENTATION_WORK_PLAN.md`
- `docs/adrs/ADR-I001_FOUNDATION_RUNTIME_AND_LANGUAGE.md`
- `docs/adrs/ADR-I002_REPOSITORY_AND_DEPENDENCY_POLICY.md`
- `docs/adrs/ADR-I007_FOUNDATION_BUILD_VERIFICATION_AND_PROMOTION_PIPELINE.md`
- `docs/adrs/ADR-I008_FOUNDATION_BOOTSTRAP_AND_ACTIVATION_SEQUENCING.md`
- `docs/adrs/ADR-F001_FOUNDATION_EXECUTION_AND_ISOLATION_MODEL.md`
- `docs/governance/GOV-057_STAGE_0C_INTERIM_RESULTS_AND_SCOPED_ACTIVATION.md`
- `docs/governance/GOV-059_STAGE_0C_COMPLETION_AND_CLOSURE.md`

