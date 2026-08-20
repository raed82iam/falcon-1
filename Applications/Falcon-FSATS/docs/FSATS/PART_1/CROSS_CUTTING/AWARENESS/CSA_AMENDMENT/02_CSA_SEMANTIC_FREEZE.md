# FSATS Part 1 — CSA Amendment Semantic Freeze

**Status:** `FROZEN_FOR_REVIEW / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Semantic Target Commit:** `09fa310edfa4e18673c87cfa9756cfd878efadba`  
**Candidate:** `01_CSA_ELIGIBILITY_AND_INITIAL_TOPOLOGY_CANDIDATE.md`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## Frozen Meaning

The reviewed candidate defines seven initial CSA identities only:

```text
CSA-T06-01 -> StrategyController -> T-LSA-06
CSA-T05-01 -> OpportunityDiscoveryEngine -> T-LSA-05
CSA-T12-01 -> StrategyEvolutionEngine -> T-LSA-12
CSA-P05-01 -> AnomalyDetector -> P-LSA-05
CSA-G01-01 -> IncidentClassifier -> G-LSA-01
CSA-S02-01 -> SyntheticMarketGenerator -> S-LSA-02
CSA-S07-01 -> CalibrationEngine -> S-LSA-07
```

Count if later accepted:

```text
Trading = 3
FSAPMA = 1
Trading Guardian = 1
FSTSimA = 2
APP-RSC = 0 initially
TOTAL CSA = 7
```

The freeze also includes the candidate's eligibility criteria, explicit non-CSA exclusions, independent-evidence/anti-Goodhart rules, research/egress boundaries, Safety Continuity/repair bindings and the separation between CSA diagnosis/learning and runtime mutation.

## Preserved Current Accepted Baseline

Until explicit Owner final acceptance, the currently accepted Part 0/Part 1 baseline remains controlling and CSA remains optional/eligibility-gated with no newly accepted CSA identities.

Historical Part 1 freeze `d203891d75a8c32cbc589dcbb92ddfc2bfcfe82a` remains preserved as the exact previously accepted Part 1 semantic instant.

## Review Rule

Architecture/Consistency and Red-Team results apply only to this exact semantic target. Any semantic remediation requires a new freeze before Owner final decision.
