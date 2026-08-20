namespace Falcon.FSATS.ResourceManagement.Application;

public sealed record FoundationOnboardingDeclaration(
    string AdmissionKind, string ApplicationId, string ApplicationVersion, string ApplicationOwner, string PackageId, string ManifestId,
    string RequiredApplicationContract, string RequiredApplicationContractVersion, string RequiredApplicationSpecification, string RequiredApplicationSpecificationVersion,
    IReadOnlyList<string> RequiredFoundationServices, string BootstrapContextState, string ProviderBoundary,
    string MsaId, IReadOnlyList<string> LsaIds, IReadOnlyList<string> CsaIds,
    bool ExactArtifactIdentityRequired, bool PositiveAdmissionEvidenceRequired, bool LifecycleAttachEligibilityRequired, bool CurrentFoundationResourceGrantRequired,
    bool RuntimeRegistrationMayAuthorizeActivation, bool RuntimeRegistrationMayAuthorizeDeployment, bool RuntimeRegistrationMayAuthorizeProduction,
    bool RuntimeRegistrationMayGrantBusinessAuthority, bool SilentUpgradeAllowed, bool ExternalConnectivityActivated, bool PaperAuthorityGranted, bool LiveAuthorityGranted);

public static class ResourceManagementFoundationOnboarding
{
    public static FoundationOnboardingDeclaration Current { get; } = new(
        "APPLICATION", ResourceManagementManifest.Current.ApplicationId, ResourceManagementManifest.Current.Version, ResourceManagementManifest.Current.Owner,
        ResourceManagementManifest.Current.PackageId, "manifest:fsats:app-rsc:foundation-admission:v1",
        "CON-023", "1.1", "APP-001", "1.0",
        Array.AsReadOnly(new[] { "ApplicationLifecycle", "Communication", "Evidence", "Resources", "Security", "Persistence" }),
        "DEFINED", "FOUNDATION_RESOURCE_BINDING_DECLARED_GOVERNED_AND_UNBOUND_UNTIL_FOUNDATION_ADMISSION_AND_CURRENT_GRANT_EVIDENCE",
        ResourceManagementManifest.Current.MsaId, ResourceManagementManifest.Current.LsaIds, ResourceManagementManifest.Current.CsaIds,
        true, true, true, true, false, false, false, false, false, false, false, false);

    public static bool IsApplicationSideReadyForFoundationAdmission()
    {
        var v = Current;
        return v.AdmissionKind == "APPLICATION" && v.ApplicationId == ResourceManagementManifest.Current.ApplicationId
            && v.ApplicationVersion == ResourceManagementManifest.Current.Version && v.ApplicationOwner == ResourceManagementManifest.Current.Owner
            && v.PackageId == ResourceManagementManifest.Current.PackageId && !string.IsNullOrWhiteSpace(v.ManifestId)
            && v.RequiredApplicationContract == "CON-023" && v.RequiredApplicationContractVersion == "1.1"
            && v.RequiredApplicationSpecification == "APP-001" && v.RequiredApplicationSpecificationVersion == "1.0"
            && v.RequiredFoundationServices.Count > 0 && v.BootstrapContextState == "DEFINED" && !string.IsNullOrWhiteSpace(v.ProviderBoundary)
            && v.MsaId == ResourceManagementManifest.Current.MsaId
            && v.LsaIds.SequenceEqual(ResourceManagementManifest.Current.LsaIds, StringComparer.Ordinal)
            && v.CsaIds.SequenceEqual(ResourceManagementManifest.Current.CsaIds, StringComparer.Ordinal)
            && v.ExactArtifactIdentityRequired && v.PositiveAdmissionEvidenceRequired && v.LifecycleAttachEligibilityRequired && v.CurrentFoundationResourceGrantRequired
            && !v.RuntimeRegistrationMayAuthorizeActivation && !v.RuntimeRegistrationMayAuthorizeDeployment && !v.RuntimeRegistrationMayAuthorizeProduction
            && !v.RuntimeRegistrationMayGrantBusinessAuthority && !v.SilentUpgradeAllowed && !v.ExternalConnectivityActivated && !v.PaperAuthorityGranted && !v.LiveAuthorityGranted;
    }
}
