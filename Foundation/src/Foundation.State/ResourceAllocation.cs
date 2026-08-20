using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public sealed record ApplicationResourceAllocation
{
    public ApplicationResourceAllocation(
        ResourceGrantId grantId,
        ApplicationPrincipalId applicationId,
        ResourceClassId resourceClassId,
        ResourceQuantity allocation,
        ResourceQuantity quota,
        ResourceQuantity ceiling,
        ResourceEffectiveLifetime lifetime,
        ResourceEvidenceReference evidence)
    {
        GrantId = grantId ?? throw new ArgumentNullException(nameof(grantId));
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        Allocation = allocation ?? throw new ArgumentNullException(nameof(allocation));
        Quota = quota ?? throw new ArgumentNullException(nameof(quota));
        Ceiling = ceiling ?? throw new ArgumentNullException(nameof(ceiling));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));

        if (!StringComparer.Ordinal.Equals(Allocation.Unit, Quota.Unit) ||
            !StringComparer.Ordinal.Equals(Allocation.Unit, Ceiling.Unit))
        {
            throw new ArgumentException("Allocation, quota, and ceiling must use the exact same canonical unit.");
        }

        if (Allocation.Amount > Quota.Amount)
        {
            throw new ArgumentException("Application allocation cannot exceed its quota.");
        }

        if (Quota.Amount > Ceiling.Amount)
        {
            throw new ArgumentException("Application quota cannot exceed its ceiling.");
        }
    }

    public ResourceGrantId GrantId { get; }
    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity Allocation { get; }
    public ResourceQuantity Quota { get; }
    public ResourceQuantity Ceiling { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record ApplicationResourceAllocationView
{
    private readonly ReadOnlyCollection<ApplicationResourceAllocation> _allocations;

    internal ApplicationResourceAllocationView(
        ApplicationPrincipalId applicationId,
        IEnumerable<ApplicationResourceAllocation> allocations,
        string sourceSnapshotIdentitySha256)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ArgumentNullException.ThrowIfNull(allocations);
        SourceSnapshotIdentitySha256 = sourceSnapshotIdentitySha256 ?? throw new ArgumentNullException(nameof(sourceSnapshotIdentitySha256));
        _allocations = Array.AsReadOnly(allocations.ToArray());
    }

    public ApplicationPrincipalId ApplicationId { get; }
    public IReadOnlyList<ApplicationResourceAllocation> Allocations => _allocations;
    public string SourceSnapshotIdentitySha256 { get; }
}

public sealed record ApplicationResourceAllocationSnapshot
{
    private readonly ReadOnlyCollection<ApplicationResourceAllocation> _allocations;

    public ApplicationResourceAllocationSnapshot(
        FoundationResourceTruthSnapshot resourceTruth,
        DateTimeOffset observedAt,
        IEnumerable<ApplicationResourceAllocation> allocations,
        bool allocationTruthAvailable)
    {
        ResourceTruth = resourceTruth ?? throw new ArgumentNullException(nameof(resourceTruth));
        ArgumentNullException.ThrowIfNull(allocations);

        if (!allocationTruthAvailable)
        {
            throw new InvalidOperationException("Application resource allocation truth is unavailable and must fail closed.");
        }

        if (observedAt < ResourceTruth.ObservedAt)
        {
            throw new ArgumentException("Application allocation snapshot cannot predate its Foundation resource-truth snapshot.", nameof(observedAt));
        }

        var ordered = allocations
            .OrderBy(item => item.ApplicationId.Value, StringComparer.Ordinal)
            .ThenBy(item => item.ResourceClassId.Value, StringComparer.Ordinal)
            .ThenBy(item => item.GrantId.Value, StringComparer.Ordinal)
            .ToArray();

        var duplicateBinding = ordered
            .GroupBy(item => (Application: item.ApplicationId.Value, Resource: item.ResourceClassId.Value))
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateBinding is not null)
        {
            throw new ArgumentException(
                $"Duplicate Application/resource allocation binding '{duplicateBinding.Key.Application}/{duplicateBinding.Key.Resource}' is not allowed.",
                nameof(allocations));
        }

        var duplicateGrant = ordered
            .GroupBy(item => item.GrantId.Value, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateGrant is not null)
        {
            throw new ArgumentException($"Duplicate resource grant identity '{duplicateGrant.Key}' is not allowed.", nameof(allocations));
        }

        foreach (var item in ordered)
        {
            var truth = ResourceTruth.GetRequired(item.ResourceClassId);

            if (!StringComparer.Ordinal.Equals(item.Allocation.Unit, truth.AllocatableCapacity.Unit))
            {
                throw new ArgumentException(
                    $"Allocation unit for resource class '{item.ResourceClassId}' does not match Foundation resource truth.",
                    nameof(allocations));
            }

            if (!StringComparer.Ordinal.Equals(item.Evidence.EpochId.Value, ResourceTruth.EpochId.Value))
            {
                throw new ArgumentException(
                    $"Allocation evidence epoch '{item.Evidence.EpochId}' does not match Foundation resource epoch '{ResourceTruth.EpochId}'.",
                    nameof(allocations));
            }

            if (item.Evidence.ObservedAt > observedAt)
            {
                throw new ArgumentException("Allocation evidence observation cannot be later than the allocation snapshot observation.", nameof(allocations));
            }

            if (item.Lifetime.EffectiveFrom > observedAt)
            {
                throw new ArgumentException("A current allocation cannot become effective after the allocation snapshot observation.", nameof(allocations));
            }

            if (item.Lifetime.EffectiveUntil.HasValue && item.Lifetime.EffectiveUntil.Value < observedAt)
            {
                throw new ArgumentException("An expired allocation cannot appear in the current allocation snapshot.", nameof(allocations));
            }

            if (item.Ceiling.Amount > truth.AllocatableCapacity.Amount)
            {
                throw new ArgumentException(
                    $"Application ceiling for resource class '{item.ResourceClassId}' exceeds Foundation allocatable capacity.",
                    nameof(allocations));
            }
        }

