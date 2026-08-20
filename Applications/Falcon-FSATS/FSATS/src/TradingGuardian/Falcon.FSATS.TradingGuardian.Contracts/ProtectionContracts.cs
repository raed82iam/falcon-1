namespace Falcon.FSATS.TradingGuardian.Contracts;

public enum ProtectionCommandType { AiKill, NewRiskFreeze, OrderEntryKill, CancelWorkingEntries, EmergencyPositionExit }
public enum ProtectionOutcomeState { Received, Accepted, Rejected, Applied, PartiallyApplied, Expired, Revoked, DispatchFailed, ReconciliationRequired }
public enum ProtectionTargetKind { Application, Broker, BrokerAccount, ExecutionRoute, Order, Position }
public readonly record struct CommandId(string Value);
public readonly record struct ProtectionEpoch(long Value);

public sealed record ProtectionTarget
{
    public ProtectionTargetKind Kind { get; }
    public string? BrokerId { get; }
    public string? BrokerAccountId { get; }
    public string? Environment { get; }
    public string? ExecutionRouteId { get; }
    public string? OrderId { get; }
    public string? PositionId { get; }

    public ProtectionTarget(
        ProtectionTargetKind kind,
        string? brokerId = null,
        string? brokerAccountId = null,
        string? environment = null,
        string? executionRouteId = null,
        string? orderId = null,
        string? positionId = null)
    {
        Kind = kind;
        BrokerId = Normalize(brokerId)?.ToUpperInvariant();
        BrokerAccountId = Normalize(brokerAccountId);
        Environment = Normalize(environment)?.ToUpperInvariant();
        ExecutionRouteId = Normalize(executionRouteId);
        OrderId = Normalize(orderId);
        PositionId = Normalize(positionId);

        if (!IsStructurallyValid()) throw new ArgumentException("INVALID_PROTECTION_TARGET");
    }

    public bool IsStructurallyValid()
        => Kind switch
        {
            ProtectionTargetKind.Application =>
                BrokerId is null && BrokerAccountId is null && Environment is null && ExecutionRouteId is null && OrderId is null && PositionId is null,
            ProtectionTargetKind.Broker =>
                BrokerId is not null && Environment is not null && BrokerAccountId is null && ExecutionRouteId is null && OrderId is null && PositionId is null,
            ProtectionTargetKind.BrokerAccount =>
                BrokerId is not null && BrokerAccountId is not null && Environment is not null && ExecutionRouteId is null && OrderId is null && PositionId is null,
            ProtectionTargetKind.ExecutionRoute =>
                BrokerId is not null && BrokerAccountId is not null && Environment is not null && ExecutionRouteId is not null && OrderId is null && PositionId is null,
            ProtectionTargetKind.Order =>
                BrokerId is not null && BrokerAccountId is not null && Environment is not null && ExecutionRouteId is not null && OrderId is not null && PositionId is null,
            ProtectionTargetKind.Position =>
                BrokerId is not null && BrokerAccountId is not null && Environment is not null && ExecutionRouteId is null && OrderId is null && PositionId is not null,
            _ => false
        };

    public string CanonicalKey => string.Join('|',
        Kind,
        Part(BrokerId),
        Part(BrokerAccountId),
        Part(Environment),
        Part(ExecutionRouteId),
        Part(OrderId),
        Part(PositionId));

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string Part(string? value) => Uri.EscapeDataString(value ?? string.Empty);
}

public sealed record ProtectionCommand(
    CommandId CommandId,
    ProtectionCommandType Type,
    string TargetApplication,
    ProtectionTarget Target,
    string AuthorityBasis,
    string ReasonCode,
    ProtectionEpoch Epoch,
    DateTimeOffset EffectiveAt,
    DateTimeOffset? ExpiresAt,
    string CorrelationId,
    string CausationId);

public sealed record ProtectionCommandOutcome(
    CommandId CommandId,
    ProtectionOutcomeState State,
    string TargetApplication,
    ProtectionTarget Target,
    string ReasonCode,
    DateTimeOffset EffectiveAt,
    string CorrelationId,
    string RequestFingerprint = "",
    string EvidenceReference = "");

public sealed record IncidentEvidence(
    string IncidentId,
    string SourceApplication,
    string Classification,
    string Severity,
    string EvidenceReference,
    DateTimeOffset ObservedAt);
