namespace Falcon.FSATS.ResourceManagement.Application;

public enum ResourceConfigurationEvidenceIntegrity { Valid, Invalid, Unknown }
public enum ResourceConfigurationCompatibility { Compatible, MigrationRequired, Incompatible, Unknown }
public enum ResourceConfigurationCondition { Valid, MigrationRequired, RequiresSeparateAuthority, NotReady }

public sealed record ResourceOperationalConfigurationSnapshot(
    string ApplicationId,
    string Environment,
    string ConfigurationId,
    string ConfigurationVersion,
    long ConfigurationEpoch,
    long CoordinatorEpoch,
    string ConfigurationDigest,
    string EvidenceId,
    ResourceConfigurationEvidenceIntegrity EvidenceIntegrity,
    ResourceConfigurationCompatibility Compatibility,
    string FoundationEnvelopeReference,
    string ResourceProfileReference,
    string SafetyFloorPolicyReference,
    bool MigrationEvidenceValidated,
    bool ClaimsFoundationGrantExpansion,
    bool ReinterpretsFoundationCeilingOrFloor,
    bool RequestsEnvironmentEscalation,
    bool OperationalHealthEligible);

public sealed record ResourceOperationalConfigurationAssessment(
    bool Accepted,
    ResourceConfigurationCondition Condition,
    string ReasonCode,
    bool CurrentConfiguration,
    bool PreservesFoundationAuthorityBoundary,
    bool PreservesSafetyFloorBoundary,
    bool CanApplyByConfigurationOnly,
    bool GrantsRuntimeAuthority)
{
    public static ResourceOperationalConfigurationAssessment Reject(string reason) =>
        new(false, ResourceConfigurationCondition.NotReady, reason, false, true, true, false, false);
}

public static class ResourceOperationalConfiguration
{
    public const string ApplicationId = "APP-RSC";

    public static ResourceOperationalConfigurationAssessment Assess(
        ResourceOperationalConfigurationSnapshot? snapshot,
        long expectedConfigurationEpoch,
        long expectedCoordinatorEpoch)
    {
        if (snapshot is null)
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity) || !Enum.IsDefined(snapshot.Compatibility))
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) || !Valid(snapshot.Environment) || !Valid(snapshot.ConfigurationId) ||
            !Valid(snapshot.ConfigurationVersion) || !Valid(snapshot.ConfigurationDigest) || !Valid(snapshot.EvidenceId) ||
            !Valid(snapshot.FoundationEnvelopeReference) || !Valid(snapshot.ResourceProfileReference) ||
            !Valid(snapshot.SafetyFloorPolicyReference))
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_IDENTITY_OR_BINDING_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != ResourceConfigurationEvidenceIntegrity.Valid)
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_EVIDENCE_NOT_VALID");

        if (snapshot.ConfigurationEpoch != expectedConfigurationEpoch || snapshot.CoordinatorEpoch != expectedCoordinatorEpoch)
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_STALE_EPOCH");

        if (snapshot.ClaimsFoundationGrantExpansion || snapshot.ReinterpretsFoundationCeilingOrFloor)
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_FOUNDATION_AUTHORITY_MINTING_PROHIBITED");

        if (snapshot.Compatibility is ResourceConfigurationCompatibility.Incompatible or ResourceConfigurationCompatibility.Unknown)
            return ResourceOperationalConfigurationAssessment.Reject("P6_APP_RSC_CONFIG_COMPATIBILITY_NOT_ESTABLISHED");

        if (!snapshot.OperationalHealthEligible)
            return new(true, ResourceConfigurationCondition.NotReady, "P6_APP_RSC_CONFIG_OPERATIONAL_HEALTH_NOT_ELIGIBLE", true, true, true, false, false);

        if (snapshot.Compatibility == ResourceConfigurationCompatibility.MigrationRequired)
        {
            if (!snapshot.MigrationEvidenceValidated)
                return new(true, ResourceConfigurationCondition.NotReady, "P6_APP_RSC_CONFIG_MIGRATION_EVIDENCE_REQUIRED", true, true, true, false, false);

            return new(true, ResourceConfigurationCondition.MigrationRequired, "P6_APP_RSC_CONFIG_MIGRATION_REQUIRES_LIFECYCLE_REVIEW", true, true, true, false, false);
        }

        if (snapshot.RequestsEnvironmentEscalation)
            return new(true, ResourceConfigurationCondition.RequiresSeparateAuthority, "P6_APP_RSC_CONFIG_ENVIRONMENT_CHANGE_REQUIRES_SEPARATE_AUTHORITY", true, true, true, false, false);

        return new(true, ResourceConfigurationCondition.Valid, "P6_APP_RSC_CONFIG_VALID_NON_AUTHORITY_CHANGE", true, true, true, true, false);
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
