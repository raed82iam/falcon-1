using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public enum AiEmergencyAction
{
    Restrict = 1,
    Suspend = 2,
    Isolate = 3,
    Kill = 4,
    GlobalAiKill = 5
}

public enum AiKillIngress
{
    WebOwner = 1,
    ExternalOwner = 2
}

public enum AiTargetKind
{
    Component = 1,
    Csa = 2,
    Lsa = 3,
    Msa = 4,
    Fsa = 5,
    DefinedGroup = 6,
    AllAi = 7
}

public sealed record AiTargetRegistration(
    string RegistrationId,
    string TargetId,
    AiTargetKind TargetKind,
    string? ParentTargetId,
    string OwningScopeId,
    bool ExecutableAi,
    string EvidenceReference,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string Identity => AiKillControlIdentity.ComputeRegistration(this);
}

public sealed record AiKillRequest(
    string RequestId,
    string ActorIdentity,
    AiKillIngress Ingress,
    AiEmergencyAction Action,
    AiTargetKind TargetKind,
    string TargetId,
    string Correlation,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry)
{
    public string Identity => AiKillControlIdentity.ComputeRequest(this);
}

public sealed class AiKillControlDecision
{
    internal AiKillControlDecision(
        bool accepted,
        string reason,
        string requestId,
        string requestIdentity,
        string actorIdentity,
        AiKillIngress ingress,
        AiEmergencyAction action,
        AiTargetKind targetKind,
        string targetId,
        IReadOnlyList<string> impactedTargetIds,
        bool authorityRevocationRequired,
        bool suspensionRequired,
        bool stopRequired,
        bool isolationRequired,
        bool evidenceFreezeRequired,
        bool safeCorePreserved,
        bool falconShutdownAuthorized,
        bool targetCooperationRequired,
        bool releaseRequiresGovernedRecovery,
        string authorityDecisionId,
        string authorityEvidenceReference,
        DateTimeOffset decisionTime,
        DateTimeOffset reviewDeadline)
    {
        Accepted = accepted;
        Reason = reason;
        RequestId = requestId;
        RequestIdentity = requestIdentity;
        ActorIdentity = actorIdentity;
        Ingress = ingress;
        Action = action;
        TargetKind = targetKind;
        TargetId = targetId;
        ImpactedTargetIds = impactedTargetIds;
        AuthorityRevocationRequired = authorityRevocationRequired;
        SuspensionRequired = suspensionRequired;
        StopRequired = stopRequired;
        IsolationRequired = isolationRequired;
        EvidenceFreezeRequired = evidenceFreezeRequired;
        SafeCorePreserved = safeCorePreserved;
        FalconShutdownAuthorized = falconShutdownAuthorized;
        TargetCooperationRequired = targetCooperationRequired;
        ReleaseRequiresGovernedRecovery = releaseRequiresGovernedRecovery;
        AuthorityDecisionId = authorityDecisionId;
        AuthorityEvidenceReference = authorityEvidenceReference;
        DecisionTime = decisionTime;
        ReviewDeadline = reviewDeadline;
    }

    public bool Accepted { get; }
    public string Reason { get; }
    public string RequestId { get; }
    public string RequestIdentity { get; }
    public string ActorIdentity { get; }
    public AiKillIngress Ingress { get; }
    public AiEmergencyAction Action { get; }
    public AiTargetKind TargetKind { get; }
    public string TargetId { get; }
    public IReadOnlyList<string> ImpactedTargetIds { get; }
    public bool AuthorityRevocationRequired { get; }
    public bool SuspensionRequired { get; }
    public bool StopRequired { get; }
    public bool IsolationRequired { get; }
    public bool EvidenceFreezeRequired { get; }
    public bool SafeCorePreserved { get; }
    public bool FalconShutdownAuthorized { get; }
    public bool TargetCooperationRequired { get; }
    public bool ReleaseRequiresGovernedRecovery { get; }
    public string AuthorityDecisionId { get; }
    public string AuthorityEvidenceReference { get; }
    public DateTimeOffset DecisionTime { get; }
    public DateTimeOffset ReviewDeadline { get; }
    public string Identity => AiKillControlIdentity.ComputeDecision(this);
}

