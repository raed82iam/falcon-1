using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.ContractRegistry;
using Foundation.Contracts;

namespace Foundation.ServiceCatalog;

public enum ServiceKind
{
    GeneralFoundationService,
    IdentifierProvider,
    CryptographicProvider,
    SecretCustodyProvider,
    CertificateIdentityProvider,
    RandomnessProvider
}

public enum ServiceRegistrationDecision { Registered, Rejected }
public enum ServiceRegistrationState { Registered, Rejected }
public enum ServiceOperationalState { NotActive, Rejected }
public enum RegistrationMode { Explicit, Automatic }
public enum EvidenceOrigin { Operational, Candidate, Synthetic }

public sealed record ServiceCatalogKey
{
    public string ServiceIdentity { get; init; } = string.Empty;
    public string ServiceVersion { get; init; } = string.Empty;
    public string CanonicalText => $"{ServiceIdentity.Length}:{ServiceIdentity}|{ServiceVersion.Length}:{ServiceVersion}";

    public static ServiceCatalogKey From(string serviceIdentity, string serviceVersion)
        => new() { ServiceIdentity = serviceIdentity ?? string.Empty, ServiceVersion = serviceVersion ?? string.Empty };

    public override string ToString() => CanonicalText;
}

public sealed record ServiceContractRequirement(string ContractIdentity, string Version, string Purpose, string Relation);
public sealed record ServiceConsumedContractRequirement(string ContractIdentity, IReadOnlyList<string> CompatibleVersions, string Purpose, string Relation);
public sealed record ServiceDependencyDeclaration(string Identity, IReadOnlyList<string> CompatibleVersions, string Kind, string Relation, string Purpose, string DegradedBehavior, string LifecycleOrder = "");
public sealed record ServiceResponsibilityDeclaration(string Identity, string Owner, string Responsibility);
public sealed record ServiceLifecycleDeclaration(string DeclaredSourceState, IReadOnlyList<string> SupportedTransitions, string UpdateBehavior, string ReplacementBehavior, string MigrationBehavior, string RemovalBehavior);
public sealed record ServiceOperationalBoundary(string ResourceRequirements, string HealthReportingRequirements, string RecoveryRequirements, string FailureContainmentRequirements, string Permissions, string AuthorityLimits, string EvidenceRequirements, string ProvenanceRequirements, string IntegrityRequirements, string AutomaticActivationProhibition);
public sealed record ServiceProtectionDeclaration(bool NoAutomaticRegistration, bool NoAutomaticActivation, bool NoAuthorityGain, bool NoPermissionGain, bool NoTrustGain, bool NoResponsibilityGain);
public sealed record ServiceRegistrationIntent(RegistrationMode Mode, bool ActivationRequested, bool AdmissionRequested, bool AuthorityRequested, bool PermissionRequested, bool TrustRequested, bool ResponsibilityGainRequested);

public sealed record ServiceManifest
{
    public string ServiceIdentity { get; init; }
    public string ServiceVersion { get; init; }
    public string AccountableOwner { get; init; }
    public string Purpose { get; init; }
    public string ExclusiveResponsibilityBoundary { get; init; }
    public ReadOnlyCollection<ServiceResponsibilityDeclaration> OwnedResponsibilities { get; init; }
    public ReadOnlyCollection<ServiceContractRequirement> ProvidedContracts { get; init; }
    public ReadOnlyCollection<ServiceConsumedContractRequirement> ConsumedContracts { get; init; }
    public ReadOnlyCollection<string> AuthorizedConsumers { get; init; }
    public ReadOnlyCollection<string> RestrictedConsumers { get; init; }
    public ServiceLifecycleDeclaration Lifecycle { get; init; }
    public ReadOnlyCollection<ServiceDependencyDeclaration> Dependencies { get; init; }
    public ServiceOperationalBoundary OperationalBoundary { get; init; }
    public ServiceProtectionDeclaration Protection { get; init; }
    public string ManifestId { get; init; }
    public string PackageIdentity { get; init; }
    public string PackageVersion { get; init; }
    public string PackageContentOrIntegrityInput { get; init; }
    public string CsaEligibilityPolicy { get; init; }
    public string SelfDevelopmentOriginAndEscalationPath { get; init; }
    public string GuardianAndProtectionInterface { get; init; }

