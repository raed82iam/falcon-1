# Stage 5 WP-04 — Implementation Boundary

**Status:** Active implementation boundary  
**Authority:** `Stage5-WP04-Implementation-Authorization-20260807-205500`  
**Branch:** `foundation-development`

## In scope

WP-04 implements only the bounded FIL validation and message-admission decision surface.

The production assembly is:

- `Foundation.MessageAdmission`

The admission evaluator may consume accepted predecessor surfaces to determine whether a canonical WP-01 message is admissible at the WP-04 boundary.

It may:

- validate the accepted WP-01 `CanonicalFilEnvelope`;
- consume explicit typed producer, Application, Manifest, recipient-scope, intended-consumer, authority, and evidence bindings;
- resolve the exact WP-03 Application Communication Manifest;
- verify explicit producer identity and producer Application binding;
- verify explicit recipient-scope binding and declared intended-consumer membership;
- require exactly one applicable communication declaration;
- verify message type, kind, classification, producer direction/role, and schema declaration consistency;
- resolve exact schema versions through the accepted WP-02 registry;
- use explicit accepted WP-02 compatibility rules when received and declared schema versions differ;
- reject unknown, retired, unresolved, or incompatible schema use;
- consume an explicit `AuthorityResult` binding from the accepted Stage 4 authority surface without creating a second authority engine;
- require exact authority-reference, producer, Application, recipient-scope, admission-purpose, effective-scope, and provenance binding evidence;
- reject malformed, denied, not-yet-effective, expired, mismatched, or unverifiable authority input;
- evaluate message expiry against an explicitly supplied deterministic UTC observation time;
- emit only an immutable `ADMITTED` or `REJECTED` decision with an exact canonical reason;
- bind material decision inputs into a deterministic SHA-256 decision identity; and
- preserve payload opacity by binding the accepted WP-01 envelope digest instead of copying or interpreting payload meaning.

## Required production dependencies

`Foundation.MessageAdmission` may depend only on:

- `Foundation.Contracts` for WP-01 canonical messaging primitives and accepted contract validation;
- `Foundation.SchemaRegistry` for WP-02 schema resolution and compatibility;
- `Foundation.ApplicationManifest` for WP-03 Manifest resolution and communication declarations; and
- `Foundation.Authority` for the accepted Stage 4 authority decision surface and constants.

No reverse dependency from an accepted predecessor to `Foundation.MessageAdmission` is permitted.
No Application project reference is permitted.
No Service Bus runtime project or later Stage 5 implementation may be referenced.

## Explicit typed admission context

WP-04 does not derive Application identity, producer ownership, recipient ownership, consumer identity, or authority scope by parsing naming conventions.

The caller supplies explicit typed bindings.

### MessageProducerBinding

`MessageProducerBinding` binds:

- exact WP-01 `ProducerIdentityReference`;
- producer `ApplicationIdentityReference`;
- producer `ManifestIdentity`; and
- producer-binding evidence.

Admission fails closed if the producer binding is absent, does not exactly match the envelope producer identity, resolves an unknown Manifest, or identifies an Application different from the resolved Manifest owner.

### MessageRecipientBinding

`MessageRecipientBinding` binds:

- exact WP-01 `RecipientScopeReference`;
- declared WP-03 `ManifestReference` representing the intended consumer; and
- recipient-binding evidence.

Admission fails closed if the recipient binding is absent, differs from the envelope recipient scope, or identifies a consumer that is not declared exactly once in the resolved Manifest.

This is admission evidence only. It does not create, discover, resolve, select, or activate a route.

### MessageAdmissionContext

The context additionally carries:

- producer Manifest version;
- deterministic UTC observation time;
- optional explicit `MessageAuthorityBinding`; and
- admission evidence/provenance reference.

The context supplies attributable evidence. It does not create authority.

## Authority binding rule

Stage 4 remains the sole owner of authority evaluation. WP-04 does not reinterpret or replace `DefaultDenyAuthorityEngine`.

The accepted `AuthorityResult` does not expose every actor/purpose relationship that WP-04 must prove for an exact message admission. The correct fail-closed response is explicit typed binding evidence rather than inferred string conventions.

`MessageAuthorityBinding` therefore binds:

- the exact WP-01 `AuthorityReference`;
- the accepted Stage 4 `AuthorityResult`;
- authorized producer `ProducerIdentityReference`;
- authorized producer `ApplicationIdentityReference`;
- authorized `RecipientScopeReference`;
- canonical WP-04 purpose `fil-message-admission`;
- the effective scope attributed to that accepted `AuthorityResult`; and
- binding provenance/evidence.

