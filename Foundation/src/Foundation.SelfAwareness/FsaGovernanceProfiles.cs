using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.SelfAwareness;

public enum FsaInvestigationState
{
    Normal = 1,
    MinimumIntegrityCheck = 2,
    InvestigationHold = 3,
    Isolated = 4,
    Killed = 5,
    Remediation = 6,
    ReadyForGovernedReentry = 7,
    Probationary = 8
}

public sealed record FsaMonitorRegistration(
    string MonitorId,
    string TargetFsaId,
    string PerspectiveId,
    string PolicyIdentity,
    string LifecycleIdentity,
    bool Replaceable,
    bool KillAuthorityAllowed,
    bool SelfDevelopmentAllowed,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry,
    string EvidenceReference);

public sealed record FsaMonitorPairDecision(
    bool Accepted,
    bool IndependentCoverageEstablished,
    bool RecursiveMonitorHierarchyRequired,
    string Reason);

public static class FsaMonitorRegistryRuntime
{
    public static FsaMonitorPairDecision Evaluate(IReadOnlyCollection<FsaMonitorRegistration>? registrations, string? targetFsaId, DateTimeOffset now)
    {
        if (registrations is null || string.IsNullOrWhiteSpace(targetFsaId) || now == default)
            return Deny("INVALID_MONITOR_REGISTRY");

        var active = registrations.Where(x => x is not null && x.EffectiveTime <= now && now < x.Expiry && string.Equals(x.TargetFsaId, targetFsaId, StringComparison.Ordinal)).ToArray();
        if (active.Length != 2) return Deny("EXACTLY_TWO_ACTIVE_FSA_MONITORS_REQUIRED");
        if (active.Any(x => !Valid(x))) return Deny("INVALID_MONITOR_REGISTRATION");
        if (active.Any(x => x.KillAuthorityAllowed || x.SelfDevelopmentAllowed)) return Deny("MONITOR_AUTHORITY_OR_SELF_DEVELOPMENT_PROHIBITED");
        if (active.Select(x => x.MonitorId).Distinct(StringComparer.Ordinal).Count() != 2) return Deny("MONITOR_IDENTITY_NOT_INDEPENDENT");
        if (active.Select(x => x.PerspectiveId).Distinct(StringComparer.Ordinal).Count() != 2) return Deny("MONITOR_PERSPECTIVE_NOT_INDEPENDENT");
        if (active.Select(x => x.PolicyIdentity).Distinct(StringComparer.Ordinal).Count() != 2) return Deny("MONITOR_POLICY_NOT_INDEPENDENT");
        if (active.Select(x => x.LifecycleIdentity).Distinct(StringComparer.Ordinal).Count() != 2) return Deny("MONITOR_LIFECYCLE_NOT_INDEPENDENT");
        if (active.Any(x => !x.Replaceable)) return Deny("MONITOR_REPLACEABILITY_REQUIRED");

        return new(true, true, false, "INDEPENDENT_TWO_MONITOR_COVERAGE_ESTABLISHED");
    }

    private static bool Valid(FsaMonitorRegistration registration) =>
        Token(registration.MonitorId) && Token(registration.TargetFsaId) && Token(registration.PerspectiveId) &&
        Token(registration.PolicyIdentity) && Token(registration.LifecycleIdentity) && Token(registration.EvidenceReference) &&
        registration.EffectiveTime != default && registration.Expiry > registration.EffectiveTime;

    private static FsaMonitorPairDecision Deny(string reason) => new(false, false, false, reason);
    private static bool Token(string? value) => !string.IsNullOrWhiteSpace(value);
}

public static class FsaAuthorityCeiling
{
    public static IReadOnlyList<string> AllowedFoundationReviewPurposes { get; } = Array.AsReadOnly(new[]
    {
        "FOUNDATION_CONDITION_REVIEW",
        "FOUNDATION_ARCHITECTURE_COMPATIBILITY_REVIEW",
        "FOUNDATION_SECURITY_GOVERNANCE_REVIEW",
        "FOUNDATION_AUTHORITY_COMPATIBILITY_REVIEW",
        "FOUNDATION_IMPROVEMENT_PROPOSAL_REVIEW"
    });

    public static IReadOnlyList<string> ProhibitedBusinessDomains { get; } = Array.AsReadOnly(new[]
    {
        "TRADING",
        "STRATEGY",
        "MARKET",
        "PORTFOLIO",
        "BROKER",
        "APPLICATION_RISK_BUSINESS_MEANING",
        "PROVIDER_BUSINESS_MEANING",
        "EXECUTION_BUSINESS_MEANING"
    });

