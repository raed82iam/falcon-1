# Stage 5 WP-05 — Bounded Implementation Design

**Status:** IMPLEMENTATION_DESIGN_COMPLETE / SECURITY_HARDENED  
**Authority:** `Stage5-WP05-Implementation-Authorization-20260807-221800`  
**Branch:** `foundation-development`

## 1. Production boundary

WP-05 introduces one Application-neutral production assembly:

`Foundation.MessageRouting`

Its direct production references are exactly:

- `Foundation.Contracts`;
- `Foundation.ApplicationManifest`;
- `Foundation.MessageAdmission`.

WP-05 deliberately does **not** reference the `Foundation.Authority` engine implementation. It consumes the accepted `AuthorityResult` contract through `Foundation.Contracts`, preserving Stage 4 as the sole authority-engine owner.

No Application project dependency is permitted.

No transport provider, queue, network client, broker, event publisher, crypto provider, scheduler, or runtime lifecycle executor is permitted in WP-05.

## 2. Core immutable types

### RouteIdentity / RouteEndpointIdentity

Canonical immutable technical identities. Neither is inferred from Application naming or business payload meaning.

### RouteState

Bounded selection state:

- `Eligible`
- `Isolated`
- `Unavailable`

WP-05 consumes this state for selection only. It does not execute lifecycle transitions that create the state.

### RoutingMessageTypeBinding

Typed bridge for the one predecessor field not publicly exposed by `MessageAdmissionResult`:

- exact WP-04 `AdmissionDecisionId`;
- canonical message type;
- binding evidence.

Message type is never reconstructed from digest, naming convention, recipient, or payload.

### RouteAuthorityBinding

Explicit attributable binding of one accepted Stage-4 `AuthorityResult` contract to one route declaration:

- authority reference;
- authority result;
- authorized route identity/version;
- producer identity;
- producer Application identity;
- recipient scope;
- intended consumer;
- message type;
- route purpose;
- effective scope;
- binding evidence.

Route existence is not authority. A route declaration cannot enter the governed registry unless this binding is valid and the authority result is structurally valid and `ALLOW`.

### RouteDeclaration

Immutable declaration containing:

- route identity/version;
- source Manifest identity/version;
- exact source Manifest SHA-256;
- source producer/Application identity;
- destination recipient scope;
- intended consumer;
- message type;
- source/destination endpoint identities;
- route purpose;
- route state;
- route-authority binding;
- evidence/provenance.

### RouteRegistry

A thread-safe deterministic governed registry.

Registration resolves the exact accepted WP-03 Manifest identity/version and requires:

1. Manifest resolution succeeds;
2. supplied Manifest SHA-256 equals the registered canonical digest;
3. Manifest Application identity matches the route declaration;
4. intended consumer is declared exactly once;
5. message communication declaration exists exactly once and is `Outbound / Producer`;
6. route authority result passes accepted contract validation;
7. route authority binding matches the exact route declaration;
8. bound effective scope matches the authority result;
9. authority decision is `ALLOW`;
10. route identity/version is not already registered.

Registration remains distinct from message admission, route selection, delivery, activation, and execution.

### RouteSelectionContext

Explicit deterministic routing input:

- accepted/rejected WP-04 `MessageAdmissionResult`;
- typed `RoutingMessageTypeBinding`;
- route purpose;
- UTC observation time;
- optional endpoint-state evidence;
- routing-decision evidence.

No ambient clock is permitted.

### RouteDecision

Immutable decision recording, where available:

- selected/rejected state and exact reason;
- SHA-256 decision identity;
- SHA-256 registry snapshot digest;
- WP-04 admission/message identities;
- producer/Application/Manifest/recipient/consumer bindings;
- message type and route purpose;
- route identity/version and endpoints;
- route authority reference/decision/effective scope/evidence;
- route Manifest digest;
- route/routing evidence;
- observation time.

`SELECTED` means only that exactly one governed eligible route was selected.

## 3. Selection algorithm

One immutable registry snapshot is frozen per evaluation and reused for candidate matching and decision identity.

The evaluator:

1. requires a registry and routing context;
2. requires WP-04 `ADMITTED`;
3. rejects an expired WP-04 effective boundary;
4. requires a message-type binding to the exact WP-04 DecisionId;
5. requires at least one governed registered route;
6. matches producer and producer Application exactly;
7. matches recipient scope exactly;
8. matches intended consumer exactly;
9. matches message type exactly;
10. matches route purpose exactly;
11. matches route Manifest identity/version to the exact admitted Manifest;
12. excludes isolated/unavailable routes;
13. requires route authority to be effective at the supplied observation time;
14. rejects expired route authority;
15. excludes ineligible/isolated/unavailable/unknown endpoints when explicit endpoint-state evidence is supplied;
16. fails closed if no candidate remains;
17. fails `ROUTE_AMBIGUOUS` if more than one eligible route remains;
18. selects only when exactly one governed route remains.

There is no priority invention, random tie-break, FSATS branch, or business-payload inspection.

## 4. Security hardening from implementation review

The initial bounded design was hardened during pre-validation red-team review without changing the authorized WP-05 responsibility.

### A. Governed Manifest binding

An early route registry could accept a structurally valid route object without independently proving it was backed by the accepted WP-03 Manifest. This was rejected as insufficient governance.

The final registry requires exact Manifest identity/version/SHA-256 and validates the Application, consumer, and outbound-producer communication declaration before registration.

### B. Explicit route authority

Manifest declaration and WP-04 admission do not create route authority. The final design therefore adds `RouteAuthorityBinding` and consumes an accepted Stage-4 `AuthorityResult` contract. WP-05 does not create or evaluate policy authority itself.

### C. Immutable evaluation snapshot

An early evaluator could select from one registry snapshot and later derive evidence from a second mutable read. The final evaluator freezes exactly one thread-safe snapshot per evaluation.

### D. Complete rejection identity

Rejected/ambiguous decisions bind the same registry snapshot used for selection. Registry mutation therefore changes rejection identity rather than silently producing the same evidence identity.

### E. Canonicalization hardening

Decision and snapshot hashing use length-prefixed fields instead of delimiter concatenation.

Material route-authority fields bound into the canonical identity include:

- authority reference;
- decision identity;
- decision (`ALLOW`/other contract value);
- effective scope;
- decision time;
- expiry;
- declaration-binding fields;
- authority-binding evidence.

A dedicated verifier gate checks that authority decision-time and expiry mutations change the canonical routing identity.

## 5. Isolation behavior

Isolation is selection exclusion, not lifecycle execution.

- isolating route A does not alter route B;
- an isolated/unavailable endpoint excludes only affected declarations;
- when endpoint-state evidence is supplied, a missing/unknown endpoint fails closed for affected routes;
- WP-05 does not suspend, quarantine, recover, detach, remove, restart, or activate Applications.

## 6. Fail-closed outcomes

Registration rejects unknown/mismatched Manifest and authority bindings before a route can become selectable.

Selection rejects, among other cases:

- non-admitted/expired WP-04 inputs;
- missing/mismatched message-type binding;
- no route;
- source/destination/consumer/message/purpose/Manifest mismatch;
- isolated/unavailable route;
- future/expired route authority;
- ineligible endpoints;
- ambiguous eligible routes.

## 7. Verification boundary

Dedicated verifier:

`Falcon.Stage5.WP05.Verifier`

It uses real WP-02 schema registration, real WP-03 Manifest registration/resolution, real WP-04 admission evaluation, and explicit Stage-4 authority-result contracts. It does not fabricate an internal `MessageAdmissionResult`.

Coverage includes route-registration governance, selection, isolation containment, authority time boundaries, deterministic identities, registry mutation, Application neutrality, zero-Application behavior, payload opacity, and no-WP06+ public-surface checks.

## 8. Later-WP firewall

WP-05 exposes no behavior that sends, dispatches, enqueues, delivers, acknowledges, retries, dead-letters, applies backpressure/flow-control, publishes events, performs crypto, executes Application lifecycle, deploys, or activates runtime behavior.

## 9. Design conclusion

`WP05_PRODUCTION_BOUNDARY = Foundation.MessageRouting`

`WP05_ROUTE_DECLARATION = WP03_MANIFEST_BOUND + EXPLICIT_ROUTE_AUTHORITY_BOUND`

`WP05_AUTHORITY_ENGINE_OWNERSHIP = STAGE4_UNCHANGED / CONTRACT_CONSUMPTION_ONLY`

`WP05_DECISION = ROUTE_ELIGIBILITY_AND_SELECTION_ONLY`

`WP05_MESSAGE_TYPE_BINDING = EXPLICIT_TYPED_BINDING / NO INFERENCE`

`WP06_THROUGH_WP10 = NOT_AUTHORIZED`
