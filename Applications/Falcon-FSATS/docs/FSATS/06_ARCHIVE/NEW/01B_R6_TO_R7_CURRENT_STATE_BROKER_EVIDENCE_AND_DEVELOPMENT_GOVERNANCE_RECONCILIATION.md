# FSATS SIA v0.1 — R6 to R7 Current-State, Broker-Evidence and Development-Governance Reconciliation

**Package:** `FSATS-SIA-v0.1-R7`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Branch:** `application-development`
**Implementation Authority:** `NOT_GRANTED`
**Runtime / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`
**Controls over conflicting earlier SIA wording for R7 only:** `YES`

## 1. Purpose

This reconciliation preserves the complete R6 semantic history while prospectively correcting three subjects for R7:

1. current Foundation/FCR state that advanced after the R6 freeze;
2. broker evidence capability semantics where a broker does not provide a particular acknowledgement/status/evidence item by design versus where expected evidence is missing unexpectedly;
3. Owner-directed classification and bounded pre-delegation of self-development changes, including a lawful 24-hour no-veto mechanism that does not treat Owner silence or timer expiry as a new source of authority.

R6 files and reviews remain immutable historical evidence for the exact R6 freeze they reviewed. This file does not retroactively rewrite R6.

## 2. Governing Authority and Non-Grant

Interpret this reconciliation under the current authority order:

```text
Falcon Vision
> Falcon Constitution
> current explicit Owner decisions
> approved Specifications / Contracts / accepted ADRs
> current Foundation capability / FCR dispositions
> current accepted FSATS semantics
> R7 candidate
> R6/R5/R4/R3/R2/R1/P0/P1/V1.3 history
```

Mandatory invariants remain:

```text
PROTECT > MANAGE > GROW
SELF_AWARENESS != AUTHORITY
INTELLIGENCE != AUTHORITY
RECOMMENDATION != AUTHORIZATION
OWNER_SILENCE != OWNER_APPROVAL
TIMER_EXPIRY != NEW_AUTHORITY
FSA_REVIEW != PRODUCTION_ADOPTION
REQUEST != GRANT
TECHNICAL DELIVERY != BUSINESS SUCCESS
```

This reconciliation is design only. It grants no implementation, runtime, external connectivity, broker, Paper, Tiny Live, Live, deployment, autonomous-promotion or production authority.

## 3. R7 Current-State Refresh

The current Foundation repository evidence read for this R7 reconciliation reports:

```text
FOUNDATION README BLOB = c556bd25061ecd40930013041d6902501fa13955
STAGE 0 THROUGH STAGE 6 = ACCEPTED_AND_CLOSED
STAGE 7 PLANNING_AND_DESIGN = AUTHORIZED
STAGE 7 PLAN v0.3 = OWNER_ACCEPTED
STAGE 7 POST_OWNER_PLAN_ACCEPTANCE_RED_TEAM = PASS
STAGE 7 IMPLEMENTATION = NOT_AUTHORIZED
STAGE 8 THROUGH STAGE 17 = NOT_AUTHORIZED
```

Current relevant live FCR handling remains:

```text
FCR-0008  = ACCEPTED_FOR_PLANNING / Waiting On NONE / Stage12 research-only egress
FCR-0012  = ACCEPTED_FOR_PLANNING / Waiting On NONE / Stage13 FSA governance reconciliation
FCR-0030  = ACCEPTED_FOR_PLANNING / Waiting On NONE / Stage13 MSA-to-FSA exact binding
FCR-0014  = ACCEPTED_FOR_PLANNING / Waiting On NONE / Stage12 broker egress/credential boundary
FCR-0010  = FOUNDATION_IMPLEMENTED / Waiting On APPLICATION / final consuming implementation verification pending
FCR-0031  = FOUNDATION_IMPLEMENTED / Waiting On APPLICATION / final consuming implementation verification pending
```

`Waiting On NONE` does not mean capability available, FCR closed, implementation authorized or runtime authorized.

Where R6/01A contains older Foundation Stage 6 status, this R7 current-state refresh controls only for current-state interpretation. It does not rewrite the historical R6 freeze.

## 4. Broker Evidence Capability Model

### 4.1 Problem corrected

Earlier R6 correctly models ambiguous external execution outcomes through states such as `TRANSPORT_UNKNOWN`, `UNKNOWN_ORDER`, `CONFLICTED` and `RECONCILIATION_REQUIRED`. R7 adds an explicit distinction between:

```text
EXPECTED EVIDENCE MISSING UNEXPECTEDLY
!=
EVIDENCE NOT PROVIDED BY THE CERTIFIED BROKER CAPABILITY BY DESIGN
```

A broker adapter SHALL NOT represent a certified absence-of-capability as a transport failure, and SHALL NOT fabricate unavailable evidence to satisfy Falcon's canonical lifecycle.

### 4.2 BrokerEvidenceCapabilityProfile

Every certified `BrokerProfileVersion` SHALL declare, for each material evidence item required by its admitted order/account capability, an exact `BrokerEvidenceCapabilityProfile` containing at minimum:

```text
EvidenceKind
AvailabilityClass
AcquisitionPath
ExpectedTimingClass
AuthoritativenessClass
ReconciliationMethod
FallbackMethod if governed and valid
RequiredForSafetyOrFinality: bool
MissingExpectedEvidenceDisposition
CertifiedAt
CertificationEvidenceRefs[]
RevalidationTrigger
```

Canonical `AvailabilityClass` values for R7:

```text
DIRECT_RESPONSE
ASYNCHRONOUS_EVENT
QUERY_RECONCILABLE
DERIVABLE_BY_GOVERNED_RULE
NOT_PROVIDED_BY_BROKER
UNKNOWN
```

`DERIVABLE_BY_GOVERNED_RULE` is permitted only where the derivation is explicitly specified, independently testable, provenance-preserving and does not invent broker truth.

### 4.3 Missing-versus-absent semantics

If the certified profile says an evidence item should arrive but it does not arrive within the governed expectation, Falcon SHALL use the appropriate ambiguous/missing-evidence path such as `TRANSPORT_UNKNOWN`, `CONFLICTED` or `RECONCILIATION_REQUIRED`.

If the certified profile says `NOT_PROVIDED_BY_BROKER`, Falcon SHALL NOT wait forever for the item, fabricate it, default it to zero/success, or label the absence as a transport fault. Falcon SHALL instead use the declared alternative reconciliation path if one exists.

If an evidence item is required to establish safe order/account/fill/cancel/replace/fee/settlement truth and no certified direct, asynchronous, query, or governed derivation path can establish that truth, the affected broker capability / order type / route is `INELIGIBLE` for that required Falcon use.

### 4.4 Examples

```text
BROKER HAS NO SEPARATE ACK
+ certified order-status query/event path can prove receipt/state
-> use the certified query/event reconciliation path
-> do not invent ACK