    public ServiceManifest(
        string serviceIdentity,
        string serviceVersion,
        string accountableOwner,
        string purpose,
        string exclusiveResponsibilityBoundary,
        IEnumerable<ServiceResponsibilityDeclaration> ownedResponsibilities,
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
        string packageContentOrIntegrityInput,
        string csaEligibilityPolicy,
        string selfDevelopmentOriginAndEscalationPath,
        string guardianAndProtectionInterface)
    {
        ServiceIdentity = serviceIdentity ?? string.Empty;
        ServiceVersion = serviceVersion ?? string.Empty;
        AccountableOwner = accountableOwner ?? string.Empty;
        Purpose = purpose ?? string.Empty;
        ExclusiveResponsibilityBoundary = exclusiveResponsibilityBoundary ?? string.Empty;
        OwnedResponsibilities = Freeze((ownedResponsibilities ?? Array.Empty<ServiceResponsibilityDeclaration>())
            .Select(CloneResponsibility));
        ProvidedContracts = Freeze((providedContracts ?? Array.Empty<ServiceContractRequirement>())
            .Select(CloneProvidedContract));
        ConsumedContracts = Freeze((consumedContracts ?? Array.Empty<ServiceConsumedContractRequirement>())
            .Select(CloneConsumedContract));
        AuthorizedConsumers = FreezeStrings(authorizedConsumers);
        RestrictedConsumers = FreezeStrings(restrictedConsumers);
        Lifecycle = lifecycle is null
            ? new ServiceLifecycleDeclaration(string.Empty, FreezeStrings(null), string.Empty, string.Empty, string.Empty, string.Empty)
            : lifecycle with { SupportedTransitions = FreezeStrings(lifecycle.SupportedTransitions) };
        Dependencies = Freeze((dependencies ?? Array.Empty<ServiceDependencyDeclaration>())
            .Select(CloneDependency));
        OperationalBoundary = operationalBoundary ?? new ServiceOperationalBoundary(
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        Protection = protection ?? new ServiceProtectionDeclaration(false, false, false, false, false, false);
        ManifestId = manifestId ?? string.Empty;
        PackageIdentity = packageIdentity ?? string.Empty;
        PackageVersion = packageVersion ?? string.Empty;
        PackageContentOrIntegrityInput = packageContentOrIntegrityInput ?? string.Empty;
        CsaEligibilityPolicy = csaEligibilityPolicy ?? string.Empty;
        SelfDevelopmentOriginAndEscalationPath = selfDevelopmentOriginAndEscalationPath ?? string.Empty;
        GuardianAndProtectionInterface = guardianAndProtectionInterface ?? string.Empty;
    }

    public string CanonicalText() => ServiceManifestCanonicalizer.Serialize(this);
    public string ComputeDigest() => ServiceManifestCanonicalizer.ComputeDigest(this);

    private static ServiceResponsibilityDeclaration CloneResponsibility(ServiceResponsibilityDeclaration? value)
        => value is null ? new(string.Empty, string.Empty, string.Empty) : value with { };

    private static ServiceContractRequirement CloneProvidedContract(ServiceContractRequirement? value)
        => value is null ? new(string.Empty, string.Empty, string.Empty, string.Empty) : value with { };

    private static ServiceConsumedContractRequirement CloneConsumedContract(ServiceConsumedContractRequirement? value)
        => value is null
            ? new(string.Empty, FreezeStrings(null), string.Empty, string.Empty)
            : value with { CompatibleVersions = FreezeStrings(value.CompatibleVersions) };

    private static ServiceDependencyDeclaration CloneDependency(ServiceDependencyDeclaration? value)
        => value is null
            ? new(string.Empty, FreezeStrings(null), string.Empty, string.Empty, string.Empty, string.Empty)
            : value with { CompatibleVersions = FreezeStrings(value.CompatibleVersions) };

    private static ReadOnlyCollection<T> Freeze<T>(IEnumerable<T>? values)
        => new((values ?? Array.Empty<T>()).ToArray());

    private static ReadOnlyCollection<string> FreezeStrings(IEnumerable<string>? values)
        => new((values ?? Array.Empty<string>()).Select(value => value ?? string.Empty).ToArray());
}

public abstract record ServiceProviderEvidence
{
    public ServiceKind ServiceKind { get; init; }
    public string ServiceIdentity { get; init; } = string.Empty;
    public string ServiceVersion { get; init; } = string.Empty;
    public string ProviderContractIdentity { get; init; } = string.Empty;
    public string ProviderContractVersion { get; init; } = string.Empty;
    public EvidenceOrigin EvidenceOrigin { get; init; }
    public string OperationalStatus { get; init; } = string.Empty;
    public DateTimeOffset EffectiveTime { get; init; }
    public DateTimeOffset Expiry { get; init; }
}

public sealed record IdentifierProviderEvidence : ServiceProviderEvidence
{
    public IdentifierProviderRecord ProviderRecord { get; init; } = new("", "", "", "", "", "", "", "", "", default, default);
}

public sealed record CryptographicProviderEvidence : ServiceProviderEvidence
{
    public CryptographicProviderRecord ProviderRecord { get; init; } = new("", "", "", "", "", "", "", "", default, default);
}

public sealed record SecretCustodyProviderEvidence : ServiceProviderEvidence
{
    public SecretCustodyRecord ProviderRecord { get; init; } = new("", "", "", "", "", "", "", "", default, default);
}

public sealed record CertificateIdentityProviderEvidence : ServiceProviderEvidence
{
    public CertificateIdentityProviderRecord ProviderRecord { get; init; } = new("", "", "", "", "", "", "", default, default);
}

public sealed record RandomnessProviderEvidence : ServiceProviderEvidence
{
    public RandomnessProviderRecord ProviderRecord { get; init; } = new("", "", "", "", "", "", "", default, default);
}

public sealed record ServiceRegistrationRequest
{
    public string RegistrationId { get; init; } = string.Empty;
    public string ServiceIdentity { get; init; } = string.Empty;
    public string ServiceVersion { get; init; } = string.Empty;
    public string RequesterIdentity { get; init; } = string.Empty;
    public string AccountableOwner { get; init; } = string.Empty;
    public ServiceKind ServiceKind { get; init; }
    public ServiceRegistrationIntent Intent { get; init; } = new(RegistrationMode.Explicit, false, false, false, false, false, false);
    public ServiceManifest Manifest { get; init; } = null!;
    public FilEnvelope RegistrationEnvelope { get; init; } = null!;
    public string ManifestDigest { get; init; } = string.Empty;
    public string ProvenanceContent { get; init; } = string.Empty;
    public string ProvenanceDigest { get; init; } = string.Empty;
    public string RegistrationEvidenceReference { get; init; } = string.Empty;
    public DateTimeOffset ObservationTime { get; init; }
    public ServiceProviderEvidence? ProviderEvidence { get; init; }
}

public sealed record ServiceRegistrationRecord
{
    public string RegistrationId { get; init; } = string.Empty;
    public ServiceCatalogKey CatalogKey { get; init; } = new();
    public ServiceRegistrationDecision Decision { get; init; }
    public string ReasonCode { get; init; } = string.Empty;
    public string AccountableOwner { get; init; } = string.Empty;
    public string ManifestDigest { get; init; } = string.Empty;
    public string ProvenanceDigest { get; init; } = string.Empty;
    public string RegistrationEvidenceReference { get; init; } = string.Empty;
    public ServiceRegistrationState RegistrationState { get; init; }
    public ServiceOperationalState OperationalState { get; init; }
    public bool AuthorityGranted { get; init; }
    public bool PermissionGranted { get; init; }
    public bool TrustGranted { get; init; }
    public bool AdmissionGranted { get; init; }
    public int RegistrationSequence { get; init; }

    public string ServiceIdentity => CatalogKey.ServiceIdentity;
    public string ServiceVersion => CatalogKey.ServiceVersion;
    public bool Success => Decision == ServiceRegistrationDecision.Registered;
}

public sealed record ServiceCatalogEntry
{
    public ServiceCatalogKey Key { get; init; } = new();
    public ServiceRegistrationRecord Registration { get; init; } = null!;
    public ServiceManifest Manifest { get; init; } = null!;
}

public sealed record ServiceCatalogLookup
{
    public ServiceCatalogKey Key { get; init; } = new();
    public ServiceCatalogEntry Entry { get; init; } = null!;
}

public sealed record ServiceCatalogResponsibilityLookup
{
    public string ResponsibilityIdentity { get; init; } = string.Empty;
    public string ResponsibilityMeaning { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public ServiceCatalogEntry Entry { get; init; } = null!;
}

public sealed record ServiceCatalogHistory
{
    public string ServiceIdentity { get; init; } = string.Empty;
    public ReadOnlyCollection<ServiceCatalogEntry> Entries { get; init; } = new(Array.Empty<ServiceCatalogEntry>());
}

public sealed record ServiceRegistrationValidationResult
{
    public bool Success { get; init; }
    public string ReasonCode { get; init; } = string.Empty;
    public static ServiceRegistrationValidationResult Pass() => new() { Success = true, ReasonCode = "REGISTERED" };
    public static ServiceRegistrationValidationResult Fail(string reason) => new() { Success = false, ReasonCode = reason };
}

public sealed record ServiceRegistrationResult
{
    public bool Success { get; init; }
    public string ReasonCode { get; init; } = string.Empty;
    public ServiceRegistrationRecord? Record { get; init; }
    public ServiceRegistrationRequest? Request { get; init; }
    public ServiceRegistrationDecision Decision => Record?.Decision ?? ServiceRegistrationDecision.Rejected;
    public ServiceRegistrationState RegistrationState => Record?.RegistrationState ?? ServiceRegistrationState.Rejected;
    public ServiceOperationalState OperationalState => Record?.OperationalState ?? ServiceOperationalState.Rejected;
    public bool AuthorityGranted => Record?.AuthorityGranted ?? false;
    public bool PermissionGranted => Record?.PermissionGranted ?? false;
    public bool TrustGranted => Record?.TrustGranted ?? false;
    public bool AdmissionGranted => Record?.AdmissionGranted ?? false;

    public static ServiceRegistrationResult Passed(ServiceRegistrationRequest request, ServiceRegistrationRecord record)
        => new() { Success = true, ReasonCode = "REGISTERED", Request = request, Record = record };

    public static ServiceRegistrationResult Failed(string reasonCode, ServiceRegistrationRequest? request)
        => new() { Success = false, ReasonCode = reasonCode, Request = request };
}

public sealed record ServiceResponsibilityOwnership
{
    public string ResponsibilityIdentity { get; init; } = string.Empty;
    public string ResponsibilityMeaning { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public ServiceCatalogKey Key { get; init; } = new();
    public int RegistrationSequence { get; init; }
}

public sealed class ServiceCatalog
{
    private readonly object _sync = new();
    private readonly Dictionary<ServiceCatalogKey, ServiceCatalogEntry> _entries = new();
    private readonly Dictionary<string, List<ServiceCatalogEntry>> _history = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ServiceResponsibilityOwnership> _responsibilitiesByPair = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ServiceResponsibilityOwnership> _responsibilitiesByIdentity = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ServiceResponsibilityOwnership> _responsibilitiesByMeaning = new(StringComparer.Ordinal);
    private readonly HashSet<string> _registrationIds = new(StringComparer.Ordinal);
    private int _sequence;

