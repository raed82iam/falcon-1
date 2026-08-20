# HANDOVER — Shared Falcon Web Application — Post Red Team

**Date:** `2026-08-15`  
**Repository:** `raed82iam/Falcon`  
**Writable branch:** `web-development`  
**Writable subtree:** `applications/shared/web/**`  
**Red Team report:** `applications/shared/web/docs/RED_TEAM_ALL_WEB_2026-08-15.md`  
**Red Team report commit:** `07eb5455344742b648cd3021f1992961699c2b2c`

This handover **supersedes prior Web handover assumptions where they conflict with the current FCR bodies, current Owner decisions, or the Red Team findings below**. Historical planning documents remain audit history and must not be treated as canonical over the live FCR header.

---

# 1. MANDATORY STARTUP FOR THE NEXT PAGE

Before analysis, design, implementation, review, or recommendation:

1. Read `applications/shared/web/WORKSTREAM_RULES.md` completely.
2. Read `applications/shared/web/README.md` completely.
3. Read this handover completely.
4. Read `RED_TEAM_ALL_WEB_2026-08-15.md` completely.
5. Read `FCR_RECONCILIATION_IMPLEMENTATION_CHECKPOINT_03.md` completely.
6. Read **every file under `applications/shared/web/**` completely**, including Ideas/planning/source/tests/checkpoints.
7. Read current Falcon Vision and Constitution.
8. Read current APP-001, CON-023, ADR-I012, ADR-I015.
9. Read Issue #1 FCR protocol fully.
10. Perform a **fresh repository-wide live FCR check**, not only Web-owned FCRs.
11. Before every Web response, recheck live FCR state, especially FCR-0095 and any FCR whose current `Waiting On` changes to WEB.

Live GitHub state wins over this handover.

---

# 2. AUTHORITY / WRITE BOUNDARY

Allowed source/document writes:

```text
branch: web-development
path:   applications/shared/web/**
```

Do not write Foundation/Application source or other branches.

GitHub FCR Issues are neutral coordination records. Comments/updates do not grant source authority outside Web.

Project Owner has already authorized complete Web-owned implementation inside the allowed subtree.

This does **not** grant:

- live deployment;
- external provider/broker connectivity;
- production OAuth/MFA/session issuance;
- Trading/business authority;
- secret-byte ownership;
- Foundation/Application source writes;
- FCR closure without implementation + binding + governed verification.

---

# 3. CURRENT LIVE FCR-0095 STATE AT RED TEAM START

At the Red Team live check:

```text
Status: APPLICATION_VERIFIED
Waiting On: WEB
Classification: CROSS_WORKSTREAM_SEMANTICS_DEFINED / WEB_IMPLEMENTATION_IN_PROGRESS
Blocking: BLOCKING_FOR_FCR_CLOSURE_UNTIL_WEB_IMPLEMENTATION_AND_VERIFICATION_COMPLETE
GOVERNED_VERIFICATION = PENDING
```

Current incident/support semantics include:

```text
BROKER_ACCOUNT_IDENTITY != CUSTOMER_IDENTITY
APPLICATION_ACTION_SEMANTICS != WEB_INTERACTION_DESIGN
UI_CLICK != BUSINESS_AUTHORIZATION
NOTIFICATION_DELIVERED != INCIDENT_RESOLVED
HUMAN_REPORTED_BROKER_STATE != BROKER_CONFIRMED_TRUTH
SCREENSHOT_OBSERVED != BROKER_API_CONFIRMED
SIMULATOR_ESTIMATE != BROKER_TRUTH
SUPPORT_TAKEOVER != PORTFOLIO_CONTROL
SUPPORT_MESSAGE != BUSINESS_AUTHORIZATION
```

Current Support decision:

```text
SUPPORT_INCIDENT_CHAT_ACCESS = VIEW_AND_EXPLICIT_TAKEOVER
SUPPORT_IN_CHAT_PARTICIPATION = ALLOWED_AFTER_EXPLICIT_TAKEOVER
FALCON_TO_SUPPORT_TAKEOVER = EXPLICIT_AND_VISIBLE
SUPPORT_IDENTITY_MUST_BE_CLEAR_TO_CUSTOMER = TRUE
SUPPORT_MUST_NOT_IMPERSONATE_FALCON_AI = TRUE
FALCON_DURING_SUPPORT_TAKEOVER = SILENT_OBSERVER
ESCALATED_TO_SUPPORT != SUPPORT_TAKEOVER
SUPPORT_TAKEOVER != INCIDENT_RESOLVED
```