        foreach (var resourceGroup in ordered.GroupBy(item => item.ResourceClassId.Value, StringComparer.Ordinal))
        {
            var truth = ResourceTruth.GetRequired(new ResourceClassId(resourceGroup.Key));
            var allocationTotal = resourceGroup.Sum(item => item.Allocation.Amount);
            var quotaTotal = resourceGroup.Sum(item => item.Quota.Amount);
            var ceilingTotal = resourceGroup.Sum(item => item.Ceiling.Amount);

            if (allocationTotal > truth.AllocatableCapacity.Amount)
            {
                throw new ArgumentException($"Aggregate allocation for resource class '{resourceGroup.Key}' exceeds Foundation allocatable capacity.", nameof(allocations));
            }

            if (quotaTotal > truth.AllocatableCapacity.Amount)
            {
                throw new ArgumentException($"Aggregate quota for resource class '{resourceGroup.Key}' exceeds Foundation allocatable capacity.", nameof(allocations));
            }

            if (ceilingTotal > truth.AllocatableCapacity.Amount)
            {
                throw new ArgumentException($"Aggregate ceiling for resource class '{resourceGroup.Key}' exceeds Foundation allocatable capacity.", nameof(allocations));
            }
        }

        ObservedAt = observedAt;
        _allocations = Array.AsReadOnly(ordered);
        IdentitySha256 = ComputeIdentity(ordered);
    }

    public FoundationResourceTruthSnapshot ResourceTruth { get; }
    public ResourceEpochId EpochId => ResourceTruth.EpochId;
    public DateTimeOffset ObservedAt { get; }
    public IReadOnlyList<ApplicationResourceAllocation> Allocations => _allocations;
    public string IdentitySha256 { get; }

    public ApplicationResourceAllocation GetRequiredAllocation(
        ApplicationPrincipalId applicationId,
        ResourceClassId resourceClassId)
    {
        ArgumentNullException.ThrowIfNull(applicationId);
        ArgumentNullException.ThrowIfNull(resourceClassId);

        var match = _allocations.SingleOrDefault(item =>
            StringComparer.Ordinal.Equals(item.ApplicationId.Value, applicationId.Value) &&
            StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value));

        return match ?? throw new KeyNotFoundException(
            $"No allocation exists for Application '{applicationId}' and resource class '{resourceClassId}'.");
    }

    public ApplicationResourceAllocationView GetApplicationView(ApplicationPrincipalId applicationId)
    {
        ArgumentNullException.ThrowIfNull(applicationId);
        var ownAllocations = _allocations.Where(item =>
            StringComparer.Ordinal.Equals(item.ApplicationId.Value, applicationId.Value));
        return new ApplicationResourceAllocationView(applicationId, ownAllocations, IdentitySha256);
    }

    private string ComputeIdentity(IReadOnlyList<ApplicationResourceAllocation> allocations)
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("resource_truth_identity", ResourceTruth.IdentitySha256),
            CanonicalResourceIdentity.IdentifierField("epoch", ResourceTruth.EpochId),
            new("observed_at", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new("allocation_count", allocations.Count.ToString(CultureInfo.InvariantCulture))
        };

        for (var index = 0; index < allocations.Count; index++)
        {
            var item = allocations[index];
            var prefix = $"allocation_{index:D4}_";
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "grant", item.GrantId));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "application", item.ApplicationId));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "resource_class", item.ResourceClassId));
            fields.Add(CanonicalResourceIdentity.QuantityField(prefix + "allocation", item.Allocation));
            fields.Add(CanonicalResourceIdentity.QuantityField(prefix + "quota", item.Quota));
            fields.Add(CanonicalResourceIdentity.QuantityField(prefix + "ceiling", item.Ceiling));
            fields.Add(CanonicalResourceIdentity.LifetimeStartField(prefix + "effective_from", item.Lifetime));
            fields.Add(new CanonicalIdentityField(
                prefix + "effective_until",
                item.Lifetime.EffectiveUntil?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
            fields.Add(new CanonicalIdentityField(
                prefix + "explicitly_open_ended",
                item.Lifetime.ExplicitlyOpenEnded ? "true" : "false"));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_id", item.Evidence.EvidenceId));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_scope", item.Evidence.ScopeId));
            fields.Add(new CanonicalIdentityField(
                prefix + "evidence_observed_at",
                item.Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_epoch", item.Evidence.EpochId));
        }

        return CanonicalResourceIdentity.ComputeSha256(fields);
    }
}
