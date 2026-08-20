using System.Collections.ObjectModel;

namespace Falcon.Stage0B.Candidates;

public enum CandidateLifecycle
{
    Candidate,
    Restricted,
    Suspended,
    Retired,
    Forbidden
}

public enum CandidateDisposition
{
    Succeeded,
    Rejected,
    Failed
}

public sealed record CandidateContext(
    string AuthorityDecision,
    string Environment,
    string RuntimeEpochId,
    string RequesterId,
    bool GuardianPermits,
    string EvidenceReference)
{
    public const string ApprovedAuthority = "GOV-051";
    public const string ApprovedEnvironment = "STAGE_0B_WINDOWS_CANDIDATE";

    public bool IsAuthorized =>
        StringComparer.Ordinal.Equals(AuthorityDecision, ApprovedAuthority) &&
        StringComparer.Ordinal.Equals(Environment, ApprovedEnvironment) &&
        GuardianPermits &&
        !string.IsNullOrWhiteSpace(RuntimeEpochId) &&
        !string.IsNullOrWhiteSpace(RequesterId) &&
        !string.IsNullOrWhiteSpace(EvidenceReference);
}

public sealed record CandidateEvidence(
    string EvidenceId,
    string CandidateId,
    string Operation,
    CandidateDisposition Disposition,
    string Reason,
    IReadOnlyDictionary<string, string> Claims)
{
    public static CandidateEvidence Create(
        string evidenceId,
        string candidateId,
        string operation,
        CandidateDisposition disposition,
        string reason,
        params (string Key, string Value)[] claims)
    {
        var values = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in claims)
        {
            values.Add(key, value);
        }

        return new CandidateEvidence(
            evidenceId,
            candidateId,
            operation,
            disposition,
            reason,
            new ReadOnlyDictionary<string, string>(values));
    }
}

public interface ICandidateProvider
{
    string CandidateId { get; }
    CandidateLifecycle Lifecycle { get; }
    bool IsOperational { get; }
    bool CanSelfActivate { get; }
}

public abstract class CandidateProviderBase(string candidateId) : ICandidateProvider
{
    public string CandidateId { get; } = candidateId;
    public CandidateLifecycle Lifecycle => CandidateLifecycle.Candidate;
    public bool IsOperational => false;
    public bool CanSelfActivate => false;

    protected static bool IsKnownToken(string value, params string[] allowed) =>
        allowed.Contains(value, StringComparer.Ordinal);

    protected CandidateEvidence Reject(
        string evidenceId,
        string operation,
        string reason,
        params (string Key, string Value)[] claims) =>
        CandidateEvidence.Create(
            evidenceId,
            CandidateId,
            operation,
            CandidateDisposition.Rejected,
            reason,
            claims);

    protected CandidateEvidence Succeed(
        string evidenceId,
        string operation,
        params (string Key, string Value)[] claims) =>
        CandidateEvidence.Create(
            evidenceId,
            CandidateId,
            operation,
            CandidateDisposition.Succeeded,
            "candidate_observation_only",
            claims);
}

public sealed class CandidateBoundaryException(string message) : InvalidOperationException(message);
