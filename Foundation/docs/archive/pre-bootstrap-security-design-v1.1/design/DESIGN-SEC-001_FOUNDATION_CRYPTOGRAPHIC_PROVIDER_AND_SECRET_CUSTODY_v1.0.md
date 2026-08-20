# DESIGN-SEC-001 — Foundation Cryptographic Provider and Secret Custody Design

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-018  
**Owner:** Falcon Security Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; GOV-SEC-001; FCE-001; CRY-001; ADR-I005  
**Applicable Platforms:** Approved Windows and Linux Foundation environments  
**Implementation Authority:** Not Granted

## 1. Purpose

DESIGN-SEC-001 defines the Foundation design for cryptographic provider isolation, key and secret custody, platform realization, lifecycle enforcement, recovery, evidence, and failure behavior.

It translates the Approved cryptographic policy into one replaceable, cross-platform design without creating keys, installing providers, or authorizing cryptographic operation.

> **Falcon components use cryptographic capabilities through governed handles. They do not own raw keys.**

## 2. Design Objectives

The design SHALL:

- keep cryptographic policy independent of operating system and vendor;
- keep raw private and secret material outside ordinary components;
- enforce domain, purpose, environment, identity, and operation at the provider boundary;
- use independent roots for CRY-001 domains;
- prevent silent fallback and downgrade;
- support Windows and Linux;
- preserve exact evidence without exposing secrets;
- remain replaceable;
- permit hardware-backed custody where available;
- fail closed when required custody cannot be proven;
- separate Approved Foundation material from test material;
- support rotation, revocation, recovery, and historical verification; and
- avoid claims stronger than the active platform can prove.

## 3. Non-Goals

This design does not:

- create cryptographic material;
- activate `FALCON-CRYPTO-1`;
- authorize source implementation;
- authorize production;
- authorize financial activity;
- select a commercial secret manager;
- require cloud connectivity;
- require Internet connectivity;
- invent cryptographic algorithms;
- expose private keys to components;
- make operating-system administrators invisible to the threat model;
- claim perfect memory erasure;
- treat encryption as authority; or
- replace Guardian, Authority Engine, Recovery, or Evidence Authority.

## 4. Governing Boundary

The logical boundary is:

```text
Falcon Component
    ↓
Falcon Cryptographic Contract
    ↓
Authority, Domain, Purpose, and Policy Enforcement
    ↓
Falcon Cryptographic Provider Adapter
    ↓
Approved Platform or External Provider
    ↓
Protected Key or Secret Custody
```

Components SHALL depend only on Falcon Contracts.

Platform objects, provider handles, certificate-store types, TPM objects, keyring serials, file paths, vendor identifiers, and provider-specific errors SHALL NOT cross the Falcon provider boundary.

## 5. Provider Contracts

### 5.1 Falcon Cryptographic Provider

The provider SHALL expose bounded operations:

- sign;
- verify signature;
- encrypt;
- decrypt;
- create MAC;
- verify MAC;
- derive within one permitted domain;
- digest;
- verify digest;
- generate approved random material;
- inspect non-secret key metadata;
- rotate;
- suspend;
- revoke;
- verify provider capability; and
- produce protected evidence.

### 5.2 Falcon Secret Provider

The Secret Provider SHALL:

- resolve opaque Secret References;
- authorize access by purpose and scope;
- return only the minimum permitted secret form;
- prefer provider-executed operations over secret release;
- enforce lifetime and use limits;
- record access evidence;
- reject export where prohibited;
- support rotation and revocation;
- prevent ordinary enumeration; and
- fail closed.

### 5.3 Falcon Certificate and Identity Provider

The identity provider SHALL:

- resolve certificate and identity references;
- verify chain, profile, validity, revocation, environment, and subject binding;
- preserve original issuer and subject evidence;
- keep private operations inside the provider where supported;
- distinguish authentication from authorization; and
- reject stale or unknown trust state.

## 6. Opaque References

Falcon components SHALL receive:

- Key Reference;
- Secret Reference;
- Certificate Reference;
- Profile ID and version;
- Domain ID;
- Purpose ID;
- Key Version;
- lifecycle state;
- permitted operation result; and
- non-secret evidence reference.

References SHALL NOT contain:

- raw key bytes;
- passwords;
- private parameters;
- unprotected key-encryption keys;
- bearer authority;
- provider credentials;
- file content;
- command-line secrets; or
- sufficient information to bypass provider authorization.

Possession of a reference does not grant use authority.

## 7. Enforcement Sequence

