using Falcon.FSATS.FSAPMA.Domain;

namespace Falcon.FSATS.FSAPMA.Application;

public enum ProviderConfigurationEvidenceIntegrity { Valid, Invalid, Unknown }
public enum ProviderConfigurationCompatibility { Compatible, MigrationRequired, Incompatible, Unknown }
public enum ProviderConfigurationCondition { Valid, MigrationRequired, RequiresSeparateAuthority, NotReady }

public sealed record ProviderOperationalConfigurationSnapshot(
    string ApplicationId,
    string ProviderId,
    string ProviderAccountId,
    string ServiceRole,
    string Environment,
    string ConfigurationId,
    string ConfigurationVersion,
    long ConfigurationEpoch,
    string ConfigurationDigest,
    string EvidenceId,
    ProviderConfigurationEvidenceIntegrity EvidenceIntegrity,
    ProviderConfigurationCompatibility Compatibility,
    string CapabilityProfileReference,
    string QuotaEntitlementPolicyReference,
    string CredentialReferenceId,
    bool MigrationEvidenceValidated,
    bool ContainsSecretBytes,
    bool RequestsProviderEgressEnablement,
    bool RequestsEnvironmentEscalation,
    bool OperationalHealthEligible)
{
    public string ApiInstanceId { get; init; } = string.Empty;
    public string EndpointId { get; init; } = string.Empty;
    public string EndpointBaseUrl { get; init; } = string.Empty;

    public bool HasCurrentProviderRouteBinding
        => ProviderOperationalConfiguration.IsCurrentRouteBindingComplete(this);
}

public sealed record ProviderOperationalConfigurationAssessment(
    bool Accepted,
    ProviderConfigurationCondition Condition,
    string ReasonCode,
    bool CurrentConfiguration,
    bool PreservesProviderAccountBoundary,
    bool PreservesSecretBoundary,
    bool CanApplyByConfigurationOnly,
    bool GrantsRuntimeAuthority)
{
    public static ProviderOperationalConfigurationAssessment Reject(string reason) =>
        new(false, ProviderConfigurationCondition.NotReady, reason, false, true, true, false, false);
}

public static class ProviderOperationalConfiguration
{
    public const string ApplicationId = "FSATS-FSAPMA";

