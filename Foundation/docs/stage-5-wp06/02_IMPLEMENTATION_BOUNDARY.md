# Stage 5 WP-06 — Implementation Boundary

**Status:** IMPLEMENTATION_BOUNDARY_DEFINED  
**Authority:** `Stage5-WP06-Implementation-Authorization-20260808-003200`  
**Branch:** `foundation-development`

## 1. Production ownership

WP-06 introduces one permanent Application-neutral Foundation production component:

- `src/Foundation.MessageDelivery/Foundation.MessageDelivery.csproj`
- Assembly / namespace: `Foundation.MessageDelivery`

Its only direct production references are:

- `Foundation.Contracts`
- `Foundation.MessageAdmission`
- `Foundation.MessageRouting`

No Application project is referenced.

## 2. Predecessor boundary

`Foundation.MessageDelivery` consumes but does not recreate predecessor truth.

A delivery decision may proceed only from:

- an accepted WP-05 `RouteDecision` whose decision is `Selected`; and
- the exact accepted WP-04 `MessageAdmissionResult` bound by the route decision.

The implementation verifies material predecessor identity including admission decision ID, message digest/ID, producer identity/Application, Manifest ID/version, recipient scope, and intended consumer.

WP-06 does not re-run Manifest validation, message admission, route registration, route authority validation, route selection, or route/endpoint isolation ownership.

## 3. Delivery policy boundary

`DeliveryPolicy` contains technical transport semantics only:

- `BestEffort`, `AtMostOnce`, or `AtLeastOnce` transport guarantee;
- finite maximum attempt count;
- explicit ordering guarantee and optional canonical ordering key;
- retry/idempotency requirement;
- terminal-containment/dead-letter policy;
- technical traffic class;
- optional governed elevated-priority authority binding;
- evidence reference.

The production surface exposes no exactly-once business-success guarantee.

For `BestEffort` and `AtMostOnce`, construction requires exactly one allowed attempt. `AtLeastOnce` may permit bounded retries only under the remaining WP-06 gates.

## 4. Attempt and outcome truth boundary

WP-06 distinguishes:

- `DispatchEligible`
- `RetryEligible`
- `Deferred`
- `DeadLetter`
- `Rejected`
- `Expired`
- `AlreadyAcknowledged`

A `DeliveryDecision` says what the transport gate permits. It is not itself evidence that an external transport operation occurred.

`DeliveryOutcomeRecorder` records a separately supplied transport observation only for a dispatchable decision and binds the outcome to the exact delivery decision identity.

Transport observations distinguish:

- dispatch accepted;
- recipient acknowledged;
- retryable failure;
- terminal failure.

Recipient acknowledgement remains transport status only and does not establish Application/business success.

## 5. Retry and expiry boundary

Retry eligibility is fail-closed and requires, where applicable:

- exact previous-attempt lineage;
- retryable previous transport outcome;
- `AtLeastOnce` guarantee;
- attempt number within the finite policy limit;
- message/effective authority expiry not reached;
- exact idempotency binding when the policy requires it;
- destination/route health that does not prohibit progress;
- pressure state that permits delivery.

Acknowledged or terminal previous outcomes cannot silently become retry authority.

Retry exhaustion and terminal failures are explicitly contained according to the policy. No message is silently represented as delivered.

## 6. Ordering boundary

Ordering is explicit:

- `None`, with no ordering key permitted; or
- `PerKey`, requiring a canonical ordering key.

WP-06 does not claim global ordering when only key-scoped ordering is represented.

## 7. Flow-control and congestion boundary

`DeliveryPressureSnapshot` binds the delivery evaluation to deterministic technical capacity facts:

- global limit / in-flight count;
- route limit / in-flight count;
- producer limit / in-flight count;
- reserved elevated-traffic slots;
- exact route-decision identity;
- exact producer Application identity;
- evidence.

Malformed capacity snapshots fail at construction. A saturated route or producer defers that bounded delivery decision without changing unrelated route/Application evidence.

Normal traffic cannot consume reserved elevated capacity. Elevated traffic may use reserved global capacity only while still respecting route, producer, and total global limits.

## 8. Technical-priority authority boundary

`Normal` traffic cannot carry an elevated authority binding.

`Protective` and `Revocation` traffic require an explicit `DeliveryPolicyAuthorityBinding` backed by a structurally valid `AuthorityResult` contract. The binding must match:

- exact policy ID/version;
- exact WP-05 route decision ID;
- exact technical traffic class;
- governed effective scope `service-bus-delivery-policy`;
- ALLOW decision and time bounds.

WP-06 consumes this authority; it does not create it and does not depend directly on the Stage-4 Authority Engine implementation.

Application names, payload contents, or producer-declared business priority cannot create Foundation technical criticality.

## 9. Determinism and evidence boundary

Delivery-decision SHA-256 identity binds material inputs using length-prefixed canonical fields, including:

- route decision and route-registry snapshot;
- admission identity/message identity;
- policy/version/evidence and guarantee;
- retry limit, ordering, idempotency, dead-letter and traffic class;
- attempt number and previous outcome/delivery-decision lineage;
- destination health;
- pressure snapshot identity;
- effective expiry;
- governed priority authority material fields;
- observation time;
- decision evidence and final outcome/reason.

Outcome identity additionally binds the exact `DeliveryDecision.DecisionId`, transport observation, observation time and outcome evidence.

No ambient clock, random tie-break, business-name parser, or payload interpretation participates in the decision.

## 10. Explicitly absent from WP-06

The `Foundation.MessageDelivery` public production surface does not own or authorize:

- WP-07 event publication/subscription, event truth, replay truth or event-journal semantics;
- WP-08 encryption, decryption, signing, key management or cryptographic protection profiles;
- WP-09 Application installation, attachment, activation, upgrade, draining, detachment or removal lifecycle;
- WP-10 integrated Stage 5 closure;
- business success/completion truth;
- Application-specific routing/delivery logic;
- external transport/provider implementation;
- deployment or runtime activation.

## 11. Verification component

Dedicated verifier:

- `verification/Falcon.Stage5.WP06.Verifier`

The verifier builds real WP-03/WP-04/WP-05 predecessor state and currently defines 49 named WP-06 scenarios covering delivery truth, retry, expiry, idempotency, terminal containment, ordering, pressure isolation, technical priority, deterministic identities, neutrality and later-WP exclusion.

## 12. Current state

```text
STAGE5_WP01_THROUGH_WP05 = ACCEPTED_AND_CLOSED
STAGE5_WP06_IMPLEMENTATION = AUTHORIZED_AND_IN_PROGRESS
STAGE5_WP06_VALIDATION = NOT_YET_EXECUTED
STAGE5_WP06_OWNER_ACCEPTANCE_AND_CLOSURE = NOT_YET_GRANTED
STAGE5_WP07_THROUGH_WP10 = UNAUTHORIZED
```
