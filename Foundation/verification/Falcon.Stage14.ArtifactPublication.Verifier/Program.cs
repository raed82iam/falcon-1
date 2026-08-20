using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.ArtifactPublication;

namespace Falcon.Stage14.ArtifactPublication.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 16, 18, 30, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            var artifact = Descriptor("foundation/contracts", "1.0.0", Digest('A'), FoundationArtifactPublicationState.Published);
            var candidate = new ArtifactPublicationCandidate(artifact, true, true, true, true, true);
            var publication = ArtifactPublicationRuntime.EvaluatePublication(candidate);

            Check(publication.EligibleForPublication, "valid artifact not eligible for publication");
            Check(publication.Reason == "ELIGIBLE_FOR_CANONICAL_PUBLICATION", "publication reason incorrect");
            Check(publication.ExactArtifactIdentity == artifact.ExactIdentity, "publication exact identity mismatch");
            Check(!publication.ActivationAuthorized, "publication authorized activation");
            Check(!publication.DeploymentAuthorized, "publication authorized deployment");
            Check(!publication.BusinessAuthorityGranted, "publication granted business authority");
            Check(artifact.ExactIdentity.StartsWith("sha256/", StringComparison.Ordinal), "artifact exact identity not SHA-256 based");
            Check(artifact.ExactIdentity.Length == 71, "artifact exact identity length invalid");
            Check(artifact.ExactIdentity == Descriptor("foundation/contracts", "1.0.0", Digest('A'), FoundationArtifactPublicationState.Published).ExactIdentity, "artifact identity not deterministic");

            Check(!ArtifactPublicationRuntime.EvaluatePublication(candidate with { GovernanceAccepted = false }).EligibleForPublication, "unaccepted artifact published");
            Check(!ArtifactPublicationRuntime.EvaluatePublication(candidate with { Immutable = false }).EligibleForPublication, "mutable artifact published");
            Check(!ArtifactPublicationRuntime.EvaluatePublication(candidate with { IntegrityVerified = false }).EligibleForPublication, "unverified artifact published");
            Check(!ArtifactPublicationRuntime.EvaluatePublication(candidate with { EvidenceValid = false }).EligibleForPublication, "invalid evidence artifact published");
            Check(!ArtifactPublicationRuntime.EvaluatePublication(candidate with { ProvenanceValid = false }).EligibleForPublication, "invalid provenance artifact published");
            Check(!ArtifactPublicationRuntime.EvaluatePublication(candidate with { Descriptor = artifact with { State = FoundationArtifactPublicationState.Revoked } }).EligibleForPublication, "revoked artifact published");
            Check(!ArtifactPublicationRuntime.EvaluatePublication(candidate with { Descriptor = artifact with { State = FoundationArtifactPublicationState.Superseded } }).EligibleForPublication, "superseded artifact published");

            Check(ArtifactPublicationRuntime.ValidSha256(Digest('B')), "valid SHA-256 rejected");
            Check(!ArtifactPublicationRuntime.ValidSha256("AABB"), "bare digest accepted");
            Check(!ArtifactPublicationRuntime.ValidSha256("sha256/1234"), "short digest accepted");
            Check(!ArtifactPublicationRuntime.ValidSha256("sha256/" + new string('Z', 64)), "non-hex digest accepted");

            Check(ArtifactPublicationRuntime.ImmutableProvenance("commit/0123456789abcdef0123456789abcdef01234567"), "immutable commit provenance rejected");
            Check(ArtifactPublicationRuntime.ImmutableProvenance(Digest('C')), "immutable digest provenance rejected");
            Check(!ArtifactPublicationRuntime.ImmutableProvenance("refs/heads/foundation-development"), "moving refs/heads provenance accepted");
            Check(!ArtifactPublicationRuntime.ImmutableProvenance("branch/main"), "moving branch provenance accepted");
            Check(!ArtifactPublicationRuntime.ImmutableProvenance("foundation-development"), "moving branch name accepted");
            Check(!ArtifactPublicationRuntime.ImmutableProvenance("application-development"), "Application moving branch accepted");
            Check(!ArtifactPublicationRuntime.ImmutableProvenance("web-development"), "Web moving branch accepted");

            var catalog = new FoundationArtifactCatalog(new[] { artifact });
            Check(catalog.Count == 1, "catalog count invalid");
            Check(catalog.TryGetExact(artifact.ArtifactId, artifact.ArtifactVersion, artifact.Sha256Digest, out var exact), "exact artifact lookup failed");
            Check(exact == artifact, "exact artifact lookup returned wrong descriptor");
            Check(!catalog.TryGetExact(artifact.ArtifactId, "latest", artifact.Sha256Digest, out _), "latest alias resolved");
            Check(!catalog.TryGetExact(artifact.ArtifactId, artifact.ArtifactVersion, Digest('D'), out _), "wrong digest resolved");

            var request = Request(artifact);
            var accepted = catalog.Evaluate(request);
            Check(accepted.AcceptedForTechnicalConsumption, "exact artifact consumption rejected");
            Check(accepted.ExactArtifactIdentity == artifact.ExactIdentity, "consumption identity mismatch");
            Check(!accepted.ActivationAuthorized, "consumption authorized activation");
            Check(!accepted.DeploymentAuthorized, "consumption authorized deployment");
            Check(!accepted.ProductionAuthorized, "consumption authorized production");
            Check(!accepted.BusinessAuthorityGranted, "consumption granted business authority");
            Check(!accepted.SilentUpgradePerformed, "consumption performed silent upgrade");

            Check(!catalog.Evaluate(request with { ConsumerApplicationId = "" }).AcceptedForTechnicalConsumption, "anonymous consumer accepted");
            Check(!catalog.Evaluate(request with { ArtifactVersion = "1.0.1" }).AcceptedForTechnicalConsumption, "wrong version accepted");
            Check(!catalog.Evaluate(request with { Sha256Digest = Digest('E') }).AcceptedForTechnicalConsumption, "wrong digest accepted");
            Check(!catalog.Evaluate(request with { EvidenceReference = "evidence:other" }).AcceptedForTechnicalConsumption, "wrong evidence accepted");
            Check(!catalog.Evaluate(request with { CompatibilityIdentity = "compat:other" }).AcceptedForTechnicalConsumption, "wrong compatibility accepted");
            Check(!catalog.Evaluate(request with { ArtifactId = "foundation/other" }).AcceptedForTechnicalConsumption, "wrong artifact ID accepted");

            var revokedCatalog = new FoundationArtifactCatalog(new[] { artifact with { State = FoundationArtifactPublicationState.Revoked } });
            var revokedDecision = revokedCatalog.Evaluate(request);
            Check(!revokedDecision.AcceptedForTechnicalConsumption, "revoked artifact consumed");
            Check(revokedDecision.Reason == "ARTIFACT_REVOKED", "revoked artifact reason incorrect");
            Check(!revokedDecision.SilentUpgradePerformed, "revoked artifact silently upgraded");

            var supersededCatalog = new FoundationArtifactCatalog(new[] { artifact with { State = FoundationArtifactPublicationState.Superseded } });
            var supersededDecision = supersededCatalog.Evaluate(request);
            Check(!supersededDecision.AcceptedForTechnicalConsumption, "superseded artifact consumed");
            Check(supersededDecision.Reason == "ARTIFACT_SUPERSEDED_NO_SILENT_UPGRADE", "superseded artifact reason incorrect");
            Check(!supersededDecision.SilentUpgradePerformed, "superseded artifact silently upgraded");

            var conflictingThrown = false;
            try
            {
                _ = new FoundationArtifactCatalog(new[] { artifact, artifact with { Sha256Digest = Digest('F') } });
            }
            catch (InvalidOperationException ex)
            {
                conflictingThrown = ex.Message == "CONFLICTING_ARTIFACT_VERSION_DIGEST";
            }
            Check(conflictingThrown, "same ID/version different digest conflict not rejected");

            var duplicateCatalog = new FoundationArtifactCatalog(new[] { artifact, artifact });
            Check(duplicateCatalog.Count == 1, "identical duplicate biased catalog");

            var truth = new FoundationOperationalTruth(
                "falcon-foundation",
                "STAGE13_ACCEPTED_AND_CLOSED",
                "HEALTHY",
                "GOVERNED",
                "RUNNING",
                0,
                "evidence:foundation:operational:1",
                Now);

            var projectionDecision = ArtifactPublicationRuntime.BuildOperationalProjection(truth);
            Check(projectionDecision.Accepted, "valid Foundation operational truth rejected");
            Check(projectionDecision.Projection is not null, "projection missing");
            var projection = projectionDecision.Projection!;
            Check(projection.ProjectionIdentity.StartsWith("sha256/", StringComparison.Ordinal), "projection identity not SHA-256 based");
            Check(projection.ApplicationCount == 0, "zero-Application projection invalid");
            Check(projection.PresentationOnly, "projection not presentation-only");
            Check(!projection.CarriesExecutionAuthority, "projection carries execution authority");
            Check(!projection.CarriesBusinessAuthority, "projection carries business authority");
            Check(projection.EvidenceReference == truth.EvidenceReference, "projection evidence binding mismatch");
            Check(projection.ProjectionIdentity == ArtifactPublicationRuntime.BuildOperationalProjection(truth).Projection!.ProjectionIdentity, "projection identity not deterministic");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { ApplicationCount = -1 }).Accepted, "negative Application count accepted");
            Check(ArtifactPublicationRuntime.BuildOperationalProjection(truth with { ApplicationCount = 2 }).Accepted, "multi-Application projection rejected");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { EvidenceReference = "" }).Accepted, "projection without evidence accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { ObservedAt = default }).Accepted, "projection without observation time accepted");

            var publicMethods = typeof(ArtifactPublicationRuntime).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Check(!publicMethods.Any(x => x.Name.Contains("Activate", StringComparison.OrdinalIgnoreCase)), "Stage 15 activation surface leaked into Stage 14");
            Check(!publicMethods.Any(x => x.Name.Contains("Deploy", StringComparison.OrdinalIgnoreCase)), "deployment surface leaked into Stage 14");
            Check(!publicMethods.Any(x => x.Name.Contains("Execute", StringComparison.OrdinalIgnoreCase)), "execution surface leaked into Stage 14");
            Check(!publicMethods.Any(x => x.Name.Contains("Kill", StringComparison.OrdinalIgnoreCase)), "Kill authority leaked into Stage 14");
            Check(!publicMethods.Any(x => x.Name.Contains("Release", StringComparison.OrdinalIgnoreCase)), "release authority leaked into Stage 14");

            var exported = typeof(ArtifactPublicationRuntime).Assembly.GetExportedTypes();
            Check(!exported.Any(x => x.Name.Contains("Trade", StringComparison.OrdinalIgnoreCase)), "trading semantics leaked into Stage 14");
            Check(!exported.Any(x => x.Name.Contains("Broker", StringComparison.OrdinalIgnoreCase)), "broker semantics leaked into Stage 14");
            Check(!exported.Any(x => x.Name.Contains("Strategy", StringComparison.OrdinalIgnoreCase)), "strategy semantics leaked into Stage 14");
            Check(!exported.Any(x => x.Name.Contains("Portfolio", StringComparison.OrdinalIgnoreCase)), "portfolio semantics leaked into Stage 14");
            Check(!exported.Any(x => x.Name.Contains("Market", StringComparison.OrdinalIgnoreCase)), "market semantics leaked into Stage 14");

            Check(_checks >= 75, $"insufficient Stage 14 coverage: {_checks}");

            Console.WriteLine("STAGE14_ARTIFACT_PUBLICATION_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("WP01_CANONICAL_ARTIFACT_IDENTITY = PASS");
            Console.WriteLine("WP02_PUBLICATION_ELIGIBILITY = PASS");
            Console.WriteLine("WP03_IMMUTABLE_PUBLICATION_CATALOG = PASS");
            Console.WriteLine("WP04_EXACT_APPLICATION_CONSUMPTION = PASS");
            Console.WriteLine("WP05_SUPERSESSION_REVOCATION = PASS");
            Console.WriteLine("WP06_FOUNDATION_PUBLIC_OPERATIONAL_PROJECTION = PASS");
            Console.WriteLine("WP07_ZERO_APPLICATION_NEUTRALITY = PASS");
            Console.WriteLine("WP08_ADVERSARIAL_HARDENING = PASS");
            Console.WriteLine("WP09_INTEGRATED_VERIFICATION = PASS");
            Console.WriteLine("SOURCE_TREE != CANONICAL_RUNTIME_ARTIFACT");
            Console.WriteLine("MOVING_BRANCH_HEAD != RUNTIME_CONSUMPTION_IDENTITY");
            Console.WriteLine("PUBLICATION != ACTIVATION");
            Console.WriteLine("PUBLICATION != DEPLOYMENT");
            Console.WriteLine("CONSUMPTION != AUTHORITY");
            Console.WriteLine("TECHNICAL_CONSUMPTION != BUSINESS_AUTHORITY");
            Console.WriteLine("REVOKED_ARTIFACT != CONSUMABLE");
            Console.WriteLine("SUPERSEDED_ARTIFACT != SILENT_AUTO_UPGRADE");
            Console.WriteLine("WEB_PROJECTION != FOUNDATION_AUTHORITY");
            Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE14_ARTIFACT_PUBLICATION_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static FoundationArtifactDescriptor Descriptor(
        string id,
        string version,
        string digest,
        FoundationArtifactPublicationState state)
    {
        return new FoundationArtifactDescriptor(
            id,
            version,
            digest,
            FoundationArtifactKind.Contract,
            "foundation-pipeline",
            "commit/0123456789abcdef0123456789abcdef01234567",
            "evidence:artifact:1",
            "compat:foundation-api:v1",
            state,
            Now);
    }

    private static ArtifactConsumptionRequest Request(FoundationArtifactDescriptor descriptor) =>
        new(
            "application:test",
            descriptor.ArtifactId,
            descriptor.ArtifactVersion,
            descriptor.Sha256Digest,
            descriptor.EvidenceReference,
            descriptor.CompatibilityIdentity);

    private static string Digest(char value) => "sha256/" + new string(value, 64);

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
