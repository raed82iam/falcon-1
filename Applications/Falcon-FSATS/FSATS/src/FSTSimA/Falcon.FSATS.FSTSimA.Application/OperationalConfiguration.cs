namespace Falcon.FSATS.FSTSimA.Application;

public enum SimulationConfigurationEvidenceIntegrity { Valid, Invalid, Unknown }
public enum SimulationConfigurationCompatibility { Compatible, MigrationRequired, Incompatible, Unknown }
public enum SimulationConfigurationCondition { Valid, MigrationRequired, RequiresSeparateAuthority, NotReady }

public sealed record SimulationOperationalConfigurationSnapshot(
    string ApplicationId,
    string Environment,
    string ConfigurationId,
    string ConfigurationVersion,
    long ConfigurationEpoch,
    string ConfigurationDigest,
    string EvidenceId,
    SimulationConfigurationEvidenceIntegrity EvidenceIntegrity,
    SimulationConfigurationCompatibility Compatibility,
    string SimulationProfileReference,
    string RunClassificationPolicyReference,
    bool MigrationEvidenceValidated,
    bool ReplayOrSyntheticOnly,
    bool RequestsLiveOrProductionEgress,
    bool ClaimsOperationalQualificationFromConfiguration,
    bool RequestsEnvironmentEscalation,
    bool OperationalHealthEligible);

public sealed record SimulationOperationalConfigurationAssessment(
    bool Accepted,
    SimulationConfigurationCondition Condition,
    string ReasonCode,
    bool CurrentConfiguration,
    bool PreservesSimulationBoundary,
    bool PreservesNonLiveBoundary,
    bool CanApplyByConfigurationOnly,
    bool GrantsRuntimeAuthority)
{
    public static SimulationOperationalConfigurationAssessment Reject(string reason) =>
        new(false, SimulationConfigurationCondition.NotReady, reason, false, true, true, false, false);
}

public static class SimulationOperationalConfiguration
{
    public const string ApplicationId = "FSATS-FSTSIMA";

    public static SimulationOperationalConfigurationAssessment Assess(
        SimulationOperationalConfigurationSnapshot? snapshot,
        long expectedConfigurationEpoch)
    {
        if (snapshot is null)
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity) || !Enum.IsDefined(snapshot.Compatibility))
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) || !Valid(snapshot.Environment) || !Valid(snapshot.ConfigurationId) ||
            !Valid(snapshot.ConfigurationVersion) || !Valid(snapshot.ConfigurationDigest) || !Valid(snapshot.EvidenceId) ||
            !Valid(snapshot.SimulationProfileReference) || !Valid(snapshot.RunClassificationPolicyReference))
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_IDENTITY_OR_BINDING_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != SimulationConfigurationEvidenceIntegrity.Valid)
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_EVIDENCE_NOT_VALID");

        if (snapshot.ConfigurationEpoch != expectedConfigurationEpoch)
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_STALE_EPOCH");

        if (snapshot.ClaimsOperationalQualificationFromConfiguration)
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_QUALIFICATION_MINTING_PROHIBITED");

        if (snapshot.Compatibility is SimulationConfigurationCompatibility.Incompatible or SimulationConfigurationCompatibility.Unknown)
            return SimulationOperationalConfigurationAssessment.Reject("P6_FSTSIMA_CONFIG_COMPATIBILITY_NOT_ESTABLISHED");

        if (!snapshot.OperationalHealthEligible)
            return new(true, SimulationConfigurationCondition.NotReady, "P6_FSTSIMA_CONFIG_OPERATIONAL_HEALTH_NOT_ELIGIBLE", true, true, true, false, false);

        if (snapshot.Compatibility == SimulationConfigurationCompatibility.MigrationRequired)
        {
            if (!snapshot.MigrationEvidenceValidated)
                return new(true, SimulationConfigurationCondition.NotReady, "P6_FSTSIMA_CONFIG_MIGRATION_EVIDENCE_REQUIRED", true, true, true, false, false);

            return new(true, SimulationConfigurationCondition.MigrationRequired, "P6_FSTSIMA_CONFIG_MIGRATION_REQUIRES_LIFECYCLE_REVIEW", true, true, true, false, false);
        }

        if (snapshot.RequestsLiveOrProductionEgress || snapshot.RequestsEnvironmentEscalation)
            return new(true, SimulationConfigurationCondition.RequiresSeparateAuthority, "P6_FSTSIMA_CONFIG_LIVE_OR_ENVIRONMENT_CHANGE_REQUIRES_SEPARATE_AUTHORITY", true, true, false, false, false);

        if (!snapshot.ReplayOrSyntheticOnly)
            return new(true, SimulationConfigurationCondition.RequiresSeparateAuthority, "P6_FSTSIMA_CONFIG_NON_SIMULATION_CLASSIFICATION_REQUIRES_SEPARATE_AUTHORITY", true, true, false, false, false);

        return new(true, SimulationConfigurationCondition.Valid, "P6_FSTSIMA_CONFIG_VALID_NON_AUTHORITY_CHANGE", true, true, true, true, false);
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
