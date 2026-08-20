# CON-012 — Authority Instrument Contract

**Identifier:** CON-012  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-026  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-AUT-001; AUT-001 v1.1; SEC-002; ADR-I007; ADR-I008; AMD-003; AMD-003-IR-001  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the observable boundary by which authority is assigned, restricted, suspended, restored, revoked, expired, terminated, challenged, and evidenced within an established Falcon jurisdiction.

An Authority Instrument records authority. It does not create jurisdiction, prove its own legitimacy, execute an action, or grant authority beyond its governing source.

## 2. Participants

- **Issuing Authority:** the competent authority that issues the Instrument.
- **Authority Holder:** the identified recipient of the authority.
- **Authority Registry:** the governed participant that preserves the Instrument and its lifecycle.
- **Authority Evaluator:** the participant that verifies whether the Instrument may support a decision.
- **Protective Authority:** Guardian or another competent authority that may lawfully restrict exercise.
- **Review Authority:** the competent independent authority for material challenges or restoration.
- **Evidence Authority:** the participant responsible for preserving the resulting Trust Objects.

No participant acquires authority merely by participating in this Contract.

## 3. Authority

Issuance requires:

- an established jurisdiction;
- a competent Issuing Authority within that jurisdiction;
- an active and verified source Authority Chain;
- authority to issue the declared decision classes;
- satisfaction of applicable separation-of-duty rules; and
- an approved Authority Instrument type.

Delegation-specific behavior is governed by CON-013.

## 4. Authority Instrument

An Authority Instrument SHALL contain:

- Instrument ID;
- Instrument type;
- jurisdiction ID;
- Jurisdictional Source reference;
- source Authority Instrument reference, when applicable;
- issuing authority identity;
- Authority Holder identity;
- decision classes;
- action scope;
- subject and resource scope;
- purpose;
- environment scope;
- consequence ceiling;
- effective time;
- expiry or review condition;
- conditions;
- constraints;
- prohibitions;
- dependency and approval requirements;
- separation-of-duty and independence requirements;
- delegation and redelegation rights;
- suspension, revocation, termination, and restoration authorities;
- evidence obligations;
- challenge and review path;
- lifecycle state;
- version;
- provenance;
- canonical digest;
- integrity protection; and
- issuance evidence reference.

Every dimension is independently binding. Permission in one dimension SHALL NOT imply permission in another.

## 5. Instrument Types

Approved governance may define Instrument types including:

- jurisdiction assignment;
- authority appointment;
- authority restriction;
- authority suspension;
- authority restoration;
- authority revocation;
- authority termination;
- temporary authority;
- emergency containment authority;
- preparation authority;
- Provider candidate authority;
- verification execution authority;
- Profile Activation authority;
- Foundation Implementation Authority; and
- operational authority.

An Instrument type SHALL NOT be interpreted more broadly than its declared fields and governing policy.

## 6. Lifecycle

The governed lifecycle states are:

- `DRAFT`;
- `PENDING_ACCEPTANCE`;
- `ACTIVE`;
- `RESTRICTED`;
- `SUSPENDED`;
- `EXPIRED`;
- `REVOKED`;
- `TERMINATED`; and
- `ARCHIVED`.

Only `ACTIVE` may grant the full declared authority. `RESTRICTED` may grant only its explicitly preserved subset. All other states are non-permissive.

A lifecycle transition SHALL:

- be requested by an attributable actor;
- be authorized by a competent authority;
- identify the prior state;
- identify the new state;
- state the reason and effective time;
- preserve the previous record;
- produce immutable evidence; and
- propagate consequences to dependent authority.

## 7. Preconditions

Before issuance or a material lifecycle transition:

- all required identities SHALL be authenticated;
- jurisdiction SHALL be valid and applicable;
- the Issuing Authority SHALL be competent for the Instrument type;
- the complete Authority Chain SHALL be verified;
- the source authority SHALL be exercisable;
- the proposed scope SHALL NOT exceed the source;
- required independence SHALL be satisfied;
- material conflicts SHALL be resolved or governed;
- time and environment SHALL be sufficiently trustworthy;
- no higher prohibition SHALL apply; and
- the proposed record SHALL pass canonical and integrity validation.

## 8. Postconditions

On acceptance:

- the immutable Instrument version is preserved;
- its state and effective scope are unambiguous;
- its Authority Chain is reconstructable;
- the Authority Holder can be distinguished from the Issuing Authority;
- dependent evaluators can discover applicable restriction, suspension, revocation, expiry, or termination;
- all evidence is attributable; and
- no action is executed merely because the Instrument exists.

## 9. Invariants

- Jurisdiction precedes authority.
- An Instrument SHALL NOT create, widen, merge, transfer, or reinterpret jurisdiction.
- An issuer SHALL NOT issue more authority than it can lawfully exercise.
- An Instrument SHALL NOT waive a higher obligation.
- An Instrument SHALL NOT approve its own legitimacy.
- Technical capability, access, urgency, prior success, or possession SHALL NOT substitute for authority.
- Authority state SHALL NOT be inferred from absence of contrary evidence.
- Historical versions and adverse lifecycle events SHALL NOT be erased.
- A narrower restriction SHALL take precedence over a conflicting subordinate permission.
- Authority uncertainty SHALL reduce or deny authority.

## 10. Rejection and Failure

The Instrument SHALL be rejected or treated as non-permissive when:

- jurisdiction is absent, invalid, conflicted, or unverifiable;
- the issuer lacks competence;
- the Authority Chain is incomplete or invalid;
- the scope exceeds the source;
- identity is missing or untrustworthy;
- a required condition, approval, or independence control is absent;
- the record is expired, suspended, revoked, terminated, or archived;
- canonical representation or integrity cannot be verified;
- conflicting versions cannot be authoritatively resolved;
- required evidence cannot be preserved; or
- governing policy cannot determine a lawful result.

Rejection SHALL include a bounded reason and evidence reference without exposing protected security material.

## 11. Compatibility

- Instrument consumers SHALL depend on Falcon-governed fields and semantics, not a storage, transport, cryptographic, or platform representation.
- Unknown mandatory fields, unknown Instrument types, or unsupported governing versions SHALL cause rejection or explicit restriction.
- Additive optional fields MAY be ignored only when governing policy declares them non-material.
- A new version SHALL NOT reinterpret an existing Instrument ID or historical version.
- Supersession SHALL create a new immutable version and explicit lineage.

## 12. Evidence

Evidence SHALL preserve:

- original request;
- authoritative inputs;
- applicable jurisdiction and Authority Chain;
- issuer and holder identity;
- policy and context versions;
- validation results;
- issued canonical Instrument;
- lifecycle transitions;
- acceptance where required;
- restrictions and protective actions;
- challenges and resolutions;
- dependent authority affected;
- decision time and time quality;
- integrity proof; and
- responsible authorities.

Authority Instruments and their evidence are Trust Objects governed by SEC-002. Classification as a Trust Object does not establish trust.

## 13. Security

- Confidential fields SHALL receive protection appropriate to their classification.
- Integrity and provenance SHALL be verifiable before reliance.
- Replay, substitution, rollback, equivocation, and unauthorized supersession SHALL be detected and rejected.
- Compromise of an issuer, holder, registry, or evaluator SHALL trigger bounded restriction according to governing policy.
- No secret, private credential, or unrestricted personal data SHALL be embedded unless explicitly required and protected.
- Signature validity SHALL NOT alone prove jurisdiction or authority.

## 14. Normative Requirements

- **CON-012-REQ-001:** Every Authority Instrument SHALL identify an established jurisdiction and its Jurisdictional Source.
- **CON-012-REQ-002:** Every Instrument SHALL identify a competent Issuing Authority and an Authority Holder.
- **CON-012-REQ-003:** Every Instrument SHALL preserve a complete, verifiable Authority Chain.
- **CON-012-REQ-004:** An Instrument SHALL NOT create or expand jurisdiction.
- **CON-012-REQ-005:** Issued authority SHALL NOT exceed the issuer's lawful authority.
- **CON-012-REQ-006:** Authority dimensions SHALL remain independently bounded.
- **CON-012-REQ-007:** Only `ACTIVE` or the explicit subset of `RESTRICTED` authority MAY support permission.
- **CON-012-REQ-008:** Every material lifecycle transition SHALL require competent authority and immutable evidence.
- **CON-012-REQ-009:** Upstream restriction, suspension, revocation, expiry, or termination SHALL constrain dependent authority.
- **CON-012-REQ-010:** Invalid, conflicted, incomplete, stale, or unverifiable authority SHALL be non-permissive.
- **CON-012-REQ-011:** Historical Instruments and lifecycle evidence SHALL be immutable and reconstructable.
- **CON-012-REQ-012:** An Instrument SHALL NOT execute or approve the governed action.
- **CON-012-REQ-013:** Material restoration SHALL require a new decision and the required independent confirmation.
- **CON-012-REQ-014:** Material challenges SHALL have a competent independent resolution path.
- **CON-012-REQ-015:** Signature or possession SHALL NOT substitute for verified authority.

## 15. Acceptance Examples

Acceptance requires verified examples showing:

- valid issuance inside established jurisdiction;
- denial where jurisdiction is absent;
- denial where issuer authority is insufficient;
- denial of scope exceeding the source;
- independent enforcement of purpose, environment, consequence, and time;
- transition from `ACTIVE` to `SUSPENDED`;
- immediate effect of revocation on dependent authority;
- automatic expiry;
- restoration requiring new evidence and independent confirmation;
- rejection of modified or replayed Instrument material;
- preservation of superseded history;
- restriction under uncertain time or integrity; and
- inability of the Instrument to execute an action by itself.

## 16. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-026 | 2026-07-25 |

This Approval admits CON-012 as a governed Foundation Contract. It does not issue an Authority Instrument, appoint an Authority Holder, activate any authority, authorize implementation, or authorize financial activity.
