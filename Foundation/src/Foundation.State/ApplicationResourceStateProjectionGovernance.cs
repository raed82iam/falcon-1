using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public enum ResourceCapacityBasisLane
{
    FoundationAuthoritativeAllocation = 0,
    DelegatedEffectiveDistribution = 1
}

public enum TechnicalLoadSheddingSignalClass
{
    NoAction = 0,
    AdvisoryReduction = 1,
    ComplianceReductionRequired = 2,
    StateUnavailable = 3
}

public sealed record ExactApplicationResourceUseObservation
{
    public ExactApplicationResourceUseObservation(ApplicationPrincipalId applicationId, ResourceClassId resourceClassId, ResourceEpochId epochId,
        ResourceQuantity usedCapacity, ResourceEvidenceReference evidence, DateTimeOffset observedAt)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        EpochId = epochId ?? throw new ArgumentNullException(nameof(epochId));
        UsedCapacity = usedCapacity ?? throw new ArgumentNullException(nameof(usedCapacity));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (UsedCapacity.Amount < 0m) throw new ArgumentOutOfRangeException(nameof(usedCapacity));
        ResourceMutationGuard.Evidence(Evidence, EpochId, observedAt, "exact resource-use observation");
        ObservedAt = observedAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("application", ApplicationId.Value), new CanonicalIdentityField("resourceClass", ResourceClassId.Value),
            new CanonicalIdentityField("epoch", EpochId.Value), CanonicalResourceIdentity.QuantityField("used", UsedCapacity),
            new CanonicalIdentityField("evidenceId", Evidence.EvidenceId.Value), new CanonicalIdentityField("evidenceScope", Evidence.ScopeId.Value),
            new CanonicalIdentityField("evidenceObservedAt", Evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("observedAt", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }

    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceEpochId EpochId { get; }
    public ResourceQuantity UsedCapacity { get; }
    public ResourceEvidenceReference Evidence { get; }
    public DateTimeOffset ObservedAt { get; }
    public string IdentitySha256 { get; }
}

public sealed record AcceptedResourceCapacityTransitionBasis
{
    private AcceptedResourceCapacityTransitionBasis(ResourceCapacityBasisLane lane, ApplicationPrincipalId applicationId, ResourceClassId resourceClassId,
        ResourceQuantity predecessorCapacity, ResourceQuantity acceptedCapacity, string predecessorStateIdentitySha256,
        string acceptedStateIdentitySha256, ResourceEffectBatch effectBatch, ResourceEffectApplicationResult appliedEffect)
    {
        Lane = lane;
        ApplicationId = applicationId;
        ResourceClassId = resourceClassId;
        PredecessorCapacity = predecessorCapacity;
        AcceptedCapacity = acceptedCapacity;
        PredecessorStateIdentitySha256 = ResourceMutationGuard.Id(predecessorStateIdentitySha256, nameof(predecessorStateIdentitySha256));
        AcceptedStateIdentitySha256 = ResourceMutationGuard.Id(acceptedStateIdentitySha256, nameof(acceptedStateIdentitySha256));
        EffectBatchIdentitySha256 = effectBatch.IdentitySha256;
        AppliedEffect = appliedEffect;
        ResourceMutationGuard.SameUnit(PredecessorCapacity, AcceptedCapacity, "accepted capacity transition");
        IsReduction = AcceptedCapacity.Amount < PredecessorCapacity.Amount;
        IsRestorationOrIncrease = AcceptedCapacity.Amount > PredecessorCapacity.Amount;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("lane", Lane.ToString()), new CanonicalIdentityField("application", ApplicationId.Value),
            new CanonicalIdentityField("resourceClass", ResourceClassId.Value), CanonicalResourceIdentity.QuantityField("before", PredecessorCapacity),
            CanonicalResourceIdentity.QuantityField("after", AcceptedCapacity), new CanonicalIdentityField("predecessor", PredecessorStateIdentitySha256),
            new CanonicalIdentityField("accepted", AcceptedStateIdentitySha256), new CanonicalIdentityField("effectBatch", EffectBatchIdentitySha256),
            new CanonicalIdentityField("effect", AppliedEffect.IdentitySha256)
        });
    }

    public static AcceptedResourceCapacityTransitionBasis FromFoundationMutation(ApplicationResourceAllocationSnapshot predecessor,
        AcceptedFoundationAllocationMutation acceptedMutation, ResourceEffectBatch exactEffectBatch,
        ApplicationPrincipalId applicationId, ResourceClassId resourceClassId)
    {
        ArgumentNullException.ThrowIfNull(predecessor);
        ArgumentNullException.ThrowIfNull(acceptedMutation);
        ArgumentNullException.ThrowIfNull(exactEffectBatch);
        ValidateAcceptedEffect(exactEffectBatch, acceptedMutation.AppliedEffect, predecessor.ResourceTruth.EpochId);
        if (!StringComparer.Ordinal.Equals(predecessor.ResourceTruth.EpochId.Value, acceptedMutation.AcceptedSnapshot.ResourceTruth.EpochId.Value))
            throw new InvalidOperationException("Accepted Foundation mutation epoch mismatch.");
        if (acceptedMutation.AcceptedSnapshot.ObservedAt < predecessor.ObservedAt) throw new InvalidOperationException("Accepted mutation predates predecessor.");

        var before = predecessor.GetRequiredAllocation(applicationId, resourceClassId);
        var after = acceptedMutation.AcceptedSnapshot.GetRequiredAllocation(applicationId, resourceClassId);
        if (!StringComparer.Ordinal.Equals(before.GrantId.Value, after.GrantId.Value)) throw new InvalidOperationException("Accepted mutation changed grant identity.");

        var operations = exactEffectBatch.Operations.Where(op => op.Lane == ResourceEffectLane.FoundationAuthoritativeAllocation &&
            op.SourceApplicationId is not null && StringComparer.Ordinal.Equals(op.SourceApplicationId.Value, applicationId.Value) &&
            StringComparer.Ordinal.Equals(op.ResourceClassId.Value, resourceClassId.Value)).ToArray();
        if (operations.Length == 0) throw new InvalidOperationException("Effect batch contains no Foundation operation for exact Application/resource scope.");
        var last = operations[^1];
        if (last.SourceGrantId is null || !StringComparer.Ordinal.Equals(last.SourceGrantId.Value, before.GrantId.Value))
            throw new InvalidOperationException("Effect batch grant mismatch.");
        if (last.FoundationOperationKind is null || last.TargetQuota is null || last.TargetCeiling is null)
            throw new InvalidOperationException("Foundation effect payload is incomplete.");
        ResourceMutationGuard.SameUnit(after.Allocation, last.PrimaryQuantity, "Foundation effect allocation");
        ResourceMutationGuard.SameUnit(after.Quota, last.TargetQuota, "Foundation effect quota");
        ResourceMutationGuard.SameUnit(after.Ceiling, last.TargetCeiling, "Foundation effect ceiling");
        if (after.Allocation.Amount != last.PrimaryQuantity.Amount || after.Quota.Amount != last.TargetQuota.Amount || after.Ceiling.Amount != last.TargetCeiling.Amount)
            throw new InvalidOperationException("Accepted Foundation snapshot does not match exact applied effect payload.");
        if (last.FoundationOperationKind == ResourceDecisionKind.Reduce && after.Allocation.Amount > before.Allocation.Amount)
            throw new InvalidOperationException("Reduce effect cannot increase allocation.");
        if (last.FoundationOperationKind == ResourceDecisionKind.Revoke && (after.Allocation.Amount != 0m || after.Quota.Amount != 0m || after.Ceiling.Amount != 0m))
            throw new InvalidOperationException("Revoke effect did not produce zero capacity.");
        if (last.FoundationOperationKind == ResourceDecisionKind.Restore && after.Allocation.Amount < before.Allocation.Amount)
            throw new InvalidOperationException("Restore effect cannot lower allocation.");

        return new AcceptedResourceCapacityTransitionBasis(ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, applicationId, resourceClassId,
            before.Allocation, after.Allocation, predecessor.IdentitySha256, acceptedMutation.AcceptedSnapshot.IdentitySha256,
            exactEffectBatch, acceptedMutation.AppliedEffect);
    }

    public static AcceptedResourceCapacityTransitionBasis FromEffectiveDistributionMutation(EffectiveResourceDistributionSnapshot predecessor,
        AcceptedEffectiveDistributionMutation acceptedMutation, ResourceEffectBatch exactEffectBatch,
        ApplicationPrincipalId applicationId, ResourceClassId resourceClassId)
    {
        ArgumentNullException.ThrowIfNull(predecessor);
        ArgumentNullException.ThrowIfNull(acceptedMutation);
        ArgumentNullException.ThrowIfNull(exactEffectBatch);
        var epoch = predecessor.AuthoritativeAllocationSnapshot.ResourceTruth.EpochId;
        ValidateAcceptedEffect(exactEffectBatch, acceptedMutation.AppliedEffect, epoch);
        if (!StringComparer.Ordinal.Equals(predecessor.AuthoritativeAllocationSnapshot.IdentitySha256,
            acceptedMutation.AcceptedSnapshot.AuthoritativeAllocationSnapshot.IdentitySha256))
            throw new InvalidOperationException("Accepted effective-distribution predecessor mismatch.");
        if (acceptedMutation.AcceptedSnapshot.ObservedAt < predecessor.ObservedAt) throw new InvalidOperationException("Accepted redistribution predates predecessor.");

        var before = predecessor.GetEffectiveCapacity(applicationId, resourceClassId);
        var after = acceptedMutation.AcceptedSnapshot.GetEffectiveCapacity(applicationId, resourceClassId);
        decimal expectedDelta = 0m;
        var relevant = 0;
        foreach (var op in exactEffectBatch.Operations.Where(op => op.Lane == ResourceEffectLane.DelegatedEffectiveDistribution &&
                     StringComparer.Ordinal.Equals(op.ResourceClassId.Value, resourceClassId.Value)))
        {
            if (op.SourceApplicationId is null || op.TargetApplicationId is null || op.EffectiveOperationKind is null) continue;
            var isSource = StringComparer.Ordinal.Equals(op.SourceApplicationId.Value, applicationId.Value);
            var isTarget = StringComparer.Ordinal.Equals(op.TargetApplicationId.Value, applicationId.Value);
            if (!isSource && !isTarget) continue;
            relevant++;
            ResourceMutationGuard.SameUnit(before, op.PrimaryQuantity, "effective-distribution effect quantity");
            var amount = op.PrimaryQuantity.Amount;
            if (op.EffectiveOperationKind == EffectiveDistributionOperationKind.Borrow)
                expectedDelta += isSource ? -amount : amount;
            else if (op.EffectiveOperationKind == EffectiveDistributionOperationKind.ReturnBorrowed)
                expectedDelta += isSource ? amount : -amount;
            else
                throw new InvalidOperationException("Unsupported effective-distribution operation.");
        }
        if (relevant == 0) throw new InvalidOperationException("Effect batch contains no effective-distribution operation for exact Application/resource scope.");
        if (after.Amount != before.Amount + expectedDelta)
            throw new InvalidOperationException("Accepted effective capacity does not match exact applied effect batch delta.");

        return new AcceptedResourceCapacityTransitionBasis(ResourceCapacityBasisLane.DelegatedEffectiveDistribution, applicationId, resourceClassId,
            before, after, predecessor.IdentitySha256, acceptedMutation.AcceptedSnapshot.IdentitySha256,
            exactEffectBatch, acceptedMutation.AppliedEffect);
    }

    private static void ValidateAcceptedEffect(ResourceEffectBatch batch, ResourceEffectApplicationResult effect, ResourceEpochId epoch)
    {
        if (!effect.Success || effect.PartialEffectObserved) throw new InvalidOperationException("Only successful non-partial applied resource effects can establish accepted capacity basis.");
        if (!StringComparer.Ordinal.Equals(effect.BatchIdentitySha256, batch.IdentitySha256)) throw new InvalidOperationException("Applied effect batch identity mismatch.");
        var expected = batch.Operations.Select(op => op.OperationId).OrderBy(id => id, StringComparer.Ordinal).ToArray();
        var actual = effect.AppliedOperationIds.OrderBy(id => id, StringComparer.Ordinal).ToArray();
        if (!expected.SequenceEqual(actual, StringComparer.Ordinal)) throw new InvalidOperationException("Applied effect operation set does not exactly match effect batch.");
        ResourceMutationGuard.Evidence(effect.Evidence, epoch, effect.ObservedAt, "accepted resource effect");
    }

    public ResourceCapacityBasisLane Lane { get; }
    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceQuantity PredecessorCapacity { get; }
    public ResourceQuantity AcceptedCapacity { get; }
    public string PredecessorStateIdentitySha256 { get; }
    public string AcceptedStateIdentitySha256 { get; }
    public string EffectBatchIdentitySha256 { get; }
    public ResourceEffectApplicationResult AppliedEffect { get; }
    public DateTimeOffset AcceptedAt => AppliedEffect.ObservedAt;
    public bool IsReduction { get; }
    public bool IsRestorationOrIncrease { get; }
    public string IdentitySha256 { get; }
}

