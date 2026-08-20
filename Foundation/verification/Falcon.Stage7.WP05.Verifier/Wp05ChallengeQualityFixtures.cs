using System;
using System.Runtime.CompilerServices;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05ChallengeQualityFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var t = Wp05FixtureSupport.T;
        var coverage = Wp05FixtureSupport.Coverage();
        var competence = Wp05FixtureSupport.Competence(coverage);
        var contradicted = Wp05FixtureSupport.Challenge() with { Result = ChallengeResult.Contradicted };

        var evaluation = EvidenceAwarenessRuntime.Evaluate(
            "foundation.health.subject:wp05:coverage", "foundation.technical.health", "scope:foundation:wp05:coverage",
            coverage, competence, Array.Empty<DriftFinding>(), contradicted, t);

        Wp05FixtureSupport.Require(
            evaluation.ChallengeQuality == EvidenceQuality.Insufficient,
            "WP05 coverage: contradictory challenge did not reduce challenge quality.");
    }
}
