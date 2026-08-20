# VPL-BST-003 — Identifier Provider Candidate Verification Plan

**Identifier:** VPL-BST-003  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-031  
**Owner:** Falcon Verification Authority  
**Governing Sources:** IDN-001; FCE-001; CON-014; CON-019; ADR-I006; ADR-I008  
**Master Plan:** VPL-BST-000  
**Implementation Authority:** Not Granted

## 1. Objective

Prove that an Identifier Provider candidate correctly enforces class, profile, scope, canonical representation, continuity, collision, privacy, dependency, and candidate isolation without creating identity, trust, authority, time, or Activation.

## 2. Required Setup

- exact candidate and synthetic profile;
- candidate Authority Instrument and CON-020 context;
- controlled Time and Randomness candidate inputs;
- all approved Foundation identifier classes;
- collision and retry fixtures;
- privacy-boundary fixtures;
- cross-platform canonical comparison; and
- independent external control.

## 3. Procedure

1. verify candidate identity and authorized capabilities.
2. issue test values for every eligible class.
3. validate FCE canonical text and bytes.
4. test retry continuity and distinct attempts.
5. inject collisions, wrong class, nil, reserved, wrong-version, and caller-controlled values.
6. test privacy and external-exposure denial.
7. fail Time and Randomness dependencies.
8. attempt candidate escape and self-Activation.
9. independently reconstruct every result.

## 4. Pass Rule

`PASS` requires all valid cases, rejection of every invalid case, collision containment, dependency failure without fallback, candidate isolation, and no identity or authority claim.

## 5. Requirements

- **VPL-BST-003-REQ-001:** Every approved class and profile SHALL be exercised.
- **VPL-BST-003-REQ-002:** Canonical output SHALL match across supported environments.
- **VPL-BST-003-REQ-003:** Invalid, reserved, wrong-class, and caller-controlled generation SHALL be rejected.
- **VPL-BST-003-REQ-004:** Retry continuity and attempt identity SHALL remain distinct.
- **VPL-BST-003-REQ-005:** Collision SHALL cause containment and evidence.
- **VPL-BST-003-REQ-006:** Unapproved exposure SHALL be denied.
- **VPL-BST-003-REQ-007:** Dependency failure SHALL cause no direct-generation fallback.
- **VPL-BST-003-REQ-008:** Candidate output SHALL remain non-operational and incapable of self-Activation.

## 6. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-031 | 2026-07-25 |
