# ADR-I005 — Foundation Cryptographic and Secret Profile

**Identifier:** ADR-I005  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-25  
**Decision Owner:** Falcon Project Owner  
**Scope:** FRS-001 cryptographic algorithms, profiles, domain separation, key and secret custody, lifecycle, failure, and migration  
**Affected Specifications:** SEC-001, SYS-005, SYS-009, SYS-011, OPS-004, FRS-001  
**Applicable Standards:** STD-003, STD-007, STD-009, STD-012, STD-013  
**Related ADRs:** ADR-F004, ADR-F005, ADR-F006, ADR-F008, ADR-I001, ADR-I002, ADR-I003, ADR-I004  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-25

## 1. Context

FRS-001 requires a concrete cryptographic and secret-management profile before any implementation handles protected FIL communication, identity material, sensitive persistence, evidence, backup, or recovery.

The profile must select interoperable standard algorithms, preserve authentication and authorization separation, prevent secret leakage and cross-purpose key use, support controlled replacement, and fail safely when required protection cannot be established.

## 2. Decision Drivers

- current standardized cryptographic protection;
- Windows and Linux support under .NET 10 LTS;
- mutually authenticated encrypted communication;
- authenticated message-level and at-rest encryption;
- attributable release, identity, and evidence signatures;
- strict separation of key purposes and compromise boundaries;
- canonical and governed key-derivation context;
- lifecycle governance for keys and cryptographic profiles;
- protected secret custody without custom cryptography;
- algorithm and provider replaceability;
- independent trust recovery; and
- complete negative and migration verification.

## 3. Higher-Authority Constraints

This decision remains subordinate to the Vision, Constitution, SEC-001, SYS-005, SYS-009, SYS-011, SEC-PLAN-001, AMD-001, approved Contracts, and FRS-001.

Cryptographic validity does not establish authorization, factual truth, execution, persistence, or successful outcome.

> **Encryption does not grant authority. Authentication does not grant authority.**

> **No verified key, no trusted identity. Unknown key status, no unrestricted authority.**

## 4. Alternatives Considered

### Custom Falcon cryptographic algorithms or protocols

This was rejected. Falcon SHALL use reviewed standards and platform cryptographic implementations; it SHALL NOT invent cryptographic primitives or protection protocols.

### One key or root for all protection

This was rejected because compromise, rotation, recovery, and authority would become unnecessarily coupled across unrelated security domains.

### Algorithm names embedded throughout Falcon Contracts

This was rejected because algorithms would become part of Falcon business meaning and make migration unsafe.

### Governed versioned profiles behind Falcon-owned providers

This was selected because it makes protection concrete while preserving domain separation, lifecycle control, and cryptographic agility.

## 5. Decision

### 5.1 Initial Profile

The initial Foundation profile SHALL be identified as:

```text
FALCON-CRYPTO-1
```

Its approved primitives are:

| Purpose | Primitive |
|---|---|
| Protected cross-boundary transport | TLS 1.3 |
| Transport endpoint identity | Mutually authenticated X.509 certificates |
| Runtime component and endpoint signatures | ECDSA P-256 with SHA-256 |
| Foundation baseline and high-consequence checkpoint signatures | ECDSA P-384 with SHA-384 |
| Sensitive FIL payload encryption | AES-256-GCM |
| Sensitive state and evidence encryption | AES-256-GCM |
| Sensitive backup and recovery encryption | AES-256-GCM |
| Key derivation | HKDF with SHA-256 or SHA-384 as declared by the profile purpose |
| Local keyed integrity where required | HMAC-SHA-256 |
| Ordinary artifact, schema, and evidence digest identity | SHA-256 |
| Random key, nonce, salt, and identifier material | Approved operating-system cryptographic random source |

The exact platform implementation, operating-system cryptographic provider, certificate encoding, protocol options, and allowed TLS 1.3 cipher suites SHALL be pinned in the implementation security baseline and verified on Windows and Linux.

An algorithm being technically available does not make it approved.

### 5.2 AES-GCM Profile

FALCON-CRYPTO-1 AES-GCM SHALL use:

- a 256-bit key;
- a 96-bit nonce;
- a 128-bit authentication tag;
- authenticated associated data for security-relevant visible context; and
- an approved nonce-allocation mechanism that prevents reuse under the same key.

