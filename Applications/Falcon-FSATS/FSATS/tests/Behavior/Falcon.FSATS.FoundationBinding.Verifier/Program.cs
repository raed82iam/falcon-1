using RM = Falcon.FSATS.ResourceManagement.Contracts;
using TR = Falcon.FSATS.Trading.Contracts;
using TA = Falcon.FSATS.Trading.Awareness;
using FA = Falcon.FSATS.FSAPMA.Awareness;
using GA = Falcon.FSATS.TradingGuardian.Awareness;
using SA = Falcon.FSATS.FSTSimA.Awareness;
using RA = Falcon.FSATS.ResourceManagement.Awareness;
using FP = Falcon.FSATS.FSAPMA.Contracts;
using FS = Falcon.FSATS.FSTSimA.Contracts;
using GQ = Falcon.FSATS.TradingGuardian.Contracts;

var checks = new List<(string Name, bool Pass)>();
void Check(string name, bool pass) => checks.Add((name, pass));
var now = DateTimeOffset.UtcNow;
const string hash = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

var appProjection = new RM.FoundationResourceProjectionDescriptor(RM.FoundationResourceProjectionBinding.ApplicationArtifactId, RM.FoundationResourceProjectionBinding.ArtifactVersion, RM.FoundationResourceProjectionBinding.CompatibilityIdentity, RM.FoundationResourceProjectionBinding.ApplicationSourceContract, "evidence:foundation:resource", RM.FoundationResourceProjectionBinding.FoundationCandidateCommit, RM.FoundationResourceProjectionKind.ApplicationResourceState, now, hash, true, false, false);
var appDecision = RM.FoundationResourceProjectionBinding.Evaluate(appProjection, now, TimeSpan.FromMinutes(5));
Check("FCR0010 exact resource descriptor accepted", appDecision.Accepted);
Check("FCR0010 grants no resource authority", !appDecision.ResourceAuthorityGranted && !appDecision.LoadSheddingExecutionAuthorized && !appDecision.RuntimeActivationAuthorized && !appDecision.BusinessAuthorityGranted);
Check("FCR0010 stale projection fails closed", !RM.FoundationResourceProjectionBinding.Evaluate(appProjection with { ObservedAt = now.AddHours(-1) }, now, TimeSpan.FromMinutes(5)).Accepted);
Check("FCR0010 authority smuggling rejected", !RM.FoundationResourceProjectionBinding.Evaluate(appProjection with { RuntimeActivationAuthorized = true }, now, TimeSpan.FromMinutes(5)).Accepted);
var aggregate = appProjection with { Kind = RM.FoundationResourceProjectionKind.AggregateResourceState, ArtifactId = RM.FoundationResourceProjectionBinding.AggregateArtifactId, SourceContract = RM.FoundationResourceProjectionBinding.AggregateSourceContract };
Check("FCR0031 aggregate descriptor accepted", RM.FoundationResourceProjectionBinding.Evaluate(aggregate, now, TimeSpan.FromMinutes(5)).Accepted);
Check("FCR0031 wrong source rejected", !RM.FoundationResourceProjectionBinding.Evaluate(aggregate with { SourceContract = "wrong" }, now, TimeSpan.FromMinutes(5)).Accepted);

var fsa = new TA.FsaPeerBindingRequest(TA.FoundationFsaPeerBinding.FoundationCandidate, TA.FoundationFsaPeerBinding.DestinationFsaId, "trading", "MSA-TRADING-01", "candidate:1", hash, "evidence:1", hash, false, false, false, false, false);
var fsaDecision = TA.FoundationFsaPeerBinding.Evaluate(fsa);
Check("FCR0012/0030 Trading FSA peer binding accepted", fsaDecision.Accepted);
Check("FCR0012/0030 FSA review grants no adoption authority", !fsaDecision.FsaAcceptanceGranted && !fsaDecision.OwnerAdoptionGranted && !fsaDecision.RuntimeAuthorityGranted && !fsaDecision.BusinessAuthorityGranted);
Check("FCR0030 wrong FSA destination rejected", !TA.FoundationFsaPeerBinding.Evaluate(fsa with { DestinationFsaId = "fsa:wrong" }).Accepted);
Check("FCR0012 owner silence rejected", !TA.FoundationFsaPeerBinding.Evaluate(fsa with { OwnerSilenceTreatedAsApproval = true }).Accepted);
Check("FCR0012 business judgment rejected", !TA.FoundationFsaPeerBinding.Evaluate(fsa with { ContainsBusinessJudgmentAsFsaDecision = true }).Accepted);
Check("FCR0030 changed candidate digest rejected", !TA.FoundationFsaPeerBinding.Evaluate(fsa with { CandidateSha256 = "BAD" }).Accepted);

