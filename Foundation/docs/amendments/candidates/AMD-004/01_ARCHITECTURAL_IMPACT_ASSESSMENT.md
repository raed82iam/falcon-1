# AMD-004 Architectural Impact Assessment

**Status:** Approved Assessment  
**Approval Record:** GOV-061  
**Assessment Date:** 2026-07-27  
**Scope:** Active Stage 0 architectural baseline and source artifacts  
**Stage 1 Authority:** Not Granted

## 1. Executive Summary

The current baseline places one Approved `AWR-001 — Self-Awareness System` inside Core and gives it a unified Self Model containing operational, financial, decisional, capability, dependency, temporal, and authority awareness.

That model is internally consistent with the original architecture, but it conflicts with the newly directed boundary that Foundation awareness shall remain domain-independent and shall not interpret Application business meaning.

The required correction is bounded:

1. preserve AWR-001 v1.0 as Approved history;
2. propose AWR-001 v2.0 as FSA, limited to Foundation operational, structural, integrity, and conformance awareness;
3. move Applications-ecosystem awareness to proposed MSA;
4. place Application-owned awareness in proposed LSA;
5. place eligible intelligent-component awareness in proposed CSA;
6. preserve Guardian, Authority Engine, Security, Health Monitoring, Risk, and governance as separate authorities;
7. revise references by meaning, never by blind replacement.

No Stage 0 source implements the current AWR-001 or Guardian as operational components. The correction is therefore architectural and documentary; no runtime migration is required.

## 2. Source-of-Truth Review

The review used the active hierarchy in GOV-001 v1.2:

```text
Vision
  ↓
Constitution
  ↓
Governance
  ↓
Specifications and Standards
  ↓
ADRs
  ↓
Contracts, Catalogs, Designs, Plans, Policies
  ↓
Implementation, Verification, Evidence
```

No archived document was treated as active authority.

## 3. Findings

### F-001 — Foundation Self-Awareness Owns Financial Awareness

- **Classification:** Responsibility conflict; Required pre-Stage 1 correction
- **Severity:** Critical
- **File:** `docs/specifications/core/AWR-001_SELF_AWARENESS_SYSTEM.md`
- **Sections and lines:** Purpose, lines 12–16; Self Model, lines 46–61; AWR-001-REQ-011, line 94
- **Current wording:** “financial ... condition”; “financial state and material exposure”; “correlate ... financial ... evidence”
- **Required correction:** Replace the active meaning through proposed AWR-001 v2.0. FSA may receive opaque impact categories or governed summaries but shall not own or interpret financial state, exposure, portfolios, orders, positions, capital, profit, loss, or business fitness.
- **Correct owner:** MSA for ecosystem summaries; relevant LSA for Application business state; CSA for eligible component state; CAP/FIN/RSK and domain authorities for financial facts and decisions.
- **Reason:** Foundation domain independence requires technical protection without business ownership.
- **Stage 1 impact:** Blocks Foundation Self-Awareness implementation until Owner approval.

### F-002 — Concept Document Places Financial Detail in Core Awareness

- **Classification:** Documentation inconsistency; Required pre-Stage 1 correction
- **Severity:** Critical
- **File:** `docs/06_FALCON_SELF_AWARE_SYSTEM_CONCEPT_AR.md`
- **Sections and lines:** 5 “معنى Self-Awareness,” lines 74–98; 8 “قلب Falcon,” lines 146–177; 9 “Self-Awareness Engine,” lines 179–196
- **Current wording:** The financial-awareness section says Falcon knows available and committed capital, positions, exposures, obligations, liquidity, concentration, correlations, and potential capital effect; the Core section places Self-Awareness Engine in Core.
- **Required correction:** Preserve the original document, then approve a versioned successor separating FSA from MSA/LSA/CSA. Core placement remains correct only for FSA; financial detail moves to Application/domain awareness.
- **Correct owner:** FSA for Foundation awareness; MSA/LSA/CSA and financial domains for business awareness.
- **Reason:** The current document predates the corrected hierarchy.
- **Stage 1 impact:** Blocks reliance on the current conceptual wording for implementation.

