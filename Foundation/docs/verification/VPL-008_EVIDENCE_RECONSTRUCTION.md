# VPL-008 — Evidence Reconstruction Verification Plan

**Identifier:** VPL-008  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-007  
**Scenario:** FRS-SCN-008  
**Owner:** Falcon Verification Authority  
**Governing Sources:** FRS-001; OPS-004; SYS-011; DEC-006; CON-008; ADR-F002; ADR-F005  
**Master Plan:** VPL-000  
**Supersedes:** None  
**Superseded By:** None

## 1. Verification Objective

Prove that an authorized reviewer who did not operate the scenarios can reconstruct the identities, inputs, authority, state changes, communications, restrictions, recovery, and outcomes of VPL-001 through VPL-007 without relying on undocumented knowledge.

## Scope and Non-Scope

This plan verifies reconstruction of the isolated FRS-001 demonstration. It does not claim regulatory financial record compliance, permanent retention sufficiency, production forensics, or reconstruction of systems outside the Foundation boundary.

Result vocabulary and global evidence rules are governed by VPL-000.

## 2. Required Setup

- sealed evidence packages from VPL-001 through VPL-007;
- an authorized reviewer not involved in scenario operation;
- approved schemas, Contracts, baselines, clocks, and integrity anchors;
- access controls that preserve confidentiality; and
- controlled altered, missing, reordered, duplicated, and corrected evidence variants.

## 3. Procedure

1. Give the reviewer only the governed baseline and sealed evidence packages.
2. Reconstruct each scenario’s initial state, actors, security contexts, inputs, authority decisions, FIL interactions, transitions, persistence outcomes, and final state.
3. Reconstruct correlation and causation across all scenarios.
4. Verify the distinction between attempted, accepted, authorized, executed, persisted, and successful outcomes.
5. Verify integrity checkpoints and identify every expected segment.
6. Introduce controlled mutation, deletion, insertion, reordering, and duplication in copies of the package.
7. Present an appended correction and confirm the original remains intact.
8. Verify that redaction protects secrets while preserving authorized accountability.
9. Compare the reviewer’s reconstruction with the independently sealed expected chronology.

## 4. Expected Results

- The reviewer reconstructs every material step and controlling authority.
- Integrity failures, gaps, mutations, insertions, reordering, and duplicates are detected.
- Corrections append and do not rewrite accepted history.
- Missing evidence produces `INCONCLUSIVE` or `FAIL`, never assumed success.
- Confidentiality controls do not prevent authorized reconstruction.
- Logs and events are not mistaken for authoritative state without ownership evidence.

## 5. Required Evidence

Reviewer identity and authorization, evidence inventory, integrity-verification results, reconstructed chronology, state lineage, authority lineage, correlation and causation graph, detected fault variants, correction lineage, redaction assessment, deviations, and final independent decision.

## 6. Pass Rule

`PASS` requires materially complete reconstruction of VPL-001 through VPL-007, detection of every controlled integrity fault, preservation of confidentiality, and no undocumented assumption required to determine a result.

Any undetected material mutation, inability to identify controlling authority, or conversion of missing evidence into success is an immediate `FAIL`.

## 7. Independent Verification

The reconstructing reviewer shall not have operated, repaired, or approved the scenarios being reconstructed. The sealed expected chronology shall be revealed only after the reviewer records the independent reconstruction.

## 8. Containment, Cleanup, and Repeatability

All mutation tests shall use copies of sealed evidence. Original packages and expected chronology shall remain protected. Repetition shall use a new reviewer or blinded execution identity and shall preserve every reconstruction result.

## 9. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-007 | 2026-07-24 |
