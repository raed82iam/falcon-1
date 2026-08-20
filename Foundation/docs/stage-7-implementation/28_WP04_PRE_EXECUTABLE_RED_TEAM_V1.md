# Stage 7 WP-04 Pre-Executable Architecture / Consistency and Red-Team V1

**Date:** 2026-08-13  
**Reviewed Material Candidate:** `6865f2123a1557b2fa5f1757069fa9dd0b6b9f88`  
**Disposition:** `PASS / READY_FOR_FRESH_EXECUTABLE_REVALIDATION`  
**Technical Validation:** `NOT_YET_COMPLETE`

## 1. Review Purpose and Chronology

This record captures the full WP-04 pre-executable challenge set as actually resolved before the next exact executable revalidation.

It does not claim that every finding was discovered before the first source candidate existed. WP-04 source was already present on GitHub when some hardening findings were identified. The workstream intentionally treated those bytes as a candidate, corrected the findings, and requires a new executable run after every material source change.

A prior exact executable validation passed on commit `419185350ed4ac2695f5309eca2d9989b3ec5799`. Subsequent adversarial review found an additional Recovery exception edge, so that PASS was not used to declare WP-04 technically complete. The current material candidate is `6865f2123a1557b2fa5f1757069fa9dd0b6b9f88` and must be revalidated.

## 2. Fresh Authority and FCR Check

Fresh governing-source and current FCR review was performed before this disposition.

No actual current FCR header requires immediate Foundation or Owner action for WP-04.

- FCR-0076: `Waiting On: WEB`, WP-04 explicitly unaffected.
- FCR-0077: `Waiting On: WEB`.
- FCR-0012/FCR-0030: `Waiting On: NONE`, Stage 13-bound.
- FCR-0010/FCR-0031: `Waiting On: APPLICATION`.

Stage 7 v0.3 implementation authority remains the governing implementation authority. No Stage 8+ authority is inferred.

## 3. Challenge Matrix

| ID | Challenge | Risk if unmitigated | Disposition |
|---|---|---|---|
| R1 | evaluator placed in HealthFitness while consuming Self Model | production dependency cycle | CLOSED — evaluator placed in `Foundation.SelfAwareness` |
| R2 | Self Model assertion current at model time but stale at later Fitness time | stale truth reused as positive Fitness | CLOSED — expiry rechecked at assessment time |
| R3 | Recovery proof referenced by label only | proof from wrong area/subject/scope could satisfy exception | CLOSED — exact assertion ID + area + subject + scope + expected value binding |
| R4 | declared fault owner not bound to actual RecoveryRequired evidence | spoofed declaration could unlock restricted path | CLOSED — actual fault evidence source-owner binding required |
| R5 | second non-Recovery NOT_FIT blocker ignored | Recovery exception could override unrelated blocker | CLOSED — any other NOT_FIT blocker prevents exception |
| R6 | trust condition represented by one weak signal | unresolved trust/integrity/authority condition hidden | CLOSED — rule may require multiple distinct exact TrustBoundaryClear proofs and all must pass |
| R7 | Fitness or same assessment used as its own positive proof | circular positive evidence | CLOSED — TechnicalFitness area input rejected; direct assessment self-reference fails closed |
| R8 | EQ-LIMITED produces unrestricted FIT | weak evidence promoted | CLOSED — limited evidence cannot map to FIT |
| R9 | RESTRICTED emitted without explicit constraint | bounded mode becomes semantically unbounded | CLOSED — meaningful non-`NONE` constraint mandatory |
| R10 | result outlives supporting evidence | stale positive assessment persists | CLOSED — expiry clamped to earliest fresh evidence/requested expiry |
| R11 | input ordering changes result identity | nondeterministic technical truth | CLOSED — severity, priority and canonical ID ordering are deterministic |
| R12 | WP-04 verifier omitted from controlled solution | executable verification could be accidentally excluded | CLOSED — solution membership added and architecture guard enforces exactly once |
| R13 | `EQ-INSUFFICIENT` checked before `EQ-INVALID` | invalid evidence severity masked | CLOSED — INVALID now dominates INSUFFICIENT and regression guard added |
| R14 | direct circularity always overwrites aggregate quality with INSUFFICIENT | separately present INVALID evidence masked | CLOSED — derive aggregate first; INVALID remains dominant |
| R15 | Recovery proof contradiction visible but aggregate quality remains SUFFICIENT | contradiction/evidence classification inconsistent | CLOSED — material contradictions reduce non-invalid aggregate quality to INSUFFICIENT |
| R16 | multiple RecoveryRequired faults from different source owners share one declared exception | unbound recovery fault could inherit another fault's restricted exception | CLOSED — every active RecoveryRequired fault assertion must bind to declared fault owner |
| R17 | Recovery exception performs recovery/release/authority action | Stage 9/authority scope pulled backward | CLOSED — evaluator returns assessment only |
| R18 | Application/Web/business semantics leak into Foundation evaluator | Foundation loses zero-Application neutrality | CLOSED — production/verifier boundary contains no such surface |

## 4. Most Material Late Finding — R16

### Attack

Create two requirements that both evaluate to `RECOVERY_REQUIRED`:

- one backed by the declaration's `FaultSourceOwner`;
- one backed by a different source owner.

Under the earlier predicate, the presence of **any one** matching fault source was sufficient to continue exception evaluation.

