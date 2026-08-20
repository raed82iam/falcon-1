using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static partial class Program
{
    private static readonly string RepositoryRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
    private static readonly string SolutionPath = Path.Combine(RepositoryRoot, "Falcon.Foundation.ControlledProjectFoundation.slnx");
    private static readonly string CoreProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Core", "Foundation.Core.csproj");
    private static readonly string EnablingProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Enabling", "Foundation.Enabling.csproj");
    private static readonly string ContractsProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Contracts", "Foundation.Contracts.csproj");
    private static readonly string HealthFitnessProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj");
    private static readonly string SelfAwarenessProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.SelfAwareness", "Foundation.SelfAwareness.csproj");
    private static readonly string ContractRegistryProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.ContractRegistry", "Foundation.ContractRegistry.csproj");
    private static readonly string SchemaRegistryProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.SchemaRegistry", "Foundation.SchemaRegistry.csproj");
    private static readonly string ApplicationManifestProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.ApplicationManifest", "Foundation.ApplicationManifest.csproj");
    private static readonly string ArtifactPublicationProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.ArtifactPublication", "Foundation.ArtifactPublication.csproj");
    private static readonly string MessageAdmissionProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.MessageAdmission", "Foundation.MessageAdmission.csproj");
    private static readonly string MessageRoutingProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.MessageRouting", "Foundation.MessageRouting.csproj");
    private static readonly string MessageDeliveryProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.MessageDelivery", "Foundation.MessageDelivery.csproj");
    private static readonly string EventSystemProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.EventSystem", "Foundation.EventSystem.csproj");
    private static readonly string MessageProtectionProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.MessageProtection", "Foundation.MessageProtection.csproj");
    private static readonly string ApplicationLifecycleProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.ApplicationLifecycle", "Foundation.ApplicationLifecycle.csproj");
    private static readonly string ApplicationRuntimeHostingProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.ApplicationRuntimeHosting", "Foundation.ApplicationRuntimeHosting.csproj");
    private static readonly string IdentityRuntimeProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.IdentityRuntime", "Foundation.IdentityRuntime.csproj");
    private static readonly string AdmissionProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Admission", "Foundation.Admission.csproj");
    private static readonly string ServiceCatalogProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.ServiceCatalog", "Foundation.ServiceCatalog.csproj");
    private static readonly string DependencyGovernanceProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.DependencyGovernance", "Foundation.DependencyGovernance.csproj");
    private static readonly string AuthorityProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Authority", "Foundation.Authority.csproj");
    private static readonly string GuardianProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Guardian", "Foundation.Guardian.csproj");
    private static readonly string StateProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.State", "Foundation.State.csproj");
    private static readonly string EvidenceProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Evidence", "Foundation.Evidence.csproj");
    private static readonly string ReconciliationProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Reconciliation", "Foundation.Reconciliation.csproj");
    private static readonly string InfrastructureProjectPath = Path.Combine(RepositoryRoot, "src", "Foundation.Infrastructure", "Foundation.Infrastructure.csproj");
    private static readonly string TestProjectPath = Path.Combine(RepositoryRoot, "tests", "Falcon.Foundation.Architecture.Tests", "Falcon.Foundation.Architecture.Tests.csproj");
    private static readonly string SecurityTestProjectPath = Path.Combine(RepositoryRoot, "tests", "Falcon.Foundation.Security.Tests", "Falcon.Foundation.Security.Tests.csproj");
    private static readonly string WP04VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage3.WP04.Verifier", "Falcon.Stage3.WP04.Verifier.csproj");
    private static readonly string WP05VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage3.WP05.Verifier", "Falcon.Stage3.WP05.Verifier.csproj");
    private static readonly string Stage4WP01VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage4.WP01.Verifier", "Falcon.Stage4.WP01.Verifier.csproj");
    private static readonly string Stage4WP02VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage4.WP02.Verifier", "Falcon.Stage4.WP02.Verifier.csproj");
    private static readonly string Stage4WP03VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage4.WP03.Verifier", "Falcon.Stage4.WP03.Verifier.csproj");
    private static readonly string Stage4WP04VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage4.WP04.Verifier", "Falcon.Stage4.WP04.Verifier.csproj");
    private static readonly string Stage4WP05VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage4.WP05.Verifier", "Falcon.Stage4.WP05.Verifier.csproj");
    private static readonly string BaselineIntegrityVerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.BaselineIntegrity.Verifier", "Falcon.BaselineIntegrity.Verifier.csproj");
    private static readonly string Stage5WP03VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage5.WP03.Verifier", "Falcon.Stage5.WP03.Verifier.csproj");
    private static readonly string Stage5WP04VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage5.WP04.Verifier", "Falcon.Stage5.WP04.Verifier.csproj");
    private static readonly string Stage5WP05VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage5.WP05.Verifier", "Falcon.Stage5.WP05.Verifier.csproj");
    private static readonly string Stage5WP06VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage5.WP06.Verifier", "Falcon.Stage5.WP06.Verifier.csproj");
    private static readonly string Stage5WP07VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage5.WP07.Verifier", "Falcon.Stage5.WP07.Verifier.csproj");
    private static readonly string Stage5WP08VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage5.WP08.Verifier", "Falcon.Stage5.WP08.Verifier.csproj");
    private static readonly string Stage5WP09VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage5.WP09.Verifier", "Falcon.Stage5.WP09.Verifier.csproj");
    private static readonly string Stage7WP01VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage7.WP01.Verifier", "Falcon.Stage7.WP01.Verifier.csproj");
    private static readonly string Stage7WP02VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage7.WP02.Verifier", "Falcon.Stage7.WP02.Verifier.csproj");
    private static readonly string Stage7WP03VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage7.WP03.Verifier", "Falcon.Stage7.WP03.Verifier.csproj");
    private static readonly string Stage8WP01VerifierProjectPath = Path.Combine(RepositoryRoot, "verification", "Falcon.Stage8.WP01.Verifier", "Falcon.Stage8.WP01.Verifier.csproj");

    private static readonly string[] RequiredProductionProjects =
    [
        NormalizeRelative("src/Foundation.Core/Foundation.Core.csproj"),
        NormalizeRelative("src/Foundation.Infrastructure/Foundation.Infrastructure.csproj"),
        NormalizeRelative("src/Foundation.Enabling/Foundation.Enabling.csproj"),
        NormalizeRelative("src/Foundation.Contracts/Foundation.Contracts.csproj"),
        NormalizeRelative("src/Foundation.HealthFitness/Foundation.HealthFitness.csproj"),
        NormalizeRelative("src/Foundation.SelfAwareness/Foundation.SelfAwareness.csproj"),
        NormalizeRelative("src/Foundation.ContractRegistry/Foundation.ContractRegistry.csproj"),
        NormalizeRelative("src/Foundation.SchemaRegistry/Foundation.SchemaRegistry.csproj"),
        NormalizeRelative("src/Foundation.ApplicationManifest/Foundation.ApplicationManifest.csproj"),
        NormalizeRelative("src/Foundation.ArtifactPublication/Foundation.ArtifactPublication.csproj"),
        NormalizeRelative("src/Foundation.MessageAdmission/Foundation.MessageAdmission.csproj"),
        NormalizeRelative("src/Foundation.MessageRouting/Foundation.MessageRouting.csproj"),
        NormalizeRelative("src/Foundation.MessageDelivery/Foundation.MessageDelivery.csproj"),
        NormalizeRelative("src/Foundation.EventSystem/Foundation.EventSystem.csproj"),
        NormalizeRelative("src/Foundation.MessageProtection/Foundation.MessageProtection.csproj"),
        NormalizeRelative("src/Foundation.ApplicationLifecycle/Foundation.ApplicationLifecycle.csproj"),
        NormalizeRelative("src/Foundation.ApplicationRuntimeHosting/Foundation.ApplicationRuntimeHosting.csproj"),
        NormalizeRelative("src/Foundation.IdentityRuntime/Foundation.IdentityRuntime.csproj"),
        NormalizeRelative("src/Foundation.Admission/Foundation.Admission.csproj"),
        NormalizeRelative("src/Foundation.ServiceCatalog/Foundation.ServiceCatalog.csproj"),
        NormalizeRelative("src/Foundation.DependencyGovernance/Foundation.DependencyGovernance.csproj"),
        NormalizeRelative("src/Foundation.Authority/Foundation.Authority.csproj"),
        NormalizeRelative("src/Foundation.Guardian/Foundation.Guardian.csproj"),
        NormalizeRelative("src/Foundation.State/Foundation.State.csproj"),
        NormalizeRelative("src/Foundation.Evidence/Foundation.Evidence.csproj"),
        NormalizeRelative("src/Foundation.Reconciliation/Foundation.Reconciliation.csproj")
    ];

    private static readonly string[] ForbiddenProductionProjectTokens =
    [
        NormalizeRelative("src/Falcon.Foundation.Core/Falcon.Foundation.Core.csproj"),
        NormalizeRelative("src/Falcon.Foundation.Infrastructure/Falcon.Foundation.Infrastructure.csproj"),
        NormalizeRelative("src/Falcon.Foundation.Enabling/Falcon.Foundation.Enabling.csproj"),
        NormalizeRelative("src/Falcon.Foundation.ServiceCatalog/Falcon.Foundation.ServiceCatalog.csproj"),
        NormalizeRelative("src/Falcon.Stage2.Contracts/Falcon.Stage2.Contracts.csproj"),
        NormalizeRelative("src/Falcon.Stage3.Registry/Falcon.Stage3.Registry.csproj"),
        NormalizeRelative("src/Falcon.Stage3.Admission/Falcon.Stage3.Admission.csproj")
    ];

    private static int Main()
    {
        var failures = new List<string>();

        RequireFile(SolutionPath, failures, "solution file");
        RequireFile(CoreProjectPath, failures, "core project file");
        RequireFile(ContractsProjectPath, failures, "contracts project file");
        RequireFile(HealthFitnessProjectPath, failures, "health and fitness project file");
        RequireFile(SelfAwarenessProjectPath, failures, "self-awareness project file");
        RequireFile(EnablingProjectPath, failures, "enabling project file");
        RequireFile(ContractRegistryProjectPath, failures, "contract registry project file");
        RequireFile(SchemaRegistryProjectPath, failures, "schema registry project file");
        RequireFile(ApplicationManifestProjectPath, failures, "application manifest project file");
        RequireFile(ArtifactPublicationProjectPath, failures, "artifact publication project file");
        RequireFile(MessageAdmissionProjectPath, failures, "message admission project file");
        RequireFile(MessageRoutingProjectPath, failures, "message routing project file");
        RequireFile(MessageDeliveryProjectPath, failures, "message delivery project file");
        RequireFile(EventSystemProjectPath, failures, "event system project file");
        RequireFile(MessageProtectionProjectPath, failures, "message protection project file");
        RequireFile(ApplicationLifecycleProjectPath, failures, "application lifecycle project file");
        RequireFile(ApplicationRuntimeHostingProjectPath, failures, "application runtime hosting project file");
        RequireFile(IdentityRuntimeProjectPath, failures, "identity runtime project file");
        RequireFile(AdmissionProjectPath, failures, "admission project file");
        RequireFile(ServiceCatalogProjectPath, failures, "service catalog project file");
        RequireFile(DependencyGovernanceProjectPath, failures, "dependency governance project file");
        RequireFile(InfrastructureProjectPath, failures, "infrastructure project file");
        RequireFile(AuthorityProjectPath, failures, "authority project file");
        RequireFile(GuardianProjectPath, failures, "guardian project file");
        RequireFile(StateProjectPath, failures, "state project file");
        RequireFile(EvidenceProjectPath, failures, "evidence project file");
        RequireFile(ReconciliationProjectPath, failures, "reconciliation project file");
        RequireFile(TestProjectPath, failures, "architecture test project file");
        RequireFile(SecurityTestProjectPath, failures, "security test project file");
        RequireFile(WP04VerifierProjectPath, failures, "WP-04 verifier project file");
        RequireFile(WP05VerifierProjectPath, failures, "WP-05 verifier project file");
        RequireFile(Stage4WP01VerifierProjectPath, failures, "Stage 4 WP-01 verifier project file");
        RequireFile(Stage4WP02VerifierProjectPath, failures, "Stage 4 WP-02 verifier project file");
        RequireFile(Stage4WP03VerifierProjectPath, failures, "Stage 4 WP-03 verifier project file");
        RequireFile(Stage4WP04VerifierProjectPath, failures, "Stage 4 WP-04 verifier project file");
        RequireFile(Stage4WP05VerifierProjectPath, failures, "Stage 4 WP-05 verifier project file");
        RequireFile(BaselineIntegrityVerifierProjectPath, failures, "baseline-integrity verifier project file");
        RequireFile(Stage5WP03VerifierProjectPath, failures, "Stage 5 WP-03 verifier project file");
        RequireFile(Stage5WP04VerifierProjectPath, failures, "Stage 5 WP-04 verifier project file");
        RequireFile(Stage5WP05VerifierProjectPath, failures, "Stage 5 WP-05 verifier project file");
        RequireFile(Stage5WP06VerifierProjectPath, failures, "Stage 5 WP-06 verifier project file");
        RequireFile(Stage5WP07VerifierProjectPath, failures, "Stage 5 WP-07 verifier project file");
        RequireFile(Stage5WP08VerifierProjectPath, failures, "Stage 5 WP-08 verifier project file");
        RequireFile(Stage5WP09VerifierProjectPath, failures, "Stage 5 WP-09 verifier project file");
        RequireFile(Stage7WP01VerifierProjectPath, failures, "Stage 7 WP-01 verifier project file");
        RequireFile(Stage7WP02VerifierProjectPath, failures, "Stage 7 WP-02 verifier project file");
        RequireFile(Stage7WP03VerifierProjectPath, failures, "Stage 7 WP-03 verifier project file");
        RequireFile(Stage8WP01VerifierProjectPath, failures, "Stage 8 WP-01 verifier project file");

        var solutionProjects = ReadSolutionProjectPaths(SolutionPath);
        ValidateSolutionMembership(solutionProjects, failures);
        ValidateForbiddenProjectPaths(failures);

        ValidateAcceptedFactPublicationBoundary(failures);

        AssertNoProjectReferences(CoreProjectPath, Array.Empty<string>(), failures, "Core");
        AssertNoProjectReferences(ContractsProjectPath, Array.Empty<string>(), failures, "Contracts");
        AssertProjectReferences(HealthFitnessProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath)
        }, failures, "HealthFitness");
        AssertProjectReferences(SelfAwarenessProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(HealthFitnessProjectPath)
        }, failures, "SelfAwareness");
        AssertNoProjectReferences(EnablingProjectPath, Array.Empty<string>(), failures, "Enabling");
        AssertNoProjectReferences(ArtifactPublicationProjectPath, Array.Empty<string>(), failures, "ArtifactPublication");
        AssertNoProjectReferences(MessageProtectionProjectPath, Array.Empty<string>(), failures, "MessageProtection");
        AssertNoProjectReferences(ApplicationLifecycleProjectPath, Array.Empty<string>(), failures, "ApplicationLifecycle");
        AssertNoProjectReferences(ApplicationRuntimeHostingProjectPath, Array.Empty<string>(), failures, "ApplicationRuntimeHosting");
        AssertNoProjectReferences(IdentityRuntimeProjectPath, Array.Empty<string>(), failures, "IdentityRuntime");
        AssertProjectReferences(SchemaRegistryProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath)
        }, failures, "SchemaRegistry");
        AssertProjectReferences(ApplicationManifestProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(SchemaRegistryProjectPath)
        }, failures, "ApplicationManifest");
        AssertProjectReferences(MessageAdmissionProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(SchemaRegistryProjectPath),
            NormalizeAbsolute(ApplicationManifestProjectPath),
            NormalizeAbsolute(AuthorityProjectPath)
        }, failures, "MessageAdmission");
        AssertProjectReferences(MessageRoutingProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(ApplicationManifestProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath)
        }, failures, "MessageRouting");
        AssertProjectReferences(MessageDeliveryProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath),
            NormalizeAbsolute(MessageRoutingProjectPath)
        }, failures, "MessageDelivery");
        AssertProjectReferences(EventSystemProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath),
            NormalizeAbsolute(MessageDeliveryProjectPath)
        }, failures, "EventSystem");
        AssertProjectReferences(AuthorityProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath)
        }, failures, "Authority");
        AssertProjectReferences(GuardianProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath)
        }, failures, "Guardian");
        AssertProjectReferences(StateProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath)
        }, failures, "State");
        AssertProjectReferences(EvidenceProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(StateProjectPath),
            NormalizeAbsolute(AuthorityProjectPath)
        }, failures, "Evidence");
        AssertProjectReferences(ReconciliationProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(StateProjectPath),
            NormalizeAbsolute(EvidenceProjectPath)
        }, failures, "Reconciliation");
        AssertProjectReferences(InfrastructureProjectPath, new[]
        {
            NormalizeAbsolute(CoreProjectPath),
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(AuthorityProjectPath),
            NormalizeAbsolute(StateProjectPath),
            NormalizeAbsolute(EvidenceProjectPath),
            NormalizeAbsolute(ReconciliationProjectPath)
        }, failures, "Infrastructure");
        AssertProjectReferences(ServiceCatalogProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(ContractRegistryProjectPath)
        }, failures, "ServiceCatalog");
        AssertProjectReferences(DependencyGovernanceProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(ContractRegistryProjectPath),
            NormalizeAbsolute(AdmissionProjectPath),
            NormalizeAbsolute(ServiceCatalogProjectPath)
        }, failures, "DependencyGovernance");
        AssertProjectReferences(TestProjectPath, new[]
        {
            NormalizeAbsolute(CoreProjectPath),
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(InfrastructureProjectPath)
        }, failures, "Architecture test harness");
        AssertProjectReferences(WP04VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(ContractRegistryProjectPath),
            NormalizeAbsolute(AdmissionProjectPath),
            NormalizeAbsolute(ServiceCatalogProjectPath),
            NormalizeAbsolute(DependencyGovernanceProjectPath)
        }, failures, "WP-04 verifier");
        AssertProjectReferences(WP05VerifierProjectPath, new[]
        {
            NormalizeAbsolute(CoreProjectPath),
            NormalizeAbsolute(InfrastructureProjectPath),
            NormalizeAbsolute(ContractsProjectPath)
        }, failures, "WP-05 verifier");
        AssertProjectReferences(Stage4WP01VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(AuthorityProjectPath)
        }, failures, "Stage 4 WP-01 verifier");
        AssertProjectReferences(Stage4WP02VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(CoreProjectPath),
            NormalizeAbsolute(InfrastructureProjectPath),
            NormalizeAbsolute(AuthorityProjectPath)
        }, failures, "Stage 4 WP-02 verifier");
        AssertProjectReferences(Stage4WP03VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(CoreProjectPath),
            NormalizeAbsolute(AuthorityProjectPath),
            NormalizeAbsolute(StateProjectPath),
            NormalizeAbsolute(InfrastructureProjectPath)
        }, failures, "Stage 4 WP-03 verifier");
        AssertProjectReferences(Stage4WP04VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(CoreProjectPath),
            NormalizeAbsolute(AuthorityProjectPath),
            NormalizeAbsolute(StateProjectPath),
            NormalizeAbsolute(EvidenceProjectPath),
            NormalizeAbsolute(InfrastructureProjectPath)
        }, failures, "Stage 4 WP-04 verifier");
        AssertProjectReferences(Stage4WP05VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(StateProjectPath),
            NormalizeAbsolute(EvidenceProjectPath),
            NormalizeAbsolute(ReconciliationProjectPath)
        }, failures, "Stage 4 WP-05 verifier");
        AssertProjectReferences(BaselineIntegrityVerifierProjectPath, new[]
        {
            NormalizeAbsolute(EnablingProjectPath)
        }, failures, "Baseline-integrity verifier");
        AssertProjectReferences(Stage5WP03VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(SchemaRegistryProjectPath),
            NormalizeAbsolute(ApplicationManifestProjectPath)
        }, failures, "Stage 5 WP-03 verifier");
        AssertProjectReferences(Stage5WP04VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(SchemaRegistryProjectPath),
            NormalizeAbsolute(ApplicationManifestProjectPath),
            NormalizeAbsolute(AuthorityProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath)
        }, failures, "Stage 5 WP-04 verifier");
        AssertProjectReferences(Stage5WP05VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(SchemaRegistryProjectPath),
            NormalizeAbsolute(ApplicationManifestProjectPath),
            NormalizeAbsolute(AuthorityProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath),
            NormalizeAbsolute(MessageRoutingProjectPath)
        }, failures, "Stage 5 WP-05 verifier");
        AssertProjectReferences(Stage5WP06VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(SchemaRegistryProjectPath),
            NormalizeAbsolute(ApplicationManifestProjectPath),
            NormalizeAbsolute(AuthorityProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath),
            NormalizeAbsolute(MessageRoutingProjectPath),
            NormalizeAbsolute(MessageDeliveryProjectPath)
        }, failures, "Stage 5 WP-06 verifier");
        AssertProjectReferences(Stage5WP07VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath),
            NormalizeAbsolute(MessageDeliveryProjectPath),
            NormalizeAbsolute(EventSystemProjectPath)
        }, failures, "Stage 5 WP-07 verifier");
        AssertProjectReferences(Stage5WP08VerifierProjectPath, new[]
        {
            NormalizeAbsolute(MessageProtectionProjectPath)
        }, failures, "Stage 5 WP-08 verifier");
        AssertProjectReferences(Stage5WP09VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ApplicationLifecycleProjectPath)
        }, failures, "Stage 5 WP-09 verifier");
        AssertProjectReferences(Stage7WP01VerifierProjectPath, new[]
        {
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(HealthFitnessProjectPath),
            NormalizeAbsolute(ContractRegistryProjectPath)
        }, failures, "Stage 7 WP-01 verifier");
        AssertProjectReferences(Stage7WP02VerifierProjectPath, new[]
        {
            NormalizeAbsolute(HealthFitnessProjectPath)
        }, failures, "Stage 7 WP-02 verifier");
        AssertProjectReferences(Stage7WP03VerifierProjectPath, new[]
        {
            NormalizeAbsolute(SelfAwarenessProjectPath),
            NormalizeAbsolute(HealthFitnessProjectPath)
        }, failures, "Stage 7 WP-03 verifier");
        AssertProjectReferences(Stage8WP01VerifierProjectPath, new[]
        {
            NormalizeAbsolute(GuardianProjectPath)
        }, failures, "Stage 8 WP-01 verifier");

        ValidateProductionReferenceGraph(failures);
        ValidatePermanentProductionIdentitySurfaces(failures);

        if (failures.Count > 0)
        {
            Console.Error.WriteLine("Baseline integrity architecture boundary validation: FAIL");
            foreach (var failure in failures)
            {
                Console.Error.WriteLine($"- {failure}");
            }

            return 1;
        }

        Console.WriteLine("Baseline integrity architecture boundary validation: PASS");
        Console.WriteLine("Validated solution membership, project-reference direction, and boundary surface.");
        return 0;
    }

    private static void RequireFile(string path, ICollection<string> failures, string label)
    {
        if (!File.Exists(path)) failures.Add($"Missing {label}: {path}");
    }

    private static IReadOnlyList<string> ReadSolutionProjectPaths(string path)
    {
        var document = XDocument.Load(path);
        return document.Root?.Elements("Project")
            .Select(project => NormalizeRelative(project.Attribute("Path")?.Value ?? string.Empty))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList() ?? new List<string>();
    }

    private static void ValidateSolutionMembership(IReadOnlyCollection<string> solutionProjects, ICollection<string> failures)
    {
        var normalizedProjects = solutionProjects.ToArray();
        var duplicatePaths = normalizedProjects.GroupBy(value => value, StringComparer.Ordinal)
            .Where(group => group.Count() > 1).Select(group => group.Key).ToArray();
        if (duplicatePaths.Length > 0) failures.Add($"Duplicate solution project paths: {string.Join(", ", duplicatePaths)}.");

        foreach (var required in RequiredProductionProjects)
        {
            var count = normalizedProjects.Count(value => StringComparer.Ordinal.Equals(value, required));
            if (count != 1) failures.Add($"Required production project path '{required}' expected exactly once, found {count}.");
        }

        RequireSolutionProjectCount(normalizedProjects, "tests/Falcon.Foundation.Architecture.Tests/Falcon.Foundation.Architecture.Tests.csproj", "Architecture test", failures);
        RequireSolutionProjectCount(normalizedProjects, "tests/Falcon.Foundation.Security.Tests/Falcon.Foundation.Security.Tests.csproj", "Security test", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage3.WP04.Verifier/Falcon.Stage3.WP04.Verifier.csproj", "WP-04 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage3.WP05.Verifier/Falcon.Stage3.WP05.Verifier.csproj", "WP-05 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage4.WP01.Verifier/Falcon.Stage4.WP01.Verifier.csproj", "Stage 4 WP-01 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage4.WP05.Verifier/Falcon.Stage4.WP05.Verifier.csproj", "Stage 4 WP-05 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.BaselineIntegrity.Verifier/Falcon.BaselineIntegrity.Verifier.csproj", "Baseline-integrity verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage5.WP03.Verifier/Falcon.Stage5.WP03.Verifier.csproj", "Stage 5 WP-03 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage5.WP04.Verifier/Falcon.Stage5.WP04.Verifier.csproj", "Stage 5 WP-04 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage5.WP05.Verifier/Falcon.Stage5.WP05.Verifier.csproj", "Stage 5 WP-05 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage5.WP06.Verifier/Falcon.Stage5.WP06.Verifier.csproj", "Stage 5 WP-06 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage5.WP07.Verifier/Falcon.Stage5.WP07.Verifier.csproj", "Stage 5 WP-07 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage5.WP08.Verifier/Falcon.Stage5.WP08.Verifier.csproj", "Stage 5 WP-08 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage5.WP09.Verifier/Falcon.Stage5.WP09.Verifier.csproj", "Stage 5 WP-09 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage7.WP01.Verifier/Falcon.Stage7.WP01.Verifier.csproj", "Stage 7 WP-01 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage7.WP02.Verifier/Falcon.Stage7.WP02.Verifier.csproj", "Stage 7 WP-02 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage7.WP03.Verifier/Falcon.Stage7.WP03.Verifier.csproj", "Stage 7 WP-03 verifier", failures);
        RequireSolutionProjectCount(normalizedProjects, "verification/Falcon.Stage8.WP01.Verifier/Falcon.Stage8.WP01.Verifier.csproj", "Stage 8 WP-01 verifier", failures);

        foreach (var forbidden in ForbiddenProductionProjectTokens)
        {
            if (normalizedProjects.Any(value => string.Equals(value, forbidden, StringComparison.Ordinal)))
                failures.Add($"Forbidden legacy production project path detected: {forbidden}");
        }

        if (normalizedProjects.Any(value => value.Contains("Stage", StringComparison.OrdinalIgnoreCase) && value.StartsWith(NormalizeRelative("src/"), StringComparison.Ordinal)))
            failures.Add("Production project path contains a Stage identifier.");

        if (normalizedProjects.Where(value => value.StartsWith(NormalizeRelative("src/"), StringComparison.Ordinal))
                .Any(value => ContainsForbiddenIdentityToken(Path.GetFileNameWithoutExtension(value))))
            failures.Add("Permanent production project filename contains a forbidden identity token.");
    }

    private static void RequireSolutionProjectCount(string[] normalizedProjects, string path, string label, ICollection<string> failures)
    {
        var count = normalizedProjects.Count(value => StringComparer.Ordinal.Equals(value, NormalizeRelative(path)));
        if (count != 1) failures.Add($"{label} project path expected exactly once, found {count}.");
    }

    private static void ValidateForbiddenProjectPaths(ICollection<string> failures)
    {
        foreach (var path in ForbiddenProductionProjectTokens)
        {
            var windowsStyle = path.Replace('/', '\\');
            var unixStyle = path.Replace('\\', '/');
            if (!IsForbiddenProductionProjectPath(windowsStyle) || !IsForbiddenProductionProjectPath(unixStyle))
                failures.Add($"Forbidden path token did not reject both separators: {path}");
        }
    }

    private static void ValidateProductionReferenceGraph(ICollection<string> failures)
    {
        var discoveredProjects = Directory.EnumerateFiles(Path.Combine(RepositoryRoot, "src"), "*.csproj", SearchOption.AllDirectories)
            .Select(NormalizeAbsolute).Distinct(StringComparer.Ordinal).ToArray();

        var candidateProject = NormalizeAbsolute(Path.Combine(RepositoryRoot, "src", "Falcon.Stage0B.Candidates", "Falcon.Stage0B.Candidates.csproj"));
        var permanentProjects = discoveredProjects.Where(project => !string.Equals(project, candidateProject, StringComparison.Ordinal)).ToArray();

        var requiredPermanentProjects = new HashSet<string>(StringComparer.Ordinal)
        {
            NormalizeAbsolute(CoreProjectPath),
            NormalizeAbsolute(InfrastructureProjectPath),
            NormalizeAbsolute(EnablingProjectPath),
            NormalizeAbsolute(ContractsProjectPath),
            NormalizeAbsolute(HealthFitnessProjectPath),
            NormalizeAbsolute(SelfAwarenessProjectPath),
            NormalizeAbsolute(ContractRegistryProjectPath),
            NormalizeAbsolute(SchemaRegistryProjectPath),
            NormalizeAbsolute(ApplicationManifestProjectPath),
            NormalizeAbsolute(ArtifactPublicationProjectPath),
            NormalizeAbsolute(MessageAdmissionProjectPath),
            NormalizeAbsolute(MessageRoutingProjectPath),
            NormalizeAbsolute(MessageDeliveryProjectPath),
            NormalizeAbsolute(EventSystemProjectPath),
            NormalizeAbsolute(MessageProtectionProjectPath),
            NormalizeAbsolute(ApplicationLifecycleProjectPath),
            NormalizeAbsolute(ApplicationRuntimeHostingProjectPath),
            NormalizeAbsolute(IdentityRuntimeProjectPath),
            NormalizeAbsolute(AdmissionProjectPath),
            NormalizeAbsolute(ServiceCatalogProjectPath),
            NormalizeAbsolute(DependencyGovernanceProjectPath),
            NormalizeAbsolute(AuthorityProjectPath),
            NormalizeAbsolute(GuardianProjectPath),
            NormalizeAbsolute(StateProjectPath),
            NormalizeAbsolute(EvidenceProjectPath),
            NormalizeAbsolute(ReconciliationProjectPath)
        };

        foreach (var project in permanentProjects)
        {
            if (!requiredPermanentProjects.Contains(project)) failures.Add($"Unapproved permanent production project discovered: {project}");
        }

        if (!discoveredProjects.Any(project => string.Equals(project, candidateProject, StringComparison.Ordinal)))
            failures.Add($"Missing classified candidate project: {candidateProject}");

        var allowedEdges = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [NormalizeAbsolute(InfrastructureProjectPath)] = [NormalizeAbsolute(CoreProjectPath), NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(AuthorityProjectPath), NormalizeAbsolute(StateProjectPath), NormalizeAbsolute(EvidenceProjectPath), NormalizeAbsolute(ReconciliationProjectPath)],
            [NormalizeAbsolute(ReconciliationProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(StateProjectPath), NormalizeAbsolute(EvidenceProjectPath)],
            [NormalizeAbsolute(EvidenceProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(StateProjectPath), NormalizeAbsolute(AuthorityProjectPath)],
            [NormalizeAbsolute(StateProjectPath)] = [NormalizeAbsolute(ContractsProjectPath)],
            [NormalizeAbsolute(HealthFitnessProjectPath)] = [NormalizeAbsolute(ContractsProjectPath)],
            [NormalizeAbsolute(SelfAwarenessProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(HealthFitnessProjectPath)],
            [NormalizeAbsolute(ServiceCatalogProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(ContractRegistryProjectPath)],
            [NormalizeAbsolute(SchemaRegistryProjectPath)] = [NormalizeAbsolute(ContractsProjectPath)],
            [NormalizeAbsolute(ApplicationManifestProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(SchemaRegistryProjectPath)],
            [NormalizeAbsolute(MessageAdmissionProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(SchemaRegistryProjectPath), NormalizeAbsolute(ApplicationManifestProjectPath), NormalizeAbsolute(AuthorityProjectPath)],
            [NormalizeAbsolute(MessageRoutingProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(ApplicationManifestProjectPath), NormalizeAbsolute(MessageAdmissionProjectPath)],
            [NormalizeAbsolute(MessageDeliveryProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(MessageAdmissionProjectPath), NormalizeAbsolute(MessageRoutingProjectPath)],
            [NormalizeAbsolute(EventSystemProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(MessageAdmissionProjectPath), NormalizeAbsolute(MessageDeliveryProjectPath)],
            [NormalizeAbsolute(AdmissionProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(ContractRegistryProjectPath)],
            [NormalizeAbsolute(DependencyGovernanceProjectPath)] = [NormalizeAbsolute(ContractsProjectPath), NormalizeAbsolute(ContractRegistryProjectPath), NormalizeAbsolute(AdmissionProjectPath), NormalizeAbsolute(ServiceCatalogProjectPath)],
            [NormalizeAbsolute(AuthorityProjectPath)] = [NormalizeAbsolute(ContractsProjectPath)],
            [NormalizeAbsolute(GuardianProjectPath)] = [NormalizeAbsolute(ContractsProjectPath)]
        };

        var graph = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.Ordinal);
        foreach (var project in permanentProjects)
        {
            var references = ReadProjectReferences(project).Select(NormalizeAbsolute)
                .Where(reference => permanentProjects.Contains(reference, StringComparer.Ordinal))
                .Distinct(StringComparer.Ordinal).ToArray();
            graph[project] = references;

            foreach (var reference in references)
            {
                if (!allowedEdges.TryGetValue(project, out var allowed) || !allowed.Contains(reference, StringComparer.Ordinal))
                    failures.Add($"Disallowed production project reference: {Path.GetFileNameWithoutExtension(project)} -> {Path.GetFileNameWithoutExtension(reference)}");
            }

            foreach (var reference in ReadProjectReferences(project))
            {
                var normalizedReference = NormalizeAbsolute(reference);
                if (string.Equals(normalizedReference, candidateProject, StringComparison.Ordinal))
                    failures.Add($"Permanent production project references candidate project: {Path.GetFileNameWithoutExtension(project)} -> {Path.GetFileNameWithoutExtension(reference)}");

                if (reference.Contains($"{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                    !permanentProjects.Contains(normalizedReference, StringComparer.Ordinal) &&
                    !string.Equals(normalizedReference, candidateProject, StringComparison.Ordinal))
                    failures.Add($"Permanent production project references unapproved non-production project: {Path.GetFileNameWithoutExtension(project)} -> {Path.GetFileNameWithoutExtension(reference)}");
            }
        }

        var visited = new HashSet<string>(StringComparer.Ordinal);
        var visiting = new HashSet<string>(StringComparer.Ordinal);
        foreach (var project in permanentProjects)
        {
            if (HasCycle(project, graph, visited, visiting))
            {
                failures.Add($"Production project reference cycle detected starting at {Path.GetFileNameWithoutExtension(project)}");
                break;
            }
        }
    }

    private static void ValidatePermanentProductionIdentitySurfaces(ICollection<string> failures)
    {
        var productionProjects = new[]
        {
            CoreProjectPath,
            InfrastructureProjectPath,
            EnablingProjectPath,
            ContractsProjectPath,
            HealthFitnessProjectPath,
            ContractRegistryProjectPath,
            SchemaRegistryProjectPath,
            ApplicationManifestProjectPath,
            ArtifactPublicationProjectPath,
            MessageAdmissionProjectPath,
            MessageRoutingProjectPath,
            MessageDeliveryProjectPath,
            EventSystemProjectPath,
            MessageProtectionProjectPath,
            ApplicationLifecycleProjectPath,
            ApplicationRuntimeHostingProjectPath,
            IdentityRuntimeProjectPath,
            AdmissionProjectPath,
            ServiceCatalogProjectPath,
            DependencyGovernanceProjectPath,
            AuthorityProjectPath,
            GuardianProjectPath,
            StateProjectPath,
            EvidenceProjectPath
        };

        var expectedAssemblyNames = new[]
        {
            "Foundation.Core",
            "Foundation.Infrastructure",
            "Foundation.Enabling",
            "Foundation.Contracts",
            "Foundation.HealthFitness",
            "Foundation.ContractRegistry",
            "Foundation.SchemaRegistry",
            "Foundation.ApplicationManifest",
            "Foundation.ArtifactPublication",
            "Foundation.MessageAdmission",
            "Foundation.MessageRouting",
            "Foundation.MessageDelivery",
            "Foundation.EventSystem",
            "Foundation.MessageProtection",
            "Foundation.ApplicationLifecycle",
            "Foundation.ApplicationRuntimeHosting",
            "Foundation.IdentityRuntime",
            "Foundation.Admission",
            "Foundation.ServiceCatalog",
            "Foundation.DependencyGovernance",
            "Foundation.Authority",
            "Foundation.Guardian",
            "Foundation.State",
            "Foundation.Evidence"
        };

        foreach (var projectPath in productionProjects) ValidateProjectIdentity(projectPath, failures);

        foreach (var assemblyName in expectedAssemblyNames)
        {
            if (ContainsForbiddenIdentityToken(assemblyName))
                failures.Add($"Permanent production assembly identity contains a forbidden token: {assemblyName}");
        }

        var sourceFiles = productionProjects
            .SelectMany(projectPath => Directory.EnumerateFiles(Path.GetDirectoryName(projectPath) ?? RepositoryRoot, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                             && !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        var publicTypeNames = new HashSet<string>(StringComparer.Ordinal);
        var namespaceDeclarations = new HashSet<string>(StringComparer.Ordinal);

        foreach (var sourceFile in sourceFiles)
        {
            var text = File.ReadAllText(sourceFile);
            foreach (Match match in NamespaceRegex().Matches(text)) namespaceDeclarations.Add(match.Groups[1].Value);
            foreach (Match match in PublicTypeRegex().Matches(text)) publicTypeNames.Add(match.Groups[1].Value);
        }

        var requiredTypeNames = new[]
        {
            "ContractVersions",
            "ContractValidators",
            "ProviderContractVersions",
            "ProviderContractValidators",
            "BootstrapContractValidators"
        };

        var forbiddenTypeNames = new[]
        {
            "Stage2ContractVersions",
            "Stage2ContractValidators",
            "Stage2ProviderVersions",
            "Stage2ProviderValidators",
            "Stage2BootstrapValidators"
        };

        foreach (var typeName in publicTypeNames)
        {
            if (ContainsForbiddenIdentityToken(typeName)) failures.Add($"Permanent production public type name contains a forbidden identity token: {typeName}");
        }
        foreach (var typeName in requiredTypeNames)
        {
            if (!publicTypeNames.Contains(typeName)) failures.Add($"Required public production type missing: {typeName}");
        }
        foreach (var typeName in forbiddenTypeNames)
        {
            if (publicTypeNames.Contains(typeName)) failures.Add($"Forbidden legacy public production type present: {typeName}");
        }
        foreach (var ns in namespaceDeclarations.Where(ns => ns.StartsWith("Foundation.", StringComparison.Ordinal)))
        {
            if (ContainsForbiddenIdentityToken(ns)) failures.Add($"Permanent production namespace contains a forbidden identity token: {ns}");
        }
    }

    private static void ValidateAcceptedFactPublicationBoundary(ICollection<string> failures)
    {
        var providerPath = Path.Combine(RepositoryRoot, "src", "Foundation.Evidence", "FileEvidenceJournalProvider.cs");
        if (!File.Exists(providerPath))
        {
            failures.Add("Accepted-fact provider source is missing.");
            return;
        }

        var source = File.ReadAllText(providerPath);
        if (!source.Contains("internal AcceptedFactPublishResult AppendAcceptedFact", StringComparison.Ordinal))
            failures.Add("Accepted-fact append must remain assembly-internal.");
        if (source.Contains("public AcceptedFactPublishResult AppendAcceptedFact", StringComparison.Ordinal))
            failures.Add("Accepted-fact provider bypass is publicly exposed.");
    }

    private static void ValidateProjectIdentity(string projectPath, ICollection<string> failures)
    {
        var document = XDocument.Load(projectPath);
        var projectName = Path.GetFileNameWithoutExtension(projectPath);
        var expectedAssemblyName = projectName;
        var expectedRootNamespace = projectName;

        if (ContainsForbiddenIdentityToken(projectName)) failures.Add($"Permanent production project filename contains a forbidden identity token: {projectName}");

        foreach (var elementName in new[] { "AssemblyName", "RootNamespace" })
        {
            var value = document.Descendants().FirstOrDefault(element => element.Name.LocalName == elementName)?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                failures.Add($"{projectName} missing {elementName}.");
                continue;
            }

            if (!string.Equals(value, elementName == "AssemblyName" ? expectedAssemblyName : expectedRootNamespace, StringComparison.Ordinal))
                failures.Add($"{projectName} {elementName} mismatch. Expected '{expectedAssemblyName}', actual '{value}'.");
            if (ContainsForbiddenIdentityToken(value)) failures.Add($"{projectName} {elementName} contains a forbidden identity token: {value}");
        }
    }

    private static bool ContainsForbiddenIdentityToken(string value)
    {
        var tokens = Tokenize(value);
        return tokens.Any(token =>
            string.Equals(token, "Falcon", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(token, "Stage", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(token, "WP", StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<string> Tokenize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) yield break;
        foreach (Match match in TokenRegex().Matches(value)) yield return match.Value;
    }

    private static bool IsForbiddenProductionProjectPath(string path)
        => ForbiddenProductionProjectTokens.Contains(NormalizeRelative(path), StringComparer.Ordinal);

    private static void AssertNoProjectReferences(string projectPath, IReadOnlyCollection<string> expectedNone, ICollection<string> failures, string label)
    {
        var references = ReadProjectReferences(projectPath);
        if (references.Count != expectedNone.Count)
            failures.Add($"{label} reference count mismatch. Expected {expectedNone.Count}, actual {references.Count}.");
    }

    private static void AssertProjectReferences(string projectPath, IReadOnlyCollection<string> expectedReferences, ICollection<string> failures, string label)
    {
        var references = ReadProjectReferences(projectPath);
        var normalizedExpected = expectedReferences.Select(NormalizeAbsolute).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        var normalizedActual = references.Select(NormalizeAbsolute).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        if (!normalizedActual.SequenceEqual(normalizedExpected, StringComparer.Ordinal))
            failures.Add($"{label} project references mismatch. Expected: {string.Join(", ", normalizedExpected)}. Actual: {string.Join(", ", normalizedActual)}.");
    }

    private static IReadOnlyList<string> ReadProjectReferences(string projectPath)
    {
        var document = XDocument.Load(projectPath);
        return document.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath) ?? RepositoryRoot, value)))
            .ToList() ?? new List<string>();
    }

    private static bool HasCycle(string project, IReadOnlyDictionary<string, IReadOnlyCollection<string>> graph, ISet<string> visited, ISet<string> visiting)
    {
        if (visited.Contains(project)) return false;
        if (!visiting.Add(project)) return true;
        if (graph.TryGetValue(project, out var references))
        {
            foreach (var reference in references)
            {
                if (HasCycle(reference, graph, visited, visiting)) return true;
            }
        }
        visiting.Remove(project);
        visited.Add(project);
        return false;
    }

    private static string NormalizeRelative(string path) => path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
    private static string NormalizeAbsolute(string path) => Path.GetFullPath(path);

    [GeneratedRegex("[A-Z]?[a-z]+|[A-Z]+(?![a-z])|\\d+")]
    private static partial Regex TokenRegex();

    [GeneratedRegex("^\\s*namespace\\s+([A-Za-z_][A-Za-z0-9_.]*)", RegexOptions.Multiline)]
    private static partial Regex NamespaceRegex();

    [GeneratedRegex("^\\s*public\\s+(?:file\\s+)?(?:static\\s+|sealed\\s+|abstract\\s+|partial\\s+|new\\s+)*?(?:class|record(?:\\s+class|\\s+struct)?|struct|interface|enum)\\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Multiline)]
    private static partial Regex PublicTypeRegex();
}
