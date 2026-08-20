# CON-002 — Authority Decision Contract

**Version:** 1.1  
**Status:** Approved  
**Approval Record:** GOV-063  
**Documentary Activation:** Active  
**Activation Record:** GOV-092; GOV-093; GOV-094  
**Effective Documentary Instant:** 2026-07-31 22:54:57 +03:00  
**Supersedes:** CON-002 v1.0  
**Identifier:** CON-002  
**Canonical Target:** `docs/contracts/CON-002_AUTHORITY_DECISION.md`  
**Governing Authority:** GOV-063; GOV-AUT-001; AWR-001 v2.1  

## Purpose

This Contract defines the request and result of a governed authority evaluation and clarifies the separation between authority decision, fitness evidence, and execution.

## Request

An Authority Request SHALL contain:

- request ID;
- authenticated actor identity;
- action;
- resource;
- purpose;
- requested scope;
- operating and security context;
- required Fitness to Operate;
- correlation and causation;
- request time; and
- expiry.

## Result

An Authority Result SHALL contain:

- request ID;
- decision ID;
- `ALLOW` or `DENY`;
- effective scope;
- controlling policy and version;
- material conditions;
- constraints;
- reason;
- decision time;
- expiry; and
- evidence reference.

## Obligations

- **CON-002-REQ-001:** Missing trustworthy actor identity SHALL produce `DENY`.
- **CON-002-REQ-002:** Missing authority provenance SHALL produce `DENY`.
- **CON-002-REQ-003:** Insufficient Fitness to Operate SHALL produce `DENY`.
- **CON-002-REQ-004:** The granted scope SHALL NOT exceed the requested or authorized scope.
- **CON-002-REQ-005:** An expired result SHALL NOT authorize action.
- **CON-002-REQ-006:** The same trusted inputs and policy baseline SHALL produce the same result.
- **CON-002-REQ-007:** A result SHALL NOT execute the action.
- **CON-002-REQ-008:** Revocation SHALL invalidate affected unexecuted authority.

## Acceptance

Acceptance requires allow, deny, expiry, revocation, insufficient-fitness, conflicting-policy, and reconstruction examples.
