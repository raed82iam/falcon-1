# FSATS Part 0 - Integrated Current Design Candidate

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / REQUIRES_FRESH_REVIEW / NOT_OWNER_ACCEPTED / NOT_CLOSED`

**Branch:** `application-development`

**Implementation Authority:** `NOT_GRANTED`

**Runtime Authority:** `NOT_GRANTED`

## Purpose

This folder replaces the fragmented Part 0 reading model with one integrated candidate set. It restates the current intended FSATS Part 0 design directly, including later Owner-accepted corrections that were previously scattered across Part 1, Part 2, NEW lineage and follow-on reconciliation records.

The predecessor Part 0 tree was preserved byte-for-byte at:

`applications/docs/FSATS/06_ARCHIVE/PART_0_PRE_INTEGRATED_REWRITE_2026-08-15/`

Historical files remain audit/provenance evidence only. They are not required reading for implementation once this rewrite is eventually Owner-accepted.

## Canonical Part 0 file set

1. `P0-A_GOVERNANCE_AUTHORITY_AND_EVIDENCE.md`
2. `P0-B_REQUIREMENTS_HISTORY_AND_TRACEABILITY.md`
3. `P0-C_APPLICATION_TOPOLOGY_SELF_AWARENESS_LEARNING_RESEARCH_AND_EVOLUTION.md`
4. `P0-D_FOUNDATION_CAPABILITY_CONTRACT_AND_RUNTIME_READINESS.md`
5. `P0-E_APPLICATION_IDENTITY_MANIFEST_LIFECYCLE_AND_DEPLOYMENT_ELIGIBILITY.md`
6. `P0-F_CROSS_APPLICATION_CONTRACTS_AUTHORITY_SECURITY_AND_INFORMATION_FLOW.md`
7. `P0-G_FSAPMA_OPERATIONAL_DATA_FABRIC.md`
8. `P0-H_SELF_AWARE_TRADING_CORE_13_LSA_MODEL.md`
9. `P0-I_GUARDIAN_PROTECTION_CRISIS_SURVIVAL_AND_RECOVERY.md`
10. `P0-J_PERFORMANCE_RESOURCE_QOS_OVERLOAD_AND_RESILIENCE.md`
11. `P0-K_VALIDATION_CREDIBILITY_FSTSIMA_AND_PROMOTION.md`
12. `P0-L_CANONICAL_END_TO_END_INTEGRATION_ASSURANCE_CLOSURE_AND_IMPLEMENTATION_READINESS_GATE.md`

## Current topology used by every file

FSATS is a non-owning, non-runtime system boundary containing exactly five independent Falcon Applications:

1. Falcon Self-Aware Trading Application: `1 MSA / 13 LSA / 3 CSA`
2. Falcon Self-Aware Provider Management Application (FSAPMA): `1 MSA / 6 LSA / 1 CSA`
3. Falcon Trading Guardian Application: `1 MSA / 4 LSA / 1 CSA`
4. Falcon Self-Aware Trading Simulation Application (FSTSimA): `1 MSA / 8 LSA / 2 CSA`
5. Falcon Self-Aware Resource Management Application (APP-RSC): `1 MSA / 3 LSA / 0 CSA initially`

Totals:

```text
APPLICATIONS = 5
MSA = 5
LSA = 34
CSA = 7
```

APP-RSC is FSATS-scoped only. It is not Falcon Foundation Resource Governance, is not an FSATS container principal, and cannot mint or override Foundation resource authority.

## Current business identity rule

FSATS runtime business identity is broker-account centric:

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
TRADING_OPERATING_SUBJECT = BROKER_ACCOUNT
BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = additional identity dimension where material
```

Shared Web owns customer/user/contact mapping to broker-account scope. FSATS does not own customer identity merely because customer-facing requests originate from Web.

## Current authority separation

```text
DESIGN != IMPLEMENTATION_AUTHORITY
IMPLEMENTED != RUNTIME_AUTHORIZED
REQUEST != AUTHORIZATION
DELIVERY != ACCEPTANCE
ROUTE_EXISTS != AUTHORITY
REPLAY != OPERATIONAL
STALE != CURRENT
UNKNOWN != SUCCESS
```

No file in this rewrite activates provider connectivity, broker connectivity, research Internet, Paper, Shadow, Tiny-Live, Live or deployment.

## Open cross-workstream dependencies that remain explicit

Current live FCR state must be refreshed before implementation. Material known dependencies include:

- FCR-0008: research-only Internet egress, Foundation Stage 12 future implementation;
- FCR-0009: transport QoS/deadline governance, Foundation Stage 11 future implementation;
- FCR-0010: canonical resource runtime consumption still pending Foundation canonical consumption mechanism;
- FCR-0011: FSTSimA non-Live isolation/egress guard, Foundation Stage 12;
- FCR-0013: FSAPMA operational provider egress and credential reference, Foundation Stage 12;
- FCR-0014: broker execution egress and credential reference, Foundation Stage 12;
- FCR-0016: canonical Foundation artifact publication/Application consumption, Foundation Stage 14;
- FCR-0012 and FCR-0030: FSA governance/control plane and MSA-to-FSA binding, Foundation Stage 13;
- FCR-0031: APP-RSC canonical runtime binding, pending Foundation Stage 14 consumption capability;
- FCR-0133: current Shared Web portfolio/activity binding metadata obligation remains `Waiting On: APPLICATION` and is separate from this documentation rewrite.

## Review rule

This rewrite does not become controlling merely because the files exist. Required sequence:

```text
OWNER-DIRECTED REWRITE
-> FRESH ARCHITECTURE / CONSISTENCY REVIEW
-> FRESH RED TEAM
-> APPLY REQUIRED CORRECTIONS
-> RE-RUN REVIEWS IF SEMANTICS CHANGE
-> OWNER FINAL ACCEPTANCE
```

Until final acceptance, predecessor accepted evidence remains preserved in archive and Git history.