using System.Collections.ObjectModel;

namespace Falcon.Stage0B.Candidates;

public sealed record CandidateVerificationCase(
    string VerificationId,
    string CandidateId,
    string RequirementId,
    Func<CandidateEvidence> Execute);

public sealed record CandidateVerificationCaseResult(
    string VerificationId,
    string CandidateId,
    string RequirementId,
    CandidateDisposition Disposition,
    CandidateEvidence Evidence);

public sealed class BootstrapPipelineHarnessCandidate : CandidateProviderBase
{
    public BootstrapPipelineHarnessCandidate()
        : base("CND-PIPE-001")
    {
    }

    public IReadOnlyList<CandidateVerificationCaseResult> Run(
        IEnumerable<CandidateVerificationCase> cases,
        CandidateContext context)
    {
        if (!context.IsAuthorized)
        {
            throw new CandidateBoundaryException("pipeline_context_rejected");
        }

        var results = new List<CandidateVerificationCaseResult>();
        foreach (var verificationCase in cases.OrderBy(item => item.VerificationId, StringComparer.Ordinal))
        {
            if (!verificationCase.CandidateId.StartsWith("CND-", StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(verificationCase.RequirementId))
            {
                throw new CandidateBoundaryException("pipeline_case_scope_rejected");
            }

            var evidence = verificationCase.Execute();
            results.Add(new CandidateVerificationCaseResult(
                verificationCase.VerificationId,
                verificationCase.CandidateId,
                verificationCase.RequirementId,
                evidence.Disposition,
                evidence));
        }

        return new ReadOnlyCollection<CandidateVerificationCaseResult>(results);
    }
}

public sealed class IsolatedVerificationFixturesCandidate : CandidateProviderBase
{
    public IsolatedVerificationFixturesCandidate()
        : base("CND-FIX-001")
    {
    }

    public static void RequireSynthetic(string classification)
    {
        if (!StringComparer.Ordinal.Equals(classification, "TEST_ONLY") &&
            !StringComparer.Ordinal.Equals(classification, "CANDIDATE"))
        {
            throw new CandidateBoundaryException("non_synthetic_fixture_rejected");
        }
    }
}
