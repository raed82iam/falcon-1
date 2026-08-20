using System;
using Foundation.IdentityRuntime;

namespace Falcon.Stage16.IdentityRuntime.Verifier;

internal static class Program
{
    private const string Provider = "provider:microsoft";
    private const string Issuer = "issuer:microsoft:test";
    private const string Audience = "falcon-web";
    private static int checks;
    private static int passed;

    private static void Main()
    {
        var now = new DateTimeOffset(2026, 8, 17, 7, 0, 0, TimeSpan.Zero);
        var runtime = new global::Foundation.IdentityRuntime.IdentityRuntime();
        Check(runtime.IdentityCount == 0 && runtime.SessionCount == 0, "ZERO_APPLICATION_OPERATION");

        var owner = new IdentityProfile(
            "falcon:identity:owner-1",
            IdentityStatus.Active,
            new[] { "role:owner" },
            new[] { "entitlement:foundation-admin" },
            "evidence:identity:owner-1");
        runtime.RegisterFalconIdentity(owner);
        Check(runtime.IdentityCount == 1, "FALCON_IDENTITY_REGISTERED");
        ExpectFail(() => runtime.RegisterFalconIdentity(owner), "DUPLICATE_FALCON_IDENTITY_REJECTED");
        ExpectFail(() => runtime.RegisterFalconIdentity(owner with { FalconIdentityId = "falcon:identity:bad-status", Status = (IdentityStatus)999 }), "UNDEFINED_IDENTITY_STATUS_REJECTED");
        ExpectFail(() => runtime.SetFalconIdentityStatus(owner.FalconIdentityId, (IdentityStatus)999, "evidence:bad-status"), "UNDEFINED_STATUS_TRANSITION_REJECTED");

        var user = new IdentityProfile(
            "falcon:identity:user-1",
            IdentityStatus.Active,
            new[] { "role:user" },
            Array.Empty<string>(),
            "evidence:identity:user-1");
        runtime.RegisterFalconIdentity(user);

        var externalOwner = new ExternalIdentityKey(Provider, Issuer, "subject-owner-1");
        runtime.LinkExternalIdentity(externalOwner, owner.FalconIdentityId, "evidence:link:owner-1");
        Check(true, "EXPLICIT_EXTERNAL_LINK_ACCEPTED");
        ExpectFail(() => runtime.LinkExternalIdentity(externalOwner, user.FalconIdentityId, "evidence:link:ambiguous"), "AMBIGUOUS_EXTERNAL_LINK_REJECTED");

        var assertion = MakeAssertion("assertion-1", "subject-owner-1", now, AuthenticationAssurance.High, true);
        var receipt = runtime.IngestVerifiedAssertion(assertion, Provider, Issuer, Audience, now);
        Check(receipt.ExternalIdentity == externalOwner, "VERIFIED_ASSERTION_EXACT_IDENTITY");
        Check(receipt.AuthenticationMethod == "oidc" && receipt.Assurance == AuthenticationAssurance.High, "AUTHENTICATION_EVIDENCE_PRESERVED");
        ExpectFail(() => runtime.IngestVerifiedAssertion(MakeAssertion("invalid-assurance", "subject-owner-1", now, (AuthenticationAssurance)999, true), Provider, Issuer, Audience, now), "UNDEFINED_ASSERTION_ASSURANCE_REJECTED");
        ExpectFail(() => runtime.IngestVerifiedAssertion(MakeAssertion("unverified", "subject-owner-1", now, AuthenticationAssurance.High, false), Provider, Issuer, Audience, now), "UNVERIFIED_ASSERTION_REJECTED");
        ExpectFail(() => runtime.IngestVerifiedAssertion(MakeAssertion("wrong-provider", "subject-owner-1", now, AuthenticationAssurance.High, true), "provider:other", Issuer, Audience, now), "PROVIDER_MISMATCH_REJECTED");
        ExpectFail(() => runtime.IngestVerifiedAssertion(MakeAssertion("wrong-issuer", "subject-owner-1", now, AuthenticationAssurance.High, true), Provider, "issuer:other", Audience, now), "ISSUER_MISMATCH_REJECTED");
        ExpectFail(() => runtime.IngestVerifiedAssertion(MakeAssertion("wrong-audience", "subject-owner-1", now, AuthenticationAssurance.High, true), Provider, Issuer, "other-audience", now), "AUDIENCE_MISMATCH_REJECTED");
        ExpectFail(() => runtime.IngestVerifiedAssertion(assertion, Provider, Issuer, Audience, now), "ASSERTION_ID_REPLAY_REJECTED");
        ExpectFail(() => runtime.IngestVerifiedAssertion(MakeAssertion("assertion-new-id-same-nonce", "subject-owner-1", now, AuthenticationAssurance.High, true) with { Nonce = assertion.Nonce }, Provider, Issuer, Audience, now), "ASSERTION_NONCE_REPLAY_REJECTED");

        var expired = MakeAssertion("expired", "subject-owner-1", now, AuthenticationAssurance.High, true) with { IssuedAt = now.AddMinutes(-5), ExpiresAt = now };
        ExpectFail(() => runtime.IngestVerifiedAssertion(expired, Provider, Issuer, Audience, now), "EXPIRED_ASSERTION_REJECTED");
        Check(runtime.ResolveFalconIdentity("assertion-1") == owner.FalconIdentityId, "EXPLICIT_LINK_RESOLUTION");

        runtime.IngestVerifiedAssertion(MakeAssertion("unlinked", "subject-unknown", now, AuthenticationAssurance.High, true), Provider, Issuer, Audience, now);
        ExpectFail(() => runtime.ResolveFalconIdentity("unlinked"), "UNLINKED_EXTERNAL_IDENTITY_FAILS_CLOSED");

        const string authenticatorReference = "mfa-ref:owner-authenticator-01";
        runtime.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration(owner.FalconIdentityId, authenticatorReference, "totp-compatible", AuthenticatorState.Active, "evidence:mfa:enrollment-1"));
        Check(true, "OPAQUE_AUTHENTICATOR_REFERENCE_ACCEPTED");
        ExpectFail(() => runtime.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration(owner.FalconIdentityId, "plain-value", "totp-compatible", AuthenticatorState.Active, "evidence:mfa:bad")), "NON_OPAQUE_AUTHENTICATOR_REFERENCE_REJECTED");
        ExpectFail(() => runtime.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration(owner.FalconIdentityId, "mfa-ref:invalid-state", "totp-compatible", (AuthenticatorState)999, "evidence:mfa:bad-state")), "UNDEFINED_AUTHENTICATOR_STATE_REJECTED");

        var mfa = new VerifiedMfaChallenge("challenge-1", owner.FalconIdentityId, authenticatorReference, "evidence:mfa:challenge-1", now.AddMinutes(-1), now.AddMinutes(5), true);
        var mfaReceipt = runtime.VerifyMfaChallenge(mfa, now);
        Check(mfaReceipt.FalconIdentityId == owner.FalconIdentityId, "MFA_IDENTITY_BOUND");
        ExpectFail(() => runtime.VerifyMfaChallenge(mfa, now), "MFA_CHALLENGE_REPLAY_REJECTED");
        ExpectFail(() => runtime.VerifyMfaChallenge(mfa with { ChallengeId = "challenge-failed", VerificationPassed = false }, now), "FAILED_MFA_REJECTED");
        ExpectFail(() => runtime.VerifyMfaChallenge(mfa with { ChallengeId = "challenge-cross", FalconIdentityId = user.FalconIdentityId }, now), "CROSS_IDENTITY_MFA_REJECTED");

        var policy = new SessionPolicy(AuthenticationAssurance.High, true, "trust:owner-web", TimeSpan.FromHours(1), TimeSpan.FromMinutes(5));
        var issue = new SessionIssueRequest("session-1", owner.FalconIdentityId, "assertion-1", "challenge-1", now, now.AddMinutes(30), policy, "evidence:session:issue-1");
        var session = runtime.IssueSession(issue);
        Check(session.State == SessionState.Active, "SESSION_ISSUED");
        Check(session.RoleIds.Count == 1 && session.RoleIds[0] == "role:owner", "ROLE_FACT_PRESERVED");
        var context = runtime.ProjectSecurityContext("session-1", now.AddMinutes(1));
        Check(context.SessionId == "session-1" && context.FalconIdentityId == owner.FalconIdentityId, "SECURITY_CONTEXT_EXACT_SESSION_AND_IDENTITY");
        Check(context.Assurance == AuthenticationAssurance.High && context.TrustBoundary == "trust:owner-web", "SECURITY_CONTEXT_TRUST_FACTS");
        Check(!context.GrantsBusinessAuthority, "AUTHENTICATION_NOT_BUSINESS_AUTHORITY");
        ExpectFail(() => runtime.IssueSession(issue), "DUPLICATE_SESSION_REJECTED");

        var assertionForMfaReplay = MakeAssertion("assertion-mfa-replay", "subject-owner-1", now.AddMinutes(1), AuthenticationAssurance.High, true);
        runtime.IngestVerifiedAssertion(assertionForMfaReplay, Provider, Issuer, Audience, now.AddMinutes(1));
        ExpectFail(() => runtime.IssueSession(issue with { SessionId = "session-mfa-replay", AssertionId = assertionForMfaReplay.AssertionId, IssuedAt = now.AddMinutes(1), ExpiresAt = now.AddMinutes(20), ProvenanceEvidenceId = "evidence:session:mfa-replay" }), "MFA_PROOF_SESSION_REUSE_REJECTED");

        var rotated = runtime.RotateSession("session-1", "session-2", now.AddMinutes(2), now.AddMinutes(25), "evidence:session:rotate-1");
        Check(rotated.PredecessorSessionId == "session-1", "ROTATION_PREDECESSOR_RECORDED");
        Check(rotated.FalconIdentityId == owner.FalconIdentityId && rotated.Assurance == AuthenticationAssurance.High && rotated.MfaChallengeId == "challenge-1", "ROTATION_PRESERVES_IDENTITY_ASSURANCE_MFA");
        ExpectFail(() => runtime.ProjectSecurityContext("session-1", now.AddMinutes(3)), "ROTATED_PREDECESSOR_REJECTED");
        Check(runtime.ProjectSecurityContext("session-2", now.AddMinutes(3)).SessionId == "session-2", "ROTATED_SESSION_CURRENT");
        ExpectFail(() => runtime.RotateSession("session-2", "session-too-long", now.AddMinutes(4), now.AddHours(2), "evidence:session:bad-rotation"), "ROTATION_EXPIRY_EXTENSION_REJECTED");
        runtime.RevokeSession("session-2", "evidence:session:revoke-2");
        Check(runtime.GetSessionSnapshot("session-2").State == SessionState.Revoked, "SESSION_REVOCATION_RECORDED");
        ExpectFail(() => runtime.ProjectSecurityContext("session-2", now.AddMinutes(5)), "REVOKED_SESSION_FAILS_CLOSED");

        var noMfa = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var noMfaAssertion);
        ExpectFail(() => noMfa.IssueSession(new SessionIssueRequest("session:no-mfa", "falcon:identity:test", noMfaAssertion, null, now, now.AddMinutes(10), Policy(AuthenticationAssurance.High, true), "evidence:session:no-mfa")), "REQUIRED_MFA_MISSING_DENIED");

        var low = BuildLinkedRuntime(now, AuthenticationAssurance.Standard, out var lowAssertion);
        ExpectFail(() => low.IssueSession(new SessionIssueRequest("session:low", "falcon:identity:test", lowAssertion, null, now, now.AddMinutes(10), Policy(AuthenticationAssurance.High, false), "evidence:session:low")), "INSUFFICIENT_ASSURANCE_DENIED");

        var invalidPolicy = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var invalidPolicyAssertion);
        ExpectFail(() => invalidPolicy.IssueSession(new SessionIssueRequest("session:bad-assurance-policy", "falcon:identity:test", invalidPolicyAssertion, null, now, now.AddMinutes(10), Policy((AuthenticationAssurance)999, false), "evidence:session:bad-assurance-policy")), "UNDEFINED_MINIMUM_ASSURANCE_REJECTED");

        var invalidFreshness = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var invalidFreshnessAssertion);
        ExpectFail(() => invalidFreshness.IssueSession(new SessionIssueRequest("session:bad-freshness", "falcon:identity:test", invalidFreshnessAssertion, null, now, now.AddMinutes(10), new SessionPolicy(AuthenticationAssurance.High, true, "trust:test", TimeSpan.FromMinutes(20), TimeSpan.Zero), "evidence:session:bad-freshness")), "INVALID_MFA_FRESHNESS_POLICY_REJECTED");

        var longSession = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var longAssertion);
        ExpectFail(() => longSession.IssueSession(new SessionIssueRequest("session:long", "falcon:identity:test", longAssertion, null, now, now.AddHours(2), new SessionPolicy(AuthenticationAssurance.High, false, "trust:test", TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(5)), "evidence:session:long")), "SESSION_MAX_LIFETIME_ENFORCED");

        var earlySession = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var earlyAssertion);
        ExpectFail(() => earlySession.IssueSession(new SessionIssueRequest("session:early", "falcon:identity:test", earlyAssertion, null, now.AddMinutes(-2), now.AddMinutes(5), Policy(AuthenticationAssurance.High, false), "evidence:session:early")), "SESSION_BEFORE_ASSERTION_ISSUED_REJECTED");

        var staleMfa = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var staleAssertion);
        staleMfa.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration("falcon:identity:test", "mfa-ref:stale-proof", "totp-compatible", AuthenticatorState.Active, "evidence:mfa:stale-enroll"));
        staleMfa.VerifyMfaChallenge(new VerifiedMfaChallenge("challenge:stale", "falcon:identity:test", "mfa-ref:stale-proof", "evidence:mfa:stale-proof", now.AddMinutes(-1), now.AddMinutes(1), true), now);
        ExpectFail(() => staleMfa.IssueSession(new SessionIssueRequest("session:stale-mfa", "falcon:identity:test", staleAssertion, "challenge:stale", now.AddMinutes(6), now.AddMinutes(8), new SessionPolicy(AuthenticationAssurance.High, true, "trust:test", TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(5)), "evidence:session:stale-mfa")), "STALE_MFA_PROOF_REJECTED");

        var revokedReceipt = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var revokedReceiptAssertion);
        revokedReceipt.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration("falcon:identity:test", "mfa-ref:receipt-revoked", "totp-compatible", AuthenticatorState.Active, "evidence:mfa:receipt-enroll"));
        revokedReceipt.VerifyMfaChallenge(new VerifiedMfaChallenge("challenge:receipt-revoked", "falcon:identity:test", "mfa-ref:receipt-revoked", "evidence:mfa:receipt-proof", now.AddMinutes(-1), now.AddMinutes(2), true), now);
        revokedReceipt.RevokeMfaAuthenticator("mfa-ref:receipt-revoked", "evidence:mfa:receipt-revoke");
        ExpectFail(() => revokedReceipt.IssueSession(new SessionIssueRequest("session:revoked-receipt", "falcon:identity:test", revokedReceiptAssertion, "challenge:receipt-revoked", now, now.AddMinutes(5), Policy(AuthenticationAssurance.High, true), "evidence:session:revoked-receipt")), "REVOKED_AUTHENTICATOR_RECEIPT_REJECTED");

        var recovery = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var recoveryAssertion);
        recovery.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration("falcon:identity:test", "mfa-ref:lost-authenticator", "totp-compatible", AuthenticatorState.Active, "evidence:mfa:lost-enroll"));
        recovery.VerifyMfaChallenge(new VerifiedMfaChallenge("challenge:pre-recovery", "falcon:identity:test", "mfa-ref:lost-authenticator", "evidence:mfa:pre-recovery", now.AddMinutes(-1), now.AddMinutes(2), true), now);
        var recoveryRequest = new VerifiedMfaRecovery("recovery-1", "falcon:identity:test", "mfa-ref:lost-authenticator", "mfa-ref:replacement-authenticator", "totp-compatible", "evidence:mfa:recovery-1", now.AddMinutes(-1), now.AddMinutes(5), true);
        var recoveryReceipt = recovery.RecoverMfaAuthenticator(recoveryRequest, now);
        Check(recoveryReceipt.ReplacementAuthenticatorReference == "mfa-ref:replacement-authenticator", "MFA_RECOVERY_REPLACEMENT_BOUND");
        ExpectFail(() => recovery.RecoverMfaAuthenticator(recoveryRequest, now), "MFA_RECOVERY_REPLAY_REJECTED");
        ExpectFail(() => recovery.VerifyMfaChallenge(new VerifiedMfaChallenge("challenge:old-after-recovery", "falcon:identity:test", "mfa-ref:lost-authenticator", "evidence:mfa:old", now, now.AddMinutes(2), true), now), "RECOVERED_PREDECESSOR_AUTHENTICATOR_REJECTED");
        ExpectFail(() => recovery.IssueSession(new SessionIssueRequest("session:pre-recovery-receipt", "falcon:identity:test", recoveryAssertion, "challenge:pre-recovery", now, now.AddMinutes(5), Policy(AuthenticationAssurance.High, true), "evidence:session:pre-recovery-receipt")), "RECOVERED_AUTHENTICATOR_RECEIPT_REJECTED");
        Check(recovery.VerifyMfaChallenge(new VerifiedMfaChallenge("challenge:new-after-recovery", "falcon:identity:test", "mfa-ref:replacement-authenticator", "evidence:mfa:new", now, now.AddMinutes(2), true), now).AuthenticatorReference == "mfa-ref:replacement-authenticator", "RECOVERY_REPLACEMENT_AUTHENTICATOR_ACTIVE");

        var crossRecovery = BuildLinkedRuntime(now, AuthenticationAssurance.High, out _);
        crossRecovery.RegisterFalconIdentity(new IdentityProfile("falcon:identity:other", IdentityStatus.Active, Array.Empty<string>(), Array.Empty<string>(), "evidence:identity:other"));
        crossRecovery.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration("falcon:identity:test", "mfa-ref:cross-recovery-old", "totp-compatible", AuthenticatorState.Active, "evidence:mfa:cross-enroll"));
        ExpectFail(() => crossRecovery.RecoverMfaAuthenticator(new VerifiedMfaRecovery("recovery-cross", "falcon:identity:other", "mfa-ref:cross-recovery-old", "mfa-ref:cross-recovery-new", "totp-compatible", "evidence:mfa:cross-recovery", now, now.AddMinutes(2), true), now), "CROSS_IDENTITY_MFA_RECOVERY_REJECTED");
        ExpectFail(() => crossRecovery.RecoverMfaAuthenticator(new VerifiedMfaRecovery("recovery-unverified", "falcon:identity:test", "mfa-ref:cross-recovery-old", "mfa-ref:unverified-recovery-new", "totp-compatible", "evidence:mfa:unverified-recovery", now, now.AddMinutes(2), false), now), "UNVERIFIED_MFA_RECOVERY_REJECTED");
        ExpectFail(() => crossRecovery.RecoverMfaAuthenticator(new VerifiedMfaRecovery("recovery-nonopaque", "falcon:identity:test", "mfa-ref:cross-recovery-old", "plain-value", "totp-compatible", "evidence:mfa:nonopaque-recovery", now, now.AddMinutes(2), true), now), "NON_OPAQUE_RECOVERY_REPLACEMENT_REJECTED");

        var revokedAuthenticator = BuildLinkedRuntime(now, AuthenticationAssurance.High, out _);
        revokedAuthenticator.RegisterMfaAuthenticator(new MfaAuthenticatorRegistration("falcon:identity:test", "mfa-ref:revoked-authenticator", "totp-compatible", AuthenticatorState.Active, "evidence:mfa:enroll"));
        revokedAuthenticator.RevokeMfaAuthenticator("mfa-ref:revoked-authenticator", "evidence:mfa:revoke");
        ExpectFail(() => revokedAuthenticator.VerifyMfaChallenge(new VerifiedMfaChallenge("challenge:revoked", "falcon:identity:test", "mfa-ref:revoked-authenticator", "evidence:mfa:verify", now.AddMinutes(-1), now.AddMinutes(1), true), now), "REVOKED_AUTHENTICATOR_DENIED");

        var disableRuntime = BuildLinkedRuntime(now, AuthenticationAssurance.High, out var disableAssertion);
        disableRuntime.SetFalconIdentityStatus("falcon:identity:test", IdentityStatus.Disabled, "evidence:identity:disable");
        ExpectFail(() => disableRuntime.ResolveFalconIdentity(disableAssertion), "DISABLED_IDENTITY_CANNOT_RESOLVE_FOR_NEW_USE");

        Console.WriteLine("STAGE16_IDENTITY_RUNTIME_VERIFIER = PASS");
        Console.WriteLine($"CHECKS = {passed}/{checks}");
        Console.WriteLine("AUTHENTICATION_NOT_AUTHORIZATION = PASS");
        Console.WriteLine("EXPLICIT_IDENTITY_LINK_ONLY = PASS");
        Console.WriteLine("ASSERTION_AND_MFA_REPLAY_PROTECTION = PASS");
        Console.WriteLine("ASSURANCE_ENUM_FAIL_CLOSED = PASS");
        Console.WriteLine("MFA_FRESHNESS_BOUND = PASS");
        Console.WriteLine("MFA_RECOVERY_REFERENCE_FLOW = PASS");
        Console.WriteLine("MFA_AUTHENTICATOR_REVOCATION_INVALIDATES_RECEIPT = PASS");
        Console.WriteLine("SESSION_ROTATION_REVOKES_PREDECESSOR = PASS");
        Console.WriteLine("REVOCATION_FAIL_CLOSED = PASS");
        Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");

        if (passed != checks) Environment.ExitCode = 1;
    }

    private static SessionPolicy Policy(AuthenticationAssurance assurance, bool requireMfa) =>
        new(assurance, requireMfa, "trust:test", TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(5));

    private static VerifiedExternalAssertion MakeAssertion(string assertionId, string subject, DateTimeOffset now, AuthenticationAssurance assurance, bool verified) =>
        new(assertionId, Provider, Issuer, subject, Audience, "nonce:" + assertionId, "evidence:assertion:" + assertionId, "oidc", assurance, now.AddMinutes(-1), now.AddMinutes(9), verified);

    private static global::Foundation.IdentityRuntime.IdentityRuntime BuildLinkedRuntime(DateTimeOffset now, AuthenticationAssurance assurance, out string assertionId)
    {
        var runtime = new global::Foundation.IdentityRuntime.IdentityRuntime();
        var identity = new IdentityProfile("falcon:identity:test", IdentityStatus.Active, Array.Empty<string>(), Array.Empty<string>(), "evidence:identity:test");
        runtime.RegisterFalconIdentity(identity);
        runtime.LinkExternalIdentity(new ExternalIdentityKey(Provider, Issuer, "subject-test"), identity.FalconIdentityId, "evidence:link:test");
        assertionId = "assertion:test:" + assurance;
        runtime.IngestVerifiedAssertion(MakeAssertion(assertionId, "subject-test", now, assurance, true), Provider, Issuer, Audience, now);
        return runtime;
    }

    private static void Check(bool condition, string name)
    {
        checks++;
        if (!condition) throw new InvalidOperationException("CHECK_FAILED:" + name);
        passed++;
    }

    private static void ExpectFail(Action action, string name)
    {
        checks++;
        try { action(); }
        catch (InvalidOperationException) { passed++; return; }
        throw new InvalidOperationException("EXPECTED_FAILURE_DID_NOT_OCCUR:" + name);
    }
}