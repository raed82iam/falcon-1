# VPL-005 — Health Evidence Loss Plan

**Identifier:** VPL-005  
**Version:** 1.1  
**Status:** Proposed  
**Canonical Target:** `docs/verification/VPL-005_HEALTH_EVIDENCE_LOSS.md`  
**Approval Record:** Pending  
**Owner:** Falcon Verification Governance  
**Governing Authority:** GOV-063; AWR-001 v2.1; CON-006; AUT-001; AUT-002; GOV-AUT-001  
**Activation Authority:** Not Granted  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted  

## 1. Purpose

VPL-005 defines the documentary verification plan for health evidence loss. It identifies evidence-loss modes, the resulting effects on confidence, fitness, authority, restriction, isolation, and recovery, and the documentary evidence required to support later governed decisions.

It does not itself decide fitness, authority, or recovery. Those decisions remain with AWR-001, CON-006, Authority Engine, Guardian, Health Monitoring, and Recovery.

## 2. Scope

VPL-005 governs documentary verification planning for:

- missing evidence;
- stale evidence;
- delayed evidence;
- contradictory evidence;
- unverifiable evidence;
- inaccessible evidence;
- corrupted evidence;
- provenance failure;
- partial visibility;
- last-known-state preservation;
- evidence-age tracking;
- notification, restriction, isolation, recovery, and audit behavior; and
- fail-closed handling when health evidence is insufficient.

## 3. Non-Scope

VPL-005 does not:

- generate health truth by itself;
- grant authority;
- grant activation authority;
- grant implementation authority;
- grant Stage 1 authority;
- interpret Application business meaning;
- interpret Application financial meaning;
- perform runtime verification execution; or
- replace the authority of AWR-001, CON-006, AUT-001, AUT-002, or Recovery.

## 4. Owners and Authority Boundaries

- The plan owner is Falcon Verification Governance.
- AWR-001 owns Foundation fitness interpretation.
- CON-006 owns health and fitness contract semantics.
- Authority Engine owns authority decisions.
- Guardian owns protective restrictions.
- Health Monitoring owns observed health evidence.
- Recovery owns recovery execution.

No entity may treat this plan as a source of truth. It is a governed plan for producing evidence, not a governing truth object.

## 5. Evidence-Loss Classes

VPL-005 recognizes the following evidence-loss classes:

- `MISSING`
- `STALE`
- `DELAYED`
- `CONTRADICTORY`
- `UNVERIFIABLE`
- `INACCESSIBLE`
- `CORRUPTED`
- `PROVENANCE_FAILURE`
- `PARTIAL_VISIBILITY`

## 6. Normative Requirements

- **VPL-005-REQ-001:** Every health-evidence assessment SHALL declare the evidence-loss class, subject, scope, and observation time.
- **VPL-005-REQ-002:** Missing evidence SHALL reduce confidence and SHALL prevent healthy or fit inference for the affected scope.
- **VPL-005-REQ-003:** Stale evidence SHALL remain visible with age and SHALL not be treated as fresh.
- **VPL-005-REQ-004:** Delayed evidence SHALL remain pending until arrival or expiry and SHALL not be converted into a positive health claim.
- **VPL-005-REQ-005:** Contradictory evidence SHALL remain explicit and SHALL trigger challenge, not collapse.
- **VPL-005-REQ-006:** Unverifiable evidence SHALL be treated as insufficient for reliance.
- **VPL-005-REQ-007:** Inaccessible evidence SHALL reduce fitness and MAY trigger restriction or isolation.
- **VPL-005-REQ-008:** Corrupted evidence SHALL be rejected for reliance and preserved only as failure evidence.
- **VPL-005-REQ-009:** Provenance failure SHALL invalidate the affected evidence relation.
- **VPL-005-REQ-010:** Partial visibility SHALL be treated as incomplete evidence, not implicit confirmation.
- **VPL-005-REQ-011:** Unknown or insufficient evidence SHALL NEVER be converted into healthy, fit, or authorized status.
- **VPL-005-REQ-012:** The last trustworthy state MAY be preserved only with explicit age, source, and expiration constraints.
- **VPL-005-REQ-013:** A last trustworthy state SHALL become unusable when its freshness window, evidence relation, or governing policy expires.
- **VPL-005-REQ-014:** Evidence loss SHALL produce notifications to the governing authority paths declared by AWR-001, CON-006, Authority Engine, Guardian, and Health Monitoring.
- **VPL-005-REQ-015:** Evidence loss MAY require restriction, isolation, recovery, or audit activation, but this plan SHALL NOT itself impose those controls.
- **VPL-005-REQ-016:** Evidence-loss handling SHALL preserve failure evidence, preserved state, and reconstruction data.
- **VPL-005-REQ-017:** Evidence-loss handling SHALL remain challengeable and reproducible.

## 7. Loss-to-Effect Mapping