Before every governed cryptographic operation, the provider boundary SHALL verify:

1. caller identity;
2. authenticated workload identity;
3. Authority Engine decision;
4. Guardian restriction state;
5. Crypto Profile and lifecycle;
6. Domain ID;
7. Purpose ID;
8. Environment ID;
9. identity and instance scope;
10. requested operation;
11. algorithm;
12. key identity and version;
13. key lifecycle state;
14. activation and expiry;
15. revocation and compromise state;
16. maximum-use bound;
17. nonce policy where applicable;
18. protected-context validity;
19. provider capability; and
20. evidence availability.

A failure at any controlling check SHALL prevent the operation.

## 8. Provider Result Model

Every operation SHALL return one governed result:

- `SUCCESS`;
- `DENIED`;
- `INVALID_INPUT`;
- `INVALID_CONTEXT`;
- `KEY_UNAVAILABLE`;
- `KEY_EXPIRED`;
- `KEY_REVOKED`;
- `KEY_COMPROMISED`;
- `PROFILE_INACTIVE`;
- `DOMAIN_MISMATCH`;
- `PURPOSE_MISMATCH`;
- `ENVIRONMENT_MISMATCH`;
- `IDENTITY_MISMATCH`;
- `OPERATION_PROHIBITED`;
- `INTEGRITY_FAILURE`;
- `NONCE_RISK`;
- `CAPABILITY_UNAVAILABLE`;
- `PROVIDER_UNTRUSTED`;
- `TEMPORALLY_UNCERTAIN`; or
- `INTERNAL_FAILURE`.

Only `SUCCESS` establishes successful completion of the declared cryptographic operation.

Error details exposed outside the provider SHALL be bounded to prevent secret, oracle, topology, or custody disclosure.

## 9. Custody Classes

### 9.1 Offline Root Custody

Used for:

- `falcon/root/baseline-offline`; and
- any future root explicitly classified offline.

The private root SHALL:

- remain absent from runtime hosts;
- remain absent from source and ordinary configuration;
- be used only through an attributable ceremony;
- require competent authority;
- use independent verification;
- produce public verification material and evidence only;
- have an Approved recovery or declared non-recoverability policy; and
- never issue ordinary runtime operations.

### 9.2 Platform-Isolated Asymmetric Custody

Private operations occur through a platform or hardware provider. Raw private key export is prohibited.

Used for runtime identity, endpoint identity, and other asymmetric classes where supported.

### 9.3 Wrapped Symmetric Custody

Symmetric key material is encrypted at rest under an independent Key-Encryption Key or protected provider and exposed only within the minimum authorized provider operation boundary.

### 9.4 Ephemeral Test Custody

Test material:

- is generated per isolated session or fixture;
- uses `falcon/test/ephemeral`;
- cannot validate Approved Foundation material;
- is never promoted;
- is destroyed at session end;
- is prohibited from backup; and
- is unmistakably classified.

### 9.5 External or Hardware Custody

TPM, HSM, PKCS#11, cloud KMS, or another external provider MAY be admitted later behind Falcon Contracts after a separate decision, threat review, capability verification, exit plan, and recovery design.

## 10. Windows Foundation Design

### 10.1 Service Identity

Each Falcon runtime SHALL use a dedicated Windows service identity or service SID with:

- no interactive logon;
- no shared application identity;
- least operating-system rights;
- no administrative membership;
- explicit key-store ACLs;
- explicit data-directory ACLs;
- denied access for unrelated services; and
- attributable lifecycle.

### 10.2 Asymmetric Private Keys

Windows asymmetric private keys SHALL use CNG Key Storage Provider boundaries.

The initial preference order is:

1. an Approved hardware-backed or platform-backed CNG provider when capability and recovery are proven;
2. Microsoft Software Key Storage Provider with CNG key isolation for eligible Foundation non-production scope; or
3. fail closed.

Private keys SHALL:

- be persisted only under the dedicated service identity or explicit machine-key ACL where justified;
- use no private-key export permission;
- expose operations through opaque handles;
- deny legacy-store duplication;
- deny plaintext export;
- bind provider and key name to governed metadata;
- enforce domain-specific access;
- remain inaccessible to ordinary components; and
- produce operation evidence without private material.

### 10.3 Symmetric Secrets

Windows symmetric material that must persist SHALL be wrapped using an Approved Windows protection profile bound to the dedicated service identity and additional Falcon purpose context.

Current-user or service-identity scope is preferred.

