using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Falcon.Stage0B.Candidates;

public sealed record CertificateReference(
    string ReferenceId,
    string ExpectedSubject,
    string IssuerScope,
    string Environment,
    string Purpose,
    string Usage,
    string Profile,
    string TrustAnchorSet,
    int Version);

public enum CertificateValidationDisposition
{
    Valid,
    Invalid,
    Indeterminate,
    Rejected
}

public sealed record CertificateValidationRequest(
    string RequestId,
    CertificateReference Reference,
    string ExpectedSubject,
    string Purpose,
    string Environment,
    string Usage,
    TimeCandidateObservation TimeObservation,
    CandidateContext Context);

public sealed record CertificateValidationResult(
    CertificateValidationDisposition Disposition,
    string CertificateDigest,
    string ChainDisposition,
    string RevocationDisposition,
    string Reason,
    CandidateEvidence Evidence);

internal sealed class CertificateEntry(
    X509Certificate2 certificate,
    CertificateReference reference,
    X509Certificate2 trustAnchor)
{
    public X509Certificate2 Certificate { get; } = certificate;
    public CertificateReference Reference { get; } = reference;
    public X509Certificate2 TrustAnchor { get; } = trustAnchor;
    public bool Revoked { get; set; }
}

public sealed class SyntheticCertificateFixture : IDisposable
{
    private SyntheticCertificateFixture(
        CertificateReference reference,
        X509Certificate2 certificate,
        X509Certificate2 trustAnchor)
    {
        Reference = reference;
        Certificate = certificate;
        TrustAnchor = trustAnchor;
    }

    public CertificateReference Reference { get; }
    public X509Certificate2 Certificate { get; }
    public X509Certificate2 TrustAnchor { get; }

    public static SyntheticCertificateFixture Create(
        string referenceId,
        string subject,
        string environment,
        string purpose,
        string usage,
        DateTimeOffset notBefore,
        DateTimeOffset notAfter,
        CandidateContext context)
    {
        if (!context.IsAuthorized ||
            !StringComparer.Ordinal.Equals(environment, CandidateContext.ApprovedEnvironment))
        {
            throw new CandidateBoundaryException("certificate_fixture_creation_rejected");
        }

        using var rootKey = RSA.Create(2048);
        var rootRequest = new CertificateRequest(
            $"CN=Falcon Stage0B Test Root {referenceId}",
            rootKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        rootRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        rootRequest.CertificateExtensions.Add(new X509KeyUsageExtension(
            X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
            true));
        rootRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(rootRequest.PublicKey, false));
        var root = rootRequest.CreateSelfSigned(notBefore.AddMinutes(-1), notAfter.AddMinutes(1));

        using var leafKey = RSA.Create(2048);
        var leafRequest = new CertificateRequest(
            $"CN={subject}",
            leafKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        leafRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
        leafRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
        leafRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
            new OidCollection { new("1.3.6.1.5.5.7.3.1") },
            true));
        leafRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(leafRequest.PublicKey, false));
        var serial = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(referenceId))[..16];
        var leaf = leafRequest.Create(root, notBefore, notAfter, serial);

        var anchorSet = $"test-anchor-set:{referenceId}";
        var reference = new CertificateReference(
            referenceId,
            subject,
            "stage0b-test-issuer",
            environment,
            purpose,
            usage,
            "FALCON-CERT-CANDIDATE-1",
            anchorSet,
            1);
        return new SyntheticCertificateFixture(reference, leaf, root);
    }

    public void Dispose()
    {
        Certificate.Dispose();
        TrustAnchor.Dispose();
    }
}

