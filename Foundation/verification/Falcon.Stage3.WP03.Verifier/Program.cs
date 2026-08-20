using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Admission;
using Foundation.Contracts;
using Foundation.ContractRegistry;
using Foundation.ServiceCatalog;

var failures = new List<string>();
var contractsAssemblyMarker = ContractIdentity.Con001;
var contractsRuntimeMarker = ValidationResult.Pass;
GC.KeepAlive(contractsAssemblyMarker);
GC.KeepAlive(contractsRuntimeMarker);

static bool Registered(ServiceRegistrationResult result)
    => result.Success
       && result.Decision == ServiceRegistrationDecision.Registered
       && result.RegistrationState == ServiceRegistrationState.Registered
       && result.OperationalState == ServiceOperationalState.NotActive
       && string.Equals(result.ReasonCode, "REGISTERED", StringComparison.Ordinal);

static void Expect(ICollection<string> failures, string label, bool condition, string? detail = null)
{
    if (!condition)
    {
        failures.Add(string.IsNullOrWhiteSpace(detail) ? $"{label} failed" : $"{label} failed: {detail}");
    }
}

static void ExpectRejected(ICollection<string> failures, string label, ServiceRegistrationValidationResult result, string expectedReason)
{
    if (result.Success || !string.Equals(result.ReasonCode, expectedReason, StringComparison.Ordinal))
    {
        failures.Add($"{label} expected {expectedReason} but got {(result.Success ? "PASS" : result.ReasonCode)}");
    }
}

static void ExpectRejectedRegistration(ICollection<string> failures, string label, ServiceCatalog catalog, ServiceRegistrationRequest request, string expectedReason)
{
    var beforeEntries = catalog.Entries.Count;
    var beforeHistory = catalog.HistoryFor(request.ServiceIdentity)?.Entries.Count ?? 0;
    var beforeVersions = catalog.VersionsFor(request.ServiceIdentity).Count;
    var beforeResponsibilities = catalog.ResponsibilityCount;

    var result = catalog.Register(request);
    if (result.Success || !string.Equals(result.ReasonCode, expectedReason, StringComparison.Ordinal))
    {
        failures.Add($"{label} expected {expectedReason} but got {(result.Success ? "PASS" : result.ReasonCode)}");
        return;
    }

    Expect(failures, $"{label}-entry-count-unchanged", catalog.Entries.Count == beforeEntries, $"{beforeEntries} -> {catalog.Entries.Count}");
    Expect(failures, $"{label}-history-unchanged", (catalog.HistoryFor(request.ServiceIdentity)?.Entries.Count ?? 0) == beforeHistory, $"{beforeHistory} -> {(catalog.HistoryFor(request.ServiceIdentity)?.Entries.Count ?? 0)}");
    Expect(failures, $"{label}-versions-unchanged", catalog.VersionsFor(request.ServiceIdentity).Count == beforeVersions, $"{beforeVersions} -> {catalog.VersionsFor(request.ServiceIdentity).Count}");
    Expect(failures, $"{label}-responsibilities-unchanged", catalog.ResponsibilityCount == beforeResponsibilities, $"{beforeResponsibilities} -> {catalog.ResponsibilityCount}");
}

static string Sha256(string content)
    => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content ?? string.Empty)));

static ServiceRegistrationIntent ExplicitNoGrantIntent()
    => new(RegistrationMode.Explicit, false, false, false, false, false, false);

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
    DateTimeOffset observationTime,
    ServiceProviderEvidence? providerEvidence = null)
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
        ProviderEvidence = providerEvidence
    };
}

static ServiceRegistrationRequest WithManifest(ServiceRegistrationRequest request, ServiceManifest manifest)
{
    var digest = manifest.ComputeDigest();
    return request with
    {
        Manifest = manifest,
        ManifestDigest = digest,
        RegistrationEnvelope = request.RegistrationEnvelope with { Payload = digest }
    };
}

static ApplicationManifest BuildApplicationManifest(
    string manifestId,
    string applicationIdentity,
    string applicationVersion,
    string applicationOwner,
    string applicationPurpose,
    string packageIdentity,
    string packageVersion,
    string packageContentOrIntegrityInput,
    string requiredContractVersion,
    string requiredSpecificationVersion,
    string requiredServiceVersion,
    string providerBoundary)
{
    return new ApplicationManifest(
        manifestId,
        applicationIdentity,
        applicationVersion,
        applicationOwner,
        applicationPurpose,
        packageIdentity,
        packageVersion,
        packageContentOrIntegrityInput,
        new[]
        {
            new DependencyDeclaration("CON-023", new[] { requiredContractVersion })
        },
        new[]
        {
            new FoundationRequirement("CON-023", requiredContractVersion, "Falcon Application Authority", "CON-000 / CON-023")
        },
        new[]
        {
            new FoundationRequirement("APP-001", requiredSpecificationVersion, "Falcon Application Authority", "Falcon Application Authority")
        },
        new[]
        {
            new FoundationServiceRequirement("Foundation.ServiceCatalog", requiredServiceVersion, "application admission support")
        },
        new[] { "capability-1" },
        new[] { "consumer-1" },
        new[] { new PermissionDeclaration("run", "bounded", "required") },
        new[] { new Foundation.Admission.AuthorityRequest("admit", "bounded", "required") },
        new SecurityProfile("profile", "Approved", "Isolated"),
        new ResourceRequirements("1GiB", "1 CPU", "1GiB", "bounded"),
        new ResourceRequirements("2GiB", "2 CPU", "2GiB", "bounded"),
        "degraded",
        "persistent",
        "controlled",
        "controlled",
        "evidence",
        new LifecycleBehavior("install", "validate", "register", "admit", "activate", "update", "suspend", "recover", "replace", "remove"),
        "health",
        "containment",
        false,
        new[] { new MsaDeclaration("msa-1", "Falcon Application Authority", "scope") },
        Array.Empty<MajorBranchDeclaration>(),
        Array.Empty<LsaDeclaration>(),
        "csa-policy",
        "self-dev",
        "guardian",
        "rollback");
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

static ServiceProviderEvidence BuildIdentifierEvidence(string serviceIdentity, string serviceVersion, EvidenceOrigin origin, string status, DateTimeOffset effectiveTime, DateTimeOffset expiry)
    => new IdentifierProviderEvidence
    {
        ServiceKind = ServiceKind.IdentifierProvider,
        ServiceIdentity = serviceIdentity,
        ServiceVersion = serviceVersion,
        ProviderContractIdentity = "CON-014",
        ProviderContractVersion = "1.0",
        EvidenceOrigin = origin,
        OperationalStatus = status,
        EffectiveTime = effectiveTime,
        Expiry = expiry,
        ProviderRecord = new IdentifierProviderRecord(
            serviceIdentity,
            serviceVersion,
            "identifier-provider",
            "CON-000 / CON-014",
            "bounded identity operations",
            "identity-evidence",
            status,
            "PROHIBITED",
            "validation-evidence",
            effectiveTime,
            expiry)
    };

static ServiceProviderEvidence BuildCryptographicEvidence(string serviceIdentity, string serviceVersion, EvidenceOrigin origin, string status, DateTimeOffset effectiveTime, DateTimeOffset expiry)
    => new CryptographicProviderEvidence
    {
        ServiceKind = ServiceKind.CryptographicProvider,
        ServiceIdentity = serviceIdentity,
        ServiceVersion = serviceVersion,
        ProviderContractIdentity = "CON-016",
        ProviderContractVersion = "1.0",
        EvidenceOrigin = origin,
        OperationalStatus = status,
        EffectiveTime = effectiveTime,
        Expiry = expiry,
        ProviderRecord = new CryptographicProviderRecord(
            serviceIdentity,
            serviceVersion,
            "cryptographic-provider",
            "CON-000 / CON-016",
            "bounded cryptographic operations",
            "key-ref",
            "validation-evidence",
            status,
            effectiveTime,
            expiry)
    };

static ServiceProviderEvidence BuildSecretCustodyEvidence(string serviceIdentity, string serviceVersion, EvidenceOrigin origin, string status, DateTimeOffset effectiveTime, DateTimeOffset expiry)
    => new SecretCustodyProviderEvidence
    {
        ServiceKind = ServiceKind.SecretCustodyProvider,
        ServiceIdentity = serviceIdentity,
        ServiceVersion = serviceVersion,
        ProviderContractIdentity = "CON-017",
        ProviderContractVersion = "1.0",
        EvidenceOrigin = origin,
        OperationalStatus = status,
        EffectiveTime = effectiveTime,
        Expiry = expiry,
        ProviderRecord = new SecretCustodyRecord(
            serviceIdentity,
            serviceVersion,
            serviceIdentity,
            "secret-class",
            "custody-policy",
            "access-boundary",
            "validation-evidence",
            status,
            effectiveTime,
            expiry)
    };

static ServiceProviderEvidence BuildCertificateEvidence(string serviceIdentity, string serviceVersion, EvidenceOrigin origin, string status, DateTimeOffset effectiveTime, DateTimeOffset expiry)
    => new CertificateIdentityProviderEvidence
    {
        ServiceKind = ServiceKind.CertificateIdentityProvider,
        ServiceIdentity = serviceIdentity,
        ServiceVersion = serviceVersion,
        ProviderContractIdentity = "CON-018",
        ProviderContractVersion = "1.0",
        EvidenceOrigin = origin,
        OperationalStatus = status,
        EffectiveTime = effectiveTime,
        Expiry = expiry,
        ProviderRecord = new CertificateIdentityProviderRecord(
            serviceIdentity,
            serviceVersion,
            "certificate-identity-provider",
            "CON-000 / CON-018",
            "trust-anchor",
            status,
            "validation-evidence",
            effectiveTime,
            expiry)
    };

static ServiceProviderEvidence BuildRandomnessEvidence(string serviceIdentity, string serviceVersion, EvidenceOrigin origin, string status, DateTimeOffset effectiveTime, DateTimeOffset expiry)
    => new RandomnessProviderEvidence
    {
        ServiceKind = ServiceKind.RandomnessProvider,
        ServiceIdentity = serviceIdentity,
        ServiceVersion = serviceVersion,
        ProviderContractIdentity = "CON-019",
        ProviderContractVersion = "1.0",
        EvidenceOrigin = origin,
        OperationalStatus = status,
        EffectiveTime = effectiveTime,
        Expiry = expiry,
        ProviderRecord = new RandomnessProviderRecord(
            serviceIdentity,
            serviceVersion,
            "randomness-provider",
            "CON-000 / CON-019",
            "entropy-source",
            "validation-evidence",
            status,
            effectiveTime,
            expiry)
    };

var effectiveTime = new DateTimeOffset(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);
var expiryTime = effectiveTime.AddYears(1);

var goldenManifest = BuildManifest(
    "foundation-service-catalog",
    "1.0",
    "Falcon Foundation",
    "governed service admission and registration",
    "bounded service catalog admission and registration control",
    new[] { new ServiceResponsibilityDeclaration("service-registration", "Falcon Foundation", "service registration"), new ServiceResponsibilityDeclaration("catalog-lookup", "Falcon Foundation", "catalog lookup") },
    Array.Empty<ServiceContractRequirement>(),
    new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
    new[] { "foundation-runtime" },
    new[] { "external-guest" },
    new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
    new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") },
    new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
    new ServiceProtectionDeclaration(true, true, true, true, true, true),
    "manifest-foundation-service-catalog-1.0",
    "foundation-service-catalog-package",
    "1.0",
    "package-content",
    "package governed only by approved service catalog",
    "controlled self-development path",
    "guardian interface declared");

var goldenText = goldenManifest.CanonicalText();
var goldenDigest = goldenManifest.ComputeDigest();
Expect(failures, "golden-manifest-lf-only", !goldenText.Contains("\r", StringComparison.Ordinal));
Expect(failures, "golden-manifest-digest", string.Equals(goldenDigest, "ADEEDE04F0A245B0CD1DEF296F8ABA78D2802015181C48AFE4839671FD2199A6", StringComparison.OrdinalIgnoreCase), goldenDigest);
Expect(failures, "golden-manifest-byte-length", Encoding.UTF8.GetByteCount(goldenText) == 1466, Encoding.UTF8.GetByteCount(goldenText).ToString());

var catalog = new ServiceCatalog();
Expect(failures, "catalog-empty", catalog.IsEmpty);
Expect(failures, "collision-safe-key", !string.Equals(ServiceCatalogKey.From("a@b", "c").CanonicalText, ServiceCatalogKey.From("a", "b@c").CanonicalText, StringComparison.Ordinal));

var generalRequest = BuildRequest(
    "svc-reg-001",
    "foundation-service-catalog",
    "foundation-service-catalog",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    goldenManifest,
    "service-catalog-provenance-001",
    "evidence/service-catalog/registration-001",
    effectiveTime);

var generalResult = catalog.Register(generalRequest);
Expect(failures, "general-registration", Registered(generalResult), generalResult.ReasonCode);
Expect(failures, "lookup-by-identity", catalog.Lookup("foundation-service-catalog", "1.0") is not null);
Expect(failures, "lookup-by-key", catalog.Lookup(ServiceCatalogKey.From("foundation-service-catalog", "1.0")) is not null);
Expect(failures, "versions-for", catalog.VersionsFor("foundation-service-catalog").SequenceEqual(new[] { "1.0" }, StringComparer.Ordinal));
Expect(failures, "history-for", catalog.HistoryFor("foundation-service-catalog")?.Entries.Count == 1);
Expect(failures, "responsibility-owner-lookup", catalog.LookupResponsibilityOwner("service-registration", "service registration") is not null);
Expect(failures, "consumer-authorized", catalog.IsAuthorizedConsumer("foundation-service-catalog", "1.0", "foundation-runtime"));
Expect(failures, "consumer-restricted", !catalog.IsAuthorizedConsumer("foundation-service-catalog", "1.0", "external-guest"));
Expect(failures, "stable-enumeration", catalog.Entries.Count == 1);

var mutableResponsibilities = new List<ServiceResponsibilityDeclaration> { new("immutable-resp", "Falcon Foundation", "immutable responsibility") };
var mutableProvidedContracts = new List<ServiceContractRequirement>
{
    new("CON-023", "1.1", "operational control", "provides")
};
var mutableConsumedVersions = new List<string> { "1.1" };
var mutableConsumedContracts = new List<ServiceConsumedContractRequirement>
{
    new("CON-023", mutableConsumedVersions, "controlled service admission", "requires")
};
var mutableAuthorizedConsumers = new List<string> { "foundation-runtime" };
var mutableRestrictedConsumers = new List<string> { "external-guest" };
var mutableLifecycleTransitions = new List<string> { "registered" };
var mutableDependenciesVersions = new List<string> { "1.1" };
var mutableDependencies = new List<ServiceDependencyDeclaration>
{
    new("foundation-contract-registry", mutableDependenciesVersions, "governed-service", "self-description", "service registration control", "reject")
};
var mutableManifest = BuildManifest(
    "foundation-immutable-snapshot",
    "1.0",
    "Falcon Foundation",
    "immutable snapshot",
    "bounded immutable snapshot",
    mutableResponsibilities,
    mutableProvidedContracts,
    mutableConsumedContracts,
    mutableAuthorizedConsumers,
    mutableRestrictedConsumers,
    new ServiceLifecycleDeclaration("prepared", mutableLifecycleTransitions, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
    mutableDependencies,
    new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
    new ServiceProtectionDeclaration(true, true, true, true, true, true),
    "manifest-immutable-snapshot",
    "foundation-immutable-package",
    "1.0",
    "package-content",
    "policy",
    "self-dev",
    "guardian interface declared");
var mutableRequest = BuildRequest(
    "svc-reg-immutable",
    "foundation-immutable-snapshot",
    "foundation-immutable-snapshot",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    mutableManifest,
    "service-catalog-provenance-immutable",
    "evidence/service-catalog/immutable",
    effectiveTime);
var mutableDigest = mutableManifest.ComputeDigest();
var mutableCatalog = new ServiceCatalog();
Expect(failures, "immutable-registration", Registered(mutableCatalog.Register(mutableRequest)));
var mutableSnapshot = mutableCatalog.Lookup("foundation-immutable-snapshot", "1.0");
Expect(failures, "immutable-registration-snapshot", mutableSnapshot is not null);
Expect(failures, "immutable-registration-digest", mutableSnapshot is not null && string.Equals(mutableSnapshot.Entry.Manifest.ComputeDigest(), mutableDigest, StringComparison.Ordinal));

mutableResponsibilities[0] = new ServiceResponsibilityDeclaration("immutable-resp", "Changed Owner", "changed responsibility");
mutableProvidedContracts[0] = new ServiceContractRequirement("CON-023", "9.9", "changed operational control", "provides");
mutableConsumedVersions[0] = "9.9";
mutableConsumedContracts[0] = new ServiceConsumedContractRequirement("CON-023", mutableConsumedVersions, "changed controlled service admission", "requires");
mutableAuthorizedConsumers.Add("new-consumer");
mutableRestrictedConsumers[0] = "other-restriction";
mutableLifecycleTransitions[0] = "changed";
mutableDependenciesVersions[0] = "9.9";
mutableDependencies[0] = new ServiceDependencyDeclaration("foundation-contract-registry", mutableDependenciesVersions, "changed governed-service", "changed self-description", "changed service registration control", "reject");

Expect(failures, "post-registration-mutation-digest", mutableSnapshot is not null && string.Equals(mutableSnapshot.Entry.Manifest.ComputeDigest(), mutableDigest, StringComparison.Ordinal));
Expect(failures, "post-registration-mutation-responsibilities", mutableSnapshot is not null && mutableSnapshot.Entry.Manifest.OwnedResponsibilities.Count == 1 && mutableSnapshot.Entry.Manifest.OwnedResponsibilities[0] == new ServiceResponsibilityDeclaration("immutable-resp", "Falcon Foundation", "immutable responsibility"), "responsibilities changed");
Expect(failures, "post-registration-mutation-provided-contracts", mutableSnapshot is not null && mutableSnapshot.Entry.Manifest.ProvidedContracts.Count == 1 && mutableSnapshot.Entry.Manifest.ProvidedContracts[0] == new ServiceContractRequirement("CON-023", "1.1", "operational control", "provides"), "provided contracts changed");
Expect(failures, "post-registration-mutation-consumed-contracts", mutableSnapshot is not null && mutableSnapshot.Entry.Manifest.ConsumedContracts.Count == 1 && mutableSnapshot.Entry.Manifest.ConsumedContracts[0].ContractIdentity == "CON-023" && mutableSnapshot.Entry.Manifest.ConsumedContracts[0].Purpose == "controlled service admission" && mutableSnapshot.Entry.Manifest.ConsumedContracts[0].Relation == "requires" && mutableSnapshot.Entry.Manifest.ConsumedContracts[0].CompatibleVersions.SequenceEqual(new[] { "1.1" }, StringComparer.Ordinal), "consumed contracts changed");
Expect(failures, "post-registration-mutation-authorized-consumers", mutableSnapshot is not null && mutableSnapshot.Entry.Manifest.AuthorizedConsumers.SequenceEqual(new[] { "foundation-runtime" }, StringComparer.Ordinal), "authorized consumers changed");
Expect(failures, "post-registration-mutation-restricted-consumers", mutableSnapshot is not null && mutableSnapshot.Entry.Manifest.RestrictedConsumers.SequenceEqual(new[] { "external-guest" }, StringComparer.Ordinal), "restricted consumers changed");
Expect(failures, "post-registration-mutation-lifecycle", mutableSnapshot is not null && mutableSnapshot.Entry.Manifest.Lifecycle.SupportedTransitions.SequenceEqual(new[] { "registered" }, StringComparer.Ordinal), "lifecycle transitions changed");
Expect(failures, "post-registration-mutation-dependencies", mutableSnapshot is not null && mutableSnapshot.Entry.Manifest.Dependencies.Count == 1 && mutableSnapshot.Entry.Manifest.Dependencies[0].Identity == "foundation-contract-registry" && mutableSnapshot.Entry.Manifest.Dependencies[0].Kind == "governed-service" && mutableSnapshot.Entry.Manifest.Dependencies[0].Relation == "self-description" && mutableSnapshot.Entry.Manifest.Dependencies[0].Purpose == "service registration control" && mutableSnapshot.Entry.Manifest.Dependencies[0].DegradedBehavior == "reject" && mutableSnapshot.Entry.Manifest.Dependencies[0].CompatibleVersions.SequenceEqual(new[] { "1.1" }, StringComparer.Ordinal), "dependencies changed");

var lineageCatalog = new ServiceCatalog();
var lineageV1 = BuildRequest(
    "svc-reg-002",
    "foundation-lineage-service",
    "foundation-lineage-service",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-lineage-service",
        "1.0",
        "Falcon Foundation",
        "lineage service",
        "bounded lineage service",
        new[] { new ServiceResponsibilityDeclaration("lineage-ownership", "Falcon Foundation", "lineage responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") },
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-lineage-1",
        "foundation-lineage-package",
        "1.0",
        "package-content",
        "lineage policy",
        "self-dev",
        "guardian interface declared"),
    "provenance-lineage-1",
    "evidence/lineage-1",
    effectiveTime);
Expect(failures, "lineage-v1", Registered(lineageCatalog.Register(lineageV1)));

var lineageV2 = lineageV1 with
{
    RegistrationId = "svc-reg-003",
    ServiceVersion = "1.1",
    Manifest = BuildManifest(
        "foundation-lineage-service",
        "1.1",
        "Falcon Foundation",
        "lineage service",
        "bounded lineage service",
        new[] { new ServiceResponsibilityDeclaration("lineage-ownership", "Falcon Foundation", "lineage responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") },
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-lineage-2",
        "foundation-lineage-package",
        "1.1",
        "package-content",
        "lineage policy",
        "self-dev",
        "guardian interface declared"),
    ManifestDigest = string.Empty,
    ProvenanceContent = "provenance-lineage-2",
    ProvenanceDigest = string.Empty,
    RegistrationEnvelope = null!
};
lineageV2 = lineageV2 with
{
    ManifestDigest = lineageV2.Manifest.ComputeDigest(),
    ProvenanceDigest = Sha256(lineageV2.ProvenanceContent),
    RegistrationEnvelope = BuildRequest(
        "svc-reg-003-envelope",
        "foundation-lineage-service",
        "foundation-lineage-service",
        "1.1",
        "Falcon Foundation",
        ServiceKind.GeneralFoundationService,
        ExplicitNoGrantIntent(),
        lineageV2.Manifest,
        lineageV2.ProvenanceContent,
        "evidence/lineage-2",
        effectiveTime).RegistrationEnvelope
};
Expect(failures, "lineage-v2", Registered(lineageCatalog.Register(lineageV2)));
Expect(failures, "lineage-history-count", lineageCatalog.HistoryFor("foundation-lineage-service")?.Entries.Count == 2);

var ownerCollisionCatalog = new ServiceCatalog();
var ownerCollisionFirst = BuildRequest(
    "svc-reg-010",
    "foundation-owner-a",
    "foundation-owner-a",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-owner-a",
        "1.0",
        "Falcon Foundation",
        "owner a",
        "bounded owner a",
        new[] { new ServiceResponsibilityDeclaration("ownership-key", "Falcon Foundation", "same meaning") },
        Array.Empty<ServiceContractRequirement>(),
        Array.Empty<ServiceConsumedContractRequirement>(),
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-owner-a",
        "foundation-owner-a-package",
        "1.0",
        "package-content",
        "owner policy",
        "self-dev",
        "guardian interface declared"),
    "provenance-owner-a",
    "evidence/owner-a",
    effectiveTime);
Expect(failures, "owner-collision-seed", Registered(ownerCollisionCatalog.Register(ownerCollisionFirst)));
var ownerCollisionSecond = BuildRequest(
    "svc-reg-011",
    "foundation-owner-b",
    "foundation-owner-b",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-owner-b",
        "1.0",
        "Falcon Foundation",
        "owner b",
        "bounded owner b",
        new[] { new ServiceResponsibilityDeclaration("ownership-key", "Falcon Foundation", "same meaning") },
        Array.Empty<ServiceContractRequirement>(),
        Array.Empty<ServiceConsumedContractRequirement>(),
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-owner-b",
        "foundation-owner-b-package",
        "1.0",
        "package-content",
        "owner policy",
        "self-dev",
        "guardian interface declared"),
    "provenance-owner-b",
    "evidence/owner-b",
    effectiveTime);
ExpectRejected(failures, "responsibility-collision", ownerCollisionCatalog.Validate(ownerCollisionSecond), "RESPONSIBILITY_IDENTITY_COLLISION");

var responsibilityMeaningCollisionCatalog = new ServiceCatalog();
var responsibilityMeaningCollisionFirst = BuildRequest(
    "svc-reg-011a",
    "foundation-meaning-a",
    "foundation-meaning-a",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-meaning-a",
        "1.0",
        "Falcon Foundation",
        "meaning a",
        "bounded meaning a",
        new[] { new ServiceResponsibilityDeclaration("meaning-a", "Falcon Foundation", "shared meaning") },
        Array.Empty<ServiceContractRequirement>(),
        Array.Empty<ServiceConsumedContractRequirement>(),
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-meaning-a",
        "foundation-meaning-a-package",
        "1.0",
        "package-content",
        "owner policy",
        "self-dev",
        "guardian interface declared"),
    "provenance-meaning-a",
    "evidence/meaning-a",
    effectiveTime);
Expect(failures, "meaning-collision-seed", Registered(responsibilityMeaningCollisionCatalog.Register(responsibilityMeaningCollisionFirst)));
var responsibilityMeaningCollisionSecond = BuildRequest(
    "svc-reg-011b",
    "foundation-meaning-b",
    "foundation-meaning-b",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-meaning-b",
        "1.0",
        "Falcon Foundation",
        "meaning b",
        "bounded meaning b",
        new[] { new ServiceResponsibilityDeclaration("meaning-b", "Falcon Foundation", "shared meaning") },
        Array.Empty<ServiceContractRequirement>(),
        Array.Empty<ServiceConsumedContractRequirement>(),
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-meaning-b",
        "foundation-meaning-b-package",
        "1.0",
        "package-content",
        "owner policy",
        "self-dev",
        "guardian interface declared"),
    "provenance-meaning-b",
    "evidence/meaning-b",
    effectiveTime);
ExpectRejected(failures, "responsibility-meaning-collision", responsibilityMeaningCollisionCatalog.Validate(responsibilityMeaningCollisionSecond), "RESPONSIBILITY_MEANING_COLLISION");

var lineageMismatchRequest = BuildRequest(
    "svc-reg-012",
    "foundation-lineage-service",
    "foundation-lineage-service",
    "2.0",
    "Changed Owner",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-lineage-service",
        "2.0",
        "Changed Owner",
        "lineage service",
        "bounded lineage service",
        new[] { new ServiceResponsibilityDeclaration("lineage-ownership", "Changed Owner", "lineage responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") },
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-lineage-3",
        "foundation-lineage-package",
        "2.0",
        "package-content",
        "lineage policy",
        "self-dev",
        "guardian interface declared"),
    "provenance-lineage-3",
    "evidence/lineage-3",
    effectiveTime);
ExpectRejected(failures, "owner-lineage-mismatch", lineageCatalog.Validate(lineageMismatchRequest), "OWNER_LINEAGE_MISMATCH");

var responsibilityLineageMismatchRequest = BuildRequest(
    "svc-reg-013",
    "foundation-lineage-service",
    "foundation-lineage-service",
    "2.1",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-lineage-service",
        "2.1",
        "Falcon Foundation",
        "lineage service",
        "bounded lineage service",
        new[] { new ServiceResponsibilityDeclaration("lineage-ownership", "Falcon Foundation", "changed responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") },
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-lineage-4",
        "foundation-lineage-package",
        "2.1",
        "package-content",
        "lineage policy",
        "self-dev",
        "guardian interface declared"),
    "provenance-lineage-4",
    "evidence/lineage-4",
    effectiveTime);
ExpectRejected(failures, "responsibility-lineage-mismatch", lineageCatalog.Validate(responsibilityLineageMismatchRequest), "RESPONSIBILITY_LINEAGE_MISMATCH");

