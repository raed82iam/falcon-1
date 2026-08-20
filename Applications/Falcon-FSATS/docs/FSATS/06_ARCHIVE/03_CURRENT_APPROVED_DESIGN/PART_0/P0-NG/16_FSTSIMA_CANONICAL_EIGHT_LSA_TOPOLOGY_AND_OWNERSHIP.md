# FSTSimA — Canonical Eight-LSA Topology and Ownership

**Status:** `FINAL_CONSOLIDATION_SEMANTIC_REMEDIATION / NOT_FINAL_OWNER_CLOSED`  
**Affected Scope:** `P0-C + P0-K`  
**Application:** `Falcon Self-Aware Trading Simulation Application (FSTSimA)`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

---

## 1. Purpose

This record restores the explicit FSTSimA branch topology that was too compressed in the first P0-NG final-consolidation candidate.

It is controlling for the exact final candidate until its semantics are integrated directly into the final Current Approved P0-C and P0-K files after final Owner closure.

The objective is zero ambiguity about:

- the fact that FSTSimA is one independent Application;
- the fact that it has exactly one Simulation MSA;
- the fact that its current major-branch topology contains exactly eight LSAs;
- the responsibility boundary of each LSA;
- the distinction between simulation fidelity/calibration work and independent evidence/validation assessment;
- the fact that FSTSimA validation never creates Live authority.

---

## 2. Canonical Application Topology

```text
FSTSimA
└── Simulation MSA
    ├── S-LSA-01 — Simulation Time and Scenario
    ├── S-LSA-02 — Market Environment Simulation
    ├── S-LSA-03 — Provider and External Service Simulation
    ├── S-LSA-04 — Broker, Exchange and Execution Simulation
    ├── S-LSA-05 — Account, Capital and Settlement Simulation
    ├── S-LSA-06 — Fault, Latency and Crisis Injection
    ├── S-LSA-07 — Fidelity and Calibration
    └── S-LSA-08 — Oracle, Evidence, Reproducibility and Validation Assessment
```

Canonical count:

```text
FSTSIMA_MSA_COUNT = 1
FSTSIMA_MAJOR_BRANCH_COUNT = 8
FSTSIMA_LSA_COUNT = 8
```

No ninth FSTSimA LSA exists by implication.

No listed LSA may be silently merged with another merely because two branches consume similar evidence.

---

## 3. Simulation MSA

Simulation MSA is the single Main Self-Awareness entity for FSTSimA.

It SHALL:

- maintain the complete FSTSimA Application self-model;
- consume governed evidence from all eight LSAs;
- detect cross-branch simulation inconsistencies;
- assess complete-Application validation readiness;
- assess whether FSTSimA itself is fit for its declared validation purpose;
- coordinate FSTSimA self-development proposals at Application scope;
- preserve uncertainty, limitations, contradictory evidence and unresolved disagreement;
- produce the final FSTSimA Application-level recommendation for FSTSimA-origin self-development.

It SHALL NOT:

- become Trading MSA;
- own the business semantics of the Application being validated;
- promote another Application's candidate;
- create Live execution authority;
- bypass the owning Application's MSA;
- bypass FSA/Owner governance where required.

```text
FSTSIMA_MSA_VALIDATION_JUDGMENT != TARGET_APPLICATION_BUSINESS_AUTHORITY
```

---

# 4. S-LSA-01 — Simulation Time and Scenario

## 4.1 Owns

- simulation clock and deterministic time progression;
- scenario identity and immutable scenario configuration;
- historical/replay temporal boundaries;
- event-time scheduling for simulated external conditions;
- deterministic seed/time controls where applicable;
- scenario start/stop/reset semantics;
- time acceleration/deceleration semantics;
- scenario orchestration metadata.

## 4.2 Produces

- authoritative FSTSimA simulation-time truth;
- scenario timeline evidence;
- deterministic time/event-order evidence;
- scenario execution metadata.

