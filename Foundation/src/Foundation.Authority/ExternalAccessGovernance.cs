using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public static class ExternalAccessPurpose
{
    public const string Research = "RESEARCH";
    public const string NonLiveValidation = "NON_LIVE_VALIDATION";
    public const string OperationalProviderData = "OPERATIONAL_PROVIDER_DATA";
    public const string BrokerExecution = "BROKER_EXECUTION";
    public const string PresentationData = "PRESENTATION_DATA";

    public static bool IsKnown(string? value) =>
        StringComparer.Ordinal.Equals(value, Research) ||
        StringComparer.Ordinal.Equals(value, NonLiveValidation) ||
        StringComparer.Ordinal.Equals(value, OperationalProviderData) ||
        StringComparer.Ordinal.Equals(value, BrokerExecution) ||
        StringComparer.Ordinal.Equals(value, PresentationData);
}

public static class ExternalAuthenticationMode
{
    public const string Public = "PUBLIC";
    public const string CredentialReference = "CREDENTIAL_REFERENCE";
    public const string ChannelDependent = "CHANNEL_DEPENDENT";

    public static bool IsKnown(string? value) =>
        StringComparer.Ordinal.Equals(value, Public) ||
        StringComparer.Ordinal.Equals(value, CredentialReference) ||
        StringComparer.Ordinal.Equals(value, ChannelDependent);
}

public static class ExternalAccessEnvironment
{
    public const string NonLive = "NON_LIVE";
    public const string Live = "LIVE";
    public const string Neutral = "ENVIRONMENT_NEUTRAL";

    public static bool IsKnown(string? value) =>
        StringComparer.Ordinal.Equals(value, NonLive) ||
        StringComparer.Ordinal.Equals(value, Live) ||
        StringComparer.Ordinal.Equals(value, Neutral);
}

public static class ExternalAccessDecision
{
    public const string Allow = "ALLOW";
    public const string Deny = "DENY";
}

public static class ExternalAccessReason
{
    public const string Allowed = "EXTERNAL_ACCESS_ALLOWED";
    public const string RequestMalformed = "EXTERNAL_ACCESS_REQUEST_MALFORMED";
    public const string AuthorityMissingOrDenied = "EXTERNAL_ACCESS_AUTHORITY_MISSING_OR_DENIED";
    public const string AuthorityBindingMismatch = "EXTERNAL_ACCESS_AUTHORITY_BINDING_MISMATCH";
    public const string AuthorityExpired = "EXTERNAL_ACCESS_AUTHORITY_EXPIRED";
    public const string PolicyMissing = "EXTERNAL_ACCESS_POLICY_MISSING";
    public const string PolicyMalformed = "EXTERNAL_ACCESS_POLICY_MALFORMED";
    public const string PolicyAmbiguous = "EXTERNAL_ACCESS_POLICY_AMBIGUOUS";
    public const string RouteNotAuthorized = "EXTERNAL_ACCESS_ROUTE_NOT_AUTHORIZED";
    public const string CredentialReferenceMissing = "EXTERNAL_ACCESS_CREDENTIAL_REFERENCE_MISSING";
    public const string CredentialReferenceInvalid = "EXTERNAL_ACCESS_CREDENTIAL_REFERENCE_INVALID";
    public const string CredentialReferenceMismatch = "EXTERNAL_ACCESS_CREDENTIAL_REFERENCE_MISMATCH";
    public const string CredentialReferenceRevoked = "EXTERNAL_ACCESS_CREDENTIAL_REFERENCE_REVOKED";
    public const string CredentialReferenceExpired = "EXTERNAL_ACCESS_CREDENTIAL_REFERENCE_EXPIRED";
    public const string NonLiveLiveBoundaryDenied = "EXTERNAL_ACCESS_NON_LIVE_LIVE_BOUNDARY_DENIED";
    public const string PurposeBoundaryDenied = "EXTERNAL_ACCESS_PURPOSE_BOUNDARY_DENIED";
    public const string EvidenceMissing = "EXTERNAL_ACCESS_EVIDENCE_MISSING";
}