public static class AiKillControlPlaneContract
{
    public const string ControlPlaneId = "foundation:ai-kill-control-plane";
    public const string AllAiTargetId = "falcon:all-ai";
    public const string Purpose = "AI_EMERGENCY_CONTROL";

    public static IReadOnlyList<string> SafeCoreCapabilities { get; } = Array.AsReadOnly(new[]
    {
        "OWNER_CONTROL",
        "AI_KILL_CONTROL",
        "LIFECYCLE_ENFORCEMENT",
        "AUTHORITY_REVOCATION",
        "SECURITY",
        "AUDIT_EVIDENCE",
        "FORENSICS",
        "RECOVERY_INFRASTRUCTURE",
        "EMERGENCY_COMMUNICATIONS"
    });
}

public static class AiKillControlReason
{
    public const string AcceptedTargeted = "AI_KILL_CONTROL_ACCEPTED_TARGETED";
    public const string AcceptedGlobal = "AI_KILL_CONTROL_ACCEPTED_GLOBAL";
    public const string InvalidRegistry = "AI_KILL_CONTROL_INVALID_REGISTRY";
    public const string InvalidRequest = "AI_KILL_CONTROL_INVALID_REQUEST";
    public const string AiActorProhibited = "AI_KILL_CONTROL_AI_ACTOR_PROHIBITED";
    public const string InvalidAuthorityRequest = "AI_KILL_CONTROL_INVALID_AUTHORITY_REQUEST";
    public const string AuthorityNotGranted = "AI_KILL_CONTROL_AUTHORITY_NOT_GRANTED";
    public const string AuthorityBindingMismatch = "AI_KILL_CONTROL_AUTHORITY_BINDING_MISMATCH";
    public const string TargetNotFound = "AI_KILL_CONTROL_TARGET_NOT_FOUND";
    public const string AmbiguousTarget = "AI_KILL_CONTROL_AMBIGUOUS_TARGET";
    public const string InvalidGlobalRequest = "AI_KILL_CONTROL_INVALID_GLOBAL_REQUEST";
    public const string NoImpactedAi = "AI_KILL_CONTROL_NO_IMPACTED_AI";
}

