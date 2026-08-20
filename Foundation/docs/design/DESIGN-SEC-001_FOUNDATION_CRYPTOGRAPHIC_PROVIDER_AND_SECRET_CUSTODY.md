# DESIGN-SEC-001 — Foundation Cryptographic Provider and Secret Custody Design

**Identifier:** DESIGN-SEC-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-037  
**Amendment Package:** AMD-003  
**Owner:** Falcon Security Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; ADR-I005; ADR-I008; AMD-003; SEC-001; SEC-002; GOV-SEC-001; CRY-001 v1.1; CON-016; CON-017; CON-018; CON-019; CON-020; CON-021; VPL-BST-005  
**Initial Foundation Platform:** Windows  
**Planned Cloud Direction:** Oracle Cloud Infrastructure, subject to future governed admission  
**Implementation Authority:** Not Granted  
**Supersedes:** DESIGN-SEC-001 v1.0  
**Superseded By:** None

## 1. Purpose

DESIGN-SEC-001 v1.1 defines the governed design for constructing, isolating, testing, evaluating, and later activating Falcon cryptographic, secret, certificate and identity, randomness, and custody Providers.

It preserves the approved security design of v1.0 and closes the bootstrap boundary required by AMD-003.

It does not create security material, run tests, install a Provider, activate a Profile, authorize implementation, or authorize cloud deployment.

## 2. Preserved Design

Unless explicitly amended here, DESIGN-SEC-001 v1.0 remains controlling for:

- Falcon Contract and Adapter isolation;
- opaque references;
- provider enforcement and result models;
- Windows and Linux custody rules;
- independent cryptographic roots;
- key generation, derivation, rotation, revocation, recovery, and memory protection;
- nonce safety;
- provider admission and replaceability;
- threat, evidence, failure, and restoration models; and
- all requirements `DESIGN-SEC-001-REQ-001` through `DESIGN-SEC-001-REQ-034`.

No existing Contract, Profile, Domain, Purpose, Algorithm, custody class, failure meaning, or security boundary is weakened.

## 3. Deployment Direction

Falcon Foundation SHALL begin with an Approved Windows deployment profile.

Windows is the initial operating environment. It is not a permanent architectural dependency and does not define Falcon Contracts.

Falcon is expected to support a later governed cloud deployment, with Oracle Cloud Infrastructure as the currently intended first cloud environment.

That direction:

- SHALL NOT make Falcon dependent on Oracle-specific types, identities, key handles, secret references, event formats, or lifecycle semantics;
- SHALL NOT permit direct Oracle Cloud access from ordinary Falcon components;
- SHALL preserve Falcon Contracts and place cloud-specific behavior behind replaceable Adapters;
- SHALL require a separate environment, threat, custody, identity, recovery, exit, and activation assessment;
- SHALL preserve portability to another cloud, private infrastructure, Linux environment, or future deployment profile; and
- SHALL NOT treat this document as authority to provision or connect to Oracle Cloud.

> **Windows is the first deployment profile. Oracle Cloud is a planned hosting direction. Neither is Falcon’s identity.**

## 4. Platform-Neutral Security Boundary

The controlling boundary remains:

```text
Falcon Component
    ↓
Falcon Security Contract
    ↓
Falcon Policy and Authority Enforcement
    ↓
Replaceable Falcon Adapter
    ↓
Admitted Platform or Cloud Provider
    ↓
Governed Custody
```

The same Falcon meaning SHALL survive platform transition.

Platform or vendor changes SHALL NOT alter:

- Domain and Purpose meaning;
- authority and jurisdiction;
- Key, Secret, Certificate, Identity, and Randomness References;
- failure and uncertainty semantics;
- evidence obligations;
- Guardian restrictions;
- recovery requirements;
- no-downgrade guarantees; or
- independent compromise boundaries.

## 5. Candidate Security Environment

Provider and custody candidates SHALL be exercised only inside the `CANDIDATE_PROVIDER_VERIFY` environment defined by CRY-001 v1.1.

Every execution SHALL be bounded by:

- one competent Authority Instrument;
- one immutable CON-020 Bootstrap Execution Context;
- an exact candidate Provider, Adapter, artifact, configuration, and environment identity;
- `FALCON-CRYPTO-CANDIDATE-TEST-1`;
- synthetic, disposable, non-financial material;
- external bootstrap identity, time, and provenance;
- independently controlled evidence collection;
- the applicable VPL-BST-005 obligations; and
- explicit termination and cleanup conditions.

No operational, Foundation-active, production, market, account, broker, portfolio, or customer material may enter this environment.

## 6. Candidate Provider Boundary

Candidate Providers SHALL implement only the relevant Falcon candidate Contract:

- CON-016 for cryptographic operations;
- CON-017 for secret resolution and custody;
- CON-018 for certificate and identity behavior; and
- CON-019 for randomness.

Candidate status SHALL NOT permit ordinary components to access platform or cloud mechanisms directly.

Candidate-specific types, handles, credentials, SDK objects, error types, tenancy details, regions, vault identifiers, stores, and platform paths SHALL remain inside the Adapter boundary.

## 7. Candidate Material and Custody

Candidate verification SHALL use only:

- ephemeral test roots;
- disposable test keys and secrets;
- test certificates, issuers, identities, and trust anchors;
- synthetic plaintext and protected objects;
- controlled test nonces and counters; and
- declared bootstrap entropy.

Candidate material SHALL:

- remain bound to one candidate environment and verification scope;
- be visibly classified as test-only;
- grant no operational authority;
- be prohibited from backup unless evidence policy preserves only non-secret representations;
- never be promoted, migrated, rewrapped, reclassified, or reused operationally; and
- be destroyed or quarantined under an independently evidenced cleanup decision.

## 8. Windows-First Candidate Realization

The first Foundation candidate realization SHALL target the exact Windows profile admitted by ENV-001 and BLD-001.

Windows candidate verification SHALL prove, as applicable:

- dedicated non-interactive service identity;
- CNG Provider and key-storage identity;
- non-exportable asymmetric operation through an opaque handle;
- service-scoped symmetric protection;
- explicit key-store, certificate-store, and filesystem access boundaries;
- rejection of machine-wide protection as the sole service boundary;
- rejection of friendly-name-only certificate selection;
- rejection of environment-variable, command-line, source, log, dump, and plaintext-file secrets;
- restart, revocation, rotation, cleanup, and recovery behavior; and
- absence of silent software, legacy, local-file, or weaker fallback.

Passing on Windows establishes validity only for the declared Windows scope. It does not establish Linux, cloud, Oracle Cloud, hardware-backed, production, or financial validity.

## 9. Future Oracle Cloud Admission

Oracle Cloud Infrastructure SHALL be treated as a future external platform Provider, not as an intrinsic Falcon dependency.

Before any Oracle Cloud security capability is admitted, a separate governed decision SHALL identify and verify:

- tenancy, compartment, region, environment, and workload identity boundaries;
- exact Adapter and external Provider identities and versions;
- key, secret, certificate, randomness, and custody capabilities used;
- hardware and software compromise claims;
- data residency and cross-region behavior;
- availability, partition, throttling, and outage behavior;
- revocation and policy propagation;
- time, identity, network, and evidence dependencies;
- backup, recovery, migration, deletion, and residual-data behavior;
- administrative and privileged-access scope;
- shared-responsibility assumptions;
- vendor update and deprecation behavior;
- exit, replacement, and retained-data recovery plans;
- cost or quota failure as a security availability condition; and
- independent negative and recovery evidence.

No Oracle Cloud Provider claim, certification, service status, or successful API response SHALL by itself establish Falcon validity, acceptance, or Activation.

## 10. Portability and No Vendor Lock-In

Every platform or cloud security realization SHALL have an exit path before Activation.

The exit path SHALL define:

- how references remain meaningful;
- how protected data remains recoverable;
- how historical signatures and evidence remain verifiable;
- how keys and secrets are rotated or replaced without unsafe export;
- how authority and identity bindings are re-established;
- how migration uncertainty is contained;
- how the old Provider is revoked and retired;
- how residual material is addressed; and
- how rollback avoids weakening protection.

A Provider SHALL NOT be admitted when replacing it would require redesigning Falcon policy, ordinary components, or governing Contracts.

## 11. External Bootstrap Inputs

Before the applicable Falcon Providers are active:

- identity SHALL remain `BOOTSTRAP_EXTERNAL_ID`;
- time SHALL remain `BOOTSTRAP_EXTERNAL`;
- entropy SHALL retain its external source and limitation;
- platform custody SHALL remain external custody;
- signatures and certificates SHALL retain external provenance;
- Provider and Adapter identity SHALL be established independently of candidate Claims; and
- shared roots of trust SHALL be disclosed.

External bootstrap inputs MAY support candidate observations. They SHALL NOT be represented as active Falcon security services or allowed to approve their own replacement.

## 12. Two-Control Verification

Each candidate security case SHALL preserve two distinguishable controls:

1. **External Bootstrap Control:** establishes the execution context, candidate identity, inputs, faults, custody, original observations, and evidence origin.
2. **Candidate Control:** performs the bounded security behavior under evaluation.

