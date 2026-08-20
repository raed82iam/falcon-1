# Stage 8 WP-01 — Implementation Design and Trace v1

**Work Package:** WP-01 — Guardian Runtime Primitives, Protective Mandate & Decision Evidence Model  
**Status:** IMPLEMENTED / PRE-EXECUTABLE-VALIDATION  
**Date:** 2026-08-14

## Implementation

Production project:

`src/Foundation.Guardian/Foundation.Guardian.csproj`

Production source:

`src/Foundation.Guardian/GuardianProtectionPrimitives.cs`

Executable verifier:

`verification/Falcon.Stage8.WP01.Verifier`

Architecture guard:

`tests/Falcon.Foundation.Architecture.Tests/Stage8Wp01ArchitectureGuard.cs`

## Production dependency boundary

`Foundation.Guardian -> Foundation.Contracts`

No direct dependency on Authority, Lifecycle, Recovery, SelfAwareness, Application or Web is introduced in WP-01.

## Implemented primitives

- GuardianProtectiveMode: NORMAL / HEIGHTENED / RESTRICTED / SAFE / RECOVERY_GUARD semantics.
- GuardianProtectiveAction: Observe / Warn / Restrict / Isolate / Suspend / RequestEmergencyStop.
- GuardianConsequenceClass: Low / Moderate / High / Critical.
- GuardianScopeKind: Component / Application / FoundationSubsystem / FalconWide.
- GuardianProtectiveDecision with exact target, scope, mode, action, consequence, trigger, evidence, authority, policy, reason, release-condition declaration and decision time.
- deterministic SHA-256 decision identity over all material fields.
- fail-closed validator.

## Fail-closed coverage

The validator rejects missing/malformed identities, evidence, authority/policy, trigger, reason/release conditions, invalid enum values, invalid time, NORMAL with restrictive action, and unjustified low/moderate Falcon-wide consequence scope.

## Explicit non-capabilities

WP-01 does not expose public methods that:

- grant/mint authority;
- execute lifecycle transitions;
- perform recovery;
- release a subject;
- restore trust;
- reintroduce a subject;
- perform Controlled Revival.

## Governing trace

- AUT-002 modes and protective actions are represented without transferring routine lifecycle or recovery ownership.
- AUT-001 remains the authority interpreter.
- SYS-002 remains transition owner.
- Stage 9 recovery/release remains out of scope.
- Stage 13 FSA-specific governance remains out of scope.
- FCR-0076/FCR-0082 Stage 8 scope is supported by the canonical primitives but not claimed complete by WP-01.

## Executable acceptance target

WP-01 verifier target: 12/12 PASS twice with identical output, stable binaries, Architecture PASS, Security PASS and predecessor Stage 7 cross-stage regression PASS.
