# 04 — Master Implementation Work Packages

**Status:** `MASTER_PLAN_CANDIDATE / EXECUTION ORDER PROPOSED`  
**Principle:** continue from current implementation; do not rebuild already-good source from zero.

# Execution model

Each Work Package (WP) follows:

```text
FRESH SOURCE + FCR CHECK
→ ENTRY CRITERIA
→ WEB-OWNED IMPLEMENTATION
→ TESTS
→ ARCHITECTURE/SECURITY CHECK
→ WP RED TEAM
→ EVIDENCE
→ OWNER REVIEW WHEN SEMANTIC/MAJOR
→ CLOSE ONLY WITH REQUIRED AUTHORITY
```

Dependencies should block only the affected slice, not unrelated Web progress.

---

# PHASE A — Baseline, reconciliation and architecture lock

## WP-00 — Master Plan V2 reconciliation and Owner planning acceptance

### Goal
Establish this folder as the current Web execution plan while preserving all historical records.

### Includes
- reconcile all Ideas and later Owner decisions;
- reconcile current FCR statuses;
- identify outdated historical statuses without rewriting history;
- confirm Web MSA/LSA responsibility model;
- confirm implementation sequence and dependencies.

### Exit
- plan consistency review PASS;
- plan Red Team PASS;
- Owner approves/adjusts the Master Plan;
- if Owner adjusts it, rerun review before final acceptance.

### Authority
Planning/documentation only for this WP.

---

## WP-01 — Current source and contract baseline inventory

### Goal
Produce an exact implementation inventory of what already exists versus what is missing.

### Required inventory
- routes and surfaces;
- feature modules;
- ports/adapters;
- composition/state;
- incident stack;
- voice stack;
- security helpers;
- Web market-data/provider plan;
- Owner emergency path;
- tests;
- current browser verification state;
- all FCR bindings.

### Exit
One evidence-backed matrix:

```text
REQUIREMENT | CURRENT SOURCE | TEST | FCR/CONTRACT | GAP | NEXT OWNER
```

No feature is reimplemented if existing source already satisfies it correctly.

---

## WP-02 — Architecture decomposition and composition stabilization

### Goal
Finish migration from broad `app.js` composition into maintainable feature/port boundaries without changing business semantics.

### Preserve
- route registry;
- design-system safe presentation primitives;
- shell/composition boundaries;
- existing extracted public/user features;
- strict output encoding;
- preview vs authoritative source separation.

### Work
- extract remaining large user feature slices;
- extract Owner Command Center slices;
- specialize adapters by contract family;
- prevent presentation modules from direct network transport;
- keep dependency direction one-way.

### Exit
- architecture tests PASS;
- no direct cross-workstream internal imports;
- no direct provider calls in presentation features;
- no unsafe dynamic HTML bypass.

---

# PHASE B — Shared Web product foundation

## WP-03 — Design system, localization and accessibility foundation

### Goal
One reusable Falcon Web design system for public, user and Owner surfaces.

### Includes
- Graphite/Charcoal token system;
- status semantics and non-color cues;
- typography/readability;
- Arabic RTL and English LTR;
- language switch + safe preference persistence;
- focus states;
- skip links;
- reduced motion;
- responsive primitives;
- accessible cards/tables/tabs/dialogs/forms;
- canonical Falcon visual asset component.

### Exit
- design primitives tested;
- no feature-specific accessibility workaround where reusable primitive belongs;
- RTL/LTR baseline verified in browser.

---

## WP-04 — Public Falcon OS experience

### Goal
Finish public Falcon product surface.

### Includes
- Falcon OS Home;
- About/Applications discovery;
- correct Falcon OS vs FSATS hierarchy;
- FSATS public page;
- future systems shown truthfully as future/non-operational;
- Sign In/Create Account entry points;
- canonical shared visual use;
- legal/regulatory claim guardrails;
- responsive/public SEO-compatible structure where useful without changing authority.

### Exit
- public browser verification PASS;
- keyboard/RTL/LTR/mobile PASS;
- no sensitive operational truth exposed;
- no fake operational future Application links.

---

## WP-05 — Account, authentication and protected-route UX shell

### Goal
Complete the Web-owned UX around account/sign-in without inventing identity authority.

