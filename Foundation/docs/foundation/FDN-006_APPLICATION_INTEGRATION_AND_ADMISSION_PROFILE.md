# FDN-006 — Falcon Foundation Application Integration and Admission Profile

**Identifier:** FDN-006  
**Version:** 1.0  
**Status:** CANONICAL_CONSOLIDATED_REFERENCE  
**Effective Date:** 2026-08-19  
**Owner:** Falcon Foundation  
**Project Owner Direction:** Officially document the validated Foundation Application integration and admission rules, without modifying Application or Web workstreams.  
**Normative Sources:** Falcon Vision; Falcon Constitution; APP-001; CON-023 v1.1; applicable Foundation contracts; accepted Foundation Stage 14, Stage 15 and Stage 16 runtime semantics; current FCR protocol.  
**Validated Executable Source Baseline:** `889a52ddcf492ecfa4f69c3f940d56362163f04f`  
**Executable Validation Status at Source Baseline:** `FULL FOUNDATION VALIDATION = PASS`; all 88 governed verifiers PASS; Unknown Application verifier 42/42 PASS; working tree clean.  
**Authority Effect:** NONE. This document consolidates existing governed and executable rules. It does not create activation, deployment, production, business, network, provider, broker, financial, or self-development authority.  
**Supersedes:** None.  
**Superseded By:** None.

---

## 1. Purpose

This document is the canonical Foundation-side integration and admission profile for any Application or plug-in that is to be hosted by Falcon Foundation.

Its purpose is to make the executable Foundation boundary explicit and consumable without making Foundation dependent on any particular business Application.

Foundation SHALL remain Application-neutral. Foundation SHALL validate the identity, declaration, compatibility, evidence, authority boundaries, resources, lifecycle eligibility, security posture, and runtime-hosting prerequisites of an Application, but SHALL NOT require prior business-specific knowledge of the Application's internal domain logic.

An Application being Trading, Accounting, Web, Logistics, Research, or any other business domain SHALL NOT by itself change the Foundation admission model.

No Application-name allowlist or Application-version allowlist is required by the validated generic admission path. Admission depends on governed declarations and evidence, not on a hard-coded list of known business Applications.

---

## 2. Scope

This profile applies prospectively to every Falcon Application and supported plug-in that consumes Foundation contracts or is registered with Foundation runtime hosting.

It includes Shared Web whenever Shared Web is acting as a Falcon Application consumer of Foundation runtime capability.

This profile governs the Foundation-side requirements for:

- Application identity and package identity;
- Manifest declaration;
- contract and specification compatibility;
- admission validation and admission decision;
- canonical artifact consumption;
- runtime registration;
- resource binding;
- capability declaration and isolation;
- lifecycle eligibility and transitions;
- authority separation;
- security and external-access boundaries;
- awareness placement boundaries;
- failure containment and zero-Application operation;
- update, replacement, removal, and rollback expectations;
- evidence, provenance, and fail-closed behavior; and
- cross-workstream conformance and FCR escalation.

This document does NOT define Application business logic and does NOT grant implementation authority to another workstream.

---

## 3. Canonical Integration Sequence

The normal Application-hosting path SHALL preserve the following separation:

```text
APPLICATION / PLUG-IN
        |
        v
MANIFEST DECLARATION
        |
        v
FOUNDATION VALIDATION
        |
        v
ADMISSION DECISION
        |
        v
CANONICAL ARTIFACT / LIFECYCLE / RESOURCE BINDING
        |
        v
RUNTIME REGISTRATION
        |
        v
SEPARATE ACTIVATION AUTHORITY
        |
        v
ACTIVE RUNTIME ONLY WHEN ALL APPLICABLE AUTHORITY GATES PASS
```

