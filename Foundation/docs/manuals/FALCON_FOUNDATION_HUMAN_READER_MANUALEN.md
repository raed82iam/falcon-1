# Falcon Foundation Human Reader Manual

**Edition:** 2026-08-19  
**Current governed state:** `Stage 0A through Stage 16 = ACCEPTED_AND_CLOSED`  
**Current Foundation posture:** technically complete and ready for formal Live Seal; not automatically activated, deployed, or business-authorized by this manual.  
**Validated executable baseline:** `889a52ddcf492ecfa4f69c3f940d56362163f04f`  
**Executable validation:** `FULL FOUNDATION VALIDATION = PASS`; all 88 governed verifiers PASS; Unknown Application verifier 42/42 PASS.  
**Current documentation lineage:** FDN-006 and FDN-007 define the generic Application integration contract and the post-Live-Seal onboarding rule.  
**Audience:** Project Owner, project managers, reviewers, architects, operators, and readers who want to understand Foundation without reading source code.

> This manual explains the current accepted Foundation in human terms. It does not replace Falcon Vision, Falcon Constitution, canonical Owner records, accepted contracts/ADRs, FDN-006, FDN-007, or production source. If a conflict exists, the higher governing source and accepted executable evidence prevail.

---

# 1. What Falcon Foundation is

Falcon Foundation is the stable operating substrate beneath Falcon Applications. It provides reusable technical governance and runtime services while Applications keep their own business logic.

Foundation owns platform concerns such as:

- project/Application identity and manifests;
- registries and canonical contract resolution;
- lifecycle and runtime lifecycle governance;
- FIL messaging and transport semantics;
- dependency and evidence governance;
- resource governance and pressure handling;
- technical health, Foundation self-awareness, and fitness;
- Guardian protection, containment, Safe State, recovery and independent release;
- external-access authorization and credential-reference security;
- canonical artifact publication and exact consumption;
- generic Application runtime hosting;
- authoritative identity, authentication, session and MFA runtime;
- Foundation-level FSA governance and independent AI Kill enforcement.

Foundation does **not** own a hosted Application's trading logic, accounting rules, medical logic, chart rendering, portfolio decisions, or other business-domain judgment.

The architectural direction remains:

```text
Applications
    ↓
Capabilities
    ↓
Shared Services
    ↓
Kernel / Foundation
```

The permanent boundary is:

```text
APPLICATION BUSINESS LOGIC = APPLICATION OWNED
FOUNDATION TECHNICAL GOVERNANCE = FOUNDATION OWNED
```

---

# 2. The rules that matter most

Falcon Foundation is built around strict separations:

```text
TECHNICAL_CAPABILITY != AUTHORITY
TECHNICAL_SUCCESS != BUSINESS_AUTHORITY
AUTHENTICATION != AUTHORIZATION
REGISTERED != ACTIVATED
ADMITTED != ACTIVATED
PUBLISHED != ACTIVATED
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
SELF_AWARENESS != AUTHORITY
```

Other permanent rules:

- Architecture First.
- Vision and Constitution outrank implementation convenience.
- Authority must be explicit, bounded, attributable, reviewable, minimal and current.
- Unknown, stale, mismatched, revoked or unverifiable authority-critical state fails closed.
- Zero-Application operation remains valid.
- One Application may not gain control over another merely because both are hosted by Foundation.
- FSA is Foundation-level only. MSA/LSA/CSA remain inside Applications.

---

# 3. What was built, stage by stage

## Stage 0A: planning, contracts and policy corpus
Established authority hierarchy, governance, source classification, legacy/reference handling, Architecture-first rules and validation expectations.

## Stage 1: project models, manifests, DTOs and configuration
Created governed technical description of projects/Applications. Description does not grant runtime authority.

## Stage 2: registry and catalog
Provided exact registration/discovery without confusing discovery with activation.

## Stage 3: technical lifecycle state machine
Defined deterministic legal states and transitions. A lifecycle transition cannot mint new authority.

## Stage 4: technical runtime lifecycle
Governed start/stop/restart and related technical runtime transitions. Runtime start is not business readiness or deployment authority.