EXPECTED ACK SUPPORTED
+ no ACK received
+ delivery outcome unknown
-> AMBIGUOUS / TRANSPORT_UNKNOWN
-> reconcile before retry

BROKER DOES NOT PROVIDE FINAL FEE IMMEDIATELY
+ certified later final-fee path exists
-> accounting remains explicitly provisional/reconciliation-pending where required
-> final accounting uses reconciled evidence

BROKER DOES NOT PROVIDE REQUIRED SAFETY-CRITICAL TRUTH
+ no governed alternative
-> affected capability INELIGIBLE / FAIL_CLOSED
```

### 4.5 Broker profile and verifier consequences

R7 adds mandatory broker certification/verifier coverage for:

- absence-by-design versus unexpected missing evidence;
- no fabricated ACK/status/fill/fee/settlement values;
- exact per-evidence acquisition path;
- safe finality requirements;
- ambiguous outcome reconciliation before duplicate submission;
- capability ineligibility when required truth cannot be established;
- profile invalidation when broker behavior materially changes from certification.

This R7 section controls over any earlier wording in `04_CANONICAL_DOMAIN_TYPE_CATALOG.md` or `16_MARKET_PROVIDER_BROKER_AND_EXECUTION_PROFILE_SPEC.md` that could otherwise be read as requiring a broker to natively emit every canonical Falcon evidence event.

## 5. Development Change Classification (DCC)

Every material self-development or self-improvement candidate submitted toward production adoption SHALL carry a `DevelopmentChangeClassification` determined from actual semantic impact, not from the candidate's title, claimed purpose, profitability, confidence or origin tier.

R7 defines five classes:

### DCC-1 — BOUNDED_OPTIMIZATION

Improves the same already-authorized responsibility without changing business meaning, authority, permissions, protected architecture, hard gates or admitted scope.

Typical examples:

- faster equivalent algorithm;
- higher-accuracy model/estimator within the same purpose and inputs/outputs;
- better calibration;
- lower resource cost;
- lower false-positive rate without weakening deterministic protection predicates;
- parameter/model refinement that preserves the same governed responsibility and ceilings.

### DCC-2 — BOUNDED_CAPABILITY_EXTENSION

Adds a new capability or candidate behavior entirely within an already-authorized Application/domain boundary and without crossing a Hard Escalation Gate.

Typical examples may include:

- a new Trading strategy under existing market/data/Risk/capital/execution boundaries;
- a new feature/estimator under an existing LSA responsibility;
- a new strategy School only where it is a bounded organizational/intelligence extension that does not change orchestration authority, Risk semantics, capital competition, market authority, permissions, contracts or protected architecture.

DCC-2 is not automatically low risk merely because it is local or profitable.

### DCC-3 — MATERIAL_DOMAIN_CHANGE

Changes material Application business semantics, policy meaning, operating scope or high-consequence domain behavior while remaining below sovereign governance/core-intelligence-control changes.

Examples include:

- changing the definition or acceptance semantics of Risk;
- changing Risk limits/ceilings or capital exposure meaning;
- changing strategy conflict-resolution or capital-competition semantics materially;
- adding a market or materially different broker/execution semantic;
- a new School that changes decision hierarchy, weighting, capital allocation or business policy;
- material contract/state-machine/persistence behavior that changes business truth.

DCC-3 requires explicit Owner/governance approval and is not eligible for timer-based no-veto adoption under the current R7 direction.

### DCC-4 — PROTECTED_INTELLIGENCE_OR_ARCHITECTURE_CHANGE

Changes the machinery that learns, reasons, self-develops, obtains tools/access, or changes protected Application architecture or intelligence-control behavior.

Examples include:

- changing the self-development algorithm itself;
- changing MSA/LSA/CSA reasoning or escalation architecture materially;
- changing Meta-Learning candidate-generation authority/logic materially;
- adding or expanding Internet/tool/write permissions for Awareness;
- changing learned-state/memory governance materially;
- changing CSA/LSA/MSA parentage or protected Application architecture;
- changing Monitor AI policy or target-controlled monitoring boundaries;
- changing candidate promotion logic or protected self-development controls.

DCC-4 requires explicit Owner/governance approval, fresh consequence-appropriate Architecture/Consistency, Security and Red-Team/adversarial review, and no timer-based no-veto adoption.

### DCC-5 — SOVEREIGN_GOVERNANCE_CHANGE

Changes the authority/control system that governs Falcon itself.

Examples include:

- Falcon Vision or Constitution change;
- Owner control or delegation model change;
- FSA authority ceiling/jurisdiction change;
- Guardian/containment/Kill/Factory Reset/release control change;
- audit/monitor independence control change;
- root trust/security/governance authority change;
- the DCC/pre-delegation governance mechanism itself where the change would expand autonomous adoption authority.

DCC-5 is not ordinary self-development. It requires the competent sovereign/governance authority defined by Falcon and is never eligible for timer-based no-veto adoption under this candidate.

## 6. Hard Escalation Gates

Classification SHALL use deterministic hard escalation before any discretionary score or benefit assessment.

At minimum, a candidate SHALL be escalated to the minimum class required by any changed dimension involving:

```text
PURPOSE / RESPONSIBILITY / OWNERSHIP
AUTHORITY / PERMISSION / DELEGATION
RISK DEFINITION / RISK LIMIT / CAPITAL AUTHORITY
MARKET / ACCOUNT / BROKER / EXECUTION BUSINESS SCOPE
CROSS-APPLICATION CONTRACT / ROUTE / OWNERSHIP SEMANTIC
PROTECTED CORE ARCHITECTURE
SELF-DEVELOPMENT / META-LEARNING / PROMOTION MECHANISM
MSA / LSA / CSA PARENTAGE OR JURISDICTION
MONITOR / GUARDIAN / FSA CONTROL BOUNDARY
INTERNET / TOOL / WRITE / SECRET / CREDENTIAL ACCESS
OWNER CONTROL / GOVERNANCE / AUDIT / KILL / RESET / RELEASE
```

A candidate described as `IMPROVE_ACCURACY`, `IMPROVE_SPEED` or `IMPROVE_PERFORMANCE` does not remain DCC-1 if its actual diff crosses a higher gate.

No averaged risk score may reduce a hard-gate classification.

## 7. Classification Governance

The originating CSA/LSA/MSA may propose a class but SHALL NOT be the sole classifier for its own production-bound candidate.

Required behavior:

```text
ORIGIN PROPOSES CLASS
-> DETERMINISTIC HARD-GATE CLASSIFICATION
-> REQUIRED PARENT REVIEW
-> APPLICATION MSA REVIEW
-> MONITOR CHALLENGE / INTEGRITY EVIDENCE AS APPLICABLE
-> FSA OS-GOVERNANCE / COMPATIBILITY REVIEW WHEN AVAILABLE/APPLICABLE
-> OWNER/GOVERNANCE PATH
```

A reviewer may raise the class when evidence shows higher consequence.

No Awareness entity may lower a candidate below the deterministic Hard Escalation Gate result.

Material classification disagreement SHALL use the higher class until governed reconciliation resolves the disagreement.

`UNKNOWN` material classification evidence fails closed for promotion.

## 8. Owner Pre-Delegation Profile

R7 restores the Owner's intended bounded autonomy through explicit prospective delegation rather than silence-created authority.

A pre-delegation is a separate, explicit, revocable Owner/governance authority instrument that SHALL identify at minimum:

```text
PreDelegationId / Version
IssuingOwnerGovernanceIdentity
EffectiveAt
ExpiresAt or explicit no-expiry rule
RevocationState
AllowedApplications / LSAs / CSA scopes
AllowedDCCClasses
AllowedDCC2Subclasses if any
ExcludedChangeDimensions
RequiredValidationProfile
RequiredEvidenceProfile
RequiredFSACompatibilityState when applicable
RequiredMonitorState
RequiredRollbackState
MaximumPromotionStep
NoVetoWindow = 24h when this mechanism is enabled
OwnerDeliveryChannelIdentity
DeliveryProofRequirement
MaterialChangeResetRules
PostPromotionObservationRequirements
```

The pre-delegation SHALL be narrower than the Owner's full authority and may be revoked or reduced at any time through the governed authority path.

## 9. 24-Hour No-Veto Mechanism

The 24-hour rule is defined as a use condition on already-existing explicit Owner pre-delegation.

Mandatory semantic:

```text
OWNER PRE-DELEGATION
+ ELIGIBLE DCC CLASS/SUBCLASS
+ ALL REQUIRED VALIDATION/EVIDENCE PASS
+ EXACT IMMUTABLE OWNER PACKAGE DELIVERED THROUGH GOVERNED CHANNEL
+ 24 HOURS WITHOUT OWNER VETO/HOLD/RECLASSIFICATION
= PRE-EXISTING BOUNDED DELEGATED AUTHORITY MAY BE EXERCISED
```

It SHALL NOT be represented as:

```text
OWNER SILENCE = APPROVAL
TIMER EXPIRY = NEW AUTHORITY
FSA = OWNER SUBSTITUTE
```

### 9.1 Eligibility

Current R7 candidate rule:

```text
DCC-1 = MAY be eligible when covered by exact active pre-delegation
DCC-2 = MAY be eligible only for exact Owner-allowlisted subclasses under active pre-delegation
DCC-3 = NOT eligible
DCC-4 = NOT eligible
DCC-5 = NOT eligible
```

### 9.2 Timer start

The timer SHALL begin only after verifiable successful delivery of the exact immutable decision package to the governed Owner channel.

Queueing, attempted notification, unavailable Owner channel or unverifiable delivery does not start the timer.

### 9.3 Timer invalidation / reset

The pending no-veto path SHALL be canceled or reset as governed when any material event occurs, including:

- candidate bytes/digest change;
- classification escalation;
- material new evidence or contradiction;
- required test/Red-Team/regression changes from PASS to non-PASS;
- Monitor material disagreement;
- integrity signal/investigation hold;
- rollback/corrective path no longer verified;
- authority/pre-delegation expiry, revocation or scope change;
- market/broker/provider/environment change that invalidates material candidate evidence;
- Owner veto, hold or request for explicit review.

A materially changed candidate requires a new immutable package and a new applicable timer window.

## 10. Maximum Promotion Step

Expiry of a valid no-veto window SHALL NOT imply unrestricted or Full-Live promotion.

The exact pre-delegation SHALL define the maximum promotion step permitted for that class/subclass and current lifecycle state.

Examples of governed future steps may include:

```text
ISOLATED_CANDIDATE -> SHADOW
SHADOW -> PAPER
PAPER -> BOUNDED_TINY_LIVE
BOUNDED_TINY_LIVE -> separately governed next step
```

No step is currently runtime-authorized by this R7 design candidate.

A candidate may advance only to the lesser of:

```text
MAXIMUM_PROMOTION_STEP_IN_PREDELEGATION
AND
CURRENTLY_AUTHORIZED_PLATFORM/APPLICATION_LIFECYCLE_STEP
```

## 11. FSA Boundary Under Pre-Delegation

FSA remains Foundation-owned and is not granted production-adoption sovereignty by this mechanism.

Where FSA review is applicable and available, FSA may verify and attest that:

- identity/provenance/evidence are complete;
- protected properties are unchanged as claimed;
- classification and hard-gate evidence are coherent;
- architecture/security/permission/isolation/Foundation compatibility requirements are satisfied;
- the cited pre-delegation exists and appears applicable according to the governed interface.

FSA SHALL NOT create the delegation, widen it, waive its exclusions, start a timer without valid delivery evidence, ignore an Owner veto, or convert an ineligible DCC-3/4/5 candidate into a timer-eligible candidate.

Authority for a no-veto promotion, if later implemented, derives from the explicit Owner pre-delegation instrument plus satisfaction of its conditions, not from FSA review or Owner silence.

The exact Foundation realization remains subject to the future Stage 13 reconciliation tracked by FCR-0012/FCR-0030. Applications SHALL NOT invent that Foundation control plane locally.

## 12. Owner Package Minimum Classification Summary

Every material production-bound development package SHALL present an Owner-readable first-page/first-surface summary containing at minimum:

```text
CandidateId / Version / Digest
OriginEntity / Tier
DCC Class
Hard Escalation Gates Triggered[]
Protected Property Change = NONE | EXACT DIFF
Authority / Permission Change = NONE | EXACT DIFF
Risk / Capital Semantic Change = NONE | EXACT DIFF
Architecture / Contract Change = NONE | EXACT DIFF
Expected Benefit Metrics
Worst Regression / Known Limitations
Validation / FSTSimA / Adversarial / Regression Result
Monitor Disagreement State
Unresolved Unknowns
Rollback / Corrective Path State
PreDelegationId if claimed
24h Eligibility = YES | NO + reason
Maximum Promotion Step if eligible
Owner Requested Action = VETO | HOLD | EXPLICIT_APPROVAL | NO_ACTION_WITHIN_PREDELEGATION_WINDOW as applicable
```

The detailed evidence package remains attached/referenced. The summary does not replace evidence.

## 13. Classification Examples

```text
Existing Strategy algorithm made faster with same outputs/risk/authority
-> DCC-1

