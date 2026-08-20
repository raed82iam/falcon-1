# FSATS — FSAPMA and Trading Guardian Canonical LSA Topology and Ownership

**Status:** `FINAL_CONSOLIDATION_SEMANTIC_REMEDIATION / NOT_FINAL_OWNER_CLOSED`  
**Affected Scope:** `P0-C + P0-G + P0-I`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

---

## 1. Purpose

This record removes the remaining major-Application topology ambiguity in the P0-NG final-consolidation candidate.

The accepted architecture requires exactly one MSA per Application and exactly one LSA per qualified major Application branch. Historical Owner-directed FSATS evidence preserved six FSAPMA LSA rooms and four Guardian LSA rooms, but earlier source material spans multiple architecture eras and therefore cannot be copied blindly.

This final candidate establishes one explicit current topology, derived from the preserved current business responsibilities and current Application separation rules, so future readers do not need to infer branch count, branch ownership, or awareness placement.

After final Owner closure, these semantics SHALL be integrated directly into active Current Approved P0-C/P0-G/P0-I.

---

# PART A — FSAPMA

## 2. FSAPMA Canonical Topology

```text
Falcon Self-Aware Provider Management Application (FSAPMA)
└── FSAPMA MSA
    ├── P-LSA-01 — Provider Registry and Onboarding
    ├── P-LSA-02 — Data Products, Semantics and Normalization
    ├── P-LSA-03 — Provider Capability, Account and Entitlement
    ├── P-LSA-04 — Provider Selection, Routing and Delivery
    ├── P-LSA-05 — Data Quality, Verification and Reconciliation
    └── P-LSA-06 — Quota, Capacity, Cost and Reliability
```

```text
FSAPMA_MSA_COUNT = 1
FSAPMA_MAJOR_BRANCH_COUNT = 6
FSAPMA_LSA_COUNT = 6
```

No seventh FSAPMA LSA exists by implication.

Operational components remain operational components. LSA placement does not replace Provider Controller, registry, validators, schedulers, streaming managers, adapters, caches, circuit breakers, or normalizers.

---

## 3. FSAPMA MSA

FSAPMA MSA owns complete FSAPMA Application awareness and final FSAPMA Application evaluation/recommendation.

It SHALL understand the combined state of all six provider-management branches, including cross-branch conflicts such as:

- capability exists but entitlement does not;
- provider is eligible but capacity is exhausted;
- route is available but data quality is below contract;
- data is semantically valid but stale;
- source is high quality but legally/licensing restricted;
- fallback is technically available but violates cost/authority policy;
- provider onboarding is complete but external egress authority is absent.

It does not perform normal provider selection or acquisition in place of runtime controllers.

---

## 4. P-LSA-01 — Provider Registry and Onboarding

### Owns

- provider/service identity registration;
- provider lifecycle state inside FSAPMA;
- provider group membership;
- onboarding/offboarding evidence;
- source-independence/dependency lineage;
- provider endpoint/service catalog registration;
- licensing/legal metadata registration;
- adapter registration identity;
- approved-provider baseline history.

### Produces

- `ProviderRegistrySnapshot`;
- onboarding evidence;
- provider lifecycle state;
- source/dependency lineage.

### Does Not Own

- runtime provider selection;
- data quality judgment;
- quota admission;
- provider credential secret values;
- Foundation egress authorization;
- Trading business decisions.

```text
REGISTERED_PROVIDER != RUNTIME_ELIGIBLE_ROUTE
ONBOARDED_PROVIDER != EXTERNAL_EGRESS_AUTHORIZED
```

---

## 5. P-LSA-02 — Data Products, Semantics and Normalization

### Owns

- canonical Data Product identities;
- product schemas and semantic definitions;
- field meaning/precision/unit rules;
- normalization contracts;
- one-canonical-source-per-product-instance semantics where applicable;
- no-mixed-candle/data-product integrity rules;
- product freshness/quality requirements as declared contract semantics;
- provider-native-to-canonical transformation requirements.

### Produces

- Data Service Contract definitions;
- canonical schema/semantic snapshots;
- normalization rules and version lineage.

### Does Not Own

- provider choice;
- provider health score;
- runtime quota;
- truth verification outcome for one observation;
- Trading interpretation of the data.

```text
DATA_PRODUCT_SEMANTICS != DATA_QUALITY_OUTCOME
NORMALIZATION != VERIFICATION
```

---

## 6. P-LSA-03 — Provider Capability, Account and Entitlement

### Owns

