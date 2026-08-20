# Falcon Shared Web Application — Next Page Handover

**Date:** 2026-08-16
**Repository:** `raed82iam/Falcon`
**Writable branch:** `web-development`
**Writable source scope:** `applications/shared/web/**` only
**Current Web checkpoint / source HEAD to continue from:** `9757cf112d4bd9f34d2803727d70f2ee69ff0e5b`
**Checkpoint message:** `web: test authoritative school grouping without invented applicability`

---

# 0. THIS IS A CONTINUATION, NOT A NEW DESIGN

The next page must treat this handover as a direct continuation of the current Falcon Shared Web Application workstream.

Do **not** restart architecture, do **not** redesign decisions already settled by the Project Owner, do **not** ask for implementation permission again, and do **not** reinterpret Falcon / FSATS / Foundation boundaries from memory.

Project Owner already authorized full Web implementation inside the Web-owned scope:

```text
ابدأ implementation كامل.
```

This authorizes implementation only inside:

```text
applications/shared/web/**
```

It does **not** authorize production deployment, provider activation, broker connectivity, secrets, Foundation/Application writes, or business/trading authority.

---

# 1. MANDATORY READING BEFORE ANY ANALYSIS, PROPOSAL, OR WRITE

Before responding with implementation analysis, planning, architecture suggestions, code changes, review results, or closure claims, the next page SHALL perform the following fresh read.

## 1.1 Read this handover fully

Read this file from first byte to last byte:

```text
applications/shared/web/docs/HANDOVER_NEXT_WEB_PAGE_2026-08-16.md
```

## 1.2 Read EVERY file under Shared Web fully

Read **every file under**:

```text
applications/shared/web/**
```

from beginning to end, including but not limited to:

- README files
- Workstream Rules
- planning documents
- Ideas
- architecture documents
- implementation documents
- Owner decision records
- provider/market-data reconciliation documents
- source code
- adapters
- presenters
- runtime ports
- composition
- navigation/routes
- security helpers
- incidents
- voice
- features
- CSS
- tests
- package scripts
- checkpoints
- prior handovers

Do not sample files and do not rely on this handover as a replacement for reading the real source.

## 1.3 Read Falcon governing references fresh

Read the current canonical Falcon Vision and Falcon Constitution, plus the current governing references referenced by the Web documentation and the previous handover.

Do not rely on memory for current governance.

## 1.4 READ FCR PROTOCOL AND EVERY FCR FULLY

This is mandatory.

Read GitHub Issue #1 fully:

```text
FCR Shared Registry and Operating Protocol
```

Then read **every current `[FCR-xxxx]` issue in the repository fully**, not only Web FCRs and not only issues currently `Waiting On: WEB`.

For every FCR:

1. read the **entire current Issue body**;
2. read **all comments from oldest to newest**;
3. treat Issue body as current canonical state;
4. treat comments as chronological audit trail;
5. inspect current `Waiting On` before acting;
6. do not assume an old handoff remains current.

This means Foundation/Application/Web FCRs must all be understood because cross-workstream handoffs can change while the Web page is active.

### Before EVERY Web response

Perform a fresh live FCR check.

Especially inspect every FCR currently:

```text
Waiting On: WEB
```

If a new FCR appears, handle it before continuing unrelated Web implementation where it materially changes current work.

Permitted `Waiting On` values are only:

```text
FOUNDATION
APPLICATION
WEB
NONE
```

`Waiting On: OWNER` is prohibited by Issue #1. If Web needs an Owner decision, the FCR remains `Waiting On: WEB` while Web asks the Owner directly.

---

# 2. REPOSITORY / AUTHORITY BOUNDARIES

Repository:

```text
raed82iam/Falcon
```

Writable branch for this workstream:

```text
web-development
```

Writable scope only:

```text
applications/shared/web/**
```

Do not write to:

```text
foundation-development
application-development
main
reference/fsats-v1.3-scratch
```

Do not modify anything outside `applications/shared/web/**` unless the Project Owner gives explicit authority.

GitHub Issues used for FCR coordination are neutral cross-workstream transport and may be commented/updated according to Issue #1 protocol.

Do not create accidental issues for tooling checks.

Historical accidental/no-op issues were created in prior tooling and immediately closed as not planned. They are **not project requirements** and must not be revived:

- #205 historical accidental tool issue
- #208 `[VOID] Accidental tool check - no action`
- #209 accidental temporary tool issue
- #223 accidental temporary tool issue

Use tool discovery instead of creating issues to test tool availability.

---

# 3. CURRENT SOURCE HEAD AND VERIFICATION TRUTH

The Project Owner supplied and this page verified that the continuation checkpoint exists:

```text
9757cf112d4bd9f34d2803727d70f2ee69ff0e5b
```

Commit message:

```text
web: test authoritative school grouping without invented applicability
```

This commit specifically adds an authoritative catalog test proving:

- strategies are grouped by **Application-supplied School metadata**;
- Web does not invent School applicability;
- `NOT_APPLICABLE` Strategy remains visible-disabled with reason;
- the authoritative path does not fall back to legacy catalog markup.

The source is significantly advanced.

**The one thing that must NOT be called complete is governed executable verification.**

