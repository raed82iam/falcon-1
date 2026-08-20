using System;
using System.Globalization;
using Foundation.Contracts;

namespace Foundation.Authority;

public static class StandingOwnerPolicyManagementReason
{
    public const string Applied = "STANDING_OWNER_POLICY_MUTATION_APPLIED";
    public const string InvalidRequest = "STANDING_OWNER_POLICY_MUTATION_INVALID";
    public const string RequestNotCurrent = "STANDING_OWNER_POLICY_MUTATION_REQUEST_NOT_CURRENT";
    public const string WrongDecisionSurface = "STANDING_OWNER_POLICY_MUTATION_WRONG_DECISION_SURFACE";
    public const string AuthenticationRequired = "STANDING_OWNER_POLICY_MUTATION_AUTHENTICATION_REQUIRED";
    public const string FreshAuthorityRequired = "STANDING_OWNER_POLICY_MUTATION_FRESH_AUTHORITY_REQUIRED";
    public const string AuthorityScopeMismatch = "STANDING_OWNER_POLICY_MUTATION_AUTHORITY_SCOPE_MISMATCH";
    public const string RegistryRejected = "STANDING_OWNER_POLICY_MUTATION_REGISTRY_REJECTED";
}

public sealed record StandingOwnerPolicyManagementRequest(
    string MutationRequestId,
    string OwnerIdentity,
    string DecisionSurfaceIdentity,
    StandingOwnerPolicyMutationOperation Operation,
    StandingOwnerPreapprovalProfile? Profile,
    string PolicyId,
    string PolicyVersion,
    bool OwnerAuthenticated,
    bool MfaSatisfied,
    string SecurityContextEvidenceReference,
    AuthorityResult FreshMutationAuthorityDecision,
    string MutationEvidenceReference,
    DateTimeOffset RequestedAt,
    DateTimeOffset Expiry);

public sealed record StandingOwnerPolicyManagementDecision(
    bool Applied,
    string Reason,
    string DecisionIdentitySha256,
    string RegistrationIdentitySha256,
    string PolicyId,
    string PolicyVersion,
    bool Revoked,
    DateTimeOffset DecisionTime,
    DateTimeOffset Expiry,
    string EvidenceReference);

public sealed class StandingOwnerPolicyManagementService
{
    private readonly StandingOwnerPreapprovalRegistry _registry;

    public StandingOwnerPolicyManagementService(StandingOwnerPreapprovalRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public StandingOwnerPolicyManagementDecision Apply(StandingOwnerPolicyManagementRequest? request, DateTimeOffset observationTime)
    {
        if (!ValidRequest(request) || observationTime == default || observationTime.Offset != TimeSpan.Zero)
            return Deny(StandingOwnerPolicyManagementReason.InvalidRequest, request, observationTime);

        var r = request!;
        if (r.RequestedAt.Offset != TimeSpan.Zero || r.Expiry.Offset != TimeSpan.Zero ||
            observationTime < r.RequestedAt || observationTime >= r.Expiry)
            return Deny(StandingOwnerPolicyManagementReason.RequestNotCurrent, r, observationTime);

        if (!StringComparer.Ordinal.Equals(r.DecisionSurfaceIdentity.Trim(), WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity))
            return Deny(StandingOwnerPolicyManagementReason.WrongDecisionSurface, r, observationTime);
        if (!r.OwnerAuthenticated || !r.MfaSatisfied || string.IsNullOrWhiteSpace(r.SecurityContextEvidenceReference))
            return Deny(StandingOwnerPolicyManagementReason.AuthenticationRequired, r, observationTime);

        var authority = r.FreshMutationAuthorityDecision;
        if (!StringComparer.Ordinal.Equals(authority.Decision, AuthorityDecision.Allow) ||
            authority.DecisionTime > observationTime || authority.Expiry <= observationTime ||
            !StringComparer.Ordinal.Equals(authority.RequestId, r.MutationRequestId.Trim()) ||
            string.IsNullOrWhiteSpace(authority.EvidenceReference) ||
            !authority.DecisionId.StartsWith("authority-decision/sha256/", StringComparison.Ordinal))
            return Deny(StandingOwnerPolicyManagementReason.FreshAuthorityRequired, r, observationTime);

        var requiredScope = "foundation:standing-owner-preapproval-policy:" + r.PolicyId.Trim();
        if (!ScopeContains(authority.EffectiveScope, requiredScope))
            return Deny(StandingOwnerPolicyManagementReason.AuthorityScopeMismatch, r, observationTime);

        var internalAuthorization = new StandingOwnerPolicyMutationAuthorization(
            r.MutationRequestId.Trim(), r.OwnerIdentity.Trim(), r.PolicyId.Trim(), r.PolicyVersion.Trim(), r.Operation,
            true, true, true, r.SecurityContextEvidenceReference.Trim(), authority.DecisionId,
            r.MutationEvidenceReference.Trim(), authority.DecisionTime, authority.Expiry);

        StandingOwnerPreapprovalRegistration registration;
        try
        {
            if (r.Operation == StandingOwnerPolicyMutationOperation.RegisterOrReplace)
            {
                if (r.Profile is null ||
                    !StringComparer.Ordinal.Equals(r.Profile.PolicyId, r.PolicyId.Trim()) ||
                    !StringComparer.Ordinal.Equals(r.Profile.PolicyVersion, r.PolicyVersion.Trim()) ||
                    !StringComparer.Ordinal.Equals(r.Profile.OwnerIdentity, r.OwnerIdentity.Trim()))
                    return Deny(StandingOwnerPolicyManagementReason.InvalidRequest, r, observationTime);
                registration = _registry.RegisterOrReplace(r.Profile, internalAuthorization, observationTime);
            }
            else if (r.Operation == StandingOwnerPolicyMutationOperation.Revoke)
            {
                registration = _registry.Revoke(r.PolicyId, r.PolicyVersion, internalAuthorization, observationTime);
            }
            else
            {
                return Deny(StandingOwnerPolicyManagementReason.InvalidRequest, r, observationTime);
            }
        }
        catch (InvalidOperationException)
        {
            return Deny(StandingOwnerPolicyManagementReason.RegistryRejected, r, observationTime);
        }

        var expiry = authority.Expiry < r.Expiry ? authority.Expiry : r.Expiry;
        var identity = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "policyManagement=APPLIED",
            "mutationRequest=" + r.MutationRequestId.Trim(),
            "owner=" + r.OwnerIdentity.Trim(),
            "surface=" + r.DecisionSurfaceIdentity.Trim(),
            "operation=" + r.Operation,
            "policy=" + r.PolicyId.Trim() + "@" + r.PolicyVersion.Trim(),
            "authorityDecision=" + authority.DecisionId,
            "registration=" + registration.IdentitySha256,
            "observation=" + observationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));

