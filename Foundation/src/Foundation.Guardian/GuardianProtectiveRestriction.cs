using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Guardian;

public enum GuardianRestrictionSeverity
{
    Moderate = 1,
    High = 2,
    Critical = 3
}

public enum GuardianRestrictionStatus
{
    Active = 1,
    ReviewRequired = 2
}

public sealed record GuardianProtectiveRestriction(
    string RestrictionId,
    string SourceDecisionId,
    string SourceDecisionIdentity,
    string TargetId,
    GuardianScopeKind ScopeKind,
    string ScopeId,
    GuardianRestrictionSeverity Severity,
    GuardianProtectiveAction EnforcementAction,
    string TriggerCode,
    string EvidenceReference,
    string AuthorityReference,
    string PolicyReference,
    DateTimeOffset EffectiveAt,
    DateTimeOffset? ReviewDeadline,
    bool PersistAcrossRestart,
    bool SubjectSelfReleaseForbidden)
{
    public string Identity => GuardianRestrictionIdentity.Compute(this);
}

public sealed record GuardianRestrictionEvaluation(
    bool Success,
    string Reason,
    GuardianRestrictionStatus Status,
    bool RemainsEnforced)
{
    public static GuardianRestrictionEvaluation Fail(string reason)
        => new(false, reason, GuardianRestrictionStatus.Active, false);
}

