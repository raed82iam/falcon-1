using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.ApplicationManifest;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.MessageAdmission;
using Foundation.SchemaRegistry;

namespace Falcon.Stage5.WP04.Verifier;

internal static class Program
{
    private const string MessageType = "falcon.reference.operation.v1";
    private const string SchemaId = "schema:falcon.reference.operation";
    private const string AuthorityRef = "authority:message-admission/reference";
    private const string ProducerId = "application-neutral-producer/reference";
    private const string RecipientScope = "application-neutral-recipient/reference";
    private const string ConsumerRef = "consumer:reference";
    private const string EffectiveScope = "scope:message-admission/reference";
    private static readonly string DigestA = new('A', 64);
    private static readonly string DigestB = new('B', 64);
    private static readonly DateTimeOffset Observation =
        new(2026, 8, 7, 18, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        var scenarios = new (string Name, Action Test)[]
        {
            ("exact_schema_valid_manifest_authority_admitted", ExactSchemaAdmitted),
            ("compatible_schema_version_admitted", CompatibleSchemaAdmitted),
            ("two_independent_applications_admit_independently", TwoApplicationsAdmitIndependently),
            ("null_envelope_rejected", NullEnvelopeRejected),
            ("null_context_rejected", NullContextRejected),
            ("missing_producer_binding_rejected", MissingProducerBindingRejected),
            ("producer_identity_binding_mismatch_rejected", ProducerIdentityBindingMismatchRejected),
            ("unknown_manifest_rejected", UnknownManifestRejected),
            ("wrong_producer_application_binding_rejected", WrongProducerApplicationRejected),
            ("missing_recipient_binding_rejected", MissingRecipientBindingRejected),
            ("recipient_scope_binding_mismatch_rejected", RecipientScopeBindingMismatchRejected),
            ("undeclared_intended_consumer_rejected", UndeclaredIntendedConsumerRejected),
            ("undeclared_message_type_rejected", UndeclaredMessageTypeRejected),
            ("conflicting_communication_predecessor_fails_closed", ConflictingCommunicationPredecessorFailsClosed),
            ("message_kind_mismatch_rejected", MessageKindMismatchRejected),
            ("classification_mismatch_rejected", ClassificationMismatchRejected),
            ("inbound_consumer_cannot_become_producer", InboundConsumerRejected),
            ("schema_identity_mismatch_rejected", SchemaIdentityMismatchRejected),
            ("unknown_schema_version_rejected", UnknownSchemaVersionRejected),
            ("retired_message_schema_rejected", RetiredMessageSchemaRejected),
            ("retired_manifest_schema_rejected", RetiredManifestSchemaRejected),
            ("incompatible_schema_rejected", IncompatibleSchemaRejected),
            ("undeclared_schema_compatibility_rejected", UndeclaredCompatibilityRejected),
            ("missing_authority_binding_rejected", MissingAuthorityRejected),
            ("authority_reference_mismatch_rejected", AuthorityReferenceMismatchRejected),
            ("authority_producer_binding_mismatch_rejected", AuthorityProducerMismatchRejected),
            ("authority_application_binding_mismatch_rejected", AuthorityApplicationMismatchRejected),
            ("authority_recipient_binding_mismatch_rejected", AuthorityRecipientMismatchRejected),
            ("authority_purpose_mismatch_rejected", AuthorityPurposeMismatchRejected),
            ("authority_effective_scope_mismatch_rejected", AuthorityEffectiveScopeMismatchRejected),
            ("malformed_authority_result_rejected", MalformedAuthorityRejected),
            ("deny_authority_rejected", DenyAuthorityRejected),
            ("future_authority_rejected", FutureAuthorityRejected),
            ("expired_authority_rejected", ExpiredAuthorityRejected),
            ("unexpired_message_eligible", UnexpiredMessageEligible),
            ("boundary_expired_message_rejected", BoundaryExpiredMessageRejected),
            ("observation_time_mutation_changes_outcome", ObservationMutationChangesOutcome),
            ("equivalent_inputs_same_decision_identity", DeterministicDecisionIdentity),
            ("material_message_mutation_changes_decision_identity", MessageMutationChangesDecisionIdentity),
            ("producer_binding_mutation_changes_decision_identity", ProducerBindingMutationChangesIdentity),
            ("recipient_binding_mutation_changes_decision_identity", RecipientBindingMutationChangesIdentity),
            ("authority_binding_mutation_changes_decision_identity", AuthorityBindingMutationChangesIdentity),
            ("set_reordering_preserves_admission_identity", SetReorderingPreservesIdentity),
            ("admission_surface_has_no_later_wp_operations", NoLaterWpOperations),
            ("admission_does_not_create_route", AdmissionDoesNotCreateRoute),
            ("admission_does_not_deliver", AdmissionDoesNotDeliver),
            ("admission_does_not_execute", AdmissionDoesNotExecute),
            ("payload_business_semantics_remain_opaque", PayloadRemainsOpaque),
            ("fsats_receives_no_special_treatment", FsatsNoSpecialTreatment),
            ("zero_application_foundation_remains_valid", ZeroApplicationFoundationValid),
            ("result_surface_is_immutable", ResultSurfaceImmutable),
            ("decision_identity_is_sha256_bound", DecisionIdentitySha256Bound),
            ("effective_expiry_is_minimum_boundary", EffectiveExpiryMinimum)
        };

        var failures = new List<string>();
        foreach (var scenario in scenarios)
        {
            try
            {
                scenario.Test();
                Console.WriteLine($"PASS {scenario.Name}");
            }
            catch (Exception exception)
            {
                failures.Add($"{scenario.Name}: {exception.GetType().Name}: {exception.Message}");
                Console.WriteLine($"FAIL {scenario.Name}: {exception.Message}");
            }
        }

        Console.WriteLine($"RESULT {scenarios.Length - failures.Count}/{scenarios.Length} PASS");
        Console.WriteLine(
            failures.Count == 0
                ? "STAGE 5 WP-04 FIL VALIDATION AND MESSAGE ADMISSION VERIFIER: PASS"
                : "STAGE 5 WP-04 FIL VALIDATION AND MESSAGE ADMISSION VERIFIER: FAIL");

        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"DETAIL {failure}");
        }

        return failures.Count == 0 ? 0 : 1;
    }

    private static void ExactSchemaAdmitted() => AssertAdmitted(Evaluate(CreateFixture()));

    private static void CompatibleSchemaAdmitted()
    {
        var fixture = CreateFixture(manifestSchemaVersion: "1.0", messageSchemaVersion: "2.0", includeVersion2: true);
        AssertAccepted(fixture.SchemaRegistry.DeclareCompatibility(
            new SchemaCompatibilityRule(new SchemaIdentity(SchemaId), "2.0", "1.0", SchemaCompatibilityClassification.Backward,
                new ProvenanceReference("evidence:compatibility/backward"))));
        AssertAdmitted(Evaluate(fixture));
    }

    private static void TwoApplicationsAdmitIndependently()
    {
        var alpha = CreateFixture("application.alpha", "manifest:alpha");
        var beta = CreateFixture("application.beta", "manifest:beta");
        var first = Evaluate(alpha);
        var second = Evaluate(beta);
        AssertAdmitted(first);
        AssertAdmitted(second);
        Assert(first.DecisionId != second.DecisionId, "independent_applications_collapsed");
    }

    private static void NullEnvelopeRejected()
    {
        var fixture = CreateFixture();
        AssertRejected(fixture.Evaluator.Evaluate(null, fixture.Context), MessageAdmissionReason.InvalidEnvelope);
    }

    private static void NullContextRejected()
    {
        var fixture = CreateFixture();
        AssertRejected(fixture.Evaluator.Evaluate(fixture.Envelope, null), MessageAdmissionReason.InvalidContext);
    }

    private static void MissingProducerBindingRejected() =>
        AssertRejected(Evaluate(CreateFixture(omitProducerBinding: true)), MessageAdmissionReason.ProducerBindingMissing);

    private static void ProducerIdentityBindingMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(contextProducerIdentity: "producer:spoofed/reference")), MessageAdmissionReason.ProducerIdentityBindingMismatch);

    private static void UnknownManifestRejected() =>
        AssertRejected(Evaluate(CreateFixture(registerManifest: false)), MessageAdmissionReason.ManifestUnknown);

    private static void WrongProducerApplicationRejected() =>
        AssertRejected(Evaluate(CreateFixture(contextApplicationId: "application.other")), MessageAdmissionReason.ProducerApplicationMismatch);

    private static void MissingRecipientBindingRejected() =>
        AssertRejected(Evaluate(CreateFixture(omitRecipientBinding: true)), MessageAdmissionReason.RecipientBindingMissing);

    private static void RecipientScopeBindingMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(contextRecipientScope: "recipient:spoofed/reference")), MessageAdmissionReason.RecipientScopeBindingMismatch);

    private static void UndeclaredIntendedConsumerRejected() =>
        AssertRejected(Evaluate(CreateFixture(contextIntendedConsumer: "consumer:undeclared")), MessageAdmissionReason.RecipientConsumerUndeclared);

    private static void UndeclaredMessageTypeRejected() =>
        AssertRejected(Evaluate(CreateFixture(envelopeMessageType: "falcon.reference.other.v1")), MessageAdmissionReason.CommunicationUndeclared);

    private static void ConflictingCommunicationPredecessorFailsClosed()
    {
        var schemas = CreateSchemaRegistry(includeVersion2: true);
        var manifest = CreateManifest(communications: new[]
        {
            Declaration(schemaVersion: "1.0"),
            Declaration(schemaVersion: "2.0")
        });
        var validation = ApplicationCommunicationManifestValidator.Validate(manifest, schemas);
        Assert(!validation.IsValid, "conflicting_communication_predecessor_was_accepted");
        AssertEqual("CONFLICTING_COMMUNICATION_DECLARATION", validation.Code, "conflicting_communication_reason_mismatch");
    }

    private static void MessageKindMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(manifestKind: FilMessageKind.Query, envelopeKind: FilMessageKind.Command)), MessageAdmissionReason.MessageKindMismatch);

    private static void ClassificationMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(manifestClassification: FilMessageClassification.Security, envelopeClassification: FilMessageClassification.Operational)), MessageAdmissionReason.ClassificationMismatch);

    private static void InboundConsumerRejected() =>
        AssertRejected(Evaluate(CreateFixture(direction: CommunicationDirection.Inbound, role: CommunicationRole.Consumer)), MessageAdmissionReason.ProducerDeclarationInvalid);

    private static void SchemaIdentityMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(registerOtherSchema: true, envelopeSchemaId: "schema:falcon.reference.other")), MessageAdmissionReason.SchemaIdentityMismatch);

    private static void UnknownSchemaVersionRejected() =>
        AssertRejected(Evaluate(CreateFixture(messageSchemaVersion: "2.0")), MessageAdmissionReason.SchemaUnknown);

    private static void RetiredMessageSchemaRejected()
    {
        var fixture = CreateFixture();
        Retire(fixture.SchemaRegistry, SchemaId, "1.0");
        AssertRejected(Evaluate(fixture), MessageAdmissionReason.SchemaUnusable);
    }

    private static void RetiredManifestSchemaRejected()
    {
        var fixture = CreateFixture(manifestSchemaVersion: "1.0", messageSchemaVersion: "2.0", includeVersion2: true);
        AssertAccepted(fixture.SchemaRegistry.DeclareCompatibility(
            new SchemaCompatibilityRule(new SchemaIdentity(SchemaId), "2.0", "1.0", SchemaCompatibilityClassification.Backward,
                new ProvenanceReference("evidence:compatibility/backward"))));
        Retire(fixture.SchemaRegistry, SchemaId, "1.0");
        AssertRejected(Evaluate(fixture), MessageAdmissionReason.SchemaUnusable);
    }

    private static void IncompatibleSchemaRejected()
    {
        var fixture = CreateFixture(manifestSchemaVersion: "2.0", messageSchemaVersion: "1.0", includeVersion2: true);
        AssertAccepted(fixture.SchemaRegistry.DeclareCompatibility(
            new SchemaCompatibilityRule(new SchemaIdentity(SchemaId), "1.0", "2.0", SchemaCompatibilityClassification.Incompatible,
                new ProvenanceReference("evidence:compatibility/incompatible"))));
        AssertRejected(Evaluate(fixture), MessageAdmissionReason.SchemaIncompatible);
    }

    private static void UndeclaredCompatibilityRejected() =>
        AssertRejected(Evaluate(CreateFixture(manifestSchemaVersion: "2.0", messageSchemaVersion: "1.0", includeVersion2: true)), MessageAdmissionReason.SchemaCompatibilityUnresolved);

    private static void MissingAuthorityRejected() =>
        AssertRejected(Evaluate(CreateFixture(omitAuthorityBinding: true)), MessageAdmissionReason.AuthorityBindingMissing);

    private static void AuthorityReferenceMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(authorityReference: "authority:alternate/reference"))), MessageAdmissionReason.AuthorityBindingMismatch);

    private static void AuthorityProducerMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(authorizedProducer: "producer:other/reference"))), MessageAdmissionReason.AuthorityProducerMismatch);

    private static void AuthorityApplicationMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(authorizedApplication: "application.other"))), MessageAdmissionReason.AuthorityApplicationMismatch);

    private static void AuthorityRecipientMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(authorizedRecipient: "recipient:other/reference"))), MessageAdmissionReason.AuthorityRecipientMismatch);

    private static void AuthorityPurposeMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(authorizedPurpose: "other-purpose"))), MessageAdmissionReason.AuthorityPurposeMismatch);

    private static void AuthorityEffectiveScopeMismatchRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(boundEffectiveScope: "scope:other/reference"))), MessageAdmissionReason.AuthorityEffectiveScopeMismatch);

    private static void MalformedAuthorityRejected()
    {
        var malformed = new AuthorityResult("request:authority/1", "decision:authority/1", AuthorityDecision.Allow, EffectiveScope, "", "1.0",
            "conditions:reference", "bounded", AuthorityReason.Allowed, Observation.AddMinutes(-1), Observation.AddMinutes(10), "evidence:authority/result");
        var fixture = CreateFixture(authorityBinding: ValidAuthorityBinding(authorityResult: malformed));
        AssertRejected(Evaluate(fixture), MessageAdmissionReason.AuthorityResultMalformed);
    }

    private static void DenyAuthorityRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(decision: AuthorityDecision.Deny, boundEffectiveScope: "NONE"))), MessageAdmissionReason.AuthorityDenied);

    private static void FutureAuthorityRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(decisionTime: Observation.AddMinutes(1), expiry: Observation.AddMinutes(20)))), MessageAdmissionReason.AuthorityNotYetEffective);

    private static void ExpiredAuthorityRejected() =>
        AssertRejected(Evaluate(CreateFixture(authorityBinding: ValidAuthorityBinding(decisionTime: Observation.AddMinutes(-10), expiry: Observation))), MessageAdmissionReason.AuthorityExpired);

    private static void UnexpiredMessageEligible() => AssertAdmitted(Evaluate(CreateFixture(messageExpiry: Observation.AddMinutes(1))));

    private static void BoundaryExpiredMessageRejected() =>
        AssertRejected(Evaluate(CreateFixture(messageExpiry: Observation)), MessageAdmissionReason.MessageExpired);

    private static void ObservationMutationChangesOutcome()
    {
        var fixture = CreateFixture(messageExpiry: Observation.AddMinutes(1));
        var admitted = Evaluate(fixture);
        var later = CreateContext("application.alpha", "manifest:alpha", Observation.AddMinutes(1), ValidAuthorityBinding(expiry: Observation.AddMinutes(10)));
        var rejected = fixture.Evaluator.Evaluate(fixture.Envelope, later);
        AssertAdmitted(admitted);
        AssertRejected(rejected, MessageAdmissionReason.MessageExpired);
        Assert(admitted.DecisionId != rejected.DecisionId, "observation_mutation_not_bound");
    }

    private static void DeterministicDecisionIdentity()
    {
        var fixture = CreateFixture();
        var first = Evaluate(fixture);
        var second = Evaluate(fixture);
        AssertAdmitted(first);
        AssertEqual(first.DecisionId, second.DecisionId, "equivalent_inputs_not_deterministic");
    }

    private static void MessageMutationChangesDecisionIdentity()
    {
        var fixture = CreateFixture();
        var first = Evaluate(fixture);
        var second = fixture.Evaluator.Evaluate(CreateEnvelope(messageId: "msg:00000002"), fixture.Context);
        AssertAdmitted(first);
        AssertAdmitted(second);
        Assert(first.DecisionId != second.DecisionId, "material_message_mutation_not_bound");
    }

    private static void ProducerBindingMutationChangesIdentity()
    {
        var fixture = CreateFixture();
        var first = Evaluate(fixture);
        var second = fixture.Evaluator.Evaluate(fixture.Envelope, CreateContext(producerEvidence: "evidence:producer/binding-mutated"));
        AssertAdmitted(first);
        AssertAdmitted(second);
        Assert(first.DecisionId != second.DecisionId, "producer_binding_evidence_not_bound");
    }

    private static void RecipientBindingMutationChangesIdentity()
    {
        var fixture = CreateFixture();
        var first = Evaluate(fixture);
        var second = fixture.Evaluator.Evaluate(fixture.Envelope, CreateContext(recipientEvidence: "evidence:recipient/binding-mutated"));
        AssertAdmitted(first);
        AssertAdmitted(second);
        Assert(first.DecisionId != second.DecisionId, "recipient_binding_evidence_not_bound");
    }

    private static void AuthorityBindingMutationChangesIdentity()
    {
        var fixture = CreateFixture();
        var first = Evaluate(fixture);
        var context = CreateContext(authorityBinding: ValidAuthorityBinding(bindingEvidence: "evidence:authority/binding-mutated"));
        var second = fixture.Evaluator.Evaluate(fixture.Envelope, context);
        AssertAdmitted(first);
        AssertAdmitted(second);
        Assert(first.DecisionId != second.DecisionId, "authority_binding_evidence_not_bound");
    }

    private static void SetReorderingPreservesIdentity()
    {
        var schemasA = CreateSchemaRegistry();
        var schemasB = CreateSchemaRegistry();
        var manifestA = CreateManifest(reverseSets: false);
        var manifestB = CreateManifest(reverseSets: true);
        AssertEqual(ManifestCanonicalization.ComputeSha256(manifestA), ManifestCanonicalization.ComputeSha256(manifestB), "manifest_set_order_changed_digest");
        var registryA = new InMemoryApplicationCommunicationManifestRegistry(schemasA);
        var registryB = new InMemoryApplicationCommunicationManifestRegistry(schemasB);
        Assert(registryA.Register(manifestA).Accepted, "first_manifest_registration_failed");
        Assert(registryB.Register(manifestB).Accepted, "second_manifest_registration_failed");
        var envelope = CreateEnvelope();
        var context = CreateContext();
        var first = new FilMessageAdmissionEvaluator(registryA, schemasA).Evaluate(envelope, context);
        var second = new FilMessageAdmissionEvaluator(registryB, schemasB).Evaluate(envelope, context);
        AssertAdmitted(first);
        AssertAdmitted(second);
        AssertEqual(first.DecisionId, second.DecisionId, "set_reordering_changed_admission_identity");
    }

    private static void NoLaterWpOperations()
    {
        var prohibited = new[] { "Route", "Dispatch", "Send", "Deliver", "Retry", "Acknowledge", "DeadLetter", "Backpressure", "FlowControl", "Publish", "Subscribe", "EventJournal", "Encrypt", "Decrypt", "Sign", "KeyRotate", "Attach", "Detach", "Drain", "Upgrade", "ReplaceApplication" };
        var methodNames = typeof(FilMessageAdmissionEvaluator).Assembly.GetExportedTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Select(method => method.Name).Distinct(StringComparer.Ordinal).ToArray();
        foreach (var forbidden in prohibited)
        {
            Assert(!methodNames.Any(name => name.Contains(forbidden, StringComparison.OrdinalIgnoreCase)), $"later_wp_operation_exposed:{forbidden}");
        }
    }

    private static void AdmissionDoesNotCreateRoute() => AssertNoPublicMethodContaining("route");
    private static void AdmissionDoesNotDeliver() => AssertNoPublicMethodContaining("deliver");
    private static void AdmissionDoesNotExecute() => AssertNoPublicMethodContaining("execute");

    private static void PayloadRemainsOpaque()
    {
        var fixture = CreateFixture();
        var trading = CreateEnvelope(payload: "{\"market\":\"US\",\"action\":\"BUY\"}");
        var accounting = CreateEnvelope(payload: "{\"ledger\":\"receivable\",\"action\":\"POST\"}");
        AssertAdmitted(fixture.Evaluator.Evaluate(trading, fixture.Context));
        AssertAdmitted(fixture.Evaluator.Evaluate(accounting, fixture.Context));
    }

    private static void FsatsNoSpecialTreatment()
    {
        var fsats = CreateFixture("application.fsats", "manifest:fsats");
        var accounting = CreateFixture("application.accounting", "manifest:accounting");
        AssertAdmitted(Evaluate(fsats));
        AssertAdmitted(Evaluate(accounting));
    }

    private static void ZeroApplicationFoundationValid()
    {
        var schemas = CreateSchemaRegistry();
        var manifests = new InMemoryApplicationCommunicationManifestRegistry(schemas);
        var evaluator = new FilMessageAdmissionEvaluator(manifests, schemas);
        AssertRejected(evaluator.Evaluate(CreateEnvelope(), CreateContext()), MessageAdmissionReason.ManifestUnknown);
    }

    private static void ResultSurfaceImmutable()
    {
        var writable = typeof(MessageAdmissionResult).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.SetMethod is { IsPublic: true }).ToArray();
        Assert(writable.Length == 0, "message_admission_result_has_public_setter");
    }

    private static void DecisionIdentitySha256Bound()
    {
        var result = Evaluate(CreateFixture());
        const string prefix = "message-admission/sha256/";
        Assert(result.DecisionId.StartsWith(prefix, StringComparison.Ordinal), "decision_identity_prefix_invalid");
        var digest = result.DecisionId[prefix.Length..];
        Assert(digest.Length == 64, "decision_identity_digest_length_invalid");
        Assert(digest.All(character => character is >= '0' and <= '9' || character is >= 'A' and <= 'F'), "decision_identity_digest_not_upper_hex");
    }

    private static void EffectiveExpiryMinimum()
    {
        var fixture = CreateFixture(messageExpiry: Observation.AddMinutes(5), authorityBinding: ValidAuthorityBinding(expiry: Observation.AddMinutes(2)));
        var result = Evaluate(fixture);
        AssertAdmitted(result);
        AssertEqual(Observation.AddMinutes(2), result.EffectiveExpiry ?? throw new InvalidOperationException("effective_expiry_missing"), "effective_expiry_not_minimum");
    }

    private static MessageAdmissionResult Evaluate(Fixture fixture) => fixture.Evaluator.Evaluate(fixture.Envelope, fixture.Context);

    private static Fixture CreateFixture(
        string applicationId = "application.alpha", string manifestId = "manifest:alpha", string manifestSchemaVersion = "1.0",
        string messageSchemaVersion = "1.0", bool includeVersion2 = false, bool registerManifest = true,
        FilMessageKind manifestKind = FilMessageKind.Command, FilMessageKind envelopeKind = FilMessageKind.Command,
        FilMessageClassification manifestClassification = FilMessageClassification.Operational,
        FilMessageClassification envelopeClassification = FilMessageClassification.Operational,
        CommunicationDirection direction = CommunicationDirection.Outbound, CommunicationRole role = CommunicationRole.Producer,
        DateTimeOffset? messageExpiry = null, MessageAuthorityBinding? authorityBinding = null, bool omitAuthorityBinding = false,
        bool omitProducerBinding = false, bool omitRecipientBinding = false, string? contextApplicationId = null,
        string? contextProducerIdentity = null, string? contextRecipientScope = null, string? contextIntendedConsumer = null,
        string envelopeMessageType = MessageType, string envelopeSchemaId = SchemaId, bool registerOtherSchema = false)
    {
        var schemas = CreateSchemaRegistry(includeVersion2);
        if (registerOtherSchema) AssertAccepted(schemas.Register(Definition("schema:falcon.reference.other", "1.0", DigestB)));
        var manifests = new InMemoryApplicationCommunicationManifestRegistry(schemas);
        var manifest = CreateManifest(applicationId, manifestId, manifestSchemaVersion, manifestKind, manifestClassification, direction, role);
        if (registerManifest) Assert(manifests.Register(manifest).Accepted, "manifest_registration_failed");
        var producerIdentity = contextProducerIdentity ?? ProducerId;
        var recipientScope = contextRecipientScope ?? RecipientScope;
        var contextApp = contextApplicationId ?? applicationId;
        var envelope = CreateEnvelope(messageType: envelopeMessageType, schemaId: envelopeSchemaId, schemaVersion: messageSchemaVersion,
            kind: envelopeKind, classification: envelopeClassification, expiresAt: messageExpiry ?? Observation.AddMinutes(30));
        var effectiveAuthority = omitAuthorityBinding ? null : authorityBinding ?? ValidAuthorityBinding(
            authorizedProducer: producerIdentity, authorizedApplication: contextApp, authorizedRecipient: recipientScope);
        var context = CreateContext(contextApp, manifestId, authorityBinding: effectiveAuthority, defaultAuthorityWhenNull: false,
            omitProducerBinding: omitProducerBinding, omitRecipientBinding: omitRecipientBinding, producerIdentity: producerIdentity,
            recipientScope: recipientScope, intendedConsumer: contextIntendedConsumer ?? ConsumerRef);
        return new Fixture(schemas, manifests, new FilMessageAdmissionEvaluator(manifests, schemas), envelope, context);
    }

    private static InMemorySchemaRegistry CreateSchemaRegistry(bool includeVersion2 = false)
    {
        var registry = new InMemorySchemaRegistry();
        AssertAccepted(registry.Register(Definition(SchemaId, "1.0", DigestA)));
        if (includeVersion2) AssertAccepted(registry.Register(Definition(SchemaId, "2.0", DigestB)));
        return registry;
    }

    private static SchemaDefinition Definition(string schemaId, string version, string digest) =>
        new(new SchemaIdentity(schemaId), version, new SchemaOwnerReference("owner:schema/reference"), digest,
            new ProvenanceReference("evidence:schema/reference"));

    private static ApplicationCommunicationManifest CreateManifest(
        string applicationId = "application.alpha", string manifestId = "manifest:alpha", string schemaVersion = "1.0",
        FilMessageKind kind = FilMessageKind.Command, FilMessageClassification classification = FilMessageClassification.Operational,
        CommunicationDirection direction = CommunicationDirection.Outbound, CommunicationRole role = CommunicationRole.Producer,
        IEnumerable<CommunicationDeclaration>? communications = null, bool reverseSets = false)
    {
        var contracts = new[] { new ManifestReference("CON-004"), new ManifestReference("CON-023") };
        var services = new[] { new ManifestReference("service:fil"), new ManifestReference("service:authority") };
        if (reverseSets) { Array.Reverse(contracts); Array.Reverse(services); }
        return new ApplicationCommunicationManifest(new ManifestIdentity(manifestId), "1.0", new ApplicationIdentityReference(applicationId), "1.0",
            new ApplicationOwnerReference("owner:application/reference"), contracts, services,
            new[] { new ManifestReference("capability:reference") }, new[] { new ManifestReference(ConsumerRef) },
            new[] { new AuthorityReference(AuthorityRef) }, new[] { new ManifestReference("security:reference") },
            new[] { new ManifestReference("dependency:reference") }, new[] { new ManifestReference("configuration:reference") },
            new[] { new ProvenanceReference("evidence:manifest/reference") }, Lifecycle(),
            communications ?? new[] { Declaration(schemaVersion, kind, classification, direction, role) });
    }

    private static CommunicationDeclaration Declaration(string schemaVersion = "1.0", FilMessageKind kind = FilMessageKind.Command,
        FilMessageClassification classification = FilMessageClassification.Operational,
        CommunicationDirection direction = CommunicationDirection.Outbound, CommunicationRole role = CommunicationRole.Producer) =>
        new(MessageType, kind, classification, new ManifestSchemaReference(new SchemaIdentity(SchemaId), schemaVersion), direction, role);

    private static ManifestLifecycleDeclaration[] Lifecycle() => new[]
    {
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.ApplicationVersionChange, ManifestApplicabilityRule.RequiresRevalidation),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Update, ManifestApplicabilityRule.RequiresRevalidation),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Replacement, ManifestApplicabilityRule.Invalidated),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Suspension, ManifestApplicabilityRule.RemainsApplicable),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Removal, ManifestApplicabilityRule.Invalidated)
    };

    private static CanonicalFilEnvelope CreateEnvelope(string messageId = "msg:00000001", string messageType = MessageType,
        string schemaId = SchemaId, string schemaVersion = "1.0", FilMessageKind kind = FilMessageKind.Command,
        FilMessageClassification classification = FilMessageClassification.Operational, string producerIdentity = ProducerId,
        string recipientScope = RecipientScope, DateTimeOffset? expiresAt = null, string payload = "{\"reference\":\"opaque-payload\"}") =>
        CanonicalFilEnvelope.Create(new MessageIdentity(messageId), kind, classification, messageType, new SchemaIdentity(schemaId), schemaVersion,
            new ProducerIdentityReference(producerIdentity), new RecipientScopeReference(recipientScope),
            new CorrelationIdentity("correlation:00000001"), new CausationIdentity("causation:00000000"),
            new AuthorityReference(AuthorityRef), new ProvenanceReference("evidence:message/reference"),
            new IdempotencyIdentity("idempotency:00000001"), new DeliveryAttemptIdentity("attempt:00000001"),
            new RetryLineageIdentity("retry-lineage:00000001"),
            new CanonicalMessageTime(Observation.AddMinutes(-5), expiresAt ?? Observation.AddMinutes(30)),
            CanonicalOutcome.Unknown("processing_not_yet_attempted"), payload);

    private static MessageAdmissionContext CreateContext(string applicationId = "application.alpha", string manifestId = "manifest:alpha",
        DateTimeOffset? observation = null, MessageAuthorityBinding? authorityBinding = null, bool defaultAuthorityWhenNull = true,
        bool omitProducerBinding = false, bool omitRecipientBinding = false, string producerIdentity = ProducerId,
        string recipientScope = RecipientScope, string intendedConsumer = ConsumerRef,
        string producerEvidence = "evidence:producer/binding", string recipientEvidence = "evidence:recipient/binding")
    {
        MessageProducerBinding? producerBinding = omitProducerBinding ? null : new MessageProducerBinding(
            new ProducerIdentityReference(producerIdentity), new ApplicationIdentityReference(applicationId), new ManifestIdentity(manifestId),
            new ProvenanceReference(producerEvidence));
        MessageRecipientBinding? recipientBinding = omitRecipientBinding ? null : new MessageRecipientBinding(
            new RecipientScopeReference(recipientScope), new ManifestReference(intendedConsumer), new ProvenanceReference(recipientEvidence));
        var effectiveAuthority = defaultAuthorityWhenNull ? authorityBinding ?? ValidAuthorityBinding(
            authorizedProducer: producerIdentity, authorizedApplication: applicationId, authorizedRecipient: recipientScope) : authorityBinding;
        return new MessageAdmissionContext(producerBinding, "1.0", recipientBinding, observation ?? Observation, effectiveAuthority,
            new ProvenanceReference("evidence:admission/reference"));
    }

    private static MessageAuthorityBinding ValidAuthorityBinding(string decision = AuthorityDecision.Allow, DateTimeOffset? decisionTime = null,
        DateTimeOffset? expiry = null, string authorityReference = AuthorityRef, string authorizedProducer = ProducerId,
        string authorizedApplication = "application.alpha", string authorizedRecipient = RecipientScope,
        string authorizedPurpose = MessageAdmissionPurpose.FilMessageAdmission, string boundEffectiveScope = EffectiveScope,
        string resultEffectiveScope = EffectiveScope, string bindingEvidence = "evidence:authority/binding", AuthorityResult? authorityResult = null) =>
        new(new AuthorityReference(authorityReference), authorityResult ?? ValidAuthorityResult(decision, decisionTime, expiry, resultEffectiveScope),
            new ProducerIdentityReference(authorizedProducer), new ApplicationIdentityReference(authorizedApplication),
            new RecipientScopeReference(authorizedRecipient), authorizedPurpose, boundEffectiveScope, new ProvenanceReference(bindingEvidence));

    private static AuthorityResult ValidAuthorityResult(string decision = AuthorityDecision.Allow, DateTimeOffset? decisionTime = null,
        DateTimeOffset? expiry = null, string effectiveScope = EffectiveScope) =>
        new("request:authority/0001", "decision:authority/0001", decision, decision == AuthorityDecision.Allow ? effectiveScope : "NONE",
            "policy:message-admission", "1.0", "conditions:reference",
            decision == AuthorityDecision.Allow ? "BOUNDED_TO_EFFECTIVE_SCOPE" : "NO_EXECUTION_AUTHORITY",
            decision == AuthorityDecision.Allow ? AuthorityReason.Allowed : AuthorityReason.DefaultDeny,
            decisionTime ?? Observation.AddMinutes(-2), expiry ?? Observation.AddMinutes(20), "evidence:authority/result");

    private static void Retire(InMemorySchemaRegistry registry, string schemaId, string version)
    {
        var id = new SchemaIdentity(schemaId);
        AssertAccepted(registry.TransitionLifecycle(id, version, SchemaLifecycleState.Active));
        AssertAccepted(registry.TransitionLifecycle(id, version, SchemaLifecycleState.Deprecated));
        AssertAccepted(registry.TransitionLifecycle(id, version, SchemaLifecycleState.Retired));
    }

    private static void AssertNoPublicMethodContaining(string fragment)
    {
        var found = typeof(FilMessageAdmissionEvaluator).Assembly.GetExportedTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Any(method => method.Name.Contains(fragment, StringComparison.OrdinalIgnoreCase));
        Assert(!found, $"prohibited_public_operation_found:{fragment}");
    }

    private static void AssertAdmitted(MessageAdmissionResult result)
    {
        Assert(result.IsAdmitted, $"expected_admitted:{result.Reason}");
        AssertEqual(MessageAdmissionReason.Admitted, result.Reason, "admitted_reason_mismatch");
    }

    private static void AssertRejected(MessageAdmissionResult result, string reason)
    {
        Assert(!result.IsAdmitted, "expected_rejected");
        AssertEqual(reason, result.Reason, "rejection_reason_mismatch");
    }

    private static void AssertAccepted(SchemaRegistryOperationResult result) => Assert(result.Accepted, $"schema_operation_rejected:{result.Reason}");

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{message}:expected={expected}:actual={actual}");
    }

    private sealed record Fixture(InMemorySchemaRegistry SchemaRegistry, InMemoryApplicationCommunicationManifestRegistry ManifestRegistry,
        FilMessageAdmissionEvaluator Evaluator, CanonicalFilEnvelope Envelope, MessageAdmissionContext Context);
}
