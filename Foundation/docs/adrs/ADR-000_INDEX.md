# Falcon ADR Index

**Version:** 2.7  
**Status:** Approved  
**Approval Record:** GOV-063  
**Documentary Activation:** Active  
**Activation Record:** GOV-092; GOV-093; GOV-094  
**Effective Documentary Instant:** 2026-07-31 22:54:57 +03:00  
**Supersedes:** ADR-000 v2.6  
**Identifier:** ADR-000  
**Owner:** Falcon Architecture Authority

## 1. Current ADR Baseline

No legacy ADR is automatically accepted into the Falcon1 baseline.

The previous ADR collection contains valuable architectural reasoning, but its authority predates the current Vision and Constitution. Each decision requires review for:

- continuing necessity;
- constitutional compatibility;
- correct specification ownership;
- current factual validity;
- duplicate or bundled decisions;
- boundaries and consequences; and
- compliance with STD-003.

## 2. Migration Queue

| Legacy ADR | Subject | Falcon1 disposition |
|---|---|---|
| ADR-001 | Runtime does not own lifecycle | Review under SYS-002 |
| ADR-002 | Event-driven kernel | Review as architectural choice under SYS-005 |
| ADR-003 | Centralized event bus communication | Re-evaluate; centralization is a solution, not a constitutional rule |
| ADR-004 | Service definition and boundaries | Extract requirements to SYS-003; retain residual choice as ADR |
| ADR-005 | Mandatory runtime mediation | Review against authority separation and failure containment |
| ADR-006 | Service contract | Extract contract requirements to SYS-003 |
| ADR-007 | Service registry | Review under SYS-003 |
| ADR-008 | Service runtime layer | Review for scope and continuing necessity |
| ADR-009 | Service bus | Review under SYS-005 |
| ADR-010 series | Self-awareness framework | Decompose across DEC, INT, AUT, SYS, and OPS before deciding architecture |
| ADR-011 | Lifecycle authority model | Review under SYS-002 and AUT-001 |

## 3. Acceptance Rule

A migrated ADR shall receive a new Falcon1 review record. Its original identifier may be preserved when the decision remains singular and semantically continuous. If the decision changes materially, a new ADR shall supersede the legacy record.

## 4. Index Table

| ID | Title | Status | Decision owner | Governing Specifications | Supersedes |
|---|---|---|---|---|---|
| ADR-F001 | Foundation Execution and Isolation Model | Accepted | Falcon Project Owner | SYS-001, SEC-001, FRS-001 | None |
| ADR-F002 | Authoritative State Ownership Model | Accepted | Falcon Project Owner | SYS-002, SYS-011, DEC-006, FRS-001 | None |
| ADR-F003 | Initial Communication Topology | Accepted | Falcon Project Owner | SYS-005, SYS-009, SYS-010, FRS-001 | None |
| ADR-F004 | FIL Representation and Schema Mechanism | Accepted | Falcon Project Owner | SYS-009, CON-004, FRS-001 | None |
| ADR-F005 | Persistence and Evidence Integrity Model | Accepted | Falcon Project Owner | SYS-011, OPS-004, DEC-006, CON-008, FRS-001 | None |
| ADR-F006 | Identity and Trust Bootstrap | Accepted | Falcon Project Owner | SYS-001, SEC-001, CON-001, CON-009, FRS-001 | None |
| ADR-F007 | Configuration Source and Precedence | Accepted | Falcon Project Owner | SYS-007, SEC-001, CON-007, FRS-001 | None |
| ADR-F008 | Safe-State Enforcement Boundary | Accepted | Falcon Project Owner | AUT-001, AUT-002, SYS-001, SYS-002, OPS-003, RSK-005, FRS-001 | None |
| ADR-I001 | Foundation Runtime and Language | Accepted | Falcon Project Owner | SYS-001, SEC-001, PLG-001, FRS-001 | None |
| ADR-I002 | Repository and Dependency Policy | Accepted | Falcon Project Owner | SYS-001, SEC-001, PLG-001, FRS-001 | None |
| ADR-I003 | Foundation Persistence Realization | Accepted | Falcon Project Owner | SYS-011, OPS-003, OPS-004, SYS-008, AWR-001, AUT-001, FRS-001 | None |
| ADR-I004 | Foundation Communication Realization | Accepted | Falcon Project Owner | SYS-005, SYS-009, SYS-010, SEC-001, AUT-001, FRS-001 | None |
| ADR-I005 | Foundation Cryptographic and Secret Profile | Accepted | Falcon Project Owner | SEC-001, SYS-005, SYS-009, SYS-011, OPS-004, FRS-001 | None |
| ADR-I006 | Foundation Time and Identity Realization | Accepted | Falcon Project Owner | SYS-001, SYS-008, SYS-009, SEC-001, AWR-001, AUT-001, FRS-001 | None |
| ADR-I007 | Foundation Build, Verification, and Promotion Pipeline | Accepted | Falcon Project Owner | SYS-001, SEC-001, SEC-002, AUT-001, AUT-002, EVO-001, OPS-004, FRS-001 | None |
| ADR-I008 | Foundation Bootstrap and Activation Sequencing | Accepted | Falcon Project Owner | SYS-001, SEC-001, SEC-002, AUT-001, AUT-002, PIPE-001, FRS-001 | None |
| ADR-I009 | Self-Awareness Hierarchy and Boundary | Superseded | Falcon Project Owner | AWR-001, AWR-006, AWR-007, AWR-008 | Superseded by ADR-I015; historical text remains immutable |
| ADR-I012 | Foundation Plug-and-Play Application Integration Boundary | Accepted | Falcon Project Owner | APP-001, CON-023, SYS-005, SYS-009, SYS-010, PLG-001 | None |
| ADR-I015 | Falcon OS Application and Awareness Alignment | Accepted | Falcon Project Owner | AWR-001, AWR-006, AWR-007, AWR-008, APP-001, SYS-003, SYS-004, SYS-006 | Current alignment decision |