## 4.3 Does Not Own

- market-price generation logic;
- provider behavior models;
- broker fill semantics;
- account/capital truth;
- fault-model semantics;
- fidelity judgment;
- final validation sufficiency judgment.

```text
SIMULATION_CLOCK_OWNER = S-LSA-01
SIMULATION_TIME != FOUNDATION_OR_LIVE_TIME_AUTHORITY
```

---

# 5. S-LSA-02 — Market Environment Simulation

## 5.1 Owns

- simulated market state;
- price/volume/order-book/volatility/liquidity market-environment models as applicable;
- market-regime and market-event scenario realization;
- synthetic/historical/counterfactual market-environment behavior;
- market microstructure assumptions used by the simulation.

## 5.2 Produces

- simulated market truth tagged explicitly non-Live;
- market-scenario provenance;
- market-state lineage;
- declared model assumptions and limitations.

## 5.3 Does Not Own

- operational provider data;
- Trading market selection authority;
- provider availability/failure semantics;
- broker execution behavior;
- final validation acceptance.

```text
SIMULATED_MARKET_TRUTH != OPERATIONAL_MARKET_TRUTH
```

---

# 6. S-LSA-03 — Provider and External Service Simulation

## 6.1 Owns

- simulation of provider/service availability;
- provider latency/rate-limit/quota behavior;
- malformed/stale/missing/duplicate provider-response scenarios;
- provider outage/recovery behavior;
- external-service degradation/fallback conditions;
- simulated provider capability profiles used by FSTSimA.

## 6.2 Produces

- attributable simulated provider/service behavior;
- provider fault/degradation evidence;
- simulated service-response lineage.

## 6.3 Does Not Own

- real provider registration;
- FSAPMA provider selection in production;
- operational provider credentials;
- real provider egress;
- Trading decisions.

```text
S-LSA-03_SIMULATED_PROVIDER != FSAPMA_OPERATIONAL_PROVIDER
```

---

# 7. S-LSA-04 — Broker, Exchange and Execution Simulation

## 7.1 Owns

- simulated broker/exchange behavior;
- order acknowledgement/rejection models;
- fill/partial-fill/cancel/amend behavior;
- execution latency/slippage/spread/queue assumptions;
- exchange/session execution constraints;
- execution ambiguity and race-condition simulation.

## 7.2 Produces

- simulated order/execution truth;
- deterministic execution lineage where the model permits;
- declared execution assumptions;
- fill/rejection/cancel evidence.

## 7.3 Does Not Own

- real broker credentials;
- Live broker route authority;
- production order submission;
- Trading Execution business authority;
- real account reconciliation.

```text
SIMULATED_FILL != LIVE_FILL
S-LSA-04 != T-LSA-09
```

---

# 8. S-LSA-05 — Account, Capital and Settlement Simulation

## 8.1 Owns

- simulated account state;
- simulated cash/capital/buying-power state;
- simulated reservations and settlement behavior;
- fee/commission/funding assumptions where modeled;
- account restriction and settlement scenario state;
- simulated portfolio/account consequences of simulated execution.

## 8.2 Produces

- simulated account/capital truth;
- capital/settlement lineage;
- declared account assumptions and limitations.

## 8.3 Does Not Own

- real user funds;
- real broker account truth;
- FSAOL production financial truth;
- Trading Portfolio business authority;
- real settlement authority.

```text
SIMULATED_CAPITAL != REAL_CAPITAL
```

---

# 9. S-LSA-06 — Fault, Latency and Crisis Injection

## 9.1 Owns

- controlled fault injection;
- latency/jitter/tail-latency injection;
- partial dependency failures;
- network/service interruption scenarios;
- stale/duplicate/reordered message scenarios;
- resource-pressure scenario injection where simulation permits;
- crisis/adversarial scenario orchestration for validation.

## 9.2 Produces

