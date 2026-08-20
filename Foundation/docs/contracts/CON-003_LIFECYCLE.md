# CON-003 — Lifecycle Contract

**Identifier:** CON-003  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Core Authority  
**Governing Specifications:** SYS-002, AUT-002, OPS-003

## Purpose

This Contract defines requests, decisions, and facts for governed component lifecycle transitions.

## Transition Request

Every request SHALL contain:

- transition request ID;
- component identity;
- authoritative source state;
- requested target state;
- requester;
- authority reference;
- reason;
- dependency context;
- request time; and
- expiry.

## Transition Result

Every result SHALL contain:

- request ID;
- transition ID;
- `ACCEPTED`, `REJECTED`, or `FAILED`;
- source and target state;
- actual resulting state;
- reason;
- validation evidence;
- completion time; and
- emitted event reference.

## Obligations

- **CON-003-REQ-001:** Source state SHALL match authoritative lifecycle state.
- **CON-003-REQ-002:** Invalid transitions SHALL be rejected without state mutation.
- **CON-003-REQ-003:** Missing authority SHALL produce rejection.
- **CON-003-REQ-004:** Accepted request SHALL NOT be represented as completed transition.
- **CON-003-REQ-005:** Failed transition SHALL expose the actual resulting state.
- **CON-003-REQ-006:** Guardian restriction SHALL remain distinguishable from routine transition.
- **CON-003-REQ-007:** Recovery completion SHALL require independent validation evidence.
- **CON-003-REQ-008:** Every completed transition SHALL produce one authoritative event.

## Acceptance

Acceptance requires valid, invalid, stale-source, unauthorized, interrupted, Guardian-imposed, and recovery transitions.
