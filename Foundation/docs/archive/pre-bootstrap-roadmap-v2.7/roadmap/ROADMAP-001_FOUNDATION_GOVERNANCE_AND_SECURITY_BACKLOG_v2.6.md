# ROADMAP-001 — Foundation Governance and Security Backlog

**Version:** 2.6  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** Project Owner approval recorded on 2026-07-25  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; ADR-I005  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This Roadmap records governed Foundation documents that are required or explicitly deferred. Listing an item does not approve its future content and does not authorize implementation.

## 2. Required Before Cryptographic Implementation

| ID | Document | Class | Required outcome | Status |
|---|---|---|---|---|
| CRY-001 | Cryptographic Domain and Profile Catalog | Catalog | Immutable Domain IDs, Purpose IDs, profile values, independent-root rules, lifecycle states, algorithms, and permitted operations | Approved as v1.0; FALCON-CRYPTO-1 remains APPROVED, not ACTIVE |
| FCE-001 | Falcon Canonical Encoding Specification | Specification | Sole canonical, versioned, cross-platform encoding for cryptographic Domain Context, operational identifiers, canonical timestamps, Runtime Epoch IDs, and protected Time Observations | Approved as v1.0; registered in SPEC-000 |
| DESIGN-SEC-001 | Foundation Cryptographic Provider and Secret Custody Design | Design | Exact Windows and Linux provider realization, custody, boundaries, recovery, and verification | Approved as v1.0; all custody profiles remain non-active |
| IDN-001 | Foundation Identifier Catalog | Catalog | Identifier classes, schemes, profiles, scopes, privacy rules, continuity, collision, and lifecycle | Approved as v1.0; internal UUIDv7 profile remains APPROVED, not ACTIVE |
| TIM-001 | Foundation Time and Clock-Quality Catalog | Catalog | Time semantics, Clock Quality, Clock Capabilities, uncertainty, Runtime Epoch, Deployment Profile thresholds, and failure rules | Approved as v1.0; Foundation time profiles remain APPROVED, not ACTIVE |

## 3. Required Before Build and Verification Implementation

| ID | Document | Class | Required outcome | Status |
|---|---|---|---|---|
| BLD-001 | Foundation Toolchain and Build Baseline Catalog | Catalog | Exact SDK, compiler, build, analysis, test, security, SBOM, provenance, database, and runner versions and sources | Approved as v1.0 under GOV-019; FALCON-BUILD-FOUNDATION-1 remains APPROVED, not ACTIVE |
| PIPE-001 | Foundation Pipeline Specification | Specification | Build Intents, Gate Profiles, stages, evidence obligations, evaluation, completeness, promotion, failure, and portability | Approved as v1.0 and registered under GOV-020; all Gate Profiles remain PROPOSED |
| TRC-001 | Foundation Requirement-to-Verification Traceability Matrix | Governed Matrix | Complete forward and reverse mapping between requirements, Contracts, invariants, risks, gates, and verification | Approved as v1.1 under GOV-021 and GOV-022; covers 776 requirements; machine-readable atomic expansion remains non-active |
| ENV-001 | Foundation Build and Verification Environment Profile | Profile | Isolated Windows and Linux environment identity, capabilities, trust, tools, network, data, time, and non-financial boundary | Approved as v1.0 and registered under GOV-022; all Environment Profiles remain PROPOSED |
| SEC-002 | Foundation Trust Object Model | Specification | Common identity, provenance, integrity, lineage, validity, Claims, Acceptance, Reliance, lifecycle, immutability, supersession, and challenge model | Approved as v1.0; registered in SPEC-000 |
| GOV-AUT-001 | Authority Jurisdiction and Delegation Model | Governance | Jurisdiction, authority, delegation, cross-jurisdiction decisions, challenge resolution, limits, and default-deny evidence | Approved as v1.0 |
| AUT-001 v1.1 amendment | Jurisdiction Verification Amendment | Specification amendment | Require Authority Engine to verify declared jurisdiction and valid delegation before authorization | AMD-002 v1.0 Approved; AUT-001 v1.1 activated |

## 4. Governance Roadmap

| ID | Document | Class | Required outcome | Timing |
|---|---|---|---|---|
| GOV-SEC-001 | Falcon Security Authority Charter | Governance | Delegation, appointment, powers, limits, emergency suspension, profile lifecycle authority, relationship to Guardian and Authority Engine, evidence, review, and succession | Approved as v1.0; no permanent holder appointed |
| GOV-001 v1.2 amendment package | Document-Class Responsibility Clarification | Governance amendment | Incorporate the approved document-class responsibility rule without silently modifying GOV-001 v1.1 | Before the rule is represented as part of the general document-authority baseline |
| ADR-I008 | Foundation Bootstrap and Activation Sequencing | ADR | Remove circular activation dependencies through bounded preparation, candidate, verification, activation, implementation, and operational authority stages | Accepted as v1.0 under GOV-023 |
| AMD-003 | Bootstrap and Activation Sequencing Amendment Package | Amendment package | Apply ADR-I008 consistently to IMP-001, BLD-001, IDN-001, TIM-001, CRY-001, DESIGN-SEC-001, ENV-001, PIPE-001, TRC-001, ROADMAP-001, readiness, and affected Contracts | Authorized for preparation; not yet authored |

## 5. Approved Classification Rule Pending GOV-001 Amendment

The Project Owner approved the following classification rule on 2026-07-25:

> **ADRs record consequential solution decisions. Specifications define required behavior, properties, constraints, and acceptance outcomes. Catalogs define governed values. Designs define implementation structure.**

This approval authorizes preparation of the GOV-001 amendment package. GOV-001 v1.1 remains the controlling general document-authority text until the amendment is separately activated and versioned.

## 6. Control Rules

1. A Roadmap entry creates no technical or operational authority.
2. Every listed artifact must follow GOV-001 and STD-001.
3. FCE-001 must be registered in SPEC-000 before Approval.
4. CRY-001 must have one canonical registry location and accountable owner.
5. GOV-SEC-001 cannot grant authority beyond the Constitution or permit Security Authority to self-expand its mandate.
6. Completion status requires the artifact's own approval evidence.
7. FCE-001 is the sole canonical encoding authority; IDN-001 and TIM-001 define governed values and SHALL NOT redefine encoding.
8. SEC-002 v1.0 is binding within its Approved scope; dependent documents remain separately required.
9. GOV-AUT-001 and AUT-001 v1.1 SHALL NOT create jurisdiction or authority beyond the Constitution.
10. BLD-001, PIPE-001, TRC-001, and ENV-001 are mandatory before build or verification implementation.

## 7. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | رائد عموره — approval of ADR-I005 through ADR-I008; Roadmap preparation package; SEC-002 v1.0; GOV-AUT-001 v1.0; AMD-002 v1.0; AUT-001 v1.1 activation; GOV-SEC-001 v1.0; FCE-001 v1.0; CRY-001 v1.0; IDN-001 v1.0; TIM-001 v1.0; DESIGN-SEC-001 v1.0; BLD-001 v1.0; PIPE-001 v1.0; TRC-001 v1.0; ENV-001 v1.0; TRC-001 v1.1 trace extension; and ADR-I008 v1.0 | 2026-07-25 |
