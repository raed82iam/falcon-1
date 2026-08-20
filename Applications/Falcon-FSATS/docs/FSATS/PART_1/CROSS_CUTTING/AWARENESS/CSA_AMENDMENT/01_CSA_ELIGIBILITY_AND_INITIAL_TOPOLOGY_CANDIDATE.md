# FSATS Part 1 — CSA Eligibility and Initial Topology Candidate

**Status:** `OWNER-DIRECTED SEMANTIC CHANGE / CANDIDATE V2 / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Scope:** `LIMITED PART 1 AWARENESS / MANIFEST AMENDMENT ONLY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

This candidate records the Project Owner direction to select the CSA topology that is best for the current FSATS design, using the smallest set of Component Self-Awareness instances that provides material specialized self-evaluation and governed improvement value without duplicating LSA responsibility or creating authority leakage.

Historical Part 0 and Part 1 accepted records remain preserved. This candidate does not rewrite them. If later Owner-accepted, it prospectively amends only the current CSA eligibility/topology interpretation and corresponding future Manifest declarations.

## 2. Governing Rules

The controlling hierarchy remains:

```text
FSA = Foundation / OS awareness
MSA = exactly one complete Application
LSA = exactly one major Application branch
CSA = optional awareness of exactly one eligible intelligent component
```

A CSA:

- SHALL belong to exactly one intelligent component;
- SHALL belong to exactly one parent LSA;
- SHALL NOT duplicate the whole responsibility of its parent LSA;
- SHALL NOT create authority, ownership, jurisdiction, deployment, promotion or production-adoption rights;
- MAY evaluate its own specialized performance, limits, confidence, weaknesses and improvement opportunities inside the same already-authorized component responsibility;
- SHALL use `CSA -> Parent LSA -> Application MSA -> FSA -> separate Owner/governance adoption decision` for production-bound self-development;
- SHALL follow accepted Safety Continuity and AI Repair / Controlled Recovery rules if killed, untrusted or under integrity investigation.

## 3. Eligibility Test

A component is eligible for an initial CSA only when all are true:

1. `SINGLE_COMPONENT_IDENTITY` — one bounded component can be named without spanning multiple LSAs.
2. `SPECIALIZED_INTELLIGENCE` — the component performs non-trivial adaptive/inferential/intelligent work rather than deterministic plumbing.
3. `SELF_EVALUATION_VALUE` — it has meaningful component-local quality, confidence, drift, weakness or failure metrics.
4. `LOCAL_IMPROVEMENT_SPACE` — it can propose improvements to the same component responsibility without redefining the LSA/Application.
5. `INDEPENDENT_REVIEW_REMAINS` — the CSA cannot approve, validate or deploy its own material change.
6. `FAILURE_CAN_BE_SCOPED` — CSA/target failure can be contained without pretending the whole Application is automatically untrusted.
7. `NO_SAFETY_INVERSION` — adding CSA does not make a deterministic safety/control path depend on the intelligence it must survive.
8. `INDEPENDENT_EVIDENCE_EXISTS` — the component's self-reported success, confidence or improvement evidence can be challenged by evidence not solely produced or controlled by the CSA/target component.
9. `RESOURCE_COST_IS_ACCOUNTED` — compute/memory/storage/model/accelerator/network and evidence overhead is visible inside the owning Application's resource accounting and cannot become hidden free capacity.

If any criterion is not currently proven, LSA awareness remains sufficient and eligibility may be reconsidered later.

## 4. Recommended Initial CSA Set

### CSA-T06-01 — StrategyController CSA

```text
Application: Falcon Self-Aware Trading Application
Parent LSA: T-LSA-06 Strategy Orchestration & Decision
Target Component: StrategyController
Initial State: CSA_ELIGIBLE_CANDIDATE
```

Purpose: assess strategy-selection/synthesis quality, confidence calibration, conflict-resolution quality, recurring orchestration weakness, strategy/school weighting performance and component-local improvement opportunities.

Forbidden: broker action, Risk override, capital authority, self-promotion, direct deployment, changing Trading goals or market ownership.

