using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public enum ResourcePressureScopeKind
{
    FoundationResourceClass = 0,
    ApplicationResource = 1
}

public enum ResourceEnforcementObservationState
{
    None = 0,
    AdmissionRestricted = 1,
    ReductionObserved = 2,
    ProtectedCapacityPreservation = 3,
    RestorationNotEligible = 4,
    Unavailable = 5
}

public sealed record ResourcePressureTransitionPolicy
{
    public ResourcePressureTransitionPolicy(ResourceClassId resourceClassId, int constrainedThresholdBasisPoints, int degradedThresholdBasisPoints, int criticalThresholdBasisPoints, int recoveryHysteresisBasisPoints, string policyVersion, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        PolicyVersion = ResourcePrimitiveValidation.RequireCanonicalIdentifier(policyVersion);
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (constrainedThresholdBasisPoints <= 0 || constrainedThresholdBasisPoints >= 10_000) throw new ArgumentOutOfRangeException(nameof(constrainedThresholdBasisPoints));
        if (degradedThresholdBasisPoints <= constrainedThresholdBasisPoints || degradedThresholdBasisPoints >= 10_000) throw new ArgumentOutOfRangeException(nameof(degradedThresholdBasisPoints));
        if (criticalThresholdBasisPoints <= degradedThresholdBasisPoints || criticalThresholdBasisPoints > 10_000) throw new ArgumentOutOfRangeException(nameof(criticalThresholdBasisPoints));
        if (recoveryHysteresisBasisPoints <= 0 || recoveryHysteresisBasisPoints >= constrainedThresholdBasisPoints) throw new ArgumentOutOfRangeException(nameof(recoveryHysteresisBasisPoints));
        ConstrainedThresholdBasisPoints = constrainedThresholdBasisPoints;
        DegradedThresholdBasisPoints = degradedThresholdBasisPoints;
        CriticalThresholdBasisPoints = criticalThresholdBasisPoints;
        RecoveryHysteresisBasisPoints = recoveryHysteresisBasisPoints;
    }
    public ResourceClassId ResourceClassId { get; }
    public int ConstrainedThresholdBasisPoints { get; }
    public int DegradedThresholdBasisPoints { get; }
    public int CriticalThresholdBasisPoints { get; }
    public int RecoveryHysteresisBasisPoints { get; }
    public string PolicyVersion { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record ResourcePressureObservation
{
    public ResourcePressureObservation(ResourcePressureScopeKind scopeKind, ResourceScopeId technicalScopeId, ResourceClassId resourceClassId, ApplicationPrincipalId? applicationId, ResourceQuantity? usedCapacity, long sequence, ResourceEvidenceReference evidence)
    {
        TechnicalScopeId = technicalScopeId ?? throw new ArgumentNullException(nameof(technicalScopeId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (sequence <= 0) throw new ArgumentOutOfRangeException(nameof(sequence));
        if (scopeKind == ResourcePressureScopeKind.FoundationResourceClass && applicationId is not null) throw new ArgumentException("Foundation-resource pressure observation cannot carry an Application identity.", nameof(applicationId));
        if (scopeKind == ResourcePressureScopeKind.ApplicationResource && applicationId is null) throw new ArgumentException("Application-bound pressure observation requires an exact Application identity.", nameof(applicationId));
        ScopeKind = scopeKind;
        ApplicationId = applicationId;
        UsedCapacity = usedCapacity;
        Sequence = sequence;
    }
    public ResourcePressureScopeKind ScopeKind { get; }
    public ResourceScopeId TechnicalScopeId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ApplicationPrincipalId? ApplicationId { get; }
    public ResourceQuantity? UsedCapacity { get; }
    public long Sequence { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record ResourcePreemptionEligibilityBinding
{
    public ResourcePreemptionEligibilityBinding(ResourceGrantId grantId, ApplicationPrincipalId applicationId, ResourceClassId resourceClassId, ResourceReclaimability reclaimability, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        GrantId = grantId ?? throw new ArgumentNullException(nameof(grantId));
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        Reclaimability = reclaimability;
    }
    public ResourceGrantId GrantId { get; }
    public ApplicationPrincipalId ApplicationId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ResourceReclaimability Reclaimability { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record ResourceEnforcementObservation
{
    public ResourceEnforcementObservation(ResourcePressureScopeKind scopeKind, ResourceScopeId technicalScopeId, ResourceClassId resourceClassId, ApplicationPrincipalId? applicationId, ResourceEnforcementObservationState state, ResourceEvidenceReference authorityEvidence)
    {
        TechnicalScopeId = technicalScopeId ?? throw new ArgumentNullException(nameof(technicalScopeId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        AuthorityEvidence = authorityEvidence ?? throw new ArgumentNullException(nameof(authorityEvidence));
        if (scopeKind == ResourcePressureScopeKind.FoundationResourceClass && applicationId is not null) throw new ArgumentException("Foundation-resource enforcement observation cannot carry an Application identity.", nameof(applicationId));
        if (scopeKind == ResourcePressureScopeKind.ApplicationResource && applicationId is null) throw new ArgumentException("Application-bound enforcement observation requires an exact Application identity.", nameof(applicationId));
        ScopeKind = scopeKind;
        ApplicationId = applicationId;
        State = state;
    }
    public ResourcePressureScopeKind ScopeKind { get; }
    public ResourceScopeId TechnicalScopeId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ApplicationPrincipalId? ApplicationId { get; }
    public ResourceEnforcementObservationState State { get; }
    public ResourceEvidenceReference AuthorityEvidence { get; }
}

public sealed record ResourcePressureTruth
{
    internal ResourcePressureTruth(ResourcePressureScopeKind scopeKind, ResourceScopeId technicalScopeId, ResourceClassId resourceClassId, ApplicationPrincipalId? applicationId, long sequence, ResourcePressureState? state, int? utilizationBasisPoints, ResourceEnforcementObservationState enforcementState, bool preemptionEligibleForConsideration, ResourceEvidenceReference observationEvidence, string transitionPolicyVersion)
    {
        ScopeKind = scopeKind;
        TechnicalScopeId = technicalScopeId;
        ResourceClassId = resourceClassId;
        ApplicationId = applicationId;
        Sequence = sequence;
        State = state;
        UtilizationBasisPoints = utilizationBasisPoints;
        EnforcementState = enforcementState;
        PreemptionEligibleForConsideration = preemptionEligibleForConsideration;
        ObservationEvidence = observationEvidence;
        TransitionPolicyVersion = transitionPolicyVersion;
    }
    public ResourcePressureScopeKind ScopeKind { get; }
    public ResourceScopeId TechnicalScopeId { get; }
    public ResourceClassId ResourceClassId { get; }
    public ApplicationPrincipalId? ApplicationId { get; }
    public long Sequence { get; }
    public ResourcePressureState? State { get; }
    public bool PressureAvailable => State.HasValue;
    public int? UtilizationBasisPoints { get; }
    public ResourceEnforcementObservationState EnforcementState { get; }
    public bool PreemptionEligibleForConsideration { get; }
    public ResourceEvidenceReference ObservationEvidence { get; }
    public string TransitionPolicyVersion { get; }
}

public sealed record FoundationResourcePressureSnapshot
{
    private readonly ReadOnlyCollection<ResourcePressureTruth> _truth;

    public FoundationResourcePressureSnapshot(ResourcePriorityGovernanceSnapshot prioritySnapshot, DateTimeOffset observedAt, IEnumerable<ResourcePressureTransitionPolicy> transitionPolicies, IEnumerable<ResourcePressureObservation> observations, IEnumerable<ResourcePreemptionEligibilityBinding> eligibilityBindings, IEnumerable<ResourceEnforcementObservation> enforcementObservations, FoundationResourcePressureSnapshot? previousSnapshot = null)
    {
        PrioritySnapshot = prioritySnapshot ?? throw new ArgumentNullException(nameof(prioritySnapshot));
        ArgumentNullException.ThrowIfNull(transitionPolicies);
        ArgumentNullException.ThrowIfNull(observations);
        ArgumentNullException.ThrowIfNull(eligibilityBindings);
        ArgumentNullException.ThrowIfNull(enforcementObservations);
        if (observedAt < PrioritySnapshot.ObservedAt) throw new ArgumentException("Pressure snapshot cannot predate its priority-governance predecessor.", nameof(observedAt));
        ObservedAt = observedAt;

        var policies = transitionPolicies.OrderBy(item => item.ResourceClassId.Value, StringComparer.Ordinal).ToArray();
        RejectDuplicate(policies.Select(item => item.ResourceClassId.Value), "resource pressure transition policy");
        foreach (var policy in policies)
        {
            _ = PrioritySnapshot.AllocationSnapshot.ResourceTruth.GetRequired(policy.ResourceClassId);
            ValidateEvidence(policy.Evidence, "transition policy");
            ValidateLifetime(policy.Lifetime, "transition policy");
        }

        var orderedObservations = observations.OrderBy(item => ObservationKey(item), StringComparer.Ordinal).ToArray();
        RejectDuplicate(orderedObservations.Select(ObservationKey), "pressure observation scope");

        var bindings = eligibilityBindings.OrderBy(item => item.GrantId.Value, StringComparer.Ordinal).ToArray();
        RejectDuplicate(bindings.Select(item => item.GrantId.Value), "preemption eligibility grant binding");
        foreach (var binding in bindings)
        {
            var allocation = PrioritySnapshot.AllocationSnapshot.GetRequiredAllocation(binding.ApplicationId, binding.ResourceClassId);
            if (!StringComparer.Ordinal.Equals(allocation.GrantId.Value, binding.GrantId.Value)) throw new ArgumentException("Preemption eligibility binding grant does not match the current WP-03 allocation grant.", nameof(eligibilityBindings));
            ValidateEvidence(binding.Evidence, "preemption eligibility binding");
            ValidateLifetime(binding.Lifetime, "preemption eligibility binding");
        }

        var enforcement = enforcementObservations.OrderBy(item => EnforcementKey(item), StringComparer.Ordinal).ToArray();
        RejectDuplicate(enforcement.Select(EnforcementKey), "enforcement observation scope");
        foreach (var item in enforcement)
        {
            ValidateScope(item.ScopeKind, item.TechnicalScopeId, item.ResourceClassId, item.ApplicationId);
            ValidateEvidence(item.AuthorityEvidence, "enforcement observation");
        }

        var previousByKey = previousSnapshot is not null && StringComparer.Ordinal.Equals(previousSnapshot.EpochId.Value, EpochId.Value)
            ? previousSnapshot.Truth.ToDictionary(TruthKey, StringComparer.Ordinal)
            : new Dictionary<string, ResourcePressureTruth>(StringComparer.Ordinal);

        var truth = new List<ResourcePressureTruth>(orderedObservations.Length);
        foreach (var observation in orderedObservations)
        {
            ValidateScope(observation.ScopeKind, observation.TechnicalScopeId, observation.ResourceClassId, observation.ApplicationId);
            ValidateEvidence(observation.Evidence, "pressure observation");
            var policy = policies.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.ResourceClassId.Value, observation.ResourceClassId.Value)) ?? throw new ArgumentException($"Missing transition policy for resource class '{observation.ResourceClassId}'.", nameof(transitionPolicies));
            var utilization = ComputeUtilizationBasisPoints(observation);
            ResourcePressureState? rawState = utilization.HasValue ? DeriveRawState(utilization.Value, policy) : null;
            previousByKey.TryGetValue(ObservationKey(observation), out var previous);
            if (previous is not null && observation.Sequence <= previous.Sequence) throw new ArgumentException("Pressure observation sequence must advance monotonically for the same exact scope.", nameof(observations));
            var stableState = ApplyTransitionStability(rawState, utilization, previous, policy);
            var observedEnforcement = enforcement.SingleOrDefault(item => StringComparer.Ordinal.Equals(EnforcementKey(item), ObservationKey(observation)));
            var preemptionEligible = ResolvePreemptionEligibility(observation, stableState, bindings);
            truth.Add(new ResourcePressureTruth(observation.ScopeKind, observation.TechnicalScopeId, observation.ResourceClassId, observation.ApplicationId, observation.Sequence, stableState, utilization, observedEnforcement?.State ?? ResourceEnforcementObservationState.None, preemptionEligible, observation.Evidence, policy.PolicyVersion));
        }

        _truth = Array.AsReadOnly(truth.ToArray());
        IdentitySha256 = ComputeIdentity(_truth);
    }

    public ResourcePriorityGovernanceSnapshot PrioritySnapshot { get; }
    public ResourceEpochId EpochId => PrioritySnapshot.EpochId;
    public DateTimeOffset ObservedAt { get; }
    public IReadOnlyList<ResourcePressureTruth> Truth => _truth;
    public string IdentitySha256 { get; }

    public IReadOnlyList<ResourcePressureTruth> GetApplicationView(ApplicationPrincipalId applicationId)
    {
        ArgumentNullException.ThrowIfNull(applicationId);
        return Array.AsReadOnly(_truth.Where(item => item.ScopeKind == ResourcePressureScopeKind.ApplicationResource && StringComparer.Ordinal.Equals(item.ApplicationId?.Value, applicationId.Value)).ToArray());
    }

    private void ValidateScope(ResourcePressureScopeKind scopeKind, ResourceScopeId technicalScopeId, ResourceClassId resourceClassId, ApplicationPrincipalId? applicationId)
    {
        _ = PrioritySnapshot.AllocationSnapshot.ResourceTruth.GetRequired(resourceClassId);
        if (!PrioritySnapshot.TechnicalBindings.Any(item => StringComparer.Ordinal.Equals(item.TechnicalScopeId.Value, technicalScopeId.Value) && StringComparer.Ordinal.Equals(item.ResourceClassId.Value, resourceClassId.Value))) throw new ArgumentException("Pressure/enforcement scope lacks an exact accepted WP-04 technical-criticality binding.");
        if (scopeKind == ResourcePressureScopeKind.ApplicationResource)
        {
            if (applicationId is null) throw new ArgumentException("Application-bound pressure scope requires Application identity.");
            _ = PrioritySnapshot.AllocationSnapshot.GetRequiredAllocation(applicationId, resourceClassId);
            if (PrioritySnapshot.GetApplicationView(applicationId).Binding is null) throw new ArgumentException("Application-bound pressure scope lacks an accepted WP-04 Application-priority binding.");
        }
    }

    private int? ComputeUtilizationBasisPoints(ResourcePressureObservation observation)
    {
        if (observation.UsedCapacity is null) return null;
        var denominator = observation.ScopeKind == ResourcePressureScopeKind.FoundationResourceClass
            ? PrioritySnapshot.AllocationSnapshot.ResourceTruth.GetRequired(observation.ResourceClassId).TotalCapacity
            : PrioritySnapshot.AllocationSnapshot.GetRequiredAllocation(observation.ApplicationId!, observation.ResourceClassId).Ceiling;
        if (!StringComparer.Ordinal.Equals(observation.UsedCapacity.Unit, denominator.Unit)) throw new ArgumentException("Pressure observation unit does not match its authoritative resource denominator.");
        if (denominator.Amount <= 0m) return observation.UsedCapacity.Amount == 0m ? 0 : 10_000;
        var value = decimal.Round(observation.UsedCapacity.Amount / denominator.Amount * 10_000m, 0, MidpointRounding.AwayFromZero);
        return decimal.ToInt32(Math.Clamp(value, 0m, 10_000m));
    }

    private static ResourcePressureState DeriveRawState(int utilizationBasisPoints, ResourcePressureTransitionPolicy policy)
    {
        if (utilizationBasisPoints >= policy.CriticalThresholdBasisPoints) return ResourcePressureState.Critical;
        if (utilizationBasisPoints >= policy.DegradedThresholdBasisPoints) return ResourcePressureState.Degraded;
        if (utilizationBasisPoints >= policy.ConstrainedThresholdBasisPoints) return ResourcePressureState.Constrained;
        return ResourcePressureState.Normal;
    }

    private static ResourcePressureState? ApplyTransitionStability(ResourcePressureState? rawState, int? utilizationBasisPoints, ResourcePressureTruth? previous, ResourcePressureTransitionPolicy policy)
    {
        if (!rawState.HasValue || !utilizationBasisPoints.HasValue) return null;
        if (previous is null || !previous.State.HasValue) return rawState;
        if ((int)rawState.Value >= (int)previous.State.Value) return rawState;
        var recoveryBoundary = previous.State.Value switch
        {
            ResourcePressureState.Critical => policy.CriticalThresholdBasisPoints - policy.RecoveryHysteresisBasisPoints,
            ResourcePressureState.Degraded => policy.DegradedThresholdBasisPoints - policy.RecoveryHysteresisBasisPoints,
            ResourcePressureState.Constrained => policy.ConstrainedThresholdBasisPoints - policy.RecoveryHysteresisBasisPoints,
            _ => 0
        };
        return utilizationBasisPoints.Value < recoveryBoundary ? rawState : previous.State;
    }

    private bool ResolvePreemptionEligibility(ResourcePressureObservation observation, ResourcePressureState? state, IEnumerable<ResourcePreemptionEligibilityBinding> bindings)
    {
        if (observation.ScopeKind != ResourcePressureScopeKind.ApplicationResource || observation.ApplicationId is null || !state.HasValue) return false;
        if (state.Value != ResourcePressureState.Degraded && state.Value != ResourcePressureState.Critical) return false;
        var allocation = PrioritySnapshot.AllocationSnapshot.GetRequiredAllocation(observation.ApplicationId, observation.ResourceClassId);
        var binding = bindings.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.GrantId.Value, allocation.GrantId.Value));
        return binding is not null && (binding.Reclaimability == ResourceReclaimability.Reclaimable || binding.Reclaimability == ResourceReclaimability.Temporary);
    }

    private void ValidateEvidence(ResourceEvidenceReference evidence, string subject)
    {
        if (!StringComparer.Ordinal.Equals(evidence.EpochId.Value, EpochId.Value)) throw new ArgumentException($"{subject} evidence epoch does not match the current resource epoch.");
        if (evidence.ObservedAt > ObservedAt) throw new ArgumentException($"{subject} evidence cannot be future-dated relative to the pressure snapshot.");
    }

    private void ValidateLifetime(ResourceEffectiveLifetime lifetime, string subject)
    {
        if (lifetime.EffectiveFrom > ObservedAt) throw new ArgumentException($"{subject} cannot become effective after the pressure snapshot observation.");
        if (lifetime.EffectiveUntil.HasValue && lifetime.EffectiveUntil.Value < ObservedAt) throw new ArgumentException($"Expired {subject} cannot appear in the current pressure snapshot.");
    }

    private string ComputeIdentity(IReadOnlyList<ResourcePressureTruth> truth)
    {
        var fields = new List<CanonicalIdentityField>
        {
            new("priority_snapshot_identity", PrioritySnapshot.IdentitySha256),
            CanonicalResourceIdentity.IdentifierField("epoch", EpochId),
            new("observed_at", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new("truth_count", truth.Count.ToString(CultureInfo.InvariantCulture))
        };
        for (var index = 0; index < truth.Count; index++)
        {
            var item = truth[index];
            var prefix = $"truth_{index:D4}_";
            fields.Add(new(prefix + "scope_kind", item.ScopeKind.ToString()));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "technical_scope", item.TechnicalScopeId));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "resource_class", item.ResourceClassId));
            fields.Add(new(prefix + "application", item.ApplicationId?.Value));
            fields.Add(new(prefix + "sequence", item.Sequence.ToString(CultureInfo.InvariantCulture)));
            fields.Add(new(prefix + "pressure_available", item.PressureAvailable ? "true" : "false"));
            fields.Add(new(prefix + "pressure_state", item.State?.ToString()));
            fields.Add(new(prefix + "utilization_bps", item.UtilizationBasisPoints?.ToString(CultureInfo.InvariantCulture)));
            fields.Add(new(prefix + "enforcement_state", item.EnforcementState.ToString()));
            fields.Add(new(prefix + "preemption_eligible", item.PreemptionEligibleForConsideration ? "true" : "false"));
            fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "observation_evidence", item.ObservationEvidence.EvidenceId));
            fields.Add(new(prefix + "transition_policy_version", item.TransitionPolicyVersion));
        }
        return CanonicalResourceIdentity.ComputeSha256(fields);
    }

    private static string ObservationKey(ResourcePressureObservation item) => $"{item.ScopeKind}|{item.ApplicationId?.Value ?? string.Empty}|{item.TechnicalScopeId.Value}|{item.ResourceClassId.Value}";
    private static string EnforcementKey(ResourceEnforcementObservation item) => $"{item.ScopeKind}|{item.ApplicationId?.Value ?? string.Empty}|{item.TechnicalScopeId.Value}|{item.ResourceClassId.Value}";
    private static string TruthKey(ResourcePressureTruth item) => $"{item.ScopeKind}|{item.ApplicationId?.Value ?? string.Empty}|{item.TechnicalScopeId.Value}|{item.ResourceClassId.Value}";

    private static void RejectDuplicate(IEnumerable<string> keys, string subject)
    {
        var duplicate = keys.GroupBy(item => item, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null) throw new ArgumentException($"Duplicate {subject} '{duplicate.Key}' is not allowed.");
    }
}