public sealed record AdditionalResourceDecisionProjectionReference
{
    private AdditionalResourceDecisionProjectionReference(AdditionalResourceDecisionRecord decision)
    {
        DecisionId = decision.DecisionId;
        RequestId = decision.RequestId;
        DecisionIdentitySha256 = decision.IdentitySha256;
        RequestIdentitySha256 = decision.RequestIdentitySha256;
        RequesterKind = decision.RequesterKind;
        ResourceClassId = decision.ResourceClassId;
        Outcome = decision.Outcome;
        AllocationSnapshotIdentitySha256 = decision.AllocationSnapshotIdentitySha256;
        CorrelationId = decision.CorrelationId;
        CausationId = decision.CausationId;
        DecidedAt = decision.DecidedAt;
        ExpiresAt = decision.ExpiresAt;
    }

    public static AdditionalResourceDecisionProjectionReference Create(AdditionalResourceDecisionRecord decision, ApplicationResourceAllocationSnapshot allocationSnapshot,
        ApplicationPrincipalId applicationId, ResourceClassId resourceClassId, DateTimeOffset projectedAt)
    {
        ArgumentNullException.ThrowIfNull(decision);
        ArgumentNullException.ThrowIfNull(allocationSnapshot);
        if (!StringComparer.Ordinal.Equals(decision.ResourceClassId.Value, resourceClassId.Value)) throw new InvalidOperationException("Additional-resource decision resource mismatch.");
        if (!StringComparer.Ordinal.Equals(decision.AllocationSnapshotIdentitySha256, allocationSnapshot.IdentitySha256)) throw new InvalidOperationException("Additional-resource decision predecessor mismatch.");
        if (decision.DecidedAt > projectedAt || decision.ExpiresAt <= projectedAt) throw new InvalidOperationException("Additional-resource decision is not applicable at projection time.");
        var applies = decision.RequesterKind == ResourceRequesterKind.DirectApplication
            ? decision.DirectApplicationId is not null && StringComparer.Ordinal.Equals(decision.DirectApplicationId.Value, applicationId.Value)
            : decision.RequesterKind == ResourceRequesterKind.DelegatedAggregateCoordinator && decision.RepresentedApplications.Any(item => StringComparer.Ordinal.Equals(item.Value, applicationId.Value));
        if (!applies) throw new InvalidOperationException("Additional-resource decision does not apply to exact Application scope.");
        return new AdditionalResourceDecisionProjectionReference(decision);
    }

