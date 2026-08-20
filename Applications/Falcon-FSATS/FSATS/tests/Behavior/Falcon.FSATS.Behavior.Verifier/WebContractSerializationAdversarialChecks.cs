using System.Text.Json;
using TC = Falcon.FSATS.Trading.Contracts;

internal static class WebContractSerializationAdversarialChecks
{
    internal static void Run()
    {
        var account = new TC.BrokerAccountScope(new TC.BrokerId("alpaca"), new TC.BrokerAccountId("A"), "paper");
        var update = new TC.WebPortfolioProjectionUpdate(
            "UP-1",
            1,
            TC.WebProjectionUpdateKind.Correction,
            new TC.CorrelationId("CORR-1"),
            account,
            new[] { TC.WebPortfolioContractIds.PortfolioSummaryProjection },
            "1",
            DateTimeOffset.Parse("2026-08-15T17:00:00Z"),
            TC.TruthClassification.LastKnown,
            TC.WebFreshnessState.Stale,
            "EV-1",
            "TEST",
            correctsUpdateId: "UP-0");

        var json = JsonSerializer.Serialize(update, TC.WebContractSerialization.CreateV1Options());

        if (!json.Contains("\"updateId\":\"UP-1\"", StringComparison.Ordinal)
            || !json.Contains("\"updateKind\":\"CORRECTION\"", StringComparison.Ordinal)
            || !json.Contains("\"truthState\":\"LAST_KNOWN\"", StringComparison.Ordinal)
            || !json.Contains("\"freshnessState\":\"STALE\"", StringComparison.Ordinal)
            || !json.Contains("\"correctsUpdateId\":\"UP-0\"", StringComparison.Ordinal))
            throw new InvalidOperationException("WEB_V1_SERIALIZATION_POLICY_MISMATCH");

        if (json.Contains("\"UpdateId\"", StringComparison.Ordinal)
            || json.Contains("\"updateKind\":1", StringComparison.Ordinal)
            || json.Contains("\"truthState\":1", StringComparison.Ordinal))
            throw new InvalidOperationException("WEB_V1_SERIALIZATION_FELL_BACK_TO_NONCANONICAL_WIRE_SHAPE");
    }
}
