using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.SchemaRegistry;

public enum SchemaCompatibilityClassification
{
    Exact = 1,
    Backward = 2,
    Forward = 3,
    Incompatible = 4
}

public enum SchemaLifecycleState
{
    Registered = 1,
    Active = 2,
    Deprecated = 3,
    Retired = 4
}

public sealed record SchemaOwnerReference
{
    public SchemaOwnerReference(string value)
    {
        Value = SchemaRegistryRules.RequireIdentifier(value, nameof(value));
    }

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed record SchemaDefinition
{
    public SchemaDefinition(
        SchemaIdentity schemaId,
        string version,
        SchemaOwnerReference owner,
        string definitionSha256,
        ProvenanceReference provenance)
    {
        SchemaId = schemaId ?? throw new ArgumentNullException(nameof(schemaId));
        Version = SchemaRegistryRules.RequireVersion(version, nameof(version));
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        DefinitionSha256 = SchemaRegistryRules.RequireSha256(definitionSha256, nameof(definitionSha256));
        Provenance = provenance ?? throw new ArgumentNullException(nameof(provenance));
    }

    public SchemaIdentity SchemaId { get; }
    public string Version { get; }
    public SchemaOwnerReference Owner { get; }
    public string DefinitionSha256 { get; }
    public ProvenanceReference Provenance { get; }
}

public sealed record SchemaRegistryEntry
{
    public SchemaRegistryEntry(
        SchemaDefinition definition,
        SchemaLifecycleState lifecycle)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        Lifecycle = SchemaRegistryRules.RequireDefined(lifecycle, nameof(lifecycle));
    }

    public SchemaDefinition Definition { get; }
    public SchemaLifecycleState Lifecycle { get; }
}

public sealed record SchemaCompatibilityRule
{
    public SchemaCompatibilityRule(
        SchemaIdentity schemaId,
        string fromVersion,
        string toVersion,
        SchemaCompatibilityClassification classification,
        ProvenanceReference provenance)
    {
        SchemaId = schemaId ?? throw new ArgumentNullException(nameof(schemaId));
        FromVersion = SchemaRegistryRules.RequireVersion(fromVersion, nameof(fromVersion));
        ToVersion = SchemaRegistryRules.RequireVersion(toVersion, nameof(toVersion));
        Classification = SchemaRegistryRules.RequireDefined(classification, nameof(classification));
        Provenance = provenance ?? throw new ArgumentNullException(nameof(provenance));

        if (string.Equals(FromVersion, ToVersion, StringComparison.Ordinal) &&
            Classification != SchemaCompatibilityClassification.Exact)
        {
            throw new ArgumentException(
                "same_version_requires_exact_compatibility",
                nameof(classification));
        }

        if (!string.Equals(FromVersion, ToVersion, StringComparison.Ordinal) &&
            Classification == SchemaCompatibilityClassification.Exact)
        {
            throw new ArgumentException(
                "exact_compatibility_requires_same_version",
                nameof(classification));
        }
    }

    public SchemaIdentity SchemaId { get; }
    public string FromVersion { get; }
    public string ToVersion { get; }
    public SchemaCompatibilityClassification Classification { get; }
    public ProvenanceReference Provenance { get; }
}

public sealed record SchemaRegistryOperationResult(
    bool Accepted,
    string Reason);

public sealed record SchemaResolutionResult(
    bool Resolved,
    string Reason,
    SchemaRegistryEntry? Entry);

public sealed record SchemaCompatibilityDecision(
    bool Resolved,
    bool IsCompatible,
    string Reason,
    SchemaCompatibilityClassification? Classification);

public sealed record SchemaRegistrySnapshot
{
    internal SchemaRegistrySnapshot(
        long revision,
        IReadOnlyList<SchemaRegistryEntry> entries,
        IReadOnlyList<SchemaCompatibilityRule> compatibilityRules,
        string sha256)
    {
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision), "revision_must_be_non_negative");
        }

        Revision = revision;

        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(compatibilityRules);

        Entries = Array.AsReadOnly(entries.ToArray());
        CompatibilityRules = Array.AsReadOnly(compatibilityRules.ToArray());
        Sha256 = SchemaRegistryRules.RequireSha256(sha256, nameof(sha256));
    }

    public long Revision { get; }
    public IReadOnlyList<SchemaRegistryEntry> Entries { get; }
    public IReadOnlyList<SchemaCompatibilityRule> CompatibilityRules { get; }
    public string Sha256 { get; }
}

