using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.ArtifactPublication;

public enum FoundationArtifactKind
{
    Contract,
    Schema,
    Specification,
    RuntimePackage,
    EvidenceBundle,
    OperationalProjection
}

public enum FoundationArtifactPublicationState
{
    Published,
    Superseded,
    Revoked
}

public sealed record FoundationArtifactDescriptor(
    string ArtifactId,
    string ArtifactVersion,
    string Sha256Digest,
    FoundationArtifactKind Kind,
    string ProducerIdentity,
    string ProvenanceReference,
    string EvidenceReference,
    string CompatibilityIdentity,
    FoundationArtifactPublicationState State,
    DateTimeOffset PublishedAt)
{
    public string ExactIdentity => ArtifactPublicationRuntime.ExactIdentity(this);
}

public sealed record ArtifactPublicationCandidate(
    FoundationArtifactDescriptor Descriptor,
    bool GovernanceAccepted,
    bool Immutable,
    bool IntegrityVerified,
    bool EvidenceValid,
    bool ProvenanceValid);

public sealed record ArtifactPublicationDecision(
    bool EligibleForPublication,
    string Reason,
    string ExactArtifactIdentity,
    bool ActivationAuthorized,
    bool DeploymentAuthorized,
    bool BusinessAuthorityGranted);

public sealed record ArtifactConsumptionRequest(
    string ConsumerApplicationId,
    string ArtifactId,
    string ArtifactVersion,
    string Sha256Digest,
    string EvidenceReference,
    string CompatibilityIdentity);

public sealed record ArtifactConsumptionDecision(
    bool AcceptedForTechnicalConsumption,
    string Reason,
    string ExactArtifactIdentity,
    bool ActivationAuthorized,
    bool DeploymentAuthorized,
    bool ProductionAuthorized,
    bool BusinessAuthorityGranted,
    bool SilentUpgradePerformed);

public sealed class FoundationArtifactCatalog
{
    private readonly Dictionary<string, FoundationArtifactDescriptor> _exact = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _versionDigests = new(StringComparer.Ordinal);

    public FoundationArtifactCatalog(IEnumerable<FoundationArtifactDescriptor> descriptors)
    {
        if (descriptors is null)
        {
            throw new ArgumentNullException(nameof(descriptors));
        }

        foreach (var descriptor in descriptors)
        {
            if (!ArtifactPublicationRuntime.ValidDescriptor(descriptor))
            {
                throw new InvalidOperationException("INVALID_ARTIFACT_DESCRIPTOR");
            }

            var versionKey = descriptor.ArtifactId + "|" + descriptor.ArtifactVersion;
            if (_versionDigests.TryGetValue(versionKey, out var existingDigest) &&
                !string.Equals(existingDigest, descriptor.Sha256Digest, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("CONFLICTING_ARTIFACT_VERSION_DIGEST");
            }

            _versionDigests[versionKey] = descriptor.Sha256Digest;
            var exactKey = ExactKey(descriptor.ArtifactId, descriptor.ArtifactVersion, descriptor.Sha256Digest);

            if (_exact.TryGetValue(exactKey, out var existing) && existing != descriptor)
            {
                throw new InvalidOperationException("CONFLICTING_EXACT_PUBLICATION");
            }

            _exact[exactKey] = descriptor;
        }
    }

    public int Count => _exact.Count;

    public ArtifactConsumptionDecision Evaluate(ArtifactConsumptionRequest request)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(request.ConsumerApplicationId) ||
            string.IsNullOrWhiteSpace(request.ArtifactId) ||
            string.IsNullOrWhiteSpace(request.ArtifactVersion) ||
            !ArtifactPublicationRuntime.ValidSha256(request.Sha256Digest) ||
            string.IsNullOrWhiteSpace(request.EvidenceReference) ||
            string.IsNullOrWhiteSpace(request.CompatibilityIdentity))
        {
            return Deny("INVALID_CONSUMPTION_REQUEST");
        }

        var exactKey = ExactKey(request.ArtifactId, request.ArtifactVersion, request.Sha256Digest);
        if (!_exact.TryGetValue(exactKey, out var descriptor))
        {
            return Deny("EXACT_ARTIFACT_NOT_FOUND");
        }

        if (descriptor.State == FoundationArtifactPublicationState.Revoked)
        {
            return Deny("ARTIFACT_REVOKED", descriptor.ExactIdentity);
        }

