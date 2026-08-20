# Falcon Self-Aware Trading System (FSATS) — Developer & Engineer Manual

**Edition:** 2026-08-19  
**Language:** English  
**Audience:** software engineers, architects, maintainers, reviewers, test engineers, integration engineers, operators, and AI coding agents working on FSATS  
**Branch:** `application-development`  
**Writable scope for ordinary FSATS Application work:** `applications/**` only  
**Current system posture:** Parts 0 through 10 are Owner accepted and closed. Part 11 onboarding preparation is implemented and technically verified. Foundation reports `FULL_PLUG_READY_PREFLIGHT = VERIFIED_BY_COMPOSITION`, while actual Admission, canonical Runtime Registration, Activation, provider/broker connectivity, Paper, Shadow, Tiny-Live, Live, deployment, and business authority remain separately governed and not authorized by preparation evidence.

> This manual is an engineering companion for FSATS. It does not replace Falcon Vision, Falcon Constitution, `applications/FSATS/WORKSTREAM_RULES.md`, APP-001, CON-023, ADR-I012, ADR-I015, FDN-006, FDN-007, current Owner decisions, current FCR headers, accepted Part records, source code, tests, or exact executable evidence. Higher governing sources prevail.

---

# 1. Prime engineering rule

All FSATS work follows:

```text
SOURCE
-> AUTHORITY
-> COMPARE
-> DECIDE
-> CHANGE
```

Never begin from remembered state or previous chat context when current repository evidence is available.

Before every substantive FSATS response or work cycle:

1. perform a fresh broad FCR check;
2. inspect any canonical `Waiting On: APPLICATION` issue body and relevant latest comments;
3. fetch fresh `application-development` HEAD;
4. read the current workstream rules and directly governing sources for the scope;
5. establish exact authority before implementation.

---

# 2. Repository and ownership boundaries

Ordinary FSATS Application writes are limited to:

```text
application-development
applications/**
```

Do not write to:

```text
foundation-development
web-development
main
reference/fsats-v1.3-scratch
applications/shared/web/**
applications/FSATS/WORKSTREAM_RULES.md
```

unless the Project Owner separately grants explicit authority.

Foundation-owned responsibilities are read-only from the Application workstream. Shared Web is independently owned by the Web workstream.

```text
APPLICATION MUST NOT PATCH FOUNDATION TO FIT FSATS
ORDINARY APPLICATION MUST NOT PATCH SHARED WEB
```

---

# 3. Current FSATS architecture

FSATS is a non-owning, non-runtime system boundary composed of five independent Falcon Applications:

```text
FSATS-TRADING
FSATS-FSAPMA
FSATS-TRADING-GUARDIAN
FSATS-FSTSIMA
APP-RSC
```

Current awareness topology:

```text
Trading:          MSA=1 / LSA=13 / CSA=3
FSAPMA:           MSA=1 / LSA=6  / CSA=1
Trading Guardian: MSA=1 / LSA=4  / CSA=1
FSTSimA:          MSA=1 / LSA=8  / CSA=2
APP-RSC:          MSA=1 / LSA=3  / CSA=0 initially
TOTAL: 5 Applications / 5 MSA / 34 LSA / 7 CSA
```

The architectural awareness boundary is:

```text
CSA -> LSA -> MSA -> FSA review where applicable
```

FSA remains Foundation-owned. MSA/LSA/CSA remain Application-owned.

---

# 4. Responsibilities by Application

## 4.1 Trading

Owns trading-domain intelligence and broker-account-scoped business workflow. Typical responsibility families include:

- market interpretation;
- strategy selection and orchestration;
- trading opportunity evaluation;
- portfolio/trading-domain decision logic within accepted scope;
- broker-account-scoped execution preparation;
- trading evidence and state;
- trading-domain recovery/reconciliation semantics.

Trading does not own Foundation admission, Foundation activation, Foundation resource governance, Foundation FSA, or Shared Web identity mapping.

## 4.2 FSAPMA

Owns FSATS operational provider management:

- provider capabilities;
- provider selection/suitability;
- quota/rate-limit awareness;
- route readiness;
- provider failure/degradation behavior;
- operational market-data provider coordination.

