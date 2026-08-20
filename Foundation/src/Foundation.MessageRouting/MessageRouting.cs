using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.ApplicationManifest;
using Foundation.Contracts;
using Foundation.MessageAdmission;

namespace Foundation.MessageRouting;

public enum RouteState { Eligible = 1, Isolated = 2, Unavailable = 3 }
public enum RouteEndpointState { Eligible = 1, Isolated = 2, Unavailable = 3, Unknown = 4 }
public enum RouteSelectionDecision { Selected = 1, Rejected = 2 }

public static class RouteRegistrationReason
{
    public const string Registered = "ROUTE_REGISTERED";
    public const string NullDeclaration = "NULL_ROUTE_DECLARATION";
    public const string DuplicateIdentity = "DUPLICATE_ROUTE_IDENTITY";
    public const string ManifestUnknown = "ROUTE_MANIFEST_UNKNOWN";
    public const string ManifestDigestMismatch = "ROUTE_MANIFEST_DIGEST_MISMATCH";
    public const string ManifestApplicationMismatch = "ROUTE_MANIFEST_APPLICATION_MISMATCH";
    public const string ManifestConsumerUndeclared = "ROUTE_MANIFEST_CONSUMER_UNDECLARED";
    public const string ManifestCommunicationUndeclared = "ROUTE_MANIFEST_COMMUNICATION_UNDECLARED";
    public const string ManifestCommunicationInvalid = "ROUTE_MANIFEST_COMMUNICATION_INVALID";
    public const string ManifestAuthorityUndeclared = "ROUTE_MANIFEST_AUTHORITY_UNDECLARED";
    public const string AuthorityMalformed = "ROUTE_AUTHORITY_MALFORMED";
    public const string AuthorityBindingMismatch = "ROUTE_AUTHORITY_BINDING_MISMATCH";
    public const string AuthorityDenied = "ROUTE_AUTHORITY_DENIED";
}

public static class RouteSelectionReason
{
    public const string InvalidContext = "INVALID_ROUTING_CONTEXT";
    public const string MessageNotAdmitted = "MESSAGE_NOT_ADMITTED";
    public const string AdmissionExpiredForRouting = "ADMISSION_EXPIRED_FOR_ROUTING";
    public const string MessageTypeBindingMissing = "MESSAGE_TYPE_BINDING_MISSING";
    public const string MessageTypeBindingMismatch = "MESSAGE_TYPE_BINDING_MISMATCH";
    public const string NoDeclaredRoute = "NO_DECLARED_ROUTE";
    public const string RouteAmbiguous = "ROUTE_AMBIGUOUS";
    public const string RouteIsolated = "ROUTE_ISOLATED";
    public const string RouteUnavailable = "ROUTE_UNAVAILABLE";
    public const string SourceBindingMismatch = "SOURCE_BINDING_MISMATCH";
    public const string DestinationBindingMismatch = "DESTINATION_BINDING_MISMATCH";
    public const string ConsumerBindingMismatch = "CONSUMER_BINDING_MISMATCH";
    public const string MessageTypeMismatch = "MESSAGE_TYPE_MISMATCH";
    public const string RoutePurposeMismatch = "ROUTE_PURPOSE_MISMATCH";
    public const string ManifestBindingMismatch = "ROUTE_MANIFEST_BINDING_MISMATCH";
    public const string RouteAuthorityNotYetEffective = "ROUTE_AUTHORITY_NOT_YET_EFFECTIVE";
    public const string RouteAuthorityExpired = "ROUTE_AUTHORITY_EXPIRED";
    public const string SourceEndpointIneligible = "SOURCE_ENDPOINT_INELIGIBLE";
    public const string DestinationEndpointIneligible = "DESTINATION_ENDPOINT_INELIGIBLE";
    public const string RouteSelected = "ROUTE_SELECTED";
}

public sealed record RouteIdentity
{
    public RouteIdentity(string value) => Value = RoutingRules.RequireIdentifier(value, nameof(value));
    public string Value { get; }
    public override string ToString() => Value;
}

public sealed record RouteEndpointIdentity
{
    public RouteEndpointIdentity(string value) => Value = RoutingRules.RequireIdentifier(value, nameof(value));
    public string Value { get; }
    public override string ToString() => Value;
}

public sealed record RoutingMessageTypeBinding
{
    public RoutingMessageTypeBinding(string admissionDecisionId, string messageType, ProvenanceReference bindingEvidence)
    {
        AdmissionDecisionId = RoutingRules.RequireIdentifier(admissionDecisionId, nameof(admissionDecisionId));
        MessageType = RoutingRules.RequireCanonicalText(messageType, nameof(messageType));
        BindingEvidence = bindingEvidence ?? throw new ArgumentNullException(nameof(bindingEvidence));
    }

