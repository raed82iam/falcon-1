# Stage 6 WP-07 — Planning v0.2 Red-Team

Status: FINDINGS / REMEDIATION REQUIRED
Date: 2026-08-10
Target: `docs/stage-6-wp07/04_WP07_PLANNING_v0.2_RED_TEAM_REMEDIATED.md`

## Result

- Critical: 0
- High: 1
- Medium: 1

`WP07_PLANNING_v0.2 = NOT_READY_FOR_OWNER_ACCEPTANCE`

## HIGH-01 — Cross-Application effective capacity provenance was not explicit enough

v0.2 permits a target effective assignment to change inside the envelope while authoritative allocations remain unchanged.

Without exact capacity provenance, a target could appear to consume capacity beyond its own authoritative allocation with no exact source-grant attribution. That would create an opaque effective pool and weaken WP-03 attribution/accounting/isolation.

### Required remediation

Every cross-Application effective transfer must retain dual attribution:

- authoritative source Application and source grant from which capacity remains owned/allocated;
- effective target Application that is currently permitted to consume that borrowed capacity.

The effective-distribution model must distinguish native effective capacity from borrowed effective capacity and preserve source-grant provenance for every borrowed segment.

No borrowed quantity may exceed the source quantity validly made movable under the envelope. A target's native-plus-borrowed effective use must remain inside the target's envelope bound and authoritative ceiling.

Returning/reclaiming borrowed capacity must restore or release the same provenance-bound capacity, not an anonymous pool quantity.

## MEDIUM-01 — Effect adapter must remain environment-neutral

v0.2 requires `AppliedEffectEvidence`, which is correct, but it did not explicitly prohibit WP-07 from embedding Windows/Linux/container-specific enforcement semantics.

### Required remediation

Define a generic resource-effect adapter/port contract. WP-07 governs intent/effect evidence/commit semantics generically. Environment-specific realization/qualification remains a later separately evidenced concern and SHALL NOT redefine WP-07 semantics.

A logical/test effect adapter may be used for Stage 6 verification if it proves the generic transaction contract without claiming environment operational qualification.

## Preserved conclusions

The v0.2 fixes for canonical rebalance authority, capacity conservation, intent/effect/truth separation and current-state restoration are retained.

`WP01_WP06_CLOSURES_REOPENED = NO`
`WP07_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
