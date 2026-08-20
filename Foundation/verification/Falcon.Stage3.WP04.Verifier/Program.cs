using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Reflection;
using System.Text;
using Foundation.Admission;
using Foundation.ContractRegistry;
using Foundation.Contracts;
using Foundation.DependencyGovernance;
using Foundation.ServiceCatalog;
using GovernanceDependencyDeclaration = Foundation.DependencyGovernance.DependencyDeclaration;

var failures = new List<string>();
AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
{
    var assemblyName = new AssemblyName(args.Name).Name;
    if (string.IsNullOrWhiteSpace(assemblyName))
    {
        return null;
    }

    var assemblyPath = Path.Combine(AppContext.BaseDirectory, assemblyName + ".dll");
    return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
};
_ = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Foundation.Contracts.dll"));

static void Expect(ICollection<string> failures, string label, bool condition, string? detail = null)
{
    if (!condition)
    {
        failures.Add(string.IsNullOrWhiteSpace(detail) ? $"{label} failed" : $"{label} failed: {detail}");
    }
}

static void ExpectPass(ICollection<string> failures, string label, DependencyValidationResult result)
{
    Expect(failures, label, result.Success, result.ReasonCode);
    Expect(failures, $"{label}-graph-decision", string.Equals(result.GraphDecision, "DEPENDENCY_GRAPH_VALIDATED", StringComparison.Ordinal), result.GraphDecision);
    Expect(failures, $"{label}-order-decision", string.Equals(result.ActivationOrderDecision, "ACTIVATION_ORDER_VALIDATED", StringComparison.Ordinal), result.ActivationOrderDecision);
    Expect(failures, $"{label}-events", result.EvidenceEvents.Count == 2, result.EvidenceEvents.Count.ToString(CultureInfo.InvariantCulture));
    Expect(failures, $"{label}-graph-digest", !string.IsNullOrWhiteSpace(result.GraphDigest), result.GraphDigest);
    Expect(failures, $"{label}-order-digest", !string.IsNullOrWhiteSpace(result.ActivationOrderDigest), result.ActivationOrderDigest);
}

static void ExpectFail(ICollection<string> failures, string label, DependencyValidationResult result, string expectedReason)
{
    Expect(failures, label, !result.Success, "unexpected PASS");
    Expect(failures, $"{label}-reason", string.Equals(result.ReasonCode, expectedReason, StringComparison.Ordinal), $"expected {expectedReason}, got {result.ReasonCode}");
    Expect(failures, $"{label}-no-events", result.EvidenceEvents.Count == 0, result.EvidenceEvents.Count.ToString(CultureInfo.InvariantCulture));
}

static string Sha256(string content)
    => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content ?? string.Empty)));

static ServiceManifest BuildManifest(
    string serviceIdentity,
    string serviceVersion,
    string owner,
    string purpose,
    string boundary,
    IEnumerable<ServiceResponsibilityDeclaration> responsibilities,
    IEnumerable<ServiceContractRequirement> providedContracts,
    IEnumerable<ServiceConsumedContractRequirement> consumedContracts,
    IEnumerable<string> authorizedConsumers,
    IEnumerable<string> restrictedConsumers,
    ServiceLifecycleDeclaration lifecycle,
    IEnumerable<ServiceDependencyDeclaration> dependencies,
    ServiceOperationalBoundary operationalBoundary,
    ServiceProtectionDeclaration protection,
    string manifestId,
    string packageIdentity,
    string packageVersion,
    string packageContent,
    string csaPolicy,
    string selfDevelopmentPath,
    string guardianInterface)
{
    return new ServiceManifest(
        serviceIdentity,
        serviceVersion,
        owner,
        purpose,
        boundary,
        responsibilities,
        providedContracts,
        consumedContracts,
        authorizedConsumers,
        restrictedConsumers,
        lifecycle,
        dependencies,
        operationalBoundary,
        protection,
        manifestId,
        packageIdentity,
        packageVersion,
        packageContent,
        csaPolicy,
        selfDevelopmentPath,
        guardianInterface);
}

static ServiceRegistrationRequest BuildRequest(
    string registrationId,
    string requesterIdentity,
    string serviceIdentity,
    string serviceVersion,
    string owner,
    ServiceKind serviceKind,
    ServiceRegistrationIntent intent,
    ServiceManifest manifest,
    string provenanceContent,
    string evidenceReference,
    DateTimeOffset observationTime)
{
    var manifestDigest = manifest.ComputeDigest();
    var envelope = new FilEnvelope(
        $"msg-{registrationId}",
        "COMMAND",
        "SERVICE_REGISTRATION_REQUEST",
        "foundation.service-registration",
        "1.0",
        requesterIdentity,
        observationTime,
        "governed service registration",
        "confidential",
        null,
        null,
        observationTime.AddHours(1),
        "governed-authority",
        "integrity-evidence",
        "protection-profile",
        "1.0",
        "integrity-scope",
        "encryption-scope",
        "key-ref",
        "1.0",
        null,
        "reject-on-replay",
        $"attempt-{registrationId}",
        "nonce",
        manifestDigest);

    return new ServiceRegistrationRequest
    {
        RegistrationId = registrationId,
        ServiceIdentity = serviceIdentity,
        ServiceVersion = serviceVersion,
        RequesterIdentity = requesterIdentity,
        AccountableOwner = owner,
        ServiceKind = serviceKind,
        Intent = intent,
        Manifest = manifest,
        RegistrationEnvelope = envelope,
        ManifestDigest = manifestDigest,
        ProvenanceContent = provenanceContent,
        ProvenanceDigest = Sha256(provenanceContent),
        RegistrationEvidenceReference = evidenceReference,
        ObservationTime = observationTime,
        ProviderEvidence = null
    };
}

static AdmissionRequest BuildAdmissionRequest(
    string admissionId,
    string kind,
    string identity,
    string version,
    string owner,
    string authoritySource,
    string contractVersion,
    string manifestId,
    ApplicationManifest manifest,
    string provenanceId,
    string provenanceContent,
    string bootstrapContextId,
    string bootstrapContextState,
    string providerBoundary,
    string decisionSeed)
{
    return new AdmissionRequest(
        admissionId,
        kind,
        identity,
        version,
        owner,
        authoritySource,
        "CON-023",
        contractVersion,
        manifestId,
        manifest,
        manifest.ComputeDigest(),
        provenanceId,
        provenanceContent,
        Sha256(provenanceContent),
        bootstrapContextId,
        bootstrapContextState,
        providerBoundary,
        decisionSeed);
}

static AdmissionDecision BuildAdmissionDecision(AdmissionRequest request, string evidenceId, string reason = "admission accepted")
    => new(
        request.AdmissionId,
        "ADMITTED",
        reason,
        request.ContractId,
        request.ContractVersion,
        evidenceId);

static ApplicationSubjectEvidence BuildApplicationSubject(ApplicationManifest manifest, AdmissionRequest request, AdmissionDecision decision, string evidenceReference)
    => new()
    {
        SubjectKind = DependencySubjectKind.Application,
        SubjectKey = new DependencySubjectKey(manifest.ApplicationIdentity, manifest.ApplicationVersion),
        EvidenceReference = evidenceReference,
        AdmissionRequest = request,
        AdmissionDecision = decision,
        Manifest = manifest,
        ManifestDigest = manifest.ComputeDigest(),
        AdmissionEvidenceIdentity = evidenceReference
    };

static DependencyGraphRequest BuildGraphRequest(
    string graphId,
    string graphVersion,
    string requesterIdentity,
    string authoritySource,
    DateTimeOffset observationTime,
    ManifestSurfaceRecord manifestSurface,
    DelegationRecord delegationEvidence,
    IReadOnlyList<DependencySubjectEvidence> subjects,
    IReadOnlyList<GovernanceDependencyDeclaration> dependencies,
    IReadOnlyList<DependencySubjectKey> proposedActivationOrder)
    => new()
    {
        GraphId = graphId,
        GraphVersion = graphVersion,
        RequesterIdentity = requesterIdentity,
        AuthoritySource = authoritySource,
        ObservationTime = observationTime,
        ManifestSurface = manifestSurface,
        DelegationEvidence = delegationEvidence,
        Subjects = subjects,
        Dependencies = dependencies,
        ProposedActivationOrder = proposedActivationOrder
    };

static AdmissionBaselineSnapshot BuildAdmissionBaseline()
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

static FoundationServiceSubjectEvidence BuildFoundationServiceSubject(ServiceCatalogEntry entry, string evidenceReference)
    => new()
    {
        SubjectKind = DependencySubjectKind.FoundationService,
        SubjectKey = new DependencySubjectKey(entry.Key.ServiceIdentity, entry.Key.ServiceVersion),
        EvidenceReference = evidenceReference,
        CatalogEntry = entry
    };

static ExternalDependencySubjectEvidence BuildExternalSubject(
    string identity,
    string version,
    string owner,
    string source,
    string evidenceReference,
    string digestSeed,
    DateTimeOffset effectiveTime,
    DateTimeOffset expiry)
    => new()
    {
        SubjectKind = DependencySubjectKind.External,
        SubjectKey = new DependencySubjectKey(identity, version),
        EvidenceReference = evidenceReference,
        Owner = owner,
        Source = source,
        IntegrityDigest = Sha256(digestSeed),
        AvailabilityResult = "AVAILABLE",
        ContainmentEvidence = "contained",
        EffectiveTime = effectiveTime,
        Expiry = expiry
    };

static ManifestSurfaceRecord BuildGraphManifest(string graphId, DateTimeOffset effectiveTime, DateTimeOffset expiry, string canonicalDigest)
    => new(
        "graph-manifest-001",
        ContractVersions.Con010,
        "CANDIDATE_MANIFEST",
        graphId,
        "graph-evidence-set-001",
        "SEPARATE",
        "INTACT",
        "GOV-090",
        "graph-manifest-validation",
        canonicalDigest,
        effectiveTime,
        expiry);

static DelegationRecord BuildDelegation(string requesterIdentity, string authoritySource, DateTimeOffset effectiveTime, DateTimeOffset expiry)
    => new(
        "delegation-001",
        "1.0",
        "Falcon Governance",
        requesterIdentity,
        "dependency graph validation;activation-order validation",
        "delegation-chain-001",
        authoritySource,
        "delegation-validation",
        "GRANTED",
        "revoked-by-authority",
        effectiveTime,
        expiry);

static string ComputeCandidateGraphDigest(DependencyGraphRequest request)
{
    var method = typeof(DependencyGovernanceValidator).GetMethod("SerializeCandidateGraphRequest", BindingFlags.NonPublic | BindingFlags.Static);
    if (method is null)
    {
        throw new InvalidOperationException("Unable to locate dependency-governance candidate graph serializer.");
    }

    var serialized = (string?)method.Invoke(null, new object[] { request });
    if (serialized is null)
    {
        throw new InvalidOperationException("Dependency-governance candidate graph serializer returned no content.");
    }

    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(serialized)));
}

static DependencyLifecycleOrderDeclaration BuildLifecycleOrder()
    => new(
        DependencyLifecycleOrderRule.DependencyBeforeConsumer,
        DependencyLifecycleOrderRule.ConsumerBeforeDependency,
        DependencyLifecycleOrderRule.DependencyBeforeConsumer,
        DependencyLifecycleOrderRule.DependencyBeforeConsumer,
        DependencyLifecycleOrderRule.ConsumerBeforeDependency);

static string BuildLifecycleOrderText()
{
    var method = typeof(DependencyGovernanceValidator).GetMethod("SerializeLifecycleOrder", BindingFlags.NonPublic | BindingFlags.Static);
    if (method is null)
    {
        throw new InvalidOperationException("Unable to locate dependency-governance lifecycle-order serializer.");
    }

    var serialized = (string?)method.Invoke(null, new object[] { BuildLifecycleOrder() });
    if (serialized is null)
    {
        throw new InvalidOperationException("Dependency-governance lifecycle-order serializer returned no content.");
    }

    return serialized;
}

