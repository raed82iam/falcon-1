# FSATS SIA — R5 to R6 Current-State, Conditional Runtime and Provenance Reconciliation

**Package:** `FSATS-SIA-v0.1`
**Record Type:** `CONTROLLING SEMANTIC RECONCILIATION FOR R6`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT_GRANTED`
**Runtime / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

## 1. Purpose

This record preserves the exact R5 semantic history while correcting three material ambiguities discovered during the fresh source-first R5 Architecture/Consistency entry review:

1. current Foundation/FCR state drift after the R5 source snapshot;
2. ambiguity between the accepted/current four-Application topology and the proposed fifth APP-RSC/FSARM runtime identity;
3. ambiguity between an Owner/governance decision reference and actual artifact activation in the immutable provenance graph.

This file does not rewrite R5 history. It is a prospective controlling reconciliation for R6.

## 2. Finding R5-AC-ENTRY-001 — HIGH — Current-State Baseline Drift

R5 file `01_AUTHORITY_SOURCE_AND_CURRENT_STATE_BASELINE.md` captured a previously valid current-state snapshot, including:

- Foundation HEAD `f5eea8266852c6bf1f5695d6e11b6b437e570cad`;
- FCR-0012 and FCR-0030 as `Waiting On: FOUNDATION`.

Fresh source-first review on 2026-08-11 found newer controlling evidence:

```text
FOUNDATION HEAD = d2fae8d78378c4e7865f67c32727edf3b2ed2c72
FOUNDATION README BLOB = 7c62c92321896f96ca7a2676c4013f76a6076d2d
```

Current Foundation state relevant to this SIA:

```text
STAGE 0 THROUGH STAGE 5 = ACCEPTED / CLOSED
STAGE 6 WP-01 THROUGH WP-09 = ACCEPTED / CLOSED
STAGE 6 WP-10 = IMPLEMENTED / STATIC RED-TEAM PASS /
                  EXACT EXECUTABLE VALIDATION PENDING /
                  OWNER CLOSURE NOT_YET
STAGE 6 = IN_PROGRESS / OWNER CLOSURE NOT_YET
STAGE 7 THROUGH STAGE 17 IMPLEMENTATION = NOT AUTHORIZED
```

Current FCR disposition relevant to this SIA:

```text
FCR-0004 = EXISTS / Waiting On: APPLICATION / implementation hold
FCR-0005 = EXISTS / Waiting On: APPLICATION / implementation hold
FCR-0006 = EXISTS / Waiting On: APPLICATION / implementation hold
FCR-0010 = FOUNDATION_IMPLEMENTED / Waiting On: APPLICATION / implementation hold
FCR-0031 = FOUNDATION_IMPLEMENTED / Waiting On: APPLICATION / implementation hold

FCR-0012 = ACCEPTED_FOR_PLANNING / Waiting On: NONE /
           Stage 13 future reconciliation / no current Stage 6 action
FCR-0030 = ACCEPTED_FOR_PLANNING / Waiting On: NONE /
           linked to FCR-0012 Stage 13 future reconciliation
