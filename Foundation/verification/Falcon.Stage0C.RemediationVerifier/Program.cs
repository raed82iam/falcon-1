using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Foundation.Enabling;

var options = Arguments.Parse(args);
var runner = new RemediationVerificationRunner();
var results = runner.RunAll();
var trace = TraceBuilder.Build(options.RepositoryRoot);
var traceChecks = runner.VerifyTrace(trace);
var allResults = results.Concat(traceChecks).ToArray();
var summary = new VerificationSummary(
    "STAGE_0C_REMEDIATION_VERIFICATION",
    "GOV-058",
    FoundationBoundary.Classification,
    false,
    DateTimeOffset.UtcNow,
    allResults.Length,
    allResults.Count(item => item.Passed),
    allResults.Count(item => !item.Passed),
    allResults);

WriteJson(options.EvidencePath, summary);
WriteJson(options.TracePath, new TraceManifest(
    "FALCON-FOUNDATION-TRACE-LOCAL-1",
    "1.0",
    "ACTIVATION_CANDIDATE",
    DateTimeOffset.UtcNow,
    trace.Count,
    trace.OrderBy(item => item.RequirementId, StringComparer.Ordinal).ToArray(),
    ["NO_STAGE_1", "NO_PRODUCTION", "NO_CLOUD", "NO_FINANCIAL_AUTHORITY"]));

Console.WriteLine($"Stage 0C remediation verification: {summary.Passed}/{summary.Total} passed.");
Console.WriteLine($"Machine-readable atomic trace: {trace.Count} unique requirements.");
foreach (var failure in allResults.Where(item => !item.Passed))
{
    Console.WriteLine($"FAILED {failure.VerificationId}: {failure.Reason}");
}

return summary.Failed == 0 ? 0 : 1;

static void WriteJson<T>(string path, T value)
{
    var fullPath = Path.GetFullPath(path);
    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
    File.WriteAllText(
        fullPath,
        JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true }),
        new UTF8Encoding(false));
}

internal sealed record Arguments(string EvidencePath, string TracePath, string RepositoryRoot)
{
    public static Arguments Parse(string[] args)
    {
        string? evidence = null;
        string? trace = null;
        string? root = null;
        for (var index = 0; index < args.Length - 1; index++)
        {
            switch (args[index])
            {
                case "--evidence":
                    evidence = args[++index];
                    break;
                case "--trace":
                    trace = args[++index];
                    break;
                case "--root":
                    root = args[++index];
                    break;
            }
        }

        if (evidence is null || trace is null || root is null)
        {
            throw new ArgumentException("--evidence, --trace, and --root are required.");
        }

        return new(evidence, trace, Path.GetFullPath(root));
    }
}

internal sealed record VerificationResult(
    string VerificationId,
    string Plan,
    string RequirementId,
    string SubjectId,
    bool Passed,
    string Reason);

internal sealed record VerificationSummary(
    string BuildIntent,
    string Authority,
    string Classification,
    bool Operational,
    DateTimeOffset ObservedAt,
    int Total,
    int Passed,
    int Failed,
    IReadOnlyList<VerificationResult> Results);

internal sealed record TraceManifest(
    string TraceId,
    string Version,
    string Status,
    DateTimeOffset GeneratedAt,
    int RequirementCount,
    IReadOnlyList<TraceEntry> Entries,
    IReadOnlyList<string> NonAuthorities);

internal sealed class RemediationVerificationRunner
{
    private readonly List<VerificationResult> _results = [];
    private readonly FoundationAuthorityContext _context;
    private readonly FixedTimeProvider _fixedTime;
    private readonly WindowsCryptographicRandomnessProvider _randomness;
    private readonly WindowsFoundationTimeProvider _time;
    private readonly FoundationIdentifierProvider _identifiers;

