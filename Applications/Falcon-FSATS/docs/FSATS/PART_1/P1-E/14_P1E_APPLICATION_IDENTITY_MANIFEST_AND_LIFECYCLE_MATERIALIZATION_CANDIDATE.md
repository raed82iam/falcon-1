# FSATS Part 1 — P1-E Application Identity, Manifest and Lifecycle Materialization

**Status:** `DESIGN_CANDIDATE / OWNER_REVIEWED_FOR_DIRECTION / NOT_FINAL_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Authority Type:** `PART 1 DESIGN ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`  

## 1. Purpose

P1-E converts the current FSATS topology, the governing Falcon Application requirements, and the current prospective FSARM correction into exact identity, Manifest and lifecycle materialization rules suitable for later implementation planning.

P1-E does not create implementation authority, runtime authority, deployment authority, Paper/Tiny-Live/Live authority, or a new Falcon Application by implication.

```text
P1-E
= IDENTITY
+ MANIFEST
+ OWNERSHIP
+ LIFECYCLE MATERIALIZATION
+ AUTHORITY BOUNDARIES
+ FOUNDATION BINDINGS

P1-E != IMPLEMENTATION
P1-E != RUNTIME ACTIVATION
P1-E != DEPLOYMENT
```

## 2. Governing Sources

This candidate is governed by the current Falcon Vision, Falcon Constitution, APP-001, CON-023, ADR-I012, ADR-I015, the Owner-accepted Part 0 design, the active Part 1 candidate set, current Owner-directed FSARM clarification, and current FCR state.

Where a current Foundation capability is unavailable or not yet implementation-verified, P1-E records the dependency and fails closed rather than inventing an Application-local substitute.

## 3. Current FSATS Application Topology

The current FSATS topology remains four independent Falcon Applications inside a non-owning FSATS system boundary:

```text
FSATS SYSTEM BOUNDARY
  Application = NO
  MSA = 0
  LSA = 0
  Runtime Principal = NO
  Hidden Resource Pool = NO
  Hidden State Owner = NO

FALCON SELF-AWARE TRADING APPLICATION
  MSA = 1
  LSA = 13

FALCON SELF-AWARE PROVIDER MANAGEMENT APPLICATION (FSAPMA)
  MSA = 1
  LSA = 6

FALCON TRADING GUARDIAN APPLICATION
  MSA = 1
  LSA = 4

FALCON SELF-AWARE TRADING SIMULATION APPLICATION (FSTSimA)
  MSA = 1
  LSA = 8
