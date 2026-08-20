# CRY-001 — Cryptographic Domain and Profile Catalog

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-015  
**Owner:** Falcon Security Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; GOV-SEC-001; FCE-001; ADR-I005  
**Affected Domains:** All protected Foundation activity  
**Implementation Authority:** Not Granted
**Superseded By:** CRY-001 v1.1 under GOV-036

## 1. Purpose

CRY-001 is the canonical catalog of cryptographic profiles, domains, purposes, operations, root-boundary rules, and lifecycle values approved for the Falcon Foundation.

It converts governed cryptographic policy into immutable identifiers and permitted combinations.

> **A cryptographic capability is not an authorized cryptographic use.**

## 2. Catalog Authority

CRY-001 owns:

- Crypto Profile IDs;
- Crypto Profile lifecycle state;
- algorithm identifiers used by profiles;
- Cryptographic Domain IDs;
- Purpose IDs;
- operation identifiers;
- cryptographic environment classes;
- root-boundary identifiers;
- permitted domain-purpose-operation combinations;
- prohibited sharing relationships; and
- catalog lifecycle history.

CRY-001 does not own:

- canonical byte encoding, owned by FCE-001;
- operational authorization, governed by AUT-001;
- Guardian authority;
- platform realization, owned by DESIGN-SEC-001;
- identifier generation, owned by IDN-001;
- time semantics, owned by TIM-001;
- key material;
- secret custody implementation;
- financial permission; or
- implementation authority.

## 3. Catalog Rules

1. Every identifier in this Catalog is immutable in meaning.
2. An identifier SHALL NOT be reassigned, repurposed, aliased, or reused.
3. Changed meaning requires a new identifier.
4. Deprecation occurs by lifecycle transition and catalog versioning.
5. Historical identifiers remain interpretable.
6. Unknown identifiers are rejected.
7. Components SHALL NOT create cryptographic identifiers autonomously.
8. A Catalog entry does not create key material.
9. A Catalog entry does not grant use authority.
10. Only permitted combinations are valid; unlisted combinations are prohibited.

## 4. Lifecycle Values

### 4.1 Crypto Profile Lifecycle

| ID | Meaning | New protection |
|---|---|---|
| `DRAFT` | Review and isolated test only | Prohibited |
| `APPROVED` | Formally approved but not authorized for active creation | Prohibited |
| `ACTIVE` | Permitted for declared new protection and applicable historical operations | Permitted within authority |
| `DEPRECATED` | New protection prohibited; bounded historical read or verification during migration | Prohibited |
| `RETIRED` | Historical recovery or verification only under explicit authority | Prohibited |
| `FORBIDDEN` | Use prohibited because risk is unacceptable, except separately authorized isolated containment or recovery | Prohibited |

Only `ACTIVE` permits new Approved protection.

### 4.2 Domain Lifecycle

| ID | Meaning |
|---|---|
| `DRAFT` | Catalog definition under review |
| `APPROVED` | Definition approved; no operational use without Active profile and authority |
| `ACTIVE` | Domain may receive governed key material and authorized use |
| `DEPRECATED` | No new key classes or protection; migration in progress |
| `RETIRED` | Historical interpretation or recovery only |
| `FORBIDDEN` | Domain use prohibited |

### 4.3 Key Lifecycle Vocabulary

CRY-001 reserves:

- `PLANNED`;
- `GENERATED`;
- `STAGED`;
- `ACTIVE`;
- `ROTATING`;
- `SUSPENDED`;
- `REVOKED`;
- `COMPROMISED`;
- `EXPIRED`;
- `RETIRED`;
- `DESTROYED`; and
- `LOST`.

This vocabulary does not represent actual keys. No Foundation key inventory exists in this Catalog.

## 5. Cryptographic Environment Classes

| Environment ID | Meaning | Approved-material access |
|---|---|---|
| `falcon/env/development` | Local or shared development using non-authoritative disposable material | Prohibited |
| `falcon/env/test` | Automated, integration, fault, and security testing using isolated test material | Prohibited |
| `falcon/env/foundation` | Approved non-financial Foundation verification and future authorized Foundation runtime | Only after separate implementation and custody authorization |
| `falcon/env/recovery` | Isolated governed recovery under explicit incident authority | Bounded by recovery decision |
| `falcon/env/production` | Future financial production environment | Not authorized by FRS-001 |

Secret or private material SHALL NOT be shared across environment classes.

