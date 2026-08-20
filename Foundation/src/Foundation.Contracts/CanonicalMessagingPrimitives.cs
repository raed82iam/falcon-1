using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Contracts;

public enum FilMessageKind
{
    Command = 1,
    Query = 2,
    Response = 3,
    Event = 4,
    Notice = 5
}

public enum FilMessageClassification
{
    Operational = 1,
    Governance = 2,
    Evidence = 3,
    Health = 4,
    Security = 5,
    Administrative = 6
}

public enum CanonicalOutcomeCode
{
    Unknown = 0,
    Succeeded = 1,
    Failed = 2,
    Rejected = 3
}

public abstract record CanonicalMessagingIdentifier
{
    protected CanonicalMessagingIdentifier(string value)
    {
        Value = CanonicalMessagingRules.RequireIdentifier(value);
    }

    public string Value { get; }

    public sealed override string ToString() => Value;
}

public sealed record MessageIdentity : CanonicalMessagingIdentifier
{
    public MessageIdentity(string value) : base(value) { }
}

public sealed record ProducerIdentityReference : CanonicalMessagingIdentifier
{
    public ProducerIdentityReference(string value) : base(value) { }
}

public sealed record RecipientScopeReference : CanonicalMessagingIdentifier
{
    public RecipientScopeReference(string value) : base(value) { }
}

public sealed record CorrelationIdentity : CanonicalMessagingIdentifier
{
    public CorrelationIdentity(string value) : base(value) { }
}

public sealed record CausationIdentity : CanonicalMessagingIdentifier
{
    public CausationIdentity(string value) : base(value) { }
}

public sealed record SchemaIdentity : CanonicalMessagingIdentifier
{
    public SchemaIdentity(string value) : base(value) { }
}

public sealed record AuthorityReference : CanonicalMessagingIdentifier
{
    public AuthorityReference(string value) : base(value) { }
}

public sealed record ProvenanceReference : CanonicalMessagingIdentifier
{
    public ProvenanceReference(string value) : base(value) { }
}

public sealed record IdempotencyIdentity : CanonicalMessagingIdentifier
{
    public IdempotencyIdentity(string value) : base(value) { }
}

public sealed record DeliveryAttemptIdentity : CanonicalMessagingIdentifier
{
    public DeliveryAttemptIdentity(string value) : base(value) { }
}

public sealed record RetryLineageIdentity : CanonicalMessagingIdentifier
{
    public RetryLineageIdentity(string value) : base(value) { }
}

public sealed record CanonicalOutcome
{
    public CanonicalOutcome(
        CanonicalOutcomeCode code,
        string reason)
    {
        Code = CanonicalMessagingRules.RequireDefined(code, nameof(code));
        Reason = CanonicalMessagingRules.RequireOutcomeReason(reason);
    }

    public CanonicalOutcomeCode Code { get; }

    public string Reason { get; }

    public static CanonicalOutcome Unknown(string reason) =>
        new(CanonicalOutcomeCode.Unknown, reason);

    public static CanonicalOutcome Succeeded(string reason) =>
        new(CanonicalOutcomeCode.Succeeded, reason);

    public static CanonicalOutcome Failed(string reason) =>
        new(CanonicalOutcomeCode.Failed, reason);

    public static CanonicalOutcome Rejected(string reason) =>
        new(CanonicalOutcomeCode.Rejected, reason);
}

public sealed record CanonicalMessageTime
{
    public CanonicalMessageTime(
        DateTimeOffset createdAt,
        DateTimeOffset? expiresAt)
    {
        if (createdAt == default)
        {
            throw new ArgumentException("created_at_required", nameof(createdAt));
        }

        if (createdAt.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("created_at_must_be_utc", nameof(createdAt));
        }

        if (expiresAt is { } expiry)
        {
            if (expiry.Offset != TimeSpan.Zero)
            {
                throw new ArgumentException("expiry_must_be_utc", nameof(expiresAt));
            }

            if (expiry <= createdAt)
            {
                throw new ArgumentException("expiry_must_follow_creation", nameof(expiresAt));
            }
        }

        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? ExpiresAt { get; }
}

public sealed record CanonicalFilEnvelope
{
    public CanonicalFilEnvelope(
        MessageIdentity messageId,
        FilMessageKind messageKind,
        FilMessageClassification classification,
        string messageType,
        SchemaIdentity schemaId,
        string schemaVersion,
        ProducerIdentityReference producer,
        RecipientScopeReference recipientScope,
        CorrelationIdentity correlationId,
        CausationIdentity? causationId,
        AuthorityReference authority,
        ProvenanceReference provenance,
        IdempotencyIdentity idempotencyId,
        DeliveryAttemptIdentity deliveryAttemptId,
        RetryLineageIdentity retryLineageId,
        CanonicalMessageTime time,
        CanonicalOutcome outcome,
        string payload,
        string payloadSha256)
    {
        MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
        MessageKind = CanonicalMessagingRules.RequireDefined(messageKind, nameof(messageKind));
        Classification = CanonicalMessagingRules.RequireDefined(classification, nameof(classification));
        MessageType = CanonicalMessagingRules.RequireTypeName(messageType);
        SchemaId = schemaId ?? throw new ArgumentNullException(nameof(schemaId));
        SchemaVersion = CanonicalMessagingRules.RequireVersion(schemaVersion);
        Producer = producer ?? throw new ArgumentNullException(nameof(producer));
        RecipientScope = recipientScope ?? throw new ArgumentNullException(nameof(recipientScope));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        CausationId = causationId;
        Authority = authority ?? throw new ArgumentNullException(nameof(authority));
        Provenance = provenance ?? throw new ArgumentNullException(nameof(provenance));
        IdempotencyId = idempotencyId ?? throw new ArgumentNullException(nameof(idempotencyId));
        DeliveryAttemptId = deliveryAttemptId ?? throw new ArgumentNullException(nameof(deliveryAttemptId));
        RetryLineageId = retryLineageId ?? throw new ArgumentNullException(nameof(retryLineageId));
        Time = time ?? throw new ArgumentNullException(nameof(time));
        Outcome = outcome ?? throw new ArgumentNullException(nameof(outcome));
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        PayloadSha256 = CanonicalMessagingRules.RequireSha256(payloadSha256, nameof(payloadSha256));

        if (CausationId is not null &&
            string.Equals(
                CorrelationId.Value,
                CausationId.Value,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "correlation_and_causation_must_remain_distinct",
                nameof(causationId));
        }

        var expectedPayloadDigest =
            CanonicalMessagingDigest.ComputePayloadSha256(Payload);

        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(expectedPayloadDigest),
                Convert.FromHexString(PayloadSha256)))
        {
            throw new ArgumentException(
                "payload_digest_does_not_match_payload",
                nameof(payloadSha256));
        }
    }

    public MessageIdentity MessageId { get; }
    public FilMessageKind MessageKind { get; }
    public FilMessageClassification Classification { get; }
    public string MessageType { get; }
    public SchemaIdentity SchemaId { get; }
    public string SchemaVersion { get; }
    public ProducerIdentityReference Producer { get; }
    public RecipientScopeReference RecipientScope { get; }
    public CorrelationIdentity CorrelationId { get; }
    public CausationIdentity? CausationId { get; }
    public AuthorityReference Authority { get; }
    public ProvenanceReference Provenance { get; }
    public IdempotencyIdentity IdempotencyId { get; }
    public DeliveryAttemptIdentity DeliveryAttemptId { get; }
    public RetryLineageIdentity RetryLineageId { get; }
    public CanonicalMessageTime Time { get; }
    public CanonicalOutcome Outcome { get; }
    public string Payload { get; }
    public string PayloadSha256 { get; }

    public static CanonicalFilEnvelope Create(
        MessageIdentity messageId,
        FilMessageKind messageKind,
        FilMessageClassification classification,
        string messageType,
        SchemaIdentity schemaId,
        string schemaVersion,
        ProducerIdentityReference producer,
        RecipientScopeReference recipientScope,
        CorrelationIdentity correlationId,
        CausationIdentity? causationId,
        AuthorityReference authority,
        ProvenanceReference provenance,
        IdempotencyIdentity idempotencyId,
        DeliveryAttemptIdentity deliveryAttemptId,
        RetryLineageIdentity retryLineageId,
        CanonicalMessageTime time,
        CanonicalOutcome outcome,
        string payload)
    {
        return new CanonicalFilEnvelope(
            messageId,
            messageKind,
            classification,
            messageType,
            schemaId,
            schemaVersion,
            producer,
            recipientScope,
            correlationId,
            causationId,
            authority,
            provenance,
            idempotencyId,
            deliveryAttemptId,
            retryLineageId,
            time,
            outcome,
            payload,
            CanonicalMessagingDigest.ComputePayloadSha256(payload));
    }
}

public sealed record CanonicalMessagingValidationResult(
    bool IsValid,
    string Code,
    string Message)
{
    public static CanonicalMessagingValidationResult Pass() =>
        new(true, "PASS", "canonical_messaging_validation_passed");

    public static CanonicalMessagingValidationResult Fail(string code, string message) =>
        new(false, code, message);
}

public static class CanonicalMessagingValidator
{
    public static CanonicalMessagingValidationResult Validate(CanonicalFilEnvelope? envelope)
    {
        if (envelope is null)
        {
            return CanonicalMessagingValidationResult.Fail(
                "NULL_ENVELOPE",
                "canonical_envelope_required");
        }

        if (!Enum.IsDefined(envelope.MessageKind))
        {
            return CanonicalMessagingValidationResult.Fail(
                "INVALID_MESSAGE_KIND",
                "message_kind_not_defined");
        }

        if (!Enum.IsDefined(envelope.Classification))
        {
            return CanonicalMessagingValidationResult.Fail(
                "INVALID_CLASSIFICATION",
                "message_classification_not_defined");
        }

        if (!Enum.IsDefined(envelope.Outcome.Code))
        {
            return CanonicalMessagingValidationResult.Fail(
                "INVALID_OUTCOME",
                "outcome_code_not_defined");
        }

        if (string.IsNullOrWhiteSpace(envelope.Outcome.Reason))
        {
            return CanonicalMessagingValidationResult.Fail(
                "MISSING_OUTCOME_REASON",
                "outcome_reason_required");
        }

        if (envelope.CausationId is not null &&
            string.Equals(
                envelope.CorrelationId.Value,
                envelope.CausationId.Value,
                StringComparison.Ordinal))
        {
            return CanonicalMessagingValidationResult.Fail(
                "CORRELATION_CAUSATION_COLLISION",
                "correlation_and_causation_must_remain_distinct");
        }

        var expectedPayloadDigest =
            CanonicalMessagingDigest.ComputePayloadSha256(envelope.Payload);

        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(expectedPayloadDigest),
                Convert.FromHexString(envelope.PayloadSha256)))
        {
            return CanonicalMessagingValidationResult.Fail(
                "PAYLOAD_DIGEST_MISMATCH",
                "payload_digest_does_not_match_payload");
        }

        return CanonicalMessagingValidationResult.Pass();
    }
}

public static class CanonicalMessagingDigest
{
    public static string ComputePayloadSha256(string payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));
    }

    public static string ComputeEnvelopeSha256(CanonicalFilEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        var canonical = CanonicalMessagingCanonicalizer.Canonicalize(envelope);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }
}

public static class CanonicalMessagingCanonicalizer
{
    public static string Canonicalize(CanonicalFilEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        var validation = CanonicalMessagingValidator.Validate(envelope);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"canonicalization_rejected:{validation.Code}"));
        }

        var builder = new StringBuilder(1024);

        Append(builder, "message_id", envelope.MessageId.Value);
        Append(builder, "message_kind", ((int)envelope.MessageKind).ToString(CultureInfo.InvariantCulture));
        Append(builder, "classification", ((int)envelope.Classification).ToString(CultureInfo.InvariantCulture));
        Append(builder, "message_type", envelope.MessageType);
        Append(builder, "schema_id", envelope.SchemaId.Value);
        Append(builder, "schema_version", envelope.SchemaVersion);
        Append(builder, "producer", envelope.Producer.Value);
        Append(builder, "recipient_scope", envelope.RecipientScope.Value);
        Append(builder, "correlation_id", envelope.CorrelationId.Value);
        Append(builder, "causation_id", envelope.CausationId?.Value ?? string.Empty);
        Append(builder, "authority", envelope.Authority.Value);
        Append(builder, "provenance", envelope.Provenance.Value);
        Append(builder, "idempotency_id", envelope.IdempotencyId.Value);
        Append(builder, "delivery_attempt_id", envelope.DeliveryAttemptId.Value);
        Append(builder, "retry_lineage_id", envelope.RetryLineageId.Value);
        Append(builder, "created_at", envelope.Time.CreatedAt.ToString("O", CultureInfo.InvariantCulture));
        Append(builder, "expires_at", envelope.Time.ExpiresAt?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty);
        Append(builder, "outcome_code", ((int)envelope.Outcome.Code).ToString(CultureInfo.InvariantCulture));
        Append(builder, "outcome_reason", envelope.Outcome.Reason);
        Append(builder, "payload_sha256", envelope.PayloadSha256);
        Append(builder, "payload", envelope.Payload);

        return builder.ToString();
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

