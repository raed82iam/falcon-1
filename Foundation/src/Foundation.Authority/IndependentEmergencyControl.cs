using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public enum EmergencyControlAction
{
    Hold = 1,
    DenyNewActivity = 2,
    IsolateTarget = 3,
    EnterPlatformSafe = 4,
    EmergencyStop = 5
}

public enum EmergencyControlScopeKind
{
    Principal = 1,
    Application = 2,
    FalconWide = 3
}

public enum EmergencyTrustState
{
    Trustworthy = 1,
    Unavailable = 2,
    Contradictory = 3,
    Compromised = 4
}

public enum EmergencyPropagationState
{
    Excluded = 1,
    Possible = 2,
    Unknown = 3
}

public sealed record IndependentEmergencyControlRequest(
    string RequestId,
    string ActorIdentity,
    string TargetSubjectId,
    EmergencyControlAction Action,
    EmergencyControlScopeKind RequestedScopeKind,
    string RequestedScopeId,
    string ProtectiveMandateReference,
    string Correlation,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry)
{
    public string Identity => IndependentEmergencyControlIdentity.ComputeRequest(this);
}

public sealed record EmergencyBlastRadiusEvidence(
    string EvidenceId,
    string TargetSubjectId,
    string RequestedScopeId,
    EmergencyTrustState LocalBoundaryTrust,
    EmergencyPropagationState PropagationState,
    EmergencyTrustState UnaffectedScopeTrust,
    EmergencyTrustState EvidenceSourceTrust,
    bool GuardianCompromiseSuspected,
    bool GuardianEvidenceSoleSource,
    DateTimeOffset ObservedAt,
    DateTimeOffset Expiry)
{
    public string Identity => IndependentEmergencyControlIdentity.ComputeEvidence(this);
}

public sealed class IndependentEmergencyControlDecision
{
    internal IndependentEmergencyControlDecision(
        bool accepted,
        string reason,
        string requestId,
        string requestIdentity,
        string targetSubjectId,
        EmergencyControlAction action,
        EmergencyControlScopeKind requestedScopeKind,
        string requestedScopeId,
        EmergencyControlScopeKind effectiveScopeKind,
        string effectiveScopeId,
        bool containmentRequired,
        bool unaffectedOperationEligible,
        bool unaffectedOperationStillRequiresAuthority,
        string protectiveMandateReference,
        string authorityDecisionId,
        string authorityEvidenceReference,
        string blastRadiusEvidenceIdentity,
        DateTimeOffset decisionTime,
        DateTimeOffset reviewDeadline)
    {
        Accepted = accepted;
        Reason = reason;
        RequestId = requestId;
        RequestIdentity = requestIdentity;
        TargetSubjectId = targetSubjectId;
        Action = action;
        RequestedScopeKind = requestedScopeKind;
        RequestedScopeId = requestedScopeId;
        EffectiveScopeKind = effectiveScopeKind;
        EffectiveScopeId = effectiveScopeId;
        ContainmentRequired = containmentRequired;
        UnaffectedOperationEligible = unaffectedOperationEligible;
        UnaffectedOperationStillRequiresAuthority = unaffectedOperationStillRequiresAuthority;
        ProtectiveMandateReference = protectiveMandateReference;
        AuthorityDecisionId = authorityDecisionId;
        AuthorityEvidenceReference = authorityEvidenceReference;
        BlastRadiusEvidenceIdentity = blastRadiusEvidenceIdentity;
        DecisionTime = decisionTime;
        ReviewDeadline = reviewDeadline;
    }

    public bool Accepted { get; }
    public string Reason { get; }
    public string RequestId { get; }
    public string RequestIdentity { get; }
    public string TargetSubjectId { get; }
    public EmergencyControlAction Action { get; }
    public EmergencyControlScopeKind RequestedScopeKind { get; }
    public string RequestedScopeId { get; }
    public EmergencyControlScopeKind EffectiveScopeKind { get; }
    public string EffectiveScopeId { get; }
    public bool ContainmentRequired { get; }
    public bool UnaffectedOperationEligible { get; }
    public bool UnaffectedOperationStillRequiresAuthority { get; }
    public string ProtectiveMandateReference { get; }
    public string AuthorityDecisionId { get; }
    public string AuthorityEvidenceReference { get; }
    public string BlastRadiusEvidenceIdentity { get; }
    public DateTimeOffset DecisionTime { get; }
    public DateTimeOffset ReviewDeadline { get; }
    public string Identity => IndependentEmergencyControlIdentity.ComputeDecision(this);
}

