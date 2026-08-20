using System;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.HealthFitness;

public enum HealthState
{
    Healthy = 1,
    Degraded = 2,
    Unhealthy = 3,
    Unknown = 4,
    NotApplicable = 5
}

public enum TechnicalFitnessState
{
    Fit = 1,
    FitWithConstraints = 2,
    Degraded = 3,
    Unknown = 4,
    Unavailable = 5,
    IntegrityFailure = 6,
    IsolationRequired = 7,
    RecoveryRequired = 8,
    NotFit = 9
}

public enum FitnessProjectionResult
{
    Fit = 1,
    Restricted = 2,
    NotFit = 3
}

public enum EvidenceQuality
{
    Sufficient = 1,
    Limited = 2,
    Insufficient = 3,
    Invalid = 4
}

public sealed record CanonicalHealthFitnessAssessment(
    string AssessmentId,
    string SubjectId,
    string Capability,
    string RequestedAuthorityLevel,
    HealthState HealthState,
    TechnicalFitnessState TechnicalFitnessState,
    FitnessProjectionResult FitnessResult,
    string Scope,
    string EvidenceReference,
    string SelfModelReference,
    EvidenceQuality EvidenceQuality,
    string Confidence,
    string Unknowns,
    string Contradictions,
    string Constraints,
    string Reason,
    string RuleId,
    string RuleVersion,
    DateTimeOffset ObservationTime,
    DateTimeOffset AssessmentTime,
    DateTimeOffset EffectiveTime,
    DateTimeOffset Expiry)
{
    public string Identity => HealthFitnessAssessmentIdentity.Compute(this);
}

public static class HealthFitnessPrimitiveValidator
{
    public static ValidationOutcome Validate(CanonicalHealthFitnessAssessment? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("Stage 7 health/fitness assessment missing");
        }

        if (!Enum.IsDefined(value.HealthState) ||
            !Enum.IsDefined(value.TechnicalFitnessState) ||
            !Enum.IsDefined(value.FitnessResult) ||
            !Enum.IsDefined(value.EvidenceQuality))
        {
            return ValidationOutcome.Failed("Stage 7 health/fitness enum rejected");
        }

        if (!HealthFitnessContractV12.IsCanonicalIdentifier(value.AssessmentId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.SubjectId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.Capability) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.RequestedAuthorityLevel) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.Scope) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.EvidenceReference) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.SelfModelReference) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.RuleId) ||
            !HealthFitnessContractV12.IsCanonicalIdentifier(value.RuleVersion))
        {
            return ValidationOutcome.Failed("Stage 7 health/fitness canonical identity rejected");
        }

        if (string.IsNullOrWhiteSpace(value.Confidence) ||
            string.IsNullOrWhiteSpace(value.Unknowns) ||
            string.IsNullOrWhiteSpace(value.Contradictions) ||
            string.IsNullOrWhiteSpace(value.Constraints) ||
            string.IsNullOrWhiteSpace(value.Reason))
        {
            return ValidationOutcome.Failed("Stage 7 health/fitness required detail missing");
        }

        if (value.ObservationTime == default ||
            value.AssessmentTime == default ||
            value.EffectiveTime == default ||
            value.Expiry == default ||
            value.ObservationTime > value.AssessmentTime ||
            value.AssessmentTime > value.EffectiveTime ||
            value.Expiry <= value.EffectiveTime)
        {
            return ValidationOutcome.Failed("Stage 7 health/fitness time order rejected");
        }

        return ValidationOutcome.Passed("Stage 7 health/fitness primitives valid");
    }
}