- provider/service/plan capability profiles;
- endpoint-level product/field/history/precision/session capability;
- REST/WebSocket/batch/page/streaming support;
- account/plan entitlement state supplied to FSAPMA;
- provider-role capability separation;
- market/instrument coverage capability;
- service-role restrictions such as market-data-only versus execution-capable vendor service.

### Produces

- `CapabilitySnapshot`;
- entitlement/capability evidence;
- exact unsupported-capability truth.

### Does Not Own

- secret credential material;
- Foundation credential-reference authority;
- provider selection;
- quota/cost admission;
- real broker execution authority.

```text
PROVIDER_BRAND_CAPABLE != ACCOUNT_ENTITLED
ACCOUNT_ENTITLED != FOUNDATION_EGRESS_AUTHORIZED
MARKET_DATA_ROLE != BROKER_EXECUTION_ROLE
```

---

## 7. P-LSA-04 — Provider Selection, Routing and Delivery

### Owns

- requirement resolution for an authorized Data Product request;
- candidate-provider resolution;
- runtime route eligibility filtering using owner snapshots;
- Provider Controller operational responsibility;
- deterministic provider/route selection;
- bounded acquisition-command planning;
- fallback orchestration;
- parallel/redundant/best-response routing where authorized;
- streaming subscription reuse and consumer registration;
- cache/deduplication use as part of bounded request fulfillment;
- delivery of normalized FSAPMA-owned Data Products through governed contracts;
- explicit degraded/NACK outcome when requirements cannot be met.

### Required Inputs

- P-LSA-01 registry snapshot;
- P-LSA-02 product/semantic contract;
- P-LSA-03 capability/entitlement snapshot;
- P-LSA-05 quality snapshot;
- P-LSA-06 capacity/cost/reliability snapshot;
- Guardian provider restriction where applicable;
- current Foundation/runtime route/egress readiness.

### Does Not Own

- source registry truth;
- product semantic definition;
- quality truth manufacture;
- quota truth manufacture;
- paid-provider purchase authority;
- external egress authority;
- Trading business decision.

```text
PROVIDER_CONTROLLER = OPERATIONAL_RUNTIME_OWNER_INSIDE_P-LSA-04
P-LSA-04_AWARENESS != PROVIDER_CONTROLLER_RUNTIME
ROUTE_SELECTED != PROVIDER_EGRESS_AUTHORIZED
```

---

## 8. P-LSA-05 — Data Quality, Verification and Reconciliation

### Owns

- freshness/completeness validation;
- cross-provider verification;
- duplicate/circular-source detection;
- conflict detection;
- confidence-quality assessment;
- correction/supersession handling;
- source independence assessment;
- rumor/news credibility classification where applicable;
- data-quality degradation state;
- reconciliation of conflicting observations into explicit bounded outcome or unresolved state.

### Produces

- `QualitySnapshot`;
- quality/confidence/provenance evidence;
- contradiction/unresolved flags;
- correction lineage.

### Does Not Own

- provider registry;
- provider routing authority;
- Trading signal/strategy decision;
- fabrication of missing truth;
- treating repeated articles as independent confirmation.

```text
REPETITION != INDEPENDENT_CONFIRMATION
LOW_CONFIDENCE != FALSE
UNRESOLVED_CONFLICT != PERMISSION_TO_GUESS
```

---

## 9. P-LSA-06 — Quota, Capacity, Cost and Reliability

### Owns

- provider rate-limit and quota state;
- endpoint/concurrency/stream capacity state;
- protected provider-capacity reserve;
- queue/load state within FSAPMA provider usage;
- provider cost/plan consumption state;
- free-first policy evidence;
- provider latency/reliability/availability operational score inputs;
- circuit-breaker/failure-recovery readiness evidence;
- bounded provider-use capacity admission supplied to P-LSA-04.

### Produces

- `CapacitySnapshot`;
- cost/quota/reliability evidence;
- capacity-pressure/degradation state.

### Does Not Own

- Foundation CPU/RAM/network resource truth;
- Foundation resource grants;
- automatic purchase/upgrade authority;
- Trading TARC authority;
- product semantics;
- provider selection final decision.

```text
PROVIDER_QUOTA != FOUNDATION_RESOURCE_QUOTA
FSAPMA_CAPACITY != TRADING_TARC_RESOURCE_AUTHORITY
FREE_TIER_EXHAUSTED != AUTO_PURCHASE_AUTHORITY
```

---

## 10. FSAPMA Canonical Request Flow

