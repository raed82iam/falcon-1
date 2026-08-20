# 09 - Stage 1 Evidence Model

## Evidence classes

| Scenario ID | Evidence ID | Evidence class | Producer | Collector | Evaluator | Completeness authority | Exact artifact | Path | Identity and digest rule | Retention | Challenge rule | Correction rule |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| VS-01 | EV-01 | PRE_EXECUTION_AUTHORIZATION_EVIDENCE | proposal authoring role | proposal package owner | owner review | owner decision package | boundary map | `docs/stage-1-proposal/02_STAGE_1_FOUNDATION_COMPONENT_AND_PATH_BOUNDARY.md` | immutable text with digest | retain with package | challenge by cross-reference only | replace only through documented package revision |
| VS-02 | EV-02 | PRE_EXECUTION_AUTHORIZATION_EVIDENCE | proposal authoring role | proposal package owner | architecture reviewer | owner decision package | architecture map | `docs/stage-1-proposal/05_STAGE_1_ARCHITECTURE_IMPACT_REVIEW.md` | immutable text with digest | retain with package | challenge by exact citation | replace only through documented package revision |
| VS-03 | EV-03 | PRE_EXECUTION_AUTHORIZATION_EVIDENCE | proposal authoring role | proposal package owner | solution reviewer | owner decision package | solution identity report | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md` | immutable text with digest | retain with package | challenge by exact citation | replace only through documented package revision |
| VS-04 | EV-04 | PRE_EXECUTION_AUTHORIZATION_EVIDENCE | proposal authoring role | proposal package owner | environment reviewer | owner decision package | environment admission report | `docs/stage-1-proposal/07_STAGE_1_ENVIRONMENT_TOOLCHAIN_AND_RESOURCE_PLAN.md` | immutable text with digest | retain with package | challenge by exact citation | replace only through documented package revision |
| VS-05 | EV-05 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | offline restore log | `artifacts/evidence/stage-1/repository-inventory.json` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-06 | EV-06 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | lock/provenance report | `artifacts/evidence/stage-1/dependency-provenance-report.json` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-07 | EV-07 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | formatter and analyzer logs | `artifacts/evidence/stage-1/formatter-and-analyzer-log.txt` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-08 | EV-08 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | scan report | `artifacts/evidence/stage-1/secret-scan-report.txt` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-09 | EV-09 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | exclusion proof | `artifacts/evidence/stage-1/financial-path-exclusion-report.txt` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-10 | EV-10 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | analysis report | `artifacts/evidence/stage-1/architecture-analysis-report.txt` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-11 | EV-11 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | build log | `artifacts/evidence/stage-1/empty-build.log` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-12 | EV-12 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | repeat-build report | `artifacts/evidence/stage-1/reproducibility-report.txt` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-13 | EV-13 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | artifact identity report | `artifacts/evidence/stage-1/generated-artifact-inventory.json` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-14 | EV-14 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | trace output | `artifacts/evidence/stage-1/traceability-export.json` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-15 | EV-15 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | evidence output | `artifacts/evidence/stage-1/evidence-inventory.json` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |
| VS-16 | EV-16 | POST_STAGE_1_ACCEPTANCE_EVIDENCE | future acceptance work | future collector | future acceptance evaluator | future acceptance authority | residual-risk report | `artifacts/evidence/stage-1/residual-risk-report.txt` | immutable acceptance artifact with digest | retain under acceptance policy | challenge by exact citation | replace only by new acceptance evidence |
| VS-17 | EV-17 | PRE_EXECUTION_AUTHORIZATION_EVIDENCE | proposal authoring role | proposal package owner | reviewer | owner decision package | behavior-exclusion report | `docs/stage-1-proposal/01_STAGE_1_PURPOSE_SCOPE_AND_NON_SCOPE.md` | immutable text with digest | retain with package | challenge by exact citation | replace only through documented package revision |
| VS-18 | EV-18 | STAGE_1_EXECUTION_EVIDENCE | future execution work | future collector | future evaluator | future execution authority | environment revalidation report | `artifacts/evidence/stage-1/environment-admission-report.txt` | immutable execution artifact with digest | retain under evidence policy | challenge by exact citation | replace only by new execution evidence |

## Evidence rules

- Proposal documents and the Authority Instrument draft are
  PRE_EXECUTION_AUTHORIZATION_EVIDENCE.
- Restore logs, scan results, dependency inventories, build outputs, artifact
  hashes, and reproducibility reports are STAGE_1_EXECUTION_EVIDENCE.
- Final acceptance, completion, residual-risk, and closure evidence are
  POST_STAGE_1_ACCEPTANCE_EVIDENCE.
- Future execution evidence must point to exact planned paths under
  `artifacts/evidence/stage-1/`.
- It must not point to proposal documents as though they were completed
  execution results.
- Self-referential evidence mappings are prohibited.
