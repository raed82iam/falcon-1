using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Foundation.Contracts;

public enum PublicRequestResponseContractState
{
    Published = 1,
    Superseded = 2,
    Revoked = 3
}

public sealed record PublicRuntimeRequestResponseProfile(
    string FamilyIdentity,
    string ContractIdentity,
    string ContractVersion,
    string CompatibilityIdentity,
    PublicRequestResponseContractState ContractState,
    string RequestRouteIdentity,
    string ResponseRouteIdentity,
    FilMessageKind RequestMessageKind,
    string RequestMessageType,
    string ResponseMessageType,
    SchemaIdentity RequestSchemaId,
    SchemaIdentity ResponseSchemaId,
    string SchemaVersion,
    ProducerIdentityReference RequestProducer,
    RecipientScopeReference RequestRecipientScope,
    ProducerIdentityReference ResponseProducer,
    RecipientScopeReference ResponseRecipientScope,
    FilMessageClassification Classification,
    AuthorityReference RequestTransportAuthority,
    AuthorityReference ResponseTransportAuthority,
    string AdmissionIdentity,
    int RequestMaxTtlSeconds,
    int ResponseMaxTtlSeconds,
    int MaxDeliveryAttempts,
    bool RetryRequiresSameIdempotencyIdentity,
    string EvidenceReference)
{
    public string ProfileIdentitySha256 => PublicRuntimeRequestResponseTransport.ComputeProfileIdentitySha256(this);
}

public sealed record PublicRuntimeRequestResponseTransportDecision(
    bool Accepted,
    string Reason,
    CanonicalFilEnvelope? Envelope,
    string ProfileIdentitySha256,
    bool RouteAvailable,
    bool RouteActivated,
    bool RouteAuthorized,
    bool ConnectionExecuted,
    bool ExecutionAuthorized,
    bool BusinessAuthorityGranted);

public static class OwnerGovernanceRequestResponseProfiles
{
    private const string Version = "1.0.0";
    private const string RequestAuthority = "authority:transport:owner-command-center-request";
    private const string ResponseAuthority = "authority:transport:owner-governance-response";
    private const string RequestProducer = "shared-web";
    private const string RequestRecipient = "foundation.owner-governance";
    private const string ResponseProducer = "foundation.runtime";
    private const string ResponseRecipient = "shared-web";
    private const int RequestTtlSeconds = 120;
    private const int ResponseTtlSeconds = 120;
    private const int MaxAttempts = 3;

    public static readonly PublicRuntimeRequestResponseProfile StandingOwnerPolicyManagement = Create(
        "foundation:owner-governance:standing-policy-management:v1",
        "foundation/contracts/standing-owner-policy-management-request-response",
        "compat:foundation-owner-policy-management:v1",
        "route:foundation:owner-policy-management:web:v1",
        "route:foundation:owner-policy-management-result:web:v1",
        FilMessageKind.Command,
        "Foundation.Authority.StandingOwnerPolicyManagementRequest",
        "Foundation.Authority.StandingOwnerPolicyManagementDecision",
        "foundation.authority.standing-owner-policy-management.request",
        "foundation.authority.standing-owner-policy-management.decision",
        "admission:foundation:owner-policy-management:web:v1",
        "evidence:fcr-0241:standing-owner-policy-management:v1");

    public static readonly PublicRuntimeRequestResponseProfile StandingOwnerPreapprovalEvaluation = Create(
        "foundation:owner-governance:standing-preapproval-evaluation:v1",
        "foundation/contracts/standing-owner-preapproval-evaluation-request-response",
        "compat:foundation-owner-preapproval-evaluation:v1",
        "route:foundation:owner-preapproval-evaluation:web:v1",
        "route:foundation:owner-preapproval-evaluation-result:web:v1",
        FilMessageKind.Query,
        "Foundation.Authority.WebOwnerPreapprovalProposal",
        "Foundation.Authority.WebOwnerDerivedAutoAcceptDecision",
        "foundation.authority.web-owner-preapproval.proposal",
        "foundation.authority.web-owner-preapproval.decision",
        "admission:foundation:owner-preapproval-evaluation:web:v1",
        "evidence:fcr-0241:standing-owner-preapproval-evaluation:v1");

