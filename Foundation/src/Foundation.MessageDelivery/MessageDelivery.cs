using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;
using Foundation.MessageAdmission;
using Foundation.MessageRouting;

namespace Foundation.MessageDelivery;

public enum DeliveryGuarantee
{
    BestEffort = 1,
    AtMostOnce = 2,
    AtLeastOnce = 3
}

public enum DeliveryOrderingGuarantee
{
    None = 1,
    PerKey = 2
}

public enum DeliveryTrafficClass
{
    Normal = 1,
    Protective = 2,
    Revocation = 3
}

public enum DeliveryDestinationHealth
{
    Healthy = 1,
    Degraded = 2,
    Unavailable = 3,
    Unknown = 4
}

public enum TransportObservationKind
{
    DispatchAccepted = 1,
    RecipientAcknowledged = 2,
    RetryableFailure = 3,
    TerminalFailure = 4
}

public enum DeliveryDecisionKind
{
    DispatchEligible = 1,
    RetryEligible = 2,
    Deferred = 3,
    DeadLetter = 4,
    Rejected = 5,
    Expired = 6,
    AlreadyAcknowledged = 7
}

public static class MessageDeliveryPurpose
{
    public const string GovernedDeliveryPolicy = "service-bus-delivery-policy";
    public const string GovernedPressureTruth = "service-bus-pressure-truth";
}

public static class MessageDeliveryReason
{
    public const string DispatchEligible = "DELIVERY_DISPATCH_ELIGIBLE";
    public const string RetryEligible = "DELIVERY_RETRY_ELIGIBLE";
    public const string InvalidContext = "INVALID_DELIVERY_CONTEXT";
    public const string RouteNotSelected = "ROUTE_NOT_SELECTED";
    public const string AdmissionNotAdmitted = "ADMISSION_NOT_ADMITTED";
    public const string PredecessorBindingMismatch = "DELIVERY_PREDECESSOR_BINDING_MISMATCH";
    public const string EnvelopeRequired = "DELIVERY_CANONICAL_ENVELOPE_REQUIRED";
    public const string EnvelopeBindingMismatch = "DELIVERY_CANONICAL_ENVELOPE_BINDING_MISMATCH";
    public const string PolicyRouteMismatch = "DELIVERY_POLICY_ROUTE_MISMATCH";
    public const string MessageExpired = "DELIVERY_MESSAGE_EXPIRED";
    public const string PreviousOutcomeMismatch = "DELIVERY_PREVIOUS_OUTCOME_MISMATCH";
    public const string PreviousOutcomeTerminal = "DELIVERY_PREVIOUS_OUTCOME_TERMINAL";
    public const string AlreadyAcknowledged = "DELIVERY_ALREADY_ACKNOWLEDGED";
    public const string RetryNotPermitted = "DELIVERY_RETRY_NOT_PERMITTED";
    public const string RetryLimitExhausted = "DELIVERY_RETRY_LIMIT_EXHAUSTED";
    public const string IdempotencyRequired = "DELIVERY_IDEMPOTENCY_REQUIRED";
    public const string IdempotencyBindingMismatch = "DELIVERY_IDEMPOTENCY_BINDING_MISMATCH";
    public const string DestinationUnavailable = "DELIVERY_DESTINATION_UNAVAILABLE";
    public const string DestinationUnknown = "DELIVERY_DESTINATION_UNKNOWN";
    public const string FlowControlDeferred = "DELIVERY_FLOW_CONTROL_DEFERRED";
    public const string PressureAuthorityRequired = "DELIVERY_PRESSURE_AUTHORITY_REQUIRED";
    public const string PressureAuthorityMalformed = "DELIVERY_PRESSURE_AUTHORITY_MALFORMED";
    public const string PressureAuthorityMismatch = "DELIVERY_PRESSURE_AUTHORITY_MISMATCH";
    public const string PressureAuthorityDenied = "DELIVERY_PRESSURE_AUTHORITY_DENIED";
    public const string PressureAuthorityNotYetEffective = "DELIVERY_PRESSURE_AUTHORITY_NOT_YET_EFFECTIVE";
    public const string PressureAuthorityExpired = "DELIVERY_PRESSURE_AUTHORITY_EXPIRED";
    public const string PressureObservationTimeInvalid = "DELIVERY_PRESSURE_OBSERVATION_TIME_INVALID";
    public const string PriorityAuthorityRequired = "DELIVERY_PRIORITY_AUTHORITY_REQUIRED";
    public const string PriorityAuthorityMalformed = "DELIVERY_PRIORITY_AUTHORITY_MALFORMED";
    public const string PriorityAuthorityMismatch = "DELIVERY_PRIORITY_AUTHORITY_MISMATCH";
    public const string PriorityAuthorityDenied = "DELIVERY_PRIORITY_AUTHORITY_DENIED";
    public const string PriorityAuthorityNotYetEffective = "DELIVERY_PRIORITY_AUTHORITY_NOT_YET_EFFECTIVE";
    public const string PriorityAuthorityExpired = "DELIVERY_PRIORITY_AUTHORITY_EXPIRED";
    public const string DeadLettered = "DELIVERY_DEAD_LETTERED";
}

public static class DeliveryOutcomeReason
{
    public const string Recorded = "DELIVERY_OUTCOME_RECORDED";
    public const string InvalidObservation = "INVALID_DELIVERY_OUTCOME_OBSERVATION";
    public const string DecisionNotDispatchable = "DELIVERY_DECISION_NOT_DISPATCHABLE";
    public const string ObservationBindingMismatch = "DELIVERY_OUTCOME_BINDING_MISMATCH";
    public const string ObservationTimeInvalid = "DELIVERY_OUTCOME_TIME_INVALID";
}

public sealed record DeliveryIdempotencyBinding
{
    public DeliveryIdempotencyBinding(
        string routeDecisionId,
        string admissionDecisionId,
        string idempotencyIdentity,
        string evidenceReference)
    {
        RouteDecisionId = DeliveryRules.RequireIdentifier(routeDecisionId, nameof(routeDecisionId));
        AdmissionDecisionId = DeliveryRules.RequireIdentifier(admissionDecisionId, nameof(admissionDecisionId));
        IdempotencyIdentity = DeliveryRules.RequireIdentifier(idempotencyIdentity, nameof(idempotencyIdentity));
        EvidenceReference = DeliveryRules.RequireCanonicalText(evidenceReference, nameof(evidenceReference));
    }

