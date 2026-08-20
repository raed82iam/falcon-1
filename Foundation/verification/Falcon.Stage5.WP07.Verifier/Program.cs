using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;
using Foundation.EventSystem;
using Foundation.MessageAdmission;
using Foundation.MessageDelivery;

namespace Falcon.Stage5.WP07.Verifier;

internal static class Program
{
    private const string EventType = "falcon.reference.event.v1";
    private const string SchemaId = "schema:falcon.reference.event";
    private const string Publisher = "application.alpha";
    private const string Subscriber = "application.beta";
    private const string SubscriberScope = "application.beta/events/reference";
    private static readonly DateTimeOffset BaseTime = new(2026, 8, 8, 3, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        var scenarios = new (string Name, Action Test)[]
        {
            ("authoritative_event_publishes_with_explicit_authority", AuthoritativeEventPublishes),
            ("published_event_binds_exact_admission_digest", PublishedEventBindsAdmissionDigest),
            ("payload_substitution_after_admission_rejected", PayloadSubstitutionRejected),
            ("non_event_source_rejected", NonEventSourceRejected),
            ("non_admitted_source_rejected", NonAdmittedSourceRejected),
            ("non_dispatchable_delivery_rejected", NonDispatchableDeliveryRejected),
            ("admission_delivery_binding_mismatch_rejected", AdmissionDeliveryMismatchRejected),
            ("producer_identity_mismatch_rejected", ProducerIdentityMismatchRejected),
            ("subscriber_attribution_preserved", SubscriberAttributionPreserved),
            ("malformed_classification_fails_at_construction", MalformedClassificationFailsAtConstruction),
            ("publication_authority_denied_rejected", PublicationAuthorityDeniedRejected),
            ("publication_authority_future_rejected", PublicationAuthorityFutureRejected),
            ("publication_authority_expired_rejected", PublicationAuthorityExpiredRejected),
            ("publication_authority_binding_mismatch_rejected", PublicationAuthorityMismatchRejected),
            ("subscription_authority_denied_rejected", SubscriptionAuthorityDeniedRejected),
            ("subscription_authority_future_rejected", SubscriptionAuthorityFutureRejected),
            ("subscription_authority_expired_rejected", SubscriptionAuthorityExpiredRejected),
            ("subscription_authority_binding_mismatch_rejected", SubscriptionAuthorityMismatchRejected),
            ("subscription_classification_mismatch_rejected", SubscriptionClassificationMismatchRejected),
            ("replay_of_authoritative_event_remains_non_authoritative", ReplayRemainsNonAuthoritative),
            ("replay_cannot_escalate_to_authoritative", ReplayEscalationRejected),
            ("correction_same_publisher_same_truth_publishes", CorrectionPublishes),
            ("related_event_exact_identity_preserved", RelatedEventIdentityPreserved),
            ("unknown_relation_target_rejected", UnknownRelationRejected),
            ("cross_publisher_correction_rejected", CrossPublisherCorrectionRejected),
            ("correction_truth_classification_mismatch_rejected", CorrectionClassificationMismatchRejected),
            ("duplicate_same_identity_is_idempotent", DuplicateSameIdentityIdempotent),
            ("duplicate_event_id_with_conflicting_identity_rejected", DuplicateConflictRejected),
            ("same_source_cannot_mint_second_event", SameSourceCannotMintSecondEvent),
            ("ordered_subscription_requires_key", OrderingKeyRequired),
            ("ordered_subscription_requires_sequence", OrderingSequenceRequired),
            ("unordered_subscription_rejects_sequence", UnorderedSequenceRejected),
            ("ordered_sequence_one_then_two_publishes", OrderedSequencePublishes),
            ("ordered_sequence_gap_rejected", OrderedSequenceGapRejected),
            ("independent_ordering_keys_are_isolated", IndependentOrderingKeysIsolated),
            ("correlation_causation_preserved", CorrelationCausationPreserved),
            ("publication_decision_journal_is_append_only", PublicationDecisionJournalAppendOnly),
            ("event_and_decision_identities_are_sha256", EventAndDecisionIdentitiesSha256),
            ("published_event_surface_is_immutable", PublishedEventSurfaceImmutable),
            ("publication_decision_surface_is_immutable", PublicationDecisionSurfaceImmutable),
            ("publication_audit_surface_is_immutable", PublicationAuditSurfaceImmutable),
            ("subscription_classification_order_is_deterministic", SubscriptionClassificationOrderDeterministic),
            ("equivalent_inputs_are_deterministic", EquivalentInputsDeterministic),
            ("evidence_mutation_changes_event_identity", EvidenceMutationChangesIdentity),
            ("authority_binding_evidence_mutation_changes_event_identity", AuthorityBindingEvidenceChangesIdentity),
            ("payload_business_semantics_remain_opaque", PayloadBusinessSemanticsOpaque),
            ("application_identity_receives_no_special_treatment", ApplicationNeutrality),
            ("event_surface_has_no_wp08_plus_operations", NoWp08PlusOperations)
        };

        var failures = new List<string>();
        foreach (var scenario in scenarios)
        {
            try
            {
                scenario.Test();
                Console.WriteLine($"PASS {scenario.Name}");
            }
            catch (Exception ex)
            {
                failures.Add($"{scenario.Name}: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"FAIL {scenario.Name}: {ex.Message}");
            }
        }

        Console.WriteLine($"RESULT {scenarios.Length - failures.Count}/{scenarios.Length} PASS");
        Console.WriteLine(failures.Count == 0
            ? "STAGE 5 WP-07 EVENT SYSTEM AND TRUTHFUL PUBLICATION VERIFIER: PASS"
            : "STAGE 5 WP-07 EVENT SYSTEM AND TRUTHFUL PUBLICATION VERIFIER: FAIL");
        foreach (var failure in failures) Console.Error.WriteLine($"DETAIL {failure}");
        return failures.Count == 0 ? 0 : 1;
    }

    private static void AuthoritativeEventPublishes() => AssertPublished(Evaluate(Fixture()));

    private static void PublishedEventBindsAdmissionDigest()
    {
        var f = Fixture();
        var published = RequirePublished(Evaluate(f));
        AssertEqual(f.Admission.MessageDigest, published.SourceEnvelopeDigest, "source_digest_not_preserved");
        AssertEqual(f.Admission.DecisionId, published.SourceAdmissionDecisionId, "admission_identity_not_preserved");
    }

    private static void PayloadSubstitutionRejected()
    {
        var f = Fixture();
        var substituted = Envelope(Publisher, SubscriberScope, "substituted", "{\"payload\":\"substituted\"}");
        AssertRejected(Evaluate(f with { Request = Request(f, sourceEnvelope: substituted) }), EventPublicationReason.SourceBindingMismatch);
    }

    private static void NonEventSourceRejected()
    {
        var f = Fixture();
        var command = Envelope(Publisher, SubscriberScope, "command", "opaque", FilMessageKind.Command);
        var admission = Admission(command, Publisher, admitted: true);
        var delivery = Delivery(command, admission, Publisher, dispatchable: true);
        var request = Request(f, sourceEnvelope: command, admission: admission, delivery: delivery,
            authority: PublicationAuthority(delivery, Publisher, EventTruthClassification.AuthoritativeOperational));
        AssertRejected(Evaluate(f with { Request = request }), EventPublicationReason.SourceNotEvent);
    }

    private static void NonAdmittedSourceRejected()
    {
        var f = Fixture();
        var admission = Admission(f.Envelope, Publisher, admitted: false);
        var delivery = Delivery(f.Envelope, admission, Publisher, dispatchable: true);
        var request = Request(f, admission: admission, delivery: delivery,
            authority: PublicationAuthority(delivery, Publisher, EventTruthClassification.AuthoritativeOperational));
        AssertRejected(Evaluate(f with { Request = request }), EventPublicationReason.AdmissionNotAdmitted);
    }

    private static void NonDispatchableDeliveryRejected()
    {
        var f = Fixture();
        var delivery = Delivery(f.Envelope, f.Admission, Publisher, dispatchable: false);
        var request = Request(f, delivery: delivery,
            authority: PublicationAuthority(delivery, Publisher, EventTruthClassification.AuthoritativeOperational));
        AssertRejected(Evaluate(f with { Request = request }), EventPublicationReason.DeliveryNotDispatchable);
    }

    private static void AdmissionDeliveryMismatchRejected()
    {
        var f = Fixture();
        var otherAdmission = Admission(f.Envelope, Publisher, admitted: true, decisionId: "admission:other");
        AssertRejected(Evaluate(f with { Request = Request(f, admission: otherAdmission) }), EventPublicationReason.SourceBindingMismatch);
    }

    private static void ProducerIdentityMismatchRejected()
    {
        var f = Fixture();
        var otherAuthority = PublicationAuthority(f.Delivery, "application.other", f.Request.Classification);
        var request = new EventPublicationRequest(f.Request.EventId, f.Request.EventType, "application.other",
            f.Request.SubscriberScope, f.Request.Classification, f.Request.RelationKind, f.Request.RelatedEventId,
            f.Request.OrderingKey, f.Request.SequenceNumber, f.Envelope, f.Admission, f.Delivery, otherAuthority,
            f.Request.ObservedAt, f.Request.JournalReference, f.Request.EvidenceReference);
        AssertRejected(Evaluate(f with { Request = request }), EventPublicationReason.SourceBindingMismatch);
    }

    private static void SubscriberAttributionPreserved()
    {
        var published = RequirePublished(Evaluate(Fixture()));
        AssertEqual(Subscriber, published.SubscriberApplicationId, "subscriber_application_not_preserved");
        AssertEqual(SubscriberScope, published.SubscriberScope, "subscriber_scope_not_preserved");
        AssertEqual(Publisher + "/component.publisher", published.ProducerIdentity, "producer_identity_not_preserved");
        AssertSha256(published.SubscriptionIdentity, "subscription_identity");
    }

    private static void MalformedClassificationFailsAtConstruction()
    {
        var f = Fixture();
        var threw = false;
        try
        {
            _ = new EventPublicationRequest("event:invalid", EventType, Publisher, SubscriberScope,
                (EventTruthClassification)999, EventRelationKind.None, null, null, 0, f.Envelope, f.Admission,
                f.Delivery, f.Request.AuthorityBinding, BaseTime, "journal:event/wp07", "evidence:event/wp07");
        }
        catch (ArgumentOutOfRangeException)
        {
            threw = true;
        }
        Assert(threw, "undefined_event_classification_was_accepted");
    }

    private static void PublicationAuthorityDeniedRejected() =>
        AssertPublicationAuthorityVariant(AuthorityDecision.Deny, BaseTime.AddMinutes(-1), BaseTime.AddMinutes(10), EventPublicationReason.PublicationAuthorityDenied);
    private static void PublicationAuthorityFutureRejected() =>
        AssertPublicationAuthorityVariant(AuthorityDecision.Allow, BaseTime.AddMinutes(1), BaseTime.AddMinutes(10), EventPublicationReason.PublicationAuthorityNotYetEffective);
    private static void PublicationAuthorityExpiredRejected() =>
        AssertPublicationAuthorityVariant(AuthorityDecision.Allow, BaseTime.AddMinutes(-2), BaseTime, EventPublicationReason.PublicationAuthorityExpired);

    private static void AssertPublicationAuthorityVariant(string decision, DateTimeOffset decisionTime, DateTimeOffset expiry, string expected)
    {
        var f = Fixture();
        var authority = PublicationAuthority(f.Delivery, Publisher, f.Request.Classification,
            decision: decision, decisionTime: decisionTime, expiry: expiry);
        AssertRejected(Evaluate(f with { Request = Request(f, authority: authority) }), expected);
    }

    private static void PublicationAuthorityMismatchRejected()
    {
        var f = Fixture();
        var authority = PublicationAuthority(f.Delivery, "application.other", f.Request.Classification);
        AssertRejected(Evaluate(f with { Request = Request(f, authority: authority) }), EventPublicationReason.PublicationAuthorityMismatch);
    }

    private static void SubscriptionAuthorityDeniedRejected() =>
        AssertSubscriptionAuthorityVariant(AuthorityDecision.Deny, BaseTime.AddMinutes(-1), BaseTime.AddMinutes(10), EventPublicationReason.SubscriptionAuthorityDenied);
    private static void SubscriptionAuthorityFutureRejected() =>
        AssertSubscriptionAuthorityVariant(AuthorityDecision.Allow, BaseTime.AddMinutes(1), BaseTime.AddMinutes(10), EventPublicationReason.SubscriptionAuthorityNotYetEffective);
    private static void SubscriptionAuthorityExpiredRejected() =>
        AssertSubscriptionAuthorityVariant(AuthorityDecision.Allow, BaseTime.AddMinutes(-2), BaseTime, EventPublicationReason.SubscriptionAuthorityExpired);

    private static void AssertSubscriptionAuthorityVariant(string decision, DateTimeOffset decisionTime, DateTimeOffset expiry, string expected)
    {
        var f = Fixture();
        var subscription = Subscription(new[] { EventTruthClassification.AuthoritativeOperational }, false,
            decision: decision, decisionTime: decisionTime, expiry: expiry);
        AssertRejected(Evaluate(f with { Subscription = subscription }), expected);
    }

    private static void SubscriptionAuthorityMismatchRejected()
    {
        var f = Fixture();
        var subscription = Subscription(new[] { EventTruthClassification.AuthoritativeOperational }, false,
            authorizedScope: "application.other/events/reference");
        AssertRejected(Evaluate(f with { Subscription = subscription }), EventPublicationReason.SubscriptionAuthorityMismatch);
    }

    private static void SubscriptionClassificationMismatchRejected()
    {
        var f = Fixture();
        var subscription = Subscription(new[] { EventTruthClassification.Replay }, false);
        AssertRejected(Evaluate(f with { Subscription = subscription }), EventPublicationReason.SourceBindingMismatch);
    }

    private static void ReplayRemainsNonAuthoritative()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal)));
        var replay = Fixture(journal: journal, eventId: "event:replay/0001", classification: EventTruthClassification.Replay,
            relation: EventRelationKind.ReplayOf, relatedEventId: "event:wp07/0001",
            acceptedClassifications: new[] { EventTruthClassification.Replay });
        AssertEqual(EventTruthClassification.Replay, RequirePublished(Evaluate(replay)).Classification, "replay_classification_changed");
    }

    private static void ReplayEscalationRejected()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal)));
        var replay = Fixture(journal: journal, eventId: "event:replay/0002",
            classification: EventTruthClassification.AuthoritativeOperational, relation: EventRelationKind.ReplayOf,
            relatedEventId: "event:wp07/0001");
        AssertRejected(Evaluate(replay), EventPublicationReason.ReplayOperationalEscalation);
    }

    private static void CorrectionPublishes()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal)));
        AssertPublished(Evaluate(Fixture(journal: journal, eventId: "event:correction/0001",
            relation: EventRelationKind.CorrectionOf, relatedEventId: "event:wp07/0001")));
    }

    private static void RelatedEventIdentityPreserved()
    {
        var journal = new EventJournal();
        var original = RequirePublished(Evaluate(Fixture(journal: journal)));
        var correction = RequirePublished(Evaluate(Fixture(journal: journal, eventId: "event:correction/identity",
            relation: EventRelationKind.CorrectionOf, relatedEventId: original.EventId)));
        AssertEqual(original.EventIdentity, correction.RelatedEventIdentity, "related_event_identity_not_preserved");
    }

    private static void UnknownRelationRejected() =>
        AssertRejected(Evaluate(Fixture(relation: EventRelationKind.CorrectionOf, relatedEventId: "event:unknown")), EventPublicationReason.RelationTargetUnknown);

    private static void CrossPublisherCorrectionRejected()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal)));
        var correction = Fixture(journal: journal, eventId: "event:correction/0002", publisher: "application.other",
            relation: EventRelationKind.CorrectionOf, relatedEventId: "event:wp07/0001");
        AssertRejected(Evaluate(correction), EventPublicationReason.RelationTargetMismatch);
    }

    private static void CorrectionClassificationMismatchRejected()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal)));
        var correction = Fixture(journal: journal, eventId: "event:correction/0003",
            classification: EventTruthClassification.NonAuthoritativeEvidence, relation: EventRelationKind.CorrectionOf,
            relatedEventId: "event:wp07/0001", acceptedClassifications: new[] { EventTruthClassification.NonAuthoritativeEvidence });
        AssertRejected(Evaluate(correction), EventPublicationReason.RelationTargetMismatch);
    }

    private static void DuplicateSameIdentityIdempotent()
    {
        var f = Fixture();
        AssertPublished(Evaluate(f));
        var duplicate = Evaluate(f);
        AssertEqual(EventPublicationDecisionKind.Duplicate, duplicate.Decision, "duplicate_not_idempotent");
        AssertEqual(EventPublicationReason.Duplicate, duplicate.Reason, "duplicate_reason_mismatch");
    }

    private static void DuplicateConflictRejected()
    {
        var f = Fixture();
        AssertPublished(Evaluate(f));
        AssertRejected(Evaluate(f with { Request = Request(f, evidence: "evidence:event/changed") }), EventPublicationReason.DuplicateConflict);
    }

    private static void SameSourceCannotMintSecondEvent()
    {
        var f = Fixture();
        AssertPublished(Evaluate(f));
        var second = new EventPublicationRequest("event:second-from-same-source", f.Request.EventType,
            f.Request.PublisherApplicationId, f.Request.SubscriberScope, f.Request.Classification, EventRelationKind.None,
            null, null, 0, f.Envelope, f.Admission, f.Delivery, f.Request.AuthorityBinding, f.Request.ObservedAt,
            f.Request.JournalReference, "evidence:event/second");
        AssertRejected(Evaluate(f with { Request = second }), EventPublicationReason.SourceAlreadyPublishedConflict);
    }

    private static void OrderingKeyRequired() =>
        AssertRejected(Evaluate(Fixture(requiresOrdering: true, orderingKey: null, sequence: 1)), EventPublicationReason.OrderingKeyRequired);
    private static void OrderingSequenceRequired() =>
        AssertRejected(Evaluate(Fixture(requiresOrdering: true, orderingKey: "order:alpha", sequence: 0)), EventPublicationReason.SequenceRequired);
    private static void UnorderedSequenceRejected() =>
        AssertRejected(Evaluate(Fixture(requiresOrdering: false, orderingKey: null, sequence: 1)), EventPublicationReason.SequenceUnexpected);

    private static void OrderedSequencePublishes()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal, eventId: "event:ordered/0001", requiresOrdering: true, orderingKey: "order:alpha", sequence: 1)));
        AssertPublished(Evaluate(Fixture(journal: journal, eventId: "event:ordered/0002", requiresOrdering: true, orderingKey: "order:alpha", sequence: 2)));
    }

    private static void OrderedSequenceGapRejected()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal, eventId: "event:ordered/0010", requiresOrdering: true, orderingKey: "order:alpha", sequence: 1)));
        AssertRejected(Evaluate(Fixture(journal: journal, eventId: "event:ordered/0012", requiresOrdering: true, orderingKey: "order:alpha", sequence: 3)), EventPublicationReason.SequenceViolation);
    }

    private static void IndependentOrderingKeysIsolated()
    {
        var journal = new EventJournal();
        AssertPublished(Evaluate(Fixture(journal: journal, eventId: "event:key-a/0001", requiresOrdering: true, orderingKey: "order:a", sequence: 1)));
        AssertPublished(Evaluate(Fixture(journal: journal, eventId: "event:key-b/0001", requiresOrdering: true, orderingKey: "order:b", sequence: 1)));
    }

    private static void CorrelationCausationPreserved()
    {
        var published = RequirePublished(Evaluate(Fixture()));
        AssertEqual("correlation:wp07/0001", published.CorrelationId, "correlation_not_preserved");
        AssertEqual("causation:wp07/0000", published.CausationId, "causation_not_preserved");
    }

    private static void PublicationDecisionJournalAppendOnly()
    {
        var f = Fixture();
        AssertPublished(Evaluate(f));
        _ = Evaluate(f);
        var audit = f.Journal.DecisionSnapshot();
        AssertEqual(2, audit.Count, "publication_decision_audit_count");
        AssertEqual(EventPublicationDecisionKind.Published, audit[0].Decision, "first_audit_not_published");
        AssertEqual(EventPublicationDecisionKind.Duplicate, audit[1].Decision, "second_audit_not_duplicate");
        AssertSha256(audit[0].AuditIdentity, "audit_identity");
    }

    private static void EventAndDecisionIdentitiesSha256()
    {
        var decision = Evaluate(Fixture());
        AssertSha256(decision.DecisionId, "decision_identity");
        AssertSha256(RequirePublished(decision).EventIdentity, "event_identity");
    }

    private static void PublishedEventSurfaceImmutable() => AssertNoPublicSetters(typeof(PublishedEvent), "published_event");
    private static void PublicationDecisionSurfaceImmutable() => AssertNoPublicSetters(typeof(EventPublicationDecision), "publication_decision");
    private static void PublicationAuditSurfaceImmutable() => AssertNoPublicSetters(typeof(EventPublicationAuditRecord), "publication_audit");

    private static void SubscriptionClassificationOrderDeterministic()
    {
        var first = Subscription(new[] { EventTruthClassification.Replay, EventTruthClassification.AuthoritativeOperational }, false);
        var second = Subscription(new[] { EventTruthClassification.AuthoritativeOperational, EventTruthClassification.Replay }, false);
        AssertEqual(first.SubscriptionIdentity, second.SubscriptionIdentity, "classification_order_changed_subscription_identity");
    }

    private static void EquivalentInputsDeterministic()
    {
        var first = Evaluate(Fixture());
        var second = Evaluate(Fixture());
        AssertEqual(first.DecisionId, second.DecisionId, "equivalent_decisions_not_deterministic");
        AssertEqual(RequirePublished(first).EventIdentity, RequirePublished(second).EventIdentity, "equivalent_events_not_deterministic");
    }

    private static void EvidenceMutationChangesIdentity()
    {
        var first = Evaluate(Fixture());
        var second = Evaluate(Fixture(eventId: "event:wp07/0001", evidence: "evidence:event/alternate"));
        Assert(RequirePublished(first).EventIdentity != RequirePublished(second).EventIdentity, "event_evidence_not_identity_material");
    }

    private static void AuthorityBindingEvidenceChangesIdentity()
    {
        var first = Fixture();
        var firstEvent = RequirePublished(Evaluate(first));
        var second = Fixture();
        var changedAuthority = PublicationAuthority(second.Delivery, Publisher, second.Request.Classification,
            bindingEvidence: "evidence:event-publication/alternate-binding");
        var secondEvent = RequirePublished(Evaluate(second with { Request = Request(second, authority: changedAuthority) }));
        Assert(firstEvent.EventIdentity != secondEvent.EventIdentity, "publication_authority_binding_evidence_not_identity_material");
    }

    private static void PayloadBusinessSemanticsOpaque()
    {
        AssertPublished(Evaluate(Fixture(payload: "{\"BUY\":\"NEVER_INTERPRET_ME\",\"price\":12345}")));
        var forbidden = new[] { "Trade", "TradingOrder", "Price", "Position", "Strategy" };
        var properties = typeof(PublishedEvent).GetProperties().Select(p => p.Name).ToArray();
        foreach (var token in forbidden)
            Assert(!properties.Any(name => name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"event_surface_interprets_business_payload:{token}");
    }

    private static void ApplicationNeutrality()
    {
        AssertPublished(Evaluate(Fixture(publisher: "application.alpha", eventId: "event:neutral/alpha")));
        AssertPublished(Evaluate(Fixture(publisher: "application.omega", eventId: "event:neutral/omega")));
    }

    private static void NoWp08PlusOperations()
    {
        var publicMethods = typeof(EventPublicationEvaluator).Assembly.GetExportedTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Select(m => m.Name).ToArray();
        var forbidden = new[] { "Encrypt", "Decrypt", "Sign", "VerifySignature", "Install", "Attach", "Upgrade", "Detach", "RemoveApplication" };
        foreach (var token in forbidden)
            Assert(!publicMethods.Any(name => name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"wp08_plus_surface_detected:{token}");
    }

    private static FixtureRecord Fixture(
        EventJournal? journal = null,
        string eventId = "event:wp07/0001",
        string publisher = Publisher,
        EventTruthClassification classification = EventTruthClassification.AuthoritativeOperational,
        EventRelationKind relation = EventRelationKind.None,
        string? relatedEventId = null,
        bool requiresOrdering = false,
        string? orderingKey = null,
        long sequence = 0,
        IReadOnlyCollection<EventTruthClassification>? acceptedClassifications = null,
        string payload = "{\"event\":\"opaque\"}",
        string evidence = "evidence:event/wp07")
    {
        var sourceSeed = Seed(eventId);
        var envelope = Envelope(publisher, SubscriberScope, sourceSeed, payload);
        var admission = Admission(envelope, publisher, admitted: true);
        var delivery = Delivery(envelope, admission, publisher, dispatchable: true);
        var classes = acceptedClassifications ?? new[] { classification };
        var subscription = Subscription(classes, requiresOrdering);
        var authority = PublicationAuthority(delivery, publisher, classification);
        var request = new EventPublicationRequest(eventId, EventType, publisher, SubscriberScope, classification, relation,
            relatedEventId, orderingKey, sequence, envelope, admission, delivery, authority, BaseTime,
            "journal:event/wp07", evidence);
        return new FixtureRecord(envelope, admission, delivery, subscription, request, journal ?? new EventJournal());
    }

    private static string Seed(string eventId) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(eventId)))[..16];

    private static EventPublicationRequest Request(
        FixtureRecord f,
        CanonicalFilEnvelope? sourceEnvelope = null,
        MessageAdmissionResult? admission = null,
        DeliveryDecision? delivery = null,
        EventPublicationAuthorityBinding? authority = null,
        string? evidence = null) =>
        new(f.Request.EventId, f.Request.EventType, f.Request.PublisherApplicationId, f.Request.SubscriberScope,
            f.Request.Classification, f.Request.RelationKind, f.Request.RelatedEventId, f.Request.OrderingKey,
            f.Request.SequenceNumber, sourceEnvelope ?? f.Envelope, admission ?? f.Admission, delivery ?? f.Delivery,
            authority ?? f.Request.AuthorityBinding, f.Request.ObservedAt, f.Request.JournalReference,
            evidence ?? f.Request.EvidenceReference);

    private static EventPublicationDecision Evaluate(FixtureRecord f) =>
        new EventPublicationEvaluator().Evaluate(f.Request, f.Subscription, f.Journal);

    private static CanonicalFilEnvelope Envelope(string producerApplicationId, string recipient, string seed, string payload,
        FilMessageKind kind = FilMessageKind.Event) =>
        CanonicalFilEnvelope.Create(
            new MessageIdentity("msg:wp07/" + seed), kind, FilMessageClassification.Operational, EventType,
            new SchemaIdentity(SchemaId), "1.0", new ProducerIdentityReference(producerApplicationId + "/component.publisher"),
            new RecipientScopeReference(recipient), new CorrelationIdentity("correlation:wp07/0001"),
            new CausationIdentity("causation:wp07/0000"), new AuthorityReference("authority:event-source/wp07"),
            new ProvenanceReference("evidence:event-source/wp07/" + seed), new IdempotencyIdentity("idempotency:wp07/" + seed),
            new DeliveryAttemptIdentity("attempt:wp07/" + seed), new RetryLineageIdentity("retry-lineage:wp07/" + seed),
            new CanonicalMessageTime(BaseTime.AddMinutes(-5), BaseTime.AddMinutes(30)),
            CanonicalOutcome.Unknown("processing_not_yet_attempted"), payload);

    private static MessageAdmissionResult Admission(CanonicalFilEnvelope envelope, string applicationId, bool admitted,
        string? decisionId = null)
    {
        var ctor = typeof(MessageAdmissionResult).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(candidate => candidate.GetParameters().Length == 19);
        return (MessageAdmissionResult)ctor.Invoke(new object?[]
        {
            admitted ? MessageAdmissionDecision.Admitted : MessageAdmissionDecision.Rejected,
            admitted ? MessageAdmissionReason.Admitted : MessageAdmissionReason.AuthorityDenied,
            decisionId ?? "admission:" + envelope.MessageId.Value,
            CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope),
            envelope.MessageId.Value,
            envelope.Producer.Value,
            applicationId,
            "manifest:wp07/reference",
            "1.0",
            envelope.RecipientScope.Value,
            Subscriber,
            envelope.SchemaId.Value,
            envelope.SchemaVersion,
            "authority-decision:admission/wp07",
            MessageAdmissionPurpose.FilMessageAdmission,
            "scope:message-admission/wp07",
            BaseTime.AddMinutes(-2),
            BaseTime.AddMinutes(20),
            "evidence:admission/wp07"
        });
    }

    private static DeliveryDecision Delivery(CanonicalFilEnvelope envelope, MessageAdmissionResult admission,
        string publisherApplicationId, bool dispatchable)
    {
        var ctor = typeof(DeliveryDecision).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(candidate => candidate.GetParameters().Length == 25);
        var correlation = envelope.CorrelationId?.Value ?? "NONE";
        var causation = envelope.CausationId?.Value ?? "NONE";
        return (DeliveryDecision)ctor.Invoke(new object?[]
        {
            dispatchable ? DeliveryDecisionKind.DispatchEligible : DeliveryDecisionKind.Rejected,
            dispatchable ? MessageDeliveryReason.DispatchEligible : MessageDeliveryReason.InvalidContext,
            dispatchable ? "delivery:" + admission.DecisionId : "delivery:rejected:" + admission.DecisionId,
            "route:decision/wp07",
            new string('A', 64),
            admission.DecisionId,
            envelope.MessageId.Value,
            correlation,
            causation,
            publisherApplicationId,
            "route:event/wp07",
            "1.0",
            "policy:delivery/wp07",
            "1.0",
            DeliveryGuarantee.AtLeastOnce,
            DeliveryOrderingGuarantee.None,
            string.Empty,
            DeliveryTrafficClass.Normal,
            1,
            string.Empty,
            "idempotency:" + envelope.MessageId.Value,
            DeliveryDestinationHealth.Healthy,
            new string('B', 64),
            BaseTime.AddMinutes(-1),
            "evidence:delivery/wp07"
        });
    }

    private static EventPublicationAuthorityBinding PublicationAuthority(
        DeliveryDecision delivery,
        string authorizedPublisher,
        EventTruthClassification classification,
        string decision = AuthorityDecision.Allow,
        DateTimeOffset? decisionTime = null,
        DateTimeOffset? expiry = null,
        string bindingEvidence = "evidence:event-publication/binding")
    {
        var result = new AuthorityResult("request:event-publication/wp07", "decision:event-publication/" + delivery.DecisionId, decision,
            EventPublicationPurpose.GovernedEventPublication, "policy:event-publication", "1.0",
            "conditions:bounded-event-publication", decision == AuthorityDecision.Allow ? "BOUNDED_TO_EVENT_PUBLICATION" : "DENIED",
            decision == AuthorityDecision.Allow ? AuthorityReason.Allowed : AuthorityReason.DefaultDeny,
            decisionTime ?? BaseTime.AddMinutes(-2), expiry ?? BaseTime.AddMinutes(20), "evidence:event-publication/result");
        return new EventPublicationAuthorityBinding("authority:event-publication/wp07", result, authorizedPublisher,
            EventType, SubscriberScope, classification, delivery.DecisionId,
            EventPublicationPurpose.GovernedEventPublication, bindingEvidence);
    }

    private static EventSubscription Subscription(
        IReadOnlyCollection<EventTruthClassification> classifications,
        bool requiresOrdering,
        string decision = AuthorityDecision.Allow,
        DateTimeOffset? decisionTime = null,
        DateTimeOffset? expiry = null,
        string? authorizedScope = null)
    {
        var digest = ClassificationDigest(classifications);
        var result = new AuthorityResult("request:event-subscription/wp07", "decision:event-subscription/wp07", decision,
            EventPublicationPurpose.GovernedEventSubscription, "policy:event-subscription", "1.0",
            "conditions:bounded-event-subscription", decision == AuthorityDecision.Allow ? "BOUNDED_TO_EVENT_SUBSCRIPTION" : "DENIED",
            decision == AuthorityDecision.Allow ? AuthorityReason.Allowed : AuthorityReason.DefaultDeny,
            decisionTime ?? BaseTime.AddMinutes(-2), expiry ?? BaseTime.AddMinutes(20), "evidence:event-subscription/result");
        var binding = new EventSubscriptionAuthorityBinding("authority:event-subscription/wp07", result,
            "subscription:wp07/reference", Subscriber, EventType, SchemaId, "1.0", authorizedScope ?? SubscriberScope,
            digest, EventPublicationPurpose.GovernedEventSubscription, "evidence:event-subscription/binding");
        return new EventSubscription("subscription:wp07/reference", Subscriber, EventType, SchemaId, "1.0",
            classifications, SubscriberScope, requiresOrdering, binding, "evidence:subscription/wp07");
    }

    private static string ClassificationDigest(IReadOnlyCollection<EventTruthClassification> classifications)
    {
        var ordered = classifications.Select(value => (int)value).Distinct().OrderBy(value => value)
            .Select(value => value.ToString(CultureInfo.InvariantCulture));
        return Hash(("classifications", string.Join(",", ordered)));
    }

    private static string Hash(params (string Name, string Value)[] fields)
    {
        var builder = new StringBuilder();
        foreach (var field in fields)
        {
            Append(builder, field.Name);
            Append(builder, field.Value);
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    private static void Append(StringBuilder builder, string value)
    {
        builder.Append(value.Length.ToString(CultureInfo.InvariantCulture));
        builder.Append(':');
        builder.Append(value);
        builder.Append('|');
    }

    private static PublishedEvent RequirePublished(EventPublicationDecision decision)
    {
        AssertPublished(decision);
        return decision.PublishedEvent ?? throw new InvalidOperationException("published_event_missing");
    }

    private static void AssertPublished(EventPublicationDecision decision)
    {
        AssertEqual(EventPublicationDecisionKind.Published, decision.Decision, $"expected_published:{decision.Reason}");
        AssertEqual(EventPublicationReason.Published, decision.Reason, "published_reason_mismatch");
    }

    private static void AssertRejected(EventPublicationDecision decision, string reason)
    {
        AssertEqual(EventPublicationDecisionKind.Rejected, decision.Decision, $"expected_rejected:{decision.Decision}:{decision.Reason}");
        AssertEqual(reason, decision.Reason, "rejection_reason_mismatch");
    }

    private static void AssertNoPublicSetters(Type type, string label)
    {
        var writable = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.SetMethod is { IsPublic: true }).ToArray();
        Assert(writable.Length == 0, $"{label}_has_public_setter:{string.Join(',', writable.Select(p => p.Name))}");
    }

    private static void AssertSha256(string value, string label)
    {
        Assert(value.Length == 64, label + ":length");
        Assert(value.All(c => c is >= '0' and <= '9' || c is >= 'A' and <= 'F'), label + ":format");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{message}:expected={expected}:actual={actual}");
    }

    private sealed record FixtureRecord(
        CanonicalFilEnvelope Envelope,
        MessageAdmissionResult Admission,
        DeliveryDecision Delivery,
        EventSubscription Subscription,
        EventPublicationRequest Request,
        EventJournal Journal);
}