public sealed record ExternalCredentialReference(
    string ReferenceId,
    string PrincipalIdentity,
    string ServiceRole,
    string Environment,
    string Purpose,
    string Destination,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset Expiry,
    bool IsRevoked,
    string ProvenanceReference);

public sealed record ExternalAccessRequest(
    string RequestId,
    string PrincipalIdentity,
    string ServiceRole,
    string Environment,
    string Purpose,
    string Destination,
    string AuthenticationMode,
    string? CredentialReferenceId,
    string AuthorityRequestId,
    string AuthorityDecisionId,
    string RequiredAuthorityScope,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry,
    string CorrelationId);

public sealed record ExternalAccessPolicyRule(
    string RuleId,
    string PolicyId,
    string PolicyVersion,
    string PrincipalIdentity,
    string ServiceRole,
    string Environment,
    string Purpose,
    string Destination,
    string AuthenticationMode,
    string RequiredAuthorityScope,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset Expiry,
    bool IsRevoked,
    string ProvenanceReference);

public sealed record ExternalAccessEvaluationContext(
    IReadOnlyCollection<ExternalAccessPolicyRule> PolicyRules,
    AuthorityResult? AuthorityResult,
    ExternalCredentialReference? CredentialReference,
    DateTimeOffset ObservationTime,
    string EvidenceReference);

public sealed record ExternalAccessResult(
    string RequestId,
    string DecisionId,
    string Decision,
    string Reason,
    string PrincipalIdentity,
    string ServiceRole,
    string Environment,
    string Purpose,
    string Destination,
    string AuthenticationMode,
    string CredentialReferenceId,
    string ControllingPolicy,
    string PolicyVersion,
    string AuthorityDecisionId,
    DateTimeOffset DecisionTime,
    DateTimeOffset Expiry,
    string EvidenceReference,
    string Constraints);

public sealed class ExternalAccessEvaluator
{
    public ExternalAccessResult Evaluate(ExternalAccessRequest? request, ExternalAccessEvaluationContext? context)
    {
        var observation = context?.ObservationTime ?? DateTimeOffset.UnixEpoch;
        var evaluation = EvaluateReason(request, context);
        var decision = StringComparer.Ordinal.Equals(evaluation.Reason, ExternalAccessReason.Allowed)
            ? ExternalAccessDecision.Allow
            : ExternalAccessDecision.Deny;
        var rule = evaluation.Rule;
        var requestId = Clean(request?.RequestId, "missing-request");
        var expiry = ComputeExpiry(request, context, rule, observation);
        var credentialRef = decision == ExternalAccessDecision.Allow
            ? Clean(request?.CredentialReferenceId, "NONE")
            : "NONE";
        var decisionId = ComputeDecisionIdentity(request, context, rule, decision, evaluation.Reason, expiry);

        return new ExternalAccessResult(
            requestId,
            decisionId,
            decision,
            evaluation.Reason,
            Clean(request?.PrincipalIdentity, "UNKNOWN"),
            Clean(request?.ServiceRole, "UNKNOWN"),
            Clean(request?.Environment, "UNKNOWN"),
            Clean(request?.Purpose, "UNKNOWN"),
            Clean(request?.Destination, "UNKNOWN"),
            Clean(request?.AuthenticationMode, "UNKNOWN"),
            credentialRef,
            Clean(rule?.PolicyId, "NONE"),
            Clean(rule?.PolicyVersion, "NONE"),
            Clean(context?.AuthorityResult?.DecisionId, "NONE"),
            observation,
            expiry,
            Clean(context?.EvidenceReference, "missing-evidence"),
            decision == ExternalAccessDecision.Allow
                ? "EXACT_ROUTE_ONLY / NO_NETWORK_EXECUTION / NO_BUSINESS_AUTHORITY"
                : "NO_EXTERNAL_ACCESS_AUTHORITY");
    }

