using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.IdentityRuntime;

public enum IdentityStatus { Active = 0, Disabled = 1, Revoked = 2 }
public enum AuthenticationAssurance { Low = 0, Standard = 1, High = 2, PhishingResistant = 3 }
public enum AuthenticatorState { Active = 0, Revoked = 1 }
public enum SessionState { Active = 0, Rotated = 1, Revoked = 2, LoggedOut = 3 }

public sealed record IdentityProfile(
    string FalconIdentityId,
    IdentityStatus Status,
    IReadOnlyList<string> RoleIds,
    IReadOnlyList<string> EntitlementIds,
    string EvidenceId);

public sealed record ExternalIdentityKey(string ProviderId, string Issuer, string ExternalSubjectId);

public sealed record ExternalIdentityLink(
    ExternalIdentityKey ExternalIdentity,
    string FalconIdentityId,
    string EvidenceId);

public sealed record VerifiedExternalAssertion(
    string AssertionId,
    string ProviderId,
    string Issuer,
    string ExternalSubjectId,
    string Audience,
    string Nonce,
    string VerificationEvidenceId,
    string AuthenticationMethod,
    AuthenticationAssurance Assurance,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    bool CryptographicVerificationPassed);

public sealed record ExternalAssertionReceipt(
    string AssertionId,
    ExternalIdentityKey ExternalIdentity,
    string Audience,
    string Nonce,
    string VerificationEvidenceId,
    string AuthenticationMethod,
    AuthenticationAssurance Assurance,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt);

public sealed record MfaAuthenticatorRegistration(
    string FalconIdentityId,
    string AuthenticatorReference,
    string AuthenticatorType,
    AuthenticatorState State,
    string EvidenceId);

public sealed record VerifiedMfaChallenge(
    string ChallengeId,
    string FalconIdentityId,
    string AuthenticatorReference,
    string VerificationEvidenceId,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    bool VerificationPassed);

public sealed record MfaReceipt(
    string ChallengeId,
    string FalconIdentityId,
    string AuthenticatorReference,
    string VerificationEvidenceId,
    DateTimeOffset VerifiedAt);

public sealed record VerifiedMfaRecovery(
    string RecoveryId,
    string FalconIdentityId,
    string PreviousAuthenticatorReference,
    string ReplacementAuthenticatorReference,
    string ReplacementAuthenticatorType,
    string VerificationEvidenceId,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    bool VerificationPassed);

public sealed record MfaRecoveryReceipt(
    string RecoveryId,
    string FalconIdentityId,
    string PreviousAuthenticatorReference,
    string ReplacementAuthenticatorReference,
    string VerificationEvidenceId,
    DateTimeOffset RecoveredAt);

public sealed record SessionPolicy(
    AuthenticationAssurance MinimumAssurance,
    bool RequireMfa,
    string TrustBoundary,
    TimeSpan MaximumSessionLifetime,
    TimeSpan MaximumMfaAge);

public sealed record SessionIssueRequest(
    string SessionId,
    string FalconIdentityId,
    string AssertionId,
    string? MfaChallengeId,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    SessionPolicy Policy,
    string ProvenanceEvidenceId);

public sealed record IdentitySession(
    string SessionId,
    string FalconIdentityId,
    string AuthenticationMethod,
    AuthenticationAssurance Assurance,
    string? MfaChallengeId,
    string TrustBoundary,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    SessionState State,
    string ProvenanceEvidenceId,
    string? PredecessorSessionId,
    IReadOnlyList<string> RoleIds,
    IReadOnlyList<string> EntitlementIds);

public sealed record SecurityContextProjection(
    string FalconIdentityId,
    string AuthenticationMethod,
    AuthenticationAssurance Assurance,
    string SessionId,
    string TrustBoundary,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    bool Revoked,
    string ProvenanceEvidenceId,
    IReadOnlyList<string> RoleIds,
    IReadOnlyList<string> EntitlementIds,
    bool GrantsBusinessAuthority);

public sealed class IdentityRuntime
{
    private readonly Dictionary<string, IdentityProfile> identities = new(StringComparer.Ordinal);
    private readonly Dictionary<ExternalIdentityKey, ExternalIdentityLink> externalLinks = new();
    private readonly Dictionary<string, ExternalAssertionReceipt> assertions = new(StringComparer.Ordinal);
    private readonly HashSet<string> assertionReplayKeys = new(StringComparer.Ordinal);
    private readonly HashSet<string> consumedAssertions = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MfaAuthenticatorRegistration> authenticators = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MfaReceipt> mfaReceipts = new(StringComparer.Ordinal);
    private readonly HashSet<string> consumedMfaChallenges = new(StringComparer.Ordinal);
    private readonly HashSet<string> consumedMfaForSessions = new(StringComparer.Ordinal);
    private readonly HashSet<string> consumedMfaRecoveries = new(StringComparer.Ordinal);
    private readonly HashSet<string> recoveredAuthenticatorReferences = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IdentitySession> sessions = new(StringComparer.Ordinal);