Current truth:

```text
WEB_SOURCE_IMPLEMENTATION = ADVANCED
WEB_EXECUTABLE_GOVERNED_VERIFICATION = PENDING
npm test = NOT PROVEN PASS
npm run check = NOT PROVEN PASS
FCR closure eligibility = NO unless exact required verification later passes
production readiness = NO
```

The previous page attempted a fresh checkout to run:

```bash
npm test
npm run check
```

but the execution environment failed before checkout due DNS / GitHub access problems.

The page also tried alternate GitHub/archive access, but did not obtain executable bytes suitable for a trustworthy local Node test run.

Therefore:

```text
SOURCE IMPLEMENTED != EXECUTABLY VERIFIED
STATIC REVIEW != npm test PASS
STATIC REVIEW != npm run check PASS
```

Never retroactively claim PASS.

When a usable runner becomes available, run from exact `web-development` source:

```bash
cd applications/shared/web
npm test
npm run check
```

Then perform fresh browser/accessibility/keyboard/AR-EN verification and a fresh Red Team before any final closure claims.

---

# 4. CURRENT WEB ARCHITECTURE / UX DIRECTION

The UI is intentionally split into separate surfaces.

## 4.1 Customer/User surface

Customer FSATS workspace includes:

- Dashboard / Trading Command Center
- Markets
- Advisory Markets
- Portfolio
- Activity
- Falcon AI / analysis
- Notifications
- Settings
- Incident Conversation
- Support interaction inside the Incident Conversation

## 4.2 Owner / Support surface

Owner is **not** a trading user surface.

Owner/Support includes:

- System Overview
- Applications
- Incidents
- explicit Support Takeover
- Approvals
- Provider Actions
- Users
- Audit
- Settings
- Simulator/diagnostic presentation where permitted

Support takeover is incident-scoped and explicit.

Do not merge customer trading UI with Owner system controls.

## 4.3 Trading UI benchmark direction

Current design direction was deliberately synthesized from current professional trading UX patterns rather than copying one product:

- TradingView: chart-first hierarchy
- Robinhood Legend: clean modular workspace
- Webull: configurable/widget-oriented workspace
- Interactive Brokers: portfolio/activity/risk density
- Falcon-native differentiator: AI + Guardian + Incident/Support integration

Key document:

```text
applications/shared/web/docs/TRADING_UI_BENCHMARK_AND_DESIGN_DIRECTION_2026-08-16.md
```

The image shown to the Owner in chat was only a conceptual approximation, **not** an executable screenshot and not the final exact Owner/User layout.

---

# 5. IMPORTANT CURRENT FILE MAP

The next page must read all files, but the following are especially important continuation points.

## 5.1 Main composition/runtime entry

```text
applications/shared/web/src/app.js
```

Important current fact:

The app still has an explicit Preview data source path for development/demo. Do not confuse Preview with authoritative runtime truth.

The long-term composition boundary is through Web-owned runtime ports/adapters, not direct Application internals.

## 5.2 Runtime ports

```text
applications/shared/web/src/core/runtime-port.js
applications/shared/web/src/core/ports/fsats-runtime-port.js
applications/shared/web/src/core/ports/falcon-system-runtime-port.js
applications/shared/web/src/core/ports/web-market-data-port.js
```

`FsatsRuntimePortMethods` includes:

```text
portfolio
activity
chart
tradingOverlay
strategyCatalog
onDemandAnalysis
detailedAnalysis
incidents
```

`chart` remains an Application-provided compatibility surface only where applicable. Ordinary raw presentation market data is Web-owned under FCR-0125, subject to provider/FCR authority.

## 5.3 Authoritative FSATS composition

```text
applications/shared/web/src/composition/fsats-authoritative-data.js
```

This is the Web-owned contract-to-UI-model bridge.

It binds known public FSATS payload semantics without inventing a runtime transport route.

It is used to prevent the UI from consuming arbitrary raw objects and to preserve null/freshness/applicability truth.

## 5.4 Portfolio adapter

```text
applications/shared/web/src/adapters/fsats-portfolio-v1.js
```

Important semantics:

```text
UNSUPPORTED / NOT_APPLICABLE summary -> numeric business values NULL
UNSUPPORTED / NOT_APPLICABLE positions -> EMPTY
UNSUPPORTED / NOT_APPLICABLE activity -> EMPTY
UNSUPPORTED / NOT_APPLICABLE performance -> numeric NULL + history EMPTY
AVAILABLE authoritative zero = legal zero
NO_SOURCE_VALUE != ZERO
```

Do not convert null to `0`.

## 5.5 Analysis and Strategy adapters

```text
applications/shared/web/src/adapters/fsats-analysis-intent-v1.js
applications/shared/web/src/adapters/fsats-analysis-strategy-v1.js
```

Important reconciliation already done:

There were previously two inconsistent On-Demand result binding paths. The newer/current Web path was reconciled so `NEEDS_CLARIFICATION` does **not** claim a resolved instrument identity.

Mandatory analysis distinctions include:

```text
CUSTOMER_ANALYSIS_REQUEST != TRADING_UNIVERSE_ADMISSION
CUSTOMER_ANALYSIS_REQUEST != STRATEGY_ACTIVATION
ON_DEMAND_ANALYSIS != CAPITAL_RESERVATION
ON_DEMAND_ANALYSIS != ORDER_INTENT
ON_DEMAND_ANALYSIS != EXECUTION_AUTHORITY
SYNTHESIS != CONSENSUS
DISAGREEMENT != ERROR
MISSING_TARGET != INVENT_TARGET
MISSING_CONFIDENCE != INVENT_CONFIDENCE
STALE_SOURCE != CURRENT_SYNTHESIS
PARTIAL_INPUTS != COMPLETE_RESULT
```

## 5.6 Trading overlay presenter

```text
applications/shared/web/src/presenters/trading-overlay.js
```

Current rules:

```text
USER_SELECTED_OVERLAY != APPLICATION_CONFIRMED_APPLICABILITY
WEB_RENDER_ATTEMPT != STRATEGY_ACTIVATION
OVERLAY_APPLICABLE != TRADE_AUTHORIZED
DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH
MARKET_DATA_SERIES != SCHOOL_STRATEGY_PROJECTION
```

Unsupported/unknown overlay element types fail closed.

## 5.7 FSATS Trading Command Center

```text
applications/shared/web/src/features/fsats-workspace/fsats-workspace.js
applications/shared/web/src/trading-workspace.css
```

Recent work up to `9757...` strengthened:

- authoritative overlay display;
- Strategy catalog display;
- grouping Strategies under supplied School metadata;
- no invented School applicability;
- `NOT_APPLICABLE` items visible-disabled-with-reason;
- no fallback to legacy hard-coded product truth in authoritative path.

Relevant test:

```text
applications/shared/web/tests/fsats-workspace-overlay.test.mjs
```

## 5.8 AI surface

```text
applications/shared/web/src/features/ai/ai.js
applications/shared/web/tests/ai.test.mjs
```

Current AI surface:

- shows FSATS-supplied analysis only;
- preserves horizon/Strategy/School truth;
- renders material disagreements and unresolved conflicts;
- does not invent confidence/targets;
- bounded rendering replaced unsafe generic `JSON.stringify` of disagreement objects;
- operational request button remains **disabled** while no authorized runtime request transport exists.

This is intentional:

```text
UI_CLICK != REQUEST_SENT
REQUEST_SENT != ACTION_ACCEPTED
```

Do not enable a fake request path.

## 5.9 Advisory Markets, FCR-0220

```text
applications/shared/web/src/features/advisory-markets/advisory-markets.js
applications/shared/web/src/advisory-provider-surfaces.css
applications/shared/web/tests/advisory-provider-surfaces.test.mjs
```

Separate customer surface, currently shaped for generic advisory-market profiles.

Saudi market is the first current example, but Web must **not hard-code architecture to Saudi-only semantics**.

Current Saudi advisory mode:

```text
ADVISORY_ONLY
Daily / Weekly / Monthly
NO intraday
NO execution
NO advisory-position tracking
NO autonomous opportunity scanning/push
USER_REQUEST_ONLY analysis
NO analysis request -> NO FSAPMA analysis data fetch
```

## 5.10 Owner provider actions

```text
applications/shared/web/src/features/owner-provider-actions/owner-provider-actions.js
```

Owner-only provider action surface is intentionally separate from customer Advisory Markets.

If a free provider requires an API key:

- Web may display `ACTION_REQUIRED`;
- Web may show provider/status/reason metadata;
- plaintext key input is **not** sent through ordinary chat/business payload;
- secure credential entry remains disabled/fail-closed until a governed secure route exists.

Do not add a normal text field that makes the Owner paste an API key into chat or ordinary app state.

## 5.11 Market-data plan / quota policy

```text
applications/shared/web/src/core/market-data-plan.js
applications/shared/web/docs/OWNER_DECISION_PROVIDER_QUOTA_SPLIT_2026-08-16.md
```

The current policy is:

### Independent source first

```text
WEB_HAS_SUITABLE_INDEPENDENT_DATA_SOURCE = TRUE
-> WEB_USES_ITS_OWN_GOVERNED_SOURCE
-> NO_FSAPMA_QUOTA_SHARING_REQUIRED
```

### 50/50 is fallback only

Only if Web has no suitable independent source and Web + FSAPMA must share the **same real constrained quota pool**:

```text
WEB_QUOTA_POOL_ID == FSAPMA_QUOTA_POOL_ID
AND QUOTA_POOL_IS_CONSTRAINED = TRUE
-> WEB_MAX_SHARE = 50%
-> FSAPMA_MAX_SHARE = 50%
```

Important:

```text
50_50 != DEFAULT_PROVIDER_RULE
50_50_APPLIES_ONLY_TO_SHARED_POOL
SAME_PROVIDER != SAME_QUOTA_POOL
DIFFERENT_API_KEY != GUARANTEED_INDEPENDENT_QUOTA
UNKNOWN_QUOTA_SCOPE != INDEPENDENT_CAPACITY
```

If the constrained shared pool exists but the effective limit cannot be determined reliably, Web must fail/degrade closed rather than assume unlimited capacity.

