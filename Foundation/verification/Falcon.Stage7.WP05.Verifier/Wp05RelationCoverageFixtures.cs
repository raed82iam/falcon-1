using System;
using System.Runtime.CompilerServices;
using Foundation.Contracts;
using Foundation.HealthFitness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05RelationCoverageFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var rule = Wp05FixtureSupport.Rule();
        var health = Wp05FixtureSupport.Health(rule);
        var available = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Available);

        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available).Result == ValidationResult.Pass,
            "WP05 coverage: exact valid relation rejected.");

        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available with { HealthRequirementId = "requirement:wp05:missing" }).Result != ValidationResult.Pass,
            "WP05 coverage: missing requirement accepted.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available with { EvidenceRole = HealthEvidenceRole.Supporting }).Result != ValidationResult.Pass,
            "WP05 coverage: wrong role accepted.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available with { SourceId = "source:wrong" }).Result != ValidationResult.Pass,
            "WP05 coverage: wrong source accepted.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available with { SourceOwner = "Foundation Other Authority" }).Result != ValidationResult.Pass,
            "WP05 coverage: wrong source owner accepted.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available with { HealthRuleVersion = "2.0" }).Result != ValidationResult.Pass,
            "WP05 coverage: wrong rule version accepted.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available with { SubjectId = "foundation.health.subject:wrong" }).Result != ValidationResult.Pass,
            "WP05 coverage: wrong subject accepted.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, available with { Capability = "foundation.technical.wrong" }).Result != ValidationResult.Pass,
            "WP05 coverage: wrong capability accepted.");

        var stale = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Stale);
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, stale).Result == ValidationResult.Pass,
            "WP05 coverage: valid stale evidence rejected.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, stale with { SourceExpiry = null }).Result != ValidationResult.Pass,
            "WP05 coverage: stale evidence without expiry accepted.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.ValidateRelation(rule, health, stale with { SourceExpiry = Wp05FixtureSupport.T.AddMinutes(1) }).Result != ValidationResult.Pass,
            "WP05 coverage: unexpired evidence classified stale.");

        var corrupted = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Corrupted);
        var provenance = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.ProvenanceFailure);
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.Evaluate("wp05:coverage:corrupted", rule, health, corrupted, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient).EffectiveQuality == EvidenceQuality.Invalid,
            "WP05 coverage: corrupted required evidence was not invalid.");
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.Evaluate("wp05:coverage:provenance", rule, health, provenance, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient).EffectiveQuality == EvidenceQuality.Invalid,
            "WP05 coverage: provenance failure was not invalid.");

        var partial = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.PartialVisibility);
        Wp05FixtureSupport.Require(
            HealthEvidenceQualityRuntime.Evaluate("wp05:coverage:partial", rule, health, partial, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient).EffectiveQuality == EvidenceQuality.Insufficient,
            "WP05 coverage: partial visibility did not remain insufficient.");
    }
}
