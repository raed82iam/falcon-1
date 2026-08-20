# Falcon Specification Registry

**Identifier:** SPEC-000  
**Version:** 1.4  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-003; Project Owner approval of SEC-002 recorded on 2026-07-25; GOV-012; GOV-014; GOV-020
**Owner:** Falcon Specification Authority

## 1. Purpose

This registry is the canonical inventory of Falcon Specifications.

It controls identifiers, titles, domains, ownership, status, and dependency visibility. Registration does not constitute approval.

## 2. Identifier Scheme

Identifiers use:

```text
DOMAIN-NNN
```

The domain prefix is defined by the Specification Tree. Numbers are permanent and shall not be reused.

Child specifications may use a decimal suffix only when they decompose the same governed subject:

```text
DEC-001
DEC-001.01
DEC-001.02
```

Decimal identifiers shall not be used merely to group unrelated documents.

## 3. Specification Baseline

| ID | Title | Domain | Status | Constitutional basis |
|---|---|---|---|---|
| CAP-001 | Capital Mandate | Capital Stewardship | Planned | Articles 7–12 |
| CAP-002 | Capital State and Accounting | Capital Stewardship | Planned | Articles 8–10, 31–32 |
| CAP-003 | Allocation and Exposure | Capital Stewardship | Planned | Articles 9, 19, 23 |
| CAP-004 | Portfolio Stewardship | Capital Stewardship | Planned | Articles 8–10, 22–23 |
| CAP-005 | Performance and Attribution | Capital Stewardship | Planned | Articles 10, 20, 31–32 |
| RSK-001 | Risk Taxonomy | Risk and Protection | Planned | Articles 8, 15, 23 |
| RSK-002 | Risk Appetite and Limits | Risk and Protection | Planned | Articles 7–10, 23–24 |
| RSK-003 | Loss Containment and Safe State | Risk and Protection | Planned | Articles 8, 18, 24, 34, 43 |
| RSK-004 | Crisis Governance | Risk and Protection | Planned | Articles 18, 24, 41–43 |
| RSK-005 | Capital Safety Plane | Risk and Protection | Approved | Articles 7–10, 23–24, 34 |
| DEC-001 | Decision Lifecycle | Decision System | Planned | Articles 19–24 |
| DEC-002 | Evidence and Data Fitness | Decision System | Planned | Articles 13, 15, 31–33 |
| DEC-003 | Assumptions, Confidence, and Uncertainty | Decision System | Planned | Articles 15, 19, 29, 31 |
| DEC-004 | Explainability and Traceability | Decision System | Planned | Articles 16–17, 32 |
| DEC-005 | Outcome Evaluation and Learning | Decision System | Planned | Articles 20, 28, 32 |
| DEC-006 | Decision Ledger | Decision System | Approved | Articles 17, 19–20, 31–32 |
| AWR-001 | Self-Awareness System | Self-Awareness | Approved | Articles 6, 15, 19, 25, 28–30 |
| AWR-002 | Fitness to Operate | Self-Awareness | Planned | Articles 6, 15, 24, 29, 36C |
| AWR-003 | Confidence and Uncertainty | Self-Awareness | Planned | Articles 15, 19, 29, 31 |
| AWR-004 | Temporal Awareness | Self-Awareness | Planned | Articles 15, 19–20, 31–32 |
| AWR-005 | Drift and Blind-Spot Detection | Self-Awareness | Planned | Articles 28–29, 31, 36 |
| INT-001 | Intelligence Governance | Intelligence | Planned | Articles 25–30 |
| INT-002 | Model and Strategy Admission | Intelligence | Planned | Articles 25, 28–29, 31 |
| INT-003 | Validation, Challenge, and Drift | Intelligence | Planned | Articles 28–31 |
| EVO-001 | Self-Maintenance and Evolution System | Maintenance and Evolution | Approved | Articles 28–30, 35–36D |
| EVO-002 | Progressive Autonomy | Maintenance and Evolution | Planned | Articles 26–27, 30, 36C |
| EVO-003 | Safe Evolution Envelope | Maintenance and Evolution | Planned | Articles 23–24, 30, 36B–36C |
| EVO-004 | Digital Twin and Simulation | Maintenance and Evolution | Planned | Articles 23, 31, 36B |
| EVO-005 | Shadow, Canary, Promotion, and Rollback | Maintenance and Evolution | Planned | Articles 23, 30, 36–36B |
| AUT-001 | Authority Engine | Autonomy and Control | Approved | Articles 16, 19, 24, 26–27, 39 |
| AUT-002 | Guardian | Autonomy and Control | Approved | Articles 18, 24, 30, 34, 43 |
| AUT-003 | Intervention, Revocation, and Recovery | Autonomy and Control | Planned | Articles 18, 26, 30, 43 |
| FIN-001 | Market and Reference Data | Financial Operations | Planned | Articles 9, 13, 31–33 |
| FIN-002 | Order and Execution Governance | Financial Operations | Planned | Articles 19, 23–24, 27 |
| FIN-003 | Position and Portfolio Operations | Financial Operations | Planned | Articles 8–10, 23 |
| FIN-004 | Reconciliation and Valuation | Financial Operations | Planned | Articles 9, 13, 31–33 |
| SYS-001 | Kernel | OS Foundation | Approved | Articles 6, 11, 18, 35 |
| SYS-002 | Lifecycle | OS Foundation | Approved | Articles 18, 26, 39 |
| SYS-003 | Service Identity and Catalog | OS Foundation | Candidate Migration | Articles 16, 32, 39 |
| SYS-004 | Dependency Governance | OS Foundation | Candidate Migration | Articles 18, 23, 34, 36 |
| SYS-005 | Service Bus | OS Foundation | Approved | Articles 16–17, 32–34 |
| SYS-006 | Resource Governance | OS Foundation | Candidate Migration | Articles 8–10, 18, 23 |
| SYS-007 | Configuration | OS Foundation | Approved | Articles 3–5, 26, 39 |
| SYS-008 | Health Monitoring | OS Foundation | Approved | Articles 18, 31, 34, 41–43 |
| SYS-009 | FIL | OS Foundation | Approved | Articles 13, 17, 26–27, 31–33 |
| SYS-010 | Event System | OS Foundation | Approved | Articles 13, 17, 20, 31–33 |
| SYS-011 | Persistence | OS Foundation | Approved | Articles 9, 13, 18, 23, 31–34 |
| PLG-001 | Capability Passport and Admission | Replaceable Capability Ecosystem | Approved | Articles 26, 33, 35, 36D |
| PLG-002 | Falcon Cells and Capability Isolation | Replaceable Capability Ecosystem | Planned | Articles 18, 22–24, 34, 36D |
| PLG-003 | Capability Update, Migration, and Removal | Replaceable Capability Ecosystem | Planned | Articles 29, 35–36D |
| PLG-004 | Supply Chain Trust | Replaceable Capability Ecosystem | Planned | Articles 31–34, 36D |
| SEC-001 | Security | Trust and Security | Approved | Articles 21, 24, 26, 30, 33–34 |
| SEC-002 | Foundation Trust Object Model | Trust and Security | Approved | Articles 13, 16–17, 21, 24, 26, 31–34, 40–44 |
| SEC-003 | Auditability | Trust and Security | Planned | Articles 16–17, 31–32, 40–44 |
| FCE-001 | Falcon Canonical Encoding Specification | Canonical Representation | Approved | Articles 13, 16–17, 31–33 |
| PIPE-001 | Foundation Pipeline Specification | Build, Verification, and Promotion | Approved | Articles 13, 16–18, 21, 23–24, 26–27, 30–36, 39–44 |
| OPS-001 | Observability | Reliability and Operations | Planned | Articles 17–18, 31–34 |
| OPS-002 | Fault Containment and Degradation | Reliability and Operations | Planned | Articles 18, 24, 34, 41–43 |
| OPS-003 | Recovery | Reliability and Operations | Approved | Articles 8, 18, 32–34 |
| OPS-004 | Logging | Reliability and Operations | Approved | Articles 13, 16–17, 31–33 |
| EXT-001 | External Dependency Governance | External Relationships | Planned | Articles 18, 23, 33–36 |
| EXT-002 | Broker and Venue Relationship | External Relationships | Planned | Articles 8–10, 23–24, 33 |
| APP-001 | Application Boundary and Admission | Applications | Planned | Articles 6, 11, 35–36 |

## 4. Status Meaning

- **Planned:** required coverage identified; no approved content exists.
- **Candidate Migration:** relevant legacy content exists and requires constitutional review.
- **Candidate Rewrite:** legacy subject remains valid, but its authority or framing is incompatible with the current foundation.
- **Draft, Proposed, Approved, Deprecated, Superseded, Rejected, Archived:** as defined by GOV-001.

## 5. Admission Rule

No new Specification may enter this registry unless:

1. its subject belongs to a defined Tree domain;
2. no current specification already owns the same requirement;
3. its constitutional basis is identified;
4. its accountable owner is assigned;
5. affected cross-domain obligations are identified; and
6. its proposed scope is narrow enough to remain coherent.

## 6. Coverage Rule

The Registry shall expose both duplication and absence.

Missing specification coverage shall remain visible as **Planned**. Planned entries shall not be cited as if they contain requirements.
