using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foundation.Admission;
using Foundation.ContractRegistry;
using Foundation.Contracts;
using Foundation.ServiceCatalog;

namespace Foundation.DependencyGovernance;

public enum DependencySubjectKind
{
    FoundationService,
    Application,
    External
}

public enum DependencyRelationship
{
    Required,
    Optional,
    Conditional,
    Prohibited
}

public enum DependencyConditionState
{
    RequiredNow,
    NotRequiredNow
}

public enum DependencyLifecycleOrderRule
{
    DependencyBeforeConsumer,
    ConsumerBeforeDependency,
    NoRelativeOrder
}

public sealed record DependencyLifecycleOrderDeclaration(
    DependencyLifecycleOrderRule Startup,
    DependencyLifecycleOrderRule Shutdown,
    DependencyLifecycleOrderRule Update,
    DependencyLifecycleOrderRule Recovery,
    DependencyLifecycleOrderRule Removal);

public readonly record struct DependencySubjectKey(string Identity, string Version)
{
    public string CanonicalText => $"{Identity.Length}:{Identity}|{Version.Length}:{Version}";

    public override string ToString() => CanonicalText;
}

public readonly record struct DependencyGraphKey(string Identity, string Version)
{
    public string CanonicalText => $"{Identity.Length}:{Identity}|{Version.Length}:{Version}";

    public override string ToString() => CanonicalText;
}

public readonly record struct DependencyEdgeKey(
    DependencySubjectKey Consumer,
    DependencySubjectKey Dependency,
    DependencyRelationship Relationship,
    DependencyConditionState? ConditionState,
    string? ResolvedVersion)
{
    public string CanonicalText
        => $"{Consumer.CanonicalText.Length}:{Consumer.CanonicalText}|{Dependency.CanonicalText.Length}:{Dependency.CanonicalText}|{Relationship}|{ConditionState?.ToString() ?? string.Empty}|{(ResolvedVersion ?? string.Empty).Length}:{ResolvedVersion ?? string.Empty}";

    public override string ToString() => CanonicalText;
}

public abstract record DependencySubjectEvidence
{
    public DependencySubjectKind SubjectKind { get; init; }
    public DependencySubjectKey SubjectKey { get; init; }
    public string EvidenceReference { get; init; } = string.Empty;
}

public sealed record FoundationServiceSubjectEvidence : DependencySubjectEvidence
{
    public ServiceCatalogEntry CatalogEntry { get; init; } = null!;
}

public sealed record ApplicationSubjectEvidence : DependencySubjectEvidence
{
    public AdmissionRequest AdmissionRequest { get; init; } = null!;
    public AdmissionDecision AdmissionDecision { get; init; } = null!;
    public ApplicationManifest Manifest { get; init; } = null!;
    public string ManifestDigest { get; init; } = string.Empty;
    public string AdmissionEvidenceIdentity { get; init; } = string.Empty;
}

