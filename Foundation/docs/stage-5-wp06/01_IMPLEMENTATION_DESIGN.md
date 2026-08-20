# Stage 5 WP-06 — Implementation Design

**Status:** IMPLEMENTATION_DESIGN_BOUND  
**Authority:** `Stage5-WP06-Implementation-Authorization-20260808-003200`  
**Branch:** `foundation-development`

## 1. Design objective

Implement a permanent, Application-neutral Foundation delivery-semantics component that consumes an accepted WP-05 route decision and makes deterministic transport-delivery, retry, terminal-containment and flow-control decisions without interpreting Application business payload meaning.

Proposed permanent production identity:

- Project: `src/Foundation.MessageDelivery/Foundation.MessageDelivery.csproj`
- Namespace / assembly: `Foundation.MessageDelivery`

Proposed dedicated verifier:

- `verification/Falcon.Stage5.WP06.Verifier/Falcon.Stage5.WP06.Verifier.csproj`

## 2. Dependency direction

Production direct references SHALL be minimal:

- `Foundation.Contracts`
- `Foundation.MessageAdmission`
- `Foundation.MessageRouting`

Rationale:

- `Foundation.MessageRouting` supplies the accepted WP-05 route decision.
- `Foundation.MessageAdmission` supplies the accepted predecessor admission decision and effective expiry needed for retry/expiry behavior without reopening or modifying WP-05.
- `Foundation.Contracts` supplies canonical authority/provenance contract types.

WP-06 SHALL NOT depend directly on an Application project, event implementation, crypto implementation, package lifecycle implementation, or Stage/WP-specific production assembly.

## 3. Predecessor binding rule

A delivery evaluation is eligible only when:

1. the WP-05 route decision is `Selected`;
2. the supplied WP-04 admission result is admitted;
3. the admission decision identity exactly equals `RouteDecision.AdmissionDecisionId`;
4. message/producer/Application/Manifest/recipient identities exposed by the route decision remain consistent with the supplied admitted predecessor;
5. the deterministic delivery observation time has not exceeded the accepted effective expiry where that expiry governs transport eligibility.

WP-06 does not re-run WP-04 admission or WP-05 route selection.

## 4. Delivery policy model

A typed immutable `DeliveryPolicy` binds to the exact WP-05 `RouteDecision.DecisionId` and defines only technical transport semantics.

Planned fields include:

- `PolicyId` / version;
- exact bound route-decision identity;
- `DeliveryGuarantee`;
- `MaxAttempts`;
- `OrderingGuarantee` and optional ordering key;
- retry/idempotency requirement;
- terminal containment/dead-letter behavior;
- technical traffic class;
- policy evidence;
- optional governed authority binding when elevated/protective/revocation treatment is requested.

Allowed delivery guarantees SHALL be explicit and finite. WP-06 SHALL NOT expose an `ExactlyOnceBusinessSuccess` guarantee or any equivalent overclaim.

## 5. Delivery guarantee semantics

The initial bounded model will distinguish transport-level guarantees such as:

- `BestEffort`
- `AtMostOnce`
- `AtLeastOnce`

Semantics:

- `BestEffort`: no automatic retry after a failed attempt unless a separately governed policy explicitly permits a transition consistent with the declared guarantee.
- `AtMostOnce`: a message shall not be automatically retried after dispatch when doing so could duplicate effect.
- `AtLeastOnce`: bounded retries may be permitted only while expiry, retry limit, idempotency expectations, route/endpoint eligibility and governed pressure policy allow them.

No transport-level guarantee implies business success.

## 6. Attempt and outcome model

WP-06 will model immutable delivery attempts and deterministic decisions rather than choosing a concrete external transport technology.

A delivery attempt context binds:

- exact route decision;
- exact admitted predecessor;
- delivery policy;
- attempt number;
- previous attempt/outcome lineage where applicable;
- idempotency binding where required;
- deterministic observation time;
- route/destination health evidence;
- pressure/capacity snapshot;
- delivery evidence.

Decision outcomes are intended to distinguish at minimum:

- dispatch eligible/authorized by this delivery gate;
- deferred due to bounded pressure/degradation;
- retry eligible;
- retry prohibited;
- expired;
- terminal containment / dead-letter;
- rejected due to invalid/mismatched predecessor or policy state.

A dispatch decision is not a delivery acknowledgement. An acknowledgement is not business success.

## 7. Retry model

Retry decisions SHALL be deterministic and bounded.

