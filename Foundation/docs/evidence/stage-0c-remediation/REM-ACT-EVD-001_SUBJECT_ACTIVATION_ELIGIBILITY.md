# Stage 0C Remediation Subject Activation Eligibility

**Evidence ID:** REM-ACT-EVD-001  
**Version:** 1.0  
**Status:** Evaluation Complete; Decisions Pending  
**Authority:** GOV-058  
**Root Evidence Set:** RVES-STG-0C-REM-001

| Subject | Eligibility | Exact Scope |
|---|---|---|
| ACT-RND-001 | Eligible | OS CSPRNG through Falcon Adapter; local Foundation verification |
| ACT-TIM-001 | Eligible | Windows local-build clock Profile with declared uncertainty and single-source limitation |
| ACT-IDN-001 | Eligible | Internal Foundation UUIDv7 issuance through Falcon Contract |
| ACT-CRY-001 | Eligible | BCL crypto Adapter with domain, purpose, nonce, and opaque custody enforcement |
| ACT-SEC-001 | Eligible | Ephemeral non-exporting Foundation Secret Provider |
| ACT-CID-001 | Eligible | Digest-pinned local certificate/identity verification |
| ACT-ENV-001 | Eligible | Exact local Windows Foundation build-verification environment |
| ACT-BLD-001 | Eligible | Exact deterministic BCL-only Release Build Baseline |
| ACT-TRC-001 | Eligible | Exact 953-entry machine-readable authoritative atomic trace |
| ACT-PIPE-001 | Eligible | Exact local remediation verification Pipeline Definition |
| ACT-GATE-001 | Eligible | Complete-evidence, non-self-promoting Gate Profile |

Eligibility is limited by the exact source, Profile, evidence, environment, non-authorities, and expiry recorded in the related Manifest candidate.

No subject is eligible for operational Falcon, production, cloud, financial activity, or Stage 1.
