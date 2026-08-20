using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Guardian;

public enum GuardianCredibleHarm
{
    None = 1,
    Low = 2,
    Moderate = 3,
    High = 4,
    Critical = 5
}

public enum GuardianUncertainty
{
    Low = 1,
    Moderate = 2,
    High = 3,
    Unknown = 4
}

public enum GuardianReversibility
{
    Easy = 1,
    Bounded = 2,
    Difficult = 3,
    Irreversible = 4,
    Unknown = 5
}

public enum GuardianEvidenceIndependence
{
    Independent = 1,
    Mixed = 2,
    SubjectOnly = 3,
    Unknown = 4
}

public sealed record GuardianProtectiveEvaluationRequest(
    string EvaluationId,
    string TargetId,
    GuardianScopeKind ScopeKind,
    string ScopeId,
    GuardianCredibleHarm CredibleHarm,
    GuardianUncertainty Uncertainty,
    GuardianReversibility Reversibility,
    GuardianEvidenceIndependence EvidenceIndependence,
    bool MandatoryInterventionThresholdMet,
    string TriggerCode,
    string EvidenceReference,
    string AuthorityReference,
    string PolicyReference,
    DateTimeOffset EvaluationTime)
{
    public string Identity => GuardianProtectiveEvaluationIdentity.Compute(this);
}

public sealed record GuardianProtectiveEvaluationOutcome(
    bool Success,
    string Reason,
    GuardianProtectiveDecision? Decision,
    bool InterventionRequired,
    bool ProtectionFailureObservable,
    int ProtectiveScore)
{
    public static GuardianProtectiveEvaluationOutcome Fail(
        string reason,
        bool mandatoryInterventionThresholdMet)
        => new(
            false,
            reason,
            null,
            false,
            mandatoryInterventionThresholdMet,
            0);
}

public static class GuardianProtectiveEvaluationRuntime
{
    public static GuardianProtectiveEvaluationOutcome Evaluate(
        GuardianProtectiveEvaluationRequest? request)
    {
        if (request is null)
            return GuardianProtectiveEvaluationOutcome.Fail("MISSING_EVALUATION_REQUEST", false);

        var validationFailure = Validate(request);
        if (validationFailure is not null)
        {
            return GuardianProtectiveEvaluationOutcome.Fail(
                validationFailure,
                request.MandatoryInterventionThresholdMet);
        }

        var score = ComputeProtectiveScore(request);
        var selection = SelectProtection(score);

        if (request.MandatoryInterventionThresholdMet &&
            selection.Action is GuardianProtectiveAction.Observe or GuardianProtectiveAction.Warn)
        {
            selection = new ProtectionSelection(
                GuardianProtectiveMode.Restricted,
                GuardianProtectiveAction.Restrict,
                GuardianConsequenceClass.High,
                true);
            score = Math.Max(score, 7);
        }

        if (request.EvidenceIndependence is GuardianEvidenceIndependence.SubjectOnly or GuardianEvidenceIndependence.Unknown &&
            selection.Action is GuardianProtectiveAction.Observe or GuardianProtectiveAction.Warn)
        {
            selection = new ProtectionSelection(
                GuardianProtectiveMode.Restricted,
                GuardianProtectiveAction.Restrict,
                GuardianConsequenceClass.High,
                true);
            score = Math.Max(score, 7);
        }

        var decision = new GuardianProtectiveDecision(
            request.EvaluationId + ":protective-decision",
            request.TargetId,
            request.ScopeKind,
            request.ScopeId,
            selection.Mode,
            selection.Action,
            selection.ConsequenceClass,
            request.TriggerCode,
            request.EvidenceReference,
            request.AuthorityReference,
            request.PolicyReference,
            BuildReason(request, score),
            "Independent governed evidence must establish that the triggering condition is resolved or acceptably contained before release.",
            request.EvaluationTime);

        var decisionValidation = GuardianProtectiveDecisionValidator.Validate(decision);
        if (!decisionValidation.Success)
        {
            return GuardianProtectiveEvaluationOutcome.Fail(
                "GENERATED_DECISION_INVALID:" + decisionValidation.Reason,
                request.MandatoryInterventionThresholdMet);
        }

        return new GuardianProtectiveEvaluationOutcome(
            true,
            "PASS",
            decision,
            selection.InterventionRequired,
            false,
            score);
    }

    private static string? Validate(GuardianProtectiveEvaluationRequest request)
    {
        if (!CanonicalToken(request.EvaluationId))
            return "INVALID_EVALUATION_ID";
        if (!CanonicalToken(request.TargetId))
            return "INVALID_TARGET_ID";
        if (!CanonicalToken(request.ScopeId))
            return "INVALID_SCOPE_ID";
        if (!CanonicalToken(request.TriggerCode))
            return "INVALID_TRIGGER_CODE";
        if (!CanonicalToken(request.EvidenceReference))
            return "INVALID_EVIDENCE_REFERENCE";
        if (!CanonicalToken(request.AuthorityReference))
            return "INVALID_AUTHORITY_REFERENCE";
        if (!CanonicalToken(request.PolicyReference))
            return "INVALID_POLICY_REFERENCE";

        if (!Enum.IsDefined(request.ScopeKind))
            return "INVALID_SCOPE_KIND";
        if (!Enum.IsDefined(request.CredibleHarm))
            return "INVALID_CREDIBLE_HARM";
        if (!Enum.IsDefined(request.Uncertainty))
            return "INVALID_UNCERTAINTY";
        if (!Enum.IsDefined(request.Reversibility))
            return "INVALID_REVERSIBILITY";
        if (!Enum.IsDefined(request.EvidenceIndependence))
            return "INVALID_EVIDENCE_INDEPENDENCE";
        if (request.EvaluationTime == default)
            return "INVALID_EVALUATION_TIME";

        return null;
    }

