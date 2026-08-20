using System;
using System.Security.Cryptography;
using System.Text;
using Foundation.ArtifactPublication;

namespace Falcon.CanonicalArtifactPublication.Verifier;

internal static class Program
{
    private static int _checks;

    private static int Main()
    {
        try
        {
            var descriptor = CanonicalFoundationArtifacts.Stage13AiKillControlPlane;

            Check(descriptor.ArtifactId == "foundation/contracts/ai-kill-control-plane", "artifact id mismatch");
            Check(descriptor.ArtifactVersion == "1.0.0", "artifact version mismatch");
            Check(descriptor.Kind == FoundationArtifactKind.Contract, "artifact kind mismatch");
            Check(descriptor.ProducerIdentity == "foundation.authority", "producer identity mismatch");
            Check(descriptor.ProvenanceReference == "commit/8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc", "provenance mismatch");
            Check(descriptor.EvidenceReference == "evidence:foundation:stage13:owner-closure:e59ccbba5786755b4e7f17a29810465ab0d4d6ed", "evidence reference mismatch");
            Check(descriptor.CompatibilityIdentity == "compat:foundation-ai-kill-control-plane:v1", "compatibility identity mismatch");
            Check(descriptor.State == FoundationArtifactPublicationState.Published, "artifact not published");
            Check(descriptor.PublishedAt == new DateTimeOffset(2026, 8, 17, 19, 17, 0, TimeSpan.Zero), "publication time mismatch");

            var computedDigest = "sha256/" + Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneCanonicalPayload)));
            Check(computedDigest == CanonicalFoundationArtifacts.Stage13AiKillControlPlaneSha256, "canonical payload SHA-256 mismatch");
            Check(descriptor.Sha256Digest == computedDigest, "descriptor digest does not bind canonical payload");
            Check(ArtifactPublicationRuntime.ValidDescriptor(descriptor), "canonical descriptor rejected");
            Check(ArtifactPublicationRuntime.ImmutableProvenance(descriptor.ProvenanceReference), "canonical descriptor provenance not immutable");

            var publication = ArtifactPublicationRuntime.EvaluatePublication(
                new ArtifactPublicationCandidate(descriptor, true, true, true, true, true));
            Check(publication.EligibleForPublication, "canonical Stage 13 contract not eligible for publication");
            Check(publication.Reason == "ELIGIBLE_FOR_CANONICAL_PUBLICATION", "publication reason mismatch");
            Check(publication.ExactArtifactIdentity == descriptor.ExactIdentity, "publication exact identity mismatch");
            Check(!publication.ActivationAuthorized, "publication authorized activation");
            Check(!publication.DeploymentAuthorized, "publication authorized deployment");
            Check(!publication.BusinessAuthorityGranted, "publication granted business authority");

            var catalog = CanonicalFoundationArtifacts.CreateCatalog();
            Check(catalog.Count == 3, "canonical catalog count mismatch");
            Check(catalog.TryGetExact(descriptor.ArtifactId, descriptor.ArtifactVersion, descriptor.Sha256Digest, out var exact), "exact Stage 13 descriptor not found");
            Check(exact == descriptor, "exact Stage 13 descriptor lookup mismatch");

            var resourceState = CanonicalFoundationArtifacts.Stage6ResourceStateProjection;
            Check(catalog.TryGetExact(resourceState.ArtifactId, resourceState.ArtifactVersion, resourceState.Sha256Digest, out var exactResourceState), "exact Stage 6 resource-state descriptor not found");
            Check(exactResourceState == resourceState, "exact Stage 6 resource-state descriptor lookup mismatch");
            Check(resourceState.State == FoundationArtifactPublicationState.Published, "Stage 6 resource-state descriptor not published");
            Check(ArtifactPublicationRuntime.ValidDescriptor(resourceState), "Stage 6 resource-state descriptor rejected");

            var aggregateResourceState = CanonicalFoundationArtifacts.Stage6AggregateResourceStateProjection;
            Check(catalog.TryGetExact(aggregateResourceState.ArtifactId, aggregateResourceState.ArtifactVersion, aggregateResourceState.Sha256Digest, out var exactAggregateResourceState), "exact Stage 6 aggregate-resource descriptor not found");
            Check(exactAggregateResourceState == aggregateResourceState, "exact Stage 6 aggregate-resource descriptor lookup mismatch");
            Check(aggregateResourceState.State == FoundationArtifactPublicationState.Published, "Stage 6 aggregate-resource descriptor not published");
            Check(ArtifactPublicationRuntime.ValidDescriptor(aggregateResourceState), "Stage 6 aggregate-resource descriptor rejected");

            var request = new ArtifactConsumptionRequest(
                "application:fsats",
                descriptor.ArtifactId,
                descriptor.ArtifactVersion,
                descriptor.Sha256Digest,
                descriptor.EvidenceReference,
                descriptor.CompatibilityIdentity);

            var accepted = catalog.Evaluate(request);
            Check(accepted.AcceptedForTechnicalConsumption, "exact Stage 13 canonical consumption rejected");
            Check(accepted.Reason == "EXACT_ARTIFACT_CONSUMPTION_ACCEPTED", "exact Stage 13 consumption reason mismatch");
            Check(accepted.ExactArtifactIdentity == descriptor.ExactIdentity, "consumption exact identity mismatch");
            Check(!accepted.ActivationAuthorized, "consumption authorized activation");
            Check(!accepted.DeploymentAuthorized, "consumption authorized deployment");
            Check(!accepted.ProductionAuthorized, "consumption authorized production");
            Check(!accepted.BusinessAuthorityGranted, "consumption granted business authority");
            Check(!accepted.SilentUpgradePerformed, "consumption silently upgraded artifact");

            Check(!catalog.Evaluate(request with { ArtifactId = "foundation/contracts/other" }).AcceptedForTechnicalConsumption, "wrong artifact id accepted");
            Check(!catalog.Evaluate(request with { ArtifactVersion = "1.0.1" }).AcceptedForTechnicalConsumption, "wrong artifact version accepted");
            Check(!catalog.Evaluate(request with { Sha256Digest = "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" }).AcceptedForTechnicalConsumption, "wrong digest accepted");
            Check(!catalog.Evaluate(request with { EvidenceReference = "evidence:other" }).AcceptedForTechnicalConsumption, "wrong evidence accepted");
            Check(!catalog.Evaluate(request with { CompatibilityIdentity = "compat:other" }).AcceptedForTechnicalConsumption, "wrong compatibility accepted");

            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneAuthoritativeSource == "src/Foundation.ArtifactPublication/CanonicalFoundationArtifacts.cs", "authoritative publication source mismatch");
            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneSourceContract == "Foundation.Authority.AiKillControlPlaneContract", "source contract mismatch");
            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneGoverningCommit == "8453bd5961eb4ef3c9e35650f94d8c91ad0b81bc", "governing commit mismatch");

            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneCanonicalPayload.Contains("FOUNDATION_KILL_ENFORCEMENT=FOUNDATION_OWNED", StringComparison.Ordinal), "Foundation Kill ownership invariant missing");
            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneCanonicalPayload.Contains("APPLICATION_AI_BUSINESS_SEMANTICS=APPLICATION_OWNED", StringComparison.Ordinal), "Application business semantics invariant missing");
            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneCanonicalPayload.Contains("GLOBAL_AI_KILL!=FALCON_SHUTDOWN", StringComparison.Ordinal), "global AI kill separation missing");
            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneCanonicalPayload.Contains("AI_RESTART!=AUTHORITY_RESTORATION", StringComparison.Ordinal), "AI restart separation missing");
            Check(CanonicalFoundationArtifacts.Stage13AiKillControlPlaneCanonicalPayload.Contains("TECHNICAL_CONSUMPTION!=BUSINESS_AUTHORITY", StringComparison.Ordinal), "technical consumption separation missing");

            Console.WriteLine("CANONICAL_ARTIFACT_PUBLICATION_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("STAGE13_AI_KILL_DESCRIPTOR = PUBLISHED");
            Console.WriteLine("STAGE6_RESOURCE_STATE_DESCRIPTOR = PUBLISHED");
            Console.WriteLine("STAGE6_AGGREGATE_RESOURCE_STATE_DESCRIPTOR = PUBLISHED");
            Console.WriteLine($"ARTIFACT_ID = {descriptor.ArtifactId}");
            Console.WriteLine($"ARTIFACT_VERSION = {descriptor.ArtifactVersion}");
            Console.WriteLine($"SHA256 = {descriptor.Sha256Digest}");
            Console.WriteLine($"EVIDENCE_REFERENCE = {descriptor.EvidenceReference}");
            Console.WriteLine($"COMPATIBILITY_IDENTITY = {descriptor.CompatibilityIdentity}");
            Console.WriteLine($"AUTHORITATIVE_PUBLICATION_SOURCE = {CanonicalFoundationArtifacts.Stage13AiKillControlPlaneAuthoritativeSource}");
            Console.WriteLine($"GOVERNING_FOUNDATION_COMMIT = {CanonicalFoundationArtifacts.Stage13AiKillControlPlaneGoverningCommit}");
            Console.WriteLine("CANONICAL_ARTIFACT_CONSUMPTION != RUNTIME_ACTIVATION");
            Console.WriteLine("TECHNICAL_CONSUMPTION != BUSINESS_AUTHORITY");
            Console.WriteLine("FOUNDATION_KILL_ENFORCEMENT = FOUNDATION_OWNED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("CANONICAL_ARTIFACT_PUBLICATION_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