    public string RouteDecisionId { get; }
    public string AdmissionDecisionId { get; }
    public string IdempotencyIdentity { get; }
    public string EvidenceReference { get; }
}

public sealed record DeliveryPolicyAuthorityBinding
{
    public DeliveryPolicyAuthorityBinding(
        string authorityReference,
        AuthorityResult authorityResult,
        string authorizedPolicyId,
        string authorizedPolicyVersion,
        string authorizedRouteDecisionId,
        DeliveryTrafficClass authorizedTrafficClass,
        string effectiveScope,
        string bindingEvidence)
    {
        AuthorityReference = DeliveryRules.RequireIdentifier(authorityReference, nameof(authorityReference));
        AuthorityResult = authorityResult ?? throw new ArgumentNullException(nameof(authorityResult));
        AuthorizedPolicyId = DeliveryRules.RequireIdentifier(authorizedPolicyId, nameof(authorizedPolicyId));
        AuthorizedPolicyVersion = DeliveryRules.RequireVersion(authorizedPolicyVersion, nameof(authorizedPolicyVersion));
        AuthorizedRouteDecisionId = DeliveryRules.RequireIdentifier(authorizedRouteDecisionId, nameof(authorizedRouteDecisionId));
        AuthorizedTrafficClass = DeliveryRules.RequireDefined(authorizedTrafficClass, nameof(authorizedTrafficClass));
        EffectiveScope = DeliveryRules.RequireIdentifier(effectiveScope, nameof(effectiveScope));
        BindingEvidence = DeliveryRules.RequireCanonicalText(bindingEvidence, nameof(bindingEvidence));
    }

    public string AuthorityReference { get; }
    public AuthorityResult AuthorityResult { get; }
    public string AuthorizedPolicyId { get; }
    public string AuthorizedPolicyVersion { get; }
    public string AuthorizedRouteDecisionId { get; }
    public DeliveryTrafficClass AuthorizedTrafficClass { get; }
    public string EffectiveScope { get; }
    public string BindingEvidence { get; }
}

public sealed record DeliveryPolicy
{
    public DeliveryPolicy(
        string policyId,
        string policyVersion,
        string routeDecisionId,
        DeliveryGuarantee guarantee,
        int maxAttempts,
        DeliveryOrderingGuarantee orderingGuarantee,
        string? orderingKey,
        bool retryRequiresIdempotency,
        bool deadLetterOnTerminalFailure,
        DeliveryTrafficClass trafficClass,
        DeliveryPolicyAuthorityBinding? authorityBinding,
        string evidenceReference)
    {
        PolicyId = DeliveryRules.RequireIdentifier(policyId, nameof(policyId));
        PolicyVersion = DeliveryRules.RequireVersion(policyVersion, nameof(policyVersion));
        RouteDecisionId = DeliveryRules.RequireIdentifier(routeDecisionId, nameof(routeDecisionId));
        Guarantee = DeliveryRules.RequireDefined(guarantee, nameof(guarantee));
        if (maxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(maxAttempts), "max_attempts_must_be_positive");
        if ((guarantee == DeliveryGuarantee.BestEffort || guarantee == DeliveryGuarantee.AtMostOnce) && maxAttempts != 1)
            throw new ArgumentException("non_retrying_guarantee_requires_single_attempt", nameof(maxAttempts));
        MaxAttempts = maxAttempts;
        OrderingGuarantee = DeliveryRules.RequireDefined(orderingGuarantee, nameof(orderingGuarantee));
        if (orderingGuarantee == DeliveryOrderingGuarantee.None && orderingKey is not null)
            throw new ArgumentException("ordering_key_not_allowed_without_ordering", nameof(orderingKey));
        if (orderingGuarantee == DeliveryOrderingGuarantee.PerKey)
            OrderingKey = DeliveryRules.RequireIdentifier(orderingKey ?? string.Empty, nameof(orderingKey));
        RetryRequiresIdempotency = retryRequiresIdempotency;
        DeadLetterOnTerminalFailure = deadLetterOnTerminalFailure;
        TrafficClass = DeliveryRules.RequireDefined(trafficClass, nameof(trafficClass));
        if (trafficClass == DeliveryTrafficClass.Normal && authorityBinding is not null)
            throw new ArgumentException("normal_traffic_must_not_carry_elevated_authority_binding", nameof(authorityBinding));
        if (trafficClass != DeliveryTrafficClass.Normal && authorityBinding is null)
            throw new ArgumentNullException(nameof(authorityBinding), "elevated_traffic_class_requires_authority_binding");
        AuthorityBinding = authorityBinding;
        EvidenceReference = DeliveryRules.RequireCanonicalText(evidenceReference, nameof(evidenceReference));
    }

    public string PolicyId { get; }
    public string PolicyVersion { get; }
    public string RouteDecisionId { get; }
    public DeliveryGuarantee Guarantee { get; }
    public int MaxAttempts { get; }
    public DeliveryOrderingGuarantee OrderingGuarantee { get; }
    public string? OrderingKey { get; }
    public bool RetryRequiresIdempotency { get; }
    public bool DeadLetterOnTerminalFailure { get; }
    public DeliveryTrafficClass TrafficClass { get; }
    public DeliveryPolicyAuthorityBinding? AuthorityBinding { get; }
    public string EvidenceReference { get; }
}

public sealed record DeliveryPressureAuthorityBinding
{
    public DeliveryPressureAuthorityBinding(
        string authorityReference,
        AuthorityResult authorityResult,
        string authorizedProducerApplicationId,
        string authorizedRouteDecisionId,
        int authorizedGlobalLimit,
        int authorizedRouteLimit,
        int authorizedProducerLimit,
        int authorizedReservedElevatedSlots,
        string effectiveScope,
        string restorationConditions,
        string bindingEvidence)
    {
        AuthorityReference = DeliveryRules.RequireIdentifier(authorityReference, nameof(authorityReference));
        AuthorityResult = authorityResult ?? throw new ArgumentNullException(nameof(authorityResult));
        AuthorizedProducerApplicationId = DeliveryRules.RequireIdentifier(authorizedProducerApplicationId, nameof(authorizedProducerApplicationId));
        AuthorizedRouteDecisionId = DeliveryRules.RequireIdentifier(authorizedRouteDecisionId, nameof(authorizedRouteDecisionId));
        if (authorizedGlobalLimit < 1) throw new ArgumentOutOfRangeException(nameof(authorizedGlobalLimit));
        if (authorizedRouteLimit < 1) throw new ArgumentOutOfRangeException(nameof(authorizedRouteLimit));
        if (authorizedProducerLimit < 1) throw new ArgumentOutOfRangeException(nameof(authorizedProducerLimit));
        if (authorizedReservedElevatedSlots < 0 || authorizedReservedElevatedSlots > authorizedGlobalLimit)
            throw new ArgumentOutOfRangeException(nameof(authorizedReservedElevatedSlots));
        AuthorizedGlobalLimit = authorizedGlobalLimit;
        AuthorizedRouteLimit = authorizedRouteLimit;
        AuthorizedProducerLimit = authorizedProducerLimit;
        AuthorizedReservedElevatedSlots = authorizedReservedElevatedSlots;
        EffectiveScope = DeliveryRules.RequireIdentifier(effectiveScope, nameof(effectiveScope));
        RestorationConditions = DeliveryRules.RequireCanonicalText(restorationConditions, nameof(restorationConditions));
        BindingEvidence = DeliveryRules.RequireCanonicalText(bindingEvidence, nameof(bindingEvidence));
    }

