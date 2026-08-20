using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.MessageProtection;

public enum CryptographicProfileStatus
{
    Approved = 1,
    Deprecated = 2,
    Prohibited = 3,
    Disabled = 4
}

public enum CryptographicKeyStatus
{
    Active = 1,
    NotYetActive = 2,
    Revoked = 3,
    Retired = 4,
    Disabled = 5,
    Unknown = 6
}

public enum CryptographicDecisionKind
{
    Protected = 1,
    Verified = 2,
    Rejected = 3
}

public static class CryptographicProtectionReason
{
    public const string Protected = "CRYPTO_PROTECTED";
    public const string Verified = "CRYPTO_VERIFIED";
    public const string ProfileRequired = "CRYPTO_PROFILE_REQUIRED";
    public const string ProfileUnknown = "CRYPTO_PROFILE_UNKNOWN";
    public const string ProfileProhibited = "CRYPTO_PROFILE_PROHIBITED";
    public const string ProfileDisabled = "CRYPTO_PROFILE_DISABLED";
    public const string ProfileDeprecated = "CRYPTO_PROFILE_DEPRECATED";
    public const string ProfileNotYetEffective = "CRYPTO_PROFILE_NOT_YET_EFFECTIVE";
    public const string ProfileExpired = "CRYPTO_PROFILE_EXPIRED";
    public const string ParametersUnsupported = "CRYPTO_PARAMETERS_UNSUPPORTED";
    public const string KeyReferenceRequired = "CRYPTO_KEY_REFERENCE_REQUIRED";
    public const string KeyReferenceUnknown = "CRYPTO_KEY_REFERENCE_UNKNOWN";
    public const string KeyClassMismatch = "CRYPTO_KEY_CLASS_MISMATCH";
    public const string KeyProfileMismatch = "CRYPTO_KEY_PROFILE_MISMATCH";
    public const string KeyNotYetActive = "CRYPTO_KEY_NOT_YET_ACTIVE";
    public const string KeyExpired = "CRYPTO_KEY_EXPIRED";
    public const string KeyRevoked = "CRYPTO_KEY_REVOKED";
    public const string KeyRetired = "CRYPTO_KEY_RETIRED";
    public const string KeyDisabled = "CRYPTO_KEY_DISABLED";
    public const string KeyScopeMismatch = "CRYPTO_KEY_SCOPE_MISMATCH";
    public const string ContextInvalid = "CRYPTO_CONTEXT_INVALID";
    public const string ContextMismatch = "CRYPTO_CONTEXT_MISMATCH";
    public const string RecipientMismatch = "CRYPTO_RECIPIENT_MISMATCH";
    public const string ClassificationMismatch = "CRYPTO_CLASSIFICATION_MISMATCH";
    public const string PredecessorBindingMismatch = "CRYPTO_PREDECESSOR_BINDING_MISMATCH";
    public const string KeyMaterialUnavailable = "CRYPTO_KEY_MATERIAL_UNAVAILABLE";
    public const string NonceInvalid = "CRYPTO_NONCE_INVALID";
    public const string NonceReuse = "CRYPTO_NONCE_REUSE";
    public const string PackageMalformed = "CRYPTO_PACKAGE_MALFORMED";
    public const string PackageIdentityMismatch = "CRYPTO_PACKAGE_IDENTITY_MISMATCH";
    public const string AuthenticationFailed = "CRYPTO_AUTHENTICATION_FAILED";
    public const string ProtectionRequired = "CRYPTO_PROTECTION_REQUIRED";
    public const string ProviderFailure = "CRYPTO_PROVIDER_FAILURE";
}