public static class AiKillControlRuntime
{
    public static AiKillControlDecision Evaluate(
        AiKillRequest? request,
        IReadOnlyCollection<AiTargetRegistration>? registry,
        AuthorityRequest? authorityRequest,
        AuthorityEvaluationContext? authorityContext,
        DateTimeOffset observationTime)
    {
        var registryState = ValidateRegistry(registry, observationTime);
        if (!registryState.Valid)
            return Reject(request, AiKillControlReason.InvalidRegistry, null, observationTime);

        if (!IsValidRequest(request, observationTime))
            return Reject(request, AiKillControlReason.InvalidRequest, null, observationTime);

        var active = registryState.Active;
        if (active.Any(item => string.Equals(item.TargetId, request!.ActorIdentity, StringComparison.Ordinal)))
            return Reject(request, AiKillControlReason.AiActorProhibited, null, observationTime);

        if (authorityRequest is null || ContractValidators.Validate(authorityRequest).Result != ValidationResult.Pass)
            return Reject(request, AiKillControlReason.InvalidAuthorityRequest, null, observationTime);

        if (authorityContext is null || authorityContext.ObservationTime != observationTime)
            return Reject(request, AiKillControlReason.AuthorityNotGranted, null, observationTime);

        var authorityResult = new DefaultDenyAuthorityEngine().Evaluate(authorityRequest, authorityContext);
        if (ContractValidators.Validate(authorityResult).Result != ValidationResult.Pass ||
            !string.Equals(authorityResult.Decision, AuthorityDecision.Allow, StringComparison.Ordinal) ||
            observationTime < authorityResult.DecisionTime || observationTime >= authorityResult.Expiry)
        {
            return Reject(request, AiKillControlReason.AuthorityNotGranted, authorityResult, observationTime);
        }

        if (!AuthorityBindsExactly(request!, authorityRequest, authorityResult))
            return Reject(request, AiKillControlReason.AuthorityBindingMismatch, authorityResult, observationTime);

        var resolution = ResolveTargets(request!, active);
        if (!resolution.Accepted)
            return Reject(request, resolution.Reason, authorityResult, observationTime);

        var impacted = resolution.ImpactedTargetIds;
        var global = request!.Action == AiEmergencyAction.GlobalAiKill;
        var suspension = request.Action is AiEmergencyAction.Suspend or AiEmergencyAction.Kill or AiEmergencyAction.GlobalAiKill;
        var stop = request.Action is AiEmergencyAction.Kill or AiEmergencyAction.GlobalAiKill;
        var isolation = request.Action is AiEmergencyAction.Isolate or AiEmergencyAction.Kill or AiEmergencyAction.GlobalAiKill;
        var deadline = request.Expiry < authorityResult.Expiry ? request.Expiry : authorityResult.Expiry;

        return new AiKillControlDecision(
            true,
            global ? AiKillControlReason.AcceptedGlobal : AiKillControlReason.AcceptedTargeted,
            request.RequestId,
            request.Identity,
            request.ActorIdentity,
            request.Ingress,
            request.Action,
            request.TargetKind,
            request.TargetId,
            impacted,
            true,
            suspension,
            stop,
            isolation,
            true,
            true,
            false,
            false,
            true,
            authorityResult.DecisionId,
            authorityResult.EvidenceReference,
            observationTime,
            deadline);
    }

    public static bool ValidateDecision(AiKillControlDecision? decision)
    {
        if (decision is null || !decision.Accepted ||
            !CanonicalToken(decision.RequestId) || !CanonicalToken(decision.RequestIdentity) ||
            !CanonicalToken(decision.ActorIdentity) || !Enum.IsDefined(decision.Ingress) ||
            !Enum.IsDefined(decision.Action) || !Enum.IsDefined(decision.TargetKind) ||
            !CanonicalToken(decision.TargetId) || decision.ImpactedTargetIds.Count == 0 ||
            decision.ImpactedTargetIds.Any(id => !CanonicalToken(id)) ||
            decision.ImpactedTargetIds.Distinct(StringComparer.Ordinal).Count() != decision.ImpactedTargetIds.Count ||
            !decision.AuthorityRevocationRequired || !decision.EvidenceFreezeRequired ||
            !decision.SafeCorePreserved || decision.FalconShutdownAuthorized || decision.TargetCooperationRequired ||
            !decision.ReleaseRequiresGovernedRecovery || !CanonicalToken(decision.AuthorityDecisionId) ||
            !CanonicalToken(decision.AuthorityEvidenceReference) || decision.DecisionTime == default ||
            decision.ReviewDeadline <= decision.DecisionTime)
        {
            return false;
        }

        if (decision.Action == AiEmergencyAction.GlobalAiKill &&
            (decision.TargetKind != AiTargetKind.AllAi ||
             !string.Equals(decision.TargetId, AiKillControlPlaneContract.AllAiTargetId, StringComparison.Ordinal)))
            return false;

        return string.Equals(decision.Identity, AiKillControlIdentity.ComputeDecision(decision), StringComparison.Ordinal);
    }

    public static string ToAuthorityAction(AiEmergencyAction action) => action switch
    {
        AiEmergencyAction.Restrict => "RESTRICT_AI",
        AiEmergencyAction.Suspend => "SUSPEND_AI",
        AiEmergencyAction.Isolate => "ISOLATE_AI",
        AiEmergencyAction.Kill => "KILL_AI",
        AiEmergencyAction.GlobalAiKill => "GLOBAL_AI_KILL",
        _ => "DENY_UNKNOWN_AI_EMERGENCY_ACTION"
    };

