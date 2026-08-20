# Stage 5 WP-05 — Implementation Boundary

**Status:** IMPLEMENTED / PRE-VALIDATION REVIEW IN PROGRESS  
**Authority:** `Stage5-WP05-Implementation-Authorization-20260807-221800`  
**Branch:** `foundation-development`

## Production surface

WP-05 introduces:

- `src/Foundation.MessageRouting/Foundation.MessageRouting.csproj`
- assembly/root namespace `Foundation.MessageRouting`

Direct production references are exactly:

- `Foundation.Contracts`
- `Foundation.ApplicationManifest`
- `Foundation.MessageAdmission`

No Application project is referenced. `Foundation.MessageRouting` consumes `AuthorityResult` as a contract through `Foundation.Contracts`; it does not reference or reimplement the `Foundation.Authority` engine.

## Owned behavior

WP-05 owns only bounded governed route declaration, governed registration, route eligibility, deterministic route selection, and routing isolation decisions.

The implemented path is:

```text
Accepted WP-03 Manifest identity/version/SHA-256
  + explicit RouteAuthorityBinding carrying accepted AuthorityResult contract
    -> governed RouteRegistry registration

Accepted WP-04 MessageAdmissionResult
  + explicit RoutingMessageTypeBinding
  + one frozen governed RouteRegistry snapshot
  + exact route purpose
  + explicit UTC observation time
  + optional endpoint-state evidence
  + routing evidence
    -> SELECTED / REJECTED
```

`SELECTED` means one and only one governed eligible route was selected. It does not mean dispatch, queueing, delivery, acknowledgement, retry, event publication, execution, or business completion.

## Governed registration boundary

A route cannot enter `RouteRegistry` merely because a caller created a `RouteDeclaration` object.

Registration requires:

- exact source Manifest identity/version resolution;
- exact canonical Manifest SHA-256 match;
- exact Application identity match;
- exact intended-consumer declaration;
- exactly one matching communication declaration that is `Outbound / Producer`;
- structurally valid `AuthorityResult` contract;
- explicit route-authority binding to the exact route/application/producer/recipient/consumer/message-type/purpose;
- exact authority effective-scope binding;
- `ALLOW` authority decision;
- unique route identity/version.

This preserves the distinction:

`Manifest declaration != route authority != message admission != route selection != delivery`.

## Selection binding rules

Selection requires exact binding to:

- admitted WP-04 decision identity;
- admitted message identity and digest;
- original producer identity;
- producer Application identity;
- admitted Manifest identity/version;
- recipient scope;
- intended consumer;
- explicit message-type binding tied to the exact WP-04 decision;
- exact route purpose;
- route identity/version;
- source/destination endpoint identities;
- route state;
- route authority time window;
- route/authority evidence;
- supplied endpoint-state evidence when present.

No business payload interpretation and no Application-name parsing is used.

## Authority ownership

WP-05 does not create authority and does not own Stage-4 policy evaluation.

`RouteAuthorityBinding` consumes an already-produced `AuthorityResult` contract. Route registration requires a valid ALLOW result bound to the declaration. Route selection additionally enforces its `DecisionTime` and `Expiry` against the supplied routing observation time.

The route-authority decision, effective scope, decision time, expiry, binding fields, and evidence are material to the deterministic registry/decision identity.

## Determinism and evidence

Each evaluation freezes exactly one thread-safe deterministic registry snapshot before candidate evaluation.

That same snapshot is used for:

- candidate matching;
- ambiguity detection;
- isolation filtering;
- authority-time filtering;
- selected/rejected decision identity.

The snapshot is SHA-256 bound into `RouteDecision.RegistrySnapshotDigest`.

Canonical hashing uses length-prefixed fields, preventing delimiter ambiguity. No ambient clock or random tie-break exists.

## Isolation semantics

- `RouteState.Isolated` excludes only the affected route declaration.
- `RouteState.Unavailable` excludes only the affected route declaration.
- explicit endpoint state excludes routes using an isolated, unavailable, or unknown endpoint.
- when endpoint-state evidence is supplied, a missing endpoint fails closed for the affected route.
- isolation does not execute Application lifecycle operations.

## Ambiguity behavior

If more than one otherwise eligible governed route remains, WP-05 returns `ROUTE_AMBIGUOUS`.

WP-05 does not invent a priority, random tie-break, route preference, or Application-specific special case.

## FCR interpretation

Application FCRs are problem statements, not implementation authority.

- FCR-0004: WP-05 addresses the generic governed routing/isolation portion. Delivery and command execution remain later work.
- FCR-0005: WP-05 addresses generic producer/consumer route eligibility only. Market-data delivery semantics remain later work.
- FCR-0006: WP-05 preserves routing attribution/isolation compatibility only. Event truth, publication, replay delivery, duplicate/correction behavior remain outside WP-05.
- FCR-0009: WP-05 may enforce already-governed expiry/deadline eligibility. Queueing, backpressure, QoS execution, congestion behavior, and tail-latency evidence remain outside WP-05.

No FCR creates an Application-specific route implementation.

## Explicit non-scope

WP-05 does not expose or execute:

- send/dispatch;
- enqueue/dequeue;
- delivery;
- acknowledgement;
- retry/dead-letter;
- duplicate-effect suppression;
- backpressure/flow control;
- event publish/subscribe;
- cryptographic protection;
- Application attach/detach/activate/recover;
- deployment/runtime activation;
- Application business logic or payload interpretation.

These boundaries preserve WP-06 through WP-10 as separately governed work.
