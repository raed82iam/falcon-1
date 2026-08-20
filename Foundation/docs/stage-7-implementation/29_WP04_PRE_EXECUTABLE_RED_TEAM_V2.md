# Stage 7 WP-04 Pre-Executable Architecture / Consistency and Red-Team V2

**Date:** 2026-08-13  
**Supersedes for current readiness:** `28_WP04_PRE_EXECUTABLE_RED_TEAM_V1.md`  
**Material Source Candidate:** `db617a91c580b385547f4812773e09172eab08ae`  
**Disposition:** `PASS / READY_FOR_FRESH_EXECUTABLE_REVALIDATION`  
**Technical Validation:** `NOT_YET_COMPLETE`

## 1. Why V2 Exists

V1 truthfully recorded the adversarial findings known at its freeze point. The review continued before a new executable run was requested and found two additional boundary problems. V1 is preserved as historical evidence and is not rewritten.

V2 adds and closes:

- R19 — non-canonical Self Model snapshot consistency bypass;
- R20 — loss of independent RESTRICTED constraints when the Recovery exception is also RESTRICTED.

The material implementation candidate after those closures is `db617a91c580b385547f4812773e09172eab08ae`.

The exact branch commit used for the next Windows executable validation shall be the `foundation-development` HEAD containing this V2 record. Documentation commits after the material source candidate do not change production/verifier source, but the executable procedure shall still bind to one exact branch HEAD and verify that remote HEAD does not move during the run.

## 2. Fresh FCR / Authority State

Fresh current FCR review immediately before this V2 freeze found no current WP-04 handoff requiring Foundation or Owner action.

Current relevant headers remain:

- FCR-0076: `Waiting On: WEB`; explicitly states Stage 7 WP-04 is unaffected;
- FCR-0077: `Waiting On: WEB`;
- FCR-0012 and FCR-0030: `Waiting On: NONE`, Stage 13-bound;
- FCR-0010 and FCR-0031: `Waiting On: APPLICATION`.

The Stage 7 v0.3 Owner authorization remains the implementation authority. No FCR grants extra authority and no Stage 8+ authority is inferred.

## 3. R19 — Non-Canonical Self Model Consistency Bypass

### Attack

`FoundationSelfModelSnapshot` is a public immutable record. A caller can construct or copy a snapshot directly rather than obtaining the exact output of `FoundationSelfModelProjector.Build(...)`.

Before this hardening, WP-04 validated basic model identifiers and then trusted the supplied `Assertions`, `Contradictions` and `EvidenceReference` as a coherent package.

An adversarial caller could therefore:

1. start from assertions containing a real current contradiction;
2. remove the contradiction from the supplied `Contradictions` list;
3. pass the manually altered snapshot to Fitness evaluation.

If the contradiction was not otherwise selected by a rule, WP-04 had no independent structural proof that the snapshot still matched canonical projector semantics.

### Required behavior

WP-04 must consume a structurally canonical Self Model, not merely a record whose individual fields look valid.

This requirement does **not** authenticate external authoritative sources and does not replace WP-06 predecessor/source binding. It only ensures the supplied snapshot is internally consistent with the already accepted WP-03 canonical projector.

### Remediation

`ValidateModel(...)` now rebuilds the snapshot through the canonical projector using the supplied model identity inputs and assertions:

```text
CANONICAL = FoundationSelfModelProjector.Build(...)
REQUIRE CANONICAL.Identity == supplied Model.Identity
```

The rebuild also re-applies existing WP-03 validation for:

- assertion validity;
- duplicate IDs;
- required current-area coverage;
- canonical contradiction derivation;
- canonical Self Model evidence reference;
- deterministic snapshot identity.

Any mismatch is rejected with:

`Stage 7 Technical Fitness non-canonical Self Model rejected`

### Regression guard

`ModelIntegrityAndConstraintGuard` constructs a canonical Self Model with a real Security contradiction, then forges a copy whose `Contradictions` list is empty. WP-04 must reject it before Fitness evaluation proceeds.

## 4. R20 — RESTRICTED Constraint Loss Under Recovery Exception

### Attack

A result may legitimately contain more than one simultaneous reason for restriction.

Example:

- `RecoveryRequired` satisfies the narrowly declared Recovery restricted-mode exception;
- another independent requirement evaluates to `DEGRADED`, with its own required constraint.

Earlier `BuildConstraints(...)` returned the Recovery declaration constraint immediately whenever the Recovery exception was satisfied. That could drop the second required restriction from the combined CON-006 assessment.

The result remained `RESTRICTED`, so this did not promote to FIT, but the emitted operating boundary was incomplete.

### Required behavior

A RESTRICTED result must preserve **all material simultaneously applicable restrictions**.

### Remediation

`BuildConstraints(...)` now:

1. collects every non-FIT outcome whose base mapping is `RESTRICTED`;
2. collects each meaningful declared requirement constraint;
3. adds the Recovery restricted-mode constraint when the Recovery exception is active;
4. removes blank/`NONE` values;
5. de-duplicates and sorts deterministically;
6. emits the union.

