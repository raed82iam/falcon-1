using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

internal static class ResourceMutationGuard
{
    internal static string Id(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Identifier is required.", name);
        if (!StringComparer.Ordinal.Equals(value, value.Trim()) || value.Any(char.IsWhiteSpace))
            throw new ArgumentException("Identifier must be canonical and contain no whitespace.", name);
        return value;
    }

    internal static void SameUnit(ResourceQuantity expected, ResourceQuantity actual, string label)
    {
        if (!StringComparer.Ordinal.Equals(expected.Unit, actual.Unit)) throw new ArgumentException($"{label} unit mismatch.");
    }

    internal static void Evidence(ResourceEvidenceReference evidence, ResourceEpochId epoch, DateTimeOffset at, string label)
    {
        if (!StringComparer.Ordinal.Equals(evidence.EpochId.Value, epoch.Value)) throw new InvalidOperationException($"{label} evidence epoch mismatch.");
        if (evidence.ObservedAt > at) throw new InvalidOperationException($"{label} evidence cannot be from the future.");
    }

    internal static void Lifetime(ResourceEffectiveLifetime lifetime, DateTimeOffset at, string label)
    {
        if (lifetime.EffectiveFrom > at) throw new InvalidOperationException($"{label} is not yet effective.");
        if (lifetime.EffectiveUntil.HasValue && lifetime.EffectiveUntil.Value < at) throw new InvalidOperationException($"{label} is expired.");
    }

    internal static string AllocationKey(ApplicationPrincipalId applicationId, ResourceClassId resourceClassId)
        => string.Join("|", applicationId.Value, resourceClassId.Value);
}

public enum EffectiveDistributionOperationKind
{
    Borrow = 0,
    ReturnBorrowed = 1
}

public enum ResourceEffectLane
{
    DelegatedEffectiveDistribution = 0,
    FoundationAuthoritativeAllocation = 1
}

public sealed record ResourceCoordinationEnvelopeMember
{
    public ResourceCoordinationEnvelopeMember(
        ApplicationPrincipalId applicationId,
        ResourceGrantId grantId,
        ResourceClassId resourceClassId,
        ResourceQuantity protectedEffectiveMinimum,
        ResourceQuantity maximumBorrowOut,
        ResourceQuantity maximumBorrowIn,
        ResourcePreemptionEligibilityBinding reclaimabilityBinding)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        GrantId = grantId ?? throw new ArgumentNullException(nameof(grantId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        ProtectedEffectiveMinimum = protectedEffectiveMinimum ?? throw new ArgumentNullException(nameof(protectedEffectiveMinimum));
        MaximumBorrowOut = maximumBorrowOut ?? throw new ArgumentNullException(nameof(maximumBorrowOut));
        MaximumBorrowIn = maximumBorrowIn ?? throw new ArgumentNullException(nameof(maximumBorrowIn));
        ReclaimabilityBinding = reclaimabilityBinding ?? throw new ArgumentNullException(nameof(reclaimabilityBinding));

        ResourceMutationGuard.SameUnit(ProtectedEffectiveMinimum, MaximumBorrowOut, "maximum borrow-out");
        ResourceMutationGuard.SameUnit(ProtectedEffectiveMinimum, MaximumBorrowIn, "maximum borrow-in");
        if (!StringComparer.Ordinal.Equals(ReclaimabilityBinding.ApplicationId.Value, ApplicationId.Value) ||
            !StringComparer.Ordinal.Equals(ReclaimabilityBinding.GrantId.Value, GrantId.Value) ||
            !StringComparer.Ordinal.Equals(ReclaimabilityBinding.ResourceClassId.Value, ResourceClassId.Value))
            throw new ArgumentException("Reclaimability binding does not match exact envelope member identity.", nameof(reclaimabilityBinding));
        if (MaximumBorrowOut.Amount > 0m && ReclaimabilityBinding.Reclaimability == ResourceReclaimability.NonReclaimable)
            throw new ArgumentException("Non-reclaimable capacity cannot expose positive borrow-out authority.", nameof(maximumBorrowOut));
    }

    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceGrantId GrantId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity ProtectedEffectiveMinimum { get; }
    public ResourceQuantity MaximumBorrowOut { get; }
    public ResourceQuantity MaximumBorrowIn { get; }
    public ResourcePreemptionEligibilityBinding ReclaimabilityBinding { get; }
}

public sealed record ResourceCoordinationEnvelope
{
    private readonly ReadOnlyCollection<ResourceCoordinationEnvelopeMember> _members;

    public ResourceCoordinationEnvelope(
        string authorityId,
        ResourceScopeId scopeId,
        string coordinatorInstanceId,
        string coordinatorRoleId,
        long authorityGeneration,
        long fenceGeneration,
        string fencingToken,
        ApplicationResourceAllocationSnapshot allocationSnapshot,
        IEnumerable<ResourceCoordinationEnvelopeMember> members,
        ResourceEvidenceReference authorityEvidence,
        DateTimeOffset effectiveFrom,
        DateTimeOffset expiresAt)
    {
        AuthorityId = ResourceMutationGuard.Id(authorityId, nameof(authorityId));
        ScopeId = scopeId ?? throw new ArgumentNullException(nameof(scopeId));
        CoordinatorInstanceId = ResourceMutationGuard.Id(coordinatorInstanceId, nameof(coordinatorInstanceId));
        CoordinatorRoleId = ResourceMutationGuard.Id(coordinatorRoleId, nameof(coordinatorRoleId));
        FencingToken = ResourceMutationGuard.Id(fencingToken, nameof(fencingToken));
        AllocationSnapshot = allocationSnapshot ?? throw new ArgumentNullException(nameof(allocationSnapshot));
        AuthorityEvidence = authorityEvidence ?? throw new ArgumentNullException(nameof(authorityEvidence));
        ArgumentNullException.ThrowIfNull(members);
        if (authorityGeneration <= 0) throw new ArgumentOutOfRangeException(nameof(authorityGeneration));
        if (fenceGeneration <= 0) throw new ArgumentOutOfRangeException(nameof(fenceGeneration));
        if (expiresAt <= effectiveFrom) throw new ArgumentException("Coordination envelope must be bounded.", nameof(expiresAt));

        ResourceMutationGuard.Evidence(AuthorityEvidence, AllocationSnapshot.ResourceTruth.EpochId, effectiveFrom, "coordination-envelope authority");
        var ordered = members.Select(item => item ?? throw new ArgumentException("Envelope member cannot be null.", nameof(members)))
            .OrderBy(MemberKey, StringComparer.Ordinal).ToArray();
        if (ordered.Length == 0) throw new ArgumentException("Coordination envelope requires at least one member.", nameof(members));
        if (ordered.Select(MemberKey).Distinct(StringComparer.Ordinal).Count() != ordered.Length) throw new ArgumentException("Duplicate coordination-envelope member.", nameof(members));

        foreach (var member in ordered)
        {
            var allocation = AllocationSnapshot.GetRequiredAllocation(member.ApplicationId, member.ResourceClassId);
            if (!StringComparer.Ordinal.Equals(allocation.GrantId.Value, member.GrantId.Value)) throw new ArgumentException("Envelope member grant mismatch.", nameof(members));
            ResourceMutationGuard.SameUnit(allocation.Allocation, member.ProtectedEffectiveMinimum, "protected effective minimum");
            ResourceMutationGuard.SameUnit(allocation.Allocation, member.MaximumBorrowOut, "maximum borrow-out");
            ResourceMutationGuard.SameUnit(allocation.Allocation, member.MaximumBorrowIn, "maximum borrow-in");
            ResourceMutationGuard.Evidence(member.ReclaimabilityBinding.Evidence, AllocationSnapshot.ResourceTruth.EpochId, effectiveFrom, "reclaimability binding");
            ResourceMutationGuard.Lifetime(member.ReclaimabilityBinding.Lifetime, effectiveFrom, "reclaimability binding");
            if (member.ProtectedEffectiveMinimum.Amount > allocation.Allocation.Amount) throw new ArgumentException("Protected effective minimum exceeds native allocation.", nameof(members));
            if (member.MaximumBorrowOut.Amount > allocation.Allocation.Amount - member.ProtectedEffectiveMinimum.Amount) throw new ArgumentException("Maximum borrow-out exceeds movable granted capacity.", nameof(members));
            if (allocation.Allocation.Amount + member.MaximumBorrowIn.Amount > allocation.Ceiling.Amount) throw new ArgumentException("Maximum borrow-in exceeds authoritative ceiling space.", nameof(members));
        }

        _members = Array.AsReadOnly(ordered);
        AuthorityGeneration = authorityGeneration;
        FenceGeneration = fenceGeneration;
        EffectiveFrom = effectiveFrom;
        ExpiresAt = expiresAt;
        IdentitySha256 = ComputeIdentity();
    }