    public static bool IsFoundationReviewPurposeAllowed(string? purpose) =>
        !string.IsNullOrWhiteSpace(purpose) && AllowedFoundationReviewPurposes.Contains(purpose, StringComparer.Ordinal);

    public static bool IsBusinessDomainAllowed(string? domain) =>
        !string.IsNullOrWhiteSpace(domain) && !ProhibitedBusinessDomains.Contains(domain, StringComparer.Ordinal);
}

public sealed record FsaInvestigationTransitionRequest(
    string ActorIdentity,
    string TargetFsaId,
    FsaInvestigationState CurrentState,
    FsaInvestigationState RequestedState,
    bool AuthorityEvidenceValid,
    bool EvidencePreserved,
    bool IndependentDecision,
    DateTimeOffset DecisionTime);

public sealed record FsaInvestigationTransitionDecision(bool Accepted, FsaInvestigationState EffectiveState, string Reason);

public static class FsaInvestigationRuntime
{
    public static FsaInvestigationTransitionDecision Evaluate(FsaInvestigationTransitionRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.ActorIdentity) || string.IsNullOrWhiteSpace(request.TargetFsaId) ||
            !Enum.IsDefined(request.CurrentState) || !Enum.IsDefined(request.RequestedState) || request.DecisionTime == default)
            return Deny(FsaInvestigationState.InvestigationHold, "INVALID_INVESTIGATION_TRANSITION");

        if (string.Equals(request.ActorIdentity, request.TargetFsaId, StringComparison.Ordinal))
            return Deny(request.CurrentState, "FSA_SELF_TRANSITION_DENIED");
        if (!request.AuthorityEvidenceValid || !request.IndependentDecision)
            return Deny(request.CurrentState, "INDEPENDENT_AUTHORITY_DECISION_REQUIRED");

        var requiresEvidence = request.RequestedState is FsaInvestigationState.InvestigationHold or FsaInvestigationState.Isolated or
            FsaInvestigationState.Killed or FsaInvestigationState.Remediation or FsaInvestigationState.ReadyForGovernedReentry or FsaInvestigationState.Probationary;
        if (requiresEvidence && !request.EvidencePreserved)
            return Deny(request.CurrentState, "EVIDENCE_PRESERVATION_REQUIRED");

        if (!Allowed(request.CurrentState, request.RequestedState))
            return Deny(request.CurrentState, "INVALID_INVESTIGATION_STATE_TRANSITION");

        return new(true, request.RequestedState, "INVESTIGATION_STATE_TRANSITION_ACCEPTED");
    }

    private static bool Allowed(FsaInvestigationState current, FsaInvestigationState next) => (current, next) switch
    {
        (FsaInvestigationState.Normal, FsaInvestigationState.MinimumIntegrityCheck) => true,
        (FsaInvestigationState.MinimumIntegrityCheck, FsaInvestigationState.Normal) => true,
        (FsaInvestigationState.MinimumIntegrityCheck, FsaInvestigationState.InvestigationHold) => true,
        (FsaInvestigationState.InvestigationHold, FsaInvestigationState.Isolated) => true,
        (FsaInvestigationState.InvestigationHold, FsaInvestigationState.Killed) => true,
        (FsaInvestigationState.Isolated, FsaInvestigationState.Killed) => true,
        (FsaInvestigationState.Isolated, FsaInvestigationState.Remediation) => true,
        (FsaInvestigationState.Killed, FsaInvestigationState.Remediation) => true,
        (FsaInvestigationState.Remediation, FsaInvestigationState.ReadyForGovernedReentry) => true,
        (FsaInvestigationState.ReadyForGovernedReentry, FsaInvestigationState.Probationary) => true,
        (FsaInvestigationState.Probationary, FsaInvestigationState.Normal) => true,
        _ => false
    };

    private static FsaInvestigationTransitionDecision Deny(FsaInvestigationState state, string reason) => new(false, state, reason);
}

public static class FsaDeferredGovernanceCandidates
{
    public const bool TwentyFourHourNoResponseProductionApprovalAuthorized = false;
    public const bool OwnerSilenceMayActivateFallback = false;
    public const bool TimerExpiryMayActivateFallback = false;
}