Machine-wide DPAPI scope SHALL NOT be used for Falcon secret custody because it permits broader machine-local decryption than the required service boundary.

Wrapped blobs SHALL additionally be protected by:

- dedicated directory ACL;
- dedicated service identity;
- immutable Key Reference metadata;
- Domain and Purpose binding;
- environment binding;
- integrity verification;
- rotation;
- revocation state;
- backup policy; and
- evidence.

DPAPI or any Windows protection mechanism SHALL remain inside the Adapter. It SHALL NOT become a Falcon Contract.

### 10.4 Certificates

Transport certificates SHALL:

- bind the admitted workload identity;
- use mutually authenticated TLS;
- reference non-exportable private keys;
- reside in an explicitly selected certificate store or provider scope;
- reject ambiguous certificate selection;
- reject expired, revoked, wrong-purpose, wrong-environment, and wrong-subject certificates;
- prohibit search-by-friendly-name alone;
- use immutable certificate identity and key reference; and
- preserve chain and revocation evidence.

### 10.5 Windows Prohibitions

Windows custody SHALL NOT use:

- plaintext secrets in appsettings or environment files;
- registry values containing plaintext secrets;
- command-line secrets;
- machine-wide DPAPI as the sole service boundary;
- exportable private keys;
- PFX files retained with embedded passwords;
- interactive prompts in unattended runtime;
- friendly-name-only certificate lookup;
- inherited broad ACLs;
- administrator convenience as a substitute for custody; or
- automatic software fallback from required hardware-backed custody.

## 11. Linux Foundation Design

### 11.1 Service Identity

Each Falcon runtime SHALL use a dedicated Linux service identity with:

- no interactive shell;
- no shared service account;
- minimal groups;
- no unnecessary capabilities;
- restrictive umask;
- isolated runtime directory;
- dedicated secret-access policy;
- read-only executable and configuration boundaries where applicable;
- mandatory-access-control policy where available; and
- attributable lifecycle.

### 11.2 Persistent Asymmetric Private Keys

The initial preference order is:

1. Approved TPM2, TEE, HSM, or PKCS#11 provider where private operations remain inside the protected provider;
2. Linux Trusted Keys or another Approved kernel or platform protected-key mechanism whose trust source and user-space exposure are verified;
3. an admitted external secret provider behind Falcon Contracts; or
4. fail closed.

A root-owned private-key file with restrictive permissions alone SHALL NOT satisfy Approved persistent asymmetric custody.

### 11.3 Persistent Symmetric Secrets

The initial preference order is:

1. provider-executed operation without releasing raw material;
2. hardware- or kernel-protected key with verified trust source;
3. encrypted credential or wrapped blob delivered only to the dedicated service identity through a protected runtime credential boundary;
4. an admitted external provider; or
5. fail closed.

Where a wrapped secret must enter user-space memory:

- exposure SHALL occur only inside the Falcon provider boundary;
- the buffer SHALL be minimal in size and lifetime;
- the buffer SHALL NOT enter managed object graphs unnecessarily;
- copies SHALL be prohibited;
- diagnostic capture SHALL be disabled for the boundary;
- best-effort supported clearing SHALL occur;
- swap and core-dump exposure SHALL be governed;
- access evidence SHALL be produced; and
- Falcon SHALL NOT claim perfect erasure.

### 11.4 Linux Kernel Keyrings

Linux keyrings MAY be used only after capability verification.

The design SHALL prefer a key type and permissions that prevent ordinary user-space payload reading where the required operation permits it.

Use SHALL define:

- key type;
- owner UID and GID;
- possessor, user, group, and other permissions;
- keyring scope;
- session and process behavior;
- expiry;
- revocation;
- invalidation;
- garbage collection;
- provider operation;
- migration;
- recovery; and
- evidence.

A kernel keyring serial number is a platform handle, not a Falcon Key Reference and not authority.

### 11.5 Linux Encrypted Credentials

An Approved encrypted-credential mechanism MAY deliver wrapped material to a service only when:

- ciphertext is protected at rest;
- decryption is bound to the intended host, user, hardware state, or admitted provider according to profile;
- the runtime delivery location is non-persistent and restricted;
- credential naming is governed;
- plaintext is absent from environment and command line;
- service identity is dedicated;
- restart and rotation behavior are defined;
- host cloning and migration behavior are defined;
- recovery is explicit; and
- capability tests pass.

Host-only encryption without hardware binding SHALL be classified honestly and SHALL NOT claim an independent hardware compromise boundary.

