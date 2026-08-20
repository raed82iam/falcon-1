using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05FixtureSupport
{
    internal static readonly DateTimeOffset T = new(2026, 8, 13, 20, 0, 0, TimeSpan.Zero);

    internal static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    internal static HealthRuleDefinition Rule(HealthEvidenceRole role = HealthEvidenceRole.RequiredPrimary)
    {
        return new HealthRuleDefinition(
            "health-rule:stage7:wp05:coverage", "1.0", "foundation.health.subject:wp05:coverage", "foundation.technical.health",
            HealthFreshnessProfile.Fast, null, HealthConsequenceClass.CapabilityBlocking,
            "Falcon Operational Integrity Authority", "SYS-008 v1.1", true, false,
            new[]
            {
                new HealthEvidenceRequirement(
                    "requirement:wp05:coverage", HealthDimension.Availability, role,
                    "source:runtime:coverage", "Foundation Runtime Authority")
            },
            Array.Empty<HealthDependencyRequirement>());
    }

    internal static CanonicalHealthAssessment Health(HealthRuleDefinition rule)
    {
        var observation = new HealthObservation(
            "observation:wp05:coverage", "requirement:wp05:coverage", rule.SubjectId, rule.Capability,
            HealthDimension.Availability, "source:runtime:coverage", "Foundation Runtime Authority", "evidence:runtime:coverage",
            HealthObservationCondition.Satisfied, T.AddSeconds(-1), T.AddMinutes(1), true, true, true, true, true, true);
        return HealthObservationAssessmentRuntime.Evaluate(
            rule, new[] { observation }, Array.Empty<HealthDependencyAssessment>(), T).Assessment;
    }

    internal static HealthEvidenceRelationAssessment Relation(
        HealthRuleDefinition rule,
        CanonicalHealthAssessment health,
        HealthEvidenceLossClass loss,
        HealthEvidenceRole? role = null)
    {
        var acquisition = loss switch
        {
            HealthEvidenceLossClass.Delayed => HealthEvidenceAcquisitionState.Pending,
            HealthEvidenceLossClass.Missing => HealthEvidenceAcquisitionState.Unavailable,
            _ => HealthEvidenceAcquisitionState.Arrived
        };
        var expiry = loss == HealthEvidenceLossClass.Stale ? T.AddMilliseconds(-500) : T.AddMinutes(1);
        var effectiveRole = role ?? rule.EvidenceRequirements.Single().Role;
        return new HealthEvidenceRelationAssessment(
            "wp05:coverage:relation:" + loss.ToString().ToLowerInvariant(), "requirement:wp05:coverage", rule.RuleId, rule.RuleVersion,
            rule.SubjectId, rule.Capability, "scope:foundation:wp05:coverage", effectiveRole,
            "source:runtime:coverage", "Foundation Runtime Authority", "evidence:runtime:coverage", acquisition, loss,
            loss switch
            {
                HealthEvidenceLossClass.Available => EvidenceQuality.Sufficient,
                HealthEvidenceLossClass.Corrupted or HealthEvidenceLossClass.ProvenanceFailure => EvidenceQuality.Invalid,
                _ => effectiveRole is HealthEvidenceRole.RequiredPrimary or HealthEvidenceRole.RequiredIndependent
                    ? EvidenceQuality.Insufficient : EvidenceQuality.Limited
            },
            "coverage:" + loss.ToString().ToLowerInvariant(), T.AddSeconds(-1), T, expiry,
            health.AssessmentId, health.Identity);
    }

    internal static DriftCoverageDeclaration[] Coverage()
    {
        return Enum.GetValues<EvidenceDriftDomain>().Select(domain => new DriftCoverageDeclaration(
            "drift:coverage:extra:" + domain.ToString().ToLowerInvariant(), "drift-rule:wp05:coverage", "1.0",
            "AWR-001 v2.1", "evaluator:wp05:coverage", "foundation.health.subject:wp05:coverage", "foundation.technical.health",
            "scope:foundation:wp05:coverage", domain, DriftApplicability.Applicable,
            "basis:coverage:" + domain.ToString().ToLowerInvariant(), "evidence:drift:coverage:" + domain.ToString().ToLowerInvariant(),
            "governed-drift-basis", T.AddMinutes(-1), T.AddMinutes(1))).ToArray();
    }

    internal static CompetenceDeclaration[] Competence(IEnumerable<DriftCoverageDeclaration> coverage)
    {
        return coverage.Select(x => new CompetenceDeclaration(
            "competence:extra:" + x.Domain.ToString().ToLowerInvariant(), x.EvaluatorId, "Foundation Self Awareness Runtime",
            x.Domain, FoundationSelfModelArea.RuntimeCondition, x.SubjectId, x.Scope,
            "evidence:competence:extra:" + x.Domain.ToString().ToLowerInvariant(), "source:competence:registry",
            "Foundation Governance Authority", x.RuleId, x.RuleVersion, x.GoverningAuthority,
            T.AddMinutes(-1), T.AddMinutes(1))).ToArray();
    }

    internal static IndependentChallengeRecord Challenge(string relationIdentity = "wp05:coverage:relation:available")
    {
        return new IndependentChallengeRecord(
            "challenge:wp05:coverage", relationIdentity, "Foundation Runtime Authority",
            "challenger:wp05:coverage", "Foundation Independent Verification Authority",
            "evidence:challenge:authority:coverage", "evidence:challenge:independent:coverage", SourceAuthenticityState.PendingWp06,
            ChallengeResult.Confirmed, "independent-confirmation", T.AddSeconds(-1), T, T.AddMinutes(1));
    }
}
