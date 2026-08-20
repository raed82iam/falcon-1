# Stage 0B Root Verification Evidence Set

**Root Evidence Set ID:** STG-0B-ROOT-EVD-001  
**Version:** 1.0  
**Recorded Date:** 2026-07-26  
**Authority:** GOV-051; GOV-052  
**Build Intent:** `STAGE_0B_CANDIDATE_VERIFICATION`  
**Classification:** Candidate Evidence  
**Completeness:** Complete for the declared Stage 0B scope  
**Acceptance:** Pending Project Owner decision  
**Activation Authority:** Not Granted

## Evidence Index

| Evidence | Purpose | Status |
|---|---|---|
| STG-0B-STOP-001 | Initial .NET boundary event | Preserved and Remediated |
| STG-0B-REM-001 | Isolated .NET remediation | Satisfied |
| STG-0B-OBS-001 | Machine-readable verification observations | 37/37 Passed |
| STG-0B-BLD-EVD-001 | Build, tool, dependency, and artifact evidence | Satisfied |
| STG-0B-SEC-EVD-001 | Security, secret, network, and financial isolation | Satisfied |
| STG-0B-TRC-EVD-001 | Requirement and candidate trace assessment | Satisfied |
| STG-0B-CLEAN-001 | Cleanup and residue assessment | Satisfied |
| STG-0B-COMP-001 | Stage 0B completion assessment | Complete |

## Preserved Obligations

This evidence set preserves:

- the obligations fixed by STG-0B-EVD-001;
- original machine-readable results;
- build and dependency facts;
- source and artifact identities;
- the stop and remediation history;
- security and isolation findings;
- cleanup;
- limitations;
- and the final scoped assessment.

Evidence completion does not grant Acceptance, Activation, Stage 0C, production, cloud, or financial authority.

## STG-0B-EVD-001 Obligation Mapping

| Obligation | Preserved Evidence |
|---|---|
| 0B-EVD-001 Approved authority and jurisdiction | GOV-051; GOV-052 |
| 0B-EVD-002 Bootstrap Execution Context | STG-0B-BEC-001; STG-0B-REM-001 |
| 0B-EVD-003 Repository baseline | Commit `777e6a3` before candidate implementation |
| 0B-EVD-004 Environment and runtime epoch | STG-0B-REM-001; STG-0B-OBS-001 |
| 0B-EVD-005 Tool identity, source, version, digest | STG-0B-BLD-EVD-001 |
| 0B-EVD-006 Candidate scope and dependencies | STG-0B-SCP-001; STG-0B-BLD-EVD-001 |
| 0B-EVD-007 Synthetic-material manifest | STG-0B-SYN-001; STG-0B-SEC-EVD-001 |
| 0B-EVD-008 Build observations and outputs | STG-0B-BLD-EVD-001 |
| 0B-EVD-009 Contract results | STG-0B-OBS-001; STG-0B-TRC-EVD-001 |
| 0B-EVD-010 VPL-BST-003 to 005 results | STG-0B-OBS-001; STG-0B-TRC-EVD-001 |
| 0B-EVD-011 Fault, negative, and boundary results | STG-0B-OBS-001 |
| 0B-EVD-012 Security and custody | STG-0B-SEC-EVD-001 |
| 0B-EVD-013 Financial isolation | STG-0B-SEC-EVD-001 |
| 0B-EVD-014 Dependency and provenance | STG-0B-BLD-EVD-001 |
| 0B-EVD-015 Derived Evaluations and context | STG-0B-TRC-EVD-001; STG-0B-COMP-001 |
| 0B-EVD-016 Completeness evaluation | STG-0B-COMP-001 |
| 0B-EVD-017 Challenges and resolutions | STG-0B-STOP-001; GOV-052 |
| 0B-EVD-018 Repository and file changes | Source commit `f250ec5`; this evidence commit |
| 0B-EVD-019 Cleanup and disposition | STG-0B-CLEAN-001 |
| 0B-EVD-020 Completion or stop assessment | STG-0B-COMP-001 |
