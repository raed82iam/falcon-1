using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;
using Foundation.MessageAdmission;
using Foundation.MessageDelivery;

namespace Foundation.EventSystem;

public enum EventTruthClassification
{
    AuthoritativeOperational = 1,
    Replay = 2,
    Test = 3,
    Simulation = 4,
    NonAuthoritativeEvidence = 5
}

public enum EventRelationKind
{
    None = 1,
    ReplayOf = 2,
    CorrectionOf = 3,
    Supersedes = 4
}

public enum EventPublicationDecisionKind
{
    Published = 1,
    Duplicate = 2,
    Rejected = 3
}

public static class EventPublicationPurpose
{
    public const string GovernedEventPublication = "event-publication";
    public const string GovernedEventSubscription = "event-subscription";
}

public static class EventPublicationReason
{
    public const string Published = "EVENT_PUBLISHED";
    public const string Duplicate = "EVENT_DUPLICATE_IDEMPOTENT";
    public const string InvalidContext = "INVALID_EVENT_PUBLICATION_CONTEXT";
    public const string SourceNotEvent = "EVENT_SOURCE_MESSAGE_KIND_INVALID";
    public const string AdmissionNotAdmitted = "EVENT_SOURCE_ADMISSION_NOT_ADMITTED";
    public const string DeliveryNotDispatchable = "EVENT_SOURCE_DELIVERY_NOT_DISPATCHABLE";
    public const string SourceBindingMismatch = "EVENT_SOURCE_BINDING_MISMATCH";
    public const string SourceAlreadyPublishedConflict = "EVENT_SOURCE_ALREADY_PUBLISHED_CONFLICT";
    public const string PublicationAuthorityMalformed = "EVENT_PUBLICATION_AUTHORITY_MALFORMED";
    public const string PublicationAuthorityMismatch = "EVENT_PUBLICATION_AUTHORITY_MISMATCH";
    public const string PublicationAuthorityDenied = "EVENT_PUBLICATION_AUTHORITY_DENIED";
    public const string PublicationAuthorityNotYetEffective = "EVENT_PUBLICATION_AUTHORITY_NOT_YET_EFFECTIVE";
    public const string PublicationAuthorityExpired = "EVENT_PUBLICATION_AUTHORITY_EXPIRED";
    public const string SubscriptionAuthorityMalformed = "EVENT_SUBSCRIPTION_AUTHORITY_MALFORMED";
    public const string SubscriptionAuthorityMismatch = "EVENT_SUBSCRIPTION_AUTHORITY_MISMATCH";
    public const string SubscriptionAuthorityDenied = "EVENT_SUBSCRIPTION_AUTHORITY_DENIED";
    public const string SubscriptionAuthorityNotYetEffective = "EVENT_SUBSCRIPTION_AUTHORITY_NOT_YET_EFFECTIVE";
    public const string SubscriptionAuthorityExpired = "EVENT_SUBSCRIPTION_AUTHORITY_EXPIRED";
    public const string ReplayOperationalEscalation = "EVENT_REPLAY_OPERATIONAL_ESCALATION_REJECTED";
    public const string RelationTargetRequired = "EVENT_RELATION_TARGET_REQUIRED";
    public const string RelationTargetUnknown = "EVENT_RELATION_TARGET_UNKNOWN";
    public const string RelationTargetMismatch = "EVENT_RELATION_TARGET_MISMATCH";
    public const string DuplicateConflict = "EVENT_DUPLICATE_IDENTITY_CONFLICT";
    public const string OrderingKeyRequired = "EVENT_ORDERING_KEY_REQUIRED";
    public const string OrderingKeyUnexpected = "EVENT_ORDERING_KEY_UNEXPECTED";
    public const string SequenceRequired = "EVENT_SEQUENCE_REQUIRED";
    public const string SequenceUnexpected = "EVENT_SEQUENCE_UNEXPECTED";
    public const string SequenceViolation = "EVENT_SEQUENCE_VIOLATION";
}

public sealed record EventPublicationAuthorityBinding
{
    public EventPublicationAuthorityBinding(
        string authorityReference,
        AuthorityResult authorityResult,
        string authorizedPublisherApplicationId,
        string authorizedEventType,
        string authorizedSubscriberScope,
        EventTruthClassification authorizedClassification,
        string authorizedSourceDeliveryDecisionId,
        string effectiveScope,
        string bindingEvidence)
    {
        AuthorityReference = EventRules.RequireIdentifier(authorityReference, nameof(authorityReference));
        AuthorityResult = authorityResult ?? throw new ArgumentNullException(nameof(authorityResult));
        AuthorizedPublisherApplicationId = EventRules.RequireIdentifier(authorizedPublisherApplicationId, nameof(authorizedPublisherApplicationId));
        AuthorizedEventType = EventRules.RequireIdentifier(authorizedEventType, nameof(authorizedEventType));
        AuthorizedSubscriberScope = EventRules.RequireIdentifier(authorizedSubscriberScope, nameof(authorizedSubscriberScope));
        AuthorizedClassification = EventRules.RequireDefined(authorizedClassification, nameof(authorizedClassification));
        AuthorizedSourceDeliveryDecisionId = EventRules.RequireIdentifier(authorizedSourceDeliveryDecisionId, nameof(authorizedSourceDeliveryDecisionId));
        EffectiveScope = EventRules.RequireIdentifier(effectiveScope, nameof(effectiveScope));
        BindingEvidence = EventRules.RequireCanonicalText(bindingEvidence, nameof(bindingEvidence));
    }