## Stage 5: FIL production transport
Established canonical messaging, schemas, compatibility, delivery, dependency/evidence semantics and plug-and-play Application integration boundaries.

## Stage 6: resource governance and operational pressure
Established Foundation-owned resource truth, quotas, pressure/degradation behavior and bounded defer/deny/load-shedding decisions.

## Stage 7: health, Foundation self-awareness and technical fitness
Established health/integrity/evidence awareness and a bounded Foundation technical self-model. It does not perform Application business judgment.

## Stage 8: Guardian and Platform Safe State
Established independent technical protection, containment, isolation and protective restriction.

## Stage 9: controlled recovery and independent release
Separated repair, recovery, readiness, release authorization and release execution.

## Stage 10: FRS-001 reconstruction and release review
Reconciled release/review semantics against current governed evidence instead of relying on legacy assumptions.

## Stage 11: transport QoS, deadlines and observability
Added governed expiry/deadline behavior and deterministic transport observations such as p50/p95/p99 without converting QoS into business authority.

## Stage 12: governed external access and credential-reference security
Created exact technical egress authorization by principal, role, environment, purpose, destination and credential reference. It authorizes a technical route only; it does not open the connection or grant business execution authority.

## Stage 13: FSA governance, AI Kill and Safe Core
Established Foundation FSA governance, independent monitoring/investigation, trusted baselines, remediation sandbox, Controlled Revival, bounded self-improvement review and independent AI Kill enforcement.

## Stage 14: canonical artifact publication and public operational projection
Established immutable artifact identity/version/digest/evidence/compatibility and read-only operational projections. A moving branch HEAD is not a runtime identity.

## Stage 15: generic Application runtime hosting
Established Application-neutral runtime hosting while preserving zero-Application operation and the separation between registration and activation.

## Stage 16: identity, authentication, session and MFA runtime
Established Falcon identity/session/MFA technical security context with replay protection and explicit external-identity linking. Login facts do not create business authority.

---

# 4. Final generic Application hardening

After Stage 16, bounded compatibility hardening was completed without inventing Stage 17.

The most important result is the **Unknown Application proof**. A synthetic Application unknown to Foundation in advance, with arbitrary version `999.123.456-test`, successfully passed the generic path and was registered into the real runtime host without a name or version allowlist.

The proof established:

```text
APPLICATION_NAME_ALLOWLIST = NOT_REQUIRED
APPLICATION_VERSION_ALLOWLIST = NOT_REQUIRED
MANIFEST_AND_FOUNDATION_CONTRACTS = REQUIRED
ADMISSION_TO_RUNTIME_HOSTING = PROVEN
TAMPERED_MANIFEST = FAIL_CLOSED
INVALID_FOUNDATION_REFERENCE = FAIL_CLOSED
PROVIDER_BOUNDARY_BYPASS = FAIL_CLOSED
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
```

This is why Foundation can host future domains such as Accounting, Logistics, Research, Medical, Web, Communication or other Applications without knowing their business logic in advance.

---

# 5. FDN-006: the published Application integration contract

`docs/foundation/FDN-006_APPLICATION_INTEGRATION_AND_ADMISSION_PROFILE.md` is the canonical Foundation-side integration profile.

It defines the generic requirements for:

- identity and version;
- Manifest and provenance;
- Foundation contracts/specifications/services;
- dependencies;
- capabilities and consumers;
- permissions and authority requests;
- resource grants and ceilings;
- security and credential references;
- lifecycle and recovery;
- health and failure containment;
- MSA/LSA/CSA placement;
- admission, runtime registration and separate activation authority;
- fail-closed behavior.

The normal path is:

```text
APPLICATION DESIGN
    -> MANIFEST
    -> FOUNDATION VALIDATION
    -> ADMISSION
    -> ARTIFACT / LIFECYCLE / RESOURCE BINDING
    -> RUNTIME REGISTRATION
    -> SEPARATE ACTIVATION AUTHORITY
    -> ACTIVE ONLY WHEN ALL APPLICABLE GATES PASS
```

---

# 6. FDN-007: what changes after Live Seal

`docs/foundation/FDN-007_LIVE_FOUNDATION_SEAL_AND_FUTURE_APPLICATION_ONBOARDING_POLICY.md` defines the long-term operating model after Foundation is formally declared Live and Sealed.