public static class HealthFitnessContractProjection
{
    public static HealthFitnessAssessmentV12 ToContractV12(CanonicalHealthFitnessAssessment value)
    {
        var validation = HealthFitnessPrimitiveValidator.Validate(value);
        if (validation.Result != ValidationResult.Pass)
        {
            throw new ArgumentException(validation.Message, nameof(value));
        }

        var projected = new HealthFitnessAssessmentV12(
            value.AssessmentId,
            HealthFitnessContractV12.Version,
            value.SubjectId,
            value.Capability,
            value.RequestedAuthorityLevel,
            ToContract(value.HealthState),
            ToContract(value.TechnicalFitnessState),
            ToContract(value.FitnessResult),
            value.Scope,
            value.EvidenceReference,
            value.SelfModelReference,
            ToContract(value.EvidenceQuality),
            value.Confidence,
            value.Unknowns,
            value.Contradictions,
            value.Constraints,
            value.Reason,
            value.RuleId,
            value.RuleVersion,
            value.ObservationTime,
            value.AssessmentTime,
            value.EffectiveTime,
            value.Expiry);

        var contractValidation = HealthFitnessV12Validator.Validate(projected);
        if (contractValidation.Result != ValidationResult.Pass)
        {
            throw new InvalidOperationException(contractValidation.Message);
        }

        return projected;
    }

    public static string ToContract(HealthState value) => value switch
    {
        HealthState.Healthy => "HEALTHY",
        HealthState.Degraded => "DEGRADED",
        HealthState.Unhealthy => "UNHEALTHY",
        HealthState.Unknown => "UNKNOWN",
        HealthState.NotApplicable => "NOT_APPLICABLE",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string ToContract(TechnicalFitnessState value) => value switch
    {
        TechnicalFitnessState.Fit => "FIT",
        TechnicalFitnessState.FitWithConstraints => "FIT_WITH_CONSTRAINTS",
        TechnicalFitnessState.Degraded => "DEGRADED",
        TechnicalFitnessState.Unknown => "UNKNOWN",
        TechnicalFitnessState.Unavailable => "UNAVAILABLE",
        TechnicalFitnessState.IntegrityFailure => "INTEGRITY_FAILURE",
        TechnicalFitnessState.IsolationRequired => "ISOLATION_REQUIRED",
        TechnicalFitnessState.RecoveryRequired => "RECOVERY_REQUIRED",
        TechnicalFitnessState.NotFit => "NOT_FIT",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string ToContract(FitnessProjectionResult value) => value switch
    {
        FitnessProjectionResult.Fit => "FIT",
        FitnessProjectionResult.Restricted => "RESTRICTED",
        FitnessProjectionResult.NotFit => "NOT_FIT",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string ToContract(EvidenceQuality value) => value switch
    {
        EvidenceQuality.Sufficient => "EQ-SUFFICIENT",
        EvidenceQuality.Limited => "EQ-LIMITED",
        EvidenceQuality.Insufficient => "EQ-INSUFFICIENT",
        EvidenceQuality.Invalid => "EQ-INVALID",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };
}

public static class HealthFitnessAssessmentIdentity
{
    public static string Compute(CanonicalHealthFitnessAssessment value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var canonical = BuildCanonical(
            value.AssessmentId,
            value.SubjectId,
            value.Capability,
            value.RequestedAuthorityLevel,
            HealthFitnessContractProjection.ToContract(value.HealthState),
            HealthFitnessContractProjection.ToContract(value.TechnicalFitnessState),
            HealthFitnessContractProjection.ToContract(value.FitnessResult),
            value.Scope,
            value.EvidenceReference,
            value.SelfModelReference,
            HealthFitnessContractProjection.ToContract(value.EvidenceQuality),
            value.Confidence,
            value.Unknowns,
            value.Contradictions,
            value.Constraints,
            value.Reason,
            value.RuleId,
            value.RuleVersion,
            value.ObservationTime.ToUniversalTime().ToString("O"),
            value.AssessmentTime.ToUniversalTime().ToString("O"),
            value.EffectiveTime.ToUniversalTime().ToString("O"),
            value.Expiry.ToUniversalTime().ToString("O"));

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    private static string BuildCanonical(params string[] values)
    {
        var builder = new StringBuilder();
        foreach (var value in values)
        {
            var safe = value ?? string.Empty;
            builder.Append(safe.Length);
            builder.Append(':');
            builder.Append(safe);
            builder.Append('|');
        }

        return builder.ToString();
    }
}
