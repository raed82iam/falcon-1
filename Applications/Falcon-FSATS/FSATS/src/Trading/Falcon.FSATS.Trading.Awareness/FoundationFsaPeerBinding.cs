namespace Falcon.FSATS.Trading.Awareness;

public static class FoundationFsaPeerBinding
{
    public const string FoundationCandidate = "91da7869e7e16e943c92620ed0e8bb0fe7409459";
    public const string DestinationFsaId = "fsa:primary";
    public const string InterfaceType = "Foundation.SelfAwareness.FsaPeerInterfaceRuntime";
    public const string SubmissionType = "Foundation.SelfAwareness.FsaPeerSubmission";
    public const string DecisionType = "Foundation.SelfAwareness.FsaPeerSubmissionDecision";

    public static FsaPeerBindingDecision Evaluate(FsaPeerBindingRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.SourceScopeId) || string.IsNullOrWhiteSpace(request.SourceAwarenessId) ||
            string.IsNullOrWhiteSpace(request.CandidateId) || string.IsNullOrWhiteSpace(request.EvidenceReference))
            return Reject("INCOMPLETE_PROVENANCE_OR_EVIDENCE");
        if (!StringComparer.Ordinal.Equals(request.FoundationCandidate, FoundationCandidate)) return Reject("FOUNDATION_CANDIDATE_MISMATCH");
        if (!StringComparer.Ordinal.Equals(request.DestinationFsaId, DestinationFsaId)) return Reject("WRONG_FSA_DESTINATION");
        if (request.ContainsBusinessJudgmentAsFsaDecision) return Reject("BUSINESS_JUDGMENT_REMAINS_OUTSIDE_FSA");
        if (request.AuthorityExpansionRequested) return Reject("AUTHORITY_EXPANSION_REQUEST_REQUIRES_SEPARATE_OWNER_GOVERNANCE");
        if (request.OwnerSilenceTreatedAsApproval) return Reject("OWNER_SILENCE_IS_NOT_APPROVAL");
        if (request.RuntimeActivationRequested || request.ProductionAdoptionRequested) return Reject("REVIEW_BINDING_DOES_NOT_GRANT_RUNTIME_OR_ADOPTION");
        if (!IsSha256(request.CandidateSha256) || !IsSha256(request.EvidenceSha256)) return Reject("INVALID_INTEGRITY_DIGEST");
        return new(true, "DELIVERABLE_TO_FSA_REVIEW", false, false, false, false);
    }

    private static FsaPeerBindingDecision Reject(string reason) => new(false, reason, false, false, false, false);
    private static bool IsSha256(string? value) => value is { Length: 64 } && value.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F');
}

public sealed record FsaPeerBindingRequest(string FoundationCandidate, string DestinationFsaId, string SourceScopeId, string SourceAwarenessId,
    string CandidateId, string CandidateSha256, string EvidenceReference, string EvidenceSha256, bool ContainsBusinessJudgmentAsFsaDecision,
    bool AuthorityExpansionRequested, bool OwnerSilenceTreatedAsApproval, bool RuntimeActivationRequested, bool ProductionAdoptionRequested);
public sealed record FsaPeerBindingDecision(bool Accepted, string ReasonCode, bool FsaAcceptanceGranted, bool OwnerAdoptionGranted, bool RuntimeAuthorityGranted, bool BusinessAuthorityGranted);
