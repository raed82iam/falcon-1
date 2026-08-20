# Falcon Foundation Developer Deep Reference

**Edition:** 2026-08-19  
**Governed state:** `Stage 0A through Stage 16 = ACCEPTED_AND_CLOSED`  
**Validated executable baseline:** `889a52ddcf492ecfa4f69c3f940d56362163f04f`  
**Validation state:** `FULL FOUNDATION VALIDATION = PASS`; 88/88 governed verifiers PASS; Unknown Application verifier 42/42 PASS.  
**Current integration references:** FDN-006 and FDN-007.  
**Audience:** developers, maintainers, architects, reviewers, and AI coding agents working directly with Foundation source.

> This reference is a code-navigation and execution-flow companion to the Programming Manual. It describes current production boundaries and the final generic Application-hosting model. It does not create change authority and does not replace Vision, Constitution, canonical Owner records, accepted contracts/ADRs, FDN-006, FDN-007, or accepted executable source/evidence.

---

# 1. How to use this reference

Recommended reading order:

1. `FALCON_FOUNDATION_HUMAN_READER_MANUALEN.md` or Arabic equivalent.
2. `FALCON_FOUNDATION_PROGRAMMING_MANUAL_EN.md` or Arabic equivalent.
3. `FDN-006_APPLICATION_INTEGRATION_AND_ADMISSION_PROFILE.md`.
4. `FDN-007_LIVE_FOUNDATION_SEAL_AND_FUTURE_APPLICATION_ONBOARDING_POLICY.md`.
5. This file while navigating `src/Foundation.*`, `tests/**` and `verification/**`.

Before any executable Foundation change that is actually authorized, read the exact governing source, current HEAD, relevant contracts/ADRs, FCR state, verifier, and accepted evidence.

After formal Live Seal, ordinary new-Application onboarding is **not** authority to modify Foundation.

---

# 2. Mental model of the runtime

Foundation is a set of bounded technical authorities and neutral platform services.

Think of a request as independent questions:

```text
Who/what is this?
    -> Is its declaration exact and attributable?
    -> Are its contracts/dependencies valid?
    -> Is admission allowed?
    -> Is lifecycle Attach eligible?
    -> Are resource grants current and bounded?
    -> Are capabilities valid and isolated?
    -> Is runtime registration allowed?
    -> Is separate activation authority present?
    -> Is external route authority present?
    -> Is business authority separately present?
```

No successful answer silently answers the next one.

Core invariants:

```text
VALID_CONTRACT != ADMISSION
ADMISSION != ACTIVATION
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
AUTHENTICATION != AUTHORIZATION
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
PUBLISHED != ACTIVATED
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
TECHNICAL_SUCCESS != BUSINESS_AUTHORITY
```

---

# 3. Source tree navigation

Primary production areas include:

```text
src/Foundation.Core/
src/Foundation.Contracts/
src/Foundation.ContractRegistry/
src/Foundation.ApplicationManifest/
src/Foundation.Admission/
src/Foundation.DependencyGovernance/
src/Foundation.ApplicationLifecycle/
src/Foundation.EventSystem/
src/Foundation.Evidence/
src/Foundation.Authority/
src/Foundation.Guardian/
src/Foundation.HealthFitness/
src/Foundation.SelfAwareness/
src/Foundation.ArtifactPublication/
src/Foundation.ApplicationRuntimeHosting/
src/Foundation.IdentityRuntime/
```

Controlled solution entry point:

```text
Falcon.Foundation.ControlledProjectFoundation.slnx
```

The controlled solution includes the Unknown Application verifier so the generic hosting proof is part of governed validation and not an orphan experiment.

---

# 4. Foundation.Contracts and FIL

`Foundation.Contracts` contains neutral cross-component contracts and canonical messaging primitives such as the FIL envelope family.

A governed envelope binds concepts such as:

```text
message kind/classification
schema identity
producer/recipient
correlation/causation
provenance
authority reference
idempotency/delivery/retry lineage
payload SHA-256
validity window
```

Developer rule:

```text
MESSAGE_PARSED != MESSAGE_AUTHORIZED
MESSAGE_ACCEPTED != BUSINESS_ACTION_AUTHORIZED
```

Do not reconstruct a private partial envelope when a governed FIL contract is required.

---

# 5. Contract registry and exact compatibility

`Foundation.ContractRegistry` resolves governed contract/schema identity and compatibility.

Preserve:

