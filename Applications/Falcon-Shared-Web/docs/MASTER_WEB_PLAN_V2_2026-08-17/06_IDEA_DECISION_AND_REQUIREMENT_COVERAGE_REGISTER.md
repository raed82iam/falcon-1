# 06 — Idea, Decision and Requirement Coverage Register

**Status:** `MASTER_PLAN_CANDIDATE / COVERAGE REGISTER`  
**Purpose:** prove that the Master Plan V2 incorporates the current written Web ideas and later controlling decisions without silently discarding historical intent.

# A. Current `docs/ Ideas/**` coverage

The current Ideas directory contains six core planning documents. All six are incorporated.

## Idea 01 — `01 - خطة العمل العامة.md`

### Intent preserved
- understand all Web responsibilities first;
- site map;
- truth/status language;
- Owner/Application conversational gateway;
- users/Super User/Owner UX;
- Falcon Control;
- emergency AI control;
- emergency mobile direction;
- shared Application hosting pattern;
- audit/activity;
- wireframes/visual design;
- Architecture/Consistency/Security/Red Team;
- Owner review;
- implementation planning;
- continue non-blocked work when an external dependency exists;
- low-risk obvious UX decisions do not require Owner micro-management.

### Master Plan mapping
- governance: `00_*`
- product/site map: `01_*`
- Owner gateway: `02_*`
- integration/FCR: `03_*`
- implementation sequence: `04_*`
- assurance/Owner acceptance: `05_*`

### Reconciliation
Historical planning said implementation would come later; full Web implementation was subsequently Owner-authorized. The plan preserves the lifecycle logic but uses current implementation authority.

---

## Idea 02 — `02 - خريطة الموقع والشاشات.md`

### Intent preserved
- Public Falcon Home;
- Falcon OS vs FSATS hierarchy;
- Sign In routing;
- Owner direct routing to Falcon Command Center;
- Create Account/Application selection;
- My Applications;
- App/System Switcher;
- parallel tabs without coupling;
- Owner Command Center sections;
- generic Application presentation pattern;
- explicit truth/status language;
- Graphite/Charcoal visual direction;
- unresolved runtime/identity/deployment items handled through owners/contracts.

### Master Plan mapping
- `01_PRODUCT_SITE_AND_EXPERIENCE_BLUEPRINT.md` sections 1-7, 19, 24-26;
- WP-03 through WP-07, WP-17.

### Reconciliation
Older missing identity/Kill dependencies are updated by current FCR states rather than repeated as permanently missing.

---

## Idea 03 — `03 - بوابة الحوار والموافقات.md`

### Intent preserved
- natural-language interaction;
- AI structures user intent;
- owning Application remains business-truth owner;
- progressive conversation;
- advisory vs execution separation;
- ordinary user vs Owner authority separation;
- Owner governance flow;
- sensitive-action confirmation;
- original wording / interpretation / target / evidence / lifecycle visibility;
- clear failure/uncertainty behavior;
- pattern reusable for future Falcon Applications.

### Master Plan mapping
- `01_*` sections 13-14 and 20;
- `02_*` Owner Gateway + customer LSA;
- WP-13 and WP-14;
- `05_*` AI adversarial testing.

### Reconciliation
The original broad AI concept is narrowed by the later Owner-approved Web MSA/single-LSA model. Web AI explains/routes; it does not absorb domain intelligence.

---

## Idea 04 — `04 - الطوارئ والـ Kill.md`

### Intent preserved
- Owner emergency visibility/control request path;
- independent mobile emergency direction;
- authoritative state only;
- AI containment/kill without relying on target cooperation;
- evidence preservation;
- safe continuity;
- APP-RSC/resource-state presentation where authoritative;
- recovery distinctions;
- no automatic revival.

### Master Plan mapping
- `00_*` sections 7;
- `01_*` sections 21-22;
- `03_*` FCR-0225/FCR-0076/FCR-0226;
- WP-19;
- `05_*` emergency adversarial tests.

### Reconciliation
The historical Idea document predates accepted Foundation Stage 13 Kill/Safe-Core implementation and later Web FCR-0225 source binding. Master Plan carries current contracts instead of the old `MISSING` snapshot.

---

## Idea 05 — `05 - الحسابات والاشتراكات والتنقل.md`

### Intent preserved
- one Falcon account, multiple Falcon systems/subscriptions;
- authoritative identity/role;
- Owner direct routing;
- My Applications;
- switcher;
- parallel tabs and exact target isolation;
- subscription management UX;
- FSATS advisory does not require user broker/API secret;
- Automated Trading credential separation;
- VIP trial = one month;
- seven-day expiry warning;
- Standard/VIP visibility/upgrade model;
- Standard chart + historical max-four-indicator direction;
- Falcon Schools VIP-only direction;
- Standard one active Strategy on one asset at a time direction;
- VIP multi-Strategy/multi-asset direction where Application permits;
- Owner account experience;
- unresolved price/payment/entitlement/security specifics remain governed.

