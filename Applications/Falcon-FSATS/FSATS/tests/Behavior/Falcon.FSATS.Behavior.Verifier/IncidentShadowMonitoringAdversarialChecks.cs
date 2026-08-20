using TC = Falcon.FSATS.Trading.Contracts;
using SC = Falcon.FSATS.FSTSimA.Contracts;

internal static class IncidentShadowMonitoringAdversarialChecks
{
    internal static void Run()
    {
        AffectedPositionFollowupIsApplicationOwnedAndAccountScoped();
        IntentionalAndUnexpectedUnprotectedStatesRemainDistinct();
        ShadowEvidenceCannotMasqueradeAsBrokerTruth();
        AmbiguousShadowRequiresExplicitOutcomeScenarios();
        ShadowTimingAndEndStateRemainExplicit();
    }

    private static void AffectedPositionFollowupIsApplicationOwnedAndAccountScoped()
    {
        var account = new TC.BrokerAccountScope(new TC.BrokerId("ALPACA"), new TC.BrokerAccountId("PA-1"), "PAPER");
        var actions = new[]
        {
            new TC.WebIncidentActionInstruction(1, TC.WebIncidentNextAction.VerifyOpenPosition, true, "VERIFY_POSITION"),
            new TC.WebIncidentActionInstruction(2, TC.WebIncidentNextAction.VerifyProtectionOrders, true, "VERIFY_PROTECTION")
        };

        var projection = new TC.WebAffectedPositionFollowupProjection(
            "P-1", "INC-1", account, new TC.TradingPositionRef("POS-1"), new TC.TradingInstrumentRef("AAPL"),
            DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
            TC.WebIncidentProtectionState.ProtectionUnknownOrAmbiguous,
            TC.WebCustomerFollowupRequirement.Required,
            "BROKER_TRUTH_UNAVAILABLE",
            actions,
            true,
            "SHADOW-1",
            TC.WebIncidentFollowupLifecycleState.AwaitingCustomerFact,
            TC.TruthClassification.LastKnown,
            TC.WebFreshnessState.Stale,
            "EV-1",
            DateTimeOffset.Parse("2026-08-15T18:05:00Z"));

        if (projection.Account != account || projection.FollowupRequirement != TC.WebCustomerFollowupRequirement.Required)
            throw new InvalidOperationException("INCIDENT_FOLLOWUP_SCOPE_OR_REQUIREMENT_DRIFTED");

        ExpectThrows<ArgumentException>(() =>
            _ = new TC.WebAffectedPositionFollowupProjection(
                "P-X", "INC-X", account, new TC.TradingPositionRef("POS-X"), new TC.TradingInstrumentRef("AAPL"),
                DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
                TC.WebIncidentProtectionState.ProtectionUnknownOrAmbiguous,
                TC.WebCustomerFollowupRequirement.Required,
                "BAD",
                Array.Empty<TC.WebIncidentActionInstruction>(),
                false,
                null,
                TC.WebIncidentFollowupLifecycleState.Active,
                TC.TruthClassification.Unknown,
                TC.WebFreshnessState.Unknown,
                "EV-X",
                DateTimeOffset.Parse("2026-08-15T18:01:00Z")),
            "REQUIRED_FOLLOWUP_WITHOUT_REQUIRED_ACTION_ACCEPTED");
    }

    private static void IntentionalAndUnexpectedUnprotectedStatesRemainDistinct()
    {
        if (TC.WebIncidentProtectionState.IntentionallyRetainedWithoutCurrentBrokerProtection
            == TC.WebIncidentProtectionState.UnexpectedlyMissingOrIncompleteProtection)
            throw new InvalidOperationException("INTENTIONAL_AND_ACCIDENTAL_UNPROTECTED_STATES_COLLAPSED");
    }