    public string AuthorityReference { get; }
    public AuthorityResult AuthorityResult { get; }
    public string AuthorizedPublisherApplicationId { get; }
    public string AuthorizedEventType { get; }
    public string AuthorizedSubscriberScope { get; }
    public EventTruthClassification AuthorizedClassification { get; }
    public string AuthorizedSourceDeliveryDecisionId { get; }
    public string EffectiveScope { get; }
    public string BindingEvidence { get; }
}

public sealed record EventSubscriptionAuthorityBinding
{
    public EventSubscriptionAuthorityBinding(
        string authorityReference,
        AuthorityResult authorityResult,
        string authorizedSubscriptionId,
        string authorizedSubscriberApplicationId,
        string authorizedEventType,
        string authorizedSchemaId,
        string authorizedSchemaVersion,
        string authorizedSubscriberScope,
        string authorizedClassificationsDigest,
        string effectiveScope,
        string bindingEvidence)
    {
        AuthorityReference = EventRules.RequireIdentifier(authorityReference, nameof(authorityReference));
        AuthorityResult = authorityResult ?? throw new ArgumentNullException(nameof(authorityResult));
        AuthorizedSubscriptionId = EventRules.RequireIdentifier(authorizedSubscriptionId, nameof(authorizedSubscriptionId));
        AuthorizedSubscriberApplicationId = EventRules.RequireIdentifier(authorizedSubscriberApplicationId, nameof(authorizedSubscriberApplicationId));
        AuthorizedEventType = EventRules.RequireIdentifier(authorizedEventType, nameof(authorizedEventType));
        AuthorizedSchemaId = EventRules.RequireIdentifier(authorizedSchemaId, nameof(authorizedSchemaId));
        AuthorizedSchemaVersion = EventRules.RequireVersion(authorizedSchemaVersion, nameof(authorizedSchemaVersion));
        AuthorizedSubscriberScope = EventRules.RequireIdentifier(authorizedSubscriberScope, nameof(authorizedSubscriberScope));
        AuthorizedClassificationsDigest = EventRules.RequireSha256(authorizedClassificationsDigest, nameof(authorizedClassificationsDigest));
        EffectiveScope = EventRules.RequireIdentifier(effectiveScope, nameof(effectiveScope));
        BindingEvidence = EventRules.RequireCanonicalText(bindingEvidence, nameof(bindingEvidence));
    }

    public string AuthorityReference { get; }
    public AuthorityResult AuthorityResult { get; }
    public string AuthorizedSubscriptionId { get; }
    public string AuthorizedSubscriberApplicationId { get; }
    public string AuthorizedEventType { get; }
    public string AuthorizedSchemaId { get; }
    public string AuthorizedSchemaVersion { get; }
    public string AuthorizedSubscriberScope { get; }
    public string AuthorizedClassificationsDigest { get; }
    public string EffectiveScope { get; }
    public string BindingEvidence { get; }
}

public sealed record EventSubscription
{
    public EventSubscription(
        string subscriptionId,
        string subscriberApplicationId,
        string eventType,
        string schemaId,
        string schemaVersion,
        IReadOnlyCollection<EventTruthClassification> acceptedClassifications,
        string subscriberScope,
        bool requiresOrderingKey,
        EventSubscriptionAuthorityBinding authorityBinding,
        string evidenceReference)
    {
        SubscriptionId = EventRules.RequireIdentifier(subscriptionId, nameof(subscriptionId));
        SubscriberApplicationId = EventRules.RequireIdentifier(subscriberApplicationId, nameof(subscriberApplicationId));
        EventType = EventRules.RequireIdentifier(eventType, nameof(eventType));
        SchemaId = EventRules.RequireIdentifier(schemaId, nameof(schemaId));
        SchemaVersion = EventRules.RequireVersion(schemaVersion, nameof(schemaVersion));
        if (acceptedClassifications is null || acceptedClassifications.Count == 0)
            throw new ArgumentException("accepted_classifications_required", nameof(acceptedClassifications));

        var unique = new HashSet<EventTruthClassification>();
        foreach (var classification in acceptedClassifications)
        {
            EventRules.RequireDefined(classification, nameof(acceptedClassifications));
            if (!unique.Add(classification))
                throw new ArgumentException("duplicate_event_classification", nameof(acceptedClassifications));
        }

        var ordered = unique.OrderBy(value => (int)value).ToArray();
        AcceptedClassifications = new ReadOnlyCollection<EventTruthClassification>(ordered);
        AcceptedClassificationsDigest = EventCanonicalization.Hash(("classifications", EventCanonicalization.JoinClassifications(AcceptedClassifications)));
        SubscriberScope = EventRules.RequireIdentifier(subscriberScope, nameof(subscriberScope));
        RequiresOrderingKey = requiresOrderingKey;
        AuthorityBinding = authorityBinding ?? throw new ArgumentNullException(nameof(authorityBinding));
        EvidenceReference = EventRules.RequireCanonicalText(evidenceReference, nameof(evidenceReference));
        SubscriptionIdentity = EventCanonicalization.Hash(
            ("subscription_id", SubscriptionId),
            ("subscriber_application_id", SubscriberApplicationId),
            ("event_type", EventType),
            ("schema_id", SchemaId),
            ("schema_version", SchemaVersion),
            ("classifications_digest", AcceptedClassificationsDigest),
            ("subscriber_scope", SubscriberScope),
            ("requires_ordering_key", RequiresOrderingKey ? "1" : "0"),
            ("authority_reference", AuthorityBinding.AuthorityReference),
            ("authority_decision_id", AuthorityBinding.AuthorityResult.DecisionId),
            ("authority_effective_scope", AuthorityBinding.EffectiveScope),
            ("authority_binding_evidence", AuthorityBinding.BindingEvidence),
            ("evidence", EvidenceReference));
    }

