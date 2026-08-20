using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Falcon.Stage0B.Candidates;

var evidencePath = ParseEvidencePath(args);
var runner = new VerificationRunner();
var results = runner.RunAll();
var summary = new VerificationSummary(
    "STAGE_0B_CANDIDATE_VERIFICATION",
    "GOV-051",
    "CANDIDATE",
    false,
    DateTimeOffset.UtcNow,
    results.Count,
    results.Count(result => result.Passed),
    results.Count(result => !result.Passed),
    results);

if (evidencePath is not null)
{
    var fullPath = Path.GetFullPath(evidencePath);
    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
    File.WriteAllText(
        fullPath,
        JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true }),
        new UTF8Encoding(false));
}

Console.WriteLine($"Stage 0B candidate verification: {summary.Passed}/{summary.Total} passed.");
foreach (var failed in results.Where(result => !result.Passed))
{
    Console.WriteLine($"FAILED {failed.VerificationId}: {failed.Reason}");
}

return summary.Failed == 0 ? 0 : 1;

static string? ParseEvidencePath(string[] arguments)
{
    for (var index = 0; index < arguments.Length; index++)
    {
        if (StringComparer.Ordinal.Equals(arguments[index], "--evidence") &&
            index + 1 < arguments.Length)
        {
            return arguments[index + 1];
        }
    }

    return null;
}

