# FSATS V1.4 Part 0 — TARC Amendment Post-Remediation Exact Binding

**Status:** `POST_REMEDIATION_EXACT_BINDING`
**Date:** `2026-08-08`
**Branch:** `application-development`

## 1. Historical base

Historical Part 0 closure base:

`dc3886087c9f8bc389102200f021a864c0b8cdcb`

## 2. Exact amendment set for fresh final review

The controlling semantic amendment set is:

- `169_PART0_LIMITED_RESOURCE_GOVERNANCE_AMENDMENT_REOPEN_RECORD.md`
- `170_TARC_AND_TRADING_RESOURCE_MANAGEMENT_LSA_CANONICAL_AMENDMENT.md`
- `171_TRADING_13_LSA_TOPOLOGY_AND_RESOURCE_BOUNDARY_CURRENT_STATUS.md`
- `172_TARC_HOT_PATH_AVAILABILITY_AND_SINGLE_AUTHORITY_HARDENING.md`
- `174_T_LSA01_T_LSA13_RESOURCE_OWNERSHIP_DECONFLICTION.md`

`173_TARC_AMENDMENT_EXACT_REVIEW_BINDING.md` is historical pre-remediation binding and is superseded by this record for final review.

## 3. Commit lineage

- `021357ab4e71687dd1c12a4ed5c7f539c4e56c7d` — 169
- `abe7f4b148c417c59d0663fe3d4a23c0bab8fedb` — 170
- `9d3e1bb4dd095d356e1e547473f7c5451683d4c3` — 171
- `6cbeb5bf65041c3c984e312dcb49a051fa149b21` — 172
- `f590f177249d2bf3941c5b30ef4b5d129ab447a8` — 174

## 4. Controlling invariants

Fresh final review shall fail if the set permits:

1. more than one Trading MSA;
2. fewer or more than the explicit 13 current Trading LSA rooms absent a later Owner decision;
3. T-LSA-01 owning or competing for technical-resource governance;
4. any Trading internal role other than TARC submitting Trading Application resource requests to Foundation;
5. Guardian direct/break-glass requests for Trading Application resources;
6. TARC authority over independent Applications or an FSATS-wide hidden pool;
7. TARC development/self-improvement/strategy/Risk/execution authority;
8. awareness entities as mandatory synchronous dependencies of TARC runtime control;
9. multiple independent TARC requester principals under redundancy;
10. stale/split-brain TARC authority proceeding open;
11. requested capacity treated as granted capacity;
12. Trading resource priority extending outside Foundation-governed technical resources;
13. protected Foundation survival/protection/control floors or non-reclaimable reserves being consumed/reclaimed by Trading priority;
14. Foundation losing final total-resource grant/cap/deny/reclaim/rebalance/restore authority.

## 5. Review state

```text
POST_REMEDIATION_SEMANTIC_SET = 169 + 170 + 171 + 172 + 174
FRESH_ARCHITECTURE_CONSISTENCY_REVIEW = REQUIRED
FRESH_RED_TEAM = REQUIRED
AMENDED_PART0_OWNER_ACCEPTANCE = NOT_GRANTED
```
