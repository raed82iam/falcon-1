# Stage 12 Implementation Plan and Pre-Implementation Red Team

**Stage:** 12 — Governed External Access, Egress and Credential-Reference Security  
**State:** IMPLEMENTATION IN PROGRESS / PRE-IMPLEMENTATION REVIEW COMPLETE  
**Date:** 2026-08-16  

## 1. Implementation plan

Stage 12 is implemented as one generic subordinate enforcement capability inside existing `Foundation.Authority`, consuming AUT-001 authority truth rather than creating a second authority system.

Work packages:

1. **WP-01 — Specification definition gate:** activate EXT-001 v1.0 for the previously planned external-dependency-governance subject.
2. **WP-02 — Exact route identity:** bind principal, service role, environment, purpose, exact destination, authentication mode and required authority scope.
3. **WP-03 — Authority consumption:** require a current matching AUT-001 `AuthorityResult = ALLOW`; reject missing, denied, mismatched or expired authority.
4. **WP-04 — Credential-reference security:** validate opaque route-bound credential references, expiry and revocation without secret-value exposure.
5. **WP-05 — Purpose/environment separation:** preserve research, non-Live validation, operational-provider, broker-execution and presentation boundaries.
6. **WP-06 — Deterministic evidence:** produce deterministic route-decision identity and explicit constraints.
7. **WP-07 — No network execution:** expose evaluation only; no HTTP/WebSocket/provider/broker connector exists in Stage 12.
8. **WP-08 — Current FCR fixture coverage:** verify all exact current Shared-Web destinations remain representable without making them Foundation product truth.
9. **WP-09 — Adversarial verifier:** default deny, identity mismatches, credential attacks, ambiguity, revocation, determinism, zero-Application and Stage 13 leakage checks.
10. **WP-10 — Integrated validation:** controlled-solution build, Architecture, Security, Stage 11 regression, Stage 12 verifier and deterministic rerun.

## 2. Pre-implementation Architecture/Consistency review

Result: `PASS_WITH_REQUIRED_GUARDS`.

Preserved ownership:

- AUT-001 remains generic authority owner.
- SEC-001/SEC-002 remain security/trust owners.
- Stage 12 acts only as an exact external-route subordinate enforcement point.
- Applications retain provider selection, broker/account business meaning, market/trading semantics and customer mapping.
- exact downstream destination fixtures do not become a Foundation provider catalog.
- Stage 12 does not establish connections.

## 3. Pre-implementation Red Team

### RT12-001 — Public endpoint treated as permission
Attack: use a credential-free/public URL without an explicit route rule.
Guard: explicit exact route rule remains mandatory.

### RT12-002 — Same provider/URL authority bleed
Attack: reuse one principal's route for another principal, service role or purpose.
Guard: exact identity matching; mismatches deny.

### RT12-003 — Non-Live to Live escalation
Attack: a non-Live validation consumer attempts to acquire a Live route/credential.
Guard: environment is part of exact route and credential identity; mismatch denies.

### RT12-004 — Research becomes operational data/execution
Attack: research authorization is reused for provider or broker execution traffic.
Guard: technical purpose is exact identity; cross-purpose reuse denies.

### RT12-005 — Web presentation becomes FSAPMA truth
Attack: presentation route is reused as operational-provider authority.
Guard: purpose/principal/service-role separation; Foundation does not merge the routes.

### RT12-006 — Credential secret smuggled as reference
Attack: plaintext token/key/password is put into the credential-reference field.
Guard: reference metadata is validated and secret-like reference material fails closed; no secret-value property exists on the public credential-reference type.

### RT12-007 — Revoked/expired credential survives
Attack: stale credential reference is reused after revocation or expiry.
Guard: current observation must be within effective interval and `IsRevoked=false`.

### RT12-008 — AUT-001 bypass
Attack: route rule alone is treated as authority.
Guard: a matching current AUT-001 `ALLOW` result and exact authority decision/scope binding are mandatory.

### RT12-009 — Ambiguous duplicate policy
Attack: two exact route rules compete and evaluator chooses a convenient one.
Guard: duplicate exact matches fail closed as `POLICY_AMBIGUOUS`.

### RT12-010 — Connectivity side effect hidden inside evaluator
Attack: evaluation performs an HTTP/WebSocket/provider/broker call.
Guard: Stage 12 evaluator is pure decision logic; verifier rejects network/execution public method surface.

### RT12-011 — Provider catalog ownership creep
Attack: exact FCR destinations become permanent Foundation market/provider truth.
Guard: destinations are policy/verification fixtures only; Applications own their business/provider catalogs.

### RT12-012 — Stage 13 FSA leakage
Attack: Stage 12 becomes a direct FSA Internet/recovery/control-plane path.
Guard: FSA-specific investigation, Monitor AI, Factory Reset, remediation sandbox and Controlled Revival are excluded and verifier checks absence of Stage 13 names in Stage 12 external-access surface.

### RT12-013 — Determinism manipulated by policy ordering
Attack: rule order changes decision identity.
Guard: only one exact matching rule is accepted; unrelated-rule reorder does not change the decision/evidence identity.

### RT12-014 — Zero-Application invalidated
Attack: Foundation assumes an Application or external route must exist.
Guard: no request/no policy produces safe deny and remains a valid Foundation state.

## 4. Required executable evidence

Before Stage 12 can be called technically complete:

- exact candidate pin;
- .NET SDK 10.0.302;
- controlled solution Restore PASS;
- Release Build PASS;
- Architecture PASS;
- Security PASS;
- Stage 11 regression PASS;
- Stage 12 verifier PASS twice;
- exact deterministic Stage 12 rerun;
- required output markers present;
- clean tracked worktree;
- remote candidate unchanged.

## 5. Pre-test conclusion

`PRE_IMPLEMENTATION_ARCHITECTURE = PASS_WITH_REQUIRED_GUARDS`

`PRE_IMPLEMENTATION_RED_TEAM = PASS_TO_EXECUTABLE_VALIDATION`

No Stage 12 technical PASS or FCR completion is claimed before executable validation.
