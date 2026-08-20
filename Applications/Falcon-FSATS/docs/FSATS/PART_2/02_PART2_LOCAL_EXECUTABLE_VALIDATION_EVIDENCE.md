# FSATS Part 2 — Local Executable Validation Evidence

**Status:** `EXECUTABLE_VALIDATION_PASS / FOUNDATION_BINDING_FOLLOW_UP_REQUIRED`  
**Validated Application Commit:** `b58ffa419d9ed0905d920a8e4c4a868b39758da4`  
**Validation Date:** `2026-08-14`  
**Environment:** Project Owner local Windows validation environment  
**.NET SDK:** `10.0.302`  
**Runtime / Provider / Broker / Paper / Live Authority:** `NOT_GRANTED`

## 1. Evidence Source

The Project Owner executed the governed Part 2 local validation harness against an exact clean checkout at:

```text
C:\falcon\Application test
```

The validation result package was generated at:

```text
C:\falcon\Application test-results\20260814-132855.zip
```

The Application checkout remained clean after validation.

## 2. Exact Results

```text
FOUNDATION RESTORE = PASS
FOUNDATION RELEASE BUILD = PASS
APPLICATION RESTORE = PASS
APPLICATION RELEASE BUILD = PASS
```

Application verifier run 1:

```text
Architecture = PASS
Security = PASS
Behavior = 18 / 18 PASS
Integration = 31 / 31 PASS
Failure = 12 / 12 PASS
APPLICATION VERIFIERS = 5 / 5 PASS
```

Application verifier run 2 reproduced the same result:

```text
Architecture = PASS
Security = PASS
Behavior = 18 / 18 PASS
Integration = 31 / 31 PASS
Failure = 12 / 12 PASS
APPLICATION VERIFIERS = 5 / 5 PASS
```

Verified structural counts:

```text
SOURCE PROJECTS = 30
APPLICATIONS = 5
PROJECT ROLES PER APPLICATION = 6
MSA = 5
LSA = 34
CSA = 7
P1-K CONTRACT FAMILIES = 22
```

Security verifier result:

```text
126 source files checked
no secret literals detected
no unauthorized direct network primitives detected
```

## 3. What This Evidence Establishes

This evidence establishes an executable Application-owned Part 2 baseline for the exact validated commit:

- exact SDK restore/build succeeds;
- accepted 30-project topology compiles;
- Application architecture enforcement passes;
- security baseline passes;
- deterministic business behavior fixtures pass;
- cross-Application declaration/contract integration fixtures pass;
- composite failure/Kill/reconciliation/resource/replay fixtures pass;
- the verifier suite is repeatable across two consecutive runs;
- validation does not mutate the governed Application checkout.

## 4. Explicit Limitation Discovered During Follow-Up

The Foundation build executed in this validation came from the Foundation snapshot present on `application-development` at the validated commit. It is not, by itself, proof of compatibility with the later/current `foundation-development` branch state.

Fresh post-validation reconciliation on 2026-08-14 identified current Foundation HEAD:

```text
foundation-development = 0783337f84707c024b7a18f09be60c3c7fc5cdd4
```

Current Foundation includes later Stage 6 resource-governance implementation artifacts, including:

```text
src/Foundation.Contracts/ResourceGovernancePrimitives.cs
src/Foundation.State/ApplicationResourceStateProjectionGovernance.cs
src/Foundation.State/ResourceAdditionalRequestGovernance.cs
src/Foundation.State/ResourcePressureGovernance.cs
```

Therefore this evidence SHALL NOT be used to claim final exact Foundation resource binding compatibility or to close FCR-0010/FCR-0031 by implication.

## 5. FCR Disposition After This Evidence

The following implementation holds remain open pending exact Foundation binding verification:

```text
FCR-0004
FCR-0005
FCR-0006
FCR-0010
FCR-0031
```

This record does not close those FCRs and creates no runtime authority.

## 6. Next Governed Verification

Part 2 shall separately validate the Application-side binding layer against an immutable exact current Foundation reference without copying/forking Foundation source into `applications/**` and without treating a moving branch head as a production dependency.

Canonical cross-workstream build-time artifact consumption remains separately governed by FCR-0016 / future Stage 14 and is not fabricated by this evidence.