- exact injected-fault identity;
- fault timing/scope/severity evidence;
- observed recovery/protection behavior evidence;
- reproducible fault-scenario configuration.

## 9.3 Does Not Own

- real Guardian crisis authority;
- Foundation containment;
- production fault injection;
- real resource preemption;
- target-Application recovery decisions.

```text
SIMULATED_CRISIS != GUARDIAN_PRODUCTION_CRISIS_AUTHORITY
```

---

# 10. S-LSA-07 — Fidelity and Calibration

## 10.1 Owns

S-LSA-07 owns **model-to-reference fidelity measurement and calibration work**.

It SHALL:

- compare simulator behavior with trusted reference evidence where available;
- quantify simulator-versus-reference gaps;
- calibrate simulation model parameters within authorized non-Live scope;
- maintain calibration datasets/references and their provenance;
- detect simulation-model drift;
- document known fidelity limits by dimension;
- propose simulator-model improvements when calibration/fidelity is inadequate.

## 10.2 Typical Fidelity Dimensions

May include:

- market dynamics;
- provider behavior;
- broker/execution behavior;
- latency distribution;
- fill/rejection/partial-fill behavior;
- capital/account behavior;
- recovery timing;
- failure-frequency/severity behavior.

## 10.3 Produces

- calibration evidence;
- measured reality-gap/fidelity vectors;
- model-fit residuals;
- simulator-model limitation declarations;
- calibration candidate proposals.

## 10.4 Does Not Own

- final determination that validation evidence is sufficient for promotion;
- truth-oracle independence;
- target-Application business acceptance;
- Owner promotion authority.

```text
S-LSA-07 = MEASURE_AND_CALIBRATE_SIMULATOR_FIDELITY
S-LSA-07 != FINAL_VALIDATION_ACCEPTANCE
```

---

# 11. S-LSA-08 — Oracle, Evidence, Reproducibility and Validation Assessment

## 11.1 Owns

S-LSA-08 owns the **independent validation-evidence assessment layer inside FSTSimA**.

It SHALL:

- define/maintain truth-oracle evidence used to judge simulation outcomes where applicable;
- verify that evidence identities and lineage are complete;
- assess reproducibility/repeatability;
- assess whether scenario results actually support the declared Intended Use Claim;
- assess evidence completeness, contradictions, uncertainty and known limitations;
- verify that failed/unfavorable evidence is preserved;
- verify that promotion-grade experiment evidence is distinguishable from exploratory research;
- assess whether required FSTSimA validation obligations were actually satisfied;
- produce FSTSimA validation-assessment evidence to Simulation MSA.

## 11.2 Relationship to S-LSA-07

This boundary is explicit:

```text
S-LSA-07 = FIDELITY_MEASUREMENT_AND_CALIBRATION
S-LSA-08 = INDEPENDENT_ASSESSMENT_OF_FIDELITY_EVIDENCE_AND_OVERALL_VALIDATION_EVIDENCE
```

S-LSA-07 may produce a fidelity/calibration result.

S-LSA-08 evaluates whether that result is sufficiently attributable, reproducible, scoped and credible for the specific validation claim.

Neither automatically overrides the other.

Material disagreement SHALL be preserved and escalated to Simulation MSA.

## 11.3 Does Not Own

- candidate business semantics owned by another Application;
- target Application MSA judgment;
- FSA governance review;
- Owner authorization;
- promotion/deployment;
- Live authority.

```text
S-LSA-08_VALIDATION_ASSESSMENT != PROMOTION_AUTHORITY
```

---

# 12. Cross-Branch Information Flow

Canonical flow:

```text
S-LSA-01
  provides time/scenario truth
        ↓
S-LSA-02 / 03 / 04 / 05 / 06
  realize bounded simulated environment behavior
        ↓
Target logic executes under non-Live FSTSimA conditions
        ↓
S-LSA-07
  measures/calibrates simulation fidelity
        ↓
S-LSA-08
  assesses oracle/evidence/reproducibility/validation sufficiency
        ↓
Simulation MSA
  assesses complete FSTSimA result and FSTSimA readiness
        ↓
Owning target Application receives governed validation evidence
        ↓
Owning Application CSA/LSA/MSA performs its own business/domain evaluation
```

