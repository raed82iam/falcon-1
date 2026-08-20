# FDN-007 — Live Foundation Seal and Future Application Onboarding Policy

**Identifier:** FDN-007  
**Version:** 1.0  
**Status:** CANONICAL_PROSPECTIVE_OPERATIONAL_POLICY  
**Effective Date:** 2026-08-19  
**Owner:** Falcon Foundation  
**Project Owner Direction:** Once Falcon Foundation enters live sealed operation, future Applications SHALL be onboarded without modifying Foundation and without opening Foundation-directed FCRs for new Application needs.  
**Primary References:** Falcon Vision; Falcon Constitution; APP-001; CON-023; FDN-006; applicable accepted Foundation contracts and runtime semantics.  
**Authority Effect:** This policy constrains future onboarding and change behavior. It does not itself activate, deploy, admit, or authorize any Application.  
**Supersedes for live sealed onboarding:** Any FDN-006 wording that suggests a future Application may request Foundation modification through FCR when the issue is merely that the new Application does not fit the published Foundation contract.  

---

## 1. Purpose

This policy defines the permanent operating rule for adding new Applications after Falcon Foundation is declared live and sealed.

The central rule is:

> A future Application adapts to Falcon Foundation. Falcon Foundation does not adapt to a future Application.

The Foundation is therefore treated as a published operating substrate with stable contracts, admission semantics, authority boundaries, lifecycle behavior, resource governance, security rules, and runtime-hosting rules.

A future Application SHALL be accepted only if it can operate within those published rules.

---

## 2. Live Seal Invariant

After the Foundation live seal becomes effective:

```text
NEW_APPLICATION != FOUNDATION_CHANGE_REQUEST
NEW_APPLICATION != FOUNDATION_PATCH
NEW_APPLICATION != FOUNDATION_SPECIAL_CASE
NEW_APPLICATION != FOUNDATION_NAME_ALLOWLIST_ENTRY
NEW_APPLICATION != FOUNDATION_VERSION_ALLOWLIST_ENTRY
NEW_APPLICATION != FOUNDATION_RUNTIME_EXCEPTION
NEW_APPLICATION != FOUNDATION_AUTHORITY_EXPANSION
NEW_APPLICATION != FOUNDATION_SCHEMA_BYPASS
NEW_APPLICATION != FOUNDATION_SECURITY_BYPASS
NEW_APPLICATION != FOUNDATION_CONTRACT_REWRITE
```

No Application onboarding activity may modify Foundation source, Foundation runtime behavior, Foundation contracts, Foundation admission logic, Foundation resource semantics, Foundation security rules, or Foundation authority boundaries merely to accommodate that Application.

---

## 3. Future Application Compatibility Model

A future Application is compatible when it can truthfully satisfy the published Foundation requirements, including all applicable requirements from FDN-006 and CON-023.

The compatibility question is not:

> Can Foundation be changed to support this Application?

The compatibility question is:

> Can this Application be designed, packaged, declared, validated, admitted, registered, authorized, isolated, operated, updated, recovered, and removed using the already-published Foundation contract?

If the answer is YES, onboarding may proceed.

If the answer is NO, the Application is not eligible for Foundation hosting in its current design.

---

## 4. No Future Foundation FCR for New-Application Fit

Once Foundation is live and sealed, a new or changed Application SHALL NOT open an FCR asking Foundation to add, relax, reinterpret, bypass, or special-case a requirement for that Application.

Examples of prohibited future Foundation-directed requests include:

- add my Application name to an allowlist;
- accept my custom identity shape;
- accept my custom unsupported admission kind;
- skip a Manifest field for my Application;
- accept an undeclared dependency;
- add a private Foundation shortcut for my Application;
- bypass lifecycle attach eligibility;
- bypass resource grants;
- treat registration as activation;
- let my Application self-authorize;
- let my provider connection imply execution authority;
- store secret bytes directly because my provider SDK expects them;
- let my Application bypass FIL or governed transport;
- weaken fail-closed handling;
- add a Foundation business-domain rule for my Application;
- add a new Application-specific runtime exception;
- change Foundation because the Application architecture was designed incorrectly.

These are Application-design failures, not Foundation gaps in live sealed operation.

---

## 5. What a Future Application May Change

A future Application may change its own implementation, configuration, packaging, adapters, business logic, internal architecture, Application-owned capabilities, Application-owned data model, Application-owned provider integration, Application-owned presentation, and Application-owned orchestration as permitted by its own governance.

It may also use approved Shared Application capabilities where those already exist and where their contracts permit the intended use.

The Application may create adapters on its own side of the Foundation boundary when necessary to translate from its business model into the published Foundation contract.

The adapter SHALL NOT alter Foundation semantics.

```text
APPLICATION_ADAPTER -> FOUNDATION_CONTRACT
FOUNDATION_CONTRACT -/-> APPLICATION_SPECIAL_CASE
```

---

## 6. Unsupported Application Requirement

If a future Application requires behavior that the sealed Foundation does not provide, the default result is:

```text
APPLICATION_REQUIREMENT_NOT_SUPPORTED_BY_PUBLISHED_FOUNDATION
=> APPLICATION_MUST_REDESIGN_OR_REJECT_REQUIREMENT
=> FOUNDATION_REMAINS_UNCHANGED
```

The responsible Application team has only compliant choices:

1. redesign the Application to use existing Foundation contracts;
2. implement the need within the Application boundary when constitutionally and architecturally valid;
3. use an already-approved Shared Application capability when appropriate;
4. remove or defer the unsupported Application feature; or
5. reject the Application as incompatible with Falcon Foundation.