### Before FCR-0152 handoff
- fail-closed auth adapter;
- public sign-in/create-account UX;
- protected route denial;
- role-target routing logic tested against fixtures only.

### After FCR-0152 handoff
- exact identity/session/security-context adapter;
- Owner direct routing;
- customer tenant binding;
- Support role/capability binding;
- session rotation/revocation/logout UI behavior;
- authenticated browser verification.

### Exit
Exact runtime identity binding verified. No route uses a display label as authority.

---

## WP-06 — My Applications, subscription and tier UX

### Goal
Finish reusable Falcon account-to-system access experience.

### Includes
- My Applications;
- App/System switcher;
- multi-tab target isolation;
- subscribe/manage UX where authoritative contract exists;
- FSATS onboarding separation from Automated Trading;
- no broker/API secret for advisory-only use;
- VIP trial/Standard/VIP presentation direction preserved;
- locked feature/upgrade presentation;
- entitlement truth remains external.

### Product items that remain blocked from hard finalization until authoritative contract exists
- final plan names;
- price/payment;
- final entitlement owner/states;
- final migration/preset retention policy.

### Exit
Web presentation is ready to bind exact entitlement/subscription contract without redesign.

---

# PHASE C — FSATS customer workspace

## WP-07 — FSATS workspace and customizable layout

### Goal
Complete chart-first Trading Command Center shell.

### Includes
- desktop information-dense layout;
- mobile coherent vertical layout;
- draggable/resizable/hideable/restorable widgets;
- explicit Web-owned layout preference persistence;
- clean navigation to Markets, Portfolio, AI, Notifications, Settings;
- no demo/live ambiguity.

### Exit
- layout preference tests PASS;
- mobile/desktop browser PASS;
- no layout setting changes Trading business state.

---

## WP-08 — Web presentation market data and chart

### Goal
Complete chart/display data architecture and UI.

### Includes
- instrument discovery;
- snapshot/live presentation;
- historical bars/backfill;
- volume;
- broad-market presentation when governed;
- indicators;
- source/freshness/delay state;
- exact destination readiness integration;
- provider quota/throttle UX where needed.

### Dependencies
FCR-0125, 0173-0177, 0196-0200, 0220.

### Exit
- exact per-provider bindings verified;
- Web display data cannot flow into FSATS analysis;
- unknown quota state fails/degrades closed;
- connectivity activation remains separately authorized.

---

## WP-09 — Schools, Strategies, overlays and detailed analysis

### Goal
Finish Application-owned intelligence presentation.

### Includes
- dynamic catalog;
- School grouping from Application metadata;
- applicability/entitlement/activation separation;
- Trading overlays;
- detailed analysis;
- synthesis and disagreements;
- short summary + full details;
- no invented confidence/target;
- stale/partial/unsupported handling.

### Preserve closed semantic scopes
FCR-0126, 0127, 0128, 0130.

### Exit
All malformed-contract adversarial tests PASS and no Web business-logic reconstruction exists.

---

## WP-10 — Portfolio, positions, orders, activity and performance

### Goal
Complete customer financial/trading truth presentation.

### Includes
- broker-account scope;
- positions;
- order/activity lifecycle;
- unknown broker outcome;
- trade/activity correction/supersession;
- performance history;
- no null-to-zero conversion;
- responsive tables/cards;
- explicit source/freshness.

### Preserve
FCR-0133 semantics.

### Exit
Strict contract binding and negative tests PASS.

---

## WP-11 — Advisory Markets and Owner provider actions

### Goal
Finish request-driven advisory UX and separate Owner provider action surface.

### Includes
- advisory-market profiles generic across markets;
- Saudi initial profile without Saudi-only architecture;
- user-request-only analysis flow;
- no autonomous scanning/push in the current advisory mode;
- provider capacity/status presentation;
- Owner action-required cards;
- no plaintext API key entry in ordinary chat/state;
- secure credential control stays disabled until governed route exists.

### Exit
FCR-0220/0125 Web source/runtime obligations complete where dependencies permit.

---

# PHASE D — Web AI and customer interaction

## WP-12 — Web MSA/LSA architecture and cross-workstream contract reconciliation

### Goal
Materialize the Owner-approved Web awareness model without inventing generic Foundation/Application support.

