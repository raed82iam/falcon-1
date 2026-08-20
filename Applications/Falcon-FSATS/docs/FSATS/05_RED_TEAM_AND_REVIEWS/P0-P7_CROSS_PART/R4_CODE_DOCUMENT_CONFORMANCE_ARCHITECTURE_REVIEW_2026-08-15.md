# FSATS R4 Code-to-Document Architecture and Consistency Review

**Date:** `2026-08-15`  
**Exact reviewed semantic source:** `aa61f45fae6b4593d387a85600996222a8ee7c55`  
**Scope:** `Current FSATS code/document conformance remediation before any Part 7 work`  
**Part 7 authority:** `NOT_GRANTED / NOT_STARTED`

## 1. Authority and review basis

This review was performed after a fresh read/check of:

- `applications/FSATS/WORKSTREAM_RULES.md`;
- `applications/README.md`;
- `applications/FSATS/README.md`;
- Falcon Vision and Constitution;
- APP-001;
- CON-023;
- ADR-I012;
- ADR-I015;
- current Owner-accepted Parts 0 through 6;
- current R3 closure/handover state;
- current Part 2 Web on-demand-analysis clarification;
- live FCR-0127, FCR-0128, FCR-0130 and the newly received FCR-0201 Application handoff;
- current Trading, FSAPMA, FSTSimA, APP-RSC contract/source and Behavior verifier source affected by the remediation.

Authority was applied as:

```text
SOURCE -> AUTHORITY -> COMPARE -> DECIDE -> CHANGE
```

No runtime, egress, Paper, Shadow, Tiny-Live, Live or deployment authority is inferred from code or contract presence.

## 2. Remediation scope

The exact reviewed source remediates the following previously identified code/document drift:

1. Web on-demand-analysis canonical identity mismatch;
2. account-aware Risk invariant not enforced in code;
3. `NotApplicable` Strategy selector invariant not enforced in code;
4. Web portfolio pagination/account/lineage invariants present in documentation but not fail-closed in constructors;
5. legacy FSAPMA provider route selection capable of bypassing current route identity;
6. provider `EndpointId` not cryptographically/semantically bound at configuration assessment to its catalog provider/service-role/URL identity;
7. Web v1 wire-format policy documented but not executable;
8. APP-RSC awareness-tier helper broader than the actual awareness registry;
9. current FCR-0127/FCR-0130 on-demand and detailed-analysis semantics not fully materialized in executable contract types;
10. FCR-0201 affected-position, affected-order and emergency FSTSimA shadow-monitoring semantics newly handed to Application during this remediation.

## 3. Scope integrity

Comparison from pre-remediation source `a468767d19a484a1aefb94bb3911073af83d776c` to exact reviewed source `aa61f45fae6b4593d387a85600996222a8ee7c55` shows changes only under `applications/FSATS/**`.

No Foundation source, Shared Web source, main/reference branch, or out-of-scope Application tree was modified.

`applications/shared/web/**` remains Web-owned and untouched.

## 4. Web analysis contract conformance

### 4.1 Canonical identities

Current canonical Application/Web identities are now:

```text
FSATS.WebOnDemandAnalysisRequest.v1
FSATS.WebOnDemandAnalysisResult.v1
FSATS.WebDetailedAssetAnalysisProjection.v1

FSATS.WebStrategyCatalogRequest.v1
FSATS.WebStrategyCatalogProjection.v1
FSATS.WebStrategyCatalogUpdate.v1
```

The prior R3 `WebOnDemandAnalysisProjection` / `WebOnDemandAnalysisCommand` surface is retained only as historical/source compatibility and is not represented as the current FCR authority.

**Result:** `ALIGNED`.

### 4.2 Request authority and no-backflow

The canonical request requires `RequestingApplicationId=SHARED_WEB`, exact requested instrument reference and AnalysisIntent. Provider/account/API-instance/endpoint/URL/credential/raw-market-data controls are absent from the public request surface.

```text
WEB_PRESENTATION_DATA -/-> FSATS_ANALYSIS_INPUT
CUSTOMER_REQUEST != PROVIDER_SELECTION_AUTHORITY
```

**Result:** `ALIGNED`.

### 4.3 Resolution and analysis truth

The code now materializes:

```text
COMPLETED
PARTIAL
UNAVAILABLE
UNSUPPORTED
NEEDS_CLARIFICATION
REJECTED
```

`NEEDS_CLARIFICATION` cannot claim a resolved instrument or an analysis projection and must carry bounded clarification candidates. `COMPLETED` requires resolved instrument + analysis projection + complete inputs.

Detailed analysis preserves horizons, Strategies, Schools, synthesis, nullable target/confidence values, evidence references and input truth/freshness.

Fail-closed checks prevent:

```text
STALE/NONCURRENT INPUT -> CURRENT OVERALL TRUTH
PARTIAL INPUTS -> COMPLETE SYNTHESIS
MATERIAL DISAGREEMENT -> FALSE COMPLETE CONSENSUS
```

**Result:** `ALIGNED`.

## 5. Public Risk, Strategy and portfolio conformance

### Risk

`IsAccountAware=true` requires exact `BrokerAccountScope`; general Risk cannot carry account scope.

### Strategy applicability

`NotApplicable` is constructor-enforced as visible + disabled + reason.

### Portfolio

The executable contracts now reject:

- empty/duplicate broker-account scope sets;
- non-positive page sizes;
- `hasMore=true` without a continuation token;
- correction/supersession lineage inconsistent with update kind;
- invalid correction/supersession projection lineage combinations.

**Result:** `ALIGNED`.

## 6. FSAPMA provider-route conformance

The current provider route identity remains:

```text
Provider
+ ProviderAccount
+ Environment
+ ServiceRole
+ ApiInstanceId
+ ProviderEndpointId
+ CredentialReference
```

Historical route construction without API-instance/endpoint identity remains constructible only for historical source/test compatibility, but:

```text
legacy.HasCurrentRouteBinding = false
SelectRoute() -> SelectCurrentRoute()
current route selection requires HasCurrentRouteBinding
```

Current configuration assessment additionally binds `EndpointId` to the registered provider, service role and exact scheme/server/path in `ProviderStreamingCatalog`.

```text
KNOWN URL != EGRESS AUTHORITY
VALID ROUTE BINDING != RUNTIME AUTHORITY
```

**Result:** `ALIGNED`.

## 7. Wire-format conformance

`WebContractSerialization.CreateV1Options()` now materializes:

- lower camel-case JSON fields;
- uppercase snake-case enum tokens;
- no integer enum fallback;
- normal `DateTimeOffset` JSON timestamp behavior;
- decimal JSON numbers and nullable numeric truth.

Adversarial source checks exact examples such as `LAST_KNOWN` and `CORRECTION`.

**Result:** `ALIGNED AT SOURCE/STATIC LEVEL`.

## 8. APP-RSC awareness conformance

`ResourceAwarenessBoundary.IsAwarenessTier()` now resolves only actual MSA/LSA/CSA identities in the current APP-RSC awareness topology. It no longer treats every component except `ResourceStrategyController` as an awareness tier.

`MayMintFoundationGrant=false` remains unchanged.

**Result:** `ALIGNED`.

## 9. FCR-0201 incident and emergency-shadow conformance

### 9.1 Identity and ownership

FSATS continues to own no customer/user principal. Incident public semantics use exact:

```text
BrokerId + BrokerAccountId + Environment
```

Shared Web remains owner of customer-to-account mapping.

### 9.2 Affected positions

Application now materializes separate affected-position follow-up projections with:

- protection classification;
- follow-up requirement/reason;
- ordered actions;
- exact broker-account scope;
- broker-confirmed timestamp;
- truth/freshness/evidence;
- incident/shadow linkage.

`BROKER_CONFIRMED_PROTECTED` requires current/current truth. `UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION` requires customer follow-up. Intentional-without-current-protection remains semantically distinct from unexpected/incomplete protection.

### 9.3 Affected orders

A separate affected-order projection prevents an ambiguous order from being laundered into an invented position. Broker-confirmed order states require current broker truth; ambiguous/reconciliation-required states cannot carry `FOLLOWUP=NONE`.

### 9.4 FSTSimA shadow evidence

FSTSimA shadow subjects may bind to an exact position or an ambiguous source order. At least one is required.

If execution ambiguity is present:

- exact `SourceOrderId` is mandatory;
- separate NOT_EXECUTED / PARTIALLY_EXECUTED / FULLY_EXECUTED v1 scenarios are required;
- simulator evidence remains explicitly non-broker truth.

A position-backed shadow must reference the Trading/Application protection-classification projection. An order-only shadow does not invent one.

Automatic update and on-demand request semantics are both materialized without granting Web power to create/alter a shadow case or release restrictions.

**Result:** `ALIGNED`.

## 10. Backward compatibility and historical evidence

Historical Part 2 through Part 6 executable evidence is not rewritten.

The remediation preserves legacy FSAPMA route construction for historical source/test compatibility while preventing it from becoming a current route. The prior R3 Web projection/command types remain source-compatibility markers only.

No historical PASS is claimed for the new exact source.

**Result:** `ALIGNED`.

## 11. Executable validation state

GitHub Actions attempted Falcon Application CI for the exact semantic source `aa61f45f...` lineage, but the runner did not begin execution because the repository account reported a billing/payment or spending-limit condition. The build/tests/verifiers job was consequently skipped.

Therefore:

```text
SOURCE/STATIC ARCHITECTURE REVIEW = PASS
EXECUTABLE BUILD = NOT EXECUTED
BEHAVIOR VERIFIER EXECUTION = NOT EXECUTED
INTEGRATION/FAILURE/SECURITY EXECUTION = NOT EXECUTED
EXECUTABLE VALIDATION STATUS = BLOCKED_BY_GITHUB_ACTIONS_ACCOUNT_BILLING_OR_SPENDING_STATE
```

A GitHub Actions UI conclusion of `failure` for this event is not interpreted as a code/test failure because the job annotation states the job was not started.

## 12. Architecture / consistency result

```text
R4 CODE-DOCUMENT ARCHITECTURE / CONSISTENCY = PASS_AT_SOURCE_STATIC_LEVEL
OPEN STATIC CRITICAL = 0
OPEN STATIC HIGH = 0
OPEN STATIC MEDIUM = 0
EXECUTABLE VALIDATION = BLOCKED / NOT EXECUTED
RUNTIME / EGRESS / PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT_GRANTED
PART 7 = NOT_STARTED / NOT_AUTHORIZED BY THIS REVIEW
```

This review authorizes no later Part and is not Owner acceptance or closure.
