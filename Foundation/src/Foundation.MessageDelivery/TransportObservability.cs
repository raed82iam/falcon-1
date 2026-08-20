using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.MessageDelivery;

public enum TransportLatencyQuality
{
    Complete = 1,
    Partial = 2,
    Insufficient = 3,
    Invalid = 4
}

public enum TransportDeadlineStatus
{
    NotEvaluated = 1,
    WithinDeadline = 2,
    AfterDeadline = 3
}

public static class TransportObservabilityReason
{
    public const string Accepted = "TRANSPORT_LATENCY_SAMPLE_ACCEPTED";
    public const string InvalidInput = "TRANSPORT_LATENCY_INVALID_INPUT";
    public const string DecisionNotDispatchable = "TRANSPORT_LATENCY_DECISION_NOT_DISPATCHABLE";
    public const string DecisionOutcomeMismatch = "TRANSPORT_LATENCY_DECISION_OUTCOME_MISMATCH";
    public const string NegativeDuration = "TRANSPORT_LATENCY_NEGATIVE_DURATION";
    public const string InvalidDeadline = "TRANSPORT_LATENCY_INVALID_DEADLINE";
    public const string InvalidAggregateScope = "TRANSPORT_LATENCY_INVALID_AGGREGATE_SCOPE";
    public const string AggregateComplete = "TRANSPORT_LATENCY_AGGREGATE_COMPLETE";
    public const string AggregatePartial = "TRANSPORT_LATENCY_AGGREGATE_PARTIAL";
    public const string AggregateInsufficient = "TRANSPORT_LATENCY_AGGREGATE_INSUFFICIENT";
}

public sealed record TransportLatencySample
{
    internal TransportLatencySample(
        string sampleId,
        string deliveryDecisionId,
        string outcomeId,
        string routeDecisionId,
        string correlationId,
        string causationId,
        string policyId,
        string policyVersion,
        int attemptNumber,
        DateTimeOffset dispatchObservedAt,
        DateTimeOffset outcomeObservedAt,
        TimeSpan observedLatency,
        DateTimeOffset? deadline,
        TransportDeadlineStatus deadlineStatus,
        string decisionEvidenceReference,
        string outcomeEvidenceReference)
    {
        SampleId = sampleId;
        DeliveryDecisionId = deliveryDecisionId;
        OutcomeId = outcomeId;
        RouteDecisionId = routeDecisionId;
        CorrelationId = correlationId;
        CausationId = causationId;
        PolicyId = policyId;
        PolicyVersion = policyVersion;
        AttemptNumber = attemptNumber;
        DispatchObservedAt = dispatchObservedAt;
        OutcomeObservedAt = outcomeObservedAt;
        ObservedLatency = observedLatency;
        Deadline = deadline;
        DeadlineStatus = deadlineStatus;
        DecisionEvidenceReference = decisionEvidenceReference;
        OutcomeEvidenceReference = outcomeEvidenceReference;
    }

    public string SampleId { get; }
    public string DeliveryDecisionId { get; }
    public string OutcomeId { get; }
    public string RouteDecisionId { get; }
    public string CorrelationId { get; }
    public string CausationId { get; }
    public string PolicyId { get; }
    public string PolicyVersion { get; }
    public int AttemptNumber { get; }
    public DateTimeOffset DispatchObservedAt { get; }
    public DateTimeOffset OutcomeObservedAt { get; }
    public TimeSpan ObservedLatency { get; }
    public DateTimeOffset? Deadline { get; }
    public TransportDeadlineStatus DeadlineStatus { get; }
    public string DecisionEvidenceReference { get; }
    public string OutcomeEvidenceReference { get; }
}

public sealed record TransportLatencySampleResult(bool Accepted, string Reason, TransportLatencySample? Sample);

