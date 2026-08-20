using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Text;
using Foundation.Admission;
using Foundation.ContractRegistry;

var failures = new List<string>();

void ExpectPass(string id, AdmissionValidationResult result)
{
    if (!result.Success)
    {
        failures.Add($"{id} expected PASS but failed: {result.Message}");
    }
}

void ExpectFail(string id, AdmissionValidationResult result)
{
    if (result.Success)
    {
        failures.Add($"{id} expected FAIL but passed");
    }
}

void ExpectEqual<T>(string id, T actual, T expected) where T : IEquatable<T>
{
    if (!actual.Equals(expected))
    {
        failures.Add($"{id} expected '{expected}' but found '{actual}'");
    }
}

void ExpectConstructorFail(string id, Func<AdmissionControl> factory)
{
    try
    {
        _ = factory();
        failures.Add($"{id} expected FAIL but constructor succeeded");
    }
    catch
    {
    }
}

var inMemoryProvider = new InMemoryAdmissionBaselineProvider(BaselineFactory.CreateCanonicalBaseline());
var control = new AdmissionControl(inMemoryProvider);

var validManifest = CreateManifest(
    manifestId: "manifest-app-1",
    applicationIdentity: "app-1",
    applicationVersion: "1.0",
    applicationOwner: "Example Application Owner",
    applicationPurpose: "governed application admission",
    packageIdentity: "pkg-app-1",
    packageVersion: "1.0",
    packageContentOrIntegrityInput: "package-content-app-1",
    dependencyId: "CON-023",
    dependencyVersions: new[] { "1.1" },
    requiredContracts: new[]
    {
        new FoundationRequirement("CON-023", "1.1", "Falcon Application Authority", "CON-000 / CON-023")
    },
    requiredSpecifications: new[]
    {
        new FoundationRequirement("APP-001", "1.1", "Falcon Application Authority", "Falcon Application Authority")
    },
    requiredServices: new[]
    {
        new FoundationServiceRequirement("Service Catalog", "1.0", "registration")
    },
    providedCapabilities: new[] { "admission" },
    intendedConsumers: new[] { "foundation-runtime" },
    requestedPermissions: new[]
    {
        new PermissionDeclaration("admission.request", "governed entry", "required for controlled admission")
    },
    authorityRequests: new[]
    {
        new AuthorityRequest("admission.authority", "controlled admission", "requested authority to submit governed admission")
    },
    securityProfile: new SecurityProfile("standard", "confidential", "bounded"),
    minimumResourceRequirements: new ResourceRequirements("256MiB", "0.25 CPU", "128MiB", "offline"),
    resourceCeilings: new ResourceRequirements("1GiB", "1 CPU", "1GiB", "offline"),
    degradedBehavior: "reject on degraded prerequisite failure",
    persistenceRequirements: "no hidden persistence",
    communicationRequirements: "controlled outbound only",
    configurationRequirements: "explicit governed configuration",
    evidenceRequirements: "raw evidence retained",
    lifecycleBehavior: new LifecycleBehavior(
        "install governed package",
        "validate governed package",
        "register governed package",
        "admit governed package",
        "activate governed package",
        "update governed package",
        "suspend governed package",
        "recover governed package",
        "replace governed package",
        "remove governed package"),
    healthReportingInterface: "governed health reporting",
    failureContainmentInterface: "bounded failure containment",
    usesBranchBasedInternalArchitecture: true,
    msaDeclarations: new[]
    {
        new MsaDeclaration("MSA-1", "Example Application Owner", "main application surface")
    },
    majorBranchDeclarations: new[]
    {
        new MajorBranchDeclaration("branch-a", "primary runtime branch", "LSA-A"),
        new MajorBranchDeclaration("branch-b", "secondary runtime branch", "LSA-B")
    },
    lsaDeclarations: new[]
    {
        new LsaDeclaration("branch-a", "LSA-A", "branch-a responsibility"),
        new LsaDeclaration("branch-b", "LSA-B", "branch-b responsibility")
    },
    csaEligibilityPolicy: "CSA eligible only for explicit intelligent components",
    selfDevelopmentOriginAndEscalationPath: "application owner to governed foundation review",
    guardianAndProtectionInterface: "guardian interface declared",
    rollbackOrCorrectiveActionPlan: "rollback to last accepted governed package");

var validProvenance = "provenance-content-app-1";
var validApplication = CreateAdmission(
    admissionId: "adm-app-1",
    admissionKind: "APPLICATION",
    identity: "app-1",
    version: "1.0",
    owner: "Example Application Owner",
    authoritySource: "CON-000 / CON-023",
    contractId: "CON-023",
    contractVersion: "1.1",
    manifest: validManifest,
    provenanceContent: validProvenance,
    bootstrapContextId: "ctx-1",
    providerBoundary: "boundary-ok",
    decisionSeed: "seed-1");

