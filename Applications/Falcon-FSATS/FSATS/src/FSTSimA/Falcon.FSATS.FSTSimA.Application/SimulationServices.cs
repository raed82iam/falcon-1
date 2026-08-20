using Falcon.FSATS.FSTSimA.Contracts;
using Falcon.FSATS.FSTSimA.Domain;

namespace Falcon.FSATS.FSTSimA.Application;

public interface ISimulationEvidenceSink
{
    void Commit(string evidenceId, string scenarioId, int seed, string digest);
}

public sealed class SimulationCoordinator
{
    private readonly SyntheticMarketGenerator _market;
    private readonly ValidationAssessor _assessor;
    private readonly ISimulationEvidenceSink _evidence;

    public SimulationCoordinator(SyntheticMarketGenerator market, ValidationAssessor assessor, ISimulationEvidenceSink evidence)
    {
        _market = market;
        _assessor = assessor;
        _evidence = evidence;
    }

    public ValidationAssessment RunDeterminismQualification(string scenarioId, int seed, decimal startPrice)
        => RunDeterminismQualification(SimulationScope.Global(), scenarioId, seed, startPrice);

    public ValidationAssessment RunDeterminismQualification(SimulationScope scope, string scenarioId, int seed, decimal startPrice)
    {
        ArgumentNullException.ThrowIfNull(scope);
        if (string.IsNullOrWhiteSpace(scenarioId)) throw new ArgumentException("SCENARIO_ID_REQUIRED", nameof(scenarioId));

        var normalizedScenarioId = scenarioId.Trim();
        var first = _market.Generate(seed, 32, startPrice, new SimulationInstant(0), "NORMAL");
        var second = _market.Generate(seed, 32, startPrice, new SimulationInstant(0), "NORMAL");
        var reproducible = first.SequenceEqual(second);
        var digest = string.Join('|', first.Select(x => $"{x.Time.Ticks}:{x.Price}:{x.Volume}:{x.Regime}"));
        var evidenceId = $"evidence:{scope.CanonicalKey}:{Uri.EscapeDataString(normalizedScenarioId)}:{seed}";
        _evidence.Commit(evidenceId, normalizedScenarioId, seed, digest);
        return _assessor.Assess(reproducible, calibrationEvidenceExternalToAssessor: true, fidelityScore: reproducible ? 1m : 0m);
    }
}
