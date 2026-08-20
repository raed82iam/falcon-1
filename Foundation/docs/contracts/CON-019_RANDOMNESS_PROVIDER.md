# CON-019 — Randomness Provider Contract

**Identifier:** CON-019  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-028  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; CRY-001; IDN-001; GOV-SEC-001; DESIGN-SEC-001; ADR-I005; ADR-I006; ADR-I008; AMD-003; AMD-003-IR-001  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the exclusive governed boundary for cryptographically strong random material used by Falcon.

Components SHALL NOT obtain security-relevant randomness directly from a runtime, operating system, device, library, platform, network, cloud, or vendor API.

Randomness is purpose-bound input. Strong randomness alone does not guarantee nonce uniqueness, key safety, identity, authority, or correctness.

## 2. Participants

- **Requester:** the authenticated participant requesting random material.
- **Randomness Provider:** the Falcon boundary selecting and verifying an approved source.
- **Entropy Source:** the protected platform or external source behind the Adapter.
- **Security Authority:** the authority governing profiles and purposes.
- **Health Monitoring and Guardian:** the participants receiving material source-failure signals.
- **Evidence Authority:** the participant preserving non-secret evidence.

## 3. Request and Result

A request SHALL declare:

- request ID and requester identity;
- Authority Decision and Guardian state;
- Randomness Profile;
- purpose: key, nonce, salt, permitted identifier material, challenge, or governed test;
- requested length;
- domain, environment, and identity scope;
- uniqueness or collision requirements;
- prediction-resistance or reseed requirements where applicable;
- time and expiry;
- correlation and evidence requirements.

A result SHALL contain:

- `PRODUCED`, `REJECTED`, or `FAILED`;
- request and operation IDs;
- Profile, purpose, length, domain, and environment;
- Provider and source-class identity;
- Runtime Epoch ID;
- capability and health disposition;
- protected random output or direct bounded consumer handoff;
- constraints and bounded reason;
- non-secret evidence reference; and
- integrity protection.

Raw random output SHALL appear in evidence or logs under no circumstances.

## 4. Purpose and Source Enforcement

The Provider SHALL:

- use only an active, verified Randomness Profile;
- enforce allowed purpose, length, domain, environment, and requester;
- obtain material only from an approved cryptographic source;
- prevent caller-supplied entropy except in an isolated governed test profile;
- prevent reuse of returned material;
- detect source or capability failure to the extent supported by the profile;
- fail closed without deterministic or weak fallback; and
- expose no platform-specific source object.

Nonce generation SHALL also enforce the governing uniqueness strategy and operation bound. A strong random source SHALL NOT substitute for required durable counter or collision controls.

## 5. Bootstrap and Candidate Boundary

Candidate verification MAY use declared synthetic or controlled randomness only in isolated test profiles.

Synthetic output SHALL be unmistakably non-operational and SHALL NOT generate operational keys, credentials, identifiers, nonces, roots, or certificates.

Candidate Providers SHALL NOT attest their own source quality or Activation.

## 6. Failure and Health

Source failure, repeated output, capability loss, unexpected deterministic behavior, health-test failure, profile mismatch, or uncertain source identity SHALL:

- reject the request;
- return no partial material;
- cause no weak, cached, alternate, or silent fallback;
- preserve non-secret diagnostic evidence;
- notify Health Monitoring;
- notify Guardian where consequence may be material; and
- restrict dependent operations until governed restoration.

Provider return SHALL NOT automatically restore trust. Restoration requires current evidence and the required independent decision.

## 7. Compatibility and Evidence

Source APIs, device handles, provider types, and vendor error codes SHALL remain behind the Adapter.

Source replacement SHALL preserve profile, purpose, quality, isolation, evidence, and no-fallback semantics and requires capability verification in the active environment.

Evidence SHALL preserve request identity, authority, Guardian state, profile, purpose, length, domain, environment, Provider and source-class identity, Runtime Epoch, capability, result, failures, health events, and authorities. It SHALL NOT preserve the generated material or data enabling its reconstruction.

## 8. Normative Requirements

- **CON-019-REQ-001:** Components SHALL obtain security-relevant randomness exclusively through this Contract.
- **CON-019-REQ-002:** Every request SHALL declare an Approved Profile, purpose, length, domain, and environment.
- **CON-019-REQ-003:** Only an active verified cryptographic source may produce operational material.
- **CON-019-REQ-004:** Caller-supplied entropy SHALL be prohibited outside an isolated governed test profile.
- **CON-019-REQ-005:** Random output SHALL NOT be logged, evidenced, reused, or exposed beyond its bounded consumer.
- **CON-019-REQ-006:** Strong randomness SHALL NOT replace nonce-uniqueness, counter, collision, or operation-bound controls.
- **CON-019-REQ-007:** Source or health failure SHALL release no partial material and cause no weak or silent fallback.
- **CON-019-REQ-008:** Synthetic and candidate output SHALL remain non-operational.
- **CON-019-REQ-009:** Provider-specific source objects SHALL NOT cross the Falcon boundary.
- **CON-019-REQ-010:** Source replacement SHALL preserve Falcon semantics and require capability verification.
- **CON-019-REQ-011:** Material source anomalies SHALL trigger monitoring and proportionate protective restriction.
- **CON-019-REQ-012:** Provider return SHALL NOT automatically restore trust.
- **CON-019-REQ-013:** Evidence SHALL remain non-secret, attributable, and reconstructable.
- **CON-019-REQ-014:** Random material SHALL NOT by itself establish identity, authority, time, or uniqueness.

## 9. Acceptance Examples

Acceptance requires valid output for every approved purpose and length; source capability verification; rejection of unknown profiles and purposes; caller-entropy rejection; absence from logs and evidence; no reuse; nonce-policy enforcement; source-failure and repeated-output containment; no weak fallback; candidate isolation; source replacement; restoration control; and non-secret evidence reconstruction.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-028 | 2026-07-25 |

This Approval admits CON-019 as a governed Foundation Contract. It does not activate a source or Provider, generate operational material, authorize implementation, or authorize financial activity.