public static class IndependentEmergencyControlReason
{
    public const string AcceptedLocal = "INDEPENDENT_EMERGENCY_CONTROL_ACCEPTED_LOCAL";
    public const string AcceptedExpanded = "INDEPENDENT_EMERGENCY_CONTROL_ACCEPTED_EXPANDED";
    public const string InvalidRequest = "INDEPENDENT_EMERGENCY_CONTROL_INVALID_REQUEST";
    public const string InvalidAuthorityRequest = "INDEPENDENT_EMERGENCY_CONTROL_INVALID_AUTHORITY_REQUEST";
    public const string AuthorityNotGranted = "INDEPENDENT_EMERGENCY_CONTROL_AUTHORITY_NOT_GRANTED";
    public const string AuthorityBindingMismatch = "INDEPENDENT_EMERGENCY_CONTROL_AUTHORITY_BINDING_MISMATCH";
    public const string InvalidBlastRadiusEvidence = "INDEPENDENT_EMERGENCY_CONTROL_INVALID_BLAST_RADIUS_EVIDENCE";
}

public static class IndependentEmergencyControlRuntime
{
    public static IndependentEmergencyControlDecision Evaluate(
        IndependentEmergencyControlRequest? request,
        AuthorityRequest? authorityRequest,
        AuthorityEvaluationContext? authorityContext,
        EmergencyBlastRadiusEvidence? blastRadiusEvidence,
        DateTimeOffset observationTime)
    {
        if (!IsValidRequest(request, observationTime))
            return Reject(request, IndependentEmergencyControlReason.InvalidRequest, null, blastRadiusEvidence, observationTime);

        if (authorityRequest is null || ContractValidators.Validate(authorityRequest).Result != ValidationResult.Pass)
            return Reject(request, IndependentEmergencyControlReason.InvalidAuthorityRequest, null, blastRadiusEvidence, observationTime);

        if (authorityContext is null || authorityContext.ObservationTime != observationTime)
            return Reject(request, IndependentEmergencyControlReason.AuthorityNotGranted, null, blastRadiusEvidence, observationTime);

        var authorityResult = new DefaultDenyAuthorityEngine().Evaluate(authorityRequest, authorityContext);
        if (ContractValidators.Validate(authorityResult).Result != ValidationResult.Pass ||
            !string.Equals(authorityResult.Decision, AuthorityDecision.Allow, StringComparison.Ordinal) ||
            observationTime < authorityResult.DecisionTime ||
            observationTime >= authorityResult.Expiry)
        {
            return Reject(request, IndependentEmergencyControlReason.AuthorityNotGranted, authorityResult, blastRadiusEvidence, observationTime);
        }

        if (!AuthorityBindsExactly(request!, authorityRequest, authorityResult))
            return Reject(request, IndependentEmergencyControlReason.AuthorityBindingMismatch, authorityResult, blastRadiusEvidence, observationTime);

        if (!IsValidBlastRadiusEvidence(request!, blastRadiusEvidence, observationTime))
            return Reject(request, IndependentEmergencyControlReason.InvalidBlastRadiusEvidence, authorityResult, blastRadiusEvidence, observationTime);

        var blastEvidence = blastRadiusEvidence!;

        var preserveRequestedScope = request!.RequestedScopeKind == EmergencyControlScopeKind.FalconWide ||
            CanPreserveRequestedScope(blastEvidence);

        var effectiveScopeKind = preserveRequestedScope
            ? request.RequestedScopeKind
            : EmergencyControlScopeKind.FalconWide;
        var effectiveScopeId = effectiveScopeKind == EmergencyControlScopeKind.FalconWide
            ? "falcon:platform"
            : request.RequestedScopeId;
        var expanded = effectiveScopeKind != request.RequestedScopeKind ||
            !string.Equals(effectiveScopeId, request.RequestedScopeId, StringComparison.Ordinal);

        var unaffectedEligible = effectiveScopeKind != EmergencyControlScopeKind.FalconWide &&
            blastEvidence.UnaffectedScopeTrust == EmergencyTrustState.Trustworthy;

        var reviewDeadline = Minimum(request.Expiry, authorityResult.Expiry, blastEvidence.Expiry);

        return new IndependentEmergencyControlDecision(
            true,
            expanded ? IndependentEmergencyControlReason.AcceptedExpanded : IndependentEmergencyControlReason.AcceptedLocal,
            request.RequestId,
            request.Identity,
            request.TargetSubjectId,
            request.Action,
            request.RequestedScopeKind,
            request.RequestedScopeId,
            effectiveScopeKind,
            effectiveScopeId,
            true,
            unaffectedEligible,
            true,
            request.ProtectiveMandateReference,
            authorityResult.DecisionId,
            authorityResult.EvidenceReference,
            blastEvidence.Identity,
            observationTime,
            reviewDeadline);
    }

