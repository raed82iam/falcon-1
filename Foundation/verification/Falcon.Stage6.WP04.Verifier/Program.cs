using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP04.Verifier;

internal static class Program
{
    private static int _failures;
    private static int _tests;
    private static readonly DateTimeOffset T0 = new(2026, 8, 9, 0, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");

    private static int Main()
    {
        Run("positive_policy_snapshot", PositivePolicySnapshot);
        Run("zero_application_validity", ZeroApplicationValidity);
        Run("direct_priority_relation", DirectPriorityRelation);
        Run("transitive_priority_relation", TransitivePriorityRelation);
        Run("same_priority_class_does_not_outrank_itself", SamePriorityClassDoesNotOutrankItself);
        Run("direct_criticality_relation", DirectCriticalityRelation);
        Run("transitive_criticality_relation", TransitiveCriticalityRelation);
        Run("same_criticality_class_is_not_more_critical", SameCriticalityClassIsNotMoreCritical);
        Run("duplicate_priority_class_rejected", DuplicatePriorityClassRejected);
        Run("duplicate_criticality_class_rejected", DuplicateCriticalityClassRejected);
        Run("duplicate_priority_relation_rejected", DuplicatePriorityRelationRejected);
        Run("duplicate_criticality_relation_rejected", DuplicateCriticalityRelationRejected);
        Run("self_priority_relation_rejected", SelfPriorityRelationRejected);
        Run("self_criticality_relation_rejected", SelfCriticalityRelationRejected);
        Run("priority_cycle_rejected", PriorityCycleRejected);
        Run("criticality_cycle_rejected", CriticalityCycleRejected);
        Run("unknown_priority_relation_endpoint_rejected", UnknownPriorityRelationEndpointRejected);
        Run("unknown_criticality_relation_endpoint_rejected", UnknownCriticalityRelationEndpointRejected);
        Run("duplicate_application_binding_rejected", DuplicateApplicationBindingRejected);
        Run("duplicate_technical_binding_rejected", DuplicateTechnicalBindingRejected);
        Run("unknown_application_binding_rejected", UnknownApplicationBindingRejected);
        Run("unknown_priority_class_binding_rejected", UnknownPriorityClassBindingRejected);
        Run("unknown_resource_technical_binding_rejected", UnknownResourceTechnicalBindingRejected);
        Run("unknown_criticality_class_binding_rejected", UnknownCriticalityClassBindingRejected);
        Run("wrong_epoch_priority_policy_rejected", WrongEpochPriorityPolicyRejected);
        Run("wrong_epoch_criticality_policy_rejected", WrongEpochCriticalityPolicyRejected);
        Run("future_policy_evidence_rejected", FuturePolicyEvidenceRejected);
        Run("future_effective_policy_rejected", FutureEffectivePolicyRejected);
        Run("expired_policy_rejected", ExpiredPolicyRejected);
        Run("snapshot_predates_allocation_rejected", SnapshotPredatesAllocationRejected);
        Run("unavailable_policy_truth_fails_closed", UnavailablePolicyTruthFailsClosed);
        Run("blank_policy_version_rejected", BlankPolicyVersionRejected);
        Run("ordering_is_deterministic", OrderingIsDeterministic);
        Run("policy_version_changes_identity", PolicyVersionChangesIdentity);
        Run("priority_relation_changes_identity", PriorityRelationChangesIdentity);
        Run("criticality_relation_changes_identity", CriticalityRelationChangesIdentity);
        Run("allocation_snapshot_changes_identity", AllocationSnapshotChangesIdentity);
        Run("application_view_is_scoped", ApplicationViewIsScoped);
        Run("unknown_application_view_has_no_binding", UnknownApplicationViewHasNoBinding);
        Run("priority_and_criticality_types_are_distinct", PriorityAndCriticalityTypesAreDistinct);
        Run("application_binding_has_no_criticality_field", ApplicationBindingHasNoCriticalityField);
        Run("technical_binding_has_no_application_priority_field", TechnicalBindingHasNoApplicationPriorityField);
        Run("numeric_precedence_not_in_public_surface", NumericPrecedenceNotInPublicSurface);
        Run("foundation_protected_floor_not_application_ranking_field", FoundationProtectedFloorNotApplicationRankingField);
        Run("production_surface_has_no_trading_terms", ProductionSurfaceHasNoTradingTerms);
        Run("production_surface_has_no_wp05_runtime_terms", ProductionSurfaceHasNoWp05RuntimeTerms);
        Run("allocation_quantities_remain_unmodified", AllocationQuantitiesRemainUnmodified);
        Run("identity_is_uppercase_sha256", IdentityIsUppercaseSha256);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-04 VERIFIER: {_tests - _failures}/{_tests} PASS");
        Console.WriteLine($"Failures: {_failures}");
        return _failures == 0 ? 0 : 1;
    }

    private static FoundationResourceTruthSnapshot Truth()
        => new(Epoch, T0, new[] { new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", "foundation-resource-truth")) }, true);

    private static ApplicationResourceAllocation Allocation(string app, string grant, decimal amount = 10m)
        => new(new ResourceGrantId(grant), new ApplicationPrincipalId(app), Cpu, Q(amount), Q(amount + 10), Q(amount + 20), Lifetime(), Evidence("allocation-" + app, "application-allocation"));

    private static ApplicationResourceAllocationSnapshot Allocations(params ApplicationResourceAllocation[] allocations)
        => new(Truth(), T0, allocations, true);

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime(DateTimeOffset? start = null, DateTimeOffset? end = null)
        => end.HasValue ? new ResourceEffectiveLifetime(start ?? T0, end, false) : new ResourceEffectiveLifetime(start ?? T0, null, true);
    private static ResourceEvidenceReference Evidence(string id, string scope, ResourceEpochId? epoch = null, DateTimeOffset? observedAt = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId(scope), observedAt ?? T0, epoch ?? Epoch);

    private static ResourcePriorityClassDefinition PClass(string id, ResourceEpochId? epoch = null)
        => new(new ResourcePriorityClassId(id), Lifetime(), Evidence("pc-" + id, "foundation-priority-policy", epoch));
    private static TechnicalCriticalityClassDefinition CClass(string id, ResourceEpochId? epoch = null)
        => new(new TechnicalCriticalityClassId(id), Lifetime(), Evidence("cc-" + id, "foundation-criticality-policy", epoch));
    private static ResourcePriorityClassRelation PRel(string higher, string lower, ResourceEpochId? epoch = null)
        => new(new ResourcePriorityClassId(higher), new ResourcePriorityClassId(lower), Lifetime(), Evidence("pr-" + higher + "-" + lower, "foundation-priority-policy", epoch));
    private static TechnicalCriticalityClassRelation CRel(string higher, string lower, ResourceEpochId? epoch = null)
        => new(new TechnicalCriticalityClassId(higher), new TechnicalCriticalityClassId(lower), Lifetime(), Evidence("cr-" + higher + "-" + lower, "foundation-criticality-policy", epoch));
    private static ApplicationResourcePriorityBinding AppBinding(string app, string priority)
        => new(new ApplicationPrincipalId(app), new ResourcePriorityClassId(priority), Lifetime(), Evidence("ab-" + app, "application-priority-binding"));
    private static TechnicalCriticalityBinding TechBinding(string scope, string criticality, string resource = "cpu")
        => new(new ResourceScopeId(scope), new ResourceClassId(resource), new TechnicalCriticalityClassId(criticality), Lifetime(), Evidence("tb-" + scope, "technical-criticality-binding"));

    private static ResourcePriorityGovernanceSnapshot Snapshot(
        ApplicationResourceAllocationSnapshot? allocations = null,
        string priorityVersion = "priority-policy-v1",
        ResourceEffectiveLifetime? priorityLifetime = null,
        ResourceEvidenceReference? priorityEvidence = null,
        string criticalityVersion = "criticality-policy-v1",
        ResourceEffectiveLifetime? criticalityLifetime = null,
        ResourceEvidenceReference? criticalityEvidence = null,
        IEnumerable<ResourcePriorityClassDefinition>? priorityClasses = null,
        IEnumerable<TechnicalCriticalityClassDefinition>? criticalityClasses = null,
        IEnumerable<ResourcePriorityClassRelation>? priorityRelations = null,
        IEnumerable<TechnicalCriticalityClassRelation>? criticalityRelations = null,
        IEnumerable<ApplicationResourcePriorityBinding>? appBindings = null,
        IEnumerable<TechnicalCriticalityBinding>? technicalBindings = null,
        DateTimeOffset? observedAt = null,
        bool available = true)
        => new(
            allocations ?? Allocations(Allocation("app-a", "grant-a")), observedAt ?? T0,
            priorityVersion, priorityLifetime ?? Lifetime(), priorityEvidence ?? Evidence("priority-policy", "foundation-priority-policy"),
            criticalityVersion, criticalityLifetime ?? Lifetime(), criticalityEvidence ?? Evidence("criticality-policy", "foundation-criticality-policy"),
            priorityClasses ?? new[] { PClass("p-high"), PClass("p-low") },
            criticalityClasses ?? new[] { CClass("c-high"), CClass("c-low") },
            priorityRelations ?? new[] { PRel("p-high", "p-low") },
            criticalityRelations ?? new[] { CRel("c-high", "c-low") },
            appBindings ?? new[] { AppBinding("app-a", "p-high") },
            technicalBindings ?? new[] { TechBinding("scope-a", "c-high") }, available);

    private static void PositivePolicySnapshot() { var s = Snapshot(); Assert(s.Outranks(new("p-high"), new("p-low")), "Expected explicit policy priority relation."); }
    private static void ZeroApplicationValidity() { var s = Snapshot(allocations: Allocations(), priorityClasses: Array.Empty<ResourcePriorityClassDefinition>(), criticalityClasses: Array.Empty<TechnicalCriticalityClassDefinition>(), priorityRelations: Array.Empty<ResourcePriorityClassRelation>(), criticalityRelations: Array.Empty<TechnicalCriticalityClassRelation>(), appBindings: Array.Empty<ApplicationResourcePriorityBinding>(), technicalBindings: Array.Empty<TechnicalCriticalityBinding>()); Assert(s.ApplicationBindings.Count == 0, "Zero-Application Foundation must remain valid."); }
    private static void DirectPriorityRelation() => Assert(Snapshot().Outranks(new("p-high"), new("p-low")), "Direct priority relation must resolve.");
    private static void TransitivePriorityRelation() { var s = Snapshot(priorityClasses: new[] { PClass("p0"), PClass("p1"), PClass("p2") }, priorityRelations: new[] { PRel("p0", "p1"), PRel("p1", "p2") }, appBindings: new[] { AppBinding("app-a", "p0") }); Assert(s.Outranks(new("p0"), new("p2")), "Transitive priority relation must resolve."); }
    private static void SamePriorityClassDoesNotOutrankItself() => Assert(!Snapshot().Outranks(new("p-high"), new("p-high")), "Same priority class must not outrank itself.");
    private static void DirectCriticalityRelation() => Assert(Snapshot().IsMoreCritical(new("c-high"), new("c-low")), "Direct criticality relation must resolve.");
    private static void TransitiveCriticalityRelation() { var s = Snapshot(criticalityClasses: new[] { CClass("c0"), CClass("c1"), CClass("c2") }, criticalityRelations: new[] { CRel("c0", "c1"), CRel("c1", "c2") }, technicalBindings: new[] { TechBinding("scope-a", "c0") }); Assert(s.IsMoreCritical(new("c0"), new("c2")), "Transitive criticality relation must resolve."); }
    private static void SameCriticalityClassIsNotMoreCritical() => Assert(!Snapshot().IsMoreCritical(new("c-high"), new("c-high")), "Same criticality class must not outrank itself.");
    private static void DuplicatePriorityClassRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityClasses: new[] { PClass("p"), PClass("p") }, priorityRelations: Array.Empty<ResourcePriorityClassRelation>(), appBindings: new[] { AppBinding("app-a", "p") }));
    private static void DuplicateCriticalityClassRejected() => ExpectThrows<ArgumentException>(() => Snapshot(criticalityClasses: new[] { CClass("c"), CClass("c") }, criticalityRelations: Array.Empty<TechnicalCriticalityClassRelation>(), technicalBindings: new[] { TechBinding("scope-a", "c") }));
    private static void DuplicatePriorityRelationRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityRelations: new[] { PRel("p-high", "p-low"), PRel("p-high", "p-low") }));
    private static void DuplicateCriticalityRelationRejected() => ExpectThrows<ArgumentException>(() => Snapshot(criticalityRelations: new[] { CRel("c-high", "c-low"), CRel("c-high", "c-low") }));
    private static void SelfPriorityRelationRejected() => ExpectThrows<ArgumentException>(() => PRel("p-high", "p-high"));
    private static void SelfCriticalityRelationRejected() => ExpectThrows<ArgumentException>(() => CRel("c-high", "c-high"));
    private static void PriorityCycleRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityRelations: new[] { PRel("p-high", "p-low"), PRel("p-low", "p-high") }));
    private static void CriticalityCycleRejected() => ExpectThrows<ArgumentException>(() => Snapshot(criticalityRelations: new[] { CRel("c-high", "c-low"), CRel("c-low", "c-high") }));
    private static void UnknownPriorityRelationEndpointRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityRelations: new[] { PRel("p-high", "missing") }));
    private static void UnknownCriticalityRelationEndpointRejected() => ExpectThrows<ArgumentException>(() => Snapshot(criticalityRelations: new[] { CRel("c-high", "missing") }));
    private static void DuplicateApplicationBindingRejected() => ExpectThrows<ArgumentException>(() => Snapshot(appBindings: new[] { AppBinding("app-a", "p-high"), AppBinding("app-a", "p-low") }));
    private static void DuplicateTechnicalBindingRejected() => ExpectThrows<ArgumentException>(() => Snapshot(technicalBindings: new[] { TechBinding("scope-a", "c-high"), TechBinding("scope-a", "c-low") }));
    private static void UnknownApplicationBindingRejected() => ExpectThrows<ArgumentException>(() => Snapshot(appBindings: new[] { AppBinding("app-x", "p-high") }));
    private static void UnknownPriorityClassBindingRejected() => ExpectThrows<ArgumentException>(() => Snapshot(appBindings: new[] { AppBinding("app-a", "missing") }));
    private static void UnknownResourceTechnicalBindingRejected() => ExpectThrows<ArgumentException>(() => Snapshot(technicalBindings: new[] { TechBinding("scope-a", "c-high", "gpu") }));
    private static void UnknownCriticalityClassBindingRejected() => ExpectThrows<ArgumentException>(() => Snapshot(technicalBindings: new[] { TechBinding("scope-a", "missing") }));
    private static void WrongEpochPriorityPolicyRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityEvidence: Evidence("priority-policy", "foundation-priority-policy", new("epoch-002"))));
    private static void WrongEpochCriticalityPolicyRejected() => ExpectThrows<ArgumentException>(() => Snapshot(criticalityEvidence: Evidence("criticality-policy", "foundation-criticality-policy", new("epoch-002"))));
    private static void FuturePolicyEvidenceRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityEvidence: Evidence("priority-policy", "foundation-priority-policy", observedAt: T0.AddSeconds(1))));
    private static void FutureEffectivePolicyRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityLifetime: Lifetime(T0.AddSeconds(1))));
    private static void ExpiredPolicyRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityLifetime: Lifetime(T0.AddMinutes(-5), T0.AddSeconds(-1))));
    private static void SnapshotPredatesAllocationRejected() => ExpectThrows<ArgumentException>(() => Snapshot(observedAt: T0.AddSeconds(-1)));
    private static void UnavailablePolicyTruthFailsClosed() => ExpectThrows<InvalidOperationException>(() => Snapshot(available: false));
    private static void BlankPolicyVersionRejected() => ExpectThrows<ArgumentException>(() => Snapshot(priorityVersion: " "));
    private static void OrderingIsDeterministic() { var allocations = Allocations(Allocation("app-a", "grant-a"), Allocation("app-b", "grant-b")); var first = Snapshot(allocations, priorityClasses: new[] { PClass("p-low"), PClass("p-high") }, priorityRelations: new[] { PRel("p-high", "p-low") }, appBindings: new[] { AppBinding("app-b", "p-low"), AppBinding("app-a", "p-high") }); var second = Snapshot(allocations, priorityClasses: new[] { PClass("p-high"), PClass("p-low") }, priorityRelations: new[] { PRel("p-high", "p-low") }, appBindings: new[] { AppBinding("app-a", "p-high"), AppBinding("app-b", "p-low") }); Assert(first.IdentitySha256 == second.IdentitySha256, "Input ordering must not alter identity."); }
    private static void PolicyVersionChangesIdentity() => Assert(Snapshot(priorityVersion: "priority-policy-v1").IdentitySha256 != Snapshot(priorityVersion: "priority-policy-v2").IdentitySha256, "Policy version must be identity material.");
    private static void PriorityRelationChangesIdentity() => Assert(Snapshot(priorityRelations: new[] { PRel("p-high", "p-low") }).IdentitySha256 != Snapshot(priorityRelations: Array.Empty<ResourcePriorityClassRelation>()).IdentitySha256, "Priority policy relation must be identity material.");
    private static void CriticalityRelationChangesIdentity() => Assert(Snapshot(criticalityRelations: new[] { CRel("c-high", "c-low") }).IdentitySha256 != Snapshot(criticalityRelations: Array.Empty<TechnicalCriticalityClassRelation>()).IdentitySha256, "Criticality policy relation must be identity material.");
    private static void AllocationSnapshotChangesIdentity() => Assert(Snapshot(allocations: Allocations(Allocation("app-a", "grant-a", 10))).IdentitySha256 != Snapshot(allocations: Allocations(Allocation("app-a", "grant-a", 11))).IdentitySha256, "WP-03 allocation snapshot must be identity material.");
    private static void ApplicationViewIsScoped() { var allocations = Allocations(Allocation("app-a", "grant-a"), Allocation("app-b", "grant-b")); var s = Snapshot(allocations, appBindings: new[] { AppBinding("app-a", "p-high"), AppBinding("app-b", "p-low") }); var view = s.GetApplicationView(new("app-a")); Assert(view.Binding?.ApplicationId.Value == "app-a" && view.PriorityClass?.ClassId.Value == "p-high", "Application view must expose only its own binding."); }
    private static void UnknownApplicationViewHasNoBinding() { var view = Snapshot().GetApplicationView(new("app-x")); Assert(view.Binding is null && view.PriorityClass is null, "Unknown Application view must not leak another Application binding."); }
    private static void PriorityAndCriticalityTypesAreDistinct() => Assert(typeof(ResourcePriorityClassId) != typeof(TechnicalCriticalityClassId), "Priority and criticality identifiers must remain distinct.");
    private static void ApplicationBindingHasNoCriticalityField() => Assert(!typeof(ApplicationResourcePriorityBinding).GetProperties().Any(p => p.PropertyType == typeof(TechnicalCriticalityClassId)), "Application priority binding must not mint technical criticality.");
    private static void TechnicalBindingHasNoApplicationPriorityField() { var p = typeof(TechnicalCriticalityBinding).GetProperties(); Assert(!p.Any(x => x.PropertyType == typeof(ResourcePriorityClassId)) && !p.Any(x => x.PropertyType == typeof(ApplicationPrincipalId)), "Technical criticality binding must not become an Application-priority shortcut."); }
    private static void NumericPrecedenceNotInPublicSurface() { var names = PublicSurfaceNames(typeof(ResourcePriorityGovernanceSnapshot).Assembly); Assert(!names.Any(x => x.Contains("Precedence", StringComparison.OrdinalIgnoreCase)), "WP-04 must not invent numeric precedence semantics."); }
    private static void FoundationProtectedFloorNotApplicationRankingField() { var names = PublicSurfaceNames(typeof(ResourcePriorityGovernanceSnapshot).Assembly); Assert(!names.Any(x => x.Contains("FoundationProtectedPriorityFloor", StringComparison.OrdinalIgnoreCase)), "Foundation protected capacity must remain outside Application ranking."); }
    private static void ProductionSurfaceHasNoTradingTerms() { var names = PublicSurfaceNames(typeof(ResourcePriorityGovernanceSnapshot).Assembly); var forbidden = new[] { "Trading", "TARC", "Strategy", "Broker", "Instrument", "Order" }; Assert(!names.Any(name => forbidden.Any(term => name.Contains(term, StringComparison.OrdinalIgnoreCase))), "WP-04 production surface must remain Application-neutral."); }
    private static void ProductionSurfaceHasNoWp05RuntimeTerms()
    {
        var wp04OwnedTypes = new[]
        {
            typeof(ResourcePriorityGovernanceSnapshot),
            typeof(ResourcePriorityClassDefinition),
            typeof(TechnicalCriticalityClassDefinition),
            typeof(ResourcePriorityClassRelation),
            typeof(TechnicalCriticalityClassRelation),
            typeof(ApplicationResourcePriorityBinding),
            typeof(TechnicalCriticalityBinding),
            typeof(ApplicationResourcePriorityView)
        };
        var forbidden = new[] { "Preempt", "Enforcement", "LoadShedding", "Rebalance", "Redistribution", "Reclamation", "ResourceRequestProcessor" };
        var names = wp04OwnedTypes.SelectMany(type =>
            new[] { type.FullName ?? type.Name }
                .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(property => property.Name))
                .Concat(type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(method => method.Name)));
        Assert(!names.Any(name => forbidden.Any(term => name.Contains(term, StringComparison.OrdinalIgnoreCase))), "WP-05+ runtime concepts must not leak into WP-04-owned public surface.");
    }
    private static void AllocationQuantitiesRemainUnmodified() { var a = Allocations(Allocation("app-a", "grant-a", 10)); var before = a.GetRequiredAllocation(new("app-a"), Cpu); _ = Snapshot(allocations: a); var after = a.GetRequiredAllocation(new("app-a"), Cpu); Assert(before.Allocation.Amount == after.Allocation.Amount && before.Quota.Amount == after.Quota.Amount && before.Ceiling.Amount == after.Ceiling.Amount, "WP-04 must not mutate WP-03 allocation quantities."); }
    private static void IdentityIsUppercaseSha256() { var id = Snapshot().IdentitySha256; Assert(id.Length == 64 && id.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')), "Identity must be uppercase SHA-256 hex."); }

    private static IEnumerable<string> PublicSurfaceNames(Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes().Where(t => t.Namespace == "Foundation.State.ResourceGovernance"))
        {
            yield return type.FullName ?? type.Name;
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) yield return property.Name;
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)) yield return method.Name;
        }
    }

    private static void Run(string name, Action test) { _tests++; try { test(); Console.WriteLine($"PASS {name}"); } catch (Exception ex) { _failures++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); } }
    private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void ExpectThrows<T>(Action action) where T : Exception { try { action(); } catch (T) { return; } throw new InvalidOperationException($"Expected {typeof(T).Name}."); }
}