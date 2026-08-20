# VPL-BST-004 — Time Provider Candidate Verification Plan

**Identifier:** VPL-BST-004  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-031  
**Owner:** Falcon Verification Authority  
**Governing Sources:** TIM-001; FCE-001; CON-015; ADR-I006; ADR-I008  
**Master Plan:** VPL-BST-000  
**Implementation Authority:** Not Granted

## 1. Objective

Prove that a Time Provider candidate produces canonical observations with correct source, Runtime Epoch, Clock Quality, uncertainty, capabilities, and conservative failure behavior without claiming absolute truth or self-Activation.

## 2. Required Setup

- exact candidate and synthetic Time Provider Profile;
- candidate Authority Instrument and CON-020 context;
- controlled sources with known offset, drift, discontinuity, uncertainty, and failure;
- Runtime Epoch transition fixtures;
- canonical cross-platform comparison;
- expired-verification and source-conflict cases; and
- independent reference observations.

## 3. Procedure

1. verify candidate identity, profile, sources, and capabilities.
2. compare canonical UTC representation across platforms.
3. verify uncertainty intervals and boundary decisions.
4. test monotonic comparison within one Runtime Epoch.
5. test restart, migration, failover, source replacement, and epoch break.
6. inject drift, rollback, delay, source conflict, stale verification, and capability loss.
7. attempt direct-clock fallback and self-Activation.
8. independently reconstruct quality changes and results.

## 4. Pass Rule

`PASS` requires correct canonical representation, conservative uncertainty, valid epoch isolation, every required quality downgrade, no direct-clock fallback, and non-operational candidate output.

## 5. Requirements

- **VPL-BST-004-REQ-001:** Canonical UTC SHALL match FCE-001 across supported environments.
- **VPL-BST-004-REQ-002:** Every observation SHALL declare source, epoch, quality, uncertainty, capabilities, and verification.
- **VPL-BST-004-REQ-003:** Temporal boundaries SHALL use the uncertainty interval.
- **VPL-BST-004-REQ-004:** Monotonic comparison SHALL remain within proven epoch continuity.
- **VPL-BST-004-REQ-005:** Restart, migration, failover, and source change SHALL not assume continuity.
- **VPL-BST-004-REQ-006:** Drift, conflict, stale verification, and capability loss SHALL downgrade or reject.
- **VPL-BST-004-REQ-007:** Failure SHALL cause no direct-clock fallback.
- **VPL-BST-004-REQ-008:** Candidate observations SHALL remain non-operational and incapable of self-Activation.

## 6. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-031 | 2026-07-25 |