No step converts FSTSimA into owner of the target Application.

---

# 13. Target-Application Ownership Rule

When FSTSimA validates a Trading, Guardian, FSAPMA or other Application candidate:

```text
FSTSIMA_OWNS_VALIDATION_ENVIRONMENT_AND_FSTSIMA_EVIDENCE
TARGET_APPLICATION_OWNS_TARGET_BUSINESS_SEMANTICS
```

Examples:

- FSTSimA may show that a Risk candidate fails a stress scenario; Trading Risk still owns the Risk business interpretation.
- FSTSimA may show that an Execution candidate behaves unrealistically under latency; Trading Execution owns the execution business remediation proposal.
- FSTSimA may show that a provider-selection candidate fails under quota exhaustion; FSAPMA owns provider-selection business semantics.

---

# 14. Self-Awareness and Self-Development

Each of the eight LSAs is governed by the P0-C awareness model.

Each LSA may within its own branch:

- observe;
- learn from internal evidence;
- detect weaknesses/gaps;
- perform authorized research;
- form hypotheses;
- develop isolated branch-owned candidates;
- test/simulate/challenge;
- evaluate evidence;
- reject/hold/retest/recommend;
- escalate actual-origin proposals to Simulation MSA.

Eligible intelligent components inside a branch may have CSA under P0-C eligibility rules.

```text
CSA_ORIGIN -> PARENT_S-LSA -> SIMULATION_MSA -> FSA
S-LSA_ORIGIN -> SIMULATION_MSA -> FSA
SIMULATION_MSA_ORIGIN -> FSA
```

No artificial lower-tier SA is invented when the actual origin is higher.

---

# 15. Failure / Degraded Behavior

FSTSimA SHALL fail closed for a promotion-grade validation claim when materially applicable and any of the following cannot be established:

- deterministic/reconstructable scenario identity;
- simulation-time integrity;
- environment model provenance;
- execution/account model provenance;
- injected-fault identity;
- calibration/fidelity evidence;
- oracle/evidence integrity;
- reproducibility;
- Intended Use binding;
- required validation coverage;
- non-Live isolation boundary.

A degraded simulator may still support explicitly labeled exploratory research where safe and authorized, but that evidence SHALL NOT be silently upgraded to promotion-grade evidence.

---

# 16. FCR / Foundation Boundary

FCR-0011 remains open and `Waiting On: FOUNDATION` for enforceable non-Live credential/route/egress isolation.

FCR-0006 remains relevant to governed cross-Application event/evidence/replay delivery.

FCR-0008 remains relevant if FSTSimA awareness entities require research-only Internet egress.

FCR-0012 remains relevant to FSA/Owner bounded autonomous-evolution runtime governance.

```text
DESIGN_COMPLETENESS != RUNTIME_FOUNDATION_CAPABILITY_AVAILABILITY
```

No Application-local substitute is authorized for missing Foundation runtime capability.

---

# 17. Canonical Invariants

