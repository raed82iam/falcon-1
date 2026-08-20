using System.Collections.Concurrent;

namespace Falcon.Stage0B.Candidates;

public interface IControlledIdentifierMaterial
{
    DateTimeOffset UtcNow { get; }
    void Fill(Span<byte> destination);
    bool Available { get; }
}

public sealed record IdentifierRequest(
    string RequestId,
    string IdentifierClass,
    string Profile,
    string LogicalSubject,
    string Scope,
    string Environment,
    string ExposureBoundary,
    bool ContinuityRequired,
    bool ContainsCallerGenerationMaterial,
    CandidateContext Context);

public sealed record IdentifierCandidateResult(
    CandidateDisposition Disposition,
    string? Identifier,
    string? AttemptIdentifier,
    string Classification,
    bool Operational,
    string Reason,
    CandidateEvidence Evidence);

public sealed class IdentifierProviderCandidate(IControlledIdentifierMaterial material)
    : CandidateProviderBase("CND-IDN-001")
{
    private static readonly string[] Classes =
    [
        "falcon.foundation.operation",
        "falcon.foundation.evidence",
        "falcon.foundation.attempt",
        "falcon.foundation.runtime-epoch"
    ];

    private static readonly string[] Profiles = ["FALCON-ID-CANDIDATE-UUID7"];
    private readonly ConcurrentDictionary<string, (string Subject, Guid Id)> _continuity = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<Guid, string> _subjects = new();

    public IdentifierCandidateResult Issue(IdentifierRequest request)
    {
        var evidenceId = $"idn-evidence:{request.RequestId}";
        if (!request.Context.IsAuthorized)
        {
            return Rejected(request, evidenceId, "authority_or_context_rejected");
        }

        if (!IsKnownToken(request.IdentifierClass, Classes) ||
            !IsKnownToken(request.Profile, Profiles) ||
            !StringComparer.Ordinal.Equals(request.Environment, CandidateContext.ApprovedEnvironment))
        {
            return Rejected(request, evidenceId, "unknown_class_profile_or_environment");
        }

        if (request.ContainsCallerGenerationMaterial)
        {
            return Rejected(request, evidenceId, "caller_generation_material_prohibited");
        }

        if (!StringComparer.Ordinal.Equals(request.ExposureBoundary, "internal-candidate"))
        {
            return Rejected(request, evidenceId, "exposure_not_approved");
        }

        if (!material.Available)
        {
            return Rejected(request, evidenceId, "time_or_randomness_unavailable");
        }

        var attempt = GenerateUuid7();
        Guid identifier;
        if (request.ContinuityRequired &&
            _continuity.TryGetValue(request.RequestId, out var existing))
        {
            if (!StringComparer.Ordinal.Equals(existing.Subject, request.LogicalSubject))
            {
                return Rejected(request, evidenceId, "identity_collision");
            }

            identifier = existing.Id;
        }
        else
        {
            identifier = GenerateUuid7();
            if (_subjects.TryGetValue(identifier, out var priorSubject) &&
                !StringComparer.Ordinal.Equals(priorSubject, request.LogicalSubject))
            {
                return Rejected(request, evidenceId, "identity_collision");
            }

            _subjects[identifier] = request.LogicalSubject;
            if (request.ContinuityRequired)
            {
                _continuity[request.RequestId] = (request.LogicalSubject, identifier);
            }
        }

        var canonical = FalconCanonicalEncoding.Identifier(identifier);
        var attemptCanonical = FalconCanonicalEncoding.Identifier(attempt);
        var evidence = Succeed(
            evidenceId,
            "identifier.issue",
            ("class", request.IdentifierClass),
            ("profile", request.Profile),
            ("scope", request.Scope),
            ("classification", "CANDIDATE"),
            ("canonical_identifier", canonical),
            ("attempt_identifier", attemptCanonical),
            ("operational", "false"));
        return new IdentifierCandidateResult(
            CandidateDisposition.Succeeded,
            canonical,
            attemptCanonical,
            "CANDIDATE",
            false,
            "candidate_observation_only",
            evidence);
    }

    private IdentifierCandidateResult Rejected(
        IdentifierRequest request,
        string evidenceId,
        string reason)
    {
        var evidence = Reject(
            evidenceId,
            "identifier.issue",
            reason,
            ("class", request.IdentifierClass),
            ("profile", request.Profile),
            ("operational", "false"));
        return new IdentifierCandidateResult(
            CandidateDisposition.Rejected,
            null,
            null,
            "CANDIDATE",
            false,
            reason,
            evidence);
    }

    private Guid GenerateUuid7()
    {
        Span<byte> bytes = stackalloc byte[16];
        var milliseconds = material.UtcNow.ToUnixTimeMilliseconds();
        bytes[0] = (byte)(milliseconds >> 40);
        bytes[1] = (byte)(milliseconds >> 32);
        bytes[2] = (byte)(milliseconds >> 24);
        bytes[3] = (byte)(milliseconds >> 16);
        bytes[4] = (byte)(milliseconds >> 8);
        bytes[5] = (byte)milliseconds;
        material.Fill(bytes[6..]);
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x70);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        var result = new Guid(bytes, bigEndian: true);
        if (result == Guid.Empty)
        {
            throw new CandidateBoundaryException("nil_identifier");
        }

        return result;
    }
}

public enum ClockQuality
{
    Unverified,
    Verified,
    Degraded,
    Uncertain,
    Failed
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

public sealed record ControlledClockSample(
    DateTimeOffset ObservedUtc,
    ulong MonotonicMicroseconds,
    string ClockSourceId,
    string RuntimeEpochId,
    ulong ResolutionMicroseconds,
    ulong MaximumUncertaintyMicroseconds,
    DateTimeOffset LastVerification,
    ClockQuality AssessedQuality,
    ClockCapabilities Capabilities,
    bool Available,
    bool Discontinuity,
    bool SourceConflict);

public interface IControlledClockSource
{
    ControlledClockSample Read();
}

public sealed record TimeObservationRequest(
    string RequestId,
    ClockQuality MinimumQuality,
    ulong MaximumAcceptedUncertaintyMicroseconds,
    TimeSpan MaximumVerificationAge,
    ClockCapabilities RequiredCapabilities,
    CandidateContext Context);

public sealed record TimeCandidateObservation(
    CandidateDisposition Disposition,
    string? ObservedUtc,
    ulong? MonotonicMicroseconds,
    string? ClockSourceId,
    string? RuntimeEpochId,
    ClockQuality Quality,
    ulong MaximumUncertaintyMicroseconds,
    string? EarliestPossibleTime,
    string? LatestPossibleTime,
    ClockCapabilities Capabilities,
    string Classification,
    bool Operational,
    string Reason,
    CandidateEvidence Evidence);

public sealed class TimeProviderCandidate(IControlledClockSource source)
    : CandidateProviderBase("CND-TIM-001")
{
    public TimeCandidateObservation Observe(TimeObservationRequest request)
    {
        var evidenceId = $"tim-evidence:{request.RequestId}";
        if (!request.Context.IsAuthorized)
        {
            return Rejected(evidenceId, "authority_or_context_rejected");
        }

        ControlledClockSample sample;
        try
        {
            sample = source.Read();
        }
        catch (Exception exception) when (exception is not CandidateBoundaryException)
        {
            return Rejected(evidenceId, "controlled_source_failed");
        }

        if (!sample.Available)
        {
            return Rejected(evidenceId, "controlled_source_unavailable");
        }

        var quality = EvaluateQuality(sample, request.MaximumVerificationAge);
        if (QualityRank(quality) < QualityRank(request.MinimumQuality) ||
            sample.MaximumUncertaintyMicroseconds > request.MaximumAcceptedUncertaintyMicroseconds ||
            (sample.Capabilities & request.RequiredCapabilities) != request.RequiredCapabilities)
        {
            return Rejected(evidenceId, "quality_uncertainty_or_capability_rejected", quality);
        }

        var observed = sample.ObservedUtc.ToUniversalTime();
        var uncertainty = TimeSpan.FromTicks(checked((long)sample.MaximumUncertaintyMicroseconds * 10));
        var canonical = FalconCanonicalEncoding.Timestamp(observed);
        var earliest = FalconCanonicalEncoding.Timestamp(observed - uncertainty);
        var latest = FalconCanonicalEncoding.Timestamp(observed + uncertainty);
        var evidence = Succeed(
            evidenceId,
            "time.observe",
            ("source", sample.ClockSourceId),
            ("runtime_epoch", sample.RuntimeEpochId),
            ("quality", quality.ToString().ToUpperInvariant()),
            ("uncertainty_microseconds", sample.MaximumUncertaintyMicroseconds.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("classification", "CANDIDATE"),
            ("operational", "false"));
        return new TimeCandidateObservation(
            CandidateDisposition.Succeeded,
            canonical,
            sample.MonotonicMicroseconds,
            sample.ClockSourceId,
            sample.RuntimeEpochId,
            quality,
            sample.MaximumUncertaintyMicroseconds,
            earliest,
            latest,
            sample.Capabilities,
            "CANDIDATE",
            false,
            "candidate_observation_only",
            evidence);
    }

    public static bool CanCompareMonotonic(TimeCandidateObservation first, TimeCandidateObservation second) =>
        first.Disposition == CandidateDisposition.Succeeded &&
        second.Disposition == CandidateDisposition.Succeeded &&
        first.MonotonicMicroseconds.HasValue &&
        second.MonotonicMicroseconds.HasValue &&
        StringComparer.Ordinal.Equals(first.RuntimeEpochId, second.RuntimeEpochId) &&
        StringComparer.Ordinal.Equals(first.ClockSourceId, second.ClockSourceId) &&
        first.Capabilities.HasFlag(ClockCapabilities.Monotonic) &&
        second.Capabilities.HasFlag(ClockCapabilities.Monotonic);

    public static bool IsDefinitelyBefore(TimeCandidateObservation observation, DateTimeOffset boundary)
    {
        if (observation.Disposition != CandidateDisposition.Succeeded ||
            observation.LatestPossibleTime is null)
        {
            return false;
        }

        return DateTimeOffset.ParseExact(
            observation.LatestPossibleTime,
            "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal) < boundary;
    }

    private static ClockQuality EvaluateQuality(ControlledClockSample sample, TimeSpan maximumVerificationAge)
    {
        if (sample.Discontinuity || sample.SourceConflict)
        {
            return ClockQuality.Uncertain;
        }

        var verificationAge = sample.ObservedUtc - sample.LastVerification;
        if (verificationAge < TimeSpan.Zero || verificationAge > maximumVerificationAge)
        {
            return ClockQuality.Degraded;
        }

        return sample.AssessedQuality;
    }

    private static int QualityRank(ClockQuality quality) =>
        quality switch
        {
            ClockQuality.Verified => 4,
            ClockQuality.Unverified => 3,
            ClockQuality.Degraded => 2,
            ClockQuality.Uncertain => 1,
            ClockQuality.Failed => 0,
            _ => 0
        };

    private TimeCandidateObservation Rejected(
        string evidenceId,
        string reason,
        ClockQuality quality = ClockQuality.Failed)
    {
        var evidence = Reject(
            evidenceId,
            "time.observe",
            reason,
            ("quality", quality.ToString().ToUpperInvariant()),
            ("classification", "CANDIDATE"),
            ("operational", "false"));
        return new TimeCandidateObservation(
            CandidateDisposition.Rejected,
            null,
            null,
            null,
            null,
            quality,
            0,
            null,
            null,
            ClockCapabilities.None,
            "CANDIDATE",
            false,
            reason,
            evidence);
    }
}
