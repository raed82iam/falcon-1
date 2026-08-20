using System.Runtime.CompilerServices;
using T = Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Behavior.Verifier;

internal static class Part8EvidenceLearningAdversarialChecks
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        Run();
    }

    private static void Run()
    {
        var analytics = new T.TradingEvidenceAnalyticsEngine();
        var readiness = new T.StrategyEvolutionReadinessEvaluator();
        var policy = new T.StrategyEvolutionEvidencePolicy(
            MinimumBaselineSamples: 4,
            MinimumCandidateSamples: 4,
            MinimumProcessValidityRatio: 0.75m,
            MinimumRiskAdjustedImprovement: 0.05m,
            AllowSimulationEvidenceForCandidateReview: false);

        var baselineScope = new T.TradingEvidenceScope("baseline-v1", "ALPACA", "ACCOUNT-1", "PAPER", "US", "DAILY", 7);
        var candidateScope = baselineScope with { StrategyId = "candidate-v2" };

        var baselineEvidence = new[]
        {
            E("b-1", "d-b-1", baselineScope, 0.10m, 0.08m, true),
            E("b-2", "d-b-2", baselineScope, -0.08m, -0.10m, true),
            E("b-3", "d-b-3", baselineScope, 0.06m, 0.04m, true),
            E("b-4", "d-b-4", baselineScope, 0.02m, 0.01m, true)
        };

        var candidateEvidence = new[]
        {
            E("c-1", "d-c-1", candidateScope, 0.20m, 0.18m, true),
            E("c-2", "d-c-2", candidateScope, -0.02m, -0.03m, true),
            E("c-3", "d-c-3", candidateScope, 0.14m, 0.12m, true),
            E("c-4", "d-c-4", candidateScope, 0.10m, 0.09m, true)
        };

        var baseline = analytics.Analyze(baselineEvidence);
        var candidate = analytics.Analyze(candidateEvidence);

        Require(baseline.Status == T.TradingEvidenceSetStatus.Valid, "P8 baseline evidence should be valid");
        Require(baseline.SampleCount == 4 && baseline.NegativeCount == 1, "P8 losses must remain in analytics");
        Require(candidate.Status == T.TradingEvidenceSetStatus.Valid, "P8 candidate evidence should be valid");

        var ready = readiness.Evaluate(baseline, candidate, policy);
        Require(ready.State == T.StrategyEvolutionReadinessState.ReadyForGovernedCandidateReview, "P8 qualified evidence may reach governed candidate review only");
        Require(!ready.GrantsAdoptionAuthority && !ready.GrantsDeploymentAuthority && !ready.GrantsRuntimeAuthority, "P8 candidate readiness must never grant authority");

        var badProcessEvidence = new[]
        {
            E("p-1", "d-p-1", candidateScope, 5m, 5m, true),
            E("p-2", "d-p-2", candidateScope, 5m, 5m, false),
            E("p-3", "d-p-3", candidateScope, 5m, 5m, false),
            E("p-4", "d-p-4", candidateScope, 5m, 5m, false)
        };
        var profitableBadProcess = readiness.Evaluate(baseline, analytics.Analyze(badProcessEvidence), policy);
        Require(profitableBadProcess.State == T.StrategyEvolutionReadinessState.NotReady && profitableBadProcess.ReasonCode == "PROCESS_VALIDITY_BELOW_THRESHOLD", "P8 profit must not launder invalid decision process");

        var duplicate = analytics.Analyze(new[]
        {
            E("dup", "d-1", candidateScope, 1m, 1m, true),
            E("dup", "d-2", candidateScope, 1m, 1m, true)
        });
        Require(duplicate.Status == T.TradingEvidenceSetStatus.Invalid && duplicate.ReasonCode == "DUPLICATE_EVIDENCE_ID", "P8 duplicate evidence must fail closed");

        var duplicateDecision = analytics.Analyze(new[]
        {
            E("dd-1", "same-decision", candidateScope, 1m, 1m, true),
            E("dd-2", "same-decision", candidateScope, 1m, 1m, true)
        });
        Require(duplicateDecision.Status == T.TradingEvidenceSetStatus.Invalid && duplicateDecision.ReasonCode == "DUPLICATE_DECISION_ID", "P8 duplicate decision identity must not inflate sample count");

        var stale = analytics.Analyze(new[]
        {
            E("stale-1", "d-s1", candidateScope, 1m, 1m, true) with { Truth = T.TradingEvidenceTruth.Stale }
        });
        Require(stale.Status == T.TradingEvidenceSetStatus.Invalid && stale.ReasonCode == "EVIDENCE_NOT_CURRENT_COMPLETE", "P8 stale evidence must fail closed");

        var conflicted = analytics.Analyze(new[]
        {
            E("conflict-1", "d-x1", candidateScope, 1m, 1m, true) with { Truth = T.TradingEvidenceTruth.Conflicted }
        });
        Require(conflicted.Status == T.TradingEvidenceSetStatus.Invalid, "P8 conflicted evidence must fail closed");

        var unknownSource = analytics.Analyze(new[]
        {
            E("source-1", "d-u1", candidateScope, 1m, 1m, true) with { Source = (T.TradingEvidenceSource)999 }
        });
        Require(unknownSource.Status == T.TradingEvidenceSetStatus.Invalid && unknownSource.ReasonCode == "EVIDENCE_SOURCE_UNKNOWN", "P8 unknown evidence source must fail closed");

        var mixedScope = analytics.Analyze(new[]
        {
            E("mix-1", "d-m1", candidateScope, 1m, 1m, true),
            E("mix-2", "d-m2", candidateScope with { BrokerAccountId = "ACCOUNT-2" }, 1m, 1m, true)
        });
        Require(mixedScope.Status == T.TradingEvidenceSetStatus.Invalid && mixedScope.ReasonCode == "MIXED_EVIDENCE_SCOPE", "P8 mixed broker/environment/market/horizon/epoch/strategy scope must fail closed");

        var environmentMismatchCandidate = analytics.Analyze(candidateEvidence.Select(x => x with { Scope = x.Scope with { Environment = "LIVE" } }));
        var environmentMismatch = readiness.Evaluate(baseline, environmentMismatchCandidate, policy);
        Require(environmentMismatch.State == T.StrategyEvolutionReadinessState.NotReady && environmentMismatch.ReasonCode == "BASELINE_CANDIDATE_SCOPE_MISMATCH", "P8 Paper and Live evidence must never be silently compared");

        var sameStrategyCandidate = analytics.Analyze(candidateEvidence.Select(x => x with { Scope = x.Scope with { StrategyId = baselineScope.StrategyId } }));
        var sameStrategy = readiness.Evaluate(baseline, sameStrategyCandidate, policy);
        Require(sameStrategy.State == T.StrategyEvolutionReadinessState.NotReady && sameStrategy.ReasonCode == "BASELINE_CANDIDATE_STRATEGY_IDENTITY_NOT_DISTINCT", "P8 baseline and candidate identities must be distinct");

        var overlapCandidate = analytics.Analyze(candidateEvidence.Select((x, index) => index == 0 ? x with { EvidenceId = "b-1" } : x));
        var overlap = readiness.Evaluate(baseline, overlapCandidate, policy);
        Require(overlap.State == T.StrategyEvolutionReadinessState.NotReady && overlap.ReasonCode == "BASELINE_CANDIDATE_EVIDENCE_OVERLAP", "P8 baseline/candidate evidence overlap must not double count support");

        var decisionOverlapCandidate = analytics.Analyze(candidateEvidence.Select((x, index) => index == 0 ? x with { DecisionId = "d-b-1" } : x));
        var decisionOverlap = readiness.Evaluate(baseline, decisionOverlapCandidate, policy);
        Require(decisionOverlap.State == T.StrategyEvolutionReadinessState.NotReady && decisionOverlap.ReasonCode == "BASELINE_CANDIDATE_DECISION_OVERLAP", "P8 baseline/candidate decision overlap must not double count one governed decision through different evidence identities");

        var tooSmall = readiness.Evaluate(
            baseline,
            analytics.Analyze(candidateEvidence.Take(3)),
            policy);
        Require(tooSmall.State == T.StrategyEvolutionReadinessState.NotReady && tooSmall.ReasonCode == "INSUFFICIENT_SAMPLE", "P8 insufficient sample must not be promoted");

        var underperformingEvidence = new[]
        {
            E("u-1", "d-u-1", candidateScope, 0.01m, 0.00m, true),
            E("u-2", "d-u-2", candidateScope, 0.01m, 0.00m, true),
            E("u-3", "d-u-3", candidateScope, 0.01m, 0.00m, true),
            E("u-4", "d-u-4", candidateScope, 0.01m, 0.00m, true)
        };
        var underperforming = readiness.Evaluate(baseline, analytics.Analyze(underperformingEvidence), policy);
        Require(underperforming.State == T.StrategyEvolutionReadinessState.NotReady && underperforming.ReasonCode == "INSUFFICIENT_RISK_ADJUSTED_IMPROVEMENT", "P8 candidate must beat evidence policy, not raw optimism");

        var simulationEvidence = candidateEvidence
            .Select(x => x with { EvidenceId = "sim-" + x.EvidenceId, DecisionId = "sim-" + x.DecisionId, Source = T.TradingEvidenceSource.Simulation, Scope = x.Scope with { BrokerId = "SIMULATOR", BrokerAccountId = "SCENARIO-ACCOUNT", Environment = "SIMULATION" } })
            .ToArray();
        var simulationCandidate = analytics.Analyze(simulationEvidence);
        var simulationBaseline = analytics.Analyze(baselineEvidence.Select(x => x with { EvidenceId = "sim-" + x.EvidenceId, DecisionId = "sim-" + x.DecisionId, Source = T.TradingEvidenceSource.Simulation, Scope = x.Scope with { BrokerId = "SIMULATOR", BrokerAccountId = "SCENARIO-ACCOUNT", Environment = "SIMULATION" } }));
        var simulationBlocked = readiness.Evaluate(simulationBaseline, simulationCandidate, policy);
        Require(simulationBlocked.State == T.StrategyEvolutionReadinessState.NotReady && simulationBlocked.ReasonCode == "SIMULATION_EVIDENCE_NOT_ALLOWED_BY_POLICY", "P8 simulation must not masquerade as operational evidence");

        var simulationReviewPolicy = policy with { AllowSimulationEvidenceForCandidateReview = true };
        var simulationReview = readiness.Evaluate(simulationBaseline, simulationCandidate, simulationReviewPolicy);
        Require(simulationReview.State == T.StrategyEvolutionReadinessState.ReadyForGovernedCandidateReview && simulationReview.ContainsSimulationEvidence, "P8 simulation may support bounded review only when policy explicitly allows it");
        Require(!simulationReview.GrantsAdoptionAuthority && !simulationReview.GrantsDeploymentAuthority && !simulationReview.GrantsRuntimeAuthority, "P8 simulation review cannot create production authority");

        var replayEvidence = candidateEvidence
            .Select(x => x with { EvidenceId = "replay-" + x.EvidenceId, DecisionId = "replay-" + x.DecisionId, Source = T.TradingEvidenceSource.Replay })
            .ToArray();
        var replayDecision = readiness.Evaluate(baseline, analytics.Analyze(replayEvidence), simulationReviewPolicy);
        Require(replayDecision.State == T.StrategyEvolutionReadinessState.NotReady && replayDecision.ReasonCode == "REPLAY_EVIDENCE_NOT_ELIGIBLE_FOR_CANDIDATE_READINESS", "P8 replay evidence cannot become candidate-readiness authority");

        var forward = analytics.Analyze(candidateEvidence);
        var reverse = analytics.Analyze(candidateEvidence.Reverse());
        Require(forward.AverageOutcome == reverse.AverageOutcome &&
                forward.AverageRiskAdjustedOutcome == reverse.AverageRiskAdjustedOutcome &&
                forward.ProcessValidityRatio == reverse.ProcessValidityRatio &&
                forward.EvidenceIds.SequenceEqual(reverse.EvidenceIds) &&
                forward.DecisionIds.SequenceEqual(reverse.DecisionIds),
            "P8 analytics must be deterministic regardless of input order");
    }

    private static T.TradingOutcomeEvidence E(
        string evidenceId,
        string decisionId,
        T.TradingEvidenceScope scope,
        decimal outcome,
        decimal riskAdjustedOutcome,
        bool processValid) =>
        new(
            evidenceId,
            decisionId,
            scope,
            T.TradingEvidenceSource.Operational,
            T.TradingEvidenceTruth.Current,
            outcome,
            riskAdjustedOutcome,
            processValid,
            Complete: true);

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
