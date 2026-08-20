using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using C = Falcon.FSATS.FSTSimA.Contracts;
using D = Falcon.FSATS.FSTSimA.Domain;

namespace Falcon.FSATS.FSTSimA.Application;

public enum DigitalCityScenarioClass
{
    Synthetic,
    HistoricalReplay,
    FaultInjected,
    CalibrationReplay
}

public sealed record CalibrationEvidenceReference(
    string EvidenceId,
    string EvidenceSha256,
    string SourceIdentity,
    DateTimeOffset ObservedAt)
{
    public bool IsValid(DateTimeOffset now)
        => !string.IsNullOrWhiteSpace(EvidenceId)
           && IsSha256(EvidenceSha256)
           && !string.IsNullOrWhiteSpace(SourceIdentity)
           && ObservedAt != default
           && ObservedAt <= now;

    private static bool IsSha256(string value)
        => !string.IsNullOrWhiteSpace(value)
           && value.Length == 64
           && value.All(Uri.IsHexDigit);
}

public sealed record DigitalCityScenario(
    string ScenarioId,
    int Seed,
    DigitalCityScenarioClass ScenarioClass,
    C.SimulationScope Scope,
    int TickCount,
    decimal StartPrice,
    string Regime,
    IReadOnlyList<D.FaultEvent> Faults,
    bool IndependentCalibrationEvidence,
    decimal FidelityScore)
{
    // Qualification-relevant calibration evidence is attributable data, not a caller assertion.
    // When IndependentCalibrationEvidence is true, this reference is mandatory and is included
    // in the deterministic artifact digest.
    public CalibrationEvidenceReference? CalibrationEvidence { get; init; }
}

public sealed record DigitalCityValidationResult(
    string ScenarioId,
    int Seed,
    DigitalCityScenarioClass ScenarioClass,
    string ScopeKey,
    string EvidenceId,
    string DeterministicDigestSha256,
    bool Reproducible,
    bool FaultOrderDeterministic,
    bool IndependentCalibrationEvidence,
    decimal FidelityScore,
    string Recommendation,
    bool OperationalTruth,
    bool GrantsRuntimeAuthority,
    bool GrantsPaperAuthority,
    bool GrantsLiveAuthority);

public sealed class DigitalCityValidationCoordinator
{
    private readonly D.SyntheticMarketGenerator _market;
    private readonly D.FaultInjector _faultInjector;
    private readonly D.ValidationAssessor _assessor;
    private readonly ISimulationEvidenceSink _evidence;

    public DigitalCityValidationCoordinator(
        D.SyntheticMarketGenerator market,
        D.FaultInjector faultInjector,
        D.ValidationAssessor assessor,
        ISimulationEvidenceSink evidence)
    {
        _market = market ?? throw new ArgumentNullException(nameof(market));
        _faultInjector = faultInjector ?? throw new ArgumentNullException(nameof(faultInjector));
        _assessor = assessor ?? throw new ArgumentNullException(nameof(assessor));
        _evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
    }

    public DigitalCityValidationResult Run(DigitalCityScenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        ValidateScenario(scenario);

        var normalizedScenarioId = scenario.ScenarioId.Trim();
        var normalizedRegime = scenario.Regime.Trim().ToUpperInvariant();
        var orderedFaults = _faultInjector.Order(scenario.Faults ?? Array.Empty<D.FaultEvent>());
        var now = DateTimeOffset.UtcNow;
        var calibrationEvidenceVerified = scenario.IndependentCalibrationEvidence
            && scenario.CalibrationEvidence is not null
            && scenario.CalibrationEvidence.IsValid(now);

        var runOne = BuildRunDigestInput(scenario, normalizedScenarioId, normalizedRegime, orderedFaults);
        var runTwo = BuildRunDigestInput(scenario, normalizedScenarioId, normalizedRegime, orderedFaults);
        var reproducible = string.Equals(runOne, runTwo, StringComparison.Ordinal);
        var digest = Sha256(runOne);

        var faultOrderDeterministic = orderedFaults.SequenceEqual(
            _faultInjector.Order((scenario.Faults ?? Array.Empty<D.FaultEvent>()).Reverse()));

        var assessment = _assessor.Assess(
            reproducible && faultOrderDeterministic,
            calibrationEvidenceVerified,
            scenario.FidelityScore);

        var evidenceId = $"digital-city:{scenario.Scope.CanonicalKey}:{Uri.EscapeDataString(normalizedScenarioId)}:{scenario.Seed}:{digest}";
        _evidence.Commit(evidenceId, normalizedScenarioId, scenario.Seed, digest);

        return new DigitalCityValidationResult(
            normalizedScenarioId,
            scenario.Seed,
            scenario.ScenarioClass,
            scenario.Scope.CanonicalKey,
            evidenceId,
            digest,
            reproducible,
            faultOrderDeterministic,
            calibrationEvidenceVerified,
            scenario.FidelityScore,
            assessment.Recommendation,
            OperationalTruth: false,
            GrantsRuntimeAuthority: false,
            GrantsPaperAuthority: false,
            GrantsLiveAuthority: false);
    }

    private string BuildRunDigestInput(
        DigitalCityScenario scenario,
        string normalizedScenarioId,
        string normalizedRegime,
        IReadOnlyList<D.FaultEvent> orderedFaults)
    {
        var ticks = _market.Generate(
            scenario.Seed,
            scenario.TickCount,
            scenario.StartPrice,
            new D.SimulationInstant(0),
            normalizedRegime);

        var builder = new StringBuilder();
        builder.Append(normalizedScenarioId).Append('|')
            .Append(scenario.Seed.ToString(CultureInfo.InvariantCulture)).Append('|')
            .Append(scenario.ScenarioClass).Append('|')
            .Append(scenario.Scope.CanonicalKey).Append('|')
            .Append(normalizedRegime).Append('|')
            .Append("FIDELITY:").Append(scenario.FidelityScore.ToString(CultureInfo.InvariantCulture)).Append('|')
            .Append("INDEPENDENT_CALIBRATION:").Append(scenario.IndependentCalibrationEvidence ? "1" : "0").Append('|');

        if (scenario.CalibrationEvidence is { } calibration)
        {
            builder.Append("CALIBRATION:")
                .Append(Uri.EscapeDataString(calibration.EvidenceId?.Trim() ?? string.Empty)).Append(':')
                .Append(calibration.EvidenceSha256?.Trim().ToUpperInvariant() ?? string.Empty).Append(':')
                .Append(Uri.EscapeDataString(calibration.SourceIdentity?.Trim() ?? string.Empty)).Append(':')
                .Append(calibration.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)).Append('|');
        }
        else
        {
            builder.Append("CALIBRATION:NONE|");
        }

        foreach (var tick in ticks)
        {
            builder.Append(tick.Time.Ticks.ToString(CultureInfo.InvariantCulture)).Append(':')
                .Append(tick.Price.ToString(CultureInfo.InvariantCulture)).Append(':')
                .Append(tick.Volume.ToString(CultureInfo.InvariantCulture)).Append(':')
                .Append(tick.Regime).Append(';');
        }

        builder.Append("FAULTS|");
        foreach (var fault in orderedFaults)
        {
            builder.Append(fault.At.Ticks.ToString(CultureInfo.InvariantCulture)).Append(':')
                .Append(fault.Type).Append(':')
                .Append(Uri.EscapeDataString(fault.Target ?? string.Empty)).Append(':')
                .Append(Uri.EscapeDataString(fault.Parameters ?? string.Empty)).Append(';');
        }

        return builder.ToString();
    }

    private static void ValidateScenario(DigitalCityScenario scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario.ScenarioId))
            throw new ArgumentException("P9_DIGITAL_CITY_SCENARIO_ID_REQUIRED", nameof(scenario));
        if (!Enum.IsDefined(scenario.ScenarioClass))
            throw new ArgumentOutOfRangeException(nameof(scenario), "P9_DIGITAL_CITY_SCENARIO_CLASS_INVALID");
        if (scenario.Scope is null)
            throw new ArgumentException("P9_DIGITAL_CITY_SCOPE_REQUIRED", nameof(scenario));
        if (scenario.TickCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(scenario), "P9_DIGITAL_CITY_TICK_COUNT_MUST_BE_POSITIVE");
        if (scenario.StartPrice <= 0m)
            throw new ArgumentOutOfRangeException(nameof(scenario), "P9_DIGITAL_CITY_START_PRICE_MUST_BE_POSITIVE");
        if (string.IsNullOrWhiteSpace(scenario.Regime))
            throw new ArgumentException("P9_DIGITAL_CITY_REGIME_REQUIRED", nameof(scenario));
        if (scenario.FidelityScore < 0m || scenario.FidelityScore > 1m)
            throw new ArgumentOutOfRangeException(nameof(scenario), "P9_DIGITAL_CITY_FIDELITY_OUT_OF_RANGE");
        if (scenario.IndependentCalibrationEvidence && scenario.CalibrationEvidence is null)
            throw new ArgumentException("P9_DIGITAL_CITY_CALIBRATION_EVIDENCE_REFERENCE_REQUIRED", nameof(scenario));
        if (scenario.CalibrationEvidence is not null && !scenario.CalibrationEvidence.IsValid(DateTimeOffset.UtcNow))
            throw new ArgumentException("P9_DIGITAL_CITY_CALIBRATION_EVIDENCE_INVALID", nameof(scenario));
    }

    private static string Sha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
