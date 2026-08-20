# FSATS Part 1-NG — Work Package Decomposition

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

This decomposition is under active semantic remediation. The Owner-directed FSARM model supersedes the prior future-facing TARC-only resource-management assumption. Any previous Part 1 freeze/review evidence predating this change is historical for the changed scope.

---

## P1-A — Authority, Baseline, Historical Compatibility and Scope Lock

### Objective
Establish the exact current authority/evidence set and prevent historical or stale implementation assumptions from contaminating current planning.

### Required outputs
- current Part 0 accepted-freeze bindings;
- current Owner-decision inventory including the prospective FSARM correction;
- current Foundation/FCR snapshot;
- historical Part 1 compatibility inventory at artifact level;
- classification of each historical artifact as `REUSABLE_BY_PROOF`, `REFERENCE_ONLY`, `SUPERSEDED`, or `INCOMPATIBLE`;
- branch/path/write scope lock;
- explicit implementation/runtime non-authority record;
- explicit record that accepted Part 0 history is preserved while later Owner semantic corrections are carried prospectively in Part 1.

### Closure criteria
No current build plan depends on an unverified historical artifact, stale Foundation state, remembered identity, implied authority, or superseded TARC-only future assumption.

---

## P1-B — Foundation Integration Architecture, Capability and FCR Baseline

### Objective
Establish the Foundation-side contracts, boundaries, capability state and fail-closed behavior before physical build topology is closed.

### Required outputs
- Foundation integration profile for each FSATS Application and FSARM;
- APP-001 / CON-023 / ADR-I012 / ADR-I015 binding matrix;
- lifecycle/admission/catalog/dependency-governance bindings;
- FIL / Service Bus / event / evidence / security bindings;
- resource-governance binding including FCR-0031;
- current design-time, build-time, runtime-capability and runtime-authority state for each dependency;
- current FCR mapping and review triggers;
- fail-closed behavior for every unresolved or unavailable Foundation dependency;
- explicit prohibition of Application-local Foundation substitutes.

### Closure criteria
No physical Application/project/controller design depends on an unknown Foundation counterpart, invented capability or unresolved authority-bearing binding without an explicit FCR/fail-closed gate.

---

## P1-C — Repository, Solution, Project and Package Topology

### Objective
Define the physical build structure only after the governing Foundation integration surfaces are known.

### Required design decisions
- exact solution/workspace structure;
- independently identifiable/buildable top-level Application boundaries;
- exact structural home for FSARM without silently making the non-owning FSATS boundary an Application or hidden runtime principal;
- project/package naming and stable ownership;
- dependency direction rules;
- public versus internal project boundaries;
- shared Application-owned libraries only where genuine semantic commonality exists;
- prohibition of `FSATS` as an ungoverned runtime owning project/principal;
- no direct project reference that bypasses a governed cross-Application contract;
- no Foundation source copying.

### Required outputs
- canonical project/package tree;
- project ownership matrix;
- allowed dependency matrix;
- forbidden dependency matrix;
- package/versioning rules;
- replacement/removal impact map;
- FSARM structural placement decision contingent on Foundation reconciliation where required.

### Closure criteria
Every future source file has an unambiguous owning project/package and no topology creates hidden cross-Application or Foundation authority.

---

## P1-D — Canonical Application-Owned Primitives and Structural Types

### Objective
Define reusable Application-owned primitives without duplicating Foundation-owned semantics.

### Required rule
For every candidate primitive:

```text
FOUNDATION_OWNED_SEMANTIC?
 -> CONSUME FOUNDATION SEMANTIC

APPLICATION_BUSINESS_OWNED?
 -> DEFINE APPLICATION PRIMITIVE

DOMAIN_WRAPPER_AROUND_FOUNDATION_ID?
 -> ALLOW ONLY WITH EXPLICIT MAPPING
```

### Candidate Application-owned families
- user/account/market/instrument/provider/broker/environment/domain identifiers where Application-owned;
- money/quantity/price/exposure representations with exact unit/currency semantics;
- confidence/quality/fitness representations where Application-owned;
- bounded result/reason categories;
- FSARM resource-intent/value types only where not owned by Foundation, including Application-local minimum-safe, desired, reclaimable and workload-priority evidence representations.

### Forbidden scope
- reimplementation of Foundation identity, FIL, Service Bus, Manifest, security, lifecycle, total-resource or event-system ownership;
- cloning Foundation time/correlation/causation/evidence semantics under new names;
- one “common” type that collapses distinct business meanings merely because storage shape matches.

