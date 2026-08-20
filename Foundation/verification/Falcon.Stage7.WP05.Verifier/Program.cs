using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Contracts;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset T = new(2026, 8, 13, 20, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            VerifyNineLossClasses();
            VerifyWp02BindingAndQualityFloor();
            VerifyDelayedAndFutureSemantics();
            VerifyDriftCoverageAndCompetence();
            VerifyChallengeIndependence();
            VerifyLastKnownAndRestoration();
            VerifyDeterminismAndBoundaries();
            Console.WriteLine("STAGE7_WP05_VERIFIER=PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE7_WP05_VERIFIER=FAIL");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void VerifyNineLossClasses()
    {
        var classes = Enum.GetValues<HealthEvidenceLossClass>();
        Require(classes.Length == 10, "WP05 loss enum must contain AVAILABLE plus exactly nine loss classes.");
        Require(classes.Count(x => x != HealthEvidenceLossClass.Available) == 9, "WP05 VPL-005 loss class count mismatch.");

        var rule = Rule();
        var health = Health(rule);
        foreach (var loss in classes)
        {
            var relation = Relation(rule, health, loss);
            var result = HealthEvidenceQualityRuntime.Evaluate(
                "wp05:result:" + loss.ToString().ToLowerInvariant(), rule, health, relation,
                EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);

            if (loss == HealthEvidenceLossClass.Available)
                Require(result.EffectiveQuality == EvidenceQuality.Sufficient, "AVAILABLE did not preserve sufficient quality.");
            else
                Require(result.EffectiveQuality != EvidenceQuality.Sufficient, "Active required loss remained sufficient: " + loss);
        }
    }

    private static void VerifyWp02BindingAndQualityFloor()
    {
        var rule = Rule();
        var health = Health(rule);
        var missing = Relation(rule, health, HealthEvidenceLossClass.Missing);
        var result = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:result:missing", rule, health, missing, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);
        Require(result.EffectiveQuality == EvidenceQuality.Insufficient, "Required missing evidence did not reduce quality.");
        Require(result.Contradiction.Contains("WP05_REQUIRED_LOSS_VS_HEALTHY", StringComparison.Ordinal),
            "Healthy canonical result versus required loss contradiction was hidden.");

        var badSource = missing with { SourceOwner = "Foundation Evidence Authority" };
        Require(HealthEvidenceQualityRuntime.ValidateRelation(rule, health, badSource).Result != ValidationResult.Pass,
            "Fabricated WP02 source-owner binding was accepted.");

        var threw = false;
        try
        {
            _ = HealthEvidenceQualityRuntime.Evaluate(
                "wp05:result:bad-source", rule, health, badSource, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);
        }
        catch (ArgumentException)
        {
            threw = true;
        }
        Require(threw, "Evaluate ignored failed relation validation.");

        var limitedHealth = health with { EvidenceQuality = EvidenceQuality.Limited, Confidence = "LIMITED" };
        var available = Relation(rule, limitedHealth, HealthEvidenceLossClass.Available);
        var limited = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:result:limited", rule, limitedHealth, available, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);
        Require(limited.EffectiveQuality == EvidenceQuality.Limited, "WP05 improved canonical WP02 quality.");
    }

    private static void VerifyDelayedAndFutureSemantics()
    {
        var rule = Rule();
        var health = Health(rule);
        var delayed = Relation(rule, health, HealthEvidenceLossClass.Delayed);
        Require(HealthEvidenceQualityRuntime.ValidateRelation(rule, health, delayed).Result == ValidationResult.Pass,
            "Explicit pending delayed evidence was rejected.");

        var notPending = delayed with { AcquisitionState = HealthEvidenceAcquisitionState.Arrived };
        Require(HealthEvidenceQualityRuntime.ValidateRelation(rule, health, notPending).Result != ValidationResult.Pass,
            "DELAYED without pending acquisition was accepted.");

        var future = delayed with { ObservationTime = health.AssessmentTime.AddSeconds(1) };
        Require(HealthEvidenceQualityRuntime.ValidateRelation(rule, health, future).Result != ValidationResult.Pass,
            "Future-dated evidence was accepted as delayed/current.");
    }

    private static void VerifyDriftCoverageAndCompetence()
    {
        var coverage = Coverage();
        var competence = Competence(coverage);
        var evaluation = EvidenceAwarenessRuntime.Evaluate(
            "foundation.health.subject:wp05", "foundation.technical.health", "scope:foundation:wp05",
            coverage, competence, Array.Empty<DriftFinding>(), null, T);
        Require(evaluation.BlindSpots.Count == 0, "Complete drift coverage with competence produced blind spots.");
        Require(evaluation.CompetenceQuality == EvidenceQuality.Sufficient, "Complete competence was not sufficient.");

        var missingDomain = coverage.Where(x => x.Domain != EvidenceDriftDomain.OwnAssessment).ToArray();
        var missing = EvidenceAwarenessRuntime.Evaluate(
            "foundation.health.subject:wp05", "foundation.technical.health", "scope:foundation:wp05",
            missingDomain, competence, Array.Empty<DriftFinding>(), null, T);
        Require(missing.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.OwnAssessment),
            "Missing drift domain did not become a known blind spot.");
        Require(missing.CompetenceQuality == EvidenceQuality.Insufficient,
            "Missing drift domain did not reduce competence/awareness quality.");

        var badCompetence = competence.Select(x => x.Domain == EvidenceDriftDomain.Data
            ? x with { EvidenceOwner = x.EvaluatorOwner }
            : x).ToArray();
        var selfCertified = EvidenceAwarenessRuntime.Evaluate(
            "foundation.health.subject:wp05", "foundation.technical.health", "scope:foundation:wp05",
            coverage, badCompetence, Array.Empty<DriftFinding>(), null, T);
        Require(selfCertified.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.Data),
            "Self-issued competence evidence was treated as independent competence.");
    }

    private static void VerifyChallengeIndependence()
    {
        var challenge = Challenge();
        Require(EvidenceAwarenessRuntime.ValidateChallenge(challenge, T).Result == ValidationResult.Pass,
            "Valid independent challenge rejected.");

        var sameOwner = challenge with { ChallengerOwner = challenge.ChallengedSourceOwner };
        Require(EvidenceAwarenessRuntime.ValidateChallenge(sameOwner, T).Result != ValidationResult.Pass,
            "Same-owner independent challenge accepted.");

        var expired = challenge with { Expiry = T };
        Require(EvidenceAwarenessRuntime.ValidateChallenge(expired, T).Result != ValidationResult.Pass,
            "Expired challenge accepted.");
    }

    private static void VerifyLastKnownAndRestoration()
    {
        var assertion = new FoundationSelfModelAssertion(
            "selfmodel:assertion:wp05:lastknown", "foundation.health.subject:wp05",
            FoundationSelfModelArea.RuntimeCondition, FoundationSelfModelAssertionKind.Fact,
            FoundationSelfModelTemporalView.LastKnown, "scope:foundation:wp05", "runtime:lastknown:usable",
            "source:runtime", "Foundation Runtime Authority", "evidence:runtime:lastknown",
            EvidenceQuality.Sufficient, "SUFFICIENT", "NONE", "freshness:wp05", "rule:wp05:lastknown", "1.0",
            T.AddMinutes(-1), T.AddMinutes(-1), T.AddMinutes(1), null, null);

        var lastKnown = EvidenceAwarenessRuntime.EvaluateLastKnownReliance(
            "wp05:lastknown:assessment", assertion, "policy:lastknown:wp05", T);
        Require(lastKnown.Eligible, "Valid LastKnown fallback relation was rejected.");

        var expired = EvidenceAwarenessRuntime.EvaluateLastKnownReliance(
            "wp05:lastknown:expired", assertion with { Expiry = T }, "policy:lastknown:wp05", T);
        Require(!expired.Eligible, "Expired LastKnown remained eligible.");

        var rule = Rule();
        var health = Health(rule);
        var relation = Relation(rule, health, HealthEvidenceLossClass.Available);
        var quality = HealthEvidenceQualityRuntime.Evaluate(
            "wp05:result:restoration", rule, health, relation, EvidenceQuality.Sufficient, EvidenceQuality.Sufficient);

        var pending = EvidenceAwarenessRuntime.EvaluateRestoration(
            "wp05:restoration:pending", relation, quality, health, null, EvidenceQuality.Sufficient,
            relation.ObservationTime, T);
        Require(pending.State == RestorationGateState.SourceReappearedPendingIndependentReassessment,
            "Source reappearance alone restored trust.");

        var pendingAuthenticityChallenge = Challenge() with
        {
            ChallengedRelationIdentity = relation.Identity,
            ObservationTime = relation.ObservationTime.AddMilliseconds(100),
            AssessmentTime = relation.ObservationTime.AddMilliseconds(200),
            Expiry = T.AddMinutes(1)
        };
        var stillPending = EvidenceAwarenessRuntime.EvaluateRestoration(
            "wp05:restoration:auth-pending", relation, quality, health, pendingAuthenticityChallenge,
            EvidenceQuality.Sufficient, relation.ObservationTime, T);
        Require(stillPending.State == RestorationGateState.SourceReappearedPendingIndependentReassessment,
            "PENDING_WP06 source authenticity incorrectly satisfied restoration.");

        var verifiedChallenge = pendingAuthenticityChallenge with { SourceAuthenticity = SourceAuthenticityState.Verified };
        var restored = EvidenceAwarenessRuntime.EvaluateRestoration(
            "wp05:restoration:reassessed", relation, quality, health, verifiedChallenge, EvidenceQuality.Sufficient,
            relation.ObservationTime, T);
        Require(restored.State == RestorationGateState.IndependentlyReassessed,
            "Verified fresh independent reassessment did not satisfy WP05 restoration gate.");
    }

    private static void VerifyDeterminismAndBoundaries()
    {
        var challenge = Challenge();
        Require(challenge.Identity == (challenge with { }).Identity, "Equivalent challenge changed deterministic identity.");
        var blind = new KnownBlindSpot(
            "blindspot:deterministic", "foundation.health.subject:wp05", "foundation.technical.health",
            "scope:foundation:wp05", EvidenceDriftDomain.Data, "reason:blind", "evidence:blind",
            "authority-context:affected:foundation.technical.health", AuthorityImpactClass.PositiveInferenceBlocked,
            "awr-001:req-005", T, T, T.AddMinutes(1));
        Require(blind.Identity == (blind with { }).Identity, "Equivalent blind spot changed deterministic identity.");

        var forbiddenNames = typeof(EvidenceAwarenessRuntime).GetMethods()
            .Select(x => x.Name)
            .Where(x => x.Contains("Grant", StringComparison.OrdinalIgnoreCase) ||
                        x.Contains("Command", StringComparison.OrdinalIgnoreCase) ||
                        x.Contains("Transition", StringComparison.OrdinalIgnoreCase) ||
                        x.Contains("Release", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        Require(forbiddenNames.Length == 0, "WP05 runtime exposed authority/lifecycle/recovery action surface.");
    }

    private static HealthRuleDefinition Rule()
    {
        return new HealthRuleDefinition(
            "health-rule:stage7:wp05", "1.0", "foundation.health.subject:wp05", "foundation.technical.health",
            HealthFreshnessProfile.Fast, null, HealthConsequenceClass.CapabilityBlocking,
            "Falcon Operational Integrity Authority", "SYS-008 v1.1", true, false,
            new[]
            {
                new HealthEvidenceRequirement(
                    "requirement:wp05:runtime", HealthDimension.Availability, HealthEvidenceRole.RequiredPrimary,
                    "source:runtime", "Foundation Runtime Authority")
            },
            Array.Empty<HealthDependencyRequirement>());
    }

    private static CanonicalHealthAssessment Health(HealthRuleDefinition rule)
    {
        var observation = new HealthObservation(
            "observation:wp05:runtime", "requirement:wp05:runtime", rule.SubjectId, rule.Capability,
            HealthDimension.Availability, "source:runtime", "Foundation Runtime Authority", "evidence:runtime:wp05",
            HealthObservationCondition.Satisfied, T.AddSeconds(-1), T.AddMinutes(1), true, true, true, true, true, true);
        return HealthObservationAssessmentRuntime.Evaluate(
            rule, new[] { observation }, Array.Empty<HealthDependencyAssessment>(), T).Assessment;
    }

    private static HealthEvidenceRelationAssessment Relation(
        HealthRuleDefinition rule,
        CanonicalHealthAssessment health,
        HealthEvidenceLossClass loss)
    {
        var acquisition = loss switch
        {
            HealthEvidenceLossClass.Delayed => HealthEvidenceAcquisitionState.Pending,
            HealthEvidenceLossClass.Missing => HealthEvidenceAcquisitionState.Unavailable,
            _ => HealthEvidenceAcquisitionState.Arrived
        };
        var expiry = loss == HealthEvidenceLossClass.Stale ? T.AddMilliseconds(-500) : T.AddMinutes(1);
        return new HealthEvidenceRelationAssessment(
            "wp05:relation:" + loss.ToString().ToLowerInvariant(), "requirement:wp05:runtime", rule.RuleId, rule.RuleVersion,
            rule.SubjectId, rule.Capability, "scope:foundation:wp05", HealthEvidenceRole.RequiredPrimary,
            "source:runtime", "Foundation Runtime Authority", "evidence:runtime:wp05", acquisition, loss,
            loss switch
            {
                HealthEvidenceLossClass.Available => EvidenceQuality.Sufficient,
                HealthEvidenceLossClass.Corrupted or HealthEvidenceLossClass.ProvenanceFailure => EvidenceQuality.Invalid,
                _ => EvidenceQuality.Insufficient
            },
            "relation:" + loss.ToString().ToLowerInvariant(), T.AddSeconds(-1), T, expiry,
            health.AssessmentId, health.Identity);
    }

    private static DriftCoverageDeclaration[] Coverage()
    {
        return Enum.GetValues<EvidenceDriftDomain>().Select(domain => new DriftCoverageDeclaration(
            "drift:coverage:" + domain.ToString().ToLowerInvariant(), "drift-rule:wp05", "1.0",
            "AWR-001 v2.1", "evaluator:wp05", "foundation.health.subject:wp05", "foundation.technical.health",
            "scope:foundation:wp05", domain, DriftApplicability.Applicable,
            "basis:" + domain.ToString().ToLowerInvariant(), "evidence:drift:" + domain.ToString().ToLowerInvariant(),
            "governed-drift-basis", T.AddMinutes(-1), T.AddMinutes(1))).ToArray();
    }

    private static CompetenceDeclaration[] Competence(IEnumerable<DriftCoverageDeclaration> coverage)
    {
        return coverage.Select(x => new CompetenceDeclaration(
            "competence:" + x.Domain.ToString().ToLowerInvariant(), x.EvaluatorId, "Foundation Self Awareness Runtime",
            x.Domain, FoundationSelfModelArea.RuntimeCondition, x.SubjectId, x.Scope,
            "evidence:competence:" + x.Domain.ToString().ToLowerInvariant(), "source:competence:registry",
            "Foundation Governance Authority", x.RuleId, x.RuleVersion, x.GoverningAuthority,
            T.AddMinutes(-1), T.AddMinutes(1))).ToArray();
    }

    private static IndependentChallengeRecord Challenge()
    {
        return new IndependentChallengeRecord(
            "challenge:wp05", "wp05:relation:available", "Foundation Runtime Authority",
            "challenger:wp05", "Foundation Independent Verification Authority",
            "evidence:challenge:authority", "evidence:challenge:independent", SourceAuthenticityState.PendingWp06,
            ChallengeResult.Confirmed, "independent-confirmation", T.AddSeconds(-1), T, T.AddMinutes(1));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
