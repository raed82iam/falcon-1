using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Falcon.Stage0B.Candidates;

public interface ITestEntropySource
{
    bool Available { get; }
    void Fill(Span<byte> destination);
    string SourceClass { get; }
}

public sealed record RandomnessRequest(
    string RequestId,
    string Profile,
    string Purpose,
    int Length,
    string Domain,
    string Environment,
    bool CallerSuppliedEntropy,
    CandidateContext Context);

public sealed record RandomnessCandidateResult(
    CandidateDisposition Disposition,
    byte[]? Material,
    string Classification,
    string Reason,
    CandidateEvidence Evidence);

public sealed class RandomnessProviderCandidate(ITestEntropySource source)
    : CandidateProviderBase("CND-RND-001")
{
    private static readonly string[] Profiles = ["FALCON-RANDOM-CANDIDATE-1"];
    private static readonly string[] Purposes = ["test-key", "test-nonce", "test-salt", "test-identifier", "test-challenge"];
    private readonly ConcurrentDictionary<string, byte> _seen = new(StringComparer.Ordinal);

    public RandomnessCandidateResult Produce(RandomnessRequest request)
    {
        var evidenceId = $"rnd-evidence:{request.RequestId}";
        if (!request.Context.IsAuthorized)
        {
            return Rejected(evidenceId, request, "authority_or_context_rejected");
        }

        if (!IsKnownToken(request.Profile, Profiles) ||
            !IsKnownToken(request.Purpose, Purposes) ||
            !StringComparer.Ordinal.Equals(request.Environment, CandidateContext.ApprovedEnvironment) ||
            request.Length is < 16 or > 4096)
        {
            return Rejected(evidenceId, request, "profile_purpose_environment_or_length_rejected");
        }

        if (request.CallerSuppliedEntropy)
        {
            return Rejected(evidenceId, request, "caller_entropy_prohibited");
        }

        if (!source.Available)
        {
            return Rejected(evidenceId, request, "source_unavailable");
        }

        var material = new byte[request.Length];
        source.Fill(material);
        var fingerprint = Convert.ToHexString(SHA256.HashData(material));
        if (!_seen.TryAdd(fingerprint, 0))
        {
            CryptographicOperations.ZeroMemory(material);
            return Rejected(evidenceId, request, "repeated_output_detected");
        }

        var evidence = Succeed(
            evidenceId,
            "randomness.produce",
            ("profile", request.Profile),
            ("purpose", request.Purpose),
            ("length", request.Length.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("domain", request.Domain),
            ("source_class", source.SourceClass),
            ("classification", "TEST_ONLY"),
            ("material_recorded", "false"));
        return new RandomnessCandidateResult(
            CandidateDisposition.Succeeded,
            material,
            "TEST_ONLY",
            "bounded_consumer_handoff",
            evidence);
    }

    private RandomnessCandidateResult Rejected(
        string evidenceId,
        RandomnessRequest request,
        string reason)
    {
        var evidence = Reject(
            evidenceId,
            "randomness.produce",
            reason,
            ("profile", request.Profile),
            ("purpose", request.Purpose),
            ("length", request.Length.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("material_recorded", "false"));
        return new RandomnessCandidateResult(
            CandidateDisposition.Rejected,
            null,
            "TEST_ONLY",
            reason,
            evidence);
    }
}

public enum TestMaterialLifecycle
{
    Candidate,
    ActiveForCandidateUse,
    Restricted,
    Suspended,
    Revoked,
    Expired,
    Destroyed
}

public sealed record OpaqueKeyReference(
    string ReferenceId,
    int Version,
    string Domain,
    string Purpose,
    string Environment,
    TestMaterialLifecycle Lifecycle);

internal sealed class CandidateKeyMaterial(
    byte[] bytes,
    OpaqueKeyReference reference) : IDisposable
{
    public byte[] Bytes { get; } = bytes;
    public OpaqueKeyReference Reference { get; set; } = reference;

    public void Dispose() => CryptographicOperations.ZeroMemory(Bytes);
}

public sealed class CandidateKeyCustody : IDisposable
{
    private readonly ConcurrentDictionary<string, CandidateKeyMaterial> _keys = new(StringComparer.Ordinal);

    public OpaqueKeyReference Generate(
        string referenceId,
        string domain,
        string purpose,
        CandidateContext context,
        RandomnessProviderCandidate randomness)
    {
        if (!context.IsAuthorized)
        {
            throw new CandidateBoundaryException("custody_authority_rejected");
        }

        var random = randomness.Produce(new RandomnessRequest(
            $"generate:{referenceId}",
            "FALCON-RANDOM-CANDIDATE-1",
            "test-key",
            32,
            domain,
            context.Environment,
            false,
            context));
        if (random.Disposition != CandidateDisposition.Succeeded || random.Material is null)
        {
            throw new CandidateBoundaryException("candidate_key_generation_failed");
        }

        var reference = new OpaqueKeyReference(
            referenceId,
            1,
            domain,
            purpose,
            context.Environment,
            TestMaterialLifecycle.ActiveForCandidateUse);
        if (!_keys.TryAdd(referenceId, new CandidateKeyMaterial(random.Material, reference)))
        {
            CryptographicOperations.ZeroMemory(random.Material);
            throw new CandidateBoundaryException("duplicate_key_reference");
        }

        return reference;
    }

    internal CandidateKeyMaterial Resolve(OpaqueKeyReference reference)
    {
        if (!_keys.TryGetValue(reference.ReferenceId, out var material) ||
            material.Reference.Version != reference.Version)
        {
            throw new CandidateBoundaryException("unknown_key_reference");
        }

        return material;
    }

    public OpaqueKeyReference Revoke(OpaqueKeyReference reference)
    {
        var material = Resolve(reference);
        material.Reference = material.Reference with { Lifecycle = TestMaterialLifecycle.Revoked };
        return material.Reference;
    }

    public void Dispose()
    {
        foreach (var material in _keys.Values)
        {
            material.Dispose();
        }

        _keys.Clear();
    }
}

public sealed record CryptoOperationRequest(
    string RequestId,
    string Operation,
    string Profile,
    string Domain,
    string Purpose,
    OpaqueKeyReference KeyReference,
    byte[] Input,
    byte[]? Nonce,
    byte[] CanonicalContext,
    CandidateContext Context);

public sealed record CryptoCandidateResult(
    CandidateDisposition Disposition,
    byte[]? PublicOrProtectedOutput,
    string Reason,
    CandidateEvidence Evidence);

public sealed class CryptographicProviderAdapterCandidate(CandidateKeyCustody custody)
    : CandidateProviderBase("CND-CRY-001")
{
    private static readonly string[] Profiles = ["FALCON-CRYPTO-CANDIDATE-1"];
    private static readonly (string Domain, string Purpose)[] AllowedUses =
    [
        ("falcon/test/encryption", "protect"),
        ("falcon/test/integrity", "authenticate")
    ];

    private readonly ConcurrentDictionary<string, byte> _usedNonces = new(StringComparer.Ordinal);

    public CryptoCandidateResult Execute(CryptoOperationRequest request)
    {
        var evidenceId = $"cry-evidence:{request.RequestId}";
        if (!request.Context.IsAuthorized ||
            !IsKnownToken(request.Profile, Profiles) ||
            !AllowedUses.Contains((request.Domain, request.Purpose)) ||
            !StringComparer.Ordinal.Equals(request.Context.Environment, request.KeyReference.Environment))
        {
            return Rejected(evidenceId, request, "authority_profile_domain_purpose_or_environment_rejected");
        }

        CandidateKeyMaterial material;
        try
        {
            material = custody.Resolve(request.KeyReference);
        }
        catch (CandidateBoundaryException)
        {
            return Rejected(evidenceId, request, "key_reference_rejected");
        }

        if (material.Reference.Lifecycle != TestMaterialLifecycle.ActiveForCandidateUse ||
            !StringComparer.Ordinal.Equals(material.Reference.Domain, request.Domain) ||
            !StringComparer.Ordinal.Equals(material.Reference.Purpose, request.Purpose))
        {
            return Rejected(evidenceId, request, "key_lifecycle_or_use_rejected");
        }

        try
        {
            if (request.Operation is "encrypt" or "decrypt" &&
                !StringComparer.Ordinal.Equals(request.Purpose, "protect") ||
                request.Operation is "sign" or "verify" &&
                !StringComparer.Ordinal.Equals(request.Purpose, "authenticate"))
            {
                return Rejected(evidenceId, request, "operation_purpose_rejected");
            }

            return request.Operation switch
            {
                "encrypt" => Encrypt(evidenceId, request, material.Bytes),
                "decrypt" => Decrypt(evidenceId, request, material.Bytes),
                "sign" => Sign(evidenceId, request, material.Bytes),
                "verify" => Verify(evidenceId, request, material.Bytes),
                _ => Rejected(evidenceId, request, "operation_not_approved")
            };
        }
        catch (CryptographicException)
        {
            return Rejected(evidenceId, request, "cryptographic_operation_failed");
        }
    }

    private CryptoCandidateResult Encrypt(string evidenceId, CryptoOperationRequest request, byte[] key)
    {
        if (request.Nonce is not { Length: 12 })
        {
            return Rejected(evidenceId, request, "nonce_length_rejected");
        }

        var nonceIdentity = $"{request.KeyReference.ReferenceId}:{Convert.ToHexString(SHA256.HashData(request.Nonce))}";
        if (!_usedNonces.TryAdd(nonceIdentity, 0))
        {
            return Rejected(evidenceId, request, "nonce_reuse_rejected");
        }

        var cipher = new byte[request.Input.Length];
        var tag = new byte[16];
        using var aes = new AesGcm(key, tag.Length);
        aes.Encrypt(request.Nonce, request.Input, cipher, tag, request.CanonicalContext);
        var output = new byte[cipher.Length + tag.Length];
        cipher.CopyTo(output, 0);
        tag.CopyTo(output, cipher.Length);
        return Success(evidenceId, request, output);
    }

    private CryptoCandidateResult Decrypt(string evidenceId, CryptoOperationRequest request, byte[] key)
    {
        if (request.Nonce is not { Length: 12 } || request.Input.Length < 16)
        {
            return Rejected(evidenceId, request, "ciphertext_or_nonce_rejected");
        }

        var cipherLength = request.Input.Length - 16;
        var plaintext = new byte[cipherLength];
        using var aes = new AesGcm(key, 16);
        aes.Decrypt(
            request.Nonce,
            request.Input.AsSpan(0, cipherLength),
            request.Input.AsSpan(cipherLength, 16),
            plaintext,
            request.CanonicalContext);
        return Success(evidenceId, request, plaintext);
    }

    private CryptoCandidateResult Sign(string evidenceId, CryptoOperationRequest request, byte[] key)
    {
        var input = Combine(request.CanonicalContext, request.Input);
        var signature = HMACSHA256.HashData(key, input);
        CryptographicOperations.ZeroMemory(input);
        return Success(evidenceId, request, signature);
    }

    private CryptoCandidateResult Verify(string evidenceId, CryptoOperationRequest request, byte[] key)
    {
        if (request.Input.Length < 32)
        {
            return Rejected(evidenceId, request, "verification_input_rejected");
        }

        var messageLength = request.Input.Length - 32;
        var message = request.Input.AsSpan(0, messageLength);
        var supplied = request.Input.AsSpan(messageLength, 32);
        var combined = Combine(request.CanonicalContext, message);
        var expected = HMACSHA256.HashData(key, combined);
        CryptographicOperations.ZeroMemory(combined);
        var valid = CryptographicOperations.FixedTimeEquals(expected, supplied);
        CryptographicOperations.ZeroMemory(expected);
        return valid
            ? Success(evidenceId, request, "VALID"u8.ToArray())
            : Rejected(evidenceId, request, "verification_failed");
    }

    private CryptoCandidateResult Success(
        string evidenceId,
        CryptoOperationRequest request,
        byte[] output)
    {
        var evidence = Succeed(
            evidenceId,
            $"crypto.{request.Operation}",
            ("profile", request.Profile),
            ("domain", request.Domain),
            ("purpose", request.Purpose),
            ("key_reference", request.KeyReference.ReferenceId),
            ("classification", "CANDIDATE"),
            ("protected_material_recorded", "false"));
        return new CryptoCandidateResult(
            CandidateDisposition.Succeeded,
            output,
            "candidate_observation_only",
            evidence);
    }

    private CryptoCandidateResult Rejected(
        string evidenceId,
        CryptoOperationRequest request,
        string reason)
    {
        var evidence = Reject(
            evidenceId,
            $"crypto.{request.Operation}",
            reason,
            ("profile", request.Profile),
            ("domain", request.Domain),
            ("purpose", request.Purpose),
            ("key_reference", request.KeyReference.ReferenceId),
            ("protected_material_recorded", "false"));
        return new CryptoCandidateResult(CandidateDisposition.Rejected, null, reason, evidence);
    }

    private static byte[] Combine(ReadOnlySpan<byte> first, ReadOnlySpan<byte> second)
    {
        var result = new byte[first.Length + second.Length];
        first.CopyTo(result);
        second.CopyTo(result.AsSpan(first.Length));
        return result;
    }
}

public sealed record SecretReference(
    string ReferenceId,
    int Version,
    string Domain,
    string Purpose,
    string Environment,
    TestMaterialLifecycle Lifecycle);

internal sealed class CandidateSecretMaterial(byte[] bytes, SecretReference reference) : IDisposable
{
    public byte[] Bytes { get; } = bytes;
    public SecretReference Reference { get; set; } = reference;
    public void Dispose() => CryptographicOperations.ZeroMemory(Bytes);
}

public sealed record SecretUseRequest(
    string RequestId,
    SecretReference Reference,
    string Operation,
    string Purpose,
    string Domain,
    string Environment,
    CandidateContext Context);

public sealed record SecretUseResult(
    CandidateDisposition Disposition,
    byte[]? BoundedOutput,
    string Reason,
    CandidateEvidence Evidence);

public sealed class SecretProviderCandidate(RandomnessProviderCandidate randomness)
    : CandidateProviderBase("CND-SEC-001"), IDisposable
{
    private readonly ConcurrentDictionary<string, CandidateSecretMaterial> _secrets = new(StringComparer.Ordinal);

    public SecretReference Create(
        string referenceId,
        string domain,
        string purpose,
        CandidateContext context)
    {
        if (!context.IsAuthorized)
        {
            throw new CandidateBoundaryException("secret_creation_authority_rejected");
        }

        var random = randomness.Produce(new RandomnessRequest(
            $"secret:{referenceId}",
            "FALCON-RANDOM-CANDIDATE-1",
            "test-key",
            32,
            domain,
            context.Environment,
            false,
            context));
        if (random.Material is null)
        {
            throw new CandidateBoundaryException("secret_generation_failed");
        }

        var reference = new SecretReference(
            referenceId,
            1,
            domain,
            purpose,
            context.Environment,
            TestMaterialLifecycle.ActiveForCandidateUse);
        if (!_secrets.TryAdd(referenceId, new CandidateSecretMaterial(random.Material, reference)))
        {
            CryptographicOperations.ZeroMemory(random.Material);
            throw new CandidateBoundaryException("duplicate_secret_reference");
        }

        return reference;
    }

    public SecretUseResult Use(SecretUseRequest request, ReadOnlySpan<byte> input)
    {
        var evidenceId = $"sec-evidence:{request.RequestId}";
        if (!request.Context.IsAuthorized ||
            !StringComparer.Ordinal.Equals(request.Operation, "compute-test-integrity") ||
            !StringComparer.Ordinal.Equals(request.Reference.Domain, request.Domain) ||
            !StringComparer.Ordinal.Equals(request.Reference.Purpose, request.Purpose) ||
            !StringComparer.Ordinal.Equals(request.Reference.Environment, request.Environment) ||
            !_secrets.TryGetValue(request.Reference.ReferenceId, out var material) ||
            material.Reference.Version != request.Reference.Version ||
            material.Reference.Lifecycle != TestMaterialLifecycle.ActiveForCandidateUse)
        {
            var rejected = Reject(
                evidenceId,
                "secret.use",
                "secret_authority_scope_or_lifecycle_rejected",
                ("secret_reference", request.Reference.ReferenceId),
                ("secret_recorded", "false"));
            return new SecretUseResult(CandidateDisposition.Rejected, null, rejected.Reason, rejected);
        }

        var output = HMACSHA256.HashData(material.Bytes, input);
        var evidence = Succeed(
            evidenceId,
            "secret.use",
            ("secret_reference", request.Reference.ReferenceId),
            ("version", request.Reference.Version.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("classification", "TEST_ONLY"),
            ("secret_recorded", "false"));
        return new SecretUseResult(CandidateDisposition.Succeeded, output, "bounded_outcome_only", evidence);
    }

    public IReadOnlyList<SecretReference> Enumerate(bool explicitlyAuthorized) =>
        explicitlyAuthorized
            ? _secrets.Values.Select(value => value.Reference).OrderBy(value => value.ReferenceId, StringComparer.Ordinal).ToArray()
            : throw new CandidateBoundaryException("secret_enumeration_denied");

    public SecretReference Rotate(SecretReference reference, CandidateContext context)
    {
        if (!context.IsAuthorized ||
            !_secrets.TryGetValue(reference.ReferenceId, out var current) ||
            current.Reference.Version != reference.Version)
        {
            throw new CandidateBoundaryException("secret_rotation_rejected");
        }

        var random = randomness.Produce(new RandomnessRequest(
            $"rotate:{reference.ReferenceId}:{reference.Version + 1}",
            "FALCON-RANDOM-CANDIDATE-1",
            "test-key",
            32,
            reference.Domain,
            context.Environment,
            false,
            context));
        if (random.Material is null)
        {
            throw new CandidateBoundaryException("secret_rotation_generation_failed");
        }

        CryptographicOperations.ZeroMemory(current.Bytes);
        current.Reference = current.Reference with
        {
            Version = reference.Version + 1,
            Lifecycle = TestMaterialLifecycle.ActiveForCandidateUse
        };
        random.Material.CopyTo(current.Bytes, 0);
        CryptographicOperations.ZeroMemory(random.Material);
        return current.Reference;
    }

    public SecretReference Revoke(SecretReference reference)
    {
        if (!_secrets.TryGetValue(reference.ReferenceId, out var material) ||
            material.Reference.Version != reference.Version)
        {
            throw new CandidateBoundaryException("secret_revocation_rejected");
        }

        material.Reference = material.Reference with { Lifecycle = TestMaterialLifecycle.Revoked };
        return material.Reference;
    }

    public void Dispose()
    {
        foreach (var secret in _secrets.Values)
        {
            secret.Dispose();
        }

        _secrets.Clear();
    }
}
