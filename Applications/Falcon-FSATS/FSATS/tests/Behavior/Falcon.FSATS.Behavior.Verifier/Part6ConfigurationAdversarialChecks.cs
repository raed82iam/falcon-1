using Falcon.FSATS.FSAPMA.Application;
using Falcon.FSATS.FSTSimA.Application;
using Falcon.FSATS.ResourceManagement.Application;
using Falcon.FSATS.Trading.Application;
using Falcon.FSATS.TradingGuardian.Application;

internal static class Part6ConfigurationAdversarialChecks
{
    private const long Epoch = 6;
    private const long CoordinatorEpoch = 42;

    internal static void Run()
    {
        TradingChecks();
        ProviderChecks();
        GuardianChecks();
        ResourceChecks();
        SimulationChecks();
        Console.WriteLine("Part 6 Configuration / Policy Adversarial Verification: PASS");
    }

    private static void TradingChecks()
    {
        var safe = TradingSnapshot();
        var valid = TradingOperationalConfiguration.Assess(safe, Epoch);
        Require(valid.Accepted && valid.Condition == TradingConfigurationCondition.Valid, "P6_TRADING_VALID_REJECTED");
        Require(valid.CanApplyByConfigurationOnly, "P6_TRADING_SAFE_CONFIG_NOT_APPLICABLE");
        Require(!valid.GrantsRuntimeAuthority, "P6_TRADING_CONFIG_GRANTED_RUNTIME");

        Require(!TradingOperationalConfiguration.Assess(null, Epoch).Accepted, "P6_TRADING_NULL_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { ApplicationId = "FSATS-FSAPMA" }, Epoch).Accepted, "P6_TRADING_WRONG_APP_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { BrokerAccountId = " PA-001" }, Epoch).Accepted, "P6_TRADING_MALFORMED_ACCOUNT_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { ConfigurationDigest = "" }, Epoch).Accepted, "P6_TRADING_MISSING_DIGEST_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { EvidenceIntegrity = (TradingConfigurationEvidenceIntegrity)999 }, Epoch).Accepted, "P6_TRADING_BAD_ENUM_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { Compatibility = TradingConfigurationCompatibility.Unknown }, Epoch).Accepted, "P6_TRADING_UNKNOWN_COMPAT_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { ConfigurationEpoch = Epoch - 1 }, Epoch).Accepted, "P6_TRADING_STALE_EPOCH_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { ContainsSecretBytes = true }, Epoch).Accepted, "P6_TRADING_SECRET_ACCEPTED");
        Require(!TradingOperationalConfiguration.Assess(safe with { RequestsCrossAccountScopeExpansion = true }, Epoch).Accepted, "P6_TRADING_CROSS_ACCOUNT_EXPANSION_ACCEPTED");

        var execution = TradingOperationalConfiguration.Assess(safe with { RequestsBrokerExecutionEnablement = true }, Epoch);
        Require(execution.Condition == TradingConfigurationCondition.RequiresSeparateAuthority && !execution.CanApplyByConfigurationOnly, "P6_TRADING_EXECUTION_CONFIG_SELF_AUTHORIZED");

        var risk = TradingOperationalConfiguration.Assess(safe with { RequestsRiskIncrease = true }, Epoch);
        Require(risk.Condition == TradingConfigurationCondition.RequiresSeparateAuthority, "P6_TRADING_RISK_INCREASE_SELF_AUTHORIZED");

        var environment = TradingOperationalConfiguration.Assess(safe with { RequestsEnvironmentEscalation = true }, Epoch);
        Require(environment.Condition == TradingConfigurationCondition.RequiresSeparateAuthority && !environment.PreservesEnvironmentBoundary, "P6_TRADING_ENVIRONMENT_ESCALATION_LAUNDERED");

        var migrationMissing = TradingOperationalConfiguration.Assess(safe with { Compatibility = TradingConfigurationCompatibility.MigrationRequired }, Epoch);
        Require(migrationMissing.Condition == TradingConfigurationCondition.NotReady, "P6_TRADING_MIGRATION_WITHOUT_EVIDENCE_READY");

        var migrationValidated = TradingOperationalConfiguration.Assess(safe with { Compatibility = TradingConfigurationCompatibility.MigrationRequired, MigrationEvidenceValidated = true }, Epoch);
        Require(migrationValidated.Condition == TradingConfigurationCondition.MigrationRequired && !migrationValidated.CanApplyByConfigurationOnly, "P6_TRADING_MIGRATION_BYPASSED_LIFECYCLE");

        Require(TradingOperationalConfiguration.Assess(safe with { OperationalHealthEligible = false }, Epoch).Condition == TradingConfigurationCondition.NotReady, "P6_TRADING_CONFIG_OVERRULED_HEALTH");
    }

    private static void ProviderChecks()
    {
        var safe = ProviderSnapshot();
        var valid = ProviderOperationalConfiguration.Assess(safe, Epoch);
        Require(valid.Accepted && valid.Condition == ProviderConfigurationCondition.Valid, "P6_PROVIDER_VALID_REJECTED");
        Require(valid.PreservesSecretBoundary && !valid.GrantsRuntimeAuthority, "P6_PROVIDER_SECRET_OR_RUNTIME_BOUNDARY_LOST");

        Require(!ProviderOperationalConfiguration.Assess(safe with { EvidenceId = "" }, Epoch).Accepted, "P6_PROVIDER_MISSING_EVIDENCE_ACCEPTED");
        Require(!ProviderOperationalConfiguration.Assess(safe with { ContainsSecretBytes = true }, Epoch).Accepted, "P6_PROVIDER_SECRET_BYTES_ACCEPTED");
        Require(!ProviderOperationalConfiguration.Assess(safe with { ConfigurationEpoch = Epoch + 1 }, Epoch).Accepted, "P6_PROVIDER_WRONG_EPOCH_ACCEPTED");
        Require(!ProviderOperationalConfiguration.Assess(safe with { Compatibility = ProviderConfigurationCompatibility.Incompatible }, Epoch).Accepted, "P6_PROVIDER_INCOMPATIBLE_ACCEPTED");

        var egress = ProviderOperationalConfiguration.Assess(safe with { RequestsProviderEgressEnablement = true }, Epoch);
        Require(egress.Condition == ProviderConfigurationCondition.RequiresSeparateAuthority && !egress.CanApplyByConfigurationOnly, "P6_PROVIDER_EGRESS_SELF_AUTHORIZED");

        var env = ProviderOperationalConfiguration.Assess(safe with { RequestsEnvironmentEscalation = true }, Epoch);
        Require(env.Condition == ProviderConfigurationCondition.RequiresSeparateAuthority, "P6_PROVIDER_ENV_ESCALATION_LAUNDERED");

        Require(ProviderOperationalConfiguration.Assess(safe with { Compatibility = ProviderConfigurationCompatibility.MigrationRequired }, Epoch).Condition == ProviderConfigurationCondition.NotReady, "P6_PROVIDER_MIGRATION_WITHOUT_EVIDENCE_READY");
    }

    private static void GuardianChecks()
    {
        var safe = GuardianSnapshot();
        var valid = GuardianOperationalConfiguration.Assess(safe, Epoch);
        Require(valid.Accepted && valid.Condition == GuardianConfigurationCondition.Valid, "P6_GUARDIAN_VALID_REJECTED");
        Require(valid.PreservesHardProtection && valid.PreservesNoSelfRelease && !valid.GrantsRuntimeAuthority, "P6_GUARDIAN_SAFETY_BOUNDARY_LOST");

        Require(!GuardianOperationalConfiguration.Assess(safe with { AttemptsToWeakenHardProtection = true }, Epoch).Accepted, "P6_GUARDIAN_WEAKEN_PROTECTION_ACCEPTED");
        Require(!GuardianOperationalConfiguration.Assess(safe with { AttemptsSelfRelease = true }, Epoch).Accepted, "P6_GUARDIAN_SELF_RELEASE_ACCEPTED");
        Require(!GuardianOperationalConfiguration.Assess(safe with { EvidenceIntegrity = GuardianConfigurationEvidenceIntegrity.Invalid }, Epoch).Accepted, "P6_GUARDIAN_BAD_EVIDENCE_ACCEPTED");
        Require(!GuardianOperationalConfiguration.Assess(safe with { Compatibility = GuardianConfigurationCompatibility.Unknown }, Epoch).Accepted, "P6_GUARDIAN_UNKNOWN_COMPAT_ACCEPTED");

        var foundationRoute = GuardianOperationalConfiguration.Assess(safe with { RequestsFoundationProtectionRouteAuthority = true }, Epoch);
        Require(foundationRoute.Condition == GuardianConfigurationCondition.RequiresSeparateAuthority && !foundationRoute.CanApplyByConfigurationOnly, "P6_GUARDIAN_FOUNDATION_ROUTE_SELF_AUTHORIZED");
    }

    private static void ResourceChecks()
    {
        var safe = ResourceSnapshot();
        var valid = ResourceOperationalConfiguration.Assess(safe, Epoch, CoordinatorEpoch);
        Require(valid.Accepted && valid.Condition == ResourceConfigurationCondition.Valid, "P6_APP_RSC_VALID_REJECTED");
        Require(valid.PreservesFoundationAuthorityBoundary && !valid.GrantsRuntimeAuthority, "P6_APP_RSC_FOUNDATION_BOUNDARY_LOST");

        Require(!ResourceOperationalConfiguration.Assess(safe with { ClaimsFoundationGrantExpansion = true }, Epoch, CoordinatorEpoch).Accepted, "P6_APP_RSC_GRANT_EXPANSION_ACCEPTED");
        Require(!ResourceOperationalConfiguration.Assess(safe with { ReinterpretsFoundationCeilingOrFloor = true }, Epoch, CoordinatorEpoch).Accepted, "P6_APP_RSC_CEILING_REINTERPRETATION_ACCEPTED");
        Require(!ResourceOperationalConfiguration.Assess(safe with { CoordinatorEpoch = CoordinatorEpoch - 1 }, Epoch, CoordinatorEpoch).Accepted, "P6_APP_RSC_STALE_COORDINATOR_ACCEPTED");
        Require(!ResourceOperationalConfiguration.Assess(safe with { ConfigurationEpoch = Epoch - 1 }, Epoch, CoordinatorEpoch).Accepted, "P6_APP_RSC_STALE_CONFIG_ACCEPTED");

        var env = ResourceOperationalConfiguration.Assess(safe with { RequestsEnvironmentEscalation = true }, Epoch, CoordinatorEpoch);
        Require(env.Condition == ResourceConfigurationCondition.RequiresSeparateAuthority, "P6_APP_RSC_ENVIRONMENT_ESCALATION_LAUNDERED");
    }

    private static void SimulationChecks()
    {
        var safe = SimulationSnapshot();
        var valid = SimulationOperationalConfiguration.Assess(safe, Epoch);
        Require(valid.Accepted && valid.Condition == SimulationConfigurationCondition.Valid, "P6_FSTSIMA_VALID_REJECTED");
        Require(valid.PreservesSimulationBoundary && valid.PreservesNonLiveBoundary && !valid.GrantsRuntimeAuthority, "P6_FSTSIMA_BOUNDARY_LOST");

        Require(!SimulationOperationalConfiguration.Assess(safe with { ClaimsOperationalQualificationFromConfiguration = true }, Epoch).Accepted, "P6_FSTSIMA_CONFIG_MINTED_QUALIFICATION");
        Require(!SimulationOperationalConfiguration.Assess(safe with { Compatibility = SimulationConfigurationCompatibility.Incompatible }, Epoch).Accepted, "P6_FSTSIMA_INCOMPATIBLE_ACCEPTED");
        Require(!SimulationOperationalConfiguration.Assess(safe with { ConfigurationEpoch = Epoch + 1 }, Epoch).Accepted, "P6_FSTSIMA_WRONG_EPOCH_ACCEPTED");

        var live = SimulationOperationalConfiguration.Assess(safe with { RequestsLiveOrProductionEgress = true }, Epoch);
        Require(live.Condition == SimulationConfigurationCondition.RequiresSeparateAuthority && !live.CanApplyByConfigurationOnly, "P6_FSTSIMA_LIVE_SELF_AUTHORIZED");

        var classification = SimulationOperationalConfiguration.Assess(safe with { ReplayOrSyntheticOnly = false }, Epoch);
        Require(classification.Condition == SimulationConfigurationCondition.RequiresSeparateAuthority, "P6_FSTSIMA_NON_SIM_CLASSIFICATION_LAUNDERED");
    }

    private static TradingOperationalConfigurationSnapshot TradingSnapshot() => new(
        ApplicationId: TradingOperationalConfiguration.ApplicationId,
        BrokerId: "ALPACA",
        BrokerAccountId: "PA-001",
        Environment: "PAPER",
        ConfigurationId: "CFG-TR-01",
        ConfigurationVersion: "1.0.0",
        ConfigurationEpoch: Epoch,
        ConfigurationDigest: "SHA256:TRADING-CONFIG-01",
        EvidenceId: "EVID-P6-TR-01",
        EvidenceIntegrity: TradingConfigurationEvidenceIntegrity.Valid,
        Compatibility: TradingConfigurationCompatibility.Compatible,
        RiskPolicyReference: "RISK-POLICY-01",
        StrategyPolicyReference: "STRATEGY-POLICY-01",
        MigrationEvidenceValidated: false,
        ContainsSecretBytes: false,
        RequestsCrossAccountScopeExpansion: false,
        RequestsBrokerExecutionEnablement: false,
        RequestsRiskIncrease: false,
        RequestsEnvironmentEscalation: false,
        OperationalHealthEligible: true);

    private static ProviderOperationalConfigurationSnapshot ProviderSnapshot() => new(
        ApplicationId: ProviderOperationalConfiguration.ApplicationId,
        ProviderId: "ALPACA",
        ProviderAccountId: "DATA-01",
        ServiceRole: "MARKET_DATA",
        Environment: "PAPER",
        ConfigurationId: "CFG-PR-01",
        ConfigurationVersion: "1.0.0",
        ConfigurationEpoch: Epoch,
        ConfigurationDigest: "SHA256:PROVIDER-CONFIG-01",
        EvidenceId: "EVID-P6-PR-01",
        EvidenceIntegrity: ProviderConfigurationEvidenceIntegrity.Valid,
        Compatibility: ProviderConfigurationCompatibility.Compatible,
        CapabilityProfileReference: "CAP-PROFILE-01",
        QuotaEntitlementPolicyReference: "QUOTA-POLICY-01",
        CredentialReferenceId: "CRED-REF-01",
        MigrationEvidenceValidated: false,
        ContainsSecretBytes: false,
        RequestsProviderEgressEnablement: false,
        RequestsEnvironmentEscalation: false,
        OperationalHealthEligible: true);

    private static GuardianOperationalConfigurationSnapshot GuardianSnapshot() => new(
        ApplicationId: GuardianOperationalConfiguration.ApplicationId,
        ProtectedTargetId: "BROKER:ALPACA:PA-001:PAPER",
        Environment: "PAPER",
        ConfigurationId: "CFG-GUARD-01",
        ConfigurationVersion: "1.0.0",
        ConfigurationEpoch: Epoch,
        ConfigurationDigest: "SHA256:GUARD-CONFIG-01",
        EvidenceId: "EVID-P6-GUARD-01",
        EvidenceIntegrity: GuardianConfigurationEvidenceIntegrity.Valid,
        Compatibility: GuardianConfigurationCompatibility.Compatible,
        ProtectionPolicyReference: "PROTECTION-POLICY-01",
        MigrationEvidenceValidated: false,
        AttemptsToWeakenHardProtection: false,
        AttemptsSelfRelease: false,
        RequestsFoundationProtectionRouteAuthority: false,
        RequestsEnvironmentEscalation: false,
        OperationalHealthEligible: true);

    private static ResourceOperationalConfigurationSnapshot ResourceSnapshot() => new(
        ApplicationId: ResourceOperationalConfiguration.ApplicationId,
        Environment: "PAPER",
        ConfigurationId: "CFG-RSC-01",
        ConfigurationVersion: "1.0.0",
        ConfigurationEpoch: Epoch,
        CoordinatorEpoch: CoordinatorEpoch,
        ConfigurationDigest: "SHA256:RSC-CONFIG-01",
        EvidenceId: "EVID-P6-RSC-01",
        EvidenceIntegrity: ResourceConfigurationEvidenceIntegrity.Valid,
        Compatibility: ResourceConfigurationCompatibility.Compatible,
        FoundationEnvelopeReference: "FOUNDATION-ENVELOPE-01",
        ResourceProfileReference: "RESOURCE-PROFILE-01",
        SafetyFloorPolicyReference: "SAFETY-FLOOR-01",
        MigrationEvidenceValidated: false,
        ClaimsFoundationGrantExpansion: false,
        ReinterpretsFoundationCeilingOrFloor: false,
        RequestsEnvironmentEscalation: false,
        OperationalHealthEligible: true);

    private static SimulationOperationalConfigurationSnapshot SimulationSnapshot() => new(
        ApplicationId: SimulationOperationalConfiguration.ApplicationId,
        Environment: "SIMULATION",
        ConfigurationId: "CFG-SIM-01",
        ConfigurationVersion: "1.0.0",
        ConfigurationEpoch: Epoch,
        ConfigurationDigest: "SHA256:SIM-CONFIG-01",
        EvidenceId: "EVID-P6-SIM-01",
        EvidenceIntegrity: SimulationConfigurationEvidenceIntegrity.Valid,
        Compatibility: SimulationConfigurationCompatibility.Compatible,
        SimulationProfileReference: "SIM-PROFILE-01",
        RunClassificationPolicyReference: "RUN-CLASS-POLICY-01",
        MigrationEvidenceValidated: false,
        ReplayOrSyntheticOnly: true,
        RequestsLiveOrProductionEgress: false,
        ClaimsOperationalQualificationFromConfiguration: false,
        RequestsEnvironmentEscalation: false,
        OperationalHealthEligible: true);

    private static void Require(bool condition, string reason)
    {
        if (!condition)
            throw new InvalidOperationException(reason);
    }
}