Required independent evidence: Trading outcome/attribution evidence, parent-LSA review, Risk/execution outcome evidence where relevant, and FSTSimA validation for material strategy-orchestration changes. CSA self-scores are never sufficient promotion evidence.

### CSA-T05-01 — OpportunityDiscoveryEngine CSA

```text
Application: Falcon Self-Aware Trading Application
Parent LSA: T-LSA-05 Opportunity Hunting School
Target Component: OpportunityDiscoveryEngine
Initial State: CSA_ELIGIBLE_CANDIDATE
```

`OpportunityDiscoveryEngine` is the canonical bounded intelligent component identity for the opportunity-hunting/whale/liquidity discovery engine inside T-LSA-05. It does not replace the LSA or own global strategy selection.

Purpose: evaluate discovery precision/recall, false-positive rate, regime sensitivity, confidence calibration, missed-opportunity patterns and component-local method improvement.

Forbidden: direct execution, portfolio authority, Risk override, market-wide strategy control or autonomous production adoption.

Required independent evidence: downstream outcome/attribution evidence, market-regime holdout evidence and parent-LSA/MSA review. Discovery confidence is not proof of discovery quality.

### CSA-T12-01 — StrategyEvolutionEngine CSA

```text
Application: Falcon Self-Aware Trading Application
Parent LSA: T-LSA-12 Strategy Evolution & Experimentation
Target Component: StrategyEvolutionEngine
Initial State: CSA_ELIGIBLE_CANDIDATE
```

`StrategyEvolutionEngine` is the canonical bounded intelligent component identity for candidate generation/modification/reweighting experimentation inside T-LSA-12.

Purpose: assess candidate-generation quality, novelty versus redundancy, experiment yield, failure patterns, search efficiency and component-local improvement opportunities.

Forbidden: production adoption, deployment, live mutation, authority expansion, bypassing FSTSimA/LSA/MSA/FSA/Owner review.

Required independent evidence: version-pinned FSTSimA experiments, out-of-sample/holdout challenge where applicable, parent-LSA/MSA evaluation and separately governed validation. The engine may generate candidates and self-diagnostics but may not define the only metric by which its candidates are accepted.

### CSA-P05-01 — AnomalyDetector CSA

```text
Application: FSAPMA
Parent LSA: P-LSA-05 Data Quality, Verification & Reconciliation
Target Component: AnomalyDetector
Initial State: CSA_ELIGIBLE_CANDIDATE
```

Purpose: evaluate anomaly-detection quality, false positives/negatives, drift, source/regime sensitivity, confidence calibration and specialized detector improvement.

Forbidden: provider entitlement truth, provider routing authority, silent correction of source truth, Trading interpretation or Foundation authority.

Required independent evidence: cross-provider reconciliation, known-correction outcomes, lineage-preserved source evidence and parent-LSA review. Detector output cannot erase contradictory source evidence.

### CSA-G01-01 — IncidentClassifier CSA

```text
Application: Falcon Trading Guardian Application
Parent LSA: G-LSA-01 Protection Observation & Incident Qualification
Target Component: IncidentClassifier
Initial State: CSA_ELIGIBLE_CANDIDATE
```

Purpose: evaluate incident-classification accuracy, severity calibration, false-positive/false-negative patterns, unknown-state handling and classifier-local improvement.

Forbidden: self-authorizing Kill, command authority, protection-policy rewrite, deterministic Safety Kernel control, release/trust-restoration authority or ownership of another Application's truth.

Required independent evidence: incident outcomes, protection evidence, parent-LSA/MSA review and independently governed Guardian/Owner policy. The classifier may recommend classification; it may not redefine the deterministic command/containment policy that consumes classification.

### CSA-S02-01 — SyntheticMarketGenerator CSA

```text
Application: FSTSimA
Parent LSA: S-LSA-02 Market Environment Simulation
Target Component: SyntheticMarketGenerator
Initial State: CSA_ELIGIBLE_CANDIDATE
```

Purpose: evaluate synthetic-market realism, regime coverage, adversarial/rare-event coverage, diversity, mode collapse/redundancy, microstructure fidelity and generator-local improvement.

