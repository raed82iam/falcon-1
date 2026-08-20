# FSATS Part 1 — CSA Amendment Semantic Freeze V2

**Status:** `FROZEN_FOR_REVIEW / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Semantic Target Commit:** `6d589d337ebc737e4730da4b081035480b9c8d2e`  
**Supersedes For Review:** `02_CSA_SEMANTIC_FREEZE.md` only as the current review target; historical V1 review evidence remains preserved.  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## V2 Delta

V2 preserves the same seven proposed CSA identities and adds explicit resource-accounting/degradation requirements after Red-Team discovery that CSA compute/memory/model/evidence/recovery cost must not exist as hidden ungoverned capacity.

It also retains the earlier hardening for independent evidence, anti-Goodhart protection, research/egress limits, diagnosis-versus-runtime-mutation separation, Safety Continuity and Controlled Revival.

## Frozen Initial CSA Set

```text
CSA-T05-01 -> OpportunityDiscoveryEngine -> T-LSA-05
CSA-T06-01 -> StrategyController -> T-LSA-06
CSA-T12-01 -> StrategyEvolutionEngine -> T-LSA-12
CSA-P05-01 -> AnomalyDetector -> P-LSA-05
CSA-G01-01 -> IncidentClassifier -> G-LSA-01
CSA-S02-01 -> SyntheticMarketGenerator -> S-LSA-02
CSA-S07-01 -> CalibrationEngine -> S-LSA-07
```

```text
TOTAL CSA IF ACCEPTED = 7
MSA = 5 unchanged
LSA = 34 unchanged
APPLICATIONS = 5 unchanged
```

## Resource Invariant

```text
CSA_RESOURCE_COST != ZERO
CSA_SELF_REPORTED_NEED != RESOURCE_GRANT
CSA_IMPORTANCE != AUTOMATIC_PROTECTED_FLOOR
```

CSA cost remains inside owning Application accounting and normal APP-RSC/Foundation resource governance.

## Review Rule

Only this exact V2 target may support the current Architecture/Consistency and Red-Team claims. Any semantic change after this target requires another freeze/review cycle.
