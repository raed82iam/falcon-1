# 09 — WP-01 Current Source and Contract Baseline Inventory

**Date:** 2026-08-17  
**Branch:** `web-development`  
**Writable scope:** `applications/shared/web/**`  
**Inventory input HEAD:** `3def0242bcc52d42c42352daad5f50ae3d6bdbb5`  
**WP status:** `WP01_COMPLETE / DOCUMENTARY_BASELINE_ESTABLISHED / FRESH_EXECUTABLE_RECHECK_REQUIRED_BEFORE_SOURCE-CHANGE_ACCEPTANCE`

## 1. Purpose

This record completes Master Plan V2 WP-01 by establishing an evidence-backed inventory of the current Shared Web source, current FCR ownership, verified semantic contracts, remaining runtime bindings, and newly accepted Owner requirements.

WP-01 is an inventory/reconciliation package. It does not activate connectivity, production authentication, deployment, Trading execution, Kill authority, secret storage, or another workstream's implementation authority.

## 2. Classification vocabulary

- `EXISTING_VERIFIED_SEMANTICS`: source/contract behavior exists and has governed verification evidence for the cited semantic scope.
- `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK`: current source exists, but the latest exact HEAD has source changes after an earlier executable PASS and therefore must not inherit that PASS.
- `PARTIAL_RUNTIME_BINDING_PENDING`: source architecture/guards exist, but authoritative runtime binding is incomplete.
- `NEW_OWNER_ACCEPTED_REQUIREMENT`: accepted in Master Plan but not yet implemented in current source.
- `BLOCKED_EXTERNAL`: Web cannot complete the slice until another owning workstream materializes the required governed capability.
- `FUTURE_SEPARATELY_GOVERNED`: intentionally outside current activation authority.

## 3. Exact current implementation shape

Current source is materially implemented and already decomposed into Web-owned feature, composition, port/adapter, incident, voice, security and design-system surfaces.

Current feature families observed in source include:

- Public Falcon experience;
- FSATS public experience;
- My Applications;
- FSATS workspace;
- Portfolio;
- Activity / Orders / Trades presentation;
- Markets;
- Advisory Markets;
- Customer AI analysis presentation;
- Notifications;
- Owner Command Center;
- Owner Provider Actions;
- Owner AI Emergency presentation;
- Customer Incident Conversation;
- local voice runtime abstractions;
- Web market-data provider planning/readiness;
- FSATS public-contract adapters;
- Foundation Stage 9 presentation adapter;
- fail-closed authentication adapter;
- safe HTML/output encoding;
- route registry, composition and design-system primitives.

The current `package.json` defines `npm test`, browser-verification serving, and a broad `npm run check` syntax gate covering the principal modules.

## 4. Important executable-evidence rule

FCR-0095, FCR-0125 and FCR-0220 retain exact checkout-backed Web verification evidence at commit:

`b6a62f5f441752afeaef4b4e8c12b67292982273`

with:

- `npm test` PASS;
- 223/223 tests PASS;
- `npm run check` PASS.

However, the current Web branch contains source/test changes after that exact verified commit. The old PASS is valid evidence for the exact earlier candidate and the semantic scopes it proved, but it SHALL NOT be transferred to the current exact HEAD.

Therefore:

`CURRENT_HEAD_FULL_EXECUTABLE_STATUS = NEEDS_FRESH_RECHECK`

This is not a finding that current source is broken. It is a truth/evidence requirement.

## 5. Baseline inventory matrix