The 50% is a hard maximum ceiling, not a target. Soft throttle/warning should occur below 50%.

### URL clarification

The Owner explicitly clarified:

```text
PLAIN_WEB_URL_WITHOUT_DOCUMENTED_SHARED_LIMIT != API_QUOTA
```

A plain presentation URL does **not** get an invented 50/50 quota just because it is a URL.

However:

```text
WEB_URL != UNRESTRICTED_ACCESS_AUTHORITY
```

Provider terms, attribution, access rules, traffic/session constraints, and exact egress authority still apply where documented.

API / WebSocket / account/session/burst limits are governed when they are actually constrained.

## 5.12 Incident / Support / Voice

Key files:

```text
applications/shared/web/src/features/incidents/customer-incident.js
applications/shared/web/src/incidents/incident-controller.js
applications/shared/web/src/incidents/incident-persistence.js
applications/shared/web/src/incidents/incident-timeline.js
applications/shared/web/src/incidents/incident-content-safety.js
applications/shared/web/src/incidents/screenshot-upload-controller.js
applications/shared/web/src/voice/voice-policy.js
applications/shared/web/src/voice/browser-microphone.js
applications/shared/web/src/voice/local-voice-runtime.js
applications/shared/web/src/voice/browser-local-voice-binding.js
applications/shared/web/src/voice/live-voice-session.js
applications/shared/web/src/voice/incident-voice-controller.js
applications/shared/web/src/adapters/fsats-incident-followup-v1.js
```

Owner decision record:

```text
applications/shared/web/docs/OWNER_DECISIONS_INCIDENT_CONVERSATION_2026-08-16.md
```

Voice implementation plan:

```text
applications/shared/web/docs/VOICE_IMPLEMENTATION_LOCAL_FREE_2026-08-16.md
```

### Voice provider decision

Current target is genuinely zero paid voice API cost:

- STT: local `whisper.cpp`
- TTS: local Piper Arabic
- paid remote voice API fallback is prohibited in current implementation
- browser microphone requires explicit user action/permission

Do not confuse open-source Whisper source/runtime with paid OpenAI API usage.

Exact executable whisper.cpp/Piper process binding remains an implementation/runtime concern and must remain fail-closed if unavailable.

### Owner voice behavior

```text
VOICE_MESSAGE_SILENCE_AUTO_STOP = DISABLED
LIVE_VOICE_GUIDANCE_SILENCE_TOLERANCE_BEFORE_FALCON_REPLY = 15_SECONDS
```

Ordinary voice messages do not auto-stop on silence. User explicitly stops/sends.

Live Voice waits through up to 15 seconds continuous silence after speech before ending the customer turn, unless customer explicitly ends earlier. If customer resumes during that interval, continue listening.

### Incident durability fixes already made

Red Team previously found and source-remediated important persistence issues:

- Support takeover must not change visible state before durable journal write;
- resolution/state mutation must not outpace durability;
- record + event persistence made atomic where applicable;
- event journal restores after refresh;
- screenshot artifact + event persisted atomically;
- voice audio + voice event + transcript event persisted atomically;
- avoid orphan media artifacts if event write fails.

Do not regress these properties.

---

# 6. OWNER INCIDENT / SUPPORT DECISIONS — DO NOT RE-ASK

The following are settled Owner decisions unless the Owner explicitly reopens them or a newer governing contract creates a real conflict.

## 6.1 Ownership

Application/Guardian owns business/trading incident semantics:

- affected positions/orders
- required customer information/action
- protection/priority state
- simulator/shadow semantics
- whether intervention is required

Web owns customer-facing interaction/communication.

## 6.2 Persistent Incident Conversation

One persistent Incident Conversation per customer-facing incident, covering A-to-Z chronology:

- Falcon/customer text
- voice/transcript/context
- mixed chronology
- guided steps
- customer responses
- permitted screenshots
- Support escalation/takeover/messages
- relevant state transitions
- resolution
- final summary

## 6.3 Under stress

Use short sentences, one step at a time, confirmation, reduced cognitive load, factual reassurance only.

## 6.4 Customer wants to close positions

If frightened customer wants to close positions:

- warn that rushed/incorrect close may realize/create losses;
- repeat warning sufficiently;
- if customer remains explicit/persistent, it is the customer decision;
- Web may guide broker UI step-by-step for self-action.

Mandatory distinction:

```text
CUSTOMER_DECISION != FALCON_TRADING_DECISION
GUIDANCE_TO_BROKER_UI != BROKER_EXECUTION
```

## 6.5 Support

- first authorized available Support;
- current Project Owner may serve as Support if no dedicated Support exists;
- if none available, tell customer and continue permitted guidance while request remains active;
- if Support becomes available mid-step, tell customer and offer finish-step-or-transfer-now;
- actual takeover must be explicit and visible;
- Support human identity must be clear;
- Falcon remains silent customer-facing observer during takeover;
- Support takeover does not create execution/portfolio authority;
- resolution before requested Support takeover does not silently cancel Support request.

Mandatory:

```text
ESCALATED_TO_SUPPORT != SUPPORT_TAKEOVER
SUPPORT_TAKEOVER != CUSTOMER_EXECUTION_AUTHORITY
SUPPORT_TAKEOVER != INCIDENT_RESOLVED
```

