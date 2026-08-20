# Affected Document Inventory

**Status:** Proposed  
**Execution:** Not Authorized

## A. Direct Canonical Successors

| Subject | Current canonical/effective state | Proposed successor or decision | Activation treatment |
|---|---|---|---|
| Foundation self-awareness | `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md`, v1.0 Approved | `successors/AWR-001_SELF_AWARENESS_SYSTEM_v2.1_PROPOSED.md` | coordination review only; activation remains blocked |
| Main Self-Awareness | no canonical active AWR-006 | AMD-008 AWR-006 v2.0 | admit to canonical self-awareness location |
| Local Self-Awareness | no canonical active AWR-007 | AMD-008 AWR-007 v2.0 | admit to canonical self-awareness location |
| Component Self-Awareness | no canonical active AWR-008 | AMD-008 AWR-008 v1.1 | admit to canonical self-awareness location |
| Application boundary/lifecycle | APP-001 is Planned in SPEC-000; no active content | AMD-008 APP-001 v1.1 | admit to canonical Applications location |
| Application Contract | CON-023 absent from current CON-000 | AMD-008 CON-023 v1.1 | admit to canonical Contracts location |
| Service ownership/catalog | SYS-003 is Candidate Migration in SPEC-000; no active content | AMD-008 SYS-003 v1.1 | admit to canonical Foundation location |
| Dependency governance | SYS-004 is Candidate Migration in SPEC-000; no active content | AMD-008 SYS-004 v1.0 | admit to canonical Foundation location |
| Resource governance | SYS-006 is Candidate Migration in SPEC-000; no active content | AMD-008 SYS-006 v1.1 | admit to canonical Foundation location |
| OS/Application alignment ADR | no current ADR-I015 | AMD-008 ADR-I015 | admit to canonical ADR location |

## B. Active Administrative Documents Requiring Versioned Successors

| Document | Current | Proposed successor purpose |
|---|---|---|
| `docs/specifications/SPEC-000_REGISTRY.md` | v1.4 Approved | v1.5 proposed: register the AMD-008 alignment set and preserve AWR-001 lineage |
| `docs/contracts/CON-000_CONTRACT_REGISTRY.md` | v1.6 Approved | v1.7 proposed: add CON-023 and preserve registry meaning |
| `docs/adrs/ADR-000_INDEX.md` | v2.6 Approved | v2.7 proposed: add ADR-I009 historical disposition and ADR-I015 current decision |
| `docs/04_SPECIFICATION_TREE.md` | TREE-001 v1.2 Approved | v1.3 proposed: Foundation/Application/FSA/MSA/LSA/CSA boundaries |
| `docs/05_LEGACY_MIGRATION_MAP.md` | GOV-002 v1.0 Approved | v1.1 proposed: AMD-004/ADR-I009/GOV-061 historical treatment and AMD-008 transition |
| `docs/specifications/core/README.md` | v1.0 Approved | v1.1 proposed: Foundation-only Core boundary and canonical awareness references |
| `docs/06_FALCON_SELF_AWARE_SYSTEM_CONCEPT_AR.md` | v1.0 approved conceptual reference | v1.1 proposed: aligned terminology and hierarchy |
| `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` | v1.2 Approved | v1.3 proposed: documentary trace to GOV-063/CDA decision; no executed verification claims |
| `docs/roadmap/ROADMAP-001_FOUNDATION_GOVERNANCE_AND_SECURITY_BACKLOG.md` | v2.8 Approved | v2.9 proposed: record activation closure and keep Stage 1 separately blocked |
| `docs/releases/FRS-001_READINESS_REPORT.md` | v4.1 Approved | v4.2 proposed documentary-alignment addendum; no implementation-readiness claim |

## C. Governing Documents Read but Not Changed

- `docs/01_FALCON_VISION.md`
- `docs/02_FALCON_CONSTITUTION.md`
- `docs/03_DOCUMENT_AUTHORITY.md`
- GOV-AUT-001
- SEC-001 and SEC-002
- ADR-F001, ADR-F002, ADR-F008
- GOV-063

## C.1 Explicit Dependency-Surface Disposition

| Active document | Path | Disposition | Reason |
|---|---|---|---|
| AUT-001 Authority Engine | `docs/specifications/core/AUT-001_AUTHORITY_ENGINE.md` | reviewed - no content change | already separates evidence from authority |
| CON-002 Authority Decision | `docs/contracts/CON-002_AUTHORITY_DECISION.md` | versioned successor review required | references AWR/Fitness sources; must bind FSA outcome scope |
| CON-006 Health and Fitness | `docs/contracts/CON-006_HEALTH_AND_FITNESS.md` | versioned successor required | current primary AWR-001 meaning must become tier- and scope-aware |
| EVO-001 Self-Maintenance and Evolution | `docs/specifications/core/EVO-001_SELF_MAINTENANCE_AND_EVOLUTION.md` | versioned successor review required | origin-aware Application vs Foundation paths must be explicit |
| ADR-I003 Persistence | `docs/adrs/ADR-I003_FOUNDATION_PERSISTENCE_REALIZATION.md` | reviewed - no decision change; cross-reference successor required | persistence decision remains valid; AWR reference scope must identify FSA |
| ADR-I006 Time and Identity | `docs/adrs/ADR-I006_FOUNDATION_TIME_AND_IDENTITY_REALIZATION.md` | reviewed - no decision change; cross-reference successor required | implementation decision unchanged |
| VPL-005 Health Evidence Loss | `docs/verification/VPL-005_HEALTH_EVIDENCE_LOSS.md` | versioned documentary plan successor required; execution remains unauthorized | scenarios must distinguish FSA/MSA/LSA/CSA |
| FDN-001 State Authority/Persistence Catalog | `docs/foundation/FDN-001_STATE_AUTHORITY_AND_PERSISTENCE_CATALOG.md` | versioned successor review required | state owners must reflect bounded awareness |
| FDN-002 FIL Interaction/Schema Catalog | `docs/foundation/FDN-002_FIL_INTERACTION_AND_SCHEMA_CATALOG.md` | versioned successor review required | cross-tier message identities and boundaries |
| FDN-004 Foundation Configuration Catalog | `docs/foundation/FDN-004_FOUNDATION_CONFIGURATION_CATALOG.md` | formal no-change decision required | Foundation configuration remains domain-neutral |
| GOV-000 Authority Registry | `docs/governance/GOV-000_AUTHORITY_REGISTRY.md` | reviewed - no new authority registration | awareness tiers do not create authority |
| STD-000 Registry | `docs/standards/STD-000_REGISTRY.md` | reviewed - no change | no new Standard |
| GOV-001 Document Authority | `docs/03_DOCUMENT_AUTHORITY.md` | reviewed - no change | governs transition and status normalization |
| SEC-001 / SEC-002 | canonical security specifications | reviewed - no change; trace references added | existing least-authority, integrity, lineage, challenge rules control |

## D. Immutable Historical Documents

- AMD-004 in full
- ADR-I009 in its original AMD-004 location
- GOV-061
- AMD-007 in full
- all earlier AWR-006/AWR-007/AWR-008 approved-design files
- all decision-time reports, approval statements, and evidence

They receive external lineage references only. Their content and original metadata SHALL NOT be edited.

## E. Completeness Blocker

Current AWR-001 v1.0 remains the active current line. The CDA now includes a full proposed AWR-001 v2.1 successor. Coordinated activation SHALL NOT proceed until AWR-001 v2.1, the dependency-surface successors, the administrative successor set, and the FDN-004 no-change decision are separately reviewed and approved.