    public bool IsEmpty
    {
        get { lock (_sync) { return _entries.Count == 0; } }
    }

    public int ResponsibilityCount
    {
        get { lock (_sync) { return _responsibilitiesByPair.Count; } }
    }

    public IReadOnlyCollection<ServiceCatalogEntry> Entries
    {
        get
        {
            lock (_sync)
            {
                return new ReadOnlyCollection<ServiceCatalogEntry>(
                    _entries.Values
                        .OrderBy(entry => entry.Key.ServiceIdentity, StringComparer.Ordinal)
                        .ThenBy(entry => entry.Key.ServiceVersion, StringComparer.Ordinal)
                        .ThenBy(entry => entry.Registration.RegistrationSequence)
                        .ToArray());
            }
        }
    }

    public ServiceRegistrationResult Register(ServiceRegistrationRequest? request)
    {
        if (request is null)
        {
            return ServiceRegistrationResult.Failed("INVALID_REGISTRATION_ENVELOPE", null);
        }

        lock (_sync)
        {
            if (string.IsNullOrWhiteSpace(request.RegistrationId))
            {
                return ServiceRegistrationResult.Failed("MISSING_REGISTRATION_ID", request);
            }

            if (!_registrationIds.Add(request.RegistrationId))
            {
                return ServiceRegistrationResult.Failed("DUPLICATE_REGISTRATION_ID", request);
            }

            var validation = ValidateCore(request, registrationIdReserved: true);
            if (!validation.Success)
            {
                return ServiceRegistrationResult.Failed(validation.ReasonCode, request);
            }

            var manifestSnapshot = CreateManifestSnapshot(request.Manifest);
            var key = ServiceCatalogKey.From(request.Manifest.ServiceIdentity, request.Manifest.ServiceVersion);
            var record = new ServiceRegistrationRecord
            {
                RegistrationId = request.RegistrationId,
                CatalogKey = key,
                Decision = ServiceRegistrationDecision.Registered,
                ReasonCode = "REGISTERED",
                AccountableOwner = request.AccountableOwner,
                ManifestDigest = manifestSnapshot.ComputeDigest(),
                ProvenanceDigest = request.ProvenanceDigest,
                RegistrationEvidenceReference = request.RegistrationEvidenceReference,
                RegistrationState = ServiceRegistrationState.Registered,
                OperationalState = ServiceOperationalState.NotActive,
                AuthorityGranted = false,
                PermissionGranted = false,
                TrustGranted = false,
                AdmissionGranted = false,
                RegistrationSequence = ++_sequence
            };

            var entry = new ServiceCatalogEntry { Key = key, Registration = record, Manifest = manifestSnapshot };
            _entries[key] = entry;
            if (!_history.TryGetValue(key.ServiceIdentity, out var history))
            {
                history = new List<ServiceCatalogEntry>();
                _history.Add(key.ServiceIdentity, history);
            }
            history.Add(entry);

            foreach (var responsibility in request.Manifest.OwnedResponsibilities)
            {
                RecordResponsibilityOwnership(responsibility.Identity, responsibility.Responsibility, responsibility.Owner, key, record.RegistrationSequence);
            }

            return ServiceRegistrationResult.Passed(request, record);
        }
    }

    public ServiceCatalogLookup? Lookup(string? serviceIdentity, string? serviceVersion)
        => string.IsNullOrWhiteSpace(serviceIdentity) || string.IsNullOrWhiteSpace(serviceVersion)
            ? null
            : Lookup(ServiceCatalogKey.From(serviceIdentity, serviceVersion));

    public ServiceCatalogLookup? Lookup(ServiceCatalogKey? key)
    {
        if (key is null || string.IsNullOrWhiteSpace(key.ServiceIdentity) || string.IsNullOrWhiteSpace(key.ServiceVersion)) return null;
        lock (_sync)
        {
            return _entries.TryGetValue(key, out var entry) ? new ServiceCatalogLookup { Key = key, Entry = entry } : null;
        }
    }

    public ServiceCatalogResponsibilityLookup? LookupResponsibilityOwner(string? responsibilityIdentity, string? responsibilityMeaning)
    {
        if (string.IsNullOrWhiteSpace(responsibilityIdentity) || string.IsNullOrWhiteSpace(responsibilityMeaning)) return null;
        lock (_sync)
        {
            var key = ResponsibilityKey(responsibilityIdentity, responsibilityMeaning);
            return _responsibilitiesByPair.TryGetValue(key, out var ownership) && _entries.TryGetValue(ownership.Key, out var entry)
                ? new ServiceCatalogResponsibilityLookup
                {
                    ResponsibilityIdentity = responsibilityIdentity,
                    ResponsibilityMeaning = responsibilityMeaning,
                    Owner = ownership.Owner,
                    Entry = entry
                }
                : null;
        }
    }

    public IReadOnlyList<string> VersionsFor(string? serviceIdentity)
    {
        if (string.IsNullOrWhiteSpace(serviceIdentity)) return Array.Empty<string>();
        lock (_sync)
        {
            return _history.TryGetValue(serviceIdentity, out var history)
                ? history.Select(item => item.Key.ServiceVersion).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray()
                : Array.Empty<string>();
        }
    }

    public ServiceCatalogHistory? HistoryFor(string? serviceIdentity)
    {
        if (string.IsNullOrWhiteSpace(serviceIdentity)) return null;
        lock (_sync)
        {
            return _history.TryGetValue(serviceIdentity, out var history)
                ? new ServiceCatalogHistory
                {
                    ServiceIdentity = serviceIdentity,
                    Entries = new ReadOnlyCollection<ServiceCatalogEntry>(history.OrderBy(item => item.Registration.RegistrationSequence).ToArray())
                }
                : null;
        }
    }

    public bool IsAuthorizedConsumer(string? serviceIdentity, string? serviceVersion, string? consumer)
    {
        if (string.IsNullOrWhiteSpace(consumer)) return false;
        var lookup = Lookup(serviceIdentity, serviceVersion);
        return lookup is not null &&
               lookup.Entry.Manifest.AuthorizedConsumers.Contains(consumer, StringComparer.Ordinal) &&
               !lookup.Entry.Manifest.RestrictedConsumers.Contains(consumer, StringComparer.Ordinal);
    }

    public ServiceRegistrationValidationResult Validate(ServiceRegistrationRequest? request)
    {
        lock (_sync)
        {
            return ValidateCore(request, registrationIdReserved: false);
        }
    }