### F-003 — Specification Tree Defines AWR as Financially Aware

- **Classification:** Boundary ambiguity; Specification conflict
- **Severity:** High
- **File:** `docs/04_SPECIFICATION_TREE.md`
- **Section and lines:** AWR tree, lines 47–55; AWR domain boundary, lines 181–183
- **Current wording:** AWR maintains a model of Falcon’s “financial, operational, decisional, epistemic, capability, dependency, and authority condition.”
- **Required correction:** Proposed tree successor shall define hierarchical awareness scopes and distinguish Foundation FSA from Applications MSA/LSA/CSA.
- **Correct owner:** AWR remains the specification domain; each awareness tier owns only its declared scope.
- **Stage 1 impact:** Must be corrected before awareness specifications guide implementation.

### F-004 — Core Index Treats One AWR-001 as the Whole Awareness System

- **Classification:** Naming correction; Documentation inconsistency
- **Severity:** High
- **Files and lines:** `docs/specifications/core/README.md`, lines 31–52; `docs/specifications/SPEC-000_REGISTRY.md`, lines 56–60; `docs/releases/FRS-001_FOUNDATION_RELEASE.md`, lines 36–54
- **Current wording:** AWR-001 is “Self-Awareness System” and is listed as a Core component maintaining Falcon’s model of itself and Fitness.
- **Required correction:** Register AWR-001 v2.0 as FSA; register proposed AWR-006/007/008; describe only FSA as Core/Foundation.
- **Correct owner:** Specification Authority after Owner approval.
- **Stage 1 impact:** Mandatory before implementation planning relies on registry placement.

### F-005 — Fitness Contract Does Not Separate Technical from Business Fitness

- **Classification:** Boundary ambiguity
- **Severity:** High
- **File:** `docs/contracts/CON-006_HEALTH_AND_FITNESS.md`
- **Sections and lines:** Purpose, lines 11–13; Fitness Assessment, lines 31–45; Obligations, lines 47–56
- **Current wording:** One Fitness contract applies to AWR-001 without tier or domain ownership.
- **Required correction:** A later proposed contract revision shall distinguish Foundation Technical Fitness, Applications Ecosystem Readiness, Application Fitness, and Component Fitness. Existing CON-006 remains valid for Foundation scope until a successor is approved.
- **Correct owner:** FSA owns Foundation Technical Fitness; MSA owns collective Applications readiness; LSA owns Application fitness; CSA owns component assessment. Authority decisions remain AUT-001.
- **Stage 1 impact:** Contract revision is required before multi-tier awareness integration.

### F-006 — Foundation Persistence Catalog Assigns One Self Model Owner

- **Classification:** Specification conflict
- **Severity:** High
- **File:** `docs/foundation/FDN-001_STATE_AUTHORITY_AND_PERSISTENCE_CATALOG.md`
- **Sections and lines:** State Ownership Catalog, lines 20–36; Contract Ownership, lines 42–52
- **Current wording:** “Self Model | Self-Awareness Authority”; “Fitness to Operate | Self-Awareness Authority.”
- **Required correction:** Proposed catalog successor shall rename these to Foundation Self Model and Foundation Technical Fitness owned by FSA. Application and component awareness state shall remain outside Foundation ownership.
- **Correct owner:** FSA for Foundation records; each MSA/LSA/CSA owner for its own records.
- **Stage 1 impact:** Mandatory before persistence implementation for awareness state.

### F-007 — Foundation FIL Catalog Uses Generic Fitness Messages

- **Classification:** Boundary ambiguity
- **Severity:** Medium
- **File:** `docs/foundation/FDN-002_FIL_INTERACTION_AND_SCHEMA_CATALOG.md`
- **Section and lines:** Interaction Catalog, lines 20–41
- **Current wording:** `foundation.fitness.query.v1` and `foundation.fitness.response.v1` address generic “Self-Awareness.”
- **Required correction:** Preserve Foundation messages but define them as FSA Foundation Technical Fitness messages. Application-level awareness interactions require separately owned schemas and message types.
- **Correct owner:** FSA/Communication Authority for Foundation envelope and technical interaction; owning Application awareness tier for payload meaning.
- **Stage 1 impact:** Required before Application awareness messages are introduced.

