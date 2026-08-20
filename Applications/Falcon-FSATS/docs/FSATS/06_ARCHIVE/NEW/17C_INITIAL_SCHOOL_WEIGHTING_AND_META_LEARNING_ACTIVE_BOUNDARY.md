# FSATS SIA — Initial School Weighting and Meta-Learning Active Boundary v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-STRAT-002`
**Owner:** APP-TRD / T-LSA-06 and T-LSA-12

## 1. Purpose

Remove ambiguity about whether an additional dynamic Trading-School multiplier changes live/Paper Strategy EvalScores in the initial implementation.

## 2. Initial Active Runtime Rule

For SIA v1.0:

```text
ACTIVE_SCHOOL_WEIGHT_MULTIPLIER = 1.0000 FOR EVERY ACTIVE SCHOOL
```

Current schools:

```text
CLASSICAL_TRADING = 1.0000
OPPORTUNITY_HUNTING = 1.0000
```

The multiplier is neutral and therefore is **not** applied as a separate score factor in T-LSA-06 or capital competition v1.0.

School identity still matters for:

- taxonomy;
- correlation/diversity analysis;
- strategy-health aggregation;
- experiment reporting;
- candidate Meta-Learning proposals;
- minimum diversity challenge during validation.

It does not create authority or reserved capital by school.

## 3. No Double Counting

Current Strategy EvalScore already includes:

- NetEdge;
- calibrated confidence;
- regime fitness;
- execution quality;
- strategy-specific applicability;
- correlation-cluster controls.

Capital competition also includes explicit diversification/performance/capital-efficiency terms.

Therefore v1 SHALL NOT multiply by a dynamic school weight in addition, because doing so would create an unvalidated second performance prior and can double-count recent performance/regime effects.

## 4. T-LSA-12 Meta-Learner Role

T-LSA-12 may generate **candidate** `SchoolWeightProfile` successors.

Candidate schema:

```text
SchoolWeightProfileId
ParentProfileId
CandidateVersion
SchoolIds[]
CandidateWeights[]
EvidenceWindowDefinition
ObjectiveFunctionId
Training/ResearchEvidenceRefs[]
FSTSimAExperimentRefs[]
OverfittingChallengeRefs[]
ExpectedBenefit
KnownFailureModes
ProtectedPropertyDiff
OriginProposalId
```

Weights are positive exact decimals and normalize to a declared scale, but no candidate is active by existence.

## 5. Candidate Objective Function

Initial Meta-Learning experimentation may optimize a bounded validation objective such as:

```text
NetValidatedUtility =
  NetReturnContribution
  - DrawdownPenalty
  - ExecutionCostPenalty
  - CalibrationErrorPenalty
  - CorrelationConcentrationPenalty
  - Instability/TurnoverPenalty
```

The exact objective weights are part of the candidate experiment profile and do not become active production behavior until separately reviewed.

T-LSA-12 is not permitted to choose objective weights in trusted active runtime without a governed profile.

## 6. Candidate Weight Bounds

For experiments only, initial safe search bounds:

```text
Each school normalized share between 0.25 and 0.75
Total shares = 1.00
```

These bounds prevent an experiment from trivially deleting one entire school while the current architecture intends both schools to remain represented for validation.

The bounds do not imply future Owner acceptance of dynamic school weighting.

## 7. Evaluation Lifecycle

```text
T-LSA-12 candidate proposal
-> FSTSimA isolated validation
-> out-of-sample/adversarial/regime analysis
-> compare against neutral 1.0/1.0 active baseline
-> Trading MSA review
-> FSA compatibility review when the Foundation interface exists
-> explicit Owner/governance decision
-> separate Strategy/Policy update lifecycle
```

No candidate can self-promote.

## 8. Required Evidence For A Successor Profile

A candidate dynamic school-weight profile must prove at minimum:

- exact training/evaluation window separation;
- no look-ahead leakage;
- improvement across more than one regime;
- no hidden increase in hard Risk/Guardian exposure;
- correlation/diversification impact;
- transaction-cost/capital-turnover impact;
- sensitivity to weight perturbations;
- failure under provider/data degradation;
- deterministic replay;
- comparison against neutral baseline and equal-capital opportunity set;
- no strategy excluded solely because its school had a temporary bad period without sufficient sample evidence.

## 9. Active Runtime State

Initial active state representation:

```text
SchoolWeightPolicyId = FSATS-SCHOOL-WEIGHT-NEUTRAL-v1.0
SchoolWeightingMode = NEUTRAL_NO_ADDITIONAL_MULTIPLIER
```

A coding worker SHALL NOT implement a hidden adaptive multiplier, reinforcement loop or EWMA school score under v1.

## 10. Strategy Dormancy vs School Weighting

Strategy states ACTIVE/WATCH/RESTRICTED/DORMANT/EXPERIMENTAL/RETIRED remain strategy-level.

A weak strategy can be restricted/dormant without lowering a school-wide active multiplier. Conversely, a strong school does not revive a hard-ineligible strategy.

## 11. Capital Competition Integration

`07F` CapitalPriorityScore operates on individual proposals and contains no `SchoolWeight` factor in v1.

School correlation/diversity may affect the existing exact diversification/correlation inputs, but there is no extra school quota or guaranteed school capital share.

## 12. Future Profile Change Is Semantic

Any successor that applies non-neutral school weights to active proposal scoring/capital priority changes observable Trading behavior and requires:

```text
new profile/version
fresh validation
Architecture/Consistency review
Red-Team review
Owner acceptance
separate implementation/promotion authority
```

## 13. Verification Families

Verifier SHALL prove:

1. both active school multipliers neutral;
2. no hidden SchoolWeight factor in EvalScore;
3. no SchoolWeight factor in 07F CapitalPriorityScore;
4. candidate weights cannot affect active runtime;
5. candidate search bounds 25..75 each for current two-school experiment;
6. candidate objective/profile identity mandatory;
7. FSTSimA evidence required;
8. neutral baseline comparison required;
9. strategy hard gate cannot be overridden by school candidate;
10. no self-promotion.

## 14. Finding Disposition

```text
AC-STRAT-002 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
ACTIVE_SCHOOL_WEIGHTING_v1 = NEUTRAL / NO EXTRA MULTIPLIER
META_LEARNING = CANDIDATE_ONLY
```
