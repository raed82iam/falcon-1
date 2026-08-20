using System.Security.Cryptography;
using System.Text;

namespace Falcon.FSATS.Trading.Awareness;

public enum AwarenessTier { Msa, Lsa, Csa }
public sealed record AwarenessIdentity(string Id, AwarenessTier Tier, string Responsibility, string? ParentId, string? TargetComponent = null);

public static class TradingAwarenessTopology
{
    public const string MsaId = "MSA-TRADING-01";
    public const string FsaLogicalReviewTier = "FSA_LOGICAL_REVIEW_TIER";
    public const string FsaExactDestinationBindingState = "BOUND_TO_FOUNDATION_FSA_PEER_INTERFACE_91da7869e7e16e943c92620ed0e8bb0fe7409459";

    public static IReadOnlyList<AwarenessIdentity> All { get; } = Build();

    private static IReadOnlyList<AwarenessIdentity> Build()
    {
        var list = new List<AwarenessIdentity>
        {
            new(MsaId, AwarenessTier.Msa, "Complete Falcon Self-Aware Trading Application", null)
        };
        var names = new[]
        {
            "Operations Account Environment", "Market Instrument Universe", "Analysis Frameworks", "Classical Trading School",
            "Opportunity Hunting School", "Strategy Orchestration Decision", "Unified Risk Management", "Portfolio Capital Management",
            "Execution Position Lifecycle", "Trading Learning Knowledge", "Trading Analytics Attribution", "Strategy Evolution Experimentation",
            "Trading Resource Management"
        };
        for (var i = 0; i < names.Length; i++) list.Add(new($"T-LSA-{i + 1:00}", AwarenessTier.Lsa, names[i], MsaId));
        list.Add(new("CSA-T05-01", AwarenessTier.Csa, "Opportunity discovery specialized self-awareness", "T-LSA-05", "OpportunityDiscoveryEngine"));
        list.Add(new("CSA-T06-01", AwarenessTier.Csa, "Strategy controller specialized self-awareness", "T-LSA-06", "StrategyController"));
        list.Add(new("CSA-T12-01", AwarenessTier.Csa, "Strategy evolution specialized self-awareness", "T-LSA-12", "StrategyEvolutionEngine"));
        return list.AsReadOnly();
    }
}

public enum ProposalOrigin { Csa, Lsa, Msa }
public sealed record AwarenessCandidate(
    string CandidateId,
    ProposalOrigin Origin,
    string OriginIdentity,
    string ParentLsaId,
    string MsaId,
    string EvidenceId,
    bool RuntimeMutationRequested)
{
    public string CandidateSha256 { get; init; } = string.Empty;
    public string EvidenceSha256 { get; init; } = string.Empty;
    public string LineageId { get; init; } = string.Empty;
    public string ParentIdentity { get; init; } = string.Empty;
    public string? ParentCandidateId { get; init; }
    public string BindingSha256 { get; init; } = string.Empty;
    public string FoundationDestinationBindingState { get; init; } = TradingAwarenessTopology.FsaExactDestinationBindingState;
}

public static class AwarenessGovernance
{
    public static bool RequiresIsolation(AwarenessCandidate candidate) => candidate.RuntimeMutationRequested;