static DependencyGraphRequest BuildValidRequest(
    ServiceCatalogEntry serviceEntry,
    AdmissionRequest admissionRequest,
    AdmissionDecision admissionDecision,
    ApplicationManifest applicationManifest,
    DateTimeOffset observationTime)
{
    const string graphId = "dependency-governance-graph";
    var graphManifest = BuildGraphManifest(graphId, observationTime.AddMinutes(-5), observationTime.AddHours(2), string.Empty);
    var graphRequest = new DependencyGraphRequest
    {
        GraphId = graphId,
        GraphVersion = "1.0",
        RequesterIdentity = "graph-governance-requester",
        AuthoritySource = "GOV-090",
        ObservationTime = observationTime,
        ManifestSurface = graphManifest,
        DelegationEvidence = BuildDelegation("graph-governance-requester", "GOV-090", observationTime.AddMinutes(-10), observationTime.AddHours(2)),
        Subjects = new DependencySubjectEvidence[]
        {
            BuildExternalSubject(
                "CON-023",
                "1.1",
                "Falcon Application Authority",
                "contract-reference",
                "evidence/external/contract-con-023",
                "external-contract-con-023",
                observationTime.AddMinutes(-20),
                observationTime.AddHours(2)),
            BuildExternalSubject(
                "time-source-001",
                "1.0",
                "Falcon Foundation",
                "time-provider-evidence",
                "evidence/external/time-source-001",
                "external-time-source-001",
                observationTime.AddMinutes(-20),
                observationTime.AddHours(2)),
            BuildFoundationServiceSubject(serviceEntry, "evidence/service/dependency-governance"),
            BuildApplicationSubject(applicationManifest, admissionRequest, admissionDecision, "evidence/admission/graph")
        },
        Dependencies = new GovernanceDependencyDeclaration[]
        {
            new GovernanceDependencyDeclaration
            {
                Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                DependencyIdentity = "time-source-001",
                CompatibleVersions = new[] { "1.0" },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = "1.0",
                DependencyKind = DependencySubjectKind.External,
                DependencySource = "ServiceManifest",
                DeclaredPurpose = "time source dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                DegradedStatePolicy = "isolate",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new GovernanceDependencyDeclaration
            {
                Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                DependencyIdentity = "CON-023",
                CompatibleVersions = new[] { "1.1" },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = "1.1",
                DependencyKind = DependencySubjectKind.External,
                DependencySource = "ServiceManifest",
                DeclaredPurpose = "application contract dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                IsolationBoundary = "external",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                DegradedStatePolicy = "isolate",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new GovernanceDependencyDeclaration
            {
                Consumer = new DependencySubjectKey(applicationManifest.ApplicationIdentity, applicationManifest.ApplicationVersion),
                DependencyIdentity = "CON-023",
                CompatibleVersions = new[] { "1.1" },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = "1.1",
                DependencyKind = DependencySubjectKind.External,
                DependencySource = "ApplicationManifest",
                DeclaredPurpose = "application contract dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                IsolationBoundary = "external",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                DegradedStatePolicy = "isolate",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new GovernanceDependencyDeclaration
            {
                Consumer = new DependencySubjectKey(applicationManifest.ApplicationIdentity, applicationManifest.ApplicationVersion),
                DependencyIdentity = serviceEntry.Key.ServiceIdentity,
                CompatibleVersions = new[] { serviceEntry.Key.ServiceVersion },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = serviceEntry.Key.ServiceVersion,
                DependencyKind = DependencySubjectKind.FoundationService,
                DependencySource = "ApplicationManifest",
                DeclaredPurpose = "governed dependency graph validation support",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                DegradedStatePolicy = "isolate",
                LifecycleOrder = BuildLifecycleOrder()
            }
        }
    };

    graphRequest = graphRequest with
    {
        ProposedActivationOrder = new[]
        {
            new DependencySubjectKey("time-source-001", "1.0"),
            new DependencySubjectKey("CON-023", "1.1"),
            new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
            new DependencySubjectKey(applicationManifest.ApplicationIdentity, applicationManifest.ApplicationVersion)
        }
    };

    graphRequest = graphRequest with
    {
        ManifestSurface = graphManifest with { CanonicalDigest = ComputeCandidateGraphDigest(graphRequest) }
    };

    return graphRequest;
}

static DependencyGraphRequest BuildScenarioRequest(
    string graphId,
    string graphVersion,
    string requesterIdentity,
    string authoritySource,
    DateTimeOffset observationTime,
    IReadOnlyList<DependencySubjectEvidence> subjects,
    IReadOnlyList<GovernanceDependencyDeclaration> dependencies,
    IReadOnlyList<DependencySubjectKey> activationOrder,
    string? manifestDigest = null)
{
    var manifest = BuildGraphManifest(graphId, observationTime.AddMinutes(-5), observationTime.AddHours(2), string.Empty);
    var reboundSubjects = BindSubjectsToDependencies(subjects, dependencies);
    var canonicalActivationOrder = ComputeFixtureCanonicalActivationOrder(reboundSubjects, dependencies);
    var request = BuildGraphRequest(
        graphId,
        graphVersion,
        requesterIdentity,
        authoritySource,
        observationTime,
        manifest,
        BuildDelegation(requesterIdentity, authoritySource, observationTime.AddMinutes(-10), observationTime.AddHours(2)),
        reboundSubjects,
        dependencies,
        canonicalActivationOrder);

    return request with
    {
        ManifestSurface = request.ManifestSurface with { CanonicalDigest = manifestDigest ?? ComputeCandidateGraphDigest(request) }
    };
}

DependencyGraphRequest CreatePositiveScenarioRequest(
    int scenario,
    ServiceCatalogEntry serviceEntry,
    AdmissionRequest admissionRequest,
    AdmissionDecision admissionDecision,
    ApplicationManifest applicationManifest,
    ApplicationManifest applicationServiceManifest,
    DateTimeOffset observationTime,
    DependencyGraphRequest validRequest)
{
    var appThreeManifest = applicationManifest with
    {
        ApplicationIdentity = "app-3",
        ApplicationVersion = "3.0",
        ApplicationOwner = "Example Application Owner Three",
        ManifestId = "manifest-app-3",
        PackageIdentity = "pkg-app-3",
        PackageVersion = "3.0",
        RequiredFoundationServices = new ReadOnlyCollection<FoundationServiceRequirement>(new[]
        {
            new FoundationServiceRequirement(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion, "governed dependency graph validation support")
        })
    };
    var appThreeAdmissionRequest = CreateAdmissionRequest(appThreeManifest, observationTime);
    var appThreeAdmissionDecision = BuildAdmissionDecision(appThreeAdmissionRequest, "evidence/admission/003");
    var appThreeSubject = BuildApplicationSubject(appThreeManifest, appThreeAdmissionRequest, appThreeAdmissionDecision, "evidence/admission/003");

    var serviceOnlySubject = BuildFoundationServiceSubject(serviceEntry, "evidence/service/dependency-governance");
    var externalTimeSubject = BuildExternalSubject("time-source-001", "1.0", "Falcon Foundation", "time-provider-evidence", "evidence/external/time-source-001", "external-time-source-001", observationTime.AddMinutes(-20), observationTime.AddHours(2));
    var externalConSubject = BuildExternalSubject("CON-023", "1.1", "Falcon Application Authority", "contract-reference", "evidence/external/contract-con-023", "external-contract-con-023", observationTime.AddMinutes(-20), observationTime.AddHours(2));
    var externalDegradedSubject = BuildExternalSubject("degraded-external-001", "1.0", "Falcon Foundation", "degraded-reference", "evidence/external/degraded-external-001", "degraded-external-001", observationTime.AddMinutes(-20), observationTime.AddHours(2));
    var appTwoManifest = applicationServiceManifest with
    {
        ApplicationIdentity = "app-2",
        ApplicationVersion = "2.0",
        ApplicationOwner = "Example Application Owner Two",
        ManifestId = "manifest-app-2",
        PackageIdentity = "pkg-app-2",
        PackageVersion = "2.0"
    };
    var appTwoAdmissionRequest = CreateAdmissionRequest(appTwoManifest, observationTime);
    var appTwoAdmissionDecision = BuildAdmissionDecision(appTwoAdmissionRequest, "evidence/admission/002");
    var appTwoSubject = BuildApplicationSubject(appTwoManifest, appTwoAdmissionRequest, appTwoAdmissionDecision, "evidence/admission/002");
    var independentFoundationEntry = CloneServiceEntry(serviceEntry, "foundation-dependency-governance-service-2", "1.1", "manifest-foundation-dependency-governance-service-2", "foundation-dependency-governance-package-2", "1.1", "evidence/service/dependency-governance-2");
    var independentFoundationSubject = BuildFoundationServiceSubject(independentFoundationEntry, "evidence/service/dependency-governance-2");

    switch (scenario)
    {
        case 1:
            return BuildScenarioRequest(
                "dependency-free-service",
                "1.0",
                "graph-governance-requester-01",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { serviceOnlySubject },
                Array.Empty<GovernanceDependencyDeclaration>(),
                new[] { new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion) });

        case 2:
            return BuildScenarioRequest(
                "two-subject-required-chain",
                "1.0",
                "graph-governance-requester-02",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { externalTimeSubject, serviceOnlySubject },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                    new DependencySubjectKey("time-source-001", "1.0")
                });

        case 3:
            return BuildScenarioRequest(
                "three-subject-required-chain",
                "1.0",
                "graph-governance-requester-03",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { externalConSubject, serviceOnlySubject, BuildApplicationSubject(applicationManifest, admissionRequest, admissionDecision, "evidence/admission/001") },
                new GovernanceDependencyDeclaration[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "CON-023",
                        CompatibleVersions = new[] { "1.1" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.1",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "application contract dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "external",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    },
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(applicationManifest.ApplicationIdentity, applicationManifest.ApplicationVersion),
                        DependencyIdentity = serviceEntry.Key.ServiceIdentity,
                        CompatibleVersions = new[] { serviceEntry.Key.ServiceVersion },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = serviceEntry.Key.ServiceVersion,
                        DependencyKind = DependencySubjectKind.FoundationService,
                        DependencySource = "ApplicationManifest",
                        DeclaredPurpose = "governed dependency graph validation support",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("CON-023", "1.1"),
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                    new DependencySubjectKey(applicationManifest.ApplicationIdentity, applicationManifest.ApplicationVersion)
                });

        case 4:
            return BuildScenarioRequest(
                "diamond-graph",
                "1.0",
                "graph-governance-requester-04",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[]
                {
                    externalConSubject,
                    BuildFoundationServiceSubject(CloneServiceEntry(serviceEntry, "foundation-dependency-governance-service-a", "1.0", "manifest-foundation-dependency-governance-service-a", "foundation-dependency-governance-package-a", "1.0", "evidence/service/diamond-a"), "evidence/service/diamond-a"),
                    BuildFoundationServiceSubject(CloneServiceEntry(serviceEntry, "foundation-dependency-governance-service-b", "1.0", "manifest-foundation-dependency-governance-service-b", "foundation-dependency-governance-package-b", "1.0", "evidence/service/diamond-b"), "evidence/service/diamond-b"),
                    appThreeSubject
                },
                new GovernanceDependencyDeclaration[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey("foundation-dependency-governance-service-a", "1.0"),
                        DependencyIdentity = "CON-023",
                        CompatibleVersions = new[] { "1.1" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.1",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "diamond external dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "external",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    },
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey("foundation-dependency-governance-service-b", "1.0"),
                        DependencyIdentity = "CON-023",
                        CompatibleVersions = new[] { "1.1" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.1",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "diamond external dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "external",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    },
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(appThreeSubject.SubjectKey.Identity, appThreeSubject.SubjectKey.Version),
                        DependencyIdentity = "foundation-dependency-governance-service-a",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.FoundationService,
                        DependencySource = "ApplicationManifest",
                        DeclaredPurpose = "diamond branch one",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    },
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(appThreeSubject.SubjectKey.Identity, appThreeSubject.SubjectKey.Version),
                        DependencyIdentity = "foundation-dependency-governance-service-b",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.FoundationService,
                        DependencySource = "ApplicationManifest",
                        DeclaredPurpose = "diamond branch two",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("CON-023", "1.1"),
                    new DependencySubjectKey("foundation-dependency-governance-service-a", "1.0"),
                    new DependencySubjectKey("foundation-dependency-governance-service-b", "1.0"),
                    new DependencySubjectKey(applicationManifest.ApplicationIdentity, applicationManifest.ApplicationVersion)
                });

        case 5:
            return BuildScenarioRequest(
                "ordinal-tie-breaking",
                "1.0",
                "graph-governance-requester-05",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[]
                {
                    BuildFoundationServiceSubject(CloneServiceEntry(serviceEntry, "a-service", "1.0", "manifest-a-service", "package-a-service", "1.0", "evidence/service/a-service"), "evidence/service/a-service"),
                    externalTimeSubject,
                    appThreeSubject
                },
                Array.Empty<GovernanceDependencyDeclaration>(),
                new[]
                {
                    new DependencySubjectKey("a-service", "1.0"),
                    new DependencySubjectKey("time-source-001", "1.0"),
                    new DependencySubjectKey(appThreeSubject.SubjectKey.Identity, appThreeSubject.SubjectKey.Version)
                });

        case 6:
            return BuildScenarioRequest(
                "service-to-service",
                "1.0",
                "graph-governance-requester-06",
                "GOV-090",
                observationTime,
                new[]
                {
                    BuildFoundationServiceSubject(CloneServiceEntry(serviceEntry, "service-alpha", "1.0", "manifest-service-alpha", "package-service-alpha", "1.0", "evidence/service/service-alpha"), "evidence/service/service-alpha"),
                    BuildFoundationServiceSubject(CloneServiceEntry(serviceEntry, "service-beta", "1.0", "manifest-service-beta", "package-service-beta", "1.0", "evidence/service/service-beta"), "evidence/service/service-beta")
                },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey("service-beta", "1.0"),
                        DependencyIdentity = "service-alpha",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.FoundationService,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "foundation service dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("service-alpha", "1.0"),
                    new DependencySubjectKey("service-beta", "1.0")
                });

        case 7:
            return BuildScenarioRequest(
                "application-to-foundation-service",
                "1.0",
                "graph-governance-requester-07",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[]
                {
                    BuildExternalSubject("CON-023", "1.1", "Falcon Application Authority", "contract-reference", "evidence/external/contract-con-023", "external-contract-con-023", observationTime.AddMinutes(-20), observationTime.AddHours(2)),
                    BuildExternalSubject("time-source-001", "1.0", "Falcon Foundation", "time-provider-evidence", "evidence/external/time-source-001", "external-time-source-001", observationTime.AddMinutes(-20), observationTime.AddHours(2)),
                    BuildFoundationServiceSubject(serviceEntry, "evidence/service/dependency-governance"),
                    appTwoSubject
                },
                new GovernanceDependencyDeclaration[]
                {
                    new()
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    },
                    new()
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "CON-023",
                        CompatibleVersions = new[] { "1.1" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.1",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "application contract dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "external",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    },
                    new()
                    {
                        Consumer = new DependencySubjectKey(appTwoSubject.SubjectKey.Identity, appTwoSubject.SubjectKey.Version),
                        DependencyIdentity = serviceEntry.Key.ServiceIdentity,
                        CompatibleVersions = new[] { serviceEntry.Key.ServiceVersion },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = serviceEntry.Key.ServiceVersion,
                        DependencyKind = DependencySubjectKind.FoundationService,
                        DependencySource = "ApplicationManifest",
                        DeclaredPurpose = "governed dependency graph validation support",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("CON-023", "1.1"),
                    new DependencySubjectKey("time-source-001", "1.0"),
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                    new DependencySubjectKey(appTwoSubject.SubjectKey.Identity, appTwoSubject.SubjectKey.Version)
                });

        case 8:
            return BuildScenarioRequest(
                "application-to-application",
                "1.0",
                "graph-governance-requester-08",
                "GOV-090",
                observationTime,
                new[]
                {
                    BuildApplicationSubject(applicationManifest, admissionRequest, admissionDecision, "evidence/admission/001"),
                    appTwoSubject
                },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(appTwoSubject.SubjectKey.Identity, appTwoSubject.SubjectKey.Version),
                        DependencyIdentity = applicationManifest.ApplicationIdentity,
                        CompatibleVersions = new[] { applicationManifest.ApplicationVersion },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = applicationManifest.ApplicationVersion,
                        DependencyKind = DependencySubjectKind.Application,
                        DependencySource = "ApplicationManifest",
                        DeclaredPurpose = "application dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey(applicationManifest.ApplicationIdentity, applicationManifest.ApplicationVersion),
                    new DependencySubjectKey(appTwoSubject.SubjectKey.Identity, appTwoSubject.SubjectKey.Version)
                });

        case 9:
            return BuildScenarioRequest(
                "available-external-dependency",
                "1.0",
                "graph-governance-requester-09",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { serviceOnlySubject, externalTimeSubject },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("time-source-001", "1.0"),
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion)
                });

        case 10:
            return BuildScenarioRequest(
                "resolved-optional-dependency",
                "1.0",
                "graph-governance-requester-10",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { serviceOnlySubject, externalTimeSubject },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Optional,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "degrade-inline",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("time-source-001", "1.0"),
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion)
                });

        case 11:
            return BuildScenarioRequest(
                "unresolved-optional-dependency",
                "1.0",
                "graph-governance-requester-11",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { serviceOnlySubject },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Optional,
                        ConditionState = null,
                        ResolvedVersion = null,
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "degrade-inline",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion)
                });

        case 12:
            return BuildScenarioRequest(
                "conditional-required-now",
                "1.0",
                "graph-governance-requester-12",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { serviceOnlySubject, externalTimeSubject },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Conditional,
                        ConditionState = DependencyConditionState.RequiredNow,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("time-source-001", "1.0"),
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion)
                });

        case 13:
            return BuildScenarioRequest(
                "conditional-not-required-now",
                "1.0",
                "graph-governance-requester-13",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { serviceOnlySubject },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0" },
                        Relationship = DependencyRelationship.Conditional,
                        ConditionState = DependencyConditionState.NotRequiredNow,
                        ResolvedVersion = null,
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "degrade-inline",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion)
                });

        case 14:
            return BuildScenarioRequest(
                "prohibited-absent",
                "1.0",
                "graph-governance-requester-14",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[] { serviceOnlySubject },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "CON-023",
                        CompatibleVersions = new[] { "1.1" },
                        Relationship = DependencyRelationship.Prohibited,
                        ConditionState = null,
                        ResolvedVersion = null,
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "application contract dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "external",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("CON-023", "1.1"),
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion)
                });

        case 15:
            return BuildScenarioRequest(
                "multiple-version-resolution",
                "1.0",
                "graph-governance-requester-15",
                "GOV-090",
                observationTime,
                new DependencySubjectEvidence[]
                {
                    serviceOnlySubject,
                    externalTimeSubject,
                    BuildExternalSubject(
                        "time-source-001",
                        "1.1",
                        "Falcon Foundation",
                        "time-provider-evidence-v1.1",
                        "evidence/external/time-source-001-v1.1",
                        "external-time-source-001-v1.1",
                        observationTime.AddMinutes(-20),
                        observationTime.AddHours(2))
                },
                new[]
                {
                    new GovernanceDependencyDeclaration
                    {
                        Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                        DependencyIdentity = "time-source-001",
                        CompatibleVersions = new[] { "1.0", "1.1" },
                        Relationship = DependencyRelationship.Required,
                        ConditionState = null,
                        ResolvedVersion = "1.0",
                        DependencyKind = DependencySubjectKind.External,
                        DependencySource = "ServiceManifest",
                        DeclaredPurpose = "time source dependency",
                        IntegrityRequirement = "integrity-required",
                        AvailabilityRequirement = "available",
                        TimeoutPolicy = "bounded",
                        DegradedStatePolicy = "isolate",
                        IsolationBoundary = "contained",
                        FailurePropagationLimit = "bounded",
                        ReplacementPolicy = "explicit",
                        MigrationPolicy = "manual",
                        RollbackPolicy = "rollback-allowed",
                        EvidenceRequirement = "required",
                        DelegationChainEvidenceReference = "delegation-chain-001",
                        LifecycleOrder = BuildLifecycleOrder()
                    }
                },
                new[]
                {
                    new DependencySubjectKey("time-source-001", "1.0"),
                    new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion)
                });

        case 16:
            return validRequest;

        case 17:
            return validRequest;

        case 18:
            return validRequest;

        case 19:
            return validRequest;

        default:
            throw new InvalidOperationException($"Unsupported positive scenario {scenario}.");
    }
}

