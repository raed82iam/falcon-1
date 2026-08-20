# FSATS Part 9 — Exact Executable Validation Evidence

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Status:** `PASS`  
**Exact executable Application candidate:** `a3dc731f06dbc290653bfac3ded14ddce326aa82`  
**Exact Foundation test snapshot:** `3e5977da254894afb29f39302cd7791612e44178`  
**.NET SDK:** `10.0.302`

## 1. Scope

This record captures the completed executable validation for FSATS Part 9 / FSTSimA Digital City after the first executable finding was remediated.

Part 9 remains a non-runtime validation scope. This evidence does not grant runtime activation, provider connectivity, broker connectivity, Paper, Shadow, Tiny-Live, Live, deployment or production-adoption authority.

## 2. Exact source identity

```text
APPLICATION EXPECTED HEAD = a3dc731f06dbc290653bfac3ded14ddce326aa82
APPLICATION ACTUAL HEAD   = a3dc731f06dbc290653bfac3ded14ddce326aa82
APPLICATION TRACKED TREE  = CLEAN

FOUNDATION TEST SNAPSHOT EXPECTED HEAD = 3e5977da254894afb29f39302cd7791612e44178
FOUNDATION TEST SNAPSHOT ACTUAL HEAD   = 3e5977da254894afb29f39302cd7791612e44178
FOUNDATION TEST SNAPSHOT MODE          = DETACHED / TEST ONLY
FOUNDATION TEST SNAPSHOT TRACKED TREE  = CLEAN
```

The Foundation snapshot is the same accepted Stage 12 executable snapshot previously used by the closed Part 8 executable validation and remains test-only evidence here.

## 3. Earlier exact-candidate retest

Fresh validation on the same exact Application candidate established:

```text
RESTORE = PASS
RELEASE BUILD = PASS
ARCHITECTURE = PASS
SECURITY = PASS
BEHAVIOR = PASS (40/40)
INTEGRATION = PASS (31/31)
FAILURE = PASS (12/12)
```

The first continuation then stopped before structural compatibility because the FoundationCompatibility verifier was invoked without its five required Foundation assembly arguments. This was a test-harness invocation defect, not a product or compatibility finding.

A subsequent environment-discovery attempt stopped because no suitable local Foundation checkout was present. No product failure was inferred.

## 4. Exact isolated Foundation test preparation

A detached, test-only Foundation copy was created under the Application test area at the exact accepted snapshot:

```text
FOUNDATION HEAD = 3e5977da254894afb29f39302cd7791612e44178
MODE = DETACHED / TEST ONLY
SDK = 10.0.302
```

Only the five assemblies required by the Application FoundationCompatibility verifier were built from that exact snapshot:

- `Foundation.Contracts.dll`
- `Foundation.State.dll`
- `Foundation.MessageRouting.dll`
- `Foundation.MessageDelivery.dll`
- `Foundation.EventSystem.dll`

Recorded SHA-256 digests:

```text
Foundation.Contracts.dll
2F74222F81F204A5DDF730CFD4677858111859E5A4759715C602BC8AEECCB4ED

Foundation.State.dll
6196DA10425BB3CA17ED97B7F83FE45D3CF68088C64A5EAA7E97B5A6C9ACA822

Foundation.MessageRouting.dll
2BA938AEDE2E93CE5A5173CBA50270E95E377EC606EFAB83E2D547866778709E

Foundation.MessageDelivery.dll
F73BA14272DD817D2CB4ECD225C4F38C41CAE0CCA150369887B96461FE294956

Foundation.EventSystem.dll
1C802A96A8A238CF1CA96E2B343C4D3554679AA01CC9275F31C16CA4EB367A01
```

## 5. Foundation structural compatibility

```text
FOUNDATION COMPATIBILITY VERIFIER = PASS (37/37)
ContractsAssembly = Foundation.Contracts
StateAssembly = Foundation.State
RoutingAssembly = Foundation.MessageRouting
DeliveryAssembly = Foundation.MessageDelivery
EventAssembly = Foundation.EventSystem
Scope = TEST_ONLY_STRUCTURAL_COMPATIBILITY / NO_RUNTIME_BINDING_AUTHORITY
```

This result proves only the verifier's structural compatibility scope. It does not complete or authorize the separately governed runtime/binding obligations in open FCRs.

## 6. Deterministic behavior rerun

```text
PART 4 LIFECYCLE ADVERSARIAL VERIFICATION = PASS
PART 5 HEALTH / READINESS ADVERSARIAL VERIFICATION = PASS
PART 6 CONFIGURATION / POLICY ADVERSARIAL VERIFICATION = PASS
FSATS BEHAVIOR VERIFIER = PASS (40/40)
```

The same exact Part 9 candidate therefore passed the deterministic behavior rerun after Foundation structural compatibility.

## 7. Final executable result

```text
FSATS PART 9 EXACT EXECUTABLE VALIDATION = PASS
RESTORE = PASS
RELEASE BUILD = PASS
ARCHITECTURE = PASS
SECURITY = PASS
BEHAVIOR / PART 9 = PASS (40/40)
INTEGRATION = PASS (31/31)
FAILURE = PASS (12/12)
FOUNDATION COMPATIBILITY = PASS (37/37)
BEHAVIOR DETERMINISTIC RERUN = PASS
APPLICATION TRACKED TREE = CLEAN
FOUNDATION TEST SNAPSHOT TRACKED TREE = CLEAN
```

## 8. Authority boundary

```text
TECHNICAL_PASS != OWNER_ACCEPTANCE
DIGITAL_CITY_RESULT != OPERATIONAL_TRUTH
SIMULATION_SUCCESS != RUNTIME_AUTHORITY
QUALIFICATION_RECOMMENDATION != PAPER_AUTHORITY
PAPER_QUALIFICATION_REVIEW != PAPER_ACTIVATION
FOUNDATION_STRUCTURAL_COMPATIBILITY != RUNTIME_BINDING_AUTHORITY
PART9_PASS != PROVIDER_CONNECTIVITY
PART9_PASS != BROKER_CONNECTIVITY
PART9_PASS != PAPER_AUTHORITY
PART9_PASS != LIVE_AUTHORITY
PART9_PASS != DEPLOYMENT
```

FCR-0008, FCR-0009, FCR-0011, FCR-0013, FCR-0014, FCR-0030, FCR-0012, FCR-0224/FCR-0226 and FCR-0082 remain separately governed according to their current issue bodies and are not silently completed by this Part 9 validation.

## 9. Result

```text
PART9_EXECUTABLE_VALIDATION = PASS
PART9_EXACT_EXECUTABLE_SOURCE = a3dc731f06dbc290653bfac3ded14ddce326aa82
PART9_OWNER_ACCEPTED_AND_CLOSED = NO
NEXT = FRESH POST_EXECUTABLE ARCHITECTURE / CONSISTENCY + BROAD RED TEAM
```