    public string SubscriptionId { get; }
    public string SubscriberApplicationId { get; }
    public string EventType { get; }
    public string SchemaId { get; }
    public string SchemaVersion { get; }
    public IReadOnlyList<EventTruthClassification> AcceptedClassifications { get; }
    public string AcceptedClassificationsDigest { get; }
    public string SubscriberScope { get; }
    public bool RequiresOrderingKey { get; }
    public EventSubscriptionAuthorityBinding AuthorityBinding { get; }
    public string EvidenceReference { get; }
    public string SubscriptionIdentity { get; }
    public bool Accepts(EventTruthClassification classification) => AcceptedClassifications.Contains(classification);
}

public sealed record EventPublicationRequest
{
    public EventPublicationRequest(
        string eventId,
        string eventType,
        string publisherApplicationId,
        string subscriberScope,
        EventTruthClassification classification,
        EventRelationKind relationKind,
        string? relatedEventId,
        string? orderingKey,
        long sequenceNumber,
        CanonicalFilEnvelope sourceEnvelope,
        MessageAdmissionResult sourceAdmissionResult,
        DeliveryDecision sourceDeliveryDecision,
        EventPublicationAuthorityBinding authorityBinding,
        DateTimeOffset observedAt,
        string journalReference,
        string evidenceReference)
    {
        EventId = EventRules.RequireIdentifier(eventId, nameof(eventId));
        EventType = EventRules.RequireIdentifier(eventType, nameof(eventType));
        PublisherApplicationId = EventRules.RequireIdentifier(publisherApplicationId, nameof(publisherApplicationId));
        SubscriberScope = EventRules.RequireIdentifier(subscriberScope, nameof(subscriberScope));
        Classification = EventRules.RequireDefined(classification, nameof(classification));
        RelationKind = EventRules.RequireDefined(relationKind, nameof(relationKind));
        if (relationKind == EventRelationKind.None && relatedEventId is not null)
            throw new ArgumentException("related_event_not_allowed_without_relation", nameof(relatedEventId));
        if (relationKind != EventRelationKind.None)
            RelatedEventId = EventRules.RequireIdentifier(relatedEventId ?? string.Empty, nameof(relatedEventId));
        OrderingKey = orderingKey is null ? null : EventRules.RequireIdentifier(orderingKey, nameof(orderingKey));
        if (sequenceNumber < 0) throw new ArgumentOutOfRangeException(nameof(sequenceNumber));
        SequenceNumber = sequenceNumber;
        SourceEnvelope = sourceEnvelope ?? throw new ArgumentNullException(nameof(sourceEnvelope));
        SourceAdmissionResult = sourceAdmissionResult ?? throw new ArgumentNullException(nameof(sourceAdmissionResult));
        SourceDeliveryDecision = sourceDeliveryDecision ?? throw new ArgumentNullException(nameof(sourceDeliveryDecision));
        AuthorityBinding = authorityBinding ?? throw new ArgumentNullException(nameof(authorityBinding));
        EventRules.RequireUtc(observedAt, nameof(observedAt));
        ObservedAt = observedAt;
        JournalReference = EventRules.RequireCanonicalText(journalReference, nameof(journalReference));
        EvidenceReference = EventRules.RequireCanonicalText(evidenceReference, nameof(evidenceReference));
    }

    public string EventId { get; }
    public string EventType { get; }
    public string PublisherApplicationId { get; }
    public string SubscriberScope { get; }
    public EventTruthClassification Classification { get; }
    public EventRelationKind RelationKind { get; }
    public string? RelatedEventId { get; }
    public string? OrderingKey { get; }
    public long SequenceNumber { get; }
    public CanonicalFilEnvelope SourceEnvelope { get; }
    public MessageAdmissionResult SourceAdmissionResult { get; }
    public DeliveryDecision SourceDeliveryDecision { get; }
    public EventPublicationAuthorityBinding AuthorityBinding { get; }
    public DateTimeOffset ObservedAt { get; }
    public string JournalReference { get; }
    public string EvidenceReference { get; }
}

