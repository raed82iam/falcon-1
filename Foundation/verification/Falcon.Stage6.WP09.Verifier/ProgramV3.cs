using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP09.Verifier;

internal static class ProgramV3
{
    private static int _passed;
    private static int _failed;
    private static readonly DateTimeOffset T0 = new(2026, 8, 10, 12, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");
    private static readonly ApplicationPrincipalId AppA = new("app-a");
    private static readonly ApplicationPrincipalId AppB = new("app-b");
    private static readonly ResourceGrantId GrantA = new("grant-a");
    private static readonly ResourceGrantId GrantB = new("grant-b");

    private static int Main()
    {
        Run("zero_application_valid", ZeroApplicationValid);
        Run("full_current_chain_coherent", FullCurrentChainCoherent);
        Run("missing_authoritative_lineage_unavailable", MissingAuthoritativeLineageUnavailable);
        Run("one_transition_lagging", OneTransitionLagging);
        Run("multi_transition_gap_free", MultiTransitionGapFree);
        Run("missing_intermediate_rejected", MissingIntermediateRejected);
        Run("reordered_chain_rejected", ReorderedChainRejected);
        Run("duplicate_transition_rejected", DuplicateTransitionRejected);
        Run("decision_wrong_application_contradictory", DecisionWrongApplicationContradictory);
        Run("delegated_effective_lineage_current", DelegatedEffectiveLineageCurrent);
        Run("borrowed_empty_lineage_unavailable", BorrowedEmptyLineageUnavailable);
        Run("coordinator_fence_conflict_contradictory", CoordinatorFenceConflictContradictory);
        Run("require_current_fails_closed", RequireCurrentFailsClosed);
        Run("application_view_exact", ApplicationViewExact);
        Run("signal_projection_mismatch_contradictory", SignalProjectionMismatchContradictory);
        Run("identity_changes_with_lineage", IdentityChangesWithLineage);
        Run("no_latest_selector_surface", NoLatestSelectorSurface);
        Run("no_business_or_authority_surface", NoBusinessOrAuthoritySurface);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-09 VERIFIER V3: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime() => new(T0.AddHours(-2), null, true);
    private static ResourceEvidenceReference Evidence(string id, DateTimeOffset? at = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId("scope-" + id), at ?? T0.AddMinutes(-50), Epoch);

    private static FoundationResourceTruthSnapshot Truth()
        => new(Epoch, T0.AddMinutes(-40), new[] { new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", T0.AddMinutes(-41))) }, true);

    private static ApplicationResourceAllocationSnapshot Allocations(decimal appA = 20m, decimal appB = 20m, DateTimeOffset? at = null)
        => new(Truth(), at ?? T0.AddMinutes(-30), new[]
        {
            new ApplicationResourceAllocation(GrantA, AppA, Cpu, Q(appA), Q(Math.Max(appA, 20m)), Q(Math.Max(appA, 30m)), Lifetime(), Evidence("allocation-a")),
            new ApplicationResourceAllocation(GrantB, AppB, Cpu, Q(appB), Q(Math.Max(appB, 20m)), Q(Math.Max(appB, 30m)), Lifetime(), Evidence("allocation-b"))
        }, true);

    private static ResourcePriorityGovernanceSnapshot Priority(ApplicationResourceAllocationSnapshot a)
        => new(a, a.ObservedAt.AddMinutes(5), "priority-v1", Lifetime(), Evidence("priority-policy"), "criticality-v1", Lifetime(), Evidence("criticality-policy"),
            new[] { new ResourcePriorityClassDefinition(new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("pc-high")), new ResourcePriorityClassDefinition(new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("pc-low")) },
            new[] { new TechnicalCriticalityClassDefinition(new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("cc-high")) },
            new[] { new ResourcePriorityClassRelation(new ResourcePriorityClassId("p-high"), new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("pr")) },
            Array.Empty<TechnicalCriticalityClassRelation>(),
            new[] { new ApplicationResourcePriorityBinding(AppA, new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("ab-a")), new ApplicationResourcePriorityBinding(AppB, new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("ab-b")) },
            new[] { new TechnicalCriticalityBinding(new ResourceScopeId("scope-a"), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-a")) }, true);

    private static FoundationResourcePressureSnapshot Pressure(ApplicationResourceAllocationSnapshot a)
        => new(Priority(a), a.ObservedAt.AddMinutes(10),
            new[] { new ResourcePressureTransitionPolicy(Cpu, 6000, 8000, 9500, 500, "pressure-v1", Lifetime(), Evidence("pressure-policy")) },
            new[] { new ResourcePressureObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, Q(10), 1, Evidence("pressure-a")) },
            new[] { new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("reclaim-a")) },
            Array.Empty<ResourceEnforcementObservation>());

    private static ResourceCoordinationEnvelope Envelope(ApplicationResourceAllocationSnapshot a, long fence = 1, string token = "fence-1")
    {
        ResourceCoordinationEnvelopeMember Member(ApplicationPrincipalId app, ResourceGrantId grant)
        {
            var allocation = a.GetRequiredAllocation(app, Cpu);
            var minimum = Math.Min(10m, allocation.Allocation.Amount);
            var movable = Math.Max(0m, allocation.Allocation.Amount - minimum);
            var maxOut = Math.Min(10m, movable);
            var ceilingSpace = Math.Max(0m, allocation.Ceiling.Amount - allocation.Allocation.Amount);
            var maxIn = Math.Min(20m, ceilingSpace);
            return new ResourceCoordinationEnvelopeMember(app, grant, Cpu, Q(minimum), Q(maxOut), Q(maxIn),
                new ResourcePreemptionEligibilityBinding(grant, app, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-" + app.Value)));
        }

        return new ResourceCoordinationEnvelope("envelope-authority", new ResourceScopeId("scope-coordination"), "coordinator-1", "aggregate-resource-coordinator", 1, fence, token, a,
            new[] { Member(AppA, GrantA), Member(AppB, GrantB) }, Evidence("envelope-" + fence), a.ObservedAt.AddMinutes(1), T0.AddHours(1));
    }

    private static EffectiveResourceDistributionSnapshot Effective(ApplicationResourceAllocationSnapshot a, ResourceCoordinationEnvelope? envelope = null, DateTimeOffset? at = null)
    {
        var e = envelope ?? Envelope(a);
        return new EffectiveResourceDistributionSnapshot(a, e, at ?? a.ObservedAt.AddMinutes(12), Array.Empty<BorrowedEffectiveCapacitySegment>());
    }

    private static AdditionalResourceDecisionRecord DirectDecision(ApplicationResourceAllocationSnapshot a, ApplicationPrincipalId app)
    {
        var requester = "requester-" + app.Value;
        var role = "application-resource-requester";
        var auth = new ResourceRequestAuthorityBinding("request-authority-" + app.Value, requester, role, new ResourceScopeId("scope-" + app.Value), new[] { app }, 1, Evidence("request-authority-" + app.Value), T0.AddHours(-1), T0.AddHours(1));
        var request = new AdditionalResourceRequest(new ResourceRequestId("request-" + app.Value), ResourceRequesterKind.DirectApplication, requester, role, auth, app, null, Cpu, Q(5), Q(5), a, null,
            new CorrelationId("corr-request-" + app.Value), new CausationId("cause-request-" + app.Value), Evidence("request-" + app.Value), Evidence("residual-" + app.Value), T0.AddMinutes(-8), T0.AddMinutes(30));
        var policy = new ResourceAdditionalRequestDecisionPolicy(Cpu, Q(10), false, Evidence("decision-policy"), T0.AddHours(-1), T0.AddHours(1));
        var authority = new ResourceRequestDecisionAuthority("decision-authority", Evidence("decision-authority"), T0.AddHours(-1), T0.AddHours(1));
        return new AdditionalResourceRequestDecisionProcessor(new[] { policy }, authority).Evaluate(request, new ResourceDecisionId("decision-" + app.Value), T0.AddMinutes(-7));
    }

    private static (ApplicationResourceAllocationSnapshot Before, AcceptedFoundationAllocationMutation Accepted, AcceptedResourceCapacityTransitionBasis Basis) ReduceA(ApplicationResourceAllocationSnapshot? predecessor = null, decimal target = 10m, string suffix = "1", DateTimeOffset? appliedAt = null)
    {
        var before = predecessor ?? Allocations();
        var authority = new FoundationResourceMutationAuthority("mutation-authority-" + suffix, new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1, Evidence("mutation-authority-" + suffix), T0.AddHours(-1), T0.AddHours(1));
        var intent = new FoundationAllocationMutationIntent("reduce-a-" + suffix, ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(target), Q(Math.Max(target, 20m)), Q(Math.Max(target, 30m)), authority, null, before.IdentitySha256,
            new CorrelationId("corr-reduce-" + suffix), new CausationId("cause-reduce-" + suffix), Evidence("reduce-intent-" + suffix), T0.AddMinutes(-10), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-reduce-a-" + suffix, new[] { ResourceEffectOperation.ForFoundation(intent) });
        var at = appliedAt ?? T0.AddMinutes(-3);
        var accepted = new ResourceMutationProcessor().ApplyFoundationAllocationMutations(before, batch.BatchId, new[] { intent }, new SuccessAdapter(), at, Effective(before, at: at.AddMinutes(-1)));
        return (before, accepted, AcceptedResourceCapacityTransitionBasis.FromFoundationMutation(before, accepted, batch, AppA, Cpu));
    }

    private static (EffectiveResourceDistributionSnapshot Before, AcceptedEffectiveDistributionMutation Accepted, AcceptedResourceCapacityTransitionBasis BasisA, AcceptedResourceCapacityTransitionBasis BasisB) BorrowAtoB()
    {
        var allocation = Allocations();
        var envelope = Envelope(allocation);
        var before = Effective(allocation, envelope, T0.AddMinutes(-6));
        var intent = new EffectiveDistributionMutationIntent("borrow-a-b", EffectiveDistributionOperationKind.Borrow, AppA, GrantA, AppB, Cpu, Q(5), null, envelope,
            envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken, new CorrelationId("corr-borrow"), new CausationId("cause-borrow"), Evidence("borrow-intent"), T0.AddMinutes(-5), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-borrow-a-b", new[] { ResourceEffectOperation.ForEffective(intent) });
        var accepted = new ResourceMutationProcessor().ApplyEffectiveRedistribution(before, batch.BatchId, new[] { intent }, new SuccessAdapter(), T0.AddMinutes(-2));
        return (before, accepted,
            AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(before, accepted, batch, AppA, Cpu),
            AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(before, accepted, batch, AppB, Cpu));
    }

    private static AggregateResourceStateProjection BorrowAggregate((EffectiveResourceDistributionSnapshot Before, AcceptedEffectiveDistributionMutation Accepted, AcceptedResourceCapacityTransitionBasis BasisA, AcceptedResourceCapacityTransitionBasis BasisB) t)
    {
        var state = t.Accepted.AcceptedSnapshot;
        var pa = ApplicationResourceStateProjectionBuilder.CreateDirect(state.AuthoritativeAllocationSnapshot, AppA, Cpu, T0, Pressure(state.AuthoritativeAllocationSnapshot), state, null, t.BasisA);
        var pb = ApplicationResourceStateProjectionBuilder.CreateDirect(state.AuthoritativeAllocationSnapshot, AppB, Cpu, T0, null, state, null, t.BasisB);
        return new AggregateResourceStateProjection(state.Envelope, T0, new[] { pa, pb });
    }

    private static void ZeroApplicationValid() => Equal(0, new ResourceIntegrationCoherenceSet(Epoch, T0, Array.Empty<ResourceIntegrationCoherenceBinding>()).Bindings.Count);

    private static void FullCurrentChainCoherent()
    {
        var t = BorrowAtoB();
        var aggregate = BorrowAggregate(t);
        var projection = aggregate.Constituents.Single(x => StringComparer.Ordinal.Equals(x.ApplicationId.Value, AppA.Value));
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(projection, T0);
        var baseBinding = new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0, Priority(t.Before.AuthoritativeAllocationSnapshot), Pressure(t.Before.AuthoritativeAllocationSnapshot), null, projection, signal);
        var effectiveLineage = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.DelegatedEffectiveDistribution, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.BasisA });
        var full = new ResourceIntegrationEvidenceBinding(baseBinding, DirectDecision(t.Before.AuthoritativeAllocationSnapshot, AppA), t.Accepted.AcceptedSnapshot, effectiveLineage, aggregate);
        Equal(ResourceIntegrationHealth.CoherentCurrent, full.Health);
        full.RequireCurrent(ResourceIntegrationCurrentContextRequirement.PriorityAndPressure | ResourceIntegrationCurrentContextRequirement.Decision | ResourceIntegrationCurrentContextRequirement.EffectiveState | ResourceIntegrationCurrentContextRequirement.ProjectionAndSignal | ResourceIntegrationCurrentContextRequirement.Coordinator);
    }

    private static void MissingAuthoritativeLineageUnavailable()
    {
        var t = ReduceA();
        var binding = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), Pressure(t.Before));
        Equal(ResourceCoherenceFreshness.Unavailable, binding.PriorityFreshness);
        Equal(ResourceCoherenceFreshness.Unavailable, binding.PressureFreshness);
    }

    private static void OneTransitionLagging()
    {
        var t = ReduceA();
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis });
        var binding = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), Pressure(t.Before), chain);
        Equal(ResourceCoherenceFreshness.Lagging, binding.PriorityFreshness);
        Equal(ResourceCoherenceFreshness.Lagging, binding.PressureFreshness);
        Equal(ResourceIntegrationHealth.CoherentWithLagging, binding.Health);
    }