    public ResourceDecisionId DecisionId { get; }
    public ResourceRequestId RequestId { get; }
    public string DecisionIdentitySha256 { get; }
    public string RequestIdentitySha256 { get; }
    public ResourceRequesterKind RequesterKind { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceDecisionKind Outcome { get; }
    public string AllocationSnapshotIdentitySha256 { get; }
    public CorrelationId CorrelationId { get; }
    public CausationId CausationId { get; }
    public DateTimeOffset DecidedAt { get; }
    public DateTimeOffset ExpiresAt { get; }
}

public sealed record ApplicationResourceStateProjection
{
    private readonly ReadOnlyCollection<BorrowedEffectiveCapacitySegment> _borrowedProvenance;

    internal ApplicationResourceStateProjection(ApplicationPrincipalId applicationId, ResourceClassId resourceClassId, ResourceEpochId epochId,
        string allocationSnapshotIdentitySha256, ResourceGrantId grantId, ResourceQuantity allocation, ResourceQuantity quota, ResourceQuantity ceiling,
        ResourceQuantity? effectiveCapacity, string? effectiveDistributionIdentitySha256, string? coordinationEnvelopeIdentitySha256,
        IEnumerable<BorrowedEffectiveCapacitySegment> borrowedProvenance, bool pressureAvailable, ResourcePressureState? pressureState,
        int? utilizationBasisPoints, ResourceEnforcementObservationState enforcementObservationState, bool preemptionEligibleForConsideration,
        AdditionalResourceDecisionProjectionReference? decisionReference, AcceptedResourceCapacityTransitionBasis? acceptedCapacityBasis,
        ExactApplicationResourceUseObservation? exactUseObservation, DateTimeOffset observedAt)
    {
        ApplicationId = applicationId;
        ResourceClassId = resourceClassId;
        EpochId = epochId;
        AllocationSnapshotIdentitySha256 = allocationSnapshotIdentitySha256;
        GrantId = grantId;
        Allocation = allocation;
        Quota = quota;
        Ceiling = ceiling;
        EffectiveCapacity = effectiveCapacity;
        EffectiveDistributionIdentitySha256 = effectiveDistributionIdentitySha256;
        CoordinationEnvelopeIdentitySha256 = coordinationEnvelopeIdentitySha256;
        ArgumentNullException.ThrowIfNull(borrowedProvenance);
        var provenance = borrowedProvenance.OrderBy(item => item.IdentitySha256, StringComparer.Ordinal).ToArray();
        if (provenance.Select(item => item.IdentitySha256).Distinct(StringComparer.Ordinal).Count() != provenance.Length) throw new InvalidOperationException("Duplicate borrowed provenance.");
        _borrowedProvenance = Array.AsReadOnly(provenance);
        PressureAvailable = pressureAvailable;
        PressureState = pressureState;
        UtilizationBasisPoints = utilizationBasisPoints;
        EnforcementObservationState = enforcementObservationState;
        PreemptionEligibleForConsideration = preemptionEligibleForConsideration;
        DecisionReference = decisionReference;
        AcceptedCapacityBasis = acceptedCapacityBasis;
        ExactUseObservation = exactUseObservation;
        ObservedAt = observedAt;
        IdentitySha256 = ComputeIdentity();
    }

    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceEpochId EpochId { get; }
    public string AllocationSnapshotIdentitySha256 { get; }
    public ResourceGrantId GrantId { get; }
    public ResourceQuantity Allocation { get; }
    public ResourceQuantity Quota { get; }
    public ResourceQuantity Ceiling { get; }
    public ResourceQuantity? EffectiveCapacity { get; }
    public bool EffectiveCapacityAvailable => EffectiveCapacity is not null;
    public string? EffectiveDistributionIdentitySha256 { get; }
    public string? CoordinationEnvelopeIdentitySha256 { get; }
    public IReadOnlyList<BorrowedEffectiveCapacitySegment> BorrowedProvenance => _borrowedProvenance;
    public bool PressureAvailable { get; }
    public ResourcePressureState? PressureState { get; }
    public int? UtilizationBasisPoints { get; }
    public ResourceEnforcementObservationState EnforcementObservationState { get; }
    public bool PreemptionEligibleForConsideration { get; }
    public AdditionalResourceDecisionProjectionReference? DecisionReference { get; }
    public AcceptedResourceCapacityTransitionBasis? AcceptedCapacityBasis { get; }
    public ExactApplicationResourceUseObservation? ExactUseObservation { get; }
    public DateTimeOffset ObservedAt { get; }
    public string IdentitySha256 { get; }

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("application", ApplicationId.Value), new("resourceClass", ResourceClassId.Value), new("epoch", EpochId.Value),
            new("allocationSnapshot", AllocationSnapshotIdentitySha256), new("grant", GrantId.Value),
            CanonicalResourceIdentity.QuantityField("allocation", Allocation), CanonicalResourceIdentity.QuantityField("quota", Quota),
            CanonicalResourceIdentity.QuantityField("ceiling", Ceiling), new("effectiveCapacity", EffectiveCapacity?.ToCanonicalString()),
            new("effectiveDistribution", EffectiveDistributionIdentitySha256), new("envelope", CoordinationEnvelopeIdentitySha256),
            new("pressureAvailable", PressureAvailable ? "1" : "0"), new("pressureState", PressureState?.ToString()),
            new("utilizationBasisPoints", UtilizationBasisPoints?.ToString(CultureInfo.InvariantCulture)),
            new("enforcementObservation", EnforcementObservationState.ToString()), new("preemptionEligible", PreemptionEligibleForConsideration ? "1" : "0"),
            new("decision", DecisionReference?.DecisionIdentitySha256), new("capacityBasis", AcceptedCapacityBasis?.IdentitySha256),
            new("exactUse", ExactUseObservation?.IdentitySha256), new("observedAt", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        };
        for (var i = 0; i < _borrowedProvenance.Count; i++) fields.Add(new CanonicalIdentityField($"borrowed[{i:D4}]", _borrowedProvenance[i].IdentitySha256));
        return CanonicalResourceIdentity.ComputeSha256(fields);
    }
}

