# FSATS Specialized Implementation Architecture — Canonical State Machine Catalog

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`

## 1. Purpose

Define the state transitions that implementation must enforce. A transition not declared by the owning state machine is rejected unless a later versioned design explicitly adds it.

General rule:

```text
CURRENT_STATE + EVENT + PRECONDITIONS
-> NEXT_STATE + REQUIRED_SIDE_EFFECTS/EVIDENCE
```

No state may be inferred from a successful transport call alone.

## 2. Universal State-Machine Rules

1. State version is monotonic per aggregate.
2. Every accepted transition binds causation/evidence and actor/authority where applicable.
3. Duplicate identical command at the same idempotency identity is idempotent.
4. Conflicting duplicate fails closed.
5. Older state version/event cannot roll back a newer state except through an explicit correction/rollback transition.
6. `UNKNOWN`, `AMBIGUOUS`, `RECONCILIATION_REQUIRED` are first-class states where required; they never default to success.
7. Terminal state remains terminal except explicit governed correction/reopen transition where specified.
8. State history is append-only; current-state projection is derived.

## 3. Trade Proposal State Machine

States:

```text
PROPOSED
REJECTED_BY_APPLICABILITY
REJECTED_BY_UNCERTAINTY
REJECTED_BY_RISK
REJECTED_BY_CAPITAL
EXPIRED
SUPERSEDED
APPROVED_FOR_EXECUTION_INTENT
```

Transitions:

| Current | Event | Preconditions | Next |
|---|---|---|---|
| none | `ProposalCreated` | valid strategy evaluations, pinned evidence, non-expired decision cycle | PROPOSED |
| PROPOSED | `ApplicabilityFailureConfirmed` | hard applicability violation | REJECTED_BY_APPLICABILITY |
| PROPOSED | `UncertaintyGateFailed` | calibration/uncertainty policy fails | REJECTED_BY_UNCERTAINTY |
| PROPOSED | `RiskDenied` | exact RiskDecision DENY bound to proposal | REJECTED_BY_RISK |
| PROPOSED | `CapitalReservationFailedTerminal` | capital cannot be reserved under current valid proposal/risk context | REJECTED_BY_CAPITAL |
| PROPOSED | `ProposalExpired` | expiry reached before execution intent | EXPIRED |
| PROPOSED | `NewProposalSupersedes` | same decision key, newer valid proposal | SUPERSEDED |
| PROPOSED | `ExecutionIntentAuthorized` | ALLOW risk + valid HELD reservation + readiness/Guardian gates | APPROVED_FOR_EXECUTION_INTENT |

No transition out of terminal proposal states.

## 4. Risk Decision Lifecycle

RiskDecision is immutable after creation. It does not mutate through statuses. A new evaluation creates a new RiskDecisionId and supersedes the old decision for future action.

Decision outcomes:

```text
ALLOW
ALLOW_WITH_REDUCTION
DENY
REQUIRE_REVIEW
UNKNOWN
```

`ALLOW/ALLOW_WITH_REDUCTION` expires with the bound proposal/snapshot/policy validity. Execution requiring risk revalidation creates a new decision.

## 5. Capital Reservation State Machine

| Current | Event | Preconditions | Next |
|---|---|---|---|
| none | `ReservationRequested` | valid proposal/risk context | REQUESTED |
| REQUESTED | `FundsHeldAtomically` | enough unreserved capital; policy valid | HELD |
| REQUESTED | `ReservationRejected` | deterministic insufficiency/policy failure | INVALID |
| HELD | `FillConsumesPart` | reconciled fill amount < held remaining | PARTIALLY_CONSUMED |
| HELD | `FillConsumesAll` | reconciled total consumption >= held required amount subject to exact accounting | CONSUMED |
| PARTIALLY_CONSUMED | `AdditionalFillConsumesPart` | remaining > 0 | PARTIALLY_CONSUMED |
| PARTIALLY_CONSUMED | `FinalFillConsumes` | remaining reserved amount consumed | CONSUMED |
| HELD/PARTIALLY_CONSUMED | `ReleaseRequested` | terminal order outcome/unused amount identified | RELEASING |
| RELEASING | `ReleaseCommitted` | atomic ledger release persisted | RELEASED |
| HELD | `ReservationExpiredBeforeSubmission` | no active compatible execution intent; expiry rule satisfied | EXPIRED |
| any nonterminal | `AccountingConflictDetected` | authoritative reconciliation mismatch | RECONCILIATION_REQUIRED |
| RECONCILIATION_REQUIRED | `ReconciliationResolved` | exact corrected capital state/evidence | state derived from reconciled consumption/release outcome |

`EXPIRED` releases capacity through an atomic ledger operation; state/event evidence must prove the release. Expiry timestamp alone must not silently increase available capital before release is persisted.

## 6. Execution Intent / Order Chain State Machine

Canonical operational state:

```text
CREATED
VALIDATED
SUBMISSION_ELIGIBLE
SUBMITTING
SUBMITTED
PARTIALLY_FILLED
FILLED
CANCEL_REQUESTED
CANCELED
REPLACE_REQUESTED
REPLACED
REJECTED
EXPIRED
AMBIGUOUS
RECONCILIATION_REQUIRED
TERMINAL_FAILURE
```

Core transitions:

| Current | Event | Preconditions | Next |
|---|---|---|---|
| none | `IntentCreated` | proposal+risk+reservation exact binding | CREATED |
| CREATED | `IntentValidated` | market/account/broker/Guardian/price/qty gates valid | VALIDATED |
| VALIDATED | `SubmissionGatePassed` | persisted idempotency key + route/capability available | SUBMISSION_ELIGIBLE |
| SUBMISSION_ELIGIBLE | `BrokerDispatchStarted` | durable attempt record persisted | SUBMITTING |
| SUBMITTING | `BrokerAckAccepted` | accepted broker identity/order ref | SUBMITTED |
| SUBMITTING | `BrokerReject` | explicit terminal rejection | REJECTED |
| SUBMITTING | `DispatchOutcomeUnknown` | timeout/connection ambiguity | AMBIGUOUS |
| SUBMITTED | `PartialFillReconciled` | new valid fill delta | PARTIALLY_FILLED |
| PARTIALLY_FILLED | `PartialFillReconciled` | more fill, remaining > 0 | PARTIALLY_FILLED |
| SUBMITTED/PARTIALLY_FILLED | `FullFillReconciled` | total reconciled fill reaches intended terminal quantity under current chain | FILLED |
| SUBMITTED/PARTIALLY_FILLED | `CancelRequested` | valid cancel authority/scope | CANCEL_REQUESTED |
| CANCEL_REQUESTED | `CancelConfirmed` | broker evidence + no unresolved later fill conflict | CANCELED |
| CANCEL_REQUESTED | `FillArrives` | valid fill event | PARTIALLY_FILLED or FILLED; cancel remains historical attempt |
| SUBMITTED/PARTIALLY_FILLED | `ReplaceRequested` | broker/market capability + policy permits | REPLACE_REQUESTED |
| REPLACE_REQUESTED | `ReplacementConfirmed` | new broker order identity/attempt reconciled | REPLACED then child/current attempt becomes SUBMITTED-equivalent under same OrderChainId |
| any active | `BrokerStateConflict` | contradictory/out-of-order/unproven status | RECONCILIATION_REQUIRED |
| AMBIGUOUS | `BrokerQueryFindsAcceptedOrder` | identity/provenance exact | SUBMITTED/PARTIALLY_FILLED/FILLED based on reconciled evidence |
| AMBIGUOUS | `BrokerQueryProvesNoOrderAndSafeRetry` | broker semantics prove no prior acceptance; intent still valid | SUBMISSION_ELIGIBLE with new attempt sequence, same chain/idempotency policy |
| AMBIGUOUS | `CannotResolveBeforeSafetyDeadline` | unresolved | RECONCILIATION_REQUIRED |
| active | `BrokerExplicitExpiry` | compatible status | EXPIRED |
| active | `UnrecoverableExecutionFailure` | no safe retry/reconcile path | TERMINAL_FAILURE |

Blind retry from AMBIGUOUS directly to SUBMITTING is forbidden.

## 7. Position State Machine

Position current side/quantity is derived from reconciled fill ledger; side state:

```text
FLAT
LONG
SHORT
RECONCILIATION_REQUIRED
```

Transitions:

- FLAT + positive reconciled net fill -> LONG;
- FLAT + negative reconciled net fill -> SHORT only when market/account policy supports it; otherwise integrity failure;
- LONG + additional buy -> LONG increased;
- LONG + sell less than position -> LONG reduced;
- LONG + sell exactly position -> FLAT;
- LONG + sell beyond position -> SHORT only if explicitly supported/authorized, else RECONCILIATION_REQUIRED;
- symmetric rules for SHORT;
- contradictory broker/ledger evidence -> RECONCILIATION_REQUIRED;
- correction/bust creates a new ledger correction event and recalculates state; history is not rewritten.

## 8. Strategy Lifecycle State Machine

```text
EXPERIMENTAL
VALIDATION
WATCH
ACTIVE
RESTRICTED
DORMANT
RETIRED
```

Allowed transitions:

```text
EXPERIMENTAL -> VALIDATION
VALIDATION -> WATCH | EXPERIMENTAL | RETIRED
WATCH -> ACTIVE | VALIDATION | DORMANT | RETIRED
ACTIVE -> RESTRICTED | WATCH | DORMANT | RETIRED
RESTRICTED -> ACTIVE | WATCH | DORMANT | RETIRED
DORMANT -> WATCH | RETIRED
RETIRED -> no automatic reactivation; new successor version required
```

Promotion requires separate governed evidence/authority. Performance alone cannot self-transition state.

## 9. Provider Lifecycle State Machine

```text
DISCOVERED -> PROFILED -> ADAPTER_AVAILABLE -> VALIDATED -> ELIGIBLE -> ENABLED
```

Side transitions:

- any pre-retired -> INCOMPATIBLE on incompatible schema/API/profile;
- ELIGIBLE/ENABLED -> DISABLED by configuration/governance;
- ENABLED -> DEGRADED on provider/business health threshold;
- DEGRADED -> ENABLED only after recovery validation;
- any -> QUARANTINED on integrity/security/malformed-behavior policy;
- valid states -> RETIRED by governed decision;
- RETIRED has no implicit reactivation; successor/profile version required.

Provider ENABLED != Foundation egress authorized.

## 10. Provider Route Runtime State

```text
CANDIDATE
ELIGIBLE
SELECTED
ACTIVE
DEGRADED
ISOLATED
QUOTA_BLOCKED
UNAVAILABLE
RECOVERING
```

Key rules:

- hard-gate failure cannot produce ELIGIBLE;
- SELECTED must reference an exact eligible snapshot;
- Guardian isolate -> ISOLATED;
- quota cannot be reserved -> QUOTA_BLOCKED;
- active route failure -> DEGRADED/UNAVAILABLE then route controller reselects among other ELIGIBLE routes;
- recovery requires capability/health/quota revalidation;
- historical selected route remains evidence even after failover.

## 11. Data Observation Quality State

Quality state is derived, not manually transitioned. Given same product/profile/evidence, result must be deterministic:

```text
VALID
DEGRADED
CONFLICTED
STALE
INCOMPLETE
UNAVAILABLE
UNKNOWN
```

A correction creates a new observation state/identity with `CorrectionOf`/`Supersedes`, not an in-place transition that hides original quality.

## 12. Guardian Incident State Machine

| Current | Event | Next |
|---|---|---|
| none | protection signal observed | OBSERVED |
| OBSERVED | correlation/qualification begins | QUALIFYING |
| QUALIFYING | policy qualifies | QUALIFIED |
| QUALIFYING | insufficient evidence | EVIDENCE_INSUFFICIENT |
| QUALIFYING | proven false positive | FALSE_POSITIVE |
| QUALIFIED | directive/protection action active | PROTECTION_ACTIVE |
| QUALIFIED/PROTECTION_ACTIVE | broader incident supersedes | SUPERSEDED |
| PROTECTION_ACTIVE | threat contained but release not yet safe | CONTAINED |
| CONTAINED | recovery assessment starts | RECOVERY_ASSESSMENT |
| RECOVERY_ASSESSMENT | release criteria pass and directives reconciled | RESOLVED |
| RESOLVED | closure evidence complete | CLOSED |
| any nonterminal | severity increases/material new scope | ESCALATED then corresponding active qualification/protection state |

`EVIDENCE_INSUFFICIENT` may re-enter QUALIFYING on new evidence, preserving prior evidence state.

## 13. Guardian Directive State Machine

```text
CREATED
VALIDATED
PUBLISHED
TARGET_ACKNOWLEDGED
TARGET_APPLYING
EFFECT_CONFIRMED
EFFECT_PARTIAL
EFFECT_FAILED
SUPERSEDED
RELEASE_PENDING
RELEASED
EXPIRED_UNRELEASED
RECONCILIATION_REQUIRED
```

Important rules:

- PUBLISHED != EFFECT_CONFIRMED;
- expiry while underlying incident remains active -> EXPIRED_UNRELEASED and requires reconciliation/new directive, not automatic safe release;
- release is explicit and references the directive being released/superseded;
- conflicting target outcomes -> RECONCILIATION_REQUIRED/EFFECT_PARTIAL, never confirmed success.

## 14. Crisis Episode State Machine

```text
NONE -> ELEVATED -> SEVERE -> CRITICAL -> EMERGENCY
```

Transitions can move upward as qualifying evidence worsens. Downward movement requires policy evidence for the lower level and reconciled protection state. It may step down one or multiple levels if exact policy permits, but every transition is recorded.

Time elapsed alone cannot reduce crisis level.

## 15. Simulation Run State Machine

```text
DEFINED -> VALIDATED -> READY -> RUNNING
RUNNING -> PAUSED | CHECKPOINTED | COMPLETED | FAILED | CANCELED
PAUSED -> RUNNING | CANCELED
CHECKPOINTED -> RUNNING | CANCELED
COMPLETED -> EVIDENCE_FROZEN
FAILED/CANCELED -> terminal for that RunId
EVIDENCE_FROZEN -> terminal immutable evidence
```

A resumed checkpoint keeps RunId only if checkpoint semantics prove exact reproducibility continuity; otherwise restart creates a new RunId linked to the canceled/superseded run.

## 16. Experiment / Candidate State Machine

```text
IDEA
-> SCOPED
-> AUTHORIZED_FOR_ISOLATED_RESEARCH
-> CANDIDATE_BUILT
-> TESTING
-> EVIDENCE_COMPLETE
-> PARENT_REVIEW
-> MSA_REVIEW
-> PENDING_FSA_COMPATIBILITY_REVIEW
-> PENDING_OWNER_GOVERNANCE
-> ACCEPTED_FOR_SEPARATE_IMPLEMENTATION_AUTHORIZATION
```

Side/terminal states:

```text
REJECTED
SUPERSEDED
INVALID_EVIDENCE
```

Rules:

- actual origin determines whether PARENT_REVIEW exists (CSA->LSA, LSA->MSA, MSA directly MSA review);
- FSA interface stage remains fail-closed pending FCR-0030;
- `ACCEPTED_FOR_SEPARATE_IMPLEMENTATION_AUTHORIZATION` still does not mean implemented/deployed;
- Owner silence/timer expiry cannot transition candidate.

## 17. FSARM Resource Plan State Machine

```text
DRAFT
VALIDATED
ISSUED
APPLYING
PARTIALLY_EFFECTIVE
EFFECTIVE
FOUNDATION_REQUEST_PENDING
FOUNDATION_OUTCOME_RECONCILING
RESTORATION_PENDING
RESTORING
COMPLETED
SUPERSEDED
FAILED_CLOSED
RECONCILIATION_REQUIRED
```

Key transitions:

- DRAFT -> VALIDATED only against current ResourcePicture/Envelope/CoordinatorEpoch;
- VALIDATED -> ISSUED persists immutable ordered actions;
- ISSUED -> APPLYING when first target action dispatched;
- APPLYING -> PARTIALLY_EFFECTIVE as confirmed target effects arrive;
- APPLYING/PARTIALLY_EFFECTIVE -> EFFECTIVE only when required reclaim/reassign confirmations are complete;
- after internal actions, nonzero remaining deficit -> FOUNDATION_REQUEST_PENDING;
- Foundation outcome received -> FOUNDATION_OUTCOME_RECONCILING;
- valid new effective capacity may create a successor plan, never mutate the old plan's historical math;
- pressure decreases/current demand changes -> RESTORATION_PENDING -> RESTORING -> COMPLETED;
- stale envelope/epoch/coordinator fence breach -> FAILED_CLOSED or RECONCILIATION_REQUIRED.

## 18. FSARM Coordination Command State

Per action:

```text
CREATED
DELIVERED
ACKNOWLEDGED
TARGET_APPLYING
EFFECT_CONFIRMED
PARTIAL
REJECTED
FAILED
SUPERSEDED
EXPIRED
```

Capacity cannot be counted as reclaimed solely at DELIVERED/ACKNOWLEDGED unless the resource action profile explicitly defines acknowledgement itself as the authoritative effect boundary.

## 19. Resource Demand Report State

Reports are immutable snapshots, not mutable state machines. They are classified:

```text
CURRENT_VALID
STALE
SUPERSEDED
INVALID
CONFLICTED
```

At most one `CURRENT_VALID` report per `(ApplicationId, ResourceClass, ReportSeriesId)` for a pinned resource picture.

## 20. Awareness Integrity / Hold State (Application Side)

Application MSA/LSA/CSA local integrity state supports:

```text
NORMAL
INTEGRITY_CHECK
INVESTIGATION_HOLD
RESTRICTED
ISOLATED_CANDIDATE_ONLY
RECOVERY_VALIDATION
PROBATIONARY
```

Exact Foundation/FSA enforcement for system-level controls remains Foundation-owned. Application side cannot self-release from an externally imposed Foundation restriction.

Material own-Awareness anomaly transitions NORMAL -> INTEGRITY_CHECK. A material mismatch in goals/authority/permissions/core architecture -> INVESTIGATION_HOLD and freezes affected self-development authority. Release requires independent/governed evidence under file 18.

## 21. Application Removal Readiness State

Each Application maintains a business removal readiness projection:

```text
NOT_EVALUATED
BLOCKED_ACTIVE_OBLIGATION
BLOCKED_UNRECONCILED_STATE
READY_FOR_TECHNICAL_REMOVAL_REVIEW
```

This projection is input to Foundation lifecycle. It does not transition Foundation lifecycle itself.

## 22. State-Machine Verification Rules

A generated verifier SHALL:

1. enumerate every declared state and transition;
2. prove no undeclared transition is accepted by implementation;
3. generate invalid-transition negative cases;
4. test duplicate idempotency and conflicting duplicates;
5. test stale version/order rejection;
6. test UNKNOWN/AMBIGUOUS not converted to success;
7. test terminal-state protection;
8. test replay classifications cannot drive operational machines;
9. bind every state transition to exact evidence/causation;
10. rerun transition sequences deterministically from the same event stream;
11. compare reconstructed current state with persisted snapshot;
12. include correction/supersession tests where defined.
