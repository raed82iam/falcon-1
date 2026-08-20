using T = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Application;
using G = Falcon.FSATS.TradingGuardian.Application;
using S = Falcon.FSATS.FSTSimA.Application;
using R = Falcon.FSATS.ResourceManagement.Application;

var checks = new List<(string Name, bool Pass)>();
void Check(string name, bool pass) => checks.Add((name, pass));

Check("Trading onboarding declaration ready", T.TradingFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("FSAPMA onboarding declaration ready", P.FSAPMAFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("Guardian onboarding declaration ready", G.TradingGuardianFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("FSTSimA onboarding declaration ready", S.FSTSimAFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("APP-RSC onboarding declaration ready", R.ResourceManagementFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());

Check("Trading exact awareness topology", T.TradingFoundationOnboarding.Current.LsaIds.Count == 13 && T.TradingFoundationOnboarding.Current.CsaIds.Count == 3);
Check("FSAPMA exact awareness topology", P.FSAPMAFoundationOnboarding.Current.LsaIds.Count == 6 && P.FSAPMAFoundationOnboarding.Current.CsaIds.Count == 1);
Check("Guardian exact awareness topology", G.TradingGuardianFoundationOnboarding.Current.LsaIds.Count == 4 && G.TradingGuardianFoundationOnboarding.Current.CsaIds.Count == 1);
Check("FSTSimA exact awareness topology", S.FSTSimAFoundationOnboarding.Current.LsaIds.Count == 8 && S.FSTSimAFoundationOnboarding.Current.CsaIds.Count == 2);
Check("APP-RSC exact awareness topology", R.ResourceManagementFoundationOnboarding.Current.LsaIds.Count == 3 && R.ResourceManagementFoundationOnboarding.Current.CsaIds.Count == 0);

var declarations = new[]
{
    (T.TradingFoundationOnboarding.Current.ApplicationId, T.TradingFoundationOnboarding.Current.ManifestId,
        T.TradingFoundationOnboarding.Current.RequiredApplicationContract, T.TradingFoundationOnboarding.Current.RequiredApplicationContractVersion,
        T.TradingFoundationOnboarding.Current.RequiredApplicationSpecification, T.TradingFoundationOnboarding.Current.RequiredApplicationSpecificationVersion,
        T.TradingFoundationOnboarding.Current.BootstrapContextState, T.TradingFoundationOnboarding.Current.ExactArtifactIdentityRequired,
        T.TradingFoundationOnboarding.Current.PositiveAdmissionEvidenceRequired, T.TradingFoundationOnboarding.Current.LifecycleAttachEligibilityRequired,
        T.TradingFoundationOnboarding.Current.CurrentFoundationResourceGrantRequired, T.TradingFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeActivation,
        T.TradingFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeDeployment, T.TradingFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeProduction,
        T.TradingFoundationOnboarding.Current.RuntimeRegistrationMayGrantBusinessAuthority, T.TradingFoundationOnboarding.Current.SilentUpgradeAllowed,
        T.TradingFoundationOnboarding.Current.ExternalConnectivityActivated, T.TradingFoundationOnboarding.Current.PaperAuthorityGranted, T.TradingFoundationOnboarding.Current.LiveAuthorityGranted),
    (P.FSAPMAFoundationOnboarding.Current.ApplicationId, P.FSAPMAFoundationOnboarding.Current.ManifestId,
        P.FSAPMAFoundationOnboarding.Current.RequiredApplicationContract, P.FSAPMAFoundationOnboarding.Current.RequiredApplicationContractVersion,
        P.FSAPMAFoundationOnboarding.Current.RequiredApplicationSpecification, P.FSAPMAFoundationOnboarding.Current.RequiredApplicationSpecificationVersion,
        P.FSAPMAFoundationOnboarding.Current.BootstrapContextState, P.FSAPMAFoundationOnboarding.Current.ExactArtifactIdentityRequired,
        P.FSAPMAFoundationOnboarding.Current.PositiveAdmissionEvidenceRequired, P.FSAPMAFoundationOnboarding.Current.LifecycleAttachEligibilityRequired,
        P.FSAPMAFoundationOnboarding.Current.CurrentFoundationResourceGrantRequired, P.FSAPMAFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeActivation,
        P.FSAPMAFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeDeployment, P.FSAPMAFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeProduction,
        P.FSAPMAFoundationOnboarding.Current.RuntimeRegistrationMayGrantBusinessAuthority, P.FSAPMAFoundationOnboarding.Current.SilentUpgradeAllowed,
        P.FSAPMAFoundationOnboarding.Current.ExternalConnectivityActivated, P.FSAPMAFoundationOnboarding.Current.PaperAuthorityGranted, P.FSAPMAFoundationOnboarding.Current.LiveAuthorityGranted),
    (G.TradingGuardianFoundationOnboarding.Current.ApplicationId, G.TradingGuardianFoundationOnboarding.Current.ManifestId,
        G.TradingGuardianFoundationOnboarding.Current.RequiredApplicationContract, G.TradingGuardianFoundationOnboarding.Current.RequiredApplicationContractVersion,
        G.TradingGuardianFoundationOnboarding.Current.RequiredApplicationSpecification, G.TradingGuardianFoundationOnboarding.Current.RequiredApplicationSpecificationVersion,
        G.TradingGuardianFoundationOnboarding.Current.BootstrapContextState, G.TradingGuardianFoundationOnboarding.Current.ExactArtifactIdentityRequired,
        G.TradingGuardianFoundationOnboarding.Current.PositiveAdmissionEvidenceRequired, G.TradingGuardianFoundationOnboarding.Current.LifecycleAttachEligibilityRequired,
        G.TradingGuardianFoundationOnboarding.Current.CurrentFoundationResourceGrantRequired, G.TradingGuardianFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeActivation,
        G.TradingGuardianFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeDeployment, G.TradingGuardianFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeProduction,
        G.TradingGuardianFoundationOnboarding.Current.RuntimeRegistrationMayGrantBusinessAuthority, G.TradingGuardianFoundationOnboarding.Current.SilentUpgradeAllowed,
        G.TradingGuardianFoundationOnboarding.Current.ExternalConnectivityActivated, G.TradingGuardianFoundationOnboarding.Current.PaperAuthorityGranted, G.TradingGuardianFoundationOnboarding.Current.LiveAuthorityGranted),
    (S.FSTSimAFoundationOnboarding.Current.ApplicationId, S.FSTSimAFoundationOnboarding.Current.ManifestId,
        S.FSTSimAFoundationOnboarding.Current.RequiredApplicationContract, S.FSTSimAFoundationOnboarding.Current.RequiredApplicationContractVersion,
        S.FSTSimAFoundationOnboarding.Current.RequiredApplicationSpecification, S.FSTSimAFoundationOnboarding.Current.RequiredApplicationSpecificationVersion,
        S.FSTSimAFoundationOnboarding.Current.BootstrapContextState, S.FSTSimAFoundationOnboarding.Current.ExactArtifactIdentityRequired,
        S.FSTSimAFoundationOnboarding.Current.PositiveAdmissionEvidenceRequired, S.FSTSimAFoundationOnboarding.Current.LifecycleAttachEligibilityRequired,
        S.FSTSimAFoundationOnboarding.Current.CurrentFoundationResourceGrantRequired, S.FSTSimAFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeActivation,
        S.FSTSimAFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeDeployment, S.FSTSimAFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeProduction,
        S.FSTSimAFoundationOnboarding.Current.RuntimeRegistrationMayGrantBusinessAuthority, S.FSTSimAFoundationOnboarding.Current.SilentUpgradeAllowed,
        S.FSTSimAFoundationOnboarding.Current.ExternalConnectivityActivated, S.FSTSimAFoundationOnboarding.Current.PaperAuthorityGranted, S.FSTSimAFoundationOnboarding.Current.LiveAuthorityGranted),
    (R.ResourceManagementFoundationOnboarding.Current.ApplicationId, R.ResourceManagementFoundationOnboarding.Current.ManifestId,
        R.ResourceManagementFoundationOnboarding.Current.RequiredApplicationContract, R.ResourceManagementFoundationOnboarding.Current.RequiredApplicationContractVersion,
        R.ResourceManagementFoundationOnboarding.Current.RequiredApplicationSpecification, R.ResourceManagementFoundationOnboarding.Current.RequiredApplicationSpecificationVersion,
        R.ResourceManagementFoundationOnboarding.Current.BootstrapContextState, R.ResourceManagementFoundationOnboarding.Current.ExactArtifactIdentityRequired,
        R.ResourceManagementFoundationOnboarding.Current.PositiveAdmissionEvidenceRequired, R.ResourceManagementFoundationOnboarding.Current.LifecycleAttachEligibilityRequired,
        R.ResourceManagementFoundationOnboarding.Current.CurrentFoundationResourceGrantRequired, R.ResourceManagementFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeActivation,
        R.ResourceManagementFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeDeployment, R.ResourceManagementFoundationOnboarding.Current.RuntimeRegistrationMayAuthorizeProduction,
        R.ResourceManagementFoundationOnboarding.Current.RuntimeRegistrationMayGrantBusinessAuthority, R.ResourceManagementFoundationOnboarding.Current.SilentUpgradeAllowed,
        R.ResourceManagementFoundationOnboarding.Current.ExternalConnectivityActivated, R.ResourceManagementFoundationOnboarding.Current.PaperAuthorityGranted, R.ResourceManagementFoundationOnboarding.Current.LiveAuthorityGranted)
};

Check("Five unique Application onboarding identities", declarations.Length == 5 && declarations.Select(x => x.ApplicationId).Distinct(StringComparer.Ordinal).Count() == 5);
Check("Five unique immutable manifest identities", declarations.Select(x => x.ManifestId).Distinct(StringComparer.Ordinal).Count() == 5 && declarations.All(x => !string.IsNullOrWhiteSpace(x.ManifestId)));
Check("CON-023 v1.1 required by all", declarations.All(x => x.RequiredApplicationContract == "CON-023" && x.RequiredApplicationContractVersion == "1.1"));
Check("APP-001 v1.0 required by all", declarations.All(x => x.RequiredApplicationSpecification == "APP-001" && x.RequiredApplicationSpecificationVersion == "1.0"));
Check("Defined bootstrap context required by all", declarations.All(x => x.BootstrapContextState == "DEFINED"));
Check("Exact artifact binding required by all", declarations.All(x => x.ExactArtifactIdentityRequired));
Check("Positive admission evidence required by all", declarations.All(x => x.PositiveAdmissionEvidenceRequired));
Check("Lifecycle attach eligibility required by all", declarations.All(x => x.LifecycleAttachEligibilityRequired));
Check("Current Foundation resource grant required by all", declarations.All(x => x.CurrentFoundationResourceGrantRequired));
Check("Runtime registration cannot activate", declarations.All(x => !x.RuntimeRegistrationMayAuthorizeActivation));
Check("Runtime registration cannot deploy", declarations.All(x => !x.RuntimeRegistrationMayAuthorizeDeployment));
Check("Runtime registration cannot authorize production", declarations.All(x => !x.RuntimeRegistrationMayAuthorizeProduction));
Check("Runtime registration cannot grant business authority", declarations.All(x => !x.RuntimeRegistrationMayGrantBusinessAuthority));
Check("Silent upgrade forbidden", declarations.All(x => !x.SilentUpgradeAllowed));
Check("External connectivity remains disabled", declarations.All(x => !x.ExternalConnectivityActivated));
Check("Paper authority remains not granted", declarations.All(x => !x.PaperAuthorityGranted));
Check("Live authority remains not granted", declarations.All(x => !x.LiveAuthorityGranted));

var failed = checks.Where(x => !x.Pass).ToArray();
if (failed.Length > 0)
{
    Console.Error.WriteLine($"FOUNDATION ONBOARDING VERIFIER: FAIL ({checks.Count - failed.Length}/{checks.Count})");
    foreach (var failure in failed) Console.Error.WriteLine(" - " + failure.Name);
    return 1;
}

Console.WriteLine($"FOUNDATION ONBOARDING VERIFIER: PASS ({checks.Count}/{checks.Count})");
Console.WriteLine("Scope=APPLICATION_SIDE_ONBOARDING_DECLARATIONS / NO_ADMISSION_DECISION / NO_RUNTIME_ACTIVATION / NO_EXTERNAL_CONNECTIVITY");
return 0;
