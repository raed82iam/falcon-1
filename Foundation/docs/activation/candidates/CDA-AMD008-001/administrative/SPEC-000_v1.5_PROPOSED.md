# Falcon Specification Registry

**Identifier:** SPEC-000  
**Version:** 1.5 Proposed  
**Status:** Proposed  
**Canonical Target:** `docs/specifications/SPEC-000_REGISTRY.md`  
**Approval Record:** Pending  
**Owner:** Falcon Specification Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-063; TREE-001; ADR-I015  
**Activation Authority:** Not Granted  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted

## 1. Purpose

SPEC-000 is the canonical registry of Falcon Specifications.

It records identity, title, domain, canonical placement, ownership, effective state, proposed successor state, constitutional basis, dependency visibility, and lineage in one governed registry surface.

Registration is not approval. Approval is not activation.

## 2. Scope

SPEC-000 governs:

- specification identity;
- title;
- domain;
- canonical path;
- accountable owner;
- current effective version and status;
- proposed successor version and status;
- predecessor / successor relation;
- constitutional or governing basis;
- primary dependencies; and
- registry lineage preservation.

## 3. Non-Scope

SPEC-000 does not:

- create requirements;
- grant activation authority;
- grant implementation authority;
- grant Stage 1 authority;
- reinterpret business meaning;
- reinterpret financial meaning; or
- replace the underlying governed Specification.

## 4. Registry Ownership and Boundaries

The Specification Authority owns the registry structure only.

Each Specification retains its own accountable owner and its own governing basis.

Cross-reference documents such as Contracts, ADRs, Plans, Catalogs, and Traceability artifacts are not Specification rows unless they are explicitly governed as Specifications.

## 5. Canonical Registry