### 11.6 Linux Prohibitions

Linux custody SHALL NOT use:

- plaintext key files as Approved persistent custody;
- secrets in environment variables;
- command-line secrets;
- repository secrets;
- ordinary configuration secrets;
- world-, group-, or unrelated-service-readable material;
- shared service identities;
- unverified `/tmp` delivery;
- retained plaintext credentials;
- unrestricted process dumps;
- silent file fallback when a required protected provider is unavailable; or
- a kernel keyring claim without proving payload-access behavior and permissions.

## 12. Foundation Platform Profiles

### 12.1 `FALCON-CUSTODY-WINDOWS-SOFTWARE-1`

**Lifecycle:** `APPROVED`, not `ACTIVE`

Permits:

- CNG-isolated non-exportable asymmetric private keys;
- dedicated service identity;
- service-bound wrapped symmetric blobs;
- Approved Foundation non-production verification.

Does not permit:

- offline root runtime use;
- production;
- financial use;
- machine-wide DPAPI as sole boundary;
- exportable private keys; or
- hardware-backed claims.

### 12.2 `FALCON-CUSTODY-WINDOWS-HARDWARE-1`

**Lifecycle:** `DRAFT`

Requires:

- exact hardware or platform KSP;
- attested capability;
- non-exportability;
- platform-state binding where required;
- recovery and replacement;
- firmware and provider lifecycle;
- independent verification; and
- separate activation.

### 12.3 `FALCON-CUSTODY-LINUX-PROTECTED-1`

**Lifecycle:** `APPROVED`, not `ACTIVE`

Requires:

- protected provider, kernel Trusted Key, TPM2, TEE, HSM, PKCS#11, or admitted equivalent;
- verified trust source;
- dedicated service identity;
- no raw persistent private key;
- provider-enforced operation;
- explicit recovery; and
- cross-platform evidence.

### 12.4 `FALCON-CUSTODY-LINUX-WRAPPED-1`

**Lifecycle:** `APPROVED`, not `ACTIVE`

Permits only bounded symmetric-secret use where:

- the secret is encrypted at rest;
- access is restricted to the dedicated identity;
- delivery is non-persistent;
- user-space exposure is confined to the provider boundary;
- the profile does not claim hardware isolation;
- diagnostics and dumps are controlled;
- rotation and recovery are proven; and
- the domain's consequence permits software custody.

It does not permit persistent asymmetric root or identity private keys.

### 12.5 `FALCON-CUSTODY-TEST-EPHEMERAL-1`

**Lifecycle:** `APPROVED`, not `ACTIVE`

Permits disposable isolated test material only.

It SHALL NOT protect or validate Approved Foundation or future production artifacts.

## 13. Domain-to-Custody Assignment

| CRY-001 domain | Required custody |
|---|---|
| `falcon/baseline/signing` | Offline Root Custody only |
| `falcon/identity/issuing` | Platform-isolated or external asymmetric custody |
| `falcon/transport/identity` | Platform-isolated or external asymmetric custody |
| `falcon/fil/encryption` | Protected or wrapped symmetric custody permitted by consequence |
| `falcon/fil/integrity` | Protected or wrapped symmetric custody permitted by consequence |
| `falcon/storage/encryption` | Protected or wrapped symmetric custody with recovery policy |
| `falcon/evidence/encryption` | Protected or wrapped symmetric custody with independent evidence access policy |
| `falcon/backup/encryption` | Independent protected custody with tested recovery |
| `falcon/evidence/linking` | Platform-isolated asymmetric or protected symmetric custody according to checkpoint class |
| `falcon/test/ephemeral` | Test Ephemeral Custody only |

Custody assignment SHALL NOT merge CRY-001 independent root boundaries.

## 14. Root and Parent Design

Every CRY-001 root boundary SHALL have:

- independent root material;
- immutable Root Reference;
- accountable owner;
- custodian;
- environment;
- permitted domains;
- permitted operations;
- generation authority;
- storage profile;
- export policy;
- recovery policy;
- activation;
- rotation;
- revocation;
- compromise scope;
- retirement;
- destruction;
- evidence; and
- independent verification.

One platform provider MAY host multiple independent roots only when:

- key material remains cryptographically independent;
- access policies remain independent;
- references cannot cross domains;
- compromise boundaries are honestly documented;
- provider compromise scope is declared; and
- CRY-001 independent-root meaning is preserved.

Provider co-location does not make roots cryptographically shared, but provider compromise may expose several co-located roots. That shared provider risk SHALL remain explicit.