var validPluginManifest = validManifest with
{
    ManifestId = "manifest-plugin-1",
    ApplicationIdentity = "plugin-1",
    PackageIdentity = "pkg-plugin-1",
    PackageContentOrIntegrityInput = "package-content-plugin-1",
    ProvidedCapabilities = new[] { "plugin-admission" },
    IntendedConsumers = new[] { "foundation-plugin-host" }
};

var validPlugin = CreateAdmission(
    admissionId: "adm-plugin-1",
    admissionKind: "PLUG-IN",
    identity: "plugin-1",
    version: "1.0",
    owner: "Example Application Owner",
    authoritySource: "CON-000 / CON-023",
    contractId: "CON-023",
    contractVersion: "1.1",
    manifest: validPluginManifest,
    provenanceContent: "provenance-content-plugin-1",
    bootstrapContextId: "ctx-2",
    providerBoundary: "boundary-ok",
    decisionSeed: "seed-2");

var goldenManifest = CreateManifest(
    manifestId: "golden-manifest",
    applicationIdentity: "golden-application",
    applicationVersion: "1.0",
    applicationOwner: "Golden Owner",
    applicationPurpose: "governed admission sample",
    packageIdentity: "golden-package",
    packageVersion: "1.0",
    packageContentOrIntegrityInput: "golden-package-content",
    dependencyId: "CON-023",
    dependencyVersions: new[] { "1.1" },
    requiredContracts: new[]
    {
        new FoundationRequirement("CON-023", "1.1", "Falcon Application Authority", "CON-000 / CON-023")
    },
    requiredSpecifications: new[]
    {
        new FoundationRequirement("APP-001", "1.1", "Falcon Application Authority", "Falcon Application Authority")
    },
    requiredServices: new[]
    {
        new FoundationServiceRequirement("Service Catalog", "1.0", "registration")
    },
    providedCapabilities: new[] { "admission" },
    intendedConsumers: new[] { "foundation-runtime" },
    requestedPermissions: new[]
    {
        new PermissionDeclaration("admission.request", "governed entry", "required for controlled admission")
    },
    authorityRequests: new[]
    {
        new AuthorityRequest("admission.authority", "controlled admission", "requested authority to submit governed admission")
    },
    securityProfile: new SecurityProfile("standard", "confidential", "bounded"),
    minimumResourceRequirements: new ResourceRequirements("256MiB", "0.25 CPU", "128MiB", "offline"),
    resourceCeilings: new ResourceRequirements("1GiB", "1 CPU", "1GiB", "offline"),
    degradedBehavior: "reject on degraded prerequisite failure",
    persistenceRequirements: "no hidden persistence",
    communicationRequirements: "controlled outbound only",
    configurationRequirements: "explicit governed configuration",
    evidenceRequirements: "raw evidence retained",
    lifecycleBehavior: new LifecycleBehavior(
        "install governed package",
        "validate governed package",
        "register governed package",
        "admit governed package",
        "activate governed package",
        "update governed package",
        "suspend governed package",
        "recover governed package",
        "replace governed package",
        "remove governed package"),
    healthReportingInterface: "governed health reporting",
    failureContainmentInterface: "bounded failure containment",
    usesBranchBasedInternalArchitecture: true,
    msaDeclarations: new[]
    {
        new MsaDeclaration("MSA-1", "Golden Owner", "main application surface")
    },
    majorBranchDeclarations: new[]
    {
        new MajorBranchDeclaration("branch-a", "primary runtime branch", "LSA-A")
    },
    lsaDeclarations: new[]
    {
        new LsaDeclaration("branch-a", "LSA-A", "branch-a responsibility")
    },
    csaEligibilityPolicy: "CSA eligible only for explicit intelligent components",
    selfDevelopmentOriginAndEscalationPath: "application owner to governed foundation review",
    guardianAndProtectionInterface: "guardian interface declared",
    rollbackOrCorrectiveActionPlan: "rollback to last accepted governed package");

