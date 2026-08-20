# STG-0B-EVD-001 — Evidence Requirement Set

**Identifier:** STG-0B-EVD-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** SEC-002; ADR-I007; CON-021; PIPE-001  
**Approval Record:** GOV-051  
**Evidence Authority:** Granted for Stage 0B only

## 1. Purpose

This candidate defines the mandatory evidence obligations for a future Stage 0B case.

The approved version shall be snapshotted before execution so later policy changes cannot rewrite historical obligations.

## 2. Evidence Requirements

| Evidence ID | Obligation | Classification |
|---|---|---|
| 0B-EVD-001 | Approved authority and jurisdiction | Mandatory |
| 0B-EVD-002 | Immutable Bootstrap Execution Context | Mandatory |
| 0B-EVD-003 | Repository baseline before execution | Mandatory |
| 0B-EVD-004 | Environment and runtime epoch | Mandatory |
| 0B-EVD-005 | Tool identity, version, source, and digest | Mandatory |
| 0B-EVD-006 | Candidate scope and dependency manifest | Mandatory |
| 0B-EVD-007 | Synthetic-material manifest | Mandatory |
| 0B-EVD-008 | Original build observations and outputs | Mandatory |
| 0B-EVD-009 | Applicable Contract results | Mandatory |
| 0B-EVD-010 | Applicable VPL-BST-003 to 005 results | Conditional |
| 0B-EVD-011 | Fault, negative, and boundary results | Mandatory |
| 0B-EVD-012 | Security and secret-custody findings | Mandatory |
| 0B-EVD-013 | Financial-isolation finding | Mandatory |
| 0B-EVD-014 | Dependency and provenance finding | Mandatory |
| 0B-EVD-015 | Derived Evaluations and Evaluation Context | Derived |
| 0B-EVD-016 | Completeness evaluation | Derived |
| 0B-EVD-017 | Challenges and resolutions | Conditional |
| 0B-EVD-018 | Repository and file-change record after execution | Mandatory |
| 0B-EVD-019 | Cleanup and material-disposition result | Mandatory |
| 0B-EVD-020 | Stage 0B completion or stop assessment | Mandatory |

## 3. Evidence Set Structure

```text
Verification Obligations
→ Observed Evidence
→ Derived Evaluations
→ Evaluation Context
→ Stage 0B Decision Context
```

The Root Verification Evidence Set shall preserve both evidence and the obligations against which it was evaluated.

## 4. Completeness States

- `COMPLETE`: all applicable mandatory obligations satisfied.
- `PARTIAL`: only policy-permitted omissions exist for the declared scope.
- `INCOMPLETE`: required evidence is missing.
- `INVALID`: integrity, provenance, or trust failure prevents reliance.

Only `COMPLETE` and integrity-valid evidence may support a Stage 0B completion claim.

## 5. Evaluation Context

Each Derived Evaluation shall reference an immutable Evaluation Context containing:

- policy and Contract versions;
- configuration;
- environment;
- authority scope;
- rule versions;
- tool versions;
- feature states;
- trust state;
- provenance;
- digest;
- and validity scope.

## 6. Independence

No component that produces, transforms, aggregates, or signs evidence may be the sole authority declaring the case complete.

No challenged Claim or decision may be conclusively resolved solely by its producer or the authority under material challenge.

## 7. Integrity

Evidence shall be attributable, scoped, content-identified, immutable after recording, and corrected through superseding records.

An evaluation without preserved evidence and governing rules is unverifiable.
