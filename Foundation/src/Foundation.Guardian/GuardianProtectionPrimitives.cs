using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Guardian;

public enum GuardianProtectiveMode
{
    Normal = 1,
    Heightened = 2,
    Restricted = 3,
    Safe = 4,
    RecoveryGuard = 5
}

public enum GuardianProtectiveAction
{
    Observe = 1,
    Warn = 2,
    Restrict = 3,
    Isolate = 4,
    Suspend = 5,
    RequestEmergencyStop = 6
}

public enum GuardianConsequenceClass
{
    Low = 1,
    Moderate = 2,
    High = 3,
    Critical = 4
}

public enum GuardianScopeKind
{
    Component = 1,
    Application = 2,
    FoundationSubsystem = 3,
    FalconWide = 4
}

public sealed record GuardianProtectiveDecision(
    string DecisionId,
    string TargetId,
    GuardianScopeKind ScopeKind,
    string ScopeId,
    GuardianProtectiveMode ProtectiveMode,
    GuardianProtectiveAction ProtectiveAction,
    GuardianConsequenceClass ConsequenceClass,
    string TriggerCode,
    string EvidenceReference,
    string AuthorityReference,
    string PolicyReference,
    string Reason,
    string ExpectedReleaseConditions,
    DateTimeOffset DecisionTime)
{
    public string Identity => GuardianIdentity.Compute(this);
}

public sealed record GuardianValidationOutcome(bool Success, string Reason)
{
    public static GuardianValidationOutcome Pass() => new(true, "PASS");
    public static GuardianValidationOutcome Fail(string reason) => new(false, reason);
}

public static class GuardianProtectiveDecisionValidator
{
    public static GuardianValidationOutcome Validate(GuardianProtectiveDecision? decision)
    {
        if (decision is null)
            return GuardianValidationOutcome.Fail("MISSING_DECISION");

        if (!CanonicalToken(decision.DecisionId))
            return GuardianValidationOutcome.Fail("INVALID_DECISION_ID");
        if (!CanonicalToken(decision.TargetId))
            return GuardianValidationOutcome.Fail("INVALID_TARGET_ID");
        if (!CanonicalToken(decision.ScopeId))
            return GuardianValidationOutcome.Fail("INVALID_SCOPE_ID");
        if (!CanonicalToken(decision.TriggerCode))
            return GuardianValidationOutcome.Fail("INVALID_TRIGGER_CODE");
        if (!CanonicalToken(decision.EvidenceReference))
            return GuardianValidationOutcome.Fail("INVALID_EVIDENCE_REFERENCE");
        if (!CanonicalToken(decision.AuthorityReference))
            return GuardianValidationOutcome.Fail("INVALID_AUTHORITY_REFERENCE");
        if (!CanonicalToken(decision.PolicyReference))
            return GuardianValidationOutcome.Fail("INVALID_POLICY_REFERENCE");
        if (!CanonicalText(decision.Reason))
            return GuardianValidationOutcome.Fail("INVALID_REASON");
        if (!CanonicalText(decision.ExpectedReleaseConditions))
            return GuardianValidationOutcome.Fail("INVALID_RELEASE_CONDITIONS");

        if (!Enum.IsDefined(decision.ScopeKind))
            return GuardianValidationOutcome.Fail("INVALID_SCOPE_KIND");
        if (!Enum.IsDefined(decision.ProtectiveMode))
            return GuardianValidationOutcome.Fail("INVALID_PROTECTIVE_MODE");
        if (!Enum.IsDefined(decision.ProtectiveAction))
            return GuardianValidationOutcome.Fail("INVALID_PROTECTIVE_ACTION");
        if (!Enum.IsDefined(decision.ConsequenceClass))
            return GuardianValidationOutcome.Fail("INVALID_CONSEQUENCE_CLASS");

        if (decision.DecisionTime == default)
            return GuardianValidationOutcome.Fail("INVALID_DECISION_TIME");

        if (decision.ProtectiveMode == GuardianProtectiveMode.Normal &&
            decision.ProtectiveAction is GuardianProtectiveAction.Restrict or
                GuardianProtectiveAction.Isolate or
                GuardianProtectiveAction.Suspend or
                GuardianProtectiveAction.RequestEmergencyStop)
        {
            return GuardianValidationOutcome.Fail("NORMAL_MODE_CONTRADICTS_RESTRICTIVE_ACTION");
        }

        if (decision.ScopeKind == GuardianScopeKind.FalconWide &&
            decision.ConsequenceClass is GuardianConsequenceClass.Low or GuardianConsequenceClass.Moderate)
        {
            return GuardianValidationOutcome.Fail("FALCON_WIDE_SCOPE_REQUIRES_HIGHER_CONSEQUENCE");
        }

        return GuardianValidationOutcome.Pass();
    }

    private static bool CanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        return value.All(ch => !char.IsControl(ch) && !char.IsWhiteSpace(ch));
    }

    private static bool CanonicalText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        return value.All(ch => !char.IsControl(ch) || ch is '\r' or '\n' or '\t');
    }
}

internal static class GuardianIdentity
{
    internal static string Compute(GuardianProtectiveDecision decision)
    {
        var canonical = string.Join("\n", new[]
        {
            decision.DecisionId,
            decision.TargetId,
            ((int)decision.ScopeKind).ToString(CultureInfo.InvariantCulture),
            decision.ScopeId,
            ((int)decision.ProtectiveMode).ToString(CultureInfo.InvariantCulture),
            ((int)decision.ProtectiveAction).ToString(CultureInfo.InvariantCulture),
            ((int)decision.ConsequenceClass).ToString(CultureInfo.InvariantCulture),
            decision.TriggerCode,
            decision.EvidenceReference,
            decision.AuthorityReference,
            decision.PolicyReference,
            decision.Reason,
            decision.ExpectedReleaseConditions,
            decision.DecisionTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
