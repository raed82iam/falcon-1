# P0-C - Application Topology, Self-Awareness, Learning, Research and Evolution

**Status:** `OWNER_DIRECTED_INTEGRATED_REWRITE_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime Authority:** `NOT_GRANTED`

## 1. Purpose

P0-C defines the exact current FSATS Application topology, awareness placement and jurisdiction, learning/research/development behavior, awareness integrity, independent monitoring, self-maintenance versus self-evolution, candidate applicability, cumulative-change assessment, Owner/FSA review and the separation between awareness and operational control.

The current design is stated directly. A programmer must not compose a four-Application P0 baseline with later APP-RSC files to discover current topology.

## 2. Canonical FSATS topology

FSATS is a non-owning, non-runtime trading-system boundary containing exactly five independent Falcon Applications:

```text
1. Falcon Self-Aware Trading Application
   MSA = 1 / LSA = 13 / CSA = 3

2. Falcon Self-Aware Provider Management Application (FSAPMA)
   MSA = 1 / LSA = 6 / CSA = 1

3. Falcon Trading Guardian Application
   MSA = 1 / LSA = 4 / CSA = 1

4. Falcon Self-Aware Trading Simulation Application (FSTSimA)
   MSA = 1 / LSA = 8 / CSA = 2

5. Falcon Self-Aware Resource Management Application (APP-RSC)
   MSA = 1 / LSA = 3 / CSA = 0 initially
```

Totals:

```text
APPLICATIONS = 5
MSA = 5
LSA = 34
CSA = 7
```

FSATS itself has no MSA, runtime principal, shared credentials, hidden resource pool, hidden mutable business state, lifecycle authority, Foundation authority or undeclared cross-Application authority.

Shared Falcon-wide Web/Communication Applications remain separate Applications outside the FSATS ownership boundary and interact only through explicit governed contracts.

## 3. Canonical awareness hierarchy

```text
PROJECT OWNER / VALID GOVERNANCE
        |
        v
FSA - FOUNDATION SELF-AWARENESS
        ^
        | MSA-backed Application proposal/evidence
        |
APPLICATION MSA
        ^
        |
        +-- LSA-origin proposal
        |
        +-- CSA-origin proposal after parent-LSA review
```

Jurisdiction:

```text
CSA = one eligible intelligent component / specialization
LSA = one qualified major Application branch
MSA = one complete Application
FSA = Foundation / OS self-awareness and OS-governance/conformance review
OWNER = governance authority source
```

```text
AWARENESS_RANK != AUTHORITY
MORE_AWARENESS != MORE_PERMISSION
SELF_AWARENESS != SELF_GOVERNANCE
```

## 4. Definition of self-awareness

Self-awareness is the governed capability to maintain evidence-based understanding of its own condition, purpose, responsibility, capabilities, limitations, dependencies, performance, failures, weaknesses, uncertainty, confidence, competence boundaries, authority limits, blind spots, current accepted baseline, active experiments/candidates, historical outcomes, lessons, improvement opportunities and readiness/fitness inside jurisdiction.

```text
SELF_AWARENESS
= OBSERVE + UNDERSTAND + LEARN + CHALLENGE + RESEARCH + DEVELOP_CANDIDATE + EVALUATE + PROPOSE

