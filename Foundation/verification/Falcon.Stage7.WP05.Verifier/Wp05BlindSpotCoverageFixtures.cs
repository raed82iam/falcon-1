using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05BlindSpotCoverageFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var coverage = Wp05FixtureSupport.Coverage().Where(x => x.Domain != EvidenceDriftDomain.Authority).ToArray();
        var competence = Wp05FixtureSupport.Competence(coverage);
        var evaluation = EvidenceAwarenessRuntime.Evaluate(
            "foundation.health.subject:wp05:coverage", "foundation.technical.health", "scope:foundation:wp05:coverage",
            coverage, competence, Array.Empty<DriftFinding>(), null, Wp05FixtureSupport.T);

        var blind = evaluation.BlindSpots.Single(x => x.Domain == EvidenceDriftDomain.Authority);
        Wp05FixtureSupport.Require(!string.IsNullOrWhiteSpace(blind.AffectedAuthorityContext),
            "WP05 coverage: blind spot omitted affected authority context.");
        Wp05FixtureSupport.Require(blind.AuthorityImpact == AuthorityImpactClass.PositiveInferenceBlocked,
            "WP05 coverage: generated blind spot did not block positive inference.");
        Wp05FixtureSupport.Require(!string.IsNullOrWhiteSpace(blind.GoverningBasis),
            "WP05 coverage: blind spot omitted governing basis.");
        Wp05FixtureSupport.Require(Enum.IsDefined(AuthorityImpactClass.RequiresGovernedReassessment),
            "WP05 coverage: governed reassessment impact class unavailable.");
    }
}
