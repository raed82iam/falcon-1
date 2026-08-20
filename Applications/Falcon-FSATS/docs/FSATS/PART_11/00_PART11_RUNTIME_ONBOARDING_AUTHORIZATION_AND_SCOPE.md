# FSATS Part 11 — Runtime Onboarding / Admission & Binding

**Status:** `OWNER_AUTHORIZED_IMPLEMENTATION / FCR0254_EXACT_REQUEST_MATERIALIZATION_COMPLETE / CANDIDATE_NOT_OWNER_ACCEPTED`  
**Owner Authorization:** Project Owner direction on 2026-08-19: `ابدأ وخلصها كامله` following the explicit proposal to begin the bounded Runtime Onboarding / Admission & Binding phase, followed by explicit direction to complete the Foundation-requested FCR-0254 Application preparation to the end.  
**Writable Branch:** `application-development`  
**Writable Scope:** `applications/**` only.  

## 1. Purpose

Part 11 prepares the five current FSATS Applications for the generic sealed-Foundation Application admission and runtime-registration path defined by APP-001, CON-023, FDN-006 and FDN-007.

This Part is an Application-owned onboarding implementation and verification scope. It does not redesign Foundation and does not create an Application-side substitute for Foundation Admission, Foundation Runtime Hosting, Foundation FSA, Foundation Resource Governance or Foundation authority.

## 2. Exact Application Set

```text
FSATS-TRADING
FSATS-FSAPMA
FSATS-TRADING-GUARDIAN
FSATS-FSTSIMA
APP-RSC
```

No sixth Application is introduced by Part 11.

## 3. Governing Lifecycle Separation

Part 11 preserves:

```text
MANIFEST DECLARATION
  -> FOUNDATION VALIDATION
  -> ADMISSION DECISION
  -> CANONICAL ARTIFACT / LIFECYCLE / RESOURCE BINDING
  -> RUNTIME REGISTRATION
  -> SEPARATE ACTIVATION AUTHORITY
  -> ACTIVE ONLY AFTER SEPARATE AUTHORITY
```

Mandatory distinctions remain:

```text
VALID_CONTRACT != ADMISSION
ADMISSION != ACTIVATION
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != PRODUCTION_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
TECHNICAL_CONSUMPTION != RUNTIME_AUTHORITY
ROUTE_EXISTS != CONNECTION_AUTHORIZED
CONNECTIVITY_READY != CONNECTIVITY_ACTIVATED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
```

## 4. Authorized Application-Owned Work

Part 11 is authorized to:

- materialize an Application-owned onboarding declaration for each of the five Applications;
- bind each declaration to the current Application identity/version/owner/package and MSA/LSA/CSA topology;
- declare CON-023 v1.1 and APP-001 v1.0 as mandatory admission references;
- require the Foundation-defined `DEFINED` bootstrap context state;
- require exact artifact identity, positive admission evidence, lifecycle attach eligibility and a current Foundation resource grant before runtime registration;
- preserve explicit provider/broker/protection/resource boundaries without activating them;
- add governed executable verification that all five Application declarations remain fail closed;
- materialize one exact `AdmissionRequest` candidate and one corresponding `RuntimeRegistrationRequest` preparation template for each of the five Applications under FCR-0254;
- keep runtime-current artifact, admission, lifecycle, resource-grant and observation-time facts as explicit bind-at-execution inputs from their authoritative Foundation sources rather than fabricating them;
- prepare the exact cross-workstream handoff for Foundation-owned non-mutating admission preflight and isolated throwaway-host registration preflight.

## 5. Explicitly Not Authorized

Part 11 does **not** authorize:

- Foundation code changes;
- Shared Web changes;
- Foundation Admission decisions by the Application workstream;
- Foundation Runtime Host registration decisions fabricated by the Application workstream;
- actual canonical admission execution;
- actual canonical runtime registration;
- runtime activation;
- deployment or production adoption;
- provider or broker connection execution;
- Paper, Shadow, Tiny-Live or Live trading;
- secret-byte injection;
- business/trading authority;
- silent upgrade;
- self-granted MSA/FSA or resource authority.

## 6. Application-Side Implementation Contract

Each Application onboarding declaration SHALL require:

```text
AdmissionKind = APPLICATION
RequiredApplicationContract = CON-023 / 1.1
RequiredApplicationSpecification = APP-001 / 1.0
BootstrapContextState = DEFINED
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

The declaration SHALL remain identity-bound to its current Application Manifest and awareness topology.

## 7. FCR-0254 Exact Request Materialization

Foundation disposition on FCR-0254 established that the sealed generic admission/runtime-hosting path already exists and returned the immediate preparation action to Application.

Application materializes exactly five request pairs in:

`applications/FSATS/tests/FoundationCompatibility/Falcon.FSATS.FoundationOnboarding.Verifier/Fcr0254CandidateCatalog.cs`

The governed verifier enforcement is in:

`applications/FSATS/tests/FoundationCompatibility/Falcon.FSATS.FoundationOnboarding.Verifier/Fcr0254CandidateCatalogVerifier.cs`

Each pair contains:

1. one concrete Application-owned `AdmissionRequest` candidate whose field names and nested Manifest shape match the published Foundation contract; and
2. one corresponding `RuntimeRegistrationRequest` preparation template preserving exact Application identity/version/capabilities while leaving runtime-current facts as authoritative bind-at-execution inputs.

Exact pair identities are:

```text
admission-candidate:fcr0254:fsats-trading
runtime-candidate:fcr0254:fsats-trading

admission-candidate:fcr0254:fsats-fsapma
runtime-candidate:fcr0254:fsats-fsapma

admission-candidate:fcr0254:fsats-trading-guardian
runtime-candidate:fcr0254:fsats-trading-guardian

admission-candidate:fcr0254:fsats-fstsima
runtime-candidate:fcr0254:fsats-fstsima

admission-candidate:fcr0254:app-rsc
runtime-candidate:fcr0254:app-rsc
```

The following runtime-registration inputs are intentionally **not** fabricated by Application and remain bind-at-execution only:

```text
ExpectedArtifactExactIdentity / accepted technical-consumption evidence
positive Admission binding and Admission evidence identity
current Lifecycle Attach eligibility and decision identity
current Foundation Resource Grant set and evidence timestamps
ObservedAt authoritative execution time
```

Candidate preparation explicitly preserves:

```text
ACTUAL_ADMISSION = NOT_EXECUTED
ACTUAL_RUNTIME_REGISTRATION = NOT_EXECUTED
RUNTIME_ACTIVATION = NOT_AUTHORIZED
DEPLOYMENT = NOT_AUTHORIZED
PRODUCTION = NOT_AUTHORIZED
BUSINESS_AUTHORITY = NOT_GRANTED
PROVIDER_BROKER_CONNECTIVITY = NOT_AUTHORIZED
PAPER_LIVE = NOT_AUTHORIZED
```

## 8. Verification Gate

Technical completion requires all of the following against the exact final Application HEAD:

```text
APPLICATION RESTORE = PASS
APPLICATION RELEASE BUILD = PASS
APPLICATION DOTNET TEST = PASS
GOVERNED APPLICATION VERIFIERS = PASS, including Foundation Onboarding and FCR-0254 materialization checks
FRESH ARCHITECTURE / CONSISTENCY REVIEW = PASS
FRESH RED TEAM = PASS
TRACKED WORKING TREE = CLEAN
```

The Foundation Onboarding verifier SHALL validate all five declarations and SHALL explicitly prove that runtime registration cannot smuggle activation, deployment, production, business, Paper, Live or external-connectivity authority.

The FCR-0254 materialization verifier SHALL fail if the request-pair count is not exactly five, if identity/version/contract bindings diverge, if Manifest digests are not deterministic, or if runtime-current authority evidence is replaced by fabricated values.

## 9. Cross-Workstream Completion Boundary

Application-side technical completion does not manufacture a Foundation admission or runtime-registration result.

After the exact Application candidate passes its governed verification, the Application workstream SHALL hand the exact request-pair catalog and evidence to Foundation through FCR-0254.

Foundation remains responsible for its own exact-data non-mutating admission preflight, isolated throwaway-host runtime-registration preflight, any later actual admission decision, canonical artifact/lifecycle/resource binding, runtime registration and later runtime authority operation.

## 10. Owner Acceptance Boundary

A technical PASS for Part 11 is not final Project Owner acceptance or closure.

```text
TECHNICAL_PASS != OWNER_ACCEPTANCE
APPLICATION_SIDE_READY != FOUNDATION_ADMITTED
FOUNDATION_ADMITTED != RUNTIME_REGISTERED
RUNTIME_REGISTERED != ACTIVE
FULL_PLUG_READY_PREFLIGHT != ACTUAL_LINK
```

Part 11 may be presented to the Project Owner for final acceptance only after the exact semantic candidate has current executable, Architecture/Consistency and Red-Team evidence.
