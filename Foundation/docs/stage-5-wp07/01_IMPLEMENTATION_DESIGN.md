# Stage 5 WP-07 — Implementation Design

**Status:** IMPLEMENTATION_DESIGN_ACCEPTED_FOR_EXECUTION  
**Authority:** `Stage5-WP07-Implementation-Authorization-20260808-021900`  
**Workstream:** `foundation-development`

## 1. Design objective

Implement an Application-neutral Foundation event system that can state truthfully whether an event is eligible to be published, who produced it, who may consume it, whether it is authoritative operational truth or replay/test/non-authoritative material, how it relates causally to prior events/messages, and whether it corrects/supersedes prior immutable event history.

WP-07 shall not convert transport success into business/event truth automatically. Publication is a distinct governed decision.

## 2. Proposed production boundary

Create one permanent production project:

`src/Foundation.EventSystem/Foundation.EventSystem.csproj`

Primary production source:

`src/Foundation.EventSystem/EventSystem.cs`

The production project may depend only on the exact accepted predecessor projects needed to consume canonical identities and accepted delivery evidence. It must not depend on Applications or later Stage 5 projects.

Expected direct predecessor set is intentionally minimal and shall be confirmed by implementation/architecture review before validation.

## 3. Event identity model

A canonical event record shall carry only generic Foundation metadata, including at minimum:

- immutable EventId;
- EventType identifier;
- publisher Application/producer identity;
- declared consumer/subscriber scope;
- canonical schema identity/version;
- event classification;
- correlation identity;
- causation identity;
- source message/delivery evidence identity where applicable;
- ordering scope/key where applicable;
- duplicate/idempotency identity where applicable;
- correction/supersession reference where applicable;
- publication observation instant;
- immutable evidence/journal reference;
- deterministic canonical digest/identity.

Foundation shall not parse the business payload to decide what the event means.

## 4. Event classification

The generic event classification must distinguish at least:

- `AuthoritativeOperational`
- `Replay`
- `Test`
- `Simulation`
- `NonAuthoritativeEvidence`

Exact enum naming may be refined during implementation, but the semantics must remain explicit and fail closed for unknown values.

Classification does not itself grant authority. An `AuthoritativeOperational` label is only usable when exact governing authority/predecessor evidence permits it.

## 5. Publication decision

Publication must be a deterministic, immutable decision distinct from message admission, route selection, and delivery.

A successful publication decision means only:

> the event passed the governed Foundation event-publication truth gate for its declared classification and scope.

It does not mean:

- a subscriber acted on the event;
- a business outcome occurred;
- a trading action is authorized;
- replay created live authority;
- an external system received anything.

## 6. Subscription model

Subscriptions are declarations/eligibility rules, not hidden runtime coupling.

A subscription must bind:

- subscriber identity;
- event type/schema;
- accepted classification(s);
- scope;
- authority/evidence where required;
- ordering expectations where declared.

Unknown, conflicting, undeclared, or classification-incompatible subscriptions fail closed.

## 7. Replay truth isolation

Replay/test/simulation events must never silently become authoritative operational events.

The event system shall:

- preserve original immutable event identity/history;
- create explicit replay lineage rather than mutating the original event;
- preserve source correlation/causation/evidence references;
- require an explicit non-operational classification for replay/test/simulation material unless separate lawful authority explicitly establishes a new operational event;
- never recreate Application action authority merely because an original event was once operational.

WP-07 governs event truth classification, not Application-side replay execution.

## 8. Duplicate handling

Exact duplicate identity with exact immutable canonical content may be classified deterministically as an idempotent duplicate/no-new-truth outcome.

The same event identity with different canonical content must fail closed as a conflict.

Duplicate handling must not rewrite history.

## 9. Correction and supersession

Corrections are append-only event relationships.

A correction/supersession event must:

- have its own immutable EventId;
- reference the exact prior event identity;
- state the relationship explicitly;
- preserve the prior event unchanged;
- fail closed for unknown/incompatible targets;
- produce deterministic evidence of the relationship.

The Foundation event layer records the correction relationship. Application business interpretation of the correction remains outside Foundation.

## 10. Ordering

WP-06 already owns transport ordering declarations/behavior. WP-07 may claim event ordering only where the event contract explicitly binds an ordering scope/key and the relevant accepted predecessor evidence supports it.

No global total ordering is implied unless separately specified and authorized.

## 11. Journal and reconstructability

WP-07 may maintain an immutable event-journal/evidence surface sufficient to reconstruct:

- what event identity was considered;
- publisher/classification/scope;
- publication decision;
- duplicate/correction/replay lineage;
- causal/correlation references;
- canonical digest/evidence identity.

The journal is evidence/truth history, not an Application database and not a business-state store.

## 12. Authority rules

Event publication shall preserve the Falcon authority principle:

- no self-declared operational authority;
- no authority inherited merely from installation, schema validity, route existence, delivery success, subscription existence, or replay lineage;
- DENY remains DENY;
- expired/future/mismatched authority fails closed;
- exact scope and producer bindings are material to the decision identity.

## 13. FCR-0006 mapping

WP-07 directly targets the remaining Foundation-owned portions of FCR-0006:

- immutable event identity;
- producer/consumer attribution;
- causation/correlation at event layer;
- duplicate handling;
- correction/supersession semantics;
- replay/test versus authoritative operational classification;
- replay-safe identity;
- evidence retention/journal references;
- fail-closed handling of unauthoritative replay traffic.

Application verification remains required before FCR-0006 itself can be considered for closure.

## 14. Required verifier families

The dedicated WP-07 verifier must cover at least:

1. positive authoritative publication;
2. positive non-authoritative/replay publication;
3. malformed/unknown classification rejection;
4. publisher identity mismatch;
5. subscriber/scope mismatch;
6. source predecessor evidence mismatch;
7. correlation/causation preservation;
8. replay cannot become operational by label substitution;
9. duplicate exact identity/content deterministic handling;
10. duplicate identity/content conflict rejection;
11. valid correction/supersession append-only behavior;
12. unknown/incompatible correction target rejection;
13. ordering declaration/key enforcement;
14. deterministic publication identity;
15. journal/evidence immutability;
16. payload opacity;
17. zero-Application neutrality;
18. two-Application isolation;
19. no WP-08+ public operations;
20. no Application/trading special case.

Scenario count shall be determined by complete traceability, not by a preselected number.

## 15. Validation sequence

Before Owner closure:

1. static source/design review;
2. complete FCR review;
3. controlled Restore and Release Build;
4. Architecture and Security gates;
5. accepted predecessor regressions;
6. focused WP-07 verifier execution;
7. deterministic WP-07 rerun;
8. full final regression including Baseline Integrity and all accepted predecessors;
9. independent post-implementation architecture/security/completeness review;
10. final FCR reconciliation and issue updates;
11. explicit Owner acceptance/closure.

## 16. Current gate

`STAGE5_WP07_IMPLEMENTATION = AUTHORIZED_AND_IN_PROGRESS`

`STAGE5_WP08_THROUGH_WP10 = UNAUTHORIZED`
