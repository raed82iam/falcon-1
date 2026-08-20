# Stage 9 Implementation Plan v0.1 — PROPOSED

**Stage:** 9 — Controlled Recovery and Independent Release  
**Status:** PROPOSED_FOR_OWNER_REVIEW / NOT YET IMPLEMENTATION AUTHORITY  
**Date:** 2026-08-15  
**Branch:** `foundation-development`  
**Planning Authority:** Stage 9 Entry and Planning Authorization 2026-08-15  
**Implementation Authority:** NOT YET GRANTED

## 1. Stage objective

Implement the generic Foundation recovery framework required by OPS-003, CON-011 and VPL-007 so that a restricted/failed subject can be restored, reconciled, independently validated, lawfully released and reintroduced without allowing repair, restart, Guardian, the subject or technical success to self-create trust or authority.

Stage 9 does not own the actual domain-specific repair logic of every component. The authorized repair actor performs the corrective/restoration work appropriate to the owning component/domain. Stage 9 owns the generic governed recovery path around that work.

## 2. Governing sources

Primary effective sources:

- Falcon Vision;
- Falcon Constitution;
- IMP-001 v1.3;
- OPS-003 Recovery v1.0;
- AUT-001 Authority Engine v1.1;
- AUT-002 Guardian v1.0;
- SYS-002 Lifecycle v1.0;
- SYS-011 Persistence v1.0;
- SEC-001 Security;
- SEC-002 Foundation Trust Object Model;
- CON-002 Authority Decision;
- CON-003 Lifecycle;
- CON-008 Evidence and Logging;
- CON-009 Security Context;
- CON-011 Protective Restriction;
- VPL-006 Guardian Restriction;
- VPL-007 Controlled Recovery;
- Stage 8 final closure and Stage 8 WP-09/WP-10 recovery-handoff evidence;
- FCR-0076;
- FCR-0082.

Gate 0A and Gate 0B planning evidence:

- `00_STAGE9_ENTRY_FCR_CENSUS_AND_EXISTING_CAPABILITY_RECONCILIATION_V0.1.md`;
- `01_STAGE9_GATE0A_COMPLETE_SOURCE_AND_CAPABILITY_RECONCILIATION.md`;
- `02_STAGE9_GATE0B_SPECIFICATION_CONTRACT_AND_AUTHORITY_ACTIVATION_REVIEW.md`.

AUT-003 is NOT effective and is not an implementation authority or normative dependency for this plan.

## 3. Permanent Stage 9 invariants

- `REPAIR_SUCCESS != RELEASE`
- `RESTART != RECOVERY`
- `REPAIRED != TRUSTED`
- `TESTED != RELEASED`
- `READY_FOR_RECOVERY_EVALUATION != RELEASE`
- `PLAN_DEFINED != PLAN_AUTHORIZED`
- `PLAN_AUTHORIZED != RESTORATION_AUTHORIZED`
- `RECOVERY_VALIDATION_PASS != RELEASE_AUTHORIZATION`
- `RELEASE_AUTHORIZATION != LIFECYCLE_TRANSITION`
- `LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION`
- `SUBJECT_SELF_RELEASE = DENIED`
- `GUARDIAN_SELF_RELEASE = DENIED`
- `REPAIR_ACTOR_SELF_CERTIFICATION = DENIED`
- `UNKNOWN_RECOVERY_STATE = FAIL_CLOSED`
- `PARTIAL_RECOVERY != COMPLETE_RECOVERY`
- `STALE_SECURITY_CONTEXT != TRUSTED_SECURITY_CONTEXT`
- `STAGE8_RESTRICTION_PERSISTS_UNTIL_LAWFUL_RELEASE`
- `AUT001 = AUTHORITY_OWNER`
- `SYS002 = LIFECYCLE_TRANSITION_OWNER`
- `AUT002/CON011 = PROTECTIVE_RESTRICTION_AND_RELEASE_CONDITION_OWNER`
- `FOUNDATION_RECONCILIATION = AUTHORITATIVE_RECONCILIATION_SUBSTRATE`
- `APPLICATION_BUSINESS_REPAIR = APPLICATION_OWNED`
- `FSA_SPECIFIC_INVESTIGATION_FACTORY_RESET_CONTROLLED_REVIVAL = STAGE13`
- `FINAL_STAGE9_CROSS_STAGE_VALIDATION = FULL_ACCEPTED_STAGE0_THROUGH_STAGE9_CHAIN`

