using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public static class AuthorityDecision
{
    public const string Allow = "ALLOW";
    public const string Deny = "DENY";
}

public static class AuthorityReason
{
    public const string Allowed = "AUTHORITY_ALLOWED";
    public const string MalformedRequest = "AUTHORITY_REQUEST_MALFORMED";
    public const string MalformedContext = "AUTHORITY_CONTEXT_MALFORMED";
    public const string PolicyMalformed = "AUTHORITY_POLICY_MALFORMED";
    public const string DelegationMalformed = "AUTHORITY_DELEGATION_MALFORMED";
    public const string FitnessMalformed = "AUTHORITY_FITNESS_MALFORMED";
    public const string EvidenceMissing = "AUTHORITY_EVIDENCE_MISSING";
    public const string ActorUnknown = "AUTHORITY_ACTOR_UNKNOWN";
    public const string ProvenanceMissing = "AUTHORITY_PROVENANCE_MISSING";
    public const string PolicyMissing = "AUTHORITY_POLICY_MISSING";
    public const string PolicyAmbiguous = "AUTHORITY_POLICY_AMBIGUOUS";
    public const string ActionDenied = "AUTHORITY_ACTION_DENIED";
    public const string ResourceDenied = "AUTHORITY_RESOURCE_DENIED";
    public const string PurposeDenied = "AUTHORITY_PURPOSE_DENIED";
    public const string ScopeExceeded = "AUTHORITY_SCOPE_EXCEEDED";
    public const string Expired = "AUTHORITY_EXPIRED";
    public const string DelegationMissing = "AUTHORITY_DELEGATION_MISSING";
    public const string DelegationRevoked = "AUTHORITY_DELEGATION_REVOKED";
    public const string DelegationScopeExceeded = "AUTHORITY_DELEGATION_SCOPE_EXCEEDED";
    public const string FitnessInsufficient = "AUTHORITY_FITNESS_INSUFFICIENT";
    public const string FitnessLevelMismatch = "AUTHORITY_FITNESS_LEVEL_MISMATCH";
    public const string SecurityContextRejected = "AUTHORITY_SECURITY_CONTEXT_REJECTED";
    public const string DefaultDeny = "AUTHORITY_DEFAULT_DENY";
}

public sealed record AuthorityPolicy(
    string PolicyId,
    string PolicyVersion,
    string AuthorityProvenance,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset Expiry,
    IReadOnlyCollection<string> ActorIdentities,
    IReadOnlyCollection<string> Actions,
    IReadOnlyCollection<string> Resources,
    IReadOnlyCollection<string> Purposes,
    IReadOnlyCollection<string> AuthorizedScopes,
    IReadOnlyCollection<string> AcceptedSecurityContexts,
    bool IsAmbiguous = false);

public sealed record DelegationEvidence(
    string DelegationId,
    string ActorIdentity,
    string AuthorityProvenance,
    IReadOnlyCollection<string> AuthorizedScopes,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset Expiry,
    bool IsRevoked);

public sealed record FitnessEvidence(
    string SubjectIdentity,
    string FitnessLevel,
    bool IsSufficient,
    DateTimeOffset ObservedAt,
    DateTimeOffset Expiry,
    string EvidenceReference);

public sealed record AuthorityEvaluationContext(
    AuthorityPolicy? Policy,
    DelegationEvidence? Delegation,
    FitnessEvidence? Fitness,
    DateTimeOffset ObservationTime,
    string EvidenceReference);

public sealed class DefaultDenyAuthorityEngine
{
    public AuthorityResult Evaluate(AuthorityRequest? request, AuthorityEvaluationContext? context)
    {
        var observationTime = context?.ObservationTime ?? DateTimeOffset.UnixEpoch;
        var reason = EvaluateReason(request, context);
        var decision = StringComparer.Ordinal.Equals(reason, AuthorityReason.Allowed)
            ? AuthorityDecision.Allow
            : AuthorityDecision.Deny;

        var requestId = Clean(request?.RequestId, "missing-request");
        var policyId = Clean(context?.Policy?.PolicyId, "missing-policy");
        var policyVersion = Clean(context?.Policy?.PolicyVersion, "missing-version");
        var effectiveScope = decision == AuthorityDecision.Allow
            ? Clean(request?.RequestedScope, "NONE")
            : "NONE";
        var expiry = MinimumExpiry(request, context, observationTime);
        var evidence = Clean(context?.EvidenceReference, "missing-evidence");
        var decisionId = ComputeDecisionIdentity(request, context, decision, reason, effectiveScope, expiry);

        return new AuthorityResult(
            requestId,
            decisionId,
            decision,
            effectiveScope,
            policyId,
            policyVersion,
            BuildMaterialConditions(context, observationTime),
            decision == AuthorityDecision.Allow ? "BOUNDED_TO_EFFECTIVE_SCOPE" : "NO_EXECUTION_AUTHORITY",
            reason,
            observationTime,
            expiry,
            evidence);
    }

