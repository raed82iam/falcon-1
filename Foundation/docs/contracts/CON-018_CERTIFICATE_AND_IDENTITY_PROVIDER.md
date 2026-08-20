# CON-018 — Certificate and Identity Provider Contract

**Identifier:** CON-018  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-028  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; CRY-001; GOV-SEC-001; DESIGN-SEC-001; ADR-I005; ADR-I008; AMD-003; AMD-003-IR-001  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the governed boundary for certificate discovery, identity binding, chain validation, validity evaluation, revocation checking, presentation, rotation, and evidence.

A valid certificate proves only the claims established by the applicable identity policy within its declared scope. It does not by itself grant Falcon authority or trust.

## 2. Participants

- **Requester:** the participant requesting identity material or validation.
- **Certificate and Identity Provider:** the Falcon boundary enforcing identity policy.
- **Identity Issuer:** the governed issuer of the credential or certificate.
- **Identity Subject:** the human, workload, instance, endpoint, or authority represented.
- **Trust-Anchor Authority:** the authority governing accepted anchors and constraints.
- **Revocation Source:** the governed source of revocation state.
- **Security Authority and Guardian:** the authorities governing security policy and protective restriction.
- **Evidence Authority:** the participant preserving validation evidence.

## 3. Identity Reference and Request

An Identity or Certificate Reference SHALL identify subject class, expected identity, issuer scope, environment, usage, profile, version, validity policy, revocation policy, and protected custody reference without exposing private key material.

A request SHALL declare:

- request ID and requester identity;
- Authority Decision and Guardian state;
- operation: resolve, validate, present, bind, rotate, or revoke;
- Certificate or Identity Reference;
- expected subject and identity scope;
- purpose, environment, endpoint, and usage;
- required Profile and Trust-Anchor Set;
- required revocation freshness;
- required Clock Quality and uncertainty;
- correlation, expiry, and evidence requirement.

## 4. Validation Result

A result SHALL contain:

- `VALID`, `INVALID`, `INDETERMINATE`, or `REJECTED`;
- request and validation IDs;
- exact subject and issuer identity;
- Certificate Reference and immutable certificate digest;
- Profile and Trust-Anchor Set versions;
- chain result and constraints;
- key usage and extended usage disposition;
- name and scope binding disposition;
- validity-interval evaluation;
- revocation state, source, and freshness;
- algorithm and protection disposition;
- environment and purpose;
- time observation;
- bounded reason;
- evidence reference; and
- integrity protection.

Only `VALID` within the exact declared scope may support further authority evaluation. Acceptance and authority remain separate decisions.

## 5. Validation Rules

Validation SHALL verify:

- canonical certificate identity and integrity;
- complete chain to an Approved trust anchor;
- issuer constraints and permitted path;
- subject and expected-name binding;
- environment and purpose;
- key usage and extended usage;
- algorithm and parameter policy;
- validity interval using CON-015 uncertainty;
- current revocation state and required freshness;
- identity lifecycle;
- Provider and Profile state;
- authority and Guardian restrictions; and
- evidence completeness.

System or platform trust stores SHALL NOT be accepted merely because they are present.

## 6. Private-Key Boundary

Private-key operations SHALL be performed through CON-016 using an opaque reference.

This Contract SHALL NOT export raw private keys, allow certificate possession to imply private-key possession, or expose provider-specific certificate-store handles.

## 7. Bootstrap and Candidate Boundary

Candidate verification SHALL use test-only issuers, trust anchors, certificates, identities, revocation sources, and private keys.

No bootstrap or candidate identity material shall enter operational trust. External bootstrap attestations remain `BOOTSTRAP_EXTERNAL` and SHALL NOT become Falcon workload or authority identity.

## 8. Rotation, Revocation, and Failure

Rotation SHALL preserve subject continuity only when immutable identity attributes, issuer policy, scope, and governing Contract permit it. Otherwise, a new identity is required.

Revocation SHALL propagate according to consequence policy and SHALL prevent new reliance. Unknown or stale revocation state SHALL produce `INDETERMINATE` or rejection, never unrestricted validity.

Failure SHALL cause no anonymous fallback, weaker trust-anchor fallback, skipped revocation, platform-default acceptance, or silent certificate substitution.

## 9. Compatibility and Evidence

Provider-specific store types, paths, handles, status codes, and vendor objects SHALL remain behind the Adapter.

Provider replacement SHALL preserve Certificate Reference, subject binding, trust-anchor policy, revocation semantics, evidence, and no-downgrade behavior.

Evidence SHALL preserve the request, exact certificate digest, chain and anchors, expected identity, issuer, purpose, environment, algorithms, time, revocation source and freshness, result, policy versions, failures, and responsible authorities without exposing private keys or reusable credentials.

## 10. Normative Requirements

- **CON-018-REQ-001:** Components SHALL resolve and validate certificates and governed identities exclusively through this Contract.
- **CON-018-REQ-002:** Certificate validity SHALL be scoped to declared identity, purpose, environment, usage, policy, and time.
- **CON-018-REQ-003:** A valid certificate SHALL NOT by itself grant authority or trust.
- **CON-018-REQ-004:** Validation SHALL use an Approved Trust-Anchor Set and shall not inherit platform trust implicitly.
- **CON-018-REQ-005:** Subject, name, issuer, usage, algorithm, chain, time, and revocation SHALL be verified.
- **CON-018-REQ-006:** Temporal validity SHALL use CON-015 quality and uncertainty.
- **CON-018-REQ-007:** Stale, unknown, or conflicted revocation state SHALL restrict or reject reliance.
- **CON-018-REQ-008:** Private-key operations SHALL remain behind CON-016 and raw private keys SHALL NOT be exported.
- **CON-018-REQ-009:** Bootstrap and candidate identity material SHALL remain non-operational.
- **CON-018-REQ-010:** Failure SHALL cause no anonymous, weaker-anchor, skipped-revocation, or platform-default fallback.
- **CON-018-REQ-011:** Rotation SHALL not conceal an identity collision or incompatible subject change.
- **CON-018-REQ-012:** Provider-specific certificate objects SHALL NOT cross the Falcon boundary.
- **CON-018-REQ-013:** Provider replacement SHALL preserve identity and validation semantics.
- **CON-018-REQ-014:** Validation evidence SHALL be attributable, integrity-protected, and reconstructable.
- **CON-018-REQ-015:** `INDETERMINATE` SHALL NOT be treated as `VALID`.

## 11. Acceptance Examples

Acceptance requires valid and invalid chain cases; wrong subject, purpose, environment, usage, and issuer rejection; expired and not-yet-valid cases under uncertainty; revoked and stale-revocation cases; unknown anchor rejection; no platform-store inheritance; private-key non-export; rotation and collision cases; candidate isolation; Provider replacement; and complete reconstruction.

## 12. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-028 | 2026-07-25 |

This Approval admits CON-018 as a governed Foundation Contract. It does not activate an issuer, trust anchor, certificate, identity, or Provider; authorize implementation; or authorize financial activity.
