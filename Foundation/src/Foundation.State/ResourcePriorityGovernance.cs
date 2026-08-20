using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public sealed record ResourcePriorityClassDefinition
{
    public ResourcePriorityClassDefinition(ResourcePriorityClassId classId, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        ClassId = classId ?? throw new ArgumentNullException(nameof(classId));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
    }
    public ResourcePriorityClassId ClassId { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record TechnicalCriticalityClassDefinition
{
    public TechnicalCriticalityClassDefinition(TechnicalCriticalityClassId classId, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        ClassId = classId ?? throw new ArgumentNullException(nameof(classId));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
    }
    public TechnicalCriticalityClassId ClassId { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record ResourcePriorityClassRelation
{
    public ResourcePriorityClassRelation(ResourcePriorityClassId higherPriorityClassId, ResourcePriorityClassId lowerPriorityClassId, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        HigherPriorityClassId = higherPriorityClassId ?? throw new ArgumentNullException(nameof(higherPriorityClassId));
        LowerPriorityClassId = lowerPriorityClassId ?? throw new ArgumentNullException(nameof(lowerPriorityClassId));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (StringComparer.Ordinal.Equals(HigherPriorityClassId.Value, LowerPriorityClassId.Value)) throw new ArgumentException("A resource-priority class cannot outrank itself.");
    }
    public ResourcePriorityClassId HigherPriorityClassId { get; }
    public ResourcePriorityClassId LowerPriorityClassId { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record TechnicalCriticalityClassRelation
{
    public TechnicalCriticalityClassRelation(TechnicalCriticalityClassId higherCriticalityClassId, TechnicalCriticalityClassId lowerCriticalityClassId, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        HigherCriticalityClassId = higherCriticalityClassId ?? throw new ArgumentNullException(nameof(higherCriticalityClassId));
        LowerCriticalityClassId = lowerCriticalityClassId ?? throw new ArgumentNullException(nameof(lowerCriticalityClassId));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        if (StringComparer.Ordinal.Equals(HigherCriticalityClassId.Value, LowerCriticalityClassId.Value)) throw new ArgumentException("A technical-criticality class cannot outrank itself.");
    }
    public TechnicalCriticalityClassId HigherCriticalityClassId { get; }
    public TechnicalCriticalityClassId LowerCriticalityClassId { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record ApplicationResourcePriorityBinding
{
    public ApplicationResourcePriorityBinding(ApplicationPrincipalId applicationId, ResourcePriorityClassId priorityClassId, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        PriorityClassId = priorityClassId ?? throw new ArgumentNullException(nameof(priorityClassId));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
    }
    public ApplicationPrincipalId ApplicationId { get; }
    public ResourcePriorityClassId PriorityClassId { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record TechnicalCriticalityBinding
{
    public TechnicalCriticalityBinding(ResourceScopeId technicalScopeId, ResourceClassId resourceClassId, TechnicalCriticalityClassId criticalityClassId, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence)
    {
        TechnicalScopeId = technicalScopeId ?? throw new ArgumentNullException(nameof(technicalScopeId));
        ResourceClassId = resourceClassId ?? throw new ArgumentNullException(nameof(resourceClassId));
        CriticalityClassId = criticalityClassId ?? throw new ArgumentNullException(nameof(criticalityClassId));
        Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
    }
    public ResourceScopeId TechnicalScopeId { get; }
    public ResourceClassId ResourceClassId { get; }
    public TechnicalCriticalityClassId CriticalityClassId { get; }
    public ResourceEffectiveLifetime Lifetime { get; }
    public ResourceEvidenceReference Evidence { get; }
}

public sealed record ApplicationResourcePriorityView
{
    internal ApplicationResourcePriorityView(ApplicationPrincipalId applicationId, ApplicationResourcePriorityBinding? binding, ResourcePriorityClassDefinition? priorityClass, string sourceSnapshotIdentitySha256)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        Binding = binding;
        PriorityClass = priorityClass;
        SourceSnapshotIdentitySha256 = sourceSnapshotIdentitySha256 ?? throw new ArgumentNullException(nameof(sourceSnapshotIdentitySha256));
    }
    public ApplicationPrincipalId ApplicationId { get; }
    public ApplicationResourcePriorityBinding? Binding { get; }
    public ResourcePriorityClassDefinition? PriorityClass { get; }
    public string SourceSnapshotIdentitySha256 { get; }
}

public sealed record ResourcePriorityGovernanceSnapshot
{
    private readonly ReadOnlyCollection<ResourcePriorityClassDefinition> _priorityClasses;
    private readonly ReadOnlyCollection<TechnicalCriticalityClassDefinition> _criticalityClasses;
    private readonly ReadOnlyCollection<ResourcePriorityClassRelation> _priorityRelations;
    private readonly ReadOnlyCollection<TechnicalCriticalityClassRelation> _criticalityRelations;
    private readonly ReadOnlyCollection<ApplicationResourcePriorityBinding> _applicationBindings;
    private readonly ReadOnlyCollection<TechnicalCriticalityBinding> _technicalBindings;

    public ResourcePriorityGovernanceSnapshot(ApplicationResourceAllocationSnapshot allocationSnapshot, DateTimeOffset observedAt, string priorityPolicyVersion, ResourceEffectiveLifetime priorityPolicyLifetime, ResourceEvidenceReference priorityPolicyEvidence, string criticalityPolicyVersion, ResourceEffectiveLifetime criticalityPolicyLifetime, ResourceEvidenceReference criticalityPolicyEvidence, IEnumerable<ResourcePriorityClassDefinition> priorityClasses, IEnumerable<TechnicalCriticalityClassDefinition> criticalityClasses, IEnumerable<ResourcePriorityClassRelation> priorityRelations, IEnumerable<TechnicalCriticalityClassRelation> criticalityRelations, IEnumerable<ApplicationResourcePriorityBinding> applicationBindings, IEnumerable<TechnicalCriticalityBinding> technicalBindings, bool policyTruthAvailable)
    {
        AllocationSnapshot = allocationSnapshot ?? throw new ArgumentNullException(nameof(allocationSnapshot));
        PriorityPolicyVersion = ResourcePrimitiveValidation.RequireCanonicalIdentifier(priorityPolicyVersion);
        PriorityPolicyLifetime = priorityPolicyLifetime ?? throw new ArgumentNullException(nameof(priorityPolicyLifetime));
        PriorityPolicyEvidence = priorityPolicyEvidence ?? throw new ArgumentNullException(nameof(priorityPolicyEvidence));
        CriticalityPolicyVersion = ResourcePrimitiveValidation.RequireCanonicalIdentifier(criticalityPolicyVersion);
        CriticalityPolicyLifetime = criticalityPolicyLifetime ?? throw new ArgumentNullException(nameof(criticalityPolicyLifetime));
        CriticalityPolicyEvidence = criticalityPolicyEvidence ?? throw new ArgumentNullException(nameof(criticalityPolicyEvidence));
        ArgumentNullException.ThrowIfNull(priorityClasses); ArgumentNullException.ThrowIfNull(criticalityClasses); ArgumentNullException.ThrowIfNull(priorityRelations); ArgumentNullException.ThrowIfNull(criticalityRelations); ArgumentNullException.ThrowIfNull(applicationBindings); ArgumentNullException.ThrowIfNull(technicalBindings);
        if (!policyTruthAvailable) throw new InvalidOperationException("Resource priority and technical-criticality policy truth is unavailable and must fail closed.");
        if (observedAt < AllocationSnapshot.ObservedAt) throw new ArgumentException("Priority-governance snapshot cannot predate its Application allocation snapshot.", nameof(observedAt));

        ObservedAt = observedAt;
        ValidateEvidenceAndLifetime(PriorityPolicyEvidence, PriorityPolicyLifetime, observedAt, "resource-priority policy");
        ValidateEvidenceAndLifetime(CriticalityPolicyEvidence, CriticalityPolicyLifetime, observedAt, "technical-criticality policy");

        var orderedPriorityClasses = priorityClasses.OrderBy(item => item.ClassId.Value, StringComparer.Ordinal).ToArray();
        var orderedCriticalityClasses = criticalityClasses.OrderBy(item => item.ClassId.Value, StringComparer.Ordinal).ToArray();
        var orderedPriorityRelations = priorityRelations.OrderBy(item => item.HigherPriorityClassId.Value, StringComparer.Ordinal).ThenBy(item => item.LowerPriorityClassId.Value, StringComparer.Ordinal).ToArray();
        var orderedCriticalityRelations = criticalityRelations.OrderBy(item => item.HigherCriticalityClassId.Value, StringComparer.Ordinal).ThenBy(item => item.LowerCriticalityClassId.Value, StringComparer.Ordinal).ToArray();
        var orderedApplicationBindings = applicationBindings.OrderBy(item => item.ApplicationId.Value, StringComparer.Ordinal).ThenBy(item => item.PriorityClassId.Value, StringComparer.Ordinal).ToArray();
        var orderedTechnicalBindings = technicalBindings.OrderBy(item => item.TechnicalScopeId.Value, StringComparer.Ordinal).ThenBy(item => item.ResourceClassId.Value, StringComparer.Ordinal).ThenBy(item => item.CriticalityClassId.Value, StringComparer.Ordinal).ToArray();

        RejectDuplicateIds(orderedPriorityClasses.Select(item => item.ClassId.Value), "resource priority class", nameof(priorityClasses));
        RejectDuplicateIds(orderedCriticalityClasses.Select(item => item.ClassId.Value), "technical criticality class", nameof(criticalityClasses));
        RejectDuplicateIds(orderedPriorityRelations.Select(item => item.HigherPriorityClassId.Value + "|" + item.LowerPriorityClassId.Value), "resource priority relation", nameof(priorityRelations));
        RejectDuplicateIds(orderedCriticalityRelations.Select(item => item.HigherCriticalityClassId.Value + "|" + item.LowerCriticalityClassId.Value), "technical criticality relation", nameof(criticalityRelations));
        RejectDuplicateIds(orderedApplicationBindings.Select(item => item.ApplicationId.Value), "Application priority binding", nameof(applicationBindings));
        RejectDuplicateIds(orderedTechnicalBindings.Select(item => item.TechnicalScopeId.Value + "|" + item.ResourceClassId.Value), "technical criticality scope/resource binding", nameof(technicalBindings));

        foreach (var item in orderedPriorityClasses) ValidateEvidenceAndLifetime(item.Evidence, item.Lifetime, observedAt, "resource priority class");
        foreach (var item in orderedCriticalityClasses) ValidateEvidenceAndLifetime(item.Evidence, item.Lifetime, observedAt, "technical criticality class");
        var priorityClassIds = orderedPriorityClasses.Select(item => item.ClassId.Value).ToHashSet(StringComparer.Ordinal);
        var criticalityClassIds = orderedCriticalityClasses.Select(item => item.ClassId.Value).ToHashSet(StringComparer.Ordinal);

        foreach (var relation in orderedPriorityRelations)
        {
            ValidateEvidenceAndLifetime(relation.Evidence, relation.Lifetime, observedAt, "resource priority relation");
            if (!priorityClassIds.Contains(relation.HigherPriorityClassId.Value) || !priorityClassIds.Contains(relation.LowerPriorityClassId.Value)) throw new ArgumentException("Resource-priority relation references an unknown priority class.", nameof(priorityRelations));
        }
        foreach (var relation in orderedCriticalityRelations)
        {
            ValidateEvidenceAndLifetime(relation.Evidence, relation.Lifetime, observedAt, "technical criticality relation");
            if (!criticalityClassIds.Contains(relation.HigherCriticalityClassId.Value) || !criticalityClassIds.Contains(relation.LowerCriticalityClassId.Value)) throw new ArgumentException("Technical-criticality relation references an unknown criticality class.", nameof(criticalityRelations));
        }
        RejectCycles(priorityClassIds, orderedPriorityRelations.Select(item => (Higher: item.HigherPriorityClassId.Value, Lower: item.LowerPriorityClassId.Value)), "resource priority");
        RejectCycles(criticalityClassIds, orderedCriticalityRelations.Select(item => (Higher: item.HigherCriticalityClassId.Value, Lower: item.LowerCriticalityClassId.Value)), "technical criticality");

        var allocatedApplications = AllocationSnapshot.Allocations.Select(item => item.ApplicationId.Value).ToHashSet(StringComparer.Ordinal);
        var knownResources = AllocationSnapshot.ResourceTruth.Resources.Select(item => item.ResourceClassId.Value).ToHashSet(StringComparer.Ordinal);
        foreach (var binding in orderedApplicationBindings)
        {
            ValidateEvidenceAndLifetime(binding.Evidence, binding.Lifetime, observedAt, "Application priority binding");
            if (!allocatedApplications.Contains(binding.ApplicationId.Value)) throw new ArgumentException($"Application priority binding references Application '{binding.ApplicationId}' without a current admitted allocation.", nameof(applicationBindings));
            if (!priorityClassIds.Contains(binding.PriorityClassId.Value)) throw new ArgumentException($"Application priority binding references unknown priority class '{binding.PriorityClassId}'.", nameof(applicationBindings));
        }
        foreach (var binding in orderedTechnicalBindings)
        {
            ValidateEvidenceAndLifetime(binding.Evidence, binding.Lifetime, observedAt, "technical criticality binding");
            if (!knownResources.Contains(binding.ResourceClassId.Value)) throw new ArgumentException($"Technical criticality binding references unknown resource class '{binding.ResourceClassId}'.", nameof(technicalBindings));
            if (!criticalityClassIds.Contains(binding.CriticalityClassId.Value)) throw new ArgumentException($"Technical criticality binding references unknown criticality class '{binding.CriticalityClassId}'.", nameof(technicalBindings));
        }

        _priorityClasses = Array.AsReadOnly(orderedPriorityClasses); _criticalityClasses = Array.AsReadOnly(orderedCriticalityClasses); _priorityRelations = Array.AsReadOnly(orderedPriorityRelations); _criticalityRelations = Array.AsReadOnly(orderedCriticalityRelations); _applicationBindings = Array.AsReadOnly(orderedApplicationBindings); _technicalBindings = Array.AsReadOnly(orderedTechnicalBindings);
        IdentitySha256 = ComputeIdentity();
    }

    public ApplicationResourceAllocationSnapshot AllocationSnapshot { get; }
    public ResourceEpochId EpochId => AllocationSnapshot.EpochId;
    public DateTimeOffset ObservedAt { get; }
    public string PriorityPolicyVersion { get; }
    public ResourceEffectiveLifetime PriorityPolicyLifetime { get; }
    public ResourceEvidenceReference PriorityPolicyEvidence { get; }
    public string CriticalityPolicyVersion { get; }
    public ResourceEffectiveLifetime CriticalityPolicyLifetime { get; }
    public ResourceEvidenceReference CriticalityPolicyEvidence { get; }
    public IReadOnlyList<ResourcePriorityClassDefinition> PriorityClasses => _priorityClasses;
    public IReadOnlyList<TechnicalCriticalityClassDefinition> CriticalityClasses => _criticalityClasses;
    public IReadOnlyList<ResourcePriorityClassRelation> PriorityRelations => _priorityRelations;
    public IReadOnlyList<TechnicalCriticalityClassRelation> CriticalityRelations => _criticalityRelations;
    public IReadOnlyList<ApplicationResourcePriorityBinding> ApplicationBindings => _applicationBindings;
    public IReadOnlyList<TechnicalCriticalityBinding> TechnicalBindings => _technicalBindings;
    public string IdentitySha256 { get; }

    public ResourcePriorityClassDefinition GetRequiredPriorityClass(ResourcePriorityClassId classId) { ArgumentNullException.ThrowIfNull(classId); return _priorityClasses.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.ClassId.Value, classId.Value)) ?? throw new KeyNotFoundException($"Unknown resource priority class '{classId}'."); }
    public TechnicalCriticalityClassDefinition GetRequiredCriticalityClass(TechnicalCriticalityClassId classId) { ArgumentNullException.ThrowIfNull(classId); return _criticalityClasses.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.ClassId.Value, classId.Value)) ?? throw new KeyNotFoundException($"Unknown technical criticality class '{classId}'."); }

    public bool Outranks(ResourcePriorityClassId higherPriorityClassId, ResourcePriorityClassId lowerPriorityClassId)
    {
        ArgumentNullException.ThrowIfNull(higherPriorityClassId); ArgumentNullException.ThrowIfNull(lowerPriorityClassId); _ = GetRequiredPriorityClass(higherPriorityClassId); _ = GetRequiredPriorityClass(lowerPriorityClassId);
        if (StringComparer.Ordinal.Equals(higherPriorityClassId.Value, lowerPriorityClassId.Value)) return false;
        return HasPath(higherPriorityClassId.Value, lowerPriorityClassId.Value, _priorityRelations.Select(item => (Higher: item.HigherPriorityClassId.Value, Lower: item.LowerPriorityClassId.Value)));
    }

    public bool IsMoreCritical(TechnicalCriticalityClassId higherCriticalityClassId, TechnicalCriticalityClassId lowerCriticalityClassId)
    {
        ArgumentNullException.ThrowIfNull(higherCriticalityClassId); ArgumentNullException.ThrowIfNull(lowerCriticalityClassId); _ = GetRequiredCriticalityClass(higherCriticalityClassId); _ = GetRequiredCriticalityClass(lowerCriticalityClassId);
        if (StringComparer.Ordinal.Equals(higherCriticalityClassId.Value, lowerCriticalityClassId.Value)) return false;
        return HasPath(higherCriticalityClassId.Value, lowerCriticalityClassId.Value, _criticalityRelations.Select(item => (Higher: item.HigherCriticalityClassId.Value, Lower: item.LowerCriticalityClassId.Value)));
    }

    public ApplicationResourcePriorityView GetApplicationView(ApplicationPrincipalId applicationId)
    {
        ArgumentNullException.ThrowIfNull(applicationId); var binding = _applicationBindings.SingleOrDefault(item => StringComparer.Ordinal.Equals(item.ApplicationId.Value, applicationId.Value)); var priorityClass = binding is null ? null : GetRequiredPriorityClass(binding.PriorityClassId); return new ApplicationResourcePriorityView(applicationId, binding, priorityClass, IdentitySha256);
    }

    private void ValidateEvidenceAndLifetime(ResourceEvidenceReference evidence, ResourceEffectiveLifetime lifetime, DateTimeOffset observedAt, string subject)
    {
        if (!StringComparer.Ordinal.Equals(evidence.EpochId.Value, EpochId.Value)) throw new ArgumentException($"{subject} evidence epoch does not match the current resource epoch.");
        if (evidence.ObservedAt > observedAt) throw new ArgumentException($"{subject} evidence cannot be future-dated relative to the snapshot observation.");
        if (lifetime.EffectiveFrom > observedAt) throw new ArgumentException($"{subject} cannot be future-effective in the current snapshot.");
        if (lifetime.EffectiveUntil.HasValue && lifetime.EffectiveUntil.Value < observedAt) throw new ArgumentException($"Expired {subject} cannot appear as current policy truth.");
    }

    private static void RejectDuplicateIds(IEnumerable<string> values, string subject, string parameterName) { var duplicate = values.GroupBy(value => value, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1); if (duplicate is not null) throw new ArgumentException($"Duplicate {subject} '{duplicate.Key}' is not allowed.", parameterName); }
    private static void RejectCycles(IEnumerable<string> nodes, IEnumerable<(string Higher, string Lower)> relations, string subject) { var edges = relations.GroupBy(item => item.Higher, StringComparer.Ordinal).ToDictionary(group => group.Key, group => group.Select(item => item.Lower).ToArray(), StringComparer.Ordinal); foreach (var node in nodes) if (HasPath(node, node, edges, true)) throw new ArgumentException($"Cyclic {subject} policy is not allowed."); }
    private static bool HasPath(string start, string target, IEnumerable<(string Higher, string Lower)> relations) { var edges = relations.GroupBy(item => item.Higher, StringComparer.Ordinal).ToDictionary(group => group.Key, group => group.Select(item => item.Lower).ToArray(), StringComparer.Ordinal); return HasPath(start, target, edges, false); }
    private static bool HasPath(string start, string target, IReadOnlyDictionary<string, string[]> edges, bool requireAtLeastOneEdge)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal); var stack = new Stack<(string Node, bool Traversed)>(); stack.Push((start, false));
        while (stack.Count > 0) { var current = stack.Pop(); if (StringComparer.Ordinal.Equals(current.Node, target) && (!requireAtLeastOneEdge || current.Traversed)) return true; if (!visited.Add(current.Node)) continue; if (!edges.TryGetValue(current.Node, out var nextNodes)) continue; foreach (var next in nextNodes) stack.Push((next, true)); }
        return false;
    }

    private string ComputeIdentity()
    {
        var fields = new List<CanonicalIdentityField> { new("allocation_snapshot_identity", AllocationSnapshot.IdentitySha256), CanonicalResourceIdentity.IdentifierField("epoch", EpochId), new("observed_at", ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("priority_policy_version", PriorityPolicyVersion), CanonicalResourceIdentity.LifetimeStartField("priority_policy_effective_from", PriorityPolicyLifetime), new("priority_policy_effective_until", PriorityPolicyLifetime.EffectiveUntil?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("priority_policy_open_ended", PriorityPolicyLifetime.ExplicitlyOpenEnded ? "true" : "false"), CanonicalResourceIdentity.IdentifierField("priority_policy_evidence_id", PriorityPolicyEvidence.EvidenceId), CanonicalResourceIdentity.IdentifierField("priority_policy_evidence_scope", PriorityPolicyEvidence.ScopeId), new("priority_policy_evidence_observed_at", PriorityPolicyEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), CanonicalResourceIdentity.IdentifierField("priority_policy_evidence_epoch", PriorityPolicyEvidence.EpochId), new("criticality_policy_version", CriticalityPolicyVersion), CanonicalResourceIdentity.LifetimeStartField("criticality_policy_effective_from", CriticalityPolicyLifetime), new("criticality_policy_effective_until", CriticalityPolicyLifetime.EffectiveUntil?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), new("criticality_policy_open_ended", CriticalityPolicyLifetime.ExplicitlyOpenEnded ? "true" : "false"), CanonicalResourceIdentity.IdentifierField("criticality_policy_evidence_id", CriticalityPolicyEvidence.EvidenceId), CanonicalResourceIdentity.IdentifierField("criticality_policy_evidence_scope", CriticalityPolicyEvidence.ScopeId), new("criticality_policy_evidence_observed_at", CriticalityPolicyEvidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)), CanonicalResourceIdentity.IdentifierField("criticality_policy_evidence_epoch", CriticalityPolicyEvidence.EpochId), new("priority_class_count", _priorityClasses.Count.ToString(CultureInfo.InvariantCulture)), new("criticality_class_count", _criticalityClasses.Count.ToString(CultureInfo.InvariantCulture)), new("priority_relation_count", _priorityRelations.Count.ToString(CultureInfo.InvariantCulture)), new("criticality_relation_count", _criticalityRelations.Count.ToString(CultureInfo.InvariantCulture)), new("application_binding_count", _applicationBindings.Count.ToString(CultureInfo.InvariantCulture)), new("technical_binding_count", _technicalBindings.Count.ToString(CultureInfo.InvariantCulture)) };
        AddPriorityClasses(fields); AddCriticalityClasses(fields); AddPriorityRelations(fields); AddCriticalityRelations(fields); AddApplicationBindings(fields); AddTechnicalBindings(fields); return CanonicalResourceIdentity.ComputeSha256(fields);
    }
    private void AddPriorityClasses(List<CanonicalIdentityField> fields) { for (var index = 0; index < _priorityClasses.Count; index++) { var item = _priorityClasses[index]; var prefix = $"priority_class_{index:D4}_"; fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "id", item.ClassId)); AddLifetimeAndEvidence(fields, prefix, item.Lifetime, item.Evidence); } }
    private void AddCriticalityClasses(List<CanonicalIdentityField> fields) { for (var index = 0; index < _criticalityClasses.Count; index++) { var item = _criticalityClasses[index]; var prefix = $"criticality_class_{index:D4}_"; fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "id", item.ClassId)); AddLifetimeAndEvidence(fields, prefix, item.Lifetime, item.Evidence); } }
    private void AddPriorityRelations(List<CanonicalIdentityField> fields) { for (var index = 0; index < _priorityRelations.Count; index++) { var item = _priorityRelations[index]; var prefix = $"priority_relation_{index:D4}_"; fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "higher", item.HigherPriorityClassId)); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "lower", item.LowerPriorityClassId)); AddLifetimeAndEvidence(fields, prefix, item.Lifetime, item.Evidence); } }
    private void AddCriticalityRelations(List<CanonicalIdentityField> fields) { for (var index = 0; index < _criticalityRelations.Count; index++) { var item = _criticalityRelations[index]; var prefix = $"criticality_relation_{index:D4}_"; fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "higher", item.HigherCriticalityClassId)); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "lower", item.LowerCriticalityClassId)); AddLifetimeAndEvidence(fields, prefix, item.Lifetime, item.Evidence); } }
    private void AddApplicationBindings(List<CanonicalIdentityField> fields) { for (var index = 0; index < _applicationBindings.Count; index++) { var item = _applicationBindings[index]; var prefix = $"application_binding_{index:D4}_"; fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "application", item.ApplicationId)); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "priority_class", item.PriorityClassId)); AddLifetimeAndEvidence(fields, prefix, item.Lifetime, item.Evidence); } }
    private void AddTechnicalBindings(List<CanonicalIdentityField> fields) { for (var index = 0; index < _technicalBindings.Count; index++) { var item = _technicalBindings[index]; var prefix = $"technical_binding_{index:D4}_"; fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "scope", item.TechnicalScopeId)); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "resource_class", item.ResourceClassId)); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "criticality_class", item.CriticalityClassId)); AddLifetimeAndEvidence(fields, prefix, item.Lifetime, item.Evidence); } }
    private static void AddLifetimeAndEvidence(List<CanonicalIdentityField> fields, string prefix, ResourceEffectiveLifetime lifetime, ResourceEvidenceReference evidence) { fields.Add(CanonicalResourceIdentity.LifetimeStartField(prefix + "effective_from", lifetime)); fields.Add(new(prefix + "effective_until", lifetime.EffectiveUntil?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))); fields.Add(new(prefix + "explicitly_open_ended", lifetime.ExplicitlyOpenEnded ? "true" : "false")); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_id", evidence.EvidenceId)); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_scope", evidence.ScopeId)); fields.Add(new(prefix + "evidence_observed_at", evidence.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))); fields.Add(CanonicalResourceIdentity.IdentifierField(prefix + "evidence_epoch", evidence.EpochId)); }
}
