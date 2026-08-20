# FSATS V1.4 Part 0 / P0-H — Start and Scope Control Record

**Status:** `P0H_DESIGN_REVIEW_AUTHORIZED_AND_STARTED`
**Date:** `2026-08-08`
**Authority:** explicit Project Owner instruction to start the next Part 0 work package after P0-G closure
**Scope:** `P0-H — Trading Core Design Review`
**Writable branch:** `application-development`
**Starting HEAD:** `c3f2455977f1b4bdf68aab5f61de909c62910b34`

## 1. Entry condition

P0-A through P0-G are `OWNER_ACCEPTED_AND_CLOSED`.

P0-H is authorized for architecture/design/review only. This record grants no implementation, runtime, provider/broker connectivity, Paper, Tiny Live, Live, deployment or production authority.

## 2. Purpose

Review and define the coherent Application-owned Trading Core architecture across the previously accepted twelve Trading major branches without redesigning predecessor decisions or transferring authority to Foundation, Guardian, FSAPMA, Shared Applications, FSTSimA or awareness entities.

## 3. Canonical Trading major branches preserved

- `T-LSA-01` Operations, Tenant, Account and Environment Control
- `T-LSA-02` Market Profiles, Universe and Instrument Eligibility
- `T-LSA-03` Analysis Frameworks and Market Interpretation
- `T-LSA-04` Trading Schools and Strategy Management
- `T-LSA-05` Opportunity, Proposal and Decision Orchestration
- `T-LSA-06` Unified Risk Management
- `T-LSA-07` Portfolio and Trading Capital Allocation
- `T-LSA-08` Trading Intent and Horizon Governance
- `T-LSA-09` Execution and Broker Interaction
- `T-LSA-10` Position, Fill Allocation and Reconciliation
- `T-LSA-11` Learning, Performance Attribution and Evolution
- `T-LSA-12` Trading Business Continuity, Readiness and Runbooks

Exactly one LSA remains associated with each accepted major branch. P0-H does not create or remove LSAs.

## 4. Mandatory predecessor boundaries

P0-H SHALL preserve:

- P0-C awareness jurisdiction: CSA/LSA/MSA remain Application-side; FSA is OS/Foundation governance and not a Trading decision layer;
- P0-D Foundation/Application separation and anti-reimplementation;
- P0-E Application/container/Manifest/lifecycle/identity boundaries;
- P0-F cross-Application contracts, user/Owner command semantics, subscription-expiry and managed-exit semantics;
- P0-G FSAPMA as sole operational external-data gateway and exact data-provider versus broker-execution role separation;
- Guardian as independent Trading protection/crisis authority;
- one shared authoritative Trading Risk across all markets;
- FSTSimA as independently governed non-Live simulation/validation Application.

## 5. Design questions in scope

P0-H shall define and Red-Team at minimum:

1. tenant/user/account/environment ownership and isolation;
2. market-profile and market-universe truth;
3. analysis-framework and market-interpretation boundaries;
4. central strategy catalog, Trading Schools, Strategy Controller and strategy applicability;
5. opportunity/proposal/decision pipeline and evidence;
6. Unified Risk relationship to strategies, capital and execution;
7. portfolio/capital allocation and reservation semantics;
8. trading intent/horizon semantics and lifecycle;
9. broker execution ownership and interaction boundaries;
10. order/fill/position allocation and reconciliation truth;
11. learning/performance attribution/evolution without self-approval;
12. Trading business readiness/continuity/runbooks without recreating Foundation technical lifecycle;
13. multi-user, multi-market and multi-broker isolation;
14. Paper/Tiny Live/Live authority separation at design level only;
15. fast-path business semantics without bypassing Risk, Guardian, evidence or Foundation contracts;
16. interaction with accepted P0-F Owner/user/subscription controls;
17. interaction with P0-G Data Products and provider/broker dual-role credential capacity boundaries.

## 6. Explicit non-authorities

P0-H SHALL NOT:

- modify Foundation;
- implement or close any FCR;
- create runtime routes or external connectivity;
- choose exact production brokers/providers or purchase paid services;
- authorize Paper/Tiny Live/Live trading;
- authorize deployment or production;
- start P0-I or later work;
- reinterpret accepted predecessor closures merely for implementation convenience.

## 7. Required exit process

P0-H requires:

1. canonical Trading Core design candidate;
2. fresh architecture/consistency review;
3. adversarial Red-Team of the exact updated semantic set;
4. remediation of all material findings followed by fresh re-review;
5. explicit Owner final review and acceptance before closure.

`P0H_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED`
`P0I_THROUGH_P0L = NOT_STARTED`
