# FSATS V1.4 PROPOSED — Application Ownership and Awareness Room Map

## Status

**Status:** `PART 0 ALIGNMENT BASELINE / OWNER REVIEW REQUIRED`  
**Authority:** design only; no implementation authority.

## Canonical migration rule for awareness topology

The final owner-approved V1.3 FSATS architecture is the migration baseline. The canonical twelve-room topology applies to the **Falcon Self-Aware Trading Application**, not to FSATS as a whole.

The earlier V1.4 proposal `2 Guardian + 3 FSAPMA + 7 Trading = 12` was an incorrect reinterpretation and is superseded.

The preserved final V1.3 topology is:

- Guardian: 4 LSA rooms;
- FSAPMA: 6 LSA rooms;
- Trading Application: 12 LSA rooms;
- total across the three Applications: 22 LSA rooms.

Part 0 does **not** reopen these rooms merely because V1.4 aligns to current Foundation. A room changes only if a concrete current-Foundation conflict, final-V1.3 supersession, explicit later Owner decision, or material Red-Team finding requires a documented delta.

# 1. Strict locality rule

For every Application:

- CSA sees and evaluates only its own eligible intelligent component;
- LSA sees and governs awareness only for its own Major Branch room and supervises only CSAs beneath it;
- LSA SHALL NOT inspect sibling LSA private state or perform sibling work;
- MSA receives reports/evidence from LSAs in its own Application and verifies that LSAs remain within declared room boundaries;
- MSA SHALL NOT enter another Application or direct another Application's MSA/LSA/CSA;
- cross-room and cross-Application effects use declared contracts/evidence rather than awareness entities as hidden integration paths;
- all MSA/LSA/CSA entities remain outside synchronous operational hot paths.

# 2. Falcon Trading Guardian Application

**MSA:** `MSA-GUARDIAN`

## G-LSA-01 — Crisis Detection and Severity
Owns trading-threat detection, incident correlation, severity classification, false-alarm calibration and local evidence.

## G-LSA-02 — Incident Command and Safe Mode
Owns playbook selection, protection scope, safe-mode/protection posture, command assurance and incident-scoped coordination.

## G-LSA-03 — Open Position Protection
Owns unprotected-position detection, emergency protective-action selection, broker-state conflict handling and residual capital-risk protection.

## G-LSA-04 — Recovery and Reconciliation
Owns recovery readiness, reconciliation integrity, progressive release recommendations and post-incident effectiveness evaluation.

### Guardian containment rule

Guardian SHALL prefer the smallest safe affected scope. A problem isolated to one user/account SHALL be contained for that user/account when technically and safely possible, without unnecessarily affecting other users. Broader restrictions require evidence of a broader trading-domain threat.

### Foundation Guardian relationship

Trading Guardian does not allocate Foundation resources. During a broad trading threat it may submit an evidenced governed request to the Foundation Guardian / Foundation-owned resource boundary for additional resources for the affected Application. Foundation retains decision and allocation authority.

Canonical confirmed Foundation gaps: FCR-0004 / Issue #4 for protection command transport, FCR-0007 / Issue #7 for resource escalation, FCR-0009 / Issue #9 for latency/QoS where required, and FCR-0010 / Issue #10 for resource-pressure signals.

# 3. Falcon Self-Aware Provider Management Application (FSAPMA)

**MSA:** `MSA-FSAPMA`

## P-LSA-01 — Provider Registry and Onboarding
Provider identity, capability profile, entitlement/license state and onboarding evidence.

## P-LSA-02 — Data Product and Semantics
Canonical trading-data products, provider semantics, source independence and trading-data lineage definitions.

## P-LSA-03 — Provider Selection and Routing
Provider-controller decisions, route suitability, fallback selection, route confidence and multi-provider conflict handling.

## P-LSA-04 — Quota, Capacity and Cost
External provider/API quota forecasting, batching/streaming optimization, provider-capacity management, protected provider reserve and free-first/cost awareness. This room does not own Foundation CPU/RAM/network allocation.

## P-LSA-05 — Data Quality and Reconciliation
Freshness, anomalies, cross-provider reconciliation, data-quality confidence, degraded-data state and evidence.

