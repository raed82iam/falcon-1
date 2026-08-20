namespace Falcon.FSATS.TradingGuardian.Application;

public sealed record FoundationOnboardingDeclaration(
    string AdmissionKind, string ApplicationId, string ApplicationVersion, string ApplicationOwner, string PackageId, string ManifestId,
    string RequiredApplicationContract, string RequiredApplicationContractVersion, string RequiredApplicationSpecification, string RequiredApplicationSpecificationVersion,
    IReadOnlyList<string> RequiredFoundationServices, string BootstrapContextState, string ProviderBoundary,
    string MsaId, IReadOnlyList<string> LsaIds, IReadOnlyList<string> CsaIds,
    bool ExactArtifactIdentityRequired, bool PositiveAdmissionEvidenceRequired, bool LifecycleAttachEligibilityRequired, bool CurrentFoundationResourceGrantRequired,
    bool RuntimeRegistrationMayAuthorizeActivation, bool RuntimeRegistrationMayAuthorizeDeployment, bool RuntimeRegistrationMayAuthorizeProduction,
    bool RuntimeRegistrationMayGrantBusinessAuthority, bool SilentUpgradeAllowed, bool ExternalConnectivityActivated, bool PaperAuthorityGranted, bool LiveAuthorityGranted);

public static class TradingGuardianFoundationOnboarding
{
    public static FoundationOnboardingDeclaration Current { get; } = new(
        "APPLICATION", TradingGuardianManifest.Current.ApplicationId, TradingGuardianManifest.Current.Version, TradingGuardianManifest.Current.Owner,
        TradingGuardianManifest.Current.PackageId, "manifest:fsats:trading-guardian:foundation-admission:v1",
        "CON-023", "1.1", "APP-001", "1.0",
        Array.AsReadOnly(new[] { "ApplicationLifecycle", "Communication", "Evidence", "Resources", "Security", "Persistence" }),
        "DEFINED", "TRADING_PROTECTION_ROUTE_DECLARED_GOVERNED_AND_UNBOUND_UNTIL_SEPARATE_RUNTIME_AUTHORITY",
        TradingGuardianManifest.Current.MsaId, TradingGuardianManifest.Current.LsaIds, TradingGuardianManifest.Current.CsaIds,
        true, true, true, true, false, false, false, false, false, false, false, false);

    public static bool IsApplicationSideReadyForFoundationAdmission()
    {
        var v = Current;
        return v.AdmissionKind == "APPLICATION" && v.ApplicationId == TradingGuardianManifest.Current.ApplicationId
            && v.ApplicationVersion == TradingGuardianManifest.Current.Version && v.ApplicationOwner == TradingGuardianManifest.Current.Owner
            && v.PackageId == TradingGuardianManifest.Current.PackageId && !string.IsNullOrWhiteSpace(v.ManifestId)
            && v.RequiredApplicationContract == "CON-023" && v.RequiredApplicationContractVersion == "1.1"
            && v.RequiredApplicationSpecification == "APP-001" && v.RequiredApplicationSpecificationVersion == "1.0"
            && v.RequiredFoundationServices.Count > 0 && v.BootstrapContextState == "DEFINED" && !string.IsNullOrWhiteSpace(v.ProviderBoundary)
            && v.MsaId == TradingGuardianManifest.Current.MsaId
            && v.LsaIds.SequenceEqual(TradingGuardianManifest.Current.LsaIds, StringComparer.Ordinal)
            && v.CsaIds.SequenceEqual(TradingGuardianManifest.Current.CsaIds, StringComparer.Ordinal)
            && v.ExactArtifactIdentityRequired && v.PositiveAdmissionEvidenceRequired && v.LifecycleAttachEligibilityRequired && v.CurrentFoundationResourceGrantRequired
            && !v.RuntimeRegistrationMayAuthorizeActivation && !v.RuntimeRegistrationMayAuthorizeDeployment && !v.RuntimeRegistrationMayAuthorizeProduction
            && !v.RuntimeRegistrationMayGrantBusinessAuthority && !v.SilentUpgradeAllowed && !v.ExternalConnectivityActivated && !v.PaperAuthorityGranted && !v.LiveAuthorityGranted;
    }
}