### Closure criteria
All shared primitives have one owner, exact semantics, serialization rules, invalid-state rules, equality rules and negative fixtures.

---

## P1-E — Application Identity, Manifest and Lifecycle Materialization

### Objective
Convert current topology declarations and later Owner-directed corrections into exact materialization rules compliant with APP-001 and CON-023.

### Required outputs per Application
- immutable Application identity strategy;
- package identity/version/provenance strategy;
- purpose and owned business boundary;
- exact MSA identity;
- exact LSA identity set;
- optional CSA eligibility declarations;
- provided/consumed capability declarations;
- Foundation dependency declarations;
- permissions/security profile declarations;
- resource requirement/minimum/ceiling/degraded behavior declarations;
- FSARM interaction declarations for resource need, minimum-safe requirement, pressure, reclaimability, allocation outcome and restoration evidence;
- persistence/configuration/evidence requirements;
- lifecycle/rollback/removal declarations;
- Guardian/protection interface declarations;
- origin-aware self-development declarations.

### FSARM materialization requirement
Part 1 SHALL determine the exact governed identity/manifest/binding model for FSARM after Foundation reconciliation under FCR-0031. It SHALL NOT invent a hidden FSATS runtime principal.

### Foundation gate
Canonical authority-bearing Foundation identity/materialization SHALL remain fail closed where exact current Foundation identity or artifact consumption cannot yet be resolved.

### Closure criteria
Every Application and FSARM can be represented by a complete, internally consistent identity/binding design without inventing Foundation fields, permissions or authority.

---

## P1-F — Trading Application 13-LSA Implementation Decomposition

### Objective
Create a code-ready decomposition of the Falcon Self-Aware Trading Application while preserving exact Trading ownership and the new FSARM separation.

### Required branch coverage
1. T-LSA-01 Operations, Account & Environment
2. T-LSA-02 Market & Instrument Universe
3. T-LSA-03 Analysis Frameworks
4. T-LSA-04 Classical Trading School
5. T-LSA-05 Opportunity Hunting School
6. T-LSA-06 Strategy Orchestration & Decision
7. T-LSA-07 Unified Risk Management
8. T-LSA-08 Portfolio & Capital Management
9. T-LSA-09 Execution & Position Lifecycle
10. T-LSA-10 Trading Learning & Knowledge
11. T-LSA-11 Trading Analytics & Attribution
12. T-LSA-12 Strategy Evolution & Experimentation
13. T-LSA-13 Trading Resource Management

### Resource separation

```text
T_LSA13 = TRADING_RESOURCE_AWARENESS_AND_EVALUATION
T_LSA13 != FSARM
```

T-LSA-13 SHALL understand Trading-side current allocation, demand, pressure, minimum-safe requirement, reclaimability, shedding effects and additional need evidence, and shall report attributable Trading resource evidence to FSARM.

Trading MSA/LSAs/CSAs/components SHALL NOT independently bypass FSARM to request FSATS resource reallocation or additional Foundation resources.

### Required outputs
For every LSA: components, state ownership, internal interfaces, data inputs/outputs, concurrency model, failure/degraded behavior, security boundary, resource profile, tests and Foundation dependencies, plus exact Trading-to-FSARM evidence/decision interfaces.

### Closure criteria
No Trading responsibility is orphaned, duplicated, pushed into Guardian/FSAPMA/Foundation, or hidden behind a generic “Trading Engine” owner; T-LSA-13 remains awareness/evaluation while FSARM owns FSATS-wide operational resource coordination.

---

## P1-G — FSAPMA 6-LSA Implementation Decomposition

### Objective
Create the code-ready implementation architecture for the sole operational external-data/provider-management Application.

### Required branch coverage
1. P-LSA-01 Provider Registry and Onboarding
2. P-LSA-02 Data Products, Semantics and Normalization
3. P-LSA-03 Provider Capability, Account and Entitlement
4. P-LSA-04 Provider Selection, Routing and Delivery
5. P-LSA-05 Data Quality, Verification and Reconciliation
6. P-LSA-06 Quota, Capacity, Cost and Reliability

Provider Controller remains an operational controller inside P-LSA-04 and is not a CSA.

### FSARM requirement
FSAPMA SHALL expose attributable resource need, current consumption, minimum-safe live-data requirements, reclaimable/degradable workload, pressure and restoration evidence to FSARM without transferring provider/data business authority to FSARM.

### Required outputs
Provider registry model, Data Product model, capability/entitlement truth, routing/controller model, quality/reconciliation model, quota/capacity/cost model, internal state boundaries, FSARM resource interface, external-egress gate, delivery contract binding and complete verifier plan.