internal static class CanonicalMessagingRules
{
    internal static string RequireIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("identifier_required", nameof(value));
        }

        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("identifier_not_canonical", nameof(value));
        }

        if (value.Length is < 3 or > 160)
        {
            throw new ArgumentException("identifier_length_invalid", nameof(value));
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
                throw new ArgumentException("identifier_character_invalid", nameof(value));
            }
        }

        return value;
    }

    internal static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("text_required", parameterName);
        }

        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("text_not_canonical", parameterName);
        }

        return value;
    }

    internal static string RequireTypeName(string value)
    {
        var canonical = RequireText(value, nameof(value));
        var segments = canonical.Split('.', StringSplitOptions.None);

        if (segments.Length < 2)
        {
            throw new ArgumentException(
                "message_type_requires_namespace",
                nameof(value));
        }

        foreach (var segment in segments)
        {
            RequireNameSegment(segment, nameof(value));
        }

        return canonical;
    }

    internal static string RequireVersion(string value)
    {
        var canonical = RequireText(value, nameof(value));
        var segments = canonical.Split('.', StringSplitOptions.None);

        if (segments.Length is < 2 or > 3)
        {
            throw new ArgumentException(
                "schema_version_not_canonical",
                nameof(value));
        }

        foreach (var segment in segments)
        {
            if (segment.Length == 0)
            {
                throw new ArgumentException(
                    "schema_version_segment_required",
                    nameof(value));
            }

            if (segment.Length > 1 && segment[0] == '0')
            {
                throw new ArgumentException(
                    "schema_version_leading_zero",
                    nameof(value));
            }

            foreach (var character in segment)
            {
                if (character is < '0' or > '9')
                {
                    throw new ArgumentException(
                        "schema_version_numeric_segments_required",
                        nameof(value));
                }
            }
        }

        return canonical;
    }

    internal static string RequireOutcomeReason(string value)
    {
        var canonical = RequireText(value, nameof(value));

        foreach (var character in canonical)
        {
            var allowed =
                character is >= 'a' and <= 'z' ||
                character is >= '0' and <= '9' ||
                character is '_' or '-' or '.' or ':' or '/';

            if (!allowed)
            {
                throw new ArgumentException(
                    "outcome_reason_not_canonical",
                    nameof(value));
            }
        }

        return canonical;
    }

    private static void RequireNameSegment(
        string segment,
        string parameterName)
    {
        if (segment.Length == 0)
        {
            throw new ArgumentException(
                "message_type_segment_required",
                parameterName);
        }

        var first = segment[0];
        if (first is not (>= 'a' and <= 'z') &&
            first is not (>= 'A' and <= 'Z'))
        {
            throw new ArgumentException(
                "message_type_segment_must_start_with_letter",
                parameterName);
        }

        foreach (var character in segment)
        {
            var allowed =
                character is >= 'A' and <= 'Z' ||
                character is >= 'a' and <= 'z' ||
                character is >= '0' and <= '9' ||
                character is '-' or '_';

            if (!allowed)
            {
                throw new ArgumentException(
                    "message_type_segment_character_invalid",
                    parameterName);
            }
        }
    }

    internal static string RequireSha256(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 64)
        {
            throw new ArgumentException("sha256_length_invalid", parameterName);
        }

        foreach (var character in value)
        {
            var isHex =
                character is >= '0' and <= '9' ||
                character is >= 'A' and <= 'F';

            if (!isHex)
            {
                throw new ArgumentException("sha256_must_be_uppercase_hex", parameterName);
            }
        }

        return value;
    }

    internal static TEnum RequireDefined<TEnum>(TEnum value, string parameterName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "enum_value_not_defined");
        }

        return value;
    }
}