## 6.6 Screenshots

- screenshot or description choice without pressure;
- screenshots one at a time;
- no credentials/secrets;
- screenshot observation is not broker truth.

```text
SCREENSHOT_OBSERVED != BROKER_API_CONFIRMED
```

## 6.7 Closure summary

Web may render closure summary only from supplied authoritative data:

- what happened
- start/end timing
- affected positions/orders
- state
- FSTSimA/shadow
- simulator evidence explicitly labeled
- recovery action
- follow-up

Do not invent missing Application truth.

---

# 7. FCR-0201 — AFFECTED POSITION / ORDER / FSTSIMA

Current Application exact executable source referenced by FCR:

```text
bef4f6c516cdccb973044153be0b089ae2c1bfa9
```

Canonical Application contract:

```text
applications/FSATS/contracts/web/FSATS.WebIncidentAffectedPositionAndShadowMonitoringContracts.v1.md
```

The next page must read that current Application source/contract fresh from `application-development` when working on this binding.

## Affected position

Contract identities:

```text
FSATS.WebAffectedPositionFollowupProjection.v1
FSATS.WebAffectedPositionFollowupUpdate.v1
```

Protection states include:

```text
BROKER_CONFIRMED_PROTECTED
PROTECTION_UNKNOWN_OR_AMBIGUOUS
INTENTIONALLY_RETAINED_WITHOUT_CURRENT_BROKER_PROTECTION
UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION
RECONCILIATION_REQUIRED
NOT_APPLICABLE
```

Rules include:

```text
BROKER_CONFIRMED_PROTECTED -> CURRENT truth + CURRENT freshness
UNEXPECTEDLY_MISSING_OR_INCOMPLETE_PROTECTION -> REQUIRED follow-up
INTENTIONALLY... != UNEXPECTEDLY...
```

## Affected order

Contract identities:

```text
FSATS.WebAffectedOrderFollowupProjection.v1
FSATS.WebAffectedOrderFollowupUpdate.v1
```

Order states include:

```text
BROKER_CONFIRMED_WORKING
BROKER_CONFIRMED_REJECTED
BROKER_CONFIRMED_PARTIALLY_FILLED
BROKER_CONFIRMED_FILLED
BROKER_CONFIRMED_CANCELLED
OUTCOME_UNKNOWN_OR_AMBIGUOUS
RECONCILIATION_REQUIRED
```

Never invent `PositionId` from ambiguous execution.

## FSTSimA emergency shadow

Contracts:

```text
FSATS.WebEmergencyShadowMonitoringRequest.v1
FSATS.WebEmergencyShadowMonitoringProjection.v1
FSATS.WebEmergencyShadowMonitoringUpdate.v1
```

Top-level projection truth is diagnostic only:

```text
SIMULATOR
REPLAY
SYNTHETIC
TEST
```

Freshness independently:

```text
CURRENT
STALE
UNKNOWN
UNAVAILABLE
```

Mandatory:

```text
SIMULATOR_ESTIMATE != BROKER_TRUTH
SHADOW_PROJECTION_TRUTH != BROKER_TRUTH
CURRENT_SHADOW_FRESHNESS != CURRENT_BROKER_ACCOUNT_TRUTH
SHADOW_POSITION != CONFIRMED_LIVE_POSITION
SHADOW_MONITORING != EXECUTION_CONFIRMATION
```

Ambiguous execution scenarios can include:

```text
NOT_EXECUTED
PARTIALLY_EXECUTED
FULLY_EXECUTED
```

They are alternatives, not probabilities and not broker facts.

Reconnect chain remains Application-owned:

```text
RECONNECT != RECOVERED
RECONNECT != INCIDENT_RESOLVED
SHADOW_ENDED != BROKER_TRUTH_RECONCILED
```

---

# 8. FCR-0133 — PORTFOLIO

FCR-0133 is currently `Waiting On: WEB`.

Application public payload contract is materialized and executable verification on the Application side passed at its exact source.

Web work already includes strict adapter semantics and authoritative composition.

Do not invent runtime transport identity. The FCR explicitly says no public cross-Application transport route was materialized/authorized by that response.

The next page should continue binding UI to the Web-owned authoritative composition/port while keeping unavailable runtime fail-closed.

Never convert no-source null to financial zero.

---

# 9. FCR-0126 / 0128 — OVERLAYS AND DYNAMIC STRATEGY CATALOG

## FCR-0126

Contracts:

```text
FSATS.WebTradingOverlayRequest.v1
FSATS.WebTradingOverlayProjection.v1
FSATS.WebTradingOverlayUpdate.v1
```

Web presenter exists:

```text
src/presenters/trading-overlay.js
```

Recent workspace changes bind overlay presentation without treating render/application as activation or trading authority.

## FCR-0128

Contracts:

```text
FSATS.WebStrategyCatalogRequest.v1
FSATS.WebStrategyCatalogProjection.v1
FSATS.WebStrategyCatalogUpdate.v1
```

Owner-approved rule:

```text
CATALOG_PRESENT + NOT_APPLICABLE_TO_CURRENT_ASSET
-> VISIBLE_DISABLED_WITH_REASON
```

