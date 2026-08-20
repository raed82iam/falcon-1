# 03 — Runtime Integration and FCR Dependency Plan

**Status:** `MASTER_PLAN_CANDIDATE`  
**FCR snapshot date:** 2026-08-17  
**Rule:** this matrix is an execution aid, not a substitute for a fresh repository-wide FCR check before work.

# 1. Integration architecture

All cross-workstream integrations terminate at Web-owned ports/adapters before presentation code.

```text
UI / FEATURE
    ↓
WEB PRESENTATION / STATE POLICY
    ↓
WEB-OWNED PORT
    ↓
GOVERNED ADAPTER
    ↓
FOUNDATION / APPLICATION PUBLIC CONTRACT
```

Forbidden:

```text
UI -> FOUNDATION INTERNALS
UI -> FSATS INTERNALS
UI -> PROVIDER/BROKER INTERNALS
```

Network transport belongs in adapters, not feature modules.

# 2. Runtime data classes

Every composed datum must be classified as one of:

- authoritative projection state;
- Web interaction state;
- Web/user presentation preference state;
- explicit Preview/demo fixture state.

Preview and authoritative source profiles must remain mutually exclusive. Demo truth may never satisfy a missing authoritative dependency.

# 3. Application contract families already materially reconciled Web-side

The current Web source already has substantial binding/presentation work for:

- FSATS dynamic Strategy/School catalog;
- Trading overlays;
- on-demand analysis request/result semantics;
- detailed analysis/synthesis presentation;
- portfolio/positions/activity/performance semantics;
- incident affected-position/order/FSTSimA shadow presentation;
- Owner AI emergency presentation/request boundary;
- Web presentation market-data planning and exact destination records.

These must be preserved and specialized through ports/adapters rather than rewritten into direct UI assumptions.

# 4. Current FCR execution matrix

## FCR-0169 — Foundation operational projection

Current state: `FOUNDATION_IMPLEMENTED / Waiting On: WEB`.

Web action:

- bind the accepted Foundation Stage 14 public operational projection;
- preserve source freshness/availability/completeness;
- expose system status in Owner Command Center;
- do not convert health projection into repair authority;
- add exact contract/adversarial tests;
- complete governed Web verification.

Mandatory:

```text
HEALTH_PROJECTION != REPAIR_AUTHORITY
RESOURCE_STATE_PROJECTION != RESOURCE_AUTHORITY
WEB_RUNTIME_AUTHORITY = NOT_GRANTED_BY_PROJECTION
```

This is a priority Web-owned binding because it unlocks truthful Owner system presentation.

## FCR-0095 — Customer incident/notification/support/voice stack

Current state: `APPLICATION_VERIFIED / Waiting On: WEB`.

Source-level semantics are advanced and previously governed-verified for an earlier exact checkout. Remaining production/runtime completion includes, as applicable:

- authoritative identity/session/MFA and principal/tenant binding;
- production incident persistence policy;
- governed screenshot scanner;
- exact local Whisper.cpp/Piper executable binding;
- authorized Support/contact transport;
- final browser/runtime keyboard/focus/accessibility verification.

Web must not weaken fail-closed behavior to close the FCR faster.

## FCR-0220 — Advisory/provider presentation semantics

Current state: `SUBMITTED / Waiting On: WEB` with Application semantics verified and final provider runtime/UI binding pending.

Web action:

- bind exact provider presentation routes;
- preserve user-request-only FSATS analysis behavior;
- preserve independent Web source first;
- enforce 50/50 only per exact shared constrained provider dimension;
- bind Web principal/service role/policy/credential reference where required;
- verify unknown quota scope/limit fails closed;
- do not activate connectivity until governed prerequisites pass.

## FCR-0125 — Web chart/presentation market data

Current state: `APPLICATION_VERIFIED / Waiting On: WEB`.

Web action:

- finish exact Web principal/service-role and policy binding;
- finish credential-reference binding where required;
- verify presentation-only boundary;
- verify no Web display data backflow to FSATS analysis;
- perform final runtime verification before connectivity activation.

