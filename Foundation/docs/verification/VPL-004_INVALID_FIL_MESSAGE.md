# VPL-004 — Invalid FIL Message Verification Plan

**Identifier:** VPL-004  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-010  
**Scenario:** FRS-SCN-004  
**Owner:** Falcon Verification Authority  
**Governing Sources:** FRS-001; SYS-005; SYS-009; SYS-010; CON-004; CON-005; ADR-F003; ADR-F004
**Master Plan:** VPL-000  
**Supersedes:** None  
**Superseded By:** None

## 1. Verification Objective

Prove that FIL and the Service Bus preserve canonical meaning and reject malformed, unsupported, expired, integrity-failed, replay-prohibited, or unauthorized messages before governed action.

## Scope and Non-Scope

This plan verifies the Foundation JSON FIL representation, released schemas, and initial non-distributed communication path. It does not verify production scale, distributed delivery, high availability, or financial message types.

Result vocabulary and global evidence rules are governed by VPL-000.

## 2. Required Setup

- released schemas for every FIL message kind;
- one valid Command, Query, Response, Event, and Notice;
- controlled invalid encodings and schema variants;
- authenticated authorized and unauthorized producers; and
- an observed destination whose handling and state can be independently inspected.

## 3. Procedure

1. Send each valid message kind and confirm declared handling.
2. Submit malformed UTF-8 and malformed JSON.
3. Submit duplicate member names, missing required fields, invalid types, and unknown required meaning.
4. Submit unsupported schema versions.
5. Submit expired and integrity-failed messages.
6. Submit an unauthorized producer, destination, topic, and event-fact assertion.
7. Replay a message where replay is prohibited and duplicate one where deduplication is required.
8. Make one message undeliverable and observe containment.
9. Confirm that admission, delivery, acceptance, execution, persistence, and outcome remain distinct.
10. Attempt interception, endpoint impersonation, envelope and ciphertext alteration, wrong-recipient use, downgrade, plaintext fallback, expired or revoked key use, stale revocation, and prohibited replay.
11. Confirm sensitive plaintext is absent from logs, errors, evidence, dead-letter storage, and observable transport.
12. Confirm cryptographic-service loss restricts affected authority and retained encrypted evidence restores only with authorized valid key context.

## 4. Expected Results

- Valid messages preserve identity, producer, schema, correlation, causation, classification, and payload integrity.
- Every invalid variant is rejected at the correct stage with an explicit reason.
- No rejected message reaches governed execution or changes authoritative state.
- An undeliverable message does not disappear silently.
- Transport never becomes the authority for message meaning or action.

## 5. Required Evidence

Original and canonical message identities, released schema identities, validation-stage results, authorization decisions, routing and delivery records, rejection or quarantine records, destination observations, authoritative state comparison, and integrity evidence.

## 6. Pass Rule

`PASS` requires all valid controls to preserve normative meaning, all invalid variants to be rejected before governed action, and every outcome to be reconstructable. Silent loss, payload mutation, unauthorized fact publication, or rejected-message execution is an immediate `FAIL`.

## 7. Independent Verification

The Independent Verifier shall validate preserved message samples against the released schemas and inspect the destination state independently of Service Bus delivery claims.

## 8. Containment, Cleanup, and Repeatability

Messages shall target an isolated harmless destination. Quarantined and undeliverable messages shall be retained as evidence and then removed through governed cleanup. Repetition shall use fresh delivery-attempt identities while preserving logical replay identities where required.

## 9. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-007 | 2026-07-24 |
