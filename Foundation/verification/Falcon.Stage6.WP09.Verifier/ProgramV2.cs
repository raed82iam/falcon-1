using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP09.Verifier;

internal static class ProgramV2
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
        Run("current_chain_through_projection_signal", CurrentChainThroughProjectionSignal);
        Run("missing_authoritative_lineage_is_unavailable", MissingAuthoritativeLineageIsUnavailable);
        Run("one_transition_bridges_lagging_context", OneTransitionBridgesLaggingContext);
        Run("multi_transition_gap_free_chain", MultiTransitionGapFreeChain);
        Run("missing_intermediate_transition_rejected", MissingIntermediateTransitionRejected);
        Run("ordered_transition_reversal_rejected", OrderedTransitionReversalRejected);
        Run("duplicate_transition_rejected", DuplicateTransitionRejected);
        Run("exact_decision_current_attribution", ExactDecisionCurrentAttribution);
        Run("wrong_decision_application_contradictory", WrongDecisionApplicationContradictory);
        Run("expired_decision_contradictory", ExpiredDecisionContradictory);
        Run("delegated_effective_lineage_current", DelegatedEffectiveLineageCurrent);
        Run("effective_lineage_lane_mismatch_contradictory", EffectiveLineageLaneMismatchContradictory);
        Run("coordinator_constituent_current", CoordinatorConstituentCurrent);
        Run("coordinator_envelope_conflict_contradictory", CoordinatorEnvelopeConflictContradictory);
        Run("require_current_fails_closed_on_unavailable", RequireCurrentFailsClosedOnUnavailable);
        Run("application_view_is_exact", ApplicationViewIsExact);
        Run("signal_projection_mismatch_contradictory", SignalProjectionMismatchContradictory);
        Run("identity_changes_with_lineage", IdentityChangesWithLineage);
        Run("no_latest_selector_surface", NoLatestSelectorSurface);
        Run("no_application_business_or_authority_surface", NoApplicationBusinessOrAuthoritySurface);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-09 VERIFIER V2: {_passed}/{_passed + _failed} PASS");
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
            new[] { new TechnicalCriticalityClassDefinition(new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("cc-high")) },
            new[] { new ResourcePriorityClassRelation(new ResourcePriorityClassId("p-high"), new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("pr")) },
            Array.Empty<TechnicalCriticalityClassRelation>(),
            new[]
            {
                new ApplicationResourcePriorityBinding(AppA, new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("ab-a")),
                new ApplicationResourcePriorityBinding(AppB, new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("ab-b"))
            },
            new[] { new TechnicalCriticalityBinding(new ResourceScopeId("scope-a"), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-a")) }, true);

    private static FoundationResourcePressureSnapshot Pressure(ApplicationResourceAllocationSnapshot a)
        => new(Priority(a), a.ObservedAt.AddMinutes(10),
            new[] { new ResourcePressureTransitionPolicy(Cpu, 6000, 8000, 9500, 500, "pressure-v1", Lifetime(), Evidence("pressure-policy")) },
            new[] { new ResourcePressureObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, Q(10), 1, Evidence("pressure-a")) },
            new[] { new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("reclaim-a")) },
            Array.Empty<ResourceEnforcementObservation>());

    private static ResourceCoordinationEnvelope Envelope(ApplicationResourceAllocationSnapshot a, long fence = 1, string token = "fence-1")
        => new("envelope-authority", new ResourceScopeId("scope-coordination"), "coordinator-1", "aggregate-resource-coordinator", 1, fence, token, a,
            new[]
            {
                new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-a"))),
                new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantB, AppB, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-b")))
            }, Evidence("envelope-" + fence), a.ObservedAt.AddMinutes(1), T0.AddHours(1));

    private static EffectiveResourceDistributionSnapshot Effective(ApplicationResourceAllocationSnapshot a, ResourceCoordinationEnvelope? envelope = null, DateTimeOffset? at = null)
    {
        var e = envelope ?? Envelope(a);
        return new EffectiveResourceDistributionSnapshot(a, e, at ?? a.ObservedAt.AddMinutes(12), Array.Empty<BorrowedEffectiveCapacitySegment>());
    }

    private static ApplicationResourceStateProjection Projection(ApplicationResourceAllocationSnapshot a, ApplicationPrincipalId app, EffectiveResourceDistributionSnapshot? effective = null)
        => ApplicationResourceStateProjectionBuilder.CreateDirect(a, app, Cpu, T0, app == AppA ? Pressure(a) : null, effective ?? Effective(a));

    private static AdditionalResourceDecisionRecord DirectDecision(ApplicationResourceAllocationSnapshot a, ApplicationPrincipalId app = null!)
    {
        var actualApp = app ?? AppA;
        var requesterId = StringComparer.Ordinal.Equals(actualApp.Value, AppA.Value) ? "requester-a" : "requester-b";
        var role = "application-resource-requester";
        var authority = new ResourceRequestAuthorityBinding("request-authority-" + actualApp.Value, requesterId, role, new ResourceScopeId("scope-" + actualApp.Value), new[] { actualApp }, 1, Evidence("request-authority-" + actualApp.Value), T0.AddHours(-1), T0.AddHours(1));
        var request = new AdditionalResourceRequest(new ResourceRequestId("request-" + actualApp.Value), ResourceRequesterKind.DirectApplication, requesterId, role, authority, actualApp, null, Cpu, Q(5), Q(5), a, null,
            new CorrelationId("corr-request-" + actualApp.Value), new CausationId("cause-request-" + actualApp.Value), Evidence("request-" + actualApp.Value), Evidence("residual-" + actualApp.Value), T0.AddMinutes(-8), T0.AddMinutes(30));
        var policy = new ResourceAdditionalRequestDecisionPolicy(Cpu, Q(10), false, Evidence("decision-policy"), T0.AddHours(-1), T0.AddHours(1));
        var decisionAuthority = new ResourceRequestDecisionAuthority("decision-authority", Evidence("decision-authority"), T0.AddHours(-1), T0.AddHours(1));
        return new AdditionalResourceRequestDecisionProcessor(new[] { policy }, decisionAuthority).Evaluate(request, new ResourceDecisionId("decision-" + actualApp.Value), T0.AddMinutes(-7));
    }

    private static (ApplicationResourceAllocationSnapshot Before, AcceptedFoundationAllocationMutation Accepted, ResourceEffectBatch Batch, AcceptedResourceCapacityTransitionBasis Basis) ReduceA(ApplicationResourceAllocationSnapshot? predecessor = null, decimal target = 10m, string suffix = "1", DateTimeOffset? appliedAt = null)
    {
        var before = predecessor ?? Allocations();
        var authority = new FoundationResourceMutationAuthority("mutation-authority-" + suffix, new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1, Evidence("mutation-authority-" + suffix), T0.AddHours(-1), T0.AddHours(1));
        var intent = new FoundationAllocationMutationIntent("reduce-a-" + suffix, ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(target), Q(Math.Max(target, 20m)), Q(Math.Max(target, 30m)), authority, null, before.IdentitySha256,
            new CorrelationId("corr-reduce-" + suffix), new CausationId("cause-reduce-" + suffix), Evidence("reduce-intent-" + suffix), T0.AddMinutes(-10), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-reduce-a-" + suffix, new[] { ResourceEffectOperation.ForFoundation(intent) });
        var at = appliedAt ?? T0.AddMinutes(-3);
        var accepted = new ResourceMutationProcessor().ApplyFoundationAllocationMutations(before, "batch-reduce-a-" + suffix, new[] { intent }, new SuccessAdapter(), at, Effective(before, at: at.AddMinutes(-1)));
        var basis = AcceptedResourceCapacityTransitionBasis.FromFoundationMutation(before, accepted, batch, AppA, Cpu);
        return (before, accepted, batch, basis);
    }

    private static (EffectiveResourceDistributionSnapshot Before, AcceptedEffectiveDistributionMutation Accepted, ResourceEffectBatch Batch, AcceptedResourceCapacityTransitionBasis BasisA, AcceptedResourceCapacityTransitionBasis BasisB) BorrowAtoB()
    {
        var a = Allocations();
        var envelope = Envelope(a);
        var before = Effective(a, envelope, T0.AddMinutes(-6));
        var intent = new EffectiveDistributionMutationIntent("borrow-a-b", EffectiveDistributionOperationKind.Borrow, AppA, GrantA, AppB, Cpu, Q(5), null, envelope,
            envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken, new CorrelationId("corr-borrow"), new CausationId("cause-borrow"), Evidence("borrow-intent"), T0.AddMinutes(-5), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-borrow-a-b", new[] { ResourceEffectOperation.ForEffective(intent) });
        var accepted = new ResourceMutationProcessor().ApplyEffectiveRedistribution(before, "batch-borrow-a-b", new[] { intent }, new SuccessAdapter(), T0.AddMinutes(-2));
        return (before, accepted, batch,
            AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(before, accepted, batch, AppA, Cpu),
            AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(before, accepted, batch, AppB, Cpu));
    }

    private static AggregateResourceStateProjection AggregateForBorrow((EffectiveResourceDistributionSnapshot Before, AcceptedEffectiveDistributionMutation Accepted, ResourceEffectBatch Batch, AcceptedResourceCapacityTransitionBasis BasisA, AcceptedResourceCapacityTransitionBasis BasisB) t, ResourceCoordinationEnvelope? overrideEnvelope = null)
    {
        var state = t.Accepted.AcceptedSnapshot;
        if (overrideEnvelope is null)
        {
            var pa = ApplicationResourceStateProjectionBuilder.CreateDirect(state.AuthoritativeAllocationSnapshot, AppA, Cpu, T0, Pressure(state.AuthoritativeAllocationSnapshot), state, null, t.BasisA);
            var pb = ApplicationResourceStateProjectionBuilder.CreateDirect(state.AuthoritativeAllocationSnapshot, AppB, Cpu, T0, null, state, null, t.BasisB);
            return new AggregateResourceStateProjection(state.Envelope, T0, new[] { pa, pb });
        }
        var altState = Effective(state.AuthoritativeAllocationSnapshot, overrideEnvelope, T0.AddMinutes(-1));
        var paAlt = Projection(state.AuthoritativeAllocationSnapshot, AppA, altState);
        var pbAlt = Projection(state.AuthoritativeAllocationSnapshot, AppB, altState);
        return new AggregateResourceStateProjection(overrideEnvelope, T0, new[] { paAlt, pbAlt });
    }

    private static void ZeroApplicationSetValid()
        => Equal(0, new ResourceIntegrationCoherenceSet(Epoch, T0, Array.Empty<ResourceIntegrationCoherenceBinding>()).Bindings.Count);

    private static void CurrentChainThroughProjectionSignal()
    {
        var a = Allocations();
        var projection = Projection(a, AppA);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(projection, T0);
        var c = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0, Priority(a), Pressure(a), null, projection, signal);
        Equal(ResourceIntegrationHealth.CoherentCurrent, c.Health);
    }

    private static void MissingAuthoritativeLineageIsUnavailable()
    {
        var t = ReduceA();
        var c = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), Pressure(t.Before));
        Equal(ResourceCoherenceFreshness.Unavailable, c.PriorityFreshness);
        Equal(ResourceCoherenceFreshness.Unavailable, c.PressureFreshness);
    }

    private static void OneTransitionBridgesLaggingContext()
    {
        var t = ReduceA();
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis });
        var c = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), Pressure(t.Before), chain);
        Equal(ResourceCoherenceFreshness.Lagging, c.PriorityFreshness);
        Equal(ResourceCoherenceFreshness.Lagging, c.PressureFreshness);
    }

    private static void MultiTransitionGapFreeChain()
    {
        var first = ReduceA(target: 15m, suffix: "first", appliedAt: T0.AddMinutes(-4));
        var second = ReduceA(first.Accepted.AcceptedSnapshot, 10m, "second", T0.AddMinutes(-2));
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, first.Before.IdentitySha256, second.Accepted.AcceptedSnapshot.IdentitySha256, new[] { first.Basis, second.Basis });
        Require(chain.Bridges(first.Before.IdentitySha256, second.Accepted.AcceptedSnapshot.IdentitySha256), "Gap-free two-transition chain did not bridge declared endpoints.");
    }

    private static void MissingIntermediateTransitionRejected()
    {
        var first = ReduceA(target: 15m, suffix: "first-m", appliedAt: T0.AddMinutes(-4));
        var second = ReduceA(first.Accepted.AcceptedSnapshot, 10m, "second-m", T0.AddMinutes(-2));
        Throws<InvalidOperationException>(() => new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, first.Before.IdentitySha256, second.Accepted.AcceptedSnapshot.IdentitySha256, new[] { second.Basis }));
    }

    private static void OrderedTransitionReversalRejected()
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

    private static void ExactDecisionCurrentAttribution()
    {
        var a = Allocations();
        var c = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0);
        var e = new ResourceIntegrationEvidenceBinding(c, exactDecision: DirectDecision(a));
        Equal(ResourceCoherenceFreshness.Current, e.DecisionFreshness);
    }

    private static void WrongDecisionApplicationContradictory()
    {
        var a = Allocations();
        var c = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0);
        var e = new ResourceIntegrationEvidenceBinding(c, exactDecision: DirectDecision(a, AppB));
        Equal(ResourceCoherenceFreshness.Contradictory, e.DecisionFreshness);
    }

    private static void ExpiredDecisionContradictory()
    {
        var a = Allocations();
        var c = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0.AddHours(2));
        var e = new ResourceIntegrationEvidenceBinding(c, exactDecision: DirectDecision(a));
        Equal(ResourceCoherenceFreshness.Contradictory, e.DecisionFreshness);
    }

    private static void DelegatedEffectiveLineageCurrent()
    {
        var t = BorrowAtoB();
        var lineage = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.DelegatedEffectiveDistribution, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.BasisA });
        var projection = ApplicationResourceStateProjectionBuilder.CreateDirect(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0, Pressure(t.Before.AuthoritativeAllocationSnapshot), t.Accepted.AcceptedSnapshot, null, t.BasisA);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(projection, T0);
        var c = new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0, Priority(t.Before.AuthoritativeAllocationSnapshot), Pressure(t.Before.AuthoritativeAllocationSnapshot), null, projection, signal);
        var e = new ResourceIntegrationEvidenceBinding(c, acceptedEffectiveState: t.Accepted.AcceptedSnapshot, effectiveDistributionLineage: lineage);
        Equal(ResourceCoherenceFreshness.Current, e.EffectiveStateFreshness);
    }

    private static void EffectiveLineageLaneMismatchContradictory()
    {
        var t = BorrowAtoB();
        var fake = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, t.Accepted.AcceptedSnapshot.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, Array.Empty<AcceptedResourceCapacityTransitionBasis>());
        var c = new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0);
        var e = new ResourceIntegrationEvidenceBinding(c, acceptedEffectiveState: t.Accepted.AcceptedSnapshot, effectiveDistributionLineage: fake);
        Equal(ResourceCoherenceFreshness.Contradictory, e.EffectiveStateFreshness);
    }

    private static void CoordinatorConstituentCurrent()
    {
        var t = BorrowAtoB();
        var lineage = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.DelegatedEffectiveDistribution, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.BasisA });
        var aggregate = AggregateForBorrow(t);
        var appProjection = aggregate.Constituents.Single(x => StringComparer.Ordinal.Equals(x.ApplicationId.Value, AppA.Value));
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(appProjection, T0);
        var c = new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0, Priority(t.Before.AuthoritativeAllocationSnapshot), Pressure(t.Before.AuthoritativeAllocationSnapshot), null, appProjection, signal);
        var e = new ResourceIntegrationEvidenceBinding(c, acceptedEffectiveState: t.Accepted.AcceptedSnapshot, effectiveDistributionLineage: lineage, aggregateProjection: aggregate);
        Equal(ResourceCoherenceFreshness.Current, e.CoordinatorFreshness);
    }

    private static void CoordinatorEnvelopeConflictContradictory()
    {
        var t = BorrowAtoB();
        var conflictingEnvelope = Envelope(t.Before.AuthoritativeAllocationSnapshot, 2, "fence-2");
        var aggregate = AggregateForBorrow(t, conflictingEnvelope);
        var c = new ResourceIntegrationCoherenceBinding(t.Before.AuthoritativeAllocationSnapshot, AppA, Cpu, T0);
        var e = new ResourceIntegrationEvidenceBinding(c, acceptedEffectiveState: t.Accepted.AcceptedSnapshot, aggregateProjection: aggregate);
        Equal(ResourceCoherenceFreshness.Contradictory, e.CoordinatorFreshness);
    }

    private static void RequireCurrentFailsClosedOnUnavailable()
    {
        var a = Allocations();
        var e = new ResourceIntegrationEvidenceBinding(new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0));
        Throws<InvalidOperationException>(() => e.RequireCurrent(ResourceIntegrationCurrentContextRequirement.PriorityAndPressure | ResourceIntegrationCurrentContextRequirement.Decision));
    }

    private static void ApplicationViewIsExact()
    {
        var a = Allocations();
        var set = new ResourceIntegrationCoherenceSet(Epoch, T0, new[] { new ResourceIntegrationCoherenceBinding(a, AppB, Cpu, T0), new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0) });
        Equal(1, set.GetApplicationView(AppA).Count);
        Equal(AppA.Value, set.GetApplicationView(AppA).Single().ApplicationId.Value);
    }

    private static void SignalProjectionMismatchContradictory()
    {
        var a = Allocations();
        var pA = Projection(a, AppA);
        var signalB = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppB), T0);
        var c = new ResourceIntegrationCoherenceBinding(a, AppA, Cpu, T0, projection: pA, signal: signalB);
        Equal(ResourceCoherenceFreshness.Contradictory, c.SignalFreshness);
    }

    private static void IdentityChangesWithLineage()
    {
        var t = ReduceA();
        var without = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before));
        var chain = new ResourceAcceptedTransitionChain(AppA, Cpu, ResourceCapacityBasisLane.FoundationAuthoritativeAllocation, t.Before.IdentitySha256, t.Accepted.AcceptedSnapshot.IdentitySha256, new[] { t.Basis });
        var with = new ResourceIntegrationCoherenceBinding(t.Accepted.AcceptedSnapshot, AppA, Cpu, T0, Priority(t.Before), authoritativeAllocationLineage: chain);
        Require(!StringComparer.Ordinal.Equals(without.IdentitySha256, with.IdentitySha256), "Lineage/freshness change did not alter integrated identity.");
    }

    private static void NoLatestSelectorSurface()
    {
        var types = new[] { typeof(ResourceAcceptedTransitionChain), typeof(ResourceIntegrationCoherenceBinding), typeof(ResourceIntegrationEvidenceBinding), typeof(ResourceIntegrationCoherenceSet) };
        var names = types.SelectMany(t => t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => m.Name)).ToArray();
        var forbidden = new[] { "Latest", "MostRecent", "History", "Timeline", "SelectCurrent", "FindCurrent" };
        Require(forbidden.All(term => names.All(name => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "Implicit latest/history selector surface leaked.");
    }

    private static void NoApplicationBusinessOrAuthoritySurface()
    {
        var types = new[] { typeof(ResourceAcceptedTransitionChain), typeof(ResourceIntegrationCoherenceBinding), typeof(ResourceIntegrationEvidenceBinding), typeof(ResourceIntegrationCoherenceSet), typeof(ResourceIntegrationCurrentContextRequirement) };
        var names = types.SelectMany(t => new[] { t.FullName ?? t.Name }.Concat(t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => m.Name))).ToArray();
        var forbidden = new[] { "FSATS", "FSARM", "TARC", "Trading", "Broker", "Strategy", "ExecuteShedding", "Authorize", "Authenticate", "AdmitApplication", "Stage6", "WP09", "Wp09", "WP10", "Wp10" };
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
