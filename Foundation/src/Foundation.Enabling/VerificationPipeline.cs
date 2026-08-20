using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Enabling;

public static class FoundationDigest
{
    public static bool IsCanonicalSha256(string? digest)
    {
        if (digest is null || digest.Length != 64)
        {
            return false;
        }

        foreach (var value in digest)
        {
            var uppercaseHex = value is >= '0' and <= '9' or >= 'A' and <= 'F';
            if (!uppercaseHex)
            {
                return false;
            }
        }

        return true;
    }

    public static string ComputeSha256(string? canonicalContent)
    {
        if (canonicalContent is null)
        {
            throw new FoundationBoundaryException("canonical_content_required");
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalContent)));
    }

    public static bool MatchesCanonicalContent(string? canonicalContent, string? admittedDigest)
    {
        if (canonicalContent is null || !IsCanonicalSha256(admittedDigest))
        {
            return false;
        }

        var computed = Encoding.ASCII.GetBytes(ComputeSha256(canonicalContent));
        var admitted = Encoding.ASCII.GetBytes(admittedDigest!);
        try
        {
            return CryptographicOperations.FixedTimeEquals(computed, admitted);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(computed);
            CryptographicOperations.ZeroMemory(admitted);
        }
    }
}

public sealed record TraceEntry(
    string RequirementId,
    string SourcePath,
    int SourceLine,
    string SourceDigest);

public sealed class FoundationTraceCatalog
{
    private readonly IReadOnlyDictionary<string, TraceEntry> _entries;
    private readonly IReadOnlyCollection<TraceEntry> _orderedEntries;

    public FoundationTraceCatalog(IEnumerable<TraceEntry>? entries)
    {
        if (entries is null)
        {
            throw new FoundationBoundaryException("trace_catalog_required");
        }

        var values = new SortedDictionary<string, TraceEntry>(StringComparer.Ordinal);
        foreach (var entry in entries)
        {
            if (entry is null ||
                string.IsNullOrWhiteSpace(entry.RequirementId) ||
                string.IsNullOrWhiteSpace(entry.SourcePath) ||
                entry.SourceLine <= 0 ||
                !FoundationDigest.IsCanonicalSha256(entry.SourceDigest) ||
                !values.TryAdd(entry.RequirementId, entry))
            {
                throw new FoundationBoundaryException("trace_entry_rejected");
            }
        }

        _entries = new ReadOnlyDictionary<string, TraceEntry>(values);
        _orderedEntries = new ReadOnlyCollection<TraceEntry>(values.Values.ToArray());
    }

    public int Count => _entries.Count;

    public bool Contains(string? requirementId) =>
        !string.IsNullOrWhiteSpace(requirementId) && _entries.ContainsKey(requirementId);

    public IReadOnlyCollection<TraceEntry> Entries => _orderedEntries;
}

public enum EvidenceRequirementClass
{
    Mandatory,
    Optional,
    Conditional,
    Excluded,
    Derived
}

public sealed record EvidenceRequirement(
    string RequirementId,
    EvidenceRequirementClass Classification);

public sealed record ObservedEvidence(
    string RequirementId,
    string EvidenceId,
    string CanonicalContent,
    string Digest)
{
    public static ObservedEvidence Create(
        string requirementId,
        string evidenceId,
        string canonicalContent)
        => new(
            requirementId,
            evidenceId,
            canonicalContent,
            FoundationDigest.ComputeSha256(canonicalContent));

    public bool IntegrityValid =>
        FoundationDigest.MatchesCanonicalContent(CanonicalContent, Digest);
}

public sealed record RootEvidenceSet(
    string RootEvidenceSetId,
    string RequirementSetId,
    IReadOnlyList<EvidenceRequirement> Requirements,
    IReadOnlyList<ObservedEvidence> Evidence,
    string EvaluationContextId,
    string ProducerAuthority,
    string CompletenessAuthority,
    bool DirectSessionPromotion,
    bool GateWeakened);

public enum EvidenceCompleteness
{
    Complete,
    Partial,
    Incomplete,
    Invalid
}

