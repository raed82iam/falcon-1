using System;
using System.Globalization;
using System.Text;

namespace Foundation.Contracts;

public enum PublicProjectionArtifactState
{
    Published = 1,
    Superseded = 2,
    Revoked = 3
}

public sealed record PublicRuntimeProjectionRoute(
    string RouteIdentity,
    string MessageType,
    SchemaIdentity SchemaId,
    string SchemaVersion,
    ProducerIdentityReference Producer,
    RecipientScopeReference RecipientScope,
    FilMessageKind MessageKind,
    FilMessageClassification Classification,
    AuthorityReference TransportAuthority,
    ProvenanceReference Provenance,
    string ArtifactId,
    string ArtifactVersion,
    string ArtifactSha256,
    string EvidenceReference,
    string CompatibilityIdentity,
    PublicProjectionArtifactState ArtifactState);

public sealed record PublicRuntimeProjectionBinding(
    string BindingIdentity,
    string RouteIdentity,
    string ArtifactId,
    string ArtifactVersion,
    string ArtifactSha256,
    string EvidenceReference,
    string CompatibilityIdentity,
    string SourceProvenance,
    string PayloadSha256);

public sealed record PublicRuntimeProjectionTransportDecision(
    bool Accepted,
    string Reason,
    CanonicalFilEnvelope? Envelope,
    PublicRuntimeProjectionBinding? Binding,
    bool ActivationAuthorized,
    bool ExecutionAuthorized,
    bool BusinessAuthorityGranted);

public static class PublicRuntimeProjectionTransport
{
    public static PublicRuntimeProjectionTransportDecision Build(
        PublicRuntimeProjectionRoute? route,
        string? payload,
        MessageIdentity messageId,
        CorrelationIdentity correlationId,
        CausationIdentity? causationId,
        IdempotencyIdentity idempotencyId,
        DeliveryAttemptIdentity deliveryAttemptId,
        RetryLineageIdentity retryLineageId,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        if (route is null ||
            !Token(route.RouteIdentity) ||
            !Token(route.ArtifactId) ||
            !Version(route.ArtifactVersion) ||
            !Sha256(route.ArtifactSha256) ||
            !Token(route.EvidenceReference) ||
            !Token(route.CompatibilityIdentity) ||
            !Enum.IsDefined(route.ArtifactState) ||
            route.SchemaId is null ||
            route.Producer is null ||
            route.RecipientScope is null ||
            route.TransportAuthority is null ||
            route.Provenance is null ||
            string.IsNullOrWhiteSpace(payload) ||
            messageId is null ||
            correlationId is null ||
            idempotencyId is null ||
            deliveryAttemptId is null ||
            retryLineageId is null ||
            createdAt == default ||
            expiresAt <= createdAt)
        {
            return Deny("INVALID_PUBLIC_RUNTIME_PROJECTION_ROUTE");
        }

        if (route.ArtifactState == PublicProjectionArtifactState.Revoked)
        {
            return Deny("PUBLIC_RUNTIME_PROJECTION_ARTIFACT_REVOKED");
        }

        if (route.ArtifactState == PublicProjectionArtifactState.Superseded)
        {
            return Deny("PUBLIC_RUNTIME_PROJECTION_ARTIFACT_SUPERSEDED_NO_SILENT_UPGRADE");
        }

        if (route.MessageKind is FilMessageKind.Command or FilMessageKind.Query)
        {
            return Deny("PUBLIC_RUNTIME_PROJECTION_CONTROL_MESSAGE_FORBIDDEN");
        }

        if (route.MessageKind is not (FilMessageKind.Event or FilMessageKind.Response or FilMessageKind.Notice))
        {
            return Deny("PUBLIC_RUNTIME_PROJECTION_MESSAGE_KIND_INVALID");
        }

        var binding = BuildBinding(route, payload);

        CanonicalFilEnvelope envelope;
        try
        {
            envelope = CanonicalFilEnvelope.Create(
                messageId,
                route.MessageKind,
                route.Classification,
                route.MessageType,
                route.SchemaId,
                route.SchemaVersion,
                route.Producer,
                route.RecipientScope,
                correlationId,
                causationId,
                route.TransportAuthority,
                new ProvenanceReference("projection-binding:" + binding.BindingIdentity),
                idempotencyId,
                deliveryAttemptId,
                retryLineageId,
                new CanonicalMessageTime(createdAt, expiresAt),
                CanonicalOutcome.Succeeded("authoritative_public_runtime_projection"),
                payload);
        }
        catch (ArgumentException)
        {
            return Deny("PUBLIC_RUNTIME_PROJECTION_FIL_BINDING_REJECTED");
        }

        var validation = CanonicalMessagingValidator.Validate(envelope);
        if (!validation.IsValid)
        {
            return Deny("PUBLIC_RUNTIME_PROJECTION_FIL_VALIDATION_FAILED");
        }

        return new PublicRuntimeProjectionTransportDecision(
            true,
            "PUBLIC_RUNTIME_PROJECTION_FIL_ENVELOPE_ACCEPTED",
            envelope,
            binding,
            false,
            false,
            false);
    }

