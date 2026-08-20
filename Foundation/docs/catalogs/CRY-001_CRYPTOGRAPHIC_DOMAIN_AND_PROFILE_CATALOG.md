# CRY-001 — Cryptographic Domain and Profile Catalog

**Identifier:** CRY-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-036  
**Amendment Package:** AMD-003  
**Owner:** Falcon Security Authority  
**Governing Authority:** ADR-I005; ADR-I008; AMD-003; IMP-001 v1.2; CON-016; CON-017; CON-018; CON-019; CON-020; CON-021; VPL-BST-005  
**Implementation Authority:** Not Granted
**Supersedes:** CRY-001 v1.0  
**Superseded By:** None

## 1. Purpose

This Catalog defines how Falcon cryptographic, secret, certificate and identity, and randomness Provider candidates may be constructed and verified before their Profiles or custody mechanisms are active.

It prevents candidate security material, test roots, bootstrap entropy, external identity, or candidate evidence from entering operational trust or certifying itself.

## 2. Preserved Decisions

Unless explicitly amended here, CRY-001 v1.0 remains controlling for:

- cryptographic Profile, Domain, Purpose, Operation, Environment, Algorithm, Root, and Key Class catalogs;
- Profile, Domain, and key lifecycle values;
- `FALCON-CRYPTO-1`;
- approved primitive and parameter selections;
- immutable Domain and Purpose identifiers;
- canonical Domain Context ownership by FCE-001;
- independent root boundaries;
- key-use enforcement;
- nonce, operation, and rotation limits;
- secret prohibitions;
- failure, compromise, migration, coexistence, and post-quantum boundaries;
- Security Authority governance; and
- the rule that Catalog presence, Key Reference possession, signature validity, or cryptographic success does not establish authority or truth.

No existing Profile ID, Domain ID, Purpose ID, Algorithm ID, Root Boundary ID, parameter, or lifecycle meaning is changed.

## 3. Candidate Cryptographic Environment

Candidate cryptographic work SHALL occur only in:

```text
CANDIDATE_PROVIDER_VERIFY
```

under:

- an Enabling-Provider Candidate Authority Instrument;
- an immutable CON-020 Bootstrap Execution Context;
- an exact candidate artifact and Adapter identity;
- synthetic-only data and security material;
- test-only profiles and domains;
- isolated custody;
- external bootstrap identity and time;
- independently controlled evidence collection; and
- the VPL-BST-005 safety boundary.

The environment SHALL contain no production or Foundation-active root, key, secret, certificate, identity, trust anchor, revocation source, credential, protected message, backup, evidence key, or financial material.

## 4. Candidate Profile

CRY-001 v1.1 SHALL recognize:

| Profile ID | Version | Lifecycle | Purpose |
|---|---:|---|---|
| `FALCON-CRYPTO-CANDIDATE-TEST-1` | `1` | `APPROVED`, not `ACTIVE` | isolated verification of Provider and Adapter behavior with synthetic material |

This Profile:

- MAY use only `falcon/test/ephemeral`;
- SHALL use `falcon/root/test-ephemeral`;
- SHALL NOT use any Approved Foundation operational root;
- SHALL NOT protect or validate an Approved Foundation or production artifact;
- SHALL NOT produce material eligible for migration into operational custody;
- SHALL NOT support operational FIL, storage, backup, evidence, identity, baseline, or transport protection; and
- SHALL NOT establish conformance, completeness, validity, or Activation by its own output.

## 5. Synthetic Material

Candidate verification SHALL use only:

- freshly generated disposable test roots;
- disposable test keys;
- test secrets;
- test certificates and issuers;
- test trust anchors;
- test identities;
- test revocation sources;
- synthetic plaintext and protected objects;
- controlled nonces, counters, salts, and randomness;
- declared bootstrap entropy; and
- non-financial fixtures.

Every item SHALL be:

- identified as test-only;
- bound to the candidate environment;
- scoped to one verification case;
- incapable of authorizing operation;
- prohibited from export to operational custody;
- destroyed or retained only according to test evidence policy; and
- absent from production and future production Manifests.

No candidate material may be “promoted,” “reclassified,” “blessed,” rewrapped, reissued, copied, or migrated into an operational Domain.

## 6. Candidate Provider Operations

A candidate Provider MAY execute only operations required to verify:

- algorithm and parameter correctness;
- known-answer and negative test vectors;
- canonical Domain Context;
- Domain, Purpose, environment, identity, operation, and lifecycle enforcement;
- independent-root separation;
- key generation and custody behavior;
- derivation within one test Domain;
- nonce and counter control;
- rotation, revocation, expiry, destruction, and recovery behavior;
- certificate chain, subject, usage, validity, and revocation behavior;
- Secret Reference behavior and prohibited-location controls;
- randomness purpose, length, health, and failure behavior;
- Adapter replaceability; and
- no-downgrade and fail-closed behavior.

Unlisted operations are prohibited.

## 7. Candidate Result Classification

Every candidate operation result SHALL declare:

