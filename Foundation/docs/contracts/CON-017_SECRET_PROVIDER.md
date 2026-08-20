# CON-017 — Secret Provider Contract

**Identifier:** CON-017  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-028  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; CRY-001; GOV-SEC-001; DESIGN-SEC-001; ADR-I005; ADR-I008; AMD-003; AMD-003-IR-001  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the exclusive governed boundary for referencing, authorizing, using, rotating, revoking, and evidencing Falcon secrets.

Components receive opaque Secret References and bounded use outcomes. Raw secret material SHALL NOT enter ordinary components unless an exceptional Contract explicitly requires it and an Approved protection profile permits it.

## 2. Participants

- **Requester:** the authenticated participant requesting a bounded secret use.
- **Secret Provider:** the Falcon boundary resolving opaque references and enforcing use.
- **Custodian:** the protected facility retaining secret material.
- **Secret Owner:** the authority accountable for purpose, lifecycle, and access policy.
- **Security Authority:** the authority governing secret classes and custody profiles.
- **Guardian:** the protective authority capable of restricting use.
- **Evidence Authority:** the participant preserving non-secret evidence.

## 3. Secret Reference

A Secret Reference SHALL identify, without revealing the secret:

- reference ID and version;
- secret class;
- owner;
- domain and purpose;
- environment and identity scope;
- custody profile;
- lifecycle;
- permitted operations;
- access-policy reference;
- rotation and expiry conditions;
- recovery classification; and
- provenance and integrity.

Possession of a Secret Reference grants no authority.

## 4. Use Request and Result

A request SHALL contain requester identity, Authority Decision, Guardian state, Secret Reference, operation, purpose, environment, identity scope, time, expiry, correlation, and evidence requirement.

A result SHALL contain:

- `SUCCEEDED`, `REJECTED`, or `FAILED`;
- request and operation IDs;
- Secret Reference and version;
- Provider and custody identity;
- bounded outcome or protected session reference;
- lifecycle and policy versions;
- reason and constraints;
- evidence reference; and
- no reusable raw secret.

## 5. Secret Handling

Secrets SHALL NOT be placed in:

- source or repository content;
- ordinary configuration;
- environment variables;
- command lines;
- logs, traces, metrics, or evidence;
- exception messages;
- process dumps;
- temporary unprotected files;
- messages outside an Approved protected Contract; or
- developer or bootstrap convenience storage.

Enumeration SHALL be denied by default. Discovery requires explicit authority and returns only bounded metadata.

## 6. Lifecycle

Secret lifecycle SHALL include governed creation or import, activation, use, rotation, restriction, suspension, revocation, expiry, destruction, and evidence retention.

Rotation SHALL:

- create a distinguishable version;
- preserve lineage;
- define overlap and cutover;
- prevent indefinite old-version use;
- preserve recovery obligations; and
- not silently reactivate revoked material.

Destruction evidence SHALL not claim physical erasure beyond what the custody profile can prove.

## 7. Bootstrap and Candidate Boundary

Bootstrap and candidate work SHALL use synthetic, test-only, isolated, disposable secrets.

No bootstrap or candidate secret, root, credential, certificate, or custody object may be promoted into operational custody.

Candidate custody SHALL NOT certify its own protection, completeness, or Activation.

## 8. Failure, Compatibility, and Recovery

Missing authority, invalid reference, wrong purpose, environment, identity, lifecycle, Provider state, Guardian restriction, custody failure, or uncertain revocation SHALL fail closed.

Failure SHALL cause no plaintext fallback, environment-variable fallback, local-file fallback, or silent Provider substitution.

Provider-specific paths, handles, store names, and types SHALL remain behind the Adapter. Provider replacement SHALL preserve Secret Reference meaning, lifecycle, access policy, evidence, and recovery semantics.

Recovery SHALL require explicit authority, declared consequence, dual control where required, protected output, and immutable evidence.

## 9. Evidence and Security

Evidence SHALL preserve identities, authority, Guardian state, Secret Reference, domain, purpose, environment, requested operation, Provider and custody profiles, lifecycle, result, rotation, revocation, recovery, failures, and responsible authorities.

Evidence SHALL NOT contain the secret, a reusable credential, protected plaintext, or provider material that enables extraction.

Requests and results SHALL be authenticated, integrity-protected, replay-resistant, time-bounded, and least-privileged.

## 10. Normative Requirements

- **CON-017-REQ-001:** Components SHALL access secrets exclusively through this Contract.
- **CON-017-REQ-002:** Ordinary components SHALL use opaque Secret References and SHALL NOT receive raw reusable secrets.
- **CON-017-REQ-003:** Secret Reference possession SHALL NOT grant authority.
- **CON-017-REQ-004:** Every use SHALL enforce identity, authority, Guardian state, domain, purpose, environment, lifecycle, and operation.
- **CON-017-REQ-005:** Secret material SHALL NOT enter prohibited locations.
- **CON-017-REQ-006:** Enumeration SHALL be denied unless explicitly authorized.
- **CON-017-REQ-007:** Rotation SHALL preserve lineage and bound old-version use.
- **CON-017-REQ-008:** Revoked, expired, suspended, unknown, or compromised material SHALL not be used.
- **CON-017-REQ-009:** Bootstrap and candidate material SHALL never enter operational custody.
- **CON-017-REQ-010:** Failure SHALL cause no plaintext or convenience-storage fallback.
- **CON-017-REQ-011:** Provider-specific custody details SHALL NOT cross the Falcon boundary.
- **CON-017-REQ-012:** Recovery SHALL require explicit bounded authority and evidence.
- **CON-017-REQ-013:** Evidence SHALL remain non-secret, attributable, and reconstructable.
- **CON-017-REQ-014:** Secret destruction claims SHALL remain limited to demonstrable custody capabilities.
- **CON-017-REQ-015:** Provider replacement SHALL preserve Falcon meaning and no-downgrade behavior.

## 11. Acceptance Examples

Acceptance requires valid bounded use; rejection for wrong identity, purpose, environment, lifecycle, or authority; absence from every prohibited location; enumeration denial; rotation and revocation propagation; candidate isolation; custody failure without fallback; controlled recovery; Provider replacement; and non-secret evidence reconstruction.

## 12. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-028 | 2026-07-25 |

This Approval admits CON-017 as a governed Foundation Contract. It does not activate custody, create or import operational secrets, authorize implementation, or authorize financial activity.
