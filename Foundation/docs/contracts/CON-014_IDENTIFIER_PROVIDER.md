# CON-014 — Identifier Provider Contract

**Identifier:** CON-014  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-027  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; IDN-001; FCE-001; SEC-001; SEC-002; ADR-I006; ADR-I008; AMD-003; AMD-003-IR-001  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the sole governed boundary through which Falcon operational identifiers are issued, validated, classified, and evidenced.

Components SHALL NOT generate operational identifiers directly.

An issued identifier refers to a logical subject. It does not create that subject, establish identity, prove trust, grant authority, or establish time.

## 2. Participants

- **Requester:** the governed participant requesting an identifier for an eligible logical subject.
- **Identifier Provider:** the participant issuing and validating operational identifiers.
- **Time Provider:** the CON-015 participant supplying time when the active scheme requires it.
- **Randomness Provider:** the CON-019 participant supplying approved random material.
- **Subject Owner:** the participant accountable for binding the issued identifier to the logical subject.
- **Identifier Catalog Authority:** the authority governing classes and profiles under IDN-001.
- **Evidence Authority:** the participant preserving issuance, collision, and reconciliation evidence.

## 3. Provider Profile and State

Every Provider instance SHALL identify:

- Provider identity;
- Provider Profile ID and version;
- supported Identifier Profiles;
- environment and Deployment Profile;
- provider lifecycle state;
- active configuration identity;
- Runtime Epoch ID;
- dependencies and their states;
- capability declaration;
- Activation Decision reference, when active; and
- current health and trust state.

Provider lifecycle states are:

- `CANDIDATE`;
- `ACTIVE`;
- `RESTRICTED`;
- `SUSPENDED`;
- `RETIRED`; and
- `FORBIDDEN`.

Only `ACTIVE` may issue approved operational identifiers. `RESTRICTED` may issue only the explicitly preserved classes and profiles. Candidate output is test observation and SHALL be marked non-operational.

## 4. Issuance Request

An Issuance Request SHALL contain:

- request ID;
- authenticated requester identity;
- Identifier Class;
- Identifier Profile;
- logical-subject reference or creation context;
- continuity or new-issuance intent;
- scope;
- environment;
- exposure boundary;
- privacy classification;
- governing Contract;
- idempotency context where applicable;
- correlation and causation;
- requested evidence level;
- request time observation; and
- authority context.

The Requester SHALL NOT provide raw UUID bits, timestamps, randomness, version fields, or provider-specific generation parameters as the value to be issued.

## 5. Issuance Result

An Issuance Result SHALL contain:

- request ID;
- result ID;
- `ISSUED` or `REJECTED`;
- typed operational identifier, when issued;
- Identifier Class;
- Identifier Profile and scheme;
- uniqueness scope;
- issuer identity;
- environment;
- Runtime Epoch ID;
- canonical representation;
- privacy and exposure classification;
- issuance time observation reference;
- continuity disposition;
- collision-check disposition;
- governing Contract;
- reason and constraints;
- evidence reference; and
- integrity protection.

The embedded time portion of a time-ordered identifier SHALL NOT be represented as authoritative occurrence time.

## 6. Preconditions

Before issuance:

- the Requester SHALL be authenticated and permitted to request the class;
- the Identifier Class and Profile SHALL be known and applicable;
- the Provider SHALL be active for the declared environment and profile;
- required Time and Randomness Providers SHALL meet the active profile;
- the logical-subject intent and continuity rule SHALL be explicit;
- the exposure boundary SHALL be permitted;
- privacy requirements SHALL be satisfied;
- reserved and prohibited values SHALL be excluded;
- collision controls SHALL be available; and
- issuance evidence SHALL be preservable where required.

## 7. Postconditions

After successful issuance:

- exactly one canonical typed identifier is returned;
- the identifier is bound to its class, profile, issuer, environment, and scope;
- the Subject Owner may establish the identity binding separately;
- required evidence is immutable and attributable;
- retry behavior is governed by declared continuity and idempotency rules; and
- no authority, trust, admission, ownership, occurrence time, or causal order is inferred.