    private ServiceRegistrationValidationResult ValidateCore(ServiceRegistrationRequest? request, bool registrationIdReserved)
    {
        if (request is null) return ServiceRegistrationValidationResult.Fail("INVALID_REGISTRATION_ENVELOPE");
        if (string.IsNullOrWhiteSpace(request.RegistrationId)) return ServiceRegistrationValidationResult.Fail("MISSING_REGISTRATION_ID");
        if (string.IsNullOrWhiteSpace(request.RequesterIdentity)) return ServiceRegistrationValidationResult.Fail("MISSING_REQUESTER");
        if (string.IsNullOrWhiteSpace(request.AccountableOwner)) return ServiceRegistrationValidationResult.Fail("MISSING_OWNER");
        if (request.Intent is null) return ServiceRegistrationValidationResult.Fail("INVALID_REGISTRATION_INTENT");
        if (request.RegistrationEnvelope is null) return ServiceRegistrationValidationResult.Fail("INVALID_REGISTRATION_ENVELOPE");
        if (request.Manifest is null) return ServiceRegistrationValidationResult.Fail("INVALID_MANIFEST");
        if (string.IsNullOrWhiteSpace(request.ManifestDigest)) return ServiceRegistrationValidationResult.Fail("MISSING_MANIFEST_DIGEST");
        if (string.IsNullOrWhiteSpace(request.ProvenanceDigest)) return ServiceRegistrationValidationResult.Fail("MISSING_PROVENANCE_DIGEST");
        if (string.IsNullOrWhiteSpace(request.ProvenanceContent)) return ServiceRegistrationValidationResult.Fail("MISSING_PROVENANCE_CONTENT");
        if (string.IsNullOrWhiteSpace(request.RegistrationEvidenceReference)) return ServiceRegistrationValidationResult.Fail("MISSING_REGISTRATION_EVIDENCE");
        if (request.Intent.Mode != RegistrationMode.Explicit) return ServiceRegistrationValidationResult.Fail("AUTOMATIC_REGISTRATION_PROHIBITED");
        if (request.Intent.ActivationRequested) return ServiceRegistrationValidationResult.Fail("AUTOMATIC_ACTIVATION_PROHIBITED");
        if (request.Intent.AdmissionRequested) return ServiceRegistrationValidationResult.Fail("ADMISSION_GRANT_PROHIBITED");
        if (request.Intent.AuthorityRequested) return ServiceRegistrationValidationResult.Fail("AUTHORITY_GRANT_PROHIBITED");
        if (request.Intent.PermissionRequested) return ServiceRegistrationValidationResult.Fail("PERMISSION_GRANT_PROHIBITED");
        if (request.Intent.TrustRequested) return ServiceRegistrationValidationResult.Fail("TRUST_GRANT_PROHIBITED");
        if (request.Intent.ResponsibilityGainRequested) return ServiceRegistrationValidationResult.Fail("RESPONSIBILITY_GAIN_PROHIBITED");
        if (!string.Equals(request.RequesterIdentity, request.RegistrationEnvelope.ProducerIdentity, StringComparison.Ordinal)) return ServiceRegistrationValidationResult.Fail("PRODUCER_REQUESTER_MISMATCH");
        if (request.ObservationTime == default) return ServiceRegistrationValidationResult.Fail("INVALID_REGISTRATION_ENVELOPE");

        var manifestReason = ValidateManifest(request.Manifest);
        if (manifestReason is not null) return ServiceRegistrationValidationResult.Fail(manifestReason);
        if (!string.Equals(request.AccountableOwner, request.Manifest.AccountableOwner, StringComparison.Ordinal)) return ServiceRegistrationValidationResult.Fail("OWNER_MISMATCH");
        if (!string.Equals(request.ServiceIdentity, request.Manifest.ServiceIdentity, StringComparison.Ordinal)) return ServiceRegistrationValidationResult.Fail("IDENTITY_MISMATCH");
        if (!string.Equals(request.ServiceVersion, request.Manifest.ServiceVersion, StringComparison.Ordinal)) return ServiceRegistrationValidationResult.Fail("VERSION_MISMATCH");
        if (!string.Equals(request.ManifestDigest, request.Manifest.ComputeDigest(), StringComparison.OrdinalIgnoreCase)) return ServiceRegistrationValidationResult.Fail("MANIFEST_DIGEST_MISMATCH");
        if (!string.Equals(ComputeSha256(request.ProvenanceContent), request.ProvenanceDigest, StringComparison.OrdinalIgnoreCase)) return ServiceRegistrationValidationResult.Fail("PROVENANCE_DIGEST_MISMATCH");
        if (!ValidateEnvelope(request, out var envelopeReason)) return ServiceRegistrationValidationResult.Fail(envelopeReason);
        var contractReason = ValidateContracts(request.Manifest);
        if (contractReason is not null) return ServiceRegistrationValidationResult.Fail(contractReason);
        if (!ValidateLineage(request, ServiceCatalogKey.From(request.Manifest.ServiceIdentity, request.Manifest.ServiceVersion), out var lineageReason)) return ServiceRegistrationValidationResult.Fail(lineageReason);
        if (!ValidateProviderEvidence(request, out var providerReason)) return ServiceRegistrationValidationResult.Fail(providerReason);

        var catalogKey = ServiceCatalogKey.From(request.Manifest.ServiceIdentity, request.Manifest.ServiceVersion);
        if (!registrationIdReserved && _registrationIds.Contains(request.RegistrationId)) return ServiceRegistrationValidationResult.Fail("DUPLICATE_REGISTRATION_ID");
        if (_entries.ContainsKey(catalogKey)) return ServiceRegistrationValidationResult.Fail("DUPLICATE_SERVICE_VERSION");
        return ServiceRegistrationValidationResult.Pass();
    }

    private bool ValidateLineage(ServiceRegistrationRequest request, ServiceCatalogKey key, out string reason)
    {
        foreach (var responsibility in request.Manifest.OwnedResponsibilities)
        {
            if (TryDetectResponsibilityCollision(_responsibilitiesByIdentity, ResponsibilityIdentityKey(responsibility.Identity), key.ServiceIdentity, out reason) ||
                TryDetectResponsibilityCollision(_responsibilitiesByMeaning, ResponsibilityMeaningKey(responsibility.Responsibility), key.ServiceIdentity, out reason) ||
                TryDetectResponsibilityCollision(_responsibilitiesByPair, ResponsibilityKey(responsibility.Identity, responsibility.Responsibility), key.ServiceIdentity, out reason))
            {
                return false;
            }
        }

        if (_history.TryGetValue(key.ServiceIdentity, out var history) && history.Count > 0)
        {
            var previous = history[^1].Manifest;
            if (!string.Equals(previous.AccountableOwner, request.Manifest.AccountableOwner, StringComparison.Ordinal))
            {
                reason = "OWNER_LINEAGE_MISMATCH";
                return false;
            }

            if (!ResponsibilityLineageMatches(previous.OwnedResponsibilities, request.Manifest.OwnedResponsibilities))
            {
                reason = "RESPONSIBILITY_LINEAGE_MISMATCH";
                return false;
            }
        }

        reason = string.Empty;
        return true;
    }

