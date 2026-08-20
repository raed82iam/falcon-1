# P1-I — FSTSimA 8-LSA Code-Ready Decomposition

**Status:** `DESIGN_MATERIALIZATION / OWNER_DIRECTED_COMPLETION_CYCLE`  
**Scope:** `P1-I DESIGN ONLY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime / Paper / Shadow / Tiny-Live / Live Authority:** `NOT_GRANTED`

## 1. Boundary

FSTSimA is an independent non-Live simulation, synthetic-market, qualification, fault-injection, fidelity/calibration and validation-evidence Application. It does not own Live provider truth, Trading business decisions, broker execution truth, Guardian authority, APP-RSC resource authority, Foundation governance or promotion authority.

The prior Owner-directed market-qualification/synthetic-market candidate remains preserved and is incorporated prospectively into this decomposition without adding a ninth LSA.

Physical placement follows P1-C:

```text
FSTSimA.Contracts
FSTSimA.Domain
FSTSimA.Application
FSTSimA.Infrastructure
FSTSimA.Awareness
FSTSimA.Host
```

## 2. S-LSA-01 Simulation Time & Scenario

Components: `SimulationClock`, `ScenarioRegistry`, `ScenarioScheduler`, `SeedRegistry`, `ScenarioLineageService`.

Owns deterministic simulation time, scenario identity/version, seed/reproducibility metadata and scenario progression. Simulation time cannot masquerade as operational time authority.

## 3. S-LSA-02 Market Environment Simulation

Components: `HistoricalReplayEngine`, `SyntheticMarketGenerator`, `MarketMicrostructureModel`, `RegimeEngine`, `LiquidityVolatilityModel`.

Owns historical replay and required synthetic market generation for rare/extreme/adversarial conditions. Synthetic output remains explicitly classified and provenance-bound. Synthetic markets must preserve the market rules needed for meaningful testing rather than random-price noise.

Synthetic Market generator may be CSA-eligible only if later component decomposition proves specialized self-awareness value; eligibility grants no authority.

## 4. S-LSA-03 Provider & External Service Simulation

Components: `ProviderSimulator`, `DataDelayInjector`, `QuotaFailureSimulator`, `ProviderOutageModel`, `SchemaDriftSimulator`.

Simulates provider/service behavior without creating operational provider authority or impersonating FSAPMA authoritative state.

## 5. S-LSA-04 Broker, Exchange & Execution Simulation

Components: `BrokerSimulator`, `ExchangeSimulator`, `OrderBookExecutionModel`, `FillSlippageModel`, `OrderRaceSimulator`.

Simulates order acceptance/rejection/partial fill/cancel races/gaps/halts/liquidity constraints and broker capability differences. Simulation results can challenge Trading assumptions but cannot become broker execution truth.

## 6. S-LSA-05 Account, Capital & Settlement Simulation

Components: `SimulatedAccount`, `CapitalLedger`, `FeeSettlementModel`, `MarginLeverageConstraintModel`, `CorporateActionSettlementModel` where applicable.

Initial qualification keeps 1:1 funded exposure semantics unless separately authorized; ability to model leverage does not authorize leveraged operation.

## 7. S-LSA-06 Fault, Latency & Crisis Injection

Components: `FaultInjector`, `LatencyInjector`, `NetworkPartitionModel`, `AIKillScenarioDriver`, `ResourcePressureScenarioDriver`, `EvidenceCorruptionChallenge`.

Owns controlled failure injection for Application crashes, stale data, delayed/corrupt messages, provider/broker failure, Guardian failure, APP-RSC split/stale epoch, queued-work-after-Kill, restart/recovery and black-swan combinations.

Fault injection is simulation authority only and must never route into operational systems without separately governed test infrastructure.

## 8. S-LSA-07 Fidelity & Calibration

Components: `FidelityModel`, `CalibrationEngine`, `SimulationPaperDivergenceAnalyzer`, `ParameterCalibrationRegistry`.

Owns measurement/calibration of simulator realism against trusted historical and, when separately authorized/available, Paper/Shadow observations.

```text
S_LSA07 = FIDELITY_MEASUREMENT_AND_CALIBRATION
```

Calibration cannot rewrite accepted evidence retroactively; every parameter change is versioned and attributable.

## 9. S-LSA-08 Oracle, Evidence, Reproducibility & Validation Assessment

Components: `SimulationOracle`, `EvidenceBundleBuilder`, `ReproducibilityVerifier`, `ValidationAssessor`, `QualificationCaseRegistry`.

Owns independent assessment of simulation evidence, reproducibility and qualification readiness recommendations.

```text
S_LSA08 = INDEPENDENT_ASSESSMENT
READINESS_RECOMMENDATION != AUTHORITY
```

It must be structurally separated from S-LSA-07 enough that calibration cannot simply mark its own output valid.

## 10. Market Qualification Lifecycle

An Owner-initiated candidate market follows:

```text
OWNER INITIATION
-> MARKET DISCOVERY/MODEL CANDIDATE
-> CURRENT PROVIDER/BROKER INVENTORY CHECK
-> FSAPMA PROVIDER RESEARCH + TRADING BROKER EVALUATION IF NEEDED
-> OWNER EXTERNAL-CAPABILITY DECISION WHEN REQUIRED
-> DATA/PROVIDER QUALIFICATION
-> HISTORICAL + SYNTHETIC ENVIRONMENT
-> STRATEGY/RISK/EXECUTION GAP STUDY
-> DEEP SIMULATION
-> QUALIFICATION ASSESSMENT
-> READY_FOR_PAPER_QUALIFICATION_REVIEW (recommendation only)
-> separate Paper/Shadow authority
-> divergence/calibration loop
-> READY_FOR_TINY_LIVE_REVIEW (recommendation only)
```

No duration or sample count alone creates readiness. Evidence sufficiency is consequence-aware and must expose unresolved gaps.

## 11. Cross-Application Ownership

FSTSimA hosts the qualification laboratory, not the other Applications' business truth:

- FSAPMA owns provider research/operational data truth;
- Trading owns Market Model business semantics, strategy compatibility, Risk and broker/execution evaluation;
- Guardian owns protection/crisis qualification;
- APP-RSC owns FSATS technical resource coordination only;
- Foundation owns platform/lifecycle/security/resource authority.

All exchanges use P1-K governed contracts.

## 12. APP-RSC Reclaimability

FSTSimA declares active test obligations, minimum-safe floor, pause/degradation semantics, checkpoint/restart cost, reclaimable capacity and restoration requirements. Idle/deferrable simulation work is normally highly reclaimable when not supporting an active higher-priority validation obligation.

Resource reduction may pause simulation computation but must not corrupt accepted evidence. Evidence already committed becomes immutable/attributable and remains distinguishable from incomplete runs.

## 13. Replay/Operational Separation

Every artifact/message is classified as simulation/replay/synthetic/qualification evidence. No simulation identifier, credential, order, event or route may become Live operational authority by naming coincidence.

`SIMULATION_PASS != PAPER_AUTHORITY != LIVE_AUTHORITY`.

## 14. AI Safety and Recovery

FSTSimA AI failure follows Safety Continuity and AI Repair/Controlled Recovery. Loss of intelligent scenario generation does not authorize fabricated validation. Previously committed evidence remains available; new validation claims requiring failed intelligence stop until trustworthy recovery.

## 15. Required Later Implementation Tests

Seed reproducibility; same scenario deterministic replay; intentional nondeterminism explicitly classified; historical/synthetic contamination challenge; simulated order escaping to operational route; partial fill/cancel race; provider quota outage; broker capability mismatch; AI Kill scenario; Guardian Safety Kernel challenge; APP-RSC split brain; resource reclaim during active run; pause/resume preserving evidence; calibration changes after assessment; validator assessing its own modified oracle; candidate market with missing provider/broker; Paper readiness without authority; Tiny-Live readiness without authority; non-reproducible evidence denied.

## 16. P1-I Closure Invariants

- exactly eight LSAs;
- S-LSA-07 calibration and S-LSA-08 independent assessment remain separate;
- synthetic market generation is required and provenance-bound;
- market qualification does not self-promote into Paper/Tiny-Live/Live;
- resource reclamation cannot corrupt evidence truth;
- simulation traffic cannot become operational authority;
- FSTSimA never absorbs FSAPMA/Trading/Guardian/APP-RSC ownership.
