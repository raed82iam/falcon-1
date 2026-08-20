# STG-0B-FIN-001 — Financial Isolation Proof Plan

**Identifier:** STG-0B-FIN-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** Falcon Vision; Falcon Constitution; STG-0B-AUTH-001  
**Approval Record:** GOV-051  
**Financial Authority:** Not Granted

## 1. Purpose

This candidate defines how Stage 0B would prove that candidate construction and verification remain financially sterile.

## 2. Prohibited Connections and Material

- brokers, exchanges, banks, custodians, and market-data providers;
- accounts and credentials;
- financial APIs and SDKs;
- real or delayed market feeds;
- orders, positions, portfolios, balances, and transactions;
- customer financial data;
- production financial datasets;
- and capital-bearing instructions.

## 3. Required Controls

- network denied by default;
- no financial package or endpoint in the dependency manifest;
- no financial environment variable;
- no financial credential or secret;
- synthetic fixtures incapable of external side effects;
- no test requiring financial connectivity;
- and repository and process inspection within the authorized boundary.

## 4. Required Evidence

The case shall record:

- dependency and configuration inspection;
- secret-class inspection without revealing values;
- network-destination review;
- fixture classification;
- output review;
- file-change review;
- and an explicit final isolation finding.

## 5. Failure Rule

Discovery of prohibited material or attempted financial contact shall:

- stop the case;
- prevent retry under the same unchanged conditions;
- preserve non-sensitive evidence;
- contain any affected material;
- notify the Project Owner;
- and require a new authority decision.

## 6. Final Finding

```text
FINANCIALLY_ISOLATED
ISOLATION_NOT_PROVEN
ISOLATION_VIOLATED
```

Only `FINANCIALLY_ISOLATED` may satisfy Stage 0B exit requirements.

No isolation finding grants financial authority.
