# Stage 9 Gate 0A — Complete Source and Capability Reconciliation

**Stage:** 9 — Controlled Recovery and Independent Release  
**Gate:** 0A — EXISTING_CAPABILITY_RECONCILIATION  
**Status:** COMPLETE_FOR_PLANNING / NO_PRODUCTION_IMPLEMENTATION_AUTHORITY  
**Date:** 2026-08-15  
**Branch:** `foundation-development`

## 1. Authority and purpose

Stage 8 is accepted and closed by explicit Project Owner decision. Stage 9 entry and planning/reconciliation are separately authorized by the Project Owner.

This Gate 0A document completes the required existing-capability reconciliation before any Stage 9 production implementation. It does not grant implementation authority.

Stage 9 purpose, per activated IMP-001 v1.3:

> complete governed restoration, reconciliation, independent recovery validation, controlled reintroduction and separate release authority.

Mandatory invariants:

- `REPAIR_SUCCESS != RELEASE`
- `RESTART != RECOVERY`
- `REPAIRED != TRUSTED`
- `TESTED != RELEASED`
- `READY_FOR_RECOVERY_EVALUATION != RELEASE`
- `GUARDIAN_RESTRICTION_PERSISTS_UNTIL_LAWFUL_RELEASE`
- `RECOVERY_VALIDATOR != REPAIR_ACTOR`
- `SUBJECT != RELEASE_AUTHORITY`
- `GUARDIAN != RELEASE_AUTHORITY`
- `LIFECYCLE_REMAINS_TRANSITION_OWNER`
- `AUT001_REMAINS_AUTHORITY_OWNER`

## 2. Current FCR census relevant to Stage 9

### FCR-0076

Current header:

- Stage 8 portion: accepted and closed;
- Stage 9 entry/planning: authorized;
- immediate Foundation action: Stage 9 reconciliation and implementation-plan preparation;
- residual Stage 9 scope: generic recovery, independent recovery validation, authorized release and controlled reintroduction;
- Shared Web remains request/presentation transport only;
- Stage 13 retains FSA-specific governance/recovery.

Disposition: `DIRECT_STAGE9_INPUT`.

### FCR-0082

Current header:

- Stage 8 containment/protective-continuity portion: accepted and closed;
- Stage 9 entry/planning: authorized;
- residual Stage 9 scope: generic restoration, independent recovery validation, release and reintroduction;
- subject, Guardian and repair actor may not self-certify release;
- Stage 8 restriction remains authoritative until lawful Stage 9 release;
- Stage 13 retains FSA-specific monitoring, investigation, Factory Reset, remediation sandbox and FSA Controlled Revival.

Disposition: `DIRECT_STAGE9_INPUT`.

### FCR-0012 and FCR-0030

These remain Stage 13 obligations. They are read in Stage 9 only as a hard non-leakage boundary.

Disposition: `BOUNDARY_ONLY / NO_STAGE9_FSA_SPECIFIC_IMPLEMENTATION`.

### Other open FCRs

No other current open FCR identified by the Stage 9 census assigns generic Foundation recovery/release/reintroduction implementation to Stage 9. Future Stage 11/12/13/14 obligations remain outside this Stage.

## 3. Governing sources and current effective state

### 3.1 OPS-003 Recovery — EFFECTIVE / PRIMARY

`docs/specifications/core/OPS-003_RECOVERY.md`

OPS-003 v1.0 is Approved and effective. It already governs:

- recovery initiation and authority;
- versioned recovery plans;
- containment prerequisites;
- restoration;
- dependency-aware sequencing;
- validation;
- rollback/abandonment;
- controlled reintroduction;
- recovery evidence.

Its required phases are:

1. Containment
2. Assessment
3. Plan Authorization
4. Restoration
5. Validation
6. Controlled Reintroduction
7. Closure

Key binding requirements include authenticated/authorized initiation, explicit triggering/containment truth, versioned plans, evidence preservation, lifecycle/dependency compliance, configuration/authority/security/data/dependency reconciliation, authoritative-state reconciliation, explicit partial recovery, independent validation, failed-validation denial, staged reintroduction, persistent Guardian restriction, bounded attempts and escalation on irrecoverable/uncertain state.

Disposition: `PRESERVE / IMPLEMENT_STAGE9_AGAINST_THIS_SPEC`.

### 3.2 VPL-007 Controlled Recovery — EFFECTIVE VERIFICATION BASIS

`docs/verification/VPL-007_CONTROLLED_RECOVERY.md`

VPL-007 already requires:

- unresolved restriction from VPL-006;
- approved versioned recovery plan;
- controlled repair action and rollback direction;
- authoritative state/evidence checkpoint;
- Independent Verifier separated from repair actor;
- declared release authority;
- valid and deliberately invalid recovery outcomes;
- reconciliation of configuration, identity, authority, security, durable state, dependencies and evidence integrity;
- independent validation;
- declared release-authority decision;
- Guardian release-condition satisfaction;
- controlled Lifecycle reintroduction;
- new attributable authority decision;
- heightened observation before normal status.