static ServiceCatalogEntry CreateServiceCatalogEntry(DateTimeOffset observationTime, ICollection<string> failures)
{
    var serviceIdentity = "foundation-dependency-governance-service";
    var serviceVersion = "1.0";
    var serviceManifest = BuildManifest(
        serviceIdentity,
        serviceVersion,
        "Falcon Foundation",
        "governed dependency graph validation support",
        "bounded dependency governance control",
        new[]
        {
            new ServiceResponsibilityDeclaration("dependency-graph-validation", "Falcon Foundation", "dependency graph validation"),
            new ServiceResponsibilityDeclaration("activation-order-validation", "Falcon Foundation", "activation order validation")
        },
        Array.Empty<ServiceContractRequirement>(),
        new[]
        {
            new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "application contract dependency", "requires")
        },
        new[] { "foundation-runtime" },
        new[] { "external-guest" },
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        new[]
        {
            new ServiceDependencyDeclaration("time-source-001", new[] { "1.0" }, "external", "requires", "time source dependency", "isolate", BuildLifecycleOrderText()),
            new ServiceDependencyDeclaration("CON-023", new[] { "1.1" }, "external", "requires", "application contract dependency", "isolate", BuildLifecycleOrderText())
        },
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-foundation-dependency-governance-service-1.0",
        "foundation-dependency-governance-package",
        "1.0",
        "package-content",
        "package governed only by approved dependency governance",
        "controlled self-development path",
        "guardian interface declared");

    var catalog = new ServiceCatalog();
    var request = BuildRequest(
        "svc-reg-dependency-governance",
        serviceIdentity,
        serviceIdentity,
        serviceVersion,
        "Falcon Foundation",
        ServiceKind.GeneralFoundationService,
        new ServiceRegistrationIntent(RegistrationMode.Explicit, false, false, false, false, false, false),
        serviceManifest,
        "service-dependency-governance-provenance",
        "evidence/service/dependency-governance",
        observationTime);

    var result = catalog.Register(request);
    Expect(failures, "service-registration", result.Success && result.Decision == ServiceRegistrationDecision.Registered, result.ReasonCode);
    var lookup = catalog.Lookup(serviceIdentity, serviceVersion);
    Expect(failures, "service-lookup", lookup is not null, "missing service entry");
    return lookup!.Entry;
}

