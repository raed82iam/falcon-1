using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public static class IndependentEmergencyAuthorityReason
{
    public const string EvidenceUnavailable = "AUTHORITY_INDEPENDENT_EMERGENCY_CONTROL_EVIDENCE_UNAVAILABLE";
    public const string DecisionMalformed = "AUTHORITY_INDEPENDENT_EMERGENCY_CONTROL_DECISION_MALFORMED";
    public const string Restricted = "AUTHORITY_RESTRICTED_BY_INDEPENDENT_EMERGENCY_CONTROL";
}

public sealed class IndependentEmergencyControlAuthorityEnforcer
{
    private readonly ProtectiveRestrictionAuthorityEnforcer _protective = new();

    public AuthorityResult Evaluate(
        AuthorityRequest? request,
        AuthorityEvaluationContext? context,
        IReadOnlyCollection<RestrictionRecord>? restrictions,
        IReadOnlyCollection<IndependentEmergencyControlDecision>? emergencyControls)
    {
        var baseline = _protective.Evaluate(request, context, restrictions);

        if (emergencyControls is null)
        {
            return Deny(
                baseline,
                IndependentEmergencyAuthorityReason.EvidenceUnavailable,
                "INDEPENDENT_EMERGENCY_CONTROL_EVIDENCE_UNAVAILABLE",
                "missing-independent-emergency-control-evidence");
        }

        foreach (var control in emergencyControls)
        {
            if (!IndependentEmergencyControlRuntime.ValidateDecision(control))
            {
                return Deny(
                    baseline,
                    IndependentEmergencyAuthorityReason.DecisionMalformed,
                    "INDEPENDENT_EMERGENCY_CONTROL_DECISION_MALFORMED",
                    control?.BlastRadiusEvidenceIdentity ?? "malformed-independent-emergency-control");
            }
        }

        if (!string.Equals(baseline.Decision, AuthorityDecision.Allow, StringComparison.Ordinal))
            return baseline;

        if (request is null)
        {
            return Deny(
                baseline,
                AuthorityReason.DefaultDeny,
                "NO_EXECUTION_AUTHORITY",
                "missing-authority-request");
        }

        var matchingControls = emergencyControls
            .Where(control => AppliesTo(control, request))
            .OrderByDescending(control => ScopeRank(control.EffectiveScopeKind))
            .ThenBy(control => control.RequestId, StringComparer.Ordinal)
            .ToArray();

        foreach (var control in matchingControls)
        {
            if (!ActionAllowedDuringContainment(request.Action))
            {
                return Deny(
                    baseline,
                    IndependentEmergencyAuthorityReason.Restricted,
                    "EMERGENCY_CONTROL=" + control.RequestId +
                    "|SCOPE=" + control.EffectiveScopeKind.ToString().ToUpperInvariant() +
                    "|SCOPE_ID=" + control.EffectiveScopeId,
                    control.BlastRadiusEvidenceIdentity);
            }
        }

        return baseline;
    }

    private static bool AppliesTo(IndependentEmergencyControlDecision control, AuthorityRequest request)
    {
        return control.EffectiveScopeKind switch
        {
            EmergencyControlScopeKind.FalconWide => true,
            EmergencyControlScopeKind.Principal =>
                string.Equals(control.TargetSubjectId, request.ActorIdentity, StringComparison.Ordinal),
            EmergencyControlScopeKind.Application =>
                ScopeContains(control.EffectiveScopeId, request.RequestedScope),
            _ => true
        };
    }

    private static bool ScopeContains(string governedScope, string requestedScope)
    {
        if (string.IsNullOrWhiteSpace(governedScope) || string.IsNullOrWhiteSpace(requestedScope))
            return false;

        var governed = governedScope.Trim();
        var requested = requestedScope.Trim();
        return string.Equals(governed, requested, StringComparison.Ordinal) ||
            requested.StartsWith(governed + ":", StringComparison.Ordinal);
    }

    private static bool ActionAllowedDuringContainment(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
            return false;

        return ProtectiveSafeStateContractPolicy.CanonicalAllowedSafeActions
            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(token => string.Equals(token, action, StringComparison.Ordinal));
    }

    private static int ScopeRank(EmergencyControlScopeKind scopeKind) => scopeKind switch
    {
        EmergencyControlScopeKind.FalconWide => 3,
        EmergencyControlScopeKind.Application => 2,
        EmergencyControlScopeKind.Principal => 1,
        _ => 4
    };

    private static AuthorityResult Deny(
        AuthorityResult baseline,
        string reason,
        string constraint,
        string evidence)
    {
        var decisionId = "authority-independent-emergency-decision/sha256/" + Convert.ToHexString(
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