`falcon/env/production` is a reserved future identifier. Its presence does not approve production.

## 6. Operation Catalog

| Operation ID | Meaning |
|---|---|
| `SIGN` | Create a digital signature |
| `VERIFY_SIGNATURE` | Verify a digital signature |
| `ISSUE_IDENTITY` | Issue a bounded identity credential |
| `AUTHENTICATE_ENDPOINT` | Prove or verify transport endpoint identity |
| `ENCRYPT` | Create authenticated ciphertext |
| `DECRYPT` | Verify and recover authenticated plaintext |
| `CREATE_MAC` | Create keyed integrity output |
| `VERIFY_MAC` | Verify keyed integrity output |
| `DERIVE_KEY` | Derive subordinate material within one permitted cryptographic domain |
| `DIGEST` | Create an unkeyed digest identity |
| `VERIFY_DIGEST` | Verify an unkeyed digest identity |
| `GENERATE_RANDOM` | Generate approved cryptographic random material |
| `RECOVER_PROTECTED` | Perform separately authorized isolated historical recovery |

An operation not listed in a domain entry is prohibited for that domain.

## 7. Purpose Catalog

| Purpose ID | Meaning |
|---|---|
| `falcon/purpose/baseline-sign` | Sign or verify an Approved Foundation baseline or high-consequence checkpoint |
| `falcon/purpose/runtime-sign` | Sign or verify bounded runtime component statements |
| `falcon/purpose/identity-issue` | Issue or verify scoped Falcon runtime identity |
| `falcon/purpose/endpoint-authenticate` | Establish mutually authenticated transport endpoint identity |
| `falcon/purpose/fil-encrypt` | Protect sensitive FIL payload confidentiality and integrity |
| `falcon/purpose/fil-authenticate` | Create or verify local keyed FIL integrity when required |
| `falcon/purpose/state-encrypt` | Protect authoritative persisted state |
| `falcon/purpose/evidence-encrypt` | Protect confidential evidence |
| `falcon/purpose/backup-encrypt` | Protect backup and recovery material |
| `falcon/purpose/evidence-link` | Link evidence and checkpoints cryptographically |
| `falcon/purpose/artifact-digest` | Establish ordinary artifact, schema, or evidence digest identity |
| `falcon/purpose/random-material` | Generate key, nonce, salt, or permitted identifier material |
| `falcon/purpose/test-protect` | Exercise cryptographic behavior with isolated disposable test material |
| `falcon/purpose/isolated-recovery` | Recover retained material under exceptional explicit authority |

Purpose IDs are not free text.

## 8. Algorithm Identifier Catalog

| Algorithm ID | Governed meaning | Initial status |
|---|---|---|
| `TLS-1.3` | TLS protocol version 1.3 under the Approved transport profile | Approved in FALCON-CRYPTO-1 |
| `X509-MUTUAL-AUTH` | Mutual endpoint authentication with governed X.509 certificate profiles | Approved in FALCON-CRYPTO-1 |
| `ECDSA-P256-SHA256` | ECDSA over P-256 with SHA-256 | Approved in FALCON-CRYPTO-1 |
| `ECDSA-P384-SHA384` | ECDSA over P-384 with SHA-384 | Approved in FALCON-CRYPTO-1 |
| `AES-256-GCM` | AES-GCM with 256-bit key, 96-bit nonce, and 128-bit tag | Approved in FALCON-CRYPTO-1 |
| `HKDF-SHA256` | HKDF using SHA-256 | Approved in FALCON-CRYPTO-1 |
| `HKDF-SHA384` | HKDF using SHA-384 | Approved in FALCON-CRYPTO-1 |
| `HMAC-SHA256` | HMAC using SHA-256 | Approved in FALCON-CRYPTO-1 |
| `SHA-256` | SHA-256 unkeyed digest | Approved in FALCON-CRYPTO-1 |
| `OS-CSPRNG` | Approved operating-system cryptographic random source behind Falcon provider boundaries | Approved in FALCON-CRYPTO-1 |

Availability in a platform or library does not add an algorithm to this Catalog.

## 9. Crypto Profile Registry

| Profile ID | Version | Lifecycle | New Approved protection | Effective authority |
|---|---:|---|---|---|
| `FALCON-CRYPTO-1` | `1` | `APPROVED` | Prohibited until separately activated | ADR-I005; CRY-001 |

`FALCON-CRYPTO-1` is Approved but not `ACTIVE`.

