# 01 — Product, Site and Experience Blueprint

**Status:** `MASTER_PLAN_CANDIDATE`  
**Goal:** define the complete Shared Falcon Web product surface before implementation completion and production binding.

# 1. Falcon product hierarchy

The public and authenticated Web must preserve Falcon's product hierarchy:

```text
FALCON OS
│
├── FSATS — Falcon Self-Aware Trading System
│   ├── FSATA
│   ├── FSAPMA
│   ├── FTGA
│   ├── FSTSimA
│   └── APP-RSC
│
├── FUTURE ACCOUNTING SYSTEM / FAMILY
├── FUTURE WAREHOUSE SYSTEM / FAMILY
└── OTHER FUTURE FALCON SYSTEMS
```

```text
FALCON_OS != FSATS
NON_TRADING_DOMAIN != FSATS_CHILD
```

Shared Web must be reusable for future Falcon systems rather than architecturally hard-coded to Trading.

# 2. Primary site areas

The Web experience is divided into four major classes of surface:

1. **Public Falcon experience**
2. **Regular authenticated Falcon user experience**
3. **Project Owner / Support experience**
4. **Degraded / emergency access experience**

These are not interchangeable.

```text
PUBLIC HOME != USER HOME != OWNER COMMAND CENTER != EMERGENCY CONTROL
```

# 3. Public Falcon Home

Purpose:

- explain Falcon OS clearly without exposing sensitive internal state;
- introduce current Falcon systems and future systems truthfully;
- present FSATS as the current Trading domain system with its internal Applications;
- provide `Sign In` and `Create Account` entry points;
- provide public product/discovery pages where useful;
- use the canonical Owner-approved Falcon visual asset when a primary visual is appropriate;
- avoid unapproved regulatory/licensing claims;
- avoid presenting public marketing content as live operational truth.

Target qualities:

- premium, calm, modern and trustworthy;
- not visually confused with a Trading terminal;
- bilingual Arabic/English;
- responsive and accessible;
- fast-loading and dependency-light where practical.

# 4. Sign In and role routing

After authoritative authentication and role resolution:

```text
SIGN IN
  ↓
AUTHORITATIVE FALCON IDENTITY / SESSION / ROLE
  ↓
PROJECT OWNER?
  ├─ YES → FALCON COMMAND CENTER
  └─ NO  → MY APPLICATIONS
```

The Owner does not land on the regular user home first.

Protected route hashes must remain fail-closed without authoritative session evidence.

# 5. One Falcon Account, multiple Falcon systems

Product direction:

```text
ONE FALCON ACCOUNT
→ MULTIPLE FALCON SYSTEM / APPLICATION RELATIONSHIPS
```

But maintain strict distinction:

```text
FALCON_ACCOUNT
!= SUBSCRIPTION
!= ENTITLEMENT
!= ACTION_AUTHORIZATION
```

Create Account flow candidate:

```text
PUBLIC HOME
→ CREATE FALCON ACCOUNT
→ CHOOSE AVAILABLE TOP-LEVEL FALCON SYSTEMS
→ SYSTEM-SPECIFIC ONBOARDING WHEN REQUIRED
→ MY APPLICATIONS
```

FSATS internal Applications are not automatically five separate top-level subscriptions merely because they are separate Applications architecturally.

# 6. My Applications / Falcon user home

The regular user home should:

- show subscribed/accessible top-level Falcon systems;
- distinguish current access, pending access, unavailable and future systems;
- allow opening a system in the current tab or new tab/window;
- show available systems that can be learned about or subscribed to when permitted;
- expose account-level settings appropriate to Web;
- preserve authoritative subscription/entitlement state rather than guessing.

Parallel tabs are allowed as UX, but requests/results remain explicitly targeted.

```text
MULTIPLE_OPEN_TABS != SHARED_BUSINESS_STATE
```

# 7. App/System switcher

Every authenticated system experience should have a consistent Falcon-level switcher for top-level systems the user may access.

The switcher is navigation, not authorization.

It should support:

- current system indication;
- quick switch;
- open in new tab;
- return to Falcon user home;
- manage subscriptions/applications when available.

# 8. FSATS regular-user workspace

The FSATS workspace should synthesize professional trading-platform lessons without copying another product.