### Master Plan mapping
- `01_*` sections 5-7, 14-15;
- WP-05 and WP-06;
- subscription/entitlement tests in `05_*`.

### Reconciliation
FCR-0081 is now closed for the advisory credential clarification scope. Exact future entitlement/billing runtime remains outside Web invention.

---

## Idea 06 — `06 - محادثة المستخدم مع AI.md`

### Intent preserved
- AI chat for every FSATS user experience;
- explanation/consultation/recommendation presentation;
- novice-first exploration;
- no broker/API secret for advisory chat;
- no execution authority from chat;
- accessible persistent entry point;
- progressive natural conversation;
- source truth/freshness attribution;
- User Chat vs Owner Gateway;
- multi-user isolation;
- degraded AI-unavailable UX;
- unresolved ordinary-chat long-term retention/privacy/model/provider details remain governed.

### Master Plan mapping
- `01_*` section 13;
- `02_*` single LSA;
- WP-14;
- `05_*` tenant/AI security tests.

### Reconciliation
Later Owner decision settles Incident Conversation persistence separately. Ordinary AI-chat retention remains distinct and must not inherit unlimited incident retention.

# B. Later Owner decisions / reconciliations incorporated

## `AI_CHAT_AND_INCIDENT_PERSISTENCE_RECONCILIATION_2026-08-16.md`

Preserved distinction:

```text
ORDINARY USER AI CHAT != CUSTOMER-FACING INCIDENT CONVERSATION
```

Master Plan: sections 13/16 and WP-14/WP-15.

## `OWNER_DECISIONS_INCIDENT_CONVERSATION_2026-08-16.md`

All major settled decisions incorporated:

- one persistent incident conversation;
- A-to-Z chronology;
- mixed text/voice;
- stress adaptation;
- broker connectivity explanation;
- customer manual-close guidance boundary;
- Support request/takeover;
- mid-step transfer choice;
- post-resolution Support choice;
- screenshot/description choice;
- mandatory closure summary;
- side-topic handling;
- voice behavior;
- 15-second Live Voice silence tolerance;
- ordinary voice no silence auto-stop.

Master Plan: `01_*` sections 16-18; WP-15/WP-16.

## `VOICE_IMPLEMENTATION_LOCAL_FREE_2026-08-16.md`

Preserved:

```text
WHISPER_CPP_LOCAL
PIPER_LOCAL
PAID_REMOTE_VOICE_API = PROHIBITED_FOR_CURRENT_IMPLEMENTATION
```

Master Plan: section 18; WP-16; runtime binding in `03_*`.

## `OWNER_DECISION_PROVIDER_QUOTA_SPLIT_2026-08-16.md`

Preserved:

- independent Web source first;
- 50/50 only exact shared constrained pool/dimension;
- unknown constrained limit fails/degrades closed;
- soft throttle below 50%; hard ceiling 50% when rule applies;
- plain URL does not receive artificial API quota;
- Web presentation data remains separate from FSATS operational data;
- request-driven advisory analysis.

Master Plan: `00_*` sections 8-9; `03_*` sections 6-7; WP-08/WP-11/WP-21.

## `TRADING_UI_BENCHMARK_AND_DESIGN_DIRECTION_2026-08-16.md`

Preserved synthesis:

- chart-first;
- customizable modular workspace;
- professional information density;
- AI adjacent to chart;
- Strategy/School subordinate to current analysis;
- positions/trades below primary workspace;
- persistent incident safety layer;
- desktop density + mobile coherent collapse.

Master Plan: `01_*` section 8; WP-07.

## `OWNER_DECISION_SHARED_VISUAL_ASSET_2026-08-17.md`

Preserved:

- canonical `falcon-shared-visual.jpg`;
- use for primary imagery when actually needed;
- no new bird illustration per page by default;
- no forced decoration;
- preserve 1:1 identity without distortion.

Master Plan: sections 3/24; WP-03/WP-04.

## `OWNER_DIRECTION_WEB_MSA_LSA_2026-08-17.md`

Preserved and elevated into dedicated Master Plan module:

- one Web MSA;
- one Customer Interaction & Support LSA;
- bounded Web self-maintenance;
- Owner-directed Web development only;
- no Web autonomous research for self-development;
- LSA support-only research;
- no LSA self-development;
- personalization != self-development;
- Owner request routing by owning workstream;
- Web presents FSATS development reports but does not own FSATS development.