Key and nonce reuse is prohibited.

Every AES-GCM key class SHALL declare its maximum message or operation bound and SHALL rotate before that bound or its time limit is reached.

Authentication failure SHALL reject the complete protected object. Falcon SHALL NOT release partial plaintext, retry with weaker protection, or distinguish failure details in a way that creates a useful oracle.

### 5.3 FIL Protection Binding

When FIL-P3 or FIL-P4 applies, authenticated associated data SHALL bind the material visible envelope fields required by SEC-PLAN-001 and CON-004, including identity, recipient or scope, purpose, classification, profile, time, causation, priority authority, and schema identity where applicable.

Transport protection, message protection, persistence protection, and authorization SHALL remain distinct results.

### 5.4 Key-Purpose Separation

Independent key classes SHALL exist for at least:

- Foundation baseline signing;
- runtime identity issuance;
- transport endpoint identity;
- FIL encryption;
- FIL integrity where a keyed integrity mechanism is required;
- authoritative-state encryption;
- evidence encryption;
- backup and recovery encryption;
- evidence linking and checkpoints; and
- test-only ephemeral protection.

Development, test, approved Foundation, and future production environments SHALL NOT share secret or private key material.

A key SHALL have one declared domain and permitted purpose set. Possession of decryption capability does not grant authority to read, alter, approve, or act upon the plaintext.

### 5.5 Cryptographic Domain Separation

> **Cryptographic keys and derived material SHALL be domain-separated by purpose, environment, identity scope, and protection profile. Compromise of a domain-specific key SHALL NOT authorize or directly expose another domain. Domains requiring independent compromise boundaries SHALL use independent root key material.**

Independent roots SHALL be used when domains differ materially in:

- accountable owner;
- consequence of compromise;
- authority or identity scope;
- lifecycle or rotation;
- backup or recovery need;
- exposure boundary; or
- required independent revocation.

If an approved policy permits multiple keys to derive from one parent, HKDF SHALL use a governed canonical Domain Context in its `info` input. Relying on `salt` alone for domain separation is prohibited.

Changing any material domain field SHALL produce a cryptographically independent derived key under the approved derivation model.

Domain separation does not justify unnecessary root-key sharing. Compromise of a parent key SHALL place every dependent key and domain into suspected-compromise state.

### 5.6 Canonical Domain Context

The Domain Context SHALL be:

- canonically encoded;
- explicitly versioned;
- typed and length-delimited or equivalently unambiguous;
- independent of unordered field iteration;
- identical across Windows and Linux;
- validated before derivation; and
- covered by published positive and negative test vectors.

It SHALL contain, as applicable:

- context-format version;
- Falcon identity;
- environment identity;
- instance or approved sharing scope;
- Domain ID;
- Purpose ID;
- direction;
- protection-profile ID and version;
- algorithm ID; and
- key version.

Free-form concatenation is prohibited.

The Falcon Canonical Encoding Specification SHALL be the sole authority for serializing Cryptographic Domain Context. Components SHALL NOT implement, modify, alias, or reinterpret Domain Context serialization independently.

Before cryptographic implementation begins, `FCE-001 — Falcon Canonical Cryptographic Context Encoding` SHALL be Approved and registered as the governing technical Specification.

### 5.7 Governed Cryptographic-Domain Catalog

Domain IDs and Purpose IDs SHALL be selected from a governed Falcon cryptographic-domain catalog. Components SHALL NOT create, alias, or reinterpret cryptographic domains autonomously.

The catalog SHALL define for every domain:

- immutable unique Domain ID;
- owner;
- permitted purpose and operation;
- permitted environment and identity scope;
- key type and algorithm profile;
- permitted parent or independent-root requirement;
- lifecycle and rotation;
- revocation and recovery;
- prohibited sharing relationships; and
- definition version.

Initial required domains include:

```text
falcon/baseline/signing
falcon/identity/issuing
falcon/transport/identity
falcon/fil/encryption
falcon/fil/integrity
falcon/storage/encryption
falcon/evidence/encryption
falcon/backup/encryption
falcon/evidence/linking
falcon/test/ephemeral
```

Before cryptographic implementation begins, `CRY-001 — Cryptographic Domain and Profile Catalog` SHALL be Approved.

### 5.8 Domain Catalog Governance