    private static (string Reason, ExternalAccessPolicyRule? Rule) EvaluateReason(
        ExternalAccessRequest? request,
        ExternalAccessEvaluationContext? context)
    {
        if (!IsValidRequest(request))
        {
            return (ExternalAccessReason.RequestMalformed, null);
        }

        if (context is null || context.ObservationTime == default || context.PolicyRules is null)
        {
            return (ExternalAccessReason.PolicyMissing, null);
        }

        if (string.IsNullOrWhiteSpace(context.EvidenceReference))
        {
            return (ExternalAccessReason.EvidenceMissing, null);
        }

        var authority = context.AuthorityResult;
        if (authority is null || !StringComparer.Ordinal.Equals(authority.Decision, AuthorityDecision.Allow))
        {
            return (ExternalAccessReason.AuthorityMissingOrDenied, null);
        }

        if (!StringComparer.Ordinal.Equals(authority.RequestId, request!.AuthorityRequestId) ||
            !StringComparer.Ordinal.Equals(authority.DecisionId, request.AuthorityDecisionId) ||
            !StringComparer.Ordinal.Equals(authority.EffectiveScope, request.RequiredAuthorityScope))
        {
            return (ExternalAccessReason.AuthorityBindingMismatch, null);
        }

        if (context.ObservationTime < request.RequestTime ||
            context.ObservationTime >= request.Expiry ||
            context.ObservationTime < authority.DecisionTime ||
            context.ObservationTime >= authority.Expiry)
        {
            return (ExternalAccessReason.AuthorityExpired, null);
        }

        var exactRules = context.PolicyRules
            .Where(rule => RuleIdentityMatches(rule, request))
            .ToArray();

        if (exactRules.Length == 0)
        {
            return (ExternalAccessReason.RouteNotAuthorized, null);
        }

        if (exactRules.Length > 1)
        {
            return (ExternalAccessReason.PolicyAmbiguous, null);
        }

        var rule = exactRules[0];
        if (!IsValidRule(rule))
        {
            return (ExternalAccessReason.PolicyMalformed, rule);
        }

        if (rule.IsRevoked || context.ObservationTime < rule.EffectiveFrom || context.ObservationTime >= rule.Expiry)
        {
            return (ExternalAccessReason.RouteNotAuthorized, rule);
        }

        if (StringComparer.Ordinal.Equals(request.Environment, ExternalAccessEnvironment.NonLive) &&
            StringComparer.Ordinal.Equals(rule.Environment, ExternalAccessEnvironment.Live))
        {
            return (ExternalAccessReason.NonLiveLiveBoundaryDenied, rule);
        }

        if (!StringComparer.Ordinal.Equals(request.Purpose, rule.Purpose))
        {
            return (ExternalAccessReason.PurposeBoundaryDenied, rule);
        }

        if (StringComparer.Ordinal.Equals(request.AuthenticationMode, ExternalAuthenticationMode.CredentialReference) ||
            StringComparer.Ordinal.Equals(request.AuthenticationMode, ExternalAuthenticationMode.ChannelDependent))
        {
            var credentialResult = ValidateCredential(request, context.CredentialReference, context.ObservationTime);
            if (!StringComparer.Ordinal.Equals(credentialResult, ExternalAccessReason.Allowed))
            {
                return (credentialResult, rule);
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.CredentialReferenceId))
        {
            return (ExternalAccessReason.CredentialReferenceMismatch, rule);
        }

        return (ExternalAccessReason.Allowed, rule);
    }

