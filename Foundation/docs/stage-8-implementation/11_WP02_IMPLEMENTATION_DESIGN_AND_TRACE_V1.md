# Stage 8 WP-02 Implementation Design and Trace V1

**Work Package:** WP-02 — Guardian Protective Evaluation & Proportionate Intervention Decision Runtime  
**Status:** IMPLEMENTED / PRE-EXECUTABLE VALIDATION  
**Date:** 2026-08-14

## Runtime

WP-02 extends the existing `Foundation.Guardian` production project with `GuardianProtectiveEvaluationRuntime`.

The evaluation request binds:

- exact evaluation identity;
- target and governed scope;
- credible harm;
- uncertainty;
- reversibility;
- evidence independence;
- mandatory-intervention threshold state;
- trigger/evidence/authority/policy references;
- evaluation time.

The runtime produces a WP-01 `GuardianProtectiveDecision` intent. It does not execute that action.

## Deterministic proportional model

A deterministic protective score is derived from harm, uncertainty, reversibility and evidence independence. Higher credible harm, greater uncertainty, harder reversibility and weaker independent evidence monotonically increase protective pressure.

The selection surface is bounded to the approved AUT-002 protective vocabulary:

`NORMAL / HEIGHTENED / RESTRICTED / SAFE`

and:

`OBSERVE / WARN / RESTRICT / SUSPEND / ISOLATE / REQUEST_EMERGENCY_STOP`

`RECOVERY_GUARD` remains a valid WP-01 primitive but WP-02 does not claim Stage 9 recovery execution.

## Fail-closed rules

- a mandatory intervention threshold cannot resolve to Observe or Warn;
- `SubjectOnly` or `Unknown` evidence independence cannot support optimistic continuation and is elevated to at least bounded restriction;
- severe harm combined with high/unknown uncertainty favors protection;
- malformed required identity/evidence/authority/policy input fails closed;
- if a mandatory-threshold evaluation cannot produce a valid intervention decision, the outcome marks `ProtectionFailureObservable = true`.

## AUT-002 trace

WP-02 directly exercises:

- REQ-001 explicit authority/policy references;
- REQ-002 declared target/scope;
- REQ-003 proportionality to harm, uncertainty and reversibility;
- REQ-007 no exclusive reliance on subject-produced evidence for optimistic continuation;
- REQ-008 severe uncertainty favors protection;
- REQ-009 decision evidence fields inherited from WP-01;
- REQ-010 observable failure when mandatory intervention cannot be produced.

Later WPs own actual AUT-001 enforcement, Lifecycle execution, persistence, Safe State, independent emergency control and release/recovery boundaries.

## Non-authority boundary

`PROTECTIVE_EVALUATION != AUTHORITY_GRANT`

`PROTECTIVE_DECISION_INTENT != LIFECYCLE_TRANSITION`

`PROTECTIVE_DECISION_INTENT != SAFE_STATE_EXECUTION`

`PROTECTIVE_DECISION_INTENT != RECOVERY_OR_RELEASE`
