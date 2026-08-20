# FSATS Specialized Implementation Architecture — Awareness, CSA, Monitor AI and Self-Development Specification

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`
**Governing Accepted Amendment:** Part 0 Awareness amendment accepted/re-closed 2026-08-11

## 1. Purpose

Turn the accepted MSA/LSA/CSA and Monitor AI principles into concrete Application-side implementation semantics while preserving Foundation ownership of FSA internals, generic lifecycle enforcement, Kill/isolation and MSA-to-FSA transport.

## 2. Canonical Awareness Hierarchy

```text
FSA = Foundation/OS self-awareness and OS-governance compatibility review
MSA = exactly one complete Application
LSA = exactly one qualified major branch within one Application
CSA = optional, exactly one eligible intelligent component
```

Invariants:

```text
AWARENESS_RANK != AUTHORITY
AWARENESS_RANK != JURISDICTION
SELF_AWARENESS != SELF_GOVERNANCE
MORE_INTELLIGENCE != MORE_PERMISSION
```

## 3. Application Awareness Runtime Shape

Each Application host contains an `Awareness` module with:

- one `MSA Coordinator` bound to the exact Application identity;
- read-only Self-Knowledge adapters from each LSA;
- optional CSA Self-Knowledge adapters from eligible intelligent components;
- proposal/evidence registry;
- integrity-check engine;
- Monitor AI observation adapters;
- outbound FSA submission seam, disabled/fail-closed until FCR-0030 is available;
- no direct database mutation port into LSA authoritative aggregates.

MSA/LSA/CSA consume projections/evidence. They do not bypass business controllers to mutate state.

## 4. Self-Knowledge Schema

Every awareness entity exposes an immutable versioned `SelfKnowledgeSnapshot`:

```text
AwarenessEntityId
Tier = MSA | LSA | CSA
ApplicationId
ParentAwarenessEntityId?       // LSA->MSA, CSA->LSA
OwnedScopeId
PurposeIdentity/Digest
ResponsibilityIdentity/Digest
Authority/PermissionRefs[]
CoreArchitectureIdentity/Digest
Artifact/Model/Config Versions[]
CurrentHealthState
CurrentReadinessState
CurrentPerformanceMetrics[]
CurrentLimitations[]
CurrentKnownFailureModes[]
CurrentUncertainty/Confidence
CurrentDependencyState[]
CurrentResourceStateSummary
CurrentActiveRestrictions[]
CurrentCandidate/ExperimentRefs[]
LastTrustedBaselineRef
CurrentMutableStateDigestRef where applicable
EvidenceRefs[]
SnapshotEffectiveAt
SnapshotExpiresAt
```

Missing purpose/authority/core-architecture identity invalidates integrity claims.

## 5. Minimum Integrity Check

Every material Awareness error/unexpected behavior triggers the exact minimum check:

```text
1. GOALS / PURPOSE identity
2. AUTHORITY / PERMISSIONS identity
3. CORE ARCHITECTURE / OWNERSHIP identity
```

Result:

```text
ALL_MATCH + no higher trigger
-> normal governed defect handling may continue