    private static string EvaluateReason(AuthorityRequest? request, AuthorityEvaluationContext? context)
    {
        if (request is null || ContractValidators.Validate(request).Result != ValidationResult.Pass)
        {
            return AuthorityReason.MalformedRequest;
        }

        if (context is null || context.ObservationTime == default)
        {
            return AuthorityReason.MalformedContext;
        }

        if (string.IsNullOrWhiteSpace(context.EvidenceReference))
        {
            return AuthorityReason.EvidenceMissing;
        }

        if (string.IsNullOrWhiteSpace(request.ActorIdentity))
        {
            return AuthorityReason.ActorUnknown;
        }

        var policy = context.Policy;
        if (policy is null)
        {
            return AuthorityReason.PolicyMissing;
        }

        if (!IsValidPolicy(policy))
        {
            return AuthorityReason.PolicyMalformed;
        }

        if (string.IsNullOrWhiteSpace(policy.AuthorityProvenance))
        {
            return AuthorityReason.ProvenanceMissing;
        }

        if (policy.IsAmbiguous)
        {
            return AuthorityReason.PolicyAmbiguous;
        }

        if (context.ObservationTime < policy.EffectiveFrom || context.ObservationTime >= policy.Expiry ||
            context.ObservationTime < request.RequestTime || context.ObservationTime >= request.Expiry)
        {
            return AuthorityReason.Expired;
        }

        if (!Contains(policy.ActorIdentities, request.ActorIdentity))
        {
            return AuthorityReason.ActorUnknown;
        }

        if (!Contains(policy.Actions, request.Action))
        {
            return AuthorityReason.ActionDenied;
        }

        if (!Contains(policy.Resources, request.Resource))
        {
            return AuthorityReason.ResourceDenied;
        }

        if (!Contains(policy.Purposes, request.Purpose))
        {
            return AuthorityReason.PurposeDenied;
        }

        if (!AnyScopeContains(policy.AuthorizedScopes, request.RequestedScope))
        {
            return AuthorityReason.ScopeExceeded;
        }

        if (!Contains(policy.AcceptedSecurityContexts, request.SecurityContext))
        {
            return AuthorityReason.SecurityContextRejected;
        }

        var delegation = context.Delegation;
        if (delegation is null)
        {
            return AuthorityReason.DelegationMissing;
        }

        if (!IsValidDelegation(delegation))
        {
            return AuthorityReason.DelegationMalformed;
        }

        if (!StringComparer.Ordinal.Equals(delegation.ActorIdentity.Trim(), request.ActorIdentity.Trim()) ||
            !StringComparer.Ordinal.Equals(delegation.AuthorityProvenance.Trim(), policy.AuthorityProvenance.Trim()))
        {
            return AuthorityReason.DelegationMissing;
        }

        if (delegation.IsRevoked)
        {
            return AuthorityReason.DelegationRevoked;
        }

        if (context.ObservationTime < delegation.EffectiveFrom || context.ObservationTime >= delegation.Expiry)
        {
            return AuthorityReason.Expired;
        }

        if (!AnyScopeContains(delegation.AuthorizedScopes, request.RequestedScope))
        {
            return AuthorityReason.DelegationScopeExceeded;
        }

        var fitness = context.Fitness;
        if (fitness is null)
        {
            return AuthorityReason.FitnessInsufficient;
        }

        if (!IsValidFitness(fitness))
        {
            return AuthorityReason.FitnessMalformed;
        }

        if (!StringComparer.Ordinal.Equals(fitness.SubjectIdentity.Trim(), request.ActorIdentity.Trim()) ||
            !fitness.IsSufficient ||
            context.ObservationTime < fitness.ObservedAt ||
            context.ObservationTime >= fitness.Expiry)
        {
            return AuthorityReason.FitnessInsufficient;
        }

        if (!StringComparer.Ordinal.Equals(
                fitness.FitnessLevel.Trim(),
                request.RequiredFitnessToOperate.Trim()))
        {
            return AuthorityReason.FitnessLevelMismatch;
        }

        return AuthorityReason.Allowed;
    }