No Approved Foundation or production cryptographic material SHALL be created under this profile until:

- DESIGN-SEC-001 is Approved;
- exact provider and platform capabilities are verified;
- all required vectors pass;
- key custody is authorized;
- the profile receives a separate `ACTIVE` lifecycle decision from competent authority; and
- implementation authority exists.

## 10. FALCON-CRYPTO-1 Primitive Matrix

| Protection purpose | Algorithm or protocol | Parameters |
|---|---|---|
| Protected cross-boundary transport | `TLS-1.3` | Mutually authenticated; no downgrade |
| Transport endpoint identity | `X509-MUTUAL-AUTH` | Certificate and provider details deferred to DESIGN-SEC-001 |
| Runtime component and endpoint signatures | `ECDSA-P256-SHA256` | Governed key profile |
| Foundation baseline and high-consequence checkpoint signatures | `ECDSA-P384-SHA384` | Offline Foundation custody |
| Sensitive FIL payload encryption | `AES-256-GCM` | Section 11 |
| Sensitive authoritative-state encryption | `AES-256-GCM` | Section 11 |
| Sensitive evidence encryption | `AES-256-GCM` | Section 11 |
| Sensitive backup and recovery encryption | `AES-256-GCM` | Section 11 |
| Key derivation | `HKDF-SHA256` or `HKDF-SHA384` | Exact choice declared by domain entry |
| Local keyed integrity where required | `HMAC-SHA256` | Domain-specific key |
| Ordinary artifact, schema, and evidence digest identity | `SHA-256` | Not proof of truth or authority |
| Random key, nonce, salt, and permitted identifier material | `OS-CSPRNG` | Provider capability verified on target platform |

## 11. AES-256-GCM Parameters

FALCON-CRYPTO-1 AES-256-GCM SHALL use:

- key length: 256 bits;
- nonce length: 96 bits;
- authentication tag length: 128 bits;
- authenticated associated data for material visible context;
- a governed nonce-allocation mechanism;
- no nonce reuse under one key;
- a declared maximum operation bound for every key class;
- rotation before the operation bound or time limit;
- complete rejection on authentication failure;
- no partial plaintext release;
- no weaker retry; and
- no failure detail that creates a useful oracle.

The exact operation bound and rotation interval are not invented by this Catalog. They SHALL be fixed by the key-class profile before activation.

## 12. Domain Registry

All initial domains have lifecycle `APPROVED`, not `ACTIVE`.

### 12.1 `falcon/baseline/signing`

| Property | Value |
|---|---|
| Owner | Project Owner under Foundation signing governance |
| Purpose | `falcon/purpose/baseline-sign` |
| Operations | `SIGN`, `VERIFY_SIGNATURE` |
| Algorithms | `ECDSA-P384-SHA384` |
| Environments | Signing ceremony; verification in governed environments |
| Identity scope | Falcon Foundation baseline and declared high-consequence checkpoints |
| Root boundary | `falcon/root/baseline-offline` |
| Root policy | Independent root required; private material offline |
| Derivation | Prohibited for ordinary runtime use |
| Export | Private material non-exportable from approved custody |
| Sharing prohibited with | Every other domain |

### 12.2 `falcon/identity/issuing`

| Property | Value |
|---|---|
| Owner | Falcon Security Authority within appointed jurisdiction |
| Purpose | `falcon/purpose/identity-issue` |
| Operations | `ISSUE_IDENTITY`, `SIGN`, `VERIFY_SIGNATURE` |
| Algorithms | `ECDSA-P256-SHA256` |
| Environments | Environment-specific independent material |
| Identity scope | Falcon instance and admitted workload identity |
| Root boundary | `falcon/root/identity-issuing` |
| Root policy | Independent root required |
| Derivation | Permitted only within domain using FCE Domain Context |
| Sharing prohibited with | Baseline, transport, FIL, storage, evidence, backup, test |

### 12.3 `falcon/transport/identity`

| Property | Value |
|---|---|
| Owner | Falcon Security Authority within appointed jurisdiction |
| Purpose | `falcon/purpose/endpoint-authenticate` |
| Operations | `AUTHENTICATE_ENDPOINT`, `SIGN`, `VERIFY_SIGNATURE` |
| Algorithms | `TLS-1.3`, `X509-MUTUAL-AUTH`, `ECDSA-P256-SHA256` |
| Environments | Environment-specific independent material |
| Identity scope | One admitted endpoint or explicitly governed endpoint group |
| Root boundary | `falcon/root/transport-identity` |
| Root policy | Independent root required |
| Derivation | Prohibited unless a future Approved certificate profile permits bounded subordinate issuance |
| Sharing prohibited with | Baseline, identity issuing, FIL, storage, evidence, backup, test |

