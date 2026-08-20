using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP08.Verifier;

internal static class Program
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
        Run("zero_application_projection_set_valid", ZeroApplicationProjectionSetValid);
        Run("direct_projection_binds_allocation_truth", DirectProjectionBindsAllocationTruth);
        Run("missing_effective_truth_does_not_invent_capacity", MissingEffectiveTruthDoesNotInventCapacity);
        Run("exact_effective_distribution_projects_capacity", ExactEffectiveDistributionProjectsCapacity);
        Run("borrowed_provenance_preserved", BorrowedProvenancePreserved);
        Run("direct_projection_is_application_scoped", DirectProjectionIsApplicationScoped);
        Run("aggregate_projection_positive", AggregateProjectionPositive);
        Run("aggregate_projection_requires_exact_constituents", AggregateProjectionRequiresExactConstituents);
        Run("aggregate_projection_requires_effective_truth", AggregateProjectionRequiresEffectiveTruth);
        Run("aggregate_projection_rejects_wrong_envelope", AggregateProjectionRejectsWrongEnvelope);
        Run("critical_pressure_yields_advisory_only", CriticalPressureYieldsAdvisoryOnly);
        Run("unavailable_pressure_yields_state_unavailable", UnavailablePressureYieldsStateUnavailable);
        Run("normal_pressure_yields_no_action", NormalPressureYieldsNoAction);
        Run("enforcement_observation_does_not_mint_compliance", EnforcementObservationDoesNotMintCompliance);
        Run("exact_wp06_decision_reference_positive", ExactWp06DecisionReferencePositive);
        Run("wp06_decision_wrong_application_rejected", Wp06DecisionWrongApplicationRejected);
        Run("wp06_decision_wrong_predecessor_rejected", Wp06DecisionWrongPredecessorRejected);
        Run("wp06_decision_not_applied_capacity", Wp06DecisionNotAppliedCapacity);
        Run("accepted_effective_reduction_yields_compliance", AcceptedEffectiveReductionYieldsCompliance);
        Run("accepted_effective_return_is_not_reduction", AcceptedEffectiveReturnIsNotReduction);
        Run("accepted_foundation_reduction_yields_compliance", AcceptedFoundationReductionYieldsCompliance);
        Run("foundation_basis_without_current_effective_truth_is_time_bounded", FoundationBasisWithoutCurrentEffectiveTruthIsTimeBounded);
        Run("reduction_quantity_omitted_without_exact_use", ReductionQuantityOmittedWithoutExactUse);
        Run("reduction_quantity_derived_from_exact_use", ReductionQuantityDerivedFromExactUse);
        Run("exact_use_scope_mismatch_rejected", ExactUseScopeMismatchRejected);
        Run("utilization_is_not_reverse_engineered_into_exact_use", UtilizationIsNotReverseEngineeredIntoExactUse);
        Run("pressure_never_creates_binding_basis", PressureNeverCreatesBindingBasis);
        Run("projection_identity_is_deterministic", ProjectionIdentityIsDeterministic);
        Run("projection_identity_changes_with_decision_context", ProjectionIdentityChangesWithDecisionContext);
        Run("no_application_business_terms_in_wp08_surface", NoApplicationBusinessTermsInWp08Surface);
        Run("no_load_shedding_executor", NoLoadSheddingExecutor);
        Run("projection_scope_is_not_runtime_authentication", ProjectionScopeIsNotRuntimeAuthentication);
        Run("no_wp09_integration_surface", NoWp09IntegrationSurface);
        Run("wp05_observation_and_wp07_authority_remain_distinct", Wp05ObservationAndWp07AuthorityRemainDistinct);
        Run("wp06_decision_and_wp08_signal_remain_distinct", Wp06DecisionAndWp08SignalRemainDistinct);
        Run("effective_headroom_is_not_capacity", EffectiveHeadroomIsNotCapacity);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-08 VERIFIER: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime(DateTimeOffset? until = null)
        => until.HasValue ? new ResourceEffectiveLifetime(T0.AddHours(-1), until, false) : new ResourceEffectiveLifetime(T0.AddHours(-1), null, true);
    private static ResourceEvidenceReference Evidence(string id, ResourceEpochId? epoch = null, DateTimeOffset? at = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId("scope-" + id), at ?? T0.AddMinutes(-30), epoch ?? Epoch);

    private static FoundationResourceTruthSnapshot Truth(ResourceEpochId? epoch = null)
    {
        var e = epoch ?? Epoch;
        return new FoundationResourceTruthSnapshot(e, T0.AddMinutes(-20), new[]
        {
            new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", e, T0.AddMinutes(-21)))
        }, true);
    }

    private static ApplicationResourceAllocation Allocation(ApplicationPrincipalId app, ResourceGrantId grant, decimal allocation = 20, decimal quota = 30, decimal ceiling = 40, ResourceEpochId? epoch = null)
        => new(grant, app, Cpu, Q(allocation), Q(quota), Q(ceiling), Lifetime(), Evidence("allocation-" + app.Value, epoch));

    private static ApplicationResourceAllocationSnapshot Allocations(bool twoApps = true, decimal appAAllocation = 20)
    {
        var truth = Truth();
        var records = twoApps
            ? new[] { Allocation(AppA, GrantA, appAAllocation, 30, 40, truth.EpochId), Allocation(AppB, GrantB, 20, 30, 40, truth.EpochId) }
            : Array.Empty<ApplicationResourceAllocation>();
        return new ApplicationResourceAllocationSnapshot(truth, T0.AddMinutes(-15), records, true);
    }

    private static ResourcePriorityClassDefinition PClass(string id) => new(new ResourcePriorityClassId(id), Lifetime(), Evidence("pc-" + id));
    private static TechnicalCriticalityClassDefinition CClass(string id) => new(new TechnicalCriticalityClassId(id), Lifetime(), Evidence("cc-" + id));
    private static ResourcePriorityClassRelation PRel(string high, string low) => new(new ResourcePriorityClassId(high), new ResourcePriorityClassId(low), Lifetime(), Evidence("pr-" + high + "-" + low));
    private static TechnicalCriticalityClassRelation CRel(string high, string low) => new(new TechnicalCriticalityClassId(high), new TechnicalCriticalityClassId(low), Lifetime(), Evidence("cr-" + high + "-" + low));
    private static ApplicationResourcePriorityBinding AppBinding(ApplicationPrincipalId app, string priority) => new(app, new ResourcePriorityClassId(priority), Lifetime(), Evidence("ab-" + app.Value));
    private static TechnicalCriticalityBinding TechBinding(string scope) => new(new ResourceScopeId(scope), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-" + scope));

    private static ResourcePriorityGovernanceSnapshot PrioritySnapshot(ApplicationResourceAllocationSnapshot allocations)
        => new(allocations, T0.AddMinutes(-8),
            "priority-policy-v1", Lifetime(), Evidence("priority-policy"),
            "criticality-policy-v1", Lifetime(), Evidence("criticality-policy"),
            new[] { PClass("p-high"), PClass("p-low") }, new[] { CClass("c-high"), CClass("c-low") },
            new[] { PRel("p-high", "p-low") }, new[] { CRel("c-high", "c-low") },
            new[] { AppBinding(AppA, "p-high"), AppBinding(AppB, "p-low") },
            new[] { TechBinding("scope-a"), TechBinding("scope-b") }, true);

    private static ResourcePressureTransitionPolicy PressurePolicy()
        => new(Cpu, 6000, 8000, 9500, 500, "pressure-policy-v1", Lifetime(), Evidence("pressure-policy"));

    private static FoundationResourcePressureSnapshot Pressure(ApplicationResourceAllocationSnapshot allocations, decimal? usedA, ResourceEnforcementObservationState enforcement = ResourceEnforcementObservationState.None)
    {
        var observations = new[]
        {
            new ResourcePressureObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, usedA.HasValue ? Q(usedA.Value) : null, 1, Evidence("pressure-a"))
        };
        var eligibility = new[] { new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("reclaim-a")) };
        var enforcementItems = enforcement == ResourceEnforcementObservationState.None
            ? Array.Empty<ResourceEnforcementObservation>()
            : new[] { new ResourceEnforcementObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, enforcement, Evidence("enforcement-a")) };
        return new FoundationResourcePressureSnapshot(PrioritySnapshot(allocations), T0.AddMinutes(-5), new[] { PressurePolicy() }, observations, eligibility, enforcementItems);
    }

    private static ResourcePreemptionEligibilityBinding Binding(ApplicationPrincipalId app, ResourceGrantId grant)
        => new(grant, app, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-" + app.Value));

    private static ResourceCoordinationEnvelope Envelope(ApplicationResourceAllocationSnapshot? allocations = null, string token = "fence-1")
    {
        var a = allocations ?? Allocations();
        return new ResourceCoordinationEnvelope("envelope-authority", new ResourceScopeId("scope-coordination"), "coordinator-1", "aggregate-resource-coordinator",
            1, 1, token, a,
            new[]
            {
                new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(10), Q(10), Q(20), Binding(AppA, GrantA)),
                new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(10), Q(10), Q(20), Binding(AppB, GrantB))
            }, Evidence("envelope", a.ResourceTruth.EpochId), T0.AddMinutes(-12), T0.AddHours(1));
    }

    private static EffectiveResourceDistributionSnapshot Effective(ResourceCoordinationEnvelope envelope, IEnumerable<BorrowedEffectiveCapacitySegment>? segments = null, DateTimeOffset? at = null)
        => new(envelope.AllocationSnapshot, envelope, at ?? T0.AddMinutes(-2), segments ?? Array.Empty<BorrowedEffectiveCapacitySegment>());

    private static EffectiveDistributionMutationIntent BorrowIntent(ResourceCoordinationEnvelope envelope, string id = "borrow-1", decimal amount = 5)
        => new(id, EffectiveDistributionOperationKind.Borrow, AppA, GrantA, AppB, Cpu, Q(amount), null, envelope,
            envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id), T0.AddMinutes(-2), T0.AddMinutes(30));

    private static EffectiveDistributionMutationIntent ReturnIntent(ResourceCoordinationEnvelope envelope, string segmentId, string id = "return-1", decimal amount = 5)
        => new(id, EffectiveDistributionOperationKind.ReturnBorrowed, AppA, GrantA, AppB, Cpu, Q(amount), segmentId, envelope,
            envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id), T0.AddMinutes(-1), T0.AddMinutes(30));

    private static AcceptedEffectiveDistributionMutation BorrowOnce(ResourceMutationProcessor? processor = null, ResourceCoordinationEnvelope? envelope = null)
    {
        var e = envelope ?? Envelope();
        return (processor ?? new ResourceMutationProcessor()).ApplyEffectiveRedistribution(Effective(e), "batch-borrow", new[] { BorrowIntent(e) }, new SuccessAdapter(Epoch), T0);
    }

    private static FoundationResourceMutationAuthority MutationAuthority()
        => new("mutation-authority", new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu },
            new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1, Evidence("mutation-authority"), T0.AddHours(-1), T0.AddHours(1));

    private static AcceptedFoundationAllocationMutation ReduceFoundation(ApplicationResourceAllocationSnapshot current, decimal target = 10)
    {
        var intent = new FoundationAllocationMutationIntent("reduce-1", ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(target), Q(20), Q(30), MutationAuthority(), null,
            current.IdentitySha256, new CorrelationId("corr-reduce"), new CausationId("cause-reduce"), Evidence("reduce-intent"), T0.AddMinutes(-2), T0.AddMinutes(30));
        var quiesced = Effective(Envelope(current), Array.Empty<BorrowedEffectiveCapacitySegment>(), T0.AddMinutes(-1));
        return new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "batch-reduce", new[] { intent }, new SuccessAdapter(Epoch), T0, quiesced);
    }

    private static AdditionalResourceDecisionRecord DirectDecision(ApplicationResourceAllocationSnapshot allocations, ApplicationPrincipalId? app = null)
    {
        var target = app ?? AppA;
        var authority = new ResourceRequestAuthorityBinding("request-authority", "requester-" + target.Value, "application-resource-requester",
            new ResourceScopeId("scope-" + target.Value), new[] { target }, 1, Evidence("request-authority"), T0.AddHours(-1), T0.AddHours(1));
        var request = new AdditionalResourceRequest(new ResourceRequestId("request-" + target.Value), ResourceRequesterKind.DirectApplication,
            "requester-" + target.Value, "application-resource-requester", authority, target, null, Cpu, Q(5), Q(5), allocations, null,
            new CorrelationId("corr-request-" + target.Value), new CausationId("cause-request-" + target.Value), Evidence("request-" + target.Value), Evidence("residual-" + target.Value),
            T0.AddMinutes(-4), T0.AddMinutes(30));
        var policy = new ResourceAdditionalRequestDecisionPolicy(Cpu, Q(10), false, Evidence("decision-policy"), T0.AddHours(-1), T0.AddHours(1));
        var decisionAuthority = new ResourceRequestDecisionAuthority("decision-authority", Evidence("decision-authority"), T0.AddHours(-1), T0.AddHours(1));
        return new AdditionalResourceRequestDecisionProcessor(new[] { policy }, decisionAuthority).Evaluate(request, new ResourceDecisionId("decision-" + target.Value), T0.AddMinutes(-3));
    }

    private static ApplicationResourceStateProjection Projection(ApplicationResourceAllocationSnapshot allocations, ApplicationPrincipalId app,
        FoundationResourcePressureSnapshot? pressure = null, EffectiveResourceDistributionSnapshot? effective = null,
        AdditionalResourceDecisionRecord? decision = null, Wp07AcceptedCapacityBasis? basis = null, ExactApplicationResourceUseObservation? use = null,
        DateTimeOffset? at = null)
        => ApplicationResourceStateProjectionBuilder.CreateDirect(allocations, app, Cpu, at ?? T0, pressure, effective, decision, basis, use);

    private static void ZeroApplicationProjectionSetValid()
    {
        var set = new ApplicationResourceStateProjectionSet(Epoch, T0, Array.Empty<ApplicationResourceStateProjection>());
        Equal(0, set.Projections.Count);
    }

    private static void DirectProjectionBindsAllocationTruth()
    {
        var a = Allocations();
        var p = Projection(a, AppA);
        Equal(GrantA.Value, p.GrantId.Value);
        Equal(20m, p.Allocation.Amount);
        Equal(30m, p.Quota.Amount);
        Equal(40m, p.Ceiling.Amount);
    }

    private static void MissingEffectiveTruthDoesNotInventCapacity()
        => Require(!Projection(Allocations(), AppA).EffectiveCapacityAvailable, "Missing WP-07 effective truth must not be converted into invented effective capacity.");

    private static void ExactEffectiveDistributionProjectsCapacity()
    {
        var e = Envelope();
        var p = Projection(e.AllocationSnapshot, AppA, effective: Effective(e));
        Require(p.EffectiveCapacityAvailable, "Exact effective distribution should make effective capacity available.");
        Equal(20m, p.EffectiveCapacity!.Amount);
    }

    private static void BorrowedProvenancePreserved()
    {
        var accepted = BorrowOnce();
        var p = Projection(accepted.AcceptedSnapshot.AuthoritativeAllocationSnapshot, AppA, effective: accepted.AcceptedSnapshot);
        var segment = p.BorrowedProvenance.Single();
        Equal(AppA.Value, segment.SourceApplicationId.Value);
        Equal(GrantA.Value, segment.SourceGrantId.Value);
        Equal(AppB.Value, segment.TargetApplicationId.Value);
    }

    private static void DirectProjectionIsApplicationScoped()
    {
        var a = Allocations();
        var set = new ApplicationResourceStateProjectionSet(Epoch, T0, new[] { Projection(a, AppA), Projection(a, AppB) });
        var view = set.GetApplicationView(AppA);
        Require(view.Count == 1 && view.All(item => item.ApplicationId.Value == AppA.Value), "Direct Application view leaked another Application.");
    }

    private static void AggregateProjectionPositive()
    {
        var e = Envelope();
        var effective = Effective(e);
        var aggregate = new AggregateResourceStateProjection(e, T0, new[]
        {
            Projection(e.AllocationSnapshot, AppA, effective: effective),
            Projection(e.AllocationSnapshot, AppB, effective: effective)
        });
        Equal(2, aggregate.Constituents.Count);
        Equal(e.IdentitySha256, aggregate.Envelope.IdentitySha256);
    }

    private static void AggregateProjectionRequiresExactConstituents()
    {
        var e = Envelope();
        var effective = Effective(e);
        Throws<InvalidOperationException>(() => new AggregateResourceStateProjection(e, T0, new[] { Projection(e.AllocationSnapshot, AppA, effective: effective) }));
    }

    private static void AggregateProjectionRequiresEffectiveTruth()
    {
        var e = Envelope();
        Throws<InvalidOperationException>(() => new AggregateResourceStateProjection(e, T0, new[] { Projection(e.AllocationSnapshot, AppA), Projection(e.AllocationSnapshot, AppB) }));
    }

    private static void AggregateProjectionRejectsWrongEnvelope()
    {
        var e1 = Envelope(token: "fence-1");
        var e2 = Envelope(e1.AllocationSnapshot, "fence-2");
        var effective = Effective(e1);
        Throws<InvalidOperationException>(() => new AggregateResourceStateProjection(e2, T0, new[]
        {
            Projection(e1.AllocationSnapshot, AppA, effective: effective), Projection(e1.AllocationSnapshot, AppB, effective: effective)
        }));
    }

    private static void CriticalPressureYieldsAdvisoryOnly()
    {
        var a = Allocations();
        var p = Projection(a, AppA, pressure: Pressure(a, 39));
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(p, T0);
        Equal(TechnicalLoadSheddingSignalClass.AdvisoryReduction, signal.SignalClass);
        Require(signal.AcceptedWp07BasisIdentitySha256 is null, "Pressure may not mint WP-07 authority basis.");
    }

    private static void UnavailablePressureYieldsStateUnavailable()
    {
        var a = Allocations();
        Equal(TechnicalLoadSheddingSignalClass.StateUnavailable,
            ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, pressure: Pressure(a, null)), T0).SignalClass);
    }

    private static void NormalPressureYieldsNoAction()
    {
        var a = Allocations();
        Equal(TechnicalLoadSheddingSignalClass.NoAction,
            ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, pressure: Pressure(a, 10)), T0).SignalClass);
    }

    private static void EnforcementObservationDoesNotMintCompliance()
    {
        var a = Allocations();
        var p = Projection(a, AppA, pressure: Pressure(a, 39, ResourceEnforcementObservationState.ReductionObserved));
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(p, T0);
        Require(signal.SignalClass != TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, "WP-05 enforcement observation minted binding compliance authority.");
    }

    private static void ExactWp06DecisionReferencePositive()
    {
        var a = Allocations();
        var decision = DirectDecision(a);
        var p = Projection(a, AppA, decision: decision);
        Equal(decision.IdentitySha256, p.DecisionReference!.DecisionIdentitySha256);
    }

    private static void Wp06DecisionWrongApplicationRejected()
    {
        var a = Allocations();
        Throws<InvalidOperationException>(() => Projection(a, AppB, decision: DirectDecision(a, AppA)));
    }

    private static void Wp06DecisionWrongPredecessorRejected()
    {
        var a1 = Allocations();
        var a2 = new ApplicationResourceAllocationSnapshot(a1.ResourceTruth, a1.ObservedAt.AddSeconds(1), a1.Allocations, true);
        var decision = DirectDecision(a1);
        Throws<InvalidOperationException>(() => Projection(a2, AppA, decision: decision));
    }

    private static void Wp06DecisionNotAppliedCapacity()
    {
        var a = Allocations();
        var p = Projection(a, AppA, decision: DirectDecision(a));
        Require(!p.EffectiveCapacityAvailable, "WP-06 decision was incorrectly treated as applied capacity.");
        Equal(20m, p.Allocation.Amount);
    }

    private static void AcceptedEffectiveReductionYieldsCompliance()
    {
        var e = Envelope();
        var before = Effective(e);
        var accepted = new ResourceMutationProcessor().ApplyEffectiveRedistribution(before, "batch-borrow-x", new[] { BorrowIntent(e, "borrow-x") }, new SuccessAdapter(Epoch), T0);
        var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(before, accepted, AppA, Cpu);
        var p = Projection(e.AllocationSnapshot, AppA, effective: accepted.AcceptedSnapshot, basis: basis);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(p, T0);
        Equal(TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, signal.SignalClass);
        Equal(15m, signal.CompliantCapacityTarget!.Amount);
    }

    private static void AcceptedEffectiveReturnIsNotReduction()
    {
        var processor = new ResourceMutationProcessor();
        var borrowed = BorrowOnce(processor);
        var segment = borrowed.AcceptedSnapshot.BorrowedSegments.Single();
        var returned = processor.ApplyEffectiveRedistribution(borrowed.AcceptedSnapshot, "batch-return-x", new[] { ReturnIntent(borrowed.AcceptedSnapshot.Envelope, segment.SegmentId, "return-x") }, new SuccessAdapter(Epoch), T0.AddMinutes(1));
        var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(borrowed.AcceptedSnapshot, returned, AppA, Cpu);
        Require(!basis.IsReduction && basis.IsRestorationOrIncrease, "Return of lent capacity should be an increase/restoration for source Application.");
    }

    private static void AcceptedFoundationReductionYieldsCompliance()
    {
        var current = Allocations();
        var accepted = ReduceFoundation(current);
        var basis = Wp07AcceptedCapacityBasis.FromFoundationMutation(current, accepted, AppA, Cpu);
        var p = Projection(accepted.AcceptedSnapshot, AppA, basis: basis, at: basis.AcceptedAt);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(p, basis.AcceptedAt);
        Equal(TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, signal.SignalClass);
        Equal(10m, signal.CompliantCapacityTarget!.Amount);
    }

    private static void FoundationBasisWithoutCurrentEffectiveTruthIsTimeBounded()
    {
        var current = Allocations();
        var accepted = ReduceFoundation(current);
        var basis = Wp07AcceptedCapacityBasis.FromFoundationMutation(current, accepted, AppA, Cpu);
        Throws<InvalidOperationException>(() => Projection(accepted.AcceptedSnapshot, AppA, basis: basis, at: basis.AcceptedAt.AddSeconds(1)));
    }

    private static void ReductionQuantityOmittedWithoutExactUse()
    {
        var e = Envelope();
        var before = Effective(e);
        var accepted = new ResourceMutationProcessor().ApplyEffectiveRedistribution(before, "batch-borrow-r", new[] { BorrowIntent(e, "borrow-r") }, new SuccessAdapter(Epoch), T0);
        var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(before, accepted, AppA, Cpu);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(e.AllocationSnapshot, AppA, effective: accepted.AcceptedSnapshot, basis: basis), T0);
        Require(signal.RequiredReduction is null, "Reduction quantity must be omitted without exact observed use.");
    }

    private static void ReductionQuantityDerivedFromExactUse()
    {
        var e = Envelope();
        var before = Effective(e);
        var accepted = new ResourceMutationProcessor().ApplyEffectiveRedistribution(before, "batch-borrow-u", new[] { BorrowIntent(e, "borrow-u") }, new SuccessAdapter(Epoch), T0);
        var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(before, accepted, AppA, Cpu);
        var use = new ExactApplicationResourceUseObservation(AppA, Cpu, Epoch, Q(19), Evidence("exact-use"), T0);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(e.AllocationSnapshot, AppA, effective: accepted.AcceptedSnapshot, basis: basis, use: use), T0);
        Equal(4m, signal.RequiredReduction!.Amount);
    }

    private static void ExactUseScopeMismatchRejected()
    {
        var a = Allocations();
        var use = new ExactApplicationResourceUseObservation(AppB, Cpu, Epoch, Q(10), Evidence("wrong-use"), T0);
        Throws<InvalidOperationException>(() => Projection(a, AppA, use: use));
    }

    private static void UtilizationIsNotReverseEngineeredIntoExactUse()
    {
        var e = Envelope();
        var before = Effective(e);
        var accepted = new ResourceMutationProcessor().ApplyEffectiveRedistribution(before, "batch-borrow-p", new[] { BorrowIntent(e, "borrow-p") }, new SuccessAdapter(Epoch), T0);
        var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(before, accepted, AppA, Cpu);
        var p = Projection(e.AllocationSnapshot, AppA, pressure: Pressure(e.AllocationSnapshot, 19), effective: accepted.AcceptedSnapshot, basis: basis);
        var signal = ApplicationResourceLoadSheddingSignalFactory.Create(p, T0);
        Require(signal.RequiredReduction is null, "Rounded utilization basis points were reverse-engineered into fabricated exact use.");
    }

    private static void PressureNeverCreatesBindingBasis()
    {
        var a = Allocations();
        var p = Projection(a, AppA, pressure: Pressure(a, 39));
        Require(p.AcceptedCapacityBasis is null, "Pressure created an accepted capacity basis.");
    }

    private static void ProjectionIdentityIsDeterministic()
    {
        var a = Allocations();
        var e = Effective(Envelope(a));
        var p1 = Projection(a, AppA, effective: e);
        var p2 = Projection(a, AppA, effective: e);
        Equal(p1.IdentitySha256, p2.IdentitySha256);
    }

    private static void ProjectionIdentityChangesWithDecisionContext()
    {
        var a = Allocations();
        var p1 = Projection(a, AppA);
        var p2 = Projection(a, AppA, decision: DirectDecision(a));
        Require(!StringComparer.Ordinal.Equals(p1.IdentitySha256, p2.IdentitySha256), "Decision context did not alter projection identity.");
    }

    private static void NoApplicationBusinessTermsInWp08Surface()
    {
        var names = Wp08Types().Select(type => type.FullName ?? type.Name).ToArray();
        var forbidden = new[] { "Trading", "FSATS", "TARC", "FSARM", "Strategy", "Broker", "Market", "Guardian", "Simulator" };
        Require(names.All(name => forbidden.All(term => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "Application business term leaked into generic WP-08 public surface.");
    }

    private static void NoLoadSheddingExecutor()
    {
        var publicMethods = Wp08Types().SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)).Where(method => !method.IsSpecialName).ToArray();
        Require(publicMethods.All(method => !method.Name.Contains("Stop", StringComparison.OrdinalIgnoreCase) && !method.Name.Contains("Kill", StringComparison.OrdinalIgnoreCase) && !method.Name.Contains("Disable", StringComparison.OrdinalIgnoreCase) && !method.Name.Contains("Shed", StringComparison.OrdinalIgnoreCase)),
            "WP-08 public surface contains an Application load-shedding executor.");
    }

    private static void ProjectionScopeIsNotRuntimeAuthentication()
    {
        var names = Wp08Types().SelectMany(type => type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)).Select(member => member.Name).ToArray();
        Require(names.All(name => !name.Contains("Authenticate", StringComparison.OrdinalIgnoreCase) && !name.Contains("Login", StringComparison.OrdinalIgnoreCase) && !name.Contains("Session", StringComparison.OrdinalIgnoreCase) && !name.Contains("Admission", StringComparison.OrdinalIgnoreCase)),
            "WP-08 projection scope leaked runtime authentication/admission semantics.");
    }

    private static void NoWp09IntegrationSurface()
        => Require(Wp08Types().All(type => !(type.FullName ?? type.Name).Contains("Integration", StringComparison.OrdinalIgnoreCase) && !(type.FullName ?? type.Name).Contains("Hardening", StringComparison.OrdinalIgnoreCase)), "WP-09 integration/hardening leaked into WP-08.");

    private static void Wp05ObservationAndWp07AuthorityRemainDistinct()
        => Require(typeof(ResourcePressureTruth) != typeof(Wp07AcceptedCapacityBasis), "WP-05 observation truth and WP-07 accepted capacity basis collapsed into one type.");

    private static void Wp06DecisionAndWp08SignalRemainDistinct()
        => Require(typeof(AdditionalResourceDecisionRecord) != typeof(ApplicationResourceLoadSheddingSignal), "WP-06 resource decision and WP-08 load-shedding signal collapsed into one authority surface.");

    private static void EffectiveHeadroomIsNotCapacity()
    {
        var a = Allocations();
        var p = Projection(a, AppA);
        Require(!p.EffectiveCapacityAvailable, "Quota/ceiling headroom was treated as current effective capacity without WP-07 truth.");
        Equal(40m, p.Ceiling.Amount);
    }

    private static Type[] Wp08Types() => new[]
    {
        typeof(ResourceCapacityBasisLane), typeof(TechnicalLoadSheddingSignalClass), typeof(ExactApplicationResourceUseObservation),
        typeof(Wp07AcceptedCapacityBasis), typeof(Wp06DecisionProjectionReference), typeof(ApplicationResourceStateProjection),
        typeof(ApplicationResourceStateProjectionSet), typeof(AggregateResourceStateProjection), typeof(ApplicationResourceStateProjectionBuilder),
        typeof(ApplicationResourceLoadSheddingSignal), typeof(ApplicationResourceLoadSheddingSignalFactory)
    };

    private sealed class SuccessAdapter : IResourceEffectAdapter
    {
        private readonly ResourceEpochId _epoch;
        public SuccessAdapter(ResourceEpochId epoch) => _epoch = epoch;
        public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt)
            => new(batch.IdentitySha256, true, false, batch.Operations.Select(item => item.OperationId), Evidence("effect", _epoch, appliedAt), appliedAt);
    }

    private static void Run(string name, Action test)
    {
        try { test(); _passed++; Console.WriteLine($"PASS {name}"); }
        catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}' but got '{actual}'.");
    }

    private static void Throws<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException($"Expected exception {typeof(TException).Name}.");
    }
}