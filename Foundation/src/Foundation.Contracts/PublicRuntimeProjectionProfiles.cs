using System;

namespace Foundation.Contracts;

public static class PublicRuntimeProjectionProfiles
{
    public const string ContractVersion = "1.0.0";
    public const string CompatibilityIdentity = "compat:foundation-public-runtime-projection:v1";
    public const string FoundationProducerIdentity = "foundation.runtime";
    public const string ProjectionOnlyAuthority = "authority:transport:projection-only";

    public const string RecoveryMessageType = "Foundation.Operational.RecoveryProjection";
    public const string RecoverySchemaIdentity = "foundation.operational.recovery";
    public const string RecoveryArtifactId = "foundation/runtime-projection/recovery";

    // FCR-0082: exact Application consumption profile for the accepted-and-closed
    // Stage 9 generic recovery/release projection. The canonical Foundation route,
    // artifact digest, evidence and provenance are Application-neutral. A recipient
    // identity is supplied explicitly by the consuming Application. The legacy FSATS
    // symbols remain compatibility aliases for the accepted-and-closed FCR-0082 binding.
    public const string RecoveryApplicationRouteIdentity = "route:foundation:recovery:application:v1";
    public const string FsatsRecipientIdentity = "fsats";
    public const string RecoveryApplicationArtifactSha256 = "sha256/468B594FF7D4F9641BE4A21BA8A0965922FFE0ADFBCED3B14C2C6A5272CBB5FF";
    public const string RecoveryApplicationEvidenceReference = "evidence:foundation:stage9:owner-closure:c387958118561fbf3e1b9a66c1c9203c5916136b";
    public const string RecoveryApplicationProvenanceReference = "commit/33ff6232624d84b0a4f8156c8eb4f5f323353b65";
    public const string RecoveryApplicationSourceContract = "Foundation.ArtifactPublication.RecoveryOperationalProjection";
    public const string RecoveryApplicationCanonicalPayload =
        "artifact_contract=foundation:runtime-projection:recovery\n" +
        "artifact_version=1.0.0\n" +
        "kind=Contract\n" +
        "source_contract=Foundation.ArtifactPublication.RecoveryOperationalProjection\n" +
        "source_contract_set=RecoveryOperationalProjection|RecoveryProjectionFreshness|ReleaseAuthorizationProjectionState|ReleaseExecutionProjectionState|ReintroductionProjectionState\n" +
        "governing_stage=9\n" +
        "purpose=APPLICATION_CONSUMABLE_GENERIC_RECOVERY_RELEASE_RUNTIME_PROJECTION\n" +
        "invariant_1=REPAIR_SUCCESS!=RELEASE\n" +
        "invariant_2=READY_FOR_RELEASE_DECISION!=RELEASE_AUTHORIZATION\n" +
        "invariant_3=RELEASE_AUTHORIZATION!=RELEASE_EXECUTION\n" +
        "invariant_4=LIFECYCLE_TRANSITION!=NEW_AUTHORITY_DECISION\n" +
        "invariant_5=APPLICATION_BUSINESS_RECOVERY=APPLICATION_OWNED\n" +
        "invariant_6=STAGE13_FSA_CONTROLLED_REVIVAL!=STAGE9_GENERIC_RECOVERY\n" +
        "invariant_7=TECHNICAL_CONSUMPTION!=RUNTIME_AUTHORITY\n" +
        "invariant_8=CANONICAL_ARTIFACT_CONSUMPTION!=RUNTIME_ACTIVATION\n";

    public const string IdentitySecurityContextMessageType = "Foundation.Security.IdentityContextProjection";
    public const string IdentitySecurityContextSchemaIdentity = "foundation.security.identity-context";
    public const string IdentitySecurityContextArtifactId = "foundation/runtime-projection/identity-security-context";

    // FCR-0239: exact Shared Web consumption profile for the accepted Stage 14
    // FoundationOperationalProjection. The generic builder below owns the Foundation
    // transport semantics; this named route/recipient pair remains a compatibility alias.
    public const string FoundationOperationalMessageType = "Foundation.Operational.FoundationProjection";
    public const string FoundationOperationalSchemaIdentity = "foundation.operational.foundation";
    public const string FoundationOperationalArtifactId = "foundation/runtime-projection/operational";
    public const string FoundationOperationalSharedWebRouteIdentity = "route:foundation:operational:web:v1";
    public const string SharedWebRecipientIdentity = "shared-web";

    public static PublicRuntimeProjectionRoute RecoveryOperational(
        string routeIdentity,
        string recipientScope,
        string artifactSha256,
        string evidenceReference,
        string provenanceReference) =>
        Create(
            routeIdentity,
            recipientScope,
            RecoveryMessageType,
            RecoverySchemaIdentity,
            FilMessageClassification.Operational,
            RecoveryArtifactId,
            artifactSha256,
            evidenceReference,
            provenanceReference);

    public static PublicRuntimeProjectionRoute RecoveryOperationalForApplication(string recipientScope)
    {
        if (string.IsNullOrWhiteSpace(recipientScope))
            throw new ArgumentException("Application recipient scope is required.", nameof(recipientScope));

        return Create(
            RecoveryApplicationRouteIdentity,
            recipientScope,
            RecoveryMessageType,
            RecoverySchemaIdentity,
            FilMessageClassification.Operational,
            RecoveryArtifactId,
            RecoveryApplicationArtifactSha256,
            RecoveryApplicationEvidenceReference,
            RecoveryApplicationProvenanceReference);
    }

    public static bool IsCanonicalRecoveryOperationalForApplication(
        PublicRuntimeProjectionRoute? route,
        string recipientScope)
    {
        if (route is null || string.IsNullOrWhiteSpace(recipientScope))
            return false;

        return route.RouteIdentity == RecoveryApplicationRouteIdentity &&
               route.MessageType == RecoveryMessageType &&
               route.SchemaId is not null && route.SchemaId.Value == RecoverySchemaIdentity &&
               route.SchemaVersion == ContractVersion &&
               route.Producer is not null && route.Producer.Value == FoundationProducerIdentity &&
               route.RecipientScope is not null && route.RecipientScope.Value == recipientScope &&
               route.MessageKind == FilMessageKind.Event &&
               route.Classification == FilMessageClassification.Operational &&
               route.TransportAuthority is not null && route.TransportAuthority.Value == ProjectionOnlyAuthority &&
               route.Provenance is not null && route.Provenance.Value == RecoveryApplicationProvenanceReference &&
               route.ArtifactId == RecoveryArtifactId &&
               route.ArtifactVersion == ContractVersion &&
               route.ArtifactSha256 == RecoveryApplicationArtifactSha256 &&
               route.EvidenceReference == RecoveryApplicationEvidenceReference &&
               route.CompatibilityIdentity == CompatibilityIdentity &&
               route.ArtifactState == PublicProjectionArtifactState.Published;
    }

    public static PublicRuntimeProjectionRoute RecoveryOperationalForFsats() =>
        RecoveryOperationalForApplication(FsatsRecipientIdentity);

    public static bool IsCanonicalRecoveryOperationalForFsats(PublicRuntimeProjectionRoute? route) =>
        IsCanonicalRecoveryOperationalForApplication(route, FsatsRecipientIdentity);

    public static PublicRuntimeProjectionRoute IdentitySecurityContext(
        string routeIdentity,
        string recipientScope,
        string artifactSha256,
        string evidenceReference,
        string provenanceReference) =>
        Create(
            routeIdentity,
            recipientScope,
            IdentitySecurityContextMessageType,
            IdentitySecurityContextSchemaIdentity,
            FilMessageClassification.Security,
            IdentitySecurityContextArtifactId,
            artifactSha256,
            evidenceReference,
            provenanceReference);

    public static PublicRuntimeProjectionRoute FoundationOperational(
        string routeIdentity,
        string recipientScope,
        string artifactSha256,
        string evidenceReference,
        string provenanceReference) =>
        Create(
            routeIdentity,
            recipientScope,
            FoundationOperationalMessageType,
            FoundationOperationalSchemaIdentity,
            FilMessageClassification.Operational,
            FoundationOperationalArtifactId,
            artifactSha256,
            evidenceReference,
            provenanceReference);

    public static PublicRuntimeProjectionRoute FoundationOperationalForSharedWeb(
        string artifactSha256,
        string evidenceReference,
        string provenanceReference) =>
        FoundationOperational(
            FoundationOperationalSharedWebRouteIdentity,
            SharedWebRecipientIdentity,
            artifactSha256,
            evidenceReference,
            provenanceReference);

    private static PublicRuntimeProjectionRoute Create(
        string routeIdentity,
        string recipientScope,
        string messageType,
        string schemaIdentity,
        FilMessageClassification classification,
        string artifactId,
        string artifactSha256,
        string evidenceReference,
        string provenanceReference) =>
        new(
            routeIdentity,
            messageType,
            new SchemaIdentity(schemaIdentity),
            ContractVersion,
            new ProducerIdentityReference(FoundationProducerIdentity),
            new RecipientScopeReference(recipientScope),
            FilMessageKind.Event,
            classification,
            new AuthorityReference(ProjectionOnlyAuthority),
            new ProvenanceReference(provenanceReference),
            artifactId,
            ContractVersion,
            artifactSha256,
            evidenceReference,
            CompatibilityIdentity,
            PublicProjectionArtifactState.Published);
}
