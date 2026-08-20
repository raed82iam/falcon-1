# AMD-001 — FIL Cryptographic Protection Amendment Package

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-010  
**Owner:** Falcon Security Authority  
**Governing Authority:** SEC-PLAN-001; GOV-009  
**Target Versions:** SEC-001 v1.1; SYS-005 v1.1; SYS-009 v1.1; CON-004 v1.1; FDN-002 v1.1; FIL-001 v1.1; VPL-004 v1.1; IMP-001 v1.1  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This package defines the exact semantic changes required to establish cryptographic protection as a Foundation obligation before implementation.

Approval authorizes creation of the listed version 1.1 canonical documents, preservation of their version 1.0 predecessors, and activation of the amended baseline. It does not select algorithms or authorize code.

## 2. Compatibility Decision

The amendment is backward-compatible with the Vision and Constitution but intentionally incompatible with any implementation that accepts unprotected cross-boundary FIL communication.

Version 1.0 documents remain historical authority for their original effective period. Version 1.1 shall supersede them prospectively.

Unknown or legacy protection metadata SHALL NOT be interpreted permissively. Migration requires an explicit bounded compatibility profile; plaintext fallback is prohibited.

## 3. Candidate A — SEC-001 v1.1

Add the following normative requirements:

- **SEC-001-REQ-019:** Every material communication crossing a security boundary SHALL use mutually authenticated encrypted transport under an approved cryptographic profile.
- **SEC-001-REQ-020:** Sensitive content SHALL use authorized message-level authenticated encryption when transport protection alone does not preserve its required confidentiality boundary.
- **SEC-001-REQ-021:** Sensitive persisted data, messages, evidence, backups, and recovery artifacts SHALL be encrypted at rest under governed key management.
- **SEC-001-REQ-022:** Cryptographic protection SHALL bind identity, intended recipient or scope, classification, and material routing context to protected content where substitution could cause harm.
- **SEC-001-REQ-023:** Every cryptographic key class SHALL have an accountable owner and governed generation, custody, access, distribution, activation, rotation, revocation, recovery, retirement, and destruction lifecycle.
- **SEC-001-REQ-024:** Secret or private key material SHALL NOT appear in source code, ordinary configuration, messages, logs, verification evidence, or uncontrolled copies.
- **SEC-001-REQ-025:** Falcon SHALL reject unknown, prohibited, deprecated, downgraded, integrity-failed, wrong-recipient, expired, or revoked cryptographic contexts.
- **SEC-001-REQ-026:** Failure of required cryptographic protection SHALL deny or restrict affected action and SHALL NOT cause silent plaintext fallback.
- **SEC-001-REQ-027:** Cryptographic algorithms, protocols, parameters, and providers SHALL be selected through a governed replaceable profile and SHALL NOT redefine protected business meaning.
- **SEC-001-REQ-028:** Falcon SHALL NOT use custom cryptographic algorithms or protocols for governed protection.

Add acceptance evidence for transport interception, payload exposure, tampering, downgrade, wrong recipient, key compromise, rotation, revocation, secret leakage, encrypted restoration, and fail-closed cryptographic loss.

## 4. Candidate B — SYS-005 v1.1

Add:

- **SYS-005-REQ-016:** Every route crossing a declared security boundary SHALL enforce the minimum SEC-PLAN-001 protection profile before message admission.
- **SYS-005-REQ-017:** Transport endpoint identity SHALL be mutually verified and bound to the admitted producer, consumer, or explicitly authorized intermediary.
- **SYS-005-REQ-018:** The Service Bus SHALL preserve original message protection metadata and SHALL NOT weaken, remove, terminate, or replace end-to-end payload protection without explicit authority.
- **SYS-005-REQ-019:** Protected routing metadata and encrypted payload SHALL remain cryptographically bound where alteration could redirect, reclassify, reprioritize, or substitute the message.
- **SYS-005-REQ-020:** Unknown protection profile, failed channel authentication, downgrade, invalid integrity, wrong recipient, stale revocation status, or prohibited replay SHALL cause explicit rejection.
- **SYS-005-REQ-021:** Required encryption failure SHALL NOT trigger plaintext delivery, insecure retry, or permissive cached transport authority.
- **SYS-005-REQ-022:** Transport evidence SHALL record protection profile, endpoint identities, key references, and validation results without exposing keys or sensitive plaintext.

## 5. Candidate C — SYS-009 v1.1

Resolve the name:

> **FIL means Falcon Interaction Language.**

Add:

- **SYS-009-REQ-016:** Every FIL message SHALL declare a protection-profile ID and version, integrity scope, authorized key reference, and temporal-validity or replay policy.
- **SYS-009-REQ-017:** Every accepted FIL message SHALL have a verified original producer identity and integrity result.
- **SYS-009-REQ-018:** A Command and Query SHALL have explicit expiry; a Notice SHALL have expiry when its information can become stale.
- **SYS-009-REQ-019:** A Response SHALL remain bound to the request’s identity and validity policy.
- **SYS-009-REQ-020:** Event occurrence, delivery validity, and replay status SHALL remain distinct; replay SHALL NOT recreate action authority.
- **SYS-009-REQ-021:** Sensitive payload protection SHALL bind message ID, kind, type, schema, producer, recipient or topic, purpose, classification, time policy, correlation, causation, and priority authority.
- **SYS-009-REQ-022:** Original producer identity SHALL survive authorized transport and cryptographic intermediaries.
- **SYS-009-REQ-023:** Unknown, downgraded, expired, wrong-recipient, replay-prohibited, or integrity-failed protection semantics SHALL produce explicit rejection.

