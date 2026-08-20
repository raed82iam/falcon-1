using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Contracts.ResourceGovernance;

public abstract record CanonicalResourceIdentifier
{
    protected CanonicalResourceIdentifier(string value)
    {
        Value = ResourcePrimitiveValidation.RequireCanonicalIdentifier(value);
    }

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed record ResourceClassId : CanonicalResourceIdentifier
{
    public ResourceClassId(string value) : base(value) { }
}

public sealed record ApplicationPrincipalId : CanonicalResourceIdentifier
{
    public ApplicationPrincipalId(string value) : base(value) { }
}

public sealed record ResourceGrantId : CanonicalResourceIdentifier
{
    public ResourceGrantId(string value) : base(value) { }
}

public sealed record ResourceRequestId : CanonicalResourceIdentifier
{
    public ResourceRequestId(string value) : base(value) { }
}

public sealed record ResourceDecisionId : CanonicalResourceIdentifier
{
    public ResourceDecisionId(string value) : base(value) { }
}

public sealed record ResourceEvidenceId : CanonicalResourceIdentifier
{
    public ResourceEvidenceId(string value) : base(value) { }
}

public sealed record CorrelationId : CanonicalResourceIdentifier
{
    public CorrelationId(string value) : base(value) { }
}

public sealed record CausationId : CanonicalResourceIdentifier
{
    public CausationId(string value) : base(value) { }
}

public sealed record ResourceEpochId : CanonicalResourceIdentifier
{
    public ResourceEpochId(string value) : base(value) { }
}

public sealed record ResourcePriorityClassId : CanonicalResourceIdentifier
{
    public ResourcePriorityClassId(string value) : base(value) { }
}

public sealed record TechnicalCriticalityClassId : CanonicalResourceIdentifier
{
    public TechnicalCriticalityClassId(string value) : base(value) { }
}

public sealed record ResourceScopeId : CanonicalResourceIdentifier
{
    public ResourceScopeId(string value) : base(value) { }
}

public enum ResourcePressureState
{
    Normal = 0,
    Constrained = 1,
    Degraded = 2,
    Critical = 3
}

public enum ResourceDecisionKind
{
    Grant = 0,
    PartialGrant = 1,
    Cap = 2,
    Deny = 3,
    Defer = 4,
    Revoke = 5,
    Reduce = 6,
    Restore = 7
}

public enum ResourceReclaimability
{
    Reclaimable = 0,
    NonReclaimable = 1,
    Temporary = 2
}

public sealed record ResourceQuantity
{
    public ResourceQuantity(decimal amount, string unit)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Resource quantity cannot be negative.");
        }

        Amount = amount;
        Unit = ResourcePrimitiveValidation.RequireCanonicalUnit(unit);
    }

    public decimal Amount { get; }
    public string Unit { get; }

    public string ToCanonicalString() => Amount.ToString("G29", CultureInfo.InvariantCulture) + "|" + Unit;
}

public sealed record ResourceEffectiveLifetime
{
    public ResourceEffectiveLifetime(
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveUntil,
        bool explicitlyOpenEnded)
    {
        if (effectiveUntil.HasValue && explicitlyOpenEnded)
        {
            throw new ArgumentException("A lifetime cannot be both bounded and explicitly open-ended.");
        }

        if (!effectiveUntil.HasValue && !explicitlyOpenEnded)
        {
            throw new ArgumentException("An unbounded lifetime must be explicitly declared open-ended.");
        }

        if (effectiveUntil.HasValue && effectiveUntil.Value < effectiveFrom)
        {
            throw new ArgumentException("Effective end cannot precede effective start.");
        }

        EffectiveFrom = effectiveFrom;
        EffectiveUntil = effectiveUntil;
        ExplicitlyOpenEnded = explicitlyOpenEnded;
    }

    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset? EffectiveUntil { get; }
    public bool ExplicitlyOpenEnded { get; }
}

