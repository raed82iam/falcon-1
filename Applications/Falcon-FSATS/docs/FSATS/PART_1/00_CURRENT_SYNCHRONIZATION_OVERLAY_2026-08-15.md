# FSATS Part 1 Current Synchronization Overlay

**Status:** `CURRENT_READING_CONTROL / DOES_NOT_REWRITE_HISTORICAL_OWNER_RECORDS`  
**Date:** `2026-08-15`

## Purpose

This overlay removes ambiguity created by early Part 1-NG candidate files that predate the accepted APP-RSC topology.

For current interpretation, the following early candidate files are historical pre-APP-RSC planning input and are not current topology authority:
- `00_PART1NG_MASTER_DESIGN_AND_SCOPE.md`
- `01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`
- `02_PART1NG_DEPENDENCY_FCR_AND_PARALLELIZATION_MODEL.md`
- `03_PART1NG_PART0_TRACEABILITY_AND_COMPLETENESS_REGISTER.md`

They preserve provenance and MUST NOT be deleted or rewritten to pretend they always contained the later decision.

## Current topology

Current FSATS topology is exactly five independent Applications:
1. Falcon Self-Aware Trading Application
2. FSAPMA
3. Falcon Trading Guardian Application
4. FSTSimA
5. APP-RSC — Falcon Self-Aware Resource Management Application

Totals: `5 Applications / 5 MSA / 34 LSA / 7 initial CSA`.

FSATS is a non-owning/non-runtime system boundary and has no MSA/LSA/CSA.

`FSARM` as a system-level/non-Application controller is superseded for current interpretation by the independent APP-RSC Application. APP-RSC is FSATS-scoped and is not Foundation Resource Governance.

## Current business identity

`FSATS_USER_ID = NONE`  
`FSATS_CUSTOMER_ID = NONE`  
`TRADING_OPERATING_SUBJECT = BROKER_ACCOUNT`  
`BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId`  
`ENVIRONMENT = additional identity dimension where material`

Shared Web owns customer/user/contact mapping to exact broker-account scopes.

## Provider route identity

Current FSAPMA interpretation keeps these dimensions distinct:

`PROVIDER != PROVIDER_ACCOUNT != SERVICE_ROLE != API_INSTANCE != ENDPOINT`

Endpoint URL/configuration is bound through an endpoint identity and API-instance configuration. A URL is configuration evidence, not egress or runtime authority. Credential references are identifiers only; secret bytes remain outside Application payload/state.

## Contract lineage

The predecessor 43 cross-Application families remain semantic migration/provenance obligations. The 22 P1K identities are the current implementation/catalog family set. `43 + 22` is not interpreted as 65 simultaneously independent runtime routes.

## Historical closure protection

Part 1 Owner closure records and their exact reviewed semantic instants remain immutable historical evidence. This overlay changes current reading priority only. It does not retroactively change what a historical review proved and grants no implementation/runtime authority by itself.
