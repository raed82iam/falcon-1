# Falcon Self-Aware Trading Simulation Application (FSTSimA) — 8-LSA Specialized Implementation Architecture

**Package:** `FSATS-SIA-v0.1`
**Application:** `APP-SIM`
**MSA:** `MSA-SIM`
**Status:** `DESIGN_CANDIDATE`
**Operational Classification:** `NON_LIVE_ONLY`

## 1. Mission

FSTSimA provides deterministic and/or explicitly stochastic-but-reproducible simulation, replay, shadow evaluation, fault/crisis injection, fidelity calibration and independent validation assessment for FSATS candidates and behaviors.

It SHALL NOT create Live authority, broker/provider credential authority, production promotion or authoritative Trading state.

## 2. Run Classification

Every run has exactly one class:

```text
SYNTHETIC_SIMULATION
HISTORICAL_REPLAY
SHADOW_MARKET
FAULT_INJECTION
REGRESSION_FIXTURE
ADVERSARIAL_VALIDATION
```

No run is `LIVE_OPERATIONAL`.

All emitted FSTSimA contracts/events carry non-authoritative classification. Missing classification fails closed.

## 3. Reproducibility Identity

A `SimulationRunId` is bound to immutable `RunDefinition`:

```text
ScenarioId + ScenarioVersion
SimulationEngineVersion
MarketModelVersion(s)
ProviderSimulatorVersion(s)
BrokerSimulatorVersion(s)
Account/SettlementModelVersion
FaultPolicyVersion
CandidateArtifactVersions
BaselineArtifactVersions
InputDatasetSnapshotIds
RandomAlgorithmId
MasterSeed
ConfigurationDigest
StartBoundary
EndBoundary
```

Same RunDefinition + same input bytes SHALL reproduce the same deterministic event sequence and results for deterministic modes.

Any nondeterministic external dependency is forbidden inside an evidence-bearing run unless its exact response stream is captured and replayed as immutable input.

## 4. S-LSA-01 — Simulation Time & Scenario

### Components

- `S01.ScenarioRegistry`
- `S01.RunDefinitionValidator`
- `S01.SimulationClock`
- `S01.EventScheduler`
- `S01.RandomStreamRegistry`
- `S01.RunLifecycleAggregate`
- `S01.CheckpointManager`

### Simulation clock

Uses logical simulation time independent of wall-clock progress.

Clock modes:

```text
FAST_FORWARD
REALTIME_PACED
STEP
DETERMINISTIC_EVENT_DRIVEN
```

Wall-clock timestamps may be recorded as audit metadata but do not alter simulated event ordering.

### Event ordering

Canonical scheduler key:

```text
(SimulationEffectiveTime, EventPriorityClass, SourceSequence, CanonicalEventId)
```

Priority class exists only to resolve same-time simulation mechanics and is defined by scenario policy. It does not imply business authority.

### Randomness

Master seed derives named independent streams by stable hash of `(MasterSeed, ComponentStreamName)`. Adding an unrelated component stream SHALL NOT change another component's random sequence.

### Run lifecycle

```text
DEFINED
VALIDATED
READY
RUNNING
PAUSED
CHECKPOINTED
COMPLETED
FAILED
CANCELED
EVIDENCE_FROZEN
```

Only `EVIDENCE_FROZEN` output may be cited as final validation evidence.

### Resource reclaimability

A RUNNING non-critical run may checkpoint/pause under FSARM instruction if scenario permits. A frozen evidence set cannot be mutated to save resources.

### CSA

No CSA for clock/scheduler/lifecycle; deterministic infrastructure.

## 5. S-LSA-02 — Market Environment Simulation

### Components

- `S02.MarketModelRegistry`
- `S02.OrderBookMarketModel`
- `S02.TradeBarGenerator`
- `S02.RegimeScenarioEngine`
- `S02.LiquidityModel`
- `S02.VolatilityModel`
- `S02.SessionHaltModel`
- `S02.CorporateActionScenarioModel`

### Modes

- replay exact historical market stream;
- transform historical stream under controlled perturbation;
- generate synthetic market from a declared stochastic model;
- adversarial scenario templates.

### Synthetic model rule

Every synthetic output carries model/version/seed provenance. Synthetic price paths are not represented as historical or predicted truth.

### Market constraints

Simulation market profile reuses the same canonical market/instrument precision/session rules as Trading where the scenario intends production equivalence, with explicit overrides recorded as scenario parameters.

### CSA

Synthetic scenario generator/model may be CSA-eligible. The simulation scheduler and truth classification are not.

## 6. S-LSA-03 — Provider & External Service Simulation

### Components

- `S03.ProviderBehaviorRegistry`
- `S03.ProviderResponseSimulator`
- `S03.QuotaSimulator`
- `S03.StreamDisconnectSimulator`
- `S03.DataCorruptionSimulator`
- `S03.ProviderLatencySimulator`
- `S03.ExternalServiceFixtureAdapter`