    public static readonly PublicRuntimeRequestResponseProfile OwnerRollbackOrder = Create(
        "foundation:owner-governance:rollback-order:v1",
        "foundation/contracts/owner-rollback-order-request-response",
        "compat:foundation-owner-rollback-order:v1",
        "route:foundation:owner-rollback-order:web:v1",
        "route:foundation:owner-rollback-order-result:web:v1",
        FilMessageKind.Command,
        "Foundation.Authority.OwnerRollbackOrderRequest",
        "Foundation.Authority.OwnerRollbackOrderDecision",
        "foundation.authority.owner-rollback-order.request",
        "foundation.authority.owner-rollback-order.decision",
        "admission:foundation:owner-rollback-order:web:v1",
        "evidence:fcr-0241:owner-rollback-order:v1");

    public static IReadOnlyList<PublicRuntimeRequestResponseProfile> All { get; } =
        new[] { StandingOwnerPolicyManagement, StandingOwnerPreapprovalEvaluation, OwnerRollbackOrder };

    public static PublicRuntimeRequestResponseProfile? FindByRequestRoute(string routeIdentity) =>
        All.SingleOrDefault(x => StringComparer.Ordinal.Equals(x.RequestRouteIdentity, routeIdentity));

    public static PublicRuntimeRequestResponseProfile? FindByResponseRoute(string routeIdentity) =>
        All.SingleOrDefault(x => StringComparer.Ordinal.Equals(x.ResponseRouteIdentity, routeIdentity));

    internal static bool IsCanonicalPublished(PublicRuntimeRequestResponseProfile profile)
    {
        var identity = profile.ProfileIdentitySha256;
        return All.Any(x =>
            StringComparer.Ordinal.Equals(x.FamilyIdentity, profile.FamilyIdentity) &&
            StringComparer.Ordinal.Equals(x.ProfileIdentitySha256, identity));
    }

    private static PublicRuntimeRequestResponseProfile Create(
        string familyIdentity,
        string contractIdentity,
        string compatibilityIdentity,
        string requestRouteIdentity,
        string responseRouteIdentity,
        FilMessageKind requestKind,
        string requestMessageType,
        string responseMessageType,
        string requestSchemaIdentity,
        string responseSchemaIdentity,
        string admissionIdentity,
        string evidenceReference) =>
        new(
            familyIdentity,
            contractIdentity,
            Version,
            compatibilityIdentity,
            PublicRequestResponseContractState.Published,
            requestRouteIdentity,
            responseRouteIdentity,
            requestKind,
            requestMessageType,
            responseMessageType,
            new SchemaIdentity(requestSchemaIdentity),
            new SchemaIdentity(responseSchemaIdentity),
            Version,
            new ProducerIdentityReference(RequestProducer),
            new RecipientScopeReference(RequestRecipient),
            new ProducerIdentityReference(ResponseProducer),
            new RecipientScopeReference(ResponseRecipient),
            FilMessageClassification.Governance,
            new AuthorityReference(RequestAuthority),
            new AuthorityReference(ResponseAuthority),
            admissionIdentity,
            RequestTtlSeconds,
            ResponseTtlSeconds,
            MaxAttempts,
            true,
            evidenceReference);
}

public static class PublicRuntimeRequestResponseTransport
{
    public static PublicRuntimeRequestResponseTransportDecision BuildRequest(
        PublicRuntimeRequestResponseProfile? profile,
        string? payload,
        MessageIdentity messageId,
        CorrelationIdentity correlationId,
        CausationIdentity? causationId,
        IdempotencyIdentity idempotencyId,
        DeliveryAttemptIdentity deliveryAttemptId,
        RetryLineageIdentity retryLineageId,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt,
        DateTimeOffset? observationTime = null)
    {
        var validation = ValidateProfile(profile);
        if (validation is not null) return Deny(validation, profile);
        var p = profile!;
        var observedAt = ResolveObservationTime(observationTime);
        if (observedAt is null) return Deny("PUBLIC_REQUEST_OBSERVATION_TIME_INVALID", p);
        if (string.IsNullOrWhiteSpace(payload)) return Deny("PUBLIC_REQUEST_PAYLOAD_REQUIRED", p);
        if (!ValidWindow(createdAt, expiresAt, observedAt.Value, p.RequestMaxTtlSeconds))
            return Deny("PUBLIC_REQUEST_FRESHNESS_INVALID", p);

        try
        {
            var envelope = CanonicalFilEnvelope.Create(
                messageId,
                p.RequestMessageKind,
                p.Classification,
                p.RequestMessageType,
                p.RequestSchemaId,
                p.SchemaVersion,
                p.RequestProducer,
                p.RequestRecipientScope,
                correlationId,
                causationId,
                p.RequestTransportAuthority,
                new ProvenanceReference("request-profile:" + DigestToken(p.ProfileIdentitySha256)),
                idempotencyId,
                deliveryAttemptId,
                retryLineageId,
                new CanonicalMessageTime(createdAt, expiresAt),
                CanonicalOutcome.Unknown("transport_request_pending"),
                payload);

            if (!CanonicalMessagingValidator.Validate(envelope).IsValid)
                return Deny("PUBLIC_REQUEST_FIL_VALIDATION_FAILED", p);
            return Accept(envelope, p, "PUBLIC_REQUEST_FIL_ENVELOPE_ACCEPTED");
        }
        catch (ArgumentException)
        {
            return Deny("PUBLIC_REQUEST_FIL_BINDING_REJECTED", p);
        }
    }

