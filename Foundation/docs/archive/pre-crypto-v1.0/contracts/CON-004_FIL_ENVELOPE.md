# CON-004 — FIL Envelope Contract

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Owner:** Falcon Communication Authority  
**Governing Specifications:** SYS-009, SEC-001

## Purpose

This Contract defines the canonical semantic fields shared by all FIL messages.

## Required Envelope

Every FIL message SHALL contain:

- message ID;
- message kind;
- message type;
- schema ID and version;
- producer identity;
- creation time;
- purpose;
- security classification;
- correlation ID when applicable;
- causation ID when applicable;
- expiry when applicable;
- priority authority;
- integrity evidence; and
- payload.

## Obligations

- **CON-004-REQ-001:** Message ID SHALL be globally unique within the governed identity scope.
- **CON-004-REQ-002:** Message kind SHALL be one of Command, Query, Response, Event, or Notice.
- **CON-004-REQ-003:** A Command SHALL identify target and authority context.
- **CON-004-REQ-004:** A Response SHALL identify its request.
- **CON-004-REQ-005:** An Event SHALL identify its authoritative fact owner.
- **CON-004-REQ-006:** Correlation SHALL NOT replace causation.
- **CON-004-REQ-007:** Expired messages SHALL be rejected before governed action.
- **CON-004-REQ-008:** Envelope validity SHALL NOT imply authorization or payload validity.
- **CON-004-REQ-009:** Transport SHALL preserve envelope and payload integrity.
- **CON-004-REQ-010:** Unsupported required schema versions SHALL be rejected explicitly.

## Acceptance

Acceptance requires valid examples for every message kind and rejection examples for malformed, expired, unsupported, integrity-failed, and falsely authorized messages.
