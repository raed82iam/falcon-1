# CON-008 — Evidence and Logging Contract

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005  
**Superseded By:** CON-008 v1.1 under GOV-030  
**Owner:** Falcon Evidence Authority  
**Governing Specifications:** OPS-004, DEC-006, SEC-001

## Purpose

This Contract defines the minimum structured evidence record required for Foundation reconstruction.

## Evidence Record

Every record SHALL contain:

- record ID;
- record class;
- source identity;
- source and observation time;
- severity or consequence;
- correlation and causation;
- subject;
- structured fact;
- authority reference where applicable;
- security classification;
- integrity evidence; and
- retention class.

## Obligations

- **CON-008-REQ-001:** Audit-critical actions SHALL produce attributable evidence.
- **CON-008-REQ-002:** Acceptance SHALL protect the record against undetected mutation.
- **CON-008-REQ-003:** Secrets and prohibited sensitive data SHALL NOT enter ordinary evidence.
- **CON-008-REQ-004:** Missing audit-critical evidence SHALL be observable.
- **CON-008-REQ-005:** Logging success SHALL distinguish accepted, persisted, and externally retained states.
- **CON-008-REQ-006:** Correction SHALL append rather than rewrite.
- **CON-008-REQ-007:** Clock-quality limitations SHALL be preserved.
- **CON-008-REQ-008:** Authorized reconstruction SHALL preserve confidentiality.

## Acceptance

Acceptance requires complete reconstruction of FRS-001 scenarios, mutation detection, redaction, evidence-loss signaling, correction, and access-control examples.
