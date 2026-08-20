using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP09.Verifier;

internal static class Program
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
        Run("zero_application_set_valid", ZeroApplicationSetValid);
        Run("current_context_coherent", CurrentContextCoherent);
        Run("missing_lineage_is_unavailable", MissingLineageIsUnavailable);
        Run("supplied_conflicting_lineage_is_contradictory", SuppliedConflictingLineageIsContradictory);
        Run("accepted_transition_bridges_lagging_context", AcceptedTransitionBridgesLaggingContext);
        Run("transition_chain_gap_rejected", TransitionChainGapRejected);
        Run("transition_chain_duplicate_rejected", TransitionChainDuplicateRejected);
        Run("application_view_is_exact", ApplicationViewIsExact);
        Run("projection_signal_current_coherence", ProjectionSignalCurrentCoherence);
        Run("signal_projection_mismatch_is_contradictory", SignalProjectionMismatchIsContradictory);
        Run("identity_changes_with_freshness", IdentityChangesWithFreshness);
        Run("no_application_business_or_authority_surface", NoApplicationBusinessOrAuthoritySurface);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-09 VERIFIER: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime() => new(T0.AddHours(-2), null, true);
    private static ResourceEvidenceReference Evidence(string id, DateTimeOffset? at = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId("scope-" + id), at ?? T0.AddMinutes(-50), Epoch);

    private static FoundationResourceTruthSnapshot Truth()
        => new(Epoch, T0.AddMinutes(-40), new[]
        {
            new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", T0.AddMinutes(-41)))
        }, true);

    private static ApplicationResourceAllocationSnapshot Allocations(decimal appA = 20m, decimal appB = 20m, DateTimeOffset? at = null)
        => new(Truth(), at ?? T0.AddMinutes(-30), new[]
        {
            new ApplicationResourceAllocation(GrantA, AppA, Cpu, Q(appA), Q(Math.Max(appA, 30m)), Q(Math.Max(appA, 40m)), Lifetime(), Evidence("allocation-a")),
            new ApplicationResourceAllocation(GrantB, AppB, Cpu, Q(appB), Q(Math.Max(appB, 30m)), Q(Math.Max(appB, 40m)), Lifetime(), Evidence("allocation-b"))
        }, true);

    private static ResourcePriorityGovernanceSnapshot Priority(ApplicationResourceAllocationSnapshot a)
        => new(a, a.ObservedAt.AddMinutes(5), "priority-v1", Lifetime(), Evidence("priority-policy"), "criticality-v1", Lifetime(), Evidence("criticality-policy"),
            new[]
            {
                new ResourcePriorityClassDefinition(new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("pc-high")),
                new ResourcePriorityClassDefinition(new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("pc-low"))
            },
            new[]
            {
                new TechnicalCriticalityClassDefinition(new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("cc-high"))
            },
            new[]
            {
                new ResourcePriorityClassRelation(new ResourcePriorityClassId("p-high"), new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("pr"))
            },
            Array.Empty<TechnicalCriticalityClassRelation>(),
            new[]
            {
                new ApplicationResourcePriorityBinding(AppA, new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("ab-a")),
                new ApplicationResourcePriorityBinding(AppB, new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("ab-b"))
            },
            new[]
            {
                new TechnicalCriticalityBinding(new ResourceScopeId("scope-a"), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-a"))
            }, true);

    private static FoundationResourcePressureSnapshot Pressure(ApplicationResourceAllocationSnapshot a)
        => new(Priority(a), a.ObservedAt.AddMinutes(10),
            new[] { new ResourcePressureTransitionPolicy(Cpu, 6000, 8000, 9500, 500, "pressure-v1", Lifetime(), Evidence("pressure-policy")) },
            new[] { new ResourcePressureObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, Q(10), 1, Evidence("pressure-a")) },
            new[] { new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("reclaim-a")) },
            Array.Empty<ResourceEnforcementObservation>());

    private static ResourceCoordinationEnvelope Envelope(ApplicationResourceAllocationSnapshot a)
        => new("envelope-authority", new ResourceScopeId("scope-coordination"), "coordinator-1", "aggregate-resource-coordinator", 1, 1, "fence-1", a,
            new[]
            {
                new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-a"))),
                new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantB, AppB, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-b")))
            }, Evidence("envelope"), a.ObservedAt.AddMinutes(1), T0.AddHours(1));

    private static EffectiveResourceDistributionSnapshot Effective(ApplicationResourceAllocationSnapshot a)
    {
        var e = Envelope(a);
        return new EffectiveResourceDistributionSnapshot(a, e, a.ObservedAt.AddMinutes(12), Array.Empty<BorrowedEffectiveCapacitySegment>());
    }

    private static ApplicationResourceStateProjection Projection(ApplicationResourceAllocationSnapshot a, ApplicationPrincipalId app)
        => ApplicationResourceStateProjectionBuilder.CreateDirect(a, app, Cpu, T0, app == AppA ? Pressure(a) : null, Effective(a));

    private static (ApplicationResourceAllocationSnapshot Before, AcceptedFoundationAllocationMutation Accepted, ResourceEffectBatch Batch, AcceptedResourceCapacityTransitionBasis Basis) ReduceA()
    {
        var before = Allocations();
        var authority = new FoundationResourceMutationAuthority("mutation-authority", new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1, Evidence("mutation-authority"), T0.AddHours(-1), T0.AddHours(1));
        var intent = new FoundationAllocationMutationIntent("reduce-a", ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(10), Q(20), Q(30), authority, null, before.IdentitySha256,
            new CorrelationId("corr-reduce"), new CausationId("cause-reduce"), Evidence("reduce-intent"), T0.AddMinutes(-5), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-reduce-a", new[] { ResourceEffectOperation.ForFoundation(intent) });
        var accepted = new ResourceMutationProcessor().ApplyFoundationAllocationMutations(before, "batch-reduce-a", new[] { intent }, new SuccessAdapter(), T0.AddMinutes(-1), Effective(before));
        var basis = AcceptedResourceCapacityTransitionBasis.FromFoundationMutation(before, accepted, batch, AppA, Cpu);
        return (before, accepted, batch, basis);
    }

    private static void ZeroApplicationSetValid()
    {
        var set = new ResourceIntegrationCoherenceSet(Epoch, T0, Array.Empty<ResourceIntegrationCoherenceBinding>());
        Equal(0, set.Bindings.Count);
    }

    private static void CurrentContextCoherent()
    {
        var a = Allocations();
        var p = Priority(a);
        var pressure = Pressure(a);
        var projection = Projection(a, AppA);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(projection, T0);
        var b = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0, p, pressure, null, projection, signal);
        Equal(ResourceCoherenceFreshness.Current, b.PriorityFreshness);
        Equal(ResourceCoherenceFreshness.Current, b.PressureFreshness);
        Equal(ResourceCoherenceFreshness.Current, b.ProjectionFreshness);
        Equal(ResourceCoherenceFreshness.Current, b.SignalFreshness);
        Equal(ResourceIntegrationHealth.CoherentCurrent, b.Health);
    }

    private static void MissingLineageIsUnavailable()
    {
        var t = ReduceA();
        var b = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), Pressure(t.Before));
        Equal(ResourceCoherenceFreshness.Unavailable, b.PriorityFreshness);
        Equal(ResourceCoherenceFreshness.Unavailable, b.PressureFreshness);
        Equal(ResourceIntegrationHealth.Unavailable, b.Health);
    }

    private static void SuppliedConflictingLineageIsContradictory()
    {
        var t = ReduceA();
        var conflicting = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation,
            t.Accepted.AcceptedSnapshot.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, Array.Empty<AcceptedResourceCapacityTransitionBasis>());
        var b = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), Pressure(t.Before), conflicting);
        Equal(ResourceCoherenceFreshness.Contradictory, b.PriorityFreshness);
        Equal(ResourceIntegrationHealth.Contradictory, b.Health);
    }

    private static void AcceptedTransitionBridgesLaggingContext()
    {
        var t = ReduceA();
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation,
            t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis });
        var b = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), Pressure(t.Before), chain, Projection(t.Before, AppA), ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Before, AppA), T0));
        Equal(ResourceCoherenceFreshness.Lagging, b.PriorityFreshness);
        Equal(ResourceCoherenceFreshness.Lagging, b.PressureFreshness);
        Equal(ResourceCoherenceFreshness.Lagging, b.ProjectionFreshness);
        Equal(ResourceCoherenceFreshness.Lagging, b.SignalFreshness);
        Equal(ResourceIntegrationHealth.CoherentWithLagging, b.Health);
    }

    private static void TransitionChainGapRejected()
    {
        var t = ReduceA();
        Throws<InvalidOperationException>(() => new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation,
            "wrong-start", t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis }));
    }

    private static void TransitionChainDuplicateRejected()
    {
        var t = ReduceA();
        Throws<InvalidOperationException>(() => new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation,
            t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis, t.Basis }));
    }

    private static void ApplicationViewIsExact()
    {
        var a = Allocations();
        var aBinding = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0);
        var bBinding = new ResourceIntegrationCoherenceBinding(a, AppB, Cpu, T0);
        var set = new ResourceIntegrationCoherenceSet(Epoch, T0, new[] { bBinding, aBinding });
        Equal(1, set.GetApplicationView(AppA).Count);
        Equal(AppA.Value, set.GetApplicationView(AppA).Single().ApplicationId.Value);
    }

    private static void ProjectionSignalCurrentCoherence()
    {
        var a = Allocations();
        var projection = Projection(a, AppA);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(projection, T0);
        var binding = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0, projection: projection, signal: signal);
        Equal(ResourceCoherenceFreshness.Current, binding.ProjectionFreshness);
        Equal(ResourceCoherenceFreshness.Current, binding.SignalFreshness);
    }

    private static void SignalProjectionMismatchIsContradictory()
    {
        var a = Allocations();
        var pA = Projection(a, AppA);
        var pB = Projection(a, AppB);
        var signalB = ApplicationResourceLoadSheddingSignalFactory.Create(pB, T0);
        var binding = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0, projection: pA, signal: signalB);
        Equal(ResourceCoherenceFreshness.Contradictory, binding.SignalFreshness);
    }

    private static void IdentityChangesWithFreshness()
    {
        var t = ReduceA();
        var without = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before));
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation,
            t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis });
        var with = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), authoritativeAllocationLineage: chain);
        Require(!StringComparer.Ordinal.Equals(without.IdentitySha256, with.IdentitySha256), "Freshness/lineage change did not alter integrated identity.");
    }

    private static void NoApplicationBusinessOrAuthoritySurface()
    {
        var types = new[] { typeof(ResourceAcceptedTransitionChain), typeof(ResourceIntegrationCoherenceBinding), typeof(ResourceIntegrationCoherenceSet) };
        var names = types.SelectMany(t => new[] { t.FullName ?? t.Name }.Concat(t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => m.Name))).ToArray();
        var forbidden = new[] { "FSATS", "FSARM", "TARC", "Trading", "Broker", "Strategy", "Execute", "Authorize", "Authenticate", "AdmitApplication", "Stage6", "WP09", "Wp09", "WP10", "Wp10" };
        Require(forbidden.All(term => names.All(name => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "Forbidden Application/authority/work-package surface leaked.");
    }

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

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"Expected '{expected}' but found '{actual}'.");
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name} was not thrown.");
    }
}
