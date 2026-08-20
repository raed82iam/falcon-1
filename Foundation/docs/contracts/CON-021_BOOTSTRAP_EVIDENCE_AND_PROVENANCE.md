# CON-021 — Bootstrap Evidence and Provenance Contract

**Identifier:** CON-021  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-029  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-002; FCE-001; ADR-I007; ADR-I008; AMD-003; AMD-003-IR-001; CON-008; CON-020  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines how external bootstrap evidence is captured, classified, preserved, imported, cross-linked, evaluated, challenged, superseded, and reconstructed without being misrepresented as Falcon-native operational evidence.

Bootstrap evidence identifies what was externally observed during the path to Activation. It does not establish Falcon operational truth, validity, authority, or fitness by itself.

## 2. Participants

- **External Evidence Producer:** the tool, platform, runner, authority, or person producing the original observation.
- **Evidence Collector:** the participant capturing original material and metadata.
- **Evidence Importer:** the participant admitting preserved external evidence into Falcon-governed custody.
- **Candidate Subject:** the exact candidate whose behavior may be observed.
- **Evidence Evaluator:** the authority evaluating claims for a declared scope.
- **Evidence Completeness Authority:** the authority determining whether obligations are covered.
- **Activation Authority:** the separate authority deciding whether the evaluated candidate may become active.
- **Challenge Authority:** the competent independent authority resolving material disputes.
- **Evidence Custodian:** the participant preserving original and derived Trust Objects.

## 3. Evidence Origin

Every evidence item SHALL declare one origin:

- `BOOTSTRAP_EXTERNAL`;
- `CANDIDATE_PRODUCED`;
- `FALCON_NATIVE`;
- `IMPORTED_EXTERNAL`; or
- another origin admitted by Approved governance.

Evidence origin is immutable. Correction or changed understanding SHALL create a new linked record and SHALL NOT rewrite the original origin.

`BOOTSTRAP_EXTERNAL` evidence SHALL never be reclassified as `FALCON_NATIVE`.

## 4. Bootstrap Evidence Record

Every record SHALL contain:

- external evidence ID;
- origin and origin version;
- evidence class;
- producer identity and mechanism;
- observing environment and external environment identity;
- candidate-subject identity, when applicable;
- source artifact, tool, dependency, and configuration identities;
- original bytes or protected immutable object reference;
- canonical digest and algorithm;
- external wall-time observation;
- external monotonic observation, where available;
- source, resolution, uncertainty, and continuity boundary;
- scope and purpose;
- correlation and causation;
- authority and Bootstrap Execution Context references;
- security classification;
- collection and transfer method;
- chain of custody;
- known limitations and dependencies;
- claims carried, if any;
- retention and disposal class;
- integrity protection; and
- challenge path.

## 5. Capture and Preservation

Capture SHALL:

- preserve original bytes or an immutable protected representation;
- compute and record content identity without altering the source;
- distinguish producer output from collector metadata;
- preserve external identity and time as external;
- prevent secret or prohibited-data leakage;
- record collection failures and missing evidence;
- avoid lossy normalization;
- produce an attributable transfer receipt; and
- preserve every custody transition.

A digest proves content equality only. It does not prove origin, truth, completeness, or authority.

## 6. Import and Cross-Linking

After Falcon Identifier and Time Providers become active:

- imported records may receive new Falcon operational record and evidence IDs;
- the original external ID SHALL remain preserved;
- the import event SHALL link both identities;
- external time SHALL remain external;
- Falcon import time SHALL be recorded separately;
- source origin and limitations SHALL remain unchanged;
- canonical transformation, if required, SHALL preserve original bytes and transformation evidence; and
- no import operation may upgrade a Claim, validity, or authority.

Cross-linking establishes traceability, not identity continuity.

## 7. Candidate and Independent Evidence

Candidate-produced evidence SHALL be distinguishable from External Bootstrap Control evidence.

The candidate SHALL NOT be the sole producer, collector, signer, evaluator, completeness authority, or Activation authority for its own material case.

Where external and candidate controls share a host, provider, root, tool, or actor, the dependency SHALL be declared and independence SHALL NOT be claimed.

## 8. Evaluation and Activation

