# FSATS V1.4 Part 0 / P0-G — Exact Current Candidate Binding

**Status:** `BOUND_FOR_FRESH_FINAL_REVIEW`
**Final Owner acceptance:** `NOT_GRANTED`

## Exact semantic set under review

1. `102_P0G_START_AND_SCOPE_CONTROL_RECORD.md`
2. `103_P0G_CANONICAL_FSAPMA_OPERATIONAL_DATA_ARCHITECTURE_CANDIDATE.md`
   - blob SHA: `62dbb35f5264d43628e209dfac59490d945efb3e`
3. `103A_P0G_ENTITLEMENT_STREAM_CONTINUITY_ADJUSTMENT_AND_PRECISION_HARDENING.md`
   - blob SHA: `a03902bb804d11990c62f8f14f5ab1b0f2dae614`
4. `103B_P0G_EXTERNAL_PROVIDER_EGRESS_FCR_AND_FAIL_CLOSED_HARDENING.md`
   - blob SHA: `d9d04e82d49eaa0de7c133adfa8460bba06af5d9`
5. `104_P0G_INITIAL_ARCHITECTURE_AND_RED_TEAM_REPORT.md` as historical failing review evidence only.

## External governance evidence

- FCR-0005 / GitHub Issue #5 remains open and governs generic FSAPMA-to-internal-consumer operational-data delivery need.
- FCR-0013 / GitHub Issue #13 is `SUBMITTED` and governs the missing generic operational external-provider egress/credential-reference boundary.
- Current Foundation state remains the source of truth for accepted/authorized capabilities; P0-G does not convert an FCR into implementation authority.

## Effective interpretation order

Where 103 conflicts with a later hardening record:

1. 103B controls operational external-egress gap/fail-closed semantics;
2. 103A controls entitlement, stream continuity, adjustment basis, precision and operational truth terminology;
3. 103 supplies the base architecture for all remaining semantics.

No historical report may override the current semantic set.

`P0G_EXACT_CANDIDATE = BOUND`
`P0G_FRESH_FULL_REVIEW = REQUIRED`
