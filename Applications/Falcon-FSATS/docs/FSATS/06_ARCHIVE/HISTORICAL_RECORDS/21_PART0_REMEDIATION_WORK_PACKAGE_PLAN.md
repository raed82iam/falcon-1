# FSATS V1.4 Part 0 - Remediation Work Package Plan

**Status:** `ACTIVE_REMEDIATION_PLAN`  
**Scope:** `Part 0 only`  
**Branch:** `application-development`  
**Authority record:** `20_PART0_REOPEN_AND_REMEDIATION_AUTHORITY_RECORD.md`  
**Owner clarification incorporated:** `2026-08-08`

## 1. Purpose

This plan restructures reopened Part 0 into explicit work packages so that no downstream Part can legitimately interpret FSATS V1.4 more narrowly than the reviewed design.

Part 0 is neither a preservation-only migration nor a greenfield redesign.

Every work package SHALL:

1. apply Falcon Vision and Constitution constraints;
2. apply current governing Falcon/Foundation rules relevant to the subject;
3. identify the Owner objective and explicit Owner decisions;
4. review relevant FSATS V1.3 historical design sources;
5. understand the problem/outcome addressed by the V1.3 approach;
6. evaluate credible alternatives;
7. select the strongest justified V1.4 design;
8. record every material difference from V1.3 and its rationale;
9. map Foundation dependencies/FCRs separately from Application-owned design; and
10. define downstream implementation and verifier/evidence obligations.

FSATS V1.3 is `HISTORICAL_DESIGN_REFERENCE`. It is mandatory review input for completeness and prior knowledge, but is not binding current authority and has no veto over a better justified V1.4 solution.

## 2. Common V1.3 disposition vocabulary

Every material V1.3 item reviewed SHALL receive one explicit disposition:

- `RETAINED`
- `IMPROVED`
- `MODIFIED_FOR_CURRENT_ARCHITECTURE_ALIGNMENT`
- `REPLACED_BY_BETTER_DESIGN`
- `REMOVED_WITH_JUSTIFICATION`
- `OWNER_DIRECTION`
- `OWNER_DECISION_REQUIRED`

Silence is not a disposition.

## 3. Common review lifecycle

Every P0 work package uses:

```text
DRAFT
→ DESIGN DEVELOPMENT
→ ARCHITECTURE / CONSISTENCY REVIEW
→ RED-TEAM REVIEW
→ FINDINGS
→ REMEDIATION
→ FRESH RE-REVIEW OF THE CHANGED VERSION
→ POST-CHANGE REPORT
→ OWNER REVIEW CANDIDATE
→ OWNER REVIEW
```

If Owner review requests a semantic change, that review is conditional. The change SHALL be applied, then a fresh Architecture/Consistency review and fresh Red-Team review SHALL be performed before a new report is sent for Owner final review.

Technical PASS, Red-Team PASS, repository commit, silence, or conditional approval does not equal final Owner acceptance.

---

# P0-A - Authority, Source-of-Truth, Reference and Planning Baseline

## Objective

Establish the complete governing-source model, historical-reference model, review evidence lock, and planning lifecycle before any later architecture reconciliation is accepted.

## Mandatory sources

- Falcon Vision.
- Ratified Falcon Constitution.
- Applicable current governance records.
- Applicable effective standards/specifications/accepted ADRs/approved contracts.
- Current accepted Foundation boundaries and separately current implementation/evidence state.
- Explicit Owner directions and corrections.
- Complete FSATS V1.3 historical package inventory and relevant source artifacts.
- Historical V1.3 validation evidence where applicable.
- Canonical FCR workflow and freshly verified relevant FCR dispositions.

## Required output

A canonical source/register model that records, where material:

- artifact identity;
- canonical path;
- version/status;
- repository and branch;
- reviewed commit/snapshot;
- blob/digest where available;
- role/jurisdiction;
- authority vs evidence classification; and
- freshness requirement.

P0-A SHALL also define Owner authority handling, V1.3 historical-reference treatment, better-design rules, mandatory difference reporting, and the mandatory post-change re-review cycle.

## Exit gate

`P0-A PASS` only when every later Part 0 claim can identify its governing constraints and evidence sources, no draft/superseded artifact is silently used as authority, V1.3 has no unintended veto authority, and the review lifecycle cannot convert a conditional Owner approval into final acceptance.

---

# P0-B - Complete V1.3 Review, Difference and Disposition Ledger

## Objective

Review the full V1.3 historical design as a controlled knowledge source, prevent omission-based loss, and record how V1.4 treats every material item.

## Mandatory treatment

Every material V1.3 architectural/operational concept SHALL be reviewed exactly once and assigned one disposition from Section 2.

For every material difference, record:

```text
V1.3 source
→ V1.3 approach/problem addressed
→ V1.4 proposed approach
→ difference
→ reason
→ Vision/Constitution assessment
→ current Falcon/Foundation assessment
→ expected benefit
→ trade-offs
→ affected downstream Part
```

## Required coverage

At minimum:

- application topology;
- Guardian architecture;
- FSAPMA architecture;
- provider pool and role separation;
- Provider Controller and provider selection;
- operational-data gateway rule;
- provider quality/reconciliation/capacity/free-first rules;
- broker/provider separation;
- markets and market profiles;
- account/broker truth;
- horizons and immutable intent;
- frameworks, schools and strategies;
- risk/capital/decision architecture;
- Fast Track and latency protections;
- execution/positions/reconciliation;
- learning/analytics/evolution;
- FSTSimA and validation stages;
- Web/Communication boundaries;
- MSA/LSA/CSA locality;
- provenance/evidence/replay;
- historical Owner corrections and subsequent Owner design directions.

## Exit gate

Zero material V1.3 items left without review/disposition; zero omission-based deletion; and zero claim that V1.3 must be retained merely because it existed previously.

---

# P0-C - Application Topology, Ownership and Awareness Jurisdiction

## Objective

Make ownership and jurisdiction non-ambiguous while permitting improvement where a stronger current design is justified.

## Mandatory questions to derive and verify

- Is FSATS correctly a non-owning trading-system/domain boundary rather than an Application?
- Should Trading Guardian, FSAPMA and Trading remain independent Applications under current Falcon architecture?
- Should FSTSimA remain an independent non-Live validation Application?
- Should Shared Web and Communication remain independent adjacent Applications?
- Is exactly one MSA per Application still correct?
- Is exactly one LSA per declared major branch still correct?
- Is CSA limited to eligible intelligent components?
- Does awareness rank remain non-authoritative across Application boundaries?
- Is there any hidden FSATS owner, shared runtime owner, shared resource owner, or hidden authority surface?

The current 4 Guardian + 6 FSAPMA + 12 Trading topology and FSTSimA 1 + 8 topology are historical design candidates to be reviewed, not automatically preserved.

## Required output

Canonical proposed ownership map for Applications, MSAs, major branches, LSAs and CSA eligibility, plus an explicit V1.3 difference report.

## Exit gate

Every architectural object has exactly one owner/home and no cross-boundary access can be inferred from FSATS containment or awareness rank.

---

# P0-D - Foundation Alignment and Anti-Reimplementation Boundary

## Objective

Map every Foundation-facing requirement to current accepted Foundation semantics and current capability evidence without reimplementing Foundation inside Applications.

## Mandatory alignment areas

- APP-001 Application boundary/lifecycle;
- CON-023 Application Contract/Manifest and current Stage 5 communication semantics;
- Guardian/Foundation protection boundaries where accepted;
- ADR-I012 generic Application integration;
- ADR-I015 awareness model;
- SYS-006 resource governance;
- Foundation contracts/schema registry;
- communication/routing/admission capability state;
- resource escalation/pressure semantics;
- evidence/replay/event capability state;
- research-only Internet egress capability state;
- simulator/non-Live isolation capability state.

## Required disposition per dependency

- `FOUNDATION_CAPABILITY_ACCEPTED`
- `FOUNDATION_CAPABILITY_PARTIAL`
- `FOUNDATION_CAPABILITY_MISSING`
- `APPLICATION_OWNED`

Confirmed missing/partial/incompatible Foundation behavior SHALL map to a canonical FCR and remain fail-closed.

## Exit gate

No Application artifact invents Foundation routing, resource, lifecycle, security, admission, or platform-authority semantics.

---

# P0-E - Canonical Application Identity, Manifest and Lifecycle Design Contract

## Objective

Eliminate ambiguity that could let a downstream implementation substitute a partial Foundation binding for a complete Application Manifest.

## Mandatory output per Application

For Trading Guardian, FSAPMA and Trading, define the proposed complete CON-023-compliant manifest obligations, including at minimum:

- canonical Application ID;
- canonical package ID and one naming form only;
- suite/domain identity;
- version and owner;
- technical identity;
- package integrity/provenance;
- Foundation dependencies/services/contracts;
- communication declarations;
- permissions/authority requests;
- resources;
- storage/persistence;
- lifecycle declarations;
- health/degraded/failure containment;
- security;
- recovery/isolation/restart/failover/safe-shutdown;
- MSA/major branches/LSAs/CSA eligibility;
- Guardian requirement/interface semantics;
- upgrade/rollback/removal/uninstallation;
- evidence and compatibility.

## Exit gate

A downstream implementer can construct each complete manifest without inventing fields, identities, defaults or authority.

---

# P0-F - Cross-Application Contract and Information-Flow Contract

## Objective

Convert every required inter-Application relationship into an explicit bounded contract obligation.

## Mandatory core relationships to review