    private static PublicRuntimeProjectionBinding BuildBinding(
        PublicRuntimeProjectionRoute route,
        string payload)
    {
        var payloadSha256 = CanonicalMessagingDigest.ComputePayloadSha256(payload);
        var canonical = new StringBuilder(1024);

        Append(canonical, "route_identity", route.RouteIdentity);
        Append(canonical, "message_type", route.MessageType);
        Append(canonical, "schema_id", route.SchemaId.Value);
        Append(canonical, "schema_version", route.SchemaVersion);
        Append(canonical, "producer", route.Producer.Value);
        Append(canonical, "recipient_scope", route.RecipientScope.Value);
        Append(canonical, "message_kind", ((int)route.MessageKind).ToString(CultureInfo.InvariantCulture));
        Append(canonical, "classification", ((int)route.Classification).ToString(CultureInfo.InvariantCulture));
        Append(canonical, "transport_authority", route.TransportAuthority.Value);
        Append(canonical, "source_provenance", route.Provenance.Value);
        Append(canonical, "artifact_id", route.ArtifactId);
        Append(canonical, "artifact_version", route.ArtifactVersion);
        Append(canonical, "artifact_sha256", route.ArtifactSha256.ToUpperInvariant());
        Append(canonical, "evidence_reference", route.EvidenceReference);
        Append(canonical, "compatibility_identity", route.CompatibilityIdentity);
        Append(canonical, "artifact_state", ((int)route.ArtifactState).ToString(CultureInfo.InvariantCulture));
        Append(canonical, "payload_sha256", payloadSha256);

        var bindingIdentity = "sha256/" + CanonicalMessagingDigest.ComputePayloadSha256(canonical.ToString());

        return new PublicRuntimeProjectionBinding(
            bindingIdentity,
            route.RouteIdentity,
            route.ArtifactId,
            route.ArtifactVersion,
            route.ArtifactSha256.ToUpperInvariant(),
            route.EvidenceReference,
            route.CompatibilityIdentity,
            route.Provenance.Value,
            payloadSha256);
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

    private static PublicRuntimeProjectionTransportDecision Deny(string reason) =>
        new(false, reason, null, null, false, false, false);

    private static bool Sha256(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !value.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase) ||
            value.Length != 71)
        {
            return false;
        }

        for (var i = 7; i < value.Length; i++)
        {
            if (!Uri.IsHexDigit(value[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool Version(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        var parts = value.Split('.', StringSplitOptions.None);
        if (parts.Length is < 2 or > 3)
        {
            return false;
        }

        foreach (var part in parts)
        {
            if (part.Length == 0 || (part.Length > 1 && part[0] == '0'))
            {
                return false;
            }

            foreach (var ch in part)
            {
                if (!char.IsDigit(ch))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool Token(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        foreach (var ch in value)
        {
            if (char.IsControl(ch) || char.IsWhiteSpace(ch))
            {
                return false;
            }
        }

        return true;
    }
}
