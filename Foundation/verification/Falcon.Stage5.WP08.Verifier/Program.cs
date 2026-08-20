using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Foundation.MessageProtection;

namespace Falcon.Stage5.WP08.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 8, 8, 45, 0, TimeSpan.Zero);

    private static int Main()
    {
        var scenarios = new (string Name, Func<bool> Run)[]
        {
            ("approved_profile_protects", ApprovedProfileProtects),
            ("approved_profile_verifies_and_recovers_exact_plaintext", ApprovedProfileVerifiesAndRecoversExactPlaintext),
            ("ciphertext_tampering_rejected", CiphertextTamperingRejected),
            ("tag_tampering_rejected", TagTamperingRejected),
            ("nonce_tampering_rejected", NonceTamperingRejected),
            ("wrong_recipient_context_rejected", WrongRecipientContextRejected),
            ("wrong_classification_context_rejected", WrongClassificationContextRejected),
            ("wrong_message_digest_context_rejected", WrongMessageDigestContextRejected),
            ("wrong_route_context_rejected", WrongRouteContextRejected),
            ("wrong_delivery_context_rejected", WrongDeliveryContextRejected),
            ("wrong_event_context_rejected", WrongEventContextRejected),
            ("wrong_replay_classification_context_rejected", WrongReplayClassificationContextRejected),
            ("wrong_correlation_context_rejected", WrongCorrelationContextRejected),
            ("wrong_causation_context_rejected", WrongCausationContextRejected),
            ("wrong_profile_version_rejected", WrongProfileVersionRejected),
            ("prohibited_profile_rejected", ProhibitedProfileRejected),
            ("disabled_profile_rejected", DisabledProfileRejected),
            ("deprecated_profile_rejected", DeprecatedProfileRejected),
            ("future_profile_rejected", FutureProfileRejected),
            ("expired_profile_rejected", ExpiredProfileRejected),
            ("unsupported_algorithm_rejected", UnsupportedAlgorithmRejected),
            ("unsupported_parameters_rejected", UnsupportedParametersRejected),
            ("wrong_key_class_rejected", WrongKeyClassRejected),
            ("wrong_key_profile_rejected", WrongKeyProfileRejected),
            ("future_key_rejected", FutureKeyRejected),
            ("expired_key_rejected", ExpiredKeyRejected),
            ("revoked_key_rejected", RevokedKeyRejected),
            ("retired_key_rejected", RetiredKeyRejected),
            ("disabled_key_rejected", DisabledKeyRejected),
            ("unknown_key_rejected", UnknownKeyRejected),
            ("wrong_key_scope_rejected", WrongKeyScopeRejected),
            ("key_material_unavailable_rejected", KeyMaterialUnavailableRejected),
            ("wrong_key_material_authentication_rejected", WrongKeyMaterialAuthenticationRejected),
            ("nonce_reuse_rejected", NonceReuseRejected),
            ("invalid_nonce_size_rejected", InvalidNonceSizeRejected),
            ("failed_verification_releases_no_plaintext", FailedVerificationReleasesNoPlaintext),
            ("arbitrary_binary_payload_round_trips", ArbitraryBinaryPayloadRoundTrips),
            ("application_names_do_not_change_semantics", ApplicationNamesDoNotChangeSemantics),
            ("context_digest_is_deterministic", ContextDigestIsDeterministic),
            ("profile_identity_is_deterministic", ProfileIdentityIsDeterministic),
            ("key_reference_identity_is_deterministic", KeyReferenceIdentityIsDeterministic),
            ("protection_evidence_contains_no_plaintext", ProtectionEvidenceContainsNoPlaintext),
            ("protection_evidence_contains_no_key_material", ProtectionEvidenceContainsNoKeyMaterial),
            ("verification_evidence_contains_no_key_material", VerificationEvidenceContainsNoKeyMaterial),
            ("package_identity_changes_with_ciphertext", PackageIdentityChangesWithCiphertext),
            ("profile_binding_changes_package_identity", ProfileBindingChangesPackageIdentity),
            ("key_version_binding_enforced", KeyVersionBindingEnforced),
            ("empty_optional_predecessor_bindings_are_deterministic", EmptyOptionalPredecessorBindingsAreDeterministic),
        };

        var failures = new List<string>();
        foreach (var scenario in scenarios)
        {
            bool passed;
            try { passed = scenario.Run(); }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION {scenario.Name}: {ex.GetType().Name}: {ex.Message}");
                passed = false;
            }

            Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {scenario.Name}");
            if (!passed) failures.Add(scenario.Name);
        }

        Console.WriteLine($"RESULT {scenarios.Length - failures.Count}/{scenarios.Length} PASS");
        if (failures.Count != 0)
        {
            foreach (var failure in failures) Console.WriteLine($"FAILED_SCENARIO {failure}");
            return 1;
        }

        Console.WriteLine("STAGE 5 WP-08 CRYPTOGRAPHIC MESSAGE PROTECTION VERIFIER: PASS");
        return 0;
    }

    private static bool ApprovedProfileProtects()
    {
        var f = Fixture.Create();
        var result = f.Protector.Protect(f.Plaintext, f.Profile, f.KeyReference, f.Context, Now);
        return result.Kind == CryptographicDecisionKind.Protected &&
               result.Reason == CryptographicProtectionReason.Protected &&
               result.Package is not null;
    }

    private static bool ApprovedProfileVerifiesAndRecoversExactPlaintext()
    {
        var f = Fixture.Create();
        var result = f.Protector.Verify(RequirePackage(f), f.Profile, f.KeyReference, f.Context, Now);
        return result.Kind == CryptographicDecisionKind.Verified &&
               result.Reason == CryptographicProtectionReason.Verified &&
               result.Plaintext.Span.SequenceEqual(f.Plaintext.Span);
    }

    private static bool CiphertextTamperingRejected()
    {
        var f = Fixture.Create();
        var p = RequirePackage(f);
        var bytes = p.Ciphertext.ToArray();
        bytes[0] ^= 0x01;
        return RejectsVerification(f, ClonePackage(p, ciphertext: bytes), f.Context, CryptographicProtectionReason.AuthenticationFailed);
    }

    private static bool TagTamperingRejected()
    {
        var f = Fixture.Create();
        var p = RequirePackage(f);
        var tag = p.AuthenticationTag.ToArray();
        tag[0] ^= 0x01;
        return RejectsVerification(f, ClonePackage(p, tag: tag), f.Context, CryptographicProtectionReason.AuthenticationFailed);
    }

    private static bool NonceTamperingRejected()
    {
        var f = Fixture.Create();
        var p = RequirePackage(f);
        var nonce = p.Nonce.ToArray();
        nonce[0] ^= 0x01;
        return RejectsVerification(f, ClonePackage(p, nonce: nonce), f.Context, CryptographicProtectionReason.AuthenticationFailed);
    }

    private static bool WrongRecipientContextRejected()
    {
        var f = Fixture.Create();
        var p = RequirePackage(f);
        var changed = CreateContext(recipientScope: "app:other");
        return RejectsVerification(f, p, changed, CryptographicProtectionReason.KeyScopeMismatch);
    }

    private static bool WrongClassificationContextRejected() => ContextMutationRejected(CreateContext(classification: "restricted"));
    private static bool WrongMessageDigestContextRejected() => ContextMutationRejected(CreateContext(messageDigest: Sha("other-message")));
    private static bool WrongRouteContextRejected() => ContextMutationRejected(CreateContext(routeDecisionId: "route:other"));
    private static bool WrongDeliveryContextRejected() => ContextMutationRejected(CreateContext(deliveryDecisionId: "delivery:other"));
    private static bool WrongEventContextRejected() => ContextMutationRejected(CreateContext(eventIdentity: Sha("event-other")));
    private static bool WrongReplayClassificationContextRejected() => ContextMutationRejected(CreateContext(eventClassification: "replay"));
    private static bool WrongCorrelationContextRejected() => ContextMutationRejected(CreateContext(correlationId: "corr:other"));
    private static bool WrongCausationContextRejected() => ContextMutationRejected(CreateContext(causationId: "cause:other"));

    private static bool ContextMutationRejected(ProtectedMessageContext changed)
    {
        var f = Fixture.Create();
        return RejectsVerification(f, RequirePackage(f), changed, CryptographicProtectionReason.ContextMismatch);
    }

    private static bool WrongProfileVersionRejected()
    {
        var f = Fixture.Create();
        return RejectsVerification(f, RequirePackage(f), f.Context, CryptographicProtectionReason.ProfileUnknown, CreateProfile(version: "2"));
    }

    private static bool ProhibitedProfileRejected() => ProtectReason(CreateProfile(status: CryptographicProfileStatus.Prohibited), CreateKey(), CryptographicProtectionReason.ProfileProhibited);
    private static bool DisabledProfileRejected() => ProtectReason(CreateProfile(status: CryptographicProfileStatus.Disabled), CreateKey(), CryptographicProtectionReason.ProfileDisabled);
    private static bool DeprecatedProfileRejected() => ProtectReason(CreateProfile(status: CryptographicProfileStatus.Deprecated), CreateKey(), CryptographicProtectionReason.ProfileDeprecated);
    private static bool FutureProfileRejected() => ProtectReason(CreateProfile(effectiveFrom: Now.AddMinutes(1)), CreateKey(), CryptographicProtectionReason.ProfileNotYetEffective);
    private static bool ExpiredProfileRejected() => ProtectReason(CreateProfile(effectiveFrom: Now.AddHours(-2), expiresAt: Now.AddMinutes(-1)), CreateKey(), CryptographicProtectionReason.ProfileExpired);
    private static bool UnsupportedAlgorithmRejected() => ProtectReason(CreateProfile(algorithm: "CUSTOM-CIPHER"), CreateKey(), CryptographicProtectionReason.ParametersUnsupported);
    private static bool UnsupportedParametersRejected() => ProtectReason(CreateProfile(nonceSize: 16), CreateKey(), CryptographicProtectionReason.ParametersUnsupported);
    private static bool WrongKeyClassRejected() => ProtectReason(CreateProfile(), CreateKey(keyClass: "other-class"), CryptographicProtectionReason.KeyClassMismatch);
    private static bool WrongKeyProfileRejected() => ProtectReason(CreateProfile(), CreateKey(permittedProfileId: "profile:other"), CryptographicProtectionReason.KeyProfileMismatch);
    private static bool FutureKeyRejected() => ProtectReason(CreateProfile(), CreateKey(status: CryptographicKeyStatus.NotYetActive, effectiveFrom: Now.AddMinutes(1)), CryptographicProtectionReason.KeyNotYetActive);
    private static bool ExpiredKeyRejected() => ProtectReason(CreateProfile(), CreateKey(effectiveFrom: Now.AddHours(-2), expiresAt: Now.AddMinutes(-1)), CryptographicProtectionReason.KeyExpired);
    private static bool RevokedKeyRejected() => ProtectReason(CreateProfile(), CreateKey(status: CryptographicKeyStatus.Revoked), CryptographicProtectionReason.KeyRevoked);
    private static bool RetiredKeyRejected() => ProtectReason(CreateProfile(), CreateKey(status: CryptographicKeyStatus.Retired), CryptographicProtectionReason.KeyRetired);
    private static bool DisabledKeyRejected() => ProtectReason(CreateProfile(), CreateKey(status: CryptographicKeyStatus.Disabled), CryptographicProtectionReason.KeyDisabled);
    private static bool UnknownKeyRejected() => ProtectReason(CreateProfile(), CreateKey(status: CryptographicKeyStatus.Unknown), CryptographicProtectionReason.KeyReferenceUnknown);
    private static bool WrongKeyScopeRejected() => ProtectReason(CreateProfile(), CreateKey(recipientScope: "app:other"), CryptographicProtectionReason.KeyScopeMismatch);

    private static bool KeyMaterialUnavailableRejected()
    {
        var protector = new MessageProtector(new NullResolver(), new FixedNonceSource(Nonce(1)));
        var result = protector.Protect(DefaultPlaintext(), CreateProfile(), CreateKey(), CreateContext(), Now);
        return result.Kind == CryptographicDecisionKind.Rejected &&
               result.Reason == CryptographicProtectionReason.KeyMaterialUnavailable &&
               result.Package is null;
    }

    private static bool WrongKeyMaterialAuthenticationRejected()
    {
        var protector = new MessageProtector(new FixedResolver(KeyMaterial(1)), new FixedNonceSource(Nonce(1)));
        var package = protector.Protect(DefaultPlaintext(), CreateProfile(), CreateKey(), CreateContext(), Now).Package!;
        var wrongProtector = new MessageProtector(new FixedResolver(KeyMaterial(2)), new FixedNonceSource(Nonce(2)));
        var result = wrongProtector.Verify(package, CreateProfile(), CreateKey(), CreateContext(), Now);
        return result.Kind == CryptographicDecisionKind.Rejected &&
               result.Reason == CryptographicProtectionReason.AuthenticationFailed &&
               result.Plaintext.IsEmpty;
    }

    private static bool NonceReuseRejected()
    {
        var protector = new MessageProtector(new FixedResolver(KeyMaterial(1)), new FixedNonceSource(Nonce(9)));
        var first = protector.Protect(DefaultPlaintext(), CreateProfile(), CreateKey(), CreateContext(), Now);
        var second = protector.Protect(DefaultPlaintext(), CreateProfile(), CreateKey(), CreateContext(), Now);
        return first.Kind == CryptographicDecisionKind.Protected &&
               second.Kind == CryptographicDecisionKind.Rejected &&
               second.Reason == CryptographicProtectionReason.NonceReuse &&
               second.Package is null;
    }

    private static bool InvalidNonceSizeRejected()
    {
        var protector = new MessageProtector(new FixedResolver(KeyMaterial(1)), new FixedNonceSource(new byte[11]));
        var result = protector.Protect(DefaultPlaintext(), CreateProfile(), CreateKey(), CreateContext(), Now);
        return result.Kind == CryptographicDecisionKind.Rejected && result.Reason == CryptographicProtectionReason.NonceInvalid;
    }

    private static bool FailedVerificationReleasesNoPlaintext()
    {
        var f = Fixture.Create();
        var p = RequirePackage(f);
        var tag = p.AuthenticationTag.ToArray();
        tag[^1] ^= 0x80;
        var result = f.Protector.Verify(ClonePackage(p, tag: tag), f.Profile, f.KeyReference, f.Context, Now);
        return result.Kind == CryptographicDecisionKind.Rejected && result.Plaintext.IsEmpty;
    }

    private static bool ArbitraryBinaryPayloadRoundTrips()
    {
        var payload = new byte[] { 0, 255, 16, 32, 128, 1, 2, 3, 0, 99 };
        var f = Fixture.Create(payload);
        var result = f.Protector.Verify(RequirePackage(f), f.Profile, f.KeyReference, f.Context, Now);
        return result.Kind == CryptographicDecisionKind.Verified && result.Plaintext.Span.SequenceEqual(payload);
    }

    private static bool ApplicationNamesDoNotChangeSemantics()
    {
        var a = Fixture.Create(producer: "app:alpha", recipient: "app:beta", keySeed: 7, nonceSeed: 7);
        var b = Fixture.Create(producer: "app:gamma", recipient: "app:delta", keySeed: 8, nonceSeed: 8);
        return a.Protector.Verify(RequirePackage(a), a.Profile, a.KeyReference, a.Context, Now).Kind == CryptographicDecisionKind.Verified &&
               b.Protector.Verify(RequirePackage(b), b.Profile, b.KeyReference, b.Context, Now).Kind == CryptographicDecisionKind.Verified;
    }

    private static bool ContextDigestIsDeterministic() => CreateContext().ContextDigest == CreateContext().ContextDigest;
    private static bool ProfileIdentityIsDeterministic() => CreateProfile().ProfileIdentity == CreateProfile().ProfileIdentity;
    private static bool KeyReferenceIdentityIsDeterministic() => CreateKey().KeyReferenceIdentity == CreateKey().KeyReferenceIdentity;

    private static bool ProtectionEvidenceContainsNoPlaintext()
    {
        var f = Fixture.Create(Encoding.UTF8.GetBytes("TOP-SECRET-PAYLOAD"));
        var result = f.Protector.Protect(f.Plaintext, f.Profile, f.KeyReference, f.Context, Now);
        return !result.EvidenceIdentity.Contains("TOP-SECRET-PAYLOAD", StringComparison.Ordinal);
    }

    private static bool ProtectionEvidenceContainsNoKeyMaterial()
    {
        var f = Fixture.Create();
        var result = f.Protector.Protect(f.Plaintext, f.Profile, f.KeyReference, f.Context, Now);
        return !result.EvidenceIdentity.Contains(Convert.ToBase64String(f.KeyBytes), StringComparison.Ordinal);
    }

    private static bool VerificationEvidenceContainsNoKeyMaterial()
    {
        var f = Fixture.Create();
        var result = f.Protector.Verify(RequirePackage(f), f.Profile, f.KeyReference, f.Context, Now);
        return !result.EvidenceIdentity.Contains(Convert.ToBase64String(f.KeyBytes), StringComparison.Ordinal);
    }

    private static bool PackageIdentityChangesWithCiphertext()
    {
        var f = Fixture.Create();
        var p = RequirePackage(f);
        var bytes = p.Ciphertext.ToArray();
        bytes[0] ^= 0x01;
        return ClonePackage(p, ciphertext: bytes).PackageIdentity != p.PackageIdentity;
    }

    private static bool ProfileBindingChangesPackageIdentity()
    {
        var f = Fixture.Create();
        var p = RequirePackage(f);
        return ClonePackage(p, profileVersion: "other").PackageIdentity != p.PackageIdentity;
    }

    private static bool KeyVersionBindingEnforced()
    {
        var f = Fixture.Create();
        var result = f.Protector.Verify(RequirePackage(f), f.Profile, CreateKey(version: "2"), f.Context, Now);
        return result.Kind == CryptographicDecisionKind.Rejected && result.Reason == CryptographicProtectionReason.KeyReferenceUnknown;
    }

    private static bool EmptyOptionalPredecessorBindingsAreDeterministic()
    {
        var a = CreateContext(routeDecisionId: string.Empty, deliveryDecisionId: string.Empty, eventIdentity: string.Empty, eventClassification: string.Empty);
        var b = CreateContext(routeDecisionId: string.Empty, deliveryDecisionId: string.Empty, eventIdentity: string.Empty, eventClassification: string.Empty);
        return a.ContextDigest == b.ContextDigest;
    }

    private static bool ProtectReason(CryptographicProtectionProfile profile, CryptographicKeyReference key, string expectedReason)
    {
        var protector = new MessageProtector(new FixedResolver(KeyMaterial(1)), new FixedNonceSource(Nonce(1)));
        var result = protector.Protect(DefaultPlaintext(), profile, key, CreateContext(), Now);
        return result.Kind == CryptographicDecisionKind.Rejected &&
               result.Reason == expectedReason &&
               result.Package is null;
    }

    private static bool RejectsVerification(
        Fixture f,
        ProtectedMessagePackage package,
        ProtectedMessageContext context,
        string expectedReason,
        CryptographicProtectionProfile? profile = null)
    {
        var result = f.Protector.Verify(package, profile ?? f.Profile, f.KeyReference, context, Now);
        return result.Kind == CryptographicDecisionKind.Rejected &&
               result.Reason == expectedReason &&
               result.Plaintext.IsEmpty;
    }

    private static ProtectedMessagePackage RequirePackage(Fixture f)
    {
        var result = f.Protector.Protect(f.Plaintext, f.Profile, f.KeyReference, f.Context, Now);
        if (result.Package is null) throw new InvalidOperationException($"expected_package_but_got:{result.Reason}");
        return result.Package;
    }

    private static ProtectedMessagePackage ClonePackage(
        ProtectedMessagePackage source,
        byte[]? nonce = null,
        byte[]? ciphertext = null,
        byte[]? tag = null,
        string? profileVersion = null)
    {
        var ctor = typeof(ProtectedMessagePackage)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(c => c.GetParameters().Length == 9);

        return (ProtectedMessagePackage)ctor.Invoke(new object[]
        {
            source.ProfileId,
            profileVersion ?? source.ProfileVersion,
            source.KeyReferenceId,
            source.KeyVersion,
            nonce ?? source.Nonce.ToArray(),
            ciphertext ?? source.Ciphertext.ToArray(),
            tag ?? source.AuthenticationTag.ToArray(),
            source.ProtectedContextDigest,
            source.ProtectionEvidenceIdentity
        });
    }

    private static CryptographicProtectionProfile CreateProfile(
        string version = "1",
        string algorithm = MessageProtector.SupportedAlgorithmSuite,
        int nonceSize = MessageProtector.SupportedNonceSizeBytes,
        int tagSize = MessageProtector.SupportedTagSizeBytes,
        CryptographicProfileStatus status = CryptographicProfileStatus.Approved,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? expiresAt = null) => new(
            "profile:wp08:aead",
            version,
            algorithm,
            nonceSize,
            tagSize,
            MessageProtector.SupportedKeyClass,
            status,
            effectiveFrom ?? Now.AddHours(-1),
            expiresAt ?? Now.AddHours(1),
            "evidence:crypto-policy:1");

    private static CryptographicKeyReference CreateKey(
        string version = "1",
        string keyClass = MessageProtector.SupportedKeyClass,
        CryptographicKeyStatus status = CryptographicKeyStatus.Active,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? expiresAt = null,
        string permittedProfileId = "profile:wp08:aead",
        string producer = "app:producer",
        string recipientScope = "app:consumer") => new(
            "keyref:wp08:1",
            keyClass,
            "foundation:security",
            version,
            status,
            effectiveFrom ?? Now.AddHours(-1),
            expiresAt ?? Now.AddHours(1),
            permittedProfileId,
            producer,
            recipientScope,
            "evidence:key-state:1");

    private static ProtectedMessageContext CreateContext(
        string messageDigest = "A69F40E3A77F5B1D87068C5A02F8F2A92B3EF62173D7B1248A7D7C50D478D74F",
        string producer = "app:producer",
        string recipientScope = "app:consumer",
        string classification = "sensitive",
        string routeDecisionId = "route:decision:1",
        string deliveryDecisionId = "delivery:decision:1",
        string eventIdentity = "C9A55A632CEE11A4C7EA75F0EC31D4CC2FE4296F475F5CC2BCA9105FBE74341A",
        string eventClassification = "authoritative-operational",
        string correlationId = "corr:1",
        string causationId = "cause:1") => new(
            "message:1",
            messageDigest,
            producer,
            recipientScope,
            classification,
            "schema:1",
            "1.0",
            routeDecisionId,
            deliveryDecisionId,
            eventIdentity,
            eventClassification,
            correlationId,
            causationId,
            "policy:protection-required",
            Now);

    private static ReadOnlyMemory<byte> DefaultPlaintext() => Encoding.UTF8.GetBytes("payload:opaque:1");
    private static byte[] KeyMaterial(byte seed) => Enumerable.Range(0, 32).Select(i => (byte)(seed + i)).ToArray();
    private static byte[] Nonce(byte seed) => Enumerable.Range(0, 12).Select(i => (byte)(seed + i)).ToArray();
    private static string Sha(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));

    private sealed class FixedResolver : ICryptographicKeyMaterialResolver
    {
        private readonly byte[] _key;
        public FixedResolver(byte[] key) => _key = key;
        public bool TryResolve(CryptographicKeyReference keyReference, out ReadOnlyMemory<byte> keyMaterial)
        {
            keyMaterial = _key;
            return true;
        }
    }

    private sealed class NullResolver : ICryptographicKeyMaterialResolver
    {
        public bool TryResolve(CryptographicKeyReference keyReference, out ReadOnlyMemory<byte> keyMaterial)
        {
            keyMaterial = ReadOnlyMemory<byte>.Empty;
            return false;
        }
    }

    private sealed class FixedNonceSource : IProtectionNonceSource
    {
        private readonly byte[] _nonce;
        public FixedNonceSource(byte[] nonce) => _nonce = nonce;
        public byte[] CreateNonce(int sizeBytes) => _nonce.ToArray();
    }

    private sealed record Fixture(
        MessageProtector Protector,
        CryptographicProtectionProfile Profile,
        CryptographicKeyReference KeyReference,
        ProtectedMessageContext Context,
        ReadOnlyMemory<byte> Plaintext,
        byte[] KeyBytes)
    {
        public static Fixture Create(
            byte[]? payload = null,
            string producer = "app:producer",
            string recipient = "app:consumer",
            byte keySeed = 1,
            byte nonceSeed = 1)
        {
            var key = Program.KeyMaterial(keySeed);
            var profile = Program.CreateProfile();
            var keyReference = Program.CreateKey(producer: producer, recipientScope: recipient);
            var context = Program.CreateContext(producer: producer, recipientScope: recipient);
            var protector = new MessageProtector(new FixedResolver(key), new FixedNonceSource(Program.Nonce(nonceSeed)));
            var plaintext = payload is null ? Program.DefaultPlaintext() : payload.AsMemory();
            return new Fixture(protector, profile, keyReference, context, plaintext, key);
        }
    }
}
