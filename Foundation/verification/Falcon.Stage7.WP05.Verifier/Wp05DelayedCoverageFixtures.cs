using System.Runtime.CompilerServices;
using Foundation.Contracts;
using Foundation.HealthFitness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05DelayedCoverageFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var rule = Wp05FixtureSupport.Rule();
        var health = Wp05FixtureSupport.Health(rule);
        var delayed = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Delayed);

        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, delayed with { AcquisitionState = HealthEvidenceAcquisitionState.Arrived }).Result != ValidationResult.Pass,
            "WP05 coverage: arrived relation remained classified as delayed.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, delayed with { AcquisitionState = HealthEvidenceAcquisitionState.Expired }).Result != ValidationResult.Pass,
            "WP05 coverage: expired acquisition remained classified as pending delayed evidence.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, null).Result != ValidationResult.Pass,
            "WP05 coverage: omitted evidence relation did not fail closed.");
    }
}