### F-008 — Foundation Configuration Uses Generic Fitness Scope

- **Classification:** Naming correction; Boundary ambiguity
- **Severity:** Medium
- **File:** `docs/foundation/FDN-004_FOUNDATION_CONFIGURATION_CATALOG.md`
- **Section and lines:** Catalog, lines 27–55
- **Current wording:** `falcon.fitness.required_scope` owned by Self-Awareness Authority.
- **Required correction:** Proposed successor shall name Foundation technical scope explicitly and bind ownership to FSA.
- **Correct owner:** FSA for Foundation technical fitness configuration.
- **Stage 1 impact:** Required before operational FSA configuration.

### F-009 — Accepted ADRs Refer to AWR-001 by the Old Unified Meaning

- **Classification:** Migration requirement
- **Severity:** Medium
- **Files and lines:** ADR-I003 line 160; ADR-I004 line 234; ADR-I005 line 380; ADR-I006 line 460
- **Current wording:** Self-Awareness updates limitations and Fitness following persistence, communication, cryptographic, or temporal degradation.
- **Required correction:** These references mean Foundation operational awareness and shall map to FSA. Accepted ADRs shall not be overwritten; ADR-I009 shall provide the authoritative interpretation and future successor work may update cross-references.
- **Correct owner:** FSA.
- **Stage 1 impact:** No ADR reversal is required; mapping must be approved before implementation.

### F-010 — Decision, Evolution, Risk, and Standards Use an Undifferentiated Self Model

- **Classification:** Boundary ambiguity; Migration requirement
- **Severity:** High
- **Files and lines:** DEC-006 lines 25, 51, 72; EVO-001 lines 79 and 96; RSK-005 line 46; STD-006 line 61; STD-009 line 75; STD-010 line 52; STD-011 line 34
- **Current wording:** References to “Self Model” and “Fitness to Operate” do not identify awareness tier.
- **Required correction:** Every future reference shall declare FSA, MSA, LSA, or CSA scope. Existing Foundation references map to FSA only where the subject is Foundation technical fitness.
- **Correct owner:** The specification or standard owning the decision, evolution, risk, or evidence rule.
- **Stage 1 impact:** Required before awareness-tier integration or self-evolution implementation.

### F-011 — No Existing MSA, LSA, or CSA Specification

- **Classification:** Stage 1 risk; Required pre-Stage 1 correction
- **Severity:** Critical
- **Files and lines:** `docs/specifications/SPEC-000_REGISTRY.md`, lines 56–60; `docs/04_SPECIFICATION_TREE.md`, lines 47–55
- **Current wording:** AWR-002 through AWR-005 are reserved for Fitness, Confidence, Temporal Awareness, and Drift; no MSA, LSA, or CSA entry exists.
- **Required correction:** Propose non-conflicting reservations AWR-006, AWR-007, and AWR-008. They acquire no permanent authority until Owner approval.
- **Correct owner:** Falcon Specification Authority under Project Owner approval.
- **Stage 1 impact:** Blocks implementation of MSA/LSA/CSA.

### F-012 — FSA Conformance Gate Does Not Yet Exist

- **Classification:** Authority conflict risk; Required pre-Stage 1 correction
- **Severity:** Critical
- **Files and lines:** AWR-001 lines 89–94; AUT-001 lines 52–71; GOV-AUT-001 lines 200–224; PIPE-001 promotion and authority sections; ADR-I007 evaluation/promotion authority sections; SEC-002 validity, acceptance, and reliance sections
- **Current wording:** Current AWR-001 explicitly says Fitness does not grant permission. No document defines FSA conformance approval.
- **Required correction:** ADR-I009 and AWR-001 v2.0 shall define a bounded Falcon Conformance Decision. It may gate admission but shall not amend the Constitution, create jurisdiction, replace Architecture Board or Owner approval, decide business correctness, or authorize deployment/financial action.
- **Correct owner:** FSA for conformance assessment; governing authorities retain jurisdiction and acceptance.
- **Reason:** Without explicit separation, “final gate” could be misread as supreme authority.
- **Stage 1 impact:** Mandatory before any conformance-gate implementation.

