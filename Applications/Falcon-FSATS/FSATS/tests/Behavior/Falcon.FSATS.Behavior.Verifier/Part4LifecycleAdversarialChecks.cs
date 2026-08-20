using Falcon.FSATS.FSAPMA.Application;
using Falcon.FSATS.FSTSimA.Application;
using Falcon.FSATS.ResourceManagement.Application;
using Falcon.FSATS.Trading.Application;
using Falcon.FSATS.TradingGuardian.Application;

internal static class Part4LifecycleAdversarialChecks
{
    internal static void Run()
    {
        TradingChecks();
        ProviderChecks();
        GuardianChecks();
        ResourceChecks();
        SimulationChecks();
        Console.WriteLine("Part 4 Lifecycle Adversarial Verification: PASS");
    }

    private static void TradingChecks()
    {
        var safe = new TradingLifecycleSafetyState("ALPACA", "PA-001", "PAPER", false, false, false, false, false, false, false, false, true);
        var update = TradingTransition(TradingLifecycleTransitionKind.Update, TradingLifecycleCompatibility.CompatibleAsIs);
        var accepted = TradingLifecycleEvolution.Assess(update, safe);
        Require(accepted.Accepted && accepted.Readiness == TradingLifecycleReadiness.ReadyForExternalLifecycleReview && !accepted.GrantsRuntimeAuthority, "P4_TRADING_SAFE_UPDATE_NOT_READY");
        Require(!TradingLifecycleEvolution.Assess(update with { SourceTrustEpoch = 8, CurrentTrustEpoch = 9 }, safe).Accepted, "P4_TRADING_STALE_EPOCH_ACCEPTED");
        Require(!TradingLifecycleEvolution.Assess(update with { Compatibility = TradingLifecycleCompatibility.Unknown }, safe).Accepted, "P4_TRADING_UNKNOWN_COMPATIBILITY_ACCEPTED");

        var migration = update with { Compatibility = TradingLifecycleCompatibility.MigrationRequired };
        Require(TradingLifecycleEvolution.Assess(migration, safe).Readiness == TradingLifecycleReadiness.MigrationRequired, "P4_TRADING_MIGRATION_REQUIREMENT_SKIPPED");
        Require(TradingLifecycleEvolution.Assess(migration with { MigrationEvidenceValidated = true }, safe).Readiness == TradingLifecycleReadiness.ReadyForExternalLifecycleReview, "P4_TRADING_VALIDATED_MIGRATION_NOT_READY");

        var replacement = TradingTransition(TradingLifecycleTransitionKind.Replacement, TradingLifecycleCompatibility.CompatibleAsIs) with { TargetPackageId = "TRADING-PKG-3" };
        Require(!TradingLifecycleEvolution.Assess(replacement, safe).Accepted, "P4_TRADING_REPLACEMENT_IDENTITY_COLLISION_ACCEPTED");

        var contained = safe with { IsContained = true, HasCancellationTombstones = true };
        var rollback = TradingTransition(TradingLifecycleTransitionKind.Rollback, TradingLifecycleCompatibility.CompatibleAsIs);
        var rollbackResult = TradingLifecycleEvolution.Assess(rollback, contained);
        Require(!rollbackResult.Accepted && rollbackResult.Readiness == TradingLifecycleReadiness.RollbackBlocked && rollbackResult.PreservesCurrentSafetyFences, "P4_TRADING_ROLLBACK_RESURRECTED_CONTAINMENT");

        var ambiguous = safe with { HasDispatchStartedWork = true, HasUnresolvedBrokerReconciliation = true };
        Require(!TradingLifecycleEvolution.Assess(update, ambiguous).Accepted, "P4_TRADING_UNRESOLVED_BROKER_OUTCOME_BYPASSED");

        var remove = TradingTransition(TradingLifecycleTransitionKind.Removal, TradingLifecycleCompatibility.CompatibleAsIs) with { TargetVersion = string.Empty, TargetPackageId = string.Empty, TargetSchema = string.Empty };
        Require(!TradingLifecycleEvolution.Assess(remove, safe with { HasOpenExposure = true }).Accepted, "P4_TRADING_OPEN_EXPOSURE_REMOVAL_ALLOWED");
        Require(TradingLifecycleEvolution.Assess(remove, safe).Readiness == TradingLifecycleReadiness.RemovalReady, "P4_TRADING_SAFE_REMOVAL_NOT_READY");
        Require(!TradingLifecycleEvolution.Assess(update, safe with { HasStaleExecutionPermit = true }).Accepted, "P4_TRADING_STALE_PERMIT_REUSED");
        Require(!TradingLifecycleEvolution.Assess(update, safe with { RequiredEvidenceRetained = false }).Accepted, "P4_TRADING_EVIDENCE_ERASURE_ACCEPTED");
    }