### Includes
- logical Web MSA identity;
- logical single Customer Interaction & Support LSA identity;
- Web-owned responsibilities and forbidden responsibilities;
- maintenance authority envelope definition;
- Owner-directed development gate;
- no research-for-self-development policy;
- LSA support-only research policy;
- resource/health requirements;
- target registration/Kill binding requirements;
- FSA/governance interface applicability check;
- exact FCRs raised for any missing Foundation/Application generic capability.

### Exit
No unresolved cross-workstream capability is silently mocked as real. Required FCRs exist with correct owner.

---

## WP-13 — Owner Conversational Gateway and request router

### Goal
Make the Web AI the Owner's single natural-language Falcon entry point while preserving ownership boundaries.

### Includes
- Owner message capture;
- structured intent;
- target owner classification;
- compound-request splitting;
- correlation and lifecycle state;
- sensitive-scope confirmation;
- governed handoff transport;
- incoming result/evidence presentation;
- no false completion state.

### Example acceptance

```text
"Change Web logo" -> WEB
"Improve FSATS opportunity analysis" -> APPLICATION/FSATS
"Change Foundation lifecycle rule" -> FOUNDATION/GOVERNANCE PATH
```

### Exit
Routing tests prove Web never executes another workstream's source change.

---

## WP-14 — Customer AI conversation and explanation LSA

### Goal
Deliver the customer-facing conversational intelligence model.

### Includes
- novice-first explanation;
- progressive follow-ups;
- explain FSATS results without becoming analysis owner;
- language/detail/style adaptation;
- source freshness and uncertainty;
- multi-user isolation;
- ordinary chat vs Incident Conversation distinction;
- ordinary long-term memory/retention remains bounded by later exact policy;
- no self-development.

### Exit
- explanation fidelity tests;
- hostile prompt/output tests;
- tenant isolation tests;
- no hidden transition from advice to execution.

---

## WP-15 — Incident Conversation, Support and customer safety

### Goal
Complete the Web-owned customer incident experience.

### Includes
- one persistent conversation per incident;
- text + voice chronology;
- one-screenshot-at-a-time + governed scanner requirement;
- stress-adaptive guidance;
- 5-minute viewed/no-reply escalation semantics;
- customer manual-close guidance boundary;
- Support request queue;
- explicit takeover;
- Falcon silent customer-facing observer during takeover;
- behind-the-scenes LSA assistance only within permission;
- resolution-before-takeover choice;
- mandatory closure summary;
- restart/reconnect persistence.

### Dependencies
FCR-0095 + FCR-0152 + production persistence/scanner/Support transport.

### Exit
End-to-end incident runtime tests including restart/reconnect, Support takeover, scanner rejection and closure summary PASS.

---

## WP-16 — Local free voice runtime

### Goal
Complete voice without paid remote API dependency.

### Includes
- BrowserMicrophone;
- explicit permission/opt-in;
- Whisper.cpp local STT binding;
- Piper local TTS binding;
- ordinary voice explicit stop/send;
- Live Voice 15-second silence patience;
- cancellation/timeouts;
- incident correlation;
- local runtime health;
- fail-closed unavailable state.

### Exit
Real local executable test PASS on the supported deployment environment.

---

# PHASE E — Project Owner operations and governance

## WP-17 — Owner Command Center and Falcon OS operational projection

### Goal
Build the authoritative Owner system picture.

### Includes
- system Overview;
- Applications;
- Needs Your Attention;
- incidents summary;
- users/access presentation;
- provider actions;
- system health/readiness;
- resource state presentation;
- unavailable/stale/partial truth.

### Priority dependency
FCR-0169 Web binding.

### Exit
Foundation Stage 14 projection bound/governed-verified without repair/resource authority leakage.

---

## WP-18 — Owner approvals, development reports and audit evidence

### Goal
Give the Owner a clear review/decision center.

### Includes
- candidate/report inbox;
- source workstream/application identity;
- proposal reason;
- exact changed candidate identity;
- sandbox/validation evidence where supplied;
- Red-Team/security evidence;
- risks/limitations;
- FSA/governance review state when applicable;
- Owner approve/reject controls only against exact contract;
- audit history.

