# FSATS Part 0 — Read Me First: Current Effective Authority and Supersession Map

**Status:** `DOCUMENTATION_RECONCILIATION / CURRENT_NAVIGATION_GUIDE / NO_NEW_DESIGN_AUTHORITY`  
**Branch:** `application-development`  
**Purpose:** Make the effective Part 0 state unambiguous for a human programmer after documentation reorganization.  
**Important:** This file does not rewrite accepted history and does not create implementation, runtime, deployment, provider, broker, Paper, Shadow, Tiny-Live, or Live authority.

---

## 1. Start Here

Do not interpret one Part 0 file in isolation.

The effective FSATS state was built in sequence. Earlier accepted Part 0 files remain valid historical accepted baseline evidence, but later accepted Owner decisions and later accepted Parts control where they explicitly changed that baseline.

Use this order:

```text
1. Accepted Part 0 baseline:            PART_0/P0-NG/**
2. Accepted Part 0 closure:             PART_0/P0-L/**
3. Accepted Awareness amendment:        PART_0/AWARENESS_AMENDMENT/**
4. APP-RSC / fifth-Application adoption: PART_1/10_APP_RSC_OWNER_FINAL_ACCEPTANCE.md
5. Final five-Application Part 1 state:  PART_1/17_PART1_OWNER_FINAL_ACCEPTANCE_AND_CLOSURE.md
6. Broker-account identity correction:  PART_2/14_PART2_OWNER_BROKER_ACCOUNT_IDENTITY_CLARIFICATION_AND_RED_TEAM_RESCOPING.md
7. Final Part 2 closure:                 PART_2/24_PART2_OWNER_FINAL_ACCEPTANCE_AND_CLOSURE.md
8. Parts 3-6 final closure records and exact accepted executable-source identities.
9. Live FCR state for any still-open cross-workstream or Foundation dependency.
```

If an earlier file conflicts with a later accepted Owner decision inside the later decision's scope, the later accepted decision controls prospectively. Preserve the earlier file as history; do not silently rewrite it.

---

## 2. Current FSATS Topology

The current accepted FSATS topology is **five independent Falcon Applications**:

```text
1. Falcon Self-Aware Trading Application
   MSA = 1
   LSA = 13
   CSA = 3

2. Falcon Self-Aware Provider Management Application (FSAPMA)
   MSA = 1
   LSA = 6
   CSA = 1

3. Falcon Trading Guardian Application
   MSA = 1
   LSA = 4
   CSA = 1

4. Falcon Self-Aware Trading Simulation Application (FSTSimA)
   MSA = 1
   LSA = 8
   CSA = 2

5. Falcon Self-Aware Resource Management Application (APP-RSC)
   MSA = 1
   LSA = 3
   CSA = 0 initially
```

Totals:

```text
APPLICATIONS = 5
MSA = 5
LSA = 34
CSA = 7
```

FSATS itself remains a **non-owning, non-runtime system boundary**. It is not a sixth Application and is not an authority principal.

---

## 3. Why Some Part 0 Files Show Four Applications

The accepted P0-NG/P0-L baseline was completed before the later Owner-directed APP-RSC correction was finalized.

Therefore, references in the accepted historical Part 0 baseline to four Applications are historically correct for that accepted freeze, but they are **not the current topology**.

APP-RSC/FSARM design lineage was explored in archived `NEW/**`, including:

```text
06_ARCHIVE/NEW/11_FSARM_SPECIALIZED_RESOURCE_ARCHITECTURE.md
06_ARCHIVE/NEW/12A_ACCEPTED_43_CONTRACT_BASELINE_RECONCILIATION_AND_FSARM_EXTENSION.md
```

Those `NEW/**` files are design lineage and review history. They do not become current authority merely because they are newer or more detailed.

The controlling acceptance of APP-RSC comes from the later Part 1 Owner-accepted chain, especially:

```text
PART_1/10_APP_RSC_OWNER_FINAL_ACCEPTANCE.md
PART_1/17_PART1_OWNER_FINAL_ACCEPTANCE_AND_CLOSURE.md
```

For convenience and traceability, the relevant lineage and controlling accepted records are mirrored without byte changes under:

```text
PART_0/APP_RSC_CURRENT/
```

The copies do not change the original source authority or commit history.

---

## 4. Current Resource-Management Meaning

Do not implement the old Part 0 TARC model as the final FSATS-wide resource-management owner.

Current effective relationship:

```text
T-LSA-13
= Trading resource awareness / evaluation inside Trading

APP-RSC
= independent FSATS-only Falcon Application for governed FSATS resource coordination

FOUNDATION RESOURCE GOVERNANCE
= authoritative owner of Falcon-wide total-resource truth, grants, ceilings, floors and Foundation resource authority
```

Mandatory distinctions:

```text
T-LSA-13 != APP-RSC
APP-RSC != FOUNDATION RESOURCE GOVERNANCE
APP-RSC != FSATS CONTAINER
APP-RSC != AUTHORITY TO MINT FOUNDATION RESOURCE GRANTS
```