### F-013 — Human Identity Wording Is Broader Than Foundation Technical Identity

- **Classification:** Boundary ambiguity
- **Severity:** Medium
- **Files and lines:** SEC-001 lines 60–66; CON-009 lines 15–40; CON-018 lines 19–40
- **Current wording:** “Every governed actor”; “authenticated subject identity”; Identity Subject may be “human.”
- **Required correction:** Application users shall remain Application-owned. Foundation identity covers technical entities, Falcon administration, governed reviewers, and actors exercising Falcon-level authority only.
- **Correct owner:** Application or Application-level identity capability for Application users; Foundation Security for Falcon technical identities.
- **Stage 1 impact:** Clarification required before shared identity implementation.

### F-014 — Guardian Must Remain Separate from FSA

- **Classification:** Authority-boundary risk
- **Severity:** High
- **Files and lines:** AUT-002 lines 54–68; CON-011 lines 16–53; FDN-005 lines 15–31 and 67–88; ADR-F008 lines 63–99
- **Current wording:** Guardian owns binding protective restrictions; Self-Awareness supplies Fitness evidence.
- **Required correction:** Preserve this separation. FSA may detect, assess, reject conformance admission, and request containment. Guardian remains owner of protective restrictions within its mandate.
- **Correct owner:** Guardian for protective restriction; FSA for awareness and conformance.
- **Stage 1 impact:** Mandatory design invariant.

### F-015 — Stage 0 Source Contains No Operational Awareness Implementation

- **Classification:** No correction required
- **Severity:** Informational
- **Files:** `src/Falcon.Stage0B.Candidates`; `src/Falcon.Foundation.Enabling`; Stage 0 verification source and closure evidence
- **Current state:** Source implements only bounded enabling providers, trust primitives, and verification/pipeline capabilities. No operational AWR-001, MSA, LSA, CSA, Guardian, or Falcon-wide runtime exists.
- **Required correction:** None.
- **Correct owner:** Not applicable.
- **Stage 1 impact:** No runtime migration; architecture must be approved before implementation.

### F-016 — No Constitutional Text Names MSA, LSA, or CSA

- **Classification:** No correction required
- **Severity:** Informational
- **File:** `docs/02_FALCON_CONSTITUTION.md`
- **Current wording:** The Constitution requires evidence-based understanding of Falcon’s condition and preserves bounded authority, separation, traceability, and safe failure.
- **Required correction:** None.
- **Reason:** The hierarchy is a subordinate architecture realization consistent with constitutional duties.
- **Stage 1 impact:** Owner approval is required for the architecture package; constitutional amendment is not required.

## 4. Required Migration Actions

1. Approve ADR-I009.
2. Approve AWR-001 v2.0 and only then mark AWR-001 v1.0 Superseded.
3. Approve identifier reservations and specifications AWR-006, AWR-007, and AWR-008.
4. Approve proposed registry, tree, glossary, and index changes.
5. Prepare later versioned successors for CON-006, FDN-001, FDN-002, and FDN-004.
6. Map accepted ADR references to FSA through ADR-I009 without rewriting historical ADR text.
7. Require explicit awareness-tier qualification in future DEC, EVO, RSK, standards, schemas, and evidence.
8. Preserve Application business awareness requirements by reallocating them to MSA, LSA, CSA, or domain authorities.
9. Keep Stage 1 blocked until the Owner approves the required package.

## 5. Approval Requirements

The Project Owner must decide:

- acceptance of ADR-I009;
- activation of AWR-001 v2.0;
- supersession of AWR-001 v1.0;
- reservation and activation of AWR-006, AWR-007, and AWR-008;
- acceptance of FSA conformance jurisdiction and its limits;
- authorization for later contract/catalog successor work;
- whether a Falcon Architecture Board is already constituted or remains a future governance dependency.
