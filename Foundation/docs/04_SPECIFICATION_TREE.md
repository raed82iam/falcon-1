# Falcon Specification Tree

**Version:** 1.3  
**Status:** Approved  
**Approval Record:** GOV-063  
**Documentary Activation:** Active  
**Activation Record:** GOV-092; GOV-093; GOV-094  
**Effective Documentary Instant:** 2026-07-31 22:54:57 +03:00  
**Supersedes:** TREE-001 v1.2  
**Identifier:** TREE-001  
**Canonical Target:** `docs/04_SPECIFICATION_TREE.md`  
**Owner:** Falcon Specification Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SPEC-000; GOV-063  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted  

## 1. Purpose

TREE-001 defines the canonical specification tree for Falcon and fixes each governing ID to one canonical place.

It is the structural index for specifications, not a source of independent authority.

## 2. Scope

TREE-001 governs:

- canonical domains;
- document ownership placement;
- current effective documents;
- proposed successors;
- historical records;
- preserved versus replaced versus superseded versus no-change relationships; and
- validation rules for canonical uniqueness and lineage clarity.

## 3. Non-Scope

TREE-001 does not:

- grant activation authority;
- grant implementation authority;
- grant Stage 1 authority;
- create new requirements by itself;
- rewrite historical records; or
- allow orphan or duplicate canonical placement.

## 4. Canonical Tree

```text
Falcon
├── Vision
├── Constitution
├── Governance
├── Specifications
│   ├── CAP — Capital Stewardship
│   ├── RSK — Risk and Protection
│   ├── DEC — Decision System
│   ├── AWR — Self-Awareness
│   ├── INT — Intelligence
│   ├── EVO — Maintenance and Evolution
│   ├── AUT — Autonomy and Control
│   ├── FIN — Financial Operations
│   ├── SYS — Operating System Foundation
│   ├── PLG — Replaceable Capability Ecosystem
│   ├── SEC — Trust and Security
│   ├── FCE — Canonical Representation
│   ├── PIPE — Build, Verification, and Promotion
│   ├── OPS — Reliability and Operations
│   ├── EXT — External Relationships
│   └── APP — Applications and Experiences
├── Contracts
├── ADRs
├── Catalogs
├── Standards
├── Traceability
├── Plans
├── Releases
├── Evidence
└── Archive
```

## 5. Canonical Document Placement

Every governed ID SHALL occupy one canonical location:

| ID | Domain | Canonical Path | Structural Relationship |
|---|---|---|---|
| CAP-001 | Capital Stewardship | `docs/specifications/capital/CAP-001_CAPITAL_MANDATE.md` | proposed new specification |
| CAP-002 | Capital Stewardship | `docs/specifications/capital/CAP-002_CAPITAL_STATE_AND_ACCOUNTING.md` | proposed new specification |
| CAP-003 | Capital Stewardship | `docs/specifications/capital/CAP-003_ALLOCATION_AND_EXPOSURE.md` | proposed new specification |
| CAP-004 | Capital Stewardship | `docs/specifications/capital/CAP-004_PORTFOLIO_STEWARDSHIP.md` | proposed new specification |
| CAP-005 | Capital Stewardship | `docs/specifications/capital/CAP-005_PERFORMANCE_AND_ATTRIBUTION.md` | proposed new specification |
| RSK-001 | Risk and Protection | `docs/specifications/risk/RSK-001_RISK_TAXONOMY.md` | proposed new specification |
| RSK-002 | Risk and Protection | `docs/specifications/risk/RSK-002_RISK_APPETITE_AND_LIMITS.md` | proposed new specification |
| RSK-003 | Risk and Protection | `docs/specifications/risk/RSK-003_LOSS_CONTAINMENT_AND_SAFE_STATE.md` | proposed new specification |
| RSK-004 | Risk and Protection | `docs/specifications/risk/RSK-004_CRISIS_GOVERNANCE.md` | proposed new specification |
| RSK-005 | Risk and Protection | `docs/specifications/protection/RSK-005_CAPITAL_SAFETY_PLANE.md` | current effective |
| DEC-001 | Decision System | `docs/specifications/decision/DEC-001_DECISION_LIFECYCLE.md` | proposed new specification |
| DEC-002 | Decision System | `docs/specifications/decision/DEC-002_EVIDENCE_AND_DATA_FITNESS.md` | proposed new specification |
| DEC-003 | Decision System | `docs/specifications/decision/DEC-003_ASSUMPTIONS_CONFIDENCE_AND_UNCERTAINTY.md` | proposed new specification |
| DEC-004 | Decision System | `docs/specifications/decision/DEC-004_EXPLAINABILITY_AND_TRACEABILITY.md` | proposed new specification |
| DEC-005 | Decision System | `docs/specifications/decision/DEC-005_OUTCOME_EVALUATION_AND_LEARNING.md` | proposed new specification |
| DEC-006 | Decision System | `docs/specifications/decision/DEC-006_DECISION_LEDGER.md` | current effective |
| AWR-001 | Self-Awareness | `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md` | current effective |
| AWR-002 | Self-Awareness | `docs/specifications/self-awareness/AWR-002_FITNESS_TO_OPERATE.md` | proposed new specification |
| AWR-003 | Self-Awareness | `docs/specifications/self-awareness/AWR-003_CONFIDENCE_AND_UNCERTAINTY.md` | proposed new specification |
| AWR-004 | Self-Awareness | `docs/specifications/self-awareness/AWR-004_TEMPORAL_AWARENESS.md` | proposed new specification |
| AWR-005 | Self-Awareness | `docs/specifications/self-awareness/AWR-005_DRIFT_AND_BLIND_SPOT_DETECTION.md` | proposed new specification |
| AWR-006 | Self-Awareness | `docs/specifications/self-awareness/AWR-006_MAIN_SELF_AWARENESS.md` | current effective |
| AWR-007 | Self-Awareness | `docs/specifications/self-awareness/AWR-007_LOCAL_SELF_AWARENESS.md` | current effective |
| AWR-008 | Self-Awareness | `docs/specifications/self-awareness/AWR-008_COMPONENT_SELF_AWARENESS.md` | current effective |
| INT-001 | Intelligence | `docs/specifications/intelligence/INT-001_INTELLIGENCE_GOVERNANCE.md` | proposed new specification |
| INT-002 | Intelligence | `docs/specifications/intelligence/INT-002_MODEL_AND_STRATEGY_ADMISSION.md` | proposed new specification |
| INT-003 | Intelligence | `docs/specifications/intelligence/INT-003_VALIDATION_CHALLENGE_AND_DRIFT.md` | proposed new specification |
| EVO-001 | Maintenance and Evolution | `docs/specifications/core/EVO-001_SELF_MAINTENANCE_AND_EVOLUTION.md` | current effective |
| EVO-002 | Maintenance and Evolution | `docs/specifications/evolution/EVO-002_PROGRESSIVE_AUTONOMY.md` | proposed new specification |
| EVO-003 | Maintenance and Evolution | `docs/specifications/evolution/EVO-003_SAFE_EVOLUTION_ENVELOPE.md` | proposed new specification |
| EVO-004 | Maintenance and Evolution | `docs/specifications/evolution/EVO-004_DIGITAL_TWIN_AND_SIMULATION.md` | proposed new specification |
| EVO-005 | Maintenance and Evolution | `docs/specifications/evolution/EVO-005_SHADOW_CANARY_PROMOTION_AND_ROLLBACK.md` | proposed new specification |
| AUT-001 | Autonomy and Control | `docs/specifications/core/AUT-001_AUTHORITY_ENGINE.md` | current effective |
| AUT-002 | Autonomy and Control | `docs/specifications/core/AUT-002_GUARDIAN.md` | current effective |
| AUT-003 | Autonomy and Control | `docs/specifications/autonomy/AUT-003_INTERVENTION_REVOCATION_AND_RECOVERY.md` | proposed new specification |
| FIN-001 | Financial Operations | `docs/specifications/financial/FIN-001_MARKET_AND_REFERENCE_DATA.md` | proposed new specification |
| FIN-002 | Financial Operations | `docs/specifications/financial/FIN-002_ORDER_AND_EXECUTION_GOVERNANCE.md` | proposed new specification |
| FIN-003 | Financial Operations | `docs/specifications/financial/FIN-003_POSITION_AND_PORTFOLIO_OPERATIONS.md` | proposed new specification |
| FIN-004 | Financial Operations | `docs/specifications/financial/FIN-004_RECONCILIATION_AND_VALUATION.md` | proposed new specification |
| SYS-001 | OS Foundation | `docs/specifications/core/SYS-001_KERNEL.md` | current effective |
| SYS-002 | OS Foundation | `docs/specifications/core/SYS-002_LIFECYCLE.md` | current effective |
| SYS-003 | OS Foundation | `docs/specifications/foundation/SYS-003_FOUNDATION_SERVICE_OWNERSHIP_AND_CATALOG.md` | current effective |
| SYS-004 | OS Foundation | `docs/specifications/foundation/SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE.md` | current effective |
| SYS-005 | OS Foundation | `docs/specifications/core/SYS-005_SERVICE_BUS.md` | current effective |
| SYS-006 | OS Foundation | `docs/specifications/foundation/SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE.md` | current effective |
| SYS-007 | OS Foundation | `docs/specifications/core/SYS-007_CONFIGURATION.md` | current effective |
| SYS-008 | OS Foundation | `docs/specifications/core/SYS-008_HEALTH_MONITORING.md` | current effective |
| SYS-009 | OS Foundation | `docs/specifications/core/SYS-009_FIL.md` | current effective |
| SYS-010 | OS Foundation | `docs/specifications/core/SYS-010_EVENT_SYSTEM.md` | current effective |
| SYS-011 | OS Foundation | `docs/specifications/core/SYS-011_PERSISTENCE.md` | current effective |
| PLG-001 | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-001_CAPABILITY_PASSPORT_AND_ADMISSION.md` | current effective |
| PLG-002 | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-002_FALCON_CELLS_AND_CAPABILITY_ISOLATION.md` | proposed new specification |
| PLG-003 | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-003_CAPABILITY_UPDATE_MIGRATION_AND_REMOVAL.md` | proposed new specification |
| PLG-004 | Replaceable Capability Ecosystem | `docs/specifications/plugins/PLG-004_SUPPLY_CHAIN_TRUST.md` | proposed new specification |
| SEC-001 | Trust and Security | `docs/specifications/core/SEC-001_SECURITY.md` | current effective |
| SEC-002 | Trust and Security | `docs/specifications/core/SEC-002_FOUNDATION_TRUST_OBJECT_MODEL.md` | current effective |
| SEC-003 | Trust and Security | `docs/specifications/security/SEC-003_AUDITABILITY.md` | proposed new specification |
| FCE-001 | Canonical Representation | `docs/specifications/core/FCE-001_FALCON_CANONICAL_ENCODING_SPECIFICATION.md` | current effective |
| PIPE-001 | Build, Verification, and Promotion | `docs/specifications/foundation/PIPE-001_FOUNDATION_PIPELINE_SPECIFICATION.md` | current effective |
| OPS-001 | Reliability and Operations | `docs/specifications/ops/OPS-001_OBSERVABILITY.md` | proposed new specification |
| OPS-002 | Reliability and Operations | `docs/specifications/ops/OPS-002_FAULT_CONTAINMENT_AND_DEGRADATION.md` | proposed new specification |
| OPS-003 | Reliability and Operations | `docs/specifications/core/OPS-003_RECOVERY.md` | current effective |
| OPS-004 | Reliability and Operations | `docs/specifications/core/OPS-004_LOGGING.md` | current effective |
| EXT-001 | External Relationships | `docs/specifications/external/EXT-001_EXTERNAL_DEPENDENCY_GOVERNANCE.md` | proposed new specification |
| EXT-002 | External Relationships | `docs/specifications/external/EXT-002_BROKER_AND_VENUE_RELATIONSHIP.md` | proposed new specification |
| APP-001 | Applications | `docs/specifications/applications/APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE.md` | current effective |

## 6. Relationship Legend

- `current effective`: presently controlling canonical document.
- `proposed successor`: complete proposed replacement awaiting separate approval and activation.
- `proposed new specification`: governed subject without a current effective document.
- `historical record`: immutable prior decision or preserved baseline.
- `no-change decision`: reviewed and confirmed as unchanged.
- `superseded`: previously current but replaced in controlled lineage.

## 7. Normative Requirements

- **TREE-001-REQ-001:** Every governed ID SHALL have exactly one canonical path.
- **TREE-001-REQ-002:** No canonical path SHALL contain more than one governing ID.
- **TREE-001-REQ-003:** No governed ID SHALL appear in more than one canonical location.
- **TREE-001-REQ-004:** Current effective documents, proposed successors, historical records, and no-change decisions SHALL remain distinguishable.
- **TREE-001-REQ-005:** Preserved historical records SHALL not be rewritten to achieve canonical clarity.
- **TREE-001-REQ-006:** Replaced and superseded relationships SHALL be recorded explicitly.
- **TREE-001-REQ-007:** Orphan documents SHALL be rejected.
- **TREE-001-REQ-008:** Duplicate canonical entries SHALL be rejected.
- **TREE-001-REQ-009:** Ambiguous lineage SHALL be rejected.
- **TREE-001-REQ-010:** A document MAY be proposed as a successor only if its predecessor and relationship are explicit.
- **TREE-001-REQ-011:** A no-change decision SHALL identify the reviewed subject and the reason no canonical change is required.
- **TREE-001-REQ-012:** Historical records SHALL remain searchable without becoming canonical by accident.
- **TREE-001-REQ-013:** Every Specification ID represented in SPEC-000 SHALL appear exactly once in TREE-001.
- **TREE-001-REQ-014:** TREE-001 SHALL use the same canonical path for each Specification ID as SPEC-000.
- **TREE-001-REQ-015:** TREE-001 SHALL not represent any non-Specification record as a Specification ID.

## 8. Failure and Recovery Behavior

If a canonical path is missing, duplicated, ambiguous, or inconsistent:

- the affected record SHALL be marked invalid for canonical use;
- the conflict SHALL be recorded;
- no activation decision SHALL rely on the ambiguous entry;
- the governing lineage SHALL be repaired in the proposal package; and
- no silent fallback SHALL be permitted.

## 9. Invariants

1. One ID, one canonical path.
2. Historical is not current.
3. Proposed is not active.
4. No-change is still a governed decision.
5. A tree is a structural index, not a truth source beyond its own placement rules.
6. TREE-001 carries only structural placement; SPEC-000 carries ownership, versions, and registry meaning.

## 10. Acceptance Evidence

Acceptance requires proof that the tree:

- places each governed ID once;
- shows current, proposed, historical, and no-change status clearly;
- rejects orphan and duplicate entries;
- preserves lineage;
- and remains consistent with the current approved Vision and Constitution.