### Closure criteria
No provider-specific operational data path bypasses FSAPMA, and provider quota/capacity semantics never become Foundation technical-resource authority or FSARM business authority.

---

## P1-H — Trading Guardian 4-LSA Implementation Decomposition

### Objective
Create the code-ready independent protection/crisis Application architecture.

### Required branch coverage
1. G-LSA-01 Protection Observation and Incident Qualification
2. G-LSA-02 Protection Scope, Restriction and Command Governance
3. G-LSA-03 Crisis State, Survival and Protection Coordination
4. G-LSA-04 Reconciliation, Recovery and Protection Evidence

### FSARM crisis interaction
Guardian MAY provide attributable crisis/protection resource need, urgency, minimum-safe requirement and consequence-of-starvation evidence to FSARM.

Guardian SHALL NOT directly seize resources from another Application or become Foundation Resource Governance. FSARM performs any governed resource redistribution based on current policy/evidence.

During a crisis, FSARM may preferentially reallocate eligible resources toward Guardian protection obligations when this is justified by current consequence-aware evidence and protected minimums.

### Required outputs
Protection-state model, incident model, command/directive model, expiry/scope/idempotency rules, crisis state machine, recovery evidence, self-health/fail-safe behavior, exact FSARM resource interaction, exact cross-App contract bindings and negative authority tests.

### Closure criteria
Guardian cannot become Trading Risk, execution truth, provider truth, FSARM, Foundation resource authority or a general Application supervisor.

---

## P1-I — FSTSimA 8-LSA Implementation Decomposition

### Objective
Create the code-ready non-Live simulation and validation Application architecture.

### Required branch coverage
1. S-LSA-01 Simulation Time and Scenario
2. S-LSA-02 Market Environment Simulation
3. S-LSA-03 Provider and External Service Simulation
4. S-LSA-04 Broker, Exchange and Execution Simulation
5. S-LSA-05 Account, Capital and Settlement Simulation
6. S-LSA-06 Fault, Latency and Crisis Injection
7. S-LSA-07 Fidelity and Calibration
8. S-LSA-08 Oracle, Evidence, Reproducibility and Validation Assessment

### Required separation
```text
S-LSA-07 = FIDELITY_MEASUREMENT_AND_CALIBRATION
S-LSA-08 = INDEPENDENT_ASSESSMENT_OF_FIDELITY_AND_OVERALL_VALIDATION_EVIDENCE
```

### FSARM reclaimability requirement
FSTSimA SHALL explicitly declare current minimum-safe resource floor, pause/degradation semantics, reclaimable capacity and restoration rules. Non-live simulation/experimentation capacity may be highly reclaimable when it is not required for an active higher-priority obligation.

Example design intent:

```text
GUARDIAN_CRISIS_NEED
+ FSTSIMA_RECLAIMABLE_CAPACITY
 -> FSARM MAY REDUCE/PAUSE ELIGIBLE FSTSIMA WORK
 -> REALLOCATE EXISTING CAPACITY TO GUARDIAN
```

This is resource control only and does not allow Guardian or FSARM to alter simulation evidence or validation truth.

### Required outputs
Scenario model, deterministic/reproducible time model, simulator interfaces, oracle/evidence model, fidelity/calibration model, fault injection model, replay classification, FSARM resource/degradation/restoration interface and non-Live isolation requirements.

### Closure criteria
Simulation evidence cannot become Live authority, promotion authority or hidden operational traffic, and resource reclamation cannot corrupt accepted evidence truth.

---

## P1-J — FSARM: FSATS-Wide Resource Management and Foundation Resource Binding

### Objective
Materialize FSARM as the single FSATS-wide operational resource-management authority for the Trading System resource envelope, subject to Foundation reconciliation under FCR-0031.

### Prime rules

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

### Required FSARM responsibilities
- maintain attributable current resource picture across Trading, FSAPMA, Guardian and FSTSimA;
- maintain current effective allocation and consumption;
- receive minimum-safe / survival requirement;
- receive desired allocation and pressure/urgency evidence;
- identify reclaimable capacity and degradation/shedding eligibility;
- reserve/rebalance/reclaim/throttle/shed/suspend eligible workloads;
- preserve protected live-critical and safety obligations according to governed current evidence;
- perform controlled staged restoration;
- calculate proven remaining deficit after safe internal redistribution;
- request additional resources from Foundation Resource Governance only for the remaining evidenced deficit when the governed Foundation boundary exists and is authorized;
- consume Foundation grant/partial/cap/deny/reduce/revoke/reclaim/rebalance/restore outcomes without treating a request as a grant;
- preserve exact per-Application attribution/accountability/isolation.

