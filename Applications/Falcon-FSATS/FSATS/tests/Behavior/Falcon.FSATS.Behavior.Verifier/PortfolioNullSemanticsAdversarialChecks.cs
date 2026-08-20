using TC = Falcon.FSATS.Trading.Contracts;

internal static class PortfolioNullSemanticsAdversarialChecks
{
    internal static void Run()
    {
        UnsupportedSummaryCannotCarryNumericTruth();
        UnsupportedCollectionsCannotCarryDerivedTruth();
        NotApplicablePerformanceCannotCarryNumericOrHistoryTruth();
    }

    private static TC.WebProjectionEnvelope Envelope(TC.WebAvailabilityState availability)
        => new(
            "P-1",
            TC.WebPortfolioContractIds.PortfolioSummaryProjection,
            "1",
            new TC.BrokerAccountScope(new TC.BrokerId("ALPACA"), new TC.BrokerAccountId("A"), "PAPER"),
            DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
            TC.TruthClassification.Unknown,
            TC.WebFreshnessState.Unknown,
            TC.WebProjectionCompleteness.Unknown,
            availability,
            "EV-1",
            availability.ToString().ToUpperInvariant());

    private static void UnsupportedSummaryCannotCarryNumericTruth()
    {
        var unsupported = Envelope(TC.WebAvailabilityState.Unsupported);

        _ = new TC.WebPortfolioSummaryProjection(unsupported, "USD", null, null, null, null, null, null);

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebPortfolioSummaryProjection(unsupported, "USD", 0m, null, null, null, null, null),
            "UNSUPPORTED_SUMMARY_ACCEPTED_ZERO_AS_SOURCE_VALUE");
    }

    private static void UnsupportedCollectionsCannotCarryDerivedTruth()
    {
        var unsupported = Envelope(TC.WebAvailabilityState.Unsupported);
        var page = new TC.WebPageInfo(null, false, 100);

        _ = new TC.WebPositionCollectionProjection(unsupported, Array.Empty<TC.WebPositionItem>(), page);
        _ = new TC.WebOrderTradeActivityProjection(unsupported, Array.Empty<TC.WebOrderTradeActivityItem>(), page);

        var position = new TC.WebPositionItem(
            new TC.TradingPositionRef("POS-1"), new TC.TradingInstrumentRef("AAPL"), 1m, 100m, 101m, 101m, 1m,
            "USD", TC.TruthClassification.LastKnown, TC.WebFreshnessState.Stale, "LAST_KNOWN");
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebPositionCollectionProjection(unsupported, new[] { position }, page),
            "UNSUPPORTED_POSITION_COLLECTION_ACCEPTED_DERIVED_POSITION");

        var activity = new TC.WebOrderTradeActivityItem(
            new TC.TradingOrderRef("ORD-1"), new TC.TradingInstrumentRef("AAPL"), TC.WebOrderTradeState.Accepted,
            1m, null, null, "USD", DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
            TC.TruthClassification.LastKnown, TC.WebFreshnessState.Stale, "LAST_KNOWN");
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebOrderTradeActivityProjection(unsupported, new[] { activity }, page),
            "UNSUPPORTED_ORDER_ACTIVITY_ACCEPTED_DERIVED_ACTIVITY");
    }

    private static void NotApplicablePerformanceCannotCarryNumericOrHistoryTruth()
    {
        var notApplicable = Envelope(TC.WebAvailabilityState.NotApplicable);
        var page = new TC.WebPageInfo(null, false, 100);
        var start = DateTimeOffset.Parse("2026-08-15T17:00:00Z");
        var end = DateTimeOffset.Parse("2026-08-15T18:00:00Z");

        _ = new TC.WebPortfolioPerformanceProjection(
            notApplicable, start, end, "USD", null, null, null, null, null, null,
            Array.Empty<TC.WebPerformancePoint>(), page);

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebPortfolioPerformanceProjection(
                notApplicable, start, end, "USD", null, null, null, null, 0m, null,
                Array.Empty<TC.WebPerformancePoint>(), page),
            "NOT_APPLICABLE_PERFORMANCE_ACCEPTED_ZERO_AS_SOURCE_VALUE");

        var history = new[]
        {
            new TC.WebPerformancePoint(end, 100m, 0m, 0m, TC.TruthClassification.LastKnown, TC.WebFreshnessState.Stale, "LAST_KNOWN")
        };
        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebPortfolioPerformanceProjection(
                notApplicable, start, end, "USD", null, null, null, null, null, null, history, page),
            "NOT_APPLICABLE_PERFORMANCE_ACCEPTED_DERIVED_HISTORY");
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
