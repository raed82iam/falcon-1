# SEC-PLAN-001 — FIL Cryptographic Protection Plan

**Identifier:** SEC-PLAN-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-009  
**Owner:** Falcon Security Authority  
**Governing Authority:** Falcon Constitution Article 33; SEC-001; SYS-005; SYS-009; STD-007; ADR-F004; ADR-F006  
**Affected Documents:** SEC-001, SYS-005, SYS-009, CON-004, FDN-002, FIL-001, VPL-004, IMP-001  
**Implementation Authority:** Not Granted  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This plan establishes the controlled amendment and verification path for cryptographic protection of Falcon Interaction Language messages.

It translates the constitutional duties of confidentiality, integrity, authenticity, provenance, and authorized use into explicit FIL protection obligations without fixing permanent algorithms in the Constitution or Specifications.

## 2. FIL Name

The proposed amendment SHALL resolve the remaining SYS-009 naming matter as:

> **FIL means Falcon Interaction Language.**

This name describes Falcon’s canonical governed interaction language. It does not grant authority to a message or communication mechanism.

## 3. Governing Security Rule

The proposed normative rule is:

> Every FIL message SHALL carry verifiable producer identity, explicit authorization context where action authority is requested, integrity protection, freshness or temporal-validity controls, and replay policy appropriate to its message kind. Every FIL message crossing a security boundary SHALL use mutually authenticated encrypted transport. A payload classified as sensitive SHALL also use authorized message-level authenticated encryption when transport protection alone does not preserve the required confidentiality boundary. Sensitive persisted messages and evidence SHALL be encrypted at rest under governed key management.

No protection result SHALL be represented as authorization, execution, persistence, or successful outcome.

## 4. Required Distinctions

FIL protection SHALL distinguish:

- confidentiality from integrity;
- authentication from authorization;
- transport encryption from message-level encryption;
- encryption from digital signature or message authentication;
- message freshness from event fact-time;
- duplicate delivery from prohibited replay;
- key identity from secret key material;
- cryptographic integrity from factual truth;
- encrypted storage from authorized access; and
- algorithm strength from key-custody strength.

## 5. Protection Baseline for Every FIL Message

Every accepted FIL message SHALL have:

1. globally unique message identity;
2. authenticated original producer identity;
3. intended target or governed topic;
4. declared message kind and type;
5. schema identity and version;
6. creation time and clock-quality context;
7. temporal-validity or replay policy;
8. correlation and causation where applicable;
9. security classification;
10. protection-profile identity and version;
11. integrity scope and verification result;
12. authorized key reference without secret material;
13. anti-downgrade evidence;
14. rejection behavior; and
15. traceable security evidence.

An Event does not cease to be historically true when a delivery window expires. Event replay and action authority remain separately governed.

## 6. Protection Profiles

### FIL-P1 — Governed Local Protection

Applies to material interaction wholly inside one declared protected execution boundary.

Requires:

- verified producer identity;
- schema validation;
- authorization where action is requested;
- integrity protection appropriate to the boundary;
- freshness or replay control;
- classification handling; and
- attributable evidence.

P1 does not claim protection against full compromise of the shared execution boundary.

### FIL-P2 — Security-Boundary Transport Protection

Applies whenever a FIL message crosses a process, container, host, network, account, trust, or equivalent security boundary.

Requires all P1 controls plus:

- mutually authenticated encrypted transport;
- endpoint identity bound to the FIL producer or authorized intermediary;
- current approved protocol and cryptographic profile;
- downgrade prevention;
- certificate or credential expiry and revocation checks;
- traffic confidentiality and integrity;
- channel failure that fails closed for affected governed action; and
- no reliance on network location as trust.

### FIL-P3 — Sensitive Message-Level Protection

Applies when payload confidentiality must survive intermediaries, queues, evidence transport, or storage outside the authorized recipient boundary.

Requires all applicable P2 controls plus:

- authenticated encryption of the sensitive payload;
- explicit recipient or recipient-group binding;
- minimal visible routing envelope;
- cryptographic binding of visible envelope fields to the encrypted payload;
- unique nonce or equivalent misuse-resistant input under the selected profile;
- protection against substitution, truncation, and wrong-recipient use;
- explicit encryption-key reference and version; and
- decryption permitted only after identity, context, integrity, and classification checks.

