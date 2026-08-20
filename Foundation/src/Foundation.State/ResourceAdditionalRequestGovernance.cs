using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public enum ResourceRequesterKind
{
    DirectApplication = 0,
    DelegatedAggregateCoordinator = 1
}

public sealed record ResourceRequestAuthorityBinding
{
    private readonly ReadOnlyCollection<ApplicationPrincipalId> _authorizedApplications;

    public ResourceRequestAuthorityBinding(
        string authorityId,
        string authorizedRequesterInstanceId,
        string authorizedRequesterRoleId,
        ResourceScopeId authorizedScopeId,
        IEnumerable<ApplicationPrincipalId> authorizedApplications,
        long generation,
        ResourceEvidenceReference evidence,
        DateTimeOffset effectiveFrom,
        DateTimeOffset expiresAt)
    {
        AuthorityId = RequireIdentifier(authorityId, nameof(authorityId));
        AuthorizedRequesterInstanceId = RequireIdentifier(authorizedRequesterInstanceId, nameof(authorizedRequesterInstanceId));
        AuthorizedRequesterRoleId = RequireIdentifier(authorizedRequesterRoleId, nameof(authorizedRequesterRoleId));
        AuthorizedScopeId = authorizedScopeId ?? throw new ArgumentNullException(nameof(authorizedScopeId));
        ArgumentNullException.ThrowIfNull(authorizedApplications);
        if (generation <= 0) throw new ArgumentOutOfRangeException(nameof(generation));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (expiresAt <= effectiveFrom) throw new ArgumentException("Request authority must have a positive bounded lifetime.", nameof(expiresAt));

        var applications = authorizedApplications
            .Select(item => item ?? throw new ArgumentException("Authorized Application identity cannot be null.", nameof(authorizedApplications)))
            .OrderBy(item => item.Value, StringComparer.Ordinal)
            .ToArray();
        if (applications.Length == 0) throw new ArgumentException("Request authority must bind at least one exact Application identity.", nameof(authorizedApplications));
        if (applications.Select(item => item.Value).Distinct(StringComparer.Ordinal).Count() != applications.Length)
            throw new ArgumentException("Authorized Application identities must be unique.", nameof(authorizedApplications));

        _authorizedApplications = Array.AsReadOnly(applications);
        Generation = generation;
        EffectiveFrom = effectiveFrom;
        ExpiresAt = expiresAt;
        IdentitySha256 = ComputeIdentity();
    }

    public string AuthorityId { get; }
    public string AuthorizedRequesterInstanceId { get; }
    public string AuthorizedRequesterRoleId { get; }
    public ResourceScopeId AuthorizedScopeId { get; }
    public IReadOnlyList<ApplicationPrincipalId> AuthorizedApplications => _authorizedApplications;
    public long Generation { get; }
    public ResourceEvidenceReference Evidence { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }

    internal void ValidateAt(DateTimeOffset at, ResourceEpochId epoch)
    {
        if (at < EffectiveFrom || at >= ExpiresAt) throw new InvalidOperationException("Request authority is not effective at the required time.");
        ValidateEvidence(Evidence, at, epoch, "request authority");
    }

    internal void ValidateRequesterAndScope(string requesterInstanceId, string requesterRoleId, ResourceScopeId requestScopeId, IReadOnlyList<ApplicationPrincipalId> requestApplications)
    {
        if (!StringComparer.Ordinal.Equals(AuthorizedRequesterInstanceId, requesterInstanceId)) throw new InvalidOperationException("Request authority requester-instance mismatch.");
        if (!StringComparer.Ordinal.Equals(AuthorizedRequesterRoleId, requesterRoleId)) throw new InvalidOperationException("Request authority requester-role mismatch.");
        if (!StringComparer.Ordinal.Equals(AuthorizedScopeId.Value, requestScopeId.Value)) throw new InvalidOperationException("Request authority scope mismatch.");
        var expected = _authorizedApplications.Select(item => item.Value).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        var actual = requestApplications.Select(item => item.Value).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        if (!expected.SequenceEqual(actual, StringComparer.Ordinal)) throw new InvalidOperationException("Request authority Application scope mismatch.");
    }

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("authorityId", AuthorityId), new("requesterInstance", AuthorizedRequesterInstanceId), new("requesterRole", AuthorizedRequesterRoleId),
            new("scope", AuthorizedScopeId.Value), new("generation", Generation.ToString(CultureInfo.InvariantCulture)),
            new("evidenceId", Evidence.EvidenceId.Value), new("evidenceScope", Evidence.ScopeId.Value),
            new("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("evidenceEpoch", Evidence.EpochId.Value),
            new("effectiveFrom", EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        };
        for (var index = 0; index < _authorizedApplications.Count; index++) fields.Add(new CanonicalIdentityField($"authorizedApplication[{index:D4}]", _authorizedApplications[index].Value));
        return CanonicalResourceIdentity.ComputeSha256(fields);
    }