    public int IdentityCount => identities.Count;
    public int SessionCount => sessions.Count;

    public void RegisterFalconIdentity(IdentityProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        Require(profile.FalconIdentityId, nameof(profile.FalconIdentityId));
        Require(profile.EvidenceId, nameof(profile.EvidenceId));
        if (!Enum.IsDefined(profile.Status)) throw new InvalidOperationException("IDENTITY_STATUS_INVALID");
        ValidateUniqueNonEmpty(profile.RoleIds, nameof(profile.RoleIds));
        ValidateUniqueNonEmpty(profile.EntitlementIds, nameof(profile.EntitlementIds));
        if (!identities.TryAdd(profile.FalconIdentityId, profile with
        {
            RoleIds = profile.RoleIds.ToArray(),
            EntitlementIds = profile.EntitlementIds.ToArray()
        })) throw new InvalidOperationException("DUPLICATE_FALCON_IDENTITY");
    }

    public void SetFalconIdentityStatus(string falconIdentityId, IdentityStatus status, string evidenceId)
    {
        Require(falconIdentityId, nameof(falconIdentityId));
        Require(evidenceId, nameof(evidenceId));
        if (!Enum.IsDefined(status)) throw new InvalidOperationException("IDENTITY_STATUS_INVALID");
        identities[falconIdentityId] = GetIdentity(falconIdentityId) with { Status = status, EvidenceId = evidenceId };
    }

    public void LinkExternalIdentity(ExternalIdentityKey externalIdentity, string falconIdentityId, string evidenceId)
    {
        ArgumentNullException.ThrowIfNull(externalIdentity);
        ValidateExternalKey(externalIdentity);
        Require(falconIdentityId, nameof(falconIdentityId));
        Require(evidenceId, nameof(evidenceId));
        EnsureIdentityActive(GetIdentity(falconIdentityId));
        if (externalLinks.TryGetValue(externalIdentity, out var existing))
        {
            if (!StringComparer.Ordinal.Equals(existing.FalconIdentityId, falconIdentityId))
                throw new InvalidOperationException("AMBIGUOUS_EXTERNAL_IDENTITY_LINK");
            throw new InvalidOperationException("DUPLICATE_EXTERNAL_IDENTITY_LINK");
        }
        externalLinks.Add(externalIdentity, new ExternalIdentityLink(externalIdentity, falconIdentityId, evidenceId));
    }

    public ExternalAssertionReceipt IngestVerifiedAssertion(
        VerifiedExternalAssertion assertion,
        string expectedProviderId,
        string expectedIssuer,
        string expectedAudience,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(assertion);
        Require(assertion.AssertionId, nameof(assertion.AssertionId));
        Require(assertion.ProviderId, nameof(assertion.ProviderId));
        Require(assertion.Issuer, nameof(assertion.Issuer));
        Require(assertion.ExternalSubjectId, nameof(assertion.ExternalSubjectId));
        Require(assertion.Audience, nameof(assertion.Audience));
        Require(assertion.Nonce, nameof(assertion.Nonce));
        Require(assertion.VerificationEvidenceId, nameof(assertion.VerificationEvidenceId));
        Require(assertion.AuthenticationMethod, nameof(assertion.AuthenticationMethod));
        Require(expectedProviderId, nameof(expectedProviderId));
        Require(expectedIssuer, nameof(expectedIssuer));
        Require(expectedAudience, nameof(expectedAudience));

        if (!Enum.IsDefined(assertion.Assurance))
            throw new InvalidOperationException("ASSERTION_ASSURANCE_INVALID");
        if (!assertion.CryptographicVerificationPassed)
            throw new InvalidOperationException("ASSERTION_NOT_CRYPTOGRAPHICALLY_VERIFIED");
        if (!StringComparer.Ordinal.Equals(assertion.ProviderId, expectedProviderId) ||
            !StringComparer.Ordinal.Equals(assertion.Issuer, expectedIssuer) ||
            !StringComparer.Ordinal.Equals(assertion.Audience, expectedAudience))
            throw new InvalidOperationException("ASSERTION_TRUST_BINDING_MISMATCH");
        if (assertion.IssuedAt > now || assertion.ExpiresAt <= now || assertion.ExpiresAt <= assertion.IssuedAt)
            throw new InvalidOperationException("ASSERTION_TIME_INVALID");
        if (assertions.ContainsKey(assertion.AssertionId))
            throw new InvalidOperationException("ASSERTION_REPLAY");

        var replayKey = string.Concat(assertion.ProviderId, "|", assertion.Issuer, "|", assertion.Nonce);
        if (!assertionReplayKeys.Add(replayKey))
            throw new InvalidOperationException("ASSERTION_NONCE_REPLAY");

        var receipt = new ExternalAssertionReceipt(
            assertion.AssertionId,
            new ExternalIdentityKey(assertion.ProviderId, assertion.Issuer, assertion.ExternalSubjectId),
            assertion.Audience,
            assertion.Nonce,
            assertion.VerificationEvidenceId,
            assertion.AuthenticationMethod,
            assertion.Assurance,
            assertion.IssuedAt,
            assertion.ExpiresAt);
        assertions.Add(receipt.AssertionId, receipt);
        return receipt;
    }

