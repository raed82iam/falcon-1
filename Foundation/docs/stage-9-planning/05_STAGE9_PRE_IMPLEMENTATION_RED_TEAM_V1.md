# Stage 9 Pre-Implementation Red Team v1

**Stage:** 9 — Controlled Recovery and Independent Release  
**Status:** PASS_WITH_MANDATORY_TIGHTENINGS / NO_IMPLEMENTATION_AUTHORITY  
**Date:** 2026-08-15  
**Reviewed Package:** Gate 0A, Gate 0B, proposed Stage 9 Implementation Plan v0.1, Pre-Implementation Architecture and Consistency Review

## 1. Objective

Attack the proposed Stage 9 recovery/release design before implementation and determine whether an affected subject, Guardian, repair actor, verifier, recovery coordinator, release actor, stale evidence, replay, timing, plan churn, cross-stage omission or FSA-specific leakage can produce false recovery, unlawful release or restored authority.

This review does not grant implementation authority.

## 2. Attack model

The Red Team assumes one or more of the following may be faulty, compromised, stale, dishonest or merely wrong:

- recovered subject;
- repair actor;
- Guardian input;
- recovery coordinator;
- independent-verifier request/evidence source;
- release requester;
- stored recovery evidence;
- security context;
- dependency state;
- recovery plan or plan version;
- lifecycle request;
- cross-stage validation runner.

The design must remain fail-closed without trusting role labels or technical success.

## 3. Attacks and disposition

### RT-01 — Subject self-release

Attack: recovered subject submits/causes its own release.

Result: BLOCKED by Stage 8 `ProtectiveReleaseGuard`, CON-011 and proposed Stage 9 WP-07.

Verdict: PASS.

### RT-02 — Guardian self-release

Attack: Guardian that issued/owns the restriction also acts as release authority.

Result: BLOCKED. Guardian owns protective restriction/release conditions but not ordinary recovery execution or self-release.

Verdict: PASS.

### RT-03 — Repair actor self-certification

Attack: repair actor reports success and converts that into recovery validation or release.

Result: BLOCKED by WP-03/WP-05/WP-07 separation.

Verdict: PASS.

### RT-04 — Independent Verifier role collision

Attack: subject, Guardian or repair actor is also named Independent Verifier.

Result: BLOCKED by Stage 8 handoff role separation and Stage 9 WP-05.

Verdict: PASS.

### RT-05 — Independent Verifier also becomes Release Authority

Attack: a technically independent verifier validates recovery and immediately approves release using the same identity.

Finding: The original proposed plan modeled these as separate roles but did not initially require distinct identities.

Architecture tightening `ACR-9-001` closes this:

`INDEPENDENT_RECOVERY_VERIFIER_IDENTITY != DECLARED_RELEASE_AUTHORITY_IDENTITY`

Verdict: PASS AFTER MANDATORY TIGHTENING.

### RT-06 — Role-name spoofing

Attack: an actor calls itself `IndependentVerifier` or `ReleaseAuthority` without valid authority.

Result: BLOCKED because role labels do not grant authority; exact authority must be evaluated through AUT-001.

Verdict: PASS.

### RT-07 — Broken/stale authority chain

Attack: valid recovery evidence is paired with missing, revoked, stale, conflicted or unverifiable authority.

Result: AUT-001 fail-closed semantics deny the governed action.

Verdict: PASS.

### RT-08 — Plan defined but never authorized

Attack: recovery proceeds because a plan object exists.

Result: BLOCKED after plan correction. WP-02 requires separate exact plan authorization.

Verdict: PASS.

### RT-09 — Plan mutation after authorization

Attack: authorize benign plan v1, mutate sequence/criteria/actors/bounds, then execute using original approval.

Result: BLOCKED. Plan authorization binds exact plan ID/version/digest; mutation requires a newly versioned plan and new authorization.

Verdict: PASS.

### RT-10 — Plan authorization replay across another RecoveryCase/restriction

Attack: reuse authorization for another subject/restriction/case.

Required implementation rule:

`PLAN_AUTHORIZATION = BOUND_TO_EXACT_RECOVERY_CASE + SUBJECT + RESTRICTION + PLAN_ID + VERSION + DIGEST`

Cross-case reuse must fail closed.

Verdict: PASS AS BINDING IMPLEMENTATION REQUIREMENT.

### RT-11 — Attempt-budget reset by plan-version churn

Attack: after reaching max recovery attempts, create plan v2/v3/v4 to reset the plan-local attempt counter indefinitely.

Finding: this would violate OPS-003 bounded-attempt semantics while appearing individually valid per plan.

Mandatory tightening `RT9-001`:

- `RECOVERY_ATTEMPT_BUDGET_CANNOT_RESET_BY_PLAN_VERSION_CHANGE`
- cumulative attempts SHALL be tracked at RecoveryCase level across plan supersession;
- prior attempts remain part of authoritative recovery evidence;
- an increase/reset of the cumulative authorized attempt ceiling requires a separate competent AUT-001-authorized decision with explicit reason and consequence scope;
- timer expiry, restart, plan rename or ordinary version increment cannot reset attempts.