## 4. Actor separation model

Stage 9 shall preserve distinct identities/roles for:

- **Subject** — affected component/scope;
- **Guardian** — issuer/owner of protective restriction and release conditions;
- **Repair Actor** — authorized actor performing corrective/restoration work;
- **Recovery Coordinator** — owns recovery case/plan orchestration evidence only;
- **Independent Recovery Verifier** — validates independently from repair;
- **Declared Release Authority** — requests/approves release only through valid AUT-001 authority;
- **Lifecycle** — owns reintroduction transition;
- **Authority Engine** — owns authorization and restored authority decisions;
- **Evidence/Persistence** — own reconstructable durable facts.

No role name by itself creates authority. Exact actor authority must be evaluated through existing authority semantics.

## 5. Recovery state model

The Stage 9 recovery-case model shall distinguish at least:

1. `INITIATION_PENDING`
2. `AUTHORIZED_FOR_ASSESSMENT`
3. `PLAN_AUTHORIZATION_PENDING`
4. `PLAN_AUTHORIZED`
5. `RESTORATION_IN_PROGRESS`
6. `RESTORATION_REPORTED`
7. `RECONCILIATION_PENDING`
8. `VALIDATION_PENDING`
9. `VALIDATION_FAILED`
10. `READY_FOR_RELEASE_DECISION`
11. `RELEASE_DENIED`
12. `RELEASE_AUTHORIZED`
13. `REINTRODUCTION_PENDING`
14. `RECOVERY_GUARD_OBSERVATION`
15. `RECOVERED_WITH_RESTRICTED_AUTHORITY`
16. `RECOVERY_COMPLETE`
17. `ABORTED`
18. `ESCALATED`

These are Stage 9 recovery-case states, not replacements for SYS-002 canonical Lifecycle states.

## 6. Work Package sequence

### WP-01 — Recovery Case and Versioned Recovery Plan Primitives

**Goal:** create the canonical Stage 9 recovery-case/plan identity model without executing repair or release.

Implement Foundation-owned records for:

- RecoveryCase identity;
- exact subject/restriction/handoff binding;
- trigger and containment truth;
- recovery-plan ID/version;
- plan owner/coordinator;
- authorized repair actor identity;
- independent verifier identity;
- declared release authority identity/role;
- prerequisites;
- restoration sequence description/reference;
- validation criteria identities;
- abort conditions;
- rollback direction;
- explicit maximum authorized attempt count for that plan;
- residual-risk declaration requirements;
- deterministic plan/case identity.

Rules:

- missing plan bounds fail closed;
- no global numeric RTO/RPO is invented;
- plan existence does not authorize execution;
- repair actor may not also be independent verifier or release authority.

**Verifier focus:** invalid identities, missing bounds, role collision, mutation-sensitive identity, same-input determinism.

---

### WP-02 — Authorized Recovery Initiation, Plan Authorization and Attempt/Abort Governance

**Goal:** ensure recovery begins only under exact authority, the exact versioned plan is separately authorized, and attempts cannot loop destructively.

Implement:

- recovery initiation request;
- AUT-001-compatible recovery-initiation authority decision binding;
- separate `RecoveryPlanAuthorizationRequest` bound to the exact RecoveryCase and exact plan ID/version/digest;
- separate `RecoveryPlanAuthorizationDecision` with attributable actor, authority basis, scope, conditions and deterministic identity;
- denial if the plan changes after authorization unless a newly versioned plan receives a new authorization decision;
- restriction/handoff identity validation;
- current recovery attempt number;
- bounded attempt budget from the authorized plan;
- explicit authorization of the restoration attempt under the authorized plan;
- abort/escalation result;
- denial on missing/uncertain/revoked authority;
- denial when Stage 8 restriction/handoff is invalid or mismatched;
- no silent retry after attempt budget exhaustion.

Rules:

- recovery initiation authority != plan authorization by implication;
- plan definition != plan authorization;
- plan authorization != release authority;
- plan mutation invalidates the prior plan authorization for the changed plan;
- recovery initiation authority != release authority;
- time/timeout alone cannot authorize a retry or release;
- repeated failure must terminate in `ABORTED` or `ESCALATED` according to plan.

**Verifier focus:** unauthorized initiation, unauthorized/mutated plan, stale authority, mismatched restriction, exceeded attempt budget, deterministic denial.