- Trading ↔ FSAPMA;
- Guardian ↔ FSAPMA;
- Guardian ↔ Trading;
- FSATS-facing relationships with Shared Web Application;
- FSATS-facing relationships with Communication Application;
- FSTSimA interfaces where validation requires them.

## Every contract family SHALL declare

- producer/requester;
- consumer/responder;
- purpose;
- message/data product type;
- schema/version rule;
- permission/authority reference;
- correlation/causation/provenance/evidence;
- idempotency/retry/correction where applicable;
- failure/degraded behavior;
- security classification;
- deadline/freshness semantics where applicable;
- relevant FCR dependencies;
- explicit statement that declaration does not grant runtime route authority.

## Exit gate

No cross-Application behavior depends on an informal arrow, shared memory, awareness rank, or direct internal coupling.

---

# P0-G - FSAPMA Operational-Data Architecture Review

## Objective

Review the complete V1.3 FSAPMA design and derive the strongest current operational-data architecture instead of reducing the problem to a simplified provider list.

## Mandatory concepts to review

- FSAPMA as trading operational external-data gateway;
- Provider Groups;
- complete historical Initial Provider Pool and role separation;
- endpoint/plan/entitlement-level capability knowledge;
- Data Service Contracts / canonical data products;
- Provider Controller;
- dynamic provider selection by product/capability/quality/freshness/latency/quota/capacity/cost/entitlement/health;
- free-first/no automatic paid fallback;
- provider onboarding/certification;
- quota/rate/concurrency/capacity management;
- protected operational reserve within admitted Application resources;
- data quality, normalization, provenance and conflict handling;
- cross-provider verification and canonical-source semantics;
- caching/backfill/deduplication where permitted;
- explicit degraded/NACK behavior, never fabricated data;
- provider-native formats and normalization boundaries;
- awareness Internet research-only separation;
- provider/broker role separation even when one external company supplies both.

Each item may be retained, improved, aligned, replaced or removed only under the explicit disposition/difference rules.

## Foundation alignment rule

FSAPMA owns provider/business logic only. It SHALL NOT own Foundation network quota, total resource allocation, Service Bus authority, Application admission, or platform lifecycle.

## Exit gate

All material V1.3 FSAPMA concepts are reviewed and explicitly dispositioned, and every accepted V1.4 responsibility has a downstream home and verifier obligation.

---

# P0-H - Trading Core Design Review

## Objective

Review the complete V1.3 Trading Application business architecture and derive the strongest current V1.4 Trading design without implementation in Part 0.

## Mandatory concepts to review

- initial US Equities + Crypto Spot scope;
- initial leverage/exposure 1:1;
- market profiles and broker-scoped effective universe;
- dynamic universe;
- fractional eligibility where supported;
- account/custodian/currency truth;
- deterministic horizons and activation grants;
- immutable Trading Intent and lifecycle semantics;
- frameworks → schools → strategies;
- central Strategy Catalog/Controller;
- Unified Risk;
- trading Capital Reservation Ledger separate from Foundation resources;
- final decision and dispatch revalidation;
- execution/position/reconciliation truth;
- learning/analytics/strategy evolution;
- evidence/provenance/replay;
- correction/compensation/idempotency;
- Paper/Tiny Live/Live as business authorization stages, not Foundation lifecycle states.

## Exit gate

Every material V1.3 Trading concept is reviewed/dispositioned and every resulting V1.4 responsibility maps to exactly one later Part with no contradictory duplicate owner.

---

# P0-I - Guardian, Crisis, Protection and Resource-Escalation Design

## Objective

Review Trading Guardian semantics and align platform-facing actions to current Foundation authority.

## Mandatory concepts to review

- Guardian is trading-scoped and outside Foundation;
- WARNING / RESTRICTED / SAFE MODE / RECOVERY;
- open-position monitoring/advisory behavior in Safe Mode;
- missing stop-loss as potentially catastrophic while ordinary stop-loss execution is normal;
- bounded Guardian domain authority;
- no direct control of another Application or Foundation mode without governed contracts;
- Foundation protection/resource requests through governed boundaries only;
- resource escalation is never resource seizure;
- explicit protection-command/status contracts;
- missing Foundation capability remains FCR/fail-closed.

## Exit gate

No Guardian wording can be interpreted as Foundation authority or hidden direct cross-Application control.

---

# P0-J - Fast Track, Performance, Priority and Load-Shedding Design

## Objective

Review and improve performance architecture without allowing speed to bypass governance or safety.

## Mandatory concepts to review

- data plane vs control/awareness plane separation;
- MSA/LSA/CSA off synchronous hot path;
- immutable/precomputed snapshots where justified;
- end-to-end deadline propagation;
- bounded queues;
- capital-protection priority before discovery/research;
- load shedding of research/discovery/analytics before protection/reconciliation;
- p50/p95/p99/p99.9/max observability;
- external vs internal latency separation;
- non-HFT internal latency targets as empirical targets, not guarantees;
- dispatch rechecks authority, Risk, Guardian state, grant validity, freshness, economics and slippage;
- explicit FCR dependencies for transport QoS and Foundation pressure signals.

