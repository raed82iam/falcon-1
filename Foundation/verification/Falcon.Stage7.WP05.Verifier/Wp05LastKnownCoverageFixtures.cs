using System;
using System.Runtime.CompilerServices;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05LastKnownCoverageFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var t = Wp05FixtureSupport.T;
        var assertion = new FoundationSelfModelAssertion(
            "selfmodel:assertion:wp05:coverage:lastknown", "foundation.health.subject:wp05:coverage",
            FoundationSelfModelArea.RuntimeCondition, FoundationSelfModelAssertionKind.Fact,
            FoundationSelfModelTemporalView.LastKnown, "scope:foundation:wp05:coverage", "runtime:lastknown:usable",
            "source:runtime:coverage", "Foundation Runtime Authority", "evidence:runtime:lastknown:coverage",
            EvidenceQuality.Sufficient, "SUFFICIENT", "NONE", "freshness:wp05", "rule:wp05:lastknown:coverage", "1.0",
            t.AddMinutes(-1), t.AddMinutes(-1), t.AddMinutes(1), null, null);

        var eligible = EvidenceAwarenessRuntime.EvaluateLastKnownReliance(
            "wp05:lastknown:coverage:eligible", assertion, "policy:lastknown:wp05:coverage", t);
        Wp05FixtureSupport.Require(eligible.Eligible, "WP05 coverage: eligible LastKnown rejected.");

        var current = EvidenceAwarenessRuntime.EvaluateLastKnownReliance(
            "wp05:lastknown:coverage:current", assertion with { TemporalView = FoundationSelfModelTemporalView.Current },
            "policy:lastknown:wp05:coverage", t);
        Wp05FixtureSupport.Require(!current.Eligible, "WP05 coverage: Current assertion treated as LastKnown fallback.");

        var noPolicyRejected = false;
        try
        {
            _ = EvidenceAwarenessRuntime.EvaluateLastKnownReliance(
                "wp05:lastknown:coverage:no-policy", assertion, string.Empty, t);
        }
        catch (ArgumentException)
        {
            noPolicyRejected = true;
        }
        Wp05FixtureSupport.Require(noPolicyRejected, "WP05 coverage: missing LastKnown policy did not fail closed.");
    }
}
