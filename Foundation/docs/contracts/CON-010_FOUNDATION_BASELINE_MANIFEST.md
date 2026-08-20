# CON-010 — Foundation Baseline Manifest Contract

**Identifier:** CON-010  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-030  
**Owner:** Falcon Release Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SYS-001; SEC-001; SEC-002; SYS-007; ADR-F006; ADR-F007; ADR-I008; AMD-003; AMD-003-IR-001; CON-012; CON-020; CON-021  
**Supersedes:** CON-010 v1.0  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines immutable manifests that identify and bind Foundation preparation inputs, Provider candidates, Activation subjects, build and verification baselines, and approved FRS-001 releases without allowing one lifecycle class to impersonate another.

A valid Manifest establishes the exact governed subject it represents. It does not by itself activate that subject, authorize startup, grant implementation or operational authority, or authorize financial activity.

## 2. Manifest Classes

Every Manifest SHALL declare exactly one class:

- `PREPARATION_MANIFEST`;
- `CANDIDATE_MANIFEST`;
- `ACTIVATION_MANIFEST`;
- `FOUNDATION_BASELINE_MANIFEST`;
- `RELEASE_MANIFEST`; or
- another class admitted through Approved governance.

Manifest class is immutable.

A candidate or preparation Manifest SHALL NOT be represented as an active Foundation baseline or release.

## 3. Participants

- **Manifest Producer:** the participant assembling the exact subject and references.
- **Issuer:** the competent authority issuing the Manifest class.
- **Subject Owner:** the authority accountable for the represented subject.
- **Activation Authority:** the separate authority permitted to activate an exact subject.
- **Release or Promotion Authority:** the authority permitted to approve an eligible release.
- **Verifier:** the participant checking structure, identity, integrity, authority, validity, and referenced objects.
- **Custodian:** the authority preserving current and superseded Manifests.
- **Reviewer:** the competent independent verifier.

The Producer SHALL NOT become Activation or Release Authority merely by creating or signing a Manifest.

## 4. Common Manifest Fields

Every Manifest SHALL contain:

- Manifest ID, class, schema, and version;
- subject ID, type, version, lifecycle, and canonical digest;
- issuer identity and Authority Instrument;
- complete Authority Chain;
- Subject Owner;
- purpose and scope;
- Build Scope;
- environment class and environment identity;
- tools and dependency bundle identities;
- configuration, policy, and schema references;
- required Contract and Specification versions;
- Provider Profile identities and lifecycle states;
- cryptographic and custody profiles;
- input and output classifications;
- evidence requirement set and Root Evidence Set references;
- external evidence-set identity where applicable;
- bootstrap-source identity and classification where applicable;
- issue, not-before, expiry, and approval time;
- constraints, exclusions, and validity conditions;
- predecessor and supersession references;
- non-authorities;
- canonical encoding and digest; and
- integrity evidence and signature where required.

## 5. Preparation and Candidate Manifests

A `PREPARATION_MANIFEST` or `CANDIDATE_MANIFEST` SHALL additionally contain:

- Bootstrap Execution Context ID;
- `BOOTSTRAP_EXTERNAL_ID` environment identity;
- Authority Instrument class;
- exact permitted candidate subjects;
- synthetic-material requirements;
- isolation boundaries;
- stop conditions;
- cleanup obligations;
- candidate evidence references;
- independent-control evidence references; and
- explicit `NO_OPERATIONAL_AUTHORITY`.

These classes SHALL NOT:

- claim active Provider, Environment, Pipeline, or Build status;
- identify bootstrap tools as Falcon runtime dependencies;
- contain production credentials or security material;
- imply Provider or Profile Activation;
- imply release eligibility; or
- imply Foundation Implementation Authority.

## 6. Activation Manifest

An `ACTIVATION_MANIFEST` SHALL identify:

- exact activation subject and candidate Manifest;
- Activation Authority Instrument;
- applicable jurisdiction;
- exact environment and profile;
- complete verification obligations;
- accepted Root Evidence Set;
- Validity Assessment;
- Evidence Completeness Decision;
- independent review;
- disclosed conflicts and residual uncertainty;
- Activation Decision ID;
- activated scope;
- effective time;
- expiry or review condition;
- restrictions and revocation path;
- predecessor or candidate lineage; and
- exact non-authorities preserved.

An Activation Manifest is active only when the separate Activation Decision is valid and applicable.

Environment Activation SHALL NOT imply Foundation Implementation Authority.

Provider Activation SHALL NOT imply operational, release, or financial authority.

## 7. Foundation Baseline and Release Manifests

A `FOUNDATION_BASELINE_MANIFEST` or `RELEASE_MANIFEST` SHALL bind:

- release or baseline identity and version;
- Vision, Constitution, governance, Specification, Standard, ADR, Contract, and verification-baseline versions;
- required executable and non-executable artifacts;
- artifact identities, roles, digests, classifications, and required status;
- schemas and compatibility state;
- configuration baseline;
- permitted identity and workload issuers;
- active Identifier, Time, Crypto, Secret, Certificate and Identity, and Randomness Provider Profiles where required;
- active Environment, Build, Pipeline, and Gate Profiles;
- revocation-source identity and freshness rule;
- Root Verification Evidence Set;
- promotion or release decision;
- validity interval;
- restrictions and non-authorities; and
- integrity protection.

Foundation Implementation Authority SHALL NOT imply Operational Authority.

Release approval SHALL NOT imply financial authority.

## 8. Preconditions

Before issuance:

- every required governed document SHALL have the required status;
- every referenced subject SHALL have one canonical identity;
- the issuer SHALL hold explicit authority for the Manifest class;
- every referenced Provider or Profile state SHALL be exact and independently verifiable;
- required evidence SHALL be complete and accepted for the declared scope;
- required Clock Quality and revocation freshness SHALL be satisfied;
- candidate and active lifecycle states SHALL not be mixed;
- bootstrap and Falcon-native identity and evidence SHALL remain distinct; and
- integrity and canonical validation SHALL succeed.

## 9. Verification

Verification SHALL establish separately:

- schema and canonical validity;
- Manifest class and lifecycle;
- subject identity and digest;
- issuer identity, jurisdiction, and Authority Chain;
- signature and cryptographic profile;
- time validity and uncertainty;
- revocation freshness;
- referenced document, artifact, schema, configuration, and profile identities;
- required evidence and decisions;
- environment and Build Scope applicability;
- predecessor and supersession lineage;
- restrictions and non-authorities; and
- absence of prohibited lifecycle substitution.

A valid signature SHALL NOT substitute for any other check.

## 10. Postconditions

Successful verification establishes only that the presented Manifest exactly represents the declared governed subject and state.

It SHALL NOT by itself:

- activate a candidate or Profile;
- authorize unrestricted startup;
- admit a component;
- grant implementation, promotion, operational, or financial authority;
- establish current Fitness to Operate;
- upgrade bootstrap evidence; or
- remove Guardian restrictions.

## 11. Rejection

The Manifest SHALL be rejected when:

- required content or reference is missing;
- class, schema, subject, or lifecycle is unknown;
- issuer or authority is insufficient;
- signature, digest, canonical form, or artifact integrity fails;
- time validity or revocation freshness is insufficient;
- environment, Build Scope, configuration, or profile is wrong;
- a candidate is represented as active;
- bootstrap identity, time, evidence, tool, or dependency is represented as Falcon-native;
- an Authority Instrument is missing or exceeded;
- a required evidence or Activation decision is missing;
- non-authorities are omitted or contradicted; or
- versions or lineage conflict.

Unknown material state SHALL prevent unrestricted reliance.

## 12. Compatibility and Evolution

- Additive optional fields require explicit compatibility.
- Changed required meaning requires a new schema or Contract version.
- A Manifest ID SHALL NOT be reassigned or repurposed.
- Replacement SHALL create a new identity and preserve the predecessor.
- A later Manifest SHALL NOT silently approve a new subject or release.
- Storage, serialization, platform, and Provider choices SHALL NOT redefine Manifest semantics.

## 13. Security and Evidence

Private signing and secret material SHALL remain outside the Manifest.

Verification SHALL produce CON-008 evidence and, for bootstrap-origin material, preserve CON-021 origin and provenance.

Evidence SHALL identify every accepted, rejected, uncertain, stale, or missing check without exposing protected material.

Rollback, substitution, equivocation, unauthorized supersession, wrong-environment use, and class confusion SHALL be detected and rejected.

## 14. Normative Requirements

- **CON-010-REQ-001:** Every Manifest SHALL bind every required subject and artifact to one canonical integrity identity.
- **CON-010-REQ-002:** Every Manifest SHALL bind the exact governing-document, Contract, schema, configuration, and profile versions.
- **CON-010-REQ-003:** Verification SHALL establish class, schema, issuer authority, signature, validity, revocation, environment, evidence, and referenced-object integrity separately.
- **CON-010-REQ-004:** A missing required artifact, reference, decision, or Authority Instrument SHALL prevent unrestricted reliance.
- **CON-010-REQ-005:** An unknown, modified, expired, revoked, wrong-environment, conflicted, or integrity-failed Manifest SHALL be rejected.
- **CON-010-REQ-006:** A valid Manifest SHALL NOT be treated as authorization for an action.
- **CON-010-REQ-007:** Manifest replacement SHALL create a new identity and preserve predecessor lineage.
- **CON-010-REQ-008:** Verification evidence SHALL identify every accepted and rejected check without exposing protected material.
- **CON-010-REQ-009:** Clock uncertainty beyond approved tolerance SHALL prevent unrestricted reliance.
- **CON-010-REQ-010:** Revocation state older than the declared maximum age SHALL be treated as unknown.
- **CON-010-REQ-011:** Every Manifest SHALL declare exactly one immutable class.
- **CON-010-REQ-012:** Preparation and candidate Manifests SHALL NOT be represented as active baselines or releases.
- **CON-010-REQ-013:** Bootstrap identity, time, evidence, tools, and dependencies SHALL retain their external classification.
- **CON-010-REQ-014:** Every candidate Manifest SHALL identify its Bootstrap Execution Context and Authority Instrument.
- **CON-010-REQ-015:** Every Activation Manifest SHALL reference an independently evaluated evidence case and separate Activation Decision.
- **CON-010-REQ-016:** Environment or Provider Activation SHALL NOT imply Foundation Implementation Authority.
- **CON-010-REQ-017:** Foundation Implementation Authority SHALL NOT imply Operational or financial authority.
- **CON-010-REQ-018:** Candidate and active lifecycle states SHALL remain unambiguous.
- **CON-010-REQ-019:** Every Manifest SHALL preserve explicit non-authorities.
- **CON-010-REQ-020:** Manifest production, signing, or possession SHALL NOT grant Activation, promotion, or release authority.

## 15. Acceptance Examples

Acceptance requires:

- valid preparation, candidate, Activation, Foundation baseline, and release Manifests;
- rejection of candidate-as-active substitution;
- rejection of missing Authority Instrument or evidence;
- rejection of bootstrap material represented as Falcon-native;
- exact artifact, schema, configuration, and profile verification;
- issuer-jurisdiction and Authority Chain verification;
- expired, revoked, stale, wrong-environment, and clock-uncertain cases;
- explicit non-authorities;
- predecessor and supersession preservation;
- rollback and equivocation detection;
- separation of Activation, implementation, promotion, operation, and financial authority; and
- complete evidence reconstruction.

## 16. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-030 | 2026-07-25 |

This Approval activates CON-010 v1.1 and supersedes v1.0. It does not create or activate a Manifest, approve a release, authorize implementation, or authorize financial activity.