public interface ISchemaRegistry
{
    SchemaRegistryOperationResult Register(SchemaDefinition definition);

    SchemaResolutionResult Resolve(
        SchemaIdentity schemaId,
        string version);

    SchemaRegistryOperationResult TransitionLifecycle(
        SchemaIdentity schemaId,
        string version,
        SchemaLifecycleState targetState);

    SchemaRegistryOperationResult DeclareCompatibility(
        SchemaCompatibilityRule rule);

    SchemaCompatibilityDecision EvaluateCompatibility(
        SchemaIdentity schemaId,
        string fromVersion,
        string toVersion);

    SchemaRegistrySnapshot CaptureSnapshot();
}

public sealed class InMemorySchemaRegistry : ISchemaRegistry
{
    private readonly object sync = new();

    private readonly Dictionary<string, SchemaRegistryEntry> entries =
        new(StringComparer.Ordinal);

    private readonly Dictionary<string, SchemaCompatibilityRule> compatibilityRules =
        new(StringComparer.Ordinal);

    private long revision;

    public InMemorySchemaRegistry()
    {
    }

    public InMemorySchemaRegistry(SchemaRegistrySnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var canonical =
            SchemaRegistryCanonicalization.Canonicalize(
                snapshot.Revision,
                snapshot.Entries,
                snapshot.CompatibilityRules);

        var digest =
            SchemaRegistryCanonicalization.ComputeSha256(canonical);

        if (!string.Equals(
                digest,
                snapshot.Sha256,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "schema_registry_snapshot_digest_mismatch",
                nameof(snapshot));
        }

        foreach (var entry in snapshot.Entries)
        {
            var key =
                SchemaKey(
                    entry.Definition.SchemaId,
                    entry.Definition.Version);

            if (entries.ContainsKey(key))
            {
                throw new ArgumentException(
                    "schema_registry_snapshot_duplicate_schema_version",
                    nameof(snapshot));
            }

            var ownerConflict =
                entries.Values.Any(
                    existing =>
                        string.Equals(
                            existing.Definition.SchemaId.Value,
                            entry.Definition.SchemaId.Value,
                            StringComparison.Ordinal) &&
                        existing.Definition.Owner != entry.Definition.Owner);

            if (ownerConflict)
            {
                throw new ArgumentException(
                    "schema_registry_snapshot_owner_conflict",
                    nameof(snapshot));
            }

            entries.Add(key, entry);
        }

        foreach (var rule in snapshot.CompatibilityRules)
        {
            var fromKey =
                SchemaKey(
                    rule.SchemaId,
                    rule.FromVersion);

            var toKey =
                SchemaKey(
                    rule.SchemaId,
                    rule.ToVersion);

            if (!entries.ContainsKey(fromKey) ||
                !entries.ContainsKey(toKey))
            {
                throw new ArgumentException(
                    "schema_registry_snapshot_compatibility_version_unknown",
                    nameof(snapshot));
            }

            var ruleKey =
                CompatibilityKey(
                    rule.SchemaId,
                    rule.FromVersion,
                    rule.ToVersion);

            if (compatibilityRules.ContainsKey(ruleKey))
            {
                throw new ArgumentException(
                    "schema_registry_snapshot_duplicate_compatibility_rule",
                    nameof(snapshot));
            }

            compatibilityRules.Add(ruleKey, rule);
        }

        revision = snapshot.Revision;
    }

    public SchemaRegistryOperationResult Register(SchemaDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        lock (sync)
        {
            var key = SchemaKey(definition.SchemaId, definition.Version);

            if (entries.TryGetValue(key, out var existing))
            {
                if (existing.Definition == definition)
                {
                    return Reject("duplicate_schema_registration");
                }

                return Reject("conflicting_schema_registration");
            }

            var ownerConflict =
                entries.Values.Any(
                    entry =>
                        string.Equals(
                            entry.Definition.SchemaId.Value,
                            definition.SchemaId.Value,
                            StringComparison.Ordinal) &&
                        entry.Definition.Owner != definition.Owner);

            if (ownerConflict)
            {
                return Reject("schema_owner_conflict");
            }

            entries.Add(
                key,
                new SchemaRegistryEntry(
                    definition,
                    SchemaLifecycleState.Registered));

            revision++;

            return Accept("schema_registered");
        }
    }

