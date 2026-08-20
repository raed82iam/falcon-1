# EXT-001 — External Dependency Governance

**Identifier:** EXT-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-08-16  
**Activation Basis:** Project Owner full Stage 12 execution authorization on 2026-08-16  
**Owner:** Falcon External Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; SEC-001; SEC-002; AUT-001; IMP-001 Stage 12  
**Affected Domains:** Foundation external-access boundary and conforming Application consumers  
**Implementation Authority:** Granted only through the explicit Stage 12 Owner authorization; this Specification does not independently grant runtime external connectivity.

## 1. Purpose

EXT-001 defines the generic Foundation rules for deciding whether a governed Falcon principal may use an external destination. It establishes a fail-closed, Application-neutral external-access boundary without performing network I/O and without creating business, financial, trading, provider-selection or execution authority.

## 2. Core distinctions

```text
PUBLIC_ENDPOINT != UNRESTRICTED_EGRESS_AUTHORITY
SAME_URL != SAME_AUTHORITY
SAME_PROVIDER != SAME_AUTHORITY
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
TECHNICAL_EGRESS_AUTHORIZATION != BUSINESS_AUTHORITY
CREDENTIAL_REFERENCE != SECRET
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
RESEARCH_EGRESS != OPERATIONAL_PROVIDER_EGRESS
PRESENTATION_EGRESS != OPERATIONAL_PROVIDER_EGRESS
NON_LIVE != LIVE_AUTHORITY
```

## 3. Governed route identity

Every material external-access decision SHALL bind at least:

- requesting principal identity;
- service-role identity;
- environment;
- access purpose;
- exact destination identity;
- authentication mode;
- credential-reference identity when credentials are required;
- policy identity/version;
- observation time and validity interval;
- evidence/provenance reference.

Provider, provider-account or broker-account identity MAY be carried as additional route-scope dimensions when relevant, but Foundation SHALL NOT invent Application business identity.

## 4. Access-purpose classes

The generic v1 purpose classes are:

- `RESEARCH`;
- `NON_LIVE_VALIDATION`;
- `OPERATIONAL_PROVIDER_DATA`;
- `BROKER_EXECUTION`;
- `PRESENTATION_DATA`.

These are technical egress classes only. They do not define Application business behavior.

## 5. Authentication modes

The generic v1 authentication modes are:

- `PUBLIC`;
- `CREDENTIAL_REFERENCE`;
- `CHANNEL_DEPENDENT`.

A public endpoint still requires an explicit route authorization. Credential-reference authorization never exposes, stores or transports the secret value through ordinary Application state or this decision surface.

## 6. Normative requirements

- **EXT-001-REQ-001:** External access SHALL default to deny.
- **EXT-001-REQ-002:** An allow decision SHALL require one unambiguous active policy rule matching the exact governed route identity and purpose.
- **EXT-001-REQ-003:** Unknown, malformed, conflicted, expired, revoked or materially incomplete policy/evidence SHALL deny.
- **EXT-001-REQ-004:** Exact destination identity SHALL be preserved; broader host/provider similarity SHALL NOT imply authority.
- **EXT-001-REQ-005:** Principal, service-role, environment and purpose mismatches SHALL deny.
- **EXT-001-REQ-006:** A route requiring credentials SHALL deny when its governed credential reference is missing, mismatched, inactive, expired or revoked.
- **EXT-001-REQ-007:** Credential references SHALL be opaque identifiers and SHALL NOT contain secret/key/token/password material.
- **EXT-001-REQ-008:** `PUBLIC` authentication SHALL NOT accept a credential reference as a substitute for route policy.
- **EXT-001-REQ-009:** A `NON_LIVE_VALIDATION` principal/route SHALL NOT acquire or consume Live-only route or credential authority.
- **EXT-001-REQ-010:** Research routes SHALL remain separate from operational-provider, broker-execution and presentation routes.
- **EXT-001-REQ-011:** Presentation routes SHALL remain separate from operational-provider and broker-execution routes.
- **EXT-001-REQ-012:** Operational-provider routes SHALL remain separate from broker-execution routes.
- **EXT-001-REQ-013:** External route authorization SHALL be observational/decisional only and SHALL NOT perform HTTP, WebSocket, broker, provider or other network execution.
- **EXT-001-REQ-014:** A successful technical route decision SHALL NOT be represented as deployment, connection, trading, broker-execution, financial or business authority.
- **EXT-001-REQ-015:** Decisions SHALL be attributable, deterministic for equivalent governed inputs, and bind to a deterministic evidence identity.
- **EXT-001-REQ-016:** Zero-Application operation remains valid; the absence of Applications or external-route policies SHALL result in safe deny, not Foundation failure.
- **EXT-001-REQ-017:** Revocation or expiry SHALL take effect without relying on the constrained Application's cooperation.
- **EXT-001-REQ-018:** Foundation SHALL remain Application-neutral and SHALL NOT hard-code Trading, FSAPMA, Shared Web, FSTSimA, provider or broker business semantics into the generic evaluator.
- **EXT-001-REQ-019:** Exact downstream provider/broker/Web destinations MAY be verified as policy fixtures without becoming Foundation-owned provider catalogs.
- **EXT-001-REQ-020:** Stage 13 FSA-specific governance and any future FSA research mechanism SHALL remain outside this Stage 12 generic evaluator unless separately governed and authorized.

## 7. Decision outcome

An external-access evaluation SHALL produce an explicit `ALLOW` or `DENY`, a bounded reason, the effective route identity, policy identity/version when known, observation/expiry data, and deterministic evidence identity.

`ALLOW` means only that the exact technical egress route is authorized under the supplied current policy/evidence. It does not execute that route.

## 8. Failure behavior

Falcon SHALL deny external access when route identity, authority, evidence, credential-reference state, purpose separation, environment separation or policy validity cannot be established. Availability SHALL NOT be preserved by broadening a destination, reusing another principal's authority, silently converting public reachability into permission, or accepting secret material in place of a governed reference.

## 9. Acceptance evidence

Stage 12 verification SHALL cover at least:

- default deny and unknown-route rejection;
- exact principal/service-role/environment/purpose/destination matching;
- public endpoint still requiring explicit authorization;
- credential-reference presence, scope, expiry and revocation;
- same URL/provider not implying shared authority;
- non-Live versus Live isolation;
- research/provider/presentation/broker-execution separation;
- ambiguous/conflicting policy fail-closed behavior;
- deterministic decision/evidence identity;
- absence of network/execution surfaces;
- absence of secret-value public surfaces;
- zero-Application validity; and
- absence of Stage 13 FSA-specific leakage.

## 10. Non-authority

This Specification does not authorize any external network connection, deployment, market-data use, broker login, order, execution, financial action, secret provisioning, Application runtime activation or Stage 13 work. Those remain separately governed.