    private static void ShadowEvidenceCannotMasqueradeAsBrokerTruth()
    {
        var scenario = new SC.WebEmergencyShadowScenarioProjection(
            "SCN-1",
            SC.EmergencyShadowScenarioKind.NotExecuted,
            SC.EmergencyShadowEvidenceTruth.SimulatorEstimate,
            10m,
            1000m,
            50m,
            SC.EmergencyShadowThresholdState.Warning,
            "USD",
            "SIMULATOR_SCENARIO_ONLY",
            new[] { "SIM-EV-1" });

        if (scenario.EvidenceTruth != SC.EmergencyShadowEvidenceTruth.SimulatorEstimate)
            throw new InvalidOperationException("SIMULATOR_ESTIMATE_TRUTH_CLASS_DRIFTED");

        var topLevelTruthNames = Enum.GetNames<SC.EmergencyShadowProjectionTruth>();
        if (topLevelTruthNames.Any(x => x.Contains("Broker", StringComparison.OrdinalIgnoreCase)
                                        || x.Contains("Live", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("SHADOW_TOP_LEVEL_TRUTH_EXPOSES_BROKER_OR_LIVE_TRUTH_CLASS");
    }

    private static void AmbiguousShadowRequiresExplicitOutcomeScenarios()
    {
        var account = new SC.EmergencyShadowBrokerAccountScope("ALPACA", "PA-1", "PAPER");
        var incomplete = new[]
        {
            Scenario("N", SC.EmergencyShadowScenarioKind.NotExecuted),
            Scenario("F", SC.EmergencyShadowScenarioKind.FullyExecuted)
        };

        ExpectThrows<ArgumentException>(() =>
            _ = new SC.WebEmergencyShadowMonitoringProjection(
                "SP-AMB", "INC-AMB", "SHADOW-AMB", account, null, "ORD-AMB", "AAPL",
                DateTimeOffset.Parse("2026-08-15T18:00:00Z"), DateTimeOffset.Parse("2026-08-15T18:01:00Z"), null,
                SC.EmergencyShadowState.Active, true, incomplete,
                null, DateTimeOffset.Parse("2026-08-15T18:05:00Z"),
                SC.EmergencyShadowProjectionTruth.Simulator, SC.EmergencyShadowFreshnessState.Current,
                "FSTSIMA:SHADOW-AMB", "EV-SHADOW-AMB"),
            "AMBIGUOUS_SHADOW_WITH_INCOMPLETE_OUTCOME_SCENARIOS_ACCEPTED");

        var complete = new[]
        {
            Scenario("N", SC.EmergencyShadowScenarioKind.NotExecuted),
            Scenario("P", SC.EmergencyShadowScenarioKind.PartiallyExecuted),
            Scenario("F", SC.EmergencyShadowScenarioKind.FullyExecuted)
        };
        var projection = new SC.WebEmergencyShadowMonitoringProjection(
            "SP-AMB2", "INC-AMB2", "SHADOW-AMB2", account, null, "ORD-AMB2", "AAPL",
            DateTimeOffset.Parse("2026-08-15T18:00:00Z"), DateTimeOffset.Parse("2026-08-15T18:01:00Z"), null,
            SC.EmergencyShadowState.Active, true, complete,
            null, DateTimeOffset.Parse("2026-08-15T18:05:00Z"),
            SC.EmergencyShadowProjectionTruth.Simulator, SC.EmergencyShadowFreshnessState.Current,
            "FSTSIMA:SHADOW-AMB2", "EV-SHADOW-AMB2");

        if (projection.ProjectionTruth != SC.EmergencyShadowProjectionTruth.Simulator
            || projection.FreshnessState != SC.EmergencyShadowFreshnessState.Current)
            throw new InvalidOperationException("SHADOW_TOP_LEVEL_TRUTH_OR_FRESHNESS_DRIFTED");
    }

    private static void ShadowTimingAndEndStateRemainExplicit()
    {
        var account = new SC.EmergencyShadowBrokerAccountScope("ALPACA", "PA-1", "PAPER");
        var scenarios = new[]
        {
            new SC.WebEmergencyShadowScenarioProjection(
                "SCN-1", SC.EmergencyShadowScenarioKind.LastBrokerConfirmedPosition,
                SC.EmergencyShadowEvidenceTruth.LastBrokerConfirmedSeed, 10m, 1000m, 50m,
                SC.EmergencyShadowThresholdState.WithinObservedThreshold, "USD", "LAST_CONFIRMED_SEED", new[] { "EV-1" })
        };

        _ = new SC.WebEmergencyShadowMonitoringProjection(
            "SP-1", "INC-1", "SHADOW-1", account, "POS-1", null, "AAPL",
            DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
            DateTimeOffset.Parse("2026-08-15T18:01:00Z"),
            null,
            SC.EmergencyShadowState.Active,
            false,
            scenarios,
            "TRADING-PROTECTION-PROJECTION:P-1",
            DateTimeOffset.Parse("2026-08-15T18:05:00Z"),
            SC.EmergencyShadowProjectionTruth.Simulator,
            SC.EmergencyShadowFreshnessState.Current,
            "FSTSIMA:SHADOW-1",
            "EV-SHADOW-1");

        ExpectThrows<ArgumentException>(() =>
            _ = new SC.WebEmergencyShadowMonitoringProjection(
                "SP-X", "INC-X", "SHADOW-X", account, "POS-X", null, "AAPL",
                DateTimeOffset.Parse("2026-08-15T18:00:00Z"),
                DateTimeOffset.Parse("2026-08-15T18:01:00Z"),
                null,
                SC.EmergencyShadowState.EndedReconciled,
                false,
                scenarios,
                "TRADING-PROTECTION-PROJECTION:P-X",
                DateTimeOffset.Parse("2026-08-15T18:05:00Z"),
                SC.EmergencyShadowProjectionTruth.Simulator,
                SC.EmergencyShadowFreshnessState.Current,
                "FSTSIMA:SHADOW-X",
                "EV-SHADOW-X"),
            "ENDED_SHADOW_WITHOUT_END_TIME_ACCEPTED");
    }

    private static SC.WebEmergencyShadowScenarioProjection Scenario(string id, SC.EmergencyShadowScenarioKind kind)
        => new(id, kind, SC.EmergencyShadowEvidenceTruth.SimulatorEstimate, null, null, null,
            SC.EmergencyShadowThresholdState.Unknown, "USD", "SCENARIO", new[] { $"EV-{id}" });

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