The following invariants are mandatory:

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
ARTIFACT_PUBLICATION != ACTIVATION
ROUTE_EXISTS != CONNECTION_AUTHORIZED
ROUTE_POLICY_BOUND != CONNECTION_EXECUTED
PROVIDER_CONNECTIVITY != EXECUTION_AUTHORITY
DATA_ACCESS != BUSINESS_AUTHORITY
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
UNKNOWN_OR_AMBIGUOUS_AUTHORITY => DENY
```

No downstream stage may infer a stronger authority from an upstream technical success.

---

## 4. Mandatory Application Identity

Every Application SHALL have a stable, explicit and attributable identity before admission.

At minimum, the Application SHALL declare:

- Application identity;
- Application version;
- Application owner;
- Application purpose;
- package identity;
- package version;
- package integrity or content identity;
- Manifest identity; and
- provenance identity.

Application identity, version and owner used by admission SHALL exactly match their Manifest declarations.

Purpose or ownership changes are material identity changes and require governed review under the applicable authority documents.

Identity substitution, silent identity mutation, ambiguous ownership, or identity/version mismatch SHALL fail closed.

Foundation SHALL NOT require a business-specific Application-name allowlist or Application-version allowlist when the generic governed admission requirements are satisfied.

---

## 5. Mandatory Application Manifest

CON-023 remains the governing Application contract-and-Manifest source. Every Application SHALL provide the declarations required by CON-023 and the executable admission model.

The Manifest SHALL include, as applicable:

1. Manifest identity.
2. Application identity.
3. Application version.
4. Application owner.
5. Application purpose.
6. Package identity.
7. Package version.
8. Package content or integrity input.
9. Declared dependencies and compatible versions.
10. Required Foundation contracts.
11. Required Foundation specifications.
12. Required Foundation services.
13. Provided capabilities.
14. Intended consumers.
15. Requested permissions.
16. Requested authorities.
17. Security profile and isolation model.
18. Minimum resource requirements.
19. Resource ceilings.
20. Degraded behavior.
21. Persistence requirements.
22. Communication requirements.
23. Configuration requirements.
24. Evidence requirements.
25. Lifecycle behavior for installation.
26. Lifecycle behavior for validation.
27. Lifecycle behavior for registration.
28. Lifecycle behavior for admission.
29. Lifecycle behavior for activation.
30. Lifecycle behavior for update.
31. Lifecycle behavior for suspension.
32. Lifecycle behavior for recovery.
33. Lifecycle behavior for replacement.
34. Lifecycle behavior for removal.
35. Health-reporting interface.
36. Failure-containment interface.
37. Exactly one Application MSA declaration.
38. Major Application branch declarations when branch-based internal architecture is used.
39. Exactly one responsible LSA for every declared major branch when branch-based architecture is used.
40. Optional CSA identities or CSA eligibility policy only for eligible intelligent components.
41. Self-development origin, ownership, evidence, and escalation path.
42. Guardian/protection interface.
43. Rollback or approved corrective-action plan.

Undeclared capability, dependency, route, permission, resource, or authority SHALL NOT be inferred or auto-granted.

The Manifest SHALL be immutable and attributable for the decision being evaluated. Its digest SHALL match its exact content.

---

## 6. Admission Validation Gates

A candidate SHALL NOT be admitted merely because it is technically loadable.

The Foundation admission path SHALL fail closed unless all applicable requirements pass.

### 6.1 Admission kind

The admitted subject kind SHALL be one of the Foundation-supported kinds:

- `APPLICATION`; or
- `PLUG-IN`.

An unsupported or invented admission kind SHALL be rejected.

### 6.2 Required identity fields

Admission identity, subject identity, subject version, and owner SHALL be present and nonblank.

Duplicate admission identity SHALL be rejected.

Duplicate active admission for the same subject identity/version SHALL be rejected according to the governed admission model.

### 6.3 Manifest binding

The admission request and Manifest SHALL agree exactly on:

- Manifest identity;
- Application identity;
- Application version; and
- Application owner.

The Manifest digest SHALL match canonical serialization of the supplied Manifest.

Manifest tampering with a stale or mismatched digest SHALL be rejected.

### 6.4 Provenance

The admission request SHALL provide attributable provenance identity, provenance content, and provenance digest.

The digest SHALL match the exact provenance content.

Missing, substituted, or mismatched provenance SHALL be rejected.

### 6.5 Bootstrap context

The admission request SHALL bind to a declared bootstrap context.

The currently validated Application admission path requires a defined bootstrap context state. Missing or invalid bootstrap context SHALL be rejected.

### 6.6 Provider boundary

The Application SHALL declare its provider boundary when applicable.

A provider-boundary declaration that attempts to bypass governance or identifies an unapproved bypass path SHALL be rejected.

Provider access, external connectivity, or possession of a provider credential SHALL NOT create execution or business authority.

### 6.7 Canonical contract linkage

The declared governing Foundation contract identity/version SHALL resolve to the canonical Foundation contract registry.

Authority source linkage SHALL match the governing canonical record.

Unknown contract identity, unsupported contract version, invalid authority linkage, or inactive/unregistered required Foundation reference SHALL be rejected.

### 6.8 Dependencies

Every declared dependency SHALL:

- have a nonblank identity;
- declare one or more compatible versions; and
- resolve at least one compatible version in the applicable canonical Foundation registry when it is a Foundation dependency.

Unknown or unresolved required Foundation dependencies SHALL fail closed.

### 6.9 Foundation requirements

Required Foundation contracts, specifications, and services SHALL be explicit and structurally valid.

Required Foundation contracts and specifications SHALL match recognized governed identities, versions, owners, and authority sources where those fields are required.

No Application may fabricate a Foundation specification or contract and treat its declaration as Foundation authority.

### 6.10 Permissions and authority requests

Requested permissions and requested authorities SHALL be explicit, attributable and scoped.

A request for authority is not a grant of authority.

```text
AUTHORITY_REQUEST != AUTHORITY_GRANT
DECLARATION != AUTHORIZATION
```

### 6.11 Decision seed and deterministic evidence

Where the admission model requires a decision seed or decision identity input, it SHALL be explicit and attributable.

Admission evidence SHALL remain deterministic and reconstructable from the governed inputs required by the implementation.

---

## 7. Runtime Registration Gates

An ADMITTED Application is not automatically active.

Runtime registration SHALL be a separate technical gate and SHALL require the applicable Stage 14/15 bindings.

At minimum, the current validated runtime registration path requires:

- runtime instance identity;
- Application identity;
- Application version;
- expected exact artifact identity;
- accepted technical artifact-consumption binding;
- exact admission binding and admission evidence;
- lifecycle attach eligibility;
- current resource grant evidence;
- provided capability declarations;
- required capability declarations; and
- observation time.

### 7.1 Duplicate prevention

A duplicate runtime instance identity SHALL be rejected.

A second non-removed hosted slot for the same Application identity SHALL be rejected by the validated host model.

### 7.2 Exact artifact binding

The technical-consumption artifact identity SHALL exactly match the artifact expected by runtime registration.

The artifact-consumption binding SHALL NOT silently carry:

- activation authorization;
- deployment authorization;
- production authorization;
- business authority; or
- silent-upgrade authority.

Any such implied authority in the technical artifact binding SHALL invalidate runtime registration.

### 7.3 Exact admission binding

Runtime registration SHALL require positive admission evidence bound to the exact Application identity and exact Application version being registered.

Missing admission evidence, negative admission, identity substitution, or version substitution SHALL be rejected.

### 7.4 Lifecycle attach eligibility

Runtime registration SHALL require valid lifecycle eligibility for the attach operation and target version.

Lifecycle evidence for another Application or another target version SHALL NOT satisfy the gate.

### 7.5 Resource grants

At least one applicable current resource grant is required by the validated runtime-host registration model.

Every resource grant SHALL be attributable to the exact Application identity and SHALL contain valid evidence.

For every grant:

```text
0 <= Allocation <= Quota <= Ceiling
```

Grant identity and resource-class binding SHALL be valid and nonduplicated according to the runtime model.

A grant that is not yet effective, is expired, or carries evidence observed in the future relative to the registration observation time SHALL be rejected.

### 7.6 Capability declarations

Provided capabilities SHALL have valid identities and visibility declarations.

Duplicate provided capability declarations SHALL be rejected.

Required capabilities SHALL be explicit and nonduplicated.

An exclusive capability SHALL NOT be simultaneously owned by another non-removed runtime slot when the exclusivity rule applies.

### 7.7 Registration result

Successful runtime registration creates a `Registered` slot only.

A successful registration decision SHALL NOT by itself grant:

- activation;
- deployment;
- production; or
- business authority.

---

## 8. Separate Runtime Authority

Activation, suspension, isolation, removal, deployment, production adoption, and business execution SHALL remain separately governed operations.

Runtime transition authority SHALL be explicit, attributable, action-specific, identity-bound, version-bound where applicable, current, nonrevoked, and supported by the required evidence.

The Application SHALL NOT grant runtime authority to itself merely because:

- its Manifest is valid;
- its contracts are valid;
- it passed admission;
- it is registered;
- it can technically execute;
- a route exists;
- a credential exists;
- an earlier authorization existed;
- the Owner or another workstream is silent; or
- an urgent condition exists.

Technical capability SHALL NOT be treated as governance authority.

---

## 9. Awareness and Foundation Boundary

A hosted Application remains an Application.

Hosting SHALL NOT transform an Application into:

- Falcon Foundation;
- Kernel authority;
- a Foundation service by implication;
- Foundation FSA;
- Foundation Guardian or protection authority; or
- Foundation truth owner.

Application awareness SHALL remain within the Application boundary:

- one Application MSA;
- LSA entities for major branches where used; and
- CSA entities only for eligible declared intelligent components where used.

FSA remains the Foundation/OS-level self-awareness authority and review layer under the governing Falcon awareness model.

An Application-originated self-development proposal SHALL preserve its true origin and applicable escalation path. FSA review does not by itself create implementation, deployment, activation, or production adoption authority.

---

## 10. Capability Isolation and Cross-Application Boundaries

Every hosted Application SHALL retain separate identity, runtime slot, resources, capabilities, lifecycle state, evidence, and authority scope.

A private capability SHALL be consumable only within its permitted private boundary.

A shared capability SHALL require declared compatible consumption and SHALL NOT create broader authority than the capability contract grants.

One Application SHALL NOT obtain control over another Application merely because both are hosted by the same Foundation.

Application-provided capability reuse SHALL NOT turn the capability into a Foundation service by implication.

Failure, compromise, suspension, isolation, or removal of one Application SHALL be containable without requiring Foundation to fail or another unrelated Application to inherit its authority.

---

## 11. Communication and Transport

Application-to-Foundation and Application-to-Application communication SHALL use the applicable governed communication, FIL, schema, route, security, and authority contracts.

A transport path SHALL NOT become an authority side channel.

The following distinctions are mandatory:

```text
MESSAGE_ACCEPTED != BUSINESS_ACTION_AUTHORIZED
PROJECTION_AVAILABLE != CONTROL_AUTHORITY
REQUEST_TRANSPORT != EXECUTION_TRANSPORT
PUBLIC_PROJECTION != CONTROL_REQUEST
TRANSPORT_CAPABILITY != BUSINESS_AUTHORITY
```

Foundation may treat business payloads as opaque except where a separately governed security-inspection rule applies.

---

## 12. External Access, Providers, Networks and Credentials

External route declaration, provider availability, network reachability, API possession, broker connectivity, or data receipt SHALL NOT imply authority to connect, execute, trade, mutate business state, or spend capital.

Where external access is governed, exact destination, purpose, identity, security mode, credential-reference requirement, and runtime authorization SHALL be respected.

Secrets, private keys, passwords, API secret values, tokens, and equivalent sensitive bytes SHALL NOT be placed into ordinary Application state merely to make a connection work.

Applications SHALL use the applicable governed secret/credential-reference mechanism.

```text
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
PUBLIC_ENDPOINT != UNRESTRICTED_EGRESS_AUTHORITY
PROVIDER_ROUTE != UNBOUNDED_PROVIDER_AUTHORITY
CONNECTIVITY_READY != CONNECTIVITY_ACTIVATED
```

Shared Web presentation connectivity remains presentation/request capability only unless separate authority expressly grants a stronger action.

---

## 13. Lifecycle and Update Rules

Every Application SHALL declare and respect the governed lifecycle covering:

```text
Install
Validate
Register
Admit
Activate
Update
Suspend
Recover
Replace
Remove
```

A lifecycle state change does not create a new authority decision by implication.

Updates SHALL NOT use silent upgrade behavior.

Material changes to identity, ownership, purpose, package, relevant contract set, relevant dependencies, permissions, requested authority, security posture, resources, runtime bindings, or other admission-critical declaration SHALL trigger the applicable governed revalidation/review path.

Replacement and removal SHALL preserve evidence and authority separation.

Rollback or corrective action SHALL follow an approved bounded plan and SHALL NOT restore revoked authority merely because technical rollback succeeded.

---

## 14. Failure Containment and Fail-Closed Operation

Foundation and hosted Applications SHALL prefer denial and containment over unproven continuation when required trust or authority is unknown.

At minimum, the following conditions SHALL fail closed when applicable:

- invalid or missing Application identity;
- owner substitution;
- Manifest mismatch or tampering;
- provenance mismatch;
- unknown Foundation contract;
- unsupported Foundation contract version;
- invalid Foundation specification reference;
- unresolved required Foundation dependency;
- provider-boundary bypass;
- duplicate admission identity;
- duplicate Application identity/version admission;
- invalid artifact binding;
- invalid admission-to-runtime binding;
- invalid lifecycle attach binding;
- invalid or stale resource grant;
- invalid capability declaration;
- absent, stale, revoked, invalid, ambiguous, or mismatched runtime authority where authority is required.

No timeout, silence rule, legacy behavior, emergency, or technical feasibility SHALL convert an unknown authority state into permission unless an explicit higher governing rule says so.

---

## 15. Zero-Application Foundation Invariant

Falcon Foundation SHALL remain structurally valid with zero hosted Applications.

Application installation is not required to make Foundation itself a valid Foundation runtime substrate.

Removing the last Application SHALL NOT invalidate Foundation.

An Application SHALL therefore be attachable and removable as a hosted subject, not embedded as an architectural prerequisite for Foundation identity.

---

## 16. Application Neutrality and Compatibility

Foundation's generic Application path SHALL remain business-domain neutral.

The validated Unknown Application proof demonstrates that a previously unknown synthetic Application identity and arbitrary Application-defined version can:

1. produce a generic Application-targeted Foundation projection;
2. pass canonical Manifest validation;
3. receive an `ADMITTED` decision;
4. bind that admission to real Foundation runtime hosting;
5. register into the runtime host; and
6. remain unactivated and without deployment or business authority.

The same proof verifies fail-closed rejection for tampered Manifest, invalid Foundation reference, provider-boundary bypass, admission mismatch, and related negative cases.

Legacy or named compatibility surfaces, including accepted FSATS-specific aliases, SHALL be treated as compatibility aliases and SHALL NOT redefine the generic Application hosting model as Application-specific.

Preserving an accepted compatibility alias is permitted when the functional core remains generic and the alias does not create additional authority.

---

## 17. Verification Evidence

This document consolidates rules proven against the executable Foundation source baseline:

```text
VALIDATED_EXECUTABLE_HEAD = 889a52ddcf492ecfa4f69c3f940d56362163f04f
FULL_FOUNDATION_VALIDATION = PASS
ALL_GOVERNED_VERIFIERS = PASS
GOVERNED_VERIFIER_COUNT = 88
FOUNDATION_UNKNOWN_APPLICATION_VERIFIER = PASS
UNKNOWN_APPLICATION_CHECKS = 42/42
WORKING_TREE_CLEAN = PASS
POST_FIX_RED_TEAM_CRITICAL = 0
POST_FIX_RED_TEAM_HIGH = 0
POST_FIX_RED_TEAM_MEDIUM = 0
POST_FIX_RED_TEAM_LOW = 0
POST_FIX_RED_TEAM_ACTIONABLE_INFO = 0
```

The Unknown Application verification explicitly demonstrated:

```text
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