static ApplicationManifest CreateApplicationManifest()
    => new(
        "manifest-app-1",
        "app-1",
        "1.0",
        "Example Application Owner",
        "governed application admission",
        "pkg-app-1",
        "1.0",
        "package-content-app-1",
        new[]
        {
            new Foundation.Admission.DependencyDeclaration("CON-023", new[] { "1.1" })
        },
        new[]
        {
            new FoundationRequirement("CON-023", "1.1", "Falcon Application Authority", "CON-000 / CON-023")
        },
        new[]
        {
            new FoundationRequirement("APP-001", "1.1", "Falcon Application Authority", "Falcon Application Authority")
        },
        new[]
        {
            new FoundationServiceRequirement("Service Catalog", "1.0", "registration")
        },
        new[] { "admission" },
        new[] { "foundation-runtime" },
        new[]
        {
            new PermissionDeclaration("admission.request", "governed entry", "required for controlled admission")
        },
        new[]
        {
            new Foundation.Admission.AuthorityRequest("admission.authority", "controlled admission", "requested authority to submit governed admission")
        },
        new SecurityProfile("standard", "confidential", "bounded"),
        new ResourceRequirements("256MiB", "0.25 CPU", "128MiB", "offline"),
        new ResourceRequirements("1GiB", "1 CPU", "1GiB", "offline"),
        "reject on degraded prerequisite failure",
        "no hidden persistence",
        "controlled outbound only",
        "explicit governed configuration",
        "raw evidence retained",
        new LifecycleBehavior(
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
        "governed health reporting",
        "bounded failure containment",
        true,
        new[]
        {
            new MsaDeclaration("MSA-1", "Example Application Owner", "main application surface")
        },
        new[]
        {
            new MajorBranchDeclaration("branch-a", "primary runtime branch", "LSA-A"),
            new MajorBranchDeclaration("branch-b", "secondary runtime branch", "LSA-B")
        },
        new[]
        {
            new LsaDeclaration("branch-a", "LSA-A", "branch-a responsibility"),
            new LsaDeclaration("branch-b", "LSA-B", "branch-b responsibility")
        },
        "CSA eligible only for explicit intelligent components",
        "application owner to governed foundation review",
        "guardian interface declared",
        "rollback to last accepted governed package");

static AdmissionRequest CreateAdmissionRequest(ApplicationManifest manifest, DateTimeOffset observationTime)
    => BuildAdmissionRequest(
        "adm-app-1",
        "APPLICATION",
        manifest.ApplicationIdentity,
        manifest.ApplicationVersion,
        manifest.ApplicationOwner,
        "CON-000 / CON-023",
        "1.1",
        manifest.ManifestId,
        manifest,
        "prov-app-1",
        "provenance-content-app-1",
        "ctx-1",
        "DEFINED",
        "boundary-ok",
        "seed-1");

static GovernanceDependencyDeclaration CloneDependency(GovernanceDependencyDeclaration dependency, string? resolvedVersion = null, DependencyRelationship? relationship = null, DependencyConditionState? conditionState = null, string? dependencyIdentity = null, IReadOnlyList<string>? versions = null)
    => dependency with
    {
        ResolvedVersion = resolvedVersion ?? dependency.ResolvedVersion,
        Relationship = relationship ?? dependency.Relationship,
        ConditionState = conditionState ?? dependency.ConditionState,
        DependencyIdentity = dependencyIdentity ?? dependency.DependencyIdentity,
        CompatibleVersions = versions ?? dependency.CompatibleVersions
    };

static DependencySubjectEvidence CloneSubject(DependencySubjectEvidence subject)
    => subject switch
    {
        FoundationServiceSubjectEvidence foundation => foundation with { },
        ApplicationSubjectEvidence application => application with { },
        ExternalDependencySubjectEvidence external => external with { },
        _ => subject
    };

static ServiceCatalogEntry CloneServiceEntry(
    ServiceCatalogEntry template,
    string serviceIdentity,
    string serviceVersion,
    string manifestId,
    string packageIdentity,
    string packageVersion,
    string evidenceReference)
{
    var catalogKey = ServiceCatalogKey.From(serviceIdentity, serviceVersion);
    var manifest = template.Manifest with
    {
        ServiceIdentity = serviceIdentity,
        ServiceVersion = serviceVersion,
        ManifestId = manifestId,
        PackageIdentity = packageIdentity,
        PackageVersion = packageVersion
    };

    return template with
    {
        Key = catalogKey,
        Manifest = manifest,
        Registration = template.Registration with
        {
            CatalogKey = catalogKey,
            ManifestDigest = manifest.ComputeDigest(),
            RegistrationEvidenceReference = evidenceReference
        }
    };
}


static string FixtureCanonical(string value)
    => $"{(value ?? string.Empty).Length}:{value ?? string.Empty}";

static string SerializeFixtureLifecycleOrder(DependencyLifecycleOrderDeclaration? lifecycleOrder)
    => lifecycleOrder is null
        ? string.Empty
        : string.Join("|", new[]
        {
            FixtureCanonical(lifecycleOrder.Startup.ToString()),
            FixtureCanonical(lifecycleOrder.Shutdown.ToString()),
            FixtureCanonical(lifecycleOrder.Update.ToString()),
            FixtureCanonical(lifecycleOrder.Recovery.ToString()),
            FixtureCanonical(lifecycleOrder.Removal.ToString())
        });

static string ToFixtureManifestKind(DependencySubjectKind kind)
    => kind switch
    {
        DependencySubjectKind.FoundationService => "foundation-service",
        DependencySubjectKind.Application => "application",
        DependencySubjectKind.External => "external",
        _ => kind.ToString().ToLowerInvariant()
    };

static string ToFixtureManifestRelationship(DependencyRelationship relationship)
    => relationship switch
    {
        DependencyRelationship.Required => "requires",
        DependencyRelationship.Optional => "optional",
        DependencyRelationship.Conditional => "conditional",
        DependencyRelationship.Prohibited => "prohibited",
        _ => relationship.ToString().ToLowerInvariant()
    };

static IReadOnlyList<DependencySubjectEvidence> BindSubjectsToDependencies(
    IReadOnlyList<DependencySubjectEvidence> subjects,
    IReadOnlyList<GovernanceDependencyDeclaration> dependencies)
{
    var rebound = new List<DependencySubjectEvidence>(subjects.Count);

    foreach (var subject in subjects)
    {
        var sourceDependencies = dependencies
            .Where(dependency =>
                string.Equals(dependency.Consumer.Identity, subject.SubjectKey.Identity, StringComparison.Ordinal) &&
                string.Equals(dependency.Consumer.Version, subject.SubjectKey.Version, StringComparison.Ordinal))
            .ToArray();

        switch (subject)
        {
            case FoundationServiceSubjectEvidence foundation:
            {
                var manifestDependencies = sourceDependencies
                    .Select(dependency => new ServiceDependencyDeclaration(
                        dependency.DependencyIdentity,
                        dependency.CompatibleVersions.ToArray(),
                        ToFixtureManifestKind(dependency.DependencyKind),
                        ToFixtureManifestRelationship(dependency.Relationship),
                        dependency.DeclaredPurpose,
                        dependency.DegradedStatePolicy,
                        SerializeFixtureLifecycleOrder(dependency.LifecycleOrder)))
                    .ToArray();

                var manifest = foundation.CatalogEntry.Manifest with
                {
                    Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(manifestDependencies)
                };

                var entry = foundation.CatalogEntry with
                {
                    Manifest = manifest,
                    Registration = foundation.CatalogEntry.Registration with
                    {
                        CatalogKey = foundation.CatalogEntry.Key,
                        ManifestDigest = manifest.ComputeDigest()
                    }
                };

                rebound.Add(foundation with { CatalogEntry = entry });
                break;
            }

            case ApplicationSubjectEvidence application:
            {
                var declaredDependencies = sourceDependencies
                    .Where(dependency => dependency.DependencyKind != DependencySubjectKind.FoundationService)
                    .Select(dependency => new Foundation.Admission.DependencyDeclaration(
                        dependency.DependencyIdentity,
                        dependency.CompatibleVersions.ToArray()))
                    .ToArray();

                var requiredServices = sourceDependencies
                    .Where(dependency => dependency.DependencyKind == DependencySubjectKind.FoundationService)
                    .Select(dependency => new FoundationServiceRequirement(
                        dependency.DependencyIdentity,
                        dependency.ResolvedVersion ?? dependency.CompatibleVersions.OrderBy(version => version, StringComparer.Ordinal).First(),
                        dependency.DeclaredPurpose))
                    .ToArray();

                var manifest = application.Manifest with
                {
                    DeclaredDependencies = new ReadOnlyCollection<Foundation.Admission.DependencyDeclaration>(declaredDependencies),
                    RequiredFoundationServices = new ReadOnlyCollection<FoundationServiceRequirement>(requiredServices)
                };
                var manifestDigest = manifest.ComputeDigest();
                var admissionRequest = application.AdmissionRequest with
                {
                    Identity = manifest.ApplicationIdentity,
                    Version = manifest.ApplicationVersion,
                    Owner = manifest.ApplicationOwner,
                    ManifestId = manifest.ManifestId,
                    Manifest = manifest,
                    ManifestDigest = manifestDigest
                };
                var admissionDecision = application.AdmissionDecision with
                {
                    AdmissionId = admissionRequest.AdmissionId,
                    ContractId = admissionRequest.ContractId,
                    ContractVersion = admissionRequest.ContractVersion
                };

                rebound.Add(application with
                {
                    AdmissionRequest = admissionRequest,
                    AdmissionDecision = admissionDecision,
                    Manifest = manifest,
                    ManifestDigest = manifestDigest
                });
                break;
            }

            default:
                rebound.Add(subject);
                break;
        }
    }

    return new ReadOnlyCollection<DependencySubjectEvidence>(rebound);
}

static string FixtureNodeText(DependencySubjectEvidence subject)
    => $"{subject.SubjectKind}|{subject.SubjectKey.Identity.Length}:{subject.SubjectKey.Identity}|{subject.SubjectKey.Version.Length}:{subject.SubjectKey.Version}";

static IReadOnlyList<DependencySubjectKey> ComputeFixtureCanonicalActivationOrder(
    IReadOnlyList<DependencySubjectEvidence> subjects,
    IReadOnlyList<GovernanceDependencyDeclaration> dependencies)
{
    var nodeTextBySubject = subjects.ToDictionary(subject => subject, FixtureNodeText);
    var subjectByNodeText = subjects.ToDictionary(FixtureNodeText, subject => subject, StringComparer.Ordinal);
    var inDegree = subjectByNodeText.Keys.ToDictionary(key => key, _ => 0, StringComparer.Ordinal);
    var adjacency = subjectByNodeText.Keys.ToDictionary(key => key, _ => new List<string>(), StringComparer.Ordinal);

    foreach (var dependency in dependencies)
    {
        var contributesEdge =
            dependency.Relationship == DependencyRelationship.Required ||
            (dependency.Relationship == DependencyRelationship.Optional && !string.IsNullOrWhiteSpace(dependency.ResolvedVersion)) ||
            (dependency.Relationship == DependencyRelationship.Conditional && dependency.ConditionState == DependencyConditionState.RequiredNow);

        if (!contributesEdge)
        {
            continue;
        }

        var consumer = subjects.Single(subject =>
            string.Equals(subject.SubjectKey.Identity, dependency.Consumer.Identity, StringComparison.Ordinal) &&
            string.Equals(subject.SubjectKey.Version, dependency.Consumer.Version, StringComparison.Ordinal));

        var resolvedVersion = dependency.ResolvedVersion ?? dependency.CompatibleVersions[0];
        var dependencySubject = subjects.Single(subject =>
            subject.SubjectKind == dependency.DependencyKind &&
            string.Equals(subject.SubjectKey.Identity, dependency.DependencyIdentity, StringComparison.Ordinal) &&
            string.Equals(subject.SubjectKey.Version, resolvedVersion, StringComparison.Ordinal));

        var dependencyNode = nodeTextBySubject[dependencySubject];
        var consumerNode = nodeTextBySubject[consumer];
        adjacency[dependencyNode].Add(consumerNode);
        inDegree[consumerNode]++;
    }

    var queue = new SortedSet<string>(
        inDegree.Where(item => item.Value == 0).Select(item => item.Key),
        StringComparer.Ordinal);
    var ordered = new List<DependencySubjectKey>(subjects.Count);

    while (queue.Count > 0)
    {
        var current = queue.Min!;
        queue.Remove(current);
        var subject = subjectByNodeText[current];
        ordered.Add(subject.SubjectKey);

        foreach (var consumer in adjacency[current].OrderBy(value => value, StringComparer.Ordinal))
        {
            inDegree[consumer]--;
            if (inDegree[consumer] == 0)
            {
                queue.Add(consumer);
            }
        }
    }

    if (ordered.Count != subjects.Count)
    {
        throw new InvalidOperationException("Fixture activation order cannot be calculated for a cyclic graph.");
    }

    return new ReadOnlyCollection<DependencySubjectKey>(ordered);
}

static DependencyGraphRequest BindRequestManifestCoverage(
    DependencyGraphRequest request,
    bool canonicalizeActivationOrder)
{
    var subjects = BindSubjectsToDependencies(request.Subjects, request.Dependencies);
    var aligned = request with
    {
        Subjects = subjects,
        ProposedActivationOrder = canonicalizeActivationOrder
            ? ComputeFixtureCanonicalActivationOrder(subjects, request.Dependencies)
            : request.ProposedActivationOrder
    };

    return aligned with
    {
        ManifestSurface = aligned.ManifestSurface with
        {
            CanonicalDigest = ComputeCandidateGraphDigest(aligned)
        }
    };
}

static DependencyValidationResult ValidateNoThrow(
    ICollection<string> failures,
    string label,
    DependencyGraphRequest request)
{
    try
    {
        var result = new DependencyGovernanceValidator().Validate(request);
        Expect(failures, $"{label}-no-exception", true);
        return result;
    }
    catch (Exception exception)
    {
        Expect(failures, $"{label}-no-exception", false, $"{exception.GetType().Name}: {exception.Message}");
        return new DependencyValidationResult
        {
            Success = true,
            ReasonCode = "EXCEPTION",
            GraphDecision = "EXCEPTION",
            ActivationOrderDecision = "EXCEPTION"
        };
    }
}

static void ExpectRejectedMutation(ICollection<string> failures, string label, DependencyGraphRequest request, string expectedReason)
{
    var validationRequest = expectedReason == "GRAPH_MANIFEST_DIGEST_MISMATCH"
        ? request
        : request with
        {
            ManifestSurface = request.ManifestSurface with
            {
                CanonicalDigest = ComputeCandidateGraphDigest(request)
            }
        };

    var result = ValidateNoThrow(failures, label, validationRequest);
    ExpectFail(failures, label, result, expectedReason);
}

static void ExpectCircularDependency(ICollection<string> failures, string label, DependencyGraphRequest request)
{
    var first = ValidateNoThrow(failures, label, request);
    ExpectFail(failures, label, first, "CIRCULAR_DEPENDENCY");
    Expect(failures, $"{label}-cycle-evidence", !string.IsNullOrWhiteSpace(first.CycleEvidence), first.CycleEvidence);
    var firstCycle = (first.CycleEvidence ?? string.Empty).Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
    Expect(failures, $"{label}-cycle-closed", firstCycle.Length >= 2 && string.Equals(firstCycle[0], firstCycle[^1], StringComparison.Ordinal), first.CycleEvidence);
    foreach (var subject in request.Subjects)
    {
        Expect(
            failures,
            $"{label}-cycle-contains-{subject.SubjectKind}-{subject.SubjectKey.Identity}",
            firstCycle.Contains(FixtureNodeText(subject), StringComparer.Ordinal),
            first.CycleEvidence);
    }
    var second = ValidateNoThrow(failures, $"{label}-repeat", request);
    ExpectFail(failures, $"{label}-repeat", second, "CIRCULAR_DEPENDENCY");
    Expect(failures, $"{label}-cycle-deterministic", string.Equals(first.CycleEvidence, second.CycleEvidence, StringComparison.Ordinal), $"{first.CycleEvidence} vs {second.CycleEvidence}");
}


static string SerializeSubjectSnapshotForAssertion(DependencySubjectSnapshot snapshot)
    => string.Join("|", new[]
    {
        snapshot.SubjectKind.ToString(),
        snapshot.SubjectKey.CanonicalText,
        snapshot.EvidenceReference,
        snapshot.Owner,
        snapshot.Source,
        snapshot.IntegrityDigest,
        snapshot.AvailabilityResult,
        snapshot.ContainmentEvidence,
        snapshot.EffectiveTime.ToString("O", CultureInfo.InvariantCulture),
        snapshot.Expiry.ToString("O", CultureInfo.InvariantCulture),
        snapshot.AdmissionEvidenceIdentity,
        snapshot.ManifestIdentity,
        snapshot.ManifestDigest,
        snapshot.AdmissionRequestIdentity,
        snapshot.AdmissionDecisionIdentity,
        snapshot.AdmissionDecisionReason,
        snapshot.AdmissionRequestProvenanceId,
        string.Join(",", snapshot.ManifestDeclaredDependencies),
        string.Join(",", snapshot.ManifestRequiredFoundationServices),
        snapshot.ServiceCatalogIdentity,
        snapshot.ServiceCatalogVersion,
        snapshot.ServiceCatalogOwner,
        snapshot.ServiceCatalogManifestIdentity,
        snapshot.ServiceCatalogManifestDigest
    });

static string SerializeResolutionForAssertion(DependencyResolutionSnapshot resolution)
    => string.Join("|", new[]
    {
        resolution.EdgeKey.CanonicalText,
        resolution.DependencyKind.ToString(),
        resolution.DependencySource,
        resolution.DeclaredPurpose,
        resolution.IntegrityRequirement,
        resolution.AvailabilityRequirement,
        resolution.TimeoutPolicy,
        resolution.DegradedStatePolicy,
        resolution.IsolationBoundary,
        resolution.FailurePropagationLimit,
        resolution.ReplacementPolicy,
        resolution.MigrationPolicy,
        resolution.RollbackPolicy,
        resolution.EvidenceRequirement,
        resolution.DelegationChainEvidenceReference,
        string.Join(",", resolution.CompatibleVersions),
        resolution.Relationship.ToString(),
        resolution.ConditionState?.ToString() ?? string.Empty,
        resolution.ResolvedVersion ?? string.Empty,
        SerializeFixtureLifecycleOrder(resolution.LifecycleOrder)
    });

static string SerializeGraphSnapshotForAssertion(DependencyGraphSnapshot snapshot)
    => string.Join("\n", new[]
    {
        snapshot.GraphKey.CanonicalText,
        string.Join(";", snapshot.Subjects.Select(SerializeSubjectSnapshotForAssertion)),
        string.Join(";", snapshot.ResolvedDependencies.Select(SerializeResolutionForAssertion)),
        string.Join(";", snapshot.UnresolvedOptionalDependencies.Select(SerializeResolutionForAssertion)),
        string.Join(";", snapshot.CanonicalActivationOrder.Select(key => key.CanonicalText))
    });

static string SerializeEventsForAssertion(IEnumerable<FilEvent> events)
    => string.Join("\n", events.Select(eventItem => string.Join("|", new[]
    {
        eventItem.EventId,
        eventItem.EventType,
        eventItem.SchemaVersion,
        eventItem.AuthoritativeFactOwner,
        eventItem.SubjectIdentity,
        eventItem.OccurrenceTime.ToString("O", CultureInfo.InvariantCulture),
        eventItem.PublicationTime.ToString("O", CultureInfo.InvariantCulture),
        eventItem.SourceEvidence,
        eventItem.Correlation ?? string.Empty,
        eventItem.Causation ?? string.Empty,
        eventItem.ReplayIndicator ? "true" : "false",
        eventItem.CorrectionRelationship ?? string.Empty,
        eventItem.Payload
    })));

static void VerifyPostValidationMutation(
    ICollection<string> failures,
    DependencyGraphRequest template)
{
    var mutableVersionLists = template.Dependencies
        .Select(dependency => dependency.CompatibleVersions.ToList())
        .ToList();
    var mutableDependencies = template.Dependencies
        .Select((dependency, index) => dependency with
        {
            CompatibleVersions = mutableVersionLists[index]
        })
        .ToList();
    var mutableServiceManifestLists = new List<List<ServiceDependencyDeclaration>>();
    var mutableApplicationDependencyLists = new List<List<Foundation.Admission.DependencyDeclaration>>();
    var mutableApplicationServiceLists = new List<List<FoundationServiceRequirement>>();
    var mutableSubjects = new List<DependencySubjectEvidence>();

    foreach (var subject in template.Subjects)
    {
        switch (subject)
        {
            case FoundationServiceSubjectEvidence foundation:
            {
                var manifestDependencies = foundation.CatalogEntry.Manifest.Dependencies
                    .Select(dependency => dependency with
                    {
                        CompatibleVersions = dependency.CompatibleVersions.ToList()
                    })
                    .ToList();
                mutableServiceManifestLists.Add(manifestDependencies);
                var manifest = foundation.CatalogEntry.Manifest with
                {
                    Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(manifestDependencies)
                };
                var entry = foundation.CatalogEntry with
                {
                    Manifest = manifest,
                    Registration = foundation.CatalogEntry.Registration with
                    {
                        ManifestDigest = manifest.ComputeDigest()
                    }
                };
                mutableSubjects.Add(foundation with { CatalogEntry = entry });
                break;
            }

            case ApplicationSubjectEvidence application:
            {
                var declaredDependencies = application.Manifest.DeclaredDependencies
                    .Select(dependency => new Foundation.Admission.DependencyDeclaration(
                        dependency.Identity,
                        dependency.CompatibleVersions.ToList()))
                    .ToList();
                var requiredServices = application.Manifest.RequiredFoundationServices
                    .Select(service => service with { })
                    .ToList();
                mutableApplicationDependencyLists.Add(declaredDependencies);
                mutableApplicationServiceLists.Add(requiredServices);
                var manifest = application.Manifest with
                {
                    DeclaredDependencies = declaredDependencies,
                    RequiredFoundationServices = requiredServices
                };
                var manifestDigest = manifest.ComputeDigest();
                var admissionRequest = application.AdmissionRequest with
                {
                    Manifest = manifest,
                    ManifestDigest = manifestDigest
                };
                mutableSubjects.Add(application with
                {
                    AdmissionRequest = admissionRequest,
                    Manifest = manifest,
                    ManifestDigest = manifestDigest
                });
                break;
            }

            default:
                mutableSubjects.Add(CloneSubject(subject));
                break;
        }
    }

    var mutableActivationOrder = template.ProposedActivationOrder.ToList();
    var mutableRequest = template with
    {
        Subjects = mutableSubjects,
        Dependencies = mutableDependencies,
        ProposedActivationOrder = mutableActivationOrder
    };
    mutableRequest = mutableRequest with
    {
        ManifestSurface = mutableRequest.ManifestSurface with
        {
            CanonicalDigest = ComputeCandidateGraphDigest(mutableRequest)
        }
    };

    var result = new DependencyGovernanceValidator().Validate(mutableRequest);
    ExpectPass(failures, "positive-scenario-19-post-validation-mutation", result);

    var snapshotBefore = SerializeGraphSnapshotForAssertion(result.ImmutableGraphSnapshot);
    var resolvedBefore = string.Join("\n", result.ResolvedDependencies.Select(SerializeResolutionForAssertion));
    var unresolvedBefore = string.Join("\n", result.UnresolvedOptionalDependencies.Select(SerializeResolutionForAssertion));
    var graphTextBefore = result.CanonicalGraphText;
    var graphDigestBefore = result.GraphDigest;
    var orderTextBefore = result.CanonicalActivationOrderText;
    var orderDigestBefore = result.ActivationOrderDigest;
    var decisionBefore = result.DecisionIdentity;
    var eventsBefore = SerializeEventsForAssertion(result.EvidenceEvents);

    var lifecycleMutationPerformed = false;
    foreach (var manifestDependencies in mutableServiceManifestLists)
    {
        if (manifestDependencies.Count > 0)
        {
            manifestDependencies[0] = manifestDependencies[0] with
            {
                LifecycleOrder = "mutated-lifecycle-order"
            };
            lifecycleMutationPerformed = true;
        }

        manifestDependencies.Clear();
    }

    foreach (var declaredDependencies in mutableApplicationDependencyLists)
    {
        if (declaredDependencies.Count > 0)
        {
            declaredDependencies[0] = new Foundation.Admission.DependencyDeclaration(
                "mutated-application-dependency",
                new List<string> { "99.0" });
        }

        declaredDependencies.Clear();
    }

    foreach (var requiredServices in mutableApplicationServiceLists)
    {
        if (requiredServices.Count > 0)
        {
            requiredServices[0] = requiredServices[0] with
            {
                Identity = "mutated-foundation-service",
                Version = "99.0",
                Purpose = "mutated"
            };
        }

        requiredServices.Clear();
    }

    foreach (var versions in mutableVersionLists)
    {
        versions.Clear();
        versions.Add("99.0");
    }

    mutableSubjects.Clear();
    mutableDependencies.Clear();
    mutableActivationOrder.Clear();

    Expect(failures, "caller-owned-subjects-mutation-performed", mutableSubjects.Count == 0);
    Expect(failures, "caller-owned-dependencies-mutation-performed", mutableDependencies.Count == 0);
    Expect(failures, "caller-owned-activation-order-mutation-performed", mutableActivationOrder.Count == 0);
    Expect(failures, "caller-owned-compatible-version-mutation-performed", mutableVersionLists.All(list => list.SequenceEqual(new[] { "99.0" }, StringComparer.Ordinal)));
    Expect(failures, "caller-owned-manifest-dependency-mutation-performed", mutableServiceManifestLists.All(list => list.Count == 0) && mutableApplicationDependencyLists.All(list => list.Count == 0));
    Expect(failures, "caller-owned-lifecycle-source-mutation-performed", lifecycleMutationPerformed);

    Expect(failures, "deep-immutability-snapshot", string.Equals(SerializeGraphSnapshotForAssertion(result.ImmutableGraphSnapshot), snapshotBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-resolved", string.Equals(string.Join("\n", result.ResolvedDependencies.Select(SerializeResolutionForAssertion)), resolvedBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-unresolved", string.Equals(string.Join("\n", result.UnresolvedOptionalDependencies.Select(SerializeResolutionForAssertion)), unresolvedBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-graph-text", string.Equals(result.CanonicalGraphText, graphTextBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-graph-digest", string.Equals(result.GraphDigest, graphDigestBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-order-text", string.Equals(result.CanonicalActivationOrderText, orderTextBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-order-digest", string.Equals(result.ActivationOrderDigest, orderDigestBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-decision", string.Equals(result.DecisionIdentity, decisionBefore, StringComparison.Ordinal));
    Expect(failures, "deep-immutability-events", string.Equals(SerializeEventsForAssertion(result.EvidenceEvents), eventsBefore, StringComparison.Ordinal));
}


var observationTime = DateTimeOffset.Parse("2026-08-01T00:20:00+03:00", CultureInfo.InvariantCulture);
var serviceEntry = CreateServiceCatalogEntry(observationTime, failures);
var applicationManifest = CreateApplicationManifest();
var applicationServiceManifest = applicationManifest with
{
    ApplicationIdentity = "app-2",
    ApplicationVersion = "2.0",
    ApplicationOwner = "Example Application Owner Two",
    ManifestId = "manifest-app-2",
    PackageIdentity = "pkg-app-2",
    PackageVersion = "2.0",
    RequiredFoundationServices = new ReadOnlyCollection<FoundationServiceRequirement>(new[]
    {
        new FoundationServiceRequirement(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion, "governed dependency graph validation support")
    })
};
var admissionBaseline = BuildAdmissionBaseline();
var admissionControl = new AdmissionControl(new InMemoryAdmissionBaselineProvider(admissionBaseline));
var admissionRequest = CreateAdmissionRequest(applicationManifest, observationTime);
var admissionValidation = admissionControl.Validate(admissionRequest);
Expect(failures, "application-admission-validation", admissionValidation.Success, admissionValidation.Message);
var admissionDecision = new AdmissionDecision(
    admissionRequest.AdmissionId,
    "ADMITTED",
    "admission accepted",
    admissionRequest.ContractId,
    admissionRequest.ContractVersion,
    "evidence/admission/001");

var graphAdmissionRequest = CreateAdmissionRequest(applicationServiceManifest, observationTime);
var graphAdmissionDecision = BuildAdmissionDecision(graphAdmissionRequest, "evidence/admission/graph");

var validator = new DependencyGovernanceValidator();
var validRequest = BuildValidRequest(serviceEntry, graphAdmissionRequest, graphAdmissionDecision, applicationServiceManifest, observationTime);
var validResult = validator.Validate(validRequest);
var duplicateServiceManifest = serviceEntry.Manifest with
{
    Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[]
    {
        serviceEntry.Manifest.Dependencies[0],
        serviceEntry.Manifest.Dependencies[0],
        serviceEntry.Manifest.Dependencies[1]
    })
};
var duplicateServiceEntry = serviceEntry with
{
    Manifest = duplicateServiceManifest,
    Registration = serviceEntry.Registration with { ManifestDigest = duplicateServiceManifest.ComputeDigest() }
};
var duplicateDependencyRequest = validRequest with
{
    Subjects = validRequest.Subjects.Select((subject, index) => index == 2 ? BuildFoundationServiceSubject(duplicateServiceEntry, "evidence/service/dependency-governance") : subject).ToArray(),
    Dependencies = new GovernanceDependencyDeclaration[]
    {
        validRequest.Dependencies[0],
        CloneDependency(validRequest.Dependencies[0], relationship: DependencyRelationship.Optional) with
        {
            DegradedStatePolicy = "degrade-inline"
        },
        validRequest.Dependencies[1]
    }
};
var selfDependencyManifest = serviceEntry.Manifest with
{
    Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[]
    {
        serviceEntry.Manifest.Dependencies[0] with
        {
            Identity = serviceEntry.Key.ServiceIdentity,
            CompatibleVersions = new[] { serviceEntry.Key.ServiceVersion }
        },
        serviceEntry.Manifest.Dependencies[1]
    })
};
var selfDependencyServiceEntry = serviceEntry with
{
    Manifest = selfDependencyManifest,
    Registration = serviceEntry.Registration with { ManifestDigest = selfDependencyManifest.ComputeDigest() }
};
var selfDependencyRequest = validRequest with
{
    Subjects = validRequest.Subjects.Select((subject, index) => index == 2 ? BuildFoundationServiceSubject(selfDependencyServiceEntry, "evidence/service/dependency-governance") : subject).ToArray(),
    Dependencies = new GovernanceDependencyDeclaration[]
    {
        validRequest.Dependencies[0] with
        {
            DependencyIdentity = serviceEntry.Key.ServiceIdentity,
            CompatibleVersions = new[] { serviceEntry.Key.ServiceVersion },
            ResolvedVersion = serviceEntry.Key.ServiceVersion
        },
        validRequest.Dependencies[1]
    }
};
var unknownDependencyManifest = serviceEntry.Manifest with
{
    Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[]
    {
        serviceEntry.Manifest.Dependencies[0] with
        {
            Identity = "unknown-dependency"
        },
        serviceEntry.Manifest.Dependencies[1]
    })
};
var unknownDependencyServiceEntry = serviceEntry with
{
    Manifest = unknownDependencyManifest,
    Registration = serviceEntry.Registration with { ManifestDigest = unknownDependencyManifest.ComputeDigest() }
};
var unknownDependencyRequest = validRequest with
{
    Subjects = validRequest.Subjects.Select((subject, index) => index == 2 ? BuildFoundationServiceSubject(unknownDependencyServiceEntry, "evidence/service/dependency-governance") : subject).ToArray(),
    Dependencies = new GovernanceDependencyDeclaration[]
    {
        validRequest.Dependencies[0] with
        {
            DependencyIdentity = "unknown-dependency"
        },
        validRequest.Dependencies[1]
    }
};
var unknownConsumerRequest = validRequest with
{
    Dependencies = validRequest.Dependencies.Concat(new[]
    {
        new GovernanceDependencyDeclaration
        {
            Consumer = new DependencySubjectKey("unknown-consumer", "1.0"),
            DependencyIdentity = "time-source-001",
            CompatibleVersions = new[] { "1.0" },
            Relationship = DependencyRelationship.Required,
            ConditionState = null,
            ResolvedVersion = "1.0",
            DependencyKind = DependencySubjectKind.External,
            DependencySource = "ServiceManifest",
            DeclaredPurpose = "time source dependency",
            IntegrityRequirement = "integrity-required",
            AvailabilityRequirement = "available",
            TimeoutPolicy = "bounded",
            DegradedStatePolicy = "degrade-inline",
            IsolationBoundary = "contained",
            FailurePropagationLimit = "bounded",
            ReplacementPolicy = "explicit",
            MigrationPolicy = "manual",
            RollbackPolicy = "rollback-allowed",
            EvidenceRequirement = "required",
            DelegationChainEvidenceReference = "delegation-chain-unknown-consumer",
            LifecycleOrder = BuildLifecycleOrder()
        }
    }).ToArray()
};

duplicateDependencyRequest = BindRequestManifestCoverage(duplicateDependencyRequest, false);
selfDependencyRequest = BindRequestManifestCoverage(selfDependencyRequest, false);
unknownDependencyRequest = BindRequestManifestCoverage(unknownDependencyRequest, false);

ExpectPass(failures, "valid-graph", validResult);
    Expect(failures, "canonical-order-count", validResult.ImmutableGraphSnapshot.CanonicalActivationOrder.Count == 4, validResult.CanonicalActivationOrderText);

var repeatedResult = validator.Validate(validRequest);
Expect(failures, "graph-determinism-digest", string.Equals(validResult.GraphDigest, repeatedResult.GraphDigest, StringComparison.Ordinal), $"{validResult.GraphDigest} vs {repeatedResult.GraphDigest}");
Expect(failures, "order-determinism-digest", string.Equals(validResult.ActivationOrderDigest, repeatedResult.ActivationOrderDigest, StringComparison.Ordinal), $"{validResult.ActivationOrderDigest} vs {repeatedResult.ActivationOrderDigest}");
Expect(failures, "graph-determinism-order", string.Equals(validResult.CanonicalActivationOrderText, repeatedResult.CanonicalActivationOrderText, StringComparison.Ordinal), validResult.CanonicalActivationOrderText);

    var goldenGraphText = validResult.CanonicalGraphText;
    var goldenGraphBytes = Encoding.UTF8.GetByteCount(goldenGraphText);
    const string expectedGoldenDigest = "BA6CEF2A5E86EE12FA47A9A2CE31EF89B424BFF43EFEF05214788B086295D44E";
    const int expectedGoldenByteLength = 4833;
    Expect(failures, "golden-graph-digest", string.Equals(validResult.GraphDigest, expectedGoldenDigest, StringComparison.Ordinal), validResult.GraphDigest);
    Expect(failures, "golden-graph-byte-length", goldenGraphBytes == expectedGoldenByteLength, goldenGraphBytes.ToString(CultureInfo.InvariantCulture));
    Expect(failures, "golden-graph-repeat-canonical-text", string.Equals(validResult.CanonicalGraphText, repeatedResult.CanonicalGraphText, StringComparison.Ordinal), "canonical graph text changed");
    Expect(failures, "golden-graph-repeat-digest", string.Equals(validResult.GraphDigest, repeatedResult.GraphDigest, StringComparison.Ordinal), $"{validResult.GraphDigest} vs {repeatedResult.GraphDigest}");
    Expect(failures, "golden-graph-repeat-order", string.Equals(validResult.CanonicalActivationOrderText, repeatedResult.CanonicalActivationOrderText, StringComparison.Ordinal), repeatedResult.CanonicalActivationOrderText);
    Expect(failures, "golden-graph-repeat-order-digest", string.Equals(validResult.ActivationOrderDigest, repeatedResult.ActivationOrderDigest, StringComparison.Ordinal), $"{validResult.ActivationOrderDigest} vs {repeatedResult.ActivationOrderDigest}");
    Expect(failures, "golden-graph-repeat-decision", string.Equals(validResult.DecisionIdentity, repeatedResult.DecisionIdentity, StringComparison.Ordinal), $"{validResult.DecisionIdentity} vs {repeatedResult.DecisionIdentity}");
    Expect(failures, "golden-graph-repeat-events", string.Equals(SerializeEventsForAssertion(validResult.EvidenceEvents), SerializeEventsForAssertion(repeatedResult.EvidenceEvents), StringComparison.Ordinal), "event evidence changed");
    Expect(failures, "golden-graph-repeat-resolved-edges", string.Equals(string.Join("\n", validResult.ResolvedDependencies.Select(SerializeResolutionForAssertion)), string.Join("\n", repeatedResult.ResolvedDependencies.Select(SerializeResolutionForAssertion)), StringComparison.Ordinal), "resolved edges changed");
    Expect(failures, "golden-graph-repeat-unresolved-optionals", string.Equals(string.Join("\n", validResult.UnresolvedOptionalDependencies.Select(SerializeResolutionForAssertion)), string.Join("\n", repeatedResult.UnresolvedOptionalDependencies.Select(SerializeResolutionForAssertion)), StringComparison.Ordinal), "unresolved Optional records changed");
    Expect(failures, "golden-graph-repeat-snapshot", string.Equals(SerializeGraphSnapshotForAssertion(validResult.ImmutableGraphSnapshot), SerializeGraphSnapshotForAssertion(repeatedResult.ImmutableGraphSnapshot), StringComparison.Ordinal), "immutable graph snapshot changed");
    Console.WriteLine($"Golden Dependency Graph SHA-256: {validResult.GraphDigest}");
    Console.WriteLine($"Golden Dependency Graph UTF-8 byte length: {goldenGraphBytes}");

    for (var scenario = 1; scenario <= 15; scenario++)
    {
        var request = CreatePositiveScenarioRequest(
            scenario,
            serviceEntry,
            admissionRequest,
            admissionDecision,
            applicationManifest,
            applicationServiceManifest,
            observationTime,
            validRequest);
        ExpectPass(failures, $"positive-scenario-{scenario:00}", validator.Validate(request));
    }

    var ordinalRequest = CreatePositiveScenarioRequest(
        5,
        serviceEntry,
        admissionRequest,
        admissionDecision,
        applicationManifest,
        applicationServiceManifest,
        observationTime,
        validRequest);
    var ordinalResult = validator.Validate(ordinalRequest);
    ExpectPass(failures, "ordinal-tie-breaking-explicit", ordinalResult);
    Expect(
        failures,
        "ordinal-tie-breaking-order",
        ordinalResult.ImmutableGraphSnapshot.CanonicalActivationOrder.Select(key => key.Identity).SequenceEqual(
            new[] { "app-3", "time-source-001", "a-service" },
            StringComparer.Ordinal),
        ordinalResult.CanonicalActivationOrderText);

    var deterministicPositiveFirst = validator.Validate(validRequest);
    var deterministicPositiveSecond = validator.Validate(validRequest);
    ExpectPass(failures, "positive-scenario-16-repeated-deterministic-validation", deterministicPositiveFirst);
    Expect(
        failures,
        "positive-scenario-16-canonical-graph-text",
        string.Equals(deterministicPositiveFirst.CanonicalGraphText, deterministicPositiveSecond.CanonicalGraphText, StringComparison.Ordinal));
    Expect(
        failures,
        "positive-scenario-16-graph-digest",
        string.Equals(deterministicPositiveFirst.GraphDigest, deterministicPositiveSecond.GraphDigest, StringComparison.Ordinal));
    Expect(
        failures,
        "positive-scenario-16-activation-order-text",
        string.Equals(deterministicPositiveFirst.CanonicalActivationOrderText, deterministicPositiveSecond.CanonicalActivationOrderText, StringComparison.Ordinal));
    Expect(
        failures,
        "positive-scenario-16-activation-order-digest",
        string.Equals(deterministicPositiveFirst.ActivationOrderDigest, deterministicPositiveSecond.ActivationOrderDigest, StringComparison.Ordinal));
    Expect(
        failures,
        "positive-scenario-16-decision-identity",
        string.Equals(deterministicPositiveFirst.DecisionIdentity, deterministicPositiveSecond.DecisionIdentity, StringComparison.Ordinal));
    Expect(
        failures,
        "positive-scenario-16-evidence-events",
        deterministicPositiveFirst.EvidenceEvents.SequenceEqual(deterministicPositiveSecond.EvidenceEvents));

    ExpectPass(failures, "positive-scenario-17-valid-graph-evidence-event", validResult);
    var graphEvidenceEvent = validResult.EvidenceEvents.Single(eventItem => string.Equals(eventItem.EventType, "DEPENDENCY_GRAPH_VALIDATED", StringComparison.Ordinal));
    Expect(failures, "positive-scenario-17-event-id", string.Equals(graphEvidenceEvent.EventId, $"{validRequest.GraphId}:DEPENDENCY_GRAPH_VALIDATED", StringComparison.Ordinal), graphEvidenceEvent.EventId);
    Expect(failures, "positive-scenario-17-subject", string.Equals(graphEvidenceEvent.SubjectIdentity, validRequest.GraphId, StringComparison.Ordinal), graphEvidenceEvent.SubjectIdentity);
    Expect(failures, "positive-scenario-17-source-evidence", string.Equals(graphEvidenceEvent.SourceEvidence, validResult.DecisionIdentity, StringComparison.Ordinal), graphEvidenceEvent.SourceEvidence);
    Expect(failures, "positive-scenario-17-correlation", string.Equals(graphEvidenceEvent.Correlation, validResult.DecisionIdentity, StringComparison.Ordinal), graphEvidenceEvent.Correlation);
    Expect(failures, "positive-scenario-17-causation", graphEvidenceEvent.Causation is null, graphEvidenceEvent.Causation);
    Expect(failures, "positive-scenario-17-payload", string.Equals(graphEvidenceEvent.Payload, validResult.GraphDigest, StringComparison.Ordinal), graphEvidenceEvent.Payload);

    ExpectPass(failures, "positive-scenario-18-valid-activation-order-evidence-event", validResult);
    var activationOrderEvidenceEvent = validResult.EvidenceEvents.Single(eventItem => string.Equals(eventItem.EventType, "ACTIVATION_ORDER_VALIDATED", StringComparison.Ordinal));
    Expect(failures, "positive-scenario-18-event-id", string.Equals(activationOrderEvidenceEvent.EventId, $"{validRequest.GraphId}:ACTIVATION_ORDER_VALIDATED", StringComparison.Ordinal), activationOrderEvidenceEvent.EventId);
    Expect(failures, "positive-scenario-18-subject", string.Equals(activationOrderEvidenceEvent.SubjectIdentity, validRequest.GraphId, StringComparison.Ordinal), activationOrderEvidenceEvent.SubjectIdentity);
    Expect(failures, "positive-scenario-18-source-evidence", string.Equals(activationOrderEvidenceEvent.SourceEvidence, validResult.DecisionIdentity, StringComparison.Ordinal), activationOrderEvidenceEvent.SourceEvidence);
    Expect(failures, "positive-scenario-18-correlation", string.Equals(activationOrderEvidenceEvent.Correlation, validResult.DecisionIdentity, StringComparison.Ordinal), activationOrderEvidenceEvent.Correlation);
    Expect(failures, "positive-scenario-18-causation", string.Equals(activationOrderEvidenceEvent.Causation, graphEvidenceEvent.EventId, StringComparison.Ordinal), activationOrderEvidenceEvent.Causation);
    Expect(failures, "positive-scenario-18-payload", string.Equals(activationOrderEvidenceEvent.Payload, validResult.ActivationOrderDigest, StringComparison.Ordinal), activationOrderEvidenceEvent.Payload);

    var appTwoManifest = applicationServiceManifest with
    {
        ApplicationIdentity = "app-2",
        ApplicationVersion = "2.0",
        ApplicationOwner = "Example Application Owner Two",
        ManifestId = "manifest-app-2",
        PackageIdentity = "pkg-app-2",
        PackageVersion = "2.0"
    };
    var appTwoAdmissionRequest = CreateAdmissionRequest(appTwoManifest, observationTime);
    var appTwoAdmissionDecision = BuildAdmissionDecision(appTwoAdmissionRequest, "evidence/admission/002");
    var appTwoSubject = BuildApplicationSubject(appTwoManifest, appTwoAdmissionRequest, appTwoAdmissionDecision, "evidence/admission/002");
    var appServiceRequest = BuildGraphRequest(
        "application-to-service-graph",
        "1.0",
        "graph-governance-requester",
        "GOV-090",
        observationTime,
        BuildGraphManifest("application-to-service-graph", observationTime.AddMinutes(-5), observationTime.AddHours(2), string.Empty),
        BuildDelegation("graph-governance-requester", "GOV-090", observationTime.AddMinutes(-10), observationTime.AddHours(2)),
        new DependencySubjectEvidence[]
        {
            BuildExternalSubject("time-source-001", "1.0", "Falcon Foundation", "time-provider-evidence", "evidence/external/time-source-001", "external-time-source-001", observationTime.AddMinutes(-20), observationTime.AddHours(2)),
            BuildExternalSubject("CON-023", "1.1", "Falcon Application Authority", "contract-reference", "evidence/external/contract-con-023", "external-contract-con-023", observationTime.AddMinutes(-20), observationTime.AddHours(2)),
            BuildFoundationServiceSubject(serviceEntry, "evidence/service/dependency-governance"),
            appTwoSubject
        },
        new GovernanceDependencyDeclaration[]
        {
            new()
            {
                Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                DependencyIdentity = "time-source-001",
                CompatibleVersions = new[] { "1.0" },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = "1.0",
                DependencyKind = DependencySubjectKind.External,
                DependencySource = "ServiceManifest",
                DeclaredPurpose = "time source dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new()
            {
                Consumer = new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
                DependencyIdentity = "CON-023",
                CompatibleVersions = new[] { "1.1" },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = "1.1",
                DependencyKind = DependencySubjectKind.External,
                DependencySource = "ServiceManifest",
                DeclaredPurpose = "application contract dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "external",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new()
            {
                Consumer = new DependencySubjectKey(appTwoSubject.SubjectKey.Identity, appTwoSubject.SubjectKey.Version),
                DependencyIdentity = "CON-023",
                CompatibleVersions = new[] { "1.1" },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = "1.1",
                DependencyKind = DependencySubjectKind.External,
                DependencySource = "ApplicationManifest",
                DeclaredPurpose = "application contract dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "external",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new()
            {
                Consumer = new DependencySubjectKey(appTwoSubject.SubjectKey.Identity, appTwoSubject.SubjectKey.Version),
                DependencyIdentity = serviceEntry.Key.ServiceIdentity,
                CompatibleVersions = new[] { serviceEntry.Key.ServiceVersion },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = serviceEntry.Key.ServiceVersion,
                DependencyKind = DependencySubjectKind.FoundationService,
                DependencySource = "ApplicationManifest",
                DeclaredPurpose = "governed dependency graph validation support",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            }
        },
        new[]
        {
            new DependencySubjectKey("time-source-001", "1.0"),
            new DependencySubjectKey("CON-023", "1.1"),
            new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
            new DependencySubjectKey(appTwoSubject.SubjectKey.Identity, appTwoSubject.SubjectKey.Version)
        });
    appServiceRequest = appServiceRequest with { ManifestSurface = appServiceRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(appServiceRequest) } };
    ExpectPass(failures, "application-to-service", validator.Validate(appServiceRequest));

    var blankGraphVersionRequest = validRequest with { GraphVersion = string.Empty };
    ExpectFail(failures, "blank-graph-version", ValidateNoThrow(failures, "blank-graph-version", blankGraphVersionRequest), "MISSING_GRAPH_VERSION");

    var ambiguousApplicationManifest = applicationManifest with
    {
        ApplicationIdentity = serviceEntry.Key.ServiceIdentity,
        ApplicationVersion = serviceEntry.Key.ServiceVersion,
        ManifestId = "manifest-ambiguous-identity",
        PackageIdentity = "pkg-ambiguous-identity",
        PackageVersion = serviceEntry.Key.ServiceVersion
    };
    var ambiguousAdmissionRequest = CreateAdmissionRequest(ambiguousApplicationManifest, observationTime);
    var ambiguousAdmissionDecision = BuildAdmissionDecision(ambiguousAdmissionRequest, "evidence/admission/ambiguous");
    var ambiguousApplicationSubject = BuildApplicationSubject(ambiguousApplicationManifest, ambiguousAdmissionRequest, ambiguousAdmissionDecision, "evidence/admission/ambiguous");
    var ambiguousSubjectRequest = validRequest with
    {
        Subjects = validRequest.Subjects.Concat(new DependencySubjectEvidence[] { ambiguousApplicationSubject }).ToArray()
    };
    ambiguousSubjectRequest = ambiguousSubjectRequest with { ManifestSurface = ambiguousSubjectRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(ambiguousSubjectRequest) } };
    ExpectFail(failures, "ambiguous-subject-identity", ValidateNoThrow(failures, "ambiguous-subject-identity", ambiguousSubjectRequest), "AMBIGUOUS_SUBJECT_IDENTITY");

    var subjectKindMismatchRequest = validRequest with
    {
        Subjects = validRequest.Subjects.Select((subject, index) => index == 2 ? ((FoundationServiceSubjectEvidence)subject) with { SubjectKind = DependencySubjectKind.Application } : subject).ToArray()
    };
    subjectKindMismatchRequest = subjectKindMismatchRequest with { ManifestSurface = subjectKindMismatchRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(subjectKindMismatchRequest) } };
    ExpectFail(failures, "subject-kind-mismatch", ValidateNoThrow(failures, "subject-kind-mismatch", subjectKindMismatchRequest), "SUBJECT_KIND_MISMATCH");

    var invalidConditionStateRequest = validRequest with
    {
        Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { Relationship = DependencyRelationship.Conditional, ConditionState = (DependencyConditionState)999 } : dependency).ToArray()
    };
    invalidConditionStateRequest = invalidConditionStateRequest with { ManifestSurface = invalidConditionStateRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(invalidConditionStateRequest) } };
    ExpectFail(failures, "invalid-condition-state", ValidateNoThrow(failures, "invalid-condition-state", invalidConditionStateRequest), "INVALID_CONDITION_STATE");

    var invalidRelationshipCastRequest = validRequest with
    {
        Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { Relationship = (DependencyRelationship)999 } : dependency).ToArray()
    };
    invalidRelationshipCastRequest = invalidRelationshipCastRequest with { ManifestSurface = invalidRelationshipCastRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(invalidRelationshipCastRequest) } };
    ExpectFail(failures, "invalid-relationship-cast", ValidateNoThrow(failures, "invalid-relationship-cast", invalidRelationshipCastRequest), "INVALID_RELATIONSHIP");

    var cycleTwoAppManifest = appTwoManifest with
    {
        DeclaredDependencies = Array.Empty<Foundation.Admission.DependencyDeclaration>()
    };
    var cycleTwoServiceManifest = serviceEntry.Manifest with
    {
        Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[]
        {
            serviceEntry.Manifest.Dependencies[0] with
            {
                Identity = cycleTwoAppManifest.ApplicationIdentity,
                CompatibleVersions = new[] { cycleTwoAppManifest.ApplicationVersion },
                Kind = "application",
                Purpose = "application dependency"
            }
        })
    };
    var cycleTwoServiceEntry = serviceEntry with
    {
        Manifest = cycleTwoServiceManifest,
        Registration = serviceEntry.Registration with
        {
            ManifestDigest = cycleTwoServiceManifest.ComputeDigest(),
            RegistrationEvidenceReference = "evidence/service/cycle-two"
        }
    };
    var cycleTwoAdmissionRequest = CreateAdmissionRequest(cycleTwoAppManifest, observationTime);
    var cycleTwoAdmissionDecision = BuildAdmissionDecision(cycleTwoAdmissionRequest, "evidence/admission/cycle-two");
    var cycleTwoAppSubject = BuildApplicationSubject(cycleTwoAppManifest, cycleTwoAdmissionRequest, cycleTwoAdmissionDecision, "evidence/admission/cycle-two");
    var cycleTwoNodeRequest = BuildGraphRequest(
        "exact-two-node-cycle-graph",
        "1.0",
        "graph-governance-requester",
        "GOV-090",
        observationTime,
        BuildGraphManifest("exact-two-node-cycle-graph", observationTime.AddMinutes(-5), observationTime.AddHours(2), string.Empty),
        BuildDelegation("graph-governance-requester", "GOV-090", observationTime.AddMinutes(-10), observationTime.AddHours(2)),
        new DependencySubjectEvidence[]
        {
            BuildFoundationServiceSubject(cycleTwoServiceEntry, "evidence/service/cycle-two"),
            cycleTwoAppSubject
        },
        new GovernanceDependencyDeclaration[]
        {
            new()
            {
                Consumer = new DependencySubjectKey(cycleTwoServiceEntry.Key.ServiceIdentity, cycleTwoServiceEntry.Key.ServiceVersion),
                DependencyIdentity = cycleTwoAppSubject.SubjectKey.Identity,
                CompatibleVersions = new[] { cycleTwoAppSubject.SubjectKey.Version },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = cycleTwoAppSubject.SubjectKey.Version,
                DependencyKind = DependencySubjectKind.Application,
                DependencySource = "ServiceManifest",
                DeclaredPurpose = "application dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new()
            {
                Consumer = new DependencySubjectKey(cycleTwoAppSubject.SubjectKey.Identity, cycleTwoAppSubject.SubjectKey.Version),
                DependencyIdentity = cycleTwoServiceEntry.Key.ServiceIdentity,
                CompatibleVersions = new[] { cycleTwoServiceEntry.Key.ServiceVersion },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = cycleTwoServiceEntry.Key.ServiceVersion,
                DependencyKind = DependencySubjectKind.FoundationService,
                DependencySource = "ApplicationManifest",
                DeclaredPurpose = "governed dependency graph validation support",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            }
        },
        new[]
        {
            new DependencySubjectKey(cycleTwoAppSubject.SubjectKey.Identity, cycleTwoAppSubject.SubjectKey.Version),
            new DependencySubjectKey(cycleTwoServiceEntry.Key.ServiceIdentity, cycleTwoServiceEntry.Key.ServiceVersion)
        });
    cycleTwoNodeRequest = cycleTwoNodeRequest with { ManifestSurface = cycleTwoNodeRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(cycleTwoNodeRequest) } };
    ExpectCircularDependency(failures, "exact-two-node-cycle", cycleTwoNodeRequest);

    var resolvedOptionalCycleRequest = cycleTwoNodeRequest with
    {
        Dependencies = cycleTwoNodeRequest.Dependencies.Select((dependency, index) => index == 0
            ? dependency with
            {
                Relationship = DependencyRelationship.Optional,
                ConditionState = null,
                DegradedStatePolicy = "degrade-inline"
            }
            : dependency).ToArray()
    };
    resolvedOptionalCycleRequest = BindRequestManifestCoverage(resolvedOptionalCycleRequest, false);
    ExpectCircularDependency(failures, "resolved-optional-cycle", resolvedOptionalCycleRequest);

    var cycleThreeExternalSubject = BuildExternalSubject("cycle-external-001", "1.0", "Falcon Foundation", "cycle-reference", "evidence/external/cycle-external-001", "cycle-external-001", observationTime.AddMinutes(-20), observationTime.AddHours(2));
    var cycleThreeServiceManifest = serviceEntry.Manifest with
    {
        Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[]
        {
            serviceEntry.Manifest.Dependencies[0] with
            {
                Identity = cycleThreeExternalSubject.SubjectKey.Identity,
                CompatibleVersions = new[] { cycleThreeExternalSubject.SubjectKey.Version },
                Purpose = "cycle external dependency"
            }
        })
    };
    var cycleThreeServiceEntry = serviceEntry with
    {
        Manifest = cycleThreeServiceManifest,
        Registration = serviceEntry.Registration with
        {
            ManifestDigest = cycleThreeServiceManifest.ComputeDigest(),
            RegistrationEvidenceReference = "evidence/service/cycle-three"
        }
    };
    var cycleThreeAppManifest = appTwoManifest with
    {
        DeclaredDependencies = Array.Empty<Foundation.Admission.DependencyDeclaration>(),
        RequiredFoundationServices = new ReadOnlyCollection<FoundationServiceRequirement>(new[]
        {
            new FoundationServiceRequirement(cycleThreeServiceEntry.Key.ServiceIdentity, cycleThreeServiceEntry.Key.ServiceVersion, "governed dependency graph validation support")
        })
    };
    var cycleThreeAdmissionRequest = CreateAdmissionRequest(cycleThreeAppManifest, observationTime);
    var cycleThreeAdmissionDecision = BuildAdmissionDecision(cycleThreeAdmissionRequest, "evidence/admission/cycle-three");
    var cycleThreeAppSubject = BuildApplicationSubject(cycleThreeAppManifest, cycleThreeAdmissionRequest, cycleThreeAdmissionDecision, "evidence/admission/cycle-three");
    var cycleThreeNodeRequest = BuildGraphRequest(
        "exact-three-node-cycle-graph",
        "1.0",
        "graph-governance-requester",
        "GOV-090",
        observationTime,
        BuildGraphManifest("exact-three-node-cycle-graph", observationTime.AddMinutes(-5), observationTime.AddHours(2), string.Empty),
        BuildDelegation("graph-governance-requester", "GOV-090", observationTime.AddMinutes(-10), observationTime.AddHours(2)),
        new DependencySubjectEvidence[]
        {
            cycleThreeExternalSubject,
            BuildFoundationServiceSubject(cycleThreeServiceEntry, "evidence/service/cycle-three"),
            cycleThreeAppSubject
        },
        new GovernanceDependencyDeclaration[]
        {
            new()
            {
                Consumer = new DependencySubjectKey(cycleThreeServiceEntry.Key.ServiceIdentity, cycleThreeServiceEntry.Key.ServiceVersion),
                DependencyIdentity = cycleThreeExternalSubject.SubjectKey.Identity,
                CompatibleVersions = new[] { cycleThreeExternalSubject.SubjectKey.Version },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = cycleThreeExternalSubject.SubjectKey.Version,
                DependencyKind = DependencySubjectKind.External,
                DependencySource = "ServiceManifest",
                DeclaredPurpose = "cycle external dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new()
            {
                Consumer = new DependencySubjectKey(cycleThreeAppSubject.SubjectKey.Identity, cycleThreeAppSubject.SubjectKey.Version),
                DependencyIdentity = cycleThreeServiceEntry.Key.ServiceIdentity,
                CompatibleVersions = new[] { cycleThreeServiceEntry.Key.ServiceVersion },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = cycleThreeServiceEntry.Key.ServiceVersion,
                DependencyKind = DependencySubjectKind.FoundationService,
                DependencySource = "ApplicationManifest",
                DeclaredPurpose = "governed dependency graph validation support",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            },
            new()
            {
                Consumer = new DependencySubjectKey(cycleThreeExternalSubject.SubjectKey.Identity, cycleThreeExternalSubject.SubjectKey.Version),
                DependencyIdentity = cycleThreeAppSubject.SubjectKey.Identity,
                CompatibleVersions = new[] { cycleThreeAppSubject.SubjectKey.Version },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = cycleThreeAppSubject.SubjectKey.Version,
                DependencyKind = DependencySubjectKind.Application,
                DependencySource = "ExternalManifest",
                DeclaredPurpose = "cycle application dependency",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "external",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            }
        },
        new[]
        {
            new DependencySubjectKey(cycleThreeExternalSubject.SubjectKey.Identity, cycleThreeExternalSubject.SubjectKey.Version),
            new DependencySubjectKey(cycleThreeServiceEntry.Key.ServiceIdentity, cycleThreeServiceEntry.Key.ServiceVersion),
            new DependencySubjectKey(cycleThreeAppSubject.SubjectKey.Identity, cycleThreeAppSubject.SubjectKey.Version)
        });
    cycleThreeNodeRequest = cycleThreeNodeRequest with { ManifestSurface = cycleThreeNodeRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(cycleThreeNodeRequest) } };
    ExpectCircularDependency(failures, "exact-three-node-cycle", cycleThreeNodeRequest);

    var conditionalCycleRequest = cycleTwoNodeRequest with
    {
        Dependencies = cycleTwoNodeRequest.Dependencies.Select((dependency, index) => index == 0
            ? dependency with
            {
                Relationship = DependencyRelationship.Conditional,
                ConditionState = DependencyConditionState.RequiredNow
            }
            : dependency).ToArray()
    };
    conditionalCycleRequest = BindRequestManifestCoverage(conditionalCycleRequest, false);
    ExpectCircularDependency(failures, "conditional-required-now-cycle", conditionalCycleRequest);

    var externalUnavailableRequest = validRequest with
    {
        Subjects = validRequest.Subjects.Select((subject, index) => index == 0 ? ((ExternalDependencySubjectEvidence)subject) with { AvailabilityResult = "UNAVAILABLE" } : subject).ToArray()
    };
    externalUnavailableRequest = externalUnavailableRequest with { ManifestSurface = externalUnavailableRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(externalUnavailableRequest) } };
    ExpectFail(failures, "external-unavailable", ValidateNoThrow(failures, "external-unavailable", externalUnavailableRequest), "DEPENDENCY_UNAVAILABLE");

    var delegationMismatchRequest = validRequest with
    {
        Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { DelegationChainEvidenceReference = "delegation-chain-mismatch" } : dependency).ToArray()
    };
    delegationMismatchRequest = delegationMismatchRequest with { ManifestSurface = delegationMismatchRequest.ManifestSurface with { CanonicalDigest = ComputeCandidateGraphDigest(delegationMismatchRequest) } };
    ExpectFail(failures, "delegation-chain-mismatch", ValidateNoThrow(failures, "delegation-chain-mismatch", delegationMismatchRequest), "DELEGATION_CHAIN_MISMATCH");

    var applicationEvidenceMismatchManifest = applicationManifest with
    {
        ManifestId = "manifest-app-evidence-mismatch"
    };
    var applicationEvidenceMismatchRequest = CreateAdmissionRequest(applicationEvidenceMismatchManifest, observationTime);
    var applicationEvidenceMismatchDecision = BuildAdmissionDecision(applicationEvidenceMismatchRequest, "evidence/admission/mismatch");
    var applicationEvidenceMismatchSubject = BuildApplicationSubject(applicationEvidenceMismatchManifest, applicationEvidenceMismatchRequest, applicationEvidenceMismatchDecision with { EvidenceId = "evidence/admission/different" }, "evidence/admission/mismatch");
    var applicationEvidenceMismatchGraph = BuildScenarioRequest(
        "application-evidence-mismatch",
        "1.0",
        "graph-governance-requester-20",
        "GOV-090",
        observationTime,
        new DependencySubjectEvidence[] { BuildFoundationServiceSubject(serviceEntry, "evidence/service/dependency-governance"), applicationEvidenceMismatchSubject },
        new GovernanceDependencyDeclaration[]
        {
            new()
            {
                Consumer = new DependencySubjectKey(applicationEvidenceMismatchSubject.SubjectKey.Identity, applicationEvidenceMismatchSubject.SubjectKey.Version),
                DependencyIdentity = serviceEntry.Key.ServiceIdentity,
                CompatibleVersions = new[] { serviceEntry.Key.ServiceVersion },
                Relationship = DependencyRelationship.Required,
                ConditionState = null,
                ResolvedVersion = serviceEntry.Key.ServiceVersion,
                DependencyKind = DependencySubjectKind.FoundationService,
                DependencySource = "ApplicationManifest",
                DeclaredPurpose = "governed dependency graph validation support",
                IntegrityRequirement = "integrity-required",
                AvailabilityRequirement = "available",
                TimeoutPolicy = "bounded",
                DegradedStatePolicy = "isolate",
                IsolationBoundary = "contained",
                FailurePropagationLimit = "bounded",
                ReplacementPolicy = "explicit",
                MigrationPolicy = "manual",
                RollbackPolicy = "rollback-allowed",
                EvidenceRequirement = "required",
                DelegationChainEvidenceReference = "delegation-chain-001",
                LifecycleOrder = BuildLifecycleOrder()
            }
        },
        new[]
        {
            new DependencySubjectKey(serviceEntry.Key.ServiceIdentity, serviceEntry.Key.ServiceVersion),
            new DependencySubjectKey(applicationEvidenceMismatchSubject.SubjectKey.Identity, applicationEvidenceMismatchSubject.SubjectKey.Version)
        });
    var applicationEvidenceMismatchResult = ValidateNoThrow(failures, "application-evidence-mismatch", applicationEvidenceMismatchGraph);
    ExpectFail(failures, "application-evidence-mismatch", applicationEvidenceMismatchResult, "INVALID_SUBJECT_EVIDENCE");

    var foundationEvidenceMismatchEntry = serviceEntry with
    {
        Registration = serviceEntry.Registration with
        {
            AccountableOwner = "Conflicting Foundation Owner"
        }
    };
    var foundationEvidenceMismatchRequest = BuildScenarioRequest(
        "foundation-evidence-mismatch",
        "1.0",
        "graph-governance-requester-21",
        "GOV-090",
        observationTime,
        new DependencySubjectEvidence[] { BuildFoundationServiceSubject(foundationEvidenceMismatchEntry, "evidence/service/dependency-governance") },
        Array.Empty<GovernanceDependencyDeclaration>(),
        new[] { new DependencySubjectKey(foundationEvidenceMismatchEntry.Key.ServiceIdentity, foundationEvidenceMismatchEntry.Key.ServiceVersion) });
    ExpectFail(failures, "foundation-evidence-mismatch", ValidateNoThrow(failures, "foundation-evidence-mismatch", foundationEvidenceMismatchRequest), "INVALID_SUBJECT_EVIDENCE");

    ExpectRejectedMutation(failures, "duplicate-subject", validRequest with { Subjects = validRequest.Subjects.Concat(new[] { CloneSubject(validRequest.Subjects[0]) }).ToArray() }, "DUPLICATE_SUBJECT");
    ExpectRejectedMutation(failures, "duplicate-dependency", duplicateDependencyRequest, "DUPLICATE_DEPENDENCY");
    ExpectRejectedMutation(failures, "direct-self-dependency", selfDependencyRequest, "DIRECT_SELF_DEPENDENCY");
    ExpectRejectedMutation(failures, "unknown-consumer", unknownConsumerRequest, "UNKNOWN_CONSUMER");
    ExpectRejectedMutation(failures, "unknown-dependency", unknownDependencyRequest, "UNKNOWN_DEPENDENCY");