        return new StandingOwnerPolicyManagementDecision(
            true, StandingOwnerPolicyManagementReason.Applied, identity, registration.IdentitySha256,
            registration.Profile.PolicyId, registration.Profile.PolicyVersion, registration.Revoked,
            observationTime, expiry, r.MutationEvidenceReference.Trim());
    }

    private static StandingOwnerPolicyManagementDecision Deny(
        string reason, StandingOwnerPolicyManagementRequest? request, DateTimeOffset observationTime)
    {
        var safeObservation = observationTime == default ? DateTimeOffset.UnixEpoch : observationTime;
        var expiry = request?.Expiry ?? safeObservation.AddTicks(1);
        if (expiry <= safeObservation) expiry = safeObservation.AddTicks(1);
        var identity = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "policyManagement=DENIED",
            "reason=" + reason,
            "request=" + (request?.MutationRequestId?.Trim() ?? "NONE"),
            "policy=" + (request?.PolicyId?.Trim() ?? "NONE") + "@" + (request?.PolicyVersion?.Trim() ?? "NONE"),
            "observation=" + safeObservation.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
        return new StandingOwnerPolicyManagementDecision(
            false, reason, identity, "NONE", request?.PolicyId ?? "NONE", request?.PolicyVersion ?? "NONE", false,
            safeObservation, expiry, request?.MutationEvidenceReference ?? "NONE");
    }

    private static bool ValidRequest(StandingOwnerPolicyManagementRequest? request) =>
        request is not null &&
        !string.IsNullOrWhiteSpace(request.MutationRequestId) && !string.IsNullOrWhiteSpace(request.OwnerIdentity) &&
        !string.IsNullOrWhiteSpace(request.DecisionSurfaceIdentity) && Enum.IsDefined(request.Operation) &&
        !string.IsNullOrWhiteSpace(request.PolicyId) && !string.IsNullOrWhiteSpace(request.PolicyVersion) &&
        !string.IsNullOrWhiteSpace(request.SecurityContextEvidenceReference) && request.FreshMutationAuthorityDecision is not null &&
        !string.IsNullOrWhiteSpace(request.MutationEvidenceReference) && request.RequestedAt != default && request.Expiry > request.RequestedAt;

    private static bool ScopeContains(string authorizedScope, string requestedScope)
    {
        if (string.IsNullOrWhiteSpace(authorizedScope) || string.IsNullOrWhiteSpace(requestedScope)) return false;
        var authorized = authorizedScope.Trim();
        var requested = requestedScope.Trim();
        return authorized == "*" || StringComparer.Ordinal.Equals(authorized, requested) || requested.StartsWith(authorized + ":", StringComparison.Ordinal);
    }
}
