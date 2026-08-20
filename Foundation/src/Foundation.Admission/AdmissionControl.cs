using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Contracts;
using Foundation.ContractRegistry;
using ContractRegistryType = Foundation.ContractRegistry.ContractRegistry;

namespace Foundation.Admission;

public sealed record AdmissionRequest(
    string AdmissionId,
    string AdmissionKind,
    string Identity,
    string Version,
    string Owner,
    string AuthoritySource,
    string ContractId,
    string ContractVersion,
    string ManifestId,
    ApplicationManifest Manifest,
    string ManifestDigest,
    string ProvenanceId,
    string ProvenanceContent,
    string ProvenanceDigest,
    string BootstrapContextId,
    string BootstrapContextState,
    string ProviderBoundary,
    string DecisionSeed);

public sealed record AdmissionDecision(
    string AdmissionId,
    string Decision,
    string ReasonCode,
    string ContractId,
    string ContractVersion,
    string EvidenceId);

public sealed record AdmissionValidationResult(bool Success, string Message)
{
    public static AdmissionValidationResult Pass(string message) => new(true, message);
    public static AdmissionValidationResult Fail(string message) => new(false, message);
}

internal readonly record struct AdmissionSubjectKey(string AdmissionKind, string Identity, string Version);

public sealed class AdmissionControl
{
    private readonly object _sync = new();
    private readonly AdmissionBaselineSnapshot _baseline;
    private readonly ContractRegistryType _registry;
    private readonly HashSet<string> _admissionIds = new(StringComparer.Ordinal);
    private readonly HashSet<AdmissionSubjectKey> _subjectKeys = new();

    public AdmissionControl(IAdmissionBaselineProvider baselineProvider)
    {
        ArgumentNullException.ThrowIfNull(baselineProvider);
        _baseline = baselineProvider.GetCurrentBaseline()
            ?? throw new InvalidOperationException("missing admission baseline");
        _registry = _baseline.BuildRegistry();
        EnsureCanonicalRegistryIntegrity();
        ValidateBaselineSnapshot();
    }

    public AdmissionDecision Evaluate(AdmissionRequest? request)
    {
        if (request is null)
        {
            return Rejected(null, "invalid admission request");
        }

        lock (_sync)
        {
            if (string.IsNullOrWhiteSpace(request.AdmissionId))
            {
                return Rejected(request, "missing admission identity");
            }

            if (!_admissionIds.Add(request.AdmissionId))
            {
                return Rejected(request, "duplicate admission identity");
            }

            var validation = ValidateCore(request);
            if (!validation.Success)
            {
                return Rejected(request, validation.Message);
            }

            var subjectKey = new AdmissionSubjectKey(
                request.AdmissionKind,
                request.Identity,
                request.Version);
            if (!_subjectKeys.Add(subjectKey))
            {
                return Rejected(request, "duplicate application or plug-in identity");
            }

            return new AdmissionDecision(
                request.AdmissionId,
                "ADMITTED",
                "admission accepted",
                request.ContractId,
                request.ContractVersion,
                EvidenceId(request));
        }
    }

    public AdmissionValidationResult Validate(AdmissionRequest? request)
    {
        if (request is null)
        {
            return AdmissionValidationResult.Fail("invalid admission request");
        }

        lock (_sync)
        {
            return ValidateCore(request);
        }
    }

