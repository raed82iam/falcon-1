# Trust Object Primitives Candidate Manifest

**Manifest ID:** CM-TRUST-001  
**Manifest Class:** `CANDIDATE_MANIFEST`  
**Version:** 1.0  
**Status:** Issued  
**Subject:** CND-TRUST-001  
**Subject Type:** Foundation Trust Object primitives  
**Subject Lifecycle:** Candidate  
**Canonical Source SHA-256:** `DB95C06F5FC422798E19298EA550BB7757A9AEE46C584305E8174D2821DCF6A2`  
**Source:** `src/Falcon.Stage0B.Candidates/TrustObjects.cs`  
**Authority Chain:** GOV-051 → GOV-053 → GOV-055 → GOV-057  
**Bootstrap Execution Context:** STG-0B-BEC-001  
**Environment:** `BOOTSTRAP_EXTERNAL_ID`; local Windows candidate verification  
**Evidence:** Stage 0B Root Evidence Set; RVES-STG-0C-001  
**Governing Rule:** SEC-002  
**Input and Output Classification:** Foundation verification evidence only

## Constraints

- classification as a Trust Object does not establish trust;
- validity remains scoped and does not imply Acceptance;
- Claims remain attributable, verifiable, scoped, and challengeable;
- no operational runtime status or self-Activation;
- no authority beyond exact source digest.

## Integrity

Repository object identity and the preserving commit establish this Manifest’s integrity. No private key or secret is contained or implied.

## Non-Authority

`NO_OPERATIONAL_AUTHORITY`; `NO_STAGE_1`; `NO_PRODUCTION`; `NO_CLOUD`; `NO_FINANCIAL_AUTHORITY`.
