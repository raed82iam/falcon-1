using System.Reflection;
using System.Runtime.Loader;
using T = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Application;
using G = Falcon.FSATS.TradingGuardian.Application;
using S = Falcon.FSATS.FSTSimA.Application;
using R = Falcon.FSATS.ResourceManagement.Application;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: Falcon.FSATS.CrossBranchFoundationOnboarding.Verifier <Foundation.Admission.dll> <Foundation.ApplicationRuntimeHosting.dll>");
    return 2;
}

var failures = new List<string>();
var checks = 0;
void Check(string name, bool pass)
{
    checks++;
    if (!pass) failures.Add(name);
}

var admissionPath = Path.GetFullPath(args[0]);
var runtimePath = Path.GetFullPath(args[1]);
if (!File.Exists(admissionPath) || !File.Exists(runtimePath))
{
    Console.Error.WriteLine("CROSS-BRANCH FOUNDATION ONBOARDING VERIFIER: FAIL (required Foundation assemblies missing)");
    return 2;
}

var probeDirectories = new[] { Path.GetDirectoryName(admissionPath), Path.GetDirectoryName(runtimePath) }
    .Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>().Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
AssemblyLoadContext.Default.Resolving += (context, name) =>
{
    foreach (var directory in probeDirectories)
    {
        var candidate = Path.Combine(directory, name.Name + ".dll");
        if (File.Exists(candidate)) return context.LoadFromAssemblyPath(candidate);
    }
    return null;
};

var admission = AssemblyLoadContext.Default.LoadFromAssemblyPath(admissionPath);
var runtime = AssemblyLoadContext.Default.LoadFromAssemblyPath(runtimePath);

CheckType(admission, "Foundation.Admission.ApplicationManifest");
CheckProperties(admission, "Foundation.Admission.ApplicationManifest",
    "ManifestId", "ApplicationIdentity", "ApplicationVersion", "ApplicationOwner", "ApplicationPurpose",
    "PackageIdentity", "PackageVersion", "PackageContentOrIntegrityInput", "DeclaredDependencies",
    "RequiredFoundationContracts", "RequiredFoundationSpecifications", "RequiredFoundationServices",
    "ProvidedCapabilities", "IntendedConsumers", "RequestedPermissions", "AuthorityRequests", "SecurityProfile",
    "MinimumResourceRequirements", "ResourceCeilings", "DegradedBehavior", "PersistenceRequirements",
    "CommunicationRequirements", "ConfigurationRequirements", "EvidenceRequirements", "LifecycleBehavior",
    "HealthReportingInterface", "FailureContainmentInterface", "UsesBranchBasedInternalArchitecture",
    "MsaDeclarations", "MajorBranchDeclarations", "LsaDeclarations", "CsaEligibilityPolicy",
    "SelfDevelopmentOriginAndEscalationPath", "GuardianAndProtectionInterface", "RollbackOrCorrectiveActionPlan");
CheckProperties(admission, "Foundation.Admission.AdmissionRequest",
    "AdmissionId", "AdmissionKind", "Identity", "Version", "Owner", "AuthoritySource", "ContractId", "ContractVersion",
    "ManifestId", "Manifest", "ManifestDigest", "ProvenanceId", "ProvenanceContent", "ProvenanceDigest",
    "BootstrapContextId", "BootstrapContextState", "ProviderBoundary", "DecisionSeed");
CheckMethod(admission, "Foundation.Admission.AdmissionControl", "Validate");
CheckMethod(admission, "Foundation.Admission.AdmissionControl", "Evaluate");

