using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP04.Verifier;

internal static class EvidenceQualityPrecedenceGuard
{
    [ModuleInitializer]
    internal static void VerifyInvalidDominatesInsufficient()
    {
        var method = typeof(TechnicalFitnessEvaluationRuntime).GetMethod(
            "AggregateEvidenceQuality",
            BindingFlags.NonPublic | BindingFlags.Static);

        if (method is null)
            throw new InvalidOperationException("WP-04 evidence-quality aggregator not found.");

        var time = new DateTimeOffset(2026, 8, 13, 8, 0, 0, TimeSpan.Zero);
        var assertions = new[]
        {
            Assertion("selfmodel:assertion:precedence:invalid", EvidenceQuality.Invalid, time),
            Assertion("selfmodel:assertion:precedence:insufficient", EvidenceQuality.Insufficient, time)
        };

        var result = method.Invoke(null, new object[] { assertions });
        if (result is not EvidenceQuality.Invalid)
            throw new InvalidOperationException("WP-04 INVALID evidence did not dominate INSUFFICIENT evidence.");
    }

    private static FoundationSelfModelAssertion Assertion(
        string id,
        EvidenceQuality quality,
        DateTimeOffset time) =>
        new(
            id,
            "foundation",
            FoundationSelfModelArea.RuntimeCondition,
            FoundationSelfModelAssertionKind.Fact,
            FoundationSelfModelTemporalView.Current,
            "foundation",
            "technical:value:runtime:usable",
            "source:precedence",
            "owner:foundation:precedence",
            "evidence:precedence",
            quality,
            quality.ToString().ToUpperInvariant(),
            "precedence-regression",
            "freshness:precedence",
            "selfmodel:rule:precedence",
            "1.0",
            time.AddSeconds(-2),
            time.AddSeconds(-1),
            time.AddSeconds(20),
            null,
            null);
}