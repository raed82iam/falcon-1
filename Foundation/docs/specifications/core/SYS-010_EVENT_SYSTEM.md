# SYS-010 — Event System

**Identifier:** SYS-010  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Communication Authority  
**Governing Authority:** Constitution Articles 13, 17, 20, 31–33  
**Affected Domains:** All

## 1. Purpose

The Event System governs the publication and consumption of facts that have occurred within Falcon’s governed domain.

It preserves the distinction between fact, intent, observation, and interpretation.

## 2. Scope

SYS-010 governs:

- event identity and ownership;
- event type and schema;
- fact-time and publication-time;
- event publication authority;
- ordering and deduplication expectations;
- subscriptions;
- replay;
- event evolution; and
- event evidence.

## 3. Non-Scope

The Event System does not:

- transport all events by itself;
- represent commands as events;
- authorize consumers to act;
- guarantee that an observed condition is true without an authoritative fact owner;
- own event-derived projections;
- mutate published facts; or
- replace Logging.

## 4. Normative Requirements

- **SYS-010-REQ-001:** Every event SHALL represent a completed occurrence or established fact, not a request for future action.
- **SYS-010-REQ-002:** Every event type SHALL have one authoritative owner and one canonical schema.
- **SYS-010-REQ-003:** Every event SHALL include unique identity, type, schema version, source, occurrence time, publication time, and provenance.
- **SYS-010-REQ-004:** Event publication SHALL require authority to assert the declared fact.
- **SYS-010-REQ-005:** Event acceptance SHALL NOT by itself authorize a consumer action.
- **SYS-010-REQ-006:** Published event meaning SHALL be immutable.
- **SYS-010-REQ-007:** Corrections SHALL be represented by new related events, not mutation of historical events.
- **SYS-010-REQ-008:** Ordering guarantees SHALL be explicit and limited to their declared scope.
- **SYS-010-REQ-009:** Consumers SHALL tolerate duplicate delivery where the delivery contract permits it.
- **SYS-010-REQ-010:** Replay SHALL be distinguishable from original live publication.
- **SYS-010-REQ-011:** Replayed events SHALL NOT recreate prohibited irreversible action without independent authorization and idempotency controls.
- **SYS-010-REQ-012:** Event schema evolution SHALL preserve compatibility or provide explicit migration.
- **SYS-010-REQ-013:** Unknown, unauthorized, corrupt, or unsupported events SHALL be rejected or quarantined with evidence.
- **SYS-010-REQ-014:** Event streams required for authoritative reconstruction SHALL define completeness and gap-detection behavior.
- **SYS-010-REQ-015:** Derived events SHALL preserve traceability to source events and transformation authority.

## 5. Invariants

1. Events state what occurred; commands request what may occur.
2. Publication authority belongs to the fact owner.
3. Historical meaning is immutable.
4. Replay does not grant renewed authority.

## 6. Acceptance Evidence

Approval requires evidence for:

- command/event distinction;
- unauthorized fact-publication rejection;
- immutable correction behavior;
- duplicate and replay safety;
- ordering-contract enforcement;
- gap detection where completeness is required; and
- provenance across derived events.

## 7. ADR Candidates

- Event storage and streaming model;
- delivery topology;
- partition and ordering strategy;
- replay mechanism; and
- projection architecture.

## 8. Unresolved Matters

- Authoritative event catalog.
- Event completeness classes.