public sealed class FoundationEvidenceGate
{
    public EvidenceCompleteness Evaluate(RootEvidenceSet? set)
    {
        if (set is null ||
            string.IsNullOrWhiteSpace(set.RootEvidenceSetId) ||
            !set.RootEvidenceSetId.StartsWith("RVES-", StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(set.RequirementSetId) ||
            !set.RequirementSetId.StartsWith("ERS-", StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(set.EvaluationContextId) ||
            !set.EvaluationContextId.StartsWith("ECTX-", StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(set.ProducerAuthority) ||
            string.IsNullOrWhiteSpace(set.CompletenessAuthority) ||
            StringComparer.Ordinal.Equals(set.ProducerAuthority, set.CompletenessAuthority) ||
            set.DirectSessionPromotion ||
            set.GateWeakened ||
            set.Requirements is null ||
            set.Evidence is null)
        {
            return EvidenceCompleteness.Invalid;
        }

        var requirements = new Dictionary<string, EvidenceRequirementClass>(StringComparer.Ordinal);
        foreach (var requirement in set.Requirements)
        {
            if (requirement is null ||
                string.IsNullOrWhiteSpace(requirement.RequirementId) ||
                !requirements.TryAdd(requirement.RequirementId, requirement.Classification))
            {
                return EvidenceCompleteness.Invalid;
            }
        }

        var evidenceIds = new HashSet<string>(StringComparer.Ordinal);
        var observedRequirements = new HashSet<string>(StringComparer.Ordinal);
        foreach (var evidence in set.Evidence)
        {
            if (evidence is null ||
                string.IsNullOrWhiteSpace(evidence.RequirementId) ||
                string.IsNullOrWhiteSpace(evidence.EvidenceId) ||
                evidence.CanonicalContent is null ||
                !requirements.ContainsKey(evidence.RequirementId) ||
                !evidenceIds.Add(evidence.EvidenceId) ||
                !observedRequirements.Add(evidence.RequirementId) ||
                !evidence.IntegrityValid)
            {
                return EvidenceCompleteness.Invalid;
            }
        }

        var mandatory = requirements
            .Where(item => item.Value == EvidenceRequirementClass.Mandatory)
            .Select(item => item.Key)
            .ToHashSet(StringComparer.Ordinal);

        return mandatory.IsSubsetOf(observedRequirements)
            ? EvidenceCompleteness.Complete
            : EvidenceCompleteness.Incomplete;
    }
}

public sealed record FoundationEnvironmentProfile(
    string EnvironmentId,
    string DeploymentProfileId,
    string Platform,
    string Purpose,
    bool NetworkDenied,
    bool CloudDenied,
    bool FinancialDenied,
    IReadOnlyList<string> ActiveProviderProfiles,
    IReadOnlyList<string> NonAuthorities)
{
    private static readonly IReadOnlySet<string> RequiredProviderProfiles =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "FALCON-RANDOM-WINDOWS-CSPRNG-1",
            "FALCON-TIME-WINDOWS-LOCAL-BUILD-1",
            "FALCON-ID-UUID7-1",
            "FALCON-CRYPTO-BCL-1",
            "FALCON-SECRET-EPHEMERAL-1",
            "FALCON-CERT-LOCAL-TRUST-1"
        };

    private static readonly IReadOnlySet<string> RequiredNonAuthorities =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "NO_STAGE_1",
            "NO_PRODUCTION",
            "NO_CLOUD",
            "NO_FINANCIAL_AUTHORITY"
        };

    public bool IsValid =>
        StringComparer.Ordinal.Equals(EnvironmentId, FoundationBoundary.Environment) &&
        StringComparer.Ordinal.Equals(DeploymentProfileId, FoundationBoundary.DeploymentProfile) &&
        StringComparer.Ordinal.Equals(Platform, "windows") &&
        StringComparer.Ordinal.Equals(Purpose, FoundationBoundary.Classification) &&
        NetworkDenied &&
        CloudDenied &&
        FinancialDenied &&
        HasExactUniqueSet(ActiveProviderProfiles, RequiredProviderProfiles) &&
        HasExactUniqueSet(NonAuthorities, RequiredNonAuthorities);

    private static bool HasExactUniqueSet(
        IReadOnlyList<string>? actual,
        IReadOnlySet<string> required)
    {
        if (actual is null || actual.Count != required.Count)
        {
            return false;
        }

        var unique = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in actual)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                !required.Contains(value) ||
                !unique.Add(value))
            {
                return false;
            }
        }

        return unique.SetEquals(required);
    }
}