## Active AMD-008 Alignment Addendum

**Documentary Formatting Note:** This revision corrects mojibake/encoding artifacts only. No ADR meaning, authority, status, lineage, normative requirement, or historical decision content is changed.

# ADR-000 — Architectural Decision Register

**Identifier:** ADR-000  
**Canonical Target:** `docs/adrs/ADR-000_INDEX.md`  
**Owner:** Falcon Architecture Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-063; ADR-I009; ADR-I015  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted

## 1. Purpose

ADR-000 is the canonical index of Falcon architectural decisions.

It records decisions, dates, dispositions, and lineage without rewriting historical decisions.

## 2. Scope

ADR-000 governs:

- decision identity;
- decision title;
- version;
- disposition;
- approval record;
- supersession lineage;
- historical preservation; and
- index-level cross-reference clarity.

## 3. Non-Scope

ADR-000 does not:

- replace the text of a historical ADR;
- grant activation authority;
- grant implementation authority;
- infer acceptance from indexing alone; or
- reinterpret prior decisions silently.

## 4. Normative Requirements

- **ADR-000-REQ-001:** Every ADR SHALL have one index entry.
- **ADR-000-REQ-002:** The index SHALL distinguish accepted, proposed, historical, and superseded decisions.
- **ADR-000-REQ-003:** Historical ADR text SHALL remain immutable.
- **ADR-000-REQ-004:** A later decision SHALL be reflected in the index only when its relationship is explicit.
- **ADR-000-REQ-005:** ADR-I009 SHALL remain an immutable historical decision.
- **ADR-000-REQ-006:** ADR-I015 SHALL be represented as the AMD-008 alignment decision.
- **ADR-000-REQ-007:** The index SHALL not rewrite the meaning of GOV-061 or AMD-004.

## 5. Failure and Conflict Behavior

If index and historical decision text conflict:

- the historical decision text SHALL prevail for its own meaning;
- the index SHALL show the discrepancy;
- the discrepancy SHALL be challengeable; and
- no silent correction SHALL occur.

## 6. Invariants

1. Indexing is not rewriting.
2. Historical ADR text remains authoritative for its own record.
3. Later decisions are indexed, not retrofitted.
4. Acceptance by index is invalid.

## 7. Acceptance Evidence

Acceptance requires correct lineage for ADR-I009 and ADR-I015 and no alteration of historical ADR content.

## 8. Preservation Matrix

| Index area | Status | Evidence of preservation |
|---|---|---|
| Purpose and scope | Preserved | sections 1–3 remain intact and governing |
| Normative requirements | Preserved | section 4 keeps all index requirements and historical obligations |
| Failure, invariants, acceptance | Preserved | sections 5–7 remain explicit and unchanged in meaning |

