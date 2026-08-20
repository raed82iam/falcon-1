# FSATS P0-P7 Cross-Part Synchronization Matrix

**Date:** `2026-08-15`  
**Branch:** `application-development`  
**Starting audited HEAD:** `534b46d4539037bfdfcf18bba66b8725b9735ae4`

## Cross-part invariants

| Invariant | Current synchronized interpretation |
|---|---|
| Topology | 5 independent Applications: Trading, FSAPMA, Guardian, FSTSimA, APP-RSC |
| FSATS container | non-owning, non-runtime, MSA/LSA/CSA = 0 |
| Awareness totals | 5 MSA / 34 LSA / 7 initial CSA |
| Trading subject | broker account, never FSATS user/customer |
| Broker account identity | BrokerId + BrokerAccountId; Environment additionally material |
| Multi-account | explicit distinct account scopes; reservations/execution/reconciliation remain account-bound |
| Failure locality | account-local by default; expansion requires evidence of shared broker/dependency blast radius |
| Shared Web identity | Web owns customer/user/contact mapping; FSATS receives broker-account scope only |
| Provider identity | Provider != ProviderAccount != ServiceRole != ApiInstance != Endpoint |
| Provider endpoint | URL belongs to endpoint/config binding; URL alone grants no egress/runtime authority |
| Credentials | credential reference != secret bytes |
| Multi-provider | selection among entitled/capable/healthy routes; no provider truth transfers to Trading |
| Market data | operational external data enters FSATS through FSAPMA |
| Broker execution | Trading owns broker execution/reconciliation boundary; same vendor name does not merge data and execution authority |
| Resource coordination | APP-RSC is independent FSATS Application; not Foundation Resource Governance |
| Simulation | FSTSimA remains non-Live validation/simulation owner; evidence != promotion/live authority |
| Guardian | protection authority stays bounded; Guardian != Trading Risk != provider != Foundation |
| Contract lineage | predecessor 43 = semantic migration baseline; P1K 22 = current catalog identities; no implied 65 active routes |
| Web portfolio contracts | exact six v1 semantic IDs materialized in Trading.Contracts; account-scoped, no customer/user principal |
| Runtime | not granted |
| Provider/Broker connectivity | not granted |
| Paper/Shadow/Tiny-Live/Live/Deployment | not granted |

## Part alignment

- P0 current integrated rewrite carries the current topology, broker-account correction, provider/API-instance model and Web semantics, but remains a fresh-review candidate until Owner acceptance.
- P1 accepted historical baseline is preserved. Early Part1-NG pre-APP-RSC candidates are explicitly demoted from current reading authority by `00_CURRENT_SYNCHRONIZATION_OVERLAY_2026-08-15.md`.
- P2 accepted implementation already contains broker-account execution/capital/failure isolation and five-Application topology. Additive current contracts now expose broker-account Web projection shapes and API-instance/endpoint binding primitives without granting runtime.
- P3 durability semantics remain compatible with account-scoped unresolved truth and restart reconstruction.
- P4 lifecycle semantics remain compatible with version/epoch fencing and do not mint authority.
- P5 health/readiness semantics remain compatible with truth/currentness distinctions.
- P6 configuration semantics remain compatible; current provider route binding now additionally models ApiInstanceId + EndpointId + safe EndpointBaseUrl while preserving `CONFIG_VALID != AUTHORIZED`.
- P7: no canonical P7 directory, closure record, accepted executable source, or retrieved prior artifact was found. It cannot be represented as synchronized or accepted without evidence.

## P7 evidence rule

`OWNER_MEMORY_OR_STATEMENT_THAT_P7_WAS_DONE != CANONICAL_P7_BYTES`

No synthetic P7 was created. The missing canonical evidence is an audit blocker, not permission to reconstruct an unknown Part from inference.