    public static PublicRuntimeRequestResponseTransportDecision BuildResponse(
        PublicRuntimeRequestResponseProfile? profile,
        PublicRuntimeRequestResponseTransportDecision? acceptedRequest,
        string? payload,
        MessageIdentity messageId,
        IdempotencyIdentity idempotencyId,
        DeliveryAttemptIdentity deliveryAttemptId,
        RetryLineageIdentity retryLineageId,
        CanonicalOutcome outcome,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt,
        DateTimeOffset? observationTime = null)
    {
        var validation = ValidateProfile(profile);
        if (validation is not null) return Deny(validation, profile);
        var p = profile!;
        var observedAt = ResolveObservationTime(observationTime);
        if (observedAt is null) return Deny("PUBLIC_RESPONSE_OBSERVATION_TIME_INVALID", p);
        if (acceptedRequest is null || !acceptedRequest.Accepted || acceptedRequest.Envelope is null)
            return Deny("PUBLIC_RESPONSE_ACCEPTED_REQUEST_REQUIRED", p);
        if (!StringComparer.Ordinal.Equals(acceptedRequest.ProfileIdentitySha256, p.ProfileIdentitySha256))
            return Deny("PUBLIC_RESPONSE_PROFILE_BINDING_MISMATCH", p);
        if (!RequestEnvelopeMatchesProfile(acceptedRequest.Envelope, p))
            return Deny("PUBLIC_RESPONSE_REQUEST_BINDING_MISMATCH", p);
        if (!RequestStillCurrent(acceptedRequest.Envelope, observedAt.Value, p.RequestMaxTtlSeconds))
            return Deny("PUBLIC_RESPONSE_REQUEST_NO_LONGER_CURRENT", p);
        if (string.IsNullOrWhiteSpace(payload)) return Deny("PUBLIC_RESPONSE_PAYLOAD_REQUIRED", p);
        if (!ValidWindow(createdAt, expiresAt, observedAt.Value, p.ResponseMaxTtlSeconds))
            return Deny("PUBLIC_RESPONSE_FRESHNESS_INVALID", p);
        if (createdAt < acceptedRequest.Envelope.Time.CreatedAt)
            return Deny("PUBLIC_RESPONSE_PRECEDES_REQUEST", p);

        try
        {
            var envelope = CanonicalFilEnvelope.Create(
                messageId,
                FilMessageKind.Response,
                p.Classification,
                p.ResponseMessageType,
                p.ResponseSchemaId,
                p.SchemaVersion,
                p.ResponseProducer,
                p.ResponseRecipientScope,
                acceptedRequest.Envelope.CorrelationId,
                new CausationIdentity(acceptedRequest.Envelope.MessageId.Value),
                p.ResponseTransportAuthority,
                new ProvenanceReference("response-profile:" + DigestToken(p.ProfileIdentitySha256)),
                idempotencyId,
                deliveryAttemptId,
                retryLineageId,
                new CanonicalMessageTime(createdAt, expiresAt),
                outcome,
                payload);

            if (!CanonicalMessagingValidator.Validate(envelope).IsValid)
                return Deny("PUBLIC_RESPONSE_FIL_VALIDATION_FAILED", p);
            return Accept(envelope, p, "PUBLIC_RESPONSE_FIL_ENVELOPE_ACCEPTED");
        }
        catch (ArgumentException)
        {
            return Deny("PUBLIC_RESPONSE_FIL_BINDING_REJECTED", p);
        }
    }