    public static ProviderOperationalConfigurationAssessment Assess(
        ProviderOperationalConfigurationSnapshot? snapshot,
        long expectedConfigurationEpoch)
    {
        if (snapshot is null)
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_INPUT_REQUIRED");

        if (!Enum.IsDefined(snapshot.EvidenceIntegrity) || !Enum.IsDefined(snapshot.Compatibility))
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_ENUM_INVALID");

        if (!Valid(snapshot.ApplicationId) || !Valid(snapshot.ProviderId) || !Valid(snapshot.ProviderAccountId) ||
            !Valid(snapshot.ServiceRole) || !Valid(snapshot.Environment) || !Valid(snapshot.ConfigurationId) ||
            !Valid(snapshot.ConfigurationVersion) || !Valid(snapshot.ConfigurationDigest) || !Valid(snapshot.EvidenceId) ||
            !Valid(snapshot.CapabilityProfileReference) || !Valid(snapshot.QuotaEntitlementPolicyReference) ||
            !Valid(snapshot.CredentialReferenceId))
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_IDENTITY_OR_BINDING_INCOMPLETE");

        if (!StringComparer.Ordinal.Equals(snapshot.ApplicationId, ApplicationId))
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_APPLICATION_ID_MISMATCH");

        if (snapshot.EvidenceIntegrity != ProviderConfigurationEvidenceIntegrity.Valid)
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_EVIDENCE_NOT_VALID");

        if (snapshot.ConfigurationEpoch != expectedConfigurationEpoch)
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_STALE_EPOCH");

        if (snapshot.ContainsSecretBytes)
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_SECRET_BYTES_PROHIBITED");

        if (snapshot.Compatibility is ProviderConfigurationCompatibility.Incompatible or ProviderConfigurationCompatibility.Unknown)
            return ProviderOperationalConfigurationAssessment.Reject("P6_PROVIDER_CONFIG_COMPATIBILITY_NOT_ESTABLISHED");

        if (!snapshot.OperationalHealthEligible)
            return new(true, ProviderConfigurationCondition.NotReady, "P6_PROVIDER_CONFIG_OPERATIONAL_HEALTH_NOT_ELIGIBLE", true, true, true, false, false);

        if (snapshot.Compatibility == ProviderConfigurationCompatibility.MigrationRequired)
        {
            if (!snapshot.MigrationEvidenceValidated)
                return new(true, ProviderConfigurationCondition.NotReady, "P6_PROVIDER_CONFIG_MIGRATION_EVIDENCE_REQUIRED", true, true, true, false, false);

            return new(true, ProviderConfigurationCondition.MigrationRequired, "P6_PROVIDER_CONFIG_MIGRATION_REQUIRES_LIFECYCLE_REVIEW", true, true, true, false, false);
        }

        if (snapshot.RequestsProviderEgressEnablement || snapshot.RequestsEnvironmentEscalation)
            return new(true, ProviderConfigurationCondition.RequiresSeparateAuthority, "P6_PROVIDER_CONFIG_EGRESS_OR_ENVIRONMENT_CHANGE_REQUIRES_SEPARATE_AUTHORITY", true, true, true, false, false);

        return new(true, ProviderConfigurationCondition.Valid, "P6_PROVIDER_CONFIG_VALID_NON_AUTHORITY_CHANGE", true, true, true, true, false);
    }

    public static ProviderOperationalConfigurationAssessment AssessCurrentRouteBinding(
        ProviderOperationalConfigurationSnapshot? snapshot,
        long expectedConfigurationEpoch)
    {
        var historicalAssessment = Assess(snapshot, expectedConfigurationEpoch);
        if (!historicalAssessment.Accepted || snapshot is null) return historicalAssessment;
        if (!IsCurrentRouteBindingComplete(snapshot))
            return ProviderOperationalConfigurationAssessment.Reject("CURRENT_PROVIDER_ROUTE_BINDING_INCOMPLETE_OR_CATALOG_MISMATCH");
        return historicalAssessment with { ReasonCode = "CURRENT_PROVIDER_ROUTE_BINDING_VALID" };
    }

    public static bool IsCurrentRouteBindingComplete(ProviderOperationalConfigurationSnapshot? snapshot)
    {
        if (snapshot is null || !Valid(snapshot.ApiInstanceId) || !Valid(snapshot.EndpointId) || !Valid(snapshot.EndpointBaseUrl)) return false;
        if (!Uri.TryCreate(snapshot.EndpointBaseUrl, UriKind.Absolute, out var endpoint)) return false;
        if (endpoint.Scheme is not ("https" or "wss")) return false;
        if (!string.IsNullOrEmpty(endpoint.UserInfo) || !string.IsNullOrEmpty(endpoint.Query) || !string.IsNullOrEmpty(endpoint.Fragment)) return false;

        var catalog = ProviderStreamingCatalog.Find(snapshot.EndpointId);
        if (catalog is null) return false;
        if (!StringComparer.Ordinal.Equals(catalog.Provider.Value, snapshot.ProviderId.Trim().ToUpperInvariant())) return false;
        if (!StringComparer.Ordinal.Equals(catalog.ServiceRole, snapshot.ServiceRole.Trim().ToUpperInvariant())) return false;
        if (Uri.Compare(endpoint, catalog.Endpoint, UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) != 0) return false;

        return true;
    }

    private static bool Valid(string? value) =>
        !string.IsNullOrWhiteSpace(value) && StringComparer.Ordinal.Equals(value, value.Trim());
}