### 12.4 `falcon/fil/encryption`

| Property | Value |
|---|---|
| Owner | Falcon Security Authority; operational use requires Authority Engine |
| Purpose | `falcon/purpose/fil-encrypt` |
| Operations | `ENCRYPT`, `DECRYPT`, `DERIVE_KEY` |
| Algorithms | `AES-256-GCM`; `HKDF-SHA256` |
| Environments | Environment-specific independent material |
| Identity scope | Producer, recipient or governed group, direction, profile, and instance scope |
| Root boundary | `falcon/root/fil-encryption` |
| Root policy | Independent root required |
| Derivation | Permitted within domain using FCE Domain Context |
| Sharing prohibited with | FIL integrity, storage, evidence, backup, baseline, identity, test |

### 12.5 `falcon/fil/integrity`

| Property | Value |
|---|---|
| Owner | Falcon Security Authority; operational use requires Authority Engine |
| Purpose | `falcon/purpose/fil-authenticate` |
| Operations | `CREATE_MAC`, `VERIFY_MAC`, `DERIVE_KEY` |
| Algorithms | `HMAC-SHA256`; `HKDF-SHA256` |
| Environments | Environment-specific independent material |
| Identity scope | Declared producer, consumer or bounded group and direction |
| Root boundary | `falcon/root/fil-integrity` |
| Root policy | Independent root required |
| Derivation | Permitted within domain using FCE Domain Context |
| Sharing prohibited with | FIL encryption and every non-FIL domain |

### 12.6 `falcon/storage/encryption`

| Property | Value |
|---|---|
| Owner | Persistence Authority under Security policy |
| Purpose | `falcon/purpose/state-encrypt` |
| Operations | `ENCRYPT`, `DECRYPT`, `DERIVE_KEY` |
| Algorithms | `AES-256-GCM`; `HKDF-SHA256` |
| Environments | Environment-specific independent material |
| Identity scope | Falcon instance, store boundary, state class, and profile |
| Root boundary | `falcon/root/storage-encryption` |
| Root policy | Independent root required |
| Derivation | Permitted within domain using FCE Domain Context |
| Sharing prohibited with | FIL, evidence, backup, baseline, identity, test |

### 12.7 `falcon/evidence/encryption`

| Property | Value |
|---|---|
| Owner | Evidence Authority under Security policy |
| Purpose | `falcon/purpose/evidence-encrypt` |
| Operations | `ENCRYPT`, `DECRYPT`, `DERIVE_KEY` |
| Algorithms | `AES-256-GCM`; `HKDF-SHA256` |
| Environments | Environment-specific independent material |
| Identity scope | Evidence class, case, retention scope, and authorized reader scope |
| Root boundary | `falcon/root/evidence-encryption` |
| Root policy | Independent root required |
| Derivation | Permitted within domain using FCE Domain Context |
| Sharing prohibited with | FIL, storage, backup, linking, baseline, identity, test |

### 12.8 `falcon/backup/encryption`

| Property | Value |
|---|---|
| Owner | Recovery Authority under Security policy |
| Purpose | `falcon/purpose/backup-encrypt` |
| Operations | `ENCRYPT`, `DECRYPT`, `DERIVE_KEY`, `RECOVER_PROTECTED` |
| Algorithms | `AES-256-GCM`; `HKDF-SHA384` |
| Environments | Environment-specific backup plus explicitly authorized recovery |
| Identity scope | Backup set, recovery set, environment, retention class, and profile |
| Root boundary | `falcon/root/backup-encryption` |
| Root policy | Independent root required; recovery custody required |
| Derivation | Permitted within domain using FCE Domain Context |
| Sharing prohibited with | FIL, storage, evidence, baseline, identity, test |

### 12.9 `falcon/evidence/linking`