Verdict: PASS AFTER MANDATORY TIGHTENING.

### RT-12 — Restart while trigger unresolved

Attack: subject restarts cleanly and requests recovery completion.

Result: BLOCKED by OPS-003, VPL-007, CON-011 and Stage 8 restriction persistence.

Verdict: PASS.

### RT-13 — Stale or mismatched Stage 8 recovery handoff

Attack: use a handoff for an older/different restriction.

Result: WP-02/WP-06 require exact subject/restriction/integrity binding.

Verdict: PASS.

### RT-14 — New stricter restriction after readiness

Attack: WP-06 says ready, then a new stronger Guardian restriction appears before release authorization/release execution.

Finding: readiness must not become a timeless capability token.

Mandatory tightening `RT9-002`:

`RELEASE_AUTHORIZATION_AND_RELEASE_EXECUTION_MUST_REVALIDATE_CURRENT_CONTROLLING_RESTRICTION_AND_MATERIAL_TRUST_SNAPSHOT`

At WP-07 and WP-08, implementation must re-check that:

- the controlling restriction identity/integrity is still current;
- no newer/stronger restriction controls the subject/action;
- material security/dependency/reconciliation evidence has not been superseded or invalidated;
- the readiness/evidence snapshot remains within its governed freshness/validity bounds.

Any material change invalidates stale readiness/release authorization for execution and forces re-evaluation.

Verdict: PASS AFTER MANDATORY TIGHTENING.

### RT-15 — TOCTOU after release authorization

Attack: obtain WP-07 release authorization, then alter security/dependency/restriction state before WP-08 execution.

Result: covered by `RT9-002`; WP-08 cannot blindly execute a previously valid decision against materially changed current state.

Verdict: PASS AFTER MANDATORY TIGHTENING.

### RT-16 — Stale/compromised security context reuse

Attack: restore functionality while reusing security material implicated in compromise.

Result: Stage 9 requires security-context reestablishment/current validation; stale compromised context cannot prove trust.

Verdict: PASS.

### RT-17 — Evidence mutation after independent validation

Attack: validate package A, mutate restoration/reconciliation evidence, then reuse validation result.

Required: WP-05/WP-06/WP-07 bind exact immutable evidence identities/digests; mutation changes identity and invalidates downstream binding.

Verdict: PASS.

### RT-18 — Subject/repair actor supplies all evidence to Independent Verifier

Attack: verifier is nominally independent but only consumes unchallenged self-attestation.

Result: VPL-007 requires independently obtained or integrity-verified evidence and forbids sole reliance on subject/repair evidence.

Verdict: PASS.

### RT-19 — Partial recovery represented as complete

Attack: one required dimension remains partial/unknown but final result says recovered.

Result: OPS-003 + WP-04/WP-05 fail closed; partial remains explicit.

Verdict: PASS.

### RT-20 — Failed/indeterminate validation converted to readiness

Attack: non-PASS validation still progresses.

Result: WP-05/WP-06 block.

Verdict: PASS.

### RT-21 — Rollback failure hidden as recovery

Attack: rollback fails or target mismatches but recovery continues.

Result: rollback failure must remain explicit and leads to restriction/abort/escalation according to plan.

Verdict: PASS.

### RT-22 — Release authorization replay against another subject/restriction

Attack: reuse valid release decision on another target.

Required: release decision identity binds exact RecoveryCase, subject, restriction, readiness and authority context. Cross-target replay fails.

Verdict: PASS AS BINDING IMPLEMENTATION REQUIREMENT.

### RT-23 — Partial enforcement release called complete

Attack: one enforcement point still restricts or cannot confirm state but release fact says complete.

Result: WP-08 requires explicit partial/failure result; partial acknowledgement cannot be complete release.

Verdict: PASS.

### RT-24 — Rewrite/delete original restriction after release

Attack: erase the Stage 8 incident/restriction record to make state look clean.

Result: WP-08 release is a linked immutable fact; original restriction/history remains preserved.

Verdict: PASS.

### RT-25 — Lifecycle bypass

Attack: release authorization directly returns subject to `RUNNING` without Lifecycle.

Result: prohibited. SYS-002 remains transition owner; WP-09 consumes it.

Verdict: PASS.

### RT-26 — Reuse old pre-restriction authority after release

Attack: after recovery, reuse old material operational authority rather than obtaining a new decision.

Result: AUT-001 and CON-011 require new attributable authority decision where material authority was restricted/revoked.

Verdict: PASS.

### RT-27 — Lifecycle transition succeeds but new authority denied

Attack: component is technically transitioned but authority restoration fails.

Result: Lifecycle state cannot be treated as authority. Denied authority remains denied/restricted; Stage 9 must not call this unrestricted recovery complete.

Verdict: PASS.

### RT-28 — Recovery-Guard/heightened observation bypass

Attack: consequence requires staged observation but component jumps straight to normal.

