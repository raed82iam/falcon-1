using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05CompetenceDomainFixture
{
    [ModuleInitializer]
    internal static void Run()
    {
        var coverage = Wp05FixtureSupport.Coverage();
        var competence = Wp05FixtureSupport.Competence(coverage)
            .Select(x => x.Domain == EvidenceDriftDomain.Data ? x with { Domain = EvidenceDriftDomain.Dependency } : x)
            .ToArray();
        var evaluation = EvidenceAwarenessRuntime.Evaluate(
            "foundation.health.subject:wp05:coverage", "foundation.technical.health", "scope:foundation:wp05:coverage",
            coverage, competence, Array.Empty<DriftFinding>(), null, Wp05FixtureSupport.T);
        Wp05FixtureSupport.Require(evaluation.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.Data),
            "WP05 coverage: wrong competence domain was accepted.");
    }
}
