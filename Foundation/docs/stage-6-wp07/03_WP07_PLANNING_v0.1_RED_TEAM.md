# Stage 6 WP-07 — Planning v0.1 Red-Team

Status: FINDINGS / REMEDIATION REQUIRED
Date: 2026-08-10
Target: `docs/stage-6-wp07/02_WP07_PLANNING_v0.1.md`

## Result

- Critical: 0
- High: 3
- Medium: 1

`WP07_PLANNING_v0.1 = NOT_READY_FOR_OWNER_ACCEPTANCE`

## HIGH-01 — Rebalance was too close to an invented canonical decision kind

v0.1 listed `Foundation-authoritative Rebalance` beside canonical mutation outcomes.

The accepted canonical resource decision set does not establish `Rebalance` as an independent decision kind. Treating it as one would risk silently inventing authority.

### Required remediation

Define `Rebalance` only as a governed atomic transaction/batch composed from exact separately authorized canonical mutation actions. It must not mint a new decision/authority kind by wording alone.

## HIGH-02 — Coordination-envelope capacity accounting was under-specified

v0.1 did not state tightly enough which capacity can become part of the internal movable pool.

Quota/ceiling headroom is not the same as currently allocated/granted capacity. If the envelope treated headroom as already available, FSARM could effectively mint capacity without Foundation grant authority.

### Required remediation

Define at least these quantities distinctly:

- authoritative current allocation per Application/resource class;
- authoritative quota;
- authoritative ceiling;
- protected effective minimum;
- current effective assignment;
- envelope movable capacity.

The envelope movable pool must be explicitly authorized and derived only from capacity the Foundation has already made available to the governed constituent set. Unused quota/ceiling headroom alone is not granted capacity.

A target effective assignment may exceed its current effective assignment and, if explicitly allowed by the envelope, its current authoritative allocation only as an operational effective-distribution fact, but it may never exceed its authoritative ceiling or create a new Foundation grant. The envelope total must conserve already-authorized capacity.

## HIGH-03 — Recorded state could be confused with actually applied mutation

v0.1 described mutation processors/records but did not explicitly separate mutation intent, applied-effect evidence and accepted post-mutation truth.

This could allow a ledger update to claim reclamation/redistribution occurred even if the underlying enforcement/effect operation failed or was only partially applied.

### Required remediation

Establish:

`MUTATION_INTENT != APPLIED_EFFECT_EVIDENCE != ACCEPTED_POST_MUTATION_TRUTH`

No effective-distribution truth may advance until exact effect evidence confirms the intended atomic mutation was applied, or the implementation proves an in-process atomic mechanism where commit and effect are inseparable.

A failed/partial effect must not publish the intended post-state as accepted truth.

## MEDIUM-01 — Restoration target semantics needed tighter current-state binding

v0.1 correctly rejected blind restoration but did not explicitly state that the pre-reclaim state is historical evidence only, not an automatically valid restoration target.

### Required remediation

Restoration must be recalculated against current authoritative allocation/envelope/pressure/eligibility/fencing state. Historical state can be an input, not automatic authority.

## Preserved conclusions

The two-lane architecture remains valid:

1. delegated effective-distribution mutation inside a valid envelope;
2. Foundation-authoritative mutation under exact Foundation authority.

The Red-Team found no reason to merge those lanes and no reason to reopen WP-01 through WP-06.

`WP01_WP06_CLOSURES_REOPENED = NO`
`WP07_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