    public RemediationVerificationRunner()
    {
        _fixedTime = new(new DateTimeOffset(2026, 7, 27, 12, 0, 0, TimeSpan.Zero));
        _context = new(
            FoundationBoundary.Authority,
            FoundationBoundary.Environment,
            FoundationBoundary.DeploymentProfile,
            "epoch:stage0c-remediation:1",
            "stage0c-remediation-verifier",
            "RVES-STG-0C-REM-001",
            true);
        _randomness = new();
        _time = new(
            _fixedTime,
            _context.RuntimeEpochId,
            _fixedTime.GetUtcNow().AddMinutes(-5),
            50_000);
        _identifiers = new(_time, _randomness);
    }

    public IReadOnlyList<VerificationResult> RunAll()
    {
        VerifyAuthorityAndRestriction();
        VerifyRandomness();
        VerifyTime();
        VerifyIdentifier();
        VerifyCryptoAndSecrets();
        VerifyCertificateIdentity();
        VerifyEnvironment();
        VerifyGate();
        return _results;
    }

    public IReadOnlyList<VerificationResult> VerifyTrace(IReadOnlyList<TraceEntry> trace)
    {
        var start = _results.Count;
        Check("REM-TRC-V01", "VPL-BST-007", "VPL-BST-007-REQ-001", "ACT-TRC-001",
            () => True(trace.Count >= 900));
        Check("REM-TRC-V02", "VPL-BST-007", "VPL-BST-007-REQ-002", "ACT-TRC-001",
            () => Equal(trace.Count, trace.Select(item => item.RequirementId).Distinct(StringComparer.Ordinal).Count()));
        Check("REM-TRC-V03", "VPL-BST-007", "VPL-BST-007-REQ-001", "ACT-TRC-001",
            () => True(trace.All(item => FoundationDigest.IsCanonicalSha256(item.SourceDigest) && item.SourceLine > 0)));
        Check("REM-TRC-V04", "VPL-BST-007", "VPL-BST-007-REQ-002", "ACT-TRC-001",
            () => True(trace.Any(item => item.RequirementId == "CON-010-REQ-020")));
        Check("REM-TRC-V05", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-TRC-001",
            () => Throws<FoundationBoundaryException>(() =>
                _ = new FoundationTraceCatalog(trace.Concat([trace[0]]))));
        Check("REM-TRC-V06", "VPL-BST-008", "VPL-BST-008-REQ-004", "ACT-TRC-001",
            () =>
            {
                var changed = trace[0] with { SourceDigest = new string('0', 64) };
                NotEqual(trace[0].SourceDigest, changed.SourceDigest);
            });
        Check("REM-TRC-V07", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-TRC-001",
            () => Throws<FoundationBoundaryException>(() =>
                _ = new FoundationTraceCatalog([trace[0] with { SourceDigest = new string('g', 64) }])));
        Check("REM-TRC-V08", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-TRC-001",
            () => Throws<FoundationBoundaryException>(() =>
                _ = new FoundationTraceCatalog(null)));
        return _results.Skip(start).ToArray();
    }

    private void VerifyAuthorityAndRestriction()
    {
        Check("REM-AUT-V01", "CON-012", "AUTHORITY-SCOPE", "GOV-058",
            () => True(_context.IsValid));
        Check("REM-AUT-V02", "CON-012", "AUTHORITY-SCOPE", "GOV-058",
            () => False((_context with { AuthorityDecision = "GOV-999" }).IsValid));
        Check("REM-RST-V01", "CON-011", "CON-011-REQ-005", _randomness.SubjectId,
            () =>
            {
                var provider = new WindowsCryptographicRandomnessProvider();
                provider.ApplyRestriction(FoundationLifecycle.Restricted);
                Throws<FoundationBoundaryException>(() => provider.ApplyRestriction(FoundationLifecycle.ActiveScoped));
            });
        Check("REM-RST-V02", "CON-011", "CON-011-REQ-011", _randomness.SubjectId,
            () =>
            {
                var provider = new WindowsCryptographicRandomnessProvider();
                provider.ApplyRestriction(FoundationLifecycle.Suspended);
                provider.ApplyRestriction(FoundationLifecycle.Restricted);
                Equal(FoundationLifecycle.Suspended, provider.Lifecycle);
            });
    }