No applicable restriction may disappear merely because Recovery is also active.

### Regression guard

`ModelIntegrityAndConstraintGuard` creates a scenario with:

- Recovery exception -> `RESTRICTED`;
- BackupCondition -> `DEGRADED / RESTRICTED` with `constraint:backup-degraded`.

The verifier requires the final assessment to contain both:

- `recovery-bounded:read-only-operation`;
- `constraint:backup-degraded`.

## 5. Reconciliation With V1 Findings

All V1 findings R1-R18 remain closed. V2 does not reopen or weaken them.

The complete closed challenge set now covers at least:

- production dependency-cycle avoidance;
- freshness at Fitness assessment time;
- exact Recovery proof identity/area/subject/scope/value binding;
- fault-source binding;
- second NOT_FIT blocker preservation;
- trust proof requirements;
- circular positive proof denial;
- EQ-LIMITED no-FIT rule;
- meaningful RESTRICTED constraints;
- expiry clamp;
- deterministic result selection;
- controlled-solution membership;
- INVALID > INSUFFICIENT precedence;
- direct-circular evidence not masking INVALID;
- Recovery-proof contradiction -> insufficient evidence;
- all RecoveryRequired faults bound to the declared fault owner;
- no authority/recovery/future-stage action surface;
- no Application/Web/business semantic leakage;
- canonical Self Model structural consistency;
- complete union of simultaneous RESTRICTED constraints.

## 6. Static Change-Surface Audit

The delta from V1 branch freeze `019a1a3f52dd9916afa9183774553553fe6adf2e` to material candidate `db617a91c580b385547f4812773e09172eab08ae` is exactly:

1. `src/Foundation.SelfAwareness/TechnicalFitnessRuntime.cs`
2. `verification/Falcon.Stage7.WP04.Verifier/ModelIntegrityAndConstraintGuard.cs`

No `applications/**`, Web-owned, `reference/**` or Stage 8+ implementation surface is part of this delta.

The production dependency graph remains:

```text
Foundation.Contracts <- Foundation.HealthFitness <- Foundation.SelfAwareness
```

No new production project or dependency edge was introduced.

## 7. Executable Guard Inventory

The WP-04 verifier now includes module-initializer regression guards for late adversarial findings:

- `EvidenceQualityPrecedenceGuard`
  - INVALID dominates INSUFFICIENT;
- `RecoveryExceptionSafetyGuard`
  - all RecoveryRequired fault owners bind;
  - direct circularity does not mask INVALID;
  - Recovery proof contradiction is visible and insufficient;
- `ModelIntegrityAndConstraintGuard`
  - hidden canonical contradiction cannot be suppressed by a forged Self Model snapshot;
  - simultaneous RESTRICTED constraints are preserved together.

The Architecture test also contains `Stage7Wp04ArchitectureGuard`, which requires controlled-solution membership exactly once and the intended verifier project-reference boundary.

Because these are executable/module-initializer guards, a successful Architecture/WP-04 executable run proves the guard code was loaded and did not throw.

## 8. Historical Executable Evidence

Commit `419185350ed4ac2695f5309eca2d9989b3ec5799` previously passed exact Windows executable validation:

- exact remote/local commit match;
- .NET SDK 10.0.302;
- controlled restore and Release build;
- Architecture PASS;
- Security PASS with zero findings;
- WP-01/WP-02/WP-03 regressions PASS;
- WP-04 run 1 PASS;
- WP-04 run 2 PASS;
- deterministic rerun identical;
- material DLL hashes stable before/after execution;
- clean worktree;
- remote HEAD unchanged.

That evidence remains valid for the exact 419185 bytes only. It is **not** reused as executable proof for the V2 material candidate.

## 9. Open Findings at V2 Freeze

After the V2 static/architecture/adversarial pass:

- Critical open: `0`
- High open: `0`
- Medium open: `0`
- Low open: `0`

No known source/design finding remains open before executable revalidation.

This is a readiness result, not executable proof.

## 10. Final V2 Disposition

```text
WP04_PRE_EXECUTABLE_ARCHITECTURE_CONSISTENCY_V2 = PASS
WP04_PRE_EXECUTABLE_RED_TEAM_V2 = PASS
WP04_READY_FOR_FRESH_EXECUTABLE_REVALIDATION = YES
WP04_MATERIAL_SOURCE_CANDIDATE = db617a91c580b385547f4812773e09172eab08ae
WP04_EXECUTABLE_VALIDATION_FOR_V2 = NOT_YET_RUN
WP04_TECHNICALLY_VALIDATED = NO
WP04_OWNER_CLOSURE = DEFERRED
STAGE7_OWNER_CLOSURE = NOT_IMPLIED
STAGE8_AUTHORITY = NOT_GRANTED
```

The next permitted action is one exact frozen Windows executable validation against the final `foundation-development` branch HEAD containing this record. If any executable check fails, WP-04 remains unvalidated and the failure returns to remediation plus another full validation cycle.