| Loss class | Confidence effect | Fitness effect | Authority effect | Notes |
|---|---|---|---|---|
| `MISSING` | severe reduction | not fit for affected scope | deny or restrict as governed | no assumption of healthy state |
| `STALE` | reduction proportional to age | fit only if policy permits and scope remains valid | may require restriction | age must be visible |
| `DELAYED` | pending, no positive conclusion | pending or restricted | no positive authority inference | time-bound pending state |
| `CONTRADICTORY` | uncertainty increases | not fit until resolved | challenge required | contradiction remains explicit |
| `UNVERIFIABLE` | low confidence | not fit | no reliance | cannot be self-confirmed |
| `INACCESSIBLE` | confidence limited | not fit or restricted | containment possible | access failure is evidence |
| `CORRUPTED` | invalid confidence basis | not fit | reject | corruption is failure evidence |
| `PROVENANCE_FAILURE` | trust collapse for affected chain | not fit | deny reliance | provenance chain broken |
| `PARTIAL_VISIBILITY` | incomplete confidence | restricted or not fit | do not infer completeness | partial view is not truth |

## 8. Last Known Trustworthy State

The last trustworthy state MAY be preserved when:

- the source was previously authoritative;
- the evidence age is declared;
- the freshness limit remains valid;
- the governing policy explicitly allows fallback to last-known state; and
- the preserved state is marked as last-known, not current.

The last trustworthy state SHALL become invalid for reliance when:

- freshness expires;
- provenance is lost;
- the subject changes materially;
- a contradiction is unresolved;
- a higher-quality current source becomes available and conflicts;
- isolation or recovery policy requires a harder fail-closed state; or
- AWR-001 or CON-006 declares the state not fit for the requested scope.

## 9. Notification, Restriction, Isolation, Recovery, and Audit Behavior

When evidence loss occurs:

- Health Monitoring SHALL publish the evidence-loss observation.
- AWR-001 SHALL reduce the relevant fitness or mark it unknown.
- CON-006 SHALL map the loss to fitness result behavior.
- Authority Engine SHALL decide whether authority remains, is restricted, or is denied.
- Guardian MAY impose restriction or isolation according to governing policy.
- Recovery MAY begin only when authorized by the governing recovery path.
- Audit records SHALL preserve the loss class, timeline, evidence identity, and result.

## 10. Invariants

1. Evidence loss does not create evidence.
2. Unknown is not healthy.
3. Insufficient is not fit.
4. Contradiction is not confirmation.
5. The plan documents behavior; it does not make the decision.
6. Fail-closed is safer than assumed trust.

## 11. Acceptance Evidence

Acceptance requires documentary examples for each loss class, each mapping outcome, last-known-state preservation and expiry, challenge behavior, notification behavior, restriction and isolation triggers, recovery gating, and audit preservation.

## 12. Verification Criteria

This plan is acceptable only if it can be used by the governing review path to explain:

- why confidence dropped;
- why fitness changed;
- why authority was restricted or denied;
- which evidence remained usable;
- which evidence became stale or invalid; and
- how the original state can be reconstructed.

## 13. Preservation Annex: Active Edition Procedure and Pass Rule

The active verification plan content remains preserved below so the successor remains self-contained and directly reviewable.

### 13.1 Verification Objective

Prove that stale, missing, contradictory, or unverifiable required health evidence becomes explicit uncertainty, reduces Fitness to Operate, and denies the affected authority.

### 13.2 Scope and Non-Scope

This plan verifies Foundation operational evidence, scoped health, Self-Awareness, Fitness, and authority reduction. It does not claim human consciousness, full Self-Awareness, predictive intelligence, or financial fitness.

### 13.3 Required Setup

- one admitted component with declared health evidence requirements;
- defined freshness, provenance, completeness, and confidence thresholds;
- one action dependent on satisfactory Fitness;
- controllable evidence source and clock-quality input; and
- an unaffected control capability with independent evidence.

### 13.4 Procedure

1. Establish valid fresh evidence and confirm the expected health and scoped Fitness result.
2. Stop the required evidence source until freshness expires.
3. Repeat with missing provenance, failed integrity, contradictory evidence, and unacceptable clock quality.
4. Request the Fitness-dependent action under each condition.
5. Restore evidence without yet satisfying the stability requirement.
6. Provide sufficient new evidence and perform the required independent reassessment.
7. Confirm unaffected capability behavior remains separate where isolation is trustworthy.

### 13.5 Expected Results

- Health becomes `UNKNOWN` or the explicitly governed degraded result; it never remains falsely healthy.
- Fitness is reduced for the affected scope.
- Authority dependent on missing trust is denied.
- Stale cached success does not override unknown current evidence.
- Restoration of the evidence source alone does not silently restore authority.
- Contradiction, blind spot, and clock limitation remain visible.

### 13.6 Required Evidence

Evidence requirements, observations, source identity, freshness and clock data, integrity results, health assessments, Self Model change, Fitness decisions, authority denials, restoration assessment, and correlated state history.

### 13.7 Pass Rule

`PASS` requires every evidence-loss variant to produce explicit uncertainty and consequence-appropriate denial, followed by independently evidenced restoration. Any optimistic continuation based on unknown required evidence is an immediate `FAIL`.

### 13.8 Independent Verification

The Independent Verifier shall control or observe evidence withdrawal independently and shall compare raw evidence availability with Health, Self-Awareness, Fitness, and Authority results.

### 13.9 Containment, Cleanup, and Repeatability

Evidence withdrawal shall affect only the declared test scope. Cleanup shall restore evidence through a new attributable observation and independently reassess Fitness; stale success shall not be reused. Repetition shall reset freshness and clock conditions explicitly.
