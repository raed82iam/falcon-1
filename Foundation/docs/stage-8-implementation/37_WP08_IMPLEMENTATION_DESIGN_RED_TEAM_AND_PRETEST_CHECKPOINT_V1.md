# Stage 8 WP-08 Implementation Design, Red Team and Pretest Checkpoint V1

**Stage:** 8 — Foundation Guardian, Protective Restriction and Platform Safe State  
**WP:** WP-08 — Independent Emergency Control, Guardian-Compromise Containment & Blast-Radius Isolation  
**Status:** IMPLEMENTED_AWAITING_EXECUTABLE_VALIDATION  
**Branch:** `foundation-development`  
**Exact pretest candidate:** `e4a808af83ccf9ca3c277f69ee979a2f41a60c34`

## Governing sources fresh-read

- FCR Shared Registry and Operating Protocol, Issue #1;
- FCR-0076 current body and all comments;
- FCR-0082 current body and all comments;
- Stage 8 Gate 0A existing-capability reconciliation;
- Stage 8 Gate 0B Guardian jurisdiction/protective mandate;
- Stage 8 Owner-authorized Implementation Plan v0.1;
- AUT-001 Authority Engine v1.1;
- AUT-002 Guardian v1.0 and relevant AMD-005/AMD-006/AMD-007 governed architecture history;
- current WP-03/WP-04/WP-05/WP-06/WP-07 production surfaces.

## Implementation

WP-08 adds the independent emergency-control path outside `Foundation.Guardian`:

- `src/Foundation.Authority/IndependentEmergencyControl.cs`;
- `src/Foundation.Authority/IndependentEmergencyControlAuthorityEnforcer.cs`;
- `verification/Falcon.Stage8.WP08.Verifier/`.

The existing `DefaultDenyAuthorityEngine`, protective-restriction enforcer and Lifecycle owner are reused. WP-08 does not create a second Authority Engine, Lifecycle engine, inventory service, recovery engine or Guardian.

## Authority and admission boundary

An emergency request is not itself authority.

`IndependentEmergencyControlRuntime` requires:

1. a bounded emergency-control request;
2. a canonical AUT-001 `AuthorityRequest`;
3. an `AuthorityEvaluationContext`;
4. blast-radius evidence;
5. an exact observation time.

The runtime performs `DefaultDenyAuthorityEngine` evaluation internally. It does not accept a caller-supplied `AuthorityResult` as proof of authorization.

The supported narrow emergency-control action vocabulary is:

```text
HOLD
DENY_NEW_ACTIVITY
ISOLATE_TARGET
ENTER_PLATFORM_SAFE
EMERGENCY_STOP
```

This is a protective-control vocabulary only. It grants no ordinary execution authority.

## Decision anti-forgery boundary

`IndependentEmergencyControlDecision` is public/readable but its constructor is `internal` and all properties are read-only.

Therefore an external Application/Web consumer cannot directly construct an `Accepted=true` emergency-control decision and inject it into the WP-08 Authority overlay. The Authority runtime remains the construction boundary. Structural validation remains in the consumer as defense in depth.

## Blast-radius semantics

WP-08 uses explicit scope classes:

- `Principal`;
- `Application`;
- `FalconWide`.

A requested local scope may remain local only when all of the following are independently trustworthy:

- the local containment boundary;
- propagation is excluded;
- the unaffected scope is trustworthy;
- the blast-radius evidence source is trustworthy.

Additionally, if Guardian compromise is suspected and Guardian evidence is the sole source, locality is not considered proven.

If any required locality condition is unavailable, contradictory, compromised, possible or unknown, the effective containment scope expands fail-closed to:

`FalconWide / falcon:platform`

This expansion is a Foundation protective containment consequence under the Stage 8 mandate. It is **not** an expansion of the requesting Owner/Application execution authority.

## Unaffected operation

`UnaffectedOperationEligible = true` is possible only for independently trustworthy unaffected scope under proven local containment.

Even then:

`UNAFFECTED_OPERATION_ELIGIBLE != AUTHORITY_GRANTED`

AUT-001 remains independently required.

If unaffected-scope trust is unavailable or untrusted, the control does not merely hide the eligibility flag. It expands containment so the untrusted outside scope is actually denied by the Authority overlay.

## Enforcement

`IndependentEmergencyControlAuthorityEnforcer` composes on top of the existing `ProtectiveRestrictionAuthorityEnforcer`.

It does not alter the earlier WP-04/WP-07 enforcement path.

For an applicable active WP-08 containment decision:

- non-safe governed actions are denied;
- canonical Safe-State actions remain eligible for separate AUT-001 evaluation;
- `Principal` scope applies to the exact subject;
- `Application` scope applies to the exact Application scope and its subordinate scope path;
- `FalconWide` applies platform-wide;
- missing emergency-control state on the WP-08 enforcement path fails closed;
- malformed emergency-control state fails closed.

## Lifecycle ownership and blast-radius isolation

The exact target is projected into a canonical persistent CON-011 `SAFE` restriction and handed to the existing Lifecycle enforcement path.

For the target, current Lifecycle semantics enforce:

- `Stopped`;
- isolation required;
- no new execution;
- restriction remains active.

For an Application/Falcon-wide blast-radius expansion, WP-08 applies Authority denial over the effective scope. It does not invent a bulk Lifecycle inventory/orchestrator because the current Lifecycle API is target-specific and no canonical target inventory is owned by this WP.