    public static RestrictionRecord CreateTargetRestrictionRecord(
        IndependentEmergencyControlDecision decision,
        string restrictionId)
    {
        if (!ValidateDecision(decision) || !CanonicalToken(restrictionId))
            throw new ArgumentException("A valid accepted emergency-control decision and canonical restriction ID are required.", nameof(decision));

        return new RestrictionRecord(
            restrictionId,
            ContractVersions.Con011,
            decision.TargetSubjectId,
            decision.ProtectiveMandateReference,
            decision.BlastRadiusEvidenceIdentity,
            "SAFE",
            ProtectiveSafeStateContractPolicy.CanonicalAllowedSafeActions,
            "*",
            "STAGE9_INDEPENDENT_RECOVERY_VALIDATION_AND_AUTHORIZED_RELEASE_REQUIRED",
            "INDEPENDENT_GOVERNED_RELEASE_AUTHORITY",
            "IMPOSED",
            decision.Identity,
            decision.DecisionTime,
            DateTimeOffset.MaxValue);
    }

    public static bool ValidateDecision(IndependentEmergencyControlDecision? decision)
    {
        if (decision is null || !decision.Accepted)
            return false;
        if (!CanonicalToken(decision.RequestId) ||
            !CanonicalToken(decision.RequestIdentity) ||
            !CanonicalToken(decision.TargetSubjectId) ||
            !Enum.IsDefined(decision.Action) ||
            !Enum.IsDefined(decision.RequestedScopeKind) ||
            !Enum.IsDefined(decision.EffectiveScopeKind) ||
            !CanonicalToken(decision.RequestedScopeId) ||
            !CanonicalToken(decision.EffectiveScopeId) ||
            !CanonicalToken(decision.ProtectiveMandateReference) ||
            !CanonicalToken(decision.AuthorityDecisionId) ||
            !CanonicalToken(decision.AuthorityEvidenceReference) ||
            !CanonicalToken(decision.BlastRadiusEvidenceIdentity) ||
            decision.DecisionTime == default ||
            decision.ReviewDeadline <= decision.DecisionTime ||
            !decision.ContainmentRequired ||
            !decision.UnaffectedOperationStillRequiresAuthority)
        {
            return false;
        }

        if (decision.EffectiveScopeKind == EmergencyControlScopeKind.FalconWide &&
            !string.Equals(decision.EffectiveScopeId, "falcon:platform", StringComparison.Ordinal))
        {
            return false;
        }

        return string.Equals(decision.Identity, IndependentEmergencyControlIdentity.ComputeDecision(decision), StringComparison.Ordinal);
    }

    public static string ToAuthorityAction(EmergencyControlAction action) => action switch
    {
        EmergencyControlAction.Hold => "HOLD",
        EmergencyControlAction.DenyNewActivity => "DENY_NEW_ACTIVITY",
        EmergencyControlAction.IsolateTarget => "ISOLATE_TARGET",
        EmergencyControlAction.EnterPlatformSafe => "ENTER_PLATFORM_SAFE",
        EmergencyControlAction.EmergencyStop => "EMERGENCY_STOP",
        _ => "DENY_UNKNOWN_EMERGENCY_CONTROL_ACTION"
    };

