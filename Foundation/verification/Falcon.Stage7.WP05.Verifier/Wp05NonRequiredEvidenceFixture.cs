using System;
using System.Runtime.CompilerServices;
using Foundation.HealthFitness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05NonRequiredEvidenceFixture
{
    [ModuleInitializer]
    internal static void Run()
    {
        var rule = new HealthRuleDefinition(
            "health-rule:stage7:wp05:coverage",
            "1.0",
            "foundation.health.subject:wp05:coverage",
            "foundation.technical.health",
            HealthFreshnessProfile.Fast,
            null,
            HealthConsequenceClass.CapabilityBlocking,
            "Falcon Operational Integrity Authority",
            "SYS-008 v1.1",
            true,
            false,
            new[]
            {
                new HealthEvidenceRequirement(
                    "requirement:wp05:coverage",
                    HealthDimension.Availability,
                    HealthEvidenceRole.RequiredPrimary,
                    "source:runtime:coverage",
                    "Foundation Runtime Authority"),
                new HealthEvidenceRequirement(
                    "requirement:wp05:supporting",
                    HealthDimension.Availability,
                    HealthEvidenceRole.Supporting,
                    "source:runtime:supporting",
                    "Foundation Runtime Authority")
            },
            Array.Empty<HealthDependencyRequirement>());

        var health = Wp05FixtureSupport.Health(rule);
        var relation = new HealthEvidenceRelationAssessment(
            "wp05:coverage:relation:supporting-missing",
            "requirement:wp05:supporting",
            rule.RuleId,
            rule.RuleVersion,
            rule.SubjectId,
            rule.Capability,
            "scope:foundation:wp05:coverage",
            HealthEvidenceRole.Supporting,
            "source:runtime:supporting",
            "Foundation Runtime Authority",
            "evidence:runtime:supporting",
            HealthEvidenceAcquisitionState.Unavailable,
            HealthEvidenceLossClass.Missing,
            EvidenceQuality.Limited,
            "coverage:supporting-missing",
            Wp05FixtureSupport.T.AddSeconds(-1),
            Wp05FixtureSupport.T,
            Wp05FixtureSupport.T.AddMinutes(1),
            health.AssessmentId,
            health.Identity);

        var result = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:coverage:nonrequired",
            rule,
            health,
            relation,
            EvidenceQuality.Sufficient,
            EvidenceQuality.Sufficient);

        Wp05FixtureSupport.Require(
            result.StatusQuality == EvidenceQuality.Limited,
            "WP05 coverage: non-required evidence quality mismatch.");

        Wp05FixtureSupport.Require(
            result.EffectiveQuality == EvidenceQuality.Limited,
            "WP05 coverage: non-required evidence did not bound effective quality to Limited.");
    }
}
