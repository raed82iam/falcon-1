using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.ApplicationManifest;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.SchemaRegistry;

namespace Foundation.MessageAdmission;

public enum MessageAdmissionDecision
{
    Admitted = 1,
    Rejected = 2
}

public static class MessageAdmissionPurpose
{
    public const string FilMessageAdmission = "fil-message-admission";
}

public static class MessageAdmissionReason
{
    public const string Admitted = "MESSAGE_ADMITTED";
    public const string InvalidContext = "INVALID_ADMISSION_CONTEXT";
    public const string InvalidEnvelope = "INVALID_CANONICAL_ENVELOPE";
    public const string ProducerBindingMissing = "PRODUCER_BINDING_MISSING";
    public const string ProducerIdentityBindingMismatch = "PRODUCER_IDENTITY_BINDING_MISMATCH";
    public const string ManifestUnknown = "MANIFEST_UNKNOWN";
    public const string ProducerApplicationMismatch = "PRODUCER_APPLICATION_MISMATCH";
    public const string RecipientBindingMissing = "RECIPIENT_BINDING_MISSING";
    public const string RecipientScopeBindingMismatch = "RECIPIENT_SCOPE_BINDING_MISMATCH";
    public const string RecipientConsumerUndeclared = "RECIPIENT_CONSUMER_UNDECLARED";
    public const string CommunicationUndeclared = "COMMUNICATION_UNDECLARED";
    public const string CommunicationAmbiguous = "COMMUNICATION_AMBIGUOUS";
    public const string MessageKindMismatch = "MESSAGE_KIND_MISMATCH";
    public const string ClassificationMismatch = "CLASSIFICATION_MISMATCH";
    public const string ProducerDeclarationInvalid = "PRODUCER_DECLARATION_INVALID";
    public const string SchemaIdentityMismatch = "SCHEMA_IDENTITY_MISMATCH";
    public const string SchemaUnknown = "SCHEMA_UNKNOWN";
    public const string SchemaUnusable = "SCHEMA_UNUSABLE";
    public const string SchemaCompatibilityUnresolved = "SCHEMA_COMPATIBILITY_UNRESOLVED";
    public const string SchemaIncompatible = "SCHEMA_INCOMPATIBLE";
    public const string AuthorityBindingMissing = "AUTHORITY_BINDING_MISSING";
    public const string AuthorityBindingMismatch = "AUTHORITY_BINDING_MISMATCH";
    public const string AuthorityProducerMismatch = "AUTHORITY_PRODUCER_MISMATCH";
    public const string AuthorityApplicationMismatch = "AUTHORITY_APPLICATION_MISMATCH";
    public const string AuthorityRecipientMismatch = "AUTHORITY_RECIPIENT_MISMATCH";
    public const string AuthorityPurposeMismatch = "AUTHORITY_PURPOSE_MISMATCH";
    public const string AuthorityEffectiveScopeMismatch = "AUTHORITY_EFFECTIVE_SCOPE_MISMATCH";
    public const string AuthorityResultMalformed = "AUTHORITY_RESULT_MALFORMED";
    public const string AuthorityDenied = "AUTHORITY_DENIED";
    public const string AuthorityNotYetEffective = "AUTHORITY_NOT_YET_EFFECTIVE";
    public const string AuthorityExpired = "AUTHORITY_EXPIRED";
    public const string MessageExpired = "MESSAGE_EXPIRED";
}

public sealed record MessageProducerBinding
{
    public MessageProducerBinding(
        ProducerIdentityReference producerIdentity,
        ApplicationIdentityReference applicationId,
        ManifestIdentity manifestId,
        ProvenanceReference bindingEvidence)
    {
        ProducerIdentity = producerIdentity ?? throw new ArgumentNullException(nameof(producerIdentity));
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        ManifestId = manifestId ?? throw new ArgumentNullException(nameof(manifestId));
        BindingEvidence = bindingEvidence ?? throw new ArgumentNullException(nameof(bindingEvidence));
    }

    public ProducerIdentityReference ProducerIdentity { get; }
    public ApplicationIdentityReference ApplicationId { get; }
    public ManifestIdentity ManifestId { get; }
    public ProvenanceReference BindingEvidence { get; }
}

