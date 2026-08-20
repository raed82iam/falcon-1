# CON-008 — Evidence and Logging Contract

**Identifier:** CON-008  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-030  
**Owner:** Falcon Evidence Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; OPS-004; DEC-006; SEC-001; SEC-002; FCE-001; ADR-I007; ADR-I008; AMD-003; AMD-003-IR-001; CON-021  
**Supersedes:** CON-008 v1.0  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the minimum structured evidence and logging record required to reconstruct Foundation preparation, candidate verification, Activation, operation, restriction, recovery, and release decisions.

It preserves the distinction between:

- observed facts;
- producer Claims;
- external bootstrap evidence;
- Falcon-native evidence;
- derived evaluations;
- acceptance and Activation decisions; and
- authority.

Evidence records what can be demonstrated. Evidence does not establish its own truth, completeness, validity, acceptance, or authority.

## 2. Participants

- **Evidence Producer:** the participant producing an observation, result, measurement, Claim, or record.
- **Evidence Collector:** the participant capturing and protecting the record.
- **Evidence Custodian:** the participant preserving accepted evidence and retention state.
- **Evidence Consumer:** an authorized participant using evidence within a declared scope.
- **Evidence Evaluator:** the authority evaluating evidence against governed obligations and rules.
- **Evidence Completeness Authority:** the authority determining whether the required verification case is complete.
- **Challenge Authority:** the competent independent authority resolving material disputes.
- **Guardian and Health Monitoring:** protective participants receiving material evidence-loss or integrity signals.

No participant gains authority merely by producing, possessing, transforming, signing, or storing evidence.

## 3. Evidence Origin

Every record SHALL declare one governed Evidence Origin:

- `BOOTSTRAP_EXTERNAL`;
- `CANDIDATE_PRODUCED`;
- `FALCON_NATIVE`;
- `IMPORTED_EXTERNAL`; or
- another origin admitted through Approved governance.

Origin is immutable.

`BOOTSTRAP_EXTERNAL` and `CANDIDATE_PRODUCED` evidence SHALL NOT be reclassified as `FALCON_NATIVE`.

Import, cross-linking, signing, canonical transformation, or later Provider Activation SHALL NOT upgrade origin.

## 4. Evidence Record

Every record SHALL contain:

- record ID;
- record version;
- record class;
- Evidence Origin;
- producer identity;
- collector identity where distinct;
- subject identity;
- candidate-subject identity where applicable;
- source environment identity;
- source and observation time;
- Clock Quality and uncertainty, or explicit external-time classification;
- severity or consequence;
- correlation and causation;
- structured fact, observation, result, or Claim;
- scope and purpose;
- authority reference where applicable;
- governing policy and context references;
- security classification;
- integrity and provenance evidence;
- original-object reference where applicable;
- canonical digest and algorithm;
- known limitations and dependencies;
- retention class;
- supersession and lineage;
- challenge path; and
- current persistence disposition.

Required fields SHALL NOT be inferred from unstructured message text.

## 5. Record Classes

Governed record classes include:

- observation;
- execution result;
- authority decision;
- protective restriction;
- lifecycle transition;
- health and fitness assessment;
- security event;
- configuration event;
- persistence event;
- bootstrap evidence;
- candidate evidence;
- verification evidence;
- derived evaluation;
- completeness assessment;
- Acceptance Decision;
- Activation Decision;
- recovery and restoration evidence;
- challenge and resolution;
- evidence loss or corruption; and
- audit record.

Record class SHALL NOT change the immutable Evidence Origin.

## 6. Bootstrap and Candidate Evidence

External bootstrap evidence SHALL conform to CON-021 and preserve:

- external identifier and issuer;
- external environment;
- source mechanism;
- original bytes or immutable protected representation;
- external wall and monotonic observations separately;
- source resolution, uncertainty, and continuity limits;
- collection and transfer chain;
- candidate subject where applicable; and
- the explicit absence of Falcon operational identity and `VERIFIED` time.

Candidate-produced evidence SHALL remain distinguishable from External Bootstrap Control evidence.

A candidate SHALL NOT conclusively evaluate, accept, or activate itself.

## 7. Import and Reconciliation

After Falcon Identifier and Time Providers become active:

- imported evidence MAY receive Falcon record and evidence IDs;
- original external IDs SHALL remain preserved;
- the import event SHALL cross-link the identities;
- external observed time SHALL remain external;
- Falcon import time SHALL be separate;
- original bytes and digests SHALL remain available;
- transformations SHALL be attributable and reproducible;
- source limitations SHALL remain visible; and
- no Claim, validity, completeness, acceptance, or authority SHALL be upgraded by import.

Reconciliation SHALL append new evidence. It SHALL NOT rewrite historical evidence.

## 8. Logging and Persistence States

Logging disposition SHALL distinguish:

- `PRODUCED`;
- `ACCEPTED`;
- `PERSISTED`;
- `EXTERNALLY_RETAINED`;
- `UNCERTAIN`;
- `REJECTED`; and
- `LOST`.

An acknowledged audit-critical state change requires the persistence guarantees of the governing Persistence Contract and policy.

If persistence outcome is uncertain:

- the record and dependent operation SHALL be marked `UNCERTAIN`;
- the operation SHALL NOT be repeated based on assumption;
- reconciliation SHALL use the original Operation ID or an Approved duplicate-effect prevention mechanism; and
- unrestricted authority SHALL NOT rely on the unknown state.