    private static string ValidateCredential(
        ExternalAccessRequest request,
        ExternalCredentialReference? credential,
        DateTimeOffset observationTime)
    {
        if (string.IsNullOrWhiteSpace(request.CredentialReferenceId) || credential is null)
        {
            return ExternalAccessReason.CredentialReferenceMissing;
        }

        if (!IsValidCredentialReference(credential))
        {
            return ExternalAccessReason.CredentialReferenceInvalid;
        }

        if (!StringComparer.Ordinal.Equals(credential.ReferenceId, request.CredentialReferenceId) ||
            !StringComparer.Ordinal.Equals(credential.PrincipalIdentity, request.PrincipalIdentity) ||
            !StringComparer.Ordinal.Equals(credential.ServiceRole, request.ServiceRole) ||
            !StringComparer.Ordinal.Equals(credential.Environment, request.Environment) ||
            !StringComparer.Ordinal.Equals(credential.Purpose, request.Purpose) ||
            !StringComparer.Ordinal.Equals(credential.Destination, request.Destination))
        {
            return ExternalAccessReason.CredentialReferenceMismatch;
        }

        if (credential.IsRevoked)
        {
            return ExternalAccessReason.CredentialReferenceRevoked;
        }

        if (observationTime < credential.EffectiveFrom || observationTime >= credential.Expiry)
        {
            return ExternalAccessReason.CredentialReferenceExpired;
        }

        return ExternalAccessReason.Allowed;
    }

    private static bool RuleIdentityMatches(ExternalAccessPolicyRule rule, ExternalAccessRequest request) =>
        StringComparer.Ordinal.Equals(Clean(rule.PrincipalIdentity, string.Empty), request.PrincipalIdentity) &&
        StringComparer.Ordinal.Equals(Clean(rule.ServiceRole, string.Empty), request.ServiceRole) &&
        StringComparer.Ordinal.Equals(Clean(rule.Environment, string.Empty), request.Environment) &&
        StringComparer.Ordinal.Equals(Clean(rule.Purpose, string.Empty), request.Purpose) &&
        StringComparer.Ordinal.Equals(Clean(rule.Destination, string.Empty), request.Destination) &&
        StringComparer.Ordinal.Equals(Clean(rule.AuthenticationMode, string.Empty), request.AuthenticationMode) &&
        StringComparer.Ordinal.Equals(Clean(rule.RequiredAuthorityScope, string.Empty), request.RequiredAuthorityScope);