    public string AuthorityId { get; }
    public ResourceScopeId ScopeId { get; }
    public string CoordinatorInstanceId { get; }
    public string CoordinatorRoleId { get; }
    public long AuthorityGeneration { get; }
    public long FenceGeneration { get; }
    public string FencingToken { get; }
    public ApplicationResourceAllocationSnapshot AllocationSnapshot { get; }
    public IReadOnlyList<ResourceCoordinationEnvelopeMember> Members => _members;
    public ResourceEvidenceReference AuthorityEvidence { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }

    public void ValidateAt(DateTimeOffset at)
    {
        if (at < EffectiveFrom || at >= ExpiresAt) throw new InvalidOperationException("Coordination envelope is not effective.");
        foreach (var member in _members)
        {
            ResourceMutationGuard.Evidence(member.ReclaimabilityBinding.Evidence, AllocationSnapshot.ResourceTruth.EpochId, at, "reclaimability binding");
            ResourceMutationGuard.Lifetime(member.ReclaimabilityBinding.Lifetime, at, "reclaimability binding");
        }
    }

    public ResourceCoordinationEnvelopeMember GetMember(ApplicationPrincipalId applicationId, ResourceClassId resourceClassId)
        => _members.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.ApplicationId.Value, applicationId.Value) && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value))
           ?? throw new KeyNotFoundException("Application/resource-class is outside the coordination envelope.");

    public ResourceCoordinationEnvelopeMember GetMember(ApplicationPrincipalId applicationId, ResourceGrantId grantId, ResourceClassId resourceClassId)
        => _members.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.ApplicationId.Value, applicationId.Value) && StringComparer.Ordinal.Equals(item.GrantId.Value, grantId.Value) && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value))
           ?? throw new KeyNotFoundException("Application/grant/resource-class is outside the coordination envelope.");

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("authorityId", AuthorityId), new("scope", ScopeId.Value), new("coordinatorInstance", CoordinatorInstanceId), new("coordinatorRole", CoordinatorRoleId),
            new("authorityGeneration", AuthorityGeneration.ToString(CultureInfo.InvariantCulture)), new("fenceGeneration", FenceGeneration.ToString(CultureInfo.InvariantCulture)),
            new("fencingToken", FencingToken), new("allocationSnapshot", AllocationSnapshot.IdentitySha256),
            new("evidenceId", AuthorityEvidence.EvidenceId.Value), new("evidenceScope", AuthorityEvidence.ScopeId.Value),
            new("evidenceObservedAt", AuthorityEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("evidenceEpoch", AuthorityEvidence.EpochId.Value),
            new("effectiveFrom", EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        };
        for (var i = 0; i < _members.Count; i++)
        {
            var member = _members[i];
            var binding = member.ReclaimabilityBinding;
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].application", member.ApplicationId.Value));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].grant", member.GrantId.Value));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].resourceClass", member.ResourceClassId.Value));
            fields.Add(CanonicalResourceIdentity.QuantityField($"member[{i:D4}].minimum", member.ProtectedEffectiveMinimum));
            fields.Add(CanonicalResourceIdentity.QuantityField($"member[{i:D4}].maxOut", member.MaximumBorrowOut));
            fields.Add(CanonicalResourceIdentity.QuantityField($"member[{i:D4}].maxIn", member.MaximumBorrowIn));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].reclaimability", binding.Reclaimability.ToString()));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].reclaimEvidence", binding.Evidence.EvidenceId.Value));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].reclaimEvidenceScope", binding.Evidence.ScopeId.Value));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].reclaimEvidenceObservedAt", binding.Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].reclaimEffectiveFrom", binding.Lifetime.EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
            fields.Add(new CanonicalIdentityField($"member[{i:D4}].reclaimEffectiveUntil", binding.Lifetime.EffectiveUntil?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
        }
        return CanonicalResourceIdentity.ComputeSha256(fields);
    }

    private static string MemberKey(ResourceCoordinationEnvelopeMember member) => string.Join("|", member.ApplicationId.Value, member.GrantId.Value, member.ResourceClassId.Value);
}

public sealed record BorrowedEffectiveCapacitySegment
{
    public BorrowedEffectiveCapacitySegment(string segmentId, ApplicationPrincipalId sourceApplicationId, ResourceGrantId sourceGrantId, ApplicationPrincipalId targetApplicationId, ResourceClassId resourceClassId, ResourceQuantity quantity, string envelopeIdentitySha256, ResourceEvidenceReference appliedEffectEvidence)
    {
        SegmentId = ResourceMutationGuard.Id(segmentId, nameof(segmentId));
        SourceApplicationId = sourceApplicationId ?? throw new ArgumentNullException(nameof(sourceApplicationId));
        SourceGrantId = sourceGrantId ?? throw new ArgumentNullException(nameof(sourceGrantId));
        TargetApplicationId = targetApplicationId ?? throw new ArgumentNullException(nameof(targetApplicationId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        EnvelopeIdentitySha256 = ResourceMutationGuard.Id(envelopeIdentitySha256, nameof(envelopeIdentitySha256));
        AppliedEffectEvidence = appliedEffectEvidence ?? throw new ArgumentNullException(nameof(appliedEffectEvidence));
        if (Quantity.Amount <= 0m) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (StringComparer.Ordinal.Equals(SourceApplicationId.Value, TargetApplicationId.Value)) throw new ArgumentException("Source and target Applications must differ.");
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("segmentId", SegmentId), new CanonicalIdentityField("sourceApplication", SourceApplicationId.Value),
            new CanonicalIdentityField("sourceGrant", SourceGrantId.Value), new CanonicalIdentityField("targetApplication", TargetApplicationId.Value),
            new CanonicalIdentityField("resourceClass", ResourceClassId.Value), CanonicalResourceIdentity.QuantityField("quantity", Quantity),
            new CanonicalIdentityField("envelope", EnvelopeIdentitySha256), new CanonicalIdentityField("effectEvidenceId", AppliedEffectEvidence.EvidenceId.Value),
            new CanonicalIdentityField("effectEvidenceScope", AppliedEffectEvidence.ScopeId.Value),
            new CanonicalIdentityField("effectEvidenceObservedAt", AppliedEffectEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("effectEvidenceEpoch", AppliedEffectEvidence.EpochId.Value)
        });
    }

    public string SegmentId { get; }
    public ApplicationPrincipalId SourceApplicationId { get; }
    public ResourceGrantId SourceGrantId { get; }
    public ApplicationPrincipalId TargetApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity Quantity { get; }
    public string EnvelopeIdentitySha256 { get; }
    public ResourceEvidenceReference AppliedEffectEvidence { get; }
    public string IdentitySha256 { get; }
}

public sealed record EffectiveResourceDistributionSnapshot
{
    private readonly ReadOnlyCollection<BorrowedEffectiveCapacitySegment> _segments;

    public EffectiveResourceDistributionSnapshot(ApplicationResourceAllocationSnapshot authoritativeAllocationSnapshot, ResourceCoordinationEnvelope envelope, DateTimeOffset observedAt, IEnumerable<BorrowedEffectiveCapacitySegment> borrowedSegments)
    {
        AuthoritativeAllocationSnapshot = authoritativeAllocationSnapshot ?? throw new ArgumentNullException(nameof(authoritativeAllocationSnapshot));
        Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
        ArgumentNullException.ThrowIfNull(borrowedSegments);
        if (!StringComparer.Ordinal.Equals(Envelope.AllocationSnapshot.IdentitySha256, AuthoritativeAllocationSnapshot.IdentitySha256)) throw new ArgumentException("Envelope predecessor mismatch.");
        if (observedAt < AuthoritativeAllocationSnapshot.ObservedAt) throw new ArgumentException("Effective distribution cannot predate authoritative allocation truth.", nameof(observedAt));
        Envelope.ValidateAt(observedAt);
        var ordered = borrowedSegments.Select(item => item ?? throw new ArgumentException("Borrowed segment cannot be null.", nameof(borrowedSegments))).OrderBy(item => item.SegmentId, StringComparer.Ordinal).ToArray();
        if (ordered.Select(item => item.SegmentId).Distinct(StringComparer.Ordinal).Count() != ordered.Length) throw new ArgumentException("Duplicate borrowed segment identity.", nameof(borrowedSegments));
        foreach (var segment in ordered)
        {
            if (!StringComparer.Ordinal.Equals(segment.EnvelopeIdentitySha256, Envelope.IdentitySha256)) throw new ArgumentException("Borrowed segment envelope mismatch.", nameof(borrowedSegments));
            var source = Envelope.GetMember(segment.SourceApplicationId, segment.SourceGrantId, segment.ResourceClassId);
            var target = Envelope.GetMember(segment.TargetApplicationId, segment.ResourceClassId);
            ResourceMutationGuard.SameUnit(source.MaximumBorrowOut, segment.Quantity, "borrowed quantity");
            ResourceMutationGuard.SameUnit(target.MaximumBorrowIn, segment.Quantity, "borrowed quantity");
            ResourceMutationGuard.Evidence(segment.AppliedEffectEvidence, AuthoritativeAllocationSnapshot.ResourceTruth.EpochId, observedAt, "borrowed segment effect");
        }
        _segments = Array.AsReadOnly(ordered);
        ObservedAt = observedAt;
        ValidateBounds();
        IdentitySha256 = ComputeIdentity();
    }