    public static string ComputeProfileIdentitySha256(PublicRuntimeRequestResponseProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        var canonical = new StringBuilder(2048);
        Append(canonical, "family", profile.FamilyIdentity);
        Append(canonical, "contract", profile.ContractIdentity);
        Append(canonical, "contract_version", profile.ContractVersion);
        Append(canonical, "compatibility", profile.CompatibilityIdentity);
        Append(canonical, "contract_state", ((int)profile.ContractState).ToString(CultureInfo.InvariantCulture));
        Append(canonical, "request_route", profile.RequestRouteIdentity);
        Append(canonical, "response_route", profile.ResponseRouteIdentity);
        Append(canonical, "request_kind", ((int)profile.RequestMessageKind).ToString(CultureInfo.InvariantCulture));
        Append(canonical, "request_type", profile.RequestMessageType);
        Append(canonical, "response_type", profile.ResponseMessageType);
        Append(canonical, "request_schema", profile.RequestSchemaId.Value);
        Append(canonical, "response_schema", profile.ResponseSchemaId.Value);
        Append(canonical, "schema_version", profile.SchemaVersion);
        Append(canonical, "request_producer", profile.RequestProducer.Value);
        Append(canonical, "request_recipient", profile.RequestRecipientScope.Value);
        Append(canonical, "response_producer", profile.ResponseProducer.Value);
        Append(canonical, "response_recipient", profile.ResponseRecipientScope.Value);
        Append(canonical, "classification", ((int)profile.Classification).ToString(CultureInfo.InvariantCulture));
        Append(canonical, "request_authority", profile.RequestTransportAuthority.Value);
        Append(canonical, "response_authority", profile.ResponseTransportAuthority.Value);
        Append(canonical, "admission", profile.AdmissionIdentity);
        Append(canonical, "request_ttl_seconds", profile.RequestMaxTtlSeconds.ToString(CultureInfo.InvariantCulture));
        Append(canonical, "response_ttl_seconds", profile.ResponseMaxTtlSeconds.ToString(CultureInfo.InvariantCulture));
        Append(canonical, "max_delivery_attempts", profile.MaxDeliveryAttempts.ToString(CultureInfo.InvariantCulture));
        Append(canonical, "retry_same_idempotency", profile.RetryRequiresSameIdempotencyIdentity ? "true" : "false");
        Append(canonical, "evidence", profile.EvidenceReference);
        return "sha256/" + CanonicalMessagingDigest.ComputePayloadSha256(canonical.ToString());
    }

    private static string? ValidateProfile(PublicRuntimeRequestResponseProfile? profile)
    {
        if (profile is null) return "PUBLIC_REQUEST_RESPONSE_PROFILE_REQUIRED";
        if (profile.RequestSchemaId is null || profile.ResponseSchemaId is null ||
            profile.RequestProducer is null || profile.RequestRecipientScope is null ||
            profile.ResponseProducer is null || profile.ResponseRecipientScope is null ||
            profile.RequestTransportAuthority is null || profile.ResponseTransportAuthority is null)
            return "PUBLIC_REQUEST_RESPONSE_PROFILE_REFERENCE_REQUIRED";
        if (profile.ContractState == PublicRequestResponseContractState.Revoked)
            return "PUBLIC_REQUEST_RESPONSE_CONTRACT_REVOKED";
        if (profile.ContractState == PublicRequestResponseContractState.Superseded)
            return "PUBLIC_REQUEST_RESPONSE_CONTRACT_SUPERSEDED_NO_SILENT_UPGRADE";
        if (profile.ContractState != PublicRequestResponseContractState.Published)
            return "PUBLIC_REQUEST_RESPONSE_CONTRACT_STATE_INVALID";
        if (profile.RequestMessageKind is not (FilMessageKind.Command or FilMessageKind.Query))
            return "PUBLIC_REQUEST_MESSAGE_KIND_INVALID";
        if (profile.Classification != FilMessageClassification.Governance)
            return "PUBLIC_REQUEST_RESPONSE_CLASSIFICATION_INVALID";
        if (profile.RequestMaxTtlSeconds < 1 || profile.ResponseMaxTtlSeconds < 1 || profile.MaxDeliveryAttempts < 1)
            return "PUBLIC_REQUEST_RESPONSE_DELIVERY_POLICY_INVALID";
        if (!profile.RetryRequiresSameIdempotencyIdentity)
            return "PUBLIC_REQUEST_RESPONSE_RETRY_IDEMPOTENCY_REQUIRED";
        if (!Token(profile.FamilyIdentity) || !Token(profile.ContractIdentity) || !Version(profile.ContractVersion) ||
            !Token(profile.CompatibilityIdentity) || !Token(profile.RequestRouteIdentity) || !Token(profile.ResponseRouteIdentity) ||
            !Token(profile.RequestMessageType) || !Token(profile.ResponseMessageType) ||
            !Token(profile.RequestSchemaId.Value) || !Token(profile.ResponseSchemaId.Value) || !Version(profile.SchemaVersion) ||
            !Token(profile.RequestProducer.Value) || !Token(profile.RequestRecipientScope.Value) ||
            !Token(profile.ResponseProducer.Value) || !Token(profile.ResponseRecipientScope.Value) ||
            !Token(profile.RequestTransportAuthority.Value) || !Token(profile.ResponseTransportAuthority.Value) ||
            !Token(profile.AdmissionIdentity) || !Token(profile.EvidenceReference))
            return "PUBLIC_REQUEST_RESPONSE_PROFILE_IDENTITY_INVALID";
        if (!OwnerGovernanceRequestResponseProfiles.IsCanonicalPublished(profile))
            return "PUBLIC_REQUEST_RESPONSE_PROFILE_NOT_FOUND_IN_CANONICAL_REGISTRY";
        return null;
    }

