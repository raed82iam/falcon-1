using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Foundation.Enabling;

public sealed record OpaqueKeyReference(
    string ReferenceId,
    string DomainId,
    string PurposeId,
    uint Version);

internal sealed record KeyEntry(
    byte[] Material,
    string DomainId,
    string PurposeId,
    uint Version,
    bool Revoked);

public sealed class EphemeralFoundationKeyCustody : IDisposable
{
    private readonly object _sync = new();
    private readonly Dictionary<string, KeyEntry> _keys = new(StringComparer.Ordinal);
    private readonly IRandomnessProvider _randomness;
    private bool _disposed;

    public EphemeralFoundationKeyCustody(IRandomnessProvider randomness)
    {
        _randomness = randomness ?? throw new FoundationBoundaryException("randomness_provider_required");
    }

    public OpaqueKeyReference Generate(
        string referenceId,
        string domainId,
        string purposeId,
        FoundationAuthorityContext context)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(referenceId) ||
                string.IsNullOrWhiteSpace(domainId) ||
                string.IsNullOrWhiteSpace(purposeId) ||
                !CryptographicDomainCatalog.IsAllowed(domainId, purposeId))
            {
                throw new FoundationBoundaryException("unknown_crypto_domain_or_purpose");
            }

            if (_keys.ContainsKey(referenceId))
            {
                throw new FoundationBoundaryException("duplicate_key_reference");
            }

            var result = _randomness.Produce(new(
                referenceId + ":material",
                "crypto-key",
                32,
                false,
                context));
            if (result.Disposition != FoundationDisposition.Succeeded ||
                result.Material is null ||
                result.Material.Length != 32)
            {
                ZeroIfPresent(result.Material);
                throw new FoundationBoundaryException("key_material_unavailable");
            }

            var material = result.Material.ToArray();
            CryptographicOperations.ZeroMemory(result.Material);
            _keys.Add(referenceId, new KeyEntry(material, domainId, purposeId, 1, false));
            return new(referenceId, domainId, purposeId, 1);
        }
    }

    public OpaqueKeyReference Rotate(OpaqueKeyReference reference, FoundationAuthorityContext context)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var previous = GetExact(reference, requireActive: true);
            if (previous.Version == uint.MaxValue)
            {
                throw new FoundationBoundaryException("key_version_exhausted");
            }

            var result = _randomness.Produce(new(
                reference.ReferenceId + ":rotation:" + previous.Version.ToString(System.Globalization.CultureInfo.InvariantCulture),
                "crypto-key",
                32,
                false,
                context));
            if (result.Disposition != FoundationDisposition.Succeeded ||
                result.Material is null ||
                result.Material.Length != 32)
            {
                ZeroIfPresent(result.Material);
                throw new FoundationBoundaryException("rotation_material_unavailable");
            }

            var nextMaterial = result.Material.ToArray();
            CryptographicOperations.ZeroMemory(result.Material);
            var next = new KeyEntry(
                nextMaterial,
                previous.DomainId,
                previous.PurposeId,
                checked(previous.Version + 1),
                false);
            _keys[reference.ReferenceId] = next;
            CryptographicOperations.ZeroMemory(previous.Material);
            return new(reference.ReferenceId, next.DomainId, next.PurposeId, next.Version);
        }
    }

    public void Revoke(OpaqueKeyReference reference)
    {
        lock (_sync)
        {
            ThrowIfDisposed();
            var entry = GetExact(reference, requireActive: true);
            CryptographicOperations.ZeroMemory(entry.Material);
            _keys[reference.ReferenceId] = entry with { Revoked = true };
        }
    }

    internal T Use<T>(
        OpaqueKeyReference reference,
        string domainId,
        string purposeId,
        Func<byte[], T> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        lock (_sync)
        {
            ThrowIfDisposed();
            var entry = GetExact(reference, requireActive: true);
            if (!StringComparer.Ordinal.Equals(entry.DomainId, domainId) ||
                !StringComparer.Ordinal.Equals(entry.PurposeId, purposeId))
            {
                throw new FoundationBoundaryException("key_usage_rejected");
            }

            var workingCopy = entry.Material.ToArray();
            try
            {
                return operation(workingCopy);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(workingCopy);
            }
        }
    }

    private KeyEntry GetExact(OpaqueKeyReference? reference, bool requireActive)
    {
        if (reference is null ||
            string.IsNullOrWhiteSpace(reference.ReferenceId) ||
            string.IsNullOrWhiteSpace(reference.DomainId) ||
            string.IsNullOrWhiteSpace(reference.PurposeId) ||
            !_keys.TryGetValue(reference.ReferenceId, out var entry))
        {
            throw new FoundationBoundaryException("unknown_key_reference");
        }

        if (entry.Version != reference.Version ||
            !StringComparer.Ordinal.Equals(entry.DomainId, reference.DomainId) ||
            !StringComparer.Ordinal.Equals(entry.PurposeId, reference.PurposeId))
        {
            throw new FoundationBoundaryException("stale_or_mismatched_key_reference");
        }

        if (requireActive && entry.Revoked)
        {
            throw new FoundationBoundaryException("revoked_key_reference");
        }

        return entry;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            foreach (var entry in _keys.Values)
            {
                CryptographicOperations.ZeroMemory(entry.Material);
            }

            _keys.Clear();
            _disposed = true;
        }
    }

    private static void ZeroIfPresent(byte[]? material)
    {
        if (material is not null)
        {
            CryptographicOperations.ZeroMemory(material);
        }
    }
}

