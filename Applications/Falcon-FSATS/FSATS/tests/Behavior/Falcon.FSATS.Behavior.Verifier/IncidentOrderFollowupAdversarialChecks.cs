using TC = Falcon.FSATS.Trading.Contracts;

internal static class IncidentOrderFollowupAdversarialChecks
{
    internal static void Run()
    {
        var account = new TC.BrokerAccountScope(new TC.BrokerId("ALPACA"), new TC.BrokerAccountId("PA-1"), "PAPER");
        var required = new[]
        {
            new TC.WebIncidentActionInstruction(1, TC.WebIncidentNextAction.ResolveAmbiguousSubmission, true, "VERIFY_AMBIGUOUS_ORDER")
        };

        var unknown = new TC.WebAffectedOrderFollowupProjection(
            "OP-1", "INC-1", account, new TC.TradingOrderRef("ORD-1"), new TC.TradingInstrumentRef("AAPL"),
            DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
            TC.WebIncidentOrderTruthState.OutcomeUnknownOrAmbiguous,
            TC.WebCustomerFollowupRequirement.Required,
            "NOT_PROVEN_EXECUTED_OR_NOT_EXECUTED",
            required,
            TC.TruthClassification.Unknown,
            TC.WebFreshnessState.Unknown,
            "EV-1",
            DateTimeOffset.Parse("2026-08-15T18:05:00Z"));

        if (unknown.OrderTruthState != TC.WebIncidentOrderTruthState.OutcomeUnknownOrAmbiguous)
            throw new InvalidOperationException("AMBIGUOUS_ORDER_TRUTH_COLLAPSED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebAffectedOrderFollowupProjection(
                "OP-X", "INC-X", account, new TC.TradingOrderRef("ORD-X"), new TC.TradingInstrumentRef("AAPL"),
                null,
                TC.WebIncidentOrderTruthState.OutcomeUnknownOrAmbiguous,
                TC.WebCustomerFollowupRequirement.None,
                "UNKNOWN",
                Array.Empty<TC.WebIncidentActionInstruction>(),
                TC.TruthClassification.Unknown,
                TC.WebFreshnessState.Unknown,
                "EV-X",
                DateTimeOffset.Parse("2026-08-15T18:05:00Z")),
            "UNRESOLVED_ORDER_WITH_NO_FOLLOWUP_ACCEPTED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebAffectedOrderFollowupProjection(
                "OP-STALE", "INC-STALE", account, new TC.TradingOrderRef("ORD-S"), new TC.TradingInstrumentRef("AAPL"),
                DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
                TC.WebIncidentOrderTruthState.BrokerConfirmedWorking,
                TC.WebCustomerFollowupRequirement.Recommended,
                "STALE_LAST_KNOWN",
                Array.Empty<TC.WebIncidentActionInstruction>(),
                TC.TruthClassification.LastKnown,
                TC.WebFreshnessState.Stale,
                "EV-S",
                DateTimeOffset.Parse("2026-08-15T18:05:00Z")),
            "STALE_ORDER_FALSELY_CLASSIFIED_BROKER_CONFIRMED_CURRENT");
    }

    private static void ExpectThrows<TException>(Action action, string failureCode) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException(failureCode);
    }
}