| ID | Title | Domain | Canonical Path | Accountable Owner | Current Effective Version | Current Effective Status | Proposed Successor Version | Proposed Successor Status | Predecessor / Successor Relation | Constitutional or Governing Basis | Primary Dependencies | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CAP-001 | Capital Mandate | Capital Stewardship | `docs/specifications/capital/CAP-001_CAPITAL_MANDATE.md` | Falcon Capital Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | RSK-001; RSK-002 | Planned in current registry |
| CAP-002 | Capital State and Accounting | Capital Stewardship | `docs/specifications/capital/CAP-002_CAPITAL_STATE_AND_ACCOUNTING.md` | Falcon Capital Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | CAP-001; DEC-006 | Planned in current registry |
| CAP-003 | Allocation and Exposure | Capital Stewardship | `docs/specifications/capital/CAP-003_ALLOCATION_AND_EXPOSURE.md` | Falcon Capital Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | CAP-001; RSK-002 | Planned in current registry |
| CAP-004 | Portfolio Stewardship | Capital Stewardship | `docs/specifications/capital/CAP-004_PORTFOLIO_STEWARDSHIP.md` | Falcon Capital Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | CAP-002; CAP-003 | Planned in current registry |
| CAP-005 | Performance and Attribution | Capital Stewardship | `docs/specifications/capital/CAP-005_PERFORMANCE_AND_ATTRIBUTION.md` | Falcon Capital Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | CAP-002; DEC-006 | Planned in current registry |
| RSK-001 | Risk Taxonomy | Risk and Protection | `docs/specifications/risk/RSK-001_RISK_TAXONOMY.md` | Falcon Risk Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | CAP-001; DEC-002 | Planned in current registry |
| RSK-002 | Risk Appetite and Limits | Risk and Protection | `docs/specifications/risk/RSK-002_RISK_APPETITE_AND_LIMITS.md` | Falcon Risk Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | RSK-001; CAP-003 | Planned in current registry |
| RSK-003 | Loss Containment and Safe State | Risk and Protection | `docs/specifications/risk/RSK-003_LOSS_CONTAINMENT_AND_SAFE_STATE.md` | Falcon Risk Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AUT-002; OPS-002 | Planned in current registry |
| RSK-004 | Crisis Governance | Risk and Protection | `docs/specifications/risk/RSK-004_CRISIS_GOVERNANCE.md` | Falcon Risk Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AUT-002; OPS-002 | Planned in current registry |
| RSK-005 | Capital Safety Plane | Risk and Protection | `docs/specifications/protection/RSK-005_CAPITAL_SAFETY_PLANE.md` | Falcon Capital Protection Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Vision; Constitution; GOV-003 | AUT-002; OPS-003; OPS-004 | Included in active registry |
| DEC-001 | Decision Lifecycle | Decision System | `docs/specifications/decision/DEC-001_DECISION_LIFECYCLE.md` | Falcon Decision Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | DEC-002; DEC-006 | Planned in current registry |
| DEC-002 | Evidence and Data Fitness | Decision System | `docs/specifications/decision/DEC-002_EVIDENCE_AND_DATA_FITNESS.md` | Falcon Decision Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | DEC-001; DEC-003 | Planned in current registry |
| DEC-003 | Assumptions, Confidence, and Uncertainty | Decision System | `docs/specifications/decision/DEC-003_ASSUMPTIONS_CONFIDENCE_AND_UNCERTAINTY.md` | Falcon Decision Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | DEC-002 | Planned in current registry |
| DEC-004 | Explainability and Traceability | Decision System | `docs/specifications/decision/DEC-004_EXPLAINABILITY_AND_TRACEABILITY.md` | Falcon Decision Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | TRC-001; DEC-006 | Planned in current registry |
| DEC-005 | Outcome Evaluation and Learning | Decision System | `docs/specifications/decision/DEC-005_OUTCOME_EVALUATION_AND_LEARNING.md` | Falcon Decision Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | DEC-004; EVO-001 | Planned in current registry |
| DEC-006 | Decision Ledger | Decision System | `docs/specifications/decision/DEC-006_DECISION_LEDGER.md` | Falcon Decision Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Vision; Constitution; GOV-003 | DEC-001; TRC-001 | Included in active registry |
| AWR-001 | Foundation Self-Awareness System | Self-Awareness | `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md` | Falcon Self-Awareness Authority | 1.0 | Approved | 2.1 | Proposed | AWR-001 v2.1 is the proposed successor to AWR-001 v1.0; AWR-001 v2.0 remains historical and not effective | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | AUT-002; SEC-001; SEC-002; SYS-001; SYS-002; SYS-008; SYS-011; FDN-001; FDN-002 | AMD-008 aligned and reviewed |
| AWR-002 | Fitness to Operate | Self-Awareness | `docs/specifications/self-awareness/AWR-002_FITNESS_TO_OPERATE.md` | Falcon Self-Awareness Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AWR-001; CON-006 | Planned in current registry |
| AWR-003 | Confidence and Uncertainty | Self-Awareness | `docs/specifications/self-awareness/AWR-003_CONFIDENCE_AND_UNCERTAINTY.md` | Falcon Self-Awareness Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AWR-001; DEC-003 | Planned in current registry |
| AWR-004 | Temporal Awareness | Self-Awareness | `docs/specifications/self-awareness/AWR-004_TEMPORAL_AWARENESS.md` | Falcon Self-Awareness Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AWR-001; TIM-001 | Planned in current registry |
| AWR-005 | Drift and Blind-Spot Detection | Self-Awareness | `docs/specifications/self-awareness/AWR-005_DRIFT_AND_BLIND_SPOT_DETECTION.md` | Falcon Self-Awareness Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AWR-001; OPS-003; OPS-004 | Planned in current registry |
| AWR-006 | Main Self-Awareness | Self-Awareness | `docs/specifications/self-awareness/AWR-006_MAIN_SELF_AWARENESS.md` | UNKNOWN | NONE | NOT YET EFFECTIVE | 2.0 | APPROVED PENDING COORDINATED ACTIVATION | Proposed successor to the application-aware hierarchy line; separate from the Foundation AWR-001 line | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | APP-001; CON-023; SYS-003; SYS-004; SYS-006 | Owner pending review in source package |
| AWR-007 | Local Self-Awareness | Self-Awareness | `docs/specifications/self-awareness/AWR-007_LOCAL_SELF_AWARENESS.md` | UNKNOWN | NONE | NOT YET EFFECTIVE | 2.0 | APPROVED PENDING COORDINATED ACTIVATION | Proposed successor to the branch-level self-awareness line | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | AWR-006; APP-001 | Owner pending review in source package |
| AWR-008 | Component Self-Awareness | Self-Awareness | `docs/specifications/self-awareness/AWR-008_COMPONENT_SELF_AWARENESS.md` | UNKNOWN | NONE | NOT YET EFFECTIVE | 1.1 | APPROVED PENDING COORDINATED ACTIVATION | Proposed successor to the component-level self-awareness line | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | AWR-007; APP-001 | Owner pending review in source package |
| INT-001 | Intelligence Governance | Intelligence | `docs/specifications/intelligence/INT-001_INTELLIGENCE_GOVERNANCE.md` | Falcon Intelligence Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AWR-001; SEC-002 | Planned in current registry |
| INT-002 | Model and Strategy Admission | Intelligence | `docs/specifications/intelligence/INT-002_MODEL_AND_STRATEGY_ADMISSION.md` | Falcon Intelligence Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | INT-001; DEC-002 | Planned in current registry |
| INT-003 | Validation, Challenge, and Drift | Intelligence | `docs/specifications/intelligence/INT-003_VALIDATION_CHALLENGE_AND_DRIFT.md` | Falcon Intelligence Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | INT-001; TRC-001 | Planned in current registry |
| EVO-001 | Self-Maintenance and Evolution System | Maintenance and Evolution | `docs/specifications/core/EVO-001_SELF_MAINTENANCE_AND_EVOLUTION.md` | Falcon Evolution Authority | 1.0 | Approved | 1.1 | Proposed | EVO-001 v1.1 is the proposed successor line for the approved maintenance and evolution system | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | AWR-001; AUT-002; PIPE-001 | AMD-008 aligned and reviewed |
| EVO-002 | Progressive Autonomy | Maintenance and Evolution | `docs/specifications/evolution/EVO-002_PROGRESSIVE_AUTONOMY.md` | Falcon Evolution Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | EVO-001; AUT-001 | Planned in current registry |
| EVO-003 | Safe Evolution Envelope | Maintenance and Evolution | `docs/specifications/evolution/EVO-003_SAFE_EVOLUTION_ENVELOPE.md` | Falcon Evolution Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | EVO-001; PIPE-001 | Planned in current registry |
| EVO-004 | Digital Twin and Simulation | Maintenance and Evolution | `docs/specifications/evolution/EVO-004_DIGITAL_TWIN_AND_SIMULATION.md` | Falcon Evolution Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | EVO-001; INT-003 | Planned in current registry |
| EVO-005 | Shadow, Canary, Promotion, and Rollback | Maintenance and Evolution | `docs/specifications/evolution/EVO-005_SHADOW_CANARY_PROMOTION_AND_ROLLBACK.md` | Falcon Evolution Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | EVO-003; PIPE-001; OPS-003 | Planned in current registry |
| AUT-001 | Authority Engine | Autonomy and Control | `docs/specifications/core/AUT-001_AUTHORITY_ENGINE.md` | Falcon Governance Authority | 1.1 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SEC-002; SYS-005; PIPE-001 | Included in active registry |
| AUT-002 | Guardian | Autonomy and Control | `docs/specifications/core/AUT-002_GUARDIAN.md` | Falcon Protection Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SEC-001; OPS-003; OPS-004 | Included in active registry |
| AUT-003 | Intervention, Revocation, and Recovery | Autonomy and Control | `docs/specifications/autonomy/AUT-003_INTERVENTION_REVOCATION_AND_RECOVERY.md` | Falcon Protection Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AUT-001; AUT-002; OPS-003 | Planned in current registry |
| FIN-001 | Market and Reference Data | Financial Operations | `docs/specifications/financial/FIN-001_MARKET_AND_REFERENCE_DATA.md` | Falcon Financial Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | SYS-009; SYS-011; SEC-002 | Planned in current registry |
| FIN-002 | Order and Execution Governance | Financial Operations | `docs/specifications/financial/FIN-002_ORDER_AND_EXECUTION_GOVERNANCE.md` | Falcon Financial Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AUT-001; PIPE-001; SEC-001 | Planned in current registry |
| FIN-003 | Position and Portfolio Operations | Financial Operations | `docs/specifications/financial/FIN-003_POSITION_AND_PORTFOLIO_OPERATIONS.md` | Falcon Financial Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | FIN-001; DEC-006 | Planned in current registry |
| FIN-004 | Reconciliation and Valuation | Financial Operations | `docs/specifications/financial/FIN-004_RECONCILIATION_AND_VALUATION.md` | Falcon Financial Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | FIN-001; SYS-011 | Planned in current registry |
| SYS-001 | Kernel | OS Foundation | `docs/specifications/core/SYS-001_KERNEL.md` | Falcon Core Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-002; SYS-005; SEC-001 | Included in active registry |
| SYS-002 | Lifecycle | OS Foundation | `docs/specifications/core/SYS-002_LIFECYCLE.md` | Falcon Core Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-001; AUT-001 | Included in active registry |
| SYS-003 | Service Identity and Catalog | OS Foundation | `docs/specifications/foundation/SYS-003_FOUNDATION_SERVICE_OWNERSHIP_AND_CATALOG.md` | UNKNOWN | NONE | NOT YET EFFECTIVE | 1.1 | APPROVED PENDING COORDINATED ACTIVATION | PROPOSED NEW SPECIFICATION | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | SYS-001; SYS-002; SYS-005 | Owner pending review in source package |
| SYS-004 | Dependency Governance | OS Foundation | `docs/specifications/foundation/SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE.md` | UNKNOWN | NONE | NOT YET EFFECTIVE | 1.0 | APPROVED PENDING COORDINATED ACTIVATION | PROPOSED NEW SPECIFICATION | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | SYS-003; SYS-011; APP-001 | Owner pending review in source package |
| SYS-005 | Service Bus | OS Foundation | `docs/specifications/core/SYS-005_SERVICE_BUS.md` | Falcon Communication Authority | 1.1 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-009; SYS-010; SEC-002 | Included in active registry |
| SYS-006 | Resource Governance | OS Foundation | `docs/specifications/foundation/SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE.md` | UNKNOWN | NONE | NOT YET EFFECTIVE | 1.1 | APPROVED PENDING COORDINATED ACTIVATION | PROPOSED NEW SPECIFICATION | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | SYS-001; SYS-002; APP-001 | Owner pending review in source package |
| SYS-007 | Configuration | OS Foundation | `docs/specifications/core/SYS-007_CONFIGURATION.md` | Falcon Core Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-001; SEC-002 | Included in active registry |
| SYS-008 | Health Monitoring | OS Foundation | `docs/specifications/core/SYS-008_HEALTH_MONITORING.md` | Falcon Operational Integrity Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-001; OPS-003; OPS-004 | Included in active registry |
| SYS-009 | FIL | OS Foundation | `docs/specifications/core/SYS-009_FIL.md` | Falcon Communication Authority | 1.1 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-005; FCE-001; SEC-002 | Included in active registry |
| SYS-010 | Event System | OS Foundation | `docs/specifications/core/SYS-010_EVENT_SYSTEM.md` | Falcon Communication Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-005; SEC-002 | Included in active registry |
| SYS-011 | Persistence | OS Foundation | `docs/specifications/core/SYS-011_PERSISTENCE.md` | Falcon Data Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SEC-002; OPS-003; OPS-004 | Included in active registry |
| PLG-001 | Capability Passport and Admission | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-001_CAPABILITY_PASSPORT_AND_ADMISSION.md` | Falcon Capability Ecosystem Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-005; SEC-002; PIPE-001 | Included in active registry |
| PLG-002 | Falcon Cells and Capability Isolation | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-002_FALCON_CELLS_AND_CAPABILITY_ISOLATION.md` | Falcon Capability Ecosystem Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | PLG-001; SYS-011 | Planned in current registry |
| PLG-003 | Capability Update, Migration, and Removal | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-003_CAPABILITY_UPDATE_MIGRATION_AND_REMOVAL.md` | Falcon Capability Ecosystem Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | PLG-001; EVO-005 | Planned in current registry |
| PLG-004 | Supply Chain Trust | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-004_SUPPLY_CHAIN_TRUST.md` | Falcon Capability Ecosystem Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | PLG-001; SEC-001; SEC-002 | Planned in current registry |
| SEC-001 | Security | Trust and Security | `docs/specifications/core/SEC-001_SECURITY.md` | Falcon Security Authority | 1.1 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SEC-002; SYS-011; FCE-001 | Included in active registry |
| SEC-002 | Foundation Trust Object Model | Trust and Security | `docs/specifications/core/SEC-002_FOUNDATION_TRUST_OBJECT_MODEL.md` | Falcon Security Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | FCE-001; TRC-001; SYS-011 | Included in active registry |
| SEC-003 | Auditability | Trust and Security | `docs/specifications/security/SEC-003_AUDITABILITY.md` | Falcon Security Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | SEC-002; TRC-001 | Planned in current registry |
| FCE-001 | Falcon Canonical Encoding Specification | Canonical Representation | `docs/specifications/core/FCE-001_FALCON_CANONICAL_ENCODING_SPECIFICATION.md` | Falcon Specification Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SEC-002; SYS-009; SYS-010 | Included in active registry |
| PIPE-001 | Foundation Pipeline Specification | Build, Verification, and Promotion | `docs/specifications/foundation/PIPE-001_FOUNDATION_PIPELINE_SPECIFICATION.md` | Falcon Foundation Governance | 1.1 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | FCE-001; TRC-001; SEC-002; AUT-001 | Included in active registry |
| OPS-001 | Observability | Reliability and Operations | `docs/specifications/ops/OPS-001_OBSERVABILITY.md` | Falcon Operational Integrity Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | OPS-003; OPS-004 | Planned in current registry |
| OPS-002 | Fault Containment and Degradation | Reliability and Operations | `docs/specifications/ops/OPS-002_FAULT_CONTAINMENT_AND_DEGRADATION.md` | Falcon Operational Integrity Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | AUT-002; OPS-003 | Planned in current registry |
| OPS-003 | Recovery | Reliability and Operations | `docs/specifications/core/OPS-003_RECOVERY.md` | Falcon Operational Integrity Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SYS-008; SYS-011 | Included in active registry |
| OPS-004 | Logging | Reliability and Operations | `docs/specifications/core/OPS-004_LOGGING.md` | Falcon Evidence Authority | 1.0 | Approved | NONE | NONE | Current effective specification | Falcon Vision; Falcon Constitution; GOV-003 | SEC-002; TRC-001 | Included in active registry |
| EXT-001 | External Dependency Governance | External Relationships | `docs/specifications/external/EXT-001_EXTERNAL_DEPENDENCY_GOVERNANCE.md` | Falcon External Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | SYS-004; SEC-002 | Planned in current registry |
| EXT-002 | Broker and Venue Relationship | External Relationships | `docs/specifications/external/EXT-002_BROKER_AND_VENUE_RELATIONSHIP.md` | Falcon External Authority | NONE | NOT YET EFFECTIVE | NONE | NONE | New specification | Vision; Constitution | FIN-002; EXT-001 | Planned in current registry |
| APP-001 | Application Boundary and Lifecycle | Applications | `docs/specifications/applications/APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE.md` | UNKNOWN | NONE | NOT YET EFFECTIVE | 1.1 | APPROVED PENDING COORDINATED ACTIVATION | Proposed successor to the application boundary and lifecycle line | Falcon Vision; Falcon Constitution; GOV-063; ADR-I015 | SYS-003; SYS-004; SYS-006; AWR-006; AWR-007; AWR-008 | Owner pending review in source package |