    private static bool IsValidPolicy(AuthorityPolicy policy) =>
        !string.IsNullOrWhiteSpace(policy.PolicyId) &&
        !string.IsNullOrWhiteSpace(policy.PolicyVersion) &&
        policy.EffectiveFrom < policy.Expiry &&
        IsValidCollection(policy.ActorIdentities) &&
        IsValidCollection(policy.Actions) &&
        IsValidCollection(policy.Resources) &&
        IsValidCollection(policy.Purposes) &&
        IsValidCollection(policy.AuthorizedScopes) &&
        IsValidCollection(policy.AcceptedSecurityContexts);

    private static bool IsValidDelegation(DelegationEvidence delegation) =>
        !string.IsNullOrWhiteSpace(delegation.DelegationId) &&
        !string.IsNullOrWhiteSpace(delegation.ActorIdentity) &&
        !string.IsNullOrWhiteSpace(delegation.AuthorityProvenance) &&
        delegation.EffectiveFrom < delegation.Expiry &&
        IsValidCollection(delegation.AuthorizedScopes);

    private static bool IsValidFitness(FitnessEvidence fitness) =>
        !string.IsNullOrWhiteSpace(fitness.SubjectIdentity) &&
        !string.IsNullOrWhiteSpace(fitness.FitnessLevel) &&
        !string.IsNullOrWhiteSpace(fitness.EvidenceReference) &&
        fitness.ObservedAt < fitness.Expiry;

    private static bool IsValidCollection(IEnumerable<string>? values)
    {
        if (values is null)
        {
            return false;
        }

        var materialized = values.ToArray();
        return materialized.Length > 0 && materialized.All(value => !string.IsNullOrWhiteSpace(value));
    }

    private static DateTimeOffset MinimumExpiry(
        AuthorityRequest? request,
        AuthorityEvaluationContext? context,
        DateTimeOffset fallback)
    {
        var values = new List<DateTimeOffset>();
        if (request is not null && request.Expiry != default) values.Add(request.Expiry);
        if (context?.Policy is not null && context.Policy.Expiry != default) values.Add(context.Policy.Expiry);
        if (context?.Delegation is not null && context.Delegation.Expiry != default) values.Add(context.Delegation.Expiry);
        if (context?.Fitness is not null && context.Fitness.Expiry != default) values.Add(context.Fitness.Expiry);
        return values.Count == 0 ? fallback : values.Min();
    }

    private static string BuildMaterialConditions(AuthorityEvaluationContext? context, DateTimeOffset observationTime) =>
        string.Join("|",
            $"observation={CanonicalTime(observationTime)}",
            $"policy={Clean(context?.Policy?.PolicyId, "missing")}@{Clean(context?.Policy?.PolicyVersion, "missing")}",
            $"delegation={Clean(context?.Delegation?.DelegationId, "missing")}",
            $"fitness={Clean(context?.Fitness?.EvidenceReference, "missing")}",
            $"evaluationEvidence={Clean(context?.EvidenceReference, "missing")}");