    private static int ComputeProtectiveScore(GuardianProtectiveEvaluationRequest request)
    {
        var harm = request.CredibleHarm switch
        {
            GuardianCredibleHarm.None => 0,
            GuardianCredibleHarm.Low => 1,
            GuardianCredibleHarm.Moderate => 2,
            GuardianCredibleHarm.High => 3,
            GuardianCredibleHarm.Critical => 4,
            _ => 4
        };

        var uncertainty = request.Uncertainty switch
        {
            GuardianUncertainty.Low => 0,
            GuardianUncertainty.Moderate => 1,
            GuardianUncertainty.High => 2,
            GuardianUncertainty.Unknown => 3,
            _ => 3
        };

        var reversibility = request.Reversibility switch
        {
            GuardianReversibility.Easy => 0,
            GuardianReversibility.Bounded => 1,
            GuardianReversibility.Difficult => 2,
            GuardianReversibility.Irreversible => 3,
            GuardianReversibility.Unknown => 3,
            _ => 3
        };

        var evidence = request.EvidenceIndependence switch
        {
            GuardianEvidenceIndependence.Independent => 0,
            GuardianEvidenceIndependence.Mixed => 1,
            GuardianEvidenceIndependence.SubjectOnly => 2,
            GuardianEvidenceIndependence.Unknown => 2,
            _ => 2
        };

        var score = (harm * 2) + uncertainty + reversibility + evidence;

        if (request.MandatoryInterventionThresholdMet)
            score = Math.Max(score, 7);

        if (request.EvidenceIndependence is GuardianEvidenceIndependence.SubjectOnly or GuardianEvidenceIndependence.Unknown)
            score = Math.Max(score, 7);

        return score;
    }

    private static ProtectionSelection SelectProtection(int score)
        => score switch
        {
            <= 2 => new ProtectionSelection(
                GuardianProtectiveMode.Normal,
                GuardianProtectiveAction.Observe,
                GuardianConsequenceClass.Low,
                false),
            <= 4 => new ProtectionSelection(
                GuardianProtectiveMode.Heightened,
                GuardianProtectiveAction.Warn,
                GuardianConsequenceClass.Moderate,
                true),
            <= 7 => new ProtectionSelection(
                GuardianProtectiveMode.Restricted,
                GuardianProtectiveAction.Restrict,
                GuardianConsequenceClass.High,
                true),
            <= 10 => new ProtectionSelection(
                GuardianProtectiveMode.Restricted,
                GuardianProtectiveAction.Suspend,
                GuardianConsequenceClass.High,
                true),
            <= 12 => new ProtectionSelection(
                GuardianProtectiveMode.Safe,
                GuardianProtectiveAction.Isolate,
                GuardianConsequenceClass.Critical,
                true),
            _ => new ProtectionSelection(
                GuardianProtectiveMode.Safe,
                GuardianProtectiveAction.RequestEmergencyStop,
                GuardianConsequenceClass.Critical,
                true)
        };

    private static string BuildReason(
        GuardianProtectiveEvaluationRequest request,
        int score)
        => string.Join(
            "; ",
            "Guardian protective evaluation",
            "harm=" + request.CredibleHarm,
            "uncertainty=" + request.Uncertainty,
            "reversibility=" + request.Reversibility,
            "evidence=" + request.EvidenceIndependence,
            "mandatory=" + (request.MandatoryInterventionThresholdMet ? "true" : "false"),
            "score=" + score.ToString(CultureInfo.InvariantCulture));

    private static bool CanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        return value.All(ch => !char.IsControl(ch) && !char.IsWhiteSpace(ch));
    }

    private sealed record ProtectionSelection(
        GuardianProtectiveMode Mode,
        GuardianProtectiveAction Action,
        GuardianConsequenceClass ConsequenceClass,
        bool InterventionRequired);
}

internal static class GuardianProtectiveEvaluationIdentity
{
    internal static string Compute(GuardianProtectiveEvaluationRequest request)
    {
        var canonical = string.Join("\n", new[]
        {
            request.EvaluationId,
            request.TargetId,
            ((int)request.ScopeKind).ToString(CultureInfo.InvariantCulture),
            request.ScopeId,
            ((int)request.CredibleHarm).ToString(CultureInfo.InvariantCulture),
            ((int)request.Uncertainty).ToString(CultureInfo.InvariantCulture),
            ((int)request.Reversibility).ToString(CultureInfo.InvariantCulture),
            ((int)request.EvidenceIndependence).ToString(CultureInfo.InvariantCulture),
            request.MandatoryInterventionThresholdMet ? "1" : "0",
            request.TriggerCode,
            request.EvidenceReference,
            request.AuthorityReference,
            request.PolicyReference,
            request.EvaluationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
