using System;

namespace Foundation.Contracts;

/// <summary>
/// Additive executable successor for the approved CON-006 v1.2 documentary contract.
/// The v1.1 HealthFitnessAssessment remains available for predecessor compatibility;
/// new Stage 7 runtime work targets this v1.2 representation.
/// </summary>
public static class HealthFitnessContractV12
{
    public const string Version = "1.2";

    public static bool IsHealthState(string? value) =>
        IsOneOf(value, "HEALTHY", "DEGRADED", "UNHEALTHY", "UNKNOWN", "NOT_APPLICABLE");

    public static bool IsTechnicalFitnessState(string? value) =>
        IsOneOf(
            value,
            "FIT",
            "FIT_WITH_CONSTRAINTS",
            "DEGRADED",
            "UNKNOWN",
            "UNAVAILABLE",
            "INTEGRITY_FAILURE",
            "ISOLATION_REQUIRED",
            "RECOVERY_REQUIRED",
            "NOT_FIT");

    public static bool IsFitnessResult(string? value) =>
        IsOneOf(value, "FIT", "RESTRICTED", "NOT_FIT");

    public static bool IsEvidenceQuality(string? value) =>
        IsOneOf(value, "EQ-SUFFICIENT", "EQ-LIMITED", "EQ-INSUFFICIENT", "EQ-INVALID");

    public static bool IsCanonicalIdentifier(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        foreach (var character in value)
        {
            if (char.IsControl(character))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsOneOf(string? value, params string[] allowed)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        foreach (var candidate in allowed)
        {
            if (string.Equals(value, candidate, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}

public sealed record HealthFitnessAssessmentV12(
    string AssessmentId,
    string Version,
    string SubjectId,
    string Capability,
    string RequestedAuthorityLevel,
    string HealthState,
    string TechnicalFitnessState,
    string FitnessResult,
    string Scope,
    string EvidenceReference,
    string SelfModelReference,
    string EvidenceQuality,
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
    public string ContractId => ContractIdentity.Con006;
}

public static class HealthFitnessV12Validator
{
    public static ValidationOutcome Validate(HealthFitnessAssessmentV12? value)
    {
        if (value is null)
        {
            return ValidationOutcome.Failed("CON-006 v1.2 assessment missing");
        }

        if (!string.Equals(value.Version, HealthFitnessContractV12.Version, StringComparison.Ordinal))
        {
            return ValidationOutcome.Failed("CON-006 v1.2 version rejected");
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
            return ValidationOutcome.Failed("CON-006 v1.2 canonical identity rejected");
        }

        if (!HealthFitnessContractV12.IsHealthState(value.HealthState) ||
            !HealthFitnessContractV12.IsTechnicalFitnessState(value.TechnicalFitnessState) ||
            !HealthFitnessContractV12.IsFitnessResult(value.FitnessResult) ||
            !HealthFitnessContractV12.IsEvidenceQuality(value.EvidenceQuality))
        {
            return ValidationOutcome.Failed("CON-006 v1.2 canonical enum rejected");
        }

        if (string.IsNullOrWhiteSpace(value.Confidence) ||
            string.IsNullOrWhiteSpace(value.Unknowns) ||
            string.IsNullOrWhiteSpace(value.Contradictions) ||
            string.IsNullOrWhiteSpace(value.Constraints) ||
            string.IsNullOrWhiteSpace(value.Reason))
        {
            return ValidationOutcome.Failed("CON-006 v1.2 required assessment detail missing");
        }

        if (value.ObservationTime == default ||
            value.AssessmentTime == default ||
            value.EffectiveTime == default ||
            value.Expiry == default ||
            value.ObservationTime > value.AssessmentTime ||
            value.AssessmentTime > value.EffectiveTime ||
            value.Expiry <= value.EffectiveTime)
        {
            return ValidationOutcome.Failed("CON-006 v1.2 assessment time order rejected");
        }

        return ValidationOutcome.Passed("CON-006 v1.2 assessment valid");
    }
}