### Documentation-only HEAD note

The commit that introduces this FDN-006 document is necessarily newer than the executable source baseline above. Therefore, this document SHALL NOT misrepresent the earlier full validation as having executed on the documentation commit itself.

The executable behavior recorded here was validated at `889a52ddcf492ecfa4f69c3f940d56362163f04f`. The documentation commit changes no executable Foundation source by intention.

If a future closure rule requires full validation on the exact latest documentation-inclusive HEAD, the governed canonical validator SHALL be rerun against that exact HEAD before such a claim is made.

---

## 18. Consumer Workstream Conformance

Application and Web workstreams SHALL consume this Foundation profile only within their own authorized write scopes.

Foundation SHALL NOT modify Application or Shared Web files merely to force conformance.

A consumer workstream may document compatibility with this profile without obtaining new implementation authority.

If a consumer identifies a real missing, partial, or incompatible Foundation capability, it SHALL use the repository FCR protocol rather than modifying Foundation-owned files itself.

FCR coordination does not itself grant implementation authority.

No new FCR is required merely to announce or acknowledge this consolidated profile when no unresolved cross-workstream gap exists.

---

## 19. Change Control

This profile consolidates current governed and executable behavior. It is not an independent authority source that may silently override the Falcon Vision, Constitution, APP-001, CON-023, accepted Foundation contracts, accepted ADRs, or explicit Project Owner/governance decisions.

