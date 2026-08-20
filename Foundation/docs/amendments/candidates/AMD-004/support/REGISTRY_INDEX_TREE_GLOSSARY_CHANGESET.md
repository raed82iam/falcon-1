# Proposed Registry, ADR Index, Specification Tree, and Glossary Change Set

**Status:** Approved Change Set — Execution Deferred  
**Approval Record:** GOV-061  
**Application Rule:** Do not apply to active Approved files before Owner approval.

## 1. Proposed SPEC-000 Changes

Retain:

| ID | Title | Domain | Status |
|---|---|---|---|
| AWR-001 | Self-Awareness System v1.0 | Self-Awareness | Approved until successor activation |
| AWR-002 | Fitness to Operate | Self-Awareness | Planned |
| AWR-003 | Confidence and Uncertainty | Self-Awareness | Planned |
| AWR-004 | Temporal Awareness | Self-Awareness | Planned |
| AWR-005 | Drift and Blind-Spot Detection | Self-Awareness | Planned |

Propose:

| ID | Title | Domain | Proposed Status | Location |
|---|---|---|---|---|
| AWR-001 v2.0 | Falcon Self-Awareness System | Self-Awareness / Foundation | Approved design; activation deferred | AMD-004; GOV-061 |
| AWR-006 | Main Self-Awareness | Self-Awareness / Applications | Approved design; activation deferred | AMD-004; GOV-061 |
| AWR-007 | Local Self-Awareness | Self-Awareness / Application | Approved design; activation deferred | AMD-004; GOV-061 |
| AWR-008 | Component Self-Awareness | Self-Awareness / Component | Approved design; activation deferred | AMD-004; GOV-061 |

After approval only:

- mark AWR-001 v1.0 Superseded by AWR-001 v2.0;
- activate AWR-001 v2.0;
- register AWR-006, AWR-007, and AWR-008 as Approved;
- preserve the old file and approval record.

## 2. Proposed ADR-000 Entry

| ID | Title | Status | Decision Owner | Affected Specifications | Supersedes |
|---|---|---|---|---|---|
| ADR-I009 | Establish Falcon Self-Awareness and Separate Foundation Awareness from Application Awareness | Accepted; activation deferred | Falcon Project Owner | AWR-001–AWR-008, AUT, SYS, SEC, OPS, APP, PLG | GOV-061 |

## 3. Proposed Specification Tree Change

Replace the single undifferentiated AWR branch in a versioned successor with:

```text
AWR — Hierarchical Self-Awareness
├── FSA — Falcon Self-Awareness System
│   ├── Foundation Self Model
│   ├── Foundation Technical Fitness
│   ├── Operational and Structural Integrity
│   ├── Information-Flow Integrity
│   ├── Foundation Change Conformance
│   └── Foundation Recovery and Historical Integrity
├── MSA — Main Self-Awareness
│   ├── Applications Ecosystem Model
│   ├── Cross-Application Dependencies and Conflicts
│   ├── Collective Readiness
│   └── Ecosystem Improvement Coordination
├── LSA — Local Self-Awareness
│   ├── Application Self Model
│   ├── Application Fitness
│   ├── Business-Domain Awareness
│   └── CSA Coordination
└── CSA — Component Self-Awareness
    ├── Component Self Model
    ├── Performance, Quality, and Confidence
    ├── Competence and Limitations
    └── Component Improvement Proposals
```

The SYS branch remains owner of Foundation mechanisms. APP remains owner of Application boundary and admission. AWR defines awareness behavior at declared tiers.

## 4. Proposed Glossary

### Awareness Rank

The ordered scope and escalation position of an awareness tier. Rank does not create jurisdiction, command authority, data ownership, or responsibility inheritance.

### FSA

Falcon Self-Awareness System. Foundation-level operational, structural, integrity, and conformance awareness. FSA is the final Falcon conformance gate for proposed admission within its jurisdiction.

### MSA

Main Self-Awareness. Applications-ecosystem awareness located outside Foundation.

### LSA

Local Self-Awareness. Awareness owned by one Falcon Application or Approved Operating Layer.

### CSA

Component Self-Awareness. Optional awareness for one explicitly eligible intelligent component.

### Foundation Self Model

FSA’s versioned, evidence-based interpretation of Foundation operational, structural, integrity, authority, dependency, recovery, and conformance condition.

### Foundation Technical Fitness

FSA’s scoped assessment of whether Foundation technical conditions support a declared technical operation or admission. It is not Application business fitness and does not grant authority.

### Falcon Conformance Approval

A scoped FSA outcome stating that a proposed admission satisfies applicable Falcon rules within FSA jurisdiction. It is not constitutional, architecture-board, Owner, business, financial, risk, trading, release, deployment, or implementation approval.

### Governed Awareness Summary

A minimal, attributable, privacy-classified, scope-bound representation passed between awareness tiers without transferring ownership of underlying facts.

### Application Business Meaning

The semantics of Application users, customers, accounts, markets, portfolios, orders, positions, strategies, predictions, capital, profit, loss, objectives, and decisions. Foundation shall not own or interpret this meaning.

## 5. Naming Migration Rule

No global replacement is permitted.

| Existing Meaning | Correct Future Term |
|---|---|
| Core operational/technical Self-Awareness | FSA |
| Applications ecosystem awareness | MSA |
| one Application’s awareness | LSA |
| one eligible intelligent component’s awareness | CSA |
| unqualified business or financial awareness | classify by owner before migration |
