# STG-0C-SUB-001 — Activation Subject and Manifest Catalog

**Identifier:** STG-0C-SUB-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; CON-010  
**Activation Authority:** Not Granted

## 1. Catalog

| Subject ID | Subject | Candidate Lineage | Dependency Class |
|---|---|---|---|
| ACT-FCE-001 | Canonical Encoding realization | CND-FCE-001 | Foundation |
| ACT-TRUST-001 | Trust Object primitives | CND-TRUST-001 | Foundation |
| ACT-RND-001 | Randomness Provider Profile | CND-RND-001 | Source Provider |
| ACT-TIM-001 | Time Provider Profile | CND-TIM-001 | Source Provider |
| ACT-IDN-001 | Identifier Provider Profile | CND-IDN-001 | Dependent Provider |
| ACT-CRY-001 | Cryptographic Provider Adapter Profile | CND-CRY-001 | Dependent Provider |
| ACT-SEC-001 | Secret Provider Profile | CND-SEC-001 | Dependent Provider |
| ACT-CID-001 | Certificate and Identity Provider Profile | CND-CID-001 | Dependent Provider |
| ACT-ENV-001 | Windows Foundation Build Verification Environment | Stage 0B verified environment lineage | Environment |
| ACT-BLD-001 | Build Baseline | CND-BLD scope represented by CND-PIPE-001 evidence lineage | Build |
| ACT-TRC-001 | Machine-readable trace expansion | CND-TRC-001 | Trace |
| ACT-PIPE-001 | Pipeline Definition | CND-PIPE-001 | Pipeline |
| ACT-GATE-001 | Gate Profile and Evidence Requirement generation rules | CND-PIPE-001 | Gate |

`CND-FIX-001` is a verification fixture and shall never become an Activation subject.

## 2. Required Record Per Subject

Every subject requires a separate:

- immutable subject and candidate identity;
- dependency disposition;
- Candidate Manifest;
- Root Verification Evidence Set;
- scoped Validity Assessment;
- Evidence Completeness Decision;
- independent review;
- competent Acceptance and Activation decisions;
- `ACTIVATION_MANIFEST` conforming to CON-010;
- restriction, expiry, revocation, and restoration path;
- residual uncertainty;
- and explicit non-authorities.

## 3. Disposition States

`NOT_EVALUATED`, `UNDER_EVALUATION`, `ACTIVATED_SCOPED`, `NOT_ACTIVATED`, `RESTRICTED`, `SUSPENDED`, `REVOKED`, `EXPIRED`, or `REJECTED`.

No state may be inferred from repository presence, successful verification, or another subject’s state.

## 4. Current Effect

All catalog subjects remain non-active.