The permanent rule is:

```text
APPLICATION MUST FIT FOUNDATION
FOUNDATION MUST NOT CHANGE TO FIT APPLICATION
```

After Live Seal, a new Application does **not** get a Foundation patch, special case, name/version allowlist, weaker admission rule, or Foundation-directed FCR merely because it does not fit the published contract.

If a future Application cannot comply, the correct outcomes are:

```text
READY_FOR_FOUNDATION_ADMISSION
APPLICATION_REDESIGN_REQUIRED
INCOMPATIBLE_WITH_SEALED_FOUNDATION
```

There is no fourth outcome called `CHANGE_FOUNDATION_FOR_THIS_APPLICATION`.

The detailed human onboarding procedure is documented in:

- `FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_EN.md`
- `FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_AR.md`

---

# 7. Is Foundation ready?

At the validated executable baseline:

```text
FULL_FOUNDATION_VALIDATION = PASS
GOVERNED_VERIFIERS = 88/88 PASS
UNKNOWN_APPLICATION = 42/42 PASS
POST_FIX_RED_TEAM_ACTIONABLE_FINDINGS = 0
FOUNDATION_READY_FOR_APPLICATION_CONSUMPTION = YES
FOUNDATION_REQUIRES_STAGE17 = NO
```

This means the Foundation substrate itself has no known executable gap requiring another Foundation development stage for generic Application hosting.

It does **not** mean:

```text
FALCON_LIVE = AUTOMATICALLY TRUE
APPLICATION_ACTIVATION = AUTOMATICALLY AUTHORIZED
PRODUCTION_DEPLOYMENT = AUTOMATICALLY AUTHORIZED
PROVIDER_CONNECTIVITY = AUTOMATICALLY ACTIVE
BROKER_CONNECTIVITY = AUTOMATICALLY ACTIVE
LIVE_TRADING = AUTOMATICALLY AUTHORIZED
```

Those are separate consumer/runtime/business decisions.

---

# 8. Current pre-Live-Seal consumer reconciliation

The current ordinary Application workstream completed FCR-0252 and verified sealed-Foundation conformance on its own exact current HEAD with no Foundation change required.

Shared Web remains responsible for its own pre-Live-Seal conformance work under FCR-0253. Any remaining Web gap is Web-owned and must not be converted into a Foundation patch merely for consumer fit.

This does not make Foundation dependent on Web. Foundation remains valid with zero Applications.

---

# 9. Glossary

**Admission:** Foundation decision that a declared candidate satisfies applicable admission gates. Admission is not activation.  
**Runtime Registration:** technical registration into Foundation hosting. Registration is not activation.  
**Authority:** explicit permission to perform a governed action.  
**Artifact:** exact published contract/output with immutable identity/version/digest/evidence.  
**FIL:** Falcon governed integration and transport language/boundary.  
**FSA:** Foundation Self-Awareness, Foundation/OS-level only.  
**MSA/LSA/CSA:** Application awareness layers.  
**Guardian:** independent technical protection and containment boundary.  
**Fail closed:** uncertainty or invalid evidence results in denial/hold, not permission.  
**Live Seal:** the operational state in which Foundation is treated as the stable published substrate and future Applications must adapt to it.

---

# 10. Governing references

Use the following for exact meaning:

- Falcon Vision.
- Falcon Constitution.
- `docs/03_DOCUMENT_AUTHORITY.md`.
- `docs/development/FOUNDATION_WORKSTREAM_RULES.md`.
- `docs/foundation/FDN-006_APPLICATION_INTEGRATION_AND_ADMISSION_PROFILE.md`.
- `docs/foundation/FDN-007_LIVE_FOUNDATION_SEAL_AND_FUTURE_APPLICATION_ONBOARDING_POLICY.md`.
- CON-023 and applicable accepted contracts/ADRs.
- `docs/canonical-records/owner-decisions/`.
- `src/Foundation.*`.
- `verification/` and `tests/`.

**Final rule:** Foundation knows how to host a governed Application without needing to know its business domain in advance. The Application proves its identity, declarations, evidence, resources and authority. Foundation never invents authority from technical success.