    private static bool IsValidRequest(ExternalAccessRequest? request)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(request.RequestId) ||
            string.IsNullOrWhiteSpace(request.PrincipalIdentity) ||
            string.IsNullOrWhiteSpace(request.ServiceRole) ||
            string.IsNullOrWhiteSpace(request.Destination) ||
            string.IsNullOrWhiteSpace(request.AuthorityRequestId) ||
            string.IsNullOrWhiteSpace(request.AuthorityDecisionId) ||
            string.IsNullOrWhiteSpace(request.RequiredAuthorityScope) ||
            string.IsNullOrWhiteSpace(request.CorrelationId) ||
            request.RequestTime == default ||
            request.Expiry <= request.RequestTime ||
            !ExternalAccessEnvironment.IsKnown(request.Environment) ||
            !ExternalAccessPurpose.IsKnown(request.Purpose) ||
            !ExternalAuthenticationMode.IsKnown(request.AuthenticationMode))
        {
            return false;
        }

        return Uri.TryCreate(request.Destination, UriKind.Absolute, out var uri) &&
               (StringComparer.OrdinalIgnoreCase.Equals(uri.Scheme, "https") ||
                StringComparer.OrdinalIgnoreCase.Equals(uri.Scheme, "wss"));
    }

    private static bool IsValidRule(ExternalAccessPolicyRule rule) =>
        !string.IsNullOrWhiteSpace(rule.RuleId) &&
        !string.IsNullOrWhiteSpace(rule.PolicyId) &&
        !string.IsNullOrWhiteSpace(rule.PolicyVersion) &&
        !string.IsNullOrWhiteSpace(rule.PrincipalIdentity) &&
        !string.IsNullOrWhiteSpace(rule.ServiceRole) &&
        ExternalAccessEnvironment.IsKnown(rule.Environment) &&
        ExternalAccessPurpose.IsKnown(rule.Purpose) &&
        !string.IsNullOrWhiteSpace(rule.Destination) &&
        ExternalAuthenticationMode.IsKnown(rule.AuthenticationMode) &&
        !string.IsNullOrWhiteSpace(rule.RequiredAuthorityScope) &&
        rule.EffectiveFrom != default &&
        rule.Expiry > rule.EffectiveFrom &&
        !string.IsNullOrWhiteSpace(rule.ProvenanceReference) &&
        Uri.TryCreate(rule.Destination, UriKind.Absolute, out var uri) &&
        (StringComparer.OrdinalIgnoreCase.Equals(uri.Scheme, "https") ||
         StringComparer.OrdinalIgnoreCase.Equals(uri.Scheme, "wss"));

    private static bool IsValidCredentialReference(ExternalCredentialReference value) =>
        !string.IsNullOrWhiteSpace(value.ReferenceId) &&
        !LooksLikeSecret(value.ReferenceId) &&
        !string.IsNullOrWhiteSpace(value.PrincipalIdentity) &&
        !string.IsNullOrWhiteSpace(value.ServiceRole) &&
        ExternalAccessEnvironment.IsKnown(value.Environment) &&
        ExternalAccessPurpose.IsKnown(value.Purpose) &&
        !string.IsNullOrWhiteSpace(value.Destination) &&
        value.EffectiveFrom != default &&
        value.Expiry > value.EffectiveFrom &&
        !string.IsNullOrWhiteSpace(value.ProvenanceReference);

    private static bool LooksLikeSecret(string reference)
    {
        var value = reference.Trim();
        return value.Contains("=", StringComparison.Ordinal) ||
               value.Contains("Bearer ", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("token=", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("api_key=", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("apikey=", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("password=", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("secret=", StringComparison.OrdinalIgnoreCase);
    }

    private static DateTimeOffset ComputeExpiry(
        ExternalAccessRequest? request,
        ExternalAccessEvaluationContext? context,
        ExternalAccessPolicyRule? rule,
        DateTimeOffset fallback)
    {
        var values = new List<DateTimeOffset>();
        if (request is not null && request.Expiry > fallback) values.Add(request.Expiry);
        if (context?.AuthorityResult is not null && context.AuthorityResult.Expiry > fallback) values.Add(context.AuthorityResult.Expiry);
        if (rule is not null && rule.Expiry > fallback) values.Add(rule.Expiry);
        if (context?.CredentialReference is not null && context.CredentialReference.Expiry > fallback) values.Add(context.CredentialReference.Expiry);
        return values.Count == 0 ? fallback : values.Min();
    }

    private static string ComputeDecisionIdentity(
        ExternalAccessRequest? request,
        ExternalAccessEvaluationContext? context,
        ExternalAccessPolicyRule? rule,
        string decision,
        string reason,
        DateTimeOffset expiry)
    {
        var material = string.Join("\n", new[]
        {
            "EXT-001",
            Clean(request?.RequestId, "missing-request"),
            Clean(request?.PrincipalIdentity, "UNKNOWN"),
            Clean(request?.ServiceRole, "UNKNOWN"),
            Clean(request?.Environment, "UNKNOWN"),
            Clean(request?.Purpose, "UNKNOWN"),
            Clean(request?.Destination, "UNKNOWN"),
            Clean(request?.AuthenticationMode, "UNKNOWN"),
            Clean(request?.CredentialReferenceId, "NONE"),
            Clean(request?.AuthorityRequestId, "NONE"),
            Clean(request?.AuthorityDecisionId, "NONE"),
            Clean(request?.RequiredAuthorityScope, "NONE"),
            Clean(rule?.RuleId, "NONE"),
            Clean(rule?.PolicyId, "NONE"),
            Clean(rule?.PolicyVersion, "NONE"),
            Clean(context?.EvidenceReference, "missing-evidence"),
            decision,
            reason,
            (context?.ObservationTime ?? DateTimeOffset.UnixEpoch).ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            expiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(material));
        return "sha256/" + Convert.ToHexString(bytes);
    }

    private static string Clean(string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}