    public string AuthorityReference { get; }
    public AuthorityResult AuthorityResult { get; }
    public string AuthorizedProducerApplicationId { get; }
    public string AuthorizedRouteDecisionId { get; }
    public int AuthorizedGlobalLimit { get; }
    public int AuthorizedRouteLimit { get; }
    public int AuthorizedProducerLimit { get; }
    public int AuthorizedReservedElevatedSlots { get; }
    public string EffectiveScope { get; }
    public string RestorationConditions { get; }
    public string BindingEvidence { get; }
}

public sealed record DeliveryPressureSnapshot
{
    public DeliveryPressureSnapshot(
        string routeDecisionId,
        string producerApplicationId,
        int globalLimit,
        int globalInFlight,
        int routeLimit,
        int routeInFlight,
        int producerLimit,
        int producerInFlight,
        int reservedElevatedSlots,
        DateTimeOffset observedAt,
        DeliveryPressureAuthorityBinding authorityBinding,
        string evidenceReference)
    {
        RouteDecisionId = DeliveryRules.RequireIdentifier(routeDecisionId, nameof(routeDecisionId));
        ProducerApplicationId = DeliveryRules.RequireIdentifier(producerApplicationId, nameof(producerApplicationId));
        DeliveryRules.RequireCapacity(globalLimit, globalInFlight, nameof(globalLimit), nameof(globalInFlight));
        DeliveryRules.RequireCapacity(routeLimit, routeInFlight, nameof(routeLimit), nameof(routeInFlight));
        DeliveryRules.RequireCapacity(producerLimit, producerInFlight, nameof(producerLimit), nameof(producerInFlight));
        if (reservedElevatedSlots < 0 || reservedElevatedSlots > globalLimit)
            throw new ArgumentOutOfRangeException(nameof(reservedElevatedSlots), "reserved_slots_out_of_range");
        DeliveryRules.RequireUtc(observedAt, nameof(observedAt));
        GlobalLimit = globalLimit;
        GlobalInFlight = globalInFlight;
        RouteLimit = routeLimit;
        RouteInFlight = routeInFlight;
        ProducerLimit = producerLimit;
        ProducerInFlight = producerInFlight;
        ReservedElevatedSlots = reservedElevatedSlots;
        ObservedAt = observedAt;
        AuthorityBinding = authorityBinding ?? throw new ArgumentNullException(nameof(authorityBinding));
        EvidenceReference = DeliveryRules.RequireCanonicalText(evidenceReference, nameof(evidenceReference));
    }

    public string RouteDecisionId { get; }
    public string ProducerApplicationId { get; }
    public int GlobalLimit { get; }
    public int GlobalInFlight { get; }
    public int RouteLimit { get; }
    public int RouteInFlight { get; }
    public int ProducerLimit { get; }
    public int ProducerInFlight { get; }
    public int ReservedElevatedSlots { get; }
    public DateTimeOffset ObservedAt { get; }
    public DeliveryPressureAuthorityBinding AuthorityBinding { get; }
    public string EvidenceReference { get; }
}

public sealed record DeliveryAttemptOutcome
{
    internal DeliveryAttemptOutcome(
        string outcomeId,
        string deliveryDecisionId,
        string routeDecisionId,
        string correlationId,
        string causationId,
        string policyId,
        string policyVersion,
        int attemptNumber,
        TransportObservationKind observation,
        DateTimeOffset observationTime,
        string evidenceReference)
    {
        OutcomeId = outcomeId;
        DeliveryDecisionId = deliveryDecisionId;
        RouteDecisionId = routeDecisionId;
        CorrelationId = correlationId;
        CausationId = causationId;
        PolicyId = policyId;
        PolicyVersion = policyVersion;
        AttemptNumber = attemptNumber;
        Observation = observation;
        ObservationTime = observationTime;
        EvidenceReference = evidenceReference;
    }

    public string OutcomeId { get; }
    public string DeliveryDecisionId { get; }
    public string RouteDecisionId { get; }
    public string CorrelationId { get; }
    public string CausationId { get; }
    public string PolicyId { get; }
    public string PolicyVersion { get; }
    public int AttemptNumber { get; }
    public TransportObservationKind Observation { get; }
    public DateTimeOffset ObservationTime { get; }
    public string EvidenceReference { get; }

    public bool IsAcknowledged => Observation == TransportObservationKind.RecipientAcknowledged;
    public bool IsRetryable => Observation == TransportObservationKind.RetryableFailure;
    public bool IsTerminalFailure => Observation == TransportObservationKind.TerminalFailure;
}

public sealed record TransportOutcomeObservation
{
    public TransportOutcomeObservation(
        string routeDecisionId,
        string policyId,
        string policyVersion,
        int attemptNumber,
        TransportObservationKind observation,
        DateTimeOffset observationTime,
        string evidenceReference)
    {
        RouteDecisionId = DeliveryRules.RequireIdentifier(routeDecisionId, nameof(routeDecisionId));
        PolicyId = DeliveryRules.RequireIdentifier(policyId, nameof(policyId));
        PolicyVersion = DeliveryRules.RequireVersion(policyVersion, nameof(policyVersion));
        if (attemptNumber < 1) throw new ArgumentOutOfRangeException(nameof(attemptNumber));
        AttemptNumber = attemptNumber;
        Observation = DeliveryRules.RequireDefined(observation, nameof(observation));
        DeliveryRules.RequireUtc(observationTime, nameof(observationTime));
        ObservationTime = observationTime;
        EvidenceReference = DeliveryRules.RequireCanonicalText(evidenceReference, nameof(evidenceReference));
    }

