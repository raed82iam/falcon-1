using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;
using Foundation.SchemaRegistry;

namespace Foundation.ApplicationManifest;

public enum CommunicationDirection
{
    Inbound = 1,
    Outbound = 2,
    Bidirectional = 3
}

public enum CommunicationRole
{
    Producer = 1,
    Consumer = 2,
    Requester = 3,
    Responder = 4
}

public enum ManifestLifecycleEvent
{
    ApplicationVersionChange = 1,
    Update = 2,
    Replacement = 3,
    Suspension = 4,
    Removal = 5
}

public enum ManifestApplicabilityRule
{
    RemainsApplicable = 1,
    RequiresRevalidation = 2,
    Invalidated = 3
}

public sealed record ManifestLifecycleDeclaration
{
    public ManifestLifecycleDeclaration(ManifestLifecycleEvent lifecycleEvent, ManifestApplicabilityRule applicability)
    {
        LifecycleEvent = ManifestRules.RequireDefined(lifecycleEvent, nameof(lifecycleEvent));
        Applicability = ManifestRules.RequireDefined(applicability, nameof(applicability));
    }

    public ManifestLifecycleEvent LifecycleEvent { get; }
    public ManifestApplicabilityRule Applicability { get; }
}

public sealed record ManifestIdentity
{
    public ManifestIdentity(string value) => Value = ManifestRules.RequireIdentifier(value, nameof(value));
    public string Value { get; }
    public override string ToString() => Value;
}

public sealed record ApplicationIdentityReference
{
    public ApplicationIdentityReference(string value) => Value = ManifestRules.RequireIdentifier(value, nameof(value));
    public string Value { get; }
    public override string ToString() => Value;
}

public sealed record ApplicationOwnerReference
{
    public ApplicationOwnerReference(string value) => Value = ManifestRules.RequireIdentifier(value, nameof(value));
    public string Value { get; }
    public override string ToString() => Value;
}

public sealed record ManifestReference
{
    public ManifestReference(string value) => Value = ManifestRules.RequireIdentifier(value, nameof(value));
    public string Value { get; }
    public override string ToString() => Value;
}

public sealed record ManifestSchemaReference
{
    public ManifestSchemaReference(SchemaIdentity schemaId, string version)
    {
        SchemaId = schemaId ?? throw new ArgumentNullException(nameof(schemaId));
        Version = ManifestRules.RequireVersion(version, nameof(version));
    }

    public SchemaIdentity SchemaId { get; }
    public string Version { get; }
}

public sealed record CommunicationDeclaration
{
    public CommunicationDeclaration(
        string messageType,
        FilMessageKind messageKind,
        FilMessageClassification classification,
        ManifestSchemaReference schema,
        CommunicationDirection direction,
        CommunicationRole role)
    {
        MessageType = ManifestRules.RequireTypeName(messageType, nameof(messageType));
        MessageKind = ManifestRules.RequireDefined(messageKind, nameof(messageKind));
        Classification = ManifestRules.RequireDefined(classification, nameof(classification));
        Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        Direction = ManifestRules.RequireDefined(direction, nameof(direction));
        Role = ManifestRules.RequireDefined(role, nameof(role));

        if (direction == CommunicationDirection.Inbound && role == CommunicationRole.Producer)
        {
            throw new ArgumentException("inbound_cannot_declare_producer_role", nameof(role));
        }

        if (direction == CommunicationDirection.Outbound && role == CommunicationRole.Consumer)
        {
            throw new ArgumentException("outbound_cannot_declare_consumer_role", nameof(role));
        }
    }

    public string MessageType { get; }
    public FilMessageKind MessageKind { get; }
    public FilMessageClassification Classification { get; }
    public ManifestSchemaReference Schema { get; }
    public CommunicationDirection Direction { get; }
    public CommunicationRole Role { get; }
}

