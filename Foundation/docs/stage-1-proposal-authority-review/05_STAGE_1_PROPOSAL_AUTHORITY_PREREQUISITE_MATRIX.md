# 05 - Stage 1 Proposal Authority Prerequisite Matrix

| Prerequisite ID | Requirement | Canonical source | Evidence required | Severity | Owner decision required | Status |
|---|---|---|---|---|---|---|
| P-01 | Stage 0 must be complete and closed | `docs/reviews/STAGE_SEQUENCE_STATE_RECONCILIATION_001.md` | Closed current-state reconciliation and closure records | High | No | SATISFIED |
| P-02 | Stage 1 proposal authority must remain distinct from execution authority | `docs/stage-1-proposal-authority-review/02_STAGE_1_PROPOSAL_AUTHORITY_SCOPE.md` | Explicit scope and non-scope language | High | No | SATISFIED |
| P-03 | The Owner must decide whether to grant proposal authority | Canonical governance record | Owner approval statement | High | Yes | OWNER_DECISION_REQUIRED |
| P-04 | No Stage 0 reopening may occur | `docs/reviews/STAGE_SEQUENCE_STATE_RECONCILIATION_001.md` | Closed sequence and exhausted authority record | High | No | SATISFIED |
| P-05 | No implementation, deployment, or execution may start | Current canonical baseline | Proposal package only; no runtime evidence required | High | No | SATISFIED |
| P-06 | No Stage 1 execution authority may be inferred from proposal authority | Current canonical baseline | Explicit authority separation | High | No | SATISFIED |
| P-07 | Readiness packages must not be treated as current-state authority | `docs/stage-0a-readiness/`, `docs/stage-1-readiness/` | Current-state classification of those packages | Medium | No | SATISFIED |
| P-08 | A complete Stage 1 proposal framework must be defined in the governing proposal deliverables before proposal authority is considered | `docs/stage-1-proposal-authority-review/03_STAGE_1_PROPOSAL_REQUIRED_DELIVERABLES.md` | Required deliverable list and proposal framework definition | Medium | Yes | SATISFIED |

## Matrix notes

- No Stage 1 execution evidence is required before proposal authority.
- `OWNER_DECISION_REQUIRED` is used only when the decision itself must come
  from the Owner.
- `SATISFIED` for P-08 means the framework is already defined by the required
  deliverables package, so the authority decision is not blocked by circular
  dependency on the future proposal itself.