## Exit gate

No performance optimization implies authority bypass, safety bypass, silent degradation, or unsupported QoS guarantees.

---

# P0-K - Validation, FSTSimA and Environment/Authority Separation

## Objective

Review the validation architecture and guarantee simulation evidence cannot become production authority.

## Mandatory concepts to review

- FSTSimA independent non-Live Application candidate;
- Backtest → Replay → Stress → Shadow → Paper → Tiny Live → Scale progression;
- simulation environment, execution mode, evidence stage, business authorization state and Foundation lifecycle state as separate dimensions;
- replay/test/simulation cannot cause Live effects;
- Paper provider registration separate from execution truth/live authority;
- no paid provider/service purchase authority implied;
- non-Live isolation/egress gap mapping to FCR-0011 or later accepted capability.

## Exit gate

No test, replay, Shadow or Paper state can accidentally satisfy Live authority or Foundation lifecycle requirements.

---

# P0-L - End-to-End Traceability, Non-Ambiguity and Red-Team Closure Gate

## Objective

Prove the corrected Part 0 package cannot again be interpreted narrowly by Part 1 or a later Part.

## Mandatory traceability matrix

Every material resulting V1.4 requirement SHALL map:

```text
Owner Objective / Vision / Constitution / Governing Constraint
→ V1.3 reference where applicable
→ V1.3 disposition and difference record
→ V1.4 proposed/accepted requirement
→ owning Application / room / boundary
→ Foundation dependency or Application ownership
→ target implementation Part
→ required implementation artifact
→ required verifier/evidence gate
```

## Mandatory negative checks

The final Red-Team SHALL attempt to prove at minimum that Part 0 could be misread to allow:

- incomplete CON-023 manifests;
- inconsistent Application/package identities;
- informal cross-App arrows;
- awareness jurisdiction leakage;
- hidden FSATS ownership;
- Foundation logic reimplementation;
- provider-list simplification without review of historical roles;
- operational data bypassing the selected FSAPMA boundary;
- provider/broker truth conflation;
- trading capital/Foundation resource conflation;
- performance bypass of safety/governance;
- simulation/Paper implying Live authority;
- omission-based deletion of unreviewed V1.3 knowledge;
- V1.3 being treated as an unintended veto authority;
- conditional Owner approval being mistaken for final acceptance;
- verifier success despite an accepted requirement having no test.

## Closure rule

Part 0 SHALL NOT be eligible for Owner final re-acceptance until:

- P0-A through P0-K each pass their current-version reviews;
- every material resulting requirement is traceable;
- no unresolved Critical ambiguity remains;
- every later-Part obligation has an explicit home;
- every known Foundation gap has a canonical FCR or accepted capability;
- the final Red-Team concludes that the corrected Part 0 package cannot legitimately produce the prior incomplete downstream interpretation.

---

# Execution order

```text
P0-A  Authority / Source / Reference / Planning Baseline
  ↓
P0-B  Complete V1.3 Review / Difference / Disposition Ledger
  ↓
P0-C  Topology / Ownership / Awareness
  ↓
P0-D  Foundation Alignment
  ↓
P0-E  Manifest / Identity / Lifecycle
  ↓
P0-F  Cross-App Contracts
  ↓
P0-G  FSAPMA Operational-Data Architecture
  ↓
P0-H  Trading Core Design Review
  ↓
P0-I  Guardian / Protection
  ↓
P0-J  Fast Track / Performance
  ↓
P0-K  Validation / FSTSimA / Authority Separation
  ↓
P0-L  Traceability + Final Architecture Review + Red-Team
```

Evidence collection may occur in parallel where it does not bypass upstream design dependencies. No package is considered accepted/closed until its current version has completed the required review cycle and received explicit Owner final acceptance.

## Current status

- P0-A: `DESIGN_REMEDIATION_AND_POST_CHANGE_REVIEW`
- P0-A Final Owner Acceptance: `NOT_YET_GRANTED`
- P0-B: `NOT_STARTED`
- P0-C: `NOT_STARTED`
- P0-D: `NOT_STARTED`
- P0-E: `NOT_STARTED`
- P0-F: `NOT_STARTED`
- P0-G: `NOT_STARTED`
- P0-H: `NOT_STARTED`
- P0-I: `NOT_STARTED`
- P0-J: `NOT_STARTED`
- P0-K: `NOT_STARTED`
- P0-L: `NOT_STARTED`

Part 1 is frozen pending corrected Part 0 Owner re-acceptance.  
Part 2 through Part 10 remain unauthorized.
