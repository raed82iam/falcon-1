# P0-B - Requirements, Historical Knowledge and Traceability

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-B ensures that useful accepted and historical FSATS knowledge is not silently lost while preventing historical design from becoming current authority merely because it existed first. It converts design history into an explicit trace model showing what problem each current rule solves, what was retained, hardened, replaced, superseded or deferred, and what evidence must prove it.

## 2. Responsibility

P0-B owns:

- material current-P0 concept inventory;
- material historical/reference concept inventory;
- later Owner-decision deltas;
- Foundation/FCR constraints relevant to each concept;
- current integrated disposition;
- requirement-to-design-to-test-to-implementation traceability;
- silent-removal and unexplained-weakening detection;
- exact supersession accounting for the former four-Application/TARC/user-centric models.

It does not authorize historical behavior or revive superseded semantics.

## 3. Concept disposition classes

Every material inherited concept receives exactly one explicit disposition:

- `RETAIN`;
- `RETAIN_AND_CONSOLIDATE`;
- `RETAIN_AND_HARDEN`;
- `SYNTHESIZE`;
- `REPLACE_MECHANISM_PRESERVE_INTENT`;
- `HISTORICAL_ONLY`;
- `DEFER_EXPLICITLY`;
- `REJECT_WITH_REASON`;
- `FOUNDATION_DEPENDENT_FAIL_CLOSED`.

No material concept may disappear because its old document is inconvenient.

## 4. Material concept record

For every material concept the trace record must answer:

1. What Falcon/FSATS problem does this solve?
2. What Vision/Constitution constraint applies?
3. Which current integrated P0 rule addresses it?
4. Which historical/V1.3/predecessor concept is relevant?
5. Which Owner decision controls or changed it?
6. Which Foundation authority/dependency applies?
7. Which FCR applies?
8. What was retained, hardened, replaced or superseded?
9. Why is the current formulation stronger/clearer?
10. What trade-off or new risk exists?
11. What review/test/evidence obligation proves it?
12. Which current/future implementation artifact owns realization?

If a material change cannot answer these questions, it is not ready for final architecture review.

## 5. Trace graph

Node classes include at least:

- `VISION_PRINCIPLE`;
- `CONSTITUTION_RULE`;
- `OWNER_DECISION`;
- `FOUNDATION_AUTHORITY`;
- `FCR`;
- `HISTORICAL_CONCEPT`;
- `CURRENT_P0_CONCEPT`;
- `APPLICATION`;
- `MSA`;
- `LSA`;
- `CSA`;
- `OPERATIONAL_CONTROLLER`;
- `CONTRACT_EDGE`;
- `DATA_PRODUCT`;
- `GUARDIAN_PLAYBOOK`;
- `RISK_CONTROL`;
- `RESOURCE_POLICY`;
- `VALIDATION_OBLIGATION`;
- `TEST_FIXTURE`;
- `IMPLEMENTATION_ARTIFACT`.

Relationships include:

- `GOVERNS`;
- `DERIVED_FROM`;
- `RETAINS`;
- `HARDENS`;
- `SYNTHESIZES`;
- `REPLACES_MECHANISM`;
- `SUPERSEDES`;
- `DEPENDS_ON`;
- `BLOCKED_BY`;
- `OWNED_BY`;
- `CONSUMES`;
- `PRODUCES`;
- `VERIFIED_BY`;
- `RED_TEAMD_BY`;
- `IMPLEMENTED_BY`.

A trace edge never creates authority.

## 6. Historical knowledge rule

Historical design is preserved for original problem definitions, prior strengths, failure modes, alternatives, Owner intent, lessons and provenance. It cannot override Vision/Constitution, later valid Owner decisions, current Foundation boundaries or current accepted architecture.

The complete pre-rewrite Part 0 tree is preserved at:

`applications/docs/FSATS/06_ARCHIVE/PART_0_PRE_INTEGRATED_REWRITE_2026-08-15/`

The archive is not required implementation reading after this rewrite is Owner-accepted.

## 7. Major supersessions integrated by this rewrite

### 7.1 Four Applications -> five Applications

Current topology is:

```text
Trading       1 MSA / 13 LSA / 3 CSA
FSAPMA        1 MSA /  6 LSA / 1 CSA
Guardian      1 MSA /  4 LSA / 1 CSA
FSTSimA       1 MSA /  8 LSA / 2 CSA
APP-RSC       1 MSA /  3 LSA / 0 CSA initially
------------------------------------------
TOTAL         5 MSA / 34 LSA / 7 CSA
```

Any predecessor four-Application representation is historical only where it conflicts with this topology.

### 7.2 Historical 12-LSA Trading -> 13-LSA Trading

The current Trading branches are:

1. Operations, Account & Environment;
2. Market & Instrument Universe;
3. Analysis Frameworks;
4. Classical Trading School;
5. Opportunity Hunting School;
6. Strategy Orchestration & Decision;
7. Unified Risk Management;
8. Portfolio & Capital Management;
9. Execution & Position Lifecycle;
10. Trading Learning & Knowledge;
11. Trading Analytics & Attribution;
12. Strategy Evolution & Experimentation;
13. Trading Resource Awareness & Evaluation.

Historical responsibilities must map into these rooms without silent loss.

### 7.3 TARC -> APP-RSC resource architecture

The predecessor TARC model is not current FSATS-wide resource ownership.

```text
T-LSA-13 = Trading-local resource awareness/evaluation
APP-RSC = independent FSATS-only resource coordination Application
FOUNDATION = Falcon-wide authoritative resource truth/grants/floors/ceilings
```

Historical resource intent is preserved, but the mechanism/ownership is replaced.

### 7.4 User/customer-centric FSATS identity -> broker-account identity