public sealed record CryptographicProtectionProfile
{
    public CryptographicProtectionProfile(
        string profileId,
        string profileVersion,
        string algorithmSuite,
        int nonceSizeBytes,
        int tagSizeBytes,
        string allowedKeyClass,
        CryptographicProfileStatus status,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? expiresAt,
        string policyEvidenceIdentity)
    {
        ProfileId = ProtectionRules.RequireIdentifier(profileId, nameof(profileId));
        ProfileVersion = ProtectionRules.RequireIdentifier(profileVersion, nameof(profileVersion));
        AlgorithmSuite = ProtectionRules.RequireIdentifier(algorithmSuite, nameof(algorithmSuite));
        if (nonceSizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(nonceSizeBytes));
        if (tagSizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(tagSizeBytes));
        NonceSizeBytes = nonceSizeBytes;
        TagSizeBytes = tagSizeBytes;
        AllowedKeyClass = ProtectionRules.RequireIdentifier(allowedKeyClass, nameof(allowedKeyClass));
        Status = ProtectionRules.RequireDefined(status, nameof(status));
        EffectiveFrom = ProtectionRules.RequireUtc(effectiveFrom, nameof(effectiveFrom));
        if (expiresAt.HasValue)
        {
            ExpiresAt = ProtectionRules.RequireUtc(expiresAt.Value, nameof(expiresAt));
            if (ExpiresAt <= EffectiveFrom) throw new ArgumentException("profile_expiry_must_follow_effective_from", nameof(expiresAt));
        }

        PolicyEvidenceIdentity = ProtectionRules.RequireIdentifier(policyEvidenceIdentity, nameof(policyEvidenceIdentity));
        ProfileIdentity = ProtectionCanonicalization.Hash(
            ("profile_id", ProfileId),
            ("profile_version", ProfileVersion),
            ("algorithm", AlgorithmSuite),
            ("nonce_size", NonceSizeBytes.ToString(CultureInfo.InvariantCulture)),
            ("tag_size", TagSizeBytes.ToString(CultureInfo.InvariantCulture)),
            ("key_class", AllowedKeyClass),
            ("status", ((int)Status).ToString(CultureInfo.InvariantCulture)),
            ("effective_from", EffectiveFrom.ToString("O", CultureInfo.InvariantCulture)),
            ("expires_at", ExpiresAt?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty),
            ("policy_evidence", PolicyEvidenceIdentity));
    }

    public string ProfileId { get; }
    public string ProfileVersion { get; }
    public string AlgorithmSuite { get; }
    public int NonceSizeBytes { get; }
    public int TagSizeBytes { get; }
    public string AllowedKeyClass { get; }
    public CryptographicProfileStatus Status { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset? ExpiresAt { get; }
    public string PolicyEvidenceIdentity { get; }
    public string ProfileIdentity { get; }
}

public sealed record CryptographicKeyReference
{
    public CryptographicKeyReference(
        string keyReferenceId,
        string keyClass,
        string ownerIdentity,
        string keyVersion,
        CryptographicKeyStatus status,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? expiresAt,
        string permittedProfileId,
        string permittedProducerIdentity,
        string permittedRecipientScope,
        string evidenceIdentity)
    {
        KeyReferenceId = ProtectionRules.RequireIdentifier(keyReferenceId, nameof(keyReferenceId));
        KeyClass = ProtectionRules.RequireIdentifier(keyClass, nameof(keyClass));
        OwnerIdentity = ProtectionRules.RequireIdentifier(ownerIdentity, nameof(ownerIdentity));
        KeyVersion = ProtectionRules.RequireIdentifier(keyVersion, nameof(keyVersion));
        Status = ProtectionRules.RequireDefined(status, nameof(status));
        EffectiveFrom = ProtectionRules.RequireUtc(effectiveFrom, nameof(effectiveFrom));
        if (expiresAt.HasValue)
        {
            ExpiresAt = ProtectionRules.RequireUtc(expiresAt.Value, nameof(expiresAt));
            if (ExpiresAt <= EffectiveFrom) throw new ArgumentException("key_expiry_must_follow_effective_from", nameof(expiresAt));
        }

        PermittedProfileId = ProtectionRules.RequireIdentifier(permittedProfileId, nameof(permittedProfileId));
        PermittedProducerIdentity = ProtectionRules.RequireIdentifier(permittedProducerIdentity, nameof(permittedProducerIdentity));
        PermittedRecipientScope = ProtectionRules.RequireIdentifier(permittedRecipientScope, nameof(permittedRecipientScope));
        EvidenceIdentity = ProtectionRules.RequireIdentifier(evidenceIdentity, nameof(evidenceIdentity));
        KeyReferenceIdentity = ProtectionCanonicalization.Hash(
            ("key_reference_id", KeyReferenceId),
            ("key_class", KeyClass),
            ("owner_identity", OwnerIdentity),
            ("key_version", KeyVersion),
            ("status", ((int)Status).ToString(CultureInfo.InvariantCulture)),
            ("effective_from", EffectiveFrom.ToString("O", CultureInfo.InvariantCulture)),
            ("expires_at", ExpiresAt?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty),
            ("permitted_profile_id", PermittedProfileId),
            ("permitted_producer", PermittedProducerIdentity),
            ("permitted_recipient_scope", PermittedRecipientScope),
            ("evidence", EvidenceIdentity));
    }

