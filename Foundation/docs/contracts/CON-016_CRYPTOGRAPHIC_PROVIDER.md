# CON-016 — Cryptographic Provider Contract

**Identifier:** CON-016  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-028  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; FCE-001; CRY-001; GOV-SEC-001; DESIGN-SEC-001; ADR-I005; ADR-I008; AMD-003; AMD-003-IR-001  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the exclusive governed boundary for Falcon cryptographic operations and key-use enforcement.

Components SHALL NOT call platform or external cryptographic providers directly. Ordinary components SHALL NOT receive raw secret or private key material.

Cryptography protects declared properties. It does not establish truth, authority, trust, or correctness by itself.

## 2. Participants

- **Requester:** the authenticated participant requesting a bounded operation.
- **Cryptographic Provider:** the Falcon boundary enforcing policy and performing the operation.
- **Custody Provider:** the protected facility holding key material behind opaque references.
- **Security Authority:** the competent authority governing profiles, domains, purposes, and prohibitions.
- **Guardian:** the protective authority capable of restricting unsafe use.
- **Evidence Authority:** the participant preserving non-secret operation evidence.

## 3. Provider and Request Model

Every request SHALL declare:

- request and operation IDs;
- requester identity and Authority Decision;
- operation;
- Crypto Profile ID and version;
- Domain ID;
- Purpose ID;
- environment and identity scope;
- Key Reference and version, where applicable;
- canonical input or digest reference;
- canonical associated data or context;
- freshness, nonce, or counter reference where required;
- expected output class;
- Guardian state;
- time and expiry; and
- evidence requirements.

The Provider result SHALL contain:

- `SUCCEEDED`, `REJECTED`, or `FAILED`;
- exact governing identifiers and versions;
- Provider and custody identities;
- public output or opaque protected output;
- verification disposition where applicable;
- constraints and bounded reason;
- time observation;
- evidence reference; and
- integrity protection.

No raw private or secret key material SHALL be returned.

## 4. Enforced Operations

Only operations listed by the active Crypto Profile and Domain entry may be performed, including where governed:

- encrypt and decrypt;
- sign and verify;
- derive within one approved domain;
- wrap and unwrap;
- compute keyed integrity;
- generate key material inside custody;
- rotate or destroy governed key material;
- produce approved digests; and
- query non-secret capabilities and metadata.

Unlisted combinations are prohibited.

## 5. Domain Separation and Key Use

The Provider SHALL enforce:

- Domain ID and Purpose ID from CRY-001, never free text;
- environment, identity scope, operation, profile, and lifecycle;
- canonical FCE-001 Domain Context;
- independent root boundaries where CRY-001 requires them;
- no cross-domain or cross-environment key sharing;
- no operation outside the key's declared purpose;
- no use of `DEPRECATED`, `RETIRED`, or `FORBIDDEN` profiles for new protection;
- nonce, counter, and operation limits;
- Guardian restrictions; and
- current authority.

Compromise of a domain-specific key SHALL NOT authorize or directly expose another domain. Domains requiring independent compromise boundaries SHALL use independent root material.

## 6. Lifecycle and Failure

New protection requires an `ACTIVE` Crypto Profile, active Domain, usable key lifecycle, active Provider Profile, and current authority.

Authentication, integrity, policy, custody, nonce, counter, capability, or evidence failure SHALL:

- reject the complete operation;
- release no partial plaintext;
- perform no weaker retry;
- cause no plaintext fallback;
- cause no silent provider substitution;
- preserve bounded non-secret evidence; and
- trigger restriction or escalation proportionate to consequence.

Unknown or conflicting state is restrictive.

## 7. Bootstrap and Candidate Boundary

A candidate Provider may operate only:

- under Enabling-Provider Candidate Authority;
- in an isolated candidate environment;
- with synthetic, non-production material;
- within test-only domains and roots; and
- as the subject under independent verification.

Candidate output SHALL NOT protect production material, validate its own Activation, or enter operational custody.

## 8. Compatibility and Replaceability

- Provider-specific handles, types, error codes, paths, and vendor identifiers SHALL remain behind the Adapter.
- Consumers SHALL use only Falcon references and governed results.
- Provider replacement SHALL preserve Domain, Purpose, Key Reference, lifecycle, evidence, and no-downgrade meaning.
- Capability claims SHALL be verified on the exact active environment.
- Unsupported mandatory fields or profiles SHALL cause rejection.

## 9. Evidence and Security

Evidence SHALL preserve request identity, authority, Guardian state, Provider and custody identity, profile, domain, purpose, key reference, operation, input/output digests where safe, canonical context, time, result, limits, failures, and responsible authorities.

Evidence SHALL expose no secret, private key, plaintext, reusable nonce, credential, or protected provider handle.

Requests and results SHALL be authenticated, integrity-protected, replay-resistant, and bound to their declared context.

## 10. Normative Requirements

- **CON-016-REQ-001:** Components SHALL perform cryptographic operations exclusively through this Contract.
- **CON-016-REQ-002:** Provider-specific types and handles SHALL NOT cross the Falcon boundary.
- **CON-016-REQ-003:** Ordinary components SHALL NOT receive raw secret or private key material.
- **CON-016-REQ-004:** Every operation SHALL enforce authority, Guardian state, Profile, Domain, Purpose, environment, identity scope, lifecycle, and operation.
- **CON-016-REQ-005:** Domain and Purpose identifiers SHALL come from CRY-001.
- **CON-016-REQ-006:** Canonical cryptographic context SHALL conform to FCE-001.
- **CON-016-REQ-007:** Keys SHALL be rejected outside their declared use.
- **CON-016-REQ-008:** Independent root boundaries SHALL remain cryptographically independent.
- **CON-016-REQ-009:** Cross-domain and cross-environment secret sharing SHALL be prohibited.
- **CON-016-REQ-010:** Nonce, counter, rotation, and operation bounds SHALL be enforced.
- **CON-016-REQ-011:** Cryptographic failure SHALL release no partial plaintext and cause no downgrade or fallback.
- **CON-016-REQ-012:** A Key Reference or valid signature SHALL NOT establish authority by possession.
- **CON-016-REQ-013:** Candidate material SHALL remain synthetic, isolated, and non-operational.
- **CON-016-REQ-014:** Provider replacement SHALL preserve Falcon semantics and evidence.
- **CON-016-REQ-015:** Non-secret operation evidence SHALL be attributable and reconstructable.
- **CON-016-REQ-016:** Unknown, inactive, stale, conflicted, or compromised state SHALL cause rejection or restriction.

## 11. Acceptance Examples

Acceptance requires verified examples of every approved operation; cross-platform canonical equivalence; rejection of wrong purpose, domain, environment, profile, lifecycle, authority, and Guardian state; nonce-reuse prevention; independent-root enforcement; authentication-failure containment; no raw-key exposure; no plaintext fallback; candidate isolation; Provider replacement; and complete non-secret reconstruction.

## 12. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-028 | 2026-07-25 |

This Approval admits CON-016 as a governed Foundation Contract. It does not activate a Provider or Crypto Profile, create operational keys, authorize implementation, or authorize financial activity.
