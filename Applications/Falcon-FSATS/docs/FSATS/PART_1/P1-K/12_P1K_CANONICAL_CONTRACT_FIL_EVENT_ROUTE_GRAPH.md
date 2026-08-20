# P1-K — Canonical Governed Contract, FIL, Event and Route Graph

**Status:** `DESIGN_MATERIALIZATION / OWNER_DIRECTED_COMPLETION_CYCLE`  
**Scope:** `P1-K DECLARATION ONLY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Route Activation:** `NOT_GRANTED`

## 1. Baseline and Delta Rule

The accepted Part 0 `43/43` contract baseline remains immutable historical accepted evidence. P1-K does not rewrite or renumber it.

This file is the canonical Part 1 prospective contract-delta declaration set. The current Part 1 delta contains **22 semantic families**. Implementation may split a semantic family into transport/schema artifacts only if semantic identity and ownership remain traceable and no authority is expanded.

```text
PART0_ACCEPTED_FAMILIES = 43 historical
PART1_PROSPECTIVE_DELTA_FAMILIES = 22
CURRENT_DESIGN_GRAPH = historical baseline by reference + explicit Part1 delta
```

## 2. Universal Envelope Requirements

Every governed family SHALL bind as applicable:

- immutable family ID and schema version;
- exact producer Application/role and exact consumer Application/role;
- producer instance identity where runtime attribution requires it;
- payload/business owner;
- authority class;
- security classification;
- FIL envelope identity/version;
- route/delivery binding identity when applicable;
- message/event identity;
- correlation and causation identity;
- producer observation/effective timestamps and freshness/expiry/deadline as applicable;
- idempotency identity where retriable;
- ordering/duplicate/correction semantics;
- operational/replay/test/synthetic classification;
- evidence/provenance references;
- acceptance/rejection/outcome semantics;
- fail-closed behavior.

Universal invariants:

```text
DELIVERY != ACCEPTANCE
REQUEST != AUTHORIZATION
ROUTE_EXISTS != AUTHORITY
REPLAY != OPERATIONAL
UNKNOWN != SUCCESS
STALE != CURRENT
```

## 3. Canonical Part 1 Delta Families

### P1K-001 — FSAPMA Operational Data Delivery
Producer: FSAPMA. Consumers: declared FSATS Applications requiring the exact data product, primarily Trading. Authority class: `OPERATIONAL_DATA_PROJECTION`. Payload owner: FSAPMA. Includes producer instrument identity, data-product/schema version, provenance, observation time, freshness, quality/conflict state and correction lineage. Trading maps to its own instrument identity.

### P1K-002 — FSAPMA Data Quality / Correction Event
Producer: FSAPMA. Consumers: affected declared consumers. Authority class: `DATA_QUALITY_EVIDENCE`. Corrections preserve original observation identity and never silently overwrite history.

### P1K-003 — Trading Decision / Intent Evidence Projection
Producer: Trading. Consumers: declared Guardian/analytics/evidence consumers where needed. Authority class: `EVIDENCE_ONLY`. A Trading decision projection is not broker submission authority and cannot be replayed as an order.

### P1K-004 — Trading Exposure / Order / Position Safety Projection
Producer: Trading. Consumers: Guardian and authorized Web projection path. Authority class: `AUTHORITATIVE_APPLICATION_STATE_PROJECTION`. Includes explicit order/position/protection/reconciliation-needed state without granting recipient execution ownership.

### P1K-005 — Guardian Protection Restriction / Command
Producer: Guardian. Consumer: exact target FSATS Application, primarily Trading. Authority class: `PROTECTION_COMMAND`. Includes authority basis, target scope, command type, lease/expiry where applicable, causation, idempotency and explicit target acceptance/rejection semantics. FCR-0004 implementation verification remains deferred.

### P1K-006 — Protection Command Outcome
Producer: target Application. Consumer: Guardian. Authority class: `COMMAND_OUTCOME`. Distinguishes received/accepted/rejected/applied/partially-applied/expired/revoked/reconciliation-required. `REQUEST_SENT != ACTION_COMPLETED`.

### P1K-007 — Application Safety/Incident Evidence
Producer: any FSATS Application. Consumer: Guardian and other declared evidence sinks where authorized. Authority class: `INCIDENT_EVIDENCE`. Evidence does not grant Guardian business ownership.

### P1K-008 — Constituent Resource Evidence Submission
Producer: Trading, FSAPMA, Guardian or FSTSimA. Consumer: APP-RSC. Authority class: `RESOURCE_CLAIM_EVIDENCE`. Includes current allocation reference, consumption, minimum-safe claim, desired capacity, pressure/urgency, reclaimability, degradation options, starvation consequence, freshness and evidence.

### P1K-009 — APP-RSC Effective Coordination Outcome
Producer: APP-RSC. Consumer: exact constituent FSATS Application. Authority class: `FSATS_RESOURCE_COORDINATION`. Binds coordinator epoch, current Foundation envelope reference, target resource class/amount/action, effective/expiry state and policy basis. Cannot mutate Foundation-authoritative truth.

### P1K-010 — Resource Coordination Acknowledgement / Result
Producer: constituent Application. Consumer: APP-RSC. Authority class: `COORDINATION_OUTCOME`. Distinguishes accepted/rejected/partially-applied/deferred/failed/reconciliation-required and reports resulting consumption/degraded state.

### P1K-011 — APP-RSC Residual Resource Request
Producer: APP-RSC. Consumer: Foundation resource-governance boundary. Authority class: `RESOURCE_REQUEST`. Includes per-Application attribution, internal optimization/reclaim evidence, remaining deficit and APP-RSC own transparent coordination overhead. Request never equals grant.

### P1K-012 — Foundation Resource Authority Outcome
Producer: Foundation authoritative resource boundary. Consumer: APP-RSC. Authority class: `AUTHORITATIVE_FOUNDATION_OUTCOME`. Carries only Foundation-defined grant/partial/cap/deny/reduce/revoke/reclaim/rebalance/restore semantics. APP-RSC cannot fabricate or reinterpret absent authority.

### P1K-013 — APP-RSC Coordination Epoch / Fencing Projection
Producer: APP-RSC. Consumers: declared constituent/Guardian/Web evidence consumers as needed. Authority class: `COORDINATION_STATE_EVIDENCE`. Exposes current epoch, trust/fencing state and envelope freshness without exposing internals.

### P1K-014 — FSTSimA Scenario / Simulation Input Request
Producer: authorized FSATS Application. Consumer: FSTSimA. Authority class: `NONLIVE_SIMULATION_REQUEST`. Must be explicitly non-operational and cannot contain reusable Live execution authority.

### P1K-015 — FSTSimA Validation / Qualification Evidence
Producer: FSTSimA. Consumers: Trading/Guardian/FSAPMA/Owner-facing governed projection as applicable. Authority class: `NONLIVE_VALIDATION_EVIDENCE`. Includes scenario/version/seed/provenance/fidelity/limitations/readiness recommendation. `READINESS != AUTHORITY`.

### P1K-016 — AI / Awareness Integrity Incident Projection
Producer: Application Awareness/authorized monitor path. Consumer: Application MSA/Guardian and future Foundation FSA interface where governed. Authority class: `INTEGRITY_EVIDENCE`. Exact MSA->FSA runtime binding remains subject to FCR-0030/Foundation Stage 13; absence of that future binding fails closed for the affected future handoff rather than creating a local substitute.

### P1K-017 — AI Containment / Kill State Projection
Producer: authoritative owner of the actual containment state for the affected scope. Consumers: affected Application, Guardian and authorized Web status projection. Authority class: `SAFETY_STATE`. Must identify target, scope, revoked trust/authority, preserved functions, effective time and recovery status. Projection does not create Kill authority in the recipient.

### P1K-018 — Controlled Revival Evidence / Decision Request
Producer: affected Application recovery path. Consumer: the separately governed approval authority applicable to R2/R3 and authorized Web status projection where applicable. Authority class: `RECOVERY_EVIDENCE_OR_REQUEST`. `REPAIRED != TRUSTED`, `TESTED != RELEASED`.

### P1K-019 — Shared Web Informational Query / Response
Producer/consumer: Shared Web <-> exact FSATS Application. Authority class: `INFORMATIONAL_QUERY_RESPONSE`. Queries cannot be reinterpreted as business commands. Responses expose authoritative/current vs last-known/stale/unknown classification.

### P1K-020 — Shared Web Owner Command Request / Application Outcome
Producer: Shared Web as authenticated request surface. Consumer: exact authorized FSATS target Application/Guardian boundary. Return path: target -> Web. Authority class: `OWNER_REQUEST_TRANSPORT`, not Web-owned authority. Includes authenticated Owner/authority reference, target, requested operation, idempotency and explicit outcome. Backend unavailable -> fail closed; Web cannot simulate success.

### P1K-021 — FSATS Application -> Foundation Information / Evidence / Capability Query
Producer: exact FSATS Application. Consumer: exact Foundation generic communication/capability boundary. Authority class: `FOUNDATION_QUERY_OR_EVIDENCE_SUBMISSION`. Does not grant direct Foundation-internal access.

### P1K-022 — Foundation -> FSATS Application Authoritative Event / Decision / Query
Producer: exact Foundation authoritative boundary. Consumer: exact FSATS Application. Authority class: `FOUNDATION_AUTHORITATIVE_EVENT_OR_REQUEST`. Consumer validates identity/authority/version/freshness and returns explicit acknowledgement/outcome when the Foundation contract requires it.

## 4. Security and Credential Rule

No P1-K family carries reusable secret bytes in ordinary payloads. Credential references, when a capability requires them, use separately governed opaque references/identities and never create authority by presence.

Web does not own provider/broker secrets. APP-RSC resource evidence never contains secret material. Replay/simulation families cannot reuse operational credentials or execution authority.

## 5. AI Kill / Fencing Rule

Every risk-increasing command/decision/coordination family that may originate from intelligent processing SHALL be attributable to a trust/decision/coordination epoch or equivalent causation identity sufficient to fence queued/cached/scheduled stale work when that intelligence becomes untrusted.

Protective work independently valid under a trusted safety envelope is not cancelled solely because unrelated intelligent work is fenced.

If a message may already have crossed the external boundary before containment, the recipient/owner reconciles actual outcome rather than assuming cancellation.

## 6. Ordering, Duplicate and Correction Rule

Ordering is defined per aggregate/entity, not assumed globally. Duplicate delivery is safe only when the family defines idempotency. Corrections are new attributable records linked to the corrected identity. A late older state cannot silently overwrite a newer current state.

## 7. Route Admission Rule

A declared family is not an activated route. Runtime route creation requires separately authorized implementation/admission/security configuration and executable verification.

No producer may dynamically invent a new consumer or authority class outside the accepted declaration set.

## 8. FCR Reconciliation

- FCR-0080 generic external communication boundary: this declaration supplies the exact Application-side producer/consumer/authority/freshness/causation/fail-closed model required for final design compatibility verification.
- FCR-0004/0005/0006 remain open implementation holds until code/routes/fixtures exist.
- FCR-0030 remains a future Foundation Stage 13 binding hold for exact MSA->FSA runtime interface.
- FCR-0031 remains an implementation/binding hold for APP-RSC exact resource code/fixtures.

## 9. P1-K Closure Invariants

- Part 0 43/43 history remains untouched;
- all Part 1 cross-App/external semantic deltas are explicit and counted;
- one canonical declaration set owns current Part 1 delta semantics;
- producer/consumer/payload owner/authority class are explicit;
- FIL/route implementation cannot silently create new business authority;
- stale/replay/synthetic/unknown traffic cannot become current operational authority;
- Web remains request/presentation surface, not business/Foundation authority;
- Foundation remains external authoritative owner of its platform decisions;
- no runtime route is activated by design closure.
