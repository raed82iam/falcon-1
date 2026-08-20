using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Falcon.FSATS.Primitives;

public abstract class CanonicalId : IEquatable<CanonicalId>
{
    protected CanonicalId(string value)
    {
        Value = CanonicalRules.RequireIdentifier(value, nameof(value));
    }

    public string Value { get; }

    public bool Equals(CanonicalId? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        string.Equals(Value, other.Value, StringComparison.Ordinal);

    public sealed override bool Equals(object? obj) => obj is CanonicalId other && Equals(other);

    public sealed override int GetHashCode() => HashCode.Combine(GetType(), StringComparer.Ordinal.GetHashCode(Value));

    public sealed override string ToString() => Value;

    public static bool operator ==(CanonicalId? left, CanonicalId? right) => Equals(left, right);

    public static bool operator !=(CanonicalId? left, CanonicalId? right) => !Equals(left, right);
}

public sealed class FsatsApplicationId : CanonicalId
{
    public FsatsApplicationId(string value) : base(value) { }
}

public sealed class AwarenessRoomId : CanonicalId
{
    public AwarenessRoomId(string value) : base(value) { }
}

public sealed class ContractFamilyId : CanonicalId
{
    public ContractFamilyId(string value) : base(value) { }
}

public sealed class CorrelationId : CanonicalId
{
    public CorrelationId(string value) : base(value) { }
}

public sealed class CausationId : CanonicalId
{
    public CausationId(string value) : base(value) { }
}

public sealed class EvidenceId : CanonicalId
{
    public EvidenceId(string value) : base(value) { }
}

public sealed class OperationId : CanonicalId
{
    public OperationId(string value) : base(value) { }
}

public sealed class FoundationReferenceId : CanonicalId
{
    public FoundationReferenceId(string value) : base(value) { }
}

public sealed class SchemaReferenceId : CanonicalId
{
    public SchemaReferenceId(string value) : base(value) { }
}

public sealed class PermissionReferenceId : CanonicalId
{
    public PermissionReferenceId(string value) : base(value) { }
}

public sealed class ProvenanceId : CanonicalId
{
    public ProvenanceId(string value) : base(value) { }
}

public sealed record VersionId
{
    public VersionId(string value)
    {
        Value = CanonicalRules.RequireVersion(value, nameof(value));
    }

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed record UtcInstant
{
    public UtcInstant(DateTimeOffset value)
    {
        if (value.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("utc_offset_required", nameof(value));
        }

        Value = value;
    }

    public DateTimeOffset Value { get; }

    public static UtcInstant FromUtc(DateTimeOffset value) => new(value.ToUniversalTime());

    public override string ToString() => Value.ToString("O", CultureInfo.InvariantCulture);
}

public sealed record Deadline
{
    public Deadline(UtcInstant expiresAt)
    {
        ExpiresAt = expiresAt ?? throw new ArgumentNullException(nameof(expiresAt));
    }

    public UtcInstant ExpiresAt { get; }

    public bool IsExpired(UtcInstant asOf)
    {
        ArgumentNullException.ThrowIfNull(asOf);
        return asOf.Value >= ExpiresAt.Value;
    }

    public TimeSpan Remaining(UtcInstant asOf)
    {
        ArgumentNullException.ThrowIfNull(asOf);
        var remaining = ExpiresAt.Value - asOf.Value;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
}

public enum HealthDisposition
{
    Healthy = 1,
    Degraded = 2,
    Restricted = 3,
    Unavailable = 4
}

public sealed record HealthSnapshot
{
    public HealthSnapshot(
        HealthDisposition disposition,
        UtcInstant observedAt,
        string reasonCode,
        EvidenceId evidenceId)
    {
        if (!Enum.IsDefined(disposition))
        {
            throw new ArgumentOutOfRangeException(nameof(disposition));
        }

        Disposition = disposition;
        ObservedAt = observedAt ?? throw new ArgumentNullException(nameof(observedAt));
        ReasonCode = CanonicalRules.RequireReasonCode(reasonCode, nameof(reasonCode));
        EvidenceId = evidenceId ?? throw new ArgumentNullException(nameof(evidenceId));
    }

    public HealthDisposition Disposition { get; }
    public UtcInstant ObservedAt { get; }
    public string ReasonCode { get; }
    public EvidenceId EvidenceId { get; }
}

public sealed record EvidenceLink
{
    public EvidenceLink(
        EvidenceId evidenceId,
        CorrelationId correlationId,
        CausationId? causationId,
        UtcInstant recordedAt)
    {
        EvidenceId = evidenceId ?? throw new ArgumentNullException(nameof(evidenceId));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        CausationId = causationId;
        RecordedAt = recordedAt ?? throw new ArgumentNullException(nameof(recordedAt));
    }

    public EvidenceId EvidenceId { get; }
    public CorrelationId CorrelationId { get; }
    public CausationId? CausationId { get; }
    public UtcInstant RecordedAt { get; }
}

public sealed record FoundationBindingReference
{
    public FoundationBindingReference(
        FoundationReferenceId referenceId,
        VersionId version,
        ProvenanceId provenanceId)
    {
        ReferenceId = referenceId ?? throw new ArgumentNullException(nameof(referenceId));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        ProvenanceId = provenanceId ?? throw new ArgumentNullException(nameof(provenanceId));
    }

    public FoundationReferenceId ReferenceId { get; }
    public VersionId Version { get; }
    public ProvenanceId ProvenanceId { get; }
}

public static class CanonicalEncoding
{
    public static string Encode(params (string Name, string Value)[] fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        var normalized = fields
            .Select(field => (
                Name: CanonicalRules.RequireFieldName(field.Name, nameof(fields)),
                Value: field.Value ?? throw new ArgumentException("field_value_required", nameof(fields))))
            .ToArray();

        if (normalized.Select(x => x.Name).Distinct(StringComparer.Ordinal).Count() != normalized.Length)
        {
            throw new ArgumentException("duplicate_field_name", nameof(fields));
        }

        var builder = new StringBuilder();
        foreach (var (name, value) in normalized)
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

        return builder.ToString();
    }

    public static string ComputeSha256(params (string Name, string Value)[] fields)
    {
        var canonical = Encode(fields);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}

internal static class CanonicalRules
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
                throw new ArgumentException("identifier_contains_invalid_character", parameterName);
            }
        }

        return value;
    }

    internal static string RequireVersion(string value, string parameterName)
    {
        var version = RequireIdentifier(value, parameterName);
        if (!version.Any(char.IsDigit))
        {
            throw new ArgumentException("version_requires_numeric_component", parameterName);
        }

        return version;
    }

    internal static string RequireReasonCode(string value, string parameterName)
    {
        var code = RequireIdentifier(value, parameterName);
        return code.ToUpperInvariant() == code
            ? code
            : throw new ArgumentException("reason_code_must_be_uppercase", parameterName);
    }

    internal static string RequireFieldName(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("field_name_required", parameterName);
        }

        if (value.Length is > 80)
        {
            throw new ArgumentException("field_name_length_invalid", parameterName);
        }

        foreach (var character in value)
        {
            var allowed =
                character is >= 'a' and <= 'z' ||
                character is >= '0' and <= '9' ||
                character is '_' or '-';

            if (!allowed)
            {
                throw new ArgumentException("field_name_not_canonical", parameterName);
            }
        }

        return value;
    }
}
