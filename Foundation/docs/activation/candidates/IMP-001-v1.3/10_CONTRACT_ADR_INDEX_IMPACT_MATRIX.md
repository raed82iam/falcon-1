# IMP-001 v1.3 — Contract, ADR, Registry and Index Impact Matrix

**Status:** CANDIDATE SUPPORTING RECORD / IMPACT REVIEW COMPLETE  
**Date:** 2026-08-09  
**Implementation Authority:** NOT GRANTED

## 1. Purpose

Determine whether the Owner-approved Master Stage Sequence requires immediate semantic changes to current Contracts, ADRs, registries or indexes before IMP-001 v1.3 may be activated.

## 2. Contract Registry — CON-000 v1.7

Current effective registry contains CON-001 through CON-021 and CON-023.

Decision:

- No existing Contract meaning is changed merely by Stage remapping.
- No current Contract is silently widened to cover Stage 11 through Stage 17.
- CON-023 remains the generic Falcon Application Contract and Manifest and is the principal existing Application boundary input for Stage 14/15.
- CON-006 remains the Health/Fitness boundary for Stage 7.
- CON-011 remains the current protective-restriction boundary subject to Stage 8 Guardian documentary reconciliation.
- CON-012/013 remain authority/delegation inputs for Stage 13.
- CON-014 through CON-019 remain provider/security boundary inputs and do not themselves authorize external Internet/provider/broker egress.

Future Stage designs MAY require new or successor Contracts, especially for:

- Stage 11 QoS/deadline/observability boundary;
- Stage 12 external egress/service-role/credential-reference boundary;
- Stage 13 Owner/FSA governance command/review package boundary;
- Stage 14 canonical artifact publication/consumption boundary;
- Stage 15 runtime hosting/activation/isolation boundary;
- Stage 16 Environment Profile/runtime realization boundary.

Exact Contract IDs and semantics SHALL be created only after the governing Stage Specifications are defined. This Master Plan SHALL NOT invent them prematurely.

**Immediate CON-000 semantic update required for plan activation:** `NO`.

## 3. ADR Index — ADR-000 v2.7

Current accepted decisions include ADR-F001..F008, ADR-I001..I008, ADR-I012 and ADR-I015, with ADR-I009 preserved as superseded history.

Decision:

- Stage sequence correction is a governed Plan/roadmap decision and does not itself select a new implementation mechanism requiring an ADR.
- ADR-I012 already establishes the generic Plug-and-Play Application integration boundary.
- ADR-I015 already establishes Foundation/Application/Awareness alignment and Foundation independence.
- Existing environment/provider-neutral rules in PIPE/ENV/governance and the Owner-approved planning invariant do not select one new runtime/cloud technology.

Future ADR requirements remain prospective:

- Stage 15 must create an ADR if Falcon Cells are selected as the runtime isolation realization, because PLG-001 explicitly requires one.
- Stage 16 shall create ADRs for consequential environment realization choices where required, without making any provider architectural authority.
- Stage 11/12/14/15 may require ADRs for consequential technology/topology/packaging choices only after Specifications define the requirements.

**Immediate new ADR required to activate IMP-001 v1.3:** `NO`.

**Immediate ADR-000 index update required:** `NO`, unless a new ADR is approved before coordinated activation.

## 4. Specification Registry and Tree

The Master Plan does not approve any of the 38 registry-only planned Specification subjects.

Decision:

- SPEC-000 v1.5 remains valid.
- TREE-001 remains valid as the structural coverage map.
- Planned subjects retain `NOT YET EFFECTIVE` until separately authored/reviewed/activated.
- Future Stage assignment may be recorded in planning/traceability without changing a planned row into an effective Specification.

**Immediate SPEC-000 semantic update required:** `NO`.

**Immediate TREE-001 semantic update required:** `NO`.

## 5. FRS-001 and VPL-000

FRS-001 meaning remains unchanged.

VPL-000 remains the verification master plan for FRS-001 only.

Stage 10 remains the corrected FRS closure point. Stage 11 through Stage 17 remain post-FRS Foundation work with separately governed future verification plans.

**Immediate FRS-001 semantic successor required:** `NO`.

**Immediate VPL-000 semantic successor required:** `NO`.

## 6. ROADMAP-001

Current ROADMAP-001 v2.9 contains stale historical backlog wording in its body while its metadata reflects later activation.

A versioned successor is required to establish the current forward roadmap without rewriting historical v2.9.

Candidate prepared:

- `06_ROADMAP-001_v3.0_PROPOSED.md`

**Immediate versioned successor required:** `YES`.

## 7. TRC-001

Current TRC-001 v1.3 preserves extensive v1.2 body text plus AMD-008 alignment. The corrected Stage map requires explicit prospective trace extension.

Candidate prepared:

- `07_TRC-001_v1.4_PROPOSED.md`

**Immediate versioned successor required:** `YES`.

## 8. AWR-001 Documentary Consistency

AWR-001 current-effective metadata conflicts with stale candidate-era footer wording.

This is a documentary consistency issue only. It must be corrected through a versioned administrative successor or separately governed amendment without changing requirement meaning.

**Immediate documentary remediation in coordinated activation package:** `YES`.

## 9. README and Current-State Surfaces

README currently truthfully records accepted state through Stage 6 WP-04 and current non-authorities.

It SHALL NOT be updated to imply IMP-001 v1.3 is controlling before coordinated activation.

After activation, README should be synchronized to identify the new controlling master sequence while preserving current implementation authorization state.

**Pre-activation README semantic update:** `NO`.

**Activation-time README synchronization:** `YES`.

## 10. FCR Headers

FCRs remain requests/disposition records, not implementation authority.

Existing Owner-approved planning destinations should remain synchronized under Issue #1 protocol. Any target changed by the final successor package must be updated at activation time with explicit review triggers.

**FCRs block Master Plan activation solely because they remain open:** `NO`.

## 11. Final Impact Disposition

`CURRENT_CONTRACT_SEMANTIC_CHANGE_REQUIRED_NOW = NO`

`NEW_ADR_REQUIRED_NOW = NO`

`SPEC_REGISTRY_SEMANTIC_CHANGE_REQUIRED_NOW = NO`

`TREE_SEMANTIC_CHANGE_REQUIRED_NOW = NO`

`FRS001_MEANING_CHANGE_REQUIRED = NO`

`VPL000_MEANING_CHANGE_REQUIRED = NO`

`ROADMAP_SUCCESSOR_REQUIRED = YES_PREPARED`

`TRC_SUCCESSOR_REQUIRED = YES_PREPARED`

`AWR001_DOCUMENTARY_REMEDIATION_REQUIRED = YES`

`README_ACTIVATION_TIME_SYNC_REQUIRED = YES`

`CONTRACT_ADR_INDEX_MASTER_PLAN_BLOCKER = CLOSED`