Check("FCR0030 FSAPMA FSA peer binding accepted", FA.FoundationFsaPeerBinding.Evaluate(new(FA.FoundationFsaPeerBinding.FoundationCandidate, FA.FoundationFsaPeerBinding.DestinationFsaId, "fsapma", "MSA-FSAPMA-01", "candidate:2", hash, "evidence:2", hash, false, false, false, false, false)).Accepted);
Check("FCR0030 Guardian FSA peer binding accepted", GA.FoundationFsaPeerBinding.Evaluate(new(GA.FoundationFsaPeerBinding.FoundationCandidate, GA.FoundationFsaPeerBinding.DestinationFsaId, "guardian", "MSA-GUARDIAN-01", "candidate:3", hash, "evidence:3", hash, false, false, false, false, false)).Accepted);
Check("FCR0030 FSTSimA FSA peer binding accepted", SA.FoundationFsaPeerBinding.Evaluate(new(SA.FoundationFsaPeerBinding.FoundationCandidate, SA.FoundationFsaPeerBinding.DestinationFsaId, "fstsim", "MSA-FSTSimA-01", "candidate:4", hash, "evidence:4", hash, false, false, false, false, false)).Accepted);
Check("FCR0030 APP-RSC FSA peer binding accepted", RA.FoundationFsaPeerBinding.Evaluate(new(RA.FoundationFsaPeerBinding.FoundationCandidate, RA.FoundationFsaPeerBinding.DestinationFsaId, "app-rsc", "MSA-RSC-01", "candidate:5", hash, "evidence:5", hash, false, false, false, false, false)).Accepted);

var research = new TA.ResearchEgressBinding(TA.FoundationResearchEgressGovernance.Stage12Candidate, "MSA-TRADING-01", "MSA", "RESEARCH_ONLY", "research.example", "route:research", "authority:research", "evidence:research", true, false, false, false, false, false);
Check("FCR0008 Trading research binding accepted", TA.FoundationResearchEgressGovernance.Evaluate(research).Accepted);
Check("FCR0008 operational purpose escalation rejected", !TA.FoundationResearchEgressGovernance.Evaluate(research with { OperationalDataPurpose = true }).Accepted);
Check("FCR0008 FSA direct internet rejected", !TA.FoundationResearchEgressGovernance.Evaluate(research with { FsaDirectInternet = true }).Accepted);
Check("FCR0008 connection execution rejected", !TA.FoundationResearchEgressGovernance.Evaluate(research with { ConnectionExecuted = true }).Accepted);
Check("FCR0008 FSAPMA research binding accepted", FA.FoundationResearchEgressGovernance.Evaluate(new(FA.FoundationResearchEgressGovernance.Stage12Candidate, "MSA-FSAPMA-01", "MSA", "RESEARCH_ONLY", "research.example", "route:research", "authority:research", "evidence:research", true, false, false, false, false, false)).Accepted);
Check("FCR0008 Guardian research binding accepted", GA.FoundationResearchEgressGovernance.Evaluate(new(GA.FoundationResearchEgressGovernance.Stage12Candidate, "MSA-GUARDIAN-01", "MSA", "RESEARCH_ONLY", "research.example", "route:research", "authority:research", "evidence:research", true, false, false, false, false, false)).Accepted);
Check("FCR0008 FSTSimA research binding accepted", SA.FoundationResearchEgressGovernance.Evaluate(new(SA.FoundationResearchEgressGovernance.Stage12Candidate, "MSA-FSTSimA-01", "MSA", "RESEARCH_ONLY", "research.example", "route:research", "authority:research", "evidence:research", true, false, false, false, false, false)).Accepted);
Check("FCR0008 APP-RSC research binding accepted", RA.FoundationResearchEgressGovernance.Evaluate(new(RA.FoundationResearchEgressGovernance.Stage12Candidate, "MSA-RSC-01", "MSA", "RESEARCH_ONLY", "research.example", "route:research", "authority:research", "evidence:research", true, false, false, false, false, false)).Accepted);

var provider = new FP.FoundationProviderEgressBinding(FP.FoundationProviderEgressGovernance.Stage12Candidate, "FSAPMA", "provider", "account", "operational-data", "PAPER", FP.FoundationProviderEgressGovernance.OperationalPurpose, "provider.example", "credential:ref", "route:policy", "authority:evidence", "quota:evidence", true, false, false);
Check("FCR0013 provider binding accepted", FP.FoundationProviderEgressGovernance.Evaluate(provider).Accepted);
Check("FCR0013 purpose confusion rejected", !FP.FoundationProviderEgressGovernance.Evaluate(provider with { Purpose = "RESEARCH_ONLY" }).Accepted);
Check("FCR0013 connection execution rejected", !FP.FoundationProviderEgressGovernance.Evaluate(provider with { ConnectionExecuted = true }).Accepted);
Check("FCR0013 missing provider account rejected", !FP.FoundationProviderEgressGovernance.Evaluate(provider with { ProviderAccountId = "" }).Accepted);