    public string AdmissionDecisionId { get; }
    public string MessageType { get; }
    public ProvenanceReference BindingEvidence { get; }
}

public sealed record RouteAuthorityBinding
{
    public RouteAuthorityBinding(
        AuthorityReference authorityReference,
        AuthorityResult authorityResult,
        RouteIdentity authorizedRouteId,
        string authorizedRouteVersion,
        ProducerIdentityReference authorizedProducerIdentity,
        ApplicationIdentityReference authorizedApplicationId,
        RecipientScopeReference authorizedRecipientScope,
        ManifestReference authorizedConsumer,
        string authorizedMessageType,
        string authorizedPurpose,
        string effectiveScope,
        ProvenanceReference bindingEvidence)
    {
        AuthorityReference = authorityReference ?? throw new ArgumentNullException(nameof(authorityReference));
        AuthorityResult = authorityResult ?? throw new ArgumentNullException(nameof(authorityResult));
        AuthorizedRouteId = authorizedRouteId ?? throw new ArgumentNullException(nameof(authorizedRouteId));
        AuthorizedRouteVersion = RoutingRules.RequireVersion(authorizedRouteVersion, nameof(authorizedRouteVersion));
        AuthorizedProducerIdentity = authorizedProducerIdentity ?? throw new ArgumentNullException(nameof(authorizedProducerIdentity));
        AuthorizedApplicationId = authorizedApplicationId ?? throw new ArgumentNullException(nameof(authorizedApplicationId));
        AuthorizedRecipientScope = authorizedRecipientScope ?? throw new ArgumentNullException(nameof(authorizedRecipientScope));
        AuthorizedConsumer = authorizedConsumer ?? throw new ArgumentNullException(nameof(authorizedConsumer));
        AuthorizedMessageType = RoutingRules.RequireCanonicalText(authorizedMessageType, nameof(authorizedMessageType));
        AuthorizedPurpose = RoutingRules.RequireIdentifier(authorizedPurpose, nameof(authorizedPurpose));
        EffectiveScope = RoutingRules.RequireIdentifier(effectiveScope, nameof(effectiveScope));
        BindingEvidence = bindingEvidence ?? throw new ArgumentNullException(nameof(bindingEvidence));
    }

    public AuthorityReference AuthorityReference { get; }
    public AuthorityResult AuthorityResult { get; }
    public RouteIdentity AuthorizedRouteId { get; }
    public string AuthorizedRouteVersion { get; }
    public ProducerIdentityReference AuthorizedProducerIdentity { get; }
    public ApplicationIdentityReference AuthorizedApplicationId { get; }
    public RecipientScopeReference AuthorizedRecipientScope { get; }
    public ManifestReference AuthorizedConsumer { get; }
    public string AuthorizedMessageType { get; }
    public string AuthorizedPurpose { get; }
    public string EffectiveScope { get; }
    public ProvenanceReference BindingEvidence { get; }
}

public sealed record RouteDeclaration
{
    public RouteDeclaration(
        RouteIdentity routeId,
        string routeVersion,
        ManifestIdentity sourceManifestId,
        string sourceManifestVersion,
        string sourceManifestDigest,
        ProducerIdentityReference sourceProducerIdentity,
        ApplicationIdentityReference sourceApplicationId,
        RecipientScopeReference destinationRecipientScope,
        ManifestReference intendedConsumer,
        string messageType,
        RouteEndpointIdentity sourceEndpoint,
        RouteEndpointIdentity destinationEndpoint,
        string purpose,
        RouteState state,
        RouteAuthorityBinding authorityBinding,
        ProvenanceReference evidenceReference)
    {
        RouteId = routeId ?? throw new ArgumentNullException(nameof(routeId));
        RouteVersion = RoutingRules.RequireVersion(routeVersion, nameof(routeVersion));
        SourceManifestId = sourceManifestId ?? throw new ArgumentNullException(nameof(sourceManifestId));
        SourceManifestVersion = RoutingRules.RequireVersion(sourceManifestVersion, nameof(sourceManifestVersion));
        SourceManifestDigest = RoutingRules.RequireSha256(sourceManifestDigest, nameof(sourceManifestDigest));
        SourceProducerIdentity = sourceProducerIdentity ?? throw new ArgumentNullException(nameof(sourceProducerIdentity));
        SourceApplicationId = sourceApplicationId ?? throw new ArgumentNullException(nameof(sourceApplicationId));
        DestinationRecipientScope = destinationRecipientScope ?? throw new ArgumentNullException(nameof(destinationRecipientScope));
        IntendedConsumer = intendedConsumer ?? throw new ArgumentNullException(nameof(intendedConsumer));
        MessageType = RoutingRules.RequireCanonicalText(messageType, nameof(messageType));
        SourceEndpoint = sourceEndpoint ?? throw new ArgumentNullException(nameof(sourceEndpoint));
        DestinationEndpoint = destinationEndpoint ?? throw new ArgumentNullException(nameof(destinationEndpoint));
        Purpose = RoutingRules.RequireIdentifier(purpose, nameof(purpose));
        State = RoutingRules.RequireDefined(state, nameof(state));
        AuthorityBinding = authorityBinding ?? throw new ArgumentNullException(nameof(authorityBinding));
        EvidenceReference = evidenceReference ?? throw new ArgumentNullException(nameof(evidenceReference));
    }

