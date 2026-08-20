# SYS-009 — FIL

**Identifier:** SYS-009  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-010
**Owner:** Falcon Communication Authority  
**Governing Authority:** Constitution Articles 13, 17, 26–27, 31–33  
**Affected Domains:** All communicating domains

## 1. Purpose

FIL means **Falcon Interaction Language**.

FIL is Falcon’s canonical message and interaction contract.

It provides a stable language for identifying messages, expressing intent, preserving provenance, relating cause to effect, and validating compatibility across governed boundaries.

## 2. Scope

SYS-009 governs:

- message envelope;
- message identity and type;
- schema identity and version;
- producer, destination, topic, and purpose;
- correlation and causation;
- time, expiry, priority, and classification metadata;
- command, query, response, and event semantics;
- compatibility rules; and
- validation and evolution of FIL contracts.

## 3. Non-Scope

FIL does not:

- transport messages;
- authorize an action by its presence;
- define business policy;
- guarantee delivery;
- interpret every payload centrally;
- execute commands;
- store operational state; or
- replace domain Specifications.

## 4. Message Kinds

FIL SHALL distinguish at minimum:

- **Command:** a request for an authorized actor to attempt an action;
- **Query:** a request for information without authority to change governed state;
- **Response:** the result of a command or query;
- **Event:** an immutable statement that a governed fact occurred; and
- **Notice:** non-authoritative information that does not claim event truth or command authority.

## 5. Normative Requirements

- **SYS-009-REQ-001:** Every FIL message SHALL have a globally unique message identity.
- **SYS-009-REQ-002:** Every FIL message SHALL declare message kind, type, schema version, producer identity, creation time, and security classification.
- **SYS-009-REQ-003:** Every command SHALL declare intended authority context, target, purpose, and expiry.
- **SYS-009-REQ-004:** Every response SHALL identify the request to which it responds.
- **SYS-009-REQ-005:** Every event SHALL identify its authoritative fact owner and causal source where known.
- **SYS-009-REQ-006:** Correlation and causation SHALL remain distinct.
- **SYS-009-REQ-007:** Message priority SHALL be constrained by approved policy and SHALL NOT establish authority.
- **SYS-009-REQ-008:** FIL validation SHALL distinguish structural validity, schema validity, authorization, and domain validity.
- **SYS-009-REQ-009:** Structural validity SHALL NOT be represented as authorization or successful execution.
- **SYS-009-REQ-010:** Payload schemas SHALL have one accountable owner and one canonical definition.
- **SYS-009-REQ-011:** Schema evolution SHALL declare compatibility and transition consequences.
- **SYS-009-REQ-012:** Unknown required fields, unsupported versions, invalid signatures, and integrity failures SHALL produce explicit rejection.
- **SYS-009-REQ-013:** Sensitive fields SHALL carry handling requirements sufficient for SEC-001 enforcement.
- **SYS-009-REQ-014:** FIL messages SHALL be serializable without loss of normative meaning.
- **SYS-009-REQ-015:** A message SHALL remain attributable to its original producer; transport intermediaries SHALL NOT impersonate that producer.
- **SYS-009-REQ-016:** Every FIL message SHALL declare protection-profile ID and version, integrity scope, authorized key reference, and temporal-validity or replay policy.
- **SYS-009-REQ-017:** Every accepted FIL message SHALL have verified original-producer identity and integrity.
- **SYS-009-REQ-018:** Commands and Queries SHALL expire explicitly; stale-sensitive Notices SHALL also expire.
- **SYS-009-REQ-019:** Responses SHALL remain bound to request identity and validity.
- **SYS-009-REQ-020:** Event occurrence, delivery validity, and replay status SHALL remain distinct; replay SHALL NOT recreate action authority.
- **SYS-009-REQ-021:** Sensitive payload protection SHALL bind material envelope identity, routing, purpose, classification, time, causation, and priority fields.
- **SYS-009-REQ-022:** Original producer identity SHALL survive authorized transport and cryptographic intermediaries.
- **SYS-009-REQ-023:** Unknown, downgraded, expired, wrong-recipient, replay-prohibited, or integrity-failed protection SHALL produce explicit rejection.

## 6. Invariants

1. A message describes or requests; it does not grant authority.
2. An event records a fact; it is not a command.
3. A retry preserves logical request identity while allowing distinct delivery-attempt identity.
4. Contract versions remain explicit.

## 7. Acceptance Evidence

Approval requires:

- canonical envelope examples for every message kind;
- schema compatibility tests;
- rejection evidence for malformed and unsupported messages;
- preservation of identity, correlation, causation, and classification through round trips; and
- proof that message validation cannot be mistaken for authorization.

## 8. ADR Candidates

- Serialization format;
- schema-definition technology;
- signing and integrity representation; and
- compatibility negotiation mechanism.

## 9. Unresolved Matters

- Canonical time representation and clock-quality metadata.