public sealed record PublishedEvent
{
    internal PublishedEvent(
        string eventIdentity,
        string eventId,
        string eventType,
        string publisherApplicationId,
        string producerIdentity,
        string subscriberApplicationId,
        string subscriberScope,
        string subscriptionIdentity,
        EventTruthClassification classification,
        EventRelationKind relationKind,
        string relatedEventId,
        string relatedEventIdentity,
        string orderingKey,
        long sequenceNumber,
        string sourceMessageId,
        string sourceAdmissionDecisionId,
        string sourceDeliveryDecisionId,
        string sourceEnvelopeDigest,
        string schemaId,
        string schemaVersion,
        string correlationId,
        string causationId,
        string publicationAuthorityDecisionId,
        DateTimeOffset observedAt,
        string journalReference,
        string evidenceReference)
    {
        EventIdentity = eventIdentity;
        EventId = eventId;
        EventType = eventType;
        PublisherApplicationId = publisherApplicationId;
        ProducerIdentity = producerIdentity;
        SubscriberApplicationId = subscriberApplicationId;
        SubscriberScope = subscriberScope;
        SubscriptionIdentity = subscriptionIdentity;
        Classification = classification;
        RelationKind = relationKind;
        RelatedEventId = relatedEventId;
        RelatedEventIdentity = relatedEventIdentity;
        OrderingKey = orderingKey;
        SequenceNumber = sequenceNumber;
        SourceMessageId = sourceMessageId;
        SourceAdmissionDecisionId = sourceAdmissionDecisionId;
        SourceDeliveryDecisionId = sourceDeliveryDecisionId;
        SourceEnvelopeDigest = sourceEnvelopeDigest;
        SchemaId = schemaId;
        SchemaVersion = schemaVersion;
        CorrelationId = correlationId;
        CausationId = causationId;
        PublicationAuthorityDecisionId = publicationAuthorityDecisionId;
        ObservedAt = observedAt;
        JournalReference = journalReference;
        EvidenceReference = evidenceReference;
    }

    public string EventIdentity { get; }
    public string EventId { get; }
    public string EventType { get; }
    public string PublisherApplicationId { get; }
    public string ProducerIdentity { get; }
    public string SubscriberApplicationId { get; }
    public string SubscriberScope { get; }
    public string SubscriptionIdentity { get; }
    public EventTruthClassification Classification { get; }
    public EventRelationKind RelationKind { get; }
    public string RelatedEventId { get; }
    public string RelatedEventIdentity { get; }
    public string OrderingKey { get; }
    public long SequenceNumber { get; }
    public string SourceMessageId { get; }
    public string SourceAdmissionDecisionId { get; }
    public string SourceDeliveryDecisionId { get; }
    public string SourceEnvelopeDigest { get; }
    public string SchemaId { get; }
    public string SchemaVersion { get; }
    public string CorrelationId { get; }
    public string CausationId { get; }
    public string PublicationAuthorityDecisionId { get; }
    public DateTimeOffset ObservedAt { get; }
    public string JournalReference { get; }
    public string EvidenceReference { get; }
}

public sealed record EventPublicationDecision
{
    internal EventPublicationDecision(EventPublicationDecisionKind decision, string reason, string decisionId,
        PublishedEvent? publishedEvent, string subscriptionIdentity, DateTimeOffset observedAt)
    {
        Decision = decision;
        Reason = reason;
        DecisionId = decisionId;
        PublishedEvent = publishedEvent;
        SubscriptionIdentity = subscriptionIdentity;
        ObservedAt = observedAt;
    }

    public EventPublicationDecisionKind Decision { get; }
    public string Reason { get; }
    public string DecisionId { get; }
    public PublishedEvent? PublishedEvent { get; }
    public string SubscriptionIdentity { get; }
    public DateTimeOffset ObservedAt { get; }
    public bool IsPublished => Decision == EventPublicationDecisionKind.Published;
    public bool IsDuplicate => Decision == EventPublicationDecisionKind.Duplicate;
}

public sealed record EventPublicationAuditRecord
{
    internal EventPublicationAuditRecord(string auditIdentity, string decisionId, EventPublicationDecisionKind decision,
        string reason, string eventId, string eventIdentity, string subscriptionIdentity, DateTimeOffset observedAt)
    {
        AuditIdentity = auditIdentity;
        DecisionId = decisionId;
        Decision = decision;
        Reason = reason;
        EventId = eventId;
        EventIdentity = eventIdentity;
        SubscriptionIdentity = subscriptionIdentity;
        ObservedAt = observedAt;
    }

    public string AuditIdentity { get; }
    public string DecisionId { get; }
    public EventPublicationDecisionKind Decision { get; }
    public string Reason { get; }
    public string EventId { get; }
    public string EventIdentity { get; }
    public string SubscriptionIdentity { get; }
    public DateTimeOffset ObservedAt { get; }
}

public sealed class EventJournal
{
    private readonly Dictionary<string, PublishedEvent> events = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> sourceTruthBindings = new(StringComparer.Ordinal);
    private readonly Dictionary<string, long> orderedSequences = new(StringComparer.Ordinal);
    private readonly List<EventPublicationAuditRecord> decisionAudit = new();