Remove the unresolved naming matter and add cryptographic-profile evolution to compatibility evidence.

## 6. Candidate D — CON-004 v1.1

Add required envelope fields:

- protection-profile ID and version;
- integrity method reference and protected-field scope;
- encryption state and encrypted-field scope;
- authorized key reference and key version;
- intended recipient or recipient-group binding when encrypted;
- replay-policy ID;
- delivery-attempt ID separate from logical message ID;
- nonce or equivalent profile input when required; and
- cryptographic-context issue and expiry where distinct from message expiry.

Add obligations:

- **CON-004-REQ-011:** Key references SHALL NOT contain secret key material.
- **CON-004-REQ-012:** P3 and P4 payload protection SHALL cryptographically bind the required visible envelope fields.
- **CON-004-REQ-013:** Protection validation SHALL remain distinct from authorization and execution.
- **CON-004-REQ-014:** Unknown, downgraded, wrong-recipient, nonce-invalid, replay-prohibited, or cryptographically invalid messages SHALL be rejected explicitly.
- **CON-004-REQ-015:** Transport intermediaries SHALL preserve original producer and protection evidence.

## 7. Candidate E — FDN-002 v1.1

Assign minimum profiles:

| Interaction class | Minimum profile |
|---|---|
| interaction inside one protected boundary | FIL-P1 |
| every cataloged interaction crossing a process or equivalent security boundary | FIL-P2 |
| security context, revocation, Guardian restriction, recovery validation, authority details, or confidential evidence crossing an intermediary | FIL-P3 |
| retained confidential or restricted FIL payload and dead-letter evidence | FIL-P4 |

Add explicit producer-to-recipient protection, classification, replay, expiry, and key-purpose columns to the interaction catalog.

Replace example protection objects with v1.1 protection metadata and add valid encrypted-envelope examples without real secrets.

## 8. Candidate F — FIL-001 v1.1

The schema SHALL:

- require `protection_profile`, `protection_version`, `integrity_scope`, `key_ref`, and `replay_policy`;
- require `expires_at` for Command and Query;
- require request binding for Response;
- require occurrence and replay metadata for Event;
- require expiry for stale-sensitive Notice types;
- represent encrypted payload as protected content plus non-secret cryptographic metadata;
- reject unknown fields and unsupported profile versions;
- prohibit secret-key fields structurally; and
- preserve Draft 2020-12 and UTF-8 JSON compatibility.

The full candidate schema shall be generated from these approved semantics and validated against all FDN-002 examples before canonical activation.

## 9. Candidate G — VPL-004 v1.1

Add mandatory cases:

- passive interception across a P2 boundary reveals no payload;
- endpoint impersonation fails mutual authentication;
- envelope or ciphertext alteration is detected;
- visible-envelope substitution breaks P3 validation;
- wrong-recipient decryption fails;
- replay, duplicate, delay, and expired message behavior matches kind policy;
- protection-profile downgrade and plaintext fallback are rejected;
- expired or revoked key context is rejected;
- stale revocation input fails closed;
- key rotation accepts only the declared overlap window;
- sensitive plaintext does not enter logs, evidence, dead-letter storage, or errors;
- unavailable cryptographic service restricts affected authority; and
- encrypted retained evidence restores only with authorized valid key context.

Independent verification SHALL inspect both endpoints, observable transport, retained evidence, and absence of plaintext.

## 10. Candidate H — IMP-001 v1.1

Add to Stage 0:

- cryptographic threat model;
- FIL protection-profile ADR;
- algorithm and protocol profile;
- key and secret provider decision;
- certificate or workload-identity realization;
- rotation, revocation, overlap, and compromise procedure;
- encrypted persistence and backup-recovery decision; and
- cryptographic dependency provenance and update policy.

Add a new implementation gate before Contract primitives:

> No FIL transport, message persistence, evidence persistence, or secret-handling implementation may begin until the cryptographic profile ADR and security design are Accepted.

Require VPL-004 v1.1 and affected security cases to pass before Stage 5 completion.

## 11. Cross-Document Invariants

1. Encryption never grants authority.
2. Authentication remains distinct from authorization.
3. Cryptographic integrity does not prove factual truth.
4. No required protection silently falls back to plaintext.
5. No secret enters source, ordinary configuration, messages, logs, or evidence.
6. Original producer identity survives intermediaries.
7. Event replay does not recreate action authority.
8. Algorithm replacement does not change FIL business meaning.
9. Unknown required protection fails closed.
10. The Foundation remains non-financial.

## 12. Review and Activation

Before approval, review SHALL confirm:

- compatibility with Vision, Constitution, SEC-001, and ADR-F004;
- no hidden algorithm choice;
- exact requirement and field ownership;
- schema realizability;
- migration without permissive fallback;
- complete negative and abuse coverage;
- key-custody separation; and
- no financial or live-capital path.

Upon approval:

1. preserve canonical version 1.0 files as immutable history;
2. create canonical version 1.1 documents with GOV approval metadata;
3. generate and validate FIL-001 v1.1;
4. update registries and readiness report;
5. record prospective supersession; and
6. keep implementation unauthorized until Stage 0 decisions and explicit Project Owner authorization.

## 13. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-010 | 2026-07-24 |