Forbidden: operational market truth, Paper/Live authority, self-validating its generated scenarios or contaminating historical evidence classification.

Required independent evidence: version-pinned historical/observed reference sets, holdout regime tests, S-LSA-07 fidelity/calibration evidence and S-LSA-08 independent validation assessment. Generator self-evaluation may propose weakness but cannot establish validation readiness.

### CSA-S07-01 — CalibrationEngine CSA

```text
Application: FSTSimA
Parent LSA: S-LSA-07 Fidelity & Calibration
Target Component: CalibrationEngine
Initial State: CSA_ELIGIBLE_CANDIDATE
```

Purpose: evaluate calibration error, convergence quality, parameter stability, divergence residuals, overfitting and component-local calibration improvement.

Forbidden: changing accepted evidence retroactively, approving its own calibration result, modifying S-LSA-08 independent assessment, or creating Paper/Live readiness authority.

Required independent evidence: frozen input/reference datasets, holdout divergence checks, parameter-version lineage and S-LSA-08 independent assessment. CalibrationEngine cannot change the validation oracle or acceptance criteria used to judge the same candidate without a separately reviewed semantic change.

## 5. Explicitly Not Initial CSA

The following are intentionally not initial CSA identities:

- `Unified Risk Management` as a whole — wrong granularity; it is T-LSA-07 responsibility. Future bounded intelligent subcomponents may be separately evaluated.
- `Market & Instrument Universe` as a whole — LSA awareness is sufficient until a single adaptive ranking component identity and measurable self-improvement need are proven.
- `Provider Reliability & Capability Intelligence` as a combined concept — rejected because it crosses P-LSA-03 and P-LSA-06. `ReliabilityModel` may be reconsidered later as a single P-LSA-06 component if adaptive self-awareness value is proven.
- `ProviderController` — remains operational controller, not CSA.
- `DeterministicSafetyKernel` — CSA forbidden because deterministic protection must remain independent from the intelligence it may need to survive.
- `ValidationAssessor` / `SimulationOracle` — no initial CSA because independent validation must not drift toward self-modifying/self-clearing validation authority. S-LSA-08 LSA awareness remains sufficient initially.
- `ResourceStrategyController` / APP-RSC components — APP-RSC remains `CSA=0 initially`; operational resource coordination must not gain an unnecessary new awareness layer before implementation evidence proves component-local need.

## 6. Initial Count If Accepted

```text
Trading: 3 CSA
FSAPMA: 1 CSA
Trading Guardian: 1 CSA
FSTSimA: 2 CSA
APP-RSC: 0 CSA initially

TOTAL INITIAL CSA = 7
TOTAL MSA = 5 unchanged
TOTAL LSA = 34 unchanged
```

CSA count does not alter Application count, MSA count, LSA count, Application ownership or Foundation authority.

## 7. Manifest / Lifecycle Impact

If Owner-accepted, future Application Manifest materialization shall declare each accepted CSA identity, target component, parent LSA, purpose, prohibited scope, self-development eligibility, escalation path, integrity/health reporting, trusted-baseline/recovery references where applicable and Kill/repair/revival expectations.

No secret, Foundation-internal field or undeclared authority is introduced.

## 8. Safety / Recovery Invariants

For every CSA:

```text
CSA_FAULT != AUTOMATIC_APPLICATION_KILL
CSA_KILL != AUTHORITY_TRANSFER
CSA_RESTART != TRUST_RESTORATION
CSA_REPAIR != SELF_APPROVAL
```

A killed/untrusted CSA cannot approve or release itself. Derived work from a revoked trust epoch is fenced. Existing exposure/protection obligations follow the accepted Application Safety Continuity rules. Repair occurs through the accepted isolated remediation and Controlled Revival process.

## 9. Independent Evidence / Anti-Goodhart Rule

For every CSA:

```text
SELF_REPORTED_IMPROVEMENT != INDEPENDENT_PROOF
METRIC_IMPROVEMENT != BUSINESS_OR_SAFETY_ACCEPTANCE
LOCAL_OPTIMUM != APPLICATION_OPTIMUM
```

