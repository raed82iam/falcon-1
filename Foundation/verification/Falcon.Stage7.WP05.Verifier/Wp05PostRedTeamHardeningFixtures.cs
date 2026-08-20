using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Foundation.Contracts;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05PostRedTeamHardeningFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        VerifyExactRestorationRelationBinding();
        VerifyCircularChallengeRejection();
        VerifyBlindSpotValidation();
        VerifyCurrentUnknownWithRequiredLoss();
        VerifyMalformedCompetenceCreatesBlindSpot();
    }

    private static void VerifyExactRestorationRelationBinding()
    {
        var rule = Wp05FixtureSupport.Rule();
        var health = Wp05FixtureSupport.Health(rule);
        var relation = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Available);
        var quality = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:hardening:quality:exact-relation",
            rule,
            health,
            relation,
            EvidenceQuality.Sufficient,
            EvidenceQuality.Sufficient);

        Wp05FixtureSupport.Require(
            string.Equals(quality.RelationIdentity, relation.Identity, StringComparison.Ordinal),
            "WP05 hardening: quality result did not retain exact relation identity.");

        var mutatedRuleRelation = relation with { HealthRuleVersion = "2.0" };
        var ruleMutationRejected = false;
        try
        {
            _ = EvidenceAwarenessRuntime.EvaluateRestoration(
                "wp05:hardening:restoration:wrong-rule",
                mutatedRuleRelation,
                quality,
                health,
                null,
                EvidenceQuality.Sufficient,
                relation.ObservationTime,
                Wp05FixtureSupport.T);
        }
        catch (ArgumentException)
        {
            ruleMutationRejected = true;
        }

        Wp05FixtureSupport.Require(
            ruleMutationRejected,
            "WP05 hardening: restoration accepted quality from a different rule-bound relation.");

        var mutatedSourceRelation = relation with { SourceOwner = "Foundation Other Authority" };
        var sourceMutationRejected = false;
        try
        {
            _ = EvidenceAwarenessRuntime.EvaluateRestoration(
                "wp05:hardening:restoration:wrong-source",
                mutatedSourceRelation,
                quality,
                health,
                null,
                EvidenceQuality.Sufficient,
                relation.ObservationTime,
                Wp05FixtureSupport.T);
        }
        catch (ArgumentException)
        {
            sourceMutationRejected = true;
        }

        Wp05FixtureSupport.Require(
            sourceMutationRejected,
            "WP05 hardening: restoration accepted quality from a different source-bound relation.");
    }

    private static void VerifyCircularChallengeRejection()
    {
        var challenge = Wp05FixtureSupport.Challenge();
        var circular = challenge with
        {
            IndependentEvidenceReference = challenge.ChallengedRelationIdentity
        };

        Wp05FixtureSupport.Require(
            EvidenceAwarenessRuntime.ValidateChallenge(circular, Wp05FixtureSupport.T).Result != ValidationResult.Pass,
            "WP05 hardening: direct circular challenge evidence was accepted.");
    }

    private static void VerifyBlindSpotValidation()
    {
        var t = Wp05FixtureSupport.T;
        var baseline = new KnownBlindSpot(
            "blindspot:hardening:baseline",
            "foundation.health.subject:wp05:coverage",
            "foundation.technical.health",
            "scope:foundation:wp05:coverage",
            EvidenceDriftDomain.Authority,
            "governed-authority-awareness-gap",
            "evidence:blindspot:hardening",
            "authority-context:affected:foundation.technical.health",
            AuthorityImpactClass.NoneDeclared,
            "policy:blindspot:none-declared:hardening",
            t.AddSeconds(-1),
            t,
            t.AddMinutes(1));

        Wp05FixtureSupport.Require(
            EvidenceAwarenessRuntime.ValidateBlindSpot(baseline, t).Result == ValidationResult.Pass,
            "WP05 hardening: governed NONE_DECLARED blind spot was rejected.");

        Wp05FixtureSupport.Require(
            EvidenceAwarenessRuntime.ValidateBlindSpot(baseline with { GoverningBasis = string.Empty }, t).Result != ValidationResult.Pass,
            "WP05 hardening: NONE_DECLARED without governing basis was accepted.");

        Wp05FixtureSupport.Require(
            EvidenceAwarenessRuntime.ValidateBlindSpot(
                baseline with
                {
                    BlindSpotId = "blindspot:hardening:reassessment",
                    AuthorityImpact = AuthorityImpactClass.RequiresGovernedReassessment
                },
                t).Result == ValidationResult.Pass,
            "WP05 hardening: governed reassessment blind spot was rejected.");

        Wp05FixtureSupport.Require(
            EvidenceAwarenessRuntime.ValidateBlindSpot(
                baseline with
                {
                    BlindSpotId = "blindspot:hardening:no-authority-context",
                    AffectedAuthorityContext = string.Empty
                },
                t).Result != ValidationResult.Pass,
            "WP05 hardening: blind spot without affected authority context was accepted.");
    }

    private static void VerifyCurrentUnknownWithRequiredLoss()
    {
        var rule = Wp05FixtureSupport.Rule();
        var unknown = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            Array.Empty<HealthObservation>(),
            Array.Empty<HealthDependencyAssessment>(),
            Wp05FixtureSupport.T).Assessment;

        Wp05FixtureSupport.Require(
            unknown.HealthState == HealthState.Unknown,
            "WP05 hardening: missing required evidence did not produce current UNKNOWN Health.");

        var relation = Wp05FixtureSupport.Relation(
            rule,
            unknown,
            HealthEvidenceLossClass.Missing);
        var quality = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:hardening:current-unknown:loss",
            rule,
            unknown,
            relation,
            EvidenceQuality.Sufficient,
            EvidenceQuality.Sufficient);

        Wp05FixtureSupport.Require(
            quality.EffectiveQuality != EvidenceQuality.Sufficient,
            "WP05 hardening: required current loss became positive evidence despite current UNKNOWN Health.");
    }

    private static void VerifyMalformedCompetenceCreatesBlindSpot()
    {
        var coverage = Wp05FixtureSupport.Coverage();
        var competence = Wp05FixtureSupport.Competence(coverage)
            .Select(value => value.Domain == EvidenceDriftDomain.Data
                ? value with { EvidenceReference = string.Empty }
                : value)
            .ToArray();

        var evaluation = EvidenceAwarenessRuntime.Evaluate(
            "foundation.health.subject:wp05:coverage",
            "foundation.technical.health",
            "scope:foundation:wp05:coverage",
            coverage,
            competence,
            Array.Empty<DriftFinding>(),
            null,
            Wp05FixtureSupport.T);

        Wp05FixtureSupport.Require(
            evaluation.BlindSpots.Any(value => value.Domain == EvidenceDriftDomain.Data),
            "WP05 hardening: malformed competence evidence supported positive competence.");
    }
}