    public string ResolveFalconIdentity(string assertionId)
    {
        var assertion = GetAssertion(assertionId);
        if (!externalLinks.TryGetValue(assertion.ExternalIdentity, out var link))
            throw new InvalidOperationException("EXTERNAL_IDENTITY_NOT_LINKED");
        EnsureIdentityActive(GetIdentity(link.FalconIdentityId));
        return link.FalconIdentityId;
    }

    public void RegisterMfaAuthenticator(MfaAuthenticatorRegistration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        Require(registration.FalconIdentityId, nameof(registration.FalconIdentityId));
        Require(registration.AuthenticatorReference, nameof(registration.AuthenticatorReference));
        Require(registration.AuthenticatorType, nameof(registration.AuthenticatorType));
        Require(registration.EvidenceId, nameof(registration.EvidenceId));
        if (!Enum.IsDefined(registration.State)) throw new InvalidOperationException("AUTHENTICATOR_STATE_INVALID");
        EnsureIdentityActive(GetIdentity(registration.FalconIdentityId));
        ValidateOpaqueAuthenticatorReference(registration.AuthenticatorReference);
        if (!authenticators.TryAdd(registration.AuthenticatorReference, registration))
            throw new InvalidOperationException("DUPLICATE_AUTHENTICATOR_REFERENCE");
    }

    public void RevokeMfaAuthenticator(string authenticatorReference, string evidenceId)
    {
        Require(authenticatorReference, nameof(authenticatorReference));
        Require(evidenceId, nameof(evidenceId));
        authenticators[authenticatorReference] = GetAuthenticator(authenticatorReference) with
        {
            State = AuthenticatorState.Revoked,
            EvidenceId = evidenceId
        };
    }

    public MfaReceipt VerifyMfaChallenge(VerifiedMfaChallenge challenge, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(challenge);
        Require(challenge.ChallengeId, nameof(challenge.ChallengeId));
        Require(challenge.FalconIdentityId, nameof(challenge.FalconIdentityId));
        Require(challenge.AuthenticatorReference, nameof(challenge.AuthenticatorReference));
        Require(challenge.VerificationEvidenceId, nameof(challenge.VerificationEvidenceId));
        if (!challenge.VerificationPassed) throw new InvalidOperationException("MFA_NOT_VERIFIED");
        if (challenge.IssuedAt > now || challenge.ExpiresAt <= now || challenge.ExpiresAt <= challenge.IssuedAt)
            throw new InvalidOperationException("MFA_CHALLENGE_TIME_INVALID");
        if (consumedMfaChallenges.Contains(challenge.ChallengeId) || mfaReceipts.ContainsKey(challenge.ChallengeId))
            throw new InvalidOperationException("MFA_CHALLENGE_REPLAY");

        EnsureIdentityActive(GetIdentity(challenge.FalconIdentityId));
        var authenticator = GetAuthenticator(challenge.AuthenticatorReference);
        if (authenticator.State != AuthenticatorState.Active ||
            !StringComparer.Ordinal.Equals(authenticator.FalconIdentityId, challenge.FalconIdentityId))
            throw new InvalidOperationException("MFA_AUTHENTICATOR_BINDING_INVALID");

        var receipt = new MfaReceipt(
            challenge.ChallengeId,
            challenge.FalconIdentityId,
            challenge.AuthenticatorReference,
            challenge.VerificationEvidenceId,
            now);
        mfaReceipts.Add(receipt.ChallengeId, receipt);
        consumedMfaChallenges.Add(receipt.ChallengeId);
        return receipt;
    }