> **Domain IDs are immutable. They SHALL NOT be reassigned or repurposed. Deprecation SHALL occur through catalog versioning, never by changing the meaning of an existing identifier.**

A published Domain ID SHALL remain interpretable for historical evidence and retained data.

Changed meaning requires a new identifier. Removal from active use SHALL use explicit lifecycle status and migration; it SHALL NOT erase or redefine history.

Unknown, free-form, ambiguous, or autonomously created domain and purpose identifiers SHALL be rejected.

### 5.9 Key Usage Enforcement

> **Cryptographic providers SHALL reject any attempt to use a key outside its declared purpose, domain, environment, identity scope, permitted operation, or protection profile, even when the underlying algorithm technically permits that use.**

Ordinary components SHALL receive an opaque Key Reference or protected handle, not raw secret or private key material.

For every operation, the Falcon cryptographic provider SHALL verify:

- caller identity and permitted use;
- key identity and version;
- domain and purpose;
- environment and instance scope;
- requested operation;
- active profile;
- key lifecycle and compromise state; and
- applicable authority and Guardian restriction.

> **A cryptographically capable key is not necessarily an authorized key.**

A Key Reference does not grant authority by possession.

### 5.10 Root and Identity Custody

The Falcon root private key SHALL remain offline and inaccessible to runtime components. It SHALL be used only through the attributable Foundation signing ceremony permitted by FDN-003.

The root SHALL authorize a limited release or issuance chain; it SHALL NOT act as an ordinary runtime identity.

Each Falcon instance SHALL have an independent identity. Each admitted workload SHALL have a distinct, scoped, short-lived identity bound to the instance, artifact, capability, environment, and validity period.

Authentication by a valid certificate SHALL remain separate from default-deny authorization through Authority Engine.

### 5.11 Falcon Secret Provider

All runtime access to secret and private key material SHALL pass through a Falcon-owned Secret Provider Contract and Adapter.

The Windows realization SHALL use approved operating-system certificate and key protection, including non-exportable platform-backed keys where supported.

The Linux realization SHALL use an approved operating-system protected secret mechanism outside the repository and ordinary configuration, with a dedicated runtime identity and verified owner-only access.

If the required custody property cannot be established on a platform, Falcon SHALL fail closed for the affected protection. Falcon SHALL NOT compensate by creating a custom secret vault or storing plaintext keys.

Hardware-backed or external secret providers may be introduced later behind the same Contract through a separate decision and admission review.

### 5.12 Secret Prohibitions

Secret and private key material SHALL NOT appear in:

- source code;
- repository history;
- ordinary configuration;
- FIL messages;
- logs or evidence;
- error details or crash reports;
- command-line arguments;
- unprotected diagnostics or memory dumps;
- filenames or public metadata;
- uncontrolled backups; or
- retained test artifacts.

Secret-bearing buffers SHALL have minimum lifetime and scope. Disposal and memory clearing SHALL use supported platform mechanisms while avoiding claims that managed memory can always be erased perfectly.

### 5.13 Key Lifecycle

Every key class SHALL declare:

- immutable key ID and version;
- owner and custodian;
- domain, purpose, environment, and permitted operations;
- algorithm and profile;
- generation authority and random source;
- storage, access, and export policy;
- activation and expiry;
- maximum use;
- rotation and bounded overlap;
- revocation and compromise response;
- backup and recovery or explicit non-recoverability;
- retention and historical-read obligations;
- retirement and destruction; and
- required evidence.

Rotation SHALL preserve historical attribution. Old read or verification keys MAY remain available only for the approved retention need and SHALL NOT remain valid for new protection.

### 5.14 Cryptographic Profile Lifecycle

Each Crypto Profile SHALL have one governed lifecycle state:

| Status | Permitted meaning |
|---|---|
| `DRAFT` | Review and test only; no Approved artifact protection |
| `APPROVED` | Formally accepted but not yet authorized for active creation |
| `ACTIVE` | Permitted for declared new protection and applicable historical operations |
| `DEPRECATED` | Prohibited for new protection; bounded historical read or verification during migration |
| `RETIRED` | Historical recovery or verification only under explicit authorization |
| `FORBIDDEN` | Cryptographic use prohibited because risk is unacceptable, except isolated governed containment or recovery where separately authorized |