    public string KeyReferenceId { get; }
    public string KeyClass { get; }
    public string OwnerIdentity { get; }
    public string KeyVersion { get; }
    public CryptographicKeyStatus Status { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset? ExpiresAt { get; }
    public string PermittedProfileId { get; }
    public string PermittedProducerIdentity { get; }
    public string PermittedRecipientScope { get; }
    public string EvidenceIdentity { get; }
    public string KeyReferenceIdentity { get; }
}

public sealed record ProtectedMessageContext
{
    public ProtectedMessageContext(
        string canonicalMessageId,
        string canonicalMessageDigest,
        string producerIdentity,
        string recipientScope,
        string informationClassification,
        string schemaId,
        string schemaVersion,
        string routeDecisionId,
        string deliveryDecisionId,
        string eventIdentity,
        string eventClassification,
        string correlationId,
        string causationId,
        string protectionPolicyIdentity,
        DateTimeOffset observedAt)
    {
        CanonicalMessageId = ProtectionRules.RequireIdentifier(canonicalMessageId, nameof(canonicalMessageId));
        CanonicalMessageDigest = ProtectionRules.RequireSha256(canonicalMessageDigest, nameof(canonicalMessageDigest));
        ProducerIdentity = ProtectionRules.RequireIdentifier(producerIdentity, nameof(producerIdentity));
        RecipientScope = ProtectionRules.RequireIdentifier(recipientScope, nameof(recipientScope));
        InformationClassification = ProtectionRules.RequireIdentifier(informationClassification, nameof(informationClassification));
        SchemaId = ProtectionRules.RequireIdentifier(schemaId, nameof(schemaId));
        SchemaVersion = ProtectionRules.RequireIdentifier(schemaVersion, nameof(schemaVersion));
        RouteDecisionId = ProtectionRules.RequireOptionalIdentifier(routeDecisionId, nameof(routeDecisionId));
        DeliveryDecisionId = ProtectionRules.RequireOptionalIdentifier(deliveryDecisionId, nameof(deliveryDecisionId));
        EventIdentity = ProtectionRules.RequireOptionalIdentifier(eventIdentity, nameof(eventIdentity));
        EventClassification = ProtectionRules.RequireOptionalIdentifier(eventClassification, nameof(eventClassification));
        CorrelationId = ProtectionRules.RequireIdentifier(correlationId, nameof(correlationId));
        CausationId = ProtectionRules.RequireIdentifier(causationId, nameof(causationId));
        ProtectionPolicyIdentity = ProtectionRules.RequireIdentifier(protectionPolicyIdentity, nameof(protectionPolicyIdentity));
        ObservedAt = ProtectionRules.RequireUtc(observedAt, nameof(observedAt));
        ContextDigest = ProtectionCanonicalization.Hash(
            ("message_id", CanonicalMessageId),
            ("message_digest", CanonicalMessageDigest),
            ("producer", ProducerIdentity),
            ("recipient_scope", RecipientScope),
            ("classification", InformationClassification),
            ("schema_id", SchemaId),
            ("schema_version", SchemaVersion),
            ("route_decision_id", RouteDecisionId),
            ("delivery_decision_id", DeliveryDecisionId),
            ("event_identity", EventIdentity),
            ("event_classification", EventClassification),
            ("correlation_id", CorrelationId),
            ("causation_id", CausationId),
            ("protection_policy", ProtectionPolicyIdentity),
            ("observed_at", ObservedAt.ToString("O", CultureInfo.InvariantCulture)));
    }

