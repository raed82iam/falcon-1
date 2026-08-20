using System.Reflection;
using Foundation.MessageDelivery;

namespace Falcon.Stage11.TransportObservability.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset BaseTime = new(2026, 8, 16, 6, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        var checks = new List<(string Name, Func<bool> Check)>
        {
            ("positive_sample_accepted", PositiveSampleAccepted),
            ("negative_duration_rejected", NegativeDurationRejected),
            ("route_binding_mismatch_rejected", RouteBindingMismatchRejected),
            ("decision_binding_mismatch_rejected", DecisionBindingMismatchRejected),
            ("policy_binding_mismatch_rejected", PolicyBindingMismatchRejected),
            ("attempt_binding_mismatch_rejected", AttemptBindingMismatchRejected),
            ("invalid_deadline_rejected", InvalidDeadlineRejected),
            ("deadline_within_observed", DeadlineWithinObserved),
            ("deadline_after_observed", DeadlineAfterObserved),
            ("aggregate_percentiles_nearest_rank", AggregatePercentilesNearestRank),
            ("aggregate_reorder_deterministic", AggregateReorderDeterministic),
            ("duplicate_sample_does_not_bias", DuplicateSampleDoesNotBias),
            ("partial_quality_explicit", PartialQualityExplicit),
            ("empty_set_is_insufficient", EmptySetIsInsufficient),
            ("invalid_scope_fails_closed", InvalidScopeFailsClosed),
            ("aggregate_identity_is_sha256", AggregateIdentityIsSha256),
            ("observability_has_no_execution_surface", ObservabilityHasNoExecutionSurface),
            ("application_specific_semantics_absent", ApplicationSpecificSemanticsAbsent),
            ("ops001_required_boundaries_present", Ops001RequiredBoundariesPresent),
            ("zero_application_operation_remains_valid", ZeroApplicationOperationRemainsValid)
        };

        var failures = new List<string>();
        foreach (var (name, check) in checks)
        {
            try
            {
                if (check())
                {
                    Console.WriteLine($"PASS {name}");
                }
                else
                {
                    Console.WriteLine($"FAIL {name}");
                    failures.Add(name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}");
                failures.Add(name);
            }
        }

        Console.WriteLine();
        Console.WriteLine($"STAGE11_TRANSPORT_QOS_OBSERVABILITY_VERIFIER = {(failures.Count == 0 ? "PASS" : "FAIL")}");
        Console.WriteLine($"CHECKS = {checks.Count - failures.Count}/{checks.Count}");
        Console.WriteLine($"P50_P95_P99 = {(AggregatePercentilesNearestRank() ? "PASS" : "FAIL")}");
        Console.WriteLine($"ADVERSARIAL_BINDING_AND_TIMING = {(failures.Count == 0 ? "PASS" : "FAIL")}");
        Console.WriteLine("OBSERVABILITY != AUTHORITY");
        Console.WriteLine("LATENCY_OBSERVATION != LATENCY_GUARANTEE");
        Console.WriteLine("QOS != BUSINESS_AUTHORITY");
        Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");

        return failures.Count == 0 ? 0 : 1;
    }

    private static bool PositiveSampleAccepted()
    {
        var result = MakeSample("01", 10);
        return result.Accepted && result.Sample is not null && result.Sample.ObservedLatency == TimeSpan.FromMilliseconds(10);
    }

    private static bool NegativeDurationRejected()
    {
        var decision = CreateDecision("neg", BaseTime.AddMilliseconds(20));
        var outcome = CreateOutcome("neg", BaseTime.AddMilliseconds(10));
        var result = new TransportLatencySampleFactory().Create(decision, outcome);
        return !result.Accepted && result.Reason == TransportObservabilityReason.NegativeDuration;
    }

    private static bool RouteBindingMismatchRejected()
    {
        var decision = CreateDecision("route", BaseTime, routeDecisionId: "route-decision-A");
        var outcome = CreateOutcome("route", BaseTime.AddMilliseconds(5), routeDecisionId: "route-decision-B");
        return RejectedAsMismatch(decision, outcome);
    }

    private static bool DecisionBindingMismatchRejected()
    {
        var decision = CreateDecision("decision", BaseTime, decisionId: "delivery-decision-A");
        var outcome = CreateOutcome("decision", BaseTime.AddMilliseconds(5), deliveryDecisionId: "delivery-decision-B");
        return RejectedAsMismatch(decision, outcome);
    }

    private static bool PolicyBindingMismatchRejected()
    {
        var decision = CreateDecision("policy", BaseTime, policyId: "policy-A");
        var outcome = CreateOutcome("policy", BaseTime.AddMilliseconds(5), policyId: "policy-B");
        return RejectedAsMismatch(decision, outcome);
    }

    private static bool AttemptBindingMismatchRejected()
    {
        var decision = CreateDecision("attempt", BaseTime, attemptNumber: 1);
        var outcome = CreateOutcome("attempt", BaseTime.AddMilliseconds(5), attemptNumber: 2);
        return RejectedAsMismatch(decision, outcome);
    }

    private static bool InvalidDeadlineRejected()
    {
        var decision = CreateDecision("deadline-invalid", BaseTime);
        var outcome = CreateOutcome("deadline-invalid", BaseTime.AddMilliseconds(5));
        var result = new TransportLatencySampleFactory().Create(decision, outcome, BaseTime);
        return !result.Accepted && result.Reason == TransportObservabilityReason.InvalidDeadline;
    }

    private static bool DeadlineWithinObserved()
    {
        var result = MakeSample("within", 10, 20);
        return result.Sample?.DeadlineStatus == TransportDeadlineStatus.WithinDeadline;
    }

    private static bool DeadlineAfterObserved()
    {
        var result = MakeSample("after", 30, 20);
        return result.Sample?.DeadlineStatus == TransportDeadlineStatus.AfterDeadline;
    }

    private static bool AggregatePercentilesNearestRank()
    {
        var results = new[]
        {
            MakeSample("p01", 10),
            MakeSample("p02", 20),
            MakeSample("p03", 30),
            MakeSample("p04", 40),
            MakeSample("p05", 100)
        };

        var aggregate = new TransportLatencyAggregator().Aggregate("route:aggregate", results);
        return aggregate.Quality == TransportLatencyQuality.Complete &&
               aggregate.ValidSampleCount == 5 &&
               aggregate.MinimumObservedLatency == TimeSpan.FromMilliseconds(10) &&
               aggregate.MaximumObservedLatency == TimeSpan.FromMilliseconds(100) &&
               aggregate.P50ObservedLatency == TimeSpan.FromMilliseconds(30) &&
               aggregate.P95ObservedLatency == TimeSpan.FromMilliseconds(100) &&
               aggregate.P99ObservedLatency == TimeSpan.FromMilliseconds(100);
    }

    private static bool AggregateReorderDeterministic()
    {
        var samples = new[] { MakeSample("r1", 10), MakeSample("r2", 30), MakeSample("r3", 20) };
        var aggregator = new TransportLatencyAggregator();
        var first = aggregator.Aggregate("route:reorder", samples);
        var second = aggregator.Aggregate("route:reorder", samples.Reverse());
        return first.EvidenceIdentity == second.EvidenceIdentity &&
               first.P95ObservedLatency == second.P95ObservedLatency;
    }

    private static bool DuplicateSampleDoesNotBias()
    {
        var fast = MakeSample("dup-fast", 1);
        var slow = MakeSample("dup-slow", 100);
        var aggregate = new TransportLatencyAggregator().Aggregate("route:dup", new[] { fast, fast, slow });
        return aggregate.ValidSampleCount == 2 &&
               aggregate.RejectedSampleCount == 1 &&
               aggregate.Quality == TransportLatencyQuality.Partial &&
               aggregate.P95ObservedLatency == TimeSpan.FromMilliseconds(100);
    }

    private static bool PartialQualityExplicit()
    {
        var valid = MakeSample("partial", 15);
        var invalid = new TransportLatencySampleResult(false, TransportObservabilityReason.InvalidInput, null);
        var aggregate = new TransportLatencyAggregator().Aggregate("route:partial", new[] { valid, invalid });
        return aggregate.Quality == TransportLatencyQuality.Partial && aggregate.ValidSampleCount == 1 && aggregate.RejectedSampleCount == 1;
    }

    private static bool EmptySetIsInsufficient()
    {
        var aggregate = new TransportLatencyAggregator().Aggregate("route:empty", Array.Empty<TransportLatencySampleResult>());
        return aggregate.Quality == TransportLatencyQuality.Insufficient &&
               aggregate.ValidSampleCount == 0 &&
               aggregate.P50ObservedLatency is null &&
               IsSha256(aggregate.EvidenceIdentity);
    }

    private static bool InvalidScopeFailsClosed()
    {
        var aggregate = new TransportLatencyAggregator().Aggregate(" invalid scope ", new[] { MakeSample("scope", 10) });
        return aggregate.Quality == TransportLatencyQuality.Invalid && string.IsNullOrEmpty(aggregate.EvidenceIdentity);
    }

    private static bool AggregateIdentityIsSha256()
    {
        var aggregate = new TransportLatencyAggregator().Aggregate("route:identity", new[] { MakeSample("identity", 10) });
        return IsSha256(aggregate.EvidenceIdentity);
    }

    private static bool ObservabilityHasNoExecutionSurface()
    {
        var forbidden = new[] { "Authorize", "Execute", "Dispatch", "Retry", "Deliver", "Allocate", "Grant", "Revoke" };
        var types = new[] { typeof(TransportLatencySampleFactory), typeof(TransportLatencyAggregator), typeof(TransportLatencyAggregate) };
        return types
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(method => !method.IsSpecialName)
            .All(method => forbidden.All(word => !method.Name.Contains(word, StringComparison.OrdinalIgnoreCase)));
    }

    private static bool ApplicationSpecificSemanticsAbsent()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "src", "Foundation.MessageDelivery", "TransportObservability.cs"));
        var forbidden = new[] { "FSATS", "Trading", "Broker", "Strategy", "CustomerId", "UserId", "Shared Web" };
        return forbidden.All(term => !source.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    private static bool Ops001RequiredBoundariesPresent()
    {
        var root = FindRepositoryRoot();
        var spec = File.ReadAllText(Path.Combine(root, "docs", "specifications", "ops", "OPS-001_OBSERVABILITY.md"));
        return spec.Contains("OBSERVABILITY != AUTHORITY", StringComparison.Ordinal) &&
               spec.Contains("LATENCY_OBSERVATION != LATENCY_GUARANTEE", StringComparison.Ordinal) &&
               spec.Contains("MISSING", StringComparison.OrdinalIgnoreCase) &&
               spec.Contains("p50", StringComparison.OrdinalIgnoreCase) &&
               spec.Contains("p95", StringComparison.OrdinalIgnoreCase) &&
               spec.Contains("p99", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ZeroApplicationOperationRemainsValid()
    {
        var aggregate = new TransportLatencyAggregator().Aggregate("foundation:zero-application", Array.Empty<TransportLatencySampleResult>());
        return aggregate.Quality == TransportLatencyQuality.Insufficient && aggregate.ValidSampleCount == 0;
    }

    private static bool RejectedAsMismatch(DeliveryDecision decision, DeliveryAttemptOutcome outcome)
    {
        var result = new TransportLatencySampleFactory().Create(decision, outcome);
        return !result.Accepted && result.Reason == TransportObservabilityReason.DecisionOutcomeMismatch;
    }

    private static TransportLatencySampleResult MakeSample(string suffix, double latencyMs, double? deadlineMs = null)
    {
        var start = BaseTime.AddSeconds(ParseOffset(suffix));
        var decision = CreateDecision(suffix, start);
        var outcome = CreateOutcome(suffix, start.AddMilliseconds(latencyMs));
        return new TransportLatencySampleFactory().Create(
            decision,
            outcome,
            deadlineMs.HasValue ? start.AddMilliseconds(deadlineMs.Value) : null);
    }

    private static int ParseOffset(string value)
    {
        var hash = 17;
        foreach (var character in value) hash = unchecked(hash * 31 + character);
        return Math.Abs(hash % 300);
    }

    private static DeliveryDecision CreateDecision(
        string suffix,
        DateTimeOffset observedAt,
        string? decisionId = null,
        string? routeDecisionId = null,
        string? policyId = null,
        int attemptNumber = 1)
    {
        var constructor = typeof(DeliveryDecision)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(candidate => candidate.GetParameters().Length == 25);

        return (DeliveryDecision)constructor.Invoke(new object[]
        {
            DeliveryDecisionKind.DispatchEligible,
            MessageDeliveryReason.DispatchEligible,
            decisionId ?? $"delivery-decision-{suffix}",
            routeDecisionId ?? $"route-decision-{suffix}",
            $"registry-digest-{suffix}",
            $"admission-{suffix}",
            $"message-{suffix}",
            $"correlation-{suffix}",
            $"causation-{suffix}",
            $"application-{suffix}",
            $"route-{suffix}",
            "1.0",
            policyId ?? $"policy-{suffix}",
            "1.0",
            DeliveryGuarantee.AtMostOnce,
            DeliveryOrderingGuarantee.None,
            "NONE",
            DeliveryTrafficClass.Normal,
            attemptNumber,
            "NONE",
            "NONE",
            DeliveryDestinationHealth.Healthy,
            "NONE",
            observedAt,
            $"evidence/decision/{suffix}"
        });
    }

    private static DeliveryAttemptOutcome CreateOutcome(
        string suffix,
        DateTimeOffset observedAt,
        string? deliveryDecisionId = null,
        string? routeDecisionId = null,
        string? policyId = null,
        int attemptNumber = 1)
    {
        var constructor = typeof(DeliveryAttemptOutcome)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(candidate => candidate.GetParameters().Length == 11);

        return (DeliveryAttemptOutcome)constructor.Invoke(new object[]
        {
            $"outcome-{suffix}",
            deliveryDecisionId ?? $"delivery-decision-{suffix}",
            routeDecisionId ?? $"route-decision-{suffix}",
            $"correlation-{suffix}",
            $"causation-{suffix}",
            policyId ?? $"policy-{suffix}",
            "1.0",
            attemptNumber,
            TransportObservationKind.RecipientAcknowledged,
            observedAt,
            $"evidence/outcome/{suffix}"
        });
    }

    private static bool IsSha256(string value) =>
        value.Length == 64 && value.All(character => char.IsAsciiHexDigit(character));

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")))
                return current.FullName;
            current = current.Parent;
        }
        throw new InvalidOperationException("Repository root could not be located.");
    }
}
