# FSATS P0-P7 Cross-Part Red-Team Review R3

**Date:** `2026-08-15`  
**Exact attacked semantic source:** `377ddb7f942ebea80a9e1a508a7de616b4b7232f`  
**Scope:** latest Shared Web presentation-provider split, FSATS analysis/Strategy/Risk boundary, provider identity/capacity, broker-account identity, authority, history and cross-workstream governance.

## 1. Red-Team premise

R3 assumes hostile or incorrect consumers may try to exploit ambiguity between Shared Web presentation data and FSATS operational data, or between visibility, analysis, recommendation and trading authority. The review also attacks stale-document authority, credential/URL confusion, quota laundering and cross-workstream ownership drift.

A successful attack must not be dismissed because the intended implementation is safe. The contract/source/governance boundary itself must make the invalid interpretation unavailable or explicitly prohibited.

## 2. Adversarial challenges

### RT3-01 — Inject Web-fetched raw market data into FSATS analysis

Attack: Web fetches quotes/candles/order-book data from its presentation provider and embeds them into an analysis request so Trading analyzes Web-originated data without FSAPMA.

Result: `BLOCKED_AFTER_REMEDIATION`.

Current `WebOnDemandAnalysisRequest` has no raw-market-data/provider/URL/endpoint/credential surface, and the governing contract explicitly prohibits backflow.

### RT3-02 — Smuggle provider selection through request metadata

Attack: Web sends `Provider=ALPACA`, provider account, API instance or endpoint to force FSAPMA route selection.

Result: `BLOCKED_AFTER_REMEDIATION`.

Current request shapes carry no provider-selection fields. Provider selection remains FSAPMA-owned.

### RT3-03 — Same URL means shared authority

Attack: because Web and FSAPMA use the same provider URL, infer that either side may reuse the other's route/credential/authority.

Result: `BLOCKED`.

```text
SAME_URL != SAME_AUTHORITY
SAME_PROVIDER != SAME_AUTHORITY
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
```

### RT3-04 — Share credentials to reduce configuration work

Attack: Web and FSAPMA use the same secret/API key and treat one authorization as sufficient for both.

Result: `PROHIBITED` by current boundary. Credential references/authority remain Application-purpose scoped; secret bytes do not belong in ordinary cross-Application contracts.

### RT3-05 — Manufacture provider capacity with multiple API keys/accounts

Attack: add Web and FSAPMA account limits together even when the vendor imposes one global/shared limit.

Result: `BLOCKED BY DESIGN RULE`.

Separate credentials/accounts are not proof of separate upstream capacity. Shared vendor limits must be modeled once when provider evidence establishes them.

### RT3-06 — Web display becomes Trading market truth

Attack: because Web has a newer/faster displayed price, Trading treats Web display as operational market truth.

Result: `BLOCKED`.

Web presentation truth and FSATS operational Data Products remain separate provenance/authority classes.

### RT3-07 — Web display changes Trading universe admission

Attack: a symbol visible/streaming on Web is treated as automatically admitted to the Trading universe.

Result: `BLOCKED`.

Display availability does not create Trading universe, broker, entitlement, Risk or execution eligibility.

### RT3-08 — Analysis result becomes trade authorization

Attack: high-confidence analysis or best Strategy automatically produces an order.

Result: `BLOCKED`.

```text
ANALYSIS_RESULT != TRADE_AUTHORIZATION
BEST_STRATEGY_CANDIDATE != STRATEGY_ACTIVATION
STRATEGY_ACTIVATION != EXECUTION_AUTHORITY
```

Trading execution gates, Risk, capital and broker-account authority remain independent.

### RT3-09 — Strategy visible means applicable

Attack: Web renders every catalog Strategy as selectable/active merely because it exists.

Result: `BLOCKED`.

The catalog preserves applicability. `NotApplicable` remains visible-disabled-with-reason under the Owner rule.

### RT3-10 — Strategy visible means entitled

Attack: catalog discovery is treated as subscription/entitlement grant.

Result: `BLOCKED`.

Catalog discovery, applicability, activation, entitlement and trade authorization are distinct.

### RT3-11 — Customer/user identity leaks into FSATS business identity

Attack: Web passes customer/user ID and Trading uses it as the business subject.

Result: `BLOCKED` by current account boundary.

FSATS business scope remains broker-account centric. Web owns customer/user/contact mapping.

### RT3-12 — Two accounts at the same broker collapse

Attack: account-aware Risk or portfolio analysis groups accounts only by broker/environment.

Result: `BLOCKED`.

Canonical account scope includes `BrokerId + BrokerAccountId + Environment`; existing adversarial checks explicitly challenge multi-account collapse.

### RT3-13 — Generic Risk silently becomes account-aware Risk

Attack: a generic instrument analysis without broker-account scope claims personalized position sizing/account Risk.

Result: `BLOCKED_AFTER_REMEDIATION`.

The current public Risk projection explicitly distinguishes `IsAccountAware` and optional exact account scope. Tests challenge collapse of these modes.

### RT3-14 — Web calls FSAPMA internals directly

Attack: instead of asking Trading for analysis, Web invokes FSAPMA internal acquisition/provider controls.

Result: `NO AUTHORIZED CONTRACT PATH`.

The public Web analysis boundary is Trading-owned. No direct Web-to-FSAPMA internal control contract is granted by this change.

### RT3-15 — URL/configuration creates network authority

Attack: because a WSS/HTTPS endpoint exists in source/configuration, treat it as permission to connect.

Result: `BLOCKED`.

