# Stage 8 WP-03 Implementation Design and Trace V1

Status: IMPLEMENTED_AWAITING_EXECUTABLE_VALIDATION

## Scope

WP-03 implements the Foundation-owned canonical protective restriction object and its validation/evaluation semantics.

Implemented production path:
`src/Foundation.Guardian/GuardianProtectiveRestriction.cs`

Verifier:
`verification/Falcon.Stage8.WP03.Verifier/`

## Preserved boundaries

- restriction is derived from an already valid Guardian protective decision;
- exact source decision ID and deterministic identity are bound into the restriction;
- target, scope, action, trigger, evidence, authority and policy must match the source decision;
- severity is derived from the source consequence class;
- only restrictive actions may create a restriction;
- restart persistence is mandatory;
- subject self-release is forbidden;
- optional review deadline is not automatic expiry or release;
- once review deadline is reached, status becomes REVIEW_REQUIRED while `RemainsEnforced = true`;
- WP-03 exposes no recovery, trust-restoration or release method;
- actual release/recovery/reintroduction remains Stage 9.

## FCR trace

FCR-0076 and FCR-0082 both identify WP-03 as a primary Stage 8 realization point for protective restriction semantics.

## Verification intent

WP-03 verifier contains 20 checks covering valid restriction construction, source/target/scope/severity/action binding, restart persistence, anti-self-release, deadline-without-release, deterministic identity, mutation sensitivity, authority/policy binding and Stage 9 boundary preservation.