public sealed record ApplicationResourceStateProjectionSet
{
    private readonly ReadOnlyCollection<ApplicationResourceStateProjection> _projections;

    public ApplicationResourceStateProjectionSet(ResourceEpochId epochId, DateTimeOffset observedAt, IEnumerable<ApplicationResourceStateProjection> projections)
    {
        EpochId = epochId ?? throw new ArgumentNullException(nameof(epochId));
        ArgumentNullException.ThrowIfNull(projections);
        var ordered = projections.Select(item => item ?? throw new ArgumentException("Projection cannot be null.", nameof(projections)))
            .OrderBy(item => string.Join("|", item.ApplicationId.Value, item.ResourceClassId.Value), StringComparer.Ordinal).ToArray();
        if (ordered.Any(item => !StringComparer.Ordinal.Equals(item.EpochId.Value, EpochId.Value))) throw new InvalidOperationException("Projection-set epoch mismatch.");
        if (ordered.Any(item => item.ObservedAt > observedAt)) throw new InvalidOperationException("Projection-set time predates contained projection.");
        var keys = ordered.Select(item => string.Join("|", item.ApplicationId.Value, item.ResourceClassId.Value)).ToArray();
        if (keys.Distinct(StringComparer.Ordinal).Count() != keys.Length) throw new InvalidOperationException("Duplicate Application/resource projection.");
        _projections = Array.AsReadOnly(ordered);
        ObservedAt = observedAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("epoch", EpochId.Value), new CanonicalIdentityField("observedAt", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("projections", string.Join("|", _projections.Select(item => item.IdentitySha256)))
        });
    }

    public ResourceEpochId EpochId { get; }
    public DateTimeOffset ObservedAt { get; }
    public IReadOnlyList<ApplicationResourceStateProjection> Projections => _projections;
    public string IdentitySha256 { get; }

    public IReadOnlyList<ApplicationResourceStateProjection> GetApplicationView(ApplicationPrincipalId applicationId)
    {
        ArgumentNullException.ThrowIfNull(applicationId);
        return Array.AsReadOnly(_projections.Where(item => StringComparer.Ordinal.Equals(item.ApplicationId.Value, applicationId.Value)).ToArray());
    }
}

