# Stage 5 WP-07 — Requirement-to-Verifier Traceability

**Status:** IMPLEMENTATION_TRACEABILITY_COMPLETE / RUNTIME_VALIDATION_PENDING  
**Authority:** `Stage5-WP07-Implementation-Authorization-20260808-021900`  
**Workstream:** `foundation-development`

## 1. Scope

This traceability binds the authorized WP-07 event-system requirements to the dedicated verifier and architecture controls. The verifier currently defines **48 named scenarios**. Scenario count is derived from coverage, not from a preset target.

WP-07 remains Application-neutral and owns event truth/publication/replay/journal semantics only. WP-08 through WP-10 remain unauthorized.

## 2. Source truth and predecessor binding

| Requirement | Verifier coverage |
|---|---|
| Authoritative publication requires an admitted event source and dispatchable WP-06 decision | `authoritative_event_publishes_with_explicit_authority` |
| Published event binds exact WP-04 admitted canonical envelope digest | `published_event_binds_exact_admission_digest` |
| Payload/metadata substitution after admission fails closed | `payload_substitution_after_admission_rejected` |
| Non-event FIL source cannot enter event truth | `non_event_source_rejected` |
| Non-admitted source fails closed | `non_admitted_source_rejected` |
| Non-dispatchable WP-06 source fails closed | `non_dispatchable_delivery_rejected` |
| WP-04/WP-06 predecessor substitution fails closed | `admission_delivery_binding_mismatch_rejected` |
| Publisher identity cannot diverge from admitted producer Application | `producer_identity_mismatch_rejected` |
| Producer and subscriber attribution survive into published truth | `subscriber_attribution_preserved` |

Production additionally binds the exact `CanonicalFilEnvelope` digest, admission decision, producer identity, producer Application, intended consumer, schema identity/version, WP-06 delivery decision, correlation and causation identities.

## 3. Classification and authority

| Requirement | Verifier coverage |
|---|---|
| Undefined event-truth classification fails at construction | `malformed_classification_fails_at_construction` |
| Publication authority DENY remains DENY | `publication_authority_denied_rejected` |
| Future publication authority is unusable | `publication_authority_future_rejected` |
| Expired publication authority is unusable | `publication_authority_expired_rejected` |
| Publication authority must bind exact publisher/type/scope/classification/source delivery | `publication_authority_binding_mismatch_rejected` |
| Subscription authority DENY remains DENY | `subscription_authority_denied_rejected` |
| Future subscription authority is unusable | `subscription_authority_future_rejected` |
| Expired subscription authority is unusable | `subscription_authority_expired_rejected` |
| Subscription authority must bind exact subscriber/type/schema/scope/classification digest | `subscription_authority_binding_mismatch_rejected` |
| Classification-incompatible subscription fails closed | `subscription_classification_mismatch_rejected` |

`AuthoritativeOperational` is not a self-declared privilege. It is usable only with an exact valid `AuthorityResult` bound to `event-publication`, while subscription eligibility is separately governed by `event-subscription` authority.

## 4. Replay, correction, supersession and immutable lineage

| Requirement | Verifier coverage |
|---|---|
| Replay remains explicitly non-authoritative | `replay_of_authoritative_event_remains_non_authoritative` |
| Replay cannot regain live/operational truth by label substitution | `replay_cannot_escalate_to_authoritative` |
| Same-publisher/same-truth correction is append-only and publishable | `correction_same_publisher_same_truth_publishes` |
| Relation preserves the resolved prior immutable EventIdentity | `related_event_exact_identity_preserved` |
| Unknown relation target fails closed | `unknown_relation_target_rejected` |
| Another publisher cannot hijack a correction target | `cross_publisher_correction_rejected` |
| Correction/supersession cannot silently change truth classification | `correction_truth_classification_mismatch_rejected` |

Production uses the same `CorrectionOf`/`Supersedes` compatibility branch and records both related `EventId` and resolved immutable `RelatedEventIdentity`. Replay requires explicit `ReplayOf` lineage and cannot be `AuthoritativeOperational`.

## 5. Duplicate, replay-safe source identity and amplification control

| Requirement | Verifier coverage |
|---|---|
| Exact same EventId + canonical identity is idempotent duplicate | `duplicate_same_identity_is_idempotent` |
| Same EventId with changed canonical identity fails closed | `duplicate_event_id_with_conflicting_identity_rejected` |
| One admitted canonical source cannot mint multiple distinct event truths | `same_source_cannot_mint_second_event` |