    public RouteIdentity RouteId { get; }
    public string RouteVersion { get; }
    public ManifestIdentity SourceManifestId { get; }
    public string SourceManifestVersion { get; }
    public string SourceManifestDigest { get; }
    public ProducerIdentityReference SourceProducerIdentity { get; }
    public ApplicationIdentityReference SourceApplicationId { get; }
    public RecipientScopeReference DestinationRecipientScope { get; }
    public ManifestReference IntendedConsumer { get; }
    public string MessageType { get; }
    public RouteEndpointIdentity SourceEndpoint { get; }
    public RouteEndpointIdentity DestinationEndpoint { get; }
    public string Purpose { get; }
    public RouteState State { get; }
    public RouteAuthorityBinding AuthorityBinding { get; }
    public ProvenanceReference EvidenceReference { get; }
}

public sealed record RouteRegistrationResult(bool Accepted, string Reason);

public sealed class RouteRegistry
{
    private const string AllowDecision = "ALLOW";
    private readonly object gate = new();
    private readonly Dictionary<string, RouteDeclaration> routes = new(StringComparer.Ordinal);
    private readonly IApplicationCommunicationManifestRegistry manifestRegistry;

    public RouteRegistry(IApplicationCommunicationManifestRegistry manifestRegistry)
    {
        this.manifestRegistry = manifestRegistry ?? throw new ArgumentNullException(nameof(manifestRegistry));
    }

    public RouteRegistrationResult Register(RouteDeclaration? declaration)
    {
        if (declaration is null) return new(false, RouteRegistrationReason.NullDeclaration);

        ManifestResolutionResult resolution;
        try
        {
            resolution = manifestRegistry.Resolve(declaration.SourceManifestId, declaration.SourceManifestVersion);
        }
        catch (ArgumentException)
        {
            return new(false, RouteRegistrationReason.ManifestUnknown);
        }

        if (!resolution.Resolved || resolution.Manifest is null || resolution.ManifestSha256 is null)
            return new(false, RouteRegistrationReason.ManifestUnknown);
        if (!StringComparer.Ordinal.Equals(resolution.ManifestSha256, declaration.SourceManifestDigest))
            return new(false, RouteRegistrationReason.ManifestDigestMismatch);

        var manifest = resolution.Manifest;
        if (!StringComparer.Ordinal.Equals(manifest.ApplicationId.Value, declaration.SourceApplicationId.Value))
            return new(false, RouteRegistrationReason.ManifestApplicationMismatch);
        if (manifest.IntendedConsumers.Count(x => StringComparer.Ordinal.Equals(x.Value, declaration.IntendedConsumer.Value)) != 1)
            return new(false, RouteRegistrationReason.ManifestConsumerUndeclared);

        var communications = manifest.Communications.Where(x => StringComparer.Ordinal.Equals(x.MessageType, declaration.MessageType)).ToArray();
        if (communications.Length == 0) return new(false, RouteRegistrationReason.ManifestCommunicationUndeclared);
        if (communications.Length != 1 || communications[0].Direction != CommunicationDirection.Outbound || communications[0].Role != CommunicationRole.Producer)
            return new(false, RouteRegistrationReason.ManifestCommunicationInvalid);

        var authority = declaration.AuthorityBinding;
        if (manifest.AuthorityRequests.Count(x => StringComparer.Ordinal.Equals(x.Value, authority.AuthorityReference.Value)) != 1)
            return new(false, RouteRegistrationReason.ManifestAuthorityUndeclared);
        if (ContractValidators.Validate(authority.AuthorityResult).Result != ValidationResult.Pass)
            return new(false, RouteRegistrationReason.AuthorityMalformed);
        if (!AuthorityMatchesDeclaration(authority, declaration))
            return new(false, RouteRegistrationReason.AuthorityBindingMismatch);
        if (!StringComparer.Ordinal.Equals(authority.EffectiveScope, authority.AuthorityResult.EffectiveScope))
            return new(false, RouteRegistrationReason.AuthorityBindingMismatch);
        if (!StringComparer.Ordinal.Equals(authority.AuthorityResult.Decision, AllowDecision))
            return new(false, RouteRegistrationReason.AuthorityDenied);

        var key = RoutingRules.RouteKey(declaration.RouteId.Value, declaration.RouteVersion);
        lock (gate)
        {
            if (routes.ContainsKey(key)) return new(false, RouteRegistrationReason.DuplicateIdentity);
            routes.Add(key, declaration);
        }
        return new(true, RouteRegistrationReason.Registered);
    }