It SHALL NOT patch Foundation.

---

## 7. Foundation Completeness Interpretation

The Foundation is considered complete for future Application hosting when it provides a business-domain-neutral hosting substrate capable of hosting arbitrary Applications that conform to the published contract.

Completeness does NOT mean Foundation pre-implements every future business feature.

Foundation does not need to know in advance whether the future Application is:

- Trading;
- Accounting;
- Logistics;
- Medical;
- Research;
- AI orchestration;
- Web;
- Communication;
- Portfolio management;
- reporting;
- document processing;
- monitoring;
- another future business domain.

It only needs the generic hosting and governance substrate already defined and validated.

Therefore:

```text
FOUNDATION_COMPLETE_FOR_APPLICATION_HOSTING
!=
FOUNDATION_CONTAINS_EVERY_FUTURE_BUSINESS_CAPABILITY
```

The future business capability remains Application-owned unless a pre-existing Foundation contract explicitly defines otherwise.

---

## 8. Application-Neutrality Requirement

Future onboarding SHALL preserve Foundation neutrality.

Foundation SHALL NOT contain new business-domain branching such as:

```text
if application == "Accounting" ...
if application == "Trading" ...
if application == "Medical" ...
if applicationVersion == "X" then bypass ...
```

Application identity and version remain arbitrary governed values, subject to exact declaration and validation, not Foundation business-domain allowlisting.

---

## 9. Admission Remains Mandatory

The live seal does not make onboarding automatic.

Every future Application must still pass the existing governed path:

```text
APPLICATION DESIGN
    -> APPLICATION MANIFEST
    -> FOUNDATION VALIDATION
    -> ADMISSION DECISION
    -> CANONICAL ARTIFACT / LIFECYCLE / RESOURCE BINDING
    -> RUNTIME REGISTRATION
    -> SEPARATE ACTIVATION AUTHORITY
    -> ACTIVE ONLY WHEN ALL APPLICABLE AUTHORITY GATES PASS
```

The seal prohibits Foundation modification. It does not weaken Foundation admission.

---

## 10. Rejection Is a Correct Outcome

A rejected Application is not evidence that Foundation is broken.

Rejection is correct when the Application fails the published contract.

Examples:

- invalid identity;
- incomplete Manifest;
- bad provenance;
- unresolved dependency;
- unsupported Foundation reference;
- invalid authority request;
- invalid lifecycle evidence;
- missing resource grant;
- conflicting exclusive capability;
- forbidden secret handling;
- provider bypass;
- cross-Application authority leakage;
- self-granted authority;
- hidden transport side channel;
- inability to operate within resource ceilings;
- inability to contain failure;
- inability to support safe removal;
- inability to fail closed.

In each case, the Application must be corrected. Foundation remains unchanged.

---

## 11. No Silent Compatibility Exceptions

Operations, administrators, developers, owners, or automation SHALL NOT create an undocumented runtime exception merely because a new Application is important, urgent, profitable, experimental, or technically functional.

```text
URGENCY != AUTHORITY
BUSINESS_VALUE != FOUNDATION_EXCEPTION
TECHNICAL_SUCCESS != COMPLIANCE
OWNER_SILENCE != APPROVAL
LEGACY_BEHAVIOR != ENTITLEMENT
```

Any candidate that cannot meet the published gate stays outside the stronger state.

---

## 12. Human Onboarding Manuals

The authoritative human procedures for future Application onboarding SHALL be maintained in:

- `docs/manuals/FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_EN.md`
- `docs/manuals/FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_AR.md`

Those manuals explain how a human team designs and prepares a new Application to fit the sealed Foundation without modifying Foundation.

The manuals are operational guidance. FDN-006, FDN-007, CON-023, and the higher governing documents remain authoritative where applicable.

---

## 13. Future Application Decision Rule

A human onboarding team SHALL end its compatibility review with one of exactly three outcomes:

### `READY_FOR_FOUNDATION_ADMISSION`

Use when every mandatory applicable rule is satisfied and evidence is complete.

### `APPLICATION_REDESIGN_REQUIRED`

Use when the Application can likely be changed on its own side to satisfy the published Foundation contract.

### `INCOMPATIBLE_WITH_SEALED_FOUNDATION`

Use when the Application cannot satisfy the published Foundation contract without requiring a prohibited Foundation change.

There is no fourth outcome called `CHANGE_FOUNDATION_FOR_THIS_APPLICATION`.

---

## 14. Live Foundation Operational Rule

After seal:

```text
FOUNDATION = STABLE HOSTING SUBSTRATE
APPLICATION = VARIABLE CONSUMER

VARIABLE CONSUMER MUST FIT STABLE SUBSTRATE
STABLE SUBSTRATE MUST NOT MUTATE FOR VARIABLE CONSUMER
```

This is the intended long-term Falcon operating model.

---

## 15. Final Rule

The permanent onboarding principle is:

> Build the new Application around Falcon Foundation's published contracts. Never build a Foundation exception around the new Application.

In compact form:

```text
READ THE CONTRACT
DESIGN TO FIT
DECLARE COMPLETELY
VALIDATE EXACTLY
ADMIT EXPLICITLY
REGISTER WITHOUT ACTIVATING
AUTHORIZE SEPARATELY
ISOLATE BY DEFAULT
FAIL CLOSED
NEVER PATCH LIVE FOUNDATION FOR A NEW APPLICATION
```