The evaluator requires:

1. message authority reference equals the bound authority reference;
2. bound producer identity equals the already-validated producer binding;
3. bound Application identity equals the already-validated Application binding;
4. bound recipient scope equals the already-validated recipient binding;
5. bound purpose equals `fil-message-admission`;
6. `AuthorityResult` passes the accepted CON-002 validator;
7. bound effective scope exactly equals `AuthorityResult.EffectiveScope`;
8. decision is explicit `ALLOW`;
9. authority is effective at the supplied observation time; and
10. authority has not expired.

Any mismatch fails closed with a distinct canonical reason.

The authority-binding inputs and evidence are material inputs to the deterministic WP-04 decision identity, preventing silent reuse of one admission decision across a different authority context.

## Implementation-review remediations

Two material gaps were identified and corrected before validation or Owner acceptance.

### Remediation A — producer/recipient contextual binding

An early draft could pair a valid Application/Manifest context with an envelope carrying a different producer or recipient identity.

Remediation added `MessageProducerBinding`, `MessageRecipientBinding`, exact envelope equality checks, intended-consumer declaration checks, fail-closed reasons, and decision-identity binding.

### Remediation B — authority subject/purpose/scope binding

A later static review identified that an otherwise valid Stage 4 `ALLOW` result could not by itself prove that it belonged to the exact WP-04 producer/Application/recipient/purpose context.

Remediation hardened `MessageAuthorityBinding` with explicit attributable subject, purpose, and effective-scope evidence and exact fail-closed comparison against the already-validated message context and accepted `AuthorityResult`.

Neither remediation creates authority, parses Application names, creates a route, or implements transport.

## Decision semantics

`MessageAdmissionDecision.Admitted` means only:

> the supplied canonical message passed the bounded WP-04 admission prerequisites under the supplied deterministic evidence and context.

It does not mean:

- a route exists;
- a destination is reachable;
- dispatch or queueing occurred;
- delivery occurred;
- a consumer accepted the message;
- a command executed;
- an event became authoritative truth;
- a business outcome succeeded; or
- deployment/runtime activation is authorized.

## Explicitly out of scope

WP-04 shall not implement:

- WP-05 dynamic route creation, route selection, Service Bus routing, or per-Application route isolation;
- WP-06 dispatch, queueing, acknowledgement, retry execution, duplicate-effect suppression, replay delivery, ordering execution, backpressure, flow control, or dead-letter transport;
- WP-07 publication/subscription execution, event journal execution, or event publication;
- WP-08 cryptographic message protection, signing, encryption, secure-channel, key custody, rotation, or algorithm implementation;
- WP-09 Application installation, activation, attachment, upgrade, replacement, draining, detachment, removal, packaging/consumption activation, or lifecycle transition execution;
- WP-10 integrated VPL-004 closure;
- FSA, MSA, LSA, CSA, Guardian, or recovery-governance implementation;
- Application business payload parsing or interpretation;
- modification of accepted Stage 0 through Stage 4 semantics;
- modification of accepted WP-01, WP-02, or WP-03 semantics;
- modification under `applications/**`, `reference/**`, `application-development`, `reference/fsats-v1.3-scratch`, or `main`; or
- deployment, runtime activation, or baseline activation.

## Architecture gate

The Foundation architecture gate recognizes `Foundation.MessageAdmission` as a permanent production assembly only with these four approved dependency edges:

- `Foundation.Contracts`;
- `Foundation.SchemaRegistry`;
- `Foundation.ApplicationManifest`; and
- `Foundation.Authority`.

The Stage 5 WP-04 verifier must also be present exactly once in the controlled solution with only its approved verification references.

The architecture change is additive. No previous architecture check was removed or weakened.

## Security and CI gates

The Foundation security gate recursively scans `src`, `tests`, and `verification`, so WP-04 production source/project and verifier source/project are governed automatically.

WP-04 introduces no external endpoint, secret, key material, cryptographic implementation, hidden wall clock, or network transport requirement.

The Foundation CI workflow executes WP-04 after the accepted Stage 5 WP-01 through WP-03 verifiers. CI does not replace the final deterministic validation/evidence run required for Owner acceptance.

## Workstream

All WP-04 implementation occurs only on `foundation-development`.

WP-05 through WP-10 and Stage 6 through Stage 9 remain unauthorized.
