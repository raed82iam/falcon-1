namespace Falcon.FSATS.Trading.Application;

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

public static class TradingFoundationOnboarding
{
    public static FoundationOnboardingDeclaration Current { get; } = new(
        AdmissionKind: "APPLICATION",
        ApplicationId: TradingManifest.Current.ApplicationId,
        ApplicationVersion: TradingManifest.Current.Version,
        ApplicationOwner: TradingManifest.Current.Owner,
        PackageId: TradingManifest.Current.PackageId,
        ManifestId: "manifest:fsats:trading:foundation-admission:v1",
        RequiredApplicationContract: "CON-023",
        RequiredApplicationContractVersion: "1.1",
        RequiredApplicationSpecification: "APP-001",
        RequiredApplicationSpecificationVersion: "1.0",
        RequiredFoundationServices: Array.AsReadOnly(new[] { "ApplicationLifecycle", "Communication", "Evidence", "Resources", "Security", "Persistence" }),
        BootstrapContextState: "DEFINED",
        ProviderBoundary: "BROKER_EXECUTION_DECLARED_GOVERNED_AND_DISABLED_UNTIL_SEPARATE_CONNECTIVITY_AUTHORITY",
        MsaId: TradingManifest.Current.MsaId,
        LsaIds: TradingManifest.Current.LsaIds,
        CsaIds: TradingManifest.Current.CsaIds,
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
            && value.ApplicationId == TradingManifest.Current.ApplicationId
            && value.ApplicationVersion == TradingManifest.Current.Version
            && value.ApplicationOwner == TradingManifest.Current.Owner
            && value.PackageId == TradingManifest.Current.PackageId
            && !string.IsNullOrWhiteSpace(value.ManifestId)
            && value.RequiredApplicationContract == "CON-023"
            && value.RequiredApplicationContractVersion == "1.1"
            && value.RequiredApplicationSpecification == "APP-001"
            && value.RequiredApplicationSpecificationVersion == "1.0"
            && value.RequiredFoundationServices.Count > 0
            && value.BootstrapContextState == "DEFINED"
            && !string.IsNullOrWhiteSpace(value.ProviderBoundary)
            && value.MsaId == TradingManifest.Current.MsaId
            && value.LsaIds.SequenceEqual(TradingManifest.Current.LsaIds, StringComparer.Ordinal)
            && value.CsaIds.SequenceEqual(TradingManifest.Current.CsaIds, StringComparer.Ordinal)
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