    public PublishedEvent? Resolve(string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId)) return null;
        return events.TryGetValue(eventId, out var value) ? value : null;
    }

    public IReadOnlyList<PublishedEvent> Snapshot()
    {
        var values = new List<PublishedEvent>(events.Values);
        values.Sort((left, right) => StringComparer.Ordinal.Compare(left.EventId, right.EventId));
        return new ReadOnlyCollection<PublishedEvent>(values);
    }

    public IReadOnlyList<EventPublicationAuditRecord> DecisionSnapshot() =>
        new ReadOnlyCollection<EventPublicationAuditRecord>(new List<EventPublicationAuditRecord>(decisionAudit));

    internal EventPublicationDecision Record(EventPublicationRequest request, EventSubscription subscription,
        PublishedEvent candidate, DateTimeOffset observedAt)
    {
        if (events.TryGetValue(candidate.EventId, out var existing))
        {
            var same = StringComparer.Ordinal.Equals(existing.EventIdentity, candidate.EventIdentity);
            return EventPublicationEvaluator.Decide(this, request, subscription,
                same ? EventPublicationDecisionKind.Duplicate : EventPublicationDecisionKind.Rejected,
                same ? EventPublicationReason.Duplicate : EventPublicationReason.DuplicateConflict,
                same ? existing : null, observedAt);
        }

        var sourceTruthKey = EventCanonicalization.Hash(
            ("source_envelope_digest", candidate.SourceEnvelopeDigest),
            ("source_admission_decision_id", candidate.SourceAdmissionDecisionId));
        if (sourceTruthBindings.TryGetValue(sourceTruthKey, out var existingEventIdentity) &&
            !StringComparer.Ordinal.Equals(existingEventIdentity, candidate.EventIdentity))
        {
            return EventPublicationEvaluator.Decide(this, request, subscription, EventPublicationDecisionKind.Rejected,
                EventPublicationReason.SourceAlreadyPublishedConflict, null, observedAt);
        }

        if (subscription.RequiresOrderingKey)
        {
            var sequenceScope = EventCanonicalization.Hash(
                ("subscription_identity", subscription.SubscriptionIdentity),
                ("publisher_application_id", request.PublisherApplicationId),
                ("ordering_key", candidate.OrderingKey));
            var expected = orderedSequences.TryGetValue(sequenceScope, out var previous) ? previous + 1 : 1;
            if (candidate.SequenceNumber != expected)
                return EventPublicationEvaluator.Decide(this, request, subscription, EventPublicationDecisionKind.Rejected,
                    EventPublicationReason.SequenceViolation, null, observedAt);
            orderedSequences[sequenceScope] = candidate.SequenceNumber;
        }

        events.Add(candidate.EventId, candidate);
        sourceTruthBindings[sourceTruthKey] = candidate.EventIdentity;
        return EventPublicationEvaluator.Decide(this, request, subscription, EventPublicationDecisionKind.Published,
            EventPublicationReason.Published, candidate, observedAt);
    }

    internal void RecordDecision(EventPublicationDecision decision, string eventId)
    {
        var eventIdentity = decision.PublishedEvent?.EventIdentity ?? "UNAVAILABLE";
        var auditIdentity = EventCanonicalization.Hash(
            ("decision_id", decision.DecisionId),
            ("decision", ((int)decision.Decision).ToString(CultureInfo.InvariantCulture)),
            ("reason", decision.Reason),
            ("event_id", eventId),
            ("event_identity", eventIdentity),
            ("subscription_identity", decision.SubscriptionIdentity),
            ("observed_at", decision.ObservedAt.ToString("O", CultureInfo.InvariantCulture)));
        decisionAudit.Add(new EventPublicationAuditRecord(auditIdentity, decision.DecisionId, decision.Decision,
            decision.Reason, eventId, eventIdentity, decision.SubscriptionIdentity, decision.ObservedAt));
    }
}

public sealed class EventPublicationEvaluator
{
    private const string MissingTraceIdentity = "NONE";