    public string CanonicalMessageId { get; }
    public string CanonicalMessageDigest { get; }
    public string ProducerIdentity { get; }
    public string RecipientScope { get; }
    public string InformationClassification { get; }
    public string SchemaId { get; }
    public string SchemaVersion { get; }
    public string RouteDecisionId { get; }
    public string DeliveryDecisionId { get; }
    public string EventIdentity { get; }
    public string EventClassification { get; }
    public string CorrelationId { get; }
    public string CausationId { get; }
    public string ProtectionPolicyIdentity { get; }
    public DateTimeOffset ObservedAt { get; }
    public string ContextDigest { get; }

    internal byte[] ToAuthenticatedData() => ProtectionCanonicalization.Serialize(
        ("message_id", CanonicalMessageId),
        ("message_digest", CanonicalMessageDigest),
        ("producer", ProducerIdentity),
        ("recipient_scope", RecipientScope),
        ("classification", InformationClassification),
        ("schema_id", SchemaId),
        ("schema_version", SchemaVersion),
        ("route_decision_id", RouteDecisionId),
        ("delivery_decision_id", DeliveryDecisionId),
        ("event_identity", EventIdentity),
        ("event_classification", EventClassification),
        ("correlation_id", CorrelationId),
        ("causation_id", CausationId),
        ("protection_policy", ProtectionPolicyIdentity),
        ("observed_at", ObservedAt.ToString("O", CultureInfo.InvariantCulture)));
}

public sealed record ProtectedMessagePackage
{
    internal ProtectedMessagePackage(
        string profileId,
        string profileVersion,
        string keyReferenceId,
        string keyVersion,
        byte[] nonce,
        byte[] ciphertext,
        byte[] authenticationTag,
        string protectedContextDigest,
        string protectionEvidenceIdentity)
    {
        ProfileId = ProtectionRules.RequireIdentifier(profileId, nameof(profileId));
        ProfileVersion = ProtectionRules.RequireIdentifier(profileVersion, nameof(profileVersion));
        KeyReferenceId = ProtectionRules.RequireIdentifier(keyReferenceId, nameof(keyReferenceId));
        KeyVersion = ProtectionRules.RequireIdentifier(keyVersion, nameof(keyVersion));
        Nonce = (nonce ?? throw new ArgumentNullException(nameof(nonce))).AsMemory();
        Ciphertext = (ciphertext ?? throw new ArgumentNullException(nameof(ciphertext))).AsMemory();
        AuthenticationTag = (authenticationTag ?? throw new ArgumentNullException(nameof(authenticationTag))).AsMemory();
        ProtectedContextDigest = ProtectionRules.RequireSha256(protectedContextDigest, nameof(protectedContextDigest));
        ProtectionEvidenceIdentity = ProtectionRules.RequireSha256(protectionEvidenceIdentity, nameof(protectionEvidenceIdentity));
        PackageIdentity = ComputePackageIdentity(this);
    }

    public string ProfileId { get; }
    public string ProfileVersion { get; }
    public string KeyReferenceId { get; }
    public string KeyVersion { get; }
    public ReadOnlyMemory<byte> Nonce { get; }
    public ReadOnlyMemory<byte> Ciphertext { get; }
    public ReadOnlyMemory<byte> AuthenticationTag { get; }
    public string ProtectedContextDigest { get; }
    public string ProtectionEvidenceIdentity { get; }
    public string PackageIdentity { get; }