### Purpose

Simulate provider/API behavior without using operational provider credentials or uncontrolled external Internet access.

### Simulated conditions

At minimum:

- normal response;
- delayed response;
- timeout;
- rate limit/quota exhaustion;
- disconnect/reconnect;
- stale payload;
- malformed payload;
- partial/missing fields;
- contradictory providers;
- correction/backfill;
- service unavailable;
- auth/entitlement denial fixture.

### Interface

Produces provider-facing fixture responses compatible with FSAPMA adapter contract tests or captured stream injection, while marking all traffic as simulation/test.

### CSA

Provider failure-pattern generator may be eligible if intelligent. Simulation classification cannot be changed by it.

## 7. S-LSA-04 — Broker, Exchange & Execution Simulation

### Components

- `S04.BrokerCapabilitySimulator`
- `S04.OrderAcceptanceModel`
- `S04.QueueFillModel`
- `S04.SlippageImpactModel`
- `S04.PartialFillModel`
- `S04.CancelReplaceModel`
- `S04.BrokerFailureSimulator`
- `S04.ExecutionEvidenceEmitter`

### Broker semantics

The simulator returns broker-like evidence, never mutates APP-TRD production order/position state.

### Fill model inputs

Where applicable:

```text
OrderType/Side/Quantity/Limit/Stop
Market/order-book state
Liquidity/depth
Queue/priority model
Latency
Participation/capacity model
Slippage/impact parameters
Venue/broker capability profile
Random stream
```

### Ambiguous execution fixtures

Must support:

- submit timeout after unknown broker acceptance;
- duplicate client id;
- cancel timeout;
- replace conflict;
- fill arriving after cancel request;
- out-of-order status;
- duplicate fill;
- correction/bust fixture;
- broker reports unknown order.

These scenarios verify APP-TRD reconciliation behavior.

### CSA

Execution simulation fidelity model may be eligible. The canonical simulated order-state generator remains governed/versioned.

## 8. S-LSA-05 — Account, Capital & Settlement Simulation

### Components

- `S05.SimulatedAccountLedger`
- `S05.SimulatedBuyingPowerModel`
- `S05.SimulatedCapitalReservationMirror`
- `S05.FeeCommissionModel`
- `S05.SettlementModel`
- `S05.CorporateActionAccountingModel`
- `S05.PnLValuationModel`

### Separation

Simulated money/positions are separate identities from production account/capital/position aggregates.

No serialization path may deserialize a simulation account record directly into APP-TRD authoritative state.

### Accounting

Uses exact decimal/currency/asset semantics. Fees/commissions/spread/slippage/settlement effects are explicit, versioned and attributable.

### CSA

No CSA by default for deterministic ledger/accounting. Estimation model may be eligible separately.

## 9. S-LSA-06 — Fault, Latency & Crisis Injection

### Components

- `S06.FaultCatalog`
- `S06.FaultScheduleEngine`
- `S06.LatencyInjectionEngine`
- `S06.ResourcePressureScenarioInjector`
- `S06.SecurityAuthorityFailureFixtureEngine`
- `S06.GuardianCrisisScenarioInjector`
- `S06.PartitionSplitBrainFixtureEngine`

### Fault identity

Each injected fault has exact:

```text
FaultId
FaultType
TargetScope
StartCondition
EndCondition
Severity
Deterministic/random parameters
Expected observable effects
Forbidden effects
```

### Required families

- stale/conflicted data;
- provider failure;
- broker ambiguity;
- resource starvation;
- queue saturation/backpressure;
- dependency unavailable;
- permission revoked mid-operation;
- Application isolation/restart;
- message duplicate/reorder/expiry;
- evidence storage failure;
- Guardian directive delivery failure;
- FSARM partial redistribution/foundation denial;
- learned/model drift fixtures;
- clock/time boundary anomalies.

### Non-authority

Fault injection exists only inside FSTSimA/test surfaces. It cannot invoke a production Guardian directive or production Foundation fault primitive.

## 10. S-LSA-07 — Fidelity & Calibration

### Components

- `S07.FidelityMetricRegistry`
- `S07.ExecutionFidelityAnalyzer`
- `S07.MarketFidelityAnalyzer`
- `S07.ProviderFidelityAnalyzer`
- `S07.DistributionCalibrationEngine`
- `S07.ParameterCalibrationCandidateBuilder`
- `S07.FidelityReportBuilder`
- `S07.FidelityCalibrationModel`

### Fidelity dimensions

At minimum:

- return/volatility distribution similarity;
- liquidity/spread/depth behavior;
- order fill/partial-fill distribution;
- slippage/impact distribution;
- latency/error pattern similarity;
- provider gap/correction behavior;
- account/fee/settlement agreement;
- crisis/fault response timing;
- strategy outcome sensitivity.