## 15. Key Generation

Key generation SHALL:

- occur inside the strongest available Approved custody boundary;
- use the CRY-001 algorithm and profile;
- use an Approved random source;
- bind Domain, Purpose, Environment, identity scope, and version;
- prohibit caller-selected weak parameters;
- produce public material where applicable;
- record non-secret provenance;
- verify non-exportability where required;
- assign maximum use and time limits;
- create revocation state;
- create evidence; and
- require independent confirmation for material roots.

Imported private or secret material SHALL be prohibited unless a separately Approved migration or recovery plan requires it.

## 16. Key Derivation

Derived keys SHALL:

- remain within one CRY-001 domain;
- use the domain's Approved HKDF;
- use FCE-001 Domain Context in `info`;
- use governed salt according to the key-class profile;
- change when any material context changes;
- receive immutable Key ID and version;
- inherit and narrow parent constraints;
- receive independent use counters where required;
- never widen parent authority; and
- be rejected if parent state is not valid.

Salt alone SHALL NOT establish domain separation.

## 17. AES-GCM Nonce Design

Every AES-GCM key class SHALL select one Approved nonce-allocation strategy before activation.

The strategy SHALL:

- produce exactly 96-bit nonces;
- prevent reuse under one key;
- remain correct across concurrency;
- remain correct across restart;
- remain correct across failover if key use may fail over;
- define persistence and acknowledgment;
- define exhaustion;
- define rollback detection;
- define uncertain outcome handling;
- bind allocation to Key ID and version;
- rotate before the declared maximum; and
- produce evidence without exposing sensitive material.

Random nonces SHALL NOT be assumed safe merely because the random source is strong. The collision bound and maximum operation count SHALL be explicit.

Counter allocation SHALL NOT acknowledge a nonce until its uniqueness state is durably established according to the governing persistence policy.

Unknown nonce outcome SHALL suspend affected key use pending reconciliation.

## 18. Key Rotation

Rotation SHALL:

- create a new Key Version;
- preserve Key ID lineage or governed replacement relationship;
- activate the new version explicitly;
- define bounded overlap;
- prohibit new protection with the old version after cutoff;
- preserve authorized historical read or verification;
- update provider references atomically where required;
- update revocation and cache state;
- preserve evidence;
- support rollback only within the Approved window; and
- terminate overlap automatically.

Rotation SHALL NOT silently change domain, purpose, algorithm, environment, or identity scope.

## 19. Revocation and Compromise

Revocation SHALL propagate to:

- provider operation;
- Authority Engine inputs;
- Guardian restriction inputs;
- certificate validation;
- cached references;
- active sessions;
- retained decrypt or verify policy;
- dependent derived keys;
- recovery;
- evidence; and
- Self-Awareness.

Parent compromise SHALL place every dependent key, identity, and protected artifact in suspected-compromise scope.

Compromise SHALL NOT be cleared because:

- provider connectivity returns;
- a replacement key exists;
- decryption succeeds;
- a certificate remains time-valid;
- no misuse was observed; or
- the original error disappears.

## 20. Backup and Recovery

Every key class SHALL declare one:

- `NON_RECOVERABLE`;
- `RECOVERABLE_WRAPPED`;
- `RECOVERABLE_EXTERNAL`;
- `RECOVERABLE_CEREMONY`; or
- `VERIFY_ONLY_RETAINED`.

Recovery design SHALL identify:

- recovery authority;
- independent reviewer;
- recovery environment;
- material and metadata required;
- quorum where applicable;
- integrity verification;
- confidentiality;
- chain of custody;
- test frequency;
- failure behavior;
- replacement;
- re-protection under an `ACTIVE` profile;
- audit evidence; and
- destruction of temporary recovery material.

A backup containing an encrypted key blob is not proven recoverable until restoration is tested in an isolated environment.

Test material SHALL NOT be backed up.

## 21. Memory and Process Protection

Where raw symmetric material must exist in provider-process memory:

- scope SHALL be minimal;
- lifetime SHALL be minimal;
- copies SHALL be minimized;
- immutable strings SHALL NOT be used;
- ordinary logs and tracing SHALL be prohibited;
- crash dump and core dump policy SHALL protect the boundary;
- debugger access SHALL be governed;
- paging or swap exposure SHALL be addressed;
- supported clearing SHALL be used;
- failure paths SHALL clear best-effort buffers; and
- documentation SHALL avoid claiming guaranteed erasure.