Negative variants include self-certified repair success, failed validation, uncertain reconciliation, missing/integrity-failed evidence, partial recovery, stale security context reuse, restart with unresolved trigger and exceeded bounded attempts.

Disposition: `PRESERVE / STAGE9_EXECUTABLE_ACCEPTANCE_MUST_SATISFY`.

### 3.3 AUT-001 Authority Engine — EFFECTIVE / AUTHORITY OWNER

`docs/specifications/core/AUT-001_AUTHORITY_ENGINE.md`

Relevant binding semantics:

- Authority Engine evaluates permission; it does not create authority.
- active Guardian/protective restrictions constrain authorization;
- restoration of material authority requires a new attributable decision supported by restoration evidence and independent confirmation;
- the authority under material review cannot be sole authority restoring itself;
- material uncertainty reduces authority/fails closed;
- identical trusted inputs must produce deterministic decisions.

Disposition: `PRESERVE / NO_SECOND_AUTHORITY_ENGINE`.

### 3.4 AUT-002 Guardian — EFFECTIVE / RESTRICTION OWNER

`docs/specifications/core/AUT-002_GUARDIAN.md`

Relevant binding semantics:

- Guardian owns bounded protection, restriction and release-condition evidence;
- Guardian does not own ordinary recovery execution;
- Guardian cannot declare recovery complete without required evidence;
- unresolved restrictions persist across restart;
- release requires authorized evidence that trigger is resolved/acceptably contained;
- Guardian itself remains independently governable and may not become its own unrestricted release path.

Disposition: `PRESERVE / STAGE9_CONSUMES_RESTRICTION_AND_RELEASE_CONDITIONS`.

### 3.5 SYS-002 Lifecycle — EFFECTIVE / TRANSITION OWNER

`docs/specifications/core/SYS-002_LIFECYCLE.md`

Relevant binding semantics:

- Lifecycle owns authoritative component state and transitions;
- transitions require AUT-001 authorization;
- recovery transitions coordinate with OPS-003;
- component cannot return to `RUNNING` before recovery validation;
- no self-declared completed recovery.

Disposition: `PRESERVE / NO_SECOND_LIFECYCLE_STATE_MACHINE`.

### 3.6 SYS-011 Persistence — EFFECTIVE / DURABLE TRUTH

`docs/specifications/core/SYS-011_PERSISTENCE.md`

Relevant binding semantics:

- one authoritative source per governed fact;
- uncertain writes/corruption must be exposed;
- backup is not recoverable until restoration is verified;
- restoration must preserve/reconcile authority, version and causality;
- uncertain integrity restricts affected use;
- provenance survives restoration.

Disposition: `PRESERVE / RECOVERY_MUST_CONSUME_DURABLE_TRUTH`.

### 3.7 AUT-003 Intervention, Revocation, and Recovery — NOT EFFECTIVE

Registry state:

- current effective version: `NONE`;
- current effective status: `NOT YET EFFECTIVE`;
- planned canonical path: `docs/specifications/autonomy/AUT-003_INTERVENTION_REVOCATION_AND_RECOVERY.md`.

Because Stage 9 contains recovery/release behavior, this planned Specification is materially adjacent. IMP-001 requires a Specification Definition Review Activation Gate before implementing missing behavior whose material planned Specification subject lacks an effective body.

Disposition: `GATE0B_REVIEW_REQUIRED`.

No Stage 9 code may silently treat the planned AUT-003 title as already-effective normative authority.

## 4. Existing runtime capability reconciliation

### 4.1 Stage 8 protective recovery handoff — EXISTS / STRONG

Path:

`src/Foundation.Authority/ProtectiveRecoveryHandoff.cs`

Existing surface provides:

- subject self-release denial;
- Guardian self-release denial;
- role separation inputs for subject, Guardian, repair actor, independent verifier and declared release authority;
- recovery evidence package;
- restriction identity/integrity binding;
- authoritative-state reconciliation evidence;
- security-context reestablishment evidence;
- dependency reconciliation evidence;
- independent recovery-validation evidence;
- Guardian-condition evidence;
- residual-risk evidence;
- deterministic evidence/handoff identity;
- `ReadyForRecoveryEvaluation` distinction;
- `ReleaseEligibleInProtectionContext = false`;
- restriction remains enforced;
- flags requiring independent validation, authorized release, Lifecycle reintroduction and new authority decision.

Disposition: `PRESERVE_AND_CONSUME`.

### 4.2 Foundation.Reconciliation — EXISTS / STRONG

Paths:

- `src/Foundation.Reconciliation/ReconciliationModels.cs`
- `src/Foundation.Reconciliation/ReconciliationClassifier.cs`
- `src/Foundation.Reconciliation/RestartReconciler.cs`

Existing `RestartReconciler`:

- reads authoritative durable state;
- challenges missing commit truth when request/decision identity was supplied;
- fails on conflicting/corrupt commit state;
- explicitly represents uncertainty before/after commit;
- reconstructs from trusted history when exact evidence permits;
- otherwise fails closed;
- persists reconciliation state;
- uses integrity-linked evidence and accepted facts.

