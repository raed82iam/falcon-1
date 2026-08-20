using System.Runtime.CompilerServices;
using TW = Falcon.FSATS.Trading.Awareness;

internal static class AwarenessFoundationBindingAdversarialChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        MsaMayTargetLogicalFsaReviewOnlyWhileFcr0030IsPending();
    }

    private static void MsaMayTargetLogicalFsaReviewOnlyWhileFcr0030IsPending()
    {
        var unsigned = new TW.AwarenessCandidate(
            "msa-candidate-1",
            TW.ProposalOrigin.Msa,
            TW.TradingAwarenessTopology.MsaId,
            string.Empty,
            TW.TradingAwarenessTopology.MsaId,
            "msa-evidence-1",
            true)
        {
            CandidateSha256 = new string('A', 64),
            EvidenceSha256 = new string('B', 64),
            LineageId = "msa-lineage-1",
            ParentIdentity = TW.TradingAwarenessTopology.FsaLogicalReviewTier,
            FoundationDestinationBindingState = TW.TradingAwarenessTopology.FsaExactDestinationBindingState
        };
        var valid = unsigned with { BindingSha256 = TW.AwarenessGovernance.ComputeBindingSha256(unsigned) };
        if (!TW.AwarenessGovernance.IsIdentityAndEvidenceBound(valid))
            throw new InvalidOperationException("LOGICAL_FSA_REVIEW_TIER_REJECTED");

        var fabricatedBinding = valid with { FoundationDestinationBindingState = "EXACT_FSA_RUNTIME_DESTINATION_BOUND" };
        fabricatedBinding = fabricatedBinding with { BindingSha256 = TW.AwarenessGovernance.ComputeBindingSha256(fabricatedBinding) };
        if (TW.AwarenessGovernance.IsIdentityAndEvidenceBound(fabricatedBinding))
            throw new InvalidOperationException("FCR0030_EXACT_FOUNDATION_BINDING_FABRICATED_LOCALLY");

        var fabricatedIdentity = valid with { ParentIdentity = "FSA" };
        fabricatedIdentity = fabricatedIdentity with { BindingSha256 = TW.AwarenessGovernance.ComputeBindingSha256(fabricatedIdentity) };
        if (TW.AwarenessGovernance.IsIdentityAndEvidenceBound(fabricatedIdentity))
            throw new InvalidOperationException("EXACT_FSA_IDENTITY_FABRICATED_LOCALLY");
    }
}
