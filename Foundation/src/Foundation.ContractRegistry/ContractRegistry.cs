using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Foundation.ContractRegistry;

public sealed record ContractRegistryEntry(
    string ContractId,
    string Version,
    string Owner,
    string AuthoritySource,
    string ControlSurface,
    string SchemaOrExecutableRepresentation,
    string Status,
    string AdmissionState)
{
    public string RegistryKey =>
        $"{(ContractId ?? string.Empty).Length}:{ContractId ?? string.Empty}|{(Version ?? string.Empty).Length}:{Version ?? string.Empty}";
}

public sealed record RegistryLookup(string ContractId, string Version, ContractRegistryEntry Entry);

public sealed record RegistryValidationResult(bool Success, string Message)
{
    public static RegistryValidationResult Pass(string message) => new(true, message);
    public static RegistryValidationResult Fail(string message) => new(false, message);
}

internal readonly record struct ContractRegistryKey(string ContractId, string Version)
{
    public static ContractRegistryKey From(string? contractId, string? version)
        => new(contractId ?? string.Empty, version ?? string.Empty);
}

public sealed record AdmissionBaselineSnapshot(
    IReadOnlyList<ContractRegistryEntry> EffectiveContracts,
    ContractRegistryEntry ApplicationContract,
    ContractRegistryEntry ApplicationBoundary,
    string ContractOwnerRequirement,
    string AuthoritySourceRequirement,
    string ApplicationContractStatusRequirement,
    string ApplicationContractAdmissionStateRequirement,
    string ApplicationBoundaryStatusRequirement,
    string ApplicationBoundaryAdmissionStateRequirement)
{
    public ContractRegistry BuildRegistry()
    {
        if (EffectiveContracts is null)
        {
            throw new InvalidOperationException("baseline effective contracts missing");
        }

        var registry = new ContractRegistry();
        foreach (var entry in EffectiveContracts)
        {
            var result = registry.Register(entry);
            if (!result.Success)
            {
                throw new InvalidOperationException($"baseline registry construction failed: {result.Message}");
            }
        }

        return registry;
    }
}

public interface IAdmissionBaselineProvider
{
    AdmissionBaselineSnapshot GetCurrentBaseline();
}

public sealed class ContractRegistry
{
    private readonly object _sync = new();
    private readonly Dictionary<string, ContractRegistryEntry> _byId = new(StringComparer.Ordinal);
    private readonly Dictionary<ContractRegistryKey, ContractRegistryEntry> _byKey = new();

    public IReadOnlyCollection<ContractRegistryEntry> Entries
    {
        get
        {
            lock (_sync)
            {
                return new ReadOnlyCollection<ContractRegistryEntry>(
                    _byId.Values
                        .OrderBy(entry => entry.ContractId, StringComparer.Ordinal)
                        .ThenBy(entry => entry.Version, StringComparer.Ordinal)
                        .ToArray());
            }
        }
    }

    public static ContractRegistry CreateCanonical(IEnumerable<ContractRegistryEntry>? entries)
    {
        if (entries is null)
        {
            throw new InvalidOperationException("canonical registry entries missing");
        }

        var registry = new ContractRegistry();
        foreach (var entry in entries)
        {
            var result = registry.Register(entry);
            if (!result.Success)
            {
                var identity = entry is null ? "<null>" : $"{entry.ContractId}@{entry.Version}";
                throw new InvalidOperationException($"canonical registry construction failed for {identity}: {result.Message}");
            }
        }

        return registry;
    }

    public static ContractRegistry CreateCanonical() => CreateCanonical(CanonicalEntries);

    public RegistryValidationResult Register(ContractRegistryEntry? entry)
    {
        if (entry is null)
        {
            return RegistryValidationResult.Fail("missing registry entry");
        }

        if (string.IsNullOrWhiteSpace(entry.ContractId)) return RegistryValidationResult.Fail("missing contract identity");
        if (string.IsNullOrWhiteSpace(entry.Version)) return RegistryValidationResult.Fail("missing contract version");
        if (string.IsNullOrWhiteSpace(entry.Owner)) return RegistryValidationResult.Fail("missing owner");
        if (string.IsNullOrWhiteSpace(entry.AuthoritySource)) return RegistryValidationResult.Fail("missing authority source");
        if (string.IsNullOrWhiteSpace(entry.ControlSurface)) return RegistryValidationResult.Fail("missing control surface");
        if (string.IsNullOrWhiteSpace(entry.SchemaOrExecutableRepresentation)) return RegistryValidationResult.Fail("missing schema or executable representation");
        if (string.IsNullOrWhiteSpace(entry.Status) || string.IsNullOrWhiteSpace(entry.AdmissionState)) return RegistryValidationResult.Fail("missing status or admission state");

        var key = ContractRegistryKey.From(entry.ContractId, entry.Version);
        lock (_sync)
        {
            if (_byId.TryGetValue(entry.ContractId, out var existing))
            {
                return string.Equals(existing.Version, entry.Version, StringComparison.Ordinal)
                    ? RegistryValidationResult.Fail("duplicate contract identity")
                    : RegistryValidationResult.Fail("conflicting contract version");
            }

            if (_byKey.ContainsKey(key))
            {
                return RegistryValidationResult.Fail("duplicate contract identity and version");
            }

            _byId.Add(entry.ContractId, entry);
            _byKey.Add(key, entry);
            return RegistryValidationResult.Pass("registered");
        }
    }