| Property | Value |
|---|---|
| Owner | Evidence Authority |
| Purpose | `falcon/purpose/evidence-link` |
| Operations | `CREATE_MAC`, `VERIFY_MAC`, `SIGN`, `VERIFY_SIGNATURE`, `DIGEST`, `VERIFY_DIGEST` as declared by checkpoint class |
| Algorithms | `HMAC-SHA256`, `ECDSA-P384-SHA384`, or `SHA-256` as declared by checkpoint class |
| Environments | Environment-specific; Foundation checkpoints use governed signing custody |
| Identity scope | Evidence stream, checkpoint set, case, and profile |
| Root boundary | `falcon/root/evidence-linking` |
| Root policy | Independent root required for keyed or signing operations |
| Derivation | Permitted for HMAC within domain using FCE Domain Context |
| Sharing prohibited with | Evidence encryption and every other keyed domain |

`SHA-256` linking provides digest identity only. It does not provide keyed authenticity or signature authority.

### 12.10 `falcon/test/ephemeral`

| Property | Value |
|---|---|
| Owner | Verification Authority under Security policy |
| Purpose | `falcon/purpose/test-protect` |
| Operations | All operations required by an Approved isolated test profile |
| Algorithms | FALCON-CRYPTO-1 test coverage only |
| Environments | `falcon/env/development`, `falcon/env/test` |
| Identity scope | One isolated test execution or governed test fixture |
| Root boundary | `falcon/root/test-ephemeral` |
| Root policy | Independent disposable test root; never trusted by Foundation or production |
| Derivation | Permitted only within the isolated test execution |
| Sharing prohibited with | Every non-test domain and environment |

Test material SHALL NOT authenticate, decrypt, sign, or validate Approved Foundation or future production artifacts.

## 13. Root Boundary Registry

| Root Boundary ID | Domain | Independent from |
|---|---|---|
| `falcon/root/baseline-offline` | Baseline signing | All other roots |
| `falcon/root/identity-issuing` | Runtime identity issuance | All other roots |
| `falcon/root/transport-identity` | Transport endpoint identity | All other roots |
| `falcon/root/fil-encryption` | FIL encryption | All other roots |
| `falcon/root/fil-integrity` | FIL keyed integrity | All other roots |
| `falcon/root/storage-encryption` | Authoritative-state encryption | All other roots |
| `falcon/root/evidence-encryption` | Evidence encryption | All other roots |
| `falcon/root/backup-encryption` | Backup and recovery encryption | All other roots |
| `falcon/root/evidence-linking` | Evidence linking and checkpoints | All other roots |
| `falcon/root/test-ephemeral` | Test-only ephemeral protection | All Approved roots |

For CRY-001 v1.0, every initial domain requiring secret or private material uses an independent root boundary.

Subordinate keys MAY derive from a root or parent only within the same domain and only through:

- the domain's permitted HKDF algorithm;
- FCE-001 Domain Context;
- approved identity and environment scope;
- a declared key class;
- a declared maximum use;
- valid authority; and
- provider enforcement.

Parent compromise places every dependent key, identity, and protected object in suspected-compromise scope.

## 14. Key Class Template

Before a domain becomes `ACTIVE`, every key class SHALL declare:

- immutable Key Class ID;
- Domain ID;
- Purpose ID;
- Environment ID;
- identity scope;
- permitted operations;
- profile ID and version;
- algorithm;
- owner;
- custodian;
- generation authority;
- random source;
- root boundary;
- parent rule;
- export policy;
- activation;
- expiry;
- maximum operations;
- nonce policy where applicable;
- rotation threshold;
- overlap window;
- revocation;
- compromise behavior;
- recovery or non-recoverability;
- historical verification need;
- retirement;
- destruction;
- evidence; and
- independent verification.

No key class is operationally complete until every field is governed.

## 15. Key Usage Enforcement

Cryptographic providers SHALL reject use outside:

- Domain ID;
- Purpose ID;
- Environment ID;
- identity scope;
- permitted operation;
- Crypto Profile;
- algorithm;
- key version;
- lifecycle state;
- time validity;
- maximum-use bound;
- authority; and
- Guardian restriction.

A Key Reference:

- is not secret material;
- is not a bearer capability;
- does not prove authority;
- does not permit a different purpose; and
- SHALL NOT bypass provider policy.

Raw secret or private key material SHALL NOT be delivered to ordinary components.

## 16. Domain Context Binding

Every derived key SHALL use the FCE-001 schema:

```text
falcon/crypto/domain-context
```

Changing a material context field SHALL produce a distinct derivation input.

Required governed values include:

- Falcon identity;
- environment;
- instance or approved sharing scope;
- Domain ID;
- Purpose ID;
- direction where applicable;
- Crypto Profile ID and version;
- Algorithm ID; and
- key version.

