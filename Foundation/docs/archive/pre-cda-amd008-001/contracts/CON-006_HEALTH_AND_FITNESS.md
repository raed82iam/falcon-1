# CON-006 — Health and Fitness Contract

**Identifier:** CON-006  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Self-Awareness Authority  
**Governing Specifications:** SYS-008, AWR-001

## Purpose

This Contract separates observed health from assessed Fitness to Operate.

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

## Obligations

- **CON-006-REQ-001:** Missing or stale required evidence SHALL NOT produce `HEALTHY`.
- **CON-006-REQ-002:** `HEALTHY` SHALL NOT imply fitness for every authority.
- **CON-006-REQ-003:** Fitness SHALL be scoped to capability and action level.
- **CON-006-REQ-004:** Unknown required evidence SHALL prevent `FIT`.
- **CON-006-REQ-005:** Contradictions SHALL remain explicit.
- **CON-006-REQ-006:** Fitness SHALL expire when its evidence or governing conditions expire.
- **CON-006-REQ-007:** Fitness result SHALL NOT grant authority.
- **CON-006-REQ-008:** Material reduction SHALL be available to Authority Engine and Guardian.

## Acceptance

Acceptance requires healthy-but-not-fit, degraded-restricted, stale-unknown, contradictory, expired, and recovered examples.
