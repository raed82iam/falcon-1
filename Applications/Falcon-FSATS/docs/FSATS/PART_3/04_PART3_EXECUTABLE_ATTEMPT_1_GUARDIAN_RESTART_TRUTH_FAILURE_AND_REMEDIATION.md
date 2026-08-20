# FSATS Part 3 — Executable Attempt 1 Guardian Restart-Truth Failure and Remediation

**Status:** `EXECUTABLE_FAILURE_RECORDED / SOURCE_REMEDIATED / REVALIDATION_REQUIRED`  
**Failed exact source:** `35fc0f633507572cb70f7e05cdccfef86cb3117f`  
**Remediated exact source:** `0be363b713e96c8b1eeb81ae7e5fb7e5d5e562b4`  
**Branch:** `application-development`

## 1. Owner-Operated Exact Executable Evidence

The Project Owner executed the isolated Part 3 validation harness against exact source `35fc0f633507572cb70f7e05cdccfef86cb3117f` using .NET SDK `10.0.302`.

Observed evidence before failure:

```text
EXACT DETACHED HEAD = MATCH
INITIAL WORKTREE = CLEAN
RESTORE = PASS
RELEASE BUILD = PASS
DIRECT PART 3 BEHAVIOR = FAIL
```

The first material behavior failure was:

```text
P3_GUARDIAN_AMBIGUOUS_PROTECTION_RESTARTED_AS_SUCCESS
```

The validation stopped at the direct Part 3 behavior verifier. Direct Failure verification, governed verifier run 1, governed verifier run 2, final exact-HEAD proof, and final clean-tree proof were therefore not reached in this attempt.

## 2. Root Cause

`GuardianRestartReconstructor.Reconstruct(...)` correctly classified these pre-restart protection outcomes as reconciliation-owned:

```text
Received
Accepted
PartiallyApplied
DispatchFailed
ReconciliationRequired
```

and converted them into reconstructed `ReconciliationRequired` outcomes.

However, its independent `RequiresCurrentProtectionTruthVerification` flag was narrower and included only:

```text
Applied
PartiallyApplied
ReconciliationRequired
```

Therefore a durable `Accepted` outcome was correctly placed in the reconciliation list while incorrectly reporting that current protection truth did not require verification.

This was a real Part 3 semantic inconsistency, not a harness failure.

## 3. Safety Meaning

For restart truth:

```text
ACCEPTED_BEFORE_RESTART != APPLIED_NOW
RECEIVED_BEFORE_RESTART != APPLIED_NOW
DISPATCH_FAILED_BEFORE_RESTART != SAFE_CURRENT_PROTECTION_TRUTH
PARTIALLY_APPLIED != COMPLETE_PROTECTION
RECONCILIATION_REQUIRED != VERIFIED_CURRENT_TRUTH
```

A historical/ambiguous protection state that can still affect current safety must remain visible as requiring current protection truth verification after process recreation.

## 4. Remediation

Exact remediation commit:

`0be363b713e96c8b1eeb81ae7e5fb7e5d5e562b4`

Changed only:

`applications/FSATS/src/TradingGuardian/Falcon.FSATS.TradingGuardian.Application/DurableRestartRecovery.cs`

The reconstruction policy is now explicit through two separate classifications:

### Reconciliation-owned after restart

```text
Received
Accepted
PartiallyApplied
DispatchFailed
ReconciliationRequired
```

These reconstruct as `ReconciliationRequired`.

### Requires current protection-truth verification

```text
Received
Accepted
Applied
PartiallyApplied
DispatchFailed
ReconciliationRequired
```

`Applied` remains historical applied evidence but still cannot prove current protection truth after restart.

Terminal negative outcomes remain distinct:

```text
Rejected
Expired
Revoked
```

They are not silently converted into active protection.

## 5. Authority / Boundary Check

```text
FOUNDATION WRITE = NONE
SHARED WEB WRITE = NONE
PART 4 WORK = NONE
RUNTIME ACTIVATION = NONE
EXTERNAL BROKER/PROVIDER ACTIVATION = NONE
```

The remediation is Application-owned Guardian restart semantics and remains inside the Owner-authorized Part 3 scope.

## 6. Review Consequence

The earlier pre-executable Architecture/Consistency and Red-Team reviews for `35fc0f...` remain historical evidence for that exact source only. They are not current evidence for the remediated source.

Because the executable test exposed a semantic defect and source bytes changed, the remediated source requires:

```text
FRESH ARCHITECTURE / CONSISTENCY REVIEW
-> FRESH RED-TEAM REVIEW
-> EXACT EXECUTABLE REVALIDATION
```

## 7. Current State

```text
PART 3 EXECUTABLE ATTEMPT 1 = FAILED
ROOT CAUSE = IDENTIFIED
SOURCE REMEDIATION = APPLIED
REMEDIATED SOURCE = 0be363b713e96c8b1eeb81ae7e5fb7e5d5e562b4
EXECUTABLE REVALIDATION = REQUIRED
PART 3 OWNER CLOSURE = NOT ELIGIBLE
PART 4 = NOT AUTHORIZED
RUNTIME = NOT AUTHORIZED
```