## 6. Registry Rules

- Every Specification ID SHALL appear exactly once in this registry.
- Each registry row SHALL have exactly one canonical path.
- Current effective version and proposed successor version SHALL remain distinct.
- `NONE` SHALL be used when no current effective version exists.
- `NOT YET EFFECTIVE` SHALL be used when a row is planned or proposed but not yet effective.
- `PENDING_REVIEW` or `UNKNOWN` SHALL be used when the governing source does not provide a fact needed for the registry row.
- Contracts, ADRs, Plans, and Catalogs SHALL not be registered as Specification rows unless they are explicitly governed as Specifications.
- Cross-reference entries such as CON-023 and ADR-I015 are governing references only and do not become Specification rows by association.

## 7. Failure and Conflict Behavior

If duplicate IDs, ambiguous ownership, conflicting titles, missing constitutional basis, missing canonical path, or conflicting status appear:

- the affected registry row SHALL be marked invalid for canonical reliance;
- the conflict SHALL remain visible;
- no activation SHALL be inferred; and
- no hidden override SHALL be accepted.

## 8. Invariants

1. One Specification ID, one canonical row.
2. Registration does not activate.
3. Historical meaning remains historical.
4. Planned entries are visible absence, not hidden approval.
5. Registry content cannot outrank the governing Specification.
6. Current effective and proposed successor data SHALL coexist only within the same canonical row.

## 9. Acceptance Evidence

Acceptance requires proof that the registry:

- lists every Specification row from the active registry without omission;
- preserves current effective and proposed successor state in one row per ID;
- distinguishes approved, planned, proposed, and not-yet-effective states;
- avoids duplicates and orphan entries;
- stays aligned with TREE-001;
- and preserves AMD-008 / GOV-063 alignment without inventing missing facts.
