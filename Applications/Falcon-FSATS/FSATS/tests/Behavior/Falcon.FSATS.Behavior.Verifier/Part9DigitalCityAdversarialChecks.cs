using C = Falcon.FSATS.FSTSimA.Contracts;
using D = Falcon.FSATS.FSTSimA.Domain;
using A = Falcon.FSATS.FSTSimA.Application;

internal static class Part9DigitalCityAdversarialChecks
{
    internal static void Run()
    {
        SameScenarioIsBitStable();
        ReorderedFaultInputProducesSameOrderedTruth();
        SimulationNeverBecomesOperationalTruthOrAuthority();
        IndependentCalibrationEvidenceIsRequiredForQualificationRecommendation();
        MissingOrInvalidCalibrationEvidenceFailsClosed();
        InvalidScenarioFailsClosed();
        ScopeIdentityChangesEvidenceIdentity();
    }

    private static void SameScenarioIsBitStable()
    {
        var firstSink = new RecordingSink();
        var secondSink = new RecordingSink();
        var scenario = Scenario();

        var first = Coordinator(firstSink).Run(scenario);
        var second = Coordinator(secondSink).Run(scenario);

        Require(first.Reproducible, "P9_DIGITAL_CITY_NOT_REPRODUCIBLE");
        Require(first.DeterministicDigestSha256 == second.DeterministicDigestSha256, "P9_DIGITAL_CITY_DIGEST_DRIFT");
        Require(first.EvidenceId == second.EvidenceId, "P9_DIGITAL_CITY_EVIDENCE_ID_DRIFT");
        Require(firstSink.Commits.Count == 1 && secondSink.Commits.Count == 1, "P9_DIGITAL_CITY_EVIDENCE_NOT_COMMITTED_EXACTLY_ONCE");
    }

    private static void ReorderedFaultInputProducesSameOrderedTruth()
    {
        var forward = Scenario();
        var reversed = forward with { Faults = forward.Faults.Reverse().ToArray() };

        var a = Coordinator(new RecordingSink()).Run(forward);
        var b = Coordinator(new RecordingSink()).Run(reversed);

        Require(a.FaultOrderDeterministic && b.FaultOrderDeterministic, "P9_DIGITAL_CITY_FAULT_ORDER_NOT_DETERMINISTIC");
        Require(a.DeterministicDigestSha256 == b.DeterministicDigestSha256, "P9_DIGITAL_CITY_FAULT_INPUT_ORDER_CHANGED_RESULT");
    }

    private static void SimulationNeverBecomesOperationalTruthOrAuthority()
    {
        foreach (var scenarioClass in Enum.GetValues<A.DigitalCityScenarioClass>())
        {
            var result = Coordinator(new RecordingSink()).Run(Scenario() with { ScenarioClass = scenarioClass });
            Require(!result.OperationalTruth, "P9_SIMULATION_ESCALATED_TO_OPERATIONAL_TRUTH");
            Require(!result.GrantsRuntimeAuthority, "P9_SIMULATION_GRANTED_RUNTIME_AUTHORITY");
            Require(!result.GrantsPaperAuthority, "P9_SIMULATION_GRANTED_PAPER_AUTHORITY");
            Require(!result.GrantsLiveAuthority, "P9_SIMULATION_GRANTED_LIVE_AUTHORITY");
        }
    }

    private static void IndependentCalibrationEvidenceIsRequiredForQualificationRecommendation()
    {
        var rejected = Coordinator(new RecordingSink()).Run(Scenario() with { IndependentCalibrationEvidence = false });
        Require(rejected.Recommendation == "NOT_READY", "P9_NONINDEPENDENT_CALIBRATION_ACCEPTED");

        var qualified = Coordinator(new RecordingSink()).Run(Scenario() with { IndependentCalibrationEvidence = true, FidelityScore = 0.95m });
        Require(qualified.Recommendation == "READY_FOR_PAPER_QUALIFICATION_REVIEW", "P9_VALID_NONLIVE_QUALIFICATION_NOT_RECOGNIZED");
        Require(!qualified.GrantsPaperAuthority, "P9_QUALIFICATION_RECOMMENDATION_CONFUSED_WITH_PAPER_AUTHORITY");
    }

    private static void MissingOrInvalidCalibrationEvidenceFailsClosed()
    {
        RequireThrows(
            () => Coordinator(new RecordingSink()).Run(Scenario() with { CalibrationEvidence = null }),
            "P9_MISSING_CALIBRATION_EVIDENCE_REFERENCE_ACCEPTED");

        RequireThrows(
            () => Coordinator(new RecordingSink()).Run(Scenario() with
            {
                CalibrationEvidence = new A.CalibrationEvidenceReference(
                    "CAL-P9-001",
                    "NOT-A-SHA256",
                    "INDEPENDENT-CALIBRATION-HARNESS",
                    new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero))
            }),
            "P9_INVALID_CALIBRATION_EVIDENCE_REFERENCE_ACCEPTED");
    }

    private static void InvalidScenarioFailsClosed()
    {
        RequireThrows(() => Coordinator(new RecordingSink()).Run(Scenario() with { ScenarioId = " " }), "P9_EMPTY_SCENARIO_ACCEPTED");
        RequireThrows(() => Coordinator(new RecordingSink()).Run(Scenario() with { TickCount = 0 }), "P9_ZERO_TICKS_ACCEPTED");
        RequireThrows(() => Coordinator(new RecordingSink()).Run(Scenario() with { StartPrice = 0m }), "P9_ZERO_PRICE_ACCEPTED");
        RequireThrows(() => Coordinator(new RecordingSink()).Run(Scenario() with { FidelityScore = 1.01m }), "P9_INVALID_FIDELITY_ACCEPTED");
    }

    private static void ScopeIdentityChangesEvidenceIdentity()
    {
        var global = Coordinator(new RecordingSink()).Run(Scenario());
        var scoped = Coordinator(new RecordingSink()).Run(Scenario() with
        {
            Scope = new C.SimulationScope("BROKER-SCOPE", "ALPACA", "PA-ACCOUNT-A", "PAPER")
        });

        Require(global.ScopeKey != scoped.ScopeKey, "P9_SCOPE_IDENTITY_COLLAPSED");
        Require(global.EvidenceId != scoped.EvidenceId, "P9_SCOPE_NOT_BOUND_TO_EVIDENCE_ID");
        Require(global.DeterministicDigestSha256 != scoped.DeterministicDigestSha256, "P9_SCOPE_NOT_BOUND_TO_DETERMINISTIC_DIGEST");
    }

    private static A.DigitalCityScenario Scenario() => new(
        "P9-STRESS-001",
        9917,
        A.DigitalCityScenarioClass.FaultInjected,
        C.SimulationScope.Global("DIGITAL-CITY"),
        64,
        100m,
        "STRESS",
        new D.FaultEvent[]
        {
            new(D.FaultType.ProviderDelay, new D.SimulationInstant(20), "provider-a", "delay=250ms"),
            new(D.FaultType.ResourcePressure, new D.SimulationInstant(5), "fstsim", "cpu=constrained"),
            new(D.FaultType.BrokerAmbiguity, new D.SimulationInstant(12), "broker-a", "ack=unknown")
        },
        true,
        0.95m)
    {
        CalibrationEvidence = new A.CalibrationEvidenceReference(
            "CAL-P9-001",
            "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF",
            "INDEPENDENT-CALIBRATION-HARNESS",
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero))
    };

    private static A.DigitalCityValidationCoordinator Coordinator(A.ISimulationEvidenceSink sink) => new(
        new D.SyntheticMarketGenerator(),
        new D.FaultInjector(),
        new D.ValidationAssessor(),
        sink);

    private static void RequireThrows(Action action, string failure)
    {
        try
        {
            action();
        }
        catch (ArgumentException)
        {
            return;
        }

        throw new InvalidOperationException(failure);
    }

    private static void Require(bool condition, string failure)
    {
        if (!condition) throw new InvalidOperationException(failure);
    }

    private sealed class RecordingSink : A.ISimulationEvidenceSink
    {
        public List<(string EvidenceId, string ScenarioId, int Seed, string Digest)> Commits { get; } = new();

        public void Commit(string evidenceId, string scenarioId, int seed, string digest)
        {
            if (Commits.Any(x => x.EvidenceId == evidenceId)) throw new InvalidOperationException("DUPLICATE_EVIDENCE_ID");
            Commits.Add((evidenceId, scenarioId, seed, digest));
        }
    }
}