Current `9757...` test specifically confirms authoritative School grouping uses supplied School metadata and does **not** invent School applicability.

Do not hard-code the Falcon product Strategy/School catalog in authoritative mode.

---

# 10. FCR-0127 / 0130 — ON-DEMAND + DETAILED ANALYSIS

FCR-0127 and FCR-0130 are currently `Waiting On: WEB`.

Current Web source supports presentation binding and fail-closed request UI.

The operational request transport is intentionally not faked.

Until an exact authorized runtime route exists:

```text
ANALYSIS_REQUEST_UI = VISIBLE / EXPLAINED
OPERATIONAL_SEND = DISABLED
```

Do not make a button appear successful just because it was clicked.

Detailed AI must preserve disagreement and incomplete/stale inputs.

Do not flatten disagreement into a single confident answer.

---

# 11. FCR-0220 — ADVISORY MARKET + PROVIDER DISCOVERY

This FCR is new relative to the prior handover and remains `Waiting On: WEB`.

Read its entire Issue body and **all comments** fresh before working on it.

Current semantic package includes:

```text
CURRENT_RELEASE = PERSONAL_USE
CURRENT_WEB = PERSONAL_PRIVATE_NOT_PUBLIC
CURRENT_RELEASE_COMMERCIAL_PRODUCT = NO
FUTURE_COMMERCIALIZATION = SEPARATELY_GOVERNED
PERSONAL_USE != TERMS_BYPASS
```

Current advisory behavior:

```text
TRIGGER = USER_REQUEST_ONLY
BACKGROUND_MARKET_POLLING = DISABLED
CONTINUOUS_FSAPMA_ANALYSIS_API_CONSUMPTION = DISABLED
AUTONOMOUS_OPPORTUNITY_SCANNING_OR_PUSH = DISABLED
NO_ANALYSIS_REQUEST -> NO_FSAPMA_ANALYSIS_DATA_FETCH
```

Saudi advisory horizons currently:

```text
Daily
Weekly
Monthly
```

No intraday, execution, advisory-position tracking, or opportunity follow-up.

## Provider/source rule

Web presentation source priority:

1. suitable independent governed Web source first;
2. shared FSAPMA quota only as fallback if no suitable independent source exists and both sides truly share the same constrained upstream pool.

## Provider metadata

Web may show source/provider metadata as information only.

```text
PROVIDER_METADATA != CONNECTIVITY_AUTHORITY
URL_DISPLAY != EGRESS_AUTHORITY
```

If a free provider requires an API key, Owner sees `ACTION_REQUIRED`; secret bytes never go through ordinary chat/business payload.

## Quota policy

See Section 5.11 above and the Owner decision doc.

This policy applies to actual constrained resource pools, **not to a plain URL with no documented shared limit**.

---

# 12. FCR-0125 + FOUNDATION-HELD PROVIDER ROUTES

FCR-0125 is `Waiting On: WEB` for reconciliation/binding/verification.

Web presentation market data may use Web-owned provider routes **only when the exact route is governed and activated**.

All FSATS analysis/business truth still comes from FSATS, and FSATS gets operational data through FSAPMA.

Mandatory separation:

```text
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
SAME_PROVIDER != SAME_AUTHORITY
SAME_URL != SAME_AUTHORITY
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
USER_ANALYSIS_REQUEST -> FSATS
FSATS_ANALYSIS_DATA_ACQUISITION -> FSAPMA
WEB_CHART_DISPLAY != TRADING_UNIVERSE_ADMISSION
```

Several exact provider route FCRs remain Foundation-held for Stage 12 and are non-blocking for current source implementation but blocking for real provider activation.

Examples include FCR-0196 through FCR-0200 and earlier 0173..0177 route governance.

Do not activate them from Web.

Public endpoint does not equal unrestricted egress authority.

---

# 13. FCR-0076 — FOUNDATION STAGE 9

FCR-0076 is currently `Waiting On: WEB`.

Foundation Stage 9 is `ACCEPTED_AND_CLOSED` and Foundation portion is implemented/verified.

Web must consume the Stage 9 recovery/release/reintroduction boundary without assuming authority.

Mandatory:

```text
REPAIR_SUCCESS != RELEASE
RESTART != RECOVERY
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION
WEB_PRESENTATION != FOUNDATION_AUTHORITY
UI_CLICK != AUTHORIZATION
```

Existing Web adapter:

```text
applications/shared/web/src/adapters/foundation-stage9-recovery-release-v1.js
```

Important prior correction:

A previous attempt assumed a field like `systemOverview.stage9RecoveryRelease` without a published contract. That invented schema was removed.

Do **not** reintroduce an invented runtime field/route.

The adapter may validate a governed projection if one is supplied, but Web must not invent where Foundation publishes it.

If the exact Web-consumable Stage 9 runtime projection is still not published/authorized, report the residual gap accurately through FCR governance rather than inventing transport/schema.

---

# 14. AUTHENTICATION / IDENTITY / SECRETS

Real authoritative identity/session/MFA remains governed outside Web where applicable.

Do not invent authenticated session truth.

Do not treat phone/email/contact as authentication authority.

