using System;
using System.Collections.Generic;
using System.Globalization;

namespace Foundation.Authority;

public enum StandingOwnerPolicyMutationOperation
{
    RegisterOrReplace = 0,
    Revoke = 1
}

public sealed record StandingOwnerPolicyMutationAuthorization(
    string MutationRequestId,
    string OwnerIdentity,
    string PolicyId,
    string PolicyVersion,
    StandingOwnerPolicyMutationOperation Operation,
    bool OwnerAuthenticated,
    bool MfaSatisfied,
    bool MutationAuthorized,
    string SecurityContextEvidenceReference,
    string AuthorityDecisionId,
    string MutationEvidenceReference,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt);

public sealed record StandingOwnerPreapprovalRegistration
{
    internal StandingOwnerPreapprovalRegistration(
        StandingOwnerPreapprovalProfile profile,
        bool revoked,
        StandingOwnerPolicyMutationAuthorization mutationAuthorization,
        DateTimeOffset registeredAt)
    {
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
        MutationAuthorization = mutationAuthorization ?? throw new ArgumentNullException(nameof(mutationAuthorization));
        Revoked = revoked;
        RegisteredAt = registeredAt;
        IdentitySha256 = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "registration.profile=" + Profile.IdentitySha256,
            "registration.revoked=" + (Revoked ? "true" : "false"),
            "registration.mutationRequest=" + MutationAuthorization.MutationRequestId.Trim(),
            "registration.owner=" + MutationAuthorization.OwnerIdentity.Trim(),
            "registration.operation=" + MutationAuthorization.Operation,
            "registration.authorityDecision=" + MutationAuthorization.AuthorityDecisionId.Trim(),
            "registration.securityEvidence=" + MutationAuthorization.SecurityContextEvidenceReference.Trim(),
            "registration.mutationEvidence=" + MutationAuthorization.MutationEvidenceReference.Trim(),
            "registration.registeredAt=" + RegisteredAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
    }

    public StandingOwnerPreapprovalProfile Profile { get; }
    public bool Revoked { get; }
    public StandingOwnerPolicyMutationAuthorization MutationAuthorization { get; }
    public DateTimeOffset RegisteredAt { get; }
    public string IdentitySha256 { get; }
}

public sealed class StandingOwnerPreapprovalRegistry
{
    private readonly Dictionary<string, StandingOwnerPreapprovalRegistration> _registrations = new(StringComparer.Ordinal);

    public int Count => _registrations.Count;

    internal StandingOwnerPreapprovalRegistration RegisterOrReplace(
        StandingOwnerPreapprovalProfile profile,
        StandingOwnerPolicyMutationAuthorization authorization,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ValidateMutationAuthorization(authorization, StandingOwnerPolicyMutationOperation.RegisterOrReplace, profile.PolicyId, profile.PolicyVersion, profile.OwnerIdentity, now);
        if (profile.IsRevoked) throw new InvalidOperationException("STANDING_PREAPPROVAL_CANNOT_REGISTER_REVOKED_PROFILE");
        if (now < profile.EffectiveFrom || now >= profile.Expiry)
            throw new InvalidOperationException("STANDING_PREAPPROVAL_PROFILE_NOT_CURRENT_AT_REGISTRATION");

        if (_registrations.TryGetValue(profile.PolicyId, out var current))
        {
            if (!IsStrictlyNewerVersion(profile.PolicyVersion, current.Profile.PolicyVersion))
                throw new InvalidOperationException("STANDING_PREAPPROVAL_VERSION_NOT_STRICTLY_NEWER");
        }

        var registration = new StandingOwnerPreapprovalRegistration(profile, false, authorization, now);
        _registrations[profile.PolicyId] = registration;
        return registration;
    }

    internal StandingOwnerPreapprovalRegistration Revoke(
        string policyId,
        string expectedCurrentVersion,
        StandingOwnerPolicyMutationAuthorization authorization,
        DateTimeOffset now)
    {
        Require(policyId, nameof(policyId));
        Require(expectedCurrentVersion, nameof(expectedCurrentVersion));
        if (!_registrations.TryGetValue(policyId.Trim(), out var current))
            throw new InvalidOperationException("STANDING_PREAPPROVAL_POLICY_NOT_FOUND");
        if (current.Revoked) throw new InvalidOperationException("STANDING_PREAPPROVAL_POLICY_ALREADY_REVOKED");
        if (!StringComparer.Ordinal.Equals(current.Profile.PolicyVersion, expectedCurrentVersion.Trim()))
            throw new InvalidOperationException("STANDING_PREAPPROVAL_EXPECTED_VERSION_MISMATCH");

        ValidateMutationAuthorization(
            authorization,
            StandingOwnerPolicyMutationOperation.Revoke,
            current.Profile.PolicyId,
            current.Profile.PolicyVersion,
            current.Profile.OwnerIdentity,
            now);

        var registration = new StandingOwnerPreapprovalRegistration(current.Profile, true, authorization, now);
        _registrations[current.Profile.PolicyId] = registration;
        return registration;
    }

