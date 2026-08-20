# Registry, Tree, Glossary, Diagram, and Cross-Reference Change Set

**Status:** Proposed  
**Execution:** Not Authorized

## Registry and Index Updates

### SPEC-000 v1.5

- retain AWR-001 as current until its aligned successor is approved;
- add AWR-006 `Main Self-Awareness — One Application`;
- add AWR-007 `Local Self-Awareness — One Major Application Branch`;
- add AWR-008 `Component Self-Awareness — Eligible Component`;
- change APP-001 to `Application Boundary and Lifecycle`, Approved;
- change SYS-003, SYS-004, and SYS-006 from Candidate Migration to Approved;
- assign canonical paths and accountable owners;
- record GOV-063 and the future activation record.

### CON-000 v1.7

Add CON-023 v1.1 with APP-001, AWR-006, AWR-007, AWR-008, SYS-003, SYS-004, SYS-006, SEC-001, and AUT-001 as governing/affected specifications.

### ADR-000 v2.7

- add ADR-I015 as current architecture;
- retain ADR-I009 as immutable historical decision and record `Superseded by ADR-I015` in the index only;
- distinguish architectural acceptance from runtime authority.

## Specification Tree v1.3

Replace the unified AWR description with:

```text
AWR — Bounded Self-Awareness
├── Foundation Self-Awareness (FSA)
├── Application Main Self-Awareness (exactly one MSA per Application)
├── Branch Local Self-Awareness (exactly one LSA per major branch)
└── Component Self-Awareness (optional; eligible intelligent components only)
```

Expand APP to include Application Contract, lifecycle, isolation, resource boundary, dependencies, health, recovery, awareness integration, and removal.

## GLO-001 — Falcon Foundation and Application Terminology v1.0

A new canonical glossary is required because no current standalone glossary exists.

**Proposed Identifier:** GLO-001  
**Class:** Governed reference/index; it organizes meaning owned by Specifications and SHALL NOT create requirements  
**Accountable Document-Owner Function:** documentation terminology stewardship; this label creates no jurisdiction  
**Required Approval Authority:** Project Owner under GOV-001  
**Governing Authority:** GOV-001; TREE-001; ADR-I015; AWR-001/AWR-006/AWR-007/AWR-008; APP-001  
**Canonical Path:** `docs/glossary/GLO-001_FALCON_FOUNDATION_AND_APPLICATION_TERMINOLOGY.md`  
**Approval Record:** required before activation  
**Activation Record:** future CDA Owner decision  

Required entries:

- Falcon Foundation / Falcon OS;
- Falcon Application;
- FSA;
- MSA;
- Major Application Branch;
- LSA;
- Eligible Intelligent Component;
- CSA;
- Application Contract;
- documentary approval;
- coordinated documentary activation;
- production adoption;
- supersession.

Each entry SHALL state scope, owner, forbidden interpretation, and source authority.

## TREE-001-Owned Canonical Architecture Diagram

The diagram is not a standalone authority artifact. It SHALL be embedded in and owned by proposed TREE-001 v1.3 at `docs/04_SPECIFICATION_TREE.md`. GLO-001 may reference it but SHALL NOT duplicate or redefine it.

```text
Falcon Foundation / Falcon OS
  └─ FSA

Application A
  └─ MSA-A
      ├─ Major Branch A1 ─ LSA-A1 ─ optional eligible CSAs
      └─ Major Branch A2 ─ LSA-A2 ─ optional eligible CSAs

Application B
  └─ MSA-B
```

Origin-aware development routes:

```text
CSA → Parent LSA → Application MSA → FSA
LSA → Application MSA → FSA
Application MSA → FSA
FSA → separate Foundation self-development governance and approval lifecycle
```

## Cross-Reference Updates

New versions SHALL replace active references that:

- call MSA an Applications-ecosystem owner;
- call LSA the awareness owner of a whole Application;
- imply every proposal begins at CSA;
- imply FSA grants production adoption;
- mix Foundation awareness with financial/business awareness;
- treat APP-001, SYS-003, SYS-004, or SYS-006 as lacking approved content after activation.

Historical files retain their original references and receive no edits.
