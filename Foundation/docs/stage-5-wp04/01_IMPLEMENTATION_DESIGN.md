# Stage 5 WP-04 — Bounded Implementation Design

**Work Package:** Stage 5 WP-04 — FIL Validation and Message Admission  
**Status:** IMPLEMENTATION DESIGN ACTIVE  
**Branch:** `foundation-development`

## 1. Design Goal

Implement one deterministic application-neutral admission decision boundary that composes accepted Foundation owners without replacing any of them.

```text
Canonical WP-01 Message
        |
        v
Structural Validation (WP-01)
        |
        v
Explicit Admission Context
        |
        +--> Producer Application / Manifest identity
        +--> deterministic observation time
        +--> explicit AuthorityResult
        +--> evidence reference
        |
        v
WP-03 Manifest Resolution + Communication Binding
        |
        v
WP-02 Schema Resolution / Compatibility
        |
        v
Stage 4 Authority Binding
        |
        v
WP-04 Admission Decision
        |
        +--> ADMITTED
        `--> REJECTED:<canonical reason>

NO route creation
NO dispatch
NO delivery
NO execution
```

## 2. No Implicit Identity Parsing

WP-01 exposes typed producer and recipient identities. WP-03 exposes typed Application and Manifest identities.

The accepted baseline does not establish a canonical rule that permits WP-04 to derive an Application identity by parsing a producer-identity string or recipient-scope string.

WP-04 therefore SHALL NOT infer Application ownership by string-prefix parsing, delimiter conventions, substring matching, or Application-name heuristics.

Instead, the admission caller shall provide an explicit typed admission context identifying the producer Application and the Manifest identity/version against which the message is to be admitted. WP-04 then resolves that Manifest and verifies its explicit Application binding.

This preserves:

- typed identity separation;
- Application neutrality;
- deterministic behavior;
- no hidden coupling;
- no Foundation knowledge of Application internal naming conventions.

Future routing may map admitted recipient scopes to routes under WP-05. WP-04 shall not perform that mapping.

## 3. Proposed Production Boundary

A dedicated Foundation project is appropriate because WP-04 has a distinct accepted responsibility and shall not mutate WP-01, WP-02, WP-03, or Stage 4 ownership.

Proposed assembly:

`Foundation.MessageAdmission`

Allowed project references:

- `Foundation.Contracts`
- `Foundation.SchemaRegistry`
- `Foundation.ApplicationManifest`
- `Foundation.Authority`

No reference to Application projects is permitted.

No reference to a later Service Bus implementation is permitted.

## 4. Admission Context

The bounded context shall carry explicit material inputs, conceptually:

- producer `ApplicationIdentityReference`;
- producer `ManifestIdentity`;
- producer Manifest version;
- deterministic observation time;
- explicit accepted/denied `AuthorityResult` or explicit missing state;
- admission evidence/provenance reference.

The implementation shall use the existing typed identities where public accepted types already exist and shall introduce only WP-04-specific immutable types where no accepted predecessor type represents the admission concept.

The context is not an authority source. It merely supplies attributable inputs to the admission evaluator.

## 5. Evaluation Order

The evaluator shall use a stable fail-closed order so equivalent inputs produce the same reason and decision identity.

Recommended canonical gate sequence:

1. validate evaluator dependencies/context;
2. validate canonical WP-01 envelope;
3. resolve producer Manifest by exact Manifest identity/version;
4. verify Manifest is bound to the explicit producer Application identity;
5. locate one and only one applicable communication declaration for the message type;
6. verify declaration message kind/classification;
7. verify declaration direction/role is compatible with producer emission;
8. verify schema identity/version or explicit compatible-version relation through WP-02;
9. reject retired/unusable schema state;
10. verify explicit AuthorityResult is valid, allowed, relevant, unexpired, and bound to the message authority reference/context required by the bounded design;
11. evaluate message expiry against the supplied observation time;
12. produce deterministic ADMITTED result.

Any missing, malformed, ambiguous, conflicting, incompatible, denied, stale, expired, or unverifiable required input terminates evaluation with a canonical REJECTED reason.

The exact reason taxonomy shall be explicit and verifier-bound.

## 6. Communication Declaration Binding

WP-03 permits multiple communication declarations across distinct message types. It rejects conflicting bindings for the same `MessageType`.

WP-04 shall nevertheless fail closed if the supplied Manifest state cannot yield exactly one applicable declaration for the message type.

The applicable producer-side declaration must be compatible with producer emission:

- `Outbound + Producer`; or
- a future explicitly accepted equivalent already represented by WP-03 without reinterpretation.

WP-04 shall not convert inbound consumer declarations into producer authority.

WP-04 shall not invent a route from `IntendedConsumers` or recipient scope. Recipient route binding belongs to WP-05.

## 7. Schema Rules

For the message schema:

- exact registered usable version may pass;
- unknown version fails closed;
- retired version fails closed;
- when the Manifest declaration names a different version, WP-02 compatibility must explicitly resolve an allowed relation;
- `Incompatible` fails closed;
- undeclared compatibility fails closed;
- WP-04 does not create compatibility rules.

Payload semantics are never inspected to determine compatibility.

## 8. Authority Rules

WP-04 shall reuse accepted Stage 4 authority results and shall not call message or Manifest validity an authority grant.

At minimum:

- missing authority result -> reject;
- malformed authority result -> reject;
- decision other than explicit allow -> reject;
- expired authority result at observation time -> reject;
- authority reference mismatch -> reject;
- materially inconsistent actor/purpose/scope binding -> reject where the accepted AuthorityResult/request evidence exposes the necessary identity;
- successful authority binding is one admission prerequisite only.

If a binding cannot be proven from accepted public predecessor fields, WP-04 shall fail closed or require an explicit typed evidence input. It shall not guess.

## 9. Time Rules

WP-04 shall receive an explicit `DateTimeOffset observationTime` as an input.

No direct use of `DateTimeOffset.UtcNow`, `DateTime.Now`, environment-local time, or hidden ambient clocks is permitted in admission evaluation.

This makes replay deterministic.

Message expiry is an operational admission rule and remains distinct from the Falcon governance principle that time does not expire Owner authority, accepted work, evidence, or continuation rights.

## 10. Admission Result

The WP-04 result shall be immutable and deterministic.

It shall distinguish at least:

- `ADMITTED`
- `REJECTED`

The result shall carry canonical reason identity and material binding references sufficient for reconstruction without copying Application payload content into decision evidence.

The result shall include or deterministically bind:

- decision identity;
- message identity/digest;
- producer Application identity;
- Manifest identity/version;
- schema identity/version;
- authority decision identity/reference;
- observation time;
- effective expiry;
- result/reason;
- evidence reference.

## 11. Payload Opacity

The evaluator may pass the complete canonical envelope to the accepted WP-01 structural validator because that validator owns canonical-envelope validity.

WP-04-specific code shall not:

- parse payload JSON;
- inspect trading fields;
- inspect market data;
- inspect business commands;
- classify financial meaning;
- branch on payload values.

Admission decision canonicalization should bind the accepted WP-01 envelope digest rather than duplicating payload content into WP-04 evidence.

## 12. No Later-WP Leakage

The production assembly shall expose no APIs named or behaving as:

- Route/CreateRoute/ResolveRoute/Dispatch/Send/Deliver;
- Retry/Acknowledge/DeadLetter/Backpressure/FlowControl;
- Publish/Subscribe/EventJournal;
- Encrypt/Decrypt/Sign/VerifySignature/KeyRotate;
- Attach/Detach/Drain/Upgrade/ReplaceApplication.

Architecture/security verification shall inspect this surface explicitly.

## 13. Verifier Strategy

Dedicated verifier:

`verification/Falcon.Stage5.WP04.Verifier`

Initial required scenario families:

### Positive
- exact schema + valid Manifest + valid authority + unexpired message -> ADMITTED
- explicitly compatible schema version -> ADMITTED
- two independent Applications admit independently

### Structural
- null/malformed message -> REJECTED
- material envelope mutation affects admission identity or rejection

### Manifest
- unknown Manifest -> REJECTED
- wrong producer Application binding -> REJECTED
- undeclared message type -> REJECTED
- kind mismatch -> REJECTED
- classification mismatch -> REJECTED
- producer direction/role mismatch -> REJECTED
- ambiguous applicable declaration -> REJECTED/fail closed

### Schema
- unknown schema -> REJECTED
- unknown version -> REJECTED
- retired schema -> REJECTED
- incompatible version -> REJECTED
- undeclared compatibility -> REJECTED

### Authority
- missing authority -> REJECTED
- DENY authority -> REJECTED
- expired authority -> REJECTED
- mismatched authority reference -> REJECTED
- malformed authority result -> REJECTED

### Time
- unexpired message -> eligible
- boundary-expired message -> REJECTED
- observation-time mutation changes outcome where material

### Boundary
- ADMITTED does not create route
- ADMITTED does not deliver
- ADMITTED does not execute
- payload remains opaque
- no FSATS special treatment
- zero-Application Foundation remains valid

### Determinism
- equivalent inputs -> same decision identity
- reordering set-like predecessor declarations does not change decision where semantically equivalent
- material input mutation -> decision identity changes
- second verifier run from same Release outputs -> equivalent result

## 14. Implementation Rule

No source implementation shall weaken an accepted predecessor to make WP-04 easier.

If implementation discovers that a required admission binding cannot be proven using accepted predecessor public surfaces, the correct response is:

1. identify the exact missing binding;
2. determine whether it belongs to WP-04 context, a bounded predecessor-compatible additive surface, or a future WP;
3. fail closed until the binding is explicit;
4. do not infer or special-case.

`WP04_IMPLEMENTATION_DESIGN = READY`
