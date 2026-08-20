# STG-0B-VER-001 — Verification and Independent Evaluation Plan

**Identifier:** STG-0B-VER-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** VPL-BST-003; VPL-BST-004; VPL-BST-005; STG-0B-EVD-001  
**Approval Record:** GOV-051  
**Verification Authority:** Granted for Stage 0B candidate verification only

## 1. Purpose

This candidate defines how authorized Stage 0B candidates would be verified without allowing them to validate or activate themselves.

## 2. Verification Mapping

| Candidate Scope | Required Plan |
|---|---|
| Identifier Provider | VPL-BST-003 |
| Time Provider | VPL-BST-004 |
| Cryptographic, Secret, Certificate/Identity, and Randomness Providers or Adapters | VPL-BST-005 |
| FCE, Trust Object, trace, harness, and fixtures | Applicable Contract, encoding, integrity, provenance, repeatability, and boundary checks |

## 3. Required Verification Dimensions

- Contract conformance;
- deterministic behavior where required;
- canonical encoding;
- purpose and domain enforcement;
- invalid-input rejection;
- uncertainty handling;
- collision and continuity handling;
- clock-quality degradation;
- secret non-disclosure;
- cryptographic failure containment;
- provenance and integrity;
- dependency isolation;
- repeatability;
- fault injection;
- cleanup;
- and non-Activation.

## 4. Session Identity

Every verification execution shall have:

- Verification Session ID;
- Root Evidence Set ID;
- Candidate ID;
- Build Intent;
- Environment ID;
- Runtime Epoch ID;
- Evaluation Context ID;
- governing requirement snapshot;
- and external bootstrap time observation.

## 5. Evaluation Model

Every evaluation shall declare:

- Evaluation Mode: `AUTOMATED`, `HUMAN`, or `HYBRID`;
- Evaluation Nature: `DETERMINISTIC`, `PROBABILISTIC`, or `JUDGMENT_BASED`;
- Evaluation Authority;
- governing scope;
- rules and inputs;
- and reproducibility status.

AI evaluation shall declare its nature. Unless deterministic behavior is demonstrated under a governed execution profile, it shall be treated as `JUDGMENT_BASED`.

## 6. Independent Evaluation

The candidate under evaluation shall not:

- select its own obligations;
- alter expected outcomes;
- suppress failures;
- declare its own evidence complete;
- resolve a material challenge against itself;
- or approve progression.

Independent evaluation may confirm only scoped verification outcomes. It cannot activate a candidate.

## 7. Failure Rule

A failed or uncertain mandatory result shall not be converted to `PASS`.

Retry requires the original operation identity, preserved prior evidence, an approved retry basis, and protection against duplicate effects.

Unknown outcome shall remain `UNCERTAIN` until reconciled.

## 8. Final Finding

The Stage 0B verification finding shall be one of:

```text
CONFORMING_CANDIDATE
NONCONFORMING_CANDIDATE
INCOMPLETE_EVIDENCE
INVALID_EVIDENCE
UNCERTAIN
STOPPED
```

`CONFORMING_CANDIDATE` is not Acceptance or Activation.
