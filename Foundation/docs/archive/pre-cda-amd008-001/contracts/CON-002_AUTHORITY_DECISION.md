# CON-002 — Authority Decision Contract

**Identifier:** CON-002  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Governance Authority  
**Governing Specifications:** AUT-001, AWR-001, SEC-001

## Purpose

This Contract defines the request and result of a governed authority evaluation.

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
