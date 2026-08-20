# ADR-F004 — FIL Representation and Schema Mechanism

**Identifier:** ADR-F004  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** Canonical FIL representation and schema validation for FRS-001  
**Affected Specifications:** SYS-009, CON-004, FRS-001  
**Applicable Standards:** STD-003, STD-013  
**Related ADRs:** ADR-F003, ADR-F005, ADR-F006  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

FIL defines Falcon’s canonical message semantics, but FRS-001 requires one concrete and interoperable representation that can be validated, inspected, preserved, and evolved. Without a canonical representation and schema mechanism, two components could interpret the same message differently or accept incompatible meaning silently.

The selected mechanism must support trustworthy evidence and future evolution without binding FIL semantics permanently to one transport or implementation platform.

## 2. Decision Drivers

- provide deterministic and machine-verifiable message structure;
- remain human-inspectable during Foundation review and recovery;
- preserve explicit message and schema versions;
- reject unknown required meaning and incompatible messages;
- support canonical integrity evidence;
- separate message meaning from transport technology;
- permit governed schema evolution; and
- avoid proprietary or platform-specific representation.

## 3. Higher-Authority Constraints

This decision is constrained by:

- the Vision requirements for disciplined, explainable, and maintainable operation;
- constitutional requirements for truthful information, evidence, compatibility, and governed evolution;
- SYS-009 requirements for canonical FIL semantics, explicit schema identity and version, lossless serialization, and explicit rejection;
- CON-004 requirements for the FIL envelope and integrity preservation;
- STD-013 requirements for observable, versioned, compatible Contracts; and
- FRS-001 requirements for structurally valid FIL communication and complete reconstruction.

## 4. Alternatives Considered

### 4.1 Informal text objects without formal schemas

Messages could use a readable structure governed only by documentation.

This was rejected because validation would be inconsistent and compatibility failures could remain silent.

### 4.2 A compact binary representation as the sole Foundation format

Messages could use a binary schema and encoding from the first release.

This was not selected for FRS-001 because Foundation review prioritizes transparency, inspectability, and minimum complexity over size and throughput.

### 4.3 UTF-8 JSON with governed JSON Schema definitions

Messages use a canonical text representation and formal, versioned schemas.

This alternative was selected because it is open, inspectable, broadly interoperable, machine-verifiable, and independent of a transport product.

## 5. Decision

FRS-001 SHALL represent FIL envelopes and payloads as UTF-8 encoded JSON.

Every FIL message SHALL conform to:

1. the approved FIL envelope defined by CON-004;
2. one canonical, governed JSON Schema identified by schema ID and explicit version; and
3. the payload Contract owned by the declared message owner.

JSON Schema Draft 2020-12 SHALL be the Foundation schema-definition mechanism. Canonical schemas SHALL reside in a governed schema catalog with accountable ownership, immutable released versions, and explicit compatibility status.

Validation SHALL occur in distinct stages and SHALL NOT merge:

1. representation decoding;
2. envelope schema validation;
3. payload schema validation;
4. integrity verification;
5. authorization; and
6. domain acceptance.

Passing an earlier stage SHALL NOT imply success at a later stage.

Released schemas SHALL define required fields, field types, permitted values, constraints, and unknown-field behavior explicitly. Unknown required meaning, missing required fields, unsupported versions, duplicate member names, malformed encoding, or nonconforming values SHALL cause explicit rejection.

When deterministic bytes are required for hashing, signing, comparison, or preserved integrity evidence, the message SHALL use the JSON Canonicalization Scheme defined by RFC 8785. Integrity processing SHALL cover the declared canonical envelope and payload scope without changing their meaning.

Time values SHALL use an explicitly normalized UTC representation. Values requiring exact decimal meaning SHALL NOT rely on binary floating-point interpretation.

Schema compatibility SHALL be declared rather than inferred. A breaking semantic change requires a new schema version and controlled transition. A transport or future optimized encoding MAY be added by a later ADR only if round-trip equivalence preserves all normative FIL meaning.

## 6. Consequences

- Foundation messages remain readable during audit and recovery.
- Schema validation becomes consistent across component boundaries.
- Message evolution is explicit and reviewable.
- Canonicalization supports stable integrity evidence.
- Transport technology can change without redefining FIL meaning.
- Text representation has greater size and processing overhead than compact binary formats.
- Schema ownership and version lifecycle become governed project responsibilities.
- Implementations must reject ambiguous input rather than repair it silently.

## 7. Risks and Mitigations

- **Risk:** Different processors may interpret permissive JSON differently.  
  **Mitigation:** Enforce UTF-8, schema validation, duplicate-name rejection, normalized time, and canonicalization for integrity-sensitive uses.

- **Risk:** Schema evolution may break older components.  
  **Mitigation:** Require explicit compatibility classification, immutable released schemas, and controlled version transitions.

- **Risk:** Readable JSON may expose sensitive content in logs.  
  **Mitigation:** Preserve security classification and prohibit uncontrolled payload logging.

- **Risk:** Text processing overhead may become material later.  
  **Mitigation:** Treat FRS-001 as a correctness baseline; permit a proven equivalent encoding through a later ADR.

- **Risk:** Structural validity may be mistaken for permission or success.  
  **Mitigation:** Keep decoding, schema validation, integrity, authorization, acceptance, execution, and outcome as separate results.

## 8. Compatibility and Transition

No prior Falcon1 ADR is superseded.

CON-004 remains the semantic authority for the FIL envelope. This ADR selects its Foundation representation and validation mechanism but does not amend Contract meaning.

Every FRS-001 message kind shall have approved examples and rejection cases against a released schema before implementation authorization. Future representations require evidence of lossless semantic round trips and a later accepted ADR.

## 9. Conformance Evidence

Conformance shall be demonstrated by evidence that:

- valid examples for Command, Query, Response, Event, and Notice pass their released schemas;
- malformed UTF-8, malformed JSON, duplicate names, missing fields, invalid types, and unsupported versions are rejected explicitly;
- unknown-field behavior matches the released schema;
- canonicalization produces stable integrity input for semantically identical accepted messages;
- transport round trips preserve all normative meaning;
- schema validation cannot be mistaken for authorization or execution;
- breaking schema changes cannot replace a released version silently; and
- sensitive payloads are not exposed through validation evidence.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على القرار الرابع” | 2026-07-24 |