CheckEnum(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeSlotState", "Registered", "Active", "Suspended", "Isolated", "Removed");
CheckEnum(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeAuthorityAction", "Activate", "Suspend", "Isolate", "Remove");
CheckProperties(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeArtifactConsumptionBinding",
    "AcceptedForTechnicalConsumption", "ExactArtifactIdentity", "ActivationAuthorized", "DeploymentAuthorized",
    "ProductionAuthorized", "BusinessAuthorityGranted", "SilentUpgradePerformed");
CheckProperties(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeAdmissionBinding",
    "Admitted", "ApplicationIdentity", "ApplicationVersion", "EvidenceIdentity");
CheckProperties(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeLifecycleEligibilityBinding",
    "Eligible", "Kind", "ApplicationIdentity", "CurrentVersion", "TargetVersion", "DecisionIdentity");
CheckProperties(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeResourceGrantBinding",
    "GrantIdentity", "ApplicationIdentity", "ResourceClassIdentity", "Allocation", "Quota", "Ceiling",
    "EffectiveFrom", "EffectiveUntil", "EvidenceObservedAt", "EvidenceIdentity");
CheckProperties(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeRegistrationRequest",
    "RuntimeInstanceId", "ApplicationIdentity", "ApplicationVersion", "ExpectedArtifactExactIdentity", "ArtifactConsumption",
    "Admission", "LifecycleEligibility", "ResourceGrants", "ProvidedCapabilities", "RequiredCapabilities", "ObservedAt");
CheckProperties(runtime, "Foundation.ApplicationRuntimeHosting.RuntimeRegistrationDecision",
    "Registered", "Reason", "RuntimeInstanceId", "ApplicationIdentity", "ApplicationVersion", "DecisionIdentity",
    "ActivationAuthorized", "DeploymentAuthorized", "BusinessAuthorityGranted");
CheckMethod(runtime, "Foundation.ApplicationRuntimeHosting.ApplicationRuntimeHost", "Register");
CheckMethod(runtime, "Foundation.ApplicationRuntimeHosting.ApplicationRuntimeHost", "Activate");

Check("Trading Application declaration compatible", T.TradingFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("FSAPMA Application declaration compatible", P.FSAPMAFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("Guardian Application declaration compatible", G.TradingGuardianFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("FSTSimA Application declaration compatible", S.FSTSimAFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());
Check("APP-RSC Application declaration compatible", R.ResourceManagementFoundationOnboarding.IsApplicationSideReadyForFoundationAdmission());

if (failures.Count > 0)
{
    Console.Error.WriteLine($"CROSS-BRANCH FOUNDATION ONBOARDING VERIFIER: FAIL ({checks - failures.Count}/{checks})");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}

Console.WriteLine($"CROSS-BRANCH FOUNDATION ONBOARDING VERIFIER: PASS ({checks}/{checks})");
Console.WriteLine($"AdmissionAssembly={admission.GetName().Name}");
Console.WriteLine($"RuntimeHostingAssembly={runtime.GetName().Name}");
Console.WriteLine("Scope=EXACT_FOUNDATION_SHAPE_COMPATIBILITY / APPLICATION_DECLARATIONS / NO_ADMISSION_DECISION / NO_REGISTRATION_EXECUTION / NO_ACTIVATION");
return 0;

void CheckType(Assembly assembly, string fullName)
    => Check($"Missing Foundation type: {fullName}", assembly.GetType(fullName, false, false) is not null);

void CheckMethod(Assembly assembly, string fullName, string method)
{
    var type = assembly.GetType(fullName, false, false);
    Check($"Missing Foundation method: {fullName}.{method}", type?.GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(x => x.Name == method) == true);
}

void CheckEnum(Assembly assembly, string fullName, params string[] expected)
{
    var type = assembly.GetType(fullName, false, false);
    if (type is null || !type.IsEnum)
    {
        Check($"Missing Foundation enum: {fullName}", false);
        return;
    }
    var names = Enum.GetNames(type).ToHashSet(StringComparer.Ordinal);
    Check($"Foundation enum mismatch: {fullName}", expected.All(names.Contains));
}

void CheckProperties(Assembly assembly, string fullName, params string[] expected)
{
    var type = assembly.GetType(fullName, false, false);
    if (type is null)
    {
        Check($"Missing Foundation type: {fullName}", false);
        return;
    }
    var names = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
    Check($"Foundation property shape mismatch: {fullName}", expected.All(names.Contains));
}
