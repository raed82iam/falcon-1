# AMD-003 — Contract and VPL Impact Review

**Identifier:** AMD-003-IR-001  
**Version:** 1.0  
**Status:** Approved  
**Date:** 2026-07-25  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** ADR-I008; AMD-003  
**Approval Record:** GOV-025  
**Review Scope:** CON-001; CON-002; CON-008; CON-009; CON-010; VPL-000; VPL-001; VPL-007; VPL-008  
**Implementation Authority:** Not Granted

## 1. Purpose

This review determines whether ADR-I008 and AMD-003 require semantic changes to existing Contracts and verification plans.

The review applies three outcomes:

| Outcome | Meaning |
|---|---|
| `NO_CHANGE` | Existing scope remains correct; bootstrap concern is governed elsewhere |
| `AMEND` | Existing governed meaning must be extended through a new version |
| `NEW_DEPENDENCY` | Existing document remains correct but a new Contract, policy, or plan is required |

No outcome activates a change.

## 2. Governing Boundary

ADR-I008 distinguishes:

- external bootstrap evidence;
- Falcon operational identity and time;
- candidate execution;
- Profile Activation;
- FRS-001 implementation; and
- operational use.

Existing Contracts SHALL NOT be widened merely to make external bootstrap mechanisms appear native to Falcon.

## 3. Contract Decisions

### 3.1 CON-001 — Core Identity

**Decision:** `NO_CHANGE` and `NEW_DEPENDENCY`

CON-001 governs Falcon Core operational identity.

`BOOTSTRAP_EXTERNAL_ID` is deliberately not a Falcon Core identity and SHALL remain outside CON-001.

Required new dependency:

- Bootstrap Evidence and Provenance Contract or equivalent Trust Object Contract defining how an external bootstrap identifier is preserved and referenced after Falcon identifiers become available.

CON-001 SHALL later be checked to ensure:

- no external bootstrap identifier can satisfy Core identity;
- cross-linking does not imply continuity;
- operational identity still requires the Falcon Identifier Provider; and
- unknown identity remains restrictive.

No semantic change to CON-001 is required.

### 3.2 CON-002 — Authority Decision

**Decision:** `NO_CHANGE` and `NEW_DEPENDENCY`

CON-002 remains the observable Falcon decision boundary.

ADR-I008 requires new authority classes and Authority Instruments, but does not change the rule that authority must be explicit, attributable, scoped, and reconstructable.

Required new dependencies:

- Authority Instrument Contract;
- Delegation and Revocation Contract; and
- Authority Class and Decision Class Catalog.

No semantic change to CON-002 is required.

### 3.3 CON-008 — Evidence and Logging

**Decision:** `AMEND`

CON-008 must distinguish:

- Falcon-native evidence;
- imported external bootstrap evidence;
- evidence produced by a candidate subject;
- evaluation evidence;
- Activation evidence; and
- operational evidence.

Proposed new version:

```text
CON-008 v1.1
```

Required additions:

- Evidence Origin classification;
- `BOOTSTRAP_EXTERNAL` origin;
- candidate-subject identity;
- external source and environment identity;
- original bytes and digest preservation;
- import and cross-link event;
- prohibition on reclassifying external evidence as Falcon-native;
- prohibition on upgrading bootstrap time to Falcon `VERIFIED`;
- provenance of reconciliation after Provider Activation;
- independent challenge;
- supersession without rewriting; and
- failure when origin cannot be established.

CON-008 v1.1 SHALL remain backward compatible with existing FRS-001 evidence fields where the origin is Falcon-native.

### 3.4 CON-009 — Security Context

**Decision:** `NO_CHANGE` and `NEW_DEPENDENCY`

CON-009 governs Falcon Security Context.

External bootstrap identity and time do not constitute a Falcon Security Context.

Candidate execution SHALL therefore use:

- an external environment authorization context;
- an Authority Instrument;
- synthetic-only secret classes; and
- a bounded candidate scope.

Required new dependencies:

- Authority Instrument Contract;
- Bootstrap Execution Context Contract; and
- Provider Candidate Contract set.

CON-009 SHALL later verify that no bootstrap context can be accepted as operational Security Context.

No semantic change to CON-009 is required.

### 3.5 CON-010 — Foundation Baseline Manifest

**Decision:** `AMEND`

CON-010 currently identifies the Approved Foundation Baseline.

ADR-I008 requires the manifest to preserve:

- bootstrap source identity;
- Activation Manifest identity;
- Build Scope;
- Authority Instrument identity;
- Environment Class;
- candidate or active lifecycle;
- Provider Profile identities;
- external evidence set identity;
- activation decision identity; and
- no-operational-authority classification.

Proposed new version:

```text
CON-010 v1.1
```

The amendment SHALL prevent:

- candidate manifests from being represented as active baselines;
- bootstrap tools from being represented as Falcon runtime dependencies;
- missing Authority Instruments from becoming implicit permission;
- environment Activation from implying Foundation Implementation Authority; and
- Foundation Implementation Authority from implying operational or financial authority.

## 4. Verification Plan Decisions

### 4.1 VPL-000 — Foundation Verification Master Plan

**Decision:** `NO_CHANGE` and `NEW_DEPENDENCY`

