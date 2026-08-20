using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.ArtifactPublication;

public enum RecoveryProjectionFreshness
{
    Current = 1,
    Stale = 2
}

public enum ReleaseAuthorizationProjectionState
{
    NotAuthorized = 1,
    Denied = 2,
    Authorized = 3
}

public enum ReleaseExecutionProjectionState
{
    NotExecuted = 1,
    Executed = 2
}

public enum ReintroductionProjectionState
{
    NotStarted = 1,
    Pending = 2,
    Observation = 3,
    Restricted = 4,
    Complete = 5
}

public sealed record RecoveryOperationalTruth(
    string RecoveryCaseIdentity,
    string RecoveryState,
    string RestorationOutcome,
    bool ReadyForReleaseDecision,
    ReleaseAuthorizationProjectionState ReleaseAuthorization,
    ReleaseExecutionProjectionState ReleaseExecution,
    ReintroductionProjectionState Reintroduction,
    string LifecycleState,
    string EvidenceReference,
    DateTimeOffset ObservedAt,
    DateTimeOffset ValidUntil,
    bool Complete);

public sealed record RecoveryOperationalProjection(
    string ProjectionIdentity,
    string RecoveryCaseIdentity,
    string RecoveryState,
    string RestorationOutcome,
    bool ReadyForReleaseDecision,
    ReleaseAuthorizationProjectionState ReleaseAuthorization,
    ReleaseExecutionProjectionState ReleaseExecution,
    ReintroductionProjectionState Reintroduction,
    string LifecycleState,
    string EvidenceReference,
    DateTimeOffset ObservedAt,
    DateTimeOffset ValidUntil,
    bool Complete,
    RecoveryProjectionFreshness Freshness,
    bool PresentationOnly,
    bool CarriesReleaseExecutionAuthority,
    bool CarriesLifecycleAuthority,
    bool CarriesBusinessAuthority);

public sealed record RecoveryOperationalProjectionDecision(
    bool Accepted,
    string Reason,
    RecoveryOperationalProjection? Projection);

public static class RecoveryOperationalProjectionRuntime
{
    private static readonly string[] ValidRecoveryStates =
    {
        "InitiationPending",
        "AuthorizedForAssessment",
        "PlanAuthorizationPending",
        "PlanAuthorized",
        "RestorationInProgress",
        "RestorationReported",
        "ReconciliationPending",
        "ValidationPending",
        "ValidationFailed",
        "ReadyForReleaseDecision",
        "ReleaseDenied",
        "ReleaseAuthorized",
        "ReintroductionPending",
        "RecoveryGuardObservation",
        "RecoveredWithRestrictedAuthority",
        "RecoveryComplete",
        "Aborted",
        "Escalated"
    };

    private static readonly string[] ValidRestorationOutcomes =
    {
        "Requested",
        "Attempted",
        "Completed",
        "Failed",
        "Partial"
    };

    public static RecoveryOperationalProjectionDecision Build(
        RecoveryOperationalTruth? truth,
        DateTimeOffset evaluationTime)
    {
        if (truth is null ||
            !Token(truth.RecoveryCaseIdentity) ||
            !ValidRecoveryStates.Contains(truth.RecoveryState, StringComparer.Ordinal) ||
            !ValidRestorationOutcomes.Contains(truth.RestorationOutcome, StringComparer.Ordinal) ||
            !Enum.IsDefined(truth.ReleaseAuthorization) ||
            !Enum.IsDefined(truth.ReleaseExecution) ||
            !Enum.IsDefined(truth.Reintroduction) ||
            !Token(truth.LifecycleState) ||
            !Token(truth.EvidenceReference) ||
            truth.ObservedAt == default ||
            truth.ValidUntil == default ||
            truth.ValidUntil <= truth.ObservedAt ||
            evaluationTime == default)
        {
            return Deny("INVALID_RECOVERY_OPERATIONAL_TRUTH");
        }

        var expectedReady = truth.RecoveryState is
            "ReadyForReleaseDecision" or
            "ReleaseDenied" or
            "ReleaseAuthorized" or
            "ReintroductionPending" or
            "RecoveryGuardObservation" or
            "RecoveredWithRestrictedAuthority" or
            "RecoveryComplete";

        if (truth.ReadyForReleaseDecision != expectedReady)
        {
            return Deny("READY_FOR_RELEASE_DECISION_STATE_MISMATCH");
        }

        var expectedAuthorization = truth.RecoveryState switch
        {
            "ReleaseDenied" => ReleaseAuthorizationProjectionState.Denied,
            "ReleaseAuthorized" or
            "ReintroductionPending" or
            "RecoveryGuardObservation" or
            "RecoveredWithRestrictedAuthority" or
            "RecoveryComplete" => ReleaseAuthorizationProjectionState.Authorized,
            _ => ReleaseAuthorizationProjectionState.NotAuthorized
        };

        if (truth.ReleaseAuthorization != expectedAuthorization)
        {
            return Deny("RELEASE_AUTHORIZATION_STATE_MISMATCH");
        }

        var expectedExecution = truth.RecoveryState is
            "ReintroductionPending" or
            "RecoveryGuardObservation" or
            "RecoveredWithRestrictedAuthority" or
            "RecoveryComplete"
            ? ReleaseExecutionProjectionState.Executed
            : ReleaseExecutionProjectionState.NotExecuted;

        if (truth.ReleaseExecution != expectedExecution)
        {
            return Deny("RELEASE_EXECUTION_STATE_MISMATCH");
        }

        var expectedReintroduction = truth.RecoveryState switch
        {
            "ReintroductionPending" => ReintroductionProjectionState.Pending,
            "RecoveryGuardObservation" => ReintroductionProjectionState.Observation,
            "RecoveredWithRestrictedAuthority" => ReintroductionProjectionState.Restricted,
            "RecoveryComplete" => ReintroductionProjectionState.Complete,
            _ => ReintroductionProjectionState.NotStarted
        };

        if (truth.Reintroduction != expectedReintroduction)
        {
            return Deny("REINTRODUCTION_STATE_MISMATCH");
        }

        var freshness = evaluationTime <= truth.ValidUntil
            ? RecoveryProjectionFreshness.Current
            : RecoveryProjectionFreshness.Stale;

        var canonical = string.Join("|", new[]
        {
            truth.RecoveryCaseIdentity,
            truth.RecoveryState,
            truth.RestorationOutcome,
            truth.ReadyForReleaseDecision ? "1" : "0",
            ((int)truth.ReleaseAuthorization).ToString(CultureInfo.InvariantCulture),
            ((int)truth.ReleaseExecution).ToString(CultureInfo.InvariantCulture),
            ((int)truth.Reintroduction).ToString(CultureInfo.InvariantCulture),
            truth.LifecycleState,
            truth.EvidenceReference,
            truth.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            truth.ValidUntil.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            truth.Complete ? "1" : "0"
        });

        var projection = new RecoveryOperationalProjection(
            "sha256/" + Sha256Hex(canonical),
            truth.RecoveryCaseIdentity,
            truth.RecoveryState,
            truth.RestorationOutcome,
            truth.ReadyForReleaseDecision,
            truth.ReleaseAuthorization,
            truth.ReleaseExecution,
            truth.Reintroduction,
            truth.LifecycleState,
            truth.EvidenceReference,
            truth.ObservedAt,
            truth.ValidUntil,
            truth.Complete,
            freshness,
            true,
            false,
            false,
            false);

        return new RecoveryOperationalProjectionDecision(
            true,
            freshness == RecoveryProjectionFreshness.Current
                ? "RECOVERY_OPERATIONAL_PROJECTION_CURRENT"
                : "RECOVERY_OPERATIONAL_PROJECTION_STALE",
            projection);
    }

    private static RecoveryOperationalProjectionDecision Deny(string reason) =>
        new(false, reason, null);

    private static bool Token(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        foreach (var ch in value)
        {
            if (char.IsControl(ch) || char.IsWhiteSpace(ch))
            {
                return false;
            }
        }

        return true;
    }

    private static string Sha256Hex(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