    private static void ProviderChecks()
    {
        var safe = new ProviderLifecycleSafetyState("ALPACA", "DATA-01", "MARKET_DATA", "PAPER", false, false, false, false, true, false, true);
        var update = ProviderTransition(ProviderLifecycleTransitionKind.Update, ProviderLifecycleCompatibility.CompatibleAsIs);
        var accepted = ProviderLifecycleEvolution.Assess(update, safe);
        Require(accepted.Accepted && accepted.PreservesProviderTruth && !accepted.GrantsRuntimeAuthority, "P4_PROVIDER_SAFE_UPDATE_NOT_READY");
        Require(!ProviderLifecycleEvolution.Assess(update, safe with { ContainsSecretBytes = true }).Accepted, "P4_PROVIDER_SECRET_BYTES_MIGRATED");
        Require(!ProviderLifecycleEvolution.Assess(update with { SourceTrustEpoch = 1, CurrentTrustEpoch = 2 }, safe).Accepted, "P4_PROVIDER_STALE_EPOCH_ACCEPTED");
        Require(!ProviderLifecycleEvolution.Assess(update with { Compatibility = ProviderLifecycleCompatibility.Unknown }, safe).Accepted, "P4_PROVIDER_UNKNOWN_COMPATIBILITY_ACCEPTED");
        var migration = update with { Compatibility = ProviderLifecycleCompatibility.MigrationRequired };
        Require(ProviderLifecycleEvolution.Assess(migration, safe).Readiness == ProviderLifecycleReadiness.MigrationRequired, "P4_PROVIDER_MIGRATION_REQUIREMENT_SKIPPED");
        Require(ProviderLifecycleEvolution.Assess(migration with { MigrationEvidenceValidated = true }, safe).Readiness == ProviderLifecycleReadiness.ReadyForExternalLifecycleReview, "P4_PROVIDER_VALIDATED_MIGRATION_NOT_READY");
        var replacement = ProviderTransition(ProviderLifecycleTransitionKind.Replacement, ProviderLifecycleCompatibility.CompatibleAsIs) with { TargetPackageId = "FSAPMA-PKG-3" };
        Require(!ProviderLifecycleEvolution.Assess(replacement, safe).Accepted, "P4_PROVIDER_REPLACEMENT_IDENTITY_COLLISION_ACCEPTED");

        var stale = safe with { StreamGapDetected = true, StreamStale = true };
        var rollback = ProviderTransition(ProviderLifecycleTransitionKind.Rollback, ProviderLifecycleCompatibility.CompatibleAsIs);
        Require(!ProviderLifecycleEvolution.Assess(rollback, stale).Accepted, "P4_PROVIDER_GAP_LAUNDERED_BY_ROLLBACK");

        var remove = ProviderTransition(ProviderLifecycleTransitionKind.Removal, ProviderLifecycleCompatibility.CompatibleAsIs) with { TargetVersion = string.Empty, TargetPackageId = string.Empty, TargetSchema = string.Empty };
        Require(!ProviderLifecycleEvolution.Assess(remove, safe).Accepted, "P4_PROVIDER_ACTIVE_CREDENTIAL_REFERENCE_REMOVAL_ALLOWED");
        Require(ProviderLifecycleEvolution.Assess(remove, safe with { HasCurrentCredentialReference = false }).Readiness == ProviderLifecycleReadiness.RemovalReady, "P4_PROVIDER_SAFE_REMOVAL_NOT_READY");
    }