    private static string ComputeDecisionIdentity(
        AuthorityRequest? request,
        AuthorityEvaluationContext? context,
        string decision,
        string reason,
        string effectiveScope,
        DateTimeOffset expiry)
    {
        var policy = context?.Policy;
        var delegation = context?.Delegation;
        var fitness = context?.Fitness;

        var canonical = string.Join("\n",
            "request.requestId=" + Canonical(request?.RequestId),
            "request.actor=" + Canonical(request?.ActorIdentity),
            "request.action=" + Canonical(request?.Action),
            "request.resource=" + Canonical(request?.Resource),
            "request.purpose=" + Canonical(request?.Purpose),
            "request.scope=" + Canonical(request?.RequestedScope),
            "request.operatingContext=" + Canonical(request?.OperatingContext),
            "request.securityContext=" + Canonical(request?.SecurityContext),
            "request.requiredFitness=" + Canonical(request?.RequiredFitnessToOperate),
            "request.correlation=" + Canonical(request?.Correlation),
            "request.time=" + CanonicalNullableTime(request?.RequestTime),
            "request.expiry=" + CanonicalNullableTime(request?.Expiry),
            "policy.id=" + Canonical(policy?.PolicyId),
            "policy.version=" + Canonical(policy?.PolicyVersion),
            "policy.provenance=" + Canonical(policy?.AuthorityProvenance),
            "policy.effectiveFrom=" + CanonicalNullableTime(policy?.EffectiveFrom),
            "policy.expiry=" + CanonicalNullableTime(policy?.Expiry),
            "policy.actors=" + CanonicalCollection(policy?.ActorIdentities),
            "policy.actions=" + CanonicalCollection(policy?.Actions),
            "policy.resources=" + CanonicalCollection(policy?.Resources),
            "policy.purposes=" + CanonicalCollection(policy?.Purposes),
            "policy.scopes=" + CanonicalCollection(policy?.AuthorizedScopes),
            "policy.securityContexts=" + CanonicalCollection(policy?.AcceptedSecurityContexts),
            "policy.ambiguous=" + CanonicalBoolean(policy?.IsAmbiguous),
            "delegation.id=" + Canonical(delegation?.DelegationId),
            "delegation.actor=" + Canonical(delegation?.ActorIdentity),
            "delegation.provenance=" + Canonical(delegation?.AuthorityProvenance),
            "delegation.scopes=" + CanonicalCollection(delegation?.AuthorizedScopes),
            "delegation.effectiveFrom=" + CanonicalNullableTime(delegation?.EffectiveFrom),
            "delegation.expiry=" + CanonicalNullableTime(delegation?.Expiry),
            "delegation.revoked=" + CanonicalBoolean(delegation?.IsRevoked),
            "fitness.subject=" + Canonical(fitness?.SubjectIdentity),
            "fitness.level=" + Canonical(fitness?.FitnessLevel),
            "fitness.sufficient=" + CanonicalBoolean(fitness?.IsSufficient),
            "fitness.observedAt=" + CanonicalNullableTime(fitness?.ObservedAt),
            "fitness.expiry=" + CanonicalNullableTime(fitness?.Expiry),
            "fitness.evidence=" + Canonical(fitness?.EvidenceReference),
            "context.observation=" + CanonicalNullableTime(context?.ObservationTime),
            "context.evidence=" + Canonical(context?.EvidenceReference),
            "result.decision=" + decision,
            "result.reason=" + reason,
            "result.effectiveScope=" + Canonical(effectiveScope),
            "result.expiry=" + CanonicalTime(expiry));

        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return "authority-decision/sha256/" + Convert.ToHexString(digest);
    }

    private static string Canonical(string? value) =>
        value is null ? "<null>" : value.Trim();

    private static string CanonicalCollection(IEnumerable<string>? values)
    {
        if (values is null)
        {
            return "<null>";
        }

        return string.Join(",", values
            .Select(value => value is null ? "<null>" : value.Trim())
            .OrderBy(value => value, StringComparer.Ordinal));
    }

    private static string CanonicalNullableTime(DateTimeOffset? value) =>
        value.HasValue ? CanonicalTime(value.Value) : "<null>";

    private static string CanonicalBoolean(bool? value) =>
        value.HasValue ? (value.Value ? "true" : "false") : "<null>";

    private static string Clean(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static bool Contains(IEnumerable<string>? values, string candidate) =>
        values is not null && !string.IsNullOrWhiteSpace(candidate) &&
        values.Any(value => !string.IsNullOrWhiteSpace(value) &&
            StringComparer.Ordinal.Equals(value.Trim(), candidate.Trim()));

    private static bool AnyScopeContains(IEnumerable<string>? authorizedScopes, string requestedScope) =>
        authorizedScopes is not null && !string.IsNullOrWhiteSpace(requestedScope) &&
        authorizedScopes.Any(scope => !string.IsNullOrWhiteSpace(scope) && ScopeContains(scope, requestedScope));

    private static bool ScopeContains(string authorizedScope, string requestedScope)
    {
        var authorized = authorizedScope.Trim();
        var requested = requestedScope.Trim();
        return authorized == "*" ||
            StringComparer.Ordinal.Equals(authorized, requested) ||
            requested.StartsWith(authorized + ":", StringComparison.Ordinal);
    }

    private static string CanonicalTime(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
}