```text
AUTHORIZED DATA PRODUCT REQUEST
        ↓
P-LSA-02 contract/semantic requirements
        ↓
P-LSA-01 registered providers
        ↓
P-LSA-03 capability/entitlement filter
        ↓
P-LSA-05 current quality constraints
        ↓
P-LSA-06 quota/capacity/cost/reliability constraints
        ↓
P-LSA-04 Provider Controller selects bounded route
        ↓
FOUNDATION-GOVERNED PROVIDER EGRESS WHEN AVAILABLE
        ↓
provider response
        ↓
P-LSA-05 validation/verification/reconciliation
        ↓
P-LSA-02 normalization to canonical Data Product
        ↓
P-LSA-04 governed internal delivery
        ↓
AUTHORIZED CONSUMER APPLICATION
```

No step may be skipped because another branch happens to possess enough technical information to imitate it.

---

# PART B — TRADING GUARDIAN

## 11. Trading Guardian Canonical Topology

```text
Falcon Trading Guardian Application
└── Guardian MSA
    ├── G-LSA-01 — Protection Observation and Incident Qualification
    ├── G-LSA-02 — Protection Scope, Restriction and Command Governance
    ├── G-LSA-03 — Crisis State, Survival and Protection Coordination
    └── G-LSA-04 — Reconciliation, Recovery and Protection Evidence
```

```text
GUARDIAN_MSA_COUNT = 1
GUARDIAN_MAJOR_BRANCH_COUNT = 4
GUARDIAN_LSA_COUNT = 4
```

Guardian is an independent Application. It is not a Trading branch and its MSA is not Trading MSA.

---

## 12. Guardian MSA

Guardian MSA owns complete Guardian-Application awareness and final Guardian self-development evaluation/recommendation.

It SHALL understand:

- incident-detection quality;
- false-positive/false-negative patterns;
- restriction/command behavior;
- crisis-state correctness;
- survival coordination quality;
- recovery/release quality;
- authority compliance;
- unresolved protection gaps;
- cumulative Guardian change impact.

It SHALL NOT itself replace Guardian operational protection controllers merely because it understands them.

---

## 13. G-LSA-01 — Protection Observation and Incident Qualification

### Owns

- ingestion of attributable domain-owned safety/protection evidence;
- incident candidate identity;
- protection-condition detection/qualification;
- evidence sufficiency for declaring Guardian concern;
- incident class/severity assessment under approved Guardian semantics;
- false-positive/false-negative learning;
- ambiguity state when evidence is insufficient.

### Does Not Own

- Trading Risk business calculations;
- FSAPMA data-quality repair;
- Execution reconciliation truth;
- Foundation containment truth;
- final target restriction/command effect.

```text
DOMAIN_FAILURE_EVIDENCE != GUARDIAN_OWNERSHIP_OF_DOMAIN
INCIDENT_SIGNAL != AUTOMATIC_PROTECTION_COMMAND
UNKNOWN != SAFE
```

---

## 14. G-LSA-02 — Protection Scope, Restriction and Command Governance

### Owns

- smallest-safe Trading protection scope selection;
- bounded restriction/command semantics;
- exact target/scope/authority/expiry identity;
- command idempotency/replay protection requirements;
- no-new-exposure/restriction/release command intent;
- protection-command precedence within Guardian authority;
- protection release as a new governed command, not disappearance of prior state.

### Does Not Own

- broker execution itself;
- Trading strategy/risk/portfolio business decisions;
- Foundation route activation;
- Foundation resource request;
- target-Application internal implementation.

```text
GUARDIAN_COMMAND != BROKER_ORDER
GUARDIAN_COMMAND != FOUNDATION_ROUTE_AUTHORITY
GUARDIAN_PROTECTION_SCOPE != DOMAIN_BUSINESS_OWNERSHIP
```

---

## 15. G-LSA-03 — Crisis State, Survival and Protection Coordination

### Owns

- Guardian protection/crisis state machine;
- NORMAL/WARNING/RESTRICTED/SAFE_MODE/RECOVERY protection-state semantics as applicable;
- essential-vs-nonessential protection-survival coordination;
- cross-domain protective coordination using declared contracts;
- preservation of open-position supervision requirements;
- escalation of protection urgency/evidence to proper owners;
- survival constraints during uncertainty.

### Does Not Own

- Trading resource requests to Foundation;
- Foundation technical criticality;
- domain-specific recovery implementation;
- normal Trading orchestration;
- production code modification during crisis.

Resource rule:

```text
GUARDIAN_RESOURCE_URGENCY_EVIDENCE -> TARC
TARC -> FOUNDATION RESOURCE GOVERNANCE
GUARDIAN_DIRECT_FOUNDATION_RESOURCE_REQUEST = PROHIBITED
```

---

## 16. G-LSA-04 — Reconciliation, Recovery and Protection Evidence

### Owns