    internal static string ComputePackageIdentity(ProtectedMessagePackage package) => ProtectionCanonicalization.Hash(
        ("profile_id", package.ProfileId),
        ("profile_version", package.ProfileVersion),
        ("key_reference_id", package.KeyReferenceId),
        ("key_version", package.KeyVersion),
        ("nonce", Convert.ToBase64String(package.Nonce.Span)),
        ("ciphertext", Convert.ToBase64String(package.Ciphertext.Span)),
        ("tag", Convert.ToBase64String(package.AuthenticationTag.Span)),
        ("context_digest", package.ProtectedContextDigest),
        ("evidence", package.ProtectionEvidenceIdentity));
}

public sealed record CryptographicProtectionDecision
{
    internal CryptographicProtectionDecision(CryptographicDecisionKind kind, string reason, ProtectedMessagePackage? package, string evidenceIdentity)
    {
        Kind = kind;
        Reason = ProtectionRules.RequireIdentifier(reason, nameof(reason));
        Package = package;
        EvidenceIdentity = ProtectionRules.RequireSha256(evidenceIdentity, nameof(evidenceIdentity));
    }

    public CryptographicDecisionKind Kind { get; }
    public string Reason { get; }
    public ProtectedMessagePackage? Package { get; }
    public string EvidenceIdentity { get; }
}

public sealed record CryptographicVerificationDecision
{
    internal CryptographicVerificationDecision(CryptographicDecisionKind kind, string reason, byte[]? plaintext, string evidenceIdentity)
    {
        Kind = kind;
        Reason = ProtectionRules.RequireIdentifier(reason, nameof(reason));
        Plaintext = plaintext is null ? ReadOnlyMemory<byte>.Empty : plaintext.AsMemory();
        EvidenceIdentity = ProtectionRules.RequireSha256(evidenceIdentity, nameof(evidenceIdentity));
    }

    public CryptographicDecisionKind Kind { get; }
    public string Reason { get; }
    public ReadOnlyMemory<byte> Plaintext { get; }
    public string EvidenceIdentity { get; }
}

public interface ICryptographicKeyMaterialResolver
{
    bool TryResolve(CryptographicKeyReference keyReference, out ReadOnlyMemory<byte> keyMaterial);
}

public interface IProtectionNonceSource
{
    byte[] CreateNonce(int sizeBytes);
}

public sealed class RandomProtectionNonceSource : IProtectionNonceSource
{
    public byte[] CreateNonce(int sizeBytes)
    {
        if (sizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(sizeBytes));
        return RandomNumberGenerator.GetBytes(sizeBytes);
    }
}

public sealed class MessageProtector
{
    public const string SupportedAlgorithmSuite = "AES-256-GCM";
    public const int SupportedNonceSizeBytes = 12;
    public const int SupportedTagSizeBytes = 16;
    public const string SupportedKeyClass = "message-aead-256";

    private readonly ICryptographicKeyMaterialResolver _keyResolver;
    private readonly IProtectionNonceSource _nonceSource;
    private readonly HashSet<string> _usedNonces = new(StringComparer.Ordinal);
    private readonly object _nonceGate = new();

    public MessageProtector(ICryptographicKeyMaterialResolver keyResolver, IProtectionNonceSource? nonceSource = null)
    {
        _keyResolver = keyResolver ?? throw new ArgumentNullException(nameof(keyResolver));
        _nonceSource = nonceSource ?? new RandomProtectionNonceSource();
    }

