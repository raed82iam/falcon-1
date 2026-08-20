using TC = Falcon.FSATS.Trading.Contracts;

internal static class IncidentProtectionTruthAdversarialChecks
{
    internal static void Run()
    {
        var account = new TC.BrokerAccountScope(new TC.BrokerId("ALPACA"), new TC.BrokerAccountId("PA-1"), "PAPER");
        var required = new[]
        {
            new TC.WebIncidentActionInstruction(1, TC.WebIncidentNextAction.VerifyProtectionOrders, true, "VERIFY_PROTECTION")
        };

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebAffectedPositionFollowupProjection(
                "P-STALE", "INC-STALE", account, new TC.TradingPositionRef("POS-1"), new TC.TradingInstrumentRef("AAPL"),
                DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
                TC.WebIncidentProtectionState.BrokerConfirmedProtected,
                TC.WebCustomerFollowupRequirement.None,
                "LAST_CONFIRMED_ONLY",
                Array.Empty<TC.WebIncidentActionInstruction>(),
                false,
                null,
                TC.WebIncidentFollowupLifecycleState.Active,
                TC.TruthClassification.LastKnown,
                TC.WebFreshnessState.Stale,
                "EV-STALE",
                DateTimeOffset.Parse("2026-08-15T18:05:00Z")),
            "STALE_PROTECTION_FALSELY_CLASSIFIED_BROKER_CONFIRMED_PROTECTED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebAffectedPositionFollowupProjection(
                "P-GAP", "INC-GAP", account, new TC.TradingPositionRef("POS-2"), new TC.TradingInstrumentRef("MSFT"),
                DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
                TC.WebIncidentProtectionState.UnexpectedlyMissingOrIncompleteProtection,
                TC.WebCustomerFollowupRequirement.Recommended,
                "UNEXPECTED_PROTECTION_GAP",
                required,
                false,
                null,
                TC.WebIncidentFollowupLifecycleState.AwaitingCustomerFact,
                TC.TruthClassification.Current,
                TC.WebFreshnessState.Current,
                "EV-GAP",
                DateTimeOffset.Parse("2026-08-15T18:05:00Z")),
            "UNEXPECTED_PROTECTION_GAP_WITHOUT_REQUIRED_FOLLOWUP_ACCEPTED");
    }

    private static void ExpectThrows<TException>(Action action, string failureCode) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException(failureCode);
    }
}
