# Stage 7 WP-04 Implementation Design and Trace

**Date:** 2026-08-13  
**Work Package:** Stage 7 WP-04 — Technical Fitness Evaluation and CON-006 Projection  
**Current Candidate Commit at Trace Freeze:** `6865f2123a1557b2fa5f1757069fa9dd0b6b9f88`  
**Owner Closure:** `DEFERRED`  
**Stage 8 Authority:** `NOT_GRANTED`

## 1. Documentary Chronology

This record documents the actual WP-04 implementation chain without rewriting history.

The first WP-04 source/verifier candidate was created on `foundation-development` before final executable validation documentation was materialized. A first exact Windows executable validation later passed on commit `419185350ed4ac2695f5309eca2d9989b3ec5799` after controlled-solution membership and evidence-quality precedence hardening.

The subsequent post-executable adversarial review found one additional Recovery-exception fail-closed edge: multiple `RECOVERY_REQUIRED` fault evidences from different source owners could reach the bounded Recovery exception when only one fault source matched the declaration. That candidate was therefore not declared technically complete.

The finding was remediated and regression coverage was added. The current candidate at this trace freeze is `6865f2123a1557b2fa5f1757069fa9dd0b6b9f88`. A fresh executable revalidation is required before WP-04 may be marked technically validated.

## 2. Authority and Current FCR State

Implementation is authorized by the accepted Stage 7 v0.3 Owner authorization. WP-04 remains within Stage 7 scope only.

Fresh current FCR review on 2026-08-13 found no actual open current-header handoff with `Waiting On: FOUNDATION` or `Waiting On: OWNER` that blocks WP-04.

Relevant current records remain:

- FCR-0076: `Waiting On: WEB`; explicitly states Stage 7 WP-04 is unaffected.
- FCR-0077: `Waiting On: WEB`; Application/Web planning coordination only.
- FCR-0012 and FCR-0030: `Waiting On: NONE`; Stage 13-bound.
- FCR-0010 and FCR-0031: `Waiting On: APPLICATION`.

No FCR creates additional implementation authority.

## 3. Governing Sources

WP-04 is traced against:

- Falcon Vision v1.0;
- Falcon Constitution v1.0;
- Stage 7 Owner implementation authorization v0.3;
- accepted Stage 7 plan v0.3;
- Gate 0B plan reconciliation and activation sync;
- AWR-001 v2.1;
- CON-006 v1.2;
- SYS-008 v1.1;
- validated WP-01, WP-02 and WP-03 predecessor behavior.

The controlling semantic rules include:

```text
UNKNOWN != FIT
FITNESS != AUTHORITY
SELF_AWARENESS != AUTHORITY
RECOVERY_REQUIRED DEFAULT = NOT_FIT
RECOVERY_REQUIRED -> RESTRICTED ONLY WHEN ALL DECLARED BOUNDED CONDITIONS ARE SATISFIED
```

## 4. Placement and Dependency Direction

The evaluator is implemented in:

`src/Foundation.SelfAwareness/TechnicalFitnessRuntime.cs`

This placement is intentional because WP-04 consumes the Foundation Self Model while reusing Health/Fitness primitives and CON-006 projection.

Production dependency direction remains:

```text
Foundation.Contracts
        ^
        |
Foundation.HealthFitness
        ^
        |
Foundation.SelfAwareness
```

Moving the evaluator into `Foundation.HealthFitness` while consuming `Foundation.SelfAwareness` would create an invalid production cycle. WP-04 therefore does not duplicate HealthFitness contract types or create a reverse dependency.

## 5. Reused Existing Surfaces

WP-04 reuses rather than duplicates:

- `CanonicalHealthFitnessAssessment`;
- `TechnicalFitnessState`;
- `FitnessProjectionResult`;
- `EvidenceQuality`;
- `HealthFitnessPrimitiveValidator`;
- `HealthFitnessContractProjection.ToContractV12(...)`;
- `HealthFitnessV12Validator`;
- `FoundationSelfModelSnapshot` and its attributable assertions/contradictions.