    public string RouteDecisionId { get; }
    public string PolicyId { get; }
    public string PolicyVersion { get; }
    public int AttemptNumber { get; }
    public TransportObservationKind Observation { get; }
    public DateTimeOffset ObservationTime { get; }
    public string EvidenceReference { get; }
}

public sealed record DeliveryEvaluationContext
{
    public DeliveryEvaluationContext(
        RouteDecision? routeDecision,
        MessageAdmissionResult? admissionResult,
        CanonicalFilEnvelope? canonicalEnvelope,
        DeliveryPolicy? policy,
        int attemptNumber,
        DeliveryAttemptOutcome? previousOutcome,
        DeliveryIdempotencyBinding? idempotencyBinding,
        DeliveryDestinationHealth destinationHealth,
        DeliveryPressureSnapshot? pressureSnapshot,
        DateTimeOffset observationTime,
        string decisionEvidence)
    {
        if (attemptNumber < 1) throw new ArgumentOutOfRangeException(nameof(attemptNumber));
        RouteDecision = routeDecision;
        AdmissionResult = admissionResult;
        CanonicalEnvelope = canonicalEnvelope;
        Policy = policy;
        AttemptNumber = attemptNumber;
        PreviousOutcome = previousOutcome;
        IdempotencyBinding = idempotencyBinding;
        DestinationHealth = DeliveryRules.RequireDefined(destinationHealth, nameof(destinationHealth));
        PressureSnapshot = pressureSnapshot;
        DeliveryRules.RequireUtc(observationTime, nameof(observationTime));
        ObservationTime = observationTime;
        DecisionEvidence = DeliveryRules.RequireCanonicalText(decisionEvidence, nameof(decisionEvidence));
    }

    public RouteDecision? RouteDecision { get; }
    public MessageAdmissionResult? AdmissionResult { get; }
    public CanonicalFilEnvelope? CanonicalEnvelope { get; }
    public DeliveryPolicy? Policy { get; }
    public int AttemptNumber { get; }
    public DeliveryAttemptOutcome? PreviousOutcome { get; }
    public DeliveryIdempotencyBinding? IdempotencyBinding { get; }
    public DeliveryDestinationHealth DestinationHealth { get; }
    public DeliveryPressureSnapshot? PressureSnapshot { get; }
    public DateTimeOffset ObservationTime { get; }
    public string DecisionEvidence { get; }
}

public sealed record DeliveryDecision
{
    internal DeliveryDecision(
        DeliveryDecisionKind decision,
        string reason,
        string decisionId,
        string routeDecisionId,
        string routeRegistrySnapshotDigest,
        string admissionDecisionId,
        string messageId,
        string correlationId,
        string causationId,
        string producerApplicationId,
        string routeId,
        string routeVersion,
        string policyId,
        string policyVersion,
        DeliveryGuarantee guarantee,
        DeliveryOrderingGuarantee orderingGuarantee,
        string orderingKey,
        DeliveryTrafficClass trafficClass,
        int attemptNumber,
        string previousOutcomeId,
        string idempotencyIdentity,
        DeliveryDestinationHealth destinationHealth,
        string pressureSnapshotId,
        DateTimeOffset observationTime,
        string evidenceReference)
    {
        Decision = decision;
        Reason = reason;
        DecisionId = decisionId;
        RouteDecisionId = routeDecisionId;
        RouteRegistrySnapshotDigest = routeRegistrySnapshotDigest;
        AdmissionDecisionId = admissionDecisionId;
        MessageId = messageId;
        CorrelationId = correlationId;
        CausationId = causationId;
        ProducerApplicationId = producerApplicationId;
        RouteId = routeId;
        RouteVersion = routeVersion;
        PolicyId = policyId;
        PolicyVersion = policyVersion;
        Guarantee = guarantee;
        OrderingGuarantee = orderingGuarantee;
        OrderingKey = orderingKey;
        TrafficClass = trafficClass;
        AttemptNumber = attemptNumber;
        PreviousOutcomeId = previousOutcomeId;
        IdempotencyIdentity = idempotencyIdentity;
        DestinationHealth = destinationHealth;
        PressureSnapshotId = pressureSnapshotId;
        ObservationTime = observationTime;
        EvidenceReference = evidenceReference;
    }

    public DeliveryDecisionKind Decision { get; }
    public string Reason { get; }
    public string DecisionId { get; }
    public string RouteDecisionId { get; }
    public string RouteRegistrySnapshotDigest { get; }
    public string AdmissionDecisionId { get; }
    public string MessageId { get; }
    public string CorrelationId { get; }
    public string CausationId { get; }
    public string ProducerApplicationId { get; }
    public string RouteId { get; }
    public string RouteVersion { get; }
    public string PolicyId { get; }
    public string PolicyVersion { get; }
    public DeliveryGuarantee Guarantee { get; }
    public DeliveryOrderingGuarantee OrderingGuarantee { get; }
    public string OrderingKey { get; }
    public DeliveryTrafficClass TrafficClass { get; }
    public int AttemptNumber { get; }
    public string PreviousOutcomeId { get; }
    public string IdempotencyIdentity { get; }
    public DeliveryDestinationHealth DestinationHealth { get; }
    public string PressureSnapshotId { get; }
    public DateTimeOffset ObservationTime { get; }
    public string EvidenceReference { get; }

    public bool CanDispatch => Decision == DeliveryDecisionKind.DispatchEligible || Decision == DeliveryDecisionKind.RetryEligible;
    public bool IsTerminal => Decision == DeliveryDecisionKind.DeadLetter || Decision == DeliveryDecisionKind.Rejected || Decision == DeliveryDecisionKind.Expired || Decision == DeliveryDecisionKind.AlreadyAcknowledged;
}

public sealed record DeliveryOutcomeRecordResult(bool Accepted, string Reason, DeliveryAttemptOutcome? Outcome);