Existing estimator retrained for better calibration with same purpose/hard gates
-> DCC-1

New Strategy using existing market/data/Risk/capital/execution boundaries
-> DCC-2 unless a Hard Escalation Gate is crossed

New School used only as bounded classification/intelligence organization
-> DCC-2 unless it changes orchestration/weight/capital/Risk/authority

New School changes capital competition or decision hierarchy
-> DCC-3

Risk definition/limit semantics changed
-> DCC-3 or higher if authority/governance changes

Awareness gains new Internet/tool/write permission or self-development mechanism changes materially
-> DCC-4

Owner/FSA/Guardian/monitor/Kill/governance authority model changes
-> DCC-5
```

## 14. Required R7 Verifier / Red-Team Additions

R7 verification SHALL challenge at minimum:

1. candidate labels itself DCC-1 while changing Risk semantics;
2. candidate labels itself DCC-1 while adding permission/tool/Internet access;
3. origin Awareness lowers a Hard-Gate classification;
4. two reviewers disagree DCC-1 versus DCC-3 and system uses lower class;
5. DCC-3/4/5 enters timer-based no-veto path;
6. DCC-2 uses no-veto without exact subclass pre-delegation;
7. timer starts on send attempt without governed delivery proof;
8. candidate bytes change while old timer continues;
9. Monitor disagreement or integrity hold fails to stop/reset timer;
10. FSA review is represented as source of production authority;
11. 24-hour expiry promotes beyond `MaximumPromotionStep`;
12. expired/revoked pre-delegation still used;
13. broker capability says `NOT_PROVIDED_BY_BROKER` but adapter fabricates evidence;
14. expected broker evidence is missing but system treats it as certified absence-by-design;
15. safety-critical broker truth has no certified acquisition/reconciliation path but route remains eligible;
16. asynchronous/query broker evidence is mistaken for immediate ACK semantics;
17. final fees/settlement truth default to zero because broker evidence is delayed/unavailable;
18. broker behavior changes materially after certification but profile remains eligible without revalidation.

## 15. Supersession / Reconciliation Map

For R7 only, this file controls over conflicting wording in the following earlier candidate subjects:

```text
04_CANONICAL_DOMAIN_TYPE_CATALOG.md
  -> broker evidence capability distinction
  -> DevelopmentChangeClassification vocabulary and pre-delegation-related candidate status semantics