    private static bool RequestEnvelopeMatchesProfile(CanonicalFilEnvelope envelope, PublicRuntimeRequestResponseProfile profile) =>
        envelope.MessageKind == profile.RequestMessageKind &&
        envelope.Classification == profile.Classification &&
        StringComparer.Ordinal.Equals(envelope.MessageType, profile.RequestMessageType) &&
        StringComparer.Ordinal.Equals(envelope.SchemaId.Value, profile.RequestSchemaId.Value) &&
        StringComparer.Ordinal.Equals(envelope.SchemaVersion, profile.SchemaVersion) &&
        StringComparer.Ordinal.Equals(envelope.Producer.Value, profile.RequestProducer.Value) &&
        StringComparer.Ordinal.Equals(envelope.RecipientScope.Value, profile.RequestRecipientScope.Value) &&
        StringComparer.Ordinal.Equals(envelope.Authority.Value, profile.RequestTransportAuthority.Value);

    private static bool RequestStillCurrent(CanonicalFilEnvelope envelope, DateTimeOffset observationTime, int maxTtlSeconds) =>
        envelope.Time.ExpiresAt is { } expiry &&
        ValidWindow(envelope.Time.CreatedAt, expiry, observationTime, maxTtlSeconds);

    private static PublicRuntimeRequestResponseTransportDecision Accept(
        CanonicalFilEnvelope envelope,
        PublicRuntimeRequestResponseProfile profile,
        string reason) =>
        new(true, reason, envelope, profile.ProfileIdentitySha256, true, false, false, false, false, false);

    private static PublicRuntimeRequestResponseTransportDecision Deny(
        string reason,
        PublicRuntimeRequestResponseProfile? profile) =>
        new(false, reason, null, SafeProfileIdentity(profile), false, false, false, false, false, false);

    private static string SafeProfileIdentity(PublicRuntimeRequestResponseProfile? profile)
    {
        if (profile is null) return "NONE";
        try
        {
            return ComputeProfileIdentitySha256(profile);
        }
        catch (Exception exception) when (exception is ArgumentException or NullReferenceException)
        {
            return "INVALID_PROFILE";
        }
    }

    private static DateTimeOffset? ResolveObservationTime(DateTimeOffset? observationTime)
    {
        var value = observationTime ?? DateTimeOffset.UtcNow;
        return value != default && value.Offset == TimeSpan.Zero ? value : null;
    }

    private static bool ValidWindow(
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt,
        DateTimeOffset observationTime,
        int maxTtlSeconds) =>
        createdAt != default && observationTime != default &&
        createdAt.Offset == TimeSpan.Zero && expiresAt.Offset == TimeSpan.Zero && observationTime.Offset == TimeSpan.Zero &&
        expiresAt > createdAt && expiresAt <= createdAt.AddSeconds(maxTtlSeconds) &&
        observationTime >= createdAt && observationTime < expiresAt;

    private static string DigestToken(string sha256) =>
        sha256.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase) ? sha256[7..] : sha256;

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

    private static bool Token(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal)) return false;
        return value.All(ch => !char.IsControl(ch) && !char.IsWhiteSpace(ch));
    }

    private static bool Version(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal)) return false;
        var parts = value.Split('.', StringSplitOptions.None);
        if (parts.Length is < 2 or > 3) return false;
        return parts.All(part => part.Length > 0 && !(part.Length > 1 && part[0] == '0') && part.All(char.IsDigit));
    }
}