```text
FSTSIMA = ONE_INDEPENDENT_APPLICATION
FSTSIMA_MSA_COUNT = 1
FSTSIMA_LSA_COUNT = 8
ONE_MAJOR_BRANCH = ONE_LSA
S-LSA-01 = SIMULATION_TIME_AND_SCENARIO_OWNER
S-LSA-02 = MARKET_ENVIRONMENT_SIMULATION_OWNER
S-LSA-03 = PROVIDER_EXTERNAL_SERVICE_SIMULATION_OWNER
S-LSA-04 = BROKER_EXCHANGE_EXECUTION_SIMULATION_OWNER
S-LSA-05 = ACCOUNT_CAPITAL_SETTLEMENT_SIMULATION_OWNER
S-LSA-06 = FAULT_LATENCY_CRISIS_INJECTION_OWNER
S-LSA-07 = FIDELITY_MEASUREMENT_AND_CALIBRATION_OWNER
S-LSA-08 = ORACLE_EVIDENCE_REPRODUCIBILITY_VALIDATION_ASSESSMENT_OWNER
S-LSA-07 != S-LSA-08
FSTSIMA_VALIDATION != TARGET_APPLICATION_BUSINESS_AUTHORITY
FSTSIMA_VALIDATION_PASS != PROMOTION_AUTHORITY
SIMULATION_TRUTH != OPERATIONAL_TRUTH
SIMULATED_FILL != LIVE_FILL
SIMULATED_CAPITAL != REAL_CAPITAL
SIMULATED_CRISIS != PRODUCTION_CRISIS_AUTHORITY
```

---

# 18. Forbidden Interpretations

The following interpretations are explicitly invalid:

- “FSTSimA has an unspecified number of LSAs”;
- “FSTSimA has one generic Validation LSA that can absorb all eight branches”;
- “S-LSA-07 and S-LSA-08 are duplicates and may be silently merged”;
- “S-LSA-08 can approve production because it owns validation assessment”;
- “Simulation MSA can replace the target Application MSA”;
- “FSTSimA can decide Trading Risk values because it stress-tested them”;
- “FSTSimA can choose broker/provider production behavior because it simulated them”;
- “simulation data may be treated as operational truth because fidelity is high”;
- “FCR-0011 being open may be bypassed with local configuration”;
- “passing all eight branches grants Paper, Tiny Live or Live authority”.

---

# 19. Mandatory Review / Test Scenarios

Fresh reviews SHALL explicitly attack at least:

- missing one of the eight LSAs;
- duplicate LSA identity;
- branch silently merged with another;
- S-LSA-07 calibration result treated as final acceptance;
- S-LSA-08 validation result treated as promotion authority;
- Simulation MSA taking target Application business ownership;
- operational provider/broker credential injected into FSTSimA;
- simulation clock confused with operational time;
- replay result labeled Live truth;
- simulated fill labeled real fill;
- simulated capital labeled real capital;
- fault injection treated as Guardian authority;
- non-reproducible evidence presented as promotion-grade;
- contradictory evidence omitted;
- target Application accepting FSTSimA result without its own MSA/domain evaluation;
- FCR-0011 bypass attempt.

---

# 20. Exit Gates

```text
FSTSIMA_APPLICATION_IDENTITY = EXPLICIT
FSTSIMA_MSA = EXACTLY_1
FSTSIMA_MAJOR_BRANCHES = EXACTLY_8
FSTSIMA_LSAS = EXACTLY_8
ALL_8_BRANCH_OWNERSHIPS = EXPLICIT
S-LSA-07_S-LSA-08_BOUNDARY = EXPLICIT
TARGET_APPLICATION_OWNERSHIP = PRESERVED
VALIDATION_PROMOTION_CONFLATION = 0
LIVE_AUTHORITY_PATHS_FROM_FSTSIMA = 0
FCR0011_FAIL_CLOSED = EXPLICIT
FRESH_ARCHITECTURE_REVIEW = REQUIRED
FRESH_RED_TEAM_REVIEW = REQUIRED
FINAL_OWNER_CLOSURE = REQUIRED
```

---

## 21. Final Consolidation Rule

The prior semantic freeze record `15_FINAL_SEMANTIC_FREEZE_RECORD.md` is stale because this remediation changes the reviewed semantics.

A new semantic freeze SHALL be established after this remediation and all fresh review evidence SHALL bind only to that new freeze.

After final Owner closure, this topology and ownership model SHALL be integrated directly into active Current Approved P0-C/P0-K rather than forcing future readers to reconstruct the accepted design from this remediation file.