public sealed record ApplicationCommunicationManifest
{
    public ApplicationCommunicationManifest(
        ManifestIdentity manifestId,
        string manifestVersion,
        ApplicationIdentityReference applicationId,
        string applicationVersion,
        ApplicationOwnerReference owner,
        IEnumerable<ManifestReference> requiredContracts,
        IEnumerable<ManifestReference> requiredServices,
        IEnumerable<ManifestReference> providedCapabilities,
        IEnumerable<ManifestReference> intendedConsumers,
        IEnumerable<AuthorityReference> authorityRequests,
        IEnumerable<ManifestReference> securityProfiles,
        IEnumerable<ManifestReference> dependencies,
        IEnumerable<ManifestReference> configurationReferences,
        IEnumerable<ProvenanceReference> evidenceReferences,
        IEnumerable<ManifestLifecycleDeclaration> lifecycleApplicability,
        IEnumerable<CommunicationDeclaration> communications)
    {
        ManifestId = manifestId ?? throw new ArgumentNullException(nameof(manifestId));
        ManifestVersion = ManifestRules.RequireVersion(manifestVersion, nameof(manifestVersion));
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ApplicationVersion = ManifestRules.RequireVersion(applicationVersion, nameof(applicationVersion));
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));

        RequiredContracts = ManifestRules.Freeze(requiredContracts, nameof(requiredContracts));
        RequiredServices = ManifestRules.Freeze(requiredServices, nameof(requiredServices));
        ProvidedCapabilities = ManifestRules.Freeze(providedCapabilities, nameof(providedCapabilities));
        IntendedConsumers = ManifestRules.Freeze(intendedConsumers, nameof(intendedConsumers));
        AuthorityRequests = ManifestRules.Freeze(authorityRequests, nameof(authorityRequests));
        SecurityProfiles = ManifestRules.Freeze(securityProfiles, nameof(securityProfiles));
        Dependencies = ManifestRules.Freeze(dependencies, nameof(dependencies));
        ConfigurationReferences = ManifestRules.Freeze(configurationReferences, nameof(configurationReferences));
        EvidenceReferences = ManifestRules.Freeze(evidenceReferences, nameof(evidenceReferences));
        LifecycleApplicability = ManifestRules.Freeze(lifecycleApplicability, nameof(lifecycleApplicability));
        Communications = ManifestRules.Freeze(communications, nameof(communications));

        ManifestRules.RequireUnique(RequiredContracts.Select(x => x.Value), "duplicate_contract_reference");
        ManifestRules.RequireUnique(RequiredServices.Select(x => x.Value), "duplicate_service_reference");
        ManifestRules.RequireUnique(ProvidedCapabilities.Select(x => x.Value), "duplicate_capability_reference");
        ManifestRules.RequireUnique(IntendedConsumers.Select(x => x.Value), "duplicate_consumer_reference");
        ManifestRules.RequireUnique(AuthorityRequests.Select(x => x.Value), "duplicate_authority_request");
        ManifestRules.RequireUnique(SecurityProfiles.Select(x => x.Value), "duplicate_security_profile_reference");
        ManifestRules.RequireUnique(Dependencies.Select(x => x.Value), "duplicate_dependency_reference");
        ManifestRules.RequireUnique(ConfigurationReferences.Select(x => x.Value), "duplicate_configuration_reference");
        ManifestRules.RequireUnique(EvidenceReferences.Select(x => x.Value), "duplicate_evidence_reference");
        ManifestRules.RequireCompleteLifecycleApplicability(LifecycleApplicability);
        ManifestRules.RequireUnique(
            Communications.Select(ManifestCanonicalization.CommunicationKey),
            "duplicate_communication_declaration");
    }

    public ManifestIdentity ManifestId { get; }
    public string ManifestVersion { get; }
    public ApplicationIdentityReference ApplicationId { get; }
    public string ApplicationVersion { get; }
    public ApplicationOwnerReference Owner { get; }
    public IReadOnlyList<ManifestReference> RequiredContracts { get; }
    public IReadOnlyList<ManifestReference> RequiredServices { get; }
    public IReadOnlyList<ManifestReference> ProvidedCapabilities { get; }
    public IReadOnlyList<ManifestReference> IntendedConsumers { get; }
    public IReadOnlyList<AuthorityReference> AuthorityRequests { get; }
    public IReadOnlyList<ManifestReference> SecurityProfiles { get; }
    public IReadOnlyList<ManifestReference> Dependencies { get; }
    public IReadOnlyList<ManifestReference> ConfigurationReferences { get; }
    public IReadOnlyList<ProvenanceReference> EvidenceReferences { get; }
    public IReadOnlyList<ManifestLifecycleDeclaration> LifecycleApplicability { get; }
    public IReadOnlyList<CommunicationDeclaration> Communications { get; }
}

