using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Falcon.FSATS.Trading.Contracts;

public enum FoundationRecoveryFreshness
{
    Current = 1,
    Stale = 2
}

public enum FoundationReleaseAuthorizationState
{
    NotAuthorized = 1,
    Denied = 2,
    Authorized = 3
}

public enum FoundationReleaseExecutionState
{
    NotExecuted = 1,
    Executed = 2
}

public enum FoundationReintroductionState
{
    NotStarted = 1,
    Pending = 2,
    Observation = 3,
    Restricted = 4,
    Complete = 5
}

public sealed record FoundationRecoveryProjectionBindingInput(
    string FoundationCandidate,
    string RouteIdentity,
    string MessageType,
    string SchemaIdentity,
    string SchemaVersion,
    string Producer,
    string Recipient,
    string MessageKind,
    string Classification,
    string TransportAuthority,
    string ArtifactId,
    string ArtifactVersion,
    string ArtifactSha256,
    string EvidenceReference,
    string Provenance,
    string CompatibilityIdentity,
    string ArtifactState,
    string SourceContract,
    string ProjectionIdentity,
    string RecoveryCaseIdentity,
    string RecoveryState,
    string RestorationOutcome,
    bool ReadyForReleaseDecision,
    FoundationReleaseAuthorizationState ReleaseAuthorization,
    FoundationReleaseExecutionState ReleaseExecution,
    FoundationReintroductionState Reintroduction,
    string LifecycleState,
    string ProjectionEvidenceReference,
    DateTimeOffset ObservedAt,
    DateTimeOffset ValidUntil,
    bool Complete,
    FoundationRecoveryFreshness Freshness,
    bool PresentationOnly,
    bool CarriesReleaseExecutionAuthority,
    bool CarriesLifecycleAuthority,
    bool CarriesBusinessAuthority,
    bool RuntimeActivationAuthorized,
    bool LiveRouteActivated,
    bool DeploymentAuthorized);

public sealed record FoundationRecoveryProjectionBindingDecision(
    bool Accepted,
    string ReasonCode,
    bool ReadyForApplicationRecoveryDecision,
    bool ReleaseAuthorizationObserved,
    bool ReleaseExecutionObserved,
    bool RuntimeActivationAuthorized,
    bool LiveRouteActivated,
    bool DeploymentAuthorized,
    bool BusinessAuthorityGranted);

public static class FoundationRecoveryProjectionBinding
{
    public const string FoundationCandidate = "30a01643723967985c0db6204ad627e531571aec";
    public const string RouteIdentity = "route:foundation:recovery:application:v1";
    public const string MessageType = "Foundation.Operational.RecoveryProjection";
    public const string SchemaIdentity = "foundation.operational.recovery";
    public const string ContractVersion = "1.0.0";
    public const string Producer = "foundation.runtime";
    public const string Recipient = "fsats";
    public const string MessageKind = "Event";
    public const string Classification = "Operational";
    public const string TransportAuthority = "authority:transport:projection-only";
    public const string ArtifactId = "foundation/runtime-projection/recovery";
    public const string ArtifactSha256 = "sha256/468B594FF7D4F9641BE4A21BA8A0965922FFE0ADFBCED3B14C2C6A5272CBB5FF";
    public const string EvidenceReference = "evidence:foundation:stage9:owner-closure:c387958118561fbf3e1b9a66c1c9203c5916136b";
    public const string Provenance = "commit/33ff6232624d84b0a4f8156c8eb4f5f323353b65";
    public const string CompatibilityIdentity = "compat:foundation-public-runtime-projection:v1";
    public const string ArtifactState = "Published";
    public const string SourceContract = "Foundation.ArtifactPublication.RecoveryOperationalProjection";

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

    public static FoundationRecoveryProjectionBindingDecision Evaluate(
        FoundationRecoveryProjectionBindingInput? input,
        DateTimeOffset evaluationTime)
    {
        if (input is null || evaluationTime == default ||
            !Token(input.RecoveryCaseIdentity) ||
            !Token(input.LifecycleState) ||
            !Token(input.ProjectionEvidenceReference) ||
            input.ObservedAt == default || input.ValidUntil == default ||
            input.ValidUntil <= input.ObservedAt ||
            !Enum.IsDefined(input.ReleaseAuthorization) ||
            !Enum.IsDefined(input.ReleaseExecution) ||
            !Enum.IsDefined(input.Reintroduction) ||
            !Enum.IsDefined(input.Freshness))
        {
            return Reject("INVALID_RECOVERY_PROJECTION_BINDING");
        }

        if (!ExactProfile(input))
            return Reject("FOUNDATION_RECOVERY_PROFILE_MISMATCH");

        if (!ValidRecoveryStates.Contains(input.RecoveryState, StringComparer.Ordinal) ||
            !ValidRestorationOutcomes.Contains(input.RestorationOutcome, StringComparer.Ordinal))
        {
            return Reject("INVALID_RECOVERY_PROJECTION_STATE");
        }

        if (input.Freshness != FoundationRecoveryFreshness.Current || evaluationTime > input.ValidUntil)
            return Reject("FOUNDATION_RECOVERY_PROJECTION_STALE");

        if (input.ObservedAt > evaluationTime)
            return Reject("FOUNDATION_RECOVERY_PROJECTION_FROM_FUTURE");

        var expectedReady = input.RecoveryState is
            "ReadyForReleaseDecision" or
            "ReleaseDenied" or
            "ReleaseAuthorized" or
            "ReintroductionPending" or
            "RecoveryGuardObservation" or
            "RecoveredWithRestrictedAuthority" or
            "RecoveryComplete";

        if (input.ReadyForReleaseDecision != expectedReady)
            return Reject("READY_FOR_RELEASE_DECISION_STATE_MISMATCH");

        var expectedAuthorization = input.RecoveryState switch
        {
            "ReleaseDenied" => FoundationReleaseAuthorizationState.Denied,
            "ReleaseAuthorized" or
            "ReintroductionPending" or
            "RecoveryGuardObservation" or
            "RecoveredWithRestrictedAuthority" or
            "RecoveryComplete" => FoundationReleaseAuthorizationState.Authorized,
            _ => FoundationReleaseAuthorizationState.NotAuthorized
        };

        if (input.ReleaseAuthorization != expectedAuthorization)
            return Reject("RELEASE_AUTHORIZATION_STATE_MISMATCH");

        var expectedExecution = input.RecoveryState is
            "ReintroductionPending" or
            "RecoveryGuardObservation" or
            "RecoveredWithRestrictedAuthority" or
            "RecoveryComplete"
            ? FoundationReleaseExecutionState.Executed
            : FoundationReleaseExecutionState.NotExecuted;

        if (input.ReleaseExecution != expectedExecution)
            return Reject("RELEASE_EXECUTION_STATE_MISMATCH");

        var expectedReintroduction = input.RecoveryState switch
        {
            "ReintroductionPending" => FoundationReintroductionState.Pending,
            "RecoveryGuardObservation" => FoundationReintroductionState.Observation,
            "RecoveredWithRestrictedAuthority" => FoundationReintroductionState.Restricted,
            "RecoveryComplete" => FoundationReintroductionState.Complete,
            _ => FoundationReintroductionState.NotStarted
        };

        if (input.Reintroduction != expectedReintroduction)
            return Reject("REINTRODUCTION_STATE_MISMATCH");

        if (!StringComparer.Ordinal.Equals(input.ProjectionIdentity, ComputeProjectionIdentity(input)))
            return Reject("RECOVERY_PROJECTION_IDENTITY_MISMATCH");

        if (!input.PresentationOnly ||
            input.CarriesReleaseExecutionAuthority ||
            input.CarriesLifecycleAuthority ||
            input.CarriesBusinessAuthority ||
            input.RuntimeActivationAuthorized ||
            input.LiveRouteActivated ||
            input.DeploymentAuthorized)
        {
            return Reject("RECOVERY_PROJECTION_AUTHORITY_SMUGGLING_REJECTED");
        }

        return new(
            true,
            "FOUNDATION_STAGE9_RECOVERY_PROJECTION_BOUND",
            input.ReadyForReleaseDecision,
            input.ReleaseAuthorization == FoundationReleaseAuthorizationState.Authorized,
            input.ReleaseExecution == FoundationReleaseExecutionState.Executed,
            false,
            false,
            false,
            false);
    }

    public static string ComputeProjectionIdentity(FoundationRecoveryProjectionBindingInput input)
    {
        var canonical = string.Join("|", new[]
        {
            input.RecoveryCaseIdentity,
            input.RecoveryState,
            input.RestorationOutcome,
            input.ReadyForReleaseDecision ? "1" : "0",
            ((int)input.ReleaseAuthorization).ToString(CultureInfo.InvariantCulture),
            ((int)input.ReleaseExecution).ToString(CultureInfo.InvariantCulture),
            ((int)input.Reintroduction).ToString(CultureInfo.InvariantCulture),
            input.LifecycleState,
            input.ProjectionEvidenceReference,
            input.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            input.ValidUntil.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            input.Complete ? "1" : "0"
        });

        return "sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    private static bool ExactProfile(FoundationRecoveryProjectionBindingInput input) =>
        StringComparer.Ordinal.Equals(input.FoundationCandidate, FoundationCandidate) &&
        StringComparer.Ordinal.Equals(input.RouteIdentity, RouteIdentity) &&
        StringComparer.Ordinal.Equals(input.MessageType, MessageType) &&
        StringComparer.Ordinal.Equals(input.SchemaIdentity, SchemaIdentity) &&
        StringComparer.Ordinal.Equals(input.SchemaVersion, ContractVersion) &&
        StringComparer.Ordinal.Equals(input.Producer, Producer) &&
        StringComparer.Ordinal.Equals(input.Recipient, Recipient) &&
        StringComparer.Ordinal.Equals(input.MessageKind, MessageKind) &&
        StringComparer.Ordinal.Equals(input.Classification, Classification) &&
        StringComparer.Ordinal.Equals(input.TransportAuthority, TransportAuthority) &&
        StringComparer.Ordinal.Equals(input.ArtifactId, ArtifactId) &&
        StringComparer.Ordinal.Equals(input.ArtifactVersion, ContractVersion) &&
        StringComparer.OrdinalIgnoreCase.Equals(input.ArtifactSha256, ArtifactSha256) &&
        StringComparer.Ordinal.Equals(input.EvidenceReference, EvidenceReference) &&
        StringComparer.Ordinal.Equals(input.Provenance, Provenance) &&
        StringComparer.Ordinal.Equals(input.CompatibilityIdentity, CompatibilityIdentity) &&
        StringComparer.Ordinal.Equals(input.ArtifactState, ArtifactState) &&
        StringComparer.Ordinal.Equals(input.SourceContract, SourceContract);

    private static bool Token(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
            return false;

        foreach (var ch in value)
        {
            if (char.IsControl(ch) || char.IsWhiteSpace(ch))
                return false;
        }

        return true;
    }

    private static FoundationRecoveryProjectionBindingDecision Reject(string reason) =>
        new(false, reason, false, false, false, false, false, false, false);
}