Evaluation SHALL identify:

- exact obligations;
- evidence set;
- governing rules and context;
- scope;
- limitations;
- deterministic or judgment-based nature;
- Evaluation Authority;
- validity result; and
- challenges.

Evidence Completeness and Profile Activation are separate decisions.

No evidence item or producer result may mark itself complete, accepted, or activation-ready.

## 9. Uncertain and Missing Evidence

Missing, conflicted, mutated, unverifiable, stale, or origin-unknown evidence SHALL:

- remain explicitly classified;
- prevent unsupported Claims;
- cause restrictive validity or completeness;
- preserve the failure itself as evidence where possible;
- trigger challenge or escalation proportionate to consequence; and
- never be filled by assumption, inference, or candidate self-assertion.

If even failure evidence cannot be preserved, the competent system-safety authority SHALL evaluate loss of provability and impose appropriate Guardian and Health Monitoring restrictions.

## 10. Supersession and Challenge

Evidence is immutable after recording.

Correction, redaction, added context, or later reconciliation SHALL produce a new linked Trust Object preserving:

- prior record;
- reason;
- authority;
- changed Claims;
- effective scope;
- time; and
- lineage.

A material Challenge SHALL NOT be conclusively resolved solely by the producer of the challenged Claim, the candidate subject, or the authority whose decision is under challenge.

## 11. Compatibility and Security

The Contract SHALL remain independent of file format, evidence store, database, transport, operating system, runtime, platform, and vendor.

Canonical encoding SHALL be governed by FCE-001.

Original evidence SHALL remain accessible for authorized reconstruction even when derived formats or stores change.

Evidence shall receive confidentiality, integrity, availability, provenance, retention, and access protection appropriate to its classification. Secrets and reusable credentials SHALL NOT enter ordinary evidence.

## 12. Normative Requirements

- **CON-021-REQ-001:** Every bootstrap evidence item SHALL declare an immutable origin.
- **CON-021-REQ-002:** `BOOTSTRAP_EXTERNAL` evidence SHALL NOT be reclassified as Falcon-native evidence.
- **CON-021-REQ-003:** Original bytes or an immutable protected representation SHALL be preserved.
- **CON-021-REQ-004:** External issuer, environment, identity, time, uncertainty, continuity, and custody SHALL remain attributable.
- **CON-021-REQ-005:** Candidate-produced and independent-control evidence SHALL remain distinguishable.
- **CON-021-REQ-006:** A digest SHALL NOT be treated as proof of truth, provenance, completeness, or authority.
- **CON-021-REQ-007:** Import SHALL cross-link external and Falcon identifiers without replacement.
- **CON-021-REQ-008:** Import time SHALL remain separate from observed external time.
- **CON-021-REQ-009:** Import or transformation SHALL NOT upgrade Claims, validity, trust, completeness, or authority.
- **CON-021-REQ-010:** Candidate subjects SHALL NOT conclusively validate or activate themselves.
- **CON-021-REQ-011:** Shared control dependencies SHALL be declared and SHALL not be mislabeled as independent.
- **CON-021-REQ-012:** Missing or unverifiable evidence SHALL restrict unsupported Claims and decisions.
- **CON-021-REQ-013:** Correction and supersession SHALL append immutable lineage rather than rewrite history.
- **CON-021-REQ-014:** Material Challenges SHALL have a competent independent resolution path.
- **CON-021-REQ-015:** Evidence SHALL remain reconstructable across format or Provider replacement.
- **CON-021-REQ-016:** Secrets and prohibited sensitive material SHALL NOT enter ordinary evidence.

## 13. Acceptance Examples

Acceptance requires original-byte preservation; origin immutability; external identity and time retention; candidate and external-control separation; import cross-linking after Provider Activation; no time or Claim upgrade; mutation detection; missing-evidence restriction; shared-dependency disclosure; append-only correction; independent challenge; store and format migration; confidentiality-preserving reconstruction; and inability of evidence to activate its own subject.

## 14. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-029 | 2026-07-25 |

This Approval admits CON-021 as a governed Foundation Contract. It does not accept any evidence set, validate or activate a candidate, authorize implementation, or authorize financial activity.
