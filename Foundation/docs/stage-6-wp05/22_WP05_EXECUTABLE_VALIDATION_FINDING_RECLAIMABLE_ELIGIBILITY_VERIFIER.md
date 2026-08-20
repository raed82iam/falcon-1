# Stage 6 WP-05 Executable Validation Finding — Reclaimable Eligibility Verifier

Status: REMEDIATION_REQUIRED
Scope: Stage 6 WP-05 verifier only
WP-05 production mutation required: NO
WP-04 closure reopened: NO

## Exact validation evidence

Exact tested commit: `8979fea6da4d2db7efcb11f77e9271d336db0258`

The exact-worktree validation passed:
- restore
- Release build with 0 warnings / 0 errors
- Foundation Architecture
- Foundation Security with 0 findings
- Stage 6 WP-01 51/51
- Stage 6 WP-02 34/34
- Stage 6 WP-03 45/45
- Stage 6 WP-04 48/48

Stage 6 WP-05 verifier Run 1 reached 30/31 PASS and failed only:
`reclaimable_allocation_is_eligibility_only`.

## Root cause

`ResourcePressureTruth` correctly exposes the truth property `PreemptionEligibleForConsideration` and does not expose a public preemption/reclamation executor method.

The verifier used `GetMethods(...)` and rejected every method name containing `Preempt` or `Reclaim`. Reflection includes compiler-generated property accessors such as `get_PreemptionEligibleForConsideration`, so the accepted eligibility-truth property was incorrectly classified as execution authority.

This is a verifier assertion defect, not a WP-05 production defect.

## Required remediation

The assertion SHALL inspect public non-special declared methods for execution semantics while permitting compiler-generated property accessors for the accepted truth property.

The remediation SHALL NOT:
- remove `PreemptionEligibleForConsideration`;
- add execution authority;
- change WP-05 production semantics;
- modify WP-01 through WP-04 production code;
- authorize WP-06/WP-07/WP-08 behavior.

After remediation, the complete exact-worktree validation SHALL be rerun from restore through two deterministic WP-05 verifier runs.
