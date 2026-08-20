using System;

namespace Foundation.ArtifactPublication;

public static class CanonicalFoundationArtifacts
{
    public const string Stage13AiKillControlPlaneArtifactId = "foundation/contracts/ai-kill-control-plane";
    public const string Stage13AiKillControlPlaneArtifactVersion = "1.0.0";
    public const string Stage13AiKillControlPlaneSha256 = "sha256/BD36F1A4B8D49EC08347D3051461D788C636D03AF65B5616DE6B52B7A112B770";
    public const string Stage13AiKillControlPlaneEvidenceReference = "evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed";
    public const string Stage13AiKillControlPlaneCompatibilityIdentity = "compat:foundation-ai-kill-control-plane:v1";
    public const string Stage13AiKillControlPlaneAuthoritativeSource = "src/Foundation.ArtifactPublication/CanonicalFoundationArtifacts.cs";
    public const string Stage13AiKillControlPlaneSourceContract = "Foundation.Authority.AiKillControlPlaneContract";
    public const string Stage13AiKillControlPlaneGoverningCommit = "8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc";

    public const string Stage13AiKillControlPlaneCanonicalPayload =
        "artifact_contract=foundation:ai-kill-control-plane\n" +
        "artifact_version=1.0.0\n" +
        "kind=Contract\n" +
        "source_contract=Foundation.Authority.AiKillControlPlaneContract\n" +
        "control_plane_id=foundation:ai-kill-control-plane\n" +
        "all_ai_target_id=falcon:all-ai\n" +
        "purpose=AI_EMERGENCY_CONTROL\n" +
        "safe_core_capabilities=OWNER_CONTROL,AI_KILL_CONTROL,LIFECYCLE_ENFORCEMENT,AUTHORITY_REVOCATION,SECURITY,AUDIT_EVIDENCE,FORENSICS,RECOVERY_INFRASTRUCTURE,EMERGENCY_COMMUNICATIONS\n" +
        "invariant_1=APPLICATION_AI_BUSINESS_SEMANTICS=APPLICATION_OWNED\n" +
        "invariant_2=FOUNDATION_KILL_ENFORCEMENT=FOUNDATION_OWNED\n" +
        "invariant_3=GLOBAL_AI_KILL!=FALCON_SHUTDOWN\n" +
        "invariant_4=AI_RESTART!=AUTHORITY_RESTORATION\n" +
        "invariant_5=APPLICATION_RECOVERY!=FOUNDATION_RELEASE_AUTHORITY\n" +
        "invariant_6=TECHNICAL_CONSUMPTION!=BUSINESS_AUTHORITY\n";

    public const string Stage6ResourceStateArtifactId = "foundation/contracts/resource-state-projection";
    public const string Stage6ResourceStateArtifactVersion = "1.0.0";
    public const string Stage6ResourceStateSha256 = "sha256/94D8E1B6CB17C4A837FBE019B556F39EE65D006110327600BF5946210FCDC853";
    public const string Stage6ResourceStateEvidenceReference = "evidence:foundation:stage6:owner-closure:20260811";
    public const string Stage6ResourceStateCompatibilityIdentity = "compat:foundation-resource-governance:v1";
    public const string Stage6ResourceStateAuthoritativeSource = "src/Foundation.State/ApplicationResourceStateProjectionGovernance.cs";
    public const string Stage6ResourceStateSourceContract = "Foundation.State.ResourceGovernance.ApplicationResourceStateProjection";
    public const string Stage6ResourceStateGoverningCommit = "47928a5b0cc371a74c8f2063ca216fb9bb1f2ae4";

    public const string Stage6ResourceStateCanonicalPayload =
        "artifact_contract=foundation:resource-state-projection\n" +
        "artifact_version=1.0.0\n" +
        "kind=Contract\n" +
        "source_contract=Foundation.State.ResourceGovernance.ApplicationResourceStateProjection\n" +
        "source_contract_set=ApplicationResourceStateProjection|ApplicationResourceStateProjectionSet|TechnicalLoadSheddingSignalClass\n" +
        "governing_stage=6\n" +
        "purpose=RESOURCE_STATE_PRESSURE_LOAD_SHEDDING_PROJECTION\n" +
        "invariant_1=RESOURCE_STATE_PROJECTION!=RESOURCE_AUTHORITY\n" +
        "invariant_2=LOAD_SHEDDING_SIGNAL!=LOAD_SHEDDING_EXECUTOR\n" +
        "invariant_3=REQUESTED_RESOURCE!=PROVEN_RESIDUAL_NEED!=GRANTED_RESOURCE\n" +
        "invariant_4=TECHNICAL_CONSUMPTION!=BUSINESS_AUTHORITY\n" +
        "invariant_5=CANONICAL_ARTIFACT_CONSUMPTION!=RUNTIME_ACTIVATION\n";

    public const string Stage6AggregateResourceStateArtifactId = "foundation/contracts/aggregate-resource-state-projection";
    public const string Stage6AggregateResourceStateArtifactVersion = "1.0.0";
    public const string Stage6AggregateResourceStateSha256 = "sha256/883B662A048FCAED2038A83B824E81A5C0C1CE56D9EB8B5F612AD20E94CE1134";
    public const string Stage6AggregateResourceStateEvidenceReference = "evidence:foundation:stage6:owner-closure:20260811";
    public const string Stage6AggregateResourceStateCompatibilityIdentity = "compat:foundation-resource-governance:v1";
    public const string Stage6AggregateResourceStateAuthoritativeSource = "src/Foundation.State/ApplicationResourceStateProjectionGovernance.cs";
    public const string Stage6AggregateResourceStateSourceContract = "Foundation.State.ResourceGovernance.AggregateResourceStateProjection";
    public const string Stage6AggregateResourceStateGoverningCommit = "47928a5b0cc371a74c8f2063ca216fb9bb1f2ae4";

    public const string Stage6AggregateResourceStateCanonicalPayload =
        "artifact_contract=foundation:aggregate-resource-state-projection\n" +
        "artifact_version=1.0.0\n" +
        "kind=Contract\n" +
        "source_contract=Foundation.State.ResourceGovernance.AggregateResourceStateProjection\n" +
        "source_contract_set=AggregateResourceStateProjection|ApplicationResourceStateProjection|ResourceCoordinationEnvelope\n" +
        "governing_stage=6\n" +
        "purpose=AGGREGATE_RESOURCE_COORDINATION_PROJECTION\n" +
        "invariant_1=APP_RSC!=FOUNDATION_RESOURCE_GOVERNANCE\n" +
        "invariant_2=FOUNDATION_AUTHORITATIVE_RESOURCE_TRUTH=FOUNDATION_OWNED\n" +
        "invariant_3=APP_RSC_INTERNAL_EFFECTIVE_DISTRIBUTION=APPLICATION_OWNED_WITHIN_GOVERNED_COORDINATION_ENVELOPE\n" +
        "invariant_4=INTERNAL_REDISTRIBUTION_FIRST\n" +
        "invariant_5=FOUNDATION_ADDITIONAL_REQUEST_SECOND\n" +
        "invariant_6=CANONICAL_ARTIFACT_CONSUMPTION!=RUNTIME_ACTIVATION\n";

    public static FoundationArtifactDescriptor Stage13AiKillControlPlane { get; } = new(
        Stage13AiKillControlPlaneArtifactId,
        Stage13AiKillControlPlaneArtifactVersion,
        Stage13AiKillControlPlaneSha256,
        FoundationArtifactKind.Contract,
        "foundation.authority",
        "commit/" + Stage13AiKillControlPlaneGoverningCommit,
        Stage13AiKillControlPlaneEvidenceReference,
        Stage13AiKillControlPlaneCompatibilityIdentity,
        FoundationArtifactPublicationState.Published,
        new DateTimeOffset(2026, 8, 17, 19, 17, 0, TimeSpan.Zero));

    public static FoundationArtifactDescriptor Stage6ResourceStateProjection { get; } = new(
        Stage6ResourceStateArtifactId,
        Stage6ResourceStateArtifactVersion,
        Stage6ResourceStateSha256,
        FoundationArtifactKind.Contract,
        "foundation.resource-governance",
        "commit/" + Stage6ResourceStateGoverningCommit,
        Stage6ResourceStateEvidenceReference,
        Stage6ResourceStateCompatibilityIdentity,
        FoundationArtifactPublicationState.Published,
        new DateTimeOffset(2026, 8, 17, 21, 30, 0, TimeSpan.Zero));

    public static FoundationArtifactDescriptor Stage6AggregateResourceStateProjection { get; } = new(
        Stage6AggregateResourceStateArtifactId,
        Stage6AggregateResourceStateArtifactVersion,
        Stage6AggregateResourceStateSha256,
        FoundationArtifactKind.Contract,
        "foundation.resource-governance",
        "commit/" + Stage6AggregateResourceStateGoverningCommit,
        Stage6AggregateResourceStateEvidenceReference,
        Stage6AggregateResourceStateCompatibilityIdentity,
        FoundationArtifactPublicationState.Published,
        new DateTimeOffset(2026, 8, 17, 21, 30, 0, TimeSpan.Zero));

    public static FoundationArtifactCatalog CreateCatalog() =>
        new(new[]
        {
            Stage13AiKillControlPlane,
            Stage6ResourceStateProjection,
            Stage6AggregateResourceStateProjection
        });
}