Result: WP-09 plan/consequence requirements must be binding; observation exit requires governed evidence.

Verdict: PASS.

### RT-29 — Recovery Coordinator becomes universal authority

Attack: coordinator uses orchestration ownership to authorize repair/release/lifecycle/authority restoration.

Result: prohibited by ownership model. Coordinator owns case/plan orchestration evidence only.

Verdict: PASS.

### RT-30 — Application business semantics leak into Foundation

Attack: Stage 9 begins deciding whether a trading/business component is commercially or strategically fit to resume.

Result: prohibited. Stage 9 owns generic technical/governed recovery only; Application domain/business recovery remains Application-owned.

Verdict: PASS.

### RT-31 — Web click/request becomes recovery or release authority

Attack: Owner/Web UI click is treated as technical authorization itself.

Result: FCR-0076 and plan preserve `UI_CLICK != AUTHORIZATION`; exact AUT-001 authority evaluation is required.

Verdict: PASS.

### RT-32 — Stage 13 FSA-specific semantics pulled into Stage 9

Attack: implement FSA Monitor AI, integrity investigation, Factory Reset, remediation sandbox or FSA Controlled Revival under generic recovery.

Result: explicitly prohibited; FCR-0012/FCR-0030 remain Stage 13.

Verdict: PASS.

### RT-33 — AUT-003 treated as active authority

Attack: code cites planned AUT-003 as if approved/effective.

Result: Gate 0B explicitly forbids this. Initial Stage 9 is governed by existing effective sources.

Verdict: PASS.

### RT-34 — Cross-stage runner skips an accepted predecessor but reports PASS

Attack: Stage 9 final runner validates only recent stages and labels result full cross-stage.

Result: corrected plan requires fresh full accepted Stage 0-through-Stage 9 validation chain; unavailable predecessor path must be explicit and blocks truthful full-chain PASS.

Verdict: PASS.

### RT-35 — Deterministic evidence is mutation-insensitive

Attack: evidence identity stays unchanged after material plan/evidence/restriction/authority mutation.

Result: every Stage 9 identity must be deterministic for same trusted inputs and mutation-sensitive for every material governed field.

Verdict: PASS AS MANDATORY VERIFIER REQUIREMENT.

## 4. Mandatory tightenings produced by review package

### ACR-9-001

`INDEPENDENT_RECOVERY_VERIFIER_IDENTITY != DECLARED_RELEASE_AUTHORITY_IDENTITY`

### RT9-001

`RECOVERY_ATTEMPT_BUDGET_CANNOT_RESET_BY_PLAN_VERSION_CHANGE`

Cumulative attempts persist at RecoveryCase scope. Expansion/reset needs separate competent authority and full evidence.

### RT9-002

`RELEASE_AUTHORIZATION_AND_RELEASE_EXECUTION_MUST_REVALIDATE_CURRENT_CONTROLLING_RESTRICTION_AND_MATERIAL_TRUST_SNAPSHOT`

Readiness/release decisions cannot be replayed across a material change in restriction, security, dependency, reconciliation or other controlling trust state.

These tightenings are part of the Stage 9 plan package and are mandatory for implementation and executable verification if the plan is accepted.

## 5. Severity result

- Critical blockers: `0`
- High blockers after mandatory tightenings: `0`
- Medium blockers after mandatory tightenings: `0`
- Product/runtime Low blockers: `0`
- Documentary planning tightenings: `3` (`ACR-9-001`, `RT9-001`, `RT9-002`)

No production code exists under this Stage 9 plan yet, so this Red Team evaluates design/authority completeness rather than runtime correctness.

## 6. Final verdict

`STAGE9_PRE_IMPLEMENTATION_RED_TEAM_V1 = PASS_WITH_MANDATORY_TIGHTENINGS`

`ACR9_001 = REQUIRED`

`RT9_001 = REQUIRED`

`RT9_002 = REQUIRED`

`SELF_RELEASE_PATH = DENIED_BY_DESIGN`

`PLAN_REPLAY_OR_MUTATION_BYPASS = DENIED_BY_DESIGN`

`UNBOUNDED_RECOVERY_RETRY = DENIED_BY_DESIGN_AFTER_RT9_001`

`STALE_RELEASE_TOCTOU = DENIED_BY_DESIGN_AFTER_RT9_002`

`APPLICATION_BUSINESS_LEAKAGE = NONE_IDENTIFIED`

`STAGE13_FSA_SPECIFIC_LEAKAGE = NONE_IDENTIFIED`

`FULL_STAGE0_THROUGH_STAGE9_CROSS_STAGE_VALIDATION = REQUIRED`

`STAGE9_PRODUCTION_IMPLEMENTATION_AUTHORITY = NOT_YET_GRANTED`

## 7. Next action

Create a final plan-package reconciliation that makes ACR-9-001, RT9-001 and RT9-002 explicitly binding, then present the reconciled Stage 9 Implementation Plan v0.1 package to the Project Owner for explicit implementation acceptance.