Raw private asymmetric material SHALL not enter ordinary Falcon process memory under the Approved platform-isolated profiles.

## 22. Cache Design

The provider MAY cache:

- opaque provider handles;
- public keys;
- verified certificates;
- non-secret metadata;
- revocation state;
- active profile state; and
- bounded authorization results where AUT-001 permits.

It SHALL NOT cache raw secret or private material outside the Approved custody boundary.

Every cache SHALL define:

- identity;
- scope;
- maximum age;
- invalidation;
- revocation propagation;
- Guardian behavior;
- restart behavior;
- failure behavior; and
- evidence.

Stale security state SHALL fail closed.

## 23. Configuration and Bootstrap

Ordinary configuration MAY contain only non-secret references:

- provider profile;
- provider identifier;
- Key Reference;
- Certificate Reference;
- Domain ID;
- Purpose ID;
- expected public identity;
- store or provider scope;
- lifecycle policy reference; and
- evidence endpoint reference.

Bootstrap SHALL verify:

- Foundation manifest;
- provider identity;
- provider binary or service trust;
- configuration integrity;
- environment identity;
- workload identity;
- expected public root;
- revocation source;
- clock quality;
- Guardian state; and
- Authority Engine availability.

Bootstrap SHALL NOT obtain a secret from source, environment variable, command line, or unprotected configuration.

## 24. Provider Admission and Replaceability

Every provider Adapter SHALL have:

- Capability Passport;
- provider identity and version;
- supported algorithms;
- supported custody classes;
- non-exportability claims;
- threat assumptions;
- platform requirements;
- authority model;
- isolation boundary;
- error mapping;
- lifecycle;
- update policy;
- vulnerability process;
- evidence;
- test vectors;
- migration;
- rollback;
- exit plan; and
- independent verification.

No provider-specific type SHALL cross a Falcon Contract.

Provider replacement SHALL preserve:

- Key Reference meaning;
- domain and purpose;
- Authority Engine behavior;
- evidence;
- historical verification;
- retained-data recovery;
- lifecycle;
- revocation;
- failure semantics; and
- no-downgrade guarantees.

## 25. Threat Model

The design SHALL address at least:

- repository compromise;
- configuration compromise;
- secret-file disclosure;
- environment and command-line leakage;
- log and dump leakage;
- process-memory disclosure;
- provider impersonation;
- handle substitution;
- certificate substitution;
- key export;
- key-domain misuse;
- cross-environment reuse;
- nonce reuse;
- stale revocation;
- provider downgrade;
- platform clone;
- snapshot and rollback;
- backup theft;
- recovery abuse;
- administrator misuse;
- dependency compromise;
- test-key promotion;
- confused deputy;
- orphaned key;
- key loss;
- parent compromise; and
- evidence tampering.

Operating-system kernel or administrator compromise SHALL be an explicit platform compromise condition. Software custody SHALL NOT claim protection from a fully compromised privileged platform.

## 26. Capability Probes

Before activation, each Windows and Linux profile SHALL pass probes for:

- algorithm availability;
- exact parameter support;
- provider identity;
- key generation;
- non-exportability;
- operation through handle;
- access denial to unrelated identity;
- domain and purpose rejection;
- cross-environment rejection;
- lifecycle enforcement;
- rotation;
- revocation;
- provider restart;
- host restart;
- clone and migration behavior;
- backup and recovery;
- memory and dump policy;
- error behavior;
- audit evidence;
- performance bounds; and
- removal without hidden dependency.

A provider's capability claim SHALL NOT substitute for a probe.

## 27. Verification Scenarios

Mandatory negative scenarios include:

- component requests raw private key;
- caller uses a key for wrong domain;
- caller uses a key for wrong purpose;
- caller uses a key in wrong environment;
- wrong identity uses a valid Key Reference;
- export is attempted;
- stale cached authorization is used after revocation;
- revoked certificate remains otherwise time-valid;
- provider is unavailable;
- required hardware custody disappears;
- software fallback is attempted;
- Windows machine-wide secret scope is substituted;
- Linux plaintext key file is substituted;
- kernel key payload is more readable than claimed;
- test key attempts Foundation verification;
- host is cloned;
- nonce allocation persistence is uncertain;
- key rotation is interrupted;
- recovery evidence is incomplete;
- parent key is compromised; and
- provider returns after compromise without independent restoration.

## 28. Evidence Model

Cryptographic operation evidence SHALL record:

- operation ID;
- caller identity;
- Authority Decision reference;
- Guardian state reference;
- Provider ID and version;
- custody profile;
- Key Reference and version;
- Domain ID;
- Purpose ID;
- Environment ID;
- operation;
- Crypto Profile;
- algorithm;
- protected-context digest;
- result;
- reason code;
- time observation;
- use-counter state where applicable;
- revocation state;
- correlation;
- evidence integrity; and
- challenge path.

Evidence SHALL NOT contain:

- raw secret;
- private key;
- plaintext sensitive payload;
- nonce when disclosure is prohibited;
- provider credential;
- unprotected recovery material; or
- oracle-quality failure details.

## 29. Failure Behavior

Required cryptographic custody or operation failure SHALL:

- reject the affected operation;
- prohibit plaintext fallback;
- prohibit algorithm downgrade;
- prohibit silent provider substitution;
- suspend affected key or domain where state is uncertain;
- notify Health Monitoring;
- update Self-Awareness and Fitness to Operate;
- invoke Guardian restriction according to consequence;
- preserve safe evidence;
- isolate unaffected domains only where independence is proven; and
- enter Recovery under competent authority.

Unavailability SHALL NOT cause the provider to export a key for continuity.

## 30. Restoration

Return to unrestricted cryptographic authority requires:

- cause identification;
- compromise-scope assessment;
- provider integrity verification;
- key-state verification;
- revocation reconciliation;
- dependent-key assessment;
- certificate and identity reassessment;
- protected-data impact assessment;
- rotation or replacement where required;
- recovery verification;
- negative tests;
- independent security confirmation;
- Guardian release decision where applicable;
- new Authority Decision; and
- immutable evidence.

The provider and Security Authority whose trust is under review SHALL NOT be the sole restoration authority.

## 31. Design Requirements

- **DESIGN-SEC-001-REQ-001:** Components SHALL use Falcon cryptographic Contracts and SHALL NOT access platform providers directly.
- **DESIGN-SEC-001-REQ-002:** Provider-specific types and handles SHALL remain inside Adapters.
- **DESIGN-SEC-001-REQ-003:** Ordinary components SHALL NOT receive raw private or secret material.
- **DESIGN-SEC-001-REQ-004:** Every operation SHALL verify authority, Guardian state, profile, domain, purpose, environment, identity, operation, lifecycle, and evidence.
- **DESIGN-SEC-001-REQ-005:** Opaque references SHALL NOT grant authority by possession.
- **DESIGN-SEC-001-REQ-006:** Offline Foundation root private material SHALL remain absent from runtime hosts.
- **DESIGN-SEC-001-REQ-007:** Windows asymmetric private keys SHALL use CNG or an admitted stronger provider with export prohibited.
- **DESIGN-SEC-001-REQ-008:** Windows machine-wide DPAPI SHALL NOT be the sole Falcon service-secret boundary.
- **DESIGN-SEC-001-REQ-009:** Windows certificate selection SHALL use immutable identity and SHALL NOT rely on friendly name alone.
- **DESIGN-SEC-001-REQ-010:** Linux persistent asymmetric custody SHALL require a protected provider and SHALL NOT rely on file permissions alone.
- **DESIGN-SEC-001-REQ-011:** Linux wrapped symmetric custody SHALL confine plaintext exposure to the minimum provider boundary and SHALL NOT claim hardware isolation.
- **DESIGN-SEC-001-REQ-012:** Kernel keyring use SHALL require verified key type, permissions, payload visibility, lifecycle, and provider behavior.
- **DESIGN-SEC-001-REQ-013:** Secrets SHALL NOT enter environment variables, command lines, ordinary configuration, logs, or dumps.
- **DESIGN-SEC-001-REQ-014:** Test material SHALL remain isolated, disposable, non-recoverable, and incapable of validating Approved material.
- **DESIGN-SEC-001-REQ-015:** CRY-001 independent root boundaries SHALL remain cryptographically independent.
- **DESIGN-SEC-001-REQ-016:** Co-location risk in one provider SHALL remain explicit and SHALL NOT be mislabeled as independent provider compromise.
- **DESIGN-SEC-001-REQ-017:** Key generation SHALL occur within the strongest Approved custody boundary and produce non-secret evidence.
- **DESIGN-SEC-001-REQ-018:** Derived keys SHALL remain within one domain and use FCE-001 Domain Context.
- **DESIGN-SEC-001-REQ-019:** AES-GCM nonce allocation SHALL remain unique across concurrency, restart, and applicable failover.
- **DESIGN-SEC-001-REQ-020:** Unknown nonce state SHALL suspend affected key use.
- **DESIGN-SEC-001-REQ-021:** Rotation SHALL create a new version, bound overlap, and prohibit old-key new protection after cutoff.
- **DESIGN-SEC-001-REQ-022:** Revocation and compromise SHALL propagate to dependent keys, sessions, caches, authority, Guardian, and evidence.
- **DESIGN-SEC-001-REQ-023:** Every key class SHALL declare recovery or explicit non-recoverability.
- **DESIGN-SEC-001-REQ-024:** Recovery SHALL be tested before recoverability is claimed.
- **DESIGN-SEC-001-REQ-025:** Raw secret memory exposure SHALL be minimized without claiming perfect erasure.
- **DESIGN-SEC-001-REQ-026:** Cached security state SHALL have bounded age and immediate revocation behavior.
- **DESIGN-SEC-001-REQ-027:** Bootstrap SHALL use non-secret references and SHALL verify provider, environment, identity, time, authority, and Guardian state.
- **DESIGN-SEC-001-REQ-028:** Provider replacement SHALL preserve Falcon meaning, evidence, recovery, and no-downgrade behavior.
- **DESIGN-SEC-001-REQ-029:** Capability claims SHALL be verified on Approved Windows and Linux environments.
- **DESIGN-SEC-001-REQ-030:** Required custody failure SHALL fail closed without export, plaintext fallback, downgrade, or silent provider substitution.
- **DESIGN-SEC-001-REQ-031:** Provider return SHALL NOT automatically restore trust or authority.
- **DESIGN-SEC-001-REQ-032:** Restoration SHALL require independent confirmation and new governed authority.
- **DESIGN-SEC-001-REQ-033:** No custody profile in this design SHALL become `ACTIVE` without complete evidence and separate competent activation.
- **DESIGN-SEC-001-REQ-034:** Approval of this design SHALL NOT create keys, activate profiles, or authorize implementation.