    private static bool IsValidRequest(IndependentEmergencyControlRequest? request, DateTimeOffset observationTime)
    {
        if (request is null || observationTime == default)
            return false;
        if (!CanonicalToken(request.RequestId) ||
            !CanonicalToken(request.ActorIdentity) ||
            !CanonicalToken(request.TargetSubjectId) ||
            !Enum.IsDefined(request.Action) ||
            !Enum.IsDefined(request.RequestedScopeKind) ||
            !CanonicalToken(request.RequestedScopeId) ||
            !CanonicalToken(request.ProtectiveMandateReference) ||
            !CanonicalToken(request.Correlation) ||
            request.RequestTime == default ||
            request.Expiry <= request.RequestTime ||
            observationTime < request.RequestTime ||
            observationTime >= request.Expiry)
        {
            return false;
        }

        return request.RequestedScopeKind != EmergencyControlScopeKind.FalconWide ||
            string.Equals(request.RequestedScopeId, "falcon:platform", StringComparison.Ordinal);
    }

    private static bool AuthorityBindsExactly(
        IndependentEmergencyControlRequest request,
        AuthorityRequest authorityRequest,
        AuthorityResult authorityResult)
    {
        return
            string.Equals(authorityRequest.RequestId, authorityResult.RequestId, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.ActorIdentity, request.ActorIdentity, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Action, ToAuthorityAction(request.Action), StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Resource, request.TargetSubjectId, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Purpose, "PROTECTIVE_EMERGENCY_CONTROL", StringComparison.Ordinal) &&
            string.Equals(authorityRequest.RequestedScope, request.RequestedScopeId, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Correlation, request.Correlation, StringComparison.Ordinal) &&
            string.Equals(authorityResult.EffectiveScope, request.RequestedScopeId, StringComparison.Ordinal);
    }

    private static bool IsValidBlastRadiusEvidence(
        IndependentEmergencyControlRequest request,
        EmergencyBlastRadiusEvidence? evidence,
        DateTimeOffset observationTime)
    {
        if (evidence is null ||
            !CanonicalToken(evidence.EvidenceId) ||
            !CanonicalToken(evidence.TargetSubjectId) ||
            !CanonicalToken(evidence.RequestedScopeId) ||
            !Enum.IsDefined(evidence.LocalBoundaryTrust) ||
            !Enum.IsDefined(evidence.PropagationState) ||
            !Enum.IsDefined(evidence.UnaffectedScopeTrust) ||
            !Enum.IsDefined(evidence.EvidenceSourceTrust) ||
            evidence.ObservedAt == default ||
            evidence.Expiry <= evidence.ObservedAt ||
            observationTime < evidence.ObservedAt ||
            observationTime >= evidence.Expiry)
        {
            return false;
        }

        return string.Equals(evidence.TargetSubjectId, request.TargetSubjectId, StringComparison.Ordinal) &&
            string.Equals(evidence.RequestedScopeId, request.RequestedScopeId, StringComparison.Ordinal);
    }

    private static bool CanPreserveRequestedScope(EmergencyBlastRadiusEvidence evidence)
    {
        if (evidence.LocalBoundaryTrust != EmergencyTrustState.Trustworthy ||
            evidence.PropagationState != EmergencyPropagationState.Excluded ||
            evidence.UnaffectedScopeTrust != EmergencyTrustState.Trustworthy ||
            evidence.EvidenceSourceTrust != EmergencyTrustState.Trustworthy)
        {
            return false;
        }

        if (evidence.GuardianCompromiseSuspected && evidence.GuardianEvidenceSoleSource)
            return false;

        return true;
    }