```

FSATS SHALL NOT become a hidden Application, hidden runtime principal, hidden lifecycle owner, hidden state owner or hidden resource owner.

## 4. Application Identity Rule

Each of the four Falcon Applications SHALL have an independently attributable immutable identity design containing at minimum:

- Application ID;
- canonical Application name;
- version;
- owner;
- purpose;
- package identity;
- provenance;
- owned business boundary;
- prohibited Foundation responsibilities.

A change of purpose or ownership is a material identity change and requires governed review.

Application identity is not merely a project, assembly, package or directory name.

## 5. Application Manifest Rule

Each Application SHALL have one complete Manifest design declaring at minimum:

- immutable identity, version, owner and purpose;
- package identity, provenance, integrity and compatibility;
- owned business boundary and prohibited Foundation responsibilities;
- dependencies and compatible versions;
- required Foundation services/contracts;
- provided capabilities and consumers;
- permissions, authority requests and security profile;
- resource requirements, minimum requirements, useful/requested bounds and degraded behavior;
- persistence, communication, configuration and evidence requirements;
- installation, validation, registration, admission, activation, update, suspension, recovery, replacement and removal behavior;
- health reporting and failure-containment interfaces;
- exactly one MSA identity;
- every major Application branch and exactly one responsible LSA for each branch;
- optional CSA identities only where eligible, plus CSA eligibility policy;
- self-development origin, ownership, evidence, escalation path and review interfaces;
- Guardian/protection interface;
- rollback or approved corrective-action plan.

The governing rule remains:

```text
UNDECLARED CAPABILITY = DENIED
UNDECLARED DEPENDENCY = DENIED
UNDECLARED ROUTE = DENIED
UNDECLARED PERMISSION = DENIED
UNDECLARED RESOURCE = DENIED
UNDECLARED AUTHORITY = DENIED
```

## 6. Manifest Does Not Create Authority

Manifest validity, technical compatibility or declaration does not imply admission, activation, business approval or production authority.

```text
VALID_MANIFEST != ADMISSION
ADMISSION != ACTIVATION
ACTIVATION != BUSINESS_APPROVAL
BUSINESS_APPROVAL != PRODUCTION_AUTHORITY
```

A requested permission is not a granted permission. A declared Live environment is not Live authority. A technically available route is not authority to use that route.

## 7. Foundation Boundary

All Foundation use SHALL occur through declared governed Foundation boundaries. Applications SHALL NOT access Foundation internals directly and SHALL NOT clone Foundation-owned semantics under Application-owned names.

Direct access from one Application to another Application's internal files, memory, databases, credentials, state or components is forbidden.

Cross-Application interaction SHALL use declared governed contracts and admitted routes.

## 8. Trading Application Materialization

The Falcon Self-Aware Trading Application SHALL declare exactly one MSA and the following thirteen major branches:

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

Trading owns Trading business semantics including Trading decisions, strategy orchestration, Trading Risk, portfolio, execution/position lifecycle, Trading learning, Trading analytics and Trading-side resource awareness/evaluation.

Trading SHALL NOT own Foundation lifecycle, Foundation resource governance, Foundation security, FSAPMA provider truth, Guardian protection authority, FSTSimA simulation truth or FSA.

## 9. T-LSA-13 Resource Boundary

T-LSA-13 remains Trading-side resource awareness/evaluation only.

```text
T_LSA13 != FSARM
```

T-LSA-13 SHALL understand and produce attributable Trading-side evidence for current resource condition, current need, minimum-safe requirement, desired capacity, pressure, reclaimability, degradation consequence, shedding consequence and restoration need.

Trading SHALL send this resource evidence/need to FSARM for FSATS-wide operational resource coordination.

Trading MSA, LSAs, CSAs and components SHALL NOT create competing direct additional-resource requesters to Foundation for FSATS resource-control purposes.

## 10. FSAPMA Materialization

FSAPMA SHALL declare exactly one MSA and six major branches:

1. P-LSA-01 Provider Registry & Onboarding
2. P-LSA-02 Data Products, Semantics & Normalization
3. P-LSA-03 Provider Capability, Account & Entitlement
4. P-LSA-04 Provider Selection, Routing & Delivery
5. P-LSA-05 Data Quality, Verification & Reconciliation
6. P-LSA-06 Quota, Capacity, Cost & Reliability

Provider Controller remains an operational controller inside P-LSA-04 and is not an independent Application or CSA.

FSAPMA owns provider and operational-data business truth within its approved boundary. It SHALL NOT own Trading decisions, Trading Risk, Trading Execution, Guardian authority or Foundation resource authority.

## 11. Guardian Materialization

Falcon Trading Guardian Application SHALL declare exactly one MSA and four major branches:

1. G-LSA-01 Protection Observation & Incident Qualification
2. G-LSA-02 Protection Scope, Restriction & Command Governance
3. G-LSA-03 Crisis State, Survival & Protection Coordination
4. G-LSA-04 Reconciliation, Recovery & Protection Evidence

Guardian owns governed protection observation, protection restrictions/commands, crisis protection coordination and protection/recovery evidence.

Guardian SHALL NOT become Trading Risk, Trading strategy, provider truth, simulation truth, FSARM resource strategy or Foundation Resource Governance.

## 12. FSTSimA Materialization

FSTSimA SHALL declare exactly one MSA and eight major branches:

1. S-LSA-01 Simulation Time & Scenario
2. S-LSA-02 Market Environment Simulation
3. S-LSA-03 Provider & External Service Simulation
4. S-LSA-04 Broker, Exchange & Execution Simulation
5. S-LSA-05 Account, Capital & Settlement Simulation
6. S-LSA-06 Fault, Latency & Crisis Injection
7. S-LSA-07 Fidelity & Calibration
8. S-LSA-08 Oracle, Evidence, Reproducibility & Validation Assessment

FSTSimA SHALL be materialized as a non-Live Application. Live broker authority, Live provider authority and Live execution authority remain denied unless a separate future governed authority explicitly grants otherwise.

## 13. Awareness Materialization

For every Falcon Application:

```text
EXACTLY_ONE_MSA = REQUIRED
EXACTLY_ONE_LSA_PER_MAJOR_BRANCH = REQUIRED
CSA = OPTIONAL_AND_ELIGIBILITY_GATED
```

The existence of a component does not require a CSA. CSA SHALL exist only for eligible intelligent components where component-level self-awareness and governed self-development have meaningful value.

The Manifest SHALL declare the CSA eligibility policy.

## 14. Origin-Aware Self-Development

Production-bound self-development SHALL enter the review path at its actual origin:

```text
CSA-originated:
CSA -> Parent LSA -> Application MSA -> FSA