public sealed class TransportLatencySampleFactory
{
    public TransportLatencySampleResult Create(
        DeliveryDecision? decision,
        DeliveryAttemptOutcome? outcome,
        DateTimeOffset? deadline = null)
    {
        if (decision is null || outcome is null)
            return new(false, TransportObservabilityReason.InvalidInput, null);

        if (!decision.CanDispatch)
            return new(false, TransportObservabilityReason.DecisionNotDispatchable, null);

        if (!BindingsMatch(decision, outcome))
            return new(false, TransportObservabilityReason.DecisionOutcomeMismatch, null);

        if (outcome.ObservationTime < decision.ObservationTime)
            return new(false, TransportObservabilityReason.NegativeDuration, null);

        if (deadline.HasValue)
        {
            if (deadline.Value.Offset != TimeSpan.Zero || deadline.Value <= decision.ObservationTime)
                return new(false, TransportObservabilityReason.InvalidDeadline, null);
        }

        var latency = outcome.ObservationTime - decision.ObservationTime;
        var deadlineStatus = !deadline.HasValue
            ? TransportDeadlineStatus.NotEvaluated
            : outcome.ObservationTime < deadline.Value
                ? TransportDeadlineStatus.WithinDeadline
                : TransportDeadlineStatus.AfterDeadline;

        var sampleId = Hash(
            ("delivery_decision_id", decision.DecisionId),
            ("outcome_id", outcome.OutcomeId),
            ("route_decision_id", decision.RouteDecisionId),
            ("correlation_id", decision.CorrelationId),
            ("causation_id", decision.CausationId),
            ("policy_id", decision.PolicyId),
            ("policy_version", decision.PolicyVersion),
            ("attempt_number", decision.AttemptNumber.ToString(CultureInfo.InvariantCulture)),
            ("dispatch_observed_at", decision.ObservationTime.ToString("O", CultureInfo.InvariantCulture)),
            ("outcome_observed_at", outcome.ObservationTime.ToString("O", CultureInfo.InvariantCulture)),
            ("latency_ticks", latency.Ticks.ToString(CultureInfo.InvariantCulture)),
            ("deadline", deadline?.ToString("O", CultureInfo.InvariantCulture) ?? "NONE"),
            ("decision_evidence", decision.EvidenceReference),
            ("outcome_evidence", outcome.EvidenceReference));

        return new(true, TransportObservabilityReason.Accepted, new TransportLatencySample(
            sampleId,
            decision.DecisionId,
            outcome.OutcomeId,
            decision.RouteDecisionId,
            decision.CorrelationId,
            decision.CausationId,
            decision.PolicyId,
            decision.PolicyVersion,
            decision.AttemptNumber,
            decision.ObservationTime,
            outcome.ObservationTime,
            latency,
            deadline,
            deadlineStatus,
            decision.EvidenceReference,
            outcome.EvidenceReference));
    }

    private static bool BindingsMatch(DeliveryDecision decision, DeliveryAttemptOutcome outcome) =>
        StringComparer.Ordinal.Equals(decision.DecisionId, outcome.DeliveryDecisionId) &&
        StringComparer.Ordinal.Equals(decision.RouteDecisionId, outcome.RouteDecisionId) &&
        StringComparer.Ordinal.Equals(decision.CorrelationId, outcome.CorrelationId) &&
        StringComparer.Ordinal.Equals(decision.CausationId, outcome.CausationId) &&
        StringComparer.Ordinal.Equals(decision.PolicyId, outcome.PolicyId) &&
        StringComparer.Ordinal.Equals(decision.PolicyVersion, outcome.PolicyVersion) &&
        decision.AttemptNumber == outcome.AttemptNumber;