Only an `ACTIVE` profile may create new Approved protection.

Lifecycle transitions SHALL be attributable, evidence-based, effective-dated, and issued by the competent Security Authority under its delegated governance.

Deprecation or retirement SHALL include migration scope, deadline, compatibility, rollback, retained-data treatment, and verification.

Credible critical risk MAY cause immediate suspension and transition to `FORBIDDEN`. Emergency restriction SHALL NOT silently destroy the ability to interpret or recover retained information. Any exceptional recovery use of a forbidden profile SHALL occur in an isolated, explicitly authorized environment and SHALL re-protect recovered material under an Active profile.

Unknown, stale, downgraded, or conflicting profile status SHALL cause rejection or protective restriction.

### 5.15 Crypto Agility and Anti-Downgrade

Falcon Contracts SHALL identify protection by:

- profile ID and version;
- key ID and version;
- domain and purpose identity; and
- protection result.

Business meaning SHALL NOT depend on algorithm-specific types, key sizes, signature lengths, providers, or certificate vendors.

Multiple profiles MAY coexist only during a bounded, approved transition. Negotiation SHALL NOT select below the minimum profile required by classification and boundary.

A replacement profile SHALL preserve identity, authority separation, evidence interpretation, retained-data recovery, and rollback.

### 5.16 Post-Quantum Transition Readiness

FRS-001 does not claim post-quantum protection and SHALL NOT introduce an unapproved post-quantum primitive.

Falcon SHALL nevertheless:

- avoid fixed algorithm-specific field sizes in stable Contracts;
- preserve variable-length signature and key representations;
- inventory data requiring long-lived confidentiality or authenticity;
- support bounded dual-profile migration; and
- keep cryptographic algorithms outside Falcon business meaning.

Adoption of post-quantum or hybrid protection requires a later Accepted ADR, approved profile, migration evidence, and platform verification.

### 5.17 Failure and Compromise Policy

Unknown key, unavailable required key, wrong domain, wrong purpose, expired or revoked key, stale revocation status, nonce-reuse risk, integrity failure, invalid certificate, failed profile validation, or suspected compromise SHALL cause explicit rejection or protective restriction.

Falcon SHALL NOT:

- fall back to plaintext;
- downgrade silently;
- reuse stale cryptographic permission;
- treat encrypted bytes as successfully persisted evidence without verification;
- treat an undecryptable required record as usable evidence; or
- restore authority merely because cryptographic service returns.

Persistence or Evidence Authority SHALL declare required protected data unavailable when its decryption or integrity cannot be established.

Health Monitoring SHALL assess the impairment. Self-Awareness SHALL update known limitations and Fitness to Operate. Guardian SHALL impose consequence-appropriate restrictions. Recovery Authority SHALL lead replacement and reconciliation. Independent verification SHALL precede unrestricted restoration.

Parent-key compromise SHALL place all dependent domains, keys, identities, and protected artifacts within the declared exposure scope until independently resolved.

### 5.18 Test and Development Substitutes

Tests SHALL use isolated test-only roots and ephemeral keys that cannot authenticate or decrypt Approved Foundation or future production artifacts.

Mocks MAY simulate provider outcomes but SHALL NOT establish cryptographic conformance. Conformance tests SHALL execute the real platform cryptographic provider with non-production keys.

Test keys SHALL be unmistakably classified, environment-bound, disposable, and prohibited from approved runtime use.

### 5.19 Security Authority Governance

Cryptographic governance requires a formally delegated Falcon Security Authority whose mandate, limits, emergency powers, review obligations, and relationships with Guardian and Authority Engine are defined outside this ADR.

`GOV-SEC-001 — Falcon Security Authority Charter` SHALL enter the governed Roadmap.

Until that Charter is Approved, the Project Owner acting as current Constitutional Authority retains approval responsibility for Crypto Profiles and permanent lifecycle decisions. Technical components SHALL NOT infer governance authority from this transitional custody.

Security Authority may recommend or, when explicitly delegated, immediately suspend unsafe cryptographic use. It SHALL NOT grant operational authority, override Guardian, self-expand its mandate, or independently verify its own recovery.

### 5.20 Required Pre-Implementation Artifacts

Cryptographic implementation SHALL NOT begin until:

1. CRY-001 is Approved;
2. FCE-001 is Approved and registered;
3. the platform Security Design identifies exact provider realizations and custody controls;
4. Windows and Linux capability probes pass;
5. canonical derivation and serialization test vectors are Approved;
6. key and profile lifecycle procedures are Approved; and
7. negative, migration, compromise, and recovery verification plans are complete.

### 5.21 Scope Limitation

This decision does not authorize source implementation, generation or handling of production private material, installation of a secret product, remote identity service, production deployment, financial integration, or live-capital behavior.

## 6. Consequences

- FRS-001 has one concrete initial cryptographic profile.
- Algorithms remain outside Falcon business meaning.
- Keys are separated by domain, purpose, environment, and compromise boundary.
- Providers enforce key use rather than relying on developer discipline.
- Domain identifiers and canonical derivation context become governed artifacts.
- Profile deprecation and prohibition become explicit lifecycle events.
- Secret custody becomes a platform responsibility behind Falcon-owned Contracts.
- Failure of required protection reduces authority rather than security.
- CRY-001, FCE-001, and a platform Security Design become mandatory gates.
- Security Authority governance enters the Roadmap.

## 7. Risks and Mitigations

- **Nonce reuse:** use governed allocation, per-key bounds, rotation, and negative tests.
- **Cross-purpose key use:** enforce domain metadata and permitted operation inside the provider.
- **Shared-parent compromise:** require independent roots for independent compromise boundaries and treat dependent domains as exposed when a parent is compromised.
- **Ambiguous derivation context:** use one canonical versioned encoding and cross-platform test vectors.
- **Domain identifier repurposing:** make IDs immutable and deprecate only through catalog versioning.
- **Algorithm obsolescence:** govern profile lifecycle, inventory protected data, and require migration plans.
- **Secret leakage:** prevent raw-key access, minimize buffers, scan artifacts, and prohibit ordinary configuration and evidence storage.
- **Platform capability mismatch:** pin and test exact providers on Windows and Linux; fail closed when required properties are absent.
- **False recovery:** separate repair from independent verification and do not restore authority on service availability alone.
- **Governance concentration:** charter Security Authority limits and preserve Guardian and Authority Engine independence.

## 8. Compatibility and Transition

This decision realizes SEC-PLAN-001 without redefining approved security or FIL meaning.

FALCON-CRYPTO-1 SHALL remain interpretable after deprecation so retained evidence and encrypted data can be governed truthfully. A successor profile requires an Accepted ADR or approved lifecycle decision under the future Security Authority Charter, an updated CRY-001 entry, compatibility and migration evidence, and bounded rollback.

## 9. Conformance Evidence

Conformance requires:

- exact platform algorithm and provider inventory;
- TLS 1.3 mutual-authentication and downgrade tests;
- cross-platform signature and verification vectors;
- AES-256-GCM positive, tamper, wrong-key, wrong-context, and nonce-reuse tests;
- HKDF domain-separation vectors;
- canonical Domain Context encoding vectors for Windows and Linux;
- rejection of unknown, free-form, aliased, and repurposed Domain IDs;
- rejection of cross-domain and cross-purpose key use by the provider;
- independent-root and parent-compromise propagation tests;
- profile lifecycle and emergency-forbidden migration tests;
- secret discovery scans across source, configuration, logs, evidence, crashes, and artifacts;
- key rotation, revocation, retirement, loss, recovery, and destruction evidence;
- backup recovery with correct, wrong, unavailable, and retired keys;
- Guardian restriction and independently verified trust-restoration tests; and
- proof that no production or financial secret has entered FRS-001.

## 10. References

- NIST FIPS 186-5, Digital Signature Standard.
- NIST FIPS 197, Advanced Encryption Standard.
- NIST SP 800-38D, Galois/Counter Mode.
- NIST SP 800-57 Part 1 Revision 5, Key Management.
- RFC 5869, HMAC-based Extract-and-Expand Key Derivation Function.
- RFC 8446, The Transport Layer Security Protocol Version 1.3.
- Microsoft .NET 10 cross-platform cryptography documentation.

## 11. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على ADR-I005 بصيغته النهائية، وعلى إضافة GOV-SEC-001 إلى Roadmap، وعلى قاعدة تصنيف الوثائق المصححة.” | 2026-07-25 |