16_MARKET_PROVIDER_BROKER_AND_EXECUTION_PROFILE_SPEC.md
  -> exact broker evidence capability declaration/certification and absent-vs-missing behavior

18_AWARENESS_CSA_MONITOR_AND_SELF_DEVELOPMENT_SPEC.md
  -> DCC classification, Hard Escalation Gates, Owner pre-delegation and 24h no-veto candidate path

01A_R5_TO_R6_CURRENT_STATE_CONDITIONAL_RUNTIME_AND_PROVENANCE_RECONCILIATION.md
  -> current Foundation/FCR snapshot only, where R7 current-state evidence is newer
```

All unaffected R6 semantics remain preserved.

## 16. Current R7 Candidate Authority Markers

```text
BROKER_EVIDENCE_CAPABILITY_MODEL = DESIGN_CANDIDATE
DCC_1_TO_DCC_5 = DESIGN_CANDIDATE
HARD_ESCALATION_GATES = DESIGN_CANDIDATE
OWNER_PREDELEGATION_PROFILE = DESIGN_CANDIDATE
24H_NO_VETO_MECHANISM = DESIGN_CANDIDATE

OWNER_ACCEPTED_R7 = NO
IMPLEMENTATION_AUTHORIZED = NO
RUNTIME_AUTHORIZED = NO
AUTONOMOUS_PROMOTION_AUTHORIZED = NO
STAGE12_RUNTIME_CAPABILITIES_AVAILABLE = NO
STAGE13_RUNTIME_CAPABILITIES_AVAILABLE = NO
```

No implementation worker may materialize these candidate semantics until the required Owner acceptance and separate implementation/runtime authorities exist.