---

### WP-03 — Restoration Outcome and Repair Evidence Boundary

**Goal:** receive truth about corrective/restoration work without letting the repair actor certify recovery.

Implement:

- restoration action identity/reference;
- exact authorized recovery-plan identity and plan-authorization identity;
- repair actor identity;
- requested/attempted/completed/failed/partial outcome distinction;
- changed artifact/config/state/dependency references as evidence identities;
- rollback action/result evidence when applicable;
- data-loss/capability-loss declaration;
- evidence preservation marker;
- restoration outcome digest.

Rules:

- Stage 9 does not own component-specific repair implementation;
- restoration may execute only against an authorized plan/attempt;
- `RESTORATION_REPORTED` is informational and cannot clear restriction;
- repair actor cannot write independent validation result;
- partial restoration remains explicit.

**Verifier focus:** self-certification attempts, fabricated complete from partial, missing incident evidence, unauthorized/mutated plan use, rollback mismatch.

---

### WP-04 — Authoritative Recovery Reconciliation Composite

**Goal:** build one Stage 9 recovery reconciliation result by consuming existing authoritative subsystems, not replacing them.

Consume/verify as applicable:

- `Foundation.Reconciliation` authoritative/durable state result;
- configuration identity/evidence;
- current authority/delegation state;
- security context reestablishment result;
- durable-state/data integrity result;
- dependency reconciliation result;
- restriction integrity/current-state result;
- evidence/provenance integrity result.

Output:

- `COMPLETE`, `PARTIAL`, `FAILED`, or `UNCERTAIN` recovery-reconciliation classification;
- exact evidence identities for every required dimension;
- deterministic composite identity.

Rules:

- any required `UNCERTAIN` dimension prevents unrestricted recovery;
- old compromised security context cannot be reused as proof of restoration;
- recovery coordinator cannot convert unknown into pass.

**Verifier focus:** corrupted state, uncertain commit, stale security context, dependency mismatch, evidence mutation, partial recovery truth.

---

### WP-05 — Independent Recovery Validation Decision

**Goal:** create the independent validation decision required by OPS-003/VPL-007.

Implement:

- validation request bound to exact RecoveryCase, authorized plan, restoration outcome and reconciliation result;
- exact Independent Verifier identity;
- verifier authority/independence evidence;
- approved validation criteria identities;
- pass/fail/partial/indeterminate result;
- immutable attributable validation evidence;
- deterministic decision identity.

Rules:

- verifier cannot be subject, Guardian or repair actor;
- verifier authority must be valid for the exact validation action;
- failed/partial/indeterminate validation prevents release readiness;
- evidence derived solely from subject/repair self-attestation is insufficient.

**Verifier focus:** role collisions, invalid verifier authority, failed criteria, stale/mutated evidence, deterministic output.

---

### WP-06 — Recovery Readiness, Guardian Condition and Residual-Risk Evaluation

**Goal:** establish `READY_FOR_RELEASE_DECISION` without granting release.

Consume:

- valid Stage 8 `RecoveryHandoffRecord`;
- exact authorized recovery plan;
- WP-04 complete reconciliation;
- WP-05 independent validation PASS;
- current Guardian restriction and release conditions;
- current security/dependency state;
- residual-risk evidence;
- no newer stricter controlling restriction.

Output:

- release-readiness result only;
- exact unsatisfied condition reasons;
- residual-risk record;
- current controlling restriction identity.

Rules:

- Guardian conditions are checked, not self-released by Guardian;
- readiness != release;
- residual risk is evidence presented to the competent release authority, not self-accepted by Recovery;
- residual risk outside authorized bounds fails closed;
- newer/stricter restriction supersedes stale readiness.

**Verifier focus:** stale handoff, new restriction, residual risk missing, Guardian self-release path, readiness/release confusion.

---

### WP-07 — Separate Release Authorization Decision

**Goal:** obtain a competent release decision distinct from recovery validation.

Implement:

- release request bound to exact restriction, RecoveryCase and WP-06 readiness identity;
- exact declared release authority identity/role from CON-011 restriction/handoff;
- AUT-001 authority evaluation for the exact release action/resource/jurisdiction/consequence;
- explicit residual-risk evidence consumed by the release decision;
- allowed/denied release authorization result;
- immutable reason/basis/policy/conditions evidence;
- deterministic release-decision identity.

Rules:

- subject, Guardian, repair actor and Independent Verifier cannot silently become release authority merely by participation;
- role label alone is insufficient;
- missing/ambiguous/conflicted release authority denies;
- release authorization cannot itself move Lifecycle or restore operational authority.

**Verifier focus:** self-release, Guardian release, repair release, invalid authority chain, role-name spoofing, stale readiness, deterministic denial/pass.

---

### WP-08 — Immutable Restriction Release Fact and Enforcement Transition

**Goal:** translate a valid WP-07 release authorization into an attributable release fact linked to the immutable Stage 8 restriction without rewriting historical restriction evidence.

Implement:

- restriction-release record linked to original restriction ID/integrity evidence;
- release decision identity;
- independent validation identity;
- release-condition satisfaction identity;
- release effective boundary;
- affected enforcement-point acknowledgement/result evidence where required;
- failure/partial release result;
- no deletion/rewriting of original restriction.

Rules:

- original restriction remains immutable history;
- release fact is an execution/evidence result of the valid release authority decision, not a second authority decision;
- release fact is invalid without exact binding to current controlling restriction;
- partial enforcement acknowledgement remains partial and cannot be called complete release;
- new stricter restriction blocks or supersedes permissive release.

**Verifier focus:** stale restriction release, mutation of original restriction, missing enforcement result, partial-as-complete, stricter restriction race.

---

### WP-09 — Controlled Lifecycle Reintroduction, New Authority Decision and Recovery-Guard Observation

**Goal:** return a released subject to permitted operation only through existing Lifecycle and AUT-001 owners.

Implement/bind:

1. controlled Lifecycle reintroduction request using SYS-002/CON-003 semantics;
2. transition only after valid release fact;
3. required identity/config/dependency/security rechecks for reintroduction;
4. new AUT-001 authority decision after/for the allowed operational state;
5. generic `RECOVERY_GUARD` / `HEIGHTENED` observation evidence where required by plan/consequence;
6. exit from observation only on governed evidence;
7. recovery completion/closure record with residual risk, loss, approvals and follow-up obligations.

Rules:

- release != Lifecycle transition;
- Lifecycle transition != unrestricted authority;
- new authority decision is mandatory where material authority was restricted/revoked;
- observation does not create FSA-specific Controlled Revival semantics;
- failed reintroduction or observation may re-restrict/escalate but cannot fabricate recovery.

**Verifier focus:** bypass Lifecycle, reuse old authority, direct `RUNNING` transition without validation, observation bypass, FSA-specific leakage.

---

### WP-10 — Integrated Stage 9 Closure Verification and Full Cross-Stage Recovery Hardening

**Goal:** prove Stage 9 as one coherent chain while preserving every accepted predecessor Stage boundary.

WP-10 shall execute and evidence a fresh **full accepted Stage 0 through Stage 9 cross-stage chain**, not merely the most recent predecessor-stage verifier.

The final validation runner/verifier set shall cover, to the extent each accepted Stage exposes an executable verifier or accepted aggregate verifier:

- Stage 0A/0B/0C accepted validation chain;
- Stage 1 accepted validation;
- Stage 2 accepted validation;
- Stage 3 accepted validation;
- Stage 4 accepted validation;
- Stage 5 accepted final/integrated validation;
- Stage 6 accepted final cross-stage validation;
- Stage 7 accepted final cross-stage validation;
- Stage 8 WP-01 through WP-10 and accepted final technical/integration evidence;
- Stage 9 WP-01 through WP-09 deterministic regressions;
- Stage 9 integrated verifier;
- VPL-007 full positive path;
- every VPL-007 mandatory negative variant;
- AUT-001 restoration/new-authority behavior;
- AUT-002/CON-011 restriction persistence and release semantics;
- SYS-002 controlled recovery/reintroduction behavior;
- SYS-011 restoration/reconciliation truth;
- Application-neutral zero-Application operation;
- no Stage 13 FSA-specific implementation;
- Architecture gate;
- Security gate;
- deterministic and mutation-sensitive integrated evidence;
- final worktree/candidate integrity.

Where an older accepted Stage has no single current standalone verifier, WP-10 shall use the accepted canonical executable evidence path/aggregate verifier that truthfully represents that Stage rather than silently omitting it. Any missing predecessor executable path discovered during planning/implementation shall be documented and resolved before Stage 9 closure-readiness is claimed.

WP-10 technical PASS does not itself close Stage 9. Final Stage 9 closure shall require:

1. WP-10 PASS;
2. a fresh full accepted Stage 0 through Stage 9 cross-stage executable validation result;
3. post-executable Stage 9 Red Team;
4. closure-readiness evidence;
5. one explicit Owner Stage 9 closure decision.

## 7. Verification strategy

Every WP shall have an independent executable verifier.

Each technical checkpoint shall include as applicable:

- exact candidate commit;
- clean Release build;
- Architecture validation;
- Security validation;
- accepted predecessor regressions appropriate to the WP;
- exact WP verifier markers/check counts;
- deterministic rerun;
- mutation-sensitive evidence tests;
- exact final HEAD;
- clean worktree.

For final Stage 9 closure-readiness, predecessor validation is not selective: the governed final runner shall execute the complete accepted Stage 0 through Stage 9 validation chain described by WP-10 and report any unavailable predecessor path explicitly rather than treating absence as PASS.

Failure rule:

- stop at the first material failure;
- classify environment/runner/verifier/production-semantic cause;
- do not weaken Architecture/Security to obtain PASS;
- remediate only the actual cause;
- rerun the full required checkpoint.

## 8. Mandatory negative scenarios across the Stage

At minimum:

1. subject self-release;
2. Guardian self-release;
3. repair actor self-certification;
4. Independent Verifier role collision;
5. release-authority spoofing;
6. missing/broken authority chain;
7. plan defined but not authorized;
8. plan mutated after authorization;
9. restart while trigger remains unresolved;
10. repair succeeds but reconciliation uncertain;
11. validation fails;
12. recovery remains partial;
13. stale/compromised security context reused;
14. dependency reconciliation mismatch;
15. evidence mutation after validation;
16. stale recovery handoff;
17. newer stricter restriction appears before release;
18. attempt budget exhausted;
19. rollback fails;
20. release authorized but Lifecycle reintroduction fails;
21. Lifecycle transition succeeds but new authority is denied;
22. observation/Recovery Guard fails;
23. Application business semantics leak into Foundation;
24. FSA Factory Reset/Monitor/Controlled Revival leaks into Stage 9;
25. timer/expiry treated as release;
26. partial enforcement release called complete;
27. nondeterministic recovery evidence;
28. final runner skips an accepted predecessor Stage and still attempts to claim cross-stage PASS.

## 9. Cross-stage boundaries

### Stage 8

Stage 8 remains accepted and closed. Stage 9 consumes its restriction and recovery handoff. It does not reopen or weaken Stage 8.

### Stage 10

Stage 9 completion does not grant Stage 10 implementation authority.

### Stage 13

Stage 13 remains sole future owner of FSA-specific:

- Monitor AI;
- FSA integrity investigation;
- FSA Factory Reset;
- FSA remediation sandbox;
- Owner/FSA governance plane;
- FSA-specific Controlled Revival/probation logic beyond generic Stage 9 primitives.

### Applications and Shared Web

Foundation Stage 9 remains Application-neutral.

Applications own domain/business repair and business-safe degraded/recovery behavior.

Shared Web may present/request recovery-related actions only through future governed interfaces; UI interaction never becomes recovery/release authority.

## 10. Implementation cadence after Owner acceptance

If the Project Owner accepts this plan for implementation:

- WP-01 begins immediately;
- no per-WP Owner approval stop is required unless the Owner explicitly adds one;
- each WP must pass exact executable validation before continuation;
- a failed checkpoint is remediated before proceeding;
- successful WP technical validation proceeds to the next WP;
- Stage 9 final Owner closure occurs only once after WP-10, the fresh full Stage 0 through Stage 9 cross-stage executable validation, and the post-executable Red Team.

This cadence is proposed and becomes binding only if the Owner accepts this implementation plan.

## 11. Current state

`STAGE9_GATE0A = PASS`

`STAGE9_GATE0B = PASS`

`STAGE9_IMPLEMENTATION_PLAN_V0_1 = PROPOSED`

`STAGE9_FINAL_CROSS_STAGE_REQUIREMENT = FULL_ACCEPTED_STAGE0_THROUGH_STAGE9_CHAIN`

`STAGE9_PRODUCTION_IMPLEMENTATION = NOT_YET_AUTHORIZED`

`NEXT = PRE_IMPLEMENTATION_ARCHITECTURE_CONSISTENCY_AND_RED_TEAM_REVIEW`
