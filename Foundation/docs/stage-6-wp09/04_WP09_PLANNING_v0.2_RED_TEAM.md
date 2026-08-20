# Stage 6 WP-09 — Planning v0.2 Red-Team

**Stage/WP:** Stage 6 WP-09 — Integration, Cross-Subsystem Consumption and Hardening  
**Artifact Reviewed:** `docs/stage-6-wp09/03_WP09_PLANNING_v0.2_FINAL_CANDIDATE.md`  
**Review Date:** 2026-08-10  
**Result:** FAIL / REVISION REQUIRED

## Findings

### HIGH-01 — Freshness model is too pressure-specific

WP-07 authoritative allocation mutation can advance accepted capacity state while the accepted WP-04 priority/technical-criticality snapshot still binds the earlier WP-03 allocation snapshot. WP-05 pressure then legitimately inherits that older WP-04/WP-03 lineage.

Therefore lagging context is not limited to pressure. WP-09 must treat predecessor dimensions as capable of advancing at different governed times. A lineage-valid older WP-04/WP-05 context may be coherent-but-lagging relative to a newer accepted WP-07 authoritative state. It must not be silently upgraded to current, but its age alone must not invalidate the accepted newer capacity state.

Required remediation: generalize freshness/temporal-relation status to all derived predecessor contexts whose accepted lineage predates a newer accepted state, while preserving contradiction detection.

### HIGH-02 — `current` wording risks inventing a latest-selector authority

The v0.2 artifact refers to current WP-07 state and applicable WP-06/WP-08 material without an explicit prohibition on selecting a latest record from history. Stage 6 has not established a generic canonical latest-event registry for requests, decisions, mutations or signals.

Required remediation: WP-09 SHALL accept explicitly supplied exact accepted predecessor/event references and an explicit integration as-of basis only. It SHALL NOT invent a `latest`, `most recent`, or timeline-selection mechanism. Applicability/current-validity is validated against the supplied exact material, not discovered by an unauthorized selector.

## Closed findings from v0.1

- v0.1 HIGH-01 temporal binary model: CLOSED in v0.2, subject to broader freshness remediation above.
- v0.1 HIGH-02 truth reissuance risk: CLOSED by reference-centric integration model.
- v0.1 HIGH-03 Application API ambiguity: CLOSED by Foundation-internal consumption plus existing-WP-08-only Application boundary.
- v0.1 MEDIUM-01 proactive predecessor-verifier rewriting: CLOSED by traced-remediation-only rule.

## Closure preservation

No finding proves a defect inside WP-01 through WP-08 accepted production scope. No predecessor closure is reopened.

## Disposition

- Critical: 0
- High: 2 open
- Medium: 0
- Result: REVISION REQUIRED

`WP09_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
`WP10_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`