ExpectRejectedMutation(failures, "missing-required-dependency", validRequest with { Dependencies = validRequest.Dependencies.Take(1).ToArray() }, "MISSING_DECLARED_DEPENDENCY");
ExpectRejectedMutation(failures, "blank-compatible-version", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { CompatibleVersions = new[] { "" } } : dependency).ToArray() }, "BLANK_COMPATIBLE_VERSION");
ExpectRejectedMutation(failures, "duplicate-compatible-version", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { CompatibleVersions = new[] { "1.0", "1.0" } } : dependency).ToArray() }, "DUPLICATE_COMPATIBLE_VERSION");
ExpectRejectedMutation(failures, "missing-resolved-version", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { Relationship = DependencyRelationship.Required, ResolvedVersion = null } : dependency).ToArray() }, "MISSING_RESOLVED_VERSION");
ExpectRejectedMutation(failures, "resolved-version-not-compatible", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { ResolvedVersion = "9.9", CompatibleVersions = new[] { "1.0" } } : dependency).ToArray() }, "RESOLVED_VERSION_NOT_COMPATIBLE");
var prohibitedPresentRequest = validRequest with
{
    Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0
        ? dependency with
        {
            Relationship = DependencyRelationship.Prohibited,
            ConditionState = null,
            ResolvedVersion = "1.0"
        }
        : dependency).ToArray()
};
prohibitedPresentRequest = BindRequestManifestCoverage(prohibitedPresentRequest, false);
ExpectRejectedMutation(failures, "prohibited-dependency-present", prohibitedPresentRequest, "PROHIBITED_DEPENDENCY_PRESENT");

