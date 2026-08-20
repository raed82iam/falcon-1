using System;
using System.IO;

namespace Falcon.Foundation.Architecture.Tests;

internal static partial class Program
{
    private static readonly bool Stage16IdentityRuntimeOwnershipGuardInitialized = ValidateStage16IdentityRuntimeOwnership();

    private static bool ValidateStage16IdentityRuntimeOwnership()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var projectPath = Path.Combine(root, "src", "Foundation.IdentityRuntime", "Foundation.IdentityRuntime.csproj");
        var sourcePath = Path.Combine(root, "src", "Foundation.IdentityRuntime", "Stage16IdentityRuntime.cs");

        if (!File.Exists(projectPath) || !File.Exists(sourcePath))
        {
            throw new InvalidOperationException("Stage 16 identity runtime project/source is missing.");
        }

        var project = File.ReadAllText(projectPath);
        var source = File.ReadAllText(sourcePath);

        RequireContains(project, "<AssemblyName>Foundation.IdentityRuntime</AssemblyName>", "Stage 16 assembly ownership");
        RequireContains(project, "<RootNamespace>Foundation.IdentityRuntime</RootNamespace>", "Stage 16 root namespace ownership");
        RejectContains(project, "<ProjectReference", "Stage 16 production project must remain independently owned with zero production ProjectReferences");
        RequireContains(source, "namespace Foundation.IdentityRuntime;", "Stage 16 source namespace ownership");
        RequireContains(source, "public sealed class IdentityRuntime", "Stage 16 permanent public runtime type identity");
        RequireContains(source, "public enum IdentityStatus", "Stage 16 permanent public identity-status type");
        RequireContains(source, "public sealed record IdentityProfile", "Stage 16 permanent public identity-profile type");
        RequireContains(source, "public sealed record IdentitySession", "Stage 16 permanent public identity-session type");
        RejectContains(source, "public sealed class Stage16IdentityRuntime", "Stage identifiers must not leak into permanent public production type identity");
        RejectContains(source, "public enum FalconIdentityStatus", "Falcon token must not leak into permanent public production type identity");
        RejectContains(source, "public sealed record FalconIdentityProfile", "Falcon token must not leak into permanent public production type identity");
        RejectContains(source, "public sealed record FalconSession", "Falcon token must not leak into permanent public production type identity");

        RejectContains(source, "namespace Foundation.Authority;", "Stage 16 must not claim Authority ownership");
        RejectContains(source, "namespace Foundation.ApplicationLifecycle;", "Stage 16 must not claim ApplicationLifecycle ownership");
        RejectContains(source, "HttpClient", "Stage 16 must not perform live provider/network access");
        RejectContains(source, "System.Net.Http", "Stage 16 must not import live HTTP access");
        RejectContains(source, "Password", "Stage 16 must not model password secret-byte storage");
        RejectContains(source, "OtpSeed", "Stage 16 must not model OTP seed storage");
        RejectContains(source, "PrivateKey", "Stage 16 must not model private-key storage");
        RejectContains(source, "AccessToken", "Stage 16 must not model provider access-token storage");
        RejectContains(source, "RefreshToken", "Stage 16 must not model provider refresh-token storage");
        RejectContains(source, "EmailAddress", "Stage 16 must not create email-based identity-link authority");
        RejectContains(source, "EmailMatch", "Stage 16 must not create email-match identity-link authority");

        RequireContains(source, "GrantsBusinessAuthority: false", "authentication must not mint business authority");
        RequireContains(source, "AMBIGUOUS_EXTERNAL_IDENTITY_LINK", "ambiguous identity link must fail closed");
        RequireContains(source, "ASSERTION_REPLAY", "assertion replay must fail closed");
        RequireContains(source, "ASSERTION_NONCE_REPLAY", "assertion nonce replay must fail closed");
        RequireContains(source, "ASSERTION_ASSURANCE_INVALID", "undefined assurance must fail closed");
        RequireContains(source, "SESSION_MINIMUM_ASSURANCE_INVALID", "undefined minimum assurance policy must fail closed");
        RequireContains(source, "MaximumMfaAge", "MFA proof freshness must be explicitly bounded");
        RequireContains(source, "MFA_FRESHNESS_POLICY_INVALID", "invalid MFA freshness policy must fail closed");
        RequireContains(source, "MFA_CHALLENGE_REPLAY", "MFA replay must fail closed");
        RequireContains(source, "consumedMfaForSessions", "MFA proof reuse across session issuance must fail closed");
        RequireContains(source, "public sealed record VerifiedMfaRecovery", "MFA recovery must be explicit evidence");
        RequireContains(source, "public sealed record MfaRecoveryReceipt", "MFA recovery must produce bounded evidence");
        RequireContains(source, "MFA_RECOVERY_REPLAY", "MFA recovery replay must fail closed");
        RequireContains(source, "consumedMfaRecoveries", "MFA recovery identity must be one-time");
        RequireContains(source, "recoveredAuthenticatorReferences", "MFA predecessor recovery lineage must be fenced");
        RequireContains(source, "MFA_AUTHENTICATOR_NO_LONGER_ACTIVE", "revoked or recovered authenticator must invalidate any unconsumed MFA receipt before session issuance");
        RequireContains(source, "mfa.AuthenticatorReference", "session issuance must re-bind MFA receipt to current authenticator state");
        RequireContains(source, "SessionState.Rotated", "session rotation must fence predecessor session");

        return true;
    }

    private static void RequireContains(string value, string required, string message)
    {
        if (!value.Contains(required, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(message + ": required marker missing: " + required);
        }
    }

    private static void RejectContains(string value, string forbidden, string message)
    {
        if (value.Contains(forbidden, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(message + ": forbidden marker found: " + forbidden);
        }
    }
}