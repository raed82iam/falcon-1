# FSATS Broad Red Team Review — Advisory Market Onboarding, Free Providers, and Web Presentation

**Date:** `2026-08-16`  
**Reviewed Effective Semantic Set Through Commit:** `8c08d2abf99e3dbd74b899bba879ca7bfb0bab25`  
**Result:** `PASS`  
**Open Critical / High / Medium / Low:** `0 / 0 / 0 / 0`

## 1. Scope

Fresh adversarial review challenged the current planning semantics for:

- `ADVISORY_ONLY` market onboarding;
- Daily/Weekly/Monthly-only opportunities;
- delayed-data eligibility and disclosure;
- free-only provider discovery;
- API-key Owner action requests;
- dynamic market projection to Shared Web;
- dynamic School/Strategy catalog consumption;
- chart source/URL projection;
- Web/FSAPMA data separation;
- current runtime and Part authority boundaries.

## 2. Adversarial Cases

### RT-01 — Convert `ADVISORY_ONLY` into manual execution

**Attack:** Treat the absence of automated execution as permission to track a user-reported trade manually.

**Result:** REJECTED.

`ADVISORY_ONLY != MANUAL_EXECUTION`; execution, advisory-position tracking, and opportunity follow-up remain NONE/disabled.

### RT-02 — Generate intraday signals from delayed data

**Attack:** Use a 15-minute delayed feed to issue intraday opportunities because the source is technically available.

**Result:** REJECTED.

Delayed data is eligible only for the specified Daily/Weekly/Monthly suitability review. `INTRADAY` is disabled and delay disclosure is mandatory.

### RT-03 — Treat 15-minute delay as automatic fitness

**Attack:** Approve any delayed source solely because delay <= 15 minutes.

**Result:** REJECTED.

Completeness, quality, historical coverage, timestamps, terms, required fields, and market coverage still require evaluation.

### RT-04 — Quietly select a paid provider when free sources are poor

**Attack:** Optimize quality by falling back to a paid provider.

**Result:** REJECTED.

Current policy is `FREE_ONLY`; no suitable free source yields `NO_SUITABLE_FREE_PROVIDER_FOUND`.

### RT-05 — Treat a free trial as a free provider

**Attack:** Select a trial that later requires payment for continued required use.

**Result:** REJECTED.

Such a source is classified paid-required for the current policy and rejected.

### RT-06 — Secret API key through chat

**Attack:** Ask Owner to paste the API key into ordinary chat after provider discovery.

**Result:** REJECTED.

The Owner action is metadata only; secret bytes are prohibited from chat and ordinary business payloads. Future secure storage and credential-reference mechanics remain separately governed.

### RT-07 — Discovered provider signup URL becomes phishing path

**Attack:** A discovered or compromised provider metadata record supplies a malicious help/signup URL and Web renders it as trusted.

**Result:** REJECTED AFTER HARDENING.

The controlling security-hardening record requires explicit URL validation state and preserves `DISCOVERED_PROVIDER_URL != TRUSTED_DESTINATION`.

### RT-08 — Chart URL self-authorizes Web connectivity

**Attack:** Treat `sourceUrl` in an FSATS projection as Web egress authority.

**Result:** REJECTED.

`CHART_SOURCE_URL != WEB_PROVIDER_ROUTE_AUTHORITY`; Web provider access remains separately governed per destination.

### RT-09 — Web chart data backfeeds FSATS analysis

**Attack:** Reuse the convenient Web display feed as FSATS operational input.

**Result:** REJECTED.

`WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT`; FSATS operational data remains FSAPMA-owned.

### RT-10 — Same provider means shared credential

**Attack:** Share Web and FSAPMA credentials because both use the same vendor/URL.

**Result:** REJECTED.

`SAME_PROVIDER != SAME_AUTHORITY`; `WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL` unless a separately governed external constraint is explicitly modeled without merging authority.

### RT-11 — New market silently activates strategies

**Attack:** Make every visible School/Strategy active when the market appears in Web.

**Result:** REJECTED.

Catalog visibility/applicability remains separate from activation. Trading remains authoritative catalog owner.

### RT-12 — Hard-code Saudi market into Web

**Attack:** Add Saudi-specific UI branches instead of consuming a dynamic market profile.

**Result:** REJECTED BY CONTRACT DIRECTION.

The market is modeled through a generic projection; Saudi is the motivating instance, not an architectural special case.

### RT-13 — Owner market command equals runtime activation

**Attack:** Interpret `add Saudi market` as immediate provider connection/runtime activation.

**Result:** REJECTED.

Market intent, profile declaration, provider discovery, suitability, credential provisioning, provider authorization, runtime binding, activation, and deployment remain distinct.

### RT-14 — API key provision equals provider authorization

**Attack:** Activate provider connectivity immediately after Owner supplies a key.

**Result:** REJECTED.

`OWNER_PROVIDED_CREDENTIAL != PROVIDER_CONNECTIVITY_AUTHORIZED` and `CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY`.

### RT-15 — Web invents unavailable data or strategy state

**Attack:** Render last-known or missing values as current, or infer strategy/school applicability.

**Result:** REJECTED.

Availability/reason/delay state must remain explicit and Web presentation does not own Strategy/School logic or FSATS analysis truth.

### RT-16 — Use this planning record as Part 8 authorization

**Attack:** Begin market-onboarding implementation or runtime work because the Owner asked to persist the semantics.

**Result:** REJECTED.

The record explicitly remains planning/cross-workstream semantics only. Part 8 and runtime remain unauthorized.

### RT-17 — Close FCR-0082 as a side effect

**Attack:** Treat new planning semantics as completion of canonical Stage 9 Application runtime binding.

**Result:** REJECTED.

FCR-0082 remains Application-held and unchanged.

## 3. Residual Risks

No unresolved semantic blocker was found in the effective planning set.

Future implementation must still provide executable validation for, at minimum:

- fail-closed market mode enforcement;
- delayed-data horizon gating;
- provider cost classification;
- URL validation state;
- no-secret payload enforcement;
- dynamic market/catalog projection validation;
- Web/FSAPMA no-backflow;
- explicit provider and runtime authority gating.

Those are future implementation requirements, not current implementation authority.

## 4. Final Red Team Result

```text
BROAD RED TEAM = PASS
OPEN CRITICAL = 0
OPEN HIGH = 0
OPEN MEDIUM = 0
OPEN LOW = 0
RUNTIME AUTHORITY LEAKAGE = NONE
PART 8 AUTHORITY LEAKAGE = NONE
WEB OWNERSHIP VIOLATION = NONE
FOUNDATION OWNERSHIP VIOLATION = NONE
SECRET-BYTE EXPOSURE PATH IN DEFINED SEMANTICS = NONE
DISCOVERED-URL TRUST ESCALATION = CLOSED
```

The effective semantic set is ready for Owner review and cross-workstream Web compatibility handoff. This PASS is not implementation authority, runtime authority, or final Owner acceptance.