    public IReadOnlyList<RouteDeclaration> Snapshot()
    {
        lock (gate)
        {
            return new ReadOnlyCollection<RouteDeclaration>(routes.Values
                .OrderBy(x => x.RouteId.Value, StringComparer.Ordinal)
                .ThenBy(x => x.RouteVersion, StringComparer.Ordinal)
                .ToList());
        }
    }

    private static bool AuthorityMatchesDeclaration(RouteAuthorityBinding authority, RouteDeclaration declaration) =>
        StringComparer.Ordinal.Equals(authority.AuthorizedRouteId.Value, declaration.RouteId.Value) &&
        StringComparer.Ordinal.Equals(authority.AuthorizedRouteVersion, declaration.RouteVersion) &&
        StringComparer.Ordinal.Equals(authority.AuthorizedProducerIdentity.Value, declaration.SourceProducerIdentity.Value) &&
        StringComparer.Ordinal.Equals(authority.AuthorizedApplicationId.Value, declaration.SourceApplicationId.Value) &&
        StringComparer.Ordinal.Equals(authority.AuthorizedRecipientScope.Value, declaration.DestinationRecipientScope.Value) &&
        StringComparer.Ordinal.Equals(authority.AuthorizedConsumer.Value, declaration.IntendedConsumer.Value) &&
        StringComparer.Ordinal.Equals(authority.AuthorizedMessageType, declaration.MessageType) &&
        StringComparer.Ordinal.Equals(authority.AuthorizedPurpose, declaration.Purpose);
}

public sealed record RouteEndpointStateBinding
{
    public RouteEndpointStateBinding(RouteEndpointIdentity endpointId, RouteEndpointState state, ProvenanceReference evidenceReference)
    {
        EndpointId = endpointId ?? throw new ArgumentNullException(nameof(endpointId));
        State = RoutingRules.RequireDefined(state, nameof(state));
        EvidenceReference = evidenceReference ?? throw new ArgumentNullException(nameof(evidenceReference));
    }
    public RouteEndpointIdentity EndpointId { get; }
    public RouteEndpointState State { get; }
    public ProvenanceReference EvidenceReference { get; }
}

public sealed record RouteSelectionContext
{
    public RouteSelectionContext(
        MessageAdmissionResult? admissionResult,
        RoutingMessageTypeBinding? messageTypeBinding,
        string routePurpose,
        DateTimeOffset observationTime,
        IEnumerable<RouteEndpointStateBinding>? endpointStates,
        ProvenanceReference decisionEvidence)
    {
        if (observationTime == default || observationTime.Offset != TimeSpan.Zero)
            throw new ArgumentException("observation_time_must_be_utc", nameof(observationTime));
        AdmissionResult = admissionResult;
        MessageTypeBinding = messageTypeBinding;
        RoutePurpose = RoutingRules.RequireIdentifier(routePurpose, nameof(routePurpose));
        ObservationTime = observationTime;
        EndpointStates = RoutingRules.FreezeEndpointStates(endpointStates);
        DecisionEvidence = decisionEvidence ?? throw new ArgumentNullException(nameof(decisionEvidence));
    }

    public MessageAdmissionResult? AdmissionResult { get; }
    public RoutingMessageTypeBinding? MessageTypeBinding { get; }
    public string RoutePurpose { get; }
    public DateTimeOffset ObservationTime { get; }
    public IReadOnlyList<RouteEndpointStateBinding> EndpointStates { get; }
    public ProvenanceReference DecisionEvidence { get; }
}