Core direction:

1. chart-first visual center;
2. important account/portfolio context visible without hunting;
3. Falcon AI short explanation adjacent to analysis/chart context;
4. School/Strategy catalog nearby but subordinate to current analysis;
5. positions/trades/activity below or adjacent based on screen size;
6. persistent high-salience Incident/Guardian notifications when required;
7. compact navigation to preserve analytical screen area;
8. draggable/resizable/hideable/restorable widgets as Web-owned preference behavior;
9. desktop can be information-dense;
10. mobile collapses to a coherent single-column journey rather than miniature desktop;
11. Falcon AI + Guardian + Incident/Support is a Falcon-native differentiator.

Target FSATS surface family includes:

- Overview / Trading Command Center;
- Explore / Learn;
- Markets;
- Advisory Markets;
- Chart / Analysis workspace;
- Portfolio;
- Positions;
- Activity / Orders / Trades;
- Performance presentation;
- Falcon AI / Chat;
- Notifications;
- Incident Conversation;
- Automated Trading entry/onboarding when separately permitted;
- Settings.

# 9. Chart and Web presentation market data

Ordinary raw/live chart display data follows the Web-owned presentation-data path, not the FSAPMA operational analysis path.

Web chart experience should support, when governed sources exist:

- instrument search/discovery;
- selected-symbol live presentation;
- historical bars/backfill;
- volume;
- broad-market discovery where available;
- technical indicators;
- School/Strategy overlays supplied by FSATS;
- explicit stale/partial/unavailable truth;
- source/latency metadata where useful to the customer.

For Standard users, the historical product direction includes up to four simultaneously active ordinary technical indicators, subject to final authoritative entitlement implementation.

# 10. School and Strategy presentation

Shared Web presents authoritative Trading catalog/overlay semantics and does not recreate Trading logic.

Rules:

```text
CATALOG_VISIBLE != APPLICABLE
APPLICABLE != ENTITLED
ENTITLED != ACTIVATED
DISPLAYED_OVERLAY != TRADING_TRUTH_OWNER
```

Not-applicable entries may remain visible-disabled with authoritative reason.

School grouping uses Application-supplied School metadata. Web must not invent School applicability.

# 11. Advisory Markets

Current advisory-market direction is request-driven.

```text
USER REQUEST
→ WEB
→ FSATS ON-DEMAND ANALYSIS REQUEST
→ FSATS DETERMINES DATA NEED
→ FSAPMA FETCHES REQUIRED DATA
→ FSATS ANALYZES
→ WEB PRESENTS RESULT
```

No user request means no FSAPMA analysis-data fetch for this advisory mode.

Do not architect the surface as Saudi-only even if Saudi is the first active example.

# 12. Portfolio, positions, activity and performance

Web presents Application-supplied truth with exact account scope and truth metadata.

Mandatory UX behavior:

- missing numeric truth stays unknown/null, not zero;
- unknown broker outcome stays unknown;
- correction/supersession lineage is not flattened;
- performance history does not become Web-calculated business truth unless a separately governed Web calculation is explicitly authorized;
- customer-visible labels explain uncertainty instead of hiding it.

# 13. Falcon AI for regular users

The regular user should be able to interact conversationally even with zero Trading knowledge.

Experience goals:

- explain what a stock/market/indicator means in plain language;
- guide a curious beginner toward understanding without forcing Automated Trading;
- accept natural questions rather than API/query syntax;
- explain FSATS analysis, reasons, risks, uncertainty and limitations;
- support follow-up questions in context;
- allow progressive depth: simple first, more detail on request;
- offer useful next questions when the user does not know what to ask;
- render tables/cards/charts inside conversation when useful;
- preserve data freshness and source attribution;
- distinguish education, information, analysis, recommendation and execution.

```text
SIMPLIFY_PRESENTATION != REMOVE_RISK_OR_UNCERTAINTY
AI_RESPONSE != TRADE_ORDER
RECOMMENDATION != EXECUTION_AUTHORITY
```

The exact Web AI responsibility model is governed by `02_WEB_AI_MSA_LSA_AND_OWNER_GATEWAY.md`.

# 14. Advisory vs Automated Trading