    public MfaRecoveryReceipt RecoverMfaAuthenticator(VerifiedMfaRecovery recovery, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(recovery);
        Require(recovery.RecoveryId, nameof(recovery.RecoveryId));
        Require(recovery.FalconIdentityId, nameof(recovery.FalconIdentityId));
        Require(recovery.PreviousAuthenticatorReference, nameof(recovery.PreviousAuthenticatorReference));
        Require(recovery.ReplacementAuthenticatorReference, nameof(recovery.ReplacementAuthenticatorReference));
        Require(recovery.ReplacementAuthenticatorType, nameof(recovery.ReplacementAuthenticatorType));
        Require(recovery.VerificationEvidenceId, nameof(recovery.VerificationEvidenceId));
        if (!recovery.VerificationPassed) throw new InvalidOperationException("MFA_RECOVERY_NOT_VERIFIED");
        if (recovery.IssuedAt > now || recovery.ExpiresAt <= now || recovery.ExpiresAt <= recovery.IssuedAt)
            throw new InvalidOperationException("MFA_RECOVERY_TIME_INVALID");
        if (consumedMfaRecoveries.Contains(recovery.RecoveryId))
            throw new InvalidOperationException("MFA_RECOVERY_REPLAY");
        if (StringComparer.Ordinal.Equals(recovery.PreviousAuthenticatorReference, recovery.ReplacementAuthenticatorReference))
            throw new InvalidOperationException("MFA_RECOVERY_REPLACEMENT_MUST_DIFFER");

        EnsureIdentityActive(GetIdentity(recovery.FalconIdentityId));
        ValidateOpaqueAuthenticatorReference(recovery.ReplacementAuthenticatorReference);
        var previous = GetAuthenticator(recovery.PreviousAuthenticatorReference);
        if (previous.State != AuthenticatorState.Active ||
            !StringComparer.Ordinal.Equals(previous.FalconIdentityId, recovery.FalconIdentityId))
            throw new InvalidOperationException("MFA_RECOVERY_PREDECESSOR_BINDING_INVALID");
        if (recoveredAuthenticatorReferences.Contains(previous.AuthenticatorReference))
            throw new InvalidOperationException("MFA_AUTHENTICATOR_ALREADY_RECOVERED");
        if (authenticators.ContainsKey(recovery.ReplacementAuthenticatorReference))
            throw new InvalidOperationException("DUPLICATE_AUTHENTICATOR_REFERENCE");

        authenticators[previous.AuthenticatorReference] = previous with
        {
            State = AuthenticatorState.Revoked,
            EvidenceId = recovery.VerificationEvidenceId
        };
        authenticators.Add(
            recovery.ReplacementAuthenticatorReference,
            new MfaAuthenticatorRegistration(
                recovery.FalconIdentityId,
                recovery.ReplacementAuthenticatorReference,
                recovery.ReplacementAuthenticatorType,
                AuthenticatorState.Active,
                recovery.VerificationEvidenceId));
        consumedMfaRecoveries.Add(recovery.RecoveryId);
        recoveredAuthenticatorReferences.Add(previous.AuthenticatorReference);

        return new MfaRecoveryReceipt(
            recovery.RecoveryId,
            recovery.FalconIdentityId,
            recovery.PreviousAuthenticatorReference,
            recovery.ReplacementAuthenticatorReference,
            recovery.VerificationEvidenceId,
            now);
    }

