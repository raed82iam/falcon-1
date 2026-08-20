namespace Falcon.FSATS.FSAPMA.Application;

public sealed record FoundationOnboardingDeclaration(
    string AdmissionKind,
    string ApplicationId,
    string ApplicationVersion,
    string ApplicationOwner,
    string PackageId,
    string ManifestId,
    string RequiredApplicationContract,
    string RequiredApplicationContractVersion,
    string RequiredApplicationSpecification,
    string RequiredApplicationSpecificationVersion,
    IReadOnlyList<string> RequiredFoundationServices,
    string BootstrapContextState,
    string ProviderBoundary,
    string MsaId,
    IReadOnlyList<string> LsaIds,
    IReadOnlyList<string> CsaIds,
    bool ExactArtifactIdentityRequired,
    bool PositiveAdmissionEvidenceRequired,
    bool LifecycleAttachEligibilityRequired,
    bool CurrentFoundationResourceGrantRequired,
    bool RuntimeRegistrationMayAuthorizeActivation,
    bool RuntimeRegistrationMayAuthorizeDeployment,
    bool RuntimeRegistrationMayAuthorizeProduction,
    bool RuntimeRegistrationMayGrantBusinessAuthority,
    bool SilentUpgradeAllowed,
    bool ExternalConnectivityActivated,
    bool PaperAuthorityGranted,
    bool LiveAuthorityGranted);

public static class FSAPMAFoundationOnboarding
{
    public static FoundationOnboardingDeclaration Current { get; } = new(
        AdmissionKind: "APPLICATION",
        ApplicationId: FSAPMAManifest.Current.ApplicationId,
        ApplicationVersion: FSAPMAManifest.Current.Version,
        ApplicationOwner: FSAPMAManifest.Current.Owner,
        PackageId: FSAPMAManifest.Current.PackageId,
        ManifestId: "manifest:fsats:fsapma:foundation-admission:v1",
        RequiredApplicationContract: "CON-023",
        RequiredApplicationContractVersion: "1.1",
        RequiredApplicationSpecification: "APP-001",
        RequiredApplicationSpecificationVersion: "1.0",
        RequiredFoundationServices: Array.AsReadOnly(new[] { "ApplicationLifecycle", "Communication", "Evidence", "Resources", "Security", "CredentialReferenceResolution", "Persistence" }),
        BootstrapContextState: "DEFINED",
        ProviderBoundary: "OPERATIONAL_PROVIDER_DATA_DECLARED_GOVERNED_AND_DISABLED_UNTIL_SEPARATE_CONNECTIVITY_AUTHORITY",
        MsaId: FSAPMAManifest.Current.MsaId,
        LsaIds: FSAPMAManifest.Current.LsaIds,
        CsaIds: FSAPMAManifest.Current.CsaIds,
        ExactArtifactIdentityRequired: true,
        PositiveAdmissionEvidenceRequired: true,
        LifecycleAttachEligibilityRequired: true,
        CurrentFoundationResourceGrantRequired: true,
        RuntimeRegistrationMayAuthorizeActivation: false,
        RuntimeRegistrationMayAuthorizeDeployment: false,
        RuntimeRegistrationMayAuthorizeProduction: false,
        RuntimeRegistrationMayGrantBusinessAuthority: false,
        SilentUpgradeAllowed: false,
        ExternalConnectivityActivated: false,
        PaperAuthorityGranted: false,
        LiveAuthorityGranted: false);

    public static bool IsApplicationSideReadyForFoundationAdmission()
    {
        var value = Current;
        return value.AdmissionKind == "APPLICATION"
            && value.ApplicationId == FSAPMAManifest.Current.ApplicationId
            && value.ApplicationVersion == FSAPMAManifest.Current.Version
            && value.ApplicationOwner == FSAPMAManifest.Current.Owner
            && value.PackageId == FSAPMAManifest.Current.PackageId
            && !string.IsNullOrWhiteSpace(value.ManifestId)
            && value.RequiredApplicationContract == "CON-023"
            && value.RequiredApplicationContractVersion == "1.1"
            && value.RequiredApplicationSpecification == "APP-001"
            && value.RequiredApplicationSpecificationVersion == "1.0"
            && value.RequiredFoundationServices.Count > 0
            && value.BootstrapContextState == "DEFINED"
            && !string.IsNullOrWhiteSpace(value.ProviderBoundary)
            && value.MsaId == FSAPMAManifest.Current.MsaId
            && value.LsaIds.SequenceEqual(FSAPMAManifest.Current.LsaIds, StringComparer.Ordinal)
            && value.CsaIds.SequenceEqual(FSAPMAManifest.Current.CsaIds, StringComparer.Ordinal)
            && value.ExactArtifactIdentityRequired
            && value.PositiveAdmissionEvidenceRequired
            && value.LifecycleAttachEligibilityRequired
            && value.CurrentFoundationResourceGrantRequired
            && !value.RuntimeRegistrationMayAuthorizeActivation
            && !value.RuntimeRegistrationMayAuthorizeDeployment
            && !value.RuntimeRegistrationMayAuthorizeProduction
            && !value.RuntimeRegistrationMayGrantBusinessAuthority
            && !value.SilentUpgradeAllowed
            && !value.ExternalConnectivityActivated
            && !value.PaperAuthorityGranted
            && !value.LiveAuthorityGranted;
    }
}
