# CON-010 — Foundation Baseline Manifest Contract

**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-008  
**Owner:** Falcon Release Authority  
**Governing Specifications:** SYS-001, SEC-001, SYS-007  
**Applicable ADRs:** ADR-F006, ADR-F007  
**Supersedes:** None  
**Superseded By:** CON-010 v1.1 under GOV-030

## 1. Purpose

This Contract defines the signed manifest that binds an approved FRS-001 release to its governance baseline, artifacts, Contracts, schemas, configuration baseline, identity authority, cryptographic profile, validity, and revocation inputs.

## 2. Participants

- **Issuer:** the authorized Foundation Release Authority.
- **Verifier:** Falcon trusted bootstrap.
- **Custodian:** the authority responsible for preserving the released manifest and trust anchor.
- **Reviewer:** an authorized independent verifier.

## 3. Authoritative Input

The Issuer receives approved, immutable identities for:

- release and governance baseline;
- required executable and non-executable artifacts;
- approved Specifications, Standards, ADRs, Contracts, and verification plans;
- released schemas;
- configuration baseline;
- permitted identity issuer;
- cryptographic profile;
- validity interval; and
- revocation-source identity and freshness rule.

## 4. Manifest Fields

Every manifest SHALL contain:

- manifest ID and schema version;
- release ID and release version;
- issuer identity and authority reference;
- Vision, Constitution, governance, Specification, Standard, ADR, Contract, and verification-baseline identities;
- artifact entries containing canonical identity, version, role, digest, classification, and required/optional status;
- schema entries containing schema ID, version, digest, owner, and compatibility status;
- configuration-baseline identity and digest;
- permitted instance-identity issuer;
- permitted workload-identity issuer;
- cryptographic-profile ID;
- revocation-source ID and maximum acceptable age;
- issue, not-before, expiry, and approval time;
- superseded-manifest reference when applicable; and
- manifest integrity evidence and signature.

## 5. Preconditions

- Every referenced governed document SHALL be Approved or Accepted as appropriate.
- Every required artifact and schema SHALL have one canonical identity and integrity digest.
- The Issuer SHALL possess explicit release-signing authority.
- The signing identity SHALL be valid, protected, and not revoked.

## 6. Postconditions

Successful verification establishes only that the presented baseline is the exact approved baseline represented by the manifest.

It SHALL NOT by itself:

- authorize unrestricted startup;
- admit a component;
- grant operational authority;
- establish current Fitness to Operate; or
- authorize financial activity.

## 7. Obligations

- **CON-010-REQ-001:** The manifest SHALL bind every required FRS-001 artifact to one integrity identity.
- **CON-010-REQ-002:** The manifest SHALL bind the exact governing-document and schema versions used by the release.
- **CON-010-REQ-003:** Manifest verification SHALL establish signature, issuer authority, validity, revocation freshness, schema validity, and referenced-artifact integrity separately.
- **CON-010-REQ-004:** A missing required artifact or reference SHALL prevent unrestricted startup.
- **CON-010-REQ-005:** An unknown, modified, expired, revoked, wrong-environment, or integrity-failed manifest SHALL be rejected.
- **CON-010-REQ-006:** A valid manifest SHALL NOT be treated as authorization for an action.
- **CON-010-REQ-007:** Manifest replacement SHALL create a new identity and preserve the superseded manifest.
- **CON-010-REQ-008:** Verification evidence SHALL identify every accepted and rejected check without exposing private signing material.
- **CON-010-REQ-009:** Clock uncertainty beyond the approved validity tolerance SHALL prevent unrestricted startup.
- **CON-010-REQ-010:** Revocation status older than the declared maximum age SHALL be treated as unknown.

## 8. Errors and Rejection

Rejection classes SHALL distinguish malformed manifest, unsupported schema, unknown issuer, insufficient issuer authority, invalid signature, not-yet-valid, expired, revoked, stale revocation data, artifact mismatch, schema mismatch, missing required entry, configuration mismatch, and clock uncertainty.

## 9. Security and Evidence

Private signing material SHALL remain outside the manifest. Verification SHALL produce a CON-008 evidence record correlated to bootstrap identity.

## 10. Compatibility and Evolution

Additive optional fields require explicit compatibility. Changed required meaning requires a new schema version. A new manifest version does not silently approve a new release.

## 11. Acceptance Examples

- valid approved manifest and exact artifacts: accepted for continued bootstrap checks;
- valid signature with modified artifact: rejected;
- valid artifact set with expired manifest: rejected;
- valid manifest with stale revocation input: rejected as unknown trust;
- valid manifest from an unauthorized issuer: rejected.

## 12. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-008 | 2026-07-24 |