var expectedGoldenCanonicalText = string.Join('\n', new[]
{
    "ManifestId=golden-manifest",
    "ApplicationIdentity=golden-application",
    "ApplicationVersion=1.0",
    "ApplicationOwner=Golden Owner",
    "ApplicationPurpose=governed admission sample",
    "PackageIdentity=golden-package",
    "PackageVersion=1.0",
    "PackageContentOrIntegrityInput=golden-package-content",
    "DeclaredDependencies=CON-023\\|1.1",
    "RequiredFoundationContracts=CON-023\\|1.1\\|Falcon Application Authority\\|CON-000 / CON-023",
    "RequiredFoundationSpecifications=APP-001\\|1.1\\|Falcon Application Authority\\|Falcon Application Authority",
    "RequiredFoundationServices=Service Catalog\\|1.0\\|registration",
    "ProvidedCapabilities=admission",
    "IntendedConsumers=foundation-runtime",
    "RequestedPermissions=admission.request\\|governed entry\\|required for controlled admission",
    "AuthorityRequests=admission.authority\\|controlled admission\\|requested authority to submit governed admission",
    "SecurityProfile=standard\\|confidential\\|bounded",
    "MinimumResourceRequirements=256MiB\\|0.25 CPU\\|128MiB\\|offline",
    "ResourceCeilings=1GiB\\|1 CPU\\|1GiB\\|offline",
    "DegradedBehavior=reject on degraded prerequisite failure",
    "PersistenceRequirements=no hidden persistence",
    "CommunicationRequirements=controlled outbound only",
    "ConfigurationRequirements=explicit governed configuration",
    "EvidenceRequirements=raw evidence retained",
    "LifecycleBehavior=install governed package\\|validate governed package\\|register governed package\\|admit governed package\\|activate governed package\\|update governed package\\|suspend governed package\\|recover governed package\\|replace governed package\\|remove governed package",
    "HealthReportingInterface=governed health reporting",
    "FailureContainmentInterface=bounded failure containment",
    "UsesBranchBasedInternalArchitecture=true",
    "MsaDeclarations=MSA-1\\|Golden Owner\\|main application surface",
    "MajorBranchDeclarations=branch-a\\|primary runtime branch\\|LSA-A",
    "LsaDeclarations=branch-a\\|LSA-A\\|branch-a responsibility",
    "CsaEligibilityPolicy=CSA eligible only for explicit intelligent components",
    "SelfDevelopmentOriginAndEscalationPath=application owner to governed foundation review",
    "GuardianAndProtectionInterface=guardian interface declared",
    "RollbackOrCorrectiveActionPlan=rollback to last accepted governed package"
}) + '\n';

var goldenCanonicalText = goldenManifest.CanonicalText();
var goldenBytes = Encoding.UTF8.GetBytes(goldenCanonicalText);
var goldenCrlfText = goldenCanonicalText.Replace("\n", "\r\n", StringComparison.Ordinal);
var goldenCrlfDigest = ComputeSha256(goldenCrlfText);

ExpectEqual("golden-canonical-text", goldenCanonicalText, expectedGoldenCanonicalText);
ExpectEqual("golden-byte-length", goldenBytes.Length, 2139);
ExpectEqual("golden-digest", goldenManifest.ComputeDigest(), "02D825E1D1F9FD02DC7B5BEF726EEA52EFA880136B05BEC912360D008C7B3104");
ExpectEqual("golden-repeated-serialization", goldenManifest.CanonicalText(), goldenCanonicalText);

if (goldenCanonicalText.Contains('\r'))
{
    failures.Add("golden LF serialization contains CR bytes");
}

if (string.Equals(goldenCrlfDigest, "02D825E1D1F9FD02DC7B5BEF726EEA52EFA880136B05BEC912360D008C7B3104", StringComparison.OrdinalIgnoreCase))
{
    failures.Add("CRLF serialization unexpectedly matched canonical digest");
}

ExpectPass("application-admission", control.Validate(validApplication));
ExpectPass("plug-in-admission", control.Validate(validPlugin));
var arbitraryOwnerManifest = validApplication.Manifest with { ApplicationOwner = "Independent App Owner" };
ExpectPass("arbitrary-owner", control.Validate(validApplication with { Owner = "Independent App Owner", Manifest = arbitraryOwnerManifest, ManifestDigest = arbitraryOwnerManifest.ComputeDigest() }));
ExpectFail("authority-source-mismatch", control.Validate(validApplication with { AuthoritySource = "Independent Authority" }));

