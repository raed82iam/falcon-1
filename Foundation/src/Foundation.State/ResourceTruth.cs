using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public sealed record FoundationResourceClassTruth
{
    public FoundationResourceClassTruth(
        ResourceClassId resourceClassId,
        ResourceQuantity totalCapacity,
        ResourceQuantity protectionFloor,
        ResourceQuantity recoveryReserve,
        ResourceEvidenceReference evidence)
    {
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        TotalCapacity = totalCapacity ?? throw new ArgumentNullException(nameof(totalCapacity));
        ProtectionFloor = protectionFloor ?? throw new ArgumentNullException(nameof(protectionFloor));
        RecoveryReserve = recoveryReserve ?? throw new ArgumentNullException(nameof(recoveryReserve));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));

        if (!StringComparer.Ordinal.Equals(TotalCapacity.Unit, ProtectionFloor.Unit) ||
            !StringComparer.Ordinal.Equals(TotalCapacity.Unit, RecoveryReserve.Unit))
        {
            throw new ArgumentException("Total capacity, protection floor, and recovery reserve must use the exact same canonical unit.");
        }

        var protectedAmount = ProtectionFloor.Amount + RecoveryReserve.Amount;
        if (protectedAmount > TotalCapacity.Amount)
        {
            throw new ArgumentException("Foundation protection floor plus recovery reserve cannot exceed total capacity.");
        }

        AllocatableCapacity = new ResourceQuantity(TotalCapacity.Amount - protectedAmount, TotalCapacity.Unit);
    }

    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity TotalCapacity { get; }
    public ResourceQuantity ProtectionFloor { get; }
    public ResourceQuantity RecoveryReserve { get; }
    public ResourceQuantity AllocatableCapacity { get; }
    public ResourceEvidenceReference Evidence { get; }

    public ResourceReclaimability ProtectionFloorReclaimability => ResourceReclaimability.NonReclaimable;
    public ResourceReclaimability RecoveryReserveReclaimability => ResourceReclaimability.NonReclaimable;
}

public sealed record FoundationResourceTruthSnapshot
{
    private readonly ReadOnlyCollection<FoundationResourceClassTruth> _resources;

    public FoundationResourceTruthSnapshot(
        ResourceEpochId epochId,
        DateTimeOffset observedAt,
        IEnumerable<FoundationResourceClassTruth> resources,
        bool truthAvailable)
    {
        EpochId = epochId ?? throw new ArgumentNullException(nameof(epochId));
        ArgumentNullException.ThrowIfNull(resources);

        if (!truthAvailable)
        {
            throw new InvalidOperationException("Foundation resource truth is unavailable and must fail closed.");
        }

        var ordered = resources
            .OrderBy(resource => resource.ResourceClassId.Value, StringComparer.Ordinal)
            .ToArray();

        if (ordered.Length == 0)
        {
            throw new ArgumentException("At least one Foundation resource-class truth entry is required.", nameof(resources));
        }

        var duplicate = ordered
            .GroupBy(resource => resource.ResourceClassId.Value, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
        {
            throw new ArgumentException($"Duplicate Foundation resource-class truth '{duplicate.Key}' is not allowed.", nameof(resources));
        }

        foreach (var resource in ordered)
        {
            if (!StringComparer.Ordinal.Equals(resource.Evidence.EpochId.Value, EpochId.Value))
            {
                throw new ArgumentException(
                    $"Resource truth evidence epoch '{resource.Evidence.EpochId}' does not match snapshot epoch '{EpochId}'.",
                    nameof(resources));
            }

            if (resource.Evidence.ObservedAt > observedAt)
            {
                throw new ArgumentException("Resource truth evidence observation cannot be later than the snapshot observation.", nameof(resources));
            }
        }

        ObservedAt = observedAt;
        _resources = Array.AsReadOnly(ordered);
        IdentitySha256 = ComputeIdentity(ordered);
    }

    public ResourceEpochId EpochId { get; }
    public DateTimeOffset ObservedAt { get; }
    public IReadOnlyList<FoundationResourceClassTruth> Resources => _resources;
    public string IdentitySha256 { get; }

    public FoundationResourceClassTruth GetRequired(ResourceClassId resourceClassId)
    {
        ArgumentNullException.ThrowIfNull(resourceClassId);
        var match = _resources.SingleOrDefault(resource =>
            StringComparer.Ordinal.Equals(resource.ResourceClassId.Value, resourceClassId.Value));
        return match ?? throw new KeyNotFoundException($"Unknown Foundation resource class '{resourceClassId}'.");
    }

    private string ComputeIdentity(IReadOnlyList<FoundationResourceClassTruth> resources)
    {
        var fields = new List<CanonicalIdentityField>
        {
            CanonicalResourceIdentity.IdentifierField("epoch", EpochId),
            new("observed_at", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new("resource_count", resources.Count.ToString(CultureInfo.InvariantCulture))
        };

        for (var index = 0; index < resources.Count; index++)
        {
            var resource = resources[index];
            var prefix = $"resource_{index:D4}_";
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "class", resource.ResourceClassId));
            fields.Add(CanonicalResourceIdentity.QuantityField(prefix + "total", resource.TotalCapacity));
            fields.Add(CanonicalResourceIdentity.QuantityField(prefix + "protection_floor", resource.ProtectionFloor));
            fields.Add(CanonicalResourceIdentity.QuantityField(prefix + "recovery_reserve", resource.RecoveryReserve));
            fields.Add(CanonicalResourceIdentity.QuantityField(prefix + "allocatable", resource.AllocatableCapacity));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_id", resource.Evidence.EvidenceId));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_scope", resource.Evidence.ScopeId));
            fields.Add(new CanonicalIdentityField(
                prefix + "evidence_observed_at",
                resource.Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_epoch", resource.Evidence.EpochId));
        }

        return CanonicalResourceIdentity.ComputeSha256(fields);
    }
}