### Priority principle
FSARM SHALL NOT rely on one permanent Application ranking. It SHALL evaluate active obligation, consequence of starvation, minimum-safe requirement, reclaimability, current pressure, current protection state and admitted resource policy.

Design intent preferentially protects, when applicable, capital protection, crisis handling, reconciliation, open-position safety and required operational data paths before simulation, experimentation, discovery, analytics, research and other deferrable work.

### Foundation authority separation

```text
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
```

Foundation remains sole total-resource truth and final grant/cap/deny/reduce/revoke/reclaim/rebalance/restore authority.

### Non-authorities
FSARM does not own Trading decisions, Unified Risk, Guardian commands, provider/data truth, simulation/validation truth, lifecycle authority, FSA governance, security authority or Owner authority.

### Required outputs
- FSARM structural identity and placement;
- FSARM state model;
- per-Application resource contract/profile;
- minimum-safe and protected-floor semantics;
- reclaimability/degradation classes;
- dynamic priority evidence model;
- internal redistribution decision model;
- remaining-deficit calculation model;
- Foundation request/outcome binding;
- pressure/revocation/restoration model;
- fencing/split-brain/fail-closed rules;
- complete positive/negative/adversarial verifier plan;
- explicit FCR-0031 reconciliation evidence.

### Closure criteria
FSARM can redistribute existing FSATS capacity safely and accountably before seeking additional Foundation capacity; no Application can bypass it for FSATS resource control; no FSARM action can create Foundation resources or business authority; FCR-0031 Foundation reconciliation is incorporated and Application-verified where required.

---

## P1-K — Governed Contract, FIL, Event and Route Materialization

### Objective
Materialize the governed cross-Application communication graph after FSARM impact reconciliation without activating runtime routes.

### Required outputs for every governed family
- immutable family ID;
- exact producer/consumer Applications or admitted system-level role where Foundation permits;
- direction;
- purpose/business meaning;
- authority class;
- security class;
- payload/schema identity and versioning rule;
- FIL envelope binding;
- Service Bus route/delivery binding where applicable;
- correlation/causation/idempotency rule;
- observation/effective/expiry/deadline rule as applicable;
- ordering/duplicate/correction rule;
- replay/test/operational classification;
- acceptance/rejection rules;
- Foundation route/event/delivery dependencies;
- positive and negative fixtures.

### Baseline rule
The accepted Part 0 43/43 contract baseline remains preserved. Any new or changed cross-Application family required by FSARM is an explicit prospective semantic delta and SHALL be reconciled, counted and freshly reviewed rather than silently inserted into the accepted historical 43/43 record.

### Closure criteria
The complete current contract graph can be generated/validated deterministically from one canonical declaration set while remaining declaration-only and preserving historical accepted contract evidence.

---

## P1-L — Verification, Security, Failure, Performance and Integrated Implementation-Readiness Gate

### Objective
Design the proof system and integrated build/readiness gate for the remediated Part 1.

### Required verifier layers
- canonical primitive verifier;
- project/dependency boundary verifier;
- Application topology/identity/Manifest verifier;
- per-Application architecture verifiers;
- FSARM identity/authority/resource verifier;
- internal redistribution and remaining-deficit verifier;
- crisis reallocation verifier;
- minimum-safe/reclaimability/restoration verifier;
- governed contract graph verifier;
- Foundation binding verifier;
- authority/non-authority verifier;
- security/isolation verifier;
- replay/idempotency/stale/expiry verifier;
- deterministic failure/degraded/recovery fixtures;
- performance/deadline/backpressure/tail-latency test plan;
- complete project/module dependency DAG;
- Foundation/FCR blocker overlay;
- safe parallelization lanes;
- future implementation slice catalog;
- integrated risk/unresolved registers;
- final Part 1 Owner review package.

### Mandatory rule
```text
DESIGN_READY != IMPLEMENTATION_AUTHORIZED
IMPLEMENTED != RUNTIME_AUTHORIZED
RUNTIME_AUTHORIZED != PAPER_OR_LIVE_AUTHORIZED
```

### Closure criteria
There is one unambiguous, evidence-backed route from the accepted historical Part 0 baseline plus explicit later Owner corrections to separately authorizable implementation slices, with no hidden dependency, authority shortcut, stale TARC-only assumption or big-bang implementation requirement.