VPL-000 governs the eight FRS-001 demonstration scenarios.

ADR-I008 preparation and enabling-provider verification occur before the FRS-001 VPL sequence. Expanding VPL-000 to own those stages would blur activation verification with release demonstration.

Required new dependency:

- VPL-BST-000 Foundation Bootstrap Verification Master Plan.

VPL-000 SHALL retain its existing FRS-001 scope and execution order.

### 4.2 VPL-001 — Trusted Bootstrap

**Decision:** `NO_CHANGE`

VPL-001 verifies Falcon startup from an already Approved and applicable baseline.

It does not verify how enabling Providers or environments originally earned Activation.

VPL-001 SHALL consume only:

- active Provider Profiles;
- active Environment Profile;
- active Build Baseline;
- active Pipeline scope;
- approved baseline manifest; and
- accepted activation evidence references.

No bootstrap candidate may satisfy VPL-001.

### 4.3 VPL-007 — Controlled Recovery

**Decision:** `NO_CHANGE`

ADR-I008 does not change:

- repair cannot approve itself;
- restoration requires independent evidence;
- Guardian restriction persists until lawful release; and
- failure does not broaden authority.

Provider-candidate recovery testing belongs to VPL-BST-000 or its child plans.

No semantic change to VPL-007 is required.

### 4.4 VPL-008 — Evidence Reconstruction

**Decision:** `NO_CHANGE` and `NEW_DEPENDENCY`

VPL-008 reconstructs VPL-001 through VPL-007 and remains correct.

Bootstrap and Activation history SHALL be independently reconstructable, but it precedes the FRS scenario chain.

Required new dependency:

- VPL-BST-008 Bootstrap and Activation Evidence Reconstruction, or equivalent final plan within VPL-BST-000.

VPL-008 SHALL preserve references to the accepted baseline and Activation evidence without assuming responsibility for re-verifying the complete bootstrap history.

## 5. New Contract Set Required

The following Contract candidates are required:

| Proposed ID | Contract |
|---|---|
| CON-012 | Authority Instrument |
| CON-013 | Delegation and Revocation |
| CON-014 | Identifier Provider |
| CON-015 | Time Provider |
| CON-016 | Cryptographic Provider |
| CON-017 | Secret Provider |
| CON-018 | Certificate and Identity Provider |
| CON-019 | Randomness Provider |
| CON-020 | Bootstrap Execution Context |
| CON-021 | Bootstrap Evidence and Provenance |

Final identifiers require CON-000 admission review.

## 6. New Verification Set Required

Proposed bootstrap verification structure:

```text
VPL-BST-000 — Foundation Bootstrap Verification Master Plan
├── VPL-BST-001 — Preparation Environment Admission
├── VPL-BST-002 — Tool and Dependency Bundle Integrity
├── VPL-BST-003 — Identifier Provider Candidate
├── VPL-BST-004 — Time Provider Candidate
├── VPL-BST-005 — Cryptographic and Secret Provider Candidates
├── VPL-BST-006 — Environment Activation
├── VPL-BST-007 — Pipeline and Trace Activation
└── VPL-BST-008 — Bootstrap and Activation Evidence Reconstruction
```

Final identifiers and plan classes require verification-registry admission.

## 7. Contract Version Decisions

| Document | Current | Candidate | Decision |
|---|---:|---:|---|
| CON-001 | 1.0 | 1.0 | No semantic change |
| CON-002 | 1.0 | 1.0 | No semantic change |
| CON-008 | 1.0 | 1.1 | Amendment required |
| CON-009 | 1.0 | 1.0 | No semantic change |
| CON-010 | 1.0 | 1.1 | Amendment required |

## 8. VPL Version Decisions

| Document | Current | Candidate | Decision |
|---|---:|---:|---|
| VPL-000 | 1.0 | 1.0 | No semantic change |
| VPL-001 | 1.0 | 1.0 | No semantic change |
| VPL-007 | 1.0 | 1.0 | No semantic change |
| VPL-008 | 1.0 | 1.0 | No semantic change |

New bootstrap verification plans are required instead of widening the FRS-001 plans.

## 9. Traceability Consequence

TRC-001 v1.2 SHALL record:

- the two amended Contract candidates;
- the ten new Contract candidates;
- the bootstrap verification master plan;
- its eight child plans;
- every ADR-I008 authority stage;
- external bootstrap identity and time;
- candidate execution;
- Activation;
- reconstruction; and
- preserved separation from FRS-001 VPLs.

Atomic requirement counts SHALL be recalculated only after requirement-bearing documents are Approved.

## 10. Review Result

The impact review finds:

- no required semantic change to CON-001, CON-002, CON-009, VPL-000, VPL-001, VPL-007, or VPL-008;
- required amendments to CON-008 and CON-010;
- ten required new Contracts;
- one required bootstrap verification master plan with eight bounded plans; and
- no permission to activate or implement any reviewed subject.

## 11. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-025 | 2026-07-25 |

Approval adopts the impact decisions and authorizes drafting the identified Contract and verification candidates.

It does not:

- amend CON-008 or CON-010;
- approve a new Contract;
- approve a new verification plan;
- activate AMD-003 target versions;
- issue authority;
- authorize implementation; or
- authorize financial activity.
