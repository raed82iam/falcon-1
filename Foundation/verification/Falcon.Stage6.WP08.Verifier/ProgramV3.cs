using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP08.Verifier;

internal static class ProgramV3
{
    private static int _passed;
    private static int _failed;
    private static readonly DateTimeOffset T0 = new(2026, 8, 10, 10, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");
    private static readonly ApplicationPrincipalId AppA = new("app-a");
    private static readonly ApplicationPrincipalId AppB = new("app-b");
    private static readonly ResourceGrantId GrantA = new("grant-a");
    private static readonly ResourceGrantId GrantB = new("grant-b");

    private static int Main()
    {
        Run("zero_application_projection_valid", ZeroApplicationProjectionValid);
        Run("direct_projection_binds_authoritative_truth", DirectProjectionBindsAuthoritativeTruth);
        Run("missing_effective_truth_fails_closed", MissingEffectiveTruthFailsClosed);
        Run("quiescent_effective_truth_positive", QuiescentEffectiveTruthPositive);
        Run("borrowed_state_requires_exact_transition_basis", BorrowedStateRequiresExactTransitionBasis);
        Run("borrowed_provenance_preserved", BorrowedProvenancePreserved);
        Run("aggregate_projection_preserves_constituents", AggregateProjectionPreservesConstituents);
        Run("aggregate_constituent_mismatch_rejected", AggregateConstituentMismatchRejected);
        Run("critical_pressure_is_advisory_only", CriticalPressureIsAdvisoryOnly);
        Run("enforcement_observation_not_authority", EnforcementObservationNotAuthority);
        Run("normal_pressure_no_action", NormalPressureNoAction);
        Run("unavailable_pressure_state_unavailable", UnavailablePressureStateUnavailable);
        Run("wrong_effect_batch_rejected", WrongEffectBatchRejected);
        Run("effective_reduction_compliance", EffectiveReductionCompliance);
        Run("foundation_reduction_compliance", FoundationReductionCompliance);
        Run("exact_use_only_drives_reduction_quantity", ExactUseOnlyDrivesReductionQuantity);
        Run("additional_resource_decision_not_applied_capacity", AdditionalResourceDecisionNotAppliedCapacity);
        Run("no_application_business_or_executor_surface", NoApplicationBusinessOrExecutorSurface);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-08 VERIFIER V3: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime() => new(T0.AddHours(-2), null, true);
    private static ResourceEvidenceReference Evidence(string id, DateTimeOffset? at = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId("scope-" + id), at ?? T0.AddMinutes(-40), Epoch);

    private static FoundationResourceTruthSnapshot Truth()
        => new(Epoch, T0.AddMinutes(-30), new[] { new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", T0.AddMinutes(-31))) }, true);

    private static ApplicationResourceAllocationSnapshot Allocations()
        => new(Truth(), T0.AddMinutes(-20), new[]
        {
            new ApplicationResourceAllocation(GrantA, AppA, Cpu, Q(20), Q(30), Q(40), Lifetime(), Evidence("allocation-a")),
            new ApplicationResourceAllocation(GrantB, AppB, Cpu, Q(20), Q(30), Q(40), Lifetime(), Evidence("allocation-b"))
        }, true);

    private static ResourcePriorityGovernanceSnapshot Priority(ApplicationResourceAllocationSnapshot a)
        => new(a, T0.AddMinutes(-15), "priority-v1", Lifetime(), Evidence("priority-policy"), "criticality-v1", Lifetime(), Evidence("criticality-policy"),
            new[] { new ResourcePriorityClassDefinition(new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("pc-high")), new ResourcePriorityClassDefinition(new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("pc-low")) },
            new[] { new TechnicalCriticalityClassDefinition(new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("cc-high")), new TechnicalCriticalityClassDefinition(new TechnicalCriticalityClassId("c-low"), Lifetime(), Evidence("cc-low")) },
            new[] { new ResourcePriorityClassRelation(new ResourcePriorityClassId("p-high"), new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("pr")) },
            new[] { new TechnicalCriticalityClassRelation(new TechnicalCriticalityClassId("c-high"), new TechnicalCriticalityClassId("c-low"), Lifetime(), Evidence("cr")) },
            new[] { new ApplicationResourcePriorityBinding(AppA, new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("ab-a")), new ApplicationResourcePriorityBinding(AppB, new ResourcePriorityClassId("p-low"), Lifetime(), Evidence("ab-b")) },
            new[] { new TechnicalCriticalityBinding(new ResourceScopeId("scope-a"), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-a")), new TechnicalCriticalityBinding(new ResourceScopeId("scope-b"), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-b")) }, true);

    private static FoundationResourcePressureSnapshot Pressure(ApplicationResourceAllocationSnapshot a, decimal? used, ResourceEnforcementObservationState enforcement = ResourceEnforcementObservationState.None)
    {
        var obs = new[] { new ResourcePressureObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, used.HasValue ? Q(used.Value) : null, 1, Evidence("pressure-a")) };
        var eligibility = new[] { new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("reclaim-a")) };
        var en = enforcement == ResourceEnforcementObservationState.None ? Array.Empty<ResourceEnforcementObservation>() : new[] { new ResourceEnforcementObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, enforcement, Evidence("enforcement-a")) };
        return new FoundationResourcePressureSnapshot(Priority(a), T0.AddMinutes(-10), new[] { new ResourcePressureTransitionPolicy(Cpu, 6000, 8000, 9500, 500, "pressure-v1", Lifetime(), Evidence("pressure-policy")) }, obs, eligibility, en);
    }

