# CON-009 — Security Context Contract

**Identifier:** CON-009  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Security Authority  
**Governing Specifications:** SEC-001, AUT-001

## Purpose

This Contract defines the trusted security context carried into governed Core decisions.

## Security Context

Every context SHALL contain:

- context ID;
- authenticated subject identity;
- authentication method and assurance;
- session or workload identity;
- trust boundary;
- delegated authority references;
- security classification;
- integrity state;
- issue and expiry time;
- revocation status; and
- provenance.

## Obligations

- **CON-009-REQ-001:** Authentication SHALL remain distinct from authorization.
- **CON-009-REQ-002:** Missing or unverifiable identity SHALL produce an untrusted context.
- **CON-009-REQ-003:** Expired or revoked context SHALL NOT authorize action.
- **CON-009-REQ-004:** Context SHALL be bound to its intended subject and scope.
- **CON-009-REQ-005:** Security classification SHALL constrain data and communication handling.
- **CON-009-REQ-006:** Integrity failure SHALL invalidate affected trust.
- **CON-009-REQ-007:** A context SHALL NOT claim authority beyond referenced delegation.
- **CON-009-REQ-008:** Trust restoration SHALL create a new attributable context.

## Acceptance

Acceptance requires valid, expired, revoked, replayed, wrong-subject, integrity-failed, insufficient-assurance, and restored-trust examples.