    private static RegistryState ValidateRegistry(IReadOnlyCollection<AiTargetRegistration>? registry, DateTimeOffset now)
    {
        if (registry is null || now == default)
            return RegistryState.Invalid;

        var active = registry.Where(item => item is not null && item.EffectiveTime <= now && now < item.Expiry).ToArray();
        foreach (var item in active)
        {
            if (!CanonicalToken(item.RegistrationId) || !CanonicalToken(item.TargetId) ||
                !Enum.IsDefined(item.TargetKind) || item.TargetKind == AiTargetKind.AllAi ||
                !CanonicalOptional(item.ParentTargetId) || !CanonicalToken(item.OwningScopeId) ||
                !CanonicalToken(item.EvidenceReference) || item.EffectiveTime == default || item.Expiry <= item.EffectiveTime ||
                string.Equals(item.TargetId, AiKillControlPlaneContract.ControlPlaneId, StringComparison.Ordinal) ||
                string.Equals(item.TargetId, AiKillControlPlaneContract.AllAiTargetId, StringComparison.Ordinal))
                return RegistryState.Invalid;
        }

        if (active.GroupBy(item => item.TargetId, StringComparer.Ordinal).Any(group => group.Count() != 1))
            return RegistryState.Invalid;

        var byId = active.ToDictionary(item => item.TargetId, StringComparer.Ordinal);
        foreach (var item in active)
        {
            if (item.ParentTargetId is not null && !byId.ContainsKey(item.ParentTargetId))
                return RegistryState.Invalid;
            var seen = new HashSet<string>(StringComparer.Ordinal) { item.TargetId };
            var cursor = item;
            while (cursor.ParentTargetId is not null)
            {
                if (!seen.Add(cursor.ParentTargetId))
                    return RegistryState.Invalid;
                cursor = byId[cursor.ParentTargetId];
            }
        }

        return new RegistryState(true, active);
    }

    private static bool IsValidRequest(AiKillRequest? request, DateTimeOffset now)
    {
        if (request is null || now == default || !CanonicalToken(request.RequestId) ||
            !CanonicalToken(request.ActorIdentity) || !Enum.IsDefined(request.Ingress) || !Enum.IsDefined(request.Action) ||
            !Enum.IsDefined(request.TargetKind) || !CanonicalToken(request.TargetId) || !CanonicalToken(request.Correlation) ||
            request.RequestTime == default || request.Expiry <= request.RequestTime || now < request.RequestTime || now >= request.Expiry)
            return false;

        if (string.Equals(request.ActorIdentity, AiKillControlPlaneContract.ControlPlaneId, StringComparison.Ordinal))
            return false;

        return true;
    }

    private static bool AuthorityBindsExactly(AiKillRequest request, AuthorityRequest authorityRequest, AuthorityResult authorityResult)
    {
        return string.Equals(authorityRequest.RequestId, authorityResult.RequestId, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.ActorIdentity, request.ActorIdentity, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Action, ToAuthorityAction(request.Action), StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Resource, request.TargetId, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Purpose, AiKillControlPlaneContract.Purpose, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.RequestedScope, request.TargetId, StringComparison.Ordinal) &&
            string.Equals(authorityRequest.Correlation, request.Correlation, StringComparison.Ordinal) &&
            string.Equals(authorityResult.EffectiveScope, request.TargetId, StringComparison.Ordinal);
    }

