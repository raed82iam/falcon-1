namespace Falcon.FSATS.ResourceManagement.Application;

public enum FoundationPressureState
{
    Normal = 0,
    Constrained = 1,
    Degraded = 2,
    Critical = 3,
    Unavailable = 4
}

public enum FoundationLoadSheddingClass
{
    NoAction = 0,
    AdvisoryReduction = 1,
    ComplianceReductionRequired = 2,
    StateUnavailable = 3
}

public enum FoundationResourceDecisionKind
{
    Grant = 0,
    PartialGrant = 1,
    Cap = 2,
    Deny = 3,
    Defer = 4,
    Revoke = 5,
    Reduce = 6,
    Restore = 7
}

public sealed record FoundationResourceStateProjection(
    string ApplicationId,
    string ResourceClass,
    string EpochId,
    string GrantId,
    decimal Allocation,
    decimal Quota,
    decimal Ceiling,
    decimal? EffectiveCapacity,
    bool PressureAvailable,
    FoundationPressureState PressureState,
    int? UtilizationBasisPoints,
    bool PreemptionEligibleForConsideration,
    string? AdditionalDecisionReference,
    string? AcceptedCapacityBasisReference,
    DateTimeOffset ObservedAt,
    string FoundationIdentitySha256);

public sealed record FoundationLoadSheddingSignal(
    string ApplicationId,
    string ResourceClass,
    string EpochId,
    FoundationLoadSheddingClass SignalClass,
    decimal? CompliantCapacityTarget,
    decimal? RequiredReduction,
    string ProjectionIdentitySha256,
    string? AcceptedCapacityBasisIdentitySha256,
    DateTimeOffset GeneratedAt,
    string FoundationIdentitySha256);

public sealed record FoundationAdditionalResourceRequest(
    string RequestId,
    string CoordinatorApplicationId,
    string CoordinatorInstanceId,
    string CoordinatorRoleId,
    string ResourceClass,
    decimal ProvenResidualNeed,
    string Unit,
    string EpochId,
    string CoordinationScopeId,
    string EvidenceReference,
    string InternalOptimizationEvidence,
    string CorrelationId,
    string CausationId,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt);

public sealed record FoundationAdditionalResourceOutcome(
    bool Bound,
    FoundationResourceDecisionKind Decision,
    decimal GrantedAmount,
    string Unit,
    string DecisionId,
    string RequestId,
    string EpochId,
    string FoundationOutcomeReference,
    DateTimeOffset DecidedAt,
    DateTimeOffset ExpiresAt);

public static class FoundationResourceBindingGuards
{
    // Application-side fail-closed freshness ceiling used only when consuming a Foundation
    // projection. It is not a Foundation allocation-policy statement and can be tightened by
    // an explicitly governed runtime binding.
    public static readonly TimeSpan DefaultProjectionMaximumAge = TimeSpan.FromMinutes(5);

    public static bool IsUsable(FoundationResourceStateProjection projection, string expectedApplicationId, string expectedResourceClass, string expectedEpochId, DateTimeOffset now)
        => IsUsable(projection, expectedApplicationId, expectedResourceClass, expectedEpochId, now, DefaultProjectionMaximumAge);

    public static bool IsUsable(
        FoundationResourceStateProjection projection,
        string expectedApplicationId,
        string expectedResourceClass,
        string expectedEpochId,
        DateTimeOffset now,
        TimeSpan maximumAge)
        => maximumAge > TimeSpan.Zero
           && projection.ApplicationId == expectedApplicationId
           && projection.ResourceClass == expectedResourceClass
           && projection.EpochId == expectedEpochId
           && projection.ObservedAt != default
           && projection.ObservedAt <= now
           && now - projection.ObservedAt <= maximumAge
           && !string.IsNullOrWhiteSpace(projection.FoundationIdentitySha256)
           && projection.Allocation >= 0m
           && projection.Quota >= 0m
           && projection.Ceiling >= 0m
           && projection.Allocation <= projection.Ceiling
           && (!projection.EffectiveCapacity.HasValue || projection.EffectiveCapacity.Value >= 0m);

    public static bool IsCurrent(FoundationLoadSheddingSignal signal, FoundationResourceStateProjection projection, DateTimeOffset now)
        => signal.ApplicationId == projection.ApplicationId
           && signal.ResourceClass == projection.ResourceClass
           && signal.EpochId == projection.EpochId
           && signal.ProjectionIdentitySha256 == projection.FoundationIdentitySha256
           && signal.GeneratedAt != default
           && signal.GeneratedAt >= projection.ObservedAt
           && signal.GeneratedAt <= now
           && !string.IsNullOrWhiteSpace(signal.FoundationIdentitySha256);

    public static bool OutcomeMatches(FoundationAdditionalResourceRequest request, FoundationAdditionalResourceOutcome outcome, DateTimeOffset now)
        => outcome.Bound
           && IsWp06AdditionalRequestDecision(outcome.Decision)
           && outcome.RequestId == request.RequestId
           && outcome.EpochId == request.EpochId
           && outcome.DecidedAt != default
           && outcome.DecidedAt >= request.RequestedAt
           && outcome.DecidedAt <= now
           && outcome.ExpiresAt > now
           && outcome.GrantedAmount >= 0m
           && outcome.GrantedAmount <= request.ProvenResidualNeed
           && outcome.Unit == request.Unit
           && !string.IsNullOrWhiteSpace(outcome.DecisionId)
           && !string.IsNullOrWhiteSpace(outcome.FoundationOutcomeReference);

    public static bool IsWp06AdditionalRequestDecision(FoundationResourceDecisionKind decision)
        => decision is FoundationResourceDecisionKind.Grant
            or FoundationResourceDecisionKind.PartialGrant
            or FoundationResourceDecisionKind.Cap
            or FoundationResourceDecisionKind.Deny
            or FoundationResourceDecisionKind.Defer;
}
