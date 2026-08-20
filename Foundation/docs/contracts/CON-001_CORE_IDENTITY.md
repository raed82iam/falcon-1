# CON-001 — Core Identity Contract

**Identifier:** CON-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Core Authority  
**Governing Specifications:** SYS-001, SEC-001

## Purpose

This Contract defines the identity evidence required for a Falcon instance and every Core component.

## Identity Record

Every identity SHALL contain:

- stable subject ID;
- subject class;
- instance ID;
- version;
- owner;
- admitted capability;
- artifact identity;
- authority context;
- lifecycle identity;
- creation time; and
- integrity evidence.

## Obligations

- **CON-001-REQ-001:** Identity SHALL be established before Core admission.
- **CON-001-REQ-002:** Display name SHALL NOT serve as authoritative identity.
- **CON-001-REQ-003:** Instance identity SHALL remain distinct from component-type identity.
- **CON-001-REQ-004:** Identity SHALL be bound to the admitted artifact or approved subject.
- **CON-001-REQ-005:** Duplicate active instance identity SHALL be rejected.
- **CON-001-REQ-006:** Unknown or unverifiable identity SHALL prevent unrestricted operation.
- **CON-001-REQ-007:** Identity change SHALL be represented as a governed transition, not silent mutation.
- **CON-001-REQ-008:** Retirement SHALL preserve historical attribution.

## Rejections

The Contract SHALL reject missing, duplicate, expired where applicable, integrity-failed, ownerless, or unauthorized identity.

## Acceptance

Acceptance requires examples for valid admission, duplicate rejection, artifact mismatch, identity retirement, and historical reconstruction.
