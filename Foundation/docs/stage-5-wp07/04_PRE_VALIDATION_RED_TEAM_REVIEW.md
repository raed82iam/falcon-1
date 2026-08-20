# Stage 5 WP-07 — Pre-Validation Red-Team Review

**Status:** STATIC_RED_TEAM_PASS / RUNTIME_VALIDATION_PENDING  
**Authority:** `Stage5-WP07-Implementation-Authorization-20260808-021900`  
**Branch:** `foundation-development`

## 1. Review purpose

Challenge the WP-07 event-system implementation before any runtime PASS or Owner-closure claim. This review covers source truth, authority, replay isolation, duplicate/correction semantics, event amplification, ordering, reconstructability, Application neutrality, FCR ownership and later-WP leakage.

Static PASS does not substitute for Restore/Build/runtime validation.

## 2. Attack surfaces challenged

The review explicitly attacked:

- substitution of a different canonical FIL envelope after WP-04 admission;
- mismatch between WP-04 admission and WP-06 delivery evidence;
- caller self-declaration of `AuthoritativeOperational` truth;
- hidden/denied/future/expired/mismatched publication authority;
- hidden/denied/future/expired/mismatched subscription authority;
- producer/consumer attribution substitution;
- replay/test/simulation escalation into authoritative operational truth;
- replay/correction lineage against unknown or incompatible events;
- cross-publisher correction hijacking;
- duplicate identity conflicts;
- reuse of one admitted source to mint multiple event truths;
- ordering gaps and cross-key poisoning;
- loss of correlation/causation identity;
- authority/evidence mutation without identity change;
- publication-decision history loss;
- payload/business interpretation;
- Application-specific special cases;
- WP-08 cryptographic or WP-09 lifecycle leakage;
- scope leakage from FCR-0004/5/7/8/9/10/11.

## 3. Findings and remediation

### RT-01 — exact admitted-envelope binding

**Initial risk:** first draft bound selected message IDs/trace fields but did not prove exact canonical envelope bytes were the envelope admitted by WP-04.

**Remediation:** WP-07 now consumes the exact `MessageAdmissionResult`, recomputes `CanonicalMessagingDigest.ComputeEnvelopeSha256(...)`, and requires equality with `MessageAdmissionResult.MessageDigest`. Admission decision, producer identity/Application, recipient scope, schema, WP-06 delivery evidence and trace identities are also bound.

Verifier: `published_event_binds_exact_admission_digest`, `payload_substitution_after_admission_rejected`, `admission_delivery_binding_mismatch_rejected`.

### RT-02 — self-declared authoritative event truth

**Initial risk:** caller could choose `AuthoritativeOperational` classification without a distinct event-publication authority.

**Remediation:** every publication requires `EventPublicationAuthorityBinding` carrying a valid accepted `AuthorityResult` bound to exact publisher, event type, subscriber scope, classification, exact WP-06 source delivery decision and effective scope `event-publication`. DENY/future/expired/mismatch fail closed.

Subscriptions independently require `EventSubscriptionAuthorityBinding` bound to exact subscriber/type/schema/scope/classification digest and scope `event-subscription`.

Verifier families cover both authority paths.

### RT-03 — ordering label without ordering truth

**Initial risk:** requiring only an ordering key could permit gaps/out-of-order publication while still appearing ordered.

**Remediation:** ordered subscriptions require a positive sequence. `EventJournal` enforces exact monotonic `previous + 1` per subscription identity + publisher + ordering key. Unordered subscriptions reject hidden keys/sequences.

Verifier: ordering key/sequence/gap/isolation scenarios.

### RT-04 — incomplete producer/consumer attribution

**Initial risk:** event record initially preserved publisher Application + scope but not exact producer identity, subscriber Application and subscription identity.

**Remediation:** `PublishedEvent` now records producer identity, publisher Application, subscriber Application, subscriber scope and deterministic subscription identity. Source admission intended consumer must match subscriber Application.

Verifier: `producer_identity_mismatch_rejected`, `subscriber_attribution_preserved`.

### RT-05 — relation by EventId without exact target identity

**Initial risk:** correction/replay lineage initially exposed only related EventId, weakening reconstructability language requiring exact prior immutable identity.

**Remediation:** relation target is resolved from the journal and both `RelatedEventId` and exact `RelatedEventIdentity` are identity-material in the new event.

Verifier: `related_event_exact_identity_preserved`.

### RT-06 — publication decision history loss

**Initial risk:** journal stored successful event truth but not the publication decision history itself.

**Remediation:** `EventJournal` now keeps append-only `EventPublicationAuditRecord` entries for valid-journal Published/Duplicate/Rejected decisions. Audit identities are deterministic SHA-256 and the public audit surface is immutable.

Verifier: `publication_decision_journal_is_append_only`, `publication_audit_surface_is_immutable`.

No durable filesystem/database persistence is claimed. Current WP-07 journal is an in-process truth/evidence surface with explicit external `JournalReference`/evidence references.

### RT-07 — one admitted source minting multiple truths

**Initial risk:** a single admitted envelope / WP-06 source could be reused with a different EventId and create multiple event truths.