Use of `salt` alone for domain separation is prohibited.

## 17. Secret Prohibitions

Secret and private material SHALL NOT appear in:

- source code;
- repository history;
- ordinary configuration;
- FIL messages;
- logs;
- evidence;
- error details;
- crash reports;
- command-line arguments;
- filenames;
- public metadata;
- unprotected diagnostics;
- uncontrolled memory dumps;
- uncontrolled backups; or
- retained test artifacts.

CRY-001 contains identifiers and rules only. It SHALL NOT contain keys, secrets, certificates, private parameters, real nonces, or credentials.

## 18. Failure and Compromise

The following SHALL cause rejection or protective restriction:

- unknown Domain ID;
- unknown Purpose ID;
- prohibited combination;
- inactive Crypto Profile;
- unavailable required key;
- wrong domain;
- wrong purpose;
- wrong environment;
- wrong identity scope;
- wrong operation;
- expired or revoked key;
- stale revocation state;
- maximum-use exhaustion;
- nonce-reuse risk;
- integrity failure;
- invalid certificate;
- failed profile validation;
- downgrade;
- root-boundary violation; or
- suspected compromise.

Falcon SHALL NOT:

- fall back to plaintext;
- select a weaker profile silently;
- reuse stale permission;
- treat encrypted bytes as successfully persisted evidence without verification;
- treat undecryptable required evidence as usable;
- assume only one child is exposed after parent compromise; or
- restore authority merely because a provider returns.

## 19. Migration and Coexistence

Multiple Crypto Profiles MAY coexist only during an Approved bounded transition.

Every transition SHALL define:

- source profile;
- target profile;
- affected domains;
- affected key classes;
- new-protection cutoff;
- retained read or verification;
- migration deadline;
- rollback;
- downgrade prevention;
- evidence;
- recovery;
- status transition; and
- competent authority.

Negotiation SHALL NOT select below the minimum profile required by the governing boundary and classification.

## 20. Post-Quantum Boundary

CRY-001 v1.0 does not claim post-quantum protection.

No post-quantum or hybrid primitive is Approved by this Catalog.

Future adoption requires:

- an Accepted ADR;
- a new or amended Crypto Profile;
- platform verification;
- migration design;
- retained-data assessment;
- variable-length Contract compatibility;
- dual-profile transition rules; and
- competent Approval.

## 21. Catalog Governance

Every Catalog change SHALL identify:

- change identity;
- decision authority;
- jurisdiction;
- affected identifier;
- prior state;
- new state;
- rationale;
- evidence;
- effective date;
- compatibility;
- migration;
- rollback;
- retained-data impact;
- challenge path; and
- supersession.

A component SHALL NOT:

- add an entry;
- change status;
- reinterpret meaning;
- invent a purpose;
- alias a domain;
- activate a profile;
- approve a key class; or
- declare permanent `FORBIDDEN` status.

## 22. Catalog Requirements

