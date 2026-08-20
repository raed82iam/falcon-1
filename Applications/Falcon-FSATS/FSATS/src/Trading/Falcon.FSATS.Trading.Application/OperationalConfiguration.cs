namespace Falcon.FSATS.Trading.Application;

public enum TradingConfigurationEvidenceIntegrity { Valid, Invalid, Unknown }
public enum TradingConfigurationCompatibility { Compatible, MigrationRequired, Incompatible, Unknown }
public enum TradingConfigurationCondition { Valid, MigrationRequired, RequiresSeparateAuthority, NotReady }

public sealed record TradingOperationalConfigurationSnapshot(
    string ApplicationId,
    string BrokerId,
    string BrokerAccountId,
    string Environment,
    string ConfigurationId,
    string ConfigurationVersion,
    long ConfigurationEpoch,
    string ConfigurationDigest,
    string EvidenceId,
    TradingConfigurationEvidenceIntegrity EvidenceIntegrity,
    TradingConfigurationCompatibility Compatibility,
    string RiskPolicyReference,
    string StrategyPolicyReference,
    bool MigrationEvidenceValidated,
    bool ContainsSecretBytes,
    bool RequestsCrossAccountScopeExpansion,
    bool RequestsBrokerExecutionEnablement,
    bool RequestsRiskIncrease,
    bool RequestsEnvironmentEscalation,
    bool OperationalHealthEligible);

public sealed record TradingOperationalConfigurationAssessment(
    bool Accepted,
    TradingConfigurationCondition Condition,
    string ReasonCode,
    bool CurrentConfiguration,
    bool PreservesAccountBoundary,
    bool PreservesEnvironmentBoundary,
    bool CanApplyByConfigurationOnly,
    bool GrantsRuntimeAuthority)
{
    public static TradingOperationalConfigurationAssessment Reject(string reason) =>
        new(false, TradingConfigurationCondition.NotReady, reason, false, true, true, false, false);
}

public static class TradingOperationalConfiguration
{
    public const string ApplicationId = "FSATS-TRADING";

    public static TradingOperationalConfigurationAssessment Assess(
        TradingOperationalConfigurationSnapshot? snapshot,
        long expectedConfigurationEpoch)
    {
        if (snapshot is null)
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity) || !Enum.IsDefined(snapshot.Compatibility))
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) || !Valid(snapshot.BrokerId) || !Valid(snapshot.BrokerAccountId) ||
            !Valid(snapshot.Environment) || !Valid(snapshot.ConfigurationId) || !Valid(snapshot.ConfigurationVersion) ||
            !Valid(snapshot.ConfigurationDigest) || !Valid(snapshot.EvidenceId) || !Valid(snapshot.RiskPolicyReference) ||
            !Valid(snapshot.StrategyPolicyReference))
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_IDENTITY_OR_BINDING_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != TradingConfigurationEvidenceIntegrity.Valid)
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_EVIDENCE_NOT_VALID");

        if (snapshot.ConfigurationEpoch != expectedConfigurationEpoch)
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_STALE_EPOCH");

        if (snapshot.ContainsSecretBytes)
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_SECRET_BYTES_PROHIBITED");

        if (snapshot.RequestsCrossAccountScopeExpansion)
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_CROSS_ACCOUNT_EXPANSION_PROHIBITED");

        if (snapshot.Compatibility is TradingConfigurationCompatibility.Incompatible or TradingConfigurationCompatibility.Unknown)
            return TradingOperationalConfigurationAssessment.Reject("P6_TRADING_CONFIG_COMPATIBILITY_NOT_ESTABLISHED");

        if (!snapshot.OperationalHealthEligible)
            return new(true, TradingConfigurationCondition.NotReady, "P6_TRADING_CONFIG_OPERATIONAL_HEALTH_NOT_ELIGIBLE", true, true, true, false, false);

        if (snapshot.Compatibility == TradingConfigurationCompatibility.MigrationRequired)
        {
            if (!snapshot.MigrationEvidenceValidated)
                return new(true, TradingConfigurationCondition.NotReady, "P6_TRADING_CONFIG_MIGRATION_EVIDENCE_REQUIRED", true, true, true, false, false);

            return new(true, TradingConfigurationCondition.MigrationRequired, "P6_TRADING_CONFIG_MIGRATION_REQUIRES_LIFECYCLE_REVIEW", true, true, true, false, false);
        }

        if (snapshot.RequestsEnvironmentEscalation || snapshot.RequestsBrokerExecutionEnablement || snapshot.RequestsRiskIncrease)
            return new(true, TradingConfigurationCondition.RequiresSeparateAuthority, "P6_TRADING_CONFIG_AUTHORITY_BEARING_CHANGE_REQUIRES_SEPARATE_AUTHORITY", true, true, false, false, false);

        return new(true, TradingConfigurationCondition.Valid, "P6_TRADING_CONFIG_VALID_NON_AUTHORITY_CHANGE", true, true, true, true, false);
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