LSA-originated:
LSA -> Application MSA -> FSA

MSA-originated:
Application MSA -> FSA

Foundation-originated:
FSA -> separate Foundation self-development governance and approval lifecycle
```

No artificial lower awareness tier may be inserted beneath the actual proposal origin.

FSA remains OS-governance and compatibility review only and does not grant implementation, deployment, production adoption or documentary activation.

## 15. Application Lifecycle

Each Falcon Application SHALL bind to the Foundation-governed APP-001 lifecycle:

```text
PACKAGE_RECEIVED
 -> IDENTIFIED
 -> VALIDATED
 -> REGISTERED
 -> ADMISSION_REVIEWED
 -> ACTIVATION_ELIGIBLE
 -> ACTIVE
```

Governed outcomes include rejected, quarantined, suspended, degraded, isolated, recovering, update-pending, rollback, removal-pending, removed and archived.

No lifecycle state implies the next state.

## 16. Update, Rollback and Removal

Every Application Manifest SHALL define update compatibility, migration, dependency compatibility, contract compatibility, rollback, evidence and failure behavior.

Rollback SHALL restore a valid prior trusted state while preserving identity, evidence, authority, state integrity, resource reconciliation, route reconciliation and dependency reconciliation.

Removal SHALL reconcile routes, contracts, dependencies, permissions, resources, persisted state, retained evidence and FSARM constituent resource scope where applicable.

Removal or replacement of one Application SHALL NOT require Foundation redesign or compromise another Application.

## 17. Resource Declaration Does Not Equal Direct Foundation Request

CON-023 requires every Application to declare a Resource Profile. That declaration describes the Application's resource condition and requirements but does not create an independent direct additional-resource request path to Foundation inside the FSATS resource-control domain.

An Application Resource Profile SHALL support, as applicable:

- current need;
- minimum-safe requirement;
- desired capacity;
- maximum useful/requested bound;
- current pressure;
- reclaimability;
- degradation behavior;
- shedding eligibility;
- restoration requirements;
- attributable business-consequence evidence.

```text
APPLICATION_RESOURCE_DECLARATION
!= FOUNDATION_RESOURCE_GRANT
```

## 18. Current FSARM Classification

Current FSARM remains:

```text
FSARM_ROLE = DELEGATED_AGGREGATE_RESOURCE_COORDINATOR
FSARM_SCOPE = FSATS
FSARM_IS_FALCON_APPLICATION = NO
FSARM_IS_FOUNDATION_PRINCIPAL = NO
FSARM_IS_FSATS_RUNTIME_CONTAINER = NO
FSARM_MSA = 0
FSARM_LSA = 0
FSARM_CSA = 0
```

FSARM is a first-class FSATS-wide operational resource coordinator with explicit identity, responsibility, state, interfaces, evidence, failure behavior, strategy and operational self-awareness.

It is not a fifth Falcon Application and is not an ungoverned helper.

## 19. FSARM Resource-Control Sequence

The current FSATS resource-control model is:

```text
Trading ----\
FSAPMA ------+--> FSARM --> Foundation
Guardian ----+
FSTSimA -----/
```

The governing sequence remains:

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
```

FSARM SHALL aggregate current constituent Application evidence, optimize already available resources, perform only authorized internal coordination, calculate the proven residual deficit and use the governed Foundation request boundary only for remaining need or where an authoritative Foundation grant/ceiling change is required.

Within the FSATS governed resource-request scope, FSARM is the single aggregate requester for additional Foundation resources on behalf of the constituent FSATS Applications.

This exclusivity applies to the FSATS resource-request/control domain only. It does not make FSARM a general gateway for lifecycle, admission, security, Manifest, evidence, health, communication or MSA-to-FSA governance.

## 20. Foundation Resource Truth and Authority

Foundation remains the canonical total-resource truth owner and final resource authority.

Foundation retains authoritative knowledge and control of, as applicable:

- total Falcon resource state;
- constituent Application grants;
- authoritative ceilings;
- protected floors/reserves;
- Foundation-governed priority/criticality;
- final Grant / PartialGrant / Cap / Deny decisions;
- final Reduce / Revoke / Reclaim / Restore authority where applicable.

```text
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
```

FSARM SHALL NOT self-mint Foundation grants, ceilings, floors, resources or resource authority.

## 21. Resource Attribution

FSARM aggregation SHALL preserve exact per-Application identity, attribution, accounting, isolation and reconstructability.

An aggregate FSATS resource request SHALL remain explainable as:

```text
WHO needed the resource
WHAT amount was needed
WHY it was needed
WHEN the need existed
WHAT pressure/minimum/consequence evidence supported it
WHAT internal optimization/reclaim was attempted first
WHAT residual deficit remained
WHY Foundation action was required
```

Applications SHALL NOT disappear into an anonymous opaque FSATS resource pool.

## 22. FSARM Resource Strategy Controller

FSARM SHALL own a bounded Resource Strategy Controller responsible for executing approved FSATS resource-management strategies and policies, including where authorized:

- pressure evaluation;
- minimum-safe protection;
- dynamic priority evaluation;
- reclaim thresholds;
- starvation prevention;
- oscillation prevention;
- reserve policy;
- degradation/shedding selection;
- restoration timing/staging;
- residual-deficit calculation;
- Foundation request preparation.

Executing an approved strategy does not authorize FSARM to create a new architecture or expand its authority.

## 23. FSARM Operational Self-Awareness

Current FSARM self-awareness is operational self-awareness, not an MSA/LSA/CSA tier and not autonomous self-governance.

FSARM SHALL be able to understand, as applicable:

- freshness/completeness of the resource picture;
- confidence and uncertainty in current coordination decisions;
- redistribution/reclaim/restoration effectiveness;
- starvation and near-starvation conditions;
- oscillation/thrashing patterns;
- over-allocation and under-allocation patterns;
- avoidable Foundation request frequency;
- repeated coordination failure;
- stale or missing telemetry;
- current strategy effectiveness;
- known operational limitations and capability gaps.

## 24. Bounded Adaptation

FSARM MAY learn from operational outcomes and adapt only parameters that are explicitly authorized inside pre-approved bounds.

```text
BOUNDED_ADAPTATION != SELF_DEVELOPMENT
```

Every adaptive change SHALL remain attributable, reversible, observable and auditable.

FSARM SHALL NOT autonomously rewrite its source code, create a new architecture, expand its permissions/authority, modify Foundation grants/ceilings/floors, deploy its own modifications or take business authority from another Application.

A detected material weakness may produce evidence and a recommendation for a governed change process.

## 25. FSARM Operational State and Failure Boundary

Because FSARM is not a Falcon Application, APP-001 Application lifecycle states SHALL NOT be assigned to FSARM by implication.

P1-E requires an explicit FSARM operational state model, to be finalized under the appropriate later design scope, covering at least normal operation and failure/fencing conditions such as:

- initialized/ready/active state;
- degraded state;
- fenced state;
- suspended/stopped state;
- recovering state;
- stale state;
- split-brain or conflicting coordinator state;
- Foundation-binding unavailable state.

The exact state names remain design-level until separately finalized.

```text
FSARM_OPERATIONAL_STATE
!= APPLICATION_LIFECYCLE_STATE
```

On unsafe ambiguity FSARM SHALL fail closed, preserve protected floors, not invent grants, not exceed authoritative ceilings and preserve attribution/evidence.

## 26. Permissions and Security

All permission and authority materialization SHALL be deny-by-default.

Examples of required negative boundaries include:

```text
FSTSimA -> Live Broker Authority = DENIED
FSAPMA -> Trading Order Creation = DENIED
Guardian -> Provider Ownership = DENIED
Trading -> Foundation Resource Grant Mutation = DENIED
FSARM -> Foundation Grant/Ceiling Mutation = DENIED
```

## 27. Environment Classification

Applications MAY declare environments such as Development, Test, Simulation, Paper, Shadow, Tiny Live or Live where applicable.

Environment classification SHALL NOT create operational authority.

```text
ENVIRONMENT_LIVE != LIVE_AUTHORITY
```

## 28. Communication, Persistence, Configuration and Health

Each Application Manifest SHALL declare governed communication contracts/routes, persistence/state ownership, configuration ownership/version/change authority, evidence requirements, health reporting and failure-containment behavior.

Foundation infrastructure supporting persistence, transport, health or configuration does not transfer Application business ownership to Foundation.

Application health does not imply business quality or profitability.

## 29. Guardian Protection Interface

Each Application SHALL declare its governed Guardian/protection interface. Guardian interaction SHALL occur through governed protection contracts/routes and SHALL NOT use hidden access to Application internals.

The exact command set, authority scope, expiry/idempotency, denial behavior and evidence binding remain subject to the responsible later contract/route materialization scope and current Foundation capability state.

## 30. MSA-to-FSA Binding

Each Application requires the governed MSA-to-FSA proposal/evidence path.

```text
MSA_TO_FSA_INTERFACE_REQUIRED = YES
EXACT_RUNTIME_BINDING = PENDING_CURRENT_FOUNDATION_EVIDENCE
FAIL_CLOSED = YES
```