    private static string? ValidateManifest(ServiceManifest manifest)
    {
        if (string.IsNullOrWhiteSpace(manifest.ServiceIdentity)) return "MISSING_IDENTITY";
        if (string.IsNullOrWhiteSpace(manifest.ServiceVersion)) return "MISSING_VERSION";
        if (string.IsNullOrWhiteSpace(manifest.AccountableOwner)) return "MISSING_OWNER";
        if (string.IsNullOrWhiteSpace(manifest.Purpose)) return "MISSING_PURPOSE";
        if (string.IsNullOrWhiteSpace(manifest.ExclusiveResponsibilityBoundary)) return "MISSING_RESPONSIBILITY_BOUNDARY";
        if (manifest.OwnedResponsibilities is null || manifest.OwnedResponsibilities.Count == 0) return "MISSING_RESPONSIBILITY";
        if (manifest.ProvidedContracts is null) return "INVALID_MANIFEST";
        if (manifest.ConsumedContracts is null) return "INVALID_MANIFEST";
        if (manifest.AuthorizedConsumers is null || manifest.RestrictedConsumers is null) return "INVALID_MANIFEST";
        if (manifest.Lifecycle is null) return "INVALID_LIFECYCLE_DECLARATION";
        if (manifest.Dependencies is null) return "INVALID_DEPENDENCY";
        if (manifest.OperationalBoundary is null) return "INVALID_OPERATIONAL_BOUNDARY";
        if (manifest.Protection is null) return "INVALID_PROTECTION_DECLARATION";
        if (string.IsNullOrWhiteSpace(manifest.ManifestId)) return "INVALID_MANIFEST";
        if (string.IsNullOrWhiteSpace(manifest.PackageIdentity)) return "INVALID_MANIFEST";
        if (string.IsNullOrWhiteSpace(manifest.PackageVersion)) return "INVALID_MANIFEST";
        if (string.IsNullOrWhiteSpace(manifest.PackageContentOrIntegrityInput)) return "INVALID_MANIFEST";
        if (string.IsNullOrWhiteSpace(manifest.CsaEligibilityPolicy)) return "INVALID_MANIFEST";
        if (string.IsNullOrWhiteSpace(manifest.SelfDevelopmentOriginAndEscalationPath)) return "INVALID_MANIFEST";
        if (string.IsNullOrWhiteSpace(manifest.GuardianAndProtectionInterface)) return "INVALID_MANIFEST";

        var responsibilityIds = new HashSet<string>(StringComparer.Ordinal);
        var responsibilityMeanings = new HashSet<string>(StringComparer.Ordinal);
        var responsibilityPairs = new HashSet<string>(StringComparer.Ordinal);
        foreach (var responsibility in manifest.OwnedResponsibilities)
        {
            if (responsibility is null || string.IsNullOrWhiteSpace(responsibility.Identity) || string.IsNullOrWhiteSpace(responsibility.Responsibility))
            {
                return "MISSING_RESPONSIBILITY";
            }

            if (!string.Equals(responsibility.Owner, manifest.AccountableOwner, StringComparison.Ordinal))
            {
                return "RESPONSIBILITY_OWNER_MISMATCH";
            }

            if (!responsibilityPairs.Add(ResponsibilityKey(responsibility.Identity, responsibility.Responsibility)))
            {
                return "DUPLICATE_RESPONSIBILITY";
            }

            if (!responsibilityIds.Add(responsibility.Identity))
            {
                return "DUPLICATE_RESPONSIBILITY_IDENTITY";
            }

            if (!responsibilityMeanings.Add(responsibility.Responsibility))
            {
                return "DUPLICATE_RESPONSIBILITY_MEANING";
            }
        }

        if (manifest.ProvidedContracts.Any(value => value is null))
        {
            return "MALFORMED_PROVIDED_CONTRACT";
        }

        if (manifest.ConsumedContracts.Any(value => value is null))
        {
            return "MALFORMED_CONSUMED_CONTRACT";
        }

        if (manifest.Dependencies.Any(value => value is null))
        {
            return "MALFORMED_DEPENDENCY";
        }

        if (manifest.ConsumedContracts.Any(value => value.CompatibleVersions is null))
        {
            return "MALFORMED_CONSUMED_CONTRACT";
        }

        if (manifest.Dependencies.Any(value => value.CompatibleVersions is null))
        {
            return "MALFORMED_DEPENDENCY";
        }

        if (manifest.AuthorizedConsumers.Any(string.IsNullOrWhiteSpace) || manifest.RestrictedConsumers.Any(string.IsNullOrWhiteSpace))
        {
            return "INVALID_MANIFEST";
        }

        if (manifest.AuthorizedConsumers.GroupBy(value => value, StringComparer.Ordinal).Any(group => group.Count() > 1))
        {
            return "DUPLICATE_AUTHORIZED_CONSUMER";
        }

        if (manifest.RestrictedConsumers.GroupBy(value => value, StringComparer.Ordinal).Any(group => group.Count() > 1))
        {
            return "DUPLICATE_RESTRICTED_CONSUMER";
        }

        if (manifest.ProvidedContracts.GroupBy(value => $"{value.ContractIdentity}|{value.Version}", StringComparer.Ordinal).Any(group => group.Count() > 1))
        {
            return "DUPLICATE_PROVIDED_CONTRACT";
        }

        if (manifest.ConsumedContracts.GroupBy(value => $"{value.ContractIdentity}|{JoinVersions(value.CompatibleVersions)}", StringComparer.Ordinal).Any(group => group.Count() > 1))
        {
            return "DUPLICATE_CONSUMED_CONTRACT";
        }

        if (manifest.Dependencies.GroupBy(value => $"{value.Identity}|{value.Relation}", StringComparer.Ordinal).Any(group => group.Count() > 1))
        {
            return "DUPLICATE_DEPENDENCY";
        }

        foreach (var consumedContract in manifest.ConsumedContracts)
        {
            if (consumedContract.CompatibleVersions.GroupBy(value => value, StringComparer.Ordinal).Any(group => group.Count() > 1))
            {
                return "DUPLICATE_COMPATIBLE_VERSION";
            }
        }

        foreach (var dependency in manifest.Dependencies)
        {
            if (dependency.CompatibleVersions.GroupBy(value => value, StringComparer.Ordinal).Any(group => group.Count() > 1))
            {
                return "DUPLICATE_COMPATIBLE_VERSION";
            }
        }

        if (manifest.Lifecycle.SupportedTransitions is null || manifest.Lifecycle.SupportedTransitions.Count == 0)
        {
            return "INVALID_LIFECYCLE_DECLARATION";
        }

        if (manifest.Lifecycle.SupportedTransitions.Any(string.IsNullOrWhiteSpace))
        {
            return "INVALID_LIFECYCLE_DECLARATION";
        }

        if (manifest.Lifecycle.SupportedTransitions.Distinct(StringComparer.Ordinal).Count() != manifest.Lifecycle.SupportedTransitions.Count)
        {
            return "DUPLICATE_LIFECYCLE_TRANSITION";
        }
        if (manifest.AuthorizedConsumers.Intersect(manifest.RestrictedConsumers, StringComparer.Ordinal).Any()) return "CONSUMER_POLICY_CONFLICT";

        if (string.IsNullOrWhiteSpace(manifest.Lifecycle.DeclaredSourceState) || string.IsNullOrWhiteSpace(manifest.Lifecycle.UpdateBehavior) || string.IsNullOrWhiteSpace(manifest.Lifecycle.ReplacementBehavior) || string.IsNullOrWhiteSpace(manifest.Lifecycle.MigrationBehavior) || string.IsNullOrWhiteSpace(manifest.Lifecycle.RemovalBehavior))
        {
            return "INVALID_LIFECYCLE_DECLARATION";
        }

        if (string.IsNullOrWhiteSpace(manifest.OperationalBoundary.ResourceRequirements) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.HealthReportingRequirements) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.RecoveryRequirements) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.FailureContainmentRequirements) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.Permissions) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.AuthorityLimits) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.EvidenceRequirements) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.ProvenanceRequirements) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.IntegrityRequirements) ||
            string.IsNullOrWhiteSpace(manifest.OperationalBoundary.AutomaticActivationProhibition))
        {
            return "INVALID_OPERATIONAL_BOUNDARY";
        }

        if (!manifest.Protection.NoAutomaticRegistration || !manifest.Protection.NoAutomaticActivation || !manifest.Protection.NoAuthorityGain || !manifest.Protection.NoPermissionGain || !manifest.Protection.NoTrustGain || !manifest.Protection.NoResponsibilityGain)
        {
            return "INVALID_PROTECTION_DECLARATION";
        }

        foreach (var dependency in manifest.Dependencies)
        {
            if (string.IsNullOrWhiteSpace(dependency.Identity) || string.IsNullOrWhiteSpace(dependency.Kind) || string.IsNullOrWhiteSpace(dependency.Relation) || string.IsNullOrWhiteSpace(dependency.Purpose) || string.IsNullOrWhiteSpace(dependency.DegradedBehavior) || dependency.CompatibleVersions is null || dependency.CompatibleVersions.Count == 0)
            {
                return "INVALID_DEPENDENCY";
            }

            if (dependency.CompatibleVersions.Any(string.IsNullOrWhiteSpace))
            {
                return "INVALID_DEPENDENCY";
            }

            if (string.Equals(dependency.Identity, manifest.ServiceIdentity, StringComparison.Ordinal))
            {
                return "DIRECT_SELF_DEPENDENCY";
            }
        }

        return null;
    }

    private static string? ValidateContracts(ServiceManifest manifest)
    {
        var registry = Foundation.ContractRegistry.ContractRegistry.CreateCanonical();

        foreach (var requirement in manifest.ProvidedContracts)
        {
            var reason = ValidateProvidedContractRequirement(registry, requirement);
            if (reason is not null)
            {
                return reason;
            }
        }

        foreach (var requirement in manifest.ConsumedContracts)
        {
            var reason = ValidateConsumedContractRequirement(registry, requirement);
            if (reason is not null)
            {
                return reason;
            }
        }

        return null;
    }

    private static string? ValidateProvidedContractRequirement(Foundation.ContractRegistry.ContractRegistry registry, ServiceContractRequirement requirement)
    {
        if (string.IsNullOrWhiteSpace(requirement.ContractIdentity) || string.IsNullOrWhiteSpace(requirement.Version) || string.IsNullOrWhiteSpace(requirement.Purpose) || string.IsNullOrWhiteSpace(requirement.Relation))
        {
            return "MALFORMED_CONTRACT_REQUIREMENT";
        }

        var lookup = registry.Lookup(requirement.ContractIdentity, requirement.Version);
        if (lookup is null)
        {
            return registry.Entries.Any(entry => string.Equals(entry.ContractId, requirement.ContractIdentity, StringComparison.Ordinal))
                ? "INCOMPATIBLE_CONTRACT_VERSION"
                : "UNKNOWN_CONTRACT";
        }

        return string.Equals(lookup.Entry.Status, "ACCEPTED", StringComparison.Ordinal) && string.Equals(lookup.Entry.AdmissionState, "REGISTERED", StringComparison.Ordinal)
            ? null
            : "INACTIVE_CONTRACT";
    }

    private static string? ValidateConsumedContractRequirement(Foundation.ContractRegistry.ContractRegistry registry, ServiceConsumedContractRequirement requirement)
    {
        if (string.IsNullOrWhiteSpace(requirement.ContractIdentity) || string.IsNullOrWhiteSpace(requirement.Purpose) || string.IsNullOrWhiteSpace(requirement.Relation) || requirement.CompatibleVersions is null || requirement.CompatibleVersions.Count == 0)
        {
            return "MALFORMED_CONTRACT_REQUIREMENT";
        }

        if (requirement.CompatibleVersions.Any(string.IsNullOrWhiteSpace))
        {
            return "BLANK_COMPATIBLE_VERSION";
        }

        var matchingEntry = requirement.CompatibleVersions.Any(version =>
        {
            var lookup = registry.Lookup(requirement.ContractIdentity, version);
            return lookup is not null && string.Equals(lookup.Entry.Status, "ACCEPTED", StringComparison.Ordinal) && string.Equals(lookup.Entry.AdmissionState, "REGISTERED", StringComparison.Ordinal);
        });

        if (matchingEntry)
        {
            return null;
        }

        return registry.Entries.Any(entry => string.Equals(entry.ContractId, requirement.ContractIdentity, StringComparison.Ordinal))
            ? "INCOMPATIBLE_CONTRACT_VERSION"
            : "UNKNOWN_CONTRACT";
    }

    private static bool ValidateEnvelope(ServiceRegistrationRequest request, out string reason)
    {
        if (!string.Equals(request.RegistrationEnvelope.Payload, request.ManifestDigest, StringComparison.Ordinal))
        {
            reason = "MANIFEST_PAYLOAD_MISMATCH";
            return false;
        }

        if (request.RegistrationEnvelope.CreationTime == default || request.RegistrationEnvelope.Expiry is null)
        {
            reason = "INVALID_REGISTRATION_ENVELOPE";
            return false;
        }

        if (request.RegistrationEnvelope.CreationTime > request.ObservationTime)
        {
            reason = "ENVELOPE_NOT_YET_VALID";
            return false;
        }

        if (request.RegistrationEnvelope.Expiry <= request.RegistrationEnvelope.CreationTime)
        {
            reason = "INVALID_REGISTRATION_ENVELOPE";
            return false;
        }

        if (request.RegistrationEnvelope.Expiry <= request.ObservationTime)
        {
            reason = "ENVELOPE_EXPIRED";
            return false;
        }

        var outcome = ContractValidators.Validate(request.RegistrationEnvelope);
        if (outcome.Result != ValidationResult.Pass)
        {
            reason = "INVALID_REGISTRATION_ENVELOPE";
            return false;
        }

        if (!string.Equals(request.RegistrationEnvelope.MessageKind, "COMMAND", StringComparison.Ordinal))
        {
            reason = "INVALID_REGISTRATION_ENVELOPE_MESSAGE_KIND";
            return false;
        }

        if (!string.Equals(request.RegistrationEnvelope.MessageType, "SERVICE_REGISTRATION_REQUEST", StringComparison.Ordinal))
        {
            reason = "INVALID_REGISTRATION_ENVELOPE_MESSAGE_TYPE";
            return false;
        }

        if (!string.Equals(request.RegistrationEnvelope.SchemaId, "foundation.service-registration", StringComparison.Ordinal))
        {
            reason = "INVALID_REGISTRATION_ENVELOPE_SCHEMA_ID";
            return false;
        }

        if (!string.Equals(request.RegistrationEnvelope.SchemaVersion, "1.0", StringComparison.Ordinal))
        {
            reason = "INVALID_REGISTRATION_ENVELOPE_SCHEMA_VERSION";
            return false;
        }

        if (!string.Equals(request.RegistrationEnvelope.ProducerIdentity, request.RequesterIdentity, StringComparison.Ordinal))
        {
            reason = "PRODUCER_REQUESTER_MISMATCH";
            return false;
        }

        if (!string.Equals(
                request.RegistrationEnvelope.Purpose,
                "governed service registration",
                StringComparison.Ordinal))
        {
            reason = "INVALID_REGISTRATION_ENVELOPE_PURPOSE";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.RegistrationEnvelope.IntegrityEvidence) ||
            string.IsNullOrWhiteSpace(request.RegistrationEnvelope.ProtectionProfileId) ||
            string.IsNullOrWhiteSpace(request.RegistrationEnvelope.ProtectionProfileVersion) ||
            string.IsNullOrWhiteSpace(request.RegistrationEnvelope.ReplayPolicy) ||
            string.IsNullOrWhiteSpace(request.RegistrationEnvelope.DeliveryAttemptId))
        {
            reason = "INVALID_REGISTRATION_ENVELOPE";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static bool ValidateProviderEvidence(ServiceRegistrationRequest request, out string reason)
    {
        if (request.ServiceKind == ServiceKind.GeneralFoundationService)
        {
            reason = request.ProviderEvidence is null ? string.Empty : "NON_OPERATIONAL_PROVIDER_EVIDENCE";
            return request.ProviderEvidence is null;
        }

        if (request.ProviderEvidence is null)
        {
            reason = "MISSING_PROVIDER_EVIDENCE";
            return false;
        }

        if (request.ProviderEvidence.EvidenceOrigin != EvidenceOrigin.Operational)
        {
            reason = "NON_OPERATIONAL_PROVIDER_EVIDENCE";
            return false;
        }

        if (!string.Equals(request.ProviderEvidence.OperationalStatus, "ADMITTED", StringComparison.Ordinal))
        {
            reason = "PROVIDER_EVIDENCE_REJECTED";
            return false;
        }

        if (!string.Equals(request.ProviderEvidence.ServiceIdentity, request.ServiceIdentity, StringComparison.Ordinal) ||
            !string.Equals(request.ProviderEvidence.ServiceVersion, request.ServiceVersion, StringComparison.Ordinal))
        {
            reason = !string.Equals(request.ProviderEvidence.ServiceIdentity, request.ServiceIdentity, StringComparison.Ordinal)
                ? "PROVIDER_IDENTITY_MISMATCH"
                : "PROVIDER_VERSION_MISMATCH";
            return false;
        }

        if (request.ProviderEvidence.EffectiveTime == default || request.ProviderEvidence.Expiry <= request.ProviderEvidence.EffectiveTime)
        {
            reason = "MALFORMED_PROVIDER_EVIDENCE";
            return false;
        }

        if (request.ProviderEvidence.EffectiveTime > request.ObservationTime)
        {
            reason = "PROVIDER_NOT_EFFECTIVE";
            return false;
        }

        if (request.ProviderEvidence.Expiry <= request.ObservationTime)
        {
            reason = "PROVIDER_EVIDENCE_EXPIRED";
            return false;
        }

        return request.ProviderEvidence switch
        {
            IdentifierProviderEvidence identifier => ValidateIdentifierProviderEvidence(request, identifier, out reason),
            CryptographicProviderEvidence crypto => ValidateCryptographicProviderEvidence(request, crypto, out reason),
            SecretCustodyProviderEvidence secret => ValidateSecretCustodyProviderEvidence(request, secret, out reason),
            CertificateIdentityProviderEvidence certificate => ValidateCertificateIdentityProviderEvidence(request, certificate, out reason),
            RandomnessProviderEvidence randomness => ValidateRandomnessProviderEvidence(request, randomness, out reason),
            _ => FailProviderEvidenceTypeMismatch(out reason)
        };
    }

    private static bool ValidateIdentifierProviderEvidence(ServiceRegistrationRequest request, IdentifierProviderEvidence evidence, out string reason)
        => ValidateTypedProviderEvidence(
            request,
            evidence,
            ServiceKind.IdentifierProvider,
            ContractIdentity.Con014,
            ProviderContractVersions.Con014,
            evidence.ProviderRecord.ContractId,
            evidence.ProviderRecord.ProviderId,
            evidence.ProviderRecord.Version,
            evidence.ProviderRecord.EffectiveTime,
            evidence.ProviderRecord.Expiry,
            ProviderContractValidators.Validate(evidence.ProviderRecord),
            out reason);

    private static bool ValidateCryptographicProviderEvidence(ServiceRegistrationRequest request, CryptographicProviderEvidence evidence, out string reason)
        => ValidateTypedProviderEvidence(
            request,
            evidence,
            ServiceKind.CryptographicProvider,
            ContractIdentity.Con016,
            ProviderContractVersions.Con016,
            evidence.ProviderRecord.ContractId,
            evidence.ProviderRecord.ProviderId,
            evidence.ProviderRecord.Version,
            evidence.ProviderRecord.EffectiveTime,
            evidence.ProviderRecord.Expiry,
            ProviderContractValidators.Validate(evidence.ProviderRecord),
            out reason);

    private static bool ValidateSecretCustodyProviderEvidence(ServiceRegistrationRequest request, SecretCustodyProviderEvidence evidence, out string reason)
        => ValidateTypedProviderEvidence(
            request,
            evidence,
            ServiceKind.SecretCustodyProvider,
            ContractIdentity.Con017,
            ProviderContractVersions.Con017,
            evidence.ProviderRecord.ContractId,
            evidence.ProviderRecord.ProviderId,
            evidence.ProviderRecord.Version,
            evidence.ProviderRecord.EffectiveTime,
            evidence.ProviderRecord.Expiry,
            ProviderContractValidators.Validate(evidence.ProviderRecord),
            out reason);

    private static bool ValidateCertificateIdentityProviderEvidence(ServiceRegistrationRequest request, CertificateIdentityProviderEvidence evidence, out string reason)
        => ValidateTypedProviderEvidence(
            request,
            evidence,
            ServiceKind.CertificateIdentityProvider,
            ContractIdentity.Con018,
            ProviderContractVersions.Con018,
            evidence.ProviderRecord.ContractId,
            evidence.ProviderRecord.ProviderId,
            evidence.ProviderRecord.Version,
            evidence.ProviderRecord.EffectiveTime,
            evidence.ProviderRecord.Expiry,
            ProviderContractValidators.Validate(evidence.ProviderRecord),
            out reason);

    private static bool ValidateRandomnessProviderEvidence(ServiceRegistrationRequest request, RandomnessProviderEvidence evidence, out string reason)
        => ValidateTypedProviderEvidence(
            request,
            evidence,
            ServiceKind.RandomnessProvider,
            ContractIdentity.Con019,
            ProviderContractVersions.Con019,
            evidence.ProviderRecord.ContractId,
            evidence.ProviderRecord.ProviderId,
            evidence.ProviderRecord.Version,
            evidence.ProviderRecord.EffectiveTime,
            evidence.ProviderRecord.Expiry,
            ProviderContractValidators.Validate(evidence.ProviderRecord),
            out reason);

    private static bool ValidateTypedProviderEvidence<TProviderEvidence>(
        ServiceRegistrationRequest request,
        TProviderEvidence evidence,
        ServiceKind expectedKind,
        string expectedContractId,
        string expectedProviderVersion,
        string actualContractId,
        string providerId,
        string providerVersion,
        DateTimeOffset effectiveTime,
        DateTimeOffset expiry,
        ValidationOutcome validation,
        out string reason)
        where TProviderEvidence : ServiceProviderEvidence
    {
        if (validation.Result != ValidationResult.Pass)
        {
            reason = "MALFORMED_PROVIDER_EVIDENCE";
            return false;
        }

        if (request.ServiceKind != expectedKind || evidence.ServiceKind != expectedKind)
        {
            reason = "WRONG_PROVIDER_EVIDENCE_TYPE";
            return false;
        }

        if (string.IsNullOrWhiteSpace(evidence.ProviderContractIdentity))
        {
            reason = "MALFORMED_PROVIDER_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.ProviderContractIdentity, expectedContractId, StringComparison.Ordinal) ||
            !string.Equals(evidence.ProviderContractVersion, expectedProviderVersion, StringComparison.Ordinal) ||
            !string.Equals(actualContractId, expectedContractId, StringComparison.Ordinal) ||
            !string.Equals(providerId, request.ServiceIdentity, StringComparison.Ordinal) ||
            !string.Equals(providerVersion, expectedProviderVersion, StringComparison.Ordinal))
        {
            reason = "PROVIDER_CONTRACT_MISMATCH";
            return false;
        }

        if (effectiveTime > request.ObservationTime)
        {
            reason = "PROVIDER_NOT_EFFECTIVE";
            return false;
        }

        if (expiry <= request.ObservationTime)
        {
            reason = "PROVIDER_EVIDENCE_EXPIRED";
            return false;
        }

        if (evidence.EffectiveTime > effectiveTime || evidence.Expiry < expiry)
        {
            reason = "MALFORMED_PROVIDER_EVIDENCE";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static bool FailProviderEvidenceTypeMismatch(out string reason)
    {
        reason = "WRONG_PROVIDER_EVIDENCE_TYPE";
        return false;
    }

    private void RecordResponsibilityOwnership(string responsibilityIdentity, string responsibilityMeaning, string owner, ServiceCatalogKey key, int sequence)
    {
        var ownership = new ServiceResponsibilityOwnership
        {
            ResponsibilityIdentity = responsibilityIdentity,
            ResponsibilityMeaning = responsibilityMeaning,
            Owner = owner,
            Key = key,
            RegistrationSequence = sequence
        };

        _responsibilitiesByPair[ResponsibilityKey(responsibilityIdentity, responsibilityMeaning)] = ownership;
        _responsibilitiesByIdentity[ResponsibilityIdentityKey(responsibilityIdentity)] = ownership;
        _responsibilitiesByMeaning[ResponsibilityMeaningKey(responsibilityMeaning)] = ownership;
    }

    private static ServiceManifest CreateManifestSnapshot(ServiceManifest manifest)
        => new(
            manifest.ServiceIdentity,
            manifest.ServiceVersion,
            manifest.AccountableOwner,
            manifest.Purpose,
            manifest.ExclusiveResponsibilityBoundary,
            manifest.OwnedResponsibilities.Select(value => value with { }),
            manifest.ProvidedContracts.Select(value => value with { }),
            manifest.ConsumedContracts.Select(value => value with { CompatibleVersions = value.CompatibleVersions.ToArray() }),
            manifest.AuthorizedConsumers.ToArray(),
            manifest.RestrictedConsumers.ToArray(),
            manifest.Lifecycle with { SupportedTransitions = manifest.Lifecycle.SupportedTransitions.ToArray() },
            manifest.Dependencies.Select(value => value with { CompatibleVersions = value.CompatibleVersions.ToArray() }),
            manifest.OperationalBoundary,
            manifest.Protection,
            manifest.ManifestId,
            manifest.PackageIdentity,
            manifest.PackageVersion,
            manifest.PackageContentOrIntegrityInput,
            manifest.CsaEligibilityPolicy,
            manifest.SelfDevelopmentOriginAndEscalationPath,
            manifest.GuardianAndProtectionInterface);

    private static bool TryDetectResponsibilityCollision(Dictionary<string, ServiceResponsibilityOwnership> index, string lookupKey, string serviceIdentity, out string reason)
    {
        if (index.TryGetValue(lookupKey, out var existing) &&
            !string.Equals(existing.Key.ServiceIdentity, serviceIdentity, StringComparison.Ordinal))
        {
            reason = lookupKey == ResponsibilityIdentityKey(existing.ResponsibilityIdentity)
                ? "RESPONSIBILITY_IDENTITY_COLLISION"
                : lookupKey == ResponsibilityMeaningKey(existing.ResponsibilityMeaning)
                    ? "RESPONSIBILITY_MEANING_COLLISION"
                    : "RESPONSIBILITY_COLLISION";
            return true;
        }

        reason = string.Empty;
        return false;
    }

    private static bool ResponsibilityLineageMatches(IReadOnlyCollection<ServiceResponsibilityDeclaration> previous, IReadOnlyCollection<ServiceResponsibilityDeclaration> current)
    {
        var previousSet = previous
            .Select(item => ResponsibilityKey(item.Identity, item.Responsibility))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        var currentSet = current
            .Select(item => ResponsibilityKey(item.Identity, item.Responsibility))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        return previousSet.SequenceEqual(currentSet, StringComparer.Ordinal);
    }

    private static string ResponsibilityKey(string identity, string meaning) => $"{identity.Length}:{identity}|{meaning.Length}:{meaning}";
    private static string ResponsibilityIdentityKey(string identity) => $"{identity.Length}:{identity}";
    private static string ResponsibilityMeaningKey(string meaning) => $"{meaning.Length}:{meaning}";
    private static string JoinVersions(IReadOnlyCollection<string>? values)
        => string.Join(';', (values ?? Array.Empty<string>()).OrderBy(value => value, StringComparer.Ordinal));
    private static string ComputeSha256(string content) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content ?? string.Empty)));
}