A CSA may recommend new metrics or evaluation methods only as a governed candidate. It cannot silently replace the external/parent/independent evidence used to judge its own production-bound change. Evaluation datasets, holdouts, oracle identities, policy versions and relevant evidence must be versioned/provenance-bound so the CSA cannot improve its score by silently moving the goalposts.

## 10. Research / External Access Boundary

CSA status creates no direct Internet or provider/broker egress.

```text
TRADING CSA DIRECT INTERNET = FORBIDDEN
GUARDIAN CSA DIRECT INTERNET = FORBIDDEN
CSA STATUS != EGRESS AUTHORITY
```

- Trading CSA external research needs use the same governed FSTSimA research/sandbox path applicable to Trading Awareness when separately authorized and when required Foundation capabilities exist.
- FSAPMA `AnomalyDetector` CSA consumes only FSAPMA-owned/provider-delivered operational evidence available through its parent Application boundaries; CSA does not gain an independent provider session, credential or Internet path.
- Guardian `IncidentClassifier` CSA consumes governed Guardian-visible evidence only and receives no independent external Internet/provider/broker route.
- FSTSimA CSA external research, if ever required, may use only FSTSimA-governed non-Live research/sandbox egress when separately authorized and available. Research input remains provenance-bound, quarantined and non-authoritative.
- No CSA may use Web/browser paths as hidden research or egress channels.

## 11. Diagnosis, Learning and Runtime-Mutation Boundary

CSA observation/self-evaluation is separated from production behavior mutation:

```text
CSA_DIAGNOSIS != TARGET_RUNTIME_MUTATION
CSA_LEARNING != PRODUCTION_MODEL_REPLACEMENT
CSA_PROPOSAL != LIVE_PARAMETER_AUTHORITY
```

A CSA may observe, learn, diagnose, research through authorized paths and produce a versioned candidate/evidence package. It SHALL NOT silently change production code, model weights, thresholds, strategy weights, classifier policy, calibration acceptance criteria, runtime permissions or other material behavior merely because it detected weakness.

Existing already-authorized business adaptation performed by the target component remains target/LSA business behavior and does not become CSA self-development by relabeling. Conversely, a CSA-originated change to the method/model/code/configuration used by the target must use the origin-correct governed self-development path and separate adoption authority.

A CSA may recommend fail-closed restriction/degradation through the parent Awareness/governed protection path when confidence or integrity becomes insufficient, but the recommendation does not itself create enforcement authority.

## 12. Resource Accounting and Degradation

CSA capability is not free and does not sit outside Falcon resource governance.

```text
CSA_RESOURCE_COST != ZERO
CSA_SELF_REPORTED_NEED != RESOURCE_GRANT
CSA_IMPORTANCE != AUTOMATIC_PROTECTED_FLOOR
```

Each owning Application shall account for CSA compute, memory, storage, accelerator/model, evidence, checkpoint/recovery and permitted network costs inside its admitted Application allocation. Branch/component distribution remains Application-owned; total Application grants/ceilings/floors remain Foundation-owned; FSATS cross-Application coordination remains APP-RSC-owned within its accepted scope.

CSA demand/pressure/reclaimability/degradation consequences shall contribute to the owning Application's resource evidence. A CSA cannot request or seize sibling Application resources directly and cannot bypass APP-RSC/Foundation resource boundaries.

Under resource pressure, CSA monitoring/research/experimentation may be reduced, sampled, paused or checkpointed according to the owning Application's safe shedding policy. No CSA receives an automatic survival floor merely because it is Awareness. If a particular CSA function is proven necessary for a current safety obligation, that consequence may be reported as attributable evidence through the normal Application -> APP-RSC/Foundation resource path; the CSA still cannot mint its own priority or floor.

Degrading or pausing a CSA shall not erase already committed evidence, silently clear an integrity hold, restore trust, or orphan target-component safety obligations.

## 13. Review Requirement

This is a semantic change to the accepted Part 1 Awareness/Manifest topology. It requires fresh Architecture/Consistency review, fresh Red-Team review and explicit Project Owner final acceptance before becoming controlling accepted design.