public sealed record RouteDecision
{
    internal RouteDecision(
        RouteSelectionDecision decision, string reason, string decisionId, string registrySnapshotDigest,
        string admissionDecisionId, string messageDigest, string messageId, string producerIdentity,
        string producerApplicationId, string manifestId, string manifestVersion, string recipientScope,
        string intendedConsumer, string messageType, string routePurpose, string routeId, string routeVersion,
        string sourceEndpoint, string destinationEndpoint, string routeAuthorityReference,
        string routeAuthorityDecisionId, string routeAuthorityEffectiveScope, string routeAuthorityEvidence,
        string routeManifestDigest, string routeEvidence, string routingEvidence, DateTimeOffset observationTime)
    {
        Decision = decision; Reason = reason; DecisionId = decisionId; RegistrySnapshotDigest = registrySnapshotDigest;
        AdmissionDecisionId = admissionDecisionId; MessageDigest = messageDigest; MessageId = messageId;
        ProducerIdentity = producerIdentity; ProducerApplicationId = producerApplicationId; ManifestId = manifestId;
        ManifestVersion = manifestVersion; RecipientScope = recipientScope; IntendedConsumer = intendedConsumer;
        MessageType = messageType; RoutePurpose = routePurpose; RouteId = routeId; RouteVersion = routeVersion;
        SourceEndpoint = sourceEndpoint; DestinationEndpoint = destinationEndpoint; RouteAuthorityReference = routeAuthorityReference;
        RouteAuthorityDecisionId = routeAuthorityDecisionId; RouteAuthorityEffectiveScope = routeAuthorityEffectiveScope;
        RouteAuthorityEvidence = routeAuthorityEvidence; RouteManifestDigest = routeManifestDigest;
        RouteEvidence = routeEvidence; RoutingEvidence = routingEvidence; ObservationTime = observationTime;
    }

    public RouteSelectionDecision Decision { get; }
    public string Reason { get; }
    public string DecisionId { get; }
    public string RegistrySnapshotDigest { get; }
    public string AdmissionDecisionId { get; }
    public string MessageDigest { get; }
    public string MessageId { get; }
    public string ProducerIdentity { get; }
    public string ProducerApplicationId { get; }
    public string ManifestId { get; }
    public string ManifestVersion { get; }
    public string RecipientScope { get; }
    public string IntendedConsumer { get; }
    public string MessageType { get; }
    public string RoutePurpose { get; }
    public string RouteId { get; }
    public string RouteVersion { get; }
    public string SourceEndpoint { get; }
    public string DestinationEndpoint { get; }
    public string RouteAuthorityReference { get; }
    public string RouteAuthorityDecisionId { get; }
    public string RouteAuthorityEffectiveScope { get; }
    public string RouteAuthorityEvidence { get; }
    public string RouteManifestDigest { get; }
    public string RouteEvidence { get; }
    public string RoutingEvidence { get; }
    public DateTimeOffset ObservationTime { get; }
}