    public EventPublicationDecision Evaluate(EventPublicationRequest? request, EventSubscription? subscription, EventJournal? journal)
    {
        var observedAt = request?.ObservedAt ?? DateTimeOffset.UnixEpoch;
        if (request is null || subscription is null || journal is null)
            return CreateDecision(request, subscription, EventPublicationDecisionKind.Rejected,
                EventPublicationReason.InvalidContext, null, observedAt);

        var envelope = request.SourceEnvelope;
        var admission = request.SourceAdmissionResult;
        var delivery = request.SourceDeliveryDecision;

        if (envelope.MessageKind != FilMessageKind.Event)
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.SourceNotEvent, null, observedAt);
        if (!admission.IsAdmitted)
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.AdmissionNotAdmitted, null, observedAt);
        if (!delivery.CanDispatch)
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.DeliveryNotDispatchable, null, observedAt);
        if (!SourceMatches(request, subscription))
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.SourceBindingMismatch, null, observedAt);

        var publicationAuthorityFailure = ValidatePublicationAuthority(request);
        if (publicationAuthorityFailure is not null)
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, publicationAuthorityFailure, null, observedAt);

        var subscriptionAuthorityFailure = ValidateSubscriptionAuthority(subscription, observedAt);
        if (subscriptionAuthorityFailure is not null)
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, subscriptionAuthorityFailure, null, observedAt);

        if (!SubscriptionMatches(request, subscription))
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.SourceBindingMismatch, null, observedAt);
        if (request.RelationKind == EventRelationKind.ReplayOf && request.Classification == EventTruthClassification.AuthoritativeOperational)
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.ReplayOperationalEscalation, null, observedAt);
        if (!ValidateOrdering(request, subscription, out var orderingReason))
            return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, orderingReason, null, observedAt);

        PublishedEvent? related = null;
        if (request.RelationKind != EventRelationKind.None)
        {
            if (string.IsNullOrWhiteSpace(request.RelatedEventId))
                return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.RelationTargetRequired, null, observedAt);
            related = journal.Resolve(request.RelatedEventId);
            if (related is null)
                return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.RelationTargetUnknown, null, observedAt);
            if (!RelationMatches(request, related))
                return Decide(journal, request, subscription, EventPublicationDecisionKind.Rejected, EventPublicationReason.RelationTargetMismatch, null, observedAt);
        }

        return journal.Record(request, subscription, BuildPublishedEvent(request, subscription, related), observedAt);
    }

    private static bool SourceMatches(EventPublicationRequest request, EventSubscription subscription)
    {
        var envelope = request.SourceEnvelope;
        var admission = request.SourceAdmissionResult;
        var delivery = request.SourceDeliveryDecision;
        var digest = CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope);
        var correlationId = TraceValue(envelope.CorrelationId?.Value);
        var causationId = TraceValue(envelope.CausationId?.Value);

        return StringComparer.Ordinal.Equals(digest, admission.MessageDigest) &&
               StringComparer.Ordinal.Equals(admission.DecisionId, delivery.AdmissionDecisionId) &&
               StringComparer.Ordinal.Equals(admission.MessageId, envelope.MessageId.Value) &&
               StringComparer.Ordinal.Equals(admission.ProducerIdentity, envelope.Producer.Value) &&
               StringComparer.Ordinal.Equals(admission.ProducerApplicationId, request.PublisherApplicationId) &&
               StringComparer.Ordinal.Equals(admission.RecipientScope, envelope.RecipientScope.Value) &&
               StringComparer.Ordinal.Equals(admission.IntendedConsumer, subscription.SubscriberApplicationId) &&
               StringComparer.Ordinal.Equals(admission.SchemaId, envelope.SchemaId.Value) &&
               StringComparer.Ordinal.Equals(admission.SchemaVersion, envelope.SchemaVersion) &&
               StringComparer.Ordinal.Equals(delivery.MessageId, envelope.MessageId.Value) &&
               StringComparer.Ordinal.Equals(delivery.ProducerApplicationId, request.PublisherApplicationId) &&
               StringComparer.Ordinal.Equals(delivery.CorrelationId, correlationId) &&
               StringComparer.Ordinal.Equals(delivery.CausationId, causationId) &&
               request.ObservedAt >= delivery.ObservationTime;
    }

    private static string? ValidatePublicationAuthority(EventPublicationRequest request)
    {
        var binding = request.AuthorityBinding;
        var result = binding.AuthorityResult;
        if (ContractValidators.Validate(result).Result != ValidationResult.Pass)
            return EventPublicationReason.PublicationAuthorityMalformed;
        if (!StringComparer.Ordinal.Equals(binding.AuthorizedPublisherApplicationId, request.PublisherApplicationId) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedEventType, request.EventType) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedSubscriberScope, request.SubscriberScope) ||
            binding.AuthorizedClassification != request.Classification ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedSourceDeliveryDecisionId, request.SourceDeliveryDecision.DecisionId) ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, result.EffectiveScope) ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, EventPublicationPurpose.GovernedEventPublication))
            return EventPublicationReason.PublicationAuthorityMismatch;
        if (!StringComparer.Ordinal.Equals(result.Decision, AuthorityDecision.Allow))
            return EventPublicationReason.PublicationAuthorityDenied;
        if (request.ObservedAt < result.DecisionTime)
            return EventPublicationReason.PublicationAuthorityNotYetEffective;
        if (request.ObservedAt >= result.Expiry)
            return EventPublicationReason.PublicationAuthorityExpired;
        return null;
    }

    private static string? ValidateSubscriptionAuthority(EventSubscription subscription, DateTimeOffset observedAt)
    {
        var binding = subscription.AuthorityBinding;
        var result = binding.AuthorityResult;
        if (ContractValidators.Validate(result).Result != ValidationResult.Pass)
            return EventPublicationReason.SubscriptionAuthorityMalformed;
        if (!StringComparer.Ordinal.Equals(binding.AuthorizedSubscriptionId, subscription.SubscriptionId) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedSubscriberApplicationId, subscription.SubscriberApplicationId) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedEventType, subscription.EventType) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedSchemaId, subscription.SchemaId) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedSchemaVersion, subscription.SchemaVersion) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedSubscriberScope, subscription.SubscriberScope) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedClassificationsDigest, subscription.AcceptedClassificationsDigest) ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, result.EffectiveScope) ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, EventPublicationPurpose.GovernedEventSubscription))
            return EventPublicationReason.SubscriptionAuthorityMismatch;
        if (!StringComparer.Ordinal.Equals(result.Decision, AuthorityDecision.Allow))
            return EventPublicationReason.SubscriptionAuthorityDenied;
        if (observedAt < result.DecisionTime)
            return EventPublicationReason.SubscriptionAuthorityNotYetEffective;
        if (observedAt >= result.Expiry)
            return EventPublicationReason.SubscriptionAuthorityExpired;
        return null;
    }

    private static bool SubscriptionMatches(EventPublicationRequest request, EventSubscription subscription) =>
        StringComparer.Ordinal.Equals(subscription.EventType, request.EventType) &&
        StringComparer.Ordinal.Equals(subscription.SchemaId, request.SourceEnvelope.SchemaId.Value) &&
        StringComparer.Ordinal.Equals(subscription.SchemaVersion, request.SourceEnvelope.SchemaVersion) &&
        StringComparer.Ordinal.Equals(subscription.SubscriberScope, request.SubscriberScope) &&
        subscription.Accepts(request.Classification);

    private static bool ValidateOrdering(EventPublicationRequest request, EventSubscription subscription, out string reason)
    {
        if (subscription.RequiresOrderingKey)
        {
            if (string.IsNullOrWhiteSpace(request.OrderingKey))
            {
                reason = EventPublicationReason.OrderingKeyRequired;
                return false;
            }
            if (request.SequenceNumber < 1)
            {
                reason = EventPublicationReason.SequenceRequired;
                return false;
            }
        }
        else
        {
            if (request.OrderingKey is not null)
            {
                reason = EventPublicationReason.OrderingKeyUnexpected;
                return false;
            }
            if (request.SequenceNumber != 0)
            {
                reason = EventPublicationReason.SequenceUnexpected;
                return false;
            }
        }
        reason = string.Empty;
        return true;
    }

    private static bool RelationMatches(EventPublicationRequest request, PublishedEvent related)
    {
        if (!StringComparer.Ordinal.Equals(request.EventType, related.EventType)) return false;
        if (!StringComparer.Ordinal.Equals(request.SourceEnvelope.SchemaId.Value, related.SchemaId)) return false;
        if (!StringComparer.Ordinal.Equals(request.SourceEnvelope.SchemaVersion, related.SchemaVersion)) return false;
        if (request.RelationKind == EventRelationKind.ReplayOf)
            return request.Classification != EventTruthClassification.AuthoritativeOperational;
        if (request.RelationKind is EventRelationKind.CorrectionOf or EventRelationKind.Supersedes)
            return StringComparer.Ordinal.Equals(request.PublisherApplicationId, related.PublisherApplicationId) &&
                   request.Classification == related.Classification;
        return true;
    }

    private static PublishedEvent BuildPublishedEvent(EventPublicationRequest request, EventSubscription subscription, PublishedEvent? related)
    {
        var envelope = request.SourceEnvelope;
        var admission = request.SourceAdmissionResult;
        var delivery = request.SourceDeliveryDecision;
        var sourceDigest = CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope);
        var correlationId = TraceValue(envelope.CorrelationId?.Value);
        var causationId = TraceValue(envelope.CausationId?.Value);
        var relatedEventId = request.RelatedEventId ?? string.Empty;
        var relatedEventIdentity = related?.EventIdentity ?? string.Empty;
        var orderingKey = request.OrderingKey ?? string.Empty;
        var authority = request.AuthorityBinding;

        var eventIdentity = EventCanonicalization.Hash(
            ("event_id", request.EventId),
            ("event_type", request.EventType),
            ("publisher_application_id", request.PublisherApplicationId),
            ("producer_identity", admission.ProducerIdentity),
            ("subscriber_application_id", subscription.SubscriberApplicationId),
            ("subscriber_scope", request.SubscriberScope),
            ("subscription_identity", subscription.SubscriptionIdentity),
            ("classification", ((int)request.Classification).ToString(CultureInfo.InvariantCulture)),
            ("relation_kind", ((int)request.RelationKind).ToString(CultureInfo.InvariantCulture)),
            ("related_event_id", relatedEventId),
            ("related_event_identity", relatedEventIdentity),
            ("ordering_key", orderingKey),
            ("sequence_number", request.SequenceNumber.ToString(CultureInfo.InvariantCulture)),
            ("source_message_id", envelope.MessageId.Value),
            ("source_admission_decision_id", admission.DecisionId),
            ("source_delivery_decision_id", delivery.DecisionId),
            ("source_envelope_digest", sourceDigest),
            ("schema_id", envelope.SchemaId.Value),
            ("schema_version", envelope.SchemaVersion),
            ("correlation_id", correlationId),
            ("causation_id", causationId),
            ("publication_authority_reference", authority.AuthorityReference),
            ("publication_authority_decision_id", authority.AuthorityResult.DecisionId),
            ("publication_authority_effective_scope", authority.EffectiveScope),
            ("publication_authority_binding_evidence", authority.BindingEvidence),
            ("observed_at", request.ObservedAt.ToString("O", CultureInfo.InvariantCulture)),
            ("journal_reference", request.JournalReference),
            ("evidence", request.EvidenceReference));

        return new PublishedEvent(eventIdentity, request.EventId, request.EventType, request.PublisherApplicationId,
            admission.ProducerIdentity, subscription.SubscriberApplicationId, request.SubscriberScope,
            subscription.SubscriptionIdentity, request.Classification, request.RelationKind, relatedEventId,
            relatedEventIdentity, orderingKey, request.SequenceNumber, envelope.MessageId.Value, admission.DecisionId,
            delivery.DecisionId, sourceDigest, envelope.SchemaId.Value, envelope.SchemaVersion, correlationId, causationId,
            authority.AuthorityResult.DecisionId, request.ObservedAt, request.JournalReference, request.EvidenceReference);
    }

    internal static EventPublicationDecision Decide(EventJournal journal, EventPublicationRequest request,
        EventSubscription subscription, EventPublicationDecisionKind decision, string reason,
        PublishedEvent? publishedEvent, DateTimeOffset observedAt)
    {
        var result = CreateDecision(request, subscription, decision, reason, publishedEvent, observedAt);
        journal.RecordDecision(result, request.EventId);
        return result;
    }

    internal static EventPublicationDecision CreateDecision(EventPublicationRequest? request,
        EventSubscription? subscription, EventPublicationDecisionKind decision, string reason,
        PublishedEvent? publishedEvent, DateTimeOffset observedAt)
    {
        var decisionId = EventCanonicalization.Hash(
            ("decision", ((int)decision).ToString(CultureInfo.InvariantCulture)),
            ("reason", reason),
            ("event_id", request?.EventId ?? "UNAVAILABLE"),
            ("event_identity", publishedEvent?.EventIdentity ?? "UNAVAILABLE"),
            ("source_admission_decision_id", request?.SourceAdmissionResult.DecisionId ?? "UNAVAILABLE"),
            ("source_delivery_decision_id", request?.SourceDeliveryDecision.DecisionId ?? "UNAVAILABLE"),
            ("publication_authority_reference", request?.AuthorityBinding.AuthorityReference ?? "UNAVAILABLE"),
            ("publication_authority_decision_id", request?.AuthorityBinding.AuthorityResult.DecisionId ?? "UNAVAILABLE"),
            ("publication_authority_binding_evidence", request?.AuthorityBinding.BindingEvidence ?? "UNAVAILABLE"),
            ("subscription_identity", subscription?.SubscriptionIdentity ?? "UNAVAILABLE"),
            ("observation_time", observedAt.ToString("O", CultureInfo.InvariantCulture)));
        return new EventPublicationDecision(decision, reason, decisionId, publishedEvent,
            subscription?.SubscriptionIdentity ?? "UNAVAILABLE", observedAt);
    }

    private static string TraceValue(string? value) => string.IsNullOrWhiteSpace(value) ? MissingTraceIdentity : value;
}

