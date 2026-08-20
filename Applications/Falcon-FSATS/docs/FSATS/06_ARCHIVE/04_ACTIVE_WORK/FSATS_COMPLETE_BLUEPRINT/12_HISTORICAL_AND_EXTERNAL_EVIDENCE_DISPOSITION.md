# FSATS Complete Blueprint — Historical and External Evidence Disposition

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. Purpose

This register prevents two opposite errors:

1. blindly copying old architecture because it already exists;
2. losing strong historical ideas merely because a new design is cleaner.

Every material historical/external idea is classified by its current disposition.

## 2. Disposition Classes

- `KEEP` — retain the semantic concept.
- `KEEP_AND_HARDEN` — retain and strengthen controls/evidence.
- `REPLACE_MECHANISM_PRESERVE_INTENT` — same purpose, better implementation architecture.
- `REFERENCE_ONLY` — useful history, not current design.
- `FOUNDATION_DEPENDENT` — valid need but runtime realization waits on Foundation.
- `DROP` — reject because conflicting, unsafe or unnecessary.
- `FUTURE` — explicitly outside initial scope.

## 3. P0 / P1 Convenience Archive

By current Owner direction, the convenience `applications/docs/FSATS/P0/` and `applications/docs/FSATS/P1/` directories are treated as archive/reference for this design cycle.

They are mined for requirements and lessons but are not used as a shortcut around current governing sources.

The accepted canonical Part 0 baseline remains governing until a later explicit Owner acceptance supersedes it.

## 4. V1.3 Core Disposition

| V1.3 / historical concept | Disposition | Current treatment |
|---|---|---|
| FSATS as trading-system boundary | KEEP_AND_HARDEN | non-owning container; no hidden principal/state/credentials |
| Self-Aware Trading Application | KEEP | 1 MSA / 13 LSAs |
| FSAPMA | KEEP_AND_HARDEN | sole operational provider-data gateway; stronger Provider/ServiceRole/APIInstance model |
| Trading Guardian | KEEP_AND_HARDEN | independent scoped protection; no blind liquidation/business takeover |
| FSTSimA / simulator idea | KEEP_AND_HARDEN | independent 8-LSA Application with fidelity and independent validation |
| one MSA per Application | KEEP | required by APP-001/AWR-006 |
| LSA/CSA locality | KEEP_AND_HARDEN | CSA optional/eligibility-based, no artificial hierarchy |
| US Equities + Crypto Spot | KEEP | initial market scope |
| 1:1 funded exposure | KEEP_AND_HARDEN | no borrowed leverage; shorting disabled initially unless separately qualified |
| Paper-first | KEEP_AND_HARDEN | Paper is evidence, explicitly corrected for reality gap |
| central Strategy Catalog/Controller | KEEP_AND_HARDEN | one identity per strategy; market applicability profiles |
| Market Profiles | KEEP_AND_HARDEN | capability/session/data/risk/execution profiles |
| Unified Risk | KEEP_AND_HARDEN | deterministic hard pre-trade gate |
| Global Capital Reservation Ledger | KEEP_AND_HARDEN | exact reservation state + concurrency/idempotency/reconciliation |
| provider pools/routing/quota | KEEP_AND_HARDEN | entitlement/quality/cost/reliability/failover and dynamic active-set allocation |
| simulator/replay/shadow evidence | KEEP_AND_HARDEN | independent credibility/fidelity/divergence evidence |
| contract-first communication | KEEP | aligned to Foundation FIL/Service Bus boundaries |
| idempotency/reconciliation/provenance | KEEP_AND_HARDEN | required in order/protection/resource/evidence state machines |
| market-wide scan before every request | REPLACE_MECHANISM_PRESERVE_INTENT | progressive dynamic universe funnel with cheap discovery + rich active set |
| strategy copy per market | DROP | central strategy identity + Market Profile applicability |
| fixed provider dependency | DROP | pluggable capability-driven provider routing |
| hidden FSATS-owned shared state | DROP | explicit owner per Application/FSARM governed role |
| local substitute for missing Foundation service | DROP | FCR/fail-closed boundary |
| FSA as Trading evaluator/approver | DROP | FSA OS-governance/compatibility only |
| Owner silence as approval | DROP | prohibited |

## 5. Recent Owner AI/Awareness Decisions

| Concept | Disposition | Treatment |
|---|---|---|
| CSA -> LSA -> MSA -> FSA origin route | KEEP | exact origin-correct review chain |
| MSA-origin -> FSA directly | KEEP | no fake lower tier |
| same-responsibility self-development | KEEP_AND_HARDEN | performance/speed/accuracy/robustness etc. within authority |
| Internet research for Application Awareness | FOUNDATION_DEPENDENT | research-only, quarantine/provenance, FCR-0008 |
| FSA direct Internet | DROP | forbidden by Owner direction |
| 2 Monitor AI per FSATS MSA | KEEP_AND_HARDEN | 8 monitors total, independent perspectives |
| Monitor majority voting | DROP | disagreement triggers integrity check |
| recursive monitor hierarchy | DROP | deterministic monitor integrity controls |
| minimum integrity check on Awareness error | KEEP | goals/purpose + authority/permissions + core architecture |
| Investigation Hold | KEEP_AND_HARDEN | static + behavioral evidence |
| investigation refusal/interference | KEEP | critical integrity signal |
| Kill vs Rollback vs Factory Reset | KEEP | distinct semantics |
| Last Trusted vs Factory Trusted baseline | KEEP | distinct baseline authority |
| hash equality = trust | DROP | behavioral/mutable-state integrity also required |
| Controlled Revival | KEEP_AND_HARDEN | independent validation + probationary return |
| AI controls its own containment/release | DROP | independent control required |
| 24-hour FSA fallback | REFERENCE_ONLY / UNRESOLVED | not currently authorized |