public static class ServiceManifestCanonicalizer
{
    public static string Serialize(ServiceManifest? manifest)
    {
        if (manifest is null) return string.Empty;
        var builder = new StringBuilder();
        Append(builder, nameof(manifest.ServiceIdentity), manifest.ServiceIdentity);
        Append(builder, nameof(manifest.ServiceVersion), manifest.ServiceVersion);
        Append(builder, nameof(manifest.AccountableOwner), manifest.AccountableOwner);
        Append(builder, nameof(manifest.Purpose), manifest.Purpose);
        Append(builder, nameof(manifest.ExclusiveResponsibilityBoundary), manifest.ExclusiveResponsibilityBoundary);
        Append(builder, nameof(manifest.OwnedResponsibilities), JoinSorted(manifest.OwnedResponsibilities.Select(value => $"{Escape(value.Identity)}|{Escape(value.Owner)}|{Escape(value.Responsibility)}")));
        Append(builder, nameof(manifest.ProvidedContracts), JoinSorted(manifest.ProvidedContracts.Select(value => $"{Escape(value.ContractIdentity)}|{Escape(value.Version)}|{Escape(value.Purpose)}|{Escape(value.Relation)}")));
        Append(builder, nameof(manifest.ConsumedContracts), JoinSorted(manifest.ConsumedContracts.Select(value => $"{Escape(value.ContractIdentity)}|{JoinSorted(value.CompatibleVersions.Select(Escape))}|{Escape(value.Purpose)}|{Escape(value.Relation)}")));
        Append(builder, nameof(manifest.AuthorizedConsumers), JoinSorted(manifest.AuthorizedConsumers.Select(Escape)));
        Append(builder, nameof(manifest.RestrictedConsumers), JoinSorted(manifest.RestrictedConsumers.Select(Escape)));
        Append(builder, nameof(manifest.Lifecycle), $"{Escape(manifest.Lifecycle.DeclaredSourceState)}|{JoinSorted(manifest.Lifecycle.SupportedTransitions.Select(Escape))}|{Escape(manifest.Lifecycle.UpdateBehavior)}|{Escape(manifest.Lifecycle.ReplacementBehavior)}|{Escape(manifest.Lifecycle.MigrationBehavior)}|{Escape(manifest.Lifecycle.RemovalBehavior)}");
        Append(builder, nameof(manifest.Dependencies), JoinSorted(manifest.Dependencies.Select(value => $"{Escape(value.Identity)}|{JoinSorted(value.CompatibleVersions.Select(Escape))}|{Escape(value.Kind)}|{Escape(value.Relation)}|{Escape(value.Purpose)}|{Escape(value.DegradedBehavior)}")));
        Append(builder, nameof(manifest.OperationalBoundary), $"{Escape(manifest.OperationalBoundary.ResourceRequirements)}|{Escape(manifest.OperationalBoundary.HealthReportingRequirements)}|{Escape(manifest.OperationalBoundary.RecoveryRequirements)}|{Escape(manifest.OperationalBoundary.FailureContainmentRequirements)}|{Escape(manifest.OperationalBoundary.Permissions)}|{Escape(manifest.OperationalBoundary.AuthorityLimits)}|{Escape(manifest.OperationalBoundary.EvidenceRequirements)}|{Escape(manifest.OperationalBoundary.ProvenanceRequirements)}|{Escape(manifest.OperationalBoundary.IntegrityRequirements)}|{Escape(manifest.OperationalBoundary.AutomaticActivationProhibition)}");
        Append(builder, nameof(manifest.Protection), $"{manifest.Protection.NoAutomaticRegistration}|{manifest.Protection.NoAutomaticActivation}|{manifest.Protection.NoAuthorityGain}|{manifest.Protection.NoPermissionGain}|{manifest.Protection.NoTrustGain}|{manifest.Protection.NoResponsibilityGain}");
        Append(builder, nameof(manifest.ManifestId), manifest.ManifestId);
        Append(builder, nameof(manifest.PackageIdentity), manifest.PackageIdentity);
        Append(builder, nameof(manifest.PackageVersion), manifest.PackageVersion);
        Append(builder, nameof(manifest.PackageContentOrIntegrityInput), manifest.PackageContentOrIntegrityInput);
        Append(builder, nameof(manifest.CsaEligibilityPolicy), manifest.CsaEligibilityPolicy);
        Append(builder, nameof(manifest.SelfDevelopmentOriginAndEscalationPath), manifest.SelfDevelopmentOriginAndEscalationPath);
        Append(builder, nameof(manifest.GuardianAndProtectionInterface), manifest.GuardianAndProtectionInterface);
        return builder.ToString();
    }

    public static string ComputeDigest(ServiceManifest? manifest)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Serialize(manifest))));

    private static void Append(StringBuilder builder, string name, string value)
    {
        builder.Append(name);
        builder.Append('=');
        builder.Append(Escape(value));
        builder.Append('\n');
    }

    private static string JoinSorted(IEnumerable<string> values)
    {
        var copy = values.ToArray();
        Array.Sort(copy, StringComparer.Ordinal);
        return string.Join(';', copy);
    }

    private static string Escape(string? value)
        => (value ?? string.Empty)
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal)
            .Replace(";", "\\;", StringComparison.Ordinal)
            .Replace("=", "\\=", StringComparison.Ordinal);
}