    private static TargetResolution ResolveTargets(AiKillRequest request, IReadOnlyList<AiTargetRegistration> active)
    {
        if (request.Action == AiEmergencyAction.GlobalAiKill)
        {
            if (request.TargetKind != AiTargetKind.AllAi ||
                !string.Equals(request.TargetId, AiKillControlPlaneContract.AllAiTargetId, StringComparison.Ordinal))
                return TargetResolution.Reject(AiKillControlReason.InvalidGlobalRequest);

            var all = active.Where(item => item.ExecutableAi).Select(item => item.TargetId)
                .OrderBy(id => id, StringComparer.Ordinal).ToArray();
            return all.Length == 0 ? TargetResolution.Reject(AiKillControlReason.NoImpactedAi) : TargetResolution.Accept(all);
        }

        if (request.TargetKind == AiTargetKind.AllAi ||
            string.Equals(request.TargetId, AiKillControlPlaneContract.AllAiTargetId, StringComparison.Ordinal))
            return TargetResolution.Reject(AiKillControlReason.InvalidGlobalRequest);

        var matches = active.Where(item => string.Equals(item.TargetId, request.TargetId, StringComparison.Ordinal) && item.TargetKind == request.TargetKind).ToArray();
        if (matches.Length == 0)
            return TargetResolution.Reject(AiKillControlReason.TargetNotFound);
        if (matches.Length != 1)
            return TargetResolution.Reject(AiKillControlReason.AmbiguousTarget);

        var impacted = new HashSet<string>(StringComparer.Ordinal);
        var frontier = new Queue<string>();
        frontier.Enqueue(matches[0].TargetId);
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            var currentRegistration = active.Single(item => string.Equals(item.TargetId, current, StringComparison.Ordinal));
            if (currentRegistration.ExecutableAi)
                impacted.Add(current);
            foreach (var child in active.Where(item => string.Equals(item.ParentTargetId, current, StringComparison.Ordinal)).OrderBy(item => item.TargetId, StringComparer.Ordinal))
                frontier.Enqueue(child.TargetId);
        }

        var result = impacted.OrderBy(id => id, StringComparer.Ordinal).ToArray();
        return result.Length == 0 ? TargetResolution.Reject(AiKillControlReason.NoImpactedAi) : TargetResolution.Accept(result);
    }

    private static AiKillControlDecision Reject(AiKillRequest? request, string reason, AuthorityResult? authority, DateTimeOffset now)
    {
        var time = now == default ? DateTimeOffset.UnixEpoch : now;
        return new AiKillControlDecision(
            false, reason, Clean(request?.RequestId, "missing-request"), request is null ? "missing-request-identity" : request.Identity,
            Clean(request?.ActorIdentity, "missing-actor"), request?.Ingress ?? AiKillIngress.ExternalOwner,
            request?.Action ?? AiEmergencyAction.Kill, request?.TargetKind ?? AiTargetKind.Component,
            Clean(request?.TargetId, "missing-target"), Array.Empty<string>(), true, true, true, true, true, true, false, false, true,
            Clean(authority?.DecisionId, "missing-authority-decision"), Clean(authority?.EvidenceReference, "missing-authority-evidence"), time, time.AddTicks(1));
    }

    private static string Clean(string? value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    private static bool CanonicalOptional(string? value) => value is null || CanonicalToken(value);
    private static bool CanonicalToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal)) return false;
        return value.All(ch => !char.IsControl(ch) && !char.IsWhiteSpace(ch));
    }

    private sealed record RegistryState(bool Valid, IReadOnlyList<AiTargetRegistration> Active)
    {
        public static RegistryState Invalid { get; } = new(false, Array.Empty<AiTargetRegistration>());
    }

    private sealed record TargetResolution(bool Accepted, string Reason, IReadOnlyList<string> ImpactedTargetIds)
    {
        public static TargetResolution Accept(IReadOnlyList<string> ids) => new(true, string.Empty, ids);
        public static TargetResolution Reject(string reason) => new(false, reason, Array.Empty<string>());
    }
}

public static class AiKillAuthorityReason
{
    public const string EvidenceUnavailable = "AUTHORITY_AI_KILL_EVIDENCE_UNAVAILABLE";
    public const string DecisionMalformed = "AUTHORITY_AI_KILL_DECISION_MALFORMED";
    public const string TargetContained = "AUTHORITY_AI_TARGET_CONTAINED";
}