### FSATS self-development reporting
Web shall be ready to display the full governed FSATS development proposal/report when the owning Application contract materializes. Web does not invent that contract or call a report approved merely because validation passed.

### Exit
Owner decision is attributable, exact-candidate-bound and cannot be inferred from view/silence.

---

## WP-19 — Emergency AI control and degraded continuity

### Goal
Complete Owner emergency UX while preserving Foundation Kill authority.

### Includes
- exact registered AI target inventory presentation;
- targeted/global request separation;
- exact blast radius;
- Safe Core state;
- request/accepted/completed distinctions;
- no release/revival action from Web;
- AI-independent degraded presentation path;
- Web MSA/LSA target inclusion once governed;
- Stage 9 recovery/release projection when FCR-0076 Foundation handoff arrives.

### Exit
Adversarial tests prove unknown/ambiguous target fails closed with no scope widening.

---

# PHASE F — Production runtime completion

## WP-20 — Authoritative identity/session/MFA consuming-side binding

### Entry
FCR-0152 handed to `WEB` with exact Foundation evidence.

### Work
- bind exact contract;
- Owner/customer/Support routing;
- tenant isolation;
- session lifecycle;
- logout/revocation;
- authenticated browser verification;
- incident/support revalidation.

### Exit
FCR-0152 Web portion governed-verified.

---

## WP-21 — Provider/data runtime bindings

### Entry
Exact required Foundation route/principal/policy/credential-reference context available.

### Work
Close Web-owned binding/verification for FCR-0125, 0173-0177, 0196-0200, 0220.

### Exit
Per-destination governed verification complete. Connectivity activation still requires explicit deployment/runtime authority.

---

## WP-22 — Production incident persistence, scanner and Support/contact transport

### Work
- production tenant-scoped storage;
- retention/security policy binding;
- governed screenshot scanner;
- Support/contact transport;
- durable presence/read/delivery state where contract supports it;
- restart/recovery testing;
- local voice binding integration.

### Exit
FCR-0095 Web runtime obligations complete and governed-verified.

---

# PHASE G — Whole-Web assurance and Owner acceptance

## WP-23 — Full governed executable verification

Run against one exact commit:

- install/restore as applicable;
- `npm test`;
- `npm run check`;
- architecture boundary suite;
- security hostile-output suite;
- all contract/adversarial tests;
- browser public + authenticated tests;
- Arabic/English/RTL/LTR;
- accessibility;
- mobile/desktop;
- deterministic rerun where applicable;
- clean tree and exact candidate evidence.

No old PASS may be transferred to a newer commit.

---

## WP-24 — Full Web Red Team

Scope:

- architecture;
- truth/authority;
- identity/session;
- multi-tenant isolation;
- XSS/content injection;
- secrets;
- request routing;
- AI prompt/authority confusion;
- MSA/LSA maintenance/development boundaries;
- incident/support;
- voice;
- provider quota/route security;
- Kill/emergency;
- subscription/entitlement UX;
- accessibility deception/disabled controls;
- stale/unknown truth;
- demo/live separation;
- deployment readiness.

Any semantic remediation triggers fresh applicable verification and Red Team of the changed candidate.

---

## WP-25 — Final Owner acceptance freeze

Present:

- exact commit;
- complete scope delivered;
- remaining separately governed dependencies if any;
- all test evidence;
- Red-Team result;
- FCR state;
- production-readiness truth;
- screenshots/browser evidence;
- unresolved future items.

Only explicit Project Owner acceptance closes the Master Web implementation baseline.

---

## WP-26 — Production deployment / activation

**Separately governed.**

This WP begins only with explicit authority for:

- production hosting/deployment;
- identity provider connectivity;
- provider egress/connectivity;
- secrets/credential binding;
- Support/contact transports;
- observability/storage infrastructure;
- production DNS/TLS/WAF/CDN/compute bindings;
- rollback/recovery procedures.

Deployment PASS does not create Trading execution authority for FSATS.

---

# Workstream priority rule

When several WPs are available, prioritize:

1. current `Waiting On: WEB` FCR obligation that materially blocks truthful behavior;
2. Web-owned safety/security correctness;
3. contract/architecture foundations needed by multiple later features;
4. customer/Owner capability;
5. polish.

Do not prioritize visual polish over a known truth/identity/security blocker.