internal sealed record VerificationResult(
    string VerificationId,
    string Plan,
    string RequirementId,
    string CandidateId,
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

internal sealed class VerificationRunner
{
    private readonly CandidateContext _context = new(
        CandidateContext.ApprovedAuthority,
        CandidateContext.ApprovedEnvironment,
        "bootstrap-runtime-epoch:stage0b:verification",
        "stage0b-independent-verifier",
        true,
        "stage0b-root-evidence-set");

    private readonly List<VerificationResult> _results = [];

    public IReadOnlyList<VerificationResult> RunAll()
    {
        VerifyCanonicalEncoding();
        VerifyTrustObjects();
        VerifyIdentifierCandidate();
        VerifyTimeCandidate();
        VerifySecurityCandidates();
        VerifyCertificateCandidate();
        VerifyTraceAndPipelineCandidates();
        return _results;
    }

    private void VerifyCanonicalEncoding()
    {
        var candidate = new CanonicalEncodingSupportCandidate();
        Check("FCE-001-V01", "FCE-001", "FCE-001-REQ-014", candidate.CandidateId, () =>
        {
            var source = new DateTimeOffset(2026, 7, 26, 12, 34, 56, TimeSpan.Zero).AddTicks(1234567);
            Equal("2026-07-26T12:34:56.123456Z", candidate.EncodeTimestamp(source));
            False(candidate.IsOperational);
            False(candidate.CanSelfActivate);
        });

        Check("FCE-001-V02", "FCE-001", "FCE-001-REQ-003", "CND-FCE-001", () =>
        {
            True(FalconCanonicalEncoding.IsCanonicalTimestamp("2026-07-26T12:34:56.123456Z"));
            False(FalconCanonicalEncoding.IsCanonicalTimestamp("2026-07-26T12:34:56Z"));
            False(FalconCanonicalEncoding.IsCanonicalTimestamp("2026-07-26T12:34:60.123456Z"));
        });

        Check("FCE-001-V03", "FCE-001", "FCE-001-REQ-012", "CND-FCE-001", () =>
        {
            var value = Guid.Parse("018f7e32-7b10-7abc-8def-0123456789ab");
            Equal("018f7e32-7b10-7abc-8def-0123456789ab", FalconCanonicalEncoding.Identifier(value));
            True(FalconCanonicalEncoding.IsCanonicalIdentifier(FalconCanonicalEncoding.Identifier(value)));
            False(FalconCanonicalEncoding.IsCanonicalIdentifier(Guid.Empty.ToString("D")));
        });

        Check("FCE-001-V04", "FCE-001", "FCE-001-REQ-004", "CND-FCE-001", () =>
        {
            var first = FalconCanonicalEncoding.Record(
                "falcon/test",
                1,
                [
                    new(2, FceWireType.Text, FalconCanonicalEncoding.Text("b")),
                    new(1, FceWireType.Text, FalconCanonicalEncoding.Text("a"))
                ]);
            var second = FalconCanonicalEncoding.Record(
                "falcon/test",
                1,
                [
                    new(1, FceWireType.Text, FalconCanonicalEncoding.Text("a")),
                    new(2, FceWireType.Text, FalconCanonicalEncoding.Text("b"))
                ]);
            SequenceEqual(first, second);
            FalconCanonicalEncoding.ValidateRecord(first);
        });

        Check("FCE-001-V05", "FCE-001", "FCE-001-REQ-018", "CND-FCE-001", () =>
        {
            var bytes = CreateDomainContext("falcon/test/encryption", "protect", 1);
            FalconCanonicalEncoding.ValidateRecord(bytes);
            var other = CreateDomainContext("falcon/test/integrity", "authenticate", 1);
            False(bytes.AsSpan().SequenceEqual(other));
        });
    }

    private void VerifyTrustObjects()
    {
        var candidate = new TrustObjectPrimitivesCandidate();
        Check("SEC-002-V01", "SEC-002", "TRUST-SCOPED-VALIDITY", candidate.CandidateId, () =>
        {
            var trustObject = candidate.Create(
                "trust:stage0b:1",
                "VerificationEvidence",
                "1",
                "stage0b-verifier",
                "stage0b-candidate-verification",
                TrustObjectStatus.Valid,
                [
                    new(
                        "claim:1",
                        "PASS",
                        "true",
                        "stage0b-candidate-verification",
                        "GOV-051",
                        "stage0b-verifier",
                        true)
                ]);
            True(trustObject.IsAcceptedFor("stage0b-candidate-verification", "GOV-051"));
            False(trustObject.IsAcceptedFor("activation", "GOV-051"));
            True(trustObject.Claims[0].Challengeable);
            False(candidate.IsOperational);
            False(candidate.CanSelfActivate);
        });
    }

    private void VerifyIdentifierCandidate()
    {
        var material = new AdvancingIdentifierMaterial();
        var provider = new IdentifierProviderCandidate(material);
        var classes = new[]
        {
            "falcon.foundation.operation",
            "falcon.foundation.evidence",
            "falcon.foundation.attempt",
            "falcon.foundation.runtime-epoch"
        };

        Check("VPL-BST-003-V01", "VPL-BST-003", "VPL-BST-003-REQ-001", provider.CandidateId, () =>
        {
            foreach (var identifierClass in classes)
            {
                var result = provider.Issue(IdentifierRequestFor(
                    $"request:{identifierClass}",
                    identifierClass,
                    $"subject:{identifierClass}"));
                Equal(CandidateDisposition.Succeeded, result.Disposition);
                True(FalconCanonicalEncoding.IsCanonicalIdentifier(result.Identifier!));
                False(result.Operational);
            }
        });

        Check("VPL-BST-003-V02", "VPL-BST-003", "VPL-BST-003-REQ-004", provider.CandidateId, () =>
        {
            var request = IdentifierRequestFor("retry:1", classes[0], "subject:retry");
            var first = provider.Issue(request);
            var second = provider.Issue(request);
            Equal(first.Identifier, second.Identifier);
            NotEqual(first.AttemptIdentifier, second.AttemptIdentifier);
        });

        Check("VPL-BST-003-V03", "VPL-BST-003", "VPL-BST-003-REQ-003", provider.CandidateId, () =>
        {
            var request = IdentifierRequestFor("raw:1", classes[0], "subject:raw") with
            {
                ContainsCallerGenerationMaterial = true
            };
            Equal(CandidateDisposition.Rejected, provider.Issue(request).Disposition);
        });

        Check("VPL-BST-003-V04", "VPL-BST-003", "VPL-BST-003-REQ-006", provider.CandidateId, () =>
        {
            var request = IdentifierRequestFor("exposure:1", classes[0], "subject:exposure") with
            {
                ExposureBoundary = "public"
            };
            Equal("exposure_not_approved", provider.Issue(request).Reason);
        });

        Check("VPL-BST-003-V05", "VPL-BST-003", "VPL-BST-003-REQ-007", provider.CandidateId, () =>
        {
            var unavailable = new IdentifierProviderCandidate(new UnavailableIdentifierMaterial());
            var result = unavailable.Issue(IdentifierRequestFor("dependency:1", classes[0], "subject:dependency"));
            Equal(CandidateDisposition.Rejected, result.Disposition);
            Null(result.Identifier);
        });

        Check("VPL-BST-003-V06", "VPL-BST-003", "VPL-BST-003-REQ-005", provider.CandidateId, () =>
        {
            var collisionProvider = new IdentifierProviderCandidate(new CollisionIdentifierMaterial());
            var first = collisionProvider.Issue(IdentifierRequestFor("collision:1", classes[0], "subject:a") with
            {
                ContinuityRequired = false
            });
            var second = collisionProvider.Issue(IdentifierRequestFor("collision:2", classes[0], "subject:b") with
            {
                ContinuityRequired = false
            });
            Equal(CandidateDisposition.Succeeded, first.Disposition);
            Equal("identity_collision", second.Reason);
        });

        Check("VPL-BST-003-V07", "VPL-BST-003", "VPL-BST-003-REQ-008", provider.CandidateId, () =>
        {
            False(provider.IsOperational);
            False(provider.CanSelfActivate);
            Equal(CandidateLifecycle.Candidate, provider.Lifecycle);
        });
    }

    private void VerifyTimeCandidate()
    {
        var observed = new DateTimeOffset(2026, 7, 26, 13, 0, 0, TimeSpan.Zero);
        var source = new MutableClockSource(CreateSample(observed));
        var provider = new TimeProviderCandidate(source);

        Check("VPL-BST-004-V01", "VPL-BST-004", "VPL-BST-004-REQ-001", provider.CandidateId, () =>
        {
            var result = provider.Observe(TimeRequest("time:1"));
            Equal(CandidateDisposition.Succeeded, result.Disposition);
            Equal("2026-07-26T13:00:00.000000Z", result.ObservedUtc);
        });

        Check("VPL-BST-004-V02", "VPL-BST-004", "VPL-BST-004-REQ-002", provider.CandidateId, () =>
        {
            var result = provider.Observe(TimeRequest("time:2"));
            NotNull(result.ClockSourceId);
            NotNull(result.RuntimeEpochId);
            True(result.MaximumUncertaintyMicroseconds > 0);
            True(result.Capabilities.HasFlag(ClockCapabilities.Uncertainty));
            Equal("CANDIDATE", result.Classification);
        });

        Check("VPL-BST-004-V03", "VPL-BST-004", "VPL-BST-004-REQ-003", provider.CandidateId, () =>
        {
            var result = provider.Observe(TimeRequest("time:3"));
            var overlappingBoundary = observed.AddTicks(250);
            False(TimeProviderCandidate.IsDefinitelyBefore(result, overlappingBoundary));
            True(TimeProviderCandidate.IsDefinitelyBefore(result, observed.AddSeconds(1)));
        });

        Check("VPL-BST-004-V04", "VPL-BST-004", "VPL-BST-004-REQ-004", provider.CandidateId, () =>
        {
            var first = provider.Observe(TimeRequest("time:4a"));
            source.Sample = source.Sample with { MonotonicMicroseconds = 2000 };
            var second = provider.Observe(TimeRequest("time:4b"));
            True(TimeProviderCandidate.CanCompareMonotonic(first, second));
            source.Sample = source.Sample with { RuntimeEpochId = "epoch:other" };
            var otherEpoch = provider.Observe(TimeRequest("time:4c"));
            False(TimeProviderCandidate.CanCompareMonotonic(first, otherEpoch));
        });

        Check("VPL-BST-004-V05", "VPL-BST-004", "VPL-BST-004-REQ-006", provider.CandidateId, () =>
        {
            source.Sample = CreateSample(observed) with { SourceConflict = true };
            var conflict = provider.Observe(TimeRequest("time:5"));
            Equal(CandidateDisposition.Rejected, conflict.Disposition);
            source.Sample = CreateSample(observed) with { LastVerification = observed.AddHours(-2) };
            var stale = provider.Observe(TimeRequest("time:6"));
            Equal(CandidateDisposition.Rejected, stale.Disposition);
        });

        Check("VPL-BST-004-V06", "VPL-BST-004", "VPL-BST-004-REQ-007", provider.CandidateId, () =>
        {
            source.Sample = CreateSample(observed) with { Available = false };
            var failed = provider.Observe(TimeRequest("time:7"));
            Equal(CandidateDisposition.Rejected, failed.Disposition);
            Null(failed.ObservedUtc);
        });

        Check("VPL-BST-004-V07", "VPL-BST-004", "VPL-BST-004-REQ-008", provider.CandidateId, () =>
        {
            False(provider.IsOperational);
            False(provider.CanSelfActivate);
        });
    }

    private void VerifySecurityCandidates()
    {
        var entropy = new AdvancingEntropySource();
        var randomness = new RandomnessProviderCandidate(entropy);

        Check("VPL-BST-005-V01", "VPL-BST-005", "VPL-BST-005-REQ-007", randomness.CandidateId, () =>
        {
            var valid = randomness.Produce(RandomRequest("random:1", "test-nonce", 16));
            Equal(CandidateDisposition.Succeeded, valid.Disposition);
            NotNull(valid.Material);
            var rejected = randomness.Produce(RandomRequest("random:2", "test-nonce", 16) with
            {
                CallerSuppliedEntropy = true
            });
            Equal(CandidateDisposition.Rejected, rejected.Disposition);
            Null(rejected.Material);
        });

        Check("VPL-BST-005-V02", "VPL-BST-005", "VPL-BST-005-REQ-007", randomness.CandidateId, () =>
        {
            var failedProvider = new RandomnessProviderCandidate(new UnavailableEntropySource());
            var failed = failedProvider.Produce(RandomRequest("random:failed", "test-key", 32));
            Equal(CandidateDisposition.Rejected, failed.Disposition);
            Null(failed.Material);
        });

        using var custody = new CandidateKeyCustody();
        var crypto = new CryptographicProviderAdapterCandidate(custody);
        var encryptionKey = custody.Generate(
            "key:test:encryption",
            "falcon/test/encryption",
            "protect",
            _context,
            randomness);
        var integrityKey = custody.Generate(
            "key:test:integrity",
            "falcon/test/integrity",
            "authenticate",
            _context,
            randomness);
        var context = CreateDomainContext("falcon/test/encryption", "protect", 1);
        var nonce = randomness.Produce(RandomRequest("nonce:crypto:1", "test-nonce", 16)).Material![..12];
        var plaintext = "synthetic-stage0b-payload"u8.ToArray();
        CryptoCandidateResult? encrypted = null;

        Check("VPL-BST-005-V03", "VPL-BST-005", "VPL-BST-005-REQ-001", crypto.CandidateId, () =>
        {
            encrypted = crypto.Execute(CryptoRequest(
                "crypto:encrypt",
                "encrypt",
                "falcon/test/encryption",
                "protect",
                encryptionKey,
                plaintext,
                nonce,
                context));
            Equal(CandidateDisposition.Succeeded, encrypted.Disposition);
            NotNull(encrypted.PublicOrProtectedOutput);
            var decrypted = crypto.Execute(CryptoRequest(
                "crypto:decrypt",
                "decrypt",
                "falcon/test/encryption",
                "protect",
                encryptionKey,
                encrypted.PublicOrProtectedOutput!,
                nonce,
                context));
            Equal(CandidateDisposition.Succeeded, decrypted.Disposition);
            SequenceEqual(plaintext, decrypted.PublicOrProtectedOutput!);
        });

        Check("VPL-BST-005-V04", "VPL-BST-005", "VPL-BST-005-REQ-002", crypto.CandidateId, () =>
        {
            var wrongDomain = crypto.Execute(CryptoRequest(
                "crypto:wrong-domain",
                "encrypt",
                "falcon/test/integrity",
                "authenticate",
                encryptionKey,
                plaintext,
                randomness.Produce(RandomRequest("nonce:wrong", "test-nonce", 16)).Material![..12],
                context));
            Equal(CandidateDisposition.Rejected, wrongDomain.Disposition);
            var deniedContext = _context with { GuardianPermits = false };
            var denied = crypto.Execute(CryptoRequest(
                "crypto:guardian",
                "encrypt",
                "falcon/test/encryption",
                "protect",
                encryptionKey,
                plaintext,
                randomness.Produce(RandomRequest("nonce:guardian", "test-nonce", 16)).Material![..12],
                context,
                deniedContext));
            Equal(CandidateDisposition.Rejected, denied.Disposition);
        });

        Check("VPL-BST-005-V05", "VPL-BST-005", "VPL-BST-005-REQ-005", crypto.CandidateId, () =>
        {
            var reused = crypto.Execute(CryptoRequest(
                "crypto:nonce-reuse",
                "encrypt",
                "falcon/test/encryption",
                "protect",
                encryptionKey,
                plaintext,
                nonce,
                context));
            Equal("nonce_reuse_rejected", reused.Reason);
        });

        Check("VPL-BST-005-V06", "VPL-BST-005", "VPL-BST-005-REQ-008", crypto.CandidateId, () =>
        {
            var tampered = encrypted!.PublicOrProtectedOutput!.ToArray();
            tampered[^1] ^= 0x01;
            var result = crypto.Execute(CryptoRequest(
                "crypto:tamper",
                "decrypt",
                "falcon/test/encryption",
                "protect",
                encryptionKey,
                tampered,
                nonce,
                context));
            Equal(CandidateDisposition.Rejected, result.Disposition);
            Null(result.PublicOrProtectedOutput);
        });

        Check("VPL-BST-005-V07", "VPL-BST-005", "VPL-BST-005-REQ-003", crypto.CandidateId, () =>
        {
            var encryptionContext = CreateDomainContext("falcon/test/encryption", "protect", 1);
            var integrityContext = CreateDomainContext("falcon/test/integrity", "authenticate", 1);
            False(encryptionContext.AsSpan().SequenceEqual(integrityContext));
            var crossUse = crypto.Execute(CryptoRequest(
                "crypto:cross-use",
                "sign",
                "falcon/test/integrity",
                "authenticate",
                encryptionKey,
                plaintext,
                null,
                integrityContext));
            Equal(CandidateDisposition.Rejected, crossUse.Disposition);
        });

        Check("VPL-BST-005-V08", "VPL-BST-005", "VPL-BST-005-REQ-001", crypto.CandidateId, () =>
        {
            var integrityContext = CreateDomainContext("falcon/test/integrity", "authenticate", 1);
            var signed = crypto.Execute(CryptoRequest(
                "crypto:sign",
                "sign",
                "falcon/test/integrity",
                "authenticate",
                integrityKey,
                plaintext,
                null,
                integrityContext));
            Equal(CandidateDisposition.Succeeded, signed.Disposition);
            var verificationInput = plaintext.Concat(signed.PublicOrProtectedOutput!).ToArray();
            var verified = crypto.Execute(CryptoRequest(
                "crypto:verify",
                "verify",
                "falcon/test/integrity",
                "authenticate",
                integrityKey,
                verificationInput,
                null,
                integrityContext));
            Equal(CandidateDisposition.Succeeded, verified.Disposition);
        });

        using var secrets = new SecretProviderCandidate(randomness);
        var secret = secrets.Create("secret:test:1", "falcon/test/integrity", "authenticate", _context);

        Check("VPL-BST-005-V09", "VPL-BST-005", "VPL-BST-005-REQ-004", secrets.CandidateId, () =>
        {
            var used = secrets.Use(new SecretUseRequest(
                "secret:use:1",
                secret,
                "compute-test-integrity",
                "authenticate",
                "falcon/test/integrity",
                _context.Environment,
                _context), plaintext);
            Equal(CandidateDisposition.Succeeded, used.Disposition);
            NotNull(used.BoundedOutput);
            Throws<CandidateBoundaryException>(() => secrets.Enumerate(false));
            False(used.Evidence.Claims.ContainsKey("secret"));
        });

        Check("VPL-BST-005-V10", "VPL-BST-005", "VPL-BST-005-REQ-005", secrets.CandidateId, () =>
        {
            var rotated = secrets.Rotate(secret, _context);
            NotEqual(secret.Version, rotated.Version);
            var oldUse = secrets.Use(new SecretUseRequest(
                "secret:old",
                secret,
                "compute-test-integrity",
                "authenticate",
                "falcon/test/integrity",
                _context.Environment,
                _context), plaintext);
            Equal(CandidateDisposition.Rejected, oldUse.Disposition);
            var revoked = secrets.Revoke(rotated);
            var revokedUse = secrets.Use(new SecretUseRequest(
                "secret:revoked",
                revoked,
                "compute-test-integrity",
                "authenticate",
                "falcon/test/integrity",
                _context.Environment,
                _context), plaintext);
            Equal(CandidateDisposition.Rejected, revokedUse.Disposition);
        });

        Check("VPL-BST-005-V11", "VPL-BST-005", "VPL-BST-005-REQ-009", crypto.CandidateId, () =>
        {
            False(crypto.IsOperational);
            False(crypto.CanSelfActivate);
            Equal("TEST_ONLY", randomness.Produce(RandomRequest("random:isolation", "test-salt", 16)).Classification);
        });
    }

    private void VerifyCertificateCandidate()
    {
        var observed = new DateTimeOffset(2026, 7, 26, 14, 0, 0, TimeSpan.Zero);
        var timeProvider = new TimeProviderCandidate(new MutableClockSource(CreateSample(observed)));
        var time = timeProvider.Observe(TimeRequest("cert:time"));
        using var fixture = SyntheticCertificateFixture.Create(
            "certificate:test:1",
            "stage0b.test.identity",
            _context.Environment,
            "candidate-authentication",
            "server-auth",
            observed.AddMinutes(-10),
            observed.AddMinutes(10),
            _context);
        using var provider = new CertificateAndIdentityProviderCandidate();
        var reference = provider.AdmitTestIdentity(fixture, _context);

        Check("VPL-BST-005-V12", "VPL-BST-005", "VPL-BST-005-REQ-006", provider.CandidateId, () =>
        {
            var result = provider.Validate(CertificateRequest(
                "certificate:valid",
                reference,
                "stage0b.test.identity",
                time));
            Equal(CertificateValidationDisposition.Valid, result.Disposition);
            Equal("VALID", result.ChainDisposition);
            False(string.IsNullOrWhiteSpace(result.CertificateDigest));
        });

        Check("VPL-BST-005-V13", "VPL-BST-005", "VPL-BST-005-REQ-006", provider.CandidateId, () =>
        {
            var wrongSubject = provider.Validate(CertificateRequest(
                "certificate:wrong-subject",
                reference,
                "other.identity",
                time));
            Equal(CertificateValidationDisposition.Rejected, wrongSubject.Disposition);
            provider.Revoke(reference);
            var revoked = provider.Validate(CertificateRequest(
                "certificate:revoked",
                reference,
                "stage0b.test.identity",
                time));
            Equal(CertificateValidationDisposition.Invalid, revoked.Disposition);
        });

        Check("VPL-BST-005-V14", "VPL-BST-005", "VPL-BST-005-REQ-011", provider.CandidateId, () =>
        {
            False(provider.IsOperational);
            False(provider.CanSelfActivate);
        });
    }

    private void VerifyTraceAndPipelineCandidates()
    {
        var links = _results.Select(result => new TraceLink(
            result.RequirementId,
            result.CandidateId,
            result.VerificationId,
            result.Plan));
        var trace = new MachineReadableTraceCandidate(links);

        Check("TRC-001-V01", "TRC-001", "TRACE-COVERAGE", trace.CandidateId, () =>
        {
            True(trace.Covers("VPL-BST-003-REQ-001"));
            True(trace.Covers("VPL-BST-004-REQ-001"));
            True(trace.Covers("VPL-BST-005-REQ-001"));
            False(trace.IsOperational);
        });

        var fixtures = new IsolatedVerificationFixturesCandidate();
        Check("VPL-BST-005-V15", "VPL-BST-005", "VPL-BST-005-REQ-009", fixtures.CandidateId, () =>
        {
            IsolatedVerificationFixturesCandidate.RequireSynthetic("TEST_ONLY");
            Throws<CandidateBoundaryException>(() =>
                IsolatedVerificationFixturesCandidate.RequireSynthetic("PRODUCTION"));
        });

        var pipeline = new BootstrapPipelineHarnessCandidate();
        Check("PIPE-001-V01", "PIPE-001", "PIPE-CANDIDATE-ISOLATION", pipeline.CandidateId, () =>
        {
            var evidence = CandidateEvidence.Create(
                "pipeline:test:evidence",
                "CND-FCE-001",
                "pipeline.synthetic",
                CandidateDisposition.Succeeded,
                "candidate_observation_only",
                ("classification", "CANDIDATE"),
                ("operational", "false"));
            var cases = new[]
            {
                new CandidateVerificationCase(
                    "pipeline:test:1",
                    "CND-FCE-001",
                    "FCE-001-REQ-001",
                    () => evidence)
            };
            var output = pipeline.Run(cases, _context);
            Equal(1, output.Count);
            False(pipeline.IsOperational);
            False(pipeline.CanSelfActivate);
        });
    }

    private void Check(
        string verificationId,
        string plan,
        string requirementId,
        string candidateId,
        Action verification)
    {
        try
        {
            verification();
            _results.Add(new VerificationResult(
                verificationId,
                plan,
                requirementId,
                candidateId,
                true,
                "PASS"));
        }
        catch (Exception exception)
        {
            _results.Add(new VerificationResult(
                verificationId,
                plan,
                requirementId,
                candidateId,
                false,
                exception.GetType().Name + ":" + exception.Message));
        }
    }

    private IdentifierRequest IdentifierRequestFor(
        string requestId,
        string identifierClass,
        string subject) =>
        new(
            requestId,
            identifierClass,
            "FALCON-ID-CANDIDATE-UUID7",
            subject,
            "stage0b",
            _context.Environment,
            "internal-candidate",
            true,
            false,
            _context);

    private TimeObservationRequest TimeRequest(string requestId) =>
        new(
            requestId,
            ClockQuality.Verified,
            100,
            TimeSpan.FromMinutes(30),
            ClockCapabilities.Utc | ClockCapabilities.Monotonic | ClockCapabilities.Uncertainty,
            _context);

    private static ControlledClockSample CreateSample(DateTimeOffset observed) =>
        new(
            observed,
            1000,
            "clock:synthetic:1",
            "epoch:synthetic:1",
            1,
            50,
            observed.AddMinutes(-1),
            ClockQuality.Verified,
            ClockCapabilities.Utc |
            ClockCapabilities.Monotonic |
            ClockCapabilities.Uncertainty |
            ClockCapabilities.VerificationAge,
            true,
            false,
            false);

    private RandomnessRequest RandomRequest(string requestId, string purpose, int length) =>
        new(
            requestId,
            "FALCON-RANDOM-CANDIDATE-1",
            purpose,
            length,
            "falcon/test/material",
            _context.Environment,
            false,
            _context);

    private CryptoOperationRequest CryptoRequest(
        string requestId,
        string operation,
        string domain,
        string purpose,
        OpaqueKeyReference reference,
        byte[] input,
        byte[]? nonce,
        byte[] canonicalContext,
        CandidateContext? context = null) =>
        new(
            requestId,
            operation,
            "FALCON-CRYPTO-CANDIDATE-1",
            domain,
            purpose,
            reference,
            input,
            nonce,
            canonicalContext,
            context ?? _context);

    private CertificateValidationRequest CertificateRequest(
        string requestId,
        CertificateReference reference,
        string expectedSubject,
        TimeCandidateObservation time) =>
        new(
            requestId,
            reference,
            expectedSubject,
            "candidate-authentication",
            _context.Environment,
            "server-auth",
            time,
            _context);

    private static byte[] CreateDomainContext(string domain, string purpose, uint keyVersion) =>
        FalconCanonicalEncoding.CryptographicDomainContext(
            Guid.Parse("018f7e32-7b10-7abc-8def-0123456789ab"),
            CandidateContext.ApprovedEnvironment,
            "stage0b-local-instance",
            domain,
            purpose,
            "FALCON-CRYPTO-CANDIDATE-1",
            1,
            "AES-256-GCM-HMAC-SHA256-TEST",
            keyVersion);

    private static void True(bool value)
    {
        if (!value)
        {
            throw new VerificationException("Expected true.");
        }
    }

    private static void False(bool value) => True(!value);

    private static void Null(object? value)
    {
        if (value is not null)
        {
            throw new VerificationException("Expected null.");
        }
    }

    private static void NotNull(object? value)
    {
        if (value is null)
        {
            throw new VerificationException("Expected non-null.");
        }
    }

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

internal sealed class VerificationException(string message) : Exception(message);

internal sealed class AdvancingIdentifierMaterial : IControlledIdentifierMaterial
{
    private int _counter;
    public DateTimeOffset UtcNow { get; } = new(2026, 7, 26, 12, 0, 0, TimeSpan.Zero);
    public bool Available => true;

    public void Fill(Span<byte> destination)
    {
        var counter = Interlocked.Increment(ref _counter);
        var seed = SHA256.HashData(Encoding.UTF8.GetBytes($"identifier-material:{counter}"));
        seed.AsSpan(0, destination.Length).CopyTo(destination);
    }
}

internal sealed class CollisionIdentifierMaterial : IControlledIdentifierMaterial
{
    private int _index;
    private static readonly byte[] Sequence = [1, 9, 2, 9];
    public DateTimeOffset UtcNow { get; } = new(2026, 7, 26, 12, 0, 0, TimeSpan.Zero);
    public bool Available => true;

    public void Fill(Span<byte> destination)
    {
        destination.Fill(Sequence[Math.Min(_index, Sequence.Length - 1)]);
        _index++;
    }
}

internal sealed class UnavailableIdentifierMaterial : IControlledIdentifierMaterial
{
    public DateTimeOffset UtcNow => throw new InvalidOperationException("Unavailable.");
    public bool Available => false;
    public void Fill(Span<byte> destination) => throw new InvalidOperationException("Unavailable.");
}

internal sealed class MutableClockSource(ControlledClockSample sample) : IControlledClockSource
{
    public ControlledClockSample Sample { get; set; } = sample;
    public ControlledClockSample Read() => Sample;
}

internal sealed class AdvancingEntropySource : ITestEntropySource
{
    private int _counter;
    public bool Available => true;
    public string SourceClass => "SYNTHETIC_DETERMINISTIC_TEST_SOURCE";

    public void Fill(Span<byte> destination)
    {
        var written = 0;
        while (written < destination.Length)
        {
            var counter = Interlocked.Increment(ref _counter);
            var block = SHA256.HashData(Encoding.UTF8.GetBytes($"stage0b-test-entropy:{counter}"));
            var length = Math.Min(block.Length, destination.Length - written);
            block.AsSpan(0, length).CopyTo(destination[written..]);
            written += length;
        }
    }
}

internal sealed class UnavailableEntropySource : ITestEntropySource
{
    public bool Available => false;
    public string SourceClass => "UNAVAILABLE_TEST_SOURCE";
    public void Fill(Span<byte> destination) => throw new InvalidOperationException("Unavailable.");
}