- `CANDIDATE` lifecycle;
- Candidate Profile;
- candidate Provider and Adapter identity;
- test Domain and Purpose;
- test Root and Key Reference;
- environment and CON-020 context;
- synthetic-material classification;
- external bootstrap identity and time;
- result and limitations;
- evidence origin;
- independent-control reference; and
- explicit `NO_OPERATIONAL_AUTHORITY`.

Candidate success is an observation. It is not an accepted security Claim or Activation.

## 8. External Bootstrap Inputs

Before active Identifier and Time Providers:

- candidate and environment identities SHALL remain `BOOTSTRAP_EXTERNAL_ID`;
- observations SHALL remain `BOOTSTRAP_EXTERNAL`;
- entropy source, mechanism, version, environment, and limitations SHALL be declared;
- external randomness or entropy SHALL not be represented as an active Falcon Randomness Provider;
- external certificates and signatures SHALL remain external provenance;
- external custody SHALL not be represented as Falcon custody; and
- all dependency and common-root risks SHALL remain explicit.

Bootstrap inputs SHALL not validate their own import into Falcon trust.

## 9. Two-Control Verification

Each candidate security case SHALL distinguish:

1. **External Bootstrap Control:** identifies environment, candidate, inputs, execution, faults, original evidence, and custody independently of candidate Claims.
2. **Candidate Control:** performs the operation and produces the behavior under evaluation.

Where the two controls share a host, platform Provider, entropy source, trust anchor, storage boundary, actor, or root of trust:

- the dependency SHALL be disclosed;
- independence SHALL not be claimed;
- compromise scope SHALL include the common dependency; and
- Activation SHALL require additional evidence or restriction defined by the active protection profile.

## 10. Self-Certification Prohibition

No candidate Provider, Adapter, Secret Provider, Certificate and Identity Provider, Randomness Provider, custody mechanism, key, certificate, signature, report, or test harness SHALL be the sole authority that:

- declares its own capabilities valid;
- declares its evidence complete;
- accepts its own security Claims;
- approves its Profile;
- activates itself;
- resolves a material Challenge to itself;
- restores itself after material compromise; or
- promotes any test material.

Successful cryptographic verification proves only the tested property under the preserved test context.

## 11. Candidate Failure and Compromise

Candidate failure includes:

- wrong algorithm or parameter;
- wrong Domain, Purpose, environment, identity, operation, or lifecycle accepted;
- cross-domain or cross-environment use;
- root-boundary collapse;
- raw secret or private-key exposure;
- nonce reuse or operation-bound failure;
- weak, plaintext, local-file, environment-variable, platform-default, or silent Provider fallback;
- secret appearance in configuration, logs, dumps, commands, or evidence;
- skipped certificate revocation or implicit trust;
- randomness failure or repeated output;
- test material leaving the environment;
- candidate self-certification; or
- evidence loss or unverifiable origin.

Failure SHALL:

- stop affected operations;
- quarantine the candidate and material;
- preserve non-secret evidence;
- restrict dependent candidate authority;
- notify Security Authority, Health Monitoring, Self-Awareness, and Guardian where applicable;
- prevent Activation; and
- require governed remediation and independent reevaluation.

## 12. Candidate Cleanup

Cleanup SHALL:

- terminate candidate execution;
- revoke or invalidate all test credentials and certificates;
- destroy disposable keys and secrets within demonstrable custody capability;
- remove temporary plaintext and material;
- preserve required non-secret evidence;
- verify no material entered prohibited locations;
- record incomplete destruction or residual uncertainty;
- quarantine affected environment images where needed; and
- require independent confirmation.

Cleanup success SHALL not validate the candidate.

## 13. Activation Boundary

An operational Crypto, custody, Secret, Certificate and Identity, or Randomness Profile may become `ACTIVE` only after:

1. the applicable CON-016 through CON-019 Contracts are active;
2. the exact Provider, Adapter, profile, environment, configuration, and artifact identities are known;
3. all required candidate operations and negative cases pass VPL-BST-005;
4. independent root and purpose enforcement is verified;
5. no candidate material is reused;
6. the Evidence Requirement Set is complete;
7. independent validity, security, and compromise review is accepted;
8. material Challenges are resolved;
9. a competent Profile Activation Authority Instrument exists; and
10. a separate exact Activation Decision is recorded.

Activation applies only to the exact Profile, Provider, Adapter, environment, scope, roots, and validity conditions named by the decision.

Candidate Profile Activation SHALL NOT activate `FALCON-CRYPTO-1`.

## 14. Post-Activation Boundary

After operational Activation:

- components SHALL use CON-016 through CON-019;
- candidate and bootstrap Providers SHALL not satisfy operational dependencies;
- test roots and material SHALL remain prohibited;
- direct platform or vendor cryptographic access SHALL remain prohibited;
- bootstrap entropy, identity, time, custody, and certificates SHALL not serve as operational trust;
- historical candidate evidence SHALL remain candidate evidence;
- Provider failure SHALL cause restriction without weaker fallback; and
- Provider return SHALL not automatically restore authority or trust.

## 15. Evidence

Candidate cryptographic evidence SHALL preserve:

