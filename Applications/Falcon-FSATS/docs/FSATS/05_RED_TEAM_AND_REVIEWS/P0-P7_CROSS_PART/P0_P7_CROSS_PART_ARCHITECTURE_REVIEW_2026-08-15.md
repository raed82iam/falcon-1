# FSATS P0-P7 Fresh Cross-Part Architecture / Consistency Review

**Date:** `2026-08-15`  
**Starting HEAD:** `534b46d4539037bfdfcf18bba66b8725b9735ae4`  
**Scope:** current P0-P6 documentation + current FSATS source/contracts/tests + P7 evidence search

## Result

`P0-P6 ARCHITECTURE / CONSISTENCY = PASS AFTER REMEDIATION`  
`P7 = BLOCKED_BY_MISSING_CANONICAL_EVIDENCE`  
`P0-P7 OVERALL = NOT_FULL_PASS`

## Remediated architecture findings

### A-01 HIGH — early P1 topology could override current APP-RSC reading
Resolved by an explicit current synchronization overlay. Historical Part1-NG candidate files remain provenance only; current topology is 5 Applications and APP-RSC is independent.

### A-02 HIGH — provider URL existed but route identity did not model API instance/endpoint explicitly
Resolved additively. FSAPMA now exposes `ApiInstanceId` and `ProviderEndpointId` in domain/contracts, a current fully-bound route constructor/predicate, a current-route selector, and configuration binding for `ApiInstanceId + EndpointId + EndpointBaseUrl`. Existing endpoint catalog remains the URL source instead of duplicating URL truth.

### A-03 HIGH — internal broker-account model was stronger than public Trading contract surface
Resolved. Trading.Contracts now exposes exact `BrokerAccountScope(BrokerId, BrokerAccountId, Environment)` plus implementation-ready Shared-Web portfolio/position/activity/performance v1 payload shapes. No FSATS user/customer principal was introduced.

### A-04 HIGH — FCR-0133 semantic payload IDs lacked a public Application-side materialization
Resolved on the Application side by `applications/FSATS/contracts/web/FSATS.WebPortfolioContracts.v1.md` and the corresponding C# contract types. Runtime binding remains separately governed and is not granted by this review.

## Preserved boundaries

- no Foundation ownership was copied into Applications;
- no Shared Web code was modified;
- no secret bytes were introduced;
- no URL or config value is treated as authority;
- no runtime route/provider/broker/Paper/Live/deployment authority was granted;
- historical Owner-closure records were not rewritten.

## Remaining blocker

No canonical P7 evidence was found. Architecture review therefore cannot certify P0-P7 as a complete chain. P0-P6 is synchronized after the listed remediation; P7 remains evidence-blocked.