If all Recovery proof conditions were otherwise satisfied, the second unbound RecoveryRequired fault could ride inside the first fault's declaration and the assessment could become `RESTRICTED`.

### Required behavior

The bounded Recovery exception is valid only for the fault context it explicitly declares. An unbound second RecoveryRequired fault must not inherit that exception.

### Remediation

The candidate now requires:

```text
faultEvidence is non-empty
AND
EVERY RecoveryRequired fault assertion SourceOwner == declared FaultSourceOwner
```

Otherwise:

```text
RECOVERY_EXCEPTION_DENIAL = FAULT_SOURCE_BINDING_FAILED
FITNESS_RESULT = NOT_FIT
```

A dedicated module-initializer regression guard now constructs the mixed-fault-source scenario and requires the fail-closed result.

## 5. Evidence-Quality Hardening

The final candidate also closes two evidence-classification edges found during the same adversarial pass.

### Direct circular plus invalid evidence

Earlier behavior forced `EQ-INSUFFICIENT` whenever direct circular evidence existed, even if selected evidence separately contained `EQ-INVALID`.

Current behavior preserves the stronger invalid classification:

```text
DERIVE EVIDENCE QUALITY FIRST
IF DIRECT_CIRCULAR AND QUALITY != INVALID -> INSUFFICIENT
```

The regression guard requires:

```text
DIRECT_CIRCULAR + INVALID
=> NOT_FIT
=> EQ-INVALID
```

### Recovery-proof contradiction

A contradiction inside Recovery proof evidence already denied the restricted exception, but aggregate evidence quality could remain `EQ-SUFFICIENT` because the contradiction was not part of ordinary requirement outcomes.

Current behavior requires any material assessment contradiction to reduce non-invalid aggregate quality to `EQ-INSUFFICIENT`.

The regression guard requires:

```text
RECOVERY_PROOF_CONTRADICTION
=> NOT_FIT
=> CONTRADICTION VISIBLE
=> EQ-INSUFFICIENT
```

## 6. Health-State Projection Consideration

The combined assessment carries the current Self Model Health-state claim without reimplementing SYS-008 Health derivation.

This was challenged for a case where a Health assertion claims `HEALTHY` but its evidence is invalid/insufficient. WP-04's scoped Health requirement evaluates that evidence independently and drives Technical Fitness to `UNKNOWN / NOT_FIT`; such evidence therefore cannot produce positive Fitness.

The Health-state field remains source projection rather than a second Health derivation. Exact predecessor-source authenticity and accepted-source integration remain WP-06 scope. This is not classified as a WP-04 defect because no positive Fitness or authority is created from the invalid Health evidence and duplicating SYS-008 derivation/source binding here would collapse staged responsibilities.

## 7. Architecture Boundary Review

The material candidate preserves:

```text
Foundation.Contracts <- Foundation.HealthFitness <- Foundation.SelfAwareness
```

No new production dependency is introduced by the final hardening.

The controlled solution includes the Stage 7 WP-04 verifier exactly once. The architecture guard also requires the verifier project to reference exactly:

- `Foundation.SelfAwareness`
- `Foundation.HealthFitness`

No Application or Shared Web project is involved.

## 8. No-Authority / Future-Stage Review

The material candidate exposes no public method that:

- grants/revokes/restricts authority;
- performs Guardian/Safe-State enforcement;
- isolates/kills a subject;
- runs Recovery;
- releases/revives;
- deploys/activates;
- implements Monitor AI/FSA governance/evolution;
- owns Application business meaning.

`Fitness != Permission` remains intact.

## 9. Static Change-Surface Review

The late hardening from the previously tested commit `419185350ed4ac2695f5309eca2d9989b3ec5799` to material candidate `6865f2123a1557b2fa5f1757069fa9dd0b6b9f88` is limited to:

1. `src/Foundation.SelfAwareness/TechnicalFitnessRuntime.cs`
2. `verification/Falcon.Stage7.WP04.Verifier/RecoveryExceptionSafetyGuard.cs`

The runtime delta is limited to fail-closed Recovery fault binding and evidence-quality preservation. The new verifier guard exercises the three late adversarial cases.

No Application/Web/reference path changed.

## 10. Findings at Pre-Executable Freeze

All identified findings are closed in source or explicit verifier coverage before fresh executable revalidation.

- Critical open: `0`
- High open: `0`
- Medium open: `0`
- Low open: `0`

This count does not convert static review into executable proof.

## 11. Final Pre-Executable Disposition

```text
WP04_PRE_EXECUTABLE_ARCHITECTURE_CONSISTENCY = PASS
WP04_PRE_EXECUTABLE_RED_TEAM = PASS
WP04_READY_FOR_FRESH_EXECUTABLE_REVALIDATION = YES
WP04_CURRENT_MATERIAL_CANDIDATE = 6865f2123a1557b2fa5f1757069fa9dd0b6b9f88
WP04_CURRENT_CANDIDATE_EXECUTABLE_VALIDATION = NOT_YET_RUN
WP04_TECHNICALLY_VALIDATED = NO
WP04_OWNER_CLOSURE = DEFERRED
STAGE8_AUTHORITY = NOT_GRANTED
```

Fresh exact Windows executable validation is mandatory before the post-executable Red-Team and technical-validation disposition.