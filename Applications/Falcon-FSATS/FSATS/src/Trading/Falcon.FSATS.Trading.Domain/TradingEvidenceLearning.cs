namespace Falcon.FSATS.Trading.Domain;

public enum TradingEvidenceSource
{
    Operational,
    Simulation,
    Replay
}

public enum TradingEvidenceTruth
{
    Current,
    Stale,
    Conflicted,
    Incomplete
}

public sealed record TradingEvidenceScope(
    string StrategyId,
    string BrokerId,
    string BrokerAccountId,
    string Environment,
    string MarketId,
    string Horizon,
    long TrustEpoch);

public sealed record TradingOutcomeEvidence(
    string EvidenceId,
    string DecisionId,
    TradingEvidenceScope Scope,
    TradingEvidenceSource Source,
    TradingEvidenceTruth Truth,
    decimal OutcomeValue,
    decimal RiskAdjustedOutcome,
    bool DecisionProcessValid,
    bool Complete);

public enum TradingEvidenceSetStatus
{
    Valid,
    Invalid
}

public sealed record TradingEvidenceAnalytics(
    TradingEvidenceSetStatus Status,
    string ReasonCode,
    TradingEvidenceScope? Scope,
    int SampleCount,
    int PositiveCount,
    int NegativeCount,
    int FlatCount,
    decimal AverageOutcome,
    decimal AverageRiskAdjustedOutcome,
    decimal ProcessValidityRatio,
    bool ContainsSimulation,
    bool ContainsReplay,
    IReadOnlyList<string> EvidenceIds,
    IReadOnlyList<string> DecisionIds)
{
    public static TradingEvidenceAnalytics Invalid(string reasonCode) =>
        new(
            TradingEvidenceSetStatus.Invalid,
            reasonCode,
            null,
            0,
            0,
            0,
            0,
            0m,
            0m,
            0m,
            false,
            false,
            Array.Empty<string>(),
            Array.Empty<string>());
}

public sealed class TradingEvidenceAnalyticsEngine
{
    public TradingEvidenceAnalytics Analyze(IEnumerable<TradingOutcomeEvidence>? evidence)
    {
        if (evidence is null)
        {
            return TradingEvidenceAnalytics.Invalid("EVIDENCE_SET_MISSING");
        }

        var records = evidence.ToArray();
        if (records.Length == 0)
        {
            return TradingEvidenceAnalytics.Invalid("EVIDENCE_SET_EMPTY");
        }

        if (records.Any(x =>
                string.IsNullOrWhiteSpace(x.EvidenceId) ||
                string.IsNullOrWhiteSpace(x.DecisionId) ||
                x.Scope is null ||
                string.IsNullOrWhiteSpace(x.Scope.StrategyId) ||
                string.IsNullOrWhiteSpace(x.Scope.BrokerId) ||
                string.IsNullOrWhiteSpace(x.Scope.BrokerAccountId) ||
                string.IsNullOrWhiteSpace(x.Scope.Environment) ||
                string.IsNullOrWhiteSpace(x.Scope.MarketId) ||
                string.IsNullOrWhiteSpace(x.Scope.Horizon) ||
                x.Scope.TrustEpoch < 0))
        {
            return TradingEvidenceAnalytics.Invalid("EVIDENCE_IDENTITY_INVALID");
        }

        if (records.Any(x => !Enum.IsDefined(typeof(TradingEvidenceSource), x.Source)))
        {
            return TradingEvidenceAnalytics.Invalid("EVIDENCE_SOURCE_UNKNOWN");
        }

        if (records.Any(x => !Enum.IsDefined(typeof(TradingEvidenceTruth), x.Truth)))
        {
            return TradingEvidenceAnalytics.Invalid("EVIDENCE_TRUTH_UNKNOWN");
        }

        if (records.GroupBy(x => x.EvidenceId, StringComparer.Ordinal).Any(x => x.Count() != 1))
        {
            return TradingEvidenceAnalytics.Invalid("DUPLICATE_EVIDENCE_ID");
        }

        if (records.GroupBy(x => x.DecisionId, StringComparer.Ordinal).Any(x => x.Count() != 1))
        {
            return TradingEvidenceAnalytics.Invalid("DUPLICATE_DECISION_ID");
        }

        if (records.Any(x => !x.Complete || x.Truth != TradingEvidenceTruth.Current))
        {
            return TradingEvidenceAnalytics.Invalid("EVIDENCE_NOT_CURRENT_COMPLETE");
        }

        var scope = records[0].Scope;
        if (records.Any(x => x.Scope != scope))
        {
            return TradingEvidenceAnalytics.Invalid("MIXED_EVIDENCE_SCOPE");
        }

        var positive = records.Count(x => x.OutcomeValue > 0m);
        var negative = records.Count(x => x.OutcomeValue < 0m);
        var flat = records.Length - positive - negative;
        var averageOutcome = records.Sum(x => x.OutcomeValue) / records.Length;
        var averageRiskAdjusted = records.Sum(x => x.RiskAdjustedOutcome) / records.Length;
        var processValidity = records.Count(x => x.DecisionProcessValid) / (decimal)records.Length;
        var evidenceIds = records
            .Select(x => x.EvidenceId)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();
        var decisionIds = records
            .Select(x => x.DecisionId)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        return new TradingEvidenceAnalytics(
            TradingEvidenceSetStatus.Valid,
            "VALID",
            scope,
            records.Length,
            positive,
            negative,
            flat,
            averageOutcome,
            averageRiskAdjusted,
            processValidity,
            records.Any(x => x.Source == TradingEvidenceSource.Simulation),
            records.Any(x => x.Source == TradingEvidenceSource.Replay),
            evidenceIds,
            decisionIds);
    }
}

