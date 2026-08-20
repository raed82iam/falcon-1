using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public enum ResourceCoherenceFreshness
{
    Current = 0,
    Lagging = 1,
    Unavailable = 2,
    Contradictory = 3
}

public enum ResourceIntegrationHealth
{
    CoherentCurrent = 0,
    CoherentWithLagging = 1,
    Unavailable = 2,
    Contradictory = 3
}

public sealed record ResourceAcceptedTransitionChain
{
    private readonly ReadOnlyCollection<AcceptedResourceCapacityTransitionBasis> _transitions;

    public ResourceAcceptedTransitionChain(
        ApplicationPrincipalId applicationId,
        ResourceClassId resourceClassId,
        ResourceCapacityBasisLane lane,
        string startStateIdentitySha256,
        string endStateIdentitySha256,
        IEnumerable<AcceptedResourceCapacityTransitionBasis> transitions)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        Lane = lane;
        StartStateIdentitySha256 = ResourceMutationGuard.Id(startStateIdentitySha256, nameof(startStateIdentitySha256));
        EndStateIdentitySha256 = ResourceMutationGuard.Id(endStateIdentitySha256, nameof(endStateIdentitySha256));
        ArgumentNullException.ThrowIfNull(transitions);

        var ordered = transitions.Select(item => item ?? throw new ArgumentException("Transition cannot be null.", nameof(transitions))).ToArray();
        if (ordered.Select(item => item.IdentitySha256).Distinct(StringComparer.Ordinal).Count() != ordered.Length)
            throw new InvalidOperationException("Transition chain cannot contain duplicate accepted transition identities.");

        if (ordered.Length == 0)
        {
            if (!StringComparer.Ordinal.Equals(StartStateIdentitySha256, EndStateIdentitySha256))
                throw new InvalidOperationException("A non-identity state bridge requires at least one accepted transition.");
        }
        else
        {
            ResourceQuantity? priorAcceptedCapacity = null;
            DateTimeOffset? priorAcceptedAt = null;
            string expectedPredecessor = StartStateIdentitySha256;

            foreach (var transition in ordered)
            {
                if (!StringComparer.Ordinal.Equals(transition.ApplicationId.Value, ApplicationId.Value))
                    throw new InvalidOperationException("Transition-chain Application scope mismatch.");
                if (!StringComparer.Ordinal.Equals(transition.ResourceClassId.Value, ResourceClassId.Value))
                    throw new InvalidOperationException("Transition-chain resource scope mismatch.");
                if (transition.Lane != Lane)
                    throw new InvalidOperationException("Transition-chain lane mismatch.");
                if (!StringComparer.Ordinal.Equals(transition.PredecessorStateIdentitySha256, expectedPredecessor))
                    throw new InvalidOperationException("Transition chain contains a gap, fork, or reordered predecessor.");

                if (priorAcceptedCapacity is not null)
                {
                    ResourceMutationGuard.SameUnit(priorAcceptedCapacity, transition.PredecessorCapacity, "transition-chain capacity");
                    if (priorAcceptedCapacity.Amount != transition.PredecessorCapacity.Amount)
                        throw new InvalidOperationException("Transition-chain predecessor quantity does not equal the previous accepted quantity.");
                }
                if (priorAcceptedAt.HasValue && transition.AcceptedAt < priorAcceptedAt.Value)
                    throw new InvalidOperationException("Transition-chain accepted time moved backwards.");

                priorAcceptedCapacity = transition.AcceptedCapacity;
                priorAcceptedAt = transition.AcceptedAt;
                expectedPredecessor = transition.AcceptedStateIdentitySha256;
            }

            if (!StringComparer.Ordinal.Equals(expectedPredecessor, EndStateIdentitySha256))
                throw new InvalidOperationException("Transition chain does not terminate at the declared target state.");
        }

        _transitions = Array.AsReadOnly(ordered);
        IdentitySha256 = ComputeIdentity();
    }

    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceCapacityBasisLane Lane { get; }
    public string StartStateIdentitySha256 { get; }
    public string EndStateIdentitySha256 { get; }
    public IReadOnlyList<AcceptedResourceCapacityTransitionBasis> Transitions => _transitions;
    public string IdentitySha256 { get; }

    public bool Bridges(string olderStateIdentitySha256, string newerStateIdentitySha256)
    {
        var older = ResourceMutationGuard.Id(olderStateIdentitySha256, nameof(olderStateIdentitySha256));
        var newer = ResourceMutationGuard.Id(newerStateIdentitySha256, nameof(newerStateIdentitySha256));
        return StringComparer.Ordinal.Equals(StartStateIdentitySha256, older)
            && StringComparer.Ordinal.Equals(EndStateIdentitySha256, newer);
    }

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("application", ApplicationId.Value),
            new("resourceClass", ResourceClassId.Value),
            new("lane", Lane.ToString()),
            new("startState", StartStateIdentitySha256),
            new("endState", EndStateIdentitySha256)
        };

        for (var i = 0; i < _transitions.Count; i++)
            fields.Add(new CanonicalIdentityField($"transition[{i:D4}]", _transitions[i].IdentitySha256));

        return CanonicalResourceIdentity.ComputeSha256(fields);
    }
}