internal static class EventRules
{
    public static string RequireIdentifier(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value) || !StringComparer.Ordinal.Equals(value, value.Trim()))
            throw new ArgumentException("canonical_identifier_required", name);
        return value;
    }

    public static string RequireCanonicalText(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value) || !StringComparer.Ordinal.Equals(value, value.Trim()))
            throw new ArgumentException("canonical_text_required", name);
        return value;
    }

    public static string RequireVersion(string value, string name)
    {
        RequireIdentifier(value, name);
        if (!Version.TryParse(value, out _)) throw new ArgumentException("canonical_version_required", name);
        return value;
    }

    public static string RequireSha256(string value, string name)
    {
        if (value is null || value.Length != 64 || value.Any(character => !(character is >= '0' and <= '9' || character is >= 'A' and <= 'F')))
            throw new ArgumentException("sha256_required", name);
        return value;
    }

    public static T RequireDefined<T>(T value, string name) where T : struct, Enum
    {
        if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(name);
        return value;
    }

    public static void RequireUtc(DateTimeOffset value, string name)
    {
        if (value.Offset != TimeSpan.Zero) throw new ArgumentException("utc_required", name);
    }
}

internal static class EventCanonicalization
{
    public static string Hash(params (string Name, string Value)[] fields)
    {
        var builder = new StringBuilder();
        foreach (var field in fields)
        {
            Append(builder, field.Name);
            Append(builder, field.Value);
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    public static string JoinClassifications(IReadOnlyList<EventTruthClassification> classifications) =>
        string.Join(",", classifications.Select(value => ((int)value).ToString(CultureInfo.InvariantCulture)));

    private static void Append(StringBuilder builder, string value)
    {
        var normalized = value ?? string.Empty;
        builder.Append(normalized.Length.ToString(CultureInfo.InvariantCulture));
        builder.Append(':');
        builder.Append(normalized);
        builder.Append('|');
    }
}
