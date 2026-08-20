# Falcon Foundation Programming Manual

**Edition:** 2026-08-19  
**Governed state:** `Stage 0A through Stage 16 = ACCEPTED_AND_CLOSED`  
**Validated executable baseline:** `889a52ddcf492ecfa4f69c3f940d56362163f04f`  
**Executable status:** `FULL FOUNDATION VALIDATION = PASS`; all 88 governed verifiers PASS; Unknown Application verifier 42/42 PASS.  
**Current integration policy:** FDN-006 + FDN-007.  
**Audience:** developers, architects, reviewers, maintainers, and AI agents that implement Foundation code or consume Foundation contracts.

> This is a consolidated engineering manual. It does not replace Falcon Vision, Falcon Constitution, accepted ADRs/specifications, canonical Owner records, FDN-006, FDN-007, or accepted source/evidence. Higher authority and exact executable evidence prevail.

---

# 1. Operating mode and workstream rules

## 1.1 Before formal Live Seal

Foundation-owned changes belong on `foundation-development` and only under explicit Foundation governance/Owner authority. Application/Web files remain outside Foundation write authority.

## 1.2 After formal Live Seal

FDN-007 becomes the permanent consumer-fit rule:

```text
APPLICATION MUST FIT FOUNDATION
FOUNDATION MUST NOT CHANGE TO FIT APPLICATION
```

A future Application incompatibility is not a Foundation feature request. The Application must adapt on its own side, consume an already-approved Shared Application capability, remove/redesign the feature, or be classified incompatible with the sealed Foundation.

Do not create Application-name/version allowlists, special runtime branches, security exceptions, schema bypasses, weaker admission gates, or business-domain logic inside Foundation for a new consumer.

---

# 2. Architectural and authority invariants

Accepted dependency direction:

```text
Applications
    ↓
Capabilities
    ↓
Shared Services
    ↓
Kernel / Foundation
```

Mandatory separations include:

```text
TECHNICAL_CAPABILITY != AUTHORITY
TECHNICAL_SUCCESS != BUSINESS_AUTHORITY
IDENTITY_FACT != AUTHORITY_DECISION
AUTHENTICATION != AUTHORIZATION
VALID_CONTRACT != ADMISSION
ADMISSION != ACTIVATION
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
PUBLICATION != ACTIVATION
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
SELF_AWARENESS != AUTHORITY
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
```

Unknown, stale, mismatched, revoked, incomplete, ambiguous or unverifiable authority-critical state must fail closed.

Zero-Application operation remains a required invariant.

---

# 3. Source-of-truth order

1. Falcon Vision and Falcon Constitution.
2. Document authority and Foundation governance rules.
3. Explicit Owner decisions and canonical closure records.
4. Accepted specifications/ADRs, including APP-001 and CON-023 where applicable.
5. FDN-006 Application Integration and Admission Profile.
6. FDN-007 Live Foundation Seal and Future Application Onboarding Policy.
7. Production contracts/source.
8. Architecture/Security tests and governed verifiers.
9. Planning/checkpoint/history documents.
10. Legacy references such as V1.3 only when needed.

`V1.3 = REFERENCE`, not superior authority.

---

# 4. Stage engineering map

## Stage 0A: governance and policy corpus
Establishes document authority, architecture rules, contracts, validation expectations and governed change process.

## Stage 1: project/Application models and manifests
Defines typed identity, Manifest and configuration semantics. A descriptor does not grant activation.

## Stage 2: Registry/Catalog
Exact registration and discovery. Duplicate/ambiguous identity fails closed. Registered is not active.

## Stage 3: technical lifecycle state machine
Deterministic legal transitions. A lifecycle transition cannot mint authority.

## Stage 4: technical runtime lifecycle
Start/stop/restart and related runtime mechanics. Restart is not recovery; runtime start is not business readiness.

## Stage 5: FIL production transport
Canonical messaging, schema/compatibility, delivery, dependency/evidence and plug-and-play boundaries. Do not bypass governed FIL with private authority side channels.

## Stage 6: resource governance
Foundation-owned resource truth, quota/ceiling semantics, pressure/degradation, bounded defer/deny/load-shedding. Preserve:

```text
0 <= Allocation <= Quota <= Ceiling
REQUESTED_RESOURCE != GRANTED_RESOURCE
RESOURCE_PROJECTION != RESOURCE_AUTHORITY
```

## Stage 7: Health/FSA technical fitness
Foundation technical self-model and evidence/fitness evaluation only. Application business judgment stays in Applications.

## Stage 8: Guardian and Safe State
Independent protection/containment/restriction. Containment does not grant release authority.

## Stage 9: controlled recovery and independent release
Preserve repair/recovery/readiness/release separation.

## Stage 10: FRS-001 reconstruction/review
Current governed release semantics take precedence over historical assumptions.

## Stage 11: transport QoS/deadlines/observability
Deadline/expiry and latency observation. QoS or priority facts do not create business authority.

## Stage 12: external-access governance
Exact technical route authorization by identity, role, environment, purpose, destination and credential reference. The evaluator does not execute sockets/providers/brokers.

## Stage 13: FSA governance and AI Kill
FSA may observe/review/recommend within bounded governance. Independent Foundation authority owns Kill enforcement. Protected properties cannot be changed by ordinary self-evolution.

## Stage 14: canonical artifact publication
Immutable artifact identity/version/digest/evidence/compatibility/provenance. Moving branch HEAD is not runtime identity. Publication is not activation.

## Stage 15: Application runtime hosting
Generic, Application-neutral hosting. Successful registration creates a registered slot only and does not grant activation/deployment/business authority.

## Stage 16: identity/session/MFA runtime
Explicit Falcon identity, external-identity links, replay protection, MFA and session semantics. Security context is not business authority.

---

# 5. Post-Stage-16 generic hardening, not Stage 17

The final generic hardening added/validated:

- generic `PublicRuntimeProjectionProfiles` Application targeting;
- real unknown-Application admission and runtime-hosting proof;
- no Application name/version whitelist;
- canonical Stage 13 AI Kill artifact publication;
- exact public-runtime projection transport.

Unknown Application proof:

```text
UNKNOWN_APPLICATION_IDENTITY = unknown-application-proof-7f3c9a
APPLICATION_VERSION = 999.123.456-test
APPLICATION_NAME_ALLOWLIST = NOT_REQUIRED
APPLICATION_VERSION_ALLOWLIST = NOT_REQUIRED
MANIFEST_AND_FOUNDATION_CONTRACTS = REQUIRED
ADMISSION_TO_RUNTIME_HOSTING = PROVEN
TAMPERED_MANIFEST = FAIL_CLOSED
INVALID_FOUNDATION_REFERENCE = FAIL_CLOSED
PROVIDER_BOUNDARY_BYPASS = FAIL_CLOSED
CHECKS = 42/42 PASS
```

This proof is the core technical evidence that Foundation can host future Applications without knowing their business domain in advance.

---

# 6. Admission and runtime-registration programming model

The normal generic path is:

```text
APPLICATION / PLUG-IN
    -> Manifest declaration
    -> Foundation validation
    -> Admission decision
    -> exact artifact/lifecycle/resource binding
    -> runtime registration
    -> separate activation authority
```

Admission validates exact identity/version/owner, Manifest digest, provenance digest, required declarations, canonical dependencies/contracts/specifications/services, provider boundary, permissions/authority requests and deterministic evidence.

Runtime registration additionally requires exact runtime instance, artifact binding, positive admission evidence for the exact identity/version, eligible lifecycle Attach evidence, current resource grants and valid capability declarations.

Registration result must preserve:

```text
RUNTIME_REGISTERED_NOT_ACTIVATED
CarriesDeploymentAuthority = false
CarriesBusinessAuthority = false
```

Do not infer stronger authority from a successful earlier gate.

---

# 7. FDN-006 consumer contract

When integrating an Application, developers must treat FDN-006 as the Foundation-side contract for:

- stable identity/version/owner/purpose;
- complete Manifest and provenance;
- dependencies/contracts/specifications/services;
- capabilities/consumers/exclusivity;
- permissions and authority requests;
- resources and ceilings;
- security, provider boundaries and credential references;
- lifecycle/update/recovery/removal;
- health/failure containment;
- FSA/MSA/LSA/CSA placement;
- fail-closed semantics;
- admission/registration/activation separation.

Application-side adapters are allowed only when they translate into existing published Foundation contracts without changing Foundation semantics.

---

# 8. FDN-007 Live Seal rule for programmers

After formal Live Seal, do **not** use the historical pattern:

```text
Application gap -> Foundation FCR -> Foundation code change
```

for new-Application fit.

The correct post-seal pattern is:

```text
Application requirement
    -> check published Foundation/Shared Application contracts
    -> adapt inside Application if valid
    -> redesign/remove unsupported requirement if needed
    -> if Foundation change is still required: INCOMPATIBLE_WITH_SEALED_FOUNDATION
```

Historical FCRs may remain as audit records, and pre-Live-Seal reconciliation FCRs may be closed according to their evidence. They are not a permanent post-seal escape hatch for consumer-specific Foundation changes.

---

# 9. Validation discipline

For any executable Foundation change that is actually authorized before seal, the expected pattern remains:

```text
fresh authority/FCR/HEAD reconciliation
-> minimal Foundation-owned implementation
-> restore/build
-> Architecture verification
-> Security verification
-> affected-stage verifier
-> predecessor/cross-stage regressions
-> deterministic rerun where governed
-> clean tracked tree
-> stable exact candidate
-> Architecture/Consistency review
-> broad Red Team
-> Owner closure where required
```

After Live Seal, ordinary consumer onboarding must not trigger this chain because ordinary onboarding must not modify Foundation.

---

# 10. Developer Red-Team questions

For every relevant Foundation or integration change, ask:

```text
Can missing identity pass?
Can version/digest/provenance mutation pass?
Can stale evidence become current?
Can unknown become healthy/authorized?
Can registration become activation?
Can publication become deployment?
Can provider connectivity become execution authority?
Can credential reference expose secret bytes?
Can an Application-specific semantic leak into Foundation?
Can Foundation correctness require an Application to exist?
Can one Application inherit another's resources/capabilities/authority?
Can MSA/LSA/CSA cross into FSA/Foundation scope?
Can a new Application force a Foundation special case?
```

Forbidden paths must remain impossible or fail closed.

---

# 11. Current readiness boundary

```text
STAGE_0A_THROUGH_STAGE_16 = ACCEPTED_AND_CLOSED
FULL_FOUNDATION_VALIDATION = PASS
GOVERNED_VERIFIERS = 88/88 PASS
UNKNOWN_APPLICATION_CHECKS = 42/42 PASS
FOUNDATION_READY_FOR_APPLICATION_CONSUMPTION = YES
FOUNDATION_CURRENTLY_REQUIRES_STAGE17 = NO
FOUNDATION_APPLICATION_NEUTRAL = YES
```

Do not overclaim:

```text
TESTED != DEPLOYED
READY_FOR_APPLICATION_CONSUMPTION != APPLICATION_ACTIVATED
FOUNDATION_READY != LIVE_TRADING_AUTHORITY
```

---

# 12. Canonical reference map

Start with:

- `docs/03_DOCUMENT_AUTHORITY.md`
- `docs/development/FOUNDATION_WORKSTREAM_RULES.md`
- `docs/foundation/FDN-006_APPLICATION_INTEGRATION_AND_ADMISSION_PROFILE.md`
- `docs/foundation/FDN-007_LIVE_FOUNDATION_SEAL_AND_FUTURE_APPLICATION_ONBOARDING_POLICY.md`
- `docs/manuals/FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_EN.md`
- `docs/manuals/FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_AR.md`
- CON-023 and applicable accepted ADRs/contracts
- `src/Foundation.*`
- `tests/Falcon.Foundation.Architecture.Tests/`
- `tests/Falcon.Foundation.Security.Tests/`
- `verification/`

**Final programming rule:** preserve exact identity, explicit authority, evidence-bound decisions, Application neutrality, and fail-closed semantics. After Live Seal, never patch Foundation merely because a new Application was designed around a different contract.