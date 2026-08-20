using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public static class ProtectiveAuthorityReason
{
    public const string RestrictionEvidenceUnavailable = "AUTHORITY_PROTECTIVE_RESTRICTION_EVIDENCE_UNAVAILABLE";
    public const string RestrictionMalformed = "AUTHORITY_PROTECTIVE_RESTRICTION_MALFORMED";
    public const string RestrictedByGuardian = "AUTHORITY_RESTRICTED_BY_GUARDIAN";
}

public sealed class ProtectiveRestrictionAuthorityEnforcer
{
    private readonly DefaultDenyAuthorityEngine _engine = new();

    public AuthorityResult Evaluate(
        AuthorityRequest? request,
        AuthorityEvaluationContext? context,
        IReadOnlyCollection<RestrictionRecord>? restrictions)
    {
        var baseline = _engine.Evaluate(request, context);

        if (restrictions is null)
            return Deny(baseline, ProtectiveAuthorityReason.RestrictionEvidenceUnavailable, "PROTECTIVE_RESTRICTION_EVIDENCE_UNAVAILABLE", "missing-restriction-evidence");

        foreach (var restriction in restrictions)
        {
            if (!IsStructurallyValid(restriction))
                return Deny(baseline, ProtectiveAuthorityReason.RestrictionMalformed, "PROTECTIVE_RESTRICTION_MALFORMED", restriction?.IntegrityEvidence ?? "malformed-restriction");
        }

        if (!string.Equals(baseline.Decision, AuthorityDecision.Allow, StringComparison.Ordinal))
            return baseline;

        if (request is null || context is null)
            return Deny(baseline, AuthorityReason.DefaultDeny, "NO_EXECUTION_AUTHORITY", "missing-request-or-context");

        var activeMatching = restrictions
            .Where(r => IsActiveAt(r, context.ObservationTime))
            .Where(r => string.Equals(r.SubjectId, request.ActorIdentity, StringComparison.Ordinal))
            .OrderByDescending(r => RestrictionRank(r.ProtectiveMode))
            .ThenBy(r => r.RestrictionId, StringComparer.Ordinal)
            .ToArray();

        foreach (var restriction in activeMatching)
        {
            if (!ActionExplicitlyAllowed(restriction.AllowedSafeActions, request.Action))
            {
                return Deny(
                    baseline,
                    ProtectiveAuthorityReason.RestrictedByGuardian,
                    "GUARDIAN_RESTRICTION=" + restriction.RestrictionId + "|MODE=" + restriction.ProtectiveMode,
                    restriction.IntegrityEvidence);
            }
        }

        return baseline;
    }

    private static bool IsStructurallyValid(RestrictionRecord? value)
    {
        if (value is null)
            return false;

        return
            string.Equals(value.Version, ContractVersions.Con011, StringComparison.Ordinal) &&
            !string.IsNullOrWhiteSpace(value.RestrictionId) &&
            !string.IsNullOrWhiteSpace(value.SubjectId) &&
            !string.IsNullOrWhiteSpace(value.MandateReference) &&
            !string.IsNullOrWhiteSpace(value.TriggerEvidence) &&
            IsProtectiveMode(value.ProtectiveMode) &&
            !string.IsNullOrWhiteSpace(value.AllowedSafeActions) &&
            (value.ProtectiveMode != "SAFE" || ProtectiveSafeStateContractPolicy.IsCanonicalSafeAllowlist(value.AllowedSafeActions)) &&
            !string.IsNullOrWhiteSpace(value.ProhibitedActions) &&
            !string.IsNullOrWhiteSpace(value.ReleaseConditions) &&
            !string.IsNullOrWhiteSpace(value.ReleaseAuthority) &&
            string.Equals(value.Result, "IMPOSED", StringComparison.Ordinal) &&
            !string.IsNullOrWhiteSpace(value.IntegrityEvidence) &&
            value.EffectiveTime != default &&
            value.Expiry > value.EffectiveTime;
    }

    private static bool IsActiveAt(RestrictionRecord value, DateTimeOffset observationTime)
        => observationTime >= value.EffectiveTime && observationTime < value.Expiry;

    private static bool ActionExplicitlyAllowed(string allowedSafeActions, string action)
        => SplitTokens(allowedSafeActions).Any(token => string.Equals(token, action, StringComparison.Ordinal));

    private static IEnumerable<string> SplitTokens(string value)
        => value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool IsProtectiveMode(string value)
        => value is "RESTRICTED" or "ISOLATED" or "SAFE" or "RECOVERY_GUARD";

    private static int RestrictionRank(string value)
        => value switch
        {
            "RECOVERY_GUARD" => 4,
            "SAFE" => 3,
            "ISOLATED" => 2,
            "RESTRICTED" => 1,
            _ => 0
        };

    private static AuthorityResult Deny(
        AuthorityResult baseline,
        string reason,
        string constraint,
        string evidence)
    {
        var decisionId = "authority-protective-decision/sha256/" + Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n", baseline.DecisionId, reason, constraint, evidence))));

        return baseline with
        {
            DecisionId = decisionId,
            Decision = AuthorityDecision.Deny,
            EffectiveScope = "NONE",
            Constraints = constraint,
            Reason = reason,
            EvidenceReference = string.IsNullOrWhiteSpace(evidence) ? baseline.EvidenceReference : evidence
        };
    }
}