- Guardian command/outcome reconciliation;
- unresolved-command-effect tracking;
- recovery-readiness assessment from domain evidence;
- Guardian-side release-readiness assessment;
- incident timeline/protection evidence lineage;
- proof that restrictions can be safely narrowed/released;
- post-incident Guardian effectiveness analysis;
- recovery/release evidence and contradiction preservation.

### Does Not Own

- fabrication of target business outcome;
- Trading Execution reconciliation source truth;
- FSAPMA provider recovery truth;
- Foundation recovery truth;
- automatic return to NORMAL merely because transport ACK exists.

```text
DELIVERY_ACK != PROTECTION_EFFECT
TARGET_OUTCOME_UNKNOWN != RECOVERED
RECOVERY_EVIDENCE_INCOMPLETE -> RELEASE_BLOCKED_OR_NARROWED
```

---

## 17. Guardian Canonical Protection Flow

```text
DOMAIN OWNER DETECTS / OWNS DOMAIN FAILURE SCOPE
        ↓
attributable evidence
        ↓
G-LSA-01 qualifies Guardian incident concern
        ↓
G-LSA-02 determines smallest-safe protection scope and command semantics
        ↓
G-LSA-03 coordinates Guardian crisis/survival state
        ↓
GOVERNED P0-F / FOUNDATION ROUTE WHEN AVAILABLE
        ↓
TARGET APPLICATION validates/applies only its owned behavior
        ↓
target business outcome evidence
        ↓
G-LSA-04 reconciles effect and recovery readiness
        ↓
G-LSA-02 issues explicit governed release/narrowing command when justified
        ↓
G-LSA-03 transitions Guardian protection state only after gates pass
```

No Guardian LSA may jump into another Application's private state.

---

# PART C — CROSS-APPLICATION TOPOLOGY RULES

## 18. Current Major Application Awareness Counts

For the P0-NG final candidate:

```text
FSATS_CONTAINER_MSA = 0

TRADING_APPLICATION_MSA = 1
TRADING_APPLICATION_LSA = 13

FSAPMA_MSA = 1
FSAPMA_LSA = 6

TRADING_GUARDIAN_MSA = 1
TRADING_GUARDIAN_LSA = 4

FSTSIMA_MSA = 1
FSTSIMA_LSA = 8
```

Shared Web and Shared Communication are independent Shared Applications outside the FSATS system boundary in the exact current P0-F 43-family baseline. Their internal branch topology is not owned or redefined by FSATS Part 0.

A future trading-specific Web or Communication Application, if separately instantiated, belongs inside FSATS but requires a distinct Application identity, MSA, qualified branch LSAs, manifest, contracts and fresh governance. It is not silently present today.

---

## 19. No Hidden Application / No Hidden Branch Rule

```text
FSATS_CONTAINER != APPLICATION
APPLICATION != LSA_ROOM
OPERATIONAL_COMPONENT != LSA
LSA != OPERATIONAL_CONTROLLER
MSA != MASTER_RUNTIME_CONTROLLER
```

Every major branch listed here must have a declared LSA.

Every operational controller must remain an operational controller even when observed by an LSA/CSA.

No directory/folder name creates an Application or LSA by itself.

---

## 20. Self-Development Routes

For FSAPMA:

```text
ELIGIBLE CSA -> PARENT P-LSA -> FSAPMA MSA -> FSA -> OWNER/VALID GOVERNANCE
P-LSA ORIGIN -> FSAPMA MSA -> FSA -> OWNER/VALID GOVERNANCE
FSAPMA MSA ORIGIN -> FSA -> OWNER/VALID GOVERNANCE
```

For Guardian:

```text
ELIGIBLE CSA -> PARENT G-LSA -> GUARDIAN MSA -> FSA -> OWNER/VALID GOVERNANCE
G-LSA ORIGIN -> GUARDIAN MSA -> FSA -> OWNER/VALID GOVERNANCE
GUARDIAN MSA ORIGIN -> FSA -> OWNER/VALID GOVERNANCE
```

No Trading MSA is inserted into Guardian/FSAPMA self-development solely because they are inside FSATS.

Cross-Application impact is handled through contracts/evidence/governance, not awareness ownership inheritance.

---

## 21. Failure Rules

- loss of one FSAPMA LSA blocks claims that require that branch's current evidence;
- loss of P-LSA-05 cannot be replaced by P-LSA-04 guessing data quality;
- loss of P-LSA-06 cannot be replaced by selection logic assuming capacity;
- loss of Guardian G-LSA-01 cannot be replaced by treating every domain anomaly as a crisis;
- loss of G-LSA-04 cannot be replaced by assuming a protection command succeeded;
- MSA failure does not transfer complete-Application evaluation authority to one LSA;
- sibling LSA failure never expands another LSA's jurisdiction automatically.