var nonLive = new FS.FoundationNonLiveEgressBinding(FS.FoundationNonLiveEgressGovernance.Stage12Candidate, "PAPER", "SIMULATION", "paper.example", "credential:paper", "route:paper", "authority:paper", true, false, false, false, false);
Check("FCR0011 non-Live binding accepted", FS.FoundationNonLiveEgressGovernance.Evaluate(nonLive).Accepted);
Check("FCR0011 Live route rejected", !FS.FoundationNonLiveEgressGovernance.Evaluate(nonLive with { LiveRoute = true }).Accepted);
Check("FCR0011 Live credential rejected", !FS.FoundationNonLiveEgressGovernance.Evaluate(nonLive with { LiveCredential = true }).Accepted);

var broker = new TR.FoundationBrokerEgressBinding(TR.FoundationTradingBindings.Stage12Candidate, "broker", "account", "PAPER", TR.FoundationTradingBindings.BrokerExecutionPurpose, "broker.example", "credential:broker", "authority:broker", "route:broker", true, false, false, false);
Check("FCR0014 broker route binding accepted", TR.FoundationTradingBindings.EvaluateBroker(broker).Accepted);
Check("FCR0014 purpose confusion rejected", !TR.FoundationTradingBindings.EvaluateBroker(broker with { Purpose = "OPERATIONAL_PROVIDER_DATA" }).Accepted);
Check("FCR0014 order authority smuggling rejected", !TR.FoundationTradingBindings.EvaluateBroker(broker with { OrderAuthorityGranted = true }).Accepted);
Check("FCR0014 Live authority smuggling rejected", !TR.FoundationTradingBindings.EvaluateBroker(broker with { LiveAuthorityGranted = true }).Accepted);

var tq = new TR.FoundationQosBinding(TR.FoundationTradingBindings.Stage11Candidate, "deadline:1", "normal", "evidence:qos", now, true, false, false);
Check("FCR0009 Trading QoS binding accepted", TR.FoundationTradingBindings.EvaluateQos(tq).Accepted);
Check("FCR0009 Trading stale Stage11 candidate rejected", !TR.FoundationTradingBindings.EvaluateQos(tq with { FoundationCandidate = "wrong" }).Accepted);
Check("FCR0009 QoS cannot mint business authority", !TR.FoundationTradingBindings.EvaluateQos(tq with { BusinessAuthorityGranted = true }).Accepted);
var gq = new GQ.FoundationQosBinding(GQ.FoundationQosGovernance.Stage11Candidate, "deadline:guardian", "critical-governed", "evidence:qos", now, false, false, false);
Check("FCR0009 Guardian QoS binding accepted", GQ.FoundationQosGovernance.Evaluate(gq).Accepted);
Check("FCR0009 Guardian stale Stage11 candidate rejected", !GQ.FoundationQosGovernance.Evaluate(gq with { FoundationCandidate = "wrong" }).Accepted);
var fq = new FP.FoundationQosBinding(FP.FoundationQosGovernance.Stage11Candidate, "deadline:fsapma", "normal", "evidence:qos", now, false, false, false);
Check("FCR0009 FSAPMA QoS binding accepted", FP.FoundationQosGovernance.Evaluate(fq).Accepted);
Check("FCR0009 FSAPMA stale Stage11 candidate rejected", !FP.FoundationQosGovernance.Evaluate(fq with { FoundationCandidate = "wrong" }).Accepted);

var recovery = new TR.FoundationRecoveryProjectionBindingInput(
    TR.FoundationRecoveryProjectionBinding.FoundationCandidate,
    TR.FoundationRecoveryProjectionBinding.RouteIdentity,
    TR.FoundationRecoveryProjectionBinding.MessageType,
    TR.FoundationRecoveryProjectionBinding.SchemaIdentity,
    TR.FoundationRecoveryProjectionBinding.ContractVersion,
    TR.FoundationRecoveryProjectionBinding.Producer,
    TR.FoundationRecoveryProjectionBinding.Recipient,
    TR.FoundationRecoveryProjectionBinding.MessageKind,
    TR.FoundationRecoveryProjectionBinding.Classification,
    TR.FoundationRecoveryProjectionBinding.TransportAuthority,
    TR.FoundationRecoveryProjectionBinding.ArtifactId,
    TR.FoundationRecoveryProjectionBinding.ContractVersion,
    TR.FoundationRecoveryProjectionBinding.ArtifactSha256,
    TR.FoundationRecoveryProjectionBinding.EvidenceReference,
    TR.FoundationRecoveryProjectionBinding.Provenance,
    TR.FoundationRecoveryProjectionBinding.CompatibilityIdentity,
    TR.FoundationRecoveryProjectionBinding.ArtifactState,
    TR.FoundationRecoveryProjectionBinding.SourceContract,
    "pending",
    "recovery-case:1",
    "ReadyForReleaseDecision",
    "Completed",
    true,
    TR.FoundationReleaseAuthorizationState.NotAuthorized,
    TR.FoundationReleaseExecutionState.NotExecuted,
    TR.FoundationReintroductionState.NotStarted,
    "restricted",
    "evidence:recovery:1",
    now.AddMinutes(-1),
    now.AddMinutes(4),
    true,
    TR.FoundationRecoveryFreshness.Current,
    true,
    false,
    false,
    false,
    false,
    false,
    false);