public static class GuardianProtectiveRestrictionRuntime
{
    public static GuardianValidationOutcome Validate(
        GuardianProtectiveRestriction? restriction,
        GuardianProtectiveDecision? sourceDecision)
    {
        if (restriction is null)
            return GuardianValidationOutcome.Fail("MISSING_RESTRICTION");
        if (sourceDecision is null)
            return GuardianValidationOutcome.Fail("MISSING_SOURCE_DECISION");

        var decisionValidation = GuardianProtectiveDecisionValidator.Validate(sourceDecision);
        if (!decisionValidation.Success)
            return GuardianValidationOutcome.Fail("INVALID_SOURCE_DECISION:" + decisionValidation.Reason);

        if (!CanonicalToken(restriction.RestrictionId))
            return GuardianValidationOutcome.Fail("INVALID_RESTRICTION_ID");
        if (!CanonicalToken(restriction.SourceDecisionId))
            return GuardianValidationOutcome.Fail("INVALID_SOURCE_DECISION_ID");
        if (!CanonicalToken(restriction.SourceDecisionIdentity))
            return GuardianValidationOutcome.Fail("INVALID_SOURCE_DECISION_IDENTITY");
        if (!CanonicalToken(restriction.TargetId))
            return GuardianValidationOutcome.Fail("INVALID_TARGET_ID");
        if (!CanonicalToken(restriction.ScopeId))
            return GuardianValidationOutcome.Fail("INVALID_SCOPE_ID");
        if (!CanonicalToken(restriction.TriggerCode))
            return GuardianValidationOutcome.Fail("INVALID_TRIGGER_CODE");
        if (!CanonicalToken(restriction.EvidenceReference))
            return GuardianValidationOutcome.Fail("INVALID_EVIDENCE_REFERENCE");
        if (!CanonicalToken(restriction.AuthorityReference))
            return GuardianValidationOutcome.Fail("INVALID_AUTHORITY_REFERENCE");
        if (!CanonicalToken(restriction.PolicyReference))
            return GuardianValidationOutcome.Fail("INVALID_POLICY_REFERENCE");

        if (!Enum.IsDefined(restriction.ScopeKind))
            return GuardianValidationOutcome.Fail("INVALID_SCOPE_KIND");
        if (!Enum.IsDefined(restriction.Severity))
            return GuardianValidationOutcome.Fail("INVALID_RESTRICTION_SEVERITY");
        if (!Enum.IsDefined(restriction.EnforcementAction))
            return GuardianValidationOutcome.Fail("INVALID_ENFORCEMENT_ACTION");

        if (restriction.EnforcementAction is GuardianProtectiveAction.Observe or GuardianProtectiveAction.Warn)
            return GuardianValidationOutcome.Fail("RESTRICTION_REQUIRES_RESTRICTIVE_ACTION");

        if (restriction.EffectiveAt == default)
            return GuardianValidationOutcome.Fail("INVALID_EFFECTIVE_TIME");
        if (restriction.ReviewDeadline is not null && restriction.ReviewDeadline <= restriction.EffectiveAt)
            return GuardianValidationOutcome.Fail("INVALID_REVIEW_DEADLINE");
        if (!restriction.PersistAcrossRestart)
            return GuardianValidationOutcome.Fail("RESTART_PERSISTENCE_REQUIRED");
        if (!restriction.SubjectSelfReleaseForbidden)
            return GuardianValidationOutcome.Fail("SELF_RELEASE_MUST_BE_FORBIDDEN");

        if (!string.Equals(restriction.SourceDecisionId, sourceDecision.DecisionId, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("SOURCE_DECISION_ID_MISMATCH");
        if (!string.Equals(restriction.SourceDecisionIdentity, sourceDecision.Identity, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("SOURCE_DECISION_IDENTITY_MISMATCH");
        if (!string.Equals(restriction.TargetId, sourceDecision.TargetId, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("TARGET_MISMATCH");
        if (restriction.ScopeKind != sourceDecision.ScopeKind)
            return GuardianValidationOutcome.Fail("SCOPE_KIND_MISMATCH");
        if (!string.Equals(restriction.ScopeId, sourceDecision.ScopeId, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("SCOPE_ID_MISMATCH");
        if (restriction.EnforcementAction != sourceDecision.ProtectiveAction)
            return GuardianValidationOutcome.Fail("ENFORCEMENT_ACTION_MISMATCH");
        if (!string.Equals(restriction.TriggerCode, sourceDecision.TriggerCode, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("TRIGGER_MISMATCH");
        if (!string.Equals(restriction.EvidenceReference, sourceDecision.EvidenceReference, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("EVIDENCE_MISMATCH");
        if (!string.Equals(restriction.AuthorityReference, sourceDecision.AuthorityReference, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("AUTHORITY_MISMATCH");
        if (!string.Equals(restriction.PolicyReference, sourceDecision.PolicyReference, StringComparison.Ordinal))
            return GuardianValidationOutcome.Fail("POLICY_MISMATCH");

        var expectedSeverity = MapSeverity(sourceDecision.ConsequenceClass);
        if (restriction.Severity != expectedSeverity)
            return GuardianValidationOutcome.Fail("SEVERITY_MISMATCH");

        return GuardianValidationOutcome.Pass();
    }

    public static GuardianRestrictionEvaluation EvaluateAt(
        GuardianProtectiveRestriction? restriction,
        GuardianProtectiveDecision? sourceDecision,
        DateTimeOffset evaluationTime)
    {
        var validation = Validate(restriction, sourceDecision);
        if (!validation.Success)
            return GuardianRestrictionEvaluation.Fail(validation.Reason);
        if (evaluationTime == default)
            return GuardianRestrictionEvaluation.Fail("INVALID_EVALUATION_TIME");
        if (evaluationTime < restriction!.EffectiveAt)
            return GuardianRestrictionEvaluation.Fail("RESTRICTION_NOT_YET_EFFECTIVE");

        if (restriction.ReviewDeadline is not null && evaluationTime >= restriction.ReviewDeadline.Value)
        {
            return new GuardianRestrictionEvaluation(
                true,
                "REVIEW_REQUIRED_RESTRICTION_REMAINS_ENFORCED",
                GuardianRestrictionStatus.ReviewRequired,
                true);
        }

        return new GuardianRestrictionEvaluation(
            true,
            "PASS",
            GuardianRestrictionStatus.Active,
            true);
    }

    public static GuardianProtectiveRestriction CreateFromDecision(
        GuardianProtectiveDecision decision,
        string restrictionId,
        DateTimeOffset effectiveAt,
        DateTimeOffset? reviewDeadline)
    {
        var decisionValidation = GuardianProtectiveDecisionValidator.Validate(decision);
        if (!decisionValidation.Success)
            throw new ArgumentException("Invalid source decision: " + decisionValidation.Reason, nameof(decision));
        if (decision.ProtectiveAction is GuardianProtectiveAction.Observe or GuardianProtectiveAction.Warn)
            throw new ArgumentException("Protective decision does not require a restriction.", nameof(decision));

        return new GuardianProtectiveRestriction(
            restrictionId,
            decision.DecisionId,
            decision.Identity,
            decision.TargetId,
            decision.ScopeKind,
            decision.ScopeId,
            MapSeverity(decision.ConsequenceClass),
            decision.ProtectiveAction,
            decision.TriggerCode,
            decision.EvidenceReference,
            decision.AuthorityReference,
            decision.PolicyReference,
            effectiveAt,
            reviewDeadline,
            true,
            true);
    }

    private static GuardianRestrictionSeverity MapSeverity(GuardianConsequenceClass consequenceClass)
        => consequenceClass switch
        {
            GuardianConsequenceClass.Low => GuardianRestrictionSeverity.Moderate,
            GuardianConsequenceClass.Moderate => GuardianRestrictionSeverity.Moderate,
            GuardianConsequenceClass.High => GuardianRestrictionSeverity.High,
            GuardianConsequenceClass.Critical => GuardianRestrictionSeverity.Critical,
            _ => GuardianRestrictionSeverity.Critical
        };

    private static bool CanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        return value.All(ch => !char.IsControl(ch) && !char.IsWhiteSpace(ch));
    }
}

internal static class GuardianRestrictionIdentity
{
    internal static string Compute(GuardianProtectiveRestriction restriction)
    {
        var canonical = string.Join("\n", new[]
        {
            restriction.RestrictionId,
            restriction.SourceDecisionId,
            restriction.SourceDecisionIdentity,
            restriction.TargetId,
            ((int)restriction.ScopeKind).ToString(CultureInfo.InvariantCulture),
            restriction.ScopeId,
            ((int)restriction.Severity).ToString(CultureInfo.InvariantCulture),
            ((int)restriction.EnforcementAction).ToString(CultureInfo.InvariantCulture),
            restriction.TriggerCode,
            restriction.EvidenceReference,
            restriction.AuthorityReference,
            restriction.PolicyReference,
            restriction.EffectiveAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            restriction.ReviewDeadline?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture) ?? "NONE",
            restriction.PersistAcrossRestart ? "1" : "0",
            restriction.SubjectSelfReleaseForbidden ? "1" : "0"
        });

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
