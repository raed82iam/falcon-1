# Stage 6 WP-02 — Implementation Design and Pre-Implementation Red-Team

Status: AUTHORIZED / PRE-IMPLEMENTATION RED-TEAM PASS
Date: 2026-08-08

## Design

WP-02 extends the existing singular `Foundation.State` owner with an Application-neutral Foundation resource-truth model.

Each resource class records:

- total capacity;
- Foundation protection/survival floor;
- Foundation recovery reserve;
- derived allocatable capacity;
- exact evidence reference.

A snapshot binds:

- one Foundation resource epoch;
- one observation instant;
- one deterministic set of resource-class truth entries;
- one canonical SHA-256 identity over all material truth.

## Mandatory fail-closed rules

- unavailable truth cannot construct a valid snapshot;
- empty truth cannot construct a valid snapshot;
- duplicate resource-class truth is rejected;
- total/floor/reserve units must match exactly;
- protection floor + recovery reserve cannot exceed total capacity;
- allocatable capacity is derived and cannot be caller-supplied;
- entry evidence epoch must match snapshot epoch;
- evidence observation cannot be later than snapshot observation;
- mutation of any material quantity/evidence/epoch/time changes snapshot identity;
- ordering of equivalent resource entries does not change snapshot identity.

## Protection semantics

Protection floors and recovery reserves are Foundation-owned and non-reclaimable within WP-02 semantics. WP-02 exposes no method to reinterpret them as reclaimable Application capacity.

## Explicit exclusions

WP-02 has no Application allocation, priority, pressure/preemption, request/decision, reclamation, load-shedding, QoS, egress, credential or business-semantic API.

## Red-Team findings

### RT6W2-01 — Caller-supplied allocatable capacity
Severity: HIGH
Resolution: allocatable capacity SHALL be derived only from total minus protected floor minus recovery reserve.

### RT6W2-02 — Protected capacity accidentally reclaimable
Severity: CRITICAL
Resolution: protection floor and recovery reserve SHALL expose fixed `NonReclaimable` semantics and no caller-controlled reclaimability input.

### RT6W2-03 — Cross-epoch evidence mixing
Severity: HIGH
Resolution: every resource entry evidence epoch SHALL equal the snapshot epoch.

### RT6W2-04 — Duplicate resource truth
Severity: HIGH
Resolution: duplicate resource-class identities SHALL fail closed instead of last-write-wins.

### RT6W2-05 — Hidden Application policy leakage
Severity: CRITICAL
Resolution: public WP-02 production surface SHALL contain no Application grant/priority/TARC/Trading/load-shedding behavior.

## Gate

`PRE_IMPLEMENTATION_RED_TEAM = PASS`
`BLOCKERS = NONE`
`IMPLEMENTATION_MAY_PROCEED_WITHIN_WP02_ONLY`