public sealed record MessageRecipientBinding
{
    public MessageRecipientBinding(
        RecipientScopeReference recipientScope,
        ManifestReference intendedConsumer,
        ProvenanceReference bindingEvidence)
    {
        RecipientScope = recipientScope ?? throw new ArgumentNullException(nameof(recipientScope));
        IntendedConsumer = intendedConsumer ?? throw new ArgumentNullException(nameof(intendedConsumer));
        BindingEvidence = bindingEvidence ?? throw new ArgumentNullException(nameof(bindingEvidence));
    }

    public RecipientScopeReference RecipientScope { get; }
    public ManifestReference IntendedConsumer { get; }
    public ProvenanceReference BindingEvidence { get; }
}

public sealed record MessageAuthorityBinding
{
    public MessageAuthorityBinding(
        AuthorityReference authorityReference,
        AuthorityResult authorityResult,
        ProducerIdentityReference authorizedProducerIdentity,
        ApplicationIdentityReference authorizedApplicationId,
        RecipientScopeReference authorizedRecipientScope,
        string authorizedPurpose,
        string effectiveScope,
        ProvenanceReference bindingEvidence)
    {
        AuthorityReference = authorityReference ?? throw new ArgumentNullException(nameof(authorityReference));
        AuthorityResult = authorityResult ?? throw new ArgumentNullException(nameof(authorityResult));
        AuthorizedProducerIdentity = authorizedProducerIdentity ?? throw new ArgumentNullException(nameof(authorizedProducerIdentity));
        AuthorizedApplicationId = authorizedApplicationId ?? throw new ArgumentNullException(nameof(authorizedApplicationId));
        AuthorizedRecipientScope = authorizedRecipientScope ?? throw new ArgumentNullException(nameof(authorizedRecipientScope));
        AuthorizedPurpose = RequireCanonicalText(authorizedPurpose, nameof(authorizedPurpose));
        EffectiveScope = RequireCanonicalText(effectiveScope, nameof(effectiveScope));
        BindingEvidence = bindingEvidence ?? throw new ArgumentNullException(nameof(bindingEvidence));
    }

    public AuthorityReference AuthorityReference { get; }
    public AuthorityResult AuthorityResult { get; }
    public ProducerIdentityReference AuthorizedProducerIdentity { get; }
    public ApplicationIdentityReference AuthorizedApplicationId { get; }
    public RecipientScopeReference AuthorizedRecipientScope { get; }
    public string AuthorizedPurpose { get; }
    public string EffectiveScope { get; }
    public ProvenanceReference BindingEvidence { get; }

    private static string RequireCanonicalText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("authority_binding_value_required_and_canonical", parameterName);
        }

        return value;
    }
}

public sealed record MessageAdmissionContext
{
    public MessageAdmissionContext(
        MessageProducerBinding? producerBinding,
        string producerManifestVersion,
        MessageRecipientBinding? recipientBinding,
        DateTimeOffset observationTime,
        MessageAuthorityBinding? authorityBinding,
        ProvenanceReference admissionEvidence)
    {
        if (string.IsNullOrWhiteSpace(producerManifestVersion) ||
            !string.Equals(producerManifestVersion, producerManifestVersion.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("producer_manifest_version_required_and_canonical", nameof(producerManifestVersion));
        }

        if (observationTime == default || observationTime.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("observation_time_must_be_utc", nameof(observationTime));
        }

        ProducerBinding = producerBinding;
        ProducerManifestVersion = producerManifestVersion;
        RecipientBinding = recipientBinding;
        ObservationTime = observationTime;
        AuthorityBinding = authorityBinding;
        AdmissionEvidence = admissionEvidence ?? throw new ArgumentNullException(nameof(admissionEvidence));
    }

    public MessageProducerBinding? ProducerBinding { get; }
    public string ProducerManifestVersion { get; }
    public MessageRecipientBinding? RecipientBinding { get; }
    public DateTimeOffset ObservationTime { get; }
    public MessageAuthorityBinding? AuthorityBinding { get; }
    public ProvenanceReference AdmissionEvidence { get; }
}

public sealed record MessageAdmissionResult
{
    internal MessageAdmissionResult(
        MessageAdmissionDecision decision,
        string reason,
        string decisionId,
        string messageDigest,
        string messageId,
        string producerIdentity,
        string producerApplicationId,
        string manifestId,
        string manifestVersion,
        string recipientScope,
        string intendedConsumer,
        string schemaId,
        string schemaVersion,
        string authorityDecisionId,
        string authorityPurpose,
        string authorityEffectiveScope,
        DateTimeOffset observationTime,
        DateTimeOffset? effectiveExpiry,
        string evidenceReference)
    {
        Decision = decision;
        Reason = reason;
        DecisionId = decisionId;
        MessageDigest = messageDigest;
        MessageId = messageId;
        ProducerIdentity = producerIdentity;
        ProducerApplicationId = producerApplicationId;
        ManifestId = manifestId;
        ManifestVersion = manifestVersion;
        RecipientScope = recipientScope;
        IntendedConsumer = intendedConsumer;
        SchemaId = schemaId;
        SchemaVersion = schemaVersion;
        AuthorityDecisionId = authorityDecisionId;
        AuthorityPurpose = authorityPurpose;
        AuthorityEffectiveScope = authorityEffectiveScope;
        ObservationTime = observationTime;
        EffectiveExpiry = effectiveExpiry;
        EvidenceReference = evidenceReference;
    }