FSATS subscription/advisory use does not require the customer's broker/API credentials merely to ask, learn, analyze, compare or receive recommendations.

User broker/API secrets become relevant only when the user separately chooses an automated-execution capability that actually requires them.

The transition from advisory chat to Automated Trading must be explicit, separate and unmistakable.

# 15. Subscription / Standard / VIP product direction

Historical Owner product direction to preserve in planning:

```text
NEW FSATS USER
→ 1 MONTH VIP TRIAL
→ 7 DAYS BEFORE END: WARNING + COMPARE + SUBSCRIBE
→ IF NO SUBSCRIPTION: STANDARD LIMITS
```

Current planned Standard experience includes, subject to authoritative entitlement/business-contract completion:

- useful baseline chart experience;
- ordinary indicators, with historical direction of max four active at once;
- no new saved premium Presets/Layouts after Standard transition;
- premium features remain visible but locked with a clear upgrade path;
- Falcon Schools are VIP-only after trial under the historical Owner direction;
- Standard active Strategy direction: one Strategy at a time on one asset at a time globally for that user, not one per browser tab.

VIP direction includes, subject to final authoritative entitlement contract:

- Falcon Schools;
- multiple School/Strategy combinations;
- multiple assets where Application capability permits;
- saved/default Presets/Layouts;
- other future approved premium capabilities.

Do not hard-code price, payment mechanism, final plan name or final entitlement owner until governed.

```text
VISIBLE_PREMIUM_FEATURE != ENTITLED
UPGRADE_PROMPT != AUTOMATIC_UPGRADE
SUBSCRIPTION_TIER != ACTION_AUTHORIZATION
```

# 16. Notifications and Incident Conversation

Customer-facing incidents use one persistent incident conversation per customer-facing incident.

The chronology may contain, as applicable:

- Falcon/customer text;
- voice and transcripts/context;
- guided steps and replies;
- approved screenshot interactions;
- Support escalation/takeover events;
- Support/customer messages;
- relevant customer-facing state changes;
- resolution communication;
- mandatory closure summary.

Secrets and reusable credentials are excluded.

## Incident communication under stress

When a customer appears stressed/unfocused, the interaction should adapt:

- shorter sentences;
- one step at a time;
- simple choices;
- confirmation after each step;
- reduced cognitive load;
- truthful reassurance only from authoritative facts.

## Customer wants to close positions manually

Web AI may explain risk and guide the customer through broker UI steps if the customer remains explicit that they personally want to act. Guidance is not broker execution and is not Falcon's Trading decision.

## Screenshot choice

Offer description or one screenshot at a time. Screenshot content requires governed security scanning. Screenshot observation is not broker truth.

## Incident closure

Every completed customer-facing incident must have a concise customer-readable closure summary based on supplied authoritative information, including timing, affected items, simulator/shadow evidence labels, corrective/recovery action and residual follow-up where available.

# 17. Support takeover

Support transfer stays inside the same Incident Conversation.

If Support becomes available during a useful Falcon-guided step, the customer can choose to finish the current step first or transfer immediately.

Actual takeover is explicit and visible.

During takeover:

```text
FALCON_CUSTOMER_FACING_OUTPUT = SILENT
SUPPORT_IDENTITY = VISIBLE
SUPPORT_TAKEOVER != TRADING_AUTHORITY
```

Falcon may remain available behind the scenes for governed context/research assistance but must not compete with the human Support voice in the customer-facing thread.

If the incident resolves before requested takeover, ask whether the customer still wants Support to explain what happened and the fix.

# 18. Voice

Current Owner-approved implementation direction:

```text
SPEECH_TO_TEXT = WHISPER_CPP_LOCAL
TEXT_TO_SPEECH = PIPER_LOCAL
PAID_REMOTE_VOICE_API = PROHIBITED_FOR_CURRENT_IMPLEMENTATION
```

Rules:

- microphone only after explicit customer action and browser permission;
- ordinary voice message does not stop/send on silence;
- customer explicitly ends/sends ordinary recording;
- Live Voice requires explicit opt-in;
- Live Voice waits through up to 15 seconds of continuous silence before replying unless customer ends turn earlier;
- voice and text remain part of the same incident chronology when both occur;
- exact local executable binding remains fail-closed until governed and verified.