## P-LSA-06 — Broker and Account Capability
Broker/service capabilities, account restrictions, Paper/Live capability differences and execution-service capability truth consumed by authorized trading workflows.

### FSAPMA operational-data boundary

FSAPMA receives/manages operational external information required specifically for trading. It is not a general Internet gateway for Falcon.

# 4. Falcon Self-Aware Trading Application

**MSA:** `MSA-TRADING`

## T-LSA-01 — Operations, Account, and Environment
Environment Controller, Account Context, session/readiness, broker connectivity, trading mode and operating-capacity context.

## T-LSA-02 — Market and Instrument Universe
Market universe, instrument registry, market profiles, session/calendar, candidate/watch/protected sets, discovery, ranking, eligibility and compatibility.

## T-LSA-03 — Analysis Frameworks
Framework registry/engines, evidence fusion, conflict classification and eligible analytical framework intelligence.

## T-LSA-04 — Classical Trading School
Classical School engine, approved classical Strategy Models and eligible school/strategy CSAs.

## T-LSA-05 — Opportunity Hunting School
Opportunity engine, event/anomaly qualifiers, approved opportunity Strategy Models and specialized opportunity CSAs.

## T-LSA-06 — Strategy Orchestration and Decision
Central Strategy Registry, Strategy Controller, Decision Engine, trade economics, confidence calibration, strategy lifecycle/conflict/decision logic and AI-model components where eligible.

## T-LSA-07 — Unified Risk Management
Unified Risk Controller, risk budget, correlation/common-factor/concentration controls, loss controls and veto logic.

## T-LSA-08 — Portfolio and Capital Management
Portfolio management, allocation, hierarchical/global capital reservation ledger, settlement/cash availability, diversification, reservation and cross-market capital coordination. Financial capital remains categorically separate from Foundation technical resources.

## T-LSA-09 — Execution and Position Lifecycle
Intent classification, execution-adjusted edge/protective feasibility gates, execution control, broker adapters, order state machine, reconciliation, position lifecycle, protection management and Twin Paper-Live execution calibration.

## T-LSA-10 — Trading Learning and Knowledge
Learning engine, counterfactual/no-trade/rejected-opportunity evaluation, experiment registry, knowledge store, evidence promotion, drift and local learning evidence.

## T-LSA-11 — Trading Analytics and Attribution
Analytics, attribution, benchmarking, proof evaluation, contribution/cost/profitability analysis and evidence.

## T-LSA-12 — Strategy Evolution and Experimentation
Strategy decay detection, candidate generation/registry, experiment design, comparative evaluation, overfitting/leakage/cross-regime/interactions checks and promotion-evidence construction.

### Trading room non-collapse rule

Execution, Learning, Analytics and Strategy Evolution SHALL remain separate rooms unless a later explicit Owner decision changes the architecture. The previous V1.4 combined `Execution, Validation and Trading Evidence` room is superseded.

# 5. Internet research boundary for awareness entities

MSA/LSA/eligible CSA may use Internet research only for learning, research, discovery and development within their owned scope when separately permitted.

Research Internet results SHALL NOT be treated as operational market/provider truth or directly influence a trading decision. Operational external trading information must enter through FSAPMA and governed operational-data contracts. Canonical FCR-0008 / Issue #8 carries the confirmed research-egress gap.

# 6. Ownership summary

| Application | MSA | LSA count |
|---|---|---:|
| Trading Guardian | MSA-GUARDIAN | 4 |
| FSAPMA | MSA-FSAPMA | 6 |
| Trading Application | MSA-TRADING | 12 |
| **Total across three Applications** | **3 MSAs** | **22 LSAs** |

# 7. Part 0 alignment checks

Part 0 checks only whether:

1. any preserved room conflicts with current APP-001/ADR-I015 ownership semantics;
2. Foundation integration requires a route/resource/security alignment delta;
3. any later Owner correction changes a room boundary;
4. Red-Team finds a material conflict.

Current Part 0 review has found **no requirement to alter the preserved 4 + 6 + 12 topology**.