    private void VerifyRandomness()
    {
        Check("REM-RND-V01", "VPL-BST-005", "VPL-BST-005-REQ-007", _randomness.SubjectId,
            () =>
            {
                var first = _randomness.Produce(new("rnd:1", "crypto-key", 32, false, _context));
                var second = _randomness.Produce(new("rnd:2", "crypto-key", 32, false, _context));
                Equal(FoundationDisposition.Succeeded, first.Disposition);
                Equal(32, first.Material!.Length);
                False(first.Material.AsSpan().SequenceEqual(second.Material));
            });
        Check("REM-RND-V02", "VPL-BST-005", "VPL-BST-005-REQ-007", _randomness.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                _randomness.Produce(new("rnd:caller", "crypto-key", 32, true, _context)).Disposition));
        Check("REM-RND-V03", "VPL-BST-005", "VPL-BST-005-REQ-007", _randomness.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                _randomness.Produce(new("rnd:purpose", "invented-purpose", 32, false, _context)).Disposition));
        Check("REM-RND-V04", "CON-011", "CON-011-REQ-004", _randomness.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                _randomness.Produce(new("rnd:authority", "crypto-key", 32, false, _context with { ProtectivePermission = false })).Disposition));
        Check("REM-RND-V05", "VPL-BST-005", "VPL-BST-005-REQ-007", _randomness.SubjectId,
            () => Equal(FoundationDisposition.Rejected, _randomness.Produce(null).Disposition));
        Check("REM-RND-V06", "VPL-BST-005", "VPL-BST-005-REQ-007", _randomness.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                _randomness.Produce(new(" ", "crypto-key", 32, false, _context)).Disposition));
    }

    private void VerifyTime()
    {
        Check("REM-TIM-V01", "VPL-BST-004", "VPL-BST-004-REQ-001", _time.SubjectId,
            () =>
            {
                var observation = _time.Observe(_context);
                Equal(FoundationDisposition.Succeeded, observation.Disposition);
                Equal(ClockQuality.VerifiedLocalBuild, observation.Quality);
                Equal(50_000L, observation.MaximumUncertaintyMicroseconds);
            });
        Check("REM-TIM-V02", "VPL-BST-004", "VPL-BST-004-REQ-004", _time.SubjectId,
            () =>
            {
                var first = _time.Observe(_context);
                _fixedTime.Advance(TimeSpan.FromMilliseconds(1));
                var second = _time.Observe(_context);
                True(WindowsFoundationTimeProvider.CanCompareMonotonic(first, second));
            });
        Check("REM-TIM-V03", "VPL-BST-004", "VPL-BST-004-REQ-003", _time.SubjectId,
            () =>
            {
                var observation = _time.Observe(_context);
                False(WindowsFoundationTimeProvider.IsDefinitelyBefore(
                    observation,
                    observation.ObservedUtc!.Value.AddMilliseconds(1)));
                True(WindowsFoundationTimeProvider.IsDefinitelyBefore(
                    observation,
                    observation.ObservedUtc!.Value.AddSeconds(1)));
            });
        Check("REM-TIM-V04", "VPL-BST-004", "VPL-BST-004-REQ-006", _time.SubjectId,
            () =>
            {
                var staleClock = new WindowsFoundationTimeProvider(
                    _fixedTime,
                    "epoch:stale",
                    _fixedTime.GetUtcNow().AddHours(-5),
                    50_000);
                Equal(FoundationDisposition.Rejected, staleClock.Observe(_context).Disposition);
            });
        Check("REM-TIM-V05", "VPL-BST-004", "VPL-BST-004-REQ-006", _time.SubjectId,
            () =>
            {
                var futureClock = new WindowsFoundationTimeProvider(
                    _fixedTime,
                    "epoch:future",
                    _fixedTime.GetUtcNow().AddMinutes(1),
                    50_000);
                var observation = futureClock.Observe(_context);
                Equal(FoundationDisposition.Rejected, observation.Disposition);
                Equal(ClockQuality.Conflicted, observation.Quality);
            });
        Check("REM-TIM-V06", "VPL-BST-004", "VPL-BST-004-REQ-003", _time.SubjectId,
            () =>
            {
                var negative = new WindowsFoundationTimeProvider(
                    _fixedTime,
                    "epoch:negative",
                    _fixedTime.GetUtcNow().AddMinutes(-1),
                    -1);
                Equal(FoundationDisposition.Rejected, negative.Observe(_context).Disposition);
            });
        Check("REM-TIM-V07", "VPL-BST-004", "VPL-BST-004-REQ-003", _time.SubjectId,
            () =>
            {
                var excessive = new WindowsFoundationTimeProvider(
                    _fixedTime,
                    "epoch:excessive",
                    _fixedTime.GetUtcNow().AddMinutes(-1),
                    WindowsFoundationTimeProvider.MaximumSupportedUncertaintyMicroseconds + 1);
                Equal(FoundationDisposition.Rejected, excessive.Observe(_context).Disposition);
            });
        Check("REM-TIM-V08", "VPL-BST-004", "VPL-BST-004-REQ-003", _time.SubjectId,
            () => False(WindowsFoundationTimeProvider.IsDefinitelyBefore(null, _fixedTime.GetUtcNow())));
    }

    private void VerifyIdentifier()
    {
        var request = new IdentifierRequest(
            "id:continuity",
            "falcon.foundation.operation",
            "subject:one",
            "internal-foundation",
            _context);
        Check("REM-IDN-V01", "VPL-BST-003", "VPL-BST-003-REQ-001", _identifiers.SubjectId,
            () =>
            {
                var result = _identifiers.Issue(request);
                Equal(FoundationDisposition.Succeeded, result.Disposition);
                Equal('7', result.Identifier![14]);
                True(result.Identifier[19] is '8' or '9' or 'a' or 'b');
            });
        Check("REM-IDN-V02", "VPL-BST-003", "VPL-BST-003-REQ-004", _identifiers.SubjectId,
            () =>
            {
                var first = _identifiers.Issue(request);
                var second = _identifiers.Issue(request);
                Equal(first.Identifier, second.Identifier);
                NotEqual(first.AttemptIdentifier, second.AttemptIdentifier);
            });
        Check("REM-IDN-V03", "VPL-BST-003", "VPL-BST-003-REQ-006", _identifiers.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                _identifiers.Issue(request with { RequestId = "id:public", ExposureBoundary = "public" }).Disposition));
        Check("REM-IDN-V04", "VPL-BST-003", "VPL-BST-003-REQ-008", _identifiers.SubjectId,
            () => False(_identifiers.CanSelfActivate));
        Check("REM-IDN-V05", "VPL-BST-003", "VPL-BST-003-REQ-001", _identifiers.SubjectId,
            () => Equal(FoundationDisposition.Rejected, _identifiers.Issue(null).Disposition));
        Check("REM-IDN-V06", "VPL-BST-003", "VPL-BST-003-REQ-004", _identifiers.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                _identifiers.Issue(request with { LogicalSubject = "subject:changed" }).Disposition));
        Check("REM-IDN-V07", "VPL-BST-003", "VPL-BST-003-REQ-004", _identifiers.SubjectId,
            () =>
            {
                var concurrentRequest = request with { RequestId = "id:concurrent", LogicalSubject = "subject:concurrent" };
                var results = Enumerable.Range(0, 64)
                    .AsParallel()
                    .WithDegreeOfParallelism(8)
                    .Select(_ => _identifiers.Issue(concurrentRequest))
                    .ToArray();
                True(results.All(item => item.Disposition == FoundationDisposition.Succeeded));
                Equal(1, results.Select(item => item.Identifier).Distinct(StringComparer.Ordinal).Count());
                Equal(64, results.Select(item => item.AttemptIdentifier).Distinct(StringComparer.Ordinal).Count());
            });
    }

    private void VerifyCryptoAndSecrets()
    {
        using var custody = new EphemeralFoundationKeyCustody(_randomness);
        var crypto = new FoundationCryptographicAdapter(custody, _randomness);
        var encryptionKey = custody.Generate(
            "key:encryption:1",
            "falcon/foundation/evidence/encryption",
            "protect",
            _context);
        var integrityKey = custody.Generate(
            "key:integrity:1",
            "falcon/foundation/evidence/integrity",
            "authenticate",
            _context);
        var context = FoundationCanonicalContext.Create(
            FoundationBoundary.Environment,
            encryptionKey.DomainId,
            encryptionKey.PurposeId,
            crypto.ProfileId,
            encryptionKey.Version);
        var plaintext = "falcon-foundation-verification"u8.ToArray();
        var protectedPayload = crypto.Encrypt("crypto:1", encryptionKey, plaintext, context, _context);

        Check("REM-CRY-V01", "VPL-BST-005", "VPL-BST-005-REQ-001", crypto.SubjectId,
            () => SequenceEqual(plaintext, crypto.Decrypt("crypto:2", encryptionKey, protectedPayload, context, _context)));
        Check("REM-CRY-V02", "VPL-BST-005", "VPL-BST-005-REQ-008", crypto.SubjectId,
            () =>
            {
                var tamperedTag = protectedPayload.Tag;
                tamperedTag[0] ^= 1;
                var tampered = protectedPayload with { Tag = tamperedTag };
                Throws<AuthenticationTagMismatchException>(() =>
                    crypto.Decrypt("crypto:tampered", encryptionKey, tampered, context, _context));
            });
        Check("REM-CRY-V03", "VPL-BST-005", "VPL-BST-005-REQ-003", crypto.SubjectId,
            () => Throws<FoundationBoundaryException>(() =>
                crypto.Authenticate(encryptionKey, plaintext, _context)));
        Check("REM-CRY-V04", "VPL-BST-005", "VPL-BST-005-REQ-003", crypto.SubjectId,
            () => Equal(32, crypto.Authenticate(integrityKey, plaintext, _context).Length));
        Check("REM-CRY-V05", "VPL-BST-005", "VPL-BST-005-REQ-005", crypto.SubjectId,
            () =>
            {
                custody.Revoke(integrityKey);
                Throws<FoundationBoundaryException>(() =>
                    crypto.Authenticate(integrityKey, plaintext, _context));
            });
        Check("REM-CRY-V06", "VPL-BST-005", "VPL-BST-005-REQ-008", crypto.SubjectId,
            () =>
            {
                var exposedNonce = protectedPayload.Nonce;
                var exposedCiphertext = protectedPayload.Ciphertext;
                var exposedTag = protectedPayload.Tag;
                exposedNonce[0] ^= 1;
                exposedCiphertext[0] ^= 1;
                exposedTag[0] ^= 1;
                SequenceEqual(plaintext, crypto.Decrypt("crypto:alias-safe", encryptionKey, protectedPayload, context, _context));
            });
        Check("REM-CRY-V07", "VPL-BST-005", "VPL-BST-005-REQ-005", crypto.SubjectId,
            () =>
            {
                var rotatingKey = custody.Generate(
                    "key:rotation:1",
                    "falcon/foundation/evidence/integrity",
                    "authenticate",
                    _context);
                var rotated = custody.Rotate(rotatingKey, _context);
                Throws<FoundationBoundaryException>(() => custody.Rotate(rotatingKey, _context));
                Throws<FoundationBoundaryException>(() => custody.Revoke(rotatingKey));
                Equal(32, crypto.Authenticate(rotated, plaintext, _context).Length);
            });
        Check("REM-CRY-V08", "VPL-BST-005", "VPL-BST-005-REQ-005", crypto.SubjectId,
            () => Throws<FoundationBoundaryException>(() =>
                custody.Revoke(encryptionKey with { DomainId = "falcon/foundation/evidence/integrity" })));

        var secrets = new FoundationSecretProvider(custody);
        var secret = secrets.Create("secret:foundation:1", _context);
        Check("REM-SEC-V01", "VPL-BST-005", "VPL-BST-005-REQ-004", secrets.SubjectId,
            () => Equal(32, secrets.UseForBoundedDerivation(secret, plaintext, _context).Length));
        Check("REM-SEC-V02", "VPL-BST-005", "VPL-BST-005-REQ-005", secrets.SubjectId,
            () =>
            {
                var rotated = secrets.Rotate(secret, _context);
                Throws<FoundationBoundaryException>(() =>
                    secrets.UseForBoundedDerivation(secret, plaintext, _context));
                Equal(32, secrets.UseForBoundedDerivation(rotated, plaintext, _context).Length);
            });
        Check("REM-SEC-V03", "VPL-BST-005", "VPL-BST-005-REQ-005", secrets.SubjectId,
            () => Throws<FoundationBoundaryException>(() =>
                secrets.UseForBoundedDerivation(
                    secret with { DomainId = "falcon/foundation/evidence/integrity" },
                    plaintext,
                    _context)));
    }

    private void VerifyCertificateIdentity()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=falcon.foundation.local",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        using var withPrivateKey = request.CreateSelfSigned(
            _fixedTime.GetUtcNow().AddMinutes(-5),
            _fixedTime.GetUtcNow().AddMinutes(30));
        using var certificate = X509CertificateLoader.LoadCertificate(withPrivateKey.Export(X509ContentType.Cert));
        var digest = Convert.ToHexString(SHA256.HashData(certificate.RawData));
        var provider = new FoundationCertificateIdentityProvider(_time);

        Check("REM-CID-V01", "VPL-BST-005", "VPL-BST-005-REQ-006", provider.SubjectId,
            () => Equal(
                FoundationDisposition.Succeeded,
                provider.Validate(certificate, "falcon.foundation.local", digest, _context).Disposition));
        Check("REM-CID-V02", "VPL-BST-005", "VPL-BST-005-REQ-006", provider.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                provider.Validate(certificate, "other.identity", digest, _context).Disposition));
        Check("REM-CID-V03", "VPL-BST-005", "VPL-BST-005-REQ-006", provider.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                provider.Validate(certificate, "foundation.local", digest, _context).Disposition));
        Check("REM-CID-V04", "VPL-BST-005", "VPL-BST-005-REQ-006", provider.SubjectId,
            () => Equal(
                FoundationDisposition.Rejected,
                provider.Validate(certificate, "falcon.foundation.local", digest.ToLowerInvariant(), _context).Disposition));
        Check("REM-CID-V05", "VPL-BST-005", "VPL-BST-005-REQ-006", provider.SubjectId,
            () =>
            {
                provider.Revoke(digest);
                Equal(
                    FoundationDisposition.Rejected,
                    provider.Validate(certificate, "falcon.foundation.local", digest, _context).Disposition);
            });
    }

    private void VerifyEnvironment()
    {
        var profiles = new[]
        {
            _randomness.ProfileId,
            _time.ProfileId,
            _identifiers.ProfileId,
            "FALCON-CRYPTO-BCL-1",
            "FALCON-SECRET-EPHEMERAL-1",
            "FALCON-CERT-LOCAL-TRUST-1"
        };
        var valid = new FoundationEnvironmentProfile(
            FoundationBoundary.Environment,
            FoundationBoundary.DeploymentProfile,
            "windows",
            FoundationBoundary.Classification,
            true,
            true,
            true,
            profiles,
            ["NO_STAGE_1", "NO_PRODUCTION", "NO_CLOUD", "NO_FINANCIAL_AUTHORITY"]);

        Check("REM-ENV-V01", "VPL-BST-006", "VPL-BST-006-REQ-001", "ACT-ENV-001",
            () => True(valid.IsValid));
        Check("REM-ENV-V02", "VPL-BST-006", "VPL-BST-006-REQ-005", "ACT-ENV-001",
            () => False((valid with { ActiveProviderProfiles = profiles[..5] }).IsValid));
        Check("REM-ENV-V03", "VPL-BST-006", "VPL-BST-006-REQ-006", "ACT-ENV-001",
            () => False((valid with { NonAuthorities = ["NO_STAGE_1"] }).IsValid));
        Check("REM-ENV-V04", "VPL-BST-006", "VPL-BST-006-REQ-009", "ACT-ENV-001",
            () => True(valid.NonAuthorities.Contains("NO_PRODUCTION", StringComparer.Ordinal)));
        Check("REM-ENV-V05", "VPL-BST-006", "VPL-BST-006-REQ-005", "ACT-ENV-001",
            () => False((valid with { ActiveProviderProfiles = profiles[..5].Concat([profiles[0]]).ToArray() }).IsValid));
        Check("REM-ENV-V06", "VPL-BST-006", "VPL-BST-006-REQ-005", "ACT-ENV-001",
            () => False((valid with { ActiveProviderProfiles = profiles[..5].Concat(["FALCON-UNEXPECTED-1"]).ToArray() }).IsValid));
        Check("REM-ENV-V07", "VPL-BST-006", "VPL-BST-006-REQ-006", "ACT-ENV-001",
            () => False((valid with { NonAuthorities = ["NO_STAGE_1", "NO_PRODUCTION", "NO_CLOUD", "NO_CLOUD"] }).IsValid));
    }

    private void VerifyGate()
    {
        var requirements = new[]
        {
            new EvidenceRequirement("REM-EVD-001", EvidenceRequirementClass.Mandatory),
            new EvidenceRequirement("REM-EVD-002", EvidenceRequirementClass.Mandatory),
            new EvidenceRequirement("REM-EVD-003", EvidenceRequirementClass.Derived)
        };
        var evidence = new[]
        {
            ObservedEvidence.Create("REM-EVD-001", "evidence:1", "canonical evidence one"),
            ObservedEvidence.Create("REM-EVD-002", "evidence:2", "canonical evidence two")
        };
        var valid = new RootEvidenceSet(
            "RVES-REM-001",
            "ERS-REM-001",
            requirements,
            evidence,
            "ECTX-REM-001",
            "evidence-producer",
            "independent-completeness-authority",
            false,
            false);
        var gate = new FoundationEvidenceGate();

        Check("REM-GATE-V01", "VPL-BST-007", "VPL-BST-007-REQ-004", "ACT-GATE-001",
            () => Equal(EvidenceCompleteness.Complete, gate.Evaluate(valid)));
        Check("REM-GATE-V02", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-GATE-001",
            () => Equal(EvidenceCompleteness.Incomplete, gate.Evaluate(valid with { Evidence = evidence[..1] })));
        Check("REM-GATE-V03", "VPL-BST-007", "VPL-BST-007-REQ-007", "ACT-GATE-001",
            () => Equal(
                EvidenceCompleteness.Invalid,
                gate.Evaluate(valid with { CompletenessAuthority = valid.ProducerAuthority })));
        Check("REM-GATE-V04", "VPL-BST-007", "VPL-BST-007-REQ-009", "ACT-GATE-001",
            () => Equal(EvidenceCompleteness.Invalid, gate.Evaluate(valid with { GateWeakened = true })));
        Check("REM-GATE-V05", "VPL-BST-007", "VPL-BST-007-REQ-006", "ACT-PIPE-001",
            () => Equal(EvidenceCompleteness.Invalid, gate.Evaluate(valid with { DirectSessionPromotion = true })));
        Check("REM-GATE-V06", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-GATE-001",
            () => Equal(
                EvidenceCompleteness.Invalid,
                gate.Evaluate(valid with
                {
                    Evidence = [evidence[0] with { CanonicalContent = "tampered" }, evidence[1]]
                })));
        Check("REM-GATE-V07", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-GATE-001",
            () => Equal(
                EvidenceCompleteness.Invalid,
                gate.Evaluate(valid with
                {
                    Evidence = [evidence[0] with { Digest = new string('g', 64) }, evidence[1]]
                })));
        Check("REM-GATE-V08", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-GATE-001",
            () => Equal(
                EvidenceCompleteness.Invalid,
                gate.Evaluate(valid with
                {
                    Evidence = [evidence[0], evidence[1] with { RequirementId = "REM-EVD-UNKNOWN" }]
                })));
        Check("REM-GATE-V09", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-GATE-001",
            () => Equal(
                EvidenceCompleteness.Invalid,
                gate.Evaluate(valid with
                {
                    Evidence = [evidence[0], evidence[1] with { EvidenceId = evidence[0].EvidenceId }]
                })));
        Check("REM-GATE-V10", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-GATE-001",
            () => Equal(EvidenceCompleteness.Invalid, gate.Evaluate(null)));
    }

    private void Check(string id, string plan, string requirement, string subject, Action action)
    {
        try
        {
            action();
            _results.Add(new(id, plan, requirement, subject, true, "PASS"));
        }
        catch (Exception exception)
        {
            _results.Add(new(id, plan, requirement, subject, false, exception.GetType().Name + ":" + exception.Message));
        }
    }

    private static void True(bool value)
    {
        if (!value)
        {
            throw new VerificationException("Expected true.");
        }
    }

    private static void False(bool value) => True(!value);

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new VerificationException($"Expected '{expected}', got '{actual}'.");
        }
    }

    private static void NotEqual<T>(T first, T second)
    {
        if (EqualityComparer<T>.Default.Equals(first, second))
        {
            throw new VerificationException($"Expected different values, got '{first}'.");
        }
    }

    private static void SequenceEqual(byte[] expected, byte[] actual)
    {
        if (!expected.AsSpan().SequenceEqual(actual))
        {
            throw new VerificationException("Byte sequences differ.");
        }
    }

    private static void Throws<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new VerificationException($"Expected {typeof(TException).Name}.");
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset initial) : TimeProvider
{
    private DateTimeOffset _utcNow = initial;
    private long _timestamp;

    public override DateTimeOffset GetUtcNow() => _utcNow;
    public override long GetTimestamp() => Interlocked.Increment(ref _timestamp);

    public void Advance(TimeSpan duration)
    {
        _utcNow = _utcNow.Add(duration);
        Interlocked.Add(ref _timestamp, duration.Ticks);
    }
}

