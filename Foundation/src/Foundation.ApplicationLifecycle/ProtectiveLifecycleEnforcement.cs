using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.ApplicationLifecycle;

public enum ProtectiveLifecycleTarget
{
    Restricted = 1,
    Suspended = 2,
    Stopped = 3
}

public enum ProtectiveLifecycleEvidenceState
{
    Valid = 1,
    Missing = 2,
    Invalid = 3,
    Ambiguous = 4,
    Stale = 5
}

public sealed record ProtectiveLifecycleRequest(
    string RequestId,
    string SubjectId,
    string RestrictionId,
    string RestrictionIntegrityEvidence,
    string AuthorityReference,
    string TriggerEvidence,
    string ProtectiveMode,
    ProtectiveLifecycleEvidenceState RestrictionEvidenceState,
    ProtectiveLifecycleEvidenceState AuthorityEvidenceState,
    DateTimeOffset RestrictionEffectiveTime,
    DateTimeOffset RequestTime)
{
    public string Identity => ProtectiveLifecycleIdentity.Compute(this);
}

public sealed record ProtectiveLifecycleOutcome(
    bool Success,
    string Reason,
    ProtectiveLifecycleTarget Target,
    bool IsolationRequired,
    bool NewExecutionAllowed,
    bool RemainsRestricted,
    string RequestIdentity,
    string OutcomeIdentity);

public static class ProtectiveLifecycleReason
{
    public const string Enforced = "PROTECTIVE_LIFECYCLE_ENFORCED";
    public const string MissingRequest = "MISSING_PROTECTIVE_LIFECYCLE_REQUEST";
    public const string InvalidIdentity = "INVALID_PROTECTIVE_LIFECYCLE_IDENTITY";
    public const string RestrictionEvidenceUnavailable = "RESTRICTION_EVIDENCE_UNAVAILABLE";
    public const string AuthorityEvidenceUnavailable = "PROTECTIVE_AUTHORITY_EVIDENCE_UNAVAILABLE";
    public const string RestrictionNotEffective = "RESTRICTION_NOT_YET_EFFECTIVE";
    public const string UnsupportedMode = "UNSUPPORTED_PROTECTIVE_MODE";
}

public static class ProtectiveLifecycleEnforcer
{
    public static ProtectiveLifecycleOutcome Enforce(ProtectiveLifecycleRequest? request)
    {
        if (request is null)
            return Failure(ProtectiveLifecycleReason.MissingRequest, "missing-request");

        if (!CanonicalToken(request.RequestId) ||
            !CanonicalToken(request.SubjectId) ||
            !CanonicalToken(request.RestrictionId) ||
            !CanonicalToken(request.RestrictionIntegrityEvidence) ||
            !CanonicalToken(request.AuthorityReference) ||
            !CanonicalToken(request.TriggerEvidence) ||
            !CanonicalToken(request.ProtectiveMode) ||
            request.RestrictionEffectiveTime == default ||
            request.RequestTime == default)
        {
            return Failure(ProtectiveLifecycleReason.InvalidIdentity, request.Identity);
        }

        if (request.RestrictionEvidenceState != ProtectiveLifecycleEvidenceState.Valid)
            return Failure(ProtectiveLifecycleReason.RestrictionEvidenceUnavailable, request.Identity);

        if (request.AuthorityEvidenceState != ProtectiveLifecycleEvidenceState.Valid)
            return Failure(ProtectiveLifecycleReason.AuthorityEvidenceUnavailable, request.Identity);

        if (request.RequestTime < request.RestrictionEffectiveTime)
            return Failure(ProtectiveLifecycleReason.RestrictionNotEffective, request.Identity);

        var mapping = request.ProtectiveMode switch
        {
            "RESTRICTED" => (ProtectiveLifecycleTarget.Restricted, false),
            "ISOLATED" => (ProtectiveLifecycleTarget.Suspended, true),
            "SUSPENDED" => (ProtectiveLifecycleTarget.Suspended, false),
            "SAFE" => (ProtectiveLifecycleTarget.Stopped, true),
            "STOPPED" => (ProtectiveLifecycleTarget.Stopped, true),
            _ => ((ProtectiveLifecycleTarget)0, false)
        };

        if (!Enum.IsDefined(mapping.Item1))
            return Failure(ProtectiveLifecycleReason.UnsupportedMode, request.Identity);

        var outcomeIdentity = ProtectiveLifecycleIdentity.ComputeOutcome(
            request.Identity,
            ProtectiveLifecycleReason.Enforced,
            mapping.Item1,
            mapping.Item2,
            false,
            true);

        return new ProtectiveLifecycleOutcome(
            true,
            ProtectiveLifecycleReason.Enforced,
            mapping.Item1,
            mapping.Item2,
            false,
            true,
            request.Identity,
            outcomeIdentity);
    }

    private static ProtectiveLifecycleOutcome Failure(string reason, string requestIdentity)
    {
        var outcomeIdentity = ProtectiveLifecycleIdentity.ComputeOutcome(
            requestIdentity,
            reason,
            ProtectiveLifecycleTarget.Stopped,
            true,
            false,
            true);

        return new ProtectiveLifecycleOutcome(
            false,
            reason,
            ProtectiveLifecycleTarget.Stopped,
            true,
            false,
            true,
            requestIdentity,
            outcomeIdentity);
    }

    private static bool CanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        foreach (var ch in value)
        {
            if (char.IsControl(ch) || char.IsWhiteSpace(ch))
                return false;
        }
        return true;
    }
}

internal static class ProtectiveLifecycleIdentity
{
    internal static string Compute(ProtectiveLifecycleRequest request)
    {
        var canonical = string.Join("\n", new[]
        {
            request.RequestId,
            request.SubjectId,
            request.RestrictionId,
            request.RestrictionIntegrityEvidence,
            request.AuthorityReference,
            request.TriggerEvidence,
            request.ProtectiveMode,
            ((int)request.RestrictionEvidenceState).ToString(CultureInfo.InvariantCulture),
            ((int)request.AuthorityEvidenceState).ToString(CultureInfo.InvariantCulture),
            request.RestrictionEffectiveTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            request.RequestTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });
        return "protective-lifecycle-request/sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    internal static string ComputeOutcome(
        string requestIdentity,
        string reason,
        ProtectiveLifecycleTarget target,
        bool isolationRequired,
        bool newExecutionAllowed,
        bool remainsRestricted)
    {
        var canonical = string.Join("\n", new[]
        {
            requestIdentity,
            reason,
            ((int)target).ToString(CultureInfo.InvariantCulture),
            isolationRequired ? "1" : "0",
            newExecutionAllowed ? "1" : "0",
            remainsRestricted ? "1" : "0"
        });
        return "protective-lifecycle-outcome/sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}
