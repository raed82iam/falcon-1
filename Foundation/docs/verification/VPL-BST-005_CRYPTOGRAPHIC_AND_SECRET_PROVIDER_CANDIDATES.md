# VPL-BST-005 — Cryptographic and Secret Provider Candidates Verification Plan

**Identifier:** VPL-BST-005  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-031  
**Owner:** Falcon Verification Authority  
**Governing Sources:** CRY-001; SEC-001; FCE-001; DESIGN-SEC-001; CON-016; CON-017; CON-018; CON-019; ADR-I005; ADR-I008  
**Master Plan:** VPL-BST-000  
**Implementation Authority:** Not Granted

## 1. Objective

Prove that Cryptographic, Secret, Certificate and Identity, and Randomness Provider candidates enforce Falcon policy, custody, domain separation, purpose, lifecycle, validation, isolation, failure, and replaceability without exposing protected material or certifying themselves.

## 2. Required Setup

- exact Provider and Adapter candidates;
- candidate Authority Instruments and CON-020 contexts;
- test-only profiles, domains, roots, keys, secrets, certificates, identities, trust anchors, and revocation sources;
- isolated custody;
- positive and negative operation vectors;
- cross-domain, cross-environment, wrong-purpose, wrong-lifecycle, nonce, revocation, and source-failure fixtures;
- platform capability probes; and
- independent evidence control.

No production security material is permitted.

## 3. Procedure

1. Verify every candidate, Adapter, profile, root boundary, and test-material identity.
2. Exercise all approved cryptographic operations and known-answer vectors.
3. attempt wrong purpose, domain, environment, identity, lifecycle, authority, and Guardian state.
4. test nonce reuse, counter limits, rotation, revocation, and authentication failure.
5. verify Secret Reference use, enumeration denial, and prohibited-location absence.
6. test certificate chain, subject, usage, time, trust anchor, and revocation cases.
7. test randomness purposes, lengths, health failure, repeated output, and caller-supplied entropy.
8. inject custody and Provider failure and verify no fallback.
9. replace an Adapter or Provider candidate and compare Falcon-visible meaning.
10. attempt export, promotion of test material, and candidate self-Activation.
11. reconstruct every decision without protected material.

## 4. Immediate Failures

- raw secret or private-key exposure;
- test material entering operational custody;
- cross-domain or cross-environment key use;
- accepted wrong-purpose use;
- nonce reuse or unbounded use;
- plaintext, weak-source, platform-default, or silent Provider fallback;
- skipped revocation or implicit platform trust;
- secret appearance in logs, configuration, environment, commands, dumps, or evidence;
- candidate self-certification; or
- incomplete evidence for a security-critical result.

## 5. Pass Rule

`PASS` requires every valid operation, every required rejection, no protected-material leakage, independent-root and purpose enforcement, conservative failure, candidate isolation, replaceability, and complete non-secret evidence.

## 6. Requirements

- **VPL-BST-005-REQ-001:** Every approved operation and profile combination SHALL be exercised.
- **VPL-BST-005-REQ-002:** Wrong purpose, domain, environment, identity, lifecycle, authority, and Guardian state SHALL be rejected.
- **VPL-BST-005-REQ-003:** Independent roots and canonical Domain Context SHALL be verified.
- **VPL-BST-005-REQ-004:** Raw private keys, secrets, random output, and protected material SHALL not appear outside custody.
- **VPL-BST-005-REQ-005:** Nonce, counter, operation, rotation, and revocation limits SHALL be enforced.
- **VPL-BST-005-REQ-006:** Certificate identity and revocation SHALL not inherit platform trust implicitly.
- **VPL-BST-005-REQ-007:** Randomness failure SHALL cause no weak fallback.
- **VPL-BST-005-REQ-008:** Provider or custody failure SHALL cause no plaintext, file, environment, or silent substitute.
- **VPL-BST-005-REQ-009:** Test and candidate material SHALL remain non-operational and non-promotable.
- **VPL-BST-005-REQ-010:** Provider replacement SHALL preserve Falcon-visible semantics and evidence.
- **VPL-BST-005-REQ-011:** Candidate Providers SHALL not conclusively validate or activate themselves.
- **VPL-BST-005-REQ-012:** Evidence SHALL remain complete without disclosing protected material.

## 7. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-031 | 2026-07-25 |