    public ApplicationResourceAllocationSnapshot AuthoritativeAllocationSnapshot { get; }
    public ResourceCoordinationEnvelope Envelope { get; }
    public DateTimeOffset ObservedAt { get; }
    public IReadOnlyList<BorrowedEffectiveCapacitySegment> BorrowedSegments => _segments;
    public string IdentitySha256 { get; }

    public ResourceQuantity GetEffectiveCapacity(ApplicationPrincipalId applicationId, ResourceClassId resourceClassId)
    {
        var allocation = AuthoritativeAllocationSnapshot.GetRequiredAllocation(applicationId, resourceClassId);
        var lent = _segments.Where(item => StringComparer.Ordinal.Equals(item.SourceApplicationId.Value, applicationId.Value) && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value)).Sum(item => item.Quantity.Amount);
        var borrowed = _segments.Where(item => StringComparer.Ordinal.Equals(item.TargetApplicationId.Value, applicationId.Value) && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value)).Sum(item => item.Quantity.Amount);
        return new ResourceQuantity(allocation.Allocation.Amount - lent + borrowed, allocation.Allocation.Unit);
    }

    private void ValidateBounds()
    {
        foreach (var member in Envelope.Members)
        {
            var allocation = AuthoritativeAllocationSnapshot.GetRequiredAllocation(member.ApplicationId, member.ResourceClassId);
            var lent = _segments.Where(item => StringComparer.Ordinal.Equals(item.SourceGrantId.Value, member.GrantId.Value) && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, member.ResourceClassId.Value)).Sum(item => item.Quantity.Amount);
            var borrowed = _segments.Where(item => StringComparer.Ordinal.Equals(item.TargetApplicationId.Value, member.ApplicationId.Value) && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, member.ResourceClassId.Value)).Sum(item => item.Quantity.Amount);
            if (lent > member.MaximumBorrowOut.Amount) throw new InvalidOperationException("Borrow-out exceeds envelope.");
            if (borrowed > member.MaximumBorrowIn.Amount) throw new InvalidOperationException("Borrow-in exceeds envelope.");
            var effective = allocation.Allocation.Amount - lent + borrowed;
            if (effective < member.ProtectedEffectiveMinimum.Amount) throw new InvalidOperationException("Protected effective minimum violated.");
            if (effective > allocation.Ceiling.Amount) throw new InvalidOperationException("Authoritative ceiling violated by effective distribution.");
        }
    }

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("allocationSnapshot", AuthoritativeAllocationSnapshot.IdentitySha256), new("envelope", Envelope.IdentitySha256),
            new("observedAt", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        };
        for (var i = 0; i < _segments.Count; i++) fields.Add(new CanonicalIdentityField($"segment[{i:D4}]", _segments[i].IdentitySha256));
        return CanonicalResourceIdentity.ComputeSha256(fields);
    }
}