Master Plan: `02_*`; WP-12 through WP-14, WP-18.

# C. Current implementation/architecture evidence incorporated

## `IMPLEMENTATION_PLAN.md`

Original WP themes are retained and expanded. Master Plan V2 replaces the old single-list execution view with phased WPs 00-26 while preserving public, auth, My Apps, FSATS workspace, chart, AI, portfolio, incidents, Owner Command Center, preferences, adapter binding, assurance, Owner acceptance and separately governed deployment.

## `IMPLEMENTATION_ARCHITECTURE.md`

Preserved:

- feature-oriented modules;
- ports/adapters;
- one-way dependency direction;
- network transport out of presentation modules;
- source-state classification;
- provider-neutral deployment;
- incremental decomposition instead of rewrite;
- quality gates.

Master Plan: `00_*`, `03_*`, WP-02.

## `FCR_RECONCILIATION_IMPLEMENTATION_CHECKPOINT_03.md`

Preserved current source direction for incidents, chart data, overlays, on-demand analysis, catalog, detailed AI, portfolio and fail-closed Foundation dependencies.

Master Plan: `03_*`, WP-08 through WP-11, WP-15, WP-20/WP-21.

## `RED_TEAM_ALL_WEB_2026-08-15.md` and `RED_TEAM_REMEDIATION_REVIEW_2026-08-15.md`

Preserved lessons and remediations:

- centralized output encoding;
- protected route guards;
- five-minute timer semantics;
- screenshot scanner fail-closed;
- strict portfolio/on-demand validators;
- Support authoritative capability separation;
- hostile-output tests;
- Preview vs Authoritative source separation;
- unavailable controls;
- deep freeze/validated projections;
- historical-precedence clarity.

Master Plan: `05_*` and relevant WPs.

## `BROWSER_RUNTIME_VERIFICATION_2026-08-16.md`

Preserved browser verification model and distinction between public checks executable without identity and authenticated checks blocked until authoritative identity runtime binding.

Master Plan: WP-03/WP-04/WP-20/WP-23 and `05_*`.

## `HANDOVER_NEXT_WEB_PAGE_2026-08-16.md`

Preserved continuity rules:

- continue, do not restart architecture;
- read source/rules/FCRs fresh;
- exact Web write boundary;
- source implementation advanced;
- old test PASS does not transfer to changed commit;
- no fake runtime route;
- public/user/Owner separation;
- current provider/incident/AI architecture direction.

Master Plan: README, all WPs.

# D. FCR themes incorporated

The Master Plan explicitly includes current execution relevance for:

- FCR-0095 incident/support/voice;
- FCR-0125 Web presentation market data;
- FCR-0152 identity/session/MFA;
- FCR-0169 Foundation operational projection;
- FCR-0173..0177 Web presentation WebSocket egress;
- FCR-0196..0200 full-market presentation destinations;
- FCR-0220 advisory/provider quota semantics;
- FCR-0076 recovery/release projection dependency;
- FCR-0077 emergency/Application planning boundary;
- FCR-0225 Owner AI emergency Web binding;
- FCR-0226 AI target registration/Kill relevance;
- FCR-0012/FCR-0030 awareness/FSA governance relevance;
- closed semantic scopes FCR-0126/0127/0128/0130/0133/0203.

This list is not permission to skip repository-wide FCR checks. New/current FCR state always controls.

# E. Items intentionally not frozen by this plan

The following remain explicitly open/governed rather than silently invented:

- final billing/payment provider and price;
- final entitlement/subscription runtime contract;
- final VIP naming/full feature list;
- ordinary AI-chat long-term retention/export/deletion/memory policy;
- exact Web MSA/LSA generic Foundation AI registration/FSA binding if not already supported;
- exact production deployment vendor binding;
- exact production identity provider connectivity after Stage 16 consuming contract;
- exact production Support/contact transport;
- exact secure secret-entry/storage runtime;
- exact provider connectivity activation authority;
- separately governed Falcon Emergency Control mobile implementation.

# F. Coverage decision

```text
CURRENT_IDEAS_FILES = 6
IDEAS_INCORPORATED = 6
MAJOR_LATER_OWNER_DECISIONS_INCORPORATED = YES
CURRENT_ARCHITECTURE_DIRECTION_INCORPORATED = YES
CURRENT_RED_TEAM_LESSONS_INCORPORATED = YES
CURRENT_MAJOR_WEB_FCR_DEPENDENCIES_INCORPORATED = YES
HISTORICAL_RECORDS_DELETED_OR_REWRITTEN = NO
```

Any future Owner decision or FCR that materially changes Web scope must update this register and the affected Master Plan module before the plan is called current again.