**Remediation:** journal binds canonical source envelope digest + admission decision to one immutable event truth identity. Exact duplicate remains idempotent; different truth from the same source fails with `EVENT_SOURCE_ALREADY_PUBLISHED_CONFLICT`. Corrections/replays/supersessions require their own canonical source evidence.

Verifier: `same_source_cannot_mint_second_event`.

### RT-08 — authority binding evidence not identity-material

**Initial risk:** changing authority reference/binding evidence while keeping the same authority decision identity could leave event/subscription identities unchanged.

**Remediation:** event and subscription canonical identities now include authority reference, effective scope and binding evidence in addition to authority decision identity.

Verifier: `authority_binding_evidence_mutation_changes_event_identity` plus deterministic subscription scenarios.

### RT-09 — verifier reflection ambiguity / false business-semantic alarm

**Initial risk:** record copy constructors could make reflection `.Single()` ambiguous, and the token `Order` could falsely flag legitimate `OrderingKey` as business payload interpretation.

**Remediation:** verifier selects internal predecessor constructors by exact parameter count and uses narrower business-semantic tokens (`Trade`, `TradingOrder`, `Price`, `Position`, `Strategy`). No warning suppression or production weakening was introduced.

## 4. Replay/correction safety

Current behavior:

- `ReplayOf` cannot be `AuthoritativeOperational`;
- replay has its own EventId/source evidence and preserves explicit lineage;
- correction/supersession requires compatible event type/schema;
- correction/supersession requires same publisher and truth classification as target;
- original event remains immutable;
- related immutable EventIdentity is recorded;
- replay lineage does not recreate Application action authority.

## 5. Fail-closed behavior

Static review confirms fail-closed handling for:

- undefined classification/relation enums;
- non-event source;
- rejected WP-04 source;
- non-dispatchable WP-06 source;
- exact envelope/admission/delivery mismatch;
- producer/intended-consumer mismatch;
- malformed/denied/future/expired/mismatched publication authority;
- malformed/denied/future/expired/mismatched subscription authority;
- incompatible subscription classification/scope/schema;
- replay-to-operational escalation;
- unknown/incompatible relation target;
- duplicate identity conflict;
- reused source truth amplification;
- missing/unexpected ordering key or sequence;
- sequence gaps.

## 6. Determinism and evidence review

Event/subscription/decision/audit identities use length-prefixed SHA-256 canonicalization. Inputs are explicit; no ambient clock or random identity is used by production logic.

Identity-material evidence includes:

- source envelope digest;
- WP-04 admission decision;
- WP-06 delivery decision;
- producer/consumer attribution;
- subscription identity;
- correlation/causation;
- truth classification;
- relation and exact related EventIdentity;
- ordering key/sequence;
- publication authority reference/result/scope/binding evidence;
- journal/evidence references;
- observation time.

## 7. Application neutrality and later-WP boundary

No production special case exists for FSATS, Guardian, FSAPMA, FSTSimA, market, broker, strategy or trading semantics. Payload is opaque.

WP-07 does not implement:

- WP-08 encryption/signing/key-management;
- WP-09 Application install/attach/upgrade/drain/detach/remove;
- WP-10 integrated Stage 5 closure;
- resource allocation/request governance;
- Internet egress;
- Live credential/route isolation;
- new transport QoS/tail-latency guarantees;
- Application business action authority.

## 8. FCR review result

Fresh pre-validation review of all open FCRs #4 through #11 confirms:

```text
FCR_0004 = LIMITED_OVERLAP_ONLY / NO_SCOPE_EXPANSION
FCR_0005 = LIMITED_OVERLAP_ONLY / NO_SCOPE_EXPANSION
FCR_0006 = DIRECT_MATERIAL_WP07_OWNER_FIT
FCR_0007 = OUT_OF_SCOPE_WP07
FCR_0008 = OUT_OF_SCOPE_WP07
FCR_0009 = OUT_OF_SCOPE_WP07_EXCEPT_REUSING_ACCEPTED_WP06_PREDECESSOR_EVIDENCE
FCR_0010 = OUT_OF_SCOPE_WP07
FCR_0011 = OUT_OF_SCOPE_WP07
```

No FCR is closed by this review.

## 9. Static verdict

```text
WP07_STATIC_ARCHITECTURE_REVIEW = PASS
WP07_STATIC_SECURITY_RED_TEAM = PASS
WP07_APPLICATION_NEUTRALITY_REVIEW = PASS
WP07_REPLAY_TRUTH_ISOLATION_REVIEW = PASS
WP07_FCR_PRE_VALIDATION_REVIEW = COMPLETE
KNOWN_STATIC_BLOCKING_FINDINGS = NONE
WP07_VERIFIER = 48_NAMED_SCENARIOS
WP07_FOCUSED_VALIDATION = AUTHORIZED_TO_EXECUTE
WP07_OWNER_ACCEPTANCE_AND_CLOSURE = NOT_GRANTED
WP08_THROUGH_WP10 = UNAUTHORIZED
```

Runtime/build validation is still required. CI configuration presence is not a CI PASS claim.