    private static void MultiTransitionGapFree()
    {
        var first = ReduceA(target: 15m, suffix: "first", appliedAt: T0.AddMinutes(-4));
        var second = ReduceA(first.Accepted.AcceptedSnapshot, 10m, "second", T0.AddMinutes(-2));
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, first.Before.IdentitySha256, second.Accepted.AcceptedSnapshot.IdentitySha256, new[] { first.Basis, second.Basis });
        Require(chain.Bridges(first.Before.IdentitySha256, second.Accepted.AcceptedSnapshot.IdentitySha256), "Two-transition lineage did not bridge exact endpoints.");
    }

    private static void MissingIntermediateRejected()
    {
        var first = ReduceA(target: 15m, suffix: "first-m", appliedAt: T0.AddMinutes(-4));
        var second = ReduceA(first.Accepted.AcceptedSnapshot, 10m, "second-m", T0.AddMinutes(-2));
        Throws<InvalidOperationException>(() => new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, first.Before.IdentitySha256, second.Accepted.AcceptedSnapshot.IdentitySha256, new[] { second.Basis }));
    }

    private static void ReorderedChainRejected()
    {
        var first = ReduceA(target: 15m, suffix: "first-r", appliedAt: T0.AddMinutes(-4));
        var second = ReduceA(first.Accepted.AcceptedSnapshot, 10m, "second-r", T0.AddMinutes(-2));
        Throws<InvalidOperationException>(() => new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, first.Before.IdentitySha256, second.Accepted.AcceptedSnapshot.IdentitySha256, new[] { second.Basis, first.Basis }));
    }

    private static void DuplicateTransitionRejected()
    {
        var t = ReduceA();
        Throws<InvalidOperationException>(() => new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis, t.Basis }));
    }

    private static void DecisionWrongApplicationContradictory()
    {
        var a = Allocations();
        var full = new ResourceIntegrationEvidenceBinding(new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0), DirectDecision(a, AppB));
        Equal(ResourceCoherenceFreshness.Contradictory, full.DecisionFreshness);
    }

    private static void DelegatedEffectiveLineageCurrent()
    {
        var t = BorrowAtoB();
        var lineage = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.DelegatedEffectiveDistribution, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.BasisA });
        var full = new ResourceIntegrationEvidenceBinding(new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0), acceptedEffectiveState: t.Accepted.AcceptedSnapshot, effectiveDistributionLineage: lineage);
        Equal(ResourceCoherenceFreshness.Current, full.EffectiveStateFreshness);
    }

    private static void BorrowedEmptyLineageUnavailable()
    {
        var t = BorrowAtoB();
        var empty = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.DelegatedEffectiveDistribution, t.Accepted.AcceptedSnapshot.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, Array.Empty<AcceptedResourceCapacityTransitionBasis>());
        var full = new ResourceIntegrationEvidenceBinding(new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0), acceptedEffectiveState: t.Accepted.AcceptedSnapshot, effectiveDistributionLineage: empty);
        Equal(ResourceCoherenceFreshness.Unavailable, full.EffectiveStateFreshness);
    }

    private static void CoordinatorFenceConflictContradictory()
    {
        var t = BorrowAtoB();
        var otherEnvelope = Envelope(t.Before.AuthoritativeAllocationSnapshot, 2, "fence-2");
        var otherEffective = Effective(t.Before.AuthoritativeAllocationSnapshot, otherEnvelope, T0.AddMinutes(-1));
        var pa = ApplicationResourceStateProjectionBuilder.CreateDirect(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0, Pressure(t.Before.AuthoritativeAllocationSnapshot), otherEffective);
        var pb = ApplicationResourceStateProjectionBuilder.CreateDirect(t.Before.AuthoritativeAllocationSnapshot, AppB, Cpu, T0, null, otherEffective);
        var aggregate = new AggregateResourceStateProjection(otherEnvelope, T0, new[] { pa, pb });
        var full = new ResourceIntegrationEvidenceBinding(new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0), acceptedEffectiveState: t.Accepted.AcceptedSnapshot, aggregateProjection: aggregate);
        Equal(ResourceCoherenceFreshness.Contradictory, full.CoordinatorFreshness);
    }

    private static void RequireCurrentFailsClosed()
    {
        var a = Allocations();
        var full = new ResourceIntegrationEvidenceBinding(new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0));
        Throws<InvalidOperationException>(() => full.RequireCurrent(ResourceIntegrationCurrentContextRequirement.PriorityAndPressure | ResourceIntegrationCurrentContextRequirement.Decision));
    }

    private static void ApplicationViewExact()
    {
        var a = Allocations();
        var set = new ResourceIntegrationCoherenceSet(Epoch, T0, new[] { new ResourceIntegrationCoherenceBinding(a, AppB, Cpu, T0), new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0) });
        Equal(1, set.GetApplicationView(AppA).Count);
        Equal(AppA.Value, set.GetApplicationView(AppA).Single().ApplicationId.Value);
    }

    private static void SignalProjectionMismatchContradictory()
    {
        var a = Allocations();
        var pa = ApplicationResourceStateProjectionBuilder.CreateDirect(a, AppA, Cpu, T0, Pressure(a), Effective(a));
        var pb = ApplicationResourceStateProjectionBuilder.CreateDirect(a, AppB, Cpu, T0, null, Effective(a));
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(pb, T0);
        var binding = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0, projection: pa, signal: signal);
        Equal(ResourceCoherenceFreshness.Contradictory, binding.SignalFreshness);
    }

    private static void IdentityChangesWithLineage()
    {
        var t = ReduceA();
        var without = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before));
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis });
        var with = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), authoritativeAllocationLineage: chain);
        Require(!StringComparer.Ordinal.Equals(without.IdentitySha256, with.IdentitySha256), "Lineage material did not alter deterministic integrated identity.");
    }

    private static void NoLatestSelectorSurface()
    {
        var types = OwnedTypes();
        var names = types.SelectMany(t => t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => m.Name)).ToArray();
        var forbidden = new[] { "Latest", "MostRecent", "History", "Timeline", "SelectCurrent", "FindCurrent" };
        Require(forbidden.All(term => names.All(name => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "Implicit latest/history selector leaked.");
    }

    private static void NoBusinessOrAuthoritySurface()
    {
        var names = OwnedTypes().SelectMany(t => new[] { t.FullName ?? t.Name }.Concat(t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => m.Name))).ToArray();
        var forbidden = new[] { "FSATS", "FSARM", "TARC", "Trading", "Broker", "Strategy", "ExecuteShedding", "Authorize", "Authenticate", "AdmitApplication", "Stage6", "WP09", "Wp09", "WP10", "Wp10" };
        Require(forbidden.All(term => names.All(name => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "Forbidden Application/authority/work-package public surface leaked.");
    }

    private static Type[] OwnedTypes() => new[]
    {
        typeof(ResourceAcceptedTransitionChain), typeof(ResourceIntegrationCoherenceBinding), typeof(ResourceIntegrationCoherenceSet),
        typeof(ResourceIntegrationEvidenceBinding), typeof(ResourceIntegrationCurrentContextRequirement), typeof(ResourceIntegrationHealth), typeof(ResourceCoherenceFreshness)
    };

    private sealed class SuccessAdapter : IResourceEffectAdapter
    {
        public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt)
            => new(batch.IdentitySha256, true, false, batch.Operations.Select(x => x.OperationId), Evidence("effect", appliedAt), appliedAt);
    }

    private static void Run(string name, Action action)
    {
        try { action(); _passed++; Console.WriteLine($"PASS {name}"); }
        catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); }
    }

    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}' but found '{actual}'."); }
    private static void Throws<T>(Action action) where T : Exception { try { action(); } catch (T) { return; } throw new InvalidOperationException($"Expected {typeof(T).Name} was not thrown."); }
}
