using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

[Flags]
public enum ResourceIntegrationCurrentContextRequirement
{
    None = 0,
    PriorityAndPressure = 1,
    Decision = 2,
    EffectiveState = 4,
    ProjectionAndSignal = 8,
    Coordinator = 16
}

public sealed record ResourceIntegrationEvidenceBinding
{
    public ResourceIntegrationEvidenceBinding(
        ResourceIntegrationCoherenceBinding coherence,
        AdditionalResourceDecisionRecord? exactDecision = null,
        EffectiveResourceDistributionSnapshot? acceptedEffectiveState = null,
        ResourceAcceptedTransitionChain? effectiveDistributionLineage = null,
        AggregateResourceStateProjection? aggregateProjection = null)
    {
        Coherence = coherence ?? throw new ArgumentNullException(nameof(coherence));
        ExactDecision = exactDecision;
        AcceptedEffectiveState = acceptedEffectiveState;
        EffectiveDistributionLineage = effectiveDistributionLineage;
        AggregateProjection = aggregateProjection;

        DecisionFreshness = ResolveDecisionFreshness();
        EffectiveStateFreshness = ResolveEffectiveStateFreshness();
        CoordinatorFreshness = ResolveCoordinatorFreshness();
        Health = ResolveHealth();

        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("coherence", Coherence.IdentitySha256),
            new CanonicalIdentityField("decision", ExactDecision?.IdentitySha256),
            new CanonicalIdentityField("decisionFreshness", DecisionFreshness.ToString()),
            new CanonicalIdentityField("effectiveState", AcceptedEffectiveState?.IdentitySha256),
            new CanonicalIdentityField("effectiveLineage", EffectiveDistributionLineage?.IdentitySha256),
            new CanonicalIdentityField("effectiveFreshness", EffectiveStateFreshness.ToString()),
            new CanonicalIdentityField("aggregate", AggregateProjection?.IdentitySha256),
            new CanonicalIdentityField("coordinatorFreshness", CoordinatorFreshness.ToString()),
            new CanonicalIdentityField("health", Health.ToString()),
            new CanonicalIdentityField("asOf", Coherence.AsOf.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }

    public ResourceIntegrationCoherenceBinding Coherence { get; }
    public AdditionalResourceDecisionRecord? ExactDecision { get; }
    public EffectiveResourceDistributionSnapshot? AcceptedEffectiveState { get; }
    public ResourceAcceptedTransitionChain? EffectiveDistributionLineage { get; }
    public AggregateResourceStateProjection? AggregateProjection { get; }
    public ResourceCoherenceFreshness DecisionFreshness { get; }
    public ResourceCoherenceFreshness EffectiveStateFreshness { get; }
    public ResourceCoherenceFreshness CoordinatorFreshness { get; }
    public ResourceIntegrationHealth Health { get; }
    public string IdentitySha256 { get; }

    public void RequireCurrent(ResourceIntegrationCurrentContextRequirement requirement)
    {
        if ((requirement & ResourceIntegrationCurrentContextRequirement.PriorityAndPressure) != 0 &&
            (Coherence.PriorityFreshness != ResourceCoherenceFreshness.Current || Coherence.PressureFreshness != ResourceCoherenceFreshness.Current))
            throw new InvalidOperationException("Current priority/pressure context is required but is not current and coherent.");

        if ((requirement & ResourceIntegrationCurrentContextRequirement.Decision) != 0 && DecisionFreshness != ResourceCoherenceFreshness.Current)
            throw new InvalidOperationException("Current exact additional-resource decision context is required but unavailable, lagging, or contradictory.");

        if ((requirement & ResourceIntegrationCurrentContextRequirement.EffectiveState) != 0 && EffectiveStateFreshness != ResourceCoherenceFreshness.Current)
            throw new InvalidOperationException("Current accepted effective-distribution context is required but unavailable, lagging, or contradictory.");

        if ((requirement & ResourceIntegrationCurrentContextRequirement.ProjectionAndSignal) != 0 &&
            (Coherence.ProjectionFreshness != ResourceCoherenceFreshness.Current || Coherence.SignalFreshness != ResourceCoherenceFreshness.Current))
            throw new InvalidOperationException("Current projection/signal context is required but is not current and coherent.");

        if ((requirement & ResourceIntegrationCurrentContextRequirement.Coordinator) != 0 && CoordinatorFreshness != ResourceCoherenceFreshness.Current)
            throw new InvalidOperationException("Current coordinator/envelope context is required but unavailable, lagging, or contradictory.");
    }

    private ResourceCoherenceFreshness ResolveDecisionFreshness()
    {
        if (ExactDecision is null) return ResourceCoherenceFreshness.Unavailable;
        if (!StringComparer.Ordinal.Equals(ExactDecision.ResourceClassId.Value, Coherence.ResourceClassId.Value))
            return ResourceCoherenceFreshness.Contradictory;
        if (ExactDecision.DecidedAt > Coherence.AsOf || ExactDecision.ExpiresAt <= Coherence.AsOf)
            return ResourceCoherenceFreshness.Contradictory;

        var applies = ExactDecision.RequesterKind == ResourceRequesterKind.DirectApplication
            ? ExactDecision.DirectApplicationId is not null && StringComparer.Ordinal.Equals(ExactDecision.DirectApplicationId.Value, Coherence.ApplicationId.Value)
            : ExactDecision.RequesterKind == ResourceRequesterKind.DelegatedAggregateCoordinator &&
              ExactDecision.RepresentedApplications.Any(item => StringComparer.Ordinal.Equals(item.Value, Coherence.ApplicationId.Value));
        if (!applies) return ResourceCoherenceFreshness.Contradictory;

        if (StringComparer.Ordinal.Equals(ExactDecision.AllocationSnapshotIdentitySha256, Coherence.CurrentAllocation.IdentitySha256))
            return ResourceCoherenceFreshness.Current;

        if (Coherence.AuthoritativeAllocationLineage is null)
            return ResourceCoherenceFreshness.Unavailable;

        return Coherence.AuthoritativeAllocationLineage.Bridges(ExactDecision.AllocationSnapshotIdentitySha256, Coherence.CurrentAllocation.IdentitySha256)
            ? ResourceCoherenceFreshness.Lagging
            : ResourceCoherenceFreshness.Contradictory;
    }

    private ResourceCoherenceFreshness ResolveEffectiveStateFreshness()
    {
        if (AcceptedEffectiveState is null)
            return EffectiveDistributionLineage is null ? ResourceCoherenceFreshness.Unavailable : ResourceCoherenceFreshness.Contradictory;
        if (AcceptedEffectiveState.ObservedAt > Coherence.AsOf)
            return ResourceCoherenceFreshness.Contradictory;
        if (!StringComparer.Ordinal.Equals(AcceptedEffectiveState.AuthoritativeAllocationSnapshot.EpochId.Value, Coherence.CurrentAllocation.EpochId.Value))
            return ResourceCoherenceFreshness.Contradictory;

        if (EffectiveDistributionLineage is not null)
        {
            if (EffectiveDistributionLineage.Lane != ResourceCapacityBasisLane.DelegatedEffectiveDistribution)
                return ResourceCoherenceFreshness.Contradictory;
            if (!StringComparer.Ordinal.Equals(EffectiveDistributionLineage.ApplicationId.Value, Coherence.ApplicationId.Value) ||
                !StringComparer.Ordinal.Equals(EffectiveDistributionLineage.ResourceClassId.Value, Coherence.ResourceClassId.Value))
                return ResourceCoherenceFreshness.Contradictory;
            if (!StringComparer.Ordinal.Equals(EffectiveDistributionLineage.EndStateIdentitySha256, AcceptedEffectiveState.IdentitySha256))
                return ResourceCoherenceFreshness.Contradictory;
            if (AcceptedEffectiveState.BorrowedSegments.Count > 0 && EffectiveDistributionLineage.Transitions.Count == 0)
                return ResourceCoherenceFreshness.Unavailable;
        }
        else if (AcceptedEffectiveState.BorrowedSegments.Count > 0)
        {
            return ResourceCoherenceFreshness.Unavailable;
        }

        var allocationPredecessor = AcceptedEffectiveState.AuthoritativeAllocationSnapshot.IdentitySha256;
        if (StringComparer.Ordinal.Equals(allocationPredecessor, Coherence.CurrentAllocation.IdentitySha256))
            return ResourceCoherenceFreshness.Current;

        if (Coherence.AuthoritativeAllocationLineage is null)
            return ResourceCoherenceFreshness.Unavailable;

        return Coherence.AuthoritativeAllocationLineage.Bridges(allocationPredecessor, Coherence.CurrentAllocation.IdentitySha256)
            ? ResourceCoherenceFreshness.Lagging
            : ResourceCoherenceFreshness.Contradictory;
    }

    private ResourceCoherenceFreshness ResolveCoordinatorFreshness()
    {
        if (AggregateProjection is null) return ResourceCoherenceFreshness.Unavailable;
        if (AggregateProjection.ObservedAt > Coherence.AsOf) return ResourceCoherenceFreshness.Contradictory;

        try { AggregateProjection.Envelope.ValidateAt(Coherence.AsOf); }
        catch (InvalidOperationException) { return ResourceCoherenceFreshness.Contradictory; }

        var constituent = AggregateProjection.Constituents.SingleOrDefault(item =>
            StringComparer.Ordinal.Equals(item.ApplicationId.Value, Coherence.ApplicationId.Value) &&
            StringComparer.Ordinal.Equals(item.ResourceClassId.Value, Coherence.ResourceClassId.Value));
        if (constituent is null) return ResourceCoherenceFreshness.Contradictory;
        if (Coherence.Projection is not null && !StringComparer.Ordinal.Equals(constituent.IdentitySha256, Coherence.Projection.IdentitySha256))
            return ResourceCoherenceFreshness.Contradictory;
        if (AcceptedEffectiveState is not null && !StringComparer.Ordinal.Equals(AggregateProjection.Envelope.IdentitySha256, AcceptedEffectiveState.Envelope.IdentitySha256))
            return ResourceCoherenceFreshness.Contradictory;

        var predecessor = AggregateProjection.Envelope.AllocationSnapshot.IdentitySha256;
        if (StringComparer.Ordinal.Equals(predecessor, Coherence.CurrentAllocation.IdentitySha256))
            return ResourceCoherenceFreshness.Current;
        if (Coherence.AuthoritativeAllocationLineage is null)
            return ResourceCoherenceFreshness.Unavailable;
        return Coherence.AuthoritativeAllocationLineage.Bridges(predecessor, Coherence.CurrentAllocation.IdentitySha256)
            ? ResourceCoherenceFreshness.Lagging
            : ResourceCoherenceFreshness.Contradictory;
    }

    private ResourceIntegrationHealth ResolveHealth()
    {
        var statuses = new[]
        {
            Coherence.PriorityFreshness, Coherence.PressureFreshness, Coherence.ProjectionFreshness, Coherence.SignalFreshness,
            DecisionFreshness, EffectiveStateFreshness, CoordinatorFreshness
        };
        if (statuses.Any(item => item == ResourceCoherenceFreshness.Contradictory)) return ResourceIntegrationHealth.Contradictory;
        if (statuses.Any(item => item == ResourceCoherenceFreshness.Lagging)) return ResourceIntegrationHealth.CoherentWithLagging;
        if (statuses.Any(item => item == ResourceCoherenceFreshness.Unavailable)) return ResourceIntegrationHealth.Unavailable;
        return ResourceIntegrationHealth.CoherentCurrent;
    }
}