    public static string ComputeBindingSha256(AwarenessCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        var fields = new[]
        {
            candidate.CandidateId,
            candidate.Origin.ToString(),
            candidate.OriginIdentity,
            candidate.ParentLsaId,
            candidate.MsaId,
            candidate.EvidenceId,
            candidate.RuntimeMutationRequested ? "1" : "0",
            candidate.CandidateSha256,
            candidate.EvidenceSha256,
            candidate.LineageId,
            candidate.ParentIdentity,
            candidate.ParentCandidateId ?? string.Empty,
            candidate.FoundationDestinationBindingState
        };
        var value = string.Concat(fields.Select(Encode));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    public static bool IsIdentityAndEvidenceBound(AwarenessCandidate? candidate)
    {
        if (candidate is null || string.IsNullOrWhiteSpace(candidate.CandidateId) || string.IsNullOrWhiteSpace(candidate.EvidenceId) ||
            string.IsNullOrWhiteSpace(candidate.OriginIdentity) || string.IsNullOrWhiteSpace(candidate.MsaId) ||
            string.IsNullOrWhiteSpace(candidate.LineageId) || string.IsNullOrWhiteSpace(candidate.ParentIdentity) ||
            !StringComparer.Ordinal.Equals(candidate.FoundationDestinationBindingState, TradingAwarenessTopology.FsaExactDestinationBindingState) ||
            !IsSha256(candidate.CandidateSha256) || !IsSha256(candidate.EvidenceSha256) || !IsSha256(candidate.BindingSha256) ||
            !StringComparer.Ordinal.Equals(candidate.BindingSha256, ComputeBindingSha256(candidate)) ||
            !StringComparer.Ordinal.Equals(candidate.MsaId, TradingAwarenessTopology.MsaId))
            return false;

        var origin = TradingAwarenessTopology.All.SingleOrDefault(x => StringComparer.Ordinal.Equals(x.Id, candidate.OriginIdentity));
        if (origin is null) return false;

        return candidate.Origin switch
        {
            ProposalOrigin.Csa => origin.Tier == AwarenessTier.Csa &&
                                  !string.IsNullOrWhiteSpace(origin.ParentId) &&
                                  StringComparer.Ordinal.Equals(origin.ParentId, candidate.ParentLsaId) &&
                                  StringComparer.Ordinal.Equals(candidate.ParentIdentity, candidate.ParentLsaId) &&
                                  TradingAwarenessTopology.All.Any(x => x.Tier == AwarenessTier.Lsa &&
                                      StringComparer.Ordinal.Equals(x.Id, candidate.ParentLsaId) &&
                                      StringComparer.Ordinal.Equals(x.ParentId, candidate.MsaId)),
            ProposalOrigin.Lsa => origin.Tier == AwarenessTier.Lsa &&
                                  StringComparer.Ordinal.Equals(origin.Id, candidate.ParentLsaId) &&
                                  StringComparer.Ordinal.Equals(origin.ParentId, candidate.MsaId) &&
                                  StringComparer.Ordinal.Equals(candidate.ParentIdentity, candidate.MsaId),
            ProposalOrigin.Msa => origin.Tier == AwarenessTier.Msa &&
                                  StringComparer.Ordinal.Equals(origin.Id, candidate.MsaId) &&
                                  string.IsNullOrEmpty(candidate.ParentLsaId) &&
                                  StringComparer.Ordinal.Equals(candidate.ParentIdentity, TradingAwarenessTopology.FsaLogicalReviewTier),
            _ => false
        };
    }

    public static string EscalationPath(AwarenessCandidate candidate)
    {
        if (!IsIdentityAndEvidenceBound(candidate)) throw new InvalidOperationException("INVALID_AWARENESS_CANDIDATE_IDENTITY_OR_EVIDENCE_BINDING");

        return candidate.Origin switch
        {
            ProposalOrigin.Csa => "CSA->PARENT_LSA->APPLICATION_MSA->FOUNDATION_FSA_PEER_INTERFACE->OWNER_GOVERNANCE",
            ProposalOrigin.Lsa => "LSA->APPLICATION_MSA->FOUNDATION_FSA_PEER_INTERFACE->OWNER_GOVERNANCE",
            ProposalOrigin.Msa => "APPLICATION_MSA->FOUNDATION_FSA_PEER_INTERFACE->OWNER_GOVERNANCE",
            _ => throw new ArgumentOutOfRangeException(nameof(candidate))
        };
    }

    private static string Encode(string? value)
    {
        var normalized = value ?? string.Empty;
        return $"{Encoding.UTF8.GetByteCount(normalized)}:{normalized}";
    }

    private static bool IsSha256(string? value)
        => value is { Length: 64 } && value.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F');
}
