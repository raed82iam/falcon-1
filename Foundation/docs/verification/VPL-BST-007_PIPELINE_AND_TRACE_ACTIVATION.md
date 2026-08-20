# VPL-BST-007 — Pipeline and Trace Activation Verification Plan

**Identifier:** VPL-BST-007  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-031  
**Owner:** Falcon Verification Authority  
**Governing Sources:** PIPE-001; TRC-001; BLD-001; SEC-002; ADR-I008; CON-008; CON-010; CON-012; CON-021  
**Master Plan:** VPL-BST-000  
**Implementation Authority:** Not Granted

## 1. Objective

Prove that the machine-readable trace expansion, Pipeline Definition, Gate Profile, Evidence Requirement Set generation, evidence collection, evaluation, and promotion boundary are exact, complete, reproducible, independently governed, and incapable of promoting themselves.

## 2. Required Setup

- exact active prerequisite environment and Provider profiles;
- candidate machine-readable TRC expansion;
- candidate Pipeline Definition and Gate Profile;
- exact tool and dependency baseline;
- declared Build Intents;
- controlled requirement omissions, duplicates, stale versions, and wrong mappings;
- controlled evidence mutation, omission, invalid signature, and context conflict;
- independent evaluators and authorities; and
- non-promotable test artifacts.

## 3. Procedure

1. expand every Approved atomic requirement exactly once.
2. verify forward and reverse mappings, source versions, and locations.
3. generate an immutable Evidence Requirement Set for each Build Intent.
4. execute the candidate Pipeline against positive and negative fixtures.
5. verify evidence origin, identity, integrity, lineage, context, and completeness states.
6. verify Derived Evaluations are reproducible or explicitly judgment-based.
7. test producer, aggregator, signer, evaluator, completeness, and promotion separation.
8. inject omitted and duplicate requirements, stale policies, evidence mutation, missing sessions, and invalid context.
9. verify that promotion references exactly one Root Verification Evidence Set.
10. attempt Gate weakening, direct session promotion, self-promotion, and unsupported completeness.
11. independently reconstruct the complete case.

## 4. Pass Rule

`PASS` requires exact trace expansion, correct immutable obligations, all negative cases blocked, valid authority separation, no Gate weakening or self-promotion, and reproducible reconstruction.

## 5. Requirements

- **VPL-BST-007-REQ-001:** Every atomic requirement SHALL expand exactly once with exact source binding.
- **VPL-BST-007-REQ-002:** Forward and reverse traceability SHALL be complete and independently verified.
- **VPL-BST-007-REQ-003:** Every execution SHALL freeze one Evidence Requirement Set before evidence production.
- **VPL-BST-007-REQ-004:** Completeness SHALL be evaluated against the frozen obligation snapshot.
- **VPL-BST-007-REQ-005:** Promotion SHALL reference exactly one Root Verification Evidence Set.
- **VPL-BST-007-REQ-006:** Individual sessions SHALL not serve directly as promotion evidence.
- **VPL-BST-007-REQ-007:** Evidence producers, transformers, aggregators, and signers SHALL not solely declare completeness or promotion readiness.
- **VPL-BST-007-REQ-008:** Missing, duplicate, stale, mutated, conflicted, or invalid material SHALL prevent required promotion.
- **VPL-BST-007-REQ-009:** Gate weakening and self-promotion SHALL be rejected.
- **VPL-BST-007-REQ-010:** Derived Evaluations SHALL preserve rules, inputs, context, nature, mode, and authority.
- **VPL-BST-007-REQ-011:** Pipeline and trace Activation SHALL require separate competent decisions.
- **VPL-BST-007-REQ-012:** Passing this plan SHALL not promote an FRS-001 artifact.

## 6. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-031 | 2026-07-25 |