var conditionalMissingStateRequest = validRequest with
{
    Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0
        ? dependency with
        {
            Relationship = DependencyRelationship.Conditional,
            ConditionState = null,
            ResolvedVersion = "1.0"
        }
        : dependency).ToArray()
};
conditionalMissingStateRequest = BindRequestManifestCoverage(conditionalMissingStateRequest, false);
ExpectRejectedMutation(failures, "conditional-missing-state", conditionalMissingStateRequest, "INVALID_CONDITION_STATE");
ExpectRejectedMutation(failures, "invalid-relationship", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { Relationship = (DependencyRelationship)999, ConditionState = null } : dependency).ToArray() }, "INVALID_RELATIONSHIP");
    var conditionalRequiredNowUnresolvedRequest = validRequest with
    {
        Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0
            ? dependency with
            {
                Relationship = DependencyRelationship.Conditional,
                ConditionState = DependencyConditionState.RequiredNow,
                ResolvedVersion = null
            }
            : dependency).ToArray()
    };
    conditionalRequiredNowUnresolvedRequest = BindRequestManifestCoverage(conditionalRequiredNowUnresolvedRequest, false);
    ExpectRejectedMutation(failures, "conditional-dependency-unresolved", conditionalRequiredNowUnresolvedRequest, "CONDITIONAL_DEPENDENCY_UNRESOLVED");

    var conditionalNotRequiredResolvedRequest = validRequest with
    {
        Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0
            ? dependency with
            {
                Relationship = DependencyRelationship.Conditional,
                ConditionState = DependencyConditionState.NotRequiredNow,
                ResolvedVersion = "1.0"
            }
            : dependency).ToArray()
    };
    conditionalNotRequiredResolvedRequest = BindRequestManifestCoverage(conditionalNotRequiredResolvedRequest, false);
    ExpectRejectedMutation(failures, "conditional-not-required-now-resolved", conditionalNotRequiredResolvedRequest, "UNRESOLVED_VERSION_CONFLICT");

    var optionalMissingDegradedPolicyRequest = validRequest with
    {
        Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0
            ? dependency with
            {
                Relationship = DependencyRelationship.Optional,
                ConditionState = null,
                ResolvedVersion = null,
                DegradedStatePolicy = string.Empty
            }
            : dependency).ToArray()
    };
    optionalMissingDegradedPolicyRequest = BindRequestManifestCoverage(optionalMissingDegradedPolicyRequest, false);
    ExpectRejectedMutation(failures, "optional-missing-degraded-policy", optionalMissingDegradedPolicyRequest, "MISSING_DEGRADED_POLICY");
    ExpectRejectedMutation(failures, "invalid-subject-evidence", validRequest with { Subjects = validRequest.Subjects.Select((subject, index) => index == 2 ? ((FoundationServiceSubjectEvidence)subject) with { EvidenceReference = "", CatalogEntry = serviceEntry } : subject).ToArray() }, "MISSING_EVIDENCE_REFERENCE");
    ExpectRejectedMutation(failures, "expired-subject-evidence", validRequest with { Subjects = validRequest.Subjects.Select((subject, index) => index == 0 ? ((ExternalDependencySubjectEvidence)subject) with { Expiry = observationTime.AddMinutes(-1) } : subject).ToArray() }, "SUBJECT_EVIDENCE_EXPIRED");
    ExpectRejectedMutation(failures, "invalid-subject-kind", validRequest with { Subjects = validRequest.Subjects.Select((subject, index) => index == 0 ? subject with { SubjectKind = (DependencySubjectKind)999 } : subject).ToArray() }, "INVALID_SUBJECT_KIND");
    ExpectRejectedMutation(failures, "invalid-dependency-kind", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { DependencyKind = (DependencySubjectKind)999 } : dependency).ToArray() }, "INVALID_SUBJECT_KIND");
    ExpectRejectedMutation(failures, "invalid-lifecycle-order", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { LifecycleOrder = new DependencyLifecycleOrderDeclaration((DependencyLifecycleOrderRule)999, dependency.LifecycleOrder!.Shutdown, dependency.LifecycleOrder.Update, dependency.LifecycleOrder.Recovery, dependency.LifecycleOrder.Removal) } : dependency).ToArray() }, "INVALID_LIFECYCLE_ORDER");
    ExpectRejectedMutation(failures, "missing-delegation-chain", validRequest with { Dependencies = validRequest.Dependencies.Select((dependency, index) => index == 0 ? dependency with { DelegationChainEvidenceReference = string.Empty } : dependency).ToArray() }, "MISSING_EVIDENCE_REFERENCE");
    ExpectRejectedMutation(failures, "graph-manifest-class", validRequest with { ManifestSurface = validRequest.ManifestSurface with { ManifestClass = "ACTIVE_MANIFEST" } }, "LIFECYCLE_CLASS_SUBSTITUTION");
    ExpectRejectedMutation(failures, "graph-manifest-subject", validRequest with { ManifestSurface = validRequest.ManifestSurface with { SubjectId = "wrong-graph" } }, "GRAPH_MANIFEST_SUBJECT_MISMATCH");
    ExpectRejectedMutation(failures, "graph-manifest-digest", validRequest with { ManifestSurface = validRequest.ManifestSurface with { CanonicalDigest = "0000000000000000000000000000000000000000000000000000000000000000" } }, "GRAPH_MANIFEST_DIGEST_MISMATCH");
