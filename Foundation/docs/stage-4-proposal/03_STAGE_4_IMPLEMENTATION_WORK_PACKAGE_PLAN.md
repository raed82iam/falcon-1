# Stage 4 Implementation Work Package Plan

> This document is a planning artifact only. It does not authorize implementation.

## Controlling Scope Documents

This plan is bounded by:

- `07_STAGE_4_STATE_CLASS_SCOPE_AND_OWNERSHIP.md`
- `08_STAGE_4_VPL002_FIL_PATH_RESOLUTION.md`
- `09_STAGE_4_CANDIDATE_IMPLEMENTATION_BOUNDARIES.md`

## WP-01 — Default-Deny Authority Engine

### Purpose

Implement the reusable Authority Decision boundary required by CON-002.

### Deliverables

- canonical Authority Request model;
- canonical Authority Result model;
- default-deny evaluator;
- actor, action, resource, purpose, and scope validation;
- policy identity and version binding;
- expiry validation;
- delegation and revocation validation;
- fitness and security-context inputs;
- deterministic decision identity;
- attributable denial evidence;
- explicit separation between decision and execution.

### Required cases

- allow;
- deny;
- missing identity;
- missing authority provenance;
- excessive requested scope;
- expired authority;
- revoked delegation;
- insufficient fitness;
- conflicting policy;
- deterministic reconstruction.

### Exit

CON-002 requirements 001 through 008 are proven.

No execution or state mutation occurs inside Authority Engine.

---

## WP-02 — Authoritative Lifecycle Integration and Hardening

### Purpose

Integrate WP-01 with the accepted Stage 3 lifecycle implementation without duplicating it.

### Deliverables

- CON-002 Authority Result consumption at the lifecycle execution boundary;
- authoritative source-state validation;
- legal transition validation;
- stale-source rejection;
- duplicate-transition handling;
- conflicting-transition handling;
- unauthorized-request rejection;
- actual-state reporting after failure;
- exactly one event for each completed transition;
- independent recovery-validation evidence;
- preservation of Guardian restrictions as distinct from routine transitions.

### Existing Stage 3 behavior to preserve

- deterministic lifecycle vocabulary;
- versioned state snapshots;
- transition attempts;
- transition events;
- contract rejections;
- Bootstrap Context and Dependency evidence binding;
- controlled release and restriction rules.

### Exit

CON-003 requirements 001 through 008 are proven without regression of Stage 3 WP-01 through WP-06.

---

## WP-03 — State Ownership and Durable Current-State Persistence

### Purpose

Implement the FDN-001 authoritative-state rules and durable current-state model for the exact state classes listed in document 07.

### Deliverables

- state-class model;
- authoritative owner declaration;
- persistence owner;
- read and write authority declaration;
- singular write authority;
- versioned authoritative state;
- explicit source and effective time;
- retention classification;
- reconstruction and audit relations;
- compare-expected-version update rule;
- immutable prior-state history;
- explicit labels for derived, cached, observed, last-known, expected, desired, and historical state;
- Application business-state isolation.

### Required failure behavior

Missing, stale, conflicting, corrupted, or partial state must be explicit.

No fallback, cache, observation, or shadow copy may become authoritative accidentally.

### Exit

FDN-001 requirements 001 through 022 are mapped and proven for every state class named in document 07.

---

## WP-04 — Integrity-Linked Evidence Journal and Immutable Accepted Facts

### Purpose

Make decisions and accepted state effects reconstructable and tamper-evident.

### Deliverables

- append-only evidence record;
- unique evidence identity;
- integrity link to prior accepted record;
- actor, decision, request, state version, source, time, and reason;
- decision evidence for both allow and deny;
- execution-boundary evidence;
- persistence outcome evidence;
- immutable event for each accepted completed fact;
- no accepted event for rejected or incomplete actions;
- gap, deletion, insertion, replacement, reorder, and duplication detection;
- correction by new evidence, never rewriting history.

### Exit

Every accepted fact is linked to a proven durable state change.

Every denial is attributable and reconstructable.

---

## WP-05 — Concurrency, Uncertain Writes, and Restart Reconciliation

### Purpose

Ensure one authoritative successor and truthful recovery under conflict or interruption.

### Deliverables

- expected-version concurrency;
- exactly one successful writer per authoritative version;
- duplicate logical request handling;
- conflicting duplicate detection;
- stale-write rejection;
- uncertain-write classification;
- lookup by request and decision identity;
- no blind retry;
- reconciliation of state, decision, evidence, and accepted event;
- restart recovery from last trusted authoritative state;
- explicit divergence states;
- no fabricated or regressed state;
- challengeable reconstructed state where evidence is incomplete.

### Required scenarios

- two competing writes;
- same request replay;
- same request identity with changed content;
- timeout before commit;
- timeout after commit;
- crash during result return;
- state ahead of evidence;
- evidence ahead of state;
- corrupted current state;
- truncated evidence journal;
- restart after uncertain write.

### Exit

Conflict and restart behavior satisfy FDN-001 and the setup required by VPL-003.

---

## WP-06 — Integrated VPL-002 and VPL-003 Verification and Closure

### Purpose

Prove Stage 4 end to end.

### VPL-002 track

VPL-002 uses the verification-only FIL boundary adapter defined in document 08.

Required proof:

- permitted control action works separately;
- prohibited action is denied;
- expired delegation is denied;
- revoked delegation is denied;
- retry and replay do not create permission;
- the verification-only FIL-modeled path denies the action;
- all declared direct execution paths deny the action;
- authoritative state remains unchanged;
- denial is attributable and reconstructable;
- independent verifier checks the execution boundary and state owner directly.

### VPL-003 track

Prove that Lifecycle preserves one authoritative state.

Required proof:

- one valid transition succeeds;
- invalid target is rejected;
- stale prior state is rejected;
- duplicate request is handled safely;
- competing requests produce one authoritative successor;
- unauthorized transition is rejected;
- restart reconciles trusted state;
- failed attempts remain visible;
- no false success event exists;
- independent verifier compares evidence with durable state.

### Regression

- clean Release build;
- Architecture tests;
- Security tests;
- Stage 0C and Stage 2 regressions;
- Stage 3 WP-01 through WP-06 regressions;
- deterministic replay;
- mutation tests;
- second-run reconstruction from the same evidence.

### Exit

```text
VPL-002 = PASS
VPL-003 = PASS
STAGE4_TECHNICALLY_COMPLETE
READY_FOR_STAGE4_DOCUMENTARY_RECONCILIATION
```

Stage 4 remains open until separate documentary review and Owner final acceptance.

## Future Allowlist Boundary

Each WP implementation authority must use the candidate boundaries in document 09 as its starting point and replace them with an exact path allowlist before execution.
