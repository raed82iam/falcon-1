# FSATS V1.4 Part 0 / P0-C — Application Topology, Ownership and Awareness Candidate

**Status:** `DRAFT_DESIGN_CANDIDATE`  
**Scope:** `P0-C only`  
**P0-A:** `OWNER_ACCEPTED_AND_CLOSED`  
**P0-B:** `OWNER_ACCEPTED_AND_CLOSED`  
**P0-C Owner acceptance:** `NOT_GRANTED`

## 1. Design decision under review

The proposed V1.4 topology keeps FSATS as a **non-owning trading-system/domain boundary**.

FSATS SHALL NOT own:

- mutable state;
- an MSA or LSA;
- credentials;
- persistence;
- Foundation resources;
- Service Bus routes;
- lifecycle/admission authority;
- a shared runtime service;
- a shared provenance service;
- business authority that belongs to an Application.

FSATS may provide a system/domain identity, architecture navigation boundary, shared business vocabulary and traceability context. Containment inside FSATS does not grant reachability, permission, authority, resource access or internal-state access.

## 2. Proposed Application topology

### 2.1 FSATS trading-domain Applications

| Application | Role | Relationship to FSATS | MSA |
|---|---|---|---|
| Falcon Trading Guardian Application | trading-scoped crisis/protection authority | member of trading domain | exactly 1 |
| Falcon Self-Aware Provider Management Application (FSAPMA) | operational external trading-provider/data management | member of trading domain | exactly 1 |
| Falcon Self-Aware Trading Application | trading analysis/decision/risk/capital/intent/execution/learning | member of trading domain | exactly 1 |

### 2.2 Independent adjacent Applications

| Application | Role | Relationship to FSATS | MSA |
|---|---|---|---|
| Falcon Self-Aware Trading Simulation Application (FSTSimA) | independent non-Live simulation/validation Application | adjacent validation Application; not a mode inside Trading | exactly 1 |
| Shared Communication Application | user/external notification rendering/channel-delivery Application | adjacent Shared Application; not owned by FSATS | exactly 1 |
| Shared Web Application | presentation/read-model/command-and-consent surface | adjacent Shared Application; not owned by FSATS | exactly 1 |

Future Shared Applications remain deferred registry entries only until separately designed, admitted and authorized. A future name in a registry is not a current Application instance.

## 3. Guardian major-branch / LSA map

The historical four-LSA decomposition remains justified as four cohesive major responsibilities.

| LSA | Major branch | Owns | Explicitly does not own |
|---|---|---|---|
| G-LSA-01 | Protection State and Command Governance | Guardian operating state, scoped protection decision, protection command intent and restriction state | Foundation routing; Trading execution; normal Trading Risk calculation |
| G-LSA-02 | Threat, Trigger and Crisis Assessment | Guardian trigger evidence, crisis classification, unknown/extreme-regime protection assessment | market-data acquisition; strategy decision truth; Foundation health truth |
| G-LSA-03 | Recovery, Reconciliation and Release | Guardian recovery criteria, protection-state reconciliation, progressive release recommendation/decision within Guardian authority | Foundation lifecycle recovery; Trading position truth; broker truth |
| G-LSA-04 | Guardian Learning, Playbook and Protection Improvement | post-incident Guardian learning, playbook candidates, trigger/playbook improvement evidence | self-deployment; active-crisis rule mutation; Trading strategy evolution |

**Guardian topology result:** `4 major branches / 4 LSAs` remains the P0-C candidate.

## 4. FSAPMA major-branch / LSA map

The historical six-LSA decomposition remains justified after separating provider-domain responsibilities from Foundation platform ownership.

| LSA | Major branch | Owns | Explicitly does not own |
|---|---|---|---|
| P-LSA-01 | Provider Registry and Capability Intelligence | provider/service/endpoint/product/plan/entitlement capability knowledge and evidence | Foundation service catalog; broker execution truth |
| P-LSA-02 | Data Products and Data Service Contracts | vendor-neutral trading-data product/business contract definitions | Foundation canonical transport schema ownership; Trading interpretation |
| P-LSA-03 | Provider Selection, Fallback and Business Route-Lease Planning | provider choice, fallback plan, precomputed provider-selection lease/business descriptor | Foundation Service Bus route/admission authority |
| P-LSA-04 | Data Quality, Lineage and Provider Reconciliation | source lineage, freshness, corrections, provider conflict/quality/reconciliation outcomes | Trading decision confidence authority; Foundation evidence platform ownership |
| P-LSA-05 | Provider/API Capacity, Quota and Cost Governance | provider API credits, quotas, concurrency, batching/caching/dedup policy, provider-cost/free-first control | CPU/RAM/network allocation; Foundation technical priority/resource truth |
| P-LSA-06 | Provider/Broker-Service Role and Onboarding Evidence | provider-role versus broker-role capability evidence, account/service capability profile and onboarding evidence | order/fill/position truth; Trading broker execution state |