The two controls SHALL not be described as independent when they share a host, actor, administrator, platform Provider, cloud account, root of trust, entropy source, evidence store, or signing authority.

Shared dependencies SHALL be recorded as common compromise scope and SHALL require compensating evidence or restriction before Activation.

## 13. Candidate Verification

Candidate verification SHALL include:

- correct algorithm and parameter enforcement;
- Domain, Purpose, environment, identity, operation, and lifecycle separation;
- Key Usage enforcement;
- independent-root separation;
- raw-material non-exportability;
- secret-location prohibitions;
- certificate chain, identity, purpose, validity, and revocation behavior;
- randomness purpose, length, quality, health, and failure behavior;
- nonce and counter safety;
- concurrency, restart, recovery, rollback, clone, migration, and failover behavior where applicable;
- Adapter replaceability;
- denial of candidate-to-operational transition;
- denial of direct platform or vendor bypass;
- fail-closed operation; and
- absence of silent downgrade or fallback.

The exact obligations SHALL be evaluated through VPL-BST-005.

## 14. Self-Certification and Self-Activation Prohibition

No candidate Provider, Adapter, custody mechanism, key, secret, certificate, identity, randomness source, platform service, cloud service, test harness, or evidence processor SHALL be the sole authority that:

- validates its own capabilities;
- establishes its own evidence completeness;
- accepts its own Claims;
- resolves a material Challenge to itself;
- activates its own Profile;
- restores itself after material compromise; or
- promotes candidate material.

Cryptographic success proves only the bounded property tested under the preserved context.

## 15. Failure, Quarantine, and Cleanup

Candidate failure or uncertainty SHALL:

- stop the affected operation;
- prevent Activation;
- quarantine the candidate, environment, and affected material;
- preserve safe evidence;
- prohibit substitution by a weaker or undeclared Provider;
- notify the competent Security Authority and applicable Guardian, Health Monitoring, and Self-Awareness boundaries;
- record the known compromise and uncertainty scope; and
- require governed remediation and independent reevaluation.

Cleanup SHALL:

- terminate the candidate execution;
- revoke test credentials and certificates;
- destroy disposable material within demonstrable capability;
- remove temporary plaintext and handles;
- verify prohibited locations;
- preserve required non-secret evidence;
- record residual material and unverifiable erasure honestly; and
- obtain independent confirmation.

Cleanup does not validate the candidate.

## 16. Activation Boundary

A Provider or custody Profile may become `ACTIVE` only when:

1. its Falcon Contract is active for the declared scope;
2. the exact Provider, Adapter, artifact, configuration, deployment profile, and environment are identified;
3. candidate work used only approved test profiles and synthetic material;
4. VPL-BST-005 passes;
5. the Evidence Requirement Set is complete;
6. security validity is independently evaluated;
7. material Challenges are resolved;
8. recovery and exit are proven;
9. competent authority acts within declared jurisdiction;
10. an exact Activation Authority Instrument exists; and
11. a separate Activation Decision is recorded.

Activation is limited to the exact subject, version, environment, scope, purpose, validity conditions, and time bounds stated in that decision.

Windows Activation SHALL NOT imply Oracle Cloud Activation. Oracle Cloud Activation SHALL NOT retroactively validate Windows or any other environment.

## 17. Post-Activation Boundary

After Activation:

- components SHALL continue to use Falcon Contracts;
- candidate and bootstrap Providers SHALL not satisfy operational dependencies;
- test material SHALL remain prohibited;
- direct platform and vendor access SHALL remain prohibited;
- Provider unavailability SHALL restrict operation without weaker fallback;
- Provider return SHALL not restore trust automatically;
- platform migration SHALL require a new scoped validity and Activation decision; and
- Self-Awareness MAY observe, diagnose, recommend, and initiate only the maintenance actions permitted by separate authority; it SHALL NOT alter security policy, widen authority, activate a Provider, or bypass independent verification.

## 18. Evidence

Security Provider evidence SHALL preserve:

- Authority Instrument and CON-020 context;
- exact Provider, Adapter, artifact, configuration, platform, and environment identities;
- Candidate Profile, Domain, Purpose, Operation, and lifecycle;
- synthetic-material classification;
- external bootstrap sources;
- common dependencies and compromise scope;
- safe input and output digests;
- custody, key-use, certificate, secret, randomness, nonce, rotation, revocation, recovery, and cleanup results;
- Windows-specific or future cloud-specific capability limitations;
- failures, Challenges, and residual uncertainty;
- independent observations and evaluations;
- completeness, acceptance, and Activation decisions; and
- explicit non-authorities.

Evidence SHALL NOT expose raw keys, secrets, protected plaintext, reusable random material, credentials, or extractable Provider handles.