    public RegistryLookup? Lookup(string? contractId, string? version)
    {
        if (string.IsNullOrWhiteSpace(contractId) || string.IsNullOrWhiteSpace(version))
        {
            return null;
        }

        lock (_sync)
        {
            return _byKey.TryGetValue(ContractRegistryKey.From(contractId, version), out var entry)
                ? new RegistryLookup(contractId, version, entry)
                : null;
        }
    }

    public RegistryValidationResult ValidateCanonicalCoverage()
    {
        lock (_sync)
        {
            if (_byId.Count != CanonicalEntries.Count) return RegistryValidationResult.Fail("coverage count mismatch");
            foreach (var expected in CanonicalEntries)
            {
                if (!_byId.TryGetValue(expected.ContractId, out var actual)) return RegistryValidationResult.Fail($"missing {expected.ContractId}");
                if (!ValidateCanonicalEntryFields(actual, expected)) return RegistryValidationResult.Fail($"canonical field mismatch for {expected.ContractId}");
            }
            return RegistryValidationResult.Pass("coverage complete");
        }
    }

    public RegistryValidationResult ValidateCanonicalEntry(ContractRegistryEntry? actual)
    {
        if (actual is null) return RegistryValidationResult.Fail("missing registry entry");
        var expected = CanonicalEntries.FirstOrDefault(entry =>
            string.Equals(entry.ContractId, actual.ContractId, StringComparison.Ordinal) &&
            string.Equals(entry.Version, actual.Version, StringComparison.Ordinal));
        return expected is null
            ? RegistryValidationResult.Fail($"unexpected entry {actual.ContractId}@{actual.Version}")
            : ValidateCanonicalEntryFields(actual, expected)
                ? RegistryValidationResult.Pass("canonical entry matches")
                : RegistryValidationResult.Fail($"canonical field mismatch for {actual.ContractId}");
    }

    public RegistryValidationResult ValidateDeterministicLookup()
    {
        foreach (var entry in CanonicalEntries)
        {
            var lookup = Lookup(entry.ContractId, entry.Version);
            if (lookup is null || !string.Equals(lookup.Entry.ContractId, entry.ContractId, StringComparison.Ordinal) || !string.Equals(lookup.Entry.Version, entry.Version, StringComparison.Ordinal))
            {
                return RegistryValidationResult.Fail($"lookup failed for {entry.ContractId}");
            }
        }
        return RegistryValidationResult.Pass("lookup deterministic");
    }

    public RegistryValidationResult ValidateContractIdentityUniqueness()
    {
        lock (_sync)
        {
            return _byId.Count == _byId.Keys.Distinct(StringComparer.Ordinal).Count()
                ? RegistryValidationResult.Pass("unique")
                : RegistryValidationResult.Fail("duplicate identities detected");
        }
    }

    public RegistryValidationResult ValidateVersionUniqueness()
    {
        lock (_sync)
        {
            return _byKey.Count == _byKey.Keys.Distinct().Count()
                ? RegistryValidationResult.Pass("unique")
                : RegistryValidationResult.Fail("duplicate versions detected");
        }
    }

    public RegistryValidationResult ValidateCanonicalIntegrity()
    {
        lock (_sync)
        {
            return CanonicalEntries.All(entry => _byId.ContainsKey(entry.ContractId))
                ? RegistryValidationResult.Pass("canonical registry intact")
                : RegistryValidationResult.Fail("canonical registry incomplete");
        }
    }