    public IdentitySession IssueSession(SessionIssueRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateSessionRequest(request);
        if (sessions.ContainsKey(request.SessionId)) throw new InvalidOperationException("DUPLICATE_SESSION_ID");
        if (consumedAssertions.Contains(request.AssertionId)) throw new InvalidOperationException("ASSERTION_ALREADY_USED_FOR_SESSION");

        var identity = GetIdentity(request.FalconIdentityId);
        EnsureIdentityActive(identity);
        var assertion = GetAssertion(request.AssertionId);
        if (!externalLinks.TryGetValue(assertion.ExternalIdentity, out var link) ||
            !StringComparer.Ordinal.Equals(link.FalconIdentityId, request.FalconIdentityId))
            throw new InvalidOperationException("SESSION_IDENTITY_LINK_MISMATCH");
        if (request.IssuedAt < assertion.IssuedAt || assertion.ExpiresAt <= request.IssuedAt)
            throw new InvalidOperationException("STALE_OR_NOT_YET_VALID_ASSERTION_FOR_SESSION");
        if (assertion.Assurance < request.Policy.MinimumAssurance)
            throw new InvalidOperationException("INSUFFICIENT_AUTHENTICATION_ASSURANCE");

        string? mfaChallengeId = null;
        if (request.Policy.RequireMfa)
        {
            if (string.IsNullOrWhiteSpace(request.MfaChallengeId) ||
                !mfaReceipts.TryGetValue(request.MfaChallengeId, out var mfa) ||
                !StringComparer.Ordinal.Equals(mfa.FalconIdentityId, request.FalconIdentityId) ||
                mfa.VerifiedAt > request.IssuedAt ||
                request.IssuedAt - mfa.VerifiedAt > request.Policy.MaximumMfaAge ||
                consumedMfaForSessions.Contains(mfa.ChallengeId))
                throw new InvalidOperationException("REQUIRED_MFA_EVIDENCE_MISSING_MISMATCHED_STALE_OR_REPLAYED");

            var mfaAuthenticator = GetAuthenticator(mfa.AuthenticatorReference);
            if (mfaAuthenticator.State != AuthenticatorState.Active ||
                !StringComparer.Ordinal.Equals(mfaAuthenticator.FalconIdentityId, request.FalconIdentityId))
                throw new InvalidOperationException("MFA_AUTHENTICATOR_NO_LONGER_ACTIVE");

            mfaChallengeId = mfa.ChallengeId;
        }

        var session = new IdentitySession(
            request.SessionId,
            request.FalconIdentityId,
            assertion.AuthenticationMethod,
            assertion.Assurance,
            mfaChallengeId,
            request.Policy.TrustBoundary,
            request.IssuedAt,
            request.ExpiresAt,
            SessionState.Active,
            request.ProvenanceEvidenceId,
            null,
            identity.RoleIds.ToArray(),
            identity.EntitlementIds.ToArray());
        sessions.Add(session.SessionId, session);
        consumedAssertions.Add(request.AssertionId);
        if (mfaChallengeId is not null) consumedMfaForSessions.Add(mfaChallengeId);
        return session;
    }

    public IdentitySession RotateSession(
        string predecessorSessionId,
        string newSessionId,
        DateTimeOffset issuedAt,
        DateTimeOffset expiresAt,
        string provenanceEvidenceId)
    {
        Require(predecessorSessionId, nameof(predecessorSessionId));
        Require(newSessionId, nameof(newSessionId));
        Require(provenanceEvidenceId, nameof(provenanceEvidenceId));
        if (sessions.ContainsKey(newSessionId)) throw new InvalidOperationException("DUPLICATE_SESSION_ID");
        var predecessor = GetCurrentSession(predecessorSessionId, issuedAt);
        EnsureIdentityActive(GetIdentity(predecessor.FalconIdentityId));
        if (expiresAt <= issuedAt || expiresAt > predecessor.ExpiresAt)
            throw new InvalidOperationException("ROTATED_SESSION_TIME_INVALID");

        sessions[predecessorSessionId] = predecessor with { State = SessionState.Rotated };
        var rotated = predecessor with
        {
            SessionId = newSessionId,
            IssuedAt = issuedAt,
            ExpiresAt = expiresAt,
            State = SessionState.Active,
            ProvenanceEvidenceId = provenanceEvidenceId,
            PredecessorSessionId = predecessorSessionId
        };
        sessions.Add(newSessionId, rotated);
        return rotated;
    }

    public void RevokeSession(string sessionId, string evidenceId)
    {
        Require(evidenceId, nameof(evidenceId));
        sessions[sessionId] = GetSession(sessionId) with { State = SessionState.Revoked, ProvenanceEvidenceId = evidenceId };
    }

    public void Logout(string sessionId, string evidenceId)
    {
        Require(evidenceId, nameof(evidenceId));
        sessions[sessionId] = GetSession(sessionId) with { State = SessionState.LoggedOut, ProvenanceEvidenceId = evidenceId };
    }

