# IDN-001 — Foundation Identifier Catalog

**Identifier:** IDN-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-034  
**Amendment Package:** AMD-003  
**Owner:** Falcon Specification Authority  
**Governing Authority:** ADR-I006; ADR-I008; AMD-003; IMP-001 v1.2; CON-008 v1.1; CON-014; CON-020; CON-021; VPL-BST-003  
**Implementation Authority:** Not Granted
**Supersedes:** IDN-001 v1.0  
**Superseded By:** None

## 1. Purpose

This Catalog defines how preparation and candidate-verification objects are identified before the Falcon Identifier Provider is active, and the exact boundary after which Falcon operational identifiers become mandatory.

It prevents external bootstrap identifiers and candidate-produced identifiers from being mistaken for Falcon operational identity.

## 2. Preserved Decisions

Unless explicitly amended here, IDN-001 v1.0 remains controlling for:

- identifier and identity separation;
- operational, semantic, artifact, human, instance, and workload identity classes;
- UUIDv7 internal profile;
- typed canonical representation;
- operational Identifier Class registry;
- privacy and exposure;
- Runtime Epoch identity;
- continuity and collision;
- Provider obligations;
- canonical encoding ownership by FCE-001;
- lifecycle and evolution;
- evidence and failure; and
- the rule that an identifier does not establish identity, trust, authority, admission, time, order, or ownership.

No existing Identifier Class, Profile ID, or meaning is changed.

## 3. External Bootstrap Identifier

Before the Falcon Identifier Provider is active, preparation and candidate-verification objects MAY use an external identifier only when represented by a governed External Bootstrap Identifier Record.

The Record SHALL contain:

- external identifier value;
- classification `BOOTSTRAP_EXTERNAL_ID`;
- external scheme and version;
- issuing mechanism and issuer identity;
- observing environment identity;
- purpose and scope;
- subject reference;
- issuance or first-observation evidence;
- external time reference;
- collision-check disposition;
- canonical protected representation;
- provenance and custody;
- applicable CON-020 Context;
- constraints and expiry where applicable; and
- explicit non-equivalence to Falcon operational identity.

`BOOTSTRAP_EXTERNAL_ID` is an evidence classification. It is not a Falcon operational Identifier Class or Profile.

## 4. Permitted Bootstrap Subjects

External bootstrap identifiers MAY identify:

- acquisition transaction;
- downloaded source object;
- tool or dependency candidate;
- offline bundle candidate;
- environment image candidate;
- environment instance under preparation;
- bootstrap harness execution;
- Provider candidate;
- verification session;
- external evidence item;
- external transfer receipt;
- candidate Manifest; and
- another preparation or verification subject explicitly permitted by CON-020.

They SHALL NOT identify a Falcon operational actor, authority holder, operational message, operational decision, admitted component, operational Runtime Epoch, production key, production secret, financial subject, or capital-affecting action.

## 5. External Issuance Rules

The issuing mechanism SHALL:

- be declared in the Bootstrap Execution Context;
- use a known scheme and version;
- provide sufficient collision resistance for the declared scope;
- reject nil, empty, reserved, malformed, or reused values;
- preserve the exact issuer and environment;
- prevent silent scheme substitution;
- produce external provenance;
- remain independent of the candidate being identified where material; and
- avoid claiming Falcon Provider conformance.

Content identity MAY supplement object reference but SHALL NOT replace subject, issuer, scope, provenance, or authority.

## 6. Candidate Identifier Provider

An Identifier Provider candidate MAY issue test identifiers only:

- under an Enabling-Provider Candidate Authority Instrument;
- inside a `CANDIDATE_PROVIDER_VERIFY` context;
- under a candidate-only Identifier Profile;
- for synthetic subjects;
- using test Time and Randomness dependencies;
- with explicit `CANDIDATE` output lifecycle;
- as the subject of VPL-BST-003; and
- with independent external evidence.

Candidate output SHALL NOT:

- become an operational identifier;
- identify an operational Falcon subject;
- satisfy an active Provider dependency;
- establish identity or authority;
- support production security material;
- validate the candidate's own Profile;
- enter a Foundation Baseline Manifest as active; or
- escape the candidate environment as trusted identity.

