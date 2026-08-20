namespace Falcon.FSATS.FSTSimA.Application;

public sealed record FoundationOnboardingDeclaration(
    string AdmissionKind, string ApplicationId, string ApplicationVersion, string ApplicationOwner, string PackageId, string ManifestId,
    string RequiredApplicationContract, string RequiredApplicationContractVersion, string RequiredApplicationSpecification, string RequiredApplicationSpecificationVersion,
    IReadOnlyList<string> RequiredFoundationServices, string BootstrapContextState, string ProviderBoundary,
    string MsaId, IReadOnlyList<string> LsaIds, IReadOnlyList<string> CsaIds,
    bool ExactArtifactIdentityRequired, bool PositiveAdmissionEvidenceRequired, bool LifecycleAttachEligibilityRequired, bool CurrentFoundationResourceGrantRequired,
    bool RuntimeRegistrationMayAuthorizeActivation, bool RuntimeRegistrationMayAuthorizeDeployment, bool RuntimeRegistrationMayAuthorizeProduction,
    bool RuntimeRegistrationMayGrantBusinessAuthority, bool SilentUpgradeAllowed, bool ExternalConnectivityActivated, bool PaperAuthorityGranted, bool LiveAuthorityGranted);

public static class FSTSimAFoundationOnboarding
{
    public static FoundationOnboardingDeclaration Current { get; } = new(
        "APPLICATION", FSTSimAManifest.Current.ApplicationId, FSTSimAManifest.Current.Version, FSTSimAManifest.Current.Owner,
        FSTSimAManifest.Current.PackageId, "manifest:fsats:fstsim-a:foundation-admission:v1",
        "CON-023", "1.1", "APP-001", "1.0",
        Array.AsReadOnly(new[] { "ApplicationLifecycle", "Communication", "Evidence", "Resources", "Security", "Persistence" }),
        "DEFINED", "NON_LIVE_SIMULATION_ONLY_NO_OPERATIONAL_PROVIDER_OR_BROKER_CONNECTIVITY",
        FSTSimAManifest.Current.MsaId, FSTSimAManifest.Current.LsaIds, FSTSimAManifest.Current.CsaIds,
        true, true, true, true, false, false, false, false, false, false, false, false);

    public static bool IsApplicationSideReadyForFoundationAdmission()
    {
        var v = Current;
        return v.AdmissionKind == "APPLICATION" && v.ApplicationId == FSTSimAManifest.Current.ApplicationId
            && v.ApplicationVersion == FSTSimAManifest.Current.Version && v.ApplicationOwner == FSTSimAManifest.Current.Owner
            && v.PackageId == FSTSimAManifest.Current.PackageId && !string.IsNullOrWhiteSpace(v.ManifestId)
            && v.RequiredApplicationContract == "CON-023" && v.RequiredApplicationContractVersion == "1.1"
            && v.RequiredApplicationSpecification == "APP-001" && v.RequiredApplicationSpecificationVersion == "1.0"
            && v.RequiredFoundationServices.Count > 0 && v.BootstrapContextState == "DEFINED" && !string.IsNullOrWhiteSpace(v.ProviderBoundary)
            && v.MsaId == FSTSimAManifest.Current.MsaId
            && v.LsaIds.SequenceEqual(FSTSimAManifest.Current.LsaIds, StringComparer.Ordinal)
            && v.CsaIds.SequenceEqual(FSTSimAManifest.Current.CsaIds, StringComparer.Ordinal)
            && v.ExactArtifactIdentityRequired && v.PositiveAdmissionEvidenceRequired && v.LifecycleAttachEligibilityRequired && v.CurrentFoundationResourceGrantRequired
            && !v.RuntimeRegistrationMayAuthorizeActivation && !v.RuntimeRegistrationMayAuthorizeDeployment && !v.RuntimeRegistrationMayAuthorizeProduction
            && !v.RuntimeRegistrationMayGrantBusinessAuthority && !v.SilentUpgradeAllowed && !v.ExternalConnectivityActivated && !v.PaperAuthorityGranted && !v.LiveAuthorityGranted;
    }
}
