# FSATS Specialized Implementation Architecture — Traceability, Verification and Coding-Worker Contract

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE / PRE-REVIEW`
**Implementation Authority:** `NOT_GRANTED`

## 1. Purpose

Make every material design claim traceable to a governing requirement and to a future executable verification surface. Define the rules a coding worker/Codex must follow if implementation is later separately authorized.

## 2. Traceability Chain

Every material requirement follows:

```text
SOURCE / OWNER REQUIREMENT
-> SIA REQUIREMENT
-> APPLICATION
-> LSA / COMPONENT
-> TYPE / CONTRACT / STATE MACHINE / ALGORITHM
-> PERSISTENCE / RUNTIME / SECURITY RULE
-> VERIFIER / FIXTURE
-> IMPLEMENTATION ARTIFACT when later authorized
-> EXECUTABLE EVIDENCE
```

No implementation artifact may become the first place a material semantic is invented.

## 3. Governing Source Traceability

| Source requirement | SIA materialization | Verification |
|---|---|---|
| Vision Protect > Manage > Grow | files 03,07,09,11,19 | risk/protection/resource priority negative tests |
| Constitution authority bounded/no self-expansion | 03,05,09,11,18,19 | authority/security/Awareness verifiers |
| APP-001 independent Applications | 03,05,06 | topology/manifest/project-reference tests |
| APP-001 one MSA/Application | 03,05,18 | topology/manifest/Awareness tests |
| APP-001 one LSA/major branch | 03,07-10,11 candidate | topology/Awareness tests |
| CON-023 complete declarations | 05 | ManifestVerifier |
| undeclared capability/route/resource denied | 05,12,12A,19 | manifest/contract/security negative tests |
| ADR-I012 no special FSATS Foundation path | 03,06,11,19 | architecture/reference/dependency tests |
| ADR-I012 cross-App contracts only | 03,06,12,12A | project + route + direct-access tests |
| ADR-I015 MSA domain/FSA OS boundary | 18 | Awareness verifier |
| FSA review != adoption | 18 | candidate lifecycle tests |
| FCR-0004 Guardian route | 09,12/12A | future binding fixtures; remains open until code |
| FCR-0005 operational data delivery | 08,12/12A | future binding fixtures; remains open until code |
| FCR-0006 event/evidence/replay | 12/12A,13,14,19 | replay/inbox/outbox/event fixtures |
| FCR-0010 resource pressure/load shedding | 07 T13,08 P06,09 G03,10 resource handling,11 | future Foundation binding tests |
| FCR-0031 FSARM envelope/internal-first/additional-second | 11,12/12A,13,14 | FSARM verifier |
| FCR-0012 FSA internals Foundation-owned | 18 | no local FSA implementation/reference tests |
| FCR-0030 MSA->FSA binding Foundation-owned | 18 | adapter seam disabled/pending tests |
| FCR-0008/0011 research/non-Live egress | 10,18 | FSTSimA/research fail-closed tests |
| FCR-0013 provider egress | 08,16,19 | no direct provider runtime shortcut |
| FCR-0014 broker egress | 07 T09,16,19 | no direct credential/egress shortcut |
| FCR-0016 canonical artifact consumption | 06 | FoundationAdapter build gate |

## 4. Accepted P0 Work-Package Coverage

P0/P1 are treated as archive/reference in this rebuild, but accepted P0 semantics are not discarded by omission.

| Historical Part 0 subject | SIA coverage |
|---|---|
| P0-A Governance/Authority/Evidence | 01,05,18,19,20 |
| P0-B Requirements/Historical Knowledge/Traceability | 02,20 |
| P0-C Application Topology/Awareness/Learning/Research/Evolution | 03,07-11,18 |
| P0-D Foundation Capability/Contract/Runtime Readiness | 01,05,06,11,19 |
| P0-E Identity/Manifest/Lifecycle/Deployment Eligibility | 05,13,18,19 |
| P0-F Cross-App Contracts/Authority/Security/Information Flow | 12,12A,19 |
| P0-G FSAPMA Operational Data Fabric | 08,12/12A,16 |
| P0-H Trading Core/13 LSA/TARC history | 07,17,18; TARC system-wide role superseded by FSARM design |
| P0-I Guardian Protection/Crisis/Recovery | 09,12/12A,13,19 |
| P0-J Performance/Resource/QoS/Overload/Resilience | 11,15,19 |
| P0-K Validation/Credibility/FSTSimA/Promotion | 10,17,18,20 |
| P0-L End-to-End Integration/Readiness Gate | 20-23 |

## 5. Exact P0-F Contract Preservation

Fresh reconciliation found that the first SIA contract inventory was incomplete relative to the accepted 43-family baseline.

`12A_ACCEPTED_43_CONTRACT_BASELINE_RECONCILIATION_AND_FSARM_EXTENSION.md` remediates this at design-candidate level:

```text
ACCEPTED BASELINE = 43/43 PRESERVED
UNEXPLAINED DROPS = 0
APP-RSC NEW FAMILIES = 16 CANDIDATE ADDITIONS
TOTAL IF APP-RSC ACCEPTED = 59
```

This finding/remediation SHALL appear in the fresh review record; it must not disappear from history merely because it was corrected before final Owner review.

## 6. V1.3 Quality/Knowledge Coverage

Historical V1.3 package patterns preserved/replaced:

| V1.3 fact/pattern | SIA disposition |
|---|---|
| 2 markets | preserved: US Equities + Crypto Spot |
| funded 1:1 | preserved |
| 13 provider pool | preserved as current certification candidates in file 16 |
| 7 historical initial active targets | preserved as historical target-count evidence, but exact current active set requires current certification rather than blind freezing |
| 2 trading schools | preserved |
| 10 strategy models historical | expanded to 14 current SIA strategy families based on later design discussions; material delta documented here |
| 12 historical LSA rooms | superseded by current accepted 31-LSA four-App topology; APP-RSC candidate would make 34 |
| central strategy catalog/controller | preserved/specialized |
| provider routing/quota/quality | preserved/specialized |
| Unified Risk | preserved/specialized |
| capital reservation | preserved/specialized |
| simulator/replay/evidence | preserved and separated as FSTSimA |
| semantic/schema/traceability/state/structural validation | reproduced as dedicated verifier families |

### Strategy-count delta

```text
V1.3 HISTORICAL STRATEGY MODELS = 10
CURRENT SIA STRATEGY FAMILIES = 14
```

The four additional families are not claimed to have been part of V1.3. They are prospective design additions and require the same fresh review/Owner acceptance as the rest of this SIA candidate.

## 7. Material Semantic Delta Register

The package SHALL NOT hide material differences from accepted/historical design.

### DELTA-SIA-001 — FSARM runtime identity

Proposed: add dedicated APP-RSC Resource Management Application.

Effect if accepted:

```text
Applications 4 -> 5
MSA 4 -> 5
LSA 31 -> 34
```

Reason: avoid privileged placement inside a peer Application or hidden FSATS stateful principal while satisfying FCR-0031 coordination semantics.

Requires explicit Owner decision.

### DELTA-SIA-002 — Strategy catalog expansion

Historical V1.3 10 -> SIA 14 strategy families. Exact algorithm versions defined in file 17.

Requires Owner acceptance with the package.

### DELTA-SIA-003 — Contract extensions for APP-RSC

Accepted baseline 43 remains. APP-RSC adds 16 exact bilateral candidate families, producing 59 if accepted.

Requires APP-RSC decision first.

### DELTA-SIA-004 — Explicit 26 CSA candidate eligibility profiles

Adds implementation-specific CSA candidate registry without granting CSA authority. Current accepted Awareness topology/eligibility rules remain unchanged.

### DELTA-SIA-005 — Physical project/assembly architecture

Introduces one LSA assembly per major branch, dedicated Application hosts/contracts/persistence/Foundation adapters. This is a prospective implementation architecture and requires package acceptance before code.

## 8. Required Verifier Projects

If implementation is authorized, create dedicated executable verifier projects rather than relying only on generic unit tests:

```text
verification/Falcon.FSATS.PackageStructure.Verifier
verification/Falcon.FSATS.Topology.Verifier
verification/Falcon.FSATS.CanonicalTypes.Verifier
verification/Falcon.FSATS.Manifest.Verifier
verification/Falcon.FSATS.ProjectDependency.Verifier
verification/Falcon.FSATS.ContractSchemaRoute.Verifier
verification/Falcon.FSATS.StateMachine.Verifier
verification/Falcon.FSATS.Trading.Verifier
verification/Falcon.FSATS.ProviderManagement.Verifier
verification/Falcon.FSATS.Guardian.Verifier
verification/Falcon.FSATS.TradingSimulation.Verifier
verification/Falcon.FSATS.ResourceManagement.Verifier          // only if APP-RSC accepted
verification/Falcon.FSATS.PersistenceConcurrency.Verifier
verification/Falcon.FSATS.RuntimeOverload.Verifier
verification/Falcon.FSATS.MarketProviderBrokerProfile.Verifier
verification/Falcon.FSATS.StrategyIntelligence.Verifier
verification/Falcon.FSATS.Awareness.Verifier
verification/Falcon.FSATS.SecurityAuthority.Verifier
verification/Falcon.FSATS.Traceability.Verifier
verification/Falcon.FSATS.DeterminismReplay.Verifier
```

Architecture and security test projects are separate from WP/domain verifiers.

## 9. Package Structure Verifier

Checks:

- exact expected projects/files after implementation;
- no unauthorized Foundation/reference file copy;
- no hidden common mutable service;
- no FSARM project unless APP-RSC accepted;
- no unregistered provider/broker adapter;
- no test project referenced by production;
- manifest/contract/schema/config inventory complete.

## 10. Topology / Dependency Verifier

Checks every rule in files 03 and 06, including:

- Application count/topology exact for accepted version;
- MSA/LSA counts;
- cross-App implementation references forbidden;
- no direct database/internal access;
- provider adapter only FSAPMA;
- broker adapter only Trading;
- FSTSimA non-authoritative dependency use;
- Foundation artifacts only behind FoundationAdapters.

## 11. Schema / Contract Verifier

Generates fixtures from files 04/12/12A:

- required/missing/unknown fields;
- enum/type/range/unit rules;
- all 43 baseline families present;
- new APP-RSC families present only when topology accepted;
- bilateral participant exactness;
- schema compatibility;
- same-ID/different-digest mutation;
- operational/replay/simulation classification;
- Guardian authority/scope;
- Web intent not authority;
- Communication delivery not business success;
- resource report/ACK not grant/reclaim effect.

## 12. State Machine Verifier

The implementation state machines SHALL expose a machine-readable transition description or test adapter sufficient to enumerate:

- all states;
- all allowed transitions;
- all undeclared transitions as rejects;
- terminal behavior;
- idempotency/conflicting duplicates;
- stale sequence/version;
- correction/supersession;
- event-stream deterministic reconstruction.

A test suite that only exercises happy transitions is insufficient.

## 13. Domain Verifiers

Each Application verifier executes positive/negative/adversarial cases from files 07-11.

Required classes include:

- exact decision/risk/capital/execution chain;
- provider raw->normalized->quality->delivery;
- Guardian signal->incident->authority->directive->effect->recovery;
- FSTSimA run/reproducibility/oracle/fidelity isolation;
- FSARM envelope/donor/reclaim/remaining-deficit/request/outcome/restoration if accepted.

## 14. Persistence/Concurrency Verifier

Must use actual concurrent execution to prove:

- no capital double reservation;
- no quota oversubscription;
- one order attempt identity before broker call;
- ambiguous crash/recovery;
- fill/order/position/capital atomic invariants;
- outbox/inbox atomicity;
- stale optimistic write rejection;
- snapshot/event reconstruction;
- APP-RSC fencing if accepted.

## 15. Strategy/Intelligence Formula Verifier

For every StrategyVersion/ModelVersion in file 17, maintain golden fixture vectors with:

```text
Input observations/features
Expected intermediate feature values
Expected applicability result
Expected trigger/no-trigger
Expected Entry/Stop/Target hypotheses
Expected confidence/raw score
Expected reason codes
```

One-field mutation/adversarial vectors prove no hidden dependence/parameter drift.

The verifier must parse the active versioned parameter profile and compare it to expected version identity/digest.

## 16. Deterministic Replay Verifier

At minimum replay:

1. Trading decision cycle from pinned DataProduct snapshots to proposal;
2. risk + capital reservation;
3. order event stream to current state;
4. provider normalization/reconciliation;
5. Guardian incident/directive lifecycle;
6. FSTSimA deterministic run;
7. FSARM plan from exact ResourcePicture if accepted.

Run twice in a clean process/environment with the same exact inputs/config/versions. Compare canonical state/evidence digests, allowing only explicitly excluded wall-clock diagnostic metadata.

## 17. Mutation Testing

Verifier suite SHALL include semantic mutation tests such as:

- swap producer/consumer;
- change schema version;
- change unit/decimal precision;
- change authority ref;
- change environment classification;
- remove Guardian scope;
- flip Risk DENY to ALLOW;
- increase capital reservation by one unit;
- reorder fill/cancel events;
- replay older coordinator epoch;
- change strategy threshold/version without updating digest;
- remove one accepted contract family;
- convert simulation evidence to operational class;
- disable Monitor check;
- omit provenance/evidence.

Expected result = deterministic failure/rejection.

## 18. Security Verifier

Static + executable checks:

- no literal secrets/API keys in repo/config/fixtures;
- no cross-App implementation refs/DB access;
- external payload fuzzing/bounds;
- forged/replayed command rejection;
- environment swap rejection;
- path/config/serialization attack fixtures;
- dependency/version downgrade fixtures;
- queue/retry abuse;
- CSA/Monitor protected-control mutation denial.

## 19. Performance Verification

Architecture does not fabricate final SLO thresholds, but implementation must provide measurable benchmark profiles for:

- DataProduct ingress->feature update;
- qualified candidate->TradeProposal;
- Risk evaluation;
- capital reservation;
- intent persistence->broker dispatch start;
- provider normalization/quality;
- Guardian signal->directive decision;
- resource plan build;
- queue saturation/degradation behavior;
- FSTSimA throughput/reproducibility.

Before Paper/Tiny Live/Live promotion, a governed profile binds exact acceptance thresholds using measured evidence.

## 20. Traceability Manifest

Implementation shall generate machine-readable `FSATS_TRACEABILITY.json` with rows:

```json
{
  "requirement_id": "SIA-TRD-T09-AMBIGUOUS-RECONCILE",
  "source_refs": ["17...", "13..."],
  "owner": "APP-TRD/T-LSA-09",
  "implementation_refs": [],
  "test_refs": [],
  "verifier_scenarios": [],
  "status": "DESIGN_ONLY"
}
```

After implementation authorization, blank implementation/test refs for an implemented requirement are verifier failures.

## 21. Requirement ID Scheme

```text
SIA-<APP/COMMON>-<LSA/SUBSYSTEM>-<SHORT-NAME>
```

Examples:

```text
SIA-TRD-T08-CAPITAL-RESERVATION-ATOMIC
SIA-TRD-T09-AMBIGUOUS-RECONCILE
SIA-PMA-P05-CONFLICTED-DATA-FAIL-CLOSED
SIA-GRD-G02-DIRECTIVE-SCOPE-AUTHORITY
SIA-SIM-S08-VALIDATION-NOT-PROMOTION
SIA-RSC-R02-RECLAIM-BEFORE-REASSIGN
SIA-AWR-MONITOR-DISAGREEMENT-INTEGRITY-CHECK
SIA-COMMON-43-CONTRACT-BASELINE-PRESERVED
```

## 22. Coding-Worker / Codex Contract

If later separately authorized to implement this SIA, the coding worker SHALL obey:

### 22.1 Source-first

Before modifying code for a requirement, read:

- exact accepted SIA semantic freeze;
- governing Foundation contract/identity;
- related FCR current header;
- predecessor implementation/review evidence where applicable.

### 22.2 No semantic invention

```text
IF MATERIAL SEMANTIC IS MISSING OR CONTRADICTORY
-> STOP AFFECTED IMPLEMENTATION
-> REPORT EXACT GAP
-> DO NOT CHOOSE AN ALGORITHM/AUTHORITY/STATE/SCHEMA/POLICY FROM PREFERENCE
```

### 22.3 Allowed implementation discretion

Worker may choose local mechanics only when equivalent and non-observable, e.g.:

- private method names;
- local variable names;
- equivalent data structure behind identical contract/complexity bounds;
- internal refactoring that preserves all state/authority/evidence/performance constraints.

### 22.4 Forbidden discretion

Worker may not choose/change without design authority:

- Application/LSA ownership;
- contract fields/participants/authority;
- state transitions;
- strategy formulas/threshold versions;
- Risk ordering/limits;
- capital transaction semantics;
- broker retry behavior;
- Guardian policy/authority;
- FSARM priority/envelope semantics;
- persistence consistency model;
- replay/simulation classification;
- CSA/Monitor authority;
- security fail-open behavior;
- Foundation substitute.

### 22.5 One WP/slice at a time

Implementation should be authorized in bounded slices with exact scope, dependencies and verifier target. Completion of one slice does not authorize the next.

### 22.6 Preserve closed predecessors

Later implementation changes may not silently mutate accepted predecessor behavior. If a successor requires change, use the governed reopen/amend/review lifecycle.

## 23. Code-Ready Meaning

For this package:

```text
CODE_READY_DESIGN
= material observable semantics are specified and verifiable
!= implementation authorized
!= provider/broker external capability currently certified
!= Foundation future capability available
!= Paper/Live authority
```

A component may be code-ready behind a fail-closed adapter even when an external Foundation FCR is future, as long as the unresolved external contract is not locally invented.

## 24. Activation Blockers Separate From Code Design

Even after SIA design acceptance and later code implementation, these can still block operation:

- FCR-0016 canonical Foundation artifact build consumption;
- FCR-0013 provider egress/credential boundary;
- FCR-0014 broker egress/credential boundary;
- FCR-0011 FSTSimA non-Live isolation for external/research behavior;
- FCR-0008 research egress;
- current provider/broker certification;
- exact promotion thresholds/evidence;
- Owner Paper/Tiny Live/Live authority;
- security/deployment/environment qualification.

Do not convert an activation blocker into local workaround.

## 25. Pre-Semantic-Freeze Checklist

Before file 21 may report SIA semantic freeze ready:

- files 01-20 plus 12A present;
- source/FCR state current;
- accepted 43 contract baseline = 43/43;
- all current/proposed Applications have explicit topology;
- every current LSA specialized;
- types/contracts/state/persistence/runtime/security defined;
- 14 strategies/11 intelligence baselines defined;
- 26 CSA candidate profiles defined;
- APP-RSC delta explicit;
- no known accepted historical requirement silently lost;
- unresolved Foundation-owned matters explicitly fail closed;
- review can identify exact target commit.

## 26. Review Sequence

```text
SEMANTIC FREEZE CANDIDATE
-> 21 Architecture/Consistency Review
-> remediate if needed
-> new semantic freeze if semantic change
-> rerun Architecture/Consistency
-> 22 Fresh Red-Team
-> remediate if needed
-> rerun both on new freeze
-> 23 Owner Review Gate
```

No review file can change the semantic freeze it claims to review without requiring a new review cycle.
