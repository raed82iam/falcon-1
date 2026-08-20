namespace Falcon.FSATS.ResourceManagement.Contracts;

public enum CoordinationAction { Keep, Reclaim, Reduce, Reassign, Restore, RequestAdditional }
public enum CoordinationOutcomeState { Accepted, Rejected, PartiallyApplied, Deferred, Failed, ReconciliationRequired }
public readonly record struct CoordinationEpoch(long Value);

public sealed record ConstituentResourceEvidence(
    string ApplicationId,
    string ResourceClass,
    decimal CurrentAllocation,
    decimal CurrentConsumption,
    decimal MinimumSafeRequirement,
    decimal DesiredCapacity,
    decimal ReclaimableCapacity,
    int Urgency,
    string DegradationOptions,
    string ConsequenceOfStarvation,
    DateTimeOffset ObservedAt);

public sealed record CoordinationOutcome(
    string DecisionId,
    string TargetApplication,
    string ResourceClass,
    CoordinationAction Action,
    decimal Amount,
    CoordinationEpoch Epoch,
    string FoundationEnvelopeReference,
    DateTimeOffset EffectiveAt,
    DateTimeOffset? ExpiresAt);

public sealed record CoordinationAcknowledgement(string DecisionId, CoordinationOutcomeState State, decimal ResultingConsumption, string ReasonCode);

public sealed record ResidualResourceRequest(
    string RequestId,
    IReadOnlyList<string> ConstituentApplications,
    string ResourceClass,
    decimal ProvenResidualNeed,
    decimal CoordinationOverhead,
    string InternalOptimizationEvidence,
    string FoundationEnvelopeReference);
