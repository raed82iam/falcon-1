# Stage 6 WP-09 — Planning v0.1 Red-Team

**Stage/WP:** Stage 6 WP-09 — Integration, Cross-Subsystem Consumption and Hardening  
**Artifact Reviewed:** `docs/stage-6-wp09/01_WP09_PLANNING_v0.1.md`  
**Review Date:** 2026-08-10  
**Result:** FAIL / REVISION REQUIRED

## Findings

### HIGH-01 — Temporal coherence model is too binary

The v0.1 wording can be read as requiring all integrated predecessor material to represent one exact current moment. That is unsafe after accepted WP-07 capacity mutation because the most recent accepted pressure observation may legitimately predate the mutation. WP-09 must distinguish coherent-but-lagging observational context from contradictory lineage. It must never present stale pressure as current, but it also must not invalidate an otherwise accepted capacity state merely because pressure has not yet been re-observed.

Required remediation: define explicit freshness/temporal-relation status per observational predecessor, with fail-closed `Unavailable/Stale` treatment where current pressure context is required.

### HIGH-02 — Integrated snapshot could accidentally become a new truth source

The v0.1 integrated snapshot field list risks copying predecessor quantities/states and reissuing them as integrated truth. This could create a de-facto new source of allocation, pressure or capacity truth contrary to `INTEGRATED_VIEW != NEW_TRUTH_SOURCE`.

Required remediation: make WP-09 integration binding/reference-centric. It may retain exact predecessor objects/identities and derive coherence/freshness status, but authoritative values remain owned by their predecessor objects.

### HIGH-03 — Cross-subsystem consumption boundary is ambiguous about Application API ownership

The v0.1 wording mixes Foundation cross-subsystem consumption with already-approved WP-08 Application-facing projection consumption. WP-09 must not create a second Application-facing resource API or runtime authorization/admission surface.

Required remediation: define WP-09 consumption as Foundation-internal coherence packaging plus verification that already-accepted WP-08 Application-facing projections/signals remain coherent when consumed. No new Application-facing authority or API family is introduced.

### MEDIUM-01 — Successor-compatible verifier hardening could be interpreted as proactive predecessor rewriting

WP-09 should not authorize blanket edits to accepted predecessor verifiers merely because shared namespaces may later gain successor types.

Required remediation: successor-compatibility is a verifier governance rule; predecessor verifier changes occur only under exact traced failure or concrete static defect, preserving original invariant and requiring fresh review/regression evidence.

## Closure preservation

No finding proves a defect inside WP-01 through WP-08 accepted production scope. No predecessor closure is reopened.

## Disposition

- Critical: 0
- High: 3 open
- Medium: 1 open
- Result: REVISION REQUIRED

`WP09_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`
`WP10_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`