ExpectFail("missing-mandatory-group", control.Validate(validApplication with { Manifest = validManifest with { RequiredFoundationServices = Array.Empty<FoundationServiceRequirement>() } }));
ExpectFail("missing-msa", control.Validate(validApplication with { Manifest = validManifest with { MsaDeclarations = Array.Empty<MsaDeclaration>() } }));
ExpectFail("multiple-msa", control.Validate(validApplication with { Manifest = validManifest with { MsaDeclarations = new[] { new MsaDeclaration("MSA-1", "Example Application Owner", "main application surface"), new MsaDeclaration("MSA-2", "Example Application Owner", "secondary application surface") } } }));
ExpectFail("duplicate-major-branch", control.Validate(validApplication with { Manifest = validManifest with { MajorBranchDeclarations = new[] { new MajorBranchDeclaration("branch-a", "primary runtime branch", "LSA-A"), new MajorBranchDeclaration("branch-a", "secondary runtime branch", "LSA-A") } } }));
ExpectFail("empty-major-branch-name", control.Validate(validApplication with { Manifest = validManifest with { MajorBranchDeclarations = new[] { new MajorBranchDeclaration("", "primary runtime branch", "LSA-A") } } }));
ExpectFail("empty-major-branch-purpose", control.Validate(validApplication with { Manifest = validManifest with { MajorBranchDeclarations = new[] { new MajorBranchDeclaration("branch-a", "", "LSA-A") } } }));
ExpectFail("empty-major-branch-responsible-lsa", control.Validate(validApplication with { Manifest = validManifest with { MajorBranchDeclarations = new[] { new MajorBranchDeclaration("branch-a", "primary runtime branch", "") } } }));
ExpectFail("branch-without-lsa", control.Validate(validApplication with { Manifest = validManifest with { LsaDeclarations = new[] { new LsaDeclaration("branch-a", "LSA-A", "branch-a responsibility") } } }));
ExpectFail("branch-with-multiple-lsas", control.Validate(validApplication with { Manifest = validManifest with { LsaDeclarations = new[] { new LsaDeclaration("branch-a", "LSA-A", "branch-a responsibility"), new LsaDeclaration("branch-a", "LSA-A-2", "branch-a responsibility") } } }));
ExpectFail("lsa-for-undeclared-branch", control.Validate(validApplication with { Manifest = validManifest with { LsaDeclarations = new[] { new LsaDeclaration("branch-a", "LSA-A", "branch-a responsibility"), new LsaDeclaration("branch-z", "LSA-Z", "undeclared branch") } } }));
ExpectFail("empty-lsa-branch-name", control.Validate(validApplication with { Manifest = validManifest with { LsaDeclarations = new[] { new LsaDeclaration("", "LSA-A", "branch-a responsibility") } } }));
ExpectFail("empty-lsa-responsible-lsa", control.Validate(validApplication with { Manifest = validManifest with { LsaDeclarations = new[] { new LsaDeclaration("branch-a", "", "branch-a responsibility") } } }));
ExpectFail("empty-lsa-scope", control.Validate(validApplication with { Manifest = validManifest with { LsaDeclarations = new[] { new LsaDeclaration("branch-a", "LSA-A", "") } } }));
ExpectFail("no-lsa-for-major-branch", control.Validate(validApplication with { Manifest = validManifest with { LsaDeclarations = Array.Empty<LsaDeclaration>() } }));
ExpectFail("unknown-dependency", control.Validate(validApplication with { Manifest = validManifest with { DeclaredDependencies = new[] { new DependencyDeclaration("CON-999", new[] { "1.0" }) } } }));
ExpectFail("unsupported-dependency-version", control.Validate(validApplication with { Manifest = validManifest with { DeclaredDependencies = new[] { new DependencyDeclaration("CON-023", new[] { "9.9" }) } } }));
ExpectFail("empty-dependency-list", control.Validate(validApplication with { Manifest = validManifest with { DeclaredDependencies = Array.Empty<DependencyDeclaration>() } }));
ExpectFail("blank-dependency-identity", control.Validate(validApplication with { Manifest = validManifest with { DeclaredDependencies = new[] { new DependencyDeclaration("", new[] { "1.1" }) } } }));
ExpectFail("empty-dependency-version-list", control.Validate(validApplication with { Manifest = validManifest with { DeclaredDependencies = new[] { new DependencyDeclaration("CON-023", Array.Empty<string>()) } } }));
ExpectFail("blank-dependency-version", control.Validate(validApplication with { Manifest = validManifest with { DeclaredDependencies = new[] { new DependencyDeclaration("CON-023", new[] { "" }) } } }));
ExpectFail("inactive-contract", control.Validate(validApplication with { ContractVersion = "1.0" }));
ExpectFail("unregistered-contract", control.Validate(validApplication with { ContractId = "CON-999" }));
ExpectFail("missing-permission", control.Validate(validApplication with { Manifest = validManifest with { RequestedPermissions = Array.Empty<PermissionDeclaration>() } }));
ExpectFail("missing-lifecycle", control.Validate(validApplication with { Manifest = validManifest with { LifecycleBehavior = validManifest.LifecycleBehavior with { Removal = "" } } }));
ExpectFail("missing-guardian", control.Validate(validApplication with { Manifest = validManifest with { GuardianAndProtectionInterface = "" } }));
ExpectFail("missing-rollback", control.Validate(validApplication with { Manifest = validManifest with { RollbackOrCorrectiveActionPlan = "" } }));

ExpectConstructorFail("duplicate-effective-contract", () => new AdmissionControl(new InMemoryAdmissionBaselineProvider(
    BaselineFactory.CreateCanonicalBaseline() with
    {
        EffectiveContracts = BaselineFactory.CreateCanonicalBaseline().EffectiveContracts.Concat(new[]
        {
            BaselineFactory.CreateCanonicalBaseline().EffectiveContracts.Single(entry => entry.ContractId == "CON-023")
        }).ToList()
    })));

ExpectConstructorFail("conflicting-effective-contract-version", () => new AdmissionControl(new InMemoryAdmissionBaselineProvider(
    BaselineFactory.CreateCanonicalBaseline() with
    {
        EffectiveContracts = BaselineFactory.CreateCanonicalBaseline().EffectiveContracts.Select(entry =>
            entry.ContractId == "CON-023"
                ? entry with { Version = "9.9" }
                : entry).ToList()
    })));