## FCR-0173 through FCR-0177 — presentation WebSocket routes

Current state: Foundation portion implemented/closed; `Waiting On: WEB`.

Destinations include governed presentation routes for:

- Binance;
- Coinbase;
- Bybit;
- Alpaca IEX;
- Finnhub.

Web action per route:

1. exact route identity match;
2. Web principal/service role;
3. exact Web route policy;
4. credential reference when required;
5. governed verification;
6. only then connectivity activation if separately authorized.

Public route does not mean unrestricted egress.

## FCR-0196 through FCR-0200 — full-market presentation destinations

Current state: Foundation portion implemented/closed; `Waiting On: WEB`.

Covers:

- Alpaca US-equity universe;
- Alpaca historical bars;
- Binance Crypto Spot universe;
- Binance historical klines;
- Binance broad-market mini ticker.

Same per-destination binding/verification rule applies.

## FCR-0152 — authoritative identity/session/MFA

Current state at this plan snapshot: `Waiting On: FOUNDATION`; Foundation Stage 16 source/remediation exists but final governed executable revalidation remains pending.

Web action now:

- preserve fail-closed protected surfaces;
- prepare adapter/contract consumption tests without inventing runtime truth;
- do not activate authoritative Owner/customer/Support protected runtime from historical assumptions.

After Foundation handoff to `WEB`:

- bind exact identity/session/security-context projection;
- verify Owner direct routing;
- verify customer tenant scope;
- verify Support capability/session semantics;
- verify logout/session rotation/revocation behavior as projected;
- run authenticated browser/accessibility tests;
- re-check FCR-0095 dependencies.

## FCR-0076 — Stage 9 recovery/release presentation

Current state: `Waiting On: FOUNDATION`.

Web already has a fail-closed adapter. Foundation still needs to identify/materialize the exact Web-consumable authoritative Stage 9 recovery/release projection.

Web must not invent a route while waiting.

## FCR-0077 — Application/Web emergency/control planning semantics

Current state: `Waiting On: NONE` for current scope.

Preserve:

- APP-RSC and P1-K planning semantics;
- Web as presentation/request surface only;
- `REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED`;
- stale/unknown/unavailable explicitness.

## FCR-0225 — Web Owner AI Kill path

Current state: `CLOSED / Waiting On: NONE` for current source-binding scope.

Preserve the existing fail-closed Web implementation:

- exact target/scope/blast-radius validation;
- targeted vs global distinction;
- Safe Core semantics;
- Owner-only protected route;
- no Release/Revival authority;
- no live endpoint invented by closure.

## FCR-0226 — Application AI target registration / Kill binding

Current state: `Waiting On: APPLICATION` for FSATS Application AI targets.

Important to new Web MSA/LSA planning:

- the current documented Stage 13 Application target model enumerates FSATS Application AI targets, not the newly proposed Shared Web MSA/LSA;
- Web must not silently assume those new Web AI entities are registered or kill-bound;
- before Web AI production activation, determine whether an existing generic Foundation contract already supports Web-owned AI registration;
- if exact support is absent, raise a Web-owned Foundation FCR for generic registration/containment binding rather than modifying Foundation.

## FCR-0012 / FCR-0030 — awareness/FSA governance interface

Current state: Foundation portions implemented/revalidated; `Waiting On: APPLICATION` for current FSATS consuming side.

Relevance to Web AI:

- `SELF_AWARENESS != AUTHORITY` remains governing;
- existing lower-tier-awareness -> FSA interfaces were designed around the current governed consumer model;
- Web MSA/LSA must not claim those interfaces automatically;
- determine exact applicability during Web AI contract reconciliation and raise FCR if a generic Web-aware consumer binding is missing.

# 5. Closed/current Application/Web contracts to preserve

The Master Plan treats the following current semantic scopes as established inputs unless a newer FCR reopens them:

- FCR-0126 Trading overlay presentation;
- FCR-0127 on-demand analysis request/result presentation;
- FCR-0128 dynamic School/Strategy catalog;
- FCR-0130 detailed AI analysis and synthesis;
- FCR-0133 portfolio/positions/activity/performance;
- FCR-0203 incident affected-position/FSTSimA shadow projection;
- FCR-0225 Owner AI emergency presentation/request binding.

Do not reopen solved semantics merely because this Master Plan reorganizes implementation.

# 6. Provider binding readiness model

Each Web provider route must satisfy all required fields before activation:

```text
EXACT_FCR
+ EXACT_ROUTE/PATH
+ WEB_PRINCIPAL
+ WEB_SERVICE_ROLE
+ WEB_POLICY
+ OPAQUE_WEB_CREDENTIAL_REFERENCE_WHEN_REQUIRED
+ GOVERNED_VERIFICATION
= ROUTE_READY_FOR_SEPARATELY_AUTHORIZED_ACTIVATION
```

Missing any required field fails closed.

# 7. Quota/capacity implementation

Per exact provider-enforced dimension:

```text
IF SUITABLE_INDEPENDENT_WEB_SOURCE:
    USE WEB SOURCE
    DO NOT SHARE FSAPMA QUOTA
ELSE IF SAME_REAL_CONSTRAINED_POOL:
    WEB_HARD_CEILING = 50%
    FSAPMA_RESERVED = 50%
    WEB_SOFT_THROTTLE < 50%
ELSE:
    NO ARTIFICIAL 50/50 RULE
```

Unknown pool identity or unknown constrained effective limit must not be treated as unlimited capacity.

# 8. Voice runtime binding

Current abstraction exists for browser microphone, policy and local voice runtime.

Required completion:

- exact `whisper.cpp` local executable discovery/binding;
- exact Piper local executable/model binding;
- process lifecycle and health;
- input/output format constraints;
- timeout/cancellation;
- no secret leakage to transcripts/logs;
- incident correlation;
- fail-closed unavailable UX;
- local runtime verification on supported host environment.

No paid remote voice API fallback is introduced by this plan.

# 9. Incident persistence/runtime binding

Production incident persistence must preserve:

- exact customer/tenant/incident namespace;
- durable chronological event ordering;
- mixed text/voice continuity;
- screenshot provenance/security scan state;
- Support takeover events;
- reconnect/restart recovery;
- closure summary;
- prohibited-secret exclusion;
- authoritative-vs-human-reported truth classification.

Retention duration, deletion/export and privacy enforcement follow governing security/data contracts. Do not turn an unresolved policy into silent data loss or unlimited retention.

# 10. Browser/runtime binding

Before production completion, run real browser checks for:

- public surfaces;
- authenticated customer surfaces after FCR-0152 binding;
- authenticated Owner surfaces;
- Support takeover surfaces;
- Arabic RTL / English LTR;
- keyboard navigation/focus;
- responsive mobile/desktop;
- reduced motion;
- inaccessible/unavailable controls;
- browser console errors;
- reconnect/session-revocation behavior where applicable.

# 11. Deployment portability

Shared Web remains provider-neutral.

Hosting/CDN/WAF/compute/storage/observability vendors are deployment bindings, not product architecture identities.

```text
CAPABILITY != VENDOR
PROVIDER_SELECTED != DEPLOYMENT_AUTHORIZED
```

Production deployment is a separately governed final phase after source/runtime readiness.

# 12. FCR operating loop during execution

For every Work Package:

```text
FRESH FCR CHECK
→ IDENTIFY WAITING ON WEB ITEMS
→ HANDLE MATERIAL WEB OBLIGATION FIRST
→ EXECUTE CURRENT WP
→ IF EXTERNAL GAP FOUND: FCR / HANDOFF
→ CONTINUE NON-BLOCKED WEB WORK
```

A dependency in one integration area must not unnecessarily freeze unrelated Web work.