    public SchemaResolutionResult Resolve(
        SchemaIdentity schemaId,
        string version)
    {
        ArgumentNullException.ThrowIfNull(schemaId);
        var canonicalVersion = SchemaRegistryRules.RequireVersion(version, nameof(version));

        lock (sync)
        {
            var key = SchemaKey(schemaId, canonicalVersion);

            if (!entries.TryGetValue(key, out var entry))
            {
                return new SchemaResolutionResult(
                    false,
                    "schema_version_unknown",
                    null);
            }

            return new SchemaResolutionResult(
                true,
                "schema_resolved",
                entry);
        }
    }

    public SchemaRegistryOperationResult TransitionLifecycle(
        SchemaIdentity schemaId,
        string version,
        SchemaLifecycleState targetState)
    {
        ArgumentNullException.ThrowIfNull(schemaId);
        var canonicalVersion = SchemaRegistryRules.RequireVersion(version, nameof(version));
        SchemaRegistryRules.RequireDefined(targetState, nameof(targetState));

        lock (sync)
        {
            var key = SchemaKey(schemaId, canonicalVersion);

            if (!entries.TryGetValue(key, out var current))
            {
                return Reject("schema_version_unknown");
            }

            if (current.Lifecycle == targetState)
            {
                return Reject("schema_lifecycle_noop");
            }

            var allowed =
                current.Lifecycle == SchemaLifecycleState.Registered &&
                targetState == SchemaLifecycleState.Active ||
                current.Lifecycle == SchemaLifecycleState.Active &&
                targetState == SchemaLifecycleState.Deprecated ||
                current.Lifecycle == SchemaLifecycleState.Deprecated &&
                targetState == SchemaLifecycleState.Retired;

            if (!allowed)
            {
                return Reject("schema_lifecycle_transition_invalid");
            }

            entries[key] = new SchemaRegistryEntry(
                current.Definition,
                targetState);

            revision++;

            return Accept("schema_lifecycle_transitioned");
        }
    }

    public SchemaRegistryOperationResult DeclareCompatibility(
        SchemaCompatibilityRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        lock (sync)
        {
            var fromKey = SchemaKey(rule.SchemaId, rule.FromVersion);
            var toKey = SchemaKey(rule.SchemaId, rule.ToVersion);

            if (!entries.TryGetValue(fromKey, out var fromEntry) ||
                !entries.TryGetValue(toKey, out var toEntry))
            {
                return Reject("compatibility_schema_version_unknown");
            }

            if (string.Equals(
                    rule.FromVersion,
                    rule.ToVersion,
                    StringComparison.Ordinal))
            {
                if (!string.Equals(
                        fromEntry.Definition.DefinitionSha256,
                        toEntry.Definition.DefinitionSha256,
                        StringComparison.Ordinal))
                {
                    return Reject("same_version_definition_digest_conflict");
                }

                return Reject("exact_compatibility_is_implicit");
            }

            var ruleKey = CompatibilityKey(
                rule.SchemaId,
                rule.FromVersion,
                rule.ToVersion);

            if (compatibilityRules.TryGetValue(ruleKey, out var existing))
            {
                if (existing == rule)
                {
                    return Reject("duplicate_compatibility_rule");
                }

                return Reject("conflicting_compatibility_rule");
            }

            compatibilityRules.Add(ruleKey, rule);
            revision++;

            return Accept("compatibility_rule_registered");
        }
    }