## 32. Conformance Evidence

Conformance requires:

- Contract-only component access;
- no platform type leakage;
- raw-key export rejection;
- correct Windows CNG isolation and ACL behavior;
- Windows service-scope secret protection;
- rejection of machine-wide scope substitution;
- correct Linux protected-provider behavior;
- rejection of plaintext-file substitution;
- verified keyring payload and permission behavior where used;
- test-key isolation;
- independent-root verification;
- domain, purpose, environment, and identity rejection tests;
- nonce uniqueness across restart and concurrency;
- rotation and revocation propagation;
- backup restoration tests;
- memory and dump controls;
- provider outage and recovery tests;
- clone, migration, and rollback tests;
- evidence without secret leakage;
- Guardian restriction integration; and
- independent restoration verification.

## 33. Required Before Activation

No custody profile SHALL become `ACTIVE` until:

1. DESIGN-SEC-001 is Approved;
2. CRY-001, FCE-001, IDN-001, and TIM-001 are Approved;
3. exact Windows and Linux versions are pinned in ENV-001 and BLD-001;
4. exact provider identities and versions are pinned;
5. platform capability probes pass;
6. key-class profiles are complete;
7. nonce-allocation profiles are Approved;
8. rotation, revocation, compromise, and recovery procedures are Approved;
9. threat model verification passes;
10. negative and abuse scenarios pass;
11. evidence and traceability are complete;
12. an independent activation assessment is complete;
13. competent Security Authority issues the applicable activation decision; and
14. explicit implementation authority exists.

## 34. References

- Microsoft Cryptography Next Generation key-storage and key-isolation documentation.
- Microsoft Data Protection API documentation, including user and machine scope behavior.
- Linux Kernel Key Retention Service documentation.
- Linux Kernel Trusted and Encrypted Keys documentation.
- systemd credential and protected-storage documentation where applicable to the selected Linux environment.

References describe platform capabilities. They do not override Falcon policy or prove that a specific deployment satisfies this design.

## 35. Foundational Rules

> **The provider performs the operation. The component receives the result.**

> **A protected handle is safer than an exported key, but it is still not authority.**

> **File permissions are access control; they are not by themselves protected key custody.**

> **Software custody may be acceptable only when its weaker compromise boundary is explicit and permitted.**

> **If required custody cannot be proven, the protected capability does not run.**

## 36. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-018 | 2026-07-25 |

This Approval adopts DESIGN-SEC-001 v1.0 into the Foundation Baseline.

It does not create or import key material, activate a custody or Crypto Profile, install a provider, authorize implementation, authorize production, or authorize financial activity.