    private static bool ValidateCanonicalEntryFields(ContractRegistryEntry actual, ContractRegistryEntry expected)
        => string.Equals(actual.ContractId, expected.ContractId, StringComparison.Ordinal)
           && string.Equals(actual.Version, expected.Version, StringComparison.Ordinal)
           && string.Equals(actual.Owner, expected.Owner, StringComparison.Ordinal)
           && string.Equals(actual.AuthoritySource, expected.AuthoritySource, StringComparison.Ordinal)
           && string.Equals(actual.ControlSurface, expected.ControlSurface, StringComparison.Ordinal)
           && string.Equals(actual.SchemaOrExecutableRepresentation, expected.SchemaOrExecutableRepresentation, StringComparison.Ordinal)
           && string.Equals(actual.Status, expected.Status, StringComparison.Ordinal)
           && string.Equals(actual.AdmissionState, expected.AdmissionState, StringComparison.Ordinal);

    private static IReadOnlyList<ContractRegistryEntry> CanonicalEntries { get; } = new[]
    {
        new ContractRegistryEntry("CON-001", "1.0", "Falcon Core Authority", "CON-000 / CON-001", "src/Foundation.Contracts/Contracts.cs", "executive CoreIdentity type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-002", "1.1", "Falcon Contract Authority", "CON-000 / CON-002", "src/Foundation.Contracts/Contracts.cs", "executive AuthorityRequest and AuthorityResult types and validators", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-003", "1.0", "Falcon Core Authority", "CON-000 / CON-003", "src/Foundation.Contracts/Contracts.cs", "executive LifecycleTransition types and validators", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-004", "1.1", "Falcon Communication Authority", "CON-000 / CON-004", "src/Foundation.Contracts/Contracts.cs", "executive FilEnvelope type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-005", "1.0", "Falcon Communication Authority", "CON-000 / CON-005", "src/Foundation.Contracts/Contracts.cs", "executive FilEvent type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-006", "1.2", "Falcon Self-Awareness Authority", "CON-000 / CON-006", "src/Foundation.Contracts/HealthFitnessContractV12.cs", "executive HealthFitnessAssessmentV12 type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-007", "1.0", "Falcon Core Authority", "CON-000 / CON-007", "src/Foundation.Contracts/Contracts.cs", "executive ConfigurationAdmission type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-008", "1.1", "Falcon Evidence Authority", "CON-000 / CON-008", "src/Foundation.Contracts/Contracts.cs", "executive EvidenceLinkRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-009", "1.0", "Falcon Security Authority", "CON-000 / CON-009", "src/Foundation.Contracts/Contracts.cs", "executive SecurityBoundaryRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-010", "1.1", "Falcon Release Authority", "CON-000 / CON-010", "src/Foundation.Contracts/Contracts.cs", "executive ManifestSurfaceRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-011", "1.0", "Falcon Protection Authority", "CON-000 / CON-011", "src/Foundation.Contracts/Contracts.cs", "executive RestrictionRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-012", "1.0", "Falcon Contract Authority", "CON-000 / CON-012", "src/Foundation.Contracts/Contracts.cs", "executive AuthorityInstrumentRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-013", "1.0", "Falcon Contract Authority", "CON-000 / CON-013", "src/Foundation.Contracts/Contracts.cs", "executive DelegationRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-014", "1.0", "Falcon Contract Authority", "CON-000 / CON-014", "src/Foundation.Contracts/Contracts.cs", "executive IdentifierProviderRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-015", "1.0", "Falcon Contract Authority", "CON-000 / CON-015", "src/Foundation.Contracts/Contracts.cs", "executive TimeProviderRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-016", "1.0", "Falcon Contract Authority", "CON-000 / CON-016", "src/Foundation.Contracts/Contracts.cs", "executive CryptographicProviderRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-017", "1.0", "Falcon Contract Authority", "CON-000 / CON-017", "src/Foundation.Contracts/Contracts.cs", "executive SecretCustodyRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-018", "1.0", "Falcon Contract Authority", "CON-000 / CON-018", "src/Foundation.Contracts/Contracts.cs", "executive CertificateIdentityProviderRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-019", "1.0", "Falcon Contract Authority", "CON-000 / CON-019", "src/Foundation.Contracts/Contracts.cs", "executive RandomnessProviderRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-020", "1.0", "Falcon Contract Authority", "CON-000 / CON-020", "src/Foundation.Contracts/Contracts.cs", "executive BootstrapExecutionContextRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-021", "1.0", "Falcon Contract Authority", "CON-000 / CON-021", "src/Foundation.Contracts/Contracts.cs", "executive BootstrapEvidenceProvenanceRecord type and validator", "ACCEPTED", "REGISTERED"),
        new ContractRegistryEntry("CON-023", "1.1", "Falcon Application Authority", "CON-000 / CON-023", "docs/contracts/CON-023_APPLICATION_CONTRACT_AND_MANIFEST.md", "governed application contract and manifest representation", "ACCEPTED", "REGISTERED")
    };
}
