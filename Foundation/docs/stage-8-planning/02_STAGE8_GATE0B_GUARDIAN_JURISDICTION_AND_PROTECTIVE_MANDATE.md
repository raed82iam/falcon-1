# Stage 8 Gate 0B — Guardian Jurisdiction and Protective Mandate Reconciliation

**Stage:** 8  
**Status:** COMPLETE_FOR_IMPLEMENTATION_ENTRY  
**Date:** 2026-08-14  
**Branch:** `foundation-development`

## 1. Purpose

This gate fixes the Stage 8 technical protection boundary before source implementation.

## 2. Foundation Guardian jurisdiction

Foundation Guardian owns bounded Falcon-wide technical protection under approved authority. It may evaluate protective conditions and create attributable protective decisions/restrictions within declared protective scope.

Foundation Guardian does not own Application business/domain meaning, trading intelligence, financial optimization, ordinary lifecycle ownership, recovery success, or FSA-specific governance.

## 3. Mandatory separation

```text
GUARDIAN_PROTECTIVE_DECISION != AUTHORITY_GRANT
GUARDIAN_PROTECTIVE_DECISION != LIFECYCLE_TRANSITION
GUARDIAN_PROTECTIVE_DECISION != RECOVERY_SUCCESS
UI_CLICK != AUTHORIZATION
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
HEALTH != AUTHORITY
FITNESS != AUTHORITY
```

- AUT-001 remains the authority interpreter.
- SYS-002 Lifecycle remains transition owner.
- Guardian imposes lawful protective constraints and requests governed protective transitions.
- Stage 9 owns recovery validation, release and reintroduction.
- Stage 13 owns FSA-specific governance, investigation, monitoring, Factory Reset and FSA-specific Controlled Revival.

## 4. Protective mandate model

Stage 8 shall preserve the AUT-002 mode semantics:

- NORMAL
- HEIGHTENED
- RESTRICTED
- SAFE
- RECOVERY_GUARD

Stage 8 shall support proportionate technical protection including warning, restriction, isolation, suspension and emergency-stop intent within lawful scope.

## 5. Consequence and scope rules

A protective decision must bind at minimum:

- exact decision identity;
- exact target identity;
- target scope;
- protection mode/action;
- consequence/severity class;
- triggering condition;
- evidence identity;
- governing authority/policy reference;
- reason;
- decision time;
- expected release conditions.

Containment shall prefer the narrowest trustworthy safe scope. When propagated damage or blast radius cannot be excluded, containment must expand rather than optimistically assume locality.

## 6. Release boundary

Stage 8 may record release preconditions and block unauthorized/self release, but Stage 8 shall not claim recovery success or perform recovery reintroduction.

Material release requires competent authority plus required independent recovery evidence. Passage of time, restart, source reappearance, prior success or subject self-attestation are insufficient.

## 7. Guardian compromise boundary

AUT-002 requires Guardian to remain independently interruptible/correctable and a compromised Guardian to be isolatable without silently removing all independent protection.

Stage 8 shall therefore include explicit Guardian-compromise containment and independent emergency-control coverage before closure.

## 8. FCR mapping

- FCR-0076 Stage 8 scope: generic Owner emergency containment/protective-control/Safe-State plane.
- FCR-0082 Stage 8 scope: generic AI/component/Application containment and unaffected-scope safety continuity.
- Stage 9 residual: recovery/release/reintroduction.
- Stage 13 residual: FSA-specific governance/investigation/recovery.

## 9. Gate result

```text
GATE0B_RESULT = PASS
FOUNDATION_GUARDIAN_JURISDICTION = FIXED_FOR_STAGE8
PROTECTIVE_MANDATE = FIXED_FOR_STAGE8
AUT001_BOUNDARY = PRESERVED
SYS002_BOUNDARY = PRESERVED
STAGE9_BOUNDARY = PRESERVED
STAGE13_BOUNDARY = PRESERVED
APPLICATION_BUSINESS_AUTHORITY = NOT_ACQUIRED
```