    internal static string RequireIdentifier(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Identifier is required.", parameterName);
        if (!StringComparer.Ordinal.Equals(value, value.Trim())) throw new ArgumentException("Identifier must not contain leading/trailing whitespace.", parameterName);
        if (value.Any(char.IsWhiteSpace)) throw new ArgumentException("Identifier must not contain whitespace.", parameterName);
        return value;
    }

    internal static void ValidateEvidence(ResourceEvidenceReference evidence, DateTimeOffset at, ResourceEpochId epoch, string label)
    {
        if (!StringComparer.Ordinal.Equals(evidence.EpochId.Value, epoch.Value)) throw new ArgumentException($"{label} evidence epoch mismatch.");
        if (evidence.ObservedAt > at) throw new ArgumentException($"{label} evidence cannot be from the future.");
    }
}

public sealed record ResourceCoordinatorFence
{
    public ResourceCoordinatorFence(ResourceScopeId coordinationScopeId, string coordinatorInstanceId, long generation, string fencingToken, DateTimeOffset expiresAt, ResourceEvidenceReference evidence)
    {
        CoordinationScopeId = coordinationScopeId ?? throw new ArgumentNullException(nameof(coordinationScopeId));
        CoordinatorInstanceId = ResourceRequestAuthorityBinding.RequireIdentifier(coordinatorInstanceId, nameof(coordinatorInstanceId));
        FencingToken = ResourceRequestAuthorityBinding.RequireIdentifier(fencingToken, nameof(fencingToken));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (generation <= 0) throw new ArgumentOutOfRangeException(nameof(generation));
        Generation = generation;
        ExpiresAt = expiresAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("scope", CoordinationScopeId.Value), new CanonicalIdentityField("instance", CoordinatorInstanceId),
            new CanonicalIdentityField("generation", Generation.ToString(CultureInfo.InvariantCulture)), new CanonicalIdentityField("token", FencingToken),
            new CanonicalIdentityField("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("evidenceId", Evidence.EvidenceId.Value), new CanonicalIdentityField("evidenceScope", Evidence.ScopeId.Value),
            new CanonicalIdentityField("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new CanonicalIdentityField("evidenceEpoch", Evidence.EpochId.Value)
        });
    }
    public ResourceScopeId CoordinationScopeId { get; }
    public string CoordinatorInstanceId { get; }
    public long Generation { get; }
    public string FencingToken { get; }
    public DateTimeOffset ExpiresAt { get; }
    public ResourceEvidenceReference Evidence { get; }
    public string IdentitySha256 { get; }
}

public sealed record AdditionalResourceRequest
{
    private readonly ReadOnlyCollection<ApplicationPrincipalId> _representedApplications;