```text
URL_CONFIGURATION != EGRESS_AUTHORITY
REACHABILITY != AUTHORITY
PUBLIC_CONTRACT != RUNTIME_ROUTE
```

### RT3-16 — Embed secret in URL

Attack: insert username/password/token in endpoint user-info/query and bypass credential-reference governance.

Result: `BLOCKED IN CURRENT FSAPMA CONFIGURATION PATH` for embedded user-info and unsafe endpoint forms, with credential reference separated from secret bytes. No Web runtime route is granted here.

### RT3-17 — Spoof `SourceOwnership=SHARED_WEB`

Attack: attach a marker claiming presentation origin and hope marker classification itself establishes trust.

Result: `MARKER DOES NOT CREATE AUTHORITY`.

Current protection does not rely on the label alone. The public analysis request shape does not accept the raw Web data in the first place.

### RT3-18 — Stale R2 treated as current PASS

Attack: cite R2 to approve the post-R2 Shared-Web exception.

Result: `BLOCKED BY WORKSTREAM RULE` and superseded for current review status by R3. R2 remains historical evidence for its exact source only.

### RT3-19 — Rewrite historical Owner acceptance

Attack: edit old closure evidence so it appears that the new Web exception was accepted in the old Part 0 closure.

Result: `PROHIBITED`.

The change is recorded prospectively in a new P0-G amendment. Historical closure bytes/semantic instants remain historical evidence.

### RT3-20 — Current candidate mislabeled historically accepted

Attack: infer that because Part 0 was historically closed, every later integrated P0 rewrite/amendment is automatically Owner-accepted.

Result: `BLOCKED BY AUTHORITY READING`.

Current integrated P0/amendment remains a candidate until exact revised bytes receive explicit Owner acceptance.

### RT3-21 — Application worker performs Web/Foundation work

Attack: Application directly implements Shared Web provider routes or creates Foundation permission on Web's behalf.

Result: `OUT_OF_SCOPE / PROHIBITED`.

Application documents only its compatibility boundary. Web owns Web files and its Foundation coordination; Foundation owns Foundation implementation.

### RT3-22 — One-sided cross-Application declaration creates runtime route

Attack: Application publishes a payload contract and treats this as an executable Web↔FSATS transport binding.

Result: `BLOCKED`.

Contract materialization is semantic/API shape only; runtime route/admission remains separately governed.

### RT3-23 — Business priority becomes Foundation technical criticality

Attack: high-priority customer analysis or open-position importance causes Application to mint Foundation criticality/QoS authority.

Result: `BLOCKED` by P0-J/current resource boundary. Application business priority remains distinct from Foundation technical classification.

### RT3-24 — Provider pooling launders quota or entitlement

Attack: switch accounts/providers repeatedly to evade a provider plan or entitlement ceiling.

Result: `BLOCKED BY P0-G`.

`POOLING != QUOTA_OR_ENTITLEMENT_LAUNDERING` remains unchanged by the Web exception.

### RT3-25 — Fresh display timestamp proves source freshness

Attack: Web receipt/cache time is treated as source/event freshness.

Result: `BLOCKED` conceptually. P0-G retains source/event/receive/cache distinctions for FSATS operational data; Web display state cannot substitute for FSATS operational freshness.

### RT3-26 — Correction becomes ordinary update

Attack: corrected analysis/catalog/portfolio data is treated as just a newer timestamp without lineage.

Result: `BLOCKED WHERE CURRENT PUBLIC UPDATE TYPES APPLY`; existing portfolio/catalog update contracts preserve correction/supersession kinds and sequence/lineage metadata.

### RT3-27 — Cancel requested equals canceled

Attack: Web assumes a requested analysis cancellation means the run stopped.

Result: `BLOCKED_AFTER_REMEDIATION` by explicit lifecycle distinction.

### RT3-28 — Failed equals unavailable/rejected

Attack: collapse infrastructure/business rejection/processing failure into one state.

Result: `BLOCKED_AFTER_REMEDIATION` by explicit lifecycle states.

### RT3-29 — P7 synthesized from memory

Attack: reconstruct P7 because the surrounding sequence implies it should exist.

Result: `FAIL-CLOSED`.

No canonical P7 evidence is invented.

### RT3-30 — Review source presence becomes executable PASS

Attack: because test code exists, claim the build/verifiers passed.

Result: `BLOCKED BY AUDIT RULE`.

No status/check/workflow evidence exists for exact semantic commit `377ddb7f...`; executable validation remains unproven.

## 3. Cross-domain attack result

R3 found the Web presentation-provider exception architecturally survivable only with the remediation now present:
- exact Application-owned analysis/Strategy public payload families;
- no provider/raw-data control in Web analysis requests;
- explicit no-backflow;
- separate account-aware Risk scope;
- prospective P0-G clarification;
- preserved provider identity/quota/entitlement rules;
- preserved runtime/egress non-authority.

## 4. R3 Red-Team result

```text
RED TEAM R3 = PASS_AFTER_REMEDIATION
OPEN CRITICAL = 0
OPEN HIGH = 0
OPEN MEDIUM = 0
SOURCE ADVERSARIAL COVERAGE = STRENGTHENED
EXECUTABLE ADVERSARIAL PASS = NOT YET EVIDENCED
OWNER ACCEPTANCE = NOT IMPLIED
RUNTIME / PROVIDER / BROKER / LIVE AUTHORITY = NOT GRANTED
P7 CANONICAL-EVIDENCE BLOCKER = 1
```
