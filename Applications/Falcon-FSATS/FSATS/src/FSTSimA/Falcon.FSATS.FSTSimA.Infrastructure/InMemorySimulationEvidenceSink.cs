using Falcon.FSATS.FSTSimA.Application;

namespace Falcon.FSATS.FSTSimA.Infrastructure;

public sealed class InMemorySimulationEvidenceSink : ISimulationEvidenceSink
{
    private readonly Dictionary<string, (string ScenarioId, int Seed, string Digest)> _evidence = new(StringComparer.Ordinal);

    public void Commit(string evidenceId, string scenarioId, int seed, string digest)
    {
        if (!_evidence.TryAdd(evidenceId, (scenarioId, seed, digest))) throw new InvalidOperationException("DUPLICATE_EVIDENCE_ID");
    }

    public IReadOnlyDictionary<string, (string ScenarioId, int Seed, string Digest)> Snapshot()
        => new Dictionary<string, (string ScenarioId, int Seed, string Digest)>(_evidence, StringComparer.Ordinal);
}

public sealed class DisabledOperationalEgress
{
    public bool IsOperationalEgressAuthorized => false;
    public string ReasonCode => "FSTSIMA_OPERATIONAL_EGRESS_FORBIDDEN";
}