public sealed class RouteSelectionEvaluator
{
    public RouteDecision Evaluate(RouteSelectionContext? context, RouteRegistry? registry)
    {
        if (registry is null) return Reject(context, null, RouteSelectionReason.InvalidContext);
        var snapshot = registry.Snapshot();
        if (context is null) return Reject(context, snapshot, RouteSelectionReason.InvalidContext);

        var admission = context.AdmissionResult;
        if (admission is null || admission.Decision != MessageAdmissionDecision.Admitted)
            return Reject(context, snapshot, RouteSelectionReason.MessageNotAdmitted);
        if (admission.EffectiveExpiry is DateTimeOffset admissionExpiry && admissionExpiry <= context.ObservationTime)
            return Reject(context, snapshot, RouteSelectionReason.AdmissionExpiredForRouting);

        var binding = context.MessageTypeBinding;
        if (binding is null) return Reject(context, snapshot, RouteSelectionReason.MessageTypeBindingMissing);
        if (!StringComparer.Ordinal.Equals(binding.AdmissionDecisionId, admission.DecisionId))
            return Reject(context, snapshot, RouteSelectionReason.MessageTypeBindingMismatch);
        if (snapshot.Count == 0) return Reject(context, snapshot, RouteSelectionReason.NoDeclaredRoute);

        var producerMatches = snapshot.Where(x =>
            StringComparer.Ordinal.Equals(x.SourceProducerIdentity.Value, admission.ProducerIdentity) &&
            StringComparer.Ordinal.Equals(x.SourceApplicationId.Value, admission.ProducerApplicationId)).ToArray();
        if (producerMatches.Length == 0) return Reject(context, snapshot, RouteSelectionReason.SourceBindingMismatch);

        var destinationMatches = producerMatches.Where(x => StringComparer.Ordinal.Equals(x.DestinationRecipientScope.Value, admission.RecipientScope)).ToArray();
        if (destinationMatches.Length == 0) return Reject(context, snapshot, RouteSelectionReason.DestinationBindingMismatch);

        var consumerMatches = destinationMatches.Where(x => StringComparer.Ordinal.Equals(x.IntendedConsumer.Value, admission.IntendedConsumer)).ToArray();
        if (consumerMatches.Length == 0) return Reject(context, snapshot, RouteSelectionReason.ConsumerBindingMismatch);

        var messageMatches = consumerMatches.Where(x => StringComparer.Ordinal.Equals(x.MessageType, binding.MessageType)).ToArray();
        if (messageMatches.Length == 0) return Reject(context, snapshot, RouteSelectionReason.MessageTypeMismatch);

        var purposeMatches = messageMatches.Where(x => StringComparer.Ordinal.Equals(x.Purpose, context.RoutePurpose)).ToArray();
        if (purposeMatches.Length == 0) return Reject(context, snapshot, RouteSelectionReason.RoutePurposeMismatch);

        var manifestMatches = purposeMatches.Where(x =>
            StringComparer.Ordinal.Equals(x.SourceManifestId.Value, admission.ManifestId) &&
            StringComparer.Ordinal.Equals(x.SourceManifestVersion, admission.ManifestVersion)).ToArray();
        if (manifestMatches.Length == 0) return Reject(context, snapshot, RouteSelectionReason.ManifestBindingMismatch);

        if (manifestMatches.All(x => x.State == RouteState.Isolated)) return Reject(context, snapshot, RouteSelectionReason.RouteIsolated);
        if (manifestMatches.All(x => x.State == RouteState.Unavailable)) return Reject(context, snapshot, RouteSelectionReason.RouteUnavailable);

        var stateEligible = manifestMatches.Where(x => x.State == RouteState.Eligible).ToArray();
        if (stateEligible.Length == 0) return Reject(context, snapshot, RouteSelectionReason.NoDeclaredRoute);

        var effectiveAuthority = stateEligible.Where(x => context.ObservationTime >= x.AuthorityBinding.AuthorityResult.DecisionTime).ToArray();
        if (effectiveAuthority.Length == 0) return Reject(context, snapshot, RouteSelectionReason.RouteAuthorityNotYetEffective);

        var unexpiredAuthority = effectiveAuthority.Where(x => context.ObservationTime < x.AuthorityBinding.AuthorityResult.Expiry).ToArray();
        if (unexpiredAuthority.Length == 0) return Reject(context, snapshot, RouteSelectionReason.RouteAuthorityExpired);

        var endpointEligible = unexpiredAuthority.Where(candidate =>
            IsEndpointEligible(context.EndpointStates, candidate.SourceEndpoint) &&
            IsEndpointEligible(context.EndpointStates, candidate.DestinationEndpoint)).ToArray();

        if (endpointEligible.Length == 0)
        {
            var sourceIneligible = unexpiredAuthority.Any(x => !IsEndpointEligible(context.EndpointStates, x.SourceEndpoint));
            return Reject(context, snapshot, sourceIneligible ? RouteSelectionReason.SourceEndpointIneligible : RouteSelectionReason.DestinationEndpointIneligible);
        }

        if (endpointEligible.Length > 1) return Reject(context, snapshot, RouteSelectionReason.RouteAmbiguous);
        return Select(context, snapshot, endpointEligible[0]);
    }

    private static bool IsEndpointEligible(IReadOnlyList<RouteEndpointStateBinding> states, RouteEndpointIdentity endpoint)
    {
        if (states.Count == 0) return true;
        var matches = states.Where(x => StringComparer.Ordinal.Equals(x.EndpointId.Value, endpoint.Value)).ToArray();
        return matches.Length == 1 && matches[0].State == RouteEndpointState.Eligible;
    }

    private static RouteDecision Select(RouteSelectionContext context, IReadOnlyList<RouteDeclaration> snapshot, RouteDeclaration route)
    {
        var admission = context.AdmissionResult!;
        var binding = context.MessageTypeBinding!;
        var registryCanonical = CanonicalRegistry(snapshot);
        return CreateDecision(RouteSelectionDecision.Selected, RouteSelectionReason.RouteSelected,
            Sha256(BuildCanonical(RouteSelectionDecision.Selected, RouteSelectionReason.RouteSelected, context, route, admission, binding, registryCanonical)),
            Sha256(registryCanonical), context, route, admission, binding);
    }

    private static RouteDecision Reject(RouteSelectionContext? context, IReadOnlyList<RouteDeclaration>? snapshot, string reason)
    {
        var admission = context?.AdmissionResult;
        var binding = context?.MessageTypeBinding;
        var registryCanonical = CanonicalRegistry(snapshot);
        return CreateDecision(RouteSelectionDecision.Rejected, reason,
            Sha256(BuildCanonical(RouteSelectionDecision.Rejected, reason, context, null, admission, binding, registryCanonical)),
            Sha256(registryCanonical), context, null, admission, binding);
    }

    private static RouteDecision CreateDecision(
        RouteSelectionDecision decision, string reason, string decisionId, string registryDigest,
        RouteSelectionContext? context, RouteDeclaration? route, MessageAdmissionResult? admission, RoutingMessageTypeBinding? binding) =>
        new(decision, reason, decisionId, registryDigest,
            admission?.DecisionId ?? string.Empty, admission?.MessageDigest ?? string.Empty, admission?.MessageId ?? string.Empty,
            admission?.ProducerIdentity ?? string.Empty, admission?.ProducerApplicationId ?? string.Empty,
            admission?.ManifestId ?? string.Empty, admission?.ManifestVersion ?? string.Empty,
            admission?.RecipientScope ?? string.Empty, admission?.IntendedConsumer ?? string.Empty,
            binding?.MessageType ?? string.Empty, context?.RoutePurpose ?? string.Empty,
            route?.RouteId.Value ?? string.Empty, route?.RouteVersion ?? string.Empty,
            route?.SourceEndpoint.Value ?? string.Empty, route?.DestinationEndpoint.Value ?? string.Empty,
            route?.AuthorityBinding.AuthorityReference.Value ?? string.Empty,
            route?.AuthorityBinding.AuthorityResult.DecisionId ?? string.Empty,
            route?.AuthorityBinding.EffectiveScope ?? string.Empty,
            route?.AuthorityBinding.BindingEvidence.Value ?? string.Empty,
            route?.SourceManifestDigest ?? string.Empty,
            route?.EvidenceReference.Value ?? string.Empty, context?.DecisionEvidence.Value ?? string.Empty,
            context?.ObservationTime ?? default);

    private static string CanonicalRegistry(IReadOnlyList<RouteDeclaration>? routeSnapshot)
    {
        if (routeSnapshot is null) return CanonicalFields("REGISTRY_NULL");
        if (routeSnapshot.Count == 0) return CanonicalFields("REGISTRY_EMPTY");
        return CanonicalFields(routeSnapshot.Select(x => CanonicalFields(
            x.RouteId.Value, x.RouteVersion,
            x.SourceManifestId.Value, x.SourceManifestVersion, x.SourceManifestDigest,
            x.SourceProducerIdentity.Value, x.SourceApplicationId.Value,
            x.DestinationRecipientScope.Value, x.IntendedConsumer.Value, x.MessageType,
            x.SourceEndpoint.Value, x.DestinationEndpoint.Value, x.Purpose,
            ((int)x.State).ToString(CultureInfo.InvariantCulture),
            x.AuthorityBinding.AuthorityReference.Value,
            x.AuthorityBinding.AuthorityResult.DecisionId,
            x.AuthorityBinding.AuthorityResult.Decision,
            x.AuthorityBinding.AuthorityResult.EffectiveScope,
            x.AuthorityBinding.AuthorityResult.DecisionTime.ToString("O", CultureInfo.InvariantCulture),
            x.AuthorityBinding.AuthorityResult.Expiry.ToString("O", CultureInfo.InvariantCulture),
            x.AuthorityBinding.AuthorizedRouteId.Value,
            x.AuthorityBinding.AuthorizedRouteVersion,
            x.AuthorityBinding.AuthorizedProducerIdentity.Value,
            x.AuthorityBinding.AuthorizedApplicationId.Value,
            x.AuthorityBinding.AuthorizedRecipientScope.Value,
            x.AuthorityBinding.AuthorizedConsumer.Value,
            x.AuthorityBinding.AuthorizedMessageType,
            x.AuthorityBinding.AuthorizedPurpose,
            x.AuthorityBinding.EffectiveScope,
            x.AuthorityBinding.BindingEvidence.Value,
            x.EvidenceReference.Value)).ToArray());
    }

