using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP09.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset T =
        new(2026, 8, 14, 14, 0, 0, TimeSpan.Zero);

    private const string Subject = "foundation:falcon";
    private const string Capability = "foundation:health:vpl005";
    private const string Scope = "scope:foundation:vpl005";

    private static int _passCount;

    private sealed record ScenarioResult(
        HealthEvidenceLossClass LossClass,
        CanonicalHealthAssessment LossHealth,
        HealthEvidenceQualityResult Quality,
        FoundationSelfModelSnapshot Model,
        CanonicalHealthFitnessAssessment Fitness,
        GovernedFitnessConsumptionEvidence Consumption,
        HealthFitnessChangeFact ChangeFact,
        HealthFitnessHistoryRecord History,
        HealthFitnessReconstructionResult Reconstruction);

    private static int Main()
    {
        try
        {
            Run("fresh-valid-baseline", VerifyFreshBaseline);

            foreach (var loss in Enum.GetValues<HealthEvidenceLossClass>()
                         .Where(value => value != HealthEvidenceLossClass.Available))
            {
                var captured = loss;
                Run("vpl005-loss-" + captured.ToString().ToLowerInvariant(),
                    () => VerifyLossScenario(captured));
            }

            Run("all-nine-loss-classes-covered", VerifyNineLossClassesCovered);
            Run("last-known-expiry", VerifyLastKnownExpiry);
            Run("source-reappearance-pending", VerifySourceReappearancePending);
            Run("independent-reassessment-restores-input-not-authority", VerifyIndependentReassessment);
            Run("unaffected-capability-isolation", VerifyUnaffectedCapabilityIsolation);
            Run("zero-application-and-no-business-semantics", VerifyZeroApplicationBoundary);
            Run("deterministic-identical-input", VerifyDeterminism);
            Run("loss-mutation-sensitive", VerifyMutationSensitivity);
            Run("no-future-stage-action-surface", VerifyNoFutureStageActionSurface);

            Console.WriteLine("STAGE7_WP09_VERIFIER = PASS");
            Console.WriteLine("CHECKS = " + _passCount + "/" + _passCount);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE7_WP09_VERIFIER = FAIL");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void Run(string name, Action action)
    {
        action();
        _passCount++;
        Console.WriteLine("PASS | " + name);
    }

    private static void VerifyFreshBaseline()
    {
        var rule = HealthRule();
        var health = HealthyHealth(rule, T, "baseline");
        var model = BuildModel(health, "baseline", T.AddSeconds(2));
        var fitness = TechnicalFitnessEvaluationRuntime.Evaluate(
            "fitness:assessment:wp09:baseline",
            FitnessRule(),
            model,
            T.AddSeconds(3),
            T.AddSeconds(20));

        Require(health.HealthState == HealthState.Healthy,
            "Fresh baseline Health was not HEALTHY.");
        Require(health.EvidenceQuality == EvidenceQuality.Sufficient,
            "Fresh baseline Health evidence was not sufficient.");
        Require(fitness.TechnicalFitnessState == TechnicalFitnessState.Fit,
            "Fresh baseline technical fitness was not FIT.");
        Require(fitness.FitnessResult == FitnessProjectionResult.Fit,
            "Fresh baseline CON-006 projection was not FIT.");

        var consumption = HealthFitnessGovernedConsumptionRuntime.Evaluate(
            fitness,
            new GovernedFitnessConsumptionContext(
                GovernedFitnessConsumerRole.AuthorityEngine,
                T.AddSeconds(4),
                RequiredAwarenessAvailable: true,
                IndependentReassessmentConfirmed: false,
                PriorMaterialAwarenessOrFitnessLoss: false,
                PriorAuthorityRestrictionOrDenial: false));

        Require(consumption.CanSupportPositiveAuthorityCondition,
            "Fresh FIT could not support a positive authority condition input.");
        Require(!consumption.PositiveAuthorityInferenceBlocked,
            "Fresh FIT was unexpectedly blocked.");
    }

    private static void VerifyLossScenario(HealthEvidenceLossClass loss)
    {
        var scenario = BuildLossScenario(loss);

        Require(scenario.Quality.LossClass == loss,
            "VPL-005 loss class identity was not preserved: " + loss);
        Require(scenario.Quality.EffectiveQuality != EvidenceQuality.Sufficient,
            "Required evidence loss remained sufficient: " + loss);
        Require(scenario.LossHealth.HealthState != HealthState.Healthy,
            "Evidence loss remained HEALTHY: " + loss);
        Require(scenario.LossHealth.EvidenceQuality != EvidenceQuality.Sufficient,
            "Loss-aware Health remained sufficient: " + loss);

        var healthAssertion = scenario.Model.Assertions.Single(value =>
            value.Area == FoundationSelfModelArea.HealthCondition &&
            value.TemporalView == FoundationSelfModelTemporalView.Current);

        Require(healthAssertion.AssertionKind == FoundationSelfModelAssertionKind.Unknown,
            "Loss did not become explicit Self Model uncertainty: " + loss);
        Require(healthAssertion.EvidenceQuality != EvidenceQuality.Sufficient,
            "Self Model hid evidence loss: " + loss);
        Require(string.Equals(healthAssertion.SourceAssessmentReference,
                scenario.LossHealth.Identity, StringComparison.Ordinal),
            "Self Model did not bind exact loss-aware Health identity: " + loss);

        Require(scenario.Fitness.TechnicalFitnessState != TechnicalFitnessState.Fit,
            "Loss did not reduce technical fitness: " + loss);
        Require(scenario.Fitness.FitnessResult != FitnessProjectionResult.Fit,
            "Loss left CON-006 FIT: " + loss);
        Require(scenario.Consumption.PositiveAuthorityInferenceBlocked,
            "Loss did not block positive authority inference: " + loss);
        Require(!scenario.Consumption.CanSupportPositiveAuthorityCondition,
            "Loss still supported positive authority condition: " + loss);
        Require(scenario.Consumption.RestrictionInputRequired,
            "Loss did not produce restriction/gating input evidence: " + loss);

        Require(scenario.ChangeFact.FactKind == HealthFitnessFactKind.FitnessChanged,
            "Loss did not produce a material fitness-change fact: " + loss);
        Require(scenario.ChangeFact.Classification ==
                HealthFitnessEventTruthClassification.AuthoritativeOperational,
            "Loss change fact was not operationally classified: " + loss);
        Require(scenario.ChangeFact.Provenance.Contains("vpl005", StringComparison.OrdinalIgnoreCase),
            "Loss trigger provenance did not preserve VPL-005 basis: " + loss);

        Require(scenario.Reconstruction.Trusted,
            "Loss history was not reconstructable: " + loss);
        Require(scenario.Reconstruction.Assessment is not null &&
                string.Equals(scenario.Reconstruction.Assessment.Identity,
                    scenario.Fitness.Identity, StringComparison.Ordinal),
            "Reconstruction lost exact fitness basis: " + loss);
        Require(scenario.Reconstruction.Fact is not null &&
                string.Equals(scenario.Reconstruction.Fact.Identity,
                    scenario.ChangeFact.Identity, StringComparison.Ordinal),
            "Reconstruction lost exact change-fact basis: " + loss);
    }

    private static void VerifyNineLossClassesCovered()
    {
        var losses = Enum.GetValues<HealthEvidenceLossClass>()
            .Where(value => value != HealthEvidenceLossClass.Available)
            .OrderBy(value => (int)value)
            .ToArray();

        Require(losses.Length == 9,
            "VPL-005 must contain exactly nine active loss classes.");

        var expected = new[]
        {
            HealthEvidenceLossClass.Missing,
            HealthEvidenceLossClass.Stale,
            HealthEvidenceLossClass.Delayed,
            HealthEvidenceLossClass.Contradictory,
            HealthEvidenceLossClass.Unverifiable,
            HealthEvidenceLossClass.Inaccessible,
            HealthEvidenceLossClass.Corrupted,
            HealthEvidenceLossClass.ProvenanceFailure,
            HealthEvidenceLossClass.PartialVisibility
        };

        Require(losses.SequenceEqual(expected),
            "VPL-005 loss-class set/order changed.");
    }

    private static void VerifyLastKnownExpiry()
    {
        var rule = HealthRule();
        var health = HealthyHealth(rule, T, "lastknown");

        var assertion = FoundationSelfModelAssertionFactory.FromHealthAssessment(
            "selfmodel:assertion:wp09:lastknown",
            "source:health:wp02",
            "owner:foundation:health",
            Scope,
            "freshness:vpl005:lastknown",
            T.AddSeconds(10),
            health,
            FoundationSelfModelTemporalView.LastKnown);

        var eligible = EvidenceAwarenessRuntime.EvaluateLastKnownReliance(
            "lastknown:assessment:wp09:eligible",
            assertion,
            "policy:vpl005:lastknown",
            T.AddSeconds(5));

        Require(eligible.Eligible,
            "Fresh policy-bound LastKnown evidence was not eligible.");

        var expired = EvidenceAwarenessRuntime.EvaluateLastKnownReliance(
            "lastknown:assessment:wp09:expired",
            assertion with { Expiry = T.AddSeconds(4) },
            "policy:vpl005:lastknown",
            T.AddSeconds(5));

        Require(!expired.Eligible,
            "Expired LastKnown/cached success remained eligible.");
    }

    private static void VerifySourceReappearancePending()
    {
        var restoration = BuildRestoration(challenge: null);
        Require(restoration.State ==
                RestorationGateState.SourceReappearedPendingIndependentReassessment,
            "Source reappearance alone restored the Stage 7 trust gate.");
    }

    private static void VerifyIndependentReassessment()
    {
        var rule = HealthRule();
        var restoredHealth = HealthyHealth(rule, T.AddSeconds(2), "restored");
        var relation = Relation(rule, restoredHealth,
            HealthEvidenceLossClass.Available,
            T.AddSeconds(3));
        var quality = HealthEvidenceQualityRuntime.Evaluate(
            "wp09:quality:restored",
            rule,
            restoredHealth,
            relation,
            EvidenceQuality.Sufficient,
            EvidenceQuality.Sufficient);

        var challenge = new IndependentChallengeRecord(
            "challenge:wp09:restored",
            relation.Identity,
            relation.SourceOwner,
            "challenger:wp09:independent",
            "owner:foundation:independent-verification",
            "evidence:challenge:authority:wp09",
            "evidence:challenge:independent:wp09",
            SourceAuthenticityState.Verified,
            ChallengeResult.Confirmed,
            "fresh-independent-reassessment",
            T.AddSeconds(4),
            T.AddSeconds(5),
            T.AddSeconds(20));

        var restoration = EvidenceAwarenessRuntime.EvaluateRestoration(
            "restoration:assessment:wp09:verified",
            relation,
            quality,
            restoredHealth,
            challenge,
            EvidenceQuality.Sufficient,
            relation.ObservationTime,
            T.AddSeconds(6));

        Require(restoration.State == RestorationGateState.IndependentlyReassessed,
            "Fresh independent reassessment did not satisfy restoration gate.");

        var model = BuildModel(restoredHealth, "restored", T.AddSeconds(7));
        var fitness = TechnicalFitnessEvaluationRuntime.Evaluate(
            "fitness:assessment:wp09:restored",
            FitnessRule(),
            model,
            T.AddSeconds(8),
            T.AddSeconds(20));

        var beforeReassessmentContext = new GovernedFitnessConsumptionContext(
            GovernedFitnessConsumerRole.AuthorityEngine,
            T.AddSeconds(9),
            RequiredAwarenessAvailable: true,
            IndependentReassessmentConfirmed: false,
            PriorMaterialAwarenessOrFitnessLoss: true,
            PriorAuthorityRestrictionOrDenial: true);

        var blocked = HealthFitnessGovernedConsumptionRuntime.Evaluate(
            fitness, beforeReassessmentContext);
        Require(blocked.PositiveAuthorityInferenceBlocked,
            "Fresh source data bypassed independent reassessment requirement.");

        var afterReassessment = HealthFitnessGovernedConsumptionRuntime.Evaluate(
            fitness,
            beforeReassessmentContext with
            {
                IndependentReassessmentConfirmed = true
            });

        Require(afterReassessment.CanSupportPositiveAuthorityCondition,
            "Verified reassessment did not restore fitness-input admissibility.");
        Require(afterReassessment.NewAuthorityDecisionRequired,
            "Prior restriction/denial was forgotten after technical restoration.");
    }

    private static void VerifyUnaffectedCapabilityIsolation()
    {
        _ = BuildLossScenario(HealthEvidenceLossClass.Missing);

        var baseline = BuildBaselineFitness();
        var unaffected = baseline with
        {
            AssessmentId = "fitness:assessment:wp09:unaffected",
            Capability = "foundation:health:unaffected",
            Scope = "scope:foundation:unaffected",
            EvidenceReference = "fitness:evidence:wp09:unaffected",
            SelfModelReference = "selfmodel:wp09:unaffected",
            RuleId = "fitness-rule:stage7:wp09:unaffected"
        };

        var consumption = HealthFitnessGovernedConsumptionRuntime.Evaluate(
            unaffected,
            new GovernedFitnessConsumptionContext(
                GovernedFitnessConsumerRole.AuthorityEngine,
                T.AddSeconds(4),
                RequiredAwarenessAvailable: true,
                IndependentReassessmentConfirmed: false,
                PriorMaterialAwarenessOrFitnessLoss: false,
                PriorAuthorityRestrictionOrDenial: false));

        Require(consumption.CanSupportPositiveAuthorityCondition,
            "Independent unaffected capability inherited another scope's evidence loss.");
        Require(!consumption.PositiveAuthorityInferenceBlocked,
            "Independent unaffected capability was contaminated by affected scope.");
    }

    private static void VerifyZeroApplicationBoundary()
    {
        var scenario = BuildLossScenario(HealthEvidenceLossClass.PartialVisibility);
        var tokens = new[]
        {
            scenario.LossHealth.SubjectId,
            scenario.LossHealth.Capability,
            scenario.Model.FoundationId,
            scenario.Fitness.SubjectId,
            scenario.Fitness.Capability,
            scenario.Fitness.Scope,
            scenario.ChangeFact.Owner,
            scenario.ChangeFact.Provenance
        };

        var forbidden = new[]
        {
            "application", "web", "trading", "trade", "market",
            "broker", "portfolio", "strategy", "msa", "lsa", "csa"
        };

        foreach (var value in tokens)
        foreach (var token in forbidden)
            Require(!value.Contains(token, StringComparison.OrdinalIgnoreCase),
                "WP-09 leaked Application/Web/business semantics: " + value);
    }

    private static void VerifyDeterminism()
    {
        var a = BuildLossScenario(HealthEvidenceLossClass.Missing);
        var b = BuildLossScenario(HealthEvidenceLossClass.Missing);

        Require(string.Equals(a.Quality.Identity, b.Quality.Identity, StringComparison.Ordinal),
            "Identical loss inputs changed quality identity.");
        Require(string.Equals(a.Model.Identity, b.Model.Identity, StringComparison.Ordinal),
            "Identical loss inputs changed Self Model identity.");
        Require(string.Equals(a.Fitness.Identity, b.Fitness.Identity, StringComparison.Ordinal),
            "Identical loss inputs changed fitness identity.");
        Require(string.Equals(a.Consumption.Identity, b.Consumption.Identity, StringComparison.Ordinal),
            "Identical loss inputs changed consumption identity.");
        Require(string.Equals(a.History.RecordDigest, b.History.RecordDigest, StringComparison.Ordinal),
            "Identical loss inputs changed history digest.");
    }

    private static void VerifyMutationSensitivity()
    {
        var missing = BuildLossScenario(HealthEvidenceLossClass.Missing);
        var stale = BuildLossScenario(HealthEvidenceLossClass.Stale);

        Require(!string.Equals(missing.Quality.Identity, stale.Quality.Identity, StringComparison.Ordinal),
            "Loss-class mutation did not change quality identity.");
        Require(!string.Equals(missing.Fitness.Identity, stale.Fitness.Identity, StringComparison.Ordinal),
            "Loss-class mutation did not change end-to-end fitness identity.");
        Require(!string.Equals(missing.ChangeFact.Identity, stale.ChangeFact.Identity, StringComparison.Ordinal),
            "Loss-class mutation did not change trigger/fact identity.");
    }

    private static void VerifyNoFutureStageActionSurface()
    {
        var methodNames = new[]
        {
            typeof(HealthEvidenceQualityRuntime).GetMethods(),
            typeof(HealthFitnessGovernedConsumptionRuntime).GetMethods(),
            typeof(HealthFitnessHistoryRuntime).GetMethods(),
            typeof(EvidenceAwarenessRuntime).GetMethods(),
            typeof(TechnicalFitnessEvaluationRuntime).GetMethods()
        }.SelectMany(value => value)
         .Select(value => value.Name)
         .Distinct(StringComparer.Ordinal)
         .ToArray();

        var forbidden = new[]
        {
            "Grant", "Kill", "SafeState", "Recover", "Release",
            "Revive", "Deploy", "Transition", "GuardianCommand"
        };

        foreach (var name in methodNames)
        foreach (var token in forbidden)
            Require(!name.Contains(token, StringComparison.OrdinalIgnoreCase),
                "Future-stage action surface detected: " + name);
    }

    private static ScenarioResult BuildLossScenario(HealthEvidenceLossClass loss)
    {
        if (loss == HealthEvidenceLossClass.Available)
            throw new ArgumentException("Loss scenario requires an actual VPL-005 loss.", nameof(loss));

        var rule = HealthRule();
        var baselineHealth = HealthyHealth(rule, T, "baseline");
        var baselineFitness = BuildBaselineFitness();
        var relation = Relation(rule, baselineHealth, loss, T.AddSeconds(1));
        var quality = HealthEvidenceQualityRuntime.Evaluate(
            "wp09:quality:" + Slug(loss),
            rule,
            baselineHealth,
            relation,
            EvidenceQuality.Sufficient,
            EvidenceQuality.Sufficient);

        var lossHealth = new CanonicalHealthAssessment(
            "health:assessment:wp09:" + Slug(loss),
            Subject,
            Capability,
            HealthState.Unknown,
            quality.EffectiveQuality,
            "vpl005:evidence:" + Slug(loss),
            quality.EffectiveQuality == EvidenceQuality.Invalid ? "INVALID" : "INSUFFICIENT",
            quality.Contradiction,
            loss == HealthEvidenceLossClass.PartialVisibility
                ? "PARTIAL_VISIBILITY"
                : "EVIDENCE_LOSS_" + loss.ToString().ToUpperInvariant(),
            "VPL005_" + loss.ToString().ToUpperInvariant(),
            "NONE",
            HealthConsequenceClass.CapabilityBlocking,
            rule.RuleId,
            rule.RuleVersion,
            relation.ObservationTime,
            relation.AssessmentTime);

        var model = BuildModel(lossHealth, Slug(loss), T.AddSeconds(2));
        var fitness = TechnicalFitnessEvaluationRuntime.Evaluate(
            "fitness:assessment:wp09:" + Slug(loss),
            FitnessRule(),
            model,
            T.AddSeconds(3),
            T.AddSeconds(20));

        var consumption = HealthFitnessGovernedConsumptionRuntime.Evaluate(
            fitness,
            new GovernedFitnessConsumptionContext(
                GovernedFitnessConsumerRole.AuthorityEngine,
                T.AddSeconds(4),
                RequiredAwarenessAvailable: true,
                IndependentReassessmentConfirmed: false,
                PriorMaterialAwarenessOrFitnessLoss: true,
                PriorAuthorityRestrictionOrDenial: true));

        var fact = HealthFitnessHistoryRuntime.CreateChangeFact(
            "vpl005:event:" + Slug(loss),
            HealthFitnessFactKind.FitnessChanged,
            baselineFitness,
            fitness,
            HealthFitnessEventTruthClassification.AuthoritativeOperational,
            HealthFitnessEventRelationKind.None,
            null,
            "vpl005:" + Slug(loss));

        var history = HealthFitnessHistoryRuntime.CreateHistoryRecord(
            fact,
            fitness,
            1,
            string.Empty);

        var reconstruction = HealthFitnessHistoryRuntime.Reconstruct(
            history,
            loggingEvidenceAvailable: true,
            persistenceEvidenceAvailable: true);

        return new ScenarioResult(
            loss, lossHealth, quality, model, fitness, consumption,
            fact, history, reconstruction);
    }

    private static CanonicalHealthFitnessAssessment BuildBaselineFitness()
    {
        var rule = HealthRule();
        var health = HealthyHealth(rule, T, "baseline");
        var model = BuildModel(health, "baseline", T.AddSeconds(2));
        return TechnicalFitnessEvaluationRuntime.Evaluate(
            "fitness:assessment:wp09:baseline",
            FitnessRule(),
            model,
            T.AddSeconds(3),
            T.AddSeconds(20));
    }

    private static RestorationAssessment BuildRestoration(
        IndependentChallengeRecord? challenge)
    {
        var rule = HealthRule();
        var health = HealthyHealth(rule, T.AddSeconds(2), "restoration");
        var relation = Relation(rule, health,
            HealthEvidenceLossClass.Available,
            T.AddSeconds(3));
        var quality = HealthEvidenceQualityRuntime.Evaluate(
            "wp09:quality:restoration",
            rule,
            health,
            relation,
            EvidenceQuality.Sufficient,
            EvidenceQuality.Sufficient);

        return EvidenceAwarenessRuntime.EvaluateRestoration(
            "restoration:assessment:wp09",
            relation,
            quality,
            health,
            challenge,
            EvidenceQuality.Sufficient,
            relation.ObservationTime,
            T.AddSeconds(6));
    }

    private static HealthRuleDefinition HealthRule()
        => new(
            "health-rule:stage7:wp09",
            "1.0",
            Subject,
            Capability,
            HealthFreshnessProfile.Fast,
            null,
            HealthConsequenceClass.CapabilityBlocking,
            "owner:foundation:health",
            "SYS-008-v1.1",
            true,
            false,
            new[]
            {
                new HealthEvidenceRequirement(
                    "requirement:wp09:runtime",
                    HealthDimension.Availability,
                    HealthEvidenceRole.RequiredPrimary,
                    "source:runtime:wp09",
                    "owner:foundation:runtime")
            },
            Array.Empty<HealthDependencyRequirement>());

    private static CanonicalHealthAssessment HealthyHealth(
        HealthRuleDefinition rule,
        DateTimeOffset assessmentTime,
        string suffix)
    {
        var observation = new HealthObservation(
            "observation:wp09:" + suffix,
            "requirement:wp09:runtime",
            rule.SubjectId,
            rule.Capability,
            HealthDimension.Availability,
            "source:runtime:wp09",
            "owner:foundation:runtime",
            "evidence:runtime:wp09:" + suffix,
            HealthObservationCondition.Satisfied,
            assessmentTime.AddSeconds(-1),
            assessmentTime.AddSeconds(30),
            true,
            true,
            true,
            true,
            true,
            true);

        return HealthObservationAssessmentRuntime.Evaluate(
            rule,
            new[] { observation },
            Array.Empty<HealthDependencyAssessment>(),
            assessmentTime).Assessment;
    }

    private static HealthEvidenceRelationAssessment Relation(
        HealthRuleDefinition rule,
        CanonicalHealthAssessment health,
        HealthEvidenceLossClass loss,
        DateTimeOffset relationAssessmentTime)
    {
        var acquisition = loss switch
        {
            HealthEvidenceLossClass.Delayed => HealthEvidenceAcquisitionState.Pending,
            HealthEvidenceLossClass.Missing or HealthEvidenceLossClass.Inaccessible
                => HealthEvidenceAcquisitionState.Unavailable,
            _ => HealthEvidenceAcquisitionState.Arrived
        };

        var observationTime = health.AssessmentTime.AddSeconds(-1);
        var expiry = loss == HealthEvidenceLossClass.Stale
            ? relationAssessmentTime.AddMilliseconds(-100)
            : relationAssessmentTime.AddSeconds(20);

        var quality = loss switch
        {
            HealthEvidenceLossClass.Available => EvidenceQuality.Sufficient,
            HealthEvidenceLossClass.Corrupted or HealthEvidenceLossClass.ProvenanceFailure
                => EvidenceQuality.Invalid,
            _ => EvidenceQuality.Insufficient
        };

        return new HealthEvidenceRelationAssessment(
            "wp09:relation:" + Slug(loss),
            "requirement:wp09:runtime",
            rule.RuleId,
            rule.RuleVersion,
            rule.SubjectId,
            rule.Capability,
            Scope,
            HealthEvidenceRole.RequiredPrimary,
            "source:runtime:wp09",
            "owner:foundation:runtime",
            "evidence:runtime:wp09:relation:" + Slug(loss),
            acquisition,
            loss,
            quality,
            "vpl005-loss:" + Slug(loss),
            observationTime,
            relationAssessmentTime,
            expiry,
            health.AssessmentId,
            health.Identity);
    }

    private static TechnicalFitnessRuleDefinition FitnessRule()
        => new(
            "fitness-rule:stage7:wp09",
            "1.0",
            Subject,
            Capability,
            "authority-level:operational",
            Scope,
            new[]
            {
                new TechnicalFitnessRequirement(
                    "fitness-requirement:wp09:health",
                    FoundationSelfModelArea.HealthCondition,
                    Subject,
                    Scope,
                    new[] { "health-state:healthy" },
                    TechnicalFitnessState.Unknown,
                    TechnicalFitnessState.Unknown,
                    1,
                    "NONE",
                    "owner:foundation:health")
            },
            null);

    private static FoundationSelfModelSnapshot BuildModel(
        CanonicalHealthAssessment health,
        string suffix,
        DateTimeOffset modelTime)
    {
        var assertions = new List<FoundationSelfModelAssertion>();

        foreach (var area in Enum.GetValues<FoundationSelfModelArea>())
        {
            if (area == FoundationSelfModelArea.HealthCondition)
            {
                assertions.Add(
                    FoundationSelfModelAssertionFactory.FromHealthAssessment(
                        "selfmodel:assertion:wp09:health:" + suffix,
                        "source:health:wp02",
                        "owner:foundation:health",
                        Scope,
                        "freshness:vpl005:health",
                        modelTime.AddSeconds(30),
                        health));
                continue;
            }

            var slug = area.ToString().ToLowerInvariant();
            var explicitUnknown = area is FoundationSelfModelArea.TechnicalFitness
                or FoundationSelfModelArea.PendingConformance;

            assertions.Add(new FoundationSelfModelAssertion(
                "selfmodel:assertion:wp09:" + suffix + ":" + slug,
                Subject,
                area,
                explicitUnknown
                    ? FoundationSelfModelAssertionKind.Unknown
                    : FoundationSelfModelAssertionKind.Fact,
                FoundationSelfModelTemporalView.Current,
                Scope,
                explicitUnknown
                    ? "technical:value:unknown:" + slug
                    : "technical:value:wp09:" + slug,
                "source:foundation:wp09:" + slug,
                "owner:foundation:technical",
                "evidence:foundation:wp09:" + slug,
                explicitUnknown
                    ? EvidenceQuality.Insufficient
                    : EvidenceQuality.Sufficient,
                explicitUnknown ? "INSUFFICIENT" : "SUFFICIENT",
                explicitUnknown ? "not-yet-produced-in-projection" : "NONE",
                "freshness:source-bound",
                "awr-001:wp09-projection",
                "2.1",
                modelTime.AddSeconds(-1),
                modelTime.AddSeconds(-1),
                modelTime.AddSeconds(30),
                null,
                null));
        }

        return FoundationSelfModelProjector.Build(
            "selfmodel:model:wp09:" + suffix,
            Subject,
            "baseline:stage7:wp09",
            modelTime,
            assertions,
            null);
    }

    private static string Slug(HealthEvidenceLossClass value)
        => value.ToString().ToLowerInvariant();

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