    private static string Hash(params (string Key, string Value)[] fields)
    {
        var canonical = string.Join("\n", fields.Select(field => $"{field.Key}={field.Value}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}

public sealed record TransportLatencyAggregate
{
    internal TransportLatencyAggregate(
        string scope,
        TransportLatencyQuality quality,
        string reason,
        int validSampleCount,
        int rejectedSampleCount,
        TimeSpan? minimumObservedLatency,
        TimeSpan? maximumObservedLatency,
        TimeSpan? p50ObservedLatency,
        TimeSpan? p95ObservedLatency,
        TimeSpan? p99ObservedLatency,
        int withinDeadlineCount,
        int afterDeadlineCount,
        int deadlineNotEvaluatedCount,
        IReadOnlyList<string> routeDecisionIds,
        string evidenceIdentity)
    {
        Scope = scope;
        Quality = quality;
        Reason = reason;
        ValidSampleCount = validSampleCount;
        RejectedSampleCount = rejectedSampleCount;
        MinimumObservedLatency = minimumObservedLatency;
        MaximumObservedLatency = maximumObservedLatency;
        P50ObservedLatency = p50ObservedLatency;
        P95ObservedLatency = p95ObservedLatency;
        P99ObservedLatency = p99ObservedLatency;
        WithinDeadlineCount = withinDeadlineCount;
        AfterDeadlineCount = afterDeadlineCount;
        DeadlineNotEvaluatedCount = deadlineNotEvaluatedCount;
        RouteDecisionIds = routeDecisionIds;
        EvidenceIdentity = evidenceIdentity;
    }

    public string Scope { get; }
    public TransportLatencyQuality Quality { get; }
    public string Reason { get; }
    public int ValidSampleCount { get; }
    public int RejectedSampleCount { get; }
    public TimeSpan? MinimumObservedLatency { get; }
    public TimeSpan? MaximumObservedLatency { get; }
    public TimeSpan? P50ObservedLatency { get; }
    public TimeSpan? P95ObservedLatency { get; }
    public TimeSpan? P99ObservedLatency { get; }
    public int WithinDeadlineCount { get; }
    public int AfterDeadlineCount { get; }
    public int DeadlineNotEvaluatedCount { get; }
    public IReadOnlyList<string> RouteDecisionIds { get; }
    public string EvidenceIdentity { get; }

    public bool IsComplete => Quality == TransportLatencyQuality.Complete;
    public bool HasUsableSamples => ValidSampleCount > 0;
}

public sealed class TransportLatencyAggregator
{
    public TransportLatencyAggregate Aggregate(
        string? scope,
        IEnumerable<TransportLatencySampleResult>? observations)
    {
        if (!IsCanonicalToken(scope))
            return Invalid(scope ?? string.Empty);

        var input = observations?.ToArray() ?? Array.Empty<TransportLatencySampleResult>();
        var accepted = new List<TransportLatencySample>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var rejectedCount = 0;

        foreach (var result in input)
        {
            if (!result.Accepted || result.Sample is null)
            {
                rejectedCount++;
                continue;
            }

            if (!seen.Add(result.Sample.SampleId))
            {
                rejectedCount++;
                continue;
            }

            accepted.Add(result.Sample);
        }

        accepted.Sort(static (left, right) => StringComparer.Ordinal.Compare(left.SampleId, right.SampleId));

        if (accepted.Count == 0)
        {
            return new TransportLatencyAggregate(
                scope!,
                TransportLatencyQuality.Insufficient,
                TransportObservabilityReason.AggregateInsufficient,
                0,
                rejectedCount,
                null,
                null,
                null,
                null,
                null,
                0,
                0,
                0,
                Array.Empty<string>(),
                BuildAggregateIdentity(scope!, accepted, rejectedCount, TransportLatencyQuality.Insufficient));
        }

        var orderedTicks = accepted.Select(sample => sample.ObservedLatency.Ticks).OrderBy(value => value).ToArray();
        var quality = rejectedCount == 0 ? TransportLatencyQuality.Complete : TransportLatencyQuality.Partial;
        var reason = quality == TransportLatencyQuality.Complete
            ? TransportObservabilityReason.AggregateComplete
            : TransportObservabilityReason.AggregatePartial;

        var routes = accepted
            .Select(sample => sample.RouteDecisionId)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        return new TransportLatencyAggregate(
            scope!,
            quality,
            reason,
            accepted.Count,
            rejectedCount,
            TimeSpan.FromTicks(orderedTicks[0]),
            TimeSpan.FromTicks(orderedTicks[^1]),
            TimeSpan.FromTicks(NearestRank(orderedTicks, 0.50m)),
            TimeSpan.FromTicks(NearestRank(orderedTicks, 0.95m)),
            TimeSpan.FromTicks(NearestRank(orderedTicks, 0.99m)),
            accepted.Count(sample => sample.DeadlineStatus == TransportDeadlineStatus.WithinDeadline),
            accepted.Count(sample => sample.DeadlineStatus == TransportDeadlineStatus.AfterDeadline),
            accepted.Count(sample => sample.DeadlineStatus == TransportDeadlineStatus.NotEvaluated),
            routes,
            BuildAggregateIdentity(scope!, accepted, rejectedCount, quality));
    }

    private static long NearestRank(IReadOnlyList<long> orderedTicks, decimal percentile)
    {
        var rank = (int)Math.Ceiling(percentile * orderedTicks.Count);
        rank = Math.Clamp(rank, 1, orderedTicks.Count);
        return orderedTicks[rank - 1];
    }

    private static TransportLatencyAggregate Invalid(string scope) => new(
        scope,
        TransportLatencyQuality.Invalid,
        TransportObservabilityReason.InvalidAggregateScope,
        0,
        0,
        null,
        null,
        null,
        null,
        null,
        0,
        0,
        0,
        Array.Empty<string>(),
        string.Empty);

    private static string BuildAggregateIdentity(
        string scope,
        IReadOnlyList<TransportLatencySample> samples,
        int rejectedCount,
        TransportLatencyQuality quality)
    {
        var lines = new List<string>
        {
            $"scope={scope}",
            $"quality={(int)quality}",
            $"valid_count={samples.Count.ToString(CultureInfo.InvariantCulture)}",
            $"rejected_count={rejectedCount.ToString(CultureInfo.InvariantCulture)}"
        };

        lines.AddRange(samples
            .OrderBy(sample => sample.SampleId, StringComparer.Ordinal)
            .Select(sample => $"sample={sample.SampleId}|latency_ticks={sample.ObservedLatency.Ticks.ToString(CultureInfo.InvariantCulture)}|deadline={(int)sample.DeadlineStatus}|route={sample.RouteDecisionId}"));

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n", lines))));
    }

    private static bool IsCanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 160) return false;
        if (!StringComparer.Ordinal.Equals(value, value.Trim())) return false;
        return value.All(character =>
            char.IsAsciiLetterOrDigit(character) ||
            character is '-' or '_' or ':' or '/' or '.');
    }
}