        if (descriptor.State == FoundationArtifactPublicationState.Superseded)
        {
            return Deny("ARTIFACT_SUPERSEDED_NO_SILENT_UPGRADE", descriptor.ExactIdentity);
        }

        if (!string.Equals(request.EvidenceReference, descriptor.EvidenceReference, StringComparison.Ordinal))
        {
            return Deny("EVIDENCE_BINDING_MISMATCH", descriptor.ExactIdentity);
        }

        if (!string.Equals(request.CompatibilityIdentity, descriptor.CompatibilityIdentity, StringComparison.Ordinal))
        {
            return Deny("COMPATIBILITY_BINDING_MISMATCH", descriptor.ExactIdentity);
        }

        return new ArtifactConsumptionDecision(
            true,
            "EXACT_ARTIFACT_CONSUMPTION_ACCEPTED",
            descriptor.ExactIdentity,
            false,
            false,
            false,
            false,
            false);
    }

    public bool TryGetExact(string artifactId, string artifactVersion, string sha256Digest, out FoundationArtifactDescriptor? descriptor)
    {
        descriptor = null;
        if (string.IsNullOrWhiteSpace(artifactId) || string.IsNullOrWhiteSpace(artifactVersion) || !ArtifactPublicationRuntime.ValidSha256(sha256Digest))
        {
            return false;
        }

        return _exact.TryGetValue(ExactKey(artifactId, artifactVersion, sha256Digest), out descriptor);
    }

    private static string ExactKey(string artifactId, string artifactVersion, string digest) =>
        artifactId.Trim() + "|" + artifactVersion.Trim() + "|" + digest.Trim().ToUpperInvariant();

    private static ArtifactConsumptionDecision Deny(string reason, string exactIdentity = "NONE") =>
        new(false, reason, exactIdentity, false, false, false, false, false);
}

public sealed record FoundationOperationalTruth(
    string FoundationIdentity,
    string FoundationReleaseState,
    string HealthState,
    string AuthorityState,
    string LifecycleState,
    int ApplicationCount,
    string EvidenceReference,
    DateTimeOffset ObservedAt);

public sealed record FoundationOperationalProjection(
    string ProjectionIdentity,
    string FoundationIdentity,
    string FoundationReleaseState,
    string HealthState,
    string AuthorityState,
    string LifecycleState,
    int ApplicationCount,
    string EvidenceReference,
    DateTimeOffset ObservedAt,
    bool PresentationOnly,
    bool CarriesExecutionAuthority,
    bool CarriesBusinessAuthority);

public sealed record OperationalProjectionDecision(
    bool Accepted,
    string Reason,
    FoundationOperationalProjection? Projection);

public static class ArtifactPublicationRuntime
{
    private static readonly string[] MovingReferenceTokens =
    {
        "refs/heads/",
        "branch/",
        "foundation-development",
        "application-development",
        "web-development",
        "reference/fsats-v1.3-scratch"
    };

    public static ArtifactPublicationDecision EvaluatePublication(ArtifactPublicationCandidate candidate)
    {
        if (candidate is null || !ValidDescriptor(candidate.Descriptor))
        {
            return PublicationDeny("INVALID_ARTIFACT_DESCRIPTOR");
        }

        if (candidate.Descriptor.State != FoundationArtifactPublicationState.Published)
        {
            return PublicationDeny("NON_PUBLISHED_STATE_NOT_ELIGIBLE", candidate.Descriptor.ExactIdentity);
        }

        if (!candidate.GovernanceAccepted)
        {
            return PublicationDeny("GOVERNANCE_ACCEPTANCE_MISSING", candidate.Descriptor.ExactIdentity);
        }

        if (!candidate.Immutable)
        {
            return PublicationDeny("ARTIFACT_NOT_IMMUTABLE", candidate.Descriptor.ExactIdentity);
        }

        if (!candidate.IntegrityVerified)
        {
            return PublicationDeny("INTEGRITY_NOT_VERIFIED", candidate.Descriptor.ExactIdentity);
        }

        if (!candidate.EvidenceValid)
        {
            return PublicationDeny("PUBLICATION_EVIDENCE_INVALID", candidate.Descriptor.ExactIdentity);
        }

        if (!candidate.ProvenanceValid || !ImmutableProvenance(candidate.Descriptor.ProvenanceReference))
        {
            return PublicationDeny("PROVENANCE_NOT_IMMUTABLE", candidate.Descriptor.ExactIdentity);
        }

        return new ArtifactPublicationDecision(
            true,
            "ELIGIBLE_FOR_CANONICAL_PUBLICATION",
            candidate.Descriptor.ExactIdentity,
            false,
            false,
            false);
    }