- **CRY-001-REQ-001:** Every cryptographic use SHALL reference a governed Profile ID, Domain ID, Purpose ID, Environment ID, Algorithm ID, operation, and key version.
- **CRY-001-REQ-002:** Unknown or unlisted identifiers and combinations SHALL be rejected.
- **CRY-001-REQ-003:** Catalog identifiers SHALL be immutable and SHALL NOT be reassigned, repurposed, or reinterpreted.
- **CRY-001-REQ-004:** Only an `ACTIVE` Crypto Profile may create new Approved protection.
- **CRY-001-REQ-005:** `FALCON-CRYPTO-1` SHALL remain `APPROVED` and non-operative until separately activated.
- **CRY-001-REQ-006:** AES-256-GCM SHALL use a 256-bit key, 96-bit nonce, and 128-bit tag.
- **CRY-001-REQ-007:** Nonce reuse under one AES-GCM key SHALL be prohibited.
- **CRY-001-REQ-008:** Every AES-GCM key class SHALL rotate before its maximum operation bound or time limit.
- **CRY-001-REQ-009:** Authentication failure SHALL reject the complete protected object without partial plaintext or weaker retry.
- **CRY-001-REQ-010:** Initial secret or private key domains SHALL use the independent root boundaries defined by section 13.
- **CRY-001-REQ-011:** Cross-domain and cross-environment secret or private key sharing SHALL be prohibited.
- **CRY-001-REQ-012:** Within-domain derivation SHALL use the governed FCE-001 Domain Context.
- **CRY-001-REQ-013:** `salt` alone SHALL NOT establish domain separation.
- **CRY-001-REQ-014:** Parent compromise SHALL place all dependent material and domains in suspected-compromise scope.
- **CRY-001-REQ-015:** Providers SHALL enforce key purpose, domain, environment, identity scope, operation, profile, lifecycle, authority, and Guardian restrictions.
- **CRY-001-REQ-016:** A Key Reference SHALL NOT grant authority by possession.
- **CRY-001-REQ-017:** Ordinary components SHALL NOT receive raw secret or private key material.
- **CRY-001-REQ-018:** Secret material SHALL NOT enter prohibited locations listed in section 17.
- **CRY-001-REQ-019:** Development, test, Foundation, recovery, and future production SHALL NOT share secret or private material.
- **CRY-001-REQ-020:** Test material SHALL NOT authenticate, decrypt, or sign Approved Foundation or future production artifacts.
- **CRY-001-REQ-021:** Business meaning and stable Contracts SHALL remain independent of algorithm-specific field sizes and providers.
- **CRY-001-REQ-022:** Profile negotiation SHALL NOT downgrade below the governing minimum.
- **CRY-001-REQ-023:** Unknown, inactive, stale, conflicting, downgraded, or compromised cryptographic state SHALL cause rejection or restriction.
- **CRY-001-REQ-024:** Cryptographic failure SHALL NOT cause plaintext fallback.
- **CRY-001-REQ-025:** Permanent profile lifecycle decisions SHALL require competent Security Authority under GOV-SEC-001.
- **CRY-001-REQ-026:** Emergency authority MAY temporarily prohibit unsafe use but SHALL NOT alone create permanent `FORBIDDEN` status.
- **CRY-001-REQ-027:** CRY-001 SHALL NOT contain real cryptographic material.
- **CRY-001-REQ-028:** No post-quantum or hybrid protection SHALL be claimed under CRY-001 v1.0.
- **CRY-001-REQ-029:** Catalog change SHALL preserve history, evidence, compatibility, and migration context.
- **CRY-001-REQ-030:** Catalog Approval SHALL NOT grant implementation or operational authority.

## 23. Conformance Evidence

Conformance requires evidence that:

- every profile, domain, purpose, operation, algorithm, environment, and root reference resolves to this Catalog;
- unknown values fail closed;
- unlisted combinations fail closed;
- `APPROVED` profile cannot create new protection;
- AES-GCM parameters are enforced;
- nonce reuse is detected or prevented;
- operation bounds force rotation;
- domain keys cannot perform another domain's operation;
- environment keys cannot cross environments;
- independent root boundaries are preserved;
- Domain Context bytes conform to FCE-001;
- free-form Domain IDs and Purpose IDs are rejected;
- Key References cannot bypass provider enforcement;
- test roots cannot validate Approved material;
- secret scanning detects prohibited exposure;
- downgrade is rejected;
- parent compromise expands exposure to every dependent child;
- profile transitions preserve migration and historical verification;
- Catalog changes cannot rewrite identifier meaning; and
- no cryptographic action is authorized by Catalog presence alone.

## 24. Required Before Activation

FALCON-CRYPTO-1 SHALL NOT become `ACTIVE` until:

1. CRY-001 is Approved;
2. FCE-001 is Approved and registered;
3. IDN-001 is Approved;
4. TIM-001 is Approved;
5. DESIGN-SEC-001 is Approved;
6. exact Windows and Linux provider realizations are pinned;
7. key-class profiles are complete;
8. nonce and operation bounds are approved;
9. positive and negative vectors pass;
10. custody and recovery procedures are approved;
11. threat and compromise tests pass;
12. independent activation evidence is complete;
13. competent Security Authority issues an `ACTIVE` decision; and
14. explicit implementation authority exists.

## 25. Foundational Rules

> **An available algorithm is not an approved algorithm.**

> **An approved profile is not an active profile.**

> **An active profile is not permission to act.**

> **A key reference is not authority.**

> **Domain separation prevents cross-use. Independent roots create independent compromise boundaries.**

## 26. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-015 | 2026-07-25 |

This Approval adopts CRY-001 v1.0 into the Foundation Baseline.

It does not activate FALCON-CRYPTO-1, create or authorize key material, grant cryptographic use, authorize implementation, or authorize financial activity.