    private static string BuildCanonical(
        RouteSelectionDecision decision, string reason, RouteSelectionContext? context, RouteDeclaration? route,
        MessageAdmissionResult? admission, RoutingMessageTypeBinding? binding, string registryCanonical)
    {
        var endpoints = context is null ? string.Empty : CanonicalFields(context.EndpointStates
            .OrderBy(x => x.EndpointId.Value, StringComparer.Ordinal)
            .Select(x => CanonicalFields(x.EndpointId.Value, ((int)x.State).ToString(CultureInfo.InvariantCulture), x.EvidenceReference.Value))
            .ToArray());

        return CanonicalFields(
            ((int)decision).ToString(CultureInfo.InvariantCulture), reason, registryCanonical,
            admission?.DecisionId ?? string.Empty, admission?.MessageDigest ?? string.Empty, admission?.MessageId ?? string.Empty,
            admission?.ProducerIdentity ?? string.Empty, admission?.ProducerApplicationId ?? string.Empty,
            admission?.ManifestId ?? string.Empty, admission?.ManifestVersion ?? string.Empty,
            admission?.RecipientScope ?? string.Empty, admission?.IntendedConsumer ?? string.Empty,
            binding?.AdmissionDecisionId ?? string.Empty, binding?.MessageType ?? string.Empty, binding?.BindingEvidence.Value ?? string.Empty,
            context?.RoutePurpose ?? string.Empty, context?.ObservationTime.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty,
            endpoints,
            route?.RouteId.Value ?? string.Empty, route?.RouteVersion ?? string.Empty,
            route?.SourceManifestId.Value ?? string.Empty, route?.SourceManifestVersion ?? string.Empty, route?.SourceManifestDigest ?? string.Empty,
            route?.SourceEndpoint.Value ?? string.Empty, route?.DestinationEndpoint.Value ?? string.Empty,
            route?.Purpose ?? string.Empty, route is null ? string.Empty : ((int)route.State).ToString(CultureInfo.InvariantCulture),
            route?.AuthorityBinding.AuthorityReference.Value ?? string.Empty,
            route?.AuthorityBinding.AuthorityResult.DecisionId ?? string.Empty,
            route?.AuthorityBinding.AuthorityResult.Decision ?? string.Empty,
            route?.AuthorityBinding.AuthorityResult.EffectiveScope ?? string.Empty,
            route?.AuthorityBinding.AuthorityResult.DecisionTime.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty,
            route?.AuthorityBinding.AuthorityResult.Expiry.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty,
            route?.AuthorityBinding.BindingEvidence.Value ?? string.Empty,
            route?.EvidenceReference.Value ?? string.Empty, context?.DecisionEvidence.Value ?? string.Empty);
    }

    private static string CanonicalFields(params string[] values)
    {
        var builder = new StringBuilder();
        foreach (var value in values)
        {
            var safe = value ?? string.Empty;
            builder.Append(safe.Length.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(safe);
        }
        return builder.ToString();
    }

    private static string Sha256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

internal static class RoutingRules
{
    public static string RequireIdentifier(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !StringComparer.Ordinal.Equals(value, value.Trim()))
            throw new ArgumentException("routing_identifier_required_and_canonical", parameterName);
        if (value.Length is < 3 or > 200)
            throw new ArgumentException("routing_identifier_length_invalid", parameterName);
        foreach (var character in value)
        {
            var allowed = character is >= 'A' and <= 'Z' || character is >= 'a' and <= 'z' || character is >= '0' and <= '9' || character is '-' or '_' or '.' or ':' or '/' or '@' or '+';
            if (!allowed) throw new ArgumentException("routing_identifier_character_invalid", parameterName);
        }
        return value;
    }

    public static string RequireCanonicalText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !StringComparer.Ordinal.Equals(value, value.Trim()))
            throw new ArgumentException("routing_value_required_and_canonical", parameterName);
        return value;
    }

    public static string RequireVersion(string value, string parameterName)
    {
        var parts = RequireCanonicalText(value, parameterName).Split('.', StringSplitOptions.None);
        if (parts.Length is < 2 or > 3 || parts.Any(part => part.Length == 0 || part.Any(c => c < '0' || c > '9') || (part.Length > 1 && part[0] == '0')))
            throw new ArgumentException("routing_version_not_canonical", parameterName);
        return value;
    }

    public static string RequireSha256(string value, string parameterName)
    {
        if (value is null || value.Length != 64 || value.Any(c => c is not (>= '0' and <= '9') and not (>= 'A' and <= 'F')))
            throw new ArgumentException("routing_sha256_invalid", parameterName);
        return value;
    }

    public static T RequireDefined<T>(T value, string parameterName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(parameterName);
        return value;
    }

    public static IReadOnlyList<RouteEndpointStateBinding> FreezeEndpointStates(IEnumerable<RouteEndpointStateBinding>? states)
    {
        var list = states?.ToList() ?? new List<RouteEndpointStateBinding>();
        if (list.Any(x => x is null)) throw new ArgumentException("endpoint_state_collection_contains_null", nameof(states));
        if (list.GroupBy(x => x.EndpointId.Value, StringComparer.Ordinal).Any(x => x.Count() > 1))
            throw new ArgumentException("duplicate_endpoint_state_binding", nameof(states));
        return new ReadOnlyCollection<RouteEndpointStateBinding>(list.OrderBy(x => x.EndpointId.Value, StringComparer.Ordinal).ToList());
    }

    public static string RouteKey(string routeId, string version) =>
        string.Concat(routeId.Length.ToString(CultureInfo.InvariantCulture), ":", routeId, version.Length.ToString(CultureInfo.InvariantCulture), ":", version);
}