**FSAPMA topology result:** `6 major branches / 6 LSAs` remains the P0-C candidate.

## 5. Trading major-branch / LSA map

The historical twelve-LSA count remains justified only through explicit non-overlapping major responsibilities below.

| LSA | Major branch | Owns | Explicitly does not own |
|---|---|---|---|
| T-LSA-01 | Operations, Tenant, Account and Environment Control | Trading-side tenant/user/account/environment business scope and operating context | Foundation identity/auth infrastructure; broker external truth |
| T-LSA-02 | Market Profiles, Universe and Instrument Eligibility | market-profile business rules, broker/account-scoped tradable universe and instrument eligibility | provider acquisition; broker capability truth source |
| T-LSA-03 | Analysis Frameworks and Market Interpretation | reusable analysis-framework outputs and evidence | final trade authority; provider data truth |
| T-LSA-04 | Trading Schools and Strategy Management | central school/strategy registry, controller, compatibility/configuration and strategy outputs | duplicate per-market strategy registries; final Risk veto |
| T-LSA-05 | Opportunity, Proposal and Decision Orchestration | opportunity claims, proposal reconciliation and final Trading decision assembly subject to hard authorities | Risk ownership; capital ledger ownership; execution truth |
| T-LSA-06 | Unified Risk Management | authoritative Trading risk assessment, veto, aggregate/correlation/exposure constraints and risk budgets | Guardian crisis state; Foundation technical resources |
| T-LSA-07 | Portfolio and Trading Capital Allocation | Trading portfolio intent, hierarchical capital allocation/reservation and competing-opportunity allocation | Foundation compute resources; broker-reported external balance truth |
| T-LSA-08 | Trading Intent and Horizon Governance | immutable Trading Intent identity, horizon profile/clock/lifecycle and anti-reset/anti-sharding semantics | order execution truth; provider data acquisition |
| T-LSA-09 | Execution and Broker Interaction | broker-facing order intent dispatch, ACK/NACK/fill/execution truth and broker capability enforcement | provider-data ownership; Guardian protection policy ownership |
| T-LSA-10 | Position, Fill Allocation and Reconciliation | logical intent allocation, physical position reconciliation and position/fill projection | broker-origin external truth; Risk policy ownership |
| T-LSA-11 | Learning, Performance Attribution and Evolution | learning evidence, performance attribution, candidate evolution and strategy-improvement evidence | self-deployment; FSA/Owner approval; active strategy registry mutation without governance |
| T-LSA-12 | Trading Reliability, Readiness and Runtime Coordination | Trading-domain readiness/degraded state, Application runbooks, business-runtime coordination and non-Foundation operational health context | Foundation lifecycle/health/resource authority; cross-App transport ownership |

`T-LSA-12` is intentionally bounded to Trading-owned operational readiness and business-runtime coordination. Generic OS health, lifecycle, resource and transport concerns remain Foundation-owned.

**Trading topology result:** `12 major branches / 12 LSAs` remains the P0-C candidate.

## 6. FSTSimA major-branch / LSA map

FSTSimA remains an independent non-Live validation Application rather than a Trading runtime mode.

| LSA | Major branch | Owns | Explicitly does not own |
|---|---|---|---|
| S-LSA-01 | Simulation Run and Environment Control | simulation run identity, scenario/environment configuration and deterministic run orchestration | Live authority; Foundation lifecycle authority |
| S-LSA-02 | Simulation Clock and Time Control | simulated clock/time progression and deterministic time behavior | production clock authority |
| S-LSA-03 | Market Data, Replay and Scenario Feed Simulation | non-authoritative simulation/replay market inputs and scenario streams | operational market-data truth |
| S-LSA-04 | Provider Simulation | simulated provider behavior, degradation and provider fault profiles | FSAPMA operational provider truth |
| S-LSA-05 | Broker and Execution Simulation | simulated broker/order/fill behavior and execution fault models | Live broker credentials/routes/effects |
| S-LSA-06 | Account, Portfolio and Capital Simulation | simulated account/capital/portfolio state used only inside validation context | authoritative Trading or broker financial truth |
| S-LSA-07 | Fault, Stress and Adversarial Scenario Injection | controlled faults, outages, latency, malformed/degraded scenario injection | production fault injection |
| S-LSA-08 | Fidelity, Oracle, Evidence and Validation Assessment | simulation fidelity, expected/observed oracle comparison, validation evidence and reproducibility | Owner acceptance; Live authority |