public sealed record ResourceEvidenceReference
{
    public ResourceEvidenceReference(
        ResourceEvidenceId evidenceId,
        ResourceScopeId scopeId,
        DateTimeOffset observedAt,
        ResourceEpochId epochId)
    {
        EvidenceId = evidenceId ?? throw new ArgumentNullException(nameof(evidenceId));
        ScopeId = scopeId ?? throw new ArgumentNullException(nameof(scopeId));
        ObservedAt = observedAt;
        EpochId = epochId ?? throw new ArgumentNullException(nameof(epochId));
    }

    public ResourceEvidenceId EvidenceId { get; }
    public ResourceScopeId ScopeId { get; }
    public DateTimeOffset ObservedAt { get; }
    public ResourceEpochId EpochId { get; }
}

public sealed record CanonicalIdentityField
{
    public CanonicalIdentityField(string name, string? value)
    {
        Name = ResourcePrimitiveValidation.RequireCanonicalIdentifier(name);
        Value = value;
    }

    public string Name { get; }
    public string? Value { get; }
}

public static class CanonicalResourceIdentity
{
    public static string ComputeSha256(IEnumerable<CanonicalIdentityField> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);

        var ordered = fields.OrderBy(field => field.Name, StringComparer.Ordinal).ToArray();
        if (ordered.Length == 0)
        {
            throw new ArgumentException("At least one canonical identity field is required.", nameof(fields));
        }

        for (var index = 1; index < ordered.Length; index++)
        {
            if (StringComparer.Ordinal.Equals(ordered[index - 1].Name, ordered[index].Name))
            {
                throw new ArgumentException($"Duplicate canonical identity field '{ordered[index].Name}'.", nameof(fields));
            }
        }

        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var field in ordered)
        {
            AppendLengthDelimited(hash, field.Name);
            if (field.Value is null)
            {
                hash.AppendData(new byte[] { 0 });
            }
            else
            {
                hash.AppendData(new byte[] { 1 });
                AppendLengthDelimited(hash, field.Value);
            }
        }

        return Convert.ToHexString(hash.GetHashAndReset());
    }

    public static CanonicalIdentityField QuantityField(string name, ResourceQuantity quantity)
    {
        ArgumentNullException.ThrowIfNull(quantity);
        return new CanonicalIdentityField(name, quantity.ToCanonicalString());
    }

    public static CanonicalIdentityField IdentifierField(string name, CanonicalResourceIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        return new CanonicalIdentityField(name, identifier.Value);
    }

    public static CanonicalIdentityField LifetimeStartField(string name, ResourceEffectiveLifetime lifetime)
    {
        ArgumentNullException.ThrowIfNull(lifetime);
        return new CanonicalIdentityField(name, lifetime.EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
    }

    private static void AppendLengthDelimited(IncrementalHash hash, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, bytes.Length);
        hash.AppendData(length);
        hash.AppendData(bytes);
    }
}

public static class ResourcePrimitiveValidation
{
    public static string RequireCanonicalIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Identifier is required.", nameof(value));
        }

        if (!StringComparer.Ordinal.Equals(value, value.Trim()))
        {
            throw new ArgumentException("Identifier must already be in canonical form and cannot contain leading or trailing whitespace.", nameof(value));
        }

        if (value.Length > 256)
        {
            throw new ArgumentException("Identifier exceeds the maximum length of 256 characters.", nameof(value));
        }

        foreach (var character in value)
        {
            if (char.IsControl(character) || char.IsWhiteSpace(character))
            {
                throw new ArgumentException("Identifier cannot contain whitespace or control characters.", nameof(value));
            }
        }

        return value;
    }

    public static string RequireCanonicalUnit(string unit)
    {
        var value = RequireCanonicalIdentifier(unit);
        if (value.Length > 64)
        {
            throw new ArgumentException("Resource unit exceeds the maximum length of 64 characters.", nameof(unit));
        }

        return value;
    }
}