| Requirement / surface | Current source | Evidence / contract | Current classification | Exact gap / next owner |
|---|---|---|---|---|
| Public Falcon / FSATS public | Implemented feature modules and public shell exist | current source + public tests | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` | fresh executable/browser verification after relevant source changes; WEB |
| My Applications | Implemented for normal authenticated user home | current source + My Applications tests | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` | preserve authoritative entitlement boundary; WEB |
| Falcon Owner Home after sign-in | Current route name `owner` exists, but it renders Command Center, not the newly accepted Falcon Owner Home launch surface | Owner acceptance amendment 08 | `NEW_OWNER_ACCEPTED_REQUIREMENT` | implement distinct Owner Home navigation/attention surface; WEB |
| Owner direct choice of FSATS / Command Center / future systems | Not implemented as the accepted post-sign-in launch journey | Owner acceptance amendment 08 | `NEW_OWNER_ACCEPTED_REQUIREMENT` | implement extensible top-level destination cards/navigation; WEB |
| Owner permanent full FSATS VIP feature access | Not materialized as an authoritative Owner-specific entitlement binding | Owner acceptance amendment 08 | `NEW_OWNER_ACCEPTED_REQUIREMENT` + `PARTIAL_RUNTIME_BINDING_PENDING` | Web UX/guards + exact authoritative entitlement contract; WEB and owning entitlement contract as governed |
| Auth protected-route shell | Fail-closed `UnavailableAuthAdapter`, authoritative-session checks, role route guard exist | `src/auth.js` | `PARTIAL_RUNTIME_BINDING_PENDING` | consume Foundation Stage 16; WEB |
| Authoritative identity/session/MFA | Foundation Stage 16 is now accepted and closed and handed off | FCR-0152 current body: `Waiting On: WEB`, Stage16 58/58 twice + final Red Team PASS | `PARTIAL_RUNTIME_BINDING_PENDING` | Web consuming binding/governed verification is the immediate priority |
| Owner/customer route separation | Explicit route/session role guards exist | auth source/tests | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` | reconcile with new Owner Home and Owner feature-access semantics; WEB |
| FSATS workspace/layout | Workspace feature exists | source + workspace tests | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` | Master Plan WP-07 full customizable-layout acceptance still requires current-state assessment and browser verification; WEB |
| Portfolio / positions / orders / performance | v1 adapter and presentation exist | FCR-0133 CLOSED | `EXISTING_VERIFIED_SEMANTICS` + fresh current-candidate recheck required | no semantic redesign; later runtime route only if separately materialized |
| School/Strategy catalog | External authoritative catalog presentation exists | FCR-0128 CLOSED | `EXISTING_VERIFIED_SEMANTICS` | preserve catalog/applicability/entitlement separation |
| Trading chart overlays | adapter/presenter exists | FCR-0126 CLOSED | `EXISTING_VERIFIED_SEMANTICS` | preserve no Strategy activation from rendering |
| On-demand analysis | request/result adapter semantics exist | FCR-0127 CLOSED | `EXISTING_VERIFIED_SEMANTICS` | exact operational transport remains separately governed |
| Detailed AI analysis/synthesis | customer AI presentation exists | FCR-0130 CLOSED | `EXISTING_VERIFIED_SEMANTICS` | new Web LSA architecture is separate and not implied by this existing presentation |
| Ordinary customer Web AI LSA | Existing AI surface presents FSATS analysis, but Owner-approved executable LSA identity/governance model is not yet materialized | Owner direction + Master Plan 02 | `NEW_OWNER_ACCEPTED_REQUIREMENT` | WP-12 governed reconciliation precedes implementation |
| Web MSA / Owner conversational gateway | Current Owner Command Center contains UI/chat presentation, but accepted MSA routing architecture is not fully materialized | Master Plan 02 / WP-12/13 | `NEW_OWNER_ACCEPTED_REQUIREMENT` | define/register governed AI subject and routing/governance interfaces before executable activation |
| Full FSATS self-development report through Web to Owner | Owner direction accepted; exact end-to-end contract is not fully materialized | Master Plan WP-18 | `PARTIAL_RUNTIME_BINDING_PENDING` | Application/Web contract/FCR reconciliation required |
| Owner Command Center | substantial feature exists | current source | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` | bind authoritative Stage 14 projection and separate it from new Owner Home |
| Authoritative Falcon OS operational projection | Web port exists fail-closed; Foundation projection ready | FCR-0169 `Waiting On: WEB` | `PARTIAL_RUNTIME_BINDING_PENDING` | Web binding/governed verification |
| Owner AI emergency request/presentation | source and strict request/accept/completion semantics exist | FCR-0225 CLOSED | `EXISTING_VERIFIED_SEMANTICS` | no live endpoint or Kill execution authority inferred |
| Stage 9 recovery/release presentation | bounded Web adapter exists and fails closed | FCR-0076 `Waiting On: FOUNDATION` | `BLOCKED_EXTERNAL` | exact authoritative Stage 9 public projection still Foundation-owned |
| Incident Conversation | controller, timeline, content safety, screenshot, persistence and feature surface exist | FCR-0095 `Waiting On: WEB` | `PARTIAL_RUNTIME_BINDING_PENDING` | identity/tenant, production persistence, scanner, Support transport, local voice and authenticated browser verification |
| Incident persistence | IndexedDB implementation exists for local Web durability | source | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` / not production persistence | production tenant-scoped persistence remains |
| Support takeover | source semantics exist with authoritative capability gating | FCR-0095 | `PARTIAL_RUNTIME_BINDING_PENDING` | authoritative identity/capability + Support transport |
| Local voice | Browser microphone + Whisper.cpp/Piper abstraction + live voice policies exist | source + prior verification | `PARTIAL_RUNTIME_BINDING_PENDING` | exact supported-environment executable binding and real runtime verification |
| Web presentation market data | separate Web port, market plan and pending destinations exist | FCR-0125 | `PARTIAL_RUNTIME_BINDING_PENDING` | principal/service-role/policy/credential refs + governed per-route verification |
| Provider route safety/readiness | strict fail-closed readiness guard exists | source + FCR-0125/0220 | `EXISTING_VERIFIED_SEMANTICS` + runtime pending | actual route bindings remain |
| Advisory markets / quota split | request-driven advisory source and quota semantics exist | FCR-0220 `Waiting On: WEB` | `PARTIAL_RUNTIME_BINDING_PENDING` | final provider runtime/UI binding |
| Binance/Coinbase/Bybit/Alpaca/Finnhub presentation routes | destination records/source guardrails exist | FCR-0173..0177, FCR-0196..0200 | `PARTIAL_RUNTIME_BINDING_PENDING` | exact Web binding/governed verification before connectivity |
| Secure provider credential entry | ordinary chat/plain state deliberately does not provide this | Provider Actions source + FCR-0220 | `PARTIAL_RUNTIME_BINDING_PENDING` | exact secure entry/storage/runtime mechanism remains governed |
| Arabic/English RTL/LTR + accessibility | source/style/tests exist | package/tests + browser checklist | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` | authenticated browser verification after identity binding |
| Browser runtime verification | checklist/server exists | browser verification record | `EXISTING_NEEDS_EXECUTION` | rerun public and authenticated checks on exact candidate |
| Output encoding/security | central safe HTML and hostile-output tests exist | source/tests; prior 223/223 evidence | `EXISTING_NEEDS_FRESH_EXECUTABLE_RECHECK` | fresh full suite on current candidate |
| Production deployment | not activated by plan or source | Master Plan WP-26 | `FUTURE_SEPARATELY_GOVERNED` | explicit deployment authority required |

## 6. Current FCR priority snapshot

Fresh repository checks found these immediate Web-owned obligations:

1. **FCR-0152 — Waiting On: WEB**: Foundation Stage 16 is `ACCEPTED_AND_CLOSED`; Web consuming-side identity/session/MFA binding and verification is now required.
2. **FCR-0169 — Waiting On: WEB**: bind the accepted Stage 14 public operational projection.
3. **FCR-0095 — Waiting On: WEB**: complete incident/support/voice runtime bindings.
4. **FCR-0125 and FCR-0220 — Waiting On: WEB**: final provider presentation/runtime contract binding.
5. **FCR-0173..0177 and FCR-0196..0200 — Waiting On: WEB**: exact Web provider route binding/governed verification.

External hold:

- **FCR-0076 — Waiting On: FOUNDATION** for the residual exact Stage 9 recovery/release public projection. Web shall not invent it.

## 7. Owner-final amendments reconciliation

### A. Falcon Owner Home

Current source route `owner` is classified as an Owner route, but the rendered view is the Owner Command Center itself. Therefore the accepted post-sign-in path is not yet implemented.

Required behavior:

`Owner Sign In -> Falcon Owner Home -> FSATS | Command Center | future Falcon top-level systems`

with compact urgent-attention visibility.

### B. Permanent Owner FSATS feature access

Current source does not yet prove the final Owner-specific entitlement relationship that gives the Owner all current/future customer-facing VIP product features without trial/downgrade semantics. This must remain separate from business/execution authority.

## 8. Architectural findings relevant to WP-02

The codebase has already moved meaningfully toward feature/port decomposition, but `src/app.js` remains a broad composition/orchestration file. WP-02 remains valid as continuation/hardening, not rewrite.

However, current FCR precedence and the Master Plan priority rule now require the FCR-0152 Web consuming action to be handled before unrelated major WP-02 source refactoring because authoritative identity is a current `Waiting On: WEB` blocker affecting Owner routing, customer tenancy and incident/support verification.

Therefore execution order after WP-01 is:

1. fresh Stage 16/FCR-0152 contract read and Web consuming-binding implementation/verification;
2. then resume WP-02 architecture decomposition against that current identity boundary;
3. keep provider connectivity, Web MSA/LSA implementation and production deployment in their separately governed scopes.

## 9. WP-01 review / Red Team

The inventory was challenged against stale FCR ownership, inherited old PASS evidence, false Owner Home completion, Owner VIP-as-subscription confusion, authority leakage, production-persistence confusion, voice-runtime confusion, provider activation confusion and AI-architecture confusion.

Result:

- `OPEN_WP01_CRITICAL = 0`
- `OPEN_WP01_HIGH = 0`
- `OPEN_WP01_MEDIUM = 0`
- `OPEN_WP01_LOW = 0`
- `WP01_BASELINE_CONSISTENCY = PASS`

## 10. WP-01 closure and immediate next action

`WP01 = COMPLETE`

Current precedence-controlled immediate action:

`FCR-0152 WEB CONSUMING IDENTITY/SESSION/MFA BINDING + GOVERNED VERIFICATION`

After that current Web obligation is dispositioned, resume:

`WP-02 — Architecture decomposition and composition stabilization`

No production authentication activation, external identity-provider connectivity, provider connectivity, Web MSA/LSA executable activation or deployment is inferred from this sequencing.