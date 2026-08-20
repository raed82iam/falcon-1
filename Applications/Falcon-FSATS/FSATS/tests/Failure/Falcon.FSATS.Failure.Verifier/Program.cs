using T = Falcon.FSATS.Trading.Domain;
using P = Falcon.FSATS.FSAPMA.Domain;
using G = Falcon.FSATS.TradingGuardian.Domain;
using S = Falcon.FSATS.FSTSimA.Domain;
using R = Falcon.FSATS.ResourceManagement.Domain;

var failures = new List<string>();
var checks = 0;

var dataQuality = new P.AnomalyDetector().Evaluate(100m, 100m, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch.AddMinutes(5), TimeSpan.FromSeconds(30));
Check(dataQuality.State == P.QualityState.Stale, "Provider data must become stale");

var usd = new T.Currency("USD");
var risk = T.UnifiedRiskGate.Evaluate(new T.RiskRequest(new T.InstrumentId("TEST"), new T.Quantity(5m), new T.Money(10m, usd), new T.Money(20m, usd), GuardianAllowsNewRisk: false, DataIsTrusted: dataQuality.State == P.QualityState.Healthy));
Check(risk.Decision == T.RiskDecision.Denied, "New risk must be denied when provider/Guardian state is unsafe");

Check(!T.TrustEpochFence.IsEligible(new T.TrustEpoch(4), new T.TrustEpoch(5), riskIncreasing: true), "Queued risk-increasing work from killed AI epoch must be fenced");
Check(T.TrustEpochFence.IsEligible(new T.TrustEpoch(4), new T.TrustEpoch(5), riskIncreasing: false), "Independent risk-reducing work is not automatically fenced");

var order = new T.OrderLifecycle();
order.SubmissionAttempt(); order.BrokerAck(); order.PartialFill(); order.MarkAmbiguous();
Check(order.State == T.OrderState.ReconciliationRequired, "Partial-fill ambiguity must preserve reconciliation ownership");

var safety = G.DeterministicSafetyKernel.Decide(new G.SafetyContext(IntelligenceTrusted: false, ExecutionTruthKnown: false, ProtectionVerified: false, ExposureExists: true, ExitPolicyAuthorized: true));
Check(safety.Contains(G.SafetyAction.DenyExpansion), "Guardian deterministic path must deny expansion");
Check(safety.Contains(G.SafetyAction.Reconcile), "Unknown exposure truth must reconcile");
Check(!safety.Contains(G.SafetyAction.Exit), "Unknown exposure truth must not blind-liquidate");

var now = DateTimeOffset.UnixEpoch.AddDays(1);
var currentEnvelope = new R.FoundationEnvelope("foundation-env-9", "CPU", 100m, DateTimeOffset.UnixEpoch, DateTimeOffset.MaxValue, false);
Check(!R.ResourceEpochFence.IsCurrent(new R.CoordinationEpoch(8), new R.CoordinationEpoch(9), "foundation-env-9", currentEnvelope, now), "Restarted stale APP-RSC epoch must be fenced");
var staleClaim = new R.ResourceClaim("FSTSimA", "CPU", 30m, 20m, 5m, 30m, 25m, 20, Fresh: false, IntegrityTrusted: true);
Check(!R.DemandIntegrityEvaluator.IsEligible(staleClaim), "Stale resource claim cannot drive redistribution");

var generator = new S.SyntheticMarketGenerator();
var run1 = generator.Generate(1234, 50, 100m, new S.SimulationInstant(0), "STRESS");
var run2 = generator.Generate(1234, 50, 100m, new S.SimulationInstant(0), "STRESS");
Check(run1.SequenceEqual(run2), "Failure scenario replay must be reproducible");
var faults = new S.FaultInjector().Order(new[]
{
    new S.FaultEvent(S.FaultType.AiKill, new S.SimulationInstant(3), "Trading", "epoch=5"),
    new S.FaultEvent(S.FaultType.ResourcePressure, new S.SimulationInstant(3), "APP-RSC", "CPU"),
    new S.FaultEvent(S.FaultType.ProviderOutage, new S.SimulationInstant(1), "FSAPMA", "provider=test"),
    new S.FaultEvent(S.FaultType.BrokerAmbiguity, new S.SimulationInstant(2), "Trading", "partial-fill")
});
Check(faults[0].Type == S.FaultType.ProviderOutage && faults[^1].At.Ticks == 3, "Fault ordering must be deterministic");

if (failures.Count > 0)
{
    Console.Error.WriteLine($"FSATS FAILURE VERIFIER: FAIL ({checks - failures.Count}/{checks})");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}
Console.WriteLine($"FSATS FAILURE VERIFIER: PASS ({checks}/{checks}; composite degradation/kill/reconciliation/resource/replay scenario)");
return 0;

void Check(bool condition, string message) { checks++; if (!condition) failures.Add(message); }
