# Stage 5 WP-08 — Pre-Implementation Scope and FCR Review

**Work Package:** Stage 5 WP-08 — Cryptographic Message Protection  
**Branch:** `foundation-development`  
**Review Date:** 2026-08-08  
**Predecessor State:** Stage 5 WP-01 through WP-07 `ACCEPTED_AND_CLOSED`  
**Later Work:** WP-09 and WP-10 `UNAUTHORIZED`

## 1. Purpose

This review fixes the bounded WP-08 implementation scope before production changes. It does not by itself grant implementation authority.

WP-08 exists to add Application-neutral cryptographic protection to already-governed Foundation communication objects and contexts without changing business meaning, authority, routing, delivery, event truth, lifecycle, or Application ownership.

## 2. Governing Security Requirements

SEC-001 v1.1 requires, among other controls:

- mutually authenticated encrypted transport for material communication crossing a security boundary;
- authorized message-level authenticated encryption where transport protection alone is insufficient for the required confidentiality boundary;
- protection binding identity, intended recipient or scope, classification, and material routing context where substitution could cause harm;
- governed key-class ownership and lifecycle;
- no secret/private key material in source code, ordinary configuration, messages, logs, verification evidence, or uncontrolled copies;
- rejection of unknown, prohibited, deprecated, downgraded, integrity-failed, wrong-recipient, expired, or revoked cryptographic contexts;
- fail-closed behavior with no silent plaintext fallback;
- governed replaceable cryptographic profiles; and
- no custom cryptographic algorithms or protocols.

SEC-002 further requires stable Trust Object identity, provenance, integrity verification, canonical representation where cryptographic identity/signing depends on representation, and bounded reliance.

## 3. WP-08 Owned Scope

WP-08 SHALL be limited to the Foundation-neutral cryptographic message-protection boundary for governed communication artifacts and contexts. The implementation may provide:

1. a typed cryptographic protection profile identity and version;
2. approved-profile allowlisting / prohibited-profile rejection;
3. message-protection context identity;
4. key-reference identity and key-state metadata consumption without exposing key bytes;
5. authenticated-encryption protection and verification using approved platform cryptography;
6. protected-context binding to canonical message identity/digest, producer identity, recipient/scope, classification, and material route/delivery/event context as applicable;
7. nonce/IV uniqueness enforcement or validation appropriate to the selected approved primitive;
8. integrity/authentication failure rejection;
9. wrong-recipient / wrong-scope / wrong-context rejection;
10. expired, not-yet-valid, revoked, disabled, unknown, prohibited, deprecated, or downgraded context rejection;
11. explicit plaintext/not-protected state only where an approved policy says protection is not required, never as fallback from required cryptographic protection;
12. deterministic protection/verification evidence identities that do not contain secret/private key material;
13. redaction-safe cryptographic diagnostics/evidence;
14. fail-closed behavior when required protection cannot be established or verified; and
15. provider/profile replaceability without changing protected business meaning.

## 4. Explicit Non-Scope

WP-08 SHALL NOT implement or authorize:

- custom cryptographic algorithms or protocols;
- a complete enterprise KMS/HSM, secret vault, certificate authority, PKI enrollment service, or external credential-management platform;
- raw key generation/custody/distribution storage infrastructure beyond bounded key-reference/status contracts needed for message protection;
- Internet egress or destination policy enforcement;
- Live broker credential acquisition or non-Live egress guards;
- Application package installation, activation, update, replacement, draining, detachment, or removal;
- WP-09 Plug-and-Play lifecycle execution;
- WP-10 integrated Stage 5 closure;
- route selection, delivery semantics, or event truth reinterpretation already owned by WP-05/WP-06/WP-07;
- business authority, business success, financial meaning, or trading-specific behavior;
- deployment, runtime activation, baseline activation, external connectivity, broker access, market-data access, or financial activity; or
- Stage 6 through Stage 9 implementation.

## 5. FCR-0004 through FCR-0011 Review

