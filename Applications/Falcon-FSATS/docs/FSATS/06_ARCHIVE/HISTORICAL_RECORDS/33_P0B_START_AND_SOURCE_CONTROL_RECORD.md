# FSATS V1.4 Part 0 / P0-B — Start and Source-Control Record

**Status:** `DESIGN_REVIEW_IN_PROGRESS`  
**Scope:** `Part 0 / P0-B only`  
**Branch:** `application-development`  
**Owner authorization:** explicit instruction to begin P0-B  
**P0-A:** `OWNER_ACCEPTED_AND_CLOSED`  
**P0-B final Owner acceptance:** `NOT_GRANTED`  
**P0-C through P0-L:** `NOT_STARTED`  
**Part 1:** `FROZEN_PENDING_PART0_REMEDIATION`  
**Part 2 through Part 10:** `NOT_AUTHORIZED`

## 1. Objective

P0-B reviews the complete FSATS V1.3 historical design package as a controlled knowledge source and creates a complete Review / Difference / Disposition Ledger so that no material historical concept is lost by omission and no historical solution is treated as binding merely because it existed in V1.3.

P0-B does not finalize the V1.4 topology, Application ownership, contracts, Guardian design, FSAPMA design, Trading design, simulator design, runtime behavior, or implementation. Those subjects are dispositioned here only far enough to preserve traceability and assign the correct downstream review home.

## 2. Governing inputs

P0-B is governed by:

- `20_PART0_REOPEN_AND_REMEDIATION_AUTHORITY_RECORD.md`;
- `21_PART0_REMEDIATION_WORK_PACKAGE_PLAN.md`;
- `22_PART0_GITHUB_CANONICAL_REVIEW_SOURCE_POLICY.md`;
- `24_P0A_CANONICAL_AUTHORITY_SOURCE_AND_BASELINE_REGISTER.md`;
- `31_P0A_FRESH_POST_CHANGE_ARCHITECTURE_AND_RED_TEAM_REPORT.md`;
- `32_P0A_FINAL_OWNER_ACCEPTANCE_AND_CLOSURE_RECORD.md`;
- the complete V1.3 inventory in `23` + `23A` + `23B` + `23C`;
- current `reference/fsats-v1.3-scratch` content;
- current applicable Falcon/Foundation governing artifacts and separately current realization evidence;
- explicit Owner directions and corrections.

## 3. Controlled V1.3 identity

Canonical package SHA-256:

`d7fbde9fa0a584d9bb77f388016c9deb5fecdc30b0b9c3c0c7087743b32ac223`

Inventory:

- ZIP entries: `289`
- files: `273`
- directories: `16`

Original inventory reference HEAD:

`9b2046eb7539ad40c3733a1423fe374fa872fe23`

Fresh P0-B comparison found the current reference branch two commits ahead of that historical observation, with the visible compare delta limited to validation-report records. The controlled package identity and 273 package-relative design inventory remain the P0-B historical completeness anchor.

## 4. Evidence-state discipline

For every inventory item P0-B SHALL distinguish:

- `PACKAGE_PATH_INVENTORIED`
- `GITHUB_REFERENCE_PATH_MAPPED`
- `CONTENT_REVIEWED`
- `MATERIAL_CONCEPT_EXTRACTED`
- `CURRENT_CONSTRAINT_CHECKED`
- `ALTERNATIVES_ASSESSED`
- `V1_4_DISPOSITIONED`
- `DOWNSTREAM_HOME_MAPPED`

No later state may be inferred from an earlier one.

In particular, `PACKAGE_PATH_INVENTORIED` does not mean `CONTENT_REVIEWED`, and `CONTENT_REVIEWED` does not mean `V1_4_DISPOSITIONED`.

## 5. Disposition vocabulary

Every material V1.3 concept SHALL receive exactly one disposition:

- `RETAINED`
- `IMPROVED`
- `MODIFIED_FOR_CURRENT_ARCHITECTURE_ALIGNMENT`
- `REPLACED_BY_BETTER_DESIGN`
- `REMOVED_WITH_JUSTIFICATION`
- `OWNER_DIRECTION`
- `OWNER_DECISION_REQUIRED`

Silence is not a disposition.

## 6. Material-difference record

Where V1.4 treatment differs materially from V1.3, P0-B SHALL record:

```text
V1.3 source
→ V1.3 approach / problem addressed
→ proposed V1.4 treatment
→ difference
→ reason
→ Vision / Constitution assessment
→ current Falcon / Foundation assessment
→ expected benefit
→ material trade-offs
→ downstream review Part
```

## 7. Review groups

The 273 paths SHALL be reviewed through these controlled groups while retaining per-path traceability:

1. Package control and FSATS identity.
2. Trading Guardian Application.
3. FSAPMA.
4. Trading Application.
5. Integration contracts.
6. FSATS governance and historical architecture controls.
7. Experimentation / FSTSimA / validation environment.
8. Machine-readable registries, schemas and examples.
9. Implementation / verification / red-team historical evidence.
10. Shared Communication Application.
11. Shared Web Application.
12. Future Shared Applications and package-level controls.

Grouping is a review mechanism only. It does not allow multiple material concepts to receive one vague disposition.

## 8. Mandatory coverage lock

P0-B SHALL explicitly cover at minimum:

- application topology;
- Guardian architecture;
- FSAPMA architecture;
- provider pool and role separation;
- Provider Controller and provider selection;
- operational-data gateway rule;
- provider quality/reconciliation/capacity/free-first rules;
- broker/provider separation;
- markets and market profiles;
- account/broker truth;
- horizons and immutable intent;
- frameworks, schools and strategies;
- risk/capital/decision architecture;
- Fast Track and latency protections;
- execution/positions/reconciliation;
- learning/analytics/evolution;
- FSTSimA and validation stages;
- Web/Communication boundaries;
- MSA/LSA/CSA locality;
- provenance/evidence/replay;
- historical Owner corrections and later Owner directions.

## 9. P0-B non-authority

Starting P0-B does not authorize:

- P0-C or later P0 work packages;
- Part 1 remediation implementation;
- Part 2 through Part 10 implementation;
- provider or broker connectivity;
- runtime market-data flow;
- Service Bus route execution;
- Guardian runtime action;
- Paper, Tiny Live or Live activity;
- deployment or production adoption;
- paid-service purchase;
- Foundation modification from the Application workstream.

## 10. Current state

```text
P0-A = OWNER_ACCEPTED_AND_CLOSED
P0-B = DESIGN_REVIEW_IN_PROGRESS
P0-B_CONTENT_REVIEW = IN_PROGRESS
P0-B_ARCHITECTURE_REVIEW = NOT_YET_RUN
P0-B_RED_TEAM = NOT_YET_RUN
P0-B_OWNER_FINAL_ACCEPTANCE = NOT_GRANTED
P0-C_THROUGH_P0-L = NOT_STARTED
PART1 = FROZEN_PENDING_PART0_REMEDIATION
PART2_THROUGH_PART10 = NOT_AUTHORIZED
```