    private static void GuardianChecks()
    {
        var safe = new GuardianLifecycleSafetyState("ACCOUNT:ALPACA:PA-001:PAPER", "INC-01", "CORR-01", "IDEMP-01", false, false, false, false, true);
        var update = GuardianTransition(GuardianLifecycleTransitionKind.Update, GuardianLifecycleCompatibility.CompatibleAsIs);
        Require(GuardianLifecycleEvolution.Assess(update, safe).Accepted, "P4_GUARDIAN_SAFE_UPDATE_NOT_READY");
        Require(!GuardianLifecycleEvolution.Assess(update, safe with { HasStaleProtectionAuthority = true }).Accepted, "P4_GUARDIAN_STALE_AUTHORITY_ACCEPTED");
        Require(!GuardianLifecycleEvolution.Assess(update with { Compatibility = GuardianLifecycleCompatibility.Unknown }, safe).Accepted, "P4_GUARDIAN_UNKNOWN_COMPATIBILITY_ACCEPTED");
        var migration = update with { Compatibility = GuardianLifecycleCompatibility.MigrationRequired };
        Require(GuardianLifecycleEvolution.Assess(migration, safe).Readiness == GuardianLifecycleReadiness.MigrationRequired, "P4_GUARDIAN_MIGRATION_REQUIREMENT_SKIPPED");
        Require(GuardianLifecycleEvolution.Assess(migration with { MigrationEvidenceValidated = true }, safe).Readiness == GuardianLifecycleReadiness.ReadyForExternalLifecycleReview, "P4_GUARDIAN_VALIDATED_MIGRATION_NOT_READY");
        var replacement = GuardianTransition(GuardianLifecycleTransitionKind.Replacement, GuardianLifecycleCompatibility.CompatibleAsIs) with { TargetPackageId = "GUARDIAN-PKG-3" };
        Require(!GuardianLifecycleEvolution.Assess(replacement, safe).Accepted, "P4_GUARDIAN_REPLACEMENT_IDENTITY_COLLISION_ACCEPTED");

        var historicalAmbiguity = safe with { HasUnresolvedProtectionOutcome = true, RequiresCurrentProtectionTruthVerification = true };
        var rollback = GuardianTransition(GuardianLifecycleTransitionKind.Rollback, GuardianLifecycleCompatibility.CompatibleAsIs);
        Require(!GuardianLifecycleEvolution.Assess(rollback, historicalAmbiguity).Accepted, "P4_GUARDIAN_AMBIGUOUS_PROTECTION_LAUNDERED_BY_ROLLBACK");

        var remove = GuardianTransition(GuardianLifecycleTransitionKind.Removal, GuardianLifecycleCompatibility.CompatibleAsIs) with { TargetVersion = string.Empty, TargetPackageId = string.Empty, TargetSchema = string.Empty };
        Require(!GuardianLifecycleEvolution.Assess(remove, safe with { HasActiveContainmentOrRestriction = true }).Accepted, "P4_GUARDIAN_ACTIVE_PROTECTION_REMOVAL_ALLOWED");
        Require(GuardianLifecycleEvolution.Assess(remove, safe).Readiness == GuardianLifecycleReadiness.RemovalReady, "P4_GUARDIAN_SAFE_REMOVAL_NOT_READY");
    }