Additional physical Lifecycle transitions remain target-specific actions performed by the Lifecycle owner for known governed targets. Authority-wide containment already prevents untrusted governed execution while those target-specific transitions are applied.

## Persistence and release boundary

The decision carries a `ReviewDeadline`, not an expiry/release time.

`REVIEW_DEADLINE != RELEASE`

The canonical target restriction uses:

- `Expiry = DateTimeOffset.MaxValue`;
- `ReleaseConditions = STAGE9_INDEPENDENT_RECOVERY_VALIDATION_AND_AUTHORIZED_RELEASE_REQUIRED`;
- `ReleaseAuthority = INDEPENDENT_GOVERNED_RELEASE_AUTHORITY`.

Time passage, restart, Guardian return, source reappearance, self-attestation or review deadline do not release containment.

WP-08 exposes no public recovery, release, trust-restoration or Controlled Revival method.

## Web/mobile boundary

WP-08 does not implement or claim browser/mobile Owner authentication, session bootstrap, step-up re-authentication, natural-language command admission, or Web transport.

Those remain separate admission/presentation concerns identified in FCR-0076. A UI click or sent request is not authorization or completed action.

## Red Team chronology

### Finding 1 — decision expiry could be misread as release

**Severity:** HIGH  
**Finding:** initial decision surface used `Expiry`, creating an unsafe implication that containment could end by time passage.  
**Remediation:** replaced with `ReviewDeadline`; target restriction is indefinite until governed Stage 9 release.  
**Status:** CLOSED.

### Finding 2 — caller-supplied ALLOW result

**Severity:** CRITICAL  
**Finding:** initial runtime accepted an `AuthorityResult` argument, allowing a caller to attempt a forged ALLOW object.  
**Remediation:** runtime now performs `DefaultDenyAuthorityEngine` evaluation internally from request + context.  
**Status:** CLOSED.

### Finding 3 — untrusted unaffected scope not actually contained

**Severity:** HIGH  
**Finding:** an early version could preserve local effective scope while only marking the outside scope ineligible. The Authority overlay would therefore not actively deny that untrusted outside scope.  
**Remediation:** preserving locality now also requires independently trustworthy unaffected-scope evidence. Otherwise effective containment expands to Falcon-wide.  
**Status:** CLOSED.

### Finding 4 — externally constructible accepted emergency decision

**Severity:** CRITICAL  
**Finding:** a public positional decision record could be constructed outside `Foundation.Authority` and could attempt to bypass the runtime admission path.  
**Remediation:** decision construction is now internal to `Foundation.Authority`; public consumers receive read-only decision state only.  
**Status:** CLOSED.

### Finding 5 — verifier assertion-count drift

**Severity:** VERIFIER-ONLY  
**Finding:** early verifier revisions carried stale assertion counts during rapid hardening.  
**Remediation:** verifier was normalized to 30 explicit numbered checks.  
**Status:** CLOSED_FOR_PRETEST.

## Current verifier coverage

The WP-08 verifier now contains 30 explicit checks covering:

- trusted local containment;
- unaffected-operation authority separation;
- internal-only emergency decision construction;
- deterministic/mutation-sensitive decision identity;
- compromised-Guardian sole-evidence expansion;
- unknown propagation expansion;
- untrusted blast-radius evidence-source expansion;
- untrusted unaffected-scope expansion;
- AUT-001 denial;
- request/authority binding mismatch;
- wrong-target blast-radius evidence;
- canonical SAFE restriction projection;
- non-time-releasable target restriction;
- Stage 9 release ownership;
- Lifecycle stop/isolation;
- Application-local containment;
- Authority denial inside Application scope;
- canonical Safe-State action eligibility only through AUT-001;
- no principal-scope leakage;
- Falcon-wide denial;
- review deadline does not release;
- missing emergency-control evidence fails closed;
- explicit Falcon-wide control;
- narrow emergency action vocabulary;
- no Stage 9 recovery/release surface;
- no `Foundation.Guardian` dependency in the independent Authority path;
- no Trading/Application business-type leakage.

## Executable status

Executable validation has **not** been claimed.

GitHub Actions attempted to run after the WP-08 pushes, but the job did not start. GitHub reported that recent account payments failed or the Actions spending limit must be increased. The boundary job contained zero executed steps and the build/test job was skipped.

Therefore:

`GITHUB_ACTIONS_FAILURE != WP08_TECHNICAL_FAILURE`

but also:

`GITHUB_ACTIONS_FAILURE != WP08_EXECUTABLE_PASS`

A fresh exact-candidate Owner-side executable validation remains mandatory.

## Pre-executable Red Team result

- Critical: 0 open;
- High: 0 open;
- Medium: 0 open;
- Product-Low: 0 open;
- verifier-only known defect: 0 open;
- disposition: `PASS_FOR_EXECUTABLE_VALIDATION`.

## FCR state

FCR-0076 remains `Waiting On: FOUNDATION`.

FCR-0082 remains `Waiting On: FOUNDATION`.

WP-08 implementation does not make either FCR closure-eligible because governed executable validation is still pending and WP-09/WP-10 remain in the Owner-authorized Stage 8 sequence.

## Cadence

`WP08_IMPLEMENTATION = COMPLETE_FOR_PRETEST`

`WP08_EXECUTABLE_VALIDATION = REQUIRED`

`WP08_OWNER_CLOSURE = NOT_REQUESTED`

`STAGE9_RECOVERY_RELEASE = NOT_IMPLEMENTED`

`NEXT_ON_PASS = WP09_AUTOMATIC_CONTINUITY`
