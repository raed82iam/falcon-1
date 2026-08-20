# 00 — Governance, Scope and Non-Negotiables

**Status:** `MASTER_PLAN_CANDIDATE`  
**Applies to:** all Shared Falcon Web planning and implementation under this Master Plan.

## 1. Ownership boundary

Shared Falcon Web is a reusable Falcon Shared Application.

Use the governing classification:

```text
GENERIC + INTENTIONALLY REUSABLE ACROSS FALCON
→ SHARED WEB

PRIMARILY DOMAIN-SPECIFIC
→ OWNING APPLICATION
```

Therefore Web owns:

- public Falcon presentation;
- common account/application navigation UX;
- Web-local layout and presentation preferences;
- Web design system and accessibility;
- Web-owned presentation market-data path where separately governed;
- customer/Owner conversational surfaces;
- customer incident conversation UX and Web-owned interaction mechanics;
- Owner Command Center presentation/request transport;
- Web-local maintenance/development under the Owner-approved Web AI model;
- cross-workstream request routing through governed interfaces.

Web does not own:

- FSATS trading analysis, School/Strategy logic, Risk or Guardian business truth;
- broker execution or broker truth;
- FSAPMA operational-data truth;
- Foundation lifecycle/resource/security/Kill authority;
- identity, role or MFA truth unless and only to the extent an authoritative contract projects it to Web;
- another Application's business semantics.

## 2. Truth rules

All Web surfaces must preserve these distinctions:

```text
WEB_DISPLAY != SOURCE_TRUTH_OWNER
AI_EXPLANATION != APPLICATION_ANALYSIS_TRUTH
UI_CLICK != AUTHORIZATION
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
SIGNED_IN != SUBSCRIBED
SUBSCRIBED != ENTITLED
ENTITLED != ACTION_AUTHORIZED
PASS != OWNER_ACCEPTED
RESTARTED != RECOVERED
REPAIRED != TRUSTED
TESTED != RELEASED
NO_SOURCE_VALUE != ZERO
UNKNOWN != HEALTHY
STALE != CURRENT
SCREENSHOT_OBSERVED != BROKER_CONFIRMED_TRUTH
SIMULATOR_ESTIMATE != BROKER_TRUTH
```

If authoritative source truth is absent, Web must show unavailable/unknown/stale/partial as applicable. It must not synthesize a reassuring answer.

## 3. Cross-workstream request rule

A request entering through Web is routed by ownership, not by convenience.

```text
OWNER / USER INTENT
→ WEB UNDERSTANDS AND STRUCTURES
→ TARGET OWNER IDENTIFIED
→ GOVERNED REQUEST/HANDOFF
→ OWNING WORKSTREAM ACTS
→ AUTHORITATIVE RESULT RETURNS
→ WEB PRESENTS RESULT
```

Web may not modify Foundation or FSATS files merely because the Owner issued the request through Web.

## 4. FCR rule

The repository-wide FCR channel is the neutral coordination mechanism.

Current permitted `Waiting On` values are:

```text
FOUNDATION | APPLICATION | WEB | NONE
```

The Owner is not a workstream handoff target. If Web needs Owner clarification, responsibility remains on `WEB` until Web asks and consumes the decision.

Before substantive Web work and before every Owner-facing Web response, perform a fresh FCR check. Current Issue body/header controls current lifecycle state; comments are chronological audit history.

## 5. Security and secrets

Shared Web shall not place reusable secret bytes in ordinary UI state, chat, incident chronology, logs, screenshots, provider action text or ordinary cross-workstream payloads.

```text
CREDENTIAL_REFERENCE != SECRET
OWNER_ACTION_MESSAGE != CREDENTIAL
API_KEY_VALUE != CHAT_PAYLOAD
```

Secure secret entry/storage/transport must remain fail-closed until the exact governed mechanism exists.

## 6. Identity and protected surfaces

Protected user, Owner and Support surfaces require authoritative identity/session/role/capability evidence.

Route presence is presentation only.

```text
ROUTE_EXISTS != SESSION_VALID
SESSION_VALID != BUSINESS_AUTHORITY
PROJECT_OWNER_LABEL != AUTHORITATIVE_OWNER_IDENTITY
SUPPORT_UI_STATE != SUPPORT_AUTHORITY
```

Until the authoritative runtime is consumable and bound, protected surfaces remain fail-closed.

## 7. AI Kill and degraded continuity

Web may present AI target state and submit governed Owner emergency requests where the Foundation contract permits. Web is never the Kill authority.

```text
WEB_AI != KILL_AUTHORITY
WEB_CANNOT_SELF_AUTHORIZE_KILL
GLOBAL_AI_KILL != FALCON_SHUTDOWN
GLOBAL_AI_KILL -> FALCON_SAFE_CORE
```

The minimum emergency/degraded control surface must not depend on cooperation from the AI being targeted. Web AI unavailability must not automatically make the whole Web unavailable if trusted non-AI surfaces remain available.

## 8. Presentation market data vs FSATS operational data

```text
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
```

Web chart/display data is presentation-only. FSATS analysis obtains the data it needs through FSAPMA under the FSATS contracts.

## 9. Provider capacity rule

Preferred path:

```text
SUITABLE INDEPENDENT WEB SOURCE
→ WEB USES OWN GOVERNED SOURCE
→ NO FSAPMA QUOTA SHARING
```

The 50/50 rule applies only when Web has no suitable independent source and both sides truly share the same provider-enforced constrained quota dimension.

Unknown constrained capacity fails/degrades closed. Same provider name is not proof of the same quota pool.

## 10. Historical evidence rule

Historical Ideas and planning are preserved as source material. They are not silently edited to look current.

The Master Plan must reconcile historical ideas with later Owner decisions and current FCR state. If an old idea says a dependency is missing but it has since been implemented, the plan carries the current state while retaining the original idea in the coverage register.

## 11. Development and deployment are separate

The Owner has authorized full Shared Web implementation within the Web-owned subtree. That does not automatically authorize production activation.

Every runtime activation requiring identity, provider connectivity, credentials, Foundation routes, Application routes or deployment infrastructure must satisfy its own governed readiness and evidence.

## 12. Web quality principle

No UX convenience may weaken architecture, truth, security or authority boundaries.

At the same time, low-risk obvious UX details do not require constant Owner micro-decisions. Search, filters, reset, empty states, pagination/lazy loading, normal control placement and safe presentation-preference persistence should use best-practice defaults unless they alter business meaning, permissions, privacy, security, money, trading, retention, cross-workstream ownership or emergency semantics.