### Calibration

Calibration creates candidate parameter/model updates only. It cannot rewrite the frozen evidence of the run used to discover the mismatch.

A calibrated successor is a new version and must be revalidated.

### CSA

`FidelityCalibrationModel` may be CSA-eligible.

## 11. S-LSA-08 — Oracle, Evidence, Reproducibility & Validation Assessment

### Components

- `S08.OracleRegistry`
- `S08.ExpectedOutcomeEvaluator`
- `S08.InvariantEvaluator`
- `S08.ReproducibilityVerifier`
- `S08.BaselineCandidateComparator`
- `S08.StatisticalValidityEvaluator`
- `S08.ValidationEvidenceFreezer`
- `S08.IndependentValidationAssessor`

### Independence

```text
S07 = CALIBRATES THE SIMULATOR
S08 = ASSESSES WHETHER THE SIMULATOR/CANDIDATE EVIDENCE IS CREDIBLE
```

S07 cannot declare its own calibration successful without S08 assessment.

### Oracle types

```text
EXACT_STATE_ORACLE
INVARIANT_ORACLE
BOUNDED_NUMERIC_ORACLE
STATISTICAL_DISTRIBUTION_ORACLE
METAMORPHIC_ORACLE
DIFFERENTIAL_BASELINE_ORACLE
ADVERSARIAL_SAFETY_ORACLE
```

Every validation claim identifies its oracle type/version and acceptance criteria before the result is evaluated whenever practical.

### Candidate vs baseline

Comparison binds exact candidate and baseline artifact identities and identical/matched scenario sets. Report includes deltas with uncertainty, not only winner/loser.

### Minimum evidence record

```text
SimulationEvidenceId
RunDefinitionDigest
InputDatasetDigests
Engine/module versions
Candidate/baseline identities
Oracle versions
Result metrics
Failure list
Random seed/stream metadata
Reproducibility result
Fidelity result
Statistical validity result
Resource/performance result
Evidence artifact digests
```

### No production adoption

A PASS is validation evidence only. S08 cannot deploy, activate or approve production adoption.

## 12. Shadow Mode

Shadow evaluation may consume mirrored/published operational Data Products through a governed non-authoritative route when allowed, but:

- it cannot send broker orders;
- it cannot mutate production Trading state;
- it must mark outputs SHADOW;
- comparison to real outcomes occurs after the fact through evidence identities;
- operational Data Product use still follows FSAPMA and Foundation transport boundaries.

FCR-0011/FCR-0013/FCR-0014 constraints prevent shortcuts around external-access isolation.

## 13. Simulation Dataset Governance

A dataset snapshot is immutable and records:

- source Data Product versions;
- time range;
- instruments/markets;
- correction policy;
- inclusion/exclusion filters;
- survivorship/corporate-action handling;
- data quality state;
- digest/provenance;
- any feature/preprocessing version.

Train/validation/test partitions are identity-bound. Leakage between partitions invalidates the affected experiment.

## 14. Resource Degradation and Checkpointing

FSTSimA is generally highly reclaimable relative to active production protection obligations.

Default FSARM response order:

1. reduce parallel experiment count;
2. stop low-value queued runs;
3. checkpoint and pause eligible running runs;
4. terminate/restartable exploratory runs;
5. retain only explicit high-priority verification if required by current governance obligation.

Before reclaiming a running evidence-bearing run, FSTSimA declares whether checkpoint/restart preserves reproducibility. If not, the run is canceled and restarted later as a new run; partial evidence is not misrepresented as complete.

## 15. FSTSimA MSA

MSA-SIM understands simulation capability, fidelity, evidence quality, reproducibility, bottlenecks and candidate improvement opportunities.

It cannot declare production adoption, grant Live access, weaken non-Live classification or rewrite oracle results.

Two independent Monitor AI perspectives apply under file 18.

## 16. Verification Families

FSTSimA verifier SHALL cover at least:

1. exactly 8 LSAs + one MSA;
2. every run explicitly non-Live classified;
3. run-definition immutability;
4. deterministic scheduler ordering;
5. named random streams stable against unrelated stream addition;
6. deterministic replay reproducibility;
7. provider/broker failure fixtures;
8. simulated account identity separation;
9. no production credential/egress shortcut;
10. no simulation event can become operational authority;
11. S07 calibration cannot alter frozen source evidence;
12. S08 independent from S07 success claim;
13. exact oracle/acceptance criteria binding;
14. candidate/baseline matched comparison;
15. data partition/leakage checks;
16. checkpoint/resource reclaimability truth;
17. resource reclamation cannot corrupt frozen evidence;
18. shadow cannot place orders;
19. validation PASS != promotion/adoption;
20. deterministic rerun/evidence digest equivalence;
21. CSA/model cannot weaken simulation classification/oracle authority.