ExpectConstructorFail("malformed-effective-contract", () => new AdmissionControl(new InMemoryAdmissionBaselineProvider(
    BaselineFactory.CreateCanonicalBaseline() with
    {
        EffectiveContracts = BaselineFactory.CreateCanonicalBaseline().EffectiveContracts.Select(entry =>
            entry.ContractId == "CON-023"
                ? entry with { Owner = "" }
                : entry).ToList()
    })));

ExpectConstructorFail("incomplete-registry-baseline", () => new AdmissionControl(new InMemoryAdmissionBaselineProvider(
    BaselineFactory.CreateCanonicalBaseline() with
    {
        EffectiveContracts = BaselineFactory.CreateCanonicalBaseline().EffectiveContracts.Where(entry => entry.ContractId != "CON-021").ToList()
    })));

var validDigest = validManifest.ComputeDigest();
ExpectPass("correct-manifest-digest", control.Validate(validApplication with { ManifestDigest = validDigest }));
ExpectFail("fabricated-manifest-digest", control.Validate(validApplication with { ManifestDigest = new string('F', 64) }));
ExpectFail("changed-manifest-content-old-digest", control.Validate(validApplication with { Manifest = validManifest with { ApplicationPurpose = "changed purpose" }, ManifestDigest = validDigest }));

var correctProvenanceDigest = ComputeSha256(validProvenance);
ExpectPass("correct-provenance-digest", control.Validate(validApplication with { ProvenanceDigest = correctProvenanceDigest }));
ExpectFail("fabricated-provenance-digest", control.Validate(validApplication with { ProvenanceDigest = new string('A', 64), ProvenanceContent = validProvenance }));
ExpectFail("changed-provenance-content-old-digest", control.Validate(validApplication with { ProvenanceContent = "changed provenance content", ProvenanceDigest = correctProvenanceDigest }));

var firstAdmission = control.Evaluate(validApplication with { ManifestDigest = validDigest, ProvenanceDigest = correctProvenanceDigest });
var duplicateAdmission = control.Evaluate(validApplication with { ManifestDigest = validDigest, ProvenanceDigest = correctProvenanceDigest });
if (!string.Equals(firstAdmission.Decision, "ADMITTED", StringComparison.Ordinal))
{
    failures.Add("deterministic admission seed failed to admit valid application");
}

if (!string.Equals(duplicateAdmission.Decision, "REJECTED", StringComparison.Ordinal))
{
    failures.Add("duplicate admission identity not rejected");
}

var duplicateSubject = control.Evaluate(validApplication with { AdmissionId = "adm-app-2", ManifestDigest = validDigest, ProvenanceDigest = correctProvenanceDigest });
if (!string.Equals(duplicateSubject.Decision, "REJECTED", StringComparison.Ordinal))
{
    failures.Add("duplicate application or plug-in identity not rejected");
}

var firstPluginAdmission = control.Evaluate(validPlugin with { ManifestDigest = validPlugin.Manifest.ComputeDigest(), ProvenanceDigest = ComputeSha256("provenance-content-plugin-1") });
if (!string.Equals(firstPluginAdmission.Decision, "ADMITTED", StringComparison.Ordinal))
{
    failures.Add("deterministic admission seed failed to admit valid plug-in");
}

var duplicatePluginSubject = control.Evaluate(validPlugin with { AdmissionId = "adm-plugin-2", ManifestDigest = validPlugin.Manifest.ComputeDigest(), ProvenanceDigest = ComputeSha256("provenance-content-plugin-1") });
if (!string.Equals(duplicatePluginSubject.Decision, "REJECTED", StringComparison.Ordinal))
{
    failures.Add("duplicate plug-in identity not rejected");
}

var changedPluginManifest = validPlugin.Manifest with { ApplicationOwner = "Other Owner" };
var changeAttempt = control.Evaluate(validPlugin with { AdmissionId = "adm-plugin-3", Owner = "Other Owner", Manifest = changedPluginManifest, ManifestDigest = changedPluginManifest.ComputeDigest(), ProvenanceDigest = ComputeSha256("provenance-content-plugin-1") });
if (!string.Equals(changeAttempt.Decision, "REJECTED", StringComparison.Ordinal))
{
    failures.Add("unauthorized replacement not rejected");
}
if (!string.Equals(changeAttempt.ReasonCode, "duplicate application or plug-in identity", StringComparison.Ordinal))
{
    failures.Add($"unauthorized replacement rejected for unexpected reason: {changeAttempt.ReasonCode}");
}

ExpectFail("provider-boundary-bypass", control.Validate(validApplication with { ProviderBoundary = "provider-bypass" }));
ExpectFail("invalid-bootstrap", control.Validate(validApplication with { BootstrapContextState = "AMBIGUOUS" }));

var repeatedDigestOne = validManifest.ComputeDigest();
var repeatedDigestTwo = validManifest.ComputeDigest();
if (!string.Equals(repeatedDigestOne, repeatedDigestTwo, StringComparison.Ordinal))
{
    failures.Add("identical manifest did not produce identical digest");
}