    private static IndependentEmergencyControlDecision Reject(
        IndependentEmergencyControlRequest? request,
        string reason,
        AuthorityResult? authorityResult,
        EmergencyBlastRadiusEvidence? evidence,
        DateTimeOffset observationTime)
    {
        var requestId = Clean(request?.RequestId, "missing-request");
        var requestIdentity = request is null ? "missing-request-identity" : request.Identity;
        var target = Clean(request?.TargetSubjectId, "missing-target");
        var requestedScopeId = Clean(request?.RequestedScopeId, "missing-scope");
        var mandate = Clean(request?.ProtectiveMandateReference, "missing-mandate");
        var authorityDecision = Clean(authorityResult?.DecisionId, "missing-authority-decision");
        var authorityEvidence = Clean(authorityResult?.EvidenceReference, "missing-authority-evidence");
        var blastIdentity = evidence is null ? "missing-blast-radius-evidence" : evidence.Identity;
        var decisionTime = observationTime == default ? DateTimeOffset.UnixEpoch : observationTime;

        return new IndependentEmergencyControlDecision(
            false,
            reason,
            requestId,
            requestIdentity,
            target,
            request?.Action ?? EmergencyControlAction.EmergencyStop,
            request?.RequestedScopeKind ?? EmergencyControlScopeKind.FalconWide,
            requestedScopeId,
            EmergencyControlScopeKind.FalconWide,
            "falcon:platform",
            true,
            false,
            true,
            mandate,
            authorityDecision,
            authorityEvidence,
            blastIdentity,
            decisionTime,
            decisionTime.AddTicks(1));
    }

    private static DateTimeOffset Minimum(params DateTimeOffset[] values)
    {
        var result = values[0];
        for (var i = 1; i < values.Length; i++)
        {
            if (values[i] < result)
                result = values[i];
        }
        return result;
    }

    private static string Clean(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static bool CanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;
        foreach (var ch in value)
        {
            if (char.IsControl(ch) || char.IsWhiteSpace(ch))
                return false;
        }
        return true;
    }
}

internal static class IndependentEmergencyControlIdentity
{
    internal static string ComputeRequest(IndependentEmergencyControlRequest value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.RequestId,
            value.ActorIdentity,
            value.TargetSubjectId,
            ((int)value.Action).ToString(CultureInfo.InvariantCulture),
            ((int)value.RequestedScopeKind).ToString(CultureInfo.InvariantCulture),
            value.RequestedScopeId,
            value.ProtectiveMandateReference,
            value.Correlation,
            Time(value.RequestTime),
            Time(value.Expiry)
        });
        return Digest("independent-emergency-control-request", canonical);
    }

    internal static string ComputeEvidence(EmergencyBlastRadiusEvidence value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.EvidenceId,
            value.TargetSubjectId,
            value.RequestedScopeId,
            ((int)value.LocalBoundaryTrust).ToString(CultureInfo.InvariantCulture),
            ((int)value.PropagationState).ToString(CultureInfo.InvariantCulture),
            ((int)value.UnaffectedScopeTrust).ToString(CultureInfo.InvariantCulture),
            ((int)value.EvidenceSourceTrust).ToString(CultureInfo.InvariantCulture),
            value.GuardianCompromiseSuspected ? "1" : "0",
            value.GuardianEvidenceSoleSource ? "1" : "0",
            Time(value.ObservedAt),
            Time(value.Expiry)
        });
        return Digest("emergency-blast-radius-evidence", canonical);
    }

    internal static string ComputeDecision(IndependentEmergencyControlDecision value)
    {
        var canonical = string.Join("\n", new[]
        {
            value.Accepted ? "1" : "0",
            value.Reason,
            value.RequestId,
            value.RequestIdentity,
            value.TargetSubjectId,
            ((int)value.Action).ToString(CultureInfo.InvariantCulture),
            ((int)value.RequestedScopeKind).ToString(CultureInfo.InvariantCulture),
            value.RequestedScopeId,
            ((int)value.EffectiveScopeKind).ToString(CultureInfo.InvariantCulture),
            value.EffectiveScopeId,
            value.ContainmentRequired ? "1" : "0",
            value.UnaffectedOperationEligible ? "1" : "0",
            value.UnaffectedOperationStillRequiresAuthority ? "1" : "0",
            value.ProtectiveMandateReference,
            value.AuthorityDecisionId,
            value.AuthorityEvidenceReference,
            value.BlastRadiusEvidenceIdentity,
            Time(value.DecisionTime),
            Time(value.ReviewDeadline)
        });
        return Digest("independent-emergency-control-decision", canonical);
    }

    private static string Digest(string prefix, string canonical) =>
        prefix + "/sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));

    private static string Time(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
}