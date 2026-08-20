# Stage 0C Scoped Activation Decision Candidates

**Record ID:** STG-0C-DEC-CAND-001  
**Version:** 1.0  
**Status:** Candidate — Not Effective  
**Prepared:** 2026-07-27  
**Authority Required:** Project Owner or separately established competent Activation Authority  
**Root Evidence Set:** RVES-STG-0C-001

## Decision Candidate ACT-DEC-FCE-001

**Subject:** ACT-FCE-001  
**Proposed Decision:** `ACTIVATED_SCOPED`  
**Scope:** Local Foundation evidence canonical encoding and validation only  
**Validity:** Only for source digest `6F0113456CCE8FF01E4A04E68F4717D9A4CC3605FF3087EFD2CC3D5F978A1B6B` under FCE-001 and the preserved Stage 0C evidence  
**Restrictions:** No operational Falcon, FIL, persistence, production, cloud, financial, or Stage 1 authority  
**Expiry:** Earliest of source change, governing-rule change, evidence invalidation, revocation, or 2026-08-10  
**Self-Activation:** Prohibited

## Decision Candidate ACT-DEC-TRUST-001

**Subject:** ACT-TRUST-001  
**Proposed Decision:** `ACTIVATED_SCOPED`  
**Scope:** Local Foundation verification Trust Object construction and validation only  
**Validity:** Only for source digest `DB95C06F5FC422798E19298EA550BB7757A9AEE46C584305E8174D2821DCF6A2` under SEC-002 and the preserved Stage 0C evidence  
**Restrictions:** No Claim establishes truth by itself; no validity implies Acceptance; no operational, production, cloud, financial, or Stage 1 authority  
**Expiry:** Earliest of source change, governing-rule change, evidence invalidation, revocation, or 2026-08-10  
**Self-Activation:** Prohibited

## Non-Activation Decisions Proposed

| Decision ID | Subject | Proposed State |
|---|---|---|
| ACT-DEC-RND-001 | ACT-RND-001 | `NOT_ACTIVATED` |
| ACT-DEC-TIM-001 | ACT-TIM-001 | `NOT_ACTIVATED` |
| ACT-DEC-IDN-001 | ACT-IDN-001 | `NOT_ACTIVATED` |
| ACT-DEC-CRY-001 | ACT-CRY-001 | `NOT_ACTIVATED` |
| ACT-DEC-SEC-001 | ACT-SEC-001 | `NOT_ACTIVATED` |
| ACT-DEC-CID-001 | ACT-CID-001 | `NOT_ACTIVATED` |
| ACT-DEC-ENV-001 | ACT-ENV-001 | `NOT_ACTIVATED — DEFERRED` |
| ACT-DEC-BLD-001 | ACT-BLD-001 | `NOT_ACTIVATED — DEFERRED` |
| ACT-DEC-TRC-001 | ACT-TRC-001 | `NOT_ACTIVATED — DEFERRED` |
| ACT-DEC-PIPE-001 | ACT-PIPE-001 | `NOT_ACTIVATED — DEFERRED` |
| ACT-DEC-GATE-001 | ACT-GATE-001 | `NOT_ACTIVATED — DEFERRED` |

Each row is an independent subject disposition. No group Activation is proposed.

## Effect

None until separately approved. Repository presence, this record, or successful verification does not activate any subject.
