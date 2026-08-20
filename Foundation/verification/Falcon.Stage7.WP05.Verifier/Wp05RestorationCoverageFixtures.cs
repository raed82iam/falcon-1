using System.Runtime.CompilerServices;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05RestorationCoverageFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var t = Wp05FixtureSupport.T;
        var rule = Wp05FixtureSupport.Rule();
        var health = Wp05FixtureSupport.Health(rule);

        var missing = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Missing);
        var missingQuality = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:coverage:restoration:missing", rule, health, missing, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);
        var activeLoss = EvidenceAwarenessRuntime.EvaluateRestoration(
            "wp05:coverage:restoration:active-loss", missing, missingQuality, health, null,
            EvidenceQuality.Sufficient, missing.ObservationTime, t);
        Wp05FixtureSupport.Require(activeLoss.State == RestorationGateState.LossActive,
            "WP05 coverage: active loss did not hold restoration closed.");

        var available = Wp05FixtureSupport.Relation(rule, health, HealthEvidenceLossClass.Available);
        var quality = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:coverage:restoration:available", rule, health, available, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);

        var beforeObservation = Wp05FixtureSupport.Challenge(available.Identity) with
        {
            SourceAuthenticity = SourceAuthenticityState.Verified,
            ObservationTime = available.ObservationTime,
            AssessmentTime = t,
            Expiry = t.AddMinutes(1)
        };
        var before = EvidenceAwarenessRuntime.EvaluateRestoration(
            "wp05:coverage:restoration:before", available, quality, health, beforeObservation,
            EvidenceQuality.Sufficient, available.ObservationTime, t);
        Wp05FixtureSupport.Require(before.State == RestorationGateState.SourceReappearedPendingIndependentReassessment,
            "WP05 coverage: non-fresh reassessment completed restoration.");

        var sameOwner = Wp05FixtureSupport.Challenge(available.Identity) with
        {
            SourceAuthenticity = SourceAuthenticityState.Verified,
            ChallengerOwner = "Foundation Runtime Authority",
            ObservationTime = available.ObservationTime.AddMilliseconds(100),
            AssessmentTime = t,
            Expiry = t.AddMinutes(1)
        };
        var sameOwnerResult = EvidenceAwarenessRuntime.EvaluateRestoration(
            "wp05:coverage:restoration:same-owner", available, quality, health, sameOwner,
            EvidenceQuality.Sufficient, available.ObservationTime, t);
        Wp05FixtureSupport.Require(sameOwnerResult.State == RestorationGateState.SourceReappearedPendingIndependentReassessment,
            "WP05 coverage: same-owner reassessment completed restoration.");

        var wrongRequirementRejected = false;
        try
        {
            _ = EvidenceAwarenessRuntime.EvaluateRestoration(
                "wp05:coverage:restoration:wrong-requirement",
                available with { HealthRequirementId = "requirement:wp05:wrong" }, quality, health, null,
                EvidenceQuality.Sufficient, available.ObservationTime, t);
        }
        catch (System.ArgumentException)
        {
            wrongRequirementRejected = true;
        }
        Wp05FixtureSupport.Require(wrongRequirementRejected,
            "WP05 coverage: wrong restoration requirement binding was accepted.");
    }
}