    public SecurityContextProjection ProjectSecurityContext(string sessionId, DateTimeOffset now)
    {
        var session = GetCurrentSession(sessionId, now);
        EnsureIdentityActive(GetIdentity(session.FalconIdentityId));
        return new SecurityContextProjection(
            session.FalconIdentityId,
            session.AuthenticationMethod,
            session.Assurance,
            session.SessionId,
            session.TrustBoundary,
            session.IssuedAt,
            session.ExpiresAt,
            false,
            session.ProvenanceEvidenceId,
            session.RoleIds.ToArray(),
            session.EntitlementIds.ToArray(),
            GrantsBusinessAuthority: false);
    }

    public IdentitySession GetSessionSnapshot(string sessionId) => GetSession(sessionId);

    private void ValidateSessionRequest(SessionIssueRequest request)
    {
        Require(request.SessionId, nameof(request.SessionId));
        Require(request.FalconIdentityId, nameof(request.FalconIdentityId));
        Require(request.AssertionId, nameof(request.AssertionId));
        Require(request.ProvenanceEvidenceId, nameof(request.ProvenanceEvidenceId));
        ArgumentNullException.ThrowIfNull(request.Policy);
        Require(request.Policy.TrustBoundary, nameof(request.Policy.TrustBoundary));
        if (!Enum.IsDefined(request.Policy.MinimumAssurance))
            throw new InvalidOperationException("SESSION_MINIMUM_ASSURANCE_INVALID");
        if (request.Policy.MaximumSessionLifetime <= TimeSpan.Zero)
            throw new InvalidOperationException("SESSION_POLICY_LIFETIME_INVALID");
        if (request.Policy.RequireMfa && request.Policy.MaximumMfaAge <= TimeSpan.Zero)
            throw new InvalidOperationException("MFA_FRESHNESS_POLICY_INVALID");
        if (request.ExpiresAt <= request.IssuedAt || request.ExpiresAt - request.IssuedAt > request.Policy.MaximumSessionLifetime)
            throw new InvalidOperationException("SESSION_TIME_INVALID");
    }

    private IdentityProfile GetIdentity(string id)
    {
        Require(id, nameof(id));
        return identities.TryGetValue(id, out var value) ? value : throw new InvalidOperationException("UNKNOWN_FALCON_IDENTITY");
    }

    private ExternalAssertionReceipt GetAssertion(string id)
    {
        Require(id, nameof(id));
        return assertions.TryGetValue(id, out var value) ? value : throw new InvalidOperationException("UNKNOWN_ASSERTION");
    }

    private MfaAuthenticatorRegistration GetAuthenticator(string reference)
    {
        Require(reference, nameof(reference));
        return authenticators.TryGetValue(reference, out var value) ? value : throw new InvalidOperationException("UNKNOWN_AUTHENTICATOR_REFERENCE");
    }

    private IdentitySession GetSession(string id)
    {
        Require(id, nameof(id));
        return sessions.TryGetValue(id, out var value) ? value : throw new InvalidOperationException("UNKNOWN_SESSION");
    }

    private IdentitySession GetCurrentSession(string id, DateTimeOffset now)
    {
        var session = GetSession(id);
        if (session.State != SessionState.Active) throw new InvalidOperationException("SESSION_NOT_ACTIVE");
        if (session.IssuedAt > now || session.ExpiresAt <= now) throw new InvalidOperationException("SESSION_NOT_CURRENT");
        return session;
    }

    private static void EnsureIdentityActive(IdentityProfile identity)
    {
        if (identity.Status != IdentityStatus.Active) throw new InvalidOperationException("FALCON_IDENTITY_NOT_ACTIVE");
    }

    private static void ValidateExternalKey(ExternalIdentityKey key)
    {
        Require(key.ProviderId, nameof(key.ProviderId));
        Require(key.Issuer, nameof(key.Issuer));
        Require(key.ExternalSubjectId, nameof(key.ExternalSubjectId));
    }

    private static void ValidateOpaqueAuthenticatorReference(string value)
    {
        if (!value.StartsWith("mfa-ref:", StringComparison.Ordinal) || value.Length is < 12 or > 128 ||
            value.Any(ch => !(char.IsLetterOrDigit(ch) || ch is ':' or '-' or '_' or '.')))
            throw new InvalidOperationException("AUTHENTICATOR_REFERENCE_NOT_OPAQUE");
    }

    private static void ValidateUniqueNonEmpty(IReadOnlyList<string> values, string field)
    {
        ArgumentNullException.ThrowIfNull(values);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in values)
        {
            Require(value, field);
            if (!seen.Add(value)) throw new InvalidOperationException($"DUPLICATE_{field.ToUpperInvariant()}");
        }
    }

    private static void Require(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"MISSING_{field.ToUpperInvariant()}");
    }
}