## 6. Resource-Management Evolution

| Concept | Disposition | Treatment |
|---|---|---|
| Trading-only TARC as future aggregate resource owner | REPLACE_MECHANISM_PRESERVE_INTENT | T-LSA-13 remains Trading awareness; FSARM coordinates FSATS-wide resources |
| internal redistribution before Foundation request | KEEP | prime FSARM rule |
| request gross demand from Foundation first | DROP | request only proven residual need |
| fixed Application priority ranking | DROP | dynamic consequence/minimum/reclaimability evidence |
| Guardian directly seizes resources | DROP | Guardian reports need; FSARM coordinates; Foundation retains total authority |
| opaque pooled FSATS allocation | DROP | per-Application attribution retained |
| potential future Falcon-wide FSARM | FUTURE | backlog only, no current architecture effect |

## 7. External Engineering Evidence

External material informs implementation quality but is not Falcon authority.

### SEC Market Access Rule / official FAQ

Useful engineering principles adopted:

- pre-trade risk controls before orders reach the market;
- preset capital/credit-like ceilings;
- price/size/duplicate erroneous-order controls;
- authorized-person/system access controls;
- immediate post-trade/execution visibility.

Falcon applies these as internal safety design principles. This document makes no legal determination about whether or how a future Falcon deployment is regulated.

### FINRA Regulatory Notice 15-09

Useful practices adopted:

- risk assessment before algorithm deployment;
- controlled code development/change management;
- independent testing/system validation;
- segregated development/testing;
- retrievable version history;
- pilot/limited deployment;
- real-time heightened monitoring after change;
- rapid disable capability;
- post-production review/reconciliation.

These strengthen Falcon's existing governed evolution model.

### NIST AI RMF / GenAI Profile

Useful principles adopted:

- lifecycle AI risk management;
- explicit trustworthiness/limitations;
- evaluation, monitoring and evidence;
- risk-based deployment;
- continuous reassessment rather than one-time model acceptance.

Falcon's governance is stricter where its own Constitution/Owner rules require it.

### NIST SSDF

Useful practices adopted:

- integrate security through the SDLC;
- protect software/model artifacts;
- secure build/dependency provenance;
- identify/respond to vulnerabilities;
- root-cause learning to prevent recurrence.

### FIX ExecutionReport semantics

Useful pattern adopted:

- distinguish event purpose from current order state;
- maintain explicit acknowledgement/status/fill/reject/cancel semantics;
- preserve chain/order state rather than treating each message as isolated truth.

Falcon does not require FIX protocol internally; it adopts the robust semantic separation.

### Alpaca official documentation

Current vendor facts useful for the initial profile include:

- Paper and Live are separate environments;
- Paper is a simulation and may differ materially from Live;
- Paper may omit market impact, information leakage, latency slippage and queue position;
- free/basic US-equity real-time data may be limited to IEX and bounded websocket subscriptions;
- richer consolidated market coverage depends on subscription capability.

Design impact:

- strict Paper/Live isolation;
- dynamic provider capability profile;
- progressive instrument universe/subscription controller;
- FSTSimA Paper Reality Gap and pessimistic execution models;
- no assumption that one vendor plan remains permanently unchanged.

### OpenTelemetry .NET

Useful implementation evidence:

- traces, metrics and logs are mature/stable in current .NET OpenTelemetry ecosystem.

Design impact:

- use OpenTelemetry-compatible application instrumentation where useful;
- keep Falcon audit/evidence authority separate from telemetry.

## 8. New Blueprint Ideas Introduced

These are candidate design improvements, not historical authority.

### 8.1 Progressive Universe Funnel

Replaces repeated expensive whole-market deep scanning with tiered discovery and dynamically allocated rich data.

### 8.2 Paper Reality Gap Ledger

Makes Paper-to-Live modeling error a first-class measurable object rather than a footnote.

### 8.3 Canonical Broker-Independent Order Model

Prevents broker SDK state from becoming Trading domain architecture.

### 8.4 Deterministic Hard Gate Around AI

AI remains powerful in analysis/learning while deterministic policy owns authority/capital-protection edges.

### 8.5 Strategy Capital Competition as Evidence

Strategies compete by measured risk-adjusted efficiency, but Portfolio/Risk retain authority.

### 8.6 Sparse CSA

Only genuinely intelligent self-improving components receive CSA, preventing an unmaintainable awareness forest.

### 8.7 Modular-Monolith Per Application

Preserves four real Falcon Application boundaries while avoiding premature 31-service distributed complexity.

### 8.8 Explicit Truth-Class Separation

Operational, simulation/replay and governance/evidence truth are labeled at every material boundary.

### 8.9 Dynamic Resource Priority

Protects active obligation and consequence rather than permanent Application rank.

### 8.10 Behavioral Integrity

AI trust restoration requires more than matching code/config hashes; learned/mutable behavior is also investigated.

## 9. Deliberately Deferred Items

The following remain future, separately governed scope:

- derivatives/options/futures;
- borrowed leverage;
- autonomous short-selling expansion;
- multi-user commercial operation;
- new markets beyond US Equities/Crypto Spot;
- live broker/provider credentials;
- unrestricted Live deployment;
- Falcon-wide FSARM;
- autonomous expansion of AI authority;
- FSA direct Internet access;
- 24-hour silent-Owner promotion.

## 10. Preservation Guarantee

No historical artifact is deleted or rewritten by this blueprint.

If this candidate is eventually accepted, its acceptance record must explicitly state what it supersedes prospectively and what remains immutable historical evidence.