No second CON-006 representation is introduced.

## 6. Fixed Technical-Fitness Mapping

The evaluator preserves the accepted mapping:

| Technical state | CON-006 result |
|---|---|
| `FIT` | `FIT` |
| `FIT_WITH_CONSTRAINTS` | `RESTRICTED` |
| `DEGRADED` | `RESTRICTED` |
| `UNKNOWN` | `NOT_FIT` |
| `UNAVAILABLE` | `NOT_FIT` |
| `INTEGRITY_FAILURE` | `NOT_FIT` |
| `ISOLATION_REQUIRED` | `RESTRICTED` |
| `RECOVERY_REQUIRED` | `NOT_FIT` by default; bounded exception only |
| `NOT_FIT` | `NOT_FIT` |

Any path capable of producing `RESTRICTED` requires a meaningful non-`NONE` constraint.

## 7. Rule and Evidence Evaluation

Each rule binds:

- rule identity/version;
- Foundation subject;
- capability;
- requested authority level as evaluated context, not permission;
- scope;
- explicit requirements;
- accepted value identities;
- failure state;
- limited-evidence state;
- deterministic failure priority;
- optional required source owner;
- bounded Recovery declaration where applicable.

Every rule must include a current scoped HealthCondition requirement.

For each required Self Model input, WP-04:

1. selects only `CURRENT` assertions in the exact declared area/subject/scope;
2. applies required source-owner filtering when declared;
3. rechecks expiry at Fitness assessment time, not merely Self Model creation time;
4. preserves current contradictions;
5. rejects explicit unknown evidence from positive inference;
6. applies evidence-quality precedence;
7. tests exact acceptable value identity;
8. maps failure or limited evidence deterministically.

Missing, stale, unknown, invalid, insufficient or materially contradictory required evidence cannot produce `FIT`.

## 8. Evidence-Quality Rules

Current precedence is fail-closed:

```text
EQ-INVALID
> EQ-INSUFFICIENT
> EQ-LIMITED
> EQ-SUFFICIENT
```

`EQ-LIMITED` cannot produce unrestricted `FIT`.

Direct circular evidence cannot promote a result and cannot mask a separately present `EQ-INVALID` condition.

A material contradiction participating in the Fitness assessment cannot coexist with aggregate `EQ-SUFFICIENT`; absent higher-severity invalid evidence it reduces aggregate evidence quality to `EQ-INSUFFICIENT`.

## 9. Circular-Proof Boundary

WP-04 prevents circular positive proof by:

- rejecting `FoundationSelfModelArea.TechnicalFitness` as a Fitness input requirement;
- rejecting a direct selected-evidence `SourceAssessmentReference` back to the assessment being produced;
- forcing direct circularity to `TechnicalFitnessState.UNKNOWN` / `FitnessResult.NOT_FIT`.

This does not create an independent source-resolution layer; exact predecessor authoritative-source integration remains WP-06 scope.

## 10. RecoveryRequired Bounded Exception

`RECOVERY_REQUIRED` maps to `NOT_FIT` unless the declared exception is fully satisfied.

A Recovery restricted-mode declaration binds:

- exact fault source owner;
- explicit restricted constraints;
- exact proof assertion identities;
- proof area;
- subject;
- scope;
- expected value;
- optional required source owner.

The proof set must cover:

1. fault technically isolated;
2. requested capability independent of affected path;
3. fresh independent usability proof;
4. trust boundary clear.

All selected proof evidence must be current, unexpired, non-unknown, `EQ-SUFFICIENT`, exact-value matched and contradiction-free. Direct circular proof is denied. Independent usability proof cannot originate from the declared fault source owner.

The final hardening also requires **every** active assertion participating in a `RECOVERY_REQUIRED` outcome to bind to the declared fault source owner. A second unbound RecoveryRequired fault therefore fails closed with `FAULT_SOURCE_BINDING_FAILED` instead of inheriting the exception from another fault.