public sealed record ManifestValidationResult(bool IsValid, string Code, string Message)
{
    public static ManifestValidationResult Pass() => new(true, "PASS", "application_manifest_validation_passed");
    public static ManifestValidationResult Fail(string code, string message) => new(false, code, message);
}

public static class ApplicationCommunicationManifestValidator
{
    public static ManifestValidationResult Validate(
        ApplicationCommunicationManifest? manifest,
        ISchemaRegistry schemaRegistry)
    {
        if (manifest is null)
        {
            return ManifestValidationResult.Fail("NULL_MANIFEST", "application_manifest_required");
        }

        ArgumentNullException.ThrowIfNull(schemaRegistry);

        if (manifest.Communications.Count == 0)
        {
            return ManifestValidationResult.Fail("EMPTY_COMMUNICATION_SET", "at_least_one_communication_declaration_required");
        }

        foreach (var declaration in manifest.Communications)
        {
            var resolved = schemaRegistry.Resolve(declaration.Schema.SchemaId, declaration.Schema.Version);
            if (!resolved.Resolved)
            {
                return ManifestValidationResult.Fail("UNRESOLVED_SCHEMA_REFERENCE", "manifest_schema_reference_unresolved");
            }

            if (resolved.Entry is null || resolved.Entry.Lifecycle == SchemaLifecycleState.Retired)
            {
                return ManifestValidationResult.Fail("SCHEMA_REFERENCE_NOT_USABLE", "manifest_schema_reference_not_usable");
            }
        }

        var conflictingMessageTypes = manifest.Communications
            .GroupBy(x => x.MessageType, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (conflictingMessageTypes.Length > 0)
        {
            return ManifestValidationResult.Fail("CONFLICTING_COMMUNICATION_DECLARATION", "manifest_contains_conflicting_communication_declaration");
        }

        return ManifestValidationResult.Pass();
    }
}

public sealed record ManifestRegistrationResult(bool Accepted, string Reason, string? ManifestSha256);

public sealed record ManifestResolutionResult(bool Resolved, string Reason, ApplicationCommunicationManifest? Manifest, string? ManifestSha256);

public interface IApplicationCommunicationManifestRegistry
{
    ManifestRegistrationResult Register(ApplicationCommunicationManifest manifest);
    ManifestResolutionResult Resolve(ManifestIdentity manifestId, string manifestVersion);
    IReadOnlyList<ApplicationCommunicationManifest> CaptureSnapshot();
}

public sealed class InMemoryApplicationCommunicationManifestRegistry : IApplicationCommunicationManifestRegistry
{
    private readonly ISchemaRegistry schemaRegistry;
    private readonly object sync = new();
    private readonly Dictionary<string, (ApplicationCommunicationManifest Manifest, string Digest)> manifests = new(StringComparer.Ordinal);

    public InMemoryApplicationCommunicationManifestRegistry(ISchemaRegistry schemaRegistry)
    {
        this.schemaRegistry = schemaRegistry ?? throw new ArgumentNullException(nameof(schemaRegistry));
    }

