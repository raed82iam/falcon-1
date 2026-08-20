namespace Falcon.FSATS.ResourceManagement.Contracts;

public enum FoundationResourceProjectionKind
{
    ApplicationResourceState = 0,
    AggregateResourceState = 1
}

public sealed record FoundationResourceProjectionDescriptor(
    string ArtifactId,
    string ArtifactVersion,
    string CompatibilityIdentity,
    string SourceContract,
    string EvidenceReference,
    string FoundationCandidateCommit,
    FoundationResourceProjectionKind Kind,
    DateTimeOffset ObservedAt,
    string PayloadSha256,
    bool PublicationVerified,
    bool RuntimeActivationAuthorized,
    bool BusinessAuthorityGranted);

public sealed record FoundationResourceProjectionBindingDecision(
    bool Accepted,
    string ReasonCode,
    FoundationResourceProjectionKind Kind,
    bool FoundationTruthOnly,
    bool ResourceAuthorityGranted,
    bool LoadSheddingExecutionAuthorized,
    bool RuntimeActivationAuthorized,
    bool BusinessAuthorityGranted);

public static class FoundationResourceProjectionBinding
{
    public const string FoundationCandidateCommit = "d24a2f7f91a3282cc556946f00741e238fc77d6e";
    public const string CompatibilityIdentity = "compat:foundation-resource-governance:v1";
    public const string ApplicationArtifactId = "foundation/contracts/resource-state-projection";
    public const string AggregateArtifactId = "foundation/contracts/aggregate-resource-state-projection";
    public const string ArtifactVersion = "1.0.0";
    public const string ApplicationSourceContract = "Foundation.State.ResourceGovernance.ApplicationResourceStateProjection";
    public const string AggregateSourceContract = "Foundation.State.ResourceGovernance.AggregateResourceStateProjection";

    public static FoundationResourceProjectionBindingDecision Evaluate(
        FoundationResourceProjectionDescriptor? descriptor,
        DateTimeOffset now,
        TimeSpan maximumAge)
    {
        if (descriptor is null)
            return Reject(FoundationResourceProjectionKind.ApplicationResourceState, "MISSING_DESCRIPTOR");
        if (maximumAge <= TimeSpan.Zero)
            return Reject(descriptor.Kind, "INVALID_MAXIMUM_AGE");
        if (!descriptor.PublicationVerified)
            return Reject(descriptor.Kind, "UNVERIFIED_PUBLICATION");
        if (descriptor.RuntimeActivationAuthorized || descriptor.BusinessAuthorityGranted)
            return Reject(descriptor.Kind, "AUTHORITY_SMUGGLING_REJECTED");
        if (!StringComparer.Ordinal.Equals(descriptor.FoundationCandidateCommit, FoundationCandidateCommit))
            return Reject(descriptor.Kind, "FOUNDATION_CANDIDATE_MISMATCH");
        if (!StringComparer.Ordinal.Equals(descriptor.ArtifactVersion, ArtifactVersion))
            return Reject(descriptor.Kind, "ARTIFACT_VERSION_MISMATCH");
        if (!StringComparer.Ordinal.Equals(descriptor.CompatibilityIdentity, CompatibilityIdentity))
            return Reject(descriptor.Kind, "COMPATIBILITY_IDENTITY_MISMATCH");
        if (string.IsNullOrWhiteSpace(descriptor.EvidenceReference))
            return Reject(descriptor.Kind, "MISSING_EVIDENCE");
        if (!IsSha256(descriptor.PayloadSha256))
            return Reject(descriptor.Kind, "INVALID_PAYLOAD_DIGEST");
        if (descriptor.ObservedAt > now || now - descriptor.ObservedAt > maximumAge)
            return Reject(descriptor.Kind, "STALE_OR_FUTURE_PROJECTION");

        var expectedArtifact = descriptor.Kind == FoundationResourceProjectionKind.ApplicationResourceState
            ? ApplicationArtifactId
            : AggregateArtifactId;
        var expectedContract = descriptor.Kind == FoundationResourceProjectionKind.ApplicationResourceState
            ? ApplicationSourceContract
            : AggregateSourceContract;

        if (!StringComparer.Ordinal.Equals(descriptor.ArtifactId, expectedArtifact))
            return Reject(descriptor.Kind, "ARTIFACT_ID_MISMATCH");
        if (!StringComparer.Ordinal.Equals(descriptor.SourceContract, expectedContract))
            return Reject(descriptor.Kind, "SOURCE_CONTRACT_MISMATCH");

        return new FoundationResourceProjectionBindingDecision(
            true,
            "ACCEPTED_FOUNDATION_TRUTH_PROJECTION",
            descriptor.Kind,
            FoundationTruthOnly: true,
            ResourceAuthorityGranted: false,
            LoadSheddingExecutionAuthorized: false,
            RuntimeActivationAuthorized: false,
            BusinessAuthorityGranted: false);
    }

    private static FoundationResourceProjectionBindingDecision Reject(FoundationResourceProjectionKind kind, string reason)
        => new(false, reason, kind, true, false, false, false, false);

    private static bool IsSha256(string? value)
        => value is { Length: 64 } && value.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F');
}