## 19. Requirements Added

- **DESIGN-SEC-001-REQ-035:** Windows SHALL be the initial Foundation platform without becoming a Falcon Contract or permanent architectural dependency.
- **DESIGN-SEC-001-REQ-036:** Oracle Cloud Infrastructure SHALL remain a future governed deployment direction and SHALL not be treated as active or intrinsic to Falcon.
- **DESIGN-SEC-001-REQ-037:** Platform and cloud capabilities SHALL remain behind replaceable Falcon Adapters.
- **DESIGN-SEC-001-REQ-038:** Candidate security work SHALL occur only under an Authority Instrument and immutable CON-020 context in `CANDIDATE_PROVIDER_VERIFY`.
- **DESIGN-SEC-001-REQ-039:** Candidate security material SHALL be synthetic, disposable, non-financial, and permanently non-promotable.
- **DESIGN-SEC-001-REQ-040:** Candidate Providers SHALL conform to CON-016 through CON-019 as applicable and SHALL not expose vendor types or handles across Contracts.
- **DESIGN-SEC-001-REQ-041:** External bootstrap identity, time, entropy, custody, certificates, and signatures SHALL retain external classification.
- **DESIGN-SEC-001-REQ-042:** Candidate and external-control observations SHALL remain distinguishable, and shared dependencies SHALL be disclosed.
- **DESIGN-SEC-001-REQ-043:** A candidate or external Provider SHALL not conclusively validate, accept, activate, restore, or promote itself.
- **DESIGN-SEC-001-REQ-044:** Windows verification SHALL prove the declared Windows custody and secret-isolation boundaries and SHALL not imply validity on another platform.
- **DESIGN-SEC-001-REQ-045:** Oracle Cloud admission SHALL require a separate threat, identity, custody, recovery, evidence, exit, and Activation decision.
- **DESIGN-SEC-001-REQ-046:** Every admitted platform or cloud Provider SHALL have a proven replacement and exit path.
- **DESIGN-SEC-001-REQ-047:** Candidate failure or uncertainty SHALL stop affected use, prevent Activation, quarantine the affected scope, and preserve evidence.
- **DESIGN-SEC-001-REQ-048:** Cleanup SHALL preserve residual uncertainty and SHALL not claim unverifiable erasure.
- **DESIGN-SEC-001-REQ-049:** Activation SHALL be exact, scoped, independently supported, and separately authorized.
- **DESIGN-SEC-001-REQ-050:** Activation in one deployment profile SHALL not activate or validate another deployment profile.
- **DESIGN-SEC-001-REQ-051:** Post-Activation operation SHALL reject candidate, bootstrap, direct-vendor, and weaker fallback mechanisms.
- **DESIGN-SEC-001-REQ-052:** Self-Awareness SHALL not widen security authority, change security policy, activate a Provider, or bypass independent verification.
- **DESIGN-SEC-001-REQ-053:** Approval of DESIGN-SEC-001 v1.1 SHALL not run verification, create security material, activate a Provider or Profile, authorize implementation, or authorize cloud deployment.

## 20. Required Before Activation

No security Provider or custody Profile SHALL become active until:

1. DESIGN-SEC-001 v1.1 is Approved;
2. CRY-001 v1.1 and applicable Contracts are Approved;
3. the exact deployment profile is Approved in ENV-001;
4. the exact toolchain and candidate identities are pinned;
5. a competent Authority Instrument permits the bounded candidate work;
6. VPL-BST-005 passes under a complete Evidence Requirement Set;
7. independent security evaluation is accepted within jurisdiction;
8. recovery, replacement, and exit are proven;
9. all material Challenges are resolved; and
10. a separate exact Activation Decision is issued.

For Oracle Cloud, the future cloud-specific admission obligations in this document SHALL also be satisfied.

## 21. Supersession

With this Approval:

- DESIGN-SEC-001 v1.1 supersedes v1.0;
- all v1.0 decisions and requirements not expressly amended remain controlling;
- Windows becomes the declared initial Foundation platform;
- Oracle Cloud Infrastructure becomes the declared planned first cloud direction, not an active dependency;
- no security material, Provider, Profile, cloud resource, or execution is created; and
- implementation and operational authority remain ungranted.

## 22. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-037 | 2026-07-25 |

This Approval adopts DESIGN-SEC-001 v1.1 into the Foundation Baseline and archives v1.0.

It does not:

- provision Windows or Oracle Cloud resources;
- create, import, migrate, or promote keys, secrets, certificates, identities, or roots;
- activate any Provider, custody mechanism, Crypto Profile, or deployment profile;
- authorize candidate execution;
- authorize implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