    public ManifestRegistrationResult Register(ApplicationCommunicationManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var validation = ApplicationCommunicationManifestValidator.Validate(manifest, schemaRegistry);
        if (!validation.IsValid)
        {
            return new ManifestRegistrationResult(false, validation.Code, null);
        }

        var canonical = ManifestCanonicalization.Canonicalize(manifest);
        var digest = ManifestCanonicalization.ComputeSha256(canonical);
        var key = Key(manifest.ManifestId, manifest.ManifestVersion);

        lock (sync)
        {
            if (manifests.TryGetValue(key, out var existing))
            {
                return string.Equals(existing.Digest, digest, StringComparison.Ordinal)
                    ? new ManifestRegistrationResult(false, "duplicate_manifest_registration", existing.Digest)
                    : new ManifestRegistrationResult(false, "conflicting_manifest_registration", existing.Digest);
            }

            var identityConflict = manifests.Values.Any(existing =>
                string.Equals(existing.Manifest.ManifestId.Value, manifest.ManifestId.Value, StringComparison.Ordinal) &&
                (!string.Equals(existing.Manifest.ApplicationId.Value, manifest.ApplicationId.Value, StringComparison.Ordinal) ||
                 !string.Equals(existing.Manifest.Owner.Value, manifest.Owner.Value, StringComparison.Ordinal)));

            if (identityConflict)
            {
                return new ManifestRegistrationResult(false, "manifest_identity_binding_conflict", null);
            }

            manifests.Add(key, (manifest, digest));
            return new ManifestRegistrationResult(true, "manifest_registered", digest);
        }
    }

    public ManifestResolutionResult Resolve(ManifestIdentity manifestId, string manifestVersion)
    {
        ArgumentNullException.ThrowIfNull(manifestId);
        var version = ManifestRules.RequireVersion(manifestVersion, nameof(manifestVersion));

        lock (sync)
        {
            if (!manifests.TryGetValue(Key(manifestId, version), out var entry))
            {
                return new ManifestResolutionResult(false, "manifest_version_unknown", null, null);
            }

            return new ManifestResolutionResult(true, "manifest_resolved", entry.Manifest, entry.Digest);
        }
    }

    public IReadOnlyList<ApplicationCommunicationManifest> CaptureSnapshot()
    {
        lock (sync)
        {
            return Array.AsReadOnly(
                manifests.Values
                    .Select(x => x.Manifest)
                    .OrderBy(x => x.ManifestId.Value, StringComparer.Ordinal)
                    .ThenBy(x => x.ManifestVersion, StringComparer.Ordinal)
                    .ToArray());
        }
    }

    private static string Key(ManifestIdentity id, string version) => string.Concat(id.Value, "\n", version);
}

public static class ManifestCanonicalization
{
    public static string Canonicalize(ApplicationCommunicationManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        var builder = new StringBuilder(2048);

        Append(builder, "manifest_id", manifest.ManifestId.Value);
        Append(builder, "manifest_version", manifest.ManifestVersion);
        Append(builder, "application_id", manifest.ApplicationId.Value);
        Append(builder, "application_version", manifest.ApplicationVersion);
        Append(builder, "owner", manifest.Owner.Value);

        AppendSorted(builder, "contract", manifest.RequiredContracts.Select(x => x.Value));
        AppendSorted(builder, "service", manifest.RequiredServices.Select(x => x.Value));
        AppendSorted(builder, "capability", manifest.ProvidedCapabilities.Select(x => x.Value));
        AppendSorted(builder, "consumer", manifest.IntendedConsumers.Select(x => x.Value));
        AppendSorted(builder, "authority_request", manifest.AuthorityRequests.Select(x => x.Value));
        AppendSorted(builder, "security_profile", manifest.SecurityProfiles.Select(x => x.Value));
        AppendSorted(builder, "dependency", manifest.Dependencies.Select(x => x.Value));
        AppendSorted(builder, "configuration", manifest.ConfigurationReferences.Select(x => x.Value));
        AppendSorted(builder, "evidence", manifest.EvidenceReferences.Select(x => x.Value));
        AppendSorted(builder, "lifecycle", manifest.LifecycleApplicability.Select(LifecycleKey));
        AppendSorted(builder, "communication", manifest.Communications.Select(CommunicationKey));

        return builder.ToString();
    }

    public static string ComputeSha256(ApplicationCommunicationManifest manifest) =>
        ComputeSha256(Canonicalize(manifest));