---

## 22. Current Foundation / FCR Dependencies

FSAPMA runtime dependencies remain governed by, among others:

- FCR-0005 operational market-data internal delivery;
- FCR-0008 research-only egress;
- FCR-0009 deadline/QoS transport;
- FCR-0013 provider operational egress/credential-reference boundary;
- FCR-0016 canonical Foundation artifact consumption.

Guardian runtime dependencies remain governed by, among others:

- FCR-0004 governed protection command route;
- FCR-0006 event/evidence/replay delivery;
- FCR-0009 deadline/QoS transport;
- FCR-0010 later resource-pressure/load-shedding stages where Guardian supplies evidence but TARC remains Trading requester;
- FCR-0016 canonical Foundation artifact consumption.

Open Foundation dependencies remain fail closed for the dependent runtime capability.

---

## 23. Prime Invariants

```text
FSAPMA_MSA_COUNT = 1
FSAPMA_LSA_COUNT = 6
GUARDIAN_MSA_COUNT = 1
GUARDIAN_LSA_COUNT = 4
FSTSIMA_MSA_COUNT = 1
FSTSIMA_LSA_COUNT = 8
TRADING_MSA_COUNT = 1
TRADING_LSA_COUNT = 13
FSATS_CONTAINER_MSA_COUNT = 0

ONE_MAJOR_BRANCH = ONE_LSA
AWARENESS != RUNTIME_CONTROLLER
TOPOLOGY != AUTHORITY
SHARED_APPLICATION != FSATS_OWNED_APPLICATION
GUARDIAN != TRADING_RISK
GUARDIAN != FOUNDATION_RESOURCE_REQUESTER
FSAPMA != FOUNDATION_PROVIDER_SERVICE
PROVIDER_QUOTA != FOUNDATION_RESOURCE_QUOTA
```

---

## 24. Forbidden Interpretations

Explicitly invalid:

- “FSAPMA has one giant LSA because old documents once described one Application-level LSA”;
- “FSAPMA has six LSAs, therefore six independent provider runtimes must exist”;
- “Provider Selection LSA may redefine data semantics because it consumes them”;
- “Quota LSA owns Foundation resources”;
- “Guardian has four LSAs, therefore each has independent crisis authority over other Applications”;
- “Guardian MSA may command Trading MSA”;
- “Guardian can bypass TARC for emergency resources”;
- “Guardian protection scope gives ownership of Risk, Execution or provider business semantics”;
- “Shared Web/Communication are hidden FSATS Applications”;
- “future Trading Web/Communication are already represented in current topology”.

---

## 25. Mandatory Review Attacks

Fresh review SHALL attack at least:

- missing/duplicate FSAPMA LSA;
- selection branch stealing registry/quality/capacity ownership;
- quality branch selecting provider route;
- capacity branch claiming Foundation resources;
- Provider Controller replaced by awareness entity;
- missing/duplicate Guardian LSA;
- incident qualification directly executing command;
- crisis state directly executing broker action;
- recovery branch inventing target business outcome;
- Guardian direct Foundation resource request;
- Guardian MSA entering Trading/FSAPMA private state;
- Shared Application silently relabeled as trading-specific;
- any MSA count other than one per Application;
- any FSATS-container MSA;
- LSA topology used as permission source.

---

## 26. Exit Gates

```text
FSAPMA_MSA = EXACTLY_1
FSAPMA_LSAS = EXACTLY_6
FSAPMA_BRANCH_OWNERSHIP = COMPLETE
GUARDIAN_MSA = EXACTLY_1
GUARDIAN_LSAS = EXACTLY_4
GUARDIAN_BRANCH_OWNERSHIP = COMPLETE
FSTSIMA_MSA = EXACTLY_1
FSTSIMA_LSAS = EXACTLY_8
TRADING_MSA = EXACTLY_1
TRADING_LSAS = EXACTLY_13
FSATS_CONTAINER_MSA = 0
OPERATIONAL_CONTROLLER_REPLACEMENT = 0
CROSS_APPLICATION_AWARENESS_TAKEOVER = 0
TOPOLOGY_PERMISSION_INFERENCE = 0
FRESH_ARCHITECTURE_REVIEW = REQUIRED
FRESH_RED_TEAM_REVIEW = REQUIRED
FINAL_OWNER_CLOSURE = REQUIRED
```

---

## 27. Freeze Effect

All semantic freeze/review records created before this file are stale for the affected final-candidate scope.

A new semantic freeze SHALL be created only after the final topology-completeness audit confirms no further semantic remediation is required.