SELF_AWARENESS
!= UNBOUNDED_RUNTIME_CONTROL
```

## 5. Canonical self-learning/self-development loop

Within exact jurisdiction and separately granted authorities, an eligible awareness entity may:

```text
1. OBSERVE
2. MEASURE
3. UNDERSTAND
4. IDENTIFY WEAKNESS / GAP / OPPORTUNITY
5. LEARN FROM ATTRIBUTABLE INTERNAL HISTORY
6. RESEARCH EXTERNALLY ONLY_WHERE_AUTHORIZED
7. FORM HYPOTHESIS
8. DEFINE CANDIDATE + APPLICABILITY SCOPE
9. BUILD ISOLATED NONAUTHORITATIVE CANDIDATE
10. TEST / SIMULATE / CHALLENGE
11. MEASURE UNCERTAINTY / LIMITATIONS / FAILURE MODES
12. COMPARE WITH CURRENT ACCEPTED BASELINE
13. REJECT / RETEST / HOLD / RECOMMEND
14. USE ORIGIN-CORRECT REVIEW CHAIN
15. OBTAIN REQUIRED OWNER / GOVERNANCE AUTHORITY
16. USE SEPARATE UPDATE / MANIFEST / ADMISSION / DEPLOYMENT LIFECYCLE
17. OBSERVE POST-ADOPTION OUTCOME
18. CONFIRM / RESTRICT / ROLLBACK / RECOVER / LEARN AGAIN
```

No step implies the next.

```text
RESEARCH_COMPLETE != CANDIDATE_APPROVED
CANDIDATE_BUILT != TESTED
TESTED != RECOMMENDED
RECOMMENDED != GOVERNANCE_APPROVED
GOVERNANCE_APPROVED != DEPLOYED
DEPLOYED != UNRESTRICTED
SUCCESSFUL_OUTCOME != AUTHORITY_EXPANSION
```

## 6. CSA - Component Self-Awareness

### 6.1 Eligibility

CSA is optional and justified only where specialized intelligence, component-level learning/research, self-evaluation, owned model/algorithm/tool improvement, isolated candidate testing or meaningful adaptation creates material value.

Ordinary deterministic infrastructure does not receive CSA merely because telemetry exists.

### 6.2 CSA may

- monitor component quality/performance;
- identify recurring specialized weaknesses;
- learn from local attributable history;
- research specialized methods where authorized;
- compare algorithms/models/techniques;
- create isolated component-owned candidate code/model/config/data-feature artifacts under valid development authority;
- design component-specific tests;
- run authorized sandbox/simulation experiments;
- reject its candidate;
- recommend to parent LSA.

### 6.3 CSA may not

- redefine responsibility;
- modify another owner's assets;
- change branch architecture by declaration;
- bypass parent LSA for a CSA-origin production-bound candidate;
- bypass MSA/FSA/Owner governance;
- deploy/promote itself;
- expand permissions/routes/resources/Risk/cross-Application access;
- treat repeated success as authority expansion.

### 6.4 CSA origin route

```text
CSA SPECIALIZATION REVIEW
-> PARENT LSA BRANCH REVIEW
-> APPLICATION MSA FINAL APPLICATION REVIEW
-> FSA OS/GOVERNANCE REVIEW WHERE_REQUIRED_AND_AVAILABLE
-> OWNER / VALID SEPARATELY AUTHORIZED GOVERNANCE
-> APP-001 / MANIFEST / ADMISSION / DEPLOYMENT LIFECYCLE
```

## 7. LSA - Local Self-Awareness

Every qualified major Application branch owns exactly one LSA. Folder/service/team/controller implementation convenience does not qualify automatically.

A qualified branch has enduring cohesive responsibility with meaningful awareness, evidence, failure/recovery and ownership semantics.

LSA understands branch purpose/state, components, dependencies, performance, limitations, failures, interactions, eligible CSAs, uncertainty and improvement opportunities. It may aggregate CSA evidence, identify cross-component weaknesses, learn, research where authorized, propose branch architecture/components, coordinate branch experiments and assess cumulative branch effects.

LSA cannot represent the complete Application, override siblings, own another Application, create Foundation authority, bypass MSA/FSA/Owner governance, self-approve production or create fake lower tiers merely to satisfy a diagram.

LSA-origin route:

```text
LSA BRANCH REVIEW
-> APPLICATION MSA FINAL APPLICATION REVIEW
-> FSA REVIEW WHERE_REQUIRED_AND_AVAILABLE
-> OWNER / VALID GOVERNANCE
-> GOVERNED UPDATE / DEPLOYMENT LIFECYCLE
```

## 8. MSA - Main Self-Awareness

Every Application has exactly one MSA. There is no FSATS-container MSA.

MSA owns complete Application self-awareness and the final Application-side evaluation/recommendation. It understands complete Application purpose/state, dependencies, all LSAs/CSAs, performance/failure, uncertainty, cross-branch interactions, cumulative changes, readiness, business consequences and whole-Application improvement opportunities.

MSA may identify cross-branch weakness, initiate an MSA-origin candidate, request LSA/CSA evidence, coordinate multi-branch experiments, research Application-level methods where authorized, assess whole-Application benefit/risk, reject locally successful but globally harmful changes and provide final Application recommendation.

```text
MSA = FINAL_APPLICATION_EVALUATOR / RECOMMENDER
```

MSA does not become Falcon-wide authority, own another Application, bypass independent controls, mint Foundation permissions/resources/routes, convert recommendation into deployment authority or become a master runtime controller.

MSA-origin route:

```text
MSA APPLICATION REVIEW
-> FSA REVIEW WHERE_REQUIRED_AND_AVAILABLE
-> OWNER / VALID GOVERNANCE
-> GOVERNED UPDATE / DEPLOYMENT LIFECYCLE
```

## 9. FSA - Foundation Self-Awareness

FSA belongs to Foundation/OS only. Applications do not design/implement its internal runtime architecture, security principal, storage, kill/isolation primitives, baseline store, recovery implementation or Owner control plane.

FSA reviews Foundation/OS/governance/conformance matters including Vision/Constitution, authority/delegation validity, APP-001/CON-023 compatibility, isolation, cross-Application effects, independent protection preservation, Foundation resource/compatibility consequences and evidence/provenance completeness.

FSA may hold/reject/restrict/escalate a proposal when higher-order constraints fail. It does not independently tune Trading Risk, select strategies/markets/instruments/positions/orders/brokers/providers, own portfolio allocation, own Trading execution or replace CSA/LSA/MSA business evaluation.

```text
FSA_GOVERNANCE_REVIEW != TRADING_BUSINESS_DECISION
FSA_REVIEW != IMPLEMENTATION_APPROVAL
FSA_REVIEW != DEPLOYMENT_APPROVAL
FSA_REVIEW != PRODUCTION_ADOPTION
OWNER_SILENCE != AUTHORITY
TIMER_EXPIRY != AUTHORITY
```

FSA governance/control-plane implementation remains Foundation-owned through live FCR-0012, and MSA-to-FSA runtime binding remains Foundation-owned through FCR-0030. Both are future Stage 13 dependencies at this point.

Foundation-origin development follows Foundation governance directly. No fake MSA/LSA/CSA is inserted underneath Foundation work.

## 10. Awareness versus operational controllers

Awareness evaluates; operational controllers perform accepted runtime responsibility.

Current examples:

```text
T-LSA-13 RESOURCE AWARENESS != APP-RSC OPERATIONAL RESOURCE COORDINATION
APP-RSC MSA != RESOURCE_STRATEGY_CONTROLLER
RISK LSA AWARENESS != UNIFIED RISK RUNTIME DECISION OWNER
EXECUTION LSA AWARENESS != EXECUTION / RECONCILIATION RUNTIME OWNER
FSAPMA AWARENESS != PROVIDER ROUTER / DATA CONTROLLERS
MSA != MASTER_RUNTIME_CONTROLLER
FSA != TRADING_CONTROLLER
```

Knowledge never creates command authority.

## 11. Application Monitor AI safety model

Accepted awareness hardening requires bounded independent Monitor AI perspectives for covered Application MSA integrity. Monitors are oversight tools, not Awareness tiers and not business authorities.

A monitor may observe, challenge, correlate evidence and raise attributable integrity signals. It may not:

- own target goals/authority/permissions;
- mutate target architecture;
- become target MSA/LSA/CSA;
- self-authorize kill/isolation/release;
- decide production adoption;
- erase audit evidence;
- autonomously self-develop under the current Owner direction.

Material disagreement between independent monitor perspectives is not averaged or majority-voted into SAFE. It remains an attributable disagreement event and triggers minimum integrity review.

The historical accepted Awareness Amendment defined two independent monitor perspectives for each then-current Application MSA. The later APP-RSC adoption did not, by itself, create an undocumented APP-RSC monitor implementation/identity. Therefore:

```text
MONITOR_REQUIREMENT_FOR_PREVIOUSLY_COVERED_APPLICATIONS = PRESERVED
APP_RSC_MONITOR_IDENTITY / RUNTIME_BINDING = MUST_BE_EXPLICITLY_RESOLVED_BEFORE_CLAIMED
NO_INFERRED_MONITOR_COUNT_EXPANSION = REQUIRED
```

This prevents both semantic loss and invented authority/topology.

## 12. Minimum awareness integrity check

For every material MSA/LSA/CSA error, unexpected behavior or jurisdiction violation, require at minimum a bounded integrity check of:

```text
GOALS / PURPOSE
AUTHORITY / PERMISSIONS
CORE ARCHITECTURE / OWNERSHIP BOUNDARY
```

If these match, normal governed error handling may continue. Material mismatch/unexplained change means integrity is not proven and the affected capability is held/escalated.

An ordinary error alone does not prove compromise or justify unbounded Kill.

## 13. Jurisdiction violation

Attempts by Awareness to self-expand responsibility, claim another branch/Application, change goals, expand permissions, bypass parent review, disable protected audit/monitor/containment controls or alter protected architecture are Awareness-integrity signals.

Affected action fails closed and produces attributable evidence/escalation.

## 14. Self-development is optimization, not self-redefinition

Current Owner direction limits autonomous Application self-development to improving the same authorized responsibility, especially performance, speed and accuracy, or equivalent improvement separately authorized.

Awareness cannot autonomously:

- change goals/purpose;
- change responsibility/ownership;
- expand authority/permissions;
- change protected core architecture;
- weaken monitoring/audit/containment/security;
- modify another owner's assets;
- deploy/promote its own candidate.

A better method inside the same responsibility may be a candidate. Self-redefinition is not.

## 15. No recursive monitor hierarchy

Monitor AI does not create infinite monitor-of-monitor recursion. Monitoring remains bounded through externally governed identity/policy, least privilege, protected baseline/integrity checks, audit and replaceability. Generic enforcement mechanisms remain Foundation-owned where applicable.

## 16. Signal, investigation and enforcement separation

```text
SIGNAL / DETECTION
!= DETERMINISTIC GOVERNING TRIGGER
!= INVESTIGATION
!= ENFORCEMENT
!= RELEASE / TRUST RESTORATION
```

Application Awareness/Monitor AI may raise signals. They do not invent generic investigation/enforcement/release authority. Foundation-owned mechanisms are used where the action crosses OS/lifecycle/security authority boundaries.

## 17. Trusted baseline and recoverability

Each Application design/manifest must be capable of identifying material needed for trusted Awareness identity and recoverable Application-owned artifacts, distinguishing:

```text
LAST_TRUSTED_BASELINE != FACTORY_TRUSTED_BASELINE
```

Relevant material may include code/artifact/model identity, configuration, dependencies, purpose, responsibility, authority/permissions, core architecture identity, manifest/contracts, policy identities, expected hashes/provenance and mutable-state checkpoints where valid.

```text
NO_TRUSTED_RECOVERY_MATERIAL => NO_HONEST_VERIFIED_ROLLBACK_OR_FACTORY_RESET_CLAIM
```

Generic protected persistence, immutable audit and authoritative isolation/release remain Foundation-owned where applicable.

## 18. Static and behavioral integrity

```text
STATIC / IDENTITY INTEGRITY
+ BEHAVIORAL / MUTABLE-STATE INTEGRITY
```

Hash equality alone does not prove behavioral trust. Review may include learned/adaptive state, decisions, proposal history, research inputs, authority-use behavior, audit evidence and unexplained drift.

## 19. Kill, rollback, factory reset and controlled revival

```text
KILL = STOP + ISOLATE + REMOVE_CURRENT_OPERATIONAL_TRUST
ROLLBACK = RESTORE_PRIOR_VERIFIED_TRUSTED_STATE
FACTORY_RESET = RESTORE_DESIGNATED_CLEAN_FACTORY_TRUSTED_BASELINE
CONTROLLED_REVIVAL = GOVERNED_RETURN_AFTER_REMEDIATION_AND_REVALIDATION
```

Restart alone never restores trust after kill/integrity hold. Immutable incident/audit history survives recovery/reset according to governing evidence rules.

Applications define business consequences, but generic authoritative isolation/revocation/release remains Foundation-owned when it crosses OS/security/lifecycle boundaries.

## 20. Investigation cooperation

Application Awareness must cooperate with legitimate authorized integrity investigation. Evidence destruction, monitor/audit disabling, state alteration to evade investigation, isolation evasion or unauthorized authority restoration is itself a Critical integrity signal. Mere disagreement is not refusal.

## 21. Research Internet boundary

Trading MSA has no direct Internet access under current Owner-directed design.

Trading-domain external research uses the bounded FSTSimA research/sandbox path where separately authorized and only when required Foundation research egress/non-Live isolation capabilities exist.

Conceptual path:

```text
TRADING AWARENESS RESEARCH NEED
-> FSTSimA SPECIALIZED RESEARCH
-> PROVENANCE
-> QUARANTINE / SANDBOX
-> SECURITY / INTEGRITY INSPECTION
-> SIMULATION / TEST / ADVERSARIAL ASSESSMENT
-> EVIDENCE
-> TRADING MSA EVALUATION
```

```text
INTERNET -> DOWNLOAD -> TRUSTED_TRADING_RUNTIME = PROHIBITED
RESEARCH_EGRESS != FSAPMA_OPERATIONAL_PROVIDER_DATA
RESEARCH_RESULT != LIVE_OPERATIONAL_TRUTH
```

Runtime research-only egress remains unavailable until Foundation implements/verifies FCR-0008 Stage 12. FSTSimA non-Live egress/isolation remains tied to FCR-0011 Stage 12.

## 22. Change classes

### Level 0 - Research/Learning
No operational authority.

### Level 1 - Governed Self-Maintenance
Restores/preserves already accepted behavior through an accepted/delegated repair path. No semantic evolution.

### Level 2 - Bounded Improvement
Model/algorithm/strategy/internal optimization inside existing responsibility/authority/protection envelopes.

### Level 3 - Controlled Evolution
Material bounded change such as major model behavior or qualified branch creation/merge/retirement inside existing Application authority.

### Level 4 - Protected Authority/Boundary Change
Guardian weakening, Foundation ownership change, cross-Application authority expansion, security-authority change, Owner-defined hard ceiling expansion or autonomous-approval-rule expansion. Explicit Owner approval required.

### Level 5 - Vision/Constitution
Never autonomous. Only formal constitutional governance may change it.

## 23. Self-maintenance versus self-evolution

```text
SELF_MAINTENANCE = RESTORE_OR_PRESERVE_PREVIOUSLY_ACCEPTED_BEHAVIOR
SELF_EVOLUTION = CHANGE_GOVERNED_BEHAVIOR / RESPONSIBILITY / AUTHORITY / ARCHITECTURE / RISK_SEMANTICS
```

If repair requires semantic change, maintenance stops and a governed evolution candidate is created.

## 24. Candidate Applicability Contract

Every material candidate declares before final validation:

- exact identity/version/digest;
- intended market/asset/instrument class where applicable;
- liquidity/volatility/regime boundaries;
- horizon/session;
- required Data Products/quality;
- required execution conditions;
- broker-account/environment boundaries;
- permitted Risk envelope;
- prohibited conditions;
- unknown/unvalidated conditions;
- evidence/confidence limits.

```text
VALIDATED_SCOPE = MAXIMUM_AUTONOMOUS_OPERATING_SCOPE
SUCCESS_IN_SCOPE_A != AUTHORITY_IN_SCOPE_B
```

Scope expansion requires a new/revised governed candidate and renewed validation.

## 25. Testing and digital-twin evidence

Material candidates are challenged proportionately across relevant calm/fast, bull/bear/sideways, high/low volatility/liquidity, gaps/shocks, correlation stress, provider degradation, stale data, execution abnormality, repeated loss/drawdown, adversarial/black-swan and interaction scenarios.

Evidence sufficiency combines scenario coverage, declared-scope coverage, sample sufficiency, time/repetition, stress/failure, interaction coverage, statistical confidence where applicable and consequence severity.

Originators cannot constrain higher-level challenge to friendly scenarios.

## 26. Cumulative change assessment

```text
NEW_CANDIDATE
+ CURRENT_ACCEPTED_STATE
+ ACTIVE_AUTHORIZED_CHANGES
+ CHANGES_UNDER_OBSERVATION
+ KNOWN_DEPENDENCIES
= CUMULATIVE_ASSESSMENT_INPUT
```

CSA assesses component-local effects, LSA branch effects, MSA Application-wide effects and FSA OS/Foundation/authority/isolation/cross-boundary effects. Insufficient interaction evidence leads to HOLD/QUEUE/RETEST.

## 27. Owner review package

A production-bound candidate reaches Owner review only after required lower reviews/tests pass. The package binds exact candidate identity/digest, applicability, origin/review chain, required PASS evidence, risk/change classification, delegation eligibility, cumulative assessment, previous trusted state, rollback/recovery plan, progressive promotion, monitoring/stop conditions, FSA review where required and authoritative delivery/order evidence.

Email/Telegram/push are alerts, not authoritative governance records.

## 28. Owner no-response rule

```text
OWNER_NO_RESPONSE != OWNER_APPROVAL
TIMER_EXPIRY != OWNER_APPROVAL
```

Without valid pre-existing delegation, candidate remains HOLD/PENDING_OWNER_DECISION and promotion is denied.

A valid pre-existing delegation may define bounded conditions under which a candidate becomes eligible for final revalidation, but authority comes from the delegation, not silence. Before any such promotion, exact candidate, scope, delegation validity, reviews, blockers, dependencies, MSA assessment, rollback path, safety ceilings and Owner freeze/reject/restriction must be revalidated.

Material change requires a new package/window.

## 29. Explicit Owner-only classes

No no-response mechanism may approve Vision/Constitution changes, self-authorization expansion, autonomous approval-rule expansion, Guardian weakening, Foundation ownership change, cross-Application authority expansion, security-authority weakening/expansion, Owner-defined hard Risk/evolution ceiling expansion or any class reserved to Owner/higher governance.

## 30. APP-001 / Manifest / lifecycle gate

Self-development never bypasses Application lifecycle. Changes to executable code/model/config/version/dependencies/permission/resource/persistence/communication/security/major branch/awareness identity/Guardian interface or other governed property follow applicable APP-001/CON-023 update/admission/activation.

```text
FSA_PASS != MANIFEST_MUTATED
OWNER_ACCEPTANCE != APP001_ACTIVATION
TOPOLOGY != PERMISSION
```

## 31. New LSA creation/merge/retirement

A new LSA requires genuine qualified major-branch need, distinct enduring responsibility, meaningful awareness, evidence/contract boundary, independent failure meaning, cohesive ownership, no Foundation duplication, no sibling co-ownership, split/merge challenge and removal/rollback direction.

```text
NEW_LSA != NEW_AUTHORITY
NEW_LSA != NEW_PERMISSION
NEW_LSA != CROSS_APPLICATION_ACCESS
NEW_LSA != GUARDIAN_BYPASS
```

## 32. Progressive promotion and financial irreversibility

Software rollback cannot erase completed financial effects. Material changes affecting capital/Risk/orders/positions/external side effects require bounded progressive promotion where separately authorized, with canary scope, max experiment exposure/loss, observation criteria, expansion gates, automatic stop/restrict/rollback triggers and Owner escalation.

Successful canary results cannot expand Owner-defined ceilings by themselves.

## 33. Owner direct governance through FSA

Where future Foundation capability exists:

```text
OWNER = AUTHORITY_SOURCE
FSA = GOVERNANCE_RECIPIENT / ASSESSOR / COORDINATOR
WEB / UI = PRESENTATION / INTERACTION ONLY
```

Owner may approve/reject/request evidence/change/suspend/restrict/freeze autonomous promotion/evolution/request rollback assessment/direct recovery or revoke/narrow delegated envelopes. FSA never becomes the source of Owner authority.

Full runtime control-plane remains blocked pending FCR-0012/FCR-0030.

## 34. Owner rollback/recovery

Owner rollback/recovery direction remains authoritative inside valid governance scope. CSA/LSA/MSA assess Application-domain consequence, MSA gives Application recommendation, FSA assesses Foundation/constitutional/cross-boundary consequence. If literal rollback is unsafe/impossible, Falcon must say so and propose a safer governed recovery path rather than pretend rollback is harmless.

## 35. Owner absence journal

A future governed control plane should preserve attributable records of autonomous promotions, self-maintenance, material evolution/Risk changes, LSA topology changes, rollback/recovery, rejected candidates, active experiments, review windows, candidates under observation and anomalies/open concerns. Success never justifies hiding a material change.

## 36. Failure behavior of Awareness

Awareness failure never implies healthy state:

- CSA failure cannot mint alternate authority;
- LSA failure prevents claims needing branch evaluation;
- MSA failure prevents claims requiring final Application assessment;
- FSA failure prevents claims requiring Foundation self-awareness/governance fitness;
- no sibling inherits another awareness role by convenience.

Operational controllers may continue only under separately valid degraded-mode rules.

## 37. APP-RSC placement and evolution

APP-RSC is a full fifth Application with one MSA and three LSAs. Its MSA/LSAs follow the same P0-C awareness rules. APP-RSC Awareness does not become Foundation Resource Governance or an FSATS master controller. Operational `ResourceStrategyController` remains distinct from APP-RSC MSA.

APP-RSC currently has zero CSA initially. Any future CSA requires explicit eligible component, review and Owner acceptance. No CSA is inferred from controller intelligence.

## 38. Foundation/FCR dependencies

Material current dependencies include:

- FCR-0008 research-only Internet egress, Foundation Stage 12;
- FCR-0011 FSTSimA non-Live isolation/egress, Foundation Stage 12;
- FCR-0012 FSA governance/control plane, Foundation Stage 13;
- FCR-0030 MSA-to-FSA interface/transport, Foundation Stage 13;
- APP-001/CON-023 lifecycle/manifest/update/admission;
- ADR-I015 Application/awareness alignment.

P0-C acceptance would not implement any missing Foundation capability.

## 39. Prime invariants

```text
FSATS_IS_APPLICATION = NO
FSATS_HAS_MSA = NO
APPLICATION_COUNT = 5
MSA_COUNT = 5
LSA_COUNT = 34
CSA_COUNT = 7
APP_RSC_IS_FALCON_APPLICATION = YES
APP_RSC_SCOPE = FSATS_ONLY
APP_RSC_IS_FOUNDATION_RESOURCE_GOVERNANCE = NO
AWARENESS != AUTHORITY
LEARNING != AUTHORITY
RESEARCH != OPERATIONAL_DATA
DEVELOPMENT != PRODUCTION_ADOPTION
TEST_PASS != DEPLOYMENT_AUTHORITY
OWNER_SILENCE != OWNER_APPROVAL
TOPOLOGY != PERMISSION
TECHNICAL_ABILITY != JURISDICTION
SELF_DEVELOPMENT != SELF_REDEFINITION
DIRECT_TRADING_MSA_INTERNET = PROHIBITED
CSA_ORIGIN -> LSA -> MSA -> FSA
LSA_ORIGIN -> MSA -> FSA
MSA_ORIGIN -> FSA
NO_ARTIFICIAL_LOWER_TIER = REQUIRED
```

## 40. Forbidden interpretations

Invalid: FSA is boss of Trading intelligence; MSA is master runtime controller; LSA commands siblings; CSA deploys itself after tests; research Internet substitutes for operational data; 24 hours means Owner approval; new LSA creates permission/resource/route; success expands authority; monitor vote creates SAFE; APP-RSC MSA is Foundation resource authority; APP-RSC automatically gets undocumented monitors/CSAs because it is fifth Application.

## 41. Mandatory scenarios

Challenge at minimum CSA local success but branch harm; LSA candidate harming sibling branch; MSA candidate creating Foundation dependency; FSA detecting constitutional conflict; research result trying to enter Live operational path; candidate outside validated market/regime; no Owner response; stale delegation; cumulative incompatible candidates; Awareness jurisdiction expansion; monitor disagreement; Awareness identity hash matches but behavioral state drift; investigation interference; restart after kill; APP-RSC MSA attempting Foundation grant; proposed APP-RSC CSA without eligibility/acceptance; and Foundation FSA runtime dependency unavailable.

## 42. Exit gates

```text
FIVE_APPLICATION_TOPOLOGY = PASS
AWARENESS_JURISDICTION_COLLISIONS = 0
ORIGIN_CORRECT_REVIEW_ROUTES = PASS
AWARENESS_OPERATIONAL_CONTROLLER_CONFLATION = 0
OWNER_SILENCE_AUTHORITY_PATHS = 0
SELF_REDEFINITION_PATHS = 0
MONITOR_AUTHORITY_EXPANSION = 0
TRUSTED_BASELINE_RECOVERY_MODEL = EXPLICIT
RESEARCH_OPERATIONAL_CONTAMINATION = 0
APP_RSC_AWARENESS_FOUNDATION_CONFLATION = 0
FCR0008_FCR0011_FCR0012_FCR0030_RUNTIME_STATE = EXPLICIT_AND_FAIL_CLOSED
```

## 43. Non-grant

Acceptance of P0-C would establish awareness/evolution design only. It would not authorize external research egress, FSA runtime control plane, autonomous promotion, provider/broker connectivity, Paper, Shadow, Tiny-Live, Live or deployment.