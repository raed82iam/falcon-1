# Falcon ADR Index

**Identifier:** ADR-000  
**Version:** 2.6  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
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