public static class CryptographicDomainCatalog
{
    private static readonly IReadOnlyDictionary<string, string[]> Catalog =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["falcon/foundation/evidence/encryption"] = ["protect"],
            ["falcon/foundation/evidence/integrity"] = ["authenticate"],
            ["falcon/foundation/secret"] = ["protect-secret"]
        };

    public static bool IsAllowed(string? domainId, string? purposeId) =>
        !string.IsNullOrWhiteSpace(domainId) &&
        !string.IsNullOrWhiteSpace(purposeId) &&
        Catalog.TryGetValue(domainId, out var purposes) &&
        purposes.Contains(purposeId, StringComparer.Ordinal);
}

public sealed record ProtectedPayload
{
    private byte[] _nonce = [];
    private byte[] _ciphertext = [];
    private byte[] _tag = [];

    public ProtectedPayload(
        byte[] nonce,
        byte[] ciphertext,
        byte[] tag,
        string domainId,
        string purposeId,
        uint keyVersion)
    {
        Nonce = nonce;
        Ciphertext = ciphertext;
        Tag = tag;
        DomainId = domainId;
        PurposeId = purposeId;
        KeyVersion = keyVersion;
    }

    public byte[] Nonce
    {
        get => _nonce.ToArray();
        init => _nonce = CloneRequired(value, "nonce_required");
    }

    public byte[] Ciphertext
    {
        get => _ciphertext.ToArray();
        init => _ciphertext = CloneRequired(value, "ciphertext_required");
    }

    public byte[] Tag
    {
        get => _tag.ToArray();
        init => _tag = CloneRequired(value, "tag_required");
    }

    public string DomainId { get; init; }
    public string PurposeId { get; init; }
    public uint KeyVersion { get; init; }

    internal ReadOnlySpan<byte> NonceSpan => _nonce;
    internal ReadOnlySpan<byte> CiphertextSpan => _ciphertext;
    internal ReadOnlySpan<byte> TagSpan => _tag;

    private static byte[] CloneRequired(byte[]? value, string reason)
    {
        if (value is null)
        {
            throw new FoundationBoundaryException(reason);
        }

        return value.ToArray();
    }
}

public interface ICryptographicProvider : IFoundationProvider
{
    ProtectedPayload Encrypt(
        string requestId,
        OpaqueKeyReference key,
        byte[] plaintext,
        byte[] canonicalContext,
        FoundationAuthorityContext context);

    byte[] Decrypt(
        string requestId,
        OpaqueKeyReference key,
        ProtectedPayload payload,
        byte[] canonicalContext,
        FoundationAuthorityContext context);

    byte[] Authenticate(
        OpaqueKeyReference key,
        byte[] input,
        FoundationAuthorityContext context);
}