ExpectRejectedRegistration(failures, "missing-requester", catalog, generalRequest with { RegistrationId = "svc-reg-missing-requester", RequesterIdentity = "" }, "MISSING_REQUESTER");
ExpectRejectedRegistration(failures, "requester-mismatch", catalog, generalRequest with { RegistrationId = "svc-reg-requester-mismatch", RequesterIdentity = "other-requester" }, "PRODUCER_REQUESTER_MISMATCH");
ExpectRejectedRegistration(failures, "future-envelope", catalog, generalRequest with { RegistrationId = "svc-reg-future", ObservationTime = effectiveTime.AddHours(-2), RegistrationEnvelope = generalRequest.RegistrationEnvelope with { CreationTime = effectiveTime.AddHours(-1), Expiry = effectiveTime.AddHours(1) } }, "ENVELOPE_NOT_YET_VALID");
ExpectRejectedRegistration(failures, "expired-envelope", catalog, generalRequest with { RegistrationId = "svc-reg-expired", ObservationTime = effectiveTime.AddHours(2), RegistrationEnvelope = generalRequest.RegistrationEnvelope with { CreationTime = effectiveTime, Expiry = effectiveTime.AddHours(1) } }, "ENVELOPE_EXPIRED");
ExpectRejectedRegistration(failures, "payload-mismatch", catalog, generalRequest with { RegistrationId = "svc-reg-payload", RegistrationEnvelope = generalRequest.RegistrationEnvelope with { Payload = "mismatch" } }, "MANIFEST_PAYLOAD_MISMATCH");
ExpectRejectedRegistration(failures, "missing-registration-evidence", catalog, generalRequest with { RegistrationId = "svc-reg-evidence", RegistrationEvidenceReference = "" }, "MISSING_REGISTRATION_EVIDENCE");
ExpectRejectedRegistration(failures, "invalid-envelope-kind", catalog, generalRequest with { RegistrationId = "svc-reg-kind", RegistrationEnvelope = generalRequest.RegistrationEnvelope with { MessageKind = "EVENT" } }, "INVALID_REGISTRATION_ENVELOPE_MESSAGE_KIND");
ExpectRejectedRegistration(failures, "invalid-envelope-type", catalog, generalRequest with { RegistrationId = "svc-reg-type", RegistrationEnvelope = generalRequest.RegistrationEnvelope with { MessageType = "SERVICE_REGISTRATION_EVENT" } }, "INVALID_REGISTRATION_ENVELOPE_MESSAGE_TYPE");
ExpectRejectedRegistration(failures, "invalid-envelope-schema-id", catalog, generalRequest with { RegistrationId = "svc-reg-schema-id", RegistrationEnvelope = generalRequest.RegistrationEnvelope with { SchemaId = "foundation.invalid-registration" } }, "INVALID_REGISTRATION_ENVELOPE_SCHEMA_ID");
ExpectRejectedRegistration(failures, "invalid-envelope-schema-version", catalog, generalRequest with { RegistrationId = "svc-reg-schema-version", RegistrationEnvelope = generalRequest.RegistrationEnvelope with { SchemaVersion = "2.0" } }, "INVALID_REGISTRATION_ENVELOPE_SCHEMA_VERSION");
ExpectRejectedRegistration(failures, "invalid-envelope-purpose", catalog, generalRequest with { RegistrationId = "svc-reg-purpose", RegistrationEnvelope = generalRequest.RegistrationEnvelope with { Purpose = "service registration only" } }, "INVALID_REGISTRATION_ENVELOPE_PURPOSE");
ExpectRejectedRegistration(failures, "duplicate-registration-id", catalog, generalRequest with { RegistrationId = "svc-reg-001" }, "DUPLICATE_REGISTRATION_ID");
ExpectRejectedRegistration(failures, "duplicate-service-version", catalog, generalRequest with { RegistrationId = "svc-reg-009" }, "DUPLICATE_SERVICE_VERSION");
ExpectRejectedRegistration(failures, "owner-mismatch", catalog, generalRequest with { RegistrationId = "svc-reg-owner", AccountableOwner = "Other Owner" }, "OWNER_MISMATCH");
ExpectRejectedRegistration(failures, "identity-mismatch", catalog, generalRequest with { RegistrationId = "svc-reg-identity", ServiceIdentity = "other-service" }, "IDENTITY_MISMATCH");
ExpectRejectedRegistration(failures, "version-mismatch", catalog, generalRequest with { RegistrationId = "svc-reg-version", ServiceVersion = "2.0" }, "VERSION_MISMATCH");
ExpectRejectedRegistration(failures, "manifest-digest-mismatch", catalog, generalRequest with { RegistrationId = "svc-reg-manifest-digest", ManifestDigest = "0000000000000000000000000000000000000000000000000000000000000000" }, "MANIFEST_DIGEST_MISMATCH");
ExpectRejectedRegistration(failures, "provenance-digest-mismatch", catalog, generalRequest with { RegistrationId = "svc-reg-provenance-digest", ProvenanceDigest = "0000000000000000000000000000000000000000000000000000000000000000" }, "PROVENANCE_DIGEST_MISMATCH");
ExpectRejectedRegistration(failures, "automatic-registration", catalog, generalRequest with { RegistrationId = "svc-reg-auto-reg", Intent = new ServiceRegistrationIntent(RegistrationMode.Automatic, false, false, false, false, false, false) }, "AUTOMATIC_REGISTRATION_PROHIBITED");
ExpectRejectedRegistration(failures, "automatic-activation", catalog, generalRequest with { RegistrationId = "svc-reg-auto-act", Intent = new ServiceRegistrationIntent(RegistrationMode.Explicit, true, false, false, false, false, false) }, "AUTOMATIC_ACTIVATION_PROHIBITED");
ExpectRejectedRegistration(failures, "admission-request", catalog, generalRequest with { RegistrationId = "svc-reg-admission", Intent = new ServiceRegistrationIntent(RegistrationMode.Explicit, false, true, false, false, false, false) }, "ADMISSION_GRANT_PROHIBITED");
ExpectRejectedRegistration(failures, "authority-request", catalog, generalRequest with { RegistrationId = "svc-reg-authority", Intent = new ServiceRegistrationIntent(RegistrationMode.Explicit, false, false, true, false, false, false) }, "AUTHORITY_GRANT_PROHIBITED");
ExpectRejectedRegistration(failures, "permission-request", catalog, generalRequest with { RegistrationId = "svc-reg-permission", Intent = new ServiceRegistrationIntent(RegistrationMode.Explicit, false, false, false, true, false, false) }, "PERMISSION_GRANT_PROHIBITED");
ExpectRejectedRegistration(failures, "trust-request", catalog, generalRequest with { RegistrationId = "svc-reg-trust", Intent = new ServiceRegistrationIntent(RegistrationMode.Explicit, false, false, false, false, true, false) }, "TRUST_GRANT_PROHIBITED");
ExpectRejectedRegistration(failures, "responsibility-request", catalog, generalRequest with { RegistrationId = "svc-reg-resp", Intent = new ServiceRegistrationIntent(RegistrationMode.Explicit, false, false, false, false, false, true) }, "RESPONSIBILITY_GAIN_PROHIBITED");
ExpectRejectedRegistration(failures, "duplicate-authorized-consumer", catalog, WithManifest(generalRequest with { ServiceVersion = "1.1" }, goldenManifest with { ServiceVersion = "1.1", AuthorizedConsumers = new ReadOnlyCollection<string>(new[] { "foundation-runtime", "shared-a", "shared-a" }), RestrictedConsumers = new ReadOnlyCollection<string>(new[] { "shared-b", "external-guest" }) }) with { RegistrationId = "svc-reg-dup-authorized-consumer" }, "DUPLICATE_AUTHORIZED_CONSUMER");
ExpectRejectedRegistration(failures, "duplicate-restricted-consumer", catalog, WithManifest(generalRequest with { ServiceVersion = "1.1" }, goldenManifest with { ServiceVersion = "1.1", AuthorizedConsumers = new ReadOnlyCollection<string>(new[] { "foundation-runtime", "shared-a" }), RestrictedConsumers = new ReadOnlyCollection<string>(new[] { "shared-b", "external-guest", "external-guest" }) }) with { RegistrationId = "svc-reg-dup-restricted-consumer" }, "DUPLICATE_RESTRICTED_CONSUMER");
ExpectRejectedRegistration(failures, "consumer-policy-conflict", catalog, WithManifest(generalRequest with { ServiceVersion = "1.1" }, goldenManifest with { ServiceVersion = "1.1", AuthorizedConsumers = new ReadOnlyCollection<string>(new[] { "foundation-runtime", "shared" }), RestrictedConsumers = new ReadOnlyCollection<string>(new[] { "external-guest", "shared" }) }) with { RegistrationId = "svc-reg-policy-conflict" }, "CONSUMER_POLICY_CONFLICT");
ExpectRejectedRegistration(failures, "duplicate-provided-contract", catalog, WithManifest(generalRequest, goldenManifest with { ProvidedContracts = new ReadOnlyCollection<ServiceContractRequirement>(new[] { new ServiceContractRequirement("CON-023", "1.1", "operational control", "provides"), new ServiceContractRequirement("CON-023", "1.1", "operational control", "provides") }) }) with { RegistrationId = "svc-reg-dup-provided" }, "DUPLICATE_PROVIDED_CONTRACT");
ExpectRejectedRegistration(failures, "duplicate-consumed-contract", catalog, WithManifest(generalRequest, goldenManifest with { ConsumedContracts = new ReadOnlyCollection<ServiceConsumedContractRequirement>(new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "controlled service admission", "requires"), new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "controlled service admission", "requires") }) }) with { RegistrationId = "svc-reg-dup-consumed" }, "DUPLICATE_CONSUMED_CONTRACT");
ExpectRejectedRegistration(failures, "duplicate-dependency", catalog, WithManifest(generalRequest, goldenManifest with { Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject"), new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") }) }) with { RegistrationId = "svc-reg-dup-dependency" }, "DUPLICATE_DEPENDENCY");
ExpectRejectedRegistration(failures, "duplicate-lifecycle-transition", catalog, WithManifest(generalRequest, goldenManifest with { Lifecycle = new ServiceLifecycleDeclaration("prepared", new[] { "registered", "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal") }) with { RegistrationId = "svc-reg-dup-transition" }, "DUPLICATE_LIFECYCLE_TRANSITION");
ExpectRejectedRegistration(failures, "duplicate-consumed-compatible-version", new ServiceCatalog(), WithManifest(generalRequest, goldenManifest with { ConsumedContracts = new ReadOnlyCollection<ServiceConsumedContractRequirement>(new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1", "1.1" }, "controlled service admission", "requires") }) }) with { RegistrationId = "svc-reg-dup-consumed-version" }, "DUPLICATE_COMPATIBLE_VERSION");
ExpectRejectedRegistration(failures, "duplicate-dependency-compatible-version", new ServiceCatalog(), WithManifest(generalRequest, goldenManifest with { Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1", "1.1" }, "governed-service", "self-description", "service registration control", "reject") }) }) with { RegistrationId = "svc-reg-dup-dependency-version" }, "DUPLICATE_COMPATIBLE_VERSION");
ExpectRejectedRegistration(failures, "missing-purpose", catalog, WithManifest(generalRequest, goldenManifest with { Purpose = "" }) with { RegistrationId = "svc-reg-missing-purpose" }, "MISSING_PURPOSE");
ExpectRejectedRegistration(failures, "missing-boundary", catalog, WithManifest(generalRequest, goldenManifest with { ExclusiveResponsibilityBoundary = "" }) with { RegistrationId = "svc-reg-missing-boundary" }, "MISSING_RESPONSIBILITY_BOUNDARY");
ExpectRejectedRegistration(failures, "missing-responsibility", catalog, WithManifest(generalRequest, goldenManifest with { OwnedResponsibilities = new ReadOnlyCollection<ServiceResponsibilityDeclaration>(Array.Empty<ServiceResponsibilityDeclaration>()) }) with { RegistrationId = "svc-reg-missing-responsibility" }, "MISSING_RESPONSIBILITY");
ExpectRejectedRegistration(failures, "duplicate-responsibility-identity", catalog, WithManifest(generalRequest, goldenManifest with { OwnedResponsibilities = new ReadOnlyCollection<ServiceResponsibilityDeclaration>(new[] { new ServiceResponsibilityDeclaration("same-identity", "Falcon Foundation", "responsibility a"), new ServiceResponsibilityDeclaration("same-identity", "Falcon Foundation", "responsibility b") }) }) with { RegistrationId = "svc-reg-dup-resp-id" }, "DUPLICATE_RESPONSIBILITY_IDENTITY");
ExpectRejectedRegistration(failures, "duplicate-responsibility-meaning", catalog, WithManifest(generalRequest, goldenManifest with { OwnedResponsibilities = new ReadOnlyCollection<ServiceResponsibilityDeclaration>(new[] { new ServiceResponsibilityDeclaration("responsibility-a", "Falcon Foundation", "same meaning"), new ServiceResponsibilityDeclaration("responsibility-b", "Falcon Foundation", "same meaning") }) }) with { RegistrationId = "svc-reg-dup-resp-meaning" }, "DUPLICATE_RESPONSIBILITY_MEANING");
ExpectRejectedRegistration(failures, "duplicate-responsibility-pair", catalog, WithManifest(generalRequest, goldenManifest with { OwnedResponsibilities = new ReadOnlyCollection<ServiceResponsibilityDeclaration>(new[] { new ServiceResponsibilityDeclaration("responsibility-a", "Falcon Foundation", "same meaning"), new ServiceResponsibilityDeclaration("responsibility-a", "Falcon Foundation", "same meaning") }) }) with { RegistrationId = "svc-reg-dup-resp-pair" }, "DUPLICATE_RESPONSIBILITY");
ExpectRejectedRegistration(failures, "invalid-lifecycle", catalog, WithManifest(generalRequest, goldenManifest with { Lifecycle = new ServiceLifecycleDeclaration("prepared", Array.Empty<string>(), "x", "y", "z", "w") }) with { RegistrationId = "svc-reg-invalid-lifecycle" }, "INVALID_LIFECYCLE_DECLARATION");
ExpectRejectedRegistration(failures, "invalid-operational-boundary", catalog, WithManifest(generalRequest, goldenManifest with { OperationalBoundary = new ServiceOperationalBoundary("", "h", "r", "f", "p", "a", "e", "p", "i", "n") }) with { RegistrationId = "svc-reg-invalid-boundary" }, "INVALID_OPERATIONAL_BOUNDARY");
ExpectRejectedRegistration(failures, "invalid-protection", catalog, WithManifest(generalRequest, goldenManifest with { Protection = new ServiceProtectionDeclaration(true, false, true, true, true, true) }) with { RegistrationId = "svc-reg-invalid-protection" }, "INVALID_PROTECTION_DECLARATION");
ExpectRejectedRegistration(failures, "invalid-dependency", catalog, WithManifest(generalRequest, goldenManifest with { Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[] { new ServiceDependencyDeclaration("", new[] { "1.0" }, "kind", "relation", "purpose", "degraded") }) }) with { RegistrationId = "svc-reg-invalid-dependency" }, "INVALID_DEPENDENCY");
ExpectRejectedRegistration(failures, "direct-self-dependency", catalog, WithManifest(generalRequest, goldenManifest with { Dependencies = new ReadOnlyCollection<ServiceDependencyDeclaration>(new[] { new ServiceDependencyDeclaration("foundation-service-catalog", new[] { "1.0" }, "kind", "relation", "purpose", "degraded") }) }) with { RegistrationId = "svc-reg-self-dep" }, "DIRECT_SELF_DEPENDENCY");
ExpectRejectedRegistration(failures, "unknown-contract", catalog, WithManifest(generalRequest, goldenManifest with { ConsumedContracts = new ReadOnlyCollection<ServiceConsumedContractRequirement>(new[] { new ServiceConsumedContractRequirement("CON-999", new[] { "1.0" }, "unknown", "requires") }) }) with { RegistrationId = "svc-reg-unknown" }, "UNKNOWN_CONTRACT");
ExpectRejectedRegistration(failures, "unsupported-contract-version", catalog, WithManifest(generalRequest, goldenManifest with { ConsumedContracts = new ReadOnlyCollection<ServiceConsumedContractRequirement>(new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "9.9" }, "unknown", "requires") }) }) with { RegistrationId = "svc-reg-unsupported" }, "INCOMPATIBLE_CONTRACT_VERSION");

var providerBase = BuildRequest(
    "svc-reg-provider-base",
    "foundation-identifier-provider",
    "foundation-identifier-provider",
    "1.0",
    "Falcon Foundation",
    ServiceKind.IdentifierProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-identifier-provider",
        "1.0",
        "Falcon Foundation",
        "identifier provider",
        "bounded identifier provider",
        new[] { new ServiceResponsibilityDeclaration("identifier-responsibility", "Falcon Foundation", "identifier responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-provider-base",
        "foundation-identifier-provider-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "provider-provenance",
    "evidence/provider",
    effectiveTime,
    BuildIdentifierEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime));

var providerCatalog = new ServiceCatalog();
var providerResult = providerCatalog.Register(providerBase);
Expect(failures, "provider-registration-valid", Registered(providerResult), providerResult.ReasonCode);
Expect(failures, "provider-entry-count", providerCatalog.Entries.Count == 1);

ExpectRejectedRegistration(failures, "missing-provider", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-missing", ProviderEvidence = null }, "MISSING_PROVIDER_EVIDENCE");
ExpectRejectedRegistration(failures, "wrong-provider-type", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-wrong", ProviderEvidence = BuildCryptographicEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime) }, "WRONG_PROVIDER_EVIDENCE_TYPE");

var providerAlphaCatalog = new ServiceCatalog();
var providerAlphaEvidence = (IdentifierProviderEvidence)BuildIdentifierEvidence("foundation-identifier-provider-alpha", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime);
providerAlphaEvidence = providerAlphaEvidence with { ProviderRecord = providerAlphaEvidence.ProviderRecord with { ProviderClass = "alpha-provider" } };
var providerAlphaRequest = BuildRequest(
    "svc-reg-provider-alpha",
    "foundation-identifier-provider-alpha",
    "foundation-identifier-provider-alpha",
    "1.0",
    "Falcon Foundation",
    ServiceKind.IdentifierProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-identifier-provider-alpha",
        "1.0",
        "Falcon Foundation",
        "identifier provider",
        "bounded identifier provider",
        new[] { new ServiceResponsibilityDeclaration("identifier-responsibility-alpha", "Falcon Foundation", "identifier responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-provider-alpha",
        "foundation-identifier-provider-alpha-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "provider-provenance-alpha",
    "evidence/provider/alpha",
    effectiveTime,
    providerAlphaEvidence);
Expect(failures, "provider-alpha-valid", Registered(providerAlphaCatalog.Register(providerAlphaRequest)));

var providerBetaCatalog = new ServiceCatalog();
var providerBetaEvidence = (IdentifierProviderEvidence)BuildIdentifierEvidence("foundation-identifier-provider-beta", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime);
providerBetaEvidence = providerBetaEvidence with { ProviderRecord = providerBetaEvidence.ProviderRecord with { ProviderClass = "beta-provider" } };
var providerBetaRequest = BuildRequest(
    "svc-reg-provider-beta",
    "foundation-identifier-provider-beta",
    "foundation-identifier-provider-beta",
    "1.0",
    "Falcon Foundation",
    ServiceKind.IdentifierProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-identifier-provider-beta",
        "1.0",
        "Falcon Foundation",
        "identifier provider",
        "bounded identifier provider",
        new[] { new ServiceResponsibilityDeclaration("identifier-responsibility-beta", "Falcon Foundation", "identifier responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-provider-beta",
        "foundation-identifier-provider-beta-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "provider-provenance-beta",
    "evidence/provider/beta",
    effectiveTime,
    providerBetaEvidence);
Expect(failures, "provider-beta-valid", Registered(providerBetaCatalog.Register(providerBetaRequest)));

var blankProviderClassEvidence = (IdentifierProviderEvidence)BuildIdentifierEvidence("foundation-identifier-provider-blank", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime);
blankProviderClassEvidence = blankProviderClassEvidence with { ProviderRecord = blankProviderClassEvidence.ProviderRecord with { ProviderClass = "" } };
var blankProviderClassRequest = BuildRequest(
    "svc-reg-provider-blank-class",
    "foundation-identifier-provider-blank",
    "foundation-identifier-provider-blank",
    "1.0",
    "Falcon Foundation",
    ServiceKind.IdentifierProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-identifier-provider-blank",
        "1.0",
        "Falcon Foundation",
        "identifier provider",
        "bounded identifier provider",
        new[] { new ServiceResponsibilityDeclaration("identifier-responsibility-blank", "Falcon Foundation", "identifier responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-provider-blank",
        "foundation-identifier-provider-blank-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "provider-provenance-blank",
    "evidence/provider/blank",
    effectiveTime,
    blankProviderClassEvidence);
ExpectRejectedRegistration(failures, "provider-class-blank", new ServiceCatalog(), blankProviderClassRequest, "MALFORMED_PROVIDER_EVIDENCE");
ExpectRejectedRegistration(failures, "provider-identity-mismatch", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-identity", ProviderEvidence = BuildIdentifierEvidence("other-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime) }, "PROVIDER_IDENTITY_MISMATCH");
var providerVersionMismatchEvidence = (IdentifierProviderEvidence)BuildIdentifierEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime);
providerVersionMismatchEvidence = providerVersionMismatchEvidence with { ServiceVersion = "2.0" };
ExpectRejectedRegistration(failures, "provider-version-mismatch", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-version", ProviderEvidence = providerVersionMismatchEvidence }, "PROVIDER_VERSION_MISMATCH");
var providerContractMismatchEvidence = (IdentifierProviderEvidence)BuildIdentifierEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime);
ExpectRejectedRegistration(failures, "provider-contract-mismatch", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-contract", ProviderEvidence = providerContractMismatchEvidence with { ProviderContractIdentity = "CON-999" } }, "PROVIDER_CONTRACT_MISMATCH");
ExpectRejectedRegistration(failures, "provider-not-effective", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-not-effective", ProviderEvidence = BuildIdentifierEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime.AddHours(1), expiryTime.AddHours(1)) }, "PROVIDER_NOT_EFFECTIVE");
ExpectRejectedRegistration(failures, "provider-expired", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-expired", ObservationTime = expiryTime.AddHours(2), RegistrationEnvelope = providerBase.RegistrationEnvelope with { Expiry = expiryTime.AddDays(1) }, ProviderEvidence = BuildIdentifierEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime) }, "PROVIDER_EVIDENCE_EXPIRED");
ExpectRejectedRegistration(failures, "provider-rejected", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-rejected", ProviderEvidence = BuildIdentifierEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Operational, "REJECTED", effectiveTime, expiryTime) }, "PROVIDER_EVIDENCE_REJECTED");
ExpectRejectedRegistration(failures, "provider-candidate", providerCatalog, providerBase with { RegistrationId = "svc-reg-provider-candidate", ProviderEvidence = BuildIdentifierEvidence("foundation-identifier-provider", "1.0", EvidenceOrigin.Candidate, "ADMITTED", effectiveTime, expiryTime) }, "NON_OPERATIONAL_PROVIDER_EVIDENCE");

var syntheticProviderCatalog = new ServiceCatalog();
var syntheticProviderEvidence = (IdentifierProviderEvidence)BuildIdentifierEvidence("foundation-identifier-provider-synthetic", "1.0", EvidenceOrigin.Synthetic, "ADMITTED", effectiveTime, expiryTime);
syntheticProviderEvidence = syntheticProviderEvidence with { ProviderRecord = syntheticProviderEvidence.ProviderRecord with { ProviderClass = "synthetic-provider" } };
var syntheticProviderRequest = BuildRequest(
    "svc-reg-provider-synthetic",
    "foundation-identifier-provider-synthetic",
    "foundation-identifier-provider-synthetic",
    "1.0",
    "Falcon Foundation",
    ServiceKind.IdentifierProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-identifier-provider-synthetic",
        "1.0",
        "Falcon Foundation",
        "identifier provider",
        "bounded identifier provider",
        new[] { new ServiceResponsibilityDeclaration("identifier-responsibility-synthetic", "Falcon Foundation", "identifier responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-provider-synthetic",
        "foundation-identifier-provider-synthetic-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "provider-provenance-synthetic",
    "evidence/provider/synthetic",
    effectiveTime,
    syntheticProviderEvidence);
ExpectRejectedRegistration(failures, "provider-synthetic", syntheticProviderCatalog, syntheticProviderRequest, "NON_OPERATIONAL_PROVIDER_EVIDENCE");

var reusedRegistrationCatalog = new ServiceCatalog();
var firstSuccessRequest = BuildRequest(
    "svc-reg-seq-1",
    "foundation-sequence-one",
    "foundation-sequence-one",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-sequence-one",
        "1.0",
        "Falcon Foundation",
        "sequence one",
        "bounded sequence one",
        new[] { new ServiceResponsibilityDeclaration("sequence-one", "Falcon Foundation", "sequence one") },
        Array.Empty<ServiceContractRequirement>(),
        Array.Empty<ServiceConsumedContractRequirement>(),
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") },
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-sequence-one",
        "foundation-sequence-one-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "sequence-provenance-one",
    "evidence/sequence-one",
    effectiveTime);
Expect(failures, "sequence-first-register", Registered(reusedRegistrationCatalog.Register(firstSuccessRequest)));
var rejectedSequenceRequest = BuildRequest(
    "svc-reg-seq-2",
    "foundation-sequence-two",
    "foundation-sequence-two",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-sequence-two",
        "1.0",
        "Falcon Foundation",
        "sequence two",
        "bounded sequence two",
        new[] { new ServiceResponsibilityDeclaration("sequence-two", "Falcon Foundation", "sequence two") },
        Array.Empty<ServiceContractRequirement>(),
        Array.Empty<ServiceConsumedContractRequirement>(),
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        new[] { new ServiceDependencyDeclaration("foundation-contract-registry", new[] { "1.1" }, "governed-service", "self-description", "service registration control", "reject") },
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-sequence-two",
        "foundation-sequence-two-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "sequence-provenance-two",
    "evidence/sequence-two",
    effectiveTime with { });
ExpectRejectedRegistration(failures, "sequence-gap-rejection", reusedRegistrationCatalog, WithManifest(rejectedSequenceRequest, rejectedSequenceRequest.Manifest with { Purpose = "" }), "MISSING_PURPOSE");
ExpectRejectedRegistration(
    failures,
    "sequence-rejected-id-reserved",
    reusedRegistrationCatalog,
    rejectedSequenceRequest,
    "DUPLICATE_REGISTRATION_ID");
var correctedSequenceRequest = BuildRequest(
    "svc-reg-seq-3",
    "foundation-sequence-two",
    "foundation-sequence-two",
    "1.0",
    "Falcon Foundation",
    ServiceKind.GeneralFoundationService,
    ExplicitNoGrantIntent(),
    rejectedSequenceRequest.Manifest,
    "sequence-provenance-two-corrected",
    "evidence/sequence-two-corrected",
    effectiveTime);
var correctedSequenceResult = reusedRegistrationCatalog.Register(correctedSequenceRequest);
Expect(
    failures,
    "sequence-second-register",
    Registered(correctedSequenceResult),
    correctedSequenceResult.ReasonCode);
var reusedHistory = reusedRegistrationCatalog.HistoryFor("foundation-sequence-two");
Expect(
    failures,
    "no-sequence-gap-after-rejection",
    reusedHistory is not null &&
    reusedHistory.Entries.Count == 1 &&
    reusedHistory.Entries[0].Registration.RegistrationSequence == 2,
    reusedHistory is null
        ? $"missing history; registration result {correctedSequenceResult.ReasonCode}"
        : $"sequence {reusedHistory.Entries[0].Registration.RegistrationSequence}; registration result {correctedSequenceResult.ReasonCode}");

var secretProvider = BuildRequest(
    "svc-reg-secret",
    "foundation-secret-custody-provider",
    "foundation-secret-custody-provider",
    "1.0",
    "Falcon Foundation",
    ServiceKind.SecretCustodyProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-secret-custody-provider",
        "1.0",
        "Falcon Foundation",
        "secret custody provider",
        "bounded secret custody provider",
        new[] { new ServiceResponsibilityDeclaration("secret-responsibility", "Falcon Foundation", "secret responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-secret-custody",
        "foundation-secret-custody-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "secret-provenance",
    "evidence/secret",
    effectiveTime,
    BuildSecretCustodyEvidence("foundation-secret-custody-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime));
Expect(failures, "secret-provider-valid", Registered(providerCatalog.Register(secretProvider)));

var certificateProvider = BuildRequest(
    "svc-reg-certificate",
    "foundation-certificate-identity-provider",
    "foundation-certificate-identity-provider",
    "1.0",
    "Falcon Foundation",
    ServiceKind.CertificateIdentityProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-certificate-identity-provider",
        "1.0",
        "Falcon Foundation",
        "certificate identity provider",
        "bounded certificate identity provider",
        new[] { new ServiceResponsibilityDeclaration("certificate-responsibility", "Falcon Foundation", "certificate responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-certificate",
        "foundation-certificate-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "certificate-provenance",
    "evidence/certificate",
    effectiveTime,
    BuildCertificateEvidence("foundation-certificate-identity-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime));
Expect(failures, "certificate-provider-valid", Registered(providerCatalog.Register(certificateProvider)));

var randomnessProvider = BuildRequest(
    "svc-reg-randomness",
    "foundation-randomness-provider",
    "foundation-randomness-provider",
    "1.0",
    "Falcon Foundation",
    ServiceKind.RandomnessProvider,
    ExplicitNoGrantIntent(),
    BuildManifest(
        "foundation-randomness-provider",
        "1.0",
        "Falcon Foundation",
        "randomness provider",
        "bounded randomness provider",
        new[] { new ServiceResponsibilityDeclaration("randomness-responsibility", "Falcon Foundation", "randomness responsibility") },
        Array.Empty<ServiceContractRequirement>(),
        new[] { new ServiceConsumedContractRequirement("CON-023", new[] { "1.1" }, "Falcon Application Authority", "controlled service admission") },
        new[] { "foundation-runtime" },
        Array.Empty<string>(),
        new ServiceLifecycleDeclaration("prepared", new[] { "registered" }, "explicit governed change", "governed replacement required", "controlled migration", "governed removal"),
        Array.Empty<ServiceDependencyDeclaration>(),
        new ServiceOperationalBoundary("bounded resources", "bounded health reporting", "bounded recovery", "bounded failure containment", "admission only", "no automatic authority", "immutable evidence", "governed provenance", "digest-stable integrity", "no automatic activation"),
        new ServiceProtectionDeclaration(true, true, true, true, true, true),
        "manifest-randomness",
        "foundation-randomness-package",
        "1.0",
        "package-content",
        "policy",
        "self-dev",
        "guardian interface declared"),
    "randomness-provenance",
    "evidence/randomness",
    effectiveTime,
    BuildRandomnessEvidence("foundation-randomness-provider", "1.0", EvidenceOrigin.Operational, "ADMITTED", effectiveTime, expiryTime));
Expect(failures, "randomness-provider-valid", Registered(providerCatalog.Register(randomnessProvider)));

var appControl = new AdmissionControl(new InMemoryAdmissionBaselineProvider(BuildAdmissionBaseline()));
var appManifest = BuildApplicationManifest(
    "app-001",
    "foundation-admission-app",
    "1.0",
    "Falcon Application Authority",
    "application admission boundary",
    "app-package",
    "1.0",
    "package-content",
    "1.1",
    "1.1",
    "1.1",
    "approved provider boundary");
var appRequest = BuildAdmissionRequest(
    "admission-001",
    "APPLICATION",
    "foundation-admission-app",
    "1.0",
    "Falcon Application Authority",
    "CON-000 / CON-023",
    "1.1",
    "app-001",
    appManifest,
    "prov-001",
    "application-provenance",
    "bootstrap-001",
    "DEFINED",
    "approved provider boundary",
    "seed-001");
var appDecision = appControl.Evaluate(appRequest);
Expect(failures, "application-admission-pass", string.Equals(appDecision.Decision, "ADMITTED", StringComparison.Ordinal) && string.Equals(appDecision.ReasonCode, "admission accepted", StringComparison.Ordinal), appDecision.ReasonCode);
Expect(failures, "application-admission-fails-on-invalid-bootstrap", string.Equals(appControl.Evaluate(appRequest with { BootstrapContextState = "MISSING" }).Decision, "REJECTED", StringComparison.Ordinal));
Expect(failures, "application-admission-fails-on-provider-bypass", string.Equals(appControl.Evaluate(appRequest with { ProviderBoundary = "unapproved bypass" }).Decision, "REJECTED", StringComparison.Ordinal));
Expect(failures, "application-admission-fails-on-required-contract", string.Equals(appControl.Evaluate(appRequest with { ContractVersion = "9.9" }).Decision, "REJECTED", StringComparison.Ordinal));
Expect(failures, "application-admission-fails-on-provenance-mismatch", string.Equals(appControl.Evaluate(appRequest with { ProvenanceContent = "tampered" }).Decision, "REJECTED", StringComparison.Ordinal));


var nullCatalog = new ServiceCatalog();
Expect(failures, "null-registration-fail-closed", !nullCatalog.Register(null).Success, "null registration unexpectedly succeeded");
Expect(failures, "null-catalog-lookup", nullCatalog.Lookup(null, "1.0") is null, "null catalog lookup returned an entry");

var firstObservationCatalog = new ServiceCatalog();
var rejectedFirstObservation = WithManifest(
    generalRequest with { RegistrationId = "svc-reg-first-observation" },
    generalRequest.Manifest with { Purpose = string.Empty });
ExpectRejectedRegistration(failures, "registration-first-observation-rejected", firstObservationCatalog, rejectedFirstObservation, "MISSING_PURPOSE");
ExpectRejectedRegistration(
    failures,
    "registration-first-observation-reserved",
    firstObservationCatalog,
    generalRequest with { RegistrationId = "svc-reg-first-observation" },
    "DUPLICATE_REGISTRATION_ID");

var exactPurposeCatalog = new ServiceCatalog();
ExpectRejectedRegistration(
    failures,
    "registration-envelope-purpose-exact",
    exactPurposeCatalog,
    generalRequest with
    {
        RegistrationId = "svc-reg-purpose-exact",
        RegistrationEnvelope = generalRequest.RegistrationEnvelope with
        {
            MessageId = "fil-purpose-exact",
            DeliveryAttemptId = "delivery-purpose-exact",
            Purpose = "prefix governed service registration suffix"
        }
    },
    "INVALID_REGISTRATION_ENVELOPE_PURPOSE");

var concurrentCatalog = new ServiceCatalog();
var concurrentResults = new ConcurrentBag<ServiceRegistrationResult>();
var concurrentRequest = generalRequest with { RegistrationId = "svc-reg-concurrent-first-observation" };
Parallel.For(0, 32, _ => concurrentResults.Add(concurrentCatalog.Register(concurrentRequest)));
Expect(
    failures,
    "concurrent-registration-single-accept",
    concurrentResults.Count(result => result.Success) == 1 && concurrentCatalog.Entries.Count == 1,
    "concurrent duplicate registration produced more than one accepted state change");

if (failures.Count > 0)
{
    Console.Error.WriteLine("Stage 3 WP-03: FAIL");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }

    return 1;
}

Console.WriteLine("Stage 3 WP-03: PASS");
Console.WriteLine($"Golden manifest digest: {goldenDigest}");
Console.WriteLine($"Golden manifest byte length: {Encoding.UTF8.GetByteCount(goldenText)}");
Console.WriteLine("Service catalog registration, typed lookup, lineage, provider evidence, and application admission checks validated.");
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
