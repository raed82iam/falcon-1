# CON-006 — Health and Fitness Contract

**Identifier:** CON-006  
**Version:** 1.1  
**Status:** Proposed  
**Canonical Target:** `docs/contracts/CON-006_HEALTH_AND_FITNESS.md`  
**Approval Record:** Pending  
**Governing Authority:** GOV-063; AWR-001 v2.1; SYS-008  
**Activation Authority:** Not Granted

## Purpose

This Contract separates observed health from assessed Fitness to Operate and clarifies that fitness is scoped, evidenced, and authority-neutral.

## Health Assessment

Every health assessment SHALL contain:

- assessment ID;
- subject identity;
- health state;
- assessment and evidence times;
- evidence references;
- rule version;
- confidence;
- affected capability; and
- known contradictions.

Allowed health states are `HEALTHY`, `DEGRADED`, `UNHEALTHY`, `UNKNOWN`, and `NOT_APPLICABLE`.

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
| `ISOLATION_REQUIRED` | `RESTRICTED` | may require containment before use |
| `RECOVERY_REQUIRED` | `RESTRICTED` or `NOT_FIT` | consequence-dependent |
| `NOT_FIT` | `NOT_FIT` | unsupported for requested scope |

## Obligations

- **CON-006-REQ-001:** Missing or stale required evidence SHALL NOT produce `HEALTHY`.
- **CON-006-REQ-002:** `HEALTHY` SHALL NOT imply fitness for every authority.
- **CON-006-REQ-003:** Fitness SHALL be scoped to capability and action level.
- **CON-006-REQ-004:** Unknown required evidence SHALL prevent `FIT`.
- **CON-006-REQ-005:** Contradictions SHALL remain explicit.
- **CON-006-REQ-006:** Fitness SHALL expire when its evidence or governing conditions expire.
- **CON-006-REQ-007:** Fitness result SHALL NOT grant authority.
- **CON-006-REQ-008:** Material reduction SHALL be available to Authority Engine and Guardian.
- **CON-006-REQ-009:** AWR-001 technical fitness states and CON-006 fitness results SHALL remain explicitly mappable through the table in this Contract.

## Acceptance

Acceptance requires healthy-but-not-fit, degraded-restricted, stale-unknown, contradictory, expired, and recovered examples.