public sealed record ExternalDependencySubjectEvidence : DependencySubjectEvidence
{
    public string Owner { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string IntegrityDigest { get; init; } = string.Empty;
    public string AvailabilityResult { get; init; } = string.Empty;
    public string ContainmentEvidence { get; init; } = string.Empty;
    public DateTimeOffset EffectiveTime { get; init; }
    public DateTimeOffset Expiry { get; init; }
}

public sealed record DependencyDeclaration
{
    public DependencySubjectKey Consumer { get; init; }
    public string DependencyIdentity { get; init; } = string.Empty;
    public IReadOnlyList<string> CompatibleVersions { get; init; } = Array.Empty<string>();
    public DependencyRelationship Relationship { get; init; }
    public DependencyConditionState? ConditionState { get; init; }
    public string? ResolvedVersion { get; init; }
    public DependencySubjectKind DependencyKind { get; init; }
    public string DependencySource { get; init; } = string.Empty;
    public string DeclaredPurpose { get; init; } = string.Empty;
    public string IntegrityRequirement { get; init; } = string.Empty;
    public string AvailabilityRequirement { get; init; } = string.Empty;
    public string TimeoutPolicy { get; init; } = string.Empty;
    public string DegradedStatePolicy { get; init; } = string.Empty;
    public string IsolationBoundary { get; init; } = string.Empty;
    public string FailurePropagationLimit { get; init; } = string.Empty;
    public string ReplacementPolicy { get; init; } = string.Empty;
    public string MigrationPolicy { get; init; } = string.Empty;
    public string RollbackPolicy { get; init; } = string.Empty;
    public string EvidenceRequirement { get; init; } = string.Empty;
    public string DelegationChainEvidenceReference { get; init; } = string.Empty;
    public DependencyLifecycleOrderDeclaration? LifecycleOrder { get; init; }
}

public sealed record DependencyGraphRequest
{
    public string GraphId { get; init; } = string.Empty;
    public string GraphVersion { get; init; } = string.Empty;
    public string RequesterIdentity { get; init; } = string.Empty;
    public string AuthoritySource { get; init; } = string.Empty;
    public DateTimeOffset ObservationTime { get; init; }
    public ManifestSurfaceRecord ManifestSurface { get; init; } = null!;
    public DelegationRecord DelegationEvidence { get; init; } = null!;
    public IReadOnlyList<DependencySubjectEvidence> Subjects { get; init; } = Array.Empty<DependencySubjectEvidence>();
    public IReadOnlyList<DependencyDeclaration> Dependencies { get; init; } = Array.Empty<DependencyDeclaration>();
    public IReadOnlyList<DependencySubjectKey> ProposedActivationOrder { get; init; } = Array.Empty<DependencySubjectKey>();
    public DependencyGraphKey GraphKey => new(GraphId, GraphVersion);
}

public sealed record DependencySubjectSnapshot
{
    public DependencySubjectKind SubjectKind { get; init; }
    public DependencySubjectKey SubjectKey { get; init; }
    public string EvidenceReference { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string IntegrityDigest { get; init; } = string.Empty;
    public string AvailabilityResult { get; init; } = string.Empty;
    public string ContainmentEvidence { get; init; } = string.Empty;
    public DateTimeOffset EffectiveTime { get; init; }
    public DateTimeOffset Expiry { get; init; }
    public string AdmissionEvidenceIdentity { get; init; } = string.Empty;
    public string ManifestIdentity { get; init; } = string.Empty;
    public string ManifestDigest { get; init; } = string.Empty;
    public string AdmissionRequestIdentity { get; init; } = string.Empty;
    public string AdmissionDecisionIdentity { get; init; } = string.Empty;
    public string AdmissionDecisionReason { get; init; } = string.Empty;
    public string AdmissionRequestProvenanceId { get; init; } = string.Empty;
    public ReadOnlyCollection<string> ManifestDeclaredDependencies { get; init; } = new(Array.Empty<string>());
    public ReadOnlyCollection<string> ManifestRequiredFoundationServices { get; init; } = new(Array.Empty<string>());
    public string ServiceCatalogIdentity { get; init; } = string.Empty;
    public string ServiceCatalogVersion { get; init; } = string.Empty;
    public string ServiceCatalogOwner { get; init; } = string.Empty;
    public string ServiceCatalogManifestIdentity { get; init; } = string.Empty;
    public string ServiceCatalogManifestDigest { get; init; } = string.Empty;
}

public sealed record DependencyResolutionSnapshot
{
    public DependencyEdgeKey EdgeKey { get; init; }
    public DependencySubjectKind DependencyKind { get; init; }
    public string DependencySource { get; init; } = string.Empty;
    public string DeclaredPurpose { get; init; } = string.Empty;
    public string IntegrityRequirement { get; init; } = string.Empty;
    public string AvailabilityRequirement { get; init; } = string.Empty;
    public string TimeoutPolicy { get; init; } = string.Empty;
    public string DegradedStatePolicy { get; init; } = string.Empty;
    public string IsolationBoundary { get; init; } = string.Empty;
    public string FailurePropagationLimit { get; init; } = string.Empty;
    public string ReplacementPolicy { get; init; } = string.Empty;
    public string MigrationPolicy { get; init; } = string.Empty;
    public string RollbackPolicy { get; init; } = string.Empty;
    public string EvidenceRequirement { get; init; } = string.Empty;
    public string DelegationChainEvidenceReference { get; init; } = string.Empty;
    public DependencyLifecycleOrderDeclaration? LifecycleOrder { get; init; }
    public ReadOnlyCollection<string> CompatibleVersions { get; init; } = new(Array.Empty<string>());
    public DependencyRelationship Relationship { get; init; }
    public DependencyConditionState? ConditionState { get; init; }
    public string? ResolvedVersion { get; init; }
}

public sealed record DependencyGraphSnapshot
{
    public ReadOnlyCollection<DependencySubjectSnapshot> Subjects { get; init; } = new(Array.Empty<DependencySubjectSnapshot>());
    public ReadOnlyCollection<DependencyResolutionSnapshot> ResolvedDependencies { get; init; } = new(Array.Empty<DependencyResolutionSnapshot>());
    public ReadOnlyCollection<DependencyResolutionSnapshot> UnresolvedOptionalDependencies { get; init; } = new(Array.Empty<DependencyResolutionSnapshot>());
    public ReadOnlyCollection<DependencySubjectKey> CanonicalActivationOrder { get; init; } = new(Array.Empty<DependencySubjectKey>());
    public DependencyGraphKey GraphKey { get; init; }
}

public sealed record DependencyValidationResult
{
    public bool Success { get; init; }
    public string ReasonCode { get; init; } = string.Empty;
    public string DecisionIdentity { get; init; } = string.Empty;
    public string GraphDecision { get; init; } = string.Empty;
    public string ActivationOrderDecision { get; init; } = string.Empty;
    public DependencyGraphKey GraphKey { get; init; }
    public string CanonicalGraphText { get; init; } = string.Empty;
    public string GraphDigest { get; init; } = string.Empty;
    public string CanonicalActivationOrderText { get; init; } = string.Empty;
    public string ActivationOrderDigest { get; init; } = string.Empty;
    public DependencyGraphSnapshot ImmutableGraphSnapshot { get; init; } = new();
    public ReadOnlyCollection<DependencyResolutionSnapshot> ResolvedDependencies { get; init; } = new(Array.Empty<DependencyResolutionSnapshot>());
    public ReadOnlyCollection<DependencyResolutionSnapshot> UnresolvedOptionalDependencies { get; init; } = new(Array.Empty<DependencyResolutionSnapshot>());
    public ReadOnlyCollection<FilEvent> EvidenceEvents { get; init; } = new(Array.Empty<FilEvent>());
    public string? CycleEvidence { get; init; }
}