```

`Waiting On: NONE` does not mean closed and does not create Stage 13 authority.

### R6 controlling consequence

Any R5 statement that describes FCR-0012 or FCR-0030 as currently `Waiting On: FOUNDATION` is historical snapshot text only.

For R6 current-state interpretation:

```text
FCR0012_CURRENT_WAITING_ON = NONE
FCR0030_CURRENT_WAITING_ON = NONE
FSA_EXACT_RUNTIME_INTERFACE = STILL FUTURE / NOT AVAILABLE
MSA_TO_FSA_EXACT_BINDING = STILL FUTURE / NOT AVAILABLE
APPLICATION_LOCAL_FSA_SUBSTITUTE = FORBIDDEN
```

The semantic safety gate is unchanged: absence of an immediate Foundation actor does not make the unavailable future interface usable.

## 3. Finding R5-AC-ENTRY-002 — HIGH — APP-RSC / FSARM Conditional Runtime Ambiguity

R5 correctly contains two different truths that must never be collapsed:

### Current accepted/current topology baseline

```text
APPLICATIONS = 4
MSA = 4
LSA = 31
FSATS = NON-OWNING SYSTEM / DOMAIN GROUPING
```

### Prospective SIA topology delta

`11_FSARM_SPECIALIZED_RESOURCE_ARCHITECTURE.md` proposes:

```text
APP-RSC = Falcon Self-Aware Resource Management Application
ROLE = FSARM
APPLICATIONS IF ACCEPTED = 5
MSA IF ACCEPTED = 5
LSA IF ACCEPTED = 34
```

The proposal explicitly requires fresh review and Owner acceptance.

However, several R5 semantic files also describe FSARM state machines, types, contracts, resource plans or topology relations in implementation-ready language. Without a controlling gate, a coding worker could incorrectly materialize a hidden FSARM service or fifth Application before the required Owner topology decision.

### R6 controlling rule

The following invariant governs every R5/R6 file:

```text
APP_RSC_OWNER_ACCEPTED == FALSE
=> APP_RSC_RUNTIME_PRINCIPAL = ABSENT
=> FSARM_RUNTIME_PRINCIPAL = ABSENT
=> R-LSA-01/R-LSA-02/R-LSA-03 = NOT MATERIALIZED
=> APP-RSC MANIFEST = NOT MATERIALIZED AS ADMITTED APP
=> APP-RSC CAPABILITIES/PERMISSIONS = DECLARATION CANDIDATES ONLY
=> CONTRACT FAMILIES #44-59 = CANDIDATE / INACTIVE
=> FSARM STATE MACHINES = SPECIFICATION CANDIDATES / INACTIVE
=> FSARM PERSISTENCE = NOT CREATED
=> FSARM ROUTES = NOT ADMITTED
=> FSARM RESOURCE COMMANDS = NOT EXECUTABLE
```

Only after an explicit Owner acceptance of the exact reviewed APP-RSC topology candidate may a separately authorized implementation package materialize APP-RSC.

Even after topology acceptance:

```text
OWNER TOPOLOGY ACCEPTANCE != IMPLEMENTATION AUTHORITY
IMPLEMENTATION PASS != RUNTIME ACTIVATION
ROUTE ADMISSION != RESOURCE AUTHORITY
```

### No fallback hidden principal

If APP-RSC is not accepted, implementation SHALL NOT fall back to:

- `FSATS.ResourceManager` hidden service;
- Trading-owned FSARM;
- Guardian-owned FSARM;
- distributed peer-election FSARM;
- shared mutable singleton/controller;
- direct peer-to-peer redistribution.

Instead:

```text
NO ACCEPTED APP-RSC
=> AFFECTED FSARM IMPLEMENTATION SCOPE = STOP / OWNER DECISION REQUIRED
```

The current FCR-0031 compatibility evidence remains preserved; this rule only prevents unaccepted Application topology from being materialized by implication.

## 4. R5-AC-ENTRY-003 — MEDIUM — Provenance Adoption / Activation Relation Ambiguity

`19A_IMMUTABLE_AUDIT_PROVENANCE_GRAPH_SPEC.md` correctly preserves that validation, Owner/governance decision, implementation authorization and activation are distinct.

The edge label `ADOPTED_BY_DECISION` is retained for historical R5 compatibility, but R6 clarifies its exact meaning so it cannot be interpreted as deployment/activation authority.

### Exact R6 relation semantics

```text
CANDIDATE_ARTIFACT
  --ADOPTED_BY_DECISION-->
OWNER_DECISION_REF
```

means only:

> the exact candidate is the subject of an explicit governance/Owner adoption or acceptance decision represented by the immutable decision reference.

It SHALL NOT mean:

- code implemented;
- implementation authorized unless the decision explicitly says so;
- artifact deployed;
- artifact activated;
- production authority granted.

Actual active-artifact lineage requires separate evidence:

```text
OWNER_DECISION_REF
  --AUTHORIZED_BY / CONSTRAINED_BY as semantically applicable-->
ACTIVATION_OR_IMPLEMENTATION_DECISION_RECORD

CANDIDATE/IMPLEMENTED_ARTIFACT
  --EXECUTED_AS or exact activation relation owned by the lifecycle profile-->