A retry can be eligible only if all applicable conditions pass:

- previous outcome is retryable;
- current attempt count is below `MaxAttempts`;
- the admitted message has not expired;
- declared guarantee permits retry;
- idempotency requirements are satisfied where duplicate effect is possible;
- destination/route health does not make retry prohibited;
- pressure policy permits the attempt;
- no terminal state has already been reached.

Retry exhaustion or non-retryable failure SHALL produce an explicit terminal-containment decision where policy requires, never a silent drop.

## 8. Idempotency binding

WP-06 will not parse payload business semantics to infer idempotency.

Where retry safety requires idempotency, a typed `DeliveryIdempotencyBinding` must bind the idempotency identity/evidence to the exact accepted admission/route decision. Missing or mismatched binding fails closed.

This binding is transport evidence only. It does not claim the Application business operation itself is idempotent unless that fact is governed by an accepted contract/evidence source.

## 9. Ordering model

Ordering SHALL be explicit:

- `None`
- scoped keyed ordering only where an ordering key is present and canonical.

WP-06 SHALL NOT claim global ordering when only a route/key-scoped guarantee exists.

Ordering evidence becomes material to the delivery decision identity.

## 10. Flow-control / pressure model

A typed immutable `DeliveryPressureSnapshot` represents only the technical capacity facts needed for a deterministic delivery decision, including bounded global/route/producer occupancy and evidence.

The evaluator SHALL:

- reject malformed or impossible counts;
- prevent route/producer capacity from exceeding governed limits;
- contain one saturated route/producer from poisoning unrelated capacity decisions;
- distinguish `defer` from `terminal reject` where policy permits deferred delivery;
- never infer technical criticality from Application business names or payload content.

## 11. Technical priority rule

Normal traffic has no elevated privilege.

Any `Protective` or `Revocation` technical handling class must be backed by an explicit governed policy/authority binding. Producer declaration alone is insufficient.

WP-06 will consume an accepted `AuthorityResult` contract for such elevated technical policy when required, but will not depend directly on the Stage-4 Authority Engine implementation and will not create authority.

## 12. Deterministic identity

Every immutable delivery decision identity SHALL bind all material inputs through canonical length-prefixed SHA-256 fields, including as applicable:

- route decision ID and registry snapshot identity;
- admission decision ID/message identity;
- delivery policy identity/version/evidence;
- guarantee;
- attempt number and lineage;
- expiry;
- idempotency binding/evidence;
- ordering mode/key;
- route/destination health;
- capacity/pressure snapshot identity/evidence;
- governed priority authority identity/evidence;
- deterministic observation time;
- final reason/outcome.

No ambient `DateTime.Now`, random tie-break, dictionary iteration order, or business-name parser may affect the result.

## 13. Isolation and neutrality

The production surface SHALL remain valid with zero Applications and support multiple independent Applications without special cases.

Forbidden implementation patterns include:

- checking for `FSATS`, `Guardian`, `FSAPMA`, market data, trading commands, symbols, brokers, or strategy names;
- payload inspection for routing/delivery policy;
- global mutable pressure state shared without explicit keyed isolation;
- one route failure automatically failing unrelated routes;
- Application self-assigned Foundation priority.

## 14. Explicit later-WP exclusions

The production surface SHALL expose no methods or types whose purpose is to implement:

- event publish/subscribe/replay truth (WP-07);
- encryption/signing/key/protection-profile operations (WP-08);
- Application attach/install/activate/upgrade/drain/detach/remove lifecycle (WP-09);
- integrated Stage 5 closure (WP-10).

## 15. Verification strategy

The verifier SHALL construct accepted predecessor state through real WP-03/WP-04/WP-05 production evaluators where practical rather than fabricating internal result types.

Verification groups:

1. predecessor binding;
2. explicit guarantee semantics;
3. truthful outcome/status separation;
4. retry and expiry;
5. idempotency;
6. terminal containment/dead-letter;
7. ordering scope;
8. flow control and isolation;
9. technical priority authority;
10. determinism/mutation sensitivity;
11. zero/two-Application neutrality;
12. WP-07+ boundary absence.

## 16. Implementation state

```text
STAGE5_WP01_THROUGH_WP05 = ACCEPTED_AND_CLOSED
STAGE5_WP06_IMPLEMENTATION = AUTHORIZED_AND_IN_PROGRESS
STAGE5_WP07_THROUGH_WP10 = UNAUTHORIZED
```