    private static void ResourceChecks()
    {
        var safe = new ResourceLifecycleSafetyState(11, 11, "FOUNDATION-ENVELOPE-REF-01", false, false, false, true);
        var update = ResourceTransition(ResourceLifecycleTransitionKind.Update, ResourceLifecycleCompatibility.CompatibleAsIs);
        var accepted = ResourceLifecycleEvolution.Assess(update, safe);
        Require(accepted.Accepted && accepted.PreservesFoundationAuthorityBoundary && !accepted.GrantsRuntimeAuthority, "P4_APP_RSC_SAFE_UPDATE_NOT_READY");
        Require(!ResourceLifecycleEvolution.Assess(update, safe with { SourceCoordinatorEpoch = 10 }).Accepted, "P4_APP_RSC_STALE_COORDINATOR_EPOCH_ACCEPTED");
        Require(!ResourceLifecycleEvolution.Assess(update, safe with { ReferenceClaimsFoundationGrant = true }).Accepted, "P4_APP_RSC_MINTED_FOUNDATION_GRANT");
        Require(!ResourceLifecycleEvolution.Assess(update with { Compatibility = ResourceLifecycleCompatibility.Unknown }, safe).Accepted, "P4_APP_RSC_UNKNOWN_COMPATIBILITY_ACCEPTED");
        var migration = update with { Compatibility = ResourceLifecycleCompatibility.MigrationRequired };
        Require(ResourceLifecycleEvolution.Assess(migration, safe).Readiness == ResourceLifecycleReadiness.MigrationRequired, "P4_APP_RSC_MIGRATION_REQUIREMENT_SKIPPED");
        Require(ResourceLifecycleEvolution.Assess(migration with { MigrationEvidenceValidated = true }, safe).Readiness == ResourceLifecycleReadiness.ReadyForExternalLifecycleReview, "P4_APP_RSC_VALIDATED_MIGRATION_NOT_READY");
        var replacement = ResourceTransition(ResourceLifecycleTransitionKind.Replacement, ResourceLifecycleCompatibility.CompatibleAsIs) with { TargetPackageId = "RSC-PKG-3" };
        Require(!ResourceLifecycleEvolution.Assess(replacement, safe).Accepted, "P4_APP_RSC_REPLACEMENT_IDENTITY_COLLISION_ACCEPTED");

        var rollback = ResourceTransition(ResourceLifecycleTransitionKind.Rollback, ResourceLifecycleCompatibility.CompatibleAsIs);
        Require(!ResourceLifecycleEvolution.Assess(rollback, safe with { HasPendingResourceOutcome = true }).Accepted, "P4_APP_RSC_PENDING_OUTCOME_BYPASSED_BY_ROLLBACK");

        var remove = ResourceTransition(ResourceLifecycleTransitionKind.Removal, ResourceLifecycleCompatibility.CompatibleAsIs) with { TargetVersion = string.Empty, TargetPackageId = string.Empty, TargetSchema = string.Empty };
        Require(!ResourceLifecycleEvolution.Assess(remove, safe with { HasUnresolvedFoundationOutcome = true }).Accepted, "P4_APP_RSC_UNRESOLVED_OUTCOME_REMOVAL_ALLOWED");
        Require(ResourceLifecycleEvolution.Assess(remove, safe).Readiness == ResourceLifecycleReadiness.RemovalReady, "P4_APP_RSC_SAFE_REMOVAL_NOT_READY");
    }

    private static void SimulationChecks()
    {
        var safe = new SimulationLifecycleSafetyState("SIM-01", "EVID-01", true, false, false, false, false, false, true);
        var update = SimulationTransition(SimulationLifecycleTransitionKind.Update, SimulationLifecycleCompatibility.CompatibleAsIs);
        var accepted = SimulationLifecycleEvolution.Assess(update, safe);
        Require(accepted.Accepted && accepted.PreservesEvidenceClassification && !accepted.GrantsRuntimeAuthority, "P4_FSTSIMA_SAFE_UPDATE_NOT_READY");
        Require(!SimulationLifecycleEvolution.Assess(update with { Compatibility = SimulationLifecycleCompatibility.Unknown }, safe).Accepted, "P4_FSTSIMA_UNKNOWN_COMPATIBILITY_ACCEPTED");
        Require(!SimulationLifecycleEvolution.Assess(update, safe with { RunCommitted = false, RunInterrupted = true, QualificationClaimed = true }).Accepted, "P4_FSTSIMA_PARTIAL_RUN_QUALIFIED");
        Require(!SimulationLifecycleEvolution.Assess(update, safe with { EvidenceIsReplayOrSynthetic = true, QualificationClaimed = true }).Accepted, "P4_FSTSIMA_REPLAY_EVIDENCE_UPGRADED");
        var migration = update with { Compatibility = SimulationLifecycleCompatibility.MigrationRequired };
        Require(SimulationLifecycleEvolution.Assess(migration, safe).Readiness == SimulationLifecycleReadiness.MigrationRequired, "P4_FSTSIMA_MIGRATION_REQUIREMENT_SKIPPED");
        Require(SimulationLifecycleEvolution.Assess(migration with { MigrationEvidenceValidated = true }, safe).Readiness == SimulationLifecycleReadiness.ReadyForExternalLifecycleReview, "P4_FSTSIMA_VALIDATED_MIGRATION_NOT_READY");
        var replacement = SimulationTransition(SimulationLifecycleTransitionKind.Replacement, SimulationLifecycleCompatibility.CompatibleAsIs) with { TargetPackageId = "SIM-PKG-3" };
        Require(!SimulationLifecycleEvolution.Assess(replacement, safe).Accepted, "P4_FSTSIMA_REPLACEMENT_IDENTITY_COLLISION_ACCEPTED");

        var rollback = SimulationTransition(SimulationLifecycleTransitionKind.Rollback, SimulationLifecycleCompatibility.CompatibleAsIs);
        Require(!SimulationLifecycleEvolution.Assess(rollback, safe with { ContainsPartialCheckpoint = true }).Accepted, "P4_FSTSIMA_PARTIAL_CHECKPOINT_ROLLBACK_ALLOWED");

        var remove = SimulationTransition(SimulationLifecycleTransitionKind.Removal, SimulationLifecycleCompatibility.CompatibleAsIs) with { TargetVersion = string.Empty, TargetPackageId = string.Empty, TargetSchema = string.Empty };
        Require(!SimulationLifecycleEvolution.Assess(remove, safe with { HasPendingValidation = true }).Accepted, "P4_FSTSIMA_PENDING_VALIDATION_REMOVAL_ALLOWED");
        Require(SimulationLifecycleEvolution.Assess(remove, safe).Readiness == SimulationLifecycleReadiness.RemovalReady, "P4_FSTSIMA_SAFE_REMOVAL_NOT_READY");
    }