Later Owner-accepted identity correction controls:

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
TRADING_OPERATING_SUBJECT = BROKER_ACCOUNT
BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = additional dimension where material
```

Shared Web owns customer/user/contact mapping to broker-account scope. Older P0 wording using `user/account` is interpreted and rewritten accordingly when it represented FSATS business identity.

## 8. Current P0 consolidation map

### P0-A
Authority hierarchy, source/evidence classes, Owner precedence, current-state freshness, FCR protocol, DPE and fail-closed ambiguity.

### P0-B
Material concept accounting, historical preservation, supersession and no-silent-removal discipline.

### P0-C
Five Applications, one MSA each, qualified LSAs, optional eligible CSAs, FSA Foundation jurisdiction, monitors/integrity, learning/research/evolution and Owner/FSA separation.

### P0-D
Foundation/Application ownership, anti-reimplementation, four readiness axes, FCR discipline and explicit runtime blocks.

### P0-E
Five independent Application identities, APP-001/CON-023 declarations, lifecycle, deployment-eligibility explanation, update/migration/rollback/removal and fail-closed unresolved fields.

### P0-F
Predecessor exact 43-family cross-Application baseline plus the later P1K APP-RSC/Foundation/recovery/current contract extensions and current identity hardenings.

### P0-G
FSAPMA provider/account/service-role/API-instance separation, Data Products, entitlements, pools, continuity, Route Leases, circuits/retries/hedging and provider egress boundary.

### P0-H
13-LSA Trading, US Equities + Crypto Spot initial scope, 1:1 funded ceiling, strategies, Unified Risk, capital reservation, execution/reconciliation, learning/evolution and T-LSA-13 resource-awareness correction.

### P0-I
Guardian independence, protection state, playbooks, MVPS, EPCP, no blind liquidation, APP-RSC resource evidence path and proof-based recovery.

### P0-J
Work Identity, lanes/deadlines/queues/QoS/load shedding and exact APP-RSC 3-LSA resource coordination with Foundation authority separation.

### P0-K
FSTSimA, V&V/UQ, Intended Use, credibility, preregistration, Shadow/Paper/Tiny-Live separation, freshness and reversible promotion.

### P0-L
Integrated end-to-end proof, current topology/contract/resource/identity consistency, review gates and non-authority.

## 9. Contract migration rule

The predecessor exact 43-family P0-F inventory is a mandatory semantic migration baseline, not a current maximum.

For every family preserve:

- exact producer/consumer;
- business family identity;
- purpose;
- authority/security class;
- schema/version policy;
- truth/environment classification;
- freshness/deadline semantics where material;
- correlation/causation;
- idempotency/replay/correction behavior;
- failure/degraded behavior;
- current Foundation/FCR dependency;
- current Owner/Guardian/Risk/broker-account identity semantics.

Later accepted APP-RSC and Foundation-facing contract families are additional current contract semantics, not permission to erase the 43 predecessor obligations.

## 10. State-machine preservation

At minimum preserve traceability for:

- Owner/user-facing stop/resume/close intent and exact control epochs, translated through current Web/broker-account boundaries;
- stop-order race/in-flight exposure exception handling;
- any accepted entitlement/subscription progressive restriction semantics where still current;
- capital reservation;
- broker order/execution ambiguity and reconciliation;
- Guardian state/directive/recovery;
- FSAPMA stream continuity and circuit state;
- APP-RSC resource evidence/coordination/fencing/restoration;
- validation/promotion/restriction/demotion/revocation;
- Application update/migration/rollback/removal.

Naming or ownership may improve after later valid decisions. Fail-safe guards may not disappear.

## 11. Implementation/test traceability

Every current material rule should trace forward to at least one of:

- architecture invariant;
- contract schema/catalog entry;
- state machine;
- source component/controller;
- negative test;
- integration scenario;
- security test;
- failure/recovery test;
- FCR dependency;
- final acceptance/closure evidence.

A documentation-only statement that cannot ever be verified is a review finding unless explicitly explanatory/non-testable by nature.

## 12. Anti-drift checks

Check current docs/source/tests for drift in:

- five Application identities;
- MSA/LSA/CSA counts and ownership;
- APP-RSC scope and exact R-LSA names;
- Trading 13-LSA map;
- broker-account identity;
- operational-controller ownership;
- contract identities and producer/consumer pairs;
- Data Product identities;
- Guardian playbooks;
- FCR references;
- lifecycle/status/runtime claims;
- Owner decision references;
- exact accepted executable-source identities.

A semantic mismatch is a review finding, not harmless documentation drift.

## 13. Explicit non-authority

Traceability does not authorize old behavior, new behavior, Foundation capability, implementation, runtime operation, FCR closure or promotion.

## 14. Invariants

```text
SILENT_MATERIAL_REMOVAL = PROHIBITED
UNEXPLAINED_SEMANTIC_WEAKENING = PROHIBITED
HISTORICAL_EXISTENCE != CURRENT_AUTHORITY
TRACE_RELATION != AUTHORITY
OLD_4_APPLICATION_TOPOLOGY != CURRENT_TOPOLOGY
OLD_12_LSA_TOPOLOGY != CURRENT_TRADING_TOPOLOGY
OLD_TARC_FSATS_RESOURCE_MODEL != CURRENT_APP_RSC_MODEL
FSATS_USER_CUSTOMER_IDENTITY != CURRENT_RUNTIME_MODEL
P0F_43_FAMILIES = REQUIRED_PREDECESSOR_MIGRATION_BASELINE
```

## 15. Forbidden interpretations

Invalid interpretations include:

- historical concept not copied verbatim means its problem can be ignored;
- V1.3 threshold remains default because historical;
- older topology controls because it was accepted earlier;
- trace edge proves implementation;
- consolidated current docs permit deletion of provenance;
- elegant rewrite proves semantic completeness without comparison;
- APP-RSC means old resource safeguards may be discarded;
- replacing user identity with broker-account identity permits loss of stop/close/control semantics.

## 16. Exit gates

```text
CURRENT_MATERIAL_CONCEPTS_MAPPED = 100%
CURRENT_OWNER_DECISIONS_APPLIED = 100%
FIVE_APPLICATION_MODEL_PRESERVED = PASS
TRADING_13_LSA_MODEL_PRESERVED = PASS
APP_RSC_RESOURCE_MODEL_PRESERVED = PASS
BROKER_ACCOUNT_IDENTITY_PRESERVED = PASS
P0F_43_FAMILY_BASELINE_MAPPED = 100%
CURRENT_P0K_STRENGTHS_PRESERVED = PASS
SILENT_REMOVALS = 0
UNEXPLAINED_WEAKENINGS = 0
HISTORICAL_VETOES = 0
TRACE_AUTHORITY_CONFLATION = 0
```

## 17. Non-grant

Acceptance of P0-B would make Part 0 auditable and semantically loss-resistant. It would not authorize implementation or runtime behavior.