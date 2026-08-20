# FSATS P0-NG — Application Identity, Topology and Manifest Binding Matrix

**Status:** `FINAL_CONSOLIDATION_COMPLETENESS_RECORD / NOT_FINAL_OWNER_CLOSED`  
**Scope:** `P0-C + P0-E + P0-F + P0-G + P0-H + P0-I + P0-K`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Purpose

This matrix removes ambiguity between architectural Application names, system-boundary placement, awareness topology, manifest declarations, and contract participants.

It deliberately does not invent Foundation manifest fields or immutable ID values that have not yet been authoritatively materialized from the applicable CON-023/Application Manifest process.

```text
UNRESOLVED_CANONICAL_ID != PERMISSION_TO_INVENT_ID
UNRESOLVED_CANONICAL_ID = EXPLICIT_FAIL_CLOSED_BINDING_REQUIREMENT
```

---

## 2. Current FSATS Application Set

| Architectural Application | Placement | MSA | LSA | Primary responsibility | Current P0 owner |
|---|---|---:|---:|---|---|
| Falcon Self-Aware Trading Application | Inside FSATS | 1 | 13 | trading business workflow/intelligence/Risk/portfolio/execution/resource coordination | P0-H |
| Falcon Self-Aware Provider Management Application (FSAPMA) | Inside FSATS | 1 | 6 | sole operational external-data/provider-management gateway for Trading domain | P0-G + record 17 |
| Falcon Trading Guardian Application | Inside FSATS | 1 | 4 | Trading protection/crisis scope, restrictions, protection coordination and recovery | P0-I + record 17 |
| Falcon Self-Aware Trading Simulation Application (FSTSimA) | Inside FSATS | 1 | 8 | independent non-Live simulation/validation/evidence | P0-K + record 16 |

FSATS itself is a non-owning system boundary and is not an Application.

```text
FSATS_CONTAINER_MSA = 0
FSATS_CONTAINER_LSA = 0
FSATS_CONTAINER_MANIFEST_PRINCIPAL = NO
```

---

## 3. Shared Applications Used by Current P0-F Baseline

The exact current 43-family P0-F baseline also names:

- Shared Web;
- Shared Communication.

They are independent Shared Falcon Applications outside the FSATS system boundary.

FSATS Part 0 owns only the exact cross-Application contract semantics by which FSATS Applications interact with them. It does not own or redefine their internal topology.

```text
SHARED_WEB != IMPLIED_TRADING_WEB
SHARED_COMMUNICATION != IMPLIED_TRADING_COMMUNICATION
```

---

## 4. Future Trading-Specific Web / Communication Rule

A separately instantiated Web or Communication Application whose primary enduring responsibility is Trading-specific SHALL:

- be inside FSATS;
- have a distinct immutable Application identity;
- have exactly one MSA;
- have exactly one LSA per qualified major branch;
- have its own manifest;
- have explicit P0-F contracts;
- not reuse Shared Web/Communication identity;
- not inherit current 43-family membership automatically.

Until separately governed instantiation occurs:

```text
CURRENT_TRADING_SPECIFIC_WEB_APPLICATION = NOT_INSTANTIATED_BY_P0_NG
CURRENT_TRADING_SPECIFIC_COMMUNICATION_APPLICATION = NOT_INSTANTIATED_BY_P0_NG
```

---

## 5. Canonical Manifest Binding Requirements

For each current FSATS Application, final implementation/admission materialization SHALL bind at least:

- exact authoritative Application identity;
- package/artifact identity;
- exact version/digest/provenance;
- owning role;
- exact MSA identity;
- every LSA identity and owned major branch;
- eligible CSA declarations/policy where applicable;
- declared provided/consumed contracts;
- dependencies and compatible Foundation artifact identities;
- permissions;
- resource requirements;
- persistence/state declarations;
- communication/event declarations;
- health/readiness evidence;
- security requirements;
- self-development escalation route;
- Guardian/FSA interfaces as applicable;
- update/migration/rollback/removal semantics.

No P0 document may treat an architectural display name as proof of the authoritative manifest identity.

---

## 6. Current Awareness Topology Binding

### Trading

```text
1 Trading MSA
13 T-LSAs
T-LSA-13 = Trading Resource Management awareness/evaluation
TARC = separate operational resource controller/requester role
```

### FSAPMA

```text
1 FSAPMA MSA
P-LSA-01 Provider Registry and Onboarding
P-LSA-02 Data Products, Semantics and Normalization
P-LSA-03 Provider Capability, Account and Entitlement
P-LSA-04 Provider Selection, Routing and Delivery
P-LSA-05 Data Quality, Verification and Reconciliation
P-LSA-06 Quota, Capacity, Cost and Reliability
```

### Trading Guardian

```text
1 Guardian MSA
G-LSA-01 Protection Observation and Incident Qualification
G-LSA-02 Protection Scope, Restriction and Command Governance
G-LSA-03 Crisis State, Survival and Protection Coordination
G-LSA-04 Reconciliation, Recovery and Protection Evidence
```

### FSTSimA