public sealed class AiKillControlAuthorityEnforcer
{
    public AuthorityResult Evaluate(
        AuthorityRequest? request,
        AuthorityEvaluationContext? context,
        IReadOnlyCollection<AiKillControlDecision>? controls)
    {
        var baseline = new DefaultDenyAuthorityEngine().Evaluate(request, context);
        if (controls is null)
            return Deny(baseline, AiKillAuthorityReason.EvidenceUnavailable, "missing-ai-kill-evidence");

        foreach (var control in controls)
        {
            if (!AiKillControlRuntime.ValidateDecision(control))
                return Deny(baseline, AiKillAuthorityReason.DecisionMalformed, control?.RequestIdentity ?? "malformed-ai-kill-decision");
        }

        if (!string.Equals(baseline.Decision, AuthorityDecision.Allow, StringComparison.Ordinal) || request is null)
            return baseline;

        var blocking = controls.Where(control => control.ImpactedTargetIds.Contains(request.ActorIdentity, StringComparer.Ordinal))
            .OrderBy(control => control.RequestId, StringComparer.Ordinal).FirstOrDefault();
        return blocking is null ? baseline : Deny(baseline, AiKillAuthorityReason.TargetContained, blocking.Identity);
    }

    private static AuthorityResult Deny(AuthorityResult baseline, string reason, string evidence)
    {
        var decisionId = "authority-ai-kill-decision/sha256/" + Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n", baseline.DecisionId, reason, evidence))));
        return baseline with
        {
            DecisionId = decisionId,
            Decision = AuthorityDecision.Deny,
            EffectiveScope = "NONE",
            Constraints = reason,
            Reason = reason,
            EvidenceReference = evidence
        };
    }
}

internal static class AiKillControlIdentity
{
    internal static string ComputeRegistration(AiTargetRegistration value) => Hash(string.Join("\n",
        value.RegistrationId, value.TargetId, value.TargetKind.ToString(), value.ParentTargetId ?? "NONE", value.OwningScopeId,
        value.ExecutableAi ? "TRUE" : "FALSE", value.EvidenceReference, Format(value.EffectiveTime), Format(value.Expiry)));

    internal static string ComputeRequest(AiKillRequest value) => Hash(string.Join("\n",
        value.RequestId, value.ActorIdentity, value.Ingress.ToString(), value.Action.ToString(), value.TargetKind.ToString(),
        value.TargetId, value.Correlation, Format(value.RequestTime), Format(value.Expiry)));

    internal static string ComputeDecision(AiKillControlDecision value) => Hash(string.Join("\n",
        value.Accepted ? "TRUE" : "FALSE", value.Reason, value.RequestId, value.RequestIdentity, value.ActorIdentity,
        value.Ingress.ToString(), value.Action.ToString(), value.TargetKind.ToString(), value.TargetId,
        string.Join("|", value.ImpactedTargetIds), value.AuthorityRevocationRequired ? "TRUE" : "FALSE",
        value.SuspensionRequired ? "TRUE" : "FALSE", value.StopRequired ? "TRUE" : "FALSE",
        value.IsolationRequired ? "TRUE" : "FALSE", value.EvidenceFreezeRequired ? "TRUE" : "FALSE",
        value.SafeCorePreserved ? "TRUE" : "FALSE", value.FalconShutdownAuthorized ? "TRUE" : "FALSE",
        value.TargetCooperationRequired ? "TRUE" : "FALSE", value.ReleaseRequiresGovernedRecovery ? "TRUE" : "FALSE",
        value.AuthorityDecisionId, value.AuthorityEvidenceReference, Format(value.DecisionTime), Format(value.ReviewDeadline)));

    private static string Hash(string value) => "ai-kill/sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    private static string Format(DateTimeOffset value) => value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
}