internal readonly record struct NonceIdentity(string ReferenceId, uint Version, string Digest);

public sealed class FoundationCryptographicAdapter(
    EphemeralFoundationKeyCustody custody,
    IRandomnessProvider randomness)
    : FoundationProviderBase("ACT-CRY-001", "FALCON-CRYPTO-BCL-1"), ICryptographicProvider
{
    private readonly EphemeralFoundationKeyCustody _custody = custody ?? throw new FoundationBoundaryException("key_custody_required");
    private readonly IRandomnessProvider _randomness = randomness ?? throw new FoundationBoundaryException("randomness_provider_required");
    private readonly HashSet<NonceIdentity> _nonceDigests = [];
    private readonly object _nonceSync = new();

    public ProtectedPayload Encrypt(
        string requestId,
        OpaqueKeyReference key,
        byte[] plaintext,
        byte[] canonicalContext,
        FoundationAuthorityContext context)
    {
        RequireUsable(context, key);
        if (string.IsNullOrWhiteSpace(requestId) || plaintext is null || canonicalContext is null)
        {
            throw new FoundationBoundaryException("encryption_request_rejected");
        }

        var nonceResult = _randomness.Produce(new(
            requestId + ":nonce",
            "crypto-nonce",
            16,
            false,
            context));
        if (nonceResult.Disposition != FoundationDisposition.Succeeded ||
            nonceResult.Material is null ||
            nonceResult.Material.Length < 12)
        {
            if (nonceResult.Material is not null)
            {
                CryptographicOperations.ZeroMemory(nonceResult.Material);
            }

            throw new FoundationBoundaryException("nonce_unavailable");
        }

        var nonce = nonceResult.Material.AsSpan(0, 12).ToArray();
        CryptographicOperations.ZeroMemory(nonceResult.Material);
        var nonceDigest = Convert.ToHexString(SHA256.HashData(nonce));
        lock (_nonceSync)
        {
            if (!_nonceDigests.Add(new NonceIdentity(key.ReferenceId, key.Version, nonceDigest)))
            {
                CryptographicOperations.ZeroMemory(nonce);
                throw new FoundationBoundaryException("nonce_reuse_rejected");
            }
        }

        return _custody.Use(key, key.DomainId, key.PurposeId, material =>
        {
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[16];
            try
            {
                using var aes = new AesGcm(material, 16);
                aes.Encrypt(nonce, plaintext, ciphertext, tag, canonicalContext);
                return new ProtectedPayload(
                    nonce,
                    ciphertext,
                    tag,
                    key.DomainId,
                    key.PurposeId,
                    key.Version);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(nonce);
                CryptographicOperations.ZeroMemory(ciphertext);
                CryptographicOperations.ZeroMemory(tag);
            }
        });
    }

    public byte[] Decrypt(
        string requestId,
        OpaqueKeyReference key,
        ProtectedPayload payload,
        byte[] canonicalContext,
        FoundationAuthorityContext context)
    {
        RequireUsable(context, key);
        if (string.IsNullOrWhiteSpace(requestId) || payload is null || canonicalContext is null ||
            !StringComparer.Ordinal.Equals(payload.DomainId, key.DomainId) ||
            !StringComparer.Ordinal.Equals(payload.PurposeId, key.PurposeId) ||
            payload.KeyVersion != key.Version ||
            payload.NonceSpan.Length != 12 ||
            payload.TagSpan.Length != 16)
        {
            throw new FoundationBoundaryException("protected_payload_context_rejected");
        }

        return _custody.Use(key, key.DomainId, key.PurposeId, material =>
        {
            var plaintext = new byte[payload.CiphertextSpan.Length];
            using var aes = new AesGcm(material, 16);
            aes.Decrypt(payload.NonceSpan, payload.CiphertextSpan, payload.TagSpan, plaintext, canonicalContext);
            return plaintext;
        });
    }

    public byte[] Authenticate(
        OpaqueKeyReference key,
        byte[] input,
        FoundationAuthorityContext context)
    {
        RequireUsable(context, key);
        if (input is null)
        {
            throw new FoundationBoundaryException("authentication_input_rejected");
        }

        return _custody.Use(
            key,
            "falcon/foundation/evidence/integrity",
            "authenticate",
            material => HMACSHA256.HashData(material, input));
    }

    private void RequireUsable(FoundationAuthorityContext? context, OpaqueKeyReference? key)
    {
        if (!IsUsable(context) ||
            key is null ||
            string.IsNullOrWhiteSpace(key.ReferenceId) ||
            !CryptographicDomainCatalog.IsAllowed(key.DomainId, key.PurposeId))
        {
            throw new FoundationBoundaryException("crypto_context_rejected");
        }
    }
}