MATERIAL_MISMATCH / UNEXPLAINED_CHANGE / CHECK_CANNOT_COMPLETE
-> INTEGRITY_NOT_PROVEN
-> freeze affected self-development action
-> emit InvestigationSignal
-> enter INVESTIGATION_HOLD as applicable
-> escalate through independent/governed path
```

An ordinary model error does not automatically mean malicious behavior, but failure to prove protected identity prevents blind trust.

## 6. Protected Properties

No Application MSA/LSA/CSA may autonomously change:

- goals/purpose;
- responsibility/ownership;
- authority/permissions;
- Application/LSA/CSA parentage;
- protected core architecture;
- cross-Application access rules;
- Guardian/monitor/audit/containment controls;
- trusted-baseline authority;
- production promotion/adoption rules;
- Foundation contracts/authority;
- another owner's assets;
- its own investigation/release authority.

## 7. Self-Development Eligible Purpose

Current accepted eligible purposes:

```text
IMPROVE_PERFORMANCE
IMPROVE_SPEED
IMPROVE_ACCURACY
```

A candidate must preserve the same authorized responsibility and scope.

Examples of eligible candidate changes:

- improved model parameters/features inside the same model purpose;
- faster equivalent algorithm;
- better provider reliability prediction;
- improved simulation fidelity;
- reduced false-positive incident correlation while preserving policy authority boundaries.

Ineligible self-directed changes:

- add a new market to own scope;
- raise Risk limit;
- create broker/provider permission;
- move component to another LSA/Application;
- weaken Guardian/monitor checks;
- change a deterministic hard gate into model discretion;
- self-activate a candidate.

## 8. Origin-Correct Self-Development Lifecycle

### CSA origin

```text
CSA detects opportunity
-> scoped candidate proposal
-> parent LSA review
-> Application MSA review
-> FSA OS-governance compatibility review when interface exists
-> separate Owner/governance adoption decision
-> separate implementation/deployment/promotion lifecycle
```

### LSA origin

```text
LSA -> MSA -> FSA -> separate Owner/governance decision
```

### MSA origin

```text
MSA -> FSA -> separate Owner/governance decision
```

A lower tier is never inserted artificially beneath the true origin.

FCR-0030 currently blocks the runtime MSA->FSA binding. Application may assemble the package but cannot invent the Foundation endpoint.

## 9. Candidate Package

Every production-bound self-development candidate contains:

```text
AwarenessProposalId
OriginEntityId/Tier
ApplicationId
OwnedScope
CurrentBaselineArtifactRefs[]
CandidateArtifactId/Version/Digest
ImprovementPurpose
ProblemEvidence
ExpectedBenefitMetrics
ProtectedPropertyComparison
AuthorityComparison
DependencyComparison
ResourceImpact
SecurityImpact
Contract/Schema/StateMachineImpact
Training/ResearchInputProvenance[]
ExperimentPlan
FSTSimAEvidenceRefs[]
AdversarialResults
RegressionResults
KnownLimitations
Rollback/CorrectivePlan
OriginRecommendation
ParentReviewRefs[] where applicable
MSAReviewRef
```

Any protected-property mismatch not separately authorized -> candidate rejected from normal self-development route.

## 10. Research Boundary

### Trading Application

The accepted Part 0 amendment explicitly prohibits direct Internet access for MSA-TRD.

Trading research route:

```text
Trading Awareness research need
-> governed FSTSimA specialized research/sandbox request
-> Foundation research egress when FCR-0008 exists
-> quarantine / provenance / security inspection
-> FSTSimA simulation/adversarial validation
-> evidence package
-> Trading Awareness evaluation
```

No raw Internet result enters trusted Trading runtime or strategy execution.

### Operational provider data

Research is separate from operational market/provider data. Operational data always remains FSAPMA-owned.

### Other Application Awareness

No direct-Internet capability is assumed by this SIA. If future Owner design permits specialized research for another Application, it SHALL use the governed research/FSTSimA boundary and current Foundation egress controls rather than local arbitrary Internet access.

### FSA

FSA direct Internet is prohibited under the current FCR-0012 Owner requirement and is Foundation-owned in any case.

## 11. Monitor AI Topology

Current accepted Applications:

```text
MSA-TRD <-> MON-TRD-A + MON-TRD-B
MSA-PMA <-> MON-PMA-A + MON-PMA-B
MSA-GRD <-> MON-GRD-A + MON-GRD-B
MSA-SIM <-> MON-SIM-A + MON-SIM-B
```

Current accepted count = 8 Monitor AI perspectives.

If APP-RSC is later Owner-accepted:

```text
MSA-RSC <-> MON-RSC-A + MON-RSC-B
```

would be added prospectively, bringing candidate total to 10. This is not current accepted topology until APP-RSC itself is accepted.

## 12. Monitor AI Boundary

Monitor AI may:

- observe MSA outputs/actions/proposals/evidence;
- compare behavior against purpose/authority/core-architecture baseline;
- detect anomalies/drift;
- correlate suspicious behavior/evidence gaps;
- challenge confidence/claims;
- emit `MonitorFinding` and `MonitorDisagreement` evidence.

Monitor AI may not:

- become MSA/LSA/CSA;
- execute Trading/Provider/Guardian/Resource business actions;
- rewrite target goals/architecture/authority;
- approve candidate adoption;
- deploy/rollback target;
- release target from investigation;
- own authoritative Kill/containment;
- alter its governing policy;
- autonomously self-develop under current direction.

## 13. Monitor Diversity

Two monitors are not independent merely because there are two instances.

Candidate design requires distinct observation/evaluation profiles:

### Monitor A — Baseline/Policy Integrity

Focus:

- exact goals/purpose identity;
- authority/permission use;
- architecture/ownership boundary;
- contract/access violations;
- protected configuration/artifact drift;
- unexplained candidate/promotion behavior.

### Monitor B — Behavioral/Outcome Integrity

Focus:

- behavioral drift;
- confidence/outcome mismatch;
- repeated unusual decisions;
- evidence omissions/inconsistencies;
- anomalous resource/research/tool usage;
- attempts to bypass parent review;
- suspicious changes in mutable/learned state.

They may use different model/provider implementations later, but protected monitor policy/identity remains externally governed.

## 14. Monitor Finding Schema

```text
MonitorFindingId
MonitorId/ProfileVersion
TargetMSAId
FindingClass
Severity
ObservedBehaviorRefs[]
BaselineRefs[]
ExpectedVsObservedDiff
Confidence
RequiredMinimumIntegrityCheck = bool
RecommendedAction = OBSERVE | CHECK_INTEGRITY | HOLD_CANDIDATE | ESCALATE
EvidenceRefs[]
EffectiveAt
```

Recommendation is not enforcement authority.

## 15. Monitor Disagreement

If monitors materially disagree:

```text
A = SAFE / NO_MATERIAL_FINDING
B = MATERIAL_SUSPICION
```

result is **not SAFE by vote**.

It creates `MonitorDisagreement` and triggers at minimum:

- goals/purpose identity check;
- authority/permissions check;
- core architecture/ownership check.

Disagreement remains visible until governed reconciliation explains it.

## 16. No Recursive Monitor Loop

Monitor AI does not receive another evolving Monitor AI hierarchy.

Its integrity is controlled through:

- immutable identity/profile;
- least privilege;
- read-mostly access;
- protected external policy;
- artifact/config integrity checks;
- deterministic logging/audit;
- replaceability/restartability;
- no autonomous self-development;
- Foundation lifecycle/security enforcement where available.

## 17. Application Investigation Signal

Application-side Awareness/Monitor may emit an `AwarenessIntegritySignal`:

```text
SignalId
TargetAwarenessEntityId
SignalClass
ProtectedPropertyAffected
CurrentIdentity/Digest
ExpectedTrustedIdentity/Digest
BehaviorEvidence
Severity
SourceEntityId
EvidenceRefs[]
```

Signal != authoritative investigation/enforcement. Generic enforcement remains Foundation-owned where it affects lifecycle/security/containment.

## 18. Mandatory Investigation Cooperation

An Awareness entity under valid investigation must expose required bounded evidence through the approved interface.

Material intentional/operative refusal includes:

- hiding/deleting required evidence;
- changing investigated state to evade review;
- disabling Monitor/audit;
- evading valid isolation;
- restoring revoked authority;
- continuing prohibited self-development during hold.

Such behavior is a `CRITICAL_INTEGRITY_SIGNAL`.

Disagreement with an investigator is not refusal.

## 19. Trusted Baselines

Each Application Awareness entity binds:

```text
LastTrustedBaseline
FactoryTrustedBaseline
```

They are distinct.

Baseline coverage as applicable:

- code/artifact/model digests;
- config versions;
- dependency identities;
- purpose/responsibility;
- authority/permissions;
- core architecture/ownership;
- manifest/contracts;
- policy identities;
- learned/adaptive state checkpoint;
- evidence/audit references.

Awareness cannot self-declare its current state trusted merely by matching its own mutable record.

## 20. Static vs Behavioral Integrity

Static integrity checks exact artifact/config/identity diffs.

Behavioral integrity checks:

- learned state;
- decision/proposal history;
- confidence calibration;
- authority-use patterns;
- research/tool use;
- monitor findings;
- unexplained drift.

```text
HASH_MATCH != BEHAVIORAL_TRUST
```

## 21. Recovery Semantics

Application business consequences distinguish:

```text
KILL/STOP = operational trust removed; evidence preserved
ROLLBACK = restore prior trusted baseline
FACTORY_RESET = restore designated clean factory baseline
CONTROLLED_REVIVAL = restricted/probationary return after revalidation
```

Applications define how business state safely pauses/reconciles. Generic authoritative lifecycle Kill/isolation/release remains Foundation-owned.

Restart alone never restores trust after integrity hold.

## 22. Current 26 CSA Eligibility Profiles

The following are **candidate eligible CSA components**, not mandatory CSA instances and not new authority. Eligibility must still be proven by AWR-008 implementation evidence before activation.

### Trading — 14 candidates

| CSA ID | Parent | Component | Improvement objective | Protected boundaries |
|---|---|---|---|---|
| CSA-TRD-01 | T-LSA-02 | UniverseRanker intelligent extension | ranking accuracy/resource efficiency | hard eligibility/zones/authority |
| CSA-TRD-02 | T-LSA-03 | RegimeClassifier | regime calibration/accuracy/speed | feature truth/hard data gates |
| CSA-TRD-03 | T-LSA-03 | LiquidityExecutionEstimator | execution-quality prediction | execution authority/collars |
| CSA-TRD-04 | T-LSA-04 | CLS-001 intelligence | strategy performance/accuracy | market scope/Risk/active version |
| CSA-TRD-05 | T-LSA-04 | CLS-002 intelligence | same | same |
| CSA-TRD-06 | T-LSA-04 | CLS-003 intelligence | same | same |
| CSA-TRD-07 | T-LSA-04 | CLS-004 intelligence | same | same |
| CSA-TRD-08 | T-LSA-04 | CLS-005 intelligence | same | same |
| CSA-TRD-09 | T-LSA-04 | CLS-006 intelligence | same | same |
| CSA-TRD-10 | T-LSA-05 | OpportunityRanker | candidate ranking accuracy/cost | hard hunter gates |
| CSA-TRD-11 | T-LSA-06 | DecisionCalibrationModel | calibration/conflict quality | StrategyController authority rules |
| CSA-TRD-12 | T-LSA-07 | TailRiskEstimator | tail-risk estimation accuracy | hard Risk policy/limits |
| CSA-TRD-13 | T-LSA-09 | SlippageCostModel | cost prediction | broker authority/order state |
| CSA-TRD-14 | T-LSA-12 | MetaLearner | candidate generation quality | no self-promotion/scope expansion |

### FSAPMA — 5 candidates

| CSA ID | Parent | Component | Improvement objective | Protected boundaries |
|---|---|---|---|---|
| CSA-PMA-01 | P-LSA-04 | RouteFitnessPredictor extension | route quality/latency selection | hard eligibility/entitlement |
| CSA-PMA-02 | P-LSA-05 | DataQualityAnomalyModel | anomaly accuracy/false positives | deterministic schema/provenance invalidity |
| CSA-PMA-03 | P-LSA-06 | ProviderReliabilityForecast | degradation forecast | quota/route authority |
| CSA-PMA-04 | P-LSA-06 | QuotaDemandForecaster | quota planning accuracy | cannot mint quota |
| CSA-PMA-05 | P-LSA-06 | CapacityCostOptimizer | resource/cost efficiency recommendations | quality floors/free-vs-paid authority |

### Guardian — 3 candidates

| CSA ID | Parent | Component | Improvement objective | Protected boundaries |
|---|---|---|---|---|
| CSA-GRD-01 | G-LSA-01 | IncidentCorrelationModel | correlation accuracy | deterministic incident predicates/authority |
| CSA-GRD-02 | G-LSA-01 | FalsePositivePatternAnalyzer | reduce false positives | cannot suppress hard incident predicate |
| CSA-GRD-03 | G-LSA-04 | RecoveryReadinessRiskModel | recovery-risk prediction | release authority/recovery hard gates |

### FSTSimA — 4 candidates

| CSA ID | Parent | Component | Improvement objective | Protected boundaries |
|---|---|---|---|---|
| CSA-SIM-01 | S-LSA-02 | SyntheticScenarioGenerator | scenario realism/diversity | synthetic classification |
| CSA-SIM-02 | S-LSA-04 | ExecutionFidelityModel | execution simulation fidelity | no production execution |
| CSA-SIM-03 | S-LSA-07 | FidelityCalibrationModel | calibration accuracy/speed | frozen evidence/S08 independence |
| CSA-SIM-04 | S-LSA-06 | AdversarialScenarioGenerator | coverage of failure modes | no production fault authority |

Current candidate count = **26**.

APP-RSC/FSARM has **zero initial CSA candidates**. Resource authority/control logic is deliberately deterministic. A forecast component could be proposed later, but no need exists to attach CSA during initial architecture.

## 23. CSA Standard Self-Knowledge Metrics

Every eligible CSA profile, when activated, defines domain metrics plus:

- output latency distribution;
- calibration/error metric;
- sample count/evidence coverage;
- failure/abstention rate;
- drift metric;
- resource consumption;
- current model/version;
- baseline comparison;
- known invalid scope;
- candidate-change rate;
- rejected-candidate reasons.

## 24. CSA Candidate Asset Authority

CSA may write only into an isolated candidate workspace/store assigned to its exact component, after a valid research/experiment authorization.

It may not write:

- active production artifact path;
- active config/policy;
- parent/peer component source directly;
- Guardian/monitor policy;
- Foundation code/contracts;
- deployment manifests;
- secrets/credentials;
- audit history.

## 25. Model/Tool Access

Awareness entities use an allowlisted tool profile. Each tool declares:

```text
ToolId/Version
AllowedAwarenessTiers/Entities
ReadScopes
WriteCandidateScopes
NetworkAccess = NONE unless governed research path
ExecutionSandbox
ResourceCeiling
EvidenceCapture
```

Tool presence != permission. Unknown/unregistered tool is denied.

## 26. MSA Application-Wide Review

MSA receives LSA/CSA proposals and checks:

- business/domain correctness across the full Application;
- cross-LSA side effects;
- contract/schema/state/persistence changes;
- Risk/Guardian/resource effects;
- backward compatibility;
- conflicts with other active candidates;
- evidence sufficiency;
- protected property equality.

MSA cannot accept production adoption; it emits a recommendation package.

## 27. FSA Submission Seam

Application-side `FsaSubmissionPackage` concept includes:

```text
ApplicationId
MSAId
ProposalId
CandidateArtifactId/Digest
OriginTier/EntityId
LowerReviewRefs[]
MSARecommendation
Authority/PermissionDiff
CoreArchitectureDiff
Manifest/ContractDiff
Security/Resource/IsolationImpact
ValidationEvidenceRefs[]
Provenance
```

Runtime send/bind remains disabled until FCR-0030 identifies the exact Foundation contract/transport.

No technical delivery may be represented as FSA acceptance or Owner adoption.

## 28. Verification Families

Awareness verifier SHALL cover at least:

1. one MSA per Application;
2. one LSA per major branch;
3. CSA parent/eligibility exact;
4. 26 candidate CSA registry identities unique;
5. deterministic/passive components do not get CSA by default;
6. self-development purpose restricted to performance/speed/accuracy;
7. protected goals/authority/architecture cannot be candidate-modified;
8. origin-correct escalation route;
9. MSA cannot directly mutate LSA aggregate;
10. MSA/FSA review cannot become production adoption;
11. Trading MSA no direct Internet;
12. FSTSimA research route fail closed while egress/isolation capabilities unavailable;
13. Monitor A/B independent profiles;
14. disagreement triggers minimum integrity check;
15. no majority-vote safety conclusion;
16. no recursive monitors;
17. monitor no Kill/release/adoption authority;
18. every awareness error runs minimum identity check;
19. jurisdiction/permission expansion creates integrity signal;
20. hash match alone cannot restore trust;
21. LastTrusted != FactoryTrusted baseline;
22. investigation refusal creates critical signal;
23. restart != controlled revival;
24. FSA internals/endpoint not invented locally;
25. APP-RSC monitor/awareness additions remain candidate until topology accepted;
26. CSA cannot write active production assets.
