# VPL-BST-001 — Preparation Environment Admission Verification Plan

**Identifier:** VPL-BST-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-031  
**Owner:** Falcon Verification Authority  
**Governing Sources:** ADR-I008; ENV-001; SEC-001; SEC-002; CON-012; CON-020; CON-021  
**Master Plan:** VPL-BST-000  
**Implementation Authority:** Not Granted

## 1. Objective

Prove that a `PREPARATION` environment can be identified, isolated, authorized, evidenced, stopped, and cleaned without relying on Falcon operational identity, `VERIFIED` time, active custody, active Pipeline, or implementation authority.

## 2. Required Setup

- exact environment candidate and image identity;
- valid Foundation Preparation Authority Instrument;
- immutable CON-020 Context;
- external identity and time sources;
- sealed tool and input candidates;
- synthetic data and material only;
- controlled network and storage policies;
- protected external evidence capture; and
- independent abort and review.

## 3. Procedure

1. Verify the Authority Instrument, context, subject, environment, and expiry.
2. establish the environment from the exact candidate.
3. confirm `BOOTSTRAP_EXTERNAL_ID` and external time classification.
4. verify network, storage, process, secret, and data isolation.
5. execute only declared preparation probes.
6. attempt prohibited actions and connections.
7. inject authority expiry, evidence failure, and isolation failure.
8. verify stop, containment, cleanup, and evidence export.
9. independently reconstruct the admission decision.

## 4. Mandatory Negative Variants

- missing or expired authority;
- changed image or tool;
- unlisted action;
- production secret or data injection;
- financial endpoint attempt;
- unrestricted network or storage;
- candidate represented as active;
- bootstrap time represented as `VERIFIED`;
- evidence export failure; and
- incomplete cleanup.

## 5. Pass Rule

`PASS` requires exact environment admission, successful isolation and cleanup, failure-closed behavior for every negative variant, complete independent evidence, and no Falcon operational or financial authority.

## 6. Requirements

- **VPL-BST-001-REQ-001:** Admission SHALL require exact authority, context, environment, and subject identity.
- **VPL-BST-001-REQ-002:** Identity and time SHALL remain externally classified.
- **VPL-BST-001-REQ-003:** Production and financial paths SHALL be absent and actively tested.
- **VPL-BST-001-REQ-004:** Only enumerated preparation actions SHALL execute.
- **VPL-BST-001-REQ-005:** Material authority, isolation, or evidence failure SHALL stop execution.
- **VPL-BST-001-REQ-006:** Cleanup and evidence export SHALL be verified independently.
- **VPL-BST-001-REQ-007:** The environment SHALL NOT claim active Falcon trust or implementation fitness.
- **VPL-BST-001-REQ-008:** Repetition SHALL begin from a declared clean or reconciled state.

## 7. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-031 | 2026-07-25 |
