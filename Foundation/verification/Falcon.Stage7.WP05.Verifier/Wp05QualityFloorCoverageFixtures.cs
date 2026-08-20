using System.Runtime.CompilerServices;
using Foundation.HealthFitness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05QualityFloorCoverageFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var rule = Wp05FixtureSupport.Rule();
        var health = Wp05FixtureSupport.Health(rule);
        var available = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Available);

        var invalidHealth = health with { EvidenceQuality = EvidenceQuality.Invalid, Confidence = "INVALID" };
        var invalidRelation = available with
        {
            CanonicalHealthAssessmentId = invalidHealth.AssessmentId,
            CanonicalHealthAssessmentIdentity = invalidHealth.Identity
        };
        var invalid = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:coverage:quality:invalid", rule, invalidHealth, invalidRelation,
            EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);
        Wp05FixtureSupport.Require(invalid.EffectiveQuality == EvidenceQuality.Invalid,
            "WP05 coverage: Invalid canonical health quality was improved.");

        var insufficientHealth = health with { EvidenceQuality = EvidenceQuality.Insufficient, Confidence = "INSUFFICIENT" };
        var insufficientRelation = available with
        {
            CanonicalHealthAssessmentId = insufficientHealth.AssessmentId,
            CanonicalHealthAssessmentIdentity = insufficientHealth.Identity
        };
        var insufficient = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:coverage:quality:insufficient", rule, insufficientHealth, insufficientRelation,
            EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);
        Wp05FixtureSupport.Require(insufficient.EffectiveQuality == EvidenceQuality.Insufficient,
            "WP05 coverage: Insufficient canonical health quality was improved.");
    }
}
