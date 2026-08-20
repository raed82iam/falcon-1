using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts;

namespace Falcon.Stage5.WP01.Verifier;

internal static class Program
{
    private static int Main()
    {
        var failures = new List<string>();
        var scenarios = new (string Name, Action Test)[]
        {
            ("positive_command", PositiveCommand),
            ("positive_query", () => PositiveKind(FilMessageKind.Query)),
            ("positive_response", () => PositiveKind(FilMessageKind.Response)),
            ("positive_event", () => PositiveKind(FilMessageKind.Event)),
            ("positive_notice", () => PositiveKind(FilMessageKind.Notice)),
            ("deterministic_digest", DeterministicDigest),
            ("deterministic_equality", DeterministicEquality),
            ("one_field_mutation_unequal", OneFieldMutationUnequal),
            ("typed_identifier_separation", TypedIdentifierSeparation),
            ("reflection_immutability", ReflectionImmutability),
            ("kind_substitution_changes_identity", KindSubstitutionChangesIdentity),
            ("classification_substitution_changes_identity", ClassificationSubstitutionChangesIdentity),
            ("producer_mutation_detected", ProducerMutationDetected),
            ("recipient_mutation_detected", RecipientMutationDetected),
            ("schema_identity_mutation_detected", SchemaIdentityMutationDetected),
            ("schema_version_mutation_detected", SchemaVersionMutationDetected),
            ("authority_mutation_detected", AuthorityMutationDetected),
            ("provenance_mutation_detected", ProvenanceMutationDetected),
            ("idempotency_mutation_detected", IdempotencyMutationDetected),
            ("delivery_attempt_mutation_detected", DeliveryAttemptMutationDetected),
            ("retry_lineage_mutation_detected", RetryLineageMutationDetected),
            ("payload_mutation_rejected_at_construction", PayloadMutationRejectedAtConstruction),
            ("correlation_causation_rejected_at_construction", CorrelationCausationRejectedAtConstruction),
            ("unknown_remains_unknown", UnknownRemainsUnknown),
            ("invalid_outcome_code_fails_closed", InvalidOutcomeCodeFailsClosed),
            ("blank_outcome_reason_fails_closed", BlankOutcomeReasonFailsClosed),
            ("noncanonical_outcome_reason_fails_closed", NoncanonicalOutcomeReasonFailsClosed),
            ("invalid_kind_fails_closed", InvalidKindFailsClosed),
            ("invalid_classification_fails_closed", InvalidClassificationFailsClosed),
            ("invalid_identifier_fails_closed", InvalidIdentifierFailsClosed),
            ("invalid_message_type_fails_closed", InvalidMessageTypeFailsClosed),
            ("invalid_schema_version_fails_closed", InvalidSchemaVersionFailsClosed),
            ("invalid_sha_length_fails_closed", InvalidShaLengthFailsClosed),
            ("invalid_sha_lowercase_fails_closed", InvalidShaLowercaseFailsClosed),
            ("invalid_sha_character_fails_closed", InvalidShaCharacterFailsClosed),
            ("non_utc_time_fails_closed", NonUtcTimeFailsClosed),
            ("expiry_order_fails_closed", ExpiryOrderFailsClosed),
            ("zero_application_neutrality", ZeroApplicationNeutrality),
            ("two_independent_application_identities", TwoIndependentApplicationIdentities),
            ("legacy_fil_envelope_preserved", LegacyFilEnvelopePreserved)
        };

        foreach (var scenario in scenarios)
        {
            Run(scenario.Name, scenario.Test, failures);
        }

        Console.WriteLine(
            failures.Count == 0
                ? "STAGE 5 WP-01 CANONICAL MESSAGING PRIMITIVES VERIFIER: PASS"
                : "STAGE 5 WP-01 CANONICAL MESSAGING PRIMITIVES VERIFIER: FAIL");

        Console.WriteLine($"Scenarios: {scenarios.Length}");
        Console.WriteLine($"Failures: {failures.Count}");

        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }

        return failures.Count == 0 ? 0 : 1;
    }

    private static void Run(
        string scenario,
        Action action,
        ICollection<string> failures)
    {
        try
        {
            action();
            Console.WriteLine($"PASS {scenario}");
        }
        catch (Exception exception)
        {
            failures.Add($"{scenario}: {exception.GetType().Name}: {exception.Message}");
            Console.WriteLine($"FAIL {scenario}");
        }
    }

    private static CanonicalFilEnvelope CreateEnvelope(
        FilMessageKind kind = FilMessageKind.Command,
        FilMessageClassification classification = FilMessageClassification.Operational,
        string producer = "application.alpha/component.sender",
        string recipient = "application.beta/capability.receiver",
        string schemaId = "schema:falcon.reference.operation",
        string schemaVersion = "1.0",
        string authority = "authority:owner-approved-stage5-wp01",
        string provenance = "evidence:stage5-wp01/reference",
        string idempotency = "idempotency:00000001",
        string attempt = "attempt:00000001",
        string retryLineage = "retry-lineage:00000001",
        CanonicalOutcome? outcome = null)
    {
        return CanonicalFilEnvelope.Create(
            new MessageIdentity("msg:00000001"),
            kind,
            classification,
            "falcon.reference.operation.v1",
            new SchemaIdentity(schemaId),
            schemaVersion,
            new ProducerIdentityReference(producer),
            new RecipientScopeReference(recipient),
            new CorrelationIdentity("correlation:00000001"),
            new CausationIdentity("causation:00000000"),
            new AuthorityReference(authority),
            new ProvenanceReference(provenance),
            new IdempotencyIdentity(idempotency),
            new DeliveryAttemptIdentity(attempt),
            new RetryLineageIdentity(retryLineage),
            new CanonicalMessageTime(
                new DateTimeOffset(2026, 8, 6, 20, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 8, 6, 21, 0, 0, TimeSpan.Zero)),
            outcome ?? CanonicalOutcome.Unknown("processing_not_yet_attempted"),
            "{\"reference\":\"opaque-payload\"}");
    }

    private static void PositiveCommand()
    {
        var envelope = CreateEnvelope();
        Require(CanonicalMessagingValidator.Validate(envelope).IsValid, "command_rejected");
    }

    private static void PositiveKind(FilMessageKind kind)
    {
        var envelope = CreateEnvelope(kind);
        Require(envelope.MessageKind == kind, "kind_not_preserved");
        Require(CanonicalMessagingValidator.Validate(envelope).IsValid, "kind_rejected");
    }

    private static void DeterministicDigest()
    {
        var first = CreateEnvelope();
        var second = CreateEnvelope();

        Require(
            CanonicalMessagingDigest.ComputeEnvelopeSha256(first) ==
            CanonicalMessagingDigest.ComputeEnvelopeSha256(second),
            "equivalent_envelopes_not_deterministic");
    }

    private static void DeterministicEquality()
    {
        Require(CreateEnvelope() == CreateEnvelope(), "equivalent_envelopes_not_equal");
    }

    private static void OneFieldMutationUnequal()
    {
        Require(
            CreateEnvelope() != CreateEnvelope(authority: "authority:alternate-owner"),
            "one_field_mutation_remained_equal");
    }

    private static void TypedIdentifierSeparation()
    {
        object message = new MessageIdentity("identity:shared");
        object schema = new SchemaIdentity("identity:shared");
        Require(!message.Equals(schema), "typed_identities_collapsed");
    }

    private static void ReflectionImmutability()
    {
        var canonicalTypes = new[]
        {
            typeof(CanonicalFilEnvelope),
            typeof(CanonicalOutcome),
            typeof(CanonicalMessageTime),
            typeof(MessageIdentity),
            typeof(ProducerIdentityReference),
            typeof(RecipientScopeReference),
            typeof(CorrelationIdentity),
            typeof(CausationIdentity),
            typeof(SchemaIdentity),
            typeof(AuthorityReference),
            typeof(ProvenanceReference),
            typeof(IdempotencyIdentity),
            typeof(DeliveryAttemptIdentity),
            typeof(RetryLineageIdentity)
        };

        foreach (var type in canonicalTypes)
        {
            var writable = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.SetMethod is { IsPublic: true })
                .Select(property => property.Name)
                .ToArray();

            Require(
                writable.Length == 0,
                $"{type.Name}_public_setters:{string.Join(",", writable)}");

            var mutableExposure = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property =>
                    property.PropertyType.IsArray ||
                    typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                .Select(property => property.Name)
                .ToArray();

            Require(
                mutableExposure.Length == 0,
                $"{type.Name}_mutable_exposure:{string.Join(",", mutableExposure)}");
        }
    }

    private static void KindSubstitutionChangesIdentity() =>
        RequireDigestDifferent(
            CreateEnvelope(FilMessageKind.Command),
            CreateEnvelope(FilMessageKind.Event),
            "message_kind_substitution_not_bound");

    private static void ClassificationSubstitutionChangesIdentity() =>
        RequireDigestDifferent(
            CreateEnvelope(classification: FilMessageClassification.Operational),
            CreateEnvelope(classification: FilMessageClassification.Governance),
            "classification_substitution_not_bound");

    private static void ProducerMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(producer: "application.alpha/component.sender"),
            CreateEnvelope(producer: "application.gamma/component.sender"),
            "producer_mutation_not_detected");

    private static void RecipientMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(recipient: "application.beta/capability.receiver"),
            CreateEnvelope(recipient: "application.delta/capability.receiver"),
            "recipient_mutation_not_detected");

    private static void SchemaIdentityMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(schemaId: "schema:falcon.reference.operation"),
            CreateEnvelope(schemaId: "schema:falcon.reference.alternate"),
            "schema_identity_mutation_not_detected");

    private static void SchemaVersionMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(schemaVersion: "1.0"),
            CreateEnvelope(schemaVersion: "1.1"),
            "schema_version_mutation_not_detected");

    private static void AuthorityMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(authority: "authority:owner-approved-stage5-wp01"),
            CreateEnvelope(authority: "authority:alternate-owner"),
            "authority_mutation_not_detected");

    private static void ProvenanceMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(provenance: "evidence:stage5-wp01/reference"),
            CreateEnvelope(provenance: "evidence:stage5-wp01/alternate"),
            "provenance_mutation_not_detected");

    private static void IdempotencyMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(idempotency: "idempotency:00000001"),
            CreateEnvelope(idempotency: "idempotency:00000002"),
            "idempotency_mutation_not_detected");

    private static void DeliveryAttemptMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(attempt: "attempt:00000001"),
            CreateEnvelope(attempt: "attempt:00000002"),
            "delivery_attempt_mutation_not_detected");

    private static void RetryLineageMutationDetected() =>
        RequireDigestDifferent(
            CreateEnvelope(retryLineage: "retry-lineage:00000001"),
            CreateEnvelope(retryLineage: "retry-lineage:00000002"),
            "retry_lineage_mutation_not_detected");

    private static void PayloadMutationRejectedAtConstruction()
    {
        var valid = CreateEnvelope();

        Expect<ArgumentException>(() =>
            new CanonicalFilEnvelope(
                valid.MessageId,
                valid.MessageKind,
                valid.Classification,
                valid.MessageType,
                valid.SchemaId,
                valid.SchemaVersion,
                valid.Producer,
                valid.RecipientScope,
                valid.CorrelationId,
                valid.CausationId,
                valid.Authority,
                valid.Provenance,
                valid.IdempotencyId,
                valid.DeliveryAttemptId,
                valid.RetryLineageId,
                valid.Time,
                valid.Outcome,
                "{\"reference\":\"tampered\"}",
                valid.PayloadSha256));
    }

    private static void CorrelationCausationRejectedAtConstruction()
    {
        var valid = CreateEnvelope();

        Expect<ArgumentException>(() =>
            new CanonicalFilEnvelope(
                valid.MessageId,
                valid.MessageKind,
                valid.Classification,
                valid.MessageType,
                valid.SchemaId,
                valid.SchemaVersion,
                valid.Producer,
                valid.RecipientScope,
                valid.CorrelationId,
                new CausationIdentity(valid.CorrelationId.Value),
                valid.Authority,
                valid.Provenance,
                valid.IdempotencyId,
                valid.DeliveryAttemptId,
                valid.RetryLineageId,
                valid.Time,
                valid.Outcome,
                valid.Payload,
                valid.PayloadSha256));
    }

    private static void UnknownRemainsUnknown()
    {
        var envelope = CreateEnvelope(
            outcome: CanonicalOutcome.Unknown("evidence_insufficient"));

        Require(
            envelope.Outcome.Code == CanonicalOutcomeCode.Unknown,
            "unknown_silently_converted");
    }

    private static void InvalidOutcomeCodeFailsClosed() =>
        Expect<ArgumentOutOfRangeException>(() =>
            new CanonicalOutcome((CanonicalOutcomeCode)999, "invalid_code"));

    private static void BlankOutcomeReasonFailsClosed() =>
        Expect<ArgumentException>(() =>
            new CanonicalOutcome(CanonicalOutcomeCode.Unknown, " "));

    private static void NoncanonicalOutcomeReasonFailsClosed() =>
        Expect<ArgumentException>(() =>
            new CanonicalOutcome(CanonicalOutcomeCode.Unknown, "Not Canonical"));

    private static void InvalidKindFailsClosed() =>
        Expect<ArgumentOutOfRangeException>(() =>
            CreateEnvelope((FilMessageKind)999));

    private static void InvalidClassificationFailsClosed() =>
        Expect<ArgumentOutOfRangeException>(() =>
            CreateEnvelope(classification: (FilMessageClassification)999));

    private static void InvalidIdentifierFailsClosed() =>
        Expect<ArgumentException>(() =>
            new MessageIdentity(" invalid identity "));

    private static void InvalidMessageTypeFailsClosed()
    {
        foreach (var invalid in new[] { ".operation", "operation.", "falcon..operation", "1falcon.operation" })
        {
            Expect<ArgumentException>(() => CreateEnvelopeWithType(invalid, "1.0"));
        }
    }

    private static void InvalidSchemaVersionFailsClosed()
    {
        foreach (var invalid in new[] { ".1", "1.", "1..0", "01.0", "1.a", "1.2.3.4" })
        {
            Expect<ArgumentException>(() => CreateEnvelopeWithType("falcon.reference.operation.v1", invalid));
        }
    }

    private static void InvalidShaLengthFailsClosed() =>
        Expect<ArgumentException>(() => CreateWithDigest("ABC"));

    private static void InvalidShaLowercaseFailsClosed() =>
        Expect<ArgumentException>(() => CreateWithDigest(new string('a', 64)));

    private static void InvalidShaCharacterFailsClosed() =>
        Expect<ArgumentException>(() => CreateWithDigest(new string('G', 64)));

    private static void NonUtcTimeFailsClosed() =>
        Expect<ArgumentException>(() =>
            new CanonicalMessageTime(
                new DateTimeOffset(2026, 8, 6, 23, 0, 0, TimeSpan.FromHours(3)),
                null));

    private static void ExpiryOrderFailsClosed() =>
        Expect<ArgumentException>(() =>
            new CanonicalMessageTime(
                new DateTimeOffset(2026, 8, 6, 20, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 8, 6, 19, 0, 0, TimeSpan.Zero)));

    private static void ZeroApplicationNeutrality()
    {
        var envelope = CreateEnvelope(
            producer: "foundation/component.reference",
            recipient: "foundation/capability.reference");

        var canonical = CanonicalMessagingCanonicalizer.Canonicalize(envelope);

        Require(!canonical.Contains("FSATS", StringComparison.OrdinalIgnoreCase), "fsats_privilege_found");
        Require(!canonical.Contains("trading", StringComparison.OrdinalIgnoreCase), "trading_semantics_found");
    }

    private static void TwoIndependentApplicationIdentities()
    {
        var first = CreateEnvelope(
            producer: "application.alpha/component.sender",
            recipient: "application.beta/capability.receiver");

        var second = CreateEnvelope(
            producer: "application.gamma/component.sender",
            recipient: "application.delta/capability.receiver");

        RequireDigestDifferent(first, second, "independent_applications_collapsed");
    }

    private static void LegacyFilEnvelopePreserved()
    {
        var type = typeof(FilEnvelope);

        Require(
            type.GetProperty("MessageId") is not null,
            "legacy_message_id_missing");

        Require(
            type.GetProperty("ProtectionProfileId") is not null,
            "legacy_protection_profile_missing");
    }

    private static CanonicalFilEnvelope CreateEnvelopeWithType(
        string messageType,
        string schemaVersion)
    {
        return CanonicalFilEnvelope.Create(
            new MessageIdentity("msg:00000001"),
            FilMessageKind.Command,
            FilMessageClassification.Operational,
            messageType,
            new SchemaIdentity("schema:falcon.reference.operation"),
            schemaVersion,
            new ProducerIdentityReference("application.alpha/component.sender"),
            new RecipientScopeReference("application.beta/capability.receiver"),
            new CorrelationIdentity("correlation:00000001"),
            new CausationIdentity("causation:00000000"),
            new AuthorityReference("authority:owner-approved-stage5-wp01"),
            new ProvenanceReference("evidence:stage5-wp01/reference"),
            new IdempotencyIdentity("idempotency:00000001"),
            new DeliveryAttemptIdentity("attempt:00000001"),
            new RetryLineageIdentity("retry-lineage:00000001"),
            new CanonicalMessageTime(
                new DateTimeOffset(2026, 8, 6, 20, 0, 0, TimeSpan.Zero),
                null),
            CanonicalOutcome.Unknown("processing_not_yet_attempted"),
            "{}");
    }

    private static void CreateWithDigest(string digest)
    {
        var valid = CreateEnvelope();

        _ = new CanonicalFilEnvelope(
            valid.MessageId,
            valid.MessageKind,
            valid.Classification,
            valid.MessageType,
            valid.SchemaId,
            valid.SchemaVersion,
            valid.Producer,
            valid.RecipientScope,
            valid.CorrelationId,
            valid.CausationId,
            valid.Authority,
            valid.Provenance,
            valid.IdempotencyId,
            valid.DeliveryAttemptId,
            valid.RetryLineageId,
            valid.Time,
            valid.Outcome,
            valid.Payload,
            digest);
    }

    private static void RequireDigestDifferent(
        CanonicalFilEnvelope first,
        CanonicalFilEnvelope second,
        string message)
    {
        Require(
            CanonicalMessagingDigest.ComputeEnvelopeSha256(first) !=
            CanonicalMessagingDigest.ComputeEnvelopeSha256(second),
            message);
    }

    private static void Expect<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException($"expected_{typeof(TException).Name}");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