    private AdmissionValidationResult ValidateCore(AdmissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AdmissionId))
        {
            return AdmissionValidationResult.Fail("missing admission identity");
        }

        if (!IsOneOf(request.AdmissionKind, "APPLICATION", "PLUG-IN"))
        {
            return AdmissionValidationResult.Fail("unsupported admission kind");
        }

        if (string.IsNullOrWhiteSpace(request.Identity) || string.IsNullOrWhiteSpace(request.Version) || string.IsNullOrWhiteSpace(request.Owner))
        {
            return AdmissionValidationResult.Fail("missing admission identity fields");
        }

        if (string.IsNullOrWhiteSpace(request.AuthoritySource)) return AdmissionValidationResult.Fail("missing authority source");
        if (string.IsNullOrWhiteSpace(request.ContractId) || string.IsNullOrWhiteSpace(request.ContractVersion)) return AdmissionValidationResult.Fail("missing contract declaration");
        if (request.Manifest is null) return AdmissionValidationResult.Fail("missing manifest content");
        if (string.IsNullOrWhiteSpace(request.ManifestId) || string.IsNullOrWhiteSpace(request.ManifestDigest)) return AdmissionValidationResult.Fail("missing manifest identity");
        if (string.IsNullOrWhiteSpace(request.ProvenanceId) || string.IsNullOrWhiteSpace(request.ProvenanceDigest)) return AdmissionValidationResult.Fail("missing provenance");
        if (string.IsNullOrWhiteSpace(request.ProvenanceContent)) return AdmissionValidationResult.Fail("missing provenance content");
        if (string.IsNullOrWhiteSpace(request.BootstrapContextId) || string.IsNullOrWhiteSpace(request.BootstrapContextState)) return AdmissionValidationResult.Fail("missing bootstrap context");
        if (string.IsNullOrWhiteSpace(request.ProviderBoundary)) return AdmissionValidationResult.Fail("missing provider boundary");
        if (!string.Equals(request.BootstrapContextState, "DEFINED", StringComparison.Ordinal)) return AdmissionValidationResult.Fail("invalid bootstrap context");
        if (request.ProviderBoundary.Contains("bypass", StringComparison.OrdinalIgnoreCase) || request.ProviderBoundary.Contains("unapproved", StringComparison.OrdinalIgnoreCase)) return AdmissionValidationResult.Fail("provider-boundary bypass");
        if (string.IsNullOrWhiteSpace(request.DecisionSeed)) return AdmissionValidationResult.Fail("missing decision seed");

        if (!string.Equals(request.Manifest.ManifestId, request.ManifestId, StringComparison.Ordinal)) return AdmissionValidationResult.Fail("manifest mismatch");
        if (!string.Equals(request.Manifest.ApplicationIdentity, request.Identity, StringComparison.Ordinal) ||
            !string.Equals(request.Manifest.ApplicationVersion, request.Version, StringComparison.Ordinal) ||
            !string.Equals(request.Manifest.ApplicationOwner, request.Owner, StringComparison.Ordinal)) return AdmissionValidationResult.Fail("manifest mismatch");
        if (!string.Equals(ApplicationManifestCanonicalizer.ComputeDigest(request.Manifest), request.ManifestDigest, StringComparison.OrdinalIgnoreCase)) return AdmissionValidationResult.Fail("manifest mismatch");
        if (!string.Equals(ComputeSha256(request.ProvenanceContent), request.ProvenanceDigest, StringComparison.OrdinalIgnoreCase)) return AdmissionValidationResult.Fail("provenance mismatch");

        if (!HasRequiredManifestDeclarations(request.Manifest)) return AdmissionValidationResult.Fail("missing mandatory manifest declaration group");
        if (!HasValidBranchDeclarations(request.Manifest)) return AdmissionValidationResult.Fail("missing or invalid branch declarations");
        if (!ValidateDeclaredDependencies(request.Manifest.DeclaredDependencies)) return AdmissionValidationResult.Fail("invalid dependency declarations");
        if (!ValidateFoundationRequirements(request.Manifest.RequiredFoundationContracts, ValidateRequiredContract)) return AdmissionValidationResult.Fail("invalid required foundation contracts");
        if (!ValidateFoundationRequirements(request.Manifest.RequiredFoundationSpecifications, ValidateRequiredSpecification)) return AdmissionValidationResult.Fail("invalid required foundation specifications");
        if (!ValidateFoundationServices(request.Manifest.RequiredFoundationServices)) return AdmissionValidationResult.Fail("invalid required foundation services");
        if (!ValidatePermissions(request.Manifest.RequestedPermissions)) return AdmissionValidationResult.Fail("invalid requested permissions");
        if (!ValidateAuthorities(request.Manifest.AuthorityRequests)) return AdmissionValidationResult.Fail("invalid authority requests");
        if (!HasUniqueDeclarations(request.Manifest)) return AdmissionValidationResult.Fail("duplicate manifest declaration");

        var canonical = _registry.Lookup(request.ContractId, request.ContractVersion);
        if (canonical is null) return AdmissionValidationResult.Fail("unknown contract");
        if (!string.Equals(canonical.Entry.AuthoritySource, request.AuthoritySource, StringComparison.Ordinal)) return AdmissionValidationResult.Fail("invalid authority linkage");
        if (!string.Equals(canonical.Entry.ContractId, request.ContractId, StringComparison.Ordinal) || !string.Equals(canonical.Entry.Version, request.ContractVersion, StringComparison.Ordinal)) return AdmissionValidationResult.Fail("unsupported contract version");
        if (!string.Equals(request.AuthoritySource, _baseline.AuthoritySourceRequirement, StringComparison.Ordinal)) return AdmissionValidationResult.Fail("authority-source mismatch");
        if (!string.Equals(request.Owner, request.Manifest.ApplicationOwner, StringComparison.Ordinal)) return AdmissionValidationResult.Fail("missing or invalid ownership");
        if (!ValidateRequiredFoundationReferences(request.Manifest)) return AdmissionValidationResult.Fail("inactive or unregistered required foundation reference");
        if (!IsManifestCompatible(request.ManifestId, request.ManifestDigest)) return AdmissionValidationResult.Fail("manifest mismatch");
        if (!IsProvenanceCompatible(request.ProvenanceId, request.ProvenanceContent, request.ProvenanceDigest)) return AdmissionValidationResult.Fail("provenance mismatch");

        return AdmissionValidationResult.Pass("ok");
    }

    private bool HasRequiredManifestDeclarations(ApplicationManifest manifest)
        => manifest.DeclaredDependencies is not null
           && manifest.RequiredFoundationContracts is not null
           && manifest.RequiredFoundationSpecifications is not null
           && manifest.RequiredFoundationServices is not null
           && manifest.ProvidedCapabilities is not null
           && manifest.IntendedConsumers is not null
           && manifest.RequestedPermissions is not null
           && manifest.AuthorityRequests is not null
           && manifest.SecurityProfile is not null
           && manifest.MinimumResourceRequirements is not null
           && manifest.ResourceCeilings is not null
           && manifest.LifecycleBehavior is not null
           && manifest.MsaDeclarations is not null
           && manifest.MajorBranchDeclarations is not null
           && manifest.LsaDeclarations is not null
           && !string.IsNullOrWhiteSpace(manifest.ManifestId)
           && !string.IsNullOrWhiteSpace(manifest.ApplicationIdentity)
           && !string.IsNullOrWhiteSpace(manifest.ApplicationVersion)
           && !string.IsNullOrWhiteSpace(manifest.ApplicationOwner)
           && !string.IsNullOrWhiteSpace(manifest.ApplicationPurpose)
           && !string.IsNullOrWhiteSpace(manifest.PackageIdentity)
           && !string.IsNullOrWhiteSpace(manifest.PackageVersion)
           && !string.IsNullOrWhiteSpace(manifest.PackageContentOrIntegrityInput)
           && ValidateDeclaredDependencies(manifest.DeclaredDependencies)
           && manifest.RequiredFoundationContracts.Count > 0
           && manifest.RequiredFoundationSpecifications.Count > 0
           && manifest.RequiredFoundationServices.Count > 0
           && manifest.ProvidedCapabilities.Count > 0
           && manifest.IntendedConsumers.Count > 0
           && manifest.RequestedPermissions.Count > 0
           && manifest.AuthorityRequests.Count > 0
           && !string.IsNullOrWhiteSpace(manifest.SecurityProfile.Name)
           && !string.IsNullOrWhiteSpace(manifest.SecurityProfile.Classification)
           && !string.IsNullOrWhiteSpace(manifest.SecurityProfile.IsolationModel)
           && !string.IsNullOrWhiteSpace(manifest.MinimumResourceRequirements.Memory)
           && !string.IsNullOrWhiteSpace(manifest.MinimumResourceRequirements.Cpu)
           && !string.IsNullOrWhiteSpace(manifest.MinimumResourceRequirements.Storage)
           && !string.IsNullOrWhiteSpace(manifest.MinimumResourceRequirements.Network)
           && !string.IsNullOrWhiteSpace(manifest.ResourceCeilings.Memory)
           && !string.IsNullOrWhiteSpace(manifest.ResourceCeilings.Cpu)
           && !string.IsNullOrWhiteSpace(manifest.ResourceCeilings.Storage)
           && !string.IsNullOrWhiteSpace(manifest.ResourceCeilings.Network)
           && !string.IsNullOrWhiteSpace(manifest.DegradedBehavior)
           && !string.IsNullOrWhiteSpace(manifest.PersistenceRequirements)
           && !string.IsNullOrWhiteSpace(manifest.CommunicationRequirements)
           && !string.IsNullOrWhiteSpace(manifest.ConfigurationRequirements)
           && !string.IsNullOrWhiteSpace(manifest.EvidenceRequirements)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Installation)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Validation)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Registration)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Admission)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Activation)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Update)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Suspension)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Recovery)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Replacement)
           && !string.IsNullOrWhiteSpace(manifest.LifecycleBehavior.Removal)
           && !string.IsNullOrWhiteSpace(manifest.HealthReportingInterface)
           && !string.IsNullOrWhiteSpace(manifest.FailureContainmentInterface)
           && manifest.MsaDeclarations.Count == 1
           && manifest.MsaDeclarations.All(msa => msa is not null && !string.IsNullOrWhiteSpace(msa.Identity) && !string.IsNullOrWhiteSpace(msa.Owner) && !string.IsNullOrWhiteSpace(msa.Scope))
           && !string.IsNullOrWhiteSpace(manifest.CsaEligibilityPolicy)
           && !string.IsNullOrWhiteSpace(manifest.SelfDevelopmentOriginAndEscalationPath)
           && !string.IsNullOrWhiteSpace(manifest.GuardianAndProtectionInterface)
           && !string.IsNullOrWhiteSpace(manifest.RollbackOrCorrectiveActionPlan);

    private bool HasValidBranchDeclarations(ApplicationManifest manifest)
    {
        if (manifest.UsesBranchBasedInternalArchitecture && manifest.MajorBranchDeclarations.Count == 0)
        {
            return false;
        }

        if (manifest.MajorBranchDeclarations.Any(branch => branch is null) ||
            manifest.LsaDeclarations.Any(lsa => lsa is null))
        {
            return false;
        }

        var branchNames = manifest.MajorBranchDeclarations.Select(branch => branch.BranchName).ToArray();
        if (branchNames.Any(string.IsNullOrWhiteSpace) ||
            branchNames.Length != branchNames.Distinct(StringComparer.Ordinal).Count() ||
            manifest.MajorBranchDeclarations.Any(branch => string.IsNullOrWhiteSpace(branch.Purpose) || string.IsNullOrWhiteSpace(branch.ResponsibleLsa)) ||
            manifest.LsaDeclarations.Any(lsa => string.IsNullOrWhiteSpace(lsa.BranchName) || string.IsNullOrWhiteSpace(lsa.ResponsibleLsa) || string.IsNullOrWhiteSpace(lsa.Scope)))
        {
            return false;
        }

        var lsaGroups = manifest.LsaDeclarations.GroupBy(lsa => lsa.BranchName, StringComparer.Ordinal).ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);

        foreach (var branch in manifest.MajorBranchDeclarations)
        {
            if (!lsaGroups.TryGetValue(branch.BranchName, out var lsas) || lsas.Count != 1)
            {
                return false;
            }

            if (!string.Equals(lsas[0].ResponsibleLsa, branch.ResponsibleLsa, StringComparison.Ordinal))
            {
                return false;
            }
        }

        foreach (var lsa in manifest.LsaDeclarations)
        {
            if (!branchNames.Contains(lsa.BranchName, StringComparer.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ValidateFoundationRequirements<T>(IReadOnlyList<T>? requirements, Func<T, bool> validator)
        => requirements is not null && requirements.Count > 0 && requirements.All(item => item is not null && validator(item));

    private static bool ValidateFoundationServices(IReadOnlyList<FoundationServiceRequirement>? services)
        => services is not null && services.Count > 0 && services.All(service => service is not null && !string.IsNullOrWhiteSpace(service.Identity) && !string.IsNullOrWhiteSpace(service.Version) && !string.IsNullOrWhiteSpace(service.Purpose));

    private static bool ValidatePermissions(IReadOnlyList<PermissionDeclaration>? permissions)
        => permissions is not null && permissions.Count > 0 && permissions.All(permission => permission is not null && !string.IsNullOrWhiteSpace(permission.Name) && !string.IsNullOrWhiteSpace(permission.Scope) && !string.IsNullOrWhiteSpace(permission.Rationale));

    private static bool ValidateAuthorities(IReadOnlyList<AuthorityRequest>? authorities)
        => authorities is not null && authorities.Count > 0 && authorities.All(authority => authority is not null && !string.IsNullOrWhiteSpace(authority.Name) && !string.IsNullOrWhiteSpace(authority.Scope) && !string.IsNullOrWhiteSpace(authority.Rationale));

    private bool ValidateDeclaredDependencies(IReadOnlyList<DependencyDeclaration>? dependencies)
    {
        if (dependencies is null || dependencies.Count == 0)
        {
            return false;
        }

        foreach (var dependency in dependencies)
        {
            if (dependency is null ||
                string.IsNullOrWhiteSpace(dependency.Identity) ||
                dependency.CompatibleVersions is null ||
                dependency.CompatibleVersions.Count == 0 ||
                dependency.CompatibleVersions.Any(string.IsNullOrWhiteSpace))
            {
                return false;
            }

            var resolved = dependency.CompatibleVersions
                .Where(version => !string.IsNullOrWhiteSpace(version))
                .Any(version => _registry.Lookup(dependency.Identity, version) is not null);

            if (!resolved)
            {
                return false;
            }
        }

        return true;
    }


    private static bool HasUniqueDeclarations(ApplicationManifest manifest)
        => Unique(manifest.DeclaredDependencies, value => Canonical(value.Identity, string.Join(";", value.CompatibleVersions.OrderBy(item => item, StringComparer.Ordinal))))
           && Unique(manifest.RequiredFoundationContracts, value => Canonical(value.Identity, value.Version, value.Owner, value.AuthoritySource))
           && Unique(manifest.RequiredFoundationSpecifications, value => Canonical(value.Identity, value.Version, value.Owner, value.AuthoritySource))
           && Unique(manifest.RequiredFoundationServices, value => Canonical(value.Identity, value.Version, value.Purpose))
           && Unique(manifest.ProvidedCapabilities, value => value)
           && Unique(manifest.IntendedConsumers, value => value)
           && Unique(manifest.RequestedPermissions, value => Canonical(value.Name, value.Scope, value.Rationale))
           && Unique(manifest.AuthorityRequests, value => Canonical(value.Name, value.Scope, value.Rationale))
           && Unique(manifest.MsaDeclarations, value => Canonical(value.Identity, value.Owner, value.Scope))
           && Unique(manifest.MajorBranchDeclarations, value => Canonical(value.BranchName, value.Purpose, value.ResponsibleLsa))
           && Unique(manifest.LsaDeclarations, value => Canonical(value.BranchName, value.ResponsibleLsa, value.Scope));

    private static bool Unique<T>(IReadOnlyList<T>? values, Func<T, string> identity)
    {
        if (values is null || values.Any(value => value is null)) return false;
        var set = new HashSet<string>(StringComparer.Ordinal);
        return values.All(value => set.Add(identity(value)));
    }

    private static string Canonical(params string?[] values)
        => string.Concat(values.Select(value => $"{System.Text.Encoding.UTF8.GetByteCount(value ?? string.Empty)}:{value ?? string.Empty};"));

    private static AdmissionDecision Rejected(AdmissionRequest? request, string reason)
        => new(
            request?.AdmissionId ?? string.Empty,
            "REJECTED",
            reason,
            request?.ContractId ?? string.Empty,
            request?.ContractVersion ?? string.Empty,
            EvidenceId(request));

    private void ValidateBaselineSnapshot()
    {
        var applicationContract = _registry.Lookup(_baseline.ApplicationContract.ContractId, _baseline.ApplicationContract.Version)
            ?? throw new InvalidOperationException("baseline application contract missing");

        if (!MatchesEntry(applicationContract.Entry, _baseline.ApplicationContract))
        {
            throw new InvalidOperationException("baseline application contract mismatch");
        }

        if (!string.Equals(_baseline.ApplicationContract.Owner, _baseline.ContractOwnerRequirement, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("baseline contract owner requirement mismatch");
        }

        if (!string.Equals(_baseline.ApplicationContract.AuthoritySource, _baseline.AuthoritySourceRequirement, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("baseline authority source requirement mismatch");
        }

        if (!IsBaselineActive(_baseline.ApplicationContract, _baseline.ApplicationContractStatusRequirement, _baseline.ApplicationContractAdmissionStateRequirement))
        {
            throw new InvalidOperationException("baseline application contract not active");
        }

        if (!IsBaselineActive(_baseline.ApplicationBoundary, _baseline.ApplicationBoundaryStatusRequirement, _baseline.ApplicationBoundaryAdmissionStateRequirement))
        {
            throw new InvalidOperationException("baseline application boundary not active");
        }
    }

    private void EnsureCanonicalRegistryIntegrity()
    {
        var coverage = _registry.ValidateCanonicalCoverage();
        if (!coverage.Success)
        {
            throw new InvalidOperationException(coverage.Message);
        }

        var integrity = _registry.ValidateCanonicalIntegrity();
        if (!integrity.Success)
        {
            throw new InvalidOperationException(integrity.Message);
        }
    }

    private static bool MatchesEntry(ContractRegistryEntry actual, ContractRegistryEntry expected)
        => string.Equals(actual.ContractId, expected.ContractId, StringComparison.Ordinal)
           && string.Equals(actual.Version, expected.Version, StringComparison.Ordinal)
           && string.Equals(actual.Owner, expected.Owner, StringComparison.Ordinal)
           && string.Equals(actual.AuthoritySource, expected.AuthoritySource, StringComparison.Ordinal)
           && string.Equals(actual.ControlSurface, expected.ControlSurface, StringComparison.Ordinal)
           && string.Equals(actual.SchemaOrExecutableRepresentation, expected.SchemaOrExecutableRepresentation, StringComparison.Ordinal)
           && string.Equals(actual.Status, expected.Status, StringComparison.Ordinal)
           && string.Equals(actual.AdmissionState, expected.AdmissionState, StringComparison.Ordinal);

    private bool ValidateRequiredFoundationReferences(ApplicationManifest manifest)
        => manifest.RequiredFoundationContracts.All(ValidateRequiredContract)
           && manifest.RequiredFoundationSpecifications.All(ValidateRequiredSpecification);

    private bool ValidateRequiredContract(FoundationRequirement requirement)
    {
        var lookup = _registry.Lookup(requirement.Identity, requirement.Version);
        if (lookup is null)
        {
            return false;
        }

        if (!string.Equals(lookup.Entry.Owner, requirement.Owner, StringComparison.Ordinal) ||
            !string.Equals(lookup.Entry.AuthoritySource, requirement.AuthoritySource, StringComparison.Ordinal))
        {
            return false;
        }

        return IsContractActive(lookup.Entry);
    }

    private bool ValidateRequiredSpecification(FoundationRequirement requirement)
        => string.Equals(requirement.Identity, "APP-001", StringComparison.Ordinal)
           && string.Equals(requirement.Version, _baseline.ApplicationBoundary.Version, StringComparison.Ordinal)
           && string.Equals(requirement.Owner, _baseline.ApplicationBoundary.Owner, StringComparison.Ordinal)
           && string.Equals(requirement.AuthoritySource, _baseline.ApplicationBoundary.AuthoritySource, StringComparison.Ordinal)
           && IsBaselineActive(_baseline.ApplicationBoundary, _baseline.ApplicationBoundaryStatusRequirement, _baseline.ApplicationBoundaryAdmissionStateRequirement);

    private static bool IsManifestCompatible(string id, string digest)
        => !string.IsNullOrWhiteSpace(id) && IsHexDigest(digest);

    private static bool IsProvenanceCompatible(string id, string content, string digest)
        => !string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(content) && IsHexDigest(digest) && string.Equals(ComputeSha256(content), digest, StringComparison.OrdinalIgnoreCase);

    private static bool IsOneOf(string value, params string[] options)
        => options.Any(option => string.Equals(value, option, StringComparison.Ordinal));

    private static bool IsHexDigest(string? value)
        => value is not null && value.Length == 64 && value.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));

    private static bool IsBaselineActive(ContractRegistryEntry entry, string requiredStatus, string requiredAdmissionState)
        => string.Equals(entry.Status, requiredStatus, StringComparison.Ordinal)
           && string.Equals(entry.AdmissionState, requiredAdmissionState, StringComparison.Ordinal);

    private static bool IsContractActive(ContractRegistryEntry entry)
        => string.Equals(entry.Status, "ACCEPTED", StringComparison.Ordinal) && string.Equals(entry.AdmissionState, "REGISTERED", StringComparison.Ordinal);

    private static string EvidenceId(AdmissionRequest? request)
    {
        var canonical = Canonical(
            request?.AdmissionId,
            request?.AdmissionKind,
            request?.Identity,
            request?.Version,
            request?.ContractId,
            request?.ContractVersion,
            request?.ManifestId,
            request?.ProvenanceId,
            request?.DecisionSeed);
        return $"ADMISSION-EVIDENCE:{ComputeSha256(canonical)}";
    }

    private static string ComputeSha256(string content)
        => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(content)));
}
