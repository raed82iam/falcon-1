# Document Diff and Migration Matrix

**Status:** Proposed  
**Migration:** Not Authorized

| Document | Source path/version/record | Exact controlled delta | Target path/version/record | Action/order | Rollback target |
|---|---|---|---|---|---|
| AWR-001 | `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md` v1.0, GOV-003, §§1–4 and REQ-011 | remove Foundation ownership of financial/material exposure; add FSA-only scope, mandatory MSA/LSA rules, four origin paths, FSA non-adoption authority | `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md` v2.1; approval record not issued; activation record not issued | separately approve, then stage first | v1.0 |
| AWR-006 | AMD-004 design and AMD-008 v2.0, GOV-061/GOV-063 | ecosystem owner → exactly one Application; MSA-origin direct-to-FSA | `docs/specifications/self-awareness/AWR-006_MAIN_SELF_AWARENESS.md` v2.0, GOV-063 + activation record | after AWR-001 | no prior canonical; archive admitted file on rollback |
| AWR-007 | AMD-004 design and AMD-008 v2.0 | whole Application → exactly one major branch; LSA-origin route | `docs/specifications/self-awareness/AWR-007_LOCAL_SELF_AWARENESS.md` v2.0 | after AWR-006 | no prior canonical |
| AWR-008 | AMD-004 design and AMD-008 v1.1 | retain eligibility; clarify CSA-only origin route and no artificial insertion | `docs/specifications/self-awareness/AWR-008_COMPONENT_SELF_AWARENESS.md` v1.1 | after AWR-007 | no prior canonical |
| APP-001 | SPEC-000 v1.4 Planned; AMD-008 v1.1 | add full plug-in lifecycle, exactly one MSA, exactly one LSA per major branch, origin-aware routing | `docs/specifications/applications/APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE.md` v1.1 | after AWR/SYS | SPEC-000 v1.4 Planned state |
| CON-023 | absent in CON-000 v1.6; AMD-008 v1.1 | add uniform Application Contract, branch-to-LSA cardinality, origin and activation references | `docs/contracts/CON-023_APPLICATION_CONTRACT_AND_MANIFEST.md` v1.1 | after APP/SYS/AWR | CON-000 v1.6 absence |
| SYS-003 | SPEC-000 Candidate Migration; AMD-008 v1.1 | single owner, exclusive responsibility, consumer/lifecycle/authority fields | `docs/specifications/foundation/SYS-003_FOUNDATION_SERVICE_OWNERSHIP_AND_CATALOG.md` v1.1 | before APP | registry Candidate Migration state |
| SYS-004 | SPEC-000 Candidate Migration; AMD-008 v1.0 | explicit dependency identity/version/order/failure/containment | `docs/specifications/foundation/SYS-004_APPLICATION_DEPENDENCY_GOVERNANCE.md` v1.0 | before APP | registry Candidate Migration state |
| SYS-006 | SPEC-000 Candidate Migration; AMD-008 v1.1 | Foundation totals/quotas vs Application internal distribution | `docs/specifications/foundation/SYS-006_MULTI_LEVEL_RESOURCE_GOVERNANCE.md` v1.1 | before APP | registry Candidate Migration state |
| ADR-I009 | AMD-004 original, GOV-061 | no content edit; later decision disposition only | original path unchanged; ADR-000 v2.7 points to ADR-I015 | index lineage before AWR | ADR-000 v2.6 |
| ADR-I015 | AMD-008, GOV-063 | canonical admission; document status vs ADR disposition separated | `docs/adrs/ADR-I015_FALCON_OS_APPLICATION_AND_AWARENESS_ALIGNMENT.md`, GOV-063 + activation record | after all content staged | ADR-000 v2.6 |
| SPEC-000 | canonical v1.4, GOV-003/GOV-012/GOV-014/GOV-020 | add AWR-006/7/8; update APP-001/SYS-003/4/6; paths/owners/records | canonical v1.5, future activation record | atomic baseline | v1.4 archive path in doc 04 |
| CON-000 | canonical v1.6, GOV-030 | add CON-023 v1.1 and governing links | canonical v1.7 | atomic baseline | v1.6 |
| ADR-000 | canonical v2.6, GOV-003 | add I015; record I009 historical disposition | canonical v2.7 | atomic baseline | v2.6 |
| TREE-001 | canonical v1.2, GOV-003/GOV-014/GOV-020 | replace unified AWR description; expand APP; own diagram | canonical v1.3 | atomic baseline | v1.2 |
| GOV-002 | canonical v1.0, GOV-003 | add exact historical/transition map | canonical v1.1 | atomic baseline | v1.0 |
| Core README | canonical v1.0, GOV-003 | define Foundation-only Core and point to bounded AWR | canonical v1.1 | atomic baseline | v1.0 |
| Concept AR | canonical v1.0, GOV-003 | align Arabic terms/hierarchy; preserve business concept outside FSA | canonical v1.1 | after GLO-001 | v1.0 |
| GLO-001 | none | create controlled terms without creating requirements | `docs/glossary/GLO-001_FALCON_FOUNDATION_AND_APPLICATION_TERMINOLOGY.md` v1.0, approval/activation not issued | before cross-references | archive admitted file under rollback |
| TRC-001 | canonical v1.2, GOV-040 | add GOV-063/CDA obligations and static review traces; no execution claims | canonical v1.3 | staged baseline | v1.2 |
| ROADMAP-001 | canonical v2.8 | record CDA outcome while retaining Stage 1 block | canonical v2.9 | after activation decision | v2.8 |
| FRS-001-READINESS | canonical v4.1, GOV-046 | assess documentary activation only; no implementation readiness | canonical v4.2 | after post-audit | v4.1 |

## Explicit Historical and Approval-Record Disposition

| Record | Diff | Migration |
|---|---|---|
| AMD-004 | none; original hierarchy remains decision-time truth | keep exact path/content/status; add manifest lineage only |
| GOV-061 | none | keep immutable; reference from GOV-002 v1.1 and activation manifest |
| ADR-I009 source | none | no edit; ADR-000 records later supersession |
| AMD-007 | none | keep Proposed historical gap package; activation manifest identifies it as non-canonical evidence |
| AMD-008 | no semantic migration; preserve GOV-063-approved package | keep candidate package as approval provenance; canonical successors are distinct copies with normalized metadata |
| GOV-063 | none | remains architectural approval record, not activation record |
| CDA-AMD008-001 | Proposed → retained preparation/decision evidence after future decision | never becomes the canonical source for specifications/contracts |

## Dependency-Surface Documents

| Document | Clause impact | Disposition |
|---|---|---|
| CON-002 | AWR/Fitness evidence source and authority boundary | prepare versioned successor before activation |
| CON-006 | unified Health/Fitness semantics | prepare tier-aware versioned successor |
| EVO-001 | proposal origins and Foundation/Application evolution | prepare versioned successor or exact no-change proof |
| ADR-I003 | AWR reference only | new ADR index/cross-reference note; decision unchanged |
| ADR-I006 | AWR reference only | cross-reference note; decision unchanged |
| VPL-005 | loss scenarios | prepare documentary plan successor; do not execute |
| FDN-001 | authoritative state owners | prepare versioned Catalog successor |
| FDN-002 | awareness messages | prepare versioned Catalog successor |
| FDN-004 | configuration values | exact semantic scan; no change unless tier values found |

## Migration Invariants

- all target documents are staged before any current pointer changes;
- the transition is one atomic documentary baseline;
- no runtime, code, verification execution, or Stage 1 artifact is changed;
- every old version remains retrievable with digest and lineage;
- no historical decision text is altered.