    public bool TryGet(string policyId, out StandingOwnerPreapprovalRegistration? registration)
    {
        registration = null;
        if (string.IsNullOrWhiteSpace(policyId)) return false;
        return _registrations.TryGetValue(policyId.Trim(), out registration);
    }

    public StandingOwnerPreapprovalRegistration GetRequired(string policyId)
    {
        if (!TryGet(policyId, out var registration) || registration is null)
            throw new InvalidOperationException("STANDING_PREAPPROVAL_POLICY_NOT_FOUND");
        return registration;
    }

    private static void ValidateMutationAuthorization(
        StandingOwnerPolicyMutationAuthorization authorization,
        StandingOwnerPolicyMutationOperation requiredOperation,
        string policyId,
        string policyVersion,
        string ownerIdentity,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        Require(authorization.MutationRequestId, nameof(authorization.MutationRequestId));
        Require(authorization.OwnerIdentity, nameof(authorization.OwnerIdentity));
        Require(authorization.PolicyId, nameof(authorization.PolicyId));
        Require(authorization.PolicyVersion, nameof(authorization.PolicyVersion));
        Require(authorization.SecurityContextEvidenceReference, nameof(authorization.SecurityContextEvidenceReference));
        Require(authorization.AuthorityDecisionId, nameof(authorization.AuthorityDecisionId));
        Require(authorization.MutationEvidenceReference, nameof(authorization.MutationEvidenceReference));

        if (!Enum.IsDefined(authorization.Operation) || authorization.Operation != requiredOperation)
            throw new InvalidOperationException("STANDING_PREAPPROVAL_MUTATION_OPERATION_MISMATCH");
        if (!authorization.OwnerAuthenticated)
            throw new InvalidOperationException("STANDING_PREAPPROVAL_OWNER_AUTHENTICATION_REQUIRED");
        if (!authorization.MfaSatisfied)
            throw new InvalidOperationException("STANDING_PREAPPROVAL_OWNER_MFA_REQUIRED");
        if (!authorization.MutationAuthorized)
            throw new InvalidOperationException("STANDING_PREAPPROVAL_MUTATION_AUTHORITY_REQUIRED");
        if (authorization.IssuedAt == default || authorization.ExpiresAt <= authorization.IssuedAt || authorization.IssuedAt > now || authorization.ExpiresAt <= now)
            throw new InvalidOperationException("STANDING_PREAPPROVAL_MUTATION_AUTHORIZATION_EXPIRED_OR_INVALID");
        if (!authorization.AuthorityDecisionId.StartsWith("authority-decision/sha256/", StringComparison.Ordinal))
            throw new InvalidOperationException("STANDING_PREAPPROVAL_AUTHORITY_DECISION_ID_INVALID");
        if (!StringComparer.Ordinal.Equals(authorization.OwnerIdentity.Trim(), ownerIdentity.Trim()))
            throw new InvalidOperationException("STANDING_PREAPPROVAL_OWNER_IDENTITY_MISMATCH");
        if (!StringComparer.Ordinal.Equals(authorization.PolicyId.Trim(), policyId.Trim()))
            throw new InvalidOperationException("STANDING_PREAPPROVAL_POLICY_ID_MISMATCH");
        if (!StringComparer.Ordinal.Equals(authorization.PolicyVersion.Trim(), policyVersion.Trim()))
            throw new InvalidOperationException("STANDING_PREAPPROVAL_POLICY_VERSION_MISMATCH");
    }

    private static bool IsStrictlyNewerVersion(string candidate, string current)
    {
        if (!Version.TryParse(candidate, out var candidateVersion) || !Version.TryParse(current, out var currentVersion))
            throw new InvalidOperationException("STANDING_PREAPPROVAL_POLICY_VERSION_INVALID");
        return candidateVersion.CompareTo(currentVersion) > 0;
    }

    private static void Require(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value is required.", name);
    }
}