public sealed record SecretReference(
    string ReferenceId,
    string DomainId,
    string PurposeId,
    uint Version);

public sealed class FoundationSecretProvider(
    EphemeralFoundationKeyCustody custody)
    : FoundationProviderBase("ACT-SEC-001", "FALCON-SECRET-EPHEMERAL-1")
{
    private readonly object _sync = new();
    private readonly Dictionary<string, OpaqueKeyReference> _secrets = new(StringComparer.Ordinal);
    private readonly EphemeralFoundationKeyCustody _custody = custody ?? throw new FoundationBoundaryException("key_custody_required");

    public SecretReference Create(string referenceId, FoundationAuthorityContext context)
    {
        if (!IsUsable(context) || string.IsNullOrWhiteSpace(referenceId))
        {
            throw new FoundationBoundaryException("secret_context_rejected");
        }

        lock (_sync)
        {
            if (_secrets.ContainsKey(referenceId))
            {
                throw new FoundationBoundaryException("duplicate_secret_reference");
            }

            var key = _custody.Generate(
                referenceId,
                "falcon/foundation/secret",
                "protect-secret",
                context);
            _secrets.Add(referenceId, key);
            return ToSecretReference(key);
        }
    }

    public byte[] UseForBoundedDerivation(
        SecretReference reference,
        byte[] input,
        FoundationAuthorityContext context)
    {
        if (!IsUsable(context) || input is null)
        {
            throw new FoundationBoundaryException("secret_use_rejected");
        }

        lock (_sync)
        {
            var key = GetExact(reference);
            return _custody.Use(key, key.DomainId, key.PurposeId, material =>
                HMACSHA256.HashData(material, input));
        }
    }

    public SecretReference Rotate(SecretReference reference, FoundationAuthorityContext context)
    {
        if (!IsUsable(context))
        {
            throw new FoundationBoundaryException("secret_context_rejected");
        }

        lock (_sync)
        {
            var current = GetExact(reference);
            var rotated = _custody.Rotate(current, context);
            _secrets[reference.ReferenceId] = rotated;
            return ToSecretReference(rotated);
        }
    }

    public void Revoke(SecretReference reference)
    {
        lock (_sync)
        {
            var current = GetExact(reference);
            _custody.Revoke(current);
        }
    }

    private OpaqueKeyReference GetExact(SecretReference? reference)
    {
        if (reference is null ||
            string.IsNullOrWhiteSpace(reference.ReferenceId) ||
            string.IsNullOrWhiteSpace(reference.DomainId) ||
            string.IsNullOrWhiteSpace(reference.PurposeId) ||
            !_secrets.TryGetValue(reference.ReferenceId, out var current))
        {
            throw new FoundationBoundaryException("unknown_secret_reference");
        }

        if (current.Version != reference.Version ||
            !StringComparer.Ordinal.Equals(current.DomainId, reference.DomainId) ||
            !StringComparer.Ordinal.Equals(current.PurposeId, reference.PurposeId))
        {
            throw new FoundationBoundaryException("stale_or_mismatched_secret_reference");
        }

        return current;
    }

    private static SecretReference ToSecretReference(OpaqueKeyReference key) =>
        new(key.ReferenceId, key.DomainId, key.PurposeId, key.Version);
}

public sealed record CertificateValidationResult(
    FoundationDisposition Disposition,
    string Reason,
    string CertificateDigest,
    string Subject);