    private static TradingLifecycleTransition TradingTransition(TradingLifecycleTransitionKind kind, TradingLifecycleCompatibility compatibility)
        => new("TR-01", kind, TradingLifecycleEvolution.ApplicationId, "3.0.0", "4.0.0", "TRADING-PKG-3", "TRADING-PKG-4", "TRADING-SCHEMA-3", "TRADING-SCHEMA-4", 9, 9, "EVID-TR-01", false, compatibility);
    private static ProviderLifecycleTransition ProviderTransition(ProviderLifecycleTransitionKind kind, ProviderLifecycleCompatibility compatibility)
        => new("PR-01", kind, ProviderLifecycleEvolution.ApplicationId, "3.0.0", "4.0.0", "FSAPMA-PKG-3", "FSAPMA-PKG-4", "FSAPMA-SCHEMA-3", "FSAPMA-SCHEMA-4", 9, 9, "EVID-PR-01", false, compatibility);
    private static GuardianLifecycleTransition GuardianTransition(GuardianLifecycleTransitionKind kind, GuardianLifecycleCompatibility compatibility)
        => new("GU-01", kind, GuardianLifecycleEvolution.ApplicationId, "3.0.0", "4.0.0", "GUARDIAN-PKG-3", "GUARDIAN-PKG-4", "GUARDIAN-SCHEMA-3", "GUARDIAN-SCHEMA-4", 9, 9, "EVID-GU-01", false, compatibility);
    private static ResourceLifecycleTransition ResourceTransition(ResourceLifecycleTransitionKind kind, ResourceLifecycleCompatibility compatibility)
        => new("RS-01", kind, ResourceLifecycleEvolution.ApplicationId, "3.0.0", "4.0.0", "RSC-PKG-3", "RSC-PKG-4", "RSC-SCHEMA-3", "RSC-SCHEMA-4", 9, 9, "EVID-RS-01", false, compatibility);
    private static SimulationLifecycleTransition SimulationTransition(SimulationLifecycleTransitionKind kind, SimulationLifecycleCompatibility compatibility)
        => new("SI-01", kind, SimulationLifecycleEvolution.ApplicationId, "3.0.0", "4.0.0", "SIM-PKG-3", "SIM-PKG-4", "SIM-SCHEMA-3", "SIM-SCHEMA-4", 9, 9, "EVID-SI-01", false, compatibility);

    private static void Require(bool condition, string failureCode)
    {
        if (!condition) throw new InvalidOperationException(failureCode);
    }
}