public sealed record StrategyEvolutionEvidencePolicy(
    int MinimumBaselineSamples,
    int MinimumCandidateSamples,
    decimal MinimumProcessValidityRatio,
    decimal MinimumRiskAdjustedImprovement,
    bool AllowSimulationEvidenceForCandidateReview)
{
    public bool IsValid =>
        MinimumBaselineSamples > 0 &&
        MinimumCandidateSamples > 0 &&
        MinimumProcessValidityRatio is >= 0m and <= 1m &&
        MinimumRiskAdjustedImprovement >= 0m;
}

public enum StrategyEvolutionReadinessState
{
    NotReady,
    ReadyForGovernedCandidateReview
}

public sealed record StrategyEvolutionReadinessDecision(
    StrategyEvolutionReadinessState State,
    string ReasonCode,
    string BaselineStrategyId,
    string CandidateStrategyId,
    decimal RiskAdjustedImprovement,
    IReadOnlyList<string> EvidenceIds,
    bool ContainsSimulationEvidence,
    bool GrantsAdoptionAuthority,
    bool GrantsDeploymentAuthority,
    bool GrantsRuntimeAuthority);

public sealed class StrategyEvolutionReadinessEvaluator
{
    public StrategyEvolutionReadinessDecision Evaluate(
        TradingEvidenceAnalytics baseline,
        TradingEvidenceAnalytics candidate,
        StrategyEvolutionEvidencePolicy policy)
    {
        var baselineId = baseline.Scope?.StrategyId ?? string.Empty;
        var candidateId = candidate.Scope?.StrategyId ?? string.Empty;
        var evidenceIds = baseline.EvidenceIds
            .Concat(candidate.EvidenceIds)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();
        var containsSimulation = baseline.ContainsSimulation || candidate.ContainsSimulation;

        StrategyEvolutionReadinessDecision NotReady(string reasonCode, decimal improvement = 0m) =>
            new(
                StrategyEvolutionReadinessState.NotReady,
                reasonCode,
                baselineId,
                candidateId,
                improvement,
                evidenceIds,
                containsSimulation,
                GrantsAdoptionAuthority: false,
                GrantsDeploymentAuthority: false,
                GrantsRuntimeAuthority: false);

        if (!policy.IsValid)
        {
            return NotReady("POLICY_INVALID");
        }

        if (baseline.Status != TradingEvidenceSetStatus.Valid || candidate.Status != TradingEvidenceSetStatus.Valid || baseline.Scope is null || candidate.Scope is null)
        {
            return NotReady("EVIDENCE_SET_INVALID");
        }

        if (string.Equals(baseline.Scope.StrategyId, candidate.Scope.StrategyId, StringComparison.Ordinal))
        {
            return NotReady("BASELINE_CANDIDATE_STRATEGY_IDENTITY_NOT_DISTINCT");
        }

        if (baseline.Scope.BrokerId != candidate.Scope.BrokerId ||
            baseline.Scope.BrokerAccountId != candidate.Scope.BrokerAccountId ||
            baseline.Scope.Environment != candidate.Scope.Environment ||
            baseline.Scope.MarketId != candidate.Scope.MarketId ||
            baseline.Scope.Horizon != candidate.Scope.Horizon ||
            baseline.Scope.TrustEpoch != candidate.Scope.TrustEpoch)
        {
            return NotReady("BASELINE_CANDIDATE_SCOPE_MISMATCH");
        }

        if (baseline.EvidenceIds.Intersect(candidate.EvidenceIds, StringComparer.Ordinal).Any())
        {
            return NotReady("BASELINE_CANDIDATE_EVIDENCE_OVERLAP");
        }

        if (baseline.DecisionIds.Intersect(candidate.DecisionIds, StringComparer.Ordinal).Any())
        {
            return NotReady("BASELINE_CANDIDATE_DECISION_OVERLAP");
        }

        if (baseline.ContainsReplay || candidate.ContainsReplay)
        {
            return NotReady("REPLAY_EVIDENCE_NOT_ELIGIBLE_FOR_CANDIDATE_READINESS");
        }

        if (containsSimulation && !policy.AllowSimulationEvidenceForCandidateReview)
        {
            return NotReady("SIMULATION_EVIDENCE_NOT_ALLOWED_BY_POLICY");
        }

        if (baseline.SampleCount < policy.MinimumBaselineSamples || candidate.SampleCount < policy.MinimumCandidateSamples)
        {
            return NotReady("INSUFFICIENT_SAMPLE");
        }

        if (baseline.ProcessValidityRatio < policy.MinimumProcessValidityRatio || candidate.ProcessValidityRatio < policy.MinimumProcessValidityRatio)
        {
            return NotReady("PROCESS_VALIDITY_BELOW_THRESHOLD");
        }

        var improvement = candidate.AverageRiskAdjustedOutcome - baseline.AverageRiskAdjustedOutcome;
        if (improvement < policy.MinimumRiskAdjustedImprovement)
        {
            return NotReady("INSUFFICIENT_RISK_ADJUSTED_IMPROVEMENT", improvement);
        }

        return new StrategyEvolutionReadinessDecision(
            StrategyEvolutionReadinessState.ReadyForGovernedCandidateReview,
            "READY_FOR_GOVERNED_CANDIDATE_REVIEW",
            baselineId,
            candidateId,
            improvement,
            evidenceIds,
            containsSimulation,
            GrantsAdoptionAuthority: false,
            GrantsDeploymentAuthority: false,
            GrantsRuntimeAuthority: false);
    }
}
