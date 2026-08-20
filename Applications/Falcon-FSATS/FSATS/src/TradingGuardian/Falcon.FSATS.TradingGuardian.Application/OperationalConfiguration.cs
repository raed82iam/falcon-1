namespace Falcon.FSATS.TradingGuardian.Application;

public enum GuardianConfigurationEvidenceIntegrity { Valid, Invalid, Unknown }
public enum GuardianConfigurationCompatibility { Compatible, MigrationRequired, Incompatible, Unknown }
public enum GuardianConfigurationCondition { Valid, MigrationRequired, RequiresSeparateAuthority, NotReady }

public sealed record GuardianOperationalConfigurationSnapshot(
    string ApplicationId,
    string ProtectedTargetId,
    string Environment,
    string ConfigurationId,
    string ConfigurationVersion,
    long ConfigurationEpoch,
    string ConfigurationDigest,
    string EvidenceId,
    GuardianConfigurationEvidenceIntegrity EvidenceIntegrity,
    GuardianConfigurationCompatibility Compatibility,
    string ProtectionPolicyReference,
    bool MigrationEvidenceValidated,
    bool AttemptsToWeakenHardProtection,
    bool AttemptsSelfRelease,
    bool RequestsFoundationProtectionRouteAuthority,
    bool RequestsEnvironmentEscalation,
    bool OperationalHealthEligible);

public sealed record GuardianOperationalConfigurationAssessment(
    bool Accepted,
    GuardianConfigurationCondition Condition,
    string ReasonCode,
    bool CurrentConfiguration,
    bool PreservesHardProtection,
    bool PreservesNoSelfRelease,
    bool CanApplyByConfigurationOnly,
    bool GrantsRuntimeAuthority)
{
    public static GuardianOperationalConfigurationAssessment Reject(string reason) =>
        new(false, GuardianConfigurationCondition.NotReady, reason, false, true, true, false, false);
}

public static class GuardianOperationalConfiguration
{
    public const string ApplicationId = "FSATS-TRADING-GUARDIAN";

    public static GuardianOperationalConfigurationAssessment Assess(
        GuardianOperationalConfigurationSnapshot? snapshot,
        long expectedConfigurationEpoch)
    {
        if (snapshot is null)
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity) || !Enum.IsDefined(snapshot.Compatibility))
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) || !Valid(snapshot.ProtectedTargetId) || !Valid(snapshot.Environment) ||
            !Valid(snapshot.ConfigurationId) || !Valid(snapshot.ConfigurationVersion) || !Valid(snapshot.ConfigurationDigest) ||
            !Valid(snapshot.EvidenceId) || !Valid(snapshot.ProtectionPolicyReference))
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_IDENTITY_OR_BINDING_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != GuardianConfigurationEvidenceIntegrity.Valid)
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_EVIDENCE_NOT_VALID");

        if (snapshot.ConfigurationEpoch != expectedConfigurationEpoch)
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_STALE_EPOCH");

        if (snapshot.AttemptsToWeakenHardProtection)
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_HARD_PROTECTION_WEAKENING_PROHIBITED");

        if (snapshot.AttemptsSelfRelease)
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_SELF_RELEASE_PROHIBITED");

        if (snapshot.Compatibility is GuardianConfigurationCompatibility.Incompatible or GuardianConfigurationCompatibility.Unknown)
            return GuardianOperationalConfigurationAssessment.Reject("P6_GUARDIAN_CONFIG_COMPATIBILITY_NOT_ESTABLISHED");

        if (!snapshot.OperationalHealthEligible)
            return new(true, GuardianConfigurationCondition.NotReady, "P6_GUARDIAN_CONFIG_OPERATIONAL_HEALTH_NOT_ELIGIBLE", true, true, true, false, false);

        if (snapshot.Compatibility == GuardianConfigurationCompatibility.MigrationRequired)
        {
            if (!snapshot.MigrationEvidenceValidated)
                return new(true, GuardianConfigurationCondition.NotReady, "P6_GUARDIAN_CONFIG_MIGRATION_EVIDENCE_REQUIRED", true, true, true, false, false);

            return new(true, GuardianConfigurationCondition.MigrationRequired, "P6_GUARDIAN_CONFIG_MIGRATION_REQUIRES_LIFECYCLE_REVIEW", true, true, true, false, false);
        }

        if (snapshot.RequestsFoundationProtectionRouteAuthority || snapshot.RequestsEnvironmentEscalation)
            return new(true, GuardianConfigurationCondition.RequiresSeparateAuthority, "P6_GUARDIAN_CONFIG_AUTHORITY_BEARING_CHANGE_REQUIRES_SEPARATE_AUTHORITY", true, true, true, false, false);

        return new(true, GuardianConfigurationCondition.Valid, "P6_GUARDIAN_CONFIG_VALID_NON_AUTHORITY_CHANGE", true, true, true, true, false);
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