## 7. Activation Boundary

The Falcon Identifier Provider becomes mandatory only after:

1. CON-014 is active as the governing Contract;
2. the exact Provider candidate and Identifier Profile are identified;
3. VPL-BST-003 produces `PASS`;
4. required evidence is complete and valid;
5. independent evaluation is accepted for the declared scope;
6. a competent Profile Activation Authority issues an Activation Decision;
7. the exact Provider Profile and Environment Profile become `ACTIVE`; and
8. the effective Activation boundary is recorded.

After that boundary:

- every new Falcon operational object SHALL obtain its identifier through CON-014;
- direct platform, runtime, database, library, or caller generation is prohibited;
- `BOOTSTRAP_EXTERNAL_ID` SHALL not satisfy an operational identifier field;
- candidate output SHALL not satisfy an operational identifier field; and
- failure of the active Provider SHALL cause restriction, not external fallback.

## 8. Historical Cross-Linking

When an external bootstrap subject later receives a Falcon operational identifier:

- the new identifier SHALL be issued through CON-014;
- the original external identifier SHALL remain unchanged;
- an immutable import or cross-link record SHALL reference both;
- source and target subjects SHALL be verified explicitly;
- external origin and limitations SHALL remain visible;
- the cross-link SHALL not claim that both identifiers were issued by the same authority;
- external observed time SHALL remain separate from Falcon import time;
- collision and identity-continuity rules SHALL apply; and
- the operation SHALL produce CON-008 and CON-021 evidence.

Cross-linking is traceability. It does not rewrite history or establish identity continuity by itself.

## 9. Bootstrap Environment Identity

Before Provider Activation, an environment instance may have an external environment identifier classified `BOOTSTRAP_EXTERNAL_ID`.

After Activation:

- each new Falcon runtime environment instance SHALL receive a Falcon `instance` identifier;
- each new proven Runtime Epoch SHALL receive a Falcon `runtime-epoch` identifier;
- external and Falcon environment identities SHALL remain cross-linked;
- the embedded UUID timestamp SHALL not establish environment start time; and
- monotonic continuity SHALL remain governed by TIM-001 and CON-015.

An existing external environment SHALL not become a Falcon operational runtime merely by receiving a new identifier.

## 10. Failure and Reconciliation

Bootstrap identity failure includes:

- missing issuer;
- unknown scheme or version;
- malformed or reserved value;
- duplicate or collision;
- subject mismatch;
- conflicting immutable attributes;
- missing context;
- origin loss;
- candidate value presented as active;
- external value presented as Falcon-native; or
- unverifiable cross-link.

Failure SHALL:

- reject affected reliance;
- contain the subject;
- preserve all conflicting evidence;
- restrict dependent authority;
- notify Health Monitoring and Self-Awareness;
- notify Guardian when material harm is possible; and
- enter governed reconciliation.

Reconciliation SHALL NOT overwrite, relabel, merge, silently select, or hide the conflict through replacement issuance.

## 11. Evidence

Identifier evidence SHALL additionally preserve:

- bootstrap or candidate classification;
- issuer mechanism and version;
- environment and context;
- candidate subject and Profile;
- Activation boundary;
- original and Falcon identifiers;
- import and cross-link decision;
- collision and continuity disposition;
- external and Falcon time references;
- limitations;
- responsible authorities; and
- immutable lineage.

Approval, signing, importing, or cross-linking SHALL NOT upgrade the identifier's origin or authority.

## 12. Provider Failure After Activation

After the operational boundary, Identifier Provider failure SHALL:

- prevent issuance of new operational identifiers;
- prevent direct or external fallback;
- preserve existing identifiers without reissuance;
- mark affected requested operations as rejected or uncertain according to their Contracts;
- restrict dependent authority;
- notify Health Monitoring and Self-Awareness;
- notify Guardian where required; and
- require independently governed restoration.

Provider return SHALL NOT automatically restore trust or authority.

## 13. Requirements Added