Operational rule preserved by the later accepted design:

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
REQUESTED_RESOURCE != PROVEN_RESIDUAL_NEED != GRANTED_RESOURCE
```

Final canonical runtime binding to Foundation remains governed by live FCR state and does not arise from documentation alone.

---

## 5. The Part 0 43/43 Contract Graph

P0-F/P0-L's `43/43` validation is a valid accepted **Part 0 baseline contract graph**.

Do not interpret it as the complete current FSATS contract inventory after later Parts.

Part 1 later preserved that 43/43 baseline by reference and added additional explicit contract families. Parts 2 and later materialized further accepted implementation semantics and cross-workstream bindings.

Therefore:

```text
P0_43_OF_43 = VALID_ACCEPTED_BASELINE
P0_43_OF_43 != COMPLETE_CURRENT_FSATS_CONTRACT_SET
```

For current implementation, use the current contract workspace, accepted Part 1/Part 2 records, source, tests, and live FCR state rather than reconstructing contracts from P0-L alone.

---

## 6. Broker Account Identity Controls Over User-Centric Candidate Wording

The later Owner clarification in Part 2 controls current FSATS trading-runtime identity:

```text
FSATS_USER_ID = NONE
FSATS_USERNAME = NONE
FSATS_CUSTOMER_ID = NONE
FSATS_CUSTOMER_ACCOUNT_OWNERSHIP_GRAPH = NONE

TRADING OPERATING SUBJECT = BROKER ACCOUNT
BROKER ACCOUNT BUSINESS IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = additional identity dimension where material
```

Shared Web owns customer/user/contact to broker-account mapping.

Therefore, any earlier or still-unaccepted Part 0 Market Qualification candidate wording that appears to make FSATS itself own a customer/user identity must not be implemented literally.

A customer-facing mandate may originate through an external/customer-facing surface, but the FSATS runtime side must remain bound to the governed broker-account scope and accepted authority model.

---

## 7. Market Qualification Files at PART_0 Root

The Market Qualification / Expansion files currently present directly under `PART_0/` are a later design candidate/review chain.

They include the `00...00G` hardening files plus Architecture/Red-Team/Owner-review gates.

They are **not automatically part of the accepted Part 0 baseline merely because they live in this folder**.

Before implementing any clause from them, verify the exact file status and whether an explicit Owner acceptance exists for the exact reviewed semantic version.

Where they conflict with an already accepted later Owner correction, especially the Part 2 broker-account identity model, the accepted later correction controls unless the Owner explicitly changes it again.

---

## 8. Awareness Amendment

`PART_0/AWARENESS_AMENDMENT/**` remains part of the accepted Part 0 authority chain.

Its durable boundaries remain important:

```text
FSA = FOUNDATION OWNED
MSA / LSA / CSA = APPLICATION OWNED
AWARENESS_RANK != AUTHORITY
FSA_REVIEW != PRODUCTION_ADOPTION
OWNER_SILENCE != AUTHORITY
TIMER_EXPIRY != AUTHORITY
```

However, counts or examples written while the system had four Applications must be read together with the later accepted five-Application topology. Do not infer a new monitor count or new APP-RSC monitor topology without an accepted source that explicitly resolves that question.

---

## 9. Parts 2-6 Are Later Accepted Implementation Evidence

Do not treat P0-L's implementation-readiness gate as a replacement for later executable evidence.

The later accepted executable-source identities are:

```text
PART 2 = 0045acef6de8157d580fcfa37af590225861db55
PART 3 = 0be363b713e96c8b1eeb81ae7e5fb7e5d5e562b4
PART 4 = 827c3067a28755638e4851090048f6e38383cf64
PART 5 = 33a1e24bd927b7083259ff89a2def6e89b458e8f
PART 6 = 697d48b6a3e2532747e68bcf5439d808a1e1f29f
```

Later documentation commits must not be substituted for these exact tested executable identities.

---

## 10. Current Authority State

At this documentation reconciliation point:

```text
PART 0 = OWNER_ACCEPTED_AND_CLOSED baseline, with accepted later amendments/corrections read prospectively
PART 1 = OWNER_ACCEPTED_AND_CLOSED
PART 1 CSA AMENDMENT = OWNER_ACCEPTED_AND_CLOSED
PART 2 = OWNER_ACCEPTED_AND_CLOSED
PART 3 = OWNER_ACCEPTED_AND_CLOSED
PART 4 = OWNER_ACCEPTED_AND_CLOSED
PART 5 = OWNER_ACCEPTED_AND_CLOSED
PART 6 = OWNER_ACCEPTED_AND_CLOSED

PART 7+ = NOT AUTHORIZED unless a later explicit Owner decision says otherwise
RUNTIME = NOT AUTHORIZED
PROVIDER CONNECTIVITY = NOT AUTHORIZED
BROKER CONNECTIVITY = NOT AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT AUTHORIZED
```

Always re-check the current repository and live FCR headers before acting because other workstreams may have advanced independently.

---

## 11. Programmer Rule

For implementation decisions, do not choose a source because its filename says `CURRENT`, `FINAL`, `NEW`, `PASS`, or because it has the newest timestamp.

Use:

```text
CURRENT REPOSITORY STATE
-> AUTHORITY / OWNER DECISION ORDER
-> ACCEPTED SEMANTIC VERSION
-> EXACT IMPLEMENTATION / TEST EVIDENCE
-> LIVE FCR STATE
-> IMPLEMENT
```

Never:

```text
NEWEST LOOKING FILE
-> ASSUME IT IS CURRENT
-> IMPLEMENT
```

This guide exists specifically to prevent that failure mode.