public sealed record EffectiveDistributionMutationIntent
{
    public EffectiveDistributionMutationIntent(string intentId, EffectiveDistributionOperationKind operationKind, ApplicationPrincipalId sourceApplicationId, ResourceGrantId sourceGrantId, ApplicationPrincipalId targetApplicationId, ResourceClassId resourceClassId, ResourceQuantity quantity, string? borrowedSegmentId, ResourceCoordinationEnvelope envelope, string coordinatorInstanceId, string coordinatorRoleId, long fenceGeneration, string fencingToken, CorrelationId correlationId, CausationId causationId, ResourceEvidenceReference intentEvidence, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        IntentId = ResourceMutationGuard.Id(intentId, nameof(intentId));
        SourceApplicationId = sourceApplicationId ?? throw new ArgumentNullException(nameof(sourceApplicationId));
        SourceGrantId = sourceGrantId ?? throw new ArgumentNullException(nameof(sourceGrantId));
        TargetApplicationId = targetApplicationId ?? throw new ArgumentNullException(nameof(targetApplicationId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
        CoordinatorInstanceId = ResourceMutationGuard.Id(coordinatorInstanceId, nameof(coordinatorInstanceId));
        CoordinatorRoleId = ResourceMutationGuard.Id(coordinatorRoleId, nameof(coordinatorRoleId));
        FencingToken = ResourceMutationGuard.Id(fencingToken, nameof(fencingToken));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        CausationId = causationId ?? throw new ArgumentNullException(nameof(causationId));
        IntentEvidence = intentEvidence ?? throw new ArgumentNullException(nameof(intentEvidence));
        if (Quantity.Amount <= 0m) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (fenceGeneration <= 0) throw new ArgumentOutOfRangeException(nameof(fenceGeneration));
        if (expiresAt <= createdAt) throw new ArgumentException("Intent must be bounded.", nameof(expiresAt));
        if (!StringComparer.Ordinal.Equals(CoordinatorInstanceId, Envelope.CoordinatorInstanceId) || !StringComparer.Ordinal.Equals(CoordinatorRoleId, Envelope.CoordinatorRoleId)) throw new ArgumentException("Coordinator identity/role mismatch.");
        if (fenceGeneration != Envelope.FenceGeneration || !StringComparer.Ordinal.Equals(FencingToken, Envelope.FencingToken)) throw new ArgumentException("Coordinator fence mismatch.");
        ResourceMutationGuard.Evidence(IntentEvidence, Envelope.AllocationSnapshot.ResourceTruth.EpochId, createdAt, "effective-distribution intent");
        Envelope.ValidateAt(createdAt);
        _ = Envelope.GetMember(SourceApplicationId, SourceGrantId, ResourceClassId);
        _ = Envelope.GetMember(TargetApplicationId, ResourceClassId);
        if (StringComparer.Ordinal.Equals(SourceApplicationId.Value, TargetApplicationId.Value)) throw new ArgumentException("Source and target Applications must differ.");
        OperationKind = operationKind;
        BorrowedSegmentId = operationKind switch
        {
            EffectiveDistributionOperationKind.Borrow when borrowedSegmentId is null => null,
            EffectiveDistributionOperationKind.Borrow => throw new ArgumentException("Borrow cannot reference an existing segment.", nameof(borrowedSegmentId)),
            EffectiveDistributionOperationKind.ReturnBorrowed when string.IsNullOrWhiteSpace(borrowedSegmentId) => throw new ArgumentException("Return requires an exact borrowed segment.", nameof(borrowedSegmentId)),
            EffectiveDistributionOperationKind.ReturnBorrowed => ResourceMutationGuard.Id(borrowedSegmentId!, nameof(borrowedSegmentId)),
            _ => throw new ArgumentOutOfRangeException(nameof(operationKind))
        };
        FenceGeneration = fenceGeneration;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("intentId", IntentId), new CanonicalIdentityField("kind", OperationKind.ToString()),
            new CanonicalIdentityField("sourceApplication", SourceApplicationId.Value), new CanonicalIdentityField("sourceGrant", SourceGrantId.Value),
            new CanonicalIdentityField("targetApplication", TargetApplicationId.Value), new CanonicalIdentityField("resourceClass", ResourceClassId.Value),
            CanonicalResourceIdentity.QuantityField("quantity", Quantity), new CanonicalIdentityField("borrowedSegment", BorrowedSegmentId),
            new CanonicalIdentityField("envelope", Envelope.IdentitySha256), new CanonicalIdentityField("coordinatorInstance", CoordinatorInstanceId),
            new CanonicalIdentityField("coordinatorRole", CoordinatorRoleId), new CanonicalIdentityField("fenceGeneration", FenceGeneration.ToString(CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("fencingToken", FencingToken), new CanonicalIdentityField("correlation", CorrelationId.Value),
            new CanonicalIdentityField("causation", CausationId.Value), new CanonicalIdentityField("evidenceId", IntentEvidence.EvidenceId.Value),
            new CanonicalIdentityField("evidenceScope", IntentEvidence.ScopeId.Value),
            new CanonicalIdentityField("evidenceObservedAt", IntentEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("evidenceEpoch", IntentEvidence.EpochId.Value),
            new CanonicalIdentityField("createdAt", CreatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }

    public string IntentId { get; }
    public EffectiveDistributionOperationKind OperationKind { get; }
    public ApplicationPrincipalId SourceApplicationId { get; }
    public ResourceGrantId SourceGrantId { get; }
    public ApplicationPrincipalId TargetApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity Quantity { get; }
    public string? BorrowedSegmentId { get; }
    public ResourceCoordinationEnvelope Envelope { get; }
    public string CoordinatorInstanceId { get; }
    public string CoordinatorRoleId { get; }
    public long FenceGeneration { get; }
    public string FencingToken { get; }
    public CorrelationId CorrelationId { get; }
    public CausationId CausationId { get; }
    public ResourceEvidenceReference IntentEvidence { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }
}

public sealed record ResourceEffectOperation
{
    private ResourceEffectOperation(string operationId, ResourceEffectLane lane, string intentIdentitySha256, EffectiveDistributionOperationKind? effectiveOperationKind, ResourceDecisionKind? foundationOperationKind, ApplicationPrincipalId? sourceApplicationId, ResourceGrantId? sourceGrantId, ApplicationPrincipalId? targetApplicationId, ResourceClassId resourceClassId, ResourceQuantity primaryQuantity, ResourceQuantity? targetQuota, ResourceQuantity? targetCeiling, string? borrowedSegmentId)
    {
        OperationId = ResourceMutationGuard.Id(operationId, nameof(operationId));
        Lane = lane;
        IntentIdentitySha256 = ResourceMutationGuard.Id(intentIdentitySha256, nameof(intentIdentitySha256));
        EffectiveOperationKind = effectiveOperationKind;
        FoundationOperationKind = foundationOperationKind;
        SourceApplicationId = sourceApplicationId;
        SourceGrantId = sourceGrantId;
        TargetApplicationId = targetApplicationId;
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        PrimaryQuantity = primaryQuantity ?? throw new ArgumentNullException(nameof(primaryQuantity));
        TargetQuota = targetQuota;
        TargetCeiling = targetCeiling;
        BorrowedSegmentId = borrowedSegmentId;
        if (lane == ResourceEffectLane.DelegatedEffectiveDistribution && (!effectiveOperationKind.HasValue || foundationOperationKind.HasValue || sourceApplicationId is null || sourceGrantId is null || targetApplicationId is null || targetQuota is not null || targetCeiling is not null)) throw new ArgumentException("Invalid delegated effect payload.");
        if (lane == ResourceEffectLane.FoundationAuthoritativeAllocation && (!foundationOperationKind.HasValue || effectiveOperationKind.HasValue || sourceApplicationId is null || sourceGrantId is null || targetApplicationId is not null || targetQuota is null || targetCeiling is null || borrowedSegmentId is not null)) throw new ArgumentException("Invalid Foundation effect payload.");
        if (effectiveOperationKind == EffectiveDistributionOperationKind.ReturnBorrowed && string.IsNullOrWhiteSpace(borrowedSegmentId)) throw new ArgumentException("Return effect requires borrowed segment identity.");
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("operationId", OperationId), new CanonicalIdentityField("lane", Lane.ToString()),
            new CanonicalIdentityField("intent", IntentIdentitySha256), new CanonicalIdentityField("effectiveKind", EffectiveOperationKind?.ToString()),
            new CanonicalIdentityField("foundationKind", FoundationOperationKind?.ToString()), new CanonicalIdentityField("sourceApplication", SourceApplicationId?.Value),
            new CanonicalIdentityField("sourceGrant", SourceGrantId?.Value), new CanonicalIdentityField("targetApplication", TargetApplicationId?.Value),
            new CanonicalIdentityField("resourceClass", ResourceClassId.Value), CanonicalResourceIdentity.QuantityField("primaryQuantity", PrimaryQuantity),
            new CanonicalIdentityField("targetQuota", TargetQuota?.ToCanonicalString()), new CanonicalIdentityField("targetCeiling", TargetCeiling?.ToCanonicalString()),
            new CanonicalIdentityField("borrowedSegmentId", BorrowedSegmentId)
        });
    }

    public static ResourceEffectOperation ForEffective(EffectiveDistributionMutationIntent intent)
        => new(intent.IntentId, ResourceEffectLane.DelegatedEffectiveDistribution, intent.IdentitySha256, intent.OperationKind, null,
            intent.SourceApplicationId, intent.SourceGrantId, intent.TargetApplicationId, intent.ResourceClassId, intent.Quantity, null, null, intent.BorrowedSegmentId);

    public static ResourceEffectOperation ForFoundation(FoundationAllocationMutationIntent intent)
        => new(intent.IntentId, ResourceEffectLane.FoundationAuthoritativeAllocation, intent.IdentitySha256, null, intent.Operation,
            intent.ApplicationId, intent.GrantId, null, intent.ResourceClassId, intent.TargetAllocation, intent.TargetQuota, intent.TargetCeiling, null);

    public string OperationId { get; }
    public ResourceEffectLane Lane { get; }
    public string IntentIdentitySha256 { get; }
    public EffectiveDistributionOperationKind? EffectiveOperationKind { get; }
    public ResourceDecisionKind? FoundationOperationKind { get; }
    public ApplicationPrincipalId? SourceApplicationId { get; }
    public ResourceGrantId? SourceGrantId { get; }
    public ApplicationPrincipalId? TargetApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity PrimaryQuantity { get; }
    public ResourceQuantity? TargetQuota { get; }
    public ResourceQuantity? TargetCeiling { get; }
    public string? BorrowedSegmentId { get; }
    public string IdentitySha256 { get; }
}

public sealed record ResourceEffectBatch
{
    private readonly ReadOnlyCollection<ResourceEffectOperation> _operations;

    public ResourceEffectBatch(string batchId, IEnumerable<ResourceEffectOperation> operations)
    {
        BatchId = ResourceMutationGuard.Id(batchId, nameof(batchId));
        ArgumentNullException.ThrowIfNull(operations);
        var ordered = operations.Select(item => item ?? throw new ArgumentException("Effect operation cannot be null.", nameof(operations))).OrderBy(item => item.OperationId, StringComparer.Ordinal).ToArray();
        if (ordered.Length == 0) throw new ArgumentException("Effect batch cannot be empty.", nameof(operations));
        if (ordered.Select(item => item.OperationId).Distinct(StringComparer.Ordinal).Count() != ordered.Length) throw new ArgumentException("Duplicate effect operation identity.", nameof(operations));
        _operations = Array.AsReadOnly(ordered);
        var fields = new List<CanonicalIdentityField> { new("batchId", BatchId) };
        for (var i = 0; i < _operations.Count; i++) fields.Add(new CanonicalIdentityField($"operation[{i:D4}]", _operations[i].IdentitySha256));
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(fields);
    }

    public string BatchId { get; }
    public IReadOnlyList<ResourceEffectOperation> Operations => _operations;
    public string IdentitySha256 { get; }
}

public sealed record ResourceEffectApplicationResult
{
    private readonly ReadOnlyCollection<string> _applied;

    public ResourceEffectApplicationResult(string batchIdentitySha256, bool success, bool partialEffectObserved, IEnumerable<string> appliedOperationIds, ResourceEvidenceReference evidence, DateTimeOffset observedAt)
    {
        BatchIdentitySha256 = ResourceMutationGuard.Id(batchIdentitySha256, nameof(batchIdentitySha256));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        ArgumentNullException.ThrowIfNull(appliedOperationIds);
        var ordered = appliedOperationIds.Select(value => ResourceMutationGuard.Id(value, nameof(appliedOperationIds))).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        if (ordered.Distinct(StringComparer.Ordinal).Count() != ordered.Length) throw new ArgumentException("Duplicate applied operation identity.", nameof(appliedOperationIds));
        if (success && partialEffectObserved) throw new ArgumentException("Atomic success cannot report a partial effect.");
        Success = success;
        PartialEffectObserved = partialEffectObserved;
        _applied = Array.AsReadOnly(ordered);
        ObservedAt = observedAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("batch", BatchIdentitySha256), new CanonicalIdentityField("success", Success ? "1" : "0"),
            new CanonicalIdentityField("partial", PartialEffectObserved ? "1" : "0"), new CanonicalIdentityField("evidenceId", Evidence.EvidenceId.Value),
            new CanonicalIdentityField("evidenceScope", Evidence.ScopeId.Value), new CanonicalIdentityField("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("evidenceEpoch", Evidence.EpochId.Value), new CanonicalIdentityField("observedAt", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("appliedSet", string.Join("|", _applied))
        });
    }

    public string BatchIdentitySha256 { get; }
    public bool Success { get; }
    public bool PartialEffectObserved { get; }
    public IReadOnlyList<string> AppliedOperationIds => _applied;
    public ResourceEvidenceReference Evidence { get; }
    public DateTimeOffset ObservedAt { get; }
    public string IdentitySha256 { get; }
}

public interface IResourceEffectAdapter
{
    ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt);
}

public sealed record FoundationResourceMutationAuthority
{
    private readonly ReadOnlyCollection<ApplicationPrincipalId> _applications;
    private readonly ReadOnlyCollection<ResourceClassId> _resourceClasses;
    private readonly ReadOnlyCollection<ResourceDecisionKind> _operations;

    public FoundationResourceMutationAuthority(string authorityId, ResourceScopeId scopeId, IEnumerable<ApplicationPrincipalId> authorizedApplications, IEnumerable<ResourceClassId> authorizedResourceClasses, IEnumerable<ResourceDecisionKind> allowedOperations, long generation, ResourceEvidenceReference evidence, DateTimeOffset effectiveFrom, DateTimeOffset expiresAt)
    {
        AuthorityId = ResourceMutationGuard.Id(authorityId, nameof(authorityId));
        ScopeId = scopeId ?? throw new ArgumentNullException(nameof(scopeId));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        ArgumentNullException.ThrowIfNull(authorizedApplications);
        ArgumentNullException.ThrowIfNull(authorizedResourceClasses);
        ArgumentNullException.ThrowIfNull(allowedOperations);
        if (generation <= 0) throw new ArgumentOutOfRangeException(nameof(generation));
        if (expiresAt <= effectiveFrom) throw new ArgumentException("Foundation mutation authority must be bounded.", nameof(expiresAt));
        var apps = authorizedApplications.Select(item => item ?? throw new ArgumentException("Authorized Application cannot be null.", nameof(authorizedApplications))).OrderBy(item => item.Value, StringComparer.Ordinal).ToArray();
        var classes = authorizedResourceClasses.Select(item => item ?? throw new ArgumentException("Authorized resource class cannot be null.", nameof(authorizedResourceClasses))).OrderBy(item => item.Value, StringComparer.Ordinal).ToArray();
        var operations = allowedOperations.OrderBy(item => (int)item).ToArray();
        if (apps.Length == 0 || classes.Length == 0 || operations.Length == 0) throw new ArgumentException("Foundation mutation authority requires a non-empty bounded scope.");
        if (apps.Select(item => item.Value).Distinct(StringComparer.Ordinal).Count() != apps.Length) throw new ArgumentException("Duplicate authorized Application.");
        if (classes.Select(item => item.Value).Distinct(StringComparer.Ordinal).Count() != classes.Length) throw new ArgumentException("Duplicate authorized resource class.");
        if (operations.Distinct().Count() != operations.Length) throw new ArgumentException("Duplicate authorized operation.");
        if (operations.Any(item => item != ResourceDecisionKind.Reduce && item != ResourceDecisionKind.Revoke && item != ResourceDecisionKind.Restore)) throw new ArgumentException("WP-07 authority may contain only Reduce, Revoke and Restore.");
        _applications = Array.AsReadOnly(apps);
        _resourceClasses = Array.AsReadOnly(classes);
        _operations = Array.AsReadOnly(operations);
        Generation = generation;
        EffectiveFrom = effectiveFrom;
        ExpiresAt = expiresAt;
        var fields = new List<CanonicalIdentityField>
        {
            new("authorityId", AuthorityId), new("scope", ScopeId.Value), new("generation", Generation.ToString(CultureInfo.InvariantCulture)),
            new("evidenceId", Evidence.EvidenceId.Value), new("evidenceScope", Evidence.ScopeId.Value),
            new("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("evidenceEpoch", Evidence.EpochId.Value),
            new("effectiveFrom", EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        };
        for (var i = 0; i < apps.Length; i++) fields.Add(new CanonicalIdentityField($"application[{i:D4}]", apps[i].Value));
        for (var i = 0; i < classes.Length; i++) fields.Add(new CanonicalIdentityField($"resourceClass[{i:D4}]", classes[i].Value));
        for (var i = 0; i < operations.Length; i++) fields.Add(new CanonicalIdentityField($"operation[{i:D4}]", operations[i].ToString()));
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(fields);
    }

    public string AuthorityId { get; }
    public ResourceScopeId ScopeId { get; }
    public IReadOnlyList<ApplicationPrincipalId> AuthorizedApplications => _applications;
    public IReadOnlyList<ResourceClassId> AuthorizedResourceClasses => _resourceClasses;
    public IReadOnlyList<ResourceDecisionKind> AllowedOperations => _operations;
    public long Generation { get; }
    public ResourceEvidenceReference Evidence { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }

    public void Validate(ApplicationPrincipalId applicationId, ResourceClassId resourceClassId, ResourceDecisionKind operation, ResourceEpochId epoch, DateTimeOffset at)
    {
        if (at < EffectiveFrom || at >= ExpiresAt) throw new InvalidOperationException("Foundation mutation authority is not effective.");
        if (!_applications.Any(item => StringComparer.Ordinal.Equals(item.Value, applicationId.Value))) throw new InvalidOperationException("Application is outside mutation authority scope.");
        if (!_resourceClasses.Any(item => StringComparer.Ordinal.Equals(item.Value, resourceClassId.Value))) throw new InvalidOperationException("Resource class is outside mutation authority scope.");
        if (!_operations.Contains(operation)) throw new InvalidOperationException("Operation is outside mutation authority scope.");
        ResourceMutationGuard.Evidence(Evidence, epoch, at, "Foundation mutation authority");
    }
}

public sealed record FoundationAllocationRestorationBasis
{
    private FoundationAllocationRestorationBasis(string basisId, ApplicationResourceAllocationSnapshot sourceSnapshot, ApplicationResourceAllocation allocation, ResourceEvidenceReference evidence)
    {
        BasisId = ResourceMutationGuard.Id(basisId, nameof(basisId));
        SourceAllocationSnapshotIdentitySha256 = sourceSnapshot.IdentitySha256;
        ApplicationId = allocation.ApplicationId;
        GrantId = allocation.GrantId;
        ResourceClassId = allocation.ResourceClassId;
        MaximumRestorableAllocation = allocation.Allocation;
        MaximumRestorableQuota = allocation.Quota;
        MaximumRestorableCeiling = allocation.Ceiling;
        Evidence = evidence;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("basisId", BasisId), new CanonicalIdentityField("sourceSnapshot", SourceAllocationSnapshotIdentitySha256),
            new CanonicalIdentityField("application", ApplicationId.Value), new CanonicalIdentityField("grant", GrantId.Value),
            new CanonicalIdentityField("resourceClass", ResourceClassId.Value), CanonicalResourceIdentity.QuantityField("maxAllocation", MaximumRestorableAllocation),
            CanonicalResourceIdentity.QuantityField("maxQuota", MaximumRestorableQuota), CanonicalResourceIdentity.QuantityField("maxCeiling", MaximumRestorableCeiling),
            new CanonicalIdentityField("evidenceId", Evidence.EvidenceId.Value), new CanonicalIdentityField("evidenceScope", Evidence.ScopeId.Value),
            new CanonicalIdentityField("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new CanonicalIdentityField("evidenceEpoch", Evidence.EpochId.Value)
        });
    }

    public static FoundationAllocationRestorationBasis Capture(string basisId, ApplicationResourceAllocationSnapshot sourceSnapshot, ApplicationPrincipalId applicationId, ResourceClassId resourceClassId, ResourceEvidenceReference evidence)
    {
        ArgumentNullException.ThrowIfNull(sourceSnapshot);
        ArgumentNullException.ThrowIfNull(applicationId);
        ArgumentNullException.ThrowIfNull(resourceClassId);
        ArgumentNullException.ThrowIfNull(evidence);
        ResourceMutationGuard.Evidence(evidence, sourceSnapshot.ResourceTruth.EpochId, sourceSnapshot.ObservedAt, "restoration-basis capture");
        var allocation = sourceSnapshot.GetRequiredAllocation(applicationId, resourceClassId);
        return new FoundationAllocationRestorationBasis(basisId, sourceSnapshot, allocation, evidence);
    }

    public string BasisId { get; }
    public string SourceAllocationSnapshotIdentitySha256 { get; }
    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceGrantId GrantId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity MaximumRestorableAllocation { get; }
    public ResourceQuantity MaximumRestorableQuota { get; }
    public ResourceQuantity MaximumRestorableCeiling { get; }
    public ResourceEvidenceReference Evidence { get; }
    public string IdentitySha256 { get; }
}

public sealed record FoundationAllocationMutationIntent
{
    public FoundationAllocationMutationIntent(string intentId, ResourceDecisionKind operation, ApplicationPrincipalId applicationId, ResourceGrantId grantId, ResourceClassId resourceClassId, ResourceQuantity targetAllocation, ResourceQuantity targetQuota, ResourceQuantity targetCeiling, FoundationResourceMutationAuthority authority, FoundationAllocationRestorationBasis? restorationBasis, string predecessorAllocationSnapshotIdentitySha256, CorrelationId correlationId, CausationId causationId, ResourceEvidenceReference intentEvidence, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        IntentId = ResourceMutationGuard.Id(intentId, nameof(intentId));
        if (operation != ResourceDecisionKind.Reduce && operation != ResourceDecisionKind.Revoke && operation != ResourceDecisionKind.Restore) throw new ArgumentException("WP-07 supports only Reduce, Revoke or Restore.", nameof(operation));
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        GrantId = grantId ?? throw new ArgumentNullException(nameof(grantId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        TargetAllocation = targetAllocation ?? throw new ArgumentNullException(nameof(targetAllocation));
        TargetQuota = targetQuota ?? throw new ArgumentNullException(nameof(targetQuota));
        TargetCeiling = targetCeiling ?? throw new ArgumentNullException(nameof(targetCeiling));
        Authority = authority ?? throw new ArgumentNullException(nameof(authority));
        PredecessorAllocationSnapshotIdentitySha256 = ResourceMutationGuard.Id(predecessorAllocationSnapshotIdentitySha256, nameof(predecessorAllocationSnapshotIdentitySha256));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        CausationId = causationId ?? throw new ArgumentNullException(nameof(causationId));
        IntentEvidence = intentEvidence ?? throw new ArgumentNullException(nameof(intentEvidence));
        if (expiresAt <= createdAt) throw new ArgumentException("Mutation intent must be bounded.", nameof(expiresAt));
        ResourceMutationGuard.SameUnit(TargetAllocation, TargetQuota, "target quota");
        ResourceMutationGuard.SameUnit(TargetAllocation, TargetCeiling, "target ceiling");
        if (TargetAllocation.Amount > TargetQuota.Amount || TargetQuota.Amount > TargetCeiling.Amount) throw new ArgumentException("Mutation target must preserve allocation <= quota <= ceiling.");
        if (operation == ResourceDecisionKind.Revoke && (TargetAllocation.Amount != 0m || TargetQuota.Amount != 0m || TargetCeiling.Amount != 0m)) throw new ArgumentException("Revoke target must be zero.");
        if (operation == ResourceDecisionKind.Restore && restorationBasis is null) throw new ArgumentNullException(nameof(restorationBasis), "Restore requires exact restoration basis.");
        if (operation != ResourceDecisionKind.Restore && restorationBasis is not null) throw new ArgumentException("Restoration basis is valid only for Restore.", nameof(restorationBasis));
        RestorationBasis = restorationBasis;
        Operation = operation;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("intentId", IntentId), new CanonicalIdentityField("operation", Operation.ToString()),
            new CanonicalIdentityField("application", ApplicationId.Value), new CanonicalIdentityField("grant", GrantId.Value),
            new CanonicalIdentityField("resourceClass", ResourceClassId.Value), CanonicalResourceIdentity.QuantityField("targetAllocation", TargetAllocation),
            CanonicalResourceIdentity.QuantityField("targetQuota", TargetQuota), CanonicalResourceIdentity.QuantityField("targetCeiling", TargetCeiling),
            new CanonicalIdentityField("authority", Authority.IdentitySha256), new CanonicalIdentityField("restorationBasis", RestorationBasis?.IdentitySha256),
            new CanonicalIdentityField("predecessor", PredecessorAllocationSnapshotIdentitySha256), new CanonicalIdentityField("correlation", CorrelationId.Value),
            new CanonicalIdentityField("causation", CausationId.Value), new CanonicalIdentityField("evidenceId", IntentEvidence.EvidenceId.Value),
            new CanonicalIdentityField("evidenceScope", IntentEvidence.ScopeId.Value),
            new CanonicalIdentityField("evidenceObservedAt", IntentEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("evidenceEpoch", IntentEvidence.EpochId.Value),
            new CanonicalIdentityField("createdAt", CreatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("expiresAt", ExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }

    public string IntentId { get; }
    public ResourceDecisionKind Operation { get; }
    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceGrantId GrantId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity TargetAllocation { get; }
    public ResourceQuantity TargetQuota { get; }
    public ResourceQuantity TargetCeiling { get; }
    public FoundationResourceMutationAuthority Authority { get; }
    public FoundationAllocationRestorationBasis? RestorationBasis { get; }
    public string PredecessorAllocationSnapshotIdentitySha256 { get; }
    public CorrelationId CorrelationId { get; }
    public CausationId CausationId { get; }
    public ResourceEvidenceReference IntentEvidence { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset ExpiresAt { get; }
    public string IdentitySha256 { get; }
}

public sealed record AcceptedEffectiveDistributionMutation(ResourceEffectApplicationResult AppliedEffect, EffectiveResourceDistributionSnapshot AcceptedSnapshot);
public sealed record AcceptedFoundationAllocationMutation(ResourceEffectApplicationResult AppliedEffect, ApplicationResourceAllocationSnapshot AcceptedSnapshot);

public sealed class ResourceMutationProcessor
{
    private sealed record EnvelopeState(long AuthorityGeneration, long FenceGeneration, string CoordinatorInstanceId, string FencingToken, string IdentitySha256);
    private sealed record FoundationAuthorityState(long Generation, string IdentitySha256);
    private readonly HashSet<string> _intentIds = new(StringComparer.Ordinal);
    private readonly HashSet<string> _batchIds = new(StringComparer.Ordinal);
    private readonly Dictionary<string, EnvelopeState> _envelopes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, FoundationAuthorityState> _authorities = new(StringComparer.Ordinal);
    private readonly HashSet<string> _quiescedAllocationSnapshots = new(StringComparer.Ordinal);

    public AcceptedEffectiveDistributionMutation ApplyEffectiveRedistribution(EffectiveResourceDistributionSnapshot current, string batchId, IEnumerable<EffectiveDistributionMutationIntent> intents, IResourceEffectAdapter effectAdapter, DateTimeOffset appliedAt)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(intents);
        ArgumentNullException.ThrowIfNull(effectAdapter);
        var batch = ResourceMutationGuard.Id(batchId, nameof(batchId));
        if (!_batchIds.Add(batch)) throw new InvalidOperationException("Duplicate/replayed mutation batch rejected.");
        var ordered = intents.Select(item => item ?? throw new ArgumentException("Intent cannot be null.", nameof(intents))).OrderBy(item => item.IntentId, StringComparer.Ordinal).ToArray();
        if (ordered.Length == 0) throw new ArgumentException("Effective redistribution requires at least one intent.", nameof(intents));
        ValidateEnvelope(current.Envelope);
        var next = current.BorrowedSegments.ToDictionary(item => item.SegmentId, StringComparer.Ordinal);

        foreach (var intent in ordered)
        {
            if (!_intentIds.Add(intent.IntentId)) throw new InvalidOperationException("Duplicate/replayed mutation intent rejected.");
            if (!StringComparer.Ordinal.Equals(intent.Envelope.IdentitySha256, current.Envelope.IdentitySha256)) throw new InvalidOperationException("Intent envelope mismatch.");
            if (appliedAt < intent.CreatedAt || appliedAt >= intent.ExpiresAt) throw new InvalidOperationException("Intent is not effective at apply time.");
            intent.Envelope.ValidateAt(appliedAt);
            ResourceMutationGuard.Evidence(intent.IntentEvidence, current.AuthoritativeAllocationSnapshot.ResourceTruth.EpochId, appliedAt, "effective intent");

            if (intent.OperationKind == EffectiveDistributionOperationKind.Borrow)
            {
                var sourceMember = current.Envelope.GetMember(intent.SourceApplicationId, intent.SourceGrantId, intent.ResourceClassId);
                if (sourceMember.ReclaimabilityBinding.Reclaimability == ResourceReclaimability.NonReclaimable) throw new InvalidOperationException("Borrow from non-reclaimable source is forbidden.");
                var segmentId = $"segment.{intent.IntentId}";
                if (next.ContainsKey(segmentId)) throw new InvalidOperationException("Duplicate borrowed segment.");
                next[segmentId] = new BorrowedEffectiveCapacitySegment(segmentId, intent.SourceApplicationId, intent.SourceGrantId, intent.TargetApplicationId, intent.ResourceClassId, intent.Quantity, current.Envelope.IdentitySha256, intent.IntentEvidence);
            }
            else
            {
                if (!next.TryGetValue(intent.BorrowedSegmentId!, out var existing)) throw new KeyNotFoundException("Return references an unknown borrowed segment.");
                if (!StringComparer.Ordinal.Equals(existing.SourceApplicationId.Value, intent.SourceApplicationId.Value) || !StringComparer.Ordinal.Equals(existing.SourceGrantId.Value, intent.SourceGrantId.Value) || !StringComparer.Ordinal.Equals(existing.TargetApplicationId.Value, intent.TargetApplicationId.Value) || !StringComparer.Ordinal.Equals(existing.ResourceClassId.Value, intent.ResourceClassId.Value)) throw new InvalidOperationException("Return provenance mismatch.");
                ResourceMutationGuard.SameUnit(existing.Quantity, intent.Quantity, "return quantity");
                if (intent.Quantity.Amount > existing.Quantity.Amount) throw new InvalidOperationException("Return exceeds borrowed segment.");
                if (intent.Quantity.Amount == existing.Quantity.Amount) next.Remove(existing.SegmentId);
                else next[existing.SegmentId] = new BorrowedEffectiveCapacitySegment(existing.SegmentId, existing.SourceApplicationId, existing.SourceGrantId, existing.TargetApplicationId, existing.ResourceClassId, new ResourceQuantity(existing.Quantity.Amount - intent.Quantity.Amount, existing.Quantity.Unit), existing.EnvelopeIdentitySha256, intent.IntentEvidence);
            }
        }

        _ = new EffectiveResourceDistributionSnapshot(current.AuthoritativeAllocationSnapshot, current.Envelope, appliedAt, next.Values);
        var effectBatch = new ResourceEffectBatch(batch, ordered.Select(ResourceEffectOperation.ForEffective));
        var effect = effectAdapter.Apply(effectBatch, appliedAt) ?? throw new InvalidOperationException("Effect adapter returned no result.");
        ValidateEffect(effectBatch, effect, current.AuthoritativeAllocationSnapshot.ResourceTruth.EpochId, appliedAt);
        var acceptedSegments = next.Values.Select(segment => new BorrowedEffectiveCapacitySegment(segment.SegmentId, segment.SourceApplicationId, segment.SourceGrantId, segment.TargetApplicationId, segment.ResourceClassId, segment.Quantity, segment.EnvelopeIdentitySha256, effect.Evidence)).ToArray();
        var accepted = new EffectiveResourceDistributionSnapshot(current.AuthoritativeAllocationSnapshot, current.Envelope, appliedAt, acceptedSegments);
        if (accepted.BorrowedSegments.Count == 0) _quiescedAllocationSnapshots.Add(accepted.AuthoritativeAllocationSnapshot.IdentitySha256);
        else _quiescedAllocationSnapshots.Remove(accepted.AuthoritativeAllocationSnapshot.IdentitySha256);
        return new AcceptedEffectiveDistributionMutation(effect, accepted);
    }

    public AcceptedFoundationAllocationMutation ApplyFoundationAllocationMutations(ApplicationResourceAllocationSnapshot current, string batchId, IEnumerable<FoundationAllocationMutationIntent> intents, IResourceEffectAdapter effectAdapter, DateTimeOffset appliedAt, EffectiveResourceDistributionSnapshot? currentEffectiveDistribution = null)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(intents);
        ArgumentNullException.ThrowIfNull(effectAdapter);
        EnsureQuiesced(current, currentEffectiveDistribution);
        var batch = ResourceMutationGuard.Id(batchId, nameof(batchId));
        if (!_batchIds.Add(batch)) throw new InvalidOperationException("Duplicate/replayed mutation batch rejected.");
        var ordered = intents.Select(item => item ?? throw new ArgumentException("Intent cannot be null.", nameof(intents))).OrderBy(item => item.IntentId, StringComparer.Ordinal).ToArray();
        if (ordered.Length == 0) throw new ArgumentException("Foundation mutation requires at least one intent.", nameof(intents));
        var intended = current.Allocations.ToDictionary(item => ResourceMutationGuard.AllocationKey(item.ApplicationId, item.ResourceClassId), StringComparer.Ordinal);
        foreach (var intent in ordered)
        {
            if (!_intentIds.Add(intent.IntentId)) throw new InvalidOperationException("Duplicate/replayed mutation intent rejected.");
            ValidateFoundationIntent(current, intent, appliedAt);
            ValidateFoundationAuthority(intent.Authority);
            ApplyFoundationIntent(intended, intent, appliedAt, intent.IntentEvidence);
        }
        _ = new ApplicationResourceAllocationSnapshot(current.ResourceTruth, appliedAt, intended.Values, true);
        var effectBatch = new ResourceEffectBatch(batch, ordered.Select(ResourceEffectOperation.ForFoundation));
        var effect = effectAdapter.Apply(effectBatch, appliedAt) ?? throw new InvalidOperationException("Effect adapter returned no result.");
        ValidateEffect(effectBatch, effect, current.ResourceTruth.EpochId, appliedAt);
        var accepted = current.Allocations.ToDictionary(item => ResourceMutationGuard.AllocationKey(item.ApplicationId, item.ResourceClassId), StringComparer.Ordinal);
        foreach (var intent in ordered) ApplyFoundationIntent(accepted, intent, appliedAt, effect.Evidence);
        var acceptedSnapshot = new ApplicationResourceAllocationSnapshot(current.ResourceTruth, appliedAt, accepted.Values, true);
        _quiescedAllocationSnapshots.Add(acceptedSnapshot.IdentitySha256);
        return new AcceptedFoundationAllocationMutation(effect, acceptedSnapshot);
    }

    private void EnsureQuiesced(ApplicationResourceAllocationSnapshot current, EffectiveResourceDistributionSnapshot? effective)
    {
        if (_quiescedAllocationSnapshots.Contains(current.IdentitySha256)) return;
        if (effective is null) throw new InvalidOperationException("Foundation authoritative mutation requires exact current effective-distribution quiescence evidence.");
        if (!StringComparer.Ordinal.Equals(effective.AuthoritativeAllocationSnapshot.IdentitySha256, current.IdentitySha256)) throw new InvalidOperationException("Effective-distribution quiescence predecessor mismatch.");
        if (effective.BorrowedSegments.Count != 0) throw new InvalidOperationException("Foundation authoritative mutation is blocked while borrowed effective capacity is active.");
        _quiescedAllocationSnapshots.Add(current.IdentitySha256);
    }

    private void ValidateEnvelope(ResourceCoordinationEnvelope envelope)
    {
        var key = envelope.ScopeId.Value;
        if (_envelopes.TryGetValue(key, out var existing))
        {
            if (envelope.AuthorityGeneration < existing.AuthorityGeneration) throw new InvalidOperationException("Superseded coordination-envelope authority rejected.");
            if (envelope.AuthorityGeneration == existing.AuthorityGeneration && !StringComparer.Ordinal.Equals(envelope.IdentitySha256, existing.IdentitySha256)) throw new InvalidOperationException("Conflicting coordination envelope at the same authority generation rejected.");
            if (envelope.FenceGeneration < existing.FenceGeneration) throw new InvalidOperationException("Stale coordinator fence rejected.");
            if (envelope.FenceGeneration == existing.FenceGeneration && (!StringComparer.Ordinal.Equals(envelope.CoordinatorInstanceId, existing.CoordinatorInstanceId) || !StringComparer.Ordinal.Equals(envelope.FencingToken, existing.FencingToken))) throw new InvalidOperationException("Split-brain coordinator state rejected.");
        }
        _envelopes[key] = new EnvelopeState(envelope.AuthorityGeneration, envelope.FenceGeneration, envelope.CoordinatorInstanceId, envelope.FencingToken, envelope.IdentitySha256);
    }

    private void ValidateFoundationAuthority(FoundationResourceMutationAuthority authority)
    {
        var key = authority.ScopeId.Value;
        if (_authorities.TryGetValue(key, out var existing))
        {
            if (authority.Generation < existing.Generation) throw new InvalidOperationException("Superseded Foundation mutation authority rejected.");
            if (authority.Generation == existing.Generation && !StringComparer.Ordinal.Equals(authority.IdentitySha256, existing.IdentitySha256)) throw new InvalidOperationException("Conflicting Foundation mutation authority at the same generation rejected.");
        }
        _authorities[key] = new FoundationAuthorityState(authority.Generation, authority.IdentitySha256);
    }

    private static void ValidateFoundationIntent(ApplicationResourceAllocationSnapshot current, FoundationAllocationMutationIntent intent, DateTimeOffset appliedAt)
    {
        if (!StringComparer.Ordinal.Equals(intent.PredecessorAllocationSnapshotIdentitySha256, current.IdentitySha256)) throw new InvalidOperationException("Mutation predecessor snapshot mismatch.");
        if (appliedAt < intent.CreatedAt || appliedAt >= intent.ExpiresAt) throw new InvalidOperationException("Mutation intent is not effective at apply time.");
        intent.Authority.Validate(intent.ApplicationId, intent.ResourceClassId, intent.Operation, current.ResourceTruth.EpochId, appliedAt);
        ResourceMutationGuard.Evidence(intent.IntentEvidence, current.ResourceTruth.EpochId, appliedAt, "mutation intent");
        var truth = current.ResourceTruth.GetRequired(intent.ResourceClassId);
        ResourceMutationGuard.SameUnit(truth.AllocatableCapacity, intent.TargetAllocation, "target allocation");
        ResourceMutationGuard.SameUnit(truth.AllocatableCapacity, intent.TargetQuota, "target quota");
        ResourceMutationGuard.SameUnit(truth.AllocatableCapacity, intent.TargetCeiling, "target ceiling");
        if (intent.RestorationBasis is not null)
        {
            var basis = intent.RestorationBasis;
            if (!StringComparer.Ordinal.Equals(basis.ApplicationId.Value, intent.ApplicationId.Value) || !StringComparer.Ordinal.Equals(basis.GrantId.Value, intent.GrantId.Value) || !StringComparer.Ordinal.Equals(basis.ResourceClassId.Value, intent.ResourceClassId.Value)) throw new InvalidOperationException("Restoration basis identity/scope mismatch.");
            ResourceMutationGuard.Evidence(basis.Evidence, current.ResourceTruth.EpochId, appliedAt, "restoration basis");
            ResourceMutationGuard.SameUnit(intent.TargetAllocation, basis.MaximumRestorableAllocation, "restoration basis allocation");
            if (intent.TargetAllocation.Amount > basis.MaximumRestorableAllocation.Amount || intent.TargetQuota.Amount > basis.MaximumRestorableQuota.Amount || intent.TargetCeiling.Amount > basis.MaximumRestorableCeiling.Amount) throw new InvalidOperationException("Restore exceeds exact historical restoration basis.");
        }
    }

    private static void ApplyFoundationIntent(IDictionary<string, ApplicationResourceAllocation> working, FoundationAllocationMutationIntent intent, DateTimeOffset appliedAt, ResourceEvidenceReference appliedEvidence)
    {
        var key = ResourceMutationGuard.AllocationKey(intent.ApplicationId, intent.ResourceClassId);
        working.TryGetValue(key, out var current);
        if (intent.Operation == ResourceDecisionKind.Reduce)
        {
            if (current is null) throw new InvalidOperationException("Reduce requires current authoritative allocation.");
            RequireGrant(current, intent.GrantId);
            if (intent.TargetAllocation.Amount > current.Allocation.Amount || intent.TargetQuota.Amount > current.Quota.Amount || intent.TargetCeiling.Amount > current.Ceiling.Amount) throw new InvalidOperationException("Reduce cannot increase authoritative values.");
            if (intent.TargetAllocation.Amount == current.Allocation.Amount && intent.TargetQuota.Amount == current.Quota.Amount && intent.TargetCeiling.Amount == current.Ceiling.Amount) throw new InvalidOperationException("Reduce must materially change authoritative truth.");
            working[key] = BuildAllocation(intent, appliedAt, appliedEvidence);
            return;
        }
        if (intent.Operation == ResourceDecisionKind.Revoke)
        {
            if (current is null) throw new InvalidOperationException("Revoke requires current authoritative allocation.");
            RequireGrant(current, intent.GrantId);
            working.Remove(key);
            return;
        }
        if (intent.Operation == ResourceDecisionKind.Restore)
        {
            var basis = intent.RestorationBasis ?? throw new InvalidOperationException("Restore requires exact restoration basis.");
            if (current is not null)
            {
                RequireGrant(current, intent.GrantId);
                if (intent.TargetAllocation.Amount < current.Allocation.Amount || intent.TargetQuota.Amount < current.Quota.Amount || intent.TargetCeiling.Amount < current.Ceiling.Amount) throw new InvalidOperationException("Restore cannot reduce current authoritative truth.");
                if (intent.TargetAllocation.Amount == current.Allocation.Amount && intent.TargetQuota.Amount == current.Quota.Amount && intent.TargetCeiling.Amount == current.Ceiling.Amount) throw new InvalidOperationException("Restore must materially restore authoritative truth.");
            }
            else if (intent.TargetAllocation.Amount <= 0m) throw new InvalidOperationException("Restore of revoked allocation requires positive target allocation.");
            if (intent.TargetAllocation.Amount > basis.MaximumRestorableAllocation.Amount || intent.TargetQuota.Amount > basis.MaximumRestorableQuota.Amount || intent.TargetCeiling.Amount > basis.MaximumRestorableCeiling.Amount) throw new InvalidOperationException("Restore exceeds restoration basis.");
            working[key] = BuildAllocation(intent, appliedAt, appliedEvidence);
            return;
        }
        throw new InvalidOperationException("Unsupported WP-07 operation.");
    }

    private static ApplicationResourceAllocation BuildAllocation(FoundationAllocationMutationIntent intent, DateTimeOffset appliedAt, ResourceEvidenceReference evidence)
        => new(intent.GrantId, intent.ApplicationId, intent.ResourceClassId, intent.TargetAllocation, intent.TargetQuota, intent.TargetCeiling, new ResourceEffectiveLifetime(appliedAt, null, explicitlyOpenEnded: true), evidence);

    private static void RequireGrant(ApplicationResourceAllocation allocation, ResourceGrantId grantId)
    {
        if (!StringComparer.Ordinal.Equals(allocation.GrantId.Value, grantId.Value)) throw new InvalidOperationException("Grant identity mismatch.");
    }

    private static void ValidateEffect(ResourceEffectBatch batch, ResourceEffectApplicationResult effect, ResourceEpochId epoch, DateTimeOffset appliedAt)
    {
        if (!StringComparer.Ordinal.Equals(effect.BatchIdentitySha256, batch.IdentitySha256)) throw new InvalidOperationException("Applied-effect batch identity mismatch.");
        ResourceMutationGuard.Evidence(effect.Evidence, epoch, appliedAt, "applied effect");
        if (effect.ObservedAt > appliedAt) throw new InvalidOperationException("Applied-effect observation cannot be from the future.");
        if (!effect.Success) throw new InvalidOperationException(effect.PartialEffectObserved ? "Partial resource effect observed; accepted truth is forbidden." : "Resource effect failed; accepted truth is forbidden.");
        if (effect.PartialEffectObserved) throw new InvalidOperationException("Partial effect cannot be accepted as atomic success.");
        var expected = batch.Operations.Select(item => item.OperationId).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        var actual = effect.AppliedOperationIds.OrderBy(item => item, StringComparer.Ordinal).ToArray();
        if (!expected.SequenceEqual(actual, StringComparer.Ordinal)) throw new InvalidOperationException("Applied-effect evidence does not prove the complete atomic operation set.");
    }
}