- Authority Instrument and CON-020 context;
- candidate and Adapter identities;
- Candidate Profile, Domain, Purpose, Operation, Environment, and lifecycle;
- test Root and non-secret Key Reference;
- synthetic-material classification;
- bootstrap sources and common dependencies;
- input and output digests where safe;
- canonical Domain Context;
- nonce, counter, operation, and rotation disposition;
- custody, certificate, secret, and randomness results;
- failures and cleanup;
- independent-control observations;
- Challenges and evaluations;
- completeness and Activation decisions; and
- explicit non-authorities.

Evidence SHALL contain no raw secret, private key, plaintext requiring protection, reusable nonce, random output, credential, or extractable Provider handle.

## 16. Requirements Added

- **CRY-001-REQ-031:** Candidate cryptographic work SHALL occur only in an authorized isolated `CANDIDATE_PROVIDER_VERIFY` context.
- **CRY-001-REQ-032:** Candidate verification SHALL use only synthetic, disposable, non-production material.
- **CRY-001-REQ-033:** `FALCON-CRYPTO-CANDIDATE-TEST-1` SHALL be limited to `falcon/test/ephemeral` and `falcon/root/test-ephemeral`.
- **CRY-001-REQ-034:** Candidate material SHALL never be promoted, reclassified, migrated, rewrapped, or copied into operational custody.
- **CRY-001-REQ-035:** Candidate operations SHALL remain limited to the declared verification obligations.
- **CRY-001-REQ-036:** Candidate results SHALL remain `CANDIDATE` observations and SHALL not establish completeness, validity, acceptance, or Activation.
- **CRY-001-REQ-037:** Bootstrap identity, time, entropy, certificates, signatures, and custody SHALL retain external classification.
- **CRY-001-REQ-038:** Candidate and external-control observations SHALL remain distinguishable.
- **CRY-001-REQ-039:** Shared control dependencies SHALL be disclosed and SHALL not be mislabeled as independent.
- **CRY-001-REQ-040:** A candidate security subject SHALL not conclusively validate, accept, activate, restore, or promote itself.
- **CRY-001-REQ-041:** Candidate failure SHALL stop affected use, quarantine material, preserve evidence, and prevent Activation.
- **CRY-001-REQ-042:** Cleanup SHALL verify removal and preserve residual uncertainty without claiming unverifiable erasure.
- **CRY-001-REQ-043:** Operational Activation SHALL require VPL-BST-005, complete independent evidence, competent authority, and an exact Activation Decision.
- **CRY-001-REQ-044:** After Activation, components SHALL not fall back to candidate, bootstrap, direct platform, or weaker Provider mechanisms.
- **CRY-001-REQ-045:** Approval of CRY-001 v1.1 SHALL not activate a Profile or Provider, create security material, or establish operational trust.

## 17. Conformance Evidence Added

Activation requires evidence that:

- only the candidate test Profile and Domain are usable in candidate scope;
- all operational Domains and roots are inaccessible;
- every test item is synthetic, disposable, and non-promotable;
- Domain, Purpose, environment, identity, operation, and lifecycle misuse is rejected;
- independent roots remain separate;
- raw secret and private material never crosses custody;
- nonce, counter, operation, rotation, revocation, and failure limits are enforced;
- no weak, plaintext, local, environment, implicit-trust, or silent fallback exists;
- certificate and randomness failure cases are conservative;
- candidate and external-control evidence remain separate;
- shared dependencies are disclosed;
- self-certification and self-Activation are impossible;
- cleanup is independently verified;
- the exact Activation boundary is reconstructable; and
- post-Activation operation rejects candidate and bootstrap substitutes.

## 18. Required Before Operational Profile Activation

An operational Profile SHALL remain non-active until:

1. CRY-001 v1.1 is Approved;
2. CON-016 through CON-019 are Approved and applicable;
3. FCE-001 Domain Context encoding is active for the declared scope;
4. the exact Provider, Adapter, environment, configuration, profile, and artifact identities are known;
5. candidate work uses only the test Profile and synthetic material;
6. VPL-BST-005 produces `PASS`;
7. independent-root, key-use, custody, secret, certificate, randomness, failure, and cleanup evidence is complete;
8. Security Authority accepts validity within its jurisdiction;
9. material Challenges are resolved;
10. a competent Authority Instrument permits Activation; and
11. a separate exact Activation Decision is recorded.

## 19. Supersession

- CRY-001 v1.1 supersedes v1.0;
- every existing operational Profile, Domain, Purpose, Algorithm, Root, parameter, and lifecycle meaning remains unchanged;
- no existing test, candidate, bootstrap, local, or externally held material is grandfathered;
- historical evidence retains its original origin and limitations;
- no Profile or Provider becomes active through the Catalog amendment; and
- no key, secret, certificate, identity, root, nonce, or random material is created by the version change.

## 20. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-036 | 2026-07-25 |

This Approval activates CRY-001 v1.1 as the controlling Catalog and archives v1.0.

It does not:

- activate a Crypto, custody, Secret, Certificate and Identity, or Randomness Profile or Provider;
- create, import, migrate, or promote security material;
- authorize candidate construction or verification execution;
- authorize implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