    public CryptographicProtectionDecision Protect(
        ReadOnlyMemory<byte> plaintext,
        CryptographicProtectionProfile profile,
        CryptographicKeyReference keyReference,
        ProtectedMessageContext context,
        DateTimeOffset observedAt)
    {
        var validation = ValidateInputs(profile, keyReference, context, observedAt);
        if (validation is not null) return RejectProtection(validation, profile, keyReference, context);

        if (!TryResolveKey(keyReference, out var keyMaterial, out var keyFailure))
            return RejectProtection(keyFailure, profile, keyReference, context);

        byte[] nonce;
        try
        {
            nonce = _nonceSource.CreateNonce(profile.NonceSizeBytes);
        }
        catch (Exception)
        {
            return RejectProtection(CryptographicProtectionReason.ProviderFailure, profile, keyReference, context);
        }

        if (nonce is null || nonce.Length != profile.NonceSizeBytes)
            return RejectProtection(CryptographicProtectionReason.NonceInvalid, profile, keyReference, context);

        var nonceIdentity = ProtectionCanonicalization.Hash(
            ("key_reference", keyReference.KeyReferenceIdentity),
            ("nonce", Convert.ToBase64String(nonce)));

        lock (_nonceGate)
        {
            if (!_usedNonces.Add(nonceIdentity))
                return RejectProtection(CryptographicProtectionReason.NonceReuse, profile, keyReference, context);
        }

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[profile.TagSizeBytes];
        var aad = context.ToAuthenticatedData();

        try
        {
            using var aes = new AesGcm(keyMaterial.Span, profile.TagSizeBytes);
            aes.Encrypt(nonce, plaintext.Span, ciphertext, tag, aad);
        }
        catch (CryptographicException)
        {
            return RejectProtection(CryptographicProtectionReason.ProviderFailure, profile, keyReference, context);
        }

        var protectionEvidence = ProtectionCanonicalization.Hash(
            ("kind", "protected"),
            ("profile_identity", profile.ProfileIdentity),
            ("key_reference_identity", keyReference.KeyReferenceIdentity),
            ("context_digest", context.ContextDigest),
            ("nonce", Convert.ToBase64String(nonce)),
            ("ciphertext_digest", ProtectionCanonicalization.HashBytes(ciphertext)),
            ("tag_digest", ProtectionCanonicalization.HashBytes(tag)));

        var package = new ProtectedMessagePackage(
            profile.ProfileId,
            profile.ProfileVersion,
            keyReference.KeyReferenceId,
            keyReference.KeyVersion,
            nonce,
            ciphertext,
            tag,
            context.ContextDigest,
            protectionEvidence);

        return new CryptographicProtectionDecision(
            CryptographicDecisionKind.Protected,
            CryptographicProtectionReason.Protected,
            package,
            ProtectionCanonicalization.Hash(("decision", "protected"), ("package_identity", package.PackageIdentity)));
    }

    public CryptographicVerificationDecision Verify(
        ProtectedMessagePackage package,
        CryptographicProtectionProfile profile,
        CryptographicKeyReference keyReference,
        ProtectedMessageContext expectedContext,
        DateTimeOffset observedAt)
    {
        if (package is null) throw new ArgumentNullException(nameof(package));

        var validation = ValidateInputs(profile, keyReference, expectedContext, observedAt);
        if (validation is not null) return RejectVerification(validation, package, expectedContext);

        if (!string.Equals(package.ProfileId, profile.ProfileId, StringComparison.Ordinal) ||
            !string.Equals(package.ProfileVersion, profile.ProfileVersion, StringComparison.Ordinal))
            return RejectVerification(CryptographicProtectionReason.ProfileUnknown, package, expectedContext);

        if (!string.Equals(package.KeyReferenceId, keyReference.KeyReferenceId, StringComparison.Ordinal) ||
            !string.Equals(package.KeyVersion, keyReference.KeyVersion, StringComparison.Ordinal))
            return RejectVerification(CryptographicProtectionReason.KeyReferenceUnknown, package, expectedContext);

        if (!string.Equals(package.ProtectedContextDigest, expectedContext.ContextDigest, StringComparison.Ordinal))
            return RejectVerification(CryptographicProtectionReason.ContextMismatch, package, expectedContext);

        if (package.Nonce.Length != profile.NonceSizeBytes || package.AuthenticationTag.Length != profile.TagSizeBytes)
            return RejectVerification(CryptographicProtectionReason.PackageMalformed, package, expectedContext);

        if (!string.Equals(ProtectedMessagePackage.ComputePackageIdentity(package), package.PackageIdentity, StringComparison.Ordinal))
            return RejectVerification(CryptographicProtectionReason.PackageIdentityMismatch, package, expectedContext);

        if (!TryResolveKey(keyReference, out var keyMaterial, out var keyFailure))
            return RejectVerification(keyFailure, package, expectedContext);

        var plaintext = new byte[package.Ciphertext.Length];
        var aad = expectedContext.ToAuthenticatedData();

        try
        {
            using var aes = new AesGcm(keyMaterial.Span, profile.TagSizeBytes);
            aes.Decrypt(package.Nonce.Span, package.Ciphertext.Span, package.AuthenticationTag.Span, plaintext, aad);
        }
        catch (CryptographicException)
        {
            CryptographicOperations.ZeroMemory(plaintext);
            return RejectVerification(CryptographicProtectionReason.AuthenticationFailed, package, expectedContext);
        }

        return new CryptographicVerificationDecision(
            CryptographicDecisionKind.Verified,
            CryptographicProtectionReason.Verified,
            plaintext,
            ProtectionCanonicalization.Hash(
                ("kind", "verified"),
                ("package_identity", package.PackageIdentity),
                ("profile_identity", profile.ProfileIdentity),
                ("key_reference_identity", keyReference.KeyReferenceIdentity),
                ("context_digest", expectedContext.ContextDigest)));
    }