All currently open FCRs were reviewed before WP-08 implementation.

### FCR-0004 — Guardian governed protection command route

**WP-08 relevance:** `LIMITED_CROSS_CUTTING`.

WP-08 may protect already-admitted authoritative command traffic cryptographically. It does not own command semantics, authority, target scope, expiry, or operational/replay classification. Those remain in their established owners.

### FCR-0005 — FSAPMA operational market-data delivery

**WP-08 relevance:** `LIMITED_CROSS_CUTTING`.

WP-08 may protect sensitive governed delivery content and bind producer/consumer/scope/context. It does not own freshness, quality/confidence, provenance semantics, provider behavior, or market-data meaning.

### FCR-0006 — event evidence and replay delivery

**WP-08 relevance:** `LIMITED_CROSS_CUTTING`.

WP-08 may protect already-governed event/evidence objects. It does not own event identity, replay truth, correction semantics, publication/subscription truth, or journal ownership, which are WP-07 concerns already technically satisfied on the Foundation side.

### FCR-0007 — Foundation resource escalation request boundary

**WP-08 relevance:** `OUT_OF_SCOPE` except generic protection if such a governed message later uses the communication stack.

Resource request/decision semantics and resource authority remain outside WP-08.

### FCR-0008 — research-only Internet egress

**WP-08 relevance:** `OUT_OF_SCOPE`.

Cryptographic message protection does not create Internet egress permission, destination policy, awareness identity authorization, or operational/research path separation. This FCR remains owned by a later/other security-egress boundary.

### FCR-0009 — latency deadline and QoS transport

**WP-08 relevance:** `LIMITED_CROSS_CUTTING`.

WP-08 must not silently erase or reinterpret governed deadline/QoS context when that context is cryptographically bound, but it does not own queueing, backpressure, service levels, technical priority, or latency guarantees.

### FCR-0010 — resource pressure and load-shedding signals

**WP-08 relevance:** `OUT_OF_SCOPE` except generic protection of already-governed messages.

WP-08 does not create resource truth, allocation telemetry, request outcomes, or redistribution authority.

### FCR-0011 — non-Live isolation and egress guard

**WP-08 relevance:** `OUT_OF_SCOPE`.

Message protection cannot establish non-Live-only authority, prevent Live credential acquisition, or enforce broker/endpoint egress isolation. Replay/test classification may be cryptographically bound if already established by predecessor contracts, but WP-08 may not invent that classification or its authority.

## 6. Architectural Constraints

- Foundation remains valid with zero Applications.
- No Application receives privileged cryptographic semantics.
- Payload/business meaning remains opaque to Foundation.
- Cryptographic verification establishes only the cryptographic protection claim. It does not prove business truth, authority, correctness, admission, routing, delivery, event truth, or successful Application action.
- Existing WP-04/WP-05/WP-06/WP-07 authority and evidence boundaries remain authoritative for their own concerns.
- Cryptographic evidence must not leak secret/private key material.
- Any future real key-management service remains separately governed unless explicitly included by a later Owner authorization.

## 7. Pre-Implementation Disposition

```text
STAGE5_WP08_SCOPE_REVIEW = COMPLETE
FCR_0004 = LIMITED_CROSS_CUTTING
FCR_0005 = LIMITED_CROSS_CUTTING
FCR_0006 = LIMITED_CROSS_CUTTING
FCR_0007 = OUT_OF_SCOPE_WP08
FCR_0008 = OUT_OF_SCOPE_WP08
FCR_0009 = LIMITED_CROSS_CUTTING
FCR_0010 = OUT_OF_SCOPE_WP08
FCR_0011 = OUT_OF_SCOPE_WP08
WP08_SCOPE = CRYPTOGRAPHIC_MESSAGE_PROTECTION_ONLY
WP09_WP10 = UNAUTHORIZED
```

No FCR is closed by this review. Any closure continues to require the protocol defined in FCR Issue #1, including required Application verification where applicable.
