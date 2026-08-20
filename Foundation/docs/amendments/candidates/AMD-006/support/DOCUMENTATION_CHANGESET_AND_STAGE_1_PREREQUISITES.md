# Documentation Change Set and Stage 1 Prerequisites

**Status:** Approved Change Set — Execution Not Authorized  
**Approval Record:** GOV-062

## 1. Proposed Registry Changes

- AUT-002 title → Falcon Foundation Guardian, version 2.1.
- reserve RSK-006 → Trading Guardian.
- reserve CON-022 → Application Guardian Protection Request.
- add ADR-I011 to ADR index.
- add FFG, Application Guardian, Trading Guardian, Platform Safe Mode, Trading Safe Mode, and technical protection request to glossary.

## 2. Proposed Architecture Diagrams

```text
Falcon Foundation
├── FSA
├── FFG
├── AUT-001
└── Foundation execution mechanisms

Falcon Applications Environment
└── Trading Application Suite
    ├── FSATA
    ├── FSAOL
    ├── Trading Risk
    ├── Broker Execution
    ├── Provider Management
    └── Trading Guardian
```

Named Trading Applications above are required conceptual boundaries, not existing Approved Specifications.

## 3. Manifest Requirements

An Application Manifest SHALL declare identity, technical criticality, dependencies, minimum resources, required routes, persistence/security/authority needs, isolation/restart/failover policy, degraded modes, recovery priority, and Guardian dependency.

The Trading Suite Manifest SHALL declare Trading Guardian mandatory and define fail-closed behavior when Guardian identity, version, policy, authority, evidence, communication, or recovery readiness is insufficient.

## 4. Required Matrices and Owners

| Matrix | Required owner |
|---|---|
| technical criticality | competent Foundation governance |
| protection request | AUT-002/CON-022 |
| isolation authority | FFG/AUT-001 |
| release authority | consequence-class governance |
| Trading protection | RSK-006 and Trading Risk authority |
| execution routing | competent execution Specifications |

## 5. Cross-Reference Updates

Review and version as necessary:

- SPEC-000, TREE-001, CON-000, Core index, ADR index, glossary;
- CON-011, CON-006, FDN-005, ADR-F008, VPL-006, VPL-007;
- AWR-001 v2.0, AWR-006, AWR-007, AWR-008;
- SYS-002, SYS-005, SYS-008, SYS-009, SYS-011;
- SEC-001, OPS-003, PLG-001, RSK-005;
- future APP-001, SYS-003, SYS-006, Trading Risk, Broker Execution, FSATA, FSAOL, and Provider Management.

## 6. Stage 1 Prerequisites

Before Stage 1 Guardian implementation:

- all AMD-006 governing documents Approved and activated documentarily;
- APP-001, SYS-003, and SYS-006 boundaries approved;
- Trading Suite and its mandatory Application specifications approved;
- Guardian authority charters and consequence classes approved;
- technical criticality, survival set, trigger, release, HA, stop-channel, and duration/quorum decisions approved;
- Contracts and FIL schemas approved;
- acceptance plans approved for execution;
- isolated environment, evidence requirements, and implementation authority separately approved.

AMD-006 grants none of these approvals.