var fileRoot = Path.Combine(Path.GetTempPath(), "falcon-stage3-wp02-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(fileRoot);
File.WriteAllText(Path.Combine(fileRoot, "admission-baseline.txt"), "mode=canonical");

var fileProvider = new FileBackedAdmissionBaselineProvider(fileRoot);
var fileControl = new AdmissionControl(fileProvider);
ExpectPass("temp-directory-admission", fileControl.Validate(validApplication with { ManifestDigest = validDigest, ProvenanceDigest = correctProvenanceDigest }));

var alternateRoot = Path.Combine(Path.GetTempPath(), "falcon-stage3-wp02-alt-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(alternateRoot);
File.WriteAllText(Path.Combine(alternateRoot, "admission-baseline.txt"), "mode=canonical");
var alternateControl = new AdmissionControl(new FileBackedAdmissionBaselineProvider(alternateRoot));
ExpectPass("alternate-root-admission", alternateControl.Validate(validApplication with { ManifestDigest = validDigest, ProvenanceDigest = correctProvenanceDigest }));

var originalCwd = Directory.GetCurrentDirectory();
var unrelatedCwd = Path.Combine(Path.GetTempPath(), "falcon-stage3-wp02-cwd-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(unrelatedCwd);
Directory.SetCurrentDirectory(unrelatedCwd);
try
{
    ExpectPass("cwd-independent-admission", new AdmissionControl(new FileBackedAdmissionBaselineProvider(fileRoot)).Validate(validApplication with { ManifestDigest = validDigest, ProvenanceDigest = correctProvenanceDigest }));
}
finally
{
    Directory.SetCurrentDirectory(originalCwd);
}

ExpectFail("missing-baseline-config", TryCreateProviderResult(null));
ExpectFail("invalid-baseline-path", TryCreateProviderResult(Path.Combine(Path.GetTempPath(), "does-not-exist-" + Guid.NewGuid().ToString("N"))));

var inaccessibleRoot = Path.Combine(Path.GetTempPath(), "falcon-stage3-wp02-locked-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(inaccessibleRoot);
var lockedPath = Path.Combine(inaccessibleRoot, "admission-baseline.txt");
using (var lockedStream = new FileStream(lockedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
{
    lockedStream.WriteByte(1);
    lockedStream.Flush();
    ExpectFail("inaccessible-baseline-source", TryCreateProviderResult(inaccessibleRoot));
}

var incompleteRoot = Path.Combine(Path.GetTempPath(), "falcon-stage3-wp02-incomplete-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(incompleteRoot);
File.WriteAllText(Path.Combine(incompleteRoot, "admission-baseline.txt"), "mode=incomplete");
ExpectFail("incomplete-baseline", TryCreateProviderResult(incompleteRoot));

var productionAssembly = typeof(AdmissionControl).Assembly.Location;
var productionText = File.ReadAllText(productionAssembly);
if (productionText.Contains(@"C:\Falcon\Falcon1", StringComparison.Ordinal))
{
    failures.Add("production assembly contains forbidden literal path");
}

var productionSourcePathText = typeof(AdmissionControl).FullName ?? string.Empty;
if (productionSourcePathText.Contains("docs", StringComparison.OrdinalIgnoreCase))
{
    failures.Add("AdmissionControl runtime dependency mentions docs");
}

if (inMemoryProvider.GetCurrentBaseline().EffectiveContracts.Count != 22)
{
    failures.Add($"registry count mismatch: {inMemoryProvider.GetCurrentBaseline().EffectiveContracts.Count}");
}


ExpectFail("null-admission-validation", control.Validate(null));
ExpectEqual("null-admission-evaluation", control.Evaluate(null).Decision, "REJECTED");

var replayControl = new AdmissionControl(inMemoryProvider);
var rejectedReplay = validApplication with
{
    AdmissionId = "adm-first-observation-replay",
    ProviderBoundary = "unapproved-boundary"
};
ExpectEqual("rejected-admission-first-observation", replayControl.Evaluate(rejectedReplay).Decision, "REJECTED");
var replayAttempt = replayControl.Evaluate(validApplication with { AdmissionId = "adm-first-observation-replay" });
ExpectEqual("rejected-admission-id-reserved", replayAttempt.ReasonCode, "duplicate admission identity");

var concurrentControl = new AdmissionControl(inMemoryProvider);
var concurrentDecisions = new ConcurrentBag<AdmissionDecision>();
var concurrentRequest = validApplication with { AdmissionId = "adm-concurrent-first-observation" };
Parallel.For(0, 32, _ => concurrentDecisions.Add(concurrentControl.Evaluate(concurrentRequest)));
if (concurrentDecisions.Count(decision => decision.Decision == "ADMITTED") != 1)
{
    failures.Add("concurrent admission replay produced more than one admitted decision");
}

var collisionControl = new AdmissionControl(inMemoryProvider);
var collisionManifestOne = validManifest with
{
    ManifestId = "manifest-subject-collision-one",
    ApplicationIdentity = "subject:a",
    ApplicationVersion = "b"
};
var collisionManifestTwo = validManifest with
{
    ManifestId = "manifest-subject-collision-two",
    ApplicationIdentity = "subject",
    ApplicationVersion = "a:b"
};
var collisionRequestOne = CreateAdmission(
    "adm-subject-collision-one", "APPLICATION", "subject:a", "b",
    "Example Application Owner", "CON-000 / CON-023", "CON-023", "1.1",
    collisionManifestOne, "prov-collision-one", "ctx-collision-one", "boundary-ok", "seed-collision-one");
var collisionRequestTwo = CreateAdmission(
    "adm-subject-collision-two", "APPLICATION", "subject", "a:b",
    "Example Application Owner", "CON-000 / CON-023", "CON-023", "1.1",
    collisionManifestTwo, "prov-collision-two", "ctx-collision-two", "boundary-ok", "seed-collision-two");
ExpectEqual("structured-subject-key-one", collisionControl.Evaluate(collisionRequestOne).Decision, "ADMITTED");
ExpectEqual("structured-subject-key-two", collisionControl.Evaluate(collisionRequestTwo).Decision, "ADMITTED");

var duplicateDeclarationManifest = validManifest with
{
    ManifestId = "manifest-duplicate-declaration",
    ProvidedCapabilities = new[] { "admission", "admission" }
};
var duplicateDeclarationRequest = CreateAdmission(
    "adm-duplicate-declaration", "APPLICATION", "app-1", "1.0",
    "Example Application Owner", "CON-000 / CON-023", "CON-023", "1.1",
    duplicateDeclarationManifest, "prov-duplicate-declaration", "ctx-duplicate", "boundary-ok", "seed-duplicate");
ExpectFail("duplicate-semantic-declaration", control.Validate(duplicateDeclarationRequest));

var seedControlOne = new AdmissionControl(inMemoryProvider);
var seedControlTwo = new AdmissionControl(inMemoryProvider);
var seedRequestOne = validApplication with { AdmissionId = "adm-seed-binding", DecisionSeed = "seed-alpha" };
var seedRequestTwo = validApplication with { AdmissionId = "adm-seed-binding", DecisionSeed = "seed-beta" };
var seedEvidenceOne = seedControlOne.Evaluate(seedRequestOne).EvidenceId;
var seedEvidenceTwo = seedControlTwo.Evaluate(seedRequestTwo).EvidenceId;
if (string.Equals(seedEvidenceOne, seedEvidenceTwo, StringComparison.Ordinal))
{
    failures.Add("decision seed is not bound to admission evidence identity");
}

if (failures.Count > 0)
{
    Console.Error.WriteLine("Stage 3 WP-02: FAIL");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

Console.WriteLine("Stage 3 WP-02: PASS");
Console.WriteLine("Application and plug-in admissions admitted valid requests and rejected malformed, unauthorized, conflicting, and invalid cases closed.");
Console.WriteLine("Admission decisions are deterministic and reproducible.");
Console.WriteLine("Portable baseline providers succeed from in-memory and temporary-directory configurations and fail closed for missing, invalid, inaccessible, and incomplete configurations.");
return 0;

static AdmissionValidationResult TryCreateProviderResult(string? rootPath)
{
    try
    {
        var provider = new FileBackedAdmissionBaselineProvider(rootPath);
        _ = provider.GetCurrentBaseline();
        return AdmissionValidationResult.Pass("provider created");
    }
    catch
    {
        return AdmissionValidationResult.Fail("provider creation failed");
    }
}

static string ComputeSha256(string content)
    => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(content)));

static AdmissionRequest CreateAdmission(
    string admissionId,
    string admissionKind,
    string identity,
    string version,
    string owner,
    string authoritySource,
    string contractId,
    string contractVersion,
    ApplicationManifest manifest,
    string provenanceContent,
    string bootstrapContextId,
    string providerBoundary,
    string decisionSeed)
    => new(
        admissionId,
        admissionKind,
        identity,
        version,
        owner,
        authoritySource,
        contractId,
        contractVersion,
        manifest.ManifestId,
        manifest,
        manifest.ComputeDigest(),
        $"prov-{identity}",
        provenanceContent,
        ComputeSha256(provenanceContent),
        bootstrapContextId,
        "DEFINED",
        providerBoundary,
        decisionSeed);

static ApplicationManifest CreateManifest(
    string manifestId,
    string applicationIdentity,
    string applicationVersion,
    string applicationOwner,
    string applicationPurpose,
    string packageIdentity,
    string packageVersion,
    string packageContentOrIntegrityInput,
    string dependencyId,
    IReadOnlyList<string> dependencyVersions,
    IReadOnlyList<FoundationRequirement> requiredContracts,
    IReadOnlyList<FoundationRequirement> requiredSpecifications,
    IReadOnlyList<FoundationServiceRequirement> requiredServices,
    IReadOnlyList<string> providedCapabilities,
    IReadOnlyList<string> intendedConsumers,
    IReadOnlyList<PermissionDeclaration> requestedPermissions,
    IReadOnlyList<AuthorityRequest> authorityRequests,
    SecurityProfile securityProfile,
    ResourceRequirements minimumResourceRequirements,
    ResourceRequirements resourceCeilings,
    string degradedBehavior,
    string persistenceRequirements,
    string communicationRequirements,
    string configurationRequirements,
    string evidenceRequirements,
    LifecycleBehavior lifecycleBehavior,
    string healthReportingInterface,
    string failureContainmentInterface,
    bool usesBranchBasedInternalArchitecture,
    IReadOnlyList<MsaDeclaration> msaDeclarations,
    IReadOnlyList<MajorBranchDeclaration> majorBranchDeclarations,
    IReadOnlyList<LsaDeclaration> lsaDeclarations,
    string csaEligibilityPolicy,
    string selfDevelopmentOriginAndEscalationPath,
    string guardianAndProtectionInterface,
    string rollbackOrCorrectiveActionPlan)
{
    var dependencies = new[]
    {
        new DependencyDeclaration(dependencyId, dependencyVersions)
    };

    return new ApplicationManifest(
        manifestId,
        applicationIdentity,
        applicationVersion,
        applicationOwner,
        applicationPurpose,
        packageIdentity,
        packageVersion,
        packageContentOrIntegrityInput,
        dependencies,
        requiredContracts,
        requiredSpecifications,
        requiredServices,
        providedCapabilities,
        intendedConsumers,
        requestedPermissions,
        authorityRequests,
        securityProfile,
        minimumResourceRequirements,
        resourceCeilings,
        degradedBehavior,
        persistenceRequirements,
        communicationRequirements,
        configurationRequirements,
        evidenceRequirements,
        lifecycleBehavior,
        healthReportingInterface,
        failureContainmentInterface,
        usesBranchBasedInternalArchitecture,
        msaDeclarations,
        majorBranchDeclarations,
        lsaDeclarations,
        csaEligibilityPolicy,
        selfDevelopmentOriginAndEscalationPath,
        guardianAndProtectionInterface,
        rollbackOrCorrectiveActionPlan);
}

sealed class InMemoryAdmissionBaselineProvider : IAdmissionBaselineProvider
{
    private readonly AdmissionBaselineSnapshot _snapshot;

    public InMemoryAdmissionBaselineProvider(AdmissionBaselineSnapshot snapshot)
    {
        _snapshot = snapshot;
    }

    public AdmissionBaselineSnapshot GetCurrentBaseline() => _snapshot;
}

sealed class FileBackedAdmissionBaselineProvider : IAdmissionBaselineProvider
{
    private readonly string _rootPath;

    public FileBackedAdmissionBaselineProvider(string? rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new InvalidOperationException("missing baseline configuration");
        }

        if (!Path.IsPathRooted(rootPath))
        {
            throw new InvalidOperationException("invalid baseline path");
        }

        _rootPath = Path.GetFullPath(rootPath);
        if (!Directory.Exists(_rootPath))
        {
            throw new InvalidOperationException("invalid baseline path");
        }

        var marker = Path.Combine(_rootPath, "admission-baseline.txt");
        if (!File.Exists(marker))
        {
            throw new InvalidOperationException("incomplete baseline");
        }
    }

    public AdmissionBaselineSnapshot GetCurrentBaseline()
    {
        var marker = Path.Combine(_rootPath, "admission-baseline.txt");
        var text = File.ReadAllText(marker);
        if (!text.Contains("mode=canonical", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("incomplete baseline");
        }

        return BaselineFactory.CreateCanonicalBaseline();
    }
}

static class BaselineFactory
{
    public static AdmissionBaselineSnapshot CreateCanonicalBaseline()
    {
        var registry = ContractRegistry.CreateCanonical();
        var con023 = new ContractRegistryEntry(
            "CON-023",
            "1.1",
            "Falcon Application Authority",
            "CON-000 / CON-023",
            "docs/contracts/CON-023_APPLICATION_CONTRACT_AND_MANIFEST.md",
            "governed application contract and manifest representation",
            "ACCEPTED",
            "REGISTERED");

        var app001 = new ContractRegistryEntry(
            "APP-001",
            "1.1",
            "Falcon Application Authority",
            "Falcon Application Authority",
            "docs/specifications/applications/APP-001_APPLICATION_BOUNDARY_AND_LIFECYCLE.md",
            "application boundary and lifecycle requirements",
            "Approved and Active",
            "Active");

        return new AdmissionBaselineSnapshot(
            registry.Entries.ToList(),
            con023,
            app001,
            "Falcon Application Authority",
            "CON-000 / CON-023",
            "ACCEPTED",
            "REGISTERED",
            "Approved and Active",
            "Active");
    }
}