- **IDN-001-REQ-031:** Pre-Activation identifiers SHALL be classified `BOOTSTRAP_EXTERNAL_ID` and SHALL not be Falcon operational identifiers.
- **IDN-001-REQ-032:** Every bootstrap identifier SHALL preserve scheme, version, issuer, environment, scope, context, provenance, and limitations.
- **IDN-001-REQ-033:** `BOOTSTRAP_EXTERNAL_ID` SHALL not be registered as a Falcon operational Identifier Class or Profile.
- **IDN-001-REQ-034:** External bootstrap identifiers SHALL be limited to explicitly permitted preparation and verification subjects.
- **IDN-001-REQ-035:** Candidate Provider output SHALL remain `CANDIDATE`, synthetic, isolated, non-operational, and incapable of self-Activation.
- **IDN-001-REQ-036:** Falcon operational identifiers SHALL become mandatory only at the recorded Provider Activation boundary.
- **IDN-001-REQ-037:** After Activation, direct, external, platform, runtime, database, or library fallback generation SHALL be prohibited.
- **IDN-001-REQ-038:** Historical external identifiers SHALL be preserved and cross-linked, never replaced or reclassified.
- **IDN-001-REQ-039:** Cross-linking SHALL not establish identity continuity, common issuance authority, trust, or authority by itself.
- **IDN-001-REQ-040:** Bootstrap environment identity SHALL not become operational runtime identity merely through cross-linking or new identifier issuance.
- **IDN-001-REQ-041:** Bootstrap identity collision or origin loss SHALL cause rejection, containment, evidence preservation, and reconciliation.
- **IDN-001-REQ-042:** Identifier evidence SHALL preserve the exact Activation boundary and original lineage.
- **IDN-001-REQ-043:** Active Provider failure SHALL cause restriction and SHALL not cause external or direct-generation fallback.
- **IDN-001-REQ-044:** Provider return SHALL not automatically restore trust, authority, or issuance permission.
- **IDN-001-REQ-045:** Approval of IDN-001 v1.1 SHALL not activate an Identifier Profile or Provider or issue an identifier.

## 14. Conformance Evidence Added

Activation requires evidence that:

- external identifiers are accepted only within declared bootstrap scope;
- missing issuer, scheme, environment, context, or provenance is rejected;
- external identifiers cannot satisfy operational Contract fields;
- candidate output is distinguishable and cannot escape as trusted identity;
- VPL-BST-003 controls the candidate case;
- the Activation boundary is exact and reconstructable;
- post-Activation operational issuance uses only CON-014;
- direct and external fallback is prevented;
- cross-linking preserves both identifiers and external origin;
- cross-linking does not establish identity continuity automatically;
- collision and subject mismatch trigger containment;
- environment and Runtime Epoch identities remain separate;
- Provider failure restricts issuance and dependent authority; and
- Provider return requires governed restoration.

## 15. Required Before Identifier Provider Activation

The operational Identifier Profile SHALL remain non-active until:

1. IDN-001 v1.1 is Approved;
2. CON-014 and CON-019 are Approved and applicable;
3. FCE-001 canonical identifier encoding is active for the declared scope;
4. the Time and Randomness candidate dependencies are verified;
5. VPL-BST-003 produces `PASS`;
6. the exact Provider, Profile, environment, configuration, and artifact identities are known;
7. the Evidence Requirement Set is complete;
8. validity and security review are accepted;
9. a competent Authority Instrument permits Activation; and
10. a separate exact Activation Decision is recorded.

## 16. Supersession

- IDN-001 v1.1 supersedes v1.0;
- existing Identifier Classes and Profile meanings remain unchanged;
- no existing external, candidate, local, or directly generated identifier is grandfathered;
- historical evidence retains its original origin and limitations;
- no Provider or Profile becomes active through the Catalog amendment; and
- no identifier is issued by the version change.

## 17. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-034 | 2026-07-25 |

This Approval activates IDN-001 v1.1 as the controlling Catalog and archives v1.0.

It does not:

- activate an Identifier Provider or Profile;
- issue a Falcon operational identifier;
- convert a bootstrap or candidate identifier;
- authorize candidate construction or verification execution;
- authorize implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