recovery = recovery with { ProjectionIdentity = TR.FoundationRecoveryProjectionBinding.ComputeProjectionIdentity(recovery) };
var recoveryDecision = TR.FoundationRecoveryProjectionBinding.Evaluate(recovery, now);
Check("FCR0082 canonical Stage9 recovery projection accepted", recoveryDecision.Accepted);
Check("FCR0082 technical consumption grants no runtime/business authority", !recoveryDecision.RuntimeActivationAuthorized && !recoveryDecision.LiveRouteActivated && !recoveryDecision.DeploymentAuthorized && !recoveryDecision.BusinessAuthorityGranted);
Check("FCR0082 readiness remains observation not release", recoveryDecision.ReadyForApplicationRecoveryDecision && !recoveryDecision.ReleaseAuthorizationObserved && !recoveryDecision.ReleaseExecutionObserved);
Check("FCR0082 wrong Foundation candidate rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { FoundationCandidate = "wrong" }, now).Accepted);
Check("FCR0082 wrong route rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { RouteIdentity = "route:wrong" }, now).Accepted);
Check("FCR0082 wrong recipient rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { Recipient = "other" }, now).Accepted);
Check("FCR0082 artifact digest mutation rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { ArtifactSha256 = "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" }, now).Accepted);
Check("FCR0082 evidence mutation rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { EvidenceReference = "evidence:wrong" }, now).Accepted);
Check("FCR0082 provenance mutation rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { Provenance = "commit/wrong" }, now).Accepted);
Check("FCR0082 source contract mutation rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { SourceContract = "wrong" }, now).Accepted);
Check("FCR0082 stale projection rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { Freshness = TR.FoundationRecoveryFreshness.Stale }, now).Accepted);
Check("FCR0082 expired projection rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { ValidUntil = now.AddSeconds(-1) }, now).Accepted);
Check("FCR0082 future observation rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { ObservedAt = now.AddMinutes(1), ValidUntil = now.AddMinutes(2) }, now).Accepted);
Check("FCR0082 readiness mismatch rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { ReadyForReleaseDecision = false }, now).Accepted);
Check("FCR0082 release authorization mismatch rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { ReleaseAuthorization = TR.FoundationReleaseAuthorizationState.Authorized }, now).Accepted);
Check("FCR0082 release execution mismatch rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { ReleaseExecution = TR.FoundationReleaseExecutionState.Executed }, now).Accepted);
Check("FCR0082 reintroduction mismatch rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { Reintroduction = TR.FoundationReintroductionState.Pending }, now).Accepted);
Check("FCR0082 projection identity mutation rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { ProjectionIdentity = "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" }, now).Accepted);
Check("FCR0082 business authority smuggling rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { CarriesBusinessAuthority = true }, now).Accepted);
Check("FCR0082 release execution authority smuggling rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { CarriesReleaseExecutionAuthority = true }, now).Accepted);
Check("FCR0082 lifecycle authority smuggling rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { CarriesLifecycleAuthority = true }, now).Accepted);
Check("FCR0082 runtime activation smuggling rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { RuntimeActivationAuthorized = true }, now).Accepted);
Check("FCR0082 live route activation smuggling rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { LiveRouteActivated = true }, now).Accepted);
Check("FCR0082 deployment authority smuggling rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { DeploymentAuthorized = true }, now).Accepted);
Check("FCR0082 unknown recovery state rejected", !TR.FoundationRecoveryProjectionBinding.Evaluate(recovery with { RecoveryState = "Unknown" }, now).Accepted);

var failures = checks.Where(x => !x.Pass).ToArray();
if (failures.Length > 0)
{
    foreach (var failure in failures) Console.Error.WriteLine($"FAIL: {failure.Name}");
    Console.Error.WriteLine($"FSATS FOUNDATION BINDING VERIFIER: FAIL ({checks.Count - failures.Length}/{checks.Count})");
    return 1;
}
Console.WriteLine($"FSATS FOUNDATION BINDING VERIFIER: PASS ({checks.Count}/{checks.Count})");
return 0;