### FIL-P4 — Sensitive Retained Protection

Applies to sensitive persisted messages, evidence, backups, dead-letter records, and recovery artifacts.

Requires all applicable P3 controls plus:

- encryption at rest;
- separation of data-encryption and key-encryption responsibilities where required;
- protected key custody outside ordinary configuration and data stores;
- retention-aware key availability;
- governed backup and restoration of required key material;
- verifiable destruction and crypto-erasure where authorized; and
- restoration tests proving both confidentiality and recoverability.

## 7. Classification Mapping

| Classification | Minimum transport | Message-level encryption | At-rest encryption |
|---|---|---|---|
| `PUBLIC` | P2 across a security boundary | Optional unless integrity boundary requires it | According to integrity and retention need |
| `INTERNAL` | P2 across a security boundary | Required when an intermediary is outside the permitted content scope | Required when retained outside the protected boundary |
| `CONFIDENTIAL` | P2 | P3 unless an approved threat model proves equivalent end-to-end confidentiality | P4 |
| `RESTRICTED` | P2 | P3 mandatory | P4 mandatory |

A lower classification SHALL NOT be selected to avoid required protection.

## 8. Message-Kind Temporal and Replay Rules

| Kind | Temporal rule | Replay rule |
|---|---|---|
| Command | explicit expiry mandatory | replay denied unless a declared idempotent retry preserves logical request identity |
| Query | explicit expiry mandatory | replay denied after expiry or completed request unless explicitly permitted |
| Response | bound to request validity and response policy | unsolicited, duplicate-unsafe, or wrong-request response rejected |
| Event | immutable occurrence time and publication time; delivery validity is separate | replay must be marked and cannot recreate action authority |
| Notice | expiry mandatory when information can become stale | stale or duplicate notice cannot become authoritative fact |

Clock uncertainty beyond the approved tolerance SHALL prevent permissive freshness decisions.

## 9. Envelope and Payload Protection

The FIL envelope SHALL expose only routing and enforcement metadata required before decryption.

The following visible fields SHALL be cryptographically bound to the protected payload when P3 applies:

- message ID;
- kind and type;
- schema ID and version;
- original producer;
- intended target or topic;
- purpose;
- correlation and causation;
- creation and expiry or replay policy;
- classification;
- protection profile;
- key reference; and
- priority authority.

An intermediary SHALL NOT alter a bound field or impersonate the original producer. Transport-attempt metadata shall remain separate from original message identity.

## 10. Key and Secret Lifecycle

Every cryptographic key class SHALL declare:

- key ID, purpose, owner, algorithm profile, and strength;
- permitted subjects, operations, data classes, and environments;
- generation authority and approved random source;
- storage and access boundary;
- distribution or agreement method;
- activation, expiry, rotation, overlap, and retirement;
- revocation and compromise response;
- backup and recovery obligations;
- destruction or crypto-erasure evidence;
- audit requirements; and
- migration and algorithm-replacement path.

Keys SHALL NOT be:

- embedded in source code;
- committed to the repository;
- stored as ordinary configuration values;
- transmitted inside FIL payloads;
- written to logs or verification evidence;
- reused across unrelated purposes or environments without explicit approval; or
- accessible to a component solely because it can access encrypted data.

## 11. Cryptographic Agility

The design SHALL support replacement of cryptographic algorithms, protocols, key sizes, certificate forms, and providers without changing FIL business meaning.

Every protected artifact SHALL identify a governed cryptographic profile and version. The profile SHALL be selected through a Stage 0 ADR and security design based on current approved standards and threat evidence.

Unknown, deprecated, prohibited, or downgraded profiles SHALL be rejected. Multiple profiles may coexist only during an explicit bounded migration.

Falcon SHALL NOT design or implement custom cryptographic algorithms or protocols.

## 12. Required Threat Cases

The amendment and technical design SHALL address:

- passive interception;
- message modification;
- producer impersonation;
- wrong-recipient delivery;
- replay, duplication, delay, and reordering;
- expired or revoked credentials;
- cryptographic downgrade;
- nonce or key misuse;
- compromised intermediary;
- compromised endpoint;
- key theft or unauthorized use;
- secret leakage through configuration, logs, crashes, evidence, or backups;
- clock manipulation or uncertainty;
- traffic-analysis and metadata exposure;
- denial of service caused by expensive verification;
- corrupted encrypted storage;
- lost key material and unrecoverable evidence; and
- malicious or failed key rotation.

Encryption does not claim to protect plaintext after a fully compromised authorized endpoint has decrypted it. Isolation, least authority, monitoring, and revocation remain necessary.

## 13. Failure and Safe Behavior

Malformed protection metadata, unknown profile, failed integrity, failed decryption, wrong recipient, invalid identity, expired credential, stale revocation status, prohibited replay, clock uncertainty, or unavailable required key SHALL cause explicit rejection or protective restriction.

Falcon SHALL NOT:

- fall back silently to plaintext;
- reduce a required protection profile;
- reuse stale permission because cryptographic services are unavailable;
- discard an undecryptable audit-critical record silently; or
- claim successful delivery or persistence when only encrypted bytes were accepted.

## 14. Logging and Evidence

Security evidence SHALL preserve:

- message and attempt identities;
- protection profile and key ID;
- producer and recipient identities;
- validation stages and results;
- expiry, revocation, and replay decisions;
- error class;
- correlation and causation; and
- time and clock quality.

Evidence SHALL NOT contain plaintext sensitive payloads, private keys, secret key material, raw credentials, or unnecessary cryptographic internals.

## 15. Required Document Amendments

Approval of this plan authorizes preparation, not automatic approval, of:

| Document | Proposed version | Required change |
|---|---:|---|
| SEC-001 | 1.1 | Add explicit transport, message-level, at-rest, key-lifecycle, anti-downgrade, and cryptographic-agility obligations |
| SYS-005 | 1.1 | Enforce protected transport, endpoint identity, profile preservation, and fail-closed boundary behavior |
| SYS-009 | 1.1 | Define FIL as Falcon Interaction Language and add universal protection, temporal, replay, and profile semantics |
| CON-004 | 1.1 | Add protection profile, key reference, recipient binding, integrity/encryption scope, and replay fields |
| FDN-002 | 1.1 | Map interactions and classifications to FIL-P1 through FIL-P4 |
| FIL-001 | 1.1 | Add machine-verifiable protection metadata and message-kind temporal rules |
| VPL-004 | 1.1 | Add interception, tampering, downgrade, replay, wrong-recipient, key, and plaintext-fallback cases |
| IMP-001 | 1.1 | Add cryptographic implementation and verification gates without selecting algorithms outside Stage 0 ADRs |

No Approved version is modified until the amendment package receives explicit approval and supersession is recorded.

## 16. Implementation Sequence

1. Approve SEC-PLAN-001.
2. Prepare the eight versioned amendment candidates.
3. Perform constitutional, security, compatibility, and verification review.
4. Approve or reject the amendment package.
5. Accept the Stage 0 cryptographic profile ADR.
6. Implement protection primitives behind governed boundaries.
7. Run Contract and negative security tests.
8. Run amended VPL-004 and affected VPL scenarios.
9. Preserve results and approve only the verified artifact.

## 17. Acceptance Criteria

The cryptographic protection amendment is ready for implementation only when:

- all eight amendments are Approved;
- the Stage 0 cryptographic profile ADR is Accepted;
- every interaction has a minimum protection profile;
- every key class has a lifecycle and owner;
- no plaintext downgrade path exists;
- schema and compatibility tests pass;
- negative and abuse cases are defined;
- independent security review passes;
- no secret exists in source, ordinary configuration, logs, or test evidence; and
- rollback and algorithm migration are defined.

## 18. Non-Claims

Approval or implementation of this plan does not prove:

- absolute security;
- protection after full compromise of an authorized endpoint;
- regulatory or financial-production compliance;
- post-quantum readiness unless separately demonstrated;
- safe live-capital operation; or
- permission to connect Falcon to any financial system.

## 19. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-009 | 2026-07-24 |