Credentials/secrets are prohibited in ordinary:

- chat
- incident conversation
- logs
- screenshots
- audio where unsafe
- business payloads
- source code

If secure credential infrastructure does not exist, fail closed.

---

# 15. GLOBAL TRUTH / AUTHORITY BOUNDARIES

Preserve these everywhere:

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
SIMULATOR_ESTIMATE != BROKER_TRUTH
SHADOW_MONITORING != EXECUTION_CONFIRMATION
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
SAME_PROVIDER != SAME_AUTHORITY
SAME_URL != SAME_AUTHORITY
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
USER_ANALYSIS_REQUEST -> FSATS
FSATS_ANALYSIS_DATA_ACQUISITION -> FSAPMA
FSATS_ANALYSIS_RESULT -> WEB
WEB_CHART_DISPLAY != TRADING_UNIVERSE_ADMISSION
```

---

# 16. RECENT IMPLEMENTATION SERIES UP TO CURRENT HEAD

Important recent checkpoints from this page include:

```text
73b27f47b1d8cd91c58dabbdfb2948e17e3f5ee3  web: add advisory market surface
771097a1abb1e5548ab8e8194e9036774fcd1854  web: add Owner provider action surface
158e803db42365b998104ffaf5ad4cca214aa26d  web: add advisory/owner routes
... navigation/composition/demo/style/check updates ...
129972cb45852e8125353420a067b0ab9b58f04b  Web market-data plan updated for independent-source-first / shared-pool fallback
... quota fallback tests ...
08ff235793b7f1722e5eecccec1a1a34b7ae2dc2  authoritative FSATS composition work
93ac574743edda05a0bb64e7f1381a3c1156a7f1  tests for authoritative composition
6c37cab53fd79aea3c805379ba585f4024c856fc  reconcile On-Demand analysis result path
c1078a665bd88ea44399a3a03df66b6b54748881  update tests to current clarification semantics
e1362cc315f272e70647aafc25f5282cf0798fad  bind AI surface to current analysis fields
cd5efbd00e9066df157fac95db0d053f8664da97  disable unauthorised analysis send UI
f7cd9337b5ac798320d69e1f7493cae9256f5be4  bounded disagreement/conflict presentation
7e5ea6ee26147ca046e7a09950748dc39a02d46e  AI test update
...
9757cf112d4bd9f34d2803727d70f2ee69ff0e5b  test authoritative school grouping without invented applicability
```

From `7e5...` to `9757...`, eight commits modified these continuation files:

```text
applications/shared/web/src/advisory-provider-surfaces.css
applications/shared/web/src/composition/fsats-authoritative-data.js
applications/shared/web/src/features/advisory-markets/advisory-markets.js
applications/shared/web/src/features/fsats-workspace/fsats-workspace.js
applications/shared/web/tests/advisory-provider-surfaces.test.mjs
applications/shared/web/tests/fsats-workspace-overlay.test.mjs
```

Read the exact commit history/diffs before further changes.

---

# 17. FCR COMMENTS / CHECKPOINTS ALREADY POSTED

Recent Web implementation checkpoints were posted to relevant FCRs.

Examples from the current page:

- FCR-0220 Web implementation checkpoint comment: `5307520744`
- FCR-0127 checkpoint: `5307544750`
- FCR-0128 checkpoint: `5307545406`
- FCR-0130 checkpoint: `5307546213`
- FCR-0133 checkpoint: `5307547036`

Earlier incident checkpoints include:

- FCR-0095 comment `5304562385`
- FCR-0201 comment `5304562860`

Do not use comment IDs as a substitute for reading current bodies and all later comments.

---

# 18. CURRENT `Waiting On: WEB` SET AT HANDOVER TIME

A fresh FCR check immediately before writing this handover returned the following important Web-owned open obligations:

```text
FCR-0076  Foundation Stage 9 recovery/release/reintroduction Web binding/verification
FCR-0095  Guardian customer notification/incident/support/voice interaction
FCR-0125  chart/presentation market-data separation and provider reconciliation
FCR-0126  School/Strategy chart overlay presentation
FCR-0127  on-demand asset analysis presentation
FCR-0128  dynamic School/Strategy catalog/selector
FCR-0130  detailed AI analysis / multi-strategy/school synthesis
FCR-0133  portfolio/positions/trades/performance authoritative adapter binding
FCR-0201  affected-position/order + FSTSimA incident projections
FCR-0220  advisory market onboarding/provider/quota/Owner-action presentation semantics
```

This list is only a snapshot.

The next page must do a fresh live FCR check because the set may change after this handover.

---

# 19. FOUNDATION-HELD PROVIDER / RUNTIME BLOCKERS

Several provider connectivity FCRs remain `Waiting On: FOUNDATION` for future Stage 12 governed external egress/security implementation.

Examples at handover include:

```text
FCR-0196
FCR-0197
FCR-0198
FCR-0199
FCR-0200
```

Earlier provider destinations 0173..0177 are also relevant under FCR-0125.

These are:

```text
NON_BLOCKING_FOR_CURRENT_WEB_SOURCE_IMPLEMENTATION
BLOCKING_FOR_REAL_PROVIDER_ACTIVATION
```

Do not cross-write Foundation or activate external routes from Web.

---

# 20. WHAT THE NEXT PAGE SHOULD DO FIRST AFTER READING EVERYTHING

After completing the mandatory full reads and fresh FCR check, continue from `9757...` in this order unless a newly updated FCR changes priority.

## Priority A — finish authoritative UI binding

Continue replacing Preview-only shortcuts with Web-owned authoritative composition/port consumption where public payload contracts are already known.

Do not invent runtime transport routes.

Focus especially on:

- Portfolio / positions / activity / performance
- dynamic Strategy/School catalog
- Trading overlays
- On-Demand analysis result presentation
- Detailed AI analysis
- Incident projections

## Priority B — FCR-0220 complete Web reconciliation

Finish generic advisory market profile presentation and provider metadata/action behavior.

Ensure:

```text
independent Web presentation source first
50/50 only shared constrained-pool fallback
plain URL without documented shared limit != API quota
user-request-only advisory analysis
no background FSAPMA analysis polling
no autonomous advisory opportunity feed
secure Owner credential path remains fail-closed
```

Do not hard-code Saudi market as the architecture.

## Priority C — FCR-0126 / 0128 source completion

The authoritative workspace now groups Strategies by supplied School metadata.

Continue validating overlay/catalog updates and lifecycle behavior without inventing Strategy/School truth.

## Priority D — Stage 9 FCR-0076

Use existing Web adapter only against real published projection semantics.

Do not invent Foundation runtime schema/field names.

If the exact Web-consumable runtime projection remains absent, record that accurately as residual dependency.

## Priority E — verification

As soon as a usable checkout/runner is possible:

```bash
npm test
npm run check
```

Then:

- browser smoke
- keyboard navigation
- accessibility
- Arabic RTL / English LTR
- user/Owner route separation
- incident voice behavior
- security/secret checks
- fresh Red Team against exact verified commit

Only after actual PASS evidence should FCR closure eligibility be reconsidered.

---

# 21. RED TEAM FINDINGS THAT MUST NOT REGRESS

Previous Red Team work already caught and remediated real issues. Re-test them.

## Incident persistence

Do not allow:

- in-memory takeover before durable event;
- resolution before journal durability;
- event saved without recoverable journal state;
- media artifact without associated event;
- refresh losing A-to-Z chronology.

## AI presentation

Do not expose arbitrary internal object metadata through generic serialization.

## Stage 9

Do not invent a runtime field or route merely to make the adapter appear connected.

## Provider quota

Do not assume:

```text
same provider = shared quota
multiple API keys = multiplied quota
URL = API quota
unknown quota = unlimited
```

## Preview data

Do not allow Preview data to masquerade as authoritative runtime truth.

---

# 22. COMMUNICATION RULES WITH THE OWNER

The Owner prefers direct status and dislikes repetitive questions.

Do not ask generic implementation permission again.

Do not re-ask settled Owner decisions unless:

1. Owner explicitly reopens them;
2. a newer governing contract conflicts;
3. implementation reveals a genuinely new unresolved Owner-level decision.

If a problem is inside Web scope and can be solved professionally without breaking governance, solve it and report.

If a decision belongs to Foundation or Application, use FCR governance rather than cross-write.

If an actual Owner-level product decision is required, ask clearly and specifically while keeping `Waiting On: WEB` where applicable.

Do not claim “finished” while governed executable verification is still missing.

---

# 23. FINAL CONTINUATION STATE

At handover:

```text
SOURCE HEAD = 9757cf112d4bd9f34d2803727d70f2ee69ff0e5b
WEB IMPLEMENTATION = ADVANCED / CONTINUE
DESIGN RESTART = NOT REQUIRED
OWNER IMPLEMENTATION AUTHORITY = ALREADY GRANTED WITHIN WEB SCOPE
FCR CHECK BEFORE EVERY RESPONSE = REQUIRED
READ ALL FCRs BODY + ALL COMMENTS = REQUIRED
READ ALL applications/shared/web/** FULLY = REQUIRED
PROVIDER ACTIVATION = NOT AUTHORIZED
PRODUCTION DEPLOYMENT = NOT AUTHORIZED
RUNTIME ROUTE INVENTION = PROHIBITED
SECRET IN CHAT/ORDINARY PAYLOAD = PROHIBITED
npm test = NOT YET PROVEN PASS
npm run check = NOT YET PROVEN PASS
GOVERNED EXECUTABLE WEB VERIFICATION = PENDING
FINAL FCR CLOSURE = NOT YET ELIGIBLE WHERE WEB VERIFICATION IS REQUIRED
```

The next page must continue implementation from this exact state, not start over.

---

# 24. FIRST MESSAGE EXPECTED FROM NEXT PAGE

The next page should not return a generic summary after reading.

After all mandatory reads and fresh FCR check, it should state concisely:

1. exact current `web-development` HEAD it verified;
2. whether any new/changed FCR now waits on Web;
3. exact next unfinished Web implementation slice from the current source;
4. then continue real implementation immediately unless a true external blocker exists.

Do not stop merely to ask the Owner what to do next when the next required Web work is already defined here and in current FCRs.