    public AdditionalResourceRequest(ResourceRequestId requestId, ResourceRequesterKind requesterKind, string requesterInstanceId, string requesterRoleId,
        ResourceRequestAuthorityBinding authority, ApplicationPrincipalId? directApplicationId, IEnumerable<ApplicationPrincipalId>? representedApplications,
        ResourceClassId resourceClassId, ResourceQuantity requestedQuantity, ResourceQuantity provenResidualNeed,
        ApplicationResourceAllocationSnapshot allocationSnapshot, FoundationResourcePressureSnapshot? pressureSnapshot,
        CorrelationId correlationId, CausationId causationId, ResourceEvidenceReference requestEvidence, ResourceEvidenceReference residualNeedEvidence,
        DateTimeOffset createdAt, DateTimeOffset expiresAt, bool internalCoordinationExhausted = false, ResourceCoordinatorFence? coordinatorFence = null)
    {
        RequestId = requestId ?? throw new ArgumentNullException(nameof(requestId));
        RequesterKind = requesterKind;
        RequesterInstanceId = ResourceRequestAuthorityBinding.RequireIdentifier(requesterInstanceId, nameof(requesterInstanceId));
        RequesterRoleId = ResourceRequestAuthorityBinding.RequireIdentifier(requesterRoleId, nameof(requesterRoleId));
        Authority = authority ?? throw new ArgumentNullException(nameof(authority));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        RequestedQuantity = requestedQuantity ?? throw new ArgumentNullException(nameof(requestedQuantity));
        ProvenResidualNeed = provenResidualNeed ?? throw new ArgumentNullException(nameof(provenResidualNeed));
        AllocationSnapshot = allocationSnapshot ?? throw new ArgumentNullException(nameof(allocationSnapshot));
        PressureSnapshot = pressureSnapshot;
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        CausationId = causationId ?? throw new ArgumentNullException(nameof(causationId));
        RequestEvidence = requestEvidence ?? throw new ArgumentNullException(nameof(requestEvidence));
        ResidualNeedEvidence = residualNeedEvidence ?? throw new ArgumentNullException(nameof(residualNeedEvidence));
        if (expiresAt <= createdAt) throw new ArgumentException("Request expiry must be after creation.", nameof(expiresAt));
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        DirectApplicationId = directApplicationId;
        InternalCoordinationExhausted = internalCoordinationExhausted;
        CoordinatorFence = coordinatorFence;

        if (!StringComparer.Ordinal.Equals(RequestedQuantity.Unit, ProvenResidualNeed.Unit)) throw new ArgumentException("Requested quantity and proven residual need must use the same unit.");
        if (RequestedQuantity.Amount <= 0m) throw new ArgumentOutOfRangeException(nameof(requestedQuantity));
        if (ProvenResidualNeed.Amount <= 0m || ProvenResidualNeed.Amount > RequestedQuantity.Amount) throw new ArgumentOutOfRangeException(nameof(provenResidualNeed));
        if (AllocationSnapshot.ObservedAt > CreatedAt) throw new ArgumentException("Allocation snapshot cannot be from the future relative to the request.", nameof(allocationSnapshot));

        var epoch = AllocationSnapshot.ResourceTruth.EpochId;
        Authority.ValidateAt(CreatedAt, epoch);
        ResourceRequestAuthorityBinding.ValidateEvidence(RequestEvidence, CreatedAt, epoch, "request");
        ResourceRequestAuthorityBinding.ValidateEvidence(ResidualNeedEvidence, CreatedAt, epoch, "residual-need");
        if (PressureSnapshot is not null)
        {
            if (!StringComparer.Ordinal.Equals(PressureSnapshot.EpochId.Value, epoch.Value)) throw new ArgumentException("Pressure snapshot epoch mismatch.", nameof(pressureSnapshot));
            if (PressureSnapshot.ObservedAt > CreatedAt) throw new ArgumentException("Pressure snapshot cannot be from the future relative to the request.", nameof(pressureSnapshot));
            if (!StringComparer.Ordinal.Equals(PressureSnapshot.PrioritySnapshot.AllocationSnapshot.IdentitySha256, AllocationSnapshot.IdentitySha256))
                throw new ArgumentException("Pressure snapshot predecessor allocation identity does not match the request allocation snapshot.", nameof(pressureSnapshot));
        }

        var represented = (representedApplications ?? Array.Empty<ApplicationPrincipalId>()).Select(item => item ?? throw new ArgumentException("Represented Application identity cannot be null.", nameof(representedApplications))).OrderBy(item => item.Value, StringComparer.Ordinal).ToArray();
        if (represented.Select(item => item.Value).Distinct(StringComparer.Ordinal).Count() != represented.Length) throw new ArgumentException("Represented Application identities must be unique.", nameof(representedApplications));
        _representedApplications = Array.AsReadOnly(represented);

        switch (RequesterKind)
        {
            case ResourceRequesterKind.DirectApplication:
                if (DirectApplicationId is null) throw new ArgumentException("Direct requester requires an exact Application identity.", nameof(directApplicationId));
                if (_representedApplications.Count != 0) throw new ArgumentException("Direct requester cannot carry an aggregate represented set.", nameof(representedApplications));
                if (CoordinatorFence is not null) throw new ArgumentException("Direct requester cannot carry coordinator fencing state.", nameof(coordinatorFence));
                if (InternalCoordinationExhausted) throw new ArgumentException("Direct requester cannot claim aggregate internal-coordination exhaustion.", nameof(internalCoordinationExhausted));
                _ = AllocationSnapshot.GetRequiredAllocation(DirectApplicationId, ResourceClassId);
                Authority.ValidateRequesterAndScope(RequesterInstanceId, RequesterRoleId, Authority.AuthorizedScopeId, new[] { DirectApplicationId });
                break;
            case ResourceRequesterKind.DelegatedAggregateCoordinator:
                if (DirectApplicationId is not null) throw new ArgumentException("Aggregate coordinator request cannot masquerade as a direct Application request.", nameof(directApplicationId));
                if (_representedApplications.Count == 0) throw new ArgumentException("Aggregate coordinator request requires an exact non-empty constituent set.", nameof(representedApplications));
                if (!InternalCoordinationExhausted) throw new ArgumentException("Aggregate escalation requires evidence that the valid internal coordination path was exhausted.", nameof(internalCoordinationExhausted));
                if (CoordinatorFence is null) throw new ArgumentException("Aggregate coordinator request requires exact fencing state.", nameof(coordinatorFence));
                if (!StringComparer.Ordinal.Equals(CoordinatorFence.CoordinatorInstanceId, RequesterInstanceId)) throw new ArgumentException("Coordinator fence instance does not match requester instance.", nameof(coordinatorFence));
                if (CoordinatorFence.ExpiresAt <= CreatedAt) throw new ArgumentException("Coordinator fence is expired.", nameof(coordinatorFence));
                ResourceRequestAuthorityBinding.ValidateEvidence(CoordinatorFence.Evidence, CreatedAt, epoch, "coordinator-fence");
                foreach (var applicationId in _representedApplications) _ = AllocationSnapshot.GetRequiredAllocation(applicationId, ResourceClassId);
                Authority.ValidateRequesterAndScope(RequesterInstanceId, RequesterRoleId, CoordinatorFence.CoordinationScopeId, _representedApplications);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(requesterKind));
        }
        IdentitySha256 = ComputeIdentity();
    }

    public ResourceRequestId RequestId { get; }
    public ResourceRequesterKind RequesterKind { get; }
    public string RequesterInstanceId { get; }
    public string RequesterRoleId { get; }
    public ResourceRequestAuthorityBinding Authority { get; }
    public ApplicationPrincipalId? DirectApplicationId { get; }
    public IReadOnlyList<ApplicationPrincipalId> RepresentedApplications => _representedApplications;
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity RequestedQuantity { get; }
    public ResourceQuantity ProvenResidualNeed { get; }
    public ApplicationResourceAllocationSnapshot AllocationSnapshot { get; }
    public FoundationResourcePressureSnapshot? PressureSnapshot { get; }
    public CorrelationId CorrelationId { get; }
    public CausationId CausationId { get; }
    public ResourceEvidenceReference RequestEvidence { get; }
    public ResourceEvidenceReference ResidualNeedEvidence { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset ExpiresAt { get; }
    public bool InternalCoordinationExhausted { get; }
    public ResourceCoordinatorFence? CoordinatorFence { get; }
    public string IdentitySha256 { get; }

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("requestId", RequestId.Value), new("requesterKind", RequesterKind.ToString()), new("requesterInstance", RequesterInstanceId), new("requesterRole", RequesterRoleId),
            new("authorityIdentity", Authority.IdentitySha256), new("directApplication", DirectApplicationId?.Value), new("resourceClass", ResourceClassId.Value),
            CanonicalResourceIdentity.QuantityField("requested", RequestedQuantity), CanonicalResourceIdentity.QuantityField("residual", ProvenResidualNeed),
            new("allocationSnapshot", AllocationSnapshot.IdentitySha256), new("pressureSnapshot", PressureSnapshot?.IdentitySha256), new("correlation", CorrelationId.Value), new("causation", CausationId.Value),
            new("requestEvidenceId", RequestEvidence.EvidenceId.Value), new("requestEvidenceScope", RequestEvidence.ScopeId.Value), new("requestEvidenceObservedAt", RequestEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("requestEvidenceEpoch", RequestEvidence.EpochId.Value),
            new("residualEvidenceId", ResidualNeedEvidence.EvidenceId.Value), new("residualEvidenceScope", ResidualNeedEvidence.ScopeId.Value), new("residualEvidenceObservedAt", ResidualNeedEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("residualEvidenceEpoch", ResidualNeedEvidence.EpochId.Value),
            new("createdAt", CreatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new("internalCoordinationExhausted", InternalCoordinationExhausted ? "1" : "0"), new("coordinatorFenceIdentity", CoordinatorFence?.IdentitySha256)
        };
        for (var index = 0; index < _representedApplications.Count; index++) fields.Add(new CanonicalIdentityField($"representedApplication[{index:D4}]", _representedApplications[index].Value));
        return CanonicalResourceIdentity.ComputeSha256(fields);
    }
}

public sealed record ResourceAdditionalRequestDecisionPolicy
{
    public ResourceAdditionalRequestDecisionPolicy(ResourceClassId resourceClassId, ResourceQuantity maximumAdditionalPerDecision, bool deferWhenGlobalPressureUnavailable, ResourceEvidenceReference evidence, DateTimeOffset effectiveFrom, DateTimeOffset expiresAt)
    {
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        MaximumAdditionalPerDecision = maximumAdditionalPerDecision ?? throw new ArgumentNullException(nameof(maximumAdditionalPerDecision));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (MaximumAdditionalPerDecision.Amount <= 0m) throw new ArgumentOutOfRangeException(nameof(maximumAdditionalPerDecision));
        if (expiresAt <= effectiveFrom) throw new ArgumentException("Decision policy must have a positive bounded lifetime.", nameof(expiresAt));
        DeferWhenGlobalPressureUnavailable = deferWhenGlobalPressureUnavailable;
        EffectiveFrom = effectiveFrom;
        ExpiresAt = expiresAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("resourceClass", ResourceClassId.Value), CanonicalResourceIdentity.QuantityField("maximumAdditional", MaximumAdditionalPerDecision),
            new CanonicalIdentityField("deferWhenPressureUnavailable", DeferWhenGlobalPressureUnavailable ? "1" : "0"),
            new CanonicalIdentityField("evidenceId", Evidence.EvidenceId.Value), new CanonicalIdentityField("evidenceScope", Evidence.ScopeId.Value), new CanonicalIdentityField("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new CanonicalIdentityField("evidenceEpoch", Evidence.EpochId.Value),
            new CanonicalIdentityField("effectiveFrom", EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new CanonicalIdentityField("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity MaximumAdditionalPerDecision { get; }
    public bool DeferWhenGlobalPressureUnavailable { get; }
    public ResourceEvidenceReference Evidence { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }
}

public sealed record ResourceRequestDecisionAuthority
{
    public ResourceRequestDecisionAuthority(string authorityId, ResourceEvidenceReference evidence, DateTimeOffset effectiveFrom, DateTimeOffset expiresAt)
    {
        AuthorityId = ResourceRequestAuthorityBinding.RequireIdentifier(authorityId, nameof(authorityId));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (expiresAt <= effectiveFrom) throw new ArgumentException("Decision authority must have a positive bounded lifetime.", nameof(expiresAt));
        EffectiveFrom = effectiveFrom;
        ExpiresAt = expiresAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("authorityId", AuthorityId), new CanonicalIdentityField("evidenceId", Evidence.EvidenceId.Value), new CanonicalIdentityField("evidenceScope", Evidence.ScopeId.Value),
            new CanonicalIdentityField("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new CanonicalIdentityField("evidenceEpoch", Evidence.EpochId.Value),
            new CanonicalIdentityField("effectiveFrom", EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new CanonicalIdentityField("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }
    public string AuthorityId { get; }
    public ResourceEvidenceReference Evidence { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }
}

public sealed record AdditionalResourceDecisionRecord
{
    internal AdditionalResourceDecisionRecord(ResourceDecisionId decisionId, AdditionalResourceRequest request, ResourceDecisionKind outcome, ResourceQuantity decidedAdditionalQuantity, ResourceAdditionalRequestDecisionPolicy policy, ResourceRequestDecisionAuthority decisionAuthority, DateTimeOffset decidedAt)
    {
        DecisionId = decisionId; RequestId = request.RequestId; RequestIdentitySha256 = request.IdentitySha256; RequesterKind = request.RequesterKind;
        RequesterInstanceId = request.RequesterInstanceId; RequesterRoleId = request.RequesterRoleId; DirectApplicationId = request.DirectApplicationId;
        RepresentedApplications = Array.AsReadOnly(request.RepresentedApplications.ToArray()); ResourceClassId = request.ResourceClassId;
        RequestedQuantity = request.RequestedQuantity; ProvenResidualNeed = request.ProvenResidualNeed; Outcome = outcome; DecidedAdditionalQuantity = decidedAdditionalQuantity;
        PolicyIdentitySha256 = policy.IdentitySha256; PolicyEvidence = policy.Evidence; DecisionAuthority = decisionAuthority;
        AllocationSnapshotIdentitySha256 = request.AllocationSnapshot.IdentitySha256; PressureSnapshotIdentitySha256 = request.PressureSnapshot?.IdentitySha256;
        CorrelationId = request.CorrelationId; CausationId = request.CausationId; DecidedAt = decidedAt; ExpiresAt = request.ExpiresAt; IdentitySha256 = ComputeIdentity();
    }
    public ResourceDecisionId DecisionId { get; }
    public ResourceRequestId RequestId { get; }
    public string RequestIdentitySha256 { get; }
    public ResourceRequesterKind RequesterKind { get; }
    public string RequesterInstanceId { get; }
    public string RequesterRoleId { get; }
    public ApplicationPrincipalId? DirectApplicationId { get; }
    public IReadOnlyList<ApplicationPrincipalId> RepresentedApplications { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity RequestedQuantity { get; }
    public ResourceQuantity ProvenResidualNeed { get; }
    public ResourceDecisionKind Outcome { get; }
    public ResourceQuantity DecidedAdditionalQuantity { get; }
    public string PolicyIdentitySha256 { get; }
    public ResourceEvidenceReference PolicyEvidence { get; }
    public ResourceRequestDecisionAuthority DecisionAuthority { get; }
    public string AllocationSnapshotIdentitySha256 { get; }
    public string? PressureSnapshotIdentitySha256 { get; }
    public CorrelationId CorrelationId { get; }
    public CausationId CausationId { get; }
    public DateTimeOffset DecidedAt { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("decisionId", DecisionId.Value), new("requestId", RequestId.Value), new("requestIdentity", RequestIdentitySha256), new("requesterKind", RequesterKind.ToString()),
            new("requesterInstance", RequesterInstanceId), new("requesterRole", RequesterRoleId), new("directApplication", DirectApplicationId?.Value), new("resourceClass", ResourceClassId.Value),
            CanonicalResourceIdentity.QuantityField("requested", RequestedQuantity), CanonicalResourceIdentity.QuantityField("residual", ProvenResidualNeed), new("outcome", Outcome.ToString()),
            CanonicalResourceIdentity.QuantityField("decidedAdditional", DecidedAdditionalQuantity), new("policyIdentity", PolicyIdentitySha256), new("decisionAuthorityIdentity", DecisionAuthority.IdentitySha256),
            new("allocationSnapshot", AllocationSnapshotIdentitySha256), new("pressureSnapshot", PressureSnapshotIdentitySha256), new("correlation", CorrelationId.Value), new("causation", CausationId.Value),
            new("decidedAt", DecidedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        };
        for (var index = 0; index < RepresentedApplications.Count; index++) fields.Add(new CanonicalIdentityField($"representedApplication[{index:D4}]", RepresentedApplications[index].Value));
        return CanonicalResourceIdentity.ComputeSha256(fields);
    }
}

public sealed class AdditionalResourceRequestDecisionProcessor
{
    private sealed record FenceState(string CoordinatorInstanceId, long Generation, string FencingToken);
    private sealed record AuthorityState(long Generation, string IdentitySha256);
    private readonly Dictionary<string, ResourceAdditionalRequestDecisionPolicy> _policies;
    private readonly HashSet<string> _processedRequests = new(StringComparer.Ordinal);
    private readonly HashSet<string> _issuedDecisions = new(StringComparer.Ordinal);
    private readonly Dictionary<string, FenceState> _fenceByScope = new(StringComparer.Ordinal);
    private readonly Dictionary<string, AuthorityState> _authorityByScope = new(StringComparer.Ordinal);

    public AdditionalResourceRequestDecisionProcessor(IEnumerable<ResourceAdditionalRequestDecisionPolicy> policies, ResourceRequestDecisionAuthority decisionAuthority)
    {
        ArgumentNullException.ThrowIfNull(policies); DecisionAuthority = decisionAuthority ?? throw new ArgumentNullException(nameof(decisionAuthority));
        var ordered = policies.OrderBy(item => item.ResourceClassId.Value, StringComparer.Ordinal).ToArray();
        if (ordered.Length == 0) throw new ArgumentException("At least one additional-resource decision policy is required.", nameof(policies));
        if (ordered.Select(item => item.ResourceClassId.Value).Distinct(StringComparer.Ordinal).Count() != ordered.Length) throw new ArgumentException("Duplicate decision policy for resource class.", nameof(policies));
        _policies = ordered.ToDictionary(item => item.ResourceClassId.Value, StringComparer.Ordinal);
    }
    public ResourceRequestDecisionAuthority DecisionAuthority { get; }

    public AdditionalResourceDecisionRecord Evaluate(AdditionalResourceRequest request, ResourceDecisionId decisionId, DateTimeOffset decidedAt)
    {
        ArgumentNullException.ThrowIfNull(request); ArgumentNullException.ThrowIfNull(decisionId);
        if (decidedAt < request.CreatedAt) throw new ArgumentException("Decision cannot predate the request.", nameof(decidedAt));
        if (decidedAt >= request.ExpiresAt) throw new InvalidOperationException("Request has expired before decision.");
        if (!_processedRequests.Add(request.RequestId.Value)) throw new InvalidOperationException("Duplicate/replayed request identity rejected.");
        if (!_issuedDecisions.Add(decisionId.Value)) throw new InvalidOperationException("Duplicate/replayed decision identity rejected.");

        var epoch = request.AllocationSnapshot.ResourceTruth.EpochId;
        ValidateDecisionAuthority(decidedAt, epoch);
        if (!_policies.TryGetValue(request.ResourceClassId.Value, out var policy)) throw new KeyNotFoundException($"No WP-06 decision policy for resource class '{request.ResourceClassId.Value}'.");
        ValidatePolicy(policy, decidedAt, epoch, request.RequestedQuantity.Unit); ValidateAuthoritySupersession(request); ValidateFence(request, decidedAt);
        if (ShouldDeferForUnavailablePressure(request, policy)) return new AdditionalResourceDecisionRecord(decisionId, request, ResourceDecisionKind.Defer, new ResourceQuantity(0m, request.RequestedQuantity.Unit), policy, DecisionAuthority, decidedAt);

        var truth = request.AllocationSnapshot.ResourceTruth.GetRequired(request.ResourceClassId);
        if (!StringComparer.Ordinal.Equals(truth.AllocatableCapacity.Unit, request.RequestedQuantity.Unit)) throw new ArgumentException("Requested resource unit does not match authoritative Foundation resource truth.");
        var committedCeiling = request.AllocationSnapshot.Allocations.Where(item => StringComparer.Ordinal.Equals(item.ResourceClassId.Value, request.ResourceClassId.Value)).Sum(item => item.Ceiling.Amount);
        var free = Math.Max(0m, truth.AllocatableCapacity.Amount - committedCeiling);
        var candidate = Math.Min(request.ProvenResidualNeed.Amount, free);
        ResourceDecisionKind outcome; decimal decided;
        if (candidate <= 0m) { outcome = ResourceDecisionKind.Deny; decided = 0m; }
        else if (candidate > policy.MaximumAdditionalPerDecision.Amount) { outcome = ResourceDecisionKind.Cap; decided = policy.MaximumAdditionalPerDecision.Amount; }
        else if (candidate < request.RequestedQuantity.Amount) { outcome = ResourceDecisionKind.PartialGrant; decided = candidate; }
        else { outcome = ResourceDecisionKind.Grant; decided = candidate; }
        return new AdditionalResourceDecisionRecord(decisionId, request, outcome, new ResourceQuantity(decided, request.RequestedQuantity.Unit), policy, DecisionAuthority, decidedAt);
    }

    private void ValidateDecisionAuthority(DateTimeOffset decidedAt, ResourceEpochId epoch)
    {
        if (decidedAt < DecisionAuthority.EffectiveFrom || decidedAt >= DecisionAuthority.ExpiresAt) throw new InvalidOperationException("Decision authority is not effective.");
        ResourceRequestAuthorityBinding.ValidateEvidence(DecisionAuthority.Evidence, decidedAt, epoch, "decision authority");
    }
    private static void ValidatePolicy(ResourceAdditionalRequestDecisionPolicy policy, DateTimeOffset decidedAt, ResourceEpochId epoch, string requestedUnit)
    {
        if (decidedAt < policy.EffectiveFrom || decidedAt >= policy.ExpiresAt) throw new InvalidOperationException("Decision policy is not effective.");
        ResourceRequestAuthorityBinding.ValidateEvidence(policy.Evidence, decidedAt, epoch, "decision policy");
        if (!StringComparer.Ordinal.Equals(policy.MaximumAdditionalPerDecision.Unit, requestedUnit)) throw new ArgumentException("Decision policy unit does not match requested resource unit.");
    }
    private void ValidateAuthoritySupersession(AdditionalResourceRequest request)
    {
        var authority = request.Authority; var key = string.Join("|", authority.AuthorizedScopeId.Value, authority.AuthorizedRequesterInstanceId, authority.AuthorizedRequesterRoleId);
        if (_authorityByScope.TryGetValue(key, out var existing))
        {
            if (authority.Generation < existing.Generation) throw new InvalidOperationException("Superseded request authority generation rejected.");
            if (authority.Generation == existing.Generation && !StringComparer.Ordinal.Equals(authority.IdentitySha256, existing.IdentitySha256)) throw new InvalidOperationException("Conflicting request authority at the same generation rejected.");
        }
        _authorityByScope[key] = new AuthorityState(authority.Generation, authority.IdentitySha256);
    }
    private void ValidateFence(AdditionalResourceRequest request, DateTimeOffset decidedAt)
    {
        if (request.RequesterKind != ResourceRequesterKind.DelegatedAggregateCoordinator) return;
        var fence = request.CoordinatorFence ?? throw new InvalidOperationException("Aggregate request is missing coordinator fencing state.");
        if (fence.ExpiresAt <= decidedAt) throw new InvalidOperationException("Coordinator fencing state expired before decision.");
        var scope = fence.CoordinationScopeId.Value;
        if (_fenceByScope.TryGetValue(scope, out var existing))
        {
            if (fence.Generation < existing.Generation) throw new InvalidOperationException("Stale coordinator fencing generation rejected.");
            if (fence.Generation == existing.Generation && (!StringComparer.Ordinal.Equals(fence.CoordinatorInstanceId, existing.CoordinatorInstanceId) || !StringComparer.Ordinal.Equals(fence.FencingToken, existing.FencingToken))) throw new InvalidOperationException("Split-brain coordinator state rejected.");
        }
        _fenceByScope[scope] = new FenceState(fence.CoordinatorInstanceId, fence.Generation, fence.FencingToken);
    }
    private static bool ShouldDeferForUnavailablePressure(AdditionalResourceRequest request, ResourceAdditionalRequestDecisionPolicy policy)
    {
        if (!policy.DeferWhenGlobalPressureUnavailable) return false;
        if (request.PressureSnapshot is null) return true;
        var global = request.PressureSnapshot.Truth.SingleOrDefault(item => item.ScopeKind == ResourcePressureScopeKind.FoundationResourceClass && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, request.ResourceClassId.Value));
        return global is null || !global.PressureAvailable;
    }
}
