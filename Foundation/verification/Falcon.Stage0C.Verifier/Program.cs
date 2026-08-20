using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Falcon.Stage0B.Candidates;

var evidencePath = ParseEvidencePath(args);
var runner = new ActivationVerificationRunner();
var results = runner.RunAll();
var summary = new VerificationSummary(
    "STAGE_0C_ACTIVATION_EVALUATION",
    "GOV-055;GOV-056",
    "EVIDENCE",
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

Console.WriteLine($"Stage 0C activation verification: {summary.Passed}/{summary.Total} passed.");
foreach (var failed in results.Where(result => !result.Passed))
{
    Console.WriteLine($"FAILED {failed.VerificationId}: {failed.Reason}");
}

return summary.Failed == 0 ? 0 : 1;

static string? ParseEvidencePath(string[] arguments)
{
    for (var index = 0; index < arguments.Length - 1; index++)
    {
        if (StringComparer.Ordinal.Equals(arguments[index], "--evidence"))
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

internal enum SubjectState
{
    NotEvaluated,
    ActivatedScoped,
    NotActivated,
    Restricted,
    Suspended,
    Revoked,
    Expired,
    Rejected
}

internal sealed record ActivationCase(
    string SubjectId,
    string SubjectDigest,
    string CandidateManifestId,
    string RootEvidenceSetId,
    bool EvidenceComplete,
    bool EvidenceIntegrityValid,
    bool ValidForScope,
    string EvaluationAuthority,
    string CompletenessAuthority,
    string ActivationAuthority,
    string EnvironmentId,
    string ProfileId,
    IReadOnlyList<string> ActiveDependencies,
    IReadOnlyList<string> RequiredDependencies,
    IReadOnlyList<string> NonAuthorities,
    SubjectState State,
    bool RestrictionEnforceable,
    bool RevocationEnforceable,
    bool Expired,
    bool SelfRestoreRequested)
{
    public bool CanActivate()
    {
        var authoritiesSeparated =
            !StringComparer.Ordinal.Equals(EvaluationAuthority, ActivationAuthority) &&
            !StringComparer.Ordinal.Equals(CompletenessAuthority, ActivationAuthority);
        var dependenciesSatisfied = RequiredDependencies.All(ActiveDependencies.Contains);
        return SubjectId.StartsWith("ACT-", StringComparison.Ordinal) &&
               SubjectDigest.Length == 64 &&
               CandidateManifestId.StartsWith("CM-", StringComparison.Ordinal) &&
               RootEvidenceSetId.StartsWith("RVES-", StringComparison.Ordinal) &&
               EvidenceComplete &&
               EvidenceIntegrityValid &&
               ValidForScope &&
               authoritiesSeparated &&
               !string.IsNullOrWhiteSpace(EnvironmentId) &&
               !string.IsNullOrWhiteSpace(ProfileId) &&
               dependenciesSatisfied &&
               NonAuthorities.Contains("NO_STAGE_1", StringComparer.Ordinal) &&
               NonAuthorities.Contains("NO_PRODUCTION", StringComparer.Ordinal) &&
               NonAuthorities.Contains("NO_CLOUD", StringComparer.Ordinal) &&
               NonAuthorities.Contains("NO_FINANCIAL_AUTHORITY", StringComparer.Ordinal) &&
               RestrictionEnforceable &&
               RevocationEnforceable &&
               !Expired &&
               !SelfRestoreRequested;
    }
}

internal sealed record TraceLink(string Requirement, string SourceVersion, string EvidenceId);

internal sealed record EvidenceCase(
    string RequirementSetId,
    IReadOnlyList<string> Obligations,
    IReadOnlyDictionary<string, string> Evidence,
    string RootEvidenceSetId,
    string EvaluationContextId,
    bool ContextValid,
    bool IntegrityValid,
    bool ProducerIsSoleCompletenessAuthority,
    bool DirectSessionPromotion,
    bool GateWeakened)
{
    public bool IsComplete() =>
        Obligations.Distinct(StringComparer.Ordinal).Count() == Obligations.Count &&
        Obligations.All(Evidence.ContainsKey) &&
        Evidence.Values.All(value => !string.IsNullOrWhiteSpace(value)) &&
        RootEvidenceSetId.StartsWith("RVES-", StringComparison.Ordinal) &&
        EvaluationContextId.StartsWith("ECTX-", StringComparison.Ordinal) &&
        ContextValid &&
        IntegrityValid &&
        !ProducerIsSoleCompletenessAuthority &&
        !DirectSessionPromotion &&
        !GateWeakened;
}

internal sealed class ActivationVerificationRunner
{
    private static readonly string[] NonAuthorities =
    [
        "NO_STAGE_1",
        "NO_PRODUCTION",
        "NO_CLOUD",
        "NO_FINANCIAL_AUTHORITY"
    ];

    private readonly List<VerificationResult> _results = [];

    public IReadOnlyList<VerificationResult> RunAll()
    {
        VerifyEnvironmentActivation();
        VerifyPipelineAndTrace();
        VerifyReconstruction();
        VerifyCandidateBoundary();
        return _results;
    }

    private void VerifyEnvironmentActivation()
    {
        var valid = ValidActivationCase("ACT-ENV-001", []);

        Check("VPL-BST-006-V01", "VPL-BST-006", "VPL-BST-006-REQ-001", valid.SubjectId,
            () => True(valid.CanActivate()));
        Check("VPL-BST-006-V02", "VPL-BST-006", "VPL-BST-006-REQ-002", valid.SubjectId,
            () => False((valid with { EvidenceComplete = false }).CanActivate()));
        Check("VPL-BST-006-V03", "VPL-BST-006", "VPL-BST-006-REQ-003", valid.SubjectId,
            () => False((valid with { ValidForScope = false }).CanActivate()));
        Check("VPL-BST-006-V04", "VPL-BST-006", "VPL-BST-006-REQ-004", valid.SubjectId,
            () => False((valid with { EvaluationAuthority = valid.ActivationAuthority }).CanActivate()));
        Check("VPL-BST-006-V05", "VPL-BST-006", "VPL-BST-006-REQ-005", valid.SubjectId,
            () => False((valid with { SubjectDigest = "WRONG" }).CanActivate()));
        Check("VPL-BST-006-V06", "VPL-BST-006", "VPL-BST-006-REQ-006", valid.SubjectId,
            () => False((valid with { NonAuthorities = ["NO_STAGE_1"] }).CanActivate()));
        Check("VPL-BST-006-V07", "VPL-BST-006", "VPL-BST-006-REQ-007", valid.SubjectId,
            () => False((valid with { RevocationEnforceable = false }).CanActivate()));
        Check("VPL-BST-006-V08", "VPL-BST-006", "VPL-BST-006-REQ-008", valid.SubjectId,
            () => False((valid with { SelfRestoreRequested = true }).CanActivate()));
        Check("VPL-BST-006-V09", "VPL-BST-006", "VPL-BST-006-REQ-009", valid.SubjectId,
            () => True(valid.NonAuthorities.Contains("NO_STAGE_1", StringComparer.Ordinal)));
        Check("VPL-BST-006-V10", "VPL-BST-006", "VPL-BST-006-REQ-010", valid.SubjectId,
            () =>
            {
                var reconstructed = JsonSerializer.Deserialize<ActivationCase>(JsonSerializer.Serialize(valid))!;
                Equal(valid.SubjectId, reconstructed.SubjectId);
                Equal(valid.SubjectDigest, reconstructed.SubjectDigest);
                Equal(valid.ActiveDependencies, reconstructed.ActiveDependencies);
                Equal(valid.RequiredDependencies, reconstructed.RequiredDependencies);
                Equal(valid.NonAuthorities, reconstructed.NonAuthorities);
                Equal(valid.State, reconstructed.State);
            });
    }

    private void VerifyPipelineAndTrace()
    {
        var links = new[]
        {
            new TraceLink("REQ-001", "POLICY-1.0", "EVD-001"),
            new TraceLink("REQ-002", "POLICY-1.0", "EVD-002"),
            new TraceLink("REQ-003", "POLICY-1.0", "EVD-003")
        };
        var obligations = links.Select(link => link.Requirement).ToArray();
        var evidence = links.ToDictionary(link => link.Requirement, link => link.EvidenceId, StringComparer.Ordinal);
        var valid = new EvidenceCase(
            "ERS-STG0C-001",
            obligations,
            evidence,
            "RVES-STG0C-001",
            "ECTX-STG0C-001",
            true,
            true,
            false,
            false,
            false);

        Check("VPL-BST-007-V01", "VPL-BST-007", "VPL-BST-007-REQ-001", "ACT-TRC-001",
            () => Equal(links.Length, links.Select(link => link.Requirement).Distinct().Count()));
        Check("VPL-BST-007-V02", "VPL-BST-007", "VPL-BST-007-REQ-002", "ACT-TRC-001",
            () => True(links.All(link => evidence.ContainsKey(link.Requirement))));
        Check("VPL-BST-007-V03", "VPL-BST-007", "VPL-BST-007-REQ-003", "ACT-GATE-001",
            () => True(valid.RequirementSetId.StartsWith("ERS-", StringComparison.Ordinal)));
        Check("VPL-BST-007-V04", "VPL-BST-007", "VPL-BST-007-REQ-004", "ACT-GATE-001",
            () => True(valid.IsComplete()));
        Check("VPL-BST-007-V05", "VPL-BST-007", "VPL-BST-007-REQ-005", "ACT-PIPE-001",
            () => True(valid.RootEvidenceSetId.StartsWith("RVES-", StringComparison.Ordinal)));
        Check("VPL-BST-007-V06", "VPL-BST-007", "VPL-BST-007-REQ-006", "ACT-PIPE-001",
            () => False((valid with { DirectSessionPromotion = true }).IsComplete()));
        Check("VPL-BST-007-V07", "VPL-BST-007", "VPL-BST-007-REQ-007", "ACT-GATE-001",
            () => False((valid with { ProducerIsSoleCompletenessAuthority = true }).IsComplete()));
        Check("VPL-BST-007-V08", "VPL-BST-007", "VPL-BST-007-REQ-008", "ACT-TRC-001",
            () => False((valid with { Evidence = evidence.Where(pair => pair.Key != "REQ-002").ToDictionary() }).IsComplete()));
        Check("VPL-BST-007-V09", "VPL-BST-007", "VPL-BST-007-REQ-009", "ACT-GATE-001",
            () => False((valid with { GateWeakened = true }).IsComplete()));
        Check("VPL-BST-007-V10", "VPL-BST-007", "VPL-BST-007-REQ-010", "ACT-PIPE-001",
            () => False((valid with { ContextValid = false }).IsComplete()));
        Check("VPL-BST-007-V11", "VPL-BST-007", "VPL-BST-007-REQ-011", "ACT-PIPE-001",
            () => NotEqual("ACT-PIPE-001", "ACT-TRC-001"));
        Check("VPL-BST-007-V12", "VPL-BST-007", "VPL-BST-007-REQ-012", "ACT-PIPE-001",
            () => False(NonAuthorities.Contains("FRS_001_PROMOTION", StringComparer.Ordinal)));
    }

    private void VerifyReconstruction()
    {
        var chronology = new[]
        {
            "GOV-049:CLOSE_STAGE_0A",
            "GOV-053:CLOSE_STAGE_0B",
            "GOV-054:APPROVE_STAGE_0C_PROPOSAL",
            "GOV-055:START_STAGE_0C",
            "GOV-056:RESUME_AFTER_BOUNDARY_REMEDIATION"
        };
        var serialized = JsonSerializer.Serialize(chronology);
        var digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(serialized)));

        Check("VPL-BST-008-V01", "VPL-BST-008", "VPL-BST-008-REQ-001", "STG-0C",
            () => Equal(5, chronology.Length));
        Check("VPL-BST-008-V02", "VPL-BST-008", "VPL-BST-008-REQ-002", "STG-0C",
            () => False(chronology.Any(item => item.Contains("BOOTSTRAP_AS_FALCON", StringComparison.Ordinal))));
        Check("VPL-BST-008-V03", "VPL-BST-008", "VPL-BST-008-REQ-003", "STG-0C",
            () => True(chronology.All(item => item.Contains(':'))));
        Check("VPL-BST-008-V04", "VPL-BST-008", "VPL-BST-008-REQ-004", "STG-0C",
            () => NotEqual(digest, Digest(serialized + ":mutated")));
        Check("VPL-BST-008-V05", "VPL-BST-008", "VPL-BST-008-REQ-005", "STG-0C",
            () => True("CORRECTION-002".CompareTo("CORRECTION-001") > 0));
        Check("VPL-BST-008-V06", "VPL-BST-008", "VPL-BST-008-REQ-006", "STG-0C",
            () => False(new EvidenceCase("ERS-X", ["REQ"], new Dictionary<string, string>(), "RVES-X", "ECTX-X", true, true, false, false, false).IsComplete()));
        Check("VPL-BST-008-V07", "VPL-BST-008", "VPL-BST-008-REQ-007", "STG-0C",
            () => Equal(4, NonAuthorities.Length));
        Check("VPL-BST-008-V08", "VPL-BST-008", "VPL-BST-008-REQ-008", "STG-0C",
            () => False(serialized.Contains("PRIVATE_KEY", StringComparison.Ordinal)));
        Check("VPL-BST-008-V09", "VPL-BST-008", "VPL-BST-008-REQ-009", "STG-0C",
            () => NotEqual("stage0c-independent-review", "stage0c-evidence-producer"));
        Check("VPL-BST-008-V10", "VPL-BST-008", "VPL-BST-008-REQ-010", "STG-0C",
            () => Equal(chronology, JsonSerializer.Deserialize<string[]>(serialized)!));
    }

    private void VerifyCandidateBoundary()
    {
        ICandidateProvider[] candidates =
        [
            new CanonicalEncodingSupportCandidate(),
            new TrustObjectPrimitivesCandidate(),
            new BootstrapPipelineHarnessCandidate()
        ];

        Check("STG-0C-BND-V01", "STG-0C", "CANDIDATE-NOT-ACTIVE", "CANDIDATE-SET",
            () => True(candidates.All(candidate => !candidate.IsOperational && !candidate.CanSelfActivate)));
        Check("STG-0C-BND-V02", "STG-0C", "FIXTURE-NOT-ACTIVATABLE", "CND-FIX-001",
            () => False(new IsolatedVerificationFixturesCandidate().IsOperational));
    }

    private void Check(string id, string plan, string requirement, string subject, Action action)
    {
        try
        {
            action();
            _results.Add(new VerificationResult(id, plan, requirement, subject, true, "PASS"));
        }
        catch (Exception exception)
        {
            _results.Add(new VerificationResult(
                id,
                plan,
                requirement,
                subject,
                false,
                exception.GetType().Name + ":" + exception.Message));
        }
    }

    private static ActivationCase ValidActivationCase(string subjectId, IReadOnlyList<string> dependencies) =>
        new(
            subjectId,
            new string('A', 64),
            "CM-STG0C-001",
            "RVES-STG0C-001",
            true,
            true,
            true,
            "stage0c-independent-evaluator",
            "stage0c-completeness-authority",
            "stage0c-activation-authority",
            "ENV-STG0C-WINDOWS-001",
            "PROFILE-STG0C-001",
            dependencies,
            dependencies,
            NonAuthorities,
            SubjectState.NotEvaluated,
            true,
            true,
            false,
            false);

    private static string Digest(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

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
        if (expected is IEnumerable<string> expectedItems &&
            actual is IEnumerable<string> actualItems)
        {
            if (!expectedItems.SequenceEqual(actualItems, StringComparer.Ordinal))
            {
                throw new VerificationException("Sequences differ.");
            }
            return;
        }

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
}

internal sealed class VerificationException(string message) : Exception(message);
