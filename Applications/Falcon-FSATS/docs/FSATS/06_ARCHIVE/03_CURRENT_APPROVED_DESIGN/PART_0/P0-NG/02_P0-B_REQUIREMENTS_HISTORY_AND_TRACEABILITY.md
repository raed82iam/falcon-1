# P0-B — Requirements, Historical Knowledge and Traceability

**Status:** `PROPOSED / OWNER_REVIEW_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `P0-B only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

P0-B ensures that useful accepted and historical FSATS knowledge is not silently lost while preventing historical design from becoming governing authority merely because it existed first.

It converts design history into an explicit trace model that explains what problem each current rule solves, what was retained, what was strengthened, what was replaced, and why.

---

## 2. Responsibility

P0-B owns:

- material current-P0 concept inventory;
- material historical/reference concept inventory;
- Owner-decision deltas;
- current Foundation constraints relevant to each concept;
- P0-NG dispositions;
- requirement-to-design-to-test traceability;
- silent-removal and unexplained-weakening detection.

P0-B does not authorize historical behavior or revive superseded semantics.

---

## 3. Concept Disposition Classes

Every material concept SHALL receive one explicit disposition:

- `RETAIN`;
- `RETAIN_AND_CONSOLIDATE`;
- `RETAIN_AND_HARDEN`;
- `SYNTHESIZE`;
- `REPLACE_MECHANISM_PRESERVE_INTENT`;
- `HISTORICAL_ONLY`;
- `DEFER_EXPLICITLY`;
- `REJECT_WITH_REASON`;
- `FOUNDATION_DEPENDENT_FAIL_CLOSED`.

No concept may disappear merely because its old document is no longer convenient.

---

## 4. Material Concept Record

For every material concept, the trace record SHALL answer:

1. What Falcon/FSATS problem does this solve?
2. What Vision/Constitution constraint applies?
3. Which current approved P0 rule(s) currently address it?
4. Which historical/V1.3 concept is relevant, if any?
5. Which Owner decision controls it, if any?
6. Which Foundation authority/dependency applies?
7. Which FCR applies, if any?
8. What does P0-NG retain/change?
9. Why is the P0-NG formulation stronger or clearer?
10. What trade-off or new risk does it create?
11. What test/evidence obligation proves the rule?
12. What future implementation owner/artifact is expected to realize it?

If a material change cannot answer these questions, it is not ready for final architecture review.

---

## 5. Trace Graph

P0-B SHALL maintain a directed trace model with node classes including:

- `VISION_PRINCIPLE`;
- `CONSTITUTION_RULE`;
- `OWNER_DECISION`;
- `FOUNDATION_AUTHORITY`;
- `FCR`;
- `HISTORICAL_CONCEPT`;
- `CURRENT_P0_CONCEPT`;
- `P0_NG_RULE`;
- `APPLICATION`;
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
- `FUTURE_IMPLEMENTATION_ARTIFACT`.

Permitted relationships include:

- `GOVERNS`;
- `DERIVED_FROM`;
- `RETAINS`;
- `HARDENS`;
- `SYNTHESIZES`;
- `REPLACES_MECHANISM`;
- `SUPERSEDES_IF_ACCEPTED`;
- `DEPENDS_ON`;
- `BLOCKED_BY`;
- `OWNED_BY`;
- `CONSUMES`;
- `PRODUCES`;
- `VERIFIED_BY`;
- `RED_TEAMD_BY`;
- `IMPLEMENTS_LATER`.

A trace relation never creates authority.

---

## 6. Historical Knowledge Rule

Historical design is preserved for:

- the original problem definition;
- previous design strengths;
- previous failure modes;
- discarded alternatives;
- prior Owner intent;
- lessons learned;
- provenance of current semantics.

Historical design SHALL NOT:

- override current Vision/Constitution;
- override a later valid Owner decision;
- override current Foundation boundaries;
- reintroduce fixed numeric gates that current P0-K intentionally replaced;
- restore the old 12-LSA Trading topology after the accepted 13-LSA/TARC amendment;
- revive old Guardian direct resource-request semantics superseded by TARC.

---

## 7. Current-to-P0-NG Consolidation Map

### P0-A
Retain authority hierarchy, source classification, Owner precedence, technical-PASS separation, evidence provenance, and current-state verification. Consolidate them into a reusable governance/evidence kernel.

### P0-B
Retain full material concept accounting and no-silent-removal discipline. Harden it into a living trace graph.

### P0-C
Retain independent Applications, one MSA per Application, qualified LSA topology, optional eligible CSA, FSA Foundation-only jurisdiction, branch qualification, and self-development governance. Consolidate the final semantics directly, including learning/research/evolution and Owner no-response behavior.

### P0-D
Retain Foundation/Application separation, anti-reimplementation, FCR discipline, and explicit runtime readiness. Harden state separation across semantic, implementation, Application verification, and runtime authorization axes.

### P0-E
Retain independent identity, CON-023 declarations, APP-001 lifecycle, rollback/removal/state-migration obligations, and fail-closed unresolved authority-bearing fields.

### P0-F
Retain the accepted 43-family minimum contract baseline and all later hardenings. Consolidate them into an exact contract graph plus state machines and negative fixtures.