public sealed class DeliveryOutcomeRecorder
{
    public DeliveryOutcomeRecordResult Record(DeliveryDecision? decision, TransportOutcomeObservation? observation)
    {
        if (decision is null || observation is null)
            return new(false, DeliveryOutcomeReason.InvalidObservation, null);
        if (!decision.CanDispatch)
            return new(false, DeliveryOutcomeReason.DecisionNotDispatchable, null);
        if (!StringComparer.Ordinal.Equals(decision.RouteDecisionId, observation.RouteDecisionId) ||
            !StringComparer.Ordinal.Equals(decision.PolicyId, observation.PolicyId) ||
            !StringComparer.Ordinal.Equals(decision.PolicyVersion, observation.PolicyVersion) ||
            decision.AttemptNumber != observation.AttemptNumber)
            return new(false, DeliveryOutcomeReason.ObservationBindingMismatch, null);
        if (observation.ObservationTime < decision.ObservationTime)
            return new(false, DeliveryOutcomeReason.ObservationTimeInvalid, null);

        var outcomeId = DeliveryCanonicalization.Hash(
            ("delivery_decision_id", decision.DecisionId),
            ("route_decision_id", observation.RouteDecisionId),
            ("correlation_id", decision.CorrelationId),
            ("causation_id", decision.CausationId),
            ("policy_id", observation.PolicyId),
            ("policy_version", observation.PolicyVersion),
            ("attempt_number", observation.AttemptNumber.ToString(CultureInfo.InvariantCulture)),
            ("observation", ((int)observation.Observation).ToString(CultureInfo.InvariantCulture)),
            ("observation_time", observation.ObservationTime.ToString("O", CultureInfo.InvariantCulture)),
            ("evidence", observation.EvidenceReference));

        return new(true, DeliveryOutcomeReason.Recorded, new DeliveryAttemptOutcome(
            outcomeId,
            decision.DecisionId,
            observation.RouteDecisionId,
            decision.CorrelationId,
            decision.CausationId,
            observation.PolicyId,
            observation.PolicyVersion,
            observation.AttemptNumber,
            observation.Observation,
            observation.ObservationTime,
            observation.EvidenceReference));
    }
}

public sealed class FilMessageDeliveryEvaluator
{
    private const string AllowDecision = "ALLOW";
    private const string MissingTraceIdentity = "NONE";

    public DeliveryDecision Evaluate(DeliveryEvaluationContext? context)
    {
        if (context is null) return CreateDecision(null, DeliveryDecisionKind.Rejected, MessageDeliveryReason.InvalidContext);

        var route = context.RouteDecision;
        var admission = context.AdmissionResult;
        var envelope = context.CanonicalEnvelope;
        var policy = context.Policy;

        if (route is null || admission is null || policy is null)
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.InvalidContext);
        if (envelope is null)
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.EnvelopeRequired);
        if (route.Decision != RouteSelectionDecision.Selected)
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.RouteNotSelected);
        if (!admission.IsAdmitted)
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.AdmissionNotAdmitted);
        if (!PredecessorsMatch(route, admission))
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.PredecessorBindingMismatch);
        if (!EnvelopeMatchesAdmission(envelope, admission))
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.EnvelopeBindingMismatch);
        if (!StringComparer.Ordinal.Equals(policy.RouteDecisionId, route.DecisionId))
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.PolicyRouteMismatch);
        if (admission.EffectiveExpiry is { } expiry && context.ObservationTime >= expiry)
            return CreateDecision(context, DeliveryDecisionKind.Expired, MessageDeliveryReason.MessageExpired);

        var authorityFailure = ValidatePriorityAuthority(policy, context.ObservationTime);
        if (authorityFailure is not null)
            return CreateDecision(context, DeliveryDecisionKind.Rejected, authorityFailure);

        if (!ValidatePressureBinding(context))
            return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.PredecessorBindingMismatch);

        var pressureAuthorityFailure = ValidatePressureAuthority(context);
        if (pressureAuthorityFailure is not null)
            return CreateDecision(context, DeliveryDecisionKind.Rejected, pressureAuthorityFailure);

        var correlationId = TraceValue(envelope.CorrelationId?.Value);
        var causationId = TraceValue(envelope.CausationId?.Value);
        var previous = context.PreviousOutcome;
        if (context.AttemptNumber == 1)
        {
            if (previous is not null)
                return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.PreviousOutcomeMismatch);
        }
        else
        {
            if (previous is null ||
                previous.AttemptNumber != context.AttemptNumber - 1 ||
                !StringComparer.Ordinal.Equals(previous.RouteDecisionId, route.DecisionId) ||
                !StringComparer.Ordinal.Equals(previous.CorrelationId, correlationId) ||
                !StringComparer.Ordinal.Equals(previous.CausationId, causationId) ||
                !StringComparer.Ordinal.Equals(previous.PolicyId, policy.PolicyId) ||
                !StringComparer.Ordinal.Equals(previous.PolicyVersion, policy.PolicyVersion))
                return CreateDecision(context, DeliveryDecisionKind.Rejected, MessageDeliveryReason.PreviousOutcomeMismatch);

            if (previous.IsAcknowledged)
                return CreateDecision(context, DeliveryDecisionKind.AlreadyAcknowledged, MessageDeliveryReason.AlreadyAcknowledged);
            if (previous.IsTerminalFailure)
                return Terminal(context, MessageDeliveryReason.PreviousOutcomeTerminal);
            if (!previous.IsRetryable)
                return Terminal(context, MessageDeliveryReason.RetryNotPermitted);
            if (policy.Guarantee != DeliveryGuarantee.AtLeastOnce)
                return Terminal(context, MessageDeliveryReason.RetryNotPermitted);
            if (context.AttemptNumber > policy.MaxAttempts)
                return Terminal(context, MessageDeliveryReason.RetryLimitExhausted);
            if (policy.RetryRequiresIdempotency && !IdempotencyMatches(context))
                return Terminal(context, context.IdempotencyBinding is null
                    ? MessageDeliveryReason.IdempotencyRequired
                    : MessageDeliveryReason.IdempotencyBindingMismatch);
        }

        if (context.DestinationHealth == DeliveryDestinationHealth.Unknown)
            return CreateDecision(context, DeliveryDecisionKind.Deferred, MessageDeliveryReason.DestinationUnknown);
        if (context.DestinationHealth == DeliveryDestinationHealth.Unavailable)
        {
            if (context.AttemptNumber >= policy.MaxAttempts)
                return Terminal(context, MessageDeliveryReason.DestinationUnavailable);
            return CreateDecision(context, DeliveryDecisionKind.Deferred, MessageDeliveryReason.DestinationUnavailable);
        }

        if (IsPressureBlocked(context))
            return CreateDecision(context, DeliveryDecisionKind.Deferred, MessageDeliveryReason.FlowControlDeferred);

        return CreateDecision(
            context,
            context.AttemptNumber == 1 ? DeliveryDecisionKind.DispatchEligible : DeliveryDecisionKind.RetryEligible,
            context.AttemptNumber == 1 ? MessageDeliveryReason.DispatchEligible : MessageDeliveryReason.RetryEligible);
    }

    private static string TraceValue(string? value) =>
        string.IsNullOrWhiteSpace(value) ? MissingTraceIdentity : value;

    private static DeliveryDecision Terminal(DeliveryEvaluationContext context, string reason) =>
        CreateDecision(
            context,
            context.Policy?.DeadLetterOnTerminalFailure == true ? DeliveryDecisionKind.DeadLetter : DeliveryDecisionKind.Rejected,
            context.Policy?.DeadLetterOnTerminalFailure == true ? MessageDeliveryReason.DeadLettered + ":" + reason : reason);

    private static bool PredecessorsMatch(RouteDecision route, MessageAdmissionResult admission) =>
        StringComparer.Ordinal.Equals(route.AdmissionDecisionId, admission.DecisionId) &&
        StringComparer.Ordinal.Equals(route.MessageDigest, admission.MessageDigest) &&
        StringComparer.Ordinal.Equals(route.MessageId, admission.MessageId) &&
        StringComparer.Ordinal.Equals(route.ProducerIdentity, admission.ProducerIdentity) &&
        StringComparer.Ordinal.Equals(route.ProducerApplicationId, admission.ProducerApplicationId) &&
        StringComparer.Ordinal.Equals(route.ManifestId, admission.ManifestId) &&
        StringComparer.Ordinal.Equals(route.ManifestVersion, admission.ManifestVersion) &&
        StringComparer.Ordinal.Equals(route.RecipientScope, admission.RecipientScope) &&
        StringComparer.Ordinal.Equals(route.IntendedConsumer, admission.IntendedConsumer);

    private static bool EnvelopeMatchesAdmission(CanonicalFilEnvelope envelope, MessageAdmissionResult admission) =>
        StringComparer.Ordinal.Equals(CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope), admission.MessageDigest) &&
        StringComparer.Ordinal.Equals(envelope.MessageId.Value, admission.MessageId) &&
        StringComparer.Ordinal.Equals(envelope.Producer.Value, admission.ProducerIdentity) &&
        StringComparer.Ordinal.Equals(envelope.RecipientScope.Value, admission.RecipientScope) &&
        StringComparer.Ordinal.Equals(envelope.SchemaId.Value, admission.SchemaId) &&
        StringComparer.Ordinal.Equals(envelope.SchemaVersion, admission.SchemaVersion);

    private static bool ValidatePressureBinding(DeliveryEvaluationContext context)
    {
        var pressure = context.PressureSnapshot;
        var route = context.RouteDecision;
        return pressure is not null && route is not null &&
               StringComparer.Ordinal.Equals(pressure.RouteDecisionId, route.DecisionId) &&
               StringComparer.Ordinal.Equals(pressure.ProducerApplicationId, route.ProducerApplicationId);
    }

    private static string? ValidatePressureAuthority(DeliveryEvaluationContext context)
    {
        var pressure = context.PressureSnapshot;
        if (pressure is null) return MessageDeliveryReason.PressureAuthorityRequired;
        var binding = pressure.AuthorityBinding;
        var result = binding.AuthorityResult;
        if (ContractValidators.Validate(result).Result != ValidationResult.Pass)
            return MessageDeliveryReason.PressureAuthorityMalformed;
        if (!StringComparer.Ordinal.Equals(binding.AuthorizedProducerApplicationId, pressure.ProducerApplicationId) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedRouteDecisionId, pressure.RouteDecisionId) ||
            binding.AuthorizedGlobalLimit != pressure.GlobalLimit ||
            binding.AuthorizedRouteLimit != pressure.RouteLimit ||
            binding.AuthorizedProducerLimit != pressure.ProducerLimit ||
            binding.AuthorizedReservedElevatedSlots != pressure.ReservedElevatedSlots ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, result.EffectiveScope) ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, MessageDeliveryPurpose.GovernedPressureTruth))
            return MessageDeliveryReason.PressureAuthorityMismatch;
        if (!StringComparer.Ordinal.Equals(result.Decision, AllowDecision))
            return MessageDeliveryReason.PressureAuthorityDenied;
        if (context.ObservationTime < result.DecisionTime || pressure.ObservedAt < result.DecisionTime)
            return MessageDeliveryReason.PressureAuthorityNotYetEffective;
        if (context.ObservationTime >= result.Expiry || pressure.ObservedAt >= result.Expiry)
            return MessageDeliveryReason.PressureAuthorityExpired;
        if (pressure.ObservedAt > context.ObservationTime)
            return MessageDeliveryReason.PressureObservationTimeInvalid;
        return null;
    }

    private static bool IdempotencyMatches(DeliveryEvaluationContext context)
    {
        var binding = context.IdempotencyBinding;
        var route = context.RouteDecision;
        var admission = context.AdmissionResult;
        var envelope = context.CanonicalEnvelope;
        return binding is not null && route is not null && admission is not null && envelope is not null &&
               StringComparer.Ordinal.Equals(binding.RouteDecisionId, route.DecisionId) &&
               StringComparer.Ordinal.Equals(binding.AdmissionDecisionId, admission.DecisionId) &&
               StringComparer.Ordinal.Equals(binding.IdempotencyIdentity, envelope.IdempotencyId.Value);
    }

    private static string? ValidatePriorityAuthority(DeliveryPolicy policy, DateTimeOffset observationTime)
    {
        if (policy.TrafficClass == DeliveryTrafficClass.Normal) return null;
        var binding = policy.AuthorityBinding;
        if (binding is null) return MessageDeliveryReason.PriorityAuthorityRequired;
        if (ContractValidators.Validate(binding.AuthorityResult).Result != ValidationResult.Pass)
            return MessageDeliveryReason.PriorityAuthorityMalformed;
        if (!StringComparer.Ordinal.Equals(binding.AuthorizedPolicyId, policy.PolicyId) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedPolicyVersion, policy.PolicyVersion) ||
            !StringComparer.Ordinal.Equals(binding.AuthorizedRouteDecisionId, policy.RouteDecisionId) ||
            binding.AuthorizedTrafficClass != policy.TrafficClass ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, binding.AuthorityResult.EffectiveScope) ||
            !StringComparer.Ordinal.Equals(binding.EffectiveScope, MessageDeliveryPurpose.GovernedDeliveryPolicy))
            return MessageDeliveryReason.PriorityAuthorityMismatch;
        if (!StringComparer.Ordinal.Equals(binding.AuthorityResult.Decision, AllowDecision))
            return MessageDeliveryReason.PriorityAuthorityDenied;
        if (observationTime < binding.AuthorityResult.DecisionTime)
            return MessageDeliveryReason.PriorityAuthorityNotYetEffective;
        if (observationTime >= binding.AuthorityResult.Expiry)
            return MessageDeliveryReason.PriorityAuthorityExpired;
        return null;
    }

    private static bool IsPressureBlocked(DeliveryEvaluationContext context)
    {
        var pressure = context.PressureSnapshot!;
        var policy = context.Policy!;

        if (pressure.RouteInFlight >= pressure.RouteLimit) return true;
        if (pressure.ProducerInFlight >= pressure.ProducerLimit) return true;

        if (policy.TrafficClass == DeliveryTrafficClass.Normal)
        {
            var normalCeiling = pressure.GlobalLimit - pressure.ReservedElevatedSlots;
            return pressure.GlobalInFlight >= normalCeiling;
        }

        return pressure.GlobalInFlight >= pressure.GlobalLimit;
    }

    private static DeliveryDecision CreateDecision(
        DeliveryEvaluationContext? context,
        DeliveryDecisionKind decision,
        string reason)
    {
        var route = context?.RouteDecision;
        var admission = context?.AdmissionResult;
        var envelope = context?.CanonicalEnvelope;
        var policy = context?.Policy;
        var previous = context?.PreviousOutcome;
        var idempotency = context?.IdempotencyBinding;
        var pressure = context?.PressureSnapshot;
        var observationTime = context?.ObservationTime ?? DateTimeOffset.UnixEpoch;
        var evidence = context?.DecisionEvidence ?? "UNAVAILABLE";
        var correlationId = TraceValue(envelope?.CorrelationId?.Value);
        var causationId = TraceValue(envelope?.CausationId?.Value);

        var pressureAuthority = pressure?.AuthorityBinding;
        var pressureAuthorityResult = pressureAuthority?.AuthorityResult;
        var pressureId = pressure is null ? "UNAVAILABLE" : DeliveryCanonicalization.Hash(
            ("route_decision_id", pressure.RouteDecisionId),
            ("producer_application_id", pressure.ProducerApplicationId),
            ("global_limit", pressure.GlobalLimit.ToString(CultureInfo.InvariantCulture)),
            ("global_in_flight", pressure.GlobalInFlight.ToString(CultureInfo.InvariantCulture)),
            ("route_limit", pressure.RouteLimit.ToString(CultureInfo.InvariantCulture)),
            ("route_in_flight", pressure.RouteInFlight.ToString(CultureInfo.InvariantCulture)),
            ("producer_limit", pressure.ProducerLimit.ToString(CultureInfo.InvariantCulture)),
            ("producer_in_flight", pressure.ProducerInFlight.ToString(CultureInfo.InvariantCulture)),
            ("reserved_elevated_slots", pressure.ReservedElevatedSlots.ToString(CultureInfo.InvariantCulture)),
            ("observed_at", pressure.ObservedAt.ToString("O", CultureInfo.InvariantCulture)),
            ("pressure_authority_reference", pressureAuthority?.AuthorityReference ?? "NONE"),
            ("pressure_authority_decision_id", pressureAuthorityResult?.DecisionId ?? "NONE"),
            ("pressure_authority_decision", pressureAuthorityResult?.Decision ?? "NONE"),
            ("pressure_authority_scope", pressureAuthorityResult?.EffectiveScope ?? "NONE"),
            ("pressure_authority_policy", pressureAuthorityResult?.ControllingPolicy ?? "NONE"),
            ("pressure_authority_policy_version", pressureAuthorityResult?.PolicyVersion ?? "NONE"),
            ("pressure_authority_conditions", pressureAuthorityResult?.MaterialConditions ?? "NONE"),
            ("pressure_authority_constraints", pressureAuthorityResult?.Constraints ?? "NONE"),
            ("pressure_authority_reason", pressureAuthorityResult?.Reason ?? "NONE"),
            ("pressure_authority_decision_time", pressureAuthorityResult?.DecisionTime.ToString("O", CultureInfo.InvariantCulture) ?? "NONE"),
            ("pressure_authority_expiry", pressureAuthorityResult?.Expiry.ToString("O", CultureInfo.InvariantCulture) ?? "NONE"),
            ("pressure_authority_evidence", pressureAuthorityResult?.EvidenceReference ?? "NONE"),
            ("pressure_restoration_conditions", pressureAuthority?.RestorationConditions ?? "NONE"),
            ("pressure_binding_evidence", pressureAuthority?.BindingEvidence ?? "NONE"),
            ("evidence", pressure.EvidenceReference));

        var authority = policy?.AuthorityBinding;
        var authorityResult = authority?.AuthorityResult;

        var decisionId = DeliveryCanonicalization.Hash(
            ("decision", ((int)decision).ToString(CultureInfo.InvariantCulture)),
            ("reason", reason),
            ("route_decision_id", route?.DecisionId ?? "UNAVAILABLE"),
            ("route_registry_snapshot", route?.RegistrySnapshotDigest ?? "UNAVAILABLE"),
            ("admission_decision_id", admission?.DecisionId ?? "UNAVAILABLE"),
            ("message_id", admission?.MessageId ?? "UNAVAILABLE"),
            ("message_digest", admission?.MessageDigest ?? "UNAVAILABLE"),
            ("correlation_id", correlationId),
            ("causation_id", causationId),
            ("producer_application_id", admission?.ProducerApplicationId ?? "UNAVAILABLE"),
            ("route_id", route?.RouteId ?? "UNAVAILABLE"),
            ("route_version", route?.RouteVersion ?? "UNAVAILABLE"),
            ("policy_id", policy?.PolicyId ?? "UNAVAILABLE"),
            ("policy_version", policy?.PolicyVersion ?? "UNAVAILABLE"),
            ("policy_evidence", policy?.EvidenceReference ?? "UNAVAILABLE"),
            ("guarantee", ((int)(policy?.Guarantee ?? (DeliveryGuarantee)0)).ToString(CultureInfo.InvariantCulture)),
            ("max_attempts", (policy?.MaxAttempts ?? 0).ToString(CultureInfo.InvariantCulture)),
            ("ordering", ((int)(policy?.OrderingGuarantee ?? (DeliveryOrderingGuarantee)0)).ToString(CultureInfo.InvariantCulture)),
            ("ordering_key", policy?.OrderingKey ?? "NONE"),
            ("retry_requires_idempotency", (policy?.RetryRequiresIdempotency ?? false) ? "1" : "0"),
            ("dead_letter", (policy?.DeadLetterOnTerminalFailure ?? false) ? "1" : "0"),
            ("traffic_class", ((int)(policy?.TrafficClass ?? (DeliveryTrafficClass)0)).ToString(CultureInfo.InvariantCulture)),
            ("attempt_number", (context?.AttemptNumber ?? 0).ToString(CultureInfo.InvariantCulture)),
            ("previous_outcome_id", previous?.OutcomeId ?? "NONE"),
            ("previous_delivery_decision_id", previous?.DeliveryDecisionId ?? "NONE"),
            ("previous_observation", ((int)(previous?.Observation ?? (TransportObservationKind)0)).ToString(CultureInfo.InvariantCulture)),
            ("idempotency_identity", idempotency?.IdempotencyIdentity ?? "NONE"),
            ("idempotency_evidence", idempotency?.EvidenceReference ?? "NONE"),
            ("destination_health", ((int)(context?.DestinationHealth ?? (DeliveryDestinationHealth)0)).ToString(CultureInfo.InvariantCulture)),
            ("pressure_snapshot_id", pressureId),
            ("effective_expiry", admission?.EffectiveExpiry?.ToString("O", CultureInfo.InvariantCulture) ?? "NONE"),
            ("priority_authority_reference", authority?.AuthorityReference ?? "NONE"),
            ("priority_authority_decision_id", authorityResult?.DecisionId ?? "NONE"),
            ("priority_authority_decision", authorityResult?.Decision ?? "NONE"),
            ("priority_authority_scope", authorityResult?.EffectiveScope ?? "NONE"),
            ("priority_authority_policy", authorityResult?.ControllingPolicy ?? "NONE"),
            ("priority_authority_policy_version", authorityResult?.PolicyVersion ?? "NONE"),
            ("priority_authority_conditions", authorityResult?.MaterialConditions ?? "NONE"),
            ("priority_authority_constraints", authorityResult?.Constraints ?? "NONE"),
            ("priority_authority_reason", authorityResult?.Reason ?? "NONE"),
            ("priority_authority_decision_time", authorityResult?.DecisionTime.ToString("O", CultureInfo.InvariantCulture) ?? "NONE"),
            ("priority_authority_expiry", authorityResult?.Expiry.ToString("O", CultureInfo.InvariantCulture) ?? "NONE"),
            ("priority_authority_evidence", authorityResult?.EvidenceReference ?? "NONE"),
            ("priority_binding_evidence", authority?.BindingEvidence ?? "NONE"),
            ("observation_time", observationTime.ToString("O", CultureInfo.InvariantCulture)),
            ("decision_evidence", evidence));

        return new DeliveryDecision(
            decision,
            reason,
            decisionId,
            route?.DecisionId ?? "UNAVAILABLE",
            route?.RegistrySnapshotDigest ?? "UNAVAILABLE",
            admission?.DecisionId ?? "UNAVAILABLE",
            admission?.MessageId ?? "UNAVAILABLE",
            correlationId,
            causationId,
            admission?.ProducerApplicationId ?? "UNAVAILABLE",
            route?.RouteId ?? "UNAVAILABLE",
            route?.RouteVersion ?? "UNAVAILABLE",
            policy?.PolicyId ?? "UNAVAILABLE",
            policy?.PolicyVersion ?? "UNAVAILABLE",
            policy?.Guarantee ?? (DeliveryGuarantee)0,
            policy?.OrderingGuarantee ?? (DeliveryOrderingGuarantee)0,
            policy?.OrderingKey ?? "NONE",
            policy?.TrafficClass ?? (DeliveryTrafficClass)0,
            context?.AttemptNumber ?? 0,
            previous?.OutcomeId ?? "NONE",
            idempotency?.IdempotencyIdentity ?? "NONE",
            context?.DestinationHealth ?? (DeliveryDestinationHealth)0,
            pressureId,
            observationTime,
            evidence);
    }
}

internal static class DeliveryCanonicalization
{
    internal static string Hash(params (string Name, string Value)[] fields)
    {
        var builder = new StringBuilder(fields.Length * 64);
        foreach (var field in fields)
        {
            Append(builder, field.Name, field.Value);
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    private static void Append(StringBuilder builder, string name, string value)
    {
        builder.Append(Encoding.UTF8.GetByteCount(name).ToString(CultureInfo.InvariantCulture));
        builder.Append(':');
        builder.Append(name);
        builder.Append('=');
        builder.Append(Encoding.UTF8.GetByteCount(value).ToString(CultureInfo.InvariantCulture));
        builder.Append(':');
        builder.Append(value);
        builder.Append('\n');
    }
}

internal static class DeliveryRules
{
    internal static T RequireDefined<T>(T value, string parameterName) where T : struct, Enum
    {
        if (!Enum.IsDefined(typeof(T), value)) throw new ArgumentOutOfRangeException(parameterName);
        return value;
    }

    internal static string RequireIdentifier(string value, string parameterName)
    {
        var canonical = RequireCanonicalText(value, parameterName);
        foreach (var c in canonical)
        {
            if (!(char.IsLetterOrDigit(c) || c is '-' or '_' or '.' or '/' or ':'))
                throw new ArgumentException("identifier_contains_invalid_character", parameterName);
        }
        return canonical;
    }

    internal static string RequireVersion(string value, string parameterName)
    {
        var canonical = RequireCanonicalText(value, parameterName);
        foreach (var c in canonical)
        {
            if (!(char.IsLetterOrDigit(c) || c is '.' or '-' or '_'))
                throw new ArgumentException("version_contains_invalid_character", parameterName);
        }
        return canonical;
    }

    internal static string RequireCanonicalText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !StringComparer.Ordinal.Equals(value, value.Trim()))
            throw new ArgumentException("value_required_and_canonical", parameterName);
        return value;
    }

    internal static void RequireUtc(DateTimeOffset value, string parameterName)
    {
        if (value == default || value.Offset != TimeSpan.Zero)
            throw new ArgumentException("time_must_be_utc", parameterName);
    }

    internal static void RequireCapacity(int limit, int inFlight, string limitName, string inFlightName)
    {
        if (limit < 1) throw new ArgumentOutOfRangeException(limitName, "capacity_limit_must_be_positive");
        if (inFlight < 0 || inFlight > limit) throw new ArgumentOutOfRangeException(inFlightName, "in_flight_out_of_range");
    }
}
