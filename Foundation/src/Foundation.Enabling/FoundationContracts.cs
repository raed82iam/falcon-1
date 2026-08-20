using System.Collections.ObjectModel;

namespace Foundation.Enabling;

public static class FoundationBoundary
{
    public const string Authority = "GOV-058";
    public const string Environment = "FALCON-FOUNDATION-WINDOWS-LOCAL-1";
    public const string DeploymentProfile = "FALCON-LOCAL-WINDOWS-FOUNDATION-1";
    public const string Classification = "FOUNDATION_VERIFICATION_ONLY";
}

public sealed record FoundationAuthorityContext(
    string AuthorityDecision,
    string EnvironmentId,
    string DeploymentProfileId,
    string RuntimeEpochId,
    string RequesterId,
    string EvidenceSetId,
    bool ProtectivePermission)
{
    public bool IsValid =>
        StringComparer.Ordinal.Equals(AuthorityDecision, FoundationBoundary.Authority) &&
        StringComparer.Ordinal.Equals(EnvironmentId, FoundationBoundary.Environment) &&
        StringComparer.Ordinal.Equals(DeploymentProfileId, FoundationBoundary.DeploymentProfile) &&
        !string.IsNullOrWhiteSpace(RuntimeEpochId) &&
        RuntimeEpochId.StartsWith("epoch:", StringComparison.Ordinal) &&
        !string.IsNullOrWhiteSpace(RequesterId) &&
        !string.IsNullOrWhiteSpace(EvidenceSetId) &&
        EvidenceSetId.StartsWith("RVES-", StringComparison.Ordinal) &&
        ProtectivePermission;
}

public enum FoundationDisposition
{
    Succeeded,
    Rejected,
    Failed
}

public enum FoundationLifecycle
{
    ActiveScoped,
    Restricted,
    Suspended,
    Revoked,
    Expired
}

public sealed record FoundationEvidence(
    string EvidenceId,
    string SubjectId,
    string Operation,
    FoundationDisposition Disposition,
    string Reason,
    IReadOnlyDictionary<string, string> Claims)
{
    public static FoundationEvidence Create(
        string evidenceId,
        string subjectId,
        string operation,
        FoundationDisposition disposition,
        string reason,
        params (string Key, string Value)[] claims)
    {
        var values = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in claims)
        {
            values.Add(key, value);
        }

        return new FoundationEvidence(
            evidenceId,
            subjectId,
            operation,
            disposition,
            reason,
            new ReadOnlyDictionary<string, string>(values));
    }
}

public interface IFoundationProvider
{
    string SubjectId { get; }
    string ProfileId { get; }
    FoundationLifecycle Lifecycle { get; }
    bool CanSelfActivate { get; }
}

public abstract class FoundationProviderBase(string subjectId, string profileId) : IFoundationProvider
{
    public string SubjectId { get; } = subjectId;
    public string ProfileId { get; } = profileId;
    public FoundationLifecycle Lifecycle { get; private set; } = FoundationLifecycle.ActiveScoped;
    public bool CanSelfActivate => false;

    public void ApplyRestriction(FoundationLifecycle state)
    {
        if (state == FoundationLifecycle.ActiveScoped)
        {
            throw new FoundationBoundaryException("self_restoration_rejected");
        }

        Lifecycle = Stronger(Lifecycle, state);
    }

    internal bool IsUsable(FoundationAuthorityContext? context) =>
        Lifecycle == FoundationLifecycle.ActiveScoped && context?.IsValid == true;

    private static FoundationLifecycle Stronger(FoundationLifecycle current, FoundationLifecycle requested) =>
        Rank(requested) > Rank(current) ? requested : current;

    private static int Rank(FoundationLifecycle lifecycle) => lifecycle switch
    {
        FoundationLifecycle.ActiveScoped => 0,
        FoundationLifecycle.Restricted => 1,
        FoundationLifecycle.Suspended => 2,
        FoundationLifecycle.Expired => 3,
        FoundationLifecycle.Revoked => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(lifecycle))
    };
}

public sealed class FoundationBoundaryException(string message) : InvalidOperationException(message);