### P0-G
Retain FSAPMA as sole operational external-data gateway, Provider/ServiceRole/APIInstance separation, Data Products, entitlements, provider pools, continuity, precision/unit/adjustment semantics, Route Leases, circuit/retry/hedging safeguards, and quota/capacity protections.

### P0-H
Retain the current 13-LSA Trading topology, central Strategy Catalog/Controller, Market Profiles, Unified Risk, portfolio/capital, execution/reconciliation, learning/analytics/evolution functions, and T-LSA-13/TARC amendment. Normalize all current semantics into one direct model.

### P0-I
Retain Guardian independence, state model, crisis/protection semantics, playbooks, MVPS, EPCP, scoped restrictions, no blind liquidation, recovery proof, and TARC resource-request separation.

### P0-J
Retain end-to-end deadlines, bounded queues, backpressure, business lanes, priority-inversion controls, load shedding, TARC tiers, Foundation technical-priority separation, coherency and staged recovery.

### P0-K
Retain independent FSTSimA, multidimensional validation/authority state, Intended Use, credibility case, V&V/UQ, preregistered experiments, independent validation, evidence freshness, TinyLive separation, and reversible promotion.

No P0-L is created by this candidate.

---

## 8. 13-LSA Migration Rule

Historical Trading responsibility SHALL map into the current 13 rooms without loss:

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
13. Trading Resource Management.

A historical responsibility mismatch is a migration problem, not permission to delete the responsibility.

---

## 9. Contract Migration Rule

The accepted 43-family P0-F inventory is the minimum migration baseline.

For each family, the consolidated model SHALL preserve:

- exact producer;
- exact consumer;
- business family identity;
- purpose;
- authority class;
- security class;
- schema/version policy;
- truth/environment classification;
- deadline/freshness semantics where material;
- correlation/causation;
- idempotency/replay semantics;
- failure/degraded behavior;
- current Foundation/FCR dependency;
- applicable user/Owner/Guardian/Risk semantics;
- later accepted hardening constraints.

Metadata profiles may be reused. Distinct business contract identities SHALL NOT be merged if accountability or authority meaning would be lost.

---

## 10. State-Machine Preservation

At minimum the following state machines SHALL remain explicitly traceable:

- Owner/user stop/resume/close;
- stop-order race and control epochs;
- subscription progressive restriction and `POST_EXPIRY_MANAGED_EXIT`;
- capital reservation;
- broker order/execution ambiguity and reconciliation;
- Guardian state/directive/recovery;
- FSAPMA stream continuity and circuit state;
- TARC resource request/control/shedding/restoration where authorized;
- validation/promotion/restriction/demotion/revocation.

Naming may improve. Accepted guards and fail-safe behavior may not disappear.

---

## 11. Anti-Drift Checks

The current design representation and future Architecture Registry SHOULD be checked for drift in:

- Application identities;
- MSA/LSA counts and ownership;
- TARC role;
- operational-controller ownership;
- contract edge identities;
- producer/consumer pairs;
- FCR references;
- Data Product identities;
- Guardian playbook identities;
- lifecycle/status claims;
- Owner decision references;
- runtime-authority claims.

A semantic mismatch is a review finding, not harmless documentation drift.

---

## 12. Explicit Non-Authority

Traceability does not authorize:

- old behavior;
- new behavior;
- Foundation capability;
- implementation;
- runtime operation;
- FCR closure;
- promotion.

---

## 13. Invariants

```text
SILENT_MATERIAL_REMOVAL = PROHIBITED
UNEXPLAINED_SEMANTIC_WEAKENING = PROHIBITED
HISTORICAL_EXISTENCE != CURRENT_AUTHORITY
TRACE_RELATION != AUTHORITY
OLD_12_LSA_TOPOLOGY != CURRENT_TRADING_TOPOLOGY
OLD_GUARDIAN_RESOURCE_ESCALATION != CURRENT_TARC_RESOURCE_ROUTE
P0F_MINIMUM_MIGRATION_BASELINE = ACCEPTED_43_FAMILIES
P0L_CREATED_BY_THIS_CANDIDATE = NO
```

---

## 14. Forbidden Interpretations

Invalid interpretations include:

- “if a historical concept is not copied verbatim, its problem can be ignored”;
- “if V1.3 had a numeric threshold, it remains a current default”;
- “older 12-LSA diagrams control because they are older”;
- “a trace graph edge means the dependency is implemented”;
- “consolidating documents permits deleting historical decision evidence”;
- “P0-NG elegance is sufficient proof of semantic completeness”.

---

## 15. Exit Gates

```text
CURRENT_ACCEPTED_MATERIAL_CONCEPTS_MAPPED = 100%
CURRENT_OWNER_DECISIONS_APPLIED = 100%
CURRENT_13_LSA_TARC_MODEL_PRESERVED = PASS
CURRENT_P0F_CONTRACT_BASELINE_MAPPED = 100%
CURRENT_P0K_ACCEPTED_STRENGTH_PRESERVED = PASS
SILENT_REMOVALS = 0
UNEXPLAINED_WEAKENINGS = 0
HISTORICAL_VETOES = 0
TRACE_AUTHORITY_CONFLATION = 0
```

---

## 16. Next Authorized Gate

P0-B completion does not authorize implementation. It makes later design auditable and prevents silent semantic loss.