The journal binds each admitted source envelope digest + admission decision to one event truth identity. A separately created replay/correction/supersession therefore requires its own canonical source message and predecessor evidence.

## 6. Ordering

| Requirement | Verifier coverage |
|---|---|
| Ordered subscription requires key | `ordered_subscription_requires_key` |
| Ordered subscription requires positive sequence | `ordered_subscription_requires_sequence` |
| Unordered subscription cannot smuggle a sequence | `unordered_subscription_rejects_sequence` |
| Exact 1→2 sequence succeeds | `ordered_sequence_one_then_two_publishes` |
| Sequence gaps/out-of-order fail closed | `ordered_sequence_gap_rejected` |
| Independent ordering keys do not poison each other | `independent_ordering_keys_are_isolated` |

Ordering is scoped by subscription identity + publisher Application + ordering key. No global total order is claimed.

## 7. Causality, evidence and journal reconstructability

| Requirement | Verifier coverage |
|---|---|
| Correlation/causation survive publication unchanged | `correlation_causation_preserved` |
| Published/Duplicate publication decisions are retained append-only | `publication_decision_journal_is_append_only` |
| Event and decision identities are SHA-256 | `event_and_decision_identities_are_sha256` |
| Published event surface is immutable | `published_event_surface_is_immutable` |
| Publication decision surface is immutable | `publication_decision_surface_is_immutable` |
| Publication audit-record surface is immutable | `publication_audit_surface_is_immutable` |
| Equivalent classification sets have deterministic subscription identity | `subscription_classification_order_is_deterministic` |
| Equivalent complete inputs produce deterministic identities | `equivalent_inputs_are_deterministic` |
| Event evidence mutation changes event identity | `evidence_mutation_changes_event_identity` |
| Publication authority binding evidence is identity-material | `authority_binding_evidence_mutation_changes_event_identity` |

Published event identity includes producer/consumer attribution, subscription identity, exact source digest/admission/delivery evidence, relation identity, ordering state, authority binding, journal reference and event evidence.

`EventJournal.DecisionSnapshot()` is append-only in current process memory and records publication outcomes. It is an event truth/evidence surface, not an Application database. Durable infrastructure persistence is not silently claimed by this WP.

## 8. Neutrality and later-WP exclusion

| Requirement | Verifier coverage |
|---|---|
| Payload/business meaning remains opaque | `payload_business_semantics_remain_opaque` |
| Arbitrary Application identities receive no special treatment | `application_identity_receives_no_special_treatment` |
| No WP-08+ crypto/lifecycle operations leak into event surface | `event_surface_has_no_wp08_plus_operations` |

Architecture harness additionally enforces:

- `Foundation.EventSystem` exists exactly once as a permanent production project;
- direct references are exactly `Foundation.Contracts`, `Foundation.MessageAdmission`, `Foundation.MessageDelivery`;
- `Falcon.Stage5.WP07.Verifier` exists exactly once;
- verifier direct references are exactly Contracts, MessageAdmission, MessageDelivery, EventSystem;
- the production dependency graph allows no other EventSystem edges;
- permanent production project/assembly/namespace identity remains free of Stage/WP/Application-specific names.

An empty `EventJournal` and the production project have no dependency on an installed Application registry, preserving zero-Application structural validity. Multiple Application identities use the same generic path.

## 9. FCR-0006 coverage

WP-07 directly implements the Foundation-owned event layer portions of FCR-0006:

- immutable event identity;
- producer/consumer attribution;
- correlation/causation;
- replay/test/non-authoritative separation;
- replay-safe source identity;
- duplicate handling;
- correction/supersession relationship semantics;
- scoped ordering enforcement;
- evidence/journal references;
- publication-decision audit history;
- fail-closed handling of unauthoritative replay traffic.

FCR-0006 itself remains open pending runtime verification, final WP-07 review/reconciliation and required Application verification.

## 10. Current gate

```text
WP07_VERIFIER_SCENARIOS = 48
WP07_TRACEABILITY = COMPLETE
WP07_RUNTIME_VALIDATION = PENDING
WP07_OWNER_ACCEPTANCE_AND_CLOSURE = NOT_GRANTED
WP08_THROUGH_WP10 = UNAUTHORIZED
```