    private bool TryResolveKey(CryptographicKeyReference keyReference, out ReadOnlyMemory<byte> keyMaterial, out string failureReason)
    {
        try
        {
            if (!_keyResolver.TryResolve(keyReference, out keyMaterial))
            {
                failureReason = CryptographicProtectionReason.KeyMaterialUnavailable;
                return false;
            }
        }
        catch (Exception)
        {
            keyMaterial = ReadOnlyMemory<byte>.Empty;
            failureReason = CryptographicProtectionReason.ProviderFailure;
            return false;
        }

        if (keyMaterial.Length != 32)
        {
            keyMaterial = ReadOnlyMemory<byte>.Empty;
            failureReason = CryptographicProtectionReason.ParametersUnsupported;
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    private static string? ValidateInputs(
        CryptographicProtectionProfile? profile,
        CryptographicKeyReference? keyReference,
        ProtectedMessageContext? context,
        DateTimeOffset observedAt)
    {
        if (profile is null) return CryptographicProtectionReason.ProfileRequired;
        if (keyReference is null) return CryptographicProtectionReason.KeyReferenceRequired;
        if (context is null) return CryptographicProtectionReason.ContextInvalid;
        ProtectionRules.RequireUtc(observedAt, nameof(observedAt));

        if (!string.Equals(profile.AlgorithmSuite, SupportedAlgorithmSuite, StringComparison.Ordinal) ||
            profile.NonceSizeBytes != SupportedNonceSizeBytes ||
            profile.TagSizeBytes != SupportedTagSizeBytes)
            return CryptographicProtectionReason.ParametersUnsupported;

        if (profile.Status == CryptographicProfileStatus.Prohibited) return CryptographicProtectionReason.ProfileProhibited;
        if (profile.Status == CryptographicProfileStatus.Disabled) return CryptographicProtectionReason.ProfileDisabled;
        if (profile.Status == CryptographicProfileStatus.Deprecated) return CryptographicProtectionReason.ProfileDeprecated;
        if (observedAt < profile.EffectiveFrom) return CryptographicProtectionReason.ProfileNotYetEffective;
        if (profile.ExpiresAt.HasValue && observedAt >= profile.ExpiresAt.Value) return CryptographicProtectionReason.ProfileExpired;

        if (!string.Equals(keyReference.KeyClass, profile.AllowedKeyClass, StringComparison.Ordinal) ||
            !string.Equals(keyReference.KeyClass, SupportedKeyClass, StringComparison.Ordinal))
            return CryptographicProtectionReason.KeyClassMismatch;

        if (!string.Equals(keyReference.PermittedProfileId, profile.ProfileId, StringComparison.Ordinal))
            return CryptographicProtectionReason.KeyProfileMismatch;

        switch (keyReference.Status)
        {
            case CryptographicKeyStatus.NotYetActive: return CryptographicProtectionReason.KeyNotYetActive;
            case CryptographicKeyStatus.Revoked: return CryptographicProtectionReason.KeyRevoked;
            case CryptographicKeyStatus.Retired: return CryptographicProtectionReason.KeyRetired;
            case CryptographicKeyStatus.Disabled: return CryptographicProtectionReason.KeyDisabled;
            case CryptographicKeyStatus.Unknown: return CryptographicProtectionReason.KeyReferenceUnknown;
        }

        if (observedAt < keyReference.EffectiveFrom) return CryptographicProtectionReason.KeyNotYetActive;
        if (keyReference.ExpiresAt.HasValue && observedAt >= keyReference.ExpiresAt.Value) return CryptographicProtectionReason.KeyExpired;
        if (!string.Equals(keyReference.PermittedProducerIdentity, context.ProducerIdentity, StringComparison.Ordinal))
            return CryptographicProtectionReason.KeyScopeMismatch;
        if (!string.Equals(keyReference.PermittedRecipientScope, context.RecipientScope, StringComparison.Ordinal))
            return CryptographicProtectionReason.KeyScopeMismatch;

        return null;
    }

    private static CryptographicProtectionDecision RejectProtection(
        string reason,
        CryptographicProtectionProfile? profile,
        CryptographicKeyReference? keyReference,
        ProtectedMessageContext? context)
    {
        return new CryptographicProtectionDecision(
            CryptographicDecisionKind.Rejected,
            reason,
            null,
            ProtectionCanonicalization.Hash(
                ("decision", "rejected"),
                ("reason", reason),
                ("profile", profile?.ProfileIdentity ?? string.Empty),
                ("key", keyReference?.KeyReferenceIdentity ?? string.Empty),
                ("context", context?.ContextDigest ?? string.Empty)));
    }

    private static CryptographicVerificationDecision RejectVerification(
        string reason,
        ProtectedMessagePackage package,
        ProtectedMessageContext context)
    {
        return new CryptographicVerificationDecision(
            CryptographicDecisionKind.Rejected,
            reason,
            null,
            ProtectionCanonicalization.Hash(
                ("decision", "rejected"),
                ("reason", reason),
                ("package", package.PackageIdentity),
                ("context", context.ContextDigest)));
    }
}

internal static class ProtectionRules
{
    public static string RequireIdentifier(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("identifier_required", parameterName);
        var normalized = value.Trim();
        if (!string.Equals(value, normalized, StringComparison.Ordinal)) throw new ArgumentException("identifier_must_be_canonical", parameterName);
        return normalized;
    }

    public static string RequireOptionalIdentifier(string value, string parameterName)
    {
        if (value is null) throw new ArgumentNullException(parameterName);
        if (value.Length == 0) return string.Empty;
        return RequireIdentifier(value, parameterName);
    }

    public static string RequireSha256(string value, string parameterName)
    {
        var canonical = RequireIdentifier(value, parameterName);
        if (canonical.Length != 64) throw new ArgumentException("sha256_length_invalid", parameterName);
        foreach (var c in canonical)
            if (!Uri.IsHexDigit(c)) throw new ArgumentException("sha256_hex_invalid", parameterName);
        return canonical.ToUpperInvariant();
    }

    public static T RequireDefined<T>(T value, string parameterName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(parameterName);
        return value;
    }

    public static DateTimeOffset RequireUtc(DateTimeOffset value, string parameterName)
    {
        if (value.Offset != TimeSpan.Zero) throw new ArgumentException("utc_required", parameterName);
        return value;
    }
}

internal static class ProtectionCanonicalization
{
    public static string Hash(params (string Name, string Value)[] fields) =>
        Convert.ToHexString(SHA256.HashData(Serialize(fields)));

    public static string HashBytes(ReadOnlySpan<byte> bytes) =>
        Convert.ToHexString(SHA256.HashData(bytes));

    public static byte[] Serialize(params (string Name, string Value)[] fields)
    {
        using var stream = new System.IO.MemoryStream();
        foreach (var (name, value) in fields)
        {
            WriteField(stream, name);
            WriteField(stream, value);
        }
        return stream.ToArray();
    }

    private static void WriteField(System.IO.Stream stream, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
        var length = BitConverter.GetBytes(bytes.Length);
        if (BitConverter.IsLittleEndian) Array.Reverse(length);
        stream.Write(length, 0, length.Length);
        stream.Write(bytes, 0, bytes.Length);
    }
}