internal static class TraceBuilder
{
    private static readonly Regex RequirementPattern = new(
        "\\*\\*([A-Z0-9-]+-REQ-[0-9]+):?\\*\\*",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly string[] ExcludedSegments =
    [
        $"{Path.DirectorySeparatorChar}old{Path.DirectorySeparatorChar}",
        $"{Path.DirectorySeparatorChar}archive{Path.DirectorySeparatorChar}",
        $"{Path.DirectorySeparatorChar}candidates{Path.DirectorySeparatorChar}",
        $"{Path.DirectorySeparatorChar}evidence{Path.DirectorySeparatorChar}",
        $"{Path.DirectorySeparatorChar}amendments{Path.DirectorySeparatorChar}"
    ];

    public static IReadOnlyList<TraceEntry> Build(string repositoryRoot)
    {
        var docsRoot = Path.Combine(repositoryRoot, "docs");
        var entries = new List<TraceEntry>();
        foreach (var file in Directory.EnumerateFiles(docsRoot, "*.md", SearchOption.AllDirectories)
                     .Where(path => !ExcludedSegments.Any(path.Contains))
                     .OrderBy(path => path, StringComparer.Ordinal))
        {
            var digest = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file)));
            var lineNumber = 0;
            foreach (var line in File.ReadLines(file))
            {
                lineNumber++;
                foreach (Match match in RequirementPattern.Matches(line))
                {
                    entries.Add(new(
                        match.Groups[1].Value,
                        Path.GetRelativePath(repositoryRoot, file).Replace('\\', '/'),
                        lineNumber,
                        digest));
                }
            }
        }

        _ = new FoundationTraceCatalog(entries);
        return entries;
    }
}

internal sealed class VerificationException(string message) : Exception(message);
