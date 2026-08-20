using System.Diagnostics;
using System.Security.Cryptography;

namespace Foundation.Enabling;

public sealed record RandomnessRequest(
    string RequestId,
    string Purpose,
    int Length,
    bool CallerSuppliedEntropy,
    FoundationAuthorityContext Context);

public sealed record RandomnessResult(
    FoundationDisposition Disposition,
    byte[]? Material,
    FoundationEvidence Evidence);

public interface IRandomnessProvider : IFoundationProvider
{
    RandomnessResult Produce(RandomnessRequest? request);
}

public sealed class WindowsCryptographicRandomnessProvider : FoundationProviderBase, IRandomnessProvider
{
    private static readonly string[] Purposes =
    [
        "identifier-randomness",
        "crypto-key",
        "crypto-nonce",
        "secret-material",
        "certificate-serial"
    ];

    public WindowsCryptographicRandomnessProvider()
        : base("ACT-RND-001", "FALCON-RANDOM-WINDOWS-CSPRNG-1")
    {
    }

    public RandomnessResult Produce(RandomnessRequest? request)
    {
        var requestId = request?.RequestId ?? string.Empty;
        if (request is null ||
            string.IsNullOrWhiteSpace(request.RequestId) ||
            string.IsNullOrWhiteSpace(request.Purpose) ||
            !IsUsable(request.Context) ||
            request.CallerSuppliedEntropy ||
            !Purposes.Contains(request.Purpose, StringComparer.Ordinal) ||
            request.Length is < 16 or > 4096)
        {
            return new(
                FoundationDisposition.Rejected,
                null,
                FoundationEvidence.Create(
                    requestId,
                    SubjectId,
                    "randomness",
                    FoundationDisposition.Rejected,
                    "randomness_request_rejected"));
        }

        var material = new byte[request.Length];
        RandomNumberGenerator.Fill(material);
        return new(
            FoundationDisposition.Succeeded,
            material,
            FoundationEvidence.Create(
                request.RequestId,
                SubjectId,
                "randomness",
                FoundationDisposition.Succeeded,
                "material_returned_to_bounded_consumer",
                ("profile", ProfileId),
                ("source", "OS_CSPRNG"),
                ("length", request.Length.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                ("classification", FoundationBoundary.Classification)));
    }
}

public enum ClockQuality
{
    Unverified,
    VerifiedLocalBuild,
    Conflicted,
    Stale,
    Unavailable
}

[Flags]
public enum ClockCapabilities
{
    None = 0,
    Utc = 1,
    Monotonic = 2,
    Uncertainty = 4,
    VerificationAge = 8
}

public sealed record TimeObservation(
    FoundationDisposition Disposition,
    DateTimeOffset? ObservedUtc,
    long? MonotonicTicks,
    string ClockSourceId,
    string RuntimeEpochId,
    ClockQuality Quality,
    long MaximumUncertaintyMicroseconds,
    DateTimeOffset LastVerification,
    TimeSpan VerificationAge,
    ClockCapabilities Capabilities,
    string EvidenceReference);

public interface IFoundationTimeProvider : IFoundationProvider
{
    TimeObservation Observe(FoundationAuthorityContext? context);
}

public sealed class WindowsFoundationTimeProvider : FoundationProviderBase, IFoundationTimeProvider
{
    public const long MaximumSupportedUncertaintyMicroseconds = 14_400_000_000L;

    private static readonly TimeSpan MaximumVerificationAge = TimeSpan.FromHours(4);
    private const string ClockSource = "windows-system-time";
    private const ClockCapabilities SupportedCapabilities =
        ClockCapabilities.Utc |
        ClockCapabilities.Monotonic |
        ClockCapabilities.Uncertainty |
        ClockCapabilities.VerificationAge;

    private readonly TimeProvider _timeProvider;
    private readonly string _runtimeEpochId;
    private readonly DateTimeOffset _verifiedAt;
    private readonly long _maximumUncertaintyMicroseconds;

    public WindowsFoundationTimeProvider(
        TimeProvider timeProvider,
        string runtimeEpochId,
        DateTimeOffset verifiedAt,
        long maximumUncertaintyMicroseconds)
        : base("ACT-TIM-001", "FALCON-TIME-WINDOWS-LOCAL-BUILD-1")
    {
        _timeProvider = timeProvider ?? throw new FoundationBoundaryException("time_provider_required");
        _runtimeEpochId = runtimeEpochId;
        _verifiedAt = verifiedAt;
        _maximumUncertaintyMicroseconds = maximumUncertaintyMicroseconds;
    }

    public TimeObservation Observe(FoundationAuthorityContext? context)
    {
        var now = _timeProvider.GetUtcNow();
        var age = now - _verifiedAt;
        var quality = DetermineQuality(now, age);
        var usable = IsUsable(context) && quality == ClockQuality.VerifiedLocalBuild;

        return new(
            usable ? FoundationDisposition.Succeeded : FoundationDisposition.Rejected,
            usable ? now : null,
            usable ? _timeProvider.GetTimestamp() : null,
            ClockSource,
            _runtimeEpochId ?? string.Empty,
            quality,
            _maximumUncertaintyMicroseconds,
            _verifiedAt,
            age,
            SupportedCapabilities,
            context?.EvidenceSetId ?? string.Empty);
    }

    public static bool CanCompareMonotonic(TimeObservation? first, TimeObservation? second) =>
        first is not null &&
        second is not null &&
        first.Disposition == FoundationDisposition.Succeeded &&
        second.Disposition == FoundationDisposition.Succeeded &&
        first.MonotonicTicks.HasValue &&
        second.MonotonicTicks.HasValue &&
        !string.IsNullOrWhiteSpace(first.ClockSourceId) &&
        !string.IsNullOrWhiteSpace(first.RuntimeEpochId) &&
        StringComparer.Ordinal.Equals(first.ClockSourceId, second.ClockSourceId) &&
        StringComparer.Ordinal.Equals(first.RuntimeEpochId, second.RuntimeEpochId);

    public static bool IsDefinitelyBefore(TimeObservation? observation, DateTimeOffset boundary)
    {
        if (observation is null ||
            observation.Disposition != FoundationDisposition.Succeeded ||
            !observation.ObservedUtc.HasValue ||
            observation.MaximumUncertaintyMicroseconds is < 0 or > MaximumSupportedUncertaintyMicroseconds)
        {
            return false;
        }

        try
        {
            var uncertaintyTicks = checked(observation.MaximumUncertaintyMicroseconds * 10);
            var latest = observation.ObservedUtc.Value.AddTicks(uncertaintyTicks);
            return latest < boundary;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    private ClockQuality DetermineQuality(DateTimeOffset now, TimeSpan age)
    {
        if (string.IsNullOrWhiteSpace(_runtimeEpochId) ||
            !_runtimeEpochId.StartsWith("epoch:", StringComparison.Ordinal) ||
            _verifiedAt == default ||
            _maximumUncertaintyMicroseconds is < 0 or > MaximumSupportedUncertaintyMicroseconds)
        {
            return ClockQuality.Unverified;
        }

        if (_verifiedAt > now || age < TimeSpan.Zero)
        {
            return ClockQuality.Conflicted;
        }

        return age <= MaximumVerificationAge
            ? ClockQuality.VerifiedLocalBuild
            : ClockQuality.Stale;
    }
}

public sealed record IdentifierRequest(
    string RequestId,
    string IdentifierClass,
    string LogicalSubject,
    string ExposureBoundary,
    FoundationAuthorityContext Context);

public sealed record IdentifierResult(
    FoundationDisposition Disposition,
    string? Identifier,
    string AttemptIdentifier,
    string Reason);

public interface IIdentifierProvider : IFoundationProvider
{
    IdentifierResult Issue(IdentifierRequest? request);
}

internal sealed record IdentifierContinuityEntry(
    string Identifier,
    string IdentifierClass,
    string LogicalSubject,
    string ExposureBoundary);

public sealed class FoundationIdentifierProvider(
    IFoundationTimeProvider time,
    IRandomnessProvider randomness) : FoundationProviderBase("ACT-IDN-001", "FALCON-ID-UUID7-1"), IIdentifierProvider
{
    private static readonly string[] Classes =
    [
        "falcon.foundation.operation",
        "falcon.foundation.evidence",
        "falcon.foundation.attempt",
        "falcon.foundation.runtime-epoch"
    ];

    private readonly object _sync = new();
    private readonly Dictionary<string, IdentifierContinuityEntry> _continuity = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _subjects = new(StringComparer.Ordinal);
    private readonly IFoundationTimeProvider _time = time ?? throw new FoundationBoundaryException("identifier_time_provider_required");
    private readonly IRandomnessProvider _randomness = randomness ?? throw new FoundationBoundaryException("identifier_randomness_provider_required");

    public IdentifierResult Issue(IdentifierRequest? request)
    {
        var attempt = Guid.NewGuid().ToString("D");
        if (request is null ||
            string.IsNullOrWhiteSpace(request.RequestId) ||
            string.IsNullOrWhiteSpace(request.IdentifierClass) ||
            string.IsNullOrWhiteSpace(request.LogicalSubject) ||
            string.IsNullOrWhiteSpace(request.ExposureBoundary) ||
            !IsUsable(request.Context) ||
            !Classes.Contains(request.IdentifierClass, StringComparer.Ordinal) ||
            !StringComparer.Ordinal.Equals(request.ExposureBoundary, "internal-foundation"))
        {
            return new(FoundationDisposition.Rejected, null, attempt, "identifier_request_rejected");
        }

        lock (_sync)
        {
            if (_continuity.TryGetValue(request.RequestId, out var existing))
            {
                if (!StringComparer.Ordinal.Equals(existing.IdentifierClass, request.IdentifierClass) ||
                    !StringComparer.Ordinal.Equals(existing.LogicalSubject, request.LogicalSubject) ||
                    !StringComparer.Ordinal.Equals(existing.ExposureBoundary, request.ExposureBoundary))
                {
                    return new(FoundationDisposition.Rejected, null, attempt, "identifier_request_identity_mismatch");
                }

                return new(FoundationDisposition.Succeeded, existing.Identifier, attempt, "identity_continuity");
            }

            var observed = _time.Observe(request.Context);
            var random = _randomness.Produce(new(
                request.RequestId + ":random",
                "identifier-randomness",
                16,
                false,
                request.Context));
            if (observed.Disposition != FoundationDisposition.Succeeded ||
                !observed.ObservedUtc.HasValue ||
                random.Disposition != FoundationDisposition.Succeeded ||
                random.Material is null ||
                random.Material.Length < 10)
            {
                return new(FoundationDisposition.Rejected, null, attempt, "identifier_dependency_rejected");
            }

            var identifier = CreateUuid7(observed.ObservedUtc.Value, random.Material);
            if (_subjects.TryGetValue(identifier, out var existingSubject) &&
                !StringComparer.Ordinal.Equals(existingSubject, request.LogicalSubject))
            {
                return new(FoundationDisposition.Rejected, null, attempt, "identity_collision");
            }

            _subjects[identifier] = request.LogicalSubject;
            _continuity.Add(
                request.RequestId,
                new IdentifierContinuityEntry(
                    identifier,
                    request.IdentifierClass,
                    request.LogicalSubject,
                    request.ExposureBoundary));

            return new(FoundationDisposition.Succeeded, identifier, attempt, "issued");
        }
    }

    private static string CreateUuid7(DateTimeOffset timestamp, byte[] randomness)
    {
        var unixMilliseconds = timestamp.ToUnixTimeMilliseconds();
        Span<byte> bytes = stackalloc byte[16];
        bytes[0] = (byte)(unixMilliseconds >> 40);
        bytes[1] = (byte)(unixMilliseconds >> 32);
        bytes[2] = (byte)(unixMilliseconds >> 24);
        bytes[3] = (byte)(unixMilliseconds >> 16);
        bytes[4] = (byte)(unixMilliseconds >> 8);
        bytes[5] = (byte)unixMilliseconds;
        randomness.AsSpan(0, 10).CopyTo(bytes[6..]);
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x70);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        var hex = Convert.ToHexString(bytes).ToLowerInvariant();
        return string.Concat(
            hex[..8], "-",
            hex.Substring(8, 4), "-",
            hex.Substring(12, 4), "-",
            hex.Substring(16, 4), "-",
            hex.Substring(20, 12));
    }
}
