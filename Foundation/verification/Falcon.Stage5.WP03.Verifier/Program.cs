using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.ApplicationManifest;
using Foundation.Contracts;
using Foundation.SchemaRegistry;

namespace Falcon.Stage5.WP03.Verifier;

internal static class Program
{
    private static readonly string DigestA = new('A', 64);

    private static int Main()
    {
        var scenarios = new (string Name, Action Test)[]
        {
            ("zero_application_foundation_is_valid", ZeroApplicationFoundationIsValid),
            ("two_independent_application_manifests_register", TwoIndependentApplicationManifestsRegister),
            ("duplicate_manifest_registration_rejected", DuplicateManifestRegistrationRejected),
            ("conflicting_manifest_registration_rejected", ConflictingManifestRegistrationRejected),
            ("manifest_identity_binding_conflict_rejected", ManifestIdentityBindingConflictRejected),
            ("unknown_manifest_resolution_fails_closed", UnknownManifestResolutionFailsClosed),
            ("known_manifest_resolves", KnownManifestResolves),
            ("unresolved_schema_reference_fails_closed", UnresolvedSchemaReferenceFailsClosed),
            ("retired_schema_reference_fails_closed", RetiredSchemaReferenceFailsClosed),
            ("supported_schema_lifecycle_states_validate", SupportedSchemaLifecycleStatesValidate),
            ("duplicate_manifest_references_rejected", DuplicateManifestReferencesRejected),
            ("duplicate_communication_declaration_rejected", DuplicateCommunicationDeclarationRejected),
            ("lifecycle_applicability_is_complete_and_explicit", LifecycleApplicabilityIsCompleteAndExplicit),
            ("incomplete_lifecycle_applicability_rejected", IncompleteLifecycleApplicabilityRejected),
            ("duplicate_lifecycle_applicability_rejected", DuplicateLifecycleApplicabilityRejected),
            ("invalid_lifecycle_values_rejected", InvalidLifecycleValuesRejected),
            ("invalid_direction_role_combinations_rejected", InvalidDirectionRoleCombinationsRejected),
            ("invalid_versions_and_identifiers_rejected", InvalidVersionsAndIdentifiersRejected),
            ("empty_communication_set_fails_closed", EmptyCommunicationSetFailsClosed),
            ("canonical_digest_is_deterministic", CanonicalDigestIsDeterministic),
            ("canonical_digest_is_order_independent_for_sets", CanonicalDigestIsOrderIndependentForSets),
            ("lifecycle_applicability_order_is_deterministic", LifecycleApplicabilityOrderIsDeterministic),
            ("lifecycle_applicability_changes_digest", LifecycleApplicabilityChangesDigest),
            ("different_manifest_content_changes_digest", DifferentManifestContentChangesDigest),
            ("snapshot_order_is_deterministic", SnapshotOrderIsDeterministic),
            ("manifest_validity_does_not_grant_authority", ManifestValidityDoesNotGrantAuthority),
            ("manifest_validity_does_not_create_route", ManifestValidityDoesNotCreateRoute),
            ("manifest_model_contains_no_business_payload", ManifestModelContainsNoBusinessPayload),
            ("fsats_receives_no_special_treatment", FsatsReceivesNoSpecialTreatment),
            ("two_application_digests_are_independent", TwoApplicationDigestsAreIndependent)
        };

        var failures = new List<string>();

        foreach (var scenario in scenarios)
        {
            try
            {
                scenario.Test();
                Console.WriteLine($"PASS {scenario.Name}");
            }
            catch (Exception ex)
            {
                failures.Add($"{scenario.Name}: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"FAIL {scenario.Name}");
            }
        }

        Console.WriteLine($"RESULT {scenarios.Length - failures.Count}/{scenarios.Length} PASS");

        if (failures.Count == 0)
        {
            Console.WriteLine("STAGE 5 WP-03 VERIFIER: PASS");
            return 0;
        }

        foreach (var failure in failures)
        {
            Console.Error.WriteLine(failure);
        }

        Console.Error.WriteLine("STAGE 5 WP-03 VERIFIER: FAIL");
        return 1;
    }

    private static void ZeroApplicationFoundationIsValid()
    {
        var schemas = new InMemorySchemaRegistry();
        var manifests = new InMemoryApplicationCommunicationManifestRegistry(schemas);
        Require(manifests.CaptureSnapshot().Count == 0, "zero application snapshot must remain empty");
    }

    private static void TwoIndependentApplicationManifestsRegister()
    {
        var context = CreateContext();
        Require(context.Manifests.Register(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha")).Accepted, "alpha rejected");
        Require(context.Manifests.Register(CreateManifest("app.beta", "manifest.beta", "owner.beta", "Foundation.Sample.Beta")).Accepted, "beta rejected");
        Require(context.Manifests.CaptureSnapshot().Count == 2, "expected two independent manifests");
    }

    private static void DuplicateManifestRegistrationRejected()
    {
        var context = CreateContext();
        var manifest = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha");
        Require(context.Manifests.Register(manifest).Accepted, "initial register failed");
        var duplicate = context.Manifests.Register(manifest);
        Require(!duplicate.Accepted && duplicate.Reason == "duplicate_manifest_registration", duplicate.Reason);
    }

    private static void ConflictingManifestRegistrationRejected()
    {
        var context = CreateContext();
        Require(context.Manifests.Register(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha")).Accepted, "initial register failed");
        var conflict = context.Manifests.Register(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Beta"));
        Require(!conflict.Accepted && conflict.Reason == "conflicting_manifest_registration", conflict.Reason);
    }

    private static void ManifestIdentityBindingConflictRejected()
    {
        var context = CreateContext();
        Require(context.Manifests.Register(CreateManifest("app.alpha", "manifest.shared", "owner.alpha", "Foundation.Sample.Alpha", "1.0")).Accepted, "first version failed");
        var conflict = context.Manifests.Register(CreateManifest("app.beta", "manifest.shared", "owner.beta", "Foundation.Sample.Beta", "2.0"));
        Require(!conflict.Accepted && conflict.Reason == "manifest_identity_binding_conflict", conflict.Reason);
    }

    private static void UnknownManifestResolutionFailsClosed()
    {
        var context = CreateContext();
        var result = context.Manifests.Resolve(new ManifestIdentity("manifest.unknown"), "1.0");
        Require(!result.Resolved && result.Reason == "manifest_version_unknown", result.Reason);
    }

    private static void KnownManifestResolves()
    {
        var context = CreateContext();
        var manifest = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha");
        Require(context.Manifests.Register(manifest).Accepted, "register failed");
        var result = context.Manifests.Resolve(manifest.ManifestId, manifest.ManifestVersion);
        Require(result.Resolved && result.Manifest is not null && IsSha256(result.ManifestSha256), result.Reason);
    }

    private static void UnresolvedSchemaReferenceFailsClosed()
    {
        var validation = ApplicationCommunicationManifestValidator.Validate(
            CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha"),
            new InMemorySchemaRegistry());
        Require(!validation.IsValid && validation.Code == "UNRESOLVED_SCHEMA_REFERENCE", validation.Code);
    }

    private static void RetiredSchemaReferenceFailsClosed()
    {
        var registry = CreateSchemaRegistry();
        Transition(registry, SchemaLifecycleState.Active);
        Transition(registry, SchemaLifecycleState.Deprecated);
        Transition(registry, SchemaLifecycleState.Retired);
        var validation = ApplicationCommunicationManifestValidator.Validate(
            CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha"), registry);
        Require(!validation.IsValid && validation.Code == "SCHEMA_REFERENCE_NOT_USABLE", validation.Code);
    }

    private static void SupportedSchemaLifecycleStatesValidate()
    {
        var registered = CreateSchemaRegistry();
        Require(ApplicationCommunicationManifestValidator.Validate(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha"), registered).IsValid, "registered rejected");

        var active = CreateSchemaRegistry();
        Transition(active, SchemaLifecycleState.Active);
        Require(ApplicationCommunicationManifestValidator.Validate(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha"), active).IsValid, "active rejected");

        var deprecated = CreateSchemaRegistry();
        Transition(deprecated, SchemaLifecycleState.Active);
        Transition(deprecated, SchemaLifecycleState.Deprecated);
        Require(ApplicationCommunicationManifestValidator.Validate(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha"), deprecated).IsValid, "deprecated rejected");
    }

    private static void DuplicateManifestReferencesRejected()
    {
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            contracts: new[] { new ManifestReference("contract.sample"), new ManifestReference("contract.sample") }), "duplicate_contract_reference");
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            services: new[] { new ManifestReference("service.sample"), new ManifestReference("service.sample") }), "duplicate_service_reference");
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            authorities: new[] { new AuthorityReference("authority.request"), new AuthorityReference("authority.request") }), "duplicate_authority_request");
    }

    private static void DuplicateCommunicationDeclarationRejected()
    {
        var declaration = CreateCommunication("Foundation.Sample.Alpha");
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            communications: new[] { declaration, declaration }), "duplicate_communication_declaration");
    }

    private static void LifecycleApplicabilityIsCompleteAndExplicit()
    {
        var manifest = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha");
        var expected = Enum.GetValues<ManifestLifecycleEvent>();
        Require(manifest.LifecycleApplicability.Count == expected.Length, "lifecycle applicability count mismatch");
        Require(expected.All(expectedEvent => manifest.LifecycleApplicability.Count(x => x.LifecycleEvent == expectedEvent) == 1), "lifecycle event missing or duplicated");
    }

    private static void IncompleteLifecycleApplicabilityRejected()
    {
        var incomplete = DefaultLifecycleApplicability()
            .Where(x => x.LifecycleEvent != ManifestLifecycleEvent.Removal)
            .ToArray();
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            lifecycleApplicability: incomplete), "incomplete_lifecycle_applicability_declaration");
    }

    private static void DuplicateLifecycleApplicabilityRejected()
    {
        var lifecycle = DefaultLifecycleApplicability().ToList();
        lifecycle.Add(new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Update, ManifestApplicabilityRule.Invalidated));
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            lifecycleApplicability: lifecycle), "duplicate_lifecycle_applicability_declaration");
    }

    private static void InvalidLifecycleValuesRejected()
    {
        ExpectArgument(
            () => new ManifestLifecycleDeclaration((ManifestLifecycleEvent)99, ManifestApplicabilityRule.RemainsApplicable),
            "enum_value_not_defined");
        ExpectArgument(
            () => new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Update, (ManifestApplicabilityRule)99),
            "enum_value_not_defined");
    }

    private static void InvalidDirectionRoleCombinationsRejected()
    {
        ExpectArgument(() => new CommunicationDeclaration("Foundation.Sample.Alpha", FilMessageKind.Command, FilMessageClassification.Operational, SchemaRef(), CommunicationDirection.Inbound, CommunicationRole.Producer), "inbound_cannot_declare_producer_role");
        ExpectArgument(() => new CommunicationDeclaration("Foundation.Sample.Alpha", FilMessageKind.Command, FilMessageClassification.Operational, SchemaRef(), CommunicationDirection.Outbound, CommunicationRole.Consumer), "outbound_cannot_declare_consumer_role");
    }

    private static void InvalidVersionsAndIdentifiersRejected()
    {
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha", manifestVersion: "01.0"), "version_not_canonical");
        ExpectArgument(() => CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha", applicationVersion: "1"), "version_not_canonical");
        ExpectArgument(() => new ManifestIdentity(" manifest.alpha"), "identifier_not_canonical");
    }

    private static void EmptyCommunicationSetFailsClosed()
    {
        var context = CreateContext();
        var manifest = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha", communications: Array.Empty<CommunicationDeclaration>());
        var validation = ApplicationCommunicationManifestValidator.Validate(manifest, context.Schemas);
        Require(!validation.IsValid && validation.Code == "EMPTY_COMMUNICATION_SET", validation.Code);
    }

    private static void CanonicalDigestIsDeterministic()
    {
        var manifest = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha");
        Require(ManifestCanonicalization.ComputeSha256(manifest) == ManifestCanonicalization.ComputeSha256(manifest), "digest changed");
    }

    private static void CanonicalDigestIsOrderIndependentForSets()
    {
        var first = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            contracts: new[] { new ManifestReference("contract.a"), new ManifestReference("contract.b") });
        var second = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            contracts: new[] { new ManifestReference("contract.b"), new ManifestReference("contract.a") });
        Require(ManifestCanonicalization.ComputeSha256(first) == ManifestCanonicalization.ComputeSha256(second), "set order changed digest");
    }

    private static void LifecycleApplicabilityOrderIsDeterministic()
    {
        var first = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            lifecycleApplicability: DefaultLifecycleApplicability());
        var second = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha",
            lifecycleApplicability: DefaultLifecycleApplicability().Reverse());
        Require(ManifestCanonicalization.ComputeSha256(first) == ManifestCanonicalization.ComputeSha256(second), "lifecycle declaration order changed digest");
    }

    private static void LifecycleApplicabilityChangesDigest()
    {
        var first = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha");
        var changed = DefaultLifecycleApplicability()
            .Select(x => x.LifecycleEvent == ManifestLifecycleEvent.Suspension
                ? new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Suspension, ManifestApplicabilityRule.RequiresRevalidation)
                : x)
            .ToArray();
        var second = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha", lifecycleApplicability: changed);
        Require(ManifestCanonicalization.ComputeSha256(first) != ManifestCanonicalization.ComputeSha256(second), "lifecycle applicability did not change digest");
    }

    private static void DifferentManifestContentChangesDigest()
    {
        var first = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha");
        var second = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Beta");
        Require(ManifestCanonicalization.ComputeSha256(first) != ManifestCanonicalization.ComputeSha256(second), "different content shared digest");
    }

    private static void SnapshotOrderIsDeterministic()
    {
        var context = CreateContext();
        Require(context.Manifests.Register(CreateManifest("app.beta", "manifest.beta", "owner.beta", "Foundation.Sample.Beta")).Accepted, "beta failed");
        Require(context.Manifests.Register(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha")).Accepted, "alpha failed");
        var actual = context.Manifests.CaptureSnapshot().Select(x => x.ManifestId.Value).ToArray();
        Require(actual.SequenceEqual(new[] { "manifest.alpha", "manifest.beta" }, StringComparer.Ordinal), "snapshot order not canonical");
    }

    private static void ManifestValidityDoesNotGrantAuthority()
    {
        var context = CreateContext();
        var result = context.Manifests.Register(CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha"));
        Require(result.Accepted, result.Reason);
        Require(typeof(ManifestRegistrationResult).GetProperties().All(p => !p.Name.Contains("Authority", StringComparison.OrdinalIgnoreCase)), "registration exposes authority grant");
    }

    private static void ManifestValidityDoesNotCreateRoute() =>
        Require(typeof(InMemoryApplicationCommunicationManifestRegistry).GetMethods().All(m => !m.Name.Contains("Route", StringComparison.OrdinalIgnoreCase)), "manifest registry exposes routing");

    private static void ManifestModelContainsNoBusinessPayload() =>
        Require(typeof(ApplicationCommunicationManifest).GetProperties().All(p => !p.Name.Contains("Payload", StringComparison.OrdinalIgnoreCase)), "manifest exposes payload");

    private static void FsatsReceivesNoSpecialTreatment()
    {
        var context = CreateContext();
        Require(context.Manifests.Register(CreateManifest("app.fsats", "manifest.fsats", "owner.fsats", "Foundation.Sample.Alpha")).Accepted, "generic FSATS identity rejected");
        Require(typeof(ApplicationCommunicationManifest).Assembly.GetTypes().All(t => !t.Name.Contains("Fsats", StringComparison.OrdinalIgnoreCase)), "FSATS special type found");
    }

    private static void TwoApplicationDigestsAreIndependent()
    {
        var alpha = CreateManifest("app.alpha", "manifest.alpha", "owner.alpha", "Foundation.Sample.Alpha");
        var beta = CreateManifest("app.beta", "manifest.beta", "owner.beta", "Foundation.Sample.Beta");
        Require(ManifestCanonicalization.ComputeSha256(alpha) != ManifestCanonicalization.ComputeSha256(beta), "independent applications shared digest");
    }

    private static Context CreateContext()
    {
        var schemas = CreateSchemaRegistry();
        return new Context(schemas, new InMemoryApplicationCommunicationManifestRegistry(schemas));
    }

    private static InMemorySchemaRegistry CreateSchemaRegistry()
    {
        var registry = new InMemorySchemaRegistry();
        var result = registry.Register(new SchemaDefinition(
            new SchemaIdentity("schema.sample"),
            "1.0",
            new SchemaOwnerReference("owner.schema"),
            DigestA,
            new ProvenanceReference("evidence.schema")));
        Require(result.Accepted, result.Reason);
        return registry;
    }

    private static void Transition(InMemorySchemaRegistry registry, SchemaLifecycleState target)
    {
        var result = registry.TransitionLifecycle(new SchemaIdentity("schema.sample"), "1.0", target);
        Require(result.Accepted, result.Reason);
    }

    private static ApplicationCommunicationManifest CreateManifest(
        string applicationId,
        string manifestId,
        string owner,
        string messageType,
        string manifestVersion = "1.0",
        string applicationVersion = "1.0",
        IEnumerable<ManifestReference>? contracts = null,
        IEnumerable<ManifestReference>? services = null,
        IEnumerable<AuthorityReference>? authorities = null,
        IEnumerable<ManifestLifecycleDeclaration>? lifecycleApplicability = null,
        IEnumerable<CommunicationDeclaration>? communications = null)
    {
        return new ApplicationCommunicationManifest(
            new ManifestIdentity(manifestId),
            manifestVersion,
            new ApplicationIdentityReference(applicationId),
            applicationVersion,
            new ApplicationOwnerReference(owner),
            contracts ?? new[] { new ManifestReference("contract.sample") },
            services ?? new[] { new ManifestReference("service.sample") },
            new[] { new ManifestReference("capability.sample") },
            new[] { new ManifestReference("consumer.sample") },
            authorities ?? new[] { new AuthorityReference("authority.request") },
            new[] { new ManifestReference("security.profile") },
            new[] { new ManifestReference("dependency.sample") },
            new[] { new ManifestReference("configuration.sample") },
            new[] { new ProvenanceReference("evidence.manifest") },
            lifecycleApplicability ?? DefaultLifecycleApplicability(),
            communications ?? new[] { CreateCommunication(messageType) });
    }

    private static ManifestLifecycleDeclaration[] DefaultLifecycleApplicability() =>
        new[]
        {
            new ManifestLifecycleDeclaration(ManifestLifecycleEvent.ApplicationVersionChange, ManifestApplicabilityRule.RequiresRevalidation),
            new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Update, ManifestApplicabilityRule.RequiresRevalidation),
            new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Replacement, ManifestApplicabilityRule.Invalidated),
            new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Suspension, ManifestApplicabilityRule.RemainsApplicable),
            new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Removal, ManifestApplicabilityRule.Invalidated)
        };

    private static CommunicationDeclaration CreateCommunication(string messageType) =>
        new(messageType, FilMessageKind.Command, FilMessageClassification.Operational, SchemaRef(), CommunicationDirection.Outbound, CommunicationRole.Producer);

    private static ManifestSchemaReference SchemaRef() => new(new SchemaIdentity("schema.sample"), "1.0");

    private static bool IsSha256(string? value) =>
        value is { Length: 64 } && value.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F');

    private static void ExpectArgument(Action action, string expectedMessage)
    {
        try
        {
            action();
        }
        catch (ArgumentException ex)
        {
            Require(ex.Message.Contains(expectedMessage, StringComparison.Ordinal), ex.Message);
            return;
        }

        throw new InvalidOperationException($"expected_argument_exception:{expectedMessage}");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed record Context(InMemorySchemaRegistry Schemas, InMemoryApplicationCommunicationManifestRegistry Manifests);
}