    private static ResourceCoordinationEnvelope Envelope(ApplicationResourceAllocationSnapshot? a = null)
    {
        var x = a ?? Allocations();
        return new ResourceCoordinationEnvelope("envelope-authority", new ResourceScopeId("scope-coordination"), "coordinator-1", "aggregate-resource-coordinator", 1, 1, "fence-1", x,
            new[]
            {
                new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-a"))),
                new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantB, AppB, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-b")))
            }, Evidence("envelope"), T0.AddMinutes(-18), T0.AddHours(1));
    }

    private static EffectiveResourceDistributionSnapshot Effective(ResourceCoordinationEnvelope e, IEnumerable<BorrowedEffectiveCapacitySegment>? segments = null, DateTimeOffset? at = null)
        => new(e.AllocationSnapshot, e, at ?? T0.AddMinutes(-5), segments ?? Array.Empty<BorrowedEffectiveCapacitySegment>());

    private static (EffectiveResourceDistributionSnapshot Before, AcceptedEffectiveDistributionMutation Accepted, ResourceEffectBatch Batch) BorrowTransition()
    {
        var e = Envelope();
        var before = Effective(e);
        var intent = new EffectiveDistributionMutationIntent("borrow-1", EffectiveDistributionOperationKind.Borrow, AppA, GrantA, AppB, Cpu, Q(5), null, e,
            e.CoordinatorInstanceId, e.CoordinatorRoleId, e.FenceGeneration, e.FencingToken, new CorrelationId("corr-borrow"), new CausationId("cause-borrow"), Evidence("borrow-intent"), T0.AddMinutes(-4), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-borrow", new[] { ResourceEffectOperation.ForEffective(intent) });
        var accepted = new ResourceMutationProcessor().ApplyEffectiveRedistribution(before, "batch-borrow", new[] { intent }, new SuccessAdapter(), T0);
        return (before, accepted, batch);
    }

    private static (ApplicationResourceAllocationSnapshot Before, AcceptedFoundationAllocationMutation Accepted, ResourceEffectBatch Batch) FoundationReduction()
    {
        var a = Allocations();
        var authority = new FoundationResourceMutationAuthority("mutation-authority", new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1, Evidence("mutation-authority"), T0.AddHours(-1), T0.AddHours(1));
        var intent = new FoundationAllocationMutationIntent("reduce-1", ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(10), Q(20), Q(30), authority, null, a.IdentitySha256,
            new CorrelationId("corr-reduce"), new CausationId("cause-reduce"), Evidence("reduce-intent"), T0.AddMinutes(-4), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-reduce", new[] { ResourceEffectOperation.ForFoundation(intent) });
        var accepted = new ResourceMutationProcessor().ApplyFoundationAllocationMutations(a, "batch-reduce", new[] { intent }, new SuccessAdapter(), T0, Effective(Envelope(a), at: T0.AddMinutes(-2)));
        return (a, accepted, batch);
    }

    private static ApplicationResourceStateProjection Projection(ApplicationResourceAllocationSnapshot a, ApplicationPrincipalId app, FoundationResourcePressureSnapshot? pressure = null, EffectiveResourceDistributionSnapshot? effective = null, AdditionalResourceDecisionRecord? decision = null, AcceptedResourceCapacityTransitionBasis? basis = null, ExactApplicationResourceUseObservation? use = null, DateTimeOffset? at = null)
        => ApplicationResourceStateProjectionBuilder.CreateDirect(a, app, Cpu, at ?? T0, pressure, effective, decision, basis, use);

    private static AdditionalResourceDecisionRecord DirectDecision(ApplicationResourceAllocationSnapshot a)
    {
        var auth = new ResourceRequestAuthorityBinding("request-authority", "requester-a", "application-resource-requester", new ResourceScopeId("scope-app-a"), new[] { AppA }, 1, Evidence("request-authority"), T0.AddHours(-1), T0.AddHours(1));
        var request = new AdditionalResourceRequest(new ResourceRequestId("request-a"), ResourceRequesterKind.DirectApplication, "requester-a", "application-resource-requester", auth, AppA, null, Cpu, Q(5), Q(5), a, null,
            new CorrelationId("corr-request"), new CausationId("cause-request"), Evidence("request"), Evidence("residual"), T0.AddMinutes(-8), T0.AddMinutes(30));
        var policy = new ResourceAdditionalRequestDecisionPolicy(Cpu, Q(10), false, Evidence("decision-policy"), T0.AddHours(-1), T0.AddHours(1));
        var authority = new ResourceRequestDecisionAuthority("decision-authority", Evidence("decision-authority"), T0.AddHours(-1), T0.AddHours(1));
        return new AdditionalResourceRequestDecisionProcessor(new[] { policy }, authority).Evaluate(request, new ResourceDecisionId("decision-a"), T0.AddMinutes(-7));
    }

    private static void ZeroApplicationProjectionValid() => Equal(0, new ApplicationResourceStateProjectionSet(Epoch, T0, Array.Empty<ApplicationResourceStateProjection>()).Projections.Count);
    private static void DirectProjectionBindsAuthoritativeTruth() { var p = Projection(Allocations(), AppA); Equal(20m, p.Allocation.Amount); Equal(40m, p.Ceiling.Amount); }
    private static void MissingEffectiveTruthFailsClosed() => Require(!Projection(Allocations(), AppA).EffectiveCapacityAvailable, "Missing effective truth was invented.");
    private static void QuiescentEffectiveTruthPositive() { var a = Allocations(); Equal(20m, Projection(a, AppA, effective: Effective(Envelope(a))).EffectiveCapacity!.Amount); }
    private static void BorrowedStateRequiresExactTransitionBasis() { var t = BorrowTransition(); Throws<InvalidOperationException>(() => Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot)); }
    private static void BorrowedProvenancePreserved() { var t = BorrowTransition(); var basis = AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); var p = Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis); Equal(GrantA.Value, p.BorrowedProvenance.Single().SourceGrantId.Value); }
    private static void AggregateProjectionPreservesConstituents() { var a = Allocations(); var e = Envelope(a); var effective = Effective(e); Equal(2, new AggregateResourceStateProjection(e, T0, new[] { Projection(a, AppA, effective: effective), Projection(a, AppB, effective: effective) }).Constituents.Count); }
    private static void AggregateConstituentMismatchRejected() { var a = Allocations(); var e = Envelope(a); Throws<InvalidOperationException>(() => new AggregateResourceStateProjection(e, T0, new[] { Projection(a, AppA, effective: Effective(e)) })); }
    private static void CriticalPressureIsAdvisoryOnly() { var a = Allocations(); var s = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, Pressure(a, 39), Effective(Envelope(a))), T0); Equal(TechnicalLoadSheddingSignalClass.AdvisoryReduction, s.SignalClass); Require(s.AcceptedCapacityBasisIdentitySha256 is null, "Pressure minted authority."); }
    private static void EnforcementObservationNotAuthority() { var a = Allocations(); Equal(TechnicalLoadSheddingSignalClass.AdvisoryReduction, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, Pressure(a, 39, ResourceEnforcementObservationState.ReductionObserved), Effective(Envelope(a))), T0).SignalClass); }
    private static void NormalPressureNoAction() { var a = Allocations(); Equal(TechnicalLoadSheddingSignalClass.NoAction, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, Pressure(a, 10), Effective(Envelope(a))), T0).SignalClass); }
    private static void UnavailablePressureStateUnavailable() { var a = Allocations(); Equal(TechnicalLoadSheddingSignalClass.StateUnavailable, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, Pressure(a, null), Effective(Envelope(a))), T0).SignalClass); }
    private static void WrongEffectBatchRejected() { var t = BorrowTransition(); var e = t.Before.Envelope; var other = new EffectiveDistributionMutationIntent("other", EffectiveDistributionOperationKind.Borrow, AppA, GrantA, AppB, Cpu, Q(5), null, e, e.CoordinatorInstanceId, e.CoordinatorRoleId, e.FenceGeneration, e.FencingToken, new CorrelationId("corr-other"), new CausationId("cause-other"), Evidence("other-intent"), T0.AddMinutes(-4), T0.AddMinutes(30)); Throws<InvalidOperationException>(() => AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, new ResourceEffectBatch("wrong", new[] { ResourceEffectOperation.ForEffective(other) }), AppA, Cpu)); }
    private static void EffectiveReductionCompliance() { var t = BorrowTransition(); var basis = AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); Equal(TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis), T0).SignalClass); }
    private static void FoundationReductionCompliance() { var t = FoundationReduction(); var basis = AcceptedResourceCapacityTransitionBasis.FromFoundationMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); var signal = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Accepted.AcceptedSnapshot, AppA, basis: basis, at: basis.AcceptedAt), basis.AcceptedAt); Equal(TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, signal.SignalClass); Equal(10m, signal.CompliantCapacityTarget!.Amount); }
    private static void ExactUseOnlyDrivesReductionQuantity() { var t = BorrowTransition(); var basis = AcceptedResourceCapacityTransitionBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); var without = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis), T0); Require(without.RequiredReduction is null, "Reduction fabricated without exact use."); var use = new ExactApplicationResourceUseObservation(AppA, Cpu, Epoch, Q(19), Evidence("exact-use"), T0); var withUse = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis, use: use), T0); Equal(4m, withUse.RequiredReduction!.Amount); }
    private static void AdditionalResourceDecisionNotAppliedCapacity() { var a = Allocations(); var p = Projection(a, AppA, effective: Effective(Envelope(a)), decision: DirectDecision(a)); Require(p.DecisionReference is not null, "Additional-resource decision reference missing."); Equal(20m, p.EffectiveCapacity!.Amount); }
    private static void NoApplicationBusinessOrExecutorSurface() { var types = new[] { typeof(ApplicationResourceStateProjection), typeof(AggregateResourceStateProjection), typeof(ApplicationResourceLoadSheddingSignal), typeof(ApplicationResourceLoadSheddingSignalFactory), typeof(AcceptedResourceCapacityTransitionBasis), typeof(AdditionalResourceDecisionProjectionReference) }; var names = types.SelectMany(t => new[] { t.FullName ?? t.Name }.Concat(t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => m.Name))).ToArray(); var forbidden = new[] { "FSATS", "TARC", "FSARM", "Strategy", "Broker", "Trading", "ExecuteShedding", "Authenticate", "AdmitApplication", "WP09", "Wp06", "Wp07" }; Require(forbidden.All(term => names.All(name => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "Forbidden Application/runtime/work-package surface leaked."); }

    private sealed class SuccessAdapter : IResourceEffectAdapter { public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt) => new(batch.IdentitySha256, true, false, batch.Operations.Select(x => x.OperationId), Evidence("effect", appliedAt), appliedAt); }
    private static void Run(string name, Action action) { try { action(); _passed++; Console.WriteLine($"PASS {name}"); } catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); } }
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}' but found '{actual}'."); }
    private static void Throws<T>(Action action) where T : Exception { try { action(); } catch (T) { return; } throw new InvalidOperationException($"Expected {typeof(T).Name} was not thrown."); }
}