Do not revert to old `OWNER_INCIDENT_CHAT = OBSERVE_ONLY` planning. That historical rule has been superseded by the current FCR/Owner Support-takeover decision.

---

# 4. CURRENT IMPLEMENTATION CHECKPOINT BEFORE RED TEAM

The prior reconciliation checkpoint was prepared from:

```text
58a1e502ce5c63abc40a17a954fdd95a89660892
```

Then the checkpoint document itself landed at:

```text
757c7d48fcda17b5b8dbfb886765fe71b1467273
```

Red Team report commit:

```text
07eb5455344742b648cd3021f1992961699c2b2c
```

The implementation currently includes:

- fail-closed auth adapter;
- Falcon public + FSATS public surfaces;
- My Applications;
- FSATS workspace;
- portfolio/positions/activity presentation;
- AI/detailed analysis presentation;
- notifications/incident surface;
- Owner Command Center / Support surface;
- Web-owned market-data presentation port separate from FSATS;
- FCR-bound market destination registry;
- portfolio v1 adapter;
- on-demand analysis intent adapter;
- Trading overlay presenter;
- localization and RTL/LTR direction;
- development/demo fixture separation labels;
- architecture and feature tests.

Do not treat presence of these components as production readiness.

---

# 5. RED TEAM RESULT — CURRENT SOURCE IS NOT READY FOR FCR CLOSURE

Overall decision:

```text
WEB_RED_TEAM = FAIL
WEB_SECURITY = FAIL
WEB_OWNER_ACCESS_PRESENTATION = FAIL
WEB_INCIDENT_FCR_0095_TIMER_CASE = FAIL
WEB_CONTRACT_VALIDATION = PARTIAL
WEB_EXTERNAL_EGRESS = CORRECTLY_FAIL_CLOSED
WEB_AUTHORITY_FABRICATION = NOT_FOUND
FULL_EXECUTABLE_GOVERNED_VERIFICATION = NOT_PROVEN
FCR_CLOSURE_ELIGIBILITY = NO
```

## Mandatory remediation order

### 1. RT-WEB-001 — CRITICAL — XSS/output encoding

Dynamic values are interpolated into HTML strings that eventually enter `root.innerHTML`.

Highest-risk current fields include:

- customer/support incident message text;
- outstanding incident action text;
- incident timeline fields;
- Owner incident/service text;
- other Application/catalog/analysis strings using the same composition style.

Required:

- central output encoding or safe DOM node/textContent strategy;
- hostile-input tests;
- no ad-hoc regex sanitizer.

**Do this first.**

### 2. RT-WEB-002 — HIGH — Owner route guard

`#owner` and related Owner routes are directly renderable from hash routing.

Required:

- fail-closed route policy;
- until FCR-0152 authoritative identity/session/MFA exists, Owner/Support content must not render as if the requester is an Owner/Support principal;
- route access never equals business authority.

### 3. RT-WEB-003 — HIGH — five-minute escalation semantics

Current `ownerDelayAlert()` suppresses escalation if `dismissedAt` exists.

This contradicts the current rule:

```text
ACTUAL_USER_VIEW + NO_REPLY -> 5 MINUTE ESCALATION
MINIMIZE/DISMISS != REPLY
MINIMIZE/DISMISS != ACKNOWLEDGEMENT
MINIMIZE/DISMISS != RESOLUTION
```

Fix implementation + tests before FCR-0095 closure.

### 4. RT-WEB-004 — HIGH — screenshot secret enforcement

Current `containsSecret` metadata is not sufficient to enforce the no-secret upload rule.

Keep screenshot transport fail-closed until content-level secret/redaction policy exists.

### 5. RT-WEB-005 — HIGH — portfolio v1 strict validation

Current adapter treats missing required nullable fields as null and does not fully validate pagination/history/update invariants.