    public MessageAdmissionDecision Decision { get; }
    public string Reason { get; }
    public string DecisionId { get; }
    public string MessageDigest { get; }
    public string MessageId { get; }
    public string ProducerIdentity { get; }
    public string ProducerApplicationId { get; }
    public string ManifestId { get; }
    public string ManifestVersion { get; }
    public string RecipientScope { get; }
    public string IntendedConsumer { get; }
    public string SchemaId { get; }
    public string SchemaVersion { get; }
    public string AuthorityDecisionId { get; }
    public string AuthorityPurpose { get; }
    public string AuthorityEffectiveScope { get; }
    public DateTimeOffset ObservationTime { get; }
    public DateTimeOffset? EffectiveExpiry { get; }
    public string EvidenceReference { get; }

    public bool IsAdmitted => Decision == MessageAdmissionDecision.Admitted;
}

public sealed class FilMessageAdmissionEvaluator
{
    private readonly IApplicationCommunicationManifestRegistry manifestRegistry;
    private readonly ISchemaRegistry schemaRegistry;

    public FilMessageAdmissionEvaluator(
        IApplicationCommunicationManifestRegistry manifestRegistry,
        ISchemaRegistry schemaRegistry)
    {
        this.manifestRegistry = manifestRegistry ?? throw new ArgumentNullException(nameof(manifestRegistry));
        this.schemaRegistry = schemaRegistry ?? throw new ArgumentNullException(nameof(schemaRegistry));
    }

    public MessageAdmissionResult Evaluate(
        CanonicalFilEnvelope? envelope,
        MessageAdmissionContext? context)
    {
        if (context is null)
        {
            return Reject(envelope, null, MessageAdmissionReason.InvalidContext);
        }

        var structural = CanonicalMessagingValidator.Validate(envelope);
        if (!structural.IsValid || envelope is null)
        {
            return Reject(envelope, context, MessageAdmissionReason.InvalidEnvelope);
        }

        var producerBinding = context.ProducerBinding;
        if (producerBinding is null)
        {
            return Reject(envelope, context, MessageAdmissionReason.ProducerBindingMissing);
        }

        if (!StringComparer.Ordinal.Equals(
                producerBinding.ProducerIdentity.Value,
                envelope.Producer.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.ProducerIdentityBindingMismatch);
        }

        var recipientBinding = context.RecipientBinding;
        if (recipientBinding is null)
        {
            return Reject(envelope, context, MessageAdmissionReason.RecipientBindingMissing);
        }

        if (!StringComparer.Ordinal.Equals(
                recipientBinding.RecipientScope.Value,
                envelope.RecipientScope.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.RecipientScopeBindingMismatch);
        }

        ManifestResolutionResult manifestResolution;
        try
        {
            manifestResolution = manifestRegistry.Resolve(
                producerBinding.ManifestId,
                context.ProducerManifestVersion);
        }
        catch (ArgumentException)
        {
            return Reject(envelope, context, MessageAdmissionReason.InvalidContext);
        }

        if (!manifestResolution.Resolved || manifestResolution.Manifest is null)
        {
            return Reject(envelope, context, MessageAdmissionReason.ManifestUnknown);
        }

        var manifest = manifestResolution.Manifest;

        if (!StringComparer.Ordinal.Equals(
                manifest.ApplicationId.Value,
                producerBinding.ApplicationId.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.ProducerApplicationMismatch);
        }

        var intendedConsumerCount = manifest.IntendedConsumers.Count(reference =>
            StringComparer.Ordinal.Equals(
                reference.Value,
                recipientBinding.IntendedConsumer.Value));

        if (intendedConsumerCount != 1)
        {
            return Reject(envelope, context, MessageAdmissionReason.RecipientConsumerUndeclared);
        }

        var declarations = manifest.Communications
            .Where(x => StringComparer.Ordinal.Equals(x.MessageType, envelope.MessageType))
            .ToArray();

        if (declarations.Length == 0)
        {
            return Reject(envelope, context, MessageAdmissionReason.CommunicationUndeclared);
        }

        if (declarations.Length != 1)
        {
            return Reject(envelope, context, MessageAdmissionReason.CommunicationAmbiguous);
        }

        var declaration = declarations[0];

        if (declaration.MessageKind != envelope.MessageKind)
        {
            return Reject(envelope, context, MessageAdmissionReason.MessageKindMismatch);
        }

        if (declaration.Classification != envelope.Classification)
        {
            return Reject(envelope, context, MessageAdmissionReason.ClassificationMismatch);
        }

        if (declaration.Direction != CommunicationDirection.Outbound ||
            declaration.Role != CommunicationRole.Producer)
        {
            return Reject(envelope, context, MessageAdmissionReason.ProducerDeclarationInvalid);
        }

        if (!StringComparer.Ordinal.Equals(
                declaration.Schema.SchemaId.Value,
                envelope.SchemaId.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.SchemaIdentityMismatch);
        }

        var messageSchema = schemaRegistry.Resolve(envelope.SchemaId, envelope.SchemaVersion);
        if (!messageSchema.Resolved || messageSchema.Entry is null)
        {
            return Reject(envelope, context, MessageAdmissionReason.SchemaUnknown);
        }

        if (messageSchema.Entry.Lifecycle == SchemaLifecycleState.Retired)
        {
            return Reject(envelope, context, MessageAdmissionReason.SchemaUnusable);
        }

        var declaredSchema = schemaRegistry.Resolve(
            declaration.Schema.SchemaId,
            declaration.Schema.Version);

        if (!declaredSchema.Resolved || declaredSchema.Entry is null)
        {
            return Reject(envelope, context, MessageAdmissionReason.SchemaUnknown);
        }

        if (declaredSchema.Entry.Lifecycle == SchemaLifecycleState.Retired)
        {
            return Reject(envelope, context, MessageAdmissionReason.SchemaUnusable);
        }

        if (!StringComparer.Ordinal.Equals(
                envelope.SchemaVersion,
                declaration.Schema.Version))
        {
            var compatibility = schemaRegistry.EvaluateCompatibility(
                envelope.SchemaId,
                envelope.SchemaVersion,
                declaration.Schema.Version);

            if (!compatibility.Resolved)
            {
                return Reject(envelope, context, MessageAdmissionReason.SchemaCompatibilityUnresolved);
            }

            if (!compatibility.IsCompatible ||
                compatibility.Classification == SchemaCompatibilityClassification.Incompatible)
            {
                return Reject(envelope, context, MessageAdmissionReason.SchemaIncompatible);
            }
        }

        var authorityBinding = context.AuthorityBinding;
        if (authorityBinding is null)
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityBindingMissing);
        }

        if (!StringComparer.Ordinal.Equals(
                authorityBinding.AuthorityReference.Value,
                envelope.Authority.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityBindingMismatch);
        }

        if (!StringComparer.Ordinal.Equals(
                authorityBinding.AuthorizedProducerIdentity.Value,
                producerBinding.ProducerIdentity.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityProducerMismatch);
        }

        if (!StringComparer.Ordinal.Equals(
                authorityBinding.AuthorizedApplicationId.Value,
                producerBinding.ApplicationId.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityApplicationMismatch);
        }

        if (!StringComparer.Ordinal.Equals(
                authorityBinding.AuthorizedRecipientScope.Value,
                recipientBinding.RecipientScope.Value))
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityRecipientMismatch);
        }

        if (!StringComparer.Ordinal.Equals(
                authorityBinding.AuthorizedPurpose,
                MessageAdmissionPurpose.FilMessageAdmission))
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityPurposeMismatch);
        }

        var authorityResult = authorityBinding.AuthorityResult;
        if (ContractValidators.Validate(authorityResult).Result != ValidationResult.Pass)
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityResultMalformed);
        }

        if (!StringComparer.Ordinal.Equals(
                authorityBinding.EffectiveScope,
                authorityResult.EffectiveScope))
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityEffectiveScopeMismatch);
        }

        if (!StringComparer.Ordinal.Equals(authorityResult.Decision, AuthorityDecision.Allow))
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityDenied);
        }

        if (context.ObservationTime < authorityResult.DecisionTime)
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityNotYetEffective);
        }

        if (context.ObservationTime >= authorityResult.Expiry)
        {
            return Reject(envelope, context, MessageAdmissionReason.AuthorityExpired);
        }

        if (envelope.Time.ExpiresAt is { } expiry && context.ObservationTime >= expiry)
        {
            return Reject(envelope, context, MessageAdmissionReason.MessageExpired);
        }

        return CreateResult(
            envelope,
            context,
            MessageAdmissionDecision.Admitted,
            MessageAdmissionReason.Admitted);
    }

    private static MessageAdmissionResult Reject(
        CanonicalFilEnvelope? envelope,
        MessageAdmissionContext? context,
        string reason) =>
        CreateResult(
            envelope,
            context,
            MessageAdmissionDecision.Rejected,
            reason);

    private static MessageAdmissionResult CreateResult(
        CanonicalFilEnvelope? envelope,
        MessageAdmissionContext? context,
        MessageAdmissionDecision decision,
        string reason)
    {
        var messageDigest = envelope is null
            ? "UNAVAILABLE"
            : CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope);

        var messageId = envelope?.MessageId.Value ?? "UNAVAILABLE";
        var producerIdentity = context?.ProducerBinding?.ProducerIdentity.Value ?? "UNAVAILABLE";
        var applicationId = context?.ProducerBinding?.ApplicationId.Value ?? "UNAVAILABLE";
        var manifestId = context?.ProducerBinding?.ManifestId.Value ?? "UNAVAILABLE";
        var manifestVersion = context?.ProducerManifestVersion ?? "UNAVAILABLE";
        var recipientScope = context?.RecipientBinding?.RecipientScope.Value ?? "UNAVAILABLE";
        var intendedConsumer = context?.RecipientBinding?.IntendedConsumer.Value ?? "UNAVAILABLE";
        var schemaId = envelope?.SchemaId.Value ?? "UNAVAILABLE";
        var schemaVersion = envelope?.SchemaVersion ?? "UNAVAILABLE";
        var authorityDecisionId = context?.AuthorityBinding?.AuthorityResult.DecisionId ?? "UNAVAILABLE";
        var authorityPurpose = context?.AuthorityBinding?.AuthorizedPurpose ?? "UNAVAILABLE";
        var authorityEffectiveScope = context?.AuthorityBinding?.EffectiveScope ?? "UNAVAILABLE";
        var observationTime = context?.ObservationTime ?? DateTimeOffset.UnixEpoch;
        var effectiveExpiry = MinimumExpiry(
            envelope?.Time.ExpiresAt,
            context?.AuthorityBinding?.AuthorityResult.Expiry);
        var evidenceReference = context?.AdmissionEvidence.Value ?? "UNAVAILABLE";

        var decisionId = ComputeDecisionIdentity(
            decision,
            reason,
            messageDigest,
            messageId,
            producerIdentity,
            applicationId,
            manifestId,
            manifestVersion,
            context?.ProducerBinding?.BindingEvidence.Value ?? "UNAVAILABLE",
            recipientScope,
            intendedConsumer,
            context?.RecipientBinding?.BindingEvidence.Value ?? "UNAVAILABLE",
            schemaId,
            schemaVersion,
            authorityDecisionId,
            context?.AuthorityBinding?.AuthorityReference.Value ?? "UNAVAILABLE",
            context?.AuthorityBinding?.AuthorizedProducerIdentity.Value ?? "UNAVAILABLE",
            context?.AuthorityBinding?.AuthorizedApplicationId.Value ?? "UNAVAILABLE",
            context?.AuthorityBinding?.AuthorizedRecipientScope.Value ?? "UNAVAILABLE",
            authorityPurpose,
            authorityEffectiveScope,
            context?.AuthorityBinding?.BindingEvidence.Value ?? "UNAVAILABLE",
            observationTime,
            effectiveExpiry,
            evidenceReference);

        return new MessageAdmissionResult(
            decision,
            reason,
            decisionId,
            messageDigest,
            messageId,
            producerIdentity,
            applicationId,
            manifestId,
            manifestVersion,
            recipientScope,
            intendedConsumer,
            schemaId,
            schemaVersion,
            authorityDecisionId,
            authorityPurpose,
            authorityEffectiveScope,
            observationTime,
            effectiveExpiry,
            evidenceReference);
    }

    private static DateTimeOffset? MinimumExpiry(
        DateTimeOffset? first,
        DateTimeOffset? second)
    {
        if (!first.HasValue)
        {
            return second;
        }

        if (!second.HasValue)
        {
            return first;
        }

        return first.Value <= second.Value ? first : second;
    }

    private static string ComputeDecisionIdentity(
        MessageAdmissionDecision decision,
        string reason,
        string messageDigest,
        string messageId,
        string producerIdentity,
        string applicationId,
        string manifestId,
        string manifestVersion,
        string producerBindingEvidence,
        string recipientScope,
        string intendedConsumer,
        string recipientBindingEvidence,
        string schemaId,
        string schemaVersion,
        string authorityDecisionId,
        string authorityReference,
        string authorityProducerIdentity,
        string authorityApplicationId,
        string authorityRecipientScope,
        string authorityPurpose,
        string authorityEffectiveScope,
        string authorityBindingEvidence,
        DateTimeOffset observationTime,
        DateTimeOffset? effectiveExpiry,
        string admissionEvidence)
    {
        var canonical = string.Join("\n",
            "decision=" + ((int)decision).ToString(CultureInfo.InvariantCulture),
            "reason=" + reason,
            "messageDigest=" + messageDigest,
            "messageId=" + messageId,
            "producerIdentity=" + producerIdentity,
            "producerApplication=" + applicationId,
            "manifestId=" + manifestId,
            "manifestVersion=" + manifestVersion,
            "producerBindingEvidence=" + producerBindingEvidence,
            "recipientScope=" + recipientScope,
            "intendedConsumer=" + intendedConsumer,
            "recipientBindingEvidence=" + recipientBindingEvidence,
            "schemaId=" + schemaId,
            "schemaVersion=" + schemaVersion,
            "authorityDecisionId=" + authorityDecisionId,
            "authorityReference=" + authorityReference,
            "authorityProducerIdentity=" + authorityProducerIdentity,
            "authorityApplicationId=" + authorityApplicationId,
            "authorityRecipientScope=" + authorityRecipientScope,
            "authorityPurpose=" + authorityPurpose,
            "authorityEffectiveScope=" + authorityEffectiveScope,
            "authorityBindingEvidence=" + authorityBindingEvidence,
            "observationTime=" + CanonicalTime(observationTime),
            "effectiveExpiry=" + (effectiveExpiry.HasValue ? CanonicalTime(effectiveExpiry.Value) : "<none>"),
            "admissionEvidence=" + admissionEvidence);

        return "message-admission/sha256/" +
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    private static string CanonicalTime(DateTimeOffset value) =>
        value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
}