# 19. Owner Command Center

The Owner surface is system-control/governance oriented, not a regular Trading-user dashboard.

Primary sections:

- **Overview**: authoritative Falcon OS operational picture and attention summary;
- **Applications**: Falcon systems/Application state and navigation;
- **Needs Your Attention**: decisions/reviews requiring Owner action;
- **Ask Falcon / Owner Gateway**: natural-language command/review interface;
- **Approvals**: explicit candidate/report review and Owner decisions;
- **Incidents**: active/closed incidents, escalation and Support takeover where authorized;
- **Provider Actions**: governed provider requirements/action-needed state without plaintext secret handling;
- **Users**: identity/access presentation under authoritative contracts;
- **Falcon Control**: governed request surfaces only where real authority path exists;
- **Emergency AI Control**: target-specific/global Kill request presentation with exact blast radius and Safe Core semantics;
- **Audit / Activity**: what was requested, interpreted, routed, accepted/rejected/completed and evidenced;
- **Settings**: Web-owned settings only;
- **Simulator/Diagnostics presentation** where permitted.

Primary Owner information flow:

```text
STATUS → ATTENTION → DECISION / REQUEST → AUTHORITATIVE OUTCOME → EVIDENCE
```

# 20. Owner conversational gateway

The Owner should not need to manually speak to every internal Falcon Application/awareness entity.

The Web AI is the human-facing gateway and routes Owner intent to the owning scope.

The full model is defined in `02_WEB_AI_MSA_LSA_AND_OWNER_GATEWAY.md`.

# 21. Emergency / degraded Web

The regular intelligent Web experience and the emergency minimum path are different concerns.

Target behavior:

- if Web AI is unavailable but trusted deterministic Web functions remain, expose truthful degraded UX;
- if an AI is contained/killed, preserve non-AI emergency presentation where authorized;
- exact target, scope, blast radius and authoritative outcome must remain visible;
- no automatic AI revival from Web;
- recovery/revival presentation must distinguish repair, trust, release and running state.

# 22. Falcon Emergency Control mobile direction

Historical idea retained as a future separately governed product surface:

- minimal emergency status only;
- independent from Shared Web as far as practical;
- can submit an authenticated Owner emergency request through the independent control path when materialized;
- can show authoritative acceptance/rejection/containment outcome;
- does not own Kill authority;
- does not revive AI.

This is not part of ordinary Web production activation unless separately authorized.

# 23. Activity and audit experience

Owner and authorized users should be able to understand, where permitted:

- original request;
- AI interpretation/structured intent;
- target owner/Application;
- delivery state;
- decision/authorization state;
- execution/completion state where applicable;
- authoritative result;
- evidence identity/provenance;
- corrections/supersession;
- timestamps and freshness.

Audit presentation must not invent causal evidence that the source does not provide.

# 24. Visual system

Direction:

- Graphite/Charcoal base;
- calm, high-contrast and readable;
- color used strongly for state/attention rather than decoration;
- one canonical Falcon shared visual asset for hero/primary branded illustrations when imagery is actually needed;
- no unnecessary decorative Falcon image on every page;
- responsive crop/scale without distorting the 1:1 source identity;
- trading workspace chart-first;
- public home visibly distinct from trading workspace;
- Owner Command Center visibly distinct from regular user trading workspace.

# 25. Accessibility, localization and responsive behavior

All major surfaces must support:

- Arabic RTL and English LTR;
- correct `lang` + `dir` switching together;
- persisted non-sensitive language preference;
- keyboard navigation;
- visible focus;
- skip-link behavior;
- reduced-motion handling;
- screen-reader friendly labels/states;
- no color-only status communication;
- usable mobile layouts;
- desktop information density without keyboard traps or overflow dependence.

# 26. Open product/business items that remain governed elsewhere

Do not fabricate:

- final billing/payment implementation;
- final VIP plan name/price/full feature list;
- final subscription/entitlement contract owner and runtime;
- final account recovery/security policies;
- final ordinary AI-chat long-term retention/deletion/export rules;
- final production hosting/provider choice;
- exact external connectivity before its governed binding;
- exact business semantics owned by FSATS/Foundation/other future systems.
