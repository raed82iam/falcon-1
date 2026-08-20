using Falcon.FSATS.FSAPMA.Application;
using Falcon.FSATS.FSTSimA.Application;
using Falcon.FSATS.ResourceManagement.Application;
using Falcon.FSATS.Trading.Application;
using Falcon.FSATS.TradingGuardian.Application;

internal static class Part5HealthReadinessAdversarialChecks
{
    private static readonly DateTimeOffset Now = new(2026, 8, 15, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Observed = Now.AddMinutes(-1);
    private static readonly DateTimeOffset ValidUntil = Now.AddMinutes(4);

    internal static void Run()
    {
        TradingChecks();
        ProviderChecks();
        GuardianChecks();
        ResourceChecks();
        SimulationChecks();
        Console.WriteLine("Part 5 Health / Readiness Adversarial Verification: PASS");
    }

    private static void TradingChecks()
    {
        var safe = TradingSnapshot();
        var healthy = TradingOperationalHealth.Assess(safe, Now);
        Require(healthy.Accepted, "P5_TRADING_HEALTHY_REJECTED");
        Require(healthy.Condition == TradingOperationalHealthCondition.Healthy, "P5_TRADING_HEALTHY_NOT_HEALTHY");
        Require(healthy.CurrentEvidence, "P5_TRADING_HEALTHY_NOT_CURRENT");
        Require(healthy.RiskIncreaseEligibleByHealthOnly, "P5_TRADING_HEALTH_ONLY_ELIGIBILITY_MISSING");
        Require(!healthy.GrantsRuntimeAuthority, "P5_TRADING_HEALTH_GRANTED_RUNTIME_AUTHORITY");

        Require(!TradingOperationalHealth.Assess(null, Now).Accepted, "P5_TRADING_NULL_INPUT_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { ApplicationId = "FSATS-FSAPMA" }, Now).Accepted, "P5_TRADING_WRONG_APP_ID_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { BrokerAccountId = " PA-001" }, Now).Accepted, "P5_TRADING_MALFORMED_ACCOUNT_ID_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { EvidenceId = "" }, Now).Accepted, "P5_TRADING_MISSING_EVIDENCE_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { EvidenceIntegrity = TradingHealthEvidenceIntegrity.Invalid }, Now).Accepted, "P5_TRADING_BAD_EVIDENCE_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { EvidenceIntegrity = (TradingHealthEvidenceIntegrity)999 }, Now).Accepted, "P5_TRADING_INVALID_ENUM_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { ObservedAtUtc = Now.AddSeconds(1) }, Now).Accepted, "P5_TRADING_FUTURE_OBSERVATION_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { ValidUntilUtc = Observed.AddSeconds(-1) }, Now).Accepted, "P5_TRADING_INVALID_INTERVAL_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { ValidUntilUtc = Now.AddSeconds(-1) }, Now).Accepted, "P5_TRADING_EXPIRED_OBSERVATION_ACCEPTED");
        Require(!TradingOperationalHealth.Assess(safe with { HasStaleExecutionAuthority = true }, Now).Accepted, "P5_TRADING_STALE_AUTHORITY_ACCEPTED");

        var recon = TradingOperationalHealth.Assess(safe with { HasDispatchStartedWork = true }, Now);
        Require(recon.Condition == TradingOperationalHealthCondition.ReconciliationRequired && recon.RequiresReconciliation, "P5_TRADING_DISPATCH_AMBIGUITY_LAUNDERED");
        Require(!recon.RiskIncreaseEligibleByHealthOnly, "P5_TRADING_RECONCILIATION_ALLOWED_RISK_INCREASE");

        var contained = TradingOperationalHealth.Assess(safe with { IsContained = true }, Now);
        Require(contained.Condition == TradingOperationalHealthCondition.Contained, "P5_TRADING_CONTAINMENT_HIDDEN");
        Require(!contained.RiskIncreaseEligibleByHealthOnly, "P5_TRADING_CONTAINMENT_ALLOWED_RISK_INCREASE");

        var obligations = TradingOperationalHealth.Assess(safe with { HasOpenExposure = true, HasCapitalReservations = true }, Now);
        Require(obligations.Condition == TradingOperationalHealthCondition.DegradedSafe, "P5_TRADING_ACTIVE_OBLIGATIONS_NOT_DEGRADED");
        Require(!obligations.RiskIncreaseEligibleByHealthOnly, "P5_TRADING_ACTIVE_OBLIGATIONS_ALLOWED_RISK_INCREASE");

        Require(TradingOperationalHealth.Assess(safe with { RestartReconstructionComplete = false }, Now).Condition == TradingOperationalHealthCondition.NotReady, "P5_TRADING_INCOMPLETE_RESTART_READY");
        Require(TradingOperationalHealth.Assess(safe with { LifecycleTransitionBlocked = true }, Now).Condition == TradingOperationalHealthCondition.NotReady, "P5_TRADING_LIFECYCLE_BLOCKER_READY");
    }

    private static void ProviderChecks()
    {
        var safe = ProviderSnapshot();
        var healthy = ProviderOperationalHealth.Assess(safe, Now);
        Require(healthy.Accepted && healthy.Condition == ProviderOperationalHealthCondition.Healthy, "P5_PROVIDER_HEALTHY_NOT_HEALTHY");
        Require(healthy.OperationalDataEligibleByHealthOnly, "P5_PROVIDER_HEALTH_ONLY_ELIGIBILITY_MISSING");
        Require(!healthy.GrantsRuntimeAuthority, "P5_PROVIDER_HEALTH_GRANTED_RUNTIME_AUTHORITY");

        Require(!ProviderOperationalHealth.Assess(null, Now).Accepted, "P5_PROVIDER_NULL_INPUT_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { ApplicationId = "FSATS-TRADING" }, Now).Accepted, "P5_PROVIDER_WRONG_APP_ID_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { EvidenceId = "" }, Now).Accepted, "P5_PROVIDER_MISSING_EVIDENCE_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { EvidenceIntegrity = ProviderHealthEvidenceIntegrity.Invalid }, Now).Accepted, "P5_PROVIDER_BAD_EVIDENCE_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { EvidenceIntegrity = (ProviderHealthEvidenceIntegrity)999 }, Now).Accepted, "P5_PROVIDER_INVALID_ENUM_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { ContainsSecretBytes = true }, Now).Accepted, "P5_PROVIDER_SECRET_BYTES_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { HasStaleProviderAuthority = true }, Now).Accepted, "P5_PROVIDER_STALE_AUTHORITY_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { ObservedAtUtc = Now.AddSeconds(1) }, Now).Accepted, "P5_PROVIDER_FUTURE_OBSERVATION_ACCEPTED");
        Require(!ProviderOperationalHealth.Assess(safe with { ValidUntilUtc = Now.AddSeconds(-1) }, Now).Accepted, "P5_PROVIDER_EXPIRED_OBSERVATION_ACCEPTED");

        var gap = ProviderOperationalHealth.Assess(safe with { StreamGapDetected = true }, Now);
        Require(gap.Condition == ProviderOperationalHealthCondition.ReconciliationRequired && gap.RequiresReconciliation, "P5_PROVIDER_GAP_REPORTED_HEALTHY");
        Require(!gap.OperationalDataEligibleByHealthOnly, "P5_PROVIDER_GAP_ALLOWED_OPERATIONAL_DATA");

        var stale = ProviderOperationalHealth.Assess(safe with { StreamStale = true }, Now);
        Require(stale.Condition == ProviderOperationalHealthCondition.ReconciliationRequired, "P5_PROVIDER_STALE_STREAM_REPORTED_CURRENT");

        var unknownDelivery = ProviderOperationalHealth.Assess(safe with { DeliveryOutcomeUnknown = true }, Now);
        Require(unknownDelivery.Condition == ProviderOperationalHealthCondition.ReconciliationRequired, "P5_PROVIDER_UNKNOWN_DELIVERY_REPORTED_HEALTHY");

        var entitlementUnknown = ProviderOperationalHealth.Assess(safe with { QuotaEntitlementKnown = false }, Now);
        Require(entitlementUnknown.Condition == ProviderOperationalHealthCondition.NotReady, "P5_PROVIDER_UNKNOWN_ENTITLEMENT_REPORTED_READY");

        var pressure = ProviderOperationalHealth.Assess(safe with { QuotaPressureActive = true }, Now);
        Require(pressure.Condition == ProviderOperationalHealthCondition.DegradedSafe, "P5_PROVIDER_QUOTA_PRESSURE_NOT_DEGRADED");
        Require(!pressure.OperationalDataEligibleByHealthOnly, "P5_PROVIDER_QUOTA_PRESSURE_ALLOWED_NORMAL_DATA");
    }

    private static void GuardianChecks()
    {
        var safe = GuardianSnapshot();
        var healthy = GuardianOperationalHealth.Assess(safe, Now);
        Require(healthy.Accepted && healthy.Condition == GuardianOperationalHealthCondition.Healthy, "P5_GUARDIAN_HEALTHY_NOT_HEALTHY");
        Require(healthy.ProtectionTruthCurrentByHealthOnly, "P5_GUARDIAN_CURRENT_TRUTH_NOT_RECOGNIZED");
        Require(!healthy.GrantsRuntimeAuthority, "P5_GUARDIAN_HEALTH_GRANTED_RUNTIME_AUTHORITY");

        Require(!GuardianOperationalHealth.Assess(null, Now).Accepted, "P5_GUARDIAN_NULL_INPUT_ACCEPTED");
        Require(!GuardianOperationalHealth.Assess(safe with { ApplicationId = "FSATS-TRADING" }, Now).Accepted, "P5_GUARDIAN_WRONG_APP_ID_ACCEPTED");
        Require(!GuardianOperationalHealth.Assess(safe with { EvidenceIntegrity = GuardianHealthEvidenceIntegrity.Invalid }, Now).Accepted, "P5_GUARDIAN_BAD_EVIDENCE_ACCEPTED");
        Require(!GuardianOperationalHealth.Assess(safe with { EvidenceIntegrity = (GuardianHealthEvidenceIntegrity)999 }, Now).Accepted, "P5_GUARDIAN_INVALID_ENUM_ACCEPTED");
        Require(!GuardianOperationalHealth.Assess(safe with { HasStaleProtectionAuthority = true }, Now).Accepted, "P5_GUARDIAN_STALE_AUTHORITY_ACCEPTED");
        Require(!GuardianOperationalHealth.Assess(safe with { ValidUntilUtc = Now.AddSeconds(-1) }, Now).Accepted, "P5_GUARDIAN_EXPIRED_OBSERVATION_ACCEPTED");

        var historicalApplied = GuardianOperationalHealth.Assess(safe with { RequiresCurrentProtectionTruthVerification = true }, Now);
        Require(historicalApplied.Condition == GuardianOperationalHealthCondition.ReconciliationRequired, "P5_GUARDIAN_HISTORICAL_APPLIED_LAUNDERED_AS_CURRENT");
        Require(!historicalApplied.ProtectionTruthCurrentByHealthOnly, "P5_GUARDIAN_UNVERIFIED_TRUTH_MARKED_CURRENT");

        var unresolved = GuardianOperationalHealth.Assess(safe with { HasUnresolvedProtectionOutcome = true }, Now);
        Require(unresolved.Condition == GuardianOperationalHealthCondition.ReconciliationRequired, "P5_GUARDIAN_UNRESOLVED_OUTCOME_REPORTED_HEALTHY");

        var contained = GuardianOperationalHealth.Assess(safe with { HasActiveContainmentOrRestriction = true }, Now);
        Require(contained.Condition == GuardianOperationalHealthCondition.Contained, "P5_GUARDIAN_ACTIVE_CONTAINMENT_REPORTED_NORMAL");
    }

    private static void ResourceChecks()
    {
        var safe = ResourceSnapshot();
        var healthy = ResourceOperationalHealth.Assess(safe, Now);
        Require(healthy.Accepted && healthy.Condition == ResourceOperationalHealthCondition.Healthy, "P5_APP_RSC_HEALTHY_NOT_HEALTHY");
        Require(healthy.PreservesFoundationAuthorityBoundary, "P5_APP_RSC_FOUNDATION_BOUNDARY_NOT_PRESERVED");
        Require(!healthy.GrantsRuntimeAuthority, "P5_APP_RSC_HEALTH_GRANTED_RUNTIME_AUTHORITY");

        Require(!ResourceOperationalHealth.Assess(null, Now).Accepted, "P5_APP_RSC_NULL_INPUT_ACCEPTED");
        Require(!ResourceOperationalHealth.Assess(safe with { ApplicationId = "FSATS-TRADING" }, Now).Accepted, "P5_APP_RSC_WRONG_APP_ID_ACCEPTED");
        Require(!ResourceOperationalHealth.Assess(safe with { SourceCoordinatorEpoch = 10 }, Now).Accepted, "P5_APP_RSC_STALE_COORDINATOR_EPOCH_ACCEPTED");
        Require(!ResourceOperationalHealth.Assess(safe with { ReferenceClaimsFoundationGrant = true }, Now).Accepted, "P5_APP_RSC_FOUNDATION_GRANT_MINTING_ACCEPTED");
        Require(!ResourceOperationalHealth.Assess(safe with { EvidenceIntegrity = ResourceHealthEvidenceIntegrity.Invalid }, Now).Accepted, "P5_APP_RSC_BAD_EVIDENCE_ACCEPTED");
        Require(!ResourceOperationalHealth.Assess(safe with { EvidenceIntegrity = (ResourceHealthEvidenceIntegrity)999 }, Now).Accepted, "P5_APP_RSC_INVALID_ENUM_ACCEPTED");

        var pending = ResourceOperationalHealth.Assess(safe with { HasPendingResourceOutcome = true }, Now);
        Require(pending.Condition == ResourceOperationalHealthCondition.ReconciliationRequired, "P5_APP_RSC_PENDING_OUTCOME_REPORTED_HEALTHY");

        var unresolved = ResourceOperationalHealth.Assess(safe with { HasUnresolvedFoundationOutcome = true }, Now);
        Require(unresolved.Condition == ResourceOperationalHealthCondition.ReconciliationRequired, "P5_APP_RSC_UNRESOLVED_FOUNDATION_OUTCOME_REPORTED_HEALTHY");

        var pressureSafe = ResourceOperationalHealth.Assess(safe with { ResourcePressureActive = true }, Now);
        Require(pressureSafe.Condition == ResourceOperationalHealthCondition.DegradedSafe, "P5_APP_RSC_SAFE_PRESSURE_NOT_DEGRADED");

        var pressureUnsafe = ResourceOperationalHealth.Assess(safe with { ResourcePressureActive = true, MinimumSafetyFloorPreserved = false }, Now);
        Require(pressureUnsafe.Condition == ResourceOperationalHealthCondition.NotReady, "P5_APP_RSC_UNSAFE_PRESSURE_MARKED_DEGRADED_SAFE");
        Require(!pressureUnsafe.InternalCoordinationEligibleByHealthOnly, "P5_APP_RSC_UNSAFE_PRESSURE_ALLOWED_COORDINATION");
    }

    private static void SimulationChecks()
    {
        var safe = SimulationSnapshot();
        var healthy = SimulationOperationalHealth.Assess(safe, Now);
        Require(healthy.Accepted && healthy.Condition == SimulationOperationalHealthCondition.Healthy, "P5_FSTSIMA_HEALTHY_NOT_HEALTHY");
        Require(healthy.QualificationEvidenceUsableByHealthOnly, "P5_FSTSIMA_VALID_QUALIFICATION_NOT_USABLE");
        Require(!healthy.GrantsRuntimeAuthority, "P5_FSTSIMA_HEALTH_GRANTED_RUNTIME_AUTHORITY");

        Require(!SimulationOperationalHealth.Assess(null, Now).Accepted, "P5_FSTSIMA_NULL_INPUT_ACCEPTED");
        Require(!SimulationOperationalHealth.Assess(safe with { ApplicationId = "FSATS-TRADING" }, Now).Accepted, "P5_FSTSIMA_WRONG_APP_ID_ACCEPTED");
        Require(!SimulationOperationalHealth.Assess(safe with { EvidenceIntegrity = SimulationHealthEvidenceIntegrity.Invalid }, Now).Accepted, "P5_FSTSIMA_BAD_EVIDENCE_ACCEPTED");
        Require(!SimulationOperationalHealth.Assess(safe with { EvidenceIntegrity = (SimulationHealthEvidenceIntegrity)999 }, Now).Accepted, "P5_FSTSIMA_INVALID_ENUM_ACCEPTED");
        Require(!SimulationOperationalHealth.Assess(safe with { EvidenceIsReplayOrSynthetic = true }, Now).Accepted, "P5_FSTSIMA_SYNTHETIC_QUALIFICATION_ACCEPTED");
        Require(!SimulationOperationalHealth.Assess(safe with { RunCommitted = false }, Now).Accepted, "P5_FSTSIMA_INCOMPLETE_QUALIFICATION_ACCEPTED");

        var interrupted = SimulationOperationalHealth.Assess(safe with { QualificationClaimed = false, RunInterrupted = true }, Now);
        Require(interrupted.Condition == SimulationOperationalHealthCondition.ReconciliationRequired, "P5_FSTSIMA_INTERRUPTED_RUN_REPORTED_HEALTHY");

        var pending = SimulationOperationalHealth.Assess(safe with { QualificationClaimed = false, HasPendingValidation = true }, Now);
        Require(pending.Condition == SimulationOperationalHealthCondition.ReconciliationRequired, "P5_FSTSIMA_PENDING_VALIDATION_REPORTED_HEALTHY");

        var replay = SimulationOperationalHealth.Assess(safe with { QualificationClaimed = false, EvidenceIsReplayOrSynthetic = true }, Now);
        Require(replay.Condition == SimulationOperationalHealthCondition.DegradedSafe, "P5_FSTSIMA_REPLAY_CLASSIFICATION_LOST");
        Require(!replay.QualificationEvidenceUsableByHealthOnly, "P5_FSTSIMA_REPLAY_EVIDENCE_MARKED_QUALIFYING");
    }

    private static TradingOperationalHealthSnapshot TradingSnapshot() =>
        new(TradingOperationalHealth.ApplicationId, "ALPACA", "PA-001", "PAPER", Observed, ValidUntil, "EVID-P5-TR-01", TradingHealthEvidenceIntegrity.Valid,
            false, false, false, false, false, false, false, false, true, false);

    private static ProviderOperationalHealthSnapshot ProviderSnapshot() =>
        new(ProviderOperationalHealth.ApplicationId, "ALPACA", "DATA-01", "MARKET_DATA", "PAPER", Observed, ValidUntil, "EVID-P5-PR-01", ProviderHealthEvidenceIntegrity.Valid,
            false, false, false, true, false, false, false, true, false);

    private static GuardianOperationalHealthSnapshot GuardianSnapshot() =>
        new(GuardianOperationalHealth.ApplicationId, "ACCOUNT:ALPACA:PA-001:PAPER", "INC-01", "CORR-01", Observed, ValidUntil, "EVID-P5-GU-01", GuardianHealthEvidenceIntegrity.Valid,
            false, false, false, false, true, false);

    private static ResourceOperationalHealthSnapshot ResourceSnapshot() =>
        new(ResourceOperationalHealth.ApplicationId, 11, 11, "FOUNDATION-ENVELOPE-REF-01", Observed, ValidUntil, "EVID-P5-RS-01", ResourceHealthEvidenceIntegrity.Valid,
            false, false, false, false, true, true, false);

    private static SimulationOperationalHealthSnapshot SimulationSnapshot() =>
        new(SimulationOperationalHealth.ApplicationId, "SIM-01", "EVID-P5-SI-01", Observed, ValidUntil, SimulationHealthEvidenceIntegrity.Valid,
            true, false, false, false, false, true, true, false);

    private static void Require(bool condition, string failureCode)
    {
        if (!condition)
            throw new InvalidOperationException(failureCode);
    }
}