- exact identity;
- explicit versions/compatibility;
- duplicate/ambiguous registration rejection;
- no guessing of missing contract/schema;
- compatibility does not imply activation.

---

# 6. Application Manifest and Admission

`Foundation.ApplicationManifest` represents the declared technical shape of an Application without importing its business semantics.

`Foundation.Admission` validates whether an `APPLICATION` or supported `PLUG-IN` is admissible.

Current generic admission behavior validates, as applicable:

- nonblank exact subject identity/version/owner;
- Manifest identity/version/owner exact binding;
- Manifest digest;
- provenance identity/content/digest;
- defined bootstrap context;
- provider boundary without bypass;
- canonical governing contract/authority linkage;
- declared dependencies and compatible versions;
- required Foundation contracts/specifications/services;
- permissions and authority requests;
- deterministic decision/evidence inputs;
- duplicate admission prevention.

Critical invariants:

```text
ADMITTED != ACTIVATED
ADMITTED != DEPLOYED
ADMITTED != BUSINESS_AUTHORITY_GRANTED
AUTHORITY_REQUEST != AUTHORITY_GRANT
```

The admission path has no business-domain Application-name or Application-version allowlist.

---

# 7. Dependency governance

`Foundation.DependencyGovernance` validates graph identity/version, declared dependencies, exact resolution, availability, delegation/authority chains, activation-order metadata and graph integrity.

Representative fail-closed conditions include:

```text
MISSING_GRAPH_VERSION
MISSING_DECLARED_DEPENDENCY
DEPENDENCY_DECLARATION_MISMATCH
DELEGATION_CHAIN_MISMATCH
DEPENDENCY_UNAVAILABLE
INCOMPLETE_ACTIVATION_ORDER
```

Dependency availability is not activation authority.

---

# 8. Lifecycle

`Foundation.ApplicationLifecycle` owns legal technical lifecycle decisions.

```text
current state
    + requested transition
    + required evidence/conditions
    -> legal transition OR deterministic rejection
```

Preserve:

```text
LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION
RESTART != RECOVERY
```

---

# 9. Evidence

`Foundation.Evidence` provides attributable evidence identity for governed decisions.

Evidence should make clear:

- what fact is supported;
- exact subject/version/state;
- producer/source;
- validity/freshness;
- provenance/digest.

```text
EVIDENCE_PRESENT != AUTHORITY_GRANTED
```

Stale, substituted, incomplete or mismatched evidence must fail where exact binding is required.

---

# 10. Authority and external access

`Foundation.Authority` contains bounded authority evaluation and independent enforcement surfaces including external access, AI Kill, emergency/protective enforcement and recovery authority.

For external access, bind exact:

```text
principal/application identity
service role
environment
purpose
destination
authentication context
credential reference
policy/authority/evidence
```

Preserve:

```text
RESEARCH_EGRESS != OPERATIONAL_PROVIDER_EGRESS
OPERATIONAL_PROVIDER_EGRESS != BROKER_EXECUTION_EGRESS
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
PROVIDER_CONNECTIVITY != BUSINESS_AUTHORITY
```

Foundation evaluates route authority. It does not become the provider SDK, broker executor, or Application business authority.

---

# 11. AI Kill, Guardian and Safe Core

`AiKillControlPlane` is an independent Foundation authority boundary. FSA/MSA/LSA/CSA do not own their own unrestricted Kill authority.

```text
APPLICATION_AI != ITS_KILL_AUTHORITY
FSA != ITS_KILL_AUTHORITY
KILL_REQUEST != KILL_AUTHORIZATION != KILL_EXECUTION
GLOBAL_AI_KILL != FALCON_SHUTDOWN
```

`Foundation.Guardian` owns bounded protection/containment, not Application business logic, release authority, broker authority or portfolio strategy.

```text
PROTECTIVE_RESTRICTION != BUSINESS_AUTHORITY
CONTAINED != RELEASED
SAFE_STATE != NORMAL_OPERATION
```

---

# 12. Health and Foundation Self-Awareness

`Foundation.HealthFitness` and `Foundation.SelfAwareness` provide technical health, evidence awareness, self-model and technical fitness.

Important runtime areas include:

```text
EvidenceAwarenessRuntime.cs
FoundationSelfModelRuntime.cs
FsaGovernanceProfiles.cs
FsaGovernanceRuntime.cs
TechnicalFitnessRuntime.cs
```

Preserve:

```text
SELF_AWARENESS != AUTHORITY
FSA_REVIEW != PRODUCTION_ADOPTION
FSA_REVIEW != DEPLOYMENT_AUTHORITY
APPLICATION_BUSINESS_JUDGMENT = APPLICATION_OWNED
OWNER_SILENCE != OWNER_APPROVAL
```

FSA stays Foundation-level. MSA/LSA/CSA stay in Applications.

---

# 13. Recovery and independent release

The recovery path deliberately separates:

```text
fault/containment
    -> recovery case
    -> assessment
    -> governed plan
    -> restoration
    -> reconciliation
    -> validation
    -> ready-for-release-decision
    -> separate release authorization
    -> separate release execution
    -> reintroduction/observation
    -> complete
```

Never write shortcuts such as `repairPassed => Released` or `restart => Recovered`.

---

# 14. Canonical artifact publication

`Foundation.ArtifactPublication` publishes exact immutable runtime identities and validates consumption.

Consumption may bind:

```text
ArtifactId
ArtifactVersion
SHA-256
EvidenceReference
CompatibilityIdentity
ProducerIdentity
PublicationState
Provenance
PayloadIdentity
```

Preserve:

```text
MOVING_BRANCH_HEAD != RUNTIME_CONSUMPTION_IDENTITY
PUBLICATION != ACTIVATION
PUBLICATION != DEPLOYMENT
CONSUMPTION != AUTHORITY
REVOKED_ARTIFACT != CONSUMABLE
SUPERSEDED_ARTIFACT != SILENT_AUTO_UPGRADE
```

---

# 15. Public runtime projections

The generic projection transport uses components such as:

```text
PublicRuntimeProjectionRoute
PublicRuntimeProjectionBinding
PublicRuntimeProjectionTransport
PublicRuntimeProjectionTransportDecision
PublicRuntimeProjectionProfiles
```

Current production profiles support generic Application recipient scopes. Legacy named aliases may remain only as compatibility delegators to the generic core.

A projection route is not command authority:

```text
PUBLIC_PROJECTION != CONTROL_REQUEST
PROJECTION_PRESENT != SYSTEM_ACTION_AUTHORIZED
WEB_PRESENTATION != FOUNDATION_AUTHORITY
ROUTE_AVAILABLE != ROUTE_AUTHORIZED != CONNECTION_EXECUTED
```

---

# 16. Application runtime hosting

`Foundation.ApplicationRuntimeHosting` hosts Applications generically.

The real `ApplicationRuntimeHost.Register` path enforces, among other things:

- required nonblank registration fields;
- unique runtime instance;
- no second nonremoved hosted slot for the same Application identity;
- accepted technical artifact-consumption binding;
- artifact binding must not smuggle activation/deployment/production/business/silent-upgrade authority;
- exact positive Admission evidence for the same identity/version;
- eligible lifecycle Attach evidence;
- at least one current identity-bound resource grant;
- `0 <= Allocation <= Quota <= Ceiling`;
- valid capability declarations and exclusivity;
- no future-dated resource evidence relative to observation time.

Successful result is:

```text
RUNTIME_REGISTERED_NOT_ACTIVATED
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
```

---

# 17. Identity Runtime

`Foundation.IdentityRuntime` provides Falcon identity, authentication, session and MFA runtime semantics.

Preserve:

```text
AUTHENTICATION != AUTHORIZATION
OIDC_AUTHENTICATED != FALCON_IDENTITY
OIDC_AUTHENTICATED != PROJECT_OWNER
MFA_PASSED != BUSINESS_AUTHORITY
ROLE_FACT != AUTHORITY_DECISION
SESSION_ISSUED != BUSINESS_AUTHORITY
FOUNDATION_SECURITY_CONTEXT != WEB_SURFACE_GRANT
```

External authentication evidence becomes Falcon security context only through governed binding. Consumer surface/business authorization remains separate.

---

# 18. Unknown Application proof

The final generic extensibility verifier uses:

```text
UNKNOWN_APPLICATION_IDENTITY = unknown-application-proof-7f3c9a
APPLICATION_VERSION = 999.123.456-test
```

It proved:

1. generic projection routing to an unknown recipient;
2. canonical ContractRegistry/Manifest admission;
3. real `ApplicationRuntimeHost` registration of that same admitted Application;
4. arbitrary Application version without a Foundation version allowlist;
5. fail-closed negative cases.

Canonical outcome:

```text
FOUNDATION_UNKNOWN_APPLICATION_VERIFIER = PASS
CHECKS = 42/42
APPLICATION_NAME_ALLOWLIST = NOT_REQUIRED
APPLICATION_VERSION_ALLOWLIST = NOT_REQUIRED
MANIFEST_AND_FOUNDATION_CONTRACTS = REQUIRED
ADMISSION_TO_RUNTIME_HOSTING = PROVEN
TAMPERED_MANIFEST = FAIL_CLOSED
INVALID_FOUNDATION_REFERENCE = FAIL_CLOSED
PROVIDER_BOUNDARY_BYPASS = FAIL_CLOSED
TECHNICAL_CONSUMPTION != RUNTIME_AUTHORITY
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
```

This proof is the technical basis for Application-neutral future hosting.

---

# 19. FDN-006 implementation-facing rule

A consumer Application must present complete and exact identity, Manifest, provenance, dependencies, Foundation references, capabilities, permissions/authority requests, security profile, resources, lifecycle, health/failure behavior and awareness placement.

Foundation asks:

```text
Who are you?
What do you require?
What may you consume?
What resources do you have?
What evidence binds you?
What authority do you actually hold?
```

Foundation does not need to understand the consumer's internal business domain.

---

# 20. FDN-007 and post-Live-Seal development

Historical cross-workstream development used FCRs to coordinate real Foundation gaps while Foundation was still under construction.

After formal Live Seal, that model is not available for ordinary new-Application fit.

Do not use:

```text
New Application gap
    -> Foundation FCR
    -> new Foundation code
```

Use:

```text
New Application requirement
    -> existing Foundation contract?
    -> existing Shared Application capability?
    -> Application-side adapter/design?
    -> redesign/remove?
    -> otherwise INCOMPATIBLE_WITH_SEALED_FOUNDATION
```

Foundation remains unchanged.

Historical FCRs remain audit evidence. Pre-Live-Seal FCRs such as current consumer conformance checks are one-time reconciliation records, not post-seal change authority.

---

# 21. Developer validation discipline

Before any authorized executable Foundation change, inspect:

1. governing authority and exact HEAD;
2. owning contract/ADR/source;
3. project references and architecture fences;
4. affected verifier(s);
5. FCR state where applicable before seal;
6. exact prior validation evidence;
7. whether identity, authority, artifact identity, resource semantics, compatibility or cross-workstream contracts change.

Expected validation sequence:

```text
restore/build
-> Architecture
-> Security
-> affected verifier
-> predecessor/cross-stage regressions
-> deterministic rerun where required
-> clean tree
-> Architecture/Consistency review
-> Red Team
-> governed closure
```

Never weaken a verifier solely because new code fails an accepted invariant.

---

# 22. Red-Team checklist

Ask:

```text
Can missing identity pass?
Can a digest/provenance/version be substituted?
Can stale data become current?
Can unknown become authorized?
Can a projection become an executor?
Can registration become activation?
Can publication become deployment?
Can provider connectivity become business authority?
Can a component approve its own authority expansion?
Can a restart skip recovery?
Can an Application-specific semantic leak into Foundation?
Can Foundation require an Application to exist?
Can one Application reuse another's grant or exclusive capability?
Can an Application force a special Foundation case after Live Seal?
```

Any forbidden path must fail closed or remain structurally impossible.

---

# 23. Current executable anchors

Historical stage-specific commit anchors remain useful for forensic navigation, but the current consolidated executable proof is:

```text
VALIDATED_EXECUTABLE_HEAD = 889a52ddcf492ecfa4f69c3f940d56362163f04f
FULL_FOUNDATION_VALIDATION = PASS
GOVERNED_VERIFIERS = 88/88 PASS
UNKNOWN_APPLICATION = 42/42 PASS
```

A later documentation-only HEAD must not be misrepresented as having run the executable suite merely because its parent executable baseline passed.

---

# 24. Final developer rule

When deciding where behavior belongs, ask:

```text
Who owns truth?
Who owns authority?
Who owns execution?
Who owns presentation?
What exact evidence crosses the boundary?
What must remain impossible even when technically feasible?
Does this preserve Application neutrality?
Would this still work with zero Applications?
Would a new Application require Foundation to change after Live Seal?
```

If the last answer is yes, the new Application design is not compliant with the sealed Foundation model.

**Architecture first. Exact identity. Explicit authority. Evidence-bound decisions. Fail closed. Application-neutral Foundation. No hidden authority transfer.**