Disposition: `PRESERVE / EXTEND_ONLY_FOR_PROVEN_STAGE9_GAPS`.

### 4.3 ApplicationLifecycle — EXISTS / MATERIAL

Path:

`src/Foundation.ApplicationLifecycle/ApplicationLifecycle.cs`

Existing surface already models:

- explicit lifecycle authority evidence;
- dependency/compatibility/security continuity;
- rollback evidence and target identity;
- fail-closed authority state;
- rollback/lifecycle eligibility.

Disposition: `PRESERVE_AND_BIND`.

### 4.4 Stage 8 restriction/Safe-State/emergency containment — EXISTS / PREDECESSOR

Stage 9 must not weaken or replace accepted Stage 8 protective runtime. Recovery evaluation occurs while restriction remains enforced.

Disposition: `PRESERVE_UNTIL_SEPARATE_AUTHORIZED_RELEASE`.

## 5. Capability classification

### EXISTS

- Guardian restriction persistence;
- Safe-State enforcement;
- independent emergency containment;
- recovery handoff package;
- no-self-release guard;
- role-separation identities;
- durable authoritative state;
- restart reconciliation;
- lifecycle transition/rollback eligibility;
- canonical Authority Engine;
- evidence/provenance primitives.

### PARTIAL

- generic Stage 9 recovery-case state/orchestration;
- explicit versioned recovery-plan model bound to OPS-003 requirements;
- bounded recovery-attempt/abort/abandonment state;
- explicit independent recovery-validation decision object/principal contract;
- explicit release request/decision/evidence contract bound to an existing restriction/handoff;
- residual-risk acceptance/denial semantics;
- release-condition satisfaction binding without Guardian self-release;
- controlled reintroduction binding into Lifecycle;
- new authority restoration decision binding through AUT-001;
- generic probation/heightened-observation semantics if required for non-FSA components;
- end-to-end deterministic Stage 9 evidence identity.

### MISSING NORMATIVE DECISION / GATE0B

- whether AUT-003 needs an effective body for Stage 9-specific intervention/recovery/release semantics;
- formal independent validation authority matrix noted unresolved in OPS-003;
- release-authority consequence-class mapping noted unresolved in AUT-002;
- recovery objective/attempt-limit policy by consequence class;
- exact generic post-release heightened-observation owner and exit condition.

## 6. Ownership model

Stage 9 shall not become a universal repair implementation owner.

The repair actor remains the owner/authorized actor for the actual corrective action appropriate to the affected component/domain.

Stage 9 Foundation-owned responsibility is the generic governed recovery framework:

- recovery case/plan governance;
- recovery initiation and attempt bounds;
- authoritative reconciliation coordination;
- independent validation result;
- separate release decision;
- binding to Guardian restriction/release conditions;
- Lifecycle reintroduction eligibility/transition handoff;
- new AUT-001 authority restoration decision requirement;
- deterministic recovery evidence and closure truth.

Application business repair/recovery semantics remain Application-owned.

FSA-specific investigation, Monitor AI, Factory Reset, remediation sandbox and FSA-specific Controlled Revival remain Stage 13.

## 7. Conflict check

No architectural conflict was found between OPS-003, VPL-007, AUT-001, AUT-002, SYS-002, SYS-011 and accepted Stage 8 recovery-handoff behavior.

The principal documentary gap is not a contradiction; it is an activation/definition gap around planned AUT-003 and unresolved authority matrices/policies.

This gap must be resolved by Gate 0B before Stage 9 code relies on new normative semantics.

## 8. Gate 0A verdict

`STAGE9_GATE0A_EXISTING_CAPABILITY_RECONCILIATION = PASS`

`DUPLICATE_AUTHORITY_ENGINE_REQUIRED = FALSE`

`DUPLICATE_GUARDIAN_REQUIRED = FALSE`

`DUPLICATE_LIFECYCLE_REQUIRED = FALSE`

`DUPLICATE_RECONCILIATION_ENGINE_REQUIRED = FALSE`

`STAGE8_RECOVERY_HANDOFF = PRESERVE_AND_CONSUME`

`OPS003 = EFFECTIVE_PRIMARY_RECOVERY_SPEC`

`AUT003 = NOT_YET_EFFECTIVE / GATE0B_REVIEW_REQUIRED`

`STAGE13_FSA_SPECIFIC_SCOPE = PRESERVED`

`STAGE9_PRODUCTION_IMPLEMENTATION = NOT_AUTHORIZED_BY_GATE0A`

## 9. Next action

Proceed to Gate 0B:

1. specification/contract/ADR gap review;
2. determine exact normative amendments/activations required before Stage 9 production implementation;
3. define recovery/release authority and independence model;
4. prepare exact WP-01 through WP-10 implementation plan;
5. Architecture/Consistency review;
6. pre-implementation Red Team;
7. Owner review of the implementation plan.
