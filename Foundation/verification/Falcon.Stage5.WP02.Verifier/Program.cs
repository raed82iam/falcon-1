using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts;
using Foundation.SchemaRegistry;

namespace Falcon.Stage5.WP02.Verifier;

internal static class Program
{
    private static readonly string DigestA =
        new('A', 64);

    private static readonly string DigestB =
        new('B', 64);

    private static readonly string DigestC =
        new('C', 64);

    private static int Main()
    {
        var scenarios = new (string Name, Action Test)[]
        {
            ("positive_registration_and_exact_resolution", PositiveRegistrationAndExactResolution),
            ("duplicate_registration_rejected", DuplicateRegistrationRejected),
            ("conflicting_digest_registration_rejected", ConflictingDigestRegistrationRejected),
            ("conflicting_owner_registration_rejected", ConflictingOwnerRegistrationRejected),
            ("cross_version_owner_change_rejected", CrossVersionOwnerChangeRejected),
            ("unknown_schema_resolution_fails_closed", UnknownSchemaResolutionFailsClosed),
            ("unknown_version_resolution_fails_closed", UnknownVersionResolutionFailsClosed),
            ("exact_compatibility_is_implicit", ExactCompatibilityIsImplicit),
            ("backward_compatibility_explicit", BackwardCompatibilityExplicit),
            ("forward_compatibility_explicit", ForwardCompatibilityExplicit),
            ("incompatible_relationship_explicit", IncompatibleRelationshipExplicit),
            ("undeclared_compatibility_fails_closed", UndeclaredCompatibilityFailsClosed),
            ("unknown_version_compatibility_fails_closed", UnknownVersionCompatibilityFailsClosed),
            ("duplicate_compatibility_rule_rejected", DuplicateCompatibilityRuleRejected),
            ("conflicting_compatibility_rule_rejected", ConflictingCompatibilityRuleRejected),
            ("cross_version_exact_rule_rejected", CrossVersionExactRuleRejected),
            ("same_version_nonexact_rule_rejected", SameVersionNonExactRuleRejected),
            ("lifecycle_registered_to_active", LifecycleRegisteredToActive),
            ("lifecycle_active_to_deprecated", LifecycleActiveToDeprecated),
            ("lifecycle_deprecated_to_retired", LifecycleDeprecatedToRetired),
            ("lifecycle_jump_rejected", LifecycleJumpRejected),
            ("lifecycle_reverse_rejected", LifecycleReverseRejected),
            ("lifecycle_noop_rejected", LifecycleNoopRejected),
            ("unknown_lifecycle_enum_rejected", UnknownLifecycleEnumRejected),
            ("unknown_compatibility_enum_rejected", UnknownCompatibilityEnumRejected),
            ("invalid_schema_version_fails_closed", InvalidSchemaVersionFailsClosed),
            ("leading_zero_schema_version_fails_closed", LeadingZeroSchemaVersionFailsClosed),
            ("invalid_owner_identifier_fails_closed", InvalidOwnerIdentifierFailsClosed),
            ("invalid_sha_length_fails_closed", InvalidShaLengthFailsClosed),
            ("invalid_sha_lowercase_fails_closed", InvalidShaLowercaseFailsClosed),
            ("invalid_sha_character_fails_closed", InvalidShaCharacterFailsClosed),
            ("snapshot_is_immutable_surface", SnapshotIsImmutableSurface),
            ("snapshot_sorted_and_deterministic", SnapshotSortedAndDeterministic),
            ("snapshot_mutation_changes_digest", SnapshotMutationChangesDigest),
            ("snapshot_digest_is_canonical_sha256", SnapshotDigestIsCanonicalSha256),
            ("snapshot_replay_is_deterministic", SnapshotReplayIsDeterministic),
            ("rejected_operation_does_not_change_snapshot", RejectedOperationDoesNotChangeSnapshot),
            ("zero_application_neutrality", ZeroApplicationNeutrality),
            ("two_independent_schema_owners", TwoIndependentSchemaOwners),
            ("payload_meaning_remains_opaque", PayloadMeaningRemainsOpaque),
            ("registry_does_not_grant_authority", RegistryDoesNotGrantAuthority),
            ("wp01_schema_identity_is_reused", Wp01SchemaIdentityIsReused)
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
                failures.Add(
                    $"{scenario.Name}: {exception.GetType().Name}: {exception.Message}");

                Console.WriteLine($"FAIL {scenario.Name}: {exception.Message}");
            }
        }

        if (failures.Count == 0)
        {
            Console.WriteLine("STAGE 5 WP-02 SCHEMA REGISTRY AND COMPATIBILITY VERIFIER: PASS");
        }
        else
        {
            Console.WriteLine("STAGE 5 WP-02 SCHEMA REGISTRY AND COMPATIBILITY VERIFIER: FAIL");
        }

        Console.WriteLine($"Scenarios: {scenarios.Length}");
        Console.WriteLine($"Failures: {failures.Count}");

        foreach (var failure in failures)
        {
            Console.WriteLine($"DETAIL {failure}");
        }

        return failures.Count == 0 ? 0 : 1;
    }

    private static void PositiveRegistrationAndExactResolution()
    {
        var registry = new InMemorySchemaRegistry();
        var definition = Definition("schema.orders", "1.0", "application.alpha", DigestA);

        AssertAccepted(registry.Register(definition), "registration");

        var resolution = registry.Resolve(definition.SchemaId, definition.Version);

        Assert(resolution.Resolved, "expected schema resolution");

        var entry =
            resolution.Entry ??
            throw new InvalidOperationException("resolved entry missing");

        Assert(entry.Definition == definition, "resolved definition mismatch");
        Assert(
            entry.Lifecycle == SchemaLifecycleState.Registered,
            "initial lifecycle must be Registered");
    }

    private static void DuplicateRegistrationRejected()
    {
        var registry = new InMemorySchemaRegistry();
        var definition = Definition("schema.orders", "1.0", "application.alpha", DigestA);

        AssertAccepted(registry.Register(definition), "first registration");
        AssertRejected(
            registry.Register(definition),
            "duplicate_schema_registration");
    }

    private static void ConflictingDigestRegistrationRejected()
    {
        var registry = new InMemorySchemaRegistry();

        AssertAccepted(
            registry.Register(
                Definition("schema.orders", "1.0", "application.alpha", DigestA)),
            "first registration");

        AssertRejected(
            registry.Register(
                Definition("schema.orders", "1.0", "application.alpha", DigestB)),
            "conflicting_schema_registration");
    }

    private static void ConflictingOwnerRegistrationRejected()
    {
        var registry = new InMemorySchemaRegistry();

        AssertAccepted(
            registry.Register(
                Definition("schema.orders", "1.0", "application.alpha", DigestA)),
            "first registration");

        AssertRejected(
            registry.Register(
                Definition("schema.orders", "1.0", "application.beta", DigestA)),
            "conflicting_schema_registration");
    }

    private static void CrossVersionOwnerChangeRejected()
    {
        var registry = new InMemorySchemaRegistry();

        AssertAccepted(
            registry.Register(
                Definition(
                    "schema.orders",
                    "1.0",
                    "application.alpha",
                    DigestA)),
            "first version registration");

        var before = registry.CaptureSnapshot();

        AssertRejected(
            registry.Register(
                Definition(
                    "schema.orders",
                    "2.0",
                    "application.beta",
                    DigestB)),
            "schema_owner_conflict");

        var after = registry.CaptureSnapshot();

        AssertEqual(
            before.Sha256,
            after.Sha256,
            "owner-conflict rejection must not mutate registry state");

        Assert(
            !registry.Resolve(
                new SchemaIdentity("schema.orders"),
                "2.0").Resolved,
            "conflicting-owner version must not be registered");
    }

    private static void UnknownSchemaResolutionFailsClosed()
    {
        var registry = new InMemorySchemaRegistry();

        var resolution =
            registry.Resolve(
                new SchemaIdentity("schema.unknown"),
                "1.0");

        Assert(!resolution.Resolved, "unknown schema must not resolve");
        AssertEqual("schema_version_unknown", resolution.Reason, "unknown reason");
        Assert(resolution.Entry is null, "unknown schema entry must be null");
    }

    private static void UnknownVersionResolutionFailsClosed()
    {
        var registry = RegistryWithVersions("1.0");

        var resolution =
            registry.Resolve(
                new SchemaIdentity("schema.orders"),
                "2.0");

        Assert(!resolution.Resolved, "unknown version must not resolve");
        AssertEqual("schema_version_unknown", resolution.Reason, "unknown reason");
    }

    private static void ExactCompatibilityIsImplicit()
    {
        var registry = RegistryWithVersions("1.0");

        var decision =
            registry.EvaluateCompatibility(
                new SchemaIdentity("schema.orders"),
                "1.0",
                "1.0");

        Assert(decision.Resolved, "exact compatibility should resolve");
        Assert(decision.IsCompatible, "exact compatibility should be compatible");
        Assert(
            decision.Classification == SchemaCompatibilityClassification.Exact,
            "exact classification expected");
    }

    private static void BackwardCompatibilityExplicit()
    {
        var registry = RegistryWithVersions("1.0", "2.0");

        AssertAccepted(
            registry.DeclareCompatibility(
                Rule(
                    "schema.orders",
                    "2.0",
                    "1.0",
                    SchemaCompatibilityClassification.Backward)),
            "backward rule");

        var decision =
            registry.EvaluateCompatibility(
                new SchemaIdentity("schema.orders"),
                "2.0",
                "1.0");

        Assert(decision.Resolved, "backward relation should resolve");
        Assert(decision.IsCompatible, "backward relation should be compatible");
        Assert(
            decision.Classification == SchemaCompatibilityClassification.Backward,
            "backward classification expected");
    }

    private static void ForwardCompatibilityExplicit()
    {
        var registry = RegistryWithVersions("1.0", "2.0");

        AssertAccepted(
            registry.DeclareCompatibility(
                Rule(
                    "schema.orders",
                    "1.0",
                    "2.0",
                    SchemaCompatibilityClassification.Forward)),
            "forward rule");

        var decision =
            registry.EvaluateCompatibility(
                new SchemaIdentity("schema.orders"),
                "1.0",
                "2.0");

        Assert(decision.Resolved, "forward relation should resolve");
        Assert(decision.IsCompatible, "forward relation should be compatible");
        Assert(
            decision.Classification == SchemaCompatibilityClassification.Forward,
            "forward classification expected");
    }

    private static void IncompatibleRelationshipExplicit()
    {
        var registry = RegistryWithVersions("1.0", "2.0");

        AssertAccepted(
            registry.DeclareCompatibility(
                Rule(
                    "schema.orders",
                    "1.0",
                    "2.0",
                    SchemaCompatibilityClassification.Incompatible)),
            "incompatible rule");

        var decision =
            registry.EvaluateCompatibility(
                new SchemaIdentity("schema.orders"),
                "1.0",
                "2.0");

        Assert(decision.Resolved, "incompatible relation should resolve");
        Assert(!decision.IsCompatible, "incompatible relation must be false");
        Assert(
            decision.Classification == SchemaCompatibilityClassification.Incompatible,
            "incompatible classification expected");
    }

    private static void UndeclaredCompatibilityFailsClosed()
    {
        var registry = RegistryWithVersions("1.0", "2.0");

        var decision =
            registry.EvaluateCompatibility(
                new SchemaIdentity("schema.orders"),
                "1.0",
                "2.0");

        Assert(!decision.Resolved, "undeclared relation must not resolve");
        Assert(!decision.IsCompatible, "undeclared relation must fail closed");
        AssertEqual(
            "compatibility_rule_undeclared",
            decision.Reason,
            "undeclared compatibility reason");
    }

    private static void UnknownVersionCompatibilityFailsClosed()
    {
        var registry = RegistryWithVersions("1.0");

        var decision =
            registry.EvaluateCompatibility(
                new SchemaIdentity("schema.orders"),
                "1.0",
                "2.0");

        Assert(!decision.Resolved, "unknown version compatibility must not resolve");
        Assert(!decision.IsCompatible, "unknown version compatibility must fail closed");
        AssertEqual(
            "compatibility_schema_version_unknown",
            decision.Reason,
            "unknown compatibility reason");
    }

    private static void DuplicateCompatibilityRuleRejected()
    {
        var registry = RegistryWithVersions("1.0", "2.0");

        var rule =
            Rule(
                "schema.orders",
                "1.0",
                "2.0",
                SchemaCompatibilityClassification.Forward);

        AssertAccepted(registry.DeclareCompatibility(rule), "first rule");
        AssertRejected(
            registry.DeclareCompatibility(rule),
            "duplicate_compatibility_rule");
    }

    private static void ConflictingCompatibilityRuleRejected()
    {
        var registry = RegistryWithVersions("1.0", "2.0");

        AssertAccepted(
            registry.DeclareCompatibility(
                Rule(
                    "schema.orders",
                    "1.0",
                    "2.0",
                    SchemaCompatibilityClassification.Forward)),
            "first rule");

        AssertRejected(
            registry.DeclareCompatibility(
                Rule(
                    "schema.orders",
                    "1.0",
                    "2.0",
                    SchemaCompatibilityClassification.Incompatible)),
            "conflicting_compatibility_rule");
    }

    private static void CrossVersionExactRuleRejected()
    {
        ExpectThrows<ArgumentException>(
            () => Rule(
                "schema.orders",
                "1.0",
                "2.0",
                SchemaCompatibilityClassification.Exact));
    }

    private static void SameVersionNonExactRuleRejected()
    {
        ExpectThrows<ArgumentException>(
            () => Rule(
                "schema.orders",
                "1.0",
                "1.0",
                SchemaCompatibilityClassification.Backward));
    }

    private static void LifecycleRegisteredToActive()
    {
        var registry = RegistryWithVersions("1.0");

        AssertAccepted(
            registry.TransitionLifecycle(
                new SchemaIdentity("schema.orders"),
                "1.0",
                SchemaLifecycleState.Active),
            "activate");

        AssertLifecycle(registry, "1.0", SchemaLifecycleState.Active);
    }

    private static void LifecycleActiveToDeprecated()
    {
        var registry = RegistryWithVersions("1.0");
        var id = new SchemaIdentity("schema.orders");

        AssertAccepted(
            registry.TransitionLifecycle(
                id,
                "1.0",
                SchemaLifecycleState.Active),
            "activate");

        AssertAccepted(
            registry.TransitionLifecycle(
                id,
                "1.0",
                SchemaLifecycleState.Deprecated),
            "deprecate");

        AssertLifecycle(registry, "1.0", SchemaLifecycleState.Deprecated);
    }

    private static void LifecycleDeprecatedToRetired()
    {
        var registry = RegistryWithVersions("1.0");
        var id = new SchemaIdentity("schema.orders");

        AssertAccepted(
            registry.TransitionLifecycle(
                id,
                "1.0",
                SchemaLifecycleState.Active),
            "activate");

        AssertAccepted(
            registry.TransitionLifecycle(
                id,
                "1.0",
                SchemaLifecycleState.Deprecated),
            "deprecate");

        AssertAccepted(
            registry.TransitionLifecycle(
                id,
                "1.0",
                SchemaLifecycleState.Retired),
            "retire");

        AssertLifecycle(registry, "1.0", SchemaLifecycleState.Retired);
    }

    private static void LifecycleJumpRejected()
    {
        var registry = RegistryWithVersions("1.0");

        AssertRejected(
            registry.TransitionLifecycle(
                new SchemaIdentity("schema.orders"),
                "1.0",
                SchemaLifecycleState.Deprecated),
            "schema_lifecycle_transition_invalid");
    }

    private static void LifecycleReverseRejected()
    {
        var registry = RegistryWithVersions("1.0");
        var id = new SchemaIdentity("schema.orders");

        AssertAccepted(
            registry.TransitionLifecycle(
                id,
                "1.0",
                SchemaLifecycleState.Active),
            "activate");

        AssertRejected(
            registry.TransitionLifecycle(
                id,
                "1.0",
                SchemaLifecycleState.Registered),
            "schema_lifecycle_transition_invalid");
    }

    private static void LifecycleNoopRejected()
    {
        var registry = RegistryWithVersions("1.0");

        AssertRejected(
            registry.TransitionLifecycle(
                new SchemaIdentity("schema.orders"),
                "1.0",
                SchemaLifecycleState.Registered),
            "schema_lifecycle_noop");
    }

    private static void UnknownLifecycleEnumRejected()
    {
        var registry = RegistryWithVersions("1.0");

        ExpectThrows<ArgumentOutOfRangeException>(
            () => registry.TransitionLifecycle(
                new SchemaIdentity("schema.orders"),
                "1.0",
                (SchemaLifecycleState)99));
    }

    private static void UnknownCompatibilityEnumRejected()
    {
        ExpectThrows<ArgumentOutOfRangeException>(
            () => new SchemaCompatibilityRule(
                new SchemaIdentity("schema.orders"),
                "1.0",
                "2.0",
                (SchemaCompatibilityClassification)99,
                new ProvenanceReference("evidence/schema-rule")));
    }

    private static void InvalidSchemaVersionFailsClosed()
    {
        foreach (var version in new[] { ".1", "1.", "1..0", "v1.0", "1" })
        {
            ExpectThrows<ArgumentException>(
                () => Definition(
                    "schema.orders",
                    version,
                    "application.alpha",
                    DigestA));
        }
    }

    private static void LeadingZeroSchemaVersionFailsClosed()
    {
        ExpectThrows<ArgumentException>(
            () => Definition(
                "schema.orders",
                "01.0",
                "application.alpha",
                DigestA));
    }

    private static void InvalidOwnerIdentifierFailsClosed()
    {
        ExpectThrows<ArgumentException>(
            () => Definition(
                "schema.orders",
                "1.0",
                "bad owner",
                DigestA));
    }

    private static void InvalidShaLengthFailsClosed()
    {
        ExpectThrows<ArgumentException>(
            () => Definition(
                "schema.orders",
                "1.0",
                "application.alpha",
                new string('A', 63)));
    }

    private static void InvalidShaLowercaseFailsClosed()
    {
        ExpectThrows<ArgumentException>(
            () => Definition(
                "schema.orders",
                "1.0",
                "application.alpha",
                new string('a', 64)));
    }

    private static void InvalidShaCharacterFailsClosed()
    {
        ExpectThrows<ArgumentException>(
            () => Definition(
                "schema.orders",
                "1.0",
                "application.alpha",
                new string('G', 64)));
    }

    private static void SnapshotIsImmutableSurface()
    {
        var registry = RegistryWithVersions("1.0", "2.0");
        var snapshot = registry.CaptureSnapshot();

        Assert(
            snapshot.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .All(property => property.SetMethod is null),
            "snapshot public properties must be get-only");

        Assert(
            typeof(SchemaRegistrySnapshot)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Length == 0,
            "snapshot construction must remain registry-controlled");

        Assert(
            snapshot.Entries is not Array,
            "snapshot entries must not expose mutable array");

        Assert(
            snapshot.CompatibilityRules is not Array,
            "snapshot rules must not expose mutable array");

        var nonGenericList = snapshot.Entries as IList;

        if (nonGenericList is not null)
        {
            Assert(nonGenericList.IsReadOnly, "snapshot entries must be read-only");
        }
    }

    private static void SnapshotSortedAndDeterministic()
    {
        var first = new InMemorySchemaRegistry();
        var second = new InMemorySchemaRegistry();

        AssertAccepted(
            first.Register(
                Definition("schema.beta", "2.0", "application.beta", DigestB)),
            "first beta");

        AssertAccepted(
            first.Register(
                Definition("schema.alpha", "1.0", "application.alpha", DigestA)),
            "first alpha");

        AssertAccepted(
            second.Register(
                Definition("schema.alpha", "1.0", "application.alpha", DigestA)),
            "second alpha");

        AssertAccepted(
            second.Register(
                Definition("schema.beta", "2.0", "application.beta", DigestB)),
            "second beta");

        var firstSnapshot = first.CaptureSnapshot();
        var secondSnapshot = second.CaptureSnapshot();

        AssertEqual(
            firstSnapshot.Sha256,
            secondSnapshot.Sha256,
            "equivalent registry states must have equal digest");

        AssertEqual(
            "schema.alpha",
            firstSnapshot.Entries[0].Definition.SchemaId.Value,
            "snapshot must be sorted by schema identity");
    }

    private static void SnapshotMutationChangesDigest()
    {
        var registry = RegistryWithVersions("1.0");

        var before = registry.CaptureSnapshot().Sha256;

        AssertAccepted(
            registry.Register(
                Definition(
                    "schema.orders",
                    "2.0",
                    "application.alpha",
                    DigestB)),
            "second version");

        var after = registry.CaptureSnapshot().Sha256;

        Assert(
            !string.Equals(before, after, StringComparison.Ordinal),
            "accepted mutation must change snapshot digest");
    }

    private static void SnapshotDigestIsCanonicalSha256()
    {
        var snapshot = RegistryWithVersions("1.0").CaptureSnapshot();

        Assert(snapshot.Sha256.Length == 64, "snapshot digest length");
        Assert(
            snapshot.Sha256.All(
                character =>
                    character is >= '0' and <= '9' ||
                    character is >= 'A' and <= 'F'),
            "snapshot digest must be uppercase hex");
    }

    private static void SnapshotReplayIsDeterministic()
    {
        var registry =
            RegistryWithVersions(
                "1.0",
                "2.0",
                "3.0");

        AssertAccepted(
            registry.TransitionLifecycle(
                new SchemaIdentity("schema.orders"),
                "1.0",
                SchemaLifecycleState.Active),
            "activate version");

        AssertAccepted(
            registry.DeclareCompatibility(
                Rule(
                    "schema.orders",
                    "2.0",
                    "1.0",
                    SchemaCompatibilityClassification.Backward)),
            "backward compatibility");

        AssertAccepted(
            registry.DeclareCompatibility(
                Rule(
                    "schema.orders",
                    "1.0",
                    "2.0",
                    SchemaCompatibilityClassification.Forward)),
            "forward compatibility");

        var before = registry.CaptureSnapshot();
        var replayed = new InMemorySchemaRegistry(before);
        var after = replayed.CaptureSnapshot();

        AssertEqual(
            before.Sha256,
            after.Sha256,
            "replayed snapshot digest must be deterministic");

        Assert(
            before.Revision == after.Revision,
            "replayed revision mismatch");

        Assert(
            before.Entries.SequenceEqual(after.Entries),
            "replayed entries mismatch");

        Assert(
            before.CompatibilityRules.SequenceEqual(after.CompatibilityRules),
            "replayed compatibility rules mismatch");
    }

    private static void RejectedOperationDoesNotChangeSnapshot()
    {
        var registry = RegistryWithVersions("1.0");
        var before = registry.CaptureSnapshot();

        AssertRejected(
            registry.Register(
                Definition(
                    "schema.orders",
                    "1.0",
                    "application.alpha",
                    DigestA)),
            "duplicate_schema_registration");

        var after = registry.CaptureSnapshot();

        AssertEqual(
            before.Sha256,
            after.Sha256,
            "rejected operation must not mutate snapshot");
        Assert(before.Revision == after.Revision, "rejected operation must not advance revision");
    }

    private static void ZeroApplicationNeutrality()
    {
        var registry = new InMemorySchemaRegistry();
        var snapshot = registry.CaptureSnapshot();

        Assert(snapshot.Entries.Count == 0, "zero-Application registry must be empty");
        Assert(snapshot.CompatibilityRules.Count == 0, "zero-Application rules must be empty");
        Assert(snapshot.Revision == 0, "zero-Application revision must be zero");
    }

    private static void TwoIndependentSchemaOwners()
    {
        var registry = new InMemorySchemaRegistry();

        AssertAccepted(
            registry.Register(
                Definition(
                    "schema.alpha.orders",
                    "1.0",
                    "application.alpha",
                    DigestA)),
            "alpha");

        AssertAccepted(
            registry.Register(
                Definition(
                    "schema.beta.orders",
                    "1.0",
                    "application.beta",
                    DigestB)),
            "beta");

        var snapshot = registry.CaptureSnapshot();

        Assert(snapshot.Entries.Count == 2, "two schemas expected");
        Assert(
            snapshot.Entries
                .Select(entry => entry.Definition.Owner.Value)
                .Distinct(StringComparer.Ordinal)
                .Count() == 2,
            "independent owners must remain distinct");
    }

    private static void PayloadMeaningRemainsOpaque()
    {
        var productionTypes =
            typeof(InMemorySchemaRegistry).Assembly
                .GetTypes()
                .Where(type =>
                    string.Equals(
                        type.Namespace,
                        "Foundation.SchemaRegistry",
                        StringComparison.Ordinal))
                .ToArray();

        var forbiddenPropertyNames =
            new HashSet<string>(
                new[]
                {
                    "Payload",
                    "BusinessMeaning",
                    "TradingMeaning",
                    "Market",
                    "Broker",
                    "Order",
                    "Position"
                },
                StringComparer.OrdinalIgnoreCase);

        foreach (var type in productionTypes)
        {
            foreach (var property in type.GetProperties(
                         BindingFlags.Instance |
                         BindingFlags.Public |
                         BindingFlags.NonPublic))
            {
                Assert(
                    !forbiddenPropertyNames.Contains(property.Name),
                    $"schema registry must not model Application payload meaning: {type.Name}.{property.Name}");
            }
        }
    }

    private static void RegistryDoesNotGrantAuthority()
    {
        var publicMethods =
            typeof(InMemorySchemaRegistry)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.DeclaringType == typeof(InMemorySchemaRegistry))
                .Select(method => method.Name)
                .ToArray();

        var forbidden =
            new[]
            {
                "Authorize",
                "GrantAuthority",
                "Execute",
                "Publish",
                "Subscribe",
                "Route",
                "Attach",
                "Detach"
            };

        foreach (var token in forbidden)
        {
            Assert(
                publicMethods.All(
                    method =>
                        !method.Contains(
                            token,
                            StringComparison.OrdinalIgnoreCase)),
                $"registry surface must not grant later-stage authority: {token}");
        }
    }

    private static void Wp01SchemaIdentityIsReused()
    {
        var definition =
            Definition(
                "schema.orders",
                "1.0",
                "application.alpha",
                DigestA);

        Assert(
            definition.SchemaId.GetType() == typeof(SchemaIdentity),
            "WP-02 must reuse WP-01 SchemaIdentity");
    }

    private static InMemorySchemaRegistry RegistryWithVersions(
        params string[] versions)
    {
        var registry = new InMemorySchemaRegistry();

        for (var index = 0; index < versions.Length; index++)
        {
            var digest =
                index switch
                {
                    0 => DigestA,
                    1 => DigestB,
                    _ => DigestC
                };

            AssertAccepted(
                registry.Register(
                    Definition(
                        "schema.orders",
                        versions[index],
                        "application.alpha",
                        digest)),
                $"register version {versions[index]}");
        }

        return registry;
    }

    private static SchemaDefinition Definition(
        string schemaId,
        string version,
        string owner,
        string digest) =>
        new(
            new SchemaIdentity(schemaId),
            version,
            new SchemaOwnerReference(owner),
            digest,
            new ProvenanceReference(
                $"evidence/schema/{schemaId}/{version}"));

    private static SchemaCompatibilityRule Rule(
        string schemaId,
        string fromVersion,
        string toVersion,
        SchemaCompatibilityClassification classification) =>
        new(
            new SchemaIdentity(schemaId),
            fromVersion,
            toVersion,
            classification,
            new ProvenanceReference(
                $"evidence/schema-rule/{schemaId}/{fromVersion}/{toVersion}"));

    private static void AssertLifecycle(
        InMemorySchemaRegistry registry,
        string version,
        SchemaLifecycleState expected)
    {
        var resolution =
            registry.Resolve(
                new SchemaIdentity("schema.orders"),
                version);

        Assert(resolution.Resolved, "schema must resolve");

        var entry =
            resolution.Entry ??
            throw new InvalidOperationException("resolved entry missing");

        Assert(
            entry.Lifecycle == expected,
            $"expected lifecycle {expected}, got {entry.Lifecycle}");
    }

    private static void AssertAccepted(
        SchemaRegistryOperationResult result,
        string label)
    {
        Assert(
            result.Accepted,
            $"{label} expected acceptance, got {result.Reason}");
    }

    private static void AssertRejected(
        SchemaRegistryOperationResult result,
        string expectedReason)
    {
        Assert(!result.Accepted, "operation should be rejected");
        AssertEqual(expectedReason, result.Reason, "rejection reason");
    }

    private static void ExpectThrows<TException>(Action action)
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

        throw new InvalidOperationException(
            $"Expected {typeof(TException).Name}.");
    }

    private static void Assert(
        bool condition,
        string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void AssertEqual(
        string expected,
        string actual,
        string label)
    {
        if (!string.Equals(
                expected,
                actual,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"{label}: expected '{expected}', actual '{actual}'.");
        }
    }
}