public sealed class FoundationCertificateIdentityProvider(
    IFoundationTimeProvider timeProvider)
    : FoundationProviderBase("ACT-CID-001", "FALCON-CERT-LOCAL-TRUST-1")
{
    private readonly HashSet<string> _revoked = new(StringComparer.Ordinal);
    private readonly object _sync = new();
    private readonly IFoundationTimeProvider _timeProvider = timeProvider ?? throw new FoundationBoundaryException("time_provider_required");

    public CertificateValidationResult Validate(
        X509Certificate2? certificate,
        string? expectedSubject,
        string? admittedCertificateDigest,
        FoundationAuthorityContext? context)
    {
        if (!IsUsable(context) ||
            certificate is null ||
            string.IsNullOrWhiteSpace(expectedSubject) ||
            !FoundationDigest.IsCanonicalSha256(admittedCertificateDigest))
        {
            return new(FoundationDisposition.Rejected, "certificate_context_rejected", string.Empty, string.Empty);
        }

        var time = _timeProvider.Observe(context);
        var digestBytes = SHA256.HashData(certificate.RawData);
        var admittedDigestBytes = Convert.FromHexString(admittedCertificateDigest!);
        var digest = Convert.ToHexString(digestBytes);
        var digestMatches = CryptographicOperations.FixedTimeEquals(
            digestBytes,
            admittedDigestBytes);
        var subject = certificate.GetNameInfo(X509NameType.SimpleName, forIssuer: false);
        var notBefore = new DateTimeOffset(certificate.NotBefore.ToUniversalTime(), TimeSpan.Zero);
        var notAfter = new DateTimeOffset(certificate.NotAfter.ToUniversalTime(), TimeSpan.Zero);
        var chainValid = false;
        if (time.Disposition == FoundationDisposition.Succeeded &&
            time.ObservedUtc.HasValue)
        {
            using var chain = new X509Chain();
            chain.ChainPolicy.DisableCertificateDownloads = true;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.VerificationTime = time.ObservedUtc.Value.UtcDateTime;
            chain.ChainPolicy.CustomTrustStore.Add(certificate);
            chainValid = chain.Build(certificate);
        }
        bool revoked;
        lock (_sync)
        {
            revoked = _revoked.Contains(digest);
        }

        if (time.Disposition != FoundationDisposition.Succeeded ||
            !time.ObservedUtc.HasValue ||
            !digestMatches ||
            !StringComparer.Ordinal.Equals(subject, expectedSubject) ||
            time.ObservedUtc.Value < notBefore ||
            time.ObservedUtc.Value > notAfter ||
            !chainValid ||
            revoked)
        {
            return new(FoundationDisposition.Rejected, "certificate_validation_rejected", digest, subject);
        }

        return new(FoundationDisposition.Succeeded, "validated_against_admitted_digest_exact_subject_and_certificate_chain", digest, subject);
    }

    public void Revoke(string certificateDigest)
    {
        if (!FoundationDigest.IsCanonicalSha256(certificateDigest))
        {
            throw new FoundationBoundaryException("certificate_digest_rejected");
        }

        lock (_sync)
        {
            _revoked.Add(certificateDigest);
        }
    }
}

public static class FoundationCanonicalContext
{
    public static byte[] Create(
        string environment,
        string domain,
        string purpose,
        string profile,
        uint keyVersion)
    {
        if (string.IsNullOrWhiteSpace(environment) ||
            string.IsNullOrWhiteSpace(domain) ||
            string.IsNullOrWhiteSpace(purpose) ||
            string.IsNullOrWhiteSpace(profile))
        {
            throw new FoundationBoundaryException("canonical_context_rejected");
        }

        var fields = new[]
        {
            ("environment", environment),
            ("domain", domain),
            ("purpose", purpose),
            ("profile", profile),
            ("keyVersion", keyVersion.ToString(System.Globalization.CultureInfo.InvariantCulture))
        };
        using var stream = new MemoryStream();
        foreach (var (name, value) in fields)
        {
            Write(stream, name);
            Write(stream, value);
        }

        return stream.ToArray();
    }

    private static void Write(Stream stream, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        Span<byte> length = stackalloc byte[4];
        System.Buffers.Binary.BinaryPrimitives.WriteInt32BigEndian(length, bytes.Length);
        stream.Write(length);
        stream.Write(bytes);
    }
}