A separate non-Recovery `NOT_FIT` blocker always prevents the Recovery exception.

Recovery exception evaluation never performs recovery, release, restart, authority grant, lifecycle transition, isolation or Guardian action.

## 11. Determinism and Time Bounding

Deterministic result selection uses:

1. severity class `NOT_FIT > RESTRICTED > FIT`;
2. declared failure priority within the selected class;
3. canonical requirement ID as deterministic tie-breaker.

Evidence references are SHA-256 identities over rule identity, exact Self Model identity, exact selected evidence identities and contradiction identities.

Assessment expiry is clamped to the earliest supporting fresh evidence expiry and requested expiry. Positive Fitness therefore cannot outlive its selected evidence.

## 12. Health Projection Boundary

The `HealthState` carried by the combined assessment remains the projected current Health-condition claim from the Self Model. WP-04 does not reimplement SYS-008 Health derivation.

However, the scoped Health requirement participates in Fitness evidence evaluation. Invalid, insufficient, unknown, stale or contradictory Health evidence therefore fails Fitness closed and cannot support positive Fitness.

Exact predecessor source authenticity and accepted-source binding remain later WP-06 integration responsibilities; WP-04 does not pull that integration policy backward.

## 13. Authority and Future-Stage Exclusions

WP-04 produces an assessment only.

It does not:

- grant/revoke/restrict authority;
- issue Guardian/protection commands;
- transition lifecycle;
- isolate or kill a subject;
- execute Recovery;
- release/revive runtime state;
- deploy/activate anything;
- implement Stage 8 Guardian/Safe-State behavior;
- implement Stage 9 Recovery behavior;
- implement Stage 13 FSA governance/Monitor AI/evolution control plane;
- introduce Application, Web, trading, market, portfolio, broker or strategy semantics.

## 14. Current Candidate Change Surface

Relative to the completed WP-03 documentation head `9856337292176bab64baa6a01c057165ef7a42fa`, WP-04 changes are limited to Foundation-owned runtime, controlled-solution/test/verifier surfaces and this Stage 7 documentation.

The material runtime/verifier surfaces are:

1. `Falcon.Foundation.ControlledProjectFoundation.slnx`
2. `src/Foundation.SelfAwareness/TechnicalFitnessRuntime.cs`
3. `tests/Falcon.Foundation.Architecture.Tests/Stage7Wp04ArchitectureGuard.cs`
4. `verification/Falcon.Stage7.WP04.Verifier/Falcon.Stage7.WP04.Verifier.csproj`
5. `verification/Falcon.Stage7.WP04.Verifier/Program.cs`
6. `verification/Falcon.Stage7.WP04.Verifier/EvidenceQualityPrecedenceGuard.cs`
7. `verification/Falcon.Stage7.WP04.Verifier/RecoveryExceptionSafetyGuard.cs`

No `applications/**`, Shared Web-owned or `reference/**` path is part of the WP-04 implementation surface.

## 15. Validation State at This Record

Commit `419185350ed4ac2695f5309eca2d9989b3ec5799` previously passed a complete exact Windows executable validation, including controlled Release build, Architecture, Security, WP-01/WP-02/WP-03 regression, WP-04 twice, deterministic rerun, stable material hashes, clean worktree and exact remote HEAD.

That validation remains valid historical evidence for the bytes it tested, but it does **not** validate the later hardening commits.

Current state:

```text
WP04_CURRENT_CANDIDATE = 6865f2123a1557b2fa5f1757069fa9dd0b6b9f88
WP04_CURRENT_CANDIDATE_EXECUTABLE_VALIDATION = NOT_YET_RUN
WP04_TECHNICALLY_VALIDATED = NO
WP04_OWNER_CLOSURE = DEFERRED
STAGE8_AUTHORITY = NOT_GRANTED
```

A fresh exact executable revalidation of the current candidate is mandatory before post-executable technical closure.