## 8. Continuity, Retry, and Attempts

Reuse of an identifier for the same immutable logical subject under the same class, scope, issuer, and governing Contract constitutes continuity.

Use for a different subject, class, scope, or conflicting immutable identity attributes constitutes collision.

Where a governing Contract requires idempotent continuity:

- retry of the same logical request SHALL preserve the logical identifier;
- every distinct execution or delivery attempt SHALL receive its appropriate distinct attempt identifier; and
- the Provider SHALL NOT infer sameness from payload similarity alone.

## 9. Collision

Suspected or confirmed collision SHALL:

- reject affected issuance or binding;
- contain affected use;
- preserve all conflicting evidence;
- notify the accountable Subject Owner;
- restrict affected authority where material;
- notify Health Monitoring and Self-Awareness;
- notify Guardian when capital, authority, evidence, or security may be harmed; and
- enter governed reconciliation.

Collision SHALL NOT be resolved by overwrite, last-write-wins, silent merge, relabeling, evidence deletion, or concealment through a replacement identifier.

## 10. Bootstrap Boundary

Before Provider Activation, external bootstrap identifiers MAY identify preparation and verification objects only when marked `BOOTSTRAP_EXTERNAL`.

Bootstrap identifiers SHALL:

- preserve external issuer, scheme, version, scope, and provenance;
- remain non-operational;
- never be reclassified as Falcon operational identifiers;
- never establish Falcon identity or authority; and
- be cross-linked, not replaced, when a later Falcon identifier is issued.

A `CANDIDATE` Provider MAY produce test identifiers only inside its authorized candidate environment. Such output SHALL NOT escape as operational identity or support its own Activation.

## 11. Privacy and Exposure

Before crossing an untrusted or public boundary, issuance SHALL evaluate:

- approximate time disclosure;
- activity-volume inference;
- cross-boundary correlation;
- enumeration;
- predictability;
- subject sensitivity;
- retention; and
- mapping risk.

Where the active profile does not approve exposure, issuance for that boundary SHALL be rejected. Internal identifiers SHALL NOT be exposed merely because their representation is technically portable.

## 12. Invariants

- Operational identifiers SHALL be issued only through this Contract.
- Identifier Class and Profile SHALL be explicit.
- An identifier SHALL NOT prove identity, trust, authority, time, order, freshness, or admission.
- Registered semantic identifiers SHALL NOT be replaced by operational identifiers.
- Identifier IDs and class meanings SHALL NOT be reassigned.
- Provider-specific representation SHALL NOT cross the Falcon boundary.
- A Provider SHALL NOT silently change scheme or profile.
- Candidate and bootstrap values SHALL remain distinguishable from operational identifiers.
- Material uncertainty SHALL cause rejection or restriction.

## 13. Rejection and Failure

Issuance SHALL be rejected when:

- requester identity or permission is unverified;
- class, profile, scope, environment, or scheme is unknown or inactive;
- the Provider is not operative for the request;
- required time, randomness, privacy, or evidence conditions fail;
- the request attempts caller-controlled raw generation;
- the value would be nil, reserved, noncanonical, or wrong-version;
- continuity cannot be distinguished from collision;
- external exposure is unapproved;
- bootstrap material is presented as operational; or
- integrity or provenance cannot be established.

Failure SHALL return a bounded reason and evidence reference without issuing a fallback identifier outside policy.

## 14. Compatibility

- Canonical representation SHALL be governed solely by FCE-001.
- Classes and profiles SHALL be governed solely by IDN-001.
- Consumers SHALL NOT depend on a language, runtime, database, operating system, UUID API, or vendor.
- Unknown mandatory fields or unsupported profiles SHALL cause rejection.
- Profile migration SHALL create explicit evidence and SHALL NOT reinterpret existing identifiers.
- A future identifier scheme may replace the active scheme without changing requesting components when an Approved profile permits it.

## 15. Evidence

Required evidence SHALL preserve:

- request and result;
- requester and Provider identity;
- class, profile, scheme, scope, and environment;
- dependency profiles;
- continuity decision;
- privacy and exposure decision;
- collision checks and outcomes;
- time observation reference;
- candidate or active state;
- bootstrap classification where applicable;
- governing configuration and policy;
- canonical representation and digest;
- failures, restrictions, and reconciliations; and
- responsible authorities.

Evidence is governed as Trust Objects under SEC-002.

## 16. Security

- Requests and results SHALL be authenticated, integrity-protected, replay-resistant, and time-bounded where consequence requires.
- Random material SHALL come only from the Approved Randomness Provider profile.
- Caller influence over identifier entropy or time fields SHALL be rejected unless an explicit test profile permits synthetic input.
- Test profiles SHALL remain isolated and non-operational.
- Collision or abnormal issuance behavior SHALL trigger protective monitoring.
- Identifier possession SHALL NOT authenticate a subject or grant permission.

## 17. Normative Requirements

- **CON-014-REQ-001:** Components SHALL obtain operational identifiers exclusively through the Falcon Identifier Provider Contract.
- **CON-014-REQ-002:** Every issuance SHALL declare an Approved Identifier Class and Profile.
- **CON-014-REQ-003:** Only an applicable `ACTIVE` or explicitly bounded `RESTRICTED` Provider may issue operational identifiers.
- **CON-014-REQ-004:** Candidate and `BOOTSTRAP_EXTERNAL` identifiers SHALL NOT become Falcon operational identifiers.
- **CON-014-REQ-005:** Canonical identifier representation SHALL conform to FCE-001.
- **CON-014-REQ-006:** Identifier semantics, classes, profiles, and exposure rules SHALL conform to IDN-001.
- **CON-014-REQ-007:** The Provider SHALL use the Falcon Time and Randomness Provider Contracts when required by the active profile.
- **CON-014-REQ-008:** Caller-controlled raw identifier generation parameters SHALL be rejected.
- **CON-014-REQ-009:** Identity continuity SHALL remain distinct from identity collision.
- **CON-014-REQ-010:** Collision SHALL cause rejection, containment, evidence preservation, and governed reconciliation.
- **CON-014-REQ-011:** Retry SHALL preserve logical identity only where the governing Contract requires continuity.
- **CON-014-REQ-012:** An issued identifier SHALL NOT establish identity, trust, authority, admission, time, or order.
- **CON-014-REQ-013:** Unapproved boundary exposure SHALL be denied.
- **CON-014-REQ-014:** Provider scheme replacement SHALL NOT change requester Contracts.
- **CON-014-REQ-015:** Issuance and collision evidence SHALL be attributable and reconstructable.
- **CON-014-REQ-016:** Failure SHALL NOT cause silent fallback to direct or unapproved identifier generation.

## 18. Acceptance Examples

Acceptance requires verified examples showing:

- valid issuance for every approved Foundation class;
- canonical cross-platform equivalence;
- rejection of unknown class, profile, environment, and scheme;
- rejection of nil, reserved, wrong-version, and noncanonical values;
- rejection of caller-supplied raw generation fields;
- correct continuity across governed retry;
- distinct attempt identity;
- detection and containment of collision;
- denial of unapproved external exposure;
- isolation of candidate output;
- preservation and cross-linking of `BOOTSTRAP_EXTERNAL` identifiers;
- failure when Time or Randomness Provider quality is insufficient;
- inability of an identifier to grant identity or authority; and
- replaceability of the issuing mechanism behind the Contract.

## 19. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-027 | 2026-07-25 |

This Approval admits CON-014 as a governed Foundation Contract. It does not activate an Identifier Provider or Profile, issue an operational identifier, authorize implementation, or authorize financial activity.