Provider connectivity does not create trading authority.

## 4.3 Trading Guardian

Owns bounded trading-domain protection and containment semantics. It does not own Foundation Guardian, global AI Kill, broker authority, or strategy logic.

## 4.4 FSTSimA

Owns governed non-Live simulation and Digital City validation, including deterministic scenario execution, replay, calibration, fault injection, evidence and qualification outputs.

Simulation output is not operational truth and does not grant Paper/Live authority.

## 4.5 APP-RSC

Owns FSATS-side resource coordination only. It must preserve Foundation as authoritative resource governance and current grant source.

```text
APP-RSC != FOUNDATION_RESOURCE_GOVERNANCE
```

---

# 5. Broker-account identity model

The controlling trading operating subject is the broker account.

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
TRADING_OPERATING_SUBJECT = BROKER_ACCOUNT
BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = ADDITIONAL DIMENSION WHERE MATERIAL
```

Shared Web owns broker-account-to-customer/user/contact mapping.

Never introduce an FSATS-owned customer identity shortcut that conflicts with this model.

---

# 6. Governing authority separations

These distinctions are architectural invariants, not style preferences:

```text
VALID_CONTRACT != ADMISSION
ADMISSION != ACTIVATION
ADMISSION != DEPLOYMENT_AUTHORITY
ADMISSION != BUSINESS_AUTHORITY
TECHNICAL_CONSUMPTION != RUNTIME_AUTHORITY
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != PRODUCTION_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
RESTART != RECOVERY
ROUTE_EXISTS != CONNECTION_AUTHORIZED
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
PROVIDER_CONNECTIVITY != EXECUTION_AUTHORITY
DATA_ACCESS != BUSINESS_AUTHORITY
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
SELF_AWARENESS != AUTHORITY
```

Any design or code path that collapses one of these distinctions requires immediate review.

---

# 7. Current Part status

The accepted documentary state is:

```text
PART 0  = OWNER_ACCEPTED_AND_CLOSED
PART 1  = OWNER_ACCEPTED_AND_CLOSED
PART 2  = OWNER_ACCEPTED_AND_CLOSED
PART 3  = OWNER_ACCEPTED_AND_CLOSED
PART 4  = OWNER_ACCEPTED_AND_CLOSED
PART 5  = OWNER_ACCEPTED_AND_CLOSED
PART 6  = OWNER_ACCEPTED_AND_CLOSED
PART 7  = OWNER_ACCEPTED_AND_CLOSED
PART 8  = OWNER_ACCEPTED_AND_CLOSED
PART 9  = OWNER_ACCEPTED_AND_CLOSED
PART 10 = OWNER_ACCEPTED_AND_CLOSED
```

Part 11 is a separately Owner-authorized Runtime Onboarding / Admission & Binding preparation scope. Its Application-side implementation and exact request materialization are technically verified, but the Part is not to be represented as Owner-accepted-and-closed unless the Owner explicitly grants that state.

---

# 8. Part 11 onboarding architecture

The intended generic path is:

```text
APPLICATION DECLARATION
-> FOUNDATION VALIDATION
-> ADMISSION DECISION
-> CANONICAL ARTIFACT / LIFECYCLE / RESOURCE BINDING
-> RUNTIME REGISTRATION
-> SEPARATE ACTIVATION AUTHORITY
-> ACTIVE ONLY WHEN ALL APPLICABLE GATES PASS
```

Each of the five Applications has an Application-owned Foundation onboarding declaration bound to its current Manifest identity and awareness topology.

Required admission references:

```text
CON-023 = 1.1
APP-001 = 1.0
BootstrapContextState = DEFINED
```

Each declaration requires:

```text
ExactArtifactIdentityRequired = true
PositiveAdmissionEvidenceRequired = true
LifecycleAttachEligibilityRequired = true
CurrentFoundationResourceGrantRequired = true
RuntimeRegistrationMayAuthorizeActivation = false
RuntimeRegistrationMayAuthorizeDeployment = false
RuntimeRegistrationMayAuthorizeProduction = false
RuntimeRegistrationMayGrantBusinessAuthority = false
SilentUpgradeAllowed = false
ExternalConnectivityActivated = false
PaperAuthorityGranted = false
LiveAuthorityGranted = false
```

---

# 9. FCR-0254 exact request materialization

The Application workstream materializes exactly five preparation pairs:

```text
5 AdmissionRequest candidates
5 RuntimeRegistrationRequest templates
5 request pairs
```

Primary implementation paths:

```text
applications/FSATS/tests/FoundationCompatibility/
  Falcon.FSATS.FoundationOnboarding.Verifier/
    Fcr0254CandidateCatalog.cs
    Fcr0254CandidateCatalogVerifier.cs
```

The five Application onboarding declarations live under their respective Application projects.

The preparation package intentionally leaves runtime-current facts unbound until an authorized operation.

Bind-at-operation inputs include:

```text
EXACT_STAGE14_ARTIFACT_IDENTITY
POSITIVE_CANONICAL_ADMISSION_EVIDENCE
LIFECYCLE_ATTACH_ELIGIBILITY_AND_DECISION_IDENTITY
CURRENT_FOUNDATION_RESOURCE_GRANTS
AUTHORITATIVE_OBSERVED_AT
```

Never fabricate these values for convenience.

---

# 10. Current Foundation handoff state

Foundation has confirmed that the generic admission/runtime-hosting capability exists and that the current preparation package is plug-ready by composition.

Current preparation verdict:

```text
FOUNDATION_GENERIC_ADMISSION_RUNTIME_PATH   = EXECUTABLE_PROVEN
APPLICATION_EXACT_REQUEST_MATERIALIZATION   = EXECUTABLE_VERIFIED
FOUNDATION_EXACT_STATIC_GATE_RECONCILIATION = PASS_5_OF_5
FULL_PLUG_READY_CONTRACT_PREFLIGHT          = VERIFIED
FULL_PLUG_READY_PREFLIGHT                   = VERIFIED_BY_COMPOSITION
FOUNDATION_CHANGE_REQUIRED                  = FALSE
APPLICATION_REDESIGN_REQUIRED               = FALSE
```

But actual operation remains held:

```text
ACTUAL_ADMISSION                      = NOT_AUTHORIZED / NOT_EXECUTED
ACTUAL_CANONICAL_RUNTIME_REGISTRATION = NOT_AUTHORIZED / NOT_EXECUTED
RUNTIME_ACTIVATION                    = NOT_AUTHORIZED / NOT_EXECUTED
DEPLOYMENT                            = NOT_AUTHORIZED / NOT_EXECUTED
PROVIDER_BROKER_CONNECTIVITY          = NOT_AUTHORIZED / NOT_EXECUTED
PAPER_LIVE_BUSINESS_AUTHORITY         = NOT_AUTHORIZED / NOT_EXECUTED
```

Do not convert readiness into runtime state in documentation, source, tests, or status messages.

---

# 11. Host egress boundary

Host projects must preserve disabled egress until separately authorized and genuinely bound.

Typical current design principle:

```text
Trading Host -> DisabledBrokerExecutionPort until governed broker execution binding
FSAPMA Host  -> DisabledProviderEgressPort until governed provider egress binding
APP-RSC Host -> DisabledFoundationResourcePort until governed Foundation resource binding
```

Do not replace disabled ports with live network implementations merely because provider/broker code exists.

```text
TECHNICAL_COMPATIBILITY != RUNTIME_BINDING_AUTHORITY
```

---

# 12. Foundation integration rule

Foundation is Application-neutral and sealed around published contracts.

Permanent rule:

```text
APPLICATION MUST FIT FOUNDATION
FOUNDATION MUST NOT CHANGE TO FIT APPLICATION
```

When FSATS cannot satisfy a published Foundation contract:

1. verify the contract and current source;
2. determine whether the issue is Application-side adaptation;
3. use an Application-side adapter only if it preserves Foundation semantics;
4. redesign/remove unsupported Application behavior if required;
5. do not introduce a local fake Foundation service;
6. do not patch Foundation from `application-development`.

---

# 13. Manifest discipline

Each Application Manifest is an authority-critical declaration, not a README.

Preserve exact alignment of:

- Application identity;
- version;
- owner;
- package identity/version;
- Manifest identity;
- provenance;
- dependencies;
- contracts/specifications/services;
- permissions;
- authority requests;
- provider boundary;
- resources;
- lifecycle;
- awareness topology.

A Manifest change can invalidate prior admission/readiness evidence.

```text
SEMANTIC_MANIFEST_CHANGE -> FRESH REVIEW REQUIRED
```

---

# 14. Awareness engineering rules

MSA, LSA, and CSA are bounded to their rooms.

Preserve:

```text
ONE MSA PER CURRENT MAJOR APPLICATION
LSA OWNERSHIP = MAJOR BRANCH
CSA = ELIGIBLE INTELLIGENT COMPONENT ONLY
FSA = FOUNDATION ONLY
```

Do not let an MSA become a second Foundation control plane.

Do not let CSA self-approve authority expansion.

Self-improvement proposals remain subject to sandbox/evidence/governance and Owner authority where required.

---

# 15. Strategy architecture

Strategies are centrally governed within the Trading Application rather than duplicated per market.

The central strategy model uses a Strategy Controller and strategy self-awareness to select/configure strategies against market-specific properties and constraints.

Market models describe market-specific facts such as:

- required data;
- indicators;
- timeframes;
- liquidity rules;
- execution constraints;
- market restrictions;
- strategy suitability characteristics.

Do not duplicate the full strategy implementation once per market merely to express market compatibility.

---

# 16. Provider architecture

FSAPMA is the operational FSATS data gateway. Provider-specific logic belongs behind FSAPMA boundaries.

Preserve:

```text
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
PUBLIC_PROVIDER_ENDPOINT != UNRESTRICTED_EGRESS_AUTHORITY
```

Provider controller logic may reason about capabilities, quota and selection, but must not create authority to connect or trade.

---

# 17. Simulation architecture

FSTSimA must remain non-Live unless separately authorized.

Preserve deterministic behavior where required:

- exact scenario identity;
- exact seed;
- exact fault ordering;
- exact evidence/digest binding;
- reproducibility assessment;
- calibration gates;
- explicit non-operational classification.

Do not feed simulation output into live truth without a separately governed contract that explicitly permits the transformation.

---

# 18. Resource architecture

APP-RSC coordinates FSATS resource demand but must consume Foundation resource truth rather than invent it.

Core invariants:

```text
REQUESTED_RESOURCE != GRANTED_RESOURCE
RESOURCE_PROJECTION != RESOURCE_AUTHORITY
0 <= Allocation <= Quota <= Ceiling
```

A grant for one Application must not be silently reused by another Application.

Current authoritative runtime resource evidence must be bound from Foundation at the authorized operation instant.

---

# 19. Security and secret handling

Do not store secret bytes in ordinary Application state.

Use governed credential references.

Preserve:

```text
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
AUTHENTICATION != AUTHORIZATION
```

Security lexical scanning is defense-in-depth, not proof that all possible egress is absent. Runtime route governance, architecture fences, dependency review and integration verification remain required.

---

# 20. Failure and recovery

Failure handling must preserve bounded, deterministic, explainable state.

Never shortcut:

```text
RESTART => RECOVERED
REPAIR_SUCCESS => RELEASED
```

Recovery and release are separate governed concepts.

Expected conceptual chain:

```text
fault/containment
-> assessment
-> governed recovery plan
-> restoration
-> reconciliation
-> validation
-> ready-for-release-decision
-> separate release authorization
-> separate release execution
-> observation
```

---

# 21. Update discipline

A material semantic update invalidates prior PASS for the changed scope.

Required review lifecycle:

```text
Semantic Change
-> Fresh Architecture / Consistency Review
-> Fresh Red Team
-> Owner Review
```

If Red Team remediation changes semantics again, repeat the cycle.

Do not present an old PASS as current evidence after changed bytes/semantics.

---

# 22. Validation model

The governed Application validation suite currently includes ten verifier projects in the Part 11 validation path, covering:

- Architecture;
- Security;
- Behavior;
- Operational Data Outcome;
- Owner Update Governance;
- Foundation Binding;
- Owner Feature Entitlement;
- Foundation Onboarding / FCR-0254 materialization;
- Integration;
- Failure.

Current exact Part 11 evidence established, on the tested candidate:

```text
Architecture = PASS
Security = PASS
Behavior = PASS 40/40
Operational Data Outcome = PASS 16/16
Owner Update Governance = PASS 44/44
Foundation Binding = PASS 67/67
Owner Feature Entitlement = PASS 44/44
FCR-0254 Materialization = PASS 129/129
Foundation Onboarding = PASS 27/27
Integration = PASS 31/31
Failure = PASS 12/12
Application verifiers = PASS 10/10 twice
Cross-branch onboarding = PASS 20/20
FAILED_CHECKS = 0
```

Technical PASS still does not create Owner acceptance or runtime authority.

---

# 23. Red-Team checklist

For every relevant FSATS semantic change ask:

```text
Can missing identity pass?
Can version/digest/provenance substitution pass?
Can stale evidence become current?
Can unknown become authorized?
Can one broker account inherit another account's state?
Can one Application reuse another Application's resource grant?
Can registration become activation?
Can a provider route become execution authority?
Can Web presentation data become FSATS operational truth?
Can simulation become live truth?
Can Guardian protection become strategy authority?
Can APP-RSC become Foundation resource authority?
Can MSA/LSA/CSA cross into FSA scope?
Can secret bytes enter ordinary state?
Can a component approve its own authority expansion?
Can a restart skip recovery?
Can a successful test become Owner approval?
```

Forbidden paths must remain impossible or fail closed.

---

# 24. FCR protocol for engineers

The GitHub Issue body is the canonical current state for each FCR. Comments are audit history.

Permitted `Waiting On` values:

```text
FOUNDATION
APPLICATION
WEB
NONE
```

`Waiting On: OWNER` is prohibited.

If an FCR says `Waiting On: APPLICATION`, inspect the current body and relevant latest comments before other dependent work.

Do not close Foundation-owned implementation claims from Application without Foundation evidence.

FCR status never creates runtime authority.

---

# 25. Documentation discipline

Historical records are not rewritten merely to make the repository look clean.

Use:

```text
Historical Record
+ Later Controlling Correction / Amendment / Supersession
```

for corrected semantics where history must be preserved.

A documentation-only commit must not be represented as if the executable suite ran on that later commit unless it actually did.

---

# 26. Practical change checklist

Before writing:

- fresh FCR check;
- fresh HEAD;
- read current governing sources;
- identify exact scope and authority;
- identify owning Application;
- verify Foundation dependency if applicable;
- inspect latest relevant Architecture/Red-Team evidence.

During implementation:

- stay within owning paths;
- preserve identity/authority boundaries;
- add/update tests for semantic behavior;
- fail closed on unknown authority-critical state;
- avoid hidden cross-Application state ownership.

After implementation:

- inspect diff;
- restore/build/test;
- run applicable governed verifiers;
- rerun deterministic checks where required;
- verify clean working tree;
- perform fresh Architecture/Consistency review;
- perform fresh Red Team;
- report exact tested commit and evidence;
- do not overclaim acceptance, activation, deployment, or Live authority.

---

# 27. Where to start in the repository

Primary Application navigation:

```text
applications/README.md
applications/FSATS/README.md
applications/FSATS/WORKSTREAM_RULES.md
applications/docs/FSATS/
applications/FSATS/src/
applications/FSATS/tests/
applications/ci/
```

Current Part 11 control document:

```text
applications/docs/FSATS/PART_11/00_PART11_RUNTIME_ONBOARDING_AUTHORIZATION_AND_SCOPE.md
```

Current request materialization verifier:

```text
applications/FSATS/tests/FoundationCompatibility/
Falcon.FSATS.FoundationOnboarding.Verifier/
```

---

# 28. Final engineering rule

When deciding where a change belongs, ask:

```text
Who owns truth?
Who owns authority?
Who owns execution?
Who owns presentation?
Which exact identity is affected?
What evidence crosses the boundary?
What must remain impossible?
Does this preserve all five Application boundaries?
Does this preserve Foundation neutrality?
Does this preserve fail-closed behavior?
```

If those answers are not explicit, the design is not ready to implement.

**Architecture first. Exact identity. Explicit authority. Evidence-bound decisions. Fail closed. No hidden authority transfer.**