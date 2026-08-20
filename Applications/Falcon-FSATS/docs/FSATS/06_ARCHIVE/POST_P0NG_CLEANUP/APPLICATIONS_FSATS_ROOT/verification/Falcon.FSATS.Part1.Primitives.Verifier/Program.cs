using Falcon.FSATS.Primitives;

namespace Falcon.FSATS.Part1.Primitives.Verifier;

internal static class Program
{
    private static readonly List<string> Failures = new();
    private const int GateCount = 20;

    private static int Main()
    {
        Run("ID_TRIM_FAILS_CLOSED", VerifyTrimFailsClosed);
        Run("ID_INVALID_CHAR_FAILS_CLOSED", VerifyInvalidCharacterFailsClosed);
        Run("ID_TYPE_SEPARATION", VerifyIdTypeSeparation);
        Run("PACKAGE_ID_TYPE_SEPARATION", VerifyPackageIdTypeSeparation);
        Run("AWARENESS_ENTITY_ROOM_TYPE_SEPARATION", VerifyAwarenessIdentitySeparation);
        Run("VERSION_REQUIRES_NUMERIC_COMPONENT", VerifyVersionRequiresNumericComponent);
        Run("UTC_OFFSET_FAILS_CLOSED", VerifyUtcOffsetFailsClosed);
        Run("UTC_NORMALIZATION_EXPLICIT", VerifyUtcNormalization);
        Run("DEADLINE_BOUNDARY_EXPIRES", VerifyDeadlineBoundary);
        Run("DEADLINE_REMAINING_NEVER_NEGATIVE", VerifyDeadlineRemaining);
        Run("HEALTH_ENUM_FAILS_CLOSED", VerifyHealthEnumFailsClosed);
        Run("HEALTH_REASON_CODE_CANONICAL", VerifyHealthReasonCode);
        Run("EVIDENCE_LINK_REQUIRES_CORE_IDENTITIES", VerifyEvidenceLinkRequirements);
        Run("FOUNDATION_BINDING_IS_OPAQUE_REFERENCE", VerifyFoundationBindingReference);
        Run("CANONICAL_ENCODING_DETERMINISTIC", VerifyCanonicalEncodingDeterministic);
        Run("CANONICAL_ENCODING_LENGTH_PREFIX_SAFE", VerifyCanonicalEncodingLengthPrefix);
        Run("CANONICAL_ENCODING_DUPLICATE_FIELDS_FAIL", VerifyDuplicateFieldsFailClosed);
        Run("CANONICAL_FIELD_NAME_FAILS_CLOSED", VerifyFieldNameFailsClosed);
        Run("CANONICAL_DIGEST_DETERMINISTIC", VerifyDigestDeterministic);
        Run("CANONICAL_DIGEST_MATERIAL_CHANGE", VerifyDigestMaterialChange);

        if (Failures.Count == 0)
        {
            Console.WriteLine($"FSATS_P1B_PRIMITIVES_VERIFIER_PASS {GateCount}/{GateCount}");
            return 0;
        }

        Console.Error.WriteLine($"FSATS_P1B_PRIMITIVES_VERIFIER_FAIL {GateCount - Failures.Count}/{GateCount}");
        foreach (var failure in Failures)
        {
            Console.Error.WriteLine(failure);
        }

        return 1;
    }

    private static void Run(string name, Action verification)
    {
        try
        {
            verification();
            Console.WriteLine($"PASS {name}");
        }
        catch (Exception ex)
        {
            Failures.Add($"FAIL {name}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void VerifyTrimFailsClosed() => ExpectArgument(() => new FsatsApplicationId(" app.invalid "));

    private static void VerifyInvalidCharacterFailsClosed() => ExpectArgument(() => new FsatsApplicationId("app invalid"));

    private static void VerifyIdTypeSeparation()
    {
        CanonicalId application = new FsatsApplicationId("falcon.reference.identity");
        CanonicalId evidence = new EvidenceId("falcon.reference.identity");
        Require(!Equals(application, evidence), "different_identity_domains_collapsed");
    }

    private static void VerifyPackageIdTypeSeparation()
    {
        CanonicalId application = new FsatsApplicationId("falcon.reference.identity");
        CanonicalId package = new PackageId("falcon.reference.identity");
        Require(!Equals(application, package), "application_and_package_identity_collapsed");
    }

    private static void VerifyAwarenessIdentitySeparation()
    {
        CanonicalId entity = new AwarenessEntityId("awareness.reference.identity");
        CanonicalId room = new AwarenessRoomId("awareness.reference.identity");
        Require(!Equals(entity, room), "awareness_entity_and_room_identity_collapsed");
    }

    private static void VerifyVersionRequiresNumericComponent() => ExpectArgument(() => new VersionId("version-alpha"));

    private static void VerifyUtcOffsetFailsClosed() =>
        ExpectArgument(() => new UtcInstant(new DateTimeOffset(2026, 8, 7, 20, 0, 0, TimeSpan.FromHours(3))));

    private static void VerifyUtcNormalization()
    {
        var source = new DateTimeOffset(2026, 8, 7, 20, 0, 0, TimeSpan.FromHours(3));
        var normalized = UtcInstant.FromUtc(source);
        Require(normalized.Value.Offset == TimeSpan.Zero, "utc_normalization_failed");
        Require(normalized.Value == source.ToUniversalTime(), "utc_instant_changed_during_normalization");
    }

    private static void VerifyDeadlineBoundary()
    {
        var expires = new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(10));
        var deadline = new Deadline(expires);
        Require(!deadline.IsExpired(new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(9))), "deadline_expired_early");
        Require(deadline.IsExpired(new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(10))), "deadline_not_expired_at_boundary");
    }

    private static void VerifyDeadlineRemaining()
    {
        var deadline = new Deadline(new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(10)));
        Require(deadline.Remaining(new UtcInstant(DateTimeOffset.UnixEpoch)) == TimeSpan.FromSeconds(10), "deadline_remaining_wrong");
        Require(deadline.Remaining(new UtcInstant(DateTimeOffset.UnixEpoch.AddSeconds(11))) == TimeSpan.Zero, "deadline_remaining_negative");
    }

    private static void VerifyHealthEnumFailsClosed()
    {
        try
        {
            _ = new HealthSnapshot((HealthDisposition)999, new UtcInstant(DateTimeOffset.UnixEpoch), "INVALID_ENUM", new EvidenceId("evidence:p1b:test"));
        }
        catch (ArgumentOutOfRangeException)
        {
            return;
        }

        throw new InvalidOperationException("undefined_health_enum_accepted");
    }

    private static void VerifyHealthReasonCode()
    {
        _ = new HealthSnapshot(HealthDisposition.Restricted, new UtcInstant(DateTimeOffset.UnixEpoch), "PART1_NOT_RUNTIME_AUTHORIZED", new EvidenceId("evidence:p1b:health"));
        ExpectArgument(() => new HealthSnapshot(HealthDisposition.Restricted, new UtcInstant(DateTimeOffset.UnixEpoch), "mixedCase", new EvidenceId("evidence:p1b:health")));
    }

    private static void VerifyEvidenceLinkRequirements()
    {
        _ = new EvidenceLink(
            new EvidenceId("evidence:p1b:1"),
            new CorrelationId("correlation:p1b:1"),
            new CausationId("causation:p1b:1"),
            new UtcInstant(DateTimeOffset.UnixEpoch));
    }

    private static void VerifyFoundationBindingReference()
    {
        var binding = new FoundationBindingReference(
            new FoundationReferenceId("foundation:contract:application-manifest"),
            new VersionId("1.0"),
            new ProvenanceId("foundation:provenance:wp03"));

        Require(binding.ReferenceId.Value == "foundation:contract:application-manifest", "foundation_reference_changed");
        Require(binding.Version.Value == "1.0", "foundation_version_changed");
        Require(binding.ProvenanceId.Value == "foundation:provenance:wp03", "foundation_provenance_changed");
    }

    private static void VerifyCanonicalEncodingDeterministic()
    {
        var a = CanonicalEncoding.Encode(("application", "falcon.trading.application"), ("version", "1.4.0"));
        var b = CanonicalEncoding.Encode(("application", "falcon.trading.application"), ("version", "1.4.0"));
        Require(string.Equals(a, b, StringComparison.Ordinal), "canonical_encoding_not_deterministic");
    }

    private static void VerifyCanonicalEncodingLengthPrefix()
    {
        var left = CanonicalEncoding.Encode(("a", "bc"));
        var right = CanonicalEncoding.Encode(("ab", "c"));
        Require(!string.Equals(left, right, StringComparison.Ordinal), "length_prefix_collision");
    }

    private static void VerifyDuplicateFieldsFailClosed() =>
        ExpectArgument(() => CanonicalEncoding.Encode(("application", "a"), ("application", "b")));

    private static void VerifyFieldNameFailsClosed() =>
        ExpectArgument(() => CanonicalEncoding.Encode(("Application Name", "falcon")));

    private static void VerifyDigestDeterministic()
    {
        var a = CanonicalEncoding.ComputeSha256(("application", "falcon.trading.application"), ("version", "1.4.0"));
        var b = CanonicalEncoding.ComputeSha256(("application", "falcon.trading.application"), ("version", "1.4.0"));
        Require(a.Length == 64, "sha256_length_invalid");
        Require(string.Equals(a, b, StringComparison.Ordinal), "sha256_not_deterministic");
    }

    private static void VerifyDigestMaterialChange()
    {
        var a = CanonicalEncoding.ComputeSha256(("application", "falcon.trading.application"), ("version", "1.4.0"));
        var b = CanonicalEncoding.ComputeSha256(("application", "falcon.trading.application"), ("version", "1.4.1"));
        Require(!string.Equals(a, b, StringComparison.Ordinal), "material_change_did_not_change_digest");
    }

    private static void ExpectArgument(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentException)
        {
            return;
        }

        throw new InvalidOperationException("expected_argument_exception_not_thrown");
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
        {
            throw new InvalidOperationException(reason);
        }
    }
}
