# CON-005 — Event Contract

**Identifier:** CON-005  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Communication Authority  
**Governing Specifications:** SYS-010, SYS-009

## Purpose

This Contract defines the evidence required for a FIL message to assert an immutable governed event.

## Event Record

Every event SHALL contain:

- event ID;
- event type and schema version;
- authoritative fact owner;
- subject identity;
- occurrence time;
- publication time;
- source evidence;
- correlation and causation;
- replay indicator;
- correction relationship where applicable; and
- payload.

## Obligations

- **CON-005-REQ-001:** An event SHALL describe an occurrence or established fact, not request action.
- **CON-005-REQ-002:** Publication SHALL require authority to assert the fact.
- **CON-005-REQ-003:** Accepted event meaning SHALL be immutable.
- **CON-005-REQ-004:** Correction SHALL use a new related event.
- **CON-005-REQ-005:** Replay SHALL remain distinguishable from live publication.
- **CON-005-REQ-006:** Replay SHALL NOT recreate irreversible action without new authority.
- **CON-005-REQ-007:** Ordering claims SHALL state their exact scope.
- **CON-005-REQ-008:** Derived events SHALL preserve source provenance.

## Acceptance

Acceptance requires original, duplicate, replayed, corrected, derived, unauthorized, and unsupported-version examples.
