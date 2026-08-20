# Canonical Encoding Candidate Manifest

**Manifest ID:** CM-FCE-001  
**Manifest Class:** `CANDIDATE_MANIFEST`  
**Version:** 1.0  
**Status:** Issued  
**Subject:** CND-FCE-001  
**Subject Type:** Canonical Encoding realization  
**Subject Lifecycle:** Candidate  
**Canonical Source SHA-256:** `6F0113456CCE8FF01E4A04E68F4717D9A4CC3605FF3087EFD2CC3D5F978A1B6B`  
**Source:** `src/Falcon.Stage0B.Candidates/CanonicalEncoding.cs`  
**Authority Chain:** GOV-051 → GOV-053 → GOV-055 → GOV-057  
**Bootstrap Execution Context:** STG-0B-BEC-001  
**Environment:** `BOOTSTRAP_EXTERNAL_ID`; local Windows candidate verification  
**Evidence:** Stage 0B Root Evidence Set; RVES-STG-0C-001  
**Governing Rule:** FCE-001  
**Input and Output Classification:** Candidate or Foundation verification evidence only

## Constraints

- no operational runtime status;
- no FIL, persistence, production, cloud, or financial authority;
- no self-Activation;
- no authority beyond exact source digest;
- external tool, identity, and time origin remains bootstrap-classified.

## Integrity

Repository object identity and the preserving commit establish this Manifest’s integrity. No private key or secret is contained or implied.

## Non-Authority

`NO_OPERATIONAL_AUTHORITY`; `NO_STAGE_1`; `NO_PRODUCTION`; `NO_CLOUD`; `NO_FINANCIAL_AUTHORITY`.