    public SchemaCompatibilityDecision EvaluateCompatibility(
        SchemaIdentity schemaId,
        string fromVersion,
        string toVersion)
    {
        ArgumentNullException.ThrowIfNull(schemaId);

        var canonicalFrom =
            SchemaRegistryRules.RequireVersion(fromVersion, nameof(fromVersion));

        var canonicalTo =
            SchemaRegistryRules.RequireVersion(toVersion, nameof(toVersion));

        lock (sync)
        {
            var fromKey = SchemaKey(schemaId, canonicalFrom);
            var toKey = SchemaKey(schemaId, canonicalTo);

            if (!entries.ContainsKey(fromKey) ||
                !entries.ContainsKey(toKey))
            {
                return new SchemaCompatibilityDecision(
                    false,
                    false,
                    "compatibility_schema_version_unknown",
                    null);
            }

            if (string.Equals(
                    canonicalFrom,
                    canonicalTo,
                    StringComparison.Ordinal))
            {
                return new SchemaCompatibilityDecision(
                    true,
                    true,
                    "exact_schema_version_match",
                    SchemaCompatibilityClassification.Exact);
            }

            var ruleKey =
                CompatibilityKey(
                    schemaId,
                    canonicalFrom,
                    canonicalTo);

            if (!compatibilityRules.TryGetValue(ruleKey, out var rule))
            {
                return new SchemaCompatibilityDecision(
                    false,
                    false,
                    "compatibility_rule_undeclared",
                    null);
            }

            var compatible =
                rule.Classification is
                    SchemaCompatibilityClassification.Backward or
                    SchemaCompatibilityClassification.Forward;

            if (rule.Classification ==
                SchemaCompatibilityClassification.Incompatible)
            {
                compatible = false;
            }

            return new SchemaCompatibilityDecision(
                true,
                compatible,
                compatible
                    ? "schema_versions_compatible"
                    : "schema_versions_incompatible",
                rule.Classification);
        }
    }

    public SchemaRegistrySnapshot CaptureSnapshot()
    {
        lock (sync)
        {
            var orderedEntries =
                entries.Values
                    .OrderBy(
                        entry => entry.Definition.SchemaId.Value,
                        StringComparer.Ordinal)
                    .ThenBy(
                        entry => entry.Definition.Version,
                        SchemaRegistryRules.VersionComparer)
                    .ThenBy(
                        entry => entry.Definition.DefinitionSha256,
                        StringComparer.Ordinal)
                    .ToArray();

            var orderedRules =
                compatibilityRules.Values
                    .OrderBy(
                        rule => rule.SchemaId.Value,
                        StringComparer.Ordinal)
                    .ThenBy(
                        rule => rule.FromVersion,
                        SchemaRegistryRules.VersionComparer)
                    .ThenBy(
                        rule => rule.ToVersion,
                        SchemaRegistryRules.VersionComparer)
                    .ThenBy(
                        rule => (int)rule.Classification)
                    .ToArray();

            var canonical =
                SchemaRegistryCanonicalization.Canonicalize(
                    revision,
                    orderedEntries,
                    orderedRules);

            var digest =
                SchemaRegistryCanonicalization.ComputeSha256(canonical);

            return new SchemaRegistrySnapshot(
                revision,
                Array.AsReadOnly(orderedEntries),
                Array.AsReadOnly(orderedRules),
                digest);
        }
    }

    private static SchemaRegistryOperationResult Accept(string reason) =>
        new(true, reason);

    private static SchemaRegistryOperationResult Reject(string reason) =>
        new(false, reason);

    private static string SchemaKey(
        SchemaIdentity schemaId,
        string version) =>
        string.Concat(
            schemaId.Value,
            "\n",
            version);

    private static string CompatibilityKey(
        SchemaIdentity schemaId,
        string fromVersion,
        string toVersion) =>
        string.Concat(
            schemaId.Value,
            "\n",
            fromVersion,
            "\n",
            toVersion);
}

internal static class SchemaRegistryCanonicalization
{
    internal static string Canonicalize(
        long revision,
        IEnumerable<SchemaRegistryEntry> entries,
        IEnumerable<SchemaCompatibilityRule> rules)
    {
        var builder = new StringBuilder();

        builder.Append("revision=")
            .Append(revision.ToString(CultureInfo.InvariantCulture))
            .Append('\n');

        foreach (var entry in entries)
        {
            builder.Append("schema|")
                .Append(entry.Definition.SchemaId.Value)
                .Append('|')
                .Append(entry.Definition.Version)
                .Append('|')
                .Append(entry.Definition.Owner.Value)
                .Append('|')
                .Append(entry.Definition.DefinitionSha256)
                .Append('|')
                .Append(entry.Definition.Provenance.Value)
                .Append('|')
                .Append(((int)entry.Lifecycle).ToString(CultureInfo.InvariantCulture))
                .Append('\n');
        }

        foreach (var rule in rules)
        {
            builder.Append("compatibility|")
                .Append(rule.SchemaId.Value)
                .Append('|')
                .Append(rule.FromVersion)
                .Append('|')
                .Append(rule.ToVersion)
                .Append('|')
                .Append(((int)rule.Classification).ToString(CultureInfo.InvariantCulture))
                .Append('|')
                .Append(rule.Provenance.Value)
                .Append('\n');
        }

        return builder.ToString();
    }