P1-E SHALL NOT invent an Application-local substitute while FCR-0030 remains unresolved at runtime/interface level.

## 31. Foundation Dependency State Classification

For every material Foundation dependency, Part 1 SHALL distinguish:

```text
DESIGN_TIME_SPEC_AVAILABLE
BUILD_TIME_ARTIFACT_AVAILABLE
RUNTIME_CAPABILITY_AVAILABLE
RUNTIME_AUTHORITY_GRANTED
```

No state implies another.

## 32. P1-E Required Output Set

P1-E SHALL ultimately materialize four complete Application Manifest designs:

```text
TRADING_APPLICATION_MANIFEST_DESIGN
FSAPMA_APPLICATION_MANIFEST_DESIGN
GUARDIAN_APPLICATION_MANIFEST_DESIGN
FSTSIMA_APPLICATION_MANIFEST_DESIGN
```

and one separate FSARM coordinator design sheet:

```text
FSARM_COORDINATOR_IDENTITY_AND_GOVERNANCE_DESIGN
```

The FSARM design sheet SHALL NOT be mislabeled as a Falcon Application Manifest.

## 33. FSARM Coordinator Design Sheet Minimum Content

The FSARM coordinator sheet SHALL cover at minimum:

- FSARM identity;
- role;
- exact FSATS scope;
- constituent Applications;
- responsibility and non-responsibility;
- resource state picture;
- Resource Strategy Controller;
- operational self-awareness;
- approved adaptation bounds;
- Application resource interfaces;
- Foundation resource request/outcome interface;
- authority envelope;
- evidence, attribution and auditability;
- failure/fencing/split-brain behavior;
- recovery/replacement seams;
- future governed evolution seam.

## 34. Future Falcon-Wide FSARM Idea Excluded from Current Design

The separately preserved Future Backlog concept of a Falcon-wide FSARM positioned between Foundation and multiple Application domains is not part of this P1-E candidate and has no current design, planning, implementation or runtime authority.

Current design remains:

```text
FSARM_SCOPE = FSATS
```

## 35. Fail-Closed Conditions

P1-E SHALL NOT be considered implementation-ready where any of the following remains unresolved without an explicit governed gate:

- unknown Application identity;
- unknown owner/purpose/boundary;
- undeclared dependency;
- undeclared route;
- undeclared permission;
- undeclared authority;
- unknown Foundation counterpart;
- unknown failure behavior;
- unknown rollback/removal behavior;
- hidden cross-Application coupling;
- invented Foundation semantic;
- ambiguous FSARM authority;
- ambiguous per-Application resource attribution.

## 36. P1-E Candidate Closure Criteria

P1-E is ready for semantic freeze and fresh review only when all of the following are true:

1. exactly four independent Falcon Applications remain;
2. every Application has one immutable identity design;
3. every Application has one complete Manifest design;
4. every Application owns exactly one MSA;
5. every major branch has exactly one LSA;
6. CSA is optional and eligibility-gated;
7. FSATS remains non-owning and non-principal;
8. FSARM remains FSATS-scoped;
9. FSARM is not silently converted into a fifth Application;
10. FSARM is the aggregate FSATS resource coordinator/requester;
11. Foundation remains canonical total-resource truth and final resource authority;
12. Applications report resource need and evidence to FSARM;
13. FSARM performs internal optimization/redistribution first within actual authority;
14. only proven residual need uses the governed Foundation additional-resource request path;
15. per-Application identity, allocation/accounting attribution and isolation remain exact;
16. no Application receives hidden Foundation authority;
17. no hidden cross-Application coupling exists;
18. Application lifecycle remains Foundation-governed;
19. Manifest validity does not imply admission/activation/production authority;
20. unavailable Foundation dependencies fail closed;
21. update, rollback, replacement and removal are defined;
22. no implementation/runtime/deployment authority is created by P1-E.

## 37. Current Review State

This document is a Design Candidate created after Owner review of the proposed P1-E direction.

The Owner's conversational approval of the direction does not by itself establish final P1-E acceptance or closure.

Required lifecycle:

```text
P1-E DESIGN CANDIDATE
 -> EXACT SEMANTIC INSPECTION
 -> FRESH ARCHITECTURE / CONSISTENCY REVIEW
 -> FRESH RED-TEAM REVIEW
 -> REPORT TO PROJECT OWNER
 -> EXPLICIT FINAL OWNER DECISION
```

Any semantic remediation caused by review SHALL trigger a fresh review cycle for the remediated exact version before final Owner acceptance is requested.