public sealed record AggregateResourceStateProjection
{
    private readonly ReadOnlyCollection<ApplicationResourceStateProjection> _constituents;

    public AggregateResourceStateProjection(ResourceCoordinationEnvelope envelope, DateTimeOffset observedAt, IEnumerable<ApplicationResourceStateProjection> constituents)
    {
        Envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
        ArgumentNullException.ThrowIfNull(constituents);
        Envelope.ValidateAt(observedAt);
        var ordered = constituents.OrderBy(item => string.Join("|", item.ApplicationId.Value, item.ResourceClassId.Value), StringComparer.Ordinal).ToArray();
        var expected = Envelope.Members.Select(item => string.Join("|", item.ApplicationId.Value, item.ResourceClassId.Value)).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        var actual = ordered.Select(item => string.Join("|", item.ApplicationId.Value, item.ResourceClassId.Value)).ToArray();
        if (!expected.SequenceEqual(actual, StringComparer.Ordinal)) throw new InvalidOperationException("Aggregate constituent set mismatch.");
        foreach (var projection in ordered)
        {
            if (!projection.EffectiveCapacityAvailable) throw new InvalidOperationException("Aggregate projection requires exact effective-capacity truth.");
            if (!StringComparer.Ordinal.Equals(projection.EpochId.Value, Envelope.AllocationSnapshot.ResourceTruth.EpochId.Value)) throw new InvalidOperationException("Aggregate epoch mismatch.");
            if (!StringComparer.Ordinal.Equals(projection.AllocationSnapshotIdentitySha256, Envelope.AllocationSnapshot.IdentitySha256)) throw new InvalidOperationException("Aggregate allocation predecessor mismatch.");
            if (!StringComparer.Ordinal.Equals(projection.CoordinationEnvelopeIdentitySha256, Envelope.IdentitySha256)) throw new InvalidOperationException("Aggregate envelope mismatch.");
        }
        _constituents = Array.AsReadOnly(ordered);
        ObservedAt = observedAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("envelope", Envelope.IdentitySha256), new CanonicalIdentityField("coordinatorInstance", Envelope.CoordinatorInstanceId),
            new CanonicalIdentityField("coordinatorRole", Envelope.CoordinatorRoleId), new CanonicalIdentityField("scope", Envelope.ScopeId.Value),
            new CanonicalIdentityField("authorityGeneration", Envelope.AuthorityGeneration.ToString(CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("fenceGeneration", Envelope.FenceGeneration.ToString(CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("fencingToken", Envelope.FencingToken),
            new CanonicalIdentityField("constituents", string.Join("|", _constituents.Select(item => item.IdentitySha256))),
            new CanonicalIdentityField("observedAt", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }

    public ResourceCoordinationEnvelope Envelope { get; }
    public string CoordinatorInstanceId => Envelope.CoordinatorInstanceId;
    public string CoordinatorRoleId => Envelope.CoordinatorRoleId;
    public ResourceScopeId CoordinationScopeId => Envelope.ScopeId;
    public IReadOnlyList<ApplicationResourceStateProjection> Constituents => _constituents;
    public DateTimeOffset ObservedAt { get; }
    public string IdentitySha256 { get; }
}

public static class ApplicationResourceStateProjectionBuilder
{
    public static ApplicationResourceStateProjection CreateDirect(ApplicationResourceAllocationSnapshot allocationSnapshot,
        ApplicationPrincipalId applicationId, ResourceClassId resourceClassId, DateTimeOffset observedAt,
        FoundationResourcePressureSnapshot? pressureSnapshot = null, EffectiveResourceDistributionSnapshot? effectiveDistribution = null,
        AdditionalResourceDecisionRecord? exactApplicableDecision = null, AcceptedResourceCapacityTransitionBasis? acceptedCapacityBasis = null,
        ExactApplicationResourceUseObservation? exactUseObservation = null)
    {
        ArgumentNullException.ThrowIfNull(allocationSnapshot);
        if (observedAt < allocationSnapshot.ObservedAt) throw new InvalidOperationException("Projection predates allocation truth.");
        var allocation = allocationSnapshot.GetRequiredAllocation(applicationId, resourceClassId);
        var epoch = allocationSnapshot.ResourceTruth.EpochId;

        ResourceQuantity? effectiveCapacity = null;
        string? effectiveDistributionIdentity = null;
        string? envelopeIdentity = null;
        BorrowedEffectiveCapacitySegment[] borrowed = Array.Empty<BorrowedEffectiveCapacitySegment>();
        if (effectiveDistribution is not null)
        {
            if (!StringComparer.Ordinal.Equals(effectiveDistribution.AuthoritativeAllocationSnapshot.IdentitySha256, allocationSnapshot.IdentitySha256))
                throw new InvalidOperationException("Effective-distribution predecessor mismatch.");
            if (effectiveDistribution.ObservedAt > observedAt) throw new InvalidOperationException("Effective-distribution truth is from the future.");
            effectiveCapacity = effectiveDistribution.GetEffectiveCapacity(applicationId, resourceClassId);
            effectiveDistributionIdentity = effectiveDistribution.IdentitySha256;
            envelopeIdentity = effectiveDistribution.Envelope.IdentitySha256;
            borrowed = effectiveDistribution.BorrowedSegments.Where(item => StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value) &&
                (StringComparer.Ordinal.Equals(item.SourceApplicationId.Value, applicationId.Value) || StringComparer.Ordinal.Equals(item.TargetApplicationId.Value, applicationId.Value))).ToArray();
            if (effectiveDistribution.BorrowedSegments.Count > 0 && acceptedCapacityBasis is null)
                throw new InvalidOperationException("Non-quiescent effective-distribution truth requires exact accepted transition basis.");
        }

        ResourcePressureTruth? pressure = null;
        if (pressureSnapshot is not null)
        {
            if (!StringComparer.Ordinal.Equals(pressureSnapshot.EpochId.Value, epoch.Value)) throw new InvalidOperationException("Pressure epoch mismatch.");
            if (!StringComparer.Ordinal.Equals(pressureSnapshot.PrioritySnapshot.AllocationSnapshot.IdentitySha256, allocationSnapshot.IdentitySha256)) throw new InvalidOperationException("Pressure predecessor mismatch.");
            if (pressureSnapshot.ObservedAt > observedAt) throw new InvalidOperationException("Pressure truth is from the future.");
            pressure = pressureSnapshot.GetApplicationView(applicationId).SingleOrDefault(item => StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value));
        }

        var decisionReference = exactApplicableDecision is null ? null
            : AdditionalResourceDecisionProjectionReference.Create(exactApplicableDecision, allocationSnapshot, applicationId, resourceClassId, observedAt);

        if (acceptedCapacityBasis is not null)
        {
            if (!StringComparer.Ordinal.Equals(acceptedCapacityBasis.ApplicationId.Value, applicationId.Value) || !StringComparer.Ordinal.Equals(acceptedCapacityBasis.ResourceClassId.Value, resourceClassId.Value))
                throw new InvalidOperationException("Accepted capacity basis scope mismatch.");
            if (acceptedCapacityBasis.AcceptedAt > observedAt) throw new InvalidOperationException("Accepted capacity basis is from the future.");
            if (acceptedCapacityBasis.Lane == ResourceCapacityBasisLane.DelegatedEffectiveDistribution)
            {
                if (effectiveDistribution is null || !StringComparer.Ordinal.Equals(acceptedCapacityBasis.AcceptedStateIdentitySha256, effectiveDistribution.IdentitySha256))
                    throw new InvalidOperationException("Accepted redistribution basis does not match exact current effective-distribution state.");
            }
            else
            {
                if (!StringComparer.Ordinal.Equals(acceptedCapacityBasis.AcceptedStateIdentitySha256, allocationSnapshot.IdentitySha256))
                    throw new InvalidOperationException("Accepted Foundation capacity basis does not match exact current allocation state.");
                if (effectiveDistribution is null)
                {
                    if (observedAt != acceptedCapacityBasis.AcceptedAt)
                        throw new InvalidOperationException("Without exact effective-distribution truth, Foundation post-mutation capacity is binding only at exact accepted post-effect instant.");
                    effectiveCapacity = acceptedCapacityBasis.AcceptedCapacity;
                }
            }
            if (effectiveCapacity is null) throw new InvalidOperationException("Binding accepted capacity basis requires exact current effective-capacity truth.");
            ResourceMutationGuard.SameUnit(effectiveCapacity, acceptedCapacityBasis.AcceptedCapacity, "accepted capacity basis");
            if (effectiveCapacity.Amount != acceptedCapacityBasis.AcceptedCapacity.Amount) throw new InvalidOperationException("Accepted capacity basis does not equal current effective capacity.");
        }

        if (exactUseObservation is not null)
        {
            if (!StringComparer.Ordinal.Equals(exactUseObservation.ApplicationId.Value, applicationId.Value) || !StringComparer.Ordinal.Equals(exactUseObservation.ResourceClassId.Value, resourceClassId.Value) ||
                !StringComparer.Ordinal.Equals(exactUseObservation.EpochId.Value, epoch.Value)) throw new InvalidOperationException("Exact-use observation scope mismatch.");
            if (exactUseObservation.ObservedAt > observedAt) throw new InvalidOperationException("Exact-use observation is from the future.");
            ResourceMutationGuard.SameUnit(allocation.Allocation, exactUseObservation.UsedCapacity, "exact use");
        }

        return new ApplicationResourceStateProjection(applicationId, resourceClassId, epoch, allocationSnapshot.IdentitySha256, allocation.GrantId,
            allocation.Allocation, allocation.Quota, allocation.Ceiling, effectiveCapacity, effectiveDistributionIdentity, envelopeIdentity, borrowed,
            pressure?.PressureAvailable ?? false, pressure?.State, pressure?.UtilizationBasisPoints,
            pressure?.EnforcementState ?? ResourceEnforcementObservationState.None, pressure?.PreemptionEligibleForConsideration ?? false,
            decisionReference, acceptedCapacityBasis, exactUseObservation, observedAt);
    }
}

public sealed record ApplicationResourceLoadSheddingSignal
{
    internal ApplicationResourceLoadSheddingSignal(ApplicationResourceStateProjection projection, TechnicalLoadSheddingSignalClass signalClass,
        ResourceQuantity? compliantCapacityTarget, ResourceQuantity? requiredReduction, string? acceptedCapacityBasisIdentitySha256, DateTimeOffset generatedAt)
    {
        Projection = projection ?? throw new ArgumentNullException(nameof(projection));
        SignalClass = signalClass;
        CompliantCapacityTarget = compliantCapacityTarget;
        RequiredReduction = requiredReduction;
        AcceptedCapacityBasisIdentitySha256 = acceptedCapacityBasisIdentitySha256;
        if (generatedAt < projection.ObservedAt) throw new InvalidOperationException("Signal predates projection.");
        GeneratedAt = generatedAt;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("projection", Projection.IdentitySha256), new CanonicalIdentityField("class", SignalClass.ToString()),
            new CanonicalIdentityField("target", CompliantCapacityTarget?.ToCanonicalString()), new CanonicalIdentityField("reduction", RequiredReduction?.ToCanonicalString()),
            new CanonicalIdentityField("acceptedCapacityBasis", AcceptedCapacityBasisIdentitySha256),
            new CanonicalIdentityField("generatedAt", GeneratedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
        });
    }

    public ApplicationResourceStateProjection Projection { get; }
    public ApplicationPrincipalId ApplicationId => Projection.ApplicationId;
    public ResourceClassId ResourceClassId => Projection.ResourceClassId;
    public TechnicalLoadSheddingSignalClass SignalClass { get; }
    public ResourceQuantity? CompliantCapacityTarget { get; }
    public ResourceQuantity? RequiredReduction { get; }
    public string? AcceptedCapacityBasisIdentitySha256 { get; }
    public DateTimeOffset GeneratedAt { get; }
    public string IdentitySha256 { get; }
}

public static class ApplicationResourceLoadSheddingSignalFactory
{
    public static ApplicationResourceLoadSheddingSignal Create(ApplicationResourceStateProjection projection, DateTimeOffset generatedAt)
    {
        ArgumentNullException.ThrowIfNull(projection);
        var basis = projection.AcceptedCapacityBasis;
        if (basis is not null && basis.IsReduction)
        {
            if (!projection.EffectiveCapacityAvailable || projection.EffectiveCapacity is null)
                return new ApplicationResourceLoadSheddingSignal(projection, TechnicalLoadSheddingSignalClass.StateUnavailable, null, null, null, generatedAt);
            ResourceQuantity? reduction = null;
            if (projection.ExactUseObservation is not null)
            {
                var used = projection.ExactUseObservation.UsedCapacity;
                ResourceMutationGuard.SameUnit(used, basis.AcceptedCapacity, "required reduction");
                reduction = new ResourceQuantity(Math.Max(used.Amount - basis.AcceptedCapacity.Amount, 0m), used.Unit);
            }
            return new ApplicationResourceLoadSheddingSignal(projection, TechnicalLoadSheddingSignalClass.ComplianceReductionRequired,
                basis.AcceptedCapacity, reduction, basis.IdentitySha256, generatedAt);
        }

        if (!projection.PressureAvailable || !projection.PressureState.HasValue)
            return new ApplicationResourceLoadSheddingSignal(projection, TechnicalLoadSheddingSignalClass.StateUnavailable, null, null, null, generatedAt);

        if (projection.PressureState.Value != ResourcePressureState.Normal)
            return new ApplicationResourceLoadSheddingSignal(projection, TechnicalLoadSheddingSignalClass.AdvisoryReduction, null, null, null, generatedAt);

        if (!projection.EffectiveCapacityAvailable)
            return new ApplicationResourceLoadSheddingSignal(projection, TechnicalLoadSheddingSignalClass.StateUnavailable, null, null, null, generatedAt);

        return new ApplicationResourceLoadSheddingSignal(projection, TechnicalLoadSheddingSignalClass.NoAction, null, null, null, generatedAt);
    }
}