ACTIVE_ARTIFACT_VERSION
```

For current SIA verification, a target `ACTIVE_ARTIFACT_VERSION` is provenance-complete only when the chain contains all required independently true nodes/refs:

```text
CANDIDATE
-> REQUIRED VALIDATION
-> REQUIRED APPLICATION/AWARENESS REVIEWS
-> FSA COMPATIBILITY REF WHEN APPLICABLE AND AVAILABLE
-> EXPLICIT OWNER/GOVERNANCE DECISION
-> SEPARATE IMPLEMENTATION AUTHORIZATION WHEN REQUIRED
-> IMPLEMENTATION/VERIFICATION EVIDENCE
-> SEPARATE ACTIVATION/PROMOTION AUTHORITY WHEN REQUIRED
-> ACTIVE ARTIFACT VERSION
```

Missing gate => graph state `INCOMPLETE`; no edge may fabricate the missing authority.

## 5. Preserved R5 Semantics

Except for the controlling reconciliations above, R5 semantic content remains unchanged.

In particular this record does not alter:

- four-Application current baseline;
- proposed APP-RSC purpose or three-LSA decomposition;
- Foundation ownership of total-resource truth/grants/ceilings;
- `INTERNAL_REDISTRIBUTION_FIRST` / `FOUNDATION_ADDITIONAL_REQUEST_SECOND`;
- `REQUESTED_RESOURCE != GRANTED_RESOURCE`;
- 43 accepted historical cross-Application contract families;
- 16 APP-RSC candidate additions;
- 14 strategy-family candidate catalog;
- 26 CSA candidate profiles;
- Risk/capital/market/provider formulas;
- FSTSimA deterministic-randomness profile;
- neutral active school weighting;
- federated immutable provenance graph ownership.

## 6. R6 Source Snapshot

R6 source-first review is bound to at least:

```text
applications/FSATS/WORKSTREAM_RULES.md
  blob 07373b0f5c12e5186025c46aa02b906582a73cc1

applications/README.md
  blob e9b3a059878adb8ed47135db4f707943bb2e5fd1

applications/FSATS/README.md
  blob 551ff1fef12500cadb11b2f1d9f1eafbdae8ab56

Falcon Vision
  blob 7a8afe912e1840e84815ecfa95db0f1c9c45a8b6

APP-001 v1.1
  blob af31ab590a351b0e9f8c47ad2bf7048f3a2b676f

CON-023 v1.1
  blob 658177581b2c83b95c19a623b530f1655682b367

ADR-I012 v1.1
  blob 0a0a8ce8a686af7553828f1478a3b09362a037f6

ADR-I015 v1.0
  blob efc330d4718ec3272875825068eaa70ccc0b3fdd

Foundation current README
  blob 7c62c92321896f96ca7a2676c4013f76a6076d2d

Foundation HEAD
  d2fae8d78378c4e7865f67c32727edf3b2ed2c72
```

The Falcon Constitution remains Ratified and Approved and subordinate only to the Vision.

## 7. Verification Additions

R6 verifier/review coverage SHALL include:

1. stale FCR `Waiting On` metadata cannot override live FCR current-state header;
2. FCR-0012/0030 `Waiting On: NONE` cannot be interpreted as capability availability;
3. four-Application baseline remains the current topology before Owner APP-RSC decision;
4. no APP-RSC runtime project/host/manifest/route/state/persistence before explicit Owner topology acceptance + separate implementation authority;
5. no hidden FSATS-level resource coordinator fallback;
6. candidate APP-RSC contract families remain inactive before topology acceptance;
7. `ADOPTED_BY_DECISION` cannot satisfy implementation/activation authority by itself;
8. active-artifact provenance closure fails if implementation or activation authority is missing where required.

## 8. Finding Disposition

```text
R5-AC-ENTRY-001 = REMEDIATED_FOR_R6
R5-AC-ENTRY-002 = REMEDIATED_FOR_R6
R5-AC-ENTRY-003 = REMEDIATED_FOR_R6

R5 = HISTORICAL SEMANTIC FREEZE / NOT OWNER ACCEPTED
R6 = REQUIRES NEW SEMANTIC FREEZE + FRESH A/C + FRESH RED-TEAM
```
