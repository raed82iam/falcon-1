# FSATS V1.4 Part 0 / P0-G — Start and Scope Control Record

**Status:** `P0G_STARTED`
**Date:** `2026-08-08`
**Authority:** explicit Project Owner instruction to begin the next Part 0 work package after P0-F Owner acceptance and closure
**Scope:** P0-G — FSAPMA Operational-Data Architecture
**Final Owner acceptance:** `NOT_GRANTED`

## 1. Accepted predecessor state

P0-A through P0-F are `OWNER_ACCEPTED_AND_CLOSED`.

P0-G SHALL preserve every accepted predecessor boundary, including:

- `falcon.app.trading.fsapma` as an independent Application inside `falcon.container.trading`;
- exactly one FSAPMA MSA;
- exactly six accepted FSAPMA major branches / LSAs;
- P0-F bilateral cross-Application contract boundaries;
- no direct cross-Application private memory/database/file coupling;
- Foundation ownership of generic admission, routing, delivery, event, security, lifecycle, resource and isolation semantics;
- Application ownership of business meaning;
- Guardian, Trading, FSTSimA and Shared-Application responsibility boundaries;
- operational-data versus research/replay/simulation separation;
- open/partial FCRs remain governed by their actual lifecycle and do not create runtime authority.

## 2. Authorized P0-G design scope

P0-G may design FSAPMA-owned operational-data semantics for the accepted six branches:

1. `P-LSA-01` Provider Registry and Capability Intelligence;
2. `P-LSA-02` Data Products and Data Service Contracts;
3. `P-LSA-03` Provider Selection, Fallback and Business Route-Lease Planning;
4. `P-LSA-04` Data Quality, Lineage and Provider Reconciliation;
5. `P-LSA-05` Provider/API Capacity, Quota and Cost Governance;
6. `P-LSA-06` External Service Role and Provider Onboarding Evidence.

P0-G may define provider/service-role identity, capability truth, product requirements, acquisition planning, provider choice, fallback, quotas/capacity/cost semantics, normalization, quality, provenance, reconciliation, corrections, degradation and evidence.

## 3. Explicit non-scope

P0-G SHALL NOT:

- authorize runtime provider connectivity, credentials, paid subscriptions or purchases;
- create provider-specific Foundation behavior;
- bypass P0-F contracts or Foundation routes;
- own broker order/fill/position/account execution truth;
- choose a market/instrument/strategy/order/position for Trading;
- define Trading Risk values or portfolio/capital-allocation semantics;
- give Guardian generic provider ownership;
- make FSTSimA replay/simulation data operational Live truth;
- treat awareness research Internet results as operational data;
- activate Paper, Tiny Live, Live, deployment or production;
- implement or close a Foundation FCR unilaterally;
- begin P0-H or later work.

## 4. Core authority invariant

```text
TRADING DEFINES WHAT OPERATIONAL DATA PRODUCT IT REQUIRES.
FSAPMA OWNS HOW AN AUTHORIZED OPERATIONAL DATA PRODUCT IS SOURCED, QUALIFIED, RECONCILED AND DELIVERED.
FOUNDATION OWNS THE GENERIC TECHNICAL CROSS-APPLICATION TRANSPORT BOUNDARY.

DATA AVAILABILITY != TRADE AUTHORITY
PROVIDER SELECTION != STRATEGY SELECTION
DATA QUALITY != RISK APPROVAL
DELIVERY SUCCESS != BUSINESS DECISION SUCCESS
```

## 5. External-service role separation

One external vendor may expose multiple service roles. Role coincidence SHALL NOT collapse ownership.

Examples of role classes include:

- operational market-data source;
- reference/corporate-action data source;
- news/alternative-data source where later authorized;
- broker/execution service;
- Paper execution service;
- replay/test/simulation source.

FSAPMA owns provider/service-role evidence and operational-data roles. Trading Execution owns broker execution capability/entitlement and broker order/fill/position/account execution truth. The same vendor name does not merge those responsibilities.

## 6. FCR boundary

FCR-0005 remains open at P0-G start. Foundation WP-05/WP-06 generic portions may be consumed as accepted Foundation capability evidence where current Foundation records support them, but Application-side operational-data semantics and verification remain P0-G work.

No P0-G document may mark FCR-0005 closed without the shared FCR protocol and sufficient Foundation/Application evidence.

## 7. Exit requirement

P0-G cannot become Owner-ready until a fresh Architecture/Consistency and Red-Team review proves at minimum:

- complete ownership coverage for all six FSAPMA LSAs;
- provider/service-role separation;
- deterministic provider-selection/fallback semantics without hidden business authority;
- quota/capacity/cost behavior that cannot silently degrade data truth;
- freshness/time/provenance/quality semantics;
- multi-provider conflict and reconciliation semantics;
- correction/supersession behavior;
- operational/replay/research/simulation isolation;
- entitlement/licensing/credential fail-closed boundaries;
- no broker-execution ownership leakage;
- P0-F contract consistency;
- Foundation neutrality and anti-reimplementation;
- open FCR truth preserved;
- no runtime/deployment authority created.

`P0G_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED`
`P0H_THROUGH_P0L = NOT_STARTED`