```text
1 Simulation MSA
S-LSA-01 Simulation Time and Scenario
S-LSA-02 Market Environment Simulation
S-LSA-03 Provider and External Service Simulation
S-LSA-04 Broker, Exchange and Execution Simulation
S-LSA-05 Account, Capital and Settlement Simulation
S-LSA-06 Fault, Latency and Crisis Injection
S-LSA-07 Fidelity and Calibration
S-LSA-08 Oracle, Evidence, Reproducibility and Validation Assessment
```

---

## 7. Identity vs Authority

```text
APPLICATION_IDENTITY != AUTHORITY
MSA_IDENTITY != APPLICATION_RUNTIME_CONTROL
LSA_IDENTITY != OPERATIONAL_CONTROLLER
CSA_IDENTITY != DEPLOYMENT_AUTHORITY
CONTRACT_PARTICIPANT_IDENTITY != ROUTE_AUTHORITY
MANIFEST_DECLARATION != ACTIVATION
ACTIVATION != BUSINESS_AUTHORIZATION
```

---

## 8. Contract Participant Binding

Every P0-F edge SHALL bind exact authoritative Application identities during manifest/route materialization.

The current 43-family human-readable participants are architectural references to:

- Trading;
- FSAPMA;
- Trading Guardian;
- FSTSimA;
- Shared Web;
- Shared Communication.

No wildcard `FSATS`, `AnyTradingApp`, folder path, Application class, or MSA/LSA identity may substitute for exact producer/consumer Application identity.

---

## 9. Unresolved Identity Handling

If an exact authoritative manifest identity value has not yet been materialized/verified:

```text
IDENTITY_STATE = UNRESOLVED
RUNTIME_ADMISSION = FAIL_CLOSED
ROUTE_BINDING = FAIL_CLOSED
PERMISSION_BINDING = FAIL_CLOSED
CREDENTIAL_BINDING = FAIL_CLOSED
RESOURCE_BINDING = FAIL_CLOSED_WHERE_IDENTITY_REQUIRED
```

The resolution source SHALL be the governed Application Manifest / CON-023 / APP-001 path and accepted artifact evidence, not memory or local invention.

FCR-0016 remains relevant where canonical Foundation artifact consumption is required.

---

## 10. Topology Change Rule

Any change to:

- Application count;
- Application responsibility boundary;
- MSA identity/count;
- qualified major-branch topology;
- LSA identity/count;
- cross-Application contract participant;
- Shared-vs-FSATS placement;

is a semantic architecture change and SHALL trigger the applicable governance, manifest/update, traceability and fresh review cycle.

```text
FOLDER_MOVE != APPLICATION_CHANGE
DISPLAY_RENAME != IDENTITY_CHANGE_UNLESS_GOVERNED_AS_SUCH
NEW_LSA != NEW_PERMISSION
NEW_APPLICATION != AUTOMATIC_CONTRACT_ACCESS
```

---

## 11. Current Foundation Binding

At this candidate refresh:

```text
STAGE_0_THROUGH_5 = ACCEPTED_AND_CLOSED
STAGE_6_WP01_THROUGH_WP04 = ACCEPTED_AND_CLOSED
STAGE_6_WP05_THROUGH_WP10 = NOT_AUTHORIZED
STAGE_7_THROUGH_STAGE_9_IMPLEMENTATION = NOT_AUTHORIZED
```

Application priority does not create Foundation technical criticality.

WP-04 closure does not authorize later resource-pressure/preemption/request/reclamation/load-shedding runtime.

---

## 12. FCR Handoff State

At the latest live gate preceding this record, no substantive open FCR is waiting on Application or Owner.

The material open FCRs remain with Foundation and remain fail closed for dependent runtime claims.

---

## 13. Forbidden Interpretations

Invalid:

- “FSATS is an Application because it contains Applications”;
- “a display name is enough to activate a manifest”;
- “P-LSA/G-LSA/S-LSA names are runtime service identities”;
- “Shared Web is inside FSATS because Trading uses it”;
- “the 43 contract families authorize a future Application automatically”;
- “an unresolved canonical identity may be filled with a convenient local string”;
- “one Application's MSA may own another Application because both are in FSATS”.

---

## 14. Exit Gates

```text
CURRENT_FSATS_APPLICATION_SET = EXPLICIT
FSATS_CONTAINER_NON_APPLICATION = EXPLICIT
TRADING_TOPOLOGY = 1_MSA_13_LSA
FSAPMA_TOPOLOGY = 1_MSA_6_LSA
GUARDIAN_TOPOLOGY = 1_MSA_4_LSA
FSTSIMA_TOPOLOGY = 1_MSA_8_LSA
SHARED_APP_PLACEMENT = EXPLICIT
MANIFEST_IDENTITY_INVENTION_PATHS = 0
UNRESOLVED_IDENTITY_FAIL_CLOSED = EXPLICIT
CONTRACT_PARTICIPANT_WILDCARDS = 0
FRESH_ARCHITECTURE_REVIEW = REQUIRED
FRESH_RED_TEAM_REVIEW = REQUIRED
FINAL_OWNER_CLOSURE = REQUIRED
```
