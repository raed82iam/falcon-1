namespace Falcon.FSATS.Trading.Domain;

public sealed record MarketInstrumentProfile(InstrumentId Instrument, bool Eligible, decimal LiquidityScore, decimal VolatilityScore, decimal DataQualityScore);
public sealed record OpportunityCandidate(InstrumentId Instrument, decimal Score, decimal Confidence, string EvidenceId);
public sealed record StrategyAssessment(string StrategyId, InstrumentId Instrument, decimal Confidence, decimal QualityScore, TrustEpoch Epoch, string EvidenceId);
public sealed record StrategySelection(string? StrategyId, InstrumentId Instrument, decimal Confidence, string ReasonCode, TrustEpoch Epoch);

public sealed class InstrumentUniverseService
{
    public IReadOnlyList<MarketInstrumentProfile> RankCandidates(IEnumerable<MarketInstrumentProfile> profiles, int maximum)
    {
        if (maximum <= 0) return Array.Empty<MarketInstrumentProfile>();
        return profiles.Where(x => x.Eligible && x.DataQualityScore > 0m)
            .OrderByDescending(x => x.LiquidityScore * 0.5m + x.DataQualityScore * 0.35m - x.VolatilityScore * 0.15m)
            .ThenBy(x => x.Instrument.Value, StringComparer.Ordinal)
            .Take(maximum)
            .ToArray();
    }
}

public sealed class OpportunityDiscoveryEngine
{
    public OpportunityCandidate? Discover(MarketInstrumentProfile profile, decimal activityScore, decimal liquidityDislocationScore, string evidenceId)
    {
        if (!profile.Eligible || profile.DataQualityScore <= 0m) return null;
        var score = decimal.Clamp(activityScore * 0.6m + liquidityDislocationScore * 0.4m, 0m, 1m);
        if (score < 0.55m) return null;
        return new(profile.Instrument, score, decimal.Clamp(profile.DataQualityScore * score, 0m, 1m), evidenceId);
    }
}

public sealed class StrategyController
{
    public StrategySelection Select(InstrumentId instrument, IEnumerable<StrategyAssessment> assessments, TrustEpoch currentEpoch)
    {
        var valid = assessments.Where(x => x.Instrument == instrument && x.Epoch == currentEpoch && x.Confidence is >= 0m and <= 1m && x.QualityScore > 0m)
            .OrderByDescending(x => x.Confidence * x.QualityScore)
            .ThenBy(x => x.StrategyId, StringComparer.Ordinal)
            .ToArray();
        if (valid.Length == 0) return new(null, instrument, 0m, "NO_ELIGIBLE_STRATEGY", currentEpoch);
        var winner = valid[0];
        if (valid.Length > 1 && Math.Abs((winner.Confidence * winner.QualityScore) - (valid[1].Confidence * valid[1].QualityScore)) < 0.01m)
            return new(null, instrument, Math.Min(winner.Confidence, valid[1].Confidence), "UNRESOLVED_STRATEGY_CONFLICT", currentEpoch);
        return new(winner.StrategyId, instrument, winner.Confidence, "SELECTED", currentEpoch);
    }
}

public sealed record EvolutionCandidate(string CandidateId, string ParentStrategyId, string ChangeDescription, string EvidenceId, bool ProductionEligible);

public sealed class StrategyEvolutionEngine
{
    public EvolutionCandidate Propose(string candidateId, string parentStrategyId, string changeDescription, string evidenceId)
        => new(candidateId, parentStrategyId, changeDescription, evidenceId, ProductionEligible: false);
}

public sealed record AttributionRecord(string DecisionId, string StrategyId, decimal OutcomeValue, string EvidenceId);
public sealed class TradingKnowledgeLedger
{
    private readonly List<AttributionRecord> _records = new();
    public void Append(AttributionRecord record) => _records.Add(record);
    public IReadOnlyList<AttributionRecord> Snapshot() => _records.ToArray();
}