public sealed class CertificateAndIdentityProviderCandidate
    : CandidateProviderBase, IDisposable
{
    private readonly Dictionary<string, CertificateEntry> _entries = new(StringComparer.Ordinal);
    private readonly Dictionary<string, X509Certificate2> _anchors = new(StringComparer.Ordinal);

    public CertificateAndIdentityProviderCandidate()
        : base("CND-CID-001")
    {
    }

    public CertificateReference AdmitTestIdentity(
        SyntheticCertificateFixture fixture,
        CandidateContext context)
    {
        if (!context.IsAuthorized ||
            !StringComparer.Ordinal.Equals(fixture.Reference.Environment, CandidateContext.ApprovedEnvironment))
        {
            throw new CandidateBoundaryException("certificate_fixture_admission_rejected");
        }

        var certificate = X509CertificateLoader.LoadCertificate(fixture.Certificate.RawData);
        var trustAnchor = X509CertificateLoader.LoadCertificate(fixture.TrustAnchor.RawData);
        _anchors.Add(fixture.Reference.TrustAnchorSet, trustAnchor);
        _entries.Add(
            fixture.Reference.ReferenceId,
            new CertificateEntry(certificate, fixture.Reference, trustAnchor));
        return fixture.Reference;
    }

    public CertificateValidationResult Validate(CertificateValidationRequest request)
    {
        var evidenceId = $"cid-evidence:{request.RequestId}";
        _entries.TryGetValue(request.Reference.ReferenceId, out var entry);
        if (!request.Context.IsAuthorized ||
            entry is null ||
            !StringComparer.Ordinal.Equals(request.ExpectedSubject, entry.Reference.ExpectedSubject) ||
            !StringComparer.Ordinal.Equals(request.Purpose, entry.Reference.Purpose) ||
            !StringComparer.Ordinal.Equals(request.Environment, entry.Reference.Environment) ||
            !StringComparer.Ordinal.Equals(request.Usage, entry.Reference.Usage) ||
            !StringComparer.Ordinal.Equals(request.Reference.TrustAnchorSet, entry.Reference.TrustAnchorSet))
        {
            return Result(
                evidenceId,
                request,
                CertificateValidationDisposition.Rejected,
                entry,
                "scope_or_reference_rejected",
                "NOT_EVALUATED",
                "NOT_EVALUATED");
        }

        if (request.TimeObservation.Disposition != CandidateDisposition.Succeeded ||
            request.TimeObservation.EarliestPossibleTime is null ||
            request.TimeObservation.LatestPossibleTime is null)
        {
            return Result(
                evidenceId,
                request,
                CertificateValidationDisposition.Indeterminate,
                entry,
                "time_indeterminate",
                "NOT_EVALUATED",
                "CURRENT");
        }

        var earliest = ParseTimestamp(request.TimeObservation.EarliestPossibleTime);
        var latest = ParseTimestamp(request.TimeObservation.LatestPossibleTime);
        if (earliest < entry.Certificate.NotBefore.ToUniversalTime() &&
            latest >= entry.Certificate.NotBefore.ToUniversalTime() ||
            earliest <= entry.Certificate.NotAfter.ToUniversalTime() &&
            latest > entry.Certificate.NotAfter.ToUniversalTime())
        {
            return Result(
                evidenceId,
                request,
                CertificateValidationDisposition.Indeterminate,
                entry,
                "validity_boundary_overlaps_uncertainty",
                "NOT_EVALUATED",
                entry.Revoked ? "REVOKED" : "CURRENT");
        }

        if (latest < entry.Certificate.NotBefore.ToUniversalTime() ||
            earliest > entry.Certificate.NotAfter.ToUniversalTime())
        {
            return Result(
                evidenceId,
                request,
                CertificateValidationDisposition.Invalid,
                entry,
                "outside_validity_interval",
                "INVALID",
                entry.Revoked ? "REVOKED" : "CURRENT");
        }

        if (entry.Revoked)
        {
            return Result(
                evidenceId,
                request,
                CertificateValidationDisposition.Invalid,
                entry,
                "certificate_revoked",
                "VALID",
                "REVOKED");
        }

        using var chain = new X509Chain();
        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        chain.ChainPolicy.CustomTrustStore.Add(entry.TrustAnchor);
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.ChainPolicy.DisableCertificateDownloads = true;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
        chain.ChainPolicy.VerificationTime = ParseTimestamp(request.TimeObservation.ObservedUtc!).UtcDateTime;
        var chainValid = chain.Build(entry.Certificate);
        return Result(
            evidenceId,
            request,
            chainValid ? CertificateValidationDisposition.Valid : CertificateValidationDisposition.Invalid,
            entry,
            chainValid ? "candidate_validation_only" : "chain_invalid",
            chainValid ? "VALID" : "INVALID",
            "CURRENT");
    }

    public void Revoke(CertificateReference reference)
    {
        if (!_entries.TryGetValue(reference.ReferenceId, out var entry))
        {
            throw new CandidateBoundaryException("unknown_certificate_reference");
        }

        entry.Revoked = true;
    }

    private CertificateValidationResult Result(
        string evidenceId,
        CertificateValidationRequest request,
        CertificateValidationDisposition disposition,
        CertificateEntry? entry,
        string reason,
        string chain,
        string revocation)
    {
        var digest = entry is null
            ? string.Empty
            : Convert.ToHexString(SHA256.HashData(entry.Certificate.RawData));
        var evidence = disposition == CertificateValidationDisposition.Valid
            ? Succeed(
                evidenceId,
                "certificate.validate",
                ("certificate_digest", digest),
                ("subject", request.ExpectedSubject),
                ("purpose", request.Purpose),
                ("environment", request.Environment),
                ("chain", chain),
                ("revocation", revocation),
                ("private_key_recorded", "false"))
            : Reject(
                evidenceId,
                "certificate.validate",
                reason,
                ("certificate_digest", digest),
                ("subject", request.ExpectedSubject),
                ("chain", chain),
                ("revocation", revocation),
                ("private_key_recorded", "false"));
        return new CertificateValidationResult(disposition, digest, chain, revocation, reason, evidence);
    }

    private static DateTimeOffset ParseTimestamp(string value) =>
        DateTimeOffset.ParseExact(
            value,
            "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal);

    public void Dispose()
    {
        foreach (var entry in _entries.Values)
        {
            entry.Certificate.Dispose();
        }

        foreach (var anchor in _anchors.Values)
        {
            anchor.Dispose();
        }

        _entries.Clear();
        _anchors.Clear();
    }
}

public sealed record TraceLink(
    string RequirementId,
    string CandidateId,
    string VerificationId,
    string GoverningSource);

public sealed class MachineReadableTraceCandidate : CandidateProviderBase
{
    private readonly IReadOnlyList<TraceLink> _links;

    public MachineReadableTraceCandidate(IEnumerable<TraceLink> links)
        : base("CND-TRC-001")
    {
        var materialized = links.OrderBy(link => link.RequirementId, StringComparer.Ordinal).ToArray();
        if (materialized.Length == 0 ||
            materialized.Select(link => link.VerificationId).Distinct(StringComparer.Ordinal).Count() != materialized.Length)
        {
            throw new CandidateBoundaryException("trace_links_invalid");
        }

        _links = new ReadOnlyCollection<TraceLink>(materialized);
    }

    public IReadOnlyList<TraceLink> Links => _links;

    public bool Covers(string requirementId) =>
        _links.Any(link => StringComparer.Ordinal.Equals(link.RequirementId, requirementId));
}