public sealed record ResourceIntegrationCoherenceBinding
{
    public ResourceIntegrationCoherenceBinding(
        ApplicationResourceAllocationSnapshot currentAllocation,
        ApplicationPrincipalId applicationId,
        ResourceClassId resourceClassId,
        DateTimeOffset asOf,
        ResourcePriorityGovernanceSnapshot? prioritySnapshot = null,
        FoundationResourcePressureSnapshot? pressureSnapshot = null,
        ResourceAcceptedTransitionChain? authoritativeAllocationLineage = null,
        ApplicationResourceStateProjection? projection = null,
        ApplicationResourceLoadSheddingSignal? signal = null)
    {
        CurrentAllocation = currentAllocation ?? throw new ArgumentNullException(nameof(currentAllocation));
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        if (asOf < CurrentAllocation.ObservedAt) throw new InvalidOperationException("Integrated coherence time predates current allocation truth.");
        _ = CurrentAllocation.GetRequiredAllocation(ApplicationId, ResourceClassId);
        AsOf = asOf;

        if (authoritativeAllocationLineage is not null)
        {
            if (authoritativeAllocationLineage.Lane != ResourceCapacityBasisLane.FoundationAuthoritativeAllocation)
                throw new InvalidOperationException("Allocation-lineage bridge must use the Foundation-authoritative allocation lane.");
            if (!StringComparer.Ordinal.Equals(authoritativeAllocationLineage.ApplicationId.Value, ApplicationId.Value) ||
                !StringComparer.Ordinal.Equals(authoritativeAllocationLineage.ResourceClassId.Value, ResourceClassId.Value))
                throw new InvalidOperationException("Allocation-lineage bridge scope mismatch.");
            if (!StringComparer.Ordinal.Equals(authoritativeAllocationLineage.EndStateIdentitySha256, CurrentAllocation.IdentitySha256))
                throw new InvalidOperationException("Allocation-lineage bridge must terminate at the exact supplied current allocation state.");
        }

        AuthoritativeAllocationLineage = authoritativeAllocationLineage;
        PrioritySnapshot = prioritySnapshot;
        PressureSnapshot = pressureSnapshot;
        Projection = projection;
        Signal = signal;

        PriorityFreshness = ResolvePriorityFreshness();
        PressureFreshness = ResolvePressureFreshness();
        ProjectionFreshness = ResolveProjectionFreshness();
        SignalFreshness = ResolveSignalFreshness();
        Health = ResolveHealth();

        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("application", ApplicationId.Value),
            new CanonicalIdentityField("resourceClass", ResourceClassId.Value),
            new CanonicalIdentityField("epoch", CurrentAllocation.EpochId.Value),
            new CanonicalIdentityField("currentAllocation", CurrentAllocation.IdentitySha256),
            new CanonicalIdentityField("allocationLineage", AuthoritativeAllocationLineage?.IdentitySha256),
            new CanonicalIdentityField("priority", PrioritySnapshot?.IdentitySha256),
            new CanonicalIdentityField("priorityFreshness", PriorityFreshness.ToString()),
            new CanonicalIdentityField("pressure", PressureSnapshot?.IdentitySha256),
            new CanonicalIdentityField("pressureFreshness", PressureFreshness.ToString()),
            new CanonicalIdentityField("projection", Projection?.IdentitySha256),
            new CanonicalIdentityField("projectionFreshness", ProjectionFreshness.ToString()),
            new CanonicalIdentityField("signal", Signal?.IdentitySha256),
            new CanonicalIdentityField("signalFreshness", SignalFreshness.ToString()),
            new CanonicalIdentityField("health", Health.ToString()),
            new CanonicalIdentityField("asOf", AsOf.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }

    public ApplicationResourceAllocationSnapshot CurrentAllocation { get; }
    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public DateTimeOffset AsOf { get; }
    public ResourceAcceptedTransitionChain? AuthoritativeAllocationLineage { get; }
    public ResourcePriorityGovernanceSnapshot? PrioritySnapshot { get; }
    public FoundationResourcePressureSnapshot? PressureSnapshot { get; }
    public ApplicationResourceStateProjection? Projection { get; }
    public ApplicationResourceLoadSheddingSignal? Signal { get; }
    public ResourceCoherenceFreshness PriorityFreshness { get; }
    public ResourceCoherenceFreshness PressureFreshness { get; }
    public ResourceCoherenceFreshness ProjectionFreshness { get; }
    public ResourceCoherenceFreshness SignalFreshness { get; }
    public ResourceIntegrationHealth Health { get; }
    public string IdentitySha256 { get; }

    private ResourceIntegrationHealth ResolveHealth()
    {
        var statuses = new[] { PriorityFreshness, PressureFreshness, ProjectionFreshness, SignalFreshness };
        if (statuses.Any(item => item == ResourceCoherenceFreshness.Contradictory)) return ResourceIntegrationHealth.Contradictory;
        if (statuses.Any(item => item == ResourceCoherenceFreshness.Lagging)) return ResourceIntegrationHealth.CoherentWithLagging;
        if (statuses.Any(item => item == ResourceCoherenceFreshness.Unavailable)) return ResourceIntegrationHealth.Unavailable;
        return ResourceIntegrationHealth.CoherentCurrent;
    }

    private ResourceCoherenceFreshness ResolvePriorityFreshness()
    {
        if (PrioritySnapshot is null) return ResourceCoherenceFreshness.Unavailable;
        if (!StringComparer.Ordinal.Equals(PrioritySnapshot.EpochId.Value, CurrentAllocation.EpochId.Value))
            return ResourceCoherenceFreshness.Contradictory;
        if (PrioritySnapshot.ObservedAt > AsOf) return ResourceCoherenceFreshness.Contradictory;

        var predecessor = PrioritySnapshot.AllocationSnapshot.IdentitySha256;
        if (StringComparer.Ordinal.Equals(predecessor, CurrentAllocation.IdentitySha256))
            return ResourceCoherenceFreshness.Current;

        if (AuthoritativeAllocationLineage is null)
            return ResourceCoherenceFreshness.Unavailable;

        return AuthoritativeAllocationLineage.Bridges(predecessor, CurrentAllocation.IdentitySha256)
            ? ResourceCoherenceFreshness.Lagging
            : ResourceCoherenceFreshness.Contradictory;
    }

    private ResourceCoherenceFreshness ResolvePressureFreshness()
    {
        if (PressureSnapshot is null) return ResourceCoherenceFreshness.Unavailable;
        if (PrioritySnapshot is null) return ResourceCoherenceFreshness.Unavailable;
        if (!StringComparer.Ordinal.Equals(PressureSnapshot.EpochId.Value, CurrentAllocation.EpochId.Value))
            return ResourceCoherenceFreshness.Contradictory;
        if (PressureSnapshot.ObservedAt > AsOf) return ResourceCoherenceFreshness.Contradictory;
        if (!StringComparer.Ordinal.Equals(PressureSnapshot.PrioritySnapshot.IdentitySha256, PrioritySnapshot.IdentitySha256))
            return ResourceCoherenceFreshness.Contradictory;

        return PriorityFreshness switch
        {
            ResourceCoherenceFreshness.Current => ResourceCoherenceFreshness.Current,
            ResourceCoherenceFreshness.Lagging => ResourceCoherenceFreshness.Lagging,
            ResourceCoherenceFreshness.Unavailable => ResourceCoherenceFreshness.Unavailable,
            _ => ResourceCoherenceFreshness.Contradictory
        };
    }

    private ResourceCoherenceFreshness ResolveProjectionFreshness()
    {
        if (Projection is null) return ResourceCoherenceFreshness.Unavailable;
        if (Projection.ObservedAt > AsOf) return ResourceCoherenceFreshness.Contradictory;
        if (!StringComparer.Ordinal.Equals(Projection.ApplicationId.Value, ApplicationId.Value) ||
            !StringComparer.Ordinal.Equals(Projection.ResourceClassId.Value, ResourceClassId.Value) ||
            !StringComparer.Ordinal.Equals(Projection.EpochId.Value, CurrentAllocation.EpochId.Value))
            return ResourceCoherenceFreshness.Contradictory;

        if (StringComparer.Ordinal.Equals(Projection.AllocationSnapshotIdentitySha256, CurrentAllocation.IdentitySha256))
            return ResourceCoherenceFreshness.Current;

        if (AuthoritativeAllocationLineage is null)
            return ResourceCoherenceFreshness.Unavailable;

        return AuthoritativeAllocationLineage.Bridges(Projection.AllocationSnapshotIdentitySha256, CurrentAllocation.IdentitySha256)
            ? ResourceCoherenceFreshness.Lagging
            : ResourceCoherenceFreshness.Contradictory;
    }

    private ResourceCoherenceFreshness ResolveSignalFreshness()
    {
        if (Signal is null) return ResourceCoherenceFreshness.Unavailable;
        if (Projection is null) return ResourceCoherenceFreshness.Unavailable;
        if (Signal.GeneratedAt > AsOf) return ResourceCoherenceFreshness.Contradictory;
        if (!StringComparer.Ordinal.Equals(Signal.Projection.IdentitySha256, Projection.IdentitySha256))
            return ResourceCoherenceFreshness.Contradictory;

        return ProjectionFreshness switch
        {
            ResourceCoherenceFreshness.Current => ResourceCoherenceFreshness.Current,
            ResourceCoherenceFreshness.Lagging => ResourceCoherenceFreshness.Lagging,
            ResourceCoherenceFreshness.Unavailable => ResourceCoherenceFreshness.Unavailable,
            _ => ResourceCoherenceFreshness.Contradictory
        };
    }
}