**FSTSimA topology result:** `8 major branches / 8 LSAs` remains the P0-C candidate.

## 7. Shared Communication major-branch / LSA map

V1.3 established Communication as an independent Shared Application but P0-B did not freeze an exact canonical LSA count. Current APP-001 requires explicit major branches. P0-C therefore proposes four cohesive branches.

| LSA | Major branch | Owns | Explicitly does not own |
|---|---|---|---|
| C-LSA-01 | Notification Intake and Source-Truth Boundary | accepted notification/message request context, source attribution and communication business intake | source Application business truth; Foundation message admission truth |
| C-LSA-02 | Rendering, Templates and Localization | user-facing message rendering, template selection, locale/terminology transformation | modification of source severity/business meaning |
| C-LSA-03 | External Channel Policy and Delivery | external/user channel selection and channel-specific delivery attempt policy | generic inter-Application Service Bus transport |
| C-LSA-04 | Recipient, Acknowledgement, Escalation and Delivery Evidence | recipient policy, user acknowledgement/escalation semantics and external delivery evidence | Trading/Guardian business-state mutation |

**Shared Communication topology result:** `4 major branches / 4 LSAs` proposed for V1.4.

## 8. Shared Web major-branch / LSA map

V1.3 established Web as an independent Shared Application but P0-B did not freeze an exact canonical LSA count. P0-C proposes four cohesive branches.

| LSA | Major branch | Owns | Explicitly does not own |
|---|---|---|---|
| W-LSA-01 | Web Shell, Module Composition and Navigation | presentation-shell composition, module layout/navigation and UI lifecycle inside Web | Application business ownership; Foundation lifecycle |
| W-LSA-02 | Read Models and Presentation Projection | Web-owned presentation/read projections derived from authoritative sources | Trading/broker/provider source truth |
| W-LSA-03 | User Commands, Consent and Interaction | UI command/consent capture, validation of presentation context and forwarding intent to governed business contracts | execution/business authority created by button click |
| W-LSA-04 | Session, Entitlement Context and Localization Presentation | consumption/presentation of governed session/entitlement context plus locale/timezone/currency presentation | authentication/authorization/secret authority |

**Shared Web topology result:** `4 major branches / 4 LSAs` proposed for V1.4.

## 9. MSA model

Every declared Application above owns exactly one MSA.

The MSA:

- maintains Application-wide awareness and operating picture;
- integrates branch/LSA evidence inside the same Application;
- does not become a cross-Application supervisor;
- does not bypass Application contracts;
- does not inherit authority over Foundation or another Application;
- uses the current origin-aware self-development path for proposals intended for production adoption.

There is no `FSATS MSA`.

There is no ecosystem MSA above the Application MSAs.

FSA remains Foundation-only and is not part of an Application topology.

## 10. LSA model

Each declared major branch has exactly one responsible LSA.

An LSA:

- understands and evaluates its own major branch;
- may observe authorized evidence from owned components;
- may participate in Application-internal coordination through declared interfaces;
- owns no sibling branch by awareness rank;
- owns no other Application;
- cannot create a route, permission or authority merely by knowing another branch/Application exists.

## 11. CSA eligibility model

P0-C does not instantiate CSA identities. Exact CSA declarations belong in the later Application Manifest/design work.

Eligibility rule:

- CSA is optional;
- CSA may exist only for an eligible intelligent component with a genuine component-owned specialization, measurable outputs and a bounded improvement surface;
- a major branch does not receive a CSA automatically;
- deterministic infrastructure, registries, passive storage, simple adapters and pure transport wrappers do not require CSA merely to satisfy symmetry;
- a CSA remains subordinate to exactly one parent LSA and one component boundary;
- CSA rank does not create external authority or cross-Application access.

Likely CSA-eligible domains include selected intelligent components inside analysis, strategy, provider selection/quality, learning/evolution, Guardian assessment/improvement and simulation/fidelity branches. Eligibility does not equal activation or implementation authority.

## 12. Single-owner invariants

The following invariants are proposed as P0-C architecture rules:

1. FSATS owns no mutable runtime truth.
2. One mutable business truth has one accountable Application/branch owner.
3. Foundation platform truth is never re-owned by an Application branch.
4. Business/domain state does not become Foundation state merely because Foundation transports or stores generic evidence.
5. A Shared Application remains independent and does not become an FSATS internal service by reuse.
6. Guardian protection authority does not make Guardian owner of Trading execution, Trading Risk, FSAPMA data, or Foundation resources.
7. FSAPMA operational-data ownership does not make FSAPMA owner of Trading interpretation or Service Bus transport.
8. Web presentation does not make Web owner of presented business truth.
9. Communication delivery does not make Communication owner of source business meaning.
10. FSTSimA simulation truth never becomes Live-authoritative truth.
11. Awareness rank never creates jurisdiction.
12. Cross-Application access requires later P0-F contract/authority/route design; topology alone grants none.

## 13. Alternative assessment

The following alternatives were challenged:

### A. Make FSATS a fourth Application

Rejected. It creates a hidden system-level owner, duplicate MSA/jurisdiction and pressure toward shared mutable state.

### B. Collapse Guardian, FSAPMA and Trading into one monolithic Application

Rejected. It weakens independent ownership, lifecycle isolation, replaceability, failure containment and specialized governance.

### C. Embed Guardian inside Trading

Rejected. Guardian's protection authority and independent failure/decision surface are materially different from ordinary Trading decision ownership.

### D. Move FSAPMA into Foundation or make provider access a Foundation service

Rejected. Provider/product/market-data semantics are trading-domain business semantics. Foundation must remain Application-neutral.

### E. Make FSTSimA a runtime flag/mode inside Trading

Rejected. This weakens non-Live isolation and increases the risk of authority/credential/route contamination.

### F. Embed Web/Communication as Trading modules

Rejected. They are reusable adjacent Shared Applications and must not become Trading-owned merely because Trading is an initial consumer.

### G. Keep independent Applications under a non-owning FSATS domain boundary

Selected as the strongest current topology.

## 14. V1.3 difference/disposition report for P0-C

| Subject | V1.3 / P0-B input | Proposed P0-C treatment | P0-C disposition |
|---|---|---|---|
| FSATS system identity | non-owning trading-system boundary | retain and make no-owner rule explicit | `RETAINED` |
| Guardian Application | independent Application | retain | `RETAINED` |
| FSAPMA Application | independent Application | retain | `RETAINED` |
| Trading Application | independent Application | retain | `RETAINED` |
| Guardian LSA count | 4 candidate | retain after branch-cohesion proof | `RETAINED` |
| FSAPMA LSA count | 6 candidate | retain after ownership separation proof | `RETAINED` |
| Trading LSA count | 12 candidate | retain after explicit major-branch proof and Foundation-boundary narrowing | `RETAINED` |
| FSTSimA | independent non-Live Application, 8-LSA candidate | retain independent Application and eight major branches | `RETAINED` |
| Shared Communication | independent adjacent Application | retain identity; explicitly define 4 major branches/LSAs to satisfy current APP-001 | `IMPROVED` |
| Shared Web | independent adjacent Application | retain identity; explicitly define 4 major branches/LSAs to satisfy current APP-001 | `IMPROVED` |
| FSATS shared provenance/resource/runtime owner | historical wording existed in some V1.3 areas | prohibit hidden FSATS mutable owner | `MODIFIED_FOR_CURRENT_ARCHITECTURE_ALIGNMENT` |
| Awareness hierarchy | one MSA/Application; LSA/major branch; optional CSA/component | retain and explicitly deny cross-App authority by rank | `RETAINED` |
| Future Shared Applications | future registry | keep deferred; not current Application instances | `RETAINED` |

No P0-C item requires `OWNER_DECISION_REQUIRED` in this draft. Owner acceptance remains required for the P0-C candidate as a whole.

## 15. Downstream obligations

P0-C acceptance, if later granted, SHALL require downstream work to preserve:

- P0-E: complete CON-023 manifests containing these Application/MSA/major-branch/LSA identities or an explicitly reviewed successor;
- P0-F: no cross-App edge derived from containment; every interaction uses explicit business contract and governed Foundation route/authority;
- P0-D: no branch duplicates Foundation-owned lifecycle, transport, security, resource or admission semantics;
- P0-L: end-to-end traceability proving every declared owner and no hidden owner.

A later work package may propose a better topology only through the accepted post-change review rule. It may not silently merge, split, rename or relocate an accepted owner in a way that changes responsibility.

## 16. Current status

```text
P0C_TOPOLOGY_CANDIDATE = DRAFT
P0C_APPLICATIONS_DECLARED = 6
P0C_FSATS_SYSTEM_OWNER = NONE
P0C_APPLICATION_MSA_COUNT = 6
P0C_GUARDIAN_LSA_COUNT = 4
P0C_FSAPMA_LSA_COUNT = 6
P0C_TRADING_LSA_COUNT = 12
P0C_FSTSimA_LSA_COUNT = 8
P0C_COMMUNICATION_LSA_COUNT = 4
P0C_WEB_LSA_COUNT = 4
P0C_ARCHITECTURE_REVIEW = NOT_YET_RUN
P0C_RED_TEAM = NOT_YET_RUN
P0C_OWNER_FINAL_ACCEPTANCE = NOT_GRANTED
P0D_THROUGH_P0L = NOT_STARTED
```