If even the failure cannot be recorded, the competent system-safety authority SHALL evaluate loss of provability and impose appropriate Guardian and Health Monitoring restrictions.

## 9. Correction, Supersession, and Redaction

Evidence is immutable after recording.

Correction, redaction, annotation, or changed interpretation SHALL create a new linked record preserving:

- original record;
- reason;
- authority;
- transformation;
- changed or withdrawn Claims;
- scope;
- time; and
- lineage.

Authorized confidentiality-preserving reconstruction SHALL retain meaning, accountability, and proof of the applied redaction.

## 10. Evaluation, Validity, and Acceptance

Evaluation SHALL identify:

- governing obligation;
- evidence inputs;
- derivation or judgment rules;
- Evaluation Context;
- evaluator and Evaluation Authority;
- declared scope;
- result;
- limitations;
- reproducibility classification; and
- challenges.

Validity is scoped fitness under governing rules. Acceptance is a separate authority decision to rely on that assessment for a stated purpose.

No producer, transformer, aggregator, collector, or signer of material evidence SHALL be the sole authority declaring the case complete or promotion-ready.

## 11. Challenge

Material evidence and Claims SHALL remain independently challengeable.

A Challenge SHALL preserve:

- challenged record or Claim;
- challenger;
- grounds;
- evidence;
- requested protection;
- reviewing jurisdiction and authority;
- interim restriction;
- resolution; and
- resulting lineage.

The producer of the challenged Claim, the subject under evaluation, and the authority whose decision is challenged SHALL NOT conclusively resolve the material dispute alone.

## 12. Security and Privacy

- Secrets and prohibited sensitive data SHALL NOT enter ordinary evidence.
- Records SHALL receive confidentiality, integrity, availability, retention, and access protection appropriate to classification.
- Integrity evidence SHALL detect mutation, substitution, rollback, truncation, and unauthorized supersession.
- Authorized readers SHALL receive only the minimum required scope.
- Logging SHALL resist injection, ambiguous encoding, forged causation, and identity substitution.
- Security controls SHALL NOT erase adverse or embarrassing evidence.

## 13. Compatibility

- Canonical encoding SHALL be governed by FCE-001.
- Evidence meaning SHALL remain independent of storage, database, transport, operating system, runtime, platform, and vendor.
- Unknown mandatory fields or unsupported record versions SHALL cause rejection or restriction.
- Format or Provider replacement SHALL preserve original evidence, semantics, lineage, retention, and reconstruction.
- Additive optional fields may be ignored only when Approved governance declares them non-material.

## 14. Normative Requirements

- **CON-008-REQ-001:** Audit-critical actions SHALL produce attributable evidence.
- **CON-008-REQ-002:** Acceptance SHALL protect records against undetected mutation.
- **CON-008-REQ-003:** Secrets and prohibited sensitive data SHALL NOT enter ordinary evidence.
- **CON-008-REQ-004:** Missing audit-critical evidence SHALL be observable.
- **CON-008-REQ-005:** Logging SHALL distinguish produced, accepted, persisted, externally retained, uncertain, rejected, and lost states.
- **CON-008-REQ-006:** Correction and supersession SHALL append rather than rewrite.
- **CON-008-REQ-007:** Clock Quality, uncertainty, and external-time limitations SHALL be preserved.
- **CON-008-REQ-008:** Authorized reconstruction SHALL preserve confidentiality and accountable meaning.
- **CON-008-REQ-009:** Every record SHALL declare an immutable Evidence Origin.
- **CON-008-REQ-010:** Bootstrap and candidate evidence SHALL NOT be reclassified as Falcon-native evidence.
- **CON-008-REQ-011:** Original bootstrap evidence and custody lineage SHALL conform to CON-021.
- **CON-008-REQ-012:** Import SHALL cross-link identities and preserve external time without upgrading Claims or authority.
- **CON-008-REQ-013:** Candidate-produced and independent-control evidence SHALL remain distinguishable.
- **CON-008-REQ-014:** Evidence SHALL NOT establish its own truth, completeness, validity, acceptance, Activation, or authority.
- **CON-008-REQ-015:** Material evidence uncertainty or loss SHALL restrict dependent authority and trigger governed escalation.
- **CON-008-REQ-016:** Unknown persistence SHALL NOT justify blind re-execution or unrestricted authority.
- **CON-008-REQ-017:** Material Challenges SHALL have a competent independent resolution path.
- **CON-008-REQ-018:** Evidence and evaluations SHALL preserve obligation, rules, context, scope, authority, and lineage.
- **CON-008-REQ-019:** Format, store, or Provider replacement SHALL preserve reconstructability.
- **CON-008-REQ-020:** Evidence origin and historical limitations SHALL remain immutable through reconciliation.

## 15. Acceptance Examples

Acceptance requires:

- complete reconstruction of the FRS-001 scenarios;
- reconstruction of preparation, candidate verification, and Activation history;
- all Evidence Origin classes;
- mutation, truncation, rollback, and substitution detection;
- secret redaction without loss of accountability;
- evidence-loss and uncertain-persistence signaling;
- append-only correction and supersession;
- import cross-linking after Provider Activation;
- preservation of external time and identity;
- candidate and independent-control separation;
- prevention of self-completeness and self-Activation;
- independent challenge;
- store and format replacement; and
- confidentiality-preserving authorized reconstruction.

## 16. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-030 | 2026-07-25 |

This Approval activates CON-008 v1.1 and supersedes v1.0. It does not accept an evidence set, activate a candidate, authorize implementation, or authorize financial activity.
