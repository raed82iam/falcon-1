# Falcon Legacy Documentation Migration Map

**Version:** 1.1  
**Status:** Approved  
**Approval Record:** GOV-063  
**Documentary Activation:** Active  
**Activation Record:** GOV-092; GOV-093; GOV-094  
**Effective Documentary Instant:** 2026-07-31 22:54:57 +03:00  
**Supersedes:** GOV-002 v1.0  
**Identifier:** GOV-002  
## 1. Purpose

This map determines how existing Falcon documentation relates to the Falcon1 authority model.

Legacy does not mean incorrect. It means the document was created under an earlier identity, authority hierarchy, or documentation discipline and therefore cannot inherit authority without review.

## 2. Foundational Documents

| Existing content | Decision | Reason |
|---|---|---|
| `docs/01_FALCON_VISION.md` | Adopted into Falcon1 | Correctly defines Falcon as an Autonomous Financial Operating System |
| `docs/02_FALCON_CONSTITUTION.md` | Adopted as ratification baseline | Correctly subordinates itself to the Vision and protects the Prime Objective |
| `docs/FALCON_VISION.md` | Archive | Earlier vision is repetitive and contains technical and architectural material |
| `docs/old/foundation/vision.md` | Archive | Superseded identity and scope |
| `docs/old/foundation/constitution.md` | Archive after extraction | Treats the Constitution as highest authority and embeds replaceable architecture and workflow |
| `docs/constitution/falcon_constitution_v1.0.md` | Archive as prior proposal | Defines Falcon primarily as a domain-independent software platform rather than a financial operating system |

## 3. Governance and Review Documents

| Existing content | Decision |
|---|---|
| Constitution review, gap analysis, change log, and extraction report | Preserve as historical review evidence |
| Constitutional compliance checklist | Rewrite against the current Constitution |
| Architecture authority | Decompose into document authority, architecture governance, and decision authority |
| Architecture principles | Reclassify enduring requirements into Specifications; retain design preferences in architecture guidance |
| Engineering manifesto | Preserve as informative culture material only |
| Review checklists | Rewrite as subordinate Standards or Procedures |

## 4. Existing Specifications

### SPEC-000

Replace with the Falcon1 registry.

The former registry begins with software layers and applications. The Falcon1 registry begins with capital stewardship, risk, decisions, intelligence, and autonomy, then defines the operating foundation that serves them.

### KER-001 Guardian

Rewrite as `AUT-002 Guardian Authority`, supported by `RSK-003`, `RSK-004`, `SYS-008`, and `OPS-002`.

The legacy subject is valid, but these claims require correction:

- Guardian is not the “constitutional protector”; the Constitution is protected through governance and compliance.
- Guardian is not automatically the highest operational authority under all conditions.
- Guardian authority must be explicit, bounded, reviewable, and revocable as required by the Constitution.
- Protection of capital is a Falcon-wide obligation, not a Kernel feature.

### KER-002 Lifecycle Manager

Rewrite as `SYS-002 Lifecycle Authority`.

The legacy specification contains useful lifecycle states and responsibilities, but its fixed dependencies, interfaces, and component claims mix requirements with architecture. Those choices require approved Specifications and ADRs.

### Old Specifications

| Legacy subject | Falcon1 target |
|---|---|
| Communication model | SYS-005 |
| Configuration model | SYS-007 |
| Dependency container | SYS-004 |
| Plugin architecture | SYS-001 and APP-001, with ADRs for chosen extension design |
| Service catalog | SYS-003 |

## 5. Existing ADRs

Preserve every accepted legacy ADR as historical evidence. Do not edit its accepted meaning.

Before migration:

1. extract normative requirements into the owning Specification;
2. remove procedural material from the architectural decision;
3. test the decision against the current Vision and Constitution;
4. split bundled decisions;
5. identify current consequences and alternatives; and
6. accept, supersede, reject, or archive through a new review record.

## 6. Standards

The legacy ADR Standard remains useful source material but is superseded for Falcon1 by `STD-003`.

Future Standards shall be introduced only when a recurring quality or evidence problem requires consistent governance. A new Standard shall not be created merely to document a preference.

## 7. Migration Sequence

The controlled migration order is:

1. ratify Vision and Constitution;
2. approve document authority and registries;
3. approve document-control, Specification, and ADR Standards;
4. define CAP, RSK, DEC, and AUT protective Specifications;
5. rewrite Guardian and lifecycle authority against those Specifications;
6. migrate OS foundation requirements;
7. review legacy ADRs against the resulting specification baseline;
8. define financial operations and intelligence Specifications;
9. validate every application and external relationship against the completed protective baseline; and
10. archive superseded documents without destroying history.

This order prevents platform architecture from defining Falcon’s financial purpose.

## AMD-008 Successor Addendum

# GOV-002 — Legacy Documentation Migration Map

**Identifier:** GOV-002  
**Canonical Target:** `docs/05_LEGACY_MIGRATION_MAP.md`  
**Owner:** Falcon Documentation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-063; AMD-004; AMD-007; ADR-I009  
**Implementation Authority:** Not Granted  
**Stage 1 Authority:** Not Granted

## 1. Purpose

GOV-002 records how historical Falcon documentation relates to the AMD-008 aligned successor surface.

It is a migration map, not a migration action.

## 2. Scope

GOV-002 governs:

- historical treatment;
- predecessor and successor relations;
- preserved records;
- canonical transition notes;
- no-change decisions; and
- migration sequencing as a documentary rule.

## 3. Non-Scope

GOV-002 does not:

- move files;
- change canonical pointers;
- activate successors;
- rewrite history; or
- authorize Stage 1.

## 4. Normative Requirements

- **GOV-002-REQ-001:** Historical records SHALL remain immutable.
- **GOV-002-REQ-002:** A migration map SHALL distinguish preserved, replaced, superseded, and no-change outcomes.
- **GOV-002-REQ-003:** A proposed successor SHALL not be treated as active by being listed in a migration map.
- **GOV-002-REQ-004:** AMD-004, GOV-061, and ADR-I009 SHALL remain historical records.
- **GOV-002-REQ-005:** GOV-063 SHALL be referenced as the architectural approval basis only.
- **GOV-002-REQ-006:** No migration SHALL be effective until coordinated documentary activation.

## 5. Migration Principles

- preserve decision-time truth;
- keep active and historical states distinct;
- list exact successor paths only as proposals until activated;
- never imply file movement from the map alone.

## 6. Failure and Conflict Behavior

If historical status, successor status, or canonical path is ambiguous:

- the ambiguity SHALL be visible;
- the map SHALL not invent an answer; and
- the issue SHALL be returned for review.

## 7. Invariants

1. History stays history.
2. Mapping is not movement.
3. Proposed is not active.
4. No-change remains a decision.

## 8. Acceptance Evidence

Acceptance requires a clear distinction between historical record and proposed successor on every mapped line.

## 9. Preservation Matrix

| Migration-map area | Status | Evidence of preservation |
|---|---|---|
| Purpose and scope | Preserved | sections 1–3 remain intact and governing |
| Normative requirements | Preserved | section 4 retains the migration-map obligations |
| Migration principles, failure, invariants | Preserved | sections 5–7 remain explicit and unchanged in meaning |
| Acceptance evidence | Preserved | section 8 keeps the required distinction visible |
