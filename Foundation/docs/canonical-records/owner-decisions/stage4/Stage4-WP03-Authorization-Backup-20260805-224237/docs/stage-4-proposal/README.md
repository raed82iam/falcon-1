# Stage 4 Proposal

## Canonical Stage

**Title:** Authority, Lifecycle, State, and Evidence  
**Purpose:** Establish accountable state change.

## Current State

```text
FALCON_FOUNDATION_STAGE3_ACCEPTED_AND_CLOSED
FALCON_FOUNDATION_STAGE4_PLANNING_AUTHORIZED
FALCON_FOUNDATION_STAGE4_IMPLEMENTATION_UNAUTHORIZED
```

## Authority

Planning is authorized under:

- `../governance/GOV-102_STAGE_4_PLANNING_AUTHORIZATION.md`
- `01_STAGE_4_PLANNING_AUTHORITY.md`

## Canonical Source Basis

Stage 4 planning is derived from:

- the canonical Stage 4 definition in `../plans/IMP-001_FOUNDATION_IMPLEMENTATION_WORK_PLAN.md` and its preserved approved predecessor;
- `../contracts/CON-002_AUTHORITY_DECISION.md`;
- `../contracts/CON-003_LIFECYCLE.md`;
- `../foundation/FDN-001_STATE_AUTHORITY_AND_PERSISTENCE_CATALOG.md`;
- `../verification/VPL-002_UNAUTHORIZED_ACTION.md`;
- `../verification/VPL-003_INVALID_LIFECYCLE_TRANSITION.md`;
- the accepted Stage 3 lifecycle and bootstrap implementation.

## Approved Planning Candidate

The Stage 4 planning candidate is divided into six sequential Work Packages:

1. WP-01 — Default-Deny Authority Engine.
2. WP-02 — Authoritative Lifecycle Integration and Hardening.
3. WP-03 — State Ownership and Durable Current-State Persistence.
4. WP-04 — Integrity-Linked Evidence Journal and Immutable Accepted Facts.
5. WP-05 — Concurrency, Uncertain Writes, and Restart Reconciliation.
6. WP-06 — Integrated VPL-002 and VPL-003 Verification and Closure.

See:

- `02_STAGE_4_SCOPE_AND_ARCHITECTURE.md`
- `03_STAGE_4_IMPLEMENTATION_WORK_PACKAGE_PLAN.md`
- `04_STAGE_4_REQUIREMENT_AND_VERIFICATION_MATRIX.md`
- `05_STAGE_4_FAILURE_ROLLBACK_AND_NON_AUTHORITY.md`
- `06_STAGE_4_PLANNING_REVIEW_CHECKLIST.md`
- `07_STAGE_4_STATE_CLASS_SCOPE_AND_OWNERSHIP.md`
- `08_STAGE_4_VPL002_FIL_PATH_RESOLUTION.md`
- `09_STAGE_4_CANDIDATE_IMPLEMENTATION_BOUNDARIES.md`

## Planning Amendment Status

The independent planning review identified three gaps:

- exact FDN-001 state-class scope;
- VPL-002 FIL-path handling before Stage 5;
- candidate implementation boundaries.

Documents 07 through 09 close those planning gaps.

## Documentation Boundary

Documents inside this folder guide development, review, governance, and traceability.

They are not:

- runtime authority sources;
- executable policies;
- runtime configuration;
- operational state;
- substitutes for code contracts or verified runtime data.

## Independent Planning Review

The renewed independent review passed.

See:

- `../reviews/STAGE_4_INDEPENDENT_PLANNING_REVIEW.md`

## WP-01 Authority Candidate

The bounded WP-01 implementation authority is defined in:

- `10_STAGE_4_WP01_IMPLEMENTATION_AUTHORITY.md`
- `../governance/GOV-103_STAGE_4_WP01_IMPLEMENTATION_AUTHORIZATION.md`

The authority is effective only after exact Owner confirmation through the authorization script.

## WP-01 Final Acceptance

WP-01 passed renewed independent implementation review and was accepted and closed by the Owner.

See:

- `11_STAGE_4_WP01_FINAL_ACCEPTANCE_AND_CLOSURE.md`
- `../reviews/STAGE_4_WP01_RENEWED_INDEPENDENT_IMPLEMENTATION_REVIEW.md`
- `../governance/GOV-104_STAGE_4_WP01_FINAL_OWNER_ACCEPTANCE_AND_CLOSURE.md`

```text
FALCON_FOUNDATION_STAGE4_WP01_ACCEPTED_AND_CLOSED
```

## Implementation Boundary

WP-02 through WP-06 remain unauthorized.

Commit, tag, merge, rebase, push, deployment, and runtime activation remain unauthorized.

## WP-02 Final Acceptance

WP-02 passed renewed independent implementation review and was accepted and closed by the Owner.

See:

- 13_STAGE_4_WP02_FINAL_ACCEPTANCE_AND_CLOSURE.md
- ../reviews/STAGE_4_WP02_RENEWED_INDEPENDENT_IMPLEMENTATION_REVIEW.md
- ../governance/GOV-106_STAGE_4_WP02_FINAL_OWNER_ACCEPTANCE_AND_CLOSURE.md

```text
FALCON_FOUNDATION_STAGE4_WP02_ACCEPTED_AND_CLOSED
```

## Implementation Boundary

WP-03 through WP-06 remain unauthorized.

Commit, tag, merge, rebase, push, deployment, and runtime activation remain unauthorized.