    internal static string ComputeSha256(string canonical)
    {
        var bytes = Encoding.UTF8.GetBytes(canonical);
        var digest = SHA256.HashData(bytes);
        return Convert.ToHexString(digest);
    }
}

internal static class SchemaRegistryRules
{
    internal static IComparer<string> VersionComparer { get; } =
        Comparer<string>.Create(CompareVersions);

    internal static T RequireDefined<T>(
        T value,
        string parameterName)
        where T : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "enum_value_undefined");
        }

        return value;
    }

    internal static string RequireIdentifier(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "identifier_required",
                parameterName);
        }

        if (!string.Equals(
                value,
                value.Trim(),
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "identifier_not_canonical",
                parameterName);
        }

        if (value.Length is < 3 or > 160)
        {
            throw new ArgumentException(
                "identifier_length_invalid",
                parameterName);
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
                throw new ArgumentException(
                    "identifier_character_invalid",
                    parameterName);
            }
        }

        return value;
    }

    internal static string RequireVersion(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "schema_version_required",
                parameterName);
        }

        if (value.Length > 64)
        {
            throw new ArgumentException(
                "schema_version_length_invalid",
                parameterName);
        }

        if (!string.Equals(
                value,
                value.Trim(),
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "schema_version_not_canonical",
                parameterName);
        }

        var segments =
            value.Split(
                '.',
                StringSplitOptions.None);

        if (segments.Length is < 2 or > 3)
        {
            throw new ArgumentException(
                "schema_version_not_canonical",
                parameterName);
        }

        foreach (var segment in segments)
        {
            if (segment.Length == 0)
            {
                throw new ArgumentException(
                    "schema_version_segment_required",
                    parameterName);
            }

            if (segment.Length > 1 &&
                segment[0] == '0')
            {
                throw new ArgumentException(
                    "schema_version_leading_zero",
                    parameterName);
            }

            foreach (var character in segment)
            {
                if (character is < '0' or > '9')
                {
                    throw new ArgumentException(
                        "schema_version_numeric_segments_required",
                        parameterName);
                }
            }
        }

        return value;
    }

    internal static string RequireSha256(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "sha256_required",
                parameterName);
        }

        if (value.Length != 64)
        {
            throw new ArgumentException(
                "sha256_length_invalid",
                parameterName);
        }

        if (!string.Equals(
                value,
                value.ToUpperInvariant(),
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "sha256_must_be_uppercase",
                parameterName);
        }

        foreach (var character in value)
        {
            var allowed =
                character is >= '0' and <= '9' ||
                character is >= 'A' and <= 'F';

            if (!allowed)
            {
                throw new ArgumentException(
                    "sha256_character_invalid",
                    parameterName);
            }
        }

        return value;
    }

    private static int CompareVersions(
        string? left,
        string? right)
    {
        if (ReferenceEquals(left, right))
        {
            return 0;
        }

        if (left is null)
        {
            return -1;
        }

        if (right is null)
        {
            return 1;
        }

        var leftParts = left.Split('.');
        var rightParts = right.Split('.');
        var count = Math.Max(leftParts.Length, rightParts.Length);

        for (var index = 0; index < count; index++)
        {
            var leftPart =
                index < leftParts.Length
                    ? leftParts[index]
                    : "0";

            var rightPart =
                index < rightParts.Length
                    ? rightParts[index]
                    : "0";

            var lengthComparison =
                leftPart.Length.CompareTo(rightPart.Length);

            if (lengthComparison != 0)
            {
                return lengthComparison;
            }

            var ordinalComparison =
                StringComparer.Ordinal.Compare(
                    leftPart,
                    rightPart);

            if (ordinalComparison != 0)
            {
                return ordinalComparison;
            }
        }

        return StringComparer.Ordinal.Compare(left, right);
    }
}