var graphManifestNotEffective = validRequest.ManifestSurface with { EffectiveTime = observationTime.AddHours(1) };
graphManifestNotEffective = graphManifestNotEffective with { CanonicalDigest = ComputeCandidateGraphDigest(validRequest with { ManifestSurface = graphManifestNotEffective }) };
ExpectRejectedMutation(failures, "graph-manifest-not-effective", validRequest with { ManifestSurface = graphManifestNotEffective }, "GRAPH_MANIFEST_NOT_EFFECTIVE");
var graphManifestExpired = validRequest.ManifestSurface with { Expiry = observationTime.AddMinutes(-1) };
graphManifestExpired = graphManifestExpired with { CanonicalDigest = ComputeCandidateGraphDigest(validRequest with { ManifestSurface = graphManifestExpired }) };
ExpectRejectedMutation(failures, "graph-manifest-expired", validRequest with { ManifestSurface = graphManifestExpired }, "GRAPH_MANIFEST_EXPIRED");
ExpectRejectedMutation(failures, "delegation-grantee", validRequest with { DelegationEvidence = validRequest.DelegationEvidence with { Grantee = "other" } }, "DELEGATION_GRANTEE_MISMATCH");
ExpectRejectedMutation(failures, "delegation-authority", validRequest with { DelegationEvidence = validRequest.DelegationEvidence with { AuthoritySource = "other-authority" } }, "DELEGATION_AUTHORITY_MISMATCH");
ExpectRejectedMutation(failures, "delegation-not-effective", validRequest with { DelegationEvidence = validRequest.DelegationEvidence with { EffectiveTime = observationTime.AddHours(1) } }, "DELEGATION_NOT_EFFECTIVE");
ExpectRejectedMutation(failures, "delegation-expired", validRequest with { DelegationEvidence = validRequest.DelegationEvidence with { Expiry = observationTime.AddMinutes(-1) } }, "DELEGATION_EXPIRED");
ExpectRejectedMutation(failures, "delegation-scope", validRequest with { DelegationEvidence = validRequest.DelegationEvidence with { Scope = "other-scope" } }, "DELEGATION_SCOPE_MISMATCH");
ExpectRejectedMutation(failures, "missing-activation-order", validRequest with { ProposedActivationOrder = Array.Empty<DependencySubjectKey>() }, "MISSING_ACTIVATION_ORDER");
ExpectRejectedMutation(failures, "duplicate-activation-subject", validRequest with { ProposedActivationOrder = new[] { validRequest.ProposedActivationOrder[0], validRequest.ProposedActivationOrder[0] } }, "DUPLICATE_ACTIVATION_SUBJECT");
ExpectRejectedMutation(failures, "unknown-activation-subject", validRequest with { ProposedActivationOrder = new[] { new DependencySubjectKey("unknown", "1.0"), validRequest.ProposedActivationOrder[1] } }, "UNKNOWN_ACTIVATION_SUBJECT");
ExpectRejectedMutation(failures, "incomplete-activation-order", validRequest with { ProposedActivationOrder = validRequest.ProposedActivationOrder.Take(2).ToArray() }, "INCOMPLETE_ACTIVATION_ORDER");
    ExpectRejectedMutation(failures, "dependency-after-consumer", validRequest with { ProposedActivationOrder = new[] { validRequest.ProposedActivationOrder[3], validRequest.ProposedActivationOrder[0], validRequest.ProposedActivationOrder[1], validRequest.ProposedActivationOrder[2] } }, "DEPENDENCY_AFTER_CONSUMER");
    ExpectRejectedMutation(failures, "non-canonical-order", validRequest with { ProposedActivationOrder = new[] { validRequest.ProposedActivationOrder[1], validRequest.ProposedActivationOrder[0], validRequest.ProposedActivationOrder[2], validRequest.ProposedActivationOrder[3] } }, "NON_CANONICAL_ACTIVATION_ORDER");
    VerifyPostValidationMutation(failures, validRequest);

if (failures.Count > 0)
{
    Console.Error.WriteLine("WP-04 dependency governance validation: FAIL");
    foreach (var failure in failures.Distinct(StringComparer.Ordinal))
    {
        Console.Error.WriteLine($"- {failure}");
    }

    return 1;
}

Console.WriteLine("WP-04 dependency governance validation: PASS");
Console.WriteLine("DEPENDENCY_GRAPH_VALIDATED");
Console.WriteLine("ACTIVATION_ORDER_VALIDATED");
return 0;

sealed class InMemoryAdmissionBaselineProvider : IAdmissionBaselineProvider
{
    private readonly AdmissionBaselineSnapshot _snapshot;

    public InMemoryAdmissionBaselineProvider(AdmissionBaselineSnapshot snapshot)
    {
        _snapshot = snapshot;
    }

    public AdmissionBaselineSnapshot GetCurrentBaseline() => _snapshot;
}
