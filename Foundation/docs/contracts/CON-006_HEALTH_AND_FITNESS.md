# CON-006 — Health and Fitness Contract

**Version:** 1.2  
**Status:** Approved  
**Approval Reference:** Explicit Project Owner approval in the Falcon Foundation workstream on 2026-08-12, limited to the Gate 0B additions defined in this version  
**Documentary Activation:** Active for the approved documentary semantics in this version  
**Effective Date:** 2026-08-12  
**Supersedes:** CON-006 v1.1  
**Identifier:** CON-006  
**Canonical Target:** `docs/contracts/CON-006_HEALTH_AND_FITNESS.md`  
**Governing Authority:** AWR-001 v2.1; SYS-008 v1.1; applicable Falcon governance authority  

## Purpose

This Contract separates observed health from assessed Fitness to Operate and clarifies that fitness is scoped, evidenced, and authority-neutral.

This version incorporates the Stage 7 Gate 0B additions without granting Guardian, Lifecycle, Recovery, or Authority Engine powers to Health or Fitness.

## Health Assessment

Every health assessment SHALL contain:

- assessment ID;
- subject identity;
- health state;
- assessment and evidence times;
- evidence references;
- rule version;
- confidence or evidence-quality reference;
- affected capability; and
- known contradictions.

Allowed health states are `HEALTHY`, `DEGRADED`, `UNHEALTHY`, `UNKNOWN`, and `NOT_APPLICABLE`.

Health evidence quality and Health consequence classes are governed by SYS-008. This Contract consumes those semantics for scoped Fitness projection only.

## Fitness Assessment

Every fitness assessment SHALL contain:

- assessment ID;
- subject and capability;
- requested authority level;
- `FIT`, `RESTRICTED`, or `NOT_FIT`;
- scope;
- evidence and Self Model reference;
- confidence and unknowns;
- constraints;
- reason;
- effective time; and
- expiry.

Fitness SHALL remain scoped to the exact subject/capability/action context being evaluated.

## Fitness Mapping

Contract fitness results SHALL map from Foundation Self-Awareness technical states as follows:

| AWR-001 technical state | CON-006 fitness result | Notes |
|---|---|---|
| `FIT` | `FIT` | no blocking constraints |
| `FIT_WITH_CONSTRAINTS` | `RESTRICTED` | usable only within listed constraints |
| `DEGRADED` | `RESTRICTED` | degraded but not absent |
| `UNKNOWN` | `NOT_FIT` | insufficient evidence for reliance |
| `UNAVAILABLE` | `NOT_FIT` | required evidence absent or inaccessible |
| `INTEGRITY_FAILURE` | `NOT_FIT` | trust failure |
| `ISOLATION_REQUIRED` | `RESTRICTED` | may require containment before use; this Contract does not perform containment |
| `RECOVERY_REQUIRED` | `NOT_FIT` by default; `RESTRICTED` only under the bounded exception below | recovery-gated |
| `NOT_FIT` | `NOT_FIT` | unsupported for requested scope |

### `RECOVERY_REQUIRED` Bounded Exception

The default mapping is:

```text
RECOVERY_REQUIRED -> NOT_FIT
```

`RECOVERY_REQUIRED -> RESTRICTED` is permitted only when ALL of the following are true:

1. the recovery-required fault is technically isolated to a declared scope;
2. the requested capability does not depend on the affected subject/path for the restricted operating mode;
3. fresh independent evidence proves the unaffected capability remains technically usable;
4. no unresolved integrity, provenance, identity, authority/trust, or cross-scope contamination affects that capability;
5. the governing Health/Fitness rule explicitly predeclares the permitted restricted mode and its exact constraints;
6. the `RESTRICTED` result has an expiry and requires reassessment;
7. source reappearance alone is not treated as restoration to `FIT`; and
8. any required recovery acceptance or release remains owned by the separately governed Recovery/Release path.

If any required condition is absent, failed, or unknown:

```text
RECOVERY_REQUIRED -> NOT_FIT
```

## Evidence-Quality Effect on Fitness

SYS-008 defines Health evidence-quality classes. Their contractual effect is:

- `EQ-SUFFICIENT` may support positive Fitness when all other Fitness conditions are satisfied;
- `EQ-LIMITED` may support only a bounded result consistent with declared constraints and SHALL NOT by itself justify unrestricted `FIT` where the missing/limited relation is required for unrestricted reliance;
- `EQ-INSUFFICIENT` SHALL prevent `FIT` for the affected scope;
- `EQ-INVALID` SHALL be excluded from positive Fitness inference.

A Health state of `HEALTHY` SHALL NOT override an independent Fitness blocker, authority limitation, recovery gate, or missing capability-specific requirement.

## Health Consequence Consumption

SYS-008 Health consequence classes are technical interpretation inputs only.

- `HC-OBSERVATION_ONLY` does not by itself reduce Fitness, but it cannot be used for a required/fail-closed/trust-critical condition.
- `HC-DEGRADING` may support `RESTRICTED` only with explicit scoped constraints.
- `HC-CAPABILITY_BLOCKING` SHALL prevent `FIT` for the affected capability.
- `HC-TRUST_BLOCKING` SHALL prevent `FIT` for the affected capability.
- `HC-RECOVERY_GATED` SHALL use the `RECOVERY_REQUIRED` mapping in this Contract.

No Health consequence class or Fitness result grants protective command authority.

## Obligations

- **CON-006-REQ-001:** Missing or stale required evidence SHALL NOT produce `HEALTHY`.
- **CON-006-REQ-002:** `HEALTHY` SHALL NOT imply fitness for every authority.
- **CON-006-REQ-003:** Fitness SHALL be scoped to capability and action level.
- **CON-006-REQ-004:** Unknown required evidence SHALL prevent `FIT`.
- **CON-006-REQ-005:** Contradictions SHALL remain explicit.
- **CON-006-REQ-006:** Fitness SHALL expire when its evidence or governing conditions expire.
- **CON-006-REQ-007:** Fitness result SHALL NOT grant authority.
- **CON-006-REQ-008:** Material reduction SHALL be available to Authority Engine and Guardian as qualified evidence; this Contract SHALL NOT issue Guardian or Authority commands.
- **CON-006-REQ-009:** AWR-001 technical fitness states and CON-006 fitness results SHALL remain explicitly mappable through the table in this Contract.
- **CON-006-REQ-010:** `RECOVERY_REQUIRED` SHALL map to `NOT_FIT` unless every bounded `RESTRICTED` exception condition in this Contract is satisfied.
- **CON-006-REQ-011:** A `RESTRICTED` result produced under `RECOVERY_REQUIRED` SHALL be expiring, capability-scoped, independently evidenced, and shall not imply recovery acceptance or release.
- **CON-006-REQ-012:** Health/Fitness positive inference SHALL NOT rely on a circular evidence chain that transitively depends on the result being produced.
- **CON-006-REQ-013:** FSA and Health Monitoring self-produced outputs SHALL NOT be the sole required positive evidence of their own technical Health where SYS-008 requires independent evidence.

## Acceptance

Acceptance and later implementation verification require healthy-but-not-fit, degraded-restricted, stale-unknown, contradictory, expired, and recovered examples, plus:

- `RECOVERY_REQUIRED -> NOT_FIT` default behavior;
- permitted and denied `RECOVERY_REQUIRED -> RESTRICTED` examples;
- circular positive-proof rejection;
- FSA technical Health evidence that does not self-certify; and
- proof that Health/Fitness outputs do not exercise Guardian, Lifecycle, Recovery, or Authority Engine powers.

## Runtime Synchronization Note

This documentary addition does not itself modify runtime source code. Runtime implementations SHALL NOT claim CON-006 v1.2 conformance until their separately governed implementation and verification are completed.

## Change History

| Version | Date | Change |
|---|---|---|
| 1.0 | Preserved historical version | Initial Contract version. |
| 1.1 | 2026-07-31 | Activated Health/Fitness contract semantics and AWR-001 technical-state mapping. |
| 1.2 | 2026-08-12 | Added the Owner-approved Stage 7 Gate 0B Fitness projection rules, evidence-quality effects, Health consequence consumption, circular-proof prohibition, and deterministic `RECOVERY_REQUIRED` default/exception mapping. |
