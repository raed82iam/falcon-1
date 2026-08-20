# FSATS V1.4 Part 0 — TARC Amendment Exact Review Binding

**Status:** `EXACT_REVIEW_BINDING`
**Date:** `2026-08-08`
**Branch:** `application-development`

## 1. Historical base

Historical Part 0 closure base before this amendment:

`dc3886087c9f8bc389102200f021a864c0b8cdcb`

## 2. Exact amendment commit chain before review binding

1. `021357ab4e71687dd1c12a4ed5c7f539c4e56c7d` — limited resource-governance reopen record.
2. `abe7f4b148c417c59d0663fe3d4a23c0bab8fedb` — canonical TARC and T-LSA-13 design amendment.
3. `9d3e1bb4dd095d356e1e547473f7c5451683d4c3` — current 13-LSA topology/resource-boundary status.
4. `6cbeb5bf65041c3c984e312dcb49a051fa149b21` — TARC hot-path/availability/single-authority hardening.

## 3. Exact semantic review set

The fresh Architecture/Consistency review and fresh Red-Team SHALL evaluate exactly the semantic delta represented by:

- `169_PART0_LIMITED_RESOURCE_GOVERNANCE_AMENDMENT_REOPEN_RECORD.md`
- `170_TARC_AND_TRADING_RESOURCE_MANAGEMENT_LSA_CANONICAL_AMENDMENT.md`
- `171_TRADING_13_LSA_TOPOLOGY_AND_RESOURCE_BOUNDARY_CURRENT_STATUS.md`
- `172_TARC_HOT_PATH_AVAILABILITY_AND_SINGLE_AUTHORITY_HARDENING.md`

against the preserved historical Part 0 baseline and relevant FCR state.

## 4. Required external consistency references

The review shall treat the following as controlling external/current boundaries where applicable:

- FCR-0007 / Issue #7 current body and controlling TARC clarification comment `5227404787`;
- FCR-0010 / Issue #10 current body and controlling TARC clarification comment `5227405872`;
- Foundation SYS-006 multi-level Resource Governance principle;
- APP-001 / CON-023 / ADR-I012 / ADR-I015 as applicable;
- Falcon Vision and Constitution;
- historical P0-C, P0-I and P0-J semantics except where explicitly superseded by 169-172.

## 5. Binding invariants

The review must fail if any amendment semantics imply:

- a second Trading MSA;
- direct Guardian-to-Foundation requests for Trading Application resources;
- direct LSA/CSA/MSA/Execution/Risk/strategy requests for Trading Application resources;
- a hidden FSATS-wide resource pool;
- TARC authority over independent Applications;
- TARC self-development authority;
- awareness entities in the synchronous TARC hot path;
- a single-process requirement masquerading as single logical authority;
- requested resources being treated as granted;
- Trading resource priority extending beyond technical resource governance;
- reclamation of protected Foundation survival/protection/control floors or non-reclaimable reserves;
- Foundation losing final total-resource allocation/enforcement authority.

## 6. Review state

```text
EXACT_SEMANTIC_SET = 169 + 170 + 171 + 172
HISTORICAL_BASE = dc3886087c9f8bc389102200f021a864c0b8cdcb
FRESH_ARCHITECTURE_REVIEW = PENDING
FRESH_RED_TEAM = PENDING
OWNER_REACCEPTANCE = NOT_GRANTED
```
