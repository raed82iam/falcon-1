# Stage 1 WP-02 Scope and Artifact Matrix

## Execution map

| Item | Canonical requirement | Exact path or identity | Permitted action | Verification | Evidence | Rollback |
| ---- | --------------------- | ---------------------- | ---------------- | ------------ | -------- | -------- |
| WP-02 | `S1-REQ-003` | `docs/stage-1-proposal/03_STAGE_1_IMPLEMENTATION_WORK_PACKAGE_PLAN.md` | establish project ownership and dependency direction | inspect project references and adapter separation | WP-02 evidence package | prior graph state |
| Core project | `S1-REQ-003` | `./src/Falcon.Foundation.Core/` | create controlled core project surface | inward-only dependency review | project-graph evidence | remove created project files |
| Contracts project | `S1-REQ-003` | `./src/Falcon.Foundation.Contracts/` | create controlled contracts surface | project-reference review | project-graph evidence | remove created project files |
| Infrastructure project | `S1-REQ-003` | `./src/Falcon.Foundation.Infrastructure/` | create controlled infrastructure boundary | adapter review | project-graph evidence | remove created project files |

## Expected WP-02 artifact family

- `src/Falcon.Foundation.Core/`
- `src/Falcon.Foundation.Contracts/`
- `src/Falcon.Foundation.Infrastructure/`
- any solution update required to reference them

## Prohibited-action matrix

| Area | Prohibited |
|---|---|
| files outside WP-02 | yes |
| unapproved projects | yes |
| project references outside approved boundary | yes |
| external packages | yes unless separately governed |
| external package sources | yes |
| runtime behavior | yes |
| business logic | yes |
| financial logic | yes |
| broker or market connectivity | yes |
| persistence | yes |
| cloud or production behavior | yes |
| behavioral testing | yes |
| WP-03 or later work | yes |