Required: strict required-field presence + negative tests.

### 6. RT-WEB-006 — HIGH — Support takeover presentation helper must not become auth

`supportTakeoverAllowed()` is client presentation state only. Do not let future transport code use it as permission. Authoritative Support principal/session capability remains dependent on FCR-0152.

### 7. RT-WEB-010 — MEDIUM — on-demand analysis result validation

Completed/partial results must be validated strictly rather than silently accepting missing required identity/provenance fields.

### 8. Add dedicated security hostile-input tests

### 9. Separate production runtime composition from demo fixtures

### 10. Uniformly disable/label unavailable actions

### 11. Run governed executable verification

---

# 6. FCRs THE NEXT PAGE MUST TRACK

At minimum, recheck live:

## Waiting On WEB / Web completion family

- FCR-0095 — customer/incident/Support interaction
- FCR-0125 — chart/presentation market-data contract binding
- FCR-0126 — Trading School/Strategy overlays
- FCR-0127 — on-demand analysis
- FCR-0128 — Strategy/School catalog
- FCR-0130 — detailed AI analysis
- FCR-0133 — portfolio/positions/activity/performance

Do not assume their current Waiting On from this document; query live.

## Foundation-held Web dependencies

- FCR-0152 — authoritative identity/session/MFA
- FCR-0169 — authoritative Falcon OS operational projection
- FCR-0173 — Binance presentation-only WebSocket egress
- FCR-0174 — Coinbase presentation-only WebSocket egress
- FCR-0175 — Bybit presentation-only WebSocket egress
- FCR-0176 — Alpaca IEX presentation-only WebSocket egress
- FCR-0177 — Finnhub presentation-only WebSocket egress
- FCR-0196 — Alpaca US-equity universe REST egress
- FCR-0197 — Alpaca US-equity historical-bars REST egress
- FCR-0198 — Binance Crypto Spot universe REST egress
- FCR-0199 — Binance historical-klines REST egress
- FCR-0200 — Binance broad-market mini-ticker WebSocket egress

These are configuration/governance references only. No route is active merely because it is listed in Web source.

---

# 7. MARKET-DATA ARCHITECTURE THAT MUST NOT BE UNDONE

Current intended separation:

```text
Web presentation market data
    -> WebMarketDataPort
    -> display/search/chart only
    -> NOT FSATS operational input

FSATS operational market data
    -> FSAPMA governed path
    -> Trading/Application semantics
```

Mandatory:

```text
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
WEB_PROVIDER_CREDENTIAL != CUSTOMER_BROKER_CREDENTIAL
SAME_PROVIDER != SAME_AUTHORITY
UNIVERSE_VISIBLE_IN_WEB != TRADING_UNIVERSE_ADMITTED
```

Current raw Web provider routes remain Stage 12 blocked/fail-closed.

---

# 8. PORTFOLIO / ORDER TRUTH RULES TO PRESERVE

Application v1 contract is broker-account scoped.

Mandatory:

```text
NO_SOURCE_VALUE != ZERO
REQUESTED != ACCEPTED != PARTIALLY_FILLED != FILLED
UNKNOWN_BROKER_OUTCOME != FILLED
WEB_DISPLAY != EXECUTION_TRUTH_OWNER
WEB_DISPLAY != PORTFOLIO_TRUTH_OWNER
```

Do not reintroduce hard-coded `FILLED`, fake percentages, or inferred broker outcomes.

Current Red Team requires stronger required-field/pagination/history/update validation before FCR-0133 can be considered ready.

---

# 9. AI / ANALYSIS RULES TO PRESERVE

Web is presentation/request transport only.

```text
AI_CHAT_EXPLANATION != ANALYSIS_TRUTH_OWNER
CUSTOMER_ANALYSIS_REQUEST != EXECUTION_AUTHORITY
ON_DEMAND_ANALYSIS != UNIVERSE_MUTATION
DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH
CATALOG_AVAILABLE != STRATEGY_ACTIVATED
SIMULATOR_ESTIMATE != BROKER_TRUTH
```

Current customer UX decision remains:

- most likely/currently relevant scenario first;
- explain why the actual Application action was taken;
- show summary first;
- full details under `عرض التحليل الكامل`;
- for open protected positions, Application-owned plan may show TP, SL and current study;
- do not claim guaranteed safety/profit/recovery;
- Web never invents SL/TP/business rationale.

---

# 10. INCIDENT / SUPPORT UX DECISIONS ALREADY SETTLED

Preserve these unless a newer Owner decision changes them:

- serious incident uses prominent red treatment;
- unresolved minimized incident continues visible warning;
- multiple incidents can share one persistent conversation while preserving individual IDs/states;
- voice alert before opening is generic and contains no sensitive details;
- ordinary chat does not passively listen when closed;
- incident live voice requires explicit opt-in;
- mute is privacy mode;
- emergency general alert may still sound while muted without opening microphone;
- five-minute no-response escalation goes to **Support**;
- customer can return and continue Falcon conversation after escalation;
- Support may explicitly take over under clear human Support identity;
- during takeover Falcon is silent observer;
- Support does not impersonate Falcon;
- takeover is not portfolio control and is not incident resolution;
- customer-specific communication learning cannot auto-modify production behavior;
- self-development remains sandbox + governed approval.

Technical browser/OS background voice feasibility remains unverified and must not be promised.

---

# 11. SECURITY/TRUTH RULES THAT MUST SURVIVE REMEDIATION

```text
UI_CLICK != AUTHORIZATION
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
WEB_DISPLAY != BUSINESS_TRUTH_OWNER
NO_SOURCE_VALUE != ZERO
STALE != CURRENT
PARTIAL != COMPLETE
DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH
CATALOG_AVAILABLE != STRATEGY_ACTIVATED
CUSTOMER_ANALYSIS_REQUEST != EXECUTION_AUTHORITY
AI_CHAT_EXPLANATION != ANALYSIS_TRUTH_OWNER
NOTIFICATION_DELIVERED != INCIDENT_RESOLVED
SCREENSHOT_OBSERVED != BROKER_API_CONFIRMED
HUMAN_REPORTED_BROKER_STATE != BROKER_CONFIRMED_TRUTH
BROKER_ACCOUNT_IDENTITY != CUSTOMER_IDENTITY
```

Credentials never go into chat or reusable Web logs/state.

---

# 12. VERIFICATION STATUS

Do **not** claim full executable verification.

The current model-side environment has not produced trustworthy evidence for:

```text
npm test
npm run check
full security suite
accessibility audit
localization audit
full architecture review after remediation
production-profile/demo separation
```

The next page must implement Red Team fixes first, then run the strongest executable verification available. If full checkout/execution remains impossible, report that limitation explicitly and do not fabricate PASS evidence.

---

# 13. EXACT NEXT ACTION

The next Web page should **not restart product-design questioning**.

After mandatory reads/live FCR check, begin with:

```text
RT-WEB-001
Central safe-output encoding / DOM rendering boundary
+ hostile-input tests
```

Then perform a fresh mini Red Team of that fix before moving to RT-WEB-002.

After each material fix:

1. inspect actual diff;
2. run relevant tests if executable environment permits;
3. update Red Team status evidence;
4. recheck FCR-0095 live before responding;
5. do not close FCRs until all required bindings and governed verification are complete.

---

# 14. HANDOVER FINAL STATE

```text
SHARED_WEB_IMPLEMENTATION = ACTIVE
OWNER_IMPLEMENTATION_AUTHORITY = GRANTED_WITHIN_WEB_SCOPE
RED_TEAM = FAILED_WITH_ACTIONABLE_BLOCKERS
CRITICAL_BLOCKER = RT-WEB-001_XSS
HIGH_BLOCKERS = RT-WEB-002..006
FCR_0095 = OPEN / WAITING_ON_WEB_AT_RED_TEAM_CHECK
MARKET_EXTERNAL_EGRESS = FOUNDATION_STAGE12_FAIL_CLOSED
IDENTITY_SESSION_MFA = FOUNDATION_FAIL_CLOSED
PRODUCTION_DEPLOYMENT = NOT_AUTHORIZED
FULL_EXECUTABLE_VERIFICATION = NOT_PROVEN
NEXT_ACTION = REMEDIATE_RT_WEB_001
```
