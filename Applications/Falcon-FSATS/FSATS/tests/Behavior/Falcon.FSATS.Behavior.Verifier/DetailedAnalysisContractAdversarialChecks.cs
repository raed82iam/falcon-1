using TC = Falcon.FSATS.Trading.Contracts;

internal static class DetailedAnalysisContractAdversarialChecks
{
    internal static void Run()
    {
        CanonicalRequestRequiresSharedWebAndAnalysisIntent();
        NeedsClarificationCannotPretendResolution();
        DetailedProjectionPreservesTruthAndDisagreement();
        MissingOptionalAnalysisValuesRemainMissing();
    }

    private static void CanonicalRequestRequiresSharedWebAndAnalysisIntent()
    {
        var request = new TC.WebOnDemandAnalysisRequest(
            "REQ-1",
            new TC.CorrelationId("CORR-1"),
            TC.WebOnDemandAnalysisRequest.SharedWebApplicationId,
            new TC.TradingInstrumentRef("AAPL"),
            "NASDAQ",
            "US_EQUITY",
            "DETAILED_ASSET_ANALYSIS",
            DateTimeOffset.UtcNow,
            "ENTITLEMENT-REF-1");

        if (request.RequestingApplicationId != TC.WebOnDemandAnalysisRequest.SharedWebApplicationId
            || request.AnalysisIntent != "DETAILED_ASSET_ANALYSIS"
            || request.RequestedInstrumentReference.Value != "AAPL")
            throw new InvalidOperationException("CANONICAL_ON_DEMAND_REQUEST_BINDING_DRIFTED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebOnDemandAnalysisRequest(
                "REQ-X", new TC.CorrelationId("CORR-X"), "OTHER_APP", new TC.TradingInstrumentRef("AAPL"),
                null, null, "DETAILED_ASSET_ANALYSIS", DateTimeOffset.UtcNow),
            "NON_WEB_REQUESTING_APPLICATION_ACCEPTED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebOnDemandAnalysisRequest(
                "REQ-X", new TC.CorrelationId("CORR-X"), TC.WebOnDemandAnalysisRequest.SharedWebApplicationId,
                new TC.TradingInstrumentRef("AAPL"), null, null, " ", DateTimeOffset.UtcNow),
            "MISSING_ANALYSIS_INTENT_ACCEPTED");
    }

    private static void NeedsClarificationCannotPretendResolution()
    {
        var summary = new TC.WebAnalysisInputTruthFreshnessSummary(
            TC.TruthClassification.Unknown,
            TC.WebFreshnessState.Unknown,
            TC.WebProjectionCompleteness.Unknown,
            new[] { "AMBIGUOUS_INSTRUMENT" });

        var result = new TC.WebOnDemandAnalysisResult(
            "REQ-AMB",
            new TC.CorrelationId("CORR-AMB"),
            "AR-AMB",
            null,
            "DETAILED_ASSET_ANALYSIS",
            TC.WebOnDemandAnalysisResultState.NeedsClarification,
            null,
            DateTimeOffset.UtcNow,
            summary,
            null,
            new[] { "CUSTOMER_CLARIFICATION_REQUIRED" },
            "NEEDS_CLARIFICATION",
            new[] { new TC.TradingInstrumentRef("NASDAQ:AAPL"), new TC.TradingInstrumentRef("OTHER:AAPL") });

        if (result.ResolvedInstrumentIdentity is not null || result.ClarificationCandidates.Count != 2)
            throw new InvalidOperationException("NEEDS_CLARIFICATION_SEMANTICS_DRIFTED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebOnDemandAnalysisResult(
                "REQ-BAD", new TC.CorrelationId("CORR-BAD"), "AR-BAD", new TC.TradingInstrumentRef("AAPL"),
                "DETAILED_ASSET_ANALYSIS", TC.WebOnDemandAnalysisResultState.NeedsClarification, null,
                DateTimeOffset.UtcNow, summary, null, Array.Empty<string>(), "BAD",
                new[] { new TC.TradingInstrumentRef("NASDAQ:AAPL") }),
            "NEEDS_CLARIFICATION_WITH_RESOLVED_INSTRUMENT_ACCEPTED");
    }

    private static void DetailedProjectionPreservesTruthAndDisagreement()
    {
        var stalePartial = new TC.WebAnalysisInputTruthFreshnessSummary(
            TC.TruthClassification.Stale,
            TC.WebFreshnessState.Stale,
            TC.WebProjectionCompleteness.Partial,
            new[] { "ONE_SOURCE_STALE" });

        var synthesis = new TC.WebDetailedAnalysisSynthesis(
            TC.WebDetailedSynthesisState.Conflicted,
            new[] { "TREND_UP" },
            new[] { "TARGET_DISAGREEMENT" },
            new[] { "SHORT_VS_LONG_HORIZON" },
            "Sources disagree materially; no false consensus.",
            new[] { "STRATEGY:S1", "SCHOOL:SC1" },
            new[] { "PARTIAL_INPUTS" });

        var projection = new TC.WebDetailedAssetAnalysisProjection(
            new TC.TradingInstrumentRef("AAPL"),
            "AR-1",
            DateTimeOffset.UtcNow,
            TC.TruthClassification.Stale,
            stalePartial,
            Array.Empty<TC.WebDetailedHorizonView>(),
            Array.Empty<TC.WebDetailedStrategyView>(),
            Array.Empty<TC.WebDetailedSchoolView>(),
            synthesis);

        if (projection.Synthesis.Disagreements.Count == 0 || projection.Synthesis.UnresolvedConflicts.Count == 0)
            throw new InvalidOperationException("DETAILED_ANALYSIS_DISAGREEMENT_WAS_SUPPRESSED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebDetailedAssetAnalysisProjection(
                new TC.TradingInstrumentRef("AAPL"), "AR-2", DateTimeOffset.UtcNow,
                TC.TruthClassification.Current, stalePartial,
                Array.Empty<TC.WebDetailedHorizonView>(), Array.Empty<TC.WebDetailedStrategyView>(), Array.Empty<TC.WebDetailedSchoolView>(), synthesis),
            "STALE_INPUT_UPGRADED_TO_CURRENT_SYNTHESIS");

        var contradictory = new TC.WebAnalysisInputTruthFreshnessSummary(
            TC.TruthClassification.Stale,
            TC.WebFreshnessState.Current,
            TC.WebProjectionCompleteness.Complete,
            new[] { "TRUTH_STALE_DESPITE_FRESH_TRANSPORT" });
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebDetailedAssetAnalysisProjection(
                new TC.TradingInstrumentRef("AAPL"), "AR-CONTRADICT", DateTimeOffset.UtcNow,
                TC.TruthClassification.Current, contradictory,
                Array.Empty<TC.WebDetailedHorizonView>(), Array.Empty<TC.WebDetailedStrategyView>(), Array.Empty<TC.WebDetailedSchoolView>(), synthesis),
            "STALE_TRUTH_WITH_CURRENT_FRESHNESS_UPGRADED_TO_CURRENT_SYNTHESIS");

        var falseComplete = synthesis with { SynthesisState = TC.WebDetailedSynthesisState.Complete };
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebDetailedAssetAnalysisProjection(
                new TC.TradingInstrumentRef("AAPL"), "AR-3", DateTimeOffset.UtcNow,
                TC.TruthClassification.Stale, stalePartial,
                Array.Empty<TC.WebDetailedHorizonView>(), Array.Empty<TC.WebDetailedStrategyView>(), Array.Empty<TC.WebDetailedSchoolView>(), falseComplete),
            "PARTIAL_OR_CONFLICTED_INPUTS_UPGRADED_TO_COMPLETE_SYNTHESIS");
    }

    private static void MissingOptionalAnalysisValuesRemainMissing()
    {
        var horizon = new TC.WebDetailedHorizonView(
            "SHORT",
            TC.WebOnDemandAnalysisResultState.Partial,
            "No authoritative target or confidence is available.",
            Array.Empty<TC.WebMaterialLevelOrTarget>(),
            null,
            new[] { "TARGET_UNAVAILABLE", "CONFIDENCE_UNAVAILABLE" },
            new[] { "ANALYSIS:SHORT" });

        if (horizon.ConfidenceOrStrength is not null || horizon.MaterialLevelsOrTargets.Count != 0)
            throw new InvalidOperationException("MISSING_TARGET_OR_CONFIDENCE_WAS_INVENTED");
    }

    private static void ExpectThrows<TException>(Action action, string failureCode) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException(failureCode);
    }
}