If this profile conflicts with a higher governing authority, the higher authority controls and the profile SHALL be corrected through the governed Foundation process.

Any future material change to Application admission, authority separation, runtime hosting, resource isolation, external-access authority, awareness placement, fail-closed behavior, or Application-neutrality SHALL require the applicable governed Foundation change process and corresponding verification evidence.

A material executable change SHALL NOT inherit the validation evidence of `889a52ddcf492ecfa4f69c3f940d56362163f04f` by implication.

---

## 20. Canonical Acceptance Checklist

Before a new Application can be treated as a valid Foundation-hosted candidate, the responsible integration review SHALL be able to answer **YES** to every applicable item below:

- [ ] Application identity is explicit and stable.
- [ ] Application version is explicit.
- [ ] Application owner and purpose are explicit.
- [ ] Package identity/version/integrity are explicit.
- [ ] Manifest is complete under CON-023.
- [ ] Manifest identity/content/digest match exactly.
- [ ] Provenance identity/content/digest are valid.
- [ ] Governing contract identity/version resolves canonically.
- [ ] Required Foundation contracts are valid.
- [ ] Required Foundation specifications are valid.
- [ ] Required Foundation services are declared.
- [ ] Required dependencies resolve to compatible versions.
- [ ] Permissions are explicitly declared.
- [ ] Authority requests are explicitly declared and are not treated as grants.
- [ ] Security profile and isolation model are declared.
- [ ] Provider/external-access boundary is declared when applicable.
- [ ] No provider-boundary bypass exists.
- [ ] Admission succeeds with attributable evidence.
- [ ] Runtime artifact binding is exact.
- [ ] Runtime admission binding matches exact identity/version.
- [ ] Lifecycle attach eligibility is valid.
- [ ] Resource grants are current, attributable, bounded and valid.
- [ ] Provided and required capability declarations are valid.
- [ ] Runtime registration creates only a Registered state.
- [ ] Separate activation authority is present before activation.
- [ ] No admission/registration path implies deployment authority.
- [ ] No admission/registration path implies production authority.
- [ ] No admission/registration path implies business authority.
- [ ] No Application self-grants Foundation authority.
- [ ] Awareness entities remain inside their governed boundaries.
- [ ] FSA remains Foundation/OS-level.
- [ ] Cross-Application isolation is preserved.
- [ ] Communication uses governed contracts/routes.
- [ ] Credentials use governed reference handling; secret bytes do not enter ordinary state.
- [ ] Updates are not silent and material changes receive required revalidation.
- [ ] Failure containment and rollback/corrective-action plans are declared.
- [ ] Unknown or ambiguous authority fails closed.
- [ ] Foundation remains valid if the Application is absent or removed.
- [ ] Any real Foundation gap is escalated through FCR rather than patched cross-boundary.

Failure of any mandatory applicable item means the candidate is not eligible for the stronger state dependent on that item.

---

## 21. Final Foundation Rule

The canonical operating principle is:

> Falcon Foundation knows how to host a governed Application without needing to know that Application's business domain in advance. The Application must prove who it is, what it requires, what it may consume, what it requests, what resources it has, what evidence binds it, and what authority it actually holds. No technical success creates authority by implication.

In compact form:

```text
DECLARE EXACTLY
VALIDATE CANONICALLY
ADMIT EXPLICITLY
REGISTER WITHOUT ACTIVATING
AUTHORIZE SEPARATELY
ISOLATE BY DEFAULT
FAIL CLOSED
PRESERVE EVIDENCE
KEEP FOUNDATION APPLICATION-NEUTRAL
```