    public static OperationalProjectionDecision BuildOperationalProjection(FoundationOperationalTruth truth)
    {
        if (truth is null ||
            string.IsNullOrWhiteSpace(truth.FoundationIdentity) ||
            string.IsNullOrWhiteSpace(truth.FoundationReleaseState) ||
            string.IsNullOrWhiteSpace(truth.HealthState) ||
            string.IsNullOrWhiteSpace(truth.AuthorityState) ||
            string.IsNullOrWhiteSpace(truth.LifecycleState) ||
            truth.ApplicationCount < 0 ||
            string.IsNullOrWhiteSpace(truth.EvidenceReference) ||
            truth.ObservedAt == default)
        {
            return new OperationalProjectionDecision(false, "INVALID_OPERATIONAL_TRUTH", null);
        }

        var canonical = string.Join("|", new[]
        {
            truth.FoundationIdentity.Trim(),
            truth.FoundationReleaseState.Trim(),
            truth.HealthState.Trim(),
            truth.AuthorityState.Trim(),
            truth.LifecycleState.Trim(),
            truth.ApplicationCount.ToString(CultureInfo.InvariantCulture),
            truth.EvidenceReference.Trim(),
            truth.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        var projection = new FoundationOperationalProjection(
            "sha256/" + Sha256Hex(canonical),
            truth.FoundationIdentity.Trim(),
            truth.FoundationReleaseState.Trim(),
            truth.HealthState.Trim(),
            truth.AuthorityState.Trim(),
            truth.LifecycleState.Trim(),
            truth.ApplicationCount,
            truth.EvidenceReference.Trim(),
            truth.ObservedAt,
            true,
            false,
            false);

        return new OperationalProjectionDecision(true, "FOUNDATION_OPERATIONAL_PROJECTION_AVAILABLE", projection);
    }

    public static bool ValidDescriptor(FoundationArtifactDescriptor descriptor)
    {
        return descriptor is not null &&
               !string.IsNullOrWhiteSpace(descriptor.ArtifactId) &&
               !string.IsNullOrWhiteSpace(descriptor.ArtifactVersion) &&
               ValidSha256(descriptor.Sha256Digest) &&
               !string.IsNullOrWhiteSpace(descriptor.ProducerIdentity) &&
               !string.IsNullOrWhiteSpace(descriptor.ProvenanceReference) &&
               !string.IsNullOrWhiteSpace(descriptor.EvidenceReference) &&
               !string.IsNullOrWhiteSpace(descriptor.CompatibilityIdentity) &&
               descriptor.PublishedAt != default;
    }

    public static bool ValidSha256(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.Trim();
        if (!text.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var hex = text.Substring(7);
        return hex.Length == 64 && hex.All(Uri.IsHexDigit);
    }

    public static bool ImmutableProvenance(string provenanceReference)
    {
        if (string.IsNullOrWhiteSpace(provenanceReference))
        {
            return false;
        }

        var value = provenanceReference.Trim();
        var lower = value.ToLowerInvariant();
        if (MovingReferenceTokens.Any(lower.Contains))
        {
            return false;
        }

        if (lower.StartsWith("commit/", StringComparison.Ordinal))
        {
            var commit = value.Substring(7);
            return (commit.Length == 40 || commit.Length == 64) && commit.All(Uri.IsHexDigit);
        }

        return ValidSha256(value);
    }

    public static string ExactIdentity(FoundationArtifactDescriptor descriptor)
    {
        if (!ValidDescriptor(descriptor))
        {
            return "INVALID";
        }

        var canonical = string.Join("|", new[]
        {
            descriptor.ArtifactId.Trim(),
            descriptor.ArtifactVersion.Trim(),
            descriptor.Sha256Digest.Trim().ToUpperInvariant(),
            descriptor.Kind.ToString(),
            descriptor.ProducerIdentity.Trim(),
            descriptor.ProvenanceReference.Trim(),
            descriptor.EvidenceReference.Trim(),
            descriptor.CompatibilityIdentity.Trim(),
            descriptor.State.ToString(),
            descriptor.PublishedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        });

        return "sha256/" + Sha256Hex(canonical);
    }

    private static ArtifactPublicationDecision PublicationDeny(string reason, string identity = "NONE") =>
        new(false, reason, identity, false, false, false);

    private static string Sha256Hex(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