    internal static string ComputeSha256(string canonical) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));

    internal static string LifecycleKey(ManifestLifecycleDeclaration declaration) =>
        string.Join("|",
            ((int)declaration.LifecycleEvent).ToString(CultureInfo.InvariantCulture),
            ((int)declaration.Applicability).ToString(CultureInfo.InvariantCulture));

    internal static string CommunicationKey(CommunicationDeclaration declaration) =>
        string.Join("|",
            declaration.MessageType,
            ((int)declaration.MessageKind).ToString(CultureInfo.InvariantCulture),
            ((int)declaration.Classification).ToString(CultureInfo.InvariantCulture),
            declaration.Schema.SchemaId.Value,
            declaration.Schema.Version,
            ((int)declaration.Direction).ToString(CultureInfo.InvariantCulture),
            ((int)declaration.Role).ToString(CultureInfo.InvariantCulture));

    private static void AppendSorted(StringBuilder builder, string name, IEnumerable<string> values)
    {
        foreach (var value in values.OrderBy(x => x, StringComparer.Ordinal))
        {
            Append(builder, name, value);
        }
    }

    private static void Append(StringBuilder builder, string name, string value)
    {
        builder.Append(name.Length.ToString(CultureInfo.InvariantCulture));
        builder.Append(':');
        builder.Append(name);
        builder.Append('=');
        builder.Append(Encoding.UTF8.GetByteCount(value).ToString(CultureInfo.InvariantCulture));
        builder.Append(':');
        builder.Append(value);
        builder.Append('\n');
    }
}

internal static class ManifestRules
{
    internal static string RequireIdentifier(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("identifier_required", parameterName);
        }

        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("identifier_not_canonical", parameterName);
        }

        if (value.Length is < 3 or > 160)
        {
            throw new ArgumentException("identifier_length_invalid", parameterName);
        }

        foreach (var character in value)
        {
            var allowed =
                character is >= 'A' and <= 'Z' ||
                character is >= 'a' and <= 'z' ||
                character is >= '0' and <= '9' ||
                character is '-' or '_' or '.' or ':' or '/' or '@' or '+';

            if (!allowed)
            {
                throw new ArgumentException("identifier_character_invalid", parameterName);
            }
        }

        return value;
    }

    internal static string RequireVersion(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("version_required_or_not_canonical", parameterName);
        }

        var parts = value.Split('.', StringSplitOptions.None);
        if (parts.Length is < 2 or > 3)
        {
            throw new ArgumentException("version_not_canonical", parameterName);
        }

        foreach (var part in parts)
        {
            if (part.Length == 0 || part.Any(c => c < '0' || c > '9') || (part.Length > 1 && part[0] == '0'))
            {
                throw new ArgumentException("version_not_canonical", parameterName);
            }
        }

        return value;
    }

    internal static string RequireTypeName(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("message_type_required_or_not_canonical", parameterName);
        }

        var parts = value.Split('.', StringSplitOptions.None);
        if (parts.Length < 2 || parts.Any(part => string.IsNullOrWhiteSpace(part)))
        {
            throw new ArgumentException("message_type_requires_namespace", parameterName);
        }

        return value;
    }

    internal static T RequireDefined<T>(T value, string parameterName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, "enum_value_not_defined");
        }

        return value;
    }

    internal static IReadOnlyList<T> Freeze<T>(IEnumerable<T> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        var array = values.ToArray();
        if (array.Any(x => x is null))
        {
            throw new ArgumentException("collection_contains_null", parameterName);
        }

        return new ReadOnlyCollection<T>(array);
    }

    internal static void RequireUnique(IEnumerable<string> values, string reason)
    {
        if (values.GroupBy(x => x, StringComparer.Ordinal).Any(group => group.Count() > 1))
        {
            throw new ArgumentException(reason);
        }
    }

    internal static void RequireCompleteLifecycleApplicability(IReadOnlyList<ManifestLifecycleDeclaration> declarations)
    {
        if (declarations.GroupBy(x => x.LifecycleEvent).Any(group => group.Count() > 1))
        {
            throw new ArgumentException("duplicate_lifecycle_applicability_declaration");
        }

        var expected = Enum.GetValues<ManifestLifecycleEvent>();
        if (declarations.Count != expected.Length || expected.Any(expectedEvent => declarations.All(x => x.LifecycleEvent != expectedEvent)))
        {
            throw new ArgumentException("incomplete_lifecycle_applicability_declaration");
        }
    }
}
