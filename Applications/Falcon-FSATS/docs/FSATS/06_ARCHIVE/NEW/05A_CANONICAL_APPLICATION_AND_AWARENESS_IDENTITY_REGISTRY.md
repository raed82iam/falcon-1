# FSATS SIA — Canonical Application and Awareness Identity Registry

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE / IDENTITY-COMPLETENESS REMEDIATION`
**Purpose:** materialize exact stable Application/MSA/LSA identities required by APP-001, CON-023 and P0-E without inventing Foundation-owned lifecycle or authority identifiers.

## 1. Identity Rules

1. These identities are Application-domain logical identities, not Foundation contract IDs, route IDs, endpoint IDs, schema IDs or authority instruments.
2. Display names and short codes are aliases only.
3. Once Owner-accepted, an identity does not change because an implementation assembly, package, provider or host changes.
4. A materially different responsibility/owner must use a new identity rather than silently reusing an old one.
5. Identity registration does not grant admission, activation, permission, runtime or financial authority.
6. The `falcon.app.*` namespace is an SIA candidate Application identity namespace and SHALL be declared through the accepted CON-023 Application Manifest model when implementation/build binding becomes available.
7. Awareness identities are subordinate to exactly one Application identity and exact parent scope.

## 2. Current Four Application Identities

| Short Code | Canonical ApplicationId | Display Name | Status |
|---|---|---|---|
| `APP-TRD` | `falcon.app.trading.core` | Falcon Self-Aware Trading Application | current accepted topology identity candidate for SIA materialization |
| `APP-PMA` | `falcon.app.trading.fsapma` | Falcon Self-Aware Provider Management Application | current accepted topology identity candidate for SIA materialization |
| `APP-GRD` | `falcon.app.trading.guardian` | Falcon Trading Guardian Application | current accepted topology identity candidate for SIA materialization |
| `APP-SIM` | `falcon.app.validation.fstsima` | Falcon Self-Aware Trading Simulation Application | current accepted topology identity candidate for SIA materialization |

The strings above are chosen to align with the already accepted P0-F canonical cross-Application contract-family namespaces. This alignment does not rewrite P0-F and does not claim those older files already declared these exact ApplicationId fields.

## 3. Candidate Fifth Application Identity

If and only if APP-RSC / FSARM is Owner-accepted after fresh review:

| Short Code | Canonical ApplicationId | Display Name | Status |
|---|---|---|---|
| `APP-RSC` | `falcon.app.resource.fsarm` | Falcon Self-Aware Resource Management Application | material candidate, not accepted |

Until Owner acceptance:

```text
falcon.app.resource.fsarm = RESERVED_CANDIDATE_IDENTITY
RUNTIME_PRINCIPAL = NOT_CREATED
MANIFEST_ADMISSION = NOT_GRANTED
```

## 4. FSATS Grouping Identity

`FSATS` remains a non-owning architecture/domain grouping and SHALL NOT receive an `ApplicationId`.

A documentary grouping code may be:

```text
falcon.group.fsats
```

but this identifier, if used in documentation/catalog metadata, SHALL NOT be accepted as:

- Application identity;
- lifecycle principal;
- permission principal;
- resource grant holder;
- MSA/LSA owner;
- route endpoint;
- mutable state owner.

## 5. MSA Identities

### Current

| Application | MSA canonical identity |
|---|---|
| `falcon.app.trading.core` | `falcon.awareness.msa.trading.core` |
| `falcon.app.trading.fsapma` | `falcon.awareness.msa.trading.fsapma` |
| `falcon.app.trading.guardian` | `falcon.awareness.msa.trading.guardian` |
| `falcon.app.validation.fstsima` | `falcon.awareness.msa.validation.fstsima` |

### Candidate APP-RSC

`falcon.awareness.msa.resource.fsarm`

Invariants:

```text
ONE_APPLICATION -> EXACTLY_ONE_MSA
MSA_IDENTITY -> EXACTLY_ONE_APPLICATION_IDENTITY
MSA_IDENTITY != APPLICATION_IDENTITY
MSA_IDENTITY != AUTHORITY_INSTRUMENT
```

## 6. Trading 13 LSA Identities

| Short | Canonical LSA identity | Branch |
|---|---|---|
| T-LSA-01 | `falcon.awareness.lsa.trading.core.operations-account-environment` | Operations, Account & Environment |
| T-LSA-02 | `falcon.awareness.lsa.trading.core.market-instrument-universe` | Market & Instrument Universe |
| T-LSA-03 | `falcon.awareness.lsa.trading.core.analysis-frameworks` | Analysis Frameworks |
| T-LSA-04 | `falcon.awareness.lsa.trading.core.classical-trading` | Classical Trading School |
| T-LSA-05 | `falcon.awareness.lsa.trading.core.opportunity-hunting` | Opportunity Hunting School |
| T-LSA-06 | `falcon.awareness.lsa.trading.core.strategy-orchestration-decision` | Strategy Orchestration & Decision |
| T-LSA-07 | `falcon.awareness.lsa.trading.core.unified-risk` | Unified Risk Management |
| T-LSA-08 | `falcon.awareness.lsa.trading.core.portfolio-capital` | Portfolio & Capital Management |
| T-LSA-09 | `falcon.awareness.lsa.trading.core.execution-position` | Execution & Position Lifecycle |
| T-LSA-10 | `falcon.awareness.lsa.trading.core.learning-knowledge` | Trading Learning & Knowledge |
| T-LSA-11 | `falcon.awareness.lsa.trading.core.analytics-attribution` | Trading Analytics & Attribution |
| T-LSA-12 | `falcon.awareness.lsa.trading.core.strategy-evolution` | Strategy Evolution & Experimentation |
| T-LSA-13 | `falcon.awareness.lsa.trading.core.resource-awareness` | Trading Resource Awareness & Evaluation |

Parent MSA for all = `falcon.awareness.msa.trading.core`.

## 7. FSAPMA 6 LSA Identities

| Short | Canonical LSA identity | Branch |
|---|---|---|
| P-LSA-01 | `falcon.awareness.lsa.trading.fsapma.provider-registry` | Provider Registry & Onboarding |
| P-LSA-02 | `falcon.awareness.lsa.trading.fsapma.data-products-normalization` | Data Products, Semantics & Normalization |
| P-LSA-03 | `falcon.awareness.lsa.trading.fsapma.capability-entitlement` | Provider Capability, Account & Entitlement |
| P-LSA-04 | `falcon.awareness.lsa.trading.fsapma.selection-routing-delivery` | Provider Selection, Routing & Delivery |
| P-LSA-05 | `falcon.awareness.lsa.trading.fsapma.quality-reconciliation` | Data Quality, Verification & Reconciliation |
| P-LSA-06 | `falcon.awareness.lsa.trading.fsapma.quota-capacity-cost-reliability` | Quota, Capacity, Cost & Reliability |

Parent MSA = `falcon.awareness.msa.trading.fsapma`.

## 8. Guardian 4 LSA Identities

| Short | Canonical LSA identity | Branch |
|---|---|---|
| G-LSA-01 | `falcon.awareness.lsa.trading.guardian.observation-incident` | Protection Observation & Incident Qualification |
| G-LSA-02 | `falcon.awareness.lsa.trading.guardian.restriction-command` | Protection Scope, Restriction & Command Governance |
| G-LSA-03 | `falcon.awareness.lsa.trading.guardian.crisis-protection-coordination` | Crisis State, Survival & Protection Coordination |
| G-LSA-04 | `falcon.awareness.lsa.trading.guardian.recovery-evidence` | Reconciliation, Recovery & Protection Evidence |

Parent MSA = `falcon.awareness.msa.trading.guardian`.

## 9. FSTSimA 8 LSA Identities

| Short | Canonical LSA identity | Branch |
|---|---|---|
| S-LSA-01 | `falcon.awareness.lsa.validation.fstsima.time-scenario` | Simulation Time & Scenario |
| S-LSA-02 | `falcon.awareness.lsa.validation.fstsima.market-environment` | Market Environment Simulation |
| S-LSA-03 | `falcon.awareness.lsa.validation.fstsima.provider-external-simulation` | Provider & External Service Simulation |
| S-LSA-04 | `falcon.awareness.lsa.validation.fstsima.broker-execution-simulation` | Broker, Exchange & Execution Simulation |
| S-LSA-05 | `falcon.awareness.lsa.validation.fstsima.account-capital-settlement` | Account, Capital & Settlement Simulation |
| S-LSA-06 | `falcon.awareness.lsa.validation.fstsima.fault-latency-crisis` | Fault, Latency & Crisis Injection |
| S-LSA-07 | `falcon.awareness.lsa.validation.fstsima.fidelity-calibration` | Fidelity & Calibration |
| S-LSA-08 | `falcon.awareness.lsa.validation.fstsima.oracle-evidence-validation` | Oracle, Evidence, Reproducibility & Validation Assessment |

Parent MSA = `falcon.awareness.msa.validation.fstsima`.

## 10. Candidate APP-RSC 3 LSA Identities

These identities are reserved only if APP-RSC is accepted:

| Short | Canonical LSA identity | Branch |
|---|---|---|
| R-LSA-01 | `falcon.awareness.lsa.resource.fsarm.resource-picture-envelope` | Resource Picture, Demand Integrity & Coordination Envelope |
| R-LSA-02 | `falcon.awareness.lsa.resource.fsarm.redistribution-rebalance` | Internal Redistribution, Degradation & Rebalance |
| R-LSA-03 | `falcon.awareness.lsa.resource.fsarm.foundation-binding-restoration-evidence` | Foundation Binding, Restoration & Resource Evidence |

Parent MSA = `falcon.awareness.msa.resource.fsarm`.

## 11. CSA Identity Pattern

The 26 candidate CSA profiles in file 18 use stable identities:

```text
falcon.awareness.csa.<application-domain>.<component-slug>
```

Examples:

```text
CSA-TRD-02 -> falcon.awareness.csa.trading.core.regime-classifier
CSA-PMA-02 -> falcon.awareness.csa.trading.fsapma.data-quality-anomaly-model
CSA-GRD-01 -> falcon.awareness.csa.trading.guardian.incident-correlation-model
CSA-SIM-03 -> falcon.awareness.csa.validation.fstsima.fidelity-calibration-model
```

Before any CSA activation, a generated registry SHALL materialize every candidate that is actually admitted with:

- exact canonical CSA identity;
- exact component owner identity;
- exact parent LSA identity;
- exact Application/MSA chain;
- eligibility evidence;
- artifact/model/config identities;
- permission/tool profile;
- protected properties.

A short `CSA-*` code alone is not sufficient runtime identity.

## 12. Monitor AI Identity Pattern

Current eight Monitor AI perspectives:

```text
falcon.monitor.msa.trading.core.a
falcon.monitor.msa.trading.core.b
falcon.monitor.msa.trading.fsapma.a
falcon.monitor.msa.trading.fsapma.b
falcon.monitor.msa.trading.guardian.a
falcon.monitor.msa.trading.guardian.b
falcon.monitor.msa.validation.fstsima.a
falcon.monitor.msa.validation.fstsima.b
```

Candidate APP-RSC additions if accepted:

```text
falcon.monitor.msa.resource.fsarm.a
falcon.monitor.msa.resource.fsarm.b
```

Monitor identity does not make Monitor AI an Awareness tier or business authority.

## 13. External Counterparty Identity Rule

The accepted P0-F 43-family baseline includes Shared Web and Shared Communication participants.

This SIA SHALL NOT invent their canonical `ApplicationId` values because they are separately governed Applications/workstreams.

Until their admitted manifests expose exact identities, FSATS contract declarations use:

```text
CounterpartyRole = SHARED_WEB_APPLICATION
CounterpartyRole = SHARED_COMMUNICATION_APPLICATION
CanonicalCounterpartyApplicationId = UNRESOLVED_EXTERNAL_OWNER
```

Affected runtime route remains fail closed until exact counterparty identity is available and bilaterally declared.

This is not a gap in FSATS business contract meaning; it is an integration identity gate owned jointly with the external Application workstream/Foundation admission boundary.

## 14. Identity Collision Rules

Verifier SHALL reject:

- duplicate canonical ApplicationId;
- same MSA bound to multiple Applications;
- LSA bound to wrong MSA/Application;
- CSA parent mismatch;
- short code mapped to multiple canonical IDs;
- `falcon.group.fsats` used as ApplicationId/route endpoint/resource principal;
- APP-RSC identity present in accepted/current manifest when APP-RSC Owner decision is not accepted;
- external Shared Web/Communication canonical ID guessed locally;
- changing accepted canonical ID during ordinary package version update.

## 15. Identity Decision Markers

```text
CURRENT_SIA_APPLICATION_ID_COUNT = 4
CURRENT_SIA_MSA_ID_COUNT = 4
CURRENT_SIA_LSA_ID_COUNT = 31
CANDIDATE_APP_RSC_APPLICATION_ID_COUNT = +1
CANDIDATE_APP_RSC_MSA_ID_COUNT = +1
CANDIDATE_APP_RSC_LSA_ID_COUNT = +3
FSATS_GROUPING_APPLICATION_ID = NONE
```

These candidate identity strings become current design only through the final reviewed SIA Owner acceptance. This file does not